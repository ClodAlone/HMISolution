using System.Diagnostics;
using Npgsql;
using NpgsqlTypes;

namespace TimescaleThroughputBenchmark;

/// <summary>
/// Benchmarks TimescaleDB / PostgreSQL write throughput simulating HMI data-logging scenarios.
/// Tests individual inserts, batched transactions, COPY bulk-load, and multi-producer
/// scenarios to report maximum records/second for each strategy.
///
/// Prerequisites:
///   - A PostgreSQL instance with the TimescaleDB extension installed.
///   - Connection string via the TIMESCALE_CONN environment variable, e.g.:
///       Host=localhost;Port=5432;Database=benchmark;Username=postgres;Password=secret
///   - If the variable is unset, defaults to localhost with user "postgres" and database "benchmark".
/// </summary>
internal static class Program
{
    private const int DefaultRecordCount = 100_000;
    private const int WarmupRecords = 1_000;

    private static string _connectionString = null!;

    static void Main(string[] args)
    {
        int recordCount = args.Length > 0 && int.TryParse(args[0], out var n) ? n : DefaultRecordCount;

        _connectionString = Environment.GetEnvironmentVariable("TIMESCALE_CONN")
            ?? "Host=localhost;Port=5432;Database=benchmark;Username=postgres;Password=postgres";

        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║       TimescaleDB Data-Logging Throughput Benchmark           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"  Records per test : {recordCount:N0}");
        Console.WriteLine($"  Runtime          : {Environment.Version}");
        Console.WriteLine($"  OS               : {Environment.OSVersion}");
        Console.WriteLine($"  Processors       : {Environment.ProcessorCount}");
        Console.WriteLine($"  Connection       : {MaskPassword(_connectionString)}");
        Console.WriteLine();

        if (!TestConnection())
        {
            Console.Error.WriteLine("  ✗ Cannot connect to PostgreSQL. Set TIMESCALE_CONN and ensure the server is running.");
            return;
        }

        var results = new List<BenchmarkResult>();

        // ── Warm-up (not reported) ──────────────────────────────────────
        Console.Write("  Warming up … ");
        RunBenchmark(WarmupRecords, useBatch: true, useHypertable: false, label: "warmup");
        Console.WriteLine("done.");
        Console.WriteLine();

        // ── 1. Individual inserts, plain table ──────────────────────────
        results.Add(RunBenchmark(
            Math.Min(recordCount, 500),
            useBatch: false,
            useHypertable: false,
            label: "PG    | Single insert | plain table"));

        // ── 2. Batched (single transaction), plain table ────────────────
        results.Add(RunBenchmark(
            recordCount,
            useBatch: true,
            useHypertable: false,
            label: "PG    | Batched txn   | plain table"));

        // ── 3. Batched, hypertable (TimescaleDB) ────────────────────────
        results.Add(RunBenchmark(
            recordCount,
            useBatch: true,
            useHypertable: true,
            label: "TS    | Batched txn   | hypertable"));

        // ── 4. COPY bulk-load, plain table ──────────────────────────────
        results.Add(RunCopyBenchmark(
            recordCount,
            useHypertable: false,
            label: "PG    | COPY binary   | plain table"));

        // ── 5. COPY bulk-load, hypertable ───────────────────────────────
        results.Add(RunCopyBenchmark(
            recordCount,
            useHypertable: true,
            label: "TS    | COPY binary   | hypertable"));

        // ── 6. Multi-producer (4 connections), batched, hypertable ──────
        results.Add(RunMultiProducerBenchmark(
            recordCount,
            producerCount: 4,
            useHypertable: true,
            label: "TS    | 4 producers   | hypertable"));

        // ── 7. Multi-producer (4 connections), COPY, hypertable ─────────
        results.Add(RunMultiProducerCopyBenchmark(
            recordCount,
            producerCount: 4,
            useHypertable: true,
            label: "TS    | 4 prod COPY   | hypertable"));

        // ── Results ─────────────────────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("┌──────────────────────────────────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  Scenario                             │  Records  │  Elapsed   │  Records/sec       │");
        Console.WriteLine("├──────────────────────────────────────────────────────────────────────────────────────┤");

        foreach (var r in results)
        {
            Console.WriteLine($"│  {r.Label,-37} │ {r.Records,8:N0} │ {r.Elapsed.TotalSeconds,8:F3} s │ {r.RecordsPerSecond,17:N0} │");
        }

        Console.WriteLine("└──────────────────────────────────────────────────────────────────────────────────────┘");
        Console.WriteLine();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Core benchmark: single-connection writer with INSERT
    // ─────────────────────────────────────────────────────────────────────────
    static BenchmarkResult RunBenchmark(
        int recordCount,
        bool useBatch,
        bool useHypertable,
        string? label = null)
    {
        label ??= "benchmark";
        var tableName = CreateBenchTable(useHypertable);

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            $"INSERT INTO {tableName} (time, variable_name, value, quality) VALUES (@t, @n, @v, @q)", conn);
        var pTime = cmd.Parameters.Add(new NpgsqlParameter("t", NpgsqlDbType.TimestampTz));
        var pName = cmd.Parameters.Add(new NpgsqlParameter("n", NpgsqlDbType.Text));
        var pValue = cmd.Parameters.Add(new NpgsqlParameter("v", NpgsqlDbType.Double));
        var pQuality = cmd.Parameters.Add(new NpgsqlParameter("q", NpgsqlDbType.Integer));
        cmd.Prepare();

