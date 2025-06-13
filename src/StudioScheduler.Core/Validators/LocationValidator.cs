using FluentValidation;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Validators;

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be positive");

        RuleFor(x => x.OpeningTime)
            .NotEmpty()
            .WithMessage("Opening time is required");

        RuleFor(x => x.ClosingTime)
            .NotEmpty()
            .WithMessage("Closing time is required")
            .Must((location, closingTime) => closingTime > location.OpeningTime)
            .WithMessage("Closing time must be after opening time");
    }
}
