using System.Reflection;
using System.Text.Json;

namespace ServerEditorWeb.Services;

/// <summary>
/// Represents a single undoable/redoable action.
/// </summary>
public abstract class UndoAction
{
    public string Description { get; init; } = "";
    public abstract void Undo();
    public abstract void Redo();
}

/// <summary>
/// Undo action for a property value change on a specific object.
/// </summary>
public class PropertyChangeAction : UndoAction
{
    public object Target { get; init; } = default!;
    public PropertyInfo Property { get; init; } = default!;
    public object? OldValue { get; init; }
    public object? NewValue { get; init; }

    public override void Undo() => Property.SetValue(Target, OldValue);
    public override void Redo() => Property.SetValue(Target, NewValue);
}

/// <summary>
/// Undo action that restores an entire object from a JSON snapshot (for bulk/complex changes).
/// </summary>
public class SnapshotAction : UndoAction
{
    public object Target { get; init; } = default!;
    public string OldJson { get; init; } = "";
    public string NewJson { get; init; } = "";

    public override void Undo() => RestoreSnapshot(OldJson);
    public override void Redo() => RestoreSnapshot(NewJson);

    private void RestoreSnapshot(string json)
    {
        var type = Target.GetType();
        var restored = JsonSerializer.Deserialize(json, type);
        if (restored == null) return;

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanWrite) continue;
            try { prop.SetValue(Target, prop.GetValue(restored)); }
            catch { }
        }
    }
}

/// <summary>
/// Undo action for adding/removing a child from a list.
/// </summary>
public class CollectionAction : UndoAction
{
    public required Action UndoCallback { get; init; }
    public required Action RedoCallback { get; init; }

    public override void Undo() => UndoCallback();
    public override void Redo() => RedoCallback();
}

/// <summary>
/// Groups multiple undo actions into a single undoable operation.
/// </summary>
public class CompoundAction : UndoAction
{
    public List<UndoAction> Actions { get; init; } = [];

    public override void Undo()
    {
        for (int i = Actions.Count - 1; i >= 0; i--)
            Actions[i].Undo();
    }

    public override void Redo()
    {
        foreach (var action in Actions)
            action.Redo();
    }
}

/// <summary>
/// Central undo/redo service for the editor.
/// Tracks property changes, structural changes, and supports Ctrl+Z / Ctrl+Y.
/// </summary>
public class UndoRedoService
{
    private readonly Stack<UndoAction> _undoStack = new();
    private readonly Stack<UndoAction> _redoStack = new();
    private const int MaxHistory = 200;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public string? UndoDescription => _undoStack.TryPeek(out var a) ? a.Description : null;
    public string? RedoDescription => _redoStack.TryPeek(out var a) ? a.Description : null;

    public int UndoCount => _undoStack.Count;
    public int RedoCount => _redoStack.Count;

    public event Action? StateChanged;

    /// <summary>
    /// Records a property value change for undo/redo.
    /// </summary>
    public void RecordPropertyChange(object target, PropertyInfo property, object? oldValue, object? newValue, string? description = null)
    {
        if (Equals(oldValue, newValue)) return;

        var action = new PropertyChangeAction
        {
            Target = target,
            Property = property,
            OldValue = oldValue,
            NewValue = newValue,
            Description = description ?? $"Change {property.Name}"
        };

        Push(action);
    }

    /// <summary>
    /// Records a snapshot-based change (for complex/bulk edits).
    /// </summary>
    public void RecordSnapshot(object target, string oldJson, string newJson, string description)
    {
        if (oldJson == newJson) return;

        var action = new SnapshotAction
        {
            Target = target,
            OldJson = oldJson,
            NewJson = newJson,
            Description = description
        };

        Push(action);
    }

    /// <summary>
    /// Records a custom undoable action.
    /// </summary>
    public void RecordAction(UndoAction action)
    {
        Push(action);
    }

    /// <summary>
    /// Undoes the last action. Returns true if an action was undone.
    /// </summary>
    public bool Undo()
    {
        if (_undoStack.Count == 0) return false;

        var action = _undoStack.Pop();
        action.Undo();
        _redoStack.Push(action);
        StateChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Redoes the last undone action. Returns true if an action was redone.
    /// </summary>
    public bool Redo()
    {
        if (_redoStack.Count == 0) return false;

        var action = _redoStack.Pop();
        action.Redo();
        _undoStack.Push(action);
        StateChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Clears all undo/redo history.
    /// </summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        StateChanged?.Invoke();
    }

    private void Push(UndoAction action)
    {
        _undoStack.Push(action);
        _redoStack.Clear();

        // Trim history
        if (_undoStack.Count > MaxHistory)
        {
            var temp = _undoStack.ToArray();
            _undoStack.Clear();
            for (int i = Math.Min(temp.Length - 1, MaxHistory - 1); i >= 0; i--)
                _undoStack.Push(temp[i]);
        }

        StateChanged?.Invoke();
    }
}
