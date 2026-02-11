using HighHeavyShipment.Domain;

namespace HighHeavyShipment.Application.DTOs;

public record ShipmentDto(
    Guid Id,
    string Reference,
    ShipmentMode Mode,
    string Origin,
    string Destination,
    decimal WeightKg,
    ShipmentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    List<ShipmentQuoteDto> Quotes);

public record ShipmentQuoteDto(
    Guid Id,
    ShipmentStatus Phase,
    decimal Amount,
    string Currency,
    DateTimeOffset CreatedAt);
