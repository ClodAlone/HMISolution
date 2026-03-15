using System;
using WpfApp3.Contracts;

using System.Collections.Generic;
using System.Linq;

namespace WpfApp3.Services
{
    public class SelectionService : ISelectionService
    {
        private static SelectionService? _instance;
        private static readonly object _lock = new();

        public static SelectionService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new SelectionService();
                    }
                }
                return _instance;
            }
        }

        private readonly List<object> _selectedObjects = new();

        public event Action<object?>? SelectionChanged;

        public object? CurrentSelection => _selectedObjects.LastOrDefault();
        public IEnumerable<object> SelectedObjects => _selectedObjects.AsReadOnly();

        public void Select(object? obj)
        {
            _selectedObjects.Clear();
            if (obj != null)
                _selectedObjects.Add(obj);
            SelectionChanged?.Invoke(CurrentSelection);
        }

        public void AddToSelection(object obj)
        {
            if (obj != null && !_selectedObjects.Contains(obj))
            {
                _selectedObjects.Add(obj);
                SelectionChanged?.Invoke(CurrentSelection);
            }
        }

        public void RemoveFromSelection(object obj)
        {
            if (obj != null && _selectedObjects.Remove(obj))
            {
                SelectionChanged?.Invoke(CurrentSelection);
            }
        }

        public void ClearSelection()
        {
            if (_selectedObjects.Any())
            {
                _selectedObjects.Clear();
                SelectionChanged?.Invoke(null);
            }
        }
    }
}
