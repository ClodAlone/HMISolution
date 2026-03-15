/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace Serpentine {
  public partial class Serpentine : UserControl {
    public Serpentine() {
      InitializeComponent();

      var names = new String[] {
          "Alpha", "Beta", "Gamma", "Delta", "Epsilon",
          "Zeta", "Eta", "Theta", "Iota", "Kappa",
          "Lambda", "Mu", "Nu", "Xi",
          "Omicron", "Pi", "Rho", "Sigma", "Tau",
          "Upsilon", "Phi", "Chi", "Psi", "Omega"
      };
      var nodes = new ObservableCollection<TreeModelNodeData<String>>();
      String prev = null;
      foreach (String n in names) {
        var data = new TreeModelNodeData<String>(n);
        data.ParentKey = prev;
        prev = n;
        nodes.Add(data);
      }
      var model = new TreeModel<TreeModelNodeData<String>, String>();
      model.NodesSource = nodes;
      myDiagram.Model = model;
    }
  }

  public class SerpentineLayout : DiagramLayout {
    public Size Spacing {
      get { return _Spacing; }
      set { _Spacing = value; }
    }
    private Size _Spacing = new Size(30, 30);

    public override void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      foreach (Node n in nodes) {
        Rect b = n.Bounds;
        if (Double.IsNaN(b.X) || Double.IsNaN(b.Y)) {
          n.Move(new Point(0, 0), false);  // make sure each Node has a Position and a Size
        }
      }

      Node root = null;
      // find a root node -- one without any incoming links
      foreach (Node n in nodes) {
        if (n.LinksInto.Count() == 0) {
          root = n;
          break;
        }
      }
      // couldn't find a root node -- just use the first one
      if (root == null) root = nodes.FirstOrDefault();
      if (root == null) return;

      // calculate the width at which we should start a new row
      double wrap = 1000;
      if (this.Diagram != null) {
        DiagramPanel panel = this.Diagram.Panel;
        if (panel != null && panel.ViewportBounds.Width > 0) {
          // leave some space for the link to turn around and for the usual padding
          Thickness pad = panel.Padding;
          wrap = Math.Max(this.Spacing.Width*2, panel.ViewportBounds.Width - 24 - pad.Left - pad.Right);
        }
      }

      double x = 0;
      double rowh = 0;
      double y = 0;
      bool increasing = true;
      Node node = root;
      while (node != null) {
        Rect b = node.Bounds;
        // get the next node, if any
        Link nextlink = node.LinksOutOf.FirstOrDefault();
        Node nextnode = (nextlink != null ? nextlink.ToNode : null);
        Rect nb = (nextnode != null ? nextnode.Bounds : new Rect());
        if (increasing) {
          node.Move(new Point(x, y), true);
          x += b.Width;
          rowh = Math.Max(rowh, b.Height);
          if (x + this.Spacing.Width + nb.Width > wrap) {
            y += rowh + this.Spacing.Height;
            x = wrap - this.Spacing.Width;
            rowh = 0;
            increasing = false;
            if (nextlink != null) {
              nextlink.Route.FromSpot = Spot.MiddleRight;
              nextlink.Route.ToSpot = Spot.MiddleRight;
            }
          } else {
            x += this.Spacing.Width;
            if (nextlink != null) {
              nextlink.Route.FromSpot = Spot.MiddleRight;
              nextlink.Route.ToSpot = Spot.MiddleLeft;
            }
          }
        } else {
          x -= b.Width;
          node.Move(new Point(x, y), true);
          rowh = Math.Max(rowh, b.Height);
          if (x - this.Spacing.Width - nb.Width < 0) {
            y += rowh + this.Spacing.Height;
            x = 0;
            rowh = 0;
            increasing = true;
            if (nextlink != null) {
              nextlink.Route.FromSpot = Spot.MiddleLeft;
              nextlink.Route.ToSpot = Spot.MiddleLeft;
            }
          } else {
            x -= this.Spacing.Width;
            if (nextlink != null) {
              nextlink.Route.FromSpot = Spot.MiddleLeft;
              nextlink.Route.ToSpot = Spot.MiddleRight;
            }
          }
        }
        node = nextnode;
      }
    }
  }
}
