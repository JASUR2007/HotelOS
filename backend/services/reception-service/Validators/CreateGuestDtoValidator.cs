using FluentValidation;
using HotelOS.ReceptionService.DTOs;
using HotelOS.ReceptionService.Models;

namespace HotelOS.ReceptionService.Validators;

public sealed class CreateGuestDtoValidator : FluentValidation.AbstractValidator<CreateGuestDto>
{
    public CreateGuestDtoValidator()
    {
        RuleFor(g => g.FullName).NotEmpty().MaximumLength(200);
        RuleFor(g => g.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}
