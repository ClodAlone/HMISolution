// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace SqliteThroughputBenchmark;

/// <summary>
/// Benchmarks SQLite write throughput simulating HMI data-logging scenarios.
/// Tests individual inserts, batched transactions, WAL mode, and in-memory databases
/// to report maximum records/second for each strategy.
/// </summary>
internal static class Program
{
    private const int DefaultRecordCount = 100_000;
    private const int WarmupRecords = 1_000;

    static void Main(string[] args)
    {
        int recordCount = args.Length > 0 && int.TryParse(args[0], out var n) ? n : DefaultRecordCount;

        Console.WriteLine("â•”â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•—");
        Console.WriteLine("â•‘          SQLite Data-Logging Throughput Benchmark            â•‘");
        Console.WriteLine("â•šâ•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
        Console.WriteLine();
        Console.WriteLine($"  Records per test : {recordCount:N0}");
        Console.WriteLine($"  Runtime          : {Environment.Version}");
        Console.WriteLine($"  OS               : {Environment.OSVersion}");
        Console.WriteLine($"  Processors       : {Environment.ProcessorCount}");
        Console.WriteLine();

        var results = new List<BenchmarkResult>();

        // â”€â”€ Warm-up (not reported) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        Console.Write("  Warming up â€¦ ");
        RunBenchmark(":memory:", WarmupRecords, useBatch: true, useWal: false);
        Console.WriteLine("done.");
        Console.WriteLine();

        // â”€â”€ 1. File-based, individual inserts, no WAL â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        results.Add(RunBenchmark(
            TempDbPath("bench_single"),
            Math.Min(recordCount, 500),
            useBatch: false,
            useWal: false,
            label: "File  | Single insert | journal=DELETE"));

