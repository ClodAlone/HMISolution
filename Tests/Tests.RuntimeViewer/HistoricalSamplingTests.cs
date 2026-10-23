using RuntimeViewer.Shared.Services;
using Xunit;

namespace Tests.RuntimeViewer;

public class HistoricalSamplingTests
{
    private static readonly DateTime Start = new(2026, 10, 23, 8, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Timeline_IncludesAlignedEndAndDoesNotExceedUnalignedEnd()
    {
        Assert.Equal(new[] { Start, Start.AddSeconds(10), Start.AddSeconds(20) }, HistoricalSampling.Timeline(Start, Start.AddSeconds(20), 10));
        Assert.Equal(new[] { Start, Start.AddSeconds(10), Start.AddSeconds(20) }, HistoricalSampling.Timeline(Start, Start.AddSeconds(29), 10));
    }

    [Theory]
    [InlineData(0, 60)]
    [InlineData(-1, 60)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(2000, 1)]
    public void Timeline_RejectsInvalidOrOversizedQueries(int seconds, int interval)
    {
        Assert.Throws<ArgumentException>(() => HistoricalSampling.Timeline(Start, Start.AddSeconds(seconds), interval));
    }

    [Fact]
    public void Timeline_AcceptsExactlyTheLimitAndRejectsNonUtc()
    {
        var times = HistoricalSampling.Timeline(Start, Start.AddSeconds(1999), 1);
        Assert.Equal(2000, times.Count);
        Assert.Equal(Start.AddSeconds(1999), times[^1]);
        Assert.Throws<ArgumentException>(() => HistoricalSampling.Timeline(DateTime.SpecifyKind(Start, DateTimeKind.Unspecified), Start.AddMinutes(1), 1));
    }

    [Fact]
    public void Samples_HoldPredecessorAndUseLatestChangeAtOrBeforeEachTimestamp()
    {
        var times = HistoricalSampling.Timeline(Start, Start.AddSeconds(30), 10);
        var result = HistoricalSampling.Sample("Speed", new[]
        {
            Point(-100, 5), Point(3, 7), Point(4, 8), Point(20, 10), Point(31, 99)
        }, times);
        Assert.Equal(times, result.Points.Select(p => p.Time));
        Assert.Equal(new double?[] { 5, 8, 10, 10 }, result.Points.Select(p => p.Value));
        Assert.Equal(5, result.MinValue);
        Assert.Equal(10, result.MaxValue);
    }

    [Fact]
    public void Samples_NeverBackfillFromFutureAndPreserveStringsAndNullChanges()
    {
        var times = HistoricalSampling.Timeline(Start, Start.AddSeconds(30), 10);
        var result = HistoricalSampling.Sample("State", new[]
        {
            new HdaDataPoint { Time = Start.AddSeconds(5), StringValue = "Idle" },
            new HdaDataPoint { Time = Start.AddSeconds(20) }
        }, times);
        Assert.Equal(new string?[] { null, "Idle", null, null }, result.Points.Select(p => p.StringValue));
        Assert.All(result.Points, p => Assert.Null(p.Value));
        Assert.Equal("Idle", HistoricalSampling.Format(result.Points[1]));
        Assert.Equal("—", HistoricalSampling.Format(result.Points[0]));
    }

    [Fact]
    public void ConstantSeries_ProducesAnIdenticalValueAtEverySampleWithoutNewRecords()
    {
        var times = HistoricalSampling.Timeline(Start, Start.AddHours(1), 60);
        var constant = HistoricalSampling.Sample("Constant", new[] { Point(-1000, 42) }, times);
        var unknown = HistoricalSampling.Sample("Unknown", Array.Empty<HdaDataPoint>(), times);
        Assert.Equal(61, constant.Points.Count);
        Assert.All(constant.Points, p => Assert.Equal(42, p.Value));
        Assert.Equal(constant.Points.Select(p => p.Time), unknown.Points.Select(p => p.Time));
        Assert.All(unknown.Points, p => Assert.Null(p.Value));
    }

    [Fact]
    public void CursorGroups_RetainRangeAndCursorWithoutCrossingScreensOrGroups()
    {
        var screen = new HistoricalCursorGroup();
        var otherScreen = new HistoricalCursorGroup();
        var selection = new HistoricalSelection(Start, Start.AddHours(1), 60, Start.AddMinutes(12));
        var received = new List<(string, HistoricalSelection)>();
        screen.Changed += (group, value) => received.Add((group, value));
        screen.Publish("mixer", selection);
        Assert.Equal(new[] { ("mixer", selection) }, received);
        Assert.Equal(selection, screen.Get("mixer"));
        Assert.Null(screen.Get("other"));
        Assert.Null(otherScreen.Get("mixer"));
        screen.Publish("", selection);
        Assert.Single(received);
        Assert.Null(screen.Get(""));
    }

    [Theory]
    [InlineData("historicaltrend")]
    [InlineData("historicaltable")]
    public void SymbolSettings_RoundTripWithoutChangingLegacyDefaults(string type)
    {
        var symbol = new SharedModels.ScreenSymbol
        {
            Type = type, HdaVariablePaths = ["Mixer.Speed", "Mixer.Level"], HdaTimeRangeMinutes = 360,
            HistoricalSamplingIntervalSeconds = 15, HistoricalSyncGroup = "mixer", WidgetTheme = "ivory"
        };
        var copy = System.Text.Json.JsonSerializer.Deserialize<SharedModels.ScreenSymbol>(System.Text.Json.JsonSerializer.Serialize(symbol))!;
        Assert.Equal(type, copy.Type);
        Assert.Equal(new[] { "Mixer.Speed", "Mixer.Level" }, copy.HdaVariablePaths);
        Assert.Equal(360, copy.HdaTimeRangeMinutes);
        Assert.Equal(15, copy.HistoricalSamplingIntervalSeconds);
        Assert.Equal("mixer", copy.HistoricalSyncGroup);
        Assert.Equal("ivory", copy.WidgetTheme);
        var legacy = System.Text.Json.JsonSerializer.Deserialize<SharedModels.ScreenSymbol>("{\"Type\":\"hdachart\"}")!;
        Assert.Equal(60, legacy.HistoricalSamplingIntervalSeconds);
        Assert.Equal("", legacy.HistoricalSyncGroup);
        Assert.Equal(500, legacy.HdaMaxPoints);
        Assert.Equal("classic", legacy.WidgetTheme);
    }

    private static HdaDataPoint Point(int seconds, double value) => new() { Time = Start.AddSeconds(seconds), Value = value };
}
