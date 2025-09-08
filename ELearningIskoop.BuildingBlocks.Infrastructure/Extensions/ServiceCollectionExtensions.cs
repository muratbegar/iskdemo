
using ELearningIskoop.BuildingBlocks.Application.Services;
using ELearningIskoop.BuildingBlocks.Infrastructure.BackgroundServices;
using ELearningIskoop.BuildingBlocks.Infrastructure.Middleware;
using ELearningIskoop.BuildingBlocks.Infrastructure.Outbox;
using ELearningIskoop.Shared.Infrastructure.Outbox;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOutboxInfrastructure(this IServiceCollection services,
            IConfiguration configuration, string connectionString = "ElearningIskoop") // Parameter ismini düzelttim
        {
            // DbContext
            services.AddDbContext<OutboxDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("ElearningIskoop");
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(OutboxDbContext).Assembly.FullName);
                    builder.EnableRetryOnFailure(maxRetryCount: 3);
                });

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // Outbox services
            services.AddScoped<IOutboxService, OutboxService>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();

            // Options configuration
            services.Configure<OutboxProcessorOptions>(
                configuration.GetSection(OutboxProcessorOptions.SectionName));

            // Background service
            services.AddHostedService<OutboxEventProcessor>();

            return services;
        }
        public static async Task ApplyOutboxMigrationsAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();
            await context.Database.MigrateAsync();
        }
        public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
        {
            // HTTP Context Accessor (required for CurrentUserService and CorrelationIdService)
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            // Application Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICorrelationIdService, CorrelationIdService>();

            return services;
        }

       
    }

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBuildingBlocks(this IApplicationBuilder app)
        {
            // Correlation ID middleware'i pipeline'ın başına ekle
            app.UseMiddleware<CorrelationIdMiddleware>();
            return app;
        }
    }


}