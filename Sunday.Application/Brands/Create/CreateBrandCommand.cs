using Sunday.Application.Abstracts;

namespace Sunday.Application.Brands.Create;

public record CreateBrandCommand(string Name, string ClientId) : ICommand<string>;
