using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstracts;
using Sunday.Application.Abstracts;
using Sunday.Application.WorkSessions.Start;
using Sunday.Core.Abstracts;

namespace Sunday.Api.Endpoints.WorkSessions;

public class StartWorkSessionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/tickets/{ticketId}/work-sessions", async (
            string ticketId,
            StartWorkSessionRequest request,
            ClaimsPrincipal user,
            IAuthorizationService authorizationService,
            ITicketRepository ticketRepository,
            ICommandHandler<StartWorkSessionCommand, string> handler) =>
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

            var command = new StartWorkSessionCommand(ticketId, userId, request.Description);
            var result = await handler.HandleAsync(command);

            return result.IsFailure 
                ? Results.BadRequest(result.Error)
                : Results.Created($"/work-sessions/{result.Value}", result.Value);
        })
        .WithName("StartWorkSession")
        .WithTags("WorkSessions")
        .RequireAuthorization();
    }
}

public record StartWorkSessionRequest(string? Description = null);
