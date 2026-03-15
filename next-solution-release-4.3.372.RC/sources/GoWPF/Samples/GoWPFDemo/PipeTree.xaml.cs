/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace PipeTree {
  public partial class PipeTree : UserControl {
    public PipeTree() {
      InitializeComponent();

      // setup the model and its initial data
      TreeModel<PipeInfo, String> model = new TreeModel<PipeInfo, String>();
      model.NodeKeyPath = "Key";
      model.ParentNodePath = "ParentKey";
      ObservableCollection<PipeInfo> pipes = new ObservableCollection<PipeInfo>() {
        new PipeInfo() { Key="0",                   Level="Main",       Current=92, Max=100 },
        new PipeInfo() { Key="1",   ParentKey="0",  Level="SubMain",    Current=47 },
        new PipeInfo() { Key="11",  ParentKey="1",  Level="Lateral",    Current=15 },
        new PipeInfo() { Key="12",  ParentKey="1",  Level="Lateral",    Current=17 },
        new PipeInfo() { Key="121", ParentKey="12", Level="SubLateral", Current=8 },
        new PipeInfo() { Key="122", ParentKey="12", Level="SubLateral", Current=9 },
        new PipeInfo() { Key="13",  ParentKey="1",  Level="Lateral",    Current=15 },
        new PipeInfo() { Key="131", ParentKey="13", Level="SubLateral", Current=5 },
        new PipeInfo() { Key="132", ParentKey="13", Level="SubLateral", Current=5 },
        new PipeInfo() { Key="133", ParentKey="13", Level="SubLateral", Current=5 },
        new PipeInfo() { Key="2",   ParentKey="0",  Level="SubMain",    Current=45 },
      };
      model.NodesSource = pipes;
      model.Modifiable = true;  // this is false, by default
      // If you want it to be modifiable and also support copying nodes,
      // you'll need to override TreeModel.MakeNodeKeyUnique.
      myDiagram.Model = model;
    }

    // test explicit layout, insertion of new PipeInfo, modifying existing PipeInfo
    private void Grid_KeyDown(object sender, KeyEventArgs e) {
      if (e.Key == Key.L) {  // layout the diagram again
        myDiagram.LayoutDiagram();
      } else if (e.Key == Key.I) {  // insert a pipe
        int id = 21;
        while (myDiagram.Model.FindNodeByKey(id.ToString()) != null) id++;
        PipeInfo pi = new PipeInfo() { Key=id.ToString(), ParentKey="2", Level="New", Current=id };
        var pipes = myDiagram.Model.NodesSource as ObservableCollection<PipeInfo>;
        if (pipes != null) {
          // need StartTransaction/CommitTransaction if supporting undo/redo
          myDiagram.StartTransaction("add node");
          pipes.Add(pi);
          myDiagram.CommitTransaction("add node");
        }
      } else if (e.Key == Key.M) {  // modify a particular pipe
        PipeInfo pi = myDiagram.Model.FindNodeByKey("11") as PipeInfo;
        if (pi != null) {
          // need StartTransaction/CommitTransaction if supporting undo/redo
          myDiagram.StartTransaction("incrementing Current");
          pi.Current++;
          myDiagram.CommitTransaction("incrementing Current");
        }
      }
    }
  }


  // Data

  public class PipeInfo : TreeModelNodeData<String> {
    public PipeInfo() {
      UpdateText();
    }

    public override object Clone() {
      PipeInfo d = (PipeInfo)base.Clone();
      d.Level = this.Level;
      d.Max = this.Max;
      d.Current = this.Current;
      return d;
    }

    public String Level {
      get { return _Level; }
      set {
        if (_Level != value) {
          String old = _Level;
          _Level = value;
          RaisePropertyChanged("Level", old, value);
          UpdateText();
        }
      }
    }
    private String _Level;

    public float Max {
      get { return _Max; }
      set {
        if (_Max != value) {
          float old = _Max;
          _Max = value;
          RaisePropertyChanged("Max", old, value);
          UpdateText();
        }
      }
    }
    private float _Max;

    public float Current {
      get { return _Current; }
      set {
        if (_Current != value) {
          float old = _Current;
          _Current = value;
          RaisePropertyChanged("Current", old, value);
          UpdateText();
        }
      }
    }
    private float _Current;

    public void UpdateText() {
      if (this.Max > 0) {
        this.Text = String.Format("{0} -- Max: {1} gpm  Current: {2} gpm",
                                  this.Level, this.Max, this.Current);
      } else {
        this.Text = String.Format("{0} -- Current: {1} gpm",
                                  this.Level, this.Current);
      }
    }
  }


  // Specialized TreeLayout

  public class PipeTreeLayout : TreeLayout {
    protected override void AssignTreeVertexValues(TreeVertex v) {
      base.AssignTreeVertexValues(v);
      // Manually rotate the TextBlock and resize the background Rectangle
      // based on the level/depth in the tree so that the node/vertex is approximately
      // the correct (minimum) size, including the TextBlock and some space around it.
      // This is kludgy but will work both in WPF and in Silverlight
      // (there's no FrameworkElement.LayoutTransform in Silverlight).
      Node node = v.Node;
      FrameworkElement back = node.FindNamedDescendant("Background");
      FrameworkElement tb = node.FindNamedDescendant("TextBlock");
      // find how big the TextBlock is
      Size tsize = new Size(tb.ActualWidth, tb.ActualHeight);
      // length includes some space at both ends
      double textlen = tsize.Width + 20;
      PipeInfo pi = node.Data as PipeInfo;
      // the thickness of the "pipe" Rectangle
      double boxthickness = Math.Max(pi.Current, 20);
      if (v.Angle == 0 || v.Angle == 180) {  // if tree is growing sideways
        // the Background Rectangle is sized according to PipeInfo.Current
        // and is long enough to hold the text
        back.Width = boxthickness;
        back.Height = textlen;
        node.SetAngle(tb, 90);  // and the text is turned to 90 degrees
      } else {
        back.Width = textlen;
        back.Height = boxthickness;
        node.SetAngle(tb, 0);  // normal upright text
      }
      // update the vertex with the new size
      v.Bounds = new Rect(0, 0, back.Width, back.Height);
    }

    protected override void LayoutNodes() {
      // once the tree layout is done, we know how far to extend each node
      // across the breadth of its subtree
      foreach (TreeVertex v in this.Network.Vertexes) {
        Size sz = v.SubtreeSize;
        Node node = v.Node;
        // Stretch the Background rectangle
        FrameworkElement back = node.FindNamedDescendant("Background");
        // if tree is growing sideways, the breadth is the height
        if (v.Angle == 0 || v.Angle == 180) {
          back.Height = Math.Max(back.Height, sz.Height);
        } else if (v.Angle == 90 || v.Angle == 270) {
          back.Width = Math.Max(back.Width, sz.Width);
        }
        // assume the LocationElement is the Background Rectangle
        node.Location = v.Position;
      }
    }
  }


  // Converters used by DataTemplates

  // Text background color, depending on the value of PipeInfo.Current
  public class CurrentBrushConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      Color c = Colors.Gray;
      if (value is float) {
        float v = (float)value;
        if (v == 8) c = Colors.Yellow;
        else if (v == 9) c = Colors.Red;
        else if (v == 23) c = Colors.Green;
      }
      return new SolidColorBrush(c);
    }
  }

  // Text color, depending on the value of PipeInfo.Current
  public class CurrentTextBrushConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      Color c = Colors.White;
      if (value is float) {
        float v = (float)value;
        if (v == 8) c = Colors.Black;
      }
      return new SolidColorBrush(c);
    }
  }

  // Text font size, depending on the value of PipeInfo.Current
  public class CurrentFontSizeConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is float) {
        float v = (float)value;
        return 10 + Math.Max(v, 0)/20;
      }
      return 10;
    }
  }

}
