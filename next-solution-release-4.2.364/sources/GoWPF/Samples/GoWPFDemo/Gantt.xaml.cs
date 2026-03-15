/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace Gantt {
  public partial class Gantt : UserControl {
    public Gantt() {
      InitializeComponent();

      // create the diagram's data model
      var model = new GraphModel<Activity, int>();
      int row = 1;
      model.NodesSource = new ObservableCollection<Activity>() {
        // don't use Key==0
        new Activity() { Key=1, Row=row++, Text="Start", FromKeys=P(), Length=0, Start=0, Critical=true, Category="Start" },
        new Activity() { Key=2, Row=row++, Text="a", FromKeys=P( 1 ), Length=4, Start=0, Critical=true },
        new Activity() { Key=3, Row=row++, Text="b", FromKeys=P( 1 ), Length=5.33, Start=0 },
        new Activity() { Key=4, Row=row++, Text="c", FromKeys=P( 2 ), Length=5.17, Start=4, Critical=true },
        new Activity() { Key=5, Row=row++, Text="d", FromKeys=P( 2 ), Length=6.33, Start=4 },
        new Activity() { Key=6, Row=row++, Text="e", FromKeys=P( 3, 4 ), Length=5.17, Start=9.17, Critical=true },
        new Activity() { Key=7, Row=row++, Text="f", FromKeys=P( 5 ), Length=4.5, Start=10.33 },
        new Activity() { Key=8, Row=row++, Text="g", FromKeys=P( 6 ), Length=5.17, Start=14.34, Critical=true },
        new Activity() { Key=9, Row=row++, Text="Finish", FromKeys=P( 7, 8 ), Length=0, Start=19.51, Critical=true, Category="Finish" },
        // add Dates along the top
        new Activity() { Key=9999, Row=0, Text="23Jul", Start=0, Category="Week" },
        new Activity() { Key=9999, Row=0, Text="30Jul", Start=5, Category="Week" },
        new Activity() { Key=9999, Row=0, Text="6Aug", Start=10, Category="Week" },
        new Activity() { Key=9999, Row=0, Text="13Aug", Start=15, Category="Week" },
      };
      myDiagram.Model = model;

      // initialize the converter to know about the Diagram, to support zooming
      var conv = Diagram.FindResource<LengthConverter>(this, "myLengthConverter");
      if (conv != null) conv.Diagram = myDiagram;
      // initialize the Diagram.GridPattern based on the spacing
      myDiagram.GridPattern.CellSize = new Size(conv.Space*Gantt.XUnit, Gantt.YUnit);
    }

    // this is just for convenience in typing in the predecessors for each Activity
    private ObservableCollection<int> P(params int[] args) {
      var coll = new ObservableCollection<int>();
      foreach (int i in args) coll.Add(i);
      return coll;
    }

    public const double XUnit = 14;  // Normal width of a "day"
    public const double YUnit = 15+3;  // Row height and spacing; also hard-coded in XAML
  }


  // This redefines mouse wheel and keyboard zoom commands to change
  // the LengthConverter.Space, rather than the DiagramPanel.Scale.
  public class SpaceZoomCommandHandler : CommandHandler {
    // This needs to be initialized!
    public LengthConverter Conv { get; set; }

    public override void DecreaseZoom(object param) {
      this.Conv.Space = Math.Max(0.1, this.Conv.Space / 1.05);
    }
    public override void IncreaseZoom(object param) {
      this.Conv.Space = Math.Min(10.0, this.Conv.Space * 1.05);
    }
    public override void Zoom(object param) {
      this.Conv.Space = Math.Max(0.1, Math.Min(10.0, (param is double ? (double)param : 1.0)));
    }
  }


  // This is used in the Rectangle.Width binding in the NodeTemplate.
  // There has to be one of these for each Diagram --
  // thus the NodeTemplate cannot be shared by different Diagrams.
#if !SILVERLIGHT
  [ValueConversion(typeof(double), typeof(double))]
#endif
  public class LengthConverter : DependencyObject, IValueConverter {
    // This needs to be initialized!
    public Diagram Diagram { get; set; }

    static LengthConverter() {
      SpaceProperty = DependencyProperty.Register("Space", typeof(double), typeof(LengthConverter),
        new PropertyMetadata(1.0, OnSpaceChanged));
    }

    private static readonly DependencyProperty SpaceProperty;
    public double Space {
      get { return (double)GetValue(SpaceProperty); }
      set { SetValue(SpaceProperty, value); }
    }
    private static void OnSpaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var conv = (LengthConverter)d;
      if ((double)e.NewValue < 0.1) {  // don't allow negative or too small value
        conv.Space = 0.1;
      } else {
        conv.UpdateAllLocations();
      }
    }

    private void UpdateAllLocations() {
      if (this.Diagram == null) return;
      foreach (Node n in this.Diagram.Nodes) {
        // re-evaluate the binding to get the updated Location for the Node
        FrameworkElement root = n.VisualElement;
        if (root != null) {
          BindingExpression expr = root.GetBindingExpression(Node.LocationProperty);
          if (expr != null) root.SetBinding(Node.LocationProperty, expr.ParentBinding);
        }
        // and update the Rectangle.Width too
        FrameworkElement shape = n.FindNamedDescendant("myRectangle");
        if (shape != null) {
          BindingExpression expr = shape.GetBindingExpression(FrameworkElement.WidthProperty);
          if (expr != null) shape.SetBinding(FrameworkElement.WidthProperty, expr.ParentBinding);
        }
      }
      this.Diagram.GridPattern.CellSize = new Size(this.Space*Gantt.XUnit, Gantt.YUnit);
      this.Diagram.Panel.UpdateDiagramBounds();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      return ((double)value) * (this.Space*Gantt.XUnit);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return ((double)value) / (this.Space*Gantt.XUnit);
    }
  }


  // This is used in the Node.Location binding in the NodeTemplate.
  // There has to be one of these for each Diagram --
  // thus the NodeTemplate cannot be shared by different Diagrams.
