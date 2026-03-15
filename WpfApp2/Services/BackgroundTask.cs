using System;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp2.Services
{
    /// <summary>
    /// Represents the progress of a background task
    /// </summary>
    public class TaskProgress
    {
        public string TaskId { get; set; } = Guid.NewGuid().ToString();
        public string TaskName { get; set; } = string.Empty;
        public int PercentComplete { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TaskStatus CurrentStatus { get; set; } = TaskStatus.Pending;
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Task status enumeration
    /// </summary>
    public enum TaskStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Cancelled
    }

    /// <summary>
    /// Interface for background tasks
    /// </summary>
    public interface IBackgroundTask
    {
        Task ExecuteAsync(IProgress<TaskProgress> progress, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Abstract base class for background tasks
    /// </summary>
    public abstract class BackgroundTaskBase : IBackgroundTask
    {
        protected string TaskName { get; set; } = "Background Task";

        public virtual async Task ExecuteAsync(IProgress<TaskProgress> progress, CancellationToken cancellationToken)
        {
            var taskProgress = new TaskProgress
            {
                TaskName = TaskName,
                StartTime = DateTime.Now,
                CurrentStatus = TaskStatus.Running,
                Status = "Running"
            };

            try
            {
                progress.Report(taskProgress);
                await ExecuteTaskAsync(progress, cancellationToken);

                taskProgress.CurrentStatus = TaskStatus.Completed;
                taskProgress.Status = "Completed";
                taskProgress.PercentComplete = 100;
                taskProgress.EndTime = DateTime.Now;
                progress.Report(taskProgress);
            }
            catch (OperationCanceledException)
            {
                taskProgress.CurrentStatus = TaskStatus.Cancelled;
                taskProgress.Status = "Cancelled";
                taskProgress.EndTime = DateTime.Now;
                progress.Report(taskProgress);
            }
            catch (Exception ex)
            {
                taskProgress.CurrentStatus = TaskStatus.Failed;
                taskProgress.Status = "Failed";
                taskProgress.ErrorMessage = ex.Message;
                taskProgress.EndTime = DateTime.Now;
                progress.Report(taskProgress);
            }
        }

        protected abstract Task ExecuteTaskAsync(IProgress<TaskProgress> progress, CancellationToken cancellationToken);

        protected void ReportProgress(IProgress<TaskProgress> progress, TaskProgress taskProgress)
        {
            progress.Report(taskProgress);
        }
    }
}
