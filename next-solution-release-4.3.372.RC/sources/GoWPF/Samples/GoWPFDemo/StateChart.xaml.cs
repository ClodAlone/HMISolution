/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace StateChart {
  public partial class StateChart : UserControl {
    public StateChart() {
      InitializeComponent();

      // because we cannot data-bind the Route.Points property,
      // we use a custom PartManager to read/write the Transition.Points data property
      myDiagram.PartManager = new CustomPartManager();

      // create the diagram's data model
      var model = new GraphLinksModel<State, String, String, Transition>();
      // initialize it from data in an XML file that is an embedded resource
      String xml = Demo.MainPage.Instance.LoadText("StateChart", "xml");
      // set the Route.Points after nodes have been built and the layout has finished
      myDiagram.LayoutCompleted += UpdateRoutes;
      model.Load<State, Transition>(XElement.Parse(xml), "State", "Transition");
      model.Modifiable = true;  // let the user modify the graph
      model.HasUndoManager = true;  // support undo/redo
      myDiagram.Model = model;

      // add a tool that lets the user shift the position of the link labels
      var tool = new SimpleLabelDraggingTool();
      tool.Diagram = myDiagram;
      myDiagram.MouseMoveTools.Insert(0, tool);
    }

    // This event handler is called from the Button that is in the Adornment
    // shown for a node when it is selected
    private void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
      // find the Adornment "parent" for the Button
      Adornment ad = Part.FindAncestor<Adornment>(e.OriginalSource as UIElement);
      if (ad == null) return;
      // its AdornedPart should be a Node that is bound to a State object
      State from = ad.AdornedPart.Data as State;
      if (from == null) return;
      // make all changes to the model within a transaction
      myDiagram.StartTransaction("Add State");
      // create a new State, add it to the model, and create a link from
      // the selected node data to the new node data
      State to = new State();
      to.Text = "new";
      Point p = from.Location;
      //?? this isn't a very smart way to decide where to place the node
      to.Location = new Point(p.X + 200, p.Y);
      myDiagram.Model.AddNode(to);
      Node newnode = myDiagram.PartManager.FindNodeForData(to, myDiagram.Model);
      myDiagram.Select(newnode);
      EventHandler<DiagramEventArgs> show = null;
      show = (snd, evt) => {
        myDiagram.Panel.MakeVisible(newnode, Rect.Empty);
        myDiagram.LayoutCompleted -= show;
      };
      myDiagram.LayoutCompleted += show;
      myDiagram.Model.AddLink(from, null, to, null);
      myDiagram.CommitTransaction("Add State");
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<State, String, String, Transition>;
      if (model == null) return;
      // copy the Route.Points into each Transition data
      foreach (Link link in myDiagram.Links) {
        Transition transition = link.Data as Transition;
        if (transition != null) {
          transition.Points = new List<Point>(link.Route.Points);
        }
      }
      XElement root = model.Save<State, Transition>("StateChart", "State", "Transition");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<State, String, String, Transition>;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        // set the Route.Points after nodes have been built and the layout has finished
        myDiagram.LayoutCompleted += UpdateRoutes;
        // tell the CustomPartManager that we're loading
        myDiagram.PartManager.UpdatesRouteDataPoints = false;
        model.Load<State, Transition>(root, "State", "Transition");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }

    // only use the saved route points after the layout has completed,
    // because links will get the default routing
    private void UpdateRoutes(object sender, DiagramEventArgs e) {
      // just set the Route points once per Load
      myDiagram.LayoutCompleted -= UpdateRoutes;
      foreach (Link link in myDiagram.Links) {
        Transition transition = link.Data as Transition;
        if (transition != null && transition.Points != null && transition.Points.Count() > 1) {
          link.Route.Points = (IList<Point>)transition.Points;
        }
      }
      myDiagram.PartManager.UpdatesRouteDataPoints = true;  // OK for CustomPartManager to update Transition.Points automatically
    }
  }


  public class CustomPartManager : PartManager {
    public CustomPartManager() {
      this.UpdatesRouteDataPoints = true;  // call UpdateRouteDataPoints when Link.Route.Points has changed
    }

    // this supports undo/redo of link route reshaping
    protected override void UpdateRouteDataPoints(Link link) {
      if (!this.UpdatesRouteDataPoints) return;   // in coordination with Load_Click and UpdateRoutes, above
      var data = link.Data as Transition;
      if (data != null) {
        data.Points = new List<Point>(link.Route.Points);
      }
    }
  }


  // the data for each node; the predefined data class is enough
#if !SILVERLIGHT
  [Serializable]
#endif
  public class State : GraphLinksModelNodeData<String> {
    public State() {
      this.Key = "0";  // be sure to provide an initial non-null value for the Key
      this.Text = "State";
    }

    // note that adding properties here means also overriding MakeXElement and LoadFromXElement
  }


  // the data for each link
#if !SILVERLIGHT
  [Serializable]
