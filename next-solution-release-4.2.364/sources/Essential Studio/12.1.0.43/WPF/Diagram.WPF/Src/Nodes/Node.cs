// <copyright file="Node.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media.Effects;
using System.Reflection;

namespace Syncfusion.Windows.Diagram
{

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
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class Node : ContentControl, IShape, INodeGroup, INotifyPropertyChanged, ICommon
    {

        internal bool m_LayoutDisconnected;
        
        internal VisualBrush Brush
        {
            get
            {
                _VisualBrush = _VisualBrush ?? PrepareClone();
                return _VisualBrush;
            }
        }

        internal Point NodeTransform(Point pt)
        {
            Matrix mat = Matrix.Identity;
            mat.RotateAt(this.RotateAngle, PxOffsetX, PxOffsetY);
            return mat.Transform(new Point(PxOffsetX + pt.X, PxOffsetY + pt.Y));
        }

        internal Point NodeTransformX()
        {
            Matrix mat = Matrix.Identity;
            mat.RotateAt(this.RotateAngle, PxOffsetX, PxOffsetY);
            return mat.Transform(new Point(PxOffsetX , PxOffsetY));
        }
        internal Point NodeTransformY()
        {
            Matrix mat = Matrix.Identity;
            mat.RotateAt(this.RotateAngle, PxOffsetX, PxOffsetY);
            return mat.Transform(new Point(PxOffsetX + 0, PxOffsetY));
        }
        internal Point NodeTransformWidth()
        {
            Matrix mat = Matrix.Identity;
            mat.RotateAt(this.RotateAngle, PxOffsetX, PxOffsetY);
            return mat.Transform(new Point(PxOffsetX + ActualWidth, PxOffsetY +0));
        }
        internal Point NodeTransformHeight()
        {
            Matrix mat = Matrix.Identity;
            mat.RotateAt(this.RotateAngle, PxOffsetX, PxOffsetY);
            return mat.Transform(new Point(PxOffsetX +0, PxOffsetY +ActualHeight));
        }
        private VisualBrush _VisualBrush;

        private VisualBrush PrepareClone()
        {
            VisualBrush brush = new VisualBrush(this);
            brush.Viewbox = new Rect(PxOffsetX, PxOffsetY, this.ActualWidth, this.ActualHeight);
            brush.Stretch = Stretch.Uniform;
            brush.ViewboxUnits = BrushMappingMode.Absolute;
            this.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == "Position")
                    {
                        brush.Viewbox = new Rect(PxOffsetX, PxOffsetY, this.ActualWidth, this.ActualHeight);
                    }
                };
            this.SizeChanged +=
                (s, e) =>
                {
                    brush.Viewbox = new Rect(PxOffsetX, PxOffsetY, this.ActualWidth, this.ActualHeight);
                };
            return brush;
        }

        #region Class variables

        internal Size m_TempSize;
        internal bool m_MouseMoving = false;
        internal bool m_MouseResizing = false;
        internal Point m_TempPosition;
        internal Point m_Delta;
        internal double m_ResizeHorDelta;
        internal double m_ResizeVertDelta;
        internal static bool rotatebycode = false;

        internal bool subTreeReVal = true;

        internal double segmentoffset;

        internal double tempx;

        internal double tempy;

        internal double RSize { get { return Math.Max(Width, Height); } }

        internal Node RParent;

        internal int Stage;
        
        internal bool currentdragging = false;

        internal bool Visited;

        internal bool BeforeConnection = false;

        internal bool onceResized;
        internal int tempcount;
        private bool IsFirstLoaded = false;
        private List<string> header = new List<string>();
        ContextMenu nodecontextmenu;

        /// <summary>
        /// Used to store Diagram Control object
        /// </summary>
        internal DiagramControl dc;

        /// <summary>
        /// used to store the groups.
        /// </summary>
        private CollectionExt m_groups = new CollectionExt();

        /// <summary>
        /// Used to store the rank
        /// </summary>
        private int m_rank = -1;

        /// <summary>
        /// Used to store the count of nodes.
        /// </summary>
        private int no = -1;

        /// <summary>
        /// Used to store the start point.
        /// </summary>
        private System.Windows.Point? startPoint = null;
                
        /// <summary>
        /// Used to store the Guid.
        /// </summary>
        private Guid m_id;

        /// <summary>
        /// Used to store the full name of the node.
        /// </summary>
        private string m_fullName;

        /// <summary>
        /// Used to store the in edges
        /// </summary>
        private CollectionExt m_inEdges = new CollectionExt();

        /// <summary>
        /// Used to store the out edges.
        /// </summary>
        private CollectionExt m_outEdges = new CollectionExt();

        /// <summary>
        /// Used to store the edges
        /// </summary>
        private CollectionExt m_edges = new CollectionExt();
        
        /// <summary>
        /// Used to store the parents
        /// </summary>
        private CollectionExt mParents = new CollectionExt();


        /// <summary>
        /// Used to store the ports.
        /// </summary>
        private ObservableCollection<ConnectionPort> m_ports = new ObservableCollection<ConnectionPort>();

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
        private CollectionExt treeChildren = null;

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
        private System.Drawing.Rectangle mRectangle = new System.Drawing.Rectangle(0, 0, 100, 70);

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

        private DiagramControl MdiagramControl;

        /// <summary>
        /// Used to store the View.
        /// </summary>
        private DiagramView dview;

        /// <summary>
        /// Used to store the execution check.
        /// </summary>
        private bool ex = false;

        /// <summary>
        /// Used to store the mouse up state
        /// </summary>
        internal static bool mouseup = true;
        /// <summary>
        ///  Used to store the mouse down state
        /// </summary>

        internal bool mousedown = false;
        /// <summary>
        /// Used to store the last node click instance
        /// </summary>
        private DateTime lastNodeClick;

        /// <summary>
        /// Used to store the last node click point
        /// </summary>
        private Point lastNodePoint;

        /// <summary>
        /// Used to store the editor
        /// </summary>
        private LabelEditor editor;

        /// <summary>
        /// Used to store the resize node property setting.
        /// </summary>
        private bool resizenode = false;

        /// <summary>
        /// Used to store the rotate thumb
        /// </summary>
        private Control rotatethumb;

        /// <summary>
        /// Used to store the Group's rotate thumb
        /// </summary>
        private Control grouprotatethumb;

        /// <summary>
        /// Used to store the source port.
        /// </summary>
        private ConnectionPort sourceHitPort;

        /// <summary>
        /// Used to store the port no.
        /// </summary>
        private int pno = 1;

        /// <summary>
        /// Used to store the old ZIndex
        /// </summary>
        private int m_oldindex = 0;

        /// <summary>
        /// Used to store the new ZIndex
        /// </summary>
        private int m_newindex = 0;

        /// <summary>
        /// Used to store the old offset position.
        /// </summary>
        private Point oldoff = new Point();

        /// <summary>
        /// Used to store the state of the node in case of cycle detection.
        /// </summary>
        private int state = 0;

        /// <summary>
        /// Used to check if this node is to be connected o its parent or not.
        /// </summary>
        private bool canconn = true;

        private bool NodeVirtual = true;

        internal bool m_NodeDrawing = false;
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
        /// Used to Store the Whether the Node is Resized or not.
        /// </summary>
        internal bool m_IsResizing = false;

        /// <summary>
        /// Used to store the old offsetx while undo/redo.
        /// </summary>
        private double oldx = 0;

        /// <summary>
        /// Used to store the old offsety while undo/redo.
        /// </summary>
        private double oldy = 0;

        /// <summary>
        /// Used to check if oldx and oldy are set once.
        /// </summary>
        private bool exeonce = false;

        internal bool _sizeCommandsApplied = false;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="Node"/> class.
        /// </summary>
        static Node()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Node), new FrameworkPropertyMetadata(typeof(Node)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="id">The Guid id.</param>
        /// <param name="name">The node name.</param>
        public Node(Guid id, string name)
        {
            this.RenderTransform = new RotateTransform();
            this.m_id = id;
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
            //this.LayoutUpdated += new EventHandler(Node_LayoutUpdated);
            this.Loaded += new RoutedEventHandler(Node_Loaded);
            this.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Node_MouseLeftButtonUp), true);
            this.AddHandler(Control.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(Node_MouseLeftButtonUp2), true);

            this.Unloaded += new RoutedEventHandler(Node_Unloaded);
            this.SizeChanged += new SizeChangedEventHandler(Node_SizeChanged);
            Boundaries = new Path();
            this.Loaded += new RoutedEventHandler(First_Load);
        }

        private void Node_MouseLeftButtonUp2(object sender, MouseButtonEventArgs e)
        {
 
        }

        private bool _FirstLoaded = false;

        void First_Load(object sender, RoutedEventArgs e)
        {
            _FirstLoaded = true;
            if (centerport != null)
            {
                centerport.Left = this.Width / 2;
                centerport.Top = this.Height / 2;
            }
            this.Width = MeasureUnitsConverter.ToPixels(this.Width, this.MeasurementUnits);
            this.Height = MeasureUnitsConverter.ToPixels(this.Height, this.MeasurementUnits);
            
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="id">The Guid id.</param>
        public Node(Guid id)
            : this(id, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
            : this(Guid.NewGuid())
        {
        }

        internal Thickness TransformRectBounds()
        {
           
            var rectpoints=new List<Point>
                               {
                                   Transform(new Point(_Width, 0)),
                                   Transform(new Point(0, _Height)),
                                   Transform(new Point(_Width, _Height)),
                                   Transform(new Point(0, 0))
                               };
            //var maxX = from rectpoint in rectpoints select rectpoint.X;
            //var maxY = from rectpoint in rectpoints select rectpoint.Y;
            //double maxWidth = maxX.Max();
            //double maxHeight = maxY.Max();

            LabelEditor labelEditor = GetTemplateChild("PART_LabelEditor") as LabelEditor;

            double maxWidth;
            double maxHeight;
            double diffWidth = 0.0;
            double diffYHeight = 0.0;
            double minX = 0.0;
            double minY = 0.0;
            if (labelEditor != null && !String.IsNullOrEmpty(labelEditor.Label))
            {
                // Label position on Node
                Point LabelEditorOnNode = labelEditor.TransformToAncestor(this as Node).Transform(new Point(0, 0));

                // Checking whether the Label is placed outside of the Node
                if (LabelEditorOnNode.X + labelEditor.ActualWidth > (this as Node).Width && LabelEditorOnNode.X >= 0)
                {
                    // Calculating the distance from Node's endpoint to Label's endpoint if the Label is placed in outside of the Node in right 
                    diffWidth = (LabelEditorOnNode.X + labelEditor.ActualWidth) - (this as Node).Width;
                }
                else if (LabelEditorOnNode.X < 0)
                {
                    // Set minX value if the Label is placed in outside of the Node in Left 
                    minX = LabelEditorOnNode.X;
                }

                if (LabelEditorOnNode.Y + labelEditor.ActualHeight > (this as Node).Height && LabelEditorOnNode.Y >= 0)
                {
                    diffYHeight = (LabelEditorOnNode.Y + labelEditor.ActualHeight) - (this as Node).Height;
                }
                else if (LabelEditorOnNode.Y < 0)
                {
                    minY = LabelEditorOnNode.Y;

                }
                if (this.dc != null)
                {
                    (this as Node).dc.View.InvalidateViewGrid();
                }
            }

            var maxX = from rectpoint in rectpoints select rectpoint.X;
            var maxY = from rectpoint in rectpoints select rectpoint.Y;
            maxWidth = maxX.Max();
            maxHeight = maxY.Max();

            if (maxWidth < 0)
            {
                 maxWidth = 0;
            }
            if (maxHeight < 0)
            {
                 maxHeight = 0;
            }

            //return new Thickness(maxX.Min(), maxY.Min(), maxWidth, maxHeight);
            return new Thickness(maxX.Min() + minX, maxY.Min() + minY, maxWidth + diffWidth, maxHeight + diffYHeight);
        }

        /// <summary>
        /// Handles the SizeChanged event of the Node control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Node n = sender as Node;
            n.RectBounds = n.TransformRectBounds();
            n.Dispatcher.BeginInvoke(new Action(n.Node_LayoutUpdated), null);

            if (n.dview != null && n.dview.DateTimeSettings.IsEnabled)
            {
                n.DurationX = n.Width.ToTimeSpan(n.dview.DateTimeSettings);
                n.DurationY = n.Height.ToTimeSpan(n.dview.DateTimeSettings);
            }
            if (dc != null && !(sender is Layer) && this.IsFirstLoaded && !this._sizeCommandsApplied)
            {
                if (!dc.View.undostak.Contains(this.Name))
                {
                    if (!dc.View.IsResized && (!dc.View.IsResizedUndone || !this.onceResized))
                    {
                        foreach (ConnectionPort port in this.Ports)
                        {
                            if (!dc.View.IsResizedRedone)
                            {
                                port.PreviousPortPoint = new Point(port.Left, port.Top);
                                port.Left = (n.Width / e.PreviousSize.Width) * port.PreviousPortPoint.X;
                                port.Top = (n.Height / e.PreviousSize.Height) * port.PreviousPortPoint.Y;
                                TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                port.RenderTransform = tr;
                            }
                            if (dview != null && dview.UndoRedoEnabled && !(this is Group))
                            {
                                dview.UndoStack.Push(port);
                                dview.UndoStack.Push(port.PreviousPortPoint);
                            }
                        }
                        if (dview != null && dview.UndoRedoEnabled && !(this is Group))
                        {
                            dview.UndoStack.Push(dview.DragDelta.ToString());
                            dview.UndoStack.Push(this);
                            dc.View.UndoStack.Push(e.PreviousSize);
                            dview.UndoStack.Push(new Point(oldx, oldy));
                            //dview.UndoStack.Push(dview.SelectionList.Count);
                            dview.UndoStack.Push(dview.SelectionList.OfType<Node>().Count<Node>());
                            dview.UndoStack.Push("Resized");
                            dview.DragDelta = "No";
                        }
                        dc.View.IsResizedRedone = false;
                    }
                }
                else
                {
                    if (dc.View.undostak.Count <= 1)
                    {
                        dc.View.IsResizedUndone = false;
                    }
                    dc.View.undostak.Remove(Name);
                }

            }
            if (this._sizeCommandsApplied)
                this._sizeCommandsApplied = false;
        }

        internal ConnectionPort centerport;

        private DiagramPage GetParent(DependencyObject dep)
        {
            DependencyObject obj = VisualTreeHelper.GetParent(dep);
            if (obj == null)
            {
                return null;
            }
            else if (obj is DiagramPage)
            {
                return obj as DiagramPage;
            }
            else
            {
                return GetParent(obj);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            RectBounds = TransformRectBounds();
            return base.MeasureOverride(constraint);
        }
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (this.Content is FrameworkElement)
            {
                (this.Content as FrameworkElement).IsHitTestVisible = false;
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
                        centerport.Left = this.Width / 2;
                        centerport.Top = this.Height / 2;
                    }
                }
                else
                {
                    double two0ne = 25;// MeasureUnitsConverter.FromPixels(25, this.MeasurementUnits);
                    centerport.Left = two0ne;
                    centerport.Top = two0ne;
                }

                if (this.Ports != null)
                {
                    this.Ports.Add(centerport);
                }

                centerport.CenterPortReferenceNo = 0;
            }
        }

        /// <summary>
        /// Calls Node_Loaded method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void Node_Loaded(object sender, RoutedEventArgs e)
        {
            Binding pagebinding = new Binding("IsPageEditable");
            pagebinding.Source = dview;
           // this.PortVisibility = PortVisibility.AlwaysHidden
            this.SetBinding(Node.PageEditableIndicatorProperty, pagebinding);
            //Binding portvisibilitybinding = new Binding("PortVisibility");
            //portvisibilitybinding.Mode = BindingMode.OneWay;
            //portvisibilitybinding.Source = dview;
            //this.SetBinding(Node.PortVisibilityProperty, portvisibilitybinding);
            if (Page == null)
            {
                Page = GetParent(this);
            }

            if (Nodediagramcontrol == null)
            {
                Nodediagramcontrol = DiagramPage.GetDiagramControl((sender as Node));

            }
            if (_rotAngle != 0)
            {
                this.RotateAngle = _rotAngle;
                _rotAngle = 0;
                rotatebycode = true;
            }
            this.IsFirstLoaded = true;
            if (this.IsSelected)
            {
                if (dview != null)
                {
                    dview.SelectionList.Add(this);
                }
            }
            try
            {
                this.MouseLeftButtonUp += new MouseButtonEventHandler(Node_MouseLeftButtonUp);
                dview = GetDiagramView(this);
                dc = DiagramPage.GetDiagramControl(this);
                if (!exeonce)
                {
                    oldx = (double)this.PxOffsetX;//MeasureUnitsConverter.ToPixels((double)this.LogicalOffsetX, (dview.Page as DiagramPage).MeasurementUnits);
                    oldy = (double)this.PxOffsetY;//MeasureUnitsConverter.ToPixels((double)this.LogicalOffsetY, (dview.Page as DiagramPage).MeasurementUnits);
                    exeonce = true;
                }

                Matrix mat = Matrix.Identity;
                mat.Rotate(this.RotateAngle);
                MatrixTransform trans = new MatrixTransform(mat);
                Point delta = trans.Transform(new Point(this.Width / 2, this.Height / 2));
                this.PxPosition = new Point(this.PxOffsetX + delta.X, this.PxOffsetY + delta.Y);

                if (!dview.IsPageEditable)
                {
                    DiagramView.PageEdit = false;
                    IsLabelEditable = false;
                }

                if (this.Content is Viewbox)
                {
                    (this.Content as Viewbox).Stretch = Stretch.Fill;
                    (this.Content as Viewbox).Width = this.Width;
                    (this.Content as Viewbox).Height = this.Height;
                }
                foreach (ConnectionPort cport in this.Ports)
                {
                    if (cport.PortReferenceNo < 1)
                    {
                        cport.PortReferenceNo = this.countpno + pno;
                        pno++;
                    }
                }
                int i = 0;
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
                    if (cport.CenterPortReferenceNo == 0&&i>1)
                    {
                        this.Ports.Remove(cport);
                        //break;
                    }
                }
                //foreach (ConnectionPort cport in this.Ports)
                //{
                //    if (cport.PortReferenceNo < 1)
                //    {
                //        cport.PortReferenceNo = (cport.Node.ReferenceNo * 10) + pno;
                //        pno++;
                //    }
                //}
            }
            catch
            {
            }

            if (this.m_NodeDrawing)
            {
                this.m_NodeDrawing = false;
                dc.View.Page.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the Node control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Node_Unloaded(object sender, RoutedEventArgs e)
        {
            if (dview != null && dview.Page != null)
            {
                //if (dview.Scrollviewer != null && !(dview.Page as DiagramPage).IsDiagrampageLoaded)
                //{
                //    if (dview.IsMouseScrolled)
                //    {
                //        //if ((dview.Page as DiagramPage).Minleft >= 0)
                //        //{
                //        //    dview.Scrollviewer.ScrollToHorizontalOffset(Math.Abs((dview.Page as DiagramPage).HorValue));
                //        //}
                //        //else
                //        //{
                //        //    dview.Scrollviewer.ScrollToHorizontalOffset(Math.Abs((dview.Page as DiagramPage).HorValue * dview.CurrentZoom));
                //        //}

                //        //if ((dview.Page as DiagramPage).MinTop >= 0)
                //        //{
                //        //    dview.Scrollviewer.ScrollToVerticalOffset(Math.Abs((dview.Page as DiagramPage).VerValue));
                //        //}
                //        //else
                //        //{
                //        //    dview.Scrollviewer.ScrollToVerticalOffset(Math.Abs((dview.Page as DiagramPage).VerValue * dview.CurrentZoom));
                //        //}
                //    }
                //    else
                //    {
                //        //dview.Scrollviewer.ScrollToHorizontalOffset(Math.Abs((dview.Page as DiagramPage).HorValue * dview.CurrentZoom));
                //        //dview.Scrollviewer.ScrollToVerticalOffset(Math.Abs((dview.Page as DiagramPage).VerValue * dview.CurrentZoom));
                //    }
                //}
            }
        }

        #endregion

        #region Properties
        
        public DateTime StartDateX
        {
            get { return (DateTime)GetValue(StartDateXProperty); }
            set { SetValue(StartDateXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartDateX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartDateXProperty =
            DependencyProperty.Register("StartDateX", typeof(DateTime), typeof(Node), new UIPropertyMetadata(new DateTime(2000,1,1), OnStartDateXChanged));

        private static void OnStartDateXChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Node n = d as Node;
            if (n.dview != null && n.dview.DateTimeSettings.IsEnabled)
            {
                n.PxOffsetX = n.StartDateX.ToPixel(n.dview.DateTimeSettings);
            }
        }

        public DateTime StartDateY
        {
            get { return (DateTime)GetValue(StartDateYProperty); }
            set { SetValue(StartDateYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartDateY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartDateYProperty =
            DependencyProperty.Register("StartDateY", typeof(DateTime), typeof(Node), new UIPropertyMetadata(new DateTime(2000, 1, 1), OnStartDateYChanged));

        private static void OnStartDateYChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Node n = d as Node;
            if (n.dview != null && n.dview.DateTimeSettings.IsEnabled)
            {
                n.PxOffsetY = n.StartDateY.ToPixel(n.dview.DateTimeSettings);
            }
        }

        public TimeSpan DurationX
        {
            get { return (TimeSpan)GetValue(DurationXProperty); }
            set { SetValue(DurationXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DurationX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationXProperty =
            DependencyProperty.Register("DurationX", typeof(TimeSpan), typeof(Node), new UIPropertyMetadata(new TimeSpan(), OnDurationXChanged));

        private static void OnDurationXChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Node n = d as Node;
            if (n.dview != null && n.dview.DateTimeSettings.IsEnabled)
            {
                n.Width = n.DurationX.ToPixel(n.dview.DateTimeSettings);
            }
        }

        public TimeSpan DurationY
        {
            get { return (TimeSpan)GetValue(DurationYProperty); }
            set { SetValue(DurationYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DurationY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationYProperty =
            DependencyProperty.Register("DurationY", typeof(TimeSpan), typeof(Node), new UIPropertyMetadata(new TimeSpan(), OnDurationYChanged));
        
        private static void OnDurationYChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Node n = d as Node;
            if (n.dview != null && n.dview.DateTimeSettings.IsEnabled)
            {
                n.Height = n.DurationY.ToPixel(n.dview.DateTimeSettings);
            }
        }

        //internal bool DragCancel
        //{
        //    get;
        //    set;
        //}

        public TextDecorationCollection LabelTextDecorations
        {
            get { return (TextDecorationCollection)GetValue(LabelTextDecorationsProperty); }
            set { SetValue(LabelTextDecorationsProperty, value); }
        }

        //internal double LeftExt
        //{
        //    get { return this.PxOffsetX; }// MeasureUnitsConverter.ToPixels(this.LogicalOffsetX, this.MeasurementUnits); }
        //}

        //internal double TopExt
        //{
        //    get { return this.PxOffsetY; }// MeasureUnitsConverter.ToPixels(this.LogicalOffsetY, this.MeasurementUnits); }
        //}

        //internal double RightExt
        //{
        //    get { return this.LeftExt + this.ActualWidth; }
        //}

        //internal double BottomExt
        //{
        //    get { return this.TopExt + this.ActualHeight; }
        //}

        /// <summary>
        /// Gets or sets the boundaries.
        /// </summary>
        /// <value>The boundaries.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Shape Boundaries
        {
            get
            {
                return (Shape)GetValue(BoundariesProperty);
            }
            set
            {
                SetValue(BoundariesProperty, value);
            }
        }

        public IntersectionMode IntersectionMode
        {
            get
            {
                return (IntersectionMode)GetValue(IntersectionModeProperty);
            }
            set
            {
                SetValue(IntersectionModeProperty, value);
            }
        }
        internal double _rotAngle;
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
                if (this.IsLoaded)
                    SetValue(RotateAngleProperty, value);
                else
                    _rotAngle = value;
                
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
            get { return hittest; }
            set { hittest = value; }
        }

        /// <summary>
        /// Gets or sets the old size.
        /// </summary>
        /// <value>The old size.</value>
        internal Size Oldsize
        {
            get { return osize; }
            set { osize = value; }
        }

        /// <summary>
        /// Gets the groups to which the INodeGroup objects belong.
        /// </summary>
        /// <value>The groups.</value>
        public CollectionExt Groups
        {
            get
            {
                return m_groups;
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
        /// <seealso cref="Gripper"/>
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
        /// <seealso cref="Gripper"/>
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


        internal Thickness RectBounds
        {
            get
            {
                return (Thickness)GetValue(RectBoundsProperty);
            }

            set
            {
                SetValue(RectBoundsProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [allow delete].
        /// </summary>
        /// <value><c>true</c> if [allow delete]; otherwise, <c>false</c>.</value>
        public bool AllowDelete
        {
            get
            {
                return (bool)GetValue(AllowDeleteProperty);
            }
            set
            {
                SetValue(AllowDeleteProperty, value);
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
            get { return resizenode; }
            set { resizenode = value; }
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
            get { return no; }
            set { no = value; }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// Type: <see cref="Panel"/>
        /// Panel instance.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel Page
        {
            get { return mPage; }
            set { mPage = value; }
        }

        internal DiagramControl Nodediagramcontrol
        {
            get { return MdiagramControl; }
            set { MdiagramControl = value; }
        }

        /*
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
        */

        /// <summary>
        /// Gets or sets a value indicating whether the node is treat as obstacle or not.
        /// Default value is true.
        /// </summary>
        public bool TreatAsObstacle
        {
            get { return (bool)GetValue(TreatAsObstacleProperty); }
            set { SetValue(TreatAsObstacleProperty, value); }
        }

        public bool IsLabelEditable
        {
            get { return (bool)GetValue(IsLabelEditableProperty); }
            set { SetValue(IsLabelEditableProperty, value); }
        }

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



        public bool EnableDragthroughLabel
        {
            get { return (bool)GetValue(EnableDragthroughLabelProperty); }
            set { SetValue(EnableDragthroughLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableDragthroughLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableDragthroughLabelProperty =
            DependencyProperty.Register("EnableDragthroughLabel", typeof(bool), typeof(Node), new PropertyMetadata(true));

        
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
                if (dview == null)
                {
                    dview = GetDiagramView(this);
                }
                if (dview != null)
                {
                    if (value)
                    {
                        dview._IsDragoverConnection = true;
                    }
                    else
                    {
                        dview._IsDragoverConnection = false;
                    }
                }
                SetValue(IsDragConnectionOverProperty, value);
            }
        }


        internal bool IsNodeFound
        {
            get { return (bool)GetValue(IsNodeFoundProperty); }
            set { SetValue(IsNodeFoundProperty, value); }
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
        /// Gets or sets the Center Position  of the Node.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// Center Point.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            private set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(Node));
        
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
        /// Gets the information about DiagramView's IsPageEditable property.
        /// </summary>
        /// <returns>Node bool value</returns>
        /// <value>
        /// Type: <see cref="PageEditableIndicator"/>
        /// </value>
        internal bool PageEditableIndicator
        {
            get
            {
                return (bool)GetValue(PageEditableIndicatorProperty);
            }

            set
            {
                SetValue(PageEditableIndicatorProperty, value);
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
            info.Position = pos;
            info.MeasurementUnit = this.MeasurementUnits;
            return info;
        }

        internal NodeInfo GetExtInfo(Thickness margin)
        {
            NodeInfo info = new NodeInfo();
            info.Left = PxOffsetX;
            info.Top = PxOffsetY; 
            if (this.IsLoaded)
            {
                info.Right = PxOffsetX + this.ActualWidth;
                info.Bottom = PxOffsetY + this.ActualHeight;
                info.Size = new Size(ActualWidth, ActualHeight);
            }
            else
            {
                info.Right = PxOffsetX + this._Width;
                info.Bottom = PxOffsetY + this._Height;
                info.Size = new Size(_Width, _Width);
            }
            info.ExtMargin = margin;
            info.Position = PxPosition;
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
            get { return m_ports; }
            set { m_ports = value; }
        }

        /// <summary>
        /// Gets or sets the port visibility.
        /// </summary>
        /// <value>The port visibility.</value>
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
        internal double LabelHeight
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
        /// Gets or sets the label text trimming.
        /// </summary>
        /// <value>
        /// Type: <see cref="TextTrimming"/>
        /// By default it is set to CharacterEllipsis.
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
        /// n.LabelTextTrimming = TextTrimming.None;
        /// Model.Nodes.Add(n);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public TextTrimming LabelTextTrimming
        {
            get
            {
                return (TextTrimming)GetValue(LabelTextTrimmingProperty);
            }

            set
            {
                SetValue(LabelTextTrimmingProperty, value);
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
        public TextAlignment LabelTextAlignment
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
        /// Gets or sets a value indicating whether [enable multiline label].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable multiline label]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMultilineLabel
        {
            get
            {
                return (bool)GetValue(EnableMultilineLabelProperty);
            }
            set
            {
                SetValue(EnableMultilineLabelProperty, value);
            }
        }
       
        /// <summary>
        /// Gets or sets the state of the node in case of cycle detection.
        /// </summary>
        /// <value>The state of the node. (0-->non-visited; 1-->Visited and InProgress; 2-->Done)</value>
        internal int State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node can be connected to the other specified node in case of hierarchical-tree layout.
        /// </summary>
        /// <value>
        /// <c>true</c> if can connect; otherwise, <c>false</c>.
        /// </value>
        internal bool CanConnect
        {
            get { return canconn; }
            set { canconn = value; }
        }

        public bool AllowVirtualization
        {
            get { return NodeVirtual; }
            set { NodeVirtual = value; }

        }

        /// <summary>
        /// Gets or sets the width of the node in pixels.
        /// </summary>
        /// <value>The width of the node.</value>
        internal double PixelWidth
        {
            get { return pwidth; }
            set { pwidth = value; }
        }

        /// <summary>
        /// Gets or sets the height of the node in pixels.
        /// </summary>
        /// <value>The height of the node.</value>
        internal double PixelHeight
        {
            get { return pheight; }
            set { pheight = value; }
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
        /// <summary>
        ///  Apply effect for Node without using Framework Effect property.
        ///  Issue Details:
        ///        When an effect is applied to a Node, printing a diagram page results in clipping of Node. This issue seems to exist in printing in framework level.
        ///  Solution: When an effect is applied to one of content in Node’s Template this issue is not reproduced.
        /// </summary>
       
         public Effect CustomEffect
        {
            get { return (Effect)GetValue(CustomEffectProperty); }
            set
            {
                SetValue(CustomEffectProperty, value);
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
        /// Identifies the CustomEffect dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomEffectProperty =DependencyProperty.Register("CustomEffect", typeof(Effect), typeof(Node));

        public static readonly DependencyProperty LabelTextDecorationsProperty = DependencyProperty.Register("LabelTextDecorations", typeof(TextDecorationCollection), typeof(Node));
        public static readonly DependencyProperty BoundariesProperty = DependencyProperty.Register("Boundaries", typeof(Shape), typeof(Node));
        public static readonly DependencyProperty IntersectionModeProperty = DependencyProperty.Register("IntersectionMode", typeof(IntersectionMode), typeof(Node));
        /// <summary>
        /// Identifies the LabelAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelAngleProperty = DependencyProperty.Register("LabelAngle", typeof(double), typeof(Node), new FrameworkPropertyMetadata(0d));



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


        public bool ProportionalResize
        {
            get { return (bool)GetValue(ProportionalResizeProperty); }
            set { SetValue(ProportionalResizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProportionalResize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProportionalResizeProperty =
            DependencyProperty.Register("ProportionalResize", typeof(bool), typeof(Node), new PropertyMetadata(false));

        
        /// <summary>
        /// Identifies the LogicalOffsetY dependency property.
        /// </summary>
        [Obsolete("Use OffsetYProperty")]
        public static readonly DependencyProperty LogicalOffsetYProperty = DependencyProperty.Register("LogicalOffsetY", typeof(double), typeof(Node), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnLogicalOffsetYChanged)));

        /// <summary>
        /// Identifies the LogicalOffsetX dependency property.
        /// </summary>
        [Obsolete("Use OffsetXProperty")]
        public static readonly DependencyProperty LogicalOffsetXProperty = DependencyProperty.Register("LogicalOffsetX", typeof(double), typeof(Node), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnLogicalOffsetXChanged)));
        

        /// <summary>
        /// Identifies the RotateAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateAngleProperty = DependencyProperty.Register("RotateAngle", typeof(double), typeof(Node), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnRotateAngleChanged)));

        /// <summary>
        /// Identifies the LabelWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register("LabelWidth", typeof(double), typeof(Node), new FrameworkPropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the LabelHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHeightProperty = DependencyProperty.Register("LabelHeight", typeof(double), typeof(Node));

        /// <summary>
        /// Identifies the LabelTextWrapping dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextWrappingProperty = DependencyProperty.Register("LabelTextWrapping", typeof(TextWrapping), typeof(Node), new FrameworkPropertyMetadata(TextWrapping.NoWrap));

        /// <summary>
        /// Identifies the LabelFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register("LabelFontSize", typeof(double), typeof(Node), new FrameworkPropertyMetadata(11d));

        /// <summary>
        /// Identifies the LabelFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontFamilyProperty = DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(Node), new FrameworkPropertyMetadata(new FontFamily("Arial")));

        /// <summary>
        /// Identifies the LabelFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontWeightProperty = DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(Node), new FrameworkPropertyMetadata(FontWeights.SemiBold));

        /// <summary>
        /// Identifies the LabelFontStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontStyleProperty = DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(Node), new FrameworkPropertyMetadata(FontStyles.Normal));

        /// <summary>
        /// Identifies the LabelTextTrimming dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextTrimmingProperty = DependencyProperty.Register("LabelTextTrimming", typeof(TextTrimming), typeof(Node), new FrameworkPropertyMetadata(TextTrimming.CharacterEllipsis));

        /// <summary>
        /// Identifies the LabelTextAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextAlignmentProperty = DependencyProperty.Register("LabelTextAlignment", typeof(TextAlignment), typeof(Node), new FrameworkPropertyMetadata(TextAlignment.Center));

        /// <summary>
        /// Identifies the GripperVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperVisibilityProperty = DependencyProperty.Register("GripperVisibility", typeof(Visibility), typeof(Node), new UIPropertyMetadata(Visibility.Hidden));

        /// <summary>
        /// Identifies the GripperStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperStyleProperty = DependencyProperty.Register("GripperStyle", typeof(Style), typeof(Node));

        /// <summary>
        ///  Identifies the LabelBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty = DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(Node), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        ///  Identifies the LabelForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(Node), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the IsLabelEditable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLabelEditableProperty = DependencyProperty.Register("IsLabelEditable", typeof(bool), typeof(Node), new UIPropertyMetadata(true));

        public static readonly DependencyProperty IsLabelDragableProperty = DependencyProperty.Register("IsLabelDragable", typeof(bool), typeof(Node), new UIPropertyMetadata(false));

        public static readonly DependencyProperty LabelDisplacementProperty = DependencyProperty.Register("LabelDisplacement", typeof(Point), typeof(Node), new UIPropertyMetadata(new Point(0, 0)));

        /// <summary>
        /// Identifies the AllowSelect dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowSelectProperty = DependencyProperty.Register("AllowSelect", typeof(bool), typeof(Node), new UIPropertyMetadata(true));


        internal static readonly DependencyProperty RectBoundsProperty = DependencyProperty.Register("Rectbounds", typeof(Thickness), typeof(Node), new PropertyMetadata(null));
        /// <summary>
        /// Identifies the AllowMove dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowMoveProperty = DependencyProperty.Register("AllowMove", typeof(bool), typeof(Node), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the AllowRotate dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowRotateProperty = DependencyProperty.Register("AllowRotate", typeof(bool), typeof(Node), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRotatePropertyChanged)));

        /// <summary>
        /// Identifies the AllowResize dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowResizeProperty = DependencyProperty.Register("AllowResize", typeof(bool), typeof(Node), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the AllowPortDrag dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowPortDragProperty = DependencyProperty.Register("AllowPortDrag", typeof(bool), typeof(Node), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the AllowDelete dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowDeleteProperty = DependencyProperty.Register("AllowDelete", typeof(bool), typeof(Node), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowDeletePropertyChanged)));

        /// <summary>
        /// Identifies the MeasurementUnits property.
        /// </summary>
        public static readonly DependencyProperty MeasurementUnitsProperty = DependencyProperty.Register("MeasurementUnits", typeof(MeasureUnits), typeof(Node), new PropertyMetadata(MeasureUnits.Pixel, new PropertyChangedCallback(OnUnitsChanged)));

        /// <summary>
        /// Identifies the ParentId dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(Node));

        /// <summary>
        /// Identifies the IsGroup dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGroupedProperty = DependencyProperty.Register("IsGrouped", typeof(bool), typeof(Node));

        /// <summary>
        /// Identifies the IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(Node), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Identifies the DragProviderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty DragProviderTemplateProperty = DependencyProperty.RegisterAttached("DragProviderTemplate", typeof(ControlTemplate), typeof(Node));

        /// <summary>
        /// Identifies the IsDragConnectionOver dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragConnectionOverProperty = DependencyProperty.Register("IsDragConnectionOver", typeof(bool), typeof(Node), new FrameworkPropertyMetadata(false));


        internal static readonly DependencyProperty IsNodeFoundProperty = DependencyProperty.Register("IsNodeFound", typeof(bool), typeof(Node), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Identifies the Shape dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register("Shape", typeof(Shapes), typeof(Node), new FrameworkPropertyMetadata(Shapes.Default));

        /// <summary>
        /// Identifies the CustomPathStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomPathStyleProperty = DependencyProperty.Register("CustomPathStyle", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the Level dependency property.
        /// </summary>
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(int), typeof(Node), new FrameworkPropertyMetadata(0));

        /// <summary>
        /// Identifies the LabelVerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty = DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), typeof(Node), new FrameworkPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Identifies the LabelHorizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty = DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(Node), new FrameworkPropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Identifies the Label dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(Node), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnLabelChanged)));

        /// <summary>
        /// Identifies the Node's Obstacle dependency property.
        /// </summary>
        public static readonly DependencyProperty TreatAsObstacleProperty = DependencyProperty.Register("TreatAsObstacle", typeof(bool), typeof(Node), new UIPropertyMetadata(true));
        
        /// <summary>
        /// Identifies the LabelVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(Node), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the PortVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty PortVisibilityProperty = DiagramView.PortVisibilityProperty.AddOwner(typeof(Node), new FrameworkPropertyMetadata(PortVisibility.MouseOverNode, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the IsPageEditable(DiagramView) dependency property.
        /// </summary>
        internal static readonly DependencyProperty PageEditableIndicatorProperty = DependencyProperty.Register("PageEditableIndicator", typeof(bool), typeof(Node), new UIPropertyMetadata(true));
        /// <summary>
        /// Identifies the IsCenterPortEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPortEnabledProperty = DependencyProperty.Register("IsPortEnabled", typeof(bool), typeof(Node), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the IsDoubleClicked dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDoubleClickedProperty = DependencyProperty.Register("IsDoubleClicked", typeof(bool), typeof(Node), new UIPropertyMetadata(true));
        /// <summary>
        /// Identifies the EnableMultilineLabel dependency Property.
        /// </summary>
        public static readonly DependencyProperty EnableMultilineLabelProperty = DependencyProperty.Register("EnableMultilineLabel", typeof(bool), typeof(Node), new PropertyMetadata(false));

        public static readonly DependencyProperty LeftResizerProperty = DependencyProperty.Register("LeftResizer", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty RightResizerProperty = DependencyProperty.Register("RightResizer", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty BottomResizerProperty = DependencyProperty.Register("BottomResizer", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TopResizerProperty = DependencyProperty.Register("TopResizer", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TopLeftCornerResizerProperty = DependencyProperty.Register("TopLeftCornerResizer", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TopRightCornerResizerProperty = DependencyProperty.Register("TopRightCornerResizer", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty BottomLeftCornerResizerProperty = DependencyProperty.Register("BottomLeftCornerResizer", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty BottomRightCornerResizerProperty = DependencyProperty.Register("BottomRightCornerResizer", typeof(Style), typeof(Node), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DeletingMode.
        /// </summary>
        public static readonly DependencyProperty DeletingModeProperty = DependencyProperty.Register("DeletingMode", typeof(DeletingMode), typeof(Node), new PropertyMetadata(DeletingMode.DeleteDependentEdges));


        #endregion

        #region Events

        /// <summary>
        /// Raises the click event.
        /// </summary>
        public void RaiseClickEvent()
        {
            NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
            newEventArgs.RoutedEvent = DiagramView.NodeClickEvent;
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// Raises the drag start event.
        /// </summary>
        internal double x, y;
        public void RaiseNodeDragStartEvent()
        {
            if (dview != null)
            {
                if (Node.mouseup && dview.IsPageEditable)
                {
                    foreach (ICommon node in dview.SelectionList.OfType<ICommon>())
                    {
                        if (node is Group)
                        {
                            NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node as Group);
                            newEventArgs.RoutedEvent = DiagramView.NodeDragStartEvent;
                            RaiseEvent(newEventArgs);
                            (node as Group).x = newEventArgs.Node.OffsetX;
                            (node as Group).y = newEventArgs.Node.OffsetY;
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
                            newEventArgs.RoutedEvent = DiagramView.NodeDragStartEvent;
                            RaiseEvent(newEventArgs);
                            (node as Node).x = newEventArgs.Node.OffsetX;
                            (node as Node).y = newEventArgs.Node.OffsetY;
                        }
                        if (node is LineConnector)
                        {
                            (node as LineConnector).sp = (node as LineConnector).StartPointPosition;
                            (node as LineConnector).ep = (node as LineConnector).EndPointPosition;
                        }
                        Node.mouseup = false;
                    }
                }
            }
        }

        /// <summary>
        /// Raises the drag end event.
        /// </summary>
        public void RaiseNodeDragEndEvent()
        {
            if (dview.IsPageEditable)
            {
                NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
                newEventArgs.RoutedEvent = DiagramView.NodeDragEndEvent;
                RaiseEvent(newEventArgs);
                dview.InvalidateViewGrid();
            }
        }

        /// <summary>
        /// Raises the double click event.
        /// </summary>
        public void RaiseDoubleClickEvent()
        {
            NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(this);
            newEventArgs.RoutedEvent = DiagramView.NodeDoubleClickEvent;
            RaiseEvent(newEventArgs);
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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                if (dview != null)
                {
                    if (dview.IsPageSaved)
                    {
                        foreach (DiagramProperty d in dview.DiagramProperties.Where(item => item.ObjectType.Equals(typeof(Node))))
                        {
                            if (d.PropertyName.Equals(name))
                            {
                                dview.IsPageSaved = false;
                            }
                        }
                    }
                }
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                if (dview != null)
                {
                    if (dview.IsPageSaved)
                    {
                        foreach (DiagramProperty d in dview.DiagramProperties.Where(item => item.ObjectType.Equals(typeof(Node))))
                        {
                            if (d.PropertyName.Equals(e.Property.Name))
                            {
                                dview.IsPageSaved = false;
                            }
                        }
                    }
                }
                handler(this, new PropertyChangedEventArgs(e.Property.ToString()));
            }
        }

        #endregion

        #region class override

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
#if !SyncfusionFramework3_5

            this.IsManipulationEnabled = true;
            this.ManipulationStarting += new EventHandler<ManipulationStartingEventArgs>(Node_ManipulationStarting);
            this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(Node_ManipulationDelta);
            this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(Node_ManipulationCompleted);
#endif
            
            editor = GetTemplateChild("PART_LabelEditor") as LabelEditor;
            rotatethumb = GetTemplateChild("PART_Rotator") as Control;
            grouprotatethumb = GetTemplateChild("PART_Rotator1") as Control;
            dview = GetDiagramView(this);
            dragge = GetTemplateChild("PART_DragProvider") as DragProvider;
            if (dview != null)
            {
                if (!dview.SelectionList.Contains(this) && this.IsSelected)
                {
                    this.NodeSelection();
                }
            }
            RectBounds = this.TransformRectBounds();
            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            if (double.IsNaN(this.Width))
            {
                this.Width = DesiredSize.Width;
            }
            if (double.IsNaN(this.Height))
            {
                this.Height = DesiredSize.Height;
            }

            if (!DiagramControl.IsPageLoaded)
            {
                ConnectionPort centerport = new ConnectionPort();
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
                    double two0ne = 21;// MeasureUnitsConverter.FromPixels(21, this.MeasurementUnits);
                    centerport.Left = two0ne;
                    centerport.Top = two0ne;
                }

                if (this.Ports != null)
                {
                    //this.Ports.Add(centerport);
                }

                centerport.CenterPortReferenceNo = 0;
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

            //Getting the Parent
            UIElement parent = this.Parent as UIElement;
            //Creating the contextmenu
            if (this.ContextMenu == null && parent is DiagramPage)
            {
                CreateContextMenu();
            }
        }
#if !SyncfusionFramework3_5
        void Node_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            e.Cancel();
        }

        void Node_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            int manipulatorcount = e.Manipulators.Count<System.Windows.Input.IManipulator>();
            var element = e.OriginalSource as FrameworkElement;
            if (manipulatorcount >= 2) 
            {
                ManipulationDelta manipDelta = e.DeltaManipulation;
                Matrix rectsMatrix = ((MatrixTransform)element.RenderTransform).Matrix;
                element.Width = element.Width * manipDelta.Scale.X;
                element.Height = element.Height * manipDelta.Scale.Y;
                element.UpdateLayout();
                e.Handled = true;

            }
            else
            {
                e.Cancel();
            }
            element.UpdateLayout();
        }

        void Node_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Handled = true;
           
        }

       
#endif

        private void NodeSelection()
        {

            dview = GetDiagramView(this);
            Node n = this as Node;
            if (n.IsSelected && n.Page != null && (n.Page as DiagramPage).SelectionList != null)
            {
                (n.Page as DiagramPage).SelectionList.Add(n);
            }
            else if (!n.IsSelected && n.Page != null && (n.Page as DiagramPage).SelectionList != null && !(n.Page as DiagramPage).SelectionList.m_collClear)
            {
                (n.Page as DiagramPage).SelectionList.Remove(n);
            }
            if ((n.GetTemplateChild("PART_Rotator") as Rotator) != null)
            {
                if (n.AllowRotate && n.IsSelected)
                {
                    (n.GetTemplateChild("PART_Rotator") as Rotator).Visibility = Visibility.Visible;
                }
                else
                {
                    (n.GetTemplateChild("PART_Rotator") as Rotator).Visibility = Visibility.Collapsed;
                }
            }
            if (!n.IsSelected)
            {
                NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(n as Node);
                newEventArgs.RoutedEvent = DiagramView.NodeUnSelectedEvent;

                n.RaiseEvent(newEventArgs);
            }
            else
            {
                NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(n as Node);
                newEventArgs.RoutedEvent = DiagramView.NodeSelectedEvent;
                n.RaiseEvent(newEventArgs);
            }
            if (n.IsSelected == true)
            {
                this.Focus();
            }

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            isdoubleclicked = true;
            if (dview.IsPageEditable)
            {
                RaiseDoubleClickEvent();
                NodeLabeledit();
            }
        }

        /// <summary>
        /// Provides class handling for the MouseDown routed event that occurs when the mouse 
        /// button is pressed while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs</param>
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            if (dc.View.IsPageEditable && !dview.IsPanEnabled && this.AllowSelect)
            {
                ////base.OnPreviewMouseLeftButtonDown(e);

                IDiagramPage diagramPanel = VisualTreeHelper.GetParent(this) as IDiagramPage;
                if (diagramPanel != null)
                {
                    if (!this.IsGrouped)
                    {
                        ////diagramPanel.SelectionList.Clear();
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
        }


        DragProvider dragge;
        #region ContextMenuForNode

        internal string ContextMenu_Delete;
        internal string ContextMenu_Grouping;
        internal string ContextMenu_Grouping_Group;
        internal string ContextMenu_Grouping_Ungroup;
        internal string ContextMenu_Order;
        internal string ContextMenu_Order_BringForward;
        internal string ContextMenu_Order_BringToFront;
        internal string ContextMenu_Order_SendBackward;
        internal string ContextMenu_Order_SendToBack;
        MenuItem Order;
        MenuItem Front;
        MenuItem Forward;
        MenuItem Backward;
        MenuItem Back;
        MenuItem Grouping;
        MenuItem Group;
        MenuItem Ungroup;
        MenuItem Delete;

        //Creating the contextmenu
        private void CreateContextMenu()
        {
            nodecontextmenu = new System.Windows.Controls.ContextMenu();
            Order = new MenuItem();
            Order.Header = ContextMenu_Order;// "Order";
            header.Add(ContextMenu_Order);
            Front = new MenuItem();
            Front.Header = ContextMenu_Order_BringToFront;// "Bring To Front";
            Front.Click += new RoutedEventHandler(Front_Click);
            Order.Items.Add(Front);
            Forward = new MenuItem();
            Forward.Header = ContextMenu_Order_BringForward;// "Bring Forward";
            Forward.Click += new RoutedEventHandler(Forward_Click);
            Order.Items.Add(Forward);
            Backward = new MenuItem();
            Backward.Header = ContextMenu_Order_SendBackward;// "Send Backward";
            Backward.Click += new RoutedEventHandler(Backward_Click);
            Order.Items.Add(Backward);
            Back = new MenuItem();
            Back.Header = ContextMenu_Order_SendToBack;// "Send To Back";
            Back.Click += new RoutedEventHandler(Back_Click);
            Order.Items.Add(Back);

            Grouping = new MenuItem();
            Grouping.Header = ContextMenu_Grouping;// "Grouping";
            header.Add(ContextMenu_Grouping);
            Group = new MenuItem();
            Group.Header = ContextMenu_Grouping_Group;// "Group";
            Group.Click += new RoutedEventHandler(Group_Click);
            Grouping.Items.Add(Group);
            Ungroup = new MenuItem();
            Ungroup.Header = ContextMenu_Grouping_Ungroup;// "Ungroup";
            Ungroup.Click += new RoutedEventHandler(Ungroup_Click);
            Grouping.Items.Add(Ungroup);

            Delete = new MenuItem();
            Delete.Header = ContextMenu_Delete;// "Delete";
            header.Add(ContextMenu_Delete);
            Delete.Click += new RoutedEventHandler(Del_Click);

            nodecontextmenu.Items.Add(Order);
            nodecontextmenu.Items.Add(Grouping);
            nodecontextmenu.Items.Add(Delete);
            //this.ContextMenu = nodecontextmenu;
            
        }
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (dview != null)
            {
                if (dc.View.EnableDrawingTools)
                {
                            if (dview.Drawing_TailPort != null)
                            {
                                dview.Drawing_TailNode = dview.Drawing_TailPort.Node as Node;
                            }
                            else
                            {
                                dview.Drawing_TailNode = this;
                            }
                           
                    return;
                }
                if (dc.View.Page != null && (dc.View.Page is DiagramPage))
                {
                    TimeSpan diff = DateTime.Now.Subtract(dc.View.timeRightClick);
                    int d = diff.CompareTo(new TimeSpan(0, 0, 0, 0, 500));
                    if (d == 1)
                    {
                        
                       //Get Invoked when the Node's contextmenu is default contextMenu and DiagramView's ContextMenu is Null
                            if (dview.IsPageEditable && dview.NodeContextMenu == null && this.nodecontextmenu != null)
                            {
                                if(this.ContextMenu==null)
                                this.ContextMenu = this.nodecontextmenu;
                                setDisableOrEnable(Front, Forward, Backward, Back, Delete, Group);

                            }
                            //Precedencde over the Node's ContextMenu(DiagramView's ContextMenu has value)
                            else if (dview.NodeContextMenu != null)
                            {
                                this.ContextMenu = dview.NodeContextMenu;
                            }
                            else
                            {
                                //Refresh reference  context menu while performaning save and Load
                                if (this.ContextMenu != null)
                                {
                                    if (isDefalutContextMenu())
                                    {
                                        ContextMenu nodecontextmenu = this.ContextMenu;
                                        MenuItem Order = nodecontextmenu.Items[0] as MenuItem;
                                        MenuItem Grouping = nodecontextmenu.Items[1] as MenuItem;
                                        MenuItem Delete = nodecontextmenu.Items[2] as MenuItem;
                                        Delete.Click -= new RoutedEventHandler(Del_Click);
                                        Delete.Click += new RoutedEventHandler(Del_Click);

                                        MenuItem Front = Order.Items[0] as MenuItem;
                                        Front.Click -= new RoutedEventHandler(Front_Click);
                                        Front.Click += new RoutedEventHandler(Front_Click);
                                        MenuItem Forward = Order.Items[1] as MenuItem;
                                        Forward.Click -= new RoutedEventHandler(Forward_Click);
                                        Forward.Click += new RoutedEventHandler(Forward_Click);
                                        MenuItem Backward = Order.Items[2] as MenuItem;
                                        Backward.Click -= new RoutedEventHandler(Backward_Click);
                                        Backward.Click += new RoutedEventHandler(Backward_Click);
                                        MenuItem Back = Order.Items[3] as MenuItem;
                                        Back.Click -= new RoutedEventHandler(Back_Click);
                                        Back.Click += new RoutedEventHandler(Back_Click);
                                        MenuItem Group = Grouping.Items[0] as MenuItem;
                                        Group.Click -= new RoutedEventHandler(Group_Click);
                                        Group.Click += new RoutedEventHandler(Group_Click);
                                        MenuItem Ungroup = Grouping.Items[1] as MenuItem;
                                        Ungroup.Click -= new RoutedEventHandler(Ungroup_Click);
                                        Ungroup.Click += new RoutedEventHandler(Ungroup_Click);

                                        setDisableOrEnable(Front, Forward, Backward, Back, Delete, Group);
                                    }
                                }
                            }
                    }
                }
            }
            base.OnMouseRightButtonUp(e);
        }


        //Checking for the customContextmenu
        private bool isDefalutContextMenu()
        {
            if (this.ContextMenu != null)
            {
                Populateheader();
                ContextMenu nodecontextmenu = this.ContextMenu;
                List<string> oldheader = new List<string>();
                //Populating with the customer ContextMenu
                for (int i = 0; i < nodecontextmenu.Items.Count; i++)
                {
                    if ((nodecontextmenu.Items[i] is MenuItem) && (nodecontextmenu.Items[i] as MenuItem).Header != null)
                    {
                        oldheader.Add((nodecontextmenu.Items[i] as MenuItem).Header.ToString());
                    }
                }

                //Cheking for Default context is null and comparing the count of Customer ContextMenu and Default contextMenu 
                if (header != null && header.Count == this.ContextMenu.Items.Count)
                {                                    
                    //Checking for the Header String
                   if(oldheader[0]==header[0])
                   {
                       if(oldheader[1]==header[1])
                       {
                           if(oldheader[2]==header[2])
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

        //SetDisableOrEnable the properties
        private void setDisableOrEnable(MenuItem Front, MenuItem Forward, MenuItem Backward, MenuItem Back, MenuItem Delete, MenuItem Group)
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
        private void Front_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.BringToFront.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the bring forward menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.MoveForward.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the send backward menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Backward_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.SendBackward.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the send to back menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.SendToBack.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the group menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Group_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.Group.Execute(dview.Page, dview);
        }

        /// <summary>
        /// Handles the Click event of the ungroup menu item.
        /// </summary>
        /// <param name="sender">The diagram view.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Ungroup_Click(object sender, RoutedEventArgs e)
        {
            DiagramCommandManager.Ungroup.Execute(dview.Page, dview);
        }
        #endregion
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (dview != null)
            {
                if (this.Groups.Count > 0)
                {
                    this.IsGrouped = true;
                }
                dview.m_DragandMove = true;
                mousedown = true;
                up = false;
                bool portadded = false;
                dview._contentHolder.UpdateFitToPage += new Shared.OverviewContentHolder.OverviewFitPageEventHandler(_contentHolder_UpdateFitToPage);
                if (editor != null && (!LabelEditor.GetIsEditing(editor) || editor.InputHitTest(e.GetPosition(editor)) == null))
                {
                    ex = false;
                    if (dview.IsPageEditable && !this.IsGrouped)
                    {

                        if (!dview.IsPanEnabled)
                        {
                            base.OnPreviewMouseLeftButtonDown(e);
                            IDiagramPage m_diagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;

                            //// update selection
                            if (m_diagramPage != null)
                            {
                                if (dview.EnableDrawingTools)
                                {
                                    this.startPoint = e.GetPosition(m_diagramPage as Panel);
                                    //e.Handled = true;
                                }
                                if (dview.IsPageEditable)
                                {
                                    if (this.AddConnectionPortEnabled)
                                    {
                                        if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
                                        //if ((Keyboard.Modifiers & (ModifierKeys.Shift & ModifierKeys.Control)) != ModifierKeys.None)
                                        {
                                            if (this.IsSelected)
                                            {
                                                Assembly ass = Assembly.GetExecutingAssembly();
                                                System.IO.Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.InsertVertex.cur");
                                                this.Cursor = new Cursor(stream);
                                                if (e.LeftButton == MouseButtonState.Pressed)
                                                {
                                                    ConnectionPort port = new ConnectionPort();
                                                    Point portpoint = e.MouseDevice.GetPosition(this);
                                                    port.Left = portpoint.X;
                                                    port.Top = portpoint.Y;
                                                    this.Ports.Add(port);
                                                    port.m_justnow = true;
                                                    port.Node = this;
                                                    portadded = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                                {
                                    if (this.IsSelected && !portadded)
                                    {
                                        (dview as DiagramView).SelectionList.Remove(this);
                                    }
                                    else
                                    {
                                        portadded = false;
                                        if (this.AllowSelect)
                                        {
                                            (dview as DiagramView).SelectionList.Add(this);
                                        }
                                    }
                                }
                                else if (!this.IsSelected)
                                {
                                    if (m_diagramPage != null)
                                    {
                                        if (dview.EnableConnection && dview.NodeMode == ConnectionMode.Connect)
                                        {
                                            this.startPoint = e.GetPosition(m_diagramPage as Panel);
                                            e.Handled = true;
                                        }
                                    }

                                    if (this.AllowSelect && CanSelect())
                                    {
                                        m_diagramPage.SelectionList.Select(this);
                                    }
                                }

                            }

                            lastNodeClick = DateTime.Now;
                            lastNodePoint = e.GetPosition(m_diagramPage as Panel);
                        }
                    }
                    else if (dview.IsPageEditable && this.IsGrouped)
                    {
                        if (!dview.IsPanEnabled)
                        {
                            base.OnPreviewMouseLeftButtonDown(e);
                            IDiagramPage m_diagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;
                            if (dview.IsPageEditable)
                            {
                                if (this.AddConnectionPortEnabled)
                                {
                                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
                                    //if ((Keyboard.Modifiers & (ModifierKeys.Shift & ModifierKeys.Control)) != ModifierKeys.None)
                                    {
                                        if (this.IsSelected)
                                        {
                                            Assembly ass = Assembly.GetExecutingAssembly();
                                            System.IO.Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.InsertVertex.cur");
                                            this.Cursor = new Cursor(stream);
                                            if (e.LeftButton == MouseButtonState.Pressed)
                                            {
                                                ConnectionPort port = new ConnectionPort();
                                                Point portpoint = e.MouseDevice.GetPosition(this);
                                                port.Left = portpoint.X;
                                                port.Top = portpoint.Y;
                                                this.Ports.Add(port);
                                                port.m_justnow = true;
                                                port.Node = this;
                                                portadded = true;
                                            }
                                        }
                                    }
                                }
                            }
                            // update selection
                            if (m_diagramPage != null)
                            {
                                if (dview.EnableConnection && dview.NodeMode == ConnectionMode.Connect)
                                {
                                    this.startPoint = e.GetPosition(m_diagramPage as Panel);
                                    e.Handled = true;
                                }
                            }
                        }
                    }
                }
                if (dview.EnableDrawingTools && dview.NodeMode == ConnectionMode.Connect &&
                dview.DrawingTool != DrawingTools.Ellipse && dview.DrawingTool != DrawingTools.RoundedRectangle && dview.DrawingTool != DrawingTools.Rectangle && dview.DrawingTool != DrawingTools.Polygon)
                { 
                    if (dview.DrawingTool == DrawingTools.PolyLine)
                    {
                        if (dview.tempPolyLine == null)
                        {
                            SetHeadNode(e.GetPosition(Page as Panel));
                        }
                    }
                    else
                    {
                        SetHeadNode(e.GetPosition(Page as Panel));
                    }
                }
            }
        }



      private  bool CanSelect()
        {
            if (!dview.EnableDrawingTools)
            {
                return true;
            }
            if(dview.EnableDrawingTools&&dview.NodeMode==ConnectionMode.Move)
            {
                return true;
            }
            else
            {
                return false;
            }
               
        }
       void SetHeadNode(Point hitpoint)
        {
            bool f_node = dview.HitTesting(hitpoint);
            if (f_node)
            {
                if (dview.HitNode != null)
                {
                    dview.newconnection = new LineConnector(dview);
                    dview.Drawing_HeadNode = dview.HitNode;
                    dview.Drawing_HeadNode.IsDragConnectionOver = true;
                    dview.sourceNode = dview.HitNode;
                    dview.HitNode = null;
                    {
                        BeforeCreateConnectionRoutedEventArgs newEventArgs = new BeforeCreateConnectionRoutedEventArgs(dview.newconnection as LineConnector);
                        newEventArgs.RoutedEvent = DiagramView.BeforeConnectionCreateEvent;
                        RaiseEvent(newEventArgs);
                        BeforeConnection = true;
                    }
                }
                if (dview.HitPort != null)
                {
                    if (dview.PortMode == ConnectionMode.Connect)
                    {

                        dview.Drawing_HeadPort = dview.HitPort;
                        dview.sourceHitPort = dview.HitPort;
                        dview.HitPort = null;
                    }
                }
            }
        }
        bool up;
        Shared.OverviewContentHolder.OverViewFitToPageEventArgs overpage;
        void _contentHolder_UpdateFitToPage(object sender, Shared.OverviewContentHolder.OverViewFitToPageEventArgs evtArgs)
        {
            overpage = evtArgs;
            if (up && dview != null && dview.EnableFitToPage)
            {
                dview._contentHolder.UpdateFitToPage -= new Shared.OverviewContentHolder.OverviewFitPageEventHandler(_contentHolder_UpdateFitToPage);
                dview._contentHolder.EnableFitToPage = true;
                evtArgs.Cancel = true;
                up = false;
            }
            else
            {
                evtArgs.Cancel = false;
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {

            if (dview.EnableDrawingTools&&dview.NodeMode==ConnectionMode.Connect)
            {
                if (dview.DrawingTool != DrawingTools.PolyLine && !(dview.Page as DiagramPage).IsPolyLineEnabled)
                {
                    bool f_node = dview.HitTesting(e.GetPosition(Page as Panel));
                    if (f_node)
                    {
                        if (dview.HitNode != null)
                        {
                            dview.Drawing_TailNode = dview.HitNode;
                            dview.Drawing_TailNode.IsDragConnectionOver = false;
                        }
                        if (dview.HitPort != null)
                        {
                            if (dview.PortMode == ConnectionMode.Connect)
                            dview.Drawing_TailPort = dview.HitPort;
                        }
                    }
                }
            }
            else
            {
                if (dview.EnableDrawingTools && dview.PortMode == ConnectionMode.Connect)
                {
                    bool f_node = dview.HitTesting(e.GetPosition(Page as Panel));
                    if (f_node)
                    {
                        if (dview.HitPort != null)
                        {
                            if (dview.PortMode == ConnectionMode.Connect)
                                dview.Drawing_TailPort = dview.HitPort;
                            if (dview.HitNode != null)
                            {
                                dview.Drawing_TailNode = dview.HitNode;
                                dview.Drawing_TailNode.IsDragConnectionOver = false;
                            }
                        else
                            if (dview.centerhit)
                            {
                                dview.Drawing_TailNode = dview.HitNode;
                                dview.Drawing_TailNode.IsDragConnectionOver = false;
                            }
                        }
                    }
                    if (dview.HitPort == null && dview.HitNode != null)
                        dview.HitNode.IsDragConnectionOver = false;
                }
            }
            mousedown = false;
            up = true;
            if (overpage != null && dview._contentHolder.EnableFitToPage)
            {
                overpage.Cancel = true;
            }
            base.OnPreviewMouseLeftButtonUp(e);
            dview.ScrollGrid.InvalidateArrange();
        
        }
        /// <summary>
        /// Provides class handling for the MouseMove routed event that occurs when the mouse 
        /// pointer  is over this control.
        /// </summary>
        /// <param name="e">The MouseEventArgs</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (dview != null)
            {
                if (dview.IsPageEditable)
                {
                    if (dragge != null)
                    {
                        if (dview.EnableDrawingTools&&dview.NodeMode==ConnectionMode.Connect)
                        {
                            dragge.IsHitTestVisible = false;
                        }
                        else
                        {
                            dragge.IsHitTestVisible = true;
                        }
                    }
                    
                    if (!dview.IsPageEditable || !this.AllowMove || !this.AllowSelect)
                    {
                        this.Cursor = System.Windows.Input.Cursors.Arrow;
                    }

                    base.OnMouseMove(e);

                    if (e.LeftButton != MouseButtonState.Pressed)
                    {
                        this.startPoint = null;
                    }

                    if (this.startPoint.HasValue)
                    {
                        IDiagramPage m_diagramPanel = GetPanel(this);

                        if (m_diagramPanel != null)
                        {
                            if (!IsDoubleClick(e.GetPosition(m_diagramPanel as DiagramPage)))
                            {
                                AdornerLayer adorner = AdornerLayer.GetAdornerLayer(m_diagramPanel as Panel);
                                if (adorner != null)
                                {
                                    foreach (ConnectionPort port in this.Ports)
                                    {
                                        if (port.Ismouseover)
                                        {
                                            sourceHitPort = port;
                                        }
                                    }
                                    if (dview.m_connectionadorner == null)
                                    {
                                        if (dview._ConnectionOnPort)
                                        {
                                            dview._ConnectionOnPort = false;
                                            NodeConnectorAdorner nodeadorner = new NodeConnectorAdorner(m_diagramPanel, sourceHitPort, this, GetDiagramView(this));
                                            dview.m_connectionadorner = nodeadorner;
                                            if (nodeadorner != null)
                                            {
                                                adorner.Add(nodeadorner);
                                                e.Handled = true;
                                            }
                                        }
                                    }
                                    dview.EnableConnection = false;
                                    dview.m_EnableConnection = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            dview = GetDiagramView(this);
            if (dview.EnableDrawingTools)
            {
                if (e.LeftButton == MouseButtonState.Pressed && dview.NodeMode==ConnectionMode.Connect)
                {
                    this.IsDragConnectionOver = true;
                }
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            dview = GetDiagramView(this);
            if (dview != null)
            {
                if (dview.EnableDrawingTools)
                {
                    this.IsDragConnectionOver = false;
                }
                base.OnMouseLeave(e);
            }
        }

        #endregion

        #region Implementation

        internal static double Round(double value, double cutoff)
        {
            return (Math.Floor(value / cutoff) + (value % cutoff > cutoff / 2 ? 1 : 0)) * cutoff;
        }

        internal static void setINodeBinding(Node n, INode i)
        {
            if (n != null && i != null)
            {
                Binding X = new Binding("OffsetX");
                X.Source = i;
                X.Mode = BindingMode.TwoWay;
                n.SetBinding(Node.OffsetXProperty, X);

                Binding Y = new Binding("OffsetY");
                Y.Source = i;
                Y.Mode = BindingMode.TwoWay;
                n.SetBinding(Node.OffsetYProperty, Y);
            }
        }

        /// <summary>
        /// Invoked when Label editing is started.
        /// </summary>
        public void NodeLabeledit()
        {
            if (IsLabelEditable)
            {
                if (editor != null)
                {
                    editor.LabelEditStartInternal(editor);
                }
            }
        }

        /// <summary>
        /// Invoked when label editing is complete.
        /// </summary>
        internal void CompleteEditing()
        {
            if (editor != null)
            {
                editor.CompleteHeaderEditInternal(editor, true);
            }
        }

        /// <summary>
        /// Called when the mouse button is clicked twice.
        /// </summary>
        /// <param name="position">Mouse Position</param>
        /// <returns>true if double clicked, false otherwise</returns>
        public bool IsDoubleClick(Point position)
        {
            if (((DateTime.Now.Subtract(lastNodeClick).TotalMilliseconds < 500) && (Math.Abs((double)(lastNodePoint.X - position.X)) <= 2)) && (Math.Abs((double)(lastNodePoint.Y - position.Y)) <= 2))
            {
                return true;
            }

            return false;
        }

        //Shared.OverviewContentHolder.OverViewFitToPageEventArgs overpage;
        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Node control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Node_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bool notselected = true;
            bool selected = true;
            mousedown = false;
            up = true;
            foreach (ConnectionPort portin in this.Ports)
            {
                portin.m_justnow = false;
            }
            if (dview == null)
            {
                dview = GetDiagramView(this);
            }
            if (dview != null)
            {
                dview.m_DragandMove = true;
                //dview._contentHolder.UpdateFitToPage += new Shared.OverviewContentHolder.OverviewFitPageEventHandler(_contentHolder_UpdateFitToPage);
                if (!dview.Ispositionchanged && this.IsGrouped)
                {
                    if (dview.IsPageEditable)
                    {
                        if (!dview.IsPanEnabled && !isdoubleclicked)
                        {
                            //// base.OnPreviewMouseLeftButtonDown(e);
                            IDiagramPage m_diagramPage = VisualTreeHelper.GetParent(this) as IDiagramPage;

                            //// update selection
                            if (m_diagramPage != null)
                            {


                                if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
                                {
                                    dview.SelectionList.Clear();
                                    this.IsSelected = true;
                                }
                              else if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                                {
                                    if (this.IsSelected)
                                    {
                                        m_diagramPage.SelectionList.Remove(this);

                                        foreach (Group group in this.Groups)
                                        {
                                            foreach (ICommon Innode in group.NodeChildren)
                                            {
                                                if (this != Innode)
                                                {
                                                    if (Innode.IsSelected)
                                                    {
                                                        selected = false;

                                                    }
                                                }
                                            }
                                        }
                                        foreach (Group groups in this.Groups)
                                        {
                                            if (selected)
                                            {
                                                if (!groups.IsSelected)
                                                {

                                                }
                                                else
                                                {
                                                    groups.IsSelected = false;
                                                    this.IsSelected = false;
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (!this.IsSelected)
                                        {

                                            foreach (Group group in this.Groups)
                                            {
                                                foreach (ICommon Innode in group.NodeChildren)
                                                {
                                                    if (this != Innode)
                                                    {
                                                        if (Innode.IsSelected)
                                                        {
                                                            selected = false;

                                                        }
                                                    }
                                                }

                                            }
                                            bool checking = true;
                                            for (int i = 0; i < this.Groups.Count; i++)
                                            {

                                                if (selected)
                                                {
                                                    if (checking)
                                                    {
                                                        if ((this.Groups[i] as Group).IsSelected)
                                                        {
                                                            (this.Groups[i] as Group).IsSelected = false;
                                                            if (i == this.Groups.Count - 1)
                                                            {
                                                                this.IsSelected = true;
                                                                checking = false;
                                                                break;
                                                            }


                                                            checking = false;
                                                        }

                                                        if (i == this.Groups.Count - 1)
                                                        {
                                                            (this.Groups[0] as Group).IsSelected = true;
                                                            checking = false;
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        (this.Groups[i] as Group).IsSelected = true;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    this.IsSelected = true;
                                                    for (int j = 0; j < this.Groups.Count; j++)
                                                    {
                                                        (this.Groups[j] as Group).IsSelected = false;
                                                    }
                                                    break;
                                                }
                                            }
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
                                    else if (this.AllowSelect && this.IsGrouped)
                                    {
                                        m_diagramPage.SelectionList.Select(this.Groups[this.Groups.Count - 1]);
                                    }
                                }
                                
                            }
                        }
                    }
                }

                dview.Ispositionchanged = false;
                isdoubleclicked = false;

                this.ResizeThisNode = false;
                if (!ex)
                {
                    if (DragProvider.Isdragging)
                    {
                        DiagramView.IsOtherEvent = true;
                    }

                    if (!DiagramView.IsOtherEvent)
                    {
                        RaiseClickEvent();
                    }
                }

                DiagramView.IsOtherEvent = false;
                ex = true;
                Node.mouseup = true;
                DragProvider.Isdragging = false;
                dview.InvalidateArrange();
            }
        }
        private static void OnAllowRotatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node node = d as Node;
            if ((node.GetTemplateChild("PART_Rotator") as Rotator) != null)
            {
                if (node.IsSelected && node.AllowRotate)
                {
                    (node.GetTemplateChild("PART_Rotator") as Rotator).Visibility = Visibility.Visible;
                }
                else
                {
                    (node.GetTemplateChild("PART_Rotator") as Rotator).Visibility = Visibility.Collapsed;
                }
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
            if (n._FirstLoaded)
            {
                n.OffsetX = MeasureUnitsConverter.Convert(n.OffsetX, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                n.OffsetY = MeasureUnitsConverter.Convert(n.OffsetY, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
                n.Position = MeasureUnitsConverter.Convert(n.Position, (MeasureUnits)e.OldValue, (MeasureUnits)e.NewValue);
            }
        }

        private static void OnAllowDeletePropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            (s as Node).Focus();
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Node n = d as Node;
            n.NodeSelection();
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
            double delta = (double)e.NewValue - (double)e.OldValue;
            Matrix mat = Matrix.Identity;
            Point pivot = node.TranslatePoint(new Point(node.ActualWidth / 2, node.ActualHeight / 2), node.Page);
            mat.RotateAt(delta, pivot.X, pivot.Y);
            MatrixTransform trans = new MatrixTransform(mat);
            Point moveDelta = trans.Transform(new Point(node.PxOffsetX, node.PxOffsetY));
            node.PxOffsetX = moveDelta.X;
            node.PxOffsetY = moveDelta.Y;
            if (node.IsGrouped)
            {
                foreach (Group g in node.Groups)
                {
                    if (g.RotateAngle != 0)
                        (node.RenderTransform as RotateTransform).Angle = node.RotateAngle + g.RotateAngle;
                    else
                        (node.RenderTransform as RotateTransform).Angle = node.RotateAngle;
                }
            }
            else
            (node.RenderTransform as RotateTransform).Angle = node.RotateAngle;
            node.Arrange(new Rect(node.PxOffsetX, node.PxOffsetY, node.DesiredSize.Width, node.DesiredSize.Height));
            if (node is Group)
            {
                foreach (Node n in (node as Group).NodeChildren.OfType<Node>().ToList<Node>())
                {
                    Matrix mat1 = Matrix.Identity;
                    Point pivot1 = node.TranslatePoint(new Point(node.ActualWidth / 2, node.ActualHeight / 2), node.Page);
                    mat1.RotateAt(delta, pivot1.X, pivot1.Y);
                    MatrixTransform trans1 = new MatrixTransform(mat1);
                    Point moveDelta1 = trans1.Transform(new Point(n.PxOffsetX, n.PxOffsetY));
                    n.PxOffsetX = moveDelta1.X;
                    n.PxOffsetY = moveDelta1.Y;
                    (n.RenderTransform as RotateTransform).Angle = n.RotateAngle + node.RotateAngle;
                    
                    n.Arrange(new Rect(n.PxOffsetX, n.PxOffsetY, n.DesiredSize.Width, n.DesiredSize.Height));
               }
            }
            if (rotatebycode && node.RotateAngle != double.NaN && node.AllowRotate)
            {
                //RotateTransform rt = new RotateTransform();
                //rt.Angle = node.RotateAngle;
                //if (node is Group)
                //{
                //    CollectionExt gnode = new CollectionExt();
                //    for (int i = 0; i < (node as Group).NodeChildren.Count; i++)
                //    {
                //        if (!((node as Group).NodeChildren[i] is LineConnector))
                //        {
                //            gnode.Add((node as Group).NodeChildren[i] as INodeGroup);
                //        }
                //    }
                //    //gnode.Add(node);
                //    foreach (INodeGroup item in gnode)
                //    {
                //        if (!(item is LineConnector))
                //        {
                //            if ((item as Node).AllowRotate)
                //            {
                //                //(item as Node).RenderTransform = rt;
                //                //(item as Node).RotateAngle += delta;
                //            }
                //        }

                //    }
                //}
                //else
                //{

                //    node.RenderTransform = rt;
                //}
            }
            if (dc != null && !dc.View.Undone && !dc.View.IsLayout)
            {
                if (!(dc.View.undo || dc.View.redo))
                {
                    dc.View.RedoStack.Clear();
                }
                if (dc.View.UndoRedoEnabled && !(d as Node).m_IsResizing)
                {
                    dc.View.UndoStack.Push(d as Node);
                    dc.View.UndoStack.Push(e.OldValue);
                    dc.View.UndoStack.Push(dc.View.NodeRotateCount);
                    dc.View.UndoStack.Push("Rotated");
                }
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
            Node n = d as Node;
            n.RectBounds = n.TransformRectBounds();
            n.Dispatcher.BeginInvoke(new Action(n.Node_LayoutUpdated), null);
            if (n.dview != null &&n.dview.DateTimeSettings.IsEnabled)
            {
                n.StartDateX = n.PxOffsetX.ToDateTime(n.dview.DateTimeSettings);
            }

            DiagramControl dc = DiagramPage.GetDiagramControl(d as Node);
            if (dc != null && !(d is Layer))
            {
                if (!(dc.View.undo || dc.View.redo || /*(dc.View.Page as DiagramPage).IsUnitChanged || */dc.View.IsMeasureCalled))
                {
                    dc.View.RedoStack.Clear();
                }
                if (/*!(dc.View.Page as DiagramPage).IsUnitChanged && */!dc.View.IsResizedRedone)
                {
                    (d as Node).oldx = (double)e.OldValue;// MeasureUnitsConverter.ToPixels((double)e.OldValue, (dc.View.Page as DiagramPage).MeasurementUnits);

                   
                }

                if (!dc.View.Redone && !dc.View.IsDragged && !dc.View.Undone && !dc.View.IsResized && !(d is Group) && !dc.View.IsMeasureCalled && !dc.View.IsLayout && !dc.View.IsMeasureCalled && !dc.View.IsLayout)// && !(dc.View.Page as DiagramPage).IsUnitChanged)
                {
                    if (dc.View.UndoRedoEnabled)
                    {
                        dc.View.UndoStack.Push((double)e.OldValue);//MeasureUnitsConverter.ToPixels((double)e.OldValue, (dc.View.Page as DiagramPage).MeasurementUnits));
                        dc.View.UndoStack.Push("offsetx");
                        dc.View.UndoStack.Push(d as Node);
                        dc.View.UndoStack.Push(dc.View.NodeDragCount);
                        dc.View.UndoStack.Push("Dragged");
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
            Node n = d as Node;
            n.RectBounds = n.TransformRectBounds();
            n.Dispatcher.BeginInvoke(new Action(n.Node_LayoutUpdated), null);
            if (n.dview != null && n.dview.DateTimeSettings.IsEnabled)
            {
                n.StartDateY = n.PxOffsetY.ToDateTime(n.dview.DateTimeSettings);
            }
            DiagramControl dc = DiagramPage.GetDiagramControl(d as Node);
            if (dc != null && !(d is Layer))
            {
                if (!(dc.View.undo || dc.View.redo || /*(dc.View.Page as DiagramPage).IsUnitChanged || */dc.View.IsMeasureCalled))
                {
                    dc.View.RedoStack.Clear();
                }
                if (/*!(dc.View.Page as DiagramPage).IsUnitChanged &&*/ !dc.View.IsResizedRedone)
                {
                    (d as Node).oldy = (double)e.OldValue;// MeasureUnitsConverter.ToPixels((double)e.OldValue, (dc.View.Page as DiagramPage).MeasurementUnits);
                }

                if (!dc.View.Redone && !dc.View.IsDragged && !dc.View.Undone && !dc.View.IsResized && !(d is Group) && !dc.View.IsMeasureCalled && !dc.View.IsLayout)// && !(dc.View.Page as DiagramPage).IsUnitChanged)
                {
                    if (dc.View.UndoRedoEnabled)
                    {
                        dc.View.UndoStack.Push((double)e.OldValue);//(MeasureUnitsConverter.ToPixels((double)e.OldValue, (dc.View.Page as DiagramPage).MeasurementUnits)));
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
            LabelRoutedEventArgs newEventArgs = new LabelRoutedEventArgs((string)e.OldValue, (string)e.NewValue, node);
            newEventArgs.RoutedEvent = DiagramView.NodeLabelChangedEvent;
            node.RaiseEvent(newEventArgs);
#if SyncfusionFramework3_5
            if (node.editor != null)
            {
                BindingExpression expression = node.editor.GetBindingExpression(LabelEditor.LabelProperty);
                if (expression != null)
                {
                    expression.UpdateTarget();
                }
            }
#endif
        }

        /// <summary>
        /// Calls Node_LayoutUpdated method of the instance, notifies of the sender value changes.
        /// </summary>
        public void Node_LayoutUpdated()//(object sender, EventArgs e)
        {
            if (this.Page != null)// && this.Page.Children.Contains(this))
            {
                Matrix mat = Matrix.Identity;
                mat.Rotate(this.RotateAngle);
                MatrixTransform trans = new MatrixTransform(mat);
                Point delta = trans.Transform(new Point(this.Width / 2, this.Height / 2));
                this.PxPosition = new Point(this.PxOffsetX + delta.X, this.PxOffsetY + delta.Y); //this.TransformToAncestor(this.Page as Panel).Transform(new Point(this.Width / 2, this.Height / 2));
            }

            //if (this.LabelWidth == 0 && editor != null)
            //{
            //    this.LabelWidth = editor.TextWidth;
            //    isdefaulted = true;
            //}

            //if (isdefaulted)
            //{
            //    this.LabelWidth = this.Width;
            //}

            //if (IsSelected && AllowRotate)
            //{
            //    if (!(this is Group) && rotatethumb != null)
            //    {
            //        //rotatethumb.Visibility = Visibility.Visible;
            //    }
            //    else if (grouprotatethumb != null)
            //    {
            //        grouprotatethumb.Visibility = Visibility.Visible;
            //    }
            //}
            //else
            //{
            //    if (!(this is Group))
            //    {
            //        if (rotatethumb != null)
            //        {
            //            //rotatethumb.Visibility = Visibility.Collapsed;
            //        }
            //    }
            //    else if (grouprotatethumb != null)
            //    {
            //        if (grouprotatethumb != null)
            //        {
            //            grouprotatethumb.Visibility = Visibility.Collapsed;
            //        }
            //    }
            //}
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

        internal void refreshBoundaries()
        {
            bool isNullContent = false;
            Shape temp = new Path();
            bool isPathFound = true;
            if (IntersectionMode == IntersectionMode.OnBorder)
            {
                isPathFound = false;
                //SetOnBorder(ref temp);
            }
            else if (IntersectionMode == IntersectionMode.OnContent)
            {
                if (Content != null)
                {
                    if (Content is Shape)
                    {
                        temp = (Content as Shape);
                    }
                    else
                    {
                        temp = null;
                    }
                }

                if (temp == null && Template != null)
                {
                    try
                    {
                        temp = Template.FindName("PART_Shape", this) as Shape;
                    }
                    catch (Exception e)
                    {
                        temp = null;
                        e.ToString();
                    }
                }
                else
                {
                    isNullContent = true;
                    Rectangle r = new Rectangle();
                    FrameworkElement element;
                    element = this as FrameworkElement;
                    if (element != null)
                    {
                        if (element.RenderSize.Width > 0.0)
                        {
                            r.Width = element.RenderSize.Width - element.Margin.Left - element.Margin.Right;
                            r.Height = element.RenderSize.Height - element.Margin.Bottom - element.Margin.Top;
                        }
                        Point pos = new Point(0, 0);
                        if (dview == null)
                        {
                            dview = GetDiagramView(this);
                        }
                        if (dview != null)
                        {
                            pos = element.TranslatePoint(new Point(0, 0), dview.Page);
                        }
                        temp = new Path();
                        Geometry g = new RectangleGeometry(new Rect(new Size(r.Width, r.Height)));
                        if (RotateAngle == 0)
                        {
                            (temp as Path).Data = Geometry.Combine(g, g, GeometryCombineMode.Intersect, new TranslateTransform(pos.X, pos.Y) as Transform);
                            Boundaries = temp;
                        }
                        else
                        {
                            g = new RectangleGeometry(new Rect(new Size(this.ActualWidth, ActualHeight)));
                            (temp as Path).Data = Geometry.Combine(g, g, GeometryCombineMode.Intersect, new TranslateTransform(0, 0) as Transform);
                        }
                        Boundaries = temp;
                    }
                }
                if (temp == null || (temp.RenderedGeometry.Bounds == Rect.Empty && !isNullContent))
                {
                    isPathFound = false;
                    //SetOnBorder(ref temp);
                }
            }
            if (!isPathFound)
            {
                Boundaries = null;
                return;
            }

            if (Boundaries != null && temp != null)
            {
                Transform t;
                Path path2 = new Path();
                t = new TranslateTransform(-temp.RenderedGeometry.Bounds.Left, -temp.RenderedGeometry.Bounds.Top);
                if (path2.RenderedGeometry != null)
                {
                    PathGeometry pg = Geometry.Combine(temp.RenderedGeometry, temp.RenderedGeometry, GeometryCombineMode.Intersect, t);
                    path2 = new Path();
                    path2.Data = pg;
                }
                if (path2.Data != null && (temp as Path) != null && (temp as Path).Data != null && (temp as Path).Data.Bounds != Rect.Empty)
                {
                    t = new TranslateTransform(-(temp as Path).Data.Bounds.Left, -(temp as Path).Data.Bounds.Top);
                    PathGeometry pg = Geometry.Combine((temp as Path).Data, (temp as Path).Data, GeometryCombineMode.Intersect, t);
                    path2 = new Path();
                    path2.Data = pg;
                }
                t = new ScaleTransform((Width - Margin.Left - Margin.Right - temp.Margin.Left - temp.Margin.Right) / path2.Data.Bounds.Size.Width,
                                    (Height - Margin.Top - Margin.Bottom - temp.Margin.Top - temp.Margin.Bottom) / path2.Data.Bounds.Size.Height);

                if (path2.Data != null)
                {
                    PathGeometry pg = Geometry.Combine(path2.Data, path2.Data, GeometryCombineMode.Intersect, t);
                    path2 = new Path();
                    path2.Data = pg;
                }

                t = new RotateTransform(RotateAngle,
                                       0,
                                        0);
                if (path2.Data != null)
                {
                    PathGeometry pg = Geometry.Combine(path2.Data, path2.Data, GeometryCombineMode.Intersect, t);
                    path2 = new Path();
                    path2.Data = pg;
                }
                if (dview != null && dview.Page != null)
                {
                    Point ofst = new Point(PxOffsetX, PxOffsetY);// MeasureUnitsConverter.ToPixels(new Point(LogicalOffsetX, LogicalOffsetY), (dview.Page as DiagramPage).MeasurementUnits);
                    t = new TranslateTransform(ofst.X + Margin.Left + temp.Margin.Left, ofst.Y + Margin.Top + temp.Margin.Top);
                }
                else
                {
                    t = new TranslateTransform(PxOffsetX + Margin.Left + temp.Margin.Left, PxOffsetY + Margin.Top + temp.Margin.Top);
                }
                if (path2.Data != null)
                {
                    PathGeometry pg = Geometry.Combine(path2.Data, path2.Data, GeometryCombineMode.Intersect, t);
                    path2 = new Path();
                    path2.Data = pg;
                }

                t = new ScaleTransform((path2.Data.Bounds.Width) / path2.Data.Bounds.Width,
                                    (path2.Data.Bounds.Height) / path2.Data.Bounds.Height, path2.Data.Bounds.Left + path2.Data.Bounds.Width / 2, path2.Data.Bounds.Top + path2.Data.Bounds.Height / 2);
                if (path2.Data != null)
                {
                    PathGeometry pg = Geometry.Combine(path2.Data, path2.Data, GeometryCombineMode.Intersect, t);
                    path2 = new Path();
                    path2.Data = pg;
                }
                Boundaries = path2;
            }
        }

        //private void SetOnBorder(ref Shape temp)
        //{
        //    Rectangle r = new Rectangle();
        //    FrameworkElement element;
        //    if (this.Content != null && this.Content is FrameworkElement)
        //    {
        //        element = this.Content as FrameworkElement;
        //        if (element.RenderSize.Width == 0 && element.RenderSize.Height == 0)
        //        {
        //            element = this as FrameworkElement;
        //        }
        //    }
        //    else
        //    {
        //        element = this as FrameworkElement;
        //    }

        //    if (element != null)
        //    {
        //        if (element.RenderSize.Width > 0.0)
        //        {
        //            r.Width = element.RenderSize.Width - element.Margin.Left - element.Margin.Right;
        //            r.Height = element.RenderSize.Height - element.Margin.Bottom - element.Margin.Top;
        //        }
        //        Point pos = new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
        //        if (dview == null)
        //        {
        //            dview = GetDiagramView(this);
        //        }
        //        if (dview != null)
        //        {
        //            pos = element.TranslatePoint(new Point(0, 0), dview.Page);
        //        }
        //        temp = new Path();
        //        Geometry g = new RectangleGeometry(new Rect(new Size(r.Width, r.Height)));
        //        if (RotateAngle == 0)
        //        {
        //            (temp as Path).Data = Geometry.Combine(g, g, GeometryCombineMode.Intersect, new TranslateTransform(pos.X, pos.Y) as Transform);
        //            Boundaries = temp;
        //            return true;
        //        }
        //        else
        //        {
        //            g = new RectangleGeometry(new Rect(new Size(this.ActualWidth, ActualHeight)));
        //            (temp as Path).Data = Geometry.Combine(g, g, GeometryCombineMode.Intersect, new TranslateTransform(0, 0) as Transform);
        //        }
        //        Boundaries = temp;
        //    }
        //    return false;
        //}

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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set 
            { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the old ZIndex value.
        /// </summary>
        /// <value>The old ZIndex value</value>
        public int OldZIndex
        {
            get { return m_oldindex; }
            set { m_oldindex = value; }
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
        /// Gets or sets the new ZIndex value.
        /// </summary>
        /// <value>The new ZIndex value</value>
        public int NewZIndex
        {
            get { return m_newindex; }
            set { m_newindex = value; }
        }

        #endregion

        #region IShape Members

        /// <summary>
        /// Gets or sets the Rank to which the node belongs to.
        /// </summary>
        internal int Rank
        {
            get { return m_rank; }
            set { m_rank = value; }
        }
        internal int countpno = 0;

        /// <summary>
        /// Gets or sets the parent nodes  based on the connections in a hierarchical layout.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CollectionExt Parents
        {
            get
            {
                return m_InNeighbors;
            }

            set
            {
                m_InNeighbors = value;
            }
        }
                
        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed; otherwise, <c>false</c>.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFixed
        {
            get { return mIsFixed; }
            set { mIsFixed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set { mIsExpanded = value; }
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
            get { return m_inEdges; }
        }

        /// <summary>
        /// Gets the collection of all outgoing edges, those for which this node
        /// is the source.
        /// </summary>
        /// <value>The OutEdges</value>
        public CollectionExt OutEdges
        {
            get { return m_outEdges; }
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
                return m_edges;
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IShape ParentNode
        {
            get
            {
                return mParentNode;
            }

            set
            {
                mParentNode = value;
            }
        }

        /// <summary>
        /// Gets or sets the edge between this node and its parent node in a tree
        /// structure.
        /// </summary>
        /// <value>The Parent edge</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEdge ParentEdge
        {
            get
            {
                return mParentEdge;
            }

            set
            {
                mParentEdge = value;
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Depth
        {
            get { return mDepth; }
            set { mDepth = value; }
        }

        /// <summary>
        /// Gets the number of tree children of this node.
        /// </summary>
        /// <value>The child count</value>
        public int ChildCount
        {
            get
            {
                if (this.Children == null)
                {
                    return 0;
                }
                else
                {
                    return this.Children.Count;
                }
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
                if (Model != null && Model.Nodes != null)
                {
                    int ind = Model.Nodes.IndexOf(this);

                    if (ind <= 0)
                    {
                        return null;
                    }
                    else
                    {
                        return (IShape)Model.Nodes[ind - 1];
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row count.</value>
        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column count.</value>
        public int Column
        {
            get { return col; }
            set { col = value; }
        }
        
        /// <summary>
        /// Gets or sets an iterator over this node's tree children.
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CollectionExt Children
        {
            get { return treeChildren; }
            set { treeChildren = value; }
        }

        ///// <summary>
        ///// Gets the System.Drawing.Rectangle.
        ///// </summary>
        //public System.Drawing.Rectangle Rectangle
        //{
        //    get { return mRectangle; }
        //}

        ///// <summary>
        ///// Gets the collection of this node's tree children's edges.
        ///// </summary>
        ///// <value></value>
        //public CollectionExt ChildEdges
        //{
        //    get
        //    {
        //        if (this.model != null && this.Model is IGraph && (this.Model as IGraph).Tree != null)
        //        {
        //            return (this.Model as IGraph).Tree.ChildEdges(this);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        /// <summary>
        /// Moves the node, the argument being the motion vector.
        /// </summary>
        /// <param name="p">The point p.</param>
        public void Move(System.Drawing.Point p)
        {
        }

        /// <summary>
        /// Gets the unique identifier of this node.
        /// </summary>
        /// <value>The unique identifier value.</value>
        public Guid ID
        {
            get { return m_id; }
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DiagramModel Model
        {
            get { return model; }
            set { model = value; }
        }

        ///// <summary>
        ///// Gets the collection of connectors.
        ///// </summary>
        ///// <value></value>
        //public CollectionExt Connectors
        //{
        //    get { return m_connectors; }
        //}

        bool load = false;

        internal bool IsInternallyLoaded
        {
            get { return load; }
            set { load = value; }
        }

        /// <summary>
        /// Gets the Node ID
        /// </summary>
        internal Node NodeID
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string FullName
        {
            get { return m_fullName; }
            set { m_fullName = value; }
        }

        
        /// <summary>
        /// Gets or sets the logical offset X. Used for internal calculation.
        /// </summary>
        /// <value>The logical offset X.</value>
        [Obsolete("Use OffsetX")]
        public double LogicalOffsetX
        {
            get
            {
                return (double)GetValue(LogicalOffsetXProperty);
            }

            set
            {
                SetValue(LogicalOffsetXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the logical offset Y. Used for internal calculation.
        /// </summary>
        /// <value>The logical offset Y.</value>
        [Obsolete("Use OffsetY")]
        public double LogicalOffsetY
        {
            get
            {
                return (double)GetValue(LogicalOffsetYProperty);
            }

            set
            {
                SetValue(LogicalOffsetYProperty, value);
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
                return oldoff;
            }

            set
            {
                oldoff = value;
            }
        }
        /// <summary>
        /// Gets or sets the offset X.
        /// </summary>
        /// <value>The offset X.</value>
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
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
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
                SetValue(OffsetXProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for OffsetX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(Node), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnLogicalOffsetXChanged)));


        /// <summary>
        /// Gets or sets the offset Y.
        /// </summary>
        /// <value>The offset Y.</value>
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
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
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
                SetValue(OffsetYProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for OffsetY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(Node), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnLogicalOffsetYChanged)));


        #endregion

      internal Point Transform(Point pt)
        {
            if (this.IsGrouped && (this.RenderTransform as RotateTransform).Angle!=0)
            {
                Matrix mat = Matrix.Identity;
                mat.RotateAt((this.RenderTransform as RotateTransform).Angle, PxOffsetX, PxOffsetY);
                return mat.Transform(new Point(PxOffsetX + pt.X, PxOffsetY + pt.Y));
            }
            else
            {
                Matrix mat = Matrix.Identity;
                mat.RotateAt(this.RotateAngle, PxOffsetX, PxOffsetY);
                return mat.Transform(new Point(PxOffsetX + pt.X, PxOffsetY + pt.Y));
            }
        }
     
        internal void SnapWidth(bool IsLeft)
        {
            if (this.dc != null && this.dc.View != null && this.dc.View.SnapToVerticalGrid && this.m_MouseResizing)
            {
                if (IsLeft)
                {
                    //double del = MeasureUnitsConverter.ToPixels(this.LogicalOffsetX, this.MeasurementUnits);
                    //this.LogicalOffsetX = MeasureUnitsConverter.FromPixels(Node.Round(this.m_TempPosition.X, dc.View.PxSnapOffsetX), this.MeasurementUnits);
                    //del -= MeasureUnitsConverter.ToPixels(this.LogicalOffsetX, this.MeasurementUnits);
                    if (this.m_TempSize.Width >= this.MinWidth && this.m_TempSize.Width<=this.MaxWidth)
                    {
                        double del = this.PxOffsetX;
                        this.PxOffsetX = Node.Round(this.m_TempPosition.X, dc.View.PxSnapOffsetX);
                        del -= this.PxOffsetX;
                        this.Width += del;
                    }
                }
                else
                {
                    //double x = MeasureUnitsConverter.ToPixels(this.LogicalOffsetX, this.MeasurementUnits);
                    if (this.m_TempSize.Width >= this.MinWidth && this.m_TempSize.Width <= this.MaxWidth)
                    {
                        double x = this.PxOffsetX;
                        this.Width = Node.Round(x + this.m_TempSize.Width, dc.View.PxSnapOffsetX) - x;
                    }
                }
            }
            else
            {
                if (IsLeft)
                {
                    //this.LogicalOffsetX = MeasureUnitsConverter.FromPixels(this.m_TempPosition.X, this.MeasurementUnits);
                    this.PxOffsetX = this.m_TempPosition.X;
                }
                if (this.m_TempSize.Width >= this.MinWidth && this.m_TempSize.Width <= this.MaxWidth)
                {
                    this.Width = this.m_TempSize.Width;
                }
            }
        }

        internal void SnapHeight(bool IsTop)
        {
            if (this.dc != null && this.dc.View != null && this.dc.View.SnapToHorizontalGrid && this.m_MouseResizing)
            {
                if (IsTop)
                {
                    if (this.m_TempSize.Height >= this.MinHeight && this.m_TempSize.Height<=this.MaxHeight)
                    {
                        double del = this.PxOffsetY;
                        this.PxOffsetY = Node.Round(this.m_TempPosition.Y, dc.View.PxSnapOffsetY);
                        del -= this.PxOffsetY;
                        this.Height += del;
                    }
                }
                else
                {
                    if (this.m_TempSize.Height >= this.MinHeight && this.m_TempSize.Height <= this.MaxHeight)
                    {
                        double y = this.PxOffsetY;
                        this.Height = Node.Round(y + this.m_TempSize.Height, dc.View.PxSnapOffsetY) - y;
                    }
                }
            }
            else
            {
                if (IsTop)
                {
                    this.PxOffsetY = this.m_TempPosition.Y;
                }
                if (this.m_TempSize.Height >= this.MinHeight && this.m_TempSize.Height <= this.MaxHeight)
                {
                    this.Height = this.m_TempSize.Height;
                }
            }
        }

        internal double PxOffsetX
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(OffsetX, this.MeasurementUnits);
            }

            set
            {
                
                OffsetX = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            }
        }

        internal double PxOffsetY
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(OffsetY, this.MeasurementUnits);
            }

            set
            {
                OffsetY = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            }
        }


        internal double _tempOffsetX;
        internal double _tempOffsetY;

        internal Point PxPosition
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(Position, this.MeasurementUnits);
            }

            set
            {
                Position = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            }
        }

        internal double PxWidth
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(this.Width, this.MeasurementUnits);
            }
            set
            {
                this.Width = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            }
        }

        internal double PxHeight
        {
            get
            {
                return MeasureUnitsConverter.ToPixels(this.Height, this.MeasurementUnits);
            }
            set
            {
                this.Height = MeasureUnitsConverter.FromPixels(value, this.MeasurementUnits);
            }
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

        public double RotateAngle { get; set; }
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

        internal double Right { get; set; }

        internal double Bottom { get; set; }

        internal Thickness ExtMargin { get; set; }

        internal double LeftExt { get { return Left - ExtMargin.Left; } }

        internal double TopExt { get { return Top - ExtMargin.Top; } }

        internal double RightExt { get { return Right + ExtMargin.Right; } }

        internal double BottomExt { get { return Bottom + ExtMargin.Bottom; } }

        internal Rect Bounds { get { return new Rect(new Point(Left, Top), new Point(Right, Bottom)); } }

        internal Rect BoundsExt { get { return new Rect(new Point(LeftExt, TopExt), new Point(RightExt, BottomExt)); } }

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
