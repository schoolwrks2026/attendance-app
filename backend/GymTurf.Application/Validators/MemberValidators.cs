using FluentValidation;
using GymTurf.Application.DTOs;

namespace GymTurf.Application.Validators;

public class UpdateMemberProfileDtoValidator : AbstractValidator<UpdateMemberProfileDto>
{
    public UpdateMemberProfileDtoValidator()
    {
        RuleFor(x => x.EmergencyContactName).MaximumLength(100);
        RuleFor(x => x.EmergencyContactPhone).MaximumLength(50);
    }
}

public class PurchaseMembershipDtoValidator : AbstractValidator<PurchaseMembershipDto>
{
    public PurchaseMembershipDtoValidator()
    {
        RuleFor(x => x.MembershipType).NotEmpty().Must(t => t == "Monthly" || t == "Annual" || t == "Premium")
            .WithMessage("Membership type must be Monthly, Annual, or Premium.");
        RuleFor(x => x.DurationInMonths).GreaterThan(0).WithMessage("Duration must be greater than zero.");
    }
}
