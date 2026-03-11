using System.Text;
using Sunday.Application.Abstract;
using Sunday.Application.Decorators;
using Sunday.Application.Users.Register;
using Sunday.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Scan(scan => scan.FromAssembliesOf(typeof(RegisterUserCommand))
    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(Sunday.Application.Abstract.ICommandHandler<>)),
        publicOnly: false)
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(Sunday.Application.Abstract.ICommandHandler<,>)),
        publicOnly: false)
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.Decorate(typeof(Sunday.Application.Abstract.ICommandHandler<,>),
    typeof(ValidationDecorator.CommandHandler<,>));
builder.Services.Decorate(typeof(Sunday.Application.Abstract.ICommandHandler<>),
    typeof(ValidationDecorator.CommandHandler<>));
builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationDecorator.QueryHandler<,>));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Bearer", policy => policy.RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme))
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
    .AddPolicy("User", policy => policy.RequireRole("User"));

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(options =>
{
    options.ShortSchemaNames = true;
    options.DocumentSettings = settings =>
    {
        settings.DocumentName = "v1";
        settings.Title = "Sunday API";
        settings.Version = "v1";
    };
});

var app = builder.Build();

app.MapServiceDefaults();

app.MapFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Errors.UseProblemDetails();
});

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUI(options => options.DocumentTitle = "Sunday API");
    // using var scope = app.Services.CreateScope();
    // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // await dbContext.Database.MigrateAsync();
}

await app.RunAsync();
