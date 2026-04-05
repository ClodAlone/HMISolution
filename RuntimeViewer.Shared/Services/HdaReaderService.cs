using System.Data.Common;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Npgsql;
using SharedModels;

namespace RuntimeViewer.Shared.Services;

/// <summary>Data point returned by HDA queries.</summary>
public class HdaDataPoint
{
    public DateTime Time { get; set; }
    public double? Value { get; set; }
    public string? StringValue { get; set; }
}

/// <summary>One series of HDA data for a single variable.</summary>
public class HdaSeries
{
    public string VariableName { get; set; } = "";
    public List<HdaDataPoint> Points { get; set; } = new();
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
}

/// <summary>
/// Reads historical data from the data-logging database (Sqlite or TimescaleDb/PostgreSQL)
/// for display in the runtime HDA chart widget.
/// </summary>
public class HdaReaderService
{
    private readonly ProjectService _project;

    public HdaReaderService(ProjectService project)
    {
        _project = project;
    }

    /// <summary>
    /// Read history for multiple variables and return one series per variable.
    /// </summary>
    public async Task<List<HdaSeries>> ReadHistoryAsync(
        List<string> variablePaths, int timeRangeMinutes, int maxPoints)
    {
        var result = new List<HdaSeries>();
        var connInfo = ResolveConnection();
        if (connInfo == null) return result;

        var endTime = DateTime.UtcNow;
        var startTime = timeRangeMinutes > 0 ? endTime.AddMinutes(-timeRangeMinutes) : DateTime.MinValue;

        // Read all series concurrently sharing a single connection open
        var tasks = variablePaths.Select(path =>
            Task.Run(() => ReadSeries(connInfo, path, startTime, endTime, maxPoints))).ToList();

        var series = await Task.WhenAll(tasks);
        result.AddRange(series);

        return result;
    }

    /// <summary>
    /// Collects all variable paths that have DataLogging enabled.
    /// </summary>
    public List<string> GetLoggedVariables()
    {
        var result = new List<string>();
        var model = _project.Model;
        if (model?.Folder != null)
            CollectLoggedVariables(model.Folder, "", result);
        return result;
    }

    private void CollectLoggedVariables(Folder folder, string prefix, List<string> result)
    {
        var currentPath = string.IsNullOrEmpty(prefix) ? folder.Name : $"{prefix}.{folder.Name}";
        foreach (var v in folder.Variables)
        {
            if (v.DataLogging is { Enabled: true })
                result.Add($"{currentPath}.{v.Name}");
        }
        foreach (var sub in folder.Folders)
            CollectLoggedVariables(sub, currentPath, result);
    }

    private HdaSeries ReadSeries(ConnectionInfo connInfo, string variableName,
        DateTime startTime, DateTime endTime, int maxPoints)
    {
        var series = new HdaSeries { VariableName = variableName };

        try
        {
            using var conn = CreateConnection(connInfo);
            conn.Open();

            using var cmd = conn.CreateCommand();

            if (connInfo.Provider == "Sqlite")
            {
                cmd.CommandText = $@"
                    SELECT time, value, value_str
                    FROM {connInfo.TableName}
                    WHERE variable_name = @n AND time >= @s AND time <= @e
                    ORDER BY time ASC
                    LIMIT @limit";
                AddParam(cmd, "@n", variableName);
                AddParam(cmd, "@s", startTime.ToString("o"));
                AddParam(cmd, "@e", endTime.ToString("o"));
                AddParam(cmd, "@limit", maxPoints);
            }
            else
            {
                cmd.CommandText = $@"
                    SELECT time, value, value_str
                    FROM {connInfo.TableName}
                    WHERE variable_name = @n AND time >= @s AND time <= @e
                    ORDER BY time ASC
                    LIMIT @limit";
                AddParam(cmd, "@n", variableName);
                AddParam(cmd, "@s", startTime);
                AddParam(cmd, "@e", endTime);
                AddParam(cmd, "@limit", maxPoints);
            }

            using var reader = cmd.ExecuteReader();
            double min = double.MaxValue, max = double.MinValue;

            while (reader.Read())
            {
                var point = new HdaDataPoint();

                if (connInfo.Provider == "Sqlite")
                    point.Time = DateTime.Parse(reader.GetString(0)).ToUniversalTime();
                else
                    point.Time = reader.GetDateTime(0).ToUniversalTime();

                if (!reader.IsDBNull(1))
                {
                    point.Value = reader.GetDouble(1);
                    if (point.Value.Value < min) min = point.Value.Value;
                    if (point.Value.Value > max) max = point.Value.Value;
                }
                else if (!reader.IsDBNull(2))
                {
                    point.StringValue = reader.GetString(2);
                    if (double.TryParse(point.StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    {
                        point.Value = d;
                        if (d < min) min = d;
                        if (d > max) max = d;
                    }
                }

                series.Points.Add(point);
            }

            series.MinValue = min == double.MaxValue ? 0 : min;
            series.MaxValue = max == double.MinValue ? 100 : max;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"HDA read error for '{variableName}': {ex.Message}");
        }

        return series;
    }

    private ConnectionInfo? ResolveConnection()
    {
        var db = _project.Model?.Database;
        if (db == null || string.IsNullOrWhiteSpace(db.ConnectionString)) return null;

        var provider = db.Provider ?? "TimescaleDb";

        if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            if (db.ConnectionString == ":memory:") return null;
            var dir = Path.GetDirectoryName(Path.GetFullPath(_project.ConfigPath));
            if (dir == null) return null;
            // Check Data subfolder first (new convention), then root (backward compat)
            var dataPath = Path.GetFullPath(Path.Combine(dir, "Data", db.ConnectionString));
            var rootPath = Path.GetFullPath(Path.Combine(dir, db.ConnectionString));
            var fullPath = File.Exists(dataPath) ? dataPath : rootPath;
            if (!File.Exists(fullPath)) return null;
            return new ConnectionInfo
            {
                Provider = "Sqlite",
                ConnectionString = $"Data Source={fullPath};Mode=ReadOnly",
                TableName = db.TableName
            };
        }

        return new ConnectionInfo
        {
            Provider = "TimescaleDb",
            ConnectionString = db.ConnectionString,
            TableName = db.TableName
        };
    }

    private static DbConnection CreateConnection(ConnectionInfo info) =>
        info.Provider == "Sqlite"
            ? new SqliteConnection(info.ConnectionString)
            : new NpgsqlConnection(info.ConnectionString);

    private static void AddParam(DbCommand cmd, string name, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
    }

    private class ConnectionInfo
    {
        public string Provider { get; set; } = "";
        public string ConnectionString { get; set; } = "";
        public string TableName { get; set; } = "variable_data";
    }
}

