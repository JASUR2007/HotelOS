using HotelOS.GatewayApi.DTOs;
using HotelOS.GatewayApi.Repositories;

namespace HotelOS.GatewayApi.Services;

public sealed class GatewayService(IGatewayRepository repository) : IGatewayService
{
    public async Task<IReadOnlyList<DashboardSummaryDto>> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var health = await repository.GetServiceHealthAsync(cancellationToken);
        return
        [
            new DashboardSummaryDto("Gateway services", health.Count.ToString(), "+1 hub", "positive"),
            new DashboardSummaryDto("Realtime channel", "Online", "SignalR ready", "positive"),
            new DashboardSummaryDto("Pending actions", "12", "3 urgent", "warning"),
        ];
    }
}