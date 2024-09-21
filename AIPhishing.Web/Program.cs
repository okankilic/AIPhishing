using System.Text.Json.Serialization;
using AIPhishing.Business.Extensions;
using AIPhishing.Database;
using AIPhishing.Web.BackgroundServices;
using AIPhishing.Web.Handlers;
using AIPhishing.Web.Requirements;
using AIPhishing.Web.Validations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information($"AIPhishing.Web starting...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddCors();

    builder.Host.UseSerilog((hostContext, configuration) =>
    {
        configuration
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.PostgreSQL(
                connectionString: hostContext.Configuration.GetConnectionString("DbConnection"),
                tableName: "errors",
                needAutoCreateTable: true,
                schemaName: PhishingDbContext.SCHEMA_NAME);
    });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient();
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiKeyPolicy", policy =>
        {
            policy.AddAuthenticationSchemes([
                JwtBearerDefaults.AuthenticationScheme
            ]);
            policy.Requirements.Add(new ApiKeyRequirement());
        });
    });

    builder.Services.AddScoped<IApiKeyValidation, ApiKeyValidation>();
    builder.Services.AddScoped<IAuthorizationHandler, ApiKeyHandler>();

    builder.Services.ConfigureBusinessServices(builder.Configuration);

    // builder.Services.AddHostedService<EmailService>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "ApiKey must appear in header",
            Name = ApiKeyValidation.ApiKeyHeaderName,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKeyScheme"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    builder.Services.AddHealthChecks();
    builder.Services.AddHttpLogging(o => { });

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    var app = builder.Build();

    app.UseHttpLogging();

// Configure the HTTP request pipeline.
    // if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseExceptionHandler(o => {});
    // app.UseAuthentication();
    // app.UseAuthorization();

    app.UseCors(o =>
    {
        o.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

    app.MapControllers();
    app.MapHealthChecks("/healthz");
    
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<PhishingDbContext>();
        
        db.Database.Migrate();
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}