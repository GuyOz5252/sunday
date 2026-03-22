using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstract;
using Sunday.Application.Abstract;
using Sunday.Application.Brands.Create;

namespace Sunday.Api.Endpoints.Brands;

public class CreateBrandEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/clients/{clientId}/brands", async (string clientId, CreateBrandRequest request,
                ICommandHandler<CreateBrandCommand, string> handler) =>
            {
                var command = new CreateBrandCommand(request.Name, clientId);
                var result = await handler.HandleAsync(command);
                return Results.Created($"/brands/{result.Value}", result.Value);
            })
            .WithName("CreateBrand")
            .WithTags("Brands")
            .RequireAuthorization("AgencyAdmin");
    }
}

public record CreateBrandRequest(string Name);
