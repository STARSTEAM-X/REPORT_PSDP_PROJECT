using Microsoft.EntityFrameworkCore;
using MyApi.Modals;

namespace MyApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<RepairCost> RepairCosts => Set<RepairCost>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enum → string ใน PostgreSQL
        modelBuilder
            .Entity<Report>()
            .Property(r => r.Status);

        // Report → Owner (User)
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Owner)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.ReportOwner)
            .OnDelete(DeleteBehavior.Restrict);

        // Report → Technician (User)
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Technician)
            .WithMany()
            .HasForeignKey(r => r.ReportTechnician)
            .OnDelete(DeleteBehavior.Restrict);

        // Report.ProgressLog เป็น JSONB
        modelBuilder.Entity<Report>()
            .Property(r => r.ProgressLog)
            .HasColumnType("jsonb");
        base.OnModelCreating(modelBuilder);

        // LocationName ต้องไม่ซ้ำกัน
        modelBuilder.Entity<Location>()
            .HasIndex(l => l.LocationName)
            .IsUnique();

        // AssetId ต้องไม่ซ้ำกัน
        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.AssetId)
            .IsUnique();

        // CategoryName ต้องไม่ซ้ำกัน
        modelBuilder.Entity<AssetCategory>()
            .HasIndex(c => c.CategoryName)
            .IsUnique();

        
    }
}