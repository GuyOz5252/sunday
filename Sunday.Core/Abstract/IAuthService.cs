using DotResults;
using Sunday.Core.Models;

namespace Sunday.Core.Abstract;

public interface IAuthService
{
    Task<Result<AuthResult>> SignInAsync(LogInRequest request, CancellationToken ct = default);
    Task<Result<AuthResult>> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task<Result> RevokeAsync(string refreshToken, CancellationToken ct = default);
    Task<Result<User>> GetCurrentUserAsync(CancellationToken ct = default);
}

public record LogInRequest(string? Email, string? Password, string? OAuthCode, string? OAuthRedirectUri);
public record AuthResult(string UserId, string Token, List<string> Roles);
