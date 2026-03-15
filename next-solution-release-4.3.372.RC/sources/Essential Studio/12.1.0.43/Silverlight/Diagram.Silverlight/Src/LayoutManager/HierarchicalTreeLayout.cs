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

    /// <summary>
    /// Represents the Hierarchical Tree layout. 
    /// </summary>
    /// <remarks>
    /// The Hierarchical Tree Layout also arranges nodes in a tree-like structure however unlike the directed tree layout, the nodes in hierarchical layout may have multiple parents. As a result, there is no need to specify the layout root. 
    /// Nodes can have multiple parents in this layout.
    /// </remarks>
    /// <example>
    /// <code language="XAML">
    ///&lt;UserControl x:Class=&quot;Sample.MainPage&quot;
    /// xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; 
    ///     xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot; 
    ///     xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight&quot;
    ///    xmlns:vsm=&quot;clr-namespace:System.Windows;assembly=System.Windows&quot; 
    ///             &gt;
    ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
    ///                                IsSymbolPaletteEnabled="True" 
    ///                                Background="WhiteSmoke"&gt;
    ///             &lt;syncfusion:DiagramControl.Model&gt;
    ///                   &lt;syncfusion:DiagramModel LayoutType="HierarchicalTreeLayout" HorizontalSpacing="30" VerticalSpacing="50" SpaceBetweenSubTrees="50" x:Name="diagramModel" &gt;
    ///                  &lt;/syncfusion:DiagramModel&gt;
    ///             &lt;/syncfusion:DiagramControl.Model&gt;
    ///           &lt;syncfusion:DiagramControl.View&gt;
    ///              &lt;syncfusion:DiagramView   Background="LightGray"  
    ///                                        Bounds="0,0,12,12"  
    ///                                        Name="diagramView"  &gt;
    ///              &lt;/syncfusion:DiagramView&gt;
    ///          &lt;/syncfusion:DiagramControl.View&gt;
    ///     &lt;/syncfusion:DiagramControl&gt;
    /// &lt;/Window&gt;
    /// </code>
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
    ///        Node n = new Node(Guid.NewGuid(), "Start");
    ///        n.Shape = Shapes.FlowChart_Start;
    ///        n.Label = "Start";
    ///        n.Level = 1;
    ///        n.Width = 150;
    ///        n.Height = 75;
    ///        Node n1 = new Node(Guid.NewGuid(), "Decision1");
    ///        n1.Shape = Shapes.FlowChart_Process;
    ///        n1.IsLabelEditable = true;
    ///        n1.Label = "Alarm Rings";
    ///        n1.Level = 2;
    ///        n1.Width = 150;
    ///        n1.Height = 75;
    ///        Model.Nodes.Add(n);
    ///        Model.Nodes.Add(n1);
    ///        Model.LayoutType = LayoutType.HierarchicalTreeLayout;
    ///        Model.HorizontalSpacing = 30;
    ///        Model.VerticalSpacing = 50;
    ///        Model.SpaceBetweenSubTrees = 50;
    ///        LineConnector o = new LineConnector();
    ///        o.ConnectorType = ConnectorType.Straight;
    ///        o.TailNode = n1;
    ///        o.HeadNode = n;
    ///        o.HeadDecoratorShape = DecoratorShape.None;
    ///        Model.Connections.Add(o);
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    public class HierarchicalTreeLayout : TreeLayoutBase
    {
        /// <summary>
        /// Used to store the units.
        /// </summary>
        private static MeasureUnits units;

        /// <summary>
        /// Used to store the page.
        /// </summary>
        private DiagramPage diagrampage;

        /// <summary>
        /// Used to store the equalities.
        /// </summary>
        private Dictionary<string, LayoutInfo> equalities;

        /// <summary>
        /// Used to store the anchor x value.
        /// </summary>
        private double manchorX;

        /// <summary>
        /// Used to store the anchor y value.
        /// </summary>
        private double manchorY;

        /// <summary>
        /// Used to store the children.
        /// </summary>
        private CollectionExt mchilds = new CollectionExt();

        /// <summary>
        /// Used to store the depths
        /// </summary>
        private float[] mdepths = new float[10];

        /// <summary>
        /// Used to store the parents
        /// </summary>
        private CollectionExt mparents = new CollectionExt();

        /// <summary>
        /// Used to store the temporary anchor point
        /// </summary>
        private Point mtempAnchorPoint;

        /// <summary>
        /// Used to store the anchor point
        /// </summary>
        private Point manchorPoint;

        /// <summary>
        /// Used to store the maximum rank.
        /// </summary>
        private int maxrank = -1;

        /// <summary>
        /// Used to store the minimum rank.
        /// </summary>
        private int minrank = -1;

        /// <summary>
        /// Used to store the maximum depth
        /// </summary>
        private int mmaxDepth = 0;

        /// <summary>
        /// Used to store the default offset value
        /// </summary>
        private float moffsetValue = 50F;
       
        /// <summary>
        /// Used to store the virtual node.
        /// </summary>
        private Node virtualnode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalTreeLayout"/> class.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <param name="view">The view object.</param>
        public HierarchicalTreeLayout(DiagramModel model, DiagramView view)
            : base(model, view)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalTreeLayout"/> class.
        /// </summary>
        /// <param name="model">The model instance.</param>
        public HierarchicalTreeLayout(DiagramModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Property Changed Event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        internal static MeasureUnits Units
        {
            get { return units; }
            set { units = value; }
        }

        /// <summary>
        /// Gets or sets the layout anchor.
        /// </summary>
        /// <value>The layout anchor.</value>
        internal Point LayoutAnchor
        {
            get
            {
                this.mtempAnchorPoint = new Point(0, 0);
                if (Graph != null)
                {
                    Thickness bounds = Bounds;
                    if (this.Model.Orientation == TreeOrientation.BottomTop)
                    {
                        this.mtempAnchorPoint = new Point(bounds.Right / 2F, bounds.Bottom / 2F);
                    }
                    else if (this.Model.Orientation == TreeOrientation.LeftRight)
                    {
                        this.mtempAnchorPoint = new Point(bounds.Right / 2F, bounds.Bottom / 2F);
                    }
                    else if (this.Model.Orientation == TreeOrientation.RightLeft)
                    {
                        this.mtempAnchorPoint = new Point(bounds.Right / 2F, bounds.Bottom / 2F);
                    }
                    else
                    {
                        this.mtempAnchorPoint = new Point(bounds.Right / 2F, bounds.Bottom / 2F);
                    }
                }

                return this.mtempAnchorPoint;
            }

            set
            {
                this.manchorPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the root node offset.
        /// </summary>
        /// <value>The root node offset.</value>
        internal float RootNodeOffset
        {
            get { return this.moffsetValue; }
            set { this.moffsetValue = value; }
        }             

        /// <summary>
        /// Generates the tree layout.
        /// </summary>
        public void DoLayout()
        {
            try
            {
                this.mdepths.Initialize();

                this.mmaxDepth = 0;

                Point a = this.LayoutAnchor;
                this.manchorX = a.X;
                this.manchorY = a.Y;

                IShape root = LayoutRoot as IShape;
                LayoutInfo rp = this.equalities[root.ID.ToString()];

                this.DoFirstWalk(root, 0, 1);

                this.CheckDepths();

                this.DoSecondWalk(root, null, -rp.Prelim, 0);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Prepares for the activity
        /// </summary>
        /// <param name="layout">ILayout instance.</param>
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.HierarchicalTreeLayout;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        LineConnector o = new LineConnector();
        ///        o.ConnectorType = ConnectorType.Straight;
        ///        o.TailNode = n1;
        ///        o.HeadNode = n;
        ///        o.HeadDecoratorShape = DecoratorShape.None;
        ///        Model.Connections.Add(o);
        ///        HierarchicalTreeLayout tree=new HierarchicalTreeLayout();
        ///        tree.PrepareActivity(tree);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void PrepareActivity(ILayout layout)
        {
            layout.Model = Model;

            layout.Bounds = View.PxBounds;
            layout.Center = new Point(View.PxBounds.Right / 2, View.PxBounds.Bottom / 2);
            if (View != null && View.PxBounds.Equals(new Thickness(0, 0, 0, 0)) && View.Scrollviewer != null)

            {
                layout.Bounds = new Thickness(0, 0, View.Scrollviewer.ActualWidth, View.Scrollviewer.ActualHeight);
                layout.Center = new Point((float)View.Scrollviewer.ActualWidth / 2, (float)View.Scrollviewer.ActualHeight / 2);
            }
        }

        /// <summary>
        /// Refreshes the layout.
        /// </summary>
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.HierarchicalTreeLayout;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        HierarchicalTreeLayout tree=new HierarchicalTreeLayout();
        ///        tree.RefreshLayout();
        ///        LineConnector o = new LineConnector();
        ///        o.ConnectorType = ConnectorType.Straight;
        ///        o.TailNode = n1;
        ///        o.HeadNode = n;
        ///        o.HeadDecoratorShape = DecoratorShape.None;
        ///        Model.Connections.Add(o);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void RefreshLayout()
        {
            this.PrepareActivity(this);
            this.StartNodeArrangement();
            (this.Model as DiagramModel).CommonUpdate(null, this.Model.Nodes.OfType<IShape>().ToList(), this);
            (this.View.Page as DiagramPage).InvalidateMeasure();
            (this.View.Page as DiagramPage).InvalidateArrange();
#if WPF
            //this.View._contentHolder.InvalidateMeasure();
#endif
        }

        /// <summary>
        /// Starts arranging the nodes.
        /// </summary>
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
        ///        Node n = new Node(Guid.NewGuid(), "Start");
        ///        n.Shape = Shapes.FlowChart_Start;
        ///        n.Label = "Start";
        ///        n.Level = 1;
        ///        n.Width = 150;
        ///        n.Height = 75;
        ///        Node n1 = new Node(Guid.NewGuid(), "Decision1");
        ///        n1.Shape = Shapes.FlowChart_Process;
        ///        n1.IsLabelEditable = true;
        ///        n1.Label = "Alarm Rings";
        ///        n1.Level = 2;
        ///        n1.Width = 150;
        ///        n1.Height = 75;
        ///        Model.Nodes.Add(n);
        ///        Model.Nodes.Add(n1);
        ///        Model.LayoutType = LayoutType.HierarchicalTreeLayout;
        ///        Model.HorizontalSpacing = 30;
        ///        Model.VerticalSpacing = 50;
        ///        Model.SpaceBetweenSubTrees = 50;
        ///        LineConnector o = new LineConnector();
        ///        o.ConnectorType = ConnectorType.Straight;
        ///        o.TailNode = n1;
        ///        o.HeadNode = n;
        ///        o.HeadDecoratorShape = DecoratorShape.None;
        ///        Model.Connections.Add(o);
        ///        HierarchicalTreeLayout tree=new HierarchicalTreeLayout();
        ///        tree.StartNodeArrangement();
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void StartNodeArrangement()
        {
                this.View.IsLayout = true;
                foreach (Node node in Model.Nodes)
                {
                    node.Rank = -1;
                    node.State = 0;
                    //(node as IShape).Parents = new CollectionExt();
                    (node as IShape).Children = new CollectionExt();
                    node.m_LayoutDisconnected = false;
                }

                this.diagrampage = View.Page as DiagramPage;
                Units = this.diagrampage.MeasurementUnits;

                if (this.Model.Nodes.Count != 0)
                {
                    if (this.InitializeLayout())
                    {
                        this.DoLayout();
                    }

                    foreach (Node n in Graph.Nodes)
                    {
                        //if (n.Parents.Contains(this.virtualnode))
                        //{
                        //    n.Parents.Remove(this.virtualnode);
                        //}

                        foreach (LineConnector line in n.InEdges)
                        {
                            if ((line.HeadNode as Node) == this.virtualnode)
                            {
                                this.View.Isnodedeleted = true;
                                break;
                            }
                        }
                    }

                    this.Graph.Nodes.Remove(this.virtualnode);
                    this.Model.Nodes.Remove(this.virtualnode);
                    this.View.Page.Children.Remove(this.virtualnode);

                    List<IEdge> edges = this.virtualnode.OutEdges.OfType<IEdge>().ToList();
                    foreach (IEdge edge in edges)
                    {
                        this.View.Page.Children.Remove(edge as LineConnector);
                        this.Model.Connections.Remove(edge as LineConnector);
                    }

                    m_disHeight -= this.Model.PxVerticalSpacing;
                    m_disWidth -= this.Model.PxHorizontalSpacing;

                    double posx = LayoutAnchor.X - m_disWidth / 2;
                    double posy = LayoutAnchor.Y - m_disHeight / 2;
                    if (double.IsNaN(posx))
                    {
                        posx = LayoutAnchor.X;
                    }
                    if (double.IsNaN(posy))
                    {
                        posy = LayoutAnchor.Y;
                    }
                    if (double.IsInfinity(m_minY))
                    {
                        m_minY = 0;
                    }

                    if (double.IsInfinity(m_minX))
                    {
                        m_minX = 0;
                    }

                    foreach (Node n in Model.Nodes)
                    {
                        if (Model.Orientation == TreeOrientation.TopBottom)
                        {
                            if (n.m_LayoutDisconnected)
                            {
                                n._tempOffsetX = posx;
                                posx = posx + n._Width
                                    + this.Model.PxHorizontalSpacing;
                                n._tempOffsetY = m_maxY + this.Model.PxVerticalSpacing;
                            }
                            else
                            {
                                n._tempOffsetY -= 70;
                            }
                        }
                        else if (Model.Orientation == TreeOrientation.BottomTop)
                        {
                            if (n.m_LayoutDisconnected)
                            {
                                n._tempOffsetX = posx;
                                posx = posx + n._Width
                                    + this.Model.PxHorizontalSpacing;
                                n._tempOffsetY = 70 + m_minY - this.Model.PxVerticalSpacing;
                            }
                            else
                            {
                                n._tempOffsetY += (70 + shifty);
                            }
                        }
                        else if (Model.Orientation == TreeOrientation.LeftRight)
                        {
                            if (n.m_LayoutDisconnected)
                            {
                                n._tempOffsetY = posy;
                                posy = posy + n._Height
                                    + this.Model.PxVerticalSpacing;
                                n._tempOffsetX = m_maxX + this.Model.PxHorizontalSpacing;
                            }
                            else
                            {
                                n._tempOffsetX -= 100;
                            }
                        }
                        else
                        {
                            if (n.m_LayoutDisconnected)
                            {
                                n._tempOffsetY = posy;
                                posy = posy + n._Height
                                    + this.Model.PxVerticalSpacing;
                                n._tempOffsetX = 100 + m_minX - this.Model.PxHorizontalSpacing;
                            }
                            else
                            {
                                n._tempOffsetX += (100 + shiftx);
                            }
                        }
                    }
                }

                this.View.IsLayout = false;
#if WPF
                Model.dc.m_delayLayout = true;
#endif
        }

        private double m_disWidth = 0;
        private double m_disHeight = 0;
        private double shiftx = 0;
        private double shifty = 0;
        private double m_maxX = 0;
        private double m_maxY = 0;
        private double m_minX = double.PositiveInfinity;
        private double m_minY = double.PositiveInfinity;

        /// <summary>
        /// Called when [property changed].
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

        /// <summary>
        /// Gets the adjacent left node.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns>The Adjacent Left shape</returns>
        private IShape AdjacentLeft(IShape shape)
        {
            IShape tempshape = null;
            if (shape.IsExpanded)
            {
                tempshape = shape.FirstChild;
            }

            return tempshape != null ? tempshape : this.equalities[shape.ID.ToString()].Thread;
        }

        /// <summary>
        /// Gets the adjacent right node.
        /// </summary>
        /// <param name="shape">Current shape</param>
        /// <returns>The Adjacent Right shape</returns>
        private IShape AdjacentRight(IShape shape)
        {
            IShape tempShape = null;
            if (shape.IsExpanded)
            {
                tempShape = shape.LastChild;
            }

            return tempShape != null ? tempShape : this.equalities[shape.ID.ToString()].Thread;
        }

        /// <summary>
        /// Allocates the space.
        /// </summary>
        /// <param name="v">The previous sibling.</param>
        /// <param name="a">The current node.</param>
        /// <returns>The shape object</returns>
        private IShape AllocateSpace(IShape v, IShape a)
        {
            IShape w = (IShape)v.PreviousSibling;
            if (w != null)
            {
                IShape vip, vim, vop, vom;
                double sip, sim, sop, som;

                vip = vop = v;
                vim = w;
                vom = (IShape)vip.ParentNode.FirstChild;

                sip = this.equalities[vip.ID.ToString()].Mod;
                sop = this.equalities[vop.ID.ToString()].Mod;
                sim = this.equalities[vim.ID.ToString()].Mod;
                som = this.equalities[vom.ID.ToString()].Mod;
                LayoutInfo parms;
                IShape nr = this.AdjacentRight(vim);
                IShape nl = this.AdjacentLeft(vip);
                while (nr != null && nl != null)
                {
                    vim = nr;
                    vip = nl;
                    vom = this.AdjacentLeft(vom);
                    vop = this.AdjacentRight(vop);
                    parms = this.equalities[vop.ID.ToString()];
                    parms.Ancestor = v;
                    double shift = (this.equalities[vim.ID.ToString()].Prelim + sim) -
                        (this.equalities[vip.ID.ToString()].Prelim + sip) + this.GetSpace(vim, vip, false);
                    if (shift > 0)
                    {
                        if (this.AncestorShape(vim, v, a) != v)
                        {
                            this.ShiftSubTree(this.AncestorShape(vim, v, a), v, shift);
                        }

                        sip += shift;
                        sop += shift;
                    }

                    sim += this.equalities[vim.ID.ToString()].Mod;
                    sip += this.equalities[vip.ID.ToString()].Mod;
                    som += this.equalities[vom.ID.ToString()].Mod;
                    sop += this.equalities[vop.ID.ToString()].Mod;

                    nr = this.AdjacentRight(vim);
                    nl = this.AdjacentLeft(vip);
                }

                if (nr != null && this.AdjacentRight(vop) == null)
                {
                    LayoutInfo vopp = this.equalities[vop.ID.ToString()];
                    vopp.Thread = nr;
                    vopp.Mod += sim - sop;
                }

                if (nl != null && this.AdjacentLeft(vom) == null)
                {
                    LayoutInfo vomp = this.equalities[vom.ID.ToString()];
                    vomp.Thread = nl;
                    vomp.Mod += sip - som;
                    a = v;
                }
            }

            return a;
        }

        /// <summary>
        /// Gets the ancestor shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        /// <param name="shape1">The shape1 object.</param>
        /// <param name="adjacentShape">The adjacent shape.</param>
        /// <returns>The ancestor shape</returns>
        private IShape AncestorShape(IShape shape, IShape shape1, IShape adjacentShape)
        {
            IShape parentshape = (IShape)shape1.ParentNode;
            LayoutInfo shapeLayoutInfo = this.equalities[shape.ID.ToString()];
            if (shapeLayoutInfo.Ancestor != null && shapeLayoutInfo.Ancestor.ParentNode == parentshape)
            {
                return shapeLayoutInfo.Ancestor;
            }
            else
            {
                return adjacentShape;
            }
        }

        ///// <summary>
        ///// Assign parents depending upon the connections specified.
        ///// </summary>
        ///// <param name="shape">The shape.</param>
        //private void AssignParents(IShape shape)
        //{
        //    try
        //    {
        //        if (shape.InEdges.Count != 0)
        //        {
        //            foreach (LineConnector line in shape.InEdges)
        //            {
        //                if (!shape.Parents.Contains(line.HeadNode))
        //                {
        //                    if ((shape as Node).CanConnect || (line.HeadNode as Node).CanConnect)
        //                    {
        //                        //shape.Parents.Add(line.HeadNode);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        /// <summary>
        /// Assign rank to each node . The nodes get arranged based on the ranks assigned to them.
        /// </summary>
        /// <param name="shape">The shape.</param>
        private void AssignRank(IShape shape)
        {
            try
            {
                if (shape.HParents(this.virtualnode).Count > 0)
                {
                    int max = 0;
                    foreach (IShape parent in shape.HParents(this.virtualnode))
                    {
                        if (parent == this.virtualnode)
                        {
                            max = -1;
                        }
                        else
                            if ((parent as Node).Rank > max)
                            {
                                max = (parent as Node).Rank;
                            }
                    }

                    (shape as Node).Rank = max + 1;
                    foreach (IShape child in shape.OutNeighbors)
                    {
                        //this.AssignRank(child);
                        if (this.Model.EnableCycleDetection)
                        {
                            if ((child as Node).Level >= (shape as Node).Level)
                                this.AssignRank(child);
                        }
                        else
                        {
                            this.AssignRank(child);
                        }
                    }

                    if ((shape as Node).Rank > this.maxrank)
                    {
                        this.maxrank = (shape as Node).Rank;
                    }
                }
            }
            catch
            {
                MessageBox.Show("There exists a cyclic path in the given input. Please set EnableCycleDetection property of DiagramModel to true", "Cyclic Path detected", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Assigns the depth to the layout.
        /// </summary>
        private void CheckDepths()
        {
            for (int i = 1; i < this.mmaxDepth-1; ++i)
            {
                float vspace = 0;
                vspace = (float)this.Model.PxVerticalSpacing;

                if (this.Model.Orientation == TreeOrientation.BottomTop || this.Model.Orientation == TreeOrientation.RightLeft)
                {
                    vspace = -vspace;
                }

                this.mdepths[i] += this.mdepths[i - 1] + vspace;
            }
        }

        /// <summary>
        /// Checks if any cycle exists in the given input.
        /// </summary>
        /// <param name="shape">The current shape.</param>
        private void CycleCheck(IShape shape)
        {
            if (shape != null)
            {
                if ((shape as Node).State != 2)
                {
                    (shape as Node).State = 1;

                    if (shape.OutEdges.Count != 0)
                    {
                        foreach (LineConnector line in shape.OutEdges)
                        {
                            if ((line.TailNode as Node).State != 2)
                            {
                                if ((line.TailNode as Node).State == 0)
                                {
                                    this.CycleCheck(line.TailNode);
                                }
                                else
                                {
                                    (line.TailNode as Node).CanConnect = false;
                                    (shape as Node).CanConnect = false;
                                }
                            }
                        }

                        (shape as Node).State = 2;
                        IShape predecessor = this.GetPredecessor(shape);
                        this.CycleCheck(predecessor);
                    }

                    (shape as Node).State = 2;
                }
                else
                {
                    (shape as Node).State = 2;
                }
            }
        }

        /// <summary>
        /// Initial Traversal of the tree.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        /// <param name="number">The number.</param>
        /// <param name="depth">The depth.</param>
        private void DoFirstWalk(IShape shape, int number, int depth)
        {
            LayoutInfo layoutInfo = this.equalities[shape.ID.ToString()];

            layoutInfo.Number = number;
            this.UpdateDepths(depth, shape);

            bool isExpanded = shape.IsExpanded;
            if (shape.ChildCount == 0 || !isExpanded)
            {
                IShape l = (IShape)shape.PreviousSibling;
                if (l == null)
                {
                    bool w = this.Model.Orientation == TreeOrientation.TopBottom || this.Model.Orientation == TreeOrientation.BottomTop;
                    layoutInfo.Prelim = w?(shape as Node)._Width/2:(shape as Node)._Height/2;
                }
                else
                {
                    layoutInfo.Prelim = this.equalities[l.ID.ToString()].Prelim + this.GetSpace(l, shape, true);
                }
            }
            else if (isExpanded)
            {
                IShape leftMostShape = shape.FirstChild;
                IShape rightMostShape = shape.LastChild;
                IShape defaultAncestor = leftMostShape;
                IShape c = leftMostShape;

                for (int i = 0; c != null; ++i, c = c.NextSibling)
                {
                    Debug.WriteLine(c.Name);
                    this.DoFirstWalk(c, i, depth + 1);
                    defaultAncestor = this.AllocateSpace(c, defaultAncestor);
                }

                this.TranslateShapePosition(shape);

                double midpoint = 0.5 *
                    (this.equalities[leftMostShape.ID.ToString()].Prelim + this.equalities[rightMostShape.ID.ToString()].Prelim);

                IShape left = (IShape)shape.PreviousSibling;
                if (left != null)
                {
                    layoutInfo.Prelim = this.equalities[left.ID.ToString()].Prelim + this.GetSpace(left, shape, true);
                    layoutInfo.Mod = layoutInfo.Prelim - midpoint;
                }
                else
                {
                    layoutInfo.Prelim = midpoint;
                }
            }
        }

        /// <summary>
        /// Second traversal of the tree . Assigns the position to the nodes.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <param name="previousShape">The previous shape.</param>
        /// <param name="space">The space object.</param>
        /// <param name="depth">The depth value.</param>
        private void DoSecondWalk(IShape node, IShape previousShape, double space, int depth)
        {
            LayoutInfo np = this.equalities[node.ID.ToString()];
            if (this.Model.Orientation == TreeOrientation.LeftRight || this.Model.Orientation == TreeOrientation.RightLeft)
            {
                this.SetBreadthSpace(node, previousShape, this.mdepths[(node as Node).Rank], depth);
                this.SetDepthSpace(node, previousShape, np.Prelim -(node as Node)._Height/2+space, depth);
            }
            else
            {
                this.SetBreadthSpace(node, previousShape, np.Prelim-(node as Node)._Width/2+space , depth);
                this.SetDepthSpace(node, previousShape, this.mdepths[(node as Node).Rank], depth);
            }

            if (node.IsExpanded)
            {
                depth += 1;

                for (IShape c = node.FirstChild; c != null; c = c.NextSibling)
                {
                    this.DoSecondWalk(c, node, space + np.Mod, depth);
                }
            }

            np.ClearInfo();
        }

        /// <summary>
        /// Gets the predecessor of the current shape.
        /// </summary>
        /// <param name="shape">The current shape.</param>
        /// <returns>The predecessor</returns>
        private IShape GetPredecessor(IShape shape)
        {
            foreach (LineConnector line in shape.InEdges)
            {
                if ((line.HeadNode as Node).State == 1)
                {
                    return line.HeadNode;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the space.
        /// </summary>
        /// <param name="l">The left shape.</param>
        /// <param name="r">The right shape.</param>
        /// <param name="siblings">if set to <c>true</c> [siblings].</param>
        /// <returns>The spacing between nodes</returns>
        private double GetSpace(IShape l, IShape r, bool siblings)
        {
            float hspace = 0;
            float stree = 0;
            bool w = this.Model.Orientation == TreeOrientation.TopBottom || this.Model.Orientation == TreeOrientation.BottomTop;
            hspace = (float)this.Model.PxHorizontalSpacing;
            stree = (float)this.Model.PxSpaceBetweenSubTrees;
            return (siblings ? hspace : stree) + (0.5 * (w ? ((l as Node)._Width + (r as Node)._Width) : ((l as Node)._Height + (r as Node)._Height)));
        }

        /// <summary>
        /// Initializes the layout.
        /// </summary>
        /// <returns>Boolen value true if initialization succeeded; false otherwise.</returns>
        private bool InitializeLayout()
        {
            this.Graph = this.Model as IGraph;

            this.virtualnode = new Node();
            this.virtualnode.Content = "VirtualNode";

            this.virtualnode.Width = 100;
            this.virtualnode.Height = 50;
            this.virtualnode.Level = 2;
            this.virtualnode.Rank = 0;
            this.Model.Nodes.Insert(0, this.virtualnode);

            foreach (IShape node in Graph.Nodes)
            {
                ////node.Parents.Clear();
                if (this.Model.EnableCycleDetection)
                {
                    if (node != this.virtualnode && (node as Node).State != 2)
                    {
                        this.CycleCheck(node);
                    }
                }

                var k = Model.Nodes.OfType<Node>().Where(n => n.Parents.Count == 0).ToList();

                //this.AssignParents(node);

                if (node.HParents(this.virtualnode).Count == 0 && node != this.virtualnode)
                {
                    if (node.OutEdges.Count > 0)
                    {
                        LineConnector line = new LineConnector();
                        line.HeadNode = virtualnode;
                        line.TailNode = node;

                        (node as Node).Rank = 1;
                        Model.Connections.Add(line);
                        //node.Parents.Add(virtualnode);
                        (node as Node).m_LayoutDisconnected = false;
                    }
                    else
                    {
                        (node as Node).m_LayoutDisconnected = true;

                        double width = (node as Node)._Width + this.Model.PxHorizontalSpacing;
                        double height = (node as Node)._Height + this.Model.PxVerticalSpacing;

                        m_disWidth = m_disWidth + width;
                        m_disHeight = m_disHeight + height;
                        shiftx = shiftx > width ? shiftx : width;
                        shifty = shifty > height ? shifty : height;
                    }
                }

                this.AssignRank(node);
                try
                {
                    if ((node as Node).InEdges.Count == 1 && (node as Node).OutEdges.Count == 0)
                    {
                        foreach (LineConnector line in node.InEdges)
                        {
                            if (line.HeadNode == this.virtualnode)
                            {
                                (node as Node).Rank = this.maxrank + 1;
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("There exists a cyclic path in the given input. Please set EnableCycleDetection property of DiagramModel to true", "Cyclic Path detected", MessageBoxButton.OK);
                }
            }

            if (Graph == null)
            {
                throw new Exception("Diagram Model is Empty");
            }

            this.LayoutRoot = this.virtualnode;
            this.Graph.ClearTraversing();
            this.Graph.MakeTraversing(LayoutRoot as IShape);

            if (this.Graph.Tree == null)
            {
                throw new Exception("RootLayout must be set");
            }

            this.equalities = new Dictionary<string, LayoutInfo>();
            if (this.Graph.Nodes.Count == 0)
            {
                return false;
            }

            if (this.Graph.Edges.Count == 0)
            {
                return false;
            }

            int r = this.minrank;
            int v = this.maxrank;

            LayoutInfo par;

            foreach (IShape node in this.Graph.Nodes)
            {
                par = new LayoutInfo();
                par.SetupInfo(node);
                this.equalities.Add(node.ID.ToString(), par);
            }

            return true;
        }                    

        /// <summary>
        /// Sets the breadth space.
        /// </summary>
        /// <param name="shapeNext">The shape next.</param>
        /// <param name="previousShape">The previous shape.</param>
        /// <param name="space">The space value.</param>
        /// <param name="depth">The depth value.</param>
        private void SetBreadthSpace(IShape shapeNext, IShape previousShape, double space, int depth)
        {
            double a = 0;
            if (this.Model.Orientation == TreeOrientation.RightLeft)
            {
                if (shapeNext != LayoutRoot)
                {
                    int rank = (shapeNext as Node).Rank;
                    if ((shapeNext as Node).Rank == 0)
                    {
                        rank = -1;
                    }

                    //a = this.manchorX + space - (rank * (shapeNext as Node)._Width);
                }
                //else
                {
                    if ((shapeNext as Node).Rank == 0)
                    {
                        a = this.manchorX-(shapeNext as Node).Width+space;
                    }
                    else
                        a = this.manchorX + mdepths[depth - 1] - (shapeNext as Node).Width;
                }
            }
            else
            {
                if (Model.Orientation == TreeOrientation.LeftRight)
                {
                    int rank = (shapeNext as Node).Rank;
                    if ((shapeNext as Node).Rank == 0)
                    {
                        a = this.manchorX + space - (shapeNext as Node).Width;
                    }
                    else
                        a = this.manchorX + mdepths[depth - 1];
                }
                else
                    a = space + this.manchorX;
               
                //a = this.manchorX + space;
            }

            this.SetTempOffX(shapeNext, previousShape, a);
            m_maxX = m_maxX > a ? m_maxX : a;
            m_minX = m_minX < a ? m_minX : a;
        }

        /// <summary>
        /// Sets the depth space.
        /// </summary>
        /// <param name="shapeNext">The shape next.</param>
        /// <param name="previousShape">The previous shape.</param>
        /// <param name="space">The space.</param>
        /// <param name="depth">The depth.</param>
        private void SetDepthSpace(IShape shapeNext, IShape previousShape, double space, int depth)
        {
            double a = 0;
            if (this.Model.Orientation == TreeOrientation.BottomTop)
            {
                if (shapeNext != LayoutRoot)
                {
                    int rank = (shapeNext as Node).Rank;
                    if ((shapeNext as Node).Rank == 0)
                    {
                        rank = -1;
                    }

                   // a = this.manchorY + space - (rank * (shapeNext as Node)._Height);
                }
                //else
                {
                    if ((shapeNext as Node).Rank == 0)
                    {
                        a = this.manchorY - (shapeNext as Node).Height + space;
                    }
                    else
                        a = this.manchorY + mdepths[depth - 1] - (shapeNext as Node).Height;
                }
            }
            else
            {
                if (Model.Orientation == TreeOrientation.TopBottom)
                {
                    int rank = (shapeNext as Node).Rank;
                    if ((shapeNext as Node).Rank == 0)
                    {
                        a = this.manchorY + space - (shapeNext as Node).Height;
                    }
                    else
                        a = this.manchorY + mdepths[depth - 1];
                }
                else
                    a = space + this.manchorY;
            }

            SetTempOffY(shapeNext, previousShape, a);
            m_maxY = m_maxY > a ? m_maxY : a;
            m_minY = m_minY < a ? m_minY : a;
        }

        /// <summary>
        /// Shifts the sub tree.
        /// </summary>
        /// <param name="shape">Shape1 object</param>
        /// <param name="shape2">Shape2 object</param>
        /// <param name="shift">Shift value</param>
        private void ShiftSubTree(IShape shape, IShape shape2, double shift)
        {
            LayoutInfo wmp = this.equalities[shape.ID.ToString()];
            LayoutInfo wpp = this.equalities[shape2.ID.ToString()];
            double distance = wpp.Number - wmp.Number;
            if (distance == 0)
            {
                distance = 1;
            }

            wpp.Change -= shift / distance;
            wpp.Shift += shift;
            wmp.Change += shift / distance;
            wpp.Prelim += shift;
            wpp.Mod += shift;
        }
     
        /// <summary>
        /// Translates the nodes .
        /// </summary>
        /// <param name="shape">Current shape</param>
        private void TranslateShapePosition(IShape shape)
        {
            double shift = 0, change = 0;
            for (IShape c = shape.LastChild; c != null; c = c.PreviousSibling)
            {
                LayoutInfo cp = this.equalities[c.ID.ToString()];
                cp.Prelim += shift;
                cp.Mod += shift;
                change += cp.Change;
                shift += cp.Shift + change;
            }
        }

        /// <summary>
        /// Updates the depths.
        /// </summary>
        /// <param name="depth">The depth value.</param>
        /// <param name="item">The item value.</param>
        private void UpdateDepths(int depth, IShape item)
        {
            bool v = this.Model.Orientation == TreeOrientation.TopBottom || this.Model.Orientation == TreeOrientation.BottomTop;
            double d = v ? (item as Node)._Height : (item as Node)._Width;
            if (this.Model.Orientation == TreeOrientation.BottomTop || this.Model.Orientation == TreeOrientation.RightLeft)
            {
                d = -d;


                if (this.mdepths.Length < (3 * this.maxrank / 2))
                {
                    Array.Resize<float>(ref this.mdepths, (int)(3 * this.maxrank / 2));
                }

                this.mdepths[depth - 1] = (float)Math.Min(this.mdepths[depth - 1], d);
            }
            else
            {
                if (this.mdepths.Length < (3 * this.maxrank / 2))
                {
                    Array.Resize<float>(ref this.mdepths, (int)(3 * this.maxrank / 2));
                }

                this.mdepths[(item as Node).Rank] = (float)Math.Max(this.mdepths[depth-1], d);
            }
            this.mmaxDepth = (int)Math.Max(this.mmaxDepth, this.maxrank + 2);
        }

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Initialization
        
        #endregion

        #region INotifyPropertyChanged Members
        
        #endregion

        #region Methods

        #endregion
    }


    internal static class IShapeExtension
    {
        internal static List<Node> HParents(this IShape shape, IShape virtualNode)
        {
            Node n = shape as Node;
            if (n != null)
            {
                return n.Parents.OfType<Node>().Where(parent => ((n.CanConnect || parent.CanConnect) && parent != virtualNode)).ToList<Node>();
            }
            else
            {
                return new List<Node>();
            }
        }
    }
}
