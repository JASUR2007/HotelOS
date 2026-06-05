using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HotelOS.PaymentService.Events;
using RabbitMQ.Client;

namespace HotelOS.PaymentService.BackgroundServices;

public sealed class ReservationExpirationService : BackgroundService
{
    private readonly ILogger<ReservationExpirationService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConnectionFactory _rabbitMqFactory;
    private readonly string _serviceToken;

    private const int CheckIntervalMs = 60_000;
    private const int ExpirationMinutes = 10;

    public ReservationExpirationService(
        ILogger<ReservationExpirationService> logger,
        IHttpClientFactory httpClientFactory,
        IConnectionFactory rabbitMqFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _rabbitMqFactory = rabbitMqFactory;
        _serviceToken = configuration["ServiceToken"] ?? string.Empty;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reservation expiration service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiredReservations(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to check expired reservations");
            }

            await Task.Delay(CheckIntervalMs, stoppingToken);
        }
    }

    private async Task CheckExpiredReservations(CancellationToken ct)
    {
        var httpClient = CreateAuthenticatedClient();
        var response = await httpClient.GetAsync(
            "/api/reception/bookings/expired?status=HELD&minutes=" + ExpirationMinutes, ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to fetch expired bookings: {StatusCode}", response.StatusCode);
            return;
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        var expiredBookings = JsonSerializer.Deserialize<List<ExpiredBookingDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (expiredBookings is null || expiredBookings.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Found {Count} expired reservations to process", expiredBookings.Count);

        foreach (var booking in expiredBookings)
        {
            await ExpireBooking(booking, ct);
        }
    }

    private async Task ExpireBooking(ExpiredBookingDto booking, CancellationToken ct)
    {
        var httpClient = CreateAuthenticatedClient();

        var expirePayload = JsonSerializer.Serialize(new { status = "EXPIRED" });
        var expireContent = new StringContent(expirePayload, Encoding.UTF8, "application/json");
        var expireResponse = await httpClient.PatchAsync(
            $"/api/reception/bookings/{booking.Id}/status", expireContent, ct);

        if (!expireResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to expire booking {BookingId}: {StatusCode}",
                booking.Id, expireResponse.StatusCode);
            return;
        }

        var releasePayload = JsonSerializer.Serialize(new { status = "Available" });
        var releaseContent = new StringContent(releasePayload, Encoding.UTF8, "application/json");
        await httpClient.PatchAsync(
            $"/api/room/rooms/{booking.RoomId}/status", releaseContent, ct);

        PublishBookingExpiredEvent(booking);

        _logger.LogInformation(
            "Booking {BookingId} for {GuestName} in room {RoomNumber} expired and room released",
            booking.Id, booking.GuestName, booking.RoomNumber);
    }

    private HttpClient CreateAuthenticatedClient()
    {
        var client = _httpClientFactory.CreateClient("gateway");
        if (!string.IsNullOrEmpty(_serviceToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _serviceToken);
        }
        return client;
    }

    private void PublishBookingExpiredEvent(ExpiredBookingDto booking)
    {
        using var connection = _rabbitMqFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare("hotelos.events", ExchangeType.Topic, durable: true);
        channel.QueueDeclare("hotelos.events.booking", durable: true, exclusive: false, autoDelete: false);

        var bookingEvent = new BookingExpiredEvent(
            booking.Id,
            booking.GuestName,
            booking.RoomNumber,
            booking.ReservedAt,
            DateTimeOffset.UtcNow);

        var body = JsonSerializer.SerializeToUtf8Bytes(bookingEvent);

        channel.BasicPublish(
            exchange: "hotelos.events",
            routingKey: "booking.expired",
            basicProperties: null,
            body: body);
    }

    private sealed class ExpiredBookingDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTimeOffset ReservedAt { get; set; }
    }
}
