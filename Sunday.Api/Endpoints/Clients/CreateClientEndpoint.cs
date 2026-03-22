using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstract;
using Sunday.Application.Abstract;
using Sunday.Application.Clients.Create;

namespace Sunday.Api.Endpoints.Clients;

public class CreateClientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/clients", async (CreateClientRequest request, ICommandHandler<CreateClientCommand, string> handler) =>
        {
            var command = new CreateClientCommand(request.Name, request.AgencyId, request.AccountManagerId);
            var result = await handler.HandleAsync(command);
            return Results.Created($"/clients/{result.Value}", result.Value);
        })
        .WithName("CreateClient")
        .WithTags("Clients")
        .RequireAuthorization("SystemAdmin");
    }
}

public record CreateClientRequest(string Name, string AgencyId, string? AccountManagerId);
