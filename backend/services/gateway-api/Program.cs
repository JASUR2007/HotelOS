using HotelOS.GatewayApi.Consumers;
using HotelOS.GatewayApi.Data;
using HotelOS.GatewayApi.Health;
using HotelOS.GatewayApi.Middleware;
using HotelOS.GatewayApi.Repositories;
using HotelOS.GatewayApi.Services;
using HotelOS.Shared.Startup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Configurations/YarpRoutes.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
});
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("room-service", client =>
{
    client.BaseAddress = new Uri("http://room-service:8084");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddScoped<HealthCheckService>();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("api", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 10,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        }));
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "HotelOS_dev_key_change_in_production_1234567890";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<GatewayDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        postgresOptions => { postgresOptions.EnableRetryOnFailure(); postgresOptions.MigrationsHistoryTable("__EFMigrationsHistory", "audit"); }));

builder.Services.AddScoped<IGatewayRepository, GatewayRepository>();
builder.Services.AddScoped<IGatewayService, GatewayService>();
builder.Services.AddScoped<IEventLogRepository, EventLogRepository>();
builder.Services.AddHostedService<AuditLogConsumer>();

var app = builder.Build();

await StartupTaskRunner.RunAsync("gateway database migration", () => DbInitializer.InitializeAsync(app.Services), app.Logger);

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
});
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseMiddleware<AuditLoggingMiddleware>();
app.UseMiddleware<PermissionMiddleware>();

app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/swagger.json");
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "docs";
    c.SwaggerEndpoint("/docs/v1/swagger.json", "Gateway API v1");
    c.SwaggerEndpoint("/service-docs/room-service/swagger/v1/swagger.json", "Room Service");
    c.SwaggerEndpoint("/service-docs/reception-service/swagger/v1/swagger.json", "Reception Service");
    c.SwaggerEndpoint("/service-docs/housekeeping-service/swagger/v1/swagger.json", "Housekeeping Service");
    c.SwaggerEndpoint("/service-docs/maintenance-service/swagger/v1/swagger.json", "Maintenance Service");
    c.SwaggerEndpoint("/service-docs/payment-service/swagger/v1/swagger.json", "Payment Service");
    c.SwaggerEndpoint("/service-docs/user-service/swagger/v1/swagger.json", "User Service");
});

app.MapControllers();
app.MapGet("/health", async (HealthCheckService healthService, CancellationToken cancellationToken) =>
    Results.Ok(await healthService.GetHealthAsync(cancellationToken)));
app.MapReverseProxy();

app.Run();

