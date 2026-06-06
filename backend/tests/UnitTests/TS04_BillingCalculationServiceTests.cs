using HotelOS.PaymentService.Services;
using Xunit;

namespace HotelOS.UnitTests;

public sealed class TS04_BillingCalculationServiceTests
{
    private readonly BillingCalculationService _service = new();

    [Fact]
    public void Calculate_Returns_GrossTotal_As_Sum_Of_All_Charges()
    {
        var result = _service.Calculate(new BillingInput(
            RoomNightsTotal: 400,
            FoodOrdersTotal: 120,
            MinibarTotal: 35,
            DamagesTotal: 45,
            DiscountsTotal: 0
        ));

        Assert.Equal(600, result.GrossTotal);
        Assert.Equal(600, result.NetTotal);
    }

    [Fact]
    public void Calculate_Deducts_Discounts_From_Gross()
    {
        var result = _service.Calculate(new BillingInput(400, 120, 35, 45, 80));

        Assert.Equal(600, result.GrossTotal);
        Assert.Equal(520, result.NetTotal);
    }

    [Fact]
    public void Calculate_NetTotal_Floor_Is_Zero()
    {
        var result = _service.Calculate(new BillingInput(0, 0, 0, 0, 100));

        Assert.Equal(0, result.GrossTotal);
        Assert.Equal(0, result.NetTotal);
    }

    [Fact]
    public void Calculate_With_Zero_Inputs_Returns_Zero()
    {
        var result = _service.Calculate(new BillingInput(0, 0, 0, 0, 0));

        Assert.Equal(0, result.GrossTotal);
        Assert.Equal(0, result.NetTotal);
    }

    [Fact]
    public void Calculate_Includes_Minibar_And_Damages_In_GrossTotal()
    {
        var result = _service.Calculate(new BillingInput(200, 40, 20, 35, 0));

        Assert.Equal(295, result.GrossTotal);
        Assert.Equal(295, result.NetTotal);
    }
}
