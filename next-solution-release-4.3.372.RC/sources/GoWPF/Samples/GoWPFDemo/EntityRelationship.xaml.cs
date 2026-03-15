/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace EntityRelationship {
  public partial class EntityRelationship : UserControl {
    public EntityRelationship() {
      InitializeComponent();

      // create and initialize the model for the Diagram
      var model = new GraphLinksModel<Entity, String, String, Relationship>();

      // this is the table information for part of a database schema
      model.NodesSource = new ObservableCollection<Entity>() {
        new Entity() { Key="Products",
                       Items=new Attribute[] {
                         new Attribute("ProductID", true, NodeFigure.Decision, "Yellow"),
                         new Attribute("ProductName", false, NodeFigure.Cube1, "LightBlue"),
                         new Attribute("SupplierID", false, NodeFigure.Decision, "Purple"),
                         new Attribute("CategoryID", false, NodeFigure.Decision, "Purple") } },
        new Entity() { Key="Suppliers",
                       Items=new Attribute[] {
                         new Attribute("SupplierID", true, NodeFigure.Decision, "Yellow"),
                         new Attribute("CompanyName", false, NodeFigure.Cube1, "LightBlue"),
                         new Attribute("ContactName", false, NodeFigure.Cube1, "LightBlue"),
                         new Attribute("Address", false, NodeFigure.Cube1, "LightBlue") } },
        new Entity() { Key="Categories",
                       Items=new Attribute[] {
                         new Attribute("CategoryID", true, NodeFigure.Decision, "Yellow"),
                         new Attribute("CategoryName", false, NodeFigure.Cube1, "LightBlue"),
                         new Attribute("Description", false, NodeFigure.Cube1, "LightBlue"),
                         new Attribute("Picture", false, NodeFigure.TriangleUp, "Red") } },
        new Entity() { Key="Order Details",
                       Items=new Attribute[] {
                         new Attribute("OrderID", true, NodeFigure.Decision, "Yellow"),
                         new Attribute("ProductID", true, NodeFigure.Decision, "Yellow"),
                         new Attribute("UnitPrice", false, NodeFigure.MagneticData, "Green"),
                         new Attribute("Quantity", false, NodeFigure.MagneticData, "Green"),
                         new Attribute("Discount", false, NodeFigure.MagneticData, "Green") } },
      };
      myDiagram.InitialCenteredNodeData = model.NodesSource.OfType<Entity>().First();

      // define the relationships between the tables
      // Text holds the string for the "From" end; ToText holds the string for the "To" end
      model.LinksSource = new ObservableCollection<Relationship>() {
        new Relationship() { From="Products", To="Suppliers", Text="0..N", ToText="1" },
        new Relationship() { From="Products", To="Categories", Text="0..N", ToText="1" },
        new Relationship() { From="Order Details", To="Products", Text="0..N", ToText="1" },
      };

      model.HasUndoManager = true;
      myDiagram.Model = model;

      // wait until the Diagram's Panel exists before establishing its event handler
      myDiagram.TemplateApplied += (s, e) => {
        myDiagram.Panel.ViewportBoundsChanged += Panel_ViewportBoundsChanged;
      };

      this.SimplifiesTemplatesOnSmallScale = true;

      printButton.Command = myDiagram.CommandHandler.PrintCommand;
      // In-place "PrintPreview" support
      new PreviewingPrintManager().Init(myDiagram);
    }


    // Synchronized selection support

    private void myListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      UpdateSelection(e.AddedItems.OfType<Entity>().FirstOrDefault());
    }

    private void myDiagram_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      UpdateSelection(e.AddedItems.OfType<Node>().Select(n => n.Data).FirstOrDefault());
    }

    private void UpdateSelection(Object data) {
      // protect against recursive selection changing behavior,
      // especially if there are more than two controls to keep in sync
      if (this.ChangingSelection) return;
      this.ChangingSelection = true;

      myListBox.SelectedItem = data;
      myListBox.ScrollIntoView(data);

      Node node = myDiagram.PartManager.FindNodeForData(data, myDiagram.Model);
      if (node != null) myDiagram.Select(node);

      this.ChangingSelection = false;
    }

    private bool ChangingSelection { get; set; }


    // Changing DataTemplates when the DiagramPanel.Scale changes

    private void SimplifyBox_Click(object sender, RoutedEventArgs e) {
      this.SimplifiesTemplatesOnSmallScale = !this.SimplifiesTemplatesOnSmallScale;
      if (!this.SimplifiesTemplatesOnSmallScale) {
        myDiagram.NodeTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "NodeTemplate");
        myDiagram.LinkTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "LinkTemplate");
      }
      Panel_ViewportBoundsChanged(myDiagram.Panel, null);
    }

    // enable or disable this behavior
    public bool SimplifiesTemplatesOnSmallScale { get; set; }

    // when the scale is small, replace the DataTemplates with simplified ones
    private void Panel_ViewportBoundsChanged(object sender, RoutedPropertyChangedEventArgs<Rect> e) {
      if (!this.SimplifiesTemplatesOnSmallScale) return;
      if (myDiagram.Panel.Scale < 0.4) {
        myDiagram.NodeTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "SimplifiedNodeTemplate");
        myDiagram.LinkTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "SimplifiedLinkTemplate");
      } else {
        myDiagram.NodeTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "NodeTemplate");
        myDiagram.LinkTemplate = Diagram.FindResource<DataTemplate>(myDiagram, "LinkTemplate");
      }
    }


    // bring up a mode-less form to edit the details of the node data
    private void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
      // find the Node that contains the Button
      Node node = Part.FindAncestor<Node>(e.OriginalSource as UIElement);
      if (node == null) return;
      // now we can get the data that the Node is bound to, an Entity
      Entity data = node.Data as Entity;
      // find the Node hosting the UserControl form
      Node form = myDiagram.PartsModel.FindNodeByKey("myDetails");
      if (form == null) return;
      // position the form at the top-right corner of this node
      form.Location = Spot.TopRight.PointInRect(node.Bounds);
      form.Visible = true;
      // support data-binding to the Entity from within the form
      if (form.VisualElement != null) form.VisualElement.DataContext = data;
    }
  }


  // Data classes

  // Each node is defined by an instance of the GraphModelNodeData class
  public class Entity : GraphLinksModelNodeData<String> {
    // If the node data is copied, we need to override this method to make sure the list of items is not shared
    public override object Clone() {
      Entity d = (Entity)base.Clone();
      List<Attribute> items = new List<Attribute>();
      foreach (Attribute e in this.Items) items.Add(new Attribute(e.Name, e.IsKey, e.Figure, e.Color));
      d.Items = items;
      return d;
    }

    public IList<Attribute> Items {
      get { return _Items; }
      set {
        if (_Items != value) {
          System.Collections.IEnumerable old = _Items;
          _Items = value;
          RaisePropertyChanged("Items", old, value);
        }
      }
    }
    private IList<Attribute> _Items = new List<Attribute>().AsReadOnly();
  }

  public class Attribute : INotifyPropertyChanged {
    public Attribute(String name, bool iskey, NodeFigure fig, String color) {
      this.Name = name;
      this.IsKey = iskey;
      this.Figure = fig;
      this.Color = color;
    }

    // Property setters need to raise the PropertyChanged event,
    // only when the value has changed, and passing both the previous and the new values

    public String Name {
      get { return _Name; }
      set {
        if (_Name != value) {
          String old = _Name;
          _Name = value;
          RaisePropertyChanged("Name", old, value);
        }
      }
    }
    private String _Name;

    public bool IsKey {
      get { return _IsKey; }
      set {
        if (_IsKey != value) {
          bool old = _IsKey;
          _IsKey = value;
          RaisePropertyChanged("IsKey", old, value);
        }
      }
    }
    private bool _IsKey;

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
    private NodeFigure _Figure = NodeFigure.DiskStorage;

    public String Color {
      get { return _Color; }
      set {
        if (_Color != value) {
          String old = _Color;
          _Color = value;
          RaisePropertyChanged("Color", old, value);
        }
      }
    }
    private String _Color;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(String pname, Object oldval, Object newval) {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, new ModelChangedEventArgs(pname, this, oldval, newval));
      }
    }
  }

  public class Relationship : GraphLinksModelLinkData<String, String> {
    // add another text property for the "to" end of each link
    public String ToText {
      get { return _ToText; }
      set {
        if (_ToText != value) {
          String old = _ToText;
          _ToText = value;
          RaisePropertyChanged("ToText", old, value);
        }
      }
    }
    private String _ToText;
  }


  // In-place PrintPreview support

  // The "Print Grid" is a GridPattern element in an unbound Node
  // that shows where the print page breaks will be.
  //  <go:Diagram ...>
  //    <go:Node Id="PrintGrid">
  //      <Grid go:Part.LayerName="Background" go:Part.Selectable="False"
  //            go:Part.LayoutId="None" go:Node.Avoidable="False"
  //            go:Part.InDiagramBounds="False" go:Part.Printable="False"
  //            go:Node.LocationElementName="GridPattern">
  //        <go:GridPattern x:Name="GridPattern">
  //          <Path Stroke="Purple" StrokeThickness="2" go:GridPattern.Figure="HorizontalLine" />
  //          <Path Stroke="Purple" StrokeThickness="2" go:GridPattern.Figure="VerticalLine" />
  //        </go:GridPattern>
  //      </Grid>
  //    </go:Node>
  //  </go:Diagram>

  // The Node must have the Id of "PrintGrid".
  // The GridPattern in that node must be the LocationElement of that Node.

  // When previewing, the PageOptions are assumed to include the "Full" page,
  // with or without the background or grid.

  // In WPF the PrintableArea defaults to the default printer's PrintableArea.
  // In Silverlight that information is not available without actually printing;
  // thus the PrintableArea property must be set before the "Print Grid" can be seen.

  public class PreviewingPrintManager : PrintManager {
    // Setup this Diagram.PrintManager to automatically update the "Print Grid".
    public void Init(Diagram diagram) {
      diagram.PrintManager = this;
      if (diagram.Panel != null) {
        InitDiagram();
      } else {
        diagram.TemplateApplied += (sd, ev) => {
          InitDiagram();
        };
      }
    }

    private void InitDiagram() {
      if (!this.Inited) {
        this.Inited = true;
#if SILVERLIGHT || XBAP
        this.PrintableArea = new Size(816, 1056);  //??? how to get default printer page information
#else
        // get the default printer's printable area
        var dlg = new PrintDialog();
        this.PrintableArea = new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);
#endif
        // automatically update the print grid's position and size
        this.Diagram.Panel.DiagramBoundsChanged += (s, e) => UpdatePrintGrid();
        UpdateAutoScales();
        UpdateIsPrintPreviewEnabled();
      }
    }
    private bool Inited { get; set; }

    // Gets or sets whether the DiagramPanel.Scale determines the PrintManager.Scale,
    // or whether the user can change the DiagramPanel.Scale using control-shift-mouse-wheel.
    public bool AutoScales {
      get { return _AutoScales; }
      set {
        if (_AutoScales != value) {
          _AutoScales = value;
          UpdateAutoScales();
        }
      }
    }
    private bool _AutoScales;

    private void UpdateAutoScales() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (diagram.Panel == null) return;
      if (this.AutoScales) {
        // support having the print scale be the same as the DiagramPanel.Scale
        this.Diagram.Panel.ViewportBoundsChanged += HandleViewportBoundsChanged;
        if (this.Diagram.DefaultTool is PrintPreviewToolManager) this.Diagram.DefaultTool = new ToolManager();
      } else {
        // support control-shift-mouse-wheel changing only the print scale
        this.Diagram.Panel.ViewportBoundsChanged -= HandleViewportBoundsChanged;
        if (!(this.Diagram.DefaultTool is PrintPreviewToolManager)) this.Diagram.DefaultTool = new PrintPreviewToolManager();
      }
    }

    // Gets or sets whether the print-preview grid is visible
    public bool IsPreviewEnabled {
      get { return _IsPreviewEnabled; }
      set {
        if (_IsPreviewEnabled != value) {
          _IsPreviewEnabled = value;
          UpdateIsPrintPreviewEnabled();
        }
      }
    }
    private bool _IsPreviewEnabled;

    private void UpdateIsPrintPreviewEnabled() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (diagram.Panel == null) return;
      Node printGrid = diagram.PartsModel.FindNodeByKey("PrintGrid");
      if (printGrid != null) {
        printGrid.Visible = this.IsPreviewEnabled;
        UpdatePrintGrid();
      }
    }

    // The size of the printed area, inside the margins, in model coordinates;
    // set this to the selected printer's PrintableArea.
    // This is initialized to be the default printer's PrintableArea in WPF.
    // In Silverlight you must set this property.
    // In WPF you should set this whenever the user chooses a different printer.
    public Size PrintableArea {
      get { return _PrintableArea; }
      set {
        if (_PrintableArea != value &&
            !Double.IsInfinity(value.Width) && !Double.IsNaN(value.Width) &&
            !Double.IsInfinity(value.Height) && !Double.IsNaN(value.Height)) {
          _PrintableArea = value;
          UpdatePrintGrid();
        }
      }
    }
    private Size _PrintableArea = new Size(0, 0);

    // If you change the PrintManager.Scale, call UpdatePrintGrid() afterwwards.

    private void HandleViewportBoundsChanged(object sender, EventArgs e) {
      if (!this.IsPreviewEnabled) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (diagram.Panel == null) return;
      this.Scale = diagram.Panel.Scale;
      UpdatePrintGrid();
    }

    // Update the position and size and CellSize of the GridPattern
    // in the Node whose Id is "PrintGrid".
    public void UpdatePrintGrid() {
      if (!this.IsPreviewEnabled) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (diagram.Panel == null) return;
      double pw = this.PrintableArea.Width;
      double ph = this.PrintableArea.Height;
      Thickness m = this.Margin;
      pw -= m.Left+m.Right;
      ph -= m.Top+m.Bottom;
      if (pw <= 0 || Double.IsNaN(pw) || ph <= 0 || Double.IsNaN(ph)) {
        return;  // can't update PrintGrid if we don't know printable area
      }
      Node printGrid = diagram.PartsModel.FindNodeByKey("PrintGrid");
      if (printGrid != null) {
        Rect db = diagram.Panel.DiagramBounds;
        double scale = this.Scale;
        if (Double.IsNaN(scale) || scale <= 0 || Double.IsInfinity(scale)) {
          scale = Math.Min(1, Math.Min(pw/db.Width, ph/db.Height));
        }
        Size eps = new Size(pw/scale, ph/scale);
        GridPattern gp = printGrid.FindDescendant(e => e is GridPattern) as GridPattern;
        if (gp != null) {
          // update the PrintGrid's position and size to cover the DiagramBounds
          printGrid.Location = new Point(db.X, db.Y);
          gp.Width = Math.Ceiling(Math.Ceiling(db.Width/eps.Width)*eps.Width);
          gp.Height = Math.Ceiling(Math.Ceiling(db.Height/eps.Height)*eps.Height);
          gp.Margin = this.Margin;
          // update the PrintGrid's GridPattern's CellSize
          gp.CellSize = eps;
        }
      }
    }
  }

  // control-shift-mouse-wheel changes PrintManager.Scale
  public class PrintPreviewToolManager : ToolManager {
    public override void DoMouseWheel() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      int delta = GetWheelDelta();
      if (delta == 0) return;
      bool shift = IsShiftKeyDown();
      bool control = IsControlKeyDown();
      var pm = diagram.PrintManager as PreviewingPrintManager;
      if (pm != null && pm.IsPreviewEnabled && shift && control) {
        double scale = pm.Scale;
        if (Double.IsNaN(scale) || scale <= 0 || Double.IsInfinity(scale))
          scale = 1;
        else if (delta > 0)
          scale *= 1.05;
        else
          scale /= 1.05;
        pm.Scale = scale;
        pm.UpdatePrintGrid();
        MouseWheelEventArgs e = diagram.LastMouseEventArgs as MouseWheelEventArgs;
        if (e != null) e.Handled = true;
      } else {
        base.DoMouseWheel();
      }
    }
  }
}
