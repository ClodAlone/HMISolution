using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using RuntimeViewer.Shared.Components.Viewer;
using RuntimeViewer.Shared.Services;
using SharedModels;
using Xunit;

namespace Tests.RuntimeViewer;

public class RealtimeTrendWidgetTests
{
    private static readonly DateTime ChartTime = new(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(0.5, 120, 80)]
    [InlineData(1, 0, 0)]
    [InlineData(2, 350, 200)]
    public async Task MouseMove_UsesClientCoordinatesAndSvgTransform(double scale, double left, double top)
    {
        var module = new CoordinateModule(scale, left, top);
        await using var widget = new RealtimeTrendWidget();
        SetProperty(widget, "JS", new CoordinateRuntime(module));
        await Move(widget, left + 428.5 * scale, top + 100 * scale);
        Assert.Equal(428.5, GetField<double?>(widget, "_cursorX"));
        Assert.Equal("clientToSvg", module.LastIdentifier);
        Assert.IsType<ElementReference>(module.LastArguments![0]);
        Assert.Equal(left + 428.5 * scale, module.LastArguments[1]);
        Assert.Equal(top + 100 * scale, module.LastArguments[2]);
    }

    [Theory]
    [InlineData(54, 100, false)]
    [InlineData(55, 8, true)]
    [InlineData(792, 260, true)]
    [InlineData(793, 100, false)]
    [InlineData(400, 7, false)]
    [InlineData(400, 261, false)]
    public async Task MouseMove_SelectsOnlyInsidePlot(double x, double y, bool selected)
    {
        await using var widget = new RealtimeTrendWidget();
        SetProperty(widget, "JS", new CoordinateRuntime(new CoordinateModule(1, 0, 0)));
        await Move(widget, x, y);
        Assert.Equal(selected ? x : (double?)null, GetField<double?>(widget, "_cursorX"));
    }

    [Fact]
    public async Task MouseLeave_InvalidatesPendingCoordinateConversion()
    {
        var module = new CoordinateModule(1, 0, 0) { Pending = new(TaskCreationOptions.RunContinuationsAsynchronously) };
        await using var widget = new RealtimeTrendWidget();
        SetProperty(widget, "JS", new CoordinateRuntime(module));
        var move = Move(widget, 400, 100);
        Invoke(widget, "OnChartMouseLeave", new MouseEventArgs());
        module.Pending.SetResult((400, 100));
        await move;
        Assert.Null(GetField<double?>(widget, "_cursorX"));
    }

    [Fact]
    public async Task LaterMouseMove_WinsOverEarlierPendingConversion()
    {
        var pending = new TaskCompletionSource<(double X, double Y)>(TaskCreationOptions.RunContinuationsAsynchronously);
        var module = new CoordinateModule(1, 0, 0) { Pending = pending };
        await using var widget = new RealtimeTrendWidget();
        SetProperty(widget, "JS", new CoordinateRuntime(module));
        var first = Move(widget, 200, 100);
        module.Pending = null;
        await Move(widget, 600, 100);
        pending.SetResult((200, 100));
        await first;
        Assert.Equal(600, GetField<double?>(widget, "_cursorX"));
    }

    [Theory]
    [InlineData(false, 55, -60)]
    [InlineData(false, 423.5, -30)]
    [InlineData(false, 792, 0)]
    [InlineData(true, 400, -30)]
    [InlineData(true, 745, 0)]
    public async Task CursorTime_UsesSameTimeOriginAndPlotWidthAsCurves(bool dualAxes, double x, int seconds)
    {
        await using var widget = new RealtimeTrendWidget();
        SetField(widget, "_hasDualAxes", dualAxes);
        SetField(widget, "_yMin", 0d);
        SetField(widget, "_yMax", 100d);
        var samples = Samples((ChartTime.AddSeconds(-60), 0), (ChartTime, 100));
        var time = (DateTime)Invoke(widget, "XToTime", x, ChartTime)!;
        Assert.Equal(ChartTime.AddSeconds(seconds), time);
        var value = (double)Invoke(widget, "InterpolateAtTime", samples, time)!;
        Assert.Equal((seconds + 60) / 60d * 100, value, 10);
        var points = (string)Invoke(widget, "BuildPolyline", samples, 0, ChartTime)!;
        Assert.Equal(dualAxes ? "55.0,260.0 745.0,8.0" : "55.0,260.0 792.0,8.0", points);
    }

