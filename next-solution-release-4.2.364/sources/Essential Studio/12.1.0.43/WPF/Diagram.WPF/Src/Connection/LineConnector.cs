// <copyright file="LineConnector.cs" company="Syncfusion">
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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Shared;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Syncfusion.Windows.Diagram
{
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
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class LineConnector : ConnectorBase, INodeGroup
    {
        #region Class Fields

        internal Point m_TempStartPoint;
        internal Point m_TempEndPoint;
        internal List<Point> m_TempInerPts;
        private List<string> header = new List<string>();
        internal ContextMenu linecontextmenu;

        /// <summary>
        /// Used to flag bridging
        /// </summary>
        internal bool bridged = false;

        /// <summary>
        /// Used to invalidate line bridging
        /// </summary>
        internal bool invalidateBridging = true;

        /// <summary>
        /// Used to store the smallest x value
        /// </summary>
        internal double minx;

        /// <summary>
        /// Used to store the largest x value
        /// </summary>
        internal double maxx;
        
        /// <summary>
        /// Used to store the smallest y value
        /// </summary>
        internal double miny;

        /// <summary>
        /// Used to store the largest y value
        /// </summary>
        internal double maxy;

        /// <summary>
        /// sets ts Widened Path Geomentry
        /// </summary>
        private Geometry WidenedPathGeometry;

        /// <summary>
        /// Used to store the bend length.
        /// </summary>
        private double mBendLength = 10d;



        /// <summary>
        /// Used to store the editor instance
        /// </summary>
        internal LabelEditor editor;
        internal ContentPresenter lablecontentpresenter;

        /// <summary>
        /// Used to store the view instance.
        /// </summary>
        internal DiagramView dview;

        /// <summary>
        /// Used to store the DiagramControl instance
        /// </summary>
        internal DiagramControl dc;
                
        /// <summary>
        /// Used to check if mouse is double clicked.
        /// </summary>
        private bool isdoubleclicked = false;

        /// <summary>
        /// Used to store the head decorator shape for internal use.
        /// </summary>
        private DecoratorShape headshape = DecoratorShape.None;

        /// <summary>
        /// Used to store the tail decorator shape for internal use.
        /// </summary>
        private DecoratorShape tailshape = DecoratorShape.None;

        /// <summary>
        /// Used to check if nodes are overlapped.
        /// </summary>
        private bool m_isoverlapped = false;
        
        internal bool isClicked;
        internal List<Point> ConnectionPoint = new List<Point>();

        private double _MinSegmentLength;

        public double MinimumSegementLength
        {
            get { return _MinSegmentLength; }
            set { _MinSegmentLength = value; }
        }
        
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="LineConnector"/> class.
        /// </summary>
        static LineConnector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineConnector), new FrameworkPropertyMetadata(typeof(LineConnector)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        public LineConnector()
            : base()
        {
            this.ID = Guid.NewGuid();
            this.Loaded += new RoutedEventHandler(LineConnector_Loaded);
            this.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Lineconnector_MouseLeftButtonUp), true);
            this.Unloaded += new RoutedEventHandler(Connection_Unloaded);

            if (this.IntermediatePoints == null)
            {
                IntermediatePoints = new List<Point>();
                IntermediatePoints.Add(new Point(0, 0));
                IntermediatePoints.Add(new Point(0, 0));
            }
            // CreateContextMenu();
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
            dview = view;
            this.ID = Guid.NewGuid();
            if (this.IntermediatePoints == null)
            {
                IntermediatePoints = new List<Point>();
                IntermediatePoints.Add(new Point(0, 0));
                IntermediatePoints.Add(new Point(0, 0));
            }
            this.ConnectorType = (dview.Page as DiagramPage).ConnectorType;
            this.HeadNode = source;
            this.TailNode = sink;
            this.Loaded += new RoutedEventHandler(LineConnector_Loaded);
            this.Unloaded += new RoutedEventHandler(Connection_Unloaded);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        /// <param name="view">The view instance.</param>
        public LineConnector(DiagramView view)
            : base()
        {
            if (view != null)
            {
                dview = view;
                this.Loaded += new RoutedEventHandler(LineConnector_Loaded);
                this.ID = Guid.NewGuid();
                this.ConnectorType = (dview.Page as DiagramPage).ConnectorType;
                this.Unloaded += new RoutedEventHandler(Connection_Unloaded);
            }
            else
            {
                this.Loaded += new RoutedEventHandler(DelayedConnectorType);
                this.ID = Guid.NewGuid();
                this.Loaded += new RoutedEventHandler(LineConnector_Loaded);
                this.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Lineconnector_MouseLeftButtonUp), true);
                this.Unloaded += new RoutedEventHandler(Connection_Unloaded);
            }
            if (this.IntermediatePoints == null)
            {
                IntermediatePoints = new List<Point>();
                IntermediatePoints.Add(new Point(0, 0));
                IntermediatePoints.Add(new Point(0, 0));
            }

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            dc = DiagramPage.GetDiagramControl(this);
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Unloaded event of the Connection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Connection_Unloaded(object sender, RoutedEventArgs e)
        {
            //// remove adorner
            if (this.LineAdorner != null)
            {
                IDiagramPage diagramPanel = VisualTreeHelper.GetParent(this) as IDiagramPage;

                AdornerLayer adorner = AdornerLayer.GetAdornerLayer(this);
                if (adorner != null)
                {
                    adorner.Remove(this.LineAdorner);
                    this.LineAdorner = null;
                }
            }
        }

        private void DelayedConnectorType(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DelayedConnectorType;
            if (dc != null && dc.View != null && dc.View.Page != null)
            {
                this.ConnectorType = (dc.View.Page as DiagramPage).ConnectorType; ;
            }
        }


        /// <summary>
        /// Handles the Loaded event of the LineConnector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void LineConnector_Loaded(object sender, RoutedEventArgs e)
        {
            //Node.SetUnitBinding("PxStartPointPosition", "MeasurementUnit", ConnectorBase.StartPointPositionProperty, this);
            //Node.SetUnitBinding("PxEndPointPosition", "MeasurementUnit", ConnectorBase.EndPointPositionProperty, this);
            //Node.SetUnitBinding("PxConnectionEndSpace", "MeasurementUnit", LineConnector.ConnectionEndSpaceProperty, this);

            dview = GetDiagramView(this);
            this.view = dview;
            if (this.IsSelected)
            {
                ShowAdorner();
                dview.SelectionList.Add(this);
                UndoRedoOperation();
                
            }

            if (this.LabelTemplate != null)
            {
                this.UpdateDecoratorPosition();
            }

            if (HeadNode != null)
            {
                this.HeadNodeReferenceNo = (this.HeadNode as Node).ReferenceNo;
            }
            if (TailNode != null)
            {
                this.TailNodeReferenceNo = (this.TailNode as Node).ReferenceNo;
            }
            if (ConnectionTailPort != null)
            {
                this.TailPortReferenceNo = this.ConnectionTailPort.PortReferenceNo;
            }
            if (ConnectionHeadPort != null)
            {
                this.HeadPortReferenceNo = this.ConnectionHeadPort.PortReferenceNo;
            }

            try
            {
                dview = Node.GetDiagramView(this);
                if (!dview.IsPageEditable)
                {
                    DiagramView.PageEdit = false;
                    IsLabelEditable = false;
                }
                Path HeadDecoratorPath = (this.GetTemplateChild("PART_HeadDecoratorAnchorPath") as Path);
                Path TailDecoratorPath = (this.GetTemplateChild("PART_SinkAnchorPath") as Path);
                (HeadDecoratorPath.Parent as Grid).SizeChanged += new SizeChangedEventHandler(LineConnectorDecorator_SizeChanged);
                (TailDecoratorPath.Parent as Grid).SizeChanged += new SizeChangedEventHandler(LineConnectorDecorator_SizeChanged);
                HeadDecoratorPath.SizeChanged += new SizeChangedEventHandler(LineConnectorDecorator_SizeChanged);
                TailDecoratorPath.SizeChanged += new SizeChangedEventHandler(LineConnectorDecorator_SizeChanged);
            }
            catch
            {
            }
            //if (this.ContextMenu == null)
            //{
            //    CreateContextMenu();
            //}


            if (this.m_LineDrawing)
            {
                this.m_LineDrawing = false;
                dc.View.Page.InvalidateMeasure();
            }
        }

        //For UndoRedoOperation
        internal void UndoRedoOperation()
        {
            if (this.LineAdorner!= null)
            {
                (this.LineAdorner as LineConnectorAdorner).HeadThumbDoStackOperation();
                (this.LineAdorner as LineConnectorAdorner).TailThumbDoStackOperation();
            } 
        }
        void LineConnectorDecorator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TranslateTransform tth = new TranslateTransform(-(this.GetTemplateChild("PART_HeadDecoratorAnchorPath") as Path).ActualWidth + 2, -(this.GetTemplateChild("PART_HeadDecoratorAnchorPath") as Path).ActualHeight / 2);
            RotateTransform headAngle = new RotateTransform(this.HeadDecoratorAngle, 0, 0);
            TransformGroup tthg = new TransformGroup();
            tthg.Children.Add(tth);
            tthg.Children.Add(headAngle);
            (this.GetTemplateChild("PART_HeadDecoratorAnchorPath") as Path).RenderTransform = tthg;

            TranslateTransform ttt = new TranslateTransform(-(this.GetTemplateChild("PART_SinkAnchorPath") as Path).ActualWidth + 2, -(this.GetTemplateChild("PART_SinkAnchorPath") as Path).ActualHeight / 2);
            RotateTransform tailAngle = new RotateTransform(this.TailDecoratorAngle, 0, 0);
            TransformGroup tttg = new TransformGroup();
            tttg.Children.Add(ttt);
            tttg.Children.Add(tailAngle);
            (this.GetTemplateChild("PART_SinkAnchorPath") as Path).RenderTransform = tttg;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the internal LineAngle. Used for internal assignments in case of LabelOrienation.
        /// </summary>
        /// <value>The internal tail decorator shape.</value>
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
        /// Used to store the LineAngle
        /// </summary>
        internal static readonly DependencyProperty LineAngleProperty = DependencyProperty.Register("LineAngle", typeof(double), typeof(LineConnector), new PropertyMetadata(0d));
        /// <summary>
        /// Gets or sets the internal tail decorator shape. Used for internal assignments in case of overlapped nodes.
        /// </summary>
        /// <value>The internal tail decorator shape.</value>
        internal DecoratorShape InternalTailShape
        {
            get { return tailshape; }
            set { tailshape = value; }
        }

        /// <summary>
        /// Gets or sets the internal head decorator shape. Used for internal assignments in case of overlapped nodes.
        /// </summary>
        /// <value>The internal head decorator shape.</value>
        internal DecoratorShape InternalHeadShape
        {
            get { return headshape; }
            set { headshape = value; }
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
                return m_isoverlapped;
            }

            set
            {
                m_isoverlapped = value;
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
                return mBendLength;
            }

            set
            {
                mBendLength = value;
            }
        }

        public bool AllowDelete
        {
            get { return (bool)GetValue(AllowDeleteProperty); }
            set { SetValue(AllowDeleteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowDelete.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowDeleteProperty =
            DependencyProperty.Register("AllowDelete", typeof(bool), typeof(LineConnector), new UIPropertyMetadata(true));

        public bool AllowSelect
        {
            get { return (bool)GetValue(AllowSelectProperty); }
            set { SetValue(AllowSelectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowSelect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowSelectProperty =
            DependencyProperty.Register("AllowSelect", typeof(bool), typeof(LineConnector), new UIPropertyMetadata(true));


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
            get { return (double)GetValue(ConnectionEndSpaceProperty); }
            set { SetValue(ConnectionEndSpaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectionEndSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionEndSpaceProperty =
            DependencyProperty.Register("ConnectionEndSpace", typeof(double), typeof(LineConnector), new UIPropertyMetadata(6d));
        
        #endregion

        #region Class Override


        internal void ConnectorSelection()
        {
            LineConnector n = this as LineConnector;
            if (n.dc != null)
            {
                IDiagramPage m_diagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;
                if (n.IsSelected && dview != null && (n.dc.View.Page as DiagramPage).SelectionList != null)
                {
                    (dview as DiagramView).SelectionList.Add(this);

                    ShowAdorner();
                }
                else
                {
                    (dview as DiagramView).SelectionList.Remove(this);
                    HideAdorner();
                }
            }


            if (n.IsSelected)
            {
                ConnectorRoutedEventArgs newEventArgs = new ConnectorRoutedEventArgs(n as LineConnector);
                newEventArgs.RoutedEvent = DiagramView.ConnectorSelectedEvent;
                n.RaiseEvent(newEventArgs);
                if (this.IsLoaded)
                    ShowAdorner();
            }
            else
            {
                ConnectorRoutedEventArgs newEventArgs = new ConnectorRoutedEventArgs(n as LineConnector);
                newEventArgs.RoutedEvent = DiagramView.ConnectorUnSelectedEvent;
                HideAdorner();
                n.RaiseEvent(newEventArgs);
            }

        }
        /// <summary>
        /// Invoked whenever application code or internal processes call
        /// <see cref="System.Windows.FrameworkElement.ApplyTemplate"/> method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            dc = DiagramPage.GetDiagramControl((FrameworkElement)this);
            dview = GetDiagramView(this);           
            editor = GetTemplateChild("PART_ConnectorLabelEditor") as LabelEditor;
            lablecontentpresenter = GetTemplateChild("PART_ConnectorLabelTemplateEditor") as ContentPresenter;
            RotateTransform transform = new RotateTransform();
            transform.Angle = this.LineAngle + this.LabelAngle;
            (this.editor.Parent as Grid).RenderTransform = transform;
            this.editor.RenderTransform = transform;

            editor.Loaded += new RoutedEventHandler(editor_Loaded);
            editor.AfterLabelEdit += new LabelEditor.AfterLabelEditHandler(editor_AfterLabelEdit);
            if (dview != null)
            {
                if (!dview.SelectionList.Contains(this) && this.IsSelected)
                {
                    ConnectorSelection();
                }
            }
            if (dc != null)
            {

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
                CreateContextMenu();
            }
        }

        internal static DiagramView GetDiagramView(DependencyObject element)
        {
            while (element != null && !(element is DiagramView))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            return element as DiagramView;
        }

        void editor_BeforeLabelEdit(object sender, BeforeLabelEditEventArgs e)
        {
        }

        private ContentPresenter presenter;
        void editor_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Visual visual in VisualUtils.EnumChildrenOfType(this.editor, typeof(ContentPresenter)))
            {
                if ((visual as ContentPresenter).Name == "Content")
                {
                    presenter = visual as ContentPresenter;
                    if (editor.TemplatedParent != null && editor.TemplatedParent is LineConnector)
                    {
                        presenter.HorizontalAlignment = HorizontalAlignment.Left;
                    }
                }
            }

            this.InvalidateConnectorPathGeometry();
            this.UpdateDecoratorPosition();

        }

        void editor_AfterLabelEdit(object sender, AfterLabelEditEventArgs e)
        {
            //this.UpdateConnectorPathGeometry();
            this.UpdateDecoratorPosition();
        }

        void editor_KeyDown(object sender, KeyEventArgs e)
        {
        }

        void editor_LostFocus(object sender, RoutedEventArgs e)
        {
        }


        /// <summary>
        /// Invoked when Label editing is started.
        /// </summary>
        public void Labeledit()
        {
            if (IsLabelEditable)
            {
                editor.LabelEditStartInternal(editor);
            }
        }

        /// <summary>
        /// Invoked when Label editing is complete
        /// </summary>
        internal void CompleteConnEditing()
        {
            editor.CompleteHeaderEditInternal(editor, true);
        }



        #region ContextMenuforLineConnector

        internal string ContextMenu_Delete;
        internal string ContextMenu_Grouping;
        internal string ContextMenu_Grouping_Group;
        internal string ContextMenu_Grouping_Ungroup;
        internal string ContextMenu_Order;
        internal string ContextMenu_Order_BringForward;
        internal string ContextMenu_Order_BringToFront;
        internal string ContextMenu_Order_SendBackward;
        internal string ContextMenu_Order_SendToBack;
        internal MenuItem Order;
        internal MenuItem Front;
        internal MenuItem Forward;
        internal MenuItem Backward;
        internal MenuItem Back;
        internal MenuItem Grouping;
        internal MenuItem Group;
        internal MenuItem Ungroup;
        internal MenuItem Delete;

        //Craeting the contextMenu
        internal void CreateContextMenu()
        {
            Order = new MenuItem();
            Order.Header = ContextMenu_Order;
            header.Add(ContextMenu_Order);

            Front = new MenuItem();
            Front.Header = ContextMenu_Order_BringToFront;
            Front.Click += new RoutedEventHandler(M1_Click);
            Order.Items.Add(Front);
            Forward = new MenuItem();
            Forward.Header = ContextMenu_Order_BringForward;
            Forward.Click += new RoutedEventHandler(M2_Click);
            Order.Items.Add(Forward);
            Backward = new MenuItem();
            Backward.Header = ContextMenu_Order_SendBackward;
            Backward.Click += new RoutedEventHandler(M3_Click);
            Order.Items.Add(Backward);
            Back = new MenuItem();
            Back.Header = ContextMenu_Order_SendToBack;
            Back.Click += new RoutedEventHandler(M4_Click);
            Order.Items.Add(Back);

            Grouping = new MenuItem();
            Grouping.Header = ContextMenu_Grouping;
            Group = new MenuItem();
            Group.Header = ContextMenu_Grouping_Group;
            Group.Click += new RoutedEventHandler(G1_Click);
            Grouping.Items.Add(Group);
            Ungroup = new MenuItem();
            Ungroup.Header = ContextMenu_Grouping_Ungroup;
            Ungroup.Click += new RoutedEventHandler(G2_Click);
            Grouping.Items.Add(Ungroup);
            header.Add(ContextMenu_Grouping);
            Delete = new MenuItem();
            Delete.Header = ContextMenu_Delete;
            Delete.Click += new RoutedEventHandler(Del_Click);
            header.Add(ContextMenu_Delete);

            linecontextmenu = new System.Windows.Controls.ContextMenu();
            linecontextmenu.Items.Add(Order);
            linecontextmenu.Items.Add(Grouping);
            linecontextmenu.Items.Add(Delete);
            //this.ContextMenu = linecontextmenu;

        }
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (dview != null)
            {
                if (dview.IsPageEditable && dview.LineConnectorContextMenu == null && this.linecontextmenu != null)
                {
                    //if (linecontextmenu == null)
                    //{
                    //    linecontextmenu = new System.Windows.Controls.ContextMenu();
                    //    linecontextmenu.Items.Add(Order);
                    //    linecontextmenu.Items.Add(Grouping);
                    //    linecontextmenu.Items.Add(Delete);

                    //}
                    if (this.ContextMenu == null)
                    {
                        this.ContextMenu = this.linecontextmenu;
                    }
                    setDisableOrEnable(Front, Forward, Backward, Back, Delete, Group);

                }
                else if (dview.LineConnectorContextMenu != null)
                {
                    this.ContextMenu = dview.LineConnectorContextMenu;
                }
                else
                {
                    //Refresh reference  context menu.
                    if (this.ContextMenu != null)
                    {

                        if (!isCustomContextMenu())
                        {
                            ContextMenu linecontectmenu = this.ContextMenu;
                            MenuItem Order = linecontectmenu.Items[0] as MenuItem;
                            MenuItem Grouping = linecontectmenu.Items[1] as MenuItem;
                            MenuItem Delete = linecontectmenu.Items[2] as MenuItem;
                            Delete.Click -= new RoutedEventHandler(Del_Click);
                            Delete.Click += new RoutedEventHandler(Del_Click);

                            MenuItem Front = Order.Items[0] as MenuItem;
                            Front.Click -= new RoutedEventHandler(M1_Click);
                            Front.Click += new RoutedEventHandler(M1_Click);
                            MenuItem Forward = Order.Items[1] as MenuItem;
                            Forward.Click -= new RoutedEventHandler(M2_Click);
                            Forward.Click += new RoutedEventHandler(M2_Click);
                            MenuItem Backward = Order.Items[2] as MenuItem;
                            Backward.Click -= new RoutedEventHandler(M3_Click);
                            Backward.Click += new RoutedEventHandler(M3_Click);
                            MenuItem Back = Order.Items[3] as MenuItem;
                            Back.Click -= new RoutedEventHandler(M4_Click);
                            Back.Click += new RoutedEventHandler(M4_Click);
                            MenuItem Group = Grouping.Items[0] as MenuItem;
                            Group.Click -= new RoutedEventHandler(G1_Click);
                            Group.Click += new RoutedEventHandler(G1_Click);
                            MenuItem Ungroup = Grouping.Items[1] as MenuItem;
                            Ungroup.Click -= new RoutedEventHandler(G2_Click);
                            Ungroup.Click += new RoutedEventHandler(G2_Click);

                            setDisableOrEnable(Front, Forward, Backward, Back, Delete, Group);
                        }

                    }
                }

            }

            base.OnMouseRightButtonUp(e);
        }

        //Checking for the  customContextmenu
        internal bool isCustomContextMenu()
        {
            if (this.ContextMenu != null)
            {
                Populateheader();
                ContextMenu linecontextmenu = this.ContextMenu;
                List<string> oldheader = new List<string>();
                for (int i = 0; i < linecontextmenu.Items.Count; i++)
                {
                    if ((linecontextmenu.Items[i] is MenuItem) && (linecontextmenu.Items[i] as MenuItem).Header != null)
                    {
                        oldheader.Add((linecontextmenu.Items[i] as MenuItem).Header.ToString());
                    }
                }


                if (header != null && header.Count == oldheader.Count)
                {

                    if (oldheader.OfType<string>().Equals(header))
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
                    return true;
                }
            }
            return false;
        }

        private void Populateheader()
        {

            if (header.Count == 0)
            {
                header.Add(ContextMenu_Order);
                header.Add(ContextMenu_Grouping);
                header.Add(ContextMenu_Delete);
            }
        }
        //setDisableOrEnable the properties
        internal void setDisableOrEnable(MenuItem Front, MenuItem Forward, MenuItem Backward, MenuItem Back, MenuItem del, MenuItem group)
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
        /// Handles the Click event of the delete menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Del_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.Delete.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the bring to front menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M1_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.BringToFront.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the bring forward menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M2_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.MoveForward.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the send backward menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M3_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.SendBackward.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the send to back menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M4_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.SendToBack.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the group menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void G1_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.Group.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the ungroup menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void G2_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.Ungroup.Execute(dview.Page, dview);
        }
        #endregion

        /// <summary>
        ///  Provides class handling for the MouseDoubleClick routed event that occurs when 
        ///  the mouse left button is clicked twice in succession.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (dview.IsPageEditable)
            {
                isdoubleclicked = true;

                ConnRoutedEventArgs newEventArgs;
                if (HeadNode != null && TailNode != null)
                {
                    newEventArgs = new ConnRoutedEventArgs(this.HeadNode as Node, this.TailNode as Node, this);
                }
                else if (HeadNode == null && TailNode != null)
                {
                    newEventArgs = new ConnRoutedEventArgs(this.TailNode as Node, this);
                }
                else if (HeadNode != null && TailNode == null)
                {
                    newEventArgs = new ConnRoutedEventArgs(this, this.HeadNode as Node);
                }
                else
                {
                    newEventArgs = new ConnRoutedEventArgs(this);
                }

                newEventArgs.RoutedEvent = DiagramView.ConnectorDoubleClickEvent;
                RaiseEvent(newEventArgs);
                if (IsLabelEditable && this.AllowSelect)
                {
                    editor.LabelEditStartInternal(editor);
                }
            }
        }
        internal bool positionchange = false;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == (ModifierKeys.Shift | ModifierKeys.Control)
                        && ((this.ConnectorType == ConnectorType.Orthogonal) || (this.ConnectorType == ConnectorType.Straight))
                        )
            {
                if (BrowserInteropHelper.IsBrowserHosted)
                {
                    this.Cursor = System.Windows.Input.Cursors.Pen;
                }
                else
                {
                    Assembly ass = Assembly.GetExecutingAssembly();
                    System.IO.Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.InsertVertex.cur");                    
                    this.Cursor = new Cursor(stream);
                }
            }
            else
                this.Cursor = Cursors.Arrow;


        }
        /// <summary>
        /// Provides class handling for the MouseLeftButtonUp routed event that occurs when
        /// the mouse left button is released over this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
        private void Lineconnector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mousedown = false;
            if (!this.isdoubleclicked && this.isClicked)
            {
                ConnectorRoutedEventArgs NewEventArgs = new ConnectorRoutedEventArgs(this);
                NewEventArgs.RoutedEvent = DiagramView.ConnectorClickEvent;
                RaiseEvent(NewEventArgs);
            }

            bool notselected = true;
            if (dc.View.IsPageEditable && this.IsGrouped)
            {
                if (!dc.View.IsPanEnabled && !isdoubleclicked)
                {
                    IDiagramPage m_diagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;

                    if (m_diagramPage != null)
                    {
                        if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                        {
                            if (this.IsSelected)
                            {
                                m_diagramPage.SelectionList.Remove(this);
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

                                    if (notselected && this.AllowSelect)
                                    {
                                        if (!this.IsGrouped)
                                        {
                                            m_diagramPage.SelectionList.Add(this);
                                        }
                                        else
                                        {
                                            m_diagramPage.SelectionList.Add(this.Groups[this.Groups.Count - 1]);
                                        }
                                    }
                                    else if (this.IsGrouped && this.AllowSelect)
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
                                                        m_diagramPage.SelectionList.Add(groupednodes[groupednodes.Count - 1]);
                                                    }
                                                    else
                                                    {
                                                        m_diagramPage.SelectionList.Add(groupednodes[index - 1]);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (this.IsGrouped && this.AllowSelect)
                                {
                                    m_diagramPage.SelectionList.Add(this.Groups[this.Groups.Count - 1]);
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

                                if (notselected && this.AllowSelect)
                                {
                                    if (!this.IsGrouped)
                                    {
                                        m_diagramPage.SelectionList.Select(this);
                                    }
                                    else
                                    {
                                        if ((this.Groups[this.Groups.Count - 1] as Group).AllowSelect)
                                        {
                                            m_diagramPage.SelectionList.Select(this.Groups[this.Groups.Count - 1]);
                                        }
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
                                                    m_diagramPage.SelectionList.Select(groupednodes[groupednodes.Count - 1]);
                                                }
                                                else
                                                {
                                                    m_diagramPage.SelectionList.Select(groupednodes[index - 1]);
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (this.IsGrouped && this.AllowSelect)
                            {
                                m_diagramPage.SelectionList.Select(this.Groups[this.Groups.Count - 1]);
                            }
                        }

                        (m_diagramPage as DiagramPage).Focus();
                    }
                }

            }



            isdoubleclicked = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            
        }

        internal bool mousedown;
        /// <summary>
        /// Provides class handling for the MouseDown routed event that occurs when the mouse 
        /// button is pressed while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Retrieve the coordinate of the mouse position.
            Point pt = e.GetPosition((UIElement)this.dc.View.Page);
            
            Visibility adornerVisibility;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mousedown = true;
            }
            if (LineAdorner != null)
            {
                adornerVisibility = LineAdorner.Visibility;
            }
            else
            {
                adornerVisibility = Visibility.Collapsed;
            }

            if (dc.View.IsPageEditable && !this.IsGrouped)
            {

                IDiagramPage diagramPanel = VisualTreeHelper.GetParent(this) as IDiagramPage;

                if (diagramPanel != null)
                {

                    (diagramPanel as DiagramPage).Focus();
                    if (dc.View.IsPageEditable && !this.isdoubleclicked && adornerVisibility.Equals(Visibility.Collapsed))
                    {
                        ConnectorRoutedEventArgs NewEventArgs = new ConnectorRoutedEventArgs(this);
                        NewEventArgs.RoutedEvent = DiagramView.ConnectorClickEvent;
                        RaiseEvent(NewEventArgs);
                        this.isClicked = false;
                    }
                    else if (dc.View.IsPageEditable && !this.isdoubleclicked && !adornerVisibility.Equals(Visibility.Collapsed))
                    {
                        this.isClicked = true;
                    }

                    if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == (ModifierKeys.Shift | ModifierKeys.Control)
                        && ((this.ConnectorType == ConnectorType.Orthogonal) || (this.ConnectorType == ConnectorType.Straight))
                        )
                    {
                        Point t = e.GetPosition(this);
                        InsertIntermediatePoint_internal(t);
                        InvalidateConnectorPathGeometry();
                        if (LineAdorner == null)
                        {
                            ShowAdorner();
                            HideAdorner();
                        }
                        diagramPanel.SelectionList.Clear();
                        diagramPanel.SelectionList.Add(this);
                        (LineAdorner as LineConnectorAdorner).InvalidateVertexs();
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
                    else if (!this.IsSelected && this.AllowSelect)
                    {
                        diagramPanel.SelectionList.Clear();
                        diagramPanel.SelectionList.Add(this);
                    }
                }
            }

            e.Handled = true;
        }

        public void InsertIntermediatePoint(Point p)
       {
           InsertIntermediatePoint_internal(p);
                if (LineAdorner != null)
                {
                    (LineAdorner as LineConnectorAdorner).InvalidateVertexs();
                }
        }

       public void RemoveIntermediatePoints(Point p)
       {
            List<Point> Interpts = IntermediatePoints;
            if (ConnectorType == ConnectorType.Straight)
            {
                foreach (Point pt in Interpts)
                {
                    if (pt.X + 5 > p.X && pt.X - 5 < p.X && pt.Y + 5 > p.Y && pt.Y - 5 < p.Y)
                    {
                        IntermediatePoints.Remove(pt);
                        break;
                    }
                }
            }
            else
            {
                if (IntermediatePoints.Count >= 4)
                {

                    foreach (Point pt in Interpts)
                    {
                        if (pt.X + 5 > p.X && pt.X - 5 < p.X && pt.Y + 5 > p.Y && pt.Y - 5 < p.Y)
                        {
                            int vertexIndex = Interpts.IndexOf(pt);
                            if (Interpts[vertexIndex].X == Interpts[vertexIndex + 1].X)
                            {
                                if (vertexIndex != 0)
                                    IntermediatePoints[vertexIndex - 1] = new Point(IntermediatePoints[vertexIndex - 1].X, IntermediatePoints[vertexIndex + 1].Y);
                                IntermediatePoints.Remove(IntermediatePoints[vertexIndex]);
                                IntermediatePoints.Remove(IntermediatePoints[vertexIndex]);
                            }
                            else if (Interpts[vertexIndex].Y == Interpts[vertexIndex + 1].Y)
                            {
                                IntermediatePoints[vertexIndex + 2] = new Point(IntermediatePoints[vertexIndex].X, IntermediatePoints[vertexIndex + 2].Y);
                                IntermediatePoints.Remove(IntermediatePoints[vertexIndex]);
                                IntermediatePoints.Remove(IntermediatePoints[vertexIndex]);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    IntermediatePoints.Clear();
                    IntermediatePoints.Add(new Point(0, 0));
                }
            }
           InvalidateConnectorPathGeometry();
       }

       internal void InsertIntermediatePoint_internal(Point point)
        {
            double three = 3.0;
            if (IntermediatePoints == null)
            {
                IntermediatePoints = new List<Point>();
            }
            List<Point> points = new List<Point>();
            if (HeadNode != null)
            {
                NodeInfo ni;
                Rect sourceRect = getNodeRect(HeadNode as Node);
                ni = getRectAsNodeInfo(sourceRect);
                if (dc != null && dc.View != null && dc.View.Page != null)
                {
                    if (this.ConnectionHeadPort != null)
                    {
                        ni.Position = new Point(this.ConnectionHeadPort.Left, this.ConnectionHeadPort.Top);
                        ni.Position = (HeadNode as Node).TranslatePoint(ni.Position, dc.View.Page);
                        ni.Position = ni.Position;
                    }
                }

                points.Add(ni.Position);
            }
            else
            {
                points.Add(this.PxStartPointPosition);
            }
            points.AddRange(IntermediatePoints);
            if (TailNode != null)
            {
                NodeInfo ni;
                Rect targetRect = getNodeRect(TailNode as Node);
                ni = getRectAsNodeInfo(targetRect);

                if (dc != null && dc.View != null && dc.View.Page != null)
                {
                    if (this.ConnectionTailPort != null)
                    {
                        ni.Position = new Point(this.ConnectionTailPort.Left, this.ConnectionTailPort.Top);
                        ni.Position = (TailNode as Node).TranslatePoint(ni.Position, dc.View.Page);
                        ni.Position = ni.Position;
                    }
                    points.Add(ni.Position);
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
            if (ConnectorType == ConnectorType.Orthogonal || ConnectorType == ConnectorType.Straight)
            {
                DoStackOperationForNewPoint(selectedIndex);
            }
        }

        private void DoStackOperationForNewPoint(int selectedIndex)
        {
            if (dc != null && dc.View != null && dc.View.UndoRedoEnabled)
            {
                if (!(dc.View.undo || dc.View.redo))
                {
                    dc.View.RedoStack.Clear();
                }
                dc.View.UndoStack.Push(selectedIndex);
                dc.View.UndoStack.Push(this);
                dc.View.UndoStack.Push(IntermediatePoints[selectedIndex]);
                dc.View.UndoStack.Push("Added");
            }
        }

        /// <summary>
        /// Shows the adorner
        /// </summary>
        protected override void ShowAdorner()
        {
            if (VertexStyle == null)
                VertexStyle = this.FindResource("ConnectorAdornerVertexStyle") as Style;
            if (this.LineAdorner == null)
            {
                IDiagramPage mPage = VisualTreeHelper.GetParent(this) as IDiagramPage;

                AdornerLayer adorner = AdornerLayer.GetAdornerLayer(this);
                if (adorner != null)
                {
                    this.LineAdorner = new LineConnectorAdorner(mPage, this);
                    adorner.Add(this.LineAdorner);
                }
            }
            else
            {
                this.LineAdorner.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Hides the adorner.
        /// </summary>
        protected override void HideAdorner()
        {
            if (this.LineAdorner != null)
            {
                this.LineAdorner.Visibility = Visibility.Collapsed;
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
                InvalidateConnectorPathGeometry();
            }
        }

        internal bool isRouting = false;
        
        protected override Size MeasureOverride(Size constraint)
        {
            Update();
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                                      new Syncfusion.Windows.Diagram.DiagramModel.Initialize(SetLineBridging));
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Called whenever the head node, tail node or position of the node is changed. 
        /// </summary>
        public override void InvalidateConnectorPathGeometry()
        {
                if (this.EnableCumulativeUpdate)
                {
                    InvalidateMeasure();
                }
                else
                    UpdateConnectorPathGeometry();
        }

        /// <summary>
        /// Called whenever the head node, tail node or position of the node is changed. 
        /// </summary>
        public override void UpdateConnectorPathGeometry()
        {
            Update();
        }

        
        private void Update()
        {
            #region Init

            if (!this.IsLoaded || (this.dc != null && this.dc.IsLoadingFromFile))
            {
                return;
            }
            {

                if (dview == null)
                {
                    dview = Node.GetDiagramView(this);
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
                double twozero = 20;
                double sourceleft = PxStartPointPosition.X;
                double sourcetop = PxStartPointPosition.Y;
                double targetleft = PxEndPointPosition.X;
                double targettop = PxEndPointPosition.Y;
                double dropcentreX = DropPoint.X;
                double dropcentreY = DropPoint.Y;
                double twofive = 25;
                if (HeadNode != null)
                {
                    (HeadNode as Node).refreshBoundaries();
                    cs = PxConnectionEndSpace;
                    sourceRect = getNodeRect(HeadNode as Node);
                    source = getRectAsNodeInfo(sourceRect);
                    if (sourceRect != Rect.Empty)
                    {
                        sourceRect = new Rect(source.Left, source.Top, source.Size.Width, source.Size.Height);
                    }
                    if (sourceRect == Rect.Empty)
                    {
                        source = (HeadNode as Node).GetInfo();
                        sourceRect = new Rect(
                                            source.Left,
                                            source.Top,
                                            source.Size.Width,
                                            source.Size.Height);
                        sourceRect.Inflate(cs, cs);
                    }

                    startPoint = new Point(source.Position.X, source.Position.Y);
                    this.m_TempStart = startPoint;
                }
                else
                {
                    startPoint = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                    s = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                    sourceRect = new Rect(
                                        sourceleft,
                                        sourcetop,
                                        twozero,
                                        twozero);
                    source.Position = new Point(PxStartPointPosition.X + twofive, PxStartPointPosition.Y + twofive);
                }

                if (TailNode != null)
                {
                    cs = PxConnectionEndSpace;
                    (TailNode as Node).refreshBoundaries();
                    targetRect = getNodeRect(TailNode as Node);
                    target = getRectAsNodeInfo(targetRect);
                    if (targetRect != Rect.Empty)
                    {
                        targetRect = new Rect(target.Left, target.Top, target.Size.Width, target.Size.Height);
                    }
                    if (targetRect == Rect.Empty)
                    {
                        target = (TailNode as Node).GetInfo();
                        targetRect = new Rect(
                                              target.Left,
                                              target.Top,
                                              target.Size.Width,
                                              target.Size.Height);
                        targetRect.Inflate(cs, cs);
                    }
                    endPoint = new Point(target.Position.X, target.Position.Y);

                }
                else
                {
                    e = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                    endPoint = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                    targetRect = new Rect(
                                          targetleft,
                                          targettop,
                                          twozero,
                                          twozero);
                    target.Position = new Point(PxEndPointPosition.X - twofive, PxEndPointPosition.Y - twofive);
                }


                if (ConnectionHeadPort != null && ConnectionHeadPort.Edge != null)
                {
                    sp = new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top);
                    s = sp;
                    sourceRect = new Rect(
                                        s.X,
                                        s.Y,
                                        twozero,
                                        twozero);
                    source.Position = new Point(s.X + twofive, s.Y + twofive);
                }

                if (ConnectionTailPort != null && ConnectionTailPort.Edge != null)
                {
                    ep = new Point(ConnectionTailPort.Left, ConnectionTailPort.Top);
                    e = ep;
                    endPoint = e;
                    targetRect = new Rect(
                                          e.X,
                                          e.Y,
                                          twozero,
                                          twozero);
                    target.Position = new Point(e.X - twofive, e.Y - twofive);
                }

            #endregion

                if (this.ConnectorType == ConnectorType.Bezier || this.ConnectorType == ConnectorType.Arc)
                {
                    #region Bezier and Arc
                    try
                    {
                        if (HeadNode != null || TailNode != null)
                        {
                            if (this.ConnectorType == ConnectorType.Bezier)
                            {
                                sp = startPoint;
                                ep = endPoint;

                                if (!(Orientation == TreeOrientation.LeftRight || Orientation == TreeOrientation.RightLeft))
                                {
                                    ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, targetRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out s, out e);
                                    extendPoints(isTop, isBottom, isLeft, isRight, tisTop, tisBottom, tisLeft, tisRight, out s, out e, s, e);
                                }
                                else
                                {
                                    ConnectorBase.GetTreeOrthogonalLineIntersect(source, target, sourceRect, targetRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out s, out e);
                                    extendPoints(isTop, isBottom, isLeft, isRight, tisTop, tisBottom, tisLeft, tisRight, out s, out e, s, e);
                                }
                            }
                            else
                            {
                                ConnectorBase.GetArcLineIntersect(source, target, sourceRect, targetRect, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out s, out e, this.ArcDirection);
                                extendPoints(isTop, isBottom, isLeft, isRight, tisTop, tisBottom, tisLeft, tisRight, out s, out e, s, e);
                            }

                        }

                        if (HeadNode != null)
                        {
                            sp = startPoint;

                            if (ConnectionHeadPort != null)
                            {
                                s = new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top);
                                s = (HeadNode as Node).TranslatePoint(s, dview.Page);
                            }
                        }

                        if (TailNode != null)
                        {
                            ep = endPoint;
                            if (ConnectionTailPort != null)
                            {
                                e = new Point(ConnectionTailPort.Left, ConnectionTailPort.Top);
                                e = (TailNode as Node).TranslatePoint(e, dview.Page);
                            }
                        }

                        if (HeadNode == null)
                        {
                            s = new Point(PxStartPointPosition.X, PxStartPointPosition.Y);
                        }

                        if (TailNode == null)
                        {
                            e = new Point(PxEndPointPosition.X, PxEndPointPosition.Y);
                        }

                        if (ConnectionHeadPort != null && ConnectionHeadPort.Edge != null)
                        {
                            sp = new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top);
                            s = sp;
                        }

                        if (ConnectionTailPort != null && ConnectionTailPort.Edge != null)
                        {
                            ep = new Point(ConnectionTailPort.Left, ConnectionTailPort.Top);
                            e = ep;
                        }

                        if (this.LineAdorner != null)
                        {
                            if ((this.LineAdorner as LineConnectorAdorner).ConnectorPoint)
                            {
                                (this.LineAdorner as LineConnectorAdorner).ConnectorPoint = false;
                                if ((this.LineAdorner as LineConnectorAdorner).IsHeadThumb)
                                {
                                    s = Mouse.GetPosition(this);// MeasureUnitsConverter.FromPixels(Mouse.GetPosition(this), this.MeasurementUnit);
                                    HeadNode = null;
                                }
                                else if ((this.LineAdorner as LineConnectorAdorner).IsTailThumb)
                                {
                                    e = Mouse.GetPosition(this);// MeasureUnitsConverter.FromPixels(Mouse.GetPosition(this), this.MeasurementUnit);
                                    TailNode = null;
                                }
                            }
                        }
                        if (this.HeadNode != null)
                        {
                            if (this.ConnectionHeadPort != null && HeadNode != null)
                            {
                                if (dview != null && dview.Page != null)
                                {
                                    s = new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top);
                                    s = (HeadNode as Node).TranslatePoint(s, dview.Page);
                                }
                            }
                            else
                            {
                                s = GetIntersectionPoint(s, (HeadNode as Node));
                            }
                        }
                        if (this.TailNode != null)
                        {

                            if (this.ConnectionTailPort != null && TailNode != null)
                            {
                                if (dview != null && dview.Page != null)
                                {
                                    e = new Point(ConnectionTailPort.Left, ConnectionTailPort.Top);
                                    e = (TailNode as Node).TranslatePoint(e, dview.Page);
                                }
                            }
                            else
                            {
                                e = GetIntersectionPoint(e, (TailNode as Node));
                            }
                        }
                        if ((this.HeadNode as Node) != null && (this.TailNode as Node) != null)
                        {
                            if (!dc.View.Page.Children.Contains(this.HeadNode as Node) && dc.View.Page.Children.Contains(this.TailNode as Node))
                            {
                                s = new Point((this.HeadNode as Node).OffsetX, (this.HeadNode as Node).OffsetY); //GetIntersectionPoint(new Point((this.HeadNode as Node).OffsetX, (this.HeadNode as Node).OffsetY), (this.TailNode as Node));
                            }
                            if (dc.View.Page.Children.Contains(this.HeadNode as Node) && !dc.View.Page.Children.Contains(this.TailNode as Node))
                            {
                                e = (new Point((this.TailNode as Node).OffsetX, (this.TailNode as Node).OffsetY)); //GetIntersectionPoint(new Point((this.TailNode as Node).OffsetX, (this.TailNode as Node).OffsetY), (this.HeadNode as Node));

                            }

                        }

                        x1 = s.X;// MeasureUnitsConverter.ToPixels(s.X, this.MeasurementUnit);
                        y1 = s.Y;// MeasureUnitsConverter.ToPixels(s.Y, this.MeasurementUnit);
                        x2 = e.X;// MeasureUnitsConverter.ToPixels(e.X, this.MeasurementUnit);
                        y2 = e.Y;// MeasureUnitsConverter.ToPixels(e.Y, this.MeasurementUnit);

                        PathGeometry pathgeometry = new PathGeometry();
                        PathFigure pathfigure = new PathFigure();
                        double num = 150.0;
                        Point t = new Point(x1, y1);
                        pathfigure.StartPoint = t;
                        this.m_TempStart = t;
                        if (this.ConnectorType == ConnectorType.Bezier)
                        {
                            BezierSegment segment = new BezierSegment();

                            if (isBottom)
                            {
                                segment = GetSegment(x1, y1 + num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                            }
                            else if (isTop)
                            {
                                segment = GetSegment(x1, y1 - num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                            }
                            else if (isRight)
                            {
                                segment = GetSegment(x1 + num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                            }
                            else
                            {
                                segment = GetSegment(x1 - num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                            }

                            pathfigure.Segments.Add(segment);
                        }
                        else
                        {
                            double x = Math.Pow((x1 - x2), 2);
                            double y = Math.Pow((y1 - y2), 2);
                            double d = Math.Sqrt(x + y);

                            double angle = ConnectorBase.findAngle(s, e);

                            pathfigure.Segments.Add(new ArcSegment(e, new Size(d / 2, this.ArcHeight), angle, false, this.ArcDirection, true));

                            pathgeometry.Figures.Add(pathfigure);
                        }

                        pathgeometry.Figures.Add(pathfigure);

                        if (HeadNode != null && TailNode != null)
                        {
                            m_isoverlapped = false;
                            this.TailDecoratorShape = this.InternalTailShape;
                            this.HeadDecoratorShape = this.InternalHeadShape;
                        }
                        if (this.ConnectorPathGeometry != null)
                        {
                            this.ConnectorPathGeometry.Figures.Clear();
                            this.ConnectorPathGeometry.Figures.Add(pathfigure);
                            this.UpdateDecoratorPosition();
                        }
                        else
                        {
                            this.ConnectorPathGeometry = pathgeometry;
                        }
                        this.m_TempEnd = new Point(x2, y2);
                        this.VirtualConnectorPathGeometry = pathgeometry.Clone();
                        this.WidenedPathGeometry = pathgeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, 1));
                        invalidateBridging = true;
                    }
                    catch
                    {
                    }
                    #endregion Bezier and Arc
                }

                else
                {
                    //if (this.dc != null && this.LineRoutingEnabled && isRouting)
                    //{
                    //    AStarLineRouter asl = new AStarLineRouter(this.dc.View);
                    //    this.dc.View.LineRouter = asl;
                    //}
                    if (dc.View._LineRoute || !this.LineRoutingEnabled || (!dc.View.IsDragged && !dc.View._LineRoute))
                    {
                        List<Point> connectionPoints = GetTerminalPoints();
                        if (IntermediatePoints == null)
                        {
                            IntermediatePoints = new List<Point>();
                        }

                        if (connectionPoints.Count > 0)
                        {
                            if (this.ConnectorType == ConnectorType.Orthogonal || (this.ConnectorType==ConnectorType.Straight && this.LineRoutingEnabled))
                            {
                                connectionPoints = meetOrhogonalConstraints(connectionPoints, cs);
                            }
                            if (IntermediatePoints.Count >= 1)
                            {
                                for (int i = IntermediatePoints.Count - 1; i >= 0; i--)
                                {
                                    connectionPoints.Insert(1, IntermediatePoints[i]);
                                }
                            }

                            if (this.ConnectorType == ConnectorType.Orthogonal || (this.ConnectorType == ConnectorType.Straight && this.LineRoutingEnabled))
                                adjustIntermediatePoints(connectionPoints);
                            ConnectionPoint = connectionPoints;
                            UpdatePathGeometry(connectionPoints);
                        }
                    }
                }


                if (LineAdorner != null && ConnectorType != ConnectorType.Bezier && ConnectorType != ConnectorType.Arc)
                {
                    (this.LineAdorner as LineConnectorAdorner).UpdateVertexsPosition();
                }
            }

            this.bridged = true;
            if (this.HeadNode != null && this.TailNode != null)
            {
                this.UpdateDecoratorSegment();
            }
            else
            {
                if (this.DecoratorSegments != null)
                    this.DecoratorSegments.Clear();
            }
            if (this.Ports != null)
            {
                foreach (var item in Ports)
                {
                    item.Invalidate();
                }
            }
        }

        internal void UpdatePathGeometry(List<Point> connectionPoints)
        {
            PathGeometry pathgeometry = new PathGeometry();
            m_isoverlapped = false;
            PathFigure pathfigure = new PathFigure();

            if (LineCornerRadius <= 0)
            {
                pathfigure.StartPoint = connectionPoints[0];
                connectionPoints.Remove(connectionPoints[0]);
                foreach (Point p in connectionPoints)
                {
                    pathfigure.Segments.Add(new LineSegment(p, true));
                }
            }
            else
            {
                Point pre = pathfigure.StartPoint = connectionPoints[0];
                Point first = this.adjustLengthPoint(pre, connectionPoints[1], -LineCornerRadius, true, true);
                pre = connectionPoints[1];
                Point preA = first;
                pathfigure.Segments.Add(new LineSegment(first, true));
                connectionPoints.Remove(connectionPoints[0]);
                connectionPoints.Remove(connectionPoints[0]);
                foreach (Point p in connectionPoints)
                {
                    Point st = this.adjustLengthPoint(pre, p, -LineCornerRadius, false, false);
                    Point en = this.adjustLengthPoint(pre, p, -LineCornerRadius, true, false);
                    if (st != p && en != pre)
                    {
                        pathfigure.Segments.Add(new QuadraticBezierSegment(pre, st, true));
                        pathfigure.Segments.Add(new LineSegment(en, true));
                    }
                    else
                    {
                        pathfigure.Segments.Add(new QuadraticBezierSegment(pre, adjustLengthPoint(pre, p, -(pre - p).Length / 2, true, false), true));
                    }
                    pre = p;
                    preA = en;
                }
                if (connectionPoints.Count > 0)
                {
                    if (pathfigure.Segments.Last() is LineSegment)
                    {
                        (pathfigure.Segments.Last() as LineSegment).Point = connectionPoints[connectionPoints.Count - 1];
                    }
                    else if (pathfigure.Segments.Last() is QuadraticBezierSegment)
                    {
                        (pathfigure.Segments.Last() as QuadraticBezierSegment).Point2 = connectionPoints[connectionPoints.Count - 1];
                    }
                }
            }

            pathgeometry.Figures.Add(pathfigure);

            if (this.ConnectorPathGeometry != null)
            {
                //this.ConnectorPathGeometry.Figures.Clear();
                //this.ConnectorPathGeometry.Figures.Add(pathfigure);
                if (this.ConnectorPathGeometry.Figures.Count > 0)
                {
                    this.ConnectorPathGeometry.Figures[0] = pathfigure;
                }
                else
                {
                    this.ConnectorPathGeometry.Figures.Add(pathfigure);
                }
            }
            else
            {
                this.ConnectorPathGeometry = pathgeometry;
            }
            this.VirtualConnectorPathGeometry = pathgeometry.Clone();
            this.WidenedPathGeometry = pathgeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, 1));
            this.m_TempStart = pathfigure.StartPoint;
            this.m_TempEnd = ConnectorBase.FindEndPoint(this.ConnectorPathGeometry);

            this.UpdateDecoratorPosition();

            setMinMax();
            invalidateBridging = true;
        }
        #region Label Alignments

        internal void UpdateLabel(Point m_TempStart, Point m_TempEnd)
        {
            Point StartPoint = m_TempStart;
            Point EndPoint = m_TempEnd;
            LabelEditor linelabel = editor;

            if (this.ConnectorType == ConnectorType.Straight)
            {
                this.LineAngle = findAngle(StartPoint, EndPoint);
            }

            if (linelabel != null)
            {
                UpdateLabelPosition(m_TempStart, m_TempEnd, this.ConnectionPoint);
            }  

            if (linelabel != null && this.lablecontentpresenter != null)
            {
                if (this.ConnectorType != ConnectorType.Arc && this.ConnectorType != ConnectorType.Bezier)
                {
                    RotateTransform transform = new RotateTransform();
                    RotateTransform ttransform = new RotateTransform();
                    if (this.LineAngle < 270 && this.LineAngle >= 90)
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
                                    this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - linelabel.DesiredSize.Height / 2, (StartPoint.Y + EndPoint.Y) / 2 + linelabel.DesiredSize.Width / 2);
                            }
                            if (this.LabelTemplateOrientation == LabelOrientation.Vertical)
                            {
                                ttransform.Angle = 270;
                            }

                            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2 - lablecontentpresenter.DesiredSize.Height / 2, (StartPoint.Y + EndPoint.Y) / 2 + linelabel.DesiredSize.Width / 2);

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
                                    this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - linelabel.DesiredSize.Width / 2, (StartPoint.Y + EndPoint.Y) / 2 - linelabel.DesiredSize.Height / 2);
                            }
                            if (this.LabelTemplateOrientation == LabelOrientation.Horizontal)
                            {
                                ttransform.Angle = 0;
                            }
                            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Center)
                            {
                               // this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2 - lablecontentpresenter.DesiredSize.Width / 2, (StartPoint.Y + EndPoint.Y) / 2 - linelabel.DesiredSize.Width / 2);
                            }

                        }

                        else
                        {

                            double angle = (this.LineAngle * Math.PI) / 180;

                        }

                    }
                    else
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
                                    this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - linelabel.DesiredSize.Height / 2, (StartPoint.Y + EndPoint.Y) / 2 + linelabel.DesiredSize.Width / 2);
                            }
                            if (this.LabelTemplateOrientation == LabelOrientation.Vertical)
                            {
                                ttransform.Angle = 270;
                            }

                            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2 - linelabel.DesiredSize.Height / 2, (StartPoint.Y + EndPoint.Y) / 2 + linelabel.DesiredSize.Width / 2);
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
                                    this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - linelabel.DesiredSize.Width / 2, (StartPoint.Y + EndPoint.Y) / 2 - linelabel.DesiredSize.Height / 2);
                            }
                            if (this.LabelTemplateOrientation == LabelOrientation.Horizontal)
                            {
                                ttransform.Angle = 0;
                            }
                            if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Center)
                            {
                                //this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2 - linelabel.DesiredSize.Width / 2, (StartPoint.Y + EndPoint.Y) / 2 - linelabel.DesiredSize.Height / 2);

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
                        double ang = this.LabelAngle;
                        if (!AllowCustomLabelPosition)
                        {

                            ttransform.Angle = this.LineAngle + Math.Abs(this.LabelAngle - ang);
                        }
                        else
                            ttransform.Angle = this.LabelAngle;
                    }

                    (linelabel.Parent as Grid).RenderTransform = transform;
                    lablecontentpresenter.RenderTransform = ttransform;

                }
            }
               
        }
        
        internal void UpdateLabelPosition(Point m_TempStart, Point m_TempEnd, List<Point> connectionPoints)
        {           
                Point StartPoint = m_TempStart;
                Point EndPoint = m_TempEnd;
                List<Point> ConnectionPoints = connectionPoints;
                LabelEditor linelabel = editor;
                if (linelabel != null)
                {
                    #region LabelAlignment

                    #region HorizontalAlignment.Left
                    if (this.LabelHorizontalAlignment == HorizontalAlignment.Left && !AllowCustomLabelPosition)
                    {
                        this.LabelPosition = StartPoint;
                        //UpdateLabelAlignment(linelabel);
                        if (this.ConnectorType == ConnectorType.Straight)
                        {
                            if (this.LabelOrientation == LabelOrientation.Auto)
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                if (LineAngle > 90 && LineAngle < 270)
                                {
                                    LabelAngle = 180;
                                    this.LabelPosition = new Point(StartPoint.X + (Math.Cos(angle)) * (linelabel.DesiredSize.Width), StartPoint.Y + (Math.Sin(angle) * (linelabel.DesiredSize.Width)));
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
                                    this.LabelPosition = new Point(StartPoint.X - linelabel.DesiredSize.Width, StartPoint.Y);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelPosition = new Point(StartPoint.X - linelabel.DesiredSize.Width, StartPoint.Y - linelabel.DesiredSize.Height);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelPosition = new Point(StartPoint.X, StartPoint.Y - linelabel.DesiredSize.Height);
                                }
                            }
                            else if (this.LabelOrientation == LabelOrientation.Vertical && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelPosition = new Point(StartPoint.X, StartPoint.Y + linelabel.DesiredSize.Width);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelPosition = new Point(StartPoint.X - linelabel.DesiredSize.Height, StartPoint.Y + linelabel.DesiredSize.Width);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelPosition = new Point(StartPoint.X - linelabel.DesiredSize.Height, StartPoint.Y);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelPosition = StartPoint;
                                }

                            }

                        }
                    }
                    #endregion

                    #region HorizontalAlignment.Right
                    else if (this.LabelHorizontalAlignment == HorizontalAlignment.Right && !AllowCustomLabelPosition)
                    {
                        this.LabelPosition = EndPoint;
                        if (this.ConnectorType == ConnectorType.Straight)
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
                                    this.LabelPosition = new Point(EndPoint.X - (Math.Cos(angle) * (linelabel.DesiredSize.Width)), EndPoint.Y - (Math.Sin(angle) * (linelabel.DesiredSize.Width)));
                                }
                            }
                            else if (this.LabelOrientation == LabelOrientation.Horizontal)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelPosition = new Point(EndPoint.X - linelabel.DesiredSize.Width - 10, EndPoint.Y - linelabel.DesiredSize.Height);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y - linelabel.DesiredSize.Height);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelPosition = new Point(EndPoint.X - linelabel.DesiredSize.Width, EndPoint.Y + 5);
                                }

                            }
                            else if (this.LabelOrientation == LabelOrientation.Vertical)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelPosition = new Point(EndPoint.X - linelabel.DesiredSize.Height, EndPoint.Y - 5);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelPosition = new Point(EndPoint.X, EndPoint.Y - 5);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelPosition = new Point(EndPoint.X + 5, EndPoint.Y + linelabel.DesiredSize.Width);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelPosition = new Point(EndPoint.X - linelabel.DesiredSize.Height - 5, EndPoint.Y + linelabel.DesiredSize.Width);
                                }
                            }

                        }
                        else
                        {
                            double angle = (this.LineAngle * Math.PI) / 180;
                            this.LabelPosition = new Point(EndPoint.X - (Math.Cos(angle) * (linelabel.DesiredSize.Width)), EndPoint.Y - (Math.Sin(angle) * (linelabel.DesiredSize.Width)));
                        }
                    }
                    #endregion

                    #region !AllowCustomLabelPosition
                    else if (!AllowCustomLabelPosition)
                    {
                        double angle = (this.LineAngle * Math.PI) / 180;
                        if (this.ConnectorType != ConnectorType.Orthogonal)
                        {
                            if (this.ConnectorType == ConnectorType.Bezier || this.ConnectorType == ConnectorType.Arc)
                            {
                                this.LabelAngle = 0;
                                this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (linelabel.DesiredSize.Width + 10)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (linelabel.DesiredSize.Width + 10)));
                            }
                            else
                            {
                                if (LineAngle > 90 && LineAngle < 270)
                                {
                                    LabelAngle = 180;
                                    this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 + (Math.Cos(angle) * (linelabel.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 + (Math.Sin(angle) * (linelabel.DesiredSize.Width / 2)));
                                }
                                else
                                {
                                    LabelAngle = 0;
                                    this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (linelabel.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (linelabel.DesiredSize.Width / 2)));
                                }
                            }
                        }
                        else
                        {
                            this.LabelAngle = 0;
                            if (this.ConnectorType == ConnectorType.Orthogonal)
                            {
                                if (connectionPoints.Count > 1)
                                {
                                    if (StartPoint.Y == connectionPoints[1].Y)
                                    {
                                        this.LabelPosition = new Point((connectionPoints[0].X + connectionPoints[1].X) / 2, connectionPoints[0].Y);
                                    }
                                }
                            }
                            else
                            {
                                this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (linelabel.DesiredSize.Width / 2)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (linelabel.DesiredSize.Width / 2)));
                            }
                            //this.LabelPosition = new Point((StartPoint.X + EndPoint.X) / 2, (StartPoint.Y + EndPoint.Y) / 2 + ((Math.Abs(StartPoint.X - EndPoint.X) < this.linelabel.ActualWidth) ? (this.ConnectionPoints[3].Y - this.ConnectionPoints[0].Y) / 2 : 0));
                        }
                    }
                    #endregion
                    #endregion
                }
                if (this.lablecontentpresenter != null)
                {

                    #region LabelTemplate Alignment

                    #region  HorizontalAlignment.Left

                    if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Left)
                    {
                        this.LabelTemplatePosition = StartPoint;

                        if (this.ConnectorType == ConnectorType.Straight)
                        {
                            if (this.LabelTemplateOrientation == LabelOrientation.Auto)
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                if (LineAngle > 90 && LineAngle < 270)
                                {
                                    if (!AllowCustomLabelPosition)
                                        LabelAngle = 180;
                                    //this.LabelTemplatePosition = new Point(StartPoint.X + (Math.Cos(angle)) * (this.lablecontentpresenter.DesiredSize.Width), StartPoint.Y + (Math.Sin(angle) * (linelabel.DesiredSize.Width)));
                                }
                                else
                                {
                                    if (!AllowCustomLabelPosition)
                                        LabelAngle = 0;
                                    //this.LabelTemplatePosition = new Point(StartPoint.X + (Math.Cos(angle)), StartPoint.Y + (Math.Sin(angle)));
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
                                    this.LabelTemplatePosition = new Point(StartPoint.X - this.lablecontentpresenter.DesiredSize.Width, StartPoint.Y);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X - this.lablecontentpresenter.DesiredSize.Width, StartPoint.Y - this.lablecontentpresenter.DesiredSize.Height);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X, StartPoint.Y - this.lablecontentpresenter.DesiredSize.Height);
                                }
                            }
                            else if (this.LabelTemplateOrientation == LabelOrientation.Vertical && this.ConnectorType == ConnectorType.Straight)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X, StartPoint.Y + this.lablecontentpresenter.DesiredSize.Width);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X - this.lablecontentpresenter.DesiredSize.Height, StartPoint.Y + this.lablecontentpresenter.DesiredSize.Width);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelTemplatePosition = new Point(StartPoint.X - this.lablecontentpresenter.DesiredSize.Height, StartPoint.Y);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelTemplatePosition = StartPoint;
                                }

                            }

                        }
                    }
                    #endregion

                    #region HorizontalAlignment.Right
                    else if (this.LabelTemplateHorizontalAlignment == HorizontalAlignment.Right)
                    {

                        if (this.ConnectorType == ConnectorType.Straight)
                        {
                            if (this.LabelTemplateOrientation == LabelOrientation.Auto)
                            {
                                double angle = (this.LineAngle * Math.PI) / 180;
                                if (LineAngle > 90 && LineAngle < 270)
                                {
                                    if (!AllowCustomLabelPosition)
                                        LabelAngle = 180;
                                   // this.LabelTemplatePosition = new Point(EndPoint.X - (Math.Cos(angle)), EndPoint.Y - (Math.Sin(angle)));
                                }
                                else
                                {
                                    if (!AllowCustomLabelPosition)
                                        LabelAngle = 0;
                                   // this.LabelTemplatePosition = new Point(EndPoint.X - (Math.Cos(angle) * (this.lablecontentpresenter.DesiredSize.Width)), EndPoint.Y - (Math.Sin(angle) * (this.lablecontentpresenter.DesiredSize.Width)));
                                }
                            }
                            else if (this.LabelTemplateOrientation == LabelOrientation.Horizontal)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X - this.lablecontentpresenter.DesiredSize.Width - 10, EndPoint.Y - this.lablecontentpresenter.DesiredSize.Height);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y - this.lablecontentpresenter.DesiredSize.Height);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X - this.lablecontentpresenter.DesiredSize.Width, EndPoint.Y + 5);
                                }

                            }
                            else if (this.LabelTemplateOrientation == LabelOrientation.Vertical)
                            {
                                if (this.LineAngle > 0 && this.LineAngle <= 90)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X - this.lablecontentpresenter.DesiredSize.Height, EndPoint.Y - 5);
                                }
                                else if (LineAngle > 90 && LineAngle <= 180)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X, EndPoint.Y - 5);
                                }
                                else if (LineAngle > 180 && LineAngle <= 270)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X + 5, EndPoint.Y + this.lablecontentpresenter.DesiredSize.Width);

                                }
                                else if (LineAngle > 270 && LineAngle <= 360)
                                {
                                    this.LabelTemplatePosition = new Point(EndPoint.X - this.lablecontentpresenter.DesiredSize.Height - 5, EndPoint.Y + this.lablecontentpresenter.DesiredSize.Width);
                                }
                            }

                        }

                        else
                        {
                            double angle = (this.LineAngle * Math.PI) / 180;
                            this.LabelTemplatePosition = new Point(EndPoint.X - (Math.Cos(angle) * (this.lablecontentpresenter.ActualWidth + 10)), EndPoint.Y - (Math.Sin(angle) * (this.lablecontentpresenter.ActualWidth + 10)));
                        }
                    }
                    #endregion

                    #region HorizontalAlignment.Center
                    else
                    {
                        double angle = (this.LineAngle * Math.PI) / 180;
                        if (this.ConnectorType != ConnectorType.Orthogonal)
                        {
                          //  this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2 - (Math.Cos(angle) * (this.lablecontentpresenter.ActualHeight + 10)), (StartPoint.Y + EndPoint.Y) / 2 - (Math.Sin(angle) * (this.lablecontentpresenter.ActualHeight + 10)));
                        }
                        else
                        {
                            this.LabelTemplatePosition = new Point((StartPoint.X + EndPoint.X) / 2, (StartPoint.Y + EndPoint.Y) / 2 + ((Math.Abs(StartPoint.X - EndPoint.X) < this.lablecontentpresenter.ActualWidth) ? (EndPoint.Y - StartPoint.Y) / 2 : 0));
                        }

                    }
                    #endregion
                    #endregion
                }
        }
        #endregion

        internal void SetLineBridging()
        {
            if (this.dc != null && this.LineBridgingEnabled)
            {
                Dictionary<int, List<ArcSegment>> arcD = new Dictionary<int, List<ArcSegment>>();
                Dictionary<int, List<Point>> startBD = new Dictionary<int, List<Point>>();
                int count = -1;
                foreach (LineConnector lc in dc.Model.Connections)
                {
                    if ((lc.zOrder < this.zOrder || lc.ConnectorType == ConnectorType.Bezier || lc.ConnectorType == ConnectorType.Arc || !lc.LineBridgingEnabled)
                        && (lc.VirtualConnectorPathGeometry != null && this.VirtualConnectorPathGeometry != null)
                        && (lc.invalidateBridging || this.invalidateBridging)
                        )
                    {
                        if (IsCPGOverlapped(this.VirtualConnectorPathGeometry.Bounds, lc.VirtualConnectorPathGeometry.Bounds))
                        {
                            Point[] pts = new Point[(this.IntermediatePoints.Count>lc.IntermediatePoints.Count?this.IntermediatePoints.Count : lc.IntermediatePoints.Count)];
                            if (!lc.Equals(this))
                            {
                                //if (lc.ConnectorType == ConnectorType.Orthogonal || lc.ConnectorType == ConnectorType.Straight)
                                //{
                                //    lc.UpdateConnectorPathGeometry();
                                //}                                
                                if (lc.ConnectorType == ConnectorType.Orthogonal && this.ConnectorType == ConnectorType.Orthogonal)
                                {
                                    List<Point> pts1 = new List<Point>();
                                    List<Point> line1 = new List<Point>();
                                    line1.Add(this.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                                    line1.AddRange(IntermediatePoints);
                                    line1.Add(ConnectorBase.FindEndPoint(this.VirtualConnectorPathGeometry));

                                    List<Point> line2 = new List<Point>();
                                    line2.Add(lc.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                                    line2.AddRange(lc.IntermediatePoints);
                                    line2.Add(ConnectorBase.FindEndPoint(lc.VirtualConnectorPathGeometry)); 
                                    pts1 = FindPOIBetweenTwoPolyLine(line1, line2, false);
                                    pts = (pts1.ToArray<Point>()).ToArray<Point>();
                                    foreach (Point p in pts)
                                    {
                                        if (line1.Contains(p) || (line2.Contains(p)))
                                        {
                                            pts1.Remove(p);
                                        }
                                    }
                                    pts = (pts1.ToArray<Point>()).ToArray<Point>();
                                }
                                else
                                {
                                    pts = GetIntersectionPointsFromWidened(this.WidenedPathGeometry, lc.WidenedPathGeometry);
                                }
                            }
                            else
                            {
                                List<Point> pts1;

                                List<Point> line1 = new List<Point>();
                                line1.Add(this.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                                line1.AddRange(IntermediatePoints);
                                line1.Add(ConnectorBase.FindEndPoint(this.VirtualConnectorPathGeometry));

                                List<Point> line2 = new List<Point>();
                                line2.Add(lc.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                                line2.AddRange(lc.IntermediatePoints);
                                line2.Add(ConnectorBase.FindEndPoint(lc.VirtualConnectorPathGeometry));

                                pts1 = FindPOIBetweenTwoPolyLine(line1, line2, true);
                                pts = (pts1.ToArray<Point>()).ToArray<Point>();                                
                            }

                            foreach (Point p in pts)
                            {
                                    double fullLength;
                                    Point dummy;
                                    int segmentIndex;
                                    double length = GetLengthAtFractionPoint(this.VirtualConnectorPathGeometry.Figures[0], p, out fullLength, out segmentIndex);
                                    if (segmentIndex < 0)
                                    {
                                        continue;
                                    }
                                    if (segmentIndex > 0 && !(this.VirtualConnectorPathGeometry.Figures[0].Segments[segmentIndex] is LineSegment))
                                    {
                                        continue;
                                    }
                                    Point startBridge, endBridge;
                                    double fractLength = (length - (this.BridgeSpacing / 2)) / fullLength;
                                    this.VirtualConnectorPathGeometry.GetPointAtFractionLength(fractLength, out startBridge, out dummy);
                                    fractLength = (length + (this.BridgeSpacing / 2)) / fullLength;
                                    this.VirtualConnectorPathGeometry.GetPointAtFractionLength(fractLength, out endBridge, out dummy);

                                    SweepDirection sd;

                                    Point start = new Point(0, 0), end;
                                    if (segmentIndex == 0)
                                    {
                                        start = (this.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                                    }
                                    else
                                    {
                                        PathSegment seg = this.VirtualConnectorPathGeometry.Figures[0].Segments[segmentIndex - 1];
                                        if (seg is LineSegment)
                                        {
                                            start = (seg as LineSegment).Point;
                                        }
                                        else if (seg is QuadraticBezierSegment)
                                        {
                                            start = (seg as QuadraticBezierSegment).Point2;
                                        }
                                        else if (seg is PolyLineSegment)
                                        {
                                            start = (seg as PolyLineSegment).Points.Last();
                                        }
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
                                        Point fixedpoint = start;
                                        //if (segmentIndex == 0)
                                        //{
                                        //    fixedpoint = (this.VirtualConnectorPathGeometry.Figures[0].StartPoint);
                                        //}
                                        //else
                                        //{
                                        //    fixedpoint = ((this.VirtualConnectorPathGeometry.Figures[0].Segments[segmentIndex - 1] as LineSegment).Point);
                                        //}

                                        double fix = Math.Abs((fixedpoint - endBridge).Length);
                                        double var;

                                        int insertAt = -1;
                                        count = -1;
                                        foreach (ArcSegment arc in arcD[segmentIndex])
                                        {
                                            count++;
                                            var = Math.Abs((fixedpoint - arc.Point).Length);
                                            if (fix < var)
                                            {
                                                insertAt = count;
                                                break;
                                            }
                                        }
                                        if (insertAt >= 0)
                                        {
                                            arcD[segmentIndex].Insert(insertAt, new ArcSegment(endBridge, new Size(1, 1), 0, true, sd, true));
                                            startBD[segmentIndex].Insert(insertAt, startBridge);
                                        }
                                        else
                                        {
                                            arcD[segmentIndex].Add(new ArcSegment(endBridge, new Size(1, 1), 0, true, sd, true));
                                            startBD[segmentIndex].Add(startBridge);
                                        }
                                    }
                                    else
                                    {
                                        List<ArcSegment> arcs = new List<ArcSegment>();
                                        arcs.Add(new ArcSegment(endBridge, new Size(1, 1), 0, true, sd, true));
                                        List<Point> points = new List<Point>();
                                        points.Add(startBridge);
                                        arcD.Add(segmentIndex, arcs);
                                        startBD.Add(segmentIndex, points);
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
                        this.ConnectorPathGeometry = this.VirtualConnectorPathGeometry.Clone();
                    }
                    foreach (int Index in v)
                    {
                        this.bridged = true;
                        for (int i = 1; i < arcD[Index].Count; i++)
                        {
                            if ((arcD[Index][i].Point - arcD[Index][i - 1].Point).Length < BridgeSpacing)
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
                            ConnectorPathGeometry.Figures[0].Segments.Insert(segmentIndex + 2, new LineSegment(end, true));
                            (ConnectorPathGeometry.Figures[0].Segments[segmentIndex] as LineSegment).Point = startBD[Index][arcD[Index].IndexOf(arc)];
                        }
                    }

                }
                //else
                //{
                //    foreach (LineConnector l in dc.Model.Connections)
                //    {
                //        if (l == this)
                //        {
                //            l.InvalidateConnectorPathGeometry();
                //        }
                //    }
                //}
            }

        }

        public bool IsCheckPointOnLine(Point endPoint1, Point endPoint2, Point checkPoint)
        {
            return (((double)checkPoint.Y - endPoint1.Y)) / ((double)(checkPoint.X - endPoint1.X))
                == ((double)(endPoint2.Y - endPoint1.Y)) / ((double)(endPoint2.X - endPoint1.X));
        }
        
        private bool IsCPGOverlapped(Rect rect, Rect rect_2)
        {
            return rect.IntersectsWith(rect_2);
        }

        private void extendPoints(bool isTop, bool isBottom, bool isLeft, bool isRight, bool tisTop, bool tisBottom, bool tisLeft, bool tisRight, out Point s, out Point e, Point xs, Point xe)
        {
            s = xs;
            e = xe;
            double fivezero = 50;
            if (isTop)
            {
                s = new Point(s.X, s.Y - fivezero);
            }
            else if (isBottom)
            {
                s = new Point(s.X, s.Y + fivezero);
            }
            else if (isRight)
            {
                s = new Point(s.X + fivezero, s.Y);
            }
            //else if (isRight)
            else if (isLeft)
            {
                s = new Point(s.X - fivezero, s.Y);
            }

            if (tisTop)
            {
                e = new Point(e.X, e.Y - fivezero);
            }
            else if (tisBottom)
            {
                e = new Point(e.X, e.Y + fivezero);
            }
            else if (tisLeft)
            {
                e = new Point(e.X - fivezero, e.Y);
            }
            else if (tisRight)
            {
                e = new Point(e.X + fivezero, e.Y);
            }

        }

        internal Rect getNodeRect(Node n)
        {
            Rect rect = new Rect();
            if (dc == null)
            {
                dc = DiagramPage.GetDiagramControl((FrameworkElement)this);
            }
            if (n != null)
            {
                if (n.Boundaries != null)
                {
                    if (n.Boundaries.RenderedGeometry != null)
                    {
                        rect = n.Boundaries.RenderedGeometry.Bounds;
                    }
                    if ((n.Boundaries is Path) && (n.Boundaries as Path).Data != null && (n.Boundaries as Path).Data != Geometry.Empty)
                    {
                        (n.Boundaries as Path).RenderTransform = new RotateTransform(n.RotateAngle,
                                     0,
                                      0);
                        rect = (n.Boundaries as Path).Data.Bounds;
                    }
                }
                else if (dc != null && dc.View != null)// && !dc.View.Page.Children.Contains(n))
                {
                    rect = new Rect(n.PxOffsetX, n.PxOffsetY, n.ActualWidth, n.ActualHeight);
                }
            }
            return rect;
        }
        internal double verifyNan(double value)
        {
            if (Double.IsNaN(value))
            {
                return 0;
            }
            else
            {
                return value;
            }
        }

        internal  Point GetRotatePoint(Node nod,Point poin)
        {
            var mat = new Matrix();
            mat.RotateAt(nod.RotateAngle,poin.X,poin.Y);
            var tran = new MatrixTransform(mat);
            return tran.Transform(poin);
        }

        internal List<Point> GetTerminalPoints()
        {
            List<Point> ret = new List<Point>();
            if (this.LineAdorner != null)
            {
                if ((this.LineAdorner as LineConnectorAdorner).ConnectorPoint)
                {
                    (this.LineAdorner as LineConnectorAdorner).ConnectorPoint = false;
                    if ((this.LineAdorner as LineConnectorAdorner).IsHeadThumb)
                    {
                        HeadNode = null;
                    }
                    else if ((this.LineAdorner as LineConnectorAdorner).IsTailThumb)
                    {
                        TailNode = null;
                    }

                }
            }
            if (this.HeadNode != null && this.TailNode != null)
            {
                if ((this.HeadNode as Node).IsInternallyLoaded && (this.TailNode as Node).IsInternallyLoaded || (dview.EnableVirtualization &&(! (this.TailNode as Node).IsLoaded|| !(this.HeadNode as Node).IsLoaded)))
                {
                    if (IntermediatePoints != null && IntermediatePoints.Count >= 1)
                    {
                        ret.Add(GetIntersectionPoint(IntermediatePoints[0], HeadNode as Node));
                        ret.Add(GetIntersectionPoint(IntermediatePoints[IntermediatePoints.Count - 1], TailNode as Node));
                    }
                    else
                    {
                        if (this.ConnectionTailPort != null)
                        {
                            Point pt = new Point((TailNode as Node).PxOffsetX + ConnectionTailPort.Left, (TailNode as Node).PxOffsetY + ConnectionTailPort.Top);
                            ret.Add(GetIntersectionPoint(pt, HeadNode as Node));
                        }
                        else
                        {
                            if ((this.TailNode as Node).IsLoaded)
                                ret.Add(GetIntersectionPoint((TailNode as Node).PxPosition, HeadNode as Node));
                            else
                            {
                                ret.Add(GetIntersectionPoint((new Point((TailNode as Node).PxOffsetX + (TailNode as Node).Width / 2, (TailNode as Node).PxOffsetY + (TailNode as Node).Height / 2)), HeadNode as Node));
                            }
                        }
                        if (this.ConnectionHeadPort != null)
                        {
                            Point pt = new Point((HeadNode as Node).PxOffsetX + ConnectionHeadPort.Left, (HeadNode as Node).PxOffsetY + ConnectionHeadPort.Top);
                            ret.Add(GetIntersectionPoint(pt, TailNode as Node));
                        }
                        else
                        {
                            if ((this.HeadNode as Node).IsLoaded)
                                ret.Add(GetIntersectionPoint((HeadNode as Node).PxPosition, TailNode as Node));
                            else
                            {
                                ret.Add(GetIntersectionPoint((new Point((HeadNode as Node).PxOffsetX + (HeadNode as Node).Width / 2, (HeadNode as Node).PxOffsetY + (HeadNode as Node).Height / 2)),TailNode as Node));
                            }
                        }
                    }
                }
                else if (!(this.HeadNode as Node).IsInternallyLoaded && (this.TailNode as Node).IsInternallyLoaded)
                {
                    ret.Add(new Point((this.HeadNode as Node).OffsetX + (verifyNan((this.HeadNode as Node)._Width) / 2), (this.HeadNode as Node).OffsetY + (verifyNan((this.HeadNode as Node)._Height) / 2)));
                    if (IntermediatePoints != null && IntermediatePoints.Count >= 1)
                    {
                        ret.Add(GetIntersectionPoint(IntermediatePoints[IntermediatePoints.Count - 1], TailNode as Node));
                    }
                    else
                    {
                        ret.Add(GetIntersectionPoint(new Point((this.HeadNode as Node).OffsetX, (this.HeadNode as Node).OffsetY), TailNode as Node));

                    }
                }
                else if ((this.HeadNode as Node).IsInternallyLoaded && !(this.TailNode as Node).IsInternallyLoaded)
                {
                    if (IntermediatePoints != null && IntermediatePoints.Count >= 1)
                    {
                        ret.Add(GetIntersectionPoint(IntermediatePoints[0], HeadNode as Node));
                    }
                    else
                    {
                        ret.Add(GetIntersectionPoint(new Point((this.TailNode as Node).OffsetX, (this.TailNode as Node).OffsetY), HeadNode as Node));
                    }
                    ret.Add(new Point((this.TailNode as Node).OffsetX + (verifyNan((this.TailNode as Node)._Width) / 2), (this.TailNode as Node).OffsetY + (verifyNan((this.TailNode as Node)._Height) / 2)));
                }
            }
            else if (this.HeadNode == null && this.TailNode != null)
            {
                if (ConnectionHeadPort != null && ConnectionHeadPort.Edge != null)
                {
                    ret.Add(new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top));
                }
                else
                {
                    ret.Add(this.PxStartPointPosition);
                }
                if (IntermediatePoints != null && IntermediatePoints.Count >= 1)
                {
                    ret.Add(GetIntersectionPoint(IntermediatePoints[IntermediatePoints.Count - 1], TailNode as Node));
                }
                else
                {
                    ret.Add(GetIntersectionPoint(this.PxStartPointPosition, TailNode as Node));
                }
            }
            else if (this.HeadNode != null && this.TailNode == null)
            {
                if (IntermediatePoints != null && IntermediatePoints.Count >= 1)
                {
                    ret.Add(GetIntersectionPoint(IntermediatePoints[0], HeadNode as Node));
                }
                else
                {
                    ret.Add(GetIntersectionPoint(this.PxEndPointPosition, HeadNode as Node));
                }
                if (ConnectionTailPort != null && ConnectionTailPort.Edge != null)
                {
                    ret.Add(new Point(ConnectionTailPort.Left, ConnectionTailPort.Top));
                }
                else
                {
                    ret.Add(this.PxEndPointPosition);
                }
            }
            else if (this.HeadNode == null && this.TailNode == null)
            {
                if (ConnectionHeadPort != null && ConnectionHeadPort.Edge != null)
                {
                    ret.Add(new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top));
                }
                else
                {
                    ret.Add(PxStartPointPosition);
                }
                if (ConnectionTailPort != null && ConnectionTailPort.Edge != null)
                {
                    ret.Add(new Point(ConnectionTailPort.Left, ConnectionTailPort.Top));
                }
                else
                {
                    ret.Add(PxEndPointPosition);
                }
            }
            if (dview != null && dview.Page != null && ret != null && ret.Count == 2)
            {
                if (this.ConnectionHeadPort != null && HeadNode != null)
                {
                    //ret[0] = new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top);
                    //ret[0] = (HeadNode as Node).TranslatePoint(ret[0], dview.Page);
                    ret[0] = (HeadNode as Node).Transform(new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top));
                }
                if (this.ConnectionTailPort != null && TailNode != null)
                {
                    //ret[1] = new Point(ConnectionTailPort.Left, ConnectionTailPort.Top);
                    //ret[1] = (TailNode as Node).TranslatePoint(ret[1], dview.Page);
                    ret[1] = (TailNode as Node).Transform(new Point(ConnectionTailPort.Left, ConnectionTailPort.Top));
                }
            }
            return ret;
        }

        private List<Point> meetOrhogonalConstraints(List<Point> connectionPoints, double cs)
        {
            if (this.AutoAdjustPoints && this.HeadNode != null && this.TailNode != null && this.ConnectionHeadPort != null && this.ConnectionTailPort != null && !this.LineRoutingEnabled)
            {
                autoAdjustPoints();
            }
            else
            {
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
                    connectionPoints = GetTerminalPoints();
                    if (IntermediatePoints.Count == 0)
                    {
                        Node Node = null;
                        ConnectionPort Port = null;
                        int fixedIndex = 0;
                        int movableIndex = 0;
                        if (ConnectionHeadPort != null && HeadNode == null && TailNode != null && ConnectionHeadPort.Edge != null)
                        {
                            Node = TailNode as Node;
                            fixedIndex = 0;
                            movableIndex = 1;
                            Port = ConnectionHeadPort;
                        }
                        else if (ConnectionTailPort != null && TailNode == null && HeadNode != null && ConnectionTailPort.Edge != null)
                        {
                            Node = HeadNode as Node;
                            fixedIndex = 1;
                            movableIndex = 0;
                            Port = ConnectionTailPort;
                        }
                        if (Node != null)
                        {
                            List<Point> line = (Port.Edge as LineConnector).GetTerminalPoints();
                            line.InsertRange(1, Port.Edge.IntermediatePoints);
                            double minDistance = double.MaxValue;
                            int segment = -1;
                            Point proposed = new Point(0, 0);

                            for (int i = 0; i < line.Count - 1; i++)
                            {
                                Point start = line[i];
                                Point end = line[i + 1];
                                double xDel = Math.Abs(start.X - end.X);
                                double yDel = Math.Abs(start.Y - end.Y);
                                if (xDel == 0)
                                {
                                    if (Node.OffsetY + Node.ActualHeight / 2 > Math.Min(start.Y, end.Y) &&
                                        Node.OffsetY + Node.ActualHeight / 2 < Math.Max(start.Y, end.Y))
                                    {
                                        segment = i;
                                        double dist = Math.Min(
                                                        Math.Abs(start.X - (Node as Node).PxOffsetX),
                                                        Math.Abs(start.X - ((Node as Node).PxOffsetX + Node.ActualWidth))
                                                        );
                                        if (dist < minDistance)
                                        {
                                            minDistance = dist;
                                            proposed = new Point(start.X, (Node as Node).PxOffsetY + Node.ActualHeight / 2);
                                        }
                                    }
                                }
                                else if (yDel == 0)
                                {
                                    if (Node.OffsetX + Node.ActualWidth / 2 > Math.Min(start.X, end.X) &&
                                        Node.OffsetX + Node.ActualWidth / 2 < Math.Max(start.X, end.X))
                                    {
                                        segment = i;
                                        double dist = Math.Min(
                                                        Math.Abs(start.Y - (Node as Node).PxOffsetY),
                                                        Math.Abs(start.Y - ((Node as Node).PxOffsetY + Node.ActualHeight))
                                                        );
                                        if (dist < minDistance)
                                        {
                                            minDistance = dist;
                                            proposed = new Point((Node as Node).PxOffsetX + Node.ActualWidth / 2, start.Y);
                                        }
                                    }
                                }
                                else
                                {
                                }
                            }
                            if (segment > -1 && minDistance <= 20)
                            {
                                connectionPoints[fixedIndex] = proposed;
                                connectionPoints[movableIndex] = GetIntersectionPoint(connectionPoints[fixedIndex], Node as Node);
                                Port.PositionToLength(connectionPoints[fixedIndex]);
                            }
                            else
                            {
                                Point start = connectionPoints[fixedIndex];
                                Point end = connectionPoints[movableIndex];
                                double xDel = Math.Abs(start.X - end.X);
                                double yDel = Math.Abs(start.Y - end.Y);
                                if (xDel != 0 && yDel != 0)
                                {
                                    if (xDel < yDel)
                                    {
                                        connectionPoints[fixedIndex] = new Point(end.X, start.Y);
                                        connectionPoints[movableIndex] = GetIntersectionPoint(connectionPoints[fixedIndex], Node as Node);
                                        connectionPoints[movableIndex] = new Point(end.X, connectionPoints[movableIndex].Y);
                                        Port.PositionToLength(connectionPoints[fixedIndex]);
                                    }
                                    else
                                    {
                                        connectionPoints[fixedIndex] = new Point(start.X, end.Y);
                                        connectionPoints[movableIndex] = GetIntersectionPoint(connectionPoints[fixedIndex], Node as Node);
                                        connectionPoints[movableIndex] = new Point(connectionPoints[movableIndex].X, end.Y);
                                        Port.PositionToLength(connectionPoints[fixedIndex]);
                                    }
                                }
                                Point? intersect;
                                Port.Edge.IntersectsWith(connectionPoints[fixedIndex], out intersect);

                                if (segment > -1 && intersect.HasValue && (intersect.Value - connectionPoints[fixedIndex]).Length > 3)
                                {
                                    connectionPoints[fixedIndex] = proposed;
                                    connectionPoints[movableIndex] = GetIntersectionPoint(connectionPoints[fixedIndex], Node as Node);
                                    Port.PositionToLength(connectionPoints[fixedIndex]);
                                }
                            }
                        }
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
                                NodeInfo head;
                                Rect sourceRect = getNodeRect(HeadNode as Node);
                                head = getRectAsNodeInfo(sourceRect);

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
                                NodeInfo head;
                                Rect sourceRect = getNodeRect(HeadNode as Node);
                                head = getRectAsNodeInfo(sourceRect);

                                if (sourceRect != Rect.Empty)
                                {
                                    sourceRect = new Rect(head.Left, head.Top, head.Size.Width, head.Size.Height);
                                }
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
                                NodeInfo tail;
                                Rect sourceRect = getNodeRect(TailNode as Node);
                                tail = getRectAsNodeInfo(sourceRect);

                                if (sourceRect != Rect.Empty)
                                {
                                    sourceRect = new Rect(tail.Left, tail.Top, tail.Size.Width, tail.Size.Height);
                                }

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
                                NodeInfo tail;
                                Rect sourceRect = getNodeRect(TailNode as Node);
                                tail = getRectAsNodeInfo(sourceRect);

                                if (sourceRect != Rect.Empty)
                                {
                                    sourceRect = new Rect(tail.Left, tail.Top, tail.Size.Width, tail.Size.Height);
                                }
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
                }
            }
            return connectionPoints;
        }

        private List<Point> autoAdjustPoints()
        {
            #region Init

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

            NodeInfo HeadNode = (this.HeadNode as Node).GetExtInfo(new Thickness(0));
            NodeInfo TailNode = (this.TailNode as Node).GetExtInfo(new Thickness(0));

            double headMargin = 0d;
            double tailMargin = 0d;

            switch (HeadDirection)
            {
                case Dock.Bottom:
                    headMargin = ConnectionHeadPort.PagePosition.Y + headDist - HeadNode.Bottom;
                    break;
                case Dock.Left:
                    headMargin = -ConnectionHeadPort.PagePosition.X + headDist + HeadNode.Left;
                    break;
                case Dock.Right:
                    headMargin = ConnectionHeadPort.PagePosition.X + headDist - HeadNode.Right;
                    break;
                case Dock.Top:
                    headMargin = -ConnectionHeadPort.PagePosition.Y + headDist + HeadNode.Top;
                    break;
            }

            switch (TailDirection)
            {
                case Dock.Bottom:
                    tailMargin = ConnectionTailPort.PagePosition.Y + tailDist - TailNode.Bottom;
                    break;
                case Dock.Left:
                    tailMargin = -ConnectionTailPort.PagePosition.X + tailDist + TailNode.Left;
                    break;
                case Dock.Right:
                    tailMargin = ConnectionTailPort.PagePosition.X + tailDist - TailNode.Right;
                    break;
                case Dock.Top:
                    tailMargin = -ConnectionTailPort.PagePosition.Y + tailDist + TailNode.Top;
                    break;
            }

            HeadNode = (this.HeadNode as Node).GetExtInfo(new Thickness(headMargin));
            TailNode = (this.TailNode as Node).GetExtInfo(new Thickness(tailMargin));

            #endregion

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
                        if ((ConnectionTailPort.PagePosition.Y > HeadNode.TopExt) && (ConnectionTailPort.PagePosition.Y < HeadNode.BottomExt))
                        {
                            is4Pts = true;
                        }
                    }
                    else if (HeadDirection == Dock.Left && TailDirection == Dock.Left)
                    {
                        if ((ConnectionTailPort.PagePosition.Y > HeadNode.TopExt) && (ConnectionTailPort.PagePosition.Y < HeadNode.BottomExt))
                        {
                            is4Pts = true;
                        }
                    }
                    else if (HeadDirection == Dock.Left && TailDirection == Dock.Right)
                    {
                        if (HeadNode.LeftExt < TailNode.RightExt)
                        {
                            is4Pts = true;
                        }
                    }
                    else if (HeadDirection == Dock.Right && TailDirection == Dock.Left)
                    {
                        if (HeadNode.RightExt > TailNode.LeftExt)
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
                            if (PxInterPts.Count > 4)
                            {
                                PxInterPts.RemoveAt(0);
                            }
                            else
                            {
                                PxInterPts.Add(new Point(0, 0));
                            }
                        }
                        //// Update Head (Right)
                        if (HeadDirection == Dock.Right)
                        {
                            if (PxInterPts.Count >= 4)
                            {
                                if (isHeadRecal || PxInterPts[0].X < HeadNode.RightExt)
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
                                if (isHeadRecal || PxInterPts[0].X > HeadNode.LeftExt)
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
                                if (isTailRecal || PxInterPts[last - 1].X < TailNode.RightExt)
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
                                if (isTailRecal || PxInterPts[last - 1].X > TailNode.LeftExt)
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
                                PxInterPts[last - 1] = new Point(ConnectionTailPort.PagePosition.X - tailDist, ConnectionTailPort.PagePosition.Y);
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
                        if (PxInterPts.Count > 2)
                        {
                            ConnectorCanRemoveSegmentsEvtArgs newEventArgs = new ConnectorCanRemoveSegmentsEvtArgs(this, false);
                            newEventArgs.RoutedEvent = DiagramView.ConnectorCanRemoveSegmentsEvent;
                            if (dview != null)
                            {
                                this.dview.RaiseEvent(newEventArgs);
                            }
                            if (newEventArgs.Remove)
                            {
                                while (PxInterPts.Count > 2)
                                {
                                    PxInterPts.RemoveAt(PxInterPts.Count - 1);
                                }
                            }
                        }
                        bool isNewPtsAdded = false;
                        if (PxInterPts.Count % 2 != 0)
                        {
                            isNewPtsAdded = true;
                            if (PxInterPts.Count > 2)
                            {
                                PxInterPts.RemoveAt(0);
                            }
                            else
                            {
                                PxInterPts.Add(new Point(0, 0));
                            }
                        }
                        if (HeadDirection == Dock.Right)
                        {
                            if (PxInterPts.Count >= 2)
                            {
                                int last = PxInterPts.Count;
                                if (isHeadRecal || PxInterPts[0].X < HeadNode.RightExt)
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
                                if (isHeadRecal || PxInterPts[0].X > HeadNode.LeftExt)
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
                                if ((isTailRecal && (PxInterPts.Count > 2 || (ConnectionHeadPort.PagePosition.X + headDist < ConnectionTailPort.PagePosition.X + tailDist))) || PxInterPts[last - 1].X < TailNode.RightExt)
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
                                if ((isTailRecal && (PxInterPts.Count > 2 || (ConnectionHeadPort.PagePosition.X - headDist > ConnectionTailPort.PagePosition.X - tailDist))) || PxInterPts[last - 1].X > TailNode.LeftExt)
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
                        if ((ConnectionTailPort.PagePosition.X > HeadNode.LeftExt) && (ConnectionTailPort.PagePosition.X < HeadNode.RightExt))
                        {
                            is4Pts = true;
                        }
                    }
                    else if (HeadDirection == Dock.Top && TailDirection == Dock.Top)
                    {
                        if ((ConnectionTailPort.PagePosition.X > HeadNode.LeftExt) && (ConnectionTailPort.PagePosition.X < HeadNode.RightExt))
                        {
                            is4Pts = true;
                        }
                    }
                    else if (HeadDirection == Dock.Top && TailDirection == Dock.Bottom)
                    {
                        if (HeadNode.TopExt < TailNode.BottomExt)
                        {
                            is4Pts = true;
                        }
                    }
                    else if (HeadDirection == Dock.Bottom && TailDirection == Dock.Top)
                    {
                        if (HeadNode.BottomExt > TailNode.TopExt)
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
                                if (isHeadRecal || PxInterPts[0].Y < HeadNode.BottomExt)
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
                                if (isHeadRecal || PxInterPts[0].Y > HeadNode.TopExt)
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
                                if (isTailRecal || PxInterPts[last - 1].Y < TailNode.BottomExt)
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
                                if (isTailRecal || PxInterPts[last - 1].Y > TailNode.TopExt)
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
                        if (PxInterPts.Count > 2)
                        {
                            ConnectorCanRemoveSegmentsEvtArgs newEventArgs = new ConnectorCanRemoveSegmentsEvtArgs(this, true);
                            newEventArgs.RoutedEvent = DiagramView.ConnectorCanRemoveSegmentsEvent;
                            if (dview != null)
                            {
                                this.dview.RaiseEvent(newEventArgs);
                            }
                            if (newEventArgs.Remove)
                            {
                                while (PxInterPts.Count > 2)
                                {
                                    PxInterPts.RemoveAt(PxInterPts.Count - 1);
                                }
                            }
                        }
                        bool isNewPtsAdded = false;
                        if (HeadDirection == Dock.Bottom)
                        {
                            if (PxInterPts.Count >= 2)
                            {
                                int last = PxInterPts.Count;
                                if (isHeadRecal || PxInterPts[0].Y < HeadNode.BottomExt)
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
                                if (isHeadRecal || PxInterPts[0].Y > HeadNode.TopExt)
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
                                if ((isTailRecal && (PxInterPts.Count > 2 || (ConnectionHeadPort.PagePosition.Y + headDist < ConnectionTailPort.PagePosition.Y + tailDist))) || PxInterPts[last - 1].Y < TailNode.BottomExt)
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
                                if ((isTailRecal && (PxInterPts.Count > 2 || (ConnectionHeadPort.PagePosition.Y + headDist > ConnectionTailPort.PagePosition.Y + tailDist))) || PxInterPts[last - 1].Y > TailNode.TopExt)
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

                    if (PxInterPts.Count == 2)
                    {
                        PxInterPts.RemoveAt(0);
                    }

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
                            if (isHeadRecal || (PxInterPts[0].X > HeadNode.LeftExt) || (isNewPtsAdded && isAddedAtHead) || isReset)
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
                            if (isHeadRecal || (PxInterPts[0].X < HeadNode.RightExt) || (isNewPtsAdded && isAddedAtHead) || isReset)
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
                            if (isHeadRecal || (PxInterPts[0].Y > HeadNode.TopExt) || (isNewPtsAdded && isAddedAtHead) || isReset)
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
                            if (isHeadRecal || (PxInterPts[0].Y < HeadNode.BottomExt) || (isNewPtsAdded && isAddedAtHead) || isReset)
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
                            if (isTailRecal || (PxInterPts[last].X > TailNode.LeftExt) || (isNewPtsAdded && !isAddedAtHead) || isReset)
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
                            if (isTailRecal || (PxInterPts[last].X < TailNode.RightExt) || (isNewPtsAdded && !isAddedAtHead) || isReset)
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
                            if (isTailRecal || (PxInterPts[last].Y > TailNode.TopExt) || (isNewPtsAdded && !isAddedAtHead) || isReset)
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
                            if (isTailRecal || PxInterPts[last].Y < TailNode.BottomExt || (isNewPtsAdded && !isAddedAtHead) || isReset)
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

            //HeadNode.ExtMargin = new Thickness(15);
            //TailNode.ExtMargin = new Thickness(15);

            if (PxInterPts.Count >= 4)
            {
                int last = PxInterPts.Count - 1;
                #region Moveaway second segment
                switch (HeadDirection)
                {
                    case Dock.Left:
                    case Dock.Right:
                        if ((PxInterPts[1].Y > HeadNode.TopExt && PxInterPts[1].Y < HeadNode.BottomExt)
                            && IsOverlapp(HeadNode.BoundsExt, new Line() { X1 = PxInterPts[1].X, Y1 = PxInterPts[1].Y, X2 = PxInterPts[2].X, Y2 = PxInterPts[2].Y }))
                        {
                            double bottomGap = TailNode.TopExt - HeadNode.BottomExt;
                            double topGap = HeadNode.TopExt - TailNode.BottomExt;

                            ////Inner
                            if (topGap > 0 || bottomGap > 0)
                            {
                                ////Go Down
                                if ((topGap > 0 && bottomGap > 0 && topGap > bottomGap) || (topGap < 0))
                                {
                                    PxInterPts[1] = new Point(PxInterPts[1].X, HeadNode.BottomExt);
                                    PxInterPts[2] = new Point(PxInterPts[2].X, HeadNode.BottomExt);
                                }
                                ////Go Top
                                else
                                {
                                    PxInterPts[1] = new Point(PxInterPts[1].X, HeadNode.TopExt);
                                    PxInterPts[2] = new Point(PxInterPts[2].X, HeadNode.TopExt);
                                }
                            }
                            ////Outer
                            else
                            {
                                double top = Math.Abs(PxInterPts[1].Y - Math.Min(HeadNode.TopExt, TailNode.TopExt));
                                double bott = Math.Abs(PxInterPts[1].Y - Math.Max(TailNode.BottomExt, HeadNode.BottomExt));
                                ////Top
                                if (top < bott)
                                {
                                    PxInterPts[1] = new Point(PxInterPts[1].X, Math.Min(HeadNode.TopExt, TailNode.TopExt));
                                    PxInterPts[2] = new Point(PxInterPts[2].X, Math.Min(HeadNode.TopExt, TailNode.TopExt));
                                }
                                ////Bottom
                                else
                                {
                                    PxInterPts[1] = new Point(PxInterPts[1].X, Math.Max(TailNode.BottomExt, HeadNode.BottomExt));
                                    PxInterPts[2] = new Point(PxInterPts[2].X, Math.Max(TailNode.BottomExt, HeadNode.BottomExt));
                                }
                            }
                        }
                        break;
                    case Dock.Top:
                    case Dock.Bottom:

                        if ((PxInterPts[1].X > HeadNode.LeftExt && PxInterPts[1].X < HeadNode.RightExt)
                            && IsOverlapp(HeadNode.BoundsExt, new Line() { X1 = PxInterPts[1].X, Y1 = PxInterPts[1].Y, X2 = PxInterPts[2].X, Y2 = PxInterPts[2].Y }))
                        {
                            double rightGap = TailNode.LeftExt - HeadNode.RightExt;
                            double leftGap = HeadNode.LeftExt - TailNode.RightExt;
                            if (rightGap > 0 || leftGap > 0)
                            {
                                //if (HeadNode.RightExt < TailNode.LeftExt)
                                ////Go Right
                                if ((leftGap > 0 && rightGap > 0 && leftGap > rightGap) || (leftGap < 0))
                                {
                                    PxInterPts[1] = new Point(HeadNode.RightExt, PxInterPts[1].Y);
                                    PxInterPts[2] = new Point(HeadNode.RightExt, PxInterPts[2].Y);
                                }
                                ////Go Left
                                else
                                {
                                    PxInterPts[1] = new Point(HeadNode.LeftExt, PxInterPts[1].Y);
                                    PxInterPts[2] = new Point(HeadNode.LeftExt, PxInterPts[2].Y);
                                }
                            }
                            ////Outer
                            else
                            {
                                double left = Math.Abs(PxInterPts[1].X - Math.Min(HeadNode.LeftExt, TailNode.LeftExt));
                                double right = Math.Abs(PxInterPts[1].X - Math.Max(HeadNode.RightExt, TailNode.RightExt));
                                ////Left
                                if (left < right)
                                {
                                    PxInterPts[1] = new Point(Math.Min(HeadNode.LeftExt, TailNode.LeftExt), PxInterPts[1].Y);
                                    PxInterPts[2] = new Point(Math.Min(HeadNode.LeftExt, TailNode.LeftExt), PxInterPts[2].Y);
                                }
                                ////Right
                                else
                                {
                                    PxInterPts[1] = new Point(Math.Max(HeadNode.RightExt, TailNode.RightExt), PxInterPts[1].Y);
                                    PxInterPts[2] = new Point(Math.Max(HeadNode.RightExt, TailNode.RightExt), PxInterPts[2].Y);
                                }
                            }
                        }
                        break;
                }
                #endregion

                #region Moveaway second segmant from last

                switch (TailDirection)
                {
                    case Dock.Left:
                    case Dock.Right:
                        if ((PxInterPts[last - 1].Y > TailNode.TopExt && PxInterPts[last - 1].Y < TailNode.BottomExt)
                            && IsOverlapp(TailNode.BoundsExt, new Line() { X1 = PxInterPts[last - 1].X, Y1 = PxInterPts[last - 1].Y, X2 = PxInterPts[last - 2].X, Y2 = PxInterPts[2].Y }))
                        {
                            double bottomGap = TailNode.TopExt - HeadNode.BottomExt;
                            double topGap = HeadNode.TopExt - TailNode.BottomExt;
                            if (topGap > 0 || bottomGap > 0)
                            {
                                ////Go Top
                                if ((topGap > 0 && bottomGap > 0 && topGap > bottomGap) || (topGap < 0))
                                //if (HeadNode.BottomExt < TailNode.TopExt)
                                {
                                    PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, TailNode.TopExt);
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, TailNode.TopExt);
                                }
                                ////Go Down
                                else
                                {
                                    PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, TailNode.BottomExt);
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, TailNode.BottomExt);
                                }
                            }
                            /////Outer
                            else
                            {
                                double top = Math.Abs(PxInterPts[last - 1].Y - Math.Min(HeadNode.TopExt, TailNode.TopExt));
                                double bott = Math.Abs(PxInterPts[last - 1].Y - Math.Max(TailNode.BottomExt, HeadNode.BottomExt));
                                ////Top
                                if (top < bott)
                                {
                                    PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, Math.Min(HeadNode.TopExt, TailNode.TopExt));
                                    PxInterPts[last - 2] = new Point(PxInterPts[2].X, Math.Min(HeadNode.TopExt, TailNode.TopExt));
                                }
                                ////Bottom
                                else
                                {
                                    PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, Math.Max(TailNode.BottomExt, HeadNode.BottomExt));
                                    PxInterPts[last - 2] = new Point(PxInterPts[last - 2].X, Math.Max(TailNode.BottomExt, HeadNode.BottomExt));
                                }
                            }
                        }
                        break;
                    case Dock.Top:
                    case Dock.Bottom:
                        last = PxInterPts.Count - 1;
                        if ((PxInterPts[last - 1].X > TailNode.LeftExt && PxInterPts[last - 1].X < TailNode.RightExt)
                            && IsOverlapp(TailNode.BoundsExt, new Line() { X1 = PxInterPts[last - 1].X, Y1 = PxInterPts[last - 1].Y, X2 = PxInterPts[last - 2].X, Y2 = PxInterPts[last - 2].Y }))
                        {
                            double rightGap = TailNode.LeftExt - HeadNode.RightExt;
                            double leftGap = HeadNode.LeftExt - TailNode.RightExt;
                            if (rightGap > 0 || leftGap > 0)
                            {
                                ////Go Left
                                //if (HeadNode.RightExt < TailNode.LeftExt)
                                if ((leftGap > 0 && rightGap > 0 && leftGap > rightGap) || (leftGap < 0))
                                {
                                    PxInterPts[last - 1] = new Point(TailNode.LeftExt, PxInterPts[last - 1].Y);
                                    PxInterPts[last - 2] = new Point(TailNode.LeftExt, PxInterPts[last - 2].Y);
                                }
                                ////Go Right
                                else
                                {
                                    PxInterPts[last - 1] = new Point(TailNode.RightExt, PxInterPts[last - 1].Y);
                                    PxInterPts[last - 2] = new Point(TailNode.RightExt, PxInterPts[last - 2].Y);
                                }
                            }
                            ////Outer
                            else
                            {
                                double left = Math.Abs(PxInterPts[last - 1].X - Math.Min(HeadNode.LeftExt, TailNode.LeftExt));
                                double right = Math.Abs(PxInterPts[last - 1].X - Math.Max(HeadNode.RightExt, TailNode.RightExt));
                                ////Left
                                if (left < right)
                                {
                                    PxInterPts[last - 1] = new Point(Math.Min(HeadNode.LeftExt, TailNode.LeftExt), PxInterPts[last - 1].Y);
                                    PxInterPts[last - 2] = new Point(Math.Min(HeadNode.LeftExt, TailNode.LeftExt), PxInterPts[last - 2].Y);
                                }
                                ////Right
                                else
                                {
                                    PxInterPts[last - 1] = new Point(Math.Max(HeadNode.RightExt, TailNode.RightExt), PxInterPts[last - 1].Y);
                                    PxInterPts[last - 2] = new Point(Math.Max(HeadNode.RightExt, TailNode.RightExt), PxInterPts[last - 2].Y);
                                }
                            }
                        }
                        break;
                }
                #endregion

                #region Moveaway first segment

                HeadNode.ExtMargin = new Thickness(HeadNode.ExtMargin.Bottom - 1);
                TailNode.ExtMargin = new Thickness(TailNode.ExtMargin.Bottom - 1);
                if (IsOverlapp(TailNode.BoundsExt, new Line() { X1 = PxInterPts[0].X, Y1 = PxInterPts[0].Y, X2 = PxInterPts[1].X, Y2 = PxInterPts[1].Y }))
                {
                    switch (HeadDirection)
                    {
                        case Dock.Left:
                            PxInterPts[0] = new Point(TailNode.LeftExt, PxInterPts[0].Y);
                            PxInterPts[1] = new Point(TailNode.LeftExt, PxInterPts[1].Y);
                            break;
                        case Dock.Top:
                            PxInterPts[0] = new Point(PxInterPts[0].X, TailNode.TopExt);
                            PxInterPts[1] = new Point(PxInterPts[1].X, TailNode.TopExt);
                            break;
                        case Dock.Right:
                            PxInterPts[0] = new Point(TailNode.RightExt, PxInterPts[0].Y);
                            PxInterPts[1] = new Point(TailNode.RightExt, PxInterPts[1].Y);
                            break;
                        case Dock.Bottom:
                            PxInterPts[0] = new Point(PxInterPts[0].X, TailNode.BottomExt);
                            PxInterPts[1] = new Point(PxInterPts[1].X, TailNode.BottomExt);
                            break;
                    }
                }
                last = PxInterPts.Count - 1;
                if (IsOverlapp(HeadNode.BoundsExt, new Line() { X1 = PxInterPts[last].X, Y1 = PxInterPts[last].Y, X2 = PxInterPts[last - 1].X, Y2 = PxInterPts[last - 1].Y }))
                {
                    last = PxInterPts.Count - 1;
                    switch (TailDirection)
                    {
                        case Dock.Left:
                            PxInterPts[last] = new Point(HeadNode.LeftExt, PxInterPts[last].Y);
                            PxInterPts[last - 1] = new Point(HeadNode.LeftExt, PxInterPts[last - 1].Y);
                            break;
                        case Dock.Top:
                            PxInterPts[last] = new Point(PxInterPts[last].X, HeadNode.TopExt);
                            PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, HeadNode.TopExt);
                            break;
                        case Dock.Right:
                            PxInterPts[last] = new Point(HeadNode.RightExt, PxInterPts[last].Y);
                            PxInterPts[last - 1] = new Point(HeadNode.RightExt, PxInterPts[last - 1].Y);
                            break;
                        case Dock.Bottom:
                            PxInterPts[last] = new Point(PxInterPts[last].X, HeadNode.BottomExt);
                            PxInterPts[last - 1] = new Point(PxInterPts[last - 1].X, HeadNode.BottomExt);
                            break;
                    }
                }

                HeadNode.ExtMargin = new Thickness(15);
                TailNode.ExtMargin = new Thickness(15);

                #endregion
            }

            IntermediatePoints.Clear();
            foreach (Point pt in PxInterPts)
            {
                IntermediatePoints.Add(pt);
            }
            return new List<Point> { ConnectionHeadPort.PagePosition, ConnectionTailPort.PagePosition };
        }

        private bool IsOverlapp(Rect rect, Line line)
        {
            return rect.IntersectsWith(new Rect(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2)));
        }

        private void fixAdjacent()
        {
            if (IntermediatePoints != null && IntermediatePoints.Count > 2)
            {
                if (HeadNode != null)
                {
                    NodeInfo h;
                    Rect sourceRect = getNodeRect(HeadNode as Node);
                    h = getRectAsNodeInfo(sourceRect);
                    if (IntermediatePoints[1].X == IntermediatePoints[2].X)
                    {
                        IntermediatePoints[0] = new Point(h.Position.X, IntermediatePoints[1].Y);
                    }
                    else if (IntermediatePoints[1].Y == IntermediatePoints[2].Y)
                    {
                        IntermediatePoints[0] = new Point(IntermediatePoints[1].X, h.Position.Y);
                    }
                }
                if (TailNode != null)
                {
                    NodeInfo t;
                    Rect sourceRect = getNodeRect(HeadNode as Node);
                    t = getRectAsNodeInfo(sourceRect);
                    int c = IntermediatePoints.Count - 2;
                    if (IntermediatePoints[c].X == IntermediatePoints[c - 1].X)
                    {
                        IntermediatePoints[c + 1] = new Point(t.Position.X, IntermediatePoints[c].Y);
                    }
                    else if (IntermediatePoints[c].Y == IntermediatePoints[c - 1].Y)
                    {
                        IntermediatePoints[c + 1] = new Point(IntermediatePoints[c].X, t.Position.Y);
                    }
                }
            }
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
            if(MinimumSegementLength!=0)
            {
                if (HeadNode != null && TailNode != null)
                {
                    thirty = 30;
                    fifty = MinimumSegementLength;
                }
                
            }
            List<Point> terms = new List<Point>();

            if (HeadNode != null && TailNode != null)
            {
                NodeInfo head;
                NodeInfo tail;

                Rect h = getNodeRect(HeadNode as Node);
                Rect t = getNodeRect(TailNode as Node);

                head = getRectAsNodeInfo(h,HeadNode as Node);
                tail = getRectAsNodeInfo(t,TailNode as Node);

                Point actualHeadPos = head.Position;
                Point actualTailPos = tail.Position;

                if (dview != null && dview.Page != null)
                {
                    if (this.ConnectionHeadPort != null && HeadNode != null)
                    {
                        head.Position = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y);
                    }
                    if (this.ConnectionTailPort != null && TailNode != null)
                    {
                        tail.Position = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y);
                    }
                }
                ////Top-Bottom
                if ((Math.Abs(actualHeadPos.Y - actualTailPos.Y) > head.Size.Height / 2 + tail.Size.Height / 2 + fifty)
                    && (
                    Orientation == TreeOrientation.TopBottom || Orientation == TreeOrientation.BottomTop))
                {
                    ////Top-to-Bottom
                    double dist = thirty;
                    if (this.FirstSegmentOrientation == SegmentOrientation.Horizontal)
                    {
                        if (head.Position.X < tail.Position.X)
                        {
                            double d = Math.Abs(actualHeadPos.X + head.Size.Width / 2 - (actualTailPos.X - tail.Size.Width / 2));
                            if (d / 2 < thirty)
                            {
                                dist = d / 2;
                            }
                        }
                    }
                    else
                    {
                        //if (head.Position.X < tail.Position.X)
                        {
                            double d = Math.Abs(actualHeadPos.Y + head.Size.Height / 2 - (actualTailPos.Y - tail.Size.Height / 2));
                            if (d / 2 < thirty)
                            {
                                dist = d / 2;
                            }
                        }
                    }
                               
                    if (head.Position.Y - tail.Position.Y < 0)
                    {
                        if (this.FirstSegmentOrientation == SegmentOrientation.Auto)
                            {
                                IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y + head.Size.Height / 2 +dist);
                                IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                            }
                            else if (this.FirstSegmentOrientation == SegmentOrientation.Horizontal)
                            {
                               
                                IntermediatePoints[0] = new Point(actualHeadPos.X + head.Size.Height / 2 + dist, head.Position.Y);
                                IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                            }
                            else if (this.FirstSegmentOrientation == SegmentOrientation.Vertical)
                            {
                                IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y + head.Size.Height / 2 + dist);
                                IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                            }
                    }
                    ////Bottom-to-Top
                    else
                    {
                        if (this.FirstSegmentOrientation == SegmentOrientation.Horizontal)
                    {
                        
                    }
                    else
                    {
                        {
                            double d = Math.Abs(actualHeadPos.Y - head.Size.Height / 2 - (actualTailPos.Y + tail.Size.Height / 2));
                            if (d / 2 < thirty)
                            {
                                dist = d / 2;
                            }
                        }
                    }
                        if (FirstSegmentOrientation == SegmentOrientation.Vertical || FirstSegmentOrientation == SegmentOrientation.Auto)
                        {
                            IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y - head.Size.Height / 2 - dist);
                            IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                        }
                        else
                        {
                            IntermediatePoints[0] = new Point(actualHeadPos.X+head.Size.Width/2+dist, head.Position.Y);
                            IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                        }
                    }
                   
                }
                ////Right-Left
                else
                {
                    ////Left-to-Right
                    if (head.Position.X - tail.Position.X < 0)
                    {
                        double dist=thirty;
                        if (this.FirstSegmentOrientation == SegmentOrientation.Horizontal || this.FirstSegmentOrientation == SegmentOrientation.Auto)
                        {
                            double d = Math.Abs(actualHeadPos.X+head.Size.Width/2 - (actualTailPos.X-tail.Size.Width/2));
                            if(MinimumSegementLength!=0)
                            if (d / 2 < thirty)
                            {
                                dist = d / 2;
                            }
                            IntermediatePoints[0] = new Point(actualHeadPos.X + head.Size.Width / 2 + dist, head.Position.Y);
                            IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                        }

                        else if (this.FirstSegmentOrientation == SegmentOrientation.Vertical)                        
                        {
                            double d = Math.Abs(actualHeadPos.Y+head.Size.Height/2 - (actualTailPos.Y-head.Size.Height/2));
                            if(MinimumSegementLength!=0)
                            if (d / 2 < thirty)
                            {
                                dist = d / 2;
                            }
                            IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y+head.Size.Height/2 + dist);
                            IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                        }
                        if (Math.Abs((actualHeadPos.X + head.Size.Width / 2) - (actualTailPos.X - tail.Size.Width / 2)) < fifty )
                        {
                            if (this.FirstSegmentOrientation == SegmentOrientation.Horizontal || this.FirstSegmentOrientation == SegmentOrientation.Auto)
                            {
                                IntermediatePoints[0] = new Point(
                                                                    actualTailPos.X + tail.Size.Width / 2 + 50, head.Position.Y);
                                IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                            }
                            else if (this.FirstSegmentOrientation == SegmentOrientation.Vertical)
                            {
                                IntermediatePoints[0] = new Point(
                                                               actualTailPos.X + tail.Size.Width / 2 + 50, head.Position.Y+30);
                                IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y+30);
                            }
                        }
                    }
                    ////Right-to-Left
                    else
                    {
                        if (FirstSegmentOrientation == SegmentOrientation.Horizontal || FirstSegmentOrientation == SegmentOrientation.Auto)
                        {
                            IntermediatePoints[0] = new Point(actualHeadPos.X - head.Size.Width / 2 - thirty, head.Position.Y);
                            IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                        }
                        else
                        {
                            IntermediatePoints[0] = new Point(head.Position.X, actualHeadPos.Y + head.Size.Height / 2 +thirty);
                            IntermediatePoints[1] = new Point(tail.Position.X,IntermediatePoints[0].Y);
                        }

                        if (Math.Abs((actualHeadPos.X - head.Size.Width / 2) - (actualTailPos.X + tail.Size.Width / 2)) < fifty
                            || ((head.Position.X - head.Size.Width / 2) - (tail.Position.X + tail.Size.Width / 2) < 0)
                            )
                        {
                            if (FirstSegmentOrientation == SegmentOrientation.Horizontal || FirstSegmentOrientation == SegmentOrientation.Auto)
                            {
                                IntermediatePoints[0] = new Point(actualTailPos.X - tail.Size.Width / 2 - 50, head.Position.Y);
                                IntermediatePoints[1] = new Point(IntermediatePoints[0].X, tail.Position.Y);
                            }
                            else
                            {
                                IntermediatePoints[0] = new Point(head.Position.X, head.Position.Y + head.Size.Height / 2 + 30);
                                IntermediatePoints[1] = new Point(tail.Position.X, IntermediatePoints[0].Y);
                            }
                        }
                    }
                }
            }
            else if (HeadNode != null && TailNode == null)
            {
                NodeInfo head;
                Rect sourceRect = getNodeRect(HeadNode as Node);
                head = getRectAsNodeInfo(sourceRect,HeadNode as Node);
                Point actualHeadPos = head.Position;
                if (dview != null && dview.Page != null)
                {
                    if (this.ConnectionHeadPort != null && HeadNode != null)
                    {
                        head.Position = new Point(ConnectionHeadPort.PagePosition.X, ConnectionHeadPort.PagePosition.Y);
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

                NodeInfo tail;
                Rect sourceRect = getNodeRect(TailNode as Node);
                tail = getRectAsNodeInfo(sourceRect,TailNode as Node);
                Point actualTailPos = tail.Position;
                if (dview != null && dview.Page != null)
                {
                    if (this.ConnectionTailPort != null && TailNode != null)
                    {
                        tail.Position = new Point(ConnectionTailPort.PagePosition.X, ConnectionTailPort.PagePosition.Y);
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

        private void divideOrthognalLine()
        {
            IntermediatePoints[0] = getOrthognalLine();
        }

        internal Point getOrthognalLine()
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
                Rect h = getNodeRect(HeadNode as Node);
                head = getRectAsNodeInfo(h);
            }

            if (TailNode == null)
            {
                tail.Position = PxEndPointPosition;
            }
            else
            {
                Rect t = getNodeRect(TailNode as Node);
                tail = getRectAsNodeInfo(t);
            }

            Point actualHeadPos = head.Position;
            Point actualTailPos = tail.Position;

            if (dview != null && dview.Page != null)
            {
                if (this.ConnectionHeadPort != null && HeadNode != null)
                {
                    head.Position = new Point(ConnectionHeadPort.Left, ConnectionHeadPort.Top);
                    head.Position = (HeadNode as Node).TranslatePoint(head.Position, dview.Page);
                    head.Position = head.Position;
                }
                if (this.ConnectionTailPort != null && TailNode != null)
                {
                    tail.Position = new Point(ConnectionTailPort.Left, ConnectionTailPort.Top);
                    tail.Position = (TailNode as Node).TranslatePoint(tail.Position, dview.Page);
                    tail.Position = tail.Position;
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
            return ret;
        }

        private bool divideOrthoganalLine(IShape HeadNode, IShape TailNode, ConnectionPort ConnectionHeadPort, ConnectionPort ConnectionTailPort, NodeInfo head, NodeInfo tail, Point hor, Point ver)
        {
            bool Horizontal = true;
          
            if (HeadNode != null && ConnectionHeadPort != null)
            {
                if (this.FirstSegmentOrientation == SegmentOrientation.Auto)
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
                else if (this.FirstSegmentOrientation == SegmentOrientation.Horizontal)
                {
                    Horizontal = true;
                }
                else if (this.FirstSegmentOrientation == SegmentOrientation.Vertical)
                {
                    Horizontal = false;
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
                    // Head has Loweroffy
                    if (head.BottomPoint.Y < tail.TopPoint.Y)
                    {
                        if (head.BottomPoint.X < tail.TopPoint.X)
                        {

                            if (this.IntermediatePoints[0].X > head.BottomPoint.X && this.IntermediatePoints[0].X <= tail.TopPoint.X)
                            {
                                Horizontal = true;
                            }
                            else if (this.IntermediatePoints[0].X <= head.BottomPoint.X && this.IntermediatePoints[0].X <= tail.TopPoint.X)
                            {
                                Horizontal = false;
                            }

                        }

                        else if (head.BottomPoint.X > tail.TopPoint.X)
                        {

                            if (this.IntermediatePoints[0].X < head.BottomPoint.X && this.IntermediatePoints[0].X <= tail.TopPoint.X)
                            {
                                Horizontal = true;
                            }
                            else if (this.IntermediatePoints[0].X <= head.BottomPoint.X && this.IntermediatePoints[0].X >= tail.TopPoint.X)
                            {
                                Horizontal = false;
                            }


                        }

                    }
                    ////Head has higher offsety
                    else if (head.TopPoint.Y > tail.BottomPoint.Y)
                    {
                        if (head.TopPoint.X > tail.BottomPoint.X)
                        {

                            if (this.IntermediatePoints[0].X < head.TopPoint.X && this.IntermediatePoints[0].X >= tail.TopPoint.X)
                            {
                                Horizontal = false;
                            }
                            else if (this.IntermediatePoints[0].X <= head.TopPoint.X && this.IntermediatePoints[0].X <= tail.BottomPoint.X)
                            {
                                Horizontal = true;
                            }

                        }
                        else if (head.TopPoint.X < tail.BottomPoint.X)
                        {
                            if (this.IntermediatePoints[0].X < head.RightPoint.X && this.IntermediatePoints[0].X <= tail.LeftPoint.X)
                            {
                                Horizontal = false;
                            }
                            else if (this.IntermediatePoints[0].X >= head.RightPoint.X && this.IntermediatePoints[0].X <= tail.LeftPoint.X)
                            {
                                Horizontal = true;
                            }

                        }
                    }
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

        internal NodeInfo getRectAsNodeInfo(Rect h)
        {
            NodeInfo head = new NodeInfo();
            if (Rect.Empty != h)
            {
                head.Left = h.Left;
                head.Top = h.Top;
                head.Size = h.Size;
                head.Position =  new Point(h.X + h.Width / 2, h.Y + h.Height / 2);
                head.MeasurementUnit = this.MeasurementUnit;
            }
            return head;
        }

         internal NodeInfo getRectAsNodeInfo(Rect h,Node nod)
        {
            NodeInfo head = new NodeInfo();
            if (Rect.Empty != h)
            {
                head.Left = h.Left;
                head.Top = h.Top;
                head.Size = h.Size;
                if (nod.Content is Path)
                {
                    head.Position =  new Point(h.X + h.Width/2, h.Y + h.Height/2);
                }
                else
                {
                    head.Position = nod.Transform(new Point(nod.ActualWidth/2, nod.ActualHeight/2));
                }
                head.MeasurementUnit = this.MeasurementUnit;
            }
            return head;
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

        #endregion

        #region Methods
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
        private BezierSegment GetSegment(double x1, double y1, double x2, double y2, double temp1, double temp2, double num1, bool isTop, bool isBottom, bool isLeft, bool isRight)
        {
            if (isTop)
            {
                return Segment(x1, y1, x2, y2, temp1, temp2 - num1);
            }
            else if (isBottom)
            {
                return Segment(x1, y1, x2, y2, temp1, temp2 + num1);
            }
            else if (isLeft)
            {
                return Segment(x1, y1, x2, y2, temp1 - num1, temp2);
            }
            else
            {
                return Segment(x1, y1, x2, y2, temp1 + num1, temp2);
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
        private BezierSegment Segment(double x1, double y1, double x2, double y2, double temp1, double temp2)
        {
            BezierSegment segment = new BezierSegment
            {
                Point1 = new Point(x1, y1),
                Point2 = new Point(temp1, temp2),
                Point3 = new Point(x2, y2)
            };

            return segment;
        }


        internal void setMinMax()
        {
            if (this.ConnectorPathGeometry != null && this.ConnectorPathGeometry.Bounds != Rect.Empty)
            {
                minx = ConnectorPathGeometry.Bounds.Left;
                miny = ConnectorPathGeometry.Bounds.Top;
                maxx = ConnectorPathGeometry.Bounds.Right;
                maxy = ConnectorPathGeometry.Bounds.Bottom;
            }
            else
            {
                minx = miny = maxx = maxy = 0;
            }
        }

        #endregion

        internal Point GetIntersectionPoint(Point pt, Node n)
        {
            if (n.Boundaries == null || (dview.EnableVirtualization && !n.IsLoaded))
            {
                Point[] corners = new Point[4];
                double x = n.PxOffsetX;
                double y = n.PxOffsetY;
                double w = (double.IsNaN(n.Width) || double.IsInfinity(n.Width)) ? n.ActualWidth : n.Width;
                double h = (double.IsNaN(n.Height) || double.IsInfinity(n.Height)) ? n.ActualHeight : n.Height;
                //Size wh = new Size(w, h);

                corners[0] = new Point(x, y);
                corners[1] = new Point(x + w, y);
                corners[2] = new Point(x + w, y + h);
                corners[3] = new Point(x, y + h);

                Matrix mat = new Matrix();
                mat.RotateAt(n.RotateAngle, x , y );
                MatrixTransform tran = new MatrixTransform(mat);
                for (int i = 0; i < 4; i++)
                {
                    corners[i] = tran.Transform(corners[i]);
                }
                double minLength = double.MaxValue;
                Point intersect = n.PxPosition;
                for (int i = 0; i < 4; i++)
                {
                    Point one = corners[i];
                    Point two = corners[(i + 1) % 4];
                    Point tempIntersect = new Point(0, 0);
                    Point nodeCenter = new Point(x + w / 2, y + h / 2);
                    nodeCenter = tran.Transform(nodeCenter);
                    if (!Double.IsNaN(pt.X) && !Double.IsInfinity(pt.X) && !Double.IsNaN(pt.Y) && !Double.IsInfinity(pt.Y) &&
                        !Double.IsNaN(nodeCenter.X) && !Double.IsInfinity(nodeCenter.X) && !Double.IsNaN(nodeCenter.Y) && !Double.IsInfinity(nodeCenter.Y)
                        && !Double.IsNaN(one.X) && !Double.IsInfinity(one.X) && !Double.IsNaN(one.Y) && !Double.IsInfinity(one.Y)
                        && !Double.IsNaN(two.X) && !Double.IsInfinity(two.X) && !Double.IsNaN(two.Y) && !Double.IsInfinity(two.Y)) 
                    {
                        if (FindPOIBetweenTwoLines(
                                        new Line { X1 = pt.X, Y1 = pt.Y, X2 = nodeCenter.X, Y2 = nodeCenter.Y },
                                        new Line { X1 = one.X, Y1 = one.Y, X2 = two.X, Y2 = two.Y },
                                        ref tempIntersect))
                        {
                            double suspect = (tempIntersect - pt).Length;
                            if (minLength > suspect)
                            {
                                minLength = suspect;
                                intersect = tempIntersect;
                            }
                        } 
                    }
                }
                return intersect;
            }
            bool top = false;
            bool right = false;
            bool bottom = false;
            bool left = false;
            if (dview != null && dview.Page != null)
            {
                List<Point> p = new List<Point>();
                Rect rect = new Rect();
                if (n != null && n.Boundaries != null && n.Boundaries.RenderedGeometry != null)
                {
                    rect = n.Boundaries.RenderedGeometry.Bounds;
                }
                if (rect != Rect.Empty)
                {
                    p.Add(new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2));
                }
                else if (n != null && n.Boundaries != null && (n.Boundaries is Path) && (n.Boundaries as Path).Data != null && (n.Boundaries as Path).Data != Geometry.Empty)
                {
                    rect = (n.Boundaries as Path).Data.Bounds;
                    p.Add(new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2));
                }
                else
                {
                    p.Add(n.PxPosition);
                }

                if (this.ConnectorType == ConnectorType.Orthogonal)
                {
                    NodeInfo ni = getRectAsNodeInfo(rect);
                    FindEndAdjacent(out top, out right, out bottom, out left, ni, pt);
                    if (rect != null && rect != Rect.Empty)
                    {
                        if (top)
                        {
                            pt = new Point(rect.Left + rect.Width / 2, rect.Top - 10);
                        }
                        else if (right)
                        {
                            pt = new Point(rect.Right + 10, rect.Top + rect.Height / 2);
                        }
                        else if (bottom)
                        {
                            pt = new Point(rect.Left + rect.Width / 2, rect.Bottom + 10);
                        }
                        else if (left)
                        {
                            pt = new Point(rect.Left - 10, rect.Top + rect.Height / 2);
                        }
                    }
                }

                PathGeometry pathgeometry = new PathGeometry();
                PathFigure pathfigure = new PathFigure();
                pathfigure.StartPoint = pt;
                pathfigure.Segments.Add(new PolyLineSegment(p, true));
                pathgeometry.Figures.Add(pathfigure);
                Path path1 = new Path();
                path1.Data = pathgeometry;
                Path path2 = new Path();
                Point[] pts = new Point[0];
                if (n.Boundaries is Path)
                {
                    path2 = (n.Boundaries as Path);
                    pts = GetIntersectionPoints(path1.Data, path2.Data);
                }
                else if (n.Boundaries != null && n.Boundaries.RenderedGeometry != null)
                {
                    pts = GetIntersectionPoints(path1.Data, n.Boundaries.RenderedGeometry);
                }
                if (pts != null)
                {
                    Point r = findNearest(pt, pts);
                    if (r.X != 0 && r.Y != 0)
                    {
                        return r;
                    }
                    else
                    {
                        return n.PxPosition;
                    }
                }
                else
                {
                    return n.PxPosition;
                }
            }
            else
                return new Point(0, 0);
        }

        private void FindEndAdjacent(out bool top, out bool right, out bool bottom, out bool left, NodeInfo n, Point pt)
        {
            top = false;
            right = false;
            bottom = false;
            left = false;

            if (IntermediatePoints != null && IntermediatePoints.Count > 0)
            {
                int i = IntermediatePoints.IndexOf(pt);
                if (i == -1)
                {
                    Point[] pts = new Point[2];
                    pts[0] = (IntermediatePoints[0]);
                    pts[1] = (IntermediatePoints[IntermediatePoints.Count - 1]);
                    pt = findNearest(pt, pts);
                    i = IntermediatePoints.IndexOf(pt);
                    if (i == -1)
                    {
                        if (Math.Abs(IntermediatePoints[0].Y - pt.Y) < Math.Abs(IntermediatePoints[IntermediatePoints.Count - 1].Y - pt.Y))
                        {
                            i = 0;
                        }
                        else
                        {
                            i = IntermediatePoints.Count - 1;
                        }
                    }
                }

                if (i == 0 && IntermediatePoints.Count >= 3)
                {
                    if (Math.Abs(IntermediatePoints[0].X - IntermediatePoints[1].X)
                    > Math.Abs(IntermediatePoints[0].Y - IntermediatePoints[1].Y))
                    {
                        if (n.Position.Y > IntermediatePoints[1].Y)
                        {
                            top = true;
                        }
                        else
                        {
                            bottom = true;
                        }
                    }
                    else if (Math.Abs(IntermediatePoints[0].X - IntermediatePoints[1].X)
                    == Math.Abs(IntermediatePoints[0].Y - IntermediatePoints[1].Y))
                    {
                        if (Math.Abs(IntermediatePoints[1].X - IntermediatePoints[2].X)
                    > Math.Abs(IntermediatePoints[1].Y - IntermediatePoints[2].Y))
                        {
                            if (n.Position.X > IntermediatePoints[1].X)
                            {
                                left = true;
                            }
                            else
                            {
                                right = true;
                            }
                        }
                        else
                        {
                            if (n.Position.Y > IntermediatePoints[1].Y)
                            {
                                top = true;
                            }
                            else
                            {
                                bottom = true;
                            }
                        }
                    }
                    else
                    {
                        if (n.Position.X > IntermediatePoints[1].X)
                        {
                            left = true;
                        }
                        else
                        {
                            right = true;
                        }
                    }
                }
                else if (i == IntermediatePoints.Count - 1 && IntermediatePoints.Count >= 3)
                {
                    int last = IntermediatePoints.Count - 1;
                    if (Math.Abs(IntermediatePoints[last].X - IntermediatePoints[last - 1].X)
                    > Math.Abs(IntermediatePoints[last].Y - IntermediatePoints[last - 1].Y))
                    {
                        if (n.Position.Y > IntermediatePoints[last - 1].Y)
                        {
                            top = true;
                        }
                        else
                        {
                            bottom = true;
                        }
                    }
                    else if (Math.Abs(IntermediatePoints[last].X - IntermediatePoints[last - 1].X)
                    == Math.Abs(IntermediatePoints[last].Y - IntermediatePoints[last - 1].Y))
                    {
                        if (Math.Abs(IntermediatePoints[last - 1].X - IntermediatePoints[last - 2].X)
                    > Math.Abs(IntermediatePoints[last - 1].Y - IntermediatePoints[last - 2].Y))
                        {
                            if (n.Position.X > IntermediatePoints[last - 1].X)
                            {
                                left = true;
                            }
                            else
                            {
                                right = true;
                            }
                        }
                        else
                        {
                            if (n.Position.Y > IntermediatePoints[last - 1].Y)
                            {
                                top = true;
                            }
                            else
                            {
                                bottom = true;
                            }
                        }
                    }
                    else
                    {
                        if (n.Position.X > IntermediatePoints[last - 1].X)
                        {
                            left = true;
                        }
                        else
                        {
                            right = true;
                        }
                    }
                }
            }
        }

        private Point findNearest(Point pt, Point[] pts)
        {
            double dist = 1e100d;
            Point ret = new Point();
            foreach (Point p in pts)
            {
                double d = (pt - p).Length;
                if (dist > d)
                {
                    dist = d;
                    ret = p;
                }
            }
            return ret;
        }

        private Point[] GetIntersectionPointsFromWidened(Geometry og1, Geometry og2)
        {
            if (og1 != null && og2 != null)
            {
                CombinedGeometry cg = new CombinedGeometry(GeometryCombineMode.Intersect, og1, og2);

                PathGeometry pg = cg.GetFlattenedPathGeometry();

                return pg.Figures.Select(f => f.StartPoint).ToArray<Point>();
            }
            else
                return null;
        }

        internal Point[] GetIntersectionPoints(Geometry g1, Geometry g2)
        {
            if (g2 != null)
            {
                double ces = PxConnectionEndSpace;
                ScaleTransform t = new ScaleTransform((g2.Bounds.Width + ces) / g2.Bounds.Width,
                                       (g2.Bounds.Height + ces) / g2.Bounds.Height, g2.Bounds.Left + g2.Bounds.Width / 2, g2.Bounds.Top + g2.Bounds.Height / 2);
                PathGeometry pg = Geometry.Combine(g2, g2, GeometryCombineMode.Intersect, t);

                g2 = pg;
            }
            if (g1 != null && g2 != null)
            {
                Geometry og1 = g1.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
                Geometry og2 = g2.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
                CombinedGeometry cg = new CombinedGeometry(GeometryCombineMode.Intersect, og1, og2);

                PathGeometry pg = cg.GetFlattenedPathGeometry();

                Point[] result = new Point[pg.Figures.Count];
                for (int i = 0; i < pg.Figures.Count; i++)
                {
                    Rect fig = new PathGeometry(new PathFigure[] { pg.Figures[i] }).Bounds;
                    result[i] = new Point(fig.Left + fig.Width / 2.0, fig.Top + fig.Height / 2.0);
                }
                return result;
            }
            else
                return null;
        }


        bool load = false;
        internal bool IsInternallyLoaded
        {
            get { return load; }
            set { load = value; }
        }
        internal double PxConnectionEndSpace
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(ConnectionEndSpace, this.MeasurementUnit);
            }
            set
            {
                ConnectionEndSpace = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnit);
            }
        }

        internal Rect GetBounds()
        {
            if (this.ConnectorPathGeometry != null && this.ConnectorPathGeometry.Bounds != Rect.Empty)
            {
                //return this.ConnectorPathGeometry.Bounds;
                LabelEditor labelEditor = GetTemplateChild("PART_ConnectorLabelEditor") as LabelEditor;
                // Label position on LineConnector 
                Point LabelEditorOnLine = labelEditor.TransformToAncestor(this as LineConnector).Transform(new Point(0, 0));

                Rect returnRect = this.ConnectorPathGeometry.Bounds;
                // Checking whether the Label is placed outside of the LineConnector's Bounds
                if (LabelEditorOnLine.X + labelEditor.ActualWidth > this.ConnectorPathGeometry.Bounds.X + this.ConnectorPathGeometry.Bounds.Width)
                {
                    // Calculating width of the returnRect (starting point of LineConnector to endpoint of LabelEditor), if Label is placed outside of the LineConnector bounds in right
                    returnRect.Width = LabelEditorOnLine.X + labelEditor.ActualWidth - returnRect.X;
                }
                else if (LabelEditorOnLine.X < this.ConnectorPathGeometry.Bounds.X)
                {
                    // Set returnRect.X value if the Label is placed in outside of the LineConnector bounds in Left 
                    returnRect.X = LabelEditorOnLine.X;
                }

                if (LabelEditorOnLine.Y + labelEditor.ActualHeight > this.ConnectorPathGeometry.Bounds.Y + this.ConnectorPathGeometry.Bounds.Height)
                {
                    returnRect.Height = LabelEditorOnLine.Y - returnRect.Y + labelEditor.ActualHeight;
                }
                else if (LabelEditorOnLine.Y < this.ConnectorPathGeometry.Bounds.Y)
                {
                    returnRect.Y = LabelEditorOnLine.Y;
                }

                return returnRect;
            }
            else
                if (dc != null && dc.View!=null)
                {
                    if (!dc.View.IsDragged && (this.TailNode != null || this.HeadNode != null))
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
                }
                return Rect.Empty;
        }
    }
}
