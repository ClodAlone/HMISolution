// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using ServerEditorWeb.Services;

namespace Tests.Editor;

public class UndoRedoServiceTests
{
    [Fact]
    public void InitialState_CannotUndoOrRedo()
    {
        var svc = new UndoRedoService();
        Assert.False(svc.CanUndo);
        Assert.False(svc.CanRedo);
        Assert.Equal(0, svc.UndoCount);
        Assert.Equal(0, svc.RedoCount);
    }

    [Fact]
    public void RecordAction_MakesUndoAvailable()
    {
        var svc = new UndoRedoService();
        int value = 0;

        svc.RecordAction(new CollectionAction
        {
            Description = "Set to 1",
            UndoCallback = () => value = 0,
            RedoCallback = () => value = 1
        });

        Assert.True(svc.CanUndo);
        Assert.False(svc.CanRedo);
        Assert.Equal(1, svc.UndoCount);
    }

    [Fact]
    public void Undo_ReversesAction()
    {
        var svc = new UndoRedoService();
        int value = 10;

        svc.RecordAction(new CollectionAction
        {
            Description = "Set to 20",
            UndoCallback = () => value = 10,
            RedoCallback = () => value = 20
        });

        svc.Undo();
        Assert.Equal(10, value);
        Assert.False(svc.CanUndo);
        Assert.True(svc.CanRedo);
    }

    [Fact]
    public void Redo_ReappliesAction()
    {
        var svc = new UndoRedoService();
        int value = 10;

        svc.RecordAction(new CollectionAction
        {
            Description = "Set to 20",
            UndoCallback = () => value = 10,
            RedoCallback = () => value = 20
        });

        svc.Undo();
        Assert.Equal(10, value);

        svc.Redo();
        Assert.Equal(20, value);
        Assert.True(svc.CanUndo);
        Assert.False(svc.CanRedo);
    }

    [Fact]
    public void NewAction_ClearsRedoStack()
    {
        var svc = new UndoRedoService();
        int value = 0;

        svc.RecordAction(new CollectionAction
        {
            Description = "A",
            UndoCallback = () => value = 0,
            RedoCallback = () => value = 1
        });
        svc.Undo();
        Assert.True(svc.CanRedo);

        svc.RecordAction(new CollectionAction
        {
            Description = "B",
            UndoCallback = () => value = 0,
            RedoCallback = () => value = 2
        });
        Assert.False(svc.CanRedo); // redo stack cleared
    }

    [Fact]
    public void MultipleUndo_WorksInOrder()
    {
        var svc = new UndoRedoService();
        var log = new List<string>();

        svc.RecordAction(new CollectionAction
        {
            Description = "Action 1",
            UndoCallback = () => log.Add("undo1"),
            RedoCallback = () => log.Add("redo1")
        });
        svc.RecordAction(new CollectionAction
        {
            Description = "Action 2",
            UndoCallback = () => log.Add("undo2"),
            RedoCallback = () => log.Add("redo2")
        });

        svc.Undo(); // undoes Action 2
        svc.Undo(); // undoes Action 1

        Assert.Equal(new[] { "undo2", "undo1" }, log);
        Assert.Equal(0, svc.UndoCount);
        Assert.Equal(2, svc.RedoCount);
    }

    [Fact]
    public void PropertyChangeAction_UndoRedo()
    {
        var target = new TestTarget { Name = "Original" };
        var prop = typeof(TestTarget).GetProperty(nameof(TestTarget.Name))!;

        var svc = new UndoRedoService();
        svc.RecordPropertyChange(target, prop, "Original", "Modified", "Rename");

        // Property was already changed externally; undo should restore
        target.Name = "Modified";
        svc.Undo();
        Assert.Equal("Original", target.Name);

        svc.Redo();
        Assert.Equal("Modified", target.Name);
    }

    [Fact]
    public void Clear_RemovesAllHistory()
    {
        var svc = new UndoRedoService();
        svc.RecordAction(new CollectionAction
        {
            Description = "X",
            UndoCallback = () => { },
            RedoCallback = () => { }
        });

        svc.Clear();
        Assert.False(svc.CanUndo);
        Assert.False(svc.CanRedo);
    }

    [Fact]
    public void StateChanged_FiresOnRecord()
    {
        var svc = new UndoRedoService();
        int fireCount = 0;
        svc.StateChanged += () => fireCount++;

        svc.RecordAction(new CollectionAction
        {
            Description = "X",
            UndoCallback = () => { },
            RedoCallback = () => { }
        });

        Assert.Equal(1, fireCount);
    }

    [Fact]
    public void StateChanged_FiresOnUndoAndRedo()
    {
        var svc = new UndoRedoService();
        svc.RecordAction(new CollectionAction
        {
            Description = "X",
            UndoCallback = () => { },
            RedoCallback = () => { }
        });

        int fireCount = 0;
        svc.StateChanged += () => fireCount++;

        svc.Undo();
        Assert.Equal(1, fireCount);

        svc.Redo();
        Assert.Equal(2, fireCount);
    }

    [Fact]
    public void UndoDescription_ReflectsTopAction()
    {
        var svc = new UndoRedoService();
        Assert.Null(svc.UndoDescription);

        svc.RecordAction(new CollectionAction
        {
            Description = "Delete Widget",
            UndoCallback = () => { },
            RedoCallback = () => { }
        });

        Assert.Equal("Delete Widget", svc.UndoDescription);
    }

    private class TestTarget
    {
        public string Name { get; set; } = "";
    }
}
