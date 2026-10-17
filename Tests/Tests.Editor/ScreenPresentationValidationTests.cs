using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class ScreenPresentationValidationTests
{
    private static List<ProjectIssue> Validate(ScreenConfig screen, params ScreenConfig[] others)
    {
        var model = new NodeModel { Screens = [screen, .. others] };
        return new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model);
    }

    [Theory]
    [InlineData("up")]
    [InlineData("down")]
    [InlineData("left")]
    [InlineData("right")]
    public void MissingSwipeDestination_IsReportedWithScreenNavigationTarget(string direction)
    {
        var screen = new ScreenConfig { Name = "Main" };
        switch (direction)
        {
            case "up": screen.SwipeUpScreen = "Missing"; break;
            case "down": screen.SwipeDownScreen = "Missing"; break;
            case "left": screen.SwipeLeftScreen = "Missing"; break;
            case "right": screen.SwipeRightScreen = "Missing"; break;
        }
        var issue = Assert.Single(Validate(screen).Where(i => i.Message.Contains("destination")));
        Assert.Equal(IssueSeverity.Error, issue.Severity);
        Assert.Equal($"Swipe {direction} destination 'Missing' does not exist.", issue.Message);
        Assert.Equal("screen:Main", issue.NavigationTarget);
    }

    [Fact]
    public void ValidDestinationsAndBack_DoNotReportBrokenSwipeLinks()
    {
        var screen = new ScreenConfig { Name = "Main", SwipeUpScreen = "detail", SwipeDownScreen = "$back" };
        Assert.DoesNotContain(Validate(screen, new ScreenConfig { Name = "Detail" }), i => i.Message.Contains("destination"));
    }

    [Fact]
    public void InvalidPresentationSettings_AreReportedRatherThanSilentlyAccepted()
    {
        var issues = Validate(new()
        {
            Name = "Panel", RuntimeDock = "left", DockMode = "invalid", DockSize = 0,
            NavigationTransition = "invalid", NavigationDurationMs = 3000
        });
        Assert.Contains(issues, i => i.Severity == IssueSeverity.Error && i.Message == "Unknown navigation transition.");
        Assert.Contains(issues, i => i.Severity == IssueSeverity.Error && i.Message == "Dock mode must be fixed or swipe.");
        Assert.Contains(issues, i => i.Severity == IssueSeverity.Warning && i.Message.StartsWith("Dock size must be"));
        Assert.Contains(issues, i => i.Severity == IssueSeverity.Warning && i.Message.StartsWith("Navigation duration must be"));
    }

    [Fact]
    public void PropertyGrid_ExposesPresentationChoicesAndHidesComputedFlag()
    {
        var groups = new PropertyGridService().GetPropertyGroups(new ScreenConfig());
        var navigation = Assert.Single(groups.Where(g => g.Category == "Runtime Navigation"));
        Assert.Equal(new[] { "none", "fade", "slide-left", "slide-right", "slide-up", "slide-down" },
            Assert.Single(navigation.Properties.Where(p => p.Name == "NavigationTransition")).EnumValues);
        var layout = Assert.Single(groups.Where(g => g.Category == "Runtime Layout"));
        Assert.Equal(new[] { "none", "top", "bottom", "left", "right" },
            Assert.Single(layout.Properties.Where(p => p.Name == "RuntimeDock")).EnumValues);
        Assert.DoesNotContain(groups.SelectMany(g => g.Properties), p => p.Name == "IsLayoutScreen");
    }
}
