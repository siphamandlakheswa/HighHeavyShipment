using HighHeavyShipment.Domain;

namespace HighHeavyShipment.Application.DTOs;

public record ShipmentQuoteDto(
    Guid Id,
    ShipmentStatus Phase,
    decimal Amount,
    string Currency,
    DateTimeOffset CreatedAt);
