using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstract;

namespace Sunday.Api.Endpoints.Auth;

public class LoginWithGoogleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/callback/google", (string? returnUrl) =>
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = string.IsNullOrEmpty(returnUrl)
                        ? "/auth/callback/google"
                        : $"/auth/callback/google?returnUrl={Uri.EscapeDataString(returnUrl)}"
                };
                return Task.FromResult(Results.Challenge(properties, [GoogleDefaults.AuthenticationScheme]));
            })
            .AllowAnonymous()
            .WithTags("Auth");
    }
}
