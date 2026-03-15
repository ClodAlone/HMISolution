/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace SequentialFunction {
  public partial class SequentialFunction : UserControl {
    public SequentialFunction() {
      InitializeComponent();

      var model = new GraphLinksModel<NodeData, String, String, LinkData>();
      // initialize it from data in an XML file that is an embedded resource
      String xml = Demo.MainPage.Instance.LoadText("SequentialFunction", "xml");
      model.Load<NodeData, LinkData>(XElement.Parse(xml), "Node", "Link");
      model.Modifiable = true;
      model.HasUndoManager = true;
      myDiagram.Model = model;

      myDiagram.MouseDownTools.Insert(0, new LinkShiftingTool() { Diagram = myDiagram });
    }

    // save and load the model data as XML, visible in the "Saved" tab of the Demo
    private void Save_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<NodeData, String, String, LinkData>;
      if (model == null) return;
      XElement root = model.Save<NodeData, LinkData>("SequentialFunctionChart", "Node", "Link");
      Demo.MainPage.Instance.SavedXML = root.ToString();
      LoadButton.IsEnabled = true;
      model.IsModified = false;
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      var model = myDiagram.Model as GraphLinksModel<NodeData, String, String, LinkData>;
      if (model == null) return;
      try {
        XElement root = XElement.Parse(Demo.MainPage.Instance.SavedXML);
        model.Load<NodeData, LinkData>(root, "Node", "Link");
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      model.IsModified = false;
    }
  }

#if !SILVERLIGHT
  [Serializable]
#endif
  public class NodeData : GraphLinksModelNodeData<String> {
    public double Width {
      get { return _Width; }
      set {
        if (_Width != value) {
          double old = _Width;
          _Width = value;
          RaisePropertyChanged("Width", old, value);
        }
      }
    }
    private double _Width = 60.0;

    // write the extra property
    public override XElement MakeXElement(XName n) {
      XElement e = base.MakeXElement(n);
      e.Add(XHelper.Attribute("Width", this.Width, 60.0));
      return e;
    }

    // read the extra property
    public override void LoadFromXElement(XElement e) {
      base.LoadFromXElement(e);
      this.Width = XHelper.Read("Width", e, 60.0);
    }
  }

#if !SILVERLIGHT
  [Serializable]
#endif
  public class LinkData : GraphLinksModelLinkData<String, String> {
  }


  /// <summary>
  /// This tool lets the user interactively position the end of a link within the port that it is connected to.
  /// </summary>
  public class LinkShiftingTool : ReshapingBaseTool {

    private const String ToolCategory = "ShiftLink";

    /// <summary>
    /// Gets the current <see cref="Link"/> that the <see cref="ReshapingBaseTool.AdornedElement"/> is in.
    /// </summary>
    protected Link AdornedLink {
      get { return Part.FindAncestor<Link>(this.AdornedElement); }
    }

    /// <summary>
    /// Gets or sets a copy of the <see cref="AdornedLink"/>'s route's initial array of Points.
    /// </summary>
    /// <value>
    /// This is null until it is set by <see cref="DoActivate"/>.
    /// </value>
    protected IList<Point> OriginalPoints { get; set; }


    /// <summary>
    /// Show an <see cref="Adornment"/> with <see cref="ToolHandle"/>s at each end of the link's route,
    /// if the link is selected and visible.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// </remarks>
    public override void UpdateAdornments(Part part) {
      Link link = part as Link;
      if (link == null) return;  // no Nodes

      Adornment adornment = null;
      if (link.IsSelected) {
        FrameworkElement selelt = link.Path;
        if (selelt != null && Part.IsVisibleElement(selelt)) {
          adornment = link.GetAdornment(ToolCategory);
          if (adornment == null) {
            Route route = link.Route;
            if (route != null) {
              IList<Point> pts = route.Points;
              int numpts = route.PointsCount;
              if (pts != null && numpts > 2) {
                // This tool's Adornments consists of a LinkPanel containing two ToolHandles.
                LinkPanel panel = new LinkPanel();
#if SILVERLIGHT
                var h = new System.Windows.Shapes.Path();
#else
                var h = new ToolHandle();
#endif
                // of course you can customize the appearance of the shift handles here...
                NodePanel.SetFigure(h, NodeFigure.Rectangle);
                h.Width = 6;
                h.Height = 6;
                h.Fill = new SolidColorBrush(Colors.Orange);
                LinkPanel.SetIndex(h, 0);
                LinkPanel.SetFraction(h, 0.5);
                if (pts[0].X == pts[1].X && pts[0].Y != pts[1].Y) {
                  SetReshapeBehavior(h, ReshapeBehavior.Horizontal);
                  h.Cursor = System.Windows.Input.Cursors.SizeWE;
                } else if (pts[0].Y == pts[1].Y && pts[0].X != pts[1].X) {
                  SetReshapeBehavior(h, ReshapeBehavior.Vertical);
                  h.Cursor = System.Windows.Input.Cursors.SizeNS;
                } else {
                  SetReshapeBehavior(h, ReshapeBehavior.All);
#if !SILVERLIGHT
                  h.Cursor = System.Windows.Input.Cursors.SizeAll;
#endif
                }
                panel.Children.Add(h);

#if SILVERLIGHT
                h = new System.Windows.Shapes.Path();
#else
                h = new ToolHandle();
#endif
                NodePanel.SetFigure(h, NodeFigure.Rectangle);
                h.Width = 6;
                h.Height = 6;
                h.Fill = new SolidColorBrush(Colors.Orange);
                LinkPanel.SetIndex(h, -1);
                LinkPanel.SetFraction(h, 0.5);
                SetReshapeBehavior(h, ReshapeBehavior.All);
                if (pts[numpts-1].X == pts[numpts-2].X && pts[numpts-1].Y != pts[numpts-2].Y) {
                  SetReshapeBehavior(h, ReshapeBehavior.Horizontal);
                  h.Cursor = System.Windows.Input.Cursors.SizeWE;
                } else if (pts[numpts-1].Y == pts[numpts-2].Y && pts[numpts-1].X != pts[numpts-2].X) {
                  SetReshapeBehavior(h, ReshapeBehavior.Vertical);
                  h.Cursor = System.Windows.Input.Cursors.SizeNS;
                } else {
                  SetReshapeBehavior(h, ReshapeBehavior.All);
#if !SILVERLIGHT
                  h.Cursor = System.Windows.Input.Cursors.SizeAll;
#endif
                }
                panel.Children.Add(h);

                adornment = new Adornment();
                adornment.AdornedElement = selelt;
                adornment.Category = ToolCategory;
                adornment.Content = panel;  // just provide the FrameworkElement as the Content and as the Visual Child
                adornment.LocationSpot = Spot.TopLeft;
              }
            }
          }
          if (adornment != null) {
            Point loc = link.GetElementPoint(selelt, Spot.TopLeft);
            adornment.Location = loc;
            adornment.RotationAngle = link.GetAngle(selelt);
            adornment.Remeasure();
          }
        }
      }
      link.SetAdornment(ToolCategory, adornment);
    }

    /// <summary>
    /// The <see cref="LinkReshapingTool"/> may run when there is a mouse-down event on a reshape handle.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// For this tool to be runnable, the diagram must be modifiable
    /// (not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>),
    /// <see cref="DiagramTool.IsLeftButtonDown"/> must be true,
    /// and <see cref="DiagramTool.FindToolHandleAt"/> must return a reshape tool handle.
    /// </remarks>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return false;
      if (!IsLeftButtonDown()) return false;
      FrameworkElement h = FindToolHandleAt(diagram.FirstMousePointInModel, ToolCategory);
      return h != null;
    }

    /// <summary>
    /// Capture the mouse when starting this tool.
    /// </summary>
    public override void DoStart() {
      CaptureMouse();
    }

    /// <summary>
    /// Start reshaping.
    /// </summary>
    /// <remarks>
    /// This sets the <see cref="ReshapingBaseTool.Handle"/> to the result of <see cref="DiagramTool.FindToolHandleAt"/>;
    /// if it is null, this method does not actually activate this tool.
    /// If there is a reshape handle,
    /// this sets <see cref="ReshapingBaseTool.AdornedElement"/>,
    /// remembers the <see cref="OriginalPoints"/>,
    /// starts a transaction (<see cref="DiagramTool.StartTransaction"/>),
    /// and sets <see cref="DiagramTool.Active"/> to true.
    /// </remarks>
    public override void DoActivate() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      this.Handle = FindToolHandleAt(diagram.FirstMousePointInModel, ToolCategory);
      if (this.Handle == null) return;

      this.AdornedElement = FindAdornedElement(this.Handle);
      Link link = this.AdornedLink;
      this.OriginalPoints = link.Route.Points.ToList();

      StartTransaction(ToolCategory);
      this.Active = true;
    }

    /// <summary>
    /// Commit or rollback the transaction, and cleanup.
    /// </summary>
    public override void DoDeactivate() {
      this.Active = false;
      StopTransaction();

      this.Handle = null;
      this.AdornedElement = null;
      this.OriginalPoints = null;
    }

    /// <summary>
    /// Just stop the mouse capture.
    /// </summary>
    public override void DoStop() {
      ReleaseMouse();
    }

    /// <summary>
    /// Restore the modified route to be the <see cref="OriginalPoints"/>
    /// and stop this tool.
    /// </summary>
    public override void DoCancel() {
      SetPoints(this.OriginalPoints);
      StopTool();
    }

    /// <summary>
    /// Call <see cref="DoReshape"/> with a new point determined by the mouse.
    /// </summary>
    public override void DoMouseMove() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        DoReshape(diagram.LastMousePointInModel);
      }
    }

    /// <summary>
    /// Call <see cref="DoReshape"/> with the most recent mouse point,
    /// save the route, and then stop this tool.
    /// </summary>
    public override void DoMouseUp() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        DoReshape(diagram.LastMousePointInModel);
        SetPoints(this.AdornedLink.Route.Points.ToList());
        this.TransactionResult = ToolCategory;  // success
      }
      StopTool();
    }

    private void SetPoints(IList<Point> points) {
      Link link = this.AdornedLink;
      if (link != null) {
        Route route = link.Route;
        if (route != null && points != null) {
          route.Points = points;
        }
      }
    }

    private void DoReshape(Point newPoint) {
      switch (GetReshapeBehavior(this.Handle)) {
        case ReshapeBehavior.Horizontal:
          newPoint.X = Math.Floor(newPoint.X/4+0.5)*4;
          break;
        case ReshapeBehavior.Vertical:
          newPoint.Y = Math.Floor(newPoint.Y/4+0.5)*4;
          break;
        default:
          break;
      }
      Link link = this.AdornedLink;
      int idx = LinkPanel.GetIndex(this.Handle);
      Rect portb;
      if (idx == 0) {
        portb = link.FromNode.GetElementBounds(link.FromPort);
      } else {
        portb = link.ToNode.GetElementBounds(link.ToPort);
      }
      if (portb.Width < 1 || portb.Height < 1) return;
      Point ctr = Spot.Center.PointInRect(portb);
      double x = (newPoint.X-portb.Left)/portb.Width;
      double y = (newPoint.Y-portb.Top)/portb.Height;
      x = Math.Max(0, Math.Min(x, 1));
      y = Math.Max(0, Math.Min(y, 1));
      switch (GetReshapeBehavior(this.Handle)) {
        case ReshapeBehavior.Horizontal:
          y = newPoint.Y < ctr.Y ? 0 : 1;
          break;
        case ReshapeBehavior.Vertical:
          x = newPoint.X < ctr.X ? 0 : 1;
          break;
        default:
          break;
      }
      Route route = link.Route;
      if (idx == 0) {
        route.FromSpot = new Spot(x, y);
      } else {
        route.ToSpot = new Spot(x, y);
      }
    }
  }
}
