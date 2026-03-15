using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using WpfApp3.Contracts;
using WpfApp3.Services;

namespace WpfApp3.Plugins
{
    public partial class PropertyGridControl : UserControl
    {
        private readonly ISelectionService _selectionService;

        public ObservableCollection<PropertyItem> Properties { get; } = new();

        public PropertyGridControl()
        {
            InitializeComponent();
            _selectionService = SelectionService.Instance;

            // Subscribe immediately in case Loaded is not raised
            _selectionService.SelectionChanged += OnSelectionChanged;
            OnSelectionChanged(_selectionService.CurrentSelection);

            Loaded += (s, e) => 
            {
                _selectionService.SelectionChanged -= OnSelectionChanged;
                _selectionService.SelectionChanged += OnSelectionChanged;
                OnSelectionChanged(_selectionService.CurrentSelection);
            };
            Unloaded += (s, e) => _selectionService.SelectionChanged -= OnSelectionChanged;
            PropertiesGrid.ItemsSource = Properties;
        }

        private void OnSelectionChanged(object? obj)
        {
            Dispatcher.Invoke(() =>
            {
                Properties.Clear();
                if (obj == null) return;

                var type = obj.GetType();
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in props)
                {
                    if (prop.CanRead)
                    {
                        try
                        {
                            var val = prop.GetValue(obj);
                            Properties.Add(new PropertyItem
                            {
                                Name = prop.Name,
                                Value = val?.ToString() ?? "null",
                                Type = prop.PropertyType.Name,
                                Category = "General" // Simplified
                            });
                        }
                        catch { }
                    }
                }
            });
        }
    }

    public class PropertyItem
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public string Type { get; set; } = "";
        public string Category { get; set; } = "";
    }
}
