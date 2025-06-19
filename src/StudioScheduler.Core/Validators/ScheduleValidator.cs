using FluentValidation;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Validators;

public class ScheduleValidator : AbstractValidator<Schedule>
{
    public ScheduleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.LocationId)
            .NotEqual(Guid.Empty)
            .WithMessage("Location is required");

        RuleFor(x => x.DanceClassId)
            .NotEqual(Guid.Empty)
            .WithMessage("Dance class is required");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum()
            .WithMessage("Day of week must be valid");

        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .WithMessage("Duration must be positive")
            .LessThanOrEqualTo(200)
            .WithMessage("Duration cannot exceed 200 minutes");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty()
            .WithMessage("Effective from date is required");

        RuleFor(x => x.Level)
            .NotEmpty()
            .WithMessage("Level is required");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be positive");

        RuleFor(x => x.InstructorId)
            .NotNull()
            .WithMessage("Instructor is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Instructor is required");

        RuleFor(x => x.RoomId)
            .NotNull()
            .WithMessage("Room is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Room is required");

        RuleFor(x => x.RecurrenceEndDate)
            .Must((schedule, endDate) => !endDate.HasValue || endDate.Value > schedule.EffectiveFrom)
            .When(x => x.IsRecurring)
            .WithMessage("Recurrence end date must be after effective from date");
    }
}
