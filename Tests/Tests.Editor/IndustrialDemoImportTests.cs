using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class IndustrialDemoImportTests
{
    public static TheoryData<string, string> Templates => new()
    {
        { "screen-ev-charging", EvChargingTemplate.Json },
        { "screen-water-treatment", WaterTreatmentTemplate.Json },
        { "screen-conveyor-line", ConveyorLineTemplate.Json },
        { "screen-microgrid", MicrogridTemplate.Json },
        { "screen-compressed-air", CompressedAirTemplate.Json }
    };

    [Theory]
    [MemberData(nameof(Templates))]
    public void CommonTask_LoadsExactBundleForReviewWithoutImporting(string id, string json)
    {
        var task = AiTaskCatalog.Tasks.Single(t => t.Id == id);
        Assert.Equal("Screens", task.Category); Assert.Equal(json, task.ResourceBundle); Assert.Contains(json, task.Prompt);
        var editor = new NodeEditorService(); editor.NewFile(); var component = new JsonEditor();
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.ResourceProject))!.SetValue(component, editor.ActiveProject);
        typeof(JsonEditor).GetMethod("OnTaskChanged", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, [new ChangeEventArgs { Value = id }]);
        typeof(JsonEditor).GetMethod("LoadTaskTemplate", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null);
        Assert.Equal(json, component.Session.ResourceJson); Assert.True(component.Session.ResourceOnly);
        Assert.Same(editor.ActiveProject, component.Session.ResourceTarget); Assert.Contains(task.Title, component.Session.AiComments);
        Assert.Empty(editor.ActiveProject!.Model.Screens); Assert.Empty(editor.ActiveProject.Model.Scripts);
    }

    [Fact]
    public void AllFiveTemplates_ImportTwiceTogetherWithoutCollisionsAndPersistValidBindings()
    {
        var editor = new NodeEditorService(); editor.NewFile(); var project = editor.ActiveProject!;
        var server = project.Model.Server; var startup = server.StartupScreen;
        foreach (var template in new[] { EvChargingTemplate.Json, WaterTreatmentTemplate.Json, ConveyorLineTemplate.Json, MicrogridTemplate.Json, CompressedAirTemplate.Json })
            for (var copy = 0; copy < 2; copy++)
            {
                var result = editor.ImportAiResources(project, template);
                Assert.True(result.success, result.message);
            }
        editor.RefreshJsonFromTree(); var model = JsonSerializer.Deserialize<NodeModel>(editor.JsonEditorText)!;
        Assert.Same(server, project.Model.Server); Assert.Equal(startup, server.StartupScreen);
        Assert.Equal(10, model.Screens.Count); Assert.Equal(10, model.Scripts.Count); Assert.Equal(10, model.Folder.Folders.Count);
        Assert.Equal(10, model.Screens.Select(s => s.Name).Distinct().Count());
        Assert.Equal(10, model.Scripts.Select(s => s.Name).Distinct().Count());
        Assert.All(model.Scripts, script => { Assert.False(script.Enabled); Assert.Equal(250, script.IntervalMs); Assert.DoesNotContain("__TASK__", script.Code); });
        var ids = new HashSet<string>(); var references = new List<string>();
        foreach (var screen in model.Screens)
        {
            Assert.Empty(screen.SwipeLeftScreen); Assert.Empty(screen.SwipeRightScreen);
            Assert.Equal((1440d, 1040d), (screen.Width, screen.Height));
            Assert.Equal(screen.Symbols.Count, screen.Symbols.Select(s => s.Id).Distinct().Count());
            Assert.DoesNotContain("__TASK__", JsonSerializer.Serialize(screen));
            foreach (var button in screen.Symbols.Where(s => s.Type == "button"))
            {
                Assert.NotEmpty(button.Commands);
                Assert.All(button.Commands, c => { Assert.Equal("Click", c.Trigger); Assert.StartsWith(screen.Group + ".", c.VariablePath); });
            }
            foreach (var symbol in screen.Symbols.Where(s => s.Type == "svg"))
            {
                var svg = XElement.Parse(symbol.SvgContent!);
                foreach (var attribute in svg.DescendantsAndSelf().Attributes("id")) Assert.True(ids.Add(attribute.Value), "Duplicate paint ID: " + attribute.Value);
                foreach (var attribute in svg.DescendantsAndSelf().Attributes("href"))
                    if (attribute.Value.StartsWith('#')) references.Add(attribute.Value[1..]);
            }
        }
        Assert.All(references, reference => Assert.Contains(reference, ids));
        Assert.Empty(new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model).Where(i => i.Severity == IssueSeverity.Error));
    }
}
