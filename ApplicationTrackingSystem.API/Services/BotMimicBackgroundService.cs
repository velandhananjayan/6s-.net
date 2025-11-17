using ApplicationTrackingSystem.API.Services;

namespace ApplicationTrackingSystem.API.Services;

/// <summary>
/// Background service that automatically processes technical applications at scheduled intervals
/// </summary>
public class BotMimicBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BotMimicBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Process every 5 minutes

    public BotMimicBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<BotMimicBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Bot Mimic Background Service started. Will process technical applications every {Interval} minutes.", _interval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a scope to get scoped services
                using var scope = _serviceProvider.CreateScope();
                var botMimicService = scope.ServiceProvider.GetRequiredService<IBotMimicService>();

                // Process technical applications
                var processedCount = await botMimicService.ProcessTechnicalApplicationsAsync();
                
                if (processedCount > 0)
                {
                    _logger.LogInformation("Bot Mimic processed {Count} technical applications automatically.", processedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing technical applications in background service.");
            }

            // Wait for the specified interval before next execution
            await Task.Delay(_interval, stoppingToken);
        }
    }
}

