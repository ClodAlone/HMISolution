using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace WpfApp3.Plugins
{
    public partial class GraphicEditorControl : UserControl
    {
        private bool _isDragging;
        private Point _lastDragPoint;
        private UIElement? _draggedElement;

        private bool _isSelectionDragging;
        private Point _selectionStartPoint;
        private Rectangle? _selectionRectangle;

        public GraphicEditorControl()
        {
            InitializeComponent();

            if (IsLoaded)
            {
                GraphicEditorControl_Loaded(this, new RoutedEventArgs());
            }

            Loaded += GraphicEditorControl_Loaded;
            Unloaded += GraphicEditorControl_Unloaded;
        }

        private void GraphicEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            WpfApp3.Services.SelectionService.Instance.SelectionChanged += OnSelectionChanged;
        }

        private void GraphicEditorControl_Unloaded(object sender, RoutedEventArgs e)
        {
            WpfApp3.Services.SelectionService.Instance.SelectionChanged -= OnSelectionChanged;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "XAML Files (*.xaml)|*.xaml";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Read file content as string to avoid encoding issues (e.g. header mismatch)
                    var xamlContent = System.IO.File.ReadAllText(dialog.FileName);
                    var obj = XamlReader.Parse(xamlContent);

                    if (obj is Canvas loadedCanvas)
                    {
                        DesignCanvas.Children.Clear();
                        WpfApp3.Services.SelectionService.Instance.ClearSelection();

                        var children = new List<UIElement>();
                        foreach (UIElement child in loadedCanvas.Children)
                        {
                            children.Add(child);
                        }

                        loadedCanvas.Children.Clear(); // Detach

                        foreach (var child in children)
                        {
                            // Re-attach events
                            child.PreviewMouseLeftButtonDown += Element_MouseLeftButtonDown;
                            child.PreviewMouseMove += Element_MouseMove;
                            child.PreviewMouseLeftButtonUp += Element_MouseLeftButtonUp;

                            DesignCanvas.Children.Add(child);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}");
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "XAML Files (*.xaml)|*.xaml";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Use stream with XmlWriter to ensure encoding matches the file content
                    var settings = new XmlWriterSettings { Indent = true };
                    using var stream = System.IO.File.Create(dialog.FileName);
                    using var writer = XmlWriter.Create(stream, settings);

                    XamlWriter.Save(DesignCanvas, writer);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}");
                }
            }
        }

        private void ExportSVG_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "SVG Files (*.svg)|*.svg";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var settings = new XmlWriterSettings { Indent = true };
                    using var writer = XmlWriter.Create(dialog.FileName, settings);

                    writer.WriteStartDocument();
                    writer.WriteStartElement("svg", "http://www.w3.org/2000/svg");
                    writer.WriteAttributeString("width", DesignCanvas.Width.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("height", DesignCanvas.Height.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("version", "1.1");

                    foreach (UIElement child in DesignCanvas.Children)
                    {
                        WriteElementToSvg(writer, child);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting SVG: {ex.Message}");
                }
            }
        }

        private void WriteElementToSvg(XmlWriter writer, UIElement element)
        {
            if (element is FrameworkElement fe)
            {
                if (!fe.IsHitTestVisible || (fe is Rectangle && fe == _selectionRectangle)) return;

                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                double width = fe.ActualWidth;
                double height = fe.ActualHeight;

                if (width <= 0 || height <= 0) return;

                if (element is Rectangle rect)
                {
                    writer.WriteStartElement("rect");
                    WriteRectAttributes(writer, left, top, width, height, rect.Fill, rect.Stroke, rect.StrokeThickness);
                    writer.WriteEndElement();
                }
                else if (element is Ellipse el)
                {
                    writer.WriteStartElement("ellipse");
                    double cx = left + width / 2;
                    double cy = top + height / 2;
                    double rx = width / 2;
                    double ry = height / 2;
                    writer.WriteAttributeString("cx", cx.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("cy", cy.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("rx", rx.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("ry", ry.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    WriteBrushAttributes(writer, el.Fill, "fill");
                    WriteBrushAttributes(writer, el.Stroke, "stroke");
                    writer.WriteAttributeString("stroke-width", el.StrokeThickness.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                else if (element is TextBlock tb)
                {
                    writer.WriteStartElement("text");
                    writer.WriteAttributeString("x", left.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    // SVG text Y is baseline approximately. Adjusting by FontSize is a heuristic.
                    // Ideally we measure baseline offset. 
                    // Using top + height - descent might be better, or just top + fontsize as simplified baseline.
                    writer.WriteAttributeString("y", (top + tb.FontSize * 0.8).ToString(System.Globalization.CultureInfo.InvariantCulture));

                    WriteBrushAttributes(writer, tb.Foreground, "fill");
                    writer.WriteAttributeString("font-size", tb.FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("font-family", tb.FontFamily.ToString());
                    writer.WriteString(tb.Text);
                    writer.WriteEndElement();
                }
                else if (element is Button btn)
                {
                    writer.WriteStartElement("g");

                    // Background
                    writer.WriteStartElement("rect");
                    WriteRectAttributes(writer, left, top, width, height, btn.Background, btn.BorderBrush, btn.BorderThickness.Left, 4, 4);
                    writer.WriteEndElement();

                    // Text Content
                    // Assuming content is string. If content is complex, we might skip or simplistic export.
                    writer.WriteStartElement("text");
                    writer.WriteAttributeString("x", (left + width / 2).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("y", (top + height / 2).ToString(System.Globalization.CultureInfo.InvariantCulture));

                    WriteBrushAttributes(writer, btn.Foreground, "fill");
                    writer.WriteAttributeString("font-size", btn.FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("font-family", btn.FontFamily.ToString());
                    writer.WriteAttributeString("text-anchor", "middle");
                    writer.WriteAttributeString("dominant-baseline", "middle");
                    writer.WriteString(btn.Content?.ToString() ?? "");
                    writer.WriteEndElement();

                    writer.WriteEndElement(); // g
                }
                else if (element is TextBox txt)
                {
                    writer.WriteStartElement("g");

                    // Background
                    writer.WriteStartElement("rect");
                    WriteRectAttributes(writer, left, top, width, height, txt.Background, txt.BorderBrush, txt.BorderThickness.Left);
                    writer.WriteEndElement();

                    // Text
                    writer.WriteStartElement("text");
                    writer.WriteAttributeString("x", (left + 2).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("y", (top + height / 2).ToString(System.Globalization.CultureInfo.InvariantCulture));

                    WriteBrushAttributes(writer, txt.Foreground, "fill");
                    writer.WriteAttributeString("font-size", txt.FontSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("font-family", txt.FontFamily.ToString());
                    writer.WriteAttributeString("dominant-baseline", "middle");
                    // Clip text if it overflows? SVG doesn't auto-clip text in simple cases.
                    writer.WriteString(txt.Text);
                    writer.WriteEndElement();

                    writer.WriteEndElement(); // g
                }
                else if (element is Viewbox vb && vb.Child is Canvas groupCanvas)
                {
                    writer.WriteStartElement("g");

                    double scaleX = width / groupCanvas.Width;
                    double scaleY = height / groupCanvas.Height;
                    if (double.IsNaN(scaleX)) scaleX = 1;
                    if (double.IsNaN(scaleY)) scaleY = 1;

                    string transform = string.Format(System.Globalization.CultureInfo.InvariantCulture, 
                        "translate({0},{1}) scale({2},{3})", left, top, scaleX, scaleY);

                    writer.WriteAttributeString("transform", transform);

                    foreach (UIElement child in groupCanvas.Children)
                    {
                        WriteElementToSvg(writer, child);
                    }

                    writer.WriteEndElement(); // g
                }
            }
        }

        private void WriteRectAttributes(XmlWriter writer, double x, double y, double width, double height, Brush fill, Brush stroke, double strokeThickness, double rx = 0, double ry = 0)
        {
            writer.WriteAttributeString("x", x.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("y", y.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("width", width.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("height", height.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (rx > 0) writer.WriteAttributeString("rx", rx.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (ry > 0) writer.WriteAttributeString("ry", ry.ToString(System.Globalization.CultureInfo.InvariantCulture));

            WriteBrushAttributes(writer, fill, "fill");
            WriteBrushAttributes(writer, stroke, "stroke");
            writer.WriteAttributeString("stroke-width", strokeThickness.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        private void WriteBrushAttributes(XmlWriter writer, Brush brush, string attributeName)
        {
            if (brush is SolidColorBrush scb)
            {
                var color = scb.Color;
                string colorHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                writer.WriteAttributeString(attributeName, colorHex);
                if (color.A < 255)
                {
                    writer.WriteAttributeString(attributeName + "-opacity", (color.A / 255.0).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            else
            {
                writer.WriteAttributeString(attributeName, "none");
            }
        }

        private string BrushToString(Brush brush)
        {
            // Legacy helper kept if referenced elsewhere, but WriteBrushAttributes handles it now.
            if (brush is SolidColorBrush scb)
            {
                return scb.Color.ToString();
            }
            return "none";
        }

        private void ImportSVG_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "SVG Files (*.svg)|*.svg";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(dialog.FileName);

                    if (doc.DocumentElement?.NamespaceURI == "http://www.w3.org/2000/svg")
                    {
                        DesignCanvas.Children.Clear();
                        WpfApp3.Services.SelectionService.Instance.ClearSelection();

                        foreach (XmlNode child in doc.DocumentElement.ChildNodes)
                        {
                            UIElement? newElement = null;
                            if (child.Name == "rect")
                            {
                                var rect = new Rectangle();
                                double x = GetAttribute(child, "x");
                                double y = GetAttribute(child, "y");
                                rect.Width = GetAttribute(child, "width");
                                rect.Height = GetAttribute(child, "height");
                                rect.Fill = GetBrush(child, "fill");
                                rect.Stroke = GetBrush(child, "stroke");
                                rect.StrokeThickness = GetAttribute(child, "stroke-width", 1);
                                Canvas.SetLeft(rect, x);
                                Canvas.SetTop(rect, y);
                                newElement = rect;
                            }
                            else if (child.Name == "ellipse")
                            {
                                var el = new Ellipse();
                                double cx = GetAttribute(child, "cx");
                                double cy = GetAttribute(child, "cy");
                                double rx = GetAttribute(child, "rx");
                                double ry = GetAttribute(child, "ry");
                                el.Width = rx * 2;
                                el.Height = ry * 2;
                                el.Fill = GetBrush(child, "fill");
                                el.Stroke = GetBrush(child, "stroke");
                                el.StrokeThickness = GetAttribute(child, "stroke-width", 1);
                                Canvas.SetLeft(el, cx - rx);
                                Canvas.SetTop(el, cy - ry);
                                newElement = el;
                            }
                            else if (child.Name == "text")
                            {
                                var tb = new TextBlock();
                                double x = GetAttribute(child, "x");
                                double y = GetAttribute(child, "y");
                                tb.FontSize = GetAttribute(child, "font-size", 12);
                                tb.Foreground = GetBrush(child, "fill");
                                tb.Text = child.InnerText;
                                Canvas.SetLeft(tb, x);
                                Canvas.SetTop(tb, y - tb.FontSize);
                                newElement = tb;
                            }

                            if (newElement != null)
                            {
                                newElement.PreviewMouseLeftButtonDown += Element_MouseLeftButtonDown;
                                newElement.PreviewMouseMove += Element_MouseMove;
                                newElement.PreviewMouseLeftButtonUp += Element_MouseLeftButtonUp;
                                DesignCanvas.Children.Add(newElement);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing SVG: {ex.Message}");
                }
            }
        }

        private double GetAttribute(XmlNode node, string name, double defaultValue = 0)
        {
            if (node.Attributes?[name] != null)
            {
                if (double.TryParse(node.Attributes[name].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
                    return val;
            }
            return defaultValue;
        }

        private Brush GetBrush(XmlNode node, string name)
        {
            if (node.Attributes?[name] != null)
            {
                string val = node.Attributes[name].Value;
                if (val == "none") return Brushes.Transparent;
                try
                {
                    return (Brush?)new BrushConverter().ConvertFrom(val) ?? Brushes.Transparent;
                }
                catch { }
            }
            return name == "fill" ? Brushes.Black : Brushes.Transparent;
        }

        private Dictionary<UIElement, Adorner> _adorners = new Dictionary<UIElement, Adorner>();

        private void OnSelectionChanged(object? obj)
        {
            UpdateAdorners();
        }

        private void UpdateAdorners()
        {
            var layer = AdornerLayer.GetAdornerLayer(DesignCanvas);
            if (layer == null) return;

            var selectionService = WpfApp3.Services.SelectionService.Instance;
            var selectedElements = selectionService.SelectedObjects.OfType<UIElement>().ToList();

            // Remove stale adorners
            var toRemove = _adorners.Keys.Where(k => !selectedElements.Contains(k)).ToList();
            foreach (var el in toRemove)
            {
                layer.Remove(_adorners[el]);
                _adorners.Remove(el);
            }

            // Add new adorners
            foreach (var el in selectedElements)
            {
                if (!_adorners.ContainsKey(el) && DesignCanvas.Children.Contains(el))
                {
                    var adorner = new ResizeRotateAdorner(el);
                    layer.Add(adorner);
                    _adorners[el] = adorner;
                }
            }
        }

        private void DesignCanvas_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void DesignCanvas_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.StringFormat))
                {
                    string content = (string)e.Data.GetData(DataFormats.StringFormat);
                    Point position = e.GetPosition(DesignCanvas);

                    UIElement? newElement = null;

                    // Try parsing as XAML first
                    if (content.TrimStart().StartsWith("<"))
                    {
                        try
                        {
                            newElement = (UIElement)XamlReader.Parse(content);
                        }
                        catch (Exception ex)
                        {
                            // Could fail if missing namespaces or something
                             System.Diagnostics.Debug.WriteLine($"XAML parse error: {ex.Message}");
                        }
                    }

                    // Fallback to old tool types if not XAML (for backward compatibility if Toolbox sends simple strings)
                    if (newElement == null)
                    {
                        switch (content)
                        {
                            case "Rectangle":
                                newElement = new Rectangle { Width = 100, Height = 50, Fill = Brushes.LightBlue, Stroke = Brushes.Black, StrokeThickness = 1 };
                                break;
                            case "Ellipse":
                                newElement = new Ellipse { Width = 80, Height = 80, Fill = Brushes.LightGreen, Stroke = Brushes.Black, StrokeThickness = 1 };
                                break;
                            case "Text":
                                newElement = new TextBlock { Text = "New Text", FontSize = 16, Foreground = Brushes.White };
                                break;
                            case "Button":
                                newElement = new Button { Content = "Button", Width = 80, Height = 30 };
                                break;
                        }
                    }

                    if (newElement != null)
                    {
                        Canvas.SetLeft(newElement, position.X);
                        Canvas.SetTop(newElement, position.Y);
                        DesignCanvas.Children.Add(newElement);

                        newElement.PreviewMouseLeftButtonDown += Element_MouseLeftButtonDown;
                        newElement.PreviewMouseMove += Element_MouseMove;
                        newElement.PreviewMouseLeftButtonUp += Element_MouseLeftButtonUp;

                        WpfApp3.Services.SelectionService.Instance.Select(newElement);
                    }
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"Error dropping item: {ex.Message}");
            }
            finally
            {
                e.Handled = true;
            }
        }

        private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement element)
            {
                var selectionService = WpfApp3.Services.SelectionService.Instance;

                // Check for modifiers (Ctrl for multi-select)
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    if (selectionService.SelectedObjects.Contains(element))
                    {
                        selectionService.RemoveFromSelection(element);
                        // If unselected, do not drag
                        return;
                    }
                    else
                    {
                        selectionService.AddToSelection(element);
                    }
                }
                else
                {
                    // If simple click and not already selected, clear others
                    if (!selectionService.SelectedObjects.Contains(element))
                    {
                        selectionService.Select(element);
                    }
                }

                // If element is now selected (it should be unless we just deselected it)
                if (selectionService.SelectedObjects.Contains(element))
                {
                    _isDragging = true;
                    _draggedElement = element;
                    _lastDragPoint = e.GetPosition(DesignCanvas);
                    _draggedElement.CaptureMouse();
                }

                e.Handled = true;
            }
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedElement != null)
            {
                Point currentPosition = e.GetPosition(DesignCanvas);
                double deltaX = currentPosition.X - _lastDragPoint.X;
                double deltaY = currentPosition.Y - _lastDragPoint.Y;

                foreach (var item in WpfApp3.Services.SelectionService.Instance.SelectedObjects.OfType<UIElement>())
                {
                    double oldLeft = Canvas.GetLeft(item);
                    double oldTop = Canvas.GetTop(item);
                    if (double.IsNaN(oldLeft)) oldLeft = 0;
                    if (double.IsNaN(oldTop)) oldTop = 0;

                    Canvas.SetLeft(item, oldLeft + deltaX);
                    Canvas.SetTop(item, oldTop + deltaY);
                }

                _lastDragPoint = currentPosition;
            }
        }

        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && _draggedElement != null)
            {
                _isDragging = false;
                _draggedElement.ReleaseMouseCapture();
                _draggedElement = null;
            }
        }

        private void DesignCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == DesignCanvas)
            {
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    WpfApp3.Services.SelectionService.Instance.ClearSelection();

                _isSelectionDragging = true;
                _selectionStartPoint = e.GetPosition(DesignCanvas);

                _selectionRectangle = new Rectangle
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)),
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(_selectionRectangle, _selectionStartPoint.X);
                Canvas.SetTop(_selectionRectangle, _selectionStartPoint.Y);
                _selectionRectangle.Width = 0;
                _selectionRectangle.Height = 0;

                DesignCanvas.Children.Add(_selectionRectangle);
                DesignCanvas.CaptureMouse();

                e.Handled = true;
            }
        }

        private void DesignCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSelectionDragging && _selectionRectangle != null)
            {
                Point currentPoint = e.GetPosition(DesignCanvas);

                double x = Math.Min(currentPoint.X, _selectionStartPoint.X);
                double y = Math.Min(currentPoint.Y, _selectionStartPoint.Y);
                double w = Math.Abs(currentPoint.X - _selectionStartPoint.X);
                double h = Math.Abs(currentPoint.Y - _selectionStartPoint.Y);

                Canvas.SetLeft(_selectionRectangle, x);
                Canvas.SetTop(_selectionRectangle, y);
                _selectionRectangle.Width = w;
                _selectionRectangle.Height = h;
            }
        }

        private void DesignCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSelectionDragging && _selectionRectangle != null)
            {
                // Finalize selection
                Rect selectionRect = new Rect(
                    Canvas.GetLeft(_selectionRectangle),
                    Canvas.GetTop(_selectionRectangle),
                    _selectionRectangle.Width,
                    _selectionRectangle.Height);

                var selectionService = WpfApp3.Services.SelectionService.Instance;

                // If simple click (small area), we might have already cleared selection in Down.
                // If we want detailed HitTest we can do it.

                if (selectionRect.Width > 0 && selectionRect.Height > 0)
                {
                    foreach (UIElement child in DesignCanvas.Children)
                    {
                        if (child == _selectionRectangle) continue;
                        if (!child.IsHitTestVisible) continue; // Skip adorners if any or non-interactive

                        // Get bounds of child
                        double left = Canvas.GetLeft(child);
                        double top = Canvas.GetTop(child);
                        if (double.IsNaN(left)) left = 0;
                        if (double.IsNaN(top)) top = 0;

                        if (child is FrameworkElement fe)
                        {
                            double w = fe.ActualWidth;
                            double h = fe.ActualHeight;
                            if (double.IsNaN(w) || w == 0) w = fe.Width;
                            if (double.IsNaN(h) || h == 0) h = fe.Height;

                            Rect childRect = new Rect(left, top, w, h);

                            if (selectionRect.IntersectsWith(childRect))
                            {
                                selectionService.AddToSelection(child);
                            }
                        }
                    }
                }

                DesignCanvas.Children.Remove(_selectionRectangle);
                _selectionRectangle = null;
                _isSelectionDragging = false;
                DesignCanvas.ReleaseMouseCapture();
            }
        }

        private void Group_Click(object sender, RoutedEventArgs e)
        {
            var selectionService = WpfApp3.Services.SelectionService.Instance;
            var selectedItems = selectionService.SelectedObjects.OfType<UIElement>().ToList();
            if (selectedItems.Count < 2) return;

            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (var item in selectedItems)
            {
                double left = Canvas.GetLeft(item);
                double top = Canvas.GetTop(item);
                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                if (item is FrameworkElement fe)
                {
                    double w = fe.Width;
                    if (double.IsNaN(w)) w = fe.ActualWidth;
                    double h = fe.Height;
                    if (double.IsNaN(h)) h = fe.ActualHeight;

                    minX = Math.Min(minX, left);
                    minY = Math.Min(minY, top);
                    maxX = Math.Max(maxX, left + w);
                    maxY = Math.Max(maxY, top + h);
                }
            }

            var groupCanvas = new Canvas
            {
                Width = Math.Max(1, maxX - minX),
                Height = Math.Max(1, maxY - minY),
                Background = Brushes.Transparent 
            };

            // Reposition children relative to group canvas (0,0)
            foreach (var item in selectedItems)
            {
                DesignCanvas.Children.Remove(item);

                double left = Canvas.GetLeft(item);
                double top = Canvas.GetTop(item);
                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                Canvas.SetLeft(item, left - minX);
                Canvas.SetTop(item, top - minY);

                groupCanvas.Children.Add(item);
            }

            var viewbox = new Viewbox
            {
                Stretch = Stretch.Fill,
                Width = groupCanvas.Width, 
                Height = groupCanvas.Height
            };

            viewbox.Child = groupCanvas;

            Canvas.SetLeft(viewbox, minX);
            Canvas.SetTop(viewbox, minY);

            viewbox.PreviewMouseLeftButtonDown += Element_MouseLeftButtonDown;
            viewbox.PreviewMouseMove += Element_MouseMove;
            viewbox.PreviewMouseLeftButtonUp += Element_MouseLeftButtonUp;

            DesignCanvas.Children.Add(viewbox);
            selectionService.Select(viewbox);
        }

        private void Ungroup_Click(object sender, RoutedEventArgs e)
        {
            var selectionService = WpfApp3.Services.SelectionService.Instance;
            var selectedItems = selectionService.SelectedObjects.OfType<Viewbox>().ToList();

            var newSelection = new List<UIElement>();

            foreach (var viewbox in selectedItems)
            {
                if (!(viewbox.Child is Canvas groupCanvas)) continue;

                // Ensure it is on DesignCanvas
                if (!DesignCanvas.Children.Contains(viewbox)) continue;

                double groupX = Canvas.GetLeft(viewbox);
                double groupY = Canvas.GetTop(viewbox);
                if (double.IsNaN(groupX)) groupX = 0;
                if (double.IsNaN(groupY)) groupY = 0;

                double scaleX = viewbox.Width / groupCanvas.Width;
                double scaleY = viewbox.Height / groupCanvas.Height;

                // Just in case of DivisionByZero or NaNs
                if (double.IsNaN(scaleX)) scaleX = 1;
                if (double.IsNaN(scaleY)) scaleY = 1;

                var children = groupCanvas.Children.OfType<UIElement>().ToList();
                foreach (var child in children)
                {
                    groupCanvas.Children.Remove(child);

                    double childRelX = Canvas.GetLeft(child);
                    double childRelY = Canvas.GetTop(child);
                    if (double.IsNaN(childRelX)) childRelX = 0;
                    if (double.IsNaN(childRelY)) childRelY = 0;

                    // Calculate new absolute position (taking into account the group offset and scale of position)
                    double childAbsX = groupX + (childRelX * scaleX);
                    double childAbsY = groupY + (childRelY * scaleY);

                    Canvas.SetLeft(child, childAbsX);
                    Canvas.SetTop(child, childAbsY);

                    // Apply visual scaling to the element itself
                    if (scaleX != 1 || scaleY != 1)
                    {
                        var st = new ScaleTransform(scaleX, scaleY);
                        if (child.RenderTransform != null && child.RenderTransform != Transform.Identity)
                        {
                            var tg = new TransformGroup();
                            tg.Children.Add(child.RenderTransform);
                            tg.Children.Add(st);
                            child.RenderTransform = tg;
                        }
                        else
                        {
                            child.RenderTransform = st;
                        }
                    }

                    DesignCanvas.Children.Add(child);
                    newSelection.Add(child);
                }

                DesignCanvas.Children.Remove(viewbox);
            }

            selectionService.ClearSelection();
            foreach (var item in newSelection) selectionService.AddToSelection(item);
        }
    }
}
