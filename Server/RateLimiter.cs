// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Threading;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Lightweight sliding-window rate limiter.
/// Tracks request counts per client key within a configurable time window.
/// Thread-safe for concurrent use across multiple endpoints.
/// </summary>
public sealed class RateLimiter : IDisposable
{
    private readonly ConcurrentDictionary<string, ClientBucket> _buckets = new();
    private readonly Timer? _cleanupTimer;

    /// <summary>Maximum number of requests allowed per window.</summary>
    public int MaxRequests { get; }

    /// <summary>Sliding window duration.</summary>
    public TimeSpan Window { get; }

    /// <summary>
    /// Creates a new rate limiter.
    /// </summary>
    /// <param name="maxRequests">Maximum requests allowed per window. 0 or negative = unlimited.</param>
    /// <param name="window">Sliding window duration.</param>
    public RateLimiter(int maxRequests, TimeSpan window)
    {
        MaxRequests = maxRequests;
        Window = window;

        if (maxRequests > 0)
        {
            // Periodically clean up expired buckets (every 60 seconds)
            _cleanupTimer = new Timer(_ => Cleanup(), null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
        }
    }

    /// <summary>
    /// Checks whether a request from the given client key is allowed.
    /// Returns true if the request is within the rate limit, false if throttled.
    /// </summary>
    public bool IsAllowed(string clientKey)
    {
        if (MaxRequests <= 0) return true; // Unlimited
        if (string.IsNullOrEmpty(clientKey)) return true;

        var bucket = _buckets.GetOrAdd(clientKey, _ => new ClientBucket());
        return bucket.TryConsume(MaxRequests, Window);
    }

    /// <summary>
    /// Returns the number of remaining requests for the given client key within the current window.
    /// </summary>
    public int GetRemaining(string clientKey)
    {
        if (MaxRequests <= 0) return int.MaxValue;
        if (string.IsNullOrEmpty(clientKey)) return int.MaxValue;

        if (_buckets.TryGetValue(clientKey, out var bucket))
        {
            return Math.Max(0, MaxRequests - bucket.GetCount(Window));
        }
        return MaxRequests;
    }

    /// <summary>
    /// Removes expired entries to prevent memory growth.
    /// </summary>
    private void Cleanup()
    {
        var cutoff = DateTime.UtcNow - Window - TimeSpan.FromSeconds(10);
        foreach (var kvp in _buckets)
        {
            kvp.Value.Prune(Window);
            if (kvp.Value.IsEmpty)
            {
                _buckets.TryRemove(kvp.Key, out _);
            }
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        _buckets.Clear();
    }

    /// <summary>
    /// Per-client sliding window bucket using a lock-free timestamp ring.
    /// </summary>
    internal sealed class ClientBucket
    {
        // Use a simple list of timestamps — adequate for typical rate limits (e.g., 100-1000/min)
        private readonly object _lock = new();
        private readonly List<long> _timestamps = new();

        public bool TryConsume(int maxRequests, TimeSpan window)
        {
            var now = DateTime.UtcNow.Ticks;
            var windowTicks = window.Ticks;

            lock (_lock)
            {
                // Remove expired entries
                var cutoff = now - windowTicks;
                _timestamps.RemoveAll(t => t < cutoff);

                if (_timestamps.Count >= maxRequests)
                    return false;

                _timestamps.Add(now);
                return true;
            }
        }

        public int GetCount(TimeSpan window)
        {
            var now = DateTime.UtcNow.Ticks;
            var cutoff = now - window.Ticks;

            lock (_lock)
            {
                _timestamps.RemoveAll(t => t < cutoff);
                return _timestamps.Count;
            }
        }

        public void Prune(TimeSpan window)
        {
            var cutoff = DateTime.UtcNow.Ticks - window.Ticks;
            lock (_lock)
            {
                _timestamps.RemoveAll(t => t < cutoff);
            }
        }

        public bool IsEmpty
        {
            get
            {
                lock (_lock) { return _timestamps.Count == 0; }
            }
        }
    }
}
