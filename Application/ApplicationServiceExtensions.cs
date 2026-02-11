using HighHeavyShipment.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HighHeavyShipment.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IShipmentCommandService, ShipmentCommandService>();
        services.AddScoped<IShipmentQueryService, ShipmentQueryService>();
        return services;
    }
}
