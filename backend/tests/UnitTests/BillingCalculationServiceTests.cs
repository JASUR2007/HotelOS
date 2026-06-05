using HotelOS.PaymentService.Services;
using Xunit;

namespace HotelOS.UnitTests;

public sealed class BillingCalculationServiceTests
{
    [Fact]
    public void Calculate_ReturnsGrossAndNetTotals()
    {
        var service = new BillingCalculationService();
        var result = service.Calculate(new BillingInput(400, 120, 35, 45, 80));

        Assert.Equal(600, result.GrossTotal);
        Assert.Equal(520, result.NetTotal);
    }
}