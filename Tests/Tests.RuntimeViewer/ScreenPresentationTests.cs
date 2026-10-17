// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Reflection;
using System.Text.Json;
using RuntimeViewer.Shared.Services;
using SharedModels;
using Xunit;

namespace Tests.RuntimeViewer;

public class ScreenPresentationTests
{
    [Theory]
    [InlineData("slide-left", "slide-right")]
    [InlineData("slide-right", "slide-left")]
    [InlineData("slide-up", "slide-down")]
    [InlineData("slide-down", "slide-up")]
    [InlineData("fade", "fade")]
    [InlineData("none", "none")]
    [InlineData("unknown", "none")]
    public void Transition_UsesConfiguredEffectAndReversesOnBack(string effect, string reversed)
    {
        var screen = new ScreenConfig { NavigationTransition = effect };
        Assert.Equal(effect == "unknown" ? "none" : effect, ScreenPresentation.Transition(screen));
        Assert.Equal(reversed, ScreenPresentation.Transition(screen, reverse: true));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(280, 280)]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2000)]
    public void Duration_ClampsToSupportedRange(int configured, int expected)
    {
        Assert.Equal(expected, ScreenPresentation.Duration(new() { NavigationDurationMs = configured }));
    }

    [Theory]
    [InlineData(0, 24)]
    [InlineData(24, 24)]
    [InlineData(160, 160)]
    [InlineData(1200, 1200)]
    [InlineData(1201, 1200)]
    public void DockSize_ClampsToSupportedRange(int configured, int expected)
    {
        Assert.Equal(expected, ScreenPresentation.DockSize(new() { DockSize = configured }));
    }

    [Theory]
    [InlineData("up", "Overview")]
    [InlineData("down", "$back")]
    [InlineData("left", "Detail")]
    [InlineData("right", "Menu")]
    [InlineData("unknown", "")]
    public void SwipeTarget_ResolvesEachDirectionIndependently(string direction, string expected)
    {
        var screen = new ScreenConfig
        {
            SwipeUpScreen = "Overview", SwipeDownScreen = "$back", SwipeLeftScreen = "Detail", SwipeRightScreen = "Menu"
        };
        Assert.Equal(expected, ScreenPresentation.SwipeTarget(screen, direction));
    }

    [Theory]
    [InlineData("top", "down")]
    [InlineData("bottom", "up")]
    [InlineData("left", "right")]
    [InlineData("right", "left")]
    [InlineData("none", "")]
    public void Layout_OnlyRecognizesEdgesAndOpensInward(string edge, string direction)
    {
        Assert.Equal(edge != "none", new ScreenConfig { RuntimeDock = edge }.IsLayoutScreen);
        Assert.Equal(direction, ScreenPresentation.OpeningDirection(edge));
    }

    [Fact]
    public void LegacyJson_LeavesNavigationInstantAndLayoutsDisabled()
    {
        var screen = JsonSerializer.Deserialize<ScreenConfig>("{\"Name\":\"Main\",\"Width\":1024}")!;
        Assert.Equal("none", screen.NavigationTransition);
        Assert.Equal("none", screen.RuntimeDock);
        Assert.False(screen.IsLayoutScreen);
        Assert.Equal("", screen.SwipeUpScreen);
        Assert.Equal("", screen.SwipeDownScreen);
        Assert.Equal(1024, screen.Width);
    }

    [Fact]
    public void Json_RoundTripsPresentationAndDoesNotPersistComputedLayoutFlag()
    {
        var original = new ScreenConfig
        {
            Name = "Menu", RuntimeDock = "left", DockMode = "swipe", DockSize = 300, DockOrder = 2,
            NavigationTransition = "slide-up", NavigationDurationMs = 450,
            SwipeUpScreen = "Next", SwipeDownScreen = "$back", SwipeLeftScreen = "Other", SwipeRightScreen = "Main"
        };
        var json = JsonSerializer.Serialize(original);
        Assert.DoesNotContain("IsLayoutScreen", json);
        var restored = JsonSerializer.Deserialize<ScreenConfig>(json)!;
        Assert.Equal("left", restored.RuntimeDock);
        Assert.Equal("swipe", restored.DockMode);
        Assert.Equal(300, restored.DockSize);
        Assert.Equal(2, restored.DockOrder);
        Assert.Equal("slide-up", restored.NavigationTransition);
        Assert.Equal(450, restored.NavigationDurationMs);
        Assert.Equal("Next", restored.SwipeUpScreen);
        Assert.Equal("$back", restored.SwipeDownScreen);
        Assert.Equal("Other", restored.SwipeLeftScreen);
        Assert.Equal("Main", restored.SwipeRightScreen);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Header")]
    [InlineData("Missing")]
    public void StartupScreen_SkipsLayoutScreensEvenWhenConfigured(string startup)
    {
        var main = new ScreenConfig { Name = "Main" };
        var model = new NodeModel
        {
            Server = new() { StartupScreen = startup },
            Screens = [new() { Name = "Header", RuntimeDock = "top" }, main]
        };
        var project = new ProjectService();
        typeof(ProjectService).GetField("_model", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(project, model);
        Assert.Same(main, project.StartupScreen);
    }

    [Fact]
    public void History_BrowserBackRemovesLaterRoutesAndPreservesEncodedNames()
    {
        var history = new ScreenHistoryService();
        history.Push("/ar");
        history.Push("/screen/Main%20Panel");
        history.Push("/screen/Detail%23One");
        Assert.True(history.ReturnToExisting("/screen/Main%20Panel"));
        history.Push("/screen/Main%20Panel");
        Assert.Equal("/ar", history.PopToPrevious());
        Assert.False(history.CanGoBack);
    }

    [Fact]
    public void History_UnknownAndCurrentRoutesDoNotTruncateHistory()
    {
        var history = new ScreenHistoryService();
        history.Push("/screen/Main");
        history.Push("/screen/Detail");
        Assert.False(history.ReturnToExisting("/screen/Unknown"));
        Assert.False(history.ReturnToExisting("/screen/Detail"));
        Assert.Equal("/screen/Detail", history.CurrentRoute);
        Assert.Equal("/screen/Main", history.PopToPrevious());
    }
}
