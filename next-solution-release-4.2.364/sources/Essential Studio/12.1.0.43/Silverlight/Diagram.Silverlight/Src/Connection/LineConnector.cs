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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents the Connectors to be used for making connections between the nodes. 
    /// </summary>
    /// <remarks>
    /// Connectors are objects that are used to create a link between two nodes. The node where the connection starts is known as the head node. The node where the connection ends is known as the tail node.
    /// <para>Three types of connectors are provided :Orthogonal, Straight and Bezier.</para>
    /// </remarks>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DiagramModel"/> in C# and add nodes and connections.
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
    ///       //Specifies the node
    ///        Node n = new Node(Guid.NewGuid(), "Start");
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
    ///        ConnectionPort port = new ConnectionPort();
    ///        port.Node=n;
    ///        port.Left=75;
    ///        port.Top=10;
    ///        port.PortShape = PortShapes.Arrow;
    ///        port.PortStyle.Fill = Brushes.Transparent;
    ///        port.Height = 11;
    ///        port.Width = 11;
    ///        n.Ports.Add(port);
    ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
    ///         n1.Shape = Shapes.FlowChart_Process;
    ///         n1.IsLabelEditable = true;
    ///         n1.Label = "Alarm Rings";
    ///         n1.Level = 2;
    ///         n1.OffsetX = 150;
    ///         n1.OffsetY = 125;
    ///         n1.Width = 150;
    ///         n1.Height = 75;
    ///        Model.Nodes.Add(n1);
    ///        ConnectionPort port1 = new ConnectionPort();
    ///        port1.Node=n;
    ///        port1.Left=75;
    ///        port1.Top=50;
    ///        port1.PortShape = PortShapes.Arrow;
    ///        port1.PortStyle.Fill = Brushes.Transparent;
    ///        port1.Height = 11;
    ///        port1.Width = 11;
    ///        n1.Ports.Add(port1);
    ///         LineConnector o = new LineConnector();
    ///         o.ConnectorType = ConnectorType.Straight;
    ///         o.TailNode = n1;
    ///         o.HeadNode = n;
    ///         o.LabelHorizontalAlignment = HorizontalAlignment.Center;
    ///         o.LabelVerticalAlignment = HorizontalAlignment.Center;
    ///         o.Label="Syncfusion";
    ///         o.ConnectionHeadPort = port;
    ///         o.ConnectionTailPort = port1;
    ///         o.HeadDecoratorShape=DecoratorShape.Arrow;
    ///         o.TailDecoratorShape=DecoratorShape.Arrow;
    ///         Model.Connections.Add(o);
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    public class LineConnector : ConnectorBase, INodeGroup, ICommon
    {
        internal Thumb Vertex = new Thumb();
        internal List<Thumb> Vertexs = new List<Thumb>();
        internal int vertexIndex = -1;
        internal List<Point> InterPts = new List<Point>();
        private ItemsControl vertexItems;
        private Point tempst = new Point();
        private Point tempend = new Point();
        private bool foundport = false;
        internal Point m_TempStartPoint;
        internal Point m_TempEndPoint;
        internal DoubleCollection m_StrokeDashArray;
        private ContextMenuControl linecontextmenu;
        /// <summary>
        /// Identifies the ConnectorPathGeometry dependency property.
        /// </summary>
        public static readonly DependencyProperty ConnectorPathGeometryProperty = DependencyProperty.Register("ConnectorPathGeometry", typeof(PathGeometry), typeof(LineConnector), new PropertyMetadata(defaultCPG, new PropertyChangedCallback(OnConnectorPathGeometryChanged)));

        private static PathGeometry defaultCPG
        {
            get
            {
                PathGeometry pg = new PathGeometry();
                pg.Figures = new PathFigureCollection();
                PathFigure pf = new PathFigure();
                pf.StartPoint = new Point(0, 0);
                pf.Segments.Add(new LineSegment());
                pg.Figures.Add(pf);
                return pg;
            }
        }

        /// <summary>
        /// Identifies the HeadDecoratorAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadDecoratorAngleProperty = DependencyProperty.Register("HeadDecoratorAngle", typeof(double), typeof(LineConnector), new PropertyMetadata(0d, new PropertyChangedCallback(OnHeadDecoratorAngleChanged)));

        /// <summary>
        /// Identifies the HeadDecoratorGrid dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadDecoratorGridProperty = DependencyProperty.Register("HeadDecoratorGrid", typeof(Grid), typeof(LineConnector), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(LineConnector), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Identifies the TailDecoratorAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty TailDecoratorAngleProperty = DependencyProperty.Register("TailDecoratorAngle", typeof(double), typeof(LineConnector), new PropertyMetadata(0d, new PropertyChangedCallback(OnTailDecoratorAngleChanged)));

        /// <summary>
        /// Identifies the TailDecoratorGrid dependency property.
        /// </summary>
        public static readonly DependencyProperty TailDecoratorGridProperty = DependencyProperty.Register("TailDecoratorGrid", typeof(Grid), typeof(LineConnector), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ContextMenu dependency property
        /// </summary>
        public static readonly DependencyProperty ContextMenuProperty = DependencyProperty.Register("ContextMenu", typeof(ContextMenuControl), typeof(LineConnector), new PropertyMetadata(null, new PropertyChangedCallback(OnContextMenuChanged)));




        internal static readonly DependencyProperty LineAngleProperty = DependencyProperty.Register("LineAngle", typeof(double), typeof(LineConnector), new PropertyMetadata(0d, new PropertyChangedCallback(OnLineAngleChanged)));

        /// <summary>
        /// Used to store path head line
        /// </summary>
        private Path headpath;

        /// <summary>
        /// Used to store the DiagramControl instance
        /// </summary>
        internal DiagramControl dc;

        /// <summary>
        /// Used to store the view instance.
        /// </summary>
        private DiagramView dview;

        /// <summary>
        /// Represents the fixed node.
        /// </summary>
        private Node fixedNodeConnection;


        /// <summary>
        /// Represents the movable node.
        /// </summary>
        private Node movableNodeConnection;

        /// <summary>
        /// Used to store the head decorator shape for internal use.
        /// </summary>
        private DecoratorShape headshape = DecoratorShape.None;

        /// <summary>
        /// Used to store the head thumb boolean
        /// </summary>
        private bool headthumb = false;

        /// <summary>
        /// Represents the head thumb.
        /// </summary>
        private Thumb headThumb = null;

        /// <summary>
        /// Represents the tail thumb.
        /// </summary>
        private Thumb tailThumb = null;

        /// <summary>
        /// Used to store node that is connected
        /// </summary>
        private Node hitNodeConnector = null;

        /// <summary>
        /// Used to check if default value is used for label width.
        /// </summary>
        private bool isdefaulted = false;

        /// <summary>
        /// Used to check if mouse is double clicked.
        /// </summary>
        private bool isdoubleclicked = false;

        /// <summary>
        /// Used to get the line canvas
        /// </summary>
        private Canvas linecanvas;
        private Grid labelGrid;
        private TextBox linetext;
        private TextBlock linelabel;
        private bool IsLinedrag = false;

        bool load = false;
        internal bool IsInternallyLoaded
        {
            get { return load; }
            set { load = value; }
        }

        internal static bool linepopup = false;
        /// <summary>
        /// Used to store whether the line is dragging
        /// </summary>
        internal bool linedragging = false;

        /// <summary>
        /// Used to store boolean information if value is defaulted
        /// </summary>
        private bool misDefaulted = true;

        /// <summary>
        /// Used to store boolean information if node is hit.
        /// </summary>
        private bool misnodehit = false;

        /// <summary>
        /// Used to check if nodes are overlapped.
        /// </summary>
        private bool misoverlapped = false;

        /// <summary>
        /// Used to store the bend length.
        /// </summary>
        private double mbendLength = 10d;

        /// <summary>
        /// Used to store connection end space
        /// </summary>
        private double mconnectionEndSpace = 6d;

        /// <summary>
        /// Used to store previous hit node
        /// </summary>
        private Node previousHitNode = null;

        Random random = new Random();

        /// <summary>
        /// Used to store tail path
        /// </summary>
        private Path tailpath;

        /// <summary>
        /// Used to store the tail decorator shape for internal use.
        /// </summary>
        private DecoratorShape tailshape = DecoratorShape.None;

        /// <summary>
        /// Used to store tail thumb
        /// </summary>
        private bool tailthumb = false;


        /// <summary>
        /// Used to store the last node click instance
        /// </summary>
        private DateTime lastLineClick;

        /// <summary>
        /// Used to store the last node click point
        /// </summary>
        private Point lastLinePoint;

        private static bool mouseup;

        private static int n = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        public LineConnector()
            : base()
        {
            this.DefaultStyleKey = typeof(LineConnector);
            if (this.ReferenceID == null)
            {
                this.ID = Guid.NewGuid();
                m_ReferenceID = this.ID.ToString();
            }

            this.Loaded += new RoutedEventHandler(this.LineConnector_Loaded);
            this.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Lineconnector_MouseLeftButtonUp), true);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(this.LineConnector_MouseLeftButtonDown);
            this.MouseMove += new MouseEventHandler(this.LineConnector_MouseMove);
            if (this.IntermediatePoints == null)
            {
                IntermediatePoints = new List<Point>();
                IntermediatePoints.Add(new Point(0, 0));
                IntermediatePoints.Add(new Point(0, 0));
            }
            PathGeometry geom = new PathGeometry();
            geom.Figures = new PathFigureCollection();
            PathFigure pf = new PathFigure();
            pf.Segments.Add(new LineSegment());
            geom.Figures.Add(pf);
            ConnectorPathGeometry = geom;
        }

        internal void settempendPoints()
        {
            Point s = pos1; ;
            if (this.dc != null && this.dc.View != null)
            {
                if (dc.View.SnapToVerticalGrid)
                {
                    s.X = Node.Round(s.X, dc.View.PxSnapOffsetX);
                }
                if (dc.View.SnapToHorizontalGrid)
                {
                    s.Y = Node.Round(s.Y, dc.View.PxSnapOffsetY);
                }
            }

            bool b1, b2, b3, b4;
            if (this.TailNode != null)
            {
                NodeInfo target = (this.TailNode as Node).GetInfo();
                Rect recttarget = new Rect(
                    target.Left,
                    target.Top,
                    target.Size.Width,
                    target.Size.Height);
                Point ep1 = new Point(target.Position.X, target.Position.Y);
                tempend = ConnectorBase.GetLineIntersect(target, ep1, s, recttarget, out b1, out b2, out b3, out b4, this.ConnectorType);
                if (dc != null && dc.View != null && dc.View.Page != null)
                {
                    if (this.ConnectionTailPort != null)
                    {
                        double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                        double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                        tempend = new Point(ConnectionTailPort.PxLeft + target.Left + width, height + ConnectionTailPort.PxTop + target.Top);
                    }
                }
            }
        }

        internal void settempstPoints()
        {
            Point endPoint = pos1;
            if (this.dc != null && this.dc.View != null)
            {
                if (dc.View.SnapToVerticalGrid)
                {
                    endPoint.X = Node.Round(endPoint.X, dc.View.PxSnapOffsetX);
                }
                if (dc.View.SnapToHorizontalGrid)
                {
                    endPoint.Y = Node.Round(endPoint.Y, dc.View.PxSnapOffsetY);
                }
            }
            bool b1, b2, b3, b4;
            if (this.HeadNode != null)
            {
                NodeInfo src = (this.HeadNode as Node).GetInfo();
                Rect rectsrc = new Rect(
                    src.Left,
                    src.Top,
                    src.Size.Width,
                    src.Size.Height);
                Point sp1 = new Point(src.Position.X, src.Position.Y);
                tempst = ConnectorBase.GetLineIntersect(src, sp1, endPoint, rectsrc, out b1, out b2, out b3, out b4, this.ConnectorType);
                if (dc != null && dc.View != null && dc.View.Page != null)
                {
                    if (this.ConnectionHeadPort != null)
                    {
                        double width = double.IsNaN(ConnectionHeadPort.Width) ? 2.5 : ConnectionHeadPort.Width / 2;
                        double height = double.IsNaN(ConnectionHeadPort.Height) ? 2.5 : ConnectionHeadPort.Height / 2;

                        tempst = new Point(ConnectionHeadPort.PxLeft + src.Left + width, height + ConnectionHeadPort.PxTop + src.Top);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="sink">The sink node.</param>
        /// <param name="view">The view instance.</param>
        public LineConnector(Node source, Node sink, DiagramView view)
            : base()
        {
            this.dview = view;
            if (this.ReferenceID == null)
            {
                this.ID = Guid.NewGuid();
                m_ReferenceID = this.ID.ToString();
            }
            if (this.IntermediatePoints == null)
            {
                IntermediatePoints = new List<Point>();
                IntermediatePoints.Add(new Point(0, 0));
                IntermediatePoints.Add(new Point(0, 0));
            }
            this.ConnectorType = (this.dview.Page as DiagramPage).ConnectorType;
            this.HeadNode = source;
            this.TailNode = sink;
            this.Loaded += new RoutedEventHandler(this.LineConnector_Loaded);
            mstartpointposition.X = DropPoint.X - 25;
            mstartpointposition.Y = DropPoint.Y - 25;
            mendpointposition.X = DropPoint.X + 25;
            mendpointposition.Y = DropPoint.Y + 25;
            PathGeometry geom = new PathGeometry();
            geom.Figures = new PathFigureCollection();
            PathFigure pf = new PathFigure();
            pf.Segments.Add(new LineSegment());
            geom.Figures.Add(pf);
            ConnectorPathGeometry = geom;
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        /// <param name="view">The view instance.</param>
        public LineConnector(DiagramView view)
            : base()
        {
            this.DefaultStyleKey = typeof(LineConnector);
            this.dview = view;
            this.Loaded += new RoutedEventHandler(this.LineConnector_Loaded);

            if (this.ReferenceID == null)
            {
                this.ID = Guid.NewGuid();
                m_ReferenceID = this.ID.ToString();
            }
            if (this.IntermediatePoints == null)
            {
                IntermediatePoints = new List<Point>();
                IntermediatePoints.Add(new Point(0, 0));
                IntermediatePoints.Add(new Point(0, 0));
            }
            this.ConnectorType = (this.dview.Page as DiagramPage).ConnectorType;
            PathGeometry geom = new PathGeometry();
            geom.Figures = new PathFigureCollection();
            PathFigure pf = new PathFigure();
            pf.Segments.Add(new LineSegment());
            geom.Figures.Add(pf);
            ConnectorPathGeometry = geom;
        }

        internal double PxConnectionEndSpace
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(ConnectionEndSpace, MeasurementUnit);
            }
            set
            {
                ConnectionEndSpace = MeasureUnitsConverter.FromPixels(value, MeasurementUnit);
            }
        }

        /// <summary>
        /// Gets or sets the distance between the connector end position and the node.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Value indicating the distance.
        /// </value>
        /// <example>
        /// <code language="C#">
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
        ///       //Specifies the node
        ///        Node n = new Node(Guid.NewGuid(), "Start");
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
        ///         Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///         n1.Shape = Shapes.FlowChart_Process;
        ///         n1.IsLabelEditable = true;
        ///         n1.Label = "Alarm Rings";
        ///         n1.Level = 2;
        ///         n1.OffsetX = 150;
        ///         n1.OffsetY = 125;
        ///         n1.Width = 150;
        ///         n1.Height = 75;
        ///        Model.Nodes.Add(n1);
        ///         LineConnector o = new LineConnector();
        ///         o.ConnectorType = ConnectorType.Straight;
        ///         o.TailNode = n1;
        ///         o.HeadNode = n;
        ///         o.Label="Syncfusion";
        ///         o.ConnectionEndSpace= 6d; 
        ///         Model.Connections.Add(o);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <remarks>
        ///  Default value is 6. 
        ///  In case, if a decorator shape other than none is specified, 
        ///  a value >=6 should be given to make the connection start from the edge of the node,
        ///  or else the connector may cross the edge of the node .
        /// </remarks>
        public double ConnectionEndSpace
        {
            get
            {
                return this.mconnectionEndSpace;
            }

            set
            {
                this.mconnectionEndSpace = value;
                this.IsDefaulted = false;
            }
        }


        /// <summary>
        /// Gets or sets the PathGeometry of te connector.
        /// </summary>
        /// <value>
        /// Type: <see cref="Geometry"/>
        /// PathGeometry of the connector.
        /// </value>
        public PathGeometry ConnectorPathGeometry
        {
            get { return (PathGeometry)GetValue(ConnectorPathGeometryProperty); }
            set { SetValue(ConnectorPathGeometryProperty, value); }
        }

        /// <summary>
        /// Gets or sets the angle at which the head decorator is to be positioned.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Angle of the head decorator.
        /// </value>
        public double HeadDecoratorAngle
        {
            get
            {
                return (double)GetValue(HeadDecoratorAngleProperty);
            }

            set
            {
                SetValue(HeadDecoratorAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the line connector is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// </value>
        public new bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the angle at which the tail decorator is to be positioned.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Angle of the tail decorator.
        /// </value>
        public double TailDecoratorAngle
        {
            get
            {
                return (double)GetValue(TailDecoratorAngleProperty);
            }

            set
            {
                SetValue(TailDecoratorAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the line connector context menu.
        /// </summary>
        /// <value>The line connector context menu.</value>
        public ContextMenuControl ContextMenu
        {
            get
            {
                return (ContextMenuControl)GetValue(ContextMenuProperty);
            }
            set
            {
                SetValue(ContextMenuProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the bent line length which is used only for Orthogonal Line ConnectorType.
        /// </summary>
        /// <remarks>
        /// Default value is 10d.
        /// </remarks>
        internal double BendLength
        {
            get
            {
                return this.mbendLength;
            }

            set
            {
                this.mbendLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the head decorator grid.Used for internal assignments.
        /// </summary>
        /// <remarks>
        /// Default value is 10d.
        /// </remarks>
        internal Grid HeadDecoratorGrid
        {
            get
            {
                return (Grid)GetValue(HeadDecoratorGridProperty);
            }

            set
            {
                SetValue(HeadDecoratorGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the head path of the line.
        /// </summary>
        /// <remarks>
        /// Default path of the head.
        /// </remarks>
        internal Path HeadShape
        {
            get
            {
                return this.headpath;
            }

            set
            {
                this.headpath = value;
            }
        }

        /// <summary>
        /// Gets or sets the internal head decorator shape. Used for internal assignments in case of overlapped nodes.
        /// </summary>
        /// <value>The internal head decorator shape.</value>
        internal DecoratorShape InternalHeadShape
        {
            get { return this.headshape; }
            set {
                this.headshape = value;
                SetShape("Head");
            }
            
        }

        /// <summary>
        /// Gets or sets the internal tail decorator shape. Used for internal assignments in case of overlapped nodes.
        /// </summary>
        /// <value>The internal tail decorator shape.</value>
        internal DecoratorShape InternalTailShape
        {
            get { return this.tailshape; }
            set { 
                this.tailshape = value;
                SetShape("Tail");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is defaulted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is defaulted; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDefaulted
        {
            get
            {
                return this.misDefaulted;
            }

            set
            {
                this.misDefaulted = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LineConnector"/> is node is hit.
        /// </summary>
        /// <value><c>true</c> if node is hit; otherwise, <c>false</c>.</value>
        internal bool Isnodehit
        {
            get
            {
                return this.misnodehit;
            }

            set
            {
                this.misnodehit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the nodes have overlapped.
        /// </summary>
        /// <value>
        /// <c>true</c> if overlapped; otherwise, <c>false</c>.
        /// </value>
        internal bool IsOverlapped
        {
            get
            {
                return this.misoverlapped;
            }

            set
            {
                this.misoverlapped = value;
            }
        }

        /// <summary>
        /// Gets or sets the line canvas.Used for internal assignments.
        /// </summary>
        /// <remarks>
        /// Default value is 10d.
        /// </remarks>
        internal Canvas LineCanvas
        {
            get { return this.linecanvas; }
            set { this.linecanvas = value; }
        }

        internal double LineAngle
        {
            get
            {
                return (double)GetValue(LineAngleProperty);
            }

            set
            {
                SetValue(LineAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Tail decorator grid.Used for internal assignments.
        /// </summary>
        /// <remarks>
        /// Default value is 10d.
        /// </remarks>
        internal Grid TailDecoratorGrid
        {
            get
            {
                return (Grid)GetValue(TailDecoratorGridProperty);
            }

            set
            {
                SetValue(TailDecoratorGridProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the tail path of the line.
        /// </summary>
        /// <remarks>
        /// Default path of the tail.
        /// </remarks>
        internal Path TailShape
        {
            get
            {
                return this.tailpath;
            }

            set
            {
                this.tailpath = value;
            }
        }

        /// <summary>
        /// Adds points to the collection in case of orthogonal line .
        /// </summary>
        /// <param name="linePoints">Collection of points.</param>
        /// <returns>The modified collection of points</returns>
        private List<Point> AddLinePoints(List<Point> linePoints)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < linePoints.Count; i++)
            {
                points.Add(linePoints[i]);
            }

            if (!(Orientation == TreeOrientation.LeftRight || Orientation == TreeOrientation.RightLeft))
            {
                points.Insert(1, new Point(points[1].X, points[0].Y));
                return points;
            }
            else
            {
                points.Insert(1, new Point(points[0].X, points[1].Y));
                return points;
            }
        }

        /// <summary>
        /// Adds points to the collection in case of orthogonal line .
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

        /// <summary>
        /// Invoked when Label editing is complete
        /// </summary>
        internal void CompleteConnEditing()
        {
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
            if (HeadNode != null)
            {
                if (isRight)
                {
                    startpoint = new Point(startPoint.X - this.BendLength, startPoint.Y);
                }
                else if (isBottom)
                {
                    startpoint = new Point(startPoint.X, startPoint.Y - this.BendLength);
                }
                else if (isLeft)
                {
                    startpoint = new Point(startPoint.X + this.BendLength, startPoint.Y);
                }
                else if (isTop)
                {
                    startpoint = new Point(startPoint.X, startPoint.Y + this.BendLength);
                }
            }
            else
            {
                startpoint = startPoint;
            }

            if (TailNode != null)
            {
                if (tisRight)
                {
                    endpoint = new Point(endPoint.X - this.BendLength, endPoint.Y);
                }
                else if (tisBottom)
                {
                    endpoint = new Point(endPoint.X, endPoint.Y - this.BendLength);
                }
                else if (tisLeft)
                {
                    endpoint = new Point(endPoint.X + this.BendLength, endPoint.Y);
                }
                else if (tisTop)
                {
                    endpoint = new Point(endPoint.X, endPoint.Y + this.BendLength);
                }
            }
            else
            {
                endpoint = endPoint;
            }

            if (ConnectionHeadPort == null)
            {
                connectionPoints.Insert(0, startpoint);
            }

            if (ConnectionTailPort == null)
            {
                connectionPoints.Add(endpoint);
            }
        }

        /// <summary>
        /// Gets the line points when tail node is null.
        /// </summary>
        /// <param name="source">The source node</param>
        /// <param name="sinkPoint">sink point</param>
        /// <param name="e">mouse event position</param>
        /// <returns>The collection of points</returns>
        internal List<Point> GetAdornerLinePoints(NodeInfo source, Point sinkPoint, MouseEventArgs e)
        {
            LineConnector lineconnector = this;
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
            Point endPoint = sinkPoint;
            try
            {
                Point ep = endPoint;// MeasureUnitsConverter.FromPixels(endPoint, source.MeasurementUnit);
                if (!rectSource.Contains(ep))
                {
                    if (lineconnector.ConnectorType == ConnectorType.Orthogonal)
                    {
                        Point startPoint;

                        if (this.HitTesting(sinkPoint))
                        {
                            if (this.hitNodeConnector != null)
                            {
                                NodeInfo target = this.hitNodeConnector.GetInfo();
                                double hitwidth = this.hitNodeConnector.Width;// MeasureUnitsConverter.FromPixels(this.hitNodeConnector.Width, target.MeasurementUnit);
                                double hitheight = this.hitNodeConnector.Height;// MeasureUnitsConverter.FromPixels(this.hitNodeConnector.Height, target.MeasurementUnit);
                                Rect rectTarget = new Rect(this.hitNodeConnector.PxOffsetX, this.hitNodeConnector.PxOffsetY, hitwidth, hitheight);
                                if (this.headthumb)
                                {
                                    ConnectorBase.GetOrthogonalLineIntersect(target, source, rectTarget, rectSource, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                                }
                                else
                                {
                                    ConnectorBase.GetOrthogonalLineIntersect(source, target, rectSource, rectTarget, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                                }

                                linePoints.Add(startPoint);
                                linePoints.Add(endPoint);
                                linePoints = this.AddPoints(linePoints);
                                this.FindConnectionEnd(linePoints, startPoint, endPoint, b1, b2, b3, b4, b5, b6, b7, b8);
                            }
                        }
                        else
                        {
                            Point s;
                            if (!this.headthumb)
                            {
                                lineconnector.ConnectionTailPort = null;
                                endPoint = pos1;// e.GetPosition(this);
                                if ((lineconnector as LineConnector).HeadNode != null)
                                {
                                    NodeInfo src = ((lineconnector as LineConnector).HeadNode as Node).GetInfo();
                                    Rect rectsrc = new Rect(
                                        src.Left,
                                        src.Top,
                                        src.Size.Width,
                                        src.Size.Height);
                                    Point sp1 = new Point(src.Position.X, src.Position.Y);
                                    if (this.IntermediatePoints.Count > 0)
                                    {
                                        s = this.ConnectionPoints[ConnectionPoints.Count - 3];
                                    }
                                    else
                                    {
                                        s = ConnectorBase.GetLineIntersect(src, sp1, endPoint, rectsrc, out b1, out b2, out b3, out b4, lineconnector.ConnectorType);
                                    }
                                }
                                else
                                {
                                    s = (lineconnector as LineConnector).PxStartPointPosition;
                                    this.fixedNodeConnection = null;
                                }
                            }
                            else
                            {
                                lineconnector.ConnectionHeadPort = null;
                                s = pos1; // e.GetPosition(this);
                                if ((lineconnector as LineConnector).TailNode != null)
                                {
                                    NodeInfo target = ((lineconnector as LineConnector).TailNode as Node).GetInfo();
                                    Rect recttarget = new Rect(
                                        target.Left,
                                        target.Top,
                                        target.Size.Width,
                                        target.Size.Height);
                                    Point ep1 = new Point(target.Position.X, target.Position.Y);
                                    if (this.IntermediatePoints.Count > 0)
                                    {
                                        endPoint = this.ConnectionPoints[2];
                                    }
                                    else
                                    {
                                        endPoint = ConnectorBase.GetLineIntersect(target, ep1, s, recttarget, out b1, out b2, out b3, out b4, lineconnector.ConnectorType);
                                    }
                                }
                                else
                                {
                                    endPoint = (lineconnector as LineConnector).PxEndPointPosition;
                                    this.fixedNodeConnection = null;
                                }
                            }

                            linePoints.Add(s);
                            linePoints.Add(endPoint);
                            linePoints = this.AddPoints(linePoints);
                        }
                    }
                    else
                    {
                        Point s;
                        if (!this.headthumb)
                        {
                            if (!this.HitTesting(sinkPoint))
                            {
                                lineconnector.ConnectionTailPort = null;
                            }

                            endPoint = e.GetPosition(this);
                            if ((lineconnector as LineConnector).HeadNode != null)
                            {
                                NodeInfo src = ((lineconnector as LineConnector).HeadNode as Node).GetInfo();
                                Rect rectsrc = new Rect(
                                    src.Left,
                                    src.Top,
                                    src.Size.Width,
                                    src.Size.Height);
                                Point sp1 = new Point(src.Position.X, src.Position.Y);
                                if (this.IntermediatePoints.Count > 0)
                                {
                                    s = this.ConnectionPoints[ConnectionPoints.Count - 2];
                                }
                                else
                                {
                                    s = ConnectorBase.GetLineIntersect(src, sp1, endPoint, rectsrc, out b1, out b2, out b3, out b4, lineconnector.ConnectorType);
                                }
                            }
                            else
                            {
                                s = (lineconnector as LineConnector).PxStartPointPosition;
                            }
                        }
                        else
                        {
                            if (!this.HitTesting(sinkPoint))
                            {
                                lineconnector.ConnectionHeadPort = null;
                            }

                            s = e.GetPosition(this);
                            if ((lineconnector as LineConnector).TailNode != null)
                            {
                                NodeInfo target = ((lineconnector as LineConnector).TailNode as Node).GetInfo();
                                Rect recttarget = new Rect(
                                                            target.Left,
                                                            target.Top,
                                                            target.Size.Width,
                                                            target.Size.Height);
                                Point ep1 = new Point(target.Position.X, target.Position.Y);
                                if (this.IntermediatePoints.Count > 0)
                                {
                                    endPoint = this.ConnectionPoints[1];
                                }
                                else
                                {
                                    endPoint = ConnectorBase.GetLineIntersect(target, ep1, s, recttarget, out b1, out b2, out b3, out b4, lineconnector.ConnectorType);
                                }
                            }
                            else
                            {
                                endPoint = (lineconnector as LineConnector).PxEndPointPosition;
                            }
                        }

                        if (s != p)
                        {
                            linePoints.Add(s);
                        }

                        linePoints.Add(endPoint);
                    }
                }
            }
            catch
            {
            }

            return linePoints;
        }

        /// <summary>
        /// Gets the line points when mouse event is raised.
        /// </summary>
        /// <param name="e">Mouse point</param>
        /// <returns>The collection of points</returns>
        internal List<Point> GetAdornerLinePoints(MouseEventArgs e, DragDeltaEventArgs ed)
        {
            LineConnector lineconnector = this;

            Point pos = new Point();
            pos = pos1; // Get Mouse position from MouseMove()

            if (this.dc != null && this.dc.View != null)
            {
                if (dc.View.SnapToVerticalGrid)
                {
                    pos.X = Node.Round(pos.X, dc.View.PxSnapOffsetX);
                }
                if (dc.View.SnapToHorizontalGrid)
                {
                    pos.Y = Node.Round(pos.Y, dc.View.PxSnapOffsetY);
                }
            }

            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(0, 0);
            List<Point> connectionPoints = new List<Point>();

            if (lineconnector.ConnectorType == ConnectorType.Straight)
            {
                if (headthumb)
                {
                    startPoint = pos;// MeasureUnitsConverter.FromPixels(pos, (this.lineconnector as LineConnector).MeasurementUnit);
                    if (InterPts.Count != 0)
                        endPoint = InterPts[0];
                    else
                        endPoint = new Point(lineconnector.PxEndPointPosition.X, lineconnector.PxEndPointPosition.Y);
                }
                else if (tailthumb)
                {
                    if (InterPts.Count != 0)
                        startPoint = InterPts[InterPts.Count - 1];
                    else
                        startPoint = new Point(lineconnector.PxStartPointPosition.X, lineconnector.PxStartPointPosition.Y);
                    endPoint = pos;// MeasureUnitsConverter.FromPixels(pos, (this.lineconnector as LineConnector).MeasurementUnit);
                }
                else
                {
                    double hLeft = Canvas.GetLeft(this.HeadDecoratorGrid);// MeasureUnitsConverter.FromPixels(Canvas.GetLeft(headThumb), (this.lineconnector as LineConnector).MeasurementUnit);
                    double hTop = Canvas.GetTop(this.HeadDecoratorGrid);// MeasureUnitsConverter.FromPixels(Canvas.GetTop(headThumb), (this.lineconnector as LineConnector).MeasurementUnit);

                    double tLeft = Canvas.GetLeft(this.TailDecoratorGrid);// MeasureUnitsConverter.FromPixels(Canvas.GetLeft(tailThumb), (this.lineconnector as LineConnector).MeasurementUnit);
                    double tTop = Canvas.GetTop(this.TailDecoratorGrid);// MeasureUnitsConverter.FromPixels(Canvas.GetTop(tailThumb), (this.lineconnector as LineConnector).MeasurementUnit);
                    if (vertexIndex == 0)
                    {
                        startPoint = new Point(hLeft, hTop);
                        if (InterPts.Count == 1)
                            endPoint = new Point(tLeft, tTop);
                        else
                            endPoint = InterPts[vertexIndex + 1];
                    }
                    else if (vertexIndex == InterPts.Count - 1)
                    {
                        startPoint = InterPts[vertexIndex - 1];
                        endPoint = new Point(tLeft, tTop);
                    }
                    else
                    {
                        startPoint = InterPts[vertexIndex - 1];
                        endPoint = InterPts[vertexIndex + 1];
                    }
                }
                connectionPoints.Add(startPoint);
                if (!headthumb && !tailthumb)
                {
                    connectionPoints.Add(pos);//MeasureUnitsConverter.FromPixels(pos, (this.lineconnector as LineConnector).MeasurementUnit));
                }
                connectionPoints.Add(endPoint);
            }
            else if (lineconnector.ConnectorType == ConnectorType.Orthogonal)
            {
                settempendPoints();
                settempstPoints();
                Point tempEnd = (lineconnector as LineConnector).PxEndPointPosition;
                if (tempEnd.Equals(new Point(0, 0)))
                    tempEnd = tempend;
                Point tempStart = (lineconnector as LineConnector).PxStartPointPosition;
                if (tempStart.Equals(new Point(0, 0)))
                    tempStart = tempst;
                if (headthumb)
                {
                    Point mousePt = pos;// MeasureUnitsConverter.FromPixels(pos, (this.lineconnector as LineConnector).MeasurementUnit);
                    connectionPoints.Add(mousePt);
                    if (Math.Abs(InterPts[1].X - InterPts[0].X)
                    < Math.Abs(InterPts[1].Y - InterPts[0].Y))
                    {
                        connectionPoints.Add(new Point(InterPts[0].X, mousePt.Y));
                    }
                    else
                    {
                        connectionPoints.Add(new Point(mousePt.X, InterPts[0].Y));
                    }
                    if (InterPts.Count == 1)
                    {
                        connectionPoints.Add(tempEnd);
                    }
                    else { connectionPoints.Add(InterPts[1]); }
                }
                else if (tailthumb)
                {
                    Point mousePt = pos;// MeasureUnitsConverter.FromPixels(pos, (this.lineconnector as LineConnector).MeasurementUnit);
                    connectionPoints.Add(mousePt);
                    if (Math.Abs(InterPts[InterPts.Count - 2].X - InterPts[InterPts.Count - 1].X)
                    < Math.Abs(InterPts[InterPts.Count - 2].Y - InterPts[InterPts.Count - 1].Y))
                    {
                        connectionPoints.Add(new Point(InterPts[InterPts.Count - 1].X, mousePt.Y));
                    }
                    else
                    {
                        connectionPoints.Add(new Point(mousePt.X, InterPts[InterPts.Count - 1].Y));
                    }
                    if (InterPts.Count == 1)
                    {
                        connectionPoints.Add(tempStart);
                    }
                    else { connectionPoints.Add(InterPts[InterPts.Count - 2]); }

                }
                else if (vertexIndex == 0)
                {
                    {
                        Point tempPoint = new Point();

                        {
                            if (Math.Abs(InterPts[1].X - InterPts[0].X)
                                                  < Math.Abs(InterPts[1].Y - InterPts[0].Y))
                            {
                                tempPoint = new Point(Math.Min(InterPts[0].X, tempStart.X) + Math.Abs(InterPts[0].X - tempStart.X) / 2, InterPts[0].Y);
                            }
                            else
                            {
                                tempPoint = new Point(InterPts[0].X, Math.Min(InterPts[0].Y, tempStart.Y) + Math.Abs(InterPts[0].Y - tempStart.Y) / 2);
                            }
                            (lineconnector as LineConnector).InsertIntermediatePoint(tempPoint);
                            for (int i = 0; i < 2; i++)
                            {
                                int vcount = this.vertexItems.Items.Count + 1;
                                Point v = InterPts[i];
                                v = MeasureUnitsConverter.ToPixels(v, (lineconnector as LineConnector).MeasurementUnit);

                                Vertex = AddVertex(vcount);
                                Canvas.SetLeft(Vertex, v.X);
                                Canvas.SetTop(Vertex, v.Y);
                                this.vertexItems.Items.Add(Vertex);

                                Vertex.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
                                Vertex.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
                                Vertex.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
                                Vertex.MouseLeftButtonUp += new MouseButtonEventHandler(Thumb_MouseLeftButtonUp);
                                Vertex.MouseMove += new MouseEventHandler(Vertex_MouseMove);
                                Vertex.SizeChanged += new SizeChangedEventHandler(Vertex_SizeChanged);
                                Vertexs.Insert(i, Vertex);
                            }
                            vertexIndex = 2;
                            Vertex = Vertexs[2];
                        }
                    }
                }
                if (vertexIndex == InterPts.Count - 1)
                {
                    {
                        Point tempPoint = new Point();
                        int i = InterPts.Count - 1;
                        Point end = (lineconnector as LineConnector).GetLinePoints()[1];
                        if (Math.Abs(InterPts[i - 1].X - InterPts[i].X)
                        < Math.Abs(InterPts[i - 1].Y - InterPts[i].Y))
                        {
                            tempPoint = new Point(Math.Min(InterPts[i].X, end.X) + Math.Abs(InterPts[i].X - end.X) / 2, InterPts[i].Y);
                        }
                        else
                        {
                            tempPoint = new Point(InterPts[i].X, Math.Min(InterPts[i].Y, end.Y) + Math.Abs(InterPts[i].Y - end.Y) / 2);
                        }
                        (lineconnector as LineConnector).InsertIntermediatePoint(tempPoint);
                        for (int j = 0; j < 2; j++)
                        {
                            Point v = InterPts[InterPts.Count - 1 - j];
                            int vcount = this.vertexItems.Items.Count + 1;

                            Vertex = AddVertex(vcount);
                            Canvas.SetLeft(Vertex, v.X);
                            Canvas.SetTop(Vertex, v.Y);
                            this.vertexItems.Items.Add(Vertex);

                            Vertex.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
                            Vertex.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
                            Vertex.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
                            Vertex.MouseLeftButtonUp += new MouseButtonEventHandler(Thumb_MouseLeftButtonUp);
                            Vertex.MouseMove += new MouseEventHandler(Vertex_MouseMove);
                            Vertex.SizeChanged += new SizeChangedEventHandler(Vertex_SizeChanged);
                            Vertexs.Add(Vertex);
                        }
                        vertexIndex = InterPts.Count - 3;
                        Vertex = Vertexs[vertexIndex];
                    }
                }
                if (vertexIndex == InterPts.Count - 1)
                {
                    Point tempPoint = new Point();
                    int i = InterPts.Count - 1;
                    Point end = (lineconnector as LineConnector).GetLinePoints()[1];
                    if (Math.Abs(InterPts[i - 1].X - InterPts[i].X)
                    < Math.Abs(InterPts[i - 1].Y - InterPts[i].Y))
                    {
                        tempPoint = new Point(Math.Min(InterPts[i].X, end.X) + Math.Abs(InterPts[i].X - end.X) / 2, InterPts[i].Y);
                    }
                    else
                    {
                        tempPoint = new Point(InterPts[i].X, Math.Min(InterPts[i].Y, end.Y) + Math.Abs(InterPts[i].Y - end.Y) / 2);
                    }
                    (lineconnector as LineConnector).InsertIntermediatePoint(tempPoint);
                    for (int j = 0; j < 2; j++)
                    {
                        Point v = InterPts[InterPts.Count - 1 - j];
                        int vcount = this.vertexItems.Items.Count + 1;

                        Vertex = AddVertex(vcount);
                        Canvas.SetLeft(Vertex, v.X);
                        Canvas.SetTop(Vertex, v.Y);
                        this.vertexItems.Items.Add(Vertex);

                        Vertex.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
                        Vertex.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
                        Vertex.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
                        Vertex.MouseLeftButtonUp += new MouseButtonEventHandler(Thumb_MouseLeftButtonUp);
                        Vertex.MouseMove += new MouseEventHandler(Vertex_MouseMove);
                        Vertex.SizeChanged += new SizeChangedEventHandler(Vertex_SizeChanged);
                        Vertexs.Add(Vertex);
                    }
                    vertexIndex = InterPts.Count - 3;
                    Vertex = Vertexs[vertexIndex];
                }

                if (InterPts.Count >= 3 && !(tailthumb || headthumb))
                {
                    Point mousePt = pos;// MeasureUnitsConverter.FromPixels(pos, (this.lineconnector as LineConnector).MeasurementUnit);

                    double hLeft = Canvas.GetLeft(this.HeadDecoratorGrid);// MeasureUnitsConverter.FromPixels(Canvas.GetLeft(headThumb), (this.lineconnector as LineConnector).MeasurementUnit);
                    double hTop = Canvas.GetTop(this.HeadDecoratorGrid);// MeasureUnitsConverter.FromPixels(Canvas.GetTop(headThumb), (this.lineconnector as LineConnector).MeasurementUnit);

                    double tLeft = Canvas.GetLeft(this.TailDecoratorGrid);//.FromPixels(Canvas.GetLeft(tailThumb), (this.lineconnector as LineConnector).MeasurementUnit);
                    double tTop = Canvas.GetTop(this.TailDecoratorGrid);// MeasureUnitsConverter.FromPixels(Canvas.GetTop(tailThumb), (this.lineconnector as LineConnector).MeasurementUnit);

                    connectionPoints.Add(mousePt);
                    if (InterPts[vertexIndex - 1].X == InterPts[vertexIndex].X)
                    {
                        connectionPoints.Insert(0, new Point(mousePt.X, InterPts[vertexIndex - 1].Y));
                        connectionPoints.Add(new Point(InterPts[vertexIndex + 1].X, mousePt.Y));
                    }
                    else
                    {
                        connectionPoints.Insert(0, new Point(InterPts[vertexIndex - 1].X, mousePt.Y));
                        connectionPoints.Add(new Point(mousePt.X, InterPts[vertexIndex + 1].Y));
                    }
                    if (vertexIndex == 1)
                    {
                        connectionPoints.Insert(0, new Point(hLeft, hTop));
                    }
                    else
                    {
                        connectionPoints.Insert(0, InterPts[vertexIndex - 2]);
                    }
                    if (vertexIndex == InterPts.Count - 2)
                    {
                        connectionPoints.Add(new Point(tLeft, tTop));
                    }
                    else
                    {
                        connectionPoints.Add(InterPts[vertexIndex + 2]);
                    }
                }
            }

            return connectionPoints;
        }

        static float vertexcount = 1;

        private Thumb AddVertex(int vcount)
        {
            Thumb vertex = new Thumb();
            vertex.Name = "Vertex" + vertexcount++;
            vertex.Style = this.VertexStyle;
            return vertex;
        }

        /// <summary>
        /// Gets the line points when tail node is null.
        /// </summary>
        /// <param name="source">The source node</param>
        /// <returns>The collection of points</returns>
        internal List<Point> GetLinePoints(NodeInfo source)
        {
            double twozero = 20;// MeasureUnitsConverter.FromPixels(20, DiagramPage.Munits);
            double sourceleft = PxStartPointPosition.X;// MeasureUnitsConverter.FromPixels(StartPointPosition.X, DiagramPage.Munits);
            double sourcetop = PxStartPointPosition.Y;// MeasureUnitsConverter.FromPixels(StartPointPosition.Y, DiagramPage.Munits);
            double dropcentreX = DropPoint.X;// MeasureUnitsConverter.FromPixels(DropPoint.X, DiagramPage.Munits);
            double dropcentreY = DropPoint.Y;// MeasureUnitsConverter.FromPixels(DropPoint.Y, DiagramPage.Munits);
            double targetleft = PxEndPointPosition.X;// MeasureUnitsConverter.FromPixels(EndPointPosition.X, DiagramPage.Munits);
            double targettop = PxEndPointPosition.Y;// MeasureUnitsConverter.FromPixels(EndPointPosition.Y, DiagramPage.Munits);
            if (this.dview == null)
            {
                this.dview = Node.GetDiagramView(this);
            }

            double cs;
            double bl;
            double endspace;

            bl = this.BendLength;// MeasureUnitsConverter.FromPixels(this.BendLength, source.MeasurementUnit); 
            cs = this.PxConnectionEndSpace;// MeasureUnitsConverter.FromPixels(this.ConnectionEndSpace, source.MeasurementUnit); 
            endspace = cs;

            bool b1, b2, b3, b4, b5, b6, b7, b8;
            Point s = new Point(0, 0);
            Point e = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
            Rect sourceRect = new Rect();
            Rect targetRect = new Rect();
            NodeInfo target = new NodeInfo();
            target.Position = new Point(PxEndPointPosition.X - 25, PxEndPointPosition.Y - 25);
            targetRect = new Rect(
                                   targetleft,
                                   targettop,
                                    twozero,
                                    twozero);
            ConnectionPoints = new List<Point>();
            if (HeadNode != null)
            {
                sourceRect = new Rect(
                                    source.Left - endspace,
                                    source.Top - endspace,
                                    source.Size.Width + endspace,
                                    source.Size.Height + endspace);
                s = new Point(source.Position.X, source.Position.Y);
            }
            else
            {
                s = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                sourceRect = new Rect(
                                    sourceleft,
                                    sourcetop,
                                    twozero,
                                    twozero);
            }

            if (this.ConnectorType != ConnectorType.Orthogonal)
            {
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);

                if (ConnectionHeadPort != null)
                {
                    double width = double.IsNaN(ConnectionHeadPort.Width) ? 2.5 : ConnectionHeadPort.Width / 2;
                    double height = double.IsNaN(ConnectionHeadPort.Height) ? 2.5 : ConnectionHeadPort.Height / 2;

                    startPoint = new Point(ConnectionHeadPort.PxLeft + source.Left + width, height + ConnectionHeadPort.PxTop+ source.Top);
                    if ((this.HeadNode as Node).RenderTransform != null)
                    {
                        if ((this.HeadNode as Node).RenderTransform is RotateTransform)
                        {
                            startPoint = this.GeneralPointRotation(source.Position, startPoint, ((this.HeadNode as Node).RenderTransform as RotateTransform).Angle);
                        }

                    }
                }
                else
                {
                    if (InterPts.Count != 0)
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, InterPts[0], sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                    else
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, e, sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                }

                if (ConnectionTailPort == null)
                {
                    endPoint = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                }
                else
                {
                    endPoint = ConnectorBase.GetLineIntersect(source, s, e, sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    if (this.TailNode != null && (this.TailNode as Node).RenderTransform != null)
                    {
                        if ((this.TailNode as Node).RenderTransform is RotateTransform)
                        {
                            endPoint = this.GeneralPointRotation(target.Position, endPoint, ((this.TailNode as Node).RenderTransform as RotateTransform).Angle);
                        }

                    }
                }

                ConnectionPoints.Add(startPoint);
                ConnectionPoints.Add(endPoint);
            }
            else
            {
                Point startPoint, endPoint;

                if (!(Orientation == TreeOrientation.LeftRight || Orientation == TreeOrientation.RightLeft))
                {
                    ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, targetRect, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                }
                else
                {
                    ConnectorBase.GetTreeOrthogonalLineIntersect(source, target, sourceRect, targetRect, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                }

                if (ConnectionHeadPort != null)
                {

                    double width = double.IsNaN(ConnectionHeadPort.Width) ? 2.5 : ConnectionHeadPort.Width / 2;
                    double height = double.IsNaN(ConnectionHeadPort.Height) ? 2.5 : ConnectionHeadPort.Height / 2;

                    startPoint = new Point(ConnectionHeadPort.PxCenterPosition.X + source.Left + width, height + ConnectionHeadPort.PxCenterPosition.Y + source.Top);
                }
                else
                {
                    if (InterPts.Count != 0)
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, InterPts[0], sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                    else
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, e, sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                }



                if (this.HeadDecoratorGrid != null)
                {
                    if (b5)
                    {
                        (this as LineConnector).HeadDecoratorAngle = -90;
                    }
                    else if (b6)
                    {
                        (this as LineConnector).HeadDecoratorAngle = 90;
                    }
                    else if (b7)
                    {
                        (this as LineConnector).HeadDecoratorAngle = -180;
                    }
                    else if (b8)
                    {
                        (this as LineConnector).HeadDecoratorAngle = 0;
                    }
                }

                if (this.TailDecoratorGrid != null)
                {
                    if (b1)
                    {
                        (this as LineConnector).TailDecoratorAngle = -90;
                    }
                    else if (b2)
                    {
                        (this as LineConnector).TailDecoratorAngle = 90;
                    }
                    else if (b3)
                    {
                        (this as LineConnector).TailDecoratorAngle = -180;
                    }
                    else if (b4)
                    {
                        (this as LineConnector).TailDecoratorAngle = 0;
                    }
                }

                ConnectionPoints.Add(startPoint);
                endPoint = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                ConnectionPoints.Add(endPoint);
            }

            int count = ConnectionPoints.Count();
            double x = Math.Pow((ConnectionPoints[0].X - ConnectionPoints[count - 1].X), 2);
            double y = Math.Pow((ConnectionPoints[0].Y - ConnectionPoints[count - 1].Y), 2);
            Distance = Math.Sqrt(x + y);
            return ConnectionPoints;
        }

        /// <summary>
        /// Calculates the points which form the path geometry. 
        /// </summary>
        /// <param name="source">The head node</param>
        /// <param name="target">The tail node</param>
        /// <returns>Collection of points.</returns>
        internal List<Point> GetLinePoints(NodeInfo source, NodeInfo target)
        {
            double twozero = 20;// MeasureUnitsConverter.FromPixels(20, DiagramPage.Munits);
            double sourceleft = PxStartPointPosition.X;// MeasureUnitsConverter.FromPixels(StartPointPosition.X, DiagramPage.Munits);
            double sourcetop = PxStartPointPosition.Y;// MeasureUnitsConverter.FromPixels(StartPointPosition.Y, DiagramPage.Munits);
            double dropcentreX = DropPoint.X;// MeasureUnitsConverter.FromPixels(DropPoint.X, DiagramPage.Munits);
            double dropcentreY = DropPoint.Y;// MeasureUnitsConverter.FromPixels(DropPoint.Y, DiagramPage.Munits);
            if (this.dview == null)
            {
                this.dview = Node.GetDiagramView(this);
            }

            double cs;
            double bl;
            double endspace;

            bl = this.BendLength;// MeasureUnitsConverter.FromPixels(this.BendLength, source.MeasurementUnit);          
            cs = this.PxConnectionEndSpace;// MeasureUnitsConverter.FromPixels(this.ConnectionEndSpace, source.MeasurementUnit);
            endspace = cs;

            bool b1, b2, b3, b4, b5, b6, b7, b8;
            Point s = new Point(0, 0);
            Point e = new Point(0, 0);
            Rect sourceRect = new Rect();
            Rect targetRect = new Rect();
            List<Point> connectionPoints = new List<Point>();

            if (HeadNode != null)
            {
                if (this.ConnectionHeadPort == null)
                {
                    sourceRect = new Rect(
                                        source.Left - endspace,
                                        source.Top - endspace,
                                        source.Size.Width + endspace,
                                      source.Size.Height + endspace);


                    if (!(this.HeadNode as Node).IsInternallyLoaded)
                    {
                        s = new Point((this.HeadNode as Node).OffsetX + (this.HeadNode as Node)._Width / 2, (this.HeadNode as Node).OffsetY + (this.HeadNode as Node)._Height / 2);
                    }
                    else
                    {
                        s = new Point(source.Position.X, source.Position.Y);
                    }

                }
                else
                {
                    sourceRect = new Rect(
                                     source.Left + ConnectionHeadPort.PxCenterPosition.X + ((double.IsNaN(ConnectionHeadPort.Width)) ? 2.5 : ConnectionHeadPort.Width / 2),
                                     source.Top + ConnectionHeadPort.PxCenterPosition.Y + ((double.IsNaN(ConnectionHeadPort.Height)) ? 2.5 : ConnectionHeadPort.Height / 2),
                                     source.Size.Width + endspace,
                                     source.Size.Height + endspace);

                    s = new Point(source.Left + ConnectionHeadPort.PxCenterPosition.X + ((double.IsNaN(ConnectionHeadPort.Width)) ? 2.5 : ConnectionHeadPort.Width / 2),
                                     source.Top + ConnectionHeadPort.PxCenterPosition.Y + ((double.IsNaN(ConnectionHeadPort.Height)) ? 2.5 : ConnectionHeadPort.Height / 2));
                }
            }
            else
            {
                s = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                sourceRect = new Rect(
                                    sourceleft,
                                    sourcetop,
                                    twozero,
                                    twozero);
            }

            if (TailNode != null)
            {
                if (this.ConnectionTailPort == null)
                {
                    targetRect = new Rect(
                                     target.Left - endspace,
                                     target.Top - endspace,
                                     target.Size.Width + endspace,
                                     target.Size.Height + endspace);
                    //e = new Point(target.Position.X, target.Position.Y);
                    if (!(this.TailNode as Node).IsInternallyLoaded)
                    {
                        e = new Point((this.TailNode as Node).OffsetX + (this.TailNode as Node)._Width / 2, (this.TailNode as Node).OffsetY + (this.TailNode as Node)._Height / 2);
                    }
                    else
                    {
                        e = new Point(target.Position.X, target.Position.Y);
                    }

                }
                else
                {
                    double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                    double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                    targetRect = new Rect(
                                     target.Left + ConnectionTailPort.PxCenterPosition.X + width,
                                     target.Top + ConnectionTailPort.PxCenterPosition.Y + height,
                                     target.Size.Width + endspace,
                                     target.Size.Height + endspace);

                    e = new Point(target.Left + ConnectionTailPort.PxCenterPosition.X + width,
                                     target.Top + ConnectionTailPort.PxCenterPosition.Y + height);
                }

            }
            else
            {
                e = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                targetRect = new Rect(
                                    sourceleft,
                                    sourcetop,
                                    twozero,
                                    twozero);
            }

            if (this.ConnectorType != ConnectorType.Orthogonal)
            {
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);

                if (ConnectionHeadPort != null)
                {
                    double width = double.IsNaN(ConnectionHeadPort.DesiredSize.Width) ? 2.5 : ConnectionHeadPort.DesiredSize.Width / 2;
                    double height = double.IsNaN(ConnectionHeadPort.DesiredSize.Height) ? 2.5 : ConnectionHeadPort.DesiredSize.Height / 2;

                    startPoint = new Point(source.Left + ConnectionHeadPort.PxLeft ,
                                     source.Top + ConnectionHeadPort.PxTop );
                    if ((this.HeadNode as Node).RenderTransform != null)
                    {
                        if ((this.HeadNode as Node).RenderTransform is RotateTransform)
                        {
                            startPoint = this.GeneralPointRotation(source.Position, startPoint, ((this.HeadNode as Node).RenderTransform as RotateTransform).Angle);
                        }

                    }
                }
                else
                {
                    if (InterPts.Count != 0)
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, InterPts[0], sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                    else
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, e, sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                }

                if (ConnectionTailPort != null)
                {
                    double width = double.IsNaN(ConnectionTailPort.DesiredSize.Width) ? 2.5 : ConnectionTailPort.DesiredSize.Width / 2;
                    double height = double.IsNaN(ConnectionTailPort.DesiredSize.Height) ? 2.5 : ConnectionTailPort.DesiredSize.Height / 2;

                    endPoint = new Point(target.Left + ConnectionTailPort.PxLeft ,
                                     target.Top + ConnectionTailPort.PxTop );

                    if ((this.TailNode as Node).RenderTransform != null)
                    {
                        if ((this.TailNode as Node).RenderTransform is RotateTransform)
                        {
                            endPoint = this.GeneralPointRotation(target.Position, endPoint, ((this.TailNode as Node).RenderTransform as RotateTransform).Angle);
                        }
                    }
                }
                else
                {
                    if (!(this.TailNode as Node).IsInternallyLoaded)
                    {
                        endPoint = new Point((this.TailNode as Node).OffsetX + (this.TailNode as Node)._Width / 2, (this.TailNode as Node).OffsetY + (this.TailNode as Node)._Height / 2);
                    }
                    else
                    {
                        if (InterPts.Count != 0)
                        {
                            endPoint = ConnectorBase.GetLineIntersect(target, InterPts[InterPts.Count - 1], e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                        }
                        else
                        {
                            endPoint = ConnectorBase.GetLineIntersect(target, s, e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                        }
                    }
                }

                connectionPoints.Add(startPoint);
                connectionPoints.Add(endPoint);
            }
            else
            {
                Point startPoint, endPoint;

                if (!(Orientation == TreeOrientation.LeftRight || Orientation == TreeOrientation.RightLeft))
                {
                    ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, targetRect, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                }
                else
                {
                    ConnectorBase.GetTreeOrthogonalLineIntersect(source, target, sourceRect, targetRect, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                }

                if (ConnectionHeadPort != null)
                {
                    double width = double.IsNaN(ConnectionHeadPort.Width) ? 2.5 : ConnectionHeadPort.Width / 2;
                    double height = double.IsNaN(ConnectionHeadPort.Height) ? 2.5 : ConnectionHeadPort.Height / 2;

                    startPoint = new Point(source.Left + ConnectionHeadPort.PxCenterPosition.X + width, source.Top + ConnectionHeadPort.PxCenterPosition.Y + height);
                    if ((this.HeadNode as Node).RenderTransform != null)
                    {
                        if ((this.HeadNode as Node).RenderTransform is RotateTransform)
                        {
                            startPoint = this.GeneralPointRotation(source.Position, startPoint, ((this.HeadNode as Node).RenderTransform as RotateTransform).Angle);
                        }

                    }
                }
                else
                {
                    if (InterPts.Count != 0)
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, InterPts[0], sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                    else if (this.IntermediatePoints.Count != 0)
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, this.IntermediatePoints[0], sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                    else
                    {
                        startPoint = ConnectorBase.GetLineIntersect(source, s, e, sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);
                    }
                }

                if (ConnectionTailPort != null)
                {
                    double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                    double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                    endPoint = new Point(target.Left + ConnectionTailPort.PxCenterPosition.X + width, target.Top + ConnectionTailPort.PxCenterPosition.Y + height);
                    if ((this.TailNode as Node).RenderTransform != null)
                    {
                        if ((this.TailNode as Node).RenderTransform is RotateTransform)
                        {
                            endPoint = this.GeneralPointRotation(target.Position, endPoint, ((this.TailNode as Node).RenderTransform as RotateTransform).Angle);
                        }

                    }
                }
                else
                {
                    if (!(this.TailNode as Node).IsInternallyLoaded)
                    {
                        endPoint = new Point((this.TailNode as Node).OffsetX + (this.TailNode as Node)._Width / 2, (this.TailNode as Node).OffsetY + (this.TailNode as Node)._Height / 2);
                    }
                    else
                    {
                        if (InterPts.Count != 0)
                        {
                            endPoint = ConnectorBase.GetLineIntersect(target, InterPts[InterPts.Count - 1], e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                        }
                        else if (this.IntermediatePoints.Count != 0)
                        {
                            endPoint = ConnectorBase.GetLineIntersect(target, this.IntermediatePoints[this.IntermediatePoints.Count - 1], e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                        }
                        else
                        {
                            endPoint = ConnectorBase.GetLineIntersect(target, s, e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                        }
                    }
                }

                if (this.HeadDecoratorGrid != null)
                {
                    if (b5)
                    {
                        (this as LineConnector).HeadDecoratorAngle = -90;
                    }
                    else if (b6)
                    {
                        (this as LineConnector).HeadDecoratorAngle = 90;
                    }
                    else if (b7)
                    {
                        (this as LineConnector).HeadDecoratorAngle = -180;
                    }
                    else if (b8)
                    {
                        (this as LineConnector).HeadDecoratorAngle = 0;
                    }
                }

                if (this.TailDecoratorGrid != null)
                {
                    if (b1)
                    {
                        (this as LineConnector).TailDecoratorAngle = -90;
                    }
                    else if (b2)
                    {
                        (this as LineConnector).TailDecoratorAngle = 90;
                    }
                    else if (b3)
                    {
                        (this as LineConnector).TailDecoratorAngle = -180;
                    }
                    else if (b4)
                    {
                        (this as LineConnector).TailDecoratorAngle = 0;
                    }
                }

                connectionPoints.Add(startPoint);
                connectionPoints.Add(endPoint);
            }

            int count = connectionPoints.Count();
            double x = Math.Pow((connectionPoints[0].X - connectionPoints[count - 1].X), 2);
            double y = Math.Pow((connectionPoints[0].Y - connectionPoints[count - 1].Y), 2);
            Distance = Math.Sqrt(x + y);
            return connectionPoints;
        }

        /// <summary>
        /// Gets the line points when head node is null.
        /// </summary>
        /// <param name="target">The source node</param>
        /// <param name="istarget">true, if it is the target.</param>
        /// <returns>Collection Of points</returns>
        internal List<Point> GetLinePoints(NodeInfo target, bool istarget)
        {
            double twozero = 20;// MeasureUnitsConverter.FromPixels(20, DiagramPage.Munits);
            double sourceleft = PxStartPointPosition.X;// MeasureUnitsConverter.FromPixels(StartPointPosition.X, DiagramPage.Munits);
            double sourcetop = PxStartPointPosition.Y;// MeasureUnitsConverter.FromPixels(StartPointPosition.Y, DiagramPage.Munits);
            double dropcentreX = DropPoint.X;// MeasureUnitsConverter.FromPixels(DropPoint.X, DiagramPage.Munits);
            double dropcentreY = DropPoint.Y;// MeasureUnitsConverter.FromPixels(DropPoint.Y, DiagramPage.Munits);
            double targetleft = PxEndPointPosition.X;// MeasureUnitsConverter.FromPixels(EndPointPosition.X, DiagramPage.Munits);
            double targettop = PxEndPointPosition.Y;// MeasureUnitsConverter.FromPixels(EndPointPosition.Y, DiagramPage.Munits);
            if (this.dview == null)
            {
                this.dview = Node.GetDiagramView(this);
            }

            double cs;
            double bl;
            double endspace;

            bl = this.BendLength;// MeasureUnitsConverter.FromPixels(this.BendLength, target.MeasurementUnit);            
            cs = this.PxConnectionEndSpace;// MeasureUnitsConverter.FromPixels(this.ConnectionEndSpace, target.MeasurementUnit);
            endspace = cs;

            bool b1, b2, b3, b4, b5, b6, b7, b8;
            Point s = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
            Point e = new Point(0, 0);
            Rect targetRect = new Rect();
            Rect sourceRect = new Rect();
            sourceRect = new Rect(
                                   sourceleft,
                                   sourcetop,
                                   twozero,
                                   twozero);
            NodeInfo source = new NodeInfo();
            source.Position = new Point(PxStartPointPosition.X + 25, PxStartPointPosition.Y + 25);
            ConnectionPoints = new List<Point>();
            if (TailNode != null)
            {
                targetRect = new Rect(
                                      target.Left - endspace,
                                      target.Top - endspace,
                                      target.Size.Width + endspace,
                                      target.Size.Height + endspace);
                if (!(this.TailNode as Node).IsInternallyLoaded)
                {
                    e = new Point((this.TailNode as Node).OffsetX + (this.TailNode as Node)._Width / 2, (this.TailNode as Node).OffsetY + (this.TailNode as Node)._Height / 2);
                }
                else
                {
                    e = new Point(target.Position.X, target.Position.Y);
                }
                //e = new Point(target.Position.X, target.Position.Y);
            }
            else
            {
                e = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                targetRect = new Rect(
                                    targetleft,
                                    targettop,
                                    twozero,
                                    twozero);
            }

            if (this.ConnectorType != ConnectorType.Orthogonal)
            {
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);

                if (ConnectionTailPort != null)
                {
                    double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                    double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                    endPoint = new Point(ConnectionTailPort.PxCenterPosition.X + target.Left + width, height + ConnectionTailPort.PxCenterPosition.Y + target.Top);
                    if ((this.TailNode as Node).RenderTransform != null)
                    {
                        if ((this.TailNode as Node).RenderTransform is RotateTransform)
                        {
                            endPoint = this.GeneralPointRotation(target.Position, endPoint, ((this.TailNode as Node).RenderTransform as RotateTransform).Angle);
                        }

                    }
                }
                else
                {
                    if (InterPts.Count != 0)
                    {
                        endPoint = ConnectorBase.GetLineIntersect(target, InterPts[InterPts.Count - 1], e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                    }
                    else
                    {
                        endPoint = ConnectorBase.GetLineIntersect(target, s, e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                    }
                }

                startPoint = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                ConnectionPoints.Add(startPoint);
                ConnectionPoints.Add(endPoint);
            }
            else
            {
                Point startPoint, endPoint;

                if (!(Orientation == TreeOrientation.LeftRight || Orientation == TreeOrientation.RightLeft))
                {
                    ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, targetRect, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                }
                else
                {
                    ConnectorBase.GetTreeOrthogonalLineIntersect(source, target, sourceRect, targetRect, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                }

                if (ConnectionTailPort != null)
                {
                    double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                    double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                    endPoint = new Point(ConnectionTailPort.PxCenterPosition.X + target.Left + width, height + ConnectionTailPort.PxCenterPosition.Y + target.Top);
                    if ((this.TailNode as Node).RenderTransform != null)
                    {
                        if ((this.TailNode as Node).RenderTransform is RotateTransform)
                        {
                            endPoint = this.GeneralPointRotation(target.Position, endPoint, ((this.TailNode as Node).RenderTransform as RotateTransform).Angle);
                        }

                    }
                }
                else
                {
                    if (InterPts.Count != 0)
                    {
                        //Point temp = endPoint;
                        endPoint = ConnectorBase.GetLineIntersect(target, InterPts[InterPts.Count - 1], e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                        //if (endPoint == new Point(0, 0))
                        //    endPoint = temp;
                    }
                    else
                    {
                        endPoint = ConnectorBase.GetLineIntersect(target, s, e, targetRect, out b5, out b6, out b7, out b8, this.ConnectorType);
                    }
                }

                if (this.HeadDecoratorGrid != null)
                {
                    if (b5)
                    {
                        (this as LineConnector).HeadDecoratorAngle = -90;
                    }
                    else if (b6)
                    {
                        (this as LineConnector).HeadDecoratorAngle = 90;
                    }
                    else if (b7)
                    {
                        (this as LineConnector).HeadDecoratorAngle = -180;
                    }
                    else if (b8)
                    {
                        (this as LineConnector).HeadDecoratorAngle = 0;
                    }
                }

                if (this.TailDecoratorGrid != null)
                {
                    if (b1)
                    {
                        (this as LineConnector).TailDecoratorAngle = -90;
                    }
                    else if (b2)
                    {
                        (this as LineConnector).TailDecoratorAngle = 90;
                    }
                    else if (b3)
                    {
                        (this as LineConnector).TailDecoratorAngle = -180;
                    }
                    else if (b4)
                    {
                        (this as LineConnector).TailDecoratorAngle = 0;
                    }
                }

                startPoint = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                ConnectionPoints.Add(startPoint);
                ConnectionPoints.Add(endPoint);
            }

            int count = ConnectionPoints.Count();
            double x = Math.Pow((ConnectionPoints[0].X - ConnectionPoints[count - 1].X), 2);
            double y = Math.Pow((ConnectionPoints[0].Y - ConnectionPoints[count - 1].Y), 2);
            Distance = Math.Sqrt(x + y);
            return ConnectionPoints;
        }

        /// <summary>
        /// Gets the line points when both head node and tail node are not specified.
        /// </summary>
        /// <returns>The collection of points</returns>
        internal List<Point> GetLinePoints()
        {
            double twozero = 20;// MeasureUnitsConverter.FromPixels(20, DiagramPage.Munits);
            double sourceleft = PxStartPointPosition.X;// MeasureUnitsConverter.FromPixels(StartPointPosition.X, DiagramPage.Munits);
            double sourcetop = PxStartPointPosition.Y;// MeasureUnitsConverter.FromPixels(StartPointPosition.Y, DiagramPage.Munits);
            double dropcentreX = DropPoint.X;// MeasureUnitsConverter.FromPixels(DropPoint.X, DiagramPage.Munits);
            double dropcentreY = DropPoint.Y;// MeasureUnitsConverter.FromPixels(DropPoint.Y, DiagramPage.Munits);
            double targetleft = PxEndPointPosition.X;// MeasureUnitsConverter.FromPixels(EndPointPosition.X, DiagramPage.Munits);
            double targettop = PxEndPointPosition.Y;// MeasureUnitsConverter.FromPixels(EndPointPosition.Y, DiagramPage.Munits);

            bool b1, b2, b3, b4, b5, b6, b7, b8;
            Point s = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
            Point e = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
            Rect sourceRect = new Rect();
            Rect targetRect = new Rect();
            NodeInfo target = new NodeInfo();
            target.Position = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
            targetRect = new Rect(
                                   targetleft,
                                   targettop,
                                    twozero,
                                    twozero);

            NodeInfo source = new NodeInfo();
            source.Position = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
            sourceRect = new Rect(
                                sourceleft,
                                sourcetop,
                                twozero,
                                twozero);

            ConnectionPoints = new List<Point>();
            if (this.ConnectorType != ConnectorType.Orthogonal)
            {
                //Point startPoint = ConnectorBase.GetLineIntersect(source, s, e, sourceRect, out b1, out b2, out b3, out b4, this.ConnectorType);

                Point startPoint = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                Point endPoint = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);

                ConnectionPoints.Add(startPoint);
                ConnectionPoints.Add(endPoint);
            }
            else
            {
                Point startPoint, endPoint;

                if (!(Orientation == TreeOrientation.LeftRight || Orientation == TreeOrientation.RightLeft))
                {
                    ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, targetRect, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                }
                else
                {
                    ConnectorBase.GetTreeOrthogonalLineIntersect(source, target, sourceRect, targetRect, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);
                }

                startPoint = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                endPoint = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);

                if (this.HeadDecoratorGrid != null)
                {
                    if (b5)
                    {
                        (this as LineConnector).HeadDecoratorAngle = -90;
                    }
                    else if (b6)
                    {
                        (this as LineConnector).HeadDecoratorAngle = 90;
                    }
                    else if (b7)
                    {
                        (this as LineConnector).HeadDecoratorAngle = -180;
                    }
                    else if (b8)
                    {
                        (this as LineConnector).HeadDecoratorAngle = 0;
                    }
                }

                if (this.TailDecoratorGrid != null)
                {
                    if (b1)
                    {
                        (this as LineConnector).TailDecoratorAngle = -90;
                    }
                    else if (b2)
                    {
                        (this as LineConnector).TailDecoratorAngle = 90;
                    }
                    else if (b3)
                    {
                        (this as LineConnector).TailDecoratorAngle = -180;
                    }
                    else if (b4)
                    {
                        (this as LineConnector).TailDecoratorAngle = 0;
                    }
                }

                ConnectionPoints.Add(startPoint);
                ConnectionPoints.Add(endPoint);
            }

            int count = ConnectionPoints.Count();
            double x = Math.Pow((ConnectionPoints[0].X - ConnectionPoints[count - 1].X), 2);
            double y = Math.Pow((ConnectionPoints[0].Y - ConnectionPoints[count - 1].Y), 2);
            Distance = Math.Sqrt(x + y);
            return ConnectionPoints;
        }

        /// <summary>
        /// Returns the Bezier segment.
        /// </summary>
        /// <param name="x1">The x coordinate of the starting point(first control point) of the curve.</param>
        /// <param name="y1">The y coordinate of the starting point(first control point)of the curve.</param>
        /// <param name="x2">The x coordinate of the end point of the curve.</param>
        /// <param name="y2">The y coordinate of the end point of the curve.</param>
        /// <param name="temp1">The x coordinate of the second control point of the curve.</param>
        /// <param name="temp2">The y coordinate of the second control point of the curve.</param>
        /// <param name="num1">It specifies the amount of curve to be provided.Value is 150d.</param>
        /// <param name="isTop">Flag indicating the top side.</param>
        /// <param name="isBottom">Flag indicating the bottom side.</param>
        /// <param name="isLeft">Flag indicating the left side.</param>
        /// <param name="isRight">Flag indicating the right side.</param>
        /// <returns>The Bezier segment</returns>
        internal BezierSegment GetSegment(double x1, double y1, double x2, double y2, double temp1, double temp2, double num1, bool isTop, bool isBottom, bool isLeft, bool isRight)
        {
            if (isTop)
            {
                return this.Segment(x1, y1, x2, y2, temp1, temp2 - num1);
            }
            else if (isBottom)
            {
                return this.Segment(x1, y1, x2, y2, temp1, temp2 + num1);
            }
            else if (isLeft)
            {
                return this.Segment(x1, y1, x2, y2, temp1 - num1, temp2);
            }
            else
            {
                return this.Segment(x1, y1, x2, y2, temp1 + num1, temp2);
            }
        }

        private List<Point> meetOrhogonalConstraints(List<Point> connectionPoints, double cs)
        {
            if (this.AutoAdjustPoints && this.HeadNode != null && this.TailNode != null && this.ConnectionHeadPort != null && this.ConnectionTailPort != null)
            {
                Dock HeadDirection = Dock.Top;
                Dock TailDirection = Dock.Top;

                double headDist = this.FirstSegmentLength;
                double tailDist = this.LastSegmentLength;
                bool isHeadRecal = false, isTailRecal = false;
                if (!double.IsNaN(headDist))
                {
                    isHeadRecal = true;
                }
                else
                {
                    headDist = 50;
                }

                if (!double.IsNaN(tailDist))
                {
                    isTailRecal = true;
                }
                else
                {
                    tailDist = 50;
                }

                Node HeadNode = (this.HeadNode as Node);
                Node TailNode = (this.TailNode as Node);

                List<Point> PxInterPts = new List<Point>();
                foreach (Point pt in this.IntermediatePoints)
                {
                    PxInterPts.Add(pt);
                }
                if (this.ConnectionHeadPort != null)
                {
                    HeadDirection = this.ConnectionHeadPort.Direction();
                }
                if (this.ConnectionTailPort != null)
                {
                    TailDirection = this.ConnectionTailPort.Direction();
                }
                if (this.HeadNode != null && this.TailNode != null && this.ConnectionHeadPort != null && this.ConnectionTailPort != null)
                {
                    //// Horizontal - Horizontal
                    if ((HeadDirection == Dock.Left || HeadDirection == Dock.Right) &&
                        (TailDirection == Dock.Left || TailDirection == Dock.Right))
                    {
                        #region init
                        bool is4Pts = false;
                        if (HeadDirection == Dock.Right && TailDirection == Dock.Right)
                        {
                            if ((ConnectionTailPort.PagePosition.Y > HeadNode.Top) && (ConnectionTailPort.PagePosition.Y < HeadNode.Bottom))
                            {
                                is4Pts = true;
                            }
                        }
                        else if (HeadDirection == Dock.Left && TailDirection == Dock.Left)
                        {
                            if ((ConnectionTailPort.PagePosition.Y > HeadNode.Top) && (ConnectionTailPort.PagePosition.Y < HeadNode.Bottom))
                            {
                                is4Pts = true;
                            }
                        }
                        else if (HeadDirection == Dock.Left && TailDirection == Dock.Right)
                        {
                            if (HeadNode.Left < TailNode.Right)
                            {
                                is4Pts = true;
                            }
                        }
                        else if (HeadDirection == Dock.Right && TailDirection == Dock.Left)
                        {
                            if (HeadNode.Right > TailNode.Left)
                            {
                                is4Pts = true;
                            }
                        }
                        #endregion

                        #region 4 Points Required
                        //// Required 4 inter pts.
                        if (is4Pts)
                        {
                            bool isNewPtsAdded = false;
                            if (PxInterPts.Count % 2 != 0)
                            {
                                isNewPtsAdded = true;
                                PxInterPts.Add(new Point(0, 0));
                            }
                            //// Update Head (Right)
                            if (HeadDirection == Dock.Right)
                            {
                                if (PxInterPts.Count >= 4)
                                {
                                    if (isHeadRecal || PxInterPts[0].X < HeadNode.Right)
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X + headDist, ConnectionHeadPort.PagePosition.Y);
                                    }
                                    else
                                    {
                                        PxInterPts[0] = new Point(PxInterPts[0].X, ConnectionHeadPort.PagePosition.Y);
                                    }
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 4)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X + headDist, ConnectionHeadPort.PagePosition.Y);
                                    PxInterPts[1] = new Point(PxInterPts[0].X, ConnectionHeadPort.PagePosition.X - headDist);
                                }
                            }
                            //// Update Head (Left)
                            else
                            {
                                if (PxInterPts.Count >= 4)
                                {
                                    if (isHeadRecal || PxInterPts[0].X > HeadNode.Left)
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X - headDist, ConnectionHeadPort.PagePosition.Y);
                                    }
                                    else
                                    {
                                        PxInterPts[0] = new Point(PxInterPts[0].X, ConnectionHeadPort.PagePosition.Y);
                                    }
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 4)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X - headDist, ConnectionHeadPort.PagePosition.Y);
                                    PxInterPts[1] = new Point(PxInterPts[0].X, ConnectionHeadPort.PagePosition.Y - headDist);
                                }
                            }

                            //// Update Tail (Right)
                            if (TailDirection == Dock.Right)
                            {
                                if (PxInterPts.Count >= 4 && !isNewPtsAdded)
                                {
                                    int last = PxInterPts.Count;
                                    if (isTailRecal || PxInterPts[last - 1].X < TailNode.Right)
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X + tailDist, ConnectionTailPort.PagePosition.Y);
                                    }
                                    else
                                    {
                                        PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, ConnectionTailPort.PagePosition.Y);
                                    }
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 1].X, PxInterPts[last - 2].Y);
                                    PxInterPts[last - 3] = new Point(PxInterPts[last - 3].X, PxInterPts[last - 2].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 4)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    int last = PxInterPts.Count;
                                    PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X + tailDist, ConnectionTailPort.PagePosition.Y);
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 1].X, ConnectionTailPort.PagePosition.Y - tailDist);
                                    PxInterPts[last - 3] = new Point(PxInterPts[last - 3].X, PxInterPts[last - 2].Y);
                                }
                            }
                            //// Update Tail (Left)
                            else
                            {
                                if (PxInterPts.Count >= 4 && !isNewPtsAdded)
                                {
                                    int last = PxInterPts.Count;
                                    if (isTailRecal || PxInterPts[last - 1].X > TailNode.Left)
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X - tailDist, ConnectionTailPort.PagePosition.Y);
                                    }
                                    else
                                    {
                                        PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, ConnectionTailPort.PagePosition.Y);
                                    }
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 1].X, PxInterPts[last - 2].Y);
                                    PxInterPts[last - 3] = new Point(PxInterPts[last - 3].X, PxInterPts[last - 2].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 4)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    int last = PxInterPts.Count;
                                    PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X + tailDist, ConnectionTailPort.PagePosition.Y);
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 1].X, ConnectionTailPort.PagePosition.Y - tailDist);
                                    PxInterPts[last - 3] = new Point(PxInterPts[last - 3].X, PxInterPts[last - 2].Y);
                                }
                            }
                        }
                        #endregion

                        #region 2 Points Required
                        //required 2 inter pts.
                        else
                        {
                            bool isNewPtsAdded = false;
                            if (PxInterPts.Count % 2 != 0)
                            {
                                isNewPtsAdded = true;
                                PxInterPts.Add(new Point(0, 0));
                            }
                            if (HeadDirection == Dock.Right)
                            {
                                if (PxInterPts.Count >= 2)
                                {
                                    int last = PxInterPts.Count;
                                    if (isHeadRecal || PxInterPts[0].X < HeadNode.Right)
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X + headDist, ConnectionHeadPort.PagePosition.Y);
                                    }
                                    else
                                    {
                                        PxInterPts[0] = new Point(PxInterPts[0].X, ConnectionHeadPort.PagePosition.Y);
                                    }
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 2)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X + headDist, ConnectionHeadPort.PagePosition.Y);
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                            }
                            else
                            {
                                if (PxInterPts.Count >= 2)
                                {
                                    if (isHeadRecal || PxInterPts[0].X > HeadNode.Left)
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X - headDist, ConnectionHeadPort.PagePosition.Y);
                                    }
                                    else
                                    {
                                        PxInterPts[0] = new Point(PxInterPts[0].X, ConnectionHeadPort.PagePosition.Y);
                                    }
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }

                                else
                                {
                                    while (PxInterPts.Count < 2)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X - headDist, ConnectionHeadPort.PagePosition.Y);
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                            }

                            if (TailDirection == Dock.Right)
                            {
                                if ((PxInterPts.Count >= 2) && !isNewPtsAdded)
                                {
                                    int last = PxInterPts.Count;
                                    if ((isTailRecal && (PxInterPts.Count > 2 || (ConnectionHeadPort.PagePosition.X + headDist < ConnectionTailPort.PagePosition.X + tailDist))) || PxInterPts[last - 1].X < TailNode.Right)
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X + tailDist, ConnectionTailPort.PagePosition.Y);
                                    }
                                    else
                                    {
                                        PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, ConnectionTailPort.PagePosition.Y);
                                    }
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 1].X, PxInterPts[last - 2].Y);
                                }

                                else
                                {
                                    while (PxInterPts.Count < 2)
                                    {
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    int last = PxInterPts.Count;
                                    PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X + tailDist, ConnectionTailPort.PagePosition.Y);
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 1].X, PxInterPts[last - 2].Y);
                                }
                            }
                            else
                            {
                                if ((PxInterPts.Count >= 2) && !isNewPtsAdded)
                                {
                                    int last = PxInterPts.Count;
                                    if ((isTailRecal && (PxInterPts.Count > 2 || (ConnectionHeadPort.PagePosition.X - headDist > ConnectionTailPort.PagePosition.X - tailDist))) || PxInterPts[last - 1].X > TailNode.Left)
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X - tailDist, ConnectionTailPort.PagePosition.Y);
                                    }
                                    else
                                    {
                                        PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, ConnectionTailPort.PagePosition.Y);
                                    }
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 1].X, PxInterPts[last - 2].Y);
                                }

                                else
                                {
                                    while (PxInterPts.Count < 2)
                                    {
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    int last = PxInterPts.Count;
                                    PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X - tailDist, ConnectionTailPort.PagePosition.Y);
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 1].X, PxInterPts[last - 2].Y);
                                }
                            }
                        }

                        #endregion

                    }
                    //// Vertical - Vertical
                    else if ((HeadDirection == Dock.Top || HeadDirection == Dock.Bottom) &&
                        (TailDirection == Dock.Top || TailDirection == Dock.Bottom))
                    {
                        #region init

                        bool is4Pts = false;
                        if (HeadDirection == Dock.Bottom && TailDirection == Dock.Bottom)
                        {
                            if ((ConnectionTailPort.PagePosition.X > HeadNode.Left) && (ConnectionTailPort.PagePosition.X < HeadNode.Right))
                            {
                                is4Pts = true;
                            }
                        }
                        else if (HeadDirection == Dock.Top && TailDirection == Dock.Top)
                        {
                            if ((ConnectionTailPort.PagePosition.X > HeadNode.Left) && (ConnectionTailPort.PagePosition.X < HeadNode.Right))
                            {
                                is4Pts = true;
                            }
                        }
                        else if (HeadDirection == Dock.Top && TailDirection == Dock.Bottom)
                        {
                            if (HeadNode.Top < TailNode.Bottom)
                            {
                                is4Pts = true;
                            }
                        }
                        else if (HeadDirection == Dock.Bottom && TailDirection == Dock.Top)
                        {
                            if (HeadNode.Bottom > TailNode.Top)
                            {
                                is4Pts = true;
                            }
                        }
                        #endregion

                        #region 4 Points Required
                        //// Required 4 inter pts.
                        if (is4Pts)
                        {
                            bool isNewPtsAdded = false;
                            //// Update Head (Bottom)
                            if (HeadDirection == Dock.Bottom)
                            {
                                if (PxInterPts.Count >= 4)
                                {
                                    if (isHeadRecal || PxInterPts[0].Y < HeadNode.Bottom)
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y + headDist);
                                    }
                                    else
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, PxInterPts[0].Y);
                                    }
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 4)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y + headDist);
                                    PxInterPts[1] = new Point(ConnectionHeadPort.PagePosition.X - headDist, PxInterPts[0].Y);
                                }
                            }
                            //// Update Head (Top)
                            else
                            {
                                if (PxInterPts.Count >= 4)
                                {
                                    if (isHeadRecal || PxInterPts[0].Y > HeadNode.Top)
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y - headDist);
                                    }
                                    else
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, PxInterPts[0].Y);
                                    }
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 4)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y - headDist);
                                    PxInterPts[1] = new Point(ConnectionHeadPort.PagePosition.X - headDist, PxInterPts[0].Y);
                                }
                            }

                            //// Update Tail (Bottom)
                            if (TailDirection == Dock.Bottom)
                            {
                                if (PxInterPts.Count >= 4 && !isNewPtsAdded)
                                {
                                    int last = PxInterPts.Count;
                                    if (isTailRecal || PxInterPts[last - 1].Y < TailNode.Bottom)
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y + tailDist);
                                    }
                                    else
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, PxInterPts[last - 1].Y);
                                    }
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 1].Y);
                                    PxInterPts[last - 3] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 3].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 4)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    int last = PxInterPts.Count;
                                    PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y + tailDist);
                                    PxInterPts[last - 2] = new Point(ConnectionTailPort.PagePosition.X - tailDist, PxInterPts[last - 1].Y);
                                    PxInterPts[last - 3] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 3].Y);
                                }
                            }
                            ////Update Tail (Top)
                            else
                            {
                                if (PxInterPts.Count >= 4 && !isNewPtsAdded)
                                {
                                    int last = PxInterPts.Count;
                                    if (isTailRecal || PxInterPts[last - 1].Y > TailNode.Top)
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y - tailDist);
                                    }
                                    else
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, PxInterPts[last - 1].Y);
                                    }
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 1].Y);
                                    PxInterPts[last - 3] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 3].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 4)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    int last = PxInterPts.Count;
                                    PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y - tailDist);
                                    PxInterPts[last - 2] = new Point(ConnectionTailPort.PagePosition.X - tailDist, PxInterPts[last - 1].Y);
                                    PxInterPts[last - 3] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 3].Y);
                                }
                            }
                        }
                        #endregion

                        #region 2 Points Required
                        //required 2 inter pts.
                        else
                        {
                            bool isNewPtsAdded = false;
                            if (HeadDirection == Dock.Bottom)
                            {
                                if (PxInterPts.Count >= 2)
                                {
                                    int last = PxInterPts.Count;
                                    if (isHeadRecal || PxInterPts[0].Y < HeadNode.Bottom)
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y + headDist);
                                    }
                                    else
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, PxInterPts[0].Y);
                                    }
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                                else
                                {
                                    while (PxInterPts.Count < 2)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y + headDist);
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                            }
                            else
                            {
                                if (PxInterPts.Count >= 2)
                                {
                                    if (isHeadRecal || PxInterPts[0].Y > HeadNode.Top)
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y - headDist);
                                    }
                                    else
                                    {
                                        PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, PxInterPts[0].Y);
                                    }
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }

                                else
                                {
                                    while (PxInterPts.Count < 2)
                                    {
                                        isNewPtsAdded = true;
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y - headDist);
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                            }

                            if (TailDirection == Dock.Bottom)
                            {
                                if ((PxInterPts.Count >= 2) && !isNewPtsAdded)
                                {
                                    int last = PxInterPts.Count;
                                    if ((isTailRecal && (PxInterPts.Count > 2 || (ConnectionHeadPort.PagePosition.Y + headDist < ConnectionTailPort.PagePosition.Y + tailDist))) || PxInterPts[last - 1].Y < TailNode.Bottom)
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y + tailDist);
                                    }
                                    else
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, PxInterPts[last - 1].Y);
                                    }
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 1].Y);
                                }

                                else
                                {
                                    while (PxInterPts.Count < 2)
                                    {
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    int last = PxInterPts.Count;
                                    PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y + tailDist);
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 1].Y);
                                }
                            }
                            else
                            {
                                if ((PxInterPts.Count >= 2) && !isNewPtsAdded)
                                {
                                    int last = PxInterPts.Count;
                                    if ((isTailRecal && (PxInterPts.Count > 2 || (ConnectionHeadPort.PagePosition.Y + headDist > ConnectionTailPort.PagePosition.Y + tailDist))) || PxInterPts[last - 1].Y > TailNode.Top)
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y - tailDist);
                                    }
                                    else
                                    {
                                        PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, PxInterPts[last - 1].Y);
                                    }
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 1].Y);
                                }

                                else
                                {
                                    while (PxInterPts.Count < 2)
                                    {
                                        PxInterPts.Add(new Point(0, 0));
                                    }
                                    int last = PxInterPts.Count;
                                    PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y - tailDist);
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, PxInterPts[last - 1].Y);
                                }
                            }
                        }

                        #endregion
                    }
                    //// Horizontal - Vertical
                    else if (((HeadDirection == Dock.Top || HeadDirection == Dock.Bottom) && ((TailDirection == Dock.Left) || (TailDirection == Dock.Right))) ||
                       ((TailDirection == Dock.Top || TailDirection == Dock.Bottom) && ((HeadDirection == Dock.Left) || (HeadDirection == Dock.Right))))
                    {
                        #region init

                        Point HeadPortPos = ConnectionHeadPort.PagePosition;
                        Point TailPortPos = ConnectionTailPort.PagePosition;
                        if ((PxInterPts.Count == 1) && ((HeadDirection == Dock.Right && TailDirection == Dock.Bottom && HeadPortPos.X < TailPortPos.X && HeadPortPos.Y > TailPortPos.Y) ||
                            (HeadDirection == Dock.Right && TailDirection == Dock.Top && HeadPortPos.X < TailPortPos.X && HeadPortPos.Y < TailPortPos.Y) ||
                            (HeadDirection == Dock.Left && TailDirection == Dock.Top && HeadPortPos.X > TailPortPos.X && HeadPortPos.Y < TailPortPos.Y) ||
                            (HeadDirection == Dock.Left && TailDirection == Dock.Bottom && HeadPortPos.X > TailPortPos.X && HeadPortPos.Y > TailPortPos.Y)
                            ))
                        {
                            PxInterPts[0] = new Point(TailPortPos.X, HeadPortPos.Y);
                        }
                        else if ((PxInterPts.Count == 1) && ((TailDirection == Dock.Right && HeadDirection == Dock.Bottom && HeadPortPos.X > TailPortPos.X && HeadPortPos.Y < TailPortPos.Y) ||
                            (TailDirection == Dock.Right && HeadDirection == Dock.Top && HeadPortPos.X > TailPortPos.X && HeadPortPos.Y > TailPortPos.Y) ||
                            (TailDirection == Dock.Left && HeadDirection == Dock.Top && HeadPortPos.X < TailPortPos.X && HeadPortPos.Y > TailPortPos.Y) ||
                            (TailDirection == Dock.Left && HeadDirection == Dock.Bottom && HeadPortPos.X < TailPortPos.X && HeadPortPos.Y < TailPortPos.Y)
                            ))
                        {
                            PxInterPts[0] = new Point(HeadPortPos.X, TailPortPos.Y);
                        }
                        else
                        {
                            bool isReset = false;

                            if (!(PxInterPts.Count >= 3))
                            {
                                isReset = true;
                                while (PxInterPts.Count < 3)
                                {
                                    PxInterPts.Add(new Point(0, 0));
                                }
                            }
                            bool isNewPtsAdded = false;
                            bool isAddedAtHead = false;
                            if (PxInterPts.Count % 2 == 0)
                            {
                                isNewPtsAdded = true;
                                //// If Vertical start
                                if (PxInterPts[0].Y == PxInterPts[1].Y)
                                {
                                    //// If Horizontal port connection
                                    if (HeadDirection == Dock.Left || HeadDirection == Dock.Right)
                                    {
                                        PxInterPts.Insert(0, new Point(0, 0));
                                        isAddedAtHead = true;
                                    }
                                    else
                                    {
                                        PxInterPts.Insert(PxInterPts.Count - 1, new Point(0, 0));
                                    }
                                }
                                else
                                {
                                    //If Horizontal port connection
                                    if (HeadDirection == Dock.Top || HeadDirection == Dock.Bottom)
                                    {
                                        PxInterPts.Insert(0, new Point(0, 0));
                                        isAddedAtHead = true;
                                    }
                                    else
                                    {
                                        PxInterPts.Insert(PxInterPts.Count - 1, new Point(0, 0));
                                    }
                                }
                            }

                        #endregion

                            #region Head region
                            if (HeadDirection == Dock.Left)
                            {
                                if (isHeadRecal || (PxInterPts[0].X > HeadNode.Left) || (isNewPtsAdded && isAddedAtHead) || isReset)
                                {
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X - headDist, ConnectionHeadPort.PagePosition.Y);
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                                else
                                {
                                    PxInterPts[0] = new Point(PxInterPts[0].X, ConnectionHeadPort.PagePosition.Y);
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                            }
                            else if (HeadDirection == Dock.Right)
                            {
                                if (isHeadRecal || (PxInterPts[0].X < HeadNode.Right) || (isNewPtsAdded && isAddedAtHead) || isReset)
                                {
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X + headDist, ConnectionHeadPort.PagePosition.Y);
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                                else
                                {
                                    PxInterPts[0] = new Point(PxInterPts[0].X, ConnectionHeadPort.PagePosition.Y);
                                    PxInterPts[1] = new Point(PxInterPts[0].X, PxInterPts[1].Y);
                                }
                            }
                            else if (HeadDirection == Dock.Top)
                            {
                                if (isHeadRecal || (PxInterPts[0].Y > HeadNode.Top) || (isNewPtsAdded && isAddedAtHead) || isReset)
                                {
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y - headDist);
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                                else
                                {
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, PxInterPts[0].Y);
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                            }
                            else if (HeadDirection == Dock.Bottom)
                            {
                                if (isHeadRecal || (PxInterPts[0].Y < HeadNode.Bottom) || (isNewPtsAdded && isAddedAtHead) || isReset)
                                {
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y + headDist);
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                                else
                                {
                                    PxInterPts[0] = new Point(ConnectionHeadPort.PagePosition.X, PxInterPts[0].Y);
                                    PxInterPts[1] = new Point(PxInterPts[1].X, PxInterPts[0].Y);
                                }
                            }
                            #endregion

                            #region Tail region
                            int last = PxInterPts.Count - 1;

                            if (TailDirection == Dock.Left)
                            {
                                if (isTailRecal || (PxInterPts[last].X > TailNode.Left) || (isNewPtsAdded && !isAddedAtHead) || isReset)
                                {
                                    PxInterPts[last] = new Point(ConnectionTailPort.PagePosition.X - tailDist, ConnectionTailPort.PagePosition.Y);
                                    PxInterPts[last - 1] = new Point(PxInterPts[last].X, PxInterPts[last - 1].Y);
                                }
                                else
                                {
                                    PxInterPts[last] = new Point(PxInterPts[last].X, ConnectionTailPort.PagePosition.Y);
                                    PxInterPts[last - 1] = new Point(PxInterPts[last].X, PxInterPts[last - 1].Y);
                                }
                            }
                            else if (TailDirection == Dock.Right)
                            {
                                if (isTailRecal || (PxInterPts[last].X < TailNode.Right) || (isNewPtsAdded && !isAddedAtHead) || isReset)
                                {
                                    PxInterPts[last] = new Point(ConnectionTailPort.PagePosition.X + tailDist, ConnectionTailPort.PagePosition.Y);
                                    PxInterPts[last - 1] = new Point(PxInterPts[last].X, PxInterPts[last - 1].Y);
                                }
                                else
                                {
                                    PxInterPts[last] = new Point(PxInterPts[last].X, ConnectionTailPort.PagePosition.Y);
                                    PxInterPts[last - 1] = new Point(PxInterPts[last].X, PxInterPts[last - 1].Y);
                                }
                            }
                            else if ((TailDirection == Dock.Top))
                            {
                                if (isTailRecal || (PxInterPts[last].Y > TailNode.Top) || (isNewPtsAdded && !isAddedAtHead) || isReset)
                                {
                                    PxInterPts[last] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y - tailDist);
                                    PxInterPts[last - 1] = new Point(PxInterPts[last - 2].X, PxInterPts[last].Y);
                                }
                                else
                                {
                                    PxInterPts[last] = new Point(ConnectionTailPort.PagePosition.X, PxInterPts[last].Y);
                                    PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, PxInterPts[last].Y);
                                }
                            }
                            else if ((TailDirection == Dock.Bottom))
                            {
                                if (isTailRecal || PxInterPts[last].Y < TailNode.Bottom || (isNewPtsAdded && !isAddedAtHead) || isReset)
                                {
                                    PxInterPts[last] = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y + tailDist);
                                    PxInterPts[last - 1] = new Point(PxInterPts[last - 2].X, PxInterPts[last].Y);
                                }
                                else
                                {
                                    PxInterPts[last] = new Point(ConnectionTailPort.PagePosition.X, PxInterPts[last].Y);
                                    PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, PxInterPts[last].Y);
                                }
                            }

                            #endregion
                        }
                    }
                }
                IntermediatePoints.Clear();
                foreach (Point pt in PxInterPts)
                {
                    IntermediatePoints.Add(pt);
                }
                connectionPoints.Clear();
                connectionPoints.Add(ConnectionHeadPort.PagePosition);
                connectionPoints.Add(ConnectionTailPort.PagePosition);
                return connectionPoints;
            }

            if (IntermediatePoints.Count <= 2)
            {
                if (IntermediatePoints.Count == 1)
                {
                    divideOrthognalLine();
                }
                else if (IntermediatePoints.Count == 2)
                {
                    defaultStyle();
                }
            }
            if (connectionPoints.Count == 2 && IntermediatePoints.Count >= 2)
            {
                Point tempEnd = connectionPoints[1];
                Point tempStart = connectionPoints[0];
                if (IntermediatePoints.Count > 2)
                {
                    if (Math.Abs(IntermediatePoints[1].X - IntermediatePoints[0].X)
                        < Math.Abs(IntermediatePoints[1].Y - IntermediatePoints[0].Y))
                    {
                        if (tempStart.Y != 0)
                            IntermediatePoints[0] = new Point(IntermediatePoints[0].X, tempStart.Y);
                        if (HeadNode != null)
                        {
                            NodeInfo head = (HeadNode as Node).GetInfo();

                            if (head.Position.X - (cs * 2 + head.Size.Width / 2) < IntermediatePoints[0].X
                                && IntermediatePoints[0].X < head.Position.X + (cs * 2 + head.Size.Width / 2))
                            {
                                if (IntermediatePoints[1].Y > head.Position.Y)
                                {
                                    moveAway(connectionPoints[0], head.Position, true, true);
                                }
                                else
                                {
                                    moveAway(connectionPoints[0], head.Position, true, true);
                                }
                            }
                        }
                    }
                    else if (Math.Abs(IntermediatePoints[1].X - IntermediatePoints[0].X)
                        == Math.Abs(IntermediatePoints[1].Y - IntermediatePoints[0].Y))
                    {
                        if (Math.Abs(tempStart.X - IntermediatePoints[0].X)
                        < Math.Abs(tempStart.Y - IntermediatePoints[0].Y))
                        {
                            IntermediatePoints[0] = new Point(tempStart.X, IntermediatePoints[0].Y);
                        }
                        else
                        {
                            IntermediatePoints[0] = new Point(IntermediatePoints[0].X, tempStart.Y);
                        }

                    }
                    else
                    {
                        if (tempStart.X != 0)
                            IntermediatePoints[0] = new Point(tempStart.X, IntermediatePoints[0].Y);
                        if (HeadNode != null)
                        {
                            NodeInfo head = (HeadNode as Node).GetInfo();

                            if (head.Position.Y - (cs * 2 + head.Size.Height / 2) < IntermediatePoints[0].Y
                                && IntermediatePoints[0].Y < head.Position.Y + (cs * 2 + head.Size.Height / 2))
                            {
                                if (IntermediatePoints[1].X > head.Position.X)
                                {
                                    moveAway(connectionPoints[0], head.Position, false, true);
                                }
                                else
                                {
                                    moveAway(connectionPoints[0], head.Position, false, true);
                                }
                            }
                        }
                    }
                    if (Math.Abs(IntermediatePoints[IntermediatePoints.Count - 2].X - IntermediatePoints[IntermediatePoints.Count - 1].X)
                        < Math.Abs(IntermediatePoints[IntermediatePoints.Count - 2].Y - IntermediatePoints[IntermediatePoints.Count - 1].Y))
                    {

                        if (tempEnd.Y != 0)
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(IntermediatePoints[IntermediatePoints.Count - 1].X, tempEnd.Y);

                        if (TailNode != null)
                        {
                            NodeInfo tail = (TailNode as Node).GetInfo();

                            if (tail.Position.X - (cs * 2 + tail.Size.Width / 2) < IntermediatePoints[IntermediatePoints.Count - 1].X
                                && IntermediatePoints[IntermediatePoints.Count - 1].X < tail.Position.X + (cs * 2 + tail.Size.Width / 2))
                            {
                                if (IntermediatePoints[IntermediatePoints.Count - 2].Y > tail.Position.Y)
                                {
                                    moveAway(connectionPoints[1], tail.Position, true, false);
                                }
                                else
                                {
                                    moveAway(connectionPoints[1], tail.Position, true, false);
                                }
                            }
                        }
                    }
                    else if (Math.Abs(IntermediatePoints[IntermediatePoints.Count - 2].X - IntermediatePoints[IntermediatePoints.Count - 1].X)
                        == Math.Abs(IntermediatePoints[IntermediatePoints.Count - 2].Y - IntermediatePoints[IntermediatePoints.Count - 1].Y))
                    {
                        if (Math.Abs(tempEnd.X - IntermediatePoints[IntermediatePoints.Count - 1].X)
                        < Math.Abs(tempEnd.Y - IntermediatePoints[IntermediatePoints.Count - 1].Y))
                        {
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(tempEnd.X, IntermediatePoints[IntermediatePoints.Count - 1].Y);
                        }
                        else
                        {
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(IntermediatePoints[IntermediatePoints.Count - 1].X, tempEnd.Y);
                        }
                    }
                    else
                    {
                        if (tempEnd.X != 0)
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(tempEnd.X, IntermediatePoints[IntermediatePoints.Count - 1].Y);

                        if (TailNode != null)
                        {
                            NodeInfo tail = (TailNode as Node).GetInfo();

                            if (tail.Position.Y - (cs * 2 + tail.Size.Height / 2) < IntermediatePoints[IntermediatePoints.Count - 1].Y
                                && IntermediatePoints[IntermediatePoints.Count - 1].Y < tail.Position.Y + (cs * 2 + tail.Size.Height / 2))
                            {
                                if (IntermediatePoints[IntermediatePoints.Count - 2].X > tail.Position.X)
                                {
                                    moveAway(connectionPoints[1], tail.Position, false, false);
                                }
                                else
                                {
                                    moveAway(connectionPoints[1], tail.Position, false, false);
                                }
                            }
                        }
                    }
                }

                else if (IntermediatePoints.Count == 2)
                {
                    //List<Point> sten = defaultStyle();
                    //connectionPoints = GetTerminalPoints();
                }
                ////only one intermediate point
                else
                {
                    if (Math.Abs(tempEnd.X - IntermediatePoints[0].X)
                        < Math.Abs(tempEnd.Y - IntermediatePoints[0].Y))
                    {
                        if (tempStart.Y != 0)
                            IntermediatePoints[0] = new Point(IntermediatePoints[0].X, tempStart.Y);
                    }
                    else if (Math.Abs(tempEnd.X - IntermediatePoints[0].X)
                        == Math.Abs(tempEnd.Y - IntermediatePoints[0].Y))
                    {
                        if (Math.Abs(tempStart.X - IntermediatePoints[IntermediatePoints.Count - 1].X)
                        < Math.Abs(tempStart.Y - IntermediatePoints[IntermediatePoints.Count - 1].Y))
                        {
                            tempStart.X++;
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(IntermediatePoints[IntermediatePoints.Count - 1].X + 1, IntermediatePoints[IntermediatePoints.Count - 1].Y);
                        }
                        else if (Math.Abs(tempStart.X - IntermediatePoints[IntermediatePoints.Count - 1].X)
                        == Math.Abs(tempStart.Y - IntermediatePoints[IntermediatePoints.Count - 1].Y))
                        {
                        }
                        else
                        {
                            tempStart.X--;
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(IntermediatePoints[IntermediatePoints.Count - 1].X, IntermediatePoints[IntermediatePoints.Count - 1].Y - 1);
                        }
                    }
                    else
                    {
                        if (tempStart.X != 0)
                            IntermediatePoints[0] = new Point(tempStart.X, IntermediatePoints[0].Y);
                    }

                    if (Math.Abs(tempStart.X - IntermediatePoints[IntermediatePoints.Count - 1].X)
                        < Math.Abs(tempStart.Y - IntermediatePoints[IntermediatePoints.Count - 1].Y))
                    {

                        if (tempEnd.Y != 0)
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(IntermediatePoints[IntermediatePoints.Count - 1].X, tempEnd.Y);
                    }
                    else if (Math.Abs(tempStart.X - IntermediatePoints[IntermediatePoints.Count - 1].X)
                        == Math.Abs(tempStart.Y - IntermediatePoints[IntermediatePoints.Count - 1].Y))
                    {
                        if (Math.Abs(tempEnd.X - IntermediatePoints[0].X)
                            < Math.Abs(tempEnd.Y - IntermediatePoints[0].Y))
                        {
                            tempEnd.X++;
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(IntermediatePoints[IntermediatePoints.Count - 1].X + 1, IntermediatePoints[IntermediatePoints.Count - 1].Y);
                        }
                        else if (Math.Abs(tempEnd.X - IntermediatePoints[0].X)
                            == Math.Abs(tempEnd.Y - IntermediatePoints[0].Y))
                        {
                        }
                        else
                        {
                            tempEnd.X--;
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(IntermediatePoints[IntermediatePoints.Count - 1].X, IntermediatePoints[IntermediatePoints.Count - 1].Y - 1);
                        }
                    }
                    else
                    {
                        if (tempEnd.X != 0)
                            IntermediatePoints[IntermediatePoints.Count - 1] = new Point(tempEnd.X, IntermediatePoints[IntermediatePoints.Count - 1].Y);
                    }
                }
            }
            else
            {
            }
            return connectionPoints;
        }

        private void divideOrthognalLine()
        {
            List<Point> terms = new List<Point>();
            Point ret = new Point(0, 0);

            if (HeadNode == null && TailNode == null)
            {
                if (Orientation == TreeOrientation.TopBottom)
                {
                    ret = new Point(PxStartPointPosition.X, PxEndPointPosition.Y);
                }
                else
                {
                    ret = new Point(PxEndPointPosition.X, PxStartPointPosition.Y);
                }
            }
            NodeInfo head = new NodeInfo();
            NodeInfo tail = new NodeInfo();
            if (HeadNode == null)
            {
                head.Position = PxStartPointPosition;
            }
            else
            {
                head = (HeadNode as Node).GetInfo();
            }

            if (TailNode == null)
            {
                tail.Position = PxEndPointPosition;
            }
            else
            {
                tail = (TailNode as Node).GetInfo();
            }

            Point actualHeadPos = head.Position;
            Point actualTailPos = tail.Position;

            if (dview != null && dview.Page != null)
            {
                if (this.ConnectionHeadPort != null && HeadNode != null)
                {
                    double width = double.IsNaN(ConnectionHeadPort.Width) ? 2.5 : ConnectionHeadPort.Width / 2;
                    double height = double.IsNaN(ConnectionHeadPort.Height) ? 2.5 : ConnectionHeadPort.Height / 2;

                    head.Position = new Point(ConnectionHeadPort.PxCenterPosition.X + head.Left + width, height + ConnectionHeadPort.PxCenterPosition.Y + head.Top);
                }
                if (this.ConnectionTailPort != null && TailNode != null)
                {
                    double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                    double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                    tail.Position = new Point(ConnectionTailPort.PxCenterPosition.X + tail.Left + width, height + ConnectionTailPort.PxCenterPosition.Y + tail.Top);
                }
            }

            Point hor = new Point(tail.Position.X, head.Position.Y);
            Point ver = new Point(head.Position.X, tail.Position.Y);

            if (HeadNode != null)
            {
                if (divideOrthoganalLine(HeadNode, TailNode, ConnectionHeadPort, ConnectionTailPort, head, tail, hor, ver))
                {
                    ret = hor;
                }
                else
                {
                    ret = ver;
                }
            }
            else if (TailNode != null)
            {
                if (divideOrthoganalLine(TailNode, HeadNode, ConnectionTailPort, ConnectionHeadPort, tail, head, hor, ver))
                {
                    ret = hor;
                }
                else
                {
                    ret = ver;
                }
            }

            IntermediatePoints[0] = ret;
        }

        private bool divideOrthoganalLine(IShape HeadNode, IShape TailNode, ConnectionPort ConnectionHeadPort, ConnectionPort ConnectionTailPort, NodeInfo head, NodeInfo tail, Point hor, Point ver)
        {
            bool Horizontal = true;
            if (HeadNode != null && ConnectionHeadPort != null)
            {
                //// Hor
                if (Math.Min(Math.Abs(head.Position.Y - head.Top), Math.Abs(head.Position.Y - (head.Top + head.Size.Height))) >
                    Math.Min(Math.Abs(head.Position.X - head.Left), Math.Abs(head.Position.X - (head.Left + head.Size.Width))))
                {
                    //// Right
                    if (Math.Abs(head.Position.X - head.Left) > Math.Abs(head.Position.X - (head.Left + head.Size.Width)))
                    {
                        if (hor.X > head.Position.X)
                        {
                            Horizontal = true;
                        }
                        else
                        {
                            Horizontal = false;
                        }
                    }
                    //// Left
                    else
                    {
                        if (hor.X < head.Position.X)
                        {
                            Horizontal = true;
                        }
                        else
                        {
                            Horizontal = false;
                        }
                    }
                }
                //// Vertical
                else
                {
                    //// Down
                    if (Math.Abs(head.Position.Y - head.Top) > Math.Abs(head.Position.Y - (head.Top + head.Size.Height)))
                    {
                        if (ver.Y > head.Position.Y)
                        {
                            Horizontal = false;
                        }
                        else
                        {
                            Horizontal = true;
                        }
                    }
                    //// Up
                    else
                    {
                        if (ver.Y < head.Position.Y)
                        {
                            Horizontal = false;
                        }
                        else
                        {
                            Horizontal = true;
                        }
                    }
                }
            }
            else if (HeadNode != null)
            {
                if (TailNode != null && ConnectionTailPort != null)
                {
                    if ((head.BottomPoint.Y < tail.Position.Y) || (head.TopPoint.Y > tail.Position.Y))
                    {
                        Horizontal = false;
                    }
                    else
                    {
                        Horizontal = true;
                    }
                }
                else if (TailNode != null)
                {
                    //// Bottom
                    if (head.BottomPoint.Y < tail.TopPoint.Y)
                    {
                        Horizontal = false;
                    }
                    //// Top
                    else if (head.TopPoint.Y > tail.BottomPoint.Y)
                    {
                        Horizontal = false;
                    }
                    //// Right Paralled
                    else if (head.RightPoint.X < tail.LeftPoint.X)
                    {
                        Horizontal = true;
                    }
                    //// Left Parallel
                    else if (head.LeftPoint.X > tail.RightPoint.X)
                    {
                        Horizontal = true;
                    }
                    //// Overlap
                    else
                    {
                        Horizontal = true;
                    }
                }
                else
                {
                    if ((head.BottomPoint.Y < tail.Position.Y) || (head.TopPoint.Y > tail.Position.Y))
                    {
                        Horizontal = false;
                    }
                    else
                    {
                        Horizontal = true;
                    }
                }
            }
            return Horizontal;
        }

        private Point moveAway(Point mov, Point fix, bool x, bool head)
        {
            double del = 20;
            if (x)
            {
                if (head)
                {
                    if ((fix.X - IntermediatePoints[0].X) < 0)
                    {
                        mov = new Point(mov.X + 2 * (fix.X - mov.X), mov.Y);
                        IntermediatePoints[0] = new Point(mov.X - del, IntermediatePoints[0].Y);
                        IntermediatePoints[1] = new Point(mov.X - del, IntermediatePoints[1].Y);
                    }
                    else
                    {
                        mov = new Point(mov.X + 2 * (fix.X - mov.X), mov.Y);
                        IntermediatePoints[0] = new Point(mov.X + del, IntermediatePoints[0].Y);
                        IntermediatePoints[1] = new Point(mov.X + del, IntermediatePoints[1].Y);
                    }
                }
                else
                {
                    int f = IntermediatePoints.Count - 1;
                    if ((fix.X - IntermediatePoints[f].X) < 0)
                    {
                        mov = new Point(mov.X + 2 * (fix.X - mov.X), mov.Y);
                        IntermediatePoints[f] = new Point(mov.X - del, IntermediatePoints[f].Y);
                        IntermediatePoints[f - 1] = new Point(mov.X - del, IntermediatePoints[f - 1].Y);
                    }
                    else
                    {
                        mov = new Point(mov.X + 2 * (fix.X - mov.X), mov.Y);
                        IntermediatePoints[f] = new Point(mov.X + del, IntermediatePoints[f].Y);
                        IntermediatePoints[f - 1] = new Point(mov.X + del, IntermediatePoints[f - 1].Y);
                    }
                }
            }
            else
            {
                if (head)
                {
                    if ((fix.Y - IntermediatePoints[0].Y) < 0)
                    {
                        mov = new Point(mov.X, mov.Y + 2 * (fix.Y - mov.Y));
                        IntermediatePoints[0] = new Point(IntermediatePoints[0].X, mov.Y - del);
                        IntermediatePoints[1] = new Point(IntermediatePoints[1].X, mov.Y - del);
                    }
                    else
                    {
                        mov = new Point(mov.X, mov.Y + 2 * (fix.Y - mov.Y));
                        IntermediatePoints[0] = new Point(IntermediatePoints[0].X, mov.Y + del);
                        IntermediatePoints[1] = new Point(IntermediatePoints[1].X, mov.Y + del);
                    }
                }
                else
                {
                    int f = IntermediatePoints.Count - 1;
                    if ((fix.Y - IntermediatePoints[f].Y) < 0)
                    {
                        mov = new Point(mov.X, mov.Y + 2 * (fix.Y - mov.Y));
                        IntermediatePoints[f] = new Point(IntermediatePoints[f].X, mov.Y - del);
                        IntermediatePoints[f - 1] = new Point(IntermediatePoints[f - 1].X, mov.Y - del);
                    }
                    else
                    {
                        mov = new Point(mov.X, mov.Y + 2 * (fix.Y - mov.Y));
                        IntermediatePoints[f] = new Point(IntermediatePoints[f].X, mov.Y + del);
                        IntermediatePoints[f - 1] = new Point(IntermediatePoints[f - 1].X, mov.Y + del);
                    }
                }
            }
            return mov;
        }

        private void defaultStyle()
        {
            double fifty = 50;
            double thirty = 30;
            List<Point> terms = new List<Point>();

            if (HeadNode != null && TailNode != null)
            {
                NodeInfo head = (HeadNode as Node).GetInfo();
                NodeInfo tail = (TailNode as Node).GetInfo();

                Point actualHeadPos = head.Position;
                Point actualTailPos = tail.Position;

                if (dview != null && dview.Page != null)
                {
                    if (this.ConnectionHeadPort != null && HeadNode != null)
                    {
                        double width = double.IsNaN(ConnectionHeadPort.Width) ? 2.5 : ConnectionHeadPort.Width / 2;
                        double height = double.IsNaN(ConnectionHeadPort.Height) ? 2.5 : ConnectionHeadPort.Height / 2;

                        head.Position = new Point(ConnectionHeadPort.PxCenterPosition.X + head.Left + width, height + ConnectionHeadPort.PxCenterPosition.Y + head.Top);
                    }
                    if (this.ConnectionTailPort != null && TailNode != null)
                    {
                        double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                        double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                        tail.Position = new Point(ConnectionTailPort.PxCenterPosition.X + tail.Left + width, height + ConnectionTailPort.PxCenterPosition.Y + tail.Top);
                    }
                }
                ////Top-Bottom
                if ((Math.Abs(head.Position.Y - tail.Position.Y) > head.Size.Height / 2 + tail.Size.Height / 2 + fifty)
                    && (
                    Orientation == TreeOrientation.TopBottom || Orientation == TreeOrientation.BottomTop))
                {
                    ////Top-to-Bottom
                    if (head.Position.Y - tail.Position.Y < 0)
                    {
                        IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y + head.Size.Height / 2 + thirty);
                        IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                    }
                    ////Bottom-to-Top
                    else
                    {
                        IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y - head.Size.Height / 2 - thirty);
                        IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                    }
                }
                ////Right-Left
                else
                {
                    ////Left-to-Right
                    if (head.Position.X - tail.Position.X < 0)
                    {
                        IntermediatePoints[0] = new Point(actualHeadPos.X + head.Size.Width / 2 + thirty, head.Position.Y);
                        IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                        if (Math.Abs((head.Position.X + head.Size.Width / 2) - (tail.Position.X - tail.Size.Width / 2)) < fifty
                            || ((head.Position.X + head.Size.Width / 2) - (tail.Position.X - tail.Size.Width / 2) > 0)
                            )
                        {
                            IntermediatePoints[0] = new Point(
                                                                actualTailPos.X + tail.Size.Width / 2 + fifty, head.Position.Y);
                            IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                        }
                    }
                    ////Right-to-Left
                    else
                    {
                        IntermediatePoints[0] = new Point(actualHeadPos.X - head.Size.Width / 2 - thirty, head.Position.Y);
                        IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);

                        if (Math.Abs((head.Position.X - head.Size.Width / 2) - (tail.Position.X + tail.Size.Width / 2)) < fifty
                            || ((head.Position.X - head.Size.Width / 2) - (tail.Position.X + tail.Size.Width / 2) < 0)
                            )
                        {
                            IntermediatePoints[0] = new Point(
                                                                actualTailPos.X - tail.Size.Width / 2 - fifty, head.Position.Y);
                            IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                        }
                    }
                }
            }
            else if (HeadNode != null && TailNode == null)
            {
                NodeInfo head = (HeadNode as Node).GetInfo();
                Point actualHeadPos = head.Position;
                if (dview != null && dview.Page != null)
                {
                    if (this.ConnectionHeadPort != null && HeadNode != null)
                    {
                        double width = double.IsNaN(ConnectionHeadPort.Width) ? 2.5 : ConnectionHeadPort.Width / 2;
                        double height = double.IsNaN(ConnectionHeadPort.Height) ? 2.5 : ConnectionHeadPort.Height / 2;

                        head.Position = new Point(ConnectionHeadPort.PxCenterPosition.X + head.Left + width, height + ConnectionHeadPort.PxCenterPosition.Y + head.Top);
                    }
                }
                Point tail = PxEndPointPosition;
                ////Top-Bottom
                if (Math.Abs(head.Position.Y - tail.Y) > head.Size.Height / 2 + fifty)
                {
                    ////Top-to-Bottom
                    if (head.Position.Y - tail.Y < 0)
                    {
                        IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y + head.Size.Height / 2 + thirty);
                        IntermediatePoints[1] = new Point(tail.X, IntermediatePoints[0].Y);
                    }
                    ////Bottom-to-Top
                    else
                    {
                        IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y - head.Size.Height / 2 - thirty);
                        IntermediatePoints[1] = new Point(tail.X, IntermediatePoints[0].Y);
                    }
                }
                ////Right-Left
                else
                {
                    ////Left-to-Right
                    if (head.Position.X - tail.X < 0)
                    {
                        IntermediatePoints[0] = new Point(actualHeadPos.X + head.Size.Width / 2 + thirty, head.Position.Y);
                        IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Y);
                    }
                    ////Right-to-Left
                    else
                    {
                        IntermediatePoints[0] = new Point(actualHeadPos.X - head.Size.Width / 2 - thirty, head.Position.Y);
                        IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Y);
                    }
                }
            }
            else if (HeadNode == null && TailNode != null)
            {
                NodeInfo tail = (TailNode as Node).GetInfo();
                Point actualTailPos = tail.Position;
                if (dview != null && dview.Page != null)
                {
                    if (this.ConnectionTailPort != null && TailNode != null)
                    {
                        double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                        double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                        tail.Position = new Point(ConnectionTailPort.PxCenterPosition.X + tail.Left + width, height + ConnectionTailPort.PxCenterPosition.Y + tail.Top);
                    }
                }
                Point head = PxStartPointPosition;
                ////Top-Bottom
                if (Math.Abs(head.Y - tail.Position.Y) > tail.Size.Height / 2 + fifty)
                {
                    ////Top-to-Bottom
                    if (head.Y - tail.Position.Y < 0)
                    {
                        IntermediatePoints[0] = new Point(head.X, head.Y + thirty);
                        IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                    }
                    ////Bottom-to-Top
                    else
                    {
                        IntermediatePoints[0] = new Point(head.X, head.Y - thirty);
                        IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                    }
                }
                ////Right-Left
                else
                {
                    ////Left-to-Right
                    if (head.X - tail.Position.X < 0)
                    {
                        IntermediatePoints[0] = new Point(head.X + thirty, head.Y);
                        IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                    }
                    ////Right-to-Left
                    else
                    {
                        IntermediatePoints[0] = new Point(head.X - thirty, head.Y);
                        IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                    }
                }
            }
            else if (HeadNode == null && TailNode == null)
            {
                Point tail = PxEndPointPosition;
                Point head = PxStartPointPosition;
                ////Top-Bottom
                if (Math.Abs(head.Y - tail.Y) > fifty)
                {
                    ////Top-to-Bottom
                    if (head.Y - tail.Y < 0)
                    {
                        IntermediatePoints[0] = new Point(head.X, head.Y + thirty);
                        IntermediatePoints[1] = new Point(tail.X, IntermediatePoints[0].Y);
                    }
                    ////Bottom-to-Top
                    else
                    {
                        IntermediatePoints[0] = new Point(head.X, head.Y - thirty);
                        IntermediatePoints[1] = new Point(tail.X, IntermediatePoints[0].Y);
                    }
                }
                ////Right-Left
                else
                {
                    ////Left-to-Right
                    if (head.X - tail.X < 0)
                    {
                        IntermediatePoints[0] = new Point(head.X + thirty, head.Y);
                        IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Y);
                    }
                    ////Right-to-Left
                    else
                    {
                        IntermediatePoints[0] = new Point(head.X - thirty, head.Y);
                        IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Y);
                    }
                }
            }
        }

        /// <summary>
        /// Hides the adorner.
        /// </summary>
        protected override void HideAdorner()
        {
        }

        UIElement hitobject;
        internal ConnectionPort hitport; // To store port on which mouse over
        /// <summary>
        /// Hittesting 
        /// </summary>
        /// <param name="hitPoint">hit point</param>
        /// <returns>Returns the boolean value whether the hit testing is performed</returns>
        
        internal bool HitTesting(Point hitPoint)
        {
            IEnumerable<UIElement> hitObjectcoll = VisualTreeHelper.FindElementsInHostCoordinates(hitPoint, this.dview);
            if (hitObjectcoll.Count() > 0)
            {
                foreach (UIElement hitObject in hitObjectcoll)
                {
                    if (hitObject is ConnectionPort)
                    {
                        ConnectionPort port = hitObject as ConnectionPort;
                        this.hitNodeConnector = (hitObject as ConnectionPort).Node;
                        if (port.CenterPortReferenceNo!=0)
                        {
                            hitport = port;
                            //this.hitNodeConnector = null;
                            //this.previousHitNode = null;
                            hitobject = port;
                            if (this.headthumb)
                            {
                                this.ConnectionHeadPort = port;
                            }
                            else
                            {
                                this.ConnectionTailPort = port;
                            }
                        }
                        else
                        {
                            if (this.headthumb)
                            {
                                this.ConnectionHeadPort = null;
                                //this.HeadNode = port.Node;
                            }
                            else
                            {
                                this.ConnectionTailPort = null;
                                //this.TailNode = port.Node;
                            }
                        }

                        if (this.hitNodeConnector != this.fixedNodeConnection)
                        {
                            port.IsDragOverPort = true;
                        }
                        foundport = true;
                        return true;
                    }
                    else if (hitObject is Node)
                    {
                        //hitobject = hitObject;
                        Node node = hitObject as Node;
                        this.hitNodeConnector = hitObject as Node;
                        if (this.headthumb)
                        {
                            this.ConnectionHeadPort = null;
                        }
                        else
                        {
                            this.ConnectionTailPort = null;
                        }

                        this.previousHitNode = hitObject as Node;

                        if (this.hitNodeConnector != this.fixedNodeConnection)
                        {
                            this.previousHitNode.IsDragConnectionOver = true;
                            this.hitNodeConnector.IsDragConnectionOver = true;
                        }
                        foundport = false;
                        return true;
                    }

                }
            }

            if (this.previousHitNode != null)
            {
                this.previousHitNode.IsDragConnectionOver = false;
            }
            this.hitNodeConnector = null;
            this.hitport = null;
            return false;
        }

        /// <summary>
        /// Invoked when Label editing is started.
        /// </summary>
        public void Labeledit()
        {
            if (IsLabelEditable)
            {
            }
        }

        /// <summary>
        /// Calls Line_PropertyChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void Line_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Position"))
            {
                this.UpdateConnectorPathGeometry();
            }
        }

        /// <summary>
        /// Handles the Loaded event of the LineConnector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void LineConnector_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.HeadNode != null && this.ConnectionHeadPort != null)
            {
                this.HeadPortReferenceNo = this.ConnectionHeadPort.PortReferenceNo;
            }
            if (this.TailNode != null && this.ConnectionTailPort != null)
            {
                this.TailPortReferenceNo = this.ConnectionTailPort.PortReferenceNo;
            }
            dc = DiagramPage.GetDiagramControl(this);
            if (this.LabelWidth == 0)
            {
                //this.LabelWidth = Distance;
                this.isdefaulted = true;
            }
            DiagramControl diagramCtrl = DiagramPage.GetDiagramControl(this);
            if (this.IsSelected)
            {
                if (this.IsGrouped)
                {
                    this.IsSelected = false;
                }
                if (!diagramCtrl.View.SelectionList.Contains(this))
                {
                    diagramCtrl.View.SelectionList.Add(this);
                    ConnectorRoutedEventArgs newEventArgs1 = new ConnectorRoutedEventArgs(this);
                    diagramCtrl.View.OnConnectorSelected(this as LineConnector, newEventArgs1);
                    diagramCtrl.View.oldselectionlist.Add(this);
                }
                else if (this.IsSelected)
                {
                    ConnectorRoutedEventArgs newEventArgs1 = new ConnectorRoutedEventArgs(this);
                    diagramCtrl.View.OnConnectorSelected(this as LineConnector, newEventArgs1);
                    diagramCtrl.View.oldselectionlist.Add(this);
                }
            }
            if (this.IsSelected && !dc.View.SelectionList.Contains(this))
            {
                if (this.IsGrouped)
                {
                    this.IsSelected = false;
                }
                dc.View.SelectionList.Add(this);
                ConnectorRoutedEventArgs newEventArgs1 = new ConnectorRoutedEventArgs(this);
                dc.View.OnConnectorSelected(this as LineConnector, newEventArgs1);
            }

            this.UpdateConnectorPathGeometry();
            if (this.HeadNode != null && this.TailNode != null)
            {
                this.HeadNodeReferenceNo = (this.HeadNode as Node).ReferenceNo;
                this.TailNodeReferenceNo = (this.TailNode as Node).ReferenceNo;
            }
            this.dview = Node.GetDiagramView(this);
            if (this.dview != null)
            {
                (this.dview.Page as DiagramPage).IsConnectorDropped = false;
                (this.dview.Page as DiagramPage).IsDiagrampageLoaded = false;
            }
            if (this.dview != null && (!this.dview.IsPageEditable))
            {
                DiagramView.PageEdit = false;
                IsLabelEditable = false;
            }
        }

        /// <summary>
        /// Called when the mouse button is clicked twice.
        /// </summary>
        /// <param name="position">Mouse Position</param>
        /// <returns>true if double clicked, false otherwise</returns>
        public bool IsDoubleClick(Point position)
        {
            if (((DateTime.Now.Subtract(this.lastLineClick).TotalMilliseconds < 500) && (Math.Abs((double)(this.lastLinePoint.X - position.X)) <= 2)) && (Math.Abs((double)(this.lastLinePoint.Y - position.Y)) <= 2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Provides class handling for the MouseLeftButtonDown routed event that occurs when
        /// the mouse left button is released over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
        void LineConnector_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dview.mnodeDownEvent = false;
            mouseup = true;

            if (this.dc != null  && !this.IsGrouped)
            {
                DiagramPage diagramPanel = this.dc.View.Page as DiagramPage;

                if (this.IsDoubleClick(e.GetPosition(diagramPanel as Panel)))
                {
                    ConnRoutedEventArgs newEventArgs = new ConnRoutedEventArgs(this);
                    dview.OnConnectorDoubleClick(this, newEventArgs);
                    if (this.dc.View.IsPageEditable && this.IsLabelEditable)
                    {
                        LabelEditConnRoutedEventArgs newEventArgs1 = new LabelEditConnRoutedEventArgs(this.linelabel.Text, this);
                        dview.OnConnectorStartLabelEdit(this, newEventArgs1);
                        this.linelabel.Visibility = Visibility.Collapsed;
                        this.linetext.Visibility = Visibility.Visible;
                        this.linetext.SelectionBackground = new SolidColorBrush(Colors.Blue);
                        this.linetext.Focus();
                        this.linetext.SelectAll();
                        this.LostFocus += new RoutedEventHandler(Node_LostFocus);
                    }
                }
                else
                {
                    if (diagramPanel != null)
                    {
                        if (this.dc.View.IsPageEditable)
                        {
                            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == (ModifierKeys.Shift | ModifierKeys.Control)
                            && ((this.ConnectorType == ConnectorType.Orthogonal) || (this.ConnectorType == ConnectorType.Straight)))
                            {
                                //this.Cursor = Cursors.Hand;
                                Point t = e.GetPosition(this);
                                InsertIntermediatePoint(t);
                                UpdateConnectorPathGeometry();

                                //if (LineAdorner == null)
                                //{
                                //    ShowAdorner();
                                //    HideAdorner();
                                //}
                                diagramPanel.SelectionList.Clear();
                                diagramPanel.SelectionList.Add(this);

                                this.InvalidateVertexs(this);
                            }
                            else if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                            {
                                if (this.IsSelected)
                                {
                                    diagramPanel.SelectionList.Remove(this);
                                }
                                else
                                {
                                    diagramPanel.SelectionList.Add(this);
                                }
                            }
                            else if (!this.IsSelected)
                            {
                                diagramPanel.SelectionList.Clear();
                                diagramPanel.SelectionList.Add(this);
                            }
                        }
                        this.lastLineClick = DateTime.Now;
                        this.lastLinePoint = e.GetPosition(diagramPanel as Panel);
                    }
                }
            }
        }

        internal Rect GetBounds()
        {
            if (!invalid)
            {
                Point Min = this.PxStartPointPosition;
                Point Max = this.PxEndPointPosition;
                if (this.IntermediatePoints != null)
                    foreach (Point point in this.IntermediatePoints)
                    {
                        if (point.X < Min.X)
                            Min.X = point.X;
                        if (point.Y < Min.Y)
                            Min.Y = point.Y;
                        if (point.X > Max.X)
                            Max.X = point.X;
                        if (point.Y > Max.Y)
                            Max.Y = point.Y;
                    }
                return new Rect(Min, Max);
            }
            if (this.ConnectorPathGeometry != null && this.ConnectorPathGeometry.Bounds != Rect.Empty)
            {
                return this.ConnectorPathGeometry.Bounds;
            }
            else
                return Rect.Empty;
        }

        internal void UpdateVertexsPosition(LineConnector lc)
        {
            InterPts = lc.IntermediatePoints;
            int i = 0;

            if (!deletingMode)
            {
                if (lc.IntermediatePoints != null && Vertexs.Count == lc.IntermediatePoints.Count)
                {
                    foreach (Thumb Vertex in Vertexs)
                    {
                        if (InterPts != null && InterPts.Count > i)
                        {
                            Canvas.SetLeft(Vertex, InterPts[i].X);
                            Canvas.SetTop(Vertex, InterPts[i].Y);
                        }
                        i++;
                    }
                }
                else
                {
                    InvalidateVertexs(lc);
                }
            }
        }

        internal void InvalidateVertexs(LineConnector lineconnector)
        {
            foreach (Thumb x in Vertexs)
            {
                this.vertexItems.Items.Remove(x);
            }

            if (Vertexs.Count > 0)
            {
                Vertexs.Clear();
            }

            if (lineconnector.IntermediatePoints != null)
            {
                int i = 1;
                InterPts = lineconnector.IntermediatePoints;
                foreach (Point v in lineconnector.IntermediatePoints)
                {
                    Vertex = AddVertex(i);

                    Canvas.SetLeft(Vertex, v.X);
                    Canvas.SetTop(Vertex, v.Y);

                    if (this.vertexItems != null)
                    {
                        if (!this.vertexItems.Items.Contains(Vertex))
                        {
                            this.vertexItems.Items.Add(Vertex);
                        }
                    }

                    if (!lineconnector.IsVertexVisible || lineconnector.ConnectorType == ConnectorType.Bezier || lineconnector.ConnectorType == ConnectorType.Arc || lineconnector.LineRoutingEnabled)
                        Vertex.Visibility = Visibility.Collapsed;

                    if (lineconnector.IsVertexMovable)
                    {
                        Vertex.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
                        Vertex.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
                        Vertex.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
                        Vertex.MouseLeftButtonUp += new MouseButtonEventHandler(Thumb_MouseLeftButtonUp);
                    }
                    Vertex.MouseMove += new MouseEventHandler(Vertex_MouseMove);
                    Vertex.SizeChanged += new SizeChangedEventHandler(Vertex_SizeChanged);
                    Vertexs.Add(Vertex);
                    i++;
                }
            }
        }

        private bool checkDecoratorMovaable(object sender)
        {
            if (this != null && sender != null && sender is Thumb && !(((Thumb)sender).Name.Contains("Vertex")))
            {
                if (!(this as LineConnector).IsDecoratorMovable)
                {
                    return true;
                }
            }
            return false;
        }

        void Vertex_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LineConnector lineconnector = this as LineConnector;

            foreach (Thumb vertex in Vertexs)
            {
                if (lineconnector != null)
                {
                    RotateTransform vtr = new RotateTransform();
                    vtr.Angle = 45;
                    vtr.CenterX = 0;
                    vtr.CenterY = 0;
                    TranslateTransform vtt = new TranslateTransform();
                    vtt.X = -vertex.Width / 2;
                    vtt.Y = -vertex.Height / 2;
                    TransformGroup vtg = new TransformGroup();
                    vtg.Children.Add(vtt);
                    vtg.Children.Add(vtr);
                    vertex.RenderTransform = vtg;
                }
            }
        }

        internal bool isClicked;
        private bool dragged = false;
        private bool deletingMode = false;
        internal bool isMouseup;
        private ConnectionPort centerport = null;
        private bool centerhit = false;

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            LineConnector lineconnector = this as LineConnector;

            if (sender is Thumb)
            {
                if ((sender as Thumb).Name.Contains("PART_HeadBorder"))
                {
                    this.headthumb = true;
                }
                else if ((sender as Thumb).Name.Contains("PART_TailBorder"))
                {
                    this.tailthumb = true;
                }

                this.linedragging = true;
                this.CaptureMouse();
            }
            else
            {
                this.linedragging = false;
            }

            lineconnector.positionchange = true;
            if (checkDecoratorMovaable(sender))
            {
                return;
            }
            lineconnector.isClicked = false;
            dragged = true;
            if (deletingMode == true)
            {
                return;
            }

            if (this.LineCanvas.Children.ElementAt(this.LineCanvas.Children.Count() - 1) is Path)
            {
                this.LineCanvas.Children.Remove(this.LineCanvas.Children.ElementAt(this.LineCanvas.Children.Count() - 1));
            }

            if (dc.View.IsPageEditable)
            {
                if (!dc.View.IsPanEnabled)
                {
                    //Point pos = new Point(Canvas.GetLeft(sender as Thumb) + e.HorizontalChange, Canvas.GetTop(sender as Thumb) + e.VerticalChange);
                    Point pos = pos1;
                    if (this.dc != null && this.dc.View != null)
                    {
                        if (dc.View.SnapToVerticalGrid)
                        {
                            pos.X = Node.Round(pos.X, dc.View.PxSnapOffsetX);
                        }
                        if (dc.View.SnapToHorizontalGrid)
                        {
                            pos.Y = Node.Round(pos.Y, dc.View.PxSnapOffsetY);
                        }
                    }
                    if (!this.isMouseup)
                    {
                        ConnDragRoutedEventArgs newEventArgs;
                        if (fixedNodeConnection != null && movableNodeConnection != null)
                        {
                            newEventArgs = new ConnDragRoutedEventArgs(fixedNodeConnection as Node, this.movableNodeConnection as Node, lineconnector);
                        }
                        else if (fixedNodeConnection == null && movableNodeConnection != null)
                        {
                            newEventArgs = new ConnDragRoutedEventArgs(this.movableNodeConnection as Node, lineconnector);
                        }
                        else if (fixedNodeConnection != null && movableNodeConnection == null)
                        {
                            newEventArgs = new ConnDragRoutedEventArgs(lineconnector, fixedNodeConnection as Node);
                        }
                        else
                        {
                            newEventArgs = new ConnDragRoutedEventArgs(lineconnector);
                        }

                        dview.OnConnectorDragStart(lineconnector, newEventArgs);

                        if (sender == headThumb)
                        {
                            lineconnector.hn = newEventArgs.MovableNodeEnd;
                            lineconnector.tn = newEventArgs.FixedNodeEnd;
                        }
                        else
                        {
                            lineconnector.tn = newEventArgs.MovableNodeEnd;
                            lineconnector.hn = newEventArgs.FixedNodeEnd;
                        }
                        this.isMouseup = true;
                    }

                    Point te = pos;

                    bool foundNode = this.HitTesting(te);
                    Thumb temp = (Thumb)sender;

                    Path path = new Path();
                    path.Stroke = this.LineStyle.Stroke;
                    path.StrokeThickness = this.LineStyle.StrokeThickness;
                    //m_StrokeDashArray = this.LineStyle.StrokeDashArray;

                    if (temp.Name.Contains("Vertex"))
                    {
                        foundNode = false;
                        centerhit = false;
                    }
                    if (foundNode && hitNodeConnector != null)
                    {
                        //pos = new Point(Canvas.GetLeft(sender as Thumb) + e.HorizontalChange, Canvas.GetTop(sender as Thumb) + e.VerticalChange);
                        if (this.dc != null && this.dc.View != null)
                        {
                            if (dc.View.SnapToVerticalGrid)
                            {
                                pos.X = Node.Round(pos.X, dc.View.PxSnapOffsetX);
                            }
                            if (dc.View.SnapToHorizontalGrid)
                            {
                                pos.Y = Node.Round(pos.Y, dc.View.PxSnapOffsetY);
                            }
                        }
                        Point t = pos;
                        path.Data = UpdateConnectorAdornerPathGeometry(t, null, e);
                        //this.ConnectorPathGeometry = UpdateConnectorAdornerPathGeometry(t, null, e);
                    }
                    else
                    {
                        //pos = new Point(Canvas.GetLeft(sender as Thumb) + e.HorizontalChange, Canvas.GetTop(sender as Thumb) + e.VerticalChange);                        
                        if (this.dc != null && this.dc.View != null)
                        {
                            if (dc.View.SnapToVerticalGrid)
                            {
                                pos.X = Node.Round(pos.X, dc.View.PxSnapOffsetX);
                            }
                            if (dc.View.SnapToHorizontalGrid)
                            {
                                pos.Y = Node.Round(pos.Y, dc.View.PxSnapOffsetY);
                            }
                        }
                        Point t = pos;
                        path.Data = UpdateConnectorAdornerPathGeometry(t, null, e);
                        //this.ConnectorPathGeometry = UpdateConnectorAdornerPathGeometry(t, null, e);
                        foreach (Node n in dc.Model.Nodes)
                        {
                            n.IsDragConnectionOver = false;
                        }
                    }

                    this.LineCanvas.Children.Add(path);

                    if (hitNodeConnector != null)
                    {
                        if (!centerhit)
                        {
                            foreach (ConnectionPort port in hitNodeConnector.Ports)
                            {
                                if (port != hitobject)
                                {
                                    port.IsDragOverPort = false;
                                }
                            }

                            if (centerport != null)
                            {
                                centerport.IsDragOverPort = false;
                            }
                        }
                        else
                        {
                            if (centerhit)
                            {
                                foreach (ConnectionPort port in hitNodeConnector.Ports)
                                {
                                    if (port.Name != null)
                                    {
                                        if (!port.Name.Equals("PART_Sync_CenterPort"))
                                        {
                                            port.IsDragOverPort = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    double sX = lineconnector.PxStartPointPosition.X;
                    double sY = lineconnector.PxStartPointPosition.Y;
                    double eX = lineconnector.PxEndPointPosition.X;
                    double eY = lineconnector.PxEndPointPosition.Y;

                    if (double.IsNaN(sX))
                    {
                        sX = 0;
                    }

                    if (double.IsNaN(sY))
                    {
                        sY = 0;
                    }

                    if (double.IsNaN(eX))
                    {
                        eX = 0;
                    }

                    if (double.IsNaN(eY))
                    {
                        eY = 0;
                    }

                    Point stp = new Point(lineconnector.PxStartPointPosition.X + sX, lineconnector.PxStartPointPosition.Y + sY);
                    Point enp = new Point(lineconnector.PxEndPointPosition.X + eX, lineconnector.PxEndPointPosition.Y + eY);
                }
            }
        }

        private double scrolloffsetx = 0;
        private double scrolloffsety = 0;

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            LineConnector lineconnector = this as LineConnector;
            lineconnector.positionchange = true;
            lineconnector.isClicked = true;
            m_StrokeDashArray = lineconnector.LineStyle.StrokeDashArray;
            this.isMouseup = false;
            if (checkDecoratorMovaable(sender))
            {
                return;
            }

            if (sender != null && sender is Thumb && (sender as Thumb).Name.Contains("Vertex") && lineconnector != null)
            {
                if (!lineconnector.IsVertexMovable)
                {
                    return;
                }
            }
            dragged = false;
            dc.View.IsDragged = true;
            if (dc.View.IsPageEditable)
            {
                if (!dc.View.IsPanEnabled)
                {
                    scrolloffsetx = dc.View.Scrollviewer.HorizontalOffset;
                    scrolloffsety = dc.View.Scrollviewer.VerticalOffset;
                    this.hitobject = null;
                    this.hitNodeConnector = null;
                    if (lineconnector.Cursor != null)
                    {
                        this.Cursor = lineconnector.Cursor;
                    }
                    else
                    {
                        this.Cursor = Cursors.Hand;
                    }

                    DoubleCollection dashArray = new DoubleCollection();
                    dashArray.Add(3);
                    dashArray.Add(3);
                    lineconnector.LineStyle.StrokeDashArray = dashArray;

                    if (sender == headThumb)
                    {
                        headthumb = true;
                        tailthumb = false;
                        if (lineconnector.TailNode != null)
                        {
                            fixedNodeConnection = lineconnector.TailNode as Node;
                        }

                        if (lineconnector.HeadNode != null)
                        {
                            movableNodeConnection = lineconnector.HeadNode as Node;
                        }

                        lineconnector.sp = lineconnector.PxStartPointPosition;
                        lineconnector.ep = lineconnector.PxEndPointPosition;
                    }

                    if (sender == tailThumb)
                    {
                        headthumb = false;
                        tailthumb = true;
                        if (lineconnector.TailNode != null)
                        {
                            movableNodeConnection = lineconnector.TailNode as Node;
                        }
                        else
                        {
                            movableNodeConnection = null;
                        }

                        if (lineconnector.HeadNode != null)
                        {
                            fixedNodeConnection = lineconnector.HeadNode as Node;
                        }
                        else
                        {
                            fixedNodeConnection = null;
                        }
                        lineconnector.sp = lineconnector.PxStartPointPosition;
                        lineconnector.ep = lineconnector.PxEndPointPosition;
                    }

                    if ((sender as Thumb).Name.Contains("Vertex"))
                    {
                        Vertex = sender as Thumb;
                        for (int i = 0; i < InterPts.Count; i++)
                        {
                            double Left = InterPts[i].X;
                            double Top = InterPts[i].Y;
                            if (Left == Canvas.GetLeft(Vertex) && Top == Canvas.GetTop(Vertex))
                            {
                                try
                                {
                                    if (i + 1 < InterPts.Count)
                                    {
                                        if (lineconnector.ConnectorType == ConnectorType.Orthogonal && (InterPts[i + 1].X == Left && InterPts[i + 1].Y == Top))
                                        {
                                            Vertex = Vertexs[i];
                                        }
                                    }
                                }
                                catch { }
                                vertexIndex = i;
                                headthumb = false;
                                tailthumb = false;
                                movableNodeConnection = null;
                                fixedNodeConnection = null;
                                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == (ModifierKeys.Shift | ModifierKeys.Control))
                                {
                                    if (lineconnector.ConnectorType == ConnectorType.Straight)
                                    {
                                        lineconnector.IntermediatePoints.Remove(InterPts[vertexIndex]);
                                    }
                                    else if (InterPts.Count >= 4)
                                    {
                                        if (vertexIndex == 0)
                                        {
                                            vertexIndex++;
                                            if (InterPts[vertexIndex].X == InterPts[vertexIndex + 1].X)
                                            {
                                                InterPts[vertexIndex - 1] = new Point(InterPts[vertexIndex - 1].X, InterPts[vertexIndex + 1].Y);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                            }
                                            else if (InterPts[vertexIndex].Y == InterPts[vertexIndex + 1].Y)
                                            {
                                                InterPts[vertexIndex + 2] = new Point(InterPts[vertexIndex].X, InterPts[vertexIndex + 2].Y);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                            }
                                        }
                                        else if (vertexIndex == InterPts.Count - 1 || vertexIndex == InterPts.Count - 2)
                                        {
                                            if (vertexIndex == InterPts.Count - 1)
                                            {
                                                vertexIndex--;
                                            }
                                            if (InterPts[vertexIndex].X == InterPts[vertexIndex + 1].X)
                                            {
                                                InterPts[vertexIndex - 1] = new Point(InterPts[vertexIndex - 1].X, InterPts[vertexIndex + 1].Y);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                            }
                                            else if (InterPts[vertexIndex].Y == InterPts[vertexIndex + 1].Y)
                                            {
                                                if (InterPts.Count != vertexIndex + 2)
                                                    InterPts[vertexIndex + 2] = new Point(InterPts[vertexIndex].X, InterPts[vertexIndex + 2].Y);
                                                else
                                                    InterPts[vertexIndex - 1] = new Point(InterPts[vertexIndex + 1].X, InterPts[vertexIndex - 1].Y);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                            }
                                        }
                                        else
                                        {
                                            if (InterPts[vertexIndex].X == InterPts[vertexIndex + 1].X)
                                            {
                                                InterPts[vertexIndex - 1] = new Point(InterPts[vertexIndex - 1].X, InterPts[vertexIndex + 1].Y);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                            }
                                            else if (InterPts[vertexIndex].Y == InterPts[vertexIndex + 1].Y)
                                            {
                                                InterPts[vertexIndex + 2] = new Point(InterPts[vertexIndex].X, InterPts[vertexIndex + 2].Y);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                                (lineconnector as LineConnector).IntermediatePoints.Remove(InterPts[vertexIndex]);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        lineconnector.IntermediatePoints.Clear();
                                        lineconnector.IntermediatePoints.Add(new Point(0, 0));
                                    }
                                    deletingMode = true;
                                    lineconnector.UpdateConnectorPathGeometry();
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            LineConnector lineconnector = this as LineConnector;
            if (checkDecoratorMovaable(sender))
            {
                return;
            }
            if (dc.View.IsPageEditable && lineconnector.isClicked)
            {
                ConnectorRoutedEventArgs NewEventArgs = new ConnectorRoutedEventArgs(lineconnector);
                dview.OnConnectorClick(lineconnector, NewEventArgs);
            }

            if (this.LineCanvas.Children.ElementAt(this.LineCanvas.Children.Count() - 1) is Path)
            {
                this.LineCanvas.Children.Remove(this.LineCanvas.Children.ElementAt(this.LineCanvas.Children.Count() - 1));
            }

            Point pos = pos1;

            lineconnector.isClicked = false;
            if (this.dc != null && this.dc.View != null)
            {
                if (dc.View.SnapToVerticalGrid)
                {
                    pos.X = Node.Round(pos.X, dc.View.PxSnapOffsetX);
                }
                if (dc.View.SnapToHorizontalGrid)
                {
                    pos.Y = Node.Round(pos.Y, dc.View.PxSnapOffsetY);
                }
            }
            if (!dragged)
            {
                lineconnector.LineStyle.StrokeDashArray = m_StrokeDashArray;
                if (deletingMode)
                {
                    InvalidateVertexs(lineconnector);
                }
                deletingMode = false;
                return;
            }

            if (deletingMode)
            {
                lineconnector.UpdateConnectorPathGeometry();
                if (lineconnector.ConnectorType == ConnectorType.Orthogonal)
                {
                    if (vertexIndex == 0)
                    {
                    }
                    else if (vertexIndex == Vertexs.Count - 1)
                    {
                    }
                    else
                    {
                    }
                }
                this.vertexItems.Items.Remove(Vertex);
                deletingMode = false;
            }
            else if ((lineconnector as LineConnector).DragCancel == false)
            {
                if (hitNodeConnector != null)
                {
                    if (lineconnector != null)
                    {
                        hitNodeConnector.IsDragConnectionOver = false;

                        if (centerport != null)
                        {
                            centerport.IsDragOverPort = false;
                        }
                        
                        if (sender == tailThumb)
                        {
                            tailthumb = true;
                            lineconnector.TailNode = this.hitNodeConnector;
                            (lineconnector as LineConnector).TailNodeReferenceNo = (lineconnector.TailNode as Node).ReferenceNo;
                            if (!centerhit)
                            {
                                foreach (ConnectionPort port in (lineconnector.TailNode as Node).Ports)
                                {
                                    port.IsDragOverPort = false;
                                    if (port == hitobject)
                                    {
                                        if (port.CenterPortReferenceNo != 0)
                                        {
                                            lineconnector.ConnectionTailPort = port;
                                            (lineconnector as LineConnector).TailPortReferenceNo = port.PortReferenceNo;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                lineconnector.ConnectionTailPort = null;
                            }
                        }
                        else if (sender == headThumb)
                        {
                            headthumb = true;
                            lineconnector.HeadNode = this.hitNodeConnector;
                            (lineconnector as LineConnector).HeadNodeReferenceNo = (lineconnector.HeadNode as Node).ReferenceNo;
                            if (!centerhit)
                            {
                                foreach (ConnectionPort port in (lineconnector.HeadNode as Node).Ports)
                                {
                                    port.IsDragOverPort = false;
                                    if (port == hitobject)
                                    {
                                        if (port.CenterPortReferenceNo != 0)
                                        {
                                            lineconnector.ConnectionHeadPort = port;
                                            (lineconnector as LineConnector).HeadPortReferenceNo = port.PortReferenceNo;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                lineconnector.ConnectionHeadPort = null;
                            }
                        }
                    }

                    hitNodeConnector.IsDragConnectionOver = false;
                }

                if (hitNodeConnector == null)
                {
                    if (sender == tailThumb)
                    {
                        if (lineconnector.ConnectorType == ConnectorType.Orthogonal && lineconnector.IntermediatePoints.Count >= 2)
                        {
                            tailthumb = true;
                            updateOrthogonalThumbs();
                        }

                        //if (previousHitNode != null)
                        //{
                        //    previousHitNode.InEdges.Remove(lineconnector);
                        //    previousHitNode.Edges.Remove(lineconnector);
                        //}
                        lineconnector.PxEndPointPosition = pos;
                    }
                    else if (sender == headThumb)
                    {
                        if (lineconnector.ConnectorType == ConnectorType.Orthogonal && lineconnector.IntermediatePoints.Count >= 2)
                        {
                            headthumb = true;
                            updateOrthogonalThumbs();
                        }

                        //if (previousHitNode != null)
                        //{
                        //    previousHitNode.OutEdges.Remove(lineconnector);
                        //    previousHitNode.Edges.Remove(lineconnector);
                        //}
                        lineconnector.PxStartPointPosition = pos;
                    }
                    else
                    {
                        if (lineconnector.ConnectorType == ConnectorType.Orthogonal)
                        {
                            updateOrthogonalThumbs();
                        }

                        InterPts[vertexIndex] = pos;
                        Canvas.SetLeft(Vertex, InterPts[vertexIndex].X);
                        Canvas.SetTop(Vertex, InterPts[vertexIndex].Y);
                    }
                }
                lineconnector.UpdateConnectorPathGeometry();
               
                dview.OnConnectorDragEnd(this, newEventArgs);
            }

            if (lineconnector.ConnectorType != ConnectorType.Bezier && lineconnector.ConnectorType != ConnectorType.Arc)
            {
                this.UpdateVertexsPosition(lineconnector);
            }

            this.hitNodeConnector = null;
            this.hitobject = null;
            lineconnector.DragCancel = false;
            dc.View.IsDragged = false;
            lineconnector.LineStyle.StrokeDashArray = m_StrokeDashArray;
            (dc.View.Page as DiagramPage).InvalidateMeasure();
            double scrollendoffsetx = dc.View.Scrollviewer.HorizontalOffset;
            double scrollendoffsety = dc.View.Scrollviewer.VerticalOffset;
            double diffx = scrollendoffsetx - scrolloffsetx;
            double diffy = scrollendoffsety - scrolloffsety;
        }

        private void updateOrthogonalThumbs()
        {
            Point pos = pos1;
            if (this.dc != null && this.dc.View != null)
            {
                if (dc.View.SnapToVerticalGrid)
                {
                    pos.X = Node.Round(pos.X, dc.View.PxSnapOffsetX);
                }
                if (dc.View.SnapToHorizontalGrid)
                {
                    pos.Y = Node.Round(pos.Y, dc.View.PxSnapOffsetY);
                }
            }
            Point mousePt = pos;

            settempstPoints();
            settempendPoints();
            Point tempEnd = this.PxEndPointPosition;
            if (tempEnd.Equals(new Point(0, 0)))
                tempEnd = tempend;
            Point tempStart = this.PxStartPointPosition;
            if (tempStart.Equals(new Point(0, 0)))
                tempStart = tempst;
            if (headthumb)
            {
                if (Math.Abs(tempStart.X - InterPts[0].X)
                > Math.Abs(tempStart.Y - InterPts[0].Y))
                {
                    InterPts[0] = new Point(InterPts[0].X, mousePt.Y);
                }
                else
                {
                    InterPts[0] = new Point(mousePt.X, InterPts[0].Y);
                }
                Point t = InterPts[0];
                Canvas.SetLeft(Vertexs[0], t.X);
                Canvas.SetTop(Vertexs[0], t.Y);
            }
            else if (tailthumb)
            {
                if (Math.Abs(tempEnd.X - InterPts[InterPts.Count - 1].X)
                > Math.Abs(tempEnd.Y - InterPts[InterPts.Count - 1].Y))
                {
                    InterPts[InterPts.Count - 1] = new Point(InterPts[InterPts.Count - 1].X, mousePt.Y);
                }
                else
                {
                    InterPts[InterPts.Count - 1] = new Point(mousePt.X, InterPts[InterPts.Count - 1].Y);
                }
                Point t = InterPts[InterPts.Count - 1];
                Canvas.SetLeft(Vertexs[InterPts.Count - 1], t.X);
                Canvas.SetTop(Vertexs[InterPts.Count - 1], t.Y);
            }
            else if (vertexIndex == 0)
            {
            }
            else if (vertexIndex == InterPts.Count - 1)
            {
            }
            else if (InterPts.Count >= 3)
            {
                if (InterPts[vertexIndex - 1].X == InterPts[vertexIndex].X)
                {
                    InterPts[vertexIndex - 1] = new Point(mousePt.X, InterPts[vertexIndex - 1].Y);
                    InterPts[vertexIndex + 1] = new Point(InterPts[vertexIndex + 1].X, mousePt.Y);

                    Point t = InterPts[vertexIndex - 1];
                    Canvas.SetLeft(Vertexs[vertexIndex - 1], t.X);
                    Canvas.SetTop(Vertexs[vertexIndex - 1], t.Y);

                    t = InterPts[vertexIndex + 1];
                    Canvas.SetLeft(Vertexs[vertexIndex + 1], t.X);
                    Canvas.SetTop(Vertexs[vertexIndex + 1], t.Y);
                }
                else
                {
                    InterPts[vertexIndex - 1] = new Point(InterPts[vertexIndex - 1].X, mousePt.Y);
                    InterPts[vertexIndex + 1] = new Point(mousePt.X, InterPts[vertexIndex + 1].Y);

                    Point t = InterPts[vertexIndex - 1];
                    Canvas.SetLeft(Vertexs[vertexIndex - 1], t.X);
                    Canvas.SetTop(Vertexs[vertexIndex - 1], t.Y);

                    t = InterPts[vertexIndex + 1];
                    Canvas.SetLeft(Vertexs[vertexIndex + 1], t.X);
                    Canvas.SetTop(Vertexs[vertexIndex + 1], t.Y);
                }
                if (vertexIndex == 1)
                {
                }
                else
                {
                }
                if (vertexIndex == InterPts.Count - 2)
                {
                }
                else
                {
                }
            }
        }

        internal void InsertIntermediatePoint(Point point)
        {
            double three = 3.0;
            if (IntermediatePoints == null)
            {
                IntermediatePoints = new List<Point>();
            }
            List<Point> points = new List<Point>();
            if (HeadNode != null)
            {
                NodeInfo source = new NodeInfo();
                source = (HeadNode as Node).GetInfo();

                if (dc != null && dc.View != null && dc.View.Page != null)
                {
                    if (this.ConnectionHeadPort != null)
                    {
                        double width = double.IsNaN(ConnectionHeadPort.Width) ? 2.5 : ConnectionHeadPort.Width / 2;
                        double height = double.IsNaN(ConnectionHeadPort.Height) ? 2.5 : ConnectionHeadPort.Height / 2;

                        source.Position = new Point(ConnectionHeadPort.PxCenterPosition.X + source.Left + width, height + ConnectionHeadPort.PxCenterPosition.Y + source.Top);
                    }
                }

                points.Add(source.Position);
            }
            else
            {
                points.Add(this.PxStartPointPosition);
            }
            points.AddRange(IntermediatePoints);

            if (TailNode != null)
            {
                NodeInfo target = new NodeInfo();
                target = (TailNode as Node).GetInfo();

                if (dc != null && dc.View != null && dc.View.Page != null)
                {
                    if (this.ConnectionTailPort != null)
                    {
                        double width = double.IsNaN(ConnectionTailPort.Width) ? 2.5 : ConnectionTailPort.Width / 2;
                        double height = double.IsNaN(ConnectionTailPort.Height) ? 2.5 : ConnectionTailPort.Height / 2;

                        target.Position = new Point(ConnectionTailPort.PxCenterPosition.X + target.Left + width, height + ConnectionTailPort.PxCenterPosition.Y + target.Top);
                    }
                    points.Add(target.Position);
                }
            }
            else
            {
                points.Add(this.PxEndPointPosition);
            }

            List<double> Diff = new List<double>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point st = points[i];
                Point en = points[i + 1];
                double delx = Math.Abs(st.X - en.X);
                double dely = Math.Abs(st.Y - en.Y);
                double lhs = ((point.Y - st.Y) / (en.Y - st.Y));
                double rhs = ((point.X - st.X) / (en.X - st.X));
                if (double.IsInfinity(lhs) || double.IsInfinity(rhs) || double.IsNaN(lhs) || double.IsNaN(rhs))
                {
                    if (st.X == en.X)
                    {
                        if (st.Y == en.Y)
                        {
                            Diff.Add(10000d);
                        }
                        else if (((st.Y > point.Y) && (point.Y > en.Y)) || ((st.Y < point.Y) && (point.Y < en.Y)))
                        {
                            Diff.Add(Math.Abs(st.X - point.X));
                        }
                        else
                        {
                            Diff.Add(10000d);
                        }
                    }
                    else if (st.Y == en.Y)
                    {
                        if (((st.X > point.X) && (point.X > en.X)) || ((st.X < point.X) && (point.X < en.X)))
                        {
                            Diff.Add(Math.Abs(st.Y - point.Y));
                        }
                        else
                        {
                            Diff.Add(10000d);
                        }
                    }
                    else
                    {
                        Diff.Add(10000d);
                    }
                }
                else if (ConnectorType != ConnectorType.Orthogonal)
                {
                    if ((st.X >= point.X && point.X >= en.X) || (st.X <= point.X && point.X <= en.X) || delx < three)
                    {
                        if ((st.Y >= point.Y && point.Y >= en.Y) || (st.Y <= point.Y && point.Y <= en.Y) || dely < three)
                        {
                            Diff.Add(Math.Abs(lhs - rhs));
                        }
                        else
                        {
                            Diff.Add(10000d);
                        }
                    }
                    else
                    {
                        Diff.Add(10000d);
                    }
                }
            }
            double selected = 10000;
            int selectedIndex = 0;
            for (int i = 0; i < Diff.Count - 1; i++)
            {
                double temp = Math.Min(Diff[i], Diff[i + 1]);
                if (temp < selected)
                {
                    selected = temp;
                    if (selected == Diff[i])
                        selectedIndex = i;
                    else
                        selectedIndex = i + 1;
                }
            }

            if (IntermediatePoints.Count < selectedIndex)
            {
                selectedIndex = IntermediatePoints.Count;
            }
            if (this.ConnectorType == ConnectorType.Orthogonal)
            {
                IntermediatePoints.Insert(selectedIndex, point);
            }
            IntermediatePoints.Insert(selectedIndex, point);
            if (ConnectorType == ConnectorType.Orthogonal)
            {
                if (IntermediatePoints.Count > 2)
                {
                    if (selectedIndex != 0)
                    {
                        if (Math.Abs(IntermediatePoints[selectedIndex].X - IntermediatePoints[selectedIndex - 1].X)
                            > Math.Abs(IntermediatePoints[selectedIndex].Y - IntermediatePoints[selectedIndex - 1].Y))
                        {
                            IntermediatePoints[selectedIndex] = new Point(IntermediatePoints[selectedIndex].X, IntermediatePoints[selectedIndex - 1].Y);
                            IntermediatePoints[selectedIndex + 1] = new Point(IntermediatePoints[selectedIndex + 1].X, IntermediatePoints[selectedIndex - 1].Y);
                        }
                        else
                        {
                            IntermediatePoints[selectedIndex] = new Point(IntermediatePoints[selectedIndex - 1].X, IntermediatePoints[selectedIndex].Y);
                            IntermediatePoints[selectedIndex + 1] = new Point(IntermediatePoints[selectedIndex - 1].X, IntermediatePoints[selectedIndex + 1].Y);
                        }
                    }
                    else
                    {
                        if (Math.Abs(IntermediatePoints[selectedIndex].X - IntermediatePoints[selectedIndex + 2].X)
                            > Math.Abs(IntermediatePoints[selectedIndex].Y - IntermediatePoints[selectedIndex + 2].Y))
                        {
                            IntermediatePoints[selectedIndex] = new Point(IntermediatePoints[selectedIndex].X, IntermediatePoints[selectedIndex + 2].Y);
                            IntermediatePoints[selectedIndex + 1] = new Point(IntermediatePoints[selectedIndex + 1].X, IntermediatePoints[selectedIndex + 2].Y);
                        }
                        else
                        {
                            IntermediatePoints[selectedIndex] = new Point(IntermediatePoints[selectedIndex + 2].X, IntermediatePoints[selectedIndex].Y);
                            IntermediatePoints[selectedIndex + 1] = new Point(IntermediatePoints[selectedIndex + 2].X, IntermediatePoints[selectedIndex + 1].Y);
                        }
                    }
                }
            }
        }

        void Node_LostFocus(object sender, RoutedEventArgs e)
        {
            this.linetext.Focus();
            this.linetext.SelectAll();
            this.linetext.LostFocus += new RoutedEventHandler(linetext_LostFocus);
        }


        void linetext_LostFocus(object sender, RoutedEventArgs e)
        {
            this.linetext.LostFocus -= new RoutedEventHandler(linetext_LostFocus);
            this.Label = this.linetext.Text;
            this.linelabel.Text = this.linetext.Text;
            this.linelabel.Visibility = Visibility.Visible;
            this.linetext.Visibility = Visibility.Collapsed;
            this.LostFocus -= new RoutedEventHandler(Node_LostFocus);
        }
        ConnDragEndRoutedEventArgs newEventArgs;
        /// <summary>
        /// Provides class handling for the MouseLeftButtonUp routed event that occurs when
        /// the mouse left button is released over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
        private void Lineconnector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsDecoratorMovable)
            {
                return;
            }
            if (this.linedragging)
            {
                this.dview.tUndoStack.Push(new LineOperation(LineOperations.Dragged, this));
                if (IsLinedrag)
                {

                    (dc.View.Page as DiagramPage).InvalidateMeasure();
                    IsLinedrag = false;
                }

                if (this.LineCanvas.Children.ElementAt(this.LineCanvas.Children.Count() - 1) is Path)
                {
                    this.LineCanvas.Children.Remove(this.LineCanvas.Children.ElementAt(this.LineCanvas.Children.Count() - 1));
                }

                if (this.headthumb)
                {
                    if (this.hitNodeConnector == null)
                    {
                        this.PxStartPointPosition = e.GetPosition(this);
                        this.ConnectionHeadPort = null;
                        this.HeadNode = null;
                    }
                    else
                    {
                        this.hitNodeConnector.IsDragConnectionOver = false;
                        this.HeadNode = this.hitNodeConnector;
                        this.ConnectionHeadPort = hitport;
                        this.HeadNodeReferenceNo = this.HeadNode.ReferenceNo;
                        if (this.ConnectionHeadPort != null)
                            this.HeadPortReferenceNo = this.ConnectionHeadPort.PortReferenceNo;
                    }
                }
                else if (this.tailthumb)
                {
                    if (this.hitNodeConnector == null)
                    {
                        this.PxEndPointPosition = e.GetPosition(this);
                        this.ConnectionTailPort = null;
                        this.TailNode = null;
                    }
                    else
                    {
                        this.hitNodeConnector.IsDragConnectionOver = false;
                        this.TailNode = this.hitNodeConnector;
                        this.ConnectionTailPort = hitport;
                        this.TailNodeReferenceNo = this.TailNode.ReferenceNo;
                        if (this.ConnectionTailPort != null)
                            this.TailPortReferenceNo = this.ConnectionTailPort.PortReferenceNo;
                    }
                }
               
                if (this.hitNodeConnector == null && this.fixedNodeConnection == null)
                {
                    newEventArgs = new ConnDragEndRoutedEventArgs(this);
                }

                else if (this.hitNodeConnector != null && this.fixedNodeConnection == null)
                {
                    newEventArgs = new ConnDragEndRoutedEventArgs(this.hitNodeConnector as Node, this as LineConnector);
                }

                else if (this.hitNodeConnector == null && this.fixedNodeConnection != null)
                {
                    newEventArgs = new ConnDragEndRoutedEventArgs(this as LineConnector, this.fixedNodeConnection as Node);
                }

                else
                {
                    newEventArgs = new ConnDragEndRoutedEventArgs(this.fixedNodeConnection as Node, this.hitNodeConnector as Node, this as LineConnector);
                }

                if (this.hitNodeConnector != null)
                {
                    foreach (ConnectionPort port in this.hitNodeConnector.Ports)
                    {
                        port.IsDragOverPort = false;
                    }
                }

                this.hitNodeConnector = null;
                this.ConnectorPathGeometry = null;
                this.LineStyle.StrokeDashArray = m_StrokeDashArray;
                this.linedragging = false;
                this.ReleaseMouseCapture();
                this.Update();
            }
            else
            {
                if (dc.View.IsPageEditable)
                {
                    ConnectorRoutedEventArgs NewEvtArgs = new ConnectorRoutedEventArgs(this);
                    dview.OnConnectorClick(this, NewEvtArgs);
                }
            }

            this.UpdateConnectorPathGeometry();
            this.headthumb = false;
            this.tailthumb = false;
            bool notselected = true;
            if (this.dc.View.IsPageEditable && this.IsGrouped)
            {
                if (!this.dc.View.IsPanEnabled && !this.isdoubleclicked)
                {
                    IDiagramPage mdiagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;
                    //// update selection
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

                                    if (notselected)
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
                                    else if (this.IsGrouped)
                                    {
                                        CollectionExt groupednodes = new CollectionExt();
                                        foreach (Group g in this.Groups)
                                        {
                                            groupednodes.Add(g);
                                        }

                                        groupednodes.Insert(0, this);
                                        foreach (ICommon gnode in groupednodes)
                                        {
                                            if (gnode.IsSelected)
                                            {
                                                if (groupednodes.Count > 1)
                                                {
                                                    int index = groupednodes.IndexOf(gnode);
                                                    if (index == 0)
                                                    {
                                                        mdiagramPage.SelectionList.Add(groupednodes[groupednodes.Count - 1]);
                                                    }
                                                    else
                                                    {
                                                        mdiagramPage.SelectionList.Add(groupednodes[index - 1]);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (this.IsGrouped)
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

                                if (notselected)
                                {
                                    if (!this.IsGrouped || this.Groups.Count == 0)
                                    {
                                        mdiagramPage.SelectionList.Select(this);
                                    }
                                    else
                                    {
                                        mdiagramPage.SelectionList.Select(this.Groups[this.Groups.Count - 1]);
                                    }
                                }
                                else if (this.IsGrouped)
                                {
                                    CollectionExt groupednodes = new CollectionExt();
                                    foreach (Group g in this.Groups)
                                    {
                                        groupednodes.Add(g);
                                    }

                                    groupednodes.Insert(0, this);

                                    foreach (INodeGroup gnode in groupednodes)
                                    {
                                        if ((gnode as ICommon).IsSelected)
                                        {
                                            if (groupednodes.Count > 1)
                                            {
                                                int index = groupednodes.IndexOf(gnode);
                                                if (index == 0)
                                                {
                                                    mdiagramPage.SelectionList.Select(groupednodes[groupednodes.Count - 1]);
                                                }
                                                else
                                                {
                                                    mdiagramPage.SelectionList.Select(groupednodes[index - 1]);
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (this.IsGrouped && this.Groups.Count > 1)
                            {
                                mdiagramPage.SelectionList.Select(this.Groups[this.Groups.Count - 1]);
                            }
                        }
                    }
                }
            }
            this.dview.Page.InvalidateMeasure();
            this.dview.ScrollGrid.ScrollOwner.InvalidateScrollInfo();
            mouseup = true;
            this.isdoubleclicked = false;
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

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (dview != null)
            {
                if (dview.IsPageEditable)
                {
                    if (dview.LineConnectorContextMenu == null && this.ContextMenu == null)
                    {
                        if (linecontextmenu != null)
                        {
                            ContextMenuControlService.SetContextMenuControl(this, linecontextmenu);
                            linecontextmenu.OpenPopup(e.GetPosition(null));
                            setDisableOrEnable(Front, Forward, Backward, Back, del, group);
                        }

                    }
                    else if (dview.LineConnectorContextMenu != null)
                    {
                        ContextMenuControlService.SetContextMenuControl(this, dview.LineConnectorContextMenu);
                        dview.LineConnectorContextMenu.OpenPopup(e.GetPosition(null));
                    }


                    base.OnMouseRightButtonUp(e);
                }

            }
        }

        private void setDisableOrEnable(ContextMenuControlItem Front, ContextMenuControlItem Forward, ContextMenuControlItem Backward, ContextMenuControlItem Back, ContextMenuControlItem del, ContextMenuControlItem group)
        {
            if (this.IsSelected)
            {
                Front.IsEnabled = true;
                Forward.IsEnabled = true;
                Backward.IsEnabled = true;
                Back.IsEnabled = true;
                del.IsEnabled = true;
            }
            else
            {
                Front.IsEnabled = false;
                Forward.IsEnabled = false;
                Backward.IsEnabled = false;
                Back.IsEnabled = false;
                del.IsEnabled = false;
            }

            if (this.IsGrouped)
            {
                foreach (Group g in this.Groups)
                {
                    if (g.IsSelected)
                    {
                        del.IsEnabled = true;
                    }
                }
            }

            if (dview != null && dview.SelectionList.Count > 1 && !this.IsGrouped)
            {
                group.IsEnabled = true;
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
                                    group.IsEnabled = true;
                                    break;
                                }
                                else
                                {
                                    group.IsEnabled = false;
                                }
                            }
                        }
                    }
                }
            }

            if (dview != null && dview.SelectionList.Count <= 1)
            {
                group.IsEnabled = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.MouseRightButtonDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (dc.View.IsPageEditable && !dview.IsPanEnabled)
            {
                DiagramPage diagramPanel = this.dc.View.Page as DiagramPage;
                if (diagramPanel != null)
                {
                    if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    {
                        if (this.IsSelected)
                        {
                            diagramPanel.SelectionList.Remove(this);
                        }
                        else
                        {
                            diagramPanel.SelectionList.Add(this);
                        }
                    }
                    else if (!this.IsSelected)
                    {
                        diagramPanel.SelectionList.Clear();
                        diagramPanel.SelectionList.Add(this);
                    }
                }
            }
            base.OnMouseRightButtonUp(e);
        }



        /// <summary>
        /// Handles the Click event of the delete menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Del_Click(object sender, RoutedEventArgs e)
        {
            dc.Delete.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the bring to front menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M1_Click(object sender, RoutedEventArgs e)
        {
            dc.BringToFront.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the bring forward menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M2_Click(object sender, RoutedEventArgs e)
        {
            dc.BringForward.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the send backward menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M3_Click(object sender, RoutedEventArgs e)
        {
            dc.SendBackward.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the send to back menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M4_Click(object sender, RoutedEventArgs e)
        {
            dc.SendToBack.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the group menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void G1_Click(object sender, RoutedEventArgs e)
        {
            dc.Group.Execute(dc.View);
        }

        /// <summary>
        /// Handles the Click event of the ungroup menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void G2_Click(object sender, RoutedEventArgs e)
        {
            dc.UnGroup.Execute(dc.View);
        }

        internal bool positionchange = false;
        internal bool oldone = true;

        internal Point pos1 = new Point();
        /// <summary>
        /// Provides class handling for the MouseMove routed event that occurs when
        /// the mouse left button is released over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
        void LineConnector_MouseMove(object sender, MouseEventArgs e)
        {
            pos1 = e.GetPosition(this);
            if (!this.IsDecoratorMovable)
            {
                return;
            }
            if (this.linedragging)
            {
                if (this.LineCanvas.Children.ElementAt(this.LineCanvas.Children.Count() - 1) is Path)
                {
                    this.LineCanvas.Children.Remove(this.LineCanvas.Children.ElementAt(this.LineCanvas.Children.Count() - 1));
                }

                this.Cursor = Cursors.Hand;

                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == (ModifierKeys.Shift | ModifierKeys.Control)
                        && ((this.ConnectorType == ConnectorType.Orthogonal) || (this.ConnectorType == ConnectorType.Straight)))
                {
                    this.Cursor = Cursors.Hand;
                }

                DoubleCollection dcoll = new DoubleCollection();
                dcoll.Add(3);
                dcoll.Add(3);
                this.LineStyle.StrokeDashArray = dcoll;

                if (this.headthumb)
                {
                    if (this.TailNode != null)
                    {
                        this.fixedNodeConnection = this.TailNode as Node;
                    }
                    else
                    {
                        this.fixedNodeConnection = null;
                    }
                    if (this.HeadNode != null)
                    {
                        movableNodeConnection = this.HeadNode as Node;
                    }
                    else
                    {
                        movableNodeConnection = null;
                    }
                }
                else if (this.tailthumb)
                {
                    if (this.HeadNode != null)
                    {
                        this.fixedNodeConnection = this.HeadNode as Node;
                    }
                    else
                    {
                        this.fixedNodeConnection = null;
                    }
                    if (this.TailNode != null)
                    {
                        movableNodeConnection = this.TailNode as Node;
                    }
                    else
                    {
                        movableNodeConnection = null;
                    }
                }
                Point hp = e.GetPosition(Application.Current.RootVisual);
                bool foundNode = this.HitTesting(hp);
                Path path = new Path();
                path.Stroke = this.LineStyle.Stroke;
                path.StrokeThickness = this.LineStyle.StrokeThickness;
                //m_StrokeDashArray = this.LineStyle.StrokeDashArray;              

                if (foundNode && this.hitNodeConnector != null)
                {
                    path.Data = this.UpdateConnectorAdornerPathGeometry(e.GetPosition(Application.Current.RootVisual), e, null);
                    bool temp = this.HitTesting(e.GetPosition(Application.Current.RootVisual));
                    if (!foundport)
                    {
                        foreach (ConnectionPort p in this.hitNodeConnector.Ports)
                        {
                            p.IsDragOverPort = false;
                        }
                    }

                }
                else
                {
                    //if (this.previousHitNode != null)
                    //{
                    //    foreach (ConnectionPort port in this.previousHitNode.Ports)
                    //    {
                    //        port.IsDragOverPort = false;
                    //    }
                    //}
                    path.Data = this.UpdateConnectorAdornerPathGeometry(e.GetPosition(Application.Current.RootVisual), e, null);
                    foreach (Node n in this.dc.Model.Nodes)
                    {
                        n.IsDragConnectionOver = false;
                    }
                }

                this.LineCanvas.Children.Add(path);
                if (mouseup)
                {
                    ConnDragRoutedEventArgs newEventArgs;
                    if (fixedNodeConnection != null && movableNodeConnection != null)
                    {
                        newEventArgs = new ConnDragRoutedEventArgs(this.fixedNodeConnection as Node, this.movableNodeConnection as Node, this);
                    }
                    else
                        if (fixedNodeConnection == null && movableNodeConnection != null)
                        {
                            newEventArgs = new ConnDragRoutedEventArgs(this.movableNodeConnection as Node, this);
                        }
                        else if (fixedNodeConnection != null && movableNodeConnection == null)
                        {
                            newEventArgs = new ConnDragRoutedEventArgs(this, fixedNodeConnection as Node);
                        }
                        else
                        {
                            newEventArgs = new ConnDragRoutedEventArgs(this);
                        }
                    dview.OnConnectorDragStart(this, newEventArgs);
                    this.IsLinedrag = true;
                    mouseup = false;
                }
            }

            else
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == (ModifierKeys.Shift | ModifierKeys.Control)
                        && ((this.ConnectorType == ConnectorType.Orthogonal) || (this.ConnectorType == ConnectorType.Straight)))
                    this.Cursor = Cursors.Stylus;
                else
                    this.Cursor = Cursors.Arrow;
            }
        }

        //private double lineangle = 0;
        /// <summary>
        /// Invoked whenever application code or internal processes call this OnApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.dc = DiagramPage.GetDiagramControl((FrameworkElement)this);
            if (dc != null)
            {
                if (dc.View != null && dc.View.Page != null)
                {

                }
            }

            this.HeadDecoratorGrid = GetTemplateChild("PART_HeadDecoratorGrid") as Grid;
            this.TailDecoratorGrid = GetTemplateChild("PART_TailDecoratorGrid") as Grid;
            vertexItems = GetTemplateChild("PART_VertexItems") as ItemsControl;
            this.LineSelection(this);
            this.LineCanvas = GetTemplateChild("PART_LineCanvas") as Canvas;
            this.HeadShape = GetTemplateChild("PART_HeadDecoratorAnchorPath") as Path;
            this.TailShape = GetTemplateChild("PART_SinkAnchorPath") as Path;
            this.linetext = GetTemplateChild("PART_TextBox1") as TextBox;
            this.linelabel = GetTemplateChild("PART_TextBlock1") as TextBlock;
            SetShape("Head");
            SetShape("Tail");
            this.labelGrid = GetTemplateChild("PART_LabelGrid") as Grid;
            RotateTransform transform = new RotateTransform();
            transform.Angle = this.LineAngle + this.LabelAngle;
            (this.linetext.Parent as Grid).RenderTransform = transform;
            this.labelGrid.RenderTransform = transform;

            RotateTransform rottransform = new RotateTransform();
            rottransform.Angle = this.HeadDecoratorAngle;
            rottransform.CenterX = 5;
            rottransform.CenterY = 5;
            TranslateTransform transtransform = new TranslateTransform();
            transtransform.X = -5;
            transtransform.Y = -5;
            if (this.HeadDecoratorGrid != null)
            {
                TransformGroup group = new TransformGroup();
                group.Children.Add(rottransform);
                group.Children.Add(transtransform);
                this.HeadDecoratorGrid.RenderTransform = group;
            }

            rottransform = new RotateTransform();
            rottransform.Angle = this.TailDecoratorAngle;
            rottransform.CenterX = 5;
            rottransform.CenterY = 5;
            transtransform = new TranslateTransform();
            transtransform.X = -5;
            transtransform.Y = -5;
            if (this.TailDecoratorGrid != null)
            {
                TransformGroup group = new TransformGroup();
                group.Children.Add(rottransform);
                group.Children.Add(transtransform);
                this.TailDecoratorGrid.RenderTransform = group;
            }
            if (this.dc != null)
            {
                if (DiagramControl.IsPageLoaded)
                {
                    (this.dc.View.Page as DiagramPage).IsDiagrampageLoaded = true;
                }
                else
                {
                    (this.dc.View.Page as DiagramPage).IsDiagrampageLoaded = false;
                }
                this.UpdateConnectorPathGeometry();
            }
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
            if (this.ContextMenu == null)
            {
                CreateDefaultContextMenu();
            }
            this.linelabel.MouseMove += new MouseEventHandler(linelabel_MouseMove);
            this.linelabel.MouseLeave += new MouseEventHandler(linelabel_MouseLeave);
            this.linelabel.MouseEnter += new MouseEventHandler(linelabel_MouseEnter);
            this.linelabel.MouseLeftButtonDown += new MouseButtonEventHandler(linelabel_MouseLeftButtonDown);
            this.linelabel.MouseLeftButtonUp += new MouseButtonEventHandler(linelabel_MouseLeftButtonUp);
        }

        ContextMenuControlItem Order;
        ContextMenuControlItem Front;
        ContextMenuControlItem Forward;
        ContextMenuControlItem Backward;
        ContextMenuControlItem Back;
        ContextMenuControlItem grouping;
        ContextMenuControlItem group;
        ContextMenuControlItem ungroup;
        ContextMenuControlItem del;


        private void CreateDefaultContextMenu()
        {
            linecontextmenu = new ContextMenuControl();

            Order = new ContextMenuControlItem();
            Order.Header = ContextMenu_Order;
            Front = new ContextMenuControlItem();
            Front.Header = ContextMenu_Order_BringToFront;
            Front.Click += new RoutedEventHandler(M1_Click);
            Order.Items.Add(Front);
            Forward = new ContextMenuControlItem();
            Forward.Header = ContextMenu_Order_BringForward;
            Forward.Click += new RoutedEventHandler(M2_Click);
            Order.Items.Add(Forward);
            Backward = new ContextMenuControlItem();
            Backward.Header = ContextMenu_Order_SendBackward;
            Backward.Click += new RoutedEventHandler(M3_Click);
            Order.Items.Add(Backward);
            Back = new ContextMenuControlItem();
            Back.Header = ContextMenu_Order_SendToBack;
            Back.Click += new RoutedEventHandler(M4_Click);
            Order.Items.Add(Back);

            grouping = new ContextMenuControlItem();
            grouping.Header = ContextMenu_Grouping;
            group = new ContextMenuControlItem();
            group.Header = ContextMenu_Grouping_Group;
            group.Click += new RoutedEventHandler(G1_Click);
            grouping.Items.Add(group);
            ungroup = new ContextMenuControlItem();
            ungroup.Header = ContextMenu_Grouping_Ungroup;
            ungroup.Click += new RoutedEventHandler(G2_Click);
            grouping.Items.Add(ungroup);

            del = new ContextMenuControlItem();
            del.Header = ContextMenu_Delete;
            del.Click += new RoutedEventHandler(Del_Click);

            linecontextmenu.Items.Add(Order);
            linecontextmenu.Items.Add(grouping);
            linecontextmenu.Items.Add(del);
        }

        private Point startPosition;
        private bool labelPressed = false;
        private Point labelcurrentPosition;
        private Point previousPosition;

        void linelabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.linelabel.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
            this.labelPressed = false;
        }

        void linelabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.labelPressed = true;
            this.linelabel.CaptureMouse();
            if (this.IsLabelDragable && startPosition == new Point(0, 0))
            {
                this.startPosition = e.GetPosition(this.dview.Page as DiagramPage);
            }
            base.OnMouseLeftButtonDown(e);
        }

        void linelabel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (labelPressed)
            {
                this.linelabel.CaptureMouse();
            }
            base.OnMouseEnter(e);
        }

        void linelabel_MouseLeave(object sender, MouseEventArgs e)
        {
            this.linelabel.ReleaseMouseCapture();
            base.OnMouseLeave(e);
        }

        void linelabel_MouseMove(object sender, MouseEventArgs e)
        {
            this.linelabel.CaptureMouse();
            if (this.IsLabelDragable)
            {
                labelcurrentPosition = e.GetPosition(this.dview.Page as DiagramPage);
                if (this.labelPressed)
                {
                    var move = new TranslateTransform();
                    this.LabelPosition = new Point(this.LabelPosition.X + (labelcurrentPosition.X - previousPosition.X), this.LabelPosition.Y + (labelcurrentPosition.Y - previousPosition.Y));
                }
                else
                {
                    this.linelabel.Cursor = Cursors.Hand;
                }
                previousPosition = labelcurrentPosition;
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Event raised when the connector path geometry is changed.
        /// </summary>
        /// <param name="d">object whose path geometry is changed</param>
        /// <param name="e">DependencyPropertyChanged event</param>
        private static void OnConnectorPathGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            line.UpdateDecoratorPosition(line);
        }

        /// <summary>
        /// Event raised when the head decorator angel is changed.
        /// </summary>
        /// <param name="d">object whose head decorator angel is changed</param>
        /// <param name="e">DependencyPropertyChanged event</param>
        private static void OnHeadDecoratorAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            RotateTransform rottransform = new RotateTransform();
            rottransform.Angle = line.HeadDecoratorAngle;
            rottransform.CenterX = 7;
            rottransform.CenterY = 7;
            TranslateTransform transtransform = new TranslateTransform();
            transtransform.X = -7;
            transtransform.Y = -7;
            if (line.HeadDecoratorGrid != null)
            {
                TransformGroup group = new TransformGroup();
                group.Children.Add(rottransform);
                group.Children.Add(transtransform);
                line.HeadDecoratorGrid.RenderTransform = group;
            }
        }

        /// <summary>
        /// Event raised when the IsSelected is changed.
        /// </summary>
        /// <param name="d">object whose IsSelected is changed</param>
        /// <param name="e">DependencyPropertyChanged event</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            line.LineSelection(line);
        }

        private static void OnLineAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            RotateTransform rottransform = new RotateTransform();
            rottransform.Angle = line.LineAngle;
            if (line.linetext != null)
                (line.linetext.Parent as Grid).RenderTransform = rottransform;
            if (line.labelGrid != null)
                line.labelGrid.RenderTransform = rottransform;
        }


        private static void OnContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            ContextMenuControlService.SetContextMenuControl(line, (ContextMenuControl)e.NewValue);

        }
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string oldvalue = (string)e.OldValue;
            string newvalue = (string)e.NewValue;

        }

        /// <summary>
        /// Event raised when the tail decorator angel is changed.
        /// </summary>
        /// <param name="d">object whose tail decorator angel is changed</param>
        /// <param name="e">DependencyPropertyChanged event</param>
        private static void OnTailDecoratorAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineConnector line = d as LineConnector;
            RotateTransform rottransform = new RotateTransform();
            rottransform.Angle = line.TailDecoratorAngle;
            rottransform.CenterX = 7;
            rottransform.CenterY = 7;
            TranslateTransform transtransform = new TranslateTransform();
            transtransform.X = -7;
            transtransform.Y = -7;
            if (line.TailDecoratorGrid != null)
            {
                TransformGroup group = new TransformGroup();
                group.Children.Add(rottransform);
                group.Children.Add(transtransform);
                line.TailDecoratorGrid.RenderTransform = group;
            }
        }

        /// <summary>
        /// Returns the segment.
        /// </summary>
        /// <param name="x1">The x coordinate of the starting point(first control point) of the curve.</param>
        /// <param name="y1">The y coordinate of the starting point(first control point) of the curve.</param>
        /// <param name="x2">The x coordinate of the end point of the curve.</param>
        /// <param name="y2">The y coordinate of the end point of the curve.</param>
        /// <param name="temp1">The x coordinate of the second control point of the curve.</param>
        /// <param name="temp2">The y coordinate of the second control point of the curve.</param>
        /// <returns>The segment.</returns>
        internal BezierSegment Segment(double x1, double y1, double x2, double y2, double temp1, double temp2)
        {
            BezierSegment segment = new BezierSegment
            {
                Point1 = new Point(x1, y1),
                Point2 = new Point(temp1, temp2),
                Point3 = new Point(x2, y2)
            };

            return segment;
        }

        /// <summary>
        /// Shows the adorner
        /// </summary>
        protected override void ShowAdorner()
        {
        }

        /// <summary>
        /// updates connector path geometry
        /// </summary>
        /// <param name="position">line connector </param>
        /// <param name="e">MouseEventArgs</param>
        /// <returns>PathGeometry</returns>
        internal PathGeometry UpdateConnectorAdornerPathGeometry(Point position, MouseEventArgs e, DragDeltaEventArgs ed)
        {
            Point pos = pos1;
            LineConnector lineconnector = this;

            if (lineconnector.ConnectorType == ConnectorType.Bezier || lineconnector.ConnectorType == ConnectorType.Arc)
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
                Point p = new Point(0, 0);
                PathGeometry pathgeometry = new PathGeometry();
                if (this.fixedNodeConnection != null)
                {
                    source = this.fixedNodeConnection.GetInfo();
                    sourceRect = new Rect(
                                          source.Left,
                                          source.Top,
                                          source.Size.Width,
                                          source.Size.Height);
                    p = position;// MeasureUnitsConverter.FromPixels(position, source.MeasurementUnit);
                    if (this.HitTesting(position))
                    {
                        if (this.hitNodeConnector != null)
                        {
                            NodeInfo target = this.hitNodeConnector.GetInfo();
                            double hitwidth = this.hitNodeConnector.Width;// MeasureUnitsConverter.FromPixels(this.hitNodeConnector.Width, target.MeasurementUnit);
                            double hitheight = this.hitNodeConnector.Height;// MeasureUnitsConverter.FromPixels(this.hitNodeConnector.Height, target.MeasurementUnit);
                            rectTarget = new Rect(this.hitNodeConnector.PxOffsetX, this.hitNodeConnector.PxOffsetY, hitwidth, hitheight);
                            if (this.ConnectorType == ConnectorType.Bezier)
                            {
                                if (this.headthumb)
                                {
                                    ConnectorBase.GetOrthogonalLineIntersect(target, source, rectTarget, sourceRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                                }
                                else
                                {
                                    ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, rectTarget, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                                }
                            }
                            else
                            {
                                if (this.headthumb)
                                {
                                    ConnectorBase.GetArcLineIntersect(target, source, rectTarget, sourceRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, this.ArcDirection);
                                }
                                else
                                {
                                    ConnectorBase.GetArcLineIntersect(source, target, sourceRect, rectTarget, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, this.ArcDirection);
                                }
                            }

                            x1 = startPoint.X;
                            y1 = startPoint.Y;
                            x2 = endPoint.X;
                            y2 = endPoint.Y;
                        }
                        else
                        {
                            if (!this.headthumb)
                            {
                                x1 = this.fixedNodeConnection.PxPosition.X;
                                y1 = this.fixedNodeConnection.PxPosition.Y;
                                x2 = pos1.X;// e.GetPosition(this).X;
                                y2 = pos1.Y;// e.GetPosition(this).Y;
                            }
                            else
                            {
                                x2 = this.fixedNodeConnection.PxPosition.X;
                                y2 = this.fixedNodeConnection.PxPosition.Y;
                                x1 = pos1.X;// e.GetPosition(this).X;
                                y1 = pos1.Y;// e.GetPosition(this).Y;
                            }
                        }
                    }
                    else
                    {
                        double twozero = 20;// MeasureUnitsConverter.FromPixels(20, DiagramPage.Munits);
                        double targetleft = pos1.X;// e.GetPosition(this).X;// MeasureUnitsConverter.FromPixels(e.GetPosition(this).X, DiagramPage.Munits);
                        double targettop = pos1.Y; //e.GetPosition(this).Y;// MeasureUnitsConverter.FromPixels(e.GetPosition(this).Y, DiagramPage.Munits);
                        NodeInfo pointend = new NodeInfo();
                        Rect pointendRect = new Rect();
                        if (!this.headthumb)
                        {
                            lineconnector.ConnectionTailPort = null;
                            pointendRect = new Rect(
                                                  targetleft,
                                                  targettop,
                                                  twozero,
                                                  twozero);
                            pointend.Position = new Point(pos1.X + 25, pos1.Y + 25);

                            if ((lineconnector as LineConnector).HeadNode != null)
                            {
                                NodeInfo src = ((lineconnector as LineConnector).HeadNode as Node).GetInfo();
                                Rect rectsrc = new Rect(
                                    src.Left,
                                    src.Top,
                                    src.Size.Width,
                                    src.Size.Height);
                                Point sp1 = new Point(src.Position.X, src.Position.Y);

                                if (this.ConnectorType == ConnectorType.Bezier)
                                {
                                    if (!this.headthumb)
                                    {
                                        ConnectorBase.GetOrthogonalLineIntersect(src, pointend, rectsrc, pointendRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                                    }
                                    else
                                    {
                                        ConnectorBase.GetOrthogonalLineIntersect(pointend, src, pointendRect, rectsrc, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                                    }
                                }
                                else
                                {
                                    if (!this.headthumb)
                                    {
                                        ConnectorBase.GetArcLineIntersect(src, pointend, rectsrc, pointendRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, this.ArcDirection);
                                    }
                                    else
                                    {
                                        ConnectorBase.GetArcLineIntersect(pointend, src, pointendRect, rectsrc, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, this.ArcDirection);
                                    }
                                }
                            }
                            else
                            {
                                startPoint = (lineconnector as LineConnector).PxStartPointPosition;
                            }

                            endPoint = pos1;// e.GetPosition(this);
                        }
                        else
                        {
                            lineconnector.ConnectionHeadPort = null;
                            pointendRect = new Rect(
                                                    targetleft,
                                                     targettop,
                                                     twozero,
                                                     twozero);
                            pointend.Position = new Point(pos1.X, pos1.Y);
                            if ((lineconnector as LineConnector).TailNode != null)
                            {
                                NodeInfo target = ((lineconnector as LineConnector).TailNode as Node).GetInfo();
                                Rect recttarget = new Rect(
                                                            target.Left,
                                                            target.Top,
                                                            target.Size.Width,
                                                            target.Size.Height);
                                Point ep1 = new Point(target.Position.X, target.Position.Y);
                                if (this.ConnectorType == ConnectorType.Bezier)
                                {
                                    if (!this.headthumb)
                                    {
                                        ConnectorBase.GetOrthogonalLineIntersect(target, pointend, recttarget, pointendRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                                    }
                                    else
                                    {
                                        ConnectorBase.GetOrthogonalLineIntersect(pointend, target, pointendRect, recttarget, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                                    }
                                }
                                else
                                {
                                    if (!this.headthumb)
                                    {
                                        ConnectorBase.GetArcLineIntersect(target, pointend, recttarget, pointendRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, this.ArcDirection);
                                    }
                                    else
                                    {
                                        ConnectorBase.GetArcLineIntersect(pointend, target, pointendRect, recttarget, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, this.ArcDirection);
                                    }
                                }
                            }
                            else
                            {
                                endPoint = (lineconnector as LineConnector).PxEndPointPosition;
                            }

                            startPoint = pos1; // e.GetPosition(this);
                        }
                    }

                    x1 = startPoint.X;
                    y1 = startPoint.Y;
                    x2 = endPoint.X;
                    y2 = endPoint.Y;
                }
                else
                {
                    if (!this.headthumb)
                    {
                        x1 = (lineconnector as LineConnector).PxStartPointPosition.X;
                        y1 = (lineconnector as LineConnector).PxStartPointPosition.Y;
                        x2 = pos1.X; //e.GetPosition(this).X;
                        y2 = pos1.Y; //e.GetPosition(this).Y;
                    }
                    else
                    {
                        x2 = (lineconnector as LineConnector).PxEndPointPosition.X;
                        y2 = (lineconnector as LineConnector).PxEndPointPosition.Y;
                        x1 = pos1.X; // e.GetPosition(this).X;
                        y1 = pos1.Y; // e.GetPosition(this).Y;
                    }
                }

                PathFigure pathfigure = new PathFigure();
                double num = Math.Max((double)(Math.Abs((double)(x2 - x1)) / 2.0), (double)20.0);
                pathfigure.StartPoint = new Point(x1, y1);

                if (this.ConnectorType == ConnectorType.Bezier)
                {
                    BezierSegment segment = new BezierSegment();
                    if (isBottom)
                    {
                        segment = this.GetSegment(x1, y1 + num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                    }
                    else if (isTop)
                    {
                        segment = this.GetSegment(x1, y1 - num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                    }
                    else if (isRight)
                    {
                        segment = this.GetSegment(x1 + num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                    }
                    else
                    {
                        segment = this.GetSegment(x1 - num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
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
                    segment.Size = new Size(d / 2, this.ArcHeight);
                    segment.RotationAngle = angle;
                    segment.IsLargeArc = false;
                    segment.SweepDirection = this.ArcDirection;

                    pathfigure.Segments.Add(segment);
                }
                pathgeometry.Figures.Add(pathfigure);
                return pathgeometry;
            }
            else
            {
                if (this.fixedNodeConnection != null && lineconnector.ConnectorType != ConnectorType.Orthogonal)
                {
                    PathGeometry pathgeometry = new PathGeometry();
                    List<Point> connectionPoints = this.GetAdornerLinePoints(this.fixedNodeConnection.GetInfo(), position, e);
                    if (connectionPoints.Count > 0)
                    {
                        PathFigure pathfigure = new PathFigure();
                        pathfigure.StartPoint = connectionPoints[0];
                        foreach (Point p in connectionPoints)
                        {
                            LineSegment polyline = new LineSegment();
                            polyline.Point = p;
                            pathfigure.Segments.Add(polyline);
                        }
                        pathgeometry.Figures.Add(pathfigure);
                    }

                    return pathgeometry;
                }
                else
                {
                    PathGeometry pathgeometry = new PathGeometry();
                    List<Point> connectionPoints = new List<Point>();
                    if (headthumb || tailthumb)
                    {
                        try
                        {
                            if (lineconnector.HeadNode != null)
                            {
                                bool t = tailthumb;
                                tailthumb = true;
                                connectionPoints = GetAdornerLinePoints((this.HeadNode as Node).GetInfo(), position, e);
                                tailthumb = t;
                            }
                            else if (lineconnector.TailNode != null)
                            {
                                bool t = headthumb;
                                headthumb = true;
                                connectionPoints = GetAdornerLinePoints((this.TailNode as Node).GetInfo(), position, e);
                                headthumb = t;
                            }
                            if (lineconnector.HeadNode != null || lineconnector.TailNode != null)
                            {
                                tempst = connectionPoints[0];
                                tempend = connectionPoints[1];
                            }
                        }
                        catch { }
                    }
                    if ((lineconnector as LineConnector).ConnectorType == ConnectorType.Orthogonal && (lineconnector as LineConnector).IntermediatePoints.Count < 2)
                    {
                        connectionPoints.Clear();
                        if (headthumb)
                        {
                            if (this.TailNode == null)
                            {
                                connectionPoints.Add(new Point((lineconnector as LineConnector).PxEndPointPosition.X, (lineconnector as LineConnector).PxEndPointPosition.Y));
                            }
                            else
                            {
                                connectionPoints.Add(tempend);
                            }
                            connectionPoints.Add(pos);//MeasureUnitsConverter.FromPixels(pos, (this.lineconnector as LineConnector).MeasurementUnit));
                            connectionPoints.Insert(1, (new Point(connectionPoints[1].X, connectionPoints[0].Y)));
                        }
                        else if (tailthumb)
                        {
                            if (this.HeadNode == null)
                            {
                                connectionPoints.Add(new Point((lineconnector as LineConnector).PxStartPointPosition.X, (lineconnector as LineConnector).PxStartPointPosition.Y));
                            }
                            else
                            {
                                connectionPoints.Add(tempst);
                            }
                            connectionPoints.Add(pos);//MeasureUnitsConverter.FromPixels(pos, (this.lineconnector as LineConnector).MeasurementUnit));
                            connectionPoints.Insert(1, (new Point(connectionPoints[0].X, connectionPoints[1].Y)));
                        }
                    }
                    else
                    {
                        connectionPoints = this.GetAdornerLinePoints(e, ed);
                    }
                    settempstPoints();
                    settempendPoints();
                    for (int i = 0; i < connectionPoints.Count; i++)
                    {
                        //connectionPoints[i] = MeasureUnitsConverter.ToPixels(connectionPoints[i], (lineconnector as LineConnector).MeasurementUnit);
                    }
                    if (connectionPoints.Count > 0)
                    {
                        PathFigure pathfigure = new PathFigure();
                        pathfigure.StartPoint = connectionPoints[0];
                        foreach (Point p in connectionPoints)
                        {
                            LineSegment polyline = new LineSegment();
                            polyline.Point = p;
                            pathfigure.Segments.Add(polyline);
                        }
                        pathgeometry.Figures.Add(pathfigure);
                    }

                    return pathgeometry;
                }
            }
        }

        internal bool isRouting = false;
        private bool invalid = true;

        /// <summary>
        /// Called whenever the head node, tail node or position of the node is changed. 
        /// </summary>
        public override void UpdateConnectorPathGeometry()
        {
            if (invalid)
            {
                invalid = false;
                this.Dispatcher.BeginInvoke(
                    () =>
                    {
                        try
                        {
                            Update();
                        }
                        finally
                        {
                            invalid = true;
                        }
                    }
                    );
            }
        }

        private void Update()
        {
            if (this.isdefaulted)
            {
                //this.LabelWidth = Distance;
            }

            if (this.dview == null)
            {
                this.dview = Node.GetDiagramView(this);
            }

            Point startpos = this.PxStartPointPosition;
            Point endpos = this.PxEndPointPosition;
            bool isLeft = false;
            bool isRight = false;
            bool isTop = false;
            bool isBottom = false;
            bool tisLeft = false;
            bool tisRight = false;
            bool tisTop = false;
            bool tisBottom = false;
            Point s = new Point(0, 0);
            Point e = new Point(0, 0);
            Point sp = new Point(0, 0);
            Point ep = new Point(0, 0);
            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(0, 0);
            Rect sourceRect = new Rect();
            Rect targetRect = new Rect();
            NodeInfo source = new NodeInfo();
            NodeInfo target = new NodeInfo();
            double cs = 0;
            double twozero = 20;// MeasureUnitsConverter.FromPixels(20, DiagramPage.Munits);
            double sourceleft = PxStartPointPosition.X;// MeasureUnitsConverter.FromPixels(StartPointPosition.X, DiagramPage.Munits);
            double sourcetop = PxStartPointPosition.Y;// MeasureUnitsConverter.FromPixels(StartPointPosition.Y, DiagramPage.Munits);
            double targetleft = PxEndPointPosition.X;// MeasureUnitsConverter.FromPixels(EndPointPosition.X, DiagramPage.Munits);
            double targettop = PxEndPointPosition.Y;// MeasureUnitsConverter.FromPixels(EndPointPosition.Y, DiagramPage.Munits);
            double dropcentreX = DropPoint.X;// MeasureUnitsConverter.FromPixels(DropPoint.X, DiagramPage.Munits);
            double dropcentreY = DropPoint.Y;// MeasureUnitsConverter.FromPixels(DropPoint.Y, DiagramPage.Munits);

            if (this.HeadNode != null)
            {
                source = (this.HeadNode as Node).GetInfo();

                cs = this.PxConnectionEndSpace;// MeasureUnitsConverter.FromPixels(this.ConnectionEndSpace, source.MeasurementUnit);

                sourceRect = new Rect(
                                   source.Left - 10,
                                   source.Top - 10,
                                   source.Size.Width + 10,
                                   source.Size.Height + 10);

                if (!(this.HeadNode as Node).IsInternallyLoaded)
                {
                    startPoint = new Point((this.HeadNode as Node).OffsetX + (this.HeadNode as Node)._Width / 2, (this.HeadNode as Node).OffsetY + (this.HeadNode as Node)._Height / 2);
                }
                else
                {
                    startPoint = new Point(source.Position.X, source.Position.Y);
                }
                this.m_TempStart = startPoint;
            }
            else
            {
                startPoint = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                s = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                sourceRect = new Rect(
                                    sourceleft - 10,
                                    sourcetop - 10,
                                    twozero + 10,
                                    twozero + 10);
                source.Position = new Point(PxStartPointPosition.X + 25, PxStartPointPosition.Y + 25);
            }

            if (this.TailNode != null)
            {
                target = (this.TailNode as Node).GetInfo();
                cs = this.PxConnectionEndSpace;// MeasureUnitsConverter.FromPixels(this.ConnectionEndSpace, target.MeasurementUnit);
                targetRect = new Rect(
                                      target.Left - 10,
                                      target.Top - 10,
                                      target.Size.Width + 10,
                                      target.Size.Height + 10);
                if (!(this.TailNode as Node).IsInternallyLoaded)
                {
                    endPoint = new Point((this.TailNode as Node).OffsetX + (this.TailNode as Node)._Width / 2, (this.TailNode as Node).OffsetY + (this.TailNode as Node)._Height / 2);
                }
                else
                {
                    endPoint = new Point(target.Position.X, target.Position.Y);
                }
                this.m_TempEnd = endPoint;
            }
            else
            {
                e = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                endPoint = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                targetRect = new Rect(
                                      targetleft - 10,
                                      targettop - 10,
                                      twozero + 10,
                                      twozero + 10);
                target.Position = new Point(PxEndPointPosition.X - 25, PxEndPointPosition.Y - 25);
            }
            if (this.linelabel == null) return;

            #region Bezier and Arc
            if (this.ConnectorType == ConnectorType.Bezier || this.ConnectorType == ConnectorType.Arc)
            {
                try
                {
                    if (this.HeadNode != null || this.TailNode != null)
                    {
                        sp = startPoint;// MeasureUnitsConverter.FromPixels(startPoint, source.MeasurementUnit);
                        ep = endPoint;// MeasureUnitsConverter.FromPixels(endPoint, source.MeasurementUnit);
                    }

                    if (this.ConnectorType == ConnectorType.Bezier)
                    {
                        if (!(Orientation == TreeOrientation.LeftRight || Orientation == TreeOrientation.RightLeft))
                        {
                            ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, targetRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out s, out e);
                        }
                        else
                        {
                            ConnectorBase.GetTreeOrthogonalLineIntersect(source, target, sourceRect, targetRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out s, out e);
                        }
                    }
                    else
                    {
                        ConnectorBase.GetArcLineIntersect(source, target, sourceRect, targetRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out s, out e, this.ArcDirection);
                    }

                    if (this.HeadNode != null)
                    {
                        if (this.ConnectionHeadPort == null)
                        {
                            sp = startPoint;// MeasureUnitsConverter.FromPixels(startPoint, source.MeasurementUnit);
                        }
                        else
                        {
                            s = new Point(source.Left + this.ConnectionHeadPort.PxLeft, source.Top + ConnectionHeadPort.PxTop);
                            if ((this.HeadNode as Node).RenderTransform != null)
                            {
                                if ((this.HeadNode as Node).RenderTransform is RotateTransform)
                                {
                                    s = this.GeneralPointRotation(source.Position, s, ((this.HeadNode as Node).RenderTransform as RotateTransform).Angle);
                                }

                            }

                        }
                    }

                    if (this.TailNode != null)
                    {
                        if (this.ConnectionTailPort == null)
                        {
                            ep = endPoint;// MeasureUnitsConverter.FromPixels(endPoint, source.MeasurementUnit);
                        }
                        else
                        {
                            e = new Point(target.Left + this.ConnectionTailPort.PxLeft , target.Top + ConnectionTailPort.PxTop );
                            if ((this.TailNode as Node).RenderTransform != null)
                            {
                                if ((this.TailNode as Node).RenderTransform is RotateTransform)
                                {
                                    e = this.GeneralPointRotation(target.Position, e, ((this.TailNode as Node).RenderTransform as RotateTransform).Angle);
                                }

                            }
                        }
                    }

                    if (this.HeadNode == null)
                    {
                        s = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                    }

                    if (this.TailNode == null)
                    {
                        e = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                    }

                    x1 = s.X;
                    y1 = s.Y;
                    x2 = e.X;
                    y2 = e.Y;
                    PathGeometry pathgeometry = new PathGeometry();
                    PathFigure pathfigure = new PathFigure();
                    double num = 150.0;
                    pathfigure.StartPoint = new Point(x1, y1);
                    if (this.ConnectorType == ConnectorType.Bezier)
                    {
                        BezierSegment segment = new BezierSegment();
                        if (this.HeadDecoratorGrid != null)
                        {
                            if (isBottom)
                            {
                                (this as LineConnector).HeadDecoratorAngle = -90;
                                segment = this.GetSegment(x1, y1 + num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                            }
                            else if (isTop)
                            {
                                (this as LineConnector).HeadDecoratorAngle = 90;
                                segment = this.GetSegment(x1, y1 - num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                            }
                            else if (isRight)
                            {
                                (this as LineConnector).HeadDecoratorAngle = -180;
                                segment = this.GetSegment(x1 + num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                            }
                            else
                            {
                                (this as LineConnector).HeadDecoratorAngle = 0;
                                segment = this.GetSegment(x1 - num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                            }
                        }

                        if (this.TailDecoratorGrid != null)
                        {
                            if (tisBottom)
                            {
                                (this as LineConnector).TailDecoratorAngle = -90;
                            }
                            else if (tisTop)
                            {
                                (this as LineConnector).TailDecoratorAngle = 90;
                            }
                            else if (tisRight)
                            {
                                (this as LineConnector).TailDecoratorAngle = 0;
                            }
                            else
                            {
                                (this as LineConnector).TailDecoratorAngle = 0;
                            }
                        }

                        double x = Math.Pow((x1 - x2), 2);
                        double y = Math.Pow((y1 - y2), 2);
                        Distance = Math.Sqrt(x + y);
                        pathfigure.Segments.Add(segment);
                    }
                    else
                    {
                        //double angle = ConnectorBase.FindAngle(s, e);

                        //double bottomtop = 0;
                        //double rightleft = 0;

                        //if (this.ArcDirection == SweepDirection.Clockwise)
                        //{
                        //    bottomtop = 90;
                        //    rightleft = -90;
                        //}
                        //else
                        //{
                        //    bottomtop = -90;
                        //    rightleft = 90;
                        //}
                        ////this.GetPointAtLength(
                        //double fullLength;
                        //int segmentIndex;
                        //if (this.ConnectorPathGeometry != null && this.ConnectorPathGeometry.Figures.Count > 0)
                        //{
                        //    this.GetLengthAtFractionPoint(this.ConnectorPathGeometry.Figures[0], new Point(0, 0), out fullLength, out segmentIndex);
                        //}
                        //if (this.HeadDecoratorGrid != null)
                        //{
                        //    if (isBottom)
                        //    {
                        //        (this as LineConnector).HeadDecoratorAngle = angle + bottomtop;
                        //    }
                        //    else if (isTop)
                        //    {
                        //        (this as LineConnector).HeadDecoratorAngle = angle + bottomtop;
                        //    }
                        //    else if (isRight)
                        //    {
                        //        (this as LineConnector).HeadDecoratorAngle = angle - 180 + rightleft;
                        //    }
                        //    else
                        //    {
                        //        (this as LineConnector).HeadDecoratorAngle = angle + 180 + rightleft;
                        //    }
                        //}

                        //if (this.TailDecoratorGrid != null)
                        //{
                        //    if (tisBottom)
                        //    {
                        //        (this as LineConnector).TailDecoratorAngle = angle + bottomtop;
                        //    }
                        //    else if (tisTop)
                        //    {
                        //        (this as LineConnector).TailDecoratorAngle = angle + bottomtop;
                        //    }
                        //    else if (tisRight)
                        //    {
                        //        (this as LineConnector).TailDecoratorAngle = angle - 180 + rightleft;
                        //    }
                        //    else
                        //    {
                        //        (this as LineConnector).TailDecoratorAngle = angle + 180 + rightleft;
                        //    }
                        //} 
                        Point mid = new Point(s.X + (e.X - s.X) / 2, s.Y + (e.Y - s.Y) / 2);
                        double ang = findAngle(s, mid);
                        if (ArcDirection == SweepDirection.Clockwise)
                        {
                            mid.Y -= (ArcHeight * 15) * Math.Cos(ang * Math.PI / 180);
                            mid.X += (ArcHeight * 15) * Math.Sin(ang * Math.PI / 180);
                        }
                        else
                        {
                            mid.X -= (ArcHeight * 15) * Math.Sin(ang * Math.PI / 180);
                            mid.Y += (ArcHeight * 15) * Math.Cos(ang * Math.PI / 180);
                        }
                        (this as LineConnector).HeadDecoratorAngle = findAngle(mid, s);
                        (this as LineConnector).TailDecoratorAngle = findAngle(mid, e);

                        

                        double x = Math.Pow((x1 - x2), 2);
                        double y = Math.Pow((y1 - y2), 2);
                        double d = Math.Sqrt(x + y);

                        double angle = ConnectorBase.FindAngle(s, e);

                        ArcSegment segment = new ArcSegment();
                        segment.Point = e;
                        segment.Size = new Size(d / 2, this.ArcHeight);
                        segment.RotationAngle = angle;
                        segment.IsLargeArc = false;
                        segment.SweepDirection = this.ArcDirection;

                        pathfigure.Segments.Add(segment);
                    }
                    pathgeometry.Figures.Add(pathfigure);

                    if (this.HeadNode != null && this.TailNode != null)
                    {
                        this.misoverlapped = false;
                        this.TailDecoratorShape = this.InternalTailShape;
                        this.HeadDecoratorShape = this.InternalHeadShape;
                    }

                    ConnectionPoints = new List<Point>();
                    ConnectionPoints.Add(s);
                    ConnectionPoints.Add(e);
                    this.PxStartPointPosition = s;
                    this.PxEndPointPosition = e;
                    if (!AllowCustomLabelPosition)
                    {
                        this.LabelPosition = new Point((s.X + e.X) / 2, (s.Y + e.Y) / 2);
                    }
                    this.LabelTemplatePosition = new Point((s.X + e.X) / 2, (s.Y + e.Y) / 2);
                    this.ConnectorPathGeometry = pathgeometry;
                    this.m_TempEnd = e;
                    this.m_TempStart = s;
                }
                catch
                {
                }

                if (this.LabelHorizontalAlignment == HorizontalAlignment.Left && !AllowCustomLabelPosition)
                {
                    this.LabelPosition = PxStartPointPosition;

                    if (PxStartPointPosition.X > PxEndPointPosition.X)
                    {
                        this.LabelPosition = new Point(PxStartPointPosition.X - this.linelabel.ActualWidth, this.LabelPosition.Y);
                    }
                    if (PxStartPointPosition.Y > PxEndPointPosition.Y)
                    {
                        this.LabelPosition = new Point(LabelPosition.X, this.LabelPosition.Y - this.linelabel.ActualHeight);
                    }
                }
                else if (this.LabelHorizontalAlignment == HorizontalAlignment.Right && !AllowCustomLabelPosition)
                {
                    this.LabelPosition = PxEndPointPosition;

                    if (PxStartPointPosition.X < PxEndPointPosition.X)
                    {
                        this.LabelPosition = new Point(PxEndPointPosition.X - this.linelabel.ActualWidth, this.LabelPosition.Y);
                    }
                    if (PxStartPointPosition.Y < PxEndPointPosition.Y)
                    {
                        this.LabelPosition = new Point(LabelPosition.X, this.LabelPosition.Y - this.linelabel.ActualHeight);
                    }
                }
                else if (!AllowCustomLabelPosition)
                {
                    if (this.ConnectorType == ConnectorType.Arc)
                    {
                        this.LabelAngle = 0;
                        Point cp = new Point((PxStartPointPosition.X + PxEndPointPosition.X) / 2, (PxStartPointPosition.Y + PxEndPointPosition.Y) / 2);
                        double arcangle = FindAngle(PxStartPointPosition, PxEndPointPosition);
                        double angle = (arcangle * Math.PI) / 180;
                        if (this.ArcDirection == SweepDirection.Clockwise)
                        {
                            this.LabelPosition = new Point(cp.X + (this.ArcHeight * Math.Sin(angle)), cp.Y - (this.ArcHeight * Math.Cos(angle)));
                        }
                        else
                        {
                            this.LabelPosition = new Point(cp.X - (this.ArcHeight * Math.Sin(angle)), cp.Y + (this.ArcHeight * Math.Cos(angle)));
                        }
                    }
                    else
                    {
                        this.LabelAngle = 0;
                        this.LabelPosition = new Point((PxStartPointPosition.X + PxEndPointPosition.X) / 2, (PxStartPointPosition.Y + PxEndPointPosition.Y) / 2);
                    }
                }
                if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Left)
                {
                    this.LabelTemplatePosition = PxStartPointPosition;

                    if (PxStartPointPosition.X > PxEndPointPosition.X)
                    {
                        this.LabelTemplatePosition = new Point(PxStartPointPosition.X - this.labelGrid.ActualWidth, this.LabelPosition.Y);
                    }
                    if (PxStartPointPosition.Y > PxEndPointPosition.Y)
                    {
                        this.LabelTemplatePosition = new Point(LabelTemplatePosition.X, this.LabelPosition.Y - this.labelGrid.ActualHeight);
                    }
                }
                else if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Right)
                {
                    this.LabelTemplatePosition = PxEndPointPosition;

                    if (PxStartPointPosition.X < PxEndPointPosition.X)
                    {
                        this.LabelTemplatePosition = new Point(PxEndPointPosition.X - this.labelGrid.ActualWidth, this.LabelTemplatePosition.Y);
                    }
                    if (PxStartPointPosition.Y < PxEndPointPosition.Y)
                    {
                        this.LabelTemplatePosition = new Point(LabelTemplatePosition.X, this.LabelTemplatePosition.Y - this.labelGrid.ActualHeight);
                    }
                }

            }
            #endregion

            else
            {
                try
                {
                    if (this.HeadNode != null || this.TailNode != null)
                    {
                        Point zero = new Point(0, 0);
                        ConnectionPoints = new List<Point>();
                        sp = startPoint;// MeasureUnitsConverter.FromPixels(startPoint, source.MeasurementUnit);
                        ep = endPoint;// MeasureUnitsConverter.FromPixels(endPoint, source.MeasurementUnit);

                        if (dc.View._LineRoute || !this.LineRoutingEnabled || (!dc.View.IsDragged && !dc.View._LineRoute))
                        {
                            if (this.HeadNode == null && this.TailNode != null)
                            {
                                ConnectionPoints = this.GetLinePoints((this.TailNode as Node).GetInfo(), true);
                            }

                            if (this.TailNode == null && this.HeadNode != null)
                            {
                                ConnectionPoints = this.GetLinePoints((this.HeadNode as Node).GetInfo());
                            }

                            if (this.HeadNode != null && this.TailNode != null)
                            {
                                ConnectionPoints = this.GetLinePoints((this.HeadNode as Node).GetInfo(), (this.TailNode as Node).GetInfo());
                            }

                            if (IntermediatePoints == null)
                            {
                                IntermediatePoints = new List<Point>();
                            }

                            if (ConnectionPoints.Count > 0)
                            {
                                this.misoverlapped = false;

                                if (this.ConnectorType == ConnectorType.Orthogonal)
                                {
                                    ConnectionPoints = meetOrhogonalConstraints(ConnectionPoints, cs);
                                }

                                if (IntermediatePoints.Count >= 1)
                                {
                                    for (int i = IntermediatePoints.Count - 1; i >= 0; i--)
                                    {
                                        ConnectionPoints.Insert(1, IntermediatePoints[i]);
                                    }
                                }

                                if (this.ConnectorType == ConnectorType.Orthogonal)
                                    adjustIntermediatePoints(ConnectionPoints);

                                UpdatePathGeometry(ConnectionPoints);
                            }
                            else
                            {
                                this.ConnectorPathGeometry = new PathGeometry();
                            }
                        }
                    }
                    else
                    {
                        PathGeometry pathgeometry = new PathGeometry();
                        ConnectionPoints = this.GetLinePoints();
                        if (IntermediatePoints == null)
                        {
                            IntermediatePoints = new List<Point>();
                        }

                        Point EndPoint = new Point(0, 0);
                        Point StartPoint = new Point(0, 0);
                        if (ConnectionPoints.Count > 0)
                        {
                            if (this.ConnectorType == ConnectorType.Orthogonal)
                            {
                                ConnectionPoints = meetOrhogonalConstraints(ConnectionPoints, cs);
                            }
                            PathFigure pathfigure = new PathFigure();
                            if (IntermediatePoints.Count >= 1)
                            {
                                for (int i = IntermediatePoints.Count - 1; i >= 0; i--)
                                {
                                    ConnectionPoints.Insert(1, IntermediatePoints[i]);
                                }
                            }

                            if (this.ConnectorType == ConnectorType.Orthogonal)
                                adjustIntermediatePoints(ConnectionPoints);

                            pathfigure.StartPoint = ConnectionPoints[0];
                            StartPoint = pathfigure.StartPoint;
                            foreach (Point p in ConnectionPoints)
                            {
                                LineSegment polyline = new LineSegment();
                                polyline.Point = p;
                                pathfigure.Segments.Add(polyline);
                                EndPoint = p;
                            }
                            pathgeometry.Figures.Add(pathfigure);
                            this.ConnectorPathGeometry = pathgeometry;
                        }

                        if (ConnectionPoints != null)
                        {
                            UpdateLabelPosition(ConnectionPoints, out StartPoint, out EndPoint);
                        }

                        if (this.LabelHorizontalAlignment == HorizontalAlignment.Left && !AllowCustomLabelPosition)
                        {
                            if (this.LabelOrientation == LabelOrientation.Horizontal && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelPosition = StartPoint;
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelPosition = new Point(StartPoint.X - this.linelabel.DesiredSize.Width, StartPoint.Y);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelPosition = new Point(StartPoint.X - this.linelabel.DesiredSize.Width, StartPoint.Y - this.linelabel.DesiredSize.Height);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelPosition = new Point(StartPoint.X, StartPoint.Y - this.linelabel.DesiredSize.Height);
                                }
                            }
                            else if (this.LabelOrientation == LabelOrientation.Vertical && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelPosition = new Point(StartPoint.X, StartPoint.Y + this.linelabel.DesiredSize.Width);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelPosition = new Point(StartPoint.X - this.linelabel.DesiredSize.Height, StartPoint.Y + this.linelabel.DesiredSize.Width);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelPosition = new Point(StartPoint.X - this.linelabel.DesiredSize.Height, StartPoint.Y);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelPosition = StartPoint;
                                }

                            }
                            else
                            {
                                this.LabelPosition = StartPoint;
                            }


                        }
                        else if (this.LabelHorizontalAlignment == HorizontalAlignment.Right && !AllowCustomLabelPosition)
                        {
                            if (this.LabelOrientation == LabelOrientation.Horizontal && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelPosition = new Point(EndPoint.X - this.linelabel.DesiredSize.Width - 10, EndPoint.Y - this.linelabel.DesiredSize.Height);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y - this.linelabel.DesiredSize.Height);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelPosition = new Point(EndPoint.X - this.linelabel.DesiredSize.Width, EndPoint.Y + 5);
                                }

                            }
                            else if (this.LabelOrientation == LabelOrientation.Vertical && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelPosition = new Point(EndPoint.X - this.linelabel.DesiredSize.Height, EndPoint.Y - 5);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelPosition = new Point(EndPoint.X, EndPoint.Y - 5);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y + this.linelabel.DesiredSize.Width);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelPosition = new Point(EndPoint.X - this.linelabel.DesiredSize.Height - 5, EndPoint.Y + this.linelabel.DesiredSize.Width);
                                }
                            }

                            else
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                this.LabelPosition = new Point(EndPoint.X - (Math.Cos(angle) * (this.linelabel.ActualWidth + 10)), EndPoint.Y - (Math.Sin(angle) * (this.linelabel.ActualWidth + 10)));
                            }
                        }
                        else if (!AllowCustomLabelPosition)
                        {
                            if (LineAngle > 90 && LineAngle < 270)
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                LabelAngle = 180;
                                this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 + (Math.Cos(angle) * (this.linelabel.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 + (Math.Sin(angle) * (this.linelabel.DesiredSize.Width / 2)));
                            }
                            else
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                LabelAngle = 0;
                                this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (this.linelabel.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (this.linelabel.DesiredSize.Width / 2)));
                            }

                        }

                        if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Left)
                        {
                            if (this.LabelTemplateOrientation == LabelOrientation.Horizontal && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelTemplatePosition = StartPoint;
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.ActualWidth, StartPoint.Y);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.ActualWidth, StartPoint.Y - this.labelGrid.ActualHeight);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X, StartPoint.Y - this.labelGrid.ActualHeight);
                                }
                            }
                            else if (this.LabelTemplateOrientation == LabelOrientation.Vertical && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X, StartPoint.Y + this.labelGrid.ActualWidth);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.ActualHeight, StartPoint.Y + this.labelGrid.ActualWidth);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.ActualHeight, StartPoint.Y);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelTemplatePosition = StartPoint;
                                }

                            }
                            else
                            {
                                this.LabelTemplatePosition = StartPoint;
                            }
                        }
                        else if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Right)
                        {
                            if (this.LabelTemplateOrientation == LabelOrientation.Horizontal && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.ActualWidth - 10, EndPoint.Y - this.labelGrid.ActualHeight);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y - this.labelGrid.ActualHeight);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.ActualWidth, EndPoint.Y + 5);
                                }

                            }
                            else if (this.LabelTemplateOrientation == LabelOrientation.Vertical && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.ActualHeight, EndPoint.Y - 5);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X, EndPoint.Y - 5);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y + this.labelGrid.ActualWidth);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.ActualHeight - 5, EndPoint.Y + this.labelGrid.ActualWidth);
                                }
                            }
                            else
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                this.LabelTemplatePosition = new Point(EndPoint.X - (Math.Cos(angle) * (this.labelGrid.ActualWidth + 10)), EndPoint.Y - (Math.Sin(angle) * (this.labelGrid.ActualWidth + 10)));
                            }
                        }
                        else
                        {
                            if (LineAngle > 90 && LineAngle < 270)
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                LabelTemplateAngle = 180;
                                this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2 + (Math.Cos(angle) * (this.labelGrid.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 + (Math.Sin(angle) * (this.labelGrid.DesiredSize.Width / 2)));
                            }
                            else
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                LabelTemplateAngle = 0;
                                this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (this.labelGrid.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (this.labelGrid.DesiredSize.Width / 2)));

                            }
                        }
                    }

                    RotateTransform transform = new RotateTransform();
                    RotateTransform ttransform = new RotateTransform();
                    if (this.LineAngle < 270 && this.LineAngle > 90)
                    {

                        if ((this.LabelOrientation == LabelOrientation.Vertical || this.LabelTemplateOrientation == LabelOrientation.Vertical) && this.ConnectorType == ConnectorType.Straight)
                        {
                            if (this.LabelOrientation == LabelOrientation.Vertical)
                            {
                                transform.Angle = 270;
                            }

                            if (this.LabelHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                if (!AllowCustomLabelPosition)
                                    this.LabelPosition = new Point((startPoint.X + endPoint.X) / 2 - this.linelabel.DesiredSize.Height / 2, (startPoint.Y + endPoint.Y) / 2 + this.linelabel.DesiredSize.Width / 2);
                            }
                            if (this.LabelTemplateOrientation == LabelOrientation.Vertical)
                            {
                                ttransform.Angle = 270;
                            }

                            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                this.LabelTemplatePosition = new Point((startPoint.X + endPoint.X) / 2 - this.labelGrid.ActualHeight / 2, (startPoint.Y + endPoint.Y) / 2 + this.labelGrid.ActualWidth / 2);

                            }

                        }
                        else if ((this.LabelOrientation == LabelOrientation.Horizontal || this.LabelTemplateOrientation == LabelOrientation.Horizontal) && this.ConnectorType == ConnectorType.Straight)
                        {
                            if (this.LabelOrientation == LabelOrientation.Horizontal)
                            {
                                transform.Angle = 0;
                            }
                            if (this.LabelHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                if (!AllowCustomLabelPosition)
                                    this.LabelPosition = new Point((startPoint.X + endPoint.X) / 2 - this.linelabel.DesiredSize.Width / 2, (startPoint.Y + endPoint.Y) / 2 - this.linelabel.DesiredSize.Height / 2);
                            }
                            if (this.LabelTemplateOrientation == LabelOrientation.Horizontal)
                            {
                                ttransform.Angle = 0;
                            }
                            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                this.LabelTemplatePosition = new Point((startPoint.X + endPoint.X) / 2 - this.labelGrid.ActualWidth / 2, (startPoint.Y + endPoint.Y) / 2 - this.labelGrid.ActualHeight / 2);
                            }

                        }
                        else
                        {
                            // transform.Angle = 180 + this.LineAngle + this.LabelAngle;
                            double angle = (this.LineAngle * Math.PI) / 180;
                            //this.LabelPosition = new Point(this.LabelPosition.X + (Math.Cos(angle) * this.linelabel.ActualWidth), this.LabelPosition.Y + (Math.Sin(angle) * this.linelabel.ActualWidth));
                            //  this.LabelTemplatePosition = new Point(this.LabelTemplatePosition.X + (Math.Cos(angle) * this.labelGrid.ActualWidth), this.LabelTemplatePosition.Y + (Math.Sin(angle) * this.labelGrid.ActualWidth));

                        }

                    }
                    else
                    {
                        if ((this.LabelOrientation == LabelOrientation.Vertical || this.LabelTemplateOrientation == LabelOrientation.Vertical) && this.ConnectorType == ConnectorType.Straight)
                        {

                            //  this.LabelAngle = 270;
                            if (this.LabelOrientation == LabelOrientation.Vertical)
                            {
                                transform.Angle = 270;
                            }

                            if (this.LabelHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                if (!AllowCustomLabelPosition)
                                    this.LabelPosition = new Point((startPoint.X + endPoint.X) / 2 - this.linelabel.DesiredSize.Height / 2, (startPoint.Y + endPoint.Y) / 2 + this.linelabel.DesiredSize.Width / 2);
                            }
                            if (this.LabelTemplateOrientation == LabelOrientation.Vertical)
                            {
                                ttransform.Angle = 270;
                            }

                            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                this.LabelTemplatePosition = new Point((startPoint.X + endPoint.X) / 2 - this.linelabel.DesiredSize.Height / 2, (startPoint.Y + endPoint.Y) / 2 + this.linelabel.DesiredSize.Width / 2);
                            }
                        }
                        else if ((this.LabelOrientation == LabelOrientation.Horizontal || this.LabelTemplateOrientation == LabelOrientation.Horizontal) && this.ConnectorType == ConnectorType.Straight)
                        {
                            //  this.LabelAngle = 270;
                            if (this.LabelOrientation == LabelOrientation.Horizontal)
                            {
                                transform.Angle = 0;
                            }
                            if (this.LabelHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                if (!AllowCustomLabelPosition)
                                    this.LabelPosition = new Point((startPoint.X + endPoint.X) / 2 - this.linelabel.DesiredSize.Width / 2, (startPoint.Y + endPoint.Y) / 2 - this.linelabel.DesiredSize.Height / 2);
                            }
                            if (this.LabelTemplateOrientation == LabelOrientation.Horizontal)
                            {
                                ttransform.Angle = 0;
                            }
                            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                this.LabelTemplatePosition = new Point((startPoint.X + endPoint.X) / 2 - this.linelabel.DesiredSize.Width / 2, (startPoint.Y + endPoint.Y) / 2 - this.linelabel.DesiredSize.Height / 2);

                            }
                        }
                        else if (!AllowCustomLabelPosition)
                        {
                            transform.Angle = this.LineAngle + this.LabelAngle;
                            ttransform.Angle = this.LineAngle + this.LabelAngle;
                        }

                        else if (AllowCustomLabelPosition)
                        {
                            transform.Angle = this.LabelAngle;
                            ttransform.Angle = this.LabelAngle;
                        }
                    }
                    if (this.LabelOrientation == LabelOrientation.Auto)
                    {
                        if (!AllowCustomLabelPosition)
                            transform.Angle = this.LineAngle + this.LabelAngle;
                        else
                            transform.Angle = this.LabelAngle;
                    }
                    if (this.LabelTemplateOrientation == LabelOrientation.Auto)
                    {
                        if (!AllowCustomLabelPosition)
                            ttransform.Angle = this.LineAngle + this.LabelAngle;
                        else
                            ttransform.Angle = this.LabelAngle;
                    }
                    (this.linetext.Parent as Grid).RenderTransform = transform;
                    this.labelGrid.RenderTransform = ttransform;

                }
                catch
                {
                }
            }

            if (ConnectorType != ConnectorType.Bezier && ConnectorType != ConnectorType.Arc)
            {
                try
                {
                    this.UpdateVertexsPosition(this);
                }
                catch
                {
                }
            }

            //this.SetLineBridging();
            if (ConnectorPathGeometry != null)
            {
                VirtualConnectorPathGeometry = this.Clone(this.ConnectorPathGeometry);
                this.invalidateBridging = true;
            }
        }

        internal void UpdatePathGeometry(List<Point> ConnectionPoints)
        {
            PathGeometry pathgeometry = new PathGeometry();
            PathFigure pathfigure = new PathFigure();
            Point StartPoint = new Point();
            Point EndPoint = new Point();
            this.misoverlapped = false;

            pathfigure.StartPoint = ConnectionPoints[0];
            StartPoint = pathfigure.StartPoint;
            foreach (Point p in ConnectionPoints)
            {
                LineSegment polyline = new LineSegment();
                polyline.Point = p;
                EndPoint = p;
                pathfigure.Segments.Add(polyline);
            }

            pathgeometry.Figures.Add(pathfigure);
            if (!this.linedragging && this.dview != null)
            {
                this.PxStartPointPosition = ConnectionPoints[0];
                this.PxEndPointPosition = ConnectionPoints.ElementAt(ConnectionPoints.Count() - 1);
            }

            this.m_TempStart = StartPoint;
            this.m_TempEnd = (pathfigure.Segments.Last() as LineSegment).Point;
            this.ConnectionPoints = ConnectionPoints;
            this.ConnectorPathGeometry = pathgeometry;
            UpdateLabel(this.m_TempStart, this.m_TempEnd);
        }

        private void UpdateLabel(Point StartPoint, Point EndPoint)
        {
            if (ConnectionPoints != null)
            {
                UpdateLabelPosition(ConnectionPoints, out StartPoint, out EndPoint);
            }

            #region Label Alignments

            if (this.LabelHorizontalAlignment == HorizontalAlignment.Left && !AllowCustomLabelPosition)
            {
                this.LabelPosition = StartPoint;
                if (this.ConnectorType == ConnectorType.Orthogonal)
                {
                    if (this.ConnectionPoints[1].X > this.ConnectionPoints[2].X)
                    {
                        this.LabelPosition = new Point(StartPoint.X - this.linelabel.ActualWidth, this.LabelPosition.Y);
                    }
                    if (this.ConnectionPoints[0].Y > this.ConnectionPoints[1].Y)
                    {
                        this.LabelPosition = new Point(LabelPosition.X, this.LabelPosition.Y - this.linelabel.ActualHeight);
                    }

                }
                if (this.ConnectorType == ConnectorType.Straight)
                {
                    if (this.LabelOrientation == LabelOrientation.Auto)
                    {
                        double angle = (this.LineAngle * Math.PI) / 180;
                        if (LineAngle > 90 && LineAngle < 270)
                        {
                            LabelAngle = 180;
                            this.LabelPosition = new Point(StartPoint.X + (Math.Cos(angle)) * (this.linelabel.DesiredSize.Width), StartPoint.Y + (Math.Sin(angle) * (this.linelabel.DesiredSize.Width)));
                        }
                        else
                        {
                            LabelAngle = 0;
                            this.LabelPosition = new Point(StartPoint.X + (Math.Cos(angle)), StartPoint.Y + (Math.Sin(angle)));
                        }
                    }
                    else if (this.LabelOrientation == LabelOrientation.Horizontal)
                    {
                        if (this.LineAngle > 0 && this.LineAngle <= 90)
                        {
                            this.LabelPosition = StartPoint;
                        }
                        else if (LineAngle > 90 && LineAngle <= 180)
                        {
                            this.LabelPosition = new Point(StartPoint.X - this.linelabel.DesiredSize.Width, StartPoint.Y);
                        }
                        else if (LineAngle > 180 && LineAngle <= 270)
                        {
                            this.LabelPosition = new Point(StartPoint.X - this.linelabel.DesiredSize.Width, StartPoint.Y - this.linelabel.DesiredSize.Height);

                        }
                        else if (LineAngle > 270 && LineAngle <= 360)
                        {
                            this.LabelPosition = new Point(StartPoint.X, StartPoint.Y - this.linelabel.DesiredSize.Height);
                        }
                    }
                    else if (this.LabelOrientation == LabelOrientation.Vertical && this.ConnectorType == ConnectorType.Straight)
                    {
                        if (this.LineAngle > 0 && this.LineAngle <= 90)
                        {
                            this.LabelPosition = new Point(StartPoint.X, StartPoint.Y + this.linelabel.DesiredSize.Width);
                        }
                        else if (LineAngle > 90 && LineAngle <= 180)
                        {
                            this.LabelPosition = new Point(StartPoint.X - this.linelabel.DesiredSize.Height, StartPoint.Y + this.linelabel.DesiredSize.Width);
                        }
                        else if (LineAngle > 180 && LineAngle <= 270)
                        {
                            this.LabelPosition = new Point(StartPoint.X - this.linelabel.DesiredSize.Height, StartPoint.Y);

                        }
                        else if (LineAngle > 270 && LineAngle <= 360)
                        {
                            this.LabelPosition = StartPoint;
                        }

                    }
                }
            }
            else if (this.LabelHorizontalAlignment == HorizontalAlignment.Right && !AllowCustomLabelPosition)
            {
                this.LabelPosition = EndPoint;
                if (this.ConnectorType == ConnectorType.Orthogonal)
                {
                    EndPoint = this.ConnectionPoints[4];
                    this.LabelPosition = this.ConnectionPoints[4];

                    if (this.ConnectionPoints[4].Y > this.ConnectionPoints[3].Y)
                    {
                        this.LabelPosition = new Point(LabelPosition.X, this.LabelPosition.Y - this.linelabel.ActualHeight);
                    }
                    if (this.ConnectionPoints[1].X > this.ConnectionPoints[2].X)
                    {
                        this.LabelPosition = new Point(EndPoint.X - this.linelabel.ActualWidth, this.LabelPosition.Y);
                    }
                    if (this.ConnectionPoints[4].X > this.ConnectionPoints[3].X)
                    {
                        this.LabelPosition = new Point(LabelPosition.X - this.linelabel.ActualWidth, this.LabelPosition.Y);
                    }
                    else
                    {
                        if ((this.ConnectionPoints[3].Y == this.ConnectionPoints[4].Y) && (this.ConnectionPoints[4].X < this.ConnectionPoints[3].X))
                            this.LabelPosition = new Point(LabelPosition.X + this.linelabel.ActualWidth, this.LabelPosition.Y);
                    }
                }
                else if (this.ConnectorType == ConnectorType.Straight)
                {
                    if (this.LabelOrientation == LabelOrientation.Auto)
                    {
                        double angle = (this.LineAngle * Math.PI) / 180;
                        if (LineAngle > 90 && LineAngle < 270)
                        {
                            LabelAngle = 180;
                            this.LabelPosition = new Point(EndPoint.X - (Math.Cos(angle)), EndPoint.Y - (Math.Sin(angle)));
                        }
                        else
                        {
                            LabelAngle = 0;
                            this.LabelPosition = new Point(EndPoint.X - (Math.Cos(angle) * (this.linelabel.DesiredSize.Width)), EndPoint.Y - (Math.Sin(angle) * (this.linelabel.DesiredSize.Width)));
                        }
                    }
                    else if (this.LabelOrientation == LabelOrientation.Horizontal)
                    {
                        if (this.LineAngle > 0 && this.LineAngle <= 90)
                        {
                            this.LabelPosition = new Point(EndPoint.X - this.linelabel.DesiredSize.Width - 10, EndPoint.Y - this.linelabel.DesiredSize.Height);
                        }
                        else if (LineAngle > 90 && LineAngle <= 180)
                        {
                            this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y - this.linelabel.DesiredSize.Height);
                        }
                        else if (LineAngle > 180 && LineAngle <= 270)
                        {
                            this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y);

                        }
                        else if (LineAngle > 270 && LineAngle <= 360)
                        {
                            this.LabelPosition = new Point(EndPoint.X - this.linelabel.DesiredSize.Width, EndPoint.Y + 5);
                        }

                    }
                    else if (this.LabelOrientation == LabelOrientation.Vertical)
                    {
                        if (this.LineAngle > 0 && this.LineAngle <= 90)
                        {
                            this.LabelPosition = new Point(EndPoint.X - this.linelabel.DesiredSize.Height, EndPoint.Y - 5);
                        }
                        else if (LineAngle > 90 && LineAngle <= 180)
                        {
                            this.LabelPosition = new Point(EndPoint.X, EndPoint.Y - 5);
                        }
                        else if (LineAngle > 180 && LineAngle <= 270)
                        {
                            this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y + this.linelabel.DesiredSize.Width);

                        }
                        else if (LineAngle > 270 && LineAngle <= 360)
                        {
                            this.LabelPosition = new Point(EndPoint.X - this.linelabel.DesiredSize.Height - 5, EndPoint.Y + this.linelabel.DesiredSize.Width);
                        }
                    }

                }
                else
                {
                    double angle = (this.LineAngle * Math.PI) / 180;
                    this.LabelPosition = new Point(EndPoint.X - (Math.Cos(angle) * (this.linelabel.DesiredSize.Width)), EndPoint.Y - (Math.Sin(angle) * (this.linelabel.DesiredSize.Width)));
                }
            }
            else if (!AllowCustomLabelPosition)
            {
                double angle = (this.LineAngle * Math.PI) / 180;
                if (this.ConnectorType != ConnectorType.Orthogonal)
                {
                    if (this.ConnectorType == ConnectorType.Bezier || this.ConnectorType == ConnectorType.Arc)
                    {
                        this.LabelAngle = 0;
                        this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (this.linelabel.DesiredSize.Width + 10)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (this.linelabel.DesiredSize.Width + 10)));
                    }
                    else
                    {
                        if (LineAngle > 90 && LineAngle < 270)
                        {
                            LabelAngle = 180;
                            this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 + (Math.Cos(angle) * (this.linelabel.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 + (Math.Sin(angle) * (this.linelabel.DesiredSize.Width / 2)));
                        }
                        else
                        {
                            LabelAngle = 0;
                            this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (this.linelabel.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (this.linelabel.DesiredSize.Width / 2)));
                        }
                    }
                }
                else
                {
                    this.LabelAngle = 0;
                    this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (this.linelabel.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (this.linelabel.DesiredSize.Width / 2)));
                    //this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2, (StartPoint.Y + EndPoint.Y) / 2 + ((Math.Abs(StartPoint.X - EndPoint.X) < this.linelabel.ActualWidth) ? (this.ConnectionPoints[3].Y - this.ConnectionPoints[0].Y) / 2 : 0));
                }
            }

            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Left)
            {
                this.LabelTemplatePosition = StartPoint;
                if (this.ConnectorType == ConnectorType.Orthogonal)
                {
                    if (this.ConnectionPoints[1].X > this.ConnectionPoints[2].X)
                    {
                        this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.ActualWidth, this.LabelTemplatePosition.Y);
                    }
                    if (this.ConnectionPoints[0].Y > this.ConnectionPoints[1].Y)
                    {
                        this.LabelTemplatePosition = new Point(LabelTemplatePosition.X, this.LabelTemplatePosition.Y - this.labelGrid.ActualHeight);
                    }

                }
                if (this.ConnectorType == ConnectorType.Straight)
                {
                    if (this.LabelTemplateOrientation == LabelOrientation.Auto)
                    {
                        double angle = (this.LineAngle * Math.PI) / 180;
                        if (LineAngle > 90 && LineAngle < 270)
                        {
                            if (!AllowCustomLabelPosition)
                                LabelAngle = 180;
                            this.LabelTemplatePosition = new Point(StartPoint.X + (Math.Cos(angle)) * (this.labelGrid.DesiredSize.Width), StartPoint.Y + (Math.Sin(angle) * (this.labelGrid.DesiredSize.Width)));
                        }
                        else
                        {
                            if (!AllowCustomLabelPosition)
                                LabelAngle = 0;
                            this.LabelTemplatePosition = new Point(StartPoint.X + (Math.Cos(angle)), StartPoint.Y + (Math.Sin(angle)));
                        }
                    }
                    else if (this.LabelTemplateOrientation == LabelOrientation.Horizontal)
                    {
                        if (this.LineAngle > 0 && this.LineAngle <= 90)
                        {
                            this.LabelTemplatePosition = StartPoint;
                        }
                        else if (LineAngle > 90 && LineAngle <= 180)
                        {
                            this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.DesiredSize.Width, StartPoint.Y);
                        }
                        else if (LineAngle > 180 && LineAngle <= 270)
                        {
                            this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.DesiredSize.Width, StartPoint.Y - this.labelGrid.DesiredSize.Height);

                        }
                        else if (LineAngle > 270 && LineAngle <= 360)
                        {
                            this.LabelTemplatePosition = new Point(StartPoint.X, StartPoint.Y - this.labelGrid.DesiredSize.Height);
                        }
                    }
                    else if (this.LabelTemplateOrientation == LabelOrientation.Vertical && this.ConnectorType == ConnectorType.Straight)
                    {
                        if (this.LineAngle > 0 && this.LineAngle <= 90)
                        {
                            this.LabelTemplatePosition = new Point(StartPoint.X, StartPoint.Y + this.labelGrid.DesiredSize.Width);
                        }
                        else if (LineAngle > 90 && LineAngle <= 180)
                        {
                            this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.DesiredSize.Height, StartPoint.Y + this.labelGrid.DesiredSize.Width);
                        }
                        else if (LineAngle > 180 && LineAngle <= 270)
                        {
                            this.LabelTemplatePosition = new Point(StartPoint.X - this.labelGrid.DesiredSize.Height, StartPoint.Y);

                        }
                        else if (LineAngle > 270 && LineAngle <= 360)
                        {
                            this.LabelTemplatePosition = StartPoint;
                        }

                    }

                }
            }
            else if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Right)
            {
                if (this.ConnectorType == ConnectorType.Orthogonal)
                {
                    EndPoint = this.ConnectionPoints[4];
                    this.LabelTemplatePosition = this.ConnectionPoints[4];

                    if (this.ConnectionPoints[4].Y > this.ConnectionPoints[3].Y)
                    {
                        this.LabelTemplatePosition = new Point(LabelTemplatePosition.X, this.LabelTemplatePosition.Y - this.labelGrid.ActualHeight);
                    }
                    if (this.ConnectionPoints[1].X > this.ConnectionPoints[2].X)
                    {
                        this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.ActualWidth, this.LabelTemplatePosition.Y);
                    }
                    if (this.ConnectionPoints[4].X > this.ConnectionPoints[3].X)
                    {
                        this.LabelTemplatePosition = new Point(LabelTemplatePosition.X - this.labelGrid.ActualWidth, this.LabelTemplatePosition.Y);
                    }
                    else
                    {
                        if ((this.ConnectionPoints[3].Y == this.ConnectionPoints[4].Y) && (this.ConnectionPoints[4].X < this.ConnectionPoints[3].X))
                            this.LabelTemplatePosition = new Point(LabelTemplatePosition.X + this.labelGrid.ActualWidth, this.LabelTemplatePosition.Y);
                    }
                }
                else if (this.ConnectorType == ConnectorType.Straight)
                {
                    if (this.LabelTemplateOrientation == LabelOrientation.Auto)
                    {
                        double angle = (this.LineAngle * Math.PI) / 180;
                        if (LineAngle > 90 && LineAngle < 270)
                        {
                            if (!AllowCustomLabelPosition)
                                LabelAngle = 180;
                            this.LabelTemplatePosition = new Point(EndPoint.X - (Math.Cos(angle)), EndPoint.Y - (Math.Sin(angle)));
                        }
                        else
                        {
                            if (!AllowCustomLabelPosition)
                                LabelAngle = 0;
                            this.LabelTemplatePosition = new Point(EndPoint.X - (Math.Cos(angle) * (this.labelGrid.DesiredSize.Width)), EndPoint.Y - (Math.Sin(angle) * (this.labelGrid.DesiredSize.Width)));
                        }
                    }
                    else if (this.LabelTemplateOrientation == LabelOrientation.Horizontal)
                    {
                        if (this.LineAngle > 0 && this.LineAngle <= 90)
                        {
                            this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.DesiredSize.Width - 10, EndPoint.Y - this.labelGrid.DesiredSize.Height);
                        }
                        else if (LineAngle > 90 && LineAngle <= 180)
                        {
                            this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y - this.labelGrid.DesiredSize.Height);
                        }
                        else if (LineAngle > 180 && LineAngle <= 270)
                        {
                            this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y);

                        }
                        else if (LineAngle > 270 && LineAngle <= 360)
                        {
                            this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.DesiredSize.Width, EndPoint.Y + 5);
                        }

                    }
                    else if (this.LabelTemplateOrientation == LabelOrientation.Vertical)
                    {
                        if (this.LineAngle > 0 && this.LineAngle <= 90)
                        {
                            this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.DesiredSize.Height, EndPoint.Y - 5);
                        }
                        else if (LineAngle > 90 && LineAngle <= 180)
                        {
                            this.LabelTemplatePosition = new Point(EndPoint.X, EndPoint.Y - 5);
                        }
                        else if (LineAngle > 180 && LineAngle <= 270)
                        {
                            this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y + this.labelGrid.DesiredSize.Width);

                        }
                        else if (LineAngle > 270 && LineAngle <= 360)
                        {
                            this.LabelTemplatePosition = new Point(EndPoint.X - this.labelGrid.DesiredSize.Height - 5, EndPoint.Y + this.labelGrid.DesiredSize.Width);
                        }
                    }

                }

                else
                {
                    double angle = (this.LineAngle * Math.PI) / 180;
                    this.LabelTemplatePosition = new Point(EndPoint.X - (Math.Cos(angle) * (this.labelGrid.ActualWidth + 10)), EndPoint.Y - (Math.Sin(angle) * (this.labelGrid.ActualWidth + 10)));
                }
            }
            else
            {
                double angle = (this.LineAngle * Math.PI) / 180;
                if (this.ConnectorType != ConnectorType.Orthogonal)
                    this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (this.labelGrid.ActualHeight + 10)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (this.labelGrid.ActualHeight + 10)));
                else
                {
                    this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2, (StartPoint.Y + EndPoint.Y) / 2 + ((Math.Abs(StartPoint.X - EndPoint.X) < this.labelGrid.ActualWidth) ? (this.ConnectionPoints[3].Y - this.ConnectionPoints[0].Y) / 2 : 0));
                }
            }

            #endregion
        }

        private void UpdateLabelPosition(List<Point> pts, out Point StartPoint, out Point EndPoint)
        {
            int count = 0;
            double total = 0;
            double mid = 0;
            List<double> lengthList = findLength(pts);

            if (lengthList != null)
            {
                foreach (double d in lengthList)
                {
                    mid += d;
                }
                mid = mid / 2;

                foreach (double d in lengthList)
                {
                    total += d;
                    if (mid <= total)
                        break;
                    count++;
                }
            }

            StartPoint = pts[count];
            EndPoint = pts[count + 1];
            if (ConnectorType == ConnectorType.Straight)
                this.LineAngle = FindAngle(StartPoint, EndPoint);
        }

        private static double findlength(Point s, Point e)
        {
            double length;
            length = Math.Sqrt(Math.Pow((s.X - e.X), 2) + Math.Pow((s.Y - e.Y), 2));
            return length;
        }

        private static Point findcenterpoint(Point s, Point e)
        {
            Point temp = new Point((s.X + e.X) / 2, (s.Y + e.Y) / 2);
            return temp;
        }

        /// <summary>
        /// Finds the length.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private static List<double> findLength(List<Point> list)
        {
            List<double> length = new List<double>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                length.Add(findHypo(list[i], list[i + 1]));
            }
            return length;
        }

        private void adjustIntermediatePoints(List<Point> connectionPoints)
        {
            if (ConnectorType == ConnectorType.Orthogonal && IntermediatePoints.Count > 2)
            {
                bool foundDefect = false;
                for (int i = 0; i < connectionPoints.Count - 1; i++)
                {
                    if (!((connectionPoints[i].X == connectionPoints[i + 1].X) ||
                        (connectionPoints[i].Y == connectionPoints[i + 1].Y)))
                    {
                        double del = Math.Min(Math.Abs(connectionPoints[i].X - connectionPoints[i + 1].X)
                            , Math.Abs(connectionPoints[i].Y - connectionPoints[i + 1].Y));
                        if (del > 10)
                        {
                            foundDefect = true;
                        }
                    }
                }
                if (foundDefect)
                {
                    for (int i = 0; i < connectionPoints.Count - 1; i++)
                    {
                        if (i % 2 != 0)
                        {
                            connectionPoints[i + 1] = new Point(connectionPoints[i + 1].X, connectionPoints[i].Y);
                        }
                        else
                        {
                            connectionPoints[i + 1] = new Point(connectionPoints[i].X, connectionPoints[i + 1].Y);
                        }
                    }
                    for (int i = 0; i < IntermediatePoints.Count; i++)
                    {
                        IntermediatePoints[i] = connectionPoints[i + 1];
                    }
                    this.PxEndPointPosition = connectionPoints[connectionPoints.Count - 1];
                }
            }
        }

        internal bool invalidateBridging = true;

        internal bool bridged = true;

        private bool IsCPGOverlapped(Rect rect, Rect rect_2)
        {
            return true;// rect.IntersectsWith(rect_2);
        }

        private List<Point> GetPoints()
        {
            return GetPoints(this.VirtualConnectorPathGeometry);
        }

        private List<Point> GetPoints(PathGeometry geom)
        {
            List<Point> pts = new List<Point>();
            PathFigure fig = geom.Figures[0];
            pts.Add(fig.StartPoint);
            foreach (PathSegment seg in fig.Segments)
            {
                if (seg is LineSegment)
                {
                    pts.Add((seg as LineSegment).Point);
                }
                else if (seg is PolyLineSegment)
                {
                    pts.AddRange((seg as PolyLineSegment).Points);
                }
                else if (seg is BezierSegment)
                {
                    pts.AddRange(BezireToPoly(pts[pts.Count - 1], seg as BezierSegment));
                }
                //else if (seg is ArcSegment)
                //{
                //    pts.AddRange(ArcToPoly(pts[pts.Count - 1], seg as ArcSegment));
                //}
            }
            return pts;
        }

        private Point GetPointAtLength(PathGeometry geom, double length)
        {
            List<Point> pts = GetPoints(geom);
            double run = 0;
            Point? pre = null;
            Point found = new Point(0, 0);
            foreach (Point pt in pts)
            {
                if (!pre.HasValue)
                {
                    pre = pt;
                    continue;
                }
                else
                {
                    double l = findHypo(pre.Value, pt);
                    if (run + l > length)
                    {
                        double r = length - run;
                        double deg = findAngle(pre.Value, pt);
                        double x = r * Math.Cos(deg * Math.PI / 180);
                        double y = r * Math.Sin(deg * Math.PI / 180);
                        found = new Point(pre.Value.X + x, pre.Value.Y + y);
                        break;
                    }
                    else
                    {
                        run += l;
                    }
                }
                pre = pt;
            }
            return found;
        }

        private PathGeometry Clone(PathGeometry geom)
        {
            PathFigure fig = geom.Figures[0];
            PathFigure clone = new PathFigure();
            clone.StartPoint = fig.StartPoint;
            foreach (PathSegment seg in fig.Segments)
            {
                if (seg is LineSegment)
                {
                    clone.Segments.Add(new LineSegment() { Point = (seg as LineSegment).Point });
                }
                else if (seg is BezierSegment)
                {
                    clone.Segments.Add(new BezierSegment()
                    {
                        Point1 = (seg as BezierSegment).Point1,
                        Point2 = (seg as BezierSegment).Point2,
                        Point3 = (seg as BezierSegment).Point3
                    });
                }
                //Arc not supported

                //else if(seg is ArcSegment)
                //{
                //    clone.Segments.Add(new ArcSegment(){ IsLargeArc = (seg as ArcSegment).IsLargeArc,
                //                                         Point = (seg as ArcSegment).Point,
                //                                         RotationAngle = (seg as ArcSegment).RotationAngle,
                //                                         Size = (seg as ArcSegment).Size,
                //                                         SweepDirection = (seg as ArcSegment).SweepDirection});
                //}
            }
            return new PathGeometry() { Figures = new PathFigureCollection() { clone } };
        }

        internal void SetLineBridging()
        {

            if (this.dc != null && this.LineBridgingEnabled)
            {
                Dictionary<int, List<ArcSegment>> arcD = new Dictionary<int, List<ArcSegment>>();
                Dictionary<int, List<Point>> startBD = new Dictionary<int, List<Point>>();
                int count = -1;
                foreach (LineConnector lc in dc.Model.Connections)
                
                
                {
                    if ((lc.zOrder < this.zOrder || lc.ConnectorType == ConnectorType.Bezier || lc.ConnectorType == Diagram.ConnectorType.Arc || !lc.LineBridgingEnabled)
                        && (lc.VirtualConnectorPathGeometry != null && this.VirtualConnectorPathGeometry != null && this.VirtualConnectorPathGeometry.Bounds != Rect.Empty && lc.VirtualConnectorPathGeometry.Bounds != Rect.Empty)
                        && (lc.invalidateBridging || this.invalidateBridging)
                        )
                    {
                        if (IsCPGOverlapped(this.VirtualConnectorPathGeometry.Bounds, lc.VirtualConnectorPathGeometry.Bounds))
                        {
                            Point[] pts;
                            //if (!lc.Equals(this))
                            //{
                            //    pts = GetIntersectionPointsFromWidened(this.WidenedPathGeometry, lc.WidenedPathGeometry);
                            //}
                            //else
                            //{
                            List<Point> pts1;

                            List<Point> line1 = new List<Point>();
                            line1 = GetPoints();

                            List<Point> line2 = new List<Point>();
                            //line2.Add(lc.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                            //line2.AddRange(lc.IntermediatePoints);
                            //line2.Add((lc.VirtualConnectorPathGeometry.Figures[0].Segments.Last<PathSegment>() as LineSegment).Point);
                            line2 = lc.GetPoints();

                            pts1 = FindPOIBetweenTwoPolyLine(line1, line2, false);
                            pts = (pts1.ToArray<Point>()).ToArray<Point>();
                            //}

                            foreach (Point p in pts)
                            {
                                double fullLength;
                                int segmentIndex;
                                double length = GetLengthAtFractionPoint(this.VirtualConnectorPathGeometry.Figures[0], p, out fullLength, out segmentIndex);
                                if (segmentIndex < 0)
                                {
                                    continue;
                                }
                                Point startBridge, endBridge;
                                double fractLength = (length - (this.BridgeSpacing / 2)) / fullLength;
                                //this.VirtualConnectorPathGeometry.GetPointAtFractionLength(fractLength, out startBridge, out dummy);
                                startBridge = GetPointAtLength(VirtualConnectorPathGeometry, (length - (this.BridgeSpacing / 2)));
                                fractLength = (length + (this.BridgeSpacing / 2)) / fullLength;
                                //this.VirtualConnectorPathGeometry.GetPointAtFractionLength(fractLength, out endBridge, out dummy);
                                endBridge = GetPointAtLength(VirtualConnectorPathGeometry, (length + (this.BridgeSpacing / 2)));

                                SweepDirection sd;

                                Point start, end;
                                if (segmentIndex == 0)
                                {
                                    start = (this.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                                }
                                else
                                {
                                    start = ((this.VirtualConnectorPathGeometry.Figures[0].Segments[segmentIndex - 1] as LineSegment).Point);
                                }
                                end = (this.VirtualConnectorPathGeometry.Figures[0].Segments[segmentIndex] as LineSegment).Point;
                                double angle = findAngle(start, end);
                                if (angle > 0 && angle < 180)
                                {
                                    sd = SweepDirection.Clockwise;
                                }
                                else
                                {
                                    sd = SweepDirection.Counterclockwise;
                                }

                                if (arcD.ContainsKey(segmentIndex))
                                {
                                    Point fixedpoint;
                                    if (segmentIndex == 0)
                                    {
                                        fixedpoint = (this.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                                    }
                                    else
                                    {
                                        fixedpoint = ((this.VirtualConnectorPathGeometry.Figures[0].Segments[segmentIndex - 1] as LineSegment).Point);
                                    }

                                    double fix = Math.Abs(findHypo(fixedpoint, endBridge));
                                    double var;

                                    int insertAt = -1;
                                    count = -1;
                                    foreach (ArcSegment arc in arcD[segmentIndex])
                                    {
                                        count++;
                                        var = Math.Abs(findHypo(fixedpoint, arc.Point));
                                        if (fix < var)
                                        {
                                            insertAt = count;
                                            break;
                                        }
                                    }
                                    if (insertAt >= 0)
                                    {
                                        arcD[segmentIndex].Insert(insertAt, new ArcSegment() { Point = endBridge, Size = new Size(1, 1), RotationAngle = 0, IsLargeArc = true, SweepDirection = sd });
                                        startBD[segmentIndex].Insert(insertAt, startBridge);
                                    }
                                    else
                                    {
                                        arcD[segmentIndex].Add(new ArcSegment() { Point = endBridge, Size = new Size(1, 1), RotationAngle = 0, IsLargeArc = true, SweepDirection = sd });
                                        startBD[segmentIndex].Add(startBridge);
                                    }
                                }
                                else
                                {
                                    if (!double.IsNaN(startBridge.X) && !double.IsNaN(startBridge.Y) && !endBridge.Equals(new Point(0, 0)))
                                    {
                                        List<ArcSegment> arcs = new List<ArcSegment>();
                                        arcs.Add(new ArcSegment() { Point = endBridge, Size = new Size(1, 1), RotationAngle = 0, IsLargeArc = true, SweepDirection = sd });
                                        List<Point> points = new List<Point>();
                                        points.Add(startBridge);
                                        arcD.Add(segmentIndex, arcs);
                                        startBD.Add(segmentIndex, points);
                                    }
                                }
                            }
                        }
                    }
                }
                if (arcD.Count != 0)
                {

                    var v = (from k in arcD.Keys orderby k ascending select k);
                    count = -1;
                    if (arcD.Count > 0)
                    {
                        this.ConnectorPathGeometry = Clone(VirtualConnectorPathGeometry); //this.VirtualConnectorPathGeometry.Clone();
                    }
                    foreach (int Index in v)
                    {
                        this.bridged = true;
                        for (int i = 1; i < arcD[Index].Count; i++)
                        {
                            if (findHypo(arcD[Index][i].Point, arcD[Index][i - 1].Point) < BridgeSpacing)
                            {
                                arcD[Index][i - 1].Point = arcD[Index][i].Point;
                                arcD[Index].RemoveAt(i);
                                startBD[Index].RemoveAt(i);
                                i--;
                            }
                        }

                        int segmentIndex;
                        var item = arcD[Index];
                        foreach (ArcSegment arc in arcD[Index])
                        {
                            count++;
                            segmentIndex = Index + (2 * count);
                            Point end = (ConnectorPathGeometry.Figures[0].Segments[segmentIndex] as LineSegment).Point;
                            ConnectorPathGeometry.Figures[0].Segments.Insert(segmentIndex + 1, arc);
                            ConnectorPathGeometry.Figures[0].Segments.Insert(segmentIndex + 2, new LineSegment() { Point = end });
                            (ConnectorPathGeometry.Figures[0].Segments[segmentIndex] as LineSegment).Point = startBD[Index][arcD[Index].IndexOf(arc)];
                        }
                    }
                }
                else
                {
                    foreach (LineConnector l in dc.Model.Connections)
                    {
                        if (l == this)
                        {
                            l.UpdateConnectorPathGeometry();
                        }
                    }
                }
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
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
            {
                if (this.LabelVisibility == Visibility.Visible)
                {
                    this.Label = this.linetext.Text;
                    this.linelabel.Text = this.linetext.Text;
                    this.linelabel.Visibility = Visibility.Visible;
                    this.linetext.Visibility = Visibility.Collapsed;
                }
            }

        }
        #region Class Fields

        #endregion

        #region Initialization

        #endregion


        #region Properties

        #endregion

        #region Class Override

        #endregion

        #region Methods

        internal void LineSelection(LineConnector line)
        {
            if (line.IsSelected && line.IsDecoratorVisible)
            {
                if (dc != null)
                {
                    if ((dc.View.Page as DiagramPage) != null && (dc.View.Page as DiagramPage).SelectionList != null)
                    {
                        (dc.View.Page as DiagramPage).SelectionList.Add(line);
                    }
                }
                if (dview != null)
                {
                    dview.Selectedelement = line;
                }
                n++;
                if (line.HeadDecoratorGrid != null)
                {
                    headThumb = new Thumb();
                    headThumb.Name = "PART_HeadBorder" + n.ToString();
                    headThumb.Style = this.DecoratorAdornerStyle;

                    Canvas.SetLeft(headThumb, this.HeadDecoratorPosition.X);
                    Canvas.SetTop(headThumb, this.HeadDecoratorPosition.Y);
                    //headThumb.Angle = line.HeadDecoratorAngle;

                    try
                    {
                        headthumb = true;

                        headThumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
                        headThumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
                        headThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
                        headThumb.MouseLeftButtonUp += new MouseButtonEventHandler(Thumb_MouseLeftButtonUp);
                        headThumb.MouseMove += new MouseEventHandler(Thumb_MouseMove);

                        line.HeadDecoratorGrid.Children.Add(headThumb);
                        //line.HeadDecoratorGrid.Height = headThumb.Height;
                        //line.HeadDecoratorGrid.Width = headThumb.Width;
                    }
                    catch
                    {
                    }
                }

                if (line.TailDecoratorGrid != null)
                {
                    tailThumb = new Thumb();
                    tailThumb.Name = "PART_TailBorder" + n.ToString();
                    tailThumb.Style = this.DecoratorAdornerStyle;

                    Canvas.SetLeft(tailThumb, this.TailDecoratorPosition.X);
                    Canvas.SetTop(tailThumb, this.TailDecoratorPosition.Y);
                    //tailThumb.Angle = line.TailDecoratorAngle;

                    try
                    {
                        tailthumb = true;

                        tailThumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
                        tailThumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
                        tailThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
                        tailThumb.MouseLeftButtonUp += new MouseButtonEventHandler(Thumb_MouseLeftButtonUp);
                        tailThumb.MouseMove += new MouseEventHandler(Thumb_MouseMove);

                        line.TailDecoratorGrid.Children.Add(tailThumb);
                        //line.TailDecoratorGrid.Height = tailThumb.Height;
                        //line.TailDecoratorGrid.Width = tailThumb.Width;
                    }
                    catch
                    {
                    }
                }
                if (vertexItems != null)
                    line.vertexItems.Visibility = Visibility.Visible;
            }
            else
            {
                if (!line.IsSelected)
                {
                    if (dc != null)
                    {
                        if ((dc.View.Page as DiagramPage) != null && (dc.View.Page as DiagramPage).SelectionList != null)
                        {
                            (dc.View.Page as DiagramPage).SelectionList.Remove(line);
                        }
                    }

                }
                if (line.HeadDecoratorGrid != null && line.HeadDecoratorGrid.Children.Count >= 2)
                {
                    line.HeadDecoratorGrid.Children.Remove(line.HeadDecoratorGrid.Children.ElementAt(1));
                }

                if (line.TailDecoratorGrid != null && line.TailDecoratorGrid.Children.Count >= 2)
                {
                    line.TailDecoratorGrid.Children.Remove(line.TailDecoratorGrid.Children.ElementAt(1));
                }
                if (vertexItems != null)
                    line.vertexItems.Visibility = Visibility.Collapsed;
            }


        }

        void Thumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsDecoratorMovable)
            {
                (sender as Thumb).Cursor = Cursors.Arrow;
            }
            else
            {
                (sender as Thumb).Cursor = Cursors.Hand;
            }
        }

        void Vertex_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsVertexMovable)
            {
                (sender as Thumb).Cursor = Cursors.Arrow;
            }
            else
            {
                (sender as Thumb).Cursor = Cursors.Hand;
            }
        }

        void Thumb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private String _lastClickedItem = String.Empty;
        private DateTime _lastClickedTime = DateTime.Now;
        private double _doubleClickIntervalTimeInMillisecond = 500d; //0.2 second 

        void Thumb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DateTime currentTime = DateTime.Now;

            if (sender is Thumb)
            {
                Thumb item = sender as Thumb;
                String currentClickedItem = item.Name;

                double intervalBetweenTwoClicks = currentTime.Subtract(_lastClickedTime).TotalMilliseconds;

                if (intervalBetweenTwoClicks < _doubleClickIntervalTimeInMillisecond
                    && intervalBetweenTwoClicks > 0
                    && _lastClickedItem == currentClickedItem && IsLabelEditable)
                {
                    ConnRoutedEventArgs newEventArgs = new ConnRoutedEventArgs(this);
                    dview.OnConnectorDoubleClick(this, newEventArgs);
                    LabelEditConnRoutedEventArgs newEventArgs1 = new LabelEditConnRoutedEventArgs(this.linelabel.Text, this);
                    dview.OnConnectorStartLabelEdit(this, newEventArgs1);
                    this.linelabel.Visibility = Visibility.Collapsed;
                    this.linetext.Visibility = Visibility.Visible;
                    this.linetext.SelectionBackground = new SolidColorBrush(Colors.Blue);
                    this.linetext.Focus();
                    this.linetext.SelectAll();
                    this.LostFocus += new RoutedEventHandler(Node_LostFocus);
                }

                _lastClickedItem = item.Name;
                _lastClickedTime = DateTime.Now;
            }

        }

        #endregion
    }
}
