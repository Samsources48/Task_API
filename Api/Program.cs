using Application.Exceptions;
using Application.Extensions;
using Domain;
using Hangfire;
using Infrastructure;
using Infrastructure.BackGroundJobs;
using Infrastructure.Configuration.Hangfire;
using Infrastructure.Seeders;
using Infrastructure.Services.Notification.Email;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Plant_HexArquitecture_API.Middlewares;
using Api.Middlewares;
using Scalar.AspNetCore;
using Api.Hubs;
using Application.Features.Notifications.Interfaces;
using Serilog;
using System.Reflection;
using System.Text.Json;
using Infrastructure.Services.NotificationSignal;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Iniciando API...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
    // Removed duplicate AddControllers

    // Configuración de OpenAPI
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Components ??= new Microsoft.OpenApi.Models.OpenApiComponents();
            document.Components.SecuritySchemes.Add("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });
            document.SecurityRequirements.Add(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            return Task.CompletedTask;
        });
    });

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddServicesLayer();
    builder.Services.AddScoped<IRealTimeNotifier, SignalRNotificationService>();


    builder.Services.AddCors(opc =>
    {
        opc.AddDefaultPolicy(opcionesCORS =>
        {
            opcionesCORS.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
    });


    var app = builder.Build();
    app.UseMiddleware<ExceptionMiddleware>();

    app.MapGet("/", async context =>
    {
        await context.Response.WriteAsync($"Api {Assembly.GetExecutingAssembly().GetName().Name!} trabajando");
    });



    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
        try
        {
            // Aplicar migraciones existentes a la base de datos
            dbContext.Database.Migrate();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                Log.Information("Se aplicaron las migraciones pendientes correctamente");
            }
            else
            {
                Log.Information("No hay migraciones pendientes para aplicar");
            }

            // Ejecutar Seeder
            await DbSeeder.SeedAsync(dbContext);
            Log.Information("Base de datos inicializada correctamente");
        }
        catch (Exception ex)
        {
            // Registrar el error pero continuar con la ejecucin
            Log.Warning(ex, "Error al aplicar migraciones automticamente. La aplicacin continuar ejecutndose.");
        }
    }

    //HANGFIRE
    var hangfireSettings = app.Services.GetRequiredService<IOptions<HangfireSettings>>().Value;

    if (hangfireSettings.EnableDashboard)
    {
        app.UseHangfireDashboard(hangfireSettings.DashboardPath, new DashboardOptions
        {
            // Sin autenticación para desarrollo
            // En producción, agregar: Authorization = new[] { new HangfireAuthFilter() }
            DashboardTitle = "Tracking RPA - Background Jobs",
            DisplayStorageConnectionString = false
        });

        Log.Information("📊 Hangfire Dashboard habilitado en: {Path}", hangfireSettings.DashboardPath);
    }
    // Registrar RecurringJobs después de que la app esté configurada
    HangFireJobSetup.ConfigureHangFireJobs(app.Services);

    Log.Information("[:----- Jobs recurrentes configurados -----:]");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/docs", options =>
        {
            options.OpenApiRoutePattern = "/openapi/v1.json";
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseMiddleware<SyncUserMiddleware>();
    app.UseCors();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<NotificationHub>("/hubs/notifications");
    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw ex;

}
finally
{
    Log.CloseAndFlushAsync();
}

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
