using FluentValidation;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Validators;

public class ScheduleValidator : AbstractValidator<Schedule>
{
    public ScheduleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Schedule name is required");

        RuleFor(x => x.LocationId)
            .NotEqual(System.Guid.Empty)
            .WithMessage("Location ID is required");

        RuleFor(x => x.DanceClassId)
            .NotEqual(System.Guid.Empty)
            .WithMessage("Dance Class ID is required");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty()
            .WithMessage("Effective from date is required");

        RuleFor(x => x.StartTime)
            .Must(startTime => startTime > System.DateTime.UtcNow)
            .WithMessage("Start time must be in the future");

        RuleFor(x => x.Duration)
            .Must(duration => duration.TotalMinutes > 0)
            .WithMessage("Duration must be positive");

        RuleFor(x => x.RecurrencePattern)
            .NotEmpty()
            .When(x => x.IsRecurring)
            .WithMessage("Recurrence pattern is required for recurring schedules");

        RuleFor(x => x.RecurrenceEndDate)
            .Must((schedule, endDate) => !endDate.HasValue || endDate.Value > schedule.StartTime)
            .When(x => x.IsRecurring)
            .WithMessage("Recurrence end date must be after start time");
    }
}
