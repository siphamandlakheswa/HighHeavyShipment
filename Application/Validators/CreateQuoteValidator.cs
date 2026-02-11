using FluentValidation;
using HighHeavyShipment.Application.Commands;

namespace HighHeavyShipment.Application.Validators;

public class CreateQuoteValidator : AbstractValidator<CreateQuoteCommand>
{
    public CreateQuoteValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Quote amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter ISO code")
            .Matches("^[A-Z]{3}$").WithMessage("Currency must contain only uppercase letters");
    }
}
