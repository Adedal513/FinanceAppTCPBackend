using HSEProjectAppBackend.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// TO DO: Solve database connection bug;

namespace HSEProjectAppBackend.Context;

public class ApplicationContext : DbContext
{
    private static readonly IConfigurationBuilder Builder =
        new ConfigurationBuilder().AddJsonFile(@"Config\DatabaseConnectionSettings.json");

    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Company> Companies { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Company>()
            .HasOne(p => p.BalanceSheet)
            .WithOne(p => p.Company);

        modelBuilder.Entity<User>()
            .HasMany(p => p.Portfolios)
            .WithOne()
            .HasForeignKey(u => u.Uid);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var root = Builder.Build();

        var connectionString =
            "Host=ec2-34-242-8-97.eu-west-1.compute.amazonaws.com;Port=5432;Database=dehncl3mtaaib5;Username=pkpjijzdqxxplx;Password=password;";
        File.WriteAllText("Test.txt", connectionString);
        optionsBuilder.UseNpgsql(connectionString);
    }
}