using FluentValidation;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName is required");

        RuleFor(x => x.PasswordHash)
            .NotEmpty()
            .WithMessage("PasswordHash is required");

        RuleFor(x => x.DateOfBirth)
            .Must(dob => !dob.HasValue || dob.Value < System.DateTime.UtcNow)
            .WithMessage("Date of birth cannot be in the future");
    }
}
