using CommunityToolkit.Mvvm.Messaging;
using UIKit;

namespace Application;

public class BackgroundService
{
    nint _taskId;
    CancellationTokenSource _cts;
    public bool isStarted = false;

    public async Task Start()
    {
        _cts = new CancellationTokenSource();
        _taskId = UIApplication.SharedApplication.BeginBackgroundTask("com.company.product.name", OnExpiration);

        try
        {
            var locShared = new Location();
            isStarted = true;
            await locShared.Run(_cts.Token);
        }
        
        catch (OperationCanceledException)
        {
        }
        
        finally
        {
            if (_cts.IsCancellationRequested)
            {
                WeakReferenceMessenger.Default.Send(new ServiceMessage(ActionsEnum.STOP));
            }
        }

        var time = UIApplication.SharedApplication.BackgroundTimeRemaining;
        UIApplication.SharedApplication.EndBackgroundTask(_taskId);
    }

    public void Stop()
    {
        isStarted = false;
        _cts.Cancel();
    }

    void OnExpiration()
    {
        UIApplication.SharedApplication.EndBackgroundTask(_taskId);
    }
}

public class Location
{
    public async Task Run(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // Simulate background work
            await Task.Delay(1000, token);
        }
    }
}

public class ServiceMessage
{
    public ActionsEnum Action { get; }

    public ServiceMessage(ActionsEnum action)
    {
        Action = action;
    }
}

public enum ActionsEnum
{
    START,
    STOP
}