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
using Northwoods.GoXam.Tool;

namespace InteractiveForce {
  public partial class InteractiveForce : UserControl {
    public InteractiveForce() {
      InitializeComponent();

      var model = new GraphLinksModel<SimpleData, String, String, LinkData>();
      var nodes = GenerateNodes();
      model.NodesSource = nodes;
      model.LinksSource = GenerateLinks(nodes);
      model.Modifiable = true;
      myDiagram.Model = model;
    }

    Random rand = new Random();

    // Takes the random collection of nodes and creates a random tree with them.
    private ObservableCollection<LinkData> GenerateLinks(ObservableCollection<SimpleData> nodes) {
      var linkSource = new ObservableCollection<LinkData>();
      if (nodes.Count == 0) return linkSource;
      int minLinks = 2;
      int maxLinks = 4;
      List<SimpleData> available = nodes.ToList<SimpleData>();
      foreach (SimpleData next in nodes) {
        available.Remove(next);
        int children = rand.Next(minLinks, maxLinks + 1);
        for (int i = 1; i <= children; i++) {
          if (available.Count == 0) break;
          SimpleData to = available[0];
          available.Remove(to);
          linkSource.Add(new LinkData() { From = next.Key, To = to.Key });
        }
      }
      return linkSource;
    }

    // Creates a collection of randomly colored nodes.
    private ObservableCollection<SimpleData> GenerateNodes() {
      var nodeSource = new ObservableCollection<SimpleData>();
      int minNodes = 20;
      int maxNodes = 50;
      int numberOfNodes = rand.Next(minNodes, maxNodes + 1);
      for (int i = 0; i < numberOfNodes; i++) {
        nodeSource.Add(new SimpleData() { 
          Key = "Node" + i.ToString(),
          Color = String.Format("#{0:X}{1:X}{2:X}", 120+rand.Next(100), 120+rand.Next(100), 120+rand.Next(100))
        });
      }
      // Randomize the nodes a little:
      for (int i = 0; i < nodeSource.Count; i++) {
        int swap = rand.Next(0, nodeSource.Count);
        SimpleData temp = nodeSource[swap];
        nodeSource[swap] = nodeSource[i];
        nodeSource[i] = temp;
      }
      return nodeSource;
    }
  }


  // each time we're dragging the selection, request a layout with a minimal number of iterations
  public class ContinuousDraggingTool : DraggingTool {
    public override void DoMouseMove() {
      base.DoMouseMove();
      if (this.Active) {
        var cfdlayout = this.Diagram.Layout as ContinuousForceDirectedLayout;
        if (cfdlayout != null) {
          LayoutManager mgr = this.Diagram.LayoutManager;
          int olditer = cfdlayout.MaxIterations;
          // limit the number of iterations during dragging
          int numnodes = this.Diagram.PartManager.NodesCount;
          cfdlayout.MaxIterations = Math.Max(1, (int)Math.Ceiling(10000.0/(numnodes*numnodes)));
          // perform the layout right now
          this.Diagram.LayoutManager.LayoutDiagram(LayoutInitial.InvalidateAll, true);
          cfdlayout.MaxIterations = olditer;
        }
      }
    }
  }

  // layout should not change position of selected nodes
  public class ContinuousForceDirectedLayout : ForceDirectedLayout {
    // selected nodes are "fixed"
    protected override bool IsFixed(ForceDirectedVertex v) {
      Node n = v.Node;
      if (n != null) return n.IsSelected;
      return base.IsFixed(v);
    }

    // don't position "fixed" nodes
    protected override void LayoutNodes() {
      foreach (ForceDirectedVertex vertex in this.Network.Vertexes) {
        if (!IsFixed(vertex)) vertex.CommitPosition();
      }
    }
  }

  // implement various optimizations
  public class ContinuousLayoutManager : LayoutManager {
    // optimize the creation of the ForceDirectedNetwork:
    // only create it when there are Nodes or Links added or removed
    protected override void PerformLayout() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      var cfdlayout = diagram.Layout as ContinuousForceDirectedLayout;
      if (cfdlayout != null) {
        // assume there are no Groups with their own Layout
        if (!cfdlayout.ValidLayout) {
          // make sure there's no animation interfering with ContinuousForceDirectedLayout
          this.Animated = false;
          // don't set Wait cursor during drag
          diagram.Cursor = null;
          // the ForceDirectedNetwork needs to be cleared whenever a Node or Link is added or removed
          var net = cfdlayout.Network;
          if (net == null) {  // create a new one
            net = new ForceDirectedNetwork();
            net.AddNodesAndLinks(diagram.Nodes.Where(n => CanLayoutPart(n, cfdlayout)),
                                 diagram.Links.Where(l => CanLayoutPart(l, cfdlayout)));
            cfdlayout.Network = net;
          } else {  // update all vertex.Bounds
            foreach (ForceDirectedVertex v in net.Vertexes) {
              Node n = v.Node;
              if (n != null) v.Bounds = n.Bounds;
            }
          }
          // don't need to re-collect the appropriate Nodes and Links to pass to DoLayout
          this.IsLayingOut = true;
          cfdlayout.DoLayout(null, null);
          this.IsLayingOut = false;
          cfdlayout.ValidLayout = true;
          // DoLayout normally discards the Network; keep it for next time
          cfdlayout.Network = net;
        }
      } else {
        base.PerformLayout();
      }
    }

    // node positioning by the layout shouldn't invalidate itself
    private bool IsLayingOut { get; set; }

    // invalidate the Diagram.Layout if node/link added/removed,
    // and remove any cached ForceDirectedNetwork
    public override void InvalidateLayout(Part p, LayoutChange change) {
      if (this.IsLayingOut) return;
      if (this.SkipsInvalidate) return;
      if (this.Diagram == null) return;
      if (p == null || p.Layer == null || p.Layer.IsTemporary) return;
      var cfdlayout = this.Diagram.Layout as ContinuousForceDirectedLayout;
      if (cfdlayout != null &&
          (change == LayoutChange.NodeAdded || change == LayoutChange.NodeRemoved ||
           change == LayoutChange.LinkAdded || change == LayoutChange.LinkRemoved)) {
        cfdlayout.ValidLayout = false;
        cfdlayout.Network = null;
      } else {
        base.InvalidateLayout(p, change);
      }
    }
  }


  // This class holds sufficient information for all nodes in this sample.
  public class SimpleData : GraphLinksModelNodeData<String> {
    public String Color {
      get { return _Color; }
      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
    }
    private String _Color = "White";
  }

  // For this sample there no properties to add to link data.
  public class LinkData : GraphLinksModelLinkData<String, String> { }
}
