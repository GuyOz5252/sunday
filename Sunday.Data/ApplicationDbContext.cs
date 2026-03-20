using Microsoft.EntityFrameworkCore;
using Sunday.Core.Models;
using DotResults;
using Sunday.Core.Abstracts;

namespace Sunday.Data;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public DbSet<Agency> Agencies { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<Client> Clients { get; init; }
    public DbSet<Brand> Brands { get; init; }
    public DbSet<Campaign> Campaigns { get; init; }
    public DbSet<Ticket> Tickets { get; init; }
    public DbSet<TicketAssignment> TicketAssignments { get; init; }
    public DbSet<TicketWatcher> TicketWatchers { get; init; }
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
        modelBuilder.Entity<Agency>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Slug).IsRequired().HasMaxLength(50);
            builder.HasIndex(a => a.Slug).IsUnique();
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Roles).HasConversion(
                r => string.Join(',', r),
                r => r.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<Role>).ToList());

            builder.HasOne(u => u.Agency)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Client>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            
            builder.HasOne(c => c.Agency)
                .WithMany(a => a.Clients)
                .HasForeignKey(c => c.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.AccountManager)
                .WithMany()
                .HasForeignKey(c => c.AccountManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Brand>(builder =>
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();
            builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
            
            builder.HasOne(b => b.Client)
                .WithMany(c => c.Brands)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Campaign>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            
            builder.HasOne(c => c.Brand)
                .WithMany(b => b.Campaigns)
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ticket>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Brief).IsRequired();
            builder.Property(t => t.Status).HasConversion<string>();

            builder.HasOne(t => t.Agency)
                .WithMany()
                .HasForeignKey(t => t.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Client)
                .WithMany()
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(t => t.DomainEvents);
        });

        modelBuilder.Entity<TicketComment>(builder =>
        {
            builder.HasKey(tc => tc.Id);
            builder.Property(tc => tc.Id).ValueGeneratedOnAdd();
            builder.Property(tc => tc.Content).IsRequired();

            builder.HasOne(tc => tc.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(tc => tc.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tc => tc.Author)
                .WithMany()
                .HasForeignKey(tc => tc.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TicketAssignment>(builder =>
        {
            builder.HasKey(ta => ta.Id);
            builder.Property(ta => ta.Id).ValueGeneratedOnAdd();
            builder.Property(ta => ta.Role).HasConversion<string>();

            builder.HasOne(ta => ta.Ticket)
                .WithMany(t => t.Assignments)
                .HasForeignKey(ta => ta.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ta => ta.User)
                .WithMany()
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketWatcher>(builder =>
        {
            builder.HasKey(tw => new { tw.TicketId, tw.UserId });

            builder.HasOne(tw => tw.Ticket)
                .WithMany(t => t.Watchers)
                .HasForeignKey(tw => tw.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tw => tw.User)
                .WithMany()
                .HasForeignKey(tw => tw.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkSession>(builder =>
        {
            builder.HasKey(te => te.Id);
            builder.Property(te => te.Id).ValueGeneratedOnAdd();

            builder.HasOne(te => te.Ticket)
                .WithMany()
                .HasForeignKey(te => te.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(te => te.User)
                .WithMany()
                .HasForeignKey(te => te.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