    [Fact]
    public async Task CurveCrossingWindowStart_MatchesInterpolatedCursorAtLeftEdge()
    {
        await using var widget = new RealtimeTrendWidget();
        SetField(widget, "_yMin", 0d);
        SetField(widget, "_yMax", 100d);
        var samples = Samples((ChartTime.AddSeconds(-90), 0), (ChartTime.AddSeconds(-30), 60), (ChartTime, 90));
        var points = (string)Invoke(widget, "BuildPolyline", samples, 0, ChartTime)!;
        Assert.Equal("55.0,184.4 423.5,108.8 792.0,33.2", points);
        var time = (DateTime)Invoke(widget, "XToTime", 55d, ChartTime)!;
        Assert.Equal(30d, (double)Invoke(widget, "InterpolateAtTime", samples, time)!);
        Assert.Equal(184.4, (double)Invoke(widget, "ValueToY", 30d, 0)!, 10);
    }

    [Theory]
    [InlineData(-46)]
    [InlineData(-14)]
    public async Task CursorOutsideSampleRange_DoesNotInventCurveReadings(int seconds)
    {
        await using var widget = new RealtimeTrendWidget();
        var samples = Samples((ChartTime.AddSeconds(-45), 10), (ChartTime.AddSeconds(-15), 70));
        Assert.Null(Invoke(widget, "InterpolateAtTime", samples, ChartTime.AddSeconds(seconds)));
        Assert.Null(Invoke(widget, "InterpolateAtTime", Samples((ChartTime, 10)), ChartTime));
        Assert.Null(Invoke(widget, "InterpolateAtTime", Samples(), ChartTime));
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Render_CrosshairAndLegendFollowEachCurveOnBothAxes(bool cursorActive, bool showCursor)
    {
        await using var services = new ServiceCollection().AddLogging()
            .AddSingleton<OpcRuntimeClient>()
            .AddSingleton<IJSRuntime>(new CoordinateRuntime(new CoordinateModule(1, 0, 0)))
            .BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<SeededTrend>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(RealtimeTrendWidget.Pens)] = new List<TrendPen>
                {
                    new() { Label = "Left", YAxisId = 0 }, new() { Label = "Right", YAxisId = 1 }
                },
                [nameof(RealtimeTrendWidget.ShowCursor)] = showCursor,
                [nameof(SeededTrend.CursorActive)] = cursorActive
            }))).ToHtmlString());

        if (cursorActive && showCursor)
        {
            var curves = Regex.Matches(html, "<polyline points=\"([^\"]+)\"");
            var markers = Regex.Matches(html, "<circle cx=\"([^\"]+)\" cy=\"([^\"]+)\"");
            Assert.Equal(2, curves.Count);
            Assert.Equal(2, markers.Count);
            for (int i = 0; i < 2; i++)
            {
                var points = curves[i].Groups[1].Value.Split(' ').Select(p => p.Split(',').Select(Parse).ToArray()).ToArray();
                var cx = Parse(markers[i].Groups[1].Value);
                var cy = Parse(markers[i].Groups[2].Value);
                var expectedY = points[0][1] + (cx - points[0][0]) / (points[1][0] - points[0][0]) * (points[1][1] - points[0][1]);
                Assert.Equal(400, cx);
                Assert.InRange(Math.Abs(expectedY - cy), 0, 0.2);
            }
            var left = Regex.Match(html, "Left: ([0-9.]+)").Groups[1].Value;
            var right = Regex.Match(html, "Right: ([0-9.]+)").Groups[1].Value;
            var leftValue = (260 - Parse(markers[0].Groups[2].Value)) / 252 * 100;
            var rightValue = (260 - Parse(markers[1].Groups[2].Value)) / 252 * 1000;
            Assert.InRange(Math.Abs(Parse(left) - leftValue), 0, 0.1);
            Assert.InRange(Math.Abs(Parse(right) - rightValue), 0, 0.3);
            Assert.Contains($"= {left}</span>", html);
            Assert.Contains($"= {right}</span>", html);
        }
        else
        {
            Assert.DoesNotContain("<circle", html);
            Assert.Contains("= 70.0</span>", html);
            Assert.Contains("= 700.0</span>", html);
        }
    }

    public sealed class SeededTrend : RealtimeTrendWidget
    {
        [Parameter] public bool CursorActive { get; set; }
        protected override void OnInitialized()
        {
            var now = DateTime.UtcNow;
            var left = Samples((now.AddSeconds(-45), 10), (now.AddSeconds(-15), 70));
            var right = Samples((now.AddSeconds(-45), 100), (now.AddSeconds(-15), 700));
            var buffers = Array.CreateInstance(left.GetType(), 2);
            buffers.SetValue(left, 0);
            buffers.SetValue(right, 1);
            SetField(this, "_buffers", buffers);
            SetField(this, "_hasDualAxes", true);
            SetField(this, "_yMin", 0d);
            SetField(this, "_yMax", 100d);
            SetField(this, "_yMinRight", 0d);
            SetField(this, "_yMaxRight", 1000d);
            if (CursorActive) SetField(this, "_cursorX", 400d);
        }
    }

    private static double Parse(string value) => double.Parse(value, CultureInfo.InvariantCulture);
    private static Task Move(RealtimeTrendWidget widget, double x, double y) =>
        (Task)Invoke(widget, "OnChartMouseMove", new MouseEventArgs { ClientX = x, ClientY = y, OffsetX = 1, OffsetY = 1 })!;
    private static object? Invoke(RealtimeTrendWidget widget, string method, params object[] args) =>
        typeof(RealtimeTrendWidget).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(widget, args);
    private static void SetProperty(RealtimeTrendWidget widget, string name, object value) =>
        typeof(RealtimeTrendWidget).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(widget, value);
    private static void SetField(RealtimeTrendWidget widget, string name, object value) =>
        typeof(RealtimeTrendWidget).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(widget, value);
    private static T GetField<T>(RealtimeTrendWidget widget, string name) =>
        (T)typeof(RealtimeTrendWidget).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(widget)!;
    private static Array Samples(params (DateTime Time, double Value)[] samples)
    {
        var type = typeof(RealtimeTrendWidget).GetNestedType("Sample", BindingFlags.NonPublic)!;
        var result = Array.CreateInstance(type, samples.Length);
        for (int i = 0; i < samples.Length; i++)
            result.SetValue(Activator.CreateInstance(type, samples[i].Time, samples[i].Value), i);
        return result;
    }

    private sealed class CoordinateRuntime(IJSObjectReference module) : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Assert.Equal("import", identifier);
            Assert.Equal("./_content/RuntimeViewer.Shared/screenrenderer.js", Assert.Single(args!));
            return ValueTask.FromResult((TValue)module);
        }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
    }

    private sealed class CoordinateModule(double scale, double left, double top) : IJSObjectReference
    {
        public string? LastIdentifier { get; private set; }
        public object?[]? LastArguments { get; private set; }
        public TaskCompletionSource<(double X, double Y)>? Pending { get; set; }
        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            LastIdentifier = identifier;
            LastArguments = args;
            var point = Pending is { } pending ? await pending.Task
                : (((double)args![1]! - left) / scale, ((double)args[2]! - top) / scale);
            return (TValue)Activator.CreateInstance(typeof(TValue), point.Item1, point.Item2)!;
        }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
