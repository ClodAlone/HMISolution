using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Components;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class AiResourceImportTests
{
    private const string Bundle = """
        {
          "Folder":{"Name":"__TASK__","Folders":[{"Name":"Demo","Variables":[
            {"Name":"Input","Type":"Double","Value":20},
            {"Name":"Output","Type":"Double","Value":0}
          ]}]},
          "Scripts":[{"Name":"__TASK___Loop","Enabled":true,"Code":"Write(\"__TASK__.Demo.Output\", ReadDouble(\"__TASK__.Demo.Input\"));"}],
          "Screens":[{"Name":"__TASK___Tanks","Symbols":[
            {"Id":"tank1","Type":"tank","VariablePath":"__TASK__.Demo.Input"},
            {"Id":"calculated","Type":"numericdisplay","VariablePath":"__TASK__.Calculated.__TASK___Average"},
            {"Id":"button","Type":"button","Commands":[{"Action":"ExecuteScript","Script":"__TASK___Loop"}]}
          ]}],
          "CalculatedVariables":[{"Name":"__TASK___Average","FolderPath":"__TASK__.Calculated","Expression":"Read(\"__TASK__.Demo.Input\")"}],
          "Recipes":[{"Name":"__TASK___Recipe","DbPath":"existing.db","Variables":[{"VariablePath":"__TASK__.Demo.Output"}]}],
          "Reports":[{"Name":"__TASK___Report","Sections":[{"Type":"Table","TableVariablePaths":["__TASK__.Demo.Input"]}]}],
          "Strings":[{"Key":"__TASK___Title","Translations":{"en":"Demo"}}]
        }
        """;

    [Fact]
    public void Import_AppendsOnlyNewResources_PreservingLiveObjectsAndUnsavedTreeEdits()
    {
        var editor = CreateEditor();
        var project = editor.ActiveProject!;
        var model = project.Model;
        var settings = model.Server;
        settings.StartupScreen = "Existing";
        editor.AddScript();
        var existingScript = Assert.IsType<ScriptNode>(editor.SelectedItem);
        existingScript.Script.Code = "// unsaved existing code";
        var groups = project.Children.ToArray();
        var root = project.Children.OfType<VariableGroupNode>().Single().Children.OfType<FolderNode>().Single();
        editor.SelectNode(root);
        editor.AddVariable();
        var existingVariable = Assert.IsType<VariableNode>(editor.SelectedItem);
        editor.JsonEditorText = "not valid JSON: importing must not parse this";

        var result = editor.ImportAiResources(project, Bundle);

        Assert.True(result.success, result.message);
        Assert.Same(model, project.Model);
        Assert.Same(settings, project.Model.Server);
        Assert.Equal("Existing", settings.StartupScreen);
        Assert.Equal(groups, project.Children);
        Assert.Contains(existingScript, Descendants(project));
        Assert.Equal("// unsaved existing code", existingScript.Script.Code);
        Assert.Contains(existingVariable, root.Children);
        var folder = Assert.Single(root.Children.OfType<FolderNode>());
        Assert.Equal("AI_Task", folder.Name);
        Assert.Same(folder.Folder, Assert.Single(model.Folder.Folders));
        Assert.Same(folder, editor.SelectedItem);
        Assert.Same(folder, Assert.Single(editor.SelectedItems));
        Assert.True(editor.HasUnsavedChanges);
        Assert.True(root.IsExpanded);
        Assert.Equal("not valid JSON: importing must not parse this", editor.JsonEditorText);
        editor.RefreshJsonFromTree();
        Assert.Contains(existingScript.Script, model.Scripts);
        Assert.Contains(existingVariable.Variable, model.Folder.Variables);
        Assert.Equal(2, model.Scripts.Count);
        Assert.Equal("AI_Task", Assert.Single(model.Scripts.Where(item => item != existingScript.Script)).Group);
        Assert.Contains("'AI_Task'", result.message);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-7")]
    [InlineData("9007199254740993123456789")]
    [InlineData("1.25")]
    [InlineData("1e+3")]
    public void Import_NumericSymbolId_IsPreservedExactlyAsAStringAndSavedAsAString(string numericId)
    {
        var json = "{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"Id\":" + numericId + ",\"Type\":\"tank\",\"X\":100,\"Width\":160}]}]}";
        var editor = CreateEditor();
        var project = editor.ActiveProject!;

        var result = editor.ImportAiResources(project, json);

        Assert.True(result.success, result.message);
        var symbol = Assert.Single(Assert.Single(project.Model.Screens).Symbols);
        Assert.Equal(numericId, symbol.Id);
        Assert.Equal(100, symbol.X);
        Assert.Equal(160, symbol.Width);
        editor.RefreshJsonFromTree();
        using var saved = JsonDocument.Parse(editor.JsonEditorText);
        var id = saved.RootElement.GetProperty("Screens")[0].GetProperty("Symbols")[0].GetProperty("Id");
        Assert.Equal(JsonValueKind.String, id.ValueKind);
        Assert.Equal(numericId, id.GetString());
    }

    [Fact]
    public void Import_MixedNumericAndStringIdentifiers_PreservesScreenConnectionsAndGroups()
    {
        const string json = """
            {"Screens":[{"Name":"__TASK___Screen","Symbols":[
              {"Id":1,"Type":"tank","GroupId":5,"ConnectionPorts":[{"Id":10}],"Commands":[{"Id":30,"Action":"Log"}]},
              {"Id":"tank2","Type":"tank","GroupId":"5","ConnectionPorts":[{"Id":"inlet"}]}
            ],"Connections":[{"Id":20,"SourceSymbolId":1,"SourcePortId":10,"TargetSymbolId":"tank2","TargetPortId":"inlet"}]}]}
            """;
        var editor = CreateEditor();
        var project = editor.ActiveProject!;

        var result = editor.ImportAiResources(project, json);

        Assert.True(result.success, result.message);
        var screen = Assert.Single(project.Model.Screens);
        var connection = Assert.Single(screen.Connections);
        Assert.Equal("20", connection.Id);
        Assert.Equal("1", screen.Symbols[0].Id);
        Assert.Equal(screen.Symbols[0].Id, connection.SourceSymbolId);
        Assert.Equal("10", Assert.Single(screen.Symbols[0].ConnectionPorts).Id);
        Assert.Equal(screen.Symbols[0].ConnectionPorts[0].Id, connection.SourcePortId);
        Assert.Equal("tank2", screen.Symbols[1].Id);
        Assert.Equal(screen.Symbols[1].Id, connection.TargetSymbolId);
        Assert.Equal("inlet", connection.TargetPortId);
        Assert.Equal("5", screen.Symbols[0].GroupId);
        Assert.Equal(screen.Symbols[0].GroupId, screen.Symbols[1].GroupId);
        Assert.Equal("30", Assert.Single(screen.Symbols[0].Commands).Id);
    }

    [Fact]
    public void Parser_IdentifierCompatibility_DoesNotChangeNumericIdentifierProperties()
    {
        const string json = """
            {"Screens":[{"Name":"__TASK___Screen","Symbols":[{"Id":"trend","TrendPens":[{"YAxisId":1}]}]}]}
            """;
        var model = AiResourceBundle.Parse(json, "AI_Task");
        Assert.Equal(1, Assert.Single(Assert.Single(Assert.Single(model.Screens).Symbols).TrendPens).YAxisId);
    }

    [Fact]
    public void Import_SameBundleTwice_UsesDifferentNamespacesAndKeepsAllInternalReferences()
    {
        var editor = CreateEditor();
        var project = editor.ActiveProject!;
        Assert.True(editor.ImportAiResources(project, Bundle).success);
        Assert.True(editor.ImportAiResources(project, Bundle).success);

        Assert.Equal(new[] { "AI_Task", "AI_Task_2" }, project.Model.Folder.Folders.Select(item => item.Name));
        for (var index = 0; index < 2; index++)
        {
            var prefix = index == 0 ? "AI_Task" : "AI_Task_2";
            var script = project.Model.Scripts[index];
            var screen = project.Model.Screens[index];
            Assert.Equal(prefix + "_Loop", script.Name);
            Assert.Equal(prefix, script.Group);
            Assert.False(script.Enabled);
            Assert.Equal($"Write(\"{prefix}.Demo.Output\", ReadDouble(\"{prefix}.Demo.Input\"));", script.Code);
            Assert.Equal(prefix + ".Demo.Input", screen.Symbols[0].VariablePath);
            Assert.Equal(prefix + ".Calculated." + prefix + "_Average", screen.Symbols[1].VariablePath);
            Assert.Equal(script.Name, screen.Symbols[2].Commands[0].Script);
            Assert.Equal(prefix + ".Calculated", project.Model.CalculatedVariables[index].FolderPath);
            Assert.Equal($"Read(\"{prefix}.Demo.Input\")", project.Model.CalculatedVariables[index].Expression);
            Assert.Equal(prefix + "_Recipe", project.Model.Recipes[index].Name);
            Assert.Equal(prefix + ".Demo.Output", project.Model.Recipes[index].Variables[0].VariablePath);
            Assert.NotEqual("existing.db", project.Model.Recipes[index].DbPath);
            Assert.Equal(prefix + ".Demo.Input", project.Model.Reports[index].Sections[0].TableVariablePaths[0]);
            Assert.Equal(prefix + "_Title", project.Model.Strings[index].Key);
        }
        Assert.Equal(2, project.Children.OfType<ScriptGroupNode>().Single().Children.OfType<ResourceFolderNode>().Count());
        editor.RefreshJsonFromTree();
        Assert.Equal(2, project.Model.Scripts.Count);
        Assert.Equal(2, project.Model.Screens.Count);
        Assert.Equal(2, project.Model.Recipes.Count);
        Assert.Equal(2, project.Model.Reports.Count);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("{\"Server\":{\"StartupScreen\":\"Changed\"}}")]
    [InlineData("{\"Database\":{}}")]
    [InlineData("{\"Unknown\":[]}")]
    [InlineData("{\"Scripts\":null}")]
    [InlineData("{\"Scripts\":[],\"Scripts\":[]}")]
    [InlineData("{\"Scripts\":[null]}")]
    [InlineData("{\"Scripts\":[{\"Name\":\"__TASK___../Invalid\"}]}")]
    [InlineData("{\"Scripts\":[{\"Name\":\"Existing\"}]}")]
    [InlineData("{\"Scripts\":[{\"Name\":\"__TASK___A\",\"Unknown\":true}]}")]
    [InlineData("{\"Scripts\":[{\"Name\":\"__TASK___A\"},{\"Name\":\"__TASK___a\"}]}")]
    [InlineData("{\"Folder\":{\"Name\":\"Root\",\"Variables\":[{\"Name\":\"A\"}]}}")]
    [InlineData("{\"Folder\":{\"Name\":\"__TASK__\",\"Variables\":[{\"Name\":\"A\"},{\"Name\":\"a\"}]}}")]
    [InlineData("{\"Folder\":{\"Name\":\"__TASK__\",\"Variables\":[{\"Name\":\"bad.path\"}]}}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"VariablePath\":\"Existing.Device\"}]}]}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":null}]}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"Id\":true}]}]}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"Id\":{}}]}]}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"Id\":[]}]}]}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"Id\":1,\"Label\":123}]}]}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"Id\":1,\"X\":\"100\"}]}]}")]
    [InlineData("{\"CalculatedVariables\":[{\"Name\":\"__TASK___Average\",\"FolderPath\":\"_Calculated\"}]}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"RecipeName\":\"ExistingRecipe\"}]}]}")]
    [InlineData("{\"Screens\":[{\"Name\":\"__TASK___Screen\",\"Symbols\":[{\"NavButtonTarget\":\"ExistingScreen\"}]}]}")]
    [InlineData("{\"AutomationRules\":[{\"Name\":\"__TASK___Rule\",\"Id\":\"ExistingRuleId\"}]}")]
    [InlineData("{\"Users\":[{\"Username\":\"__TASK___User\",\"Group\":\"ExistingGroup\"}]}")]
    [InlineData("{\"Cameras\":[{\"CameraId\":\"__TASK___Camera\"}]}")]
    [InlineData("{\"AliasMaps\":[{\"Name\":\"__TASK___Map\",\"Mappings\":{\"Motor\":null}}]}")]
    [InlineData("{\"AliasMaps\":[{\"Name\":\"__TASK___Map\",\"Mappings\":{\"__TASK___Motor\":\"\",\"AI_Task_Motor\":\"\"}}]}")]
    public void InvalidBundle_DoesNotMutateProjectOrSelection(string json)
    {
        var editor = CreateEditor();
        var project = editor.ActiveProject!;
        editor.AddScript();
        var selected = editor.SelectedItem;
        editor.HasUnsavedChanges = false;
        var before = JsonSerializer.Serialize(project.Model);
        var nodes = Descendants(project).ToArray();

        var result = editor.ImportAiResources(project, json);

        Assert.False(result.success);
        Assert.False(string.IsNullOrWhiteSpace(result.message));
        Assert.Equal(before, JsonSerializer.Serialize(project.Model));
        Assert.Equal(nodes, Descendants(project));
        Assert.Same(selected, editor.SelectedItem);
        Assert.False(editor.HasUnsavedChanges);
    }

    [Fact]
    public void Import_RejectsLockedOrSwitchedTargetProject()
    {
        var editor = CreateEditor();
        var original = editor.ActiveProject!;
        original.IsLocked = true;
        Assert.Contains("Unlock", editor.ImportAiResources(original, Bundle).message);
        original.IsLocked = false;
        editor.NewFile();
        var result = editor.ImportAiResources(original, Bundle);
        Assert.False(result.success);
        Assert.Contains("changed or was closed", result.message);
        Assert.Empty(original.Model.Scripts);
        Assert.Empty(editor.ActiveProject!.Model.Scripts);
    }

    [Fact]
    public void ResourcePrompt_HasOnlyFragmentContractTaskSchemaAndOptionalReference()
    {
        var service = new AiService { MaxReferenceChars = 1000 };
        var prompt = service.BuildResourceBundlePrompt("Create two tanks", "reference tag list", "tags.csv");
        Assert.Contains("Create two tanks", prompt);
        Assert.Contains("REFERENCE INPUT (tags.csv)", prompt);
        Assert.Contains("reference tag list", prompt);
        Assert.Contains("No existing project JSON is supplied or required", prompt);
        Assert.Contains("Folder is a NEW child folder", prompt);
        Assert.Contains("__TASK__.DemoLoop.Input1", prompt);
        Assert.Contains("ReadDouble(path)", prompt);
        Assert.Contains("TankLiquidColor", prompt);
        Assert.Contains("quoted JSON strings, never numeric IDs", prompt);
        Assert.Contains("\"SourceSymbolId\":\"tank1\"", prompt);
        Assert.DoesNotContain("modify the following JSON", prompt);
        Assert.DoesNotContain("Return the FULL valid JSON", prompt);
    }

    [Fact]
    public void ResourcePrompt_RejectsOversizedReferencesRatherThanSilentlyTruncating()
    {
        var service = new AiService { MaxReferenceChars = 1000 };
        var error = Assert.Throws<InvalidOperationException>(() => service.BuildResourceBundlePrompt("Import", new string('x', 1001), "large.csv"));
        Assert.Contains("Increase Max ref chars", error.Message);
        Assert.Contains("does not truncate", error.Message);
        var exactLimit = new string('x', 1000);
        Assert.Contains(exactLimit, service.BuildResourceBundlePrompt("Import", exactLimit, "exact.csv"));
    }

    [Fact]
    public async Task PreviewImport_UsesCapturedProjectAndOnlyBundleCallbackWithoutChangingFullJson()
    {
        var service = CreateEditor();
        var target = service.ActiveProject!;
        var component = new JsonEditor();
        AiResourceImportRequest? imported = null;
        SetProperty(component, nameof(JsonEditor.ResourceProject), target);
        SetProperty(component, nameof(JsonEditor.JsonText), "existing full project");
        SetProperty(component, nameof(JsonEditor.OnImportResources), EventCallback.Factory.Create<AiResourceImportRequest>(new object(), request => imported = request));
        SetProperty(component, nameof(JsonEditor.OnAiJsonResult), EventCallback.Factory.Create<string>(new object(), _ => throw new InvalidOperationException("Must not apply full JSON")));
        SetProperty(component, nameof(JsonEditor.JsonTextChanged), EventCallback.Factory.Create<string>(new object(), _ => throw new InvalidOperationException("Must not replace full JSON")));
        SetField(component, "_resourceTarget", target);
        SetField(component, "_resourceJson", Bundle);

        await (Task)typeof(JsonEditor).GetMethod("ImportResources", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null)!;

        Assert.NotNull(imported);
        Assert.Same(target, imported.Project);
        Assert.Equal(Bundle, imported.Json);
        Assert.Equal("existing full project", component.JsonText);
        SetProperty(component, nameof(JsonEditor.ResourceProject), new ProjectNode(new NodeModel(), "other.json"));
        imported = null;
        await (Task)typeof(JsonEditor).GetMethod("ImportResources", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null)!;
        Assert.Null(imported);
        Assert.Contains("original target project", GetField<string>(component, "_aiError"));
    }

    [Fact]
    public void Import_SubstitutesAliasDictionaryKeysAndBoundReferencesTogether()
    {
        const string json = """
            {"Folder":{"Name":"__TASK__","Variables":[{"Name":"Input","Type":"Double"}]},
             "AliasMaps":[{"Name":"__TASK___Map","Mappings":{"__TASK___Input":"__TASK__.Input"}}],
             "Screens":[{"Name":"__TASK___Screen","Symbols":[{"VariablePath":"{__TASK___Input}"}]}]}
            """;
        var editor = CreateEditor();
        var project = editor.ActiveProject!;
        var result = editor.ImportAiResources(project, json);
        Assert.True(result.success, result.message);
        var mapping = Assert.Single(Assert.Single(project.Model.AliasMaps).Mappings);
        Assert.Equal("AI_Task_Input", mapping.Key);
        Assert.Equal("AI_Task.Input", mapping.Value);
        Assert.Equal("{AI_Task_Input}", Assert.Single(Assert.Single(project.Model.Screens).Symbols).VariablePath);
    }

    public static TheoryData<string, string> ResourceSections => new()
    {
        { "Scripts", "Name" }, { "PlcPrograms", "Name" }, { "Screens", "Name" }, { "Recipes", "Name" },
        { "Schedulers", "Name" }, { "Reports", "Name" }, { "CalculatedVariables", "Name" }, { "Assets", "Name" },
        { "BatchSequences", "Name" }, { "Events", "Name" }, { "AutomationRules", "Name" }, { "Users", "Username" },
        { "UserGroups", "Name" }, { "Strings", "Key" }, { "Images", "Id" }, { "AliasMaps", "Name" },
        { "UserSymbolGroups", "Name" }, { "AiAgents", "Name" }, { "McpConnections", "Name" }
    };

    [Theory]
    [MemberData(nameof(ResourceSections))]
    public void EachResourceSection_IsIsolatedAcrossRepeatedImportsAndRetainedOnRefresh(string section, string key)
    {
        var resource = new JsonObject { [key] = "__TASK___Resource" };
        if (section == "CalculatedVariables") resource["FolderPath"] = "__TASK__.Calculated";
        var json = new JsonObject { [section] = new JsonArray(resource) }.ToJsonString();
        var editor = CreateEditor();
        var project = editor.ActiveProject!;

        var first = editor.ImportAiResources(project, json);
        var second = editor.ImportAiResources(project, json);
        Assert.True(first.success, first.message);
        Assert.True(second.success, second.message);
        editor.RefreshJsonFromTree();

        var saved = JsonNode.Parse(editor.JsonEditorText)![section]!.AsArray();
        Assert.Equal(2, saved.Count);
        Assert.Equal("AI_Task_Resource", saved[0]![key]!.GetValue<string>());
        Assert.Equal("AI_Task_2_Resource", saved[1]![key]!.GetValue<string>());
        if (section is "Scripts" or "PlcPrograms" or "Schedulers" or "BatchSequences" or "Events" or "AutomationRules" or "AiAgents" or "McpConnections")
            Assert.All(saved, item => Assert.False(item!["Enabled"]!.GetValue<bool>()));
    }

    [Fact]
    public void CameraResources_AreLinkedToTheirNewScreenAndRetainedOnRefresh()
    {
        const string json = """
            {"Cameras":[{"CameraId":"__TASK___Camera"}],"Screens":[{"Name":"__TASK___Screen",
              "Symbols":[{"Id":"camera1","Type":"ipcamera","Camera":{"CameraId":"__TASK___Camera"}}]}]}
            """;
        var editor = CreateEditor();
        var project = editor.ActiveProject!;
        Assert.True(editor.ImportAiResources(project, json).success);
        var camera = Assert.Single(project.Model.Cameras);
        Assert.Equal("AI_Task_Camera", camera.CameraId);
        Assert.Same(camera, Assert.Single(Assert.Single(project.Model.Screens).Symbols).Camera);
        editor.RefreshJsonFromTree();
        Assert.Same(camera, Assert.Single(project.Model.Cameras));
    }

    [Fact]
    public async Task StaleFullJson_CannotBeAppliedOrSentForProjectReplacementUntilRefreshed()
    {
        var component = new JsonEditor();
        var applied = 0;
        var refreshed = 0;
        SetProperty(component, nameof(JsonEditor.OnApply), EventCallback.Factory.Create<(bool, string)>(new object(), _ => applied++));
        SetProperty(component, nameof(JsonEditor.OnRefresh), EventCallback.Factory.Create(new object(), () => refreshed++));
        SetField(component, "_jsonNeedsRefresh", true);
        SetField(component, "_resourceOnly", false);
        SetField(component, "_prompt", "Modify the existing project");

        await InvokeAsync(component, "Apply");
        Assert.Equal(0, applied);
        Assert.Contains("Refresh from Tree", GetField<string>(component, "_aiError"));
        await InvokeAsync(component, "Ask");
        Assert.False(GetField<bool>(component, "_isProcessing"));
        Assert.Contains("before asking AI", GetField<string>(component, "_aiError"));
        await InvokeAsync(component, "Refresh");
        Assert.Equal(1, refreshed);
        Assert.False(GetField<bool>(component, "_jsonNeedsRefresh"));
        await InvokeAsync(component, "Apply");
        Assert.Equal(1, applied);
    }

    private static Task InvokeAsync(JsonEditor component, string method)
        => (Task)typeof(JsonEditor).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null)!;

    private static NodeEditorService CreateEditor()
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        return editor;
    }

    private static IEnumerable<TreeNode> Descendants(TreeNode node)
        => node.Children.SelectMany(child => new[] { child }.Concat(Descendants(child)));

    private static void SetProperty(object component, string property, object value)
        => component.GetType().GetProperty(property)!.SetValue(component, value);

    private static void SetField(object component, string field, object value)
        => component.GetType().GetProperty(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(component, value);

    private static T GetField<T>(object component, string field)
        => (T)component.GetType().GetProperty(field, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(component)!;
}
