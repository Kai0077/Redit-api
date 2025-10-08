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

// Load env vars
Env.Load("Secret.env");

string host = Environment.GetEnvironmentVariable("DB_HOST")!;
string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
string database = Environment.GetEnvironmentVariable("DB_NAME")!;
string user = Environment.GetEnvironmentVariable("DB_USER")!;
string password = Environment.GetEnvironmentVariable("DB_PASSWORD")!;

string connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};Ssl Mode=Disable";

// ✅ Build and register Npgsql data source
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<UserStatus>("user_status"); // <-- required
var dataSource = dataSourceBuilder.Build();

// IMPORTANT: only this AddDbContext!
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(dataSource));

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