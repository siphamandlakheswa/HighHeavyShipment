namespace HighHeavyShipment.Domain;

/// <summary>
/// Domain aggregate: Shipment representing a high & heavy shipment.
/// </summary>
public class Shipment
{
    public Guid Id { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public ShipmentMode Mode { get; private set; }
    public string Origin { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public decimal WeightKg { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    // Navigation
    public ICollection<ShipmentQuote> Quotes { get; private set; } = new List<ShipmentQuote>();

    private Shipment() { }

    public static Result<Shipment> Create(
        string reference,
        ShipmentMode mode,
        string origin,
        string destination,
        decimal weightKg)
    {
        if (string.IsNullOrWhiteSpace(reference))
            return new Result<Shipment>.Failure("INVALID_REFERENCE", "Reference cannot be empty");

        if (origin == destination)
            return new Result<Shipment>.Failure("INVALID_ROUTE", "Origin and destination must differ");

        if (weightKg <= 0)
            return new Result<Shipment>.Failure("INVALID_WEIGHT", "Weight must be greater than 0");

        return new Result<Shipment>.Success(new Shipment
        {
            Id = Guid.NewGuid(),
            Reference = reference.Trim(),
            Mode = mode,
            Origin = origin.Trim(),
            Destination = destination.Trim(),
            WeightKg = weightKg,
            Status = ShipmentStatus.QuotationRequested,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }

    public Result AdvanceStatus()
    {
        var nextStatus = (ShipmentStatus)(((int)Status) + 1);
        if ((int)nextStatus > (int)ShipmentStatus.Delivered)
            return new Result.Failure("STATUS_AT_END", "Shipment is already delivered");

        Status = nextStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
        return new Result.Success();
    }

    public Result CanTransitionToStatus(ShipmentStatus targetStatus)
    {
        if ((int)targetStatus < (int)Status)
            return new Result.Failure("STATUS_REGRESSION", "Cannot move backwards in status");

        if ((int)targetStatus > (int)Status + 1)
            return new Result.Failure("STATUS_SKIP", "Can only advance one status at a time");

        return new Result.Success();
    }
}
