// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using BenchmarkDotNet.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.VSDiagnostics;

namespace ServerBenchmarks;
[CPUUsageDiagnoser]
public class SqliteLoggerBenchmarks
{
    private SqliteConnection _connection = null!;
    private SqliteCommand _insertCmd = null!;
    private SqliteParameter _pTime = null!;
    private SqliteParameter _pName = null!;
    private SqliteParameter _pVal = null!;
    private SqliteParameter _pValStr = null!;
    private SqliteParameter _pQuality = null!;
    private readonly object _lock = new();
    [Params(100, 1000)]
    public int VariableCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        using var walCmd = _connection.CreateCommand();
        walCmd.CommandText = "PRAGMA journal_mode=WAL;";
        walCmd.ExecuteNonQuery();
        using var createCmd = _connection.CreateCommand();
        createCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS variable_data (
                time TEXT NOT NULL,
                variable_name TEXT NOT NULL,
                value REAL NULL,
                value_str TEXT NULL,
                quality INTEGER
            );
            CREATE INDEX IF NOT EXISTS idx_variable_data_name_time 
                ON variable_data (variable_name, time);";
        createCmd.ExecuteNonQuery();
        // Pre-create the parameterized command for reuse
        _insertCmd = _connection.CreateCommand();
        _insertCmd.CommandText = "INSERT INTO variable_data (time, variable_name, value, value_str, quality) VALUES ($time, $name, $val, $valstr, $quality)";
        _pTime = _insertCmd.Parameters.Add("$time", SqliteType.Text);
        _pName = _insertCmd.Parameters.Add("$name", SqliteType.Text);
        _pVal = _insertCmd.Parameters.Add("$val", SqliteType.Real);
        _pValStr = _insertCmd.Parameters.Add("$valstr", SqliteType.Text);
        _pQuality = _insertCmd.Parameters.Add("$quality", SqliteType.Integer);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _insertCmd?.Dispose();
        _connection?.Dispose();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        // Clear the table between iterations for consistent measurement
        using var delCmd = _connection.CreateCommand();
        delCmd.CommandText = "DELETE FROM variable_data;";
        delCmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Current pattern: each variable change creates a new command with lock, no transaction batching.
    /// Simulates Task.Run(() => LogToDb(...)) serialized by lock(_lock).
    /// </summary>
    [Benchmark(Baseline = true)]
    public void IndividualInsert_WithLock()
    {
        var now = DateTime.UtcNow.ToString("o");
        for (int i = 0; i < VariableCount; i++)
        {
            lock (_lock)
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = "INSERT INTO variable_data (time, variable_name, value, value_str, quality) VALUES (@t, @n, @v, @s, @q)";
                cmd.Parameters.AddWithValue("@t", now);
                cmd.Parameters.AddWithValue("@n", $"Var_{i}");
                cmd.Parameters.AddWithValue("@v", (double)i * 1.1);
                cmd.Parameters.AddWithValue("@s", DBNull.Value);
                cmd.Parameters.AddWithValue("@q", 0);
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Optimized: batch all inserts in a single transaction with reused prepared command.
    /// </summary>
    [Benchmark]
    public void BatchedTransaction_PreparedCommand()
    {
        var now = DateTime.UtcNow.ToString("o");
        lock (_lock)
        {
            using var tx = _connection.BeginTransaction();
            for (int i = 0; i < VariableCount; i++)
            {
                _pTime.Value = now;
                _pName.Value = $"Var_{i}";
                _pVal.Value = (double)i * 1.1;
                _pValStr.Value = DBNull.Value;
                _pQuality.Value = 0L;
                _insertCmd.ExecuteNonQuery();
            }

            tx.Commit();
        }
    }
}