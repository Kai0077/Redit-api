using System.Text;
using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Redit_api.Data;
using Redit_api.Models;
using Redit_api.Models.Status;
using Redit_api.Repositories;
using Redit_api.Repositories.Interfaces;
using Redit_api.Services;
using Redit_api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load("Secret.env");

// Build PostgreSQL connection string
var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var database = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString =
    $"Host={host};Port={port};Database={database};Username={user};Password={password};Ssl Mode=Disable";

// ==========================================
// JWT Authentication Setup
// ==========================================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])
            ),

            ValidateLifetime = true, // ensures the token isn't expired
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// ==========================================
// Database + Enum Mapping
// ==========================================
builder.Services.AddDbContext<AppDBContext>(opts =>
    opts.UseNpgsql(connectionString, npgsql =>
        npgsql.MapEnum<UserStatus>("user_status")));

// ==========================================
// MVC, JSON, Swagger, DI
// ==========================================
builder.Services.AddControllers().AddJsonOptions(o =>
{
    // serialize enums as strings
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<UserDTO>, PasswordHasher<UserDTO>>();

// Optional: allow frontend access if using Vite/React/etc.
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//         policy.WithOrigins("http://localhost:5173") // your frontend URL
//               .AllowAnyHeader()
//               .AllowAnyMethod());
// });

var app = builder.Build();

// ==========================================
// Middleware Pipeline
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
// app.UseCors(); // enable if needed

app.UseAuthentication();   // <-- validates JWT tokens
app.UseAuthorization();    // <-- enforces [Authorize] attributes

app.MapControllers();

app.Run();