using aspnetPractice.Models;
using FluentValidation;

public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator()
    {
        RuleFor(client => client.Name)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(client => client.Surname)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(client => client.Age)
            .NotEmpty()
            .InclusiveBetween(18, 100);
        
        RuleFor(client => client.Balance)
            .GreaterThanOrEqualTo(0);
    }
}