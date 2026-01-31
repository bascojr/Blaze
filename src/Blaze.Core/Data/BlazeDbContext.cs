using Blaze.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Blaze.Core.Data;

/// <summary>
/// Entity Framework database context for Blaze
/// </summary>
public class BlazeDbContext : DbContext
{
    public DbSet<Application> Applications { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<InstalledApp> InstalledApps { get; set; } = null!;
    public DbSet<Download> Downloads { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    private readonly string _connectionString;

    public BlazeDbContext()
    {
        _connectionString = "Server=localhost;Database=BlazeDb;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    public BlazeDbContext(DbContextOptions<BlazeDbContext> options) : base(options)
    {
        _connectionString = "Server=localhost;Database=BlazeDb;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Application configuration
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.Property(e => e.ScreenshotUrls)
                .HasConversion(
                    v => string.Join('|', v),
                    v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // InstalledApp configuration
        modelBuilder.Entity<InstalledApp>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Ignore(e => e.LaunchHistory);
        });

        // Download configuration
        modelBuilder.Entity<Download>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Review configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrls)
                .HasConversion(
                    v => string.Join('|', v),
                    v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.Ignore(e => e.DeveloperResponse);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OwnedAppIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.Property(e => e.WishlistAppIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.Property(e => e.FavoriteAppIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.Ignore(e => e.Preferences);
        });

    }
}
