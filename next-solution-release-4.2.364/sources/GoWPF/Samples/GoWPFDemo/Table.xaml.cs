/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace Table {
  public partial class Table : UserControl {
    public Table() {
      InitializeComponent();

      // Create the model holding all of the regular nodes and links,
      // and also the nodes representing the Level headers and the Area headers
      var model = new GraphModel<Item, String>();
      model.NodesSource = new ObservableCollection<Item>() {
        new Item() { Key="A", Level=1, Area=1, ToKeys=new ObservableCollection<String>() { "B" } },
        new Item() { Key="B", Level=1, Area=2 },
        new Item() { Key="C", Level=1, Area=2 },
        new Item() { Key="D", Level=2, Area=1, ToKeys=new ObservableCollection<String>() { "A" } },
        new Item() { Key="E", Level=2, Area=1, ToKeys=new ObservableCollection<String>() { "A" } },
        new Item() { Key="F", Level=2, Area=2, ToKeys=new ObservableCollection<String>() { "B", "C" } },
        new Item() { Key="G", Level=3, Area=1, ToKeys=new ObservableCollection<String>() { "D", "E", "C" } },
        new Item() { Key="H", Level=4, Area=1, ToKeys=new ObservableCollection<String>() { "G" } },
        new Item() { Key="I", Level=4, Area=1, ToKeys=new ObservableCollection<String>() { "E" } },
        new Item() { Key="J", Level=4, Area=2, ToKeys=new ObservableCollection<String>() { "G", "F" } },
        new Item() { Key="K", Level=5, Area=1, ToKeys=new ObservableCollection<String>() { "H", "L" } },
        new Item() { Key="L", Level=5, Area=1, ToKeys=new ObservableCollection<String>() { "I" } },
        new Item() { Key="M", Level=5, Area=2, ToKeys=new ObservableCollection<String>() { "J", "F" } },

        // Level headers
        new Item() { Category="Level", Text="I", Level=1, Area=0 },
        new Item() { Category="Level", Text="II", Level=2, Area=0 },
        new Item() { Category="Level", Text="III", Level=3, Area=0 },
        new Item() { Category="Level", Text="IV", Level=4, Area=0 },
        new Item() { Category="Level", Text="V", Level=5, Area=0 },

        // Area headers
        new Item() { Category="Area", Text="ADMINISTRATIVE AREA", Level=0, Area=1 },
        new Item() { Category="Area", Text="STORAGE AREA", Level=0, Area=2 },
      };
      myDiagram.Model = model;

      myDiagram.Layout = new TableLayout();
    }
  }

  // Data for each node
  // Assume the headers use the Text property
  public class Item : GraphModelNodeData<String> {
    // these properties are assumed to be unchanging, only set at initialization,
    // so they don't need to call RaisePropertyChanged when the value changes
    public int Level { get; set; }
    public int Area { get; set; }

    // TODO: add your own properties, loaded from your database,
    // so that you can data-bind to them in XAML
  }


  public class TableLayout : DiagramLayout {
    public TableLayout() {
      this.ColumnSpacing = 10;
      this.RowSpacing = 10;
      this.NodeMargins = new Thickness(11);
    }

    public double ColumnSpacing { get; set; }
    public double RowSpacing { get; set; }
    public Thickness NodeMargins { get; set; }

    public override void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      foreach (Node n in nodes) n.Move(new Point(0, 0), false);  // make sure each Node has a Position and a Size

      // figure out how many levels and areas there are,
      int maxLevel = 0;  // assume level zero holds Area headers
      int maxArea = 0;   // assume area zero holds Level headers
      foreach (Node n in nodes) {
        Item d = (Item)n.Data;
        maxLevel = Math.Max(maxLevel, d.Level);
        maxArea = Math.Max(maxArea, d.Area);
      }
      maxLevel++;  // zero-based arrays, assume exclusive maximum
      maxArea++;

      // then collect them in the array
      myArray = new Cell[maxArea, maxLevel];
      for (int i = 0; i < maxArea; i++) {
        for (int j = 0; j < maxLevel; j++) {
          myArray[i, j] = new Cell();
        }
      }
      foreach (Node n in nodes) {
        Item d = (Item)n.Data;
        myArray[d.Area, d.Level].Nodes.Add(n);
      }

      // now figure out reasonable cell sizes for each cell
      for (int i = 0; i < maxArea; i++) {
        for (int j = 0; j < maxLevel; j++) {
          Cell cell = myArray[i, j];
          if (cell.Nodes.Count == 0) continue;
          double nw = 0;
          double nh = 0;
          foreach (Node n in cell.Nodes) {
            nw = Math.Max(nw, n.Bounds.Width);
            nh = Math.Max(nh, n.Bounds.Height);
          }
          nw += this.NodeMargins.Left + this.NodeMargins.Right;
          nh += this.NodeMargins.Top + this.NodeMargins.Bottom;
          int cols = (int)Math.Ceiling(Math.Sqrt(cell.Nodes.Count));
          cell.Bounds = new Rect(0, 0, nw*cols, nh*Math.Ceiling(cell.Nodes.Count/(double)cols));
          cell.MaxSize = new Size(nw, nh);
        }
      }

      // then line up all of the rows and columns
      // first normalize all column widths
      for (int i = 0; i < maxArea; i++) {
        double w = 0;
        for (int j = 0; j < maxLevel; j++) {
          Cell cell = myArray[i, j];
          w = Math.Max(w, cell.Bounds.Width);
        }
        for (int j = 0; j < maxLevel; j++) {
          Cell cell = myArray[i, j];
          cell.Bounds.Width = w + this.ColumnSpacing;
        }
      }
      // then normalize all row heights
      for (int j = 0; j < maxLevel; j++) {
        double h = 0;
        for (int i = 0; i < maxArea; i++) {
          Cell cell = myArray[i, j];
          h = Math.Max(h, cell.Bounds.Height);
        }
        for (int i = 0; i < maxArea; i++) {
          Cell cell = myArray[i, j];
          cell.Bounds.Height = h + this.RowSpacing;
        }
      }
      // then set the X,Y for each cell
      {
        double x = 0;
        double y = 0;
        for (int i = 0; i < maxArea; i++) {
          for (int j = 0; j < maxLevel; j++) {
            Cell cell = myArray[i, j];
            cell.Bounds.X = x;
            cell.Bounds.Y = y;
            y += cell.Bounds.Height;
          }
          x += myArray[i, 0].Bounds.Width;
          y = 0;
        }
      }

      // assign positions to each node within each cell
      for (int i = 0; i < maxArea; i++) {
        for (int j = 0; j < maxLevel; j++) {
          Cell cell = myArray[i, j];
          if (cell.Nodes.Count == 0) continue;
          if (cell.Nodes.Count == 1) {
            Node n = cell.Nodes[0];
            Size nsize = new Size(n.Bounds.Width, n.Bounds.Height);
            Rect nbounds = Spot.Center.RectForPoint(Spot.Center.PointInRect(cell.Bounds), nsize);
            n.Move(new Point(nbounds.X, nbounds.Y), true);
          } else {
            double left = cell.Bounds.X + this.ColumnSpacing/2 + this.NodeMargins.Left;
            double limit = cell.Bounds.X + cell.Bounds.Width;
            double x = left;
            double y = cell.Bounds.Y + this.RowSpacing/2 + this.NodeMargins.Top;
            foreach (Node n in cell.Nodes) {
              n.Move(new Point(x, y), true);
              if (x + n.Bounds.Width + this.NodeMargins.Left + this.NodeMargins.Right < limit) {
                x += cell.MaxSize.Width;
              } else {
                x = left;
                y += cell.MaxSize.Height;
              }
            }
          }
        }
      }

      // add separator lines between the rows and columns
      // First, create the unbound Node that holds the Canvas holding the separator Lines
      if (this.Separators == null) {
        Node sepnode = new Node();
        this.Separators = new Canvas();
        // this unbound Node has this Canvas as its VisualElement
        sepnode.Content = this.Separators;
        sepnode.Location = new Point(0, 0);  // it's always positioned at 0,0
        sepnode.LayerName = "Background";  // it's in the Background layer
        sepnode.LayoutId = "None";  // it's not passed to this TableLayout to be laid out
        sepnode.Avoidable = false;  // it doesn't affect the routing of AvoidsNodes links
        if (this.Diagram.PartsModel != null) {  // if the PartsModel exists, use it
          this.Diagram.PartsModel.AddNode(sepnode);
        } else {  // otherwise postpone until the Diagram's Template is applied
          this.Diagram.InitialParts.Add(sepnode);
        }
      }
      Canvas canvas = this.Separators;
      canvas.Children.Clear();  // remove any old Lines

      Cell first = myArray[0, 0];
      Cell last = myArray[maxArea-1, maxLevel-1];
      double totalwidth = last.Bounds.X + last.Bounds.Width - first.Bounds.X;
      double totalheight = last.Bounds.Y + last.Bounds.Height - first.Bounds.Y;
      canvas.Width = totalwidth;
      canvas.Height = totalheight;

      // vertical separator lines
      for (int i = 0; i < maxArea; i++) {
        Cell cell0 = myArray[i, 0];
        Line line = new Line();
        Canvas.SetLeft(line, 0);
        Canvas.SetTop(line, 0);
        line.Stroke = new SolidColorBrush(Colors.Brown);
        line.StrokeThickness = 2;
        line.StrokeDashArray = new DoubleCollection() { 2, 2 };
        line.X1 = cell0.Bounds.X+cell0.Bounds.Width;
        line.X2 = line.X1;
        line.Y1 = cell0.Bounds.Y+cell0.Bounds.Height;
        line.Y2 = cell0.Bounds.Y+totalheight;
        canvas.Children.Add(line);
      }
      // horizontal separator lines
      for (int j = 0; j < maxLevel; j++) {
        Cell cell0 = myArray[0, j];
        Cell cellN = myArray[maxArea-1, j];
        Line line = new Line();
        Canvas.SetLeft(line, 0);
        Canvas.SetTop(line, 0);
        line.Stroke = new SolidColorBrush(Colors.Brown);
        line.StrokeThickness = 2;
        line.StrokeDashArray = new DoubleCollection() { 2, 2 };
        line.X1 = cell0.Bounds.X+cell0.Bounds.Width;
        line.X2 = cell0.Bounds.X+totalwidth;
        line.Y1 = cell0.Bounds.Y+cell0.Bounds.Height;
        line.Y2 = line.Y1;
        canvas.Children.Add(line);
      }
    }

    private Cell[,] myArray;

    private class Cell {
      public List<Node> Nodes = new List<Node>();
      public Rect Bounds = new Rect(0, 0, 0, 0);
      public Size MaxSize = new Size(0, 0);
    }

    private Canvas Separators { get; set; }
  }
}
