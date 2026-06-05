namespace HotelOS.PaymentService.Services;

public sealed record BillingInput(
    decimal RoomNightsTotal,
    decimal FoodOrdersTotal,
    decimal MinibarTotal,
    decimal DamagesTotal,
    decimal DiscountsTotal);

public sealed record BillingBreakdown(decimal GrossTotal, decimal NetTotal);

public sealed class BillingCalculationService
{
    public BillingBreakdown Calculate(BillingInput input)
    {
        var grossTotal = input.RoomNightsTotal + input.FoodOrdersTotal + input.MinibarTotal + input.DamagesTotal;
        var netTotal = Math.Max(0, grossTotal - input.DiscountsTotal);
        return new BillingBreakdown(grossTotal, netTotal);
    }
}