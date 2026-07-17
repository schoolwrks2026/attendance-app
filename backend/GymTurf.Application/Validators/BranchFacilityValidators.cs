using FluentValidation;
using GymTurf.Application.DTOs;

namespace GymTurf.Application.Validators;

public class CreateBranchDtoValidator : AbstractValidator<CreateBranchDto>
{
    public CreateBranchDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Branch name is required.");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
    }
}

public class CreateFacilityDtoValidator : AbstractValidator<CreateFacilityDto>
{
    public CreateFacilityDtoValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Facility name is required.");
        RuleFor(x => x.Type).NotEmpty().WithMessage("Facility type is required.")
            .Must(type => type == "Gym" || type == "Turf" || type == "Court")
            .WithMessage("Facility type must be either Gym, Turf, or Court.");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("Capacity must be greater than zero.");
    }
}
