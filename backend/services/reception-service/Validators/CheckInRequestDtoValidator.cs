using FluentValidation;
using HotelOS.ReceptionService.DTOs;

namespace HotelOS.ReceptionService.Validators;

public sealed class CheckInRequestDtoValidator : AbstractValidator<CheckInRequestDto>
{
    public CheckInRequestDtoValidator()
    {
        RuleFor(request => request.GuestName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Adults).GreaterThan(0);
        RuleFor(request => request.Kids).GreaterThanOrEqualTo(0);
        RuleFor(request => request.BranchId).GreaterThan(0);
        RuleFor(request => request.CheckInDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(request => request.CheckOutDate).GreaterThan(request => request.CheckInDate);
    }
}