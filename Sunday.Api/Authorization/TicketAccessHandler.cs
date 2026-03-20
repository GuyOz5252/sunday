using Microsoft.AspNetCore.Authorization;
using Sunday.Core.Models;

namespace Sunday.Api.Authorization;


public class TicketAccessHandler : AuthorizationHandler<TicketAccessRequirement, Ticket>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TicketAccessRequirement requirement, Ticket resource)
    {
        // 1. SystemAdmin access
        if (context.User.IsInRole(nameof(Role.SystemAdmin)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 2. Same agency check
        var userAgencyId = context.User.FindFirst("AgencyId")?.Value;
        if (userAgencyId != resource.AgencyId)
        {
            return Task.CompletedTask;
        }

        // 3. Agency wide access (AgencyAdmin, TM, CM)
        if (context.User.IsInRole(nameof(Role.AgencyAdmin)) ||
            context.User.IsInRole(nameof(Role.TrafficManager)) ||
            context.User.IsInRole(nameof(Role.CreativeManager)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 4. Assignee access
        var userId = context.User.FindFirst("UserId")?.Value;
        if (resource.Assignments.Any(a => a.UserId == userId && a.IsActive))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 5. Watcher access
        if (resource.Watchers.Any(w => w.UserId == userId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
