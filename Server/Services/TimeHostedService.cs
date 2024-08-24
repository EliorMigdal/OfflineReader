namespace Server.Services;

public class TimedHostedService : IHostedService, IDisposable
{
    private readonly DBService r_DBService = DBService.Instance;
    private readonly ILogger<TimedHostedService> _logger;
    private Timer _timer;

    public TimedHostedService(ILogger<TimedHostedService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        var nextRunTime = DateTime.Today.AddHours(20);
        if (DateTime.Now > nextRunTime)
        {
            nextRunTime = nextRunTime.AddDays(1);
        }
        var initialDelay = nextRunTime - DateTime.Now;

        _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromHours(24));

        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        _logger.LogInformation("Timed Hosted Service is working at {time}.", DateTimeOffset.Now);
        await r_DBService.UpdateArticlesHistory();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}