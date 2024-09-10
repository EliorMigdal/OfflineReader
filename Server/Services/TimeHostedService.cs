using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Server.Services;

public class TimedHostedService(ILogger<TimedHostedService> logger) : IHostedService, IDisposable
{
    private readonly DBService r_DBService = DBService.Instance;
    private Timer? m_Timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Timed Hosted Service running.");
        var nextRunTime = DateTime.Today.AddHours(20);
        
        if (DateTime.Now > nextRunTime)
        {
            nextRunTime = nextRunTime.AddDays(1);
        }
        
        var initialDelay = nextRunTime - DateTime.Now;
        m_Timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromHours(24));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        logger.LogInformation("Timed Hosted Service is working at {time}.", DateTimeOffset.Now);
        await r_DBService.UpdateArticlesHistory();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Timed Hosted Service is stopping.");
        m_Timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        m_Timer?.Dispose();
    }
}