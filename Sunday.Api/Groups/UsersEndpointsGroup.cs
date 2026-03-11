using FastEndpoints;

namespace Sunday.Api.Groups;

public sealed class UsersEndpointsGroup : Group
{
    public UsersEndpointsGroup()
    {
        Configure("/users", _ => {});
    }
}
