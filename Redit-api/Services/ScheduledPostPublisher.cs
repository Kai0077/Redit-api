using Npgsql;
using Redit_api.Data;

namespace Redit_api.Services
{
    public class ScheduledPostPublisher : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ScheduledPostPublisher> _logger;
        private readonly string _connectionString;

        public ScheduledPostPublisher(IConfiguration config, ILogger<ScheduledPostPublisher> logger)
        {
            _config = config;
            _logger = logger;
            _connectionString = _config.GetConnectionString("DefaultConnection")!;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduledPostPublisher service started, checking every minute for posts to publish.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var connection = await DbConnectionFactory.CreateOpenConnectionAsync(_connectionString);

                    var cmd = new NpgsqlCommand("SELECT publish_scheduled_posts();", connection);
                    await cmd.ExecuteNonQueryAsync(stoppingToken);
                    
                    _logger.LogInformation("Checked for schedule posts at {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while publishing schedule posts");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            
            _logger.LogInformation("SchedulePostPublisher stopped");
        }
    }
}