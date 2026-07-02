// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using SimpleOpcFileServer;
using SharedModels;

namespace Tests.Server;

public class LoggingCacheThroughputTests
{
    // ────────────────────────────────────────────────────────────────
    // Bounded capacity
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public void DefaultMaxSize_Is10000()
    {
        using var cache = new LoggingCache();
        Assert.Equal(10_000, cache.MaxSize);
    }

    [Fact]
    public void CustomMaxSize_IsRespected()
    {
        using var cache = new LoggingCache(maxSize: 50);
        Assert.Equal(50, cache.MaxSize);
    }

    [Fact]
    public void TryEnqueue_ReturnsTrueWhenSpaceAvailable()
    {
        using var cache = new LoggingCache(maxSize: 100);
        bool accepted = cache.TryEnqueue(() => { });
        Assert.True(accepted);
    }

    // ────────────────────────────────────────────────────────────────
    // Overflow / drop behavior
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public void TryEnqueue_DropsItemsWhenFull()
    {
        // Use a tiny cache and a blocker so items can't be consumed
        using var blocker = new ManualResetEventSlim(false);
        using var cache = new LoggingCache(maxSize: 5);

        // Fill the cache: first item blocks the consumer, remaining fill the buffer
        cache.TryEnqueue(() => blocker.Wait());
        // Give the consumer a moment to pick up the blocking item
        Thread.Sleep(50);

        // Fill remaining 5 slots
        for (int i = 0; i < 5; i++)
            cache.TryEnqueue(() => { });

        // Next enqueue should be dropped
        bool accepted = cache.TryEnqueue(() => { });
        Assert.False(accepted);
        Assert.True(cache.TotalDropped >= 1);

        blocker.Set(); // unblock consumer
    }

    [Fact]
    public void HasOverflowed_IsSetWhenItemDropped()
    {
        using var blocker = new ManualResetEventSlim(false);
        using var cache = new LoggingCache(maxSize: 2);

        cache.TryEnqueue(() => blocker.Wait());
        Thread.Sleep(50);

        // Fill remaining slots
        for (int i = 0; i < 2; i++)
            cache.TryEnqueue(() => { });

        Assert.False(cache.HasOverflowed); // not yet (maybe)
        cache.TryEnqueue(() => { }); // this should drop

        Assert.True(cache.HasOverflowed);

        blocker.Set();
    }

    [Fact]
    public void OnOverflow_FiresOnFirstDrop()
    {
        using var blocker = new ManualResetEventSlim(false);
        using var cache = new LoggingCache(maxSize: 2);
        int overflowCount = 0;
        cache.OnOverflow += () => Interlocked.Increment(ref overflowCount);

        cache.TryEnqueue(() => blocker.Wait());
        Thread.Sleep(50);
        for (int i = 0; i < 2; i++)
            cache.TryEnqueue(() => { });

        // Drop several items
        cache.TryEnqueue(() => { });
        cache.TryEnqueue(() => { });
        cache.TryEnqueue(() => { });

        // Event should fire only once
        Assert.Equal(1, overflowCount);

        blocker.Set();
    }

    // ────────────────────────────────────────────────────────────────
    // Stats tracking
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task TotalEnqueued_MatchesAcceptedItems()
    {
        using var cache = new LoggingCache(maxSize: 1000);
        int accepted = 0;
        for (int i = 0; i < 100; i++)
        {
            if (cache.TryEnqueue(() => { }))
                accepted++;
        }

        Assert.Equal(accepted, (int)cache.TotalEnqueued);

        // Wait for processing
        await Task.Delay(200);
    }

    [Fact]
    public async Task TotalProcessed_IncrementsAsItemsAreConsumed()
    {
        using var cache = new LoggingCache(maxSize: 1000);
        for (int i = 0; i < 50; i++)
            cache.TryEnqueue(() => { });

        // Give consumer time to process
        await Task.Delay(500);

        Assert.Equal(50, (int)cache.TotalProcessed);
    }

