using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TheBench.Logic.Models;

namespace TheBench.Logic.Database;

public class UserContext : DbContext
{
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamInvitation> TeamInvitations { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<SubstituteRequest> SubstituteRequests { get; set; }

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
            u.HasIndex(p => p.Id).IsUnique();
            u.HasIndex(p => p.Auth0Id).IsUnique();
            u.HasIndex(p => p.Email).IsUnique();
        });
        
        modelBuilder.Entity<Team>(team =>
        {
            team.HasKey(t => t.Id);
            team.Property(t => t.ManagerIds).HasConversion(
                list => JsonSerializer.Serialize(list, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<List<string>>(json, JsonSerializerOptions.Default) ?? new List<string>());
            
            team.Property(t => t.MemberIds).HasConversion(
                list => JsonSerializer.Serialize(list, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<List<string>>(json, JsonSerializerOptions.Default) ?? new List<string>());
        });
        
        modelBuilder.Entity<TeamInvitation>(invite =>
        {
            invite.HasKey(i => i.Id);
            invite.HasIndex(i => new { i.TeamId, i.InviteeEmail, i.Status });
        });

        modelBuilder.Entity<SubstituteRequest>(request =>
        {
            request.HasKey(r => r.Id);
            request.HasIndex(r => new { r.TeamId, r.GameId }).IsUnique();
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