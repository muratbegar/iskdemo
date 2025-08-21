// Program.cs - Birleþtirilmiþ API Startup
using ELearningIskoop.API.Middleware;
using ELearningIskoop.API.RateLimiting;
using ELearningIskoop.API.Swagger;
using ELearningIskoop.API.Versioning;
using ELearningIskoop.Courses.Application.Extensions;
using ELearningIskoop.Courses.Infrastructure.Extensions;
using ELearningIskoop.Courses.Infrastructure.Seeds;
using ELeraningIskoop.ServiceDefaults.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity; // AddServiceDefaults
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// 0. Aspire Service Defaults
// =====================================
builder.AddServiceDefaults(); // Serilog + OpenTelemetry + HealthChecks + Service Discovery

// =====================================
// 1. Application Layer (MediatR)
// =====================================
builder.Services.AddCourseApplication(); // Contains MediatR registration

// =====================================
// 2. Infrastructure Layer (DbContexts)
// =====================================
builder.Services.AddCoursesInfrastructure(builder.Configuration);


// =====================================
// 3. Authentication & Authorization
// =====================================


// =====================================
// 4. API Infrastructure (Swagger, Versioning, Rate Limiting)
// =====================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAdvancedSwagger();
// Minimal Swagger

//builder.Services.AddApiVersioningConfiguration();
builder.Services.AddRateLimitingConfiguration();

// =====================================
// 5. CORS
// =====================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// =====================================
// Middleware Pipeline
// =====================================

// 1. Exception handling middleware first
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2. Aspire default endpoints (health checks, metrics)
//app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    // Veri seed iþlemleri (isteðe baðlý)
    await CoursesDataSeeder.SeedAsync(app.Services);
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ELearning API v1");
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
//app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

// =====================================
// 6. Apply Migrations on Startup
// =====================================
//try
//{
//    await app.Services.ApplyCourseMigrationAsync();
//    await app.Services.ApplyUserMigrationsAsync();
//}
//catch (Exception ex)
//{
//    var logger = app.Services.GetRequiredService<ILogger<Program>>();
//    logger.LogError(ex, "An error occurred while applying migrations");
//    throw;
//}

// =====================================
// 7. Start Application
// =====================================
Log.Information("ELearning Platform API starting...");
try
{
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
