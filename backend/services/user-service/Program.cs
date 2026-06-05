using FluentValidation;
using FluentValidation.AspNetCore;
using HotelOS.UserService.Data;
using HotelOS.UserService.Middleware;
using HotelOS.UserService.Repositories;
using HotelOS.UserService.Seeders;
using HotelOS.UserService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelOS.Shared.Startup;

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
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        postgresOptions => { postgresOptions.EnableRetryOnFailure(); postgresOptions.MigrationsHistoryTable("__EFMigrationsHistory", "users"); }));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

await StartupTaskRunner.RunAsync("user database migration", () => DbInitializer.InitializeAsync(app.Services), app.Logger);
await StartupTaskRunner.RunAsync("user database seeding", () => DatabaseSeeder.SeedAsync(app.Services), app.Logger);

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

