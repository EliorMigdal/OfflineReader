using Application.BackgroundTask;
using Application.Helpers;
using Application.Service.Hybrid;
using Application.Service.Local;
using BackgroundTasks;
using BusinessLogic.AutoDownloadSettings;
using BusinessLogic.SupportedWebsite;
using Foundation;
using UserNotifications;

namespace Application;

public class PeriodicBackgroundService : IBackgroundTaskService
{
    void RegisterBackgroundTask()
    {
        BGTaskScheduler.Shared.Register("com.companyname.offlinereader.updateTask", null, task =>
        {
            PerformBackgroundWork(task);
        });
    }
    
    void PerformBackgroundWork(BGTask i_Task)
    {
        var backgroundService = new BackgroundService();

        SendLocalNotification("Background Task Running", "The app's background task has been triggered.");

        backgroundService.Start().ContinueWith(async t =>
        {
            AutoDownloadSettings settings = ConfigService.GetSettings();

            if (settings.AutoDownloadEnabled  
                && ((settings.DownloadOnWifiOnly && ConnectivityManager.Instance.IsDeviceConnectedToWiFi()) 
                                                  || (settings.AllowCellularRoaming && ConnectivityManager.Instance.IsDeviceConnected())))
            {
                List<SupportedWebsite> selectedWebsites = ConfigService.GetSelectedWebsites();
                await AutoDownloadService.Instance.AutoDownloadArticles(selectedWebsites, settings.MaxArticlesToStoreLocally);
            }
            
            i_Task.SetTaskCompleted(success: t.IsCompletedSuccessfully);
        });

        i_Task.ExpirationHandler = () =>
        {
            backgroundService.Stop();
        };
    }
    
    public void ScheduleTask()
    {
        var request = new BGProcessingTaskRequest("com.companyname.offlinereader.updateTask")
        {
            RequiresNetworkConnectivity = true,
            RequiresExternalPower = false
        };

        request.EarliestBeginDate = NSDate.Now.AddSeconds(86400); // 24 hours

        NSError error;
        BGTaskScheduler.Shared.Submit(request, out error);
    }
    
    void SendLocalNotification(string i_Title, string i_Message)
    {
        var content = new UNMutableNotificationContent
        {
            Title = i_Title,
            Body = i_Message
        };

        var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false); // Trigger immediately
        var requestID = Guid.NewGuid().ToString();
        var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

        UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
        {
            if (err != null)
            {
                Console.WriteLine($"Error scheduling notification: {err.LocalizedDescription}");
            }
        });
    }

    public void CancelTask()
    {
        throw new NotImplementedException();
    }
}