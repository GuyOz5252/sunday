using Microsoft.EntityFrameworkCore;
using Sunday.Core.Abstract;
using Sunday.Core.Models;
using DotResults;

namespace Sunday.Data;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public DbSet<User> Users { get; init; }
    public DbSet<Board> Boards { get; init; }
    public DbSet<Ticket> Tickets { get; init; }
    public DbSet<WorkSession> WorkSessions { get; init; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public async Task<Result> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Database.Error", ex.Message, "InternalError"));
        }
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

        modelBuilder.Entity<Ticket>(builder =>
        {
            builder.HasKey(ticket => ticket.Id);
            builder.Property(ticket => ticket.Id).ValueGeneratedOnAdd();
            builder.Property(ticket => ticket.Name).IsRequired().HasMaxLength(100);
            builder.Property(ticket => ticket.Details).IsRequired();
            builder.OwnsOne(ticket => ticket.Assignee, assigneeBuilder =>
            {
                assigneeBuilder.WithOwner()
                    .HasForeignKey(ticketAssignee => ticketAssignee.TicketId);
                assigneeBuilder.HasKey(ticketAssignee => new { ticketAssignee.TicketId, ticketAssignee.UserId });
                assigneeBuilder.Property(ticketAssignee => ticketAssignee.UserId).IsRequired();
            });
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Id).ValueGeneratedOnAdd();
            builder.Property(user => user.Username).IsRequired().HasMaxLength(50);
            builder.Property(user => user.Email).IsRequired().HasMaxLength(100);
            builder.Property(user => user.PasswordHash).IsRequired();
            builder.Property(user => user.Roles).IsRequired();
        });
    }
}
