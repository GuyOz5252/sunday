using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstracts;
using Sunday.Application.Abstracts;
using Sunday.Application.WorkSessions.Stop;

namespace Sunday.Api.Endpoints.WorkSessions;

public class StopWorkSessionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/work-sessions/stop", async (
            StopWorkSessionRequest request,
            ClaimsPrincipal user,
            ICommandHandler<StopWorkSessionCommand> handler) =>
        {
            var userId = user.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var command = new StopWorkSessionCommand(userId, request.Description);
            await handler.HandleAsync(command);

            return Results.NoContent();
        })
        .WithName("StopWorkSession")
        .WithTags("WorkSessions")
        .RequireAuthorization();
    }
}

public record StopWorkSessionRequest(string? Description = null);
