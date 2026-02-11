using HighHeavyShipment.Domain;

namespace HighHeavyShipment.Application.Commands;

public record CreateQuoteCommand(
    Guid ShipmentId,
    ShipmentStatus Phase,
    decimal Amount,
    string Currency);
