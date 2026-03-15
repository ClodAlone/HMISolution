// <copyright file="Node.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Media.Imaging;
    using System.Collections.Specialized;
    using System.Windows.Data;
    using System.Xml;

    /// <summary>
    /// Represents the node class.
    /// </summary>
    /// <remarks>
    /// Nodes are graphical objects that can be drawn on the page by selecting them from the Symbol Palette and dropping them on the page, or they can be added through code behind.
    /// </remarks>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in C# and add nodes to it.
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
    ///       Model = new DiagramModel ();
    ///       View = new DiagramView ();
    ///       Control.View = View;
    ///       Control.Model = Model;
    ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
    ///       Node n = new Node(Guid.NewGuid(), "Start");
    ///        n.Shape = Shapes.FlowChart_Start;
    ///        n.IsLabelEditable = true;
    ///        n.Label = "Start";
    ///        n.Level = 1;
    ///        n.OffsetX = 150;
    ///        n.OffsetY = 25;
    ///        n.Width = 150;
    ///        n.Height = 75;
    ///        n.LabelVerticalAlignment = VerticalAlignment.Center;
    ///        n.LabelHorizontalAlignment = HorizontalAlignment.Center;
    ///        n.ToolTip="Start Node";
    ///        Model.Nodes.Add(n);
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    /// <seealso cref="DiagramModel"/>
    /// <seealso cref="LineConnector"/>
    public class Node : ContentControl, IShape, INodeGroup, ICommon
    {
        internal bool IsLoaded = false;
        internal bool IsFirstApplyTemplate = true;
        internal Point StartPointDragging;
        internal Size m_TempSize;
        internal bool m_MouseMoving = false;
        internal bool m_MouseResizing = false;
        internal Point m_TempPosition;
        internal bool m_NodeDrawing = false;
        internal static bool m_Rotatebycode = false;
        private  ContextMenuControl nodecontextmenu;
        private List<string> header = new List<string>();

        private LayoutTransformer nodelayouttransformer;

        private bool m_IsMouseOver = false;

        internal bool IsMouseOver
        {
            get { return m_IsMouseOver; }
            set
            {
                m_IsMouseOver = value;
                fireMouseOverEvent();
            }
        }

        // Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(Node), new PropertyMetadata(false));

        internal bool CustomCursorEnabled
        {
            get { return (bool)GetValue(CustomCursorEnabledProperty); }
            set { SetValue(CustomCursorEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomCursorEnabled.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CustomCursorEnabledProperty =
            DependencyProperty.Register("CustomCursorEnabled", typeof(bool), typeof(Node), new PropertyMetadata(true, new PropertyChangedCallback(OnCustomCursorEnableChanged)));

        private static void OnCustomCursorEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs evtArgs)
        {
            Node node = d as Node;
            if (node.CustomCursorEnabled)
            {
                node.setCursor();
            }
            else
            {
                CustomCursor.SetCustomCursor(node, null);
            }
        }

        internal double Left
        {
            get { return this.PxOffsetX; }// MeasureUnitsConverter.ToPixels(this.LogicalOffsetX, this.MeasurementUnits); }
        }

        internal double Top
        {
            get { return this.PxOffsetY; }// MeasureUnitsConverter.ToPixels(this.LogicalOffsetY, this.MeasurementUnits); }
        }

        internal double Right
        {
            get { return this.Left + this.ActualWidth; }
        }

        internal double Bottom
        {
            get { return this.Top + this.ActualHeight; }
        }

        #region Class variables
        internal bool subTreeReVal = true;

        internal double segmentoffset;

        internal bool Isnodedragged = false;

        internal double tempx;

        internal double tempy;

        internal ObservableCollection<LineConnector> In = new ObservableCollection<LineConnector>();

        internal ObservableCollection<LineConnector> Out = new ObservableCollection<LineConnector>();

        internal ObservableCollection<Node> RParents = new ObservableCollection<Node>();

        internal ObservableCollection<Node> RChildrens = new ObservableCollection<Node>();

        internal Node RParent;

        internal int Stage;

        internal bool Visited;

        internal double _tempOffsetX;
        internal double _tempOffsetY;
        private Thumb DragProvider = new Thumb();
        private Resizer Resizer = new Resizer();
        private Thumb Rotator = new Thumb();
        private Grid nodegrid;
        private TextBox nodetext;
        private TextBlock nodelabel;
        //private Cursor mcursor;
        //private Point dragdelta;
        private RotateTransform rotateTransform;
        private bool beforeconncreate = false;

        /// <summary>
        /// Used to store Diagram Control object
        /// </summary>
        internal DiagramControl dc;

        /// <summary>
        /// used to store the groups.
        /// </summary>
        private CollectionExt mgroups = new CollectionExt();

        /// <summary>
        /// Used to store the rank
        /// </summary>
        private int mrank = -1;

        /// <summary>
        /// Used to store the count of nodes.
        /// </summary>
        private int no = -1;

        /// <summary>
        /// Used to store the start point.
        /// </summary>
        private System.Windows.Point? startPoint = null;

        /// <summary>
        /// Used to store the node position.
        /// </summary>
        private Point mnodePosition;

        /// <summary>
        /// Used to store the full name of the node.
        /// </summary>
        private string mfullName;

        /// <summary>
        /// Used to store the in edges
        /// </summary>
        private CollectionExt minEdges = new CollectionExt();

        /// <summary>
        /// Used to store the out edges.
        /// </summary>
        private CollectionExt moutEdges = new CollectionExt();

        /// <summary>
        /// Used to store the edges
        /// </summary>
        private CollectionExt medges = new CollectionExt();

        /// <summary>
        /// Used to store the parents
        /// </summary>
        private CollectionExt mParents = new CollectionExt();
        
        /// <summary>
        /// Used to store the ports.
        /// </summary>
        private ObservableCollection<ConnectionPort> mports = new ObservableCollection<ConnectionPort>();

        /// <summary>
        /// Used to store the parent node
        /// </summary>
        private IShape mParentNode = null;

        /// <summary>
        /// Used to store the parent edge.
        /// </summary>
        private IEdge mParentEdge = null;

        /// <summary>
        /// Used to store the tree children
        /// </summary>
        private CollectionExt treeChildren = new CollectionExt();

        /// <summary>
        /// Used to store the depth.
        /// </summary>
        private int mDepth = -1;

        /// <summary>
        /// Used to store the model.
        /// </summary>
        private DiagramModel model;

        /// <summary>
        /// Used to store the bounding rectangle.
        /// </summary>
        private Rectangle mRectangle = new Rectangle();

        /// <summary>
        /// Used to store the IsFixed property value.
        /// </summary>
        private bool mIsFixed = false;

        /// <summary>
        /// Used to store the IsExpanded property value.
        /// </summary>
        private bool mIsExpanded = true;

        /// <summary>
        /// Used to store the page.
        /// </summary>
        private Panel mPage;

        /// <summary>
        /// Used to store the View.
        /// </summary>
        internal DiagramView dview;

        /// <summary>
        /// Used to store the mouse up state
        /// </summary>
        private static bool mouseup = true;

        /// <summary>
        /// Used to store the last node click instance
        /// </summary>
        private DateTime lastNodeClick;

        /// <summary>
        /// Used to store the last node click point
        /// </summary>
        private Point lastNodePoint;

        /// <summary>
        /// Used to store the resize node property setting.
        /// </summary>
        private bool resizenode = false;

        /// <summary>
        /// Used to store the port items
        /// </summary>
        internal ItemsControl portItems;
        
        /// <summary>
        /// Used to store the old ZIndex
        /// </summary>
        private int moldindex = 0;

        /// <summary>
        /// Used to store the new ZIndex
        /// </summary>
        private int mnewindex = 0;
        
        /// <summary>
        /// Used to store the old horizontal offset
        /// </summary>
        private double oldhoff = 0;

        /// <summary>
        /// Used to store the old vertical offset
        /// </summary>
        private double oldvoff = 0;

        /// <summary>
        /// Used to store the old offset position.
        /// </summary>
        private Point oldoff = new Point();

        /// <summary>
        /// Used to store node offset assignment check value.
        /// </summary>
        private bool misexe = false;

        /// <summary>
        /// Used to store the state of the node in case of cycle detection.
        /// </summary>
        private int state = 0;

        /// <summary>
        /// Used to check if this node is to be connected o its parent or not.
        /// </summary>
        private bool canconn = true;

        /// <summary>
        /// Used to check if mouse is double clicked.
        /// </summary>
        private bool isdoubleclicked = false;

        /// <summary>
        /// Used to store the old size of node.
        /// </summary>
        private Size osize = new Size();

        /// <summary>
        /// Used to check if HitTestVisibility is true or not.
        /// </summary>
        private bool hittest = false;

        /// <summary>
        /// Used to store the row count.
        /// </summary>
        private int row = 0;

        /// <summary>
        /// Used to store the column count.
        /// </summary>
        private int col = 0;

        /// <summary>
        /// Used to store the node width in pixels.
        /// </summary>
        private double pwidth = 0;

        /// <summary>
        /// Used to store the node height in pixels.
        /// </summary>
        private double pheight = 0;

        /// <summary>
        /// Used to store the old offsetx while undo/redo.
        /// </summary>
        private double oldx = 0;

        /// <summary>
        /// Used to store the old offsety while undo/redo.
        /// </summary>
        private double oldy = 0;

        /*/// <summary>
        /// Used to check if oldx and oldy are set once.
        /// </summary>
        //private bool exeonce = false;

        /// <summary>
        /// Used to check if the default value is assigned.
        /// </summary>
        //private bool isdefaulted = false;
        */

        /// <summary>
        /// Used to check if the node was resized.
        /// </summary>
        private bool resize = false;

        private bool Isrotate = false;

        private String Nodelbl = "";

        private Point currentPosition = new Point(0, 0);

        internal System.Windows.Shapes.Path NodeShape;

        internal static bool flagpopup = false;


        #endregion

        #region Initialization


        static Node()
        {
            //noders = new ResourceDictionary();
            //noders.Source = new Uri("/Syncfusion.Diagram.Silverlight;component/Themes/NodeShapes.xaml", UriKind.RelativeOrAbsolute);

            noders = new Dictionary<string, string>();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                Dictionary<string, string> nodeShapes = new Dictionary<string, string>();
                using (XmlReader reader = XmlReader.Create("/Syncfusion.Diagram.Silverlight;component/Themes/NodeShapes.xaml"))
                {
                    // Parse the file and display each of the nodes.
                    reader.Read();
                    //writer.WriteNode(reader, true);
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            string key = reader.GetAttribute(0);
                            XmlWriterSettings ws = new XmlWriterSettings();
                            ws.ConformanceLevel = ConformanceLevel.Auto;
                            using (XmlWriter writer = XmlWriter.Create(stream, ws))
                            {
                                writer.WriteNode(reader, true);
                            }
                            stream.Position = 0;
                            System.IO.StreamReader r = new System.IO.StreamReader(stream);
                            noders.Add(key, r.ReadToEnd());
                            stream.Position = 0;
                            stream.SetLength(0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
            : this(Guid.NewGuid())
        {
            //this.LayoutUpdated += new EventHandler(this.Node_LayoutUpdated);
            //this.Loaded += new RoutedEventHandler(this.Node_Loaded);
            //this.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Node_MouseLeftButtonUp), true);
            //this.RemoveHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Node_MouseLeftButtonUp));
            //this.SizeChanged += new SizeChangedEventHandler(this.Node_SizeChanged);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="id">The Guid id.</param>
        /// <param name="name">The node name.</param>
        public Node(Guid id, string name)
        {
            if (this.ReferenceID == null)
            {
                this.ID = id;
                m_ReferenceID = this.ID.ToString();
            }
            this.IsLoaded = false;
            this.DefaultStyleKey = typeof(Node);
            this.Name = name;
            if (this.Name == null || this.Name == string.Empty)
            {
                if (id != null)
                {
                    this.Name = "Node" + id.ToString("N");
                }
                else
                {
                    this.Name = "Node" + Guid.NewGuid().ToString("N");
                }
            }
            //this.LayoutUpdated += new EventHandler(this.Node_LayoutUpdated);
            this.Loaded += new RoutedEventHandler(this.Node_Loaded);
            this.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Node_MouseLeftButtonUp), true);
            this.AddHandler(Control.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Node_MouseLeftButtonDown), true);
            this.SizeChanged += new SizeChangedEventHandler(this.Node_SizeChanged);
            
            Binding widthBinding = new Binding("Width");
            widthBinding.Source = this;
            this.SetBinding(Node.DuplicateWidthProperty, widthBinding);
            Binding heightBinding = new Binding("Height");
            heightBinding.Source = this;
            this.SetBinding(Node.DuplicateHeightProperty, heightBinding);
            //this.rs = new ResourceDictionary();
            //this.rs.Source = new Uri("/Syncfusion.Diagram.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            //this.Unloaded += new RoutedEventHandler(Node2_Unloaded);
            Loaded += new RoutedEventHandler(Node2_Loaded);
            Unloaded += new RoutedEventHandler(Node2_Unloaded);
        }

        void Node2_Unloaded(object sender, RoutedEventArgs e)
        {
            //CustomCursor.SetCustomCursor(this, null);
        }

        void Node2_Loaded(object sender, RoutedEventArgs e)
        {
            //setCursor();
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="id">The Guid id.</param>
        public Node(Guid id)
            : this(id, null)
        {
            //this.mid = id;
            //this.LayoutUpdated += new EventHandler(this.Node_LayoutUpdated);
            //this.Loaded += new RoutedEventHandler(this.Node_Loaded);
            //this.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Node_MouseLeftButtonUp), true);
            //this.RemoveHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Node_MouseLeftButtonUp));
            //this.SizeChanged += new SizeChangedEventHandler(this.Node_SizeChanged);
        }


        /// <summary>
        /// Handles the SizeChanged event of the Node control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.dc != null)
            {
                //if (dc.View.IsResized)
                //{
                //    if (!this.dc.View.Undone && !this.dc.View.Redone)
                //    {
                //        this.dc.View.tUndoStack.Push(new NodeOperation(NodeOperations.Resized, this));
                //    }
                //}
                if (!this.dc.View.IsResized && !this.dc.View.IsResizedUndone && !this.Resized)
                {
                    foreach (ConnectionPort port in this.Ports)
                    {
                        if (!this.dc.View.IsResizedRedone)
                        {
                            port.PreviousPortPoint = new Point(port.PxLeft, port.PxTop);
                        }
                        this.dview.UndoStack.Push(port);
                        this.dview.UndoStack.Push(port.PreviousPortPoint);
                    }

                    this.dview.UndoStack.Push(this);
                    this.dc.View.UndoStack.Push(e.PreviousSize);
                    this.dview.UndoStack.Push(new Point(this.oldx, this.oldy));
                    this.dview.UndoStack.Push(this.dview.NodeResizedCount);
                    this.dview.UndoStack.Push("Resized");
                    //this.Resized = true;
                    this.dc.View.IsResizedRedone = false;
                }
            }
        }




        /// <summary>
        /// Calls Node_Loaded method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void Node_Loaded(object sender, RoutedEventArgs e)
        {
            var temp = this.Ports.OfType<ConnectionPort>();
            List<ConnectionPort> portlist = new List<ConnectionPort>();
            var intersect = portlist.Intersect(this.Ports.OfType<ConnectionPort>());
            portlist.AddRange(temp.Except(intersect));
            foreach (ConnectionPort cport in portlist)
            {
                if (cport.CenterPortReferenceNo == 0)
                {
                    i++;
                }
                if (i > 1)
                {
                    this.Ports.Remove(cport);
                    //break;
                }
            }
            i = 0;
            this.IsLoaded = true;
            DiagramControl diagramCtrl = DiagramPage.GetDiagramControl(this);
            if (diagramCtrl == null)
            {
                diagramCtrl = dc;
            }
            if (this.IsSelected)
            {
                if (this.IsGrouped)
                {
                    this.IsSelected = false;
                }
                if (!diagramCtrl.View.SelectionList.Contains(this))
                {
                    diagramCtrl.View.SelectionList.Add(this);
                    NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
                    diagramCtrl.View.OnNodeSelected(this, newEventArgs);
                    diagramCtrl.View.oldselectionlist.Add(this);
                }
                //else if (this.IsSelected)
                //{
                //    NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
                //    diagramCtrl.View.OnNodeSelected(this, newEventArgs);
                //    diagramCtrl.View.oldselectionlist.Add(this);
                //}
            }
            //try
            //{
            //    this.MouseLeftButtonUp += new MouseButtonEventHandler(this.Node_MouseLeftButtonUp);
            //    this.dview = GetDiagramView(this);
            //    this.dc = DiagramPage.GetDiagramControl(this);
            //    if (!this.exeonce)
            //    {
            //        this.oldx = MeasureUnitsConverter.ToPixels((double)this.OffsetX, (this.dview.Page as DiagramPage).MeasurementUnits) + (this.dview.Page as DiagramPage).LeastX;
            //        this.oldy = MeasureUnitsConverter.ToPixels((double)this.OffsetY, (this.dview.Page as DiagramPage).MeasurementUnits) + (this.dview.Page as DiagramPage).LeastY;
            //        this.exeonce = true;
            //    }

            //    if (!this.dview.IsPageEditable)
            //    {
            //        DiagramView.PageEdit = false;
            //        this.IsLabelEditable = false;
            //    }

            foreach (ConnectionPort cport in this.Ports)
            {
                //if (cport.PortReferenceNo < 1)
                //{
                //    cport.PortReferenceNo = (cport.Node.ReferenceNo * 10) + this.pno;
                //    this.pno++;
                //}

                if (this.portItems != null)
                {
                    if (!this.portItems.Items.Contains(cport))
                    {
                        this.portItems.Items.Add(cport);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the Node control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Node_Unloaded(object sender, RoutedEventArgs e)
        {
            dc.View.UndoStack.Clear();
            if (this.dview != null)
            {
                if (this.dview.Scrollviewer != null && !(this.dview.Page as DiagramPage).IsDiagrampageLoaded)
                {
                    if (this.dview.IsMouseScrolled)
                    {
                        if ((this.dview.Page as DiagramPage).Minleft >= 0)
                        {
                            this.dview.Scrollviewer.ScrollToHorizontalOffset(Math.Abs((this.dview.Page as DiagramPage).HorValue));
                        }
                        else
                        {
                            this.dview.Scrollviewer.ScrollToHorizontalOffset(Math.Abs((this.dview.Page as DiagramPage).HorValue * this.dview.CurrentZoom));
                        }

                        if ((this.dview.Page as DiagramPage).MinTop >= 0)
                        {
                            this.dview.Scrollviewer.ScrollToVerticalOffset(Math.Abs((this.dview.Page as DiagramPage).VerValue));
                        }
                        else
                        {
                            this.dview.Scrollviewer.ScrollToVerticalOffset(Math.Abs((this.dview.Page as DiagramPage).VerValue * this.dview.CurrentZoom));
                        }
                    }
                    else
                    {
                        this.dview.Scrollviewer.ScrollToHorizontalOffset(Math.Abs((this.dview.Page as DiagramPage).HorValue * this.dview.CurrentZoom));
                        this.dview.Scrollviewer.ScrollToVerticalOffset(Math.Abs((this.dview.Page as DiagramPage).VerValue * this.dview.CurrentZoom));
                    }
                }
            }
        }

        #endregion

        #region Properties

        public bool IsLabelDragable
        {
            get { return (bool)GetValue(IsLabelDragableProperty); }
            set { SetValue(IsLabelDragableProperty, value); }
        }

        public Point LabelDisplacement
        {
            get { return (Point)GetValue(LabelDisplacementProperty); }
            set { SetValue(LabelDisplacementProperty, value); }
        }

        internal bool DragCancel
        {
            get;
            set;
        }
        public Style LeftResizer
        {
            get { return (Style)GetValue(LeftResizerProperty); }
            set { SetValue(LeftResizerProperty, value); }
        }

        public Style RightResizer
        {
            get { return (Style)GetValue(RightResizerProperty); }
            set { SetValue(RightResizerProperty, value); }
        }

        public Style BottomResizer
        {
            get { return (Style)GetValue(BottomResizerProperty); }
            set { SetValue(BottomResizerProperty, value); }
        }

        public Style TopResizer
        {
            get { return (Style)GetValue(TopResizerProperty); }
            set { SetValue(TopResizerProperty, value); }
        }

        public Style TopLeftCornerResizer
        {
            get { return (Style)GetValue(TopLeftCornerResizerProperty); }
            set { SetValue(TopLeftCornerResizerProperty, value); }
        }

        public Style TopRightCornerResizer
        {
            get { return (Style)GetValue(TopRightCornerResizerProperty); }
            set { SetValue(TopRightCornerResizerProperty, value); }
        }

        public Style BottomLeftCornerResizer
        {
            get { return (Style)GetValue(BottomLeftCornerResizerProperty); }
            set { SetValue(BottomLeftCornerResizerProperty, value); }
        }

        public Style BottomRightCornerResizer
        {
            get { return (Style)GetValue(BottomRightCornerResizerProperty); }
            set { SetValue(BottomRightCornerResizerProperty, value); }
        }

        internal Grid Nodepanel
        {
            get
            {
                return this.nodegrid;
            }

            set
            {
                this.nodegrid = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotate angle.
        /// </summary>
        /// <value>The rotate angle.</value>
        public double RotateAngle
        {
            get
            {
                return (double)GetValue(RotateAngleProperty);
            }

            set
            {
                SetValue(RotateAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether content is hit test visible. Used for serialization purposes internally.
        /// </summary>
        /// <value>
        /// <c>true</c> if [content hit test visible]; otherwise, <c>false</c>.
        /// </value>
        public bool ContentHitTestVisible
        {
            get { return this.hittest; }
            set { this.hittest = value; }
        }

        internal int i = 0;

        /// <summary>
        /// Gets or sets the reference count. Used for serialization purposes
        /// </summary>
        /// <value>The reference count.</value>
        public int PortReferenceCount
        {
            get { return this.i; }
            set { this.i = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Node"/> is resized.
        /// </summary>
        /// <value><c>true</c> if resized; otherwise, <c>false</c>.</value>
        internal bool Resized
        {
            get { return this.resize; }
            set { this.resize = value; }
        }

        /// <summary>
        /// Gets or sets the old size.
        /// </summary>
        /// <value>The old size.</value>
        internal Size Oldsize
        {
            get { return this.osize; }
            set { this.osize = value; }
        }

        /// <summary>
        /// Gets the groups to which the INodeGroup objects belong.
        /// </summary>
        /// <value>The groups.</value>
        public CollectionExt Groups
        {
            get
            {
                return this.mgroups;
            }
        }

        /// <summary>
        /// Gets or sets the gripper visibility.
        /// </summary>
        /// <value>
        /// Type: <see cref="Visibility"/>
        /// Default value is Collapsed.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.GripperVisibility=Visibility.Visible;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public Visibility GripperVisibility
        {
            get
            {
                return (Visibility)GetValue(GripperVisibilityProperty);
            }

            set
            {
                SetValue(GripperVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the gripper style.  
        /// </summary>
        /// <remarks>
        /// When the gripper style is set, it is necessary to specify the Width, Height, HorizontalAlignment, VerticalAlignment and the Margin properties because GripperStyle property overrides the default settings.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to write a style for the Gripper in Window.Resources.
        /// <code language="XAML">
        ///  &lt;Style x:Key="GripperStyle"  TargetType="{x:Type syncfusion:Gripper}"&gt;
        ///        &lt;Setter Property="Width" Value="30"/&gt;
        ///        &lt;Setter Property="Height" Value="30"/&gt;
        ///        &lt;Setter Property="HorizontalAlignment" Value="Left"/&gt;
        ///        &lt;Setter Property="VerticalAlignment" Value="Top"/&gt; 
        ///        &lt;Setter Property="Margin" Value="10,-15,0,0"/&gt;
        ///        &lt;Setter Property="Template"&gt;
        ///            &lt;Setter.Value&gt;
        ///                &lt;ControlTemplate TargetType="{x:Type syncfusion:Gripper}"&gt;
        ///                    &lt;Border Background="Blue" CornerRadius="10"   /&gt;
        ///                &lt;/ControlTemplate&gt;
        ///           &lt;/Setter.Value&gt;
        ///        &lt;/Setter&gt;
        ///    &lt;/Style&gt;
        /// </code>
        /// <para/>The following code shows how to assign the style created to the Gripper of the <see cref="Node"/>.
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.GripperStyle=this.Resources["GripperStyle"] as Style.
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public Style GripperStyle
        {
            get
            {
                return (Style)GetValue(GripperStyleProperty);
            }

            set
            {
                SetValue(GripperStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> rotation is allowed.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if rotation is enabled, false otherwise.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.AllowRotate=true;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public bool AllowRotate
        {
            get
            {
                return (bool)GetValue(AllowRotateProperty);
            }

            set
            {
                SetValue(AllowRotateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> resize is allowed.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if resizing is enabled, false otherwise.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.AllowResize=true;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public bool AllowResize
        {
            get
            {
                return (bool)GetValue(AllowResizeProperty);
            }

            set
            {
                SetValue(AllowResizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> can be moved.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if drag is enabled, false otherwise.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.AllowMove=true;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public bool AllowMove
        {
            get
            {
                return (bool)GetValue(AllowMoveProperty);
            }

            set
            {
                SetValue(AllowMoveProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> can be selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if selection is enabled, false otherwise.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.AllowSelect=true;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public bool AllowSelect
        {
            get
            {
                return (bool)GetValue(AllowSelectProperty);
            }

            set
            {
                SetValue(AllowSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to resize this node.
        /// </summary>
        /// <value><c>true</c> if [resize this node]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Used for serialization purpose.
        /// </remarks>
        public bool ResizeThisNode
        {
            get { return this.resizenode; }
            set { this.resizenode = value; }
        }

        /// <summary>
        /// Gets or sets the reference no.
        /// </summary>
        /// <value>The reference no.</value>
        /// <remarks>
        /// Used for serialization purpose.
        /// </remarks>
        public int ReferenceNo
        {
            get { return this.no; }
            set { this.no = value; }
        }
        private string m_ReferenceID;

        /// <summary>
        /// This is used for Internal Purpose to save the GUID value
        /// </summary>
        public string ReferenceID
        {
            get { return this.m_ReferenceID; }
            set { this.m_ReferenceID = value; }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// Type: <see cref="Panel"/>
        /// Panel instance.
        /// </value>
        public Panel Page
        {
            get { return this.mPage; }
            set { this.mPage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is label editable.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// True, if it can be edited, false otherwise.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// node.IsLabelEditable=true;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// Default Value is false.When this is false, HitTest is also set to false.
        /// When set to true, clicking on the label, will make the editable textbox visible.
        /// Enter the new label and press ENTER to apply the changed label,
        /// or press ESC to ignore the new label and revert back to the old one.
        /// </remarks>
        public bool IsLabelEditable
        {
            get { return (bool)GetValue(IsLabelEditableProperty); }
            set { SetValue(IsLabelEditableProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// String value.
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
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        node.Label="SyncNode";
        ///        Model.Nodes.Add(n);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <remarks>
        /// Default value is an empty string.
        /// </remarks>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label visibility.
        /// </summary>
        /// <value>
        /// Type: <see cref="Visibility"/>
        /// Enum specifying the visibility.
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
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        node.LabelVisibility=Visibility.Visible;
        ///        Model.Nodes.Add(n);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <remarks>
        /// Default value is visible.
        /// </remarks>
        public Visibility LabelVisibility
        {
            get
            {
                return (Visibility)GetValue(LabelVisibilityProperty);
            }

            set
            {
                SetValue(LabelVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the HorizontalAlignment of the Label. This will take effect only if the LabelWidth is set.
        /// </summary>
        /// <value>
        /// Type: <see cref="HorizontalAlignment"/>
        /// Enum specifying the alignment position.</value>
        /// <remarks>Default HorizontalAlignment is at the Center.</remarks>
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
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        node.LabelHorizontalAlignment=HorizontalAlignment.Left;
        ///        Model.Nodes.Add(n);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(LabelHorizontalAlignmentProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the node context menu.
        /// </summary>
        /// <value>The node context menu.</value>
        public ContextMenuControl ContextMenu
        {
            get
            {
                return (ContextMenuControl)GetValue(ContextMenuProperty);
                //return ContextMenuControlService.GetContextMenuControl(this);
            }
            set
            {
                SetValue(ContextMenuProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the VerticalAlignment of the Label.
        /// </summary>
        /// <value>
        /// Type: <see cref="VerticalAlignment"/>
        /// Enum specifying the alignment position.</value>
        /// <remarks>Default VerticalAlignment is at the Top.</remarks>
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
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        node.LabelVerticalAlignment = VerticalAlignment.Left;
        ///        Model.Nodes.Add(n);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public VerticalAlignment LabelVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty);
            }

            set
            {
                SetValue(LabelVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is treat as obstacle or not.
        /// Default value is true.
        /// </summary>
        public bool TreatAsObstacle
        {
            get { return (bool)GetValue(TreatAsObstacleProperty); }
            set { SetValue(TreatAsObstacleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label angle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Angle value in pixels.
        /// </value>
        /// <remarks>Default Angle is 0d.</remarks>
        public double LabelAngle
        {
            get { return (double)GetValue(LabelAngleProperty); }
            set { SetValue(LabelAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connection drag is over.
        /// </summary>
        /// <value>
        /// <c>true</c> if connection drag is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set
            {
                fireMouseOverEvent();
                SetValue(IsDragConnectionOverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the shape.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Enum specifying the Shapes .
        /// </value>
        /// <remarks>
        /// Several built-in shapes are provided. The user can select from any of the built-in shapes or specify their own custom shape using the <see cref="CustomPathStyle"/> property.
        /// </remarks>
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
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="Start Node";
        ///        node.LabelVerticalAlignment = VerticalAlignment.Left;
        ///        Model.Nodes.Add(n);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Shapes Shape
        {
            get { return (Shapes)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }
        private CustomPathStyle customPathStyle;

        public CustomPathStyle PathStyle
        {
            get
            {
                if (this.customPathStyle != null)
                {
                    return this.customPathStyle;
                }
                else
                {
                    this.customPathStyle = new CustomPathStyle();
                }

                return this.customPathStyle;
            }

            set
            {
                this.customPathStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the PathStyle of the Node.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        /// <remarks>
        /// While setting the custom path, the shape of the node can be set to Custom.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set PathStyle of a node .
        /// Specify a resource in XAML .
        /// <code language="XAML">
        /// &lt;Style TargetType="{x:Type Path}" x:Key="myNode"&gt;
        ///     &lt;Setter Property="Data" Value="M200,239L200,200 240,239 280,202 320,238 281,279 240,244 198,279z"&gt;&lt;/Setter&gt;
        ///     &lt;Setter Property="Fill" Value="MidnightBlue" /&gt;
        /// &lt;/Style&gt;
        ///  </code>
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
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///       Node n = new Node(Guid.NewGuid(), "Start");
        ///       Style customstyle = (Style)this.Resources["myNode"];
        ///        n.CustomPathStyle=customstyle;
        ///        n.Shape = Shapes.Custom;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Model.Nodes.Add(n);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Style CustomPathStyle
        {
            get { return (Style)GetValue(CustomPathStyleProperty); }
            set { SetValue(CustomPathStyleProperty, value); }
        }

        public Brush NodePathFill
        {
            get { return (Brush)GetValue(NodePathFillProperty); }
            set { SetValue(NodePathFillProperty, value); }
        }

        public Brush NodePathStroke
        {
            get { return (Brush)GetValue(NodePathStrokeProperty); }
            set { SetValue(NodePathStrokeProperty, value); }
        }

        public double NodePathStrokeThickness
        {
            get { return (double)GetValue(NodePathStrokeThicknessProperty); }
            set { SetValue(NodePathStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        /// <remarks>
        /// Based on the level property , the nodes belonging to the same level can be customized to have the same look and feel.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Level = 2;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// node.LabelVerticalAlignment = VerticalAlignment.Left;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        /// <summary>
        /// Gets the ActualWidth of the node in pixels.
        /// </summary>
        public new double ActualWidth
        {
            get
            {
                return base.ActualWidth;
            }
        }
        /// <summary>
        /// Gets the ActualHeight of the node in pixels.
        /// </summary>
        public new double ActualHeight
        {
            get
            {
                return base.ActualHeight;
            }
        }

        internal Point PxPosition
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(Position, MeasurementUnits);
            }
            set
            {
                Position = MeasureUnitsConverter.FromPixels(value, MeasurementUnits);
            }
        }

        /// <summary>
        /// Gets or sets the Center Position  of the Node.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// Center Point.</value>
        public Point Position
        {
            get
            {
                if (dview != null && (dview.Page as DiagramPage).MeasurementUnits == MeasureUnits.Pixel)
                {
                    return new Point(this.PxOffsetX + (this.Width / 2), this.PxOffsetY + (this.Height / 2));
                }
                else
                {
                    return this.mnodePosition;
                }
            }

            set
            {
                if (this.mnodePosition != value)
                {
                    this.mnodePosition = value;
                    this.OnPropertyChanged("Position");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is center port enabled.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// True, if it is enabled, false otherwise.
        /// </value>
        /// <remarks>Default value is true.</remarks>
        [Obsolete("This property has no use, since PortVisibility covers all the behaviors")]
        public bool IsPortEnabled
        {
            get
            {
                return (bool)GetValue(IsPortEnabledProperty);
            }

            set
            {
                SetValue(IsPortEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets the information about the node.
        /// </summary>
        /// <returns>Node info value</returns>
        /// <value>
        /// Type: <see cref="NodeInfo"/>
        /// </value>
        internal NodeInfo GetInfo()
        {
            NodeInfo info = new NodeInfo();
            info.Left = this.PxOffsetX;
            info.Top = this.PxOffsetY;
            //double aw = this._Width;
            //double ah = this._Height; 
            double aw, ah;
            if (DiagramControl.IsPageLoaded)
            {
                double paw = this.Width;// MeasureUnitsConverter.ToPixels(this.Width, this.MeasurementUnits);
                double pah = this.Height;// MeasureUnitsConverter.ToPixels(this.Height, this.MeasurementUnits);
                aw = paw;// MeasureUnitsConverter.FromPixels(paw, this.MeasurementUnits);
                ah = pah;// MeasureUnitsConverter.FromPixels(pah, this.MeasurementUnits);
            }
            else
            {
                aw = this.ActualWidth;// MeasureUnitsConverter.FromPixels(this.ActualWidth, this.MeasurementUnits);
                ah = this.ActualHeight;// MeasureUnitsConverter.FromPixels(this.ActualHeight, this.MeasurementUnits);
            }

            info.Size = new Size(aw, ah);
            Point pos = this.PxPosition;// MeasureUnitsConverter.FromPixels(this.Position, this.MeasurementUnits);
            info.Position = this.PxPosition;
            info.MeasurementUnit = this.MeasurementUnits;
            return info;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Node"/> is double clicked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this <see cref="Node"/> is double clicked; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDoubleClicked
        {
            get
            {
                return (bool)GetValue(IsDoubleClickedProperty);
            }

            set
            {
                SetValue(IsDoubleClickedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ports.
        /// </summary>
        /// <value>The ports.</value>
        /// <example>
        /// C#:
        /// <para/>
        /// The following example shows how to create a <see cref="ConnectionPort"/> in C#.
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// //Creates a node
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.Level = 1;
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// Model.Nodes.Add(n);
        /// //Define a Custom port for the node.
        /// ConnectionPort port = new ConnectionPort();
        /// port.Node=n;
        /// port.Left=75;
        /// port.Top=10;
        /// port.PortShape = PortShapes.Arrow;
        /// port.PortStyle.Fill = Brushes.Transparent;
        /// port.Height = 11;
        /// port.Width = 11;
        /// n.Ports.Add(port);
        /// Node n1 = new Node(Guid.NewGuid(), "Decision1");
        /// n1.Shape = Shapes.FlowChart_Process;
        /// n1.IsLabelEditable = true;
        /// n1.Label = "Alarm Rings";
        /// n1.Level = 2;
        /// n1.OffsetX = 150;
        /// n1.OffsetY = 125;
        /// n1.Width = 150;
        /// n1.Height = 75;
        /// Model.Nodes.Add(n1);
        /// ConnectionPort port1 = new ConnectionPort();
        /// port1.Node=n;
        /// port1.Left=75;
        /// port1.Top=50;
        /// port1.PortShape = PortShapes.Arrow;
        /// port1.PortStyle.Fill = Brushes.Transparent;
        /// port1.Height = 11;
        /// port1.Width = 11;
        /// n1.Ports.Add(port1);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ConnectionPort"/>
        public ObservableCollection<ConnectionPort> Ports
        {
            get { return this.mports; }
            set { this.mports = value; }
        }

        /// <summary>
        /// Gets or sets the port visibility.
        /// </summary>
        /// <value>The port visibility.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.PortVisibility = Visibility.Visible;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
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
        /// Gets or sets a value indicating whether port can be moved or not.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if drag is enabled, false otherwise.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.AllowPortDrag = true;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public bool AllowPortDrag
        {
            get
            {
                return (bool)GetValue(AllowPortDragProperty);
            }

            set
            {
                SetValue(AllowPortDragProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the label.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// By default the label width equals the node width.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelWidth = 50;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public double LabelWidth
        {
            get
            {
                return (double)GetValue(LabelWidthProperty);
            }

            set
            {
                SetValue(LabelWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the label.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// By default the label height equals 20.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelHeight = 50;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public double LabelHeight
        {
            get
            {
                return (double)GetValue(LabelHeightProperty);
            }

            set
            {
                SetValue(LabelHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label text wrapping.
        /// </summary>
        /// <value>
        /// Type: <see cref="TextWrapping"/>
        /// By default it is set to NoWrap.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelTextWrapping = TextWrapping.Wrap;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public TextWrapping LabelTextWrapping
        {
            get
            {
                return (TextWrapping)GetValue(LabelTextWrappingProperty);
            }

            set
            {
                SetValue(LabelTextWrappingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the label font.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// By default it is set to 11d.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelFontSize = 14;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public double LabelFontSize
        {
            get
            {
                return (double)GetValue(LabelFontSizeProperty);
            }

            set
            {
                SetValue(LabelFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font family.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontFamily"/>
        /// By default it is set to Arial.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelFontFamily = new FontFamily("Verdana");
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public FontFamily LabelFontFamily
        {
            get
            {
                return (FontFamily)GetValue(LabelFontFamilyProperty);
            }

            set
            {
                SetValue(LabelFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font weight.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontWeight"/>
        /// By default it is set to SemiBold.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelFontWeight = FontWeights.Bold;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public FontWeight LabelFontWeight
        {
            get
            {
                return (FontWeight)GetValue(LabelFontWeightProperty);
            }

            set
            {
                SetValue(LabelFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label font style.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontStyle"/>
        /// By default it is set to Normal.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelFontStyle = FontStyles.Italic;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public FontStyle LabelFontStyle
        {
            get
            {
                return (FontStyle)GetValue(LabelFontStyleProperty);
            }

            set
            {
                SetValue(LabelFontStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label background.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// By default it is set to White.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelBackground = Brushes.Red;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public Brush LabelBackground
        {
            get
            {
                return (Brush)GetValue(LabelBackgroundProperty);
            }

            set
            {
                SetValue(LabelBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label foreground.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// By default it is set to Black.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.LabelForeground = Brushes.Blue;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public Brush LabelForeground
        {
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }

            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label text alignment.
        /// </summary>
        /// <value>
        /// Type: <see cref="TextAlignment"/>
        /// By default it is set to Center.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// n.TextAlignment = TextAlignment.Left;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        internal TextAlignment LabelTextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(LabelTextAlignmentProperty);
            }

            set
            {
                SetValue(LabelTextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Node"/> offset properties are set.
        /// </summary>
        /// <value><c>true</c> if isnodeexe; otherwise, <c>false</c>.</value>
        internal bool Isnodeexe
        {
            get { return this.misexe; }
            set { this.misexe = value; }
        }

        /// <summary>
        /// Gets or sets the state of the node in case of cycle detection.
        /// </summary>
        /// <value>The state of the node. (0-->non-visited; 1-->Visited and InProgress; 2-->Done)</value>
        internal int State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node can be connected to the other specified node in case of hierarchical-tree layout.
        /// </summary>
        /// <value>
        /// <c>true</c> if can connect; otherwise, <c>false</c>.
        /// </value>
        internal bool CanConnect
        {
            get { return this.canconn; }
            set { this.canconn = value; }
        }

        /// <summary>
        /// Gets or sets the width of the node in pixels.
        /// </summary>
        /// <value>The width of the node.</value>
        internal double PixelWidth
        {
            get { return this.pwidth; }
            set { this.pwidth = value; }
        }

        /// <summary>
        /// Gets or sets the height of the node in pixels.
        /// </summary>
        /// <value>The height of the node.</value>
        internal double PixelHeight
        {
            get { return this.pheight; }
            set { this.pheight = value; }
        }

        /// <summary>
        /// Gets or sets the Measurement unit property.
        /// <value>
        /// Type: <see cref="MeasureUnits"/>
        /// Enum specifying the unit to be used.
        /// </value>
        /// </summary>
        internal MeasureUnits MeasurementUnits
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
        /// Identifies the DeletingMode dependency property.
        /// </summary>
        public DeletingMode DeletingMode
        {
            get { return (DeletingMode)GetValue(DeletingModeProperty); }
            set { SetValue(DeletingModeProperty, value); }
        }

        #endregion

        #region Dependency Properties


        /// <summary>
        /// Gets or sets a value of type TextDecorations (static class)
        /// </summary>
        public TextDecorationCollection LabelTextDecoration
        {
            get { return (TextDecorationCollection)GetValue(LabelTextDecorationProperty); }
            set { SetValue(LabelTextDecorationProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelTextDecoration dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextDecorationProperty =
            DependencyProperty.Register("LabelTextDecoration", typeof(TextDecorationCollection), typeof(Node),new PropertyMetadata(null));

        
                    

        public static readonly DependencyProperty IsLabelDragableProperty = DependencyProperty.Register("IsLabelDragable", typeof(bool), typeof(Node), new PropertyMetadata(false));

        public static readonly DependencyProperty LabelDisplacementProperty = DependencyProperty.Register("LabelDisplacement", typeof(Point), typeof(Node), new PropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnLabelDisplacementChanged)));
        public static readonly DependencyProperty LeftResizerProperty = DependencyProperty.Register("LeftResizer", typeof(Style), typeof(Node), new PropertyMetadata(null));

        public static readonly DependencyProperty RightResizerProperty = DependencyProperty.Register("RightResizer", typeof(Style), typeof(Node), new PropertyMetadata(null));

        public static readonly DependencyProperty BottomResizerProperty = DependencyProperty.Register("BottomResizer", typeof(Style), typeof(Node), new PropertyMetadata(null));


        public static readonly DependencyProperty TopResizerProperty = DependencyProperty.Register("TopResizer", typeof(Style), typeof(Node), new PropertyMetadata(null));
        public static readonly DependencyProperty TopLeftCornerResizerProperty = DependencyProperty.Register("TopLeftCornerResizer", typeof(Style), typeof(Node), new PropertyMetadata(null));
        public static readonly DependencyProperty TopRightCornerResizerProperty = DependencyProperty.Register("TopRightCornerResizer", typeof(Style), typeof(Node), new PropertyMetadata(null));
        public static readonly DependencyProperty BottomLeftCornerResizerProperty = DependencyProperty.Register("BottomLeftCornerResizer", typeof(Style), typeof(Node), new PropertyMetadata(null));
        public static readonly DependencyProperty BottomRightCornerResizerProperty = DependencyProperty.Register("BottomRightCornerResizer", typeof(Style), typeof(Node), new PropertyMetadata(null));
        
        /// <summary>
        /// Identifies the Node's Obstacle dependency property.
        /// </summary>
        public static readonly DependencyProperty TreatAsObstacleProperty = DependencyProperty.Register("TreatAsObstacle", typeof(bool), typeof(Node), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the LabelAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelAngleProperty = DependencyProperty.Register("LabelAngle", typeof(double), typeof(Node), new PropertyMetadata(0d, new PropertyChangedCallback(OnLabelAngleChanged)));

        /// <summary>
        /// Identifies the OffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty LogicalOffsetYProperty = DependencyProperty.Register("OffsetY", typeof(double), typeof(Node), new PropertyMetadata(0d, new PropertyChangedCallback(OnLogicalOffsetYChanged)));

        /// <summary>
        /// Identifies the OffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty LogicalOffsetXProperty = DependencyProperty.Register("OffsetX", typeof(double), typeof(Node), new PropertyMetadata(0d, new PropertyChangedCallback(OnLogicalOffsetXChanged)));

        /// <summary>
        /// Identifies the RotateAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateAngleProperty = DependencyProperty.Register("RotateAngle", typeof(double), typeof(Node), new PropertyMetadata(0d, new PropertyChangedCallback(OnRotateAngleChanged)));

        /// <summary>
        /// Identifies the LabelWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register("LabelWidth", typeof(double), typeof(Node), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the LabelHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHeightProperty = DependencyProperty.Register("LabelHeight", typeof(double), typeof(Node), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the LabelTextWrapping dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextWrappingProperty = DependencyProperty.Register("LabelTextWrapping", typeof(TextWrapping), typeof(Node), new PropertyMetadata(TextWrapping.NoWrap));

        /// <summary>
        /// Identifies the LabelFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register("LabelFontSize", typeof(double), typeof(Node), new PropertyMetadata(11d));

        /// <summary>
        /// Identifies the LabelFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontFamilyProperty = DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(Node), new PropertyMetadata(new FontFamily("Arial")));

        /// <summary>
        /// Identifies the LabelFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontWeightProperty = DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(Node), new PropertyMetadata(FontWeights.SemiBold));

        /// <summary>
        /// Identifies the LabelFontStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontStyleProperty = DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(Node), new PropertyMetadata(FontStyles.Normal));

        /// <summary>
        /// Identifies the LabelTextAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextAlignmentProperty = DependencyProperty.Register("LabelTextAlignment", typeof(TextAlignment), typeof(Node), new PropertyMetadata(TextAlignment.Center));

        /// <summary>
        /// Identifies the GripperVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperVisibilityProperty = DependencyProperty.Register("GripperVisibility", typeof(Visibility), typeof(Node), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the GripperStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperStyleProperty = DependencyProperty.Register("GripperStyle", typeof(Style), typeof(Node), new PropertyMetadata(new Style()));

        /// <summary>
        ///  Identifies the LabelBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty = DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(Node), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        ///  Identifies the LabelForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(Node), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies the IsLabelEditable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLabelEditableProperty = DependencyProperty.Register("IsLabelEditable", typeof(bool), typeof(Node), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the AllowSelect dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowSelectProperty = DependencyProperty.Register("AllowSelect", typeof(bool), typeof(Node), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the AllowMove dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowMoveProperty = DependencyProperty.Register("AllowMove", typeof(bool), typeof(Node), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the AllowRotate dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowRotateProperty = DependencyProperty.Register("AllowRotate", typeof(bool), typeof(Node), new PropertyMetadata(true, new PropertyChangedCallback(OnAllowRotatePropertyChanged)));

        /// <summary>
        /// Identifies the AllowResize dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowResizeProperty = DependencyProperty.Register("AllowResize", typeof(bool), typeof(Node), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the AllowPortDrag dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowPortDragProperty = DependencyProperty.Register("AllowPortDrag", typeof(bool), typeof(Node), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the MeasurementUnits property.
        /// </summary>
        public static readonly DependencyProperty MeasurementUnitsProperty = DependencyProperty.Register("MeasurementUnits", typeof(MeasureUnits), typeof(Node), new PropertyMetadata(MeasureUnits.Pixel, new PropertyChangedCallback(OnUnitsChanged)));

        /// <summary>
        /// Identifies the ParentId dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(Node), new PropertyMetadata(new Guid()));

        /// <summary>
        /// Identifies the IsGroup dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGroupedProperty = DependencyProperty.Register("IsGrouped", typeof(bool), typeof(Node), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(Node), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Identifies the DragProviderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty DragProviderTemplateProperty = DependencyProperty.RegisterAttached("DragProviderTemplate", typeof(ControlTemplate), typeof(Node), new PropertyMetadata(new ControlTemplate()));

        /// <summary>
        /// Identifies the IsDragConnectionOver dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragConnectionOverProperty = DependencyProperty.Register("IsDragConnectionOver", typeof(bool), typeof(Node), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDragConnectionOverChanged)));

        public static readonly DependencyProperty AddConnectionPortEnabledProperty = DependencyProperty.Register("AddConnectionPortEnabled", typeof(bool), typeof(Node), new PropertyMetadata(false));

        public bool AddConnectionPortEnabled
        {
            get
            {
                return (bool)GetValue(AddConnectionPortEnabledProperty);
            }

            set
            {
                SetValue(AddConnectionPortEnabledProperty, value);
            }
        }


        /// <summary>
        /// Identifies the Shape dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register("Shape", typeof(Shapes), typeof(Node), new PropertyMetadata(Shapes.Default, new PropertyChangedCallback(OnShapeChanged)));

        /// <summary>
        /// Identifies the CustomPathStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomPathStyleProperty = DependencyProperty.Register("CustomPathStyle", typeof(Style), typeof(Node), new PropertyMetadata(null, OnCustomPathStyleChanged));
        public static readonly DependencyProperty NodePathFillProperty = DependencyProperty.Register("NodePathFill", typeof(Brush), typeof(Node), new PropertyMetadata(null, OnNodePathFillChanged));

        private static void OnNodePathFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Node n = d as Node;
        }

        public static readonly DependencyProperty NodePathStrokeProperty = DependencyProperty.Register("NodePathStroke", typeof(Brush), typeof(Node), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));
        public static readonly DependencyProperty NodePathStrokeThicknessProperty = DependencyProperty.Register("NodePathStrokeThickness", typeof(double), typeof(Node), new PropertyMetadata(1d));

        internal static readonly DependencyProperty DuplicateWidthProperty = DependencyProperty.Register("DuplicateWidth", typeof(double), typeof(Node), new PropertyMetadata(-1d, new PropertyChangedCallback(OnDuplicateWidthChanged)));
        internal static readonly DependencyProperty DuplicateHeightProperty = DependencyProperty.Register("DuplicateHeight", typeof(double), typeof(Node), new PropertyMetadata(-1d, new PropertyChangedCallback(OnDuplicateHeightChanged)));

        /// <summary>
        /// Identifies the Level dependency property.
        /// </summary>
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(int), typeof(Node), new PropertyMetadata(0));

        /// <summary>
        /// Identifies the LabelVerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty = DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), typeof(Node), new PropertyMetadata((VerticalAlignment.Center), new PropertyChangedCallback(OnLabelVerticalAlignmentChanged)));

        /// <summary>
        /// Identifies the LabelHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty = DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(Node), new PropertyMetadata((HorizontalAlignment.Center), new PropertyChangedCallback(OnLabelHorizontalAlignmentChanged)));

        /// <summary>
        /// Identifies the Label dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(Node), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnLabelChanged)));

        /// <summary>
        /// Identifies the LabelVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(Node), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the PortVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty PortVisibilityProperty = DependencyProperty.Register("PortVisibility", typeof(PortVisibility), typeof(Node), new PropertyMetadata((PortVisibility.MouseOverNode), new PropertyChangedCallback(OnPortVisibilityChanged)));

        /// <summary>
        /// Identifies the IsCenterPortEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPortEnabledProperty = DependencyProperty.Register("IsPortEnabled", typeof(bool), typeof(Node), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the IsDoubleClicked dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDoubleClickedProperty = DependencyProperty.Register("IsDoubleClicked", typeof(bool), typeof(Node), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the ContextMenu dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuProperty = DependencyProperty.Register("ContextMenu", typeof(ContextMenuControl), typeof(Node), new PropertyMetadata(null,new PropertyChangedCallback(OnContextMenuChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DeletingMode.
        /// </summary>
        public static readonly DependencyProperty DeletingModeProperty = DependencyProperty.Register("DeletingMode", typeof(DeletingMode), typeof(Node), new PropertyMetadata(DeletingMode.DeleteDependentEdges));

       

        private static void OnContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Node node = d as Node;

            if (args.NewValue != null)
            {
                //for Default contextMenu              
             ContextMenuControlService.SetContextMenuControl(node, (ContextMenuControl)args.NewValue);                
            }
               
        }

        private static void OnCustomPathStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Node node = d as Node;
            Node.UpdateStyle(node);
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises the click event.
        /// </summary>
        public void RaiseClickEvent()
        {
            NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
            dview.OnNodeClick(this, newEventArgs);

        }

        /// <summary>
        /// Raises the drag start event.
        /// </summary>
        internal double x, y;
        public void RaiseNodeDragStartEvent()
        {
            if (Node.mouseup)
            {
                dc.View._LineRoute = true;
                foreach (ICommon node in dview.SelectionList.OfType<ICommon>())
                {
                    if (node is Group)
                    {
                        NodeRoutedEventArgs newEventArgs1 = new NodeRoutedEventArgs(node as Node);
                        dview.OnNodeDragStart(this, newEventArgs1);
                        (node as Group).x = newEventArgs1.Node.OffsetX;
                        (node as Group).y = newEventArgs1.Node.OffsetY;
                        foreach (INodeGroup n in (node as Group).NodeChildren)
                        {
                            if (n is Node)
                            {
                                (n as Node).x = (n as Node).OffsetX;
                                (n as Node).y = (n as Node).OffsetY;
                            }
                            else
                            {
                                (n as LineConnector).sp = (n as LineConnector).StartPointPosition;
                                (n as LineConnector).ep = (n as LineConnector).EndPointPosition;
                            }
                        }
                    }
                    if (node is Node)
                    {
                        NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node as Node);
                        dview.OnNodeDragStart(this, newEventArgs);
                        (node as Node).x = newEventArgs.Node.OffsetX;
                        (node as Node).y = newEventArgs.Node.OffsetY;
                    }
                    if (node is LineConnector)
                    {
                        (node as LineConnector).sp = (node as LineConnector).StartPointPosition;
                        (node as LineConnector).ep = (node as LineConnector).EndPointPosition;
                    }
                }
                Node.mouseup = false;
            }
        }

        /// <summary>
        /// Raises the drag end event.
        /// </summary>
        public void RaiseNodeDragEndEvent()
        {
            NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
            dview.OnNodeDragEnd(this, newEventArgs);
        }

        /// <summary>
        /// Raises the double click event.
        /// </summary>
        public void RaiseDoubleClickEvent()
        {
            NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
            dview.OnNodeDoubleClick(this, newEventArgs);
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
        /// <param name="name">The property name </param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        internal event EventHandler MouseOverEvent;

        private void fireMouseOverEvent()
        {
            if (MouseOverEvent != null)
            {
                MouseOverEvent.Invoke(this, new EventArgs());
            }
        }

        #endregion

        #region class override

        internal bool m_LayoutDisconnected;
        //private ResourceDictionary rs;
        private static Dictionary<string, string> noders;
        //private Canvas NodeCanvas;
        private bool m_IsPixelDefultUnit = true;
        private FrameworkElement m_MoveCursor;

        private FrameworkElement m_RotateCursor;

        private FrameworkElement getMoveCursor()
        {
            //String move = "<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" IsHitTestVisible=\"False\">" +
            //"<Path Width=\"16\" Height=\"16\" Stretch=\"Fill\" Fill=\"#FFF5F5F5\" Data=\"M 392.718,291.333L 392.718,290.657L 392.047,290.635L 392.047,289.986L 391.375,289.963L 391.375,289.314L 390.704,289.292L 390.704,288.089L 392.718,288.089L 392.718,283.969L 388.586,283.952L 388.569,285.963L 387.347,285.963L 387.347,285.291L 386.676,285.291L 386.676,284.615L 386.005,284.593L 386.005,283.949L 385.333,283.949L 385.333,282.718L 386.01,282.718L 386.032,282.047L 386.676,282.047L 386.676,281.375L 387.347,281.375L 387.347,280.704L 388.578,280.704L 388.578,282.718L 392.697,282.718L 392.715,278.586L 390.704,278.569L 390.704,277.347L 391.375,277.347L 391.375,276.676L 392.047,276.676L 392.047,276.005L 392.723,276.005L 392.745,275.333L 393.949,275.333L 393.949,276.01L 394.62,276.032L 394.62,276.676L 395.291,276.676L 395.291,277.347L 395.963,277.347L 395.963,278.578L 393.949,278.578L 393.949,282.697L 398.089,282.715L 398.089,280.704L 399.319,280.704L 399.319,281.375L 399.991,281.375L 399.991,282.047L 400.662,282.047L 400.662,282.718L 401.333,282.718L 401.333,283.949L 400.662,283.949L 400.662,284.62L 399.991,284.62L 399.991,285.291L 399.319,285.291L 399.319,285.963L 398.089,285.963L 398.089,283.949L 393.97,283.949L 393.952,288.081L 395.963,288.098L 395.963,289.319L 395.286,289.319L 395.264,289.991L 394.615,289.991L 394.593,290.662L 393.944,290.662L 393.921,291.333L 392.718,291.333 Z \"/>" +
            //"<Path Width=\"15.4406\" Height=\"15.4406\"  Stretch=\"Fill\" Fill=\"#FF000000\" Data=\"M 393.669,275.613C 393.669,275.826 393.669,276.038 393.669,276.251C 393.826,276.329 394.127,276.262 394.34,276.284C 394.34,276.508 394.34,276.732 394.34,276.956C 394.564,276.956 394.788,276.956 395.012,276.956C 395.012,277.179 395.012,277.403 395.012,277.627C 395.235,277.627 395.459,277.627 395.683,277.627C 395.683,277.851 395.683,278.075 395.683,278.298C 395.012,278.298 394.34,278.298 393.669,278.298C 393.669,279.854 393.669,281.409 393.669,282.964C 395.168,283.042 396.813,282.975 398.368,282.998C 398.368,282.326 398.368,281.655 398.368,280.984C 398.592,280.984 398.816,280.984 399.04,280.984C 399.04,281.207 399.04,281.431 399.04,281.655C 399.263,281.655 399.487,281.655 399.711,281.655C 399.711,281.879 399.711,282.103 399.711,282.326C 399.935,282.326 400.159,282.326 400.382,282.326C 400.382,282.55 400.382,282.774 400.382,282.998C 400.606,282.998 400.83,282.998 401.054,282.998C 401.054,283.221 401.054,283.445 401.054,283.669C 400.83,283.669 400.606,283.669 400.382,283.669C 400.382,283.893 400.382,284.117 400.382,284.34C 400.159,284.34 399.935,284.34 399.711,284.34C 399.711,284.564 399.711,284.788 399.711,285.012C 399.487,285.012 399.263,285.012 399.04,285.012C 399.04,285.235 399.04,285.459 399.04,285.683C 398.816,285.683 398.592,285.683 398.368,285.683C 398.368,285.012 398.368,284.34 398.368,283.669C 396.813,283.669 395.258,283.669 393.703,283.669C 393.624,285.157 393.691,286.791 393.669,288.335C 394.273,288.413 395.023,288.346 395.683,288.368C 395.683,288.592 395.683,288.816 395.683,289.04C 395.47,289.04 395.258,289.04 395.045,289.04C 394.967,289.197 395.034,289.498 395.012,289.711C 394.799,289.711 394.587,289.711 394.374,289.711C 394.296,289.868 394.362,290.169 394.34,290.382C 394.128,290.382 393.915,290.382 393.703,290.382C 393.625,290.539 393.691,290.841 393.669,291.054C 393.445,291.054 393.221,291.054 392.998,291.054C 392.998,290.841 392.998,290.628 392.998,290.416C 392.841,290.338 392.539,290.404 392.326,290.382C 392.326,290.17 392.326,289.957 392.326,289.745C 392.169,289.667 391.868,289.733 391.655,289.711C 391.655,289.498 391.655,289.286 391.655,289.073C 391.498,288.995 391.197,289.062 390.984,289.04C 390.984,288.816 390.984,288.592 390.984,288.368C 391.655,288.368 392.326,288.368 392.998,288.368C 392.998,286.813 392.998,285.258 392.998,283.703C 391.51,283.624 389.876,283.691 388.332,283.669C 388.254,284.273 388.321,285.023 388.298,285.683C 388.075,285.683 387.851,285.683 387.627,285.683C 387.627,285.459 387.627,285.235 387.627,285.012C 387.403,285.012 387.179,285.012 386.956,285.012C 386.956,284.799 386.956,284.586 386.956,284.374C 386.799,284.296 386.497,284.362 386.284,284.34C 386.284,284.117 386.284,283.893 386.284,283.669C 386.061,283.669 385.837,283.669 385.613,283.669C 385.613,283.445 385.613,283.221 385.613,282.998C 385.826,282.998 386.038,282.998 386.251,282.998C 386.329,282.841 386.262,282.539 386.284,282.326C 386.508,282.326 386.732,282.326 386.956,282.326C 386.956,282.103 386.956,281.879 386.956,281.655C 387.179,281.655 387.403,281.655 387.627,281.655C 387.627,281.431 387.627,281.207 387.627,280.984C 387.851,280.984 388.075,280.984 388.298,280.984C 388.298,281.655 388.298,282.326 388.298,282.998C 389.854,282.998 391.409,282.998 392.964,282.998C 393.042,281.51 392.975,279.876 392.998,278.332C 392.393,278.254 391.644,278.321 390.984,278.298C 390.984,278.075 390.984,277.851 390.984,277.627C 391.207,277.627 391.431,277.627 391.655,277.627C 391.655,277.403 391.655,277.179 391.655,276.956C 391.879,276.956 392.103,276.956 392.326,276.956C 392.326,276.732 392.326,276.508 392.326,276.284C 392.539,276.284 392.751,276.284 392.964,276.284C 393.042,276.127 392.976,275.826 392.998,275.613C 393.221,275.613 393.445,275.613 393.669,275.613 Z \"/>" +
            //"<Canvas.RenderTransform>" +
            //    "<TranslateTransform X=\"-7\" Y =\"-7\"/>" +
            //"</Canvas.RenderTransform>" +
            //"</Canvas>";

            if (m_MoveCursor == null)
            {
                string move2 = "<Viewbox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Width=\"24\" Height=\"24\">" +
                "<Grid Height=\"240\" Width=\"240\">" +
                    "<Path Stretch=\"Fill\" IsHitTestVisible=\"False\" Stroke=\"Black\" Fill=\"White\" StrokeThickness=\"7\" Data=\"M 392.718,291.333L 392.718,290.657L 392.047,290.635L 392.047,289.986L 391.375,289.963L 391.375,289.314L 390.704,289.292L 390.704,288.089L 392.718,288.089L 392.718,283.969L 388.586,283.952L 388.569,285.963L 387.347,285.963L 387.347,285.291L 386.676,285.291L 386.676,284.615L 386.005,284.593L 386.005,283.949L 385.333,283.949L 385.333,282.718L 386.01,282.718L 386.032,282.047L 386.676,282.047L 386.676,281.375L 387.347,281.375L 387.347,280.704L 388.578,280.704L 388.578,282.718L 392.697,282.718L 392.715,278.586L 390.704,278.569L 390.704,277.347L 391.375,277.347L 391.375,276.676L 392.047,276.676L 392.047,276.005L 392.723,276.005L 392.745,275.333L 393.949,275.333L 393.949,276.01L 394.62,276.032L 394.62,276.676L 395.291,276.676L 395.291,277.347L 395.963,277.347L 395.963,278.578L 393.949,278.578L 393.949,282.697L 398.089,282.715L 398.089,280.704L 399.319,280.704L 399.319,281.375L 399.991,281.375L 399.991,282.047L 400.662,282.047L 400.662,282.718L 401.333,282.718L 401.333,283.949L 400.662,283.949L 400.662,284.62L 399.991,284.62L 399.991,285.291L 399.319,285.291L 399.319,285.963L 398.089,285.963L 398.089,283.949L 393.97,283.949L 393.952,288.081L 395.963,288.098L 395.963,289.319L 395.286,289.319L 395.264,289.991L 394.615,289.991L 394.593,290.662L 393.944,290.662L 393.921,291.333L 392.718,291.333 Z \" UseLayoutRounding=\"True\"/>" +
                    "<Grid.RenderTransform>" +
                        "<TranslateTransform X=\"-120\" Y =\"-120\"/>" +
                    "</Grid.RenderTransform>" +
                "</Grid>" +
            "</Viewbox>";

                m_MoveCursor = System.Windows.Markup.XamlReader.Load(move2) as FrameworkElement;
                return m_MoveCursor;
            }
            else
            {
                return m_MoveCursor;
            }
        }

        private FrameworkElement getRotateCursor()
        {
            if (m_RotateCursor == null)
            {
                string rotate = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" IsHitTestVisible=\"False\" Stretch=\"Uniform\" Fill=\"#FF000000\" Stroke=\"Black\" Height=\"15\" Width=\"15\" Margin=\"-6\" Data=\"F1 M -208.797,404.523C -209.425,407.885 -212.377,410.44 -215.918,410.44C -219.916,410.44 -223.168,407.188 -223.168,403.19C -223.168,399.393 -220.232,396.274 -216.512,395.97L -216.185,398.607L -212.934,396.15L -209.683,393.695L -213.435,392.107L -217.188,390.519L -216.841,393.32C -221.884,393.786 -225.835,398.025 -225.835,403.19C -225.835,408.667 -221.395,413.107 -215.918,413.107C -210.895,413.107 -206.754,409.368 -206.102,404.523L -208.797,404.523 Z \"/>";
                m_RotateCursor = System.Windows.Markup.XamlReader.Load(rotate) as FrameworkElement;
                return m_RotateCursor;
            }
            else
            {
                return m_RotateCursor;
            }
        }

        private void setCursor()
        {
            if (CustomCursorEnabled)
            {
                FrameworkElement _cursor = getMoveCursor();
                CustomCursor oldCursor = CustomCursor.GetCustomCursor(this);
                if (oldCursor != null)
                {
                    oldCursor.Content = new Canvas();
                }
                CustomCursor.SetCustomCursor(this, new CustomCursor(_cursor) { _Move = _cursor, _Rotate = getRotateCursor() });
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.DragProvider = GetTemplateChild("PART_DragProvider") as Thumb;
            this.Resizer = GetTemplateChild("PART_Resizer") as Resizer;
            this.Nodepanel = GetTemplateChild("PART_NodeGrid") as Grid;
            if (this.NodeShape != null)
            {
                Path nodepath = GetTemplateChild("PART_Shape") as System.Windows.Shapes.Path;
                int index = this.Nodepanel.Children.IndexOf(nodepath);
                this.Nodepanel.Children.Insert(index, this.NodeShape);
                this.Nodepanel.Children.Remove(nodepath);
            }
            else
            {
                this.NodeShape = GetTemplateChild("PART_Shape") as System.Windows.Shapes.Path;
            }

            this.nodetext = GetTemplateChild("PART_TextBox") as TextBox;
            portItems = GetTemplateChild("PART_PortItems") as ItemsControl;
            this.Ports.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Ports_CollectionChanged);
            this.nodelabel = GetTemplateChild("PART_TextBlock") as TextBlock;
            this.nodelayouttransformer = GetTemplateChild("PART_LayoutTransformer") as LayoutTransformer;
           
            var move = new TranslateTransform();
            move.X = this.LabelDisplacement.X;
            move.Y = this.LabelDisplacement.Y;
           
            
            var group = new TransformGroup();
            group.Children.Add(move);           
            this.nodelabel.RenderTransform = group;
            this.nodetext.RenderTransform = group;
            RotateTransform rt = new RotateTransform();
            rt.Angle = this.LabelAngle;
            this.nodelayouttransformer.LayoutTransform = rt;
            this.ApplyShape();
            nodeSelection();
            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (double.IsNaN(this.Width))
            {
                this.Width = DesiredSize.Width;
            }
            if (double.IsNaN(this.Height))
            {
                this.Height = DesiredSize.Height;
            }
            if (IsFirstApplyTemplate)
            {
                IsFirstApplyTemplate = false;
                if (this.dc == null)
                {
                    this.dc = DiagramPage.GetDiagramControl(this);
                    this.dview = this.dc.View;
                    if (dview != null && dview.Page != null)
                    {
                        if (this.MeasurementUnits == MeasureUnits.Pixel && this.MeasurementUnits != (dview.Page as DiagramPage).MeasurementUnits)
                        {
                            m_IsPixelDefultUnit = false;
                        }
                        System.Windows.Data.Binding measure = new System.Windows.Data.Binding("MeasurementUnits");
                        measure.Source = dview.Page;
                        this.SetBinding(Node.MeasurementUnitsProperty, measure);
                    }
                }
            }
            int j = 1;
            foreach (ConnectionPort port in this.Ports)
            {
                port.Node = this;
                this.portItems.Items.Add(port);
                port.PortReferenceNo = j++;
            }
            if (!DiagramControl.IsPageLoaded)
            {
                centerport = new ConnectionPort();
                centerport.Name = "PART_Sync_CenterPort";
                centerport.Node = this;               
                centerport.PortShape = PortShapes.Circle;
                if (!double.IsNaN(this.Width) || !double.IsNaN(this.Height))
                {
                    if (double.IsNaN(centerport.Width) || double.IsNaN(centerport.Height))
                    {
                        centerport.Left = this.Width / 2;
                        centerport.Top = this.Height / 2;
                    }
                    else
                    {
                        centerport.Left = this.Width / 2 - centerport.Width / 2;
                        centerport.Top = this.Height / 2 - centerport.Height / 2;
                    }
                }
                else
                {
                    double two0ne = 21;//  MeasureUnitsConverter.FromPixels(21, this.MeasurementUnits);
                    centerport.PxLeft = two0ne;
                    centerport.PxTop = two0ne;
                }

                if (this.Ports != null)
                {
                    this.Ports.Add(centerport);
                }
                centerport.CenterPortReferenceNo = 0;
                centerport.PortReferenceNo = -1;
            }
            this.Nodepanel.SizeChanged += new SizeChangedEventHandler(Nodepanel_SizeChanged);
            if (this.PathStyle.PathObject != null && this.Shape == Shapes.Default)
            {
                this.NodeShape = this.PathStyle.PathObject;
                //this.nodegrid.Children.Add(this.NodeShape);
            }
            if (this is Group && this.IsSelected)
            {
                //pop.IsOpen = true;
                this.Resizer.Visibility = Visibility.Visible;
                //Thumb outerborder = new Thumb();
                //outerborder.Style = this.rs["RotateDecorator"] as Style;
                //this.Nodepanel.Children.Add(outerborder);
            }

            dc = DiagramPage.GetDiagramControl(this);
            if (dc != null)
            {
                ContextMenu_Delete = dc.m_ResourceWrapper.ContextMenu_Delete;
                ContextMenu_Grouping = dc.m_ResourceWrapper.ContextMenu_Grouping;
                ContextMenu_Grouping_Group = dc.m_ResourceWrapper.ContextMenu_Grouping_Group;
                ContextMenu_Grouping_Ungroup = dc.m_ResourceWrapper.ContextMenu_Grouping_Ungroup;
                ContextMenu_Order = dc.m_ResourceWrapper.ContextMenu_Order;
                ContextMenu_Order_BringForward = dc.m_ResourceWrapper.ContextMenu_Order_BringForward;
                ContextMenu_Order_BringToFront = dc.m_ResourceWrapper.ContextMenu_Order_BringToFront;
                ContextMenu_Order_SendBackward = dc.m_ResourceWrapper.ContextMenu_Order_SendBackward;
                ContextMenu_Order_SendToBack = dc.m_ResourceWrapper.ContextMenu_Order_SendToBack;
            }
            if (dc != null)
            {
                System.Windows.Data.Binding cusrtom = new System.Windows.Data.Binding("CustomCursorEnabled");
                cusrtom.Source = dc.View;
                this.SetBinding(Node.CustomCursorEnabledProperty, cusrtom);
            }
            setCursor();

            this.nodelabel.MouseLeftButtonDown += new MouseButtonEventHandler(nodelabel_MouseLeftButtonDown);
            this.nodelabel.MouseLeftButtonUp += new MouseButtonEventHandler(nodelabel_MouseLeftButtonUp);
            this.nodelabel.MouseEnter += new MouseEventHandler(nodelabel_MouseEnter);
            this.nodelabel.MouseLeave += new MouseEventHandler(nodelabel_MouseLeave);
            this.nodelabel.MouseMove += new MouseEventHandler(nodelabel_MouseMove);

            if (dc != null && dc.View != null && this.ReadLocalValue(PortVisibilityProperty).Equals(DependencyProperty.UnsetValue))
            {
                Binding portVisibilityBinding = new Binding("PortVisibility");
                portVisibilityBinding.Source = dc.View;
                this.SetBinding(PortVisibilityProperty, portVisibilityBinding);
            }
        }

        ContextMenuControlItem Order;
        ContextMenuControlItem Front;
        ContextMenuControlItem Forward;
        ContextMenuControlItem Backward;
        ContextMenuControlItem Back;
        ContextMenuControlItem Grouping;
        ContextMenuControlItem Group;
        ContextMenuControlItem Ungroup;
        ContextMenuControlItem Delete;
        //Creating the contextmenu
        private void CreateContextMenu()
        {
            nodecontextmenu = new ContextMenuControl();
            Order = new ContextMenuControlItem();
            Order.Header = ContextMenu_Order;// "Order";
            //header.Add(ContextMenu_Order);
            Front = new ContextMenuControlItem();
            Front.Header = ContextMenu_Order_BringToFront;// "Bring To Front";
            Front.Click += new RoutedEventHandler(Front_Click);
            Order.Items.Add(Front);
            Forward = new ContextMenuControlItem();
            Forward.Header = ContextMenu_Order_BringForward;// "Bring Forward";
            Forward.Click += new RoutedEventHandler(Forward_Click);
            Order.Items.Add(Forward);
            Backward = new ContextMenuControlItem();
            Backward.Header = ContextMenu_Order_SendBackward;// "Send Backward";
            Backward.Click += new RoutedEventHandler(Backward_Click);
            Order.Items.Add(Backward);
            Back = new ContextMenuControlItem();
            Back.Header = ContextMenu_Order_SendToBack;// "Send To Back";
            Back.Click += new RoutedEventHandler(Back_Click);
            Order.Items.Add(Back);

            Grouping = new ContextMenuControlItem();
            Grouping.Header = ContextMenu_Grouping;// "Grouping";
            //header.Add(ContextMenu_Grouping);
            Group = new ContextMenuControlItem();
            Group.Header = ContextMenu_Grouping_Group;// "Group";
            Group.Click += new RoutedEventHandler(Group_Click);
            Grouping.Items.Add(Group);
            Ungroup = new ContextMenuControlItem();
            Ungroup.Header = ContextMenu_Grouping_Ungroup;// "Ungroup";
            Ungroup.Click += new RoutedEventHandler(Ungroup_Click);
            Grouping.Items.Add(Ungroup);

            Delete = new ContextMenuControlItem();
            Delete.Header = ContextMenu_Delete;// "Delete";
            //header.Add(ContextMenu_Delete);
            Delete.Click += new RoutedEventHandler(Delete_Click);

            nodecontextmenu.Items.Add(Order);
            nodecontextmenu.Items.Add(Grouping);
            nodecontextmenu.Items.Add(Delete);
                    }

        private bool dragging = false;
        private Point startPosition;
        private bool labelPressed = false;
        private Point labelcurrentPosition;
        private Point previousPosition;
        private Transform previousTransform;
        void nodelabel_MouseMove(object sender, MouseEventArgs e)
        {
            this.nodelabel.CaptureMouse();
            if (this.IsLabelDragable)
            {
                labelcurrentPosition = e.GetPosition(this);
                if (this.labelPressed)
                {
                    this.dragging = true;
                    var move = new TranslateTransform();
                    move.X = labelcurrentPosition.X - previousPosition.X;
                    move.Y = labelcurrentPosition.Y - previousPosition.Y;
                    if (this.nodelabel.Cursor == Cursors.Hand)
                    {
                        var group = new TransformGroup();
                        if (previousTransform != null)
                        {
                            group.Children.Add(previousTransform);
                        }
                        group.Children.Add(move);
                        this.nodelabel.RenderTransform = group;
                        this.nodetext.RenderTransform = group;
                    }
                }
                else
                {
                    this.nodelabel.Cursor = Cursors.Hand;
                }
                previousPosition = labelcurrentPosition;
                previousTransform = this.nodelabel.RenderTransform;
            }
            base.OnMouseMove(e);
        }
        void nodelabel_MouseLeave(object sender, MouseEventArgs e)
        {
            this.nodelabel.ReleaseMouseCapture();
            base.OnMouseLeave(e);
        }

        void nodelabel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (labelPressed)
            {
                this.nodelabel.CaptureMouse();
            }
            base.OnMouseEnter(e);
        }

        void nodelabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.nodelabel.ReleaseMouseCapture();
            if (this.IsLabelDragable)
            {
                this.LabelDisplacement = new Point(this.labelcurrentPosition.X - this.startPosition.X, this.labelcurrentPosition.Y - this.startPosition.Y);
            }
            base.OnMouseLeftButtonUp(e);
            this.labelPressed = false;
            this.dragging = false;
        }

        void nodelabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.labelPressed = true;
            this.nodelabel.CaptureMouse();
            if (this.IsLabelDragable && startPosition == new Point(0, 0))
            {
                this.startPosition = e.GetPosition(this);
            }
            base.OnMouseLeftButtonDown(e);
        }

        void Nodepanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width != 0 && e.NewSize.Height != 0)
            {
                foreach (ConnectionPort port in this.Ports)
                {
                    if (this.m_MouseResizing)
                    {
                        port.PxLeft=port.oldCenterPosition.X + port.oldCenterPosition.X * ((port.Node._Width - port.mNodeoldSize.Width) / port.mNodeoldSize.Width);
                        port.PxTop =port.oldCenterPosition.Y + port.oldCenterPosition.Y * ((port.Node._Height - port.mNodeoldSize.Height) / port.mNodeoldSize.Height);
                    }
                    else
                    {
                        if (e.PreviousSize.Width == 0 && e.PreviousSize.Height == 0)
                        {
                            port.PxLeft = port.PxCenterPosition.X;
                            port.PxTop = port.PxCenterPosition.Y;
                        }
                        else
                        {
                            port.PxLeft = port.PxCenterPosition.X + port.PxCenterPosition.X * ((e.NewSize.Width - e.PreviousSize.Width) / e.PreviousSize.Width);
                            port.PxTop = port.PxCenterPosition.Y + port.PxCenterPosition.Y * ((e.NewSize.Height - e.PreviousSize.Height) / e.PreviousSize.Height);
                        }
                    }
                }
            }
        }
        int mremove = 0;
        void Ports_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ConnectionPort port in e.NewItems)
                {
                    port.Node = this;
                    port.PortReferenceNo = this.Ports.Count - 1 + mremove;
                    this.portItems.Items.Add(port);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ConnectionPort port in e.OldItems)
                {
                    this.portItems.Items.Remove(port);
                    mremove++;
                }
            }
        }

        private double findAngle(Point s, Point e)
        {
            Point r = new Point(e.X, s.Y);
            double sr = this.findHypo(s, r);
            double re = this.findHypo(r, e);
            double es = this.findHypo(e, s);
            double ang = Math.Asin(re / es);
            ang = ang * 180 / Math.PI;
            if (s.X < e.X)
            {
                if (s.Y < e.Y)
                {
                }
                else
                {
                    ang = 360 - ang;
                }
            }
            else
            {
                if (s.Y < e.Y)
                {
                    ang = 180 - ang;
                }
                else
                {
                    ang = 180 + ang;
                }
            }

            if (double.IsNaN(ang))
            {
                return 0;
            }
            else
            {
                return ang - 206;
            }
        }

        private double findHypo(Point s, Point e)
        {
            double length;
            length = Math.Sqrt(Math.Pow((s.X - e.X), 2) + Math.Pow((s.Y - e.Y), 2));
            return length;
        }

        private void Rotator_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.dview.IsRotating)
            {
                this.RenderTransformOrigin = new Point(.5, .5);
                this.rotateTransform = new RotateTransform();
                if (this.rotateTransform == null)
                {
                    this.rotateTransform.Angle = 0;
                }
                else
                {
                }
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            IsMouseOver = true;
            dview.nodragging = false;
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            IsMouseOver = false;
            if (dview != null)
            {
                //if (dview.ScrollGrid.ScrollOwner.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden || dview.ScrollGrid.ScrollOwner.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
                //{
                //    (dview.Page as DiagramPage).RulerUpdate();
                //}
            }

            base.OnMouseLeave(e);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Invoked when Label editing is started.
        /// </summary>
        public void NodeLabeledit()
        {
            if (this.IsLabelEditable)
            {
                LabelRoutedEventArgs newEvantArgs = new LabelRoutedEventArgs(nodetext.Text, null, this);
                dview.OnNodeStartLabelEdit(this, newEvantArgs);

            }
        }

        /// <summary>
        /// Invoked when label editing is complete.
        /// </summary>
        internal void CompleteEditing()
        {
        }

        /// <summary>
        /// Called when the mouse button is clicked twice.
        /// </summary>
        /// <param name="position">Mouse Position</param>
        /// <returns>true if double clicked, false otherwise</returns>
        public bool IsDoubleClick(Point position)
        {
            if (((DateTime.Now.Subtract(this.lastNodeClick).TotalMilliseconds < 500) && (Math.Abs((double)(this.lastNodePoint.X - position.X)) <= 2)) && (Math.Abs((double)(this.lastNodePoint.Y - position.Y)) <= 2))
            {
                return true;
            }

            return false;
        }

        //internal Popup pop;
        //internal void createPopup(MouseEventArgs e)
        //{
        //    Point p = e.GetPosition(this.Nodepanel);
        //    pop.HorizontalOffset = (p.X);// *this.dview.CurrentZoom;
        //    pop.VerticalOffset = (p.Y);


        //}

        private void RefreshPopup(string str)
        {
            BitmapImage imgb = new BitmapImage(new Uri(@"/Syncfusion.Diagram.Silverlight;component/Icons/" + str + ".png", UriKind.Relative));

            //(((pop.Child as Border).Child as StackPanel).Children[0] as Image).Source = imgb;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Node control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Node_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(HitPort!=null)
            HitPort.IsDragOverPort = false;
            if (dview != null)
            {
                if (dview.ScrollGrid.ScrollOwner.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden || dview.ScrollGrid.ScrollOwner.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
                {
                    //(dview.Page as DiagramPage).RulerUpdate();
                }
            }
            dview.nodedrag = false;
            Rect mpagesize = (dview.Page as DiagramPage).PageData();
            dview.mallowDrawandDrag = true;
            dview.ScrollGrid.InvalidateMeasure();
            dview.ScrollGrid.NodeDragged = true;
            dview.Page.InvalidateMeasure();
            (dview.Page as DiagramPage).Right = (dview.Page as DiagramPage).PageSize.Width;
            (dview.Page as DiagramPage).Bottom = (dview.Page as DiagramPage).PageSize.Height;
            dview.nodragging = true;
            (sender as Node).currentdragging = false;
            if (dview.dviewGrid != null)
            {
                TransformGroup tr = new TransformGroup();
                tr.Children.Add(new TranslateTransform
                {
                    X = -((dview.Page as DiagramPage).FindNeturalLine((dview.Page as DiagramPage).Left,
                        (dview.Page as DiagramPage).PxGridHorizontalOffset)),
                    Y = -((dview.Page as DiagramPage).FindNeturalLine((dview.Page as DiagramPage).Top,
                        ((dview.Page as DiagramPage) as DiagramPage).PxGridVerticalOffset))
                });
                //tr.Children.Add(dview.zoomTransform);
                (dview.dviewGrid as DiagramViewGrid).Measure(new Size(dview.ScrollGrid.ExtentWidth, dview.ScrollGrid.ExtentHeight));
                //(dview.dviewGrid as DiagramViewGrid).RenderTransform = new TranslateTransform { X = -((dview.Page as DiagramPage).FindNeturalLine((dview.Page as DiagramPage).Left,
                // (dview.Page as DiagramPage).PxGridHorizontalOffset)), Y = -((dview.Page as DiagramPage).FindNeturalLine((dview.Page as DiagramPage).Top,
                // ((dview.Page as DiagramPage) as DiagramPage).PxGridVerticalOffset)) };
                (dview.dviewGrid as DiagramViewGrid).RenderTransform = tr;
                // (dview.dviewGrid as DiagramViewGrid).RenderTransform = dview.zoomTransform;
                (dview.dviewGrid as DiagramViewGrid).UpdateGrid();
                //(view.dviewGrid as DiagramViewGrid).RearrrangeHorGridLines();
                //(view.dviewGrid as DiagramViewGrid).RearrrangeVerGridLines();
            }
            if ((this.Page as DiagramPage).dview.VerRuler != null)
            {
                (this.Page as DiagramPage).dview.VerRuler.OffsetY = (-(this.Page as DiagramPage).dview.ScrollGrid._VerticalOffset);
                (this.Page as DiagramPage).dview.VerRuler.PxStartValue = -(this.Page as DiagramPage).Top;
                (this.Page as DiagramPage).dview.HorRuler.Margin = new Thickness(0, 0, -((this.Page as DiagramPage).dview.ScrollGrid._VerticalOffset), 0);
            }
            if ((this.Page as DiagramPage).dview.HorRuler != null)
            {
                (this.Page as DiagramPage).dview.HorRuler.OffsetX = -(-(this.Page as DiagramPage).Left + (this.Page as DiagramPage).dview.ScrollGrid._HorizontalOffset);
                (this.Page as DiagramPage).dview.HorRuler.PxStartValue = -(this.Page as DiagramPage).Left;
                (this.Page as DiagramPage).dview.HorRuler.Margin = new Thickness(-((this.Page as DiagramPage).dview.ScrollGrid._HorizontalOffset), 0, 0, 0);
                (this.Page as DiagramPage).dview.HorRuler.InvalidateMeasure();
            }
            dview.mnodeDownEvent = false;
            foreach (ConnectionPort port in this.Ports)
            {
                port.IsMouseOver = false;
                port.IsMouseDown = false;
                port.IsDragOverPort = false;
            }
            mouseup = true;

            Point p = e.GetPosition(this);
            this.resize = false;


            if (this.dview.viewgrid.Children.ElementAt(this.dview.viewgrid.Children.Count() - 1) is Path)
            {
                this.dview.viewgrid.Children.Remove(this.dview.viewgrid.Children.ElementAt(this.dview.viewgrid.Children.Count() - 1));
            }

            if (this.HitNodeConnector != null)
            {
                LineConnector line = new LineConnector();
                line.ConnectorType = (this.dview.Page as DiagramPage).ConnectorType;
                if (this.HitPort != null || this.FixedPort!=null)
                {
                    line.HeadNode = this.fixedNodeConnection;
                    line.TailNode = this.HitNodeConnector;
                    line.ConnectionHeadPort = this.FixedPort;
                    line.ConnectionTailPort = this.HitPort;
                }
                else
                {
                    line.HeadNode = this.fixedNodeConnection;
                    line.TailNode = this.HitNodeConnector;   
                }
                if (this.dc != null)
                {
                    this.dc.Model.Connections.Add(line);

                    ConnDragEndRoutedEventArgs connEndEventnewEventArgs = new ConnDragEndRoutedEventArgs(line.HeadNode as Node, line.TailNode as Node, line);
                    dview.OnConnectorDragEnd(line, connEndEventnewEventArgs);
                }
                this.HitNodeConnector.IsDragConnectionOver = false;
                this.HitNodeConnector = null;
                //if (this.HitNodeConnector==null)
                //{
                //    pop.IsOpen = false;
                //}
                ConnDragEndRoutedEventArgs connEndnewEventArgs = new ConnDragEndRoutedEventArgs(line);
                dview.OnAfterConnectionCreate(line, connEndnewEventArgs);
            }
            if (this.dview.IsPageEditable && !this.dview.IsPanEnabled)
            {
                if (!((this.dview.IsDragging && !this.dview.IsDragged) || (this.dview.IsRotating && !this.dview.IsDragged)))
                {
                    foreach (object o in m_DelayStack)
                    {
                        if (o is string)
                        {
                            if (((string)o).Equals("Start"))
                            {
                                this.dview.tUndoStack.Push("Stop");
                            }
                            else if (((string)o).Equals("Stop"))
                            {
                                this.dview.tUndoStack.Push("Start");
                            }
                        }
                        else
                            this.dview.tUndoStack.Push(o);
                    }
                }
            }
            m_DelayStack.Clear();
            this.dview.IsDragged = false;
            this.dview.IsDragging = false;
            this.dview.IsRotating = false;
            this.startPoint = null;
            bool notselected = true;
            this.ReleaseMouseCapture();
            if (!this.dview.Ispositionchanged && this.IsGrouped)
            {
                if (this.dview.IsPageEditable)
                {
                    if (!this.dview.IsPanEnabled && !this.isdoubleclicked)
                    {
                        IDiagramPage mdiagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;
                        if (mdiagramPage != null)
                        {
                            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                            {
                                if (this.IsSelected)
                                {
                                    mdiagramPage.SelectionList.Remove(this);
                                }
                                else
                                {
                                    if (!this.IsSelected)
                                    {
                                        foreach (Node gnode in this.Groups)
                                        {
                                            if (!gnode.IsSelected)
                                            {
                                                notselected = true;
                                            }
                                            else
                                            {
                                                notselected = false;
                                                break;
                                            }
                                        }

                                      
                                            if (this.AllowSelect && notselected)
                                            {
                                                if (!this.IsGrouped)
                                                {
                                                    mdiagramPage.SelectionList.Add(this);
                                                }
                                                else
                                                {
                                                    mdiagramPage.SelectionList.Add(this.Groups[this.Groups.Count - 1]);
                                                }
                                            }
                                    }
                                    else if (this.AllowSelect && this.IsGrouped)
                                    {
                                        mdiagramPage.SelectionList.Add(this.Groups[this.Groups.Count - 1]);
                                    }
                                }
                            }
                            else
                            {
                                if (!this.IsSelected)
                                {
                                    foreach (Node gnode in this.Groups)
                                    {
                                        if (!gnode.IsSelected)
                                        {
                                            notselected = true;
                                        }
                                        else
                                        {
                                            notselected = false;
                                            break;
                                        }
                                    }

                                    if (this.AllowSelect && notselected)
                                    {
                                        if (!this.IsGrouped)
                                        {
                                            mdiagramPage.SelectionList.Select(this);
                                        }
                                        else
                                        {
                                            if ((this.Groups[this.Groups.Count - 1] as Group).AllowSelect)
                                            {
                                                mdiagramPage.SelectionList.Select(this.Groups[this.Groups.Count - 1]);
                                            }
                                        }
                                    }
                                    else if (this.AllowSelect && this.IsGrouped)
                                    {
                                        CollectionExt groupednodes = new CollectionExt();
                                        foreach (Group g in this.Groups)
                                        {
                                            groupednodes.Add(g);
                                        }

                                        groupednodes.Insert(0, this);
                                        foreach (Node gnode in groupednodes)
                                        {
                                            if (gnode.IsSelected)
                                            {
                                                if (groupednodes.Count > 1)
                                                {
                                                    int index = groupednodes.IndexOf(gnode);
                                                    if (index == 0)
                                                    {
                                                        mdiagramPage.SelectionList.Select(groupednodes[groupednodes.Count - 1]);
                                                    }
                                                    else if (!this.Isnodedragged)
                                                    {
                                                        mdiagramPage.SelectionList.Select(groupednodes[index - 1]);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (this.AllowSelect && this.IsGrouped && !this.Isnodedragged && !Isrotate)
                                {
                                    mdiagramPage.SelectionList.Select(this.Groups[this.Groups.Count - 1]);
                                }
                            }
                        }
                    }
                }
            }
            if (dview.IsPageEditable)
            {
                if (Isrotate)
                {
                    NodeRoutedEventArgs newEventArgs2 = new NodeRoutedEventArgs(this);
                    dview.OnNodeRotationChanged(this, newEventArgs2);
                    Isrotate = false;
                }
                if (Isnodedragged)
                {
                    RaiseNodeDragEndEvent();
                    dc.View._LineRoute = false;
                    Isnodedragged = false;
                }
            }
            if (!DiagramView.IsOtherEvent && !dview.EnableConnection)
            {
                RaiseClickEvent();
            }
            DiagramView.IsOtherEvent = false;
            if(this.dview.DrawingMode==DrawingMode.Default)
            this.dview.EnableConnection = false;
            this.dview.Ispositionchanged = false;
            this.isdoubleclicked = false;
            this.ResizeThisNode = false;
            Node.mouseup = true;
            this.dview.IsDragging = false;
            dview.Page.InvalidateMeasure();
            dview.ScrollGrid.InvalidateMeasure();
        }

        private Node HitNodeConnector;
        private Node fn;
        private Node fixedNodeConnection
        {
            get { return fn; }
            set { fn = value; }
        }
        private Node previousHitNode;

        private ConnectionPort HitPort; //To store port on which mouse over.
        private ConnectionPort previoushitport;// To store previous port, on which mouse overed.
        private ConnectionPort FixedPort;// To store port on which mouse clicked.

        internal bool HitTesting(Point hitPoint)
        {
            HitPort = null;
            IEnumerable<UIElement> hitObjectcoll = VisualTreeHelper.FindElementsInHostCoordinates(hitPoint, Application.Current.RootVisual);
            if (hitObjectcoll.Count() > 0)
            {
                foreach (UIElement hitObject in hitObjectcoll)
                {
                    if (hitObject is Node && (hitObject != this.fixedNodeConnection))
                    {
                        Node node = hitObject as Node;
                        this.HitNodeConnector = hitObject as Node;
                        this.previousHitNode = hitObject as Node;
                        this.previousHitNode.IsDragConnectionOver = true;
                        return true;
                    }
                    if (hitObject is ConnectionPort && (hitObject != this.FixedPort))
                    {
                        if ((hitObject as ConnectionPort).CenterPortReferenceNo == 0)
                        {
                            centerport = (hitObject as ConnectionPort);
                            centerport.IsDragOverPort = true;
                            HitPort = null;
                            previoushitport = null;
                        }
                        else
                        {
                            foreach (Node n in dc.Model.Nodes)
                            {
                                foreach (ConnectionPort port in n.Ports)
                                {
                                    if (port == hitObject as ConnectionPort)
                                    {
                                        HitPort = hitObject as ConnectionPort;
                                        this.HitNodeConnector = (hitObject as ConnectionPort).Node;
                                        previoushitport = hitObject as ConnectionPort;
                                        previoushitport.IsDragOverPort = true;

                                    }
                                }
                            }
                        }

                        try
                        {
                            foreach (Group g in ((hitObject as ConnectionPort).Node as Node).Groups)
                            {
                                // g.IsDragConnectionOver = false;
                            }
                        }
                        catch
                        {
                        }

                        this.HitNodeConnector = (hitObject as ConnectionPort).Node;
                        previousHitNode = (hitObject as ConnectionPort).Node;
                        return true;
                    }
                    else if (previoushitport != null)
                        previoushitport.IsDragOverPort = false;
                    if(centerport!=null)
                    centerport.IsDragOverPort = false;
                }
            }

            if (this.previousHitNode != null)
            {
                this.previousHitNode.IsDragConnectionOver = false;
            }
            centerport = null;
            this.HitNodeConnector = null;
            return false;
        }



        private StackExt<object> m_DelayStack = new StackExt<object>();

        private static T FindParent<T>(UIElement control, Resizer parent) where T : UIElement
        {

            UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
            if (p != null)
            {
                if (p is T && VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(p)).Equals(parent))
                    return p as T;
                else
                    return Node.FindParent<T>(p, parent);
            }
            return null;
        }

        internal bool m_portDelete = true;
        private void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dview.mallowDrawandDrag = false;
            dview.mnodeDownEvent = false;
            dview.nodedrag = true;
            (sender as Node).currentdragging = true;
            dview.nodragging = false;
            IDiagramPage mdiagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;
            if (this.IsDoubleClick(e.GetPosition(mdiagramPage as Panel)) && this.LabelVisibility == Visibility.Visible)
            {
                if (dview.IsPageEditable && IsLabelEditable)
                {
                    Nodelbl = nodelabel.Text;
                    NodeLabeledit();
                    this.nodelabel.Visibility = Visibility.Collapsed;
                    this.nodetext.Visibility = Visibility.Visible;
                    this.nodetext.Focus();
                    this.nodetext.SelectionBackground = new SolidColorBrush(Colors.Blue);
                    this.nodetext.SelectAll();
                    this.LostFocus += new RoutedEventHandler(Node_LostFocus);
                }

                if (!DiagramView.IsOtherEvent )
                {
                    isdoubleclicked = true;
                    RaiseDoubleClickEvent();
                    DiagramView.IsOtherEvent = true;
                }
              
            }
            else
            {
                if (Node.FindParent<Thumb>(e.OriginalSource as UIElement, Resizer) != null)
                {
                    this.dview.IsDragging = false;
                    this.resize = true;
                }
                else
                {
                    if (!this.dview.EnableConnection || this.dview.NodeMode==ConnectionMode.Move)
                    {
                        this.dview.IsRotating = false;
                        this.dview.IsDragging = true;
                        this.CaptureMouse();
                    }
                }

                if ((e.OriginalSource as System.Windows.Shapes.Path) != null && (e.OriginalSource as System.Windows.Shapes.Path).Name == "PART_RotatorPath")
                {
                    this.dview.IsDragging = false;
                    this.dview.IsRotating = true;
                    this.CaptureMouse();
                }

                this.startPoint = e.GetPosition(this.dc.View.Page);
                if (mdiagramPage != null)
                {
                    foreach (object ele in (mdiagramPage as DiagramPage).Children)
                    {
                        if (ele is Node)
                        {
                            Node n = ele as Node;
                            n.StartPointDragging = e.GetPosition(n);
                        }
                        else if (ele is LineConnector)
                        {
                            LineConnector lc = ele as LineConnector;
                            lc.m_TempStartPoint = new Point(e.GetPosition(lc).X - lc.PxStartPointPosition.X, e.GetPosition(lc).Y - lc.PxStartPointPosition.Y);
                            lc.m_TempEndPoint = new Point(e.GetPosition(lc).X - lc.PxEndPointPosition.X, e.GetPosition(lc).Y - lc.PxEndPointPosition.Y);

                            //if (lc.IntermediatePoints != null)
                            //    for (int i = 0; i < lc.IntermediatePoints.Count; i++)
                            //    {
                            //        //lc.m_TempInerPts[i] = new Point(lc.m_TempInerPts[i].X + dragdelta.X, lc.m_TempInerPts[i].Y + dragdelta.Y);
                            //        lc.IntermediatePoints[i] = new Point(e.GetPosition(lc).X - lc.IntermediatePoints[i].X, (e.GetPosition(lc).Y - lc.IntermediatePoints[i].Y));
                            //    }
                        }
                    }
                }
                if (this.dview.IsPageEditable && !this.IsGrouped)
                {
                    if (!this.dview.IsPanEnabled)
                    {
                        this.startPoint = e.GetPosition(mdiagramPage as Panel);
                        if (mdiagramPage != null)
                        {
                            if (this.dview.EnableConnection && this.dview.NodeMode == ConnectionMode.Connect)
                            {
                                this.CaptureMouse();
                                this.startPoint = e.GetPosition(mdiagramPage as Panel);
                                this.dview.IsDragging = false;
                                e.Handled = true;
                            }

                            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                            {
                                if (this.IsSelected&&(Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
                                {
                                    if (this.IsSelected && this.AddConnectionPortEnabled && m_portDelete&&!e.Handled)
                                    {
                                        ConnectionPort port = new ConnectionPort();
                                        Point portpoint = e.GetPosition(this);
                                        port.Left = portpoint.X - 2;
                                        port.Top = portpoint.Y - 2;
                                        this.Ports.Add(port);
                                        //port.m_justnow = true;
                                        port.Node = this;
                                        //portadded = true;
                                    }
                                    else
                                    {
                                        m_portDelete = true;
                                    }
                                }
                               else if (this.IsSelected)
                                {
                                    mdiagramPage.SelectionList.Remove(this);
                                }
                                else
                                {
                                    if (AllowSelect)
                                    {
                                        mdiagramPage.SelectionList.Add(this);
                                        if (this.IsSelected && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
                                        {
                                            if (this.IsSelected && this.AddConnectionPortEnabled && m_portDelete && !e.Handled)
                                            {
                                                ConnectionPort port = new ConnectionPort();
                                                Point portpoint = e.GetPosition(this);
                                                port.Left = portpoint.X - 2;
                                                port.Top = portpoint.Y - 2;
                                                this.Ports.Add(port);
                                                //port.m_justnow = true;
                                                port.Node = this;
                                                //portadded = true;
                                            }
                                            else
                                            {
                                                m_portDelete = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (!this.IsSelected)
                            {
                                if (mdiagramPage != null)
                                {
                                    if (this.dview.EnableConnection && this.dview.NodeMode == ConnectionMode.Connect)
                                    {
                                        this.CaptureMouse();
                                        this.startPoint = e.GetPosition(mdiagramPage as Panel);
                                        this.dview.IsDragging = false;
                                        e.Handled = true;
                                    }
                                }

                                if (this.AllowSelect)
                                {
                                    mdiagramPage.SelectionList.Select(this);
                                }
                            }
                        }


                    }
                }
                else
                    if (this.dview.IsPageEditable && this.IsGrouped)
                    {
                        if (!this.dview.IsPanEnabled)
                        {
                            mdiagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;
                            if (mdiagramPage != null)
                            {
                                if (this.dview.EnableConnection)
                                {
                                    this.CaptureMouse();
                                    this.startPoint = e.GetPosition(mdiagramPage as Panel);
                                    e.Handled = true;
                                }
                            }
                        }
                    }
                if (this.dview.IsPageEditable && !this.dview.IsPanEnabled)
                {
                    if (this.dview != null && this.dview.IsDragging)
                    {
                        if (this.dc.View.SelectionList.Count > 0)
                        {
                            if (dc.View.SelectionList.Count > 1 || dc.View.SelectionList[0] is Group)
                            {
                                m_DelayStack.Push("Stop");
                            }
                            ////
                            foreach (ICommon shape in this.dc.View.SelectionList)
                            {
                                if (shape is IShape)
                                {
                                    if (shape is Group)
                                    {
                                        foreach (ICommon node in (shape as Group).NodeChildren)
                                        {
                                            if (node is Node)
                                            {
                                                m_DelayStack.Push(new NodeOperation(NodeOperations.Dragged, node as Node));
                                            }
                                            else if (node is LineConnector)
                                            {
                                                m_DelayStack.Push(new LineOperation(LineOperations.Dragged, node as LineConnector));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        m_DelayStack.Push(new NodeOperation(NodeOperations.Dragged, shape as Node));
                                    }
                                }
                                else
                                    if (shape is IEdge)
                                    {
                                        m_DelayStack.Push(new LineOperation(LineOperations.Dragged, shape as LineConnector));
                                    }
                            }
                            /////
                            if (dc.View.SelectionList.Count > 1 || dc.View.SelectionList[0] is Group)
                            {
                                m_DelayStack.Push("Start");
                            }
                        }
                    }
                    else if (this.dview != null && this.dview.IsRotating)
                    {
                        if (this is Group)
                        {
                            m_DelayStack.Push("Stop");
                            m_DelayStack.Push(new NodeOperation(NodeOperations.Rotated, this));
                            foreach (ICommon node in (this as Group).NodeChildren)
                            {
                                if (node is Node)
                                {
                                    m_DelayStack.Push(new NodeOperation(NodeOperations.Rotated, node as Node));
                                }
                            }
                            m_DelayStack.Push("Start");
                        }
                        else
                        {
                            m_DelayStack.Push(new NodeOperation(NodeOperations.Rotated, this as Node));
                        }
                    }
                }
                this.lastNodeClick = DateTime.Now;
                this.lastNodePoint = e.GetPosition(mdiagramPage as Panel);
            }

        }

        void Node_LostFocus(object sender, RoutedEventArgs e)
        {
            this.nodetext.Focus();
            this.nodetext.SelectAll();
            this.nodetext.LostFocus += new RoutedEventHandler(nodetext_LostFocus);
        }



        void nodetext_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.LabelVisibility == Visibility.Visible)
            {
                this.nodetext.LostFocus -= new RoutedEventHandler(nodetext_LostFocus);
                this.nodelabel.Text = this.nodetext.Text;
                this.nodelabel.Visibility = Visibility.Visible;
                this.nodetext.Visibility = Visibility.Collapsed;
                this.LostFocus -= new RoutedEventHandler(Node_LostFocus);
            }

        }

        private void LineAdorner(MouseEventArgs e)
        {
            LineConnector line = new LineConnector(dview);
            if (this.dview.viewgrid.Children.ElementAt(this.dview.viewgrid.Children.Count() - 1) is Path)
            {
                this.dview.viewgrid.Children.Remove(this.dview.viewgrid.Children.ElementAt(this.dview.viewgrid.Children.Count() - 1));
            }

            //this.Cursor = Cursors.None;
            DoubleCollection dcoll = new DoubleCollection();
            dcoll.Add(3);
            dcoll.Add(3);
            this.fixedNodeConnection = this;
            foreach (ConnectionPort p in this.Ports)
            {
                if (p.IsMouseDown && (p.CenterPortReferenceNo != 0))
                {
                    this.FixedPort = p;
                    break;
                }
                else
                    this.FixedPort = null;
            }
            if (!beforeconncreate)
            {
                BeforeCreateConnectionRoutedEventArgs newEventArgs = new BeforeCreateConnectionRoutedEventArgs(line as LineConnector);
                dview.OnBeforeConnectionCreate(line as LineConnector, newEventArgs);
            }
            beforeconncreate = true;
            bool foundNode = this.HitTesting(e.GetPosition(Application.Current.RootVisual));
            Path path = new Path();
            path.Name = "LineAdorner";
            path.Stroke = new SolidColorBrush(Colors.Gray);
            path.StrokeThickness = 2;
            path.Data = this.UpdateConnectorAdornerPathGeometry(e.GetPosition(Application.Current.RootVisual), e);
            this.dview.viewgrid.Children.Add(path);
        }

        /// <summary>
        /// Provides class handling for the MouseMove routed event that occurs when the mouse 
        /// pointer  is over this control.
        /// </summary>
        /// <param name="e">The MouseEventArgs</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            IsMouseOver = true;
            if (this.IsLabelDragable && this.labelPressed && dragging)
            {
                return;
            }

            if (dview.IsPageEditable == false)
            {
                //this.Cursor = Cursors.Arrow;
            }
            m_MouseMoving = true;

            //if (!this.dview.IsPageEditable)
            //this.Cursor = Cursors.Arrow;
            ConnectionPort sourceHitPort = null;
            foreach (ConnectionPort port in this.Ports)
            {
                if (port.IsMouseDown)
                {
                    sourceHitPort = port;
                }
                if (port.IsMouseOver)
                {
                    sourceHitPort = port;
                }
            }

            if (sourceHitPort == null)
            {
                if (this.dview.IsPageEditable)
                {

                  
                    base.OnMouseMove(e);
                    if (this.startPoint.HasValue)
                    {
                        this.currentPosition = e.GetPosition(this.dc.View.Page);
                        if (dview.BoundaryConstraintsEnabled && !dview.ScrollGrid.DropAvailable(e.GetPosition(dview.Page)))
                        {

                        }
                        else
                        {
                            if (this.dview.IsDragging && this.AllowMove)
                            {
                                RaiseNodeDragStartEvent();
                                NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
                                dview.OnNodeDragging(this, newEventArgs);
                                if (newEventArgs.Cancel == true)
                                {
                                    this.DragCancel = true;
                                }
                                if (newEventArgs.Cancel == false && this.DragCancel == false)
                                {
                                    DiagramView.IsOtherEvent = true;
                                    if (this.dview.IsDragging)
                                    {
                                        Isnodedragged = true;
                                    }
                                    else
                                    {
                                        Isnodedragged = false;
                                    }
                                    Point delta = new Point(this.currentPosition.X - this.StartPointDragging.X, this.currentPosition.Y - this.StartPointDragging.Y);
                                    delta.X = delta.X - Node.Round(delta.X, dview.PxSnapOffsetX);
                                    delta.Y = delta.Y - Node.Round(delta.Y, dview.PxSnapOffsetY);
                                    if (!dview.SnapToHorizontalGrid)
                                    {
                                        delta.Y = 0;
                                    }
                                    if (!dview.SnapToVerticalGrid)
                                    {
                                        delta.X = 0;
                                    }
                                    this.dview.IsDragged = true;
                                    double diffx = this.currentPosition.X - this.startPoint.Value.X - delta.X;
                                    double diffy = this.currentPosition.Y - this.startPoint.Value.Y - delta.Y;
                                    bool isalreadygrouped = false;
                                  
                                    for (int i = 0; i < this.dc.View.SelectionList.Count; i++)
                                    {
                                        ICommon shape = this.dc.View.SelectionList[i] as ICommon;
                                        if (shape is IShape)
                                        {
                                            if (shape is Group)
                                            {
                                                (shape as Group).PxOffsetX += diffx;
                                                (shape as Group).PxOffsetY += diffy;
                                                (shape as Group).Arrange(new Rect((shape as Group).PxOffsetX, (shape as Group).PxOffsetY, (shape as Group).ActualWidth, this.ActualHeight));
                                                foreach (ICommon node in (shape as Group).NodeChildren)
                                                {

                                                    if (node is Node)
                                                    {
                                                        if ((node as Node).Groups.Count == 1)
                                                        {

                                                            (node as Node).PxOffsetX += diffx;
                                                            //(node as Node).PxOffsetX = this.currentPosition.X - (node as Node).StartPointDragging.X - delta.X;
                                                            (node as Node).PxOffsetY += diffy;
                                                            (node as Node).Arrange(new Rect((node as Node).PxOffsetX, (node as Node).PxOffsetY, (node as Node).ActualWidth, (node as Node).ActualHeight));
                                                            //(node as Node).PxOffsetY = this.currentPosition.Y - (node as Node).StartPointDragging.Y - delta.Y;
                                                            this.currentPosition.X = (node as Node).PxOffsetX;
                                                            this.currentPosition.Y = (node as Node).PxOffsetY;
                                                        }
                                                        else
                                                        {
                                                            if (isalreadygrouped == false)
                                                            {
                                                                (node as Node).PxOffsetX += diffx;
                                                                //(node as Node).PxOffsetX = this.currentPosition.X - (node as Node).StartPointDragging.X - delta.X;
                                                                (node as Node).PxOffsetY += diffy;
                                                                (node as Node).Arrange(new Rect((node as Node).PxOffsetX, (node as Node).PxOffsetY, (node as Node).ActualWidth, (node as Node).ActualHeight));
                                                                //(node as Node).PxOffsetY = this.currentPosition.Y - (node as Node).StartPointDragging.Y - delta.Y;
                                                                this.currentPosition.X = (node as Node).PxOffsetX;
                                                                this.currentPosition.Y = (node as Node).PxOffsetY;
                                                                isalreadygrouped = true;
                                                            }

                                                        }

                                                    }
                                                    else if (node is LineConnector)
                                                    {
                                                        (node as LineConnector).PxStartPointPosition = new Point((node as LineConnector).PxStartPointPosition.X + diffx, (node as LineConnector).PxStartPointPosition.Y + diffy);
                                                        //(node as LineConnector).PxStartPointPosition = new Point(this.currentPosition.X - (node as LineConnector).m_TempStartPoint.X - delta.X, this.currentPosition.Y - (node as LineConnector).m_TempStartPoint.Y - delta.Y);
                                                        (node as LineConnector).PxEndPointPosition = new Point((node as LineConnector).PxEndPointPosition.X + diffx, (node as LineConnector).PxEndPointPosition.Y + diffy);
                                                        //(node as LineConnector).PxEndPointPosition = new Point(this.currentPosition.X - (node as LineConnector).m_TempEndPoint.X - delta.X, this.currentPosition.Y - (node as LineConnector).m_TempEndPoint.Y - delta.Y);
                                                        if ((node as LineConnector).IntermediatePoints != null)
                                                        {
                                                            for (int j = 0; j < (node as LineConnector).IntermediatePoints.Count; j++)
                                                            {
                                                                (node as LineConnector).IntermediatePoints[j] = new Point((node as LineConnector).IntermediatePoints[j].X + diffx, (node as LineConnector).IntermediatePoints[j].Y + diffy);
                                                            }
                                                        }
                                                        (node as LineConnector).UpdateConnectorPathGeometry();
                                                    }

                                                }


                                            }
                                            else
                                            {
                                                if (
                                                        // Node is not Grouped or
                                                        ((shape as Node).IsGrouped != true) ||

                                                        // Node is Grouped and that group is not Selected
                                                        (
                                                            ((shape as Node).IsGrouped == true) &&
                                                            (shape as Node).Groups.Count>0 &&
                                                            !((shape as Node).Groups[0] as Group).IsSelected
                                                        )
                                                    )
                                                {
                                                    (shape as Node).PxOffsetX += diffx;
                                                    //(shape as Node).PxOffsetX = this.currentPosition.X - (shape as Node).StartPointDragging.X - delta.X;
                                                    (shape as Node).PxOffsetY += diffy;
                                                    (shape as Node).Arrange(new Rect((shape as Node).PxOffsetX, (shape as Node).PxOffsetY, (shape as Node).ActualWidth, (shape as Node).ActualHeight));
                                                    //(shape as Node).PxOffsetY = this.currentPosition.Y - (shape as Node).StartPointDragging.Y - delta.Y;
                                                    this.currentPosition.X = (shape as Node).PxOffsetX;
                                                    this.currentPosition.Y = (shape as Node).PxOffsetY;
                                                }
                                            }
                                        }

                                        else
                                            if (shape is IEdge)
                                            {

                                                (shape as LineConnector).PxStartPointPosition = new Point((shape as LineConnector).PxStartPointPosition.X + diffx, (shape as LineConnector).PxStartPointPosition.Y + diffy);
                                                //(shape as LineConnector).PxStartPointPosition = new Point(this.currentPosition.X - (shape as LineConnector).m_TempStartPoint.X - delta.X, this.currentPosition.Y - (shape as LineConnector).m_TempStartPoint.Y - delta.Y);
                                                (shape as LineConnector).PxEndPointPosition = new Point((shape as LineConnector).PxEndPointPosition.X + diffx, (shape as LineConnector).PxEndPointPosition.Y + diffy);
                                                //(shape as LineConnector).PxEndPointPosition = new Point(this.currentPosition.X - (shape as LineConnector).m_TempEndPoint.X - delta.X, this.currentPosition.Y - (shape as LineConnector).m_TempEndPoint.Y - delta.Y);
                                                if ((shape as LineConnector).IntermediatePoints != null)
                                                {
                                                    for (int j = 0; j < (shape as LineConnector).IntermediatePoints.Count; j++)
                                                    {
                                                        (shape as LineConnector).IntermediatePoints[j] = new Point((shape as LineConnector).IntermediatePoints[j].X + diffx, (shape as LineConnector).IntermediatePoints[j].Y + diffy);
                                                    }
                                                }
                                                (shape as LineConnector).UpdateConnectorPathGeometry();
                                            }

                                    }


                                    this.startPoint = new Point(this.startPoint.Value.X + diffx, this.startPoint.Value.Y + diffy);

                                    (this as Node).Arrange(new Rect(this.PxOffsetX, this.PxOffsetY, this.ActualWidth, this.ActualHeight));
                                    // (this.dc.View.Page as DiagramPage).InvalidateMeasure();

                                }

                                else if (this.DragCancel == true || newEventArgs.Cancel == true)
                                {
                                    foreach (ICommon n in dc.View.SelectionList.OfType<ICommon>())
                                    {
                                        if (n is Group)
                                        {
                                            (n as Group).OffsetX = (n as Group).x;
                                            (n as Group).OffsetY = (n as Group).y;
                                            foreach (INodeGroup n1 in (n as Group).NodeChildren)
                                            {
                                                if (n1 is Node)
                                                {
                                                    (n1 as Node).OffsetY = (n1 as Node).y;
                                                    (n1 as Node).OffsetX = (n1 as Node).x;
                                                    (n1 as Node).ReleaseMouseCapture();
                                                }
                                                else
                                                {
                                                    (n1 as LineConnector).StartPointPosition = (n1 as LineConnector).sp;
                                                    (n1 as LineConnector).EndPointPosition = (n1 as LineConnector).ep;
                                                    (n1 as LineConnector).ReleaseMouseCapture();
                                                    (n1 as LineConnector).UpdateConnectorPathGeometry();
                                                }
                                            }
                                            (n as Group).ReleaseMouseCapture();
                                        }
                                        if (n is Node)
                                        {
                                            (n as Node).OffsetY = (n as Node).y;
                                            (n as Node).OffsetX = (n as Node).x;
                                            (n as Node).ReleaseMouseCapture();
                                        }
                                        if (n is LineConnector)
                                        {
                                            (n as LineConnector).StartPointPosition = (n as LineConnector).sp;
                                            (n as LineConnector).EndPointPosition = (n as LineConnector).ep;
                                            (n as LineConnector).ReleaseMouseCapture();
                                            (n as LineConnector).UpdateConnectorPathGeometry();
                                        }
                                        Node.mouseup = true;
                                    }
                                    dview.ScrollGrid.InvalidateMeasure();
                                }
                            }
                            else
                            {
                                if (this.dview.IsRotating && this.AllowRotate)
                                {
                                    this.dview.IsDragged = true;
                                    Isrotate = true;
                                    DiagramView.IsOtherEvent = true;
                                    this.RenderTransformOrigin = new Point(.5, .5);
                                    this.rotateTransform = new RotateTransform();
                                    if (this.rotateTransform == null)
                                    {
                                        this.rotateTransform.Angle = 0;
                                    }
                                    else
                                    {
                                        double angle = this.findAngle(this.PxPosition, this.currentPosition);
                                        this.rotateTransform.Angle = angle;
                                        NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
                                        dview.OnNodeRotationChanging(this, newEventArgs);

                                    }


                                    if (this is Group)
                                    {
                                        foreach (ICommon n in (this as Group).NodeChildren)
                                        {
                                            if (n is Node)
                                            {
                                                (n as Node).RenderTransform = this.rotateTransform;
                                                (n as Node).RenderTransformOrigin = new Point(0.5, 0.5);
                                            }
                                        }

                                    }



                                    foreach (IEdge line in this.Edges)
                                    {
                                        (line as LineConnector).UpdateConnectorPathGeometry();
                                    }

                                    this.RenderTransform = this.rotateTransform;

                                }

                                else if (this.dview.EnableConnection)
                                {
                                    this.LineAdorner(e);
                                }
                                // (this.dc.View.Page as DiagramPage).InvalidateMeasure();
                            }
                        }

                        if (this.dc.View.IsPageEditable && this.AllowSelect && this.AllowMove)
                        {

                            //this.Cursor = Cursors.None;
                            //pop.IsOpen = true;
                        }
                        else
                        {
                            //this.Cursor = System.Windows.Input.Cursors.Arrow;
                        }
                    }

                }

            }

            else
            {
                if (sourceHitPort.IsMouseDown && AllowPortDrag)
                {
                    if (!this.dview.EnableConnection || this.dview.PortMode==ConnectionMode.Move)
                    {
                        if (e.GetPosition(this).X > 0 && e.GetPosition(this).X < (this.Width))
                        {
                            sourceHitPort.PxLeft = e.GetPosition(this).X;
                        }
                        if (e.GetPosition(this).Y > 0 && e.GetPosition(this).Y < (this.Height))
                        {
                            sourceHitPort.PxTop = e.GetPosition(this).Y;
                        }

                        foreach (IEdge line in this.Edges)
                        {
                            (line as LineConnector).UpdateConnectorPathGeometry();
                        }
                    }
                    else
                        this.LineAdorner(e);

                }
            }
            m_MouseMoving = false;
            if (dview.nodedrag)
            {
                dview.ScrollGrid.InvalidateMeasure();
            }
        }


        internal Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
            endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
            endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
            endpoint.X += originpoint.X;
            endpoint.Y += originpoint.Y;
            return endpoint;
        }

        private void ApplyShape()
        {
            //if (this.Shape != Shapes.Default && this.Shape != Shapes.CustomPath)
            //{
            //    string shapename = "PART_" + this.Shape.ToString();
            //    this.NodeShape.Style = this.noders[shapename] as Style;
            //}
            Node node = this;
            if (node.Shape != Shapes.CustomPath)
            {
                if (node.NodeShape != null)
                {
                    node.NodeShape.ClearValue(Path.StyleProperty);
                    string shapename = "PART_" + node.Shape.ToString();
                    if(shapename!="PART_Default")
                    node.NodeShape.Style = System.Windows.Markup.XamlReader.Load(noders[shapename]) as Style;
                }
            }
            if (node.Shape != Shapes.Default && node.NodeShape != null)
            {
                if (DependencyProperty.UnsetValue.Equals(node.ReadLocalValue(Node.CustomPathStyleProperty)) || this.CustomPathStyle == null)
                {
                    Binding PathBind = new Binding("NodePathFill");
                    PathBind.Source = node;
                    node.NodeShape.SetBinding(Path.FillProperty, PathBind);
                    PathBind = new Binding("NodePathStroke");
                    PathBind.Source = node;
                    node.NodeShape.SetBinding(Path.StrokeProperty, PathBind);
                    PathBind = new Binding("NodePathStrokeThickness");
                    PathBind.Source = node;
                    node.NodeShape.SetBinding(Path.StrokeThicknessProperty, PathBind);
                }
                else
                {
                    node.NodeShape.ClearValue(Path.FillProperty);
                    node.NodeShape.ClearValue(Path.StrokeProperty);
                    node.NodeShape.ClearValue(Path.StrokeThicknessProperty);
                    node.NodeShape.ClearValue(Path.StretchProperty);
                    Binding PathBind = new Binding("CustomPathStyle");
                    PathBind.Source = node;
                    node.NodeShape.SetBinding(Path.StyleProperty, PathBind);
                }
            }
        }


     
        private static void OnDuplicateWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node n = d as Node;
            double old = (double)e.OldValue;
            double angle = 0;
            if (n.RenderTransform != null && n.RenderTransform is RotateTransform)
            {
                angle = (n.RenderTransform as RotateTransform).Angle;
            }
            if (old != -1 && !double.IsNaN(old) && n.dc != null)
            {
                if (n.dc.View.m_IsCommandInProgress)
                {
                    NodeOperation oper = new NodeOperation(NodeOperations.Resized, d as Node);
                    oper.m_Size = new Size(old, (d as Node).Height);
                    n.dc.View.tUndoStack.Push(oper);
                }
            }
        }

        private static void OnDuplicateHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node n = d as Node;
            double old = (double)e.OldValue;
            double angle = 0;
            if (n.RenderTransform != null && n.RenderTransform is RotateTransform)
            {
                angle = (n.RenderTransform as RotateTransform).Angle;
            }
            if (old != -1 && !double.IsNaN(old) && n.dc != null)
            {
                if (n.dc.View.m_IsCommandInProgress)
                {
                    NodeOperation oper = new NodeOperation(NodeOperations.Resized, d as Node);
                    oper.m_Size = new Size((d as Node).Width, old);
                    n.dc.View.tUndoStack.Push(oper);
                }
            }
        }

        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node node = d as Node;
            Node.UpdateStyle(node);
        }

        private static void UpdateStyle(Node node)
        {
            if (node.NodeShape == null)
            {
                node.Dispatcher.BeginInvoke(delegate
                {
                    node.ApplyShape();
                }
            );
            }
            else
            {
                node.ApplyShape();
            }
        }

        /// <summary>
        /// Called when [units changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node n = d as Node;
            if (!n.IsFirstApplyTemplate)
            {
                if (n != null && n.m_IsPixelDefultUnit)
                {
                    n.OffsetX = MeasureUnitsConverter.Convert(n.OffsetX, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    n.OffsetY = MeasureUnitsConverter.Convert(n.OffsetY, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    n.Position = MeasureUnitsConverter.Convert(n.Position, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    foreach (ConnectionPort port in n.Ports)
                    {
                        port.Top = MeasureUnitsConverter.Convert(port.Top, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                        port.Left = MeasureUnitsConverter.Convert(port.Left, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                        //port.CenterPosition = MeasureUnitsConverter.Convert(port.CenterPosition, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                    }
                }
                if (!n.IsLoaded)
                {
                    n.Width = MeasureUnitsConverter.ToPixels(n.Width, (MeasureUnits)e.NewValue);
                    n.Height = MeasureUnitsConverter.ToPixels(n.Height, (MeasureUnits)e.NewValue);
                }
            }
            n.m_IsPixelDefultUnit = true;
        }

        /// <summary>
        /// Called when [IsDragConnectionOver changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsDragConnectionOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Node node = d as Node;
            //if ((bool)e.NewValue == true)
            //{
            //    Border outerborder = new Border();
            //    outerborder.Name = "PART_DragConnectionOver";
            //    outerborder.BorderThickness = new Thickness(2);
            //    outerborder.BorderBrush = new SolidColorBrush(Colors.Red);
            //    if (node.Nodepanel.FindName("PART_DragConnectionOver") == null)
            //    {
            //        node.Nodepanel.Children.Add(outerborder);
            //    }
            //}
            //else
            //{
            //    int count = node.Nodepanel.Children.Count();
            //    int i = count - 1;
            //    foreach (UIElement element in node.Nodepanel.Children)
            //    {
            //        if (element is Border && (element as Border).Name == "PART_DragConnectionOver")
            //        {
            //            i = node.Nodepanel.Children.IndexOf(element);
            //            break;
            //        }
            //    }

            //    node.Nodepanel.Children.Remove(node.Nodepanel.Children.ElementAt(i));
            //}
        }

        private static void OnAllowRotatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node node = d as Node;
            if ((node.GetTemplateChild("PART_Rotator") as Thumb) != null && node != null)
            {
                if (node != null && node.AllowRotate && node.IsSelected)
                {
                    (node.GetTemplateChild("PART_Rotator") as Thumb).Visibility = Visibility.Visible;
                }
                else
                {
                    (node.GetTemplateChild("PART_Rotator") as Thumb).Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Called when [IsSelected changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            Node node = d as Node;
            node.nodeSelection();
            //if (node.Nodepanel != null)
            //{
            //    if ((bool)e.NewValue == true)
            //    {
            //        Thumb outerborder = new Thumb();
            //        outerborder.Style = node.rs["RotateDecorator"] as Style;
            //        node.Nodepanel.Children.Add(outerborder);
            //    }
            //    else
            //    {
            //        int count = node.Nodepanel.Children.Count();
            //        int i = count - 1;
            //        foreach (UIElement element in node.Nodepanel.Children)
            //        {
            //            if (element is Thumb && (element as Thumb).Name == string.Empty)
            //            {
            //                i = node.Nodepanel.Children.IndexOf(element);
            //                break;
            //            }
            //        }

            //        node.Nodepanel.Children.Remove(node.Nodepanel.Children.ElementAt(i));
            //    }
            //}
        }

        /// <summary>
        /// Called when rotate angle is changed.
        /// </summary>
        /// <param name="d">The dependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRotateAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl dc = DiagramPage.GetDiagramControl(d as Node);
            Node node = d as Node;

            if (node.RotateAngle != double.NaN && node.AllowRotate)
            {
                RotateTransform rt = new RotateTransform();
                rt.Angle = node.RotateAngle;
                
                if (node is Group)
                {
                    CollectionExt gnode = new CollectionExt();
                    for (int i = 0; i < (node as Group).NodeChildren.Count; i++)
                    {
                        if (!((node as Group).NodeChildren[i] is LineConnector))
                        {
                            gnode.Add((node as Group).NodeChildren[i] as INodeGroup);
                        }
                    }
                    gnode.Add(node);
                    foreach (INodeGroup item in gnode)
                    {
                        if (!(item is LineConnector))
                        {
                            (item as Node).RenderTransform = rt;
                            (item as Node).RenderTransformOrigin = new Point(0.5, 0.5);
                        }
                       
                    }
                }
                else
                {
                  
                    node.RenderTransform = rt;
                    node.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                
            }

            if (dc != null && !dc.View.Undone && !dc.View.Redone && !dc.View.IsLayout)
            {
                NodeOperation oper = new NodeOperation(NodeOperations.Rotated, (d as Node));
                oper.m_Angle = (double)e.OldValue;
                dc.View.tUndoStack.Push(oper);
                //dc.View.UndoStack.Push(d as Node);
                //dc.View.UndoStack.Push(e.OldValue);
                //dc.View.UndoStack.Push(dc.View.NodeRotateCount);
                //dc.View.UndoStack.Push("Rotated");
            }
            //(dc.View as DiagramView).Focus();
        }

        /// <summary>
        /// Called when [logical offset X changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLogicalOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl dc = DiagramPage.GetDiagramControl(d as Node);
            Node n = d as Node;
            n.Dispatcher.BeginInvoke(new Action(n.Node_LayoutUpdated));
            if (dc != null)
            {
                if (!(dc.View.Page as DiagramPage).IsUnitChanged && !dc.View.IsResizedRedone)
                {
                    //   (d as Node).oldx = (double)e.OldValue + (dc.View.Page as DiagramPage).LeastX;// MeasureUnitsConverter.ToPixels((double)e.OldValue, (dc.View.Page as DiagramPage).MeasurementUnits) + (dc.View.Page as DiagramPage).LeastX;
                }

                if (dc.View.m_IsCommandInProgress && d is Node)
                {
                    NodeOperation oper = new NodeOperation(NodeOperations.Dragged, d as Node);
                    oper.m_Position = new Point((double)e.OldValue, (d as Node).PxOffsetY);
                    dc.View.tUndoStack.Push(oper);
                }

                //if (!dc.View.Redone && !dc.View.IsDragged && !dc.View.Undone && !dc.View.IsResized && !(d is Group) && !dc.View.IsMeasureCalled && !dc.View.IsLayout && !dc.View.IsMeasureCalled && !dc.View.IsLayout && !(dc.View.Page as DiagramPage).IsUnitChanged)

                if (dc.View.IsResized)
                {
                    if (!dc.View.Undone && !dc.View.Redone)
                    {

                        //dc.View.UndoStack.Push((double)e.OldValue + (dc.View.Page as DiagramPage).LeastX);// (MeasureUnitsConverter.ToPixels((double)e.OldValue, (dc.View.Page as DiagramPage).MeasurementUnits) + (dc.View.Page as DiagramPage).LeastX);
                        //dc.View.UndoStack.Push("offsetx");
                        //dc.View.UndoStack.Push(d as Node);
                        //dc.View.UndoStack.Push(dc.View.NodeDragCount);
                        //dc.View.UndoStack.Push("Dragged");
                    }
                }
            }
        }

        /// <summary>
        /// Called when [logical offset Y changed].
        /// </summary>
        /// <param name="d">The DependencyObject .</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLogicalOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl dc = DiagramPage.GetDiagramControl(d as Node);
            Node n = d as Node;
            n.Dispatcher.BeginInvoke(new Action(n.Node_LayoutUpdated));
            if (dc != null)
            {
                if (!(dc.View.Page as DiagramPage).IsUnitChanged && !dc.View.IsResizedRedone)
                {
                    (d as Node).oldy = (double)e.OldValue + (dc.View.Page as DiagramPage).LeastY;// MeasureUnitsConverter.ToPixels((double)e.OldValue, (dc.View.Page as DiagramPage).MeasurementUnits) + (dc.View.Page as DiagramPage).LeastY;
                }

                if (dc.View.m_IsCommandInProgress && d is Node)
                {
                    NodeOperation oper = new NodeOperation(NodeOperations.Dragged, d as Node);
                    oper.m_Position = new Point((d as Node).PxOffsetX, (double)e.OldValue);
                    dc.View.tUndoStack.Push(oper);
                }

                //if (!dc.View.Redone && !dc.View.IsDragged && !dc.View.Undone && !dc.View.IsResized && !(d is Group) && !dc.View.IsMeasureCalled && !dc.View.IsLayout && !(dc.View.Page as DiagramPage).IsUnitChanged)
                if (dc.View.IsResized)
                {
                    if (!dc.View.Undone && !dc.View.Redone)
                    {
                        dc.View.UndoStack.Push((double)e.OldValue + (dc.View.Page as DiagramPage).LeastY);// ((MeasureUnitsConverter.ToPixels((double)e.OldValue, (dc.View.Page as DiagramPage).MeasurementUnits) + (dc.View.Page as DiagramPage).LeastY));
                        dc.View.UndoStack.Push("offsety");
                        dc.View.UndoStack.Push(d as Node);
                        dc.View.UndoStack.Push(dc.View.NodeDragCount);
                        dc.View.UndoStack.Push("Dragged");
                    }
                }
            }
        }

        /// <summary>
        /// Called when [label changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string oldvalue = (string)e.OldValue;
            string newvalue = (string)e.NewValue;
            Node node = d as Node;
            if (node.dview != null)
            {
                LabelRoutedEventArgs newEventArgs = new LabelRoutedEventArgs(oldvalue, newvalue, node);
                node.dview.OnNodeLabelChanged(node, newEventArgs);

            }


        }

        private static void OnLabelHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node node = d as Node;
            node.startPosition = new Point(0, 0);
        }

        private static void OnLabelVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node node = d as Node;
            node.startPosition = new Point(0, 0);
        }

        private static void OnPortVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node node = d as Node;
        }
        private static void OnLabelAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node node = d as Node;
            if (node.nodelabel != null)
            {
                var rotate = new RotateTransform();
                rotate.Angle=node.LabelAngle;
                node.nodelayouttransformer.LayoutTransform = rotate;
            }
        }

        private static void OnLabelDisplacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node node = d as Node;
            if (node.nodelabel != null && !node.dragging)
            {
                var move = new TranslateTransform();
                move.X = node.LabelDisplacement.X;
                move.Y = node.LabelDisplacement.Y;
                node.nodelabel.RenderTransform = move;
                node.nodetext.RenderTransform = move;
            }
        }

        internal ConnectionPort centerport;

        /// <summary>
        /// Calls Node_LayoutUpdated method of the instance, notifies of the sender value changes.
        /// </summary>
        //public void Node_LayoutUpdated(object sender, EventArgs e)
        private void Node_LayoutUpdated()
        {
            if (this.Page != null)// && this.Page.Children.Contains(this))
            {
                this.PxPosition = new Point(this.PxOffsetX + (this.Width / 2), this.PxOffsetY + (this.Height / 2));
            }
        }

        /// <summary>
        /// Gets the diagram view.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The DiagramView instance.</returns>
        internal static DiagramView GetDiagramView(DependencyObject element)
        {
            while (element != null && !(element is DiagramView))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            return element as DiagramView;
        }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The panel instance</returns>
        private IDiagramPage GetPanel(DependencyObject element)
        {
            while (element != null && !(element is IDiagramPage))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            return element as IDiagramPage;
        }

        #endregion

        #region INodeGroup

        /// <summary>
        /// Gets or sets a value indicating whether this instance is grouped.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is grouped; otherwise, <c>false</c>.
        /// </value>
        public bool IsGrouped
        {
            get { return (bool)GetValue(IsGroupedProperty); }
            set { SetValue(IsGroupedProperty, value); }
        }

        #endregion

        #region ICommon Members

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        internal double _Width
        {
            get
            {
                if (this.IsLoaded)
                {
                    return this.ActualWidth;
                }
                else
                {
                    if (double.IsNaN(this.Width))
                    {
                        return 0;
                    }
                    else
                    {
                        return this.Width;
                    }
                }
            }
        }

        internal double _Height
        {
            get
            {
                if (this.IsLoaded)
                {
                    return this.ActualHeight;
                }
                else
                {
                    if (double.IsNaN(this.Height))
                    {
                        return 0;
                    }
                    else
                    {
                        return this.Height;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the old ZIndex value.
        /// </summary>
        /// <value>The old ZIndex value</value>
        public int OldZIndex
        {
            get { return this.moldindex; }
            set { this.moldindex = value; }
        }

        /// <summary>
        /// Gets or sets the new ZIndex value.
        /// </summary>
        /// <value>The new ZIndex value</value>
        public int NewZIndex
        {
            get { return this.mnewindex; }
            set { this.mnewindex = value; }
        }

        #endregion

        #region IShape Members

        /// <summary>
        /// Gets or sets the Rank to which the node belongs to.
        /// </summary>
        internal int Rank
        {
            get { return this.mrank; }
            set { this.mrank = value; }
        }

        /// <summary>
        /// Gets or sets the parent nodes  based on the connections in a hierarchical layout.
        /// </summary>
        public CollectionExt Parents
        {
            get
            {
                return this.m_InNeighbors;
            }

            set
            {
                this.m_InNeighbors = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed; otherwise, <c>false</c>.</value>
        public bool IsFixed
        {
            get { return this.mIsFixed; }
            set { this.mIsFixed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get { return this.mIsExpanded; }
            set { this.mIsExpanded = value; }
        }

        /// <summary>
        /// Gets the in-degree of the node, the number of edges for which this node
        /// is the target.
        /// </summary>
        /// <value>The in degree</value>
        public int InDegree
        {
            get { return this.InEdges.Count; }
        }

        /// <summary>
        /// Gets the out-degree of the node, the number of edges for which this node
        /// is the source.
        /// </summary>
        /// <value>The out degree.</value>
        public int OutDegree
        {
            get { return this.OutEdges.Count; }
        }

        /// <summary>
        /// Gets the degree of the node, the number of edges for which this node
        /// is either the source or the target.
        /// </summary>
        /// <value>The degree.</value>
        public int Degree
        {
            get { return this.Edges.Count; }
        }

        /// <summary>
        /// Gets the collection of all incoming edges, those for which this node
        /// is the target.
        /// </summary>
        /// <value>The In Edges .</value>
        public CollectionExt InEdges
        {
            get { return this.minEdges; }
        }

        /// <summary>
        /// Gets the collection of all outgoing edges, those for which this node
        /// is the source.
        /// </summary>
        /// <value>The OutEdges</value>
        public CollectionExt OutEdges
        {
            get { return this.moutEdges; }
        }

        /// <summary>
        /// Gets the collection of all incident edges, those for which this node
        /// is either the source or the target.
        /// </summary>
        /// <value>The Collection of Edges</value>
        public CollectionExt Edges
        {
            get
            {
                return this.medges;
            }
        }

        private CollectionExt m_InNeighbors = new CollectionExt();
        private CollectionExt m_OutNeighbors = new CollectionExt();
        private CollectionExt m_Neighbors = new CollectionExt();

        /// <summary>
        /// Gets the collection of all adjacent nodes connected to this node by an
        /// incoming edge (i.e., all nodes that "point" at this one).
        /// </summary>
        /// <value>Collection of inedges.</value>
        public CollectionExt InNeighbors
        {
            get
            {
                return m_InNeighbors;
            }
        }

        /// <summary>
        /// Gets the collection of adjacent nodes connected to this node by an
        /// outgoing edge (i.e., all nodes "pointed" to by this one).
        /// </summary>
        /// <value>Collection of out edges</value>
        public CollectionExt OutNeighbors
        {
            get
            {
                return m_OutNeighbors;
            }
        }

        /// <summary>
        /// Gets an iterator over all nodes connected to this node.
        /// </summary>
        /// <value></value>
        public CollectionExt Neighbors
        {
            get
            {
                return m_Neighbors;
            }
        }

        /// <summary>
        /// Gets or sets the parent of the entity
        /// </summary>
        /// <value></value>
        public IShape ParentNode
        {
            get
            {
                return this.mParentNode;
            }

            set
            {
                this.mParentNode = value;
            }
        }

        /// <summary>
        /// Gets or sets the edge between this node and its parent node in a tree
        /// structure.
        /// </summary>
        /// <value>The Parent edge</value>
        public IEdge ParentEdge
        {
            get
            {
                return this.mParentEdge;
            }

            set
            {
                this.mParentEdge = value;
            }
        }

        /// <summary>
        /// Gets or sets the tree depth of this node.
        /// <remarks>The root's tree depth is
        /// zero, and each level of the tree is one depth level greater.
        /// </remarks>
        /// </summary>
        /// <value>The depth value</value>
        /// <remarks>The root's tree depth is
        /// zero, and each level of the tree is one depth level greater.
        /// </remarks>
        public int Depth
        {
            get { return this.mDepth; }
            set { this.mDepth = value; }
        }

        /// <summary>
        /// Gets the number of tree children of this node.
        /// </summary>
        /// <value>The child count</value>
        public int ChildCount
        {
            get
            {
                return Children.Count;
            }
        }

        /// <summary>
        /// Gets this node's first tree child.
        /// </summary>
        /// <value>The first child</value>
        public IShape FirstChild
        {
            get
            {
                if (this.Children == null || this.Children.Count == 0)
                {
                    return null;
                }
                else
                {
                    return (IShape)Children[0];
                }
            }
        }

        /// <summary>
        /// Gets this node's last tree child.
        /// </summary>
        /// <value></value>
        public IShape LastChild
        {
            get
            {
                if (this.Children == null || this.Children.Count == 0)
                {
                    return null;
                }
                else
                {
                    return (IShape)Children[this.Children.Count - 1];
                }
            }
        }

        /// <summary>
        /// Gets this node's previous tree sibling.
        /// </summary>
        /// <value></value>
        public IShape PreviousSibling
        {
            get
            {
                if ((this as IShape).ParentNode == null)
                {
                    return null;
                }
                else
                {
                    CollectionExt ch = (this as IShape).ParentNode.Children;
                    int chi = ch.IndexOf(this as IShape);
                    if (chi < 0 || chi > ch.Count - 1)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    if (chi == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return (IShape)ch[chi - 1];
                    }
                }
            }
        }

        /// <summary>
        /// Gets this node's next tree sibling.
        /// </summary>
        /// <value></value>
        public IShape NextSibling
        {
            get
            {
                if ((this as IShape).ParentNode == null)
                {
                    return null;
                }
                else
                {
                    CollectionExt ch = (this as IShape).ParentNode.Children;
                    int chi = ch.IndexOf(this as IShape);
                    if (chi < 0 || chi > ch.Count - 1)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    if (chi == ch.Count - 1)
                    {
                        return null;
                    }
                    else
                    {
                        return (IShape)ch[chi + 1];
                    }
                }
            }
        }

        /// <summary>
        /// Gets the previous shape.
        /// </summary>
        /// <value>The previous shape.</value>
        public IShape PreviousShape
        {
            get
            {
                int ind = 0;
                if (this.Model != null)
                {
                    ind = this.Model.Nodes.IndexOf(this);
                }
                if (ind == 0)
                {
                    return null;
                }
                else
                {
                    return (IShape)this.Model.Nodes[ind - 1];
                }
            }
        }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row count.</value>
        public int Row
        {
            get { return this.row; }
            set { this.row = value; }
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column count.</value>
        public int Column
        {
            get { return this.col; }
            set { this.col = value; }
        }

        /// <summary>
        /// Gets or sets an iterator over this node's tree children.
        /// </summary>
        /// <value></value>
        public CollectionExt Children
        {
            get { return this.treeChildren; }
            set { this.treeChildren = value; }
        }

        /// <summary>
        /// Moves the node, the argument being the motion vector.
        /// </summary>
        /// <param name="p">The point p.</param>
        public void Move(Point p)
        {
        }

        /// <summary>
        /// Gets the unique identifier of this node.
        /// </summary>
        /// <value>The unique identifier value.</value>
        public Guid ID
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public DiagramModel Model
        {
            get { return this.model; }
            set { this.model = value; }
        }

        /// <summary>
        /// Gets the Node ID
        /// </summary>
        public Node NodeID
        {
            get { return this; }
        }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        /// <value>The parent ID.</value>
        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        /// <remarks>
        /// The full name is the name of the node concatenated with the names
        /// of all parent nodes.
        /// </remarks>
        public string FullName
        {
            get { return this.mfullName; }
            set { this.mfullName = value; }
        }

        internal double PxOffsetX
        {

            get
            {
                return MeasureUnitsConverter.ToPixels(OffsetX, MeasurementUnits);
            }
            set
            {
                if (this.dc != null && this.dc.View != null && ((this.dc.View.IsDragged && this.m_MouseMoving)))
                {
                    if (this.dview.SnapToVerticalGrid)
                    {
                        value = Node.Round(value, dc.View.PxSnapOffsetX);
                    }
                }
                OffsetX = MeasureUnitsConverter.FromPixels(value, MeasurementUnits);
            }
        }

        /// <summary>
        /// Gets or sets the logical offset X. Used for internal calculation.
        /// </summary>
        /// <value>The logical offset X.</value>
        public double OffsetX
        {
            get
            {
                return (double)GetValue(LogicalOffsetXProperty);
            }

            set
            {
                if (dc != null)
                {
                    if (dc.View.BoundaryConstraintsEnabled && dc.View.mactiveAreachanged)
                    {
                        if (value < dc.View.BoundaryConstraintsArea.Left)
                        {
                            value = dc.View.BoundaryConstraintsArea.Left;
                        }
                        else if (value + this.ActualWidth > dc.View.BoundaryConstraintsArea.Right)
                        {
                            value = dc.View.BoundaryConstraintsArea.Right - this.ActualWidth;
                        }

                    }
                }
                SetValue(LogicalOffsetXProperty, value);
            }
        }

        internal static double Round(double value, double cutoff)
        {
            return (Math.Floor(value / cutoff) + (value % cutoff > cutoff / 2 ? 1 : 0)) * cutoff;
        }


        internal double PxOffsetY
        {

            get
            {
                return MeasureUnitsConverter.ToPixels(OffsetY, MeasurementUnits);
            }
            set
            {
                if (this.dc != null && this.dc.View != null && ((this.dc.View.IsDragged && this.m_MouseMoving)))
                {
                    if (this.dview.SnapToHorizontalGrid)
                    {
                        value = Node.Round(value, dc.View.PxSnapOffsetY);
                    }
                }
                OffsetY = MeasureUnitsConverter.FromPixels(value, MeasurementUnits);
            }
        }

        /// <summary>
        /// Gets or sets the logical offset Y. Used for internal calculation.
        /// </summary>
        /// <value>The logical offset Y.</value>
        public double OffsetY
        {
            get
            {
                return (double)GetValue(LogicalOffsetYProperty);
            }

            set
            {
                if (dc != null)
                {
                    if (dc.View.BoundaryConstraintsEnabled && dc.View.mactiveAreachanged)
                    {
                        if (value < dview.BoundaryConstraintsArea.Top)
                        {
                            value = dview.BoundaryConstraintsArea.Top;
                        }
                        else if (value + this.ActualHeight > dview.BoundaryConstraintsArea.Bottom)
                        {
                            value = dview.BoundaryConstraintsArea.Bottom - this.ActualHeight;
                        }

                    }
                }
                SetValue(LogicalOffsetYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the old horizontal offset.
        /// </summary>
        /// <value>The old horizontal offset.</value>
        internal double Oldhoroff
        {
            get
            {
                return this.oldhoff;
            }

            set
            {
                this.oldhoff = value;
            }
        }

        /// <summary>
        /// Gets or sets the old vertical offset.
        /// </summary>
        /// <value>The old vertical offset.</value>
        internal double Oldveroff
        {
            get
            {
                return this.oldvoff;
            }

            set
            {
                this.oldvoff = value;
            }
        }

        /// <summary>
        /// Gets or sets the old offset position.
        /// </summary>
        /// <value>The old offset point.</value>
        internal Point OldOffset
        {
            get
            {
                return this.oldoff;
            }

            set
            {
                this.oldoff = value;
            }
        }

        private bool offsetxset = false;

        internal bool isoffsetxset
        {
            get { return this.offsetxset; }
            set { this.offsetxset = value; }
        }

        private bool offsetyset = false;

        internal bool isoffsetyset
        {
            get { return this.offsetyset; }
            set { this.offsetyset = value; }
        }

        /*
        /// <summary>
        /// Gets or sets the offset X.
        /// </summary>
        /// <value>The offset X.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// node.AllowRotate=true;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        //public double OffsetX
        //{
        //    get
        //    {
        //        if (this.dview == null)
        //        {
        //            this.dview = GetDiagramView(this);
        //        }

        //        if (this.dview != null)
        //        {
        //            if (this.dview.IsLinedragdelta)
        //            {
        //                double temphordrag = this.dview.Linehoffset - this.dview.Scrollviewer.HorizontalOffset;
        //                double oldhoroff = 0;

        //                double d = this.dview.OldHoroffset - this.dview.Scrollviewer.HorizontalOffset;
        //                if (d == 0)
        //                {
        //                    this.dview.HorThumbDragOffset = 0;
        //                }

        //                if (this.dview.IsScrollThumb || this.dview.IsMouseWheeled)
        //                {
        //                    (this.dview.Page as DiagramPage).Minleft = 0;
        //                    if (this.dview.CurrentZoom >= 1)
        //                    {
        //                        (this.dview.Page as DiagramPage).Dragleft += temphordrag / this.dview.CurrentZoom;
        //                    }
        //                    else
        //                    {
        //                        (this.dview.Page as DiagramPage).Dragleft += temphordrag * this.dview.CurrentZoom;
        //                    }

        //                    oldhoroff = 0;
        //                    if (!this.dview.IsExeOnce)
        //                    {
        //                        oldhoroff = temphordrag;
        //                    }

        //                    this.dview.IsExeOnce = true;
        //                    temphordrag = 0;
        //                }
        //                else
        //                {
        //                    {
        //                        if (this.dview.CurrentZoom >= 1)
        //                        {
        //                            (this.dview.Page as DiagramPage).Dragleft += temphordrag / this.dview.CurrentZoom;
        //                        }
        //                        else
        //                        {
        //                            (this.dview.Page as DiagramPage).Dragleft += temphordrag * this.dview.CurrentZoom;
        //                        }

        //                        temphordrag = 0;
        //                        oldhoroff = 0;
        //                    }
        //                }

        //                this.dview.IsLinedragdelta = false;
        //            }

        //            if (this.dview.IsScrollThumb || this.dview.IsMouseWheeled)
        //            {
        //                (this.dview.Page as DiagramPage).Minleft = 0;
        //            }
        //            else
        //            {
        //                if (this.dview.IsScrolledRight && !(this.dview.Page as DiagramPage).IsPositive && (this.dview.Page as DiagramPage).Minleft == 0)
        //                {
        //                }
        //                else
        //                {
        //                    if ((this.dview.Page as DiagramPage).GreaterThanZero && this.OffsetX >= 0)
        //                    {
        //                        (this.dview.Page as DiagramPage).Minleft = 0;
        //                    }

        //                    (this.dview.Page as DiagramPage).GreaterThanZero = false;
        //                }
        //            }

        //            if (!this.dview.IsMouseUponly)
        //            {
        //                double cloffx = (this.dview.Page as DiagramPage).Dragleft + (this.dview.Page as DiagramPage).Minleft;// MeasureUnitsConverter.FromPixels((this.dview.Page as DiagramPage).Dragleft + (this.dview.Page as DiagramPage).Minleft, this.MeasurementUnits);
        //               // this.loffx = this.OffsetX + cloffx;
        //            }
        //            else
        //            {
        //                double cloffx = (this.dview.Page as DiagramPage).Dragleft;// MeasureUnitsConverter.FromPixels((this.dview.Page as DiagramPage).Dragleft, this.MeasurementUnits);
        //                //this.loffx = this.OffsetX + cloffx;
        //            }
        //        }

        //        return this.OffsetX;
        //    }

        //    set
        //    {

        //        //this.loffx = value;

        //        //if (this.dview != null && this.OffsetX < 0 && this.loffx < 0)
        //        //{
        //        //    (this.dview.Page as DiagramPage).ManualMeasureCall = true;
        //        //}

        //      //  this.PxOffsetX = this.loffx;
        //        //if (this.loffx > 0 && this.dview != null)
        //        //{
        //        //    this.isoffsetxset = true;
        //        //}
        //    }
        //}
        */
        /// <summary>
        /// Gets or sets the offset Y.
        /// </summary>
        /// <value>The offset Y.</value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        ///namespace SilverlightApplication1
        /// {
        /// public partial class MainPage : UserControl
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public MainPage()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// Node n = new Node(Guid.NewGuid(), "Start");
        /// n.Shape = Shapes.FlowChart_Start;
        /// n.IsLabelEditable = true;
        /// n.Label = "Start";
        /// n.OffsetX = 150;
        /// n.OffsetY = 25;
        /// n.Width = 150;
        /// n.Height = 75;
        /// n.ToolTip="Start Node";
        /// node.AllowRotate=true;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>

        #endregion

        //double soffy1;
        Node temp = null; // To store the node on which connection started.
        /// <summary>
        /// updates connector path geometry
        /// </summary>
        /// <param name="position">line connector </param>
        /// <param name="e">MouseEventArgs</param>
        /// <returns>PathGeometry</returns>        
        internal PathGeometry UpdateConnectorAdornerPathGeometry(Point position, MouseEventArgs e)
        {
            LineConnector lineconnector = new LineConnector(dview);

            if ((this.dview.Page as DiagramPage).ConnectorType == ConnectorType.Bezier || (this.dview.Page as DiagramPage).ConnectorType == ConnectorType.Arc)
            {
                Rect sourceRect = new Rect();
                Rect rectTarget = new Rect();
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);
                double x1 = 0;
                double y1 = 0;
                double x2 = 0;
                double y2 = 0;
                bool isLeft = false;
                bool isRight = false;
                bool isTop = false;
                bool isBottom = false;
                bool tisLeft = false;
                bool tisRight = false;
                bool tisTop = false;
                bool tisBottom = false;
                NodeInfo source = new NodeInfo();
                //Point p = new Point(0, 0);
                PathGeometry pathgeometry = new PathGeometry();
                if (this.fixedNodeConnection != null)
                {
                    temp = this.fixedNodeConnection as Node;
                    source = this.fixedNodeConnection.GetInfo();
                    sourceRect = new Rect(
                                          source.Left,
                                          source.Top,
                                          source.Size.Width,
                                          source.Size.Height);
                    //p = position;// MeasureUnitsConverter.FromPixels(position, source.MeasurementUnit);
                    if (this.HitTesting(position))
                    {
                        if (this.HitNodeConnector != null)
                        {
                            NodeInfo target = this.HitNodeConnector.GetInfo();
                            double hitwidth = this.HitNodeConnector.Width;// MeasureUnitsConverter.FromPixels(this.HitNodeConnector.Width, target.MeasurementUnit);
                            double hitheight = this.HitNodeConnector.Height;// MeasureUnitsConverter.FromPixels(this.HitNodeConnector.Height, target.MeasurementUnit);
                            rectTarget = new Rect(this.HitNodeConnector.PxOffsetX, this.HitNodeConnector.PxOffsetY, hitwidth, hitheight);
                            if ((this.dview.Page as DiagramPage).ConnectorType == ConnectorType.Bezier)
                            {
                                ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, rectTarget, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                            }
                            else
                            {
                                ConnectorBase.GetArcLineIntersect(source, target, sourceRect, rectTarget, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, lineconnector.ArcDirection);
                            }
                            x1 = startPoint.X;
                            y1 = startPoint.Y;
                            x2 = endPoint.X;
                            y2 = endPoint.Y;
                        }
                        else
                        {
                            x2 = this.fixedNodeConnection.PxPosition.X;
                            y2 = this.fixedNodeConnection.PxPosition.Y;
                            x1 = e.GetPosition(this.dview.Page).X;
                            y1 = e.GetPosition(this.dview.Page).Y;
                        }
                    }
                    else
                    {
                        double twozero = 20;// MeasureUnitsConverter.FromPixels(20, DiagramPage.Munits);
                        double targetleft = e.GetPosition(this.dview.Page).X;// MeasureUnitsConverter.FromPixels(e.GetPosition(this.dview).X, DiagramPage.Munits);
                        double targettop = e.GetPosition(this.dview.Page).Y;// MeasureUnitsConverter.FromPixels(e.GetPosition(this.dview).Y, DiagramPage.Munits);
                        NodeInfo pointend = new NodeInfo();
                        Rect pointendRect = new Rect();
                        {
                            pointendRect = new Rect(
                                                  targetleft,
                                                  targettop,
                                                  twozero,
                                                  twozero);
                            pointend.Position = new Point(e.GetPosition(this.dview.Page).X, e.GetPosition(this.dview.Page).Y);

                            if (this.fixedNodeConnection != null)
                            {
                                temp = this.fixedNodeConnection as Node;
                                NodeInfo src = (this.fixedNodeConnection as Node).GetInfo();
                                Rect rectsrc = new Rect(
                                    src.Left,
                                    src.Top,
                                    src.Size.Width,
                                    src.Size.Height);
                                Point sp1 = new Point(src.Position.X, src.Position.Y);
                                if ((this.dview.Page as DiagramPage).ConnectorType == ConnectorType.Bezier)
                                {
                                    ConnectorBase.GetOrthogonalLineIntersect(src, pointend, rectsrc, pointendRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                                }
                                else
                                {
                                    ConnectorBase.GetArcLineIntersect(src, pointend, rectsrc, pointendRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, lineconnector.ArcDirection);
                                }
                            }
                        }
                    }

                    x1 = startPoint.X;
                    y1 = startPoint.Y;
                    x2 = endPoint.X;
                    y2 = endPoint.Y;

                    PathFigure pathfigure = new PathFigure();
                    double num = Math.Max((double)(Math.Abs((double)(x2 - x1)) / 2.0), (double)20.0);
                    foreach (ConnectionPort p1 in temp.Ports)
                    {
                        if (p1.IsMouseDown)
                        {
                            temp_port = p1;
                            break;
                        }
                        else
                            temp_port = null;
                    }

                    if (temp_port != null)
                    {
                        Point p_temp = new Point(temp_port.PagePosition.X + temp_port.Width / 2, temp_port.PagePosition.Y + temp_port.Height / 2);
                        x1 = p_temp.X;
                        y1 = p_temp.Y;
                    }
                    Point p_Etemp = e.GetPosition(this.dview.Page as DiagramPage);
                    x2 = p_Etemp.X;
                    y2 = p_Etemp.Y;
                    pathfigure.StartPoint = new Point(x1, y1);
                    if ((this.dview.Page as DiagramPage).ConnectorType == ConnectorType.Bezier)
                    {
                        BezierSegment segment = new BezierSegment();
                        if (isBottom)
                        {
                            segment = lineconnector.GetSegment(x1, y1 + num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                        }
                        else if (isTop)
                        {
                            segment = lineconnector.GetSegment(x1, y1 - num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                        }
                        else if (isRight)
                        {
                            segment = lineconnector.GetSegment(x1 + num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                        }
                        else
                        {
                            segment = lineconnector.GetSegment(x1 - num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                        }

                        pathfigure.Segments.Add(segment);
                    }
                    else
                    {
                        double x = Math.Pow((x1 - x2), 2);
                        double y = Math.Pow((y1 - y2), 2);
                        double d = Math.Sqrt(x + y);
                        Point s1 = new Point(x1, y1);
                        Point e1 = new Point(x2, y2);

                        double angle = ConnectorBase.FindAngle(s1, e1);

                        ArcSegment segment = new ArcSegment();
                        segment.Point = e1;
                        segment.Size = new Size(d / 2, lineconnector.ArcHeight);
                        segment.RotationAngle = angle;
                        segment.IsLargeArc = false;
                        segment.SweepDirection = lineconnector.ArcDirection;

                        pathfigure.Segments.Add(segment);
                    }
                    pathgeometry.Figures.Add(pathfigure);
                    return pathgeometry;
                }
            }
            else
            {
                if (this.fixedNodeConnection != null)
                {
                    temp = this.fixedNodeConnection as Node;
                    PathGeometry pathgeometry = new PathGeometry();
                    List<Point> ConnectionPoints = this.GetAdornerLinePoints(this.fixedNodeConnection.GetInfo(), position, e);
                    if (ConnectionPoints.Count > 0)
                    {
                        PathFigure pathfigure = new PathFigure();
                        pathfigure.StartPoint = ConnectionPoints[0];
                        PolyLineSegment polyline = new PolyLineSegment();
                        foreach (Point po in ConnectionPoints)
                        {
                            polyline.Points.Add(po);
                        }

                        pathfigure.Segments.Add(polyline);
                        pathgeometry.Figures.Add(pathfigure);
                    }

                    return pathgeometry;
                }
                else
                {
                    PathGeometry pathgeometry = new PathGeometry();
                    List<Point> ConnectionPoints = this.GetAdornerLinePoints(e);
                    if (ConnectionPoints.Count > 0)
                    {
                        PathFigure pathfigure = new PathFigure();
                        pathfigure.StartPoint = ConnectionPoints[0];
                        PolyLineSegment polyline = new PolyLineSegment();
                        foreach (Point po in ConnectionPoints)
                        {
                            polyline.Points.Add(po);
                        }

                        pathfigure.Segments.Add(polyline);
                        pathgeometry.Figures.Add(pathfigure);
                    }

                    return pathgeometry;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the line points when mouse event is raised.
        /// </summary>
        /// <param name="e">Mouse point</param>
        /// <returns>The collection of points</returns>
        internal List<Point> GetAdornerLinePoints(MouseEventArgs e)
        
        {
            LineConnector lineconnector = new LineConnector();
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(0, 0);
            List<Point> connectionPoints = new List<Point>();

            connectionPoints.Add(startPoint);
            connectionPoints.Add(endPoint);

            if (lineconnector.ConnectorType == ConnectorType.Orthogonal)
            {
                connectionPoints = this.AddPoints(connectionPoints);
            }

            return connectionPoints;
        }

        /// <summary>
        /// Adds points to the collection .
        /// </summary>
        /// <param name="linePoints">Collection of points.</param>
        /// <returns>The modified collection of points</returns>
        internal List<Point> AddPoints(List<Point> linePoints)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < linePoints.Count; i++)
            {
                points.Add(linePoints[i]);
            }

            points.Insert(1, new Point(points[1].X, points[0].Y));
            return points;
        }

        private ConnectionPort Stemp_port = null; // To Store port on which connection started.
        private ConnectionPort temp_port = null; // To Store port on which mouse over.
        /// <summary>
        /// Gets the line points when tail node is null.
        /// </summary>
        /// <param name="source">The source node</param>
        /// <param name="sinkPoint">sink point</param>
        /// <param name="e">mouse event position</param>
        /// <returns>The collection of points</returns>
        internal List<Point> GetAdornerLinePoints(NodeInfo source, Point sinkPoint, MouseEventArgs e)
        {
            bool b1 = false;
            bool b2 = false;
            bool b3 = false;
            bool b4 = false;
            bool b5 = false;
            bool b6 = false;
            bool b7 = false;
            bool b8 = false;
            Point p = new Point(0, 0);
            List<Point> linePoints = new List<Point>();
            Rect rectSource = new Rect(
                                 source.Left,
                                 source.Top,
                                 source.Size.Width,
                                 source.Size.Height);
            Point st = new Point(source.Position.X, source.Position.Y);
            temp_port = null;
            Point endPoint = sinkPoint;
            try
            {
                Point ep = endPoint;// MeasureUnitsConverter.FromPixels(endPoint, source.MeasurementUnit);
                if (!rectSource.Contains(ep))
                {
                    if ((this.dview.Page as DiagramPage).ConnectorType == ConnectorType.Orthogonal)
                    {
                        Point startPoint;

                        if (this.HitTesting(sinkPoint))
                        {
                            if (this.HitNodeConnector != null)
                            {
                                NodeInfo target = this.HitNodeConnector.GetInfo();
                                double hitwidth = this.HitNodeConnector.Width;// MeasureUnitsConverter.FromPixels(this.HitNodeConnector.Width, target.MeasurementUnit);
                                double hitheight = this.HitNodeConnector.Height;// MeasureUnitsConverter.FromPixels(this.HitNodeConnector.Height, target.MeasurementUnit);
                                Rect rectTarget = new Rect(this.HitNodeConnector.PxOffsetX, this.HitNodeConnector.PxOffsetY, hitwidth, hitheight);
                                ConnectorBase.GetOrthogonalLineIntersect(source, target, rectSource, rectTarget, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);

                                if (Stemp_port != null)
                                    linePoints.Add(new Point(Stemp_port.PagePosition.X + Stemp_port.Width / 2, Stemp_port.PagePosition.Y + Stemp_port.Height / 2));
                                else
                                    linePoints.Add(startPoint);
                               // linePoints.Add(endPoint);
                                if (HitPort != null)
                                    linePoints.Add(new Point(HitPort.PagePosition.X + HitPort.Width / 2, HitPort.PagePosition.Y + HitPort.Height / 2));
                                else 
                                linePoints.Add(e.GetPosition(this.dview.Page as DiagramPage));
                                linePoints = this.AddPoints(linePoints);
                                this.FindConnectionEnd(linePoints, startPoint, endPoint, b1, b2, b3, b4, b5, b6, b7, b8);
                            }
                        }
                        else
                        {
                            foreach (ConnectionPort p1 in temp.Ports)
                            {
                                if (p1.IsMouseDown)
                                {
                                    temp_port = p1;
                                    Stemp_port = p1;
                                    break;
                                }
                                else
                                {
                                    temp_port = null;
                                    Stemp_port = null;
                                }
                            }
                            Point s;
                            endPoint = e.GetPosition(this.dview.Page);
                            NodeInfo src = source;
                            Rect rectsrc = new Rect(
                                src.Left,
                                src.Top,
                                src.Size.Width,
                                src.Size.Height);
                            Point sp1 = new Point(src.Position.X, src.Position.Y);
                            s = ConnectorBase.GetLineIntersect(src, sp1, endPoint, rectsrc, out b1, out b2, out b3, out b4, (this.dview.Page as DiagramPage).ConnectorType);
                            if (s == new Point(0, 0))
                            {
                                s = endPoint;
                            }
                            if (temp_port != null)
                                s = new Point(temp_port.PagePosition.X + temp_port.Width / 2, temp_port.PagePosition.Y + temp_port.Height / 2);
                            else
                                Stemp_port = null;
                            linePoints.Add(s);
                            linePoints.Add(endPoint);
                            linePoints = this.AddPoints(linePoints);
                        }
                    }
                    else
                    {
                        if (this.HitTesting(sinkPoint))
                        {
                            if (this.HitNodeConnector != null)
                            {
                                Point s;
                                {
                                    NodeInfo src = source;
                                    Rect rectsrc = new Rect(
                                        src.Left,
                                        src.Top,
                                        src.Size.Width,
                                        src.Size.Height);
                                    Point sp1 = new Point(src.Position.X, src.Position.Y);
                                    s = ConnectorBase.GetLineIntersect(src, sp1, endPoint, rectsrc, out b1, out b2, out b3, out b4, (this.dview.Page as DiagramPage).ConnectorType);
                                    NodeInfo target = (this.HitNodeConnector as Node).GetInfo();
                                    Rect recttarget = new Rect(
                                        target.Left,
                                        target.Top,
                                        target.Size.Width,
                                        target.Size.Height);
                                    Point ep1 = new Point(target.Position.X, target.Position.Y);
                                    endPoint = ConnectorBase.GetLineIntersect(target, ep1, s, recttarget, out b1, out b2, out b3, out b4, (this.dview.Page as DiagramPage).ConnectorType);
                                }

                                if (Stemp_port != null)
                                    linePoints.Add(new Point(Stemp_port.PagePosition.X + Stemp_port.Width / 2, Stemp_port.PagePosition.Y + Stemp_port.Height / 2));
                                else if(s!=p)
                                    linePoints.Add(s);
                                if (HitPort != null)
                                    linePoints.Add(new Point(HitPort.PagePosition.X + HitPort.Width / 2, HitPort.PagePosition.Y + HitPort.Height / 2));
                                else
                                    linePoints.Add(e.GetPosition(this.dview.Page as DiagramPage));
                            }
                        }
                        else
                        {
                            foreach (ConnectionPort p1 in temp.Ports)
                            {
                                if (p1.IsMouseDown)
                                {
                                    temp_port = p1;
                                    Stemp_port = p1; 
                                    break;
                                }
                                else
                                {
                                    temp_port = null;
                                    Stemp_port = null;
                                }
                            }
                            Point s;
                            {
                                endPoint = e.GetPosition(this.dview.Page);
                                NodeInfo src = source;
                                Rect rectsrc = new Rect(
                                    src.Left,
                                    src.Top,
                                    src.Size.Width,
                                    src.Size.Height);
                                Point sp1 = new Point(src.Position.X, src.Position.Y);
                                s = ConnectorBase.GetLineIntersect(src, sp1, endPoint, rectsrc, out b1, out b2, out b3, out b4, (this.dview.Page as DiagramPage).ConnectorType);
                            }

                            if(temp_port!=null)
                            {
                                s = new Point(temp_port.PagePosition.X + temp_port.Width / 2, temp_port.PagePosition.Y + temp_port.Height / 2);
                                linePoints.Add(s);
                            }
                            else if (s != p)
                            {
                                linePoints.Add(s);
                            }
                            linePoints.Add(endPoint);
                        }
                    }
                }
            }
            catch
            {
            }

            return linePoints;
        }

        /// <summary>
        /// Makes the end connection to the respective node by finding the correct direction of the node.
        /// </summary>
        /// <param name="connectionPoints">Collection of points.</param>
        /// <param name="startPoint">The start point of the connector.</param>
        /// <param name="endPoint">The end point of the connector.</param>
        /// <param name="isTop">Flag indicating the top side of the source.</param>
        /// <param name="isBottom">Flag indicating the bottom side of the source.</param>
        /// <param name="isLeft">Flag indicating the left side of the source.</param>
        /// <param name="isRight">Flag indicating the right side of the source.</param>
        /// <param name="tisTop">Flag indicating the top side of the target.</param>
        /// <param name="tisBottom">Flag indicating the bottom side of the target.</param>
        /// <param name="tisLeft">Flag indicating the left side of the target.</param>
        /// <param name="tisRight">Flag indicating the right side of the target.</param>
        private void FindConnectionEnd(List<Point> connectionPoints, Point startPoint, Point endPoint, bool isTop, bool isBottom, bool isLeft, bool isRight, bool tisTop, bool tisBottom, bool tisLeft, bool tisRight)
        {
            Point startpoint = new Point(0, 0);
            Point endpoint = new Point(0, 0);
            if (isRight)
            {
                startpoint = new Point(startPoint.X - 10, startPoint.Y);
            }
            else if (isBottom)
            {
                startpoint = new Point(startPoint.X, startPoint.Y - 10);
            }
            else if (isLeft)
            {
                startpoint = new Point(startPoint.X + 10, startPoint.Y);
            }
            else if (isTop)
            {
                startpoint = new Point(startPoint.X, startPoint.Y + 10);
            }

            if (tisRight)
            {
                endpoint = new Point(endPoint.X - 10, endPoint.Y);
            }
            else if (tisBottom)
            {
                endpoint = new Point(endPoint.X, endPoint.Y - 10);
            }
            else if (tisLeft)
            {
                endpoint = new Point(endPoint.X + 10, endPoint.Y);
            }
            else if (tisTop)
            {
                endpoint = new Point(endPoint.X, endPoint.Y + 10);
            }
        }

        internal void SnapWidth(bool IsLeft)
        {
            if (this.dc != null && this.dc.View != null && this.dc.View.SnapToVerticalGrid && this.m_MouseResizing)
            {
                if (IsLeft)
                {
                    double del = this.PxOffsetX;
                    this.PxOffsetX = Node.Round(this.m_TempPosition.X, dc.View.PxSnapOffsetX);
                    del -= this.PxOffsetX;
                    this.Oldsize = new Size(this.Width, 0);
                    this.Width += del;

                }
                else
                {
                    if (Node.Round(this.PxOffsetX + this.m_TempSize.Width, dc.View.PxSnapOffsetX) - this.PxOffsetX >= 0)
                    {
                        this.Oldsize = new Size(this.Width, 0);
                        this.Width = Node.Round(this.PxOffsetX + this.m_TempSize.Width, dc.View.PxSnapOffsetX) - this.PxOffsetX;
                    }
                }
            }
            else
            {
                if (IsLeft)
                {
                    this.PxOffsetX = this.m_TempPosition.X;
                }

                this.Width = this.m_TempSize.Width;
                this.Oldsize = new Size(this.Width, 0);
            }
            this.Arrange(new Rect(this.PxOffsetX, this.PxOffsetY, this.Width, this.Height));
        }

        internal void SnapHeight(bool IsTop)
        {
            if (this.dc != null && this.dc.View != null && this.dc.View.SnapToHorizontalGrid && this.m_MouseResizing)
            {
                if (IsTop)
                {
                    double del = this.PxOffsetY;
                    this.PxOffsetY = Node.Round(this.m_TempPosition.Y, dc.View.PxSnapOffsetY);
                    del -= this.PxOffsetY;
                    this.Height += del;
                }
                else
                {
                    if (Node.Round(this.PxOffsetY + this.m_TempSize.Height, dc.View.PxSnapOffsetY) - this.PxOffsetY >= 0)
                    {
                        this.Height = Node.Round(this.PxOffsetY + this.m_TempSize.Height, dc.View.PxSnapOffsetY) - this.PxOffsetY;
                    }
                }
            }
            else
            {
                if (IsTop)
                {
                    this.PxOffsetY = this.m_TempPosition.Y;
                }
                //this.Height = Node.Round(this.m_TempSize.Height, (dc.View.Page as DiagramPage).PxGridHorizontalOffset);
                this.Height = this.m_TempSize.Height;
                //Debug.WriteLine(this.Height);
            }
            this.Arrange(new Rect(this.PxOffsetX, this.PxOffsetY, this.Width, this.Height));
        }


        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                if (this.LabelVisibility == Visibility.Visible)
                {
                    this.nodelabel.Text = this.nodetext.Text;
                    this.nodelabel.Visibility = Visibility.Visible;
                    this.nodetext.Visibility = Visibility.Collapsed;
                }
            }
        }

        internal string ContextMenu_Delete;
        internal string ContextMenu_Grouping;
        internal string ContextMenu_Grouping_Group;
        internal string ContextMenu_Grouping_Ungroup;
        internal string ContextMenu_Order;
        internal string ContextMenu_Order_BringForward;
        internal string ContextMenu_Order_BringToFront;
        internal string ContextMenu_Order_SendBackward;
        internal string ContextMenu_Order_SendToBack;

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsMouseOver = false;
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            IsMouseOver = false;
            if (dview != null && dview.IsPageEditable)
            {               
                TimeSpan diff = DateTime.Now.Subtract(dc.View.ScrollGrid.timeRightClick);
                int d = diff.CompareTo(new TimeSpan(0, 0, 0, 0, 500));
                if (d == 1)
                {
                    //First Time ContextMenu=null
                    if (this.ContextMenu == null && dview.NodeContextMenu == null && dview.ContextMenu==null)
                    {                       
                            if (nodecontextmenu == null)
                            {
                                {
                                    CreateContextMenu();
                                }
                                nodecontextmenu.OpenPopup(e.GetPosition(null));
                                ContextMenuControlService.SetContextMenuControl(this, nodecontextmenu);
                                setDisableOrEnable(Front, Forward, Backward, Back, Delete, Group);
                            }
                    }
                    //if NodeContextMenu has value
                    else if (dview.NodeContextMenu != null)
                    {
                        ContextMenuControlService.SetContextMenuControl(this, dview.NodeContextMenu);
                        dview.NodeContextMenu.Contextmenuassigned = true;
                        dview.NodeContextMenu.OpenPopup(e.GetPosition(null));
                                       
                    }
                    else
                    {
                        //Refresh reference  context menu.from the second tome click
                        if (this.ContextMenu != null)
                        {
                            if (isDefalutContextMenu())                            
                            {
                                ContextMenuControl nodecontextmenu = this.ContextMenu;
                                ContextMenuControlItem Order = nodecontextmenu.Items[0] as ContextMenuControlItem;
                                ContextMenuControlItem Grouping = nodecontextmenu.Items[1] as ContextMenuControlItem;
                                ContextMenuControlItem Delete = nodecontextmenu.Items[2] as ContextMenuControlItem;
                                Delete.Click -= new RoutedEventHandler(Delete_Click);
                                Delete.Click += new RoutedEventHandler(Delete_Click);

                                ContextMenuControlItem Front = Order.Items[0] as ContextMenuControlItem;
                                Front.Click -= new RoutedEventHandler(Front_Click);
                                Front.Click += new RoutedEventHandler(Front_Click);
                                ContextMenuControlItem Forward = Order.Items[1] as ContextMenuControlItem;
                                Forward.Click -= new RoutedEventHandler(Forward_Click);
                                Forward.Click += new RoutedEventHandler(Forward_Click);
                                ContextMenuControlItem Backward = Order.Items[2] as ContextMenuControlItem;
                                Backward.Click -= new RoutedEventHandler(Backward_Click);
                                Backward.Click += new RoutedEventHandler(Backward_Click);
                                ContextMenuControlItem Back = Order.Items[3] as ContextMenuControlItem;
                                Back.Click -= new RoutedEventHandler(Back_Click);
                                Back.Click += new RoutedEventHandler(Back_Click);
                                ContextMenuControlItem Group = Grouping.Items[0] as ContextMenuControlItem;
                                Group.Click -= new RoutedEventHandler(Group_Click);
                                Group.Click += new RoutedEventHandler(Group_Click);
                                ContextMenuControlItem Ungroup = Grouping.Items[1] as ContextMenuControlItem;
                                Ungroup.Click -= new RoutedEventHandler(Ungroup_Click);
                                Ungroup.Click += new RoutedEventHandler(Ungroup_Click);                               
                              
                                setDisableOrEnable(Front, Forward, Backward, Back, Delete, Group);
                            }
                            
                        }
                    }


                    base.OnMouseRightButtonUp(e);
                }

            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (dc.View.IsPageEditable && !dview.IsPanEnabled && this.AllowSelect)
            {

                IDiagramPage diagramPanel = VisualTreeHelper.GetParent(this) as IDiagramPage;
                if (diagramPanel != null)
                {
                    if (!this.IsGrouped)
                    {

                        if (!this.IsSelected)
                        {
                            diagramPanel.SelectionList.Clear();
                            diagramPanel.SelectionList.Add(this);
                        }
                    }
                    else
                    {
                        bool anyOneSelected = this.IsSelected;
                        if (!anyOneSelected)
                        {
                            foreach (Group g in this.Groups)
                            {
                                if (g.IsSelected == true)
                                {
                                    anyOneSelected = true;
                                }
                            }
                        }
                        if (!anyOneSelected)
                        {
                            diagramPanel.SelectionList.Clear();
                            diagramPanel.SelectionList.Add(this);
                        }
                    }
                }
            }

            base.OnMouseRightButtonDown(e);

        }


        //Checking for the customContextmenu
        private bool isDefalutContextMenu()
        {
            if (this.ContextMenu != null)
            {
                Populateheader();
                ContextMenuControl nodecontextmenu = this.ContextMenu;
                List<string> oldheader = new List<string>();
                //Populating with the customer ContextMenu
                for (int i = 0; i < nodecontextmenu.Items.Count; i++)
                {
                    if ((nodecontextmenu.Items[i] is ContextMenuControlItem) && (nodecontextmenu.Items[i] as ContextMenuControlItem).Header != null)
                    {
                        oldheader.Add((nodecontextmenu.Items[i] as ContextMenuControlItem).Header.ToString());
                    }
                }

                //Cheking for Default context is null and comparing the count of Customer ContextMenu and Default contextMenu 
                if (header != null && header.Count == this.ContextMenu.Items.Count)
                {
                    //Checking for the Header String
                    if (oldheader[0] == header[0])
                    {
                        if (oldheader[1] == header[1])
                        {
                            if (oldheader[2] == header[2])
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            return true;
        }

        //DefaultcontextMenu
        private void Populateheader()
        {

            if (header.Count == 0)
            {
                header.Add(ContextMenu_Order);
                header.Add(ContextMenu_Grouping);
                header.Add(ContextMenu_Delete);
            }
        }
        private void nodeSelection()
        {
            DiagramControl diagramCtrl = DiagramPage.GetDiagramControl(this);

            if (this.IsSelected)
            {
                if ((this.Page as DiagramPage) != null && (this.Page as DiagramPage).SelectionList != null)
                {
                    (this.Page as DiagramPage).SelectionList.Add(this);
                }
                if (this.Resizer != null)
                {
                    this.Resizer.Visibility = Visibility.Visible;
                }
                if ((this.GetTemplateChild("PART_Rotator") as Thumb) != null)
                {
                    if (this.AllowRotate)
                    {
                        (this.GetTemplateChild("PART_Rotator") as Thumb).Visibility = Visibility.Visible;
                    }
                }
                if (dview!=null && dview.FocusOnSelect)
                {
                    this.Focus();
                }
            }
            else if(!this.IsSelected)
            {
                
                    if ((this.Page as DiagramPage) != null && (this.Page as DiagramPage).SelectionList != null)
                    {
                        (this.Page as DiagramPage).SelectionList.Remove(this);
                        
                    }
               
                if (this.Resizer != null)
                {
                    this.Resizer.Visibility = Visibility.Collapsed;
                }
                if ((this.GetTemplateChild("PART_Rotator") as Thumb) != null)
                {
                    if (this.AllowRotate)
                    {
                        (this.GetTemplateChild("PART_Rotator") as Thumb).Visibility = Visibility.Collapsed;
                    }
                }
            }
            
            if (dview != null)
            {
                dview.Selectedelement = this;
            }
        }
        private void setDisableOrEnable(ContextMenuControlItem Front, ContextMenuControlItem Forward, ContextMenuControlItem Backward, ContextMenuControlItem Back, ContextMenuControlItem Delete, ContextMenuControlItem Group)
        {
            if (this.IsSelected)
            {
                Front.IsEnabled = true;
                Forward.IsEnabled = true;
                Backward.IsEnabled = true;
                Back.IsEnabled = true;
                Delete.IsEnabled = true;
            }
            else
            {
                Front.IsEnabled = false;
                Forward.IsEnabled = false;
                Backward.IsEnabled = false;
                Back.IsEnabled = false;
                Delete.IsEnabled = false;
            }

            if (this.IsGrouped)
            {
                foreach (Group g in this.Groups)
                {
                    if (g.IsSelected)
                    {
                        Delete.IsEnabled = true;
                    }
                }
            }

            if (dview != null && dview.SelectionList.Count > 1 && !this.IsGrouped)
            {
                Group.IsEnabled = true;
            }
            else
            {
                foreach (INodeGroup item in dview.SelectionList)
                {
                    if (item is Group)
                    {
                        foreach (INodeGroup n in (item as Group).NodeChildren)
                        {
                            foreach (INodeGroup node in dview.SelectionList)
                            {
                                if (!(item as Group).NodeChildren.Contains(node) && !(node is Group))
                                {
                                    Group.IsEnabled = true;
                                    break;
                                }
                                else
                                {
                                    Group.IsEnabled = false;
                                }
                            }
                        }
                    }
                }
            }

            if (dview.SelectionList.Count <= 1)
            {
                Group.IsEnabled = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the Delete Menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Delete_Click(object sender, RoutedEventArgs e)
        {
            dc.Delete.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the Ungroup Menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Ungroup_Click(object sender, RoutedEventArgs e)
        {
            dc.UnGroup.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the Group Menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Group_Click(object sender, RoutedEventArgs e)
        {
            dc.Group.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the Back Menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Back_Click(object sender, RoutedEventArgs e)
        {
            dc.SendToBack.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the Backward Menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Backward_Click(object sender, RoutedEventArgs e)
        {
            dc.SendBackward.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the Forward Menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Forward_Click(object sender, RoutedEventArgs e)
        {
            dc.BringForward.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the Front Menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Front_Click(object sender, RoutedEventArgs e)
        {
            dc.BringToFront.Execute(dc.View);
        }
        private bool NodeVirtual = true;

        internal bool currentdragging = false;

        public bool AllowVirtualization
        {
            get { return NodeVirtual; }
            set { NodeVirtual = value; }

        }
        bool load = false;
        internal bool IsInternallyLoaded
        {
            get { return load; }
            set { load = value; }
        }
    }




    #region struct
    /// <summary>
    /// Gives information about the node.
    /// </summary>
    public struct NodeInfo
    {
        /// <summary>
        /// Gets or sets the node left position.
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// Gets or sets the node top position.
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Gets or sets the node centre position.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        public MeasureUnits MeasurementUnit { get; set; }

        internal Point TopPoint
        {
            get
            {
                return new Point(Left + Size.Width / 2, Top);
            }
        }

        internal Point LeftPoint
        {
            get
            {
                return new Point(Left, Top + Size.Height / 2);
            }
        }

        internal Point RightPoint
        {
            get
            {
                return new Point(Left + Size.Width, Top + Size.Height / 2);
            }
        }

        internal Point BottomPoint
        {
            get
            {
                return new Point(Left + Size.Width / 2, Top + Size.Height);
            }
        }
    }
    #endregion
}
