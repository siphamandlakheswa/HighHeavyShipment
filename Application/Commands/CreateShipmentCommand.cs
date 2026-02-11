using HighHeavyShipment.Domain;

namespace HighHeavyShipment.Application.Commands;

public record CreateShipmentCommand(
    string Reference,
    ShipmentMode Mode,
    string Origin,
    string Destination,
    decimal WeightKg);
