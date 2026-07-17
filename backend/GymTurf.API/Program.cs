using System.Text.Json.Serialization;
using FluentValidation;
using GymTurf.Application.Interfaces;
using GymTurf.Application.Validators;
using GymTurf.Domain.Interfaces;
using GymTurf.Infrastructure.Data;
using GymTurf.Infrastructure.Repositories;
using GymTurf.Infrastructure.Services;
using GymTurf.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentMigrator.Runner;

var builder = WebApplication.CreateBuilder(args);

// Connection Strings & Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=gymturf;Username=postgres;Password=SecretPassword123!;";

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "VerySuperSecretSecuredKeyOfAtLeast32BytesLength!";
var issuer = jwtSettings["Issuer"] ?? "GymTurfAPI";
var audience = jwtSettings["Audience"] ?? "GymTurfClient";
var expiryInMinutes = int.TryParse(jwtSettings["ExpiryInMinutes"], out var exp) ? exp : 120;

// Register Services
builder.Services.AddScoped<IUnitOfWork>(sp => new UnitOfWork(connectionString));
builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(sp.GetRequiredService<IUnitOfWork>()));
builder.Services.AddScoped<IBranchRepository>(sp => new BranchRepository(sp.GetRequiredService<IUnitOfWork>()));
builder.Services.AddScoped<IFacilityRepository>(sp => new FacilityRepository(sp.GetRequiredService<IUnitOfWork>()));
builder.Services.AddScoped<IMemberRepository>(sp => new MemberRepository(sp.GetRequiredService<IUnitOfWork>()));
builder.Services.AddScoped<IBookingRepository>(sp => new BookingRepository(sp.GetRequiredService<IUnitOfWork>()));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBranchFacilityService, BranchFacilityService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddSingleton<IJwtTokenGenerator>(sp =>
    new JwtTokenGenerator(secretKey, issuer, audience, expiryInMinutes));

builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// FluentMigrator configuration
builder.Services.AddLogging(c => c.AddFluentMigratorConsole())
    .AddFluentMigratorCore()
    .ConfigureRunner(c => c
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(CreateUsersTable).Assembly).For.Migrations());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gym & Turf Management Platform API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

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

// Execute Migrations
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    try
    {
        runner.MigrateUp();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error running migrations: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gym & Turf Management Platform API v1");
    c.RoutePrefix = "swagger";
});

// Global Exception Handling Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
