using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Core.Abstract;
using IEndpoint = Sunday.Api.Endpoints.Abstract.IEndpoint;

namespace Sunday.Api.Endpoints.Auth;

public class LoginWithGoogleCallbackEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/callback/google", async (
                IAuthService authService,
                CancellationToken cancellationToken) =>
            {
                
            })
            .AllowAnonymous()
            .WithTags("Auth");
    }
}
