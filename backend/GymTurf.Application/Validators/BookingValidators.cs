using FluentValidation;
using GymTurf.Application.DTOs;
using System;

namespace GymTurf.Application.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.FacilityId).NotEmpty().WithMessage("Facility is required.");
        RuleFor(x => x.BookingDate).NotEmpty().Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Booking date cannot be in the past.");
        RuleFor(x => x.StartTime).NotEmpty().Matches(@"^([01]\d|2[0-3]):[0-5]\d$")
            .WithMessage("Start time must be in HH:mm format.");
        RuleFor(x => x.EndTime).NotEmpty().Matches(@"^([01]\d|2[0-3]):[0-5]\d$")
            .WithMessage("End time must be in HH:mm format.");
    }
}
