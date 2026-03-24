using Application.Features.Seguridad.Interfaces;
using Domain;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.BackGroundJobs.Jobs;
using Infrastructure.Configuration.Email;
using Infrastructure.Configuration.Hangfire;
using Infrastructure.Services;
using Infrastructure.Services.Notification.Email;
using Infrastructure.Services.Notification.Reports;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SqlDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });


            //Configurar opciones de Hangfire desde appsettings.json
            services.Configure<HangfireSettings>(
                configuration.GetSection(HangfireSettings.SectionName));

            services.AddHangfire((config) => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    SchemaName = "HangFire"
                })
            );

            //HagnFireServer(Procesa los trabajos en segundo plano)
            var hangfireSettings = configuration
                .GetSection(HangfireSettings.SectionName)
                .Get<HangfireSettings>() ?? new HangfireSettings();


            //services.UseHangfireDashboard("/hangfire-dashboard");
            services.AddHangfireServer(opt =>
            {
                opt.ServerName = hangfireSettings.ServerName;
                opt.WorkerCount = hangfireSettings.WorkerCount;
                opt.Queues = new[] { "default", "tracking-jobs" }; // Colas para diferentes tipos de trabajos
            });

            services.AddScoped<INotifyExpiringTasksJob, NotifyExpiringTasksJob>();


            // Services
            services.AddScoped<IJwtService, JwtService>();

            // Email Notification Services
            services.Configure<EmailSettings>(
                configuration.GetSection(EmailSettings.SectionName));
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            services.AddScoped<IExcelReportGenerator, ExcelReportGenerator>();

            // JWT Configuration
            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = "localhost";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            return services;
        }
    }
}
