using Domain.Requests;
using FluentValidation;

namespace API.Configurations.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
     public RegisterRequestValidator()
    {
        RuleFor(rr => rr.Login)
            .NotEmpty()
            .WithMessage("Login is required")
            .MinimumLength(3)
            .WithMessage("Login must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Login must be less than 50 characters")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Login can only contain letters, numbers and underscores");

        RuleFor(rr => rr.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters")
            .Matches(@"(?=.*\d)(?=.*[A-Z])(?=.*[a-z])")
            .WithMessage("Password must contain at least one digit, one uppercase and one lowercase letter");

        RuleFor(rr => rr.Age)
            .NotEmpty()
            .WithMessage("Age is required")
            .Must(age => age >= 18)
            .WithMessage("User must be at least 18 years old")
            .Must(age => age < 118).
            WithMessage("User must be younger than 118 years old");
    }
}