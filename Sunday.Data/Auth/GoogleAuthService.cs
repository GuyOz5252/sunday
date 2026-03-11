using DotResults;
using Sunday.Core.Abstract;
using Sunday.Core.Models;

namespace Sunday.Data.Auth;

public class GoogleAuthService : IAuthService
{
    private readonly IGoogleOAuthClient _googleClient;
    
    public async Task<Result<AuthResult>> SignInAsync(LogInRequest request, CancellationToken ct = default)
    {
        if (request.OAuthCode is null)
        {
            return new Error("", "", "");
        }

        var googleUser = await googleClient.ExchangeCodeAsync(request.OAuthCode, request.OAuthRedirectUri!, ct);
        if (googleUser is null)
            return AuthResult.Failure("Google auth failed.");

        // Upsert user from Google profile
        var user = await users.FindByEmailAsync(googleUser.Email, ct)
                   ?? await users.CreateFromOAuthAsync(googleUser, ct);

        var authUser = new AuthUser(user.Id, user.Email, user.DisplayName, user.Roles);
        var (access, refresh) = tokenGen.Generate(authUser);
        return AuthResult.Success(access, refresh, authUser);
    }

    public Task<Result<AuthResult>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RevokeAsync(string refreshToken, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> GetCurrentUserAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
