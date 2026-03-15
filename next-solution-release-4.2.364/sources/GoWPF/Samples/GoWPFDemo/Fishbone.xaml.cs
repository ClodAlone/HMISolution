/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace Fishbone {
  public partial class Fishbone : UserControl {
    public Fishbone() {
      InitializeComponent();

      var model = new TreeModel<Info, int>();

      // load the XML data from a file that is an embedded resource
      using (Stream stream = Demo.MainPage.Instance.GetStream("Fishbone", "xml")) {
        using (StreamReader reader = new StreamReader(stream)) {
          XElement root = XElement.Load(reader);
          root = root.Element("node");
          // iterate over all the nested elements inside the root element
          // collect a new Info() for each XElement, remembering the interesting attribute values
          var coll = new List<Info>();
          WalkTree(root, coll);
          model.NodesSource = coll;
          myDiagram.Model = model;
          FishboneButton_Click(null, null);
        }
      }
    }

    private Info WalkTree(XElement xe, List<Info> coll) {
      var info = new Info();
      info.Key = coll.Count + 1;
      XAttribute a = xe.Attribute("text");
      if (a != null) info.Text = a.Value;
      a = xe.Attribute("fontsize");
      if (a != null) {
        try {
          info.FontSize = Double.Parse(a.Value);
        } catch { }
      }
      a = xe.Attribute("fontweight");
      if (a != null) info.FontWeight = a.Value;
      coll.Add(info);
      foreach (XElement child in xe.Elements()) {
        var childinfo = WalkTree(child, coll);
        childinfo.ParentKey = info.Key;
      }
      return info;
    }

    private void FishboneButton_Click(object sender, RoutedEventArgs e) {
      myDiagram.StartTransaction("Fishbone");
      myDiagram.LinkTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "FishboneLinkTemplate");
      myDiagram.Layout = new FishboneLayout() {
        Angle = 180,
        LayerSpacing = 10,
        NodeSpacing = 20,
        RowSpacing = 10
      };
      myDiagram.CommitTransaction("Fishbone");
    }

    private void BranchingButton_Click(object sender, RoutedEventArgs e) {
      myDiagram.StartTransaction("Branching");
      myDiagram.LinkTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "NormalLinkTemplate");
      myDiagram.Layout = new TreeLayout() {
        Angle = 180,
        LayerSpacing = 20,
        Alignment = TreeAlignment.BusBranching
      };
      myDiagram.CommitTransaction("Branching");
    }

    private void NormalButton_Click(object sender, RoutedEventArgs e) {
      myDiagram.StartTransaction("Normal");
      myDiagram.LinkTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "NormalLinkTemplate");
      myDiagram.Layout = new TreeLayout() {
        Angle = 180,
        BreadthLimit = 1000,
        Alignment = TreeAlignment.Start
      };
      myDiagram.CommitTransaction("Normal");
    }
  }

  
  // only works for Angle == 0 or Angle == 180
  public class FishboneLayout : TreeLayout {
    public FishboneLayout() {
      this.Alignment = TreeAlignment.BusBranching;
      this.SetsPortSpot = false;
      this.SetsChildPortSpot = false;
    }

    public override TreeNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      System.Diagnostics.Debug.Assert(this.Angle == 0 || this.Angle == 180);
      System.Diagnostics.Debug.Assert(this.Alignment == TreeAlignment.BusBranching);
      System.Diagnostics.Debug.Assert(this.Path != TreePath.Source);
      var net = base.MakeNetwork(nodes, links);
      foreach (var v in net.Vertexes.ToArray()) {
        // ignore leaves of tree
        if (v.DestinationEdgesCount == 0) continue;
        if (v.DestinationEdgesCount % 2 == 1) {
          // if there's an odd number of real children, add two dummies
          var dummy = new TreeVertex();
          dummy.Bounds = new Rect();
          dummy.Focus = new Point();
          net.AddVertex(dummy);
          net.LinkVertexes(v, dummy, null);
        }
        // make sure there's an odd number of children, including at least one dummy;
        // LayoutNodes will move the parent node to where this dummy child node is placed
        var dummy2 = new TreeVertex();
        dummy2.Bounds = v.Bounds;
        dummy2.Focus = v.Focus;
        net.AddVertex(dummy2);
        net.LinkVertexes(v, dummy2, null);
      }
      return net;
    }

    protected override void AssignTreeVertexValues(TreeVertex v) {
      base.AssignTreeVertexValues(v);
      SetDirection(v, 0);
      if (v.Parent != null) {
        // The parent node will be moved to where the last dummy will be;
        // reduce the space to account for the future hole.
        if (v.Angle == 0 || v.Angle == 180) {
          v.LayerSpacing -= v.Bounds.Width;
        } else {
          v.LayerSpacing -= v.Bounds.Height;
        }
      }
    }

    protected override void LayoutNodes() {
      // vertex Angle is set by BusBranching "inheritance";
      // assign spots assuming overall Angle == 0 or 180
      // and links are always connecting horizontal with vertical
      foreach (var e in this.Network.Edges) {
        var link = e.Link;
        if (link == null) continue;
        var route = link.Route;
        route.FromSpot = Spot.None;
        route.ToSpot = Spot.None;

        var v = e.FromVertex;
        var w = e.ToVertex;

        if (v.Angle == 0) {
          route.FromSpot = Spot.MiddleLeft;
        } else if (v.Angle == 180) {
          route.FromSpot = Spot.MiddleRight;
        }

        if (w.Angle == 0) {
          route.ToSpot = Spot.MiddleLeft;
        } else if (w.Angle == 180) {
          route.ToSpot = Spot.MiddleRight;
        }
      }

      // move the parent node to the location of the last dummy
      foreach (var v in this.Network.Vertexes) {
        var len = v.ChildrenCount;
        if (len == 0) continue;  // ignore leaf nodes
        if (v.Parent == null) continue; // don't move root node
        var dummy2 = v.Children[len-1];
        v.Center = dummy2.Center;
      }

      foreach (var v in this.Network.Vertexes) {
        if (v.Parent == null) {
          Shift(v);
        }
      }

      // now actually change the Node.Location of all nodes
      base.LayoutNodes();
    }

    // don't use the standard routing done by TreeLayout
    protected override void LayoutLinks() { }

    private void Shift(TreeVertex v) {
      var p = v.Parent;
      if (p != null && (v.Angle == 90 || v.Angle == 270)) {
        var g = p.Parent;
        if (g != null) {
          double shift = v.NodeSpacing;
          if (GetDirection(g) > 0) {
            if (g.Angle == 90) {
              if (p.Angle == 0) {
                SetDirection(v, 1);
                if (v.Angle == 270) ShiftAll(2, -shift, p, v);
              } else if (p.Angle == 180) {
                SetDirection(v, -1);
                if (v.Angle == 90) ShiftAll(-2, shift, p, v);
              }
            } else if (g.Angle == 270) {
              if (p.Angle == 0) {
                SetDirection(v, 1);
                if (v.Angle == 90) ShiftAll(2, -shift, p, v);
              } else if (p.Angle == 180) {
                SetDirection(v, -1);
                if (v.Angle == 270) ShiftAll(-2, shift, p, v);
              }
            }
          } else if (GetDirection(g) < 0) {
            if (g.Angle == 90) {
              if (p.Angle == 0) {
                SetDirection(v, 1);
                if (v.Angle == 90) ShiftAll(2, -shift, p, v);
              } else if (p.Angle == 180) {
                SetDirection(v, -1);
                if (v.Angle == 270) ShiftAll(-2, shift, p, v);
              }
            } else if (g.Angle == 270) {
              if (p.Angle == 0) {
                SetDirection(v, 1);
                if (v.Angle == 270) ShiftAll(2, -shift, p, v);
              } else if (p.Angle == 180) {
                SetDirection(v, -1);
                if (v.Angle == 90) ShiftAll(-2, shift, p, v);
              }
            }
          }
        } else {  // g == null: V is a child of the tree ROOT
          var dir = ((p.Angle == 0) ? 1 : -1);
          SetDirection(v, dir);
          ShiftAll(dir, 0, p, v);
        }
      }
      foreach (var c in v.Children) {
        Shift(c);
      }
    }

    // rather than adding a property to TreeVertex, just reuse an otherwise unused property:
    private static double GetDirection(TreeVertex v) {
      return v.BreadthLimit;
    }
    private static void SetDirection(TreeVertex v, double dir) {
      v.BreadthLimit = dir;
    }

    private void ShiftAll(double direction, double absolute, TreeVertex root, TreeVertex v) {
      System.Diagnostics.Debug.Assert(root.Angle == 0 || root.Angle == 180);
      var loc = v.Center;
      loc.X += direction * Math.Abs(root.Center.Y-loc.Y)/2;
      loc.X += absolute;
      v.Center = loc;
      foreach (var c in v.Children) {
        ShiftAll(direction, absolute, root, c);
      }
    }
  }

  public class FishboneRoute : Route {
    protected override bool ComputePoints() {
      var result = base.ComputePoints();
      if (result) {
        // insert middle point to maintain horizontal lines
        if (this.FromSpot == Spot.MiddleRight || this.FromSpot == Spot.MiddleLeft) {
          Point p1;
          // deal with root node being on the "wrong" side
          Node fromnode = this.Link.FromNode;
          if (fromnode.LinksInto.Count() == 0) {
            // pretend the link is coming from the opposite direction than the declared FromSpot
            FrameworkElement fromport = this.Link.FromPort;
            var fromctr = fromnode.GetElementPoint(fromport, Spot.Center);
            var fromfar = fromctr;
            fromfar.X += (this.FromSpot == Spot.MiddleLeft ? 99999 : -99999);
            p1 = GetLinkPointFromPoint(fromnode, fromport, fromctr, fromfar, true);
            // update the route points
            SetPoint(0, p1);
            var endseg = this.FromEndSegmentLength;
            if (Double.IsNaN(endseg)) endseg = Node.GetFromEndSegmentLength(fromport);
            p1.X += (this.FromSpot == Spot.MiddleLeft) ? endseg : -endseg;
            SetPoint(1, p1);
          } else {
            p1 = GetPoint(1);  // points 0 & 1 should be OK already
          }
          Node tonode = this.Link.ToNode;
          FrameworkElement toport = this.Link.ToPort;
          Point toctr = tonode.GetElementPoint(toport, Spot.Center);
          Point far = toctr;
          far.X += (this.FromSpot == Spot.MiddleLeft) ? -99999/2 : 99999/2;
          far.Y += (toctr.Y < p1.Y) ? 99999 : -99999;
          var p2 = GetLinkPointFromPoint(tonode, toport, toctr, far, false);
          SetPoint(2, p2);
          var dx = Math.Abs(p2.Y-p1.Y)/2;
          if (this.FromSpot == Spot.MiddleLeft) dx = -dx;
          InsertPoint(2, new Point(p2.X+dx, p1.Y));
        } else if (this.ToSpot == Spot.MiddleRight || this.ToSpot == Spot.MiddleLeft) {
          Point p1 = GetPoint(1);  // points 1 & 2 should be OK already
          Node fromnode = this.Link.FromNode;
          FrameworkElement fromport = this.Link.FromPort;
          Link parentlink = fromnode.LinksInto.FirstOrDefault();
          Point fromctr = fromnode.GetElementPoint(fromport, Spot.Center);
          Point far = fromctr;
          far.X += (parentlink != null && parentlink.Route.FromSpot == Spot.MiddleLeft) ? -99999/2 : 99999/2;
          far.Y += (fromctr.Y < p1.Y) ? 99999 : -99999;
          var p0 = GetLinkPointFromPoint(fromnode, fromport, fromctr, far, true);
          SetPoint(0, p0);
          var dx = Math.Abs(p1.Y-p0.Y)/2;
          if (parentlink != null && parentlink.Route.FromSpot == Spot.MiddleLeft) dx = -dx;
          InsertPoint(1, new Point(p0.X+dx, p1.Y));
        }
      }
      return result;
    }
  }


  public class Info : TreeModelNodeData<int> {
    public Info() {
      this.FontSize = 11;
      this.FontWeight = "Normal";
    }

    // because these properties aren't modified after initialization,
    // it doesn't have to call RaisePropertyChanged
    public double FontSize { get; set; }
    public string FontWeight { get; set; }
  }
}
