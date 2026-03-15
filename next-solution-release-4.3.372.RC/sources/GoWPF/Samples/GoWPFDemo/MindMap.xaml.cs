/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Tool;
using System.Collections.Generic;

namespace MindMap {
  public partial class MindMap : UserControl {
    public MindMap() {
      InitializeComponent();

      // the model is a tree of MindMapData, indexed by strings
      var model = new TreeModel<MindMapData, String>();
      nodePalette.Model = new TreeModel<MindMapData, String>();

      // Default root node, will be replaced if anything is loaded
      model.NodesSource = new ObservableCollection<MindMapData>() {
        new MindMapData() { Key="/", Category="Root", Text="Mind Map", Location= new Point(0,0)}
      };

      // Initialize it from data in an XML file that is an embedded resource
      String xml = Demo.MainPage.Instance.LoadText("MindMap", "xml");
      model.Load<MindMapData>(XElement.Parse(xml), "MindMapData");

      nodePalette.Model.NodesSource = new ObservableCollection<MindMapData>() {
        new MindMapData() {  Key="Note", Category="Note", Text="Note", Figure=NodeFigure.Rectangle},
        new MindMapData() {  Key="Title", Category="Title", Text="Title", Figure=NodeFigure.Rectangle}
      };
      
      model.Modifiable = true;
      model.HasUndoManager = true;
      myDiagram.Model = model;
      myDiagram.AllowDrop = true;

      myDiagram.ClickCreatingTool.DoubleClick = true;
      myDiagram.ClickCreatingTool.PrototypeData = new MindMapData() { LayoutId = "None", Key = "/", Figure = NodeFigure.Rectangle };

      myDiagram.NodeCreated += myDiagram_NodeCreated;
      myDiagram.SelectionMoved +=myDiagram_SelectionMoved;
      myDiagram.SelectionDeleting += myDiagram_SelectionDeleting;
      
      printButton.Command = myDiagram.CommandHandler.PrintCommand;
      myDiagram.MouseRightButtonUp += CreateChild;
    }

    // When deleting a selection, don't let the root node partake
    private void myDiagram_SelectionDeleting(object sender, DiagramEventArgs e) {
      Node root = myDiagram.SelectedParts.OfType<Node>().Where(n => ((MindMapData)n.Data).Key == "/").FirstOrDefault();
      myDiagram.SelectedParts.Remove(root);
    }

    // if a node next to root moves to the other side, change its layout
    private void myDiagram_SelectionMoved(object sender, DiagramEventArgs e) {
      // find all nodes in the selection that are children of the root
      var rootChildren = myDiagram.SelectedParts.OfType<Node>().Where(n => ((MindMapData)n.Data).ParentKey == "/");

      foreach (Node n in rootChildren) {
        MindMapData root = (MindMapData)myDiagram.Model.FindNodeByKey("/");
        double rootX = root.Location.X;
        double nodeX = n.Location.X;
        if (rootX < nodeX && n.LayoutId != "Right") {
          FlipChildren(n, "Right");
        } else if (rootX > nodeX && n.LayoutId != "Left") {
          FlipChildren(n, "Left");
        }
      }
    }

    private void FlipChildren(Node n, String direction) {
      n.LayoutId = direction;
      IEnumerable<Node> collectedNodes = n.FindTreeParts(EffectiveCollectionInclusions.SubTree).OfType<Node>();
      IEnumerable<Link> collectedLinks = n.FindTreeParts(EffectiveCollectionInclusions.SubTree).OfType<Link>();
      foreach (Node child in collectedNodes) {
        MindMapData d = (MindMapData)child.Data;
        child.LayoutId = direction;
        if (direction == "Left") {
          d.ToSpot = "MiddleRight";
          d.FromSpot = "MiddleLeft";
        } else {
          d.ToSpot = "MiddleLeft";
          d.FromSpot = "MiddleRight";
        }
      }

      //select the layout to use, either Right or Left
      var layout =
        (from x in ((MultiLayout)myDiagram.Layout).Layouts.OfType<TreeLayout>()
         where ((TreeLayout)x).Id == direction
         select x).FirstOrDefault();

      if (layout != null)
        layout.DoLayout(collectedNodes, collectedLinks);
    }

    // populate lists with all recursive children and links of a given node
    private void CollectChildrenAndLinks(Node n, List<Node> collectedNodes, List<Link> collectedLinks) {
      foreach (Node child in n.NodesOutOf)
        CollectChildrenAndLinks(child, collectedNodes, collectedLinks);
      collectedNodes.Add(n);
      foreach (Link l in n.LinksOutOf)
        collectedLinks.Add(l);
    }

    // Initializes most of the MindMapData for nodes created by doubleclick and right click
    private MindMapData SetUpChild(MindMapData childdata, MindMapData parentdata) {
      childdata.Text = "New Node";
      childdata.Color = "White";
      childdata.ParentKey = parentdata.Key;

      if (parentdata.LinkColor == "LightGray") {
        Random r = new Random();
        childdata.LinkColor = myColors[r.Next(myColors.Count())];
      } else {
        childdata.LinkColor = parentdata.LinkColor;
      }
      // we need the root's location to set the direction of the layout
      MindMapData rootData = (MindMapData)myDiagram.Model.FindNodeByKey("/");
      if (rootData.Location.X < childdata.Location.X) {
        childdata.LayoutId = "Right";
        childdata.ToSpot = "MiddleLeft";
        childdata.FromSpot = "MiddleRight";
      } else {
        childdata.LayoutId = "Left";
        childdata.ToSpot = "MiddleRight";
        childdata.FromSpot = "MiddleLeft";
      }

      return childdata;
    }

