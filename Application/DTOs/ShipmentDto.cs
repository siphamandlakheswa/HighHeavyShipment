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
