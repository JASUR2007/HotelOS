using FluentValidation;
using FluentValidation.AspNetCore;
using HotelOS.RoomService.Data;
using HotelOS.RoomService.Middleware;
using HotelOS.RoomService.Repositories;
using HotelOS.RoomService.Seeders;
using HotelOS.RoomService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HotelOS.Shared.Startup;
using HotelOS.Shared.RabbitMQ;
using HotelOS.Shared.Audit;
using HotelOS.Shared.Storage;
using RabbitMQ.Client;
using Npgsql;
var builder = WebApplication.CreateBuilder(args);

#pragma warning disable CS0618
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
#pragma warning restore CS0618


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

var imagesPath = builder.Configuration["Storage:ImagesPath"] ?? Path.Combine(AppContext.BaseDirectory, "wwwroot", "images");
builder.Services.AddSingleton<IFileStorage>(_ => new LocalFileStorage(imagesPath));

builder.Services.AddDbContext<RoomDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        postgresOptions => { postgresOptions.EnableRetryOnFailure(); postgresOptions.MigrationsHistoryTable("__EFMigrationsHistory", "room_service"); })
    .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomQueries>(p => p.GetRequiredService<IRoomService>() as RoomService ?? throw new InvalidOperationException());
builder.Services.AddScoped<IRoomCommands>(p => p.GetRequiredService<IRoomService>() as RoomService ?? throw new InvalidOperationException());
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IKeyQueries>(_ => new KeyService(_.GetRequiredService<RoomDbContext>()));
builder.Services.AddScoped<IKeyCommands>(_ => new KeyService(_.GetRequiredService<RoomDbContext>()));
builder.Services.AddHostedService<HotelOS.RoomService.Consumers.RabbitMqRoomCleanedConsumer>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

await StartupTaskRunner.RunAsync("room database migration", () => DbInitializer.InitializeAsync(app.Services), app.Logger);
await StartupTaskRunner.RunAsync("room database seeding", () => DatabaseSeeder.SeedAsync(app.Services), app.Logger);

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

