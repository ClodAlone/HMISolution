using System.Data.Common;
using Microsoft.Data.Sqlite;
using Npgsql;
using SharedModels;

namespace ServerEditorWeb.Services;

public class DataLogEntry
{
    public DateTime Time { get; set; }
    public string VariableName { get; set; } = "";
    public double? NumericValue { get; set; }
    public string? StringValue { get; set; }
    public int Quality { get; set; }

    public string DisplayValue => NumericValue.HasValue ? NumericValue.Value.ToString("G6") : StringValue ?? "";
}

/// <summary>
/// Holds the resolved connection info needed to read from the data logging database.
/// </summary>
public class DataLogConnectionInfo
{
    public string Provider { get; set; } = "";
    public string ConnectionString { get; set; } = "";
    public string TableName { get; set; } = "variable_data";
}

public class DataLoggingReaderService
{
    /// <summary>
    /// Scans the loaded NodeModel to find all variables that have DataLogging enabled.
    /// Returns their full path names.
    /// </summary>
    public List<string> GetLoggedVariables(NodeModel? model)
    {
        var result = new List<string>();
        if (model?.Folder != null)
            CollectLoggedVariables(model.Folder, "", result);
        return result;
    }

    private void CollectLoggedVariables(Folder folder, string prefix, List<string> result)
    {
        string currentPath = string.IsNullOrEmpty(prefix) ? folder.Name : $"{prefix}.{folder.Name}";

        foreach (var variable in folder.Variables)
        {
            if (variable.DataLogging is { Enabled: true })
                result.Add($"{currentPath}.{variable.Name}");
        }

        foreach (var subFolder in folder.Folders)
            CollectLoggedVariables(subFolder, currentPath, result);
    }

    /// <summary>
    /// Resolves connection info from the DatabaseConfig and the nodes.json path.
    /// Returns null if no valid database is configured.
    /// </summary>
    public DataLogConnectionInfo? ResolveConnection(DatabaseConfig? db, string nodesFilePath)
    {
        if (db == null || string.IsNullOrWhiteSpace(db.ConnectionString))
            return null;

        var provider = db.Provider ?? "TimescaleDb";

        if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            var connStr = db.ConnectionString;
            if (connStr == ":memory:")
                return null;

            var dir = Path.GetDirectoryName(Path.GetFullPath(nodesFilePath));
            if (dir == null) return null;

            var fullPath = Path.GetFullPath(Path.Combine(dir, connStr));
            if (!File.Exists(fullPath))
                return null;

            return new DataLogConnectionInfo
            {
                Provider = "Sqlite",
                ConnectionString = $"Data Source={fullPath};Mode=ReadOnly",
                TableName = db.TableName
            };
        }

        if (provider.Equals("TimescaleDb", StringComparison.OrdinalIgnoreCase)
            || provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase)
            || provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        {
            return new DataLogConnectionInfo
            {
                Provider = "TimescaleDb",
                ConnectionString = db.ConnectionString,
                TableName = db.TableName
            };
        }

        return null;
    }

    /// <summary>
    /// Reads logged data for a specific variable.
    /// </summary>
    public List<DataLogEntry> ReadHistory(DataLogConnectionInfo connInfo, string variableName,
        DateTime startTime, DateTime endTime, int maxRows = 500)
    {
        var entries = new List<DataLogEntry>();

        try
        {
            using var conn = CreateConnection(connInfo);
            conn.Open();

            using var cmd = conn.CreateCommand();

            if (connInfo.Provider == "Sqlite")
            {
                cmd.CommandText = $@"
                    SELECT time, variable_name, value, value_str, quality 
                    FROM {connInfo.TableName} 
                    WHERE variable_name = @n AND time >= @s AND time <= @e 
                    ORDER BY time DESC 
                    LIMIT @limit";
                AddParameter(cmd, "@n", variableName);
                AddParameter(cmd, "@s", startTime.ToString("o"));
                AddParameter(cmd, "@e", endTime.ToString("o"));
                AddParameter(cmd, "@limit", maxRows);
            }
            else
            {
                cmd.CommandText = $@"
                    SELECT time, variable_name, value, value_str, quality 
                    FROM {connInfo.TableName} 
                    WHERE variable_name = @n AND time >= @s AND time <= @e 
                    ORDER BY time DESC 
                    LIMIT @limit";
                AddParameter(cmd, "@n", variableName);
                AddParameter(cmd, "@s", startTime);
                AddParameter(cmd, "@e", endTime);
                AddParameter(cmd, "@limit", maxRows);
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var entry = new DataLogEntry
                {
                    VariableName = reader.GetString(1),
                    Quality = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                };

                // Parse time — SQLite stores as ISO string, PostgreSQL as TIMESTAMPTZ
                if (connInfo.Provider == "Sqlite")
                    entry.Time = DateTime.Parse(reader.GetString(0)).ToUniversalTime();
                else
                    entry.Time = reader.GetDateTime(0).ToUniversalTime();

                if (!reader.IsDBNull(2))
                    entry.NumericValue = reader.GetDouble(2);
                else if (!reader.IsDBNull(3))
                    entry.StringValue = reader.GetString(3);

                entries.Add(entry);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading data log ({connInfo.Provider}): {ex.Message}");
        }

        return entries;
    }

    /// <summary>
    /// Gets summary statistics for a variable.
    /// </summary>
    public (int count, DateTime? oldest, DateTime? newest) GetVariableStats(
        DataLogConnectionInfo connInfo, string variableName)
    {
        try
        {
            using var conn = CreateConnection(connInfo);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                SELECT COUNT(*), MIN(time), MAX(time)
                FROM {connInfo.TableName}
                WHERE variable_name = @n";
            AddParameter(cmd, "@n", variableName);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var count = Convert.ToInt32(reader.GetValue(0));

                DateTime? oldest = null;
                DateTime? newest = null;

                if (!reader.IsDBNull(1))
                {
                    if (connInfo.Provider == "Sqlite")
                        oldest = DateTime.Parse(reader.GetString(1)).ToUniversalTime();
                    else
                        oldest = reader.GetDateTime(1).ToUniversalTime();
                }

                if (!reader.IsDBNull(2))
                {
                    if (connInfo.Provider == "Sqlite")
                        newest = DateTime.Parse(reader.GetString(2)).ToUniversalTime();
                    else
                        newest = reader.GetDateTime(2).ToUniversalTime();
                }

                return (count, oldest, newest);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading data log stats ({connInfo.Provider}): {ex.Message}");
        }

        return (0, null, null);
    }

    private static DbConnection CreateConnection(DataLogConnectionInfo connInfo)
    {
        return connInfo.Provider == "Sqlite"
            ? new SqliteConnection(connInfo.ConnectionString)
            : new NpgsqlConnection(connInfo.ConnectionString);
    }

    private static void AddParameter(DbCommand cmd, string name, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
    }
}
