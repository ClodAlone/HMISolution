/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace DraggableLink {
  public partial class DraggableLink : UserControl {
    public DraggableLink() {
      InitializeComponent();

      // palette
      var pmodel = new MyModel();
      pmodel.ValidUnconnectedLinks = ValidUnconnectedLinks.Allowed;
      pmodel.NodesSource = new ObservableCollection<MyData>() {
          new MyData() { Text="Alpha", Color="Pink" },
          new MyData() { Text="Beta", Color="PapayaWhip", Figure=NodeFigure.Diamond },
          new MyData() { Text="Gamma", Color="PeachPuff", Figure=NodeFigure.Database }
      };
      pmodel.LinksSource = new ObservableCollection<MyLinkData>() {
          new MyLinkData() { Points=new List<Point>() { new Point(0, 0), new Point(25, 0), new Point(25, 25), new Point(50, 25) } },
          new MyLinkData() { Points=new List<Point>() { new Point(50, 50), new Point(25, 50), new Point(25, 75), new Point(0, 75) } },
      };
      myPalette.Model = pmodel;

      // initial diagram
      var model = new MyModel();
      model.Modifiable = true;
      model.ValidUnconnectedLinks = ValidUnconnectedLinks.Allowed;
      model.NodesSource = new ObservableCollection<MyData>() {
          new MyData() { Key="A", Text="A", Color="Yellow", Figure=NodeFigure.ACvoltageSource, Location = new Point(50, 200) },
          new MyData() { Key="B", Text="B", Color="Aquamarine", Figure=NodeFigure.Alternative, Location = new Point(200, 50) },
          new MyData() { Key="C", Text="C", Color="Wheat", Figure=NodeFigure.AndGate, Location = new Point(200, 200), Width=100, Height=50 },
      };
      model.LinksSource = new ObservableCollection<MyLinkData>() {
          new MyLinkData() { From="A", Text="only connected to A", Points=new List<Point>() { new Point(50, 230), new Point(50, 240), new Point(50, 244), new Point(103, 244), new Point(103, 283), new Point(103, 293) } }
      };
      myDiagram.LayoutCompleted += LoadLinkRoutes;
      myDiagram.Model = model;
      model.HasUndoManager = true;
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as MyModel;
      if (model == null) return;
      // no data-binding of Route.Points means we have to copy the Points data explicitly
      foreach (Link link in myDiagram.Links) {
        var d = link.Data as MyLinkData;
        if (d == null) continue;
        d.Points = link.Route.Points;
      }
      XElement root = model.Save<MyData, MyLinkData>("Diagram", "Node", "Link");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as MyModel;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        // set the Route.Points after nodes have been built and the layout has finished
        myDiagram.LayoutCompleted += LoadLinkRoutes;
        // tell the CustomPartManager that we're loading
        myDiagram.PartManager.UpdatesRouteDataPoints = false;
        model.Load<MyData, MyLinkData>(root, "Node", "Link");
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
      myDiagram.PartManager.UpdatesRouteDataPoints = true;  // OK for CustomPartManager to update Transition.Points automatically
    }
  }


  public class MyModel : GraphLinksModel<MyData, String, String, MyLinkData> {
    // nothing to add or override, for now
  }


  // data representing important state about each node
#if !SILVERLIGHT
  [Serializable]
#endif
  public class MyData : GraphLinksModelNodeData<String> {
    public NodeFigure Figure {
      get { return _Figure; }
      set { if (_Figure != value) { NodeFigure old = _Figure; _Figure = value; RaisePropertyChanged("Figure", old, value); } }
    }
    private NodeFigure _Figure = NodeFigure.Rectangle;

    public String Color {
      get { return _Color; }
      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
    }
    private String _Color = "White";

    public double Width {
      get { return _Width; }
      set { if (_Width != value) { double old = _Width; _Width = value; RaisePropertyChanged("Width", old, value); } }
    }
    private double _Width = 60;

    public double Height {
      get { return _Height; }
      set { if (_Height != value) { double old = _Height; _Height = value; RaisePropertyChanged("Height", old, value); } }
    }
    private double _Height = 60;

    public double Angle {
      get { return _Angle; }
      set { if (_Angle != value) { double old = _Angle; _Angle = value; RaisePropertyChanged("Angle", old, value); } }
    }
    private double _Angle;

    // Because we added properties that we want to persist,
    // we should override these two methods to write/read those properties.
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(XHelper.AttributeEnum<NodeFigure>("Figure", this.Figure, NodeFigure.Rectangle));
      e.Add(XHelper.Attribute("Color", this.Color, "White"));
      e.Add(XHelper.Attribute("Width", this.Width, 50.0));
      e.Add(XHelper.Attribute("Height", this.Height, 50.0));
      e.Add(XHelper.Attribute("Angle", this.Angle, 0.0));
      return e;
    }

    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Figure = XHelper.ReadEnum<NodeFigure>("Figure", e, NodeFigure.Rectangle);
      this.Color = XHelper.Read("Color", e, "White");
      this.Width = XHelper.Read("Width", e, 50.0);
      this.Height = XHelper.Read("Height", e, 50.0);
      this.Angle = XHelper.Read("Angle", e, 0.0);
    }
  }


  // data representing important state about each link
