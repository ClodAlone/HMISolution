using System;

using System.Collections.Generic;

namespace WpfApp3.Contracts
{
    public interface ISelectionService
    {
        event Action<object?> SelectionChanged;
        object? CurrentSelection { get; }
        IEnumerable<object> SelectedObjects { get; }
        void Select(object? obj);
        void AddToSelection(object obj);
        void RemoveFromSelection(object obj);
        void ClearSelection();
    }
}
