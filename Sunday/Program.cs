using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sunday.Api.Endpoints.Abstracts;
using Sunday.Application.Abstracts;
using Sunday.Application.Brands.Create;
using Sunday.Application.Campaigns.Create;
using Sunday.Application.Clients.Create;
using Sunday.Application.Decorators;
using Sunday.Application.Tickets.Create;
using Sunday.Application.Tickets.UpdateStatus;
using Sunday.Application.WorkSessions.Start;
using Sunday.Application.WorkSessions.Stop;
using Sunday.Core.Abstracts;
using Sunday.Data;
using Sunday.Data.Repositories;
using Sunday.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAuthentication();
builder.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("sunday-db"), optionBuilder =>
    {
        optionBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly());
    });
});

builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateTicketCommand>();
builder.Services.AddScoped<IAgencyRepository, EfCoreAgencyRepository>();
builder.Services.AddScoped<ITicketRepository, EfCoreTicketRepository>();
builder.Services.AddScoped<IClientRepository, EfCoreClientRepository>();
builder.Services.AddScoped<IUserRepository, EfCoreUserRepository>();

builder.Services.AddScoped<ICommandHandler<CreateClientCommand, string>, CreateClientCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreateBrandCommand, string>, CreateBrandCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreateCampaignCommand, string>, CreateCampaignCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreateTicketCommand, string>, CreateTicketCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateTicketStatusCommand>, UpdateTicketStatusCommandHandler>();
builder.Services.AddScoped<ICommandHandler<StartWorkSessionCommand, string>, StartWorkSessionCommandHandler>();
builder.Services.AddScoped<ICommandHandler<StopWorkSessionCommand>, StopWorkSessionCommandHandler>();

builder.Services.Decorate(typeof(ICommandHandler<>), typeof(ValidationCommandHandlerDecorator<>));
builder.Services.Decorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));

builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));

builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

var app = builder.Build();

app.MapServiceDefaults();

app.MapEndpoints(app.MapGroup("api"));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

await app.RunAsync();
