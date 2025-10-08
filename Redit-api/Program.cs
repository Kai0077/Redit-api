using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Redit_api.Data;
using Redit_api.Models.Status;
using Redit_api.Repositories;
using Redit_api.Repositories.Interfaces;
using Redit_api.Services;
using Redit_api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Redit_api.Models;

var builder = WebApplication.CreateBuilder(args);

Env.Load("Secret.env");

var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var database = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};Ssl Mode=Disable";

// Mapping Enum type to PG Enum
builder.Services.AddDbContext<AppDBContext>(opts =>
    opts.UseNpgsql(connectionString, npgsql =>
        npgsql.MapEnum<UserStatus>("user_status")));

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<UserDTO>, PasswordHasher<UserDTO>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();