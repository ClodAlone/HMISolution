/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace FlowGrammer {

  public partial class FlowGrammer : UserControl {
    public FlowGrammer() {
      InitializeComponent();

      // Set up the model for the Palette
      var paletteModel = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();
      paletteModel.NodeCategoryPath = "Category";
      paletteModel.NodesSource = new List<MyNodeData>() {
        new MyNodeData() { Key="If", Text="If", Category="If" },
        new MyNodeData() { Key="Action 1", Text="Action 1", Category="Action" },
        new MyNodeData() { Key="Action 2", Text="Action 2", Category="Action" },
        new MyNodeData() { Key="Action 3", Text="Action 3", Category="Action" },
        new MyNodeData() { Key="Effect 1", Text="Effect 1", Category="Effect" },
        new MyNodeData() { Key="Effect 2", Text="Effect 2", Category="Effect" },
        new MyNodeData() { Key="Effect 3", Text="Effect 3", Category="Effect" },
        new MyNodeData() { Key="Output 1", Text="Output 1", Category="Output" },
        new MyNodeData() { Key="Output 2", Text="Output 2", Category="Output" },
      };
      myPalette.Model = paletteModel;
      myPalette.AllowDragOut = true;

      // Set up an initial model for the Diagram
      var model = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();
      String xml = Demo.MainPage.Instance.LoadText("FlowGrammer", "xml");
      model.Load<MyNodeData, MyLinkData>(XElement.Parse(xml), "MyNodeData", "MyLinkData");

      model.Modifiable = true;
      model.HasUndoManager = true;
      myDiagram.Model = model;
      myDiagram.AllowDrop = true;
      myDiagram.AllowCopy = false;
      myDiagram.AllowMove = false;
      myDiagram.DraggingTool = new CustomDraggingTool();
      myDiagram.PartManager = new MyPartManager();
    }

    // Save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<MyNodeData, String, String, MyLinkData>;
      if (model == null) return;
      XElement root = model.Save<MyNodeData, MyLinkData>("FlowGrammer", "MyNodeData", "MyLinkData");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<MyNodeData, String, String, MyLinkData>;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        model.Load<MyNodeData, MyLinkData>(root, "MyNodeData", "MyLinkData");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }
  }

  // A Custom DraggingTool used to add highlighting functionality and only allow drops
  // on links and nodes
  public class CustomDraggingTool : DraggingTool {
    // Override DragOver to implement highlighting of links and nodes when dragged over
    protected override void DragOver(Point pt, bool moving, bool copying) {
      // Make the dragged node less opaque
      ClearOpacities();
      this.CurrentPart = FindPartAt(this.Diagram.LastMousePointInModel, false);
      Node node = this.CurrentPart as Node;
      if (node != null) {
        MyNodeData mnd = node.Data as MyNodeData;
        if (mnd != null) mnd.Opacity = .5;
      }
      this.DropOntoEnabled = true;
      if (this.DropOntoEnabled && this.Diagram.Model.Modifiable) {
        Part overpart = this.Diagram.Panel.FindElementsAt<Part>(pt,
                        Part.FindAncestor<Part>,
                        p => !this.Diagram.SelectedParts.Contains(p),
                        SearchLayers.Parts).FirstOrDefault();
        this.DragOverPart = overpart;
        // When overpart changes, clear any previous highlights
        ClearHighlights();
        // Highlights the part being dragged over if it is a link or a node
        Link l = overpart as Link;
        if (l != null) {
          MyLinkData mld = l.Data as MyLinkData;
          if (mld != null) mld.Highlight = true;
        } else {
          Node n = overpart as Node;
          if (n != null) {
            MyNodeData mnd = n.Data as MyNodeData;
            // Don't highlight "Start" node
            if (mnd != null && mnd.Category != "Start") mnd.Highlight = true;
          }
        }
      }
      if (this.Diagram.AllowScroll && (moving || copying)) this.Diagram.Panel.DoAutoScroll(pt);
    }

    // Override DropOnto to allow drops only on links or nodes
    protected override void DropOnto(Point pt) {
      Part top = this.Diagram.Panel.FindElementsAt<Part>(pt,
                        Part.FindAncestor<Part>,
                        p => !this.Diagram.SelectedParts.Contains(p),
                        SearchLayers.Parts).FirstOrDefault();
      // If the dragged node is dropped onto the background, cancel the action
      if (top == null) {
        DoCancel();
        return;
      }
      // Remove opacities and highlights from dragging, then insert the node
      ClearOpacities();
      ClearHighlights();
      Link l = top as Link;
      if (l != null) {
        InsertNodeOnLink(this.Diagram.SelectedNode, l);
      } else {
        Node n = top as Node;
        if (n != null)
          InsertNodeBeforeNode(this.Diagram.SelectedNode, n);
      }
      this.Diagram.ClearSelection();
      base.DropOnto(pt);
    }

    // Method dealing with nodes inserted onto links
    private void InsertNodeOnLink(Node node, Link link) {
      if (node == null) return;
      var model = this.Diagram.Model;
      Object fromdata = link.FromData;
      Object fromport = link.FromPortId;
      Object todata = link.ToData;
      Object toport = link.ToPortId;
      switch (node.Category) {
        case "Effect":
        case "Action": {
            // Delete the existing link, then add links to and from the new node
            model.RemoveLink(fromdata, fromport, todata, toport);
            model.AddLink(fromdata, fromport, node.Data, "TopPort");
            model.AddLink(node.Data, "BottomPort", todata, toport);
            break;
          }
        case "If": {
            // Delete the existing link, then add links to and from the new node
            model.RemoveLink(fromdata, fromport, todata, toport);
            model.AddLink(fromdata, fromport, node.Data, "TopPort");
            model.AddLink(node.Data, "LeftPort", todata, toport);
            model.AddLink(node.Data, "RightPort", todata, toport);
            break;
          }
        case "Output": {
            // Add a new link to the new node
            model.AddLink(fromdata, fromport, node.Data, "TopPort");
            break;
          }
        default: break;
      }
    }

    // Method dealing with nodes inserted onto nodes
    // The newnode is the node dropped, and oldnode is the node being dropped on
    private void InsertNodeBeforeNode(Node newnode, Node oldnode) {
      if (newnode == null) return;
      // Can't drop onto a "Start" node
      if (oldnode.Category == "Start") {
        DoCancel();
        return;
      }
      var model = this.Diagram.Model;
      switch (newnode.Category) {
        case "Effect":
        case "Action": {
            // Take all links into oldnode, and relink to newnode, then link newnode to oldnode
            foreach (Link link in oldnode.LinksInto) {
              Object fromdata = link.FromData;
              Object fromport = link.FromPortId;
              Object todata = link.ToData;
              Object toport = link.ToPortId;
              model.RemoveLink(fromdata, fromport, todata, toport);
              model.AddLink(fromdata, fromport, newnode.Data, "TopPort");
            }
            model.AddLink(newnode.Data, "BottomPort", oldnode.Data, "TopPort");
            break;
          }
        case "If": {
            // Take all links into the oldnode, and relink to the newnode, then link newnode to oldnode
            foreach (Link link in oldnode.LinksInto) {
              Object fromdata = link.FromData;
              Object fromport = link.FromPortId;
              Object todata = link.ToData;
              Object toport = link.ToPortId;
              model.RemoveLink(fromdata, fromport, todata, toport);
              model.AddLink(fromdata, fromport, newnode.Data, "TopPort");
            }
            model.AddLink(newnode.Data, "LeftPort", oldnode.Data, "TopPort");
            model.AddLink(newnode.Data, "RightPort", oldnode.Data, "TopPort");
            break;
          }
        case "Output": {
            // Find the previous node, and if it is an "If" node, link the right port
            Node prev = GetOtherEnd(oldnode, "TopPort");
            if (prev != null) {
              if (prev.Category == "If")
                model.AddLink(prev.Data, "RightPort", newnode.Data, "TopPort");
              else
                model.AddLink(prev.Data, "BottomPort", newnode.Data, "TopPort");
            }
            break;
          }
        default: break;
      }
    }

    // Used to clear any existing opacities before changing opacity of a new part or after a drop has occurred
    private void ClearOpacities() {
      foreach (MyNodeData d in this.Diagram.Model.NodesSource) d.Opacity = 1;
    }

    // Used to clear any existing highlights before highlighting a new part or after a drop has occurred
    private void ClearHighlights() {
      foreach (MyNodeData d in this.Diagram.Model.NodesSource) d.Highlight = false;
      foreach (MyLinkData d in ((ILinksModel)this.Diagram.Model).LinksSource) d.Highlight = false;
    }

    // Find the node at the other end of a given port
    public Node GetOtherEnd(Node node, String portid) {
      if (node.FindPort(portid, true) != null)
        if (portid == "TopPort") {
          foreach (Node n in node.FindNodesIntoPort(portid)) return n;
        } else {
          foreach (Node n in node.FindNodesOutOfPort(portid)) return n;
        }
      return null;
    }
  }

  public class MyPartManager : PartManager {
    // Override DeleteParts with the only change being a custom RemoveNode
    public override void DeleteParts(IEnumerable<Part> coll) {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return;
      foreach (Part p in coll.ToList()) {  // work on copy of collection
        if (!p.CanDelete()) continue;  // not removable?
        IDiagramModel dmodel = p.Model;
        if (dmodel == null) return;
        Node n = p as Node;
        if (n != null) {
          RemoveNode(n);
        } else {
          Link l = p as Link;
          if (l != null) {
            ILinksModel lmodel = dmodel as ILinksModel;
            if (lmodel != null)
              lmodel.RemoveLink(p.Data);
            else
              dmodel.RemoveLink(l.FromData, l.FromPortId, l.ToData, l.ToPortId);
          }
        }
      }
    }

    // A custom method of RemoveNode to do relinking and correct deletion of "If" subgraphs on deletes
    private void RemoveNode(Node node) {
      if (node == null || !node.Deletable) return;
      var model = this.Diagram.Model;
      switch (node.Category) {
        case "Action":
        case "Effect": {
            // Add links from previous node to next node, then delete selected node
            Node next = GetOtherEnd(node, "BottomPort");
            foreach (Link link in node.LinksInto) {
              if (next != null)
                model.AddLink(link.FromData, link.FromPortId, next.Data, "TopPort");
            }
            model.RemoveNode(node.Data);
            break;
          }
        case "If": {
            // Get the subgraph and find the node where the two sides of the subgraph meet back up,
            // then add links from previous node to next node, then delete selected node and subgraph
            HashSet<Part> coll = new HashSet<Part>();
            Node join = GatherSubGraph(node, coll);
            if (join != null) {
              foreach (Link link in node.LinksInto) {
                model.AddLink(link.FromData, link.FromPortId, join.Data, "TopPort");
              }
            }
            foreach (Part p in coll) {
              Node n = p as Node;
              if (n != null) {
                model.RemoveNode(n.Data);
              } else {
                Link l = p as Link;
                if (l != null)
                  model.RemoveLink(l.FromData, l.FromPortId, l.ToData, l.ToPortId);
              }
            }
            break;
          }
        case "Output": {
            model.RemoveNode(node.Data);
            break;
          }
        default: break;
      }
    }

    // Find the node at the other end of a given port
    public Node GetOtherEnd(Node node, String portid) {
      if (node.FindPort(portid, true) != null)
        if (portid == "TopPort") {
          foreach (Node n in node.FindNodesIntoPort(portid)) {
            return n;
          }
        } else {
          foreach (Node n in node.FindNodesOutOfPort(portid)) {
            // Make sure not to take "Output" nodes because it will prevent correct marking
            if (n.Category != "Output") return n;
          }
        }
      return null;
    }

    // Determine which nodes are part of the "If" node's subgraph
    public Node GatherSubGraph(Node cond, HashSet<Part> coll) {
      if (cond.Category == "If") {
        Mark(GetOtherEnd(cond, "LeftPort"), 1);
        Mark(GetOtherEnd(cond, "RightPort"), 2);
        SelectMarked(cond, 1|2, coll);
        Node join = FindJoin(cond, 1|2) as Node;
        Clear(cond);
        return join;
      }
      return null;
    }

    // Clear the UserFlags on a given node and all nodes out of it
    private void Clear(Node n) {
      if (n == null) return;
      MyNodeData mnd = n.Data as MyNodeData;
      mnd.UserFlags = 0;
      foreach (Node x in n.NodesOutOf) Clear(x);
    }

    // Mark a node as being part of the subgraph
    private void Mark(Node n, int i) {
      if (n == null) return;
      MyNodeData mnd = n.Data as MyNodeData;
      mnd.UserFlags |= i;
      foreach (Node x in n.NodesOutOf) Mark(x, i);
    }

    // Now add marked nodes to the collection
    private void SelectMarked(Node n, int i, HashSet<Part> coll) {
      if (n == null) return;
      MyNodeData mnd = n.Data as MyNodeData;
      if (mnd.UserFlags != i) {
        coll.Add(n);
        foreach (Node x in n.NodesOutOf) SelectMarked(x, i, coll);
      }
    }

    // Find the node where the two sides of the subgraph meet back up
    private Node FindJoin(Node n, int i) {
      if (n == null) return null;
      MyNodeData mnd = n.Data as MyNodeData;
      if (mnd.UserFlags == i) return n;
      foreach (Node x in n.NodesOutOf) {
        Node r = FindJoin(x, i);
        if (r != null) return r;
      }
      return null;
    }
  }


  // The data class representing nodes.
