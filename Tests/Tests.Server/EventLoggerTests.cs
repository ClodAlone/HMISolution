// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Globalization;
using Microsoft.Data.Sqlite;
using SimpleOpcFileServer;
using Xunit;

namespace Tests.Server;

public class EventLoggerTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "alarm_events_" + Guid.NewGuid().ToString("N"));
    private string DbPath => Path.Combine(_directory, "events.db");

    public EventLoggerTests() => Directory.CreateDirectory(_directory);

    [Fact]
    public async Task LogAlarm_PreservesEachSnapshotAfterLaterValuesAndRestart()
    {
        var before = DateTime.UtcNow;
        using (var logger = new EventLogger(DbPath, 0))
        {
            logger.LogAlarm("Warning", "Temperature", "High", value: "85.12345678901234");
            logger.LogAlarm("Critical", "Temperature", "HighHigh", value: "100.98765432109876");
            logger.LogAlarm("Info", "Temperature", "Returned to normal", value: "50");
            logger.LogAlarm("Warning", "Unrelated", "High", value: "999");
            await WaitForEventsAsync(logger, 4);
        }

        using var reopened = new EventLogger(DbPath, 0);
        var rows = reopened.QueryEvents(category: "Alarm");
        Assert.Equal(4, rows.Count);
        var high = Assert.Single(rows, r => Equals(r["message"], "High") && Equals(r["source"], "Temperature"));
        Assert.Equal("85.12345678901234", high["value"]);
        Assert.Equal("100.98765432109876", Assert.Single(rows, r => Equals(r["message"], "HighHigh"))["value"]);
        Assert.Equal("50", Assert.Single(rows, r => Equals(r["message"], "Returned to normal"))["value"]);
        Assert.Equal("999", Assert.Single(rows, r => Equals(r["source"], "Unrelated"))["value"]);
        var time = DateTime.Parse((string)high["time"], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        Assert.InRange(time, before, DateTime.UtcNow);
    }

    [Theory]
    [InlineData("True")]
    [InlineData("False")]
    [InlineData("fault, \"sensor\" <offline>")]
    [InlineData("")]
    [InlineData("null")]
    public async Task LogAlarm_PreservesConditionValuesAndReplication(string value)
    {
        using var logger = new EventLogger(DbPath, 0);
        string? replicatedValue = null;
        logger.OnEventLogged = (_, _, _, _, _, snapshot) => replicatedValue = snapshot;

        logger.LogAlarm("Warning", "Condition", "Condition met", value: value);
        await WaitForEventsAsync(logger, 1);

        Assert.Equal(value, replicatedValue);
        Assert.Equal(value, Assert.Single(logger.QueryEvents())["value"]);
        var entry = Assert.Single(logger.ReadEventsSince(DateTime.MinValue));
        Assert.Equal(value, entry.Value);

        var partnerPath = Path.Combine(_directory, "partner.db");
        using var partner = new EventLogger(partnerPath, 0);
        partner.ReplayEvents([entry]);
        var replayed = Assert.Single(partner.ReadEventsSince(DateTime.MinValue));
        Assert.Equal(entry.Time, replayed.Time);
        Assert.Equal(entry.Source, replayed.Source);
        Assert.Equal(entry.Message, replayed.Message);
        Assert.Equal(value, replayed.Value);
    }

    [Fact]
    public async Task ExistingDatabase_IsMigratedWithoutBackfillingOldValues()
    {
        using (var connection = new SqliteConnection($"Data Source={DbPath};Pooling=False"))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE server_events (
                    id INTEGER PRIMARY KEY AUTOINCREMENT, time TEXT NOT NULL,
                    category TEXT NOT NULL, severity TEXT NOT NULL, source TEXT NOT NULL,
                    message TEXT NOT NULL, details TEXT);
                INSERT INTO server_events (time, category, severity, source, message)
                VALUES ('2026-01-01T00:00:00.0000000Z', 'Alarm', 'Warning', 'Temperature', 'Legacy alarm');
                """;
            command.ExecuteNonQuery();
        }

        using (var logger = new EventLogger(DbPath, 0))
        {
            var old = Assert.Single(logger.ReadEventsSince(DateTime.MinValue));
            Assert.Null(old.Value);
            Assert.Equal("Legacy alarm", old.Message);
            logger.LogAlarm("Critical", "Temperature", "New alarm", value: "95.125");
            await WaitForEventsAsync(logger, 2);
        }

        using var reopened = new EventLogger(DbPath, 0);
        var events = reopened.ReadEventsSince(DateTime.MinValue);
        Assert.Equal(2, events.Count);
        Assert.Null(Assert.Single(events, e => e.Message == "Legacy alarm").Value);
        Assert.Equal("95.125", Assert.Single(events, e => e.Message == "New alarm").Value);
    }

    private static async Task WaitForEventsAsync(EventLogger logger, int count)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (logger.QueryEvents().Count < count)
            await Task.Delay(10, timeout.Token);
        Assert.Equal(count, logger.QueryEvents().Count);
    }

    public void Dispose()
    {
        foreach (var name in new[] { "events.db", "partner.db" })
        {
            using var connection = new SqliteConnection($"Data Source={Path.Combine(_directory, name)}");
            SqliteConnection.ClearPool(connection);
        }
        Directory.Delete(_directory, true);
    }
}
