using Xunit;
using SimpleOpcFileServer;

namespace Tests.Server;

public class RateLimiterTests
{
    // ──────────────────────────────────────────────────────────────
    // Basic Allow / Deny
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsAllowed_UnderLimit_ReturnsTrue()
    {
        using var limiter = new RateLimiter(5, TimeSpan.FromSeconds(60));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.True(limiter.IsAllowed("client1"));
    }

    [Fact]
    public void IsAllowed_AtLimit_ReturnsFalse()
    {
        using var limiter = new RateLimiter(3, TimeSpan.FromSeconds(60));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.False(limiter.IsAllowed("client1"));
    }

    [Fact]
    public void IsAllowed_DifferentClients_IndependentLimits()
    {
        using var limiter = new RateLimiter(2, TimeSpan.FromSeconds(60));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.False(limiter.IsAllowed("client1")); // client1 exhausted

        // client2 should still be allowed
        Assert.True(limiter.IsAllowed("client2"));
        Assert.True(limiter.IsAllowed("client2"));
        Assert.False(limiter.IsAllowed("client2")); // client2 exhausted
    }

    // ──────────────────────────────────────────────────────────────
    // Unlimited (disabled)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsAllowed_ZeroMaxRequests_AlwaysAllowed()
    {
        using var limiter = new RateLimiter(0, TimeSpan.FromSeconds(60));
        for (int i = 0; i < 1000; i++)
        {
            Assert.True(limiter.IsAllowed("client1"));
        }
    }

    [Fact]
    public void IsAllowed_NegativeMaxRequests_AlwaysAllowed()
    {
        using var limiter = new RateLimiter(-1, TimeSpan.FromSeconds(60));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.True(limiter.IsAllowed("client1"));
    }

    [Fact]
    public void IsAllowed_EmptyClientKey_AlwaysAllowed()
    {
        using var limiter = new RateLimiter(1, TimeSpan.FromSeconds(60));
        Assert.True(limiter.IsAllowed(""));
        Assert.True(limiter.IsAllowed(""));
    }

    [Fact]
    public void IsAllowed_NullClientKey_AlwaysAllowed()
    {
        using var limiter = new RateLimiter(1, TimeSpan.FromSeconds(60));
        Assert.True(limiter.IsAllowed(null!));
        Assert.True(limiter.IsAllowed(null!));
    }

    // ──────────────────────────────────────────────────────────────
    // GetRemaining
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetRemaining_NewClient_ReturnsMax()
    {
        using var limiter = new RateLimiter(10, TimeSpan.FromSeconds(60));
        Assert.Equal(10, limiter.GetRemaining("client1"));
    }

    [Fact]
    public void GetRemaining_AfterRequests_Decreases()
    {
        using var limiter = new RateLimiter(5, TimeSpan.FromSeconds(60));
        limiter.IsAllowed("client1");
        limiter.IsAllowed("client1");
        Assert.Equal(3, limiter.GetRemaining("client1"));
    }

    [Fact]
    public void GetRemaining_Exhausted_ReturnsZero()
    {
        using var limiter = new RateLimiter(2, TimeSpan.FromSeconds(60));
        limiter.IsAllowed("client1");
        limiter.IsAllowed("client1");
        Assert.Equal(0, limiter.GetRemaining("client1"));
    }

    [Fact]
    public void GetRemaining_Unlimited_ReturnsMaxValue()
    {
        using var limiter = new RateLimiter(0, TimeSpan.FromSeconds(60));
        Assert.Equal(int.MaxValue, limiter.GetRemaining("client1"));
    }

    // ──────────────────────────────────────────────────────────────
    // Sliding Window (time-based)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task IsAllowed_AfterWindowExpires_AllowsAgain()
    {
        // Use a very short window for testing
        using var limiter = new RateLimiter(2, TimeSpan.FromMilliseconds(200));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.False(limiter.IsAllowed("client1"));

        // Wait for window to expire
        await Task.Delay(300);

        // Should be allowed again
        Assert.True(limiter.IsAllowed("client1"));
    }

    // ──────────────────────────────────────────────────────────────
    // Properties
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void MaxRequests_ReturnsConfiguredValue()
    {
        using var limiter = new RateLimiter(42, TimeSpan.FromSeconds(30));
        Assert.Equal(42, limiter.MaxRequests);
    }

    [Fact]
    public void Window_ReturnsConfiguredValue()
    {
        using var limiter = new RateLimiter(10, TimeSpan.FromSeconds(30));
        Assert.Equal(TimeSpan.FromSeconds(30), limiter.Window);
    }

    // ──────────────────────────────────────────────────────────────
    // Dispose
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var limiter = new RateLimiter(5, TimeSpan.FromSeconds(60));
        limiter.IsAllowed("client1");
        limiter.Dispose();
        // Double dispose should also not throw
        limiter.Dispose();
    }

    // ──────────────────────────────────────────────────────────────
    // Concurrency
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsAllowed_ConcurrentAccess_DoesNotExceedLimit()
    {
        using var limiter = new RateLimiter(100, TimeSpan.FromSeconds(60));
        int allowed = 0;
        var tasks = new Task[10];
        for (int t = 0; t < 10; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    if (limiter.IsAllowed("shared"))
                        Interlocked.Increment(ref allowed);
                }
            });
        }
        Task.WaitAll(tasks);

        // Exactly 100 should have been allowed (10 threads × 20 attempts = 200, but limit is 100)
        Assert.Equal(100, allowed);
    }

    // ──────────────────────────────────────────────────────────────
    // Single request limit
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsAllowed_SingleRequestLimit_AllowsOneThenBlocks()
    {
        using var limiter = new RateLimiter(1, TimeSpan.FromSeconds(60));
        Assert.True(limiter.IsAllowed("client1"));
        Assert.False(limiter.IsAllowed("client1"));
        Assert.False(limiter.IsAllowed("client1"));
    }
}
