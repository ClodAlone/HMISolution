using Xunit;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;

namespace Tests.Editor;

public class NodeEditorServiceTests
{
    private static NodeEditorService CreateServiceWithModel()
    {
        var svc = new NodeEditorService();
        // Use NewFile to get a fresh model with standard structure
        svc.NewFile();
        return svc;
    }

    [Fact]
    public void NewFile_CreatesModel()
    {
        var svc = CreateServiceWithModel();
        Assert.NotNull(svc.RootModel);
        Assert.True(svc.HasUnsavedChanges);
        Assert.True(svc.RootItems.Count > 0);
    }

    [Fact]
    public void DeleteSelected_RemovesTreeNode()
    {
        var svc = CreateServiceWithModel();
        var undo = new UndoRedoService();
        svc.SetUndoService(undo);

        // Find a folder node and add a variable to it
        var folderNode = FindFirstOfType<FolderNode>(svc.RootItems);
        Assert.NotNull(folderNode);

        var variable = new Variable { Name = "TestVar", Type = "Double" };
        var varNode = new VariableNode(variable) { Parent = folderNode };
        folderNode.Children.Add(varNode);

        // Select and delete
        svc.SelectedItem = varNode;
        svc.SelectedItems.Add(varNode);
        varNode.IsSelected = true;

        svc.DeleteSelected();

        Assert.DoesNotContain(varNode, folderNode.Children);
        Assert.Null(svc.SelectedItem);
    }

    [Fact]
    public void DeleteSelected_IsUndoable()
    {
        var svc = CreateServiceWithModel();
        var undo = new UndoRedoService();
        svc.SetUndoService(undo);

        var folderNode = FindFirstOfType<FolderNode>(svc.RootItems);
        Assert.NotNull(folderNode);

        var variable = new Variable { Name = "UndoVar", Type = "Int32" };
        var varNode = new VariableNode(variable) { Parent = folderNode };
        folderNode.Children.Add(varNode);
        var childCountBefore = folderNode.Children.Count;

        svc.SelectedItem = varNode;
        svc.SelectedItems.Add(varNode);
        varNode.IsSelected = true;

        svc.DeleteSelected();
        Assert.Equal(childCountBefore - 1, folderNode.Children.Count);

        // Undo should restore
        Assert.True(undo.CanUndo);
        undo.Undo();
        Assert.Equal(childCountBefore, folderNode.Children.Count);
        Assert.Contains(varNode, folderNode.Children);
    }

    [Fact]
    public void DeleteSelected_Redo_RemovesAgain()
    {
        var svc = CreateServiceWithModel();
        var undo = new UndoRedoService();
        svc.SetUndoService(undo);

        var folderNode = FindFirstOfType<FolderNode>(svc.RootItems);
        Assert.NotNull(folderNode);

        var variable = new Variable { Name = "RedoVar", Type = "Boolean" };
        var varNode = new VariableNode(variable) { Parent = folderNode };
        folderNode.Children.Add(varNode);

        svc.SelectedItem = varNode;
        svc.SelectedItems.Add(varNode);
        varNode.IsSelected = true;

        svc.DeleteSelected();
        undo.Undo();
        Assert.Contains(varNode, folderNode.Children);

        undo.Redo();
        Assert.DoesNotContain(varNode, folderNode.Children);
    }

    [Fact]
    public void DeleteSelected_MultipleItems()
    {
        var svc = CreateServiceWithModel();
        var undo = new UndoRedoService();
        svc.SetUndoService(undo);

        var folderNode = FindFirstOfType<FolderNode>(svc.RootItems);
        Assert.NotNull(folderNode);

        var var1 = new VariableNode(new Variable { Name = "V1", Type = "Double" }) { Parent = folderNode };
        var var2 = new VariableNode(new Variable { Name = "V2", Type = "Double" }) { Parent = folderNode };
        folderNode.Children.Add(var1);
        folderNode.Children.Add(var2);

        svc.SelectedItems.Add(var1);
        svc.SelectedItems.Add(var2);
        var1.IsSelected = true;
        var2.IsSelected = true;

        svc.DeleteSelected();

        Assert.DoesNotContain(var1, folderNode.Children);
        Assert.DoesNotContain(var2, folderNode.Children);

        undo.Undo();
        Assert.Contains(var1, folderNode.Children);
        Assert.Contains(var2, folderNode.Children);
    }

    [Fact]
    public void DeleteSelected_RootNode_NotDeleted()
    {
        var svc = CreateServiceWithModel();
        var root = svc.RootItems.First();

        svc.SelectedItem = root;
        svc.SelectedItems.Add(root);

        svc.DeleteSelected();

        // Root nodes have Parent == null, so they should not be deleted
        Assert.Contains(root, svc.RootItems);
    }

    private static T? FindFirstOfType<T>(IEnumerable<TreeNode> nodes) where T : TreeNode
    {
        foreach (var node in nodes)
        {
            if (node is T match) return match;
            var child = FindFirstOfType<T>(node.Children);
            if (child != null) return child;
        }
        return null;
    }
}