    // Used when a node is created by right clicking
    private void CreateChild(object sender, RoutedEventArgs e) {
      FrameworkElement elt = myDiagram.Panel.FindElementAt<FrameworkElement>(myDiagram.LastMousePointInModel,
                                          x => x as FrameworkElement, x => true, SearchLayers.Nodes);
      Node n = null;
      if (e.OriginalSource is Button) {
        Adornment ad = Part.FindAncestor<Adornment>(e.OriginalSource as UIElement);
        if (ad == null) return;
        n = ad.AdornedPart as Node;
      } else {
        n = Part.FindAncestor<Node>(elt);
      }
      if (n != null && ((MindMapData)n.Data).Key != "/") {
        // always make changes within a transaction
        myDiagram.StartTransaction("CreateNode");
        MindMapData childdata = new MindMapData();
        MindMapData parentdata = (MindMapData)n.Data;
        childdata.Location = new Point(parentdata.Location.X, parentdata.Location.Y);
        childdata.Key = parentdata.Key + "/";
        SetUpChild(childdata, parentdata);
        myDiagram.Model.AddNode(childdata);
        myDiagram.CommitTransaction("CreateNode");
      }
    }

    // When a node is created with ClickCreatingTool we must do some additional layout
    void myDiagram_NodeCreated(object sender, DiagramEventArgs e) {
      Node n = (Node)e.Part;
      MindMapData data = (MindMapData)n.Data;
      MindMapData parentData = (MindMapData)myDiagram.Model.FindNodeByKey("/");
      n.Data = SetUpChild(data, parentData);
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as TreeModel<MindMapData, String>;
      if (model == null) return;
      XElement root = model.Save<MindMapData>("MindMap", "MindMapData");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as TreeModel<MindMapData, String>;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        model.Load<MindMapData>(root, "MindMapData");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }

    String[] myColors = { "Skyblue", "DarkSeaGreen", "Tomato", "PaleVioletRed", "LightSteelBlue" };
  }


  // Add properties that are essential for proper layout
  public class MindMapData : TreeModelNodeData<String> {
    public String Color { get; set; }

    // A Color which describes the stroke used in the SimpleLinkTemplate.
    public String LinkColor {
      get { return _LinkColor; }
      set {
        if (_LinkColor != value) {
          String old = _LinkColor;
          _LinkColor = value;
          RaisePropertyChanged("LinkColor", old, value);
        }
      }
    }
    private String _LinkColor = "LightGray";

    public String ToSpot {
      get { return _ToSpot; }
      set {
        if (_ToSpot != value) {
          String old = _ToSpot;
          _ToSpot = value;
          RaisePropertyChanged("ToSpot", old, value);
        }
      }
    }
    private String _ToSpot = "";

    public String FromSpot {
      get { return _FromSpot; }
      set {
        if (_FromSpot != value) {
          String old = _FromSpot;
          _FromSpot = value;
          RaisePropertyChanged("FromSpot", old, value);
        }
      }
    }
    private String _FromSpot = "";

    public String LayoutId {
      get { return _LayoutId; }
      set {
        if (_LayoutId != value) {
          String old = _LayoutId;
          _LayoutId = value;
          RaisePropertyChanged("LayoutId", old, value);
        }
      }
    }
    private String _LayoutId = "";

    public NodeFigure Figure {
      get { return _Figure; }
      set {
        if (_Figure != value) {
          NodeFigure old = _Figure;
          _Figure = value;
          RaisePropertyChanged("Figure", old, value);
        }
      }
    }
    private NodeFigure _Figure = NodeFigure.Rectangle;

    // support standard reading/writing via Linq for XML
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(XHelper.AttributeEnum<NodeFigure>("Figure", this.Figure, NodeFigure.Rectangle));
      e.Add(XHelper.Attribute("ToSpot", this.ToSpot, ""));
      e.Add(XHelper.Attribute("FromSpot", this.FromSpot, ""));
      e.Add(XHelper.Attribute("Color", this.Color, "White"));
      e.Add(XHelper.Attribute("LinkColor", this.LinkColor, "LightGray"));
      e.Add(XHelper.Attribute("LayoutId", this.LayoutId, "All"));
      return e;
    }
    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Figure = XHelper.ReadEnum<NodeFigure>("Figure", e, NodeFigure.Rectangle);
      this.ToSpot = XHelper.Read("ToSpot", e, "");
      this.FromSpot = XHelper.Read("FromSpot", e, "");
      this.Color = XHelper.Read("Color", e, "White");
      this.LinkColor = XHelper.Read("LinkColor", e, "LightGray");
      this.LayoutId = XHelper.Read("LayoutId", e, "All");
    }
  }
}