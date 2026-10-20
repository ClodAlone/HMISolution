// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Reflection;
using Microsoft.Data.Sqlite;
using RuntimeViewer.Shared.Services;
using SharedModels;
using Xunit;

namespace Tests.RuntimeViewer;

public class EventHistoryTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "event_history_" + Guid.NewGuid().ToString("N"));
    private string DbPath => Path.Combine(_directory, "events.db");

    public EventHistoryTests() => Directory.CreateDirectory(_directory);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Reader_UsesStoredValueAndSupportsLegacyHistory(bool hasValueColumn)
    {
        using (var connection = new SqliteConnection($"Data Source={DbPath};Pooling=False"))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"""
                CREATE TABLE server_events (
                    id INTEGER PRIMARY KEY AUTOINCREMENT, time TEXT NOT NULL,
                    category TEXT NOT NULL, severity TEXT NOT NULL, source TEXT NOT NULL,
                    message TEXT NOT NULL, details TEXT{(hasValueColumn ? ", value TEXT" : "")});
                INSERT INTO server_events (time, category, severity, source, message, details{(hasValueColumn ? ", value" : "")})
                VALUES ('2026-01-01T00:00:00.0000000Z', 'Alarm', 'Warning', 'Temperature', 'High', 'Value=unrelated'{(hasValueColumn ? ", '85.12345678901234'" : "")});
                """;
            command.ExecuteNonQuery();
        }

        var project = new ProjectService();
        var field = typeof(ProjectService).GetField("_model", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        field.SetValue(project, new NodeModel
        {
            Server = new ServerSettings { EventLog = new EventLogConfig { DbPath = DbPath } }
        });
        var reader = new EventLogReaderService(project);

        var entry = Assert.Single(await reader.ReadEventsAsync(0, 10, "Alarm"));

        Assert.Equal("Temperature", entry.Source);
        Assert.Equal("High", entry.Message);
        Assert.Equal("Value=unrelated", entry.Details);
        Assert.Equal(hasValueColumn ? "85.12345678901234" : null, entry.Value);
    }

    [Fact]
    public void Exports_IncludeExactSnapshotAndEscapeText()
    {
        var service = new DataExportService();
        var events = new List<EventLogEntry>
        {
            new() { Category = "Alarm", Source = "Temperature", Message = "High", Value = "85.12345678901234" },
            new() { Category = "Alarm", Source = "Status", Message = "Condition met", Value = "fault, \"sensor\" <offline>" },
            new() { Category = "Alarm", Source = "Legacy", Message = "Old alarm" }
        };

        var csv = service.EventsToCsv(events);
        Assert.StartsWith("Timestamp,Category,Severity,Source,Message,Details,Value at event", csv);
        Assert.Contains(",Temperature,High,,85.12345678901234", csv);
        Assert.Contains("\"fault, \"\"sensor\"\" <offline>\"", csv);
        Assert.Contains(",Legacy,Old alarm,," + Environment.NewLine, csv);

        var html = service.EventsToHtml(events);
        Assert.Contains("<th>Value at event</th>", html);
        Assert.Contains("<td>85.12345678901234</td>", html);
        Assert.Contains("&lt;offline&gt;", html);
        Assert.DoesNotContain("<offline>", html);
        Assert.Contains("<td>Legacy</td><td></td><td>Old alarm</td>", html);
    }

    public void Dispose()
    {
        using var connection = new SqliteConnection($"Data Source={DbPath};Mode=ReadOnly");
        SqliteConnection.ClearPool(connection);
        Directory.Delete(_directory, true);
    }
}
