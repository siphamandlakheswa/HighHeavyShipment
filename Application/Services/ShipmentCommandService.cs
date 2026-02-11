using FluentValidation;
using HighHeavyShipment.Application.Commands;
using HighHeavyShipment.Domain;
using HighHeavyShipment.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HighHeavyShipment.Application.Services;

public class ShipmentCommandService : IShipmentCommandService
{
    private readonly ShipmentDbContext _context;
    private readonly IValidator<CreateShipmentCommand> _createShipmentValidator;
    private readonly IValidator<CreateQuoteCommand> _createQuoteValidator;

    public ShipmentCommandService(
        ShipmentDbContext context,
        IValidator<CreateShipmentCommand> createShipmentValidator,
        IValidator<CreateQuoteCommand> createQuoteValidator)
    {
        _context = context;
        _createShipmentValidator = createShipmentValidator;
        _createQuoteValidator = createQuoteValidator;
    }

    public async Task<Result<Guid>> CreateShipmentAsync(CreateShipmentCommand command)
    {
        // Validate via FluentValidation
        var validationResult = await _createShipmentValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return new Result<Guid>.Failure("VALIDATION_FAILED", errors);
        }

        // Create domain aggregate via factory method
        var shipmentResult = Shipment.Create(
            command.Reference,
            command.Mode,
            command.Origin,
            command.Destination,
            command.WeightKg);

        if (shipmentResult is not Result<Shipment>.Success shipmentSuccess)
        {
            var failure = (Result<Shipment>.Failure)shipmentResult;
            return new Result<Guid>.Failure(failure.Code, failure.Message);
        }

        var shipment = shipmentSuccess.Value;

        // Check uniqueness of reference
        var existingShipment = await _context.Shipments
            .FirstOrDefaultAsync(s => s.Reference == shipment.Reference);
        if (existingShipment != null)
            return new Result<Guid>.Failure("REFERENCE_DUPLICATE", "Reference already exists");

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        return new Result<Guid>.Success(shipment.Id);
    }

    public async Task<Result> AdvanceStatusAsync(AdvanceShipmentStatusCommand command)
    {
        var shipment = await _context.Shipments.FirstOrDefaultAsync(s => s.Id == command.ShipmentId);
        if (shipment == null)
            return new Result.Failure("NOT_FOUND", "Shipment not found");

        var result = shipment.AdvanceStatus();
        if (result is not Result.Success)
        {
            var failure = (Result.Failure)result;
            return new Result.Failure(failure.Code, failure.Message);
        }

        await _context.SaveChangesAsync();
        return new Result.Success();
    }

    public async Task<Result<Guid>> CreateQuoteAsync(CreateQuoteCommand command)
    {
        // Validate via FluentValidation
        var validationResult = await _createQuoteValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return new Result<Guid>.Failure("VALIDATION_FAILED", errors);
        }

        var shipment = await _context.Shipments.FirstOrDefaultAsync(s => s.Id == command.ShipmentId);
        if (shipment == null)
            return new Result<Guid>.Failure("NOT_FOUND", "Shipment not found");

        // Create quote via factory method
        var quoteResult = ShipmentQuote.Create(
            command.ShipmentId,
            command.Phase,
            command.Amount,
            command.Currency,
            shipment.Status);

        if (quoteResult is not Result<ShipmentQuote>.Success quoteSuccess)
        {
            var failure = (Result<ShipmentQuote>.Failure)quoteResult;
            return new Result<Guid>.Failure(failure.Code, failure.Message);
        }

        var quote = quoteSuccess.Value;

        // Enforce one quote per shipment per phase
        var existingQuote = await _context.ShipmentQuotes
            .FirstOrDefaultAsync(q => q.ShipmentId == command.ShipmentId && q.Phase == command.Phase);
        if (existingQuote != null)
            return new Result<Guid>.Failure("QUOTE_DUPLICATE", "Quote already exists for this phase");

        _context.ShipmentQuotes.Add(quote);
        await _context.SaveChangesAsync();

        return new Result<Guid>.Success(quote.Id);
    }
}
