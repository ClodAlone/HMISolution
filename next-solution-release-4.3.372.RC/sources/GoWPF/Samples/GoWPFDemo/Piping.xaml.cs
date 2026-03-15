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

namespace Piping {
  public partial class Piping : UserControl {
    public Piping() {
      InitializeComponent();

      // because we cannot data-bind the Route.Points property,
      // we use a custom PartManager to read/write the PipeData.Points data property
      myDiagram.PartManager = new CustomPartManager();

      // create the diagram's data model
      var model = new MyModel();
      // initialize it from data in an XML file that is an embedded resource
      String xml = Demo.MainPage.Instance.LoadText("Piping", "xml");
      // set the Route.Points for all of the links *after* the diagram has been created
      myDiagram.LayoutCompleted += LoadLinkRoutes;
      model.Load<ItemData, PipeData>(XElement.Parse(xml), AllocateItemData, AllocatePipeData);
      model.Modifiable = true;  // let the user modify the graph
      model.HasUndoManager = true;  // support undo/redo
      myDiagram.Model = model;

      // there is one of each kind of node in the Palette
      var palettemodel = new MyModel();
      palettemodel.NodesSource = new List<ItemData>() {
        new ProcessData() { Key="P", Text="Process Unit" },
        new ValveData() { Key="V", Text="Valve"},
        new InstrumentData() { Key="I", Text="Instr" },
      };
      myPalette.Model = palettemodel;
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as MyModel;
      if (model == null) return;
      // no data-binding of Route.Points means we have to copy the Points data explicitly
      foreach (Link link in myDiagram.Links) {
        PipeData d = link.Data as PipeData;
        if (d == null) continue;
        d.Points = link.Route.Points;
      }
      XElement root = model.Save<ItemData, PipeData>("Diagram", "Item", "Pipe");
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
        model.Load<ItemData, PipeData>(root, AllocateItemData, AllocatePipeData);
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }
    private ItemData AllocateItemData(XElement xe) {
      if (xe.Name == "Process") return new ProcessData();
      if (xe.Name == "Valve") return new ValveData();
      if (xe.Name == "Instrument") return new InstrumentData();
      return null;
    }
    private PipeData AllocatePipeData(XElement xe) {
      if (xe.Name == "Pipe") return new PipeData();
      return null;
    }

    private void LoadLinkRoutes(Object s, EventArgs e) {
      // just set the Route points once per Load
      myDiagram.LayoutCompleted -= LoadLinkRoutes;
      foreach (Link link in myDiagram.Links) {
        PipeData d = link.Data as PipeData;
        if (d == null || d.Points == null) continue;
        link.Route.Points = (IList<Point>)d.Points;
      }
      myDiagram.PartManager.UpdatesRouteDataPoints = true;  // OK for CustomPartManager to update PipeData.Points automatically
    }
  }


  public class CustomPartManager : PartManager {
    public CustomPartManager() {
      this.UpdatesRouteDataPoints = true;  // call UpdateRouteDataPoints when Link.Route.Points has changed
    }

    // this supports undo/redo of link route reshaping
    protected override void UpdateRouteDataPoints(Link link) {
      if (!this.UpdatesRouteDataPoints) return;   // in coordination with Load_Click and LoadLinkRoutes, above
      var data = link.Data as PipeData;
      if (data != null) {
        data.Points = new List<Point>(link.Route.Points);
      }
    }
  }


  public class MyModel : GraphLinksModel<ItemData, String, String, PipeData> {
    // nothing to override
  }


  // This represents a node in the model.
  // The standard node data provides other properties that we use:
  // Key, Text, Category, Location
  // There are three subclasses, for Processes, Valves, and Instruments.
  // Each has its own Category and thus its own DataTemplate.
#if !SILVERLIGHT
  [Serializable]
#endif
  public abstract class ItemData : GraphLinksModelNodeData<String> {
    // nothing to add here
  }


#if !SILVERLIGHT
  [Serializable]
#endif
  public class ProcessData : ItemData {
    public ProcessData() {
      this.Category = "Process";
    }

