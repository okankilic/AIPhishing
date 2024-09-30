using AIPhishing.Business.Attacks;
using AIPhishing.Business.Auth;
using AIPhishing.Business.Clients;
using AIPhishing.Business.Configurations;
using AIPhishing.Business.Emails;
using AIPhishing.Business.Enums;
using AIPhishing.Business.Integrations;
using AIPhishing.Business.Mocks;
using AIPhishing.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIPhishing.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PhishingDbContext>(dbContextOptions =>
        {
            dbContextOptions
                // .UseInMemoryDatabase("ai_phishing")
                // .ConfigureWarnings(w =>
                // {
                //     w.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                // });
                .UseNpgsql(configuration.GetConnectionString("DbConnection"), builder =>
                {
                    builder.MigrationsHistoryTable(PhishingDbContext.MIGRATIONS_HISTORY_TABLE_NAME, PhishingDbContext.SCHEMA_NAME);
                })
                .UseSnakeCaseNamingConvention();
        });
        
        services.Configure<EmailConfiguration>(configuration.GetSection(nameof(EmailConfiguration)));
        services.Configure<PhishingAiConfiguration>(configuration.GetSection(nameof(PhishingAiConfiguration)));
        services.Configure<AdminConfiguration>(configuration.GetSection(nameof(AdminConfiguration)));
        services.Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));
        
        services.AddScoped<IAttackBusiness, AttackBusiness>();
        services.AddScoped<IEmailBusiness, EmailBusiness>();
        services.AddScoped<IAuthBusiness, AuthBusiness>();
        services.AddScoped<IEnumBusiness, EnumBusiness>();
        services.AddScoped<IClientBusiness, ClientBusiness>();

        var useMockServices = configuration.GetValue<bool>("UseMockServices");

        if (useMockServices)
        {
            services.AddScoped<ICrmApiClient, MockCrmApiClient>();
            services.AddScoped<IPhishingAiApiClient, MockPhishingAiApiClient>();   
        }
        else
        {
            services.AddScoped<IPhishingAiApiClient, PhishingAiApiClient>();
        }
    }
}