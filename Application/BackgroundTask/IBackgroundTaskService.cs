namespace Application.BackgroundTask;

public interface IBackgroundTaskService
{
    void ScheduleTask();
    void CancelTask();
}