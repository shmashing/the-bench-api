using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TheBench.Logic.Models;

namespace TheBench.Logic.Database;

public class UserContext : DbContext
{
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = BuildConnectionString();
        optionsBuilder.UseNpgsql(connectionString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(u =>
        {
            u.HasIndex(p => p.UserId).IsUnique();
            u.Property(p => p.Schedule).HasConversion(
                s => JsonSerializer.Serialize(s, JsonSerializerOptions.Default),
                s => JsonSerializer.Deserialize<Schedule>(s, JsonSerializerOptions.Default) ?? Schedule.FullAvailability());
        });
        
        base.OnModelCreating(modelBuilder);
    }

    private static string BuildConnectionString()
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (databaseUrl == null) throw new NullReferenceException("Database url is missing from the environment");
        
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") return databaseUrl;
        
        var match = Regex.Match(databaseUrl, @"postgres://(.*):(.*)@(.*):(.*)/(.*)");
        return $"Server={match.Groups[3]};Port={match.Groups[4]};User Id={match.Groups[1]};Password={match.Groups[2]};Database={match.Groups[5]};sslmode=Prefer;Trust Server Certificate=true";
    }
}