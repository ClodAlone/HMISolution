using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using RuntimeViewer.Shared.Components.Viewer;
using RuntimeViewer.Shared.Services;
using SharedModels;
using Xunit;

namespace Tests.RuntimeViewer;

public class WidgetBindingTests
{
    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task AuxiliaryBindings_RegisterBeforeConnection_ResolveAliasesAndReleaseOldPaths(string layout)
    {
        ScreenSymbol[] symbols =
        [
            new() { Type = "heatexchanger", HeatExchangerHotInPath = "{Unit}.Hot", HeatExchangerColdInPath = "{Unit}.Cold" },
            new() { Type = "flowmeter", VariablePath = "{Unit}.Flow", FlowMeterTotalizerPath = "{Unit}.Total" },
            new() { Type = "piechart", PieChartSlices = "Good parts = {Unit}.Good #22c55e;Rejects={Unit}.Rejects #ef4444" },
            new() { Type = "barchart", BarChartBars = "Kp={Unit}.Kp #3b82f6;Ki={Unit}.Ki #22c55e" },
            new() { Type = "datatable", DataTableColumns = "Hot={Unit}.Hot,Cold={Unit}.Cold" },
            new() { Type = "datatable", HdaVariablePaths = ["{Unit}.Legacy"] },
            new() { Type = "xyplot", XYPlotXPath = "{Unit}.X", XYPlotYPath = "{Unit}.Y" },
            new() { Type = "setpointramp", SetpointTargetPath = "{Unit}.Target", SetpointRatePath = "{Unit}.Rate" },
            new() { Type = "popup", PopupVariables = " {Unit}.Status ; {Unit}.Hot " },
            new() { Type = "motorcontrol", MotorFaultPath = "{Unit}.MotorFault" },
            new() { Type = "conveyor", ConveyorFaultPath = "{Unit}.BeltFault" },
            new() { Type = "rangeslider", RangeLowPath = "{Unit}.Low", RangeHighPath = "{Unit}.High" },
            new() { Type = "kpiwidget", KpiAvailabilityPath = "{Unit}.Availability", KpiPerformancePath = "{Unit}.Performance",
                KpiQualityPath = "{Unit}.Quality", KpiUptimePath = "{Unit}.Uptime", KpiThroughputPath = "{Unit}.Throughput",
                KpiThroughputTargetPath = "{Unit}.ThroughputTarget" }
        ];
        for (var i = 0; i < symbols.Length; i++)
        {
            symbols[i].Id = "widget-" + i;
            symbols[i].InstanceAliases = new() { ["Unit"] = "{Plant}.UnitA" };
        }
        await using var session = await RenderSession.StartAsync(symbols, layout);
        string[] names = ["Hot", "Cold", "Flow", "Total", "Good", "Rejects", "Kp", "Ki", "Legacy", "X", "Y",
            "Target", "Rate", "Status", "MotorFault", "BeltFault", "Low", "High", "Availability", "Performance",
            "Quality", "Uptime", "Throughput", "ThroughputTarget"];
        Assert.False(session.Opc.IsConnected);
        Assert.Equal(names.Select(name => "Plant1.UnitA." + name).Order(), session.RegisteredPaths.Order());
        session.Aliases["Plant"] = "Plant2";
        await session.RefreshAsync();
        Assert.Equal(names.Select(name => "Plant2.UnitA." + name).Order(), session.RegisteredPaths.Order());
        await session.ClearScreenAsync();
        Assert.Empty(session.RegisteredPaths);
        await session.DisposeAsync();
        Assert.Empty(session.RegisteredPaths);
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task HeatExchanger_UpdatesSecondaryTemperatureWithoutPrimaryVariable(string layout)
    {
        await using var session = await RenderSession.StartAsync([Symbol("heatexchanger", s =>
        {
            s.HeatExchangerHotInPath = "{Plant}.Hot";
            s.HeatExchangerColdInPath = "{Plant}.Cold";
        })], layout, new() { ["Plant1.Hot"] = "83.5", ["Plant1.Cold"] = "17.25" });
        Assert.Contains("83.5", await session.HtmlAsync());
        Assert.Contains("17.25", await session.HtmlAsync());
        await session.UpdateAsync("Plant1.Cold", "19.75");
        var html = await session.HtmlAsync();
        Assert.Contains("83.5", html);
        Assert.Contains("19.75", html);
        Assert.DoesNotContain("17.25", html);
    }

    [Theory]
    [InlineData("svg", "circular")]
    [InlineData("svg", "digital")]
    [InlineData("responsive", "circular")]
    [InlineData("responsive", "digital")]
    public async Task FlowMeter_UpdatesTotalizerIndependentlyOfFlow(string layout, string style)
    {
        await using var session = await RenderSession.StartAsync([Symbol("flowmeter", s =>
        {
            s.VariablePath = "{Plant}.Flow";
            s.FlowMeterTotalizerPath = "{Plant}.Total";
            s.FlowMeterStyle = style;
        })], layout, new() { ["Plant1.Flow"] = "12.5" });
        Assert.Contains("--", await session.HtmlAsync());
        await session.UpdateAsync("Plant1.Total", "9876");
        var html = await session.HtmlAsync();
        Assert.Contains("12.5", html);
        Assert.Contains("9876", html);
        Assert.Contains("L/min", html);
        await session.UpdateAsync("Plant1.Total", "12345");
        html = await session.HtmlAsync();
        Assert.Contains("12.5", html);
        Assert.Contains("12345", html);
        Assert.DoesNotContain("9876", html);
    }

    [Theory]
    [InlineData("svg", "vertical")]
    [InlineData("svg", "horizontal")]
    [InlineData("responsive", "vertical")]
    [InlineData("responsive", "horizontal")]
    public async Task PidBarChart_UsesTwentyUnitScaleAndUpdatesWithNoPrimaryVariable(string layout, string orientation)
    {
        await using var session = await RenderSession.StartAsync([Symbol("barchart", s =>
        {
            s.BarChartBars = "Kp=PID.Kp #3b82f6;Ki=PID.Ki #22c55e;Kd=PID.Kd #f59e0b";
            s.BarChartMax = 20;
            s.BarChartMaxValue = 100;
            s.BarChartOrientation = orientation;
        })], layout, new() { ["PID.Kp"] = "10", ["PID.Ki"] = "5", ["PID.Kd"] = "2" });
        var dimension = orientation == "vertical" ? "height" : "width";
        Assert.Equal(orientation == "vertical" ? 52 : 92, Number(ColoredElement(await session.HtmlAsync(), "rect", "#3b82f6"), dimension));
        await session.UpdateAsync("PID.Kp", "20");
        Assert.Equal(orientation == "vertical" ? 104 : 184, Number(ColoredElement(await session.HtmlAsync(), "rect", "#3b82f6"), dimension));
    }

    [Theory]
    [InlineData(100, 40, 26)]
    [InlineData(20, 40, 52)]
    [InlineData(0, 100, 104)]
    public async Task BarChart_PreservesLegacyMaximumAndAutomaticScaling(double maximum, double legacyMaximum, double height)
    {
        await using var session = await RenderSession.StartAsync([Symbol("barchart", s =>
        {
            s.BarChartBars = "Gain=PID.Kp #3b82f6";
            s.BarChartMax = maximum;
            s.BarChartMaxValue = legacyMaximum;
        })], "svg", new() { ["PID.Kp"] = "10" });
        Assert.Equal(height, Number(ColoredElement(await session.HtmlAsync(), "rect", "#3b82f6"), "height"));
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task PieChart_UpdatesSlicesAndDrawsSingleSliceAsFullCircle(string layout)
    {
        await using var session = await RenderSession.StartAsync([Symbol("piechart", s =>
        {
            s.PieChartSlices = "Good parts = {Plant}.Good #22c55e;Rejects={Plant}.Rejects #ef4444";
            s.PieChartStyle = "doughnut";
            s.PieChartInnerRadius = 0.7;
        })], layout, new() { ["Plant1.Good"] = "10", ["Plant1.Rejects"] = "10" });
        var firstArc = ColoredElement(await session.HtmlAsync(), "path", "#22c55e").Attribute("d")!.Value;
        await session.UpdateAsync("Plant1.Good", "30");
        var html = await session.HtmlAsync();
        Assert.NotEqual(firstArc, ColoredElement(html, "path", "#22c55e").Attribute("d")!.Value);
        Assert.Contains(">75%</text>", html);
        await session.UpdateAsync("Plant1.Rejects", "0");
        html = await session.HtmlAsync();
        Assert.Equal(50, Number(ColoredElement(html, "circle", "#22c55e"), "r"));
        Assert.Contains(">100%</text>", html);
        Assert.Contains(Elements(html, "circle"), circle => Number(circle, "r") == 35);
    }

    [Theory]
    [InlineData("svg", ",")]
    [InlineData("svg", ";")]
    [InlineData("responsive", ",")]
    [InlineData("responsive", ";")]
    public async Task DataTable_RendersConfiguredLiveColumnsAndRefreshesCells(string layout, string separator)
    {
        await using var session = await RenderSession.StartAsync([Symbol("datatable", s =>
            s.DataTableColumns = $"Temperature = {{Plant}}.Hot{separator}Pressure={{Plant}}.Pressure")],
            layout, new() { ["Plant1.Hot"] = "83.5", ["Plant1.Pressure"] = "4.25" });
        var html = await session.HtmlAsync();
        Assert.Equal(["Temperature", "Pressure"], Elements(html, "th").Select(cell => cell.Value));
        Assert.Equal(["83.5", "4.25"], Elements(html, "td").Select(cell => cell.Value));
        await session.UpdateAsync("Plant1.Pressure", "6.75");
        Assert.Equal(["83.5", "6.75"], Elements(await session.HtmlAsync(), "td").Select(cell => cell.Value));
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task DataTable_PreservesLegacyRowsRowLimitAndHiddenHeader(string layout)
    {
        await using var session = await RenderSession.StartAsync([Symbol("datatable", s =>
        {
            s.HdaVariablePaths = ["{Plant}/Hot", "{Plant}/Pressure"];
            s.DataTableMaxRows = 1;
            s.DataTableShowHeader = false;
        })], layout, new() { ["Plant1/Hot"] = "83.5", ["Plant1/Pressure"] = "4.25" });
        var html = await session.HtmlAsync();
        Assert.Empty(Elements(html, "th"));
        Assert.Equal(["Hot", "83.5"], Elements(html, "td").Select(cell => cell.Value));
        await session.UpdateAsync("Plant1/Hot", "84.5");
        Assert.Equal(["Hot", "84.5"], Elements(await session.HtmlAsync(), "td").Select(cell => cell.Value));
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task XYPlot_UsesAxisBindingsAndRetainsOnlyConfiguredNumberOfPoints(string layout)
    {
        await using var session = await RenderSession.StartAsync([Symbol("xyplot", s =>
        {
            s.VariablePath = "{Plant}.Unrelated";
            s.XYPlotXPath = "{Plant}.X";
            s.XYPlotYPath = "{Plant}.Y";
            s.XYPlotMaxPoints = 2;
            s.XYPlotStrokeWidth = 3;
        })], layout, new() { ["Plant1.Unrelated"] = "90", ["Plant1.X"] = "25", ["Plant1.Y"] = "75" });
        var marker = ColoredElement(await session.HtmlAsync(), "circle", "#3b82f6");
        Assert.Equal(65, Number(marker, "cx"));
        Assert.Equal(45, Number(marker, "cy"));
        await session.UpdateAsync("Plant1.Y", "25");
        await session.UpdateAsync("Plant1.X", "100");
        var trace = Assert.Single(Elements(await session.HtmlAsync(), "polyline"));
        Assert.Equal("65.0,75.0 170.0,75.0", (string?)trace.Attribute("points"));
        Assert.Equal(3, Number(trace, "stroke-width"));
        await session.RefreshAsync();
        Assert.Equal("65.0,75.0 170.0,75.0", (string?)Assert.Single(Elements(await session.HtmlAsync(), "polyline")).Attribute("points"));
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task XYPlot_MissingValuesDoNotBecomeZero_AndLegacyPrimaryXStillWorks(string layout)
    {
        await using var session = await RenderSession.StartAsync([Symbol("xyplot", s =>
        {
            s.VariablePath = "{Plant}.X";
            s.XYPlotYPath = "{Plant}.Y";
        })], layout);
        Assert.Empty(Elements(await session.HtmlAsync(), "circle"));
        await session.UpdateAsync("Plant1.X", "25");
        Assert.Empty(Elements(await session.HtmlAsync(), "circle"));
        await session.UpdateAsync("Plant1.Y", "75");
        var marker = Assert.Single(Elements(await session.HtmlAsync(), "circle"));
        Assert.Equal(65, Number(marker, "cx"));
        Assert.Equal(45, Number(marker, "cy"));
    }

    [Theory]
    [InlineData("svg", "popup", "31.25", "44.75")]
    [InlineData("responsive", "popup", "31.25", "44.75")]
    [InlineData("svg", "setpointramp", "31.2", "44.8")]
    [InlineData("responsive", "setpointramp", "31.2", "44.8")]
    [InlineData("svg", "kpiwidget", "31.2", "44.8")]
    [InlineData("responsive", "kpiwidget", "31.2", "44.8")]
    public async Task OtherAuxiliaryWidgets_UpdateWithoutPrimaryValueChanges(string layout, string type, string before, string after)
    {
        var symbol = Symbol(type, s =>
        {
            s.PopupVariables = "{Plant}.Value";
            s.SetpointTargetPath = "{Plant}.Value";
            s.KpiMode = "uptime";
            s.KpiUptimePath = "{Plant}.Value";
        });
        await using var session = await RenderSession.StartAsync([symbol], layout, new() { ["Plant1.Value"] = "31.25" });
        Assert.Contains(before, await session.HtmlAsync());
        await session.UpdateAsync("Plant1.Value", "44.75");
        Assert.Contains(after, await session.HtmlAsync());
        Assert.DoesNotContain(before, await session.HtmlAsync());
    }

    [Theory]
    [InlineData("svg", "motorcontrol")]
    [InlineData("responsive", "motorcontrol")]
    [InlineData("svg", "conveyor")]
    [InlineData("responsive", "conveyor")]
    public async Task FaultBindings_EnterAndLeaveFaultWithUnchangedRunValue(string layout, string type)
    {
        await using var session = await RenderSession.StartAsync([Symbol(type, s =>
        {
            s.VariablePath = "{Plant}.Run";
            s.MotorFaultPath = "{Plant}.Fault";
            s.ConveyorFaultPath = "{Plant}.Fault";
            s.MotorFaultColor = "#a10000";
            s.ConveyorFaultColor = "#a10000";
        })], layout, new() { ["Plant1.Run"] = "1", ["Plant1.Fault"] = "0" });
        Assert.DoesNotContain("#a10000", await session.HtmlAsync());
        await session.UpdateAsync("Plant1.Fault", "1");
        Assert.Contains("#a10000", await session.HtmlAsync());
        await session.UpdateAsync("Plant1.Fault", "0");
        Assert.DoesNotContain("#a10000", await session.HtmlAsync());
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task XYPlot_AliasChangeStartsNewTraceEvenWhenCurrentCoordinatesMatch(string layout)
    {
        await using var session = await RenderSession.StartAsync([Symbol("xyplot", s =>
        {
            s.XYPlotXPath = "{Plant}.X";
            s.XYPlotYPath = "{Plant}.Y";
        })], layout, new() { ["Plant1.X"] = "25", ["Plant1.Y"] = "75", ["Plant2.X"] = "50", ["Plant2.Y"] = "75" });
        await session.UpdateAsync("Plant1.X", "50");
        Assert.Single(Elements(await session.HtmlAsync(), "polyline"));
        session.Aliases["Plant"] = "Plant2";
        await session.RefreshAsync();
        var html = await session.HtmlAsync();
        Assert.Empty(Elements(html, "polyline"));
        Assert.Equal(100, Number(Assert.Single(Elements(html, "circle")), "cx"));
        Assert.Equal(["Plant2.X", "Plant2.Y"], session.RegisteredPaths.Order());
    }

    [Fact]
    public async Task ScreenChange_DoesNotReuseCachedSymbolWithSameIdAndValues()
    {
        await using var session = await RenderSession.StartAsync([Symbol("barchart", s =>
        {
            s.BarChartBars = "Gain=PID.Kp #3b82f6";
            s.BarChartMax = 20;
        })], "svg", new() { ["PID.Kp"] = "10" });
        Assert.Equal(52, Number(ColoredElement(await session.HtmlAsync(), "rect", "#3b82f6"), "height"));
        await session.ChangeScreenAsync(Symbol("barchart", s =>
        {
            s.BarChartBars = "Gain=PID.Kp #ef4444";
            s.BarChartMax = 40;
        }));
        var html = await session.HtmlAsync();
        Assert.Equal(26, Number(ColoredElement(html, "rect", "#ef4444"), "height"));
        Assert.DoesNotContain("#3b82f6", html);
    }

    private static ScreenSymbol Symbol(string type, Action<ScreenSymbol> configure)
    {
        var symbol = new ScreenSymbol { Id = "widget", Type = type, Width = 200, Height = 120 };
        configure(symbol);
        return symbol;
    }

    private static IEnumerable<XElement> Elements(string html, string name) =>
        Regex.Matches(html, $@"<{name}\b[^>]*(?:/>|>.*?</{name}>)", RegexOptions.Singleline)
            .Select(match => XElement.Parse(match.Value));
    private static XElement ColoredElement(string html, string name, string color) =>
        Assert.Single(Elements(html, name), element => (string?)element.Attribute("fill") == color);
    private static double Number(XElement element, string attribute) => double.Parse(element.Attribute(attribute)!.Value, CultureInfo.InvariantCulture);

    private sealed class RenderSession : IAsyncDisposable
    {
        private readonly ServiceProvider _services;
        private readonly HtmlRenderer _renderer;
        private HtmlRootComponent _root;
        private ScreenHarness _host = null!;
        private bool _disposed;
        public OpcRuntimeClient Opc { get; } = new();
        public Dictionary<string, string> Aliases { get; } = new() { ["Plant"] = "Plant1" };
        public IEnumerable<string> RegisteredPaths => ((ConcurrentDictionary<string, HashSet<string>>)typeof(OpcRuntimeClient)
            .GetField("_callerPaths", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(Opc)!).Values.SelectMany(paths => paths).Distinct();

        private RenderSession()
        {
            _services = new ServiceCollection().AddLogging().AddSingleton(Opc).AddSingleton<CommandService>()
                .AddSingleton<RuntimeAuthService>().AddSingleton<LocalizationService>().AddSingleton<ProjectService>()
                .AddSingleton<IJSRuntime, NoBrowserJs>().BuildServiceProvider();
            _renderer = new HtmlRenderer(_services, _services.GetRequiredService<ILoggerFactory>());
        }

        public static async Task<RenderSession> StartAsync(ScreenSymbol[] symbols, string layout, Dictionary<string, string>? values = null)
        {
            var session = new RenderSession();
            if (values != null)
                foreach (var (path, value) in values) session.Opc.UpdateLocalValue(path, value);
            session._root = await session._renderer.Dispatcher.InvokeAsync(() => session._renderer.RenderComponentAsync<ScreenHarness>(
                ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    [nameof(ScreenHarness.Screen)] = new ScreenConfig { Name = "Bindings", LayoutMode = layout, Symbols = [.. symbols] },
                    [nameof(ScreenHarness.Aliases)] = session.Aliases,
                    [nameof(ScreenHarness.Capture)] = (Action<ScreenHarness>)(host => session._host = host)
                })));
            return session;
        }

        public Task<string> HtmlAsync() => _renderer.Dispatcher.InvokeAsync(_root.ToHtmlString);
        public Task RefreshAsync() => _renderer.Dispatcher.InvokeAsync(_host.Refresh);
        public Task ClearScreenAsync() => _renderer.Dispatcher.InvokeAsync(() => _host.ChangeScreen(null));
        public Task ChangeScreenAsync(ScreenSymbol symbol) => _renderer.Dispatcher.InvokeAsync(() =>
            _host.ChangeScreen(new ScreenConfig { Name = "Other", Symbols = [symbol] }));
        public Task UpdateAsync(string path, string value) => _renderer.Dispatcher.InvokeAsync(() =>
        {
            Opc.UpdateLocalValue(path, value);
            _host.Refresh();
        });
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;
            await _renderer.DisposeAsync();
            await _services.DisposeAsync();
        }
    }

    public sealed class ScreenHarness : ComponentBase
    {
        [Parameter] public ScreenConfig? Screen { get; set; }
        [Parameter] public Dictionary<string, string>? Aliases { get; set; }
        [Parameter] public Action<ScreenHarness>? Capture { get; set; }
        private ScreenConfig? _screen;
        protected override void OnInitialized() { _screen = Screen; Capture?.Invoke(this); }
        public void Refresh() => StateHasChanged();
        public void ChangeScreen(ScreenConfig? screen) { _screen = screen; StateHasChanged(); }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<ScreenRenderer>(0);
            builder.AddAttribute(1, nameof(ScreenRenderer.Screen), _screen);
            builder.AddAttribute(2, nameof(ScreenRenderer.VariableAliases), Aliases);
            builder.CloseComponent();
        }
    }

    private sealed class NoBrowserJs : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            throw new InvalidOperationException("Unexpected browser call: " + identifier);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);
    }
}
