using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp2.Services
{
    /// <summary>
    /// Service for managing background tasks
    /// </summary>
    public class BackgroundService
    {
        private static BackgroundService? _instance;
        public static BackgroundService Instance => _instance ??= new BackgroundService();

        private readonly Dictionary<string, CancellationTokenSource> _runningTasks = new();
        private readonly object _lockObject = new();

        public ObservableCollection<TaskProgress> Tasks { get; } = new();

        private BackgroundService() { }

        /// <summary>
        /// Enqueue a background task to be executed
        /// </summary>
        public async Task EnqueueTaskAsync(IBackgroundTask task, string taskName)
        {
            var progress = new Progress<TaskProgress>(UpdateTaskProgress);
            var cts = new CancellationTokenSource();
            var taskProgress = new TaskProgress
            {
                TaskName = taskName,
                StartTime = DateTime.Now,
                CurrentStatus = TaskStatus.Pending,
                Status = "Pending"
            };

            lock (_lockObject)
            {
                Tasks.Add(taskProgress);
                _runningTasks[taskProgress.TaskId] = cts;
            }

            try
            {
                await task.ExecuteAsync(progress, cts.Token);
            }
            finally
            {
                lock (_lockObject)
                {
                    _runningTasks.Remove(taskProgress.TaskId);
                }
            }
        }

        /// <summary>
        /// Cancel a specific task
        /// </summary>
        public void CancelTask(string taskId)
        {
            lock (_lockObject)
            {
                if (_runningTasks.TryGetValue(taskId, out var cts))
                {
                    cts.Cancel();
                }
            }
        }

        /// <summary>
        /// Cancel all running tasks
        /// </summary>
        public void CancelAllTasks()
        {
            lock (_lockObject)
            {
                foreach (var cts in _runningTasks.Values)
                {
                    cts.Cancel();
                }
            }
        }

        /// <summary>
        /// Clear completed tasks from the list
        /// </summary>
        public void ClearCompletedTasks()
        {
            var completedTasks = Tasks.Where(t => 
                t.CurrentStatus == TaskStatus.Completed || 
                t.CurrentStatus == TaskStatus.Failed || 
                t.CurrentStatus == TaskStatus.Cancelled).ToList();

            foreach (var task in completedTasks)
            {
                Tasks.Remove(task);
            }
        }

        private void UpdateTaskProgress(TaskProgress progress)
        {
            var existingTask = Tasks.FirstOrDefault(t => t.TaskId == progress.TaskId);
            if (existingTask != null)
            {
                existingTask.PercentComplete = progress.PercentComplete;
                existingTask.Status = progress.Status;
                existingTask.CurrentStatus = progress.CurrentStatus;
                existingTask.ErrorMessage = progress.ErrorMessage;
                existingTask.EndTime = progress.EndTime;
            }
        }

        public int GetRunningTaskCount()
        {
            lock (_lockObject)
            {
                return _runningTasks.Count;
            }
        }
    }
}
