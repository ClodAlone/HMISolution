using System;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp2.Services
{
    /// <summary>
    /// Sample background task that simulates work
    /// </summary>
    public class SampleBackgroundTask : BackgroundTaskBase
    {
        private readonly int _durationSeconds;

        public SampleBackgroundTask(int durationSeconds = 5)
        {
            _durationSeconds = durationSeconds;
            TaskName = $"Sample Task ({durationSeconds}s)";
        }

        protected override async Task ExecuteTaskAsync(IProgress<TaskProgress> progress, CancellationToken cancellationToken)
        {
            var taskProgress = new TaskProgress { TaskName = TaskName };
            int steps = 10;
            int delayMs = _durationSeconds * 1000 / steps;

            for (int i = 0; i < steps; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                taskProgress.PercentComplete = (i + 1) * 10;
                taskProgress.Status = $"Progress: {taskProgress.PercentComplete}%";
                taskProgress.CurrentStatus = TaskStatus.Running;
                ReportProgress(progress, taskProgress);

                await Task.Delay(delayMs, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Simulates a build task
    /// </summary>
    public class BuildTask : BackgroundTaskBase
    {
        public BuildTask()
        {
            TaskName = "Building Solution...";
        }

        protected override async Task ExecuteTaskAsync(IProgress<TaskProgress> progress, CancellationToken cancellationToken)
        {
            var taskProgress = new TaskProgress { TaskName = TaskName };

            var steps = new[] { "Restoring packages...", "Compiling...", "Linking...", "Creating package..." };

            for (int i = 0; i < steps.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                taskProgress.PercentComplete = (i + 1) * 25;
                taskProgress.Status = steps[i];
                taskProgress.CurrentStatus = TaskStatus.Running;
                ReportProgress(progress, taskProgress);

                await Task.Delay(2000, cancellationToken);
            }
        }
    }
}
