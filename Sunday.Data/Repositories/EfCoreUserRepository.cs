using DotResults;
using Microsoft.EntityFrameworkCore;
using Sunday.Core.Abstracts;
using Sunday.Core.DomainEvents;
using Sunday.Core.Models;

namespace Sunday.Data.Repositories;

public class EfCoreUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public EfCoreUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> GetUserAsync(string id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        return user is null
            ? Result.Failure<User>(new Error("User.NotFound", $"User with id {id} not found", "NotFound"))
            : Result.Success(user);
    }
}
