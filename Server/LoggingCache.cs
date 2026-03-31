using System.Threading.Channels;
using SharedModels;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Bounded work-item cache for data-logging operations.
    /// Tracks throughput statistics (enqueued, processed, dropped, peak depth)
    /// and raises an event when the cache reaches capacity.
    /// Default maximum size: 10,000 entries.
    /// </summary>
    public sealed class LoggingCache : IDisposable
    {
        public const int DefaultMaxSize = 10_000;

        private readonly Channel<Action> _channel;
        private readonly Task _processingTask;
        private readonly CancellationTokenSource _cts = new();

        private long _totalEnqueued;
        private long _totalProcessed;
        private long _totalDropped;
        private int _peakCount;
        private volatile bool _overflowed;

        public int MaxSize { get; }
        public int CurrentCount => _channel.Reader.CanCount ? _channel.Reader.Count : 0;
        public int PeakCount => Volatile.Read(ref _peakCount);
        public long TotalEnqueued => Interlocked.Read(ref _totalEnqueued);
        public long TotalProcessed => Interlocked.Read(ref _totalProcessed);
        public long TotalDropped => Interlocked.Read(ref _totalDropped);
        public bool HasOverflowed => _overflowed;

        /// <summary>Raised the first time the cache reaches capacity and a new item is dropped.</summary>
        public event Action? OnOverflow;

        public LoggingCache(int maxSize = DefaultMaxSize)
        {
            MaxSize = maxSize;
            _channel = Channel.CreateBounded<Action>(new BoundedChannelOptions(maxSize)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });
            _processingTask = Task.Run(ProcessLoop);
        }

        /// <summary>
        /// Enqueue a logging work item. Returns true if accepted, false if the cache is full
        /// (the item is dropped and <see cref="TotalDropped"/> is incremented).
        /// </summary>
        public bool TryEnqueue(Action workItem)
        {
            if (_channel.Writer.TryWrite(workItem))
            {
                Interlocked.Increment(ref _totalEnqueued);
                UpdatePeak();
                return true;
            }

            Interlocked.Increment(ref _totalDropped);
            if (!_overflowed)
            {
                _overflowed = true;
                OnOverflow?.Invoke();
            }
            return false;
        }

        /// <summary>Snapshot of current cache statistics.</summary>
        public LoggerCacheStats GetStats() => new()
        {
            CurrentCount = CurrentCount,
            PeakCount = PeakCount,
            MaxSize = MaxSize,
            TotalEnqueued = TotalEnqueued,
            TotalProcessed = TotalProcessed,
            TotalDropped = TotalDropped,
            HasOverflowed = HasOverflowed
        };

        private void UpdatePeak()
        {
            int count = CurrentCount;
            int peak;
            do
            {
                peak = Volatile.Read(ref _peakCount);
            }
            while (count > peak && Interlocked.CompareExchange(ref _peakCount, count, peak) != peak);
        }

        private async Task ProcessLoop()
        {
            try
            {
                await foreach (var workItem in _channel.Reader.ReadAllAsync(_cts.Token))
                {
                    try
                    {
                        workItem();
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Error(ex, "LoggingCache: processing error: {Message}", ex.Message);
                    }
                    finally
                    {
                        Interlocked.Increment(ref _totalProcessed);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            _channel.Writer.TryComplete();
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
