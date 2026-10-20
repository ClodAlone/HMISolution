// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RuntimeViewer.Shared.Components.Viewer;
using RuntimeViewer.Shared.Services;
using SharedModels;
using Xunit;

namespace Tests.RuntimeViewer;

public class AlarmAnalyticsDashboardTests
{
    [Fact]
    public async Task Dashboard_ShowsVariableAggregateAndIgnoresLegacyEventMessage()
    {
        var snapshot = new AlarmAnalyticsSnapshot
        {
            TotalActivations = 4,
            TotalAcknowledgements = 1,
            WindowMinutes = 60,
            TopAlarms =
            [
                new AlarmFrequencyEntry
                {
                    VariablePath = "Process.Temperature",
                    Message = "High limit (80) exceeded - value=90.12345",
                    ActivationCount = 4,
                    AvgAcknowledgeSeconds = 12
                }
            ]
        };
        using var services = new ServiceCollection().AddLogging()
            .AddSingleton<OpcRuntimeClient>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var result = await renderer.RenderComponentAsync<SnapshotDashboard>(ParameterView.FromDictionary(
                new Dictionary<string, object?> { [nameof(SnapshotDashboard.Snapshot)] = snapshot }));
            return result.ToHtmlString();
        });

        Assert.Contains("Alarm Sources by Variable", html);
        Assert.Contains("Aggregated across all alarm conditions for each variable", html);
        Assert.Contains("Process.Temperature", html);
        Assert.Contains(">4</td>", html);
        Assert.Contains(">12s</td>", html);
        Assert.DoesNotContain("<th>Message</th>", html);
        Assert.DoesNotContain("High limit", html);
        Assert.DoesNotContain("90.12345", html);
    }

    public sealed class SnapshotDashboard : AlarmAnalyticsDashboardWidget
    {
        [Parameter] public AlarmAnalyticsSnapshot Snapshot { get; set; } = new();

        protected override Task OnInitializedAsync() => Task.CompletedTask;

        protected override void OnParametersSet()
        {
            var field = typeof(AlarmAnalyticsDashboardWidget).GetField("_snapshot", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(field);
            field.SetValue(this, Snapshot);
        }
    }
}
