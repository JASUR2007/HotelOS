using FluentValidation;
using FluentValidation.AspNetCore;
using HotelOS.ReceptionService.Data;
using HotelOS.ReceptionService.Middleware;
using HotelOS.ReceptionService.Repositories;
using HotelOS.ReceptionService.Seeders;
using HotelOS.ReceptionService.Services;
using Microsoft.EntityFrameworkCore;
using HotelOS.Shared.Startup;
using HotelOS.Shared.RabbitMQ;
using HotelOS.Shared.Audit;
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

builder.Services.AddHttpClient("room-service", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["RoomService:BaseUrl"] ?? "http://room-service:8084");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("payment-service", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PaymentService:BaseUrl"] ?? "http://payment-service:8087");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddDbContext<ReceptionDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        postgresOptions => { postgresOptions.EnableRetryOnFailure(); postgresOptions.MigrationsHistoryTable("__EFMigrationsHistory", "reception"); }));

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IReceptionService, ReceptionService>();
builder.Services.AddScoped<IReceptionQueries>(p => p.GetRequiredService<IReceptionService>() as ReceptionService ?? throw new InvalidOperationException());
builder.Services.AddScoped<IReceptionCommands>(p => p.GetRequiredService<IReceptionService>() as ReceptionService ?? throw new InvalidOperationException());

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();

await StartupTaskRunner.RunAsync("reception database migration", () => DbInitializer.InitializeAsync(app.Services), app.Logger);
await StartupTaskRunner.RunAsync("reception database seeding", () => DatabaseSeeder.SeedAsync(app.Services), app.Logger);
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

