/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace FlowChart {

  public partial class FlowChart : UserControl {
    public FlowChart() {
      InitializeComponent();

      // use custom tools and PartManager; these could have been defined in XAML also
      myDiagram.PartManager = new MyPartManager();
      myDiagram.LinkingTool = new MyLinkingTool();
      myDiagram.RelinkingTool = new MyRelinkingTool();

      myDiagram.AllowDrop = true;

      // set up the model for the Palette
      var paletteModel = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();
      // this creates the palette's model's data in code:
      paletteModel.NodesSource = new List<MyNodeData>() {
        new MyNodeData() { Key="Comment",  Text="Comment",  Category="Comment", Figure=NodeFigure.Rectangle },
        new MyNodeData() { Key="Start",    Text="Start",    Category="Start", Figure=NodeFigure.RoundedRectangle },
        new MyNodeData() { Key="Step",     Text="Step",     Category="Standard", Figure=NodeFigure.Rectangle },
        new MyNodeData() { Key="Input",    Text="Input",    Category="Standard", Figure=NodeFigure.Input },
        new MyNodeData() { Key="Output",   Text="Output",   Category="Standard", Figure=NodeFigure.Output },
        new MyNodeData() { Key="Conditional", Text="?",     Category="Standard", Figure=NodeFigure.Diamond },
        new MyNodeData() { Key="Read",     Text="Read",     Category="Standard", Figure=NodeFigure.Ellipse },
        new MyNodeData() { Key="Write",    Text="Write",    Category="Standard", Figure=NodeFigure.Ellipse },
        new MyNodeData() { Key="ManualOperation", Text="Manual Operation", Category="Standard", Figure=NodeFigure.ManualOperation },
        new MyNodeData() { Key="DataBase", Text="DataBase", Category="Standard", Figure=NodeFigure.Database },
        new MyNodeData() { Key="End",      Text="End",      Category="End", Figure=NodeFigure.RoundedRectangle }
      };
      myPalette.Model = paletteModel;

      // set up an initial model for the Diagram
      var model = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();
      // and initialize it from the XML file that is an embedded resource
      String xml = Demo.MainPage.Instance.LoadText("FlowChart", "xml");
      model.Load<MyNodeData, MyLinkData>(XElement.Parse(xml), "MyNodeData", "MyLinkData");
      model.Modifiable = true;
      model.HasUndoManager = true;
      myDiagram.Model = model;
    }

    // Sets the Tag property on a Node object to true
    // when the MouseEnter event is called on the Node's SpotPanel.
    // The binding on the Stroke will change to a Black brush.
    private void Node_MouseEnter(object sender, MouseEventArgs e) {
      SetPortsVisible(sender as UIElement, true);
    }

    // Sets the Tag property on a Node object to false
    // when the MouseEnter event is called on the Node's SpotPanel.
    // The binding on the Stroke will change to a Transparent brush.
    private void Node_MouseLeave(object sender, MouseEventArgs e) {
      SetPortsVisible(sender as UIElement, false);
    }

    // Used in the above two methods to change the node's Tag property to a specified value.
    private void SetPortsVisible(UIElement uielement, bool visible) {
      if (Part.FindAncestor<Palette>(uielement) == null) {
        SpotPanel sp = uielement as SpotPanel;
        if (sp == null) return;
        Node n = Part.FindAncestor<Node>(sp);
        if (n == null) return;
        n.Tag = visible;
      }
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<MyNodeData, String, String, MyLinkData>;
      if (model == null) return;
      XElement root = model.Save<MyNodeData, MyLinkData>("FlowChart", "MyNodeData", "MyLinkData");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<MyNodeData, String, String, MyLinkData>;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        // set the Route.Points after nodes have been built and the layout has finished
        myDiagram.LayoutCompleted += LoadLinkRoutes;
        // tell the CustomPartManager that we're loading
        myDiagram.PartManager.UpdatesRouteDataPoints = false;
        model.Load<MyNodeData, MyLinkData>(root, "MyNodeData", "MyLinkData");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }

    // no data-binding of Route.Points means we have to copy the Points data explicitly
    private void LoadLinkRoutes(Object s, EventArgs e) {
      // just set the Route points once per Load
      myDiagram.LayoutCompleted -= LoadLinkRoutes;
      foreach (Link link in myDiagram.Links) {
        var d = link.Data as MyLinkData;
        if (d == null || d.Points == null) continue;
        link.Route.Points = (IList<Point>)d.Points;
      }
      myDiagram.PartManager.UpdatesRouteDataPoints = true;  // OK for MyPartManager to update MyLinkData.Points automatically
    }
  }


  // The data class representing nodes.
#if !SILVERLIGHT
  [Serializable]
#endif
  public class MyNodeData : GraphLinksModelNodeData<String> {
    // The shape of each node
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

    // note that adding properties here means also adding lines to MakeXElement and LoadFromXElement

    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(XHelper.AttributeEnum<NodeFigure>("Figure", this.Figure, NodeFigure.Rectangle));
      return e;
    }

    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Figure = XHelper.ReadEnum<NodeFigure>("Figure", e, NodeFigure.Rectangle);
    }
  }


  // The data class representing links.  Actually, there's nothing to add to the predefine link data class.
