using Sunday.Application.Abstract;

namespace Sunday.Application.Brands.Create;

public record CreateBrandCommand(string Name, string ClientId) : ICommand<string>;
