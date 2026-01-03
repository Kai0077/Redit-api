using System.Text;
using System.Text.Json.Serialization;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Neo4j.Driver;
using Redit_api.Data;
using Redit_api.FirestoreSync;
using Redit_api.GraphSync;
using Redit_api.Models;
using Redit_api.Models.Status;
using Redit_api.Repositories.Firestore;
using Redit_api.Repositories.Firestore.Interfaces;
using Redit_api.Repositories.Neo4j;
using Redit_api.Repositories.Neo4j.Interfaces;
using Redit_api.Repositories.Postgresql;
using Redit_api.Repositories.Postgresql.Interfaces;
using Redit_api.Services;
using Redit_api.Services.Interfaces;

var isMigrationMode = args.Length > 0 && args[0] == "migrate";

var builder = WebApplication.CreateBuilder(args);

// ======================= LOGGING SENTRY.IO =======================
var sentryDsn = Environment.GetEnvironmentVariable("SENTRY_DSN");
builder.WebHost.UseSentry(option =>
{
    option.Dsn = sentryDsn;
    option.TracesSampleRate = 1.0;
    option.Environment = builder.Environment.EnvironmentName;
    option.Debug = builder.Environment.IsDevelopment();
});

// ======================= POSTGRESQL ENV VARIABLES =======================
var cloudHost = Environment.GetEnvironmentVariable("CLOUD_DB_HOST");
var cloudEnabled = !string.IsNullOrWhiteSpace(cloudHost);

var host = cloudEnabled
    ? cloudHost
    : Environment.GetEnvironmentVariable("DB_HOST");

var port = cloudEnabled
    ? Environment.GetEnvironmentVariable("CLOUD_DB_PORT")
    : Environment.GetEnvironmentVariable("DB_PORT");

var database = cloudEnabled
    ? Environment.GetEnvironmentVariable("CLOUD_DB_NAME")
    : Environment.GetEnvironmentVariable("DB_NAME");

var user = cloudEnabled
    ? Environment.GetEnvironmentVariable("CLOUD_DB_USER")
    : Environment.GetEnvironmentVariable("DB_USER");

var password = cloudEnabled
    ? Environment.GetEnvironmentVariable("CLOUD_DB_PASSWORD")
    : Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString =
    $"Host={host};Port={port};Database={database};Username={user};Password={password};Ssl Mode=Disable";

builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

// ===================== FIRESTORE MIGRATION =====================
var firestoreKeyPath = Environment.GetEnvironmentVariable("FIRESTORE_KEY_PATH");
var firestoreProjectId = Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID");

if (string.IsNullOrEmpty(firestoreKeyPath) || !File.Exists(firestoreKeyPath))
{
    Console.WriteLine($"Firestore key file not found at: {Path.GetFullPath(firestoreKeyPath ?? "(null)")}");
}
else
{
    Console.WriteLine($"Using Firestore key: {Path.GetFullPath(firestoreKeyPath)}");
}

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firestoreKeyPath);

var firestoreDb = FirestoreDb.Create(firestoreProjectId);
Console.WriteLine($"Connected to Firestore project: {firestoreProjectId}");

builder.Services.AddSingleton(firestoreDb);

var migrator = new SqlToFirestoreMigrator(firestoreDb, connectionString);

if (isMigrationMode)
{
    Console.WriteLine("Running Firestore migration...");
    await migrator.RunMigrationAsync();
    Console.WriteLine("Firestore migration completed.");
}

// ===================== NEO4J MIGRATION + DI =====================
var neo4JUri = Environment.GetEnvironmentVariable("NEO4J_URI");
var neo4JUser = Environment.GetEnvironmentVariable("NEO4J_USER");
var neo4JPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");

if (string.IsNullOrWhiteSpace(neo4JUri) ||
    string.IsNullOrWhiteSpace(neo4JUser) ||
    string.IsNullOrWhiteSpace(neo4JPassword))
{
    Console.WriteLine("Neo4j env vars missing (NEO4J_URI, NEO4J_USER, NEO4J_PASSWORD).");
}
else
{
    Console.WriteLine($"Connecting to Neo4j at {neo4JUri}");
}

// Create ONE driver instance
var neo4JDriver = GraphDatabase.Driver(neo4JUri, AuthTokens.Basic(neo4JUser, neo4JPassword));

// Register the SAME instance for DI
builder.Services.AddSingleton<IDriver>(neo4JDriver);
builder.Services.AddScoped<INeo4jUserReadRepository, Neo4jUserReadRepository>();

// Use the SAME driver for migration
var neo4JMigrator = new SqlToNeo4JMigrator(neo4JDriver, connectionString);

if (isMigrationMode)
{
    Console.WriteLine("Running Neo4j migration...");
    await neo4JMigrator.RunMigrationAsync();
    Console.WriteLine("Neo4j migration completed.");

    Console.WriteLine("Migration finished. Exiting.");
    return;
}

// ======================= JWT Authentication =======================
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
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)
            ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// ======================= Database + Enum Mapping =======================
builder.Services.AddDbContext<AppDBContext>(opts =>
    opts.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MapEnum<UserStatus>("user_status");
        npgsql.MapEnum<UserRole>("user_role");
        npgsql.MapEnum<PostStatus>("post_status");
    })
);

// ======================= MVC, JSON, Swagger, DI =======================
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================= SERVICE, REPOSITORY & INFRASTRUCTURE REGISTRATION =======================
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPostgresPostRepository, PostgresPostRepository>();
builder.Services.AddScoped<IFirestorePostRepository, FirestorePostRepository>();

builder.Services.AddScoped<IPostgresUserRepository, PostgresUserRepository>();
builder.Services.AddScoped<IFirestoreUserRepository, FirestoreUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<UserDTO>, PasswordHasher<UserDTO>>();

builder.Services.AddScoped<IPostgresCommunityRepository, PostgresCommunityRepository>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<IFirestoreCommunityRepository, FirestoreCommunityRepository>();

builder.Services.AddScoped<IPostgresCommentRepository, PostgresCommentRepository>();
builder.Services.AddScoped<IFirestoreCommentRepository, FirestoreCommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddHostedService<ScheduledPostPublisher>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ISentryLogger, SentryLogger>();

// ======================= CORS CONFIG =======================
var clientLocal = Environment.GetEnvironmentVariable("CLIENT_URL_LOCAL");
var clientDeploy = Environment.GetEnvironmentVariable("CLIENT_URL_DEPLOY");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(clientLocal!, clientDeploy!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ======================= Middleware Pipeline =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();