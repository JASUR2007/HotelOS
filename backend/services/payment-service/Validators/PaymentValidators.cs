using FluentValidation;
using HotelOS.PaymentService.DTOs;

namespace HotelOS.PaymentService.Validators;

public sealed class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator()
    {
        RuleFor(item => item.InvoiceNumber).NotEmpty();
        RuleFor(item => item.GuestName).NotEmpty();
        RuleFor(item => item.TotalAmount).GreaterThan(0);
    }
}

public sealed class ProcessPaymentDtoValidator : AbstractValidator<ProcessPaymentDto>
{
    public ProcessPaymentDtoValidator()
    {
        RuleFor(item => item.InvoiceId).GreaterThan(0);
        RuleFor(item => item.Amount).GreaterThan(0);
        RuleFor(item => item.Method).NotEmpty();
    }
}