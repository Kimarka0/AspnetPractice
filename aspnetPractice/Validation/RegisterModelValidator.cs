using aspnetPractice.Models;
using FluentValidation;

namespace aspnetPractice.Validation
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(register => register.Name)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(register => register.Surname)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(register => register.Age)
                .NotEmpty()
                .InclusiveBetween(18, 100);

            RuleFor(register => register.Balance)
                .GreaterThanOrEqualTo(0);

            RuleFor(register => register.Password)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(10);
        }
    }
}
