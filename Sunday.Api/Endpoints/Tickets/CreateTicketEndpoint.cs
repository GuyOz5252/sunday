using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstract;
using Sunday.Application.Abstract;
using Sunday.Application.Tickets.Create;

namespace Sunday.Api.Endpoints.Tickets;

public class CreateTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/tickets", async (CreateTicketRequest request, ClaimsPrincipal user, ICommandHandler<CreateTicketCommand, string> handler) =>
        {
            var agencyId = user.FindFirst("AgencyId")?.Value;
            var userId = user.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(agencyId) || string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var command = new CreateTicketCommand(
                agencyId,
                request.CampaignId,
                request.BoardId,
                request.Title,
                request.Brief,
                userId,
                request.DueDate);

            var result = await handler.HandleAsync(command);
            return Results.Created($"/tickets/{result.Value}", result.Value);
        })
        .WithName("CreateTicket")
        .WithTags("Tickets")
        .RequireAuthorization("CreateTicket");
    }
}

public record CreateTicketRequest(
    string CampaignId,
    string BoardId,
    string Title,
    string Brief,
    DateTime? DueDate = null);
