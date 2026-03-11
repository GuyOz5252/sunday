using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sunday.Api.Endpoints.Auth;
using Sunday.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new Exception();
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Bearer", policy => policy.RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme))
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
    .AddPolicy("User", policy => policy.RequireRole("User"));

builder.Services.AddEndpoints(typeof(LoginWithGoogleEndpoint).Assembly);

var app = builder.Build();

app.MapServiceDefaults();

app.MapEndpoints();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    // using var scope = app.Services.CreateScope();
    // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // await dbContext.Database.MigrateAsync();
}

await app.RunAsync();
