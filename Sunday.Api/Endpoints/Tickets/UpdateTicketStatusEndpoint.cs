using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Authorization;
using Sunday.Api.Endpoints.Abstracts;
using Sunday.Application.Abstracts;
using Sunday.Application.Tickets.UpdateStatus;
using Sunday.Core.Abstracts;
using Sunday.Core.DomainEvents;
using Sunday.Core.Models;

namespace Sunday.Api.Endpoints.Tickets;

public class UpdateTicketStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/tickets/{ticketId}/status", async (
            string ticketId,
            UpdateTicketStatusRequest request,
            ClaimsPrincipal user,
            IAuthorizationService authorizationService,
            ITicketRepository ticketRepository,
            ICommandHandler<UpdateTicketStatusCommand> handler) =>
        {
            var userId = user.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var ticketResult = await ticketRepository.GetAsync(ticketId);
            if (ticketResult.IsFailure)
            {
                return Results.NotFound();
            }

            var ticket = ticketResult.Value;
            var authResult = await authorizationService.AuthorizeAsync(user, ticket, "TicketAccess");
            if (!authResult.Succeeded)
            {
                return Results.Forbid();
            }

            var command = new UpdateTicketStatusCommand(
                ticketId,
                request.NewStatus,
                userId,
                request.Note,
                request.NextAssigneeId);

            var result = await handler.HandleAsync(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.NoContent();
        })
        .WithName("UpdateTicketStatus")
        .WithTags("Tickets")
        .RequireAuthorization();
    }
}

public record UpdateTicketStatusRequest(
    TicketStatus NewStatus,
    string? Note = null,
    string? NextAssigneeId = null);
