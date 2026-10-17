using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Components.Pages;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class ResourceCreationTests
{
    public static TheoryData<string, Type, Type> ResourceCases => new()
    {
        { "AddFolder", typeof(FolderNode), typeof(FolderNode) },
        { "AddVariable", typeof(FolderNode), typeof(VariableNode) },
        { "AddScript", typeof(ScriptGroupNode), typeof(ScriptNode) },
        { "AddPlcProgram", typeof(PlcGroupNode), typeof(PlcProgramNode) },
        { "AddRecipe", typeof(RecipeGroupNode), typeof(RecipeNode) },
        { "AddScheduler", typeof(SchedulerGroupNode), typeof(SchedulerNode) },
        { "AddReport", typeof(ReportGroupNode), typeof(ReportNode) },
        { "AddCalculated", typeof(CalculatedGroupNode), typeof(CalculatedNode) },
        { "AddScreen", typeof(ScreenGroupNode), typeof(ScreenNode) },
        { "AddAsset", typeof(AssetGroupNode), typeof(AssetNode) },
        { "AddBatch", typeof(BatchGroupNode), typeof(BatchNode) },
        { "AddEvent", typeof(EventGroupNode), typeof(EventNode) },
        { "AddAliasMap", typeof(AliasMapGroupNode), typeof(AliasMapNode) },
        { "AddAutomationRule", typeof(AutomationRuleGroupNode), typeof(AutomationRuleNode) },
        { "AddAiAgent", typeof(AiAgentGroupNode), typeof(AiAgentNode) },
        { "AddMcpConnection", typeof(McpConnectionGroupNode), typeof(McpConnectionNode) },
        { "AddUserGroup", typeof(UserGroupListNode), typeof(UserGroupNode) }
    };

    [Theory]
    [MemberData(nameof(ResourceCases))]
    public void Add_FromUnrelatedSelection_CreatesInActiveProjectAndReplacesSelection(string command, Type parentType, Type resourceType)
    {
        var editor = CreateEditor();
        var project = editor.ActiveProject!;
        var previous = project.Children.OfType<ServerSettingsNode>().Single();
        editor.SelectNode(previous);
        editor.HasUnsavedChanges = false;
        var parent = Descendants(project).Single(n => n.GetType() == parentType);
        var count = parent.Children.Count;

        Invoke(editor, command);

        var created = editor.SelectedItem!;
        Assert.IsType(resourceType, created);
        Assert.Same(parent, created.Parent);
        Assert.Equal(count + 1, parent.Children.Count);
        Assert.Contains(created, parent.Children);
        Assert.Same(created, Assert.Single(editor.SelectedItems));
        Assert.True(created.IsSelected);
        Assert.False(previous.IsSelected);
        Assert.True(editor.HasUnsavedChanges);
    }

    [Theory]
    [MemberData(nameof(ResourceCases))]
    public void Add_WithoutSelection_AndRepeatedly_UsesTheResourceGroup(string command, Type parentType, Type resourceType)
    {
        var editor = CreateEditor();
        editor.ClearSelection();
        var parent = Descendants(editor.ActiveProject!).Single(n => n.GetType() == parentType);
        var count = parent.Children.Count;

        Invoke(editor, command);
        var first = editor.SelectedItem!;
        Assert.IsType(resourceType, first);
        Invoke(editor, command);
        var second = editor.SelectedItem!;
        Assert.IsType(resourceType, second);

        // Folders nest under a selected folder; other resources are added beside the selected resource.
        var expectedParent = command == "AddFolder" ? first : parent;
        Assert.Same(expectedParent, second.Parent);
        Assert.Equal(count + (command == "AddFolder" ? 1 : 2), parent.Children.Count);
        Assert.NotSame(first, second);
        Assert.Same(second, Assert.Single(editor.SelectedItems));
        Assert.False(first.IsSelected);
    }

    [Theory]
    [InlineData("AddScript", "Script", typeof(ScriptGroupNode))]
    [InlineData("AddPlcProgram", "PlcProgram", typeof(PlcGroupNode))]
    [InlineData("AddScreen", "Screen", typeof(ScreenGroupNode))]
    public void Add_FromNestedResource_PreservesItsFolder(string command, string resourceKind, Type groupType)
    {
        var editor = CreateEditor();
        var group = editor.ActiveProject!.Children.Single(n => n.GetType() == groupType);
        var outer = new ResourceFolderNode("Outer", resourceKind) { Parent = group };
        var inner = new ResourceFolderNode("Inner", resourceKind) { Parent = outer };
        group.Children.Add(outer);
        outer.Children.Add(inner);
        editor.SelectNode(inner);

        Invoke(editor, command);
        var first = editor.SelectedItem!;
        Invoke(editor, command);
        var second = editor.SelectedItem!;

        Assert.Same(inner, first.Parent);
        Assert.Same(inner, second.Parent);
        Assert.Equal(new[] { first, second }, inner.Children);
    }

    [Theory]
    [InlineData("AddScript", typeof(ScriptGroupNode))]
    [InlineData("AddScreen", typeof(ScreenGroupNode))]
    [InlineData("AddVariable", typeof(VariableGroupNode))]
    public void Add_WithStaleSelectionFromAnotherProject_UsesActiveProject(string command, Type groupType)
    {
        var editor = CreateEditor();
        var firstProject = editor.ActiveProject!;
        var staleSelection = firstProject.Children.Single(n => n.GetType() == groupType);
        editor.NewFile();
        var activeProject = editor.ActiveProject!;
        editor.SelectNode(staleSelection);

        Invoke(editor, command);

        Assert.Same(activeProject, NodeEditorService.FindAncestorTopLevelProject(editor.SelectedItem));
        Assert.Empty(staleSelection.Children.Where(n => n.IsSelected));
        Assert.False(firstProject.IsActive);
    }

    [Theory]
    [InlineData("AddFolder", typeof(FolderNode))]
    [InlineData("AddVariable", typeof(VariableNode))]
    [InlineData("AddScript", typeof(ScriptNode))]
    [InlineData("AddPlcProgram", typeof(PlcProgramNode))]
    [InlineData("AddRecipe", typeof(RecipeNode))]
    [InlineData("AddScheduler", typeof(SchedulerNode))]
    [InlineData("AddReport", typeof(ReportNode))]
    [InlineData("AddCalculated", typeof(CalculatedNode))]
    [InlineData("AddScreen", typeof(ScreenNode))]
    public void ToolbarCommand_SelectsNewResourceExpandsAncestorsAndClearsSymbolSelection(string command, Type resourceType)
    {
        var editor = CreateEditor();
        editor.SelectNode(editor.ActiveProject!.Children.OfType<ServerSettingsNode>().Single());
        var symbol = new ScreenSymbol();
        editor.SetScreenSelection(symbol, [symbol]);
        foreach (var node in Descendants(editor.ActiveProject).Prepend(editor.ActiveProject))
            node.IsExpanded = false;
        var page = CreatePage(editor);

        Invoke(page, command);

        var created = editor.SelectedItem!;
        Assert.IsType(resourceType, created);
        Assert.Same(created, Assert.Single(editor.SelectedItems));
        Assert.True(created.IsSelected);
        for (var ancestor = created.Parent; ancestor != null; ancestor = ancestor.Parent)
            Assert.True(ancestor.IsExpanded, $"{ancestor.Name} must be expanded.");
        Assert.Null(editor.SelectedScreenSymbol);
        Assert.Empty(editor.SelectedScreenSymbols);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public void AddCommandAvailability_DependsOnOpenUnlockedProject(bool openProject, bool locked, bool expected)
    {
        var editor = new NodeEditorService();
        if (openProject)
        {
            editor.NewFile();
            editor.ActiveProject!.IsLocked = locked;
            editor.ClearSelection();
        }

        Assert.Equal(expected, Invoke(CreatePage(editor), "CanAddResource"));
    }

    [Theory]
    [MemberData(nameof(ResourceCases))]
    public void ContextCommand_ActivatesItsProjectAndSelectsNewResource(string command, Type parentType, Type resourceType)
    {
        var editor = CreateEditor();
        var project = editor.ActiveProject!;
        var context = project.Children.OfType<ServerSettingsNode>().Single();
        var parent = Descendants(project).Single(n => n.GetType() == parentType);
        editor.NewFile();
        var otherProject = editor.ActiveProject!;
        project.HasUnsavedChanges = false;
        otherProject.HasUnsavedChanges = false;

        Invoke(CreatePage(editor), "On" + command, context);

        Assert.IsType(resourceType, editor.SelectedItem);
        Assert.Same(project, editor.ActiveProject);
        Assert.Same(parent, editor.SelectedItem!.Parent);
        Assert.Same(editor.SelectedItem, Assert.Single(editor.SelectedItems));
        Assert.True(project.HasUnsavedChanges);
        Assert.False(otherProject.HasUnsavedChanges);
        Assert.True(project.IsExpanded);
    }

    [Fact]
    public void AddFolder_WithStaleScriptFolderSelection_UsesActiveProjectsVariables()
    {
        var editor = CreateEditor();
        var scripts = editor.ActiveProject!.Children.OfType<ScriptGroupNode>().Single();
        var folder = new ResourceFolderNode("Other project", "Script") { Parent = scripts };
        scripts.Children.Add(folder);
        editor.SelectNode(folder);
        editor.NewFile();
        var project = editor.ActiveProject!;

        Invoke(CreatePage(editor), "AddFolder");

        Assert.IsType<FolderNode>(editor.SelectedItem);
        Assert.Same(project, NodeEditorService.FindAncestorTopLevelProject(editor.SelectedItem));
        Assert.Empty(folder.Children);
    }

    [Theory]
    [InlineData("AddAiAgent", typeof(AiAgentPanel))]
    [InlineData("AddAutomationRule", typeof(AutomationRulePanel))]
    [InlineData("AddMcpConnection", typeof(McpConnectionPanel))]
    public async Task DedicatedPanel_RendersNewTreeSelectionsEditingForm(string command, Type panelType)
    {
        var editor = CreateEditor();
        Invoke(editor, command);
        switch (editor.SelectedItem)
        {
            case AiAgentNode agent: agent.Agent.Name = "Newly added resource"; break;
            case AutomationRuleNode rule: rule.Rule.Name = "Newly added resource"; break;
            case McpConnectionNode connection: connection.Connection.Name = "Newly added resource"; break;
        }
        using var services = new ServiceCollection().AddLogging().AddSingleton(editor).BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());

        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var result = await renderer.RenderComponentAsync(panelType, ParameterView.Empty);
            return result.ToHtmlString();
        });

        Assert.Contains("value=\"Newly added resource\"", html);
    }

    private static NodeEditorService CreateEditor()
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        return editor;
    }

    private static Home CreatePage(NodeEditorService editor)
    {
        var page = new Home();
        typeof(Home).GetProperty("Editor", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, editor);
        return page;
    }

    private static object? Invoke(object target, string command, params object?[] arguments)
        => target.GetType().GetMethod(command, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.Invoke(target, arguments);

    private static IEnumerable<TreeNode> Descendants(TreeNode node)
        => node.Children.SelectMany(child => new[] { child }.Concat(Descendants(child)));
}
