using FluentValidation;
using FluentValidation.AspNetCore;
using HotelOS.HousekeepingService.Data;
using HotelOS.HousekeepingService.Middleware;
using HotelOS.HousekeepingService.Repositories;
using HotelOS.HousekeepingService.Seeders;
using HotelOS.HousekeepingService.Services;
using Microsoft.EntityFrameworkCore;
using HotelOS.Shared.Startup;
using HotelOS.Shared.RabbitMQ;
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

builder.Services.AddDbContext<HousekeepingDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        postgresOptions => postgresOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IHousekeepingRepository, HousekeepingRepository>();
builder.Services.AddScoped<IHousekeepingService, HousekeepingService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await StartupTaskRunner.RunAsync("housekeeping database migration", () => DbInitializer.InitializeAsync(app.Services), app.Logger);
await StartupTaskRunner.RunAsync("housekeeping database seeding", () => DatabaseSeeder.SeedAsync(app.Services), app.Logger);
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();


