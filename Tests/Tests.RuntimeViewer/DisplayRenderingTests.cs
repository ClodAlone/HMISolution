using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using RuntimeViewer.Shared.Components.Viewer;
using RuntimeViewer.Shared.Services;
using SharedModels;
using Xunit;

namespace Tests.RuntimeViewer;

public class DisplayRenderingTests
{
    [Theory]
    [InlineData("svg", "numericdisplay")]
    [InlineData("responsive", "numericdisplay")]
    [InlineData("svg", "editbox")]
    [InlineData("responsive", "editbox")]
    public async Task StyledDisplay_UsesConfiguredReadoutInBothLayouts(string layout, string type)
    {
        var symbol = new ScreenSymbol
        {
            Id = "display", Type = type, VariablePath = "Demo.Value", Width = 240, Height = 80,
            NumericDisplayFormat = "F2", EditBoxFormat = "F2", NumericDisplayUnit = "bar"
        };
        DisplayVisuals.ApplyPreset(symbol, "lcd");
        var opc = new OpcRuntimeClient();
        opc.UpdateLocalValue("Demo.Value", "-1234.5");
        await using var services = Services(opc);
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenRenderer>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(ScreenRenderer.Screen)] = new ScreenConfig { LayoutMode = layout, Symbols = [symbol] }
            }))).ToHtmlString());
        Assert.Contains("seven-segment-readout", html);
        Assert.Contains("aria-label=\"-1234.50\"", html);
        Assert.Contains("#d3dfb8", html);
        if (type == "editbox")
        {
            Assert.Contains("editbox-spin-btn", html);
            Assert.Contains("Click to edit", html);
        }
        else Assert.Contains("display-unit", html);
    }

    [Fact]
    public async Task ClassicEditBox_RetainsPlaceholderAndHasNoStyledFace()
    {
        await using var services = Services(new OpcRuntimeClient());
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<EditBoxWidget>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(EditBoxWidget.VariablePath)] = "Missing",
                [nameof(EditBoxWidget.Placeholder)] = "Enter value"
            }))).ToHtmlString());
        Assert.Contains("Enter value", html);
        Assert.Contains("editbox-placeholder", html);
        Assert.DoesNotContain("seven-segment", html);
        Assert.DoesNotContain("container-type:size", html);
    }

    [Fact]
    public async Task StyledEditBox_KeepsEditingCancelStatisticsAndExplicitWriteFailure()
    {
        var opc = new OpcRuntimeClient();
        opc.UpdateLocalValue("Demo.Value", "12.5");
        opc.UpdateLocalValue("Demo.Value.Statistics.Min", "1.25");
        opc.UpdateLocalValue("Demo.Value.Statistics.Max", "98.75");
        opc.UpdateLocalValue("Demo.Value.Statistics.Average", "33.33");
        opc.UpdateLocalValue("Demo.Value.Statistics.Count", "20");
        var symbol = new ScreenSymbol { Type = "editbox" };
        DisplayVisuals.ApplyPreset(symbol, "midnight");
        await using var services = Services(opc);
        await using var renderer = new EditRenderer(services);
        await renderer.StartAsync(symbol);
        Assert.Contains("editbox-stats", renderer.Html);
        Assert.Contains("1.25", renderer.Html);
        Assert.Contains("98.75", renderer.Html);
        Assert.Contains("33.33", renderer.Html);
        Assert.Contains("20", renderer.Html);
        await renderer.InvokeAsync("StartEdit");
        Assert.Contains("class=editbox-input", renderer.Html);
        Assert.Contains("cqw", renderer.Html);
        await renderer.InvokeAsync("OnKeyDown", new KeyboardEventArgs { Key = "Escape" });
        Assert.DoesNotContain("class=editbox-input", renderer.Html);
        Assert.Equal("12.5", opc.GetValue("Demo.Value"));
        await renderer.InvokeAsync("StartEdit");
        await renderer.InvokeAsync("OnKeyDown", new KeyboardEventArgs { Key = "Enter" });
        Assert.DoesNotContain("class=editbox-input", renderer.Html);
        Assert.Contains("Write failed", renderer.Html);
        Assert.Equal("12.5", opc.GetValue("Demo.Value"));
    }

    private static ServiceProvider Services(OpcRuntimeClient opc) => new ServiceCollection().AddLogging()
        .AddSingleton(opc).AddSingleton<CommandService>().AddSingleton<RuntimeAuthService>()
        .AddSingleton<LocalizationService>().AddSingleton<ProjectService>().AddSingleton<IJSRuntime, NoBrowserJsRuntime>()
        .BuildServiceProvider();

    private sealed class NoBrowserJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            throw new InvalidOperationException($"Unexpected JavaScript: {identifier}");
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
    }

#pragma warning disable BL0006 // Attach a real editable widget to exercise its existing editing lifecycle.
    private sealed class EditRenderer(IServiceProvider services) : Renderer(services, services.GetRequiredService<ILoggerFactory>())
    {
        private EditBoxWidget _widget = null!;
        private int _root;
        private ParameterView _parameters;
        public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();
        public string Html { get; private set; } = "";
        public Task StartAsync(ScreenSymbol symbol) => Dispatcher.InvokeAsync(async () =>
        {
            _widget = (EditBoxWidget)InstantiateComponent(typeof(EditBoxWidget));
            _root = AssignRootComponentId(_widget);
            _parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(EditBoxWidget.VariablePath)] = "Demo.Value",
                [nameof(EditBoxWidget.Appearance)] = symbol,
                [nameof(EditBoxWidget.ShowStatistics)] = true,
                [nameof(EditBoxWidget.Format)] = "F2"
            });
            await RenderRootComponentAsync(_root, _parameters);
        });
        public Task InvokeAsync(string method, params object[] args) => Dispatcher.InvokeAsync(async () =>
        {
            var result = typeof(EditBoxWidget).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(_widget, args);
            if (result is Task task) await task;
            await RenderRootComponentAsync(_root, _parameters);
        });
        protected override void HandleException(Exception exception) => throw new InvalidOperationException("Widget render failed.", exception);
        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            var frames = GetCurrentRenderTreeFrames(_root);
            Html = string.Join(" ", frames.Array.Take(frames.Count).Select(f => f.FrameType switch
            {
                RenderTreeFrameType.Attribute => f.AttributeName + "=" + f.AttributeValue,
                RenderTreeFrameType.Text => f.TextContent,
                RenderTreeFrameType.Markup => f.MarkupContent,
                _ => ""
            }));
            return Task.CompletedTask;
        }
    }
#pragma warning restore BL0006
}
