using System.Globalization;
using System.Data.Common;

namespace RuntimeViewer.Shared.Services;

public partial class HdaReaderService
{
    public async Task<List<HdaSeries>> ReadSampledHistoryAsync(List<string> paths, DateTime startUtc,
        DateTime endUtc, int intervalSeconds, CancellationToken ct = default)
    {
        var timeline = HistoricalSampling.Timeline(startUtc, endUtc, intervalSeconds);
        var variables = paths.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct(StringComparer.Ordinal).ToList();
        if (variables.Count == 0 || variables.Count > 20)
            throw new ArgumentException("Configure between 1 and 20 historical variables.");
        var info = ResolveConnection() ?? throw new InvalidOperationException("Historical database is not configured or is unavailable on the RuntimeViewer host.");
        if (!System.Text.RegularExpressions.Regex.IsMatch(info.TableName, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            throw new InvalidOperationException("Historical database table must be a simple SQL identifier.");

        return await Task.Run(() => variables.Select(path =>
            HistoricalSampling.Sample(path, ReadChanges(info, path, startUtc, endUtc, ct), timeline)).ToList(), ct);
    }

    private static IEnumerable<HdaDataPoint> ReadChanges(ConnectionInfo info, string path,
        DateTime start, DateTime end, CancellationToken ct)
    {
        using var connection = CreateConnection(info);
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = $@"
            SELECT time, value, value_str FROM {info.TableName}
            WHERE variable_name = @n AND time = (
                SELECT MAX(time) FROM {info.TableName} WHERE variable_name = @n AND time <= @s)
            UNION ALL
            SELECT time, value, value_str FROM {info.TableName}
            WHERE variable_name = @n AND time > @s AND time <= @e
            ORDER BY time ASC";
        AddParam(cmd, "@n", path);
        AddParam(cmd, "@s", info.Provider == "Sqlite" ? start.ToString("o", CultureInfo.InvariantCulture) : (object)start);
        AddParam(cmd, "@e", info.Provider == "Sqlite" ? end.ToString("o", CultureInfo.InvariantCulture) : (object)end);
        using var registration = ct.Register(cmd.Cancel);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            ct.ThrowIfCancellationRequested();
            var point = new HdaDataPoint
            {
                Time = info.Provider == "Sqlite"
                    ? DateTime.Parse(reader.GetString(0), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal)
                    : reader.GetDateTime(0).ToUniversalTime(),
                StringValue = reader.IsDBNull(2) ? null : reader.GetString(2)
            };
            if (!reader.IsDBNull(1)) point.Value = reader.GetDouble(1);
            else if (double.TryParse(point.StringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                point.Value = number;
            yield return point;
        }
    }
}
