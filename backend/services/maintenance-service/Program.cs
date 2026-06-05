using FluentValidation;
using FluentValidation.AspNetCore;
using HotelOS.MaintenanceService.Data;
using HotelOS.MaintenanceService.Middleware;
using HotelOS.MaintenanceService.Repositories;
using HotelOS.MaintenanceService.Seeders;
using HotelOS.MaintenanceService.Services;
using Microsoft.EntityFrameworkCore;
using HotelOS.Shared.Startup;
using HotelOS.Shared.RabbitMQ;
using HotelOS.Shared.Audit;
using HotelOS.Shared.Algorithms;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
});

builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
    Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672"),
    UserName = builder.Configuration["RabbitMQ:UserName"] ?? "guest",
    Password = builder.Configuration["RabbitMQ:Password"] ?? "guest",
    AutomaticRecoveryEnabled = true,
    NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
});

builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
builder.Services.AddSingleton<IAuditLogger, RabbitMqAuditLogger>();
builder.Services.AddSingleton<MaintenancePriorityQueue>();

builder.Services.AddDbContext<MaintenanceDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        postgresOptions => { postgresOptions.EnableRetryOnFailure(); postgresOptions.MigrationsHistoryTable("__EFMigrationsHistory", "maintenance"); }));

builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IMaintenanceQueries>(p => p.GetRequiredService<IMaintenanceService>() as MaintenanceService ?? throw new InvalidOperationException());
builder.Services.AddScoped<IMaintenanceCommands>(p => p.GetRequiredService<IMaintenanceService>() as MaintenanceService ?? throw new InvalidOperationException());

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

await StartupTaskRunner.RunAsync("maintenance database migration", () => DbInitializer.InitializeAsync(app.Services), app.Logger);
await StartupTaskRunner.RunAsync("maintenance database seeding", () => DatabaseSeeder.SeedAsync(app.Services), app.Logger);

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

