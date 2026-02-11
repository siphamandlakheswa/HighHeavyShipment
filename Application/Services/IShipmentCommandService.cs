using HighHeavyShipment.Application.Commands;
using HighHeavyShipment.Domain;

namespace HighHeavyShipment.Application.Services;

public interface IShipmentCommandService
{
    Task<Result<Guid>> CreateShipmentAsync(CreateShipmentCommand command);
    Task<Result> AdvanceStatusAsync(AdvanceShipmentStatusCommand command);
    Task<Result<Guid>> CreateQuoteAsync(CreateQuoteCommand command);
}
