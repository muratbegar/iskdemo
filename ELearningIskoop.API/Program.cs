// Program.cs - Birleþtirilmiþ API Startup
using ELearningIskoop.API.Middleware;
using ELearningIskoop.API.RateLimiting;
using ELearningIskoop.API.Swagger;
using ELearningIskoop.API.Versioning;
using ELearningIskoop.BuildingBlocks.Infrastructure.Extensions;
using ELearningIskoop.BuildingBlocks.Infrastructure.Outbox;
using ELearningIskoop.Courses.Application.Extensions;
using ELearningIskoop.Courses.Infrastructure.Extensions;
using ELearningIskoop.Courses.Infrastructure.Seeds;
using ELearningIskoop.Users.Application.Extensions;
using ELearningIskoop.Users.Infrastructure.Extensions;
using ELearningIskoop.Users.Infrastructure.Seeds;
using ELeraningIskoop.ServiceDefaults.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity; // AddServiceDefaults
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// JWT Authentication
var jwtConfig = builder.Configuration.GetSection("ELearningIskoop:Jwt");
var secretKey = jwtConfig["SecretKey"];
var issuer = jwtConfig["Issuer"];
var audience = jwtConfig["Audience"];
// =====================================
// 0. Aspire Service Defaults
// =====================================
builder.AddServiceDefaults(); // Serilog + OpenTelemetry + HealthChecks + Service Discovery

// Shared Outbox(PostgreSQL connection string name'i)
builder.Services.AddOutboxInfrastructure(builder.Configuration);
builder.Services.AddDbContext<OutboxDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OutboxDb")));

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
builder.Services.AddUsersApplication();
builder.Services.AddUsersInfrastructure(builder.Configuration);
// =====================================
// 4. API Infrastructure (Swagger, Versioning, Rate Limiting)
// =====================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAdvancedSwagger();
// Minimal Swagger
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true; // Production için true
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
builder.Services.AddAuthorization();
//builder.Services.AddApiVersioningConfiguration();
builder.Services.AddRateLimitingConfiguration();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
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
    //await CoursesDataSeeder.SeedAsync(app.Services);
    //await UserDataSeeder.SeedAsync(app.Services);
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
