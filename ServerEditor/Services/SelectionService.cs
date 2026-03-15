using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerEditor.Services
{
    public class SelectionService
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
    }
}