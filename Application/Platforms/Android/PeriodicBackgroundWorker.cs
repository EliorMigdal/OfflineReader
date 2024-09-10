// using Android.App;
// using Android.Content;
// using AndroidX.Work;
// using Application.BackgroundTask;
// using Application.Service.Hybrid;
// using Application.Service.Local;
// using BusinessLogic.AutoDownloadSettings;
// using BusinessLogic.SupportedWebsite;
// using Java.Util.Concurrent;
//
// namespace Application;
//
// public class PeriodicBackgroundWorker(Context context, WorkerParameters workerParams) : Worker(context, workerParams), IBackgroundTaskService
// {
//     public override Result DoWork()
//     {
//         var task = Task.Run(async () =>
//         {
//             await PerformBackgroundTaskAsync();
//         });
//
//         task.Wait();
//
//         return task.IsCompletedSuccessfully ? Result.InvokeSuccess() : Result.InvokeFailure();
//     }
//
//     async Task PerformBackgroundTaskAsync()
//     {
//         AutoDownloadSettings settings = ConfigService.GetSettings();
//
//         if (settings.AutoDownload)
//         {
//             List<SupportedWebsite> selectedWebsites = ConfigService.GetSelectedWebsites();
//             await AutoDownloadService.Instance.AutoDownloadArticles(selectedWebsites, settings.MaxArticlesToStoreLocally);
//         }
//     }
//     
//     public void ScheduleTask()
//     {
//         var workRequest = PeriodicWorkRequest.Builder
//             .From<PeriodicBackgroundWorker>(24, TimeUnit.Hours) // Set the interval for the task
//             .SetInitialDelay(2, TimeUnit.Hours) // Delay until the first run (2 AM in this example)
//             .Build();
//
//         WorkManager.Instance.Enqueue(workRequest);
//     }
//
//     public void CancelTask()
//     {
//         WorkManager.Instance.CancelAllWork();
//     }
// }

using Application.BackgroundTask;

public class PeriodicBackgroundWorker : IBackgroundTaskService
{
    public void ScheduleTask()
    {
        throw new NotImplementedException();
    }

    public void CancelTask()
    {
        throw new NotImplementedException();
    }
}