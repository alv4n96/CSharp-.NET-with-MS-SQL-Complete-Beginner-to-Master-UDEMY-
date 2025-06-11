using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data;

public class DataContextEF : DbContext
{
    private readonly IConfiguration _config;
    public DataContextEF(IConfiguration config) : base(new DbContextOptionsBuilder<DataContextEF>().UseSqlServer(config.GetConnectionString("DefaultConnection")).Options)
    {
        _config = config;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserSalary> UserSalary { get; set; }
    public DbSet<UserJobInfo> UserJobInfo { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("TutorialAppSchema");

        modelBuilder.Entity<User>()
            .ToTable("Users", "TutorialAppSchema")
            .HasKey(u => u.UserId);

        modelBuilder.Entity<UserJobInfo>()
            .ToTable("UserJobInfo", "TutorialAppSchema")
            .HasKey(u => u.UserId);

        modelBuilder.Entity<UserSalary>()
            .ToTable("UserSalary", "TutorialAppSchema")
            .HasKey(u => u.UserId);
    }
}