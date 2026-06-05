using FluentValidation;
using FluentValidation.AspNetCore;
using HotelOS.PaymentService.BackgroundServices;
using HotelOS.PaymentService.Data;
using HotelOS.PaymentService.Middleware;
using HotelOS.PaymentService.Repositories;
using HotelOS.PaymentService.Seeders;
using HotelOS.PaymentService.Services;
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

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        postgresOptions => postgresOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentQueries>(p => p.GetRequiredService<IPaymentService>() as PaymentService ?? throw new InvalidOperationException());
builder.Services.AddScoped<IPaymentCommands>(p => p.GetRequiredService<IPaymentService>() as PaymentService ?? throw new InvalidOperationException());
builder.Services.AddSingleton<BillingCalculationService>();

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

builder.Services.AddHostedService<RabbitMqPaymentConsumer>();
builder.Services.AddHostedService<ReservationExpirationService>();

builder.Services.AddHttpClient("gateway", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Gateway:BaseUrl"] ?? "http://gateway-api:8080");
    client.Timeout = TimeSpan.FromSeconds(10);
}).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await StartupTaskRunner.RunAsync("payment database migration", () => DbInitializer.InitializeAsync(app.Services), app.Logger);
await StartupTaskRunner.RunAsync("payment database seeding", () => DatabaseSeeder.SeedAsync(app.Services), app.Logger);
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();


