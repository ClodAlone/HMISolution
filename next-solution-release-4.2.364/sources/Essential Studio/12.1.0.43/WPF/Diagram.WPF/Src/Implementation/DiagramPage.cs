// <copyright file="DiagramPage.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using Syncfusion.Windows.Shared;
using System.Windows.Data;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the diagram page .
    /// <para> The DiagramPage is just a container to hold the objects(nodes and connectors) added through model.
    /// The DiagramView uses the page to display the diagram objects.
    /// </para>
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public partial class DiagramPage : VirtualizingPanel, IDiagramPage, INotifyPropertyChanged
    {
        #region Class variables

        internal double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Top.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(DiagramPage), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnTopChanged));

        internal double Bottom
        {
            get { return (double)GetValue(BottomProperty); }
            set { SetValue(BottomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bottom.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BottomProperty =
            DependencyProperty.Register("Bottom", typeof(double), typeof(DiagramPage), new UIPropertyMetadata(0d));

        internal double Right
        {
            get { return (double)GetValue(RightProperty); }
            set { SetValue(RightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Right.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RightProperty =
            DependencyProperty.Register("Right", typeof(double), typeof(DiagramPage), new UIPropertyMetadata(0d));

        internal double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

      
        // Using a DependencyProperty as the backing store for Left.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(DiagramPage), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnLeftChanged));

        private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs evtArgs)
        {
            DiagramPage page = d as DiagramPage;
            if (page.dview != null)
            {
                //((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).Top += (double)evtArgs.NewValue - (double)evtArgs.OldValue;
                if (!page.dview.IsPanEnabled)
                {
                    OverviewContentHolder.SetOrigin(page.dview.ScrollGrid, new Point(-page.Left, -page.Top));
                    ((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).InvalidateMeasure();
                }
                //((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft = new Point(((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft.X, ((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft.Y + (double)evtArgs.NewValue - (double)evtArgs.OldValue);
            }
        }

        private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs evtArgs)
        {
            DiagramPage page = d as DiagramPage;
            if (page.dview != null)
            {
                if (!page.dview.IsPanEnabled)
                {
                    OverviewContentHolder.SetOrigin(page.dview.ScrollGrid, new Point(-page.Left, -page.Top));
                    //((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).Left+= (double)evtArgs.NewValue - (double)evtArgs.OldValue;
                    //if (dview.ScrollGrid != null)
                    //{
                    //OverviewContentHolder.SetOrigin(dview.ScrollGrid, new Point(-Left, -Top));
                    // }
                    ((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).InvalidateMeasure();
                }
                //((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft = new Point(((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft.X + (double)evtArgs.NewValue - (double)evtArgs.OldValue, ((page.dview.Scrollviewer as ScrollViewer).Content as OverviewContentHolder).TopLeft.Y);
            }
        }

        /// <summary>
        /// Used to store a static object.
        /// </summary>
        internal static object o;

        /// <summary>
        /// Used to store selection list
        /// </summary>
        private NodeCollection m_selectionList;

        /// <summary>
        /// Used to store the View instance.
        /// </summary>
        internal DiagramView dview;

        /// <summary>
        /// Used to store the diagram control.
        /// </summary>
        private DiagramControl dc;        
        
        #endregion

        #region Initialization

        static DiagramPage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramPage), new FrameworkPropertyMetadata(typeof(DiagramPage)));
        }

        internal List<UIElement> AllChildren;
        internal ObservableCollection<UIElement> RealizedChildren;
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramPage"/> class.
        /// </summary>
        public DiagramPage()
        {

            this.AllChildren = new List<UIElement>();
            this.RealizedChildren = new ObservableCollection<UIElement>();
            this.AllowDrop = true;
            this.Focus();
            this.Focusable = true;
            FocusManager.SetIsFocusScope(this, true);
            this.Loaded += new RoutedEventHandler(DiagramPage_Loaded);
            if (!this.CaptureMouse())
            {
                this.Background = Brushes.Transparent;
            }
            this.SizeChanged += new SizeChangedEventHandler(DiagramPage_SizeChanged);
            
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (dview.startPoint.HasValue && (e.LeftButton == MouseButtonState.Pressed)&&!dview.IsPanEnabled)
            //{
            //    if (dview.ItemSelectionMode == ItemSelectionMode.Multiple)
            //    {
            //        AdornerLayer adorner = AdornerLayer.GetAdornerLayer(this);
            //        if (adorner != null)
            //        {
            //            if (dview.EnableDrawingTools == false)
            //            {
            //                NodeSelectionAdorner nodeadorner = new NodeSelectionAdorner(dview, dview.startPoint);
            //                if (adorner != null)
            //                {
            //                    adorner.Add(nodeadorner);
            //                }
            //                this.UpdateLayout();
            //            }
            //        }
            //    }
            //}
            base.OnMouseMove(e);
        }
        void DiagramPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (dview != null && dview.ScrollGrid != null)
            { 
                dview.ScrollGrid.InvalidateMeasure();
            }
        }
                
        /// <summary>
        /// Is called when the diagram page gets loaded.
        /// </summary>
        /// <param name="sender">Diagram page</param>
        /// <param name="e">Event args</param>
        private void DiagramPage_Loaded(object sender, RoutedEventArgs e)
        {
            dc = GetDiagramControl(this);
            dview = dc.View;
            double width = Math.Max(dview.Scrollviewer.ActualWidth, dview.Scrollviewer.ExtentWidth);
            double height = Math.Max(dview.Scrollviewer.ActualHeight, dview.Scrollviewer.ExtentHeight);
            this.InvalidateMeasure();
            this.dview.ScrollGrid.InvalidateMeasure();
        }

        #endregion

        #region Class Override
        
        //protected override 
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

                DependencyObject temp = VisualTreeHelper.GetParent(parent);
                if (temp == null && parent is FrameworkElement)
                {
                    parent = (parent as FrameworkElement).Parent;
                }
                else
                {
                    parent = temp;
                }
            }

            return null;
        }

        internal static ScrollBar GetScrollBarControl(FrameworkElement element, int whichone)
        {
            DependencyObject parent = VisualTreeHelper.GetChild(element, 0);
            while (parent != null)
            {
                if (parent is ScrollBar)
                {
                    return parent as ScrollBar;
                }
                DependencyObject temp = VisualTreeHelper.GetChild(parent, whichone);
                if (temp == null && parent is FrameworkElement)
                {
                    parent = (parent as FrameworkElement).Parent;
                }
                else
                {
                    parent = temp;
                }
            }

            return null;
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            Size visualDesiredSize = new Size(0, 0);
            Size dataDesiredSize = new Size(0, 0);
            Size negativeSize = new Size(0, 0);
            Rect rect = new Rect();
            double _left;
            double _top;
            int max = -1;
            //foreach (UIElement ele in this.AllChildren)
            foreach (UIElement ele in from UIElement item in this.AllChildren
                                          orderby Panel.GetZIndex(item as UIElement)
                                          select item as UIElement)
            {
                if (ele is INodeGroup)
                {
                    if ((ele as INodeGroup).ReferenceNo < 0)
                    {
                        if (dc != null)
                        {
                            foreach (Node n in dc.Model.Nodes)
                            {
                                max++;
                            }
                            (ele as INodeGroup).ReferenceNo = max;
                        }
                        else
                        {
                            (ele as INodeGroup).ReferenceNo = ReferenceCount++;
                        }
                    }
                }
                if (this.InternalChildren.Contains(ele))
                {
                    ele.Measure(new Size(double.PositiveInfinity,Double.PositiveInfinity));
                }
                if (ele is Node)
                {
                    rect = new Rect((ele as Node).RectBounds.Left, (ele as Node).RectBounds.Top,(ele as Node).RectBounds.Right,(ele as Node).RectBounds.Bottom);
                }
                if (ele is DiagramViewGrid)
                {
                    rect = Rect.Empty;
                }
                else if (ele is LineConnector)
                {
                    rect = (ele as LineConnector).GetBounds();
                    if (rect != Rect.Empty)
                    {
                        if (rect.Right > 0)
                            rect.Width = rect.Right;
                        if (rect.Bottom > 0)
                            rect.Height = rect.Bottom;
                    }
                    (ele as LineConnector).InvalidateMeasure();
                }
                if (ele is Node)
                {
                    dataDesiredSize.Width = Math.Max(dataDesiredSize.Width, (ele as Node).Transform(new Point((ele as Node).ActualWidth, 0)).X);
                    dataDesiredSize.Height = Math.Max(dataDesiredSize.Height, (ele as Node).Transform(new Point(0,(ele as Node).ActualHeight)).Y);
                }
                if (ele is ICommon)
                {
                    visualDesiredSize.Width = Math.Max(rect.Width, visualDesiredSize.Width);
                    visualDesiredSize.Height = Math.Max(rect.Height, visualDesiredSize.Height);

                    negativeSize.Width = Math.Max(-rect.Left, negativeSize.Width);
                    negativeSize.Height = Math.Max(-rect.Top, negativeSize.Height);
                }
            }
            if (dview != null)
            {
                dview.InvalidateViewGrid();
            }
            _top = negativeSize.Height;
            _left = negativeSize.Width;            
            if (dview != null)
            {
                Top = Math.Max(_top, -dview.BoundaryConstraintsArea.Top);               
                Left = Math.Max(_left, -dview.BoundaryConstraintsArea.Left);
                if (dview.IsPanEnabled)
                {
                    Left = dview.panPoint.X;
                    Top = dview.panPoint.Y;
                    if ((dview.Scrollviewer.Content as OverviewContentHolder).IsZoomResetEnabled)
                    {
                        Left = Math.Max(_left, dview.panPoint.X);
                        Top = Math.Max(_top, dview.panPoint.Y);
                    }
                    //Point orig = OverviewContentHolder.GetOrigin((dview.Scrollviewer.Content as OverviewContentHolder).Content as FourQuadrantPanel);
                    //Left = -orig.X;
                    //Top = -orig.Y;
                }
                else
                {
                    Left = Math.Max(Left, dview.panPoint.X);
                    Top = Math.Max(Top, dview.panPoint.Y);
                }
               
            }
            if (dview!=null&&dview.SizeToContent)
            {
                if (visualDesiredSize.Equals(new Size(0, 0)))
                {
                    return dataDesiredSize;
                }
            }
            if (dview != null && !dview.SizeToContent)
            {
                if (visualDesiredSize.Width <= dview.BoundaryConstraintsArea.Right)
                {
                    if (visualDesiredSize.Height <= dview.BoundaryConstraintsArea.Bottom)
                    {
                        return new Size(dview.BoundaryConstraintsArea.Right, dview.BoundaryConstraintsArea.Bottom);
                    }
                    else
                    {
                        return new Size(dview.BoundaryConstraintsArea.Right, visualDesiredSize.Height);
                    }
                }
                else if (visualDesiredSize.Height <= dview.BoundaryConstraintsArea.Bottom)
                {
                    return new Size(visualDesiredSize.Width, dview.BoundaryConstraintsArea.Bottom);
                }
                else
                {
                    return new Size(visualDesiredSize.Width, visualDesiredSize.Height);
                }

            }
            else
            {
                return visualDesiredSize;
            }
        }
        static bool fullload = true;
        internal static void SetBridging(DiagramControl dc)
        {
            if (dc != null && dc.Model != null)
            {
                List<UIElement> ordered = (from UIElement item in dc.Model.Connections
                                           orderby Panel.GetZIndex(item as UIElement)
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
                            (element as LineConnector).UpdateConnectorPathGeometry();
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
        /// Positions child elements and determines a size for the control.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in this.InternalChildren)
            {
                double offsetX = 0;
                double offsetY = 0;

                if (element is Node)
                {
                    offsetX = (element as Node).PxOffsetX;
                    offsetY = (element as Node).PxOffsetY;
                }
                if (element is LineConnector)
                {
                    if (this.dc != null && this.dc.View != null && (!this.dc.View._LineRoute || this.dc.View.RoutingMode == RoutingMode.Immediate))
                    {
                        if ((element as LineConnector).LineRoutingEnabled)
                        {

                            AStarLineRouter asl = new AStarLineRouter((element as LineConnector).dc.View);
                            if ((element as LineConnector).isRouting == true)
                            {
                                (element as LineConnector).dc.View.LineRouter = asl;
                                //(element as LineConnector).SetLineBridging();
                            }
                        }
                    }
                }
                Size dSize;
                dSize = element.DesiredSize;
                element.Arrange(new Rect(offsetX, offsetY, dSize.Width, dSize.Height));
            }
            if (disp != null)
            {
                disp.Abort();
            }
            //object o = this.Dispatcher.Invoke(new Initialize(DiagramPage.SetBridging),System.Windows.Threading.DispatcherPriority.SystemIdle,
            //                             this.dc);       
            //disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
            //                        new Initialize(DiagramPage.SetBridging), this.dc);

            if (this.dc != null)
            {
                if (this.dc.View != null)
                {
                    if (!this.dc.View._LineRoute || this.dc.View.RoutingMode == RoutingMode.Immediate)
                    {
                        if (dc.View.LineRoutingEnabled)
                        {
                            AStarLineRouter asl = new AStarLineRouter(this.dc.View);
                            this.dc.View.LineRouter = asl;
                        }
                    }
                }
            }

            //DiagramPage.SetBridging(this.dc);
            DiagramControl.IsPageLoaded = false;
            return finalSize;
        }

        DispatcherOperation disp;
        internal delegate void Initialize(DiagramControl dc);
        /// <summary>
        /// Creates a clone of the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void Copyitem(object obj)
        {
            o = obj;
        }

        #endregion

        #region  Properties
        
        [Obsolete("This feature is no more supported")]
        internal Style CustomPathStyle
        {
            get { return (Style)GetValue(CustomPathStyleProperty); }
            set { SetValue(CustomPathStyleProperty, value); }
        }

        [Obsolete("This feature is no more supported")]
        public Style CustomTheme
        {
            get { return (Style)GetValue(CustomThemeProperty); }
            set { SetValue(CustomThemeProperty, value); }
        }

        [Obsolete("This feature is no more supported")]
        public Themes Theme
        {
            get { return (Themes)GetValue(ThemePathStyleProperty); }
            set { SetValue(ThemePathStyleProperty, value); }
        }

        #region drawing tools

        private bool m_IsPolyLineEnabled = false;

        [Obsolete("Please set EnableDrawingTools to true and set PolyLine as DrawingTools")]
        ///<summary>
        /// Use EnableDrawingTools as true and DrawingTools as PolyLine it is similar to IsPolyLineEnabled
        ///(diagramView.Page as DiagramPage).EnableDrawingTools = true;
        /// (diagramView.Page as DiagramPage).DrawingTool = DrawingTools.PolyLine;
        /// </summary>
        public bool IsPolyLineEnabled
        {
            get
            {
                return m_IsPolyLineEnabled;
            }
            set
            {
                m_IsPolyLineEnabled = value;
                EnablePolyLine();
            }
        }

        private void EnablePolyLine()
        {
            DiagramView dv = Node.GetDiagramView(this);
            if (dv != null)
            {
                if (IsPolyLineEnabled == true)
                {
                    dv.EnableDrawingTools = false;
                    dv.EnableConnection = false;
                }
                
            }
        }



        #endregion
        
        /// <summary>
        /// Gets or sets the style reference. Used for Serialization purpose.
        /// </summary>
        /// <value>The style ref.</value>
        public Style StyleRef
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the LineStyleRef reference. Used for Serialization purpose.
        /// </summary>
        /// <value>The line style ref.</value>
        public Style LineStyleRef
        {
            get;
            set;
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

        /// <summary>
        /// Gets or sets the orientation reference. Used for Serialization purpose.
        /// </summary>
        /// <value>The orientation ref.</value>
        public TreeOrientation OrientationRef
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Horizontal spacing reference.Used for Serialization purpose.
        /// </summary>
        /// <value>The horizontal spacing reference.</value>
        public double HorizontalSpacingref
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vertical spacing reference .Used for Serialization purpose.
        /// </summary>
        /// <value>The vertical spacing reference.</value>
        public double VerticalSpacingref
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the SpaceBetweenSubTreeSpacing reference .Used for Serialization purpose.
        /// </summary>
        /// <value>The sub tree spacing reference.</value>
        public double SubTreeSpacingref
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the SpaceBetweenSubTreeSpacing reference .Used for Serialization purpose.
        /// </summary>
        /// <value>The sub tree spacing reference.</value>
        public LayoutType LayoutTyperef
        {
            get;
            set;
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
        public double GridHorizontalOffset
        {
            get
            {
                return (double)GetValue(GridHorizontalOffsetProperty);
            }

            set
            {
                SetValue(GridHorizontalOffsetProperty, value);
            }
        }

        internal double PxGridHorizontalOffset
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(GridHorizontalOffset, (this as DiagramPage).MeasurementUnits);
            }

            set
            {
                GridHorizontalOffset = MeasureUnitsConverter.FromPixels(value, (this as DiagramPage).MeasurementUnits);
            }
        }

        internal double PxGridVerticalOffset
        {
            get
            {
                return MeasureUnitsConverter.ToPixels((this as DiagramPage).GridVerticalOffset, (this as DiagramPage).MeasurementUnits);
            }

            set
            {
                GridVerticalOffset = MeasureUnitsConverter.FromPixels(value, (this as DiagramPage).MeasurementUnits);
            }
        }


        public static readonly DependencyProperty GridHorizontalOffsetProperty =
            DependencyProperty.Register("GridHorizontalOffset", typeof(double), typeof(DiagramPage), new PropertyMetadata(25d, new PropertyChangedCallback(OnGridHorizontalOffsetChanged)));

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
        public double GridVerticalOffset
        {
            get
            {
                return (double)GetValue(GridVerticalOffsetProperty);
            }

            set
            {
                SetValue(GridVerticalOffsetProperty, value);
            }
        }

        public static readonly DependencyProperty GridVerticalOffsetProperty =
            DependencyProperty.Register("GridVerticalOffset", typeof(double), typeof(DiagramPage), new PropertyMetadata(25d, new PropertyChangedCallback(OnGridVerticalOffsetChanged)));

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
                if (m_selectionList == null)
                {
                    m_selectionList = new NodeCollection(this);
                }

                return m_selectionList;
            }
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

        // Viewstate property is used to Save the UnSaved properties
        public ViewState Viewstate
        {
            get { return (ViewState)GetValue(ViewstateProperty); }
            set { SetValue(ViewstateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Viewstate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewstateProperty =
            DependencyProperty.Register("Viewstate", typeof(ViewState), typeof(DiagramPage), new PropertyMetadata(new PropertyChangedCallback(OnViewstateChanged)));

        private static void OnViewstateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPage page = (DiagramPage)d;
            DiagramView dv = Node.GetDiagramView(page);
            if (dv == null)
            {
                page.Loaded += new RoutedEventHandler(page_Loaded);
            }
        }

        static void page_Loaded(object sender, RoutedEventArgs e)
        {
            DiagramView dv = Node.GetDiagramView(sender as DiagramPage);
            if (dv != null)
            {
                dv.CurrentZoom = (sender as DiagramPage).Viewstate.CurrentZoom;
                dv.ZoomFactor = (sender as DiagramPage).Viewstate.ZoomFactor;
                dv.BoundaryConstraintsArea = (sender as DiagramPage).Viewstate.BoundaryConstraintsArea;
                dv.BoundaryConstraintsEnabled = (sender as DiagramPage).Viewstate.BoundaryConstraintsEnabled;
                dv.SizeToContent = (sender as DiagramPage).Viewstate.SizeToContent;
                dv.PageBackground = (sender as DiagramPage).Viewstate.PageBackground;
                dv.OffPageBackground = (sender as DiagramPage).Viewstate.OffPageBackground;
                dv.PageMargin = (sender as DiagramPage).Viewstate.PageMargin;
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
       [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ConnectorType ConnectorType
        {
            get
            {
                return (ConnectorType)GetValue(ConnectorTypeProperty);
            }

            set
            {
                SetValue(ConnectorTypeProperty, value);
            }
        }      

        public static readonly DependencyProperty ConnectorTypeProperty = DependencyProperty.Register("ConnectorType", typeof(ConnectorType), typeof(DiagramPage), new PropertyMetadata(ConnectorType.Orthogonal));
        
        #endregion

        #region Internal Properties               

        /// <summary>
        /// Gets or sets the reference count. Used for serialization purposes
        /// </summary>
        /// <value>The reference count.</value>
        public int ReferenceCount
        {
            get;
            set;
        }
        
        #endregion

        #region DPs

        /// <summary>
        /// Identifies the CustomPathStyle dependency property.
        /// </summary>
        internal static readonly DependencyProperty CustomPathStyleProperty =
            DependencyProperty.Register("CustomPathStyle", typeof(Style), typeof(DiagramPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the CustomTheme dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomThemeProperty =
            DependencyProperty.Register("CustomTheme", typeof(Style), typeof(DiagramPage), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the Theme dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemePathStyleProperty =
            DependencyProperty.Register("Theme", typeof(Themes), typeof(DiagramPage), new UIPropertyMetadata(Themes.Default));

        /// <summary>
        /// Identifies the AllowSelect dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableResizingCurrentNodeOnMultipleSelectionProperty = DependencyProperty.Register("EnableResizingCurrentNodeOnMultipleSelection", typeof(bool), typeof(DiagramPage), new UIPropertyMetadata(false));

        /// <summary>
        /// Defines the LayoutType property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LayoutTypeProperty = DependencyProperty.Register("LayoutType", typeof(LayoutType), typeof(DiagramPage), new UIPropertyMetadata(LayoutType.None));

        /// <summary>
        /// Defines the MeasurementUnits property.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty MeasurementUnitsProperty = DependencyProperty.Register("MeasurementUnits", typeof(MeasureUnits), typeof(DiagramPage), new PropertyMetadata(MeasureUnits.Pixel, new PropertyChangedCallback(OnUnitsChanged)));

        #endregion

        #region Events

        /// <summary>
        /// Called when [units changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPage page = (DiagramPage)d;
            if (page.dc != null && page.dc.View != null)
            {
                Thickness px = MeasureUnitsConverter.ToPixels(page.dc.View.Bounds, (MeasureUnits)e.OldValue);
                page.dc.View.Bounds = MeasureUnitsConverter.FromPixels(px, (MeasureUnits)e.NewValue);
                page.GridHorizontalOffset = MeasureUnitsConverter.Convert(page.GridHorizontalOffset, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                page.GridVerticalOffset = MeasureUnitsConverter.Convert(page.GridVerticalOffset, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                page.dc.View.SnapOffsetX = MeasureUnitsConverter.Convert(page.dc.View.SnapOffsetX, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                page.dc.View.SnapOffsetY = MeasureUnitsConverter.Convert(page.dc.View.SnapOffsetY, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                page.dc.View.NudgeIncrement = MeasureUnitsConverter.Convert(page.dc.View.NudgeIncrement, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
            }
        }
        private static void OnGridHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPage page = (DiagramPage)d;
            if (page.dc != null && page.dc.View != null)
            {
                page.dc.View.mViewGrid.InvalidateVisual();
            }
        }
        private static void OnGridVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramPage page = (DiagramPage)d;
            if (page.dc != null && page.dc.View != null)
            {
                page.dc.View.mViewGrid.InvalidateVisual();
            }
        }
        #endregion

        #region Implementation
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Up:
                    dc.View.IsKeyDragged = true;
                    DiagramView.MoveUp(dc.View);
                    ////DiagramCommandManager.MoveUp.Execute((e.Source as DiagramControl).View.Page, (e.Source as DiagramControl).View);
                    break;
                case Key.Down:
                    dc.View.IsKeyDragged = true;
                    DiagramView.MoveDown(dc.View);
                    ////DiagramCommandManager.MoveDown.Execute((e.Source as DiagramControl).View.Page, (e.Source as DiagramControl).View);
                    break;
                case Key.Left:
                    dc.View.IsKeyDragged = true;
                    DiagramView.MoveLeft((dc.View));
                    ////DiagramCommandManager.MoveLeft.Execute((e.Source as DiagramControl).View.Page, (e.Source as DiagramControl).View);
                    break;
                case Key.Right:
                    dc.View.IsKeyDragged = true;
                    DiagramView.MoveRight(dc.View);
                    ////DiagramCommandManager.MoveRight.Execute((e.Source as DiagramControl).View.Page, (e.Source as DiagramControl).View);
                    break;
                case Key.Escape:
                    if (dview.IsNodeDragCancel == true)
                    {
                        foreach (ICommon n in dc.View.SelectionList.OfType<ICommon>())
                        {                            
                            if (n is Group)
                            {
                                //(n as Group).DragCancel = true;
                                (n as Group).OffsetX = (n as Group).x;
                                (n as Group).OffsetY = (n as Group).y;
                                foreach (INodeGroup n1 in (n as Group).NodeChildren)
                                {
                                    if (n1 is Node)
                                    {
                                        //(n1 as Node).DragCancel = true;
                                        (n1 as Node).OffsetY = (n1 as Node).y;
                                        (n1 as Node).OffsetX = (n1 as Node).x;
                                    }
                                    else
                                    {                                        
                                        (n1 as LineConnector).StartPointPosition = (n1 as LineConnector).sp;
                                        (n1 as LineConnector).EndPointPosition = (n1 as LineConnector).ep;
                                        (n1 as LineConnector).InvalidateConnectorPathGeometry();
                                    }
                                }
                                (n as Group).ReleaseStylusCapture();
                            }
                            if (n is Node)
                            {
                                //(n as Node).DragCancel = true;
                                (n as Node).OffsetY = (n as Node).y;
                                (n as Node).OffsetX = (n as Node).x;
                                (n as Node).ReleaseStylusCapture();
                            }
                            if ((n is LineConnector) && dc.View.SelectionList.Count > 1)
                            {                                
                                (n as LineConnector).StartPointPosition = (n as LineConnector).sp;
                                (n as LineConnector).EndPointPosition = (n as LineConnector).ep;
                                (n as LineConnector).InvalidateConnectorPathGeometry();
                            }                            
                        }
                    }
                    if (dview.IsConnectorDragCancel == true && dc.View.SelectionList.Count==1)
                    {
                        foreach (LineConnector line in this.dview.SelectionList.OfType<LineConnector>())
                        {
                            line.DragCancel = true;                            
                            line.ReleaseStylusCapture();
                        }
                    }
                    (this.dc.View.Page as DiagramPage).InvalidateMeasure();
                    break;
                default:
                    break;
            }
        }

        #endregion
        
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when the appropriate property changes.
        /// </summary>
        /// <param name="name">The property name.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Virtualization Members

        internal void AddingChildren(UIElement Child)
        {
            this.AddingInternalChild(Child);
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
        private void AddingInternalChild(UIElement child)
        {

            if (child is Node && !(child as Node).IsInternallyLoaded)
            {
                (child as Node).IsInternallyLoaded = true;
                base.AddInternalChild(child);
            }
            else if (child is LineConnector && !(child as LineConnector).IsInternallyLoaded)
            {
                (child as LineConnector).IsInternallyLoaded = true;
                base.AddInternalChild(child);
            }
        }
        protected void RemoveInternalChild(UIElement child)
        {


        }
        #endregion
    }
}
