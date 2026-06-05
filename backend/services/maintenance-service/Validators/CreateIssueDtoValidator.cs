using FluentValidation;
using HotelOS.MaintenanceService.DTOs;

namespace HotelOS.MaintenanceService.Validators;

public sealed class CreateIssueDtoValidator : AbstractValidator<CreateIssueDto>
{
    public CreateIssueDtoValidator()
    {
        RuleFor(request => request.RoomNumber).NotEmpty();
        RuleFor(request => request.Title).NotEmpty();
        RuleFor(request => request.Priority).NotEmpty();
    }
}