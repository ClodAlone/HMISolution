/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace SequenceDiagram {
  public partial class SequenceDiagram : UserControl {
    public SequenceDiagram() {
      InitializeComponent();

      var model = new GraphLinksModel<NodeData, String, String, LinkData>();
      model.NodesSource = new ObservableCollection<NodeData>() {
        new NodeData() { Key="Fred", Text="Fred: Patron", IsSubGraph=true, Location=new Point(0, 0) },
        new NodeData() { Key="Bob", Text="Bob: Waiter", IsSubGraph=true, Location=new Point(100, 0) },
        new NodeData() { Key="Hank", Text="Hank: Cook", IsSubGraph=true, Location=new Point(200, 0) },
        new NodeData() { Key="Renee", Text="Renee: Cashier", IsSubGraph=true, Location=new Point(300, 0) }
      };
      model.LinksSource = new ObservableCollection<LinkData>() {
        new LinkData() { From="Fred", To="Bob", Text="order", Time=1, Duration=2 },
        new LinkData() { From="Bob", To="Hank", Text="order food", Time=2, Duration=3 },
        new LinkData() { From="Bob", To="Fred", Text="serve drinks", Time=3, Duration=1 },
        new LinkData() { From="Hank", To="Bob", Text="finish cooking", Time=5, Duration=1 },
        new LinkData() { From="Bob", To="Fred", Text="serve food", Time=6, Duration=2 },
        new LinkData() { From="Fred", To="Renee", Text="pay", Time=8, Duration=1 }
      };
      myDiagram.Model = model;

      // add an Activity node for each Message recipient
      model.Modifiable = true;
      double max = 0;
      foreach (LinkData d in model.LinksSource) {
        var grp = model.FindNodeByKey(d.To);
        var act = new NodeData() {
          SubGraphKey = d.To,
          Location = new Point(grp.Location.X, BarRoute.ConvertTimeToY(d.Time) - BarRoute.ActivityInset),
          Length = d.Duration * BarRoute.MessageSpacing + BarRoute.ActivityInset*2,
        };
        model.AddNode(act);
        max = Math.Max(max, act.Location.Y + act.Length);
      }
      // now make sure all of the lifelines are long enough
      foreach (NodeData d in model.NodesSource) {
        if (d.IsSubGraph) d.Length = max - BarRoute.LineStart + BarRoute.LineTrail;
      }
      model.Modifiable = false;
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<NodeData, String, String, LinkData>;
      if (model == null) return;
      XElement root = model.Save<NodeData, LinkData>("SequenceDiagram", "Node", "Link");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<NodeData, String, String, LinkData>;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        model.Load<NodeData, LinkData>(root, "Node", "Link");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }
  }

  // Customize the Route so that it always goes straight horizontally.
  public class BarRoute : Route {
    public override Point GetLinkPoint(Node node, FrameworkElement port, Spot spot, bool from,
                                       bool ortho, Node othernode, FrameworkElement otherport) {
      Point p = node.GetElementPoint(port, Spot.Center);
      Rect r = new Rect(node.GetElementPoint(port, Spot.TopLeft),
                        node.GetElementPoint(port, Spot.BottomRight));
      Point op = othernode.GetElementPoint(otherport, Spot.Center);
      
      LinkData data = this.Link.Data as LinkData;
      double y = (data != null ? ConvertTimeToY(data.Time) : 0);

      bool right = op.X > p.X;
      double dx = ActivityWidth/2;
      if (from) {
        Group grp = node as Group;
        if (grp != null) {
          // see if there is an Activity Node at this point -- if not, connect the link directly with the Group's lifeline
          bool found = false;
          foreach (Node mem in grp.MemberNodes) {
            NodeData d = mem.Data as NodeData;
            if (d != null && d.Location.Y <= y && y <= d.Location.Y+d.Length) {
              found = true;
              break;
            }
          }
          if (!found) dx = 0;
        }
      }
      double x = right ? p.X + dx : p.X - dx;
      return new Point(x, y);
    }

    protected override double GetLinkDirection(Node node, FrameworkElement port, Point linkpoint, Spot spot,
                                               bool from, bool ortho, Node othernode, FrameworkElement otherport) {
      Point p = node.GetElementPoint(port, Spot.Center);
      Point op = othernode.GetElementPoint(otherport, Spot.Center);
      bool right = op.X > p.X;
      return right ? 0 : 180;
    }

    public static double ConvertTimeToY(double t) {
      return t * MessageSpacing + LineStart;
    }

    // some parameters
    public static double LineStart = 30;  // vertical starting point in document for all Messages and Activations
    public static double LineTrail = 40;  // vertical extension of lifeline beyond all Activities
    public static double MessageSpacing = 20;  // vertical distance between Messages at different steps
    public static double ActivityInset = 3;  // vertical distance from the top or bottom that the links connect at
    public static double ActivityWidth = 15;  // width of each vertical activity bar
  }

#if !SILVERLIGHT
  [Serializable]
#endif
  public class NodeData : GraphLinksModelNodeData<String> {
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
    private double _Length = 100.0;

    // write the extra property
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(XHelper.Attribute("Length", this.Length, 100.0));
      return e;
    }

    // read the extra property
    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Length = XHelper.Read("Length", e, 100.0);
    }
  }

#if !SILVERLIGHT
  [Serializable]
#endif
  public class LinkData : GraphLinksModelLinkData<String, String> {
    public double Time {
      get { return _Time; }
      set {
        if (_Time != value) {
          double old = _Time;
          _Time = value;
          RaisePropertyChanged("Time", old, value);
        }
      }
    }
    private double _Time = 0.0;

    public double Duration {
      get { return _Duration; }
      set {
        if (_Duration != value) {
          double old = _Duration;
          _Duration = value;
          RaisePropertyChanged("Duration", old, value);
        }
      }
    }
    private double _Duration = 1.0;

    // write the extra property
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(XHelper.Attribute("Time", this.Time, 0.0));
      e.Add(XHelper.Attribute("Duration", this.Duration, 1.0));
      return e;
    }

    // read the extra property
    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Time = XHelper.Read("Time", e, 0.0);
      this.Duration = XHelper.Read("Duration", e, 1.0);
    }
  }
}
