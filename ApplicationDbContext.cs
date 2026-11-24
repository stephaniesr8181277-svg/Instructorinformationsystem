using Microsoft.EntityFrameworkCore;
using InstructorInformationSystem.Models;

namespace InstructorInformationSystem;

/// <summary>
/// Represents the SQLite database used by the application.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserAccount>()
            .HasIndex(account => account.Email)
            .IsUnique();

        modelBuilder.Entity<Instructor>()
            .HasIndex(instructor => instructor.EmailAddress)
            .IsUnique();
    }
}