        // â”€â”€ 2. File-based, batched (single transaction), no WAL â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        results.Add(RunBenchmark(
            TempDbPath("bench_batch"),
            recordCount,
            useBatch: true,
            useWal: false,
            label: "File  | Batched txn   | journal=DELETE"));

        // â”€â”€ 3. File-based, batched, WAL mode â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        results.Add(RunBenchmark(
            TempDbPath("bench_wal"),
            recordCount,
            useBatch: true,
            useWal: true,
            label: "File  | Batched txn   | journal=WAL"));

        // â”€â”€ 4. File-based, batched, WAL + SYNCHRONOUS=NORMAL â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        results.Add(RunBenchmark(
            TempDbPath("bench_wal_normal"),
            recordCount,
            useBatch: true,
            useWal: true,
            syncNormal: true,
            label: "File  | Batched txn   | WAL+SYNC=NORMAL"));

        // â”€â”€ 5. In-memory, batched â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        results.Add(RunBenchmark(
            ":memory:",
            recordCount,
            useBatch: true,
            useWal: false,
            label: "Memory| Batched txn   | in-memory"));

        // â”€â”€ 6. File-based, batched, WAL, multi-producer (4 threads) â”€â”€â”€â”€â”€â”€â”€â”€â”€
        results.Add(RunMultiProducerBenchmark(
            TempDbPath("bench_wal_mp"),
            recordCount,
            producerCount: 4,
            useWal: true,
            label: "File  | 4 producers   | WAL+batched"));

        // â”€â”€ Results â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        Console.WriteLine();
        Console.WriteLine("â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”");
        Console.WriteLine("â”‚  Scenario                             â”‚  Records  â”‚  Elapsed   â”‚  Records/sec       â”‚");
        Console.WriteLine("â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤");

        foreach (var r in results)
        {
            Console.WriteLine($"â”‚  {r.Label,-37} â”‚ {r.Records,8:N0} â”‚ {r.Elapsed.TotalSeconds,8:F3} s â”‚ {r.RecordsPerSecond,17:N0} â”‚");
        }

        Console.WriteLine("â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜");
        Console.WriteLine();

        // Cleanup temp files
        CleanupTempFiles();
    }

    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    // Core benchmark: single-connection writer
    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    static BenchmarkResult RunBenchmark(
        string dbPath,
        int recordCount,
        bool useBatch,
        bool useWal,
        bool syncNormal = false,
        string? label = null)
    {
        label ??= dbPath;
        var connStr = dbPath == ":memory:"
            ? "Data Source=:memory:"
            : $"Data Source={dbPath}";

        using var connection = new SqliteConnection(connStr);
        connection.Open();

        ApplyPragmas(connection, useWal, syncNormal);
        CreateTable(connection);

        // Pre-create the parameterized command
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO log_data (time, variable_name, value, quality) VALUES (@t, @n, @v, @q)";
        var pTime = cmd.Parameters.Add("@t", SqliteType.Text);
        var pName = cmd.Parameters.Add("@n", SqliteType.Text);
        var pValue = cmd.Parameters.Add("@v", SqliteType.Real);
        var pQuality = cmd.Parameters.Add("@q", SqliteType.Integer);
        cmd.Prepare();

        var baseTime = DateTime.UtcNow;
        var variableNames = GenerateVariableNames(50);
        var rng = new Random(42);

        var sw = Stopwatch.StartNew();

        if (useBatch)
        {
            using var txn = connection.BeginTransaction();
            cmd.Transaction = txn;
            for (int i = 0; i < recordCount; i++)
            {
                pTime.Value = baseTime.AddMilliseconds(i).ToString("o");
                pName.Value = variableNames[i % variableNames.Length];
                pValue.Value = rng.NextDouble() * 1000.0;
                pQuality.Value = 0; // Good quality
                cmd.ExecuteNonQuery();
            }
            txn.Commit();
        }
        else
        {
            for (int i = 0; i < recordCount; i++)
            {
                pTime.Value = baseTime.AddMilliseconds(i).ToString("o");
                pName.Value = variableNames[i % variableNames.Length];
                pValue.Value = rng.NextDouble() * 1000.0;
                pQuality.Value = 0;
                cmd.ExecuteNonQuery();
            }
        }

        sw.Stop();

        var result = new BenchmarkResult(label, recordCount, sw.Elapsed);
        Console.WriteLine($"  âœ“ {label}  â†’  {result.RecordsPerSecond:N0} rec/s  ({sw.Elapsed.TotalSeconds:F3}s)");
        return result;
    }

    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    // Multi-producer benchmark: N threads writing concurrently
    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    static BenchmarkResult RunMultiProducerBenchmark(
        string dbPath,
        int totalRecordCount,
        int producerCount,
        bool useWal,
        string? label = null)
    {
        label ??= $"{producerCount} producers";
        var connStr = $"Data Source={dbPath}";

        // Create the DB and table with a setup connection
        using (var setup = new SqliteConnection(connStr))
        {
            setup.Open();
            ApplyPragmas(setup, useWal, syncNormal: true);
            CreateTable(setup);
        }

        int recordsPerProducer = totalRecordCount / producerCount;
        var barrier = new Barrier(producerCount);

        var sw = Stopwatch.StartNew();
        var tasks = new Thread[producerCount];

        for (int p = 0; p < producerCount; p++)
        {
            int producerId = p;
            tasks[p] = new Thread(() =>
            {
                using var conn = new SqliteConnection(connStr);
                conn.Open();
                ApplyPragmas(conn, useWal, syncNormal: true);

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO log_data (time, variable_name, value, quality) VALUES (@t, @n, @v, @q)";
                var pTime = cmd.Parameters.Add("@t", SqliteType.Text);
                var pName = cmd.Parameters.Add("@n", SqliteType.Text);
                var pValue = cmd.Parameters.Add("@v", SqliteType.Real);
                var pQuality = cmd.Parameters.Add("@q", SqliteType.Integer);
                cmd.Prepare();

                var baseTime = DateTime.UtcNow;
                var variableNames = GenerateVariableNames(50);
                var rng = new Random(42 + producerId);

                barrier.SignalAndWait(); // sync start

                using var txn = conn.BeginTransaction();
                cmd.Transaction = txn;
                for (int i = 0; i < recordsPerProducer; i++)
                {
                    pTime.Value = baseTime.AddMilliseconds(i + producerId * recordsPerProducer).ToString("o");
                    pName.Value = variableNames[i % variableNames.Length];
                    pValue.Value = rng.NextDouble() * 1000.0;
                    pQuality.Value = 0;
                    cmd.ExecuteNonQuery();
                }
                txn.Commit();
            });
            tasks[p].Start();
        }

        foreach (var t in tasks)
            t.Join();

        sw.Stop();

        int actualTotal = recordsPerProducer * producerCount;
        var result = new BenchmarkResult(label, actualTotal, sw.Elapsed);
        Console.WriteLine($"  âœ“ {label}  â†’  {result.RecordsPerSecond:N0} rec/s  ({sw.Elapsed.TotalSeconds:F3}s)");
        return result;
    }

    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    // Helpers
    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    static void ApplyPragmas(SqliteConnection connection, bool useWal, bool syncNormal = false)
    {
        using var cmd = connection.CreateCommand();

        if (useWal)
        {
            cmd.CommandText = "PRAGMA journal_mode=WAL;";
            cmd.ExecuteNonQuery();
        }

        if (syncNormal)
        {
            cmd.CommandText = "PRAGMA synchronous=NORMAL;";
            cmd.ExecuteNonQuery();
        }

        // Increase page cache for better performance
        cmd.CommandText = "PRAGMA cache_size=-64000;"; // 64 MB
        cmd.ExecuteNonQuery();

        cmd.CommandText = "PRAGMA temp_store=MEMORY;";
        cmd.ExecuteNonQuery();
    }

    static void CreateTable(SqliteConnection connection)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS log_data (
                time           TEXT    NOT NULL,
                variable_name  TEXT    NOT NULL,
                value          REAL    NULL,
                quality        INTEGER NOT NULL
            );
            CREATE INDEX IF NOT EXISTS idx_log_name_time
                ON log_data (variable_name, time);
            """;
        cmd.ExecuteNonQuery();
    }

    static string[] GenerateVariableNames(int count)
    {
        var names = new string[count];
        for (int i = 0; i < count; i++)
            names[i] = $"ns=2;s=Channel1.Device1.Tag{i:D4}";
        return names;
    }

    static string TempDbPath(string name)
    {
        return Path.Combine(Path.GetTempPath(), $"{name}_{Guid.NewGuid():N}.db");
    }

    static void CleanupTempFiles()
    {
        try
        {
            foreach (var f in Directory.GetFiles(Path.GetTempPath(), "bench_*.db"))
                File.Delete(f);
            foreach (var f in Directory.GetFiles(Path.GetTempPath(), "bench_*-wal"))
                File.Delete(f);
            foreach (var f in Directory.GetFiles(Path.GetTempPath(), "bench_*-shm"))
                File.Delete(f);
        }
        catch { /* best-effort cleanup */ }
    }

    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    // Result
    // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    record BenchmarkResult(string Label, int Records, TimeSpan Elapsed)
    {
        public double RecordsPerSecond => Records / Elapsed.TotalSeconds;
    }
}