    [Fact]
    public async Task PeakCount_TracksHighWaterMark()
    {
        using var blocker = new ManualResetEventSlim(false);
        using var cache = new LoggingCache(maxSize: 100);

        // Block the consumer so items accumulate
        cache.TryEnqueue(() => blocker.Wait());
        Thread.Sleep(50);

        // Enqueue 20 items (consumer is blocked, so they accumulate)
        for (int i = 0; i < 20; i++)
            cache.TryEnqueue(() => { });

        int peakBefore = cache.PeakCount;
        Assert.True(peakBefore >= 20, $"Peak should be at least 20 but was {peakBefore}");

        // Release consumer
        blocker.Set();
        await Task.Delay(500);

        // After drain, peak should still reflect the high-water mark
        Assert.True(cache.PeakCount >= peakBefore, "Peak should not decrease");
    }

    [Fact]
    public void GetStats_ReturnsConsistentSnapshot()
    {
        using var cache = new LoggingCache(maxSize: 500);
        for (int i = 0; i < 10; i++)
            cache.TryEnqueue(() => { });

        var stats = cache.GetStats();

        Assert.NotNull(stats);
        Assert.Equal(500, stats.MaxSize);
        Assert.Equal(10, (int)stats.TotalEnqueued);
        Assert.False(stats.HasOverflowed);
    }

    // ────────────────────────────────────────────────────────────────
    // Throughput under load
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Throughput_CanHandleBurstWithoutUnboundedGrowth()
    {
        // Verify that with a bounded cache, even a large burst doesn't cause unbounded growth
        using var cache = new LoggingCache(maxSize: 1000);
        int counter = 0;

        // Burst 5000 items rapidly
        for (int i = 0; i < 5000; i++)
        {
            cache.TryEnqueue(() => Interlocked.Increment(ref counter));
        }

        // CurrentCount should never exceed MaxSize
        Assert.True(cache.CurrentCount <= cache.MaxSize,
            $"CurrentCount {cache.CurrentCount} should not exceed MaxSize {cache.MaxSize}");

        // Total enqueued + dropped should equal total attempted
        Assert.Equal(5000, (int)(cache.TotalEnqueued + cache.TotalDropped));

        // Wait for consumer to finish
        await Task.Delay(1000);

        // All enqueued items should be processed
        Assert.Equal(cache.TotalEnqueued, cache.TotalProcessed);
        Assert.Equal((int)cache.TotalProcessed, counter);
    }

    [Fact]
    public async Task Throughput_MultipleProducers()
    {
        using var cache = new LoggingCache(maxSize: 5000);
        int counter = 0;
        const int producerCount = 4;
        const int itemsPerProducer = 500;

        var tasks = new Task[producerCount];
        for (int p = 0; p < producerCount; p++)
        {
            tasks[p] = Task.Run(() =>
            {
                for (int i = 0; i < itemsPerProducer; i++)
                    cache.TryEnqueue(() => Interlocked.Increment(ref counter));
            });
        }

        await Task.WhenAll(tasks);
        await Task.Delay(1000);

        // All items should have been accepted (cache is large enough)
        Assert.Equal(producerCount * itemsPerProducer, (int)cache.TotalEnqueued);
        Assert.Equal(0, (int)cache.TotalDropped);
        Assert.Equal(producerCount * itemsPerProducer, counter);
    }

    // ────────────────────────────────────────────────────────────────
    // Disposal
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public void Dispose_StopsAcceptingItems()
    {
        var cache = new LoggingCache(maxSize: 100);
        cache.TryEnqueue(() => { });
        cache.Dispose();

        // After dispose, channel is completed — TryWrite should return false
        bool accepted = cache.TryEnqueue(() => { });
        Assert.False(accepted);
    }

    // ────────────────────────────────────────────────────────────────
    // Error handling in work items
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task ProcessLoop_ContinuesAfterWorkItemException()
    {
        using var cache = new LoggingCache(maxSize: 100);
        int successCount = 0;

        cache.TryEnqueue(() => throw new InvalidOperationException("test error"));
        cache.TryEnqueue(() => Interlocked.Increment(ref successCount));
        cache.TryEnqueue(() => Interlocked.Increment(ref successCount));

        await Task.Delay(500);

        // The two non-throwing items should still have been processed
        Assert.Equal(2, successCount);
        // All three were processed (including the throwing one)
        Assert.Equal(3, (int)cache.TotalProcessed);
    }
}
