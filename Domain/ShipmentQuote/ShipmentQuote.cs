namespace HighHeavyShipment.Domain;

/// <summary>
/// Domain entity: Quote for a shipment at a specific phase.
/// </summary>
public class ShipmentQuote
{
    public Guid Id { get; private set; }
    public Guid ShipmentId { get; private set; }
    public ShipmentStatus Phase { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    // Navigation
    public Shipment? Shipment { get; private set; }

    private ShipmentQuote() { }

    public static Result<ShipmentQuote> Create(
        Guid shipmentId,
        ShipmentStatus phase,
        decimal amount,
        string currency,
        ShipmentStatus currentShipmentStatus)
    {
        if (amount <= 0)
            return new Result<ShipmentQuote>.Failure("INVALID_AMOUNT", "Quote amount must be positive");

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            return new Result<ShipmentQuote>.Failure("INVALID_CURRENCY", "Currency must be ISO 4217 3-letter code");

        if ((int)phase < (int)currentShipmentStatus)
            return new Result<ShipmentQuote>.Failure(
                "QUOTE_PHASE_MISMATCH",
                "Quote phase cannot be earlier than current shipment status");

        return new Result<ShipmentQuote>.Success(new ShipmentQuote
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipmentId,
            Phase = phase,
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            CreatedAt = DateTimeOffset.UtcNow
        });
    }
}
