using aspnetPractice.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace aspnetPractice.Validation;

public class TransferModelValidation : AbstractValidator<TransferModel>
{
   public TransferModelValidation()
    {
        RuleFor(transfer => transfer.fromId)
            .GreaterThan(0);

        RuleFor(transfer => transfer.toId)
            .GreaterThan(0);

        RuleFor(transfer => transfer.amount)
            .NotEmpty()
            .GreaterThan(0);
    }
}

