using HotelOS.GatewayApi.DTOs;
using FluentValidation;

namespace HotelOS.GatewayApi.Validators;

public sealed class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Password).NotEmpty().MinimumLength(6);
    }
}