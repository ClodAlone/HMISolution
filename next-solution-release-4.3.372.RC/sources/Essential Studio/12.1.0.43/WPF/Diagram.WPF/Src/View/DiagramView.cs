// <copyright file="DaigramView.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Windows.Themes;
using System.Windows.Data;
using System.Linq;
using System.Windows.Media.Effects;
using Syncfusion.Windows.Shared;
using System.Windows.Threading;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Diagram View.
    /// <para>The view obtains data from the model and presents them to the user. It typically manages the overall layout of the data obtained from model.
    /// Apart from presenting the data, view also handles navigation between the items, and some aspects of item selection. 
    /// The views also implements basic user interface features, such as rulers, and drag and drop. 
    /// It handles the events, which occur on the objects, obtained from the model. 
    /// Command mechanism is also implemented by the view.
    /// </para>
    /// </summary>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DiagramView"/> in XAML.
    /// <code language="XAML">
    /// &lt;Window x:Class="RulersAndUnits.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
    ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
    ///  Icon="Images/App.ico" &gt;
    ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
    ///                                IsSymbolPaletteEnabled="True" 
    ///                                Background="WhiteSmoke"&gt;
    ///           &lt;syncfusion:DiagramControl.View&gt;
    ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
    ///                                        Background="LightGray"  
    ///                                        Bounds="0,0,12,12"  
    ///                                        ShowHorizontalGridLine="False" 
    ///                                        ShowVerticalGridLine="False"
    ///                                        Name="diagramView"  &gt;
    ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
    ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
    ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
    ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
    ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
    ///       &lt;/syncfusion:DiagramView&gt;
    ///    &lt;/syncfusion:DiagramControl.View&gt;
    /// &lt;/syncfusion:DiagramControl&gt;
    /// &lt;/Window&gt;
    /// </code>
    /// <para/>The following example shows how to create a <see cref="DiagramView"/> in C#.
    /// <code language="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using System.ComponentModel;
    /// using Syncfusion.Core;
    /// using Syncfusion.Windows.Diagram;
    /// namespace WpfApplication1
    /// {
    /// public partial class Window1 : Window
    /// {
    ///    public DiagramControl Control;
    ///    public DiagramModel Model;
    ///    public DiagramView View;
    ///    public Window1 ()
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
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    /// <seealso cref="HorizontalRuler"/>
    /// <seealso cref="VerticalRuler"/>
    /// <seealso cref="DiagramPage"/>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public partial class DiagramView : ContentControl, IView, INotifyPropertyChanged
    {

        #region Drawing Tools

        private System.Windows.Shapes.Path temppath;
        private PathGeometry tempbezier;
        private PolyLineSegment temppoly;
        private double[] Dimensions = new double[2];
        private static double oldx;
        private static double oldy;
        private BezierSegment bs;
        private Rect rt = new Rect();
        private System.Windows.Shapes.Path draw_ell = null;
        private PathFigure pf;
        ArcSegment ac;
        private Point point3;
        private Point ortho1 = new Point();
        private Point ortho2 = new Point();
        private Point ortho3 = new Point();
        internal bool DrawAllow = true;
        internal bool m_DragandMove = true;
        private bool m_drawpoly = true;
        private Node m_LabelNode;    
        Point DrawStrat = new Point(oldx, oldy);
        internal List<string> undostak = new List<string>();
        # endregion


        #region Class fields

        public Brush PageBackground
        {
            get { return (Brush)GetValue(PageBackgroundProperty); }
            set { SetValue(PageBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageBackgroundProperty =
            DependencyProperty.Register("PageBackground", typeof(Brush), typeof(DiagramView));

        public static readonly DependencyProperty BackgroundEffectProperty = DependencyProperty.Register("BackgroundEffect", typeof(Effect), typeof(DiagramView), new PropertyMetadata(new PropertyChangedCallback(OnBackgroundEffectChanged)));

        //private Popup pop;
        internal Rectangle SymbolPaletteItemPreview;

        /// <summary>
        /// Used to store copypastemanager.
        /// </summary>
        internal CopyPasteManager CPManager;

        /// <summary>
        /// Used to store the cursor used.
        /// </summary>
        private Cursor m_cursor;

        /// <summary>
        /// Used to store the scrollviewer instance
        /// </summary>
        private ScrollViewer scrollview;

        /// <summary>
        /// Used to store the IsPageEditable property
        /// </summary>
        private static bool ispagedit = true;

        /// <summary>
        ///  Used to store the other event's state
        /// </summary>
        private static bool otherevents = false;

        /// <summary>
        /// Used to store the View grid state
        /// </summary>
        private static bool isvieworiginchanged = false;

        /// <summary>
        /// Used to store the node drag information.
        /// </summary>
        private bool isdrag = false;

        /// <summary>
        /// Used to store the screen start point
        /// </summary>
        private Point screenStartPoint = new Point(0, 0);

        /// <summary>
        /// Used to store  the scale transform
        /// </summary>
        private ScaleTransform zoomTransform;

        /// <summary>
        /// Checks if node was deleted.
        /// </summary>
        private bool nodedel = false;

        /// <summary>
        /// Checks if line was deleted.
        /// </summary>
        private bool linedel = false;

        /// <summary>
        /// Checks if deletion was done.
        /// </summary>
        private bool isdupdel;

        /// <summary>
        /// Used to store internal edges collection.
        /// </summary>
        private CollectionExt intedges = new CollectionExt();

        internal FourQuadrantPanel ScrollGrid;

        /// <summary>
        /// Used to store the origin point
        /// </summary>
        private System.Drawing.Point mOrigin;

        /// <summary>
        /// Used to store ShowRulers property
        /// </summary>
        private bool showRuler;

        /// <summary>
        /// Used to store ShowPage property
        /// </summary>
        private bool showPage;

        /// <summary>
        /// Used to store the model
        /// </summary>
        private IModel mModel;

     
        /// <summary>
        /// Used to store the DiagramViewGrid.
        /// </summary>
        internal DiagramViewGrid mViewGrid;

        /// <summary>
        /// Used to store the OnReset command value.
        /// </summary>
        private bool onReset = false;

        /// <summary>
        ///  Used to store the view grid
        /// </summary>
        internal Grid viewgrid;

        /// <summary>
        ///  Used to store horizontal tickbar
        /// </summary>
        private TickBar hortickbar;

        /// <summary>
        ///  Used to store vertical tickbar
        /// </summary>
        private TickBar vertickbar;

        /// <summary>
        ///  Used to store diagram Control instance.
        /// </summary>
        internal DiagramControl dc;

        /// <summary>
        /// Used to store boolean value on zoom factor becoming less than one.
        /// </summary>
        private bool lessthanone = true;

        /// <summary>
        /// Used to store boolean value on zoom factor becoming more than one.
        /// </summary>
        private bool morethanone = true;

        /// <summary>
        /// Used to store boolean value on executing zoom for the first time.
        /// </summary>
        private bool firstzoom = false;

        /// <summary>
        /// Used to store a double value
        /// </summary>
        private int i = 1;

        /// <summary>
        /// Used to store a double value
        /// </summary>
        private int j = 1;

        /// <summary>
        /// Used to store count value
        /// </summary>
        private int count = 1;

        /// <summary>
        /// Used to check if node is dragged.
        /// </summary>
        private bool isdragged = false;

        /// <summary>
        /// Used to store pixel interval
        /// </summary>
        private double pixelvalue = 50;

        /// <summary>
        /// Used to store boolean value on executing level one once
        /// </summary>
        private bool leveloneexe = false;

        /// <summary>
        /// Used to store boolean value on executing zoom once
        /// </summary>
        private bool zoominexe = false;

        /// <summary>
        /// Used to store the groups in the page.
        /// </summary>
        private CollectionExt m_groups = new CollectionExt();

        /// <summary>
        /// Checks if MeasureOveride  was called.
        /// </summary>
        private bool measured = false;

        /// <summary>
        /// Checks if Undo command was executed.
        /// </summary>
        internal bool undo = false;

        /// <summary>
        /// Checks if Redo command was executed.
        /// </summary>
        internal bool redo = false;

        /// <summary>
        /// Checks if <see cref="Node"/> was dragged.
        /// </summary>
        private bool dragged = false;

        /// <summary>
        /// Checks if <see cref="Node"/> was resized.
        /// </summary>
        private bool resized = false;

        /// <summary>
        /// Checks if <see cref="Node"/> was resized as a resule of undo operation.
        /// </summary>
        private bool undoresize = false;

        /// <summary>
        /// Checks if <see cref="Node"/> was resized as a resule of redo operation.
        /// </summary>
        private bool redoresize = false;

        /// <summary>
        /// Checks if Delete command was executed.
        /// </summary>
        private bool deletecommandexe = false;

        internal NodeConnectorAdorner m_connectionadorner;

        /// <summary>
        /// Used to store the delete count.
        /// </summary>
        private int delcount = 0;

        /// <summary>
        /// Used to store the node drag count.
        /// </summary>
        private int nodedragcount = 1;

        /// <summary>
        /// Used to store the values in the selection list.
        /// </summary>
        private ObservableCollection<ICommon> oldselectionlist = new ObservableCollection<ICommon>();

        /// <summary>
        /// Used to store the count of the <see cref="Node"/> been resized.
        /// </summary>
        private int resizecount = 0;

        /// <summary>
        /// Used to store the count of the <see cref="Node"/> been rotated.
        /// </summary>
        private int rotatecount = 1;

        /// <summary>
        /// Refers to the undo stack.
        /// </summary>
        private Stack<object> undocommandstack = new Stack<object>();

        /// <summary>
        /// Refers to the redo stack.
        /// </summary>
        private Stack<object> redocommandstack = new Stack<object>();

        /// <summary>
        /// Checks if automatic layout is used.
        /// </summary>
        private bool layout = false;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="DiagramView"/> class.
        /// </summary>
        static DiagramView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramView), new FrameworkPropertyMetadata(typeof(DiagramView)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramView"/> class.
        /// </summary>        

        public DiagramView()
        {
            this.Loaded += new RoutedEventHandler(DiagramView_Loaded);
            this.Page = new DiagramPage();
            (this.Page as DiagramPage).dview = this;
            this.Page.Loaded += new RoutedEventHandler(Page_Loaded);
            this.Page.Focusable = true;
            FocusManager.SetIsFocusScope(this.Page, true);
            this.Page.Focus();
            DiagramCommandManager d = new DiagramCommandManager(this);
            this.AllowDrop = true;
            this.Unloaded += new RoutedEventHandler(DiagramView_Unloaded);
            if (this.CPManager == null)
            {
                CPManager = new CopyPasteManager(this);
            }
            this.AddHandler(Control.MouseDownEvent, new MouseButtonEventHandler(DiagramView_MouseDown), true);
           // OnMouseMove
            InitDiagramProperties();
            this.SizeChanged += new SizeChangedEventHandler(DiagramView_SizeChanged);

            this.DateTimeSettings = new DateTimeSettings();//new TimeSpan(1, 0, 0, 0, 0), 50);
            this.AddHandler(Control.DropEvent, new DragEventHandler(DiagramView_Drop), true);
        }
       

        //Summary
        //SymbolPaletteItemPreview is closed when the SymbolPaletteItem is dropped onto the Collection Node
        void DiagramView_Drop(object sender, DragEventArgs e)
        {
            if (SymbolPaletteItemPreview != null)
            {
                SymbolPaletteItemPreview.Visibility = Visibility.Collapsed;
            }
        }

        void _contentHolder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Border)
            {
                if ((e.OriginalSource as Border).TemplatedParent is OverviewContentHolder)
                {
                    if (e.LeftButton == MouseButtonState.Pressed || ClearSelectionOnRightClick == true)
                    {

                        if ((e.OriginalSource as Border).TemplatedParent is OverviewContentHolder || WithinPageAndView(e.OriginalSource))
                        {
                            if (!IsPanEnabled)
                                this.startPoint = new Point?(e.GetPosition(this));
                            SelectionList.Clear();
                            Focus();
                            e.Handled = true;
                        }
                        else if (Scrollviewer.Equals(e.OriginalSource))
                        {
                            this.startPoint = new Point?(e.GetPosition(this));
                            SelectionList.Clear();
                        }

                    }
                }
            }

        }

        //void pop_DragOver(object sender, DragEventArgs e)
        //{
        //    if (dc.View.IsPageEditable)
        //    {
        //        if (dc.SymbolPalette.ShowPreview)
        //        {
        //            try
        //            {
        //                pop.HorizontalOffset = e.GetPosition(this).X - 25;
        //                pop.VerticalOffset = e.GetPosition(this).Y - 25;
        //            }
        //            catch
        //            { }
        //        }
        //    } 
        //}

        internal  Size returnViewGridSize(OverviewContentHolder sscroll)
        {
            double view_height = sscroll.ViewportHeight / this.CurrentZoom + (Page as DiagramPage).Top / this.CurrentZoom;
            double view_width = sscroll.ViewportWidth / this.CurrentZoom + (Page as DiagramPage).Left /this.CurrentZoom;
            double Extnd_width = sscroll.ExtentWidth + (Page as DiagramPage).Left;
            double Extnd_height = sscroll.ExtentHeight + (Page as DiagramPage).Top;
            return new Size(Math.Max(view_width, Extnd_width), Math.Max(view_height, Extnd_height));
        }
        void DiagramView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size view_Size=returnViewGridSize(_contentHolder);
            (viewgrid.Children[1] as DiagramViewGrid).Arrange(new Rect(-(Page as DiagramPage).Left, -(Page as DiagramPage).Top, view_Size.Width / this.CurrentZoom, view_Size.Height / this.CurrentZoom));
            UpdateViewGridOrigin();
            if (vertickbar != null)
            {
                vertickbar.InvalidateVisual();
            }

            if (hortickbar != null)
            {
                hortickbar.InvalidateVisual();
            }
            if(ScrollGrid!=null)
            this.ScrollGrid.InvalidateMeasure();
        }

        #region Dirty Flag

        private void InitDiagramProperties()
        {
            List<string> nodePropList = new List<string>()
            {  
               "MinHeight", "MinWidth", "Height", "Width", "ActualHeight", "ActualWidth", "IsLabelEditable", "Label", "LabelVisibility", "LabelHorizontalAlignment", 
               "LabelVerticalAlignment", "HorizontalContentAlignment", "VerticalContentAlignment", "LabelAngle", "Shape", "CustomPathStyle", "Level", "OffsetX", "OffsetY", 
               "Content", "AllowMove", "AllowSelect", "AllowRotate", "AllowResize", "LabelTextTrimming", "LabelForeground", "LabelBackground", "LabelFontStyle", "LabelFontFamily", 
               "LabelTextAlignment", "LabelFontSize", "LabelFontWeight", "LabelTextWrapping", "LabelWidth", "EnableMultilineLabel", 
               "Name", "ZIndex"               
            };
            List<string> lineConnectorPropList = new List<string>()
            {  
               "EnableConnection", "IntermediatePoints", "LabelTemplate", "ConnectionEndSpace", "ConnectorType", "HeadNode", "TailNode", "HeadDecoratorShape", 
               "TailDecoratorShape", "HeadDecoratorStyle", "TailDecoratorStyle", "CustomHeadDecoratorStyle", "CustomTailDecoratorStyle", "LineStyle", 
               "LineBridgingEnabled", "FirstSegmentLength", "LastSegmentLength", "AutoAdjustPoints", "VertexStyle", "DecoratorAdornerStyle", "IsVertexVisible",
               "IsVertexMovable", "Name", "Height", "Width", "StartPointPosition", "EndPointPosition", "Label", "IsLabelEditable", "Label", "LabelVisibility",
               "LabelHorizontalAlignment", "LabelVerticalAlignment", "HorizontalContentAlignment", "VerticalContentAlignment", "LabelAngle", "CustomPathStyle", "Level", 
               "Content", "AllowMove", "AllowSelect", "AllowResize", "LabelTextTrimming", "LabelForeground", "LabelBackground", "LabelFontStyle", "LabelFontFamily", 
               "LabelTextAlignment", "LabelFontSize", "LabelFontWeight", "LabelTextWrapping", "LabelWidth", "EnableMultilineLabel"
            };
            List<string> connectionPortPropList = new List<string>()
            {  
               "Left", "Top", "Node", "Port", "PortShape", "PortStyle", "AllowPortDrag", "ConnectionHeadPort", "ConnectionTailPort", "Name", "Height", "Width"
            };

            AddDiagramProperties(DiagramProperties, typeof(Node), nodePropList);
            AddDiagramProperties(DiagramProperties, typeof(ConnectorBase), lineConnectorPropList);
            AddDiagramProperties(DiagramProperties, typeof(ConnectionPort), connectionPortPropList);
        }

        private void AddDiagramProperties(ObservableCollection<DiagramProperty> dp, Type type, List<string> properties)
        {
            foreach (string s in properties)
            {
                dp.Add(new DiagramProperty() { ObjectType = type, PropertyName = s });
            }
        }

        /// <summary>
        /// It is used to store the Node, Lineconnector types and Properties.
        /// </summary>
        public ObservableCollection<DiagramProperty> DiagramProperties = new ObservableCollection<DiagramProperty>();

        /// <summary>
        /// Identifies the ViewGridOrigin property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPageSavedProperty =
            DependencyProperty.Register("IsPageSaved", typeof(bool), typeof(DiagramView), new PropertyMetadata(true));

        /// <summary>
        ///  Gets or sets a value indicating whether [Is Page Saved].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [Is Page Saved]; otherwise, <c>false</c>.
        /// </value>
        public bool IsPageSaved
        {
            get
            {
                return (bool)GetValue(IsPageSavedProperty);
            }

            set
            {
                SetValue(IsPageSavedProperty, value);
            }
        }

        #endregion

        void Scrollviewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            UpdateViewGridOrigin();
        }

        //private static void SetUnitBinding(string PropName, DependencyProperty dependencyProperty, FrameworkElement src)
        //{
        //    MultiBinding UnitPixelBind = new MultiBinding();
        //    UnitPixelBind.Mode = BindingMode.TwoWay;
        //    UnitPixelBind.Converter = new PixelUnitConverter();
        //    Binding UnitValue = new Binding(PropName);
        //    UnitValue.Mode = BindingMode.TwoWay;
        //    UnitValue.Source = src;
        //    UnitPixelBind.Bindings.Add(UnitValue);
        //    Binding MeasureValue = new Binding();
        //    MeasureValue.Mode = BindingMode.TwoWay;
        //    MeasureValue.Source = src;
        //    UnitPixelBind.Bindings.Add(MeasureValue);
        //    src.SetBinding(dependencyProperty, UnitPixelBind);
        //}

        void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
        }
       internal  Binding BoundaryBackground;
       internal OverviewContentHolder _contentHolder;
        /// <summary>
        /// Invoked when the Diagram View is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DiagramView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_contentHolder != null)
            {
                this._contentHolder.MouseLeftButtonDown += new MouseButtonEventHandler(_contentHolder_MouseLeftButtonDown);
                this._contentHolder.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(_contentHolder_PreviewMouseLeftButtonDown);
                InternalBinding("Scale", CurrentZoomProperty, BindingMode.TwoWay);
                InternalBinding("ZoomFactor", ZoomFactorProperty, BindingMode.TwoWay);
                InternalBinding("EnableFitToPage", EnableFitToPageProperty, BindingMode.TwoWay);
                _contentHolder.InvalidateMeasure();
                if (EnableFitToPage)
                    _contentHolder.EnableFitToPage = this.EnableFitToPage;
            }
            Binding MarginBinding = new Binding("PageMargin");
            MarginBinding.Source = this;
            (this.Page as DiagramPage).SetBinding(DiagramPage.MarginProperty, MarginBinding);
            BoundaryBackground = new Binding("OffPageBackground");
            BoundaryBackground.Source = this;
            this.SelectionList.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectionList_CollectionChanged);
            dc = DiagramPage.GetDiagramControl(this);
            if (dc.IsUnloaded)
            {
                Scrollviewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            }

            if (this.HorizontalRuler != null)
            {
                try
                {
                    Border b1 = (Border)VisualTreeHelper.GetChild(this.HorizontalRuler, 0);
                    Grid g2 = (Grid)VisualTreeHelper.GetChild(b1, 0);
                    Grid g3 = (Grid)VisualTreeHelper.GetChild(g2, 0);
                    hortickbar = (TickBar)VisualTreeHelper.GetChild(g3, 0);
                }
                catch
                {
                }
            }

            if (this.VerticalRuler != null)
            {
                try
                {
                    Border b = (Border)VisualTreeHelper.GetChild(this.VerticalRuler, 0);
                    Grid g = (Grid)VisualTreeHelper.GetChild(b, 0);
                    Grid g1 = (Grid)VisualTreeHelper.GetChild(g, 0);
                    vertickbar = (TickBar)VisualTreeHelper.GetChild(g1, 0);
                }
                catch
                {
                }
            }
            (this.Page as DiagramPage).Viewstate = new ViewState();
        }

        void _contentHolder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void InternalBinding(string sourceProp, DependencyProperty dpProp, BindingMode mode)
        {
            Binding bin = new Binding(sourceProp);
            bin.Mode = mode;
            bin.Source = _contentHolder;
            SetBinding(dpProp, bin);
        }
        void SelectionList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Selection();
        }

        /// <summary>
        /// Handles the Unloaded event of the DiagramView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DiagramView_Unloaded(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Connector Additional Points Removing RoutedEvent
        /// </summary>
        /// <remarks></remarks>
        public static readonly RoutedEvent ConnectorCanRemoveSegmentsEvent = EventManager.RegisterRoutedEvent(
        "ConnectorCanRemoveSegments", RoutingStrategy.Bubble, typeof(ConnectorCanRemoveSegmentsEventHandler), typeof(DiagramView));

        /// <summary>
        /// Occurs when [Connector wants to remove existing Intermediate points]. 
        /// </summary>
        /// <remarks></remarks>
        public event ConnectorCanRemoveSegmentsEventHandler ConnectorCanRemoveSegments
        {
            add { AddHandler(ConnectorCanRemoveSegmentsEvent, value); }
            remove { RemoveHandler(ConnectorCanRemoveSegmentsEvent, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent LineMovedEvent = EventManager.RegisterRoutedEvent(
        "LineMoved", RoutingStrategy.Bubble, typeof(LineNudgeEventHandler), typeof(DiagramView));

        /// <summary>
        /// Occurs when [line moved].
        /// </summary>
        public event LineNudgeEventHandler LineMoved
        {
            add { AddHandler(LineMovedEvent, value); }
            remove { RemoveHandler(LineMovedEvent, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent NodeMovedEvent = EventManager.RegisterRoutedEvent(
        "NodeMoved", RoutingStrategy.Bubble, typeof(NodeNudgeEventHandler), typeof(DiagramView));

        /// <summary>
        /// Occurs when [node moved].
        /// </summary>
        public event NodeNudgeEventHandler NodeMoved
        {
            add { AddHandler(NodeMovedEvent, value); }
            remove { RemoveHandler(NodeMovedEvent, value); }
        }

        /// <summary>
        ///  NodeSelected Routed event. Is raised when the node is deleted.
        /// </summary>
        public static readonly RoutedEvent ObjectDrawnEvent = EventManager.RegisterRoutedEvent(
        "ObjectDrawn", RoutingStrategy.Bubble, typeof(DrawingToolEventHandler), typeof(DiagramView));

        /// <summary>
        /// NodeSelected Event Handler
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event DrawingToolEventHandler ObjectDrawn
        {
            add { AddHandler(ObjectDrawnEvent, value); }
            remove { RemoveHandler(ObjectDrawnEvent, value); }
        }



        /// <summary>
        ///  NodeSelected Routed event. Is raised when the node is deleted.
        /// </summary>
        public static readonly RoutedEvent NodeSelectedEvent = EventManager.RegisterRoutedEvent(
        "NodeSelected", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        /// NodeSelected Event Handler
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeSelected
        {
            add { AddHandler(NodeSelectedEvent, value); }
            remove { RemoveHandler(NodeSelectedEvent, value); }
        }

        /// <summary>
        ///  NodeUnSelected Routed event. Is raised when the node is deleted.
        /// </summary>
        public static readonly RoutedEvent NodeUnSelectedEvent = EventManager.RegisterRoutedEvent(
        "NodeUnSelected", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        /// NodeUnSelected Event Handler
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeUnSelected
        {
            add { AddHandler(NodeUnSelectedEvent, value); }
            remove { RemoveHandler(NodeUnSelectedEvent, value); }
        }

        /// <summary>
        ///  NodeDeleted Routed event. Is raised when the node is deleted.
        /// </summary>
        public static readonly RoutedEvent NodeDeletedEvent = EventManager.RegisterRoutedEvent(
        "NodeDeleted", RoutingStrategy.Bubble, typeof(NodeDeleteEventHandler), typeof(DiagramView));

        /// <summary>
        /// NodeDeleted Event Handler
        /// </summary>
        /// Type: <see cref="NodeDeleteEventHandler"/>
        public event NodeDeleteEventHandler NodeDeleted
        {
            add { AddHandler(NodeDeletedEvent, value); }
            remove { RemoveHandler(NodeDeletedEvent, value); }
        }

        /// <summary>
        ///  NodeDeleted Routed event. Is raised when the node is deleted.
        /// </summary>
        public static readonly RoutedEvent NodeDeletingEvent = EventManager.RegisterRoutedEvent(
        "NodeDeleting", RoutingStrategy.Bubble, typeof(NodeDeleteEventHandler), typeof(DiagramView));

        /// <summary>
        /// NodeDeleted Event Handler
        /// </summary>
        /// Type: <see cref="NodeDeleteEventHandler"/>
        public event NodeDeleteEventHandler NodeDeleting
        {
            add { AddHandler(NodeDeletingEvent, value); }
            remove { RemoveHandler(NodeDeletingEvent, value); }
        }

        /// <summary>
        ///  ConnectorDeleted Routed event. Is raised when the connector is deleted.
        /// </summary>
        public static readonly RoutedEvent ConnectorDeletedEvent = EventManager.RegisterRoutedEvent(
        "ConnectorDeleted", RoutingStrategy.Bubble, typeof(ConnectionDeleteEventHandler), typeof(DiagramView));

        /// <summary>
        /// ConnectorDeleted Event Handler
        /// </summary>
        /// Type: <see cref="ConnectionDeleteEventHandler"/>
        public event ConnectionDeleteEventHandler ConnectorDeleted
        {
            add { AddHandler(ConnectorDeletedEvent, value); }
            remove { RemoveHandler(ConnectorDeletedEvent, value); }
        }

        /// <summary>
        ///  ConnectorDeleted Routed event. Is raised when the connector is deleted.
        /// </summary>
        public static readonly RoutedEvent ConnectorDeletingEvent = EventManager.RegisterRoutedEvent(
        "ConnectorDeleting", RoutingStrategy.Bubble, typeof(ConnectionDeleteEventHandler), typeof(DiagramView));

        /// <summary>
        /// ConnectorDeleted Event Handler
        /// </summary>
        /// Type: <see cref="ConnectionDeleteEventHandler"/>
        public event ConnectionDeleteEventHandler ConnectorDeleting
        {
            add { AddHandler(ConnectorDeletingEvent, value); }
            remove { RemoveHandler(ConnectorDeletingEvent, value); }
        }

        /// <summary>
        ///  PreviewNodeDrop Routed event. Is raised when a node is dropped and just before a node object is created.
        /// </summary>
        public static readonly RoutedEvent PreviewNodeDropEvent = EventManager.RegisterRoutedEvent(
        "PreviewNodeDrop", RoutingStrategy.Bubble, typeof(PreviewNodeDropEventHandler), typeof(DiagramView));

        /// <summary>
        /// PreviewNodeDrop Event Handler
        /// </summary>
        /// Type: <see cref="PreviewNodeDropEventHandler"/>
        public event PreviewNodeDropEventHandler PreviewNodeDrop
        {
            add { AddHandler(PreviewNodeDropEvent, value); }
            remove { RemoveHandler(PreviewNodeDropEvent, value); }
        }

        /// <summary>
        ///  PreviewConnectorDrop Routed event. Is raised when the connector is dropped and before a line object is created.
        /// </summary>
        public static readonly RoutedEvent PreviewConnectorDropEvent = EventManager.RegisterRoutedEvent(
        "PreviewConnectorDrop", RoutingStrategy.Bubble, typeof(PreviewConnectorDropEventHandler), typeof(DiagramView));

        /// <summary>
        /// PreviewConnectorDrop Event Handler
        /// </summary>
        /// Type: <see cref="PreviewConnectorDropEventHandler"/>
        public event PreviewConnectorDropEventHandler PreviewConnectorDrop
        {
            add { AddHandler(PreviewConnectorDropEvent, value); }
            remove { RemoveHandler(PreviewConnectorDropEvent, value); }
        }

        /// <summary>
        ///  NodeDragStart Routed event. Is raised when the node drag is started.
        /// </summary>
        public static readonly RoutedEvent NodeDragStartEvent = EventManager.RegisterRoutedEvent(
        "NodeDragStart", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        ///  NodeDragStart Event Handler .
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeDragStart
        {
            add { AddHandler(NodeDragStartEvent, value); }
            remove { RemoveHandler(NodeDragStartEvent, value); }
        }

        public static readonly RoutedEvent NodeDraggingEvent = EventManager.RegisterRoutedEvent(
        "NodeDragging", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        public event NodeEventHandler NodeDragging
        {
            add { AddHandler(NodeDraggingEvent, value); }
            remove { RemoveHandler(NodeDraggingEvent, value); }
        }

        /// <summary>
        ///  NodeDragEnd Routed event. Is raised when the node drag is completed.
        /// </summary>
        public static readonly RoutedEvent NodeDragEndEvent = EventManager.RegisterRoutedEvent(
        "NodeDragEnd", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        ///  NodeDragStart Event Handler .
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeDragEnd
        {
            add { AddHandler(NodeDragEndEvent, value); }
            remove { RemoveHandler(NodeDragEndEvent, value); }
        }

        /// <summary>
        ///  NodeClick Routed event. Is raised when the node is clicked.
        /// </summary>
        public static readonly RoutedEvent NodeClickEvent = EventManager.RegisterRoutedEvent(
          "NodeClick", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));


        public static readonly RoutedEvent ConnectorClickEvent = EventManager.RegisterRoutedEvent(
            "ConnectorClick", RoutingStrategy.Bubble, typeof(ConnectorRoutedEventHandler), typeof(DiagramView));
        /// <summary>
        ///  NodeClick Event Handler .
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeClick
        {
            add { AddHandler(NodeClickEvent, value); }
            remove { RemoveHandler(NodeClickEvent, value); }
        }

        public event ConnectorRoutedEventHandler ConnectorClick
        {
            add { AddHandler(ConnectorClickEvent, value); }
            remove { RemoveHandler(ConnectorClickEvent, value); }
        }

        /// <summary>
        ///  NodeDoubleClick Routed event. Is raised when the node is clicked twice in succession.
        /// </summary>
        public static readonly RoutedEvent NodeDoubleClickEvent = EventManager.RegisterRoutedEvent(
       "NodeDoubleClick", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        ///  NodeDoubleClick Event Handler .
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeDoubleClick
        {
            add { AddHandler(NodeDoubleClickEvent, value); }
            remove { RemoveHandler(NodeDoubleClickEvent, value); }
        }

        /// <summary>
        ///  ConnectorDoubleClick Routed event. Is raised when the Connector is clicked twice in succession.
        /// </summary>
        public static readonly RoutedEvent ConnectorDoubleClickEvent = EventManager.RegisterRoutedEvent(
      "ConnectorDoubleClick", RoutingStrategy.Bubble, typeof(ConnChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  ConnectorDoubleClick Event Handler .
        /// </summary>
        /// Type: <see cref="ConnChangedEventHandler"/>
        public event ConnChangedEventHandler ConnectorDoubleClick
        {
            add { AddHandler(ConnectorDoubleClickEvent, value); }
            remove { RemoveHandler(ConnectorDoubleClickEvent, value); }
        }

        /// <summary>
        ///  NodeDrop Routed event. Is raised when the node is dropped on the page from the symbol palette.
        /// </summary>
        public static readonly RoutedEvent NodeDropEvent = EventManager.RegisterRoutedEvent(
     "NodeDrop", RoutingStrategy.Bubble, typeof(NodeDroppedEventHandler), typeof(DiagramView));

        public event NewNodeDroppedEventHandler NodeDropping
        {
            add { AddHandler(NodeDroppingEvent, value); }
            remove { RemoveHandler(NodeDroppingEvent, value); }
        }
        /// <summary>
        ///  NodeDropped Event Handler .
        /// </summary>
        /// Type: <see cref="NodeDroppedEventHandler"/>
        public event NodeDroppedEventHandler NodeDrop
        {
            add { AddHandler(NodeDropEvent, value); }
            remove { RemoveHandler(NodeDropEvent, value); }
        }

        public static readonly RoutedEvent NodeDroppingEvent = EventManager.RegisterRoutedEvent(" NodeDropping", RoutingStrategy.Bubble, typeof(NewNodeDroppedEventHandler), typeof(DiagramView));
        /// <summary>
        ///  GroupDrop Routed event. Is raised when the Group is dropped on the page from the symbol palette.
        /// </summary>
        public static readonly RoutedEvent GroupDropEvent = EventManager.RegisterRoutedEvent(
     "GroupDrop", RoutingStrategy.Bubble, typeof(GroupDroppedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  GroupDropped Event Handler .
        /// </summary>
        /// Type: <see cref="NodeDroppedEventHandler"/>
        public event GroupDroppedEventHandler GroupDrop
        {
            add { AddHandler(GroupDropEvent, value); }
            remove { RemoveHandler(GroupDropEvent, value); }
        }
        /// <summary>
        ///  ConnectorDrop Routed event. Is raised when the Connector is dropped on the page from the symbol palette.
        /// </summary>
        public static readonly RoutedEvent ConnectorDropEvent = EventManager.RegisterRoutedEvent(
     "ConnectorDrop", RoutingStrategy.Bubble, typeof(ConnectorDroppedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  ConnectorDropped Event Handler .
        /// </summary>
        /// Type: <see cref="ConnectorDroppedEventHandler"/>
        public event ConnectorDroppedEventHandler ConnectorDrop
        {
            add { AddHandler(ConnectorDropEvent, value); }
            remove { RemoveHandler(ConnectorDropEvent, value); }
        }

        /// <summary>
        ///  NodeResized Routed event. Is raised when the node is resized.
        /// </summary>
        public static readonly RoutedEvent NodeResizedEvent = EventManager.RegisterRoutedEvent(
      "NodeResized", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        ///  NodeResized Event Handler .
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeResized
        {
            add { AddHandler(NodeResizedEvent, value); }
            remove { RemoveHandler(NodeResizedEvent, value); }
        }

        /// <summary>
        ///  NodeResized Routed event. Is raised when the node is being resized.
        /// </summary>
        public static readonly RoutedEvent NodeResizingEvent = EventManager.RegisterRoutedEvent(
    "NodeResizing", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        ///  NodeResizing Event Handler .
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeResizing
        {
            add { AddHandler(NodeResizingEvent, value); }
            remove { RemoveHandler(NodeResizingEvent, value); }
        }

        /// <summary>
        ///  NodeRotationChanged Routed event. Is raised when the node is  rotated.
        /// </summary>
        public static readonly RoutedEvent NodeRotationChangedEvent = EventManager.RegisterRoutedEvent(
   "NodeRotationChanged", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        ///  NodeRotationChanged Event Handler .
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeRotationChanged
        {
            add { AddHandler(NodeRotationChangedEvent, value); }
            remove { RemoveHandler(NodeRotationChangedEvent, value); }
        }

        /// <summary>
        ///  NodeRotationChanging Routed event. Is raised when the node is being  rotated.
        /// </summary>
        public static readonly RoutedEvent NodeRotationChangingEvent = EventManager.RegisterRoutedEvent(
  "NodeRotationChanging", RoutingStrategy.Bubble, typeof(NodeEventHandler), typeof(DiagramView));

        /// <summary>
        ///  NodeRotationChanging Event Handler .
        /// </summary>
        /// Type: <see cref="NodeEventHandler"/>
        public event NodeEventHandler NodeRotationChanging
        {
            add { AddHandler(NodeRotationChangingEvent, value); }
            remove { RemoveHandler(NodeRotationChangingEvent, value); }
        }

        /// <summary>
        ///  ConnectorDragStart Routed event. Is raised when the connector drag is started.
        /// </summary>
        public static readonly RoutedEvent ConnectorDragStartEvent = EventManager.RegisterRoutedEvent(
      "ConnectorDragStart", RoutingStrategy.Bubble, typeof(ConnDragChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  ConnectorDragStart Event Handler .
        /// </summary>
        /// Type: <see cref="ConnDragChangedEventHandler"/>
        public event ConnDragChangedEventHandler ConnectorDragStart
        {
            add { AddHandler(ConnectorDragStartEvent, value); }
            remove { RemoveHandler(ConnectorDragStartEvent, value); }
        }

        /// <summary>
        ///  ConnectorDragEnd Routed event. Is raised when the connector drag is completed.
        /// </summary>
        public static readonly RoutedEvent ConnectorDragEndEvent = EventManager.RegisterRoutedEvent(
     "ConnectorDragEnd", RoutingStrategy.Bubble, typeof(ConnDragEndChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  ConnectorDragEnd Event Handler .
        /// </summary>
        /// Type: <see cref="ConnDragEndChangedEventHandler"/>
        public event ConnDragEndChangedEventHandler ConnectorDragEnd
        {
            add { AddHandler(ConnectorDragEndEvent, value); }
            remove { RemoveHandler(ConnectorDragEndEvent, value); }
        }

        /// <summary>
        ///  HeadNodeChanged Routed event. Is raised when the connector's head node is changed.
        /// </summary>
        public static readonly RoutedEvent HeadNodeChangedEvent = EventManager.RegisterRoutedEvent(
    "HeadNodeChanged", RoutingStrategy.Bubble, typeof(NodeChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  HeadNodeChanged Event Handler .
        /// </summary>
        /// Type: <see cref="NodeChangedEventHandler"/>
        public event NodeChangedEventHandler HeadNodeChanged
        {
            add { AddHandler(HeadNodeChangedEvent, value); }
            remove { RemoveHandler(HeadNodeChangedEvent, value); }
        }

        /// <summary>
        ///  TailNodeChanged Routed event. Is raised when the connector's tail node is changed.
        /// </summary>
        public static readonly RoutedEvent TailNodeChangedEvent = EventManager.RegisterRoutedEvent(
   "TailNodeChanged", RoutingStrategy.Bubble, typeof(NodeChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  TailNodeChanged Event Handler .
        /// </summary>
        /// Type: <see cref="NodeChangedEventHandler"/>
        public event NodeChangedEventHandler TailNodeChanged
        {
            add { AddHandler(TailNodeChangedEvent, value); }
            remove { RemoveHandler(TailNodeChangedEvent, value); }
        }

        /// <summary>
        ///  NodeLabelChanged Routed event. Is raised when the node's label is changed.
        /// </summary>
        public static readonly RoutedEvent NodeLabelChangedEvent = EventManager.RegisterRoutedEvent(
   "NodeLabelChanged", RoutingStrategy.Bubble, typeof(LabelChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  NodeLabelChanged Event Handler .
        /// </summary>
        /// Type: <see cref="LabelChangedEventHandler"/>
        public event LabelChangedEventHandler NodeLabelChanged
        {
            add { AddHandler(NodeLabelChangedEvent, value); }
            remove { RemoveHandler(NodeLabelChangedEvent, value); }
        }

        /// <summary>
        ///  NodeStartLabelEdit Routed event. Is raised when the node's label is started to be edited.
        /// </summary>
        public static readonly RoutedEvent NodeStartLabelEditEvent = EventManager.RegisterRoutedEvent(
"NodeStartLabelEdit", RoutingStrategy.Bubble, typeof(LabelChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  LabelChangedEventHandler Event Handler .
        /// </summary>
        /// Type: <see cref="LabelChangedEventHandler"/>
        public event LabelChangedEventHandler NodeStartLabelEdit
        {
            add { AddHandler(NodeStartLabelEditEvent, value); }
            remove { RemoveHandler(NodeStartLabelEditEvent, value); }
        }

        /// <summary>
        ///  ConnectorLabelChanged Routed event. Is invoked when the connector's label is changed.
        /// </summary>
        public static readonly RoutedEvent ConnectorLabelChangedEvent = EventManager.RegisterRoutedEvent(
"ConnectorLabelChanged", RoutingStrategy.Bubble, typeof(LabelConnChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  LabelChangedEventHandler Event Handler .
        /// </summary>
        /// Type: <see cref="LabelConnChangedEventHandler"/>
        public event LabelConnChangedEventHandler ConnectorLabelChanged
        {
            add { AddHandler(ConnectorLabelChangedEvent, value); }
            remove { RemoveHandler(ConnectorLabelChangedEvent, value); }
        }

        /// <summary>
        ///  ConnectorStartLabelEdit Routed event. Is raised when the connections's label is started to be edited.
        /// </summary>
        public static readonly RoutedEvent ConnectorStartLabelEditEvent = EventManager.RegisterRoutedEvent(
"ConnectorStartLabelEdit", RoutingStrategy.Bubble, typeof(LabelEditConnChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  ConnectorStartLabelEdit Event Handler .
        /// </summary>
        /// Type: <see cref="LabelEditConnChangedEventHandler"/>
        public event LabelEditConnChangedEventHandler ConnectorStartLabelEdit
        {
            add { AddHandler(ConnectorStartLabelEditEvent, value); }
            remove { RemoveHandler(ConnectorStartLabelEditEvent, value); }
        }

        /// <summary>
        ///  BeforeConnectionCreate Routed event. Is raised just before the user starts to create a new connection.
        /// </summary>
        public static readonly RoutedEvent BeforeConnectionCreateEvent = EventManager.RegisterRoutedEvent(
      "BeforeConnectionCreate", RoutingStrategy.Bubble, typeof(BeforeCreateConnectionEventHandler), typeof(DiagramView));

        /// <summary>
        ///  BeforeConnectionCreate Event Handler .
        /// </summary>
        /// Type: <see cref="BeforeCreateConnectionEventHandler"/>
        public event BeforeCreateConnectionEventHandler BeforeConnectionCreate
        {
            add { AddHandler(BeforeConnectionCreateEvent, value); }
            remove { RemoveHandler(BeforeConnectionCreateEvent, value); }
        }

        /// <summary>
        ///  AfterConnectionCreate Routed event. Is raised when a new connection has been made.
        /// </summary>
        public static readonly RoutedEvent AfterConnectionCreateEvent = EventManager.RegisterRoutedEvent(
      "AfterConnectionCreate", RoutingStrategy.Bubble, typeof(ConnDragEndChangedEventHandler), typeof(DiagramView));

        /// <summary>
        ///  AfterConnectionCreate Event Handler .
        /// </summary>
        /// Type: <see cref="BeforeCreateConnectionEventHandler"/>
        public event ConnDragEndChangedEventHandler AfterConnectionCreate
        {
            add { AddHandler(AfterConnectionCreateEvent, value); }
            remove { RemoveHandler(AfterConnectionCreateEvent, value); }
        }
        public static readonly RoutedEvent GroupingEvent = EventManager.RegisterRoutedEvent(
            "Grouping", RoutingStrategy.Bubble, typeof(GroupEventHandler), typeof(DiagramView));


        public event GroupEventHandler Grouping
        {
            add { AddHandler(GroupingEvent, value); }
            remove { RemoveHandler(GroupingEvent, value); }
        }

        public static readonly RoutedEvent GroupedEvent = EventManager.RegisterRoutedEvent(
            "Grouped", RoutingStrategy.Bubble, typeof(GroupEventHandler), typeof(DiagramView));


        public event GroupEventHandler Grouped
        {
            add { AddHandler(GroupedEvent, value); }
            remove { RemoveHandler(GroupedEvent, value); }
        }

        public static readonly RoutedEvent UngroupingEvent = EventManager.RegisterRoutedEvent(
            "Ungrouping", RoutingStrategy.Bubble, typeof(UnGroupEventHandler), typeof(DiagramView));


        public event UnGroupEventHandler Ungrouping
        {
            add { AddHandler(UngroupingEvent, value); }
            remove { RemoveHandler(UngroupingEvent, value); }
        }
        public static readonly RoutedEvent UngroupedEvent = EventManager.RegisterRoutedEvent(
            "Ungrouped", RoutingStrategy.Bubble, typeof(UnGroupEventHandler), typeof(DiagramView));


        public event UnGroupEventHandler Ungrouped
        {
            add { AddHandler(UngroupedEvent, value); }
            remove { RemoveHandler(UngroupedEvent, value); }
        }

        public static readonly RoutedEvent ConnectorSelectedEvent = EventManager.RegisterRoutedEvent("ConnectorSelected", RoutingStrategy.Bubble, typeof(ConnectorSelectedEventHandler), typeof(DiagramView));

        public event ConnectorSelectedEventHandler ConnectorSelected
        {
            add { AddHandler(ConnectorSelectedEvent, value); }
            remove { RemoveHandler(ConnectorSelectedEvent, value); }
        }

        public static readonly RoutedEvent ConnectorUnSelectedEvent = EventManager.RegisterRoutedEvent("ConnectorUnSelected", RoutingStrategy.Bubble, typeof(ConnectorUnSelectedEventHandler), typeof(DiagramView));

        public event ConnectorUnSelectedEventHandler ConnectorUnSelected
        {
            add { AddHandler(ConnectorUnSelectedEvent, value); }
            remove { RemoveHandler(ConnectorUnSelectedEvent, value); }
        }


        #endregion

        #region Properties

        public SnapSettings SnapSettings
        {
            get { return (SnapSettings)GetValue(SnapSettingsProperty); }
            set { SetValue(SnapSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SnapSettings.  This enables Snap to Obejcts  etc...
        public static readonly DependencyProperty SnapSettingsProperty =
            DependencyProperty.Register("SnapSettings", typeof(SnapSettings), typeof(DiagramView), new PropertyMetadata(null));

        

        //public SnapPort SnapPort
        //{
        //    get { return (SnapPort)GetValue(SnapPortProperty); }
        //    set { SetValue(SnapPortProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for SnapPort.  This enables Snap to Obejcts  etc...
        //public static readonly DependencyProperty SnapPortProperty =
        //    DependencyProperty.Register("SnapPort", typeof(SnapPort), typeof(DiagramView), new PropertyMetadata(null));


        public DateTimeSettings DateTimeSettings
        {
            get { return (DateTimeSettings)GetValue(DateTimeSettingsProperty); }
            set { SetValue(DateTimeSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeSettingsProperty =
            DependencyProperty.Register("DateTimeSettings", typeof(DateTimeSettings), typeof(DiagramView), new UIPropertyMetadata(null));
        
        public bool LineBridgingEnabled
        {
            get
            {
                return (bool)GetValue(LineBridgingEnabledProperty);
            }

            set
            {
                SetValue(LineBridgingEnabledProperty, value);
            }
        }

        public double MinimumZoom
        {
            get { return (double)GetValue(MinimumZoomProperty); }
            set { SetValue(MinimumZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumZoomProperty =
            DependencyProperty.Register("MinimumZoom", typeof(double), typeof(DiagramView), new PropertyMetadata(0.2d));

        public double MaximumZoom
        {
            get { return (double)GetValue(MaximumZoomProperty); }
            set { SetValue(MaximumZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumZoomProperty =
            DependencyProperty.Register("MaximumZoom", typeof(double), typeof(DiagramView), new PropertyMetadata(30d));
        public static readonly DependencyProperty LineBridgingEnabledProperty = DependencyProperty.Register("LineBridgingEnabled", typeof(bool), typeof(DiagramView), new PropertyMetadata(false));
        
        /// <summary>
        /// Identifies the LineRoutingEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty LineRoutingEnabledProperty = DependencyProperty.Register("LineRoutingEnabled", typeof(bool), typeof(DiagramView), new PropertyMetadata(false, new PropertyChangedCallback(OnLineRoutingEnabledChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is line routing enabled.
        /// Default value is false.
        /// </summary>
        public bool LineRoutingEnabled
        {
            get
            {
                return (bool)GetValue(LineRoutingEnabledProperty);
            }

            set
            {
                SetValue(LineRoutingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Called when [line routing enabled changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLineRoutingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (d as DiagramView);
            if ((bool)e.NewValue == true)
            {
                AStarLineRouter asl = new AStarLineRouter(view);
                view.LineRouter = asl;
            }
        }
        private LineRouter m_lineRouter;

        /// <summary>
        /// Gets or sets the LineRouter value.
        /// </summary>
        public LineRouter LineRouter
        {
            get
            {
                return m_lineRouter;
            }
            set
            {
                m_lineRouter = value;
                if (m_lineRouter != null)
                    m_lineRouter.View = this;
            }
        }

        /*
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
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        */

        public ZOrderModes ZOrderMode
        {
            get
            {
                return (ZOrderModes)GetValue(ZOrderModeProperty);
            }
            set
            {
                SetValue(ZOrderModeProperty, value);
            }
        }
        internal static readonly DependencyProperty PageMarginProperty = DependencyProperty.Register("PageMargin", typeof(Thickness), typeof(DiagramView), new PropertyMetadata());

        public Thickness PageMargin
        {
            get
            {
                return (Thickness)GetValue(PageMarginProperty);
            }

            set
            {
                SetValue(PageMarginProperty, value);
            }

        }

        public double SnapOffsetX
        {
            get
            {
                return (double)GetValue(SnapOffsetXProperty);
            }

            set
            {
                SetValue(SnapOffsetXProperty, value);
            }
        }

        public double SnapOffsetY
        {
            get
            {
                return (double)GetValue(SnapOffsetYProperty);
            }

            set
            {
                SetValue(SnapOffsetYProperty, value);
            }
        }

        public bool SnapToHorizontalGrid
        {
            get
            {
                return (bool)GetValue(SnapToHorizontalGridProperty);
            }

            set
            {
                SetValue(SnapToHorizontalGridProperty, value);
            }
        }

        public bool SnapToVerticalGrid
        {
            get
            {
                return (bool)GetValue(SnapToVerticalGridProperty);
            }

            set
            {
                SetValue(SnapToVerticalGridProperty, value);
            }
        }

        public bool IsCutEnabled
        {
            get
            {
                return (bool)GetValue(IsCutEnabledProperty);
            }

            set
            {
                SetValue(IsCutEnabledProperty, value);
            }
        }
        public bool DirectionBehaviourEnabled
        {
            get { return (bool)GetValue(DirectionBehaviourEnabledProperty); }
            set { SetValue(DirectionBehaviourEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DirectionBehaviourEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionBehaviourEnabledProperty =
            DependencyProperty.Register("DirectionBehaviourEnabled", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(false));



        internal bool AllowMoveX
        {
            get { return (bool)GetValue(AllowMoveXProperty); }
            set { SetValue(AllowMoveXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowMoveX.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AllowMoveXProperty =
            DependencyProperty.Register("AllowMoveX", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(true));



        internal bool AllowMoveY
        {
            get { return (bool)GetValue(AllowMoveYProperty); }
            set { SetValue(AllowMoveYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowMoveY.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AllowMoveYProperty =
            DependencyProperty.Register("AllowMoveY", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(true));

        private RoutingMode _RoutingMode;

        public RoutingMode RoutingMode
        {
            get { return _RoutingMode; }
            set { _RoutingMode = value; }
        }
        private NodeMovementX _NodeMovementX;

        public NodeMovementX NodeMovementX
        {
            get { return _NodeMovementX; }
            set { _NodeMovementX = value; }
        }

        private NodeMovementY _NodeMovementY;

        public NodeMovementY NodeMovementY
        {
            get { return _NodeMovementY; }
            set { _NodeMovementY = value; }
        }

        internal bool _RunTimeMovement = false;
        private TranslateRailsMode _TranslateRailsMode;

        public TranslateRailsMode TranslateRailsMode
        {
            get { return _TranslateRailsMode; }
            set { _TranslateRailsMode = value; }
        }

       
        private bool _onlyXmove;

        internal bool OnlyX
        {
            get { return _onlyXmove; }
            set { _onlyXmove = value; }
        }

        private bool _onlyYmove;

        internal bool OnlyY
        {
            get { return _onlyYmove; }
            set { _onlyYmove = value; }
        }
       
        private bool _IsRotating;

        internal bool IsRotating
        {
            get { return _IsRotating; }
            set { _IsRotating = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether connection has to be created on Node drop or not.
        /// Default value is false.
        /// </summary>
        public bool BisectConnectorOnDrop
        {
            get { return (bool)GetValue(EnableDynamicLayoutProperty); }
            set { SetValue(EnableDynamicLayoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableDynamicLayoutProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableDynamicLayoutProperty =
            DependencyProperty.Register("EnableDynamicLayout", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(false));

        
        public bool EnableDrawingTools
        {
            get
            {
                return (bool)GetValue(EnableDrawingToolsProperty);
            }

            set
            {
                SetValue(EnableDrawingToolsProperty, value);
            }
        }
        public bool IsCopyEnabled
        {
            get
            {
                return (bool)GetValue(IsCopyEnabledProperty);
            }

            set
            {
                SetValue(IsCopyEnabledProperty, value);
            }
        }

        public bool IsPasteEnabled
        {
            get
            {
                return (bool)GetValue(IsPasteEnabledProperty);
            }

            set
            {
                SetValue(IsPasteEnabledProperty, value);
            }
        }

        /*static void l_LayerPropertyChanged(object sender, LineNudgeEventArgs evtArgs)
        {
        }*/

        /// <summary>
        /// Gets or sets the port visibility.
        /// </summary>
        /// <value>The port visibility.</value>
        /// <remarks>
        /// Setting PortVisibility from DiagramView applies to all the nodes in the page. However if any node has specifically set PortVisibility, then the node's PortVisibility property will be taken into account only for that node. So even if DiagramView's PortVisibility is set to false, if Node's <see cref="T:Syncfusion.Windows.Diagram.Node.PortVisibility"/> is set to true then the ports will be displayed for that node.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Background="LightGray"  
        ///                                        Bounds="0,0,12,12" 
        ///                                        PortVisibility="False"
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.PortVisibility=false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public PortVisibility PortVisibility
        {
            get
            {
                return (PortVisibility)GetValue(PortVisibilityProperty);
            }

            set
            {
                SetValue(PortVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable connection].
        /// </summary>
        /// <value><c>true</c> if [enable connection]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property is generally placed in the click event handler for setting the <see cref="T:Syncfusion.Windows.Diagram.LineConnector.ConnectorType"/>.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView EnableConnection="True"
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.EnableConnection=true;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool EnableConnection
        {
            get
            {
                return (bool)GetValue(EnableConnectionProperty);
            }

            set
            {
                SetValue(EnableConnectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of shape to be used.
        /// </summary>
        /// <value>
        /// Type: <see cref="DrawingTools"/>
        /// Enum specifying the type of the Shape to be used.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set DrawingTools in C#.
        /// <code language="C#">
        /// diagramView.DrawingTool = DrawingTools.Ellipse;
        /// </code>
        /// </example>

        public DrawingTools DrawingTool
        {
            get { return (DrawingTools)GetValue(DrawingToolsProperty); }
            set { SetValue(DrawingToolsProperty, value); }
        }


        /// <summary>DrawingToolsProperty
        /// Gets or sets the page's position (0,0) with respect to the View.
        /// </summary>
        internal Point ViewGridOrigin
        {
            get
            {
                return (Point)GetValue(ViewGridOriginProperty);
            }

            set
            {
                SetValue(ViewGridOriginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current zoom.
        /// </summary>
        internal double CurrentZoom
        {
            get
            {
                return (double)GetValue(CurrentZoomProperty);
            }

            set
            {
                SetValue(CurrentZoomProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ZoomFactor .
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Zoom Factor in pixels.
        /// </value>
        /// <remarks>
        /// Default value is 0.2d .
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramView"/> in XAML.
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView ZoomFactor="1"
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <para/>The following example shows how to create a <see cref="DiagramView"/> in C#.
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.ZoomFactor=1;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double ZoomFactor
        {
            get
            {
                return (double)GetValue(ZoomFactorProperty);
            }

            set
            {
                SetValue(ZoomFactorProperty, value);
            }
        }

        public bool IsNodeDragCancel
        {
            get
            {
                return (bool)GetValue(IsNodeDragCancelProperty);
            }
            set
            {
                SetValue(IsNodeDragCancelProperty, value);
            }
        }
        internal bool m_EnableConnection = false;
        public bool IsConnectorDragCancel
        {
            get
            {
                return (bool)GetValue(IsConnectorDragCancelProperty);
            }
            set
            {
                SetValue(IsConnectorDragCancelProperty, value);
            }
        }

        public double NudgeIncrement
        {
            get
            {
                return (double)GetValue(NudgeIncrementProperty);
            }

            set
            {
                SetValue(NudgeIncrementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is zoom enabled.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if zooming is enabled, false otherwise.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsZoomEnabled="True" 
        ///                                        Background="LightGray"  
        ///                                        Bounds="0,0,12,12" 
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsZoomEnabled=true;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool IsZoomEnabled
        {
            get
            {
                return (bool)GetValue(IsZoomEnabledProperty);
            }

            set
            {
                SetValue(IsZoomEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is page editable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is page editable; otherwise, <c>false</c>.
        /// </value>
        /// <summary>
        /// Gets or sets a value indicating whether [enable connection].
        /// </summary>
        /// <value><c>true</c> if [enable connection]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property is generally placed in the click event handler for setting the <see cref="T:Syncfusion.Windows.Diagram.LineConnector.ConnectorType"/>.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView IsPageEditable="True"
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool IsPageEditable
        {
            get
            {
                return (bool)GetValue(IsPageEditableProperty);
            }

            set
            {
                SetValue(IsPageEditableProperty, value);
            }
        }


        public bool EnableVirtualization
        {
            get
            {
                return (bool)GetValue(EnableVirtualizationProperty);
            }

            set
            {
                SetValue(EnableVirtualizationProperty, value);
            }
        }

        public bool EnableCaching
        {
            get
            {
                return (bool)GetValue(EnableCachingProperty);
            }

            set
            {
                SetValue(EnableCachingProperty, value);
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pan enabled.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if panning is enabled, false otherwise.
        /// </value>
        /// <summary>
        /// Gets or sets a value indicating whether [enable connection].
        /// </summary>
        /// <value><c>true</c> if [enable connection]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property is generally placed in the click event handler for setting the <see cref="T:Syncfusion.Windows.Diagram.LineConnector.ConnectorType"/>.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramView"/> in XAML.
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView IsPanEnabled="False"
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.IsPanEnabled=false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool IsPanEnabled
        {
            get
            {
                return (bool)GetValue(IsPanEnabledProperty);
            }

            set
            {
                SetValue(IsPanEnabledProperty, value);
            }
        }

        public ItemSelectionMode ItemSelectionMode
        {
            get
            {
                return (ItemSelectionMode)GetValue(ItemSelectionModeProperty);
            }

            set
            {
                SetValue(ItemSelectionModeProperty, value);
            }
        }

        /// <summary>
        /// Gets the currently selected item. .
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Selected item.
        /// </value>
        public object SelectedItem
        {
            get
            {
                return this.GetValue(SelectedItemProperty);
            }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// Type: <see cref="Panel"/>
        /// Panel instance.
        /// </value>
        [Browsable(false)]
        public Panel Page
        {
            get
            {
                return (Panel)GetValue(PageProperty);
            }

            set
            {
                SetValue(PageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal grid line style.
        /// </summary>
        /// <value>
        /// Type: <see cref="Pen"/>
        /// Line Style.
        /// </value>
        public System.Windows.Media.Pen HorizontalGridLineStyle
        {
            get
            {
                return (System.Windows.Media.Pen)GetValue(HorizontalGridLineStyleProperty);
            }

            set
            {
                SetValue(HorizontalGridLineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical grid line style.
        /// </summary>
        /// <value>
        /// Type: <see cref="Pen"/>
        /// Line Style.
        /// </value>
        public System.Windows.Media.Pen VerticalGridLineStyle
        {
            get
            {
                return (System.Windows.Media.Pen)GetValue(VerticalGridLineStyleProperty);
            }

            set
            {
                SetValue(VerticalGridLineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show horizontal grid line].
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if it is to be displayed, false otherwise.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  ShowHorizontalGridLine="False" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.ShowHorizontalGridLine=false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool ShowHorizontalGridLine
        {
            get
            {
                return (bool)GetValue(ShowHorizontalGridLineProperty);
            }

            set
            {
                SetValue(ShowHorizontalGridLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show vertical grid line].
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if it is to be displayed, false otherwise.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  ShowVerticalGridLine="False" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.ShowVerticalGridLine=false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public bool ShowVerticalGridLine
        {
            get
            {
                return (bool)GetValue(ShowVerticalGridLineProperty);
            }

            set
            {
                SetValue(ShowVerticalGridLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show horizontal rulers].
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if it is to be displayed, false otherwise.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  ShowHorizontalRulers="False" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.ShowHorizontalRulers=false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="HorizontalRuler"/>
        public bool ShowHorizontalRulers
        {
            get
            {
                return (bool)GetValue(ShowHorizontalRulerProperty);
            }

            set
            {
                SetValue(ShowHorizontalRulerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show vertical rulers].
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if it is to be displayed, false otherwise.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  ShowVerticalRulers="False" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.ShowVerticalRulers=false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="VerticalRuler"/>
        public bool ShowVerticalRulers
        {
            get
            {
                return (bool)GetValue(ShowVerticalRulerProperty);
            }

            set
            {
                SetValue(ShowVerticalRulerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal ruler.
        /// </summary>
        /// <value>
        /// Type: <see cref="HorizontalRuler"/>
        /// HorizontalRuler instance.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  ShowHorizontalRulers="False" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       View.ShowHorizontalRulers=false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="HorizontalRuler"/>
        public HorizontalRuler HorizontalRuler
        {
            get
            {
                return (HorizontalRuler)GetValue(HorizontalRulerProperty);
            }

            set
            {
                SetValue(HorizontalRulerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical ruler.
        /// </summary>
        /// <value>
        /// Type: <see cref="VerticalRuler"/>
        /// VerticalRuler instance.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  ShowHorizontalRulers="False" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///       View.ShowVerticalRulers=false;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="HorizontalRuler"/>
        public VerticalRuler VerticalRuler
        {
            get
            {
                return (VerticalRuler)GetValue(VerticalRulerProperty);
            }

            set
            {
                SetValue(VerticalRulerProperty, value);
            }
        }

        /// <summary>
        /// Gets the selection list.
        /// </summary>
        /// <value>
        /// Type: <see cref="NodeCollection"/>
        /// NodeCollection items.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.ShowVerticalRulers=false;
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        View.SelectionList.Add(n);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="NodeCollection"/>
        public NodeCollection SelectionList
        {
            get
            {
                if (Page == null)
                {
                    return null;
                }
                else
                {
                    return (Page as DiagramPage).SelectionList;
                }
            }
        }

        /// <summary>
        /// Gets or sets the node context menu.
        /// </summary>
        /// <value>The node context menu.
        /// Type: <see cref="ContextMenu"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.ShowVerticalRulers=false;
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        Model.Nodes.Add(n);
        ///        ContextMenu menu = new ContextMenu();
        ///        MenuItem m1 = new MenuItem();
        ///        m1.Header = "item1";
        ///        MenuItem m2 = new MenuItem();
        ///        m2.Header = "item2";
        ///        menu.Items.Add(m1);
        ///        menu.Items.Add(m2);
        ///        View.NodeContextMenu=menu;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="ContextMenu"/>
        /// <seealso cref="Node"/>
        public ContextMenu NodeContextMenu
        {
            get
            {
                return (ContextMenu)GetValue(NodeContextMenuProperty);
            }

            set
            {
                SetValue(NodeContextMenuProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LineConnector context menu.
        /// </summary>
        /// <value>The lineConnector context menu.
        /// Type: <see cref="ContextMenu"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
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
        ///       View.ShowVerticalRulers=false;
        ///        ContextMenu menu = new ContextMenu();
        ///        MenuItem m1 = new MenuItem();
        ///        m1.Header = "item1";
        ///        MenuItem m2 = new MenuItem();
        ///        m2.Header = "item2";
        ///        menu.Items.Add(m1);
        ///        menu.Items.Add(m2);
        ///        View.LineConnectorContextMenu=menu;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="ContextMenu"/>
        /// <seealso cref="LineConnector"/>
        public ContextMenu LineConnectorContextMenu
        {
            get
            {
                return (ContextMenu)GetValue(LineConnectorContextMenuProperty);
            }

            set
            {
                SetValue(LineConnectorContextMenuProperty, value);
            }
        }

        /// <summary>
        /// Gets the vertical scroll bar visibility.
        /// </summary>
        /// <value>
        /// The  vertical scroll bar visibility. Default value is Auto.
        /// Type: <see cref="ScrollBarVisibility"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public Window1 ()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// HorizontalRuler hruler = new HorizontalRuler();
        /// View.HorizontalRuler = hruler;
        /// View.ShowHorizontalGridLine = false;
        /// View.ShowVerticalGridLine = false;
        /// VerticalRuler vruler = new VerticalRuler();
        /// View.VerticalRuler = vruler;
        /// View.Bounds = new Thickness (0, 0, 1000, 1000);
        /// View.IsPageEditable = true;
        /// View.ShowVerticalRulers=false;
        /// ScrollBarVisibility Vvisible=View.GetVerticalScrollBarVisibility;
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ScrollBarVisibility"/>
        public ScrollBarVisibility GetVerticalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)GetValue(GetVerticalScrollBarVisibilityProperty);
            }

            internal set
            {
                SetValue(GetVerticalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets the horizontal scroll bar visibility.
        /// </summary>
        /// <value>
        /// The  horizontal scroll bar visibility. Default value is Auto.
        /// Type: <see cref="ScrollBarVisibility"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public Window1 ()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// HorizontalRuler hruler = new HorizontalRuler();
        /// View.HorizontalRuler = hruler;
        /// View.ShowHorizontalGridLine = false;
        /// View.ShowVerticalGridLine = false;
        /// VerticalRuler vruler = new VerticalRuler();
        /// View.VerticalRuler = vruler;
        /// View.Bounds = new Thickness (0, 0, 1000, 1000);
        /// View.IsPageEditable = true;
        /// View.ShowVerticalRulers=false;
        /// ScrollBarVisibility hvisible=View.GetHorizontalScrollBarVisibility;
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ScrollBarVisibility"/>
        public ScrollBarVisibility GetHorizontalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)GetValue(GetHorizontalScrollBarVisibilityProperty);
            }

            internal set
            {
                SetValue(GetHorizontalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll bar visibility.
        /// </summary>
        /// <value>
        /// The  horizontal scroll bar visibility. Default value is Auto.
        /// Type: <see cref="ScrollBarVisibility"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public Window1 ()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// HorizontalRuler hruler = new HorizontalRuler();
        /// View.HorizontalRuler = hruler;
        /// View.ShowHorizontalGridLine = false;
        /// View.ShowVerticalGridLine = false;
        /// VerticalRuler vruler = new VerticalRuler();
        /// View.VerticalRuler = vruler;
        /// View.Bounds = new Thickness (0, 0, 1000, 1000);
        /// View.IsPageEditable = true;
        /// View.ShowVerticalRulers=false;
        /// ScrollBarVisibility hvisible=View.HorizontalScrollBarVisibility;
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ScrollBarVisibility"/>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            }

            set
            {
                SetValue(HorizontalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical scroll bar visibility.
        /// </summary>
        /// <value>
        /// The  vertical scroll bar visibility. Default value is Auto.
        /// Type: <see cref="ScrollBarVisibility"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public Window1 ()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// HorizontalRuler hruler = new HorizontalRuler();
        /// View.HorizontalRuler = hruler;
        /// View.ShowHorizontalGridLine = false;
        /// View.ShowVerticalGridLine = false;
        /// VerticalRuler vruler = new VerticalRuler();
        /// View.VerticalRuler = vruler;
        /// View.Bounds = new Thickness (0, 0, 1000, 1000);
        /// View.IsPageEditable = true;
        /// View.ShowVerticalRulers=false;
        /// ScrollBarVisibility Vvisible=View.VerticalScrollBarVisibility;
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ScrollBarVisibility"/>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            }

            set
            {
                SetValue(VerticalScrollBarVisibilityProperty, value);
            }
        }
        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the <see cref="Node"/> resized count.
        /// </summary>
        /// <value>The node resized count.</value>
        internal int NodeResizedCount
        {
            get { return resizecount; }
            set { resizecount = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Node"/> rotate count.
        /// </summary>
        /// <value>The node rotate count.</value>
        internal int NodeRotateCount
        {
            get { return rotatecount; }
            set { rotatecount = value; }
        }

        /// <summary>
        /// Gets or sets the undo stack.
        /// </summary>
        /// <value>The undo stack.</value>
        internal Stack<object> UndoStack
        {
            get { return undocommandstack; }
            set { undocommandstack = value; }
        }

        /// <summary>
        /// Gets or sets the redo stack.
        /// </summary>
        /// <value>The redo stack.</value>
        internal Stack<object> RedoStack
        {
            get { return redocommandstack; }
            set { redocommandstack = value; }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether the scrollbar were moved.
        ///// </summary>
        ///// <value>
        ///// <c>true</c> if mouse scrolled; otherwise, <c>false</c>.
        ///// </value>
        //internal bool IsMouseScrolled
        //{
        //    get { return mscrolled; }
        //    set { mscrolled = value; }
        //}

        /// <summary>
        /// Gets or sets the view grid which contains the page.
        /// </summary>
        /// <value>The view grid.</value>
        internal Grid ViewGrid
        {
            get { return viewgrid; }
            set { viewgrid = value; }
        }

        /// <summary>
        /// Gets the internal groups collection.
        /// </summary>
        /// <value>The internal groups collection.</value>
        internal CollectionExt InternalGroups
        {
            get { return m_groups; }
        }

        ///// <summary>
        ///// Gets or sets the old horizontal offset.
        ///// </summary>
        ///// <value>The old old horizontal offset.</value>
        //internal double OldHoroffset
        //{
        //    get { return oldhoffset; }
        //    set { oldhoffset = value; }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="Node"/> is deleted.
        /// </summary>
        /// <value><c>true</c> if node is deleted; otherwise, <c>false</c>.</value>
        internal bool Isnodedeleted
        {
            get { return nodedel; }
            set { nodedel = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="LineConnector"/> is deleted.
        /// </summary>
        /// <value><c>true</c> if is line deleted; otherwise, <c>false</c>.</value>
        internal bool Islinedeleted
        {
            get { return linedel; }
            set { linedel = value; }
        }

        /// <summary>
        /// Gets the internal edges.
        /// </summary>
        /// <value>The internal edges.</value>
        internal CollectionExt InternalEdges
        {
            get { return intedges; }
        }

        ///// <summary>
        ///// Gets or sets the horizontal thumb drag offset.
        ///// </summary>
        ///// <value>The horizontal thumb drag offset.</value>
        //internal double HorThumbDragOffset
        //{
        //    get { return scrollhorthumb; }
        //    set { scrollhorthumb = value; }
        //}

        ///// <summary>
        ///// Gets or sets the old vertical offset.
        ///// </summary>
        ///// <value>The old vertical offset.</value>
        //internal double OldVeroffset
        //{
        //    get { return oldvoffset; }
        //    set { oldvoffset = value; }
        //}

        ///// <summary>
        ///// Gets or sets the vertical thumb drag offset.
        ///// </summary>
        ///// <value>The vertical thumb drag offset.</value>
        //internal double VerThumbDragOffset
        //{
        //    get { return scrollverthumb; }
        //    set { scrollverthumb = value; }
        //}

        ///// <summary>
        ///// Gets or sets a value indicating whether this instance is scroll thumb.
        ///// </summary>
        ///// <value>
        ///// <c>true</c> if this instance is scroll thumb; otherwise, <c>false</c>.
        ///// </value>
        //internal bool IsScrollThumb
        //{
        //    get { return scrollthumb; }
        //    set { scrollthumb = value; }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Node"/> position is changed.
        /// </summary>
        /// <value><c>true</c> if position is changed; otherwise, <c>false</c>.</value>
        internal bool Ispositionchanged
        {
            get { return isdrag; }
            set { isdrag = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is zoom changed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is zoom changed; otherwise, <c>false</c>.
        /// </value>
        internal static bool ViewGridOriginChanged
        {
            get { return isvieworiginchanged; }
            set { isvieworiginchanged = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is other event.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is other event; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsOtherEvent
        {
            get { return otherevents; }
            set { otherevents = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [page edit].
        /// </summary>
        /// <value><c>true</c> if [page edit]; otherwise, <c>false</c>.</value>
        internal static bool PageEdit
        {
            get { return ispagedit; }
            set { ispagedit = value; }
        }

        /// <summary>
        /// Gets or sets the scrollviewer.
        /// </summary>
        /// <value>The scrollviewer.</value>
        internal ScrollViewer Scrollviewer
        {
            get { return scrollview; }
            set { scrollview = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether automatic layout is used.
        /// </summary>
        /// <value><c>true</c> if automatic layout is used; otherwise, <c>false</c>.</value>
        internal bool IsLayout
        {
            get { return layout; }
            set { layout = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node is dragged.
        /// </summary>
        /// <value><c>true</c> if is dragged; otherwise, <c>false</c>.</value>
        internal bool Isdragdelta
        {
            get { return isdragged; }
            set { isdragged = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether delete is done.
        /// </summary>
        /// <value><c>true</c> if [deleted]; otherwise, <c>false</c>.</value>
        internal bool DupDeleted
        {
            get { return isdupdel; }
            set { isdupdel = value; }
        }

        internal string DragDelta = "No";

        /// <summary>
        /// Gets or sets a value indicating whether [undo redo  is enabled].
        /// </summary>
        /// <value><c>true</c> if [undo redo is enabled]; otherwise, <c>false</c>.</value>
        public bool UndoRedoEnabled
        {
            get
            {
                if (CPManager != null)
                {
                    CPManager.invalidateSelection = true;
                }
                return (bool)GetValue(UndoRedoEnabledProperty);
            }

            set
            {
                SetValue(UndoRedoEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Undo command is executed.
        /// </summary>
        /// <value><c>true</c> if undone; otherwise, <c>false</c>.</value>
        internal bool Undone
        {
            get { return undo; }
            set { undo = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Redo command is executed.
        /// </summary>
        /// <value><c>true</c> if redone; otherwise, <c>false</c>.</value>
        internal bool Redone
        {
            get { return redo; }
            set { redo = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> is dragged.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Node"/>  is dragged; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDragged
        {
            get { return dragged; }
            set { dragged = value; }
        }

        internal bool IsKeyDragged = false;
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> is resized.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Node"/> is resized; otherwise, <c>false</c>.
        /// </value>
        internal bool IsResized
        {
            get { return resized; }
            set { resized = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> is resized as a result of undo operation.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Node"/> is resized undone; otherwise, <c>false</c>.
        /// </value>
        internal bool IsResizedUndone
        {
            get { return undoresize; }
            set { undoresize = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> is resized as a result of redo operation.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Node"/> is resized redone; otherwise, <c>false</c>.
        /// </value>
        internal bool IsResizedRedone
        {
            get { return redoresize; }
            set { redoresize = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the delete command is executed.
        /// </summary>
        /// <value>
        /// <c>true</c> if delete command is executed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDeleteCommandExecuted
        {
            get { return deletecommandexe; }
            set { deletecommandexe = value; }
        }

        /// <summary>
        /// Gets or sets the delete count.
        /// </summary>
        /// <value>The delete count.</value>
        internal int DeleteCount
        {
            get { return delcount; }
            set { delcount = value; }
        }

        /// <summary>
        /// Gets or sets the node drag count.
        /// </summary>
        /// <value>The node drag count.</value>
        internal int NodeDragCount
        {
            get { return nodedragcount; }
            set { nodedragcount = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the MeasureOverride of <see cref="DiagramPage"/> is called.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is measure called; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMeasureCalled
        {
            get { return measured; }
            set { measured = value; }
        }

        #endregion

        #region Dependency Property


        /// <summary>
        /// Identifies the DrawingMode dependency property.
        /// </summary>
        /// 
        public DrawingMode DrawingMode
        {
            get { return (DrawingMode)GetValue(DrawingModeProperty); }
            set { SetValue(DrawingModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawingMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawingModeProperty =
            DependencyProperty.Register("DrawingMode", typeof(DrawingMode), typeof(DiagramView), new PropertyMetadata(DrawingMode.Default, new PropertyChangedCallback(OnDrawingModeChanged)));

        /// <summary>
        /// Identifies the ClearSelectionOnRightClick dependency property.
        /// </summary>
        /// 
        public Boolean ClearSelectionOnRightClick
        {
            get { return (Boolean)GetValue(ClearSelectionOnRightClickProperty); }
            set { SetValue(ClearSelectionOnRightClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClearSelectionOnRightClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearSelectionOnRightClickProperty =
            DependencyProperty.Register("ClearSelectionOnRightClick", typeof(Boolean), typeof(DiagramView), new PropertyMetadata(true));

        

       

        public static readonly DependencyProperty BoundaryConstraintsEnabledProperty = DependencyProperty.Register("BoundaryConstraintsEnabled", typeof(bool), typeof(DiagramView), new PropertyMetadata(false, new PropertyChangedCallback(OnPageActiveAreaEnabledChanged)));

        public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.Register("SizeToContent", typeof(bool), typeof(DiagramView), new PropertyMetadata(true, new PropertyChangedCallback(OnSizeToContentChanged)));

        public static readonly DependencyProperty OffPageBackgroundProperty =
           DependencyProperty.Register("OffPageBackground", typeof(Brush), typeof(DiagramView), new PropertyMetadata(null, new PropertyChangedCallback(OnOffPageBackgroundChanged)));
        public static readonly DependencyProperty BoundaryConstraintsAreaProperty =
           DependencyProperty.Register("BoundaryConstraintsArea", typeof(Rect), typeof(DiagramView), new PropertyMetadata(new Rect(0, 0, 0, 0), new PropertyChangedCallback(OnPageActiveAreaChanged)));

        public static readonly DependencyProperty EnableCachingProperty = DependencyProperty.Register("VirtualizationRelatedToLine", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(true, new PropertyChangedCallback(OnVirtualizationRelatedToLineChanged)));
        public static readonly DependencyProperty EnableVirtualizationProperty = DependencyProperty.Register("IsVirtualizationEnabled", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsVirtualizationEnabled)));

        public static readonly DependencyProperty SnapToHorizontalGridProperty = DependencyProperty.Register("SnapToHorizontalGrid", typeof(bool), typeof(DiagramView), new PropertyMetadata(false));
        public static readonly DependencyProperty SnapToVerticalGridProperty = DependencyProperty.Register("SnapToVerticalGrid", typeof(bool), typeof(DiagramView), new PropertyMetadata(false));
        public static readonly DependencyProperty SnapOffsetXProperty = DependencyProperty.Register("SnapOffsetX", typeof(double), typeof(DiagramView), new PropertyMetadata(25d));
        public static readonly DependencyProperty SnapOffsetYProperty = DependencyProperty.Register("SnapOffsetY", typeof(double), typeof(DiagramView), new PropertyMetadata(25d));

        public static readonly DependencyProperty IsCutEnabledProperty = DependencyProperty.Register("IsCutEnabledProperty", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(true));
        public static readonly DependencyProperty IsCopyEnabledProperty = DependencyProperty.Register("IsCopyEnabledProperty", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(true));
        public static readonly DependencyProperty IsPasteEnabledProperty = DependencyProperty.Register("IsPasteEnabledProperty", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the EnableConnection dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableConnectionProperty = DependencyProperty.Register("EnableConnection", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableConnectionChanged)));

        /// <summary>
        /// Identifies the IsZoomEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the IsPanEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register("IsPanEnabled", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsPanEnableChanged)));

        /// <summary>
        /// Identifies the PortVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty PortVisibilityProperty = DependencyProperty.RegisterAttached("PortVisibility", typeof(PortVisibility), typeof(DiagramView), new FrameworkPropertyMetadata(Diagram.PortVisibility.MouseOverNode, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty UndoRedoEnabledProperty = DependencyProperty.Register("UndoRedoEnabled", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(true));

        public bool EnableProportionalResize
        {
            get { return (bool)GetValue(EnableProportionalResizeProperty); }
            set { SetValue(EnableProportionalResizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableProportionalResize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableProportionalResizeProperty =
            DependencyProperty.Register("EnableProportionalResize", typeof(bool), typeof(DiagramView), new UIPropertyMetadata(false)); 



        public bool AnimationEnabled
        {
            get { return (bool)GetValue(AnimationEnabledProperty); }
            set { SetValue(AnimationEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomAnimationEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationEnabledProperty =
            DependencyProperty.Register("AnimationEnabled", typeof(bool), typeof(DiagramView), new PropertyMetadata(true, new PropertyChangedCallback(OnIsPageEditableChanged)));

        /// <summary>
        /// Identifies the Selected item.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPageEditableProperty = DependencyProperty.Register("IsPageEditable", typeof(bool), typeof(DiagramView), new PropertyMetadata(true, new PropertyChangedCallback(OnIsPageEditableChanged)));

        /// <summary>
        /// Identifies the Selected item.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(DiagramView));

        /// <summary>
        /// Identifies the Page property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty PageProperty = DependencyProperty.Register("Page", typeof(IDiagramPage), typeof(DiagramView));

        /// <summary>
        /// Identifies the Show Grid property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowGridProperty = DependencyProperty.Register("ShowGrid", typeof(bool), typeof(DiagramView), new PropertyMetadata(true, new PropertyChangedCallback(OnShowGridChanged)));

        /// <summary>
        /// Identifies the HorizontalGridLineStyle property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalGridLineStyleProperty = DependencyProperty.Register("HorizontalGridLineStyle", typeof(Pen), typeof(DiagramView), new PropertyMetadata(new Pen(Brushes.DarkGray, 0.3d), new PropertyChangedCallback(OnHorizontalLineStyleChanged)));

        /// <summary>
        /// Identifies the VerticalGridLineStyle property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalGridLineStyleProperty = DependencyProperty.Register("VerticalGridLineStyle", typeof(Pen), typeof(DiagramView), new PropertyMetadata(new Pen(Brushes.DarkGray, 0.3d), new PropertyChangedCallback(OnVerticalLineStyleChanged)));

        /// <summary>
        /// Identifies the HorizontalRuler property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalRulerProperty = DependencyProperty.Register("HorizontalRuler", typeof(HorizontalRuler), typeof(DiagramView), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the VerticalRuler property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalRulerProperty = DependencyProperty.Register("VerticalRuler", typeof(VerticalRuler), typeof(DiagramView), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ShowHorizontalGridLine property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowHorizontalGridLineProperty = DependencyProperty.Register("ShowHorizontalGridLine", typeof(bool), typeof(DiagramView), new PropertyMetadata(false, new PropertyChangedCallback(OnShowHLineChanged)));

        /// <summary>
        /// Identifies the ShowVerticalGridLine property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowVerticalGridLineProperty = DependencyProperty.Register("ShowVerticalGridLine", typeof(bool), typeof(DiagramView), new PropertyMetadata(false, new PropertyChangedCallback(OnShowVLineChanged)));

        /// <summary>
        /// Identifies the ShowHorizontalRuler property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowHorizontalRulerProperty = DependencyProperty.Register("ShowHorizontalRulers", typeof(bool), typeof(DiagramView), new PropertyMetadata(true, new PropertyChangedCallback(OnShowHRulerChanged)));

        /// <summary>
        /// Identifies the ShowVerticalRuler property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowVerticalRulerProperty = DependencyProperty.Register("ShowVerticalRulers", typeof(bool), typeof(DiagramView), new PropertyMetadata(true, new PropertyChangedCallback(OnShowVRulerChanged)));

        /// <summary>
        /// Identifies the ZoomFactor property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(DiagramView), new PropertyMetadata(.2d, new PropertyChangedCallback(OnZoomFactorChanged)));

        /// <summary>
        /// Identifies the CurrentZoom property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentZoomProperty = DependencyProperty.Register("CurrentZoom", typeof(double), typeof(DiagramView), new PropertyMetadata(1d, new PropertyChangedCallback(OnCurrentZoomChanged)));

        /// <summary>
        /// Identifies the ViewGridOrigin property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewGridOriginProperty = DependencyProperty.Register("ViewGridOrigin", typeof(Point), typeof(DiagramView), new PropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnViewGridOriginChanged)));

        /// <summary>
        /// Identifies the NodeContextMenu property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty NodeContextMenuProperty = DependencyProperty.Register("NodeContextMenu", typeof(ContextMenu), typeof(DiagramView), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the LineConnectorContextMenu property.This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LineConnectorContextMenuProperty = DependencyProperty.Register("LineConnectorContextMenu", typeof(ContextMenu), typeof(DiagramView), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the HorizontalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty GetHorizontalScrollBarVisibilityProperty = DependencyProperty.Register("GetHorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DiagramView), new UIPropertyMetadata(ScrollBarVisibility.Auto, new PropertyChangedCallback(OnHorizontalScrollBarVisibilityChanged)));

        /// <summary>
        /// Identifies the VerticalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty GetVerticalScrollBarVisibilityProperty = DependencyProperty.Register("GetVerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DiagramView), new UIPropertyMetadata(ScrollBarVisibility.Auto, new PropertyChangedCallback(OnVerticalScrollBarVisibilityChanged)));

        /// <summary>
        /// Identifies the HorizontalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DiagramView), new UIPropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Identifies the VerticalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DiagramView), new UIPropertyMetadata(ScrollBarVisibility.Auto));
        ///<summary>
        ///Enables the Drawingtool dependency property
        ///</summary>
        public static readonly DependencyProperty EnableDrawingToolsProperty = DependencyProperty.Register("EnableDrawingTools", typeof(bool), typeof(DiagramView), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableDrawingToolsChanged)));

        public static readonly DependencyProperty DrawingToolsProperty =
           DependencyProperty.Register("DrawingTool", typeof(DrawingTools), typeof(DiagramView), new UIPropertyMetadata(DrawingTools.Ellipse, new PropertyChangedCallback(OnDrawingToolsChanged)));

        public static readonly DependencyProperty ItemSelectionModeProperty = DependencyProperty.Register("ItemSelectionMode", typeof(ItemSelectionMode), typeof(DiagramView), new PropertyMetadata(ItemSelectionMode.Multiple, new PropertyChangedCallback(OnItemSelectionModeChanged)));

        public static readonly DependencyProperty NudgeIncrementProperty = DependencyProperty.Register("NudgeIncrement", typeof(double), typeof(DiagramView), new PropertyMetadata(1d));

        public static readonly DependencyProperty IsConnectorDragCancelProperty = DependencyProperty.Register("IsConnectorDragCancel", typeof(bool), typeof(DiagramView), new PropertyMetadata(false));

        public static readonly DependencyProperty IsNodeDragCancelProperty = DependencyProperty.Register("IsNodeDragCancel", typeof(bool), typeof(DiagramView), new PropertyMetadata(false));

        public static readonly DependencyProperty ZOrderModeProperty = DependencyProperty.Register("ZOrderMode", typeof(ZOrderModes), typeof(DiagramView), new PropertyMetadata(ZOrderModes.Visual));
        #endregion

        internal Cursor _oldcursor;
        internal bool _IsDragoverConnection;
        // This variable is no longer used 
        //internal bool IsPan;

        #region Events

        private static void OnEnableConnectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.EnableConnection)
            {
                view.EnableConnection = false;
                view.EnableDrawingTools = true;
                if((view.Page as DiagramPage).ConnectorType == ConnectorType.Straight)
                {
                    view.DrawingTool = DrawingTools.StraightLine;
                }
                if ((view.Page as DiagramPage).ConnectorType == ConnectorType.Orthogonal)
                {
                    view.DrawingTool = DrawingTools.OrthogonalLine;
                }
                if ((view.Page as DiagramPage).ConnectorType == ConnectorType.Bezier)
                {
                    view.DrawingTool = DrawingTools.BezierLine;
                }
                if ((view.Page as DiagramPage).ConnectorType == ConnectorType.Arc)
                {
                    view.DrawingTool = DrawingTools.Arc;
                }
                (view.Page as DiagramPage).IsPolyLineEnabled = false;               
            }
        }

        private static void OnEnableDrawingToolsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.EnableDrawingTools)
            {
                view.EnableConnection = false;
                (view.Page as DiagramPage).IsPolyLineEnabled = false;
            }

        }

        private static void OnDrawingToolsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view != null)
            {
                view.temppath = null;
            }
        }

        /// <summary>
        /// Called when [is page editable changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsPageEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView.PageEdit = true;
            DiagramView view = (DiagramView)d;
            if (!view.IsPageEditable)
            {
                DiagramView.PageEdit = false;
                (view.Page as DiagramPage).SelectionList.Clear();
            }
                (view.Page as DiagramPage).InvalidateVisual();
        
        }

        private static void OnAnimationEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view._contentHolder != null)
                view._contentHolder.AnimationEnabled = view.AnimationEnabled;
        }
        private static void OnItemSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            view.Selection();
        }

        private static void OnIsPanEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView) d;
            if (!view.IsPanEnabled)
            {
                if (view.ScrollGrid != null && view.ScrollGrid._MasterParent != null)
                {
                    view.ScrollGrid.m_height = new Size(0, 0);
                    OverviewContentHolder.SetOrigin(view.ScrollGrid,
                        new Point(-(view.Page as DiagramPage).Left, -(view.Page as DiagramPage).Top));
                    view.ScrollGrid._MasterParent.InvalidateMeasure();
                    (view.Page as DiagramPage).InvalidateVisual();
                    view.ScrollGrid._MasterParent.InvalidateVisual();
                }
            }
        }

        /// <summary>
        /// Calls OnShowGridChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.mViewGrid != null)
            {
                view.mViewGrid.InvalidateVisual();
            }
        }

        /// <summary>
        /// Calls OnGridHOffsetChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGridHOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.mViewGrid != null)
            {
                view.mViewGrid.InvalidateVisual();
            }
        }

        /// <summary>
        /// Calls OnGridVOffsetChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGridVOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.mViewGrid != null)
            {
                view.mViewGrid.InvalidateVisual();
            }
        }

        /// <summary>
        /// Calls OnHorizontalLineStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHorizontalLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.mViewGrid != null)
            {
                view.mViewGrid.InvalidateVisual();
            }
        }

        #region BoundaryArea members
        private static void OnVirtualizationRelatedToLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view != null)
            {
                if (view.EnableCaching)
                {
                    if (view.ScrollGrid != null)
                    {
                        // view.ScrollGrid.VirtualizationRelatedToLine();
                    }
                }
            }
        }
        public Rect BoundaryConstraintsArea
        {
            get
            {
                return (Rect)GetValue(BoundaryConstraintsAreaProperty);
            }

            set
            {
                SetValue(BoundaryConstraintsAreaProperty, value);
            }

        }
        public bool BoundaryConstraintsEnabled
        {
            get
            {
                return (bool)GetValue(BoundaryConstraintsEnabledProperty);
            }

            set
            {
                SetValue(BoundaryConstraintsEnabledProperty, value);
            }
        }
        public bool SizeToContent
        {
            get
            {
                return (bool)GetValue(SizeToContentProperty);
            }

            set
            {
                SetValue(SizeToContentProperty, value);
            }
        }
        public Brush OffPageBackground
        {
            get { return (Brush)GetValue(OffPageBackgroundProperty); }
            set { SetValue(OffPageBackgroundProperty, value); }
        }
        public Effect BackgroundEffect
        {
            get
            {
                return (Effect)GetValue(BackgroundEffectProperty);
            }

            set
            {
                SetValue(BackgroundEffectProperty, value);
            }

        }
        private static void OnBackgroundEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        internal bool mactiveAreachanged = false;
        private static void OnPageActiveAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            DiagramView view = (DiagramView)d;
            view.mactiveAreachanged = true;
            view.UpdateLayout();
        }

        private static void OnDrawingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            
        }
        private static void OnOffPageBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view != null && view.ScrollGrid != null)
            {
                view.ScrollGrid.Background = view.OffPageBackground;
            }
            view.UpdateLayout();
        }
        private static void OnFitToPageEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
                 DiagramView view=(DiagramView)d;
                 if (view._contentHolder != null)
                 {
                     view._contentHolder.EnableFitToPage = view.EnableFitToPage;
                     view._contentHolder.InvalidateArrange();
                 }
        }
        private static void OnPageActiveAreaEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView.PageEdit = true;
            DiagramView view = (DiagramView)d;
            //if (view.background != null)
            //{
            //    //view.background.Margin = new Thickness(view.PageActiveArea.Left,view.PageActiveArea.Top,0,0);
            //}
        }
        private static void OnSizeToContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.BoundaryConstraintsArea == new Rect(0, 0, 0, 0))
            {
                view.BoundaryConstraintsArea = new Rect((view.Page as DiagramPage).Left - view.PageMargin.Left, (view.Page as DiagramPage).Top - view.PageMargin.Top, (view.Page as DiagramPage).Right - view.PageMargin.Right, (view.Page as DiagramPage).Bottom - view.PageMargin.Bottom);
            }
        }
        private static void OnIsVirtualizationEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view != null && view.EnableVirtualization)
            {
                if (view.ScrollGrid != null)
                {
                    view.ScrollGrid.callCalculate();
                }
            }
            else if (view != null && view.ScrollGrid != null)
            {
                view.ScrollGrid.CallNormailzation();
            }
        }
        #endregion
        /// <summary>
        /// Calls OnVerticalLineStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnVerticalLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.mViewGrid != null)
            {
                view.mViewGrid.InvalidateVisual();
            }
        }

        /// <summary>
        /// Calls OnShowHLineChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowHLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.mViewGrid != null)
            {
                view.mViewGrid.InvalidateVisual();
            }
        }

        public void FitToPage()
        {
            if (this.ScrollGrid != null && this.ScrollGrid._MasterParent != null)
            {
                this.ScrollGrid._MasterParent.FitToPage();
            }
        }

        /// <summary>
        /// Calls OnShowVLineChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowVLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            if (view.mViewGrid != null)
            {
                view.mViewGrid.InvalidateVisual();
            }
        }

        /// <summary>
        /// Calls OnShowHRulerChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowHRulerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Calls OnShowVRulerChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowVRulerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Calls OnZoomFactorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            view.InvalidateVisual();
        }

        /// <summary>
        /// Calls OnCurrentZoomChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCurrentZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            view.UpdateRuler(view);
        }

        internal void InvalidateViewGrid()
        {
            this.LayoutUpdated += new EventHandler(DiagramView_LayoutUpdated);
        }

        void DiagramView_LayoutUpdated(object sender, EventArgs e)
        {
            this.LayoutUpdated -= new EventHandler(DiagramView_LayoutUpdated);
            UpdateViewGridOrigin();
        }

        internal void UpdateViewGridOrigin()
        {
            if (ScrollGrid != null)
                ScrollGrid.InvalidateArrange();
            //ViewGridOrigin = Page.TranslatePoint(new Point(0, 0), Scrollviewer);
            DiagramControl dc = DiagramPage.GetDiagramControl(this);
            if (dc.Model != null)
            {
                    if (dc.Model.Nodes.Count > 0 || dc.Model.Connections.Count > 0)
                    {
                        double MinLeft = 0d;
                        double MinTop = 0d;
                        double MaxRight = 0d;
                        double MaxBottom = 0d;
                        List<double> leftValueCollection = new List<double>();
                        List<double> topValueCollection = new List<double>();
                        List<double> rightValueCollection = new List<double>();
                        List<double> bottomValueCollection = new List<double>();
             
                        foreach (UIElement control in dc.View.Page.Children)
                        {
                            if (control is Node)
                            {
                                leftValueCollection.Add((control as Node).RectBounds.Left);
                                topValueCollection.Add((control as Node).RectBounds.Top);
                                rightValueCollection.Add((control as Node).RectBounds.Right);
                                bottomValueCollection.Add((control as Node).RectBounds.Bottom);
                            }
                            else if (control is LineConnector)
                            {
                                leftValueCollection.Add((control as LineConnector).GetBounds().Left);
                                topValueCollection.Add((control as LineConnector).GetBounds().Top);
                                rightValueCollection.Add((control as LineConnector).GetBounds().Right);
                                bottomValueCollection.Add((control as LineConnector).GetBounds().Bottom);
                            }
                        }

                        if(leftValueCollection.Count>0)
                            MinLeft = MeasureUnitsConverter.FromPixels(leftValueCollection.ToList<double>().Min(), (this.Page as DiagramPage).MeasurementUnits);
                        if (topValueCollection.Count > 0)
                            MinTop = MeasureUnitsConverter.FromPixels(topValueCollection.ToList<double>().Min(), (this.Page as DiagramPage).MeasurementUnits);
                        if(rightValueCollection.Count>0)
                            MaxRight = MeasureUnitsConverter.FromPixels(rightValueCollection.ToList<double>().Max(), (this.Page as DiagramPage).MeasurementUnits);
                        if(bottomValueCollection.Count>0)
                            MaxBottom = MeasureUnitsConverter.FromPixels(bottomValueCollection.ToList<double>().Max(), (this.Page as DiagramPage).MeasurementUnits);
                        leftValueCollection = null;
                        topValueCollection = null;
                        rightValueCollection = null;
                        bottomValueCollection = null;

                        if (this.BoundaryConstraintsArea.X >= 0 || this.BoundaryConstraintsArea.Y >= 0)
                        {
                            if (MinLeft < 0 || MinTop < 0)
                            {
                                ViewGridOrigin = Page.TranslatePoint(new Point(0, 0), Scrollviewer);
                            }
                            else
                            {
                                if (MaxRight > MeasureUnitsConverter.FromPixels(Scrollviewer.ViewportWidth, (this.Page as DiagramPage).MeasurementUnits) || MaxBottom > MeasureUnitsConverter.FromPixels(Scrollviewer.ViewportHeight, (this.Page as DiagramPage).MeasurementUnits))
                                {
                                    ViewGridOrigin = Page.TranslatePoint(new Point(0, 0), Scrollviewer);
                                }
                                else
                                    ViewGridOrigin = new Point(0, 0);
                            }
                        }
                        else
                        {
                            ViewGridOrigin = Page.TranslatePoint(new Point(0, 0), Scrollviewer);
                        }
                    }
                    else
                    {
                        ViewGridOrigin = Page.TranslatePoint(new Point(0, 0), Scrollviewer);
                    }
            }
        }

        internal Rect CurrentViewport()
        {
            double Negativeaxistop = -(dc.View.Page as DiagramPage).Top;
            double NegativeaxisLeft = -(dc.View.Page as DiagramPage).Left;
            ScrollViewer _ScrollOwner = dc.View.Scrollviewer;
            Rect viewablesize = new Rect();
            if (NegativeaxisLeft < 0)
            {
                if (Negativeaxistop < 0)
                {

                    viewablesize = new Rect(_ScrollOwner.HorizontalOffset + NegativeaxisLeft, _ScrollOwner.VerticalOffset + Negativeaxistop, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                }
                else
                {
                    viewablesize = new Rect(_ScrollOwner.HorizontalOffset + NegativeaxisLeft, _ScrollOwner.VerticalOffset, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                }
            }

            else if (Negativeaxistop < 0)
            {
                viewablesize = new Rect(_ScrollOwner.HorizontalOffset, _ScrollOwner.VerticalOffset + Negativeaxistop, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
            }
            else
            {
                viewablesize = new Rect(_ScrollOwner.HorizontalOffset, _ScrollOwner.VerticalOffset, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
            }

            return viewablesize;
        }

        internal List<Node> ViewportNodes()
        {
            Rect viewablesize = dc.View.CurrentViewport();
            List<Node> nodecollection = new List<Node>();
            foreach (Node node in dc.Model.Nodes)
            {
                Rect nodebounds = new Rect(node.PxOffsetX, node.PxOffsetY, node._Width, node._Height);
                if (viewablesize.IntersectsWith(nodebounds))
                {
                    nodecollection.Add(node);
                }
            }
            return nodecollection;
        }

        internal Point panPoint = new Point(0, 0);


        //internal void Pan(Point tempoint, Point start)
        //{
        //    (this.Scrollviewer.Content as OverviewContentHolder).IsZoomResetEnabled = false;
        //    if (this.CurrentZoom == 1)
        //    {
        //        (Scrollviewer.Content as OverviewContentHolder)._Offset.X = -(tempoint.X - start.X);

        //        (Scrollviewer.Content as OverviewContentHolder)._Offset.Y = -(tempoint.Y - start.Y);

        //        if ((Scrollviewer.Content as OverviewContentHolder)._Offset.X < 0)
        //        {
        //            panPoint.X = -(Scrollviewer.Content as OverviewContentHolder)._Offset.X;
        //        }

        //        if ((Scrollviewer.Content as OverviewContentHolder)._Offset.Y < 0)
        //        {
        //            panPoint.Y = -(Scrollviewer.Content as OverviewContentHolder)._Offset.Y;
        //        }

        //    }
        //    else if (this.CurrentZoom > 1)
        //    {
        //        (Scrollviewer.Content as OverviewContentHolder)._Offset.X = -(tempoint.X / CurrentZoom - start.X);
        //        (Scrollviewer.Content as OverviewContentHolder)._Offset.Y = -(tempoint.Y / CurrentZoom - start.Y);
        //        if ((Scrollviewer.Content as OverviewContentHolder)._Offset.X < 0)
        //        {
        //            panPoint.X = -(Scrollviewer.Content as OverviewContentHolder)._Offset.X;
        //        }
        //        if ((Scrollviewer.Content as OverviewContentHolder)._Offset.Y < 0)
        //        {
        //            panPoint.Y = -(Scrollviewer.Content as OverviewContentHolder)._Offset.Y;
        //        }

        //    }
        //    else if (this.CurrentZoom < 1)
        //    {
        //        (Scrollviewer.Content as OverviewContentHolder)._Offset.X = -(tempoint.X / CurrentZoom - start.X);
        //        (Scrollviewer.Content as OverviewContentHolder)._Offset.Y = -(tempoint.Y / CurrentZoom - start.Y);
        //        if ((Scrollviewer.Content as OverviewContentHolder)._Offset.X < 0)
        //        {
        //            panPoint.X = -(Scrollviewer.Content as OverviewContentHolder)._Offset.X;
        //        }
        //        if ((Scrollviewer.Content as OverviewContentHolder)._Offset.Y < 0)
        //        {
        //            panPoint.Y = -(Scrollviewer.Content as OverviewContentHolder)._Offset.Y;
        //        }
        //    }


        //    (Scrollviewer.Content as OverviewContentHolder).InvalidateArrange();
        //    Page.InvalidateMeasure();
        //}

        /// <summary>
        /// Gets the rounding value for the measurement units.
        /// </summary>
        /// <returns>The rounding value</returns>
        private double GetRounding()
        {
            switch ((this.Page as DiagramPage).MeasurementUnits)
            {
                case MeasureUnits.Centimeter:
                    return 1;
                case MeasureUnits.Display:
                    return 40;
                case MeasureUnits.Document:
                    return 150;
                case MeasureUnits.EighthInch:
                    return 4;
                case MeasureUnits.Foot:
                    return .04;
                case MeasureUnits.HalfInch:
                    return 1;
                case MeasureUnits.Inch:
                    return .5;
                case MeasureUnits.Kilometer:
                    return 0.00002;
                case MeasureUnits.Meter:
                    return 0.01;
                case MeasureUnits.Mile:
                    return .00001;
                case MeasureUnits.Millimeter:
                    return 13;
                case MeasureUnits.Pixel:
                    return 50;
                case MeasureUnits.Point:
                    return 40;
                case MeasureUnits.QuarterInch:
                    return 2;
                case MeasureUnits.SixteenthInch:
                    return 8;
                case MeasureUnits.Yard:
                    return .01;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the position when the interval is default.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <returns>The position</returns>
        private double GetDefaultPosition(double x)
        {
            double c = MeasureUnitsConverter.FromPixels(50, (this.Page as DiagramPage).MeasurementUnits);

            double mul = (50 * x) / c;
            switch ((this.Page as DiagramPage).MeasurementUnits)
            {
                case MeasureUnits.Centimeter:
                    return mul * 1;
                case MeasureUnits.Display:
                    return mul * 40;
                case MeasureUnits.Document:
                    return mul * 150;
                case MeasureUnits.EighthInch:
                    return mul * 4;
                case MeasureUnits.Foot:
                    return mul * 0.04;
                case MeasureUnits.HalfInch:
                    return mul * 1;
                case MeasureUnits.Inch:
                    return mul * .5;
                case MeasureUnits.Kilometer:
                    return mul * 0.00002;
                case MeasureUnits.Meter:
                    return mul * 0.01;
                case MeasureUnits.Mile:
                    return mul * .00001;
                case MeasureUnits.Millimeter:
                    return mul * 13;
                case MeasureUnits.Pixel:
                    return mul * 50;
                case MeasureUnits.Point:
                    return mul * 40;
                case MeasureUnits.QuarterInch:
                    return mul * 2;
                case MeasureUnits.SixteenthInch:
                    return mul * 8;
                case MeasureUnits.Yard:
                    return mul * .01;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the position in the current unit interval.
        /// </summary>
        /// <returns>The position </returns>
        private double GetPosition()
        {
            double mul = 0;
            double c = MeasureUnitsConverter.FromPixels(50, (this.Page as DiagramPage).MeasurementUnits);

            mul = this.pixelvalue / c;
            switch ((this.Page as DiagramPage).MeasurementUnits)
            {
                case MeasureUnits.Centimeter:
                    return mul * 1;
                case MeasureUnits.Display:
                    return mul * 40;
                case MeasureUnits.Document:
                    return mul * 150;
                case MeasureUnits.EighthInch:
                    return mul * 4;
                case MeasureUnits.Foot:
                    return mul * 0.04;
                case MeasureUnits.HalfInch:
                    return mul;
                case MeasureUnits.Inch:
                    return mul * .5;
                case MeasureUnits.Kilometer:
                    return mul * 0.00002;
                case MeasureUnits.Meter:
                    return mul * 0.01;
                case MeasureUnits.Mile:
                    return mul * .00001;
                case MeasureUnits.Millimeter:
                    return mul * 13;
                case MeasureUnits.Pixel:
                    return mul * 50;
                case MeasureUnits.Point:
                    return mul * 40;
                case MeasureUnits.QuarterInch:
                    return mul * 2;
                case MeasureUnits.SixteenthInch:
                    return mul * 8;
                case MeasureUnits.Yard:
                    return mul * .01;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the vertical ruler position
        /// </summary>
        /// <returns>The vertical ruler position</returns>
        private double GetVerticalPosition()
        {
            double mul = 0;
            double c = MeasureUnitsConverter.FromPixels(50, (this.Page as DiagramPage).MeasurementUnits);

            mul = this.pixelvalue / c;
            switch ((this.Page as DiagramPage).MeasurementUnits)
            {
                case MeasureUnits.Centimeter:
                    return mul * 1;
                case MeasureUnits.Display:
                    return mul * 40;
                case MeasureUnits.Document:
                    return mul * 150;
                case MeasureUnits.EighthInch:
                    return mul * 4;
                case MeasureUnits.Foot:
                    return mul * 0.04;
                case MeasureUnits.HalfInch:
                    return mul;
                case MeasureUnits.Inch:
                    return mul * .5;
                case MeasureUnits.Kilometer:
                    return mul * 0.00002;
                case MeasureUnits.Meter:
                    return mul * 0.01;
                case MeasureUnits.Mile:
                    return mul * .00001;
                case MeasureUnits.Millimeter:
                    return mul * 13;
                case MeasureUnits.Pixel:
                    return mul * 50;
                case MeasureUnits.Point:
                    return mul * 40;
                case MeasureUnits.QuarterInch:
                    return mul * 2;
                case MeasureUnits.SixteenthInch:
                    return mul * 8;
                case MeasureUnits.Yard:
                    return mul * .01;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converts the value from pixels to the current measurement unit.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The Converted value</returns>
        internal double ConvertValue(double value)
        {
            double b = MeasureUnitsConverter.FromPixels(value, (this.Page as DiagramPage).MeasurementUnits);
            return b;
        }

        /// <summary>
        /// Updates the rulers.
        /// </summary>
        /// <param name="view">DiagramView instance</param>
        public void UpdateRuler(DiagramView view)
        {
            if (view.onReset)
            {
                view.i = 1;
                view.j = 1;
                view.morethanone = true;
            }

            try
            {
                if (this.VerticalRuler != null && VisualTreeHelper.GetChildrenCount(this.VerticalRuler) > 0)
                {
                    Border b = (Border)VisualTreeHelper.GetChild(this.VerticalRuler, 0);
                    Grid g = (Grid)VisualTreeHelper.GetChild(b, 0);
                    Grid g1 = (Grid)VisualTreeHelper.GetChild(g, 0);
                    vertickbar = (TickBar)VisualTreeHelper.GetChild(g1, 0);
                }

                if (this.HorizontalRuler != null && VisualTreeHelper.GetChildrenCount(this.HorizontalRuler) > 0)
                {
                    Border b1 = (Border)VisualTreeHelper.GetChild(this.HorizontalRuler, 0);
                    Grid g2 = (Grid)VisualTreeHelper.GetChild(b1, 0);
                    Grid g3 = (Grid)VisualTreeHelper.GetChild(g2, 0);
                    hortickbar = (TickBar)VisualTreeHelper.GetChild(g3, 0);
                }

                view.CurrentZoom = Math.Round(view.CurrentZoom, 4);

                if (view.CurrentZoom >= 1 && view.CurrentZoom < 3)
                {
                    if (view.hortickbar != null)
                    {
                        view.hortickbar.OriginalInterval = GetDefaultPosition(1);
                    }

                    if (view.vertickbar != null)
                    {
                        view.vertickbar.OriginalInterval = GetDefaultPosition(1);
                    }

                    view.lessthanone = true;
                    view.pixelvalue = 50;
                    view.firstzoom = false;
                    leveloneexe = true;
                    if (view.morethanone)
                    {
                        if (j != 0)
                        {
                            j++;
                        }

                        if (count != 0)
                        {
                            count++;
                        }

                        view.morethanone = false;
                    }
                }

                if (view.CurrentZoom < 1)
                {
                    leveloneexe = true;
                    if (view.hortickbar != null)
                    {
                        view.hortickbar.OriginalInterval = GetDefaultPosition(3);
                    }

                    if (view.vertickbar != null)
                    {
                        view.vertickbar.OriginalInterval = GetDefaultPosition(3);
                    }

                    view.morethanone = true;
                    view.firstzoom = true;
                    view.pixelvalue = 50;

                    if (view.lessthanone)
                    {
                        i = 1;
                        j = 0;
                        view.lessthanone = false;
                    }
                }

                if (view.i <= 3 && !(view.i < 0) && !leveloneexe)
                {
                    if ((view.CurrentZoom >= (3 * view.i)) && (view.CurrentZoom < 30))
                    {
                        if (view.firstzoom)
                        {
                            if (view.hortickbar != null)
                            {
                                view.hortickbar.OriginalInterval = view.hortickbar.OriginalInterval / 4;
                            }

                            if (view.vertickbar != null)
                            {
                                view.vertickbar.OriginalInterval = view.vertickbar.OriginalInterval / 4;
                            }

                            view.firstzoom = false;
                            view.pixelvalue /= 4;
                        }
                        else
                        {
                            if (view.hortickbar != null)
                            {
                                view.hortickbar.OriginalInterval = view.hortickbar.OriginalInterval / 2;
                            }

                            if (view.vertickbar != null)
                            {
                                view.vertickbar.OriginalInterval = view.vertickbar.OriginalInterval / 2;
                            }

                            view.pixelvalue /= 2;
                        }

                        if (count != 0)
                        {
                            view.count--;
                        }

                        view.i++;
                        if (view.j != 0)
                        {
                            view.j--;
                        }

                        view.morethanone = true;
                        view.lessthanone = true;
                        zoominexe = true;
                    }
                }

                if (view.j <= 3 && !(view.j < 0) && (!zoominexe) && !leveloneexe)
                {
                    if ((view.CurrentZoom <= (3 * (view.i - 1))) && view.CurrentZoom > 1)
                    {
                        if (view.hortickbar != null)
                        {
                            view.hortickbar.OriginalInterval = view.hortickbar.OriginalInterval * 2;
                        }

                        if (view.vertickbar != null)
                        {
                            view.vertickbar.OriginalInterval = view.vertickbar.OriginalInterval * 2;
                        }

                        view.pixelvalue = 50;

                        view.j++;
                        view.count--;
                        if (view.i != 0)
                        {
                            view.i--;
                        }

                        view.lessthanone = true;
                    }
                }

                zoominexe = false;
                leveloneexe = false;
                if (view.hortickbar != null)
                {
                    if (HorizontalRuler.ShowDateTime)
                    {
                        view.hortickbar.OriginalInterval = this.DateTimeSettings.RulerInterval.ToPixel(DateTimeSettings);
                    }
                    view.hortickbar.Interval = view.hortickbar.OriginalInterval * view.CurrentZoom;
                }

                if (view.vertickbar != null)
                {
                    if (VerticalRuler.ShowDateTime)
                    {
                        view.vertickbar.OriginalInterval = this.DateTimeSettings.RulerInterval.ToPixel(DateTimeSettings);
                    }
                    view.vertickbar.Interval = view.vertickbar.OriginalInterval * view.CurrentZoom;
                }
            }
            catch
            {
            }
            viewgrid.InvalidateArrange();
        }

        /// <summary>
        /// Called when [view grid origin changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnViewGridOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            ViewGridOriginChanged = true;
            view.InvalidateVisual();
            if (view.vertickbar != null)
            {
                view.vertickbar.InvalidateVisual();
            }

            if (view.hortickbar != null)
            {
                view.hortickbar.InvalidateVisual();
            }
        }

        /// <summary>
        /// Called when [horizontal scroll bar visibility changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHorizontalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            view.UpdateViewGridOrigin();
        }

        /// <summary>
        /// Called when [vertical scroll bar visibility changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVerticalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramView view = (DiagramView)d;
            view.UpdateViewGridOrigin();
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Calls property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when the appropriate property changes.
        /// </summary>
        /// <param name="name">The property name</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Implementation

        private LineConnector currentHitConnector;

        internal void SetHitConnector(LineConnector line)
        {
            if (currentHitConnector != line)
            {
                if (currentHitConnector != null)
                {
                    currentHitConnector.IsDragConnectionOver = false;
                }
                if (line != null)
                {
                    line.IsDragConnectionOver = true;
                }
                currentHitConnector = line;
            }
        }
        
        private ConnectionPort currentHitPort;

        internal void SetHitPort(ConnectionPort port)
        {
            if (currentHitPort != port)
            {
                if (currentHitPort != null)
                {
                    currentHitPort.IsDragOverPort = false;
                }
                if (port != null)
                {
                    port.IsDragOverPort = true;
                }
                currentHitPort = port;
            }
        }

        internal ConnectionPort GetHitPort()
        {
            return currentHitPort;
        }

        internal LineConnector GetHitConnector()
        {
            return currentHitConnector;
        }

        /// <summary>
        /// Scrolls to the specified node.
        /// </summary>
        /// <param name="node">The node object.</param>
        public void ScrollToNode(Node node)
        {
            double left = 0;
            double top = 0;
            ScrollViewer scroll = this.Template.FindName("PART_ScrollViewer", this) as ScrollViewer;
            double x = 0;
            double y = 0;         

            left = (this.Page as DiagramPage).Left + scroll.HorizontalOffset;
            top = (this.Page as DiagramPage).Top + scroll.VerticalOffset;

            Rect rect = new Rect(left, top, scroll.ViewportWidth, scroll.ViewportHeight);
            Rect rectNode = new Rect((node as Node).OffsetX, (node as Node).OffsetY, (node as Node).Width, (node as Node).Height);

            if (scroll != null)
            {
                    if (!rect.IntersectsWith(rectNode))
                    {
                        x = (node as Node).OffsetX + ((node as Node).Width) - (scroll.ViewportWidth);
                        y = (node as Node).OffsetY + ((node as Node).Height) - (scroll.ViewportHeight);
                        scroll.ScrollToHorizontalOffset(x);
                        scroll.ScrollToVerticalOffset(y);
                    }
            }
        }
        /// <summary>
        /// Bring the specified eleement into center or rect value.
        /// </summary>
        public void BringIntoCenter(Object pageElement)
        {
            ScrollViewer scroll = this.Template.FindName("PART_ScrollViewer", this) as ScrollViewer;
            double x = 0;
            double y = 0;
            if (scroll != null)
            {
                if (pageElement is Node)
                {
                    x = (this.Page as DiagramPage).Left + (pageElement as Node).OffsetX + ((pageElement as Node).Width / 2) - (scroll.ViewportWidth / 2);
                    y = (this.Page as DiagramPage).Top + (pageElement as Node).OffsetY + ((pageElement as Node).Height / 2) - (scroll.ViewportHeight / 2);
                }
                if (pageElement is LineConnector)
                {
                    double width = Math.Abs((pageElement as LineConnector).EndPointPosition.X - (pageElement as LineConnector).StartPointPosition.X);
                    double height = Math.Abs((pageElement as LineConnector).StartPointPosition.Y - (pageElement as LineConnector).StartPointPosition.Y);
                    x = (this.Page as DiagramPage).Left + (pageElement as LineConnector).StartPointPosition.X + (width / 2) - (scroll.ViewportWidth / 2);
                    y = (this.Page as DiagramPage).Top + (pageElement as LineConnector).StartPointPosition.Y + (height / 2) - (scroll.ViewportHeight / 2);
                }
                if (pageElement is Rect)
                {

                    Rect PageArea = (Rect)pageElement;
                    x = (this.Page as DiagramPage).Left + PageArea.X + (PageArea.Width / 2) - (scroll.ViewportWidth / 2);
                    y = (this.Page as DiagramPage).Top + PageArea.Y + (PageArea.Height / 2) - (scroll.ViewportHeight / 2);
                }

                if (_contentHolder != null)
                {
                    _contentHolder.SetHorizontalOffset(x);
                    _contentHolder.SetVerticalOffset(y);
                }
            }
        }
        public void ZoomIn(DiagramView dview)
        {
            if (dview.IsPageEditable && this.IsZoomEnabled)
            {
                //if (dview.onReset)
                //{
                //    dview.CurrentZoom = 1;
                //    dview.onReset = false;
                //}

                //dview.CurrentZoom += dview.ZoomFactor;
                //if (dview.CurrentZoom >= 30)
                //{
                //    dview.CurrentZoom = 30;
                //}
                //ZoomParamenter param = new ZoomParamenter();
                //param.ZoomPoint = new Point(0, 0);// Mouseposition;
                //param.ZoomFactor = 0.2;
                (dview.Scrollviewer.Content as OverviewContentHolder).ZoomIn.Execute(null);
                //dview.zoomTransform = new ScaleTransform(dview.CurrentZoom, dview.CurrentZoom);
                //dview.ViewGrid.LayoutTransform = dview.zoomTransform;
                //dview.UpdateLayout();
                //dview.UpdateViewGridOrigin();

            }
        }

        DispatcherTimer dispatcherTimer;

        /// <summary>
        /// Invoked when ZoomIn Command is Executed.
        /// </summary>
        /// <param name="dview">The diagramview instance.</param>
        internal void ZoomIn(DiagramView dview,Point Mouseposition)
        {
            if (dview.IsPageEditable && this.IsZoomEnabled)
            {

                ZoomParamenter param = new ZoomParamenter();
                //param.ZoomPoint = Mouseposition;
                //param.ZoomFactor = 0.2;
                (dview.Scrollviewer.Content as OverviewContentHolder).ZoomIn.Execute(dview.ScrollGrid);
                UpdateLineAdorner();
            }
        }

        void UpdateLineAdorner()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Tick -= dispatcherTimer_Tick;
                dispatcherTimer = null;
            }
            foreach (LineConnector line in dc.Model.Connections)
            {
                if (line.IsSelected && line.LineAdorner != null)
                {
                    line.LineAdorner.InvalidateMeasure();
                }
            }

        }

        /// <summary>
        /// Invoked when ZoomOut Command is Executed.
        /// </summary>
        /// <param name="dview">The diagramview instance.</param>
        public void ZoomOut(DiagramView dview)
        {
            if (dview.IsPageEditable && this.IsZoomEnabled)
            {
                //if (this.onReset)
                //{
                //    this.onReset = false;
                //}

                //double oldzoom = CurrentZoom;
                //CurrentZoom -= ZoomFactor;

                //if (CurrentZoom < .3)
                //{
                //    CurrentZoom = .3;
                //}

                //if (this.CurrentZoom >= 30)
                //{
                //    this.CurrentZoom = 30;
                //}
                (dview.Scrollviewer.Content as OverviewContentHolder).ZoomOut.Execute(dview.ScrollGrid);
                dview.UpdateViewGridOrigin();
                //this.zoomTransform = new ScaleTransform(CurrentZoom, CurrentZoom);
                //viewgrid.LayoutTransform = zoomTransform;

                //Page.UpdateLayout();
                //dview.UpdateLayout();
                UpdateLineAdorner();
            }
            
        }

        /// <summary>
        /// Invoked when Reset Command is Executed.
        /// </summary>
        /// <param name="dview">The diagramview instance.</param>
        public void Reset(DiagramView dview)
        {
            //dview.onReset = true;
            if (dview.IsPageEditable)
            {
                (dview.Scrollviewer.Content as OverviewContentHolder).IsZoomResetEnabled = true;
                //dview.panPoint = new Point(0, 0);
                //dview.Scrollviewer.ScrollToHorizontalOffset(0);
                //dview.Scrollviewer.ScrollToVerticalOffset(0);
                ////dview.CurrentZoom = 1;
                ////dview.zoomTransform = new ScaleTransform(1, 1);
                //(dview.Page as DiagramPage).Top = Math.Max((dview.Page as DiagramPage).Top, dview.panPoint.Y * 2);
                //(dview.Page as DiagramPage).Left = Math.Max((dview.Page as DiagramPage).Left, dview.panPoint.X * 2);
                //(dview.Scrollviewer.Content as OverviewContentHolder).IsZoomResetEnabled = true;
                (dview.Scrollviewer.Content as OverviewContentHolder).ZoomReset.Execute(dview.ScrollGrid);
                OverviewContentHolder.SetOrigin(this.ScrollGrid,new Point(-(Page as DiagramPage).Left,-(Page as DiagramPage).Top));
                //dview.ViewGrid.LayoutTransform = dview.zoomTransform;
                //dview.UpdateLayout();
                //dview.Page.InvalidateMeasure();
                UpdateLineAdorner();
            }
            dview.InvalidateViewGrid();
            dview.Page.InvalidateMeasure();
        }

        /// <summary>
        /// Invoked whenever a key is pressed and this control has the focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void DiagramView_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsPageEditable)
            {
                if (!(DirectionBehaviourEnabled))
                {
                    if (!(XCustomized(e.Key, e.KeyboardDevice.Modifiers)))
                    {
                        if (e.Key == Key.Space)
                        {
                            AllowMoveX = false;
                            _RunTimeMovement = true;

                        }
                    }

                    if (!(YCustomized(e.Key, e.KeyboardDevice.Modifiers)))
                    {
                        if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                        {
                            AllowMoveY = false;
                            _RunTimeMovement = true;
                        }
                    }

                }
                if (e.Key == Key.Escape)
                {
                    Reset(sender as DiagramView);
                }
                else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
                {
                    DiagramCommandManager.Undo.Execute(this.Page, this);
                }
                else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Y)
                {
                    DiagramCommandManager.RedoCommand(sender as DiagramView);
                }
                if (e.Key == Key.Delete)
                {
                    DiagramCommandManager.Delete.Execute(this.Page, this);
                }
            }
        }
        NodeMovementX CustomizedX = new NodeMovementX();
        NodeMovementY CustomizedY = new NodeMovementY();
        internal bool YCustomized(Key key, ModifierKeys ModKey)
        {
            if (NodeMovementY != NodeMovementY.None)
            {
                if (key == Key.Space || key == Key.Y)
                {
                    if (key == Key.Space)
                        CustomizedY = NodeMovementY.Space;
                    if (key == Key.Y)
                        CustomizedY = NodeMovementY.Y;
                    if (key == Key.Z && ModKey == ModifierKeys.Alt)
                    {
                        CustomizedY = NodeMovementY.AltZ;
                    }
                    if (key == Key.Y && ModKey == ModifierKeys.Alt)
                    {
                        CustomizedY = NodeMovementY.AltY;
                    }
                    NodeMovementY MoveY = (CustomizedY) & (NodeMovementY.Space | NodeMovementY.Y | NodeMovementY.AltZ | NodeMovementY.AltY);
                    if (MoveY == NodeMovementY)
                    {
                        _RunTimeMovement = true;
                        AllowMoveX = false;
                        AllowMoveY = true;
                    }
                }
                return true;
            }
            else return false;

        }

        internal bool XCustomized(Key key, ModifierKeys ModKey)
        {
            if (NodeMovementX != NodeMovementX.None)
            {
                if (key == Key.X || key == Key.Z || ModKey == ModifierKeys.Alt)
                {

                    if (key == Key.X)
                        CustomizedX = NodeMovementX.X;
                    if (key == Key.Z)
                        CustomizedX = NodeMovementX.Z;
                    if (ModKey == ModifierKeys.Alt)
                        CustomizedX = NodeMovementX.Alt;
                    NodeMovementX MoveX = CustomizedX & (NodeMovementX.Alt | NodeMovementX.X | NodeMovementX.Z);
                    if (MoveX == NodeMovementX)
                    {
                        _RunTimeMovement = true;
                        AllowMoveY = false;
                        AllowMoveX = true;
                    }
                }
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Invoked when MoveUp Command is Executed.
        /// </summary>
        /// <param name="dview">The DiagramView instance.</param>
        public static void MoveUp(DiagramView dview)
        {
            if (dview.IsPageEditable)
            {
                foreach (UIElement shape in dview.SelectionList)
                {
                    if (shape is Group)
                    {
                        //ProcessEvent(shape, 1, dview);
                        (shape as Group).PxOffsetY -= dview.PxNudgeIncrement;
                        (shape as Group).Focus();
                        foreach (INodeGroup n in (shape as Group).NodeChildren)
                        {
                            ProcessEvent(n, 1, dview);
                            if (n is Node)
                            {
                                (n as Node).PxOffsetY -= dview.PxNudgeIncrement;
                                (n as Node).Focus();
                            }
                            else if (n is LineConnector)
                            {
                                dview.DoPushOperations(n as LineConnector, dview);
                                (n as LineConnector).PxEndPointPosition = new Point((n as LineConnector).PxEndPointPosition.X, (n as LineConnector).PxEndPointPosition.Y - dview.PxNudgeIncrement);
                                (n as LineConnector).PxStartPointPosition = new Point((n as LineConnector).PxStartPointPosition.X, (n as LineConnector).PxStartPointPosition.Y - dview.PxNudgeIncrement);
                                if ((n as LineConnector).IntermediatePoints != null && (n as LineConnector).IntermediatePoints.Count > 0)
                                {
                                    for (int i = 0; i < (n as LineConnector).IntermediatePoints.Count; i++)
                                    {
                                        (n as LineConnector).IntermediatePoints[i] = new Point((n as LineConnector).IntermediatePoints[i].X, (n as LineConnector).IntermediatePoints[i].Y - dview.PxNudgeIncrement);
                                    }
                                }
                                (n as LineConnector).InvalidateConnectorPathGeometry();
                                (n as LineConnector).Focus();
                            }
                            ProcessEvent(shape, 1, dview);
                            (dview.Page as DiagramPage).InvalidateMeasure();
                            (dview.Page as DiagramPage).InvalidateArrange();
                        }
                    }
                    else
                    {
                       // ProcessEvent(shape, 1, dview);
                        if (shape is Node)
                        {
                            (shape as Node).PxOffsetY -= dview.PxNudgeIncrement;
                            (shape as Node).Focus();
                        }
                        else if (shape is LineConnector)
                        {
                            dview.DoPushOperations(shape as LineConnector, dview);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y - dview.PxNudgeIncrement);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y - dview.PxNudgeIncrement);
                            if ((shape as LineConnector).IntermediatePoints != null)
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X, (shape as LineConnector).IntermediatePoints[i].Y - dview.PxNudgeIncrement);
                                }
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                            (shape as LineConnector).Focus();
                        }
                        ProcessEvent(shape, 1, dview);
                        //(dview.Page as DiagramPage).Ver = -dview.Scrollviewer.VerticalOffset / dview.CurrentZoom;
                        (dview.Page as DiagramPage).InvalidateMeasure();
                        (dview.Page as DiagramPage).InvalidateArrange();
                    }
                }
                
            }
        }

        /// <summary>
        /// Clears the undo redo stack.
        /// </summary>
        public void ClearUndoRedoStack()
        {
            this.UndoStack.Clear();
            this.RedoStack.Clear();
        }

        private static void ProcessEvent(object n, int p, DiagramView dview)
        {
            if (n is Node)
            {
                //NodeNudgeEventArgs nudge = new NodeNudgeEventArgs(n as Node, p);
                NodeNudgeEventArgs nudge = new NodeNudgeEventArgs(n as Node, p, dview.PxNudgeIncrement);
                nudge.RoutedEvent = DiagramView.NodeMovedEvent;
                dview.RaiseEvent(nudge);
            }
            else if (n is LineConnector)
            {
                if ((n as LineConnector).HeadNode == null || (n as LineConnector).TailNode == null)
                {
                    LineNudgeEventArgs nudge = new LineNudgeEventArgs();
                    nudge = new LineNudgeEventArgs();
                    nudge.RoutedEvent = DiagramView.LineMovedEvent;
                    dview.RaiseEvent(nudge);
                }
            }
        }

        /*private static void ProvessEvent(object n)
        {
            NudgeEventArgs nudge = new NudgeEventArgs();
            if (n is Node)
            {
                nudge = new NudgeEventArgs(n as Node);
            }
            else if (n is LineConnector)
            {
                nudge = new NudgeEventArgs(n as LineConnector);
            }
            nudge.RoutedEvent = DiagramView.NodeMovedEvent;
            dview.RaiseEvent(nudge);
        }*/

        private void DoPushOperations(LineConnector line, DiagramView dview)
        {
            Point startpos = line.PxStartPointPosition;
            dview.UndoStack.Push(startpos);
            Point endpos = line.PxEndPointPosition;
            dview.UndoStack.Push(endpos);
            dview.UndoStack.Push("LineMoved");
            dview.UndoStack.Push(line);
            dview.UndoStack.Push(dview.SelectionList.Count);
            dview.UndoStack.Push("Dragged");
        }
        /// <summary>
        /// Invoked when MoveUp Command is Executed.
        /// </summary>
        /// <param name="dview">The DiagramView instance.</param>
        public static void MoveDown(DiagramView dview)
        {
            if (dview.IsPageEditable)
            {
                foreach (UIElement shape in dview.SelectionList)
                {
                    if (shape is Group)
                    {
                        //ProcessEvent(shape, 3, dview);
                        (shape as Group).PxOffsetY += dview.PxNudgeIncrement;
                        (shape as Group).Focus();
                        foreach (INodeGroup n in (shape as Group).NodeChildren)
                        {
                            ProcessEvent(n, 3, dview);
                            if (n is Node)
                            {
                                (n as Node).PxOffsetY += dview.PxNudgeIncrement;
                                (n as Node).Focus();
                            }
                            else if (n is LineConnector)
                            {
                                dview.DoPushOperations(n as LineConnector, dview);
                                (n as LineConnector).PxEndPointPosition = new Point((n as LineConnector).PxEndPointPosition.X, (n as LineConnector).PxEndPointPosition.Y + dview.PxNudgeIncrement);
                                (n as LineConnector).PxStartPointPosition = new Point((n as LineConnector).PxStartPointPosition.X, (n as LineConnector).PxStartPointPosition.Y + dview.PxNudgeIncrement);
                                if ((n as LineConnector).IntermediatePoints != null && (n as LineConnector).IntermediatePoints.Count > 0)
                                    for (int i = 0; i < (n as LineConnector).IntermediatePoints.Count; i++)
                                    {
                                        (n as LineConnector).IntermediatePoints[i] = new Point((n as LineConnector).IntermediatePoints[i].X, (n as LineConnector).IntermediatePoints[i].Y + dview.PxNudgeIncrement);
                                    }
                                (n as LineConnector).InvalidateConnectorPathGeometry();
                                (n as LineConnector).Focus();
                            }
                            ProcessEvent(shape, 3, dview);
                            (dview.Page as DiagramPage).InvalidateMeasure();
                            (dview.Page as DiagramPage).InvalidateArrange();
                        }
                    }
                    else
                    {
                        //ProcessEvent(shape, 3, dview);
                        if (shape is Node)
                        {
                            (shape as Node).PxOffsetY += dview.PxNudgeIncrement;
                            (shape as Node).Focus();
                        }

                        else if (shape is LineConnector)
                        {
                            dview.DoPushOperations(shape as LineConnector,dview);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X, (shape as LineConnector).PxEndPointPosition.Y + dview.PxNudgeIncrement);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X, (shape as LineConnector).PxStartPointPosition.Y + dview.PxNudgeIncrement);
                            if ((shape as LineConnector).IntermediatePoints != null)
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X, (shape as LineConnector).IntermediatePoints[i].Y + dview.PxNudgeIncrement);
                                }
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                            (shape as LineConnector).Focus();
                        }
                        ProcessEvent(shape, 3, dview);
                        (dview.Page as DiagramPage).InvalidateMeasure();
                        (dview.Page as DiagramPage).InvalidateArrange();
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when MoveUp Command is Executed.
        /// </summary>
        /// <param name="dview">The DiagramView instance.</param>
        public static void MoveLeft(DiagramView dview)
        {
            if (dview.IsPageEditable)
            {
                foreach (UIElement shape in dview.SelectionList)
                {
                    if (shape is Group)
                    {
                        //ProcessEvent(shape, 4, dview);
                        (shape as Group).PxOffsetX -= dview.PxNudgeIncrement;
                        (shape as Group).Focus();
                        foreach (INodeGroup n in (shape as Group).NodeChildren)
                        {
                            //ProcessEvent(n, 4, dview);
                            if (n is Node)
                            {
                                (n as Node).PxOffsetX -= dview.PxNudgeIncrement;
                                (n as Node).Focus();
                            }

                            else if (n is LineConnector)
                            {
                                dview.DoPushOperations(n as LineConnector, dview);
                                (n as LineConnector).PxEndPointPosition = new Point((n as LineConnector).PxEndPointPosition.X - dview.PxNudgeIncrement, (n as LineConnector).PxEndPointPosition.Y);
                                (n as LineConnector).PxStartPointPosition = new Point((n as LineConnector).PxStartPointPosition.X - dview.PxNudgeIncrement, (n as LineConnector).PxStartPointPosition.Y);
                                if ((n as LineConnector).IntermediatePoints != null)
                                    for (int i = 0; i < (n as LineConnector).IntermediatePoints.Count; i++)
                                    {
                                        (n as LineConnector).IntermediatePoints[i] = new Point((n as LineConnector).IntermediatePoints[i].X - dview.PxNudgeIncrement, (n as LineConnector).IntermediatePoints[i].Y);
                                    }
                                (n as LineConnector).InvalidateConnectorPathGeometry();
                                (n as LineConnector).Focus();
                            }
                            ProcessEvent(shape, 4, dview);
                            (dview.Page as DiagramPage).InvalidateMeasure();
                            (dview.Page as DiagramPage).InvalidateArrange();
                        }
                    }
                    else
                    {
                        //ProcessEvent(shape, 4, dview);
                        if (shape is Node)
                        {
                            (shape as Node).PxOffsetX -= dview.PxNudgeIncrement;
                            (shape as Node).Focus();
                        }

                        else if (shape is LineConnector)
                        {
                            dview.DoPushOperations(shape as LineConnector, dview);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X - dview.PxNudgeIncrement, (shape as LineConnector).PxEndPointPosition.Y);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X - dview.PxNudgeIncrement, (shape as LineConnector).PxStartPointPosition.Y);
                            if ((shape as LineConnector).IntermediatePoints != null)
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X - dview.PxNudgeIncrement, (shape as LineConnector).IntermediatePoints[i].Y);
                                }
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                            (shape as LineConnector).Focus();
                        }
                        ProcessEvent(shape, 4, dview);
                        (dview.Page as DiagramPage).InvalidateMeasure();
                        (dview.Page as DiagramPage).InvalidateArrange();
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when MoveRight Command is Executed.
        /// </summary>
        /// <param name="dview">The DiagramView instance.</param>
        public static void MoveRight(DiagramView dview)
        {
            if (dview.IsPageEditable)
            {
                foreach (UIElement shape in dview.SelectionList)
                {
                    if (shape is Group)
                    {
                        //ProcessEvent(shape, 2, dview);
                        (shape as Group).PxOffsetX += dview.PxNudgeIncrement;
                        (shape as Group).Focus();
                        foreach (INodeGroup n in (shape as Group).NodeChildren)
                        {
                            ProcessEvent(n, 2, dview);
                            if (n is Node)
                            {
                                (n as Node).PxOffsetX += dview.PxNudgeIncrement;
                                (n as Node).Focus();
                            }

                            else if (n is LineConnector)
                            {
                                dview.DoPushOperations(n as LineConnector, dview);
                                (n as LineConnector).PxEndPointPosition = new Point((n as LineConnector).PxEndPointPosition.X + dview.PxNudgeIncrement, (n as LineConnector).PxEndPointPosition.Y);
                                (n as LineConnector).PxStartPointPosition = new Point((n as LineConnector).PxStartPointPosition.X + dview.PxNudgeIncrement, (n as LineConnector).PxStartPointPosition.Y);
                                if ((n as LineConnector).IntermediatePoints != null)
                                    for (int i = 0; i < (n as LineConnector).IntermediatePoints.Count; i++)
                                    {
                                        (n as LineConnector).IntermediatePoints[i] = new Point((n as LineConnector).IntermediatePoints[i].X + dview.PxNudgeIncrement, (n as LineConnector).IntermediatePoints[i].Y);
                                    }
                                (n as LineConnector).InvalidateConnectorPathGeometry();
                                (n as LineConnector).Focus();
                            }
                            ProcessEvent(shape, 2, dview);
                            (dview.Page as DiagramPage).InvalidateMeasure();
                            (dview.Page as DiagramPage).InvalidateArrange();
                        }
                    }
                    else
                    {
                        //ProcessEvent(shape, 2, dview);
                        if (shape is Node)
                        {
                            (shape as Node).PxOffsetX += dview.PxNudgeIncrement;
                            (shape as Node).Focus();
                        }

                        else if (shape is LineConnector)
                        {
                            dview.DoPushOperations(shape as LineConnector, dview);
                            (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X + dview.PxNudgeIncrement, (shape as LineConnector).PxEndPointPosition.Y);
                            (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X + dview.PxNudgeIncrement, (shape as LineConnector).PxStartPointPosition.Y);
                            if ((shape as LineConnector).IntermediatePoints != null)
                                for (int i = 0; i < (shape as LineConnector).IntermediatePoints.Count; i++)
                                {
                                    (shape as LineConnector).IntermediatePoints[i] = new Point((shape as LineConnector).IntermediatePoints[i].X + dview.PxNudgeIncrement, (shape as LineConnector).IntermediatePoints[i].Y);
                                }
                            (shape as LineConnector).InvalidateConnectorPathGeometry();
                            (shape as LineConnector).Focus();
                        }
                        ProcessEvent(shape, 2, dview);
                        (dview.Page as DiagramPage).InvalidateMeasure();
                        (dview.Page as DiagramPage).InvalidateArrange();
                    }
                }
            }
        }

        /// <summary>
        /// Provides class handling for the PreviewMouseWheel routed event that occurs when the mouse
        /// wheel  is moved and the mouse pointer is over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void DiagramView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //DupDeleted = false;
            //if (IsPageEditable)
            //{
            //    if (IsZoomEnabled)
            //    {
            //        if (this.onReset)
            //        {
            //            this.onReset = false;
            //        }

            //        double oldzoom = CurrentZoom;
            //        if (Keyboard.Modifiers == ModifierKeys.Control)
            //        {
            //            if (e.Delta > 0)
            //            {
                            
            //                ZoomIn(this,e.MouseDevice.GetPosition(this.Page));
            //            }
            //            else
            //            {
            //                ZoomOut(this);
            //            }

            //            e.Handled = true;
            //            return;
            //        }
            //    }

            //    e.Handled = false;
            //}

            //e.Handled = false;
        }

        /// <summary>
        /// Provides class handling for the MouseUp routed event that occurs when the mouse
        /// button  is released and the mouse pointer is over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DiagramView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _iteview = false;
            if (IsPageEditable)
            {
                if (IsPanEnabled)
                {
                    if (this.IsMouseCaptured)
                    {
                        if (BrowserInteropHelper.IsBrowserHosted)
                        {
                            this.Cursor = System.Windows.Input.Cursors.Hand;
                        }
                        else
                        {
                            Assembly ass = Assembly.GetExecutingAssembly();
                            Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.Released.cur");
                            m_cursor = new Cursor(stream);
                            this.Cursor = m_cursor;
                        }

                        this.ReleaseMouseCapture();
                        e.Handled = true;
                    }
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }
                if (e.ChangedButton == MouseButton.Right)
                {
                    if (DrawingTool == DrawingTools.PolyLine)
                    {
                        if (tempPolyLine != null)
                        {
                            ConnectorBase line = null;
                            line = new LineConnector();
                            line.MeasurementUnit = (Page as DiagramPage).MeasurementUnits;
                            line.ConnectorType = ConnectorType.Straight;
                            line.m_LineDrawing = true;
                            if (Drawing_HeadNode != null)
                            {
                                line.HeadNode = Drawing_HeadNode;
                                if (Drawing_HeadPort != null)
                                {
                                    line.ConnectionHeadPort = Drawing_HeadPort;
                                    Drawing_HeadPort = null;
                                }
                                Drawing_HeadNode = null;
                            }
                            else
                            {
                                line.PxStartPointPosition = tempPolyLine.Points[0];
                            }
                            if (Drawing_TailNode != null)
                            {
                                line.TailNode = Drawing_TailNode;
                                if (Drawing_TailPort != null)
                                {
                                    line.ConnectionTailPort = Drawing_TailPort;
                                    Drawing_TailPort = null;
                                }
                                Drawing_TailNode = null;
                            }
                            else
                            {
                                line.PxEndPointPosition = tempPolyLine.Points[tempPolyLine.Points.Count - 1];
                            }

                            line.IntermediatePoints = new List<Point>();
                            for (int i = 0; i < tempPolyLine.Points.Count; i++)
                            {
                                if (i != 0 && i != tempPolyLine.Points.Count - 1)
                                    line.IntermediatePoints.Add(tempPolyLine.Points[i]);
                            }

                            (Page as DiagramPage).AllChildren.Remove(tempPolyLine);
                            (Page as DiagramPage).Children.Remove(tempPolyLine);
                            tempPolyLine = null;
                            line.InvalidateConnectorPathGeometry();
                            dc.Model.Connections.Add(line);
                            ////update selection
                            SelectionList.Clear();
                            SelectionList.Add(line);
                            DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(line as object, this.DrawingTool);
                            newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                            line.RaiseEvent(newEventArgs);
                            e.Handled = true;
                            line.Focus();
                            if (this.DrawingMode == DrawingMode.Default)
                            {
                                this.EnableDrawingTools = false;
                            }

                            timeRightClick = DateTime.Now;
                        }
                    }
                }
                e.Handled = false;
                this.ReleaseMouseCapture();
            }
            this.m_EnableConnection = false;
            m_DragandMove = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseUp"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that one or more mouse buttons were released.</param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            this.startPoint = null;

            double aa = Scrollviewer.ScrollableWidth;
            Thumb tb = new Thumb();
            object o = e.OriginalSource;
            if (o is Thumb)
            {
                tb = o as Thumb;
            }

            if (e.Source is ScrollViewer && (e.OriginalSource is Thumb || e.OriginalSource is RepeatButton))
            {
                if (!tb.Name.Equals("HeadThumb") && !tb.Name.Equals("TailThumb"))
                {
                    if (HorizontalRuler != null)
                    {
                        if (!DupDeleted)
                        {
                            UpdateViewGridOrigin();
                        }
                    }

                    if (VerticalRuler != null)
                    {
                        if (!DupDeleted)
                        {
                            UpdateViewGridOrigin();
                        }
                    }
                }
            }

            foreach (ICommon shape in SelectionList)
            {
                oldselectionlist.Clear();
                e.Handled = false;
                base.OnMouseLeftButtonUp(e);
            }
        }

        bool _iteview=false;
        /// <summary>
        /// Provides class handling for the MouseUp routed event that occurs when the mouse
        /// button  is pressed and the mouse pointer is over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DiagramView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            ScrollViewer scroll = ReturnScroll(e.OriginalSource as FrameworkElement);
            // Find the ScrollViewer and Disable the EnableDrawingTools Property of the DiagramView.
            if (scroll != null && scroll.Name == "PART_ScrollViewer" &&EnableDrawingTools)
            {
                _iteview = true;
            }

            OnlyX = false;
            OnlyY = false;
            foreach (ICommon item in this.SelectionList)
            {
                if (item is IShape)
                {
                    oldselectionlist.Add(item as IShape);
                }
                else if (item is LineConnector)
                {
                    oldselectionlist.Add(item as LineConnector);
                }
            }

            DupDeleted = false;

            ScrollViewer svr = new ScrollViewer();
            ScrollChrome schrome = new ScrollChrome();
            Thumb tb = new Thumb();
            Rectangle rect = new Rectangle();
            Point p = e.GetPosition(this);
            if (e.Source.GetType() == svr.GetType() && e.OriginalSource.GetType() != rect.GetType())
            {
            }

            if (IsPageEditable)
            {
                if (IsPanEnabled)
                {
                    ScrollViewer sv = new ScrollViewer();
                    ScrollChrome chrome = new ScrollChrome();
                    //if (!(e.Source is ScrollViewer))
                    {
                        this.screenStartPoint = e.GetPosition(Page);
                      //  this.CaptureMouse();
                        if (BrowserInteropHelper.IsBrowserHosted)
                        {
                            this.Cursor = Cursors.Hand;
                        }
                        else
                        {
                            Assembly ass = Assembly.GetExecutingAssembly();
                            Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.Grabbed.cur");
                            m_cursor = new Cursor(stream);
                            this.Cursor = m_cursor;
                        }
                    }
                  e.Handled = true;
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }

                e.Handled = false;
            }
        }
        /// <summary>
        /// Provides class handling for the MouseMove routed event that occurs when
        /// the mouse pointer is over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void DiagramView_MouseMove(object sender, MouseEventArgs e)
        {
            
                //if (this.startPoint.HasValue && (e.LeftButton == MouseButtonState.Pressed)&&!IsPanEnabled&&!EnableDrawingTools)
                //{
                //    if (this.ItemSelectionMode == ItemSelectionMode.Multiple)
                //    {
                //        AdornerLayer adorner = AdornerLayer.GetAdornerLayer(this);
                //        if (adorner != null)
                //        {
                //            if (this.EnableDrawingTools == false)
                //            {
                //                NodeSelectionAdorner nodeadorner = new NodeSelectionAdorner(this, startPoint);
                //                if (adorner != null)
                //                {
                //                    adorner.Add(nodeadorner);
                //                }
                //                e.Handled = true;
                //                Page.InvalidateMeasure();
                //            }
                //        }
                //    }
                //}
                if (IsPanEnabled)
                {
                    ////this.Cursor = Cursors.Hand;
                    if (Mouse.Captured!=null)
                    {
                        if (BrowserInteropHelper.IsBrowserHosted)
                        {
                            this.Cursor = System.Windows.Input.Cursors.Hand;
                        }
                        else
                        {
                            Assembly ass = Assembly.GetExecutingAssembly();
                            Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.Grabbed.cur");
                            m_cursor = new Cursor(stream);
                            this.Cursor = m_cursor;
                        }


                        if (VerticalRuler != null && HorizontalRuler != null)
                        {
                            InvalidateViewGrid();
                            //(this.Page as DiagramPage).InvalidateMeasure();
                           
                        }
                        Point orig = OverviewContentHolder.GetOrigin((this.Scrollviewer.Content as OverviewContentHolder).Content as FourQuadrantPanel);
                        (this.Page as DiagramPage).Left = -orig.X;
                        (this.Page as DiagramPage).Top = -orig.Y;
                        var physicalPoint = e.GetPosition(Scrollviewer);
                        //(this.Scrollviewer.Content as OverviewContentHolder).IsPanEnabled=true;
                        (this.Scrollviewer.Content as OverviewContentHolder).InvalidateMeasure();
                        //(physicalPoint, this.screenStartPoint);
                        //Pan(physicalPoint, this.screenStartPoint);
                       
                        //e.Handled = true;
                    }
                    else
                    {
                        if (BrowserInteropHelper.IsBrowserHosted)
                        {
                            this.Cursor = System.Windows.Input.Cursors.Hand;
                        }
                        else
                        {
                            Assembly ass = Assembly.GetExecutingAssembly();
                            Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.Released.cur");
                            m_cursor = new Cursor(stream);
                            this.Cursor = m_cursor;
                        }
                    }
                    (this.Scrollviewer.Content as OverviewContentHolder).InvalidateMeasure();
                }
                else if (IsPageEditable)
                {
                    if (!m_EnableConnection)
                    {
                        this.Cursor = Cursors.Arrow;
                    }
                    e.Handled = false;
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }
        }

        #endregion

        #region class override

        /// <summary>
        /// Invoked whenever application code or internal processes call
        /// <see cref="System.Windows.FrameworkElement.ApplyTemplate"/> method.
        /// </summary>
        public override void OnApplyTemplate()
        {
           
            DiagramControl d = DiagramPage.GetDiagramControl(this);
            scrollview = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            mViewGrid = GetTemplateChild("PART_Grid") as DiagramViewGrid;
            viewgrid = GetTemplateChild("viewgrid") as Grid;
            SymbolPaletteItemPreview = GetTemplateChild("SymbolPaletteItemPreview") as Rectangle;

            base.OnApplyTemplate();

            Binding minizoombind = new Binding("MinimumZoom");
            minizoombind.Source = this;
            minizoombind.Mode = BindingMode.TwoWay;
            (this.Scrollviewer.Content as OverviewContentHolder).SetBinding(OverviewContentHolder.MinimumZoomProperty, minizoombind);

            Binding maxzoombind = new Binding("MaximumZoom");
            maxzoombind.Source = this;
            maxzoombind.Mode = BindingMode.TwoWay;
            (this.Scrollviewer.Content as OverviewContentHolder).SetBinding(OverviewContentHolder.MaximumZoomProperty, maxzoombind);

            Binding Animationbind = new Binding("AnimationEnabled");
            Animationbind.Source = this;
            Animationbind.Mode = BindingMode.TwoWay;
            (this.Scrollviewer.Content as OverviewContentHolder).SetBinding(OverviewContentHolder.AnimationEnabledProperty, Animationbind);

            this.zoomTransform = new ScaleTransform();
            this.MouseMove += new MouseEventHandler(DiagramView_MouseMove);
            this.AddHandler(Control.MouseMoveEvent, new MouseEventHandler(DiagramView_MouseMove), true);
            this.PreviewMouseDown += new MouseButtonEventHandler(DiagramView_PreviewMouseDown);
            this.AddHandler(Control.MouseUpEvent, new MouseButtonEventHandler(DiagramView_MouseUp), true);
            //this.MouseUp += new MouseButtonEventHandler(DiagramView_MouseUp);
            this.PreviewMouseWheel += new MouseWheelEventHandler(DiagramView_PreviewMouseWheel);
            this.KeyDown += new KeyEventHandler(DiagramView_KeyDown);
            this.KeyUp += new KeyEventHandler(DiagramView_KeyUp);
            Scrollviewer.SizeChanged += new SizeChangedEventHandler(Scrollviewer_SizeChanged);

#if !SyncfusionFramework3_5
            ViewGrid.IsManipulationEnabled = true;
            ViewGrid.ManipulationStarting += new EventHandler<System.Windows.Input.ManipulationStartingEventArgs>(DiagramView_ManipulationStarting);
            ViewGrid.ManipulationDelta += new EventHandler<System.Windows.Input.ManipulationDeltaEventArgs>(DiagramView_ManipulationDelta);
            ViewGrid.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(DiagramView_ManipulationCompleted);
#endif
            if (d != null && this.HorizontalRuler != null)
            {
                d.SetScope(this.HorizontalRuler);
            }

            if (d != null && this.VerticalRuler != null)
            {
                d.SetScope(this.VerticalRuler);
            }
        }

        void DiagramView_KeyUp(object sender, KeyEventArgs e)
        {
            AllowMoveX = true;
            AllowMoveY = true;
            dc.View._RunTimeMovement = false;
        }
#if !SyncfusionFramework3_5
        void DiagramView_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            e.Cancel();
        }
        void DiagramView_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            int manipulatorcount = e.Manipulators.Count<System.Windows.Input.IManipulator>();
            var element = e.Source as FrameworkElement;
            if (manipulatorcount >= 2)
            {
               
                ManipulationDelta manipDelta = e.DeltaManipulation;
                Matrix rectsMatrix = ((MatrixTransform)element.RenderTransform).Matrix;
                Point rectManipOrigin = rectsMatrix.Transform(new Point(element.ActualWidth / 2, element.ActualHeight / 2));
                this.CurrentZoom *= manipDelta.Scale.X;
                zoomTransform = new ScaleTransform(CurrentZoom, CurrentZoom);
                ViewGrid.LayoutTransform = zoomTransform;
                element.UpdateLayout();
                this.UpdateViewGridOrigin();
                e.Handled = true;
            }
            else
            {
                e.Cancel();
            }
            element.UpdateLayout();
            this.UpdateViewGridOrigin();
        }

        void DiagramView_ManipulationStarting(object sender, System.Windows.Input.ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Handled = true;
            if((e.OriginalSource!=viewgrid))
            {
                e.Cancel();
            }
          
        }
#endif
        void Scrollviewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (HorizontalRuler != null)
            {
                HorizontalRuler.InvalidateArrange();
            }
            if (VerticalRuler != null)
            {
                VerticalRuler.InvalidateArrange();
            }
        }

        void rect_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Provides class handling for the PreviewMouseMove routed event that occurs when
        /// the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.HorizontalRuler != null)
            {
                double vrulerthickness = 0;
                if (VerticalRuler != null)
                {
                    vrulerthickness = VerticalRuler.RulerThickness - 8.5;
                }

                HorizontalRuler.MarkerPosition = e.GetPosition(this).X - vrulerthickness;

                if (HorizontalRuler.MarkerPosition > HorizontalRuler.ActualWidth)
                {
                    HorizontalRuler.MarkerPosition = HorizontalRuler.ActualWidth;
                }
            }

            if (this.VerticalRuler != null)
            {
                double hrulerthickness = 0;
                if (HorizontalRuler != null)
                {
                    hrulerthickness = VerticalRuler.RulerThickness - 4.25;
                }

                VerticalRuler.MarkerPosition = e.GetPosition(this).Y - hrulerthickness;

                if (VerticalRuler.MarkerPosition > VerticalRuler.ActualHeight)
                {
                    VerticalRuler.MarkerPosition = VerticalRuler.ActualHeight;
                }
            }

            base.OnPreviewMouseMove(e);
            e.Handled = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateRuler(this);
        }
        #endregion

        #region IView Members

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// Type: <see cref="IModel"/>
        /// IModel instance.
        /// </value>
        public IModel Model
        {
            get
            {
                return mModel;
            }

            set
            {
                mModel = value;
            }
        }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Point"/>
        /// The pan.
        /// </value>
        public System.Drawing.Point Origin
        {
            get
            {
                return mOrigin;
            }

            set
            {
                mOrigin = value;
            }
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Drawing.Rectangle"/>
        /// Bounds value.
        /// </value>
        public Thickness Bounds
        {
            get { return (Thickness)GetValue(BoundsProperty); }
            set { SetValue(BoundsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bounds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register("Bounds", typeof(Thickness), typeof(DiagramView));



        public bool EnableFitToPage
        {
            get
            {
                return (bool)GetValue(EnableFitToPageProperty);
            }

            set
            {
                SetValue(EnableFitToPageProperty, value);
            }
        }
        public static readonly DependencyProperty EnableFitToPageProperty = DependencyProperty.Register("EnableFitToPage", typeof(bool), typeof(DiagramView), new PropertyMetadata(false, new PropertyChangedCallback(OnFitToPageEnabled)));



        /// <summary>
        /// Gets or sets the LayoutBounds.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Windows.Rect"/>
        /// LayoutBounds value.
        /// </value>
        public Rect LayoutBounds
        {
            get { return (Rect)GetValue(LayoutBoundsProperty); }
            set { SetValue(LayoutBoundsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bounds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutBoundsProperty =
            DependencyProperty.Register("LayoutBounds", typeof(Rect), typeof(DiagramView));


        /// <summary>
        /// Identifies the CustomPathStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomPathStyleProperty =
            DependencyProperty.Register("CustomPathStyle", typeof(Style), typeof(DiagramView), new UIPropertyMetadata(null));

        public Style CustomPathStyle
        {
            get { return (Style)GetValue(CustomPathStyleProperty); }
            set { SetValue(CustomPathStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show rulers].
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if it is to displayed, false otherwise.
        /// </value>
        internal bool ShowRulers
        {
            get
            {
                return showRuler;
            }

            set
            {
                showRuler = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show page].
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if it is to displayed, false otherwise.
        /// </value>
        public bool ShowPage
        {
            get
            {
                return showPage;
            }

            set
            {
                showPage = value;
            }
        }

        #endregion

      

        internal Point PxStartPoint
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(DrawStrat, (Page as DiagramPage).MeasurementUnits);
            }
            set
            {
                DrawStrat = MeasureUnitsConverter.FromPixels(value, (Page as DiagramPage).MeasurementUnits);
            }


        }

        internal Thickness PxBounds
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(Bounds, (Page as DiagramPage).MeasurementUnits);
            }

            set
            {
                Bounds = MeasureUnitsConverter.FromPixels(value, (Page as DiagramPage).MeasurementUnits);
            }
        }

        internal Rect PxLayoutBounds
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(LayoutBounds,(Page as DiagramPage).MeasurementUnits);
            }

            set
            {
                LayoutBounds = MeasureUnitsConverter.FromPixels(value,(Page as DiagramPage).MeasurementUnits);
            }
        }

        internal double PxSnapOffsetX
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(SnapOffsetX, (Page as DiagramPage).MeasurementUnits);
            }

            set
            {
                SnapOffsetX = MeasureUnitsConverter.FromPixels(value, (Page as DiagramPage).MeasurementUnits);
            }
        }

        internal double PxSnapOffsetY
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(SnapOffsetY, (Page as DiagramPage).MeasurementUnits);
            }

            set
            {
                SnapOffsetY = MeasureUnitsConverter.FromPixels(value, (Page as DiagramPage).MeasurementUnits);
            }
        }

        internal double PxNudgeIncrement
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(this.NudgeIncrement, (Page as DiagramPage).MeasurementUnits);
            }
            set
            {
                NudgeIncrement = MeasureUnitsConverter.FromPixels(value, (Page as DiagramPage).MeasurementUnits);
            }
        }

        #region FromPage


        /// <summary>
        /// Used to store the start point.
        /// </summary>
        internal Point? startPoint = null;

        internal DateTime timeRightClick;
        internal Polyline tempPolyLine;

        /// <summary>
        /// Used to refer to the child count value.
        /// </summary>
        private bool childcount = true;

        /// <summary>
        /// Used to refer to the children count.
        /// </summary>
        /// <remarks></remarks>
        private int no;

        /// <summary>
        /// Used to refer to the name count
        /// </summary>
        private int namecount = 0;

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (dc.View.IsPageEditable && !dc.View.IsPanEnabled)
            {
                base.OnDragEnter(e);

                if (dc.SymbolPalette != null && dc.SymbolPalette.ShowPreview)
                {
                    DragObject dragobj = e.Data.GetData(typeof(DragObject)) as DragObject;
                    if (dragobj != null)
                    {
                        SymbolPaletteItemPreview.Fill = dragobj.PreviewContent;
                    }
                    SymbolPaletteItemPreview.Height = dragobj.DragItem.m_SymbolPalette.SelectedItem.PreviewSize.Height;
                    SymbolPaletteItemPreview.Width = dragobj.DragItem.m_SymbolPalette.SelectedItem.PreviewSize.Width;
                    //SymbolPaletteItemPreview.Width = 50;
                    //SymbolPaletteItemPreview.Height = 50;
                    SymbolPaletteItemPreview.IsHitTestVisible = false;
                }
            }
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            if (dc.View.IsPageEditable)
            {
                base.OnDragLeave(e);

                if (dc.SymbolPalette != null && dc.SymbolPalette.ShowPreview)
                {
                    if (SymbolPaletteItemPreview != null)
                    {
                        SymbolPaletteItemPreview.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {

            if (dc.View.IsPageEditable && !dc.View.IsPanEnabled)
            {
                base.OnDragOver(e);
                if (!this.SizeToContent)
                {
                    if (this.BoundaryConstraintsEnabled)
                    {
                        Point availablearea = e.GetPosition(this.Page);
                        if (BoundaryConstraintsArea.Left+25 < availablearea.X && BoundaryConstraintsArea.Top+25 < availablearea.Y && BoundaryConstraintsArea.Right-25 > availablearea.X && BoundaryConstraintsArea.Bottom-25 > availablearea.Y)
                        {
                            e.Effects = DragDropEffects.Copy;
                            e.Handled = false;
                        }
                        else
                        {
                            e.Effects = DragDropEffects.None;
                            this.Cursor = Cursors.IBeam;
                        }
                    }
                }
                if (dc.SymbolPalette != null && dc.SymbolPalette.ShowPreview)
                {
                    if (SymbolPaletteItemPreview != null)
                    {
                        SymbolPaletteItemPreview.Visibility = Visibility.Visible;

                        ScaleTransform sf = new ScaleTransform(this.CurrentZoom, this.CurrentZoom, SymbolPaletteItemPreview.Width / 2, SymbolPaletteItemPreview.Height / 2);
                        TranslateTransform tf = new TranslateTransform(e.GetPosition(this).X - 25, e.GetPosition(this).Y - 25);

                        TransformGroup rectGroup = new TransformGroup();
                        rectGroup.Children.Add(sf);
                        rectGroup.Children.Add(tf);

                        SymbolPaletteItemPreview.RenderTransform = rectGroup;
                    }
                }
            }
        }
        //protected override void OnPreviewDrop(DragEventArgs e)
        //{

        //    if (!this.SizeToContent)
        //    {
        //        if (this.BoundaryConstraintsEnabled)
        //        {
        //           Point availablearea=e.GetPosition(this.Page);
        //           if (BoundaryConstraintsArea.Left > availablearea.X && BoundaryConstraintsArea.Top > availablearea.Y && BoundaryConstraintsArea.Right < availablearea.X && BoundaryConstraintsArea.Bottom < availablearea.Y)
        //           {
        //               e.Handled = false;
        //           }
        //           else
        //           {
        //               this.Cursor = Cursors.No;
        //               e.Handled = true;
        //           }
        //        }
        //    }
        //    base.OnPreviewDrop(e);
        //}

        #region PART_OnDrop
        private void IsSerializationItem(DragObject shape,Point mouseposition)
        {
            string name = string.Empty;
            Node droppedNode = null;
            if (shape.SerializedItem != null||shape.CustomObject!=null)
            {
                object content;
                if (shape.SerializedItem != null)
                {
                    content =
                        System.Windows.Markup.XamlReader.Load(
                            System.Xml.XmlReader.Create(new StringReader(shape.SerializedItem)));
                }
                else
                {
                    content = shape.CustomObject;
                }
                if (content != null && content is LineConnector)
                {
                    DropLine(content as ConnectorBase,mouseposition, this.dc);
                }
                if (content != null && content is Node)
                {
                    if (content is Group)
                    {
                        _dropgrp = new Group();
                        _dropgrp = (Group)LoadingGroupContent(content);
                        dc.Model.Nodes.Add(_dropgrp);
                    }
                    else
                    {
                        droppedNode = LoadingGroupContent(content);
                    }
                }
                else if (content != null)
                {
                    if (content is UIElement)
                    {
                        name = (content as UIElement).Uid;
                    }
                    if (content is System.Windows.Shapes.Path)
                    {
                        System.Windows.Shapes.Path path = content as System.Windows.Shapes.Path;
                        path.IsHitTestVisible = false;
                        (droppedNode as ContentControl).Content = path;
                    }
                    else
                    {
                        DependencyObject d = new DependencyObject();

                        if (content is UIElement && !(content is LineConnector))
                        {
                            (content as UIElement).IsHitTestVisible = false;
                            if (content is Viewbox)
                            {
                                (content as Viewbox).Stretch = Stretch.Fill;
                                (content as Viewbox).Width = (droppedNode as ContentControl).Width;
                                (content as Viewbox).Height = (droppedNode as ContentControl).Height;
                            }

                            (droppedNode as ContentControl).Content = content;
                        }
                        else if (!(content is LineConnector))
                        {
                            (droppedNode as ContentControl).Content = content;
                        }
                    }
                }
            }
        }

        private void SetUIElementProperties(FrameworkElement element)
        {
            if (element is Node)
            {
                if (double.IsNaN(element.Width) || element.Width <= 0)
                {
                    (element as Node).PxWidth = 50;
                }
                if (double.IsNaN(element.Height) || element.Height <= 0)
                {
                    (element as  Node).PxHeight = 50;
                }
            }

        }

        private void RefreshConnection(List<LineConnector> listofline,int increasedcount)
        {
            foreach (LineConnector line in listofline)
            {
                int h1 = line.HeadNodeReferenceNo;
                int l1 = line.TailNodeReferenceNo;
                var connection = from Node nod in dc.Model.Nodes where nod.ReferenceNo == h1 || nod.ReferenceNo == l1 select nod;
                foreach (Node node in connection.ToList())
                {
                    if (line.HeadNodeReferenceNo == node.ReferenceNo&&!(node is Group))
                    {
                        line.HeadNode = node;
                    }
                    else if (line.TailNodeReferenceNo == node.ReferenceNo&&!(node is Group))
                    {
                        line.TailNode = node;

                    }
                }
            }
        }       

        private void IsOldChildren(FrameworkElement droppedNode)
        {
            if (!this.Page.Children.Contains(droppedNode as Node))
            {
                if (this.EnableVirtualization)
                {
                    this.ScrollGrid.VirtualzingNode(droppedNode as Node);
                }
                else
                {
                    (this.Page as DiagramPage).AddingChildren(droppedNode as Node);
                }
                Panel.SetZIndex((droppedNode as FrameworkElement), Page.Children.Count);
            }
        }

        private void CheckDuplicateName(FrameworkElement droppedNode,string str2)
        {
            if (droppedNode!= null && string.IsNullOrEmpty(droppedNode.Name))
            {
                if (dc.Model.Nodes.Count == 0)
                {
                    droppedNode.Name = str2;
                }
                if (droppedNode is Node)
                {
                    foreach (IShape n in dc.Model.Nodes)
                    {
                        if (DoubleCheckName(n as FrameworkElement, droppedNode, str2))
                        {
                            break;
                        }
                    }
                }
                if (droppedNode is LineConnector)
                {
                    foreach (IShape n in dc.Model.Connections)
                    {
                        if (DoubleCheckName(n as FrameworkElement, droppedNode, str2))
                        {
                            break;
                        }
                    }
                }
            }
        }

        private bool DoubleCheckName(FrameworkElement n, FrameworkElement droppedNode,string str2)
        {
            if (n != null && (n as FrameworkElement).Name != str2)
            {
                droppedNode.Name = str2;
                return false;
            }
            else
            {
                droppedNode.Name = "new" + str2;
                return true;
            }
 
        }

        private void SetPosition(List<object> ContentCollection, List<Point> correctposition, Point currentposition)
        {
            foreach (object obj in ContentCollection)
            {
                if (obj is IShape && correctposition.Count > 0)
                {
                    (obj as IShape).OffsetX = (obj as IShape).OffsetX - correctposition[0].X + currentposition.X - correctposition[1].X;
                    (obj as IShape).OffsetY = (obj as IShape).OffsetY - correctposition[0].Y + currentposition.Y - correctposition[1].Y;
                }
                this.SelectionList.Add(obj);
            }
            if (ContentCollection.Count > 1 || _dropgrp != null)
            {
                if (_dropgrp != null)
                {
                    GroupDroppedRoutedEventArgs groupdropevt = new GroupDroppedRoutedEventArgs(_dropgrp);
                    groupdropevt.RoutedEvent = DiagramView.GroupDropEvent;
                    RaiseEvent(groupdropevt);
                }
                else
                {
                    DiagramCommandManager.Group.Execute(this.Page, this);
                    GroupDroppedRoutedEventArgs groupdropevt = new GroupDroppedRoutedEventArgs(dc.Model.Nodes[dc.Model.Nodes.Count - 1] as Group);
                    groupdropevt.RoutedEvent = DiagramView.GroupDropEvent;
                    RaiseEvent(groupdropevt);
                }
            }
        }
        #endregion
        
        /// <summary>
        /// Provides class handling for the OnDrop routed event that occurs when any item
        /// is dropped on this control..
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            CollectionExt.Cleared = false;
            bool dropped=false;
            if (!this.SizeToContent)
            {
                if (this.BoundaryConstraintsEnabled)
                {
                    Point availablearea = e.GetPosition(this.Page);
                    if (BoundaryConstraintsArea.Left+25<availablearea.X && BoundaryConstraintsArea.Top+25<availablearea.Y && BoundaryConstraintsArea.Right-25>availablearea.X && BoundaryConstraintsArea.Bottom-25>availablearea.Y)
                    {
                        dropped = false;
                        e.Handled = false;
                    }
                    else
                    {
                        dropped = true;

                    }
                }
            }
            if (dropped)
            {
                SymbolPaletteItemPreview.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (dc.View.IsPageEditable && !dc.View.IsPanEnabled)
                { List<LineConnector> listofLine = new List<LineConnector>();
                    base.OnDrop(e);
                    DragObject shape = e.Data.GetData(typeof(DragObject)) as DragObject;
                    if (shape != null)
                    {
                        if (!String.IsNullOrEmpty(shape.SerializedItem) || shape.SerializedItemGroup != null)
                        {
                            if (!String.IsNullOrEmpty(shape.SerializedItem) && shape.SerializedItem.ToString().Contains("sync_L_o"))
                            {
                                DropLine(ConnectorType.Orthogonal, e.GetPosition(Page), dc);
                            }
                            else if (!String.IsNullOrEmpty(shape.SerializedItem) && shape.SerializedItem.ToString().Contains("sync_L_s"))
                            {
                                DropLine(ConnectorType.Straight, e.GetPosition(Page), dc);
                            }
                            else if (!String.IsNullOrEmpty(shape.SerializedItem) && shape.SerializedItem.ToString().Contains("sync_L_a"))
                            {
                                DropLine(ConnectorType.Arc, e.GetPosition(Page), dc);
                            }
                            else if (!String.IsNullOrEmpty(shape.SerializedItem) && shape.SerializedItem.ToString().Contains("sync_L_b"))
                            {
                                DropLine(ConnectorType.Bezier, e.GetPosition(Page), dc);
                            }
                            else
                            {
                                Node droppedNode = null;
                                PreviewNodeDropEventRoutedEventArgs nodedropnewEventArgs = new PreviewNodeDropEventRoutedEventArgs();
                                nodedropnewEventArgs.RoutedEvent = DiagramView.PreviewNodeDropEvent;
                                RaiseEvent(nodedropnewEventArgs);
                                if (nodedropnewEventArgs.Cancel == false)
                                {
                                    if (nodedropnewEventArgs.Node != null)
                                    {
                                        droppedNode = nodedropnewEventArgs.Node as Node;
                                    }
                                    else
                                    {
                                        droppedNode = new Node();
                                    }
                                    bool line=true;
                                    string name = string.Empty;
                                    if (shape.SerializedItem != null||shape.CustomObject!=null)
                                    {
                                        object content;
                                        if (shape.SerializedItem != null)
                                        {

                                             content =
                                                System.Windows.Markup.XamlReader.Load(
                                                    System.Xml.XmlReader.Create(new StringReader(shape.SerializedItem)));
                                        }
                                        else
                                        {
                                            content = shape.CustomObject;
                                        }

                                        if (content != null && content is LineConnector)
                                        {
                                            line=false;
                                            DropLine(content as ConnectorBase, e.GetPosition(this.Page), this.dc);
                                            listofLine.Add(content as LineConnector);
                                        }
                                        if (content != null && content is Node)
                                        {
                                            if (content is Group)
                                            {
                                                _dropgrp = new Group();
                                                _dropgrp = (Group)LoadingGroupContent(content);
                                                dc.Model.Nodes.Add(_dropgrp);
                                            }
                                            else
                                            {
                                                droppedNode = LoadingGroupContent(content);
                                                foreach (ConnectionPort port in droppedNode.Ports)
                                                {
                                                    port.Node = droppedNode;
                                                }
                                            }
                                        }
                                        else if (content != null)
                                        {
                                            if (content is UIElement)
                                            {
                                                name = (content as UIElement).Uid;
                                            }
                                            if (content is System.Windows.Shapes.Path)                                            
                                            {
                                                System.Windows.Shapes.Path path = content as System.Windows.Shapes.Path;
                                                path.IsHitTestVisible = false;
                                                (droppedNode as ContentControl).Content = path;
                                            }
                                            else
                                            {
                                                if (content is UIElement && !(content is LineConnector))
                                                {
                                                    (content as UIElement).IsHitTestVisible = false;
                                                    if (content is Viewbox)
                                                    {
                                                        (content as Viewbox).Stretch = Stretch.Fill;
                                                        (content as Viewbox).Width = (droppedNode as ContentControl).Width;
                                                        (content as Viewbox).Height = (droppedNode as ContentControl).Height;
                                                    }

                                                   (droppedNode as ContentControl).Content = content;
                                                }
                                                else if (!(content is LineConnector))
                                                {
                                                    (droppedNode as ContentControl).Content = content;
                                                }
                                            }
                                        }
                                    }
                                    if (shape.SerializedItemGroup != null && shape.SerializedItemGroup.Count > 0)
                                    {
                                        List<object> ContentCollection = new List<object>();
                                        this.SelectionList.Clear();
                                        foreach (string items in shape.SerializedItemGroup)
                                        {
                                            object content = System.Windows.Markup.XamlReader.Load(System.Xml.XmlReader.Create(new StringReader(items)));
                                            if (content is IShape)
                                            {
                                                dropbox.Add(content as IShape);
                                            }
                                            if (content.GetType() == typeof(LineConnector))
                                            {
                                                DropLine(content as ConnectorBase, e.GetPosition(this.Page), this.dc);
                                                listofLine.Add(content as LineConnector);
                                                continue;
                                            }
                                            droppedNode = LoadingGroupContent(content);
                                            if (_dropgrp != null)
                                            {
                                                _dropgrp.AddChild(droppedNode);
                                            }
                                            ContentCollection.Add(droppedNode);
                                            Point position3 = e.GetPosition(Page);
                                            droppedNode.MeasurementUnits = (Page as DiagramPage).MeasurementUnits;
                                            // to set Node size properties
                                            SetUIElementProperties(droppedNode);
                                            if (childcount)
                                            {
                                                no = Page.Children.Count + 1;
                                                childcount = false;
                                            }
                                            namecount = no++;
                                            string str2 = "Node" + namecount;
                                            CheckDuplicateName(droppedNode, str2);
                                            droppedNode.IsHitTestVisible = true;
                                            dc.Model.Nodes.Add(droppedNode);
                                            //Point position4 = e.GetPosition(Page);
                                            (droppedNode as FrameworkElement).Focus();
                                            //NodeDroppedRoutedEventArgs newEventArgs2 = new NodeDroppedRoutedEventArgs(droppedNode, name, new Point(position4.X - 25, position4.Y - 25));
                                            //newEventArgs2.RoutedEvent = DiagramView.NodeDropEvent;
                                            //RaiseEvent(newEventArgs2);
                                            dc.View.undo = true;
                                            //droppedNode.PxOffsetX = position4.X - 25;
                                            //droppedNode.PxOffsetY = position4.Y - 25;
                                            IsOldChildren(droppedNode);
                                            //ContentCollection.Add(content);
                                        }
                                        List<Point> correctposition = new List<Point>();
                                        if (dropbox.Count > 0)
                                        {
                                            correctposition = nodedropRect(dropbox, e.GetPosition(Page));
                                            dropbox.Clear();
                                        }
                                        SetPosition(ContentCollection, correctposition, e.GetPosition(Page));
                                    }
                                    if ((shape.SerializedItem != null||shape.CustomObject!=null) && line)
                                    {
                                        Point position = e.GetPosition(Page);
                                        droppedNode.MeasurementUnits = (Page as DiagramPage).MeasurementUnits;
                                        SetUIElementProperties(droppedNode);
                                        if (childcount)
                                        {
                                            no = Page.Children.Count + 1;
                                            childcount = false;
                                        }
                                        namecount = no++;
                                        string str = "Node" + namecount;
                                        CheckDuplicateName(droppedNode, str);
                                        dc.Model.Nodes.Add(droppedNode);
                                        if (_dropgrp != null)
                                        {
                                            _dropgrp.IsHitTestVisible = true;
                                            //dc.Model.Nodes.Add(_dropgrp);
                                            SelectionList.Clear();
                                            SelectionList.Add(_dropgrp);
                                        }
                                        
                                        Point position2 = e.GetPosition(Page);
                                        this.SelectionList.Clear();
                                        this.SelectionList.Add(dc.Model.Nodes[dc.Model.Nodes.Count - 1]);
                                        (droppedNode as FrameworkElement).Focus();

                                        NodeDroppedRoutedEventArgs newEventArgs = new NodeDroppedRoutedEventArgs(droppedNode, name, new Point(position2.X - (droppedNode.Width / 2), position2.Y - (droppedNode.Height / 2)));
                                        newEventArgs.RoutedEvent = DiagramView.NodeDropEvent;
                                        RaiseEvent(newEventArgs);

                                        dc.View.undo = true;
                                        if (!droppedNode.IsGrouped)
                                        {
                                            //droppedNode.PxOffsetX = position2.X - (droppedNode.Width / 2);
                                            //droppedNode.PxOffsetY = position2.Y - (droppedNode.Height / 2);
                                            if (dc.View.SnapToVerticalGrid)
                                            {
                                                droppedNode.PxOffsetX = Node.Round(position2.X - (droppedNode.Width / 2), dc.View.SnapOffsetX);
                                            }
                                            else
                                            {
                                                droppedNode.PxOffsetX = position2.X - (droppedNode.Width / 2);
                                            }
                                            if (dc.View.SnapToHorizontalGrid)
                                            {
                                                droppedNode.PxOffsetY = Node.Round(position2.Y - (droppedNode.Height / 2), dc.View.SnapOffsetY);
                                            }
                                            else
                                            {
                                                droppedNode.PxOffsetY = position2.Y - (droppedNode.Height / 2);
                                            }
                                        }
                                        
                                        IsOldChildren(droppedNode);
                                        if (BisectConnectorOnDrop)
                                        {
                                            InsertNewLine(droppedNode);
                                        }
                                        (droppedNode as Node).Loaded += new RoutedEventHandler(DropNode_Loaded);
                                    }
                                    dc.View.undo = false;
                                    if (dc != null)
                                    {
                                        foreach (Layer l in dc.Model.Layers)
                                        {
                                            if (l.Active)
                                            {
                                                l.Nodes.Add(droppedNode as Node);
                                            }
                                        }
                                    }
                                }
                                
                            }
                            SymbolPaletteItemPreview.Visibility = Visibility.Collapsed;
                        }
                        
                    }
                    if (listofLine != null && listofLine.Count > 0)
                    {
                        RefreshConnection(listofLine, shape.SerializedItemGroup.Count);
                    }
                }
            }
            _dropgrp = null;
            Focus();
            _contentHolder.InvalidateMeasure();
        }

        //This method will be invoked on Node drop, if dview.BisectConnectorOnNodeDrop is set to true.

        void InsertNewLine(Node node)
        {
            Rect rect = new Rect(node.PxOffsetX, node.PxOffsetY, node.Width, node.Height);
            Point? interectPoint;
            var OverlappedLines = from LineConnector lin in dc.Model.Connections where (lin.IntersectsWith(node, out interectPoint)) select lin;
            List<LineConnector> OverlappedLinesList = OverlappedLines.ToList<LineConnector>();
            if (OverlappedLinesList.Count > 0)
            {
                LineConnector line = OverlappedLinesList[0];
                LineConnector newLine = new LineConnector();
                NodeDroppedEventArgs evtArgs = new NodeDroppedEventArgs(false, newLine, OverlappedLinesList,line);
                evtArgs.RoutedEvent = DiagramView.NodeDroppingEvent;
                RaiseEvent(evtArgs);
                if (!evtArgs.Cancel)
                {
                    if (evtArgs.SelectedLine != null && evtArgs.NewConnector != null)
                    {
                        evtArgs.NewConnector.HeadNode = node;

                        if (evtArgs.SelectedLine.TailNode == null)
                            evtArgs.NewConnector.PxEndPointPosition = evtArgs.SelectedLine.PxEndPointPosition;
                        else
                            evtArgs.NewConnector.TailNode = evtArgs.SelectedLine.TailNode;
                        evtArgs.NewConnector.UpdateConnectorPathGeometry();
                        evtArgs.SelectedLine.TailNode = node;
                        dc.Model.Connections.Add(evtArgs.NewConnector);
                    }
                }
            }
        }
        
              
        List<IShape> dropbox = new List<IShape>();

        private List<Point> nodedropRect(List<IShape> _dropbox, Point _dropPosition)
        {
            List<Point> dropbox = new List<Point>();
            double offx = _dropbox.Min(p => p.OffsetX);
            double offy = _dropbox.Min(p => p.OffsetY);
            double offx1 = (_dropbox.Max(p => p.OffsetX));
            double offy1 = _dropbox.Max(p => p.OffsetY);
            dropbox.Add(new Point(offx,offy));
            dropbox.Add(new Point(Math.Abs(offx1 - offx) / 2, Math.Abs(offy1 - offy) / 2));
            return dropbox;
        }
        Group _dropgrp;
        /// <summary>
        /// Create SymbolPaletteItem from the SelectionList
        /// </summary>
        /// <returns> return SymbolPaletteItem</returns>
        /// <remarks></remarks>
        public SymbolPaletteItem CreateSymbolPaletteItem()
        {
            return preparingSymbolPaletteItem(null, null);
        }
        /// <summary>
        /// Create SymbolPaletteItem from the SelectionList and it into SymbolGroup which is given in the parameter.
        /// </summary>
        /// <returns> return SymbolPaletteItem</returns>
        /// <remarks></remarks>
        public SymbolPaletteItem CreateSymbolPaletteItem(SymbolPaletteGroup SymbolGroup)
        {
            return preparingSymbolPaletteItem(SymbolGroup, null);
        }
        /// <summary>
        /// Create SymbolPaletteItem from the List of UIElement and add it into given Group. 
        /// </summary>
        /// <returns> return SymbolPaletteItem</returns>
        /// <remarks></remarks>
        public SymbolPaletteItem CreateSymbolPaletteItem(SymbolPaletteGroup SymbolGroup, List<UIElement> GroupElements)
        {
            return preparingSymbolPaletteItem(SymbolGroup, GroupElements);
        }

        private SymbolPaletteItem preparingSymbolPaletteItem(SymbolPaletteGroup group,List<UIElement> GroupElements)
        {
            SymbolPaletteItem item = new SymbolPaletteItem();
            if (GroupElements != null)
            {
                item.Content = GroupElements;
            }
            else
            {
                CollectionExt GroupNode = new CollectionExt();
                foreach (UIElement com in this.SelectionList)
                {
                    GroupNode.Add(com);
                }
                item.Content = GroupNode;
            }
            if (group != null)
            {
                group.Items.Add(item);
            }

            return item;
        }
        private Node LoadingGroupContent(object content)
        {
            Node droppedNode;
            if (content is Group)
            {
                object content1 = System.Windows.Markup.XamlReader.Load(System.Xml.XmlReader.Create(new StringReader(GetXamlValue(content as UIElement))));
                _dropgrp = content1 as Group;
                droppedNode = content1 as Group;
            }
           else if (content is Node)
            {
                //(content as Node).IsGrouped = false;
                object content1 = System.Windows.Markup.XamlReader.Load(System.Xml.XmlReader.Create(new StringReader(GetXamlValue(content as UIElement))));
                droppedNode = content1 as Node;
            }
            else
            {
                droppedNode=new Node();
            }
            if (content != null)
            {
                if (content is UIElement)
                {
                    string name = (content as UIElement).Uid;
                }
                if (content is System.Windows.Shapes.Path)
                {
                    System.Windows.Shapes.Path path = content as System.Windows.Shapes.Path;
                    path.IsHitTestVisible = false;
                    (droppedNode as ContentControl).Content = path;
                }
                else
                {
                    if (content is UIElement&&!(content is Node))
                    {
                        (content as UIElement).IsHitTestVisible = false;
                        if (content is Viewbox)
                        {
                            (content as Viewbox).Stretch = Stretch.Fill;
                            (content as Viewbox).Width = (droppedNode as ContentControl).Width;
                            (content as Viewbox).Height = (droppedNode as ContentControl).Height;
                        }
                        (droppedNode as ContentControl).HorizontalContentAlignment = HorizontalAlignment.Stretch;
                        (droppedNode as ContentControl).VerticalContentAlignment = VerticalAlignment.Stretch;
                        (droppedNode as ContentControl).Content = content;
                    }
                    else if (!(content is Node))
                    {
                        (droppedNode as ContentControl).Content = content;
                    }
                }
            }
            droppedNode.IsHitTestVisible = true;
            return droppedNode;
        }

        private void LoadingGroup(Group group)
        {
            foreach (Node node in group.NodeChildren)
            {
                if (node is Group)
                {
                    this.Model.Nodes.Add(node);
                    LoadingGroup(node as Group);
                }
                else
                {
                    this.Model.Nodes.Add(node);
                }
            }
        }

        private string GetXamlValue(UIElement ui)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                if (ui != null && ms != null)
                {
                    ms.Flush();
                    System.Windows.Markup.XamlWriter.Save(ui, ms);
                    ms.Position = 0;
                    System.IO.StreamReader sr = new System.IO.StreamReader(ms);
                    return sr.ReadToEnd();
                }
                else
                {
                    return null;
                }
            }
        }
        private void DropNode_Loaded(object sender, RoutedEventArgs e)
        {
            dc.View.Page.InvalidateMeasure();
            dc.View.Page.InvalidateArrange();
            (sender as Node).Loaded -= new RoutedEventHandler(DropNode_Loaded);
        }
        private void DropLine_Loaded(object sender, RoutedEventArgs e)
        {
            dc.View.Page.InvalidateMeasure();
            dc.View.Page.InvalidateArrange();
            (sender as LineConnector).Loaded -= new RoutedEventHandler(DropLine_Loaded);
        }

        /// <summary>
        /// Provides class handling for the MouseLeftButtonUp routed event that occurs when the mouse
        /// button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            CollectionExt.Cleared = false;
            if (dc.View.IsPageEditable)
            {
               // (viewgrid.Children[1] as DiagramViewGrid).Arrange(new Rect( _contentHolder.HorizontalOffset, _contentHolder.VerticalOffset, (viewgrid.Children[1] as DiagramViewGrid).ActualWidth, (viewgrid.Children[1] as DiagramViewGrid).ActualHeight));
                if (BrowserInteropHelper.IsBrowserHosted)
                {
                    if (SymbolPaletteItem.Hasvalue)
                    {
                        SymbolPaletteItem.Hasvalue = false;

                        Node newItem = null;
                        object content = DiagramPage.o;

                        if (content != null)
                        {
                            if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Uid == "sync_L_o")
                            {
                                DropLine(ConnectorType.Orthogonal, e.GetPosition(Page), dc);
                            }
                            else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Uid == "sync_L_s")
                            {
                                DropLine(ConnectorType.Straight, e.GetPosition(Page), dc);
                            }
                            else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Uid == "sync_L_a")
                            {
                                DropLine(ConnectorType.Arc, e.GetPosition(Page), dc);
                            }
                            else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Uid == "sync_L_b")
                            {
                                DropLine(ConnectorType.Bezier, e.GetPosition(Page), dc);
                            }
                            else
                            {
                                string name = (content as UIElement).Uid;
                                newItem = new Node();
                                newItem.Name = name;

                                if (content is System.Windows.Shapes.Path)
                                {
                                    System.Windows.Shapes.Path path = content as System.Windows.Shapes.Path;
                                    path.IsHitTestVisible = false;
                                    newItem.Content = path;
                                }
                                else
                                {
                                    DependencyObject d = new DependencyObject();

                                    if (content is UIElement)
                                    {
                                        (content as UIElement).IsHitTestVisible = false;
                                        newItem.Content = content;
                                    }
                                    else
                                    {
                                        newItem.Content = content;
                                    }
                                }

                                Point position = e.GetPosition(Page);

                                newItem.PxOffsetX = (Math.Max(0, position.X));
                                newItem.PxOffsetY = (Math.Max(0, position.Y));
                                newItem.Width = 50;
                                newItem.Height = 50;

                                if (childcount)
                                {
                                    no = Page.Children.Count + 1;
                                    childcount = false;
                                }

                                namecount = no++;
                                string str = "Node" + namecount;

                                try
                                {
                                    if (string.IsNullOrEmpty(newItem.Name))
                                    {
                                        if (dc.Model.Nodes.Count == 0)
                                        {
                                            newItem.Name = str;
                                        }

                                        foreach (IShape n in dc.Model.Nodes)
                                        {
                                            if (!(n is LineConnector))
                                            {
                                                if (n != null && (n as Node).Name != str)
                                                {
                                                    newItem.Name = str;
                                                }
                                                else
                                                {
                                                    newItem.Name = "new" + str;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }

                                newItem.Page = Page;
                                dc.Model.Nodes.Add(newItem);
                                Panel.SetZIndex(newItem, Page.Children.Count);
                                SelectionList.Clear();
                                SelectionList.Add(newItem);
                                newItem.Focus();
                                NodeDroppedRoutedEventArgs newEventArgs = new NodeDroppedRoutedEventArgs(newItem, name, position);
                                newEventArgs.RoutedEvent = DiagramView.NodeDropEvent;
                                RaiseEvent(newEventArgs);
                            }

                            e.Handled = true;
                        }
                    }
                }
                if (temppath != null)
                {
                    if (DrawingTool == DrawingTools.Ellipse)
                    {

                        draw_ell = new System.Windows.Shapes.Path();
                        draw_ell.Data = new EllipseGeometry();
                        draw_ell.Data = temppath.Data as EllipseGeometry;
                        draw_ell.Stretch = Stretch.Fill;
                        if ((draw_ell.Data as EllipseGeometry).RadiusX != 0 && (draw_ell.Data as EllipseGeometry).RadiusY != 0)
                        {
                            Node n1 = new Node();

                            n1.MeasurementUnits = (Page as DiagramPage).MeasurementUnits;
                            n1.PxOffsetX = (draw_ell.Data as EllipseGeometry).Bounds.Location.X;
                            n1.PxOffsetY = (draw_ell.Data as EllipseGeometry).Bounds.Location.Y;
                            n1.PxWidth = ((draw_ell.Data as EllipseGeometry).RadiusX) * 2;
                            n1.PxHeight = ((draw_ell.Data as EllipseGeometry).RadiusY) * 2;
                            n1.Loaded += new RoutedEventHandler(n1_Loaded);
                            n1.m_NodeDrawing = true;
                            dc.Model.Nodes.Add(n1);
                            SelectionList.Add(n1);
                            DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(n1 as object, this.DrawingTool);
                            newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                            n1.RaiseEvent(newEventArgs);
                            draw_ell.Focus();
                        }
                        Drawtools_Remove(temppath);
                        SelectionList.Clear();
                        temppath = null;
                        e.Handled = true;
                        timeRightClick = DateTime.Now;
                    }
                    else if (DrawingTool == DrawingTools.Rectangle)
                    {
                        draw_ell = new System.Windows.Shapes.Path();
                        draw_ell.Data = new RectangleGeometry();
                        draw_ell.Data = temppath.Data as RectangleGeometry;
                        draw_ell.Stretch = Stretch.Fill;
                        if ((temppath.Data as RectangleGeometry).Rect.Width != 0 && (temppath.Data as RectangleGeometry).Rect.Height != 0)
                        {
                            Node n1 = new Node();
                            n1.MeasurementUnits = (Page as DiagramPage).MeasurementUnits;
                            n1.Loaded += new RoutedEventHandler(n1_Loaded);
                            n1.PxOffsetX = (temppath.Data as RectangleGeometry).Bounds.Location.X;
                            n1.PxOffsetY = (temppath.Data as RectangleGeometry).Bounds.Location.Y;
                            n1.PxWidth = (temppath.Data as RectangleGeometry).Rect.Width;
                            n1.PxHeight = (temppath.Data as RectangleGeometry).Rect.Height;
                            n1.m_NodeDrawing = true;
                            dc.Model.Nodes.Add(n1);
                            SelectionList.Add(n1);
                            DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(n1 as object, this.DrawingTool);
                            newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                            n1.RaiseEvent(newEventArgs);
                            draw_ell.Focus();
                        }
                        Drawtools_Remove(temppath);
                        SelectionList.Clear();
                        temppath = null;
                        e.Handled = true;
                        timeRightClick = DateTime.Now;
                    }
                    else if (DrawingTool == DrawingTools.RoundedRectangle)
                    {

                        draw_ell = new System.Windows.Shapes.Path();
                        draw_ell.Data = new RectangleGeometry();
                        draw_ell.Data = temppath.Data as RectangleGeometry;
                        draw_ell.Stretch = Stretch.Fill;
                        if ((temppath.Data as RectangleGeometry).Rect.Width != 0 && (temppath.Data as RectangleGeometry).Rect.Height != 0)
                        {
                            Node n1 = new Node();
                            n1.MeasurementUnits = (Page as DiagramPage).MeasurementUnits;
                            n1.Loaded += new RoutedEventHandler(n1_Loaded);
                            n1.PxOffsetX = (temppath.Data as RectangleGeometry).Bounds.Location.X;
                            n1.PxOffsetY = (temppath.Data as RectangleGeometry).Bounds.Location.Y;
                            n1.PxWidth = (temppath.Data as RectangleGeometry).Rect.Width;
                            n1.PxHeight = (temppath.Data as RectangleGeometry).Rect.Height;
                            n1.m_NodeDrawing = true;
                            dc.Model.Nodes.Add(n1);
                            SelectionList.Add(n1);
                            DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(n1 as object, this.DrawingTool);
                            newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                            n1.RaiseEvent(newEventArgs);
                            draw_ell.Focus();
                        }
                        Drawtools_Remove(temppath);
                        temppath = null;
                        e.Handled = true;
                        SelectionList.Clear();
                        timeRightClick = DateTime.Now;

                    }
                    else if (DrawingTool == DrawingTools.StraightLine)
                    {
                        if ((temppath.Data as LineGeometry).StartPoint != (temppath.Data as LineGeometry).EndPoint)
                        {
                            ConnectorBase line = null;
                            line = new LineConnector();
                            line.MeasurementUnit = (Page as DiagramPage).MeasurementUnits;
                            line.ConnectorType = ConnectorType.Straight;
                            newconnection = line as LineConnector;
                            line.m_LineDrawing = true;
                            if (Drawing_HeadNode != null)
                            {
                                line.HeadNode = Drawing_HeadNode;
                                if (Drawing_HeadPort != null)
                                {
                                    line.ConnectionHeadPort = Drawing_HeadPort;
                                    Drawing_HeadPort = null;
                                }
                                Drawing_HeadNode = null;
                            }
                            else
                            {
                                line.PxStartPointPosition = (temppath.Data as LineGeometry).StartPoint;
                            }
                            if (Drawing_TailNode != null)
                            {
                                line.TailNode = Drawing_TailNode;
                                if (Drawing_TailPort != null)
                                {
                                    line.ConnectionTailPort = Drawing_TailPort;
                                    Drawing_TailPort = null;
                                }
                                Drawing_TailNode = null;
                            }
                            else
                            {
                                line.PxEndPointPosition = (temppath.Data as LineGeometry).EndPoint;
                            }
                            Drawtools_Remove(temppath);
                            temppath = null;
                            line.InvalidateConnectorPathGeometry();
                            if (line.HeadNode != null && line.TailNode != null)
                            {
                                if (line.HeadNode != line.TailNode)
                                {
                                    dc.Model.Connections.Add(line);
                                    SelectionList.Clear();
                                    SelectionList.Add(line);
                                }
                            }
                            else
                            {
                                dc.Model.Connections.Add(line);
                                SelectionList.Clear();
                                SelectionList.Add(line);
                            }
                            
                            DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(line as object, this.DrawingTool);
                            newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                            line.RaiseEvent(newEventArgs);
                            line.Focus();
                        }
                        
                        ////update selection                        
                       // e.Handled = true;
                        if (this.DrawingMode == DrawingMode.Default)
                        {
                            EnableDrawingTools = false;
                        }
                        timeRightClick = DateTime.Now;
                    }
                    else if (DrawingTool == DrawingTools.BezierLine)
                    {
                        ConnectorBase line = null;
                        line = new LineConnector();
                        newconnection = line as LineConnector;
                        line.MeasurementUnit = (Page as DiagramPage).MeasurementUnits;
                        line.ConnectorType = ConnectorType.Bezier;
                        line.m_LineDrawing = true;
                        if (Drawing_HeadNode != null)
                        {
                            line.HeadNode = Drawing_HeadNode;
                            if (Drawing_HeadPort != null)
                            {
                                line.ConnectionHeadPort = Drawing_HeadPort;
                                Drawing_HeadPort = null;
                            }
                            Drawing_HeadNode = null;
                        }
                        else
                        {
                            line.PxStartPointPosition = pf.StartPoint;
                        }
                        if (Drawing_TailNode != null)
                        {
                            line.TailNode = Drawing_TailNode;
                            if (Drawing_TailPort != null)
                            {
                                line.ConnectionTailPort = Drawing_TailPort;
                                Drawing_TailPort = null;
                            }
                            Drawing_TailNode = null;
                        }
                        else
                        {
                            line.PxEndPointPosition = point3;
                        }                     
                       
                        (temppath.Data as PathGeometry).Figures.Add(pf);
                        Drawtools_Remove(temppath);
                        temppath = null;
                        line.InvalidateConnectorPathGeometry();
                        if (line.HeadNode != null && line.TailNode != null)
                        {
                            if (line.HeadNode != line.TailNode)
                            {
                                dc.Model.Connections.Add(line); 
                                SelectionList.Clear();
                                SelectionList.Add(line);
                            }
                        }
                        else
                        {
                            dc.Model.Connections.Add(line);
                            SelectionList.Clear();
                            SelectionList.Add(line);
                        }
                      
                        DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(line as object, this.DrawingTool);
                        newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                        line.RaiseEvent(newEventArgs);
                        line.Focus();
                        e.Handled = true;
                        line.Focus();
                        if (this.DrawingMode == DrawingMode.Default)
                        {
                            EnableDrawingTools = false;
                        }
                        timeRightClick = DateTime.Now;
                    }

                    else if (DrawingTool == DrawingTools.OrthogonalLine)
                    {

                        ConnectorBase line = null;
                        line = new LineConnector();
                        newconnection = line as LineConnector;
                        line.MeasurementUnit = (Page as DiagramPage).MeasurementUnits;
                        line.ConnectorType = ConnectorType.Orthogonal;
                        line.m_LineDrawing = true;
                        if (Drawing_HeadNode != null)
                        {
                            line.HeadNode = Drawing_HeadNode;
                            if (Drawing_HeadPort != null)
                            {
                                line.ConnectionHeadPort = Drawing_HeadPort;
                                Drawing_HeadPort = null;
                            }
                            Drawing_HeadNode = null;
                        }
                        else
                        {
                            line.PxStartPointPosition = pf.StartPoint;
                        }
                        if (Drawing_TailNode != null)
                        {
                            line.TailNode = Drawing_TailNode;
                            if (Drawing_TailPort != null)
                            {
                                line.ConnectionTailPort = Drawing_TailPort;
                                Drawing_TailPort = null;
                            }
                            Drawing_TailNode = null;
                        }
                        else
                        {
                            line.PxEndPointPosition = temppoly.Points[temppoly.Points.Count - 1];
                        }                        
                       
                        Drawtools_Remove(temppath);
                        temppath = null;
                        line.InvalidateConnectorPathGeometry();
                        if (line.HeadNode != null && line.TailNode != null)
                        {
                            if (line.HeadNode != line.TailNode)
                            {
                                dc.Model.Connections.Add(line);
                                SelectionList.Clear();
                                SelectionList.Add(line);
                            }
                        }
                        else
                        {
                            dc.Model.Connections.Add(line);
                            SelectionList.Clear();
                            SelectionList.Add(line);
                        }
                        
                        DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(line as object, this.DrawingTool);
                        newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                        line.RaiseEvent(newEventArgs);
                        line.Focus();
                       // e.Handled = true;
                        line.Focus();
                        if (this.DrawingMode == DrawingMode.Default)
                        {
                            EnableDrawingTools = false;
                        }
                        timeRightClick = DateTime.Now;
                    }
                    else if (DrawingTool == DrawingTools.Arc)
                    {
                        if (pf.StartPoint!=ac.Point)
                        {
                            ConnectorBase line = null;
                            line = new LineConnector();
                            newconnection = line as LineConnector;
                            line.MeasurementUnit = (Page as DiagramPage).MeasurementUnits;
                            line.ConnectorType = ConnectorType.Arc;
                            line.m_LineDrawing = true;
                            if (Drawing_HeadNode != null)
                            {
                                line.HeadNode = Drawing_HeadNode;
                                if (Drawing_HeadPort != null)
                                {
                                    line.ConnectionHeadPort = Drawing_HeadPort;
                                    Drawing_HeadPort = null;
                                }
                                Drawing_HeadNode = null;
                            }
                            else
                            {
                                line.PxStartPointPosition = pf.StartPoint;
                            }
                            if (Drawing_TailNode != null)
                            {
                                line.TailNode = Drawing_TailNode;
                                if (Drawing_TailPort != null)
                                {
                                    line.ConnectionTailPort = Drawing_TailPort;
                                    Drawing_TailPort = null;
                                }
                                Drawing_TailNode = null;
                            }
                            else
                            {
                                line.PxEndPointPosition = ac.Point;
                            }


                            Drawtools_Remove(temppath);
                            temppath = null;
                            line.InvalidateConnectorPathGeometry();
                            if (line.HeadNode != null && line.TailNode != null)
                            {
                                if (line.HeadNode != line.TailNode)
                                {
                                    dc.Model.Connections.Add(line);
                                    SelectionList.Clear();
                                    SelectionList.Add(line);
                                }
                            }
                            else
                            {
                                dc.Model.Connections.Add(line);
                                SelectionList.Clear();
                                SelectionList.Add(line);
                            }
                            
                            DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(line as object, this.DrawingTool);
                            newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                            line.RaiseEvent(newEventArgs);
                            line.Focus();
                            e.Handled = true;
                            line.Focus();
                            if (this.DrawingMode == DrawingMode.Default)
                            {
                                EnableDrawingTools = false;
                            }
                            if (Drawing_HeadPort != null)
                            {
                                Drawing_HeadPort.IsDragOverPort = false;
                            }
                            timeRightClick = DateTime.Now;
                        }
                    }
                   
                    //if (hitNode!=null && newconnection!=null)
                    //{
                    //    ConnDragEndRoutedEventArgs newEventArgs = new ConnDragEndRoutedEventArgs(sourceNode as Node, HitNode as Node, newconnection as LineConnector);
                    //    newEventArgs.RoutedEvent = DiagramView.ConnectorDragEndEvent;
                    //    RaiseEvent(newEventArgs);
                    //}
                    if(newconnection !=null && hitNode!=null)
                    {
                        ConnDragEndRoutedEventArgs newEventArgs1 = new ConnDragEndRoutedEventArgs(sourceNode as Node, HitNode as Node, newconnection as LineConnector);
                        newEventArgs1.RoutedEvent = DiagramView.AfterConnectionCreateEvent;
                        RaiseEvent(newEventArgs1);
                    }
                    if (DrawingMode == DrawingMode.Continous && DrawingTool != DrawingTools.Polygon && DrawingTool != DrawingTools.PolyLine)
                    {
                        temppath = null;
                    }
                    newconnection = null;
                }
                else if (m_LabelNode != null)
                { 
                    if (DrawingTool == DrawingTools.TextBox)
                    {
                        if (m_LabelNode.Width != 0 && m_LabelNode.Height != 0)
                        {
                            Node labelnode = new Node();
                            CreateLabelNode(labelnode, m_LabelNode);
                            labelnode.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    labelnode.NodeLabeledit();
                                }), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
                        }
                        Drawtools_Remove(m_LabelNode);
                        m_LabelNode = null;
                        m_DragandMove = true;
                        e.Handled = true;
                        timeRightClick = DateTime.Now;
                    }
                }
                
                if (EnableDrawingTools == true && DrawingTool != DrawingTools.Polygon && DrawingTool != DrawingTools.PolyLine && this.DrawingMode == DrawingMode.Default)
                {
                    EnableDrawingTools = false;
                }
            }
        }

        //Creating Node from DrawnNode
        private void CreateLabelNode(Node labelnode, Node LabelNode)
        {
            labelnode.OffsetX = LabelNode.OffsetX;
            labelnode.OffsetY = LabelNode.OffsetY;
            labelnode.Width = LabelNode.Width;
            labelnode.Height = LabelNode.Height;
            //Alignment properties
            labelnode.m_NodeDrawing = true;
            dc.Model.Nodes.Add(labelnode);
            //get the Content
            //LabelEditor box = LabelNode.Content as LabelEditor;
            //box.Style = GetStyle(box);        
            //LabelNode.Content = null;
            //labelnode.Content = box;
            //box.LabelTextWrapping = TextWrapping.Wrap;
            //box.IsHitTestVisible = true;
            //box.Height = double.NaN;
            //register the Events
            //box.TextChanged += new TextChangedEventHandler(box_TextChanged);
            //labelnode.Loaded += new RoutedEventHandler(labelnode_Loaded);
            //labelnode.SizeChanged += new SizeChangedEventHandler(labelnode_SizeChanged);
            SelectionList.Clear();
            SelectionList.Add(labelnode);
            DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(labelnode as object, this.DrawingTool);
            newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
            labelnode.RaiseEvent(newEventArgs);
            //For setting the Keyboard focus
            //box.UpdateLayout();
            //box.Focus();
        }

        ////Setting the Style to the Textbox
        //private System.Windows.Style GetStyle(TextBox box)
        //{
        //    Style textboxstyle = new Style();
        //    textboxstyle.BasedOn=box.Style;
        //    textboxstyle.TargetType = typeof(TextBox);
        //    textboxstyle.Setters.Add(new Setter(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.LightSteelBlue)));
        //    textboxstyle.Setters.Add(new Setter(TextBox.BorderThicknessProperty,new Thickness(2)));
        //    return textboxstyle;
        //}

        //void labelnode_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if(((sender as Node).Ports[0])!=null)
        //    ((sender as Node).Ports[0] as ConnectionPort).Visibility = Visibility.Hidden;
        //}

        //void labelnode_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    TextBox tb = (sender as Node).Content as TextBox;
        //    if ((sender as Node).Width > tb.Width)
        //    {
        //        tb.Width = double.NaN;
        //    }
        //}

        //void box_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox box = (sender as TextBox);           
        //    Node labelnode = (sender as TextBox).Parent as Node;
        //    int numlines = ((int)box.Text.Length + (int)box.FontSize) ;             
        //    if (box.ExtentHeight> labelnode.Height)
        //    {
        //        labelnode.Height = box.ExtentHeight;
        //    }
        //}

        private void Drawtools_Remove(Node LabelNode)
        {
            (Page as DiagramPage).AllChildren.Remove(LabelNode);
            (Page as DiagramPage).Children.Remove(LabelNode);
        }

        //Function for Path to Node Conversion
        void n1_Loaded(object sender, RoutedEventArgs e)
        {
            Node n1 = sender as Node;
            n1.Shape = Shapes.CustomPath;

            Style CPS = new Style();
            CPS.BasedOn = n1.CustomPathStyle;
            CPS.TargetType = typeof(System.Windows.Shapes.Path);
            CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.DataProperty, draw_ell.GetValue(System.Windows.Shapes.Path.DataProperty) as Geometry));
            n1.CustomPathStyle = CPS;
            (sender as Node).Loaded-= new RoutedEventHandler(n1_Loaded);
        }

        //Remove the Path from Page

        private void Drawtools_Remove(System.Windows.Shapes.Path temppath1)
        {
            (Page as DiagramPage).AllChildren.Remove(temppath1);
            (Page as DiagramPage).Children.Remove(temppath1);
        }

        internal void VerifyVirtualization()
        {
            if (this.EnableVirtualization)
            {
                this.ScrollGrid.callCalculate();
            }
            else
            {
                this.ScrollGrid.CallNormailzation();
            }
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);

            if ((Page as DiagramPage).IsPolyLineEnabled == true)
            {
                if (tempPolyLine != null)
                {
                    ConnectorBase line = null;
                    line = new LineConnector();
                    line.ConnectorType = ConnectorType.Straight;
                    if (Drawing_HeadNode != null)
                    {
                        line.HeadNode = Drawing_HeadNode;
                        if (Drawing_HeadPort != null)
                        {
                            line.ConnectionHeadPort = Drawing_HeadPort;
                            Drawing_HeadPort = null;
                        }
                        Drawing_HeadNode = null;
                    }
                    else
                    {
                        line.PxStartPointPosition = tempPolyLine.Points[0];
                    }
                    if (Drawing_TailNode != null)
                    {
                        line.TailNode = Drawing_TailNode;
                        if (Drawing_TailPort != null)
                        {
                            line.ConnectionTailPort = Drawing_TailPort;
                            Drawing_TailPort = null;
                        }
                        Drawing_TailNode = null;
                    }
                    else
                    {
                        line.PxEndPointPosition = tempPolyLine.Points[tempPolyLine.Points.Count - 1];
                    }

                    
                    line.IntermediatePoints = new List<Point>();
                    for (int i = 0; i < tempPolyLine.Points.Count; i++)
                    {
                        if (i != 0 && i != tempPolyLine.Points.Count - 1)
                            line.IntermediatePoints.Add(tempPolyLine.Points[i]);
                    }
                   
                    (Page as DiagramPage).AllChildren.Remove(tempPolyLine);
                    (Page as DiagramPage).Children.Remove(tempPolyLine);
                    tempPolyLine = null;
                    line.InvalidateConnectorPathGeometry();
                    dc.Model.Connections.Add(line);
                    ////update selection
                    SelectionList.Clear();
                    SelectionList.Add(line);
                    e.Handled = true;
                    line.Focus();
                    (Page as DiagramPage).IsPolyLineEnabled = false;
                   
                    timeRightClick = DateTime.Now;
                }
            }

            

            if (temppath != null)
            {
                if (DrawingTool == DrawingTools.Polygon)
                {
                    draw_ell = new System.Windows.Shapes.Path();
                    draw_ell.Data = new PathGeometry();
                    draw_ell.Data = (temppath.Data as PathGeometry);
                    draw_ell.Stretch = Stretch.Fill;
                    Node n2 = new Node();
                    n2.MeasurementUnits = (Page as DiagramPage).MeasurementUnits;
                    n2.Loaded += new RoutedEventHandler(n2_Loaded);
                    n2.PxOffsetX = (temppath.Data as PathGeometry).Bounds.Location.X;
                    n2.PxOffsetY = (temppath.Data as PathGeometry).Bounds.Location.Y;
                    n2.PxWidth = (temppath.Data as PathGeometry).Bounds.Width;
                    n2.PxHeight = (temppath.Data as PathGeometry).Bounds.Height;
                    Drawtools_Remove(temppath);
                    temppath = null;
                    n2.m_NodeDrawing = true;
                    dc.Model.Nodes.Add(n2);
                    SelectionList.Clear();
                    SelectionList.Add(n2);
                    DrawingToolEventArgs newEventArgs = new DrawingToolEventArgs(n2 as object, this.DrawingTool);
                    newEventArgs.RoutedEvent = DiagramView.ObjectDrawnEvent;
                    n2.RaiseEvent(newEventArgs);
                    e.Handled = true;
                    draw_ell.Focus();
                    if (this.DrawingMode == DrawingMode.Default)
                    {
                        EnableDrawingTools = false;
                    }
                    timeRightClick = DateTime.Now;
                }

            }
        }


        void n2_Loaded(object sender, RoutedEventArgs e)
        {
            Node n2 = sender as Node;
            n2.Shape = Shapes.CustomPath;

            Style CPS = new Style();
            CPS.BasedOn = n2.CustomPathStyle;
            CPS.TargetType = typeof(System.Windows.Shapes.Path);
            CPS.Setters.Add(new Setter(System.Windows.Shapes.Path.DataProperty, Geometry.Parse((draw_ell.GetValue(System.Windows.Shapes.Path.DataProperty) as Geometry).ToString())));
            n2.CustomPathStyle = CPS;
            (sender as Node).Loaded -= new RoutedEventHandler(n2_Loaded);
        }
        private bool WithinPageAndView(object p)
        {
            //if (Scrollviewer.Equals(p))
            if (p is FourQuadrantPanel)
            {
                return true;
            }
            else if (p is DiagramPage || p is Adorner)
            {
                return false;
            }
            else if (!(p is Visual))
            {
                return false;
            }
            else
            {
                return WithinPageAndView(VisualTreeHelper.GetParent(p as DependencyObject));
            }
        }

        /// <summary>
        /// Provides class handling for the MouseDown routed event that occurs when the mouse
        /// button is pressed while the mouse pointer is over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        /// 

        void DiagramView_MouseDown(object sender, MouseButtonEventArgs e)
        //protected override void OnMouseDown(MouseButtonEventArgs e)
        {

            dc = DiagramPage.GetDiagramControl(this);
            if (dc.View.IsPageEditable && !(dc.View.IsPanEnabled))
            {
                oldx = e.GetPosition(this.Page as DiagramPage).X;
                oldy = e.GetPosition(this.Page as DiagramPage).Y;
                //base.OnMouseDown(e);
                if ((Page as DiagramPage).IsPolyLineEnabled == true)
                {

                    if ((this as DiagramView).SizeToContent == false)
                    {
                        if ((this as DiagramView).BoundaryConstraintsEnabled == true)
                        {

                            if (BoundaryConstraintsArea.Left + 5 < oldx && BoundaryConstraintsArea.Top + 5 < oldy && BoundaryConstraintsArea.Right - 5 > oldx && BoundaryConstraintsArea.Bottom - 5 > oldy)
                            {
                                (Page as DiagramPage).IsPolyLineEnabled = true;
                                this.m_drawpoly = true;
                            }
                            else
                            {
                                this.m_drawpoly = false;
                            }

                        }
                    }
                }
                if ((Page as DiagramPage).IsPolyLineEnabled == true)
                {

                    if (this.m_drawpoly)
                    {
                        if (tempPolyLine == null)
                        {
                            tempPolyLine = new Polyline();
                            tempPolyLine.Points.Add(new Point(oldx, oldy));
                        }
                        tempPolyLine.Points.Add(Mouse.GetPosition(Page));
                        tempPolyLine.Stroke = new SolidColorBrush(Colors.Black);
                        tempPolyLine.StrokeThickness = 1;
                        if (Page.Children.IndexOf(tempPolyLine) < 0)
                            (Page as DiagramPage).Children.Add(tempPolyLine);
                        (Page as DiagramPage).AllChildren.Add(tempPolyLine);

                    }
                }

                //if (EnableDrawingTools == true && (Mouse.Captured == null || DrawingTool == DrawingTools.Polygon))
                if (EnableDrawingTools == true)
                {
                    EnableConnection = false;
                    if (this.m_DragandMove)
                    {

                        DrawingTools i = (dc.View as DiagramView).DrawingTool;
                        if (DrawAllow == true)
                        {
                            if ((this as DiagramView).SizeToContent == false)
                            {
                                if ((this as DiagramView).BoundaryConstraintsEnabled == true)
                                {

                                    if (BoundaryConstraintsArea.Left + 5 < oldx && BoundaryConstraintsArea.Top + 5 < oldy && BoundaryConstraintsArea.Right - 5 > oldx && BoundaryConstraintsArea.Bottom - 5 > oldy)
                                    {
                                        (this as DiagramView).EnableDrawingTools = true;
                                        this.DrawAllow = true;
                                        (this as DiagramView).DrawingTool = i;
                                    }
                                    else
                                    {
                                        this.DrawAllow = false;
                                    }
                                }
                            }
                        }
                        if (this.DrawAllow == true)
                        {
                            // shapes
                            if (DrawingTool == DrawingTools.Ellipse)
                            {

                                if (temppath == null)
                                {
                                    temppath = new System.Windows.Shapes.Path
                                    {
                                        Style = (Page as DiagramPage).CustomPathStyle,

                                        Data = new EllipseGeometry(new Point(oldx, oldy), 0, 0),
                                        Stretch = Stretch.None

                                    };

                                }
                                Dimensions[0] = 0;
                                Dimensions[1] = 0;
                                temppath.Tag = Dimensions;

                                if (Page.Children.IndexOf(temppath) < 0)
                                    Drawtools_Add(temppath);

                            }

                            else if (DrawingTool == DrawingTools.Rectangle)
                            {

                                if (temppath == null)
                                {
                                    temppath = new System.Windows.Shapes.Path();
                                    temppath.Style = (Page as DiagramPage).CustomPathStyle;
                                    temppath.Stretch = Stretch.None;
                                    RectangleGeometry rg = new RectangleGeometry();
                                    rt.X = oldx;
                                    rt.Y = oldy;
                                    rt.Height = 0;
                                    rt.Width = 0;
                                    rg.Rect = rt;
                                    temppath.Data = rg;
                                }
                                Dimensions[0] = 0;
                                Dimensions[0] = 0;
                                temppath.Tag = Dimensions;
                                if (Page.Children.IndexOf(temppath) < 0)
                                    Drawtools_Add(temppath);
                            }
                            else if (DrawingTool == DrawingTools.RoundedRectangle)
                            {

                                if (temppath == null)
                                {
                                    temppath = new System.Windows.Shapes.Path();
                                    temppath.Style = (Page as DiagramPage).CustomPathStyle;
                                    temppath.Stretch = Stretch.None;
                                    RectangleGeometry rgt = new RectangleGeometry();
                                    rt.X = oldx;
                                    rt.Y = oldy;
                                    rt.Height = 0;
                                    rt.Width = 0;
                                    rgt.RadiusX = 0;
                                    rgt.RadiusY = 0;
                                    rgt.Rect = rt;
                                    temppath.Data = rgt;
                                }

                                Dimensions[0] = 0;
                                Dimensions[0] = 0;
                                temppath.Tag = Dimensions;
                                if (Page.Children.IndexOf(temppath) < 0)

                                    Drawtools_Add(temppath);
                            }
                            else if (DrawingTool == DrawingTools.Polygon && !_iteview)
                            {

                                if (temppath == null)
                                {
                                    temppath = new System.Windows.Shapes.Path();
                                    tempbezier = new PathGeometry();
                                    pf = new PathFigure();
                                    pf.StartPoint = new Point(oldx, oldy);
                                    temppoly = new PolyLineSegment();
                                    PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
                                    pf.Segments = myPathSegmentCollection;
                                    pf.Segments.Add(temppoly);
                                    tempbezier.Figures.Add(pf);
                                    temppath.Data = tempbezier;
                                    temppath.Style = (Page as DiagramPage).CustomPathStyle;
                                    temppath.Stretch = Stretch.None;
                                }
                                temppoly.Points.Add(Mouse.GetPosition(Page));
                                pf.IsClosed = true;
                                pf.IsFilled = true;
                                if (Page.Children.IndexOf(temppath) < 0)
                                    Drawtools_Add(temppath);

                            }

                           //connectors
                            else if (DrawingTool == DrawingTools.StraightLine && Mouse.Captured == null)
                            {

                                if (temppath == null)
                                {
                                    temppath = new System.Windows.Shapes.Path
                                    {

                                        Stroke = Brushes.Black,
                                        Data = new LineGeometry(new Point(oldx, oldy), new Point(oldx, oldy))
                                    };


                                }
                                if (Page.Children.IndexOf(temppath) < 0)
                                    Drawtools_Add(temppath);
                            }

                            else if (DrawingTool == DrawingTools.BezierLine && Mouse.Captured == null)
                            {

                                if (temppath == null)
                                {
                                    temppath = new System.Windows.Shapes.Path();
                                    tempbezier = new PathGeometry();
                                    pf = new PathFigure();
                                    pf.StartPoint = new Point(oldx, oldy);
                                    bs = new BezierSegment();
                                    bs.Point1 = new Point(oldx, oldy);
                                    bs.Point2 = new Point(oldx, oldy);
                                    bs.Point3 = new Point(oldx, oldy);
                                    point3 = bs.Point3;
                                    pf.Segments.Add(bs);
                                    tempbezier.Figures.Add(pf);
                                    temppath.Stroke = Brushes.Black;
                                    temppath.StrokeThickness = 1;
                                    temppath.Data = tempbezier;
                                }
                                if (Page.Children.IndexOf(temppath) < 0)
                                    Drawtools_Add(temppath);
                            }
                            else if (DrawingTool == DrawingTools.PolyLine && Mouse.Captured == null)
                            {
                                if (tempPolyLine == null)
                                {
                                    tempPolyLine = new Polyline();                                  
                                    tempPolyLine.Points.Add(Mouse.GetPosition(Page));
                                }
                                tempPolyLine.Points.Add(Mouse.GetPosition(Page));
                                tempPolyLine.Stroke = new SolidColorBrush(Colors.Black);
                                tempPolyLine.StrokeThickness = 1;
                                if (Page.Children.IndexOf(tempPolyLine) < 0)

                                    (Page as DiagramPage).Children.Add(tempPolyLine);
                                (Page as DiagramPage).AllChildren.Add(tempPolyLine);
                            }


                            else if (DrawingTool == DrawingTools.OrthogonalLine && Mouse.Captured == null)
                            {

                                if (temppath == null)
                                {
                                    temppath = new System.Windows.Shapes.Path();
                                    tempbezier = new PathGeometry();
                                    pf = new PathFigure();
                                    pf.StartPoint = new Point(oldx, oldy);
                                    temppoly = new PolyLineSegment();
                                    pf.Segments.Add(temppoly);
                                    tempbezier.Figures.Add(pf);
                                    temppath.Data = tempbezier;
                                    temppath.Stroke = Brushes.Black;

                                }
                                if (temppoly.Points.Count <= 2)
                                {
                                    temppoly.Points.Add(ortho1);
                                    temppoly.Points.Add(ortho2);
                                    temppoly.Points.Add(ortho3);
                                }
                                if (Page.Children.IndexOf(temppath) < 0)
                                    Drawtools_Add(temppath);

                            }
                            else if (DrawingTool == DrawingTools.Arc && Mouse.Captured == null)
                            {
                                if (temppath == null)
                                {
                                    temppath = new System.Windows.Shapes.Path();
                                    pf = new PathFigure();
                                    tempbezier = new PathGeometry();
                                    pf.StartPoint = new Point(oldx, oldy);
                                    ac = new ArcSegment();
                                    ac.Point = new Point(oldx, oldy);
                                    ac.Size = new Size(0, 0);
                                    ac.SweepDirection = SweepDirection.Clockwise;
                                    pf.Segments.Add(ac);
                                    tempbezier.Figures.Add(pf);
                                    temppath.Data = tempbezier;
                                    temppath.Stroke = Brushes.Black;
                                }
                                if (Page.Children.IndexOf(temppath) < 0)
                                    Drawtools_Add(temppath);
                            }
                            else if (DrawingTool == DrawingTools.TextBox)
                            {
                                if (m_LabelNode == null)
                                {
                                    m_LabelNode = new Node();
                                    m_LabelNode.Content = new TextBox();
                                    m_LabelNode.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                                    m_LabelNode.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                                    //(m_LabelNode.Content as TextBox).Style = GetStyle((m_LabelNode.Content as TextBox));
                                    //m_LabelNode.Loaded += new RoutedEventHandler(labelnode_Loaded);
                                    m_LabelNode.OffsetX = oldx;
                                    m_LabelNode.OffsetY = oldy;
                                }
                                if (Page.Children.IndexOf(m_LabelNode) < 0)
                                    Drawtools_Add(m_LabelNode);
                            }

                        }

                    }
                }
            }
            if (e.LeftButton == MouseButtonState.Pressed || ClearSelectionOnRightClick == true)
            {

                if (e.OriginalSource is DiagramPage || WithinPageAndView(e.OriginalSource) || (e.OriginalSource is Grid))
                {
                    if ((e.OriginalSource is Grid) && ((e.OriginalSource as Grid).TemplatedParent is ScrollViewer) && ((e.OriginalSource as Grid).TemplatedParent as ScrollViewer).Name == "PART_ScrollViewer")
                    {
                        if (!IsPanEnabled)
                            this.startPoint = new Point?(e.GetPosition(this));
                        SelectionList.Clear();
                        Focus();
                    }
                    else
                    {
                        if (!IsPanEnabled)
                            this.startPoint = new Point?(e.GetPosition(this));
                        SelectionList.Clear();
                        Focus();
                    }
                   e.Handled = true;
                }
                else if (Scrollviewer.Equals(e.OriginalSource))
                {
                    this.startPoint = new Point?(e.GetPosition(this));
                    SelectionList.Clear();
                }
            }
           
        }

        private void Drawtools_Add(Node LabelNode)
        {
            if (Page.Children.IndexOf(LabelNode) < 0)

                (Page as DiagramPage).Children.Add(LabelNode);
            (Page as DiagramPage).AllChildren.Add(LabelNode);
        }
        private void Drawtools_Add(System.Windows.Shapes.Path temppath1)
        {
            if (Page.Children.IndexOf(temppath1) < 0)

                (Page as DiagramPage).Children.Add(temppath);
            (Page as DiagramPage).AllChildren.Add(temppath1);

        }


        // Get the DiagramControl ScrollViewer Reference.
        private ScrollViewer ReturnScroll(FrameworkElement element)
        {
            if (element == null)
            {
                return null;
            }
            if (element.TemplatedParent is ScrollViewer)
            {
                return element.TemplatedParent as ScrollViewer;
            }
            else
            {
                return ReturnScroll(element.TemplatedParent as FrameworkElement);
            }
        }


        #region Class variables

        /*
        /// <summary>
        /// Used to store the path geometry.
        /// </summary>
        //private PathGeometry m_pathGeometry;

        /// <summary>
        /// Used to store the page instance.
        /// </summary>
        //private IDiagramPage m_diagramPage;

        /// <summary>
        /// Used to store the view instance.
        /// </summary>
        //private DiagramView dview;
        */

        /// <summary>
        /// Used to store the source node.
        /// </summary>
        internal Node sourceNode;

        /// <summary>
        /// Used to store the hit node.
        /// </summary>
        private Node hitNode;

        /*
        /// <summary>
        /// Used to store the drawing pen.
        /// </summary>
       // private Pen drawingPen;
        */

        /// <summary>
        /// Used to store the previously hit node.
        /// </summary>
        private Node previousHitNode = null;

        /// <summary>
        /// Used to store the  current hit port.
        /// </summary>
        private ConnectionPort m_hitPort;

        /// <summary>
        /// Used to store the previously hit port.
        /// </summary>
        private ConnectionPort previoushitport = null;

        

        /// <summary>
        /// Used to store the source port.
        /// </summary>
        internal ConnectionPort sourceHitPort;

        /// <summary>
        /// Used to store the boolean information of center port being hit.
        /// </summary>
        internal bool centerhit = false;

        /*
        /// <summary>
        /// Used to store the center port.
        /// </summary>
        //private ConnectionPort centerport;

        /// <summary>
        /// Used to store the boolean information about the instance of connection
        /// </summary>
        //private bool beforeconn = false;

        /// <summary>
        ///  Used to check port visibility om hittesting.
        /// </summary>
        //private bool isportcheck = false;
        */

        #endregion

        #region  Properties

        /// <summary>
        /// Gets or sets the hit port.
        /// </summary>
        /// <value>The hit port.</value>
        internal ConnectionPort HitPort
        {
            get
            {
                return m_hitPort;
            }

            set
            {
                if (m_hitPort != value)
                {
                    m_hitPort = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the node which is currently selected through HitTesting.
        /// </summary>
        /// <value>
        /// Type: <see cref="Node"/>
        /// The Node which was hit.
        /// </value>
        internal Node HitNode
        {
            get
            {
                return hitNode;
            }

            set
            {
                if (hitNode != value)
                {
                    hitNode = value;
                }
            }
        }

        #endregion

        internal Node Drawing_HeadNode;
        internal Node Drawing_TailNode;
        internal ConnectionPort Drawing_HeadPort;
        internal ConnectionPort Drawing_TailPort;
        internal bool _ConnectionOnPort=false;
        internal bool _LineRoute = false;

        /// <summary>
        /// Identifies the PortMode dependency property.
        /// </summary>
        /// 
        public ConnectionMode PortMode
        {
            get { return (ConnectionMode)GetValue(PortModeProperty); }
            set { SetValue(PortModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PortMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PortModeProperty =
            DependencyProperty.Register("PortMode", typeof(ConnectionMode), typeof(DiagramView), new PropertyMetadata(ConnectionMode.Connect, new PropertyChangedCallback(OnDrawingModeChanged)));


        /// <summary>
        /// Identifies the NodeMode dependency property.
        /// </summary>
        /// 
        public ConnectionMode NodeMode
        {
            get { return (ConnectionMode)GetValue(NodeModeProperty); }
            set { SetValue(NodeModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NodeMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NodeModeProperty =
            DependencyProperty.Register("NodeMode", typeof(ConnectionMode), typeof(DiagramView), new PropertyMetadata(ConnectionMode.Connect, new PropertyChangedCallback(OnDrawingModeChanged)));

        /// <summary>
        /// Identifies the hit object.
        /// </summary>
        /// <param name="hitPoint">The point to be tested.</param>
        /// <returns>True if hit object is Node ,false otherwise.</returns>
        internal bool HitTesting(Point hitPoint)
        {
            //bool isporthit = false;
            DependencyObject hitelement = (Page as Panel).InputHitTest(hitPoint) as DependencyObject;
            while (hitelement != null/* &&
                   hitelement != sourceNode.ParentNode*/)
            {
                if (hitelement is ConnectionPort)
                {
                    if ((hitelement as ConnectionPort).CenterPortReferenceNo == 0)
                    {
                        //(hitelement as ConnectionPort).IsDragOverPort = true;

                        //centerport = hitelement as ConnectionPort;

                        //if (previoushitport != null)
                        //{
                        //    previoushitport.IsDragOverPort = false;
                        //}

                        HitPort = null;
                        previoushitport = null;
                        centerhit = true;
                    }
                    else
                    {
                        centerhit = false;
                        foreach (Node n in dc.Model.Nodes)
                        {
                            foreach (ConnectionPort port in n.Ports)
                            {
                              //  port.IsDragOverPort = false;
                                if (port == hitelement as ConnectionPort)
                                {
                                    HitPort = hitelement as ConnectionPort;
                                    HitNode = (hitelement as ConnectionPort).Node;
                                    previoushitport = hitelement as ConnectionPort;
                                    if (this.PortMode == ConnectionMode.Connect)
                                    {
                                        previoushitport.IsDragOverPort = true;
                                    }
                                    //isporthit = true;
                                }
                            }
                        }
                    }

                    try
                    {
                        foreach (Group g in ((hitelement as ConnectionPort).Node as Node).Groups)
                        {
                           // g.IsDragConnectionOver = false;
                        }
                    }
                    catch
                    {
                    }

                    HitNode = (hitelement as ConnectionPort).Node;
                    previousHitNode = (hitelement as ConnectionPort).Node;
                    if (HitNode != sourceNode)
                    {
                        //if (!isporthit)
                        //{
                        //    previousHitNode.IsDragConnectionOver = true;
                        //}
                    }

                    return true;
                }

                if (hitelement is Node)
                {
                    Node node = hitelement as Node;

                    if (!node.IsGrouped)
                    {
                        HitNode = hitelement as Node;
                        previousHitNode = hitelement as Node;
                        previoushitport = null;
                        HitPort = null;

                        if (HitNode != sourceNode)
                        {
                           // previousHitNode.IsDragConnectionOver = true;
                        }

                        foreach (Group g in HitNode.Groups)
                        {
                           // g.IsDragConnectionOver = false;
                        }

                        return true;
                    }
                    else
                    {
                        if (node.IsGrouped)
                        {
                           // node.IsDragConnectionOver = true;

                            HitNode = node.Groups[node.Groups.Count - 1] as Node;
                            previousHitNode = node.Groups[node.Groups.Count - 1] as Node;
                            if (previoushitport != null)
                            {
                               // previoushitport.IsDragOverPort = false;
                            }

                            //if (centerport != null)
                            //{
                            //    //centerport.IsDragOverPort = false;
                            //}

                            previoushitport = null;
                            HitPort = null;
                            if (HitNode != sourceNode)
                            {
                                //previousHitNode.IsDragConnectionOver = true;
                            }

                            return true;
                        }
                    }

                    if (centerhit)
                    {
                        return true;
                    }

                    return false;
                }

                hitelement = VisualTreeHelper.GetParent(hitelement);
            }

           // isportcheck = false;
            if (previousHitNode != null)
            {
                //previousHitNode.IsDragConnectionOver = false;
            }

            if (previoushitport != null)
            {
                //previoushitport.IsDragOverPort = false;
            }

            //if (centerport != null)
            //{
            //   // centerport.IsDragOverPort = false;
            //}

            HitNode = null;
            previoushitport = null;
            HitPort = null;

            return false;
        }

        internal LineConnector newconnection;

        /// <summary>
        /// Provides class handling for the MouseMove routed event that occurs when the mouse
        /// pointer  is over this control.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsPageEditable)
            {
                base.OnMouseMove(e);

                if (BrowserInteropHelper.IsBrowserHosted)
                {
                    if (SymbolPaletteItem.Hasvalue)
                    {
                        this.Cursor = Cursors.IBeam;
                    }
                    else
                    {
                        this.Cursor = Cursors.Arrow;
                    }
                }

                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    this.startPoint = null;                    
                }

                if (this.startPoint.HasValue && (e.LeftButton== MouseButtonState.Pressed))
                {
                    if (this.ItemSelectionMode == ItemSelectionMode.Multiple)
                    {
                        AdornerLayer adorner = AdornerLayer.GetAdornerLayer(this);
                        if (adorner != null)
                        {
                            if (this.EnableDrawingTools == false && (this.Page as DiagramPage).IsPolyLineEnabled==false)
                            {
                                NodeSelectionAdorner nodeadorner = new NodeSelectionAdorner(this, startPoint);
                                if (adorner != null)
                                {
                                    adorner.Add(nodeadorner);
                                }
                                nodeadorner.CaptureMouse();
                                e.Handled = true;
                            }
                        }
                    }
                }


                if ((Page as DiagramPage).IsPolyLineEnabled == true)
                {
                    double newx = Mouse.GetPosition(Page as DiagramPage).X;
                    double newy = Mouse.GetPosition(Page as DiagramPage).Y;
                    if ((dc.View as DiagramView).SizeToContent == false)
                    {
                        if ((dc.View as DiagramView).BoundaryConstraintsEnabled == true)
                        {
                            if (BoundaryConstraintsArea.Left + 5 < newx && BoundaryConstraintsArea.Top + 5 < newy && BoundaryConstraintsArea.Right - 5 > newx && BoundaryConstraintsArea.Bottom - 5 > newy)
                            {

                                this.m_drawpoly = true;
                            }
                            else
                            {
                                this.m_drawpoly = false;
                                
                            }
                        }
                    }
                    if (this.m_drawpoly)
                    {
                        if (tempPolyLine != null)
                        {
                            tempPolyLine.Points[tempPolyLine.Points.Count - 1] = Mouse.GetPosition(Page);
                        }
                    }
                }

                if (EnableDrawingTools == true)
                {
                    double newx = Mouse.GetPosition(Page as DiagramPage).X;
                    double newy = Mouse.GetPosition(Page as DiagramPage).Y;

                    DrawingTools i = (dc.View as DiagramView).DrawingTool;
                    if ((dc.View as DiagramView).SizeToContent == false)
                    {
                        if ((dc.View as DiagramView).BoundaryConstraintsEnabled == true)
                        {
                            if (BoundaryConstraintsArea.Left + 5 < newx && BoundaryConstraintsArea.Top + 5 < newy && BoundaryConstraintsArea.Right - 5 > newx && BoundaryConstraintsArea.Bottom - 5 > newy)
                            {

                                (dc.View as DiagramView).EnableDrawingTools = true;
                                this.DrawAllow = true;
                                (dc.View as DiagramView).DrawingTool = i;
                            }
                            else
                            {

                                this.DrawAllow = false;
                            }
                        }
                    }

                    if (this.DrawAllow == true)
                    {
                        if (DrawingTool == DrawingTools.Polygon)
                        {
                            
                            if (temppath != null)
                            {
                                //temppath.Style = CustomPathStyle;
                                temppath.Cursor = Cursors.Cross;
                                temppath.CaptureMouse();
                                pf.IsClosed = true;
                                pf.IsFilled = true;
                                temppoly.Points[temppoly.Points.Count - 1] = e.GetPosition(Page);
                            }
                        }

                        else if (DrawingTool == DrawingTools.BezierLine)
                        {

                            if (temppath != null)
                            {
                                temppath.Style = CustomPathStyle;
                                temppath.Cursor = Cursors.Cross;
                             //   temppath.CaptureMouse();
                                double diffx = Math.Abs(newx - oldx);
                                double x = new double();
                                double y = new double();
                                double dis = new double();

                                Point b1 = new Point();
                                Point b2 = new Point();
                                Point b3 = new Point();


                                if (oldx > 0)
                                {
                                    x = Math.Pow((oldx - newx), 2);
                                    y = Math.Pow((oldy - newy), 2);
                                    dis = Math.Sqrt((x + y) / 4);

                                    if (newx <= oldx)
                                    {
                                        bs.Point1 = new Point(Math.Abs(oldx - dis), oldy);
                                        bs.Point2 = new Point(Math.Abs(newx + dis), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;
                                    }
                                    else
                                    {
                                        bs.Point1 = new Point(Math.Abs(oldx - dis), oldy);
                                        bs.Point2 = new Point(Math.Abs(newx + dis), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;
                                    }
                                    if (newx <= 0)
                                    {
                                        b1 = new Point((Math.Abs(oldx - dis)), oldy);
                                        b2 = new Point((Math.Abs(newx + dis)), newy);
                                        b3 = new Point(newx, newy);
                                        bs.Point1 = new Point((b1.X), oldy);
                                        bs.Point2 = new Point(-(b2.X), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;
                                    }
                                }

                                else if (oldx < 0)
                                {
                                    x = Math.Pow((oldx - newx), 2);
                                    y = Math.Pow((oldy - newy), 2);
                                    dis = Math.Sqrt((x + y) / 4);

                                    b1 = new Point(Math.Abs((-oldx) + dis), oldy);
                                    b2 = new Point(Math.Abs((-newx) - dis), newy);
                                    b3 = new Point(newx, newy);
                                    if (newx >= oldx)
                                    {
                                        b1 = new Point(Math.Abs(oldx - dis), oldy);
                                        b2 = new Point(Math.Abs(newx + dis), newy);
                                        b3 = new Point(newx, newy);
                                        bs.Point1 = new Point(-(b1.X), oldy);
                                        bs.Point2 = new Point(-(b2.X), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;
                                    }
                                    else
                                    {
                                        b1 = new Point(Math.Abs(oldx - dis), oldy);
                                        b2 = new Point(Math.Abs(newx + dis), newy);
                                        b3 = new Point(newx, newy);
                                        bs.Point1 = new Point(-(b1.X), oldy);
                                        bs.Point2 = new Point(-(b2.X), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;

                                    }
                                    if (newx >= 0)
                                    {

                                        if (newx > 0)
                                        {
                                            b1 = new Point(Math.Abs((oldx) - dis), oldy);
                                            b2 = new Point(Math.Abs((newx) + dis), newy);
                                            b3 = new Point(newx, newy);
                                            bs.Point1 = new Point(-(b1.X), oldy);
                                            bs.Point2 = new Point((b2.X), newy);
                                            bs.Point3 = new Point(newx, newy);
                                            point3 = bs.Point3;
                                        }

                                    }

                                }


                            }

                        }
                        else if (DrawingTool == DrawingTools.PolyLine)
                        {
                            if (tempPolyLine != null)
                            {
                                tempPolyLine.Points[tempPolyLine.Points.Count - 1] = Mouse.GetPosition(Page);
                            }
                        }
                        else if (DrawingTool == DrawingTools.OrthogonalLine)
                        {
                            ortho1 = e.GetPosition(Page);
                            ortho2 = e.GetPosition(Page);
                            ortho3 = e.GetPosition(Page);


                            if (temppath != null)
                            {
                                temppath.Style = CustomPathStyle;
                               
                                temppath.Cursor = Cursors.Cross;
                              //  temppath.CaptureMouse();
                                double newy1 = new double();
                                double newx1 = new double();
                                if (temppoly.Points.Count <= 3)
                                {

                                    if ((oldx > 0))
                                    {
                                        double diffy1 = 25;
                                        if (newy > oldy)
                                        {
                                            newy1 = diffy1 + oldy;
                                        }
                                        else
                                        {
                                            //newy1 = Math.Abs(diffy1 - oldy);
                                            newy1 = (diffy1 - oldy);
                                            newy1 = -newy1;
                                        }

                                        ortho1 = new Point(oldx, newy1);
                                        temppoly.Points[0] = ortho1;
                                        double diffx1 = ((newx - oldx));
                                        if (newx > oldx)
                                        {
                                            newx1 = diffx1 + oldx;
                                        }
                                        else
                                        {
                                            if (diffx1 < 0)
                                            {
                                                newx1 = (diffx1 + oldx);
                                            }
                                            else
                                            {
                                                newx1 = (diffx1 + oldx);
                                            }
                                        }
                                        ortho2 = new Point(newx1, newy1);
                                        temppoly.Points[1] = ortho2;
                                        ortho3 = new Point(newx1, newy);
                                        temppoly.Points[2] = ortho3;
                                    }
                                    else
                                    {
                                        double diffy1 = 25;
                                        if (newy > oldy)
                                        {
                                            newy1 = diffy1 + oldy;
                                        }
                                        else
                                        {
                                            //newy1 = Math.Abs(oldy - diffy1);
                                            newy1 = oldy - diffy1;
                                        }

                                        ortho1 = new Point(oldx, newy1);
                                        temppoly.Points[0] = ortho1;
                                        if (newx > oldx)
                                        {
                                            double diffx1 = ((newx - oldx));
                                            if (diffx1 < 0)
                                            {
                                                newx1 = diffx1 - oldx;
                                            }
                                            else
                                            {
                                                newx1 = diffx1 + oldx;
                                            }

                                        }
                                        else
                                        {
                                            double diffx1 = ((oldx + newx));
                                            if (diffx1 < 0)
                                            {
                                                newx1 = diffx1 - oldx;
                                            }
                                            else
                                            {
                                                newx1 = diffx1 + oldx;
                                            }

                                        }
                                        ortho2 = new Point(newx1, newy1);
                                        temppoly.Points[1] = ortho2;
                                        ortho3 = new Point(newx1, newy);
                                        temppoly.Points[2] = ortho3;
                                    }

                                }

                            }
                        }

                        if (Mouse.LeftButton == MouseButtonState.Pressed&&!_iteview)
                        {
                            
                            if (DrawingTool == DrawingTools.Ellipse)
                            {

                                if (temppath != null)
                                {
                                    temppath.Cursor = Cursors.Cross;
                                    (temppath.Data as EllipseGeometry).Center = new Point(((newx + oldx) / 2), ((newy + oldy) / 2));
                                    (temppath.Data as EllipseGeometry).RadiusX = Math.Abs((newx - oldx) / 2);
                                    (temppath.Data as EllipseGeometry).RadiusY = Math.Abs((newy - oldy) / 2);
                                    temppath.CaptureMouse();
                                    double[] Dimensions = new double[2] { (temppath.Data as EllipseGeometry).RadiusX, (temppath.Data as EllipseGeometry).RadiusY };
                                }
                            }
                            else if (DrawingTool == DrawingTools.Rectangle)
                            {
                                if (temppath != null)
                                {
                                    temppath.Cursor = Cursors.Cross;
                                    Point startpoint = new Point(oldx, oldy);
                                    temppath.CaptureMouse();
                                    Point startpoint_rect = new Point(oldx, oldy);
                                    if ((e.GetPosition(Page).Y < oldy))
                                    {
                                        startpoint_rect.X = oldx;
                                        startpoint_rect.Y = e.GetPosition(Page).Y;
                                    }
                                    if (e.GetPosition(Page).X < oldx)
                                    {
                                        startpoint_rect.Y = oldy;
                                        startpoint_rect.X = e.GetPosition(Page).X;
                                    }
                                    if ((e.GetPosition(Page).X < oldx) && (e.GetPosition(Page).Y < oldy))
                                    {
                                        startpoint_rect.X = e.GetPosition(Page).X;
                                        startpoint_rect.Y = e.GetPosition(Page).Y;
                                    }

                                    (temppath.Data as RectangleGeometry).Rect = new Rect(new Point(startpoint_rect.X, startpoint_rect.Y), new Size(Math.Abs(oldx - newx), Math.Abs(oldy - newy)));
                                    double[] Dimensions = new double[2] { (temppath.Data as RectangleGeometry).RadiusX, (temppath.Data as RectangleGeometry).RadiusY };
                                }
                            }

                            else if (DrawingTool == DrawingTools.RoundedRectangle)
                            {

                                if (temppath != null)
                                {
                                    temppath.Cursor = Cursors.Cross;
                                    temppath.CaptureMouse();
                                    Point startpoint_rect = new Point(oldx, oldy);
                                    if ((e.GetPosition(Page).Y < oldy))
                                    {
                                        startpoint_rect.X = oldx;
                                        startpoint_rect.Y = e.GetPosition(Page).Y;
                                    }
                                    if (e.GetPosition(Page).X < oldx)
                                    {
                                        startpoint_rect.Y = oldy;
                                        startpoint_rect.X = e.GetPosition(Page).X;
                                    }
                                    if ((e.GetPosition(Page).X < oldx) && (e.GetPosition(Page).Y < oldy))
                                    {
                                        startpoint_rect.X = e.GetPosition(Page).X;
                                        startpoint_rect.Y = e.GetPosition(Page).Y;
                                    }
                                    (temppath.Data as RectangleGeometry).RadiusX = Math.Abs(newx - oldx) / 16;
                                    (temppath.Data as RectangleGeometry).RadiusY = Math.Abs(newy - oldy) / 16;
                                    (temppath.Data as RectangleGeometry).Rect = new Rect(new Point(startpoint_rect.X, startpoint_rect.Y), new Size(Math.Abs(newx - oldx), Math.Abs(newy - oldy)));
                                }

                            }

                            else if (DrawingTool == DrawingTools.StraightLine)
                            {
                                if (temppath != null)
                                {
                                    temppath.Cursor = Cursors.Cross;
                                   // temppath.CaptureMouse();
                                    temppath.Style = CustomPathStyle;
                                    (temppath.Data as LineGeometry).StartPoint = new Point(oldx, oldy);
                                    (temppath.Data as LineGeometry).EndPoint = new Point(newx, newy);
                                }

                            }
                            else if (DrawingTool == DrawingTools.Arc)
                            {
                                if (temppath != null)
                                {
                                    temppath.Style = CustomPathStyle;
                                    temppath.Cursor = Cursors.Cross;
                                   // temppath.CaptureMouse();
                                    double x = Math.Pow((newx - oldx), 2);
                                    double y = Math.Pow((newy - oldy), 2);
                                    double d = Math.Sqrt(x + y);
                                    ac.Point = new Point(newx, newy);
                                    ac.Size = new Size(d / 2, 50);
                                    ac.IsLargeArc = false;
                                    ac.RotationAngle = ConnectorBase.findAngle(new Point(oldx, oldy), new Point(newx, newy));
                                }
                            }
                            else if (DrawingTool == DrawingTools.TextBox)
                            {
                                if (m_LabelNode != null)
                                {
                                    m_LabelNode.Cursor = Cursors.Cross;
                                    m_LabelNode.CaptureMouse();
                                    //LabelEditor box = m_LabelNode.Content as LabelEditor;   
                                    m_LabelNode.Width = Math.Abs(newx - oldx);
                                    m_LabelNode.Height = Math.Abs(newy - oldy);
                                    //box.Width = Math.Abs(newx - oldx);
                                    //box.Height = Math.Abs(newy - oldy);
                                    m_LabelNode.NodeLabeledit();
                                    m_LabelNode.Focus();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Drops the line.
        /// </summary>
        /// <param name="connectortype">The connector type.</param>
        /// <param name="position">The position.</param>
        /// <param name="diagctrl">The diagram control object.</param>
        private void DropLine(ConnectorType connectortype, Point position, DiagramControl diagctrl)
        {
            PreviewConnectorDropEventRoutedEventArgs linedropnewEventArgs = new PreviewConnectorDropEventRoutedEventArgs();
            linedropnewEventArgs.RoutedEvent = DiagramView.PreviewConnectorDropEvent;
            RaiseEvent(linedropnewEventArgs);
            if (linedropnewEventArgs.Cancel == false)
            {
                ConnectorBase line = null;

                if (linedropnewEventArgs.Connector != null)
                {
                    line = linedropnewEventArgs.Connector as ConnectorBase;
                }
                else
                {
                    line = new LineConnector();
                }

                line.ConnectorType = connectortype;
                line.DropPoint = position;
                line.MeasurementUnit = (Page as DiagramPage).MeasurementUnits;
                line.PxStartPointPosition = new Point(line.DropPoint.X - 25, line.DropPoint.Y - 25);
                line.PxEndPointPosition = new Point(line.DropPoint.X + 25, line.DropPoint.Y + 25);
                if (!this.Page.Children.Contains(line as Control))
                {

                    (this.Page as DiagramPage).AllChildren.Add(line as Control);
                    if (this.EnableVirtualization)
                    {
                        this.ScrollGrid.VirtualizingLine(line as LineConnector);
                    }
                    else
                    {
                        (this.Page as DiagramPage).AddingChildren(line as LineConnector);
                    }
                    Panel.SetZIndex(line as Control, this.Page.Children.Count);
                }
                line.InvalidateConnectorPathGeometry();
                diagctrl.Model.Connections.Add(line);
                line.Loaded += new RoutedEventHandler(DropLine_Loaded);

                ////update selection
                this.SelectionList.Clear();
                this.SelectionList.Add(line);
                line.Focus();
                ConnectorDroppedRoutedEventArgs newEventArgs = new ConnectorDroppedRoutedEventArgs(line as ConnectorBase);
                newEventArgs.RoutedEvent = DiagramView.ConnectorDropEvent;
                RaiseEvent(newEventArgs);
                if (dc != null)
                {
                    foreach (Layer l in dc.Model.Layers)
                    {
                        if (l.Active)
                        {
                            l.Lines.Add(line as LineConnector);
                        }
                    }
                }

            }
        }

        private void DropLine(ConnectorBase line, Point position, DiagramControl diagctrl)
        {
            PreviewConnectorDropEventRoutedEventArgs linedropnewEventArgs = new PreviewConnectorDropEventRoutedEventArgs();
            linedropnewEventArgs.RoutedEvent = DiagramView.PreviewConnectorDropEvent;
            RaiseEvent(linedropnewEventArgs);
            line.IsGrouped = false;
            line.IsHitTestVisible = true;
            if (linedropnewEventArgs.Cancel == false)
            {
                if (linedropnewEventArgs.Connector != null)
                {
                    line = linedropnewEventArgs.Connector as ConnectorBase;
                }
                line.DropPoint = position;
                line.MeasurementUnit = (Page as DiagramPage).MeasurementUnits;
                line.PxStartPointPosition = new Point(line.DropPoint.X - 50, line.DropPoint.Y - 50);
                line.PxEndPointPosition = new Point(line.DropPoint.X + 50, line.DropPoint.Y + 50);
                if (!this.Page.Children.Contains(line as Control))
                {

                    (this.Page as DiagramPage).AllChildren.Add(line as Control);
                    if (this.EnableVirtualization)
                    {
                        this.ScrollGrid.VirtualizingLine(line as LineConnector);
                    }
                    else
                    {
                        (this.Page as DiagramPage).AddingChildren(line as LineConnector);
                    }
                    Panel.SetZIndex(line as Control, this.Page.Children.Count);
                }
                line.InvalidateConnectorPathGeometry();
                diagctrl.Model.Connections.Add(line);
                line.Loaded += new RoutedEventHandler(DropLine_Loaded);
                ////update selection
                this.SelectionList.Clear();
                this.SelectionList.Add(line);
                line.Focus();
                ConnectorDroppedRoutedEventArgs newEventArgs = new ConnectorDroppedRoutedEventArgs(line as ConnectorBase);
                newEventArgs.RoutedEvent = DiagramView.ConnectorDropEvent;
                RaiseEvent(newEventArgs);
                if (dc != null)
                {
                    foreach (Layer l in dc.Model.Layers)
                    {
                        if (l.Active)
                        {
                            l.Lines.Add(line as LineConnector);
                        }
                    }
                }

            }
        }

        #endregion
        private void Selection()
        {
            if (this.SelectionList.Count > 1 && this.ItemSelectionMode == ItemSelectionMode.Single)
            {
                object select = this.SelectionList[this.SelectionList.Count - 1] as object;
                this.SelectionList.Clear();
                this.SelectionList.Add(select);
            }
        }

    }
}