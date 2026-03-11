using Microsoft.AspNetCore.Routing;

namespace Sunday.Api.Endpoints.Abstract;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
