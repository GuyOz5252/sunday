using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sunday.Api.Endpoints.Auth;
using Sunday.Data;
using Sunday.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAuthentication();
builder.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"), optionBuilder =>
    {
        optionBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly());
    });
});

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
