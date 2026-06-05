using HotelOS.GatewayApi.Models;

namespace HotelOS.GatewayApi.Repositories;

public sealed class GatewayRepository : IGatewayRepository
{
    public Task<IReadOnlyList<ServiceHealth>> GetServiceHealthAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ServiceHealth> result =
        [
            new ServiceHealth { Service = "reception-service", Status = "Healthy", CheckedAt = DateTimeOffset.UtcNow },
            new ServiceHealth { Service = "housekeeping-service", Status = "Healthy", CheckedAt = DateTimeOffset.UtcNow },
            new ServiceHealth { Service = "room-service", Status = "Healthy", CheckedAt = DateTimeOffset.UtcNow },
        ];

        return Task.FromResult(result);
    }
}