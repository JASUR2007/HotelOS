using FluentValidation;
using HotelOS.RoomService.DTOs;

namespace HotelOS.RoomService.Validators;

public sealed class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(request => request.RoomNumber).NotEmpty();
        RuleFor(request => request.GuestName).NotEmpty();
        RuleFor(request => request.Items).NotEmpty();
    }
}