using HotelOS.RoomService.DTOs;
using HotelOS.RoomService.Events;
using HotelOS.RoomService.Models;
using HotelOS.RoomService.Repositories;
using HotelOS.Shared.Algorithms;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;
using HotelOS.Shared.Audit;

namespace HotelOS.RoomService.Services;

public sealed class RoomService(IRoomRepository roomRepository, IOrderRepository orderRepository, IEventPublisher eventPublisher, IAuditLogger auditLogger) : IRoomService, IRoomQueries, IRoomCommands
{
    public Task<IReadOnlyList<MenuItemDto>> GetMenuAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<MenuItemDto>>([
            new(1, "Breakfast set", 18),
            new(2, "Club sandwich", 14),
            new(3, "Sparkling water", 4)
        ]);

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto request, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.AddAsync(new FoodOrder
        {
            RoomNumber = request.RoomNumber,
            GuestName = request.GuestName,
            Status = "Preparing",
            Total = request.Items.Length * 12
        }, cancellationToken);

        eventPublisher.Publish(RabbitMqRoutingKeys.OrderCreated, new
        {
            order.Id,
            order.RoomNumber,
            order.GuestName,
            order.Status,
            order.Total,
            OccurredAt = DateTimeOffset.UtcNow
        });

        _ = new OrderCreatedEvent(order.Id, order.RoomNumber, DateTimeOffset.UtcNow);
        return new OrderDto(order.Id, order.RoomNumber, order.GuestName, order.Status, order.Total);
    }

    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default)
        => (await orderRepository.GetAllAsync(cancellationToken)).Select(order => new OrderDto(order.Id, order.RoomNumber, order.GuestName, order.Status, order.Total)).ToList();

    private static RoomDto MapToDto(Room r) => new(r.Id, r.RoomNumber, r.Type.ToString(), r.Status, r.PricePerNight, r.Floor, r.Description, r.GuestCapacity, r.MainImage, r.Images, r.Amenities.Select(a => a.Name).ToArray());

    public async Task<IReadOnlyList<RoomDto>> GetRoomsAsync(CancellationToken cancellationToken = default)
        => (await roomRepository.GetAllAsync(cancellationToken))
            .Select(MapToDto)
            .ToList();

    public async Task<RoomDto> GetRoomByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Room not found");
        return MapToDto(room);
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomDto request, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<RoomType>(request.Type, ignoreCase: true, out var roomType))
            throw new InvalidOperationException("Room type must be Single, Double, Suite, or Accessible.");

        var room = RoomFactory.Create(roomType, request.RoomNumber, request.Floor);
        room.PricePerNight = request.PricePerNight;
        room.GuestCapacity = request.GuestCapacity;
        room.Description = request.Description;
        room.MainImage = request.MainImage;
        room.Images = request.Images ?? Array.Empty<string>();
        room.Status = request.Status ?? "Available";
        var amenities = await roomRepository.GetAmenitiesByIdsAsync(request.AmenityIds, cancellationToken);
        foreach (var a in amenities) room.Amenities.Add(a);
        var created = await roomRepository.AddAsync(room, cancellationToken);
        auditLogger.Log("Admin", "Created Room", $"Room #{created.RoomNumber}", $"Type: {created.Type}, Floor: {created.Floor}");
        return MapToDto(created);
    }

    public async Task<RoomDto> UpdateRoomAsync(int id, UpdateRoomDto request, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Room not found");

        room.RoomNumber = request.RoomNumber;
        room.Type = Enum.Parse<RoomType>(request.Type);
        room.Status = request.Status;
        room.PricePerNight = request.PricePerNight;
        room.Floor = request.Floor;
        room.Description = request.Description;
        room.GuestCapacity = request.GuestCapacity;
        room.MainImage = request.MainImage;
        room.Images = request.Images ?? Array.Empty<string>();

        var updated = await roomRepository.UpdateAsync(room, cancellationToken);
        return MapToDto(updated);
    }

    public async Task<RoomDto> PatchRoomStatusAsync(int id, PatchRoomStatusDto request, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Room not found");
        room.Status = request.Status;
        var updated = await roomRepository.UpdateAsync(room, cancellationToken);
        return MapToDto(updated);
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto request, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Order not found");
        order.Status = request.Status;
        await orderRepository.SaveChangesAsync(cancellationToken);

        eventPublisher.Publish(RabbitMqRoutingKeys.OrderDelivered, new
        {
            order.Id,
            order.RoomNumber,
            order.GuestName,
            order.Status,
            OccurredAt = DateTimeOffset.UtcNow
        });

        return new OrderDto(order.Id, order.RoomNumber, order.GuestName, order.Status, order.Total);
    }

    public async Task DeleteRoomAsync(int id, CancellationToken cancellationToken = default)
        => await roomRepository.DeleteAsync(id, cancellationToken);

    public async Task DeleteOrderAsync(int id, CancellationToken cancellationToken = default)
        => await orderRepository.DeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<RoomOverviewDto>> GetRoomsOverviewAsync(CancellationToken cancellationToken = default)
        => (await roomRepository.GetAllAsync(cancellationToken))
            .Select(r => new RoomOverviewDto(r.Id, r.RoomNumber, r.Status, "Vacant", r.Status == "Available" ? "Clean" : "Pending"))
            .ToList();

    public async Task<IReadOnlyList<AmenityDto>> GetAmenitiesAsync(CancellationToken cancellationToken = default)
    {
        var amenities = await roomRepository.GetAllAmenitiesAsync(cancellationToken);
        return amenities.Select(a => new AmenityDto(a.Id, a.Name, a.IconUrl, a.Description)).ToList();
    }

    public async Task<IReadOnlyList<RoomCandidate>> GetAvailableRoomCandidatesAsync(int guests = 0, string? preferredRoomType = null, CancellationToken cancellationToken = default)
    {
        var rooms = await roomRepository.GetAllAsync(cancellationToken);
        preferredRoomType = string.IsNullOrWhiteSpace(preferredRoomType) ? null : preferredRoomType.Trim();

        return rooms
            .Where(r => r.Status == "Available" && (guests <= 0 || r.GuestCapacity >= guests))
            .Select(r => new RoomCandidate(
                r.Id,
                r.RoomNumber,
                true,
                r.Floor,
                Math.Abs(r.Floor - 1),
                GetRoomTypePriority(r.Type),
                preferredRoomType is not null && string.Equals(r.Type.ToString(), preferredRoomType, StringComparison.OrdinalIgnoreCase),
                r.Type.ToString(),
                r.GuestCapacity
            ))
            .ToList();
    }

    private static int GetRoomTypePriority(RoomType type) => type switch
    {
        RoomType.Suite => 4,
        RoomType.Accessible => 3,
        RoomType.Double => 2,
        RoomType.Single => 1,
        _ => 0
    };
}
