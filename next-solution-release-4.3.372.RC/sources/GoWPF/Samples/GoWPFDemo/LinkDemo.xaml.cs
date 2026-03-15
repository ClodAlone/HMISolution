/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace LinkDemo {

  public partial class LinkDemo : UserControl {
    public LinkDemo() {
      InitializeComponent();
      InitFigures();
      InitArrowheads();

      // create the Model; MyNodeData and MyLinkData are defined below
      var model = new GraphLinksModel<MyNodeData, string, string, MyLinkData>();
      model.NodeKeyPath = "Key";
      model.NodesSource = new ObservableCollection<MyNodeData>() {
        new MyNodeData() { Key="A", Location=new Point(340,200), Color="Magenta", Figure=NodeFigure.Actor },
        new MyNodeData() { Key="B", Location=new Point(100,25), Color="Orange", Figure=NodeFigure.Crescent },
        new MyNodeData() { Key="C", Location=new Point(150,200), Color="LightGray", Figure=NodeFigure.DiskStorage,
          ToSpot=Spot.AllSides, FromSpot=Spot.AllSides },
        new MyNodeData() { Key="D", Location=new Point(400,25), Color="Cyan", Figure=NodeFigure.Email,
          ToSpot=Spot.AllSides },
        new MyNodeData() { Key="E", Location=new Point(-50,0), Color="Brown", Figure=NodeFigure.NorGate },
      };
      model.LinksSource = new ObservableCollection<MyLinkData>() {
        new MyLinkData() {
          From="A", To="C", Color="Red", Thickness=1,
          Routing=LinkRouting.Normal, Curve=LinkCurve.None, Curviness=0,
          ToArrow = Arrowhead.Block, FromArrow = Arrowhead.DoubleFeathers },
        new MyLinkData() {
          From="D", To="A", Color="Blue", Thickness=2,
          Routing=LinkRouting.Normal, Curve=LinkCurve.Bezier, Curviness=100,
          ToArrow = Arrowhead.Circle, FromArrow = Arrowhead.Chevron, ToArrowScale = 1.414, FromArrowScale = 1.414 },
        new MyLinkData() {
          From="B", To="C", Color="Black", Thickness=3,
          Routing=LinkRouting.Orthogonal, Curve=LinkCurve.None, Curviness=0,
          ToArrow = Arrowhead.CircleEndedArrow, FromArrow = Arrowhead.Kite, ToArrowScale = 1.732, FromArrowScale = 1.732 },
        new MyLinkData() {
          From="B", To="D", Color="Gray", Thickness=1,
          Routing=LinkRouting.Orthogonal, Curve=LinkCurve.None, Corner=10,
          ToArrow = Arrowhead.NormalArrow, FromArrow = Arrowhead.OpenTriangleTop },
        new MyLinkData() {
          From="C", To="D", Color="Purple", Thickness=2,
          Routing=LinkRouting.AvoidsNodes, Curve=LinkCurve.JumpOver, Corner=20,
          ToArrow = Arrowhead.PentagonArrow, FromArrow = Arrowhead.PlusCircle, ToArrowScale = 1.414, FromArrowScale = 1.414 },
        new MyLinkData() {
          From="E", To="B", Color="Green", Thickness=3,
          Routing=LinkRouting.AvoidsNodes, Curve=LinkCurve.JumpOver, Corner=20,
          ToArrow = Arrowhead.None, FromArrow = Arrowhead.Standard, ToArrowScale = 1.732, FromArrowScale = 1.732 }
      };
      myDiagram.Model = model;

      //Adds an event handler which is called when the SelectedParts property of myDiagram changes.
      myDiagram.SelectedParts.CollectionChanged += SelectedPartsChanged;
    }


    private List<NodeFigure> AllFigures { get; set; }  // a list of all NodeFigures

    // Adds all of the values from the NodeFigure enum to Figs, a List<NodeFigure>.  
    private void InitFigures() {
      List<NodeFigure> figs = new List<NodeFigure>();
#if SILVERLIGHT  // Silverlight does not support Enum.GetValues
      // so we search for consecutive values starting from zero
      int i = 0;
      while (Enum.IsDefined(typeof(NodeFigure), i)) {
        figs.Add((NodeFigure)i);
        i++;
      }
#else
      foreach (NodeFigure f in Enum.GetValues(typeof(NodeFigure))) figs.Add(f);
#endif
      // sort all of the figures alphabetically, not by enum value
      figs.Sort(new Comparison<NodeFigure>((x, y) => x.ToString().CompareTo(y.ToString())));
      this.AllFigures = figs;
      FigureBox.ItemsSource = figs;
    }

    private List<Arrowhead> AllArrowheads { get; set; }   // a list of all Arrowheads

    // Adds all of the values from the Arrowhead enum to arrows, a List<Arrowhead>.
    // Than sets ToArrowBox.ItemsSource to arrows and AllArrowheads to arrows.
    private void InitArrowheads() {
      List<Arrowhead> arrows = new List<Arrowhead>();
#if SILVERLIGHT  // Silverlight does not support Enum.GetValues
      // so we search for consecutive values starting from zero
      int i = 0;
      while (Enum.IsDefined(typeof(Arrowhead), i)) {
        arrows.Add((Arrowhead)i);
        i++;
      }
#else
      foreach (Arrowhead arr in Enum.GetValues(typeof(Arrowhead))) arrows.Add(arr);
#endif
      // sort all of the Arrowheads alphabetically, not by enum value
      arrows.Sort(new Comparison<Arrowhead>((x, y) => x.ToString().CompareTo(y.ToString())));
      this.AllArrowheads = arrows;
      ToArrowBox.ItemsSource = arrows;
      FromArrowBox.ItemsSource = arrows;
    }


    // Updates all controls when the primary selected part, SelectedPart, changes
    private void SelectedPartsChanged(object sender, EventArgs e) {
      if (myDiagram.SelectedPart == null) return;

      MyNodeData mnd = myDiagram.SelectedPart.Data as MyNodeData;
      if (mnd != null) {  // if SelectedPart is a Node
        FigureBox.SelectedItem = mnd.Figure;

        this.IgnoreCheckBoxChanges = true;
        Spot from = mnd.FromSpot;
        if (from.IsSide) {
          FromLeftCB.IsChecked = from.IncludesSide(Spot.LeftSide);
          FromTopCB.IsChecked = from.IncludesSide(Spot.TopSide);
          FromRightCB.IsChecked = from.IncludesSide(Spot.RightSide);
          FromBottomCB.IsChecked = from.IncludesSide(Spot.BottomSide);
          //Deselect all FromSpot RadioButtons
          FromNone.IsChecked = false;
        } else {  // FromSpot is a specific Spot or Spot.None
          // Sets the IsChecked Property of the corresponding RadioButton
          SetRadioButton("From", from);
          FromLeftCB.IsChecked = false;
          FromTopCB.IsChecked = false;
          FromRightCB.IsChecked = false;
          FromBottomCB.IsChecked = false;
        }

        Spot to = mnd.ToSpot;
        if (to.IsSide) {
          ToLeftCB.IsChecked = to.IncludesSide(Spot.LeftSide);
          ToTopCB.IsChecked = to.IncludesSide(Spot.TopSide);
          ToRightCB.IsChecked = to.IncludesSide(Spot.RightSide);
          ToBottomCB.IsChecked = to.IncludesSide(Spot.BottomSide);
          //Deselect all ToSpot RadioButtons
          ToNone.IsChecked = false;
        } else {  // ToSpot is a specific Spot or Spot.None
          // Sets the IsChecked Property of the corresponding RadioButton
          SetRadioButton("To", to);
          ToLeftCB.IsChecked = false;
          ToTopCB.IsChecked = false;
          ToRightCB.IsChecked = false;
          ToBottomCB.IsChecked = false;
        }
        this.IgnoreCheckBoxChanges = false;
      }

      MyLinkData mld = myDiagram.SelectedPart.Data as MyLinkData;
      if (mld != null) {  // or SelectedPart is a Link
        CurvinessSlider.Value = mld.Curviness;
        CornerSlider.Value = mld.Corner;
        ToArrowBox.SelectedItem = mld.ToArrow;
        FromArrowBox.SelectedItem = mld.FromArrow;
        ToArrowScaleBox.Text = mld.ToArrowScale.ToString();
        FromArrowScaleBox.Text = mld.FromArrowScale.ToString();
        ToEndSegmentLengthBox.Text = mld.ToEndSegmentLength.ToString();
        FromEndSegmentLengthBox.Text = mld.FromEndSegmentLength.ToString();
        ToShortLengthBox.Text = mld.ToShortLength.ToString();
        FromShortLengthBox.Text = mld.FromShortLength.ToString();

        switch (mld.Routing) {
          case LinkRouting.Normal: NormalRoutingRB.IsChecked = true; break;
          case LinkRouting.Orthogonal: OrthogonalRoutingRB.IsChecked = true; break;
          case LinkRouting.AvoidsNodes: AvoidsNodesRoutingRB.IsChecked = true; break;
        }

        switch (mld.Curve) {
          case LinkCurve.None: NoneCurveRB.IsChecked = true; break;
          case LinkCurve.JumpOver: JumpOverCurveRB.IsChecked = true; break;
          case LinkCurve.JumpGap: JumpGapCurveRB.IsChecked = true; break;
          case LinkCurve.Bezier: BezierCurveRB.IsChecked = true; break;
        }
      }
    }

    // Check the appropriate RadioButton for the Spot.
    // This depends on the naming convention of the RadioButtons.
    private void SetRadioButton(string prefix, Spot s) {
      if (s == Spot.TopLeft) prefix += "TopLeft";
      else if (s == Spot.TopCenter) prefix += "TopCenter";
      else if (s == Spot.TopRight) prefix += "TopRight";
      else if (s == Spot.MiddleLeft) prefix += "MiddleLeft";
      else if (s == Spot.Center) prefix += "Center";
      else if (s == Spot.MiddleRight) prefix += "MiddleRight";
      else if (s == Spot.BottomLeft) prefix += "BottomLeft";
      else if (s == Spot.BottomCenter) prefix += "BottomCenter";
      else if (s == Spot.BottomRight) prefix += "BottomRight";
      else prefix += "None";
      RadioButton rb = this.FindName(prefix) as RadioButton;
      if (rb != null) rb.IsChecked = true;
    }

    private bool IgnoreCheckBoxChanges { get; set; }

    // Handles the changes made in the CheckBoxes in the TabControl for ports. 
    private void CheckBox_Changed(object sender, EventArgs e) {
      if (this.IgnoreCheckBoxChanges) return;
      if (myDiagram.SelectedNode == null) return;
      MyNodeData mnd = myDiagram.SelectedNode.Data as MyNodeData;
      if (mnd != null) {
        myDiagram.StartTransaction("Modify Port Spots");
        if (ToFromSpotTabControl.SelectedItem == FromPortTab) {
          FromNone.IsChecked = true;
          mnd.FromSpot = Spot.GetSide((bool)FromLeftCB.IsChecked, (bool)FromTopCB.IsChecked,
                                      (bool)FromRightCB.IsChecked, (bool)FromBottomCB.IsChecked);
          if (mnd.FromSpot != Spot.None) FromNone.IsChecked = false;
        }
        if (ToFromSpotTabControl.SelectedItem == ToPortTab) {
          ToNone.IsChecked = true;
          mnd.ToSpot = Spot.GetSide((bool)ToLeftCB.IsChecked, (bool)ToTopCB.IsChecked,
                                    (bool)ToRightCB.IsChecked, (bool)ToBottomCB.IsChecked);
          if (mnd.ToSpot != Spot.None) ToNone.IsChecked = false;
        }
        myDiagram.CommitTransaction("Modify Port Spots");
      }
    }

    // Handles all RadioButton clicks for FromSpot and ToSpot.
    private void NodeRadioButton_Click(object sender, EventArgs e) {
      if (myDiagram.SelectedNode == null) return;
      RadioButton tempRB = sender as RadioButton;
      if (tempRB == null) return;
      MyNodeData mnd = myDiagram.SelectedNode.Data as MyNodeData;
      if (mnd != null) {
        myDiagram.StartTransaction("Modify Port Spots");
        // "From" Side CheckBoxes are all unchecked when a "From" RadioButton is selected.
        if (ToFromSpotTabControl.SelectedItem == FromPortTab) {
          this.IgnoreCheckBoxChanges = true;
          FromLeftCB.IsChecked = false;
          FromTopCB.IsChecked = false;
          FromRightCB.IsChecked = false;
          FromBottomCB.IsChecked = false;
          this.IgnoreCheckBoxChanges = false;
          // The names of FromPort RadioButtons are formatted in the pattern ("From"+NameOfSpot).
          // Spot.Parse() converts the button name to a Spot value.
          mnd.FromSpot = Spot.Parse(tempRB.Name.Substring(4));
        }
        // "To" Side CheckBoxes are all unchecked when a "To" RadioButton is selected.
        if (ToFromSpotTabControl.SelectedItem == ToPortTab) {
          this.IgnoreCheckBoxChanges = true;
          ToLeftCB.IsChecked = false;
          ToTopCB.IsChecked = false;
          ToRightCB.IsChecked = false;
          ToBottomCB.IsChecked = false;
          this.IgnoreCheckBoxChanges = false;
          // The names of ToPort RadioButtons are formatted in the pattern ("To"+NameOfSpot).
          // Spot.Parse() converts the button name to a Spot value.
          mnd.ToSpot = Spot.Parse(tempRB.Name.Substring(2));
        }
        myDiagram.CommitTransaction("Modify Port Spots");
      }
    }

    // Handles all RadioButton clicks for Routing and Curve.
    private void LinkRadioButton_Click(object sender, EventArgs e) {
      if (myDiagram.SelectedLink == null) return;
      RadioButton rb = (RadioButton)sender as RadioButton;
      if (rb == null) return;
      MyLinkData mld = myDiagram.SelectedLink.Data as MyLinkData;
      if (mld != null) {
        myDiagram.StartTransaction("Modify Routing and Curve");
        // Check the RadioButtons that specify LinkRouting.
        if (rb == NormalRoutingRB) mld.Routing = LinkRouting.Normal;
        if (rb == OrthogonalRoutingRB) mld.Routing = LinkRouting.Orthogonal;
        if (rb == AvoidsNodesRoutingRB) mld.Routing = LinkRouting.AvoidsNodes;
        // Check the RadioButtons that specify LinkCurve.         
        if (rb == NoneCurveRB) mld.Curve = LinkCurve.None;
        if (rb == JumpOverCurveRB) mld.Curve = LinkCurve.JumpOver;
        if (rb == JumpGapCurveRB) mld.Curve = LinkCurve.JumpGap;
        if (rb == BezierCurveRB) mld.Curve = LinkCurve.Bezier;
        myDiagram.CommitTransaction("Modify Routing and Curve");
      }
    }

    // Handles any change in (To/)FromArrowBox by changing SelectedLink.Data.(To/)FromArrow
    private void ArrowheadChanged(object sender, EventArgs e) {
      if (myDiagram.SelectedLink == null) return;
      MyLinkData mld = myDiagram.SelectedLink.Data as MyLinkData;
      if (mld != null) {
        myDiagram.StartTransaction("Modify Arrow");
        if (sender == ToArrowBox) {
          Arrowhead arrow = (Arrowhead)ToArrowBox.SelectedItem;
          mld.ToArrow = arrow;
        } else if (sender == FromArrowBox) {
          Arrowhead arrow = (Arrowhead)FromArrowBox.SelectedItem;
          mld.FromArrow = arrow;
        }
        myDiagram.CommitTransaction("Modify Arrow");
      }
    }

    // Handles any change in FigureBox by changing SelectedNode.Data.NodeFigure
    private void FigureChanged(object sender, EventArgs e) {
      if (myDiagram.SelectedNode == null) return;
      MyNodeData mnd = myDiagram.SelectedNode.Data as MyNodeData;
      if (mnd != null) {
        myDiagram.StartTransaction("Modify Node Figure");
        mnd.Figure = (NodeFigure)FigureBox.SelectedItem;
        myDiagram.CommitTransaction("Modify Node Figure");
      }
    }

    // Handles changes in all of the Link control Sliders
    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
      if (sender == CurvinessSlider && CurvinessTextBox != null)
        CurvinessTextBox.Text = Math.Round(CurvinessSlider.Value).ToString();
      else if (sender == CornerSlider && CornerTextBox != null)
        CornerTextBox.Text = Math.Round(CornerSlider.Value).ToString();
      else if (sender  == SmoothnessSlider && SmoothnessTextBox != null)
        SmoothnessTextBox.Text = Math.Round(SmoothnessSlider.Value, 2).ToString();
      else if (sender == FromArrowScaleSlider && FromArrowScaleBox != null)
        FromArrowScaleBox.Text = Math.Round(FromArrowScaleSlider.Value, 2).ToString();
      else if (sender == ToArrowScaleSlider && ToArrowScaleBox != null)
        ToArrowScaleBox.Text = Math.Round(ToArrowScaleSlider.Value, 2).ToString();
      else if (sender == FromEndSegmentLengthSlider && FromEndSegmentLengthBox != null)
        FromEndSegmentLengthBox.Text = Math.Round(FromEndSegmentLengthSlider.Value, 1).ToString();
      else if (sender == ToEndSegmentLengthSlider && ToEndSegmentLengthBox != null)
        ToEndSegmentLengthBox.Text = Math.Round(ToEndSegmentLengthSlider.Value, 1).ToString();
      else if (sender == FromShortLengthSlider && FromShortLengthBox != null)
        FromShortLengthBox.Text = Math.Round(FromShortLengthSlider.Value, 1).ToString();
      else if (sender == ToShortLengthSlider && ToShortLengthBox != null)
        ToShortLengthBox.Text = Math.Round(ToShortLengthSlider.Value, 1).ToString();
    }

    // Handles changes in all of the TextBoxes associated with the Link control Sliders
    private void SliderText_TextChanged(object sender, TextChangedEventArgs e) {
      if (myDiagram.SelectedLink == null) return;
      MyLinkData mld = myDiagram.SelectedLink.Data as MyLinkData;
      if (mld == null) return;
      myDiagram.StartTransaction("Modify Link Route");
      if (sender == CurvinessTextBox && CurvinessSlider != null) {
        try {
          double curv = double.Parse(CurvinessTextBox.Text);
          if (curv > CurvinessSlider.Maximum) curv = CurvinessSlider.Maximum;
          if (curv < CurvinessSlider.Minimum) curv = CurvinessSlider.Minimum;
          mld.Curviness = curv;
          CurvinessSlider.Value = curv;
        } catch (FormatException) {
          mld.Curviness = CurvinessSlider.Minimum;
          CurvinessSlider.Value = CornerSlider.Minimum;
        }
      } else if (sender == CornerTextBox && CornerSlider != null) {
        try {
          double corner = double.Parse(CornerTextBox.Text);
          if (corner > CornerSlider.Maximum) corner = CornerSlider.Maximum;
          if (corner < CornerSlider.Minimum) corner = CornerSlider.Minimum;
          mld.Corner = corner;
          CornerSlider.Value = corner;
        } catch (FormatException) {
          mld.Corner = CornerSlider.Minimum;
          CornerSlider.Value = CornerSlider.Minimum;
        }
      } else if (sender == SmoothnessTextBox && SmoothnessSlider != null) {
        try {
          double smoothness = double.Parse(SmoothnessTextBox.Text);
          if (smoothness > SmoothnessSlider.Maximum) smoothness = SmoothnessSlider.Maximum;
          if (smoothness < SmoothnessSlider.Minimum) smoothness = SmoothnessSlider.Minimum;
          mld.Smoothness = smoothness;
          SmoothnessSlider.Value = smoothness;
        } catch (FormatException) {
          mld.Smoothness = SmoothnessSlider.Minimum;
          SmoothnessSlider.Value = SmoothnessSlider.Minimum;
        }
      } else if (sender == FromArrowScaleBox && FromArrowScaleSlider != null) {
        try {
          double arrowScale = double.Parse(FromArrowScaleBox.Text);
          if (arrowScale > FromArrowScaleSlider.Maximum) arrowScale = FromArrowScaleSlider.Maximum;
          if (arrowScale < FromArrowScaleSlider.Minimum) arrowScale = FromArrowScaleSlider.Minimum;
          mld.FromArrowScale = arrowScale;
          FromArrowScaleSlider.Value = arrowScale;
        } catch (FormatException) {
          mld.FromArrowScale = FromArrowScaleSlider.Minimum;
          FromArrowScaleSlider.Value = FromArrowScaleSlider.Value;
        }
      } else if (sender == ToArrowScaleBox && ToArrowScaleSlider != null) {
        try {
          double arrowScale = double.Parse(ToArrowScaleBox.Text);
          if (arrowScale > ToArrowScaleSlider.Maximum) arrowScale = ToArrowScaleSlider.Maximum;
          if (arrowScale < ToArrowScaleSlider.Minimum) arrowScale = ToArrowScaleSlider.Minimum;
          mld.ToArrowScale = arrowScale;
          ToArrowScaleSlider.Value = arrowScale;
        } catch (FormatException) {
          mld.ToArrowScale = ToArrowScaleSlider.Minimum;
          ToArrowScaleSlider.Value = ToArrowScaleSlider.Value;
        }
      } else if (sender == FromEndSegmentLengthBox && FromEndSegmentLengthSlider != null) {
        try {
          double endSegmentLength = double.Parse(FromEndSegmentLengthBox.Text);
          if (endSegmentLength > FromEndSegmentLengthSlider.Maximum) endSegmentLength = FromEndSegmentLengthSlider.Maximum;
          if (endSegmentLength < FromEndSegmentLengthSlider.Minimum) endSegmentLength = FromEndSegmentLengthSlider.Minimum;
          mld.FromEndSegmentLength = endSegmentLength;
          FromEndSegmentLengthSlider.Value = endSegmentLength;
        } catch (FormatException) {
          mld.FromEndSegmentLength = FromEndSegmentLengthSlider.Minimum;
          FromEndSegmentLengthSlider.Value = FromEndSegmentLengthSlider.Minimum;
        }
      } else if (sender == ToEndSegmentLengthBox && ToEndSegmentLengthSlider != null) {
        try {
          double endSegmentLength = double.Parse(ToEndSegmentLengthBox.Text);
          if (endSegmentLength > ToEndSegmentLengthSlider.Maximum) endSegmentLength = ToEndSegmentLengthSlider.Maximum;
          if (endSegmentLength < ToEndSegmentLengthSlider.Minimum) endSegmentLength = ToEndSegmentLengthSlider.Minimum;
          mld.ToEndSegmentLength = endSegmentLength;
          ToEndSegmentLengthSlider.Value = endSegmentLength;
        } catch (FormatException) {
          mld.ToEndSegmentLength = ToEndSegmentLengthSlider.Minimum;
          ToEndSegmentLengthSlider.Value = ToEndSegmentLengthSlider.Minimum;
        }
      } else if (sender == FromShortLengthBox && FromShortLengthSlider != null) {
        try {
          double shortLength = double.Parse(FromShortLengthBox.Text);
          if (shortLength > FromShortLengthSlider.Maximum) shortLength = FromShortLengthSlider.Maximum;
          if (shortLength < FromShortLengthSlider.Minimum) shortLength = FromShortLengthSlider.Minimum;
          mld.FromShortLength = shortLength;
          FromShortLengthSlider.Value = shortLength;
        } catch (FormatException) {
          mld.FromShortLength = FromShortLengthSlider.Minimum;
          FromShortLengthSlider.Value = FromShortLengthSlider.Minimum;
        }
      } else if (sender == ToShortLengthBox && ToShortLengthSlider != null) {
        try {
          double shortLength = double.Parse(ToShortLengthBox.Text);
          if (shortLength > ToShortLengthSlider.Maximum) shortLength = ToShortLengthSlider.Maximum;
          if (shortLength < ToShortLengthSlider.Minimum) shortLength = ToShortLengthSlider.Minimum;
          mld.ToShortLength = shortLength;
          ToShortLengthSlider.Value = shortLength;
        } catch (FormatException) {
          mld.ToShortLength = ToShortLengthSlider.Minimum;
          ToShortLengthSlider.Value = ToShortLengthSlider.Minimum;
        }
      }
      myDiagram.CommitTransaction("Modify Link Route");
    }

  }


  // The data class used for nodes (if a Node is selected,
  // myDiagram.SelectedPart.Data is an instance of MyNodeData).
  public class MyNodeData : GraphLinksModelNodeData<string> {
    // The NodeFigure which is used in the NodeTemplate.
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
    private NodeFigure _Figure = NodeFigure.Actor;

    // The Spot at which all outgoing Links connect to Node (may represent one or more sides). 
    public Spot FromSpot {
      get { return _FromSpot; }
      set {
        if (_FromSpot != value) {
          Spot old = _FromSpot;
          _FromSpot = value;
          RaisePropertyChanged("FromSpot", old, value);
        }
      }
    }
    private Spot _FromSpot = Spot.None;

    // The Spot at which all incoming Links connect to Node (may represent one or more sides). 
    public Spot ToSpot {
      get { return _ToSpot; }
      set {
        if (_ToSpot != value) {
          Spot old = _ToSpot;
          _ToSpot = value;
          RaisePropertyChanged("ToSpot", old, value);
        }
      }
    }
    private Spot _ToSpot = Spot.None;

    // A Color which describes the fill used in the NodeTemplate.  Only set at initialization.
    public String Color { get; set; }
  }


  // The data class used for links (if a Link is selected,
  // myDiagram.SelectedPart.Data is an instance of MyLinkData).
  public class MyLinkData : GraphLinksModelLinkData<string, string> {
    // The LinkCurve which is used in the LinkTemplate.
    public LinkCurve Curve {
      get { return _Curve; }
      set {
        if (_Curve != value) {
          LinkCurve old = _Curve;
          _Curve = value;
          RaisePropertyChanged("Curve", old, value);
        }
      }
    }
    private LinkCurve _Curve = LinkCurve.None;

    // The LinkRouting which is used in the LinkTemplate.
    public LinkRouting Routing {
      get { return _Routing; }
      set {
        if (_Routing != value) {
          LinkRouting old = _Routing;
          _Routing = value;
          RaisePropertyChanged("Routing", old, value);
        }
      }
    }
    private LinkRouting _Routing = LinkRouting.Normal;

    // A double which represents the Curviness property used in the LinkTemplate.
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
    private double _Curviness = 0.0;

    // A double which represents the Corner property used in the LinkTemplate.
    public double Corner {
      get { return _Corner; }
      set {
        if (_Corner != value) {
          double old = _Corner;
          _Corner = value;
          RaisePropertyChanged("Corner", old, value);
        }
      }
    }
    private double _Corner = 0.0;

    // A value of the Arrowhead enum which is used for the LinkPanel.ToArrowProperty.
    public Arrowhead ToArrow {
      get { return _ToArrow; }
      set {
        if (_ToArrow != value) {
          Arrowhead old = _ToArrow;
          _ToArrow = value;
          RaisePropertyChanged("ToArrow", old, value);
        }
      }
    }
    private Arrowhead _ToArrow = Arrowhead.None;

    // A value of the Arrowhead enum which is used for the LinkPanel.FromArrowProperty.
    public Arrowhead FromArrow {
      get { return _FromArrow; }
      set {
        if (_FromArrow != value) {
          Arrowhead old = _FromArrow;
          _FromArrow = value;
          RaisePropertyChanged("FromArrow", old, value);
        }
      }
    }
    private Arrowhead _FromArrow = Arrowhead.None;

    // A double which represents the LinkPanel.ToArrowScaleProperty dependency property.
    public double ToArrowScale {
      get { return _ToArrowScale; }
      set {
        if (_ToArrowScale != value) {
          double old = _ToArrowScale;
          _ToArrowScale = value;
          RaisePropertyChanged("ToArrowScale", old, value);
        }
      }
    }
    private double _ToArrowScale = 1.0;

    // A double which represents the LinkPanel.FromArrowScaleProperty dependency property.
    public double FromArrowScale {
      get { return _FromArrowScale; }
      set {
        if (_FromArrowScale != value) {
          double old = _FromArrowScale;
          _FromArrowScale = value;
          RaisePropertyChanged("FromArrowScale", old, value);
        }
      }
    }
    private double _FromArrowScale = 1.0;

    // A double which represents the Route.ToEndSegmentLengthProperty dependency property.
    public double ToEndSegmentLength {
      get { return _ToEndSegmentLength; }
      set {
        if (_ToEndSegmentLength != value) {
          double old = _ToEndSegmentLength;
          _ToEndSegmentLength = value;
          RaisePropertyChanged("ToEndSegmentLength", old, value);
        }
      }
    }
    private double _ToEndSegmentLength = 10.0;

    // A double which represents the Route.FromEndSegmentLengthProperty dependency property.
    public double FromEndSegmentLength {
      get { return _FromEndSegmentLength; }
      set {
        if (_FromEndSegmentLength != value) {
          double old = _FromEndSegmentLength;
          _FromEndSegmentLength = value;
          RaisePropertyChanged("FromEndSegmentLength", old, value);
        }
      }
    }
    private double _FromEndSegmentLength = 10.0;

    // A double which represents the Route.ToShortLengthProperty dependency property.
    public double ToShortLength {
      get { return _ToShortLength; }
      set {
        if (_ToShortLength != value) {
          double old = _ToShortLength;
          _ToShortLength = value;
          RaisePropertyChanged("ToShortLength", old, value);
        }
      }
    }
    private double _ToShortLength = 0.0;

    // A double which represents the Route.FromShortLengthProperty dependency property.
    public double FromShortLength {
      get { return _FromShortLength; }
      set {
        if (_FromShortLength != value) {
          double old = _FromShortLength;
          _FromShortLength = value;
          RaisePropertyChanged("FromShortLength", old, value);
        }
      }
    }
    private double _FromShortLength = 0.0;

    // A double which represents the Route.SmoothnessProperty dependency property.
    public double Smoothness {
      get { return _Smoothness; }
      set {
        if (_Smoothness != value) {
          double old = _Smoothness;
          _Smoothness = value;
          RaisePropertyChanged("Smoothness", old, value);
        }
      }
    }
    private double _Smoothness = 0.5;

    // A double that is used as the StrokeThickness property in the LinkTemplate.
    public Double Thickness { get; set; }  // Only set at initialization.

    // A Color which describes the stroke used in the LinkTemplate.
    public String Color { get; set; }  // Only set at initialization.
  }


  // Converts between an instance of MyLinkData and a SolidColorBrush for the fill of the FromArrow and ToArrow.
  public class ArrowheadFillConverter : Converter {
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      if (value == null) return null;
      return (LinkPanel.IsFilled((Arrowhead)value) ? new SolidColorBrush(Colors.LightGray) : null);
    }
  }
}