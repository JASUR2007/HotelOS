using Xunit;

namespace HotelOS.IntegrationTests;

public sealed class GatewaySmokeTests
{
    [Fact]
    public void GatewayProject_IsReferenced()
    {
        Assert.True(typeof(HotelOS.GatewayApi.DTOs.DashboardSummaryDto).IsClass);
    }
}