using FluentValidation;
using HotelOS.RoomService.DTOs;

namespace HotelOS.RoomService.Validators;

public sealed class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomDtoValidator()
    {
        RuleFor(r => r.RoomNumber).NotEmpty().MaximumLength(20);
        RuleFor(r => r.Type).NotEmpty().Must(t => new[] { "Single", "Double", "Suite", "Accessible" }.Contains(t))
            .WithMessage("Room type must be Single, Double, Suite, or Accessible.");
        RuleFor(r => r.Floor).GreaterThan(0);
    }
}

public sealed class UpdateRoomDtoValidator : AbstractValidator<UpdateRoomDto>
{
    public UpdateRoomDtoValidator()
    {
        RuleFor(r => r.RoomNumber).NotEmpty().MaximumLength(20);
        RuleFor(r => r.Type).NotEmpty().Must(t => new[] { "Single", "Double", "Suite", "Accessible" }.Contains(t));
        RuleFor(r => r.Floor).GreaterThan(0);
        RuleFor(r => r.PricePerNight).GreaterThan(0);
        RuleFor(r => r.GuestCapacity).GreaterThan(0);
    }
}