#if !SILVERLIGHT
  [Serializable]
#endif
  public class MyLinkData : GraphLinksModelLinkData<String, String> {
    public MyLinkData() {
      this.Text = "Yes";
    }

    // note that adding properties here means also overriding MakeXElement and LoadFromXElement
  }


  // Augment the normal process of adding nodes to the diagram by adjusting the position
  // of the left and right ports of those nodes showing figures that have slanted sides.
  // An alternative is to use a different DataTemplate for nodes with these kinds of figures.
  public class MyPartManager : PartManager {
    public MyPartManager() {
      this.UpdatesRouteDataPoints = true;  // call UpdateRouteDataPoints when Link.Route.Points has changed
    }

    protected override void OnNodeAdded(Node node) {
      base.OnNodeAdded(node);
      MyNodeData data = node.Data as MyNodeData;
      if (data == null) return;
      // look at the node's Figure
      switch (data.Figure) {
        case NodeFigure.Input:
        case NodeFigure.Output:
        case NodeFigure.ManualOperation: {
            FrameworkElement leftport = node.FindPort("2", false);
            if (leftport != null) SpotPanel.SetSpot(leftport, new Spot(0.05, 0.5, 1, 0));
            FrameworkElement rightport = node.FindPort("3", false);
            if (rightport != null) SpotPanel.SetSpot(rightport, new Spot(0.95, 0.5, -1, 0));
            break;
          }
        default: break;
      }
    }

    // this supports undo/redo of link route reshaping
    protected override void UpdateRouteDataPoints(Link link) {
      if (!this.UpdatesRouteDataPoints) return;   // in coordination with Load_Click and LoadLinkRoutes, above
      MyLinkData data = link.Data as MyLinkData;
      if (data != null) {
        data.Points = new List<Point>(link.Route.Points);
      }
    }
  }


  // Overrides the default LinkingTool to make the Ports appear and disappear when linking begins and ends.
  public class MyLinkingTool : LinkingTool {
    // set the Tag property of every Node in the LinkingTool.Diagram.Nodes to true.
    public override void DoStart() {
      SetAllNodesPortsVisible(true);
      base.DoStart();
    }

    // set the Tag property of every Node in the LinkingTool.Diagram.Nodes to false.
    public override void DoStop() {
      base.DoStop();
      SetAllNodesPortsVisible(false);
    }

    // Sets the Tag property of all Nodes in this.Diagram.Nodes to the bool parameter.
    private void SetAllNodesPortsVisible(bool visibility) {
      foreach (Node n in this.Diagram.Nodes) {
        n.Tag = visibility;
      }
    }

    public override bool IsValidLink(Node fromnode, FrameworkElement fromport, Node tonode, FrameworkElement toport) {
      if (!base.IsValidLink(fromnode, fromport, tonode, toport)) return false;
      // don't allow a link directly from Start to End
      MyNodeData fromnodedata = fromnode.Data as MyNodeData;
      MyNodeData tonodedata = tonode.Data as MyNodeData;
      if (fromnodedata != null && fromnodedata.Category == "Start" &&
          tonodedata != null && tonodedata.Category == "End") return false;
      return true;
    }
  }


  // Overrides the default RelinkingTool to make the Ports appear and disappear when linking begins and ends.
  public class MyRelinkingTool : RelinkingTool {
    // set the Tag property of every Node in the LinkingTool.Diagram.Nodes to true.
    public override void DoStart() {
      SetAllNodesPortsVisible(true);
      base.DoStart();
    }

    // set the Tag property of every Node in the LinkingTool.Diagram.Nodes to false.
    public override void DoStop() {
      base.DoStop();
      SetAllNodesPortsVisible(false);
    }

    // Sets the Tag property of all Nodes in this.Diagram.Nodes to the bool parameter.
    private void SetAllNodesPortsVisible(bool visibility) {
      foreach (Node n in this.Diagram.Nodes) {
        n.Tag = visibility;
      }
    }

    public override bool IsValidLink(Node fromnode, FrameworkElement fromport, Node tonode, FrameworkElement toport) {
      if (!base.IsValidLink(fromnode, fromport, tonode, toport)) return false;
      // don't allow a link directly from Start to End
      MyNodeData fromnodedata = fromnode.Data as MyNodeData;
      MyNodeData tonodedata = tonode.Data as MyNodeData;
      if (fromnodedata != null && fromnodedata.Category == "Start" &&
          tonodedata != null && tonodedata.Category == "End") return false;
      return true;
    }
  }

  // link labels are visible when they come from a "Conditional" node
  public class LabelVisibilityConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      Link link = value as Link;
      if (link != null && link.FromNode != null) {
        MyNodeData d = link.FromNode.Data as MyNodeData;
        if (d != null && d.Key.StartsWith("Conditional")) return Visibility.Visible;
      }
      return Visibility.Collapsed;
    }
  }
}