using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Redit_api.Data;

var builder = WebApplication.CreateBuilder(args);

// Load your env file (rename or pass the filename explicitly)
Env.Load("Secret.env");

// Build connection string from env
string host = Environment.GetEnvironmentVariable("DB_HOST")!;
string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
string database = Environment.GetEnvironmentVariable("DB_NAME")!;
string user = Environment.GetEnvironmentVariable("DB_USER")!;
string password = Environment.GetEnvironmentVariable("DB_PASSWORD")!;

string connectionString =
    $"Host={host};Port={port};Database={database};Username={user};Password={password};Ssl Mode=Disable";

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
