using FluentValidation;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Validators;

public class RoomValidator : AbstractValidator<Room>
{
    public RoomValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be positive");

        RuleFor(x => x.LocationId)
            .NotEqual(System.Guid.Empty)
            .WithMessage("Location is required");
    }
}
