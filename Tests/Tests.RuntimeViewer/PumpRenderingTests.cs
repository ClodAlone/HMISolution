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

public class PumpRenderingTests
{
    [Theory]
    [InlineData("svg", "centrifugal")]
    [InlineData("svg", "isometric")]
    [InlineData("responsive", "centrifugal")]
    [InlineData("responsive", "isometric")]
    public async Task BothLayouts_RenderImpellerAndAliasResolvedFaultOverridesRunning(string layout, string appearance)
    {
        await using var services = new ServiceCollection().AddLogging().AddSingleton<OpcRuntimeClient>()
            .AddSingleton<CommandService>().AddSingleton<RuntimeAuthService>().AddSingleton<LocalizationService>()
            .AddSingleton<ProjectService>().AddSingleton<IJSRuntime, NoBrowserJs>().BuildServiceProvider();
        var opc = services.GetRequiredService<OpcRuntimeClient>();
        opc.UpdateLocalValue("Plant.PumpOn", "true");
        opc.UpdateLocalValue("Plant.Fault", "false");
        var symbol = new ScreenSymbol
        {
            Id = "pump", Type = "motorcontrol", MotorStyle = "pump", MotorAppearance = appearance,
            MotorUseGradient = true, VariablePath = "{Unit}.PumpOn", MotorFaultPath = "{Unit}.Fault",
            InstanceAliases = new() { ["Unit"] = "Plant" }
        };
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(ScreenRenderer.Screen)] = new ScreenConfig { LayoutMode = layout, Symbols = [symbol] }
        });
        async Task<string> Render()
        {
            await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
            return await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<ScreenRenderer>(parameters)).ToHtmlString());
        }
        var running = await Render();
        Assert.Contains("data-state=\"running\"", running);
        Assert.Contains("class=\"pump-sheen\"", running);
        Assert.Contains("animation:hmiPumpSpin", running);
        opc.UpdateLocalValue("Plant.Fault", "true");
        var fault = await Render();
        Assert.Contains("data-state=\"fault\"", fault);
        Assert.DoesNotContain("animation:hmiPumpSpin", fault);
        Assert.Contains(">FAULT</text>", fault);
        opc.UpdateLocalValue("Plant.Fault", "false");
        opc.UpdateLocalValue("Plant.PumpOn", "false");
        var stopped = await Render();
        Assert.Contains("data-state=\"stopped\"", stopped);
        Assert.DoesNotContain("animation:hmiPumpSpin", stopped);
    }

    private sealed class NoBrowserJs : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => throw new InvalidOperationException(identifier);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
    }
}
