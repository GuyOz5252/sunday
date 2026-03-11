using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Sunday.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void AddAuthentication()
        {
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_AUDIENCE")!)),
                    };
                })
                .AddGoogle(options =>
                {
                    // TODO: Exceptions
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new Exception();
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ??
                                           throw new Exception();
                });
        }

        public void AddAuthorization()
        {
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("Bearer", policy => policy.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme))
                .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
                .AddPolicy("User", policy => policy.RequireRole("User"));
        }
    }
}
