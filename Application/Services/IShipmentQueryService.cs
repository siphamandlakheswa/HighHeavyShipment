using HighHeavyShipment.Application.DTOs;

namespace HighHeavyShipment.Application.Services;

public interface IShipmentQueryService
{
    Task<ShipmentDto?> GetShipmentByIdAsync(Guid shipmentId);
    Task<List<ShipmentDto>> GetAllShipmentsAsync();
}
