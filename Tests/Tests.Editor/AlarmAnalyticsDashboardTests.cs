// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Reflection;
using ServerEditorWeb.Components.Editor;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class AlarmAnalyticsDashboardTests
{
    [Theory]
    [InlineData("", 2)]
    [InlineData("temperature", 1)]
    [InlineData("PRESSURE", 1)]
    [InlineData("High limit", 0)]
    [InlineData("90.12345", 0)]
    public void Filter_UsesVariableIdentityNotLegacyEventDetails(string search, int expectedCount)
    {
        using var panel = new AlarmAnalyticsDashboardPanel();
        var snapshot = new AlarmAnalyticsSnapshot
        {
            TopAlarms =
            [
                new AlarmFrequencyEntry
                {
                    VariablePath = "Process.Temperature", Message = "High limit exceeded - value=90.12345", ActivationCount = 4
                },
                new AlarmFrequencyEntry
                {
                    VariablePath = "Process.Pressure", Message = "Low limit violated - value=20", ActivationCount = 2
                }
            ]
        };
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var snapshotField = typeof(AlarmAnalyticsDashboardPanel).GetField("_snapshot", flags);
        var searchField = typeof(AlarmAnalyticsDashboardPanel).GetField("_alarmSearch", flags);
        var filteredProperty = typeof(AlarmAnalyticsDashboardPanel).GetProperty("FilteredAlarms", flags);
        Assert.NotNull(snapshotField);
        Assert.NotNull(searchField);
        Assert.NotNull(filteredProperty);
        snapshotField.SetValue(panel, snapshot);
        searchField.SetValue(panel, search);

        var rows = Assert.IsType<List<AlarmFrequencyEntry>>(filteredProperty.GetValue(panel));

        Assert.Equal(expectedCount, rows.Count);
        if (expectedCount == 1)
        {
            var row = Assert.Single(rows);
            Assert.Contains(search, row.VariablePath, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(search.Equals("temperature", StringComparison.OrdinalIgnoreCase) ? 4 : 2, row.ActivationCount);
        }
    }
}
