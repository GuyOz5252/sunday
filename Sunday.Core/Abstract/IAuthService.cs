using System.Security.Claims;
using DotResults;

namespace Sunday.Core.Abstract;

public interface IAuthService
{
    Task<Result<LoginUserResult>> LoginUserAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<Result<ClaimsPrincipal>> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}

public record LoginUserResult(string UserId, string Token, List<string> Roles);
