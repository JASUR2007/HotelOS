using System.Net.Sockets;
using HotelOS.GatewayApi.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace HotelOS.GatewayApi.Health;

public sealed class HealthCheckService(GatewayDbContext dbContext, IConfiguration configuration, IHttpClientFactory httpClientFactory)
{
    public async Task<IReadOnlyList<ServiceHealthSnapshot>> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        var postgres = await CheckPostgresAsync(cancellationToken);
        var rabbitmq = await CheckRabbitMqAsync(cancellationToken);
        var websocket = await CheckWebsocketAsync(cancellationToken);

        return [postgres, rabbitmq, websocket];
    }

    private async Task<ServiceHealthSnapshot> CheckPostgresAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connected = await dbContext.Database.CanConnectAsync(cancellationToken);
            return new ServiceHealthSnapshot("postgres", connected ? "Healthy" : "Unhealthy", DateTimeOffset.UtcNow, connected ? null : "Database unreachable");
        }
        catch (Exception exception)
        {
            return new ServiceHealthSnapshot("postgres", "Unhealthy", DateTimeOffset.UtcNow, exception.Message);
        }
    }

    private async Task<ServiceHealthSnapshot> CheckRabbitMqAsync(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            using var connection = factory.CreateConnection();
            return new ServiceHealthSnapshot("rabbitmq", "Healthy", DateTimeOffset.UtcNow, null);
        }
        catch (Exception exception)
        {
            return new ServiceHealthSnapshot("rabbitmq", "Unhealthy", DateTimeOffset.UtcNow, exception.Message);
        }
    }

    private async Task<ServiceHealthSnapshot> CheckWebsocketAsync(CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var websocketBaseUrl = configuration["Services:Websocket"] ?? "http://websocket-service:8080";
            var response = await client.GetAsync(new Uri(new Uri(websocketBaseUrl), "/health"), cancellationToken);
            return new ServiceHealthSnapshot("websocket", response.IsSuccessStatusCode ? "Healthy" : "Unhealthy", DateTimeOffset.UtcNow, response.IsSuccessStatusCode ? null : $"HTTP {(int)response.StatusCode}");
        }
        catch (Exception exception)
        {
            return new ServiceHealthSnapshot("websocket", "Unhealthy", DateTimeOffset.UtcNow, exception.Message);
        }
    }
}

public sealed record ServiceHealthSnapshot(string Service, string Status, DateTimeOffset CheckedAt, string? Details);