/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

// This sample demonstrates one way to implement virtualization of the Nodes and Links
// needed for displaying a very large model.
// It depends on the locations and sizes of the Nodes being known in the model
// before the Nodes are actually created.
// Thus using a standard diagram layout is incompatible with this assumption,
// because a normal layout will need to consider the actual sizes of the Nodes
// in order to assign reasonable Locations for those Nodes, but the Nodes have to
// exist already to do so.

// You need to customize the VirtualizingPartManager.ComputeNodeBounds method to
// account for the actual Node DataTemplates used in your application.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace Virtualizing {
  public partial class Virtualizing : UserControl {
    public Virtualizing() {
      InitializeComponent();

      // automatically load all of the data AFTER this UserControl has
      // been displayed in the visual tree of the demo app
      myDiagram.Loaded += (s, e) => {
        myDiagram.Dispatcher.BeginInvoke((Action)(() => { LoadData(); }));
      };
    }

    // Create some sample data.
    private void LoadData() {
      var vpm = (VirtualizingPartManager)myDiagram.PartManager;
      var nodes = new ObservableCollection<NodeData>() {};
      var links = new ObservableCollection<LinkData>() {};

      // in production usage you'll probably want to remove these status updates,
      // which slows down scrolling and zooming
      vpm.Status = () => {
        myStatus.Text = "created " + vpm.NodesCount.ToString() + "/" + nodes.Count.ToString() + " nodes," +
          "  " + vpm.LinksCount.ToString() + "/" + links.Count.ToString() + " links."
          + "  " + myDiagram.Panel.DiagramBounds.ToString();
      };

      // create the node data and the link data for the model
      GenerateNodes(nodes);
      GenerateLinks(nodes, links);

      // create the model with the data
      var model = new GraphLinksModel<NodeData, String, String, LinkData>();
      model.LinksSource = links;
      model.NodesSource = nodes;

      // NOTE: regular DiagramLayouts (such as TreeLayout) will not work with this virtualizing scheme!
      // You need to make sure VirtualizingPartManager.ComputeNodeBounds returns good values
      // for every NodeData object.

      // just in case you set model.Modifiable = true, this makes sure the viewport is updated after every transaction:
      model.Changed += (s, e) => {
        if (e.Change == ModelChange.CommittedTransaction || e.Change == ModelChange.FinishedUndo || e.Change == ModelChange.FinishedRedo) {
          vpm.UpdateViewport(myDiagram.Panel.ViewportBounds, myDiagram.Panel.ViewportBounds);
        }
      };

      // do the initial layout *before* assigning to the Diagram.Model --
      // this avoids creating all of the Nodes and Links and then throwing away everything
      // outside of the viewport
      VirtualizingTreeLayout layout = myDiagram.Layout as VirtualizingTreeLayout;
      if (layout != null) {
        layout.Model = model;
        layout.DoLayout(null, null);
      }

      // initialize the Diagram with the model and its data
      myDiagram.Model = model;

      // remove the "Loading..." status indicator
      var loading = myDiagram.PartsModel.FindNodeByKey("Loading");
      if (loading != null) myDiagram.PartsModel.RemoveNode(loading);
    }

    // Creates a collection of randomly colored NodeData objects.
    private void GenerateNodes(IList<NodeData> nodes) {
      int numNodes = 9999;

      Random rand = new Random();
      for (int i = 0; i < numNodes; i++) {
        var d = new NodeData() {
          Key = i.ToString(),
          Location = new Point(0, 0),
          // Width and Height could be set based on the Category
          // or other information available when creating this node data object
          // assume VirtualTreeLayout will assign the Location
          Color = String.Format("#{0:X}{1:X}{2:X}", 120+rand.Next(100), 120+rand.Next(100), 120+rand.Next(100))
        };
        if (i == 0) d.Location = new Point(0, 0);  // locate root node
        nodes.Add(d);
      }
    }

    // Takes the collection of nodes and creates a random tree with them by creating LinkData objects
    private void GenerateLinks(IList<NodeData> nodes, IList<LinkData> links) {
      int maxLinks = 2;

      var available = new HashSet<NodeData>(nodes);
      Random rand = new Random();
      foreach (NodeData next in nodes) {
        available.Remove(next);
        int children = rand.Next(maxLinks)+1;
        for (int i = 0; i < children; i++) {
          NodeData to = available.FirstOrDefault();
          if (to == null) return;
          available.Remove(to);
          links.Add(new LinkData() { From = next.Key, To = to.Key });
        }
      }
    }
  }


  // This assumes that we can determine the Bounds, in model coordinates, for each data object,
  // without having to apply the DataTemplate to create the FrameworkElements in the visual tree.
  // You will need to implement the ComputeNodeBounds method for your application.
  // In this sample the Location is computed by a VirtualTreeLayout
  // and the Width and Height are assumed to come from the NodeData class.
  public class VirtualizingPartManager : PartManager {

    // The viewport has changed -- create new nodes and links that should now be visible
    public void UpdateViewport(Rect oldview, Rect newview) {
      this.ViewportBounds = newview;

      Diagram diagram = this.Diagram;
      IDiagramModel model = diagram.Model;
      if (!this.OffscreenQueued) {
        // don't immediately remove nodes and links that have scrolled out of the viewport
        this.OffscreenQueued = true;
        diagram.Dispatcher.BeginInvoke((Action)RemoveOffscreen);
      }
      // maybe create Nodes
      foreach (Object data in model.NodesSource) {
        AddNodeForData(data, model);
      }
      ILinksModel lmodel = model as ILinksModel;
      if (lmodel != null) {
        // maybe create Links
        foreach (Object data in lmodel.LinksSource) {
          AddLinkForData(data, lmodel);
        }
      }

      // You might want to delete this statement, for efficiency:
      if (this.Status != null) this.Status();
    }

    public Rect ViewportBounds { get; set; }

    // Customize when the PartManager creates Nodes -- when in the viewport,
    // when connecting with a link that is in the viewport, or when it's a root node
    protected override bool FilterNodeForData(object nodedata, IDiagramModel model) {
      NodeData data = nodedata as NodeData;
      if (data == null) return true;
      // always create "root" Nodes 
      var tmodel = model as ITreeModel;
      if (tmodel != null) {
        if (tmodel.GetParentForNode(nodedata) == null) return true;
      } else {  // with non-TreeModels, assume tree structure anyway
        if (this.Diagram.TreePath == TreePath.Source) {
          if (model.GetToNodesForNode(nodedata).Count() == 0) return true;
        } else {
          if (model.GetFromNodesForNode(nodedata).Count() == 0) return true;
        }
      }
      // don't create Node with unknown Location (unless it's a root node, above)
      if (Double.IsNaN(data.Location.X) || Double.IsNaN(data.Location.Y)) return false;
      // see if the Node would be in the viewport
      Rect thisb = ComputeNodeBounds(data);
      if (Intersects(this.ViewportBounds, thisb)) return true;
      // or if the Node is connected to with a Link that is in the viewport
      foreach (Object otherdata in model.GetConnectedNodesForNode(nodedata)) {
        Rect linkb = thisb;
        linkb.Union(ComputeNodeBounds(otherdata as NodeData));
        if (Intersects(this.ViewportBounds, linkb)) return true;
      }
      return false;
    }

    // Account for actual node size, because it would normally be unknown
    // until after the Node was actually created and measured.
    public Rect ComputeNodeBounds(NodeData data) {
      // Remember to take the relative Location into account.
      // This assumes the LocationSpot is at the Center of the whole node.
      return new Rect(data.Location.X - data.Width/2, data.Location.Y - data.Height/2, data.Width, data.Height);
    }

    // Customize when the PartManager creates Links
    protected override bool FilterLinkForData(object linkdata, IDiagramModel model) {
      LinkData data = linkdata as LinkData;
      if (data == null) return true;
      return IsOnscreen(data, model);
    }

    private bool IsOnscreen(LinkData data, IDiagramModel model) {
      if (data == null) return false;
      NodeData from = model.FindNodeByKey(data.From) as NodeData;
      if (from == null) return false;
      NodeData to = model.FindNodeByKey(data.To) as NodeData;
      if (to == null) return false;
      Rect b = ComputeNodeBounds(from);
      b.Union(ComputeNodeBounds(to));
      return Intersects(this.ViewportBounds, b);
    }

    // Customize when the PartManager creates Links if the model is not an ILinksModel
    protected override bool FilterLinkForData(object fromnodedata, object tonodedata, IDiagramModel model) {
      Rect b = ComputeNodeBounds(fromnodedata as NodeData);
      b.Union(ComputeNodeBounds(tonodedata as NodeData));
      return Intersects(this.ViewportBounds, b);
    }

    private bool OffscreenQueued { get; set; }

    // To reduce memory usage, remove existing Nodes and Links that are no longer within the viewport
    private void RemoveOffscreen() {
      this.OffscreenQueued = false;

      IDiagramModel model = this.Diagram.Model;
      var offscreennodes = new List<Node>();
      foreach (Node n in this.Diagram.Nodes) {
        if (!FilterNodeForData(n.Data, model)) offscreennodes.Add(n);
      }
      foreach (Node n in offscreennodes) {
        RemoveNodeForData(n.Data, model);
      }

      var offscreenlinks = new List<Link>();
      foreach (Link l in this.Diagram.Links) {
        LinkData data = l.Data as LinkData;
        if (data == null) continue;
        if (!IsOnscreen(data, model)) offscreenlinks.Add(l);
      }
      foreach (Link l in offscreenlinks) {
        RemoveLinkForData(l.Data, model);
      }

      // You might want to delete this statement, for efficiency:
      if (this.Status != null) this.Status();
    }

    // this property is just for informational feedback -- you can delete this:
    public Action Status { get; set; }

    // Compute intersection of Rects, handling Infinity and NaN properly.
    private static bool Intersects(Rect a, Rect b) {
      double tw = a.Width;
      double rw = b.Width;
      double tx = a.X;
      double rx = b.X;
      if (!Double.IsPositiveInfinity(tw) && !Double.IsPositiveInfinity(rw)) {
        tw += tx;
        rw += rx;
        if (Double.IsNaN(rw) || Double.IsNaN(tw) || tx > rw || rx > tw) return false;
      }

      double th = a.Height;
      double rh = b.Height;
      double ty = a.Y;
      double ry = b.Y;
      if (!Double.IsPositiveInfinity(th) && !Double.IsPositiveInfinity(rh)) {
        th += ty;
        rh += ry;
        if (Double.IsNaN(rh) || Double.IsNaN(th) || ty > rh || ry > th) return false;
      }
      return true;
    }
  }


  // This customized DiagramPanel calls VirtualizingPartManager.UpdateViewport when needed
  public class VirtualizingDiagramPanel : DiagramPanel {
    // replace this functionality to take into account non-existent Nodes
    protected override Rect ComputeDiagramBounds() {
      Rect db = Rect.Empty;
      var pm = this.Diagram.PartManager as VirtualizingPartManager;
      foreach (NodeData data in this.Diagram.Model.NodesSource) {
        Rect b = pm.ComputeNodeBounds(data);
        if (db.IsEmpty) {
          db = b;
        } else {
          db.Union(b);
        }
      }
      if (db.IsEmpty) return new Rect(0, 0, 0, 0);
      return db;
    }

    // whenever the viewport changes, maybe create or remove some Nodes and Links
    protected override void OnViewportBoundsChanged(RoutedPropertyChangedEventArgs<Rect> e) {
      var vpm = this.Diagram.PartManager as VirtualizingPartManager;
      if (vpm != null) vpm.UpdateViewport(e.OldValue, e.NewValue);
      base.OnViewportBoundsChanged(e);
    }
  }


  // TreeLayout is unusual because it depends on the existence of the actual Nodes for the roots.
  // Here we try to ignore all methods and properties that deal with Nodes or Links.
  // Instead we use Vertex and Edge classes that know about the model data.
  // This layout implementation assumes the use of a GraphLinksModel (i.e. an ILinksModel).
  public class VirtualizingTreeLayout : TreeLayout {

    public ILinksModel Model { get; set; }

    public override TreeNetwork CreateNetwork() {
      return new VirtualizingTreeNetwork();
    }

    // ignore the arguments, because they assume the existence of Nodes and Links
    public override TreeNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      var net = (VirtualizingTreeNetwork)CreateNetwork();
      net.AddData(this.Model);
      return net;
    }

    // need to find root Node(s); which in this case means creating the actual Nodes,
    // since they would otherwise not exist
    protected override void FindRoots() {
      foreach (VirtualizingTreeVertex v in this.Network.Vertexes) {
        if ((this.Path != TreePath.Source && v.SourceEdgesCount == 0) ||
            (this.Path == TreePath.Source && v.DestinationEdgesCount == 0)) {
          var node = this.Diagram.PartManager.AddNodeForData(v.Data, this.Model);
          if (node == null) throw new Exception("No root Node for: " + v.Data.ToString());
          ((VirtualizingTreeNetwork)this.Network).SetNode(v, node);
          this.Roots.Add(node);
        }
      }
      if (this.Roots.Count == 0) throw new Exception("Found no root node for tree");
    }
  }

  // Use Virtualizing versions of Vertex and Edge.
  public class VirtualizingTreeNetwork : TreeNetwork {
    public override TreeVertex CreateVertex() {
      return new VirtualizingTreeVertex();
    }

    public override TreeEdge CreateEdge() {
      return new VirtualizingTreeEdge();
    }

    // a replacement for TreeNetwork.AddNodesAndLinks using model data instead of Nodes or Links
    public VirtualizingTreeNetwork AddData(ILinksModel model) {
      var nodes = model.NodesSource as IEnumerable<NodeData>;
      var links = model.LinksSource as IEnumerable<LinkData>;

      var NodeDataMap = new Dictionary<NodeData, VirtualizingTreeVertex>();
      var LinkDataMap = new Dictionary<LinkData, VirtualizingTreeEdge>();

      foreach (NodeData d in nodes) {
        if (NodeDataMap.ContainsKey(d)) continue;
        // create and add VirtualizingTreeVertex
        var v = (VirtualizingTreeVertex)CreateVertex();
        v.Data = d;
        NodeDataMap.Add(d, v);
        AddVertex(v);
      }

      foreach (LinkData d in links) {
        if (LinkDataMap.ContainsKey(d)) continue;
        // find connected node data
        var from = (NodeData)model.FindNodeByKey(d.From);
        var to = (NodeData)model.FindNodeByKey(d.To);
        if (from == null || to == null || from == to) continue;  // skip
        // now find corresponding vertexes
        VirtualizingTreeVertex f;
        NodeDataMap.TryGetValue(from, out f);
        VirtualizingTreeVertex t;
        NodeDataMap.TryGetValue(to, out t);
        if (f == null || t == null) continue;  // skip
        // create and add VirtualizingTreeEdge
        var e = (VirtualizingTreeEdge)CreateEdge();
        e.Data = d;
        e.FromVertex = f;
        e.ToVertex = t;
        AddEdge(e);
      }
      return this;
    }

    internal void SetNode(VirtualizingTreeVertex v, Node n) {
      v.Node = n;
      this.NodeToVertexDictionary.Add(n, v);
    }
  }

  // Associate with NodeData rather than with Node.
  public class VirtualizingTreeVertex : TreeVertex {
    public NodeData Data {
      get { return _Data; }
      set {
        _Data = value;
        if (_Data != null) {
          // use bounds information from the NodeData rather than the Node.Bounds!
          this.Focus = new Point(_Data.Width/2, _Data.Height/2);
          this.Bounds = new Rect(_Data.Location.X-_Data.Width/2, _Data.Location.Y-_Data.Height/2,
                                 _Data.Width, _Data.Height);
        }
      }
    }
    private NodeData _Data = null;

    public override void CommitPosition() {
      if (this.Data != null) {
        this.Data.Location = this.Center;  // set NodeData.Location instead of Node.Location!
      } else {
        base.CommitPosition();
      }
    }
  }

  // Associate with LinkData rather than with Link.
  // NOTE: This does not support custom routing of links that is normally done by TreeLayout
  public class VirtualizingTreeEdge : TreeEdge {
    public LinkData Data { get; set; }
  }


  public class NodeData : GraphLinksModelNodeData<String> {
    // assume this won't change dynamically, so don't need to call RaisePropertyChanged
    public String Color { get; set; }

    public double Width {
      get { return _Width; }
      set { if (_Width != value) { double old = _Width; _Width = value; RaisePropertyChanged("Width", old, value); } }
    }
    private double _Width = 100;

    public double Height {
      get { return _Height; }
      set { if (_Height != value) { double old = _Height; _Height = value; RaisePropertyChanged("Height", old, value); } }
    }
    private double _Height = 50;
  }

  public class LinkData : GraphLinksModelLinkData<String, String> { }
}
