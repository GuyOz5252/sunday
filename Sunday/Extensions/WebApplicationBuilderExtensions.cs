using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Sunday.Api.Authorization;

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
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new Exception();
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ??
                                           throw new Exception();
                });
        }

        public void AddAuthorization()
        {
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, TicketAccessHandler>();

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("Bearer", policy => policy.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme))
                .AddPolicy("Admin", policy => policy.RequireRole("SystemAdmin", "AgencyAdmin"))
                .AddPolicy("SystemAdmin", policy => policy.RequireRole("SystemAdmin"))
                .AddPolicy("AgencyAdmin", policy => policy.RequireRole("SystemAdmin", "AgencyAdmin"))
                .AddPolicy("TrafficManager", policy => policy.RequireRole("SystemAdmin", "AgencyAdmin", "TrafficManager"))
                .AddPolicy("CreativeManager", policy => policy.RequireRole("SystemAdmin", "AgencyAdmin", "CreativeManager"))
                .AddPolicy("AccountManager", policy => policy.RequireRole("SystemAdmin", "AgencyAdmin", "AccountManager"))
                .AddPolicy("CreateTicket", policy => policy.RequireRole("SystemAdmin", "AgencyAdmin", "TrafficManager", "AccountManager"))
                .AddPolicy("TicketAccess", policy => policy.AddRequirements(new TicketAccessRequirement()));
        }
    }
}
