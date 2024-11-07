using System.Reflection;
using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPhishing.Database;

public class PhishingDbContext : DbContext
{
    public const string MIGRATIONS_HISTORY_TABLE_NAME = "__EFMigrationsHistory";
    public const string SCHEMA_NAME = "ai_phishing";
    
    public PhishingDbContext()
    {
        
    }

    public PhishingDbContext(DbContextOptions<PhishingDbContext> options)
        : base(options)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        optionsBuilder
            .UseNpgsql(builder =>
            {
                builder.MigrationsHistoryTable(MIGRATIONS_HISTORY_TABLE_NAME, SCHEMA_NAME);
            })
            .UseSnakeCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SCHEMA_NAME);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Attack> Attacks { get; set; }
    // public DbSet<AttackTarget> AttackTargets { get; set; }
    public DbSet<AttackEmail> AttackEmails { get; set; }
    public DbSet<AttackEmailReply> AttackEmailReplies { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ClientTarget> ClientTargets { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
}