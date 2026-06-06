using System.Net.Http.Json;
using HotelOS.ReceptionService.DTOs;
using HotelOS.ReceptionService.Models;
using HotelOS.ReceptionService.Repositories;
using HotelOS.Shared.Algorithms;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;
using HotelOS.Shared.Audit;

namespace HotelOS.ReceptionService.Services;

public sealed class ReceptionService(
    IBookingRepository bookingRepository,
    IGuestRepository guestRepository,
    IEventPublisher eventPublisher,
    IAuditLogger auditLogger,
    IHttpClientFactory httpClientFactory) : IReceptionService, IReceptionQueries, IReceptionCommands
{
    public async Task<BookingResponseDto> CheckInAsync(CheckInRequestDto request, CancellationToken cancellationToken = default)
    {
        var guest = await GetOrCreateGuestAsync(request.GuestName, request.Email, cancellationToken);

        var roomId = await AssignRoomAsync(request.Adults + request.Kids, cancellationToken);

        var booking = await bookingRepository.AddAsync(new Booking
        {
            GuestId = guest.Id,
            RoomId = roomId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Status = "Confirmed"
        }, cancellationToken);

        try
        {
            var roomClient = httpClientFactory.CreateClient("room-service");
            await roomClient.PatchAsync($"api/room/rooms/{roomId}/status",
                JsonContent.Create(new { status = "Occupied" }), cancellationToken);
        }
        catch { }

        eventPublisher.Publish(RabbitMqRoutingKeys.BookingCreated, new
        {
            BookingId = booking.Id,
            booking.RoomId,
            request.GuestName,
            request.Email,
            OccurredAt = DateTimeOffset.UtcNow
        });

        auditLogger.Log("System", "Confirmed Booking", $"Booking #{booking.Id}", $"Guest {request.GuestName} → Room {roomId}");

        return new BookingResponseDto(booking.Id, booking.RoomId, booking.Status, request.GuestName);
    }

    public async Task<BookingResponseDto> HoldReservationAsync(HoldReservationDto request, CancellationToken cancellationToken = default)
    {
        var guest = await guestRepository.AddAsync(new Guest
        {
            FullName = "Reservation Hold",
            Email = $"hold-{Guid.NewGuid():N}@grandstay.local"
        }, cancellationToken);

        var booking = await bookingRepository.AddAsync(new Booking
        {
            GuestId = guest.Id,
            RoomId = request.RoomId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Status = "HELD"
        }, cancellationToken);

        try
        {
            var roomClient = httpClientFactory.CreateClient("room-service");
            await roomClient.PatchAsync($"api/room/rooms/{request.RoomId}/status",
                JsonContent.Create(new { status = "HELD" }), cancellationToken);
        }
        catch { }

        eventPublisher.Publish(RabbitMqRoutingKeys.BookingCreated, new
        {
            BookingId = booking.Id,
            booking.RoomId,
            request.RoomNumber,
            Status = "HELD",
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
            OccurredAt = DateTimeOffset.UtcNow
        });

        auditLogger.Log("Guest", "Held Reservation", $"Booking #{booking.Id}", $"Room {request.RoomNumber}, {request.CheckInDate} → {request.CheckOutDate}");

        return new BookingResponseDto(booking.Id, booking.RoomId, "HELD", "Guest");
    }

    public async Task<BookingResponseDto> CheckOutAsync(CheckOutRequestDto request, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
            ?? throw new InvalidOperationException("Booking not found");

        var guest = await guestRepository.GetByIdAsync(booking.GuestId, cancellationToken);

        var nights = (booking.CheckOutDate.ToDateTime(TimeOnly.MinValue) - booking.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
        if (nights <= 0) nights = 1;

        booking.Status = "CheckedOut";
        await bookingRepository.SaveChangesAsync(cancellationToken);

        var roomNumber = $"Room {booking.RoomId}";
        var roomNightsTotal = 150m * nights;
        var foodOrdersTotal = 0m;

        try
        {
            var roomClient = httpClientFactory.CreateClient("room-service");
            var roomInfo = await roomClient.GetFromJsonAsync<RoomInfo>($"api/room/rooms/{booking.RoomId}", cancellationToken);
            if (roomInfo is not null)
            {
                roomNumber = roomInfo.RoomNumber;
                roomNightsTotal = roomInfo.PricePerNight * nights;
            }

            var orders = await roomClient.GetFromJsonAsync<List<OrderInfo>>("api/room/orders", cancellationToken);
            if (orders is not null)
            {
                foodOrdersTotal = orders
                    .Where(order => string.Equals(order.RoomNumber, roomNumber, StringComparison.OrdinalIgnoreCase))
                    .Sum(order => order.Total);
            }

            await roomClient.PatchAsync($"api/room/rooms/{booking.RoomId}/status",
                JsonContent.Create(new { status = "Dirty" }), cancellationToken);
        }
        catch { }

        try
        {
            var paymentClient = httpClientFactory.CreateClient("payment-service");
            var invoiceNumber = $"INV-{DateTimeOffset.UtcNow:yyyyMMdd}-{booking.Id:D4}";
            await paymentClient.PostAsJsonAsync("api/payments/invoice", new
            {
                InvoiceNumber = invoiceNumber,
                GuestName = guest?.FullName ?? $"Guest {booking.GuestId}",
                RoomNumber = roomNumber,
                TotalAmount = roomNightsTotal + foodOrdersTotal,
                RoomNightsTotal = roomNightsTotal,
                FoodOrdersTotal = foodOrdersTotal,
                MinibarTotal = 0m,
                DamagesTotal = 0m,
                DiscountsTotal = 0m
            }, cancellationToken);
        }
        catch { }

        eventPublisher.Publish(RabbitMqRoutingKeys.GuestCheckedOut, new
        {
            request.BookingId,
            booking.RoomId,
            OccurredAt = DateTimeOffset.UtcNow
        });

        auditLogger.Log("Receptionist", "Checked Out", $"Booking #{request.BookingId}", $"Room {booking.RoomId} vacated, {nights} nights");

        return new BookingResponseDto(booking.Id, booking.RoomId, booking.Status, guest?.FullName ?? "Guest");
    }

    public async Task<IReadOnlyList<BookingRecordDto>> GetBookingsAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository.GetAllAsync(cancellationToken);
        var guests = await guestRepository.GetAllAsync(cancellationToken);
        return bookings.Select(booking =>
        {
            var guest = guests.FirstOrDefault(g => g.Id == booking.GuestId);
            var nights = CalculateNights(booking.CheckInDate, booking.CheckOutDate);
            return new BookingRecordDto(
                booking.Id,
                guest?.FullName ?? $"Guest {booking.GuestId}",
                booking.RoomId.ToString(),
                booking.Status,
                booking.CheckInDate.ToString("yyyy-MM-dd"),
                booking.CheckOutDate.ToString("yyyy-MM-dd"),
                nights * 150m);
        }).ToList();
    }

    public async Task<IReadOnlyList<GuestRecordDto>> GetGuestsAsync(CancellationToken cancellationToken = default)
    {
        var guests = await guestRepository.GetAllAsync(cancellationToken);
        var bookings = await bookingRepository.GetAllAsync(cancellationToken);
        return guests.Select(g =>
        {
            var booking = bookings.FirstOrDefault(b => b.GuestId == g.Id && b.Status != "CheckedOut");
            var nights = booking is null ? 0 : CalculateNights(booking.CheckInDate, booking.CheckOutDate);
            return new GuestRecordDto(
                g.Id,
                g.FullName,
                booking?.RoomId.ToString() ?? "",
                booking?.CheckInDate.ToString("yyyy-MM-dd") ?? "",
                booking?.CheckOutDate.ToString("yyyy-MM-dd") ?? "",
                nights * 150m);
        }).ToList();
    }

    public async Task<IReadOnlyList<MyReservationDto>> GetReservationsByEmailAsync(string? email, CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository.GetAllAsync(cancellationToken);
        var guests = await guestRepository.GetAllAsync(cancellationToken);

        var filtered = string.IsNullOrWhiteSpace(email)
            ? bookings
            : bookings.Where(b => guests.Any(g => g.Id == b.GuestId && g.Email.Contains(email, StringComparison.OrdinalIgnoreCase)));

        return filtered.Select(b =>
        {
            var nights = (b.CheckOutDate.ToDateTime(TimeOnly.MinValue) - b.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
            if (nights <= 0) nights = 1;
            var guest = guests.FirstOrDefault(g => g.Id == b.GuestId);
            return new MyReservationDto(b.Id, $"Room {b.RoomId}", b.Status, b.CheckInDate.ToString("yyyy-MM-dd"), b.CheckOutDate.ToString("yyyy-MM-dd"), nights, nights * 150m);
        }).ToList();
    }

    public async Task<GuestDto> CreateGuestAsync(CreateGuestDto request, CancellationToken cancellationToken = default)
    {
        var guest = await GetOrCreateGuestAsync(request.FullName, request.Email, cancellationToken);

        return new GuestDto(guest.Id, guest.FullName, guest.Email);
    }

    public async Task<IReadOnlyList<ExpiredBookingDto>> GetExpiredBookingsAsync(string status, int minutes, CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository.GetExpiredAsync(status, minutes, cancellationToken);
        return bookings.Select(b => new ExpiredBookingDto(b.Id, b.RoomId, $"Guest {b.GuestId}", $"Room {b.RoomId}", b.CreatedAt)).ToList();
    }

    public async Task<BookingResponseDto> PatchBookingStatusAsync(int bookingId, string status, CancellationToken cancellationToken = default)
    {
        if (string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return await CancelBookingAsync(bookingId, cancellationToken);
        }

        await bookingRepository.PatchStatusAsync(bookingId, status, cancellationToken);
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new InvalidOperationException("Booking not found");
        return new BookingResponseDto(booking.Id, booking.RoomId, booking.Status, $"Guest {booking.GuestId}");
    }

    public async Task<BookingResponseDto> CancelBookingAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new InvalidOperationException("Booking not found");

        if (string.Equals(booking.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return new BookingResponseDto(booking.Id, booking.RoomId, booking.Status, $"Guest {booking.GuestId}");
        }

        var guest = await guestRepository.GetByIdAsync(booking.GuestId, cancellationToken);
        var nights = CalculateNights(booking.CheckInDate, booking.CheckOutDate);
        var refundAmount = Math.Max(0m, nights * 150m * 0.5m);

        booking.Status = "Cancelled";
        await bookingRepository.SaveChangesAsync(cancellationToken);

        try
        {
            var roomClient = httpClientFactory.CreateClient("room-service");
            await roomClient.PatchAsync($"api/room/rooms/{booking.RoomId}/status",
                JsonContent.Create(new { status = "Available" }), cancellationToken);
        }
        catch { }

        try
        {
            var paymentClient = httpClientFactory.CreateClient("payment-service");
            await paymentClient.PostAsJsonAsync("api/payments/refund", new
            {
                paymentId = booking.Id,
                amount = refundAmount,
                reason = "Booking cancelled"
            }, cancellationToken);
        }
        catch { }

        eventPublisher.Publish(RabbitMqRoutingKeys.BookingCancelled, new
        {
            BookingId = booking.Id,
            booking.RoomId,
            RefundAmount = refundAmount,
            OccurredAt = DateTimeOffset.UtcNow
        });

        auditLogger.Log("Receptionist", "Cancelled Booking", $"Booking #{booking.Id}", $"Room {booking.RoomId}, refunded {refundAmount:C}");

        return new BookingResponseDto(booking.Id, booking.RoomId, booking.Status, guest?.FullName ?? $"Guest {booking.GuestId}");
    }

    public async Task<BookingResponseDto> UpdateBookingAsync(int bookingId, UpdateBookingDto request, CancellationToken cancellationToken = default)
    {
        if (string.Equals(request.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return await CancelBookingAsync(bookingId, cancellationToken);
        }

        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new InvalidOperationException("Booking not found");
        booking.Status = request.Status;
        booking.CheckInDate = request.CheckInDate;
        booking.CheckOutDate = request.CheckOutDate;
        await bookingRepository.UpdateAsync(booking, cancellationToken);
        return new BookingResponseDto(booking.Id, booking.RoomId, booking.Status, request.GuestName);
    }

    public async Task DeleteBookingAsync(int bookingId, CancellationToken cancellationToken = default)
        => await bookingRepository.DeleteAsync(bookingId, cancellationToken);

    public async Task<GuestDto> UpdateGuestAsync(int guestId, UpdateGuestDto request, CancellationToken cancellationToken = default)
    {
        var guest = await guestRepository.GetByIdAsync(guestId, cancellationToken)
            ?? throw new InvalidOperationException("Guest not found");
        guest.FullName = request.FullName;
        guest.Email = request.Email;
        await guestRepository.UpdateAsync(guest, cancellationToken);
        return new GuestDto(guest.Id, guest.FullName, guest.Email);
    }

    public async Task DeleteGuestAsync(int guestId, CancellationToken cancellationToken = default)
        => await guestRepository.DeleteAsync(guestId, cancellationToken);

    private async Task<int> AssignRoomAsync(int guestCount, CancellationToken cancellationToken)
    {
        try
        {
            var preferredType = guestCount switch
            {
                1 => "Single",
                2 => "Double",
                _ => "Suite"
            };

            var client = httpClientFactory.CreateClient("room-service");
            var candidates = await client.GetFromJsonAsync<List<RoomCandidate>>(
                $"api/room/candidates?guests={guestCount}&preferredType={Uri.EscapeDataString(preferredType)}",
                cancellationToken);

            if (candidates is { Count: > 0 })
            {
                var ranked = RoomAssignmentAlgorithm.RankAvailableRooms(candidates);
                var best = ranked.FirstOrDefault();
                if (best is not null)
                    return best.RoomId;
            }
        }
        catch { }

        return guestCount <= 2 ? 101 : guestCount <= 4 ? 205 : 301;
    }

    private async Task<Guest> GetOrCreateGuestAsync(string fullName, string email, CancellationToken cancellationToken)
    {
        var existing = await guestRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
        {
            if (!string.IsNullOrWhiteSpace(fullName) &&
                !string.Equals(existing.FullName, fullName, StringComparison.Ordinal))
            {
                existing.FullName = fullName;
                await guestRepository.UpdateAsync(existing, cancellationToken);
            }

            return existing;
        }

        return await guestRepository.AddAsync(new Guest
        {
            FullName = fullName,
            Email = email
        }, cancellationToken);
    }

    private static int CalculateNights(DateOnly checkInDate, DateOnly checkOutDate)
    {
        var nights = (checkOutDate.ToDateTime(TimeOnly.MinValue) - checkInDate.ToDateTime(TimeOnly.MinValue)).Days;
        return nights <= 0 ? 1 : nights;
    }

    private sealed record RoomInfo(string RoomNumber, decimal PricePerNight);
    private sealed record OrderInfo(string RoomNumber, decimal Total);
}
