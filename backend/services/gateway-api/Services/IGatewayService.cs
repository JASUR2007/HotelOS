using HotelOS.GatewayApi.DTOs;

namespace HotelOS.GatewayApi.Services;

public interface IGatewayService
{
    Task<IReadOnlyList<DashboardSummaryDto>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
}