using FluentValidation;
using HotelOS.PaymentService.DTOs;

namespace HotelOS.PaymentService.Validators;

public sealed class RefundPaymentDtoValidator : AbstractValidator<RefundPaymentDto>
{
    public RefundPaymentDtoValidator()
    {
        RuleFor(item => item.PaymentId).GreaterThan(0);
        RuleFor(item => item.Amount).GreaterThan(0);
        RuleFor(item => item.Reason).NotEmpty();
    }
}

public sealed class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator()
    {
        RuleFor(item => item.InvoiceNumber).NotEmpty();
        RuleFor(item => item.GuestName).NotEmpty();
        RuleFor(item => item.TotalAmount).GreaterThan(0);
        RuleFor(item => item.RoomNightsTotal).GreaterThanOrEqualTo(0);
        RuleFor(item => item.FoodOrdersTotal).GreaterThanOrEqualTo(0);
        RuleFor(item => item.MinibarTotal).GreaterThanOrEqualTo(0);
        RuleFor(item => item.DamagesTotal).GreaterThanOrEqualTo(0);
        RuleFor(item => item.DiscountsTotal).GreaterThanOrEqualTo(0);
    }
}

public sealed class ProcessPaymentDtoValidator : AbstractValidator<ProcessPaymentDto>
{
    public ProcessPaymentDtoValidator()
    {
        RuleFor(item => item.InvoiceId).GreaterThan(0);
        RuleFor(item => item.Amount).GreaterThan(0);
        RuleFor(item => item.Method)
            .NotEmpty()
            .Must(method => method == "Card" || method == "Cash" || method == "Online")
            .WithMessage("Payment method must be Card, Cash, or Online.");

        When(item => item.Method == "Card", () =>
        {
            RuleFor(item => item.CardNumber)
                .NotEmpty()
                .Length(12, 19)
                .Matches("^[0-9 ]+$")
                .WithMessage("Card number must contain 12 to 19 digits.");

            RuleFor(item => item.CardExpiry)
                .NotEmpty()
                .Matches("^(0[1-9]|1[0-2])/[0-9]{2}$")
                .WithMessage("Expiry must be in MM/YY format.");

            RuleFor(item => item.CardCvc)
                .NotEmpty()
                .Matches("^[0-9]{3,4}$")
                .WithMessage("CVC must be 3 or 4 digits.");
        });
    }
}