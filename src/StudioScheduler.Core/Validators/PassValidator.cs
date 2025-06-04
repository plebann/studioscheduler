using FluentValidation;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Validators;

public class PassValidator : AbstractValidator<Pass>
{
    public PassValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(System.Guid.Empty)
            .WithMessage("User is required");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.ClassesPerWeek)
            .GreaterThan(0)
            .WithMessage("Classes per week must be positive");

        RuleFor(x => x.RemainingClasses)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Remaining classes cannot be negative");
    }
}