    public double Width {
      get { return _Width; }
      set { if (_Width != value) { double old = _Width; _Width = value; RaisePropertyChanged("Width", old, value); } }
    }
    private double _Width = 60;

    public double Height {
      get { return _Height; }
      set { if (_Height != value) { double old = _Height; _Height = value; RaisePropertyChanged("Height", old, value); } }
    }
    private double _Height = 100;

    // Because we added properties that we want to persist,
    // we should override these two methods to write/read those properties.
    // Also, the MakeXElement override ensures the element is named "Process".
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement("Process");
      e.Add(XHelper.Attribute("Width", this.Width, 60.0));
      e.Add(XHelper.Attribute("Height", this.Height, 100.0));
      return e;
    }

    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Width = XHelper.Read("Width", e, 60.0);
      this.Height = XHelper.Read("Height", e, 100.0);
    }
  }


#if !SILVERLIGHT
  [Serializable]
#endif
  public class ValveData : ItemData {
    public ValveData() {
      this.Category = "Valve";
    }

    public double Angle {
      get { return _Angle; }
      set { if (_Angle != value) { double old = _Angle; _Angle = value; RaisePropertyChanged("Angle", old, value); } }
    }
    private double _Angle;

    // Because we added properties that we want to persist,
    // we should override these two methods to write/read those properties.
    // Also, the MakeXElement override ensures the element is named "Valve".
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement("Valve");
      e.Add(XHelper.Attribute("Angle", this.Angle, 0.0));
      return e;
    }

    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Angle = XHelper.Read("Angle", e, 0.0);
    }
  }


#if !SILVERLIGHT
  [Serializable]
#endif
  public class InstrumentData : ItemData {
    public InstrumentData() {
      this.IsLinkLabel = true;
      this.Category = "Instrument";
    }

    public double Width {
      get { return _Width; }
      set { if (_Width != value) { double old = _Width; _Width = value; RaisePropertyChanged("Width", old, value); } }
    }
    private double _Width = 40;

    public double Height {
      get { return _Height; }
      set { if (_Height != value) { double old = _Height; _Height = value; RaisePropertyChanged("Height", old, value); } }
    }
    private double _Height = 40;

    public double Fraction {
      get { return _Fraction; }
      set { if (_Fraction != value) { double old = _Fraction; _Fraction = value; RaisePropertyChanged("Fraction", old, value); } }
    }
    private double _Fraction;

    public int Index {
      get { return _Index; }
      set { if (_Index != value) { int old = _Index; _Index = value; RaisePropertyChanged("Index", old, value); } }
    }
    private int _Index = Int32.MinValue;

    // Because we added properties that we want to persist,
    // we should override these two methods to write/read those properties.
    // Also, the MakeXElement override ensures the element is named "Instrument".
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement("Instrument");
      e.Add(XHelper.Attribute("Width", this.Width, 40.0));
      e.Add(XHelper.Attribute("Height", this.Height, 40.0));
      e.Add(XHelper.Attribute("Fraction", this.Fraction, 0.0));
      e.Add(XHelper.Attribute("Index", this.Index, Int32.MinValue));
      return e;
    }

    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Width = XHelper.Read("Width", e, 40.0);
      this.Height = XHelper.Read("Height", e, 40.0);
      this.Fraction = XHelper.Read("Fraction", e, 0.0);
      this.Index = XHelper.Read("Index", e, Int32.MinValue);
    }
  }


#if !SILVERLIGHT
  [Serializable]
