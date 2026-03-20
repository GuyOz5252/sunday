using Microsoft.AspNetCore.Routing;

namespace Sunday.Api.Endpoints.Abstracts;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
