// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using BenchmarkDotNet.Attributes;
using Microsoft.Data.Sqlite;
using System.Globalization;
using Microsoft.VSDiagnostics;

namespace RuntimeViewer.Shared.Benchmarks;
[CPUUsageDiagnoser]
public class HdaReaderBenchmarks
{
    private string _connString = null!;
    private string[] _variableNames = null!;
    private string _startTimeStr = null!;
    private string _endTimeStr = null!;
    [Params(10)]
    public int VariableCount { get; set; }

    [Params(500)]
    public int MaxPoints { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Use the live StressTest_1K history.db
        var dbPath = @"C:\Work\samples\StressTest_1K\Data\history.db";
        if (!File.Exists(dbPath))
            throw new FileNotFoundException("history.db not found", dbPath);
        _connString = $"Data Source={dbPath};Mode=ReadOnly";
        // Discover variable names from the database
        using var conn = new SqliteConnection(_connString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT DISTINCT variable_name FROM variable_data LIMIT 20";
        var names = new List<string>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            names.Add(reader.GetString(0));
        _variableNames = names.Take(VariableCount).ToArray();
        // Time window: last 60 minutes from most recent row
        var cmd2 = conn.CreateCommand();
        cmd2.CommandText = "SELECT MAX(time) FROM variable_data";
        var maxTime = DateTime.Parse((string)cmd2.ExecuteScalar()!).ToUniversalTime();
        _endTimeStr = maxTime.ToString("o");
        _startTimeStr = maxTime.AddMinutes(-60).ToString("o");
    }

    /// <summary>
    /// Baseline: mirrors the current HdaReaderService.ReadHistoryAsync pattern —
    /// one connection + query per variable, Task.Run per variable, DateTime.Parse per row.
    /// </summary>
    [Benchmark(Description = "Current: 1-conn-per-var")]
    public List<SeriesResult> Current_OneConnectionPerVariable()
    {
        var results = new List<SeriesResult>(_variableNames.Length);
        var tasks = _variableNames.Select(name => Task.Run(() =>
        {
            var series = new SeriesResult
            {
                Name = name
            };
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT time, value, value_str
                FROM variable_data
                WHERE variable_name = @n AND time >= @s AND time <= @e
                ORDER BY time ASC
                LIMIT @limit";
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@s", _startTimeStr);
            cmd.Parameters.AddWithValue("@e", _endTimeStr);
            cmd.Parameters.AddWithValue("@limit", MaxPoints);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var time = DateTime.Parse(reader.GetString(0)).ToUniversalTime();
                double? val = reader.IsDBNull(1) ? null : reader.GetDouble(1);
                string? valStr = null;
                if (val == null && !reader.IsDBNull(2))
                {
                    valStr = reader.GetString(2);
                    if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                        val = d;
                }

                series.Points.Add(new DataPt { Time = time, Value = val, StringValue = valStr });
            }

            return series;
        })).ToArray();
        Task.WaitAll(tasks);
        foreach (var t in tasks)
            results.Add(t.Result);
        return results;
    }

    /// <summary>
    /// Optimized: parallel reads with prepared commands, ParseExact, pre-allocated lists, local doubles.
    /// </summary>
    [Benchmark(Description = "Optimized: prepared+ParseExact")]
    public List<SeriesResult> Optimized_PreparedParseExact()
    {
        var results = new List<SeriesResult>(_variableNames.Length);
        var tasks = _variableNames.Select(name => Task.Run(() =>
        {
            var series = new SeriesResult { Name = name };
            series.Points = new List<DataPt>(MaxPoints);
            using var conn = new SqliteConnection(_connString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT time, value, value_str
                FROM variable_data
                WHERE variable_name = $n AND time >= $s AND time <= $e
                ORDER BY time ASC
                LIMIT $limit";
            cmd.Parameters.Add("$n", SqliteType.Text).Value = name;
            cmd.Parameters.Add("$s", SqliteType.Text).Value = _startTimeStr;
            cmd.Parameters.Add("$e", SqliteType.Text).Value = _endTimeStr;
            cmd.Parameters.Add("$limit", SqliteType.Integer).Value = MaxPoints;
            cmd.Prepare();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var time = DateTime.ParseExact(reader.GetString(0), "o",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
                double? val = null;
                string? valStr = null;
                if (!reader.IsDBNull(1))
                {
                    var d = reader.GetDouble(1);
                    val = d;
                }
                else if (!reader.IsDBNull(2))
                {
                    valStr = reader.GetString(2);
                    if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                        val = d;
                }
                series.Points.Add(new DataPt { Time = time, Value = val, StringValue = valStr });
            }
            return series;
        })).ToArray();

        Task.WaitAll(tasks);
        foreach (var t in tasks) results.Add(t.Result);
        return results;
    }

    public class SeriesResult
    {
        public string Name { get; set; } = "";
        public List<DataPt> Points { get; set; } = new();
    }

    public class DataPt
    {
        public DateTime Time { get; set; }
        public double? Value { get; set; }
        public string? StringValue { get; set; }
    }
}