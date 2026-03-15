using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace WpfApp3.Plugins
{
    public partial class ToolboxControl : UserControl
    {
        public ObservableCollection<ToolItem> Tools { get; } = new ObservableCollection<ToolItem>();

        public ToolboxControl()
        {
            InitializeComponent();
            ToolsList.ItemsSource = Tools;

            if (IsLoaded)
            {
                ToolboxControl_Loaded(this, new RoutedEventArgs());
            }

            Loaded += ToolboxControl_Loaded;
        }

        private void ToolboxControl_Loaded(object sender, RoutedEventArgs e)
        {
            Tools.Clear();
            var componentsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Components");
            if (Directory.Exists(componentsDir))
            {
                var files = Directory.GetFiles(componentsDir, "*.xaml");
                foreach (var file in files)
                {
                    try
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        var content = File.ReadAllText(file);
                        UIElement? visual = null;
                        try
                        {
                            visual = (UIElement)XamlReader.Parse(content);
                            // Avoid capturing mouse events on the preview
                            visual.IsHitTestVisible = false;
                        }
                        catch { }

                        Tools.Add(new ToolItem { Name = name, Content = content, Visual = visual });
                    }
                    catch { }
                }
            }
        }

        private void Tool_Click(object sender, RoutedEventArgs e)
        {
            // Click handler currently does nothing because drag starts on MouseDown
        }

        private Point _startPoint;
        private bool _isDragging;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _isDragging = false;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (!_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var element = FindAncestor<Button>((DependencyObject)e.OriginalSource);
                    if (element != null && element.Tag is string xamlContent)
                    {
                         _isDragging = true;
                         // Pass a specific format or check in Drop handler
                         var data = new DataObject(DataFormats.StringFormat, xamlContent);
                         data.SetData("XamlContent", xamlContent);
                         DragDrop.DoDragDrop(element, data, DragDropEffects.Copy);
                    }
                }
            }
        }

        private static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T ancestor) return ancestor;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }

    public class ToolItem
    {
        public string Name { get; set; } = "";
        public string Content { get; set; } = "";
        public UIElement? Visual { get; set; }
    }
}
