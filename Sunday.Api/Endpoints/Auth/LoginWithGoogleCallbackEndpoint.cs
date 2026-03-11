using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstract;

namespace Sunday.Api.Endpoints.Auth;

public class LoginWithGoogleCallbackEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/callback/google", () =>
            {
            })
            .AllowAnonymous()
            .WithTags("Auth");
    }
}
