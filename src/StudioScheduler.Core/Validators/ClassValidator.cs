using FluentValidation;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Validators;

public class DanceClassValidator : AbstractValidator<DanceClass>
{
    public DanceClassValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required");

        RuleFor(x => x.Level)
            .NotEmpty()
            .WithMessage("Level is required");

        RuleFor(x => x.Style)
            .NotEmpty()
            .WithMessage("Style is required");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be positive");

        RuleFor(x => x.InstructorId)
            .NotNull()
            .WithMessage("Instructor is required")
            .NotEqual(System.Guid.Empty)
            .WithMessage("Instructor is required");

        RuleFor(x => x.RoomId)
            .NotNull()
            .WithMessage("Room is required")
            .NotEqual(System.Guid.Empty)
            .WithMessage("Room is required");
    }
}
