using Pwnlabs.Application.Labs;

namespace Pwnlabs.Api.BackgroundJobs;

public class LabExpirationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<LabExpirationWorker> _logger;
    public LabExpirationWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<LabExpirationWorker> logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var labService = scope.ServiceProvider.GetRequiredService<ILabService>();

                var count = await labService.ExpireLabsAsync();

                if (count > 0)
                {
                    _logger.LogInformation("Expired {Count} labs", count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lab expiration failed");
            }
        }
    }
}