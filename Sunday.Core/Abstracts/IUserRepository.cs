using DotResults;
using Sunday.Core.Models;

namespace Sunday.Core.Abstracts;

public interface IUserRepository
{
    Task<Result<User>> GetUserAsync(string id);
}