#endif
  public class Transition : GraphLinksModelLinkData<String, String> {
    public Transition() {
      this.Text = "transition";
    }

    // this property remembers the curviness;
    // Double.NaN means let it use a default calculated value
    public double Curviness {
      get { return _Curviness; }
      set {
        if (_Curviness != value) {
          double old = _Curviness;
          _Curviness = value;
          RaisePropertyChanged("Curviness", old, value);
        }
      }
    }
    // default value of NaN causes Route to calculate it
    private double _Curviness = Double.NaN;

    public Point Offset {
      get { return _Offset; }
      set {
        if (_Offset != value) {
          Point old = _Offset;
          _Offset = value;
          RaisePropertyChanged("Offset", old, value);
        }
      }
    }
    private Point _Offset = new Point(0, 0);

    // write the extra property on the link data
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(XHelper.Attribute("Curviness", this.Curviness, Double.NaN));
      e.Add(XHelper.Attribute("Offset", this.Offset, new Point(0, 0)));
      return e;
    }

    // read the extra property on the link data
    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Curviness = XHelper.Read("Curviness", e, Double.NaN);
      this.Offset = XHelper.Read("Offset", e, new Point(0, 0));
    }
  }


  // This tool only works when a Link has a LinkPanel with a single child element
  // that is positioned at the Route.MidPoint plus some Offset.
  public class SimpleLabelDraggingTool : DiagramTool {
    public override bool CanStart() {
      if (!base.CanStart()) return false;
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      // require left button & that it has moved far enough away from the mouse down point, so it isn't a click
      if (!IsLeftButtonDown()) return false;
      if (!IsBeyondDragSize()) return false;
      return FindLabel() != null;
    }

    private FrameworkElement FindLabel() {
#if SILVERLIGHT
      var elt = this.Diagram.Panel.FindElementAt<UIElement>(this.Diagram.LastMousePointInModel, e => e, null, SearchLayers.Links);
      if (elt == null) return null;
      Link link = Part.FindAncestor<Link>(elt);
      if (link == null) return null;
      var parent = System.Windows.Media.VisualTreeHelper.GetParent(elt) as UIElement;
      while (parent != null && parent != link && !(parent is LinkPanel)) {
        elt = parent;
        parent = System.Windows.Media.VisualTreeHelper.GetParent(elt) as UIElement;
      }
#else
      var elt = this.Diagram.Panel.FindElementAt<System.Windows.Media.Visual>(this.Diagram.LastMousePointInModel, e => e, null, SearchLayers.Links);
      if (elt == null) return null;
      Link link = Part.FindAncestor<Link>(elt);
      if (link == null) return null;
      var parent = System.Windows.Media.VisualTreeHelper.GetParent(elt) as System.Windows.Media.Visual;
      while (parent != null && parent != link && !(parent is LinkPanel)) {
        elt = parent;
        parent = System.Windows.Media.VisualTreeHelper.GetParent(elt) as System.Windows.Media.Visual;
      }
#endif
      if (parent is LinkPanel) {
        FrameworkElement lab = elt as FrameworkElement;
        if (lab == null) return null;
        // needs to be positioned relative to the MidPoint
        if (LinkPanel.GetIndex(lab) != Int32.MinValue) return null;
        // also check for movable-ness?
        return lab;
      }
      return null;
    }

    public override void DoActivate() {
      StartTransaction("Shifted Label");
      this.Label = FindLabel();
      if (this.Label != null) {
        this.OriginalOffset = LinkPanel.GetOffset(this.Label);
      }
      base.DoActivate();
    }

    public override void DoDeactivate() {
      base.DoDeactivate();
      StopTransaction();
    }

    private FrameworkElement Label { get; set; }
    private Point OriginalOffset { get; set; }

    public override void DoStop() {
      this.Label = null;
      base.DoStop();
    }

    public override void DoCancel() {
      if (this.Label != null) {
        LinkPanel.SetOffset(this.Label, this.OriginalOffset);
      }
      base.DoCancel();
    }

    public override void DoMouseMove() {
      if (!this.Active) return;
      UpdateLinkPanelProperties();
    }

    public override void DoMouseUp() {
      if (!this.Active) return;
      UpdateLinkPanelProperties();
      this.TransactionResult = "Shifted Label";
      StopTool();
    }

    private void UpdateLinkPanelProperties() {
      if (this.Label == null) return;
      Link link = Part.FindAncestor<Link>(this.Label);
      if (link == null) return;
      Point last = this.Diagram.LastMousePointInModel;
      Point mid = link.Route.MidPoint;
      // need to rotate this point to account for angle of middle segment
      Point p = new Point(last.X-mid.X, last.Y-mid.Y);
      LinkPanel.SetOffset(this.Label, RotatePoint(p, -link.Route.MidAngle));
    }

    private static Point RotatePoint(Point p, double angle) {
      if (angle == 0 || (p.X == 0 && p.Y == 0))
        return p;
      double rad = angle * Math.PI / 180;
      double cosine = Math.Cos(rad);
      double sine = Math.Sin(rad);
      return new Point((cosine * p.X - sine * p.Y),
                       (sine * p.X + cosine * p.Y));
    }
  }
}
