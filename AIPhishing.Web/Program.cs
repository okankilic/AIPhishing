using System.Text.Json.Serialization;
using AIPhishing.Business.Configurations;
using AIPhishing.Business.Extensions;
using AIPhishing.Common.Helpers;
using AIPhishing.Database;
using AIPhishing.Database.Entities;
using AIPhishing.Web.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        .AddAuthentication(JwtAuthHandler.AuthenticationScheme)
        .AddScheme<JwtAuthHandlerSchemeOptions, JwtAuthHandler>(JwtAuthHandler.AuthenticationScheme, null);

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiKeyPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.AddAuthenticationSchemes([
                JwtAuthHandler.AuthenticationScheme
            ]);
        });
    });

    builder.Services.ConfigureBusinessServices(builder.Configuration);

    // builder.Services.AddHostedService<EmailService>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "ApiKey must appear in header",
            Name = JwtAuthHandler.ApiKeyHeaderName,
            Type = SecuritySchemeType.ApiKey,
            Scheme = JwtAuthHandler.AuthenticationScheme
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
        .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
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
    app.UseExceptionHandler(o => { });
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

        if (db.Database.IsRelational() && app.Environment.IsDevelopment())
            db.Database.Migrate();

        var adminConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<AdminConfiguration>>();

        var godUser = db.Users.SingleOrDefault(q => q.ClientId == null);

        if (godUser == null)
        {
            godUser = new User
            {
                Id = adminConfiguration.Value.Id,
                Email = adminConfiguration.Value.UserName,
                Password = PasswordHelper.Hash(adminConfiguration.Value.Password),
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(godUser);

            db.SaveChanges();
        }
        else
        {
            if (godUser.Email != adminConfiguration.Value.UserName
                || !PasswordHelper.Verify(adminConfiguration.Value.Password, godUser.Password))
            {
                godUser.Email = adminConfiguration.Value.UserName;
                godUser.Password = PasswordHelper.Hash(adminConfiguration.Value.Password);

                db.Users.Update(godUser);

                db.SaveChanges();
            }
        }
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