using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstracts;
using Sunday.Application.Abstracts;
using Sunday.Application.Campaigns.Create;

namespace Sunday.Api.Endpoints.Campaigns;

public class CreateCampaignEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/brands/{brandId}/campaigns", async (string brandId, CreateCampaignRequest request, ICommandHandler<CreateCampaignCommand, string> handler) =>
        {
            var command = new CreateCampaignCommand(request.Name, brandId);
            var result = await handler.HandleAsync(command);
            return Results.Created($"/campaigns/{result.Value}", result.Value);
        })
        .WithName("CreateCampaign")
        .WithTags("Campaigns")
        .RequireAuthorization("AgencyAdmin");
    }
}

public record CreateCampaignRequest(string Name);
