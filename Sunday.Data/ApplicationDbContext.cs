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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>(builder =>
        {
            builder.HasKey(board => board.Id);
            builder.Property(board => board.Id).ValueGeneratedOnAdd();
            builder.Property(board => board.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder.OwnsMany(board => board.BoardMembers, boardMemberBuilder =>
            {
                boardMemberBuilder.WithOwner()
                    .HasForeignKey("BoardId");
                boardMemberBuilder.HasKey("BoardId", nameof(BoardMember.UserId));
                boardMemberBuilder.Property(boardMember => boardMember.UserId).IsRequired();
                boardMemberBuilder.Property(boardMember => boardMember.Role)
                    .IsRequired()
                    .HasConversion<string>();
            });
        });
    }
}
