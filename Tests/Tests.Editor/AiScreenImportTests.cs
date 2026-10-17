using System.Text.Json;
using System.Text.Json.Nodes;
using ServerEditorWeb.Services;
using Xunit;

namespace Tests.Editor;

public class AiScreenImportTests
{
    private const string TwoTanksBundle = """
        {
          "Folder":{"Name":"__TASK__","Folders":[{"Name":"TwoTanks","Folders":[],"Variables":[
            {"Name":"Tank1Level","Type":"Double","Access":"ReadWrite","Value":65.0,"EngineeringUnit":"%","Description":"Tank 1 level percentage"},
            {"Name":"Tank2Level","Type":"Double","Access":"ReadWrite","Value":30.0,"EngineeringUnit":"%","Description":"Tank 2 level percentage"},
            {"Name":"Flow","Type":"Double","Access":"ReadWrite","Value":1.0,"EngineeringUnit":"L/s","Description":"Flow rate between tanks"}
          ]}],"Variables":[]},
          "Screens":[{"Name":"__TASK___TwoTanksDemo","Width":1000,"Height":600,"Background":"#0f172a","LayoutMode":"svg","Group":"__TASK__","Symbols":[
            {"Type":"text","Id":"title","X":300,"Y":30,"Width":400,"Height":50,"Text":"Two Tanks Demo System","FontSize":32,"FontWeight":"bold","Color":"#ffffff","HorizontalAlignment":"center"},
            {"Type":"tank","Id":"tank1","X":100,"Y":140,"Width":160,"Height":240,"VariablePath":"__TASK__.TwoTanks.Tank1Level","TankMin":0,"TankMax":100,"TankUnit":"%","TankShowLevel":true,"TankLiquidColor":"#3b82f6","TankBodyColor":"#1e293b","TankStyle":"default"},
            {"Type":"text","Id":"tank1Label","X":100,"Y":390,"Width":160,"Height":30,"Text":"Tank 1","FontSize":18,"FontWeight":"bold","Color":"#ffffff","HorizontalAlignment":"center"},
            {"Type":"numericdisplay","Id":"tank1Display","X":100,"Y":425,"Width":160,"Height":40,"VariablePath":"__TASK__.TwoTanks.Tank1Level","DecimalPlaces":1,"Unit":"%","FontSize":20,"Color":"#3b82f6","BackgroundColor":"#1e293b","BorderColor":"#475569","HorizontalAlignment":"center"},
            {"Type":"tank","Id":"tank2","X":700,"Y":140,"Width":160,"Height":240,"VariablePath":"__TASK__.TwoTanks.Tank2Level","TankMin":0,"TankMax":100,"TankUnit":"%","TankShowLevel":true,"TankLiquidColor":"#10b981","TankBodyColor":"#1e293b","TankStyle":"default"},
            {"Type":"text","Id":"tank2Label","X":700,"Y":390,"Width":160,"Height":30,"Text":"Tank 2","FontSize":18,"FontWeight":"bold","Color":"#ffffff","HorizontalAlignment":"center"},
            {"Type":"numericdisplay","Id":"tank2Display","X":700,"Y":425,"Width":160,"Height":40,"VariablePath":"__TASK__.TwoTanks.Tank2Level","DecimalPlaces":1,"Unit":"%","FontSize":20,"Color":"#10b981","BackgroundColor":"#1e293b","BorderColor":"#475569","HorizontalAlignment":"center"},
            {"Type":"pipe","Id":"pipe1","X":260,"Y":310,"Width":440,"Height":20,"VariablePath":"__TASK__.TwoTanks.Flow","PipeOrientation":"horizontal","PipeColor":"#475569","PipeFlowColor":"#06b6d4","PipeShowFlow":true,"PipeFlowSpeed":1},
            {"Type":"text","Id":"flowLabel","X":400,"Y":270,"Width":200,"Height":30,"Text":"Flow Rate","FontSize":16,"FontWeight":"bold","Color":"#ffffff","HorizontalAlignment":"center"},
            {"Type":"numericdisplay","Id":"flowDisplay","X":420,"Y":340,"Width":160,"Height":35,"VariablePath":"__TASK__.TwoTanks.Flow","DecimalPlaces":2,"Unit":"L/s","FontSize":18,"Color":"#06b6d4","BackgroundColor":"#1e293b","BorderColor":"#475569","HorizontalAlignment":"center"}
          ]}]
        }
        """;

    [Fact]
    public void ReportedTwoTanksBundle_ImportsAllSymbolsBindingsAndFormatting_AndSurvivesRefresh()
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        var project = editor.ActiveProject!;

        var result = editor.ImportAiResources(project, TwoTanksBundle);

