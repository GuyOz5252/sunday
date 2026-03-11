using Microsoft.EntityFrameworkCore;
using Sunday.Core.Models;

namespace Sunday.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Board> Boards { get; init; }
    public DbSet<WorkSession> WorkSessions { get; init; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
