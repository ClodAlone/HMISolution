#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    //using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Represents the diagram page .
    /// <para> The DiagramPage is just a container to hold the objects(nodes and connectors) added through model.
    /// The DiagramView uses the page to display the diagram objects.
    /// </para>
    /// </summary>
    public class DiagramPage : Panel, IDiagramPage
    {
        MultiScaleImage image = new MultiScaleImage();

        internal List<UIElement> AllChildren;
        /// <summary>
        /// Identifies the AllowSelect dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableResizingCurrentNodeOnMultipleSelectionProperty = DependencyProperty.Register("EnableResizingCurrentNodeOnMultipleSelection", typeof(bool), typeof(DiagramPage), new PropertyMetadata(false));

        /// <summary>
        /// Defines the LayoutType property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LayoutTypeProperty = DependencyProperty.Register("LayoutType", typeof(LayoutType), typeof(DiagramPage), new PropertyMetadata(LayoutType.None));

        /// <summary>
        /// Defines the MeasurementUnits property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty MeasurementUnitsProperty = DependencyProperty.Register("MeasurementUnits", typeof(MeasureUnits), typeof(DiagramPage), new PropertyMetadata(MeasureUnits.Pixel, new PropertyChangedCallback(OnUnitsChanged)));

        internal double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Top.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(DiagramPage), new PropertyMetadata(0d, OnTopChanged));

        internal double Bottom
        {
            get { return (double)GetValue(BottomProperty); }
            set { SetValue(BottomProperty, value); }
        }
        internal static double rulerstart;
        // Using a DependencyProperty as the backing store for Bottom.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BottomProperty =
            DependencyProperty.Register("Bottom", typeof(double), typeof(DiagramPage), new PropertyMetadata(0d));

        internal double Right
        {
            get { return (double)GetValue(RightProperty); }
            set { SetValue(RightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Right.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RightProperty =
            DependencyProperty.Register("Right", typeof(double), typeof(DiagramPage), new PropertyMetadata(0d));

        internal double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Left.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(DiagramPage), new PropertyMetadata(0d, OnLeftChanged));

        private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs evtArgs)
        {
            DiagramPage page = d as DiagramPage;
            if (page.dview != null)
            {
                //page.dview.UpdateLayout();
                (((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).Content as FourQuadrantPanel)._Offset.Y += (double)evtArgs.NewValue - (double)evtArgs.OldValue;
                //page.dview.Scrollviewer.ScrollToVerticalOffset(page.dview.Scrollviewer.VerticalOffset + (double)evtArgs.NewValue - (double)evtArgs.OldValue);
                FrameworkElement fe = page.Parent as FrameworkElement;
                if (fe != null)
                {
                    fe.InvalidateMeasure();
                    page.dview.ScrollGrid.InvalidateMeasure();
                }
                //
                if (page.dview.VerRuler != null && !page.dview.IsPanEnabled)
                {
                    page.dview.VerRuler.OffsetY = (-page.dview.ScrollGrid._VerticalOffset);
                    page.dview.VerRuler.PxStartValue = -page.Top;
                    page.dview.VerRuler.InvalidateMeasure();
                }
                if (page.dview.dviewGrid != null)
                {
                    TransformGroup tr = new TransformGroup();
                    //tr.Children.Add(page.dview.zoomTransform);
                    tr.Children.Add(new TranslateTransform { X = -(page.FindNeturalLine(page.Left, (page as DiagramPage).PxGridHorizontalOffset)), Y = -(page.FindNeturalLine(page.Top, (page as DiagramPage).PxGridVerticalOffset)) });

                    (page.dview.dviewGrid as DiagramViewGrid).Measure(new Size(page.dview.ScrollGrid.ExtentWidth + (page as DiagramPage).PxGridHorizontalOffset * 2, page.dview.ScrollGrid.ExtentHeight + (page as DiagramPage).PxGridVerticalOffset * 2));
                    (page.dview.dviewGrid as DiagramViewGrid).RenderTransform = tr;
                    (page.dview.dviewGrid as DiagramViewGrid).UpdateGrid();
                }
            }
            //OverviewContentHolder.SetStart(page, new Point(page.Left, page.Top));
        }

        private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs evtArgs)
        {
            DiagramPage page = d as DiagramPage;
            if (page.dview != null)
            {

                (((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).Content as FourQuadrantPanel)._Offset.X += (double)evtArgs.NewValue - (double)evtArgs.OldValue;
                rulerstart = (double)evtArgs.NewValue;
                if (page.dview.dviewGrid != null)
                {
                    TransformGroup tr = new TransformGroup();
                    //tr.Children.Add(page.dview.zoomTransform);
                    tr.Children.Add(new TranslateTransform { X = -(page.FindNeturalLine(page.Left, (page as DiagramPage).PxGridHorizontalOffset)), Y = -(page.FindNeturalLine(page.Top, (page as DiagramPage).PxGridVerticalOffset)) });

                    (page.dview.dviewGrid as DiagramViewGrid).Measure(new Size(page.dview.ScrollGrid.ExtentWidth + (page as DiagramPage).PxGridHorizontalOffset * 2, page.dview.ScrollGrid.ExtentHeight + (page as DiagramPage).PxGridVerticalOffset * 2));
                    (page.dview.dviewGrid as DiagramViewGrid).RenderTransform = tr;
                    (page.dview.dviewGrid as DiagramViewGrid).UpdateGrid();
                }

            }
            if (page.dview.HorRuler != null && !page.dview.IsPanEnabled)
            {
                page.dview.HorRuler.OffsetX = -(-page.Left + page.dview.ScrollGrid._HorizontalOffset);

                page.dview.HorRuler.PxStartValue = -page.Left;
                page.dview.HorRuler.InvalidateMeasure();

                // page.dview.HorRuler.InvalidateMeasure();
                //if (view.HorRuler.OffsetX < 0)
                //{
                page.dview.HorRuler.Margin = new Thickness(-(page.dview.ScrollGrid._HorizontalOffset), 0, 0, 0);
            }
            if (page.dview.EnableVirtualization && !page.dview.IsPanEnabled)
            {
                page.dview.VerifyVirtualization();
            }
            page.dview.ScrollGrid.InvalidateMeasure();
            //OverviewContentHolder.SetStart(page, new Point(page.Left, page.Top));
        }


        internal double FindNeturalLine(double left, double offset)
        {
            double newleft = left + offset - (left % offset);
            return newleft;
        }
        internal static string Pathstring = string.Empty;

        /// <summary>
        /// Used to store the measure units.
        /// </summary>
        private static MeasureUnits munits;

        /// <summary>
        /// Used to store selection list
        /// </summary>
        private NodeCollection mselectionList;

        /// <summary>
        /// Used to store a static object.
        /// </summary>
        internal static object o;

        internal static List<UIElement> _groupcollection;
        /// <summary>
        /// Used to store the page.
        /// </summary>
        private DiagramPage page;

        /// <summary>
        /// Used to refer to the child count value.
        /// </summary>
        internal bool childcount = true;

        /// <summary>
        ///  Used to store the minx.
        /// </summary>
        private double cminx = 0;

        /// <summary>
        ///  Used to store the miny
        /// </summary>
        private double cminy = 0;

        /// <summary>
        /// Used to store the constant minx
        /// </summary>
        private double cmx = 0;

        /// <summary>
        ///  Used to store the constant miny
        /// </summary>
        private double cmy = 0;

        /// <summary>
        ///  Used to store the  current minx
        /// </summary>
        private double curminx = 0;

        /// <summary>
        ///  Used to store the current miny
        /// </summary>
        private double curminy = 0;

        /// <summary>
        /// Used to store the diagram control.
        /// </summary>
        private DiagramControl dc;

        /// <summary>
        /// Used to refer to the diagram control instance.
        /// </summary>
        private DiagramControl diagctrl;

        /// <summary>
        /// Used to store the drag left value.
        /// </summary>
        private double dleft = 0;

        /// <summary>
        /// Used to store the drag top value.
        /// </summary>
        private double dtop = 0;

        /// <summary>
        /// Used to store the View instance.
        /// </summary>
        internal DiagramView dview;

        internal Point endPoint = new Point(0, 0);

        /// <summary>
        /// Used to refer to the execution instance.
        /// </summary>
        private bool exeonce = false;

        /// <summary>
        ///  Used to store the GreaterThanZero value.
        /// </summary>
        private bool gtz = false;

        /// <summary>
        ///  Used to store the GreaterThanZeroY bool value.
        /// </summary>
        private bool gtzy = false;

        /// <summary>
        ///  Used to store the horizontal offset.
        /// </summary>
        private double horizontaloffset = 0;

        /// <summary>
        /// Used to refer to the horizontal offset
        /// </summary>
        private double horoffset = 25d;

        /// <summary>
        /// Used to store the horizontal spacing reference.
        /// </summary>
        private double href;

        /// <summary>
        /// Used to store the horizontal offset on adding connectors.
        /// </summary>
        private double hval = 0;

        /// <summary>
        /// Used to store the int value.
        /// </summary>
        private int i = 0;

        /// <summary>
        /// Used to check if executed.
        /// </summary>
        private bool isexe = false;

        /// <summary>
        /// Used to refer to the not pixel offset.
        /// </summary>
        private bool isnotpixeloffset = false;

        /// <summary>
        /// Used to store the unit details 
        /// </summary>
        private bool isnotpixelwh = false;

        /// <summary>
        /// Used to store the changed unit.
        /// </summary>
        private bool isunitchanged = false;

        /// <summary>
        /// Used to store the least negative offsetx.
        /// </summary>
        private double leastx = 0;

        /// <summary>
        /// Used to store the least negative offsety.
        /// </summary>
        private double leasty = 0;

        /// <summary>
        /// Used to check if connector is dropped.
        /// </summary>
        private bool linedrop = false;

        /// <summary>
        /// Used to check if the page is loaded.
        /// </summary>
        private bool loaded = false;

        /// <summary>
        /// Used to store the line style reference value.
        /// </summary>
        private Style lstyleref;

        /// <summary>
        /// Used to store the layout type.
        /// </summary>
        private LayoutType ltref;

        /// <summary>
        /// Used to store the connector type.
        /// </summary>
        private ConnectorType mconnectionType = ConnectorType.Orthogonal;

        /// <summary>
        /// Used to refer to the unit changed event.
        /// </summary>
        private bool munitchanged = false;

        /// <summary>
        /// Used to store max value.
        /// </summary>
       // private double max = 0;

        private bool mcall = false;

        /// <summary>
        /// Used to store the minimumx.
        /// </summary>
        private double minimumX = 0;

        /// <summary>
        /// Used to store the minimumy.
        /// </summary>
        private double minimumY = 0;

        /// <summary>
        /// Used to store the minleftx value.
        /// </summary>
        private double minleftX = 0;

        /// <summary>
        /// Used to store the this.mintopY value.
        /// </summary>
        private double mintopY = 0;

        /// <summary>
        /// Used to refer to the name count
        /// </summary>
        internal int namecount = 0;

        /// <summary>
        /// Used to refer to the children count.
        /// </summary>
        /// <remarks></remarks>
        internal int no;

        /// <summary>
        /// Used to refer to the not first exe value.
        /// </summary>
        private bool notfirstexe = false;

        /// <summary>
        /// Used to store the units
        /// </summary>
        private MeasureUnits ounit;

        /*
        /// <summary>
        /// Used to store boolean value on executing once.
        /// </summary>
       // private bool once = false;

        /// <summary>
        /// Used to store boolean value on reaching zero once.
        /// </summary>
       // private bool oncezero = false;
        */
        /// <summary>
        /// Used to store the  tree orientation
        /// </summary>
        private TreeOrientation oref;

        /// <summary>
        /// Used to store the IsPositive value.
        /// </summary>
        private bool pos = false;

        /// <summary>
        ///  Used to store the IsPositiveY bool value.
        /// </summary>
        private bool posy = false;

        /*
        /// <summary>
        /// Used to store double value.
        /// </summary>
        //private double s = 0;

        /// <summary>
        /// Used to store mouse scroll count
        /// </summary>
        //private int scrollcount = 0;
        */

        /// <summary>
        /// Used to store the space between the sub-trees.
        /// </summary>
        private double sref;

        /// <summary>
        /// Used to store the start point.
        /// </summary>
        internal Point? startPoint = null;

        /// <summary>
        /// Used to store the resize bool value.
        /// </summary>
        private Style styleref;

        /// <summary>
        /// Used to store the horizontal scollbar width.
        /// </summary>
        private double sx = 0;

        /// <summary>
        /// Used to store the vertical scollbar width.
        /// </summary>
        private double sy = 0;

        internal ResourceDictionary symbols = new ResourceDictionary();

        /// <summary>
        /// Used to store the units temporarily.
        /// </summary>
        private MeasureUnits temp;

        /// <summary>
        /// Used to check if the node transformation is done.
        /// </summary>
        private bool transformed = false;

        /// <summary>
        /// Checks if the unit got changed.
        /// </summary>
        private bool unitchan = false;

        /// <summary>
        ///  Used to store the vertical offset.
        /// </summary>
        private double verticaloffset = 0;

        /// <summary>
        /// Used to refer to the vertical offset. Default value is 25d.
        /// </summary>
        private double vertoffset = 25d;

        /// <summary>
        /// Used to store the vertical spacing reference.
        /// </summary>
        private double vref;

        /// <summary>
        /// Used to store the vertical offset on adding connectors.
        /// </summary>
        private double vval = 0;


        /// <summary>
        /// Used to Start Point of the Page.
        /// </summary>
        internal Point spoint;

        internal enum GridOffsets
        {
            Horizontal,
            Vertical
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramPage"/> class.
        /// </summary>
        public DiagramPage()
        {
            this.AllChildren = new List<UIElement>();
            this.Loaded += new RoutedEventHandler(this.DiagramPage_Loaded);
            if (!this.CaptureMouse())
            {
                this.Background = new SolidColorBrush(Colors.Transparent);
            }
            this.MouseRightButtonDown += new MouseButtonEventHandler(DiagramPage_MouseRightButtonDown);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(this.DiagramPage_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(this.DiagramPage_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(this.DiagramPage_MouseMove);
            this.KeyDown += new KeyEventHandler(this.DiagramPage_KeyDown);
            this.SizeChanged += new SizeChangedEventHandler(DiagramPage_SizeChanged);
            this.MouseWheel += new MouseWheelEventHandler(DiagramPage_MouseWheel);
        }

        internal Size PageSize = new Size();
        void DiagramPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (dc.View != null && dview.nodragging)
            {
                dc.View.Page.InvalidateMeasure();
                this.Right = e.NewSize.Width;
                this.Bottom = e.NewSize.Height;
            }
            PageSize = e.NewSize;
            if (dview.HorRuler != null && e.NewSize.Width - e.PreviousSize.Width < 0)
            {
                page = dview.Page as DiagramPage;
                page.dview.HorRuler.OffsetX = -(-page.Left + page.dview.ScrollGrid.ScrollOwner.HorizontalOffset);
                page.dview.HorRuler.PxStartValue = -page.Left;
                page.dview.HorRuler.InvalidateMeasure();
                page.dview.HorRuler.Margin = new Thickness(-(page.dview.ScrollGrid.ScrollOwner.HorizontalOffset), 0, 0, 0);
            }
            if (this.dview.VerRuler != null && e.NewSize.Height - e.PreviousSize.Height < 0)
            {
                page = dview.Page as DiagramPage;
                page.dview.VerRuler.OffsetY = (-page.dview.ScrollGrid.ScrollOwner.VerticalOffset);
                page.dview.VerRuler.PxStartValue = -page.Top;
                dview.VerRuler.Margin = new Thickness(0, 0, (-page.dview.ScrollGrid.ScrollOwner.VerticalOffset), 0);
                page.dview.VerRuler.InvalidateMeasure();
            }
            
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the Actual Height of the DiagramPage.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Actual Height of the DiagramPage in pixels.
        /// </value>
        public new double ActualHeight
        {
            get
            {
                return base.ActualHeight;
            }
        }

        /// <summary>
        /// Gets the Actual Width of the DiagramPage.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Actual Width of the DiagramPage in pixels.
        /// </value>
        public new double ActualWidth
        {
            get
            {
                return base.ActualWidth;
            }
        }

        /// <summary>
        /// Gets or sets the the type of connection to be used.
        /// </summary>
        /// <value>
        /// Type: <see cref="ConnectorType"/>
        /// Enum specifying the type of the connector to be used.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ConnectorType in C#.
        /// <code language="C#">
        /// connObject.ConnectorType = ConnectorType.Orthogonal;
        /// </code>
        /// </example>
        public ConnectorType ConnectorType
        {
            get
            {
                return this.mconnectionType;
            }

            set
            {
                if (value != this.mconnectionType)
                {
                    this.mconnectionType = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable resizing current node on multiple selection].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable resizing current node on multiple selection]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableResizingCurrentNodeOnMultipleSelection
        {
            get
            {
                return (bool)GetValue(EnableResizingCurrentNodeOnMultipleSelectionProperty);
            }

            set
            {
                SetValue(EnableResizingCurrentNodeOnMultipleSelectionProperty, value);
            }
        }

        internal double PxGridHorizontalOffset
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(GridHorizontalOffset, this.MeasurementUnits);
            }
            set
            {
                GridHorizontalOffset = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            }
        }

        internal double PxGridVerticalOffset
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(GridVerticalOffset, this.MeasurementUnits);
            }
            set
            {
                GridVerticalOffset = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            }
        }

        /// <summary>
        /// Gets or sets the HorizontalOffset value of the grid .
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Offset value.
        /// </value>
        /// <remarks>
        /// Default value is 25d.</remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       (View.Page as DiagramPage).GridHorizontalOffset=100;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double GridHorizontalOffset
        {
            get
            {
                return this.horoffset;
            }
            set
            {
                setGridOffsets(value, GridOffsets.Horizontal);
                updateGridLines(GridOffsets.Horizontal);
            }
        }

        /// <summary>
        /// Gets or sets the VerticalOffset value of the grid .
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Offset value.
        /// </value>
        /// <remarks>
        /// Default value is 25d.</remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       (View.Page as DiagramPage).GridVerticalOffset=100;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double GridVerticalOffset
        {
            get
            {
                return this.vertoffset;
            }

            set
            {
                setGridOffsets(value, GridOffsets.Vertical);
                updateGridLines(GridOffsets.Vertical);
            }
        }

        /// <summary>
        /// Gets or sets the Horizontal spacing reference.Used for Serialization purpose.
        /// </summary>
        /// <value>The horizontal spacing reference.</value>
        public double HorizontalSpacingref
        {
            get { return this.href; }
            set { this.href = value; }
        }

        /// <summary>
        /// Gets or sets the Measurement unit property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MeasureUnits"/>
        /// Enum specifying the unit to be used.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public MainPage()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       (View.Page as DiagramPage).MeasurementUnits = MeasureUnits.Inch;
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 1.5;
        ///        n.OffsetY = 2.5;
        ///        n.Width = 1.5;
        ///        n.Height = 0.75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public MeasureUnits MeasurementUnits
        {
            get
            {
                return (MeasureUnits)GetValue(MeasurementUnitsProperty);
            }

            set
            {
                SetValue(MeasurementUnitsProperty, value);
            }
        }

        /// <summary>
        ///  Gets or sets the SpaceBetweenSubTreeSpacing reference .Used for Serialization purpose.
        /// </summary>
        /// <value>The sub tree spacing reference.</value>
        public LayoutType LayoutTyperef
        {
            get { return this.ltref; }
            set { this.ltref = value; }
        }

        /// <summary>
        /// Gets or sets the LineStyleRef reference. Used for Serialization purpose.
        /// </summary>
        /// <value>The line style ref.</value>
        public Style LineStyleRef
        {
            get { return this.lstyleref; }
            set { this.lstyleref = value; }
        }

        /// <summary>
        /// Gets or sets the orientation reference. Used for Serialization purpose.
        /// </summary>
        /// <value>The orientation ref.</value>
        public TreeOrientation OrientationRef
        {
            get { return this.oref; }
            set { this.oref = value; }
        }

        /// <summary>
        /// Gets or sets the reference count. Used for serialization purposes
        /// </summary>
        /// <value>The reference count.</value>
        public int ReferenceCount
        {
            get { return this.i; }
            set { this.i = value; }
        }

        /// <summary>
        /// Gets the Selection List of the items.
        /// </summary>
        /// <value>
        /// Type: <see cref="NodeCollection"/>
        /// The list containing the selected items.
        /// </value>
        public NodeCollection SelectionList
        {
            get
            {
                if (mselectionList == null)
                {
                    mselectionList = new NodeCollection(this);
                }

                return mselectionList;
            }
        }

        /// <summary>
        /// Gets or sets the style reference. Used for Serialization purpose.
        /// </summary>
        /// <value>The style ref.</value>
        public Style StyleRef
        {
            get { return this.styleref; }
            set { this.styleref = value; }
        }

        /// <summary>
        ///  Gets or sets the SpaceBetweenSubTreeSpacing reference .Used for Serialization purpose.
        /// </summary>
        /// <value>The sub tree spacing reference.</value>
        public double SubTreeSpacingref
        {
            get { return this.sref; }
            set { this.sref = value; }
        }

        /// <summary>
        /// Gets or sets the vertical spacing reference .Used for Serialization purpose.
        /// </summary>
        /// <value>The vertical spacing reference.</value>
        public double VerticalSpacingref
        {
            get { return this.vref; }
            set { this.vref = value; }
        }

        /// <summary>
        /// Gets or sets the measure units.
        /// </summary>
        /// <value>The measure unit.</value>
        internal static MeasureUnits Munits
        {
            get { return munits; }
            set { munits = value; }
        }

        /// <summary>
        /// Gets or sets the const min X.
        /// </summary>
        /// <value>The const min X.</value>
        internal double ConstMinX
        {
            get { return this.cminx; }
            set { this.cminx = value; }
        }

        /// <summary>
        /// Gets or sets the const min Y.
        /// </summary>
        /// <value>The const min Y.</value>
        internal double ConstMinY
        {
            get { return this.cminy; }
            set { this.cminy = value; }
        }

        /// <summary>
        /// Gets or sets the current min X.
        /// </summary>
        /// <value>The current min X.</value>
        internal double CurrentMinX
        {
            get { return this.cmx; }
            set { this.cmx = value; }
        }

        /// <summary>
        /// Gets or sets the current min Y.
        /// </summary>
        /// <value>The current min Y.</value>
        internal double CurrentMinY
        {
            get { return this.cmy; }
            set { this.cmy = value; }
        }

        /// <summary>
        /// Gets or sets the Dragleft. Used for serialization purpose
        /// </summary>
        /// <value>The Dragged value.</value>
        internal double Dragleft
        {
            get { return this.dleft; }
            set { this.dleft = value; }
        }

        /// <summary>
        /// Gets or sets the Dragtop. Used for serialization purpose
        /// </summary>
        /// <value>The dragged value.</value>
        internal double Dragtop
        {
            get { return this.dtop; }
            set { this.dtop = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [greater than zero].
        /// </summary>
        /// <value><c>true</c> if [greater than zero]; otherwise, <c>false</c>.</value>
        internal bool GreaterThanZero
        {
            get { return this.gtz; }
            set { this.gtz = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [greater than zero Y].
        /// </summary>
        /// <value><c>true</c> if [greater than zero Y]; otherwise, <c>false</c>.</value>
        internal bool GreaterThanZeroY
        {
            get { return this.gtzy; }
            set { this.gtzy = value; }
        }

        /// <summary>
        /// Gets or sets the horizontal offset.
        /// </summary>
        /// <value>The horizontal offset.</value>
        internal double Hor
        {
            get { return this.horizontaloffset; }
            set { this.horizontaloffset = value; }
        }

        /// <summary>
        /// Gets or sets the horizontal offset value.
        /// </summary>
        /// <value>The horizontal offset value.</value>
        internal double HorValue
        {
            get { return this.hval; }
            set { this.hval = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether connector is dropped.
        /// </summary>
        /// <value>
        /// <c>true</c> if connector is dropped; otherwise, <c>false</c>.
        /// </value>
        internal bool IsConnectorDropped
        {
            get { return this.linedrop; }
            set { this.linedrop = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether diagram page is loaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if diagram page is loaded; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDiagrampageLoaded
        {
            get { return this.loaded; }
            set { this.loaded = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is positive.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is positive; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPositive
        {
            get { return this.pos; }
            set { this.pos = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is positive Y.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is positive Y; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPositiveY
        {
            get { return this.posy; }
            set { this.posy = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether  <see cref="Node"/> is rotated or resized.
        /// </summary>
        /// <value><c>true</c> if transformed; otherwise, <c>false</c>.</value>
        internal bool Istransformed
        {
            get { return this.transformed; }
            set { this.transformed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the unit is changed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is unit changed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsUnitChanged
        {
            get
            {
                return this.isunitchanged;
            }

            set
            {
                this.isunitchanged = value;
            }
        }

        /// <summary>
        /// Gets or sets the least X.
        /// </summary>
        /// <value>The least X.</value>
        internal double LeastX
        {
            get { return this.leastx; }
            set { this.leastx = value; }
        }

        /// <summary>
        /// Gets or sets the least Y.
        /// </summary>
        /// <value>The least Y.</value>
        internal double LeastY
        {
            get { return this.leasty; }
            set { this.leasty = value; }
        }

        internal bool ManualMeasureCall
        {
            get { return this.mcall; }
            set { this.mcall = value; }
        }

        /// <summary>
        /// Gets or sets the minleft.
        /// </summary>
        /// <value>The minleft.</value>
        internal double Minleft
        {
            get { return this.minleftX; }
            set { this.minleftX = value; }
        }

        /// <summary>
        /// Gets or sets the min top.
        /// </summary>
        /// <value>The min top.</value>
        internal double MinTop
        {
            get { return this.mintopY; }
            set { this.mintopY = value; }
        }

        /// <summary>
        /// Gets or sets the min X.
        /// </summary>
        /// <value>The min X.</value>
        internal double MinX
        {
            get { return this.minimumX; }
            set { this.minimumX = value; }
        }

        /// <summary>
        /// Gets or sets the min Y.
        /// </summary>
        /// <value>The min Y.</value>
        internal double MinY
        {
            get { return this.minimumY; }
            set { this.minimumY = value; }
        }

        /// <summary>
        /// Gets or sets the old min X.
        /// </summary>
        /// <value>The old min X.</value>
        internal double OldMinX
        {
            get { return this.curminx; }
            set { this.curminx = value; }
        }

        /// <summary>
        /// Gets or sets the old min Y.
        /// </summary>
        /// <value>The old min Y.</value>
        internal double OldMinY
        {
            get { return this.curminy; }
            set { this.curminy = value; }
        }

        /// <summary>
        /// Gets or sets the old unit.
        /// </summary>
        /// <value>The old unit.</value>
        internal MeasureUnits OldUnit
        {
            get
            {
                return this.ounit;
            }

            set
            {
                this.ounit = value;
            }
        }

        /// <summary>
        /// Gets or sets the scroll X.
        /// </summary>
        /// <value>The scroll X.</value>
        internal double ScrollX
        {
            get
            {
                return this.sx;
            }

            set
            {
                this.sx = value;
            }
        }

        /// <summary>
        /// Gets or sets the scroll Y.
        /// </summary>
        /// <value>The scroll Y.</value>
        internal double ScrollY
        {
            get
            {
                return this.sy;
            }

            set
            {
                this.sy = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the unit changed.
        /// </summary>
        /// <value><c>true</c> if unit converted; otherwise, <c>false</c>.</value>
        internal bool Unitconverted
        {
            get { return this.unitchan; }
            set { this.unitchan = value; }
        }

        /// <summary>
        /// Gets or sets the vertical offset.
        /// </summary>
        /// <value>The vertical offset.</value>
        internal double Ver
        {
            get { return this.verticaloffset; }
            set { this.verticaloffset = value; }
        }

        /// <summary>
        /// Gets or sets the vertical offset value.
        /// </summary>
        /// <value>The vertical offset value.</value>
        internal double VerValue
        {
            get { return this.vval; }
            set { this.vval = value; }
        }

        /// <summary>
        /// Creates a clone of the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void Copyitem(object obj,SymbolPaletteItem itema)
        {
            o = obj;
            symboll = itema;
        }
        internal static SymbolPaletteItem symboll;
        public static void CopyItemGroup(List<UIElement> GroupNode, SymbolPaletteItem itema)
        {
            _groupcollection = GroupNode;
            symboll = itema;
        }
        /// <summary>
        /// Invalidates the measures.
        /// </summary>
        public new void InvalidateMeasure()
        {
            base.InvalidateMeasure();
        }

        public override void OnApplyTemplate()
        {
            this.dview = Node.GetDiagramView(this);
            this.dc = GetDiagramControl(this);

            if (dc != null && dc.Model != null)
            {
                if (dc.Model.MeasurementUnits != MeasurementUnits)
                {
                    //dc.Model.m_IsPixelDefultUnit = false;
                }
            }
        }

        /// <summary>
        /// Gets the Diagram Control object.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The Diagram Control object</returns>
        internal static DiagramControl GetDiagramControl(FrameworkElement element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                if (parent is DiagramControl)
                {
                    return parent as DiagramControl;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Measures elements.
        /// </summary>
        /// <param name="availableSize">The available size</param>
        /// <returns>The available size.</returns>
        /// 

        protected override Size MeasureOverride(Size availableSize)
        {
            Size visualDesiredSize = new Size(0, 0);
            Size dataDesiredSize = new Size(0, 0);
            Size negativeSize = new Size(0, 0);
            Rect rect = new Rect();
            //double offx = 0;
            //double offy = 0;
            //double offy2 = 0;
            //double offx1 = 0;
            this.dc = GetDiagramControl(this);
            if (dview != null)
            {
                if (dview.ScrollGrid != null)
                {
                    dview.ScrollGrid.InvalidateMeasure();
                }
            }
            dview = dc.View;
            foreach (UIElement ele in this.AllChildren)
            {
                if (ele is INodeGroup)
                {
                    if ((ele as INodeGroup).ReferenceNo < 0)
                    {
                        (ele as INodeGroup).ReferenceNo = ReferenceCount++;
                    }
                }
                if (this.Children.Contains(ele))
                {
                    ele.Measure(availableSize);
                }

                //double right = ((Matrix)ele.TransformToVisual(this).GetValue(MatrixTransform.MatrixProperty)).OffsetX;
                //double bottom = ((Matrix)ele.TransformToVisual(this).GetValue(MatrixTransform.MatrixProperty)).OffsetY;
                if (ele is Node)
                {
                    rect = new Rect((ele as Node).PxOffsetX, (ele as Node).PxOffsetY, (ele as Node).ActualWidth, (ele as Node).ActualHeight);
                }

                if (ele is DiagramViewGrid)
                {
                    rect = Rect.Empty;
                    continue;
                }
                else if (ele is LineConnector)
                {
                    if (ele is IEdge)
                    {
                        rect = (ele as LineConnector).GetBounds();
                    }
                }
                if (ele is Node)
                {
                    dataDesiredSize.Width = Math.Max(dataDesiredSize.Width, (ele as Node).OffsetX + (ele as IShape).ActualWidth);
                    dataDesiredSize.Height = Math.Max(dataDesiredSize.Height, (ele as Node).OffsetY + (ele as IShape).ActualHeight);
                }
                if (ele is ICommon)
                {
                    visualDesiredSize.Width = Math.Max(rect.Right, visualDesiredSize.Width);
                    visualDesiredSize.Height = Math.Max(rect.Bottom, visualDesiredSize.Height);
                    //visualDesiredSize.Width = Math.Max(offx, visualDesiredSize.Width);
                    //visualDesiredSize.Height = Math.Max(offy, visualDesiredSize.Height);
                    negativeSize.Width = Math.Max(-rect.Left, negativeSize.Width);
                    negativeSize.Height = Math.Max(-rect.Top, negativeSize.Height);
                }
            }

            if (dview != null)
            {

                //if (dview != null && dview.nodragging)
                //{
                //    visualDesiredSize.Width *= dview.CurrentZoom;
                //    visualDesiredSize.Height *= dview.CurrentZoom;
                    Top = negativeSize.Height * dview.CurrentZoom;
                    Left = negativeSize.Width * dview.CurrentZoom;
                    if (dview.ScrollGrid != null)
                    {
                        Top = Math.Max(Top, dview.ScrollGrid.ScrollPoint.Y);
                        Left = Math.Max(Left, dview.ScrollGrid.ScrollPoint.X);
                    }
                    //if (dview.ScrollGrid._MasterParent != null)
                    //{
                    //   OverviewContentHolder.SetOrigin(dview.ScrollGrid,new Point(-Left,-Top));
                    //}
                    Left += dview.PageMargin.Left * dview.CurrentZoom;
                    Top += dview.PageMargin.Top * dview.CurrentZoom;

                //}
                // dview.background.Measure(visualDesiredSize);
                //dview.background.Width = visualDesiredSize.Width + this.Left;
                //dview.background.Height = visualDesiredSize.Height + this.Top;
            }
            //if (visualDesiredSize.Equals(new Size(0, 0)))
            //{
            //    return dataDesiredSize;
            //}
            if (dview != null && !dview.SizeToContent)
            {
                if (visualDesiredSize.Width <= dview.BoundaryConstraintsArea.Right)
                {
                    if (visualDesiredSize.Height <= dview.BoundaryConstraintsArea.Bottom)
                    {
                        return new Size(dview.BoundaryConstraintsArea.Right + dview.PageMargin.Right, dview.BoundaryConstraintsArea.Bottom + dview.PageMargin.Bottom);
                    }
                    else
                    {
                        return new Size(dview.BoundaryConstraintsArea.Right + dview.PageMargin.Right, visualDesiredSize.Height + dview.PageMargin.Bottom);
                    }
                }
                else if (visualDesiredSize.Height <= dview.BoundaryConstraintsArea.Bottom)
                {
                    return new Size(visualDesiredSize.Width + dview.PageMargin.Right, dview.BoundaryConstraintsArea.Bottom + dview.PageMargin.Bottom);
                }
                else
                {
                    return new Size(visualDesiredSize.Width + dview.PageMargin.Right, visualDesiredSize.Height + dview.PageMargin.Bottom);
                }

            }
            else
            {
                return new Size(visualDesiredSize.Width + dview.PageMargin.Right, visualDesiredSize.Height + dview.PageMargin.Bottom);
            }
        }
        /// <summary>
        /// Raised when the appropriate property changes.
        /// </summary>
        /// <param name="name">The property name.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        internal Rect PageData()
        {
            Size visualDesiredSize = new Size(0, 0);
            Size dataDesiredSize = new Size(0, 0);
            Size negativeSize = new Size(0, 0);
            Size positiveSize = new Size(0, 0);
            Rect rect = new Rect();
            Size PageSize = new Size();
            //double offx = 0;
            //double offy = 0;
            //double offy2 = 0;
            //double offx1 = 0;
            foreach (UIElement ele in this.AllChildren)
            {
                if (ele is INodeGroup)
                {
                    if ((ele as INodeGroup).ReferenceNo < 0)
                    {
                        (ele as INodeGroup).ReferenceNo = ReferenceCount++;
                    }
                }
                if (ele is Node)
                {
                    rect = new Rect((ele as Node).PxOffsetX, (ele as Node).PxOffsetY, (ele as Node).ActualWidth, (ele as Node).ActualHeight);
                    // PageSize=new Size((ele as Node).PxOffsetX+(ele as Node).ActualWidth, (ele as Node).PxOffsetY+ (ele as Node).ActualHeight);
                }

                if (ele is DiagramViewGrid)
                {
                    rect = Rect.Empty;
                    continue;
                }
                else if (ele is LineConnector)
                {
                    if (ele is IEdge)
                    {
                        rect = (ele as LineConnector).GetBounds();
                        //offx = Math.Max((ele as LineConnector).PxStartPointPosition.X, (ele as LineConnector).PxEndPointPosition.X);
                        //offy = Math.Max((ele as LineConnector).PxStartPointPosition.Y, (ele as LineConnector).PxEndPointPosition.Y);

                        //offx1 = Math.Min((ele as LineConnector).PxStartPointPosition.X, (ele as LineConnector).PxEndPointPosition.X);
                        //offy2 = Math.Min((ele as LineConnector).PxStartPointPosition.Y, (ele as LineConnector).PxEndPointPosition.Y);
                        //negativeSize.Width = Math.Max(-offx1, negativeSize.Width);
                        //negativeSize.Height = Math.Max(-offy2, negativeSize.Height);
                    }
                }
                if (ele is Node)
                {
                    dataDesiredSize.Width = Math.Max(dataDesiredSize.Width, (ele as Node).OffsetX + (ele as IShape).ActualWidth);
                    dataDesiredSize.Height = Math.Max(dataDesiredSize.Height, (ele as Node).OffsetY + (ele as IShape).ActualHeight);
                }
                if (ele is ICommon)
                {
                    visualDesiredSize.Width = Math.Max(rect.Right, visualDesiredSize.Width);
                    visualDesiredSize.Height = Math.Max(rect.Bottom, visualDesiredSize.Height);
                    //visualDesiredSize.Width = Math.Max(offx, visualDesiredSize.Width);
                    //visualDesiredSize.Height = Math.Max(offy, visualDesiredSize.Height);
                    negativeSize.Width = Math.Max(-rect.Left, negativeSize.Width);
                    negativeSize.Height = Math.Max(-rect.Top, negativeSize.Height);
                    positiveSize.Width = Math.Max(PageSize.Width, positiveSize.Width);
                    positiveSize.Height = Math.Max(PageSize.Height, positiveSize.Height);
                }
            }

            if (dview != null)
            {
                visualDesiredSize.Width *= dview.CurrentZoom;
                visualDesiredSize.Height *= dview.CurrentZoom;
                Top = negativeSize.Height * dview.CurrentZoom;
                Left = negativeSize.Width * dview.CurrentZoom;
                if (dview.IsPanEnabled)
                {
                    Top = Math.Max(Top, dview.ScrollGrid.ScrollPoint.Y);
                    Left = Math.Max(Left, dview.ScrollGrid.ScrollPoint.X);
                }
                Left += dview.PageMargin.Left;
                Top += dview.PageMargin.Top;
            }

            return new Rect(Left, Top, dataDesiredSize.Width, dataDesiredSize.Height);
        }


        internal void RulerUpdate()
        {
            if (this.dview.HorRuler != null && !this.dview.IsPanEnabled)
            {
                this.dview.HorRuler.OffsetX = -(-this.Left + this.dview.ScrollGrid.ScrollOwner.HorizontalOffset);
                this.dview.HorRuler.PxStartValue = -this.Left;
                this.dview.HorRuler.Margin = new Thickness(-(this.dview.ScrollGrid.ScrollOwner.HorizontalOffset), 0, 0, 0);

            }
            if (this.dview.VerRuler != null && !this.dview.IsPanEnabled)
            {
                this.dview.VerRuler.OffsetY = (-this.dview.ScrollGrid.ScrollOwner.VerticalOffset);
                this.dview.VerRuler.PxStartValue = -this.Top;
                this.dview.VerRuler.Margin = new Thickness(0, 0, -(this.dview.ScrollGrid.ScrollOwner.VerticalOffset), 0);
            }
        }
        /// <summary>
        /// Positions child elements and determines a size for the control.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (dview != null)
            {
                // dview.background.Arrange(new Rect(-this.Left, -this.Top, finalSize.Width+this.Left, finalSize.Height+this.Top));
            }
            foreach (UIElement element in this.Children)
            {
                double offsetX = 0;
                double offsetY = 0;
                if (element is IShape)
                {
                    offsetX = (element as Node).PxOffsetX;// MeasureUnitsConverter.ToPixels((element as IShape).OffsetX, this.MeasurementUnits);
                    offsetY = (element as Node).PxOffsetY;// MeasureUnitsConverter.ToPixels((element as IShape).OffsetY, this.MeasurementUnits);
                }
                if (element is LineConnector)
                {
                    if ((element as LineConnector).LineRoutingEnabled)
                    {
                        if (this.dc != null && this.dc.View != null && (!this.dc.View._LineRoute || this.dc.View.RoutingMode == RoutingMode.Immediate) )
                        {
                            AStarLineRouter asl = new AStarLineRouter((element as LineConnector).dc.View);
                            if ((element as LineConnector).isRouting == true)
                            {
                                (element as LineConnector).dc.View.LineRouter = asl;
                                (element as LineConnector).SetLineBridging();
                            }
                        }
                    }
                }
                Size dsize = element.DesiredSize;
                //if (!(element is DiagramViewGrid))
                try
                {

                    if (element is DiagramViewGrid)
                    {
                        {
                            element.Arrange(new Rect(offsetX, offsetY, this.DesiredSize.Width, this.DesiredSize.Height));
                        }
                    }
                    else
                    {
                        element.Arrange(new Rect(offsetX, offsetY, dsize.Width, dsize.Height));
                    }
                }
                catch
                { }
            }
            disp = this.Dispatcher.BeginInvoke(
                                   new Initialize(DiagramPage.SetBridging), this.dc);

            if (this.dc != null)
            {
                if (this.dc.View != null && (!this.dc.View._LineRoute || this.dc.View.RoutingMode == RoutingMode.Immediate))
                {
                    if (this.dc.View.LineRoutingEnabled)
                    {
                        AStarLineRouter asl = new AStarLineRouter(this.dc.View);
                        this.dc.View.LineRouter = asl;
                    }
                }
            }

            DiagramControl.IsPageLoaded = false;
            this.dc.View.Deleted = false;
            return finalSize;

        }

        static bool fullload = true;
        System.Windows.Threading.DispatcherOperation disp;
        internal delegate void Initialize(DiagramControl dc);

        internal static void SetBridging(DiagramControl dc)
        {
            if (dc != null && dc.Model != null)
            {
                List<UIElement> ordered = (from UIElement item in dc.Model.Connections
                                           orderby Canvas.GetZIndex(item as UIElement)
                                           select item as UIElement).ToList();
                bool atleastone = false;
                foreach (UIElement element in ordered)
                {
                    if (element is LineConnector)
                    {
                        if ((element as LineConnector).invalidateBridging)
                        {
                            atleastone = true;
                        }
                        (element as LineConnector).invalidateBridging = true;
                    }
                }
                if (atleastone)
                {
                    foreach (UIElement element in ordered)
                    {
                        if (element is LineConnector && (element as LineConnector).ConnectorType != ConnectorType.Bezier && (element as LineConnector).ConnectorType != ConnectorType.Arc)
                        {
                            (element as LineConnector).SetLineBridging();
                        }
                    }
                }
                foreach (UIElement element in ordered)
                {
                    if (element is LineConnector)
                    {
                        (element as LineConnector).invalidateBridging = false;
                    }
                }
            }
            if (dc != null && fullload)
            {
                (dc.View.Page as DiagramPage).InvalidateMeasure();
                fullload = false;
            }
        }

        /// <summary>
        /// Called when [units changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPage page = (DiagramPage)d;

            if (page.dc != null && page.dc.Model != null)
            {
                DiagramModel model = page.dc.Model;
                model.MeasurementUnits = (MeasureUnits)e.NewValue;
                //if (model != null && model.m_IsPixelDefultUnit)
                {
                    page.dc.View.NudgeIncrement = MeasureUnitsConverter.Convert(page.dc.View.NudgeIncrement, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    model.HorizontalSpacing = MeasureUnitsConverter.Convert(model.HorizontalSpacing, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    model.VerticalSpacing = MeasureUnitsConverter.Convert(model.VerticalSpacing, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    model.SpaceBetweenSubTrees = MeasureUnitsConverter.Convert(model.SpaceBetweenSubTrees, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    page.dc.View.SnapOffsetX = MeasureUnitsConverter.Convert(page.dc.View.SnapOffsetX, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    page.dc.View.SnapOffsetY = MeasureUnitsConverter.Convert(page.dc.View.SnapOffsetY, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    page.setGridOffsets(MeasureUnitsConverter.Convert(page.GridHorizontalOffset, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue), GridOffsets.Horizontal);
                    page.setGridOffsets(MeasureUnitsConverter.Convert(page.GridVerticalOffset, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue), GridOffsets.Vertical);
                }
                //model.m_IsPixelDefultUnit = true;
            }

            page.IsUnitChanged = true;
            page.OldUnit = page.temp;
            if (page.isexe)
            {
                page.isnotpixelwh = true;
                page.isnotpixeloffset = true;
                page.Unitconverted = true;
                if (page.OldUnit != page.MeasurementUnits)
                {
                    page.munitchanged = true;
                }
            }

            page.isexe = true;
            page.temp = page.MeasurementUnits;
            page.UpdateNodeLayout();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseWheel"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        private void DiagramPage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //bool wheeldown = false;

            //if (this.CurrentMinY < 0)
            //{
            //    if (!this.dc.View.IsJustWheeled)
            //    {
            //        this.dc.View.IsJustWheeled = true;
            //        this.Dragtop += this.MinTop;
            //    }
            //}

            //if (Keyboard.Modifiers == ModifierKeys.Control)
            //{
            //    if (dview.IsZoomEnabled)
            //    {
            //        if (e.Delta > 0)
            //        {
            //            dc.ZoomIn.Execute(dview); e.Handled = true;
            //        }
            //        else
            //        {
            //            dc.ZoomOut.Execute(dview); e.Handled = true;
            //        }
            //    }
            //}

            //if (Keyboard.Modifiers != ModifierKeys.Control)
            //{
            //    this.s = Math.Min(48, Math.Abs(this.dc.View.Scrollviewer.VerticalOffset));
            //    double minus = Math.Abs(this.Dragtop) - (2 * this.s);
            //    if (e.Delta > 0)
            //    {
            //        this.oncezero = false;
            //        if (this.dc.View.Scrollviewer.VerticalOffset != 0)
            //        {
            //            this.scrollcount--;
            //            if (this.CurrentMinY < 0 && !wheeldown)
            //            {
            //                this.dc.View.Y -= (this.s + this.Dragtop + minus + this.dc.View.PanConstant) / this.dc.View.CurrentZoom;
            //            }
            //            else
            //            {
            //                this.dc.View.Y += (this.s + this.dc.View.PanConstant) / this.dc.View.CurrentZoom;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        this.once = false;
            //        if (this.dc.View.Scrollviewer.VerticalOffset < this.dc.View.Scrollviewer.ScrollableHeight)
            //        {
            //            this.scrollcount++;
            //            this.max = Math.Min(Math.Abs(this.dc.View.Scrollviewer.ScrollableHeight - this.dc.View.Scrollviewer.VerticalOffset), 48);
            //            wheeldown = true;
            //            this.dc.View.Y -= (this.max + this.dc.View.PanConstant) / this.dc.View.CurrentZoom;
            //        }
            //    }

            //    if (this.dc.View.Scrollviewer.VerticalOffset == this.dc.View.Scrollviewer.ScrollableHeight && e.Delta > 0)
            //    {
            //        if (!this.once)
            //        {
            //            this.once = true;
            //            this.dc.View.ViewGridOrigin = new Point(this.dc.View.ViewGridOrigin.X, this.dc.View.Y * this.dc.View.CurrentZoom);
            //        }
            //    }

            //    if (this.dc.View.Scrollviewer.VerticalOffset == 0 && e.Delta < 0)
            //    {
            //        if (!this.oncezero)
            //        {
            //            this.oncezero = true;
            //            this.dc.View.ViewGridOrigin = new Point(this.dc.View.ViewGridOrigin.X, this.dc.View.Y * this.dc.View.CurrentZoom);
            //        }
            //    }

            //    if (this.dc.View.Scrollviewer.VerticalOffset != this.dc.View.Scrollviewer.ScrollableHeight && this.dc.View.Scrollviewer.VerticalOffset != 0)
            //    {
            //        this.dc.View.ViewGridOrigin = new Point(this.dc.View.ViewGridOrigin.X, this.dc.View.Y * this.dc.View.CurrentZoom);
            //    }
            //}
        }

        internal void DropLine(LineConnector lineconector, Point position, DiagramControl diagctrl)
        {
            ConnectorBase line = null;
            PreviewConnectorDropEventRoutedEventArgs linedropnewEventArgs = new PreviewConnectorDropEventRoutedEventArgs();
            dview.OnPreviewConnectorDrop(line, linedropnewEventArgs);
            if (linedropnewEventArgs.Cancel == false)
            {
                this.IsConnectorDropped = true;
                line = lineconector;
                line.MeasurementUnit = this.MeasurementUnits;
                line.DropPoint = position;
                line.PxStartPointPosition = new Point(line.DropPoint.X - 25, line.DropPoint.Y - 25);
                line.PxEndPointPosition = new Point(line.DropPoint.X + 25, line.DropPoint.Y + 25);
                line.UpdateConnectorPathGeometry();
                diagctrl.Model.Connections.Add(line);
                this.SelectionList.Clear();
                this.SelectionList.Add(line);
                line.Focus();
                ConnectorDroppedRoutedEventArgs newEventArgs = new ConnectorDroppedRoutedEventArgs(line);
                dview.OnConnectorDrop(line, newEventArgs);
            }
        }
        /// <summary>
        /// Drops the line.
        /// </summary>
        /// <param name="connectortype">The connector type.</param>
        /// <param name="position">The position.</param>
        /// <param name="diagctrl">The diagram control object.</param>
        internal void DropLine(ConnectorType connectortype, Point position, DiagramControl diagctrl)
        {
            ConnectorBase line = null;
            PreviewConnectorDropEventRoutedEventArgs linedropnewEventArgs = new PreviewConnectorDropEventRoutedEventArgs();
            dview.OnPreviewConnectorDrop(line, linedropnewEventArgs);
            if (linedropnewEventArgs.Cancel == false)
            {
                this.IsConnectorDropped = true;
                line = new LineConnector();
                line.MeasurementUnit = this.MeasurementUnits;
                line.ConnectorType = connectortype;
                line.DropPoint = position;
                line.PxStartPointPosition = new Point(line.DropPoint.X - 25, line.DropPoint.Y - 25);
                line.PxEndPointPosition = new Point(line.DropPoint.X + 25, line.DropPoint.Y + 25);
                line.UpdateConnectorPathGeometry();
                diagctrl.Model.Connections.Add(line);
                this.SelectionList.Clear();
                this.SelectionList.Add(line);
                line.Focus();
                ConnectorDroppedRoutedEventArgs newEventArgs = new ConnectorDroppedRoutedEventArgs(line);
                dview.OnConnectorDrop(line, newEventArgs);
            }
        }

        internal void DiagramPage_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    DiagramView.MoveUp(this.dview);
                    break;
                case Key.Down:
                    DiagramView.MoveDown(this.dview);
                    break;
                case Key.Left:
                    DiagramView.MoveLeft(this.dview);
                    break;
                case Key.Right:
                    DiagramView.MoveRight(this.dview);
                    break;
                case Key.Delete:
                    dc.Delete.Execute(this.dview);
                    (this.dc.View.Page as DiagramPage).InvalidateMeasure();
                    break;
                case Key.Escape:
                    if (dview.IsNodeDragCancel == true)
                    {
                        foreach (ICommon n in dc.View.SelectionList.OfType<ICommon>())
                        {
                            if (n is Group)
                            {
                                (n as Group).DragCancel = true;
                                (n as Group).OffsetX = (n as Group).x;
                                (n as Group).OffsetY = (n as Group).y;
                                foreach (INodeGroup n1 in (n as Group).NodeChildren)
                                {
                                    if (n1 is Node)
                                    {
                                        (n1 as Node).DragCancel = true;
                                        (n1 as Node).OffsetY = (n1 as Node).y;
                                        (n1 as Node).OffsetX = (n1 as Node).x;
                                    }
                                    else
                                    {
                                        (n1 as LineConnector).StartPointPosition = (n1 as LineConnector).sp;
                                        (n1 as LineConnector).EndPointPosition = (n1 as LineConnector).ep;
                                        (n1 as LineConnector).UpdateConnectorPathGeometry();
                                    }
                                }
                                (n as Group).ReleaseMouseCapture();
                            }
                            if (n is Node)
                            {
                                (n as Node).DragCancel = true;
                                (n as Node).OffsetY = (n as Node).y;
                                (n as Node).OffsetX = (n as Node).x;
                                (n as Node).ReleaseMouseCapture();
                            }
                            if ((n is LineConnector) && dc.View.SelectionList.Count > 1)
                            {
                                (n as LineConnector).StartPointPosition = (n as LineConnector).sp;
                                (n as LineConnector).EndPointPosition = (n as LineConnector).ep;
                                (n as LineConnector).UpdateConnectorPathGeometry();
                            }
                            (this.dc.View.Page as DiagramPage).InvalidateMeasure();
                        }
                    }
                    if (dview.IsConnectorDragCancel == true && dc.View.SelectionList.Count == 1)
                    {
                        foreach (LineConnector line in this.dview.SelectionList.OfType<LineConnector>())
                        {
                            line.DragCancel = true;
                            line.ReleaseMouseCapture();
                        }
                    }
                    (this.dc.View.Page as DiagramPage).InvalidateMeasure();
                    break;
                default:
                    break;
            }

            if (this.dview.SelectionList.Count > 0)
            {
                // e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        /// <summary>
        /// Is called when the diagram page gets loaded.
        /// </summary>
        /// <param name="sender">Diagram page</param>
        /// <param name="e">Event args</param>
        private void DiagramPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.diagctrl = GetDiagramControl(this);
            this.dview = this.diagctrl.View;
            this.temp = this.MeasurementUnits;
            // page = this.dview.Page as DiagramPage;
        }


        /// <summary>
        /// Handles the MouseRightButtonDown event of the DiagramPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void DiagramPage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == this)
            {
                if(dview.ClearSelectionOnRightClick==true)
                this.SelectionList.Clear();
                // this.startPoint = e.GetPosition(this.dview);

            }

        }

        /// <summary>
        /// Is called when mouse left button is down on the diagram page.
        /// </summary>
        /// <param name="sender">Diagram page</param>
        /// <param name="e">Event args</param>
        private void DiagramPage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //if (e.OriginalSource == this && dview.IsPageEditable)
            //{
            //    this.SelectionList.Clear();
            //    this.startPoint = e.GetPosition(this.dview);
            //    this.spoint = e.GetPosition(this);
            //}

        }

        /// <summary>
        /// Is called when mouse left button is up on the diagram page.
        /// </summary>
        /// 
        internal void SelectionRemove()
        {
            if (this.startPoint.HasValue)
            {
                this.startPoint = null;
            }

            if (this.dview.SelectionCanvas.Children.Count > 0)
            {
                this.dview.SelectionCanvas.Children.Remove(this.dview.SelectionCanvas.Children.ElementAt(this.dview.SelectionCanvas.Children.Count() - 1));
            }
            CollectionExt.Cleared = false;
        }
        private void DiagramPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            if (this.startPoint.HasValue)
            {
                this.startPoint = null;
            }

            if (this.dview.SelectionCanvas.Children.Count > 0)
            {
                this.dview.SelectionCanvas.Children.Remove(this.dview.SelectionCanvas.Children.ElementAt(this.dview.SelectionCanvas.Children.Count() - 1));
            }
            CollectionExt.Cleared = false;
            foreach (ICommon n in dc.View.SelectionList.OfType<ICommon>())
            {
                if (n is Group)
                {
                    (n as Group).DragCancel = false;
                    foreach (INodeGroup n1 in (n as Group).NodeChildren)
                    {
                        if (n1 is Node)
                        {
                            (n1 as Node).DragCancel = false;
                        }
                    }
                }
                else if (n is Node)
                {
                    (n as Node).DragCancel = false;
                }
            }
        }

        /// <summary>
        /// Is called when mouse is moved on the diagram page.
        /// </summary>
        /// <param name="sender">Diagram page</param>
        /// <param name="e">Event args</param>
        internal void DiagramPage_MouseMove(object sender, MouseEventArgs e)
        {


        }

        internal void WholeSelection(Point MousePoint)
        {
            if (this.dview.IsPageEditable == true)
            {

                if (this.startPoint.HasValue)
                {
                    if (this.dview.SelectionCanvas.Children.Count > 0)
                    {
                        this.dview.SelectionCanvas.Children.Remove(this.dview.SelectionCanvas.Children.ElementAt(this.dview.SelectionCanvas.Children.Count() - 1));
                    }
                    this.CaptureMouse();
                    this.SelectionList.Clear();
                    this.endPoint = MousePoint;
                    Rect selectedArea = new Rect(this.startPoint.Value, this.endPoint);
                    Border selectionadorner = new Border();
                    selectionadorner.Background = new SolidColorBrush(Colors.Gray);
                    selectionadorner.BorderBrush = new SolidColorBrush(Colors.Black);
                    selectionadorner.BorderThickness = new Thickness(2);
                    Canvas.SetLeft(selectionadorner, selectedArea.X);
                    Canvas.SetTop(selectionadorner, selectedArea.Y);
                    selectionadorner.Width = selectedArea.Width;
                    selectionadorner.Height = selectedArea.Height;
                    selectionadorner.Opacity = .5;
                    this.dview.SelectionCanvas.Children.Add(selectionadorner);
                    foreach (Control item in this.Children)
                    {
                        if (item is Node)
                        {
                            Point p = new Point();
                            p = MousePoint;
                            selectedArea = new Rect(spoint, p);
                            Rect itemRect = new Rect(0, 0, (item as Node).ActualWidth, (item as Node).ActualHeight);
                            Rect itemBounds = item.TransformToVisual(this.dview.ScrollGrid as FourQuadrantPanel).TransformBounds(itemRect);
                            // Rect itemRect = new Rect((item as Node).PxOffsetX, (item as Node).PxOffsetY, item.ActualWidth, item.ActualHeight);
                            // if (selectedArea.Contains(new Point(itemRect.X - this.dview.Scrollviewer.HorizontalOffset, itemRect.Y - this.dview.Scrollviewer.VerticalOffset)) && selectedArea.Contains(new Point(itemRect.X + itemRect.Width - this.dview.Scrollviewer.HorizontalOffset, itemRect.Y + itemRect.Height - this.dview.Scrollviewer.VerticalOffset)))
                            if (selectedArea.Contains(new Point(itemBounds.X, itemBounds.Y)) && selectedArea.Contains(new Point(itemBounds.Right, itemBounds.Bottom)))
                            {
                                if ((item as Node).AllowSelect)
                                {
                                    this.SelectionList.Add(item);
                                }
                            }

                        }
                        else if (item is LineConnector)
                        {
                            if (selectedArea.Contains((item as LineConnector).PxStartPointPosition) && selectedArea.Contains((item as LineConnector).PxEndPointPosition) && !(item as LineConnector).IsSelected)
                            {
                                this.SelectionList.Add(item);
                            }
                        }
                    }
                }


            }
        }

        /// <summary>
        /// Updates the node offset positions and size with respect to the current unit.
        /// </summary>
        private void UpdateNodeLayout()
        {
            DiagramPage.Munits = this.MeasurementUnits;
            foreach (UIElement node in this.Children)
            {
                if (this.isnotpixeloffset)
                {
                    if (node is Node)
                    {


                    }

                    if (this.notfirstexe)
                    {
                        if (!this.exeonce)
                        {
                            if (node is Node)
                            {

                            }
                        }
                    }

                    this.notfirstexe = true;
                }

                if (!this.isnotpixelwh || !this.munitchanged)
                {
                    if (node is Node)
                    {

                    }
                    else
                    {
                        //this.munitchanged = false;
                    }
                }
            }

            this.exeonce = false;
        }

        #region Class variables

        #endregion

        #region Initialization

        #endregion

        #region Class Override

        #endregion

        #region  Properties

        #endregion

        #region Internal Properties

        #endregion

        #region DPs

        #endregion

        #region Events

        #endregion

        #region Implementation

        #endregion

        #region IDiagramPanel Members

        #endregion

        #region INotifyPropertyChanged Members

        #endregion

        #region Private Functions
        private void setGridOffsets(double value, GridOffsets offset)
        {
            double pixval;
            page = this;
            if (page != null)
            {
                pixval = MeasureUnitsConverter.ToPixels(value, page.MeasurementUnits);
                if (pixval < 1)
                {
                    value = MeasureUnitsConverter.FromPixels(1, page.MeasurementUnits);
                }
                if (offset == GridOffsets.Horizontal)
                {
                    this.horoffset = value;
                }
                else if (offset == GridOffsets.Vertical)
                {
                    this.vertoffset = value;
                }
            }
        }

        private void updateGridLines(GridOffsets offset)
        {
            if (this != null && this.dview != null && this.dview.ScrollGrid != null && this.dview.ScrollGrid.Children[1] is DiagramViewGrid)
            {
                if (offset == GridOffsets.Horizontal&&dview.ShowHorizontalGridLine==true)
                {
                    (dview.ScrollGrid.Children[1] as DiagramViewGrid).RearrrangeHorGridLines();
                }
                else if (offset == GridOffsets.Vertical&&dview.ShowVerticalGridLine == true)
                {
                    (dview.ScrollGrid.Children[1] as DiagramViewGrid).RearrrangeVerGridLines();
                }
            }
        }
        #endregion

        #region Virtualization Members

        internal void AddingChildren(UIElement Child)
        {
            this.AddInternalChild(Child);
        }
        internal void RemovingChildren(UIElement Child)
        {
            if (Child is Node && (Child as Node).IsInternallyLoaded)
            {
                (Child as Node).IsInternallyLoaded = false;
                this.Children.Remove(Child);
            }
            else if (Child is LineConnector && (Child as LineConnector).IsInternallyLoaded)
            {
                (Child as LineConnector).IsInternallyLoaded = false;
                this.Children.Remove(Child);
            }
        }
        protected void AddInternalChild(UIElement child)
        {

            if (child is Node && !(child as Node).IsInternallyLoaded)
            {
                (child as Node).IsInternallyLoaded = true;
                this.Children.Add(child);
            }
            else if (child is LineConnector && !(child as LineConnector).IsInternallyLoaded)
            {
                (child as LineConnector).IsInternallyLoaded = true;
                this.Children.Add(child);
            }
        }
        protected void RemoveInternalChild(UIElement child)
        {


        }
        #endregion
    }
}