        Assert.True(result.success, result.message);
        var folder = Assert.Single(Assert.Single(project.Model.Folder.Folders).Folders);
        Assert.Equal("TwoTanks", folder.Name);
        Assert.Equal(3, folder.Variables.Count);
        var screen = Assert.Single(project.Model.Screens);
        Assert.Equal("AI_Task_TwoTanksDemo", screen.Name);
        Assert.Equal(1000, screen.Width);
        Assert.Equal(600, screen.Height);
        Assert.Equal(10, screen.Symbols.Count);
        var title = screen.Symbols[0];
        Assert.Equal("Two Tanks Demo System", title.Label);
        Assert.Equal("#ffffff", title.Fill);
        Assert.Equal(32, title.FontSize);
        Assert.Equal("bold", title.FontWeight);
        Assert.Equal(new[] { "Tank 1", "Tank 2", "Flow Rate" }, screen.Symbols.Where(s => s.Type == "text" && s.Id != "title").Select(s => s.Label));
        var displays = screen.Symbols.Where(s => s.Type == "numericdisplay").ToArray();
        Assert.Equal(new[] { "F1", "F1", "F2" }, displays.Select(s => s.NumericDisplayFormat));
        Assert.Equal(new[] { 1, 1, 2 }, displays.Select(s => s.NumericDisplayDecimals));
        Assert.Equal(new[] { "%", "%", "L/s" }, displays.Select(s => s.NumericDisplayUnit));
        Assert.Equal(new[] { "#3b82f6", "#10b981", "#06b6d4" }, displays.Select(s => s.Fill));
        Assert.All(displays, s =>
        {
            Assert.Equal("#1e293b", s.NumericDisplayBackground);
            Assert.Equal("#475569", s.Stroke);
        });
        Assert.Equal(new[] { "AI_Task.TwoTanks.Tank1Level", "AI_Task.TwoTanks.Tank2Level", "AI_Task.TwoTanks.Flow" }, displays.Select(s => s.VariablePath));
        var tanks = screen.Symbols.Where(s => s.Type == "tank").ToArray();
        var pipe = Assert.Single(screen.Symbols.Where(s => s.Type == "pipe"));
        Assert.Equal(tanks[0].X + tanks[0].Width, pipe.X);
        Assert.Equal(tanks[1].X, pipe.X + pipe.Width);
        Assert.Equal("#06b6d4", pipe.PipeFlowColor);
        Assert.Equal("AI_Task.TwoTanks.Flow", pipe.VariablePath);
        editor.RefreshJsonFromTree();
        var saved = JsonNode.Parse(editor.JsonEditorText)!;
        Assert.Equal("Two Tanks Demo System", saved["Screens"]![0]!["Symbols"]![0]!["Label"]!.GetValue<string>());
        Assert.Null(saved["Screens"]![0]!["Symbols"]![0]!["Text"]);
        Assert.Same(screen, Assert.Single(project.Model.Screens));
    }

    [Theory]
    [InlineData("\"Text\":\"same\",\"Label\":\"same\",\"Color\":\"red\",\"Fill\":\"red\"")]
    [InlineData("\"Label\":\"same\",\"Fill\":\"red\"")]
    public void ExistingCanonicalProperties_ArePreserved(string properties)
    {
        var model = AiResourceBundle.Parse("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"Type\":\"text\"," + properties + "}]}]}", "AI_Task");
        var symbol = Assert.Single(Assert.Single(model.Screens).Symbols);
        Assert.Equal("same", symbol.Label);
        Assert.Equal("red", symbol.Fill);
    }

    [Theory]
    [InlineData("\"Text\":\"one\",\"Label\":\"two\"")]
    [InlineData("\"Color\":\"red\",\"Fill\":\"blue\"")]
    [InlineData("\"HorizontalAlignment\":\"left\"")]
    [InlineData("\"DecimalPlaces\":-1")]
    [InlineData("\"DecimalPlaces\":100")]
    [InlineData("\"DecimalPlaces\":1.5")]
    [InlineData("\"DecimalPlaces\":\"2\"")]
    [InlineData("\"DecimalPlaces\":2,\"NumericDisplayFormat\":\"F1\"")]
    [InlineData("\"DecimalPlaces\":2,\"NumericDisplayDecimals\":1")]
    [InlineData("\"UnknownStyle\":true")]
    public void ConflictingOrUnsupportedProperties_RejectWholeImport(string properties)
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        var project = editor.ActiveProject!;
        var before = JsonSerializer.Serialize(project.Model);
        var json = "{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"Type\":\"numericdisplay\"," + properties + "}]}]}";

        var result = editor.ImportAiResources(project, json);

        Assert.False(result.success);
        Assert.Equal(before, JsonSerializer.Serialize(project.Model));
    }

    [Fact]
    public void ReusableSymbolTemplates_UseTheSameAliasNormalization()
    {
        var model = AiResourceBundle.Parse("""
            {"UserSymbolGroups":[{"Id":"__TASK___Group","Name":"__TASK___Group","TemplateSymbols":[
              {"Type":"text","Text":"Template label","Color":"white","HorizontalAlignment":"center"}]}]}
            """, "AI_Task");
        var symbol = Assert.Single(Assert.Single(model.UserSymbolGroups).TemplateSymbols);
        Assert.Equal("Template label", symbol.Label);
        Assert.Equal("white", symbol.Fill);
    }

    [Fact]
    public void ProviderPrompt_UsesActualTextAndNumericDisplayProperties()
    {
        var prompt = new AiService().BuildResourceBundlePrompt("Create two tanks");
        Assert.Contains("Label (NOT Text)", prompt);
        Assert.Contains("Fill for text color (NOT Color)", prompt);
        Assert.Contains("NumericDisplayFormat", prompt);
        Assert.Contains("NumericDisplayUnit", prompt);
        Assert.Contains("NumericDisplayBackground", prompt);
        Assert.Contains("do NOT add HorizontalAlignment", prompt);
    }
}