        var baseTime = DateTime.UtcNow;
        var variableNames = GenerateVariableNames(50);
        var rng = new Random(42);

        var sw = Stopwatch.StartNew();

        if (useBatch)
        {
            using var txn = conn.BeginTransaction();
            cmd.Transaction = txn;
            for (int i = 0; i < recordCount; i++)
            {
                pTime.Value = baseTime.AddMilliseconds(i);
                pName.Value = variableNames[i % variableNames.Length];
                pValue.Value = rng.NextDouble() * 1000.0;
                pQuality.Value = 0;
                cmd.ExecuteNonQuery();
            }
            txn.Commit();
        }
        else
        {
            for (int i = 0; i < recordCount; i++)
            {
                pTime.Value = baseTime.AddMilliseconds(i);
                pName.Value = variableNames[i % variableNames.Length];
                pValue.Value = rng.NextDouble() * 1000.0;
                pQuality.Value = 0;
                cmd.ExecuteNonQuery();
            }
        }

        sw.Stop();

        DropTable(tableName);

        var result = new BenchmarkResult(label, recordCount, sw.Elapsed);
        Console.WriteLine($"  ✓ {label}  →  {result.RecordsPerSecond:N0} rec/s  ({sw.Elapsed.TotalSeconds:F3}s)");
        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COPY benchmark: uses PostgreSQL COPY protocol for bulk loading
    // ─────────────────────────────────────────────────────────────────────────
    static BenchmarkResult RunCopyBenchmark(
        int recordCount,
        bool useHypertable,
        string? label = null)
    {
        label ??= "COPY";
        var tableName = CreateBenchTable(useHypertable);

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var baseTime = DateTime.UtcNow;
        var variableNames = GenerateVariableNames(50);
        var rng = new Random(42);

        var sw = Stopwatch.StartNew();

        using (var writer = conn.BeginBinaryImport(
            $"COPY {tableName} (time, variable_name, value, quality) FROM STDIN (FORMAT BINARY)"))
        {
            for (int i = 0; i < recordCount; i++)
            {
                writer.StartRow();
                writer.Write(baseTime.AddMilliseconds(i), NpgsqlDbType.TimestampTz);
                writer.Write(variableNames[i % variableNames.Length], NpgsqlDbType.Text);
                writer.Write(rng.NextDouble() * 1000.0, NpgsqlDbType.Double);
                writer.Write(0, NpgsqlDbType.Integer);
            }
            writer.Complete();
        }

        sw.Stop();

        DropTable(tableName);

        var result = new BenchmarkResult(label, recordCount, sw.Elapsed);
        Console.WriteLine($"  ✓ {label}  →  {result.RecordsPerSecond:N0} rec/s  ({sw.Elapsed.TotalSeconds:F3}s)");
        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Multi-producer benchmark: N connections writing concurrently (INSERT)
    // ─────────────────────────────────────────────────────────────────────────
    static BenchmarkResult RunMultiProducerBenchmark(
        int totalRecordCount,
        int producerCount,
        bool useHypertable,
        string? label = null)
    {
        label ??= $"{producerCount} producers";
        var tableName = CreateBenchTable(useHypertable);

        int recordsPerProducer = totalRecordCount / producerCount;
        var barrier = new Barrier(producerCount);

        var sw = Stopwatch.StartNew();
        var threads = new Thread[producerCount];

        for (int p = 0; p < producerCount; p++)
        {
            int producerId = p;
            threads[p] = new Thread(() =>
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand(
                    $"INSERT INTO {tableName} (time, variable_name, value, quality) VALUES (@t, @n, @v, @q)", conn);
                var pTime = cmd.Parameters.Add(new NpgsqlParameter("t", NpgsqlDbType.TimestampTz));
                var pName = cmd.Parameters.Add(new NpgsqlParameter("n", NpgsqlDbType.Text));
                var pValue = cmd.Parameters.Add(new NpgsqlParameter("v", NpgsqlDbType.Double));
                var pQuality = cmd.Parameters.Add(new NpgsqlParameter("q", NpgsqlDbType.Integer));
                cmd.Prepare();

                var baseTime = DateTime.UtcNow;
                var variableNames = GenerateVariableNames(50);
                var rng = new Random(42 + producerId);

                barrier.SignalAndWait();

                using var txn = conn.BeginTransaction();
                cmd.Transaction = txn;
                for (int i = 0; i < recordsPerProducer; i++)
                {
                    pTime.Value = baseTime.AddMilliseconds(i + producerId * recordsPerProducer);
                    pName.Value = variableNames[i % variableNames.Length];
                    pValue.Value = rng.NextDouble() * 1000.0;
                    pQuality.Value = 0;
                    cmd.ExecuteNonQuery();
                }
                txn.Commit();
            });
            threads[p].Start();
        }

        foreach (var t in threads)
            t.Join();

        sw.Stop();

        DropTable(tableName);

        int actualTotal = recordsPerProducer * producerCount;
        var result = new BenchmarkResult(label, actualTotal, sw.Elapsed);
        Console.WriteLine($"  ✓ {label}  →  {result.RecordsPerSecond:N0} rec/s  ({sw.Elapsed.TotalSeconds:F3}s)");
        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Multi-producer COPY benchmark: N connections, each using COPY protocol
    // ─────────────────────────────────────────────────────────────────────────
    static BenchmarkResult RunMultiProducerCopyBenchmark(
        int totalRecordCount,
        int producerCount,
        bool useHypertable,
        string? label = null)
    {
        label ??= $"{producerCount} producers COPY";
        var tableName = CreateBenchTable(useHypertable);

        int recordsPerProducer = totalRecordCount / producerCount;
        var barrier = new Barrier(producerCount);

        var sw = Stopwatch.StartNew();
        var threads = new Thread[producerCount];

        for (int p = 0; p < producerCount; p++)
        {
            int producerId = p;
            threads[p] = new Thread(() =>
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                var baseTime = DateTime.UtcNow;
                var variableNames = GenerateVariableNames(50);
                var rng = new Random(42 + producerId);

                barrier.SignalAndWait();

                using var writer = conn.BeginBinaryImport(
                    $"COPY {tableName} (time, variable_name, value, quality) FROM STDIN (FORMAT BINARY)");
                for (int i = 0; i < recordsPerProducer; i++)
                {
                    writer.StartRow();
                    writer.Write(baseTime.AddMilliseconds(i + producerId * recordsPerProducer), NpgsqlDbType.TimestampTz);
                    writer.Write(variableNames[i % variableNames.Length], NpgsqlDbType.Text);
                    writer.Write(rng.NextDouble() * 1000.0, NpgsqlDbType.Double);
                    writer.Write(0, NpgsqlDbType.Integer);
                }
                writer.Complete();
            });
            threads[p].Start();
        }

        foreach (var t in threads)
            t.Join();

        sw.Stop();

        DropTable(tableName);

        int actualTotal = recordsPerProducer * producerCount;
        var result = new BenchmarkResult(label, actualTotal, sw.Elapsed);
        Console.WriteLine($"  ✓ {label}  →  {result.RecordsPerSecond:N0} rec/s  ({sw.Elapsed.TotalSeconds:F3}s)");
        return result;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

    static bool TestConnection()
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT 1", conn);
            cmd.ExecuteScalar();
            Console.WriteLine("  ✓ Connected to PostgreSQL");

            // Check for TimescaleDB extension
            using var extCmd = new NpgsqlCommand(
                "SELECT EXISTS(SELECT 1 FROM pg_extension WHERE extname = 'timescaledb')", conn);
            var hasTimescale = (bool)extCmd.ExecuteScalar()!;
            if (hasTimescale)
                Console.WriteLine("  ✓ TimescaleDB extension detected");
            else
                Console.WriteLine("  ⚠ TimescaleDB extension NOT found — hypertable tests will be skipped or fail");

            Console.WriteLine();
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"  ✗ Connection failed: {ex.Message}");
            return false;
        }
    }

    static string CreateBenchTable(bool useHypertable)
    {
        var tableName = $"bench_{Guid.NewGuid():N}";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using (var cmd = new NpgsqlCommand($@"
            CREATE TABLE {tableName} (
                time           TIMESTAMPTZ    NOT NULL,
                variable_name  TEXT           NOT NULL,
                value          DOUBLE PRECISION NULL,
                quality        INTEGER        NOT NULL
            );
            CREATE INDEX ON {tableName} (variable_name, time);
        ", conn))
        {
            cmd.ExecuteNonQuery();
        }

        if (useHypertable)
        {
            try
            {
                using var cmd = new NpgsqlCommand(
                    $"SELECT create_hypertable('{tableName}', 'time');", conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  ⚠ Hypertable creation failed: {ex.Message}");
            }
        }

        return tableName;
    }

    static void DropTable(string tableName)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tableName} CASCADE", conn);
            cmd.ExecuteNonQuery();
        }
        catch { /* best-effort cleanup */ }
    }

    static string[] GenerateVariableNames(int count)
    {
        var names = new string[count];
        for (int i = 0; i < count; i++)
            names[i] = $"ns=2;s=Channel1.Device1.Tag{i:D4}";
        return names;
    }

    static string MaskPassword(string connStr)
    {
        // Simple masking — replace Password=xxx with Password=***
        var idx = connStr.IndexOf("Password=", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return connStr;
        var end = connStr.IndexOf(';', idx);
        if (end < 0) end = connStr.Length;
        return string.Concat(connStr.AsSpan(0, idx), "Password=***", connStr.AsSpan(end));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Result
    // ─────────────────────────────────────────────────────────────────────────
    record BenchmarkResult(string Label, int Records, TimeSpan Elapsed)
    {
        public double RecordsPerSecond => Records / Elapsed.TotalSeconds;
    }
}