#if !SILVERLIGHT
  [Serializable]
#endif
  public class MyNodeData : GraphLinksModelNodeData<String> {
    // Allows highlighting of nodes
    public bool Highlight {
      get { return _Highlight; }
      set {
        bool old = _Highlight;
        if (old != value) {
          _Highlight = value;
          RaisePropertyChanged("Highlight", old, value);
        }
      }
    }
    private bool _Highlight = false;

    // Allows a flag to be associated with a node
    public int UserFlags { get; set; }

    // Allows for changes in opacity of nodes
    public double Opacity {
      get { return _Opacity; }
      set {
        double old = _Opacity;
        if (old != value) {
          _Opacity = value;
          RaisePropertyChanged("Opacity", old, value);
        }
      }
    }
    private double _Opacity = 1.00;
  }

  // The data class representing links.
#if !SILVERLIGHT
  [Serializable]
#endif
  public class MyLinkData : GraphLinksModelLinkData<String, String> {
    // Allows highlighting of links
    public bool Highlight {
      get { return _Highlight; }
      set {
        bool old = _Highlight;
        if (old != value) {
          _Highlight = value;
          RaisePropertyChanged("Highlight", old, value);
        }
      }
    }
    private bool _Highlight = false;
  }


  // Converter used to display false on links coming from left port of an "If" node
  public class LabelVisibilityConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      Link link = value as Link;
      if (link != null && link.FromNode != null && link.FromPortId == "LeftPort") {
        MyNodeData d = link.FromNode.Data as MyNodeData;
        if (d != null && d.Key.StartsWith("If")) return Visibility.Visible;
      }
      return Visibility.Collapsed;
    }
  }
}

