// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using CloudBridge;
using SharedModels;
using SharedModels.CloudRelay;
using Xunit;

namespace Tests.Server;

public class StoreAndForwardQueueTests : IDisposable
{
    private readonly string _tempDir;
    private readonly StoreAndForwardConfig _config;

    public StoreAndForwardQueueTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "saf_test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        _config = new StoreAndForwardConfig
        {
            Enabled = true,
            SpoolPath = "test_spool.db",
            MaxRows = 1000,
            BatchSize = 50,
            DrainIntervalSeconds = 1
        };
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    private StoreAndForwardQueue CreateQueue() => new(_config, _tempDir);

    [Fact]
    public void Enqueue_SingleBatch_StoresRows()
    {
        using var queue = CreateQueue();
        var values = MakeValues(5);

        queue.Enqueue(values);

        Assert.Equal(5, queue.GetPendingCount());
    }

    [Fact]
    public void Dequeue_ReturnsEnqueuedRows_InOrder()
    {
        using var queue = CreateQueue();
        queue.Enqueue(MakeValues(3, "batch1_"));

        var (values, ids) = queue.Dequeue(10);

        Assert.Equal(3, values.Count);
        Assert.Equal(3, ids.Count);
        Assert.Equal("batch1_var0", values[0].Path);
        Assert.Equal("batch1_var1", values[1].Path);
        Assert.Equal("batch1_var2", values[2].Path);
    }

    [Fact]
    public void Acknowledge_RemovesRows()
    {
        using var queue = CreateQueue();
        queue.Enqueue(MakeValues(5));

        var (_, ids) = queue.Dequeue(5);
        queue.Acknowledge(ids);

        Assert.Equal(0, queue.GetPendingCount());
    }

    [Fact]
    public void Dequeue_WithoutAcknowledge_RowsRemain()
    {
        using var queue = CreateQueue();
        queue.Enqueue(MakeValues(5));

        var (values1, _) = queue.Dequeue(5);

        // Without acknowledge, dequeue again returns the same rows
        var (values2, _) = queue.Dequeue(5);
        Assert.Equal(5, values2.Count);
        Assert.Equal(values1[0].Path, values2[0].Path);
    }

    [Fact]
    public void Dequeue_BatchSize_LimitsResults()
    {
        using var queue = CreateQueue();
        queue.Enqueue(MakeValues(20));

        var (values, ids) = queue.Dequeue(5);

        Assert.Equal(5, values.Count);
        Assert.Equal(5, ids.Count);
        Assert.Equal(20, queue.GetPendingCount());
    }

    [Fact]
    public void PurgeExcess_RemovesOldestRows()
    {
        _config.MaxRows = 10;
        using var queue = CreateQueue();

        queue.Enqueue(MakeValues(15));

        // After enqueue, purge should have trimmed to 10
        Assert.Equal(10, queue.GetPendingCount());

        // The surviving rows should be the newest ones (var5..var14)
        var (values, _) = queue.Dequeue(10);
        Assert.Equal("var5", values[0].Path);
    }

    [Fact]
    public void MultipleBatches_DrainInOrder()
    {
        using var queue = CreateQueue();
        queue.Enqueue(MakeValues(3, "a_"));
        queue.Enqueue(MakeValues(3, "b_"));

        Assert.Equal(6, queue.GetPendingCount());

        var (batch1, ids1) = queue.Dequeue(3);
        queue.Acknowledge(ids1);

        var (batch2, ids2) = queue.Dequeue(3);
        queue.Acknowledge(ids2);

        Assert.Equal("a_var0", batch1[0].Path);
        Assert.Equal("b_var0", batch2[0].Path);
        Assert.Equal(0, queue.GetPendingCount());
    }

    [Fact]
    public void EmptyQueue_Dequeue_ReturnsEmpty()
    {
        using var queue = CreateQueue();

        var (values, ids) = queue.Dequeue(10);

        Assert.Empty(values);
        Assert.Empty(ids);
    }

    [Fact]
    public void EmptyQueue_PendingCount_IsZero()
    {
        using var queue = CreateQueue();
        Assert.Equal(0, queue.GetPendingCount());
    }

    [Fact]
    public void Persistence_SurvivesReopen()
    {
        // Enqueue, close, reopen — data should still be there
        using (var queue = CreateQueue())
        {
            queue.Enqueue(MakeValues(5, "persist_"));
        }

        using (var queue2 = CreateQueue())
        {
            Assert.Equal(5, queue2.GetPendingCount());
            var (values, _) = queue2.Dequeue(5);
            Assert.Equal("persist_var0", values[0].Path);
        }
    }

    private static List<TagValueDto> MakeValues(int count, string prefix = "")
    {
        var list = new List<TagValueDto>();
        for (int i = 0; i < count; i++)
            list.Add(new TagValueDto { Path = $"{prefix}var{i}", Value = $"{i}.0" });
        return list;
    }
}
