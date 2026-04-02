using Xunit;
using ServerEditorWeb.Services;
using SharedModels;

namespace Tests.Editor;

public class ProjectDiffServiceTests
{
    private readonly ProjectDiffService _svc = new();

    private static NodeModel EmptyModel() => new();

    // ─── Parse ───────────────────────────────────────────────

    [Fact]
    public void Parse_ValidJson_ReturnsModel()
    {
        var json = """{ "Screens": [{ "Name": "Main" }] }""";
        var model = _svc.Parse(json);
        Assert.NotNull(model);
        Assert.Single(model!.Screens);
        Assert.Equal("Main", model.Screens[0].Name);
    }

    [Fact]
    public void Parse_InvalidJson_ReturnsNull()
    {
        Assert.Null(_svc.Parse("not json"));
    }

    // ─── Compare: empty models ──────────────────────────────

    [Fact]
    public void Compare_IdenticalEmptyModels_NoDifferences()
    {
        var result = _svc.Compare(EmptyModel(), EmptyModel());
        Assert.Empty(result);
    }

    // ─── Compare: Screens ───────────────────────────────────

    [Fact]
    public void Compare_ScreenAdded_ReportsAdded()
    {
        var left = EmptyModel();
        var right = EmptyModel();
        right.Screens.Add(new ScreenConfig { Name = "Dashboard" });

        var result = _svc.Compare(left, right);

        var entry = Assert.Single(result, e => e.Category == "Screen");
        Assert.Equal(DiffKind.Added, entry.Kind);
        Assert.Equal("Dashboard", entry.Name);
    }

    [Fact]
    public void Compare_ScreenRemoved_ReportsRemoved()
    {
        var left = EmptyModel();
        left.Screens.Add(new ScreenConfig { Name = "Old" });
        var right = EmptyModel();

        var result = _svc.Compare(left, right);

        var entry = Assert.Single(result, e => e.Category == "Screen");
        Assert.Equal(DiffKind.Removed, entry.Kind);
        Assert.Equal("Old", entry.Name);
    }

    // ─── Compare: Variables ─────────────────────────────────

    [Fact]
    public void Compare_VariableAdded_ReportsAdded()
    {
        var left = EmptyModel();
        var right = EmptyModel();
        right.Folder = new Folder
        {
            Name = "",
            Variables = new() { new Variable { Name = "Temp" } }
        };

        var result = _svc.Compare(left, right);

        var entry = Assert.Single(result, e => e.Category == "Variable");
        Assert.Equal(DiffKind.Added, entry.Kind);
        Assert.Equal("Temp", entry.Name);
    }

    [Fact]
    public void Compare_NestedVariableRemoved_ReportsFullPath()
    {
        var left = EmptyModel();
        left.Folder = new Folder
        {
            Name = "",
            Folders = new()
            {
                new Folder
                {
                    Name = "IO",
                    Variables = new() { new Variable { Name = "Sensor1" } }
                }
            }
        };
        var right = EmptyModel();

        var result = _svc.Compare(left, right);

        var entry = Assert.Single(result, e => e.Category == "Variable");
        Assert.Equal(DiffKind.Removed, entry.Kind);
        Assert.Equal("IO.Sensor1", entry.Name);
    }

    // ─── Compare: Scripts ───────────────────────────────────

    [Fact]
    public void Compare_ScriptAddedAndRemoved_ReportsBoth()
    {
        var left = EmptyModel();
        left.Scripts.Add(new ScriptConfig { Name = "OldScript" });

        var right = EmptyModel();
        right.Scripts.Add(new ScriptConfig { Name = "NewScript" });

        var result = _svc.Compare(left, right);

        Assert.Contains(result, e => e.Category == "Script" && e.Kind == DiffKind.Added && e.Name == "NewScript");
        Assert.Contains(result, e => e.Category == "Script" && e.Kind == DiffKind.Removed && e.Name == "OldScript");
    }

    // ─── Compare: Screen Symbols count ──────────────────────

    [Fact]
    public void Compare_ScreenSymbolCountChanged_ReportsModified()
    {
        var left = EmptyModel();
        left.Screens.Add(new ScreenConfig
        {
            Name = "Main",
            Symbols = new() { new ScreenSymbol { Id = "1" } }
        });

        var right = EmptyModel();
        right.Screens.Add(new ScreenConfig
        {
            Name = "Main",
            Symbols = new() { new ScreenSymbol { Id = "1" }, new ScreenSymbol { Id = "2" }, new ScreenSymbol { Id = "3" } }
        });

        var result = _svc.Compare(left, right);

        var entry = Assert.Single(result, e => e.Category == "Screen Symbols");
        Assert.Equal(DiffKind.Modified, entry.Kind);
        Assert.Contains("1", entry.Detail);
        Assert.Contains("3", entry.Detail);
    }

    // ─── Compare: Server Settings ───────────────────────────

    [Fact]
    public void Compare_ServerSettingsChanged_ReportsModified()
    {
        var left = EmptyModel();
        left.Server = new ServerSettings { StartupScreen = "A" };

        var right = EmptyModel();
        right.Server = new ServerSettings { StartupScreen = "B" };

        var result = _svc.Compare(left, right);

        Assert.Contains(result, e => e.Category == "Server Settings" && e.Kind == DiffKind.Modified);
    }

    [Fact]
    public void Compare_ServerSettingsUnchanged_NoEntry()
    {
        var left = EmptyModel();
        var right = EmptyModel();

        var result = _svc.Compare(left, right);

        Assert.DoesNotContain(result, e => e.Category == "Server Settings");
    }

    // ─── Compare: Multiple categories ───────────────────────

    [Fact]
    public void Compare_MultipleChanges_AllReported()
    {
        var left = EmptyModel();
        left.Screens.Add(new ScreenConfig { Name = "Old" });
        left.Users.Add(new UserConfig { Username = "admin" });

        var right = EmptyModel();
        right.Screens.Add(new ScreenConfig { Name = "New" });
        right.Users.Add(new UserConfig { Username = "admin" });
        right.Users.Add(new UserConfig { Username = "operator" });
        right.Recipes.Add(new RecipeConfig { Name = "Mix1" });

        var result = _svc.Compare(left, right);

        // Screen: Old removed, New added
        Assert.Contains(result, e => e.Category == "Screen" && e.Kind == DiffKind.Removed && e.Name == "Old");
        Assert.Contains(result, e => e.Category == "Screen" && e.Kind == DiffKind.Added && e.Name == "New");
        // User: operator added
        Assert.Contains(result, e => e.Category == "User" && e.Kind == DiffKind.Added && e.Name == "operator");
        // Recipe: Mix1 added
        Assert.Contains(result, e => e.Category == "Recipe" && e.Kind == DiffKind.Added && e.Name == "Mix1");
    }

    // ─── DiffEntry ──────────────────────────────────────────

    [Fact]
    public void DiffEntry_Icon_MatchesKind()
    {
        Assert.Equal("+", new DiffEntry("X", "Y", DiffKind.Added).Icon);
        Assert.Equal("−", new DiffEntry("X", "Y", DiffKind.Removed).Icon);
        Assert.Equal("~", new DiffEntry("X", "Y", DiffKind.Modified).Icon);
    }

    [Fact]
    public void DiffEntry_CssClass_MatchesKind()
    {
        Assert.Equal("diff-added", new DiffEntry("X", "Y", DiffKind.Added).CssClass);
        Assert.Equal("diff-removed", new DiffEntry("X", "Y", DiffKind.Removed).CssClass);
        Assert.Equal("diff-modified", new DiffEntry("X", "Y", DiffKind.Modified).CssClass);
    }
}