#if !SILVERLIGHT
  [Serializable]
#endif
  public class MyLinkData : GraphLinksModelLinkData<String, String> {
    // nothing to add or override, for now
  }


  // no data-binding of Route.Points means we have to copy the Points data explicitly
  public class CustomPartManager : PartManager {
    public CustomPartManager() {
      this.UpdatesRouteDataPoints = true;  // call UpdateRouteDataPoints when Link.Route.Points has changed
    }

    // copy Route.Points to MyLinkData
    public override ICopyDictionary CopyParts(IEnumerable<Part> coll, IDiagramModel destmodel) {
      ICopyDictionary dict = base.CopyParts(coll, destmodel);
      foreach (object data in dict.SourceCollection.Links) {
        MyLinkData origdata = data as MyLinkData;
        Link origlink = FindLinkForData(origdata, this.Diagram.Model);
        if (origlink != null && origlink.Route.PointsCount > 1) {
          // copy the MyLinkData.Points
          MyLinkData copieddata = dict.FindCopiedLink(origdata) as MyLinkData;
          if (copieddata != null) {
            copieddata.Points = new List<Point>(origlink.Route.Points);
            // now transfer to the Link.Route.Points
            Link copiedlink = FindLinkForData(copieddata, this.Diagram.Model);
            if (copiedlink != null) {
              copiedlink.Route.Points = (IList<Point>)copieddata.Points;
            }
          }
        }
      }
      return dict;
    }

    // use MyLinkData.Points, if any, when creating a Link
    protected override void OnLinkAdded(Link link) {
      base.OnLinkAdded(link);
      MyLinkData data = link.Data as MyLinkData;
      if (data != null && data.Points != null) {
        link.Route.Points = (IList<Point>)data.Points;
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


  // put rotation handle above node instead of on the right side
  public class CustomRotatingTool : RotatingTool {
    private const String ToolCategory = "Rotate";
    public override void UpdateAdornments(Part part) {
      if (part == null || part is Link) return;  // this tool never applies to Links
      Adornment adornment = null;
      if (part.IsSelected) {
        FrameworkElement selelt = part.SelectionElement;
        if (selelt != null && part.CanRotate() && Part.IsVisibleElement(selelt)) {
          adornment = part.GetAdornment(ToolCategory);
          if (adornment == null) {
            DataTemplate template = part.RotateAdornmentTemplate;
            if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultRotateAdornmentTemplate");
            adornment = part.MakeAdornment(selelt, template);
            if (adornment != null) {
              adornment.Category = ToolCategory;
              adornment.LocationSpot = Spot.Center;
            }
          }
          if (adornment != null) {
            adornment.Location = part.GetElementPoint(selelt, new Spot(0.5, 0, 0, -30));  // above middle top
          }
        }
      }
      part.SetAdornment(ToolCategory, adornment);
    }

    protected override void DoRotate(double newangle) {
      base.DoRotate(newangle + 90);
    }

    public override void DoCancel() {
      base.DoRotate(this.OriginalAngle);
      StopTool();
    }
  }
}
