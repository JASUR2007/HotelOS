using FluentValidation;
using HotelOS.GatewayApi.DTOs;

namespace HotelOS.GatewayApi.Validators;

public sealed class CreateBranchDtoValidator : AbstractValidator<CreateBranchDto>
{
    public CreateBranchDtoValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Address).NotEmpty().MaximumLength(500);
        RuleFor(r => r.City).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Country).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Phone).NotEmpty().MaximumLength(50);
        RuleFor(r => r.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}

public sealed class UpdateBranchDtoValidator : AbstractValidator<UpdateBranchDto>
{
    public UpdateBranchDtoValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Address).NotEmpty().MaximumLength(500);
        RuleFor(r => r.City).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Country).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Phone).NotEmpty().MaximumLength(50);
        RuleFor(r => r.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(r => r.Status).NotEmpty().Must(s => s is "Active" or "Inactive");
    }
}
