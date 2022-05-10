using System.Numerics;
using HSEProjectAppBackend.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

// TO DO: Solve database connection bug;

namespace HSEProjectAppBackend.Context;

public class ApplicationContext : DbContext
{
    ValueConverter converter = new ValueConverter<BigInteger, long>(
        model => (long)model,
        provider =>new BigInteger(provider)
        );


    private static readonly IConfigurationBuilder Builder =
        new ConfigurationBuilder().AddJsonFile(@"Config\DatabaseConnectionSettings.json");

    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Company> Companies { get; set; }

    public DbSet<BalanceSheet> BalanceSheets { get; set; }

    public DbSet<IncomeStatement> IncomeStatements { get; set; }

    public DbSet<CashFlow> CashFlows { get; set; }

    public DbSet<Metrics> Metrics { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Metrics model setup
        modelBuilder.Entity<Metrics>()
            .Property(p => p.MarketCap)
            .HasConversion(this.converter);

        // Income statement model setup
        modelBuilder.Entity<IncomeStatement>()
            .HasKey(b => b.Symbol);

        modelBuilder.Entity<IncomeStatement>()
            .Property(b => b.NetIncome)
            .HasConversion(this.converter);
        modelBuilder.Entity<IncomeStatement>()
            .Property(b => b.EBITDA)
            .HasConversion(this.converter);
        modelBuilder.Entity<IncomeStatement>()
            .Property(b => b.EBIT)
            .HasConversion(this.converter);
        modelBuilder.Entity<IncomeStatement>()
            .Property(b => b.IBT)
            .HasConversion(this.converter);

        // Balance sheet model setup
        modelBuilder.Entity<BalanceSheet>()
            .HasKey(b => b.Symbol);

        modelBuilder.Entity<BalanceSheet>()
            .Property(b => b.CurrentStock)
            .HasConversion(this.converter);
        modelBuilder.Entity<BalanceSheet>()
            .Property(b => b.TotalAssets)
            .HasConversion(this.converter);
        modelBuilder.Entity<BalanceSheet>()
            .Property(b => b.TotalCurrentAssets)
            .HasConversion(this.converter);
        modelBuilder.Entity<BalanceSheet>()
            .Property(b => b.TotalCurrentLiabilities)
            .HasConversion(this.converter);
        modelBuilder.Entity<BalanceSheet>()
            .Property(b => b.TotalLiabilities)
            .HasConversion(this.converter);

        // Cash flow model setup
        modelBuilder.Entity<CashFlow>()
            .HasKey(b => b.Symbol);

        modelBuilder.Entity<CashFlow>()
            .Property(b => b.OperatingCF)
            .HasConversion(this.converter);
        modelBuilder.Entity<CashFlow>()
            .Property(b => b.FinancingCF)
            .HasConversion(this.converter);
        modelBuilder.Entity<CashFlow>()
            .Property(b => b.InvestmentCF)
            .HasConversion(this.converter);
        modelBuilder.Entity<CashFlow>()
            .Property(b => b.ChangeInCF)
            .HasConversion(this.converter);

        // Company model setup
        modelBuilder.Entity<Company>()
            .HasKey(c => c.Symbol);

        modelBuilder.Entity<Company>()
            .HasOne(p => p.BalanceSheet)
            .WithOne(p => p.Company)
            .HasForeignKey<BalanceSheet>(b => b.Symbol);

        modelBuilder.Entity<Company>()
            .HasOne(b => b.IncomeStatement)
            .WithOne(b => b.Company)
            .HasForeignKey<IncomeStatement>(b => b.Symbol);

        modelBuilder.Entity<Company>()
            .HasOne(b => b.CashFlow)
            .WithOne(b => b.Company)
            .HasForeignKey<CashFlow>(b => b.Symbol);

        modelBuilder.Entity<Company>()
            .HasOne(b => b.Metrics)
            .WithOne(b => b.Company)
            .HasForeignKey<Metrics>(b => b.Symbol);
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