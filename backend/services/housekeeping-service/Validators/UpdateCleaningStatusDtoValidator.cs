using FluentValidation;
using HotelOS.HousekeepingService.DTOs;

namespace HotelOS.HousekeepingService.Validators;

public sealed class UpdateCleaningStatusDtoValidator : AbstractValidator<UpdateCleaningStatusDto>
{
    public UpdateCleaningStatusDtoValidator()
    {
        RuleFor(request => request.TaskId).GreaterThan(0);
        RuleFor(request => request.Status).NotEmpty();
    }
}