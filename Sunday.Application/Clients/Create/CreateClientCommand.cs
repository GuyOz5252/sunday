using Sunday.Application.Abstracts;

namespace Sunday.Application.Clients.Create;

public record CreateClientCommand(string Name, string AgencyId, string? AccountManagerId) : ICommand<string>;
