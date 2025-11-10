using Core.Interfaces;

namespace API.Services;

public class SeatHoldCleanupService(
    IServiceProvider serviceProvider,
    ILogger<SeatHoldCleanupService> logger) : BackgroundService
{
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1); // Run every minute

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Seat Hold Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredHolds();
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Service is being stopped
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during seat hold cleanup");
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }

        logger.LogInformation("Seat Hold Cleanup Service stopped");
    }

    private async Task CleanupExpiredHolds()
    {
        using var scope = serviceProvider.CreateScope();
        var seatHoldRepository = scope.ServiceProvider.GetRequiredService<ISeatHoldRepository>();

        try
        {
            var cleanedUpCount = await seatHoldRepository.CleanupExpiredHoldsAsync();
            
            if (cleanedUpCount > 0)
            {
                logger.LogInformation("Cleaned up {Count} expired seat holds", cleanedUpCount);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to cleanup expired seat holds");
        }
    }
}