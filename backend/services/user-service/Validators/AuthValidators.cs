using FluentValidation;
using HotelOS.UserService.DTOs;

namespace HotelOS.UserService.Validators;

public sealed class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(item => item.Email).NotEmpty().EmailAddress();
        RuleFor(item => item.Password).NotEmpty();
    }
}

public sealed class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(item => item.Email).NotEmpty().EmailAddress();
        RuleFor(item => item.DisplayName).NotEmpty();
        RuleFor(item => item.Password).MinimumLength(6);
    }
}