#endif
  public class PipeData : GraphLinksModelLinkData<String, String> {
    // nothing to add or override, for now
  }


  // Add some more functionality when dragging.
  // Treat nodes like "Instrument" specially:
  //   - when dropped on a Link they become part of the Link's group of label nodes;
  //     each Link can have at most one Node associated with it -- to support multiple label nodes
  //     the Link's LabelNode is a Group, which can contain any number of Instrument nodes
  //   - when dropped anywhere, they remember their position relative to the closest link segment
  //     of the owning link (Pipe)
  // Treat nodes like "Valve" specially:
  //   - when dropped on a Link they are spliced in, if they are not already connected to a Pipe
  //   - when spliced into a Link, they are rotated to follow the direction of that link segment
  //   - rotation is more complicated than just setting Node.RotationAngle: we also
  //     have to change the relative position of the TextBlock, which should not be rotated
  //   - any Instruments attached to the old Pipe are re-attached to a new Pipe connecting to
  //     the Valve that was spliced into the old Pipe
  public class CustomDraggingTool : DraggingTool {
    protected override bool ConsiderDragOver(System.Windows.Point pt, Part p) {
      Link link = p as Link;
      if (link != null) {
        Dictionary<Part, Info> parts = this.CopiedParts ?? this.DraggedParts;
        if (parts == null) return false;
        if (parts.ContainsKey(p)) return false;
        // don't do any special behavior if the node isn't a Valve or an Instrument
        if (parts.Keys.OfType<Node>().Any(n => !Spliceable(n) && !Labelable(n))) return false;
      }
      return base.ConsiderDragOver(pt, p);
    }

    private bool Spliceable(Node n) {
      ItemData data = n.Data as ItemData;
      // if the Valve is already connected to any nodes, don't splice it into any Pipe
      return (data != null && data.Category == "Valve" && n.LinksConnected.Count() == 0);
    }

    private bool Labelable(Node n) {
      ItemData data = n.Data as ItemData;
      return (data != null && data.Category == "Instrument");
    }

    protected override void DropOnto(System.Windows.Point pt) {
      // handle dropped "Instrument"s by adding them to the Link's LabelNode (a Group)
      // and by remembering the position of the label relative to a segment of the Link.Route
      var newlabs = this.Diagram.SelectedParts.OfType<Node>().Where(n => Labelable(n)).ToList();
      if (newlabs.Count() > 0) {
        // see if the user is dropping onto a Link
        Link overlink = this.Diagram.Panel.FindElementAt<Link>(pt, Part.FindAncestor<Link>, p => ConsiderDragOver(pt, p), SearchLayers.Links);
        if (overlink != null) {
          PipeData linkdata = overlink.Data as PipeData;
          MyModel model = this.Diagram.Model as MyModel;
          if (linkdata != null && model != null) {
            // get link's existing label group
            ItemData labelgroup = model.GetLabelNodeForLink(linkdata);
            if (labelgroup == null) {  // create it if it doesn't have one already
              labelgroup = new InstrumentData() { Key="LG", IsSubGraph=true };
              model.AddNode(labelgroup);  // will provide unique Key for new group
              model.SetLinkLabelKey(linkdata, labelgroup.Key);
            }
            // add each dropped label to link's label group
            foreach (Node lab in newlabs) {
              ItemData labdata = lab.Data as ItemData;
              if (labdata != null) model.SetGroupNodeKey(labdata, labelgroup.Key);
            }
          }
        }
        // find closest segment for each label node and remember it and its offset
        foreach (Node lab in newlabs) {
          InstrumentData labdata = lab.Data as InstrumentData;
          if (labdata == null) continue;
          Group labcoll = lab.ContainingSubGraph;
          if (labcoll == null) continue;
          Link link = labcoll.LabeledLink;
          if (link != null) {
            Point labc = new Point(lab.Bounds.X + lab.Bounds.Width/2, lab.Bounds.Y + lab.Bounds.Height/2);
            var pts = link.Route.Points;
            if (pts.Count < 2) continue;
            int idx = link.Route.FindClosestSegment(labc);
            if (idx >= pts.Count-1) idx = pts.Count-2;
            labdata.Index = idx;
            Point a = pts[idx];
            Point b = pts[idx+1];
            if (Math.Abs(a.Y-b.Y) < 0.1) {
              if (a.X < b.X) {
                labdata.Fraction = (labc.X <= a.X ? 0 : (labc.X >= b.X ? 1 : (labc.X-a.X)/(b.X-a.X)));
                labdata.Location = new Point(0, labc.Y-a.Y);
              } else {
                labdata.Fraction = (labc.X >= a.X ? 0 : (labc.X <= b.X ? 1 : (labc.X-a.X)/(b.X-a.X)));
                labdata.Location = new Point(0, a.Y-labc.Y);
              }
            } else {
              if (a.Y < b.Y) {
                labdata.Fraction = (labc.Y <= a.Y ? 0 : (labc.Y >= b.Y ? 1 : (labc.Y-a.Y)/(b.Y-a.Y)));
                labdata.Location = new Point(0, a.X-labc.X);
              } else {
                labdata.Fraction = (labc.Y >= a.Y ? 0 : (labc.Y <= b.Y ? 1 : (labc.Y-a.Y)/(b.Y-a.Y)));
                labdata.Location = new Point(0, labc.X-a.X);
              }
            }
          }
        }
      } else {
        // rotate dropped "Valve"s
        newlabs = this.Diagram.SelectedParts.OfType<Node>().Where(n => Spliceable(n)).ToList();
        if (newlabs.Count() > 0) {
          // see if the user is dropping onto a Link
          Link overlink = this.Diagram.Panel.FindElementAt<Link>(pt, Part.FindAncestor<Link>, p => ConsiderDragOver(pt, p), SearchLayers.Links);
          MyModel model = this.Diagram.Model as MyModel;
          if (overlink != null && model != null) {
            // get the data representing a Pipe
            PipeData oldlinkdata = overlink.Data as PipeData;
            // get the Group holding Instruments for the Pipe, if any
            ItemData labelgroup = model.GetLabelNodeForLink(oldlinkdata);
            var pts = overlink.Route.Points;
            if (pts.Count >= 2) {
              // maybe rotate the Valve nodes
              foreach (Node lab in newlabs) {
                Point labc = new Point(lab.Bounds.X + lab.Bounds.Width/2, lab.Bounds.Y + lab.Bounds.Height/2);
                int idx = overlink.Route.FindClosestSegment(labc);
                if (idx >= pts.Count-1) idx = pts.Count-2;
                Point a = pts[idx];
                Point b = pts[idx+1];
                // if the valve were bi-directional, this needs to be smarter?
                int angle = 0;
                if (Math.Abs(a.Y-b.Y) < 0.1) {  // horizontal
                  angle = (a.X < b.X ? 0 : 180);
                } else {
                  angle = (a.Y < b.Y ? 90 : 270);
                }
                lab.RotationAngle = angle;
                // the StackPanel.Orientation governs the relative position of the TextBlock;
                // it is bound to the Node.RotationAngle using a ValveOrientationConverter
              }
            }
            base.DropOnto(pt);  // this will splice the Valve(s) into the Link
            if (oldlinkdata != null) {
              // find one of the new Pipes
              PipeData newlinkdata = model.GetFromLinksForNode(newlabs.First().Data as ItemData).First();
              // this is the label group holding all of the Instruments of the original Link
              if (labelgroup != null) {
                // attach the old group of Instruments to the new Pipe
                model.AddNode(labelgroup);
                model.SetLinkLabelKey(newlinkdata, labelgroup.Key);
              }
            }
          }
        } else {  // handle regular nodes
          base.DropOnto(pt);
        }
      }
    }
  }

  public class ValveOrientationConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      if (value is Double) {
        double angle = (double)value;
        if (45 <= angle && angle <= 135) return Orientation.Horizontal;
        if (225 <= angle && angle <= 315) return Orientation.Horizontal;
        return Orientation.Vertical;
      }
      return Orientation.Vertical;
    }
  }
}
