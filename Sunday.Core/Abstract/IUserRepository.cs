using DotResults;
using Sunday.Core.Models;

namespace Sunday.Core.Abstract;

public interface IUserRepository
{
    Task<Result<User>> GetUserAsync(string id);
}
