using FluentValidation;
using HotelOS.WebsocketService.DTOs;

namespace HotelOS.WebsocketService.Validators;

public sealed class RealtimeNotificationDtoValidator : AbstractValidator<RealtimeNotificationDto>
{
    public RealtimeNotificationDtoValidator()
    {
        RuleFor(request => request.Title).NotEmpty();
        RuleFor(request => request.Message).NotEmpty();
        RuleFor(request => request.Type).NotEmpty();
    }
}