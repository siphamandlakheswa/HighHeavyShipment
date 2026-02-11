using FluentValidation;
using HighHeavyShipment.Application.Commands;
using HighHeavyShipment.Application.Services;
using HighHeavyShipment.Domain;

namespace HighHeavyShipment.Presentation;

public static class EndpointExtensions
{
    public static WebApplication MapShipmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/shipments")
            .WithName("Shipments")
            .WithOpenApi();

        group.MapPost("/", CreateShipment)
            .WithName("Create Shipment")
            .WithDescription("Create a new high & heavy shipment");

        group.MapGet("/", GetAllShipments)
            .WithName("Get All Shipments")
            .WithDescription("Retrieve all shipments");

        group.MapGet("/{id}", GetShipmentById)
            .WithName("Get Shipment")
            .WithDescription("Retrieve a specific shipment with quotes");

        group.MapPost("/{id}/quotes", CreateQuote)
            .WithName("Create Quote")
            .WithDescription("Add a quote for a shipment at a specific phase");

        group.MapPost("/{id}/status/advance", AdvanceStatus)
            .WithName("Advance Status")
            .WithDescription("Advance shipment to next status");

        return app;
    }

    private static async Task<IResult> CreateShipment(
        CreateShipmentRequest request,
        IShipmentCommandService commandService,
        IValidator<CreateShipmentCommand> validator)
    {
        var command = new CreateShipmentCommand(
            request.Reference,
            request.Mode,
            request.Origin,
            request.Destination,
            request.WeightKg);

        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new
            {
                errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        var result = await commandService.CreateShipmentAsync(command);

        return result.Match<IResult>(
            success => Results.Created($"/api/shipments/{success.Value}", new { id = success.Value }),
            failure => Results.BadRequest(new { code = failure.Code, message = failure.Message }));
    }

    private static async Task<IResult> GetAllShipments(IShipmentQueryService queryService)
    {
        var shipments = await queryService.GetAllShipmentsAsync();
        return Results.Ok(shipments);
    }

    private static async Task<IResult> GetShipmentById(Guid id, IShipmentQueryService queryService)
    {
        var shipment = await queryService.GetShipmentByIdAsync(id);
        if (shipment == null)
            return Results.NotFound();

        return Results.Ok(shipment);
    }

    private static async Task<IResult> CreateQuote(
        Guid id,
        CreateQuoteRequest request,
        IShipmentCommandService commandService,
        IValidator<CreateQuoteCommand> validator)
    {
        var command = new CreateQuoteCommand(
            id,
            request.Phase,
            request.Amount,
            request.Currency);

        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new
            {
                errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        var result = await commandService.CreateQuoteAsync(command);

        return result.Match<IResult>(
            success => Results.Created($"/api/shipments/{id}/quotes/{success.Value}", new { id = success.Value }),
            failure => Results.BadRequest(new { code = failure.Code, message = failure.Message }));
    }

    private static async Task<IResult> AdvanceStatus(
        Guid id,
        IShipmentCommandService commandService)
    {
        var command = new AdvanceShipmentStatusCommand(id);
        var result = await commandService.AdvanceStatusAsync(command);

        return result.Match<IResult>(
            success => Results.Ok(new { message = "Status advanced successfully" }),
            failure => Results.BadRequest(new { code = failure.Code, message = failure.Message }));
    }
}

public record CreateShipmentRequest(
    string Reference,
    ShipmentMode Mode,
    string Origin,
    string Destination,
    decimal WeightKg);

public record CreateQuoteRequest(
    ShipmentStatus Phase,
    decimal Amount,
    string Currency);
