/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace Planogram {
  public partial class Planogram : System.Windows.Controls.UserControl {
    ObservableCollection<MyNodeData>[] paletteOptions = new ObservableCollection<MyNodeData>[4];

    public Planogram() {
      InitializeComponent();

      // Initialize the different options for nodePalette (Stored in the Tag of the listbox items so we can bind to them)
      paletteOptions[0] = new ObservableCollection<MyNodeData>() {  
                new MyNodeData() { Key="Shelf", Category="Shelf", IsSubGraph = true, Width = 100, Height = 100 },
                new MyNodeData() { Key="Rack", Category="Rack", IsSubGraph = true, Width = 100, Height = 100 }, 
                new MyNodeData() { Key="Title", Category="Title", IsSubGraph = false },
                new MyNodeData() { Key="Note", Category="Note", IsSubGraph = false },
                new MyNodeData() { Key="Label", Category="Label", IsSubGraph = false }
                };
      paletteOptions[1] = new ObservableCollection<MyNodeData>() {  
                new MyNodeData(){ Key="BlueW", Color = "LightBlue", IsSubGraph = false, Width = 75, Height = 50 },
                new MyNodeData() { Key="YellowW", Color = "LightYellow", IsSubGraph = false, Width = 75, Height = 50 }, 
                new MyNodeData() { Key="GreenW", Color = "LightGreen", IsSubGraph = false, Width = 75, Height = 50 }
                };
      paletteOptions[2] = new ObservableCollection<MyNodeData>() {  
                new MyNodeData(){ Key="BlueT", Color = "LightBlue", IsSubGraph = false, Width = 50, Height = 75 },
                new MyNodeData() { Key="YellowT", Color = "LightYellow", IsSubGraph = false, Width = 50, Height = 75 }, 
                new MyNodeData() { Key="GreenT", Color = "LightGreen", IsSubGraph = false, Width = 50, Height = 75 }
                };
      paletteOptions[3] = new ObservableCollection<MyNodeData>() {  
                new MyNodeData(){ Key="Blue", Color = "LightBlue", IsSubGraph = false, Width = 50, Height = 50 },
                new MyNodeData() { Key="Yellow", Color = "LightYellow", IsSubGraph = false, Width = 50, Height = 50 }, 
                new MyNodeData() { Key="Green", Color = "LightGreen", IsSubGraph = false, Width = 50, Height = 50 }
                };

      var nodePalModel = new GraphLinksModel<MyNodeData, String, String, GraphLinksModelLinkData<String, String>>();
      nodePalModel.NodesSource = paletteOptions[1];
      nodePalette.Model = nodePalModel;

      // Give myDiagram a few racks and shelves to start with.
      var model = new GraphLinksModel<MyNodeData, String, String, GraphLinksModelLinkData<String, String>>();
      model.NodesSource = new ObservableCollection<MyNodeData>() { 
                new MyNodeData() { Key="Rack1", Category="Rack", IsSubGraph = true, Width = 300, Height = 200, Location = new Point(50,0) },
                new MyNodeData() { Key="Rack2", Category="Rack", IsSubGraph = true, Width = 300, Height = 200, Location = new Point(350,0) },
                new MyNodeData() { Key="Shelf1", Category="Shelf", IsSubGraph=true,  Width=300, Height=75, Location = new Point(50,200) },
                new MyNodeData() { Key="Shelf2", Category="Shelf", IsSubGraph=true,  Width=300, Height=75, Location = new Point(350,200) },
                new MyNodeData() { Key="Shelf3", Category="Shelf", IsSubGraph=true,  Width=300, Height=75, Location = new Point(50,285) },
                new MyNodeData() { Key="Shelf4", Category="Shelf", IsSubGraph=true,  Width=300, Height=75, Location = new Point(350,285) },
            };

      myDiagram.Model = model;
      model.Modifiable = true; // Allows adding more nodes
      model.HasUndoManager = true; // Add support for undo/redo

      myDiagram.AllowDrop = true;

      printButton.Command = myDiagram.CommandHandler.PrintCommand;
    }

    // get access to all of the diagram-wide user-settable parameters
    private PlanogramOptions Options {
      get {
        if (_Options == null) {
          _Options = this.Resources["PlanogramOptions"] as PlanogramOptions;
        }
        return _Options;
      }
    }
    private PlanogramOptions _Options;

    // Update the node palette when the listbox's selection changes.
    private void listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      ListBoxItem lbi = (sender as ListBox).SelectedItem as ListBoxItem;
      switch (lbi.Content as String) {
        case "Tall":
          nodePalette.Model.NodesSource = paletteOptions[2];
          break;
        case "Wide":
          nodePalette.Model.NodesSource = paletteOptions[1];
          break;
        case "Small":
          nodePalette.Model.NodesSource = paletteOptions[3];
          break;
        case "Special":
          nodePalette.Model.NodesSource = paletteOptions[0];
          break;
      }
    }

    // Keep the selection up to date when checkboxes are toggled.
    private void allowResizeDisplayCheckBox_Click(object sender, RoutedEventArgs e) {
      CheckBox cb = sender as CheckBox;
      switch (cb.Name) {
        case "allowResizeDisplayCheckBox":
          this.Options.GridResizeVisible = cb.IsChecked.Value;
          break;
        case "allowResizeItemsCheckBox":
          this.Options.BoxResizeVisible = cb.IsChecked.Value;
          break;
        case "enableMinor":
          this.Options.ShowMinorLines = cb.IsChecked.Value;
          break;
        case "enableMajor":
          this.Options.ShowMajorLines = cb.IsChecked.Value;
          break;
      }

      foreach (Node n in myDiagram.SelectedParts.ToList<Part>())
        if (!n.Selectable)
          myDiagram.SelectedParts.Remove(n);
    }

    // Keep the slider's value an integer.
    private void gridlineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
      Slider s = sender as Slider;
      if (s != null) s.Value = Math.Round(s.Value);
      switch (s.Name) {
        case "cellSlider":
          this.Options.CellSize = new Size(s.Value, s.Value);
          break;
        case "gridlineSlider1":
          this.Options.MinorInterval = (int)(s.Value);
          break;
        case "gridlineSlider2":
          this.Options.MajorInterval = (int)(s.Value);
          break;
      }
    }

    // Update the minor and major grid line colors
    private void gridlineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      ComboBoxItem cbi = (sender as ComboBox).SelectedItem as ComboBoxItem;
      if (cbi == null) return;
      if ((sender as ComboBox).Name.Equals("majorGridlineComboBox"))
        this.Options.MajorColor = cbi.Background;
      else
        this.Options.MinorColor = cbi.Background;
    }

    // Make a new object's size be the closest non-zero multiple of ResizeCellSize.
    private void myDiagram_ExternalObjectsDropped(object sender, DiagramEventArgs e) {
      Size cellsize = this.Options.CellSize;
      foreach (Node n in myDiagram.SelectedParts) {
        if (n == null) continue;
        MyNodeData data = n.Data as MyNodeData;
        double w = cellsize.Width * Math.Round(data.Width / cellsize.Width);
        w = Math.Max(w, cellsize.Width);
        double h = cellsize.Height * Math.Round(data.Height / cellsize.Height);
        h = Math.Max(h, cellsize.Height);
        Rect newb = n.LocationSpot.RectForPoint(data.Location, new Size(w, h));
        data.Width = w;
        data.Height = h;
        n.Move(new Point(newb.X, newb.Y), false);
      }
    }
  }

  // Convert from a number to a Size with X == 1 and Y == the input value
  public class NumberSizeConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      if (value == null) return new Size(1, 1);
      double v = (double)value;
      if (Double.IsNaN(v) || Double.IsInfinity(v)) return new Size(1, 1);
      return new Size(1, (double)value);
    }
  }

  // This data class holds sufficient information for all kinds of nodes in this sample.
