using Microsoft.AspNetCore.Authorization;

namespace Sunday.Api.Authorization;

public class TicketAccessRequirement : IAuthorizationRequirement
{
    public TicketAccessRequirement()
    {
    }
}
