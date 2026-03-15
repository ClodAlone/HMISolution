// <copyright file="ConnectionPort.cs" company="Syncfusion">
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
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Data;

    /// <summary>
    /// Represents a port which can be used to make connections to the node.
    /// </summary>
    /// <remarks>
    /// The <see cref="ConnectionPort"/> class can be used for defining custom ports on the nodes.
    /// <para/>
    /// Any number of ports can be defined on a node.  By default every node has a center port. 
    /// </remarks>
    /// <example>
    /// C#:
    /// <para/>
    /// The following example shows how to create a <see cref="ConnectionPort"/> in C#.
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
    ///        InitializeComponent ();
    ///        Control = new DiagramControl ();
    ///        Model = new DiagramModel ();
    ///        View = new DiagramView ();
    ///        Control.View = View;
    ///        Control.Model = Model;
    ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
    ///        //Creates a node
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
    ///        //Define a Custom port for the node.
    ///        ConnectionPort port = new ConnectionPort();
    ///        port.Node=n;
    ///        port.Left=75;
    ///        port.Top=10;
    ///        port.PortShape = PortShapes.Arrow;
    ///        port.PortStyle.Fill = Brushes.Transparent;
    ///        port.Height = 11;
    ///        port.Width = 11;
    ///        n.Ports.Add(port);
    ///        Node n1 = new Node(Guid.NewGuid(), "Decision1");
    ///        n1.Shape = Shapes.FlowChart_Process;
    ///        n1.IsLabelEditable = true;
    ///        n1.Label = "Alarm Rings";
    ///        n1.Level = 2;
    ///        n1.OffsetX = 150;
    ///        n1.OffsetY = 125;
    ///        n1.Width = 150;
    ///        n1.Height = 75;
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
    ///        LineConnector o2 = new LineConnector();
    ///        o2.ConnectorType = ConnectorType.Straight;
    ///        o2.TailNode = n1;
    ///        o2.HeadNode = n;
    ///        o2.LabelHorizontalAlignment = HorizontalAlignment.Center;
    ///        //Making connection to the ports.
    ///        o2.ConnectionHeadPort = port;
    ///        o2.ConnectionTailPort = port1;
    ///        Model.Connections.Add(o2);
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    /// <seealso cref="Node"/>
    public class ConnectionPort : Control
    {

        public PortVisibility PortVisibility
        {
            get { return (PortVisibility)GetValue(PortVisibilityProperty); }
            set { SetValue(PortVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PortVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PortVisibilityProperty =
            DependencyProperty.Register("PortVisibility", typeof(PortVisibility), typeof(ConnectionPort), new PropertyMetadata((PortVisibility.MouseOverNode),new PropertyChangedCallback(OnPortVisibilityChanged)));

        private static void OnPortVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ConnectionPort port = d as ConnectionPort;
            port.UpdateVisibility();
        }

        /// <summary>
        /// Identifies the CenterPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterPositionProperty = DependencyProperty.Register("CenterPosition", typeof(Point), typeof(ConnectionPort), new PropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnCenterPositionChanged)));

        /// <summary>
        /// Identifies the IsDragOverPort dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsDragOverPortProperty = DependencyProperty.Register("IsDragOverPort", typeof(bool), typeof(ConnectionPort), new PropertyMetadata(false,new PropertyChangedCallback(OnIsDragPortOverChanged)));


        /// <summary>
        /// Identifies the IsDragOverPort dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsMouseOverProperty = DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(ConnectionPort), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the Left dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(ConnectionPort), new PropertyMetadata(0d, new PropertyChangedCallback(OnOffsetXChanged)));

        /// <summary>
        /// Identifies the Top dependency property.
        /// </summary>
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(ConnectionPort), new PropertyMetadata(0d, new PropertyChangedCallback(OnOffsetYChanged)));

        /// <summary>
        /// Refers to the center port reference number.
        /// </summary>
        private int cportref = -1;

        /// <summary>
        /// Represents the port pointer.
        /// </summary>
        private Border customportPointer;

        /// <summary>
        /// Represents the port style.
        /// </summary>
        private PortStyle customPortStyle;

        /// <summary>
        /// Represents the current port position.
        /// </summary>
        private Point endpoint;

        /*
        /// <summary>
        /// Boolean value indicating mouse over the node.
        /// </summary>
        //private bool mmouseover = false;
         */

        /// <summary>
        /// Refers to the name of the port.
        /// </summary>
        private string mname;

        /// <summary>
        /// Represents the current node .
        /// </summary>
        private Node mnode;

        /// <summary>
        /// Represents the previous port position.
        /// </summary>
        private Point mpreviousOriginPoint;

        /// <summary>
        /// Represents the port shape.
        /// </summary>
        private PortShapes mshape = PortShapes.Diamond;

        /// <summary>
        /// Refers to the node reference number.
        /// </summary>
        private int noderef;

        /// <summary>
        /// Represents the port reference number.
        /// </summary>
        private int portref = -1;

        internal bool IsMouseDown = false;

        //private Visibility portvisibility = Visibility.Collapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPort"/> class .
        /// </summary>
        /// <param name="node">The node hosting this port.</param>
        /// <param name="position">Position of the port</param>
        public ConnectionPort(Node node, Point position)
        {
            this.DefaultStyleKey = typeof(ConnectionPort);
            this.Node = node;
            this.PxLeft = position.X;
            this.PxTop = position.Y;
            this.Loaded += new RoutedEventHandler(ConnectionPort_Loaded);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPort"/> class.
        /// </summary>
        /// <param name="node">The node hosting this port.</param>
        public ConnectionPort(Node node)
        {
            this.DefaultStyleKey = typeof(ConnectionPort);
            this.Node = node;
            this.Loaded += new RoutedEventHandler(ConnectionPort_Loaded);
            // this.LayoutUpdated += new EventHandler(ConnectionPort_LayoutUpdated);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPort"/> class .
        /// </summary>
        public ConnectionPort()
        {
            this.DefaultStyleKey = typeof(ConnectionPort);
            this.Loaded += new RoutedEventHandler(ConnectionPort_Loaded);
            this.Visibility = System.Windows.Visibility.Collapsed;
            //  this.LayoutUpdated += new EventHandler(ConnectionPort_LayoutUpdated);
            this.SizeChanged += new SizeChangedEventHandler(ConnectionPort_SizeChanged);
            this.RenderTransform = new TranslateTransform();
        }

        void ConnectionPort_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTop();
            UpdateLeft();
        }

        /// <summary>
        /// Gets or sets the center port reference no.
        /// </summary>
        /// <value>The center port reference no.</value>
        /// <remarks>
        /// Used for serialization purpose.
        /// </remarks>
        public int CenterPortReferenceNo
        {
            get { return this.cportref; }
            set { this.cportref = value; }
        }

        internal double PxLeft
        {
            get
            {
                if (Node != null)
                {
                    return MeasureUnitsConverter.ToPixels(Left, Node.MeasurementUnits);
                }
                else
                {
                    return Left;
                }
            }
            set
            {
                if (Node != null)
                {
                    Left = MeasureUnitsConverter.FromPixels(value, Node.MeasurementUnits);
                }
                else
                {
                    Left = value;
                }
            }
        }

        internal Point Oldpoint;
        internal Size mNodeoldSize;
        internal Point oldCenterPosition;
        /// <summary>
        /// Gets or sets the left position of the port. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Port left position.
        /// </value>
        /// <example>
        /// C#:
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
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.View = View;
        ///        Control.Model = Model;
        ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "NewNode");
        ///        n.Shape = Shapes.Rectangle;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="New Node";
        ///        Model.Nodes.Add(n);
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        //Specifies the node
        ///        port.Node=n;
        ///        //Specifies the left position of the port.
        ///        port.Left=75;
        ///        port.Top=10;
        ///        //Specifies the port shape
        ///        port.PortShape = PortShapes.Arrow;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double Left
        {
            get
            {
                return (double)GetValue(LeftProperty);
            }

            set
            {
                SetValue(LeftProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the identifying name of the element. The name provides a reference so that code-behind, such as event handler code, can refer to a markup element after it is constructed during processing by a XAML processor. This is a dependency property.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the element. The default is an empty string.</returns>
        internal new string Name
        {
            get { return this.mname; }
            set { this.mname = value; }
        }

        /// <summary>
        /// Gets or sets the node which is hosting this port.
        /// </summary>
        /// <value>
        /// Type: <see cref="Node"/>
        /// Node object.
        /// </value>
        /// <remarks>
        /// Any number of <see cref="ConnectionPort"/> can be specified for a <see cref="Node"/>.
        /// <para/>
        /// By default every node has a center port.
        /// </remarks>
        /// <example>
        /// C#:
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
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.View = View;
        ///        Control.Model = Model;
        ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "NewNode");
        ///        n.Shape = Shapes.Rectangle;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="New Node";
        ///        Model.Nodes.Add(n);
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        //Specifies the node
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        //Specifies the port shape
        ///        port.PortShape = PortShapes.Arrow;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public Node Node
        {
            get
            {
                return this.mnode;
            }

            set
            {
                OnNodeChanged(mnode, value);
                this.mnode = value;
            }
        }

        private void OnNodeChanged(Diagram.Node oldNode, Diagram.Node newNode)
        {
            if (oldNode != null)
            {
                newNode.MouseOverEvent -= new EventHandler(newNode_MouseOverEvent);
            }
            if (newNode != null)
            {
                newNode.MouseOverEvent += new EventHandler(newNode_MouseOverEvent);
            }

            if (newNode != null && this.ReadLocalValue(PortVisibilityProperty).Equals(DependencyProperty.UnsetValue))
            {
                Binding portVisibilityBinding = new Binding("PortVisibility");
                portVisibilityBinding.Source = newNode;
                this.SetBinding(PortVisibilityProperty, portVisibilityBinding);
            }
        }

        void newNode_MouseOverEvent(object sender, EventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            switch (PortVisibility)
            {
                case Diagram.PortVisibility.AlwaysHidden:
                    this.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Diagram.PortVisibility.AlwaysVisible:
                    this.Visibility = System.Windows.Visibility.Visible;
                    break;
                case Diagram.PortVisibility.MouseOverNode:
                    if (Node != null)
                    {
                        if (Node.IsMouseOver || Node.IsDragConnectionOver)
                        {
                            if (Node.dview != null && Node.dview.IsPageEditable)
                            {
                                this.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                this.Visibility = System.Windows.Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            this.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        this.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the port node reference.
        /// </summary>
        /// <value>The port node reference.</value>
        /// <remarks>Used for serialization purpose</remarks>
        public int PortNodeReference
        {
            get { return this.noderef; }
            set { this.noderef = value; }
        }

        /// <summary>
        /// Gets or sets the port reference no.
        /// </summary>
        /// <value>The port reference no.</value>
        /// <remarks>
        /// Used for serialization purpose.
        /// </remarks>
        public int PortReferenceNo
        {
            get { return this.portref; }
            set { this.portref = value; }
        }

        /// <summary>
        /// Gets or sets the port shape.
        /// </summary>
        /// <value>
        /// Type: <see cref="PortShapes"/>
        /// Enum specifying the port shapes.
        /// </value>
        /// <remarks>
        /// By default the port shape is Diamond. 
        /// </remarks>
        /// <example>
        /// C#:
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
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.View = View;
        ///        Control.Model = Model;
        ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "NewNode");
        ///        n.Shape = Shapes.Rectangle;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="New Node";
        ///        Model.Nodes.Add(n);
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        //Specifies the port shape
        ///        port.PortShape = PortShapes.Arrow;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public PortShapes PortShape
        {
            get
            {
                return this.mshape;
            }

            set
            {
                this.mshape = value;
            }
        }

        /// <summary>
        /// Gets or sets the port style.
        /// </summary>
        /// <value>
        /// Type: <see cref="PortStyle"/>
        /// The style to be applied.</value>
        /// <remarks> Several customizable properties have been provided which can be accessed using the <see cref="PortStyle"/> class.</remarks>
        /// <example>
        /// C#:
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
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.View = View;
        ///        Control.Model = Model;
        ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        //Creates a node
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
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        port.Node=n;
        ///        port.Left=75;
        ///        port.Top=10;
        ///        port.PortShape = PortShapes.Arrow;
        ///        //Specifies the port style.
        ///        port.PortStyle.Fill = Brushes.Red;
        ///        port.PortStyle.Stroke = Brushes.Orange;
        ///        port.PortStyle.StrokeThickness = 2;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="PortStyle"/>
        public PortStyle PortStyle
        {
            get
            {
                if (this.customPortStyle != null)
                {
                    return this.customPortStyle;
                }
                else
                {
                    this.customPortStyle = new PortStyle();
                }

                return this.customPortStyle;
            }

            set
            {
                this.customPortStyle = value;
            }
        }

        internal double PxTop
        {
            get
            {
                if (Node != null)
                {
                    return MeasureUnitsConverter.ToPixels(Top, Node.MeasurementUnits);
                }
                else
                {
                    return Top;
                }
            }
            set
            {
                if (Node != null)
                {
                    Top = MeasureUnitsConverter.FromPixels(value, Node.MeasurementUnits);
                }
                else
                {
                    Top = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the top position of the port. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Port top position.
        /// </value>
        /// <example>
        /// C#:
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
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.View = View;
        ///        Control.Model = Model;
        ///        View.Bounds = new Thickness(0, 0, 1000, 1000);
        ///        //Creates a node
        ///        Node n = new Node(Guid.NewGuid(), "NewNode");
        ///        n.Shape = Shapes.Rectangle;
        ///        n.IsLabelEditable = true;
        ///        n.Label = "Start";
        ///        n.OffsetX = 150;
        ///        n.OffsetY = 25;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        n.ToolTip="New Node";
        ///        Model.Nodes.Add(n);
        ///        //Define a Custom port for the node.
        ///        ConnectionPort port = new ConnectionPort();
        ///        //Specifies the node
        ///        port.Node=n;
        ///        //Specifies the left position of the port.
        ///        port.Left=75;
        ///        //Specifies the top position of the port.
        ///        port.Top=10;
        ///        //Specifies the port shape
        ///        port.PortShape = PortShapes.Arrow;
        ///        n.Ports.Add(port);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        internal Point PagePosition
        {
            get
            {
                if (Node != null)
                {
                    return new Point(Node.PxOffsetX + CenterPosition.X, Node.PxOffsetY + CenterPosition.Y);
                }
                else
                {
                    return new Point(0, 0);
                }
            }
        }

        internal Dock Direction()
        {
            //Dock d = Dock.Top;
            System.Windows.Shapes.Polygon poly = new System.Windows.Shapes.Polygon();
            Point one, two, three, four, center, port;
            one = new Point(Node.PxOffsetX, Node.PxOffsetY);
            two = new Point(Node.PxOffsetX + Node.ActualWidth, Node.PxOffsetY);
            three = new Point(Node.PxOffsetX + Node.ActualWidth, Node.PxOffsetY + Node.ActualHeight);
            four = new Point(Node.PxOffsetX, Node.PxOffsetY + Node.ActualHeight);
            port = new Point(Node.PxOffsetX + this.PxLeft, Node.PxOffsetY + this.PxTop);
            center = new Point(Node.PxOffsetX + Node.ActualWidth / 2, Node.PxOffsetY + Node.ActualHeight / 2);
            ////Right
            //if (IsPointWithin(port, center, two, three))
            //{
            //    return Dock.Right;
            //}
            ////Bottom
            //else if (IsPointWithin(port, center, three, four))
            //{
            //    return Dock.Bottom;
            //}
            ////Left
            //else if (IsPointWithin(port, center, four, one))
            //{
            //    return Dock.Left;
            //}
            ////Top
            //else if (IsPointWithin(port, center, one, two))
            //{
            //    return Dock.Top;
            //}
            //else
            //{
            //    //Somthing wrong.
            //    return Dock.Right;
            //}
            double angle = LineConnector.findAngle(center, port);
            if (45 > angle || angle > 315)
            {
                return Dock.Right;
            }
            else if (135 > angle && angle > 45)
            {
                return Dock.Bottom;
            }
            else if (225 > angle && angle > 135)
            {
                return Dock.Left;
            }
            else if (315 > angle && angle > 225)
            {
                return Dock.Top;
            }
            else
            {
                //Somthing wrong.
                return Dock.Right;
            }
        }

        internal Point PxCenterPosition
        {
            get
            {
                if (Node != null)
                {
                    return MeasureUnitsConverter.ToPixels(CenterPosition, Node.MeasurementUnits);
                }
                else
                {
                    return CenterPosition;
                }
            }
            set
            {
                if (Node != null)
                {
                    CenterPosition = MeasureUnitsConverter.FromPixels(value, Node.MeasurementUnits);
                }
                else
                {
                    CenterPosition = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the center position.
        /// </summary>
        /// <value>The center position.</value>
        public Point CenterPosition
        {
            get
            {
                return (Point)GetValue(CenterPositionProperty);
            }

            set
            {
                SetValue(CenterPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current port point.
        /// </summary>
        /// <value>The current port point.</value>
        internal Point CurrentPortPoint
        {
            get
            {
                return this.endpoint;
            }

            set
            {
                if (this.endpoint != value)
                {
                    this.endpoint = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drag over port.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drag over port; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDragOverPort
        {
            get { return (bool)GetValue(IsDragOverPortProperty); }
            set { SetValue(IsDragOverPortProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drag over port.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drag over port; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }




        /// <summary>
        /// Gets or sets the port pointer.
        /// </summary>
        /// <value>The port pointer.</value>
        internal Border PortPointer
        {
            get
            {
                return this.customportPointer;
            }

            set
            {
                if (this.customportPointer != value)
                {
                    this.customportPointer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the previous port point.
        /// </summary>
        /// <value>The previous port point.</value>
        internal Point PreviousPortPoint
        {
            get
            {
                return this.mpreviousOriginPoint;
            }

            set
            {
                if (this.mpreviousOriginPoint != value)
                {
                    this.mpreviousOriginPoint = value;
                }
            }
        }
        private Path connectionPath;

        /// <summary>
        /// Invoked whenever application code or internal processes call this method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.connectionPath = this.GetTemplateChild("PART_PortPath") as Path;
            if (this.PortShape == PortShapes.Arrow)
            {
                this.connectionPath.Style = (this.connectionPath.Parent as Grid).Resources["Arrow"] as Style;
            }
            else if (this.PortShape == PortShapes.Circle)
            {
                this.connectionPath.Style = (this.connectionPath.Parent as Grid).Resources["Circle"] as Style;
            }
            else
            {
                this.connectionPath.Style = (this.connectionPath.Parent as Grid).Resources["Diamond"] as Style;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                Node.Ports.Remove(this);
            }
            else
            {
                // base.OnMouseLeftButtonDown(e);
                IsMouseDown = true;
                this.Focus();
            }
            e.Handled = true;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsMouseOver = true;
            if (Node.dview.EnableConnection && Node.dview.PortMode==ConnectionMode.Connect)
                IsDragOverPort = true;

        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
            IsDragOverPort = false;
        }

        private static void OnIsDragPortOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConnectionPort p = d as ConnectionPort;
            if (p.IsDragOverPort)
                foreach (ConnectionPort port in p.Node.Ports)
                {
                    if (p != port)
                    {
                        port.IsDragOverPort = false;
                    }
                }

        }

        /// <summary>
        /// Called when [center position changed].
        /// </summary>
        /// <param name="d">The dependency Object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCenterPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConnectionPort port = d as ConnectionPort;

            if (port.Node != null)
            {
                if (port.Node.Edges != null)
                {
                    foreach(IEdge connector in port.Node.Edges)
                    {
                        (connector as LineConnector).UpdateConnectorPathGeometry();
                    }
                }
                port.Node.InvalidateMeasure();
                port.Node.InvalidateArrange();
            }
        }

        /// <summary>
        /// Called when [offset X changed].
        /// </summary>
        /// <param name="d">The dependency Object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConnectionPort port = d as ConnectionPort;
            port.UpdateLeft();
        }

        private void UpdateLeft()
        {
            //PxCenterPosition = new Point(Math.Round(PxLeft - ActualWidth / 2, 2), Math.Round(PxTop - ActualHeight / 2, 2));
            PxCenterPosition = new Point(PxLeft, PxTop);
            if (RenderTransform is TranslateTransform)
            {
                (RenderTransform as TranslateTransform).X = Math.Round(PxLeft - ActualWidth / 2, 2);
            }
        }

        /// <summary>
        /// Called when [offset Y changed].
        /// </summary>
        /// <param name="d">The dependency Object..</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConnectionPort port = d as ConnectionPort;
            port.UpdateTop();
        }

        private void UpdateTop()
        {
            //PxCenterPosition = new Point(Math.Round(PxLeft - ActualWidth / 2, 2), Math.Round(PxTop - ActualHeight / 2, 2));
            PxCenterPosition = new Point(PxLeft, PxTop);
            if (RenderTransform is TranslateTransform)
            {
                (RenderTransform as TranslateTransform).Y = Math.Round(PxTop - ActualHeight / 2, 2);
            }
        }

        /// <summary>
        /// Is invoked when the port's layout is updated..
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ConnectionPort_LayoutUpdated(object sender, EventArgs e)
        {
            this.CenterPosition = new Point(this.Left, this.Top);
        }

        /// <summary>
        /// Is invoked when the port is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ConnectionPort_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // this.PortReferenceNo = Node.Ports.Count-1;
                foreach (ConnectionPort cport in Node.Ports)
                {
                    double x = cport.PxLeft-cport.ActualWidth/2;
                    double y = cport.PxTop-cport.ActualHeight/2;
                    TranslateTransform tr = new TranslateTransform();
                    tr.X = x;
                    tr.Y = y;
                    cport.RenderTransform=tr;
                }
            }
            catch
            {
            }
        }

        #region Class Fields

        #endregion

        #region Initialization

        #endregion

        #region Properties

        #endregion

        #region DPs

        #endregion

        #region Events

        #endregion

        #region Overrides

        #endregion
    }
}
