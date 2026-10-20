// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Reflection;
using Microsoft.Data.Sqlite;
using ServerEditorWeb.Components.Editor;
using Xunit;

namespace Tests.Editor;

public class EventHistoryTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "editor_events_" + Guid.NewGuid().ToString("N"));
    private string DbPath => Path.Combine(_directory, "events.db");

    public EventHistoryTests() => Directory.CreateDirectory(_directory);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void History_ReadsCapturedValueWithoutUsingMessageOrDetails(bool hasValueColumn)
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
                VALUES ('2026-01-01T00:00:00.0000000Z', 'Alarm', 'Warning', 'Temperature', 'High', 'Value=999'{(hasValueColumn ? ", '85.12345678901234'" : "")});
                """;
            command.ExecuteNonQuery();
        }

        var panel = new EventsDataLoggingPanel();
        var clearFilters = typeof(EventsDataLoggingPanel).GetMethod("ClearFilters", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(clearFilters);
        clearFilters.Invoke(panel, null);
        var read = typeof(EventsDataLoggingPanel).GetMethod("ReadEventsFromDb", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(read);
        object?[] args = [DbPath, 0];

        var rows = Assert.IsType<List<EventsDataLoggingPanel.EventLogEntry>>(read.Invoke(panel, args));

        Assert.Equal(1, args[1]);
        var entry = Assert.Single(rows);
        Assert.Equal("Temperature", entry.Source);
        Assert.Equal("Value=999", entry.Details);
        Assert.Equal(hasValueColumn ? "85.12345678901234" : null, entry.Value);
    }

    public void Dispose()
    {
        using var connection = new SqliteConnection($"Data Source={DbPath};Mode=ReadOnly");
        SqliteConnection.ClearPool(connection);
        Directory.Delete(_directory, true);
    }
}
