var builder = DistributedApplication.CreateBuilder(args);
// Redis Cache
var cache = builder.AddRedis("cache");

// Message Broker (RabbitMQ)
var messaging = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

// PostgreSQL Database  
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var elearningDb = postgres.AddDatabase("ElearningIskoop");

//// API projesine referanslar� ekleyin
//builder.AddProject<Projects.ELearningIskoop_API>("elearningiskoop-api")
//    .WithReference(elearningDb)     // Database ba�lant�s�
//    .WithReference(cache)           // Redis ba�lant�s�  
//    .WithReference(messaging);      // RabbitMQ ba�lant�s�

builder.Build().Run();
