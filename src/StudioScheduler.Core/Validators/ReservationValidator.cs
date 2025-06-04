using FluentValidation;
using StudioScheduler.Core.Enums;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Validators;

public class ReservationValidator : AbstractValidator<Reservation>
{
    public ReservationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(default(Guid))
            .WithMessage("User ID is required");

        RuleFor(x => x.ScheduleId)
            .NotEqual(default(Guid))
            .WithMessage("Schedule ID is required");

        RuleFor(x => x.PassId)
            .NotEqual(default(Guid))
            .WithMessage("Pass ID is required");

        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .When(x => x.Status == ReservationStatus.Cancelled)
            .WithMessage("Cancellation reason is required when reservation is cancelled");

        RuleFor(x => x.HasAttended)
            .Must((reservation, attended) => !attended || reservation.Status == ReservationStatus.Confirmed)
            .WithMessage("Cannot mark attendance for unconfirmed reservation");
    }
}
