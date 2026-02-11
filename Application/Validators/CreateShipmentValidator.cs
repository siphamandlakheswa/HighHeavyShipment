using FluentValidation;
using HighHeavyShipment.Application.Commands;

namespace HighHeavyShipment.Application.Validators;

public class CreateShipmentValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentValidator()
    {
        RuleFor(x => x.Reference)
            .NotEmpty().WithMessage("Reference is required")
            .MaximumLength(50).WithMessage("Reference cannot exceed 50 characters");

        RuleFor(x => x.Origin)
            .NotEmpty().WithMessage("Origin is required")
            .MaximumLength(100).WithMessage("Origin cannot exceed 100 characters");

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Destination is required")
            .MaximumLength(100).WithMessage("Destination cannot exceed 100 characters");

        RuleFor(x => x.WeightKg)
            .GreaterThan(0).WithMessage("Weight must be greater than 0");

        RuleFor(x => new { x.Origin, x.Destination })
            .Must(x => x.Origin != x.Destination)
            .WithMessage("Origin and destination must be different")
            .WithName("route");
    }
}
