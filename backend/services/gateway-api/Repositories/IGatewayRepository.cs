using HotelOS.GatewayApi.Models;

namespace HotelOS.GatewayApi.Repositories;

public interface IGatewayRepository
{
    Task<IReadOnlyList<ServiceHealth>> GetServiceHealthAsync(CancellationToken cancellationToken = default);
}