#if !SILVERLIGHT
  [ValueConversion(typeof(Point), typeof(Point))]
#endif
  public class LocationConverter : DependencyObject, IValueConverter {
    // This needs to be initialized!
    public LengthConverter LenConv { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      Point p = (Point)value;
      return new Point(p.X * (this.LenConv.Space*Gantt.XUnit), p.Y*Gantt.YUnit);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      Point p = (Point)value;
      return new Point(p.X / (this.LenConv.Space*Gantt.XUnit), p.Y*Gantt.YUnit);
    }
  }


  // the data for each node
  public class Activity : GraphModelNodeData<int> {
    public Activity() {
      this.Location = new Point(0, 0);
      this.Start = 0;
      this.Length = 5;
      this.Critical = false;
    }

    public int Row {
      get { return _Row; }
      set {
        if (_Row != value) {
          int old = _Row;
          _Row = value;
          RaisePropertyChanged("Row", old, value);
          this.Location = new Point(this.Location.X, value);
        }
      }
    }
    private int _Row;

    public double Start {
      get { return _Start; }
      set {
        if (_Start != value) {
          double old = _Start;
          _Start = value;
          RaisePropertyChanged("Start", old, value);
          this.Location = new Point(value, this.Location.Y);
        }
      }
    }
    private double _Start;

    public double Length {
      get { return _Length; }
      set {
        if (_Length != value) {
          double old = _Length;
          _Length = value;
          RaisePropertyChanged("Length", old, value);
        }
      }
    }
    private double _Length;

    public bool Critical {
      get { return _Critical; }
      set {
        if (_Critical != value) {
          bool old = _Critical;
          _Critical = value;
          RaisePropertyChanged("Critical", old, value);
        }
      }
    }
    private bool _Critical;
  }
}
