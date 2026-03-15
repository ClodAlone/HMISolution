using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WpfApp1.Services
{
    /// <summary>
    /// Simple background task processor (producer-consumer).
    /// Enqueue work as a Func<CancellationToken, Task> and await the returned Task to observe completion.
    /// </summary>
    public sealed class BackgroundTaskProcessor : IDisposable
    {
        private sealed class TaskItem
        {
            public Func<CancellationToken, IProgress<double>?, Task> Work { get; set; } = null!;
            public CancellationToken Token { get; set; }
            public IProgress<double>? Progress { get; set; }
            public TaskCompletionSource<object?> Tcs { get; set; } = null!;
        }

        private readonly Channel<TaskItem> _channel;
        private CancellationTokenSource _internalCts = new();
        private Task[]? _workers;
        private readonly int _maxDegreeOfParallelism;

        public BackgroundTaskProcessor(int maxDegreeOfParallelism = 1)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism));
            _maxDegreeOfParallelism = maxDegreeOfParallelism;

            // unbounded channel; tasks are processed in order
            _channel = Channel.CreateUnbounded<TaskItem>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            });
        }

        /// <summary>
        /// Start processing queued tasks.
        /// Safe to call multiple times.
        /// </summary>
        public void Start()
        {
            if (_workers != null) return;
            _internalCts = new CancellationTokenSource();
            _workers = new Task[_maxDegreeOfParallelism];
            for (int i = 0; i < _maxDegreeOfParallelism; i++)
            {
                _workers[i] = Task.Run(() => WorkerLoopAsync(_internalCts.Token));
            }
        }

        /// <summary>
        /// Enqueue work. Returns a Task that completes when the work has finished or faulted.
        /// </summary>
        public Task EnqueueAsync(Func<CancellationToken, IProgress<double>?, Task> work, CancellationToken cancellationToken = default, IProgress<double>? progress = null)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));
            var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

            var item = new TaskItem
            {
                Work = work,
                Token = cancellationToken,
                Progress = progress,
                Tcs = tcs
            };

            var written = _channel.Writer.TryWrite(item);
            if (!written)
            {
                _channel.Writer.WriteAsync(item, CancellationToken.None).AsTask().ContinueWith(t =>
                {
                    if (t.IsFaulted && t.Exception != null)
                        tcs.TrySetException(t.Exception);
                }, TaskScheduler.Default);
            }

            Start(); // ensure workers running
            return tcs.Task;
        }

        /// <summary>
        /// Stop processing and wait for the current worker to finish.
        /// </summary>
        public async Task StopAsync()
        {
            try
            {
                _channel.Writer.Complete();
            }
            catch { }

            _internalCts.Cancel();
            if (_workers != null)
            {
                try
                {
                    await Task.WhenAll(_workers).ConfigureAwait(false);
                }
                catch { }
                _workers = null;
            }
        }
        private async Task WorkerLoopAsync(CancellationToken globalToken)
        {
            var reader = _channel.Reader;
            try
            {
                while (await reader.WaitToReadAsync(globalToken).ConfigureAwait(false))
                {
                    while (reader.TryRead(out var item))
                    {
                        try
                        {
                            // create linked token so caller can cancel the task independently
                            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(globalToken, item.Token);
                            await item.Work(linkedCts.Token, item.Progress).ConfigureAwait(false);
                            item.Tcs.TrySetResult(null);
                        }
                        catch (OperationCanceledException) when (globalToken.IsCancellationRequested || item.Token.IsCancellationRequested)
                        {
                            item.Tcs.TrySetCanceled();
                        }
                        catch (Exception ex)
                        {
                            item.Tcs.TrySetException(ex);
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch { }
        }

        public void Dispose()
        {
            try
            {
                StopAsync().GetAwaiter().GetResult();
            }
            catch { }
            try
            {
                _internalCts?.Dispose();
            }
            catch { }
        }
    }
}
