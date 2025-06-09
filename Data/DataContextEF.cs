using HelloWorld.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HelloWorld.Data;

public class DataContextEF : DbContext
{

    private IConfiguration _configuration;
    private string? _connectionString;
    public DataContextEF(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    public DbSet<Computer>? Computer { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"),
            opt => opt.EnableRetryOnFailure());

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("TutorialAppSchema");
        modelBuilder.Entity<Computer>(entity =>
        {
            // entity.HasNoKey();
            entity.HasKey(c => c.ComputerId);
            // entity.Property(c => c.Motherboard).IsRequired().HasMaxLength(100);
            // entity.Property(c => c.CPUCores).IsRequired();
            // entity.Property(c => c.HasWifi).IsRequired();
            // entity.Property(c => c.HasLTE).IsRequired();
            // entity.Property(c => c.ReleaseDate).IsRequired();
            // entity.Property(c => c.Price).HasColumnType("decimal(18,2)").IsRequired();
            // entity.Property(c => c.VideoCard).IsRequired().HasMaxLength(100);
        });
    }

}