#if !SILVERLIGHT
  [Serializable]
#endif
  public class MyNodeData : GraphLinksModelNodeData<String> {
    public String Color {
      get { return _Color; }
      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
    }
    private String _Color = "White";

    public double Width {
      get { return _Width; }
      set { if (_Width != value) { double old = _Width; _Width = value; RaisePropertyChanged("Width", old, value); } }
    }
    private double _Width = 50;

    public double Height {
      get { return _Height; }
      set { if (_Height != value) { double old = _Height; _Height = value; RaisePropertyChanged("Height", old, value); } }
    }
    private double _Height = 50;
  }

  // This custom dragging tool is in place to ensure that only box nodes snap to grids,
  // and that racks or shelves are not added to other racks or shelves.
  public class CustomDraggingTool : DraggingTool {
    public override bool IsValidMember(Group group, Node node) {
      if (node != null) {
        // If the node is a rack or a shelf, we can't add it to another rack or shelf
        if (node.Category == "Rack" || node.Category == "Shelf") return false;
        // If the box is part of a rack or a shelf, and that rack of shelf is being dragged,
        // we can't add it to another rack or shelf
        Group display = node.ContainingSubGraph;
        if (display != null && this.DraggedParts.ContainsKey(display)) return false;
      }
      return base.IsValidMember(group, node);
    }

    protected override bool ConsiderSnapTo(Node moving, Point pt, Node snapper, Dictionary<Part, Info> draggedparts) {
      // Only box nodes should snap: they have the default Category, the empty string
      if (moving != null && moving.Category != "") return false;
      return base.ConsiderSnapTo(moving, pt, snapper, draggedparts);
    }
  }


  // Contains properties to bind to for all of the various options in Planogram
  public class PlanogramOptions : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(PropertyChangedEventArgs e) {
      if (PropertyChanged != null) PropertyChanged(this, e);
    }

    public bool GridResizeVisible {
      get { return _GridResizeVisible; }
      set { _GridResizeVisible = value; OnPropertyChanged(new PropertyChangedEventArgs("GridResizeVisible")); OnPropertyChanged(new PropertyChangedEventArgs("GridResizeNotVisibility")); OnPropertyChanged(new PropertyChangedEventArgs("GridResizeVisibility")); }
    }
    private bool _GridResizeVisible = false;

    public Visibility GridResizeVisibility {
      get { return (this.GridResizeVisible) ? Visibility.Visible : Visibility.Collapsed; }
    }

    public Visibility GridResizeNotVisibility {
      get { return (this.GridResizeVisible) ? Visibility.Collapsed : Visibility.Visible; }
    }

    public bool BoxResizeVisible {
      get { return _BoxResizeVisible; }
      set { _BoxResizeVisible = value; OnPropertyChanged(new PropertyChangedEventArgs("BoxResizeVisible")); OnPropertyChanged(new PropertyChangedEventArgs("BoxResizeNotVisible")); }
    }
    private bool _BoxResizeVisible = false;

    public bool BoxResizeNotVisible {
      get { return !this.BoxResizeVisible; }
    }

    public bool ShowMajorLines {
      get { return _ShowMajorLines; }
      set { _ShowMajorLines = value; OnPropertyChanged(new PropertyChangedEventArgs("ShowMajorLines")); OnPropertyChanged(new PropertyChangedEventArgs("MajorFigureH")); OnPropertyChanged(new PropertyChangedEventArgs("MajorFigureV")); }
    }
    private bool _ShowMajorLines = false;

    public int MajorInterval {
      get { return _MajorInterval; }
      set { _MajorInterval = value; OnPropertyChanged(new PropertyChangedEventArgs("MajorInterval")); }
    }
    private int _MajorInterval = 4;

    public Brush MajorColor {
      get { return _MajorColor; }
      set { _MajorColor = value; OnPropertyChanged(new PropertyChangedEventArgs("MajorColor")); }
    }
    private Brush _MajorColor = new SolidColorBrush(Colors.Red);

    public GridFigure MajorFigureH {
      get { return (this.ShowMajorLines) ? GridFigure.HorizontalLine : GridFigure.None; }
    }

    public GridFigure MajorFigureV {
      get { return (this.ShowMajorLines) ? GridFigure.VerticalLine : GridFigure.None; }
    }

    public bool ShowMinorLines {
      get { return _ShowMinorLines; }
      set { _ShowMinorLines = value; OnPropertyChanged(new PropertyChangedEventArgs("ShowMinorLines")); OnPropertyChanged(new PropertyChangedEventArgs("MinorFigureH")); OnPropertyChanged(new PropertyChangedEventArgs("MinorFigureV")); }
    }
    private bool _ShowMinorLines = false;

    public int MinorInterval {
      get { return _MinorInterval; }
      set { _MinorInterval = value; OnPropertyChanged(new PropertyChangedEventArgs("MinorInterval")); }
    }
    private int _MinorInterval = 2;

    public Brush MinorColor {
      get { return _MinorColor; }
      set { _MinorColor = value; OnPropertyChanged(new PropertyChangedEventArgs("MinorColor")); }
    }
    private Brush _MinorColor = new SolidColorBrush(Colors.Blue);

    public GridFigure MinorFigureH {
      get { return (this.ShowMinorLines) ? GridFigure.HorizontalLine : GridFigure.None; }
    }

    public GridFigure MinorFigureV {
      get { return (this.ShowMinorLines) ? GridFigure.VerticalLine : GridFigure.None; }
    }

    public Size CellSize {
      get { return _CellSize; }
      set { _CellSize = value; OnPropertyChanged(new PropertyChangedEventArgs("CellSize")); }
    }
    private Size _CellSize = new Size(25, 25);
  }
}
