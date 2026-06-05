using FluentValidation;
using HotelOS.ReceptionService.DTOs;

namespace HotelOS.ReceptionService.Validators;

public sealed class HoldReservationDtoValidator : AbstractValidator<HoldReservationDto>
{
    public HoldReservationDtoValidator()
    {
        RuleFor(request => request.RoomId).GreaterThan(0);
        RuleFor(request => request.RoomNumber).NotEmpty().MaximumLength(10);
        RuleFor(request => request.GuestsCount).GreaterThan(0);
        RuleFor(request => request.CheckInDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(request => request.CheckOutDate).GreaterThan(request => request.CheckInDate);
    }
}
