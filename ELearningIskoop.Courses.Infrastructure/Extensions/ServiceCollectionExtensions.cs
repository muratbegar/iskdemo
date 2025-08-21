﻿using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Domain.Repositories;
using ELearningIskoop.Courses.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Courses.Infrastructure.Persistence;
using ELearningIskoop.Courses.Infrastructure.Repositories;
using ELearningIskoop.Courses.Infrastructure.Services;

namespace ELearningIskoop.Courses.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoursesInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            // ✅ DÜZELTME: DbContext with MediatR injection
            var connectionString = configuration.GetConnectionString("ElearningIskoop");
            services.AddDbContext<CoursesDbContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(CoursesDbContext).Assembly.FullName);
                    builder.EnableRetryOnFailure(maxRetryCount: 3);
                });

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            #region Repositories
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            #endregion

            // ✅ DÜZELTME: UnitOfWork registration
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Domain Services
            services.AddScoped<CourseDomainService>();

            // Infrastructure Services
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            return services;
        }

        public static async Task ApplyCourseMigrationAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
