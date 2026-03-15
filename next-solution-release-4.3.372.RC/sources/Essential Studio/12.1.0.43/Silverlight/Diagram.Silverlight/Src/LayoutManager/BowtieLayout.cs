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
    /// Represents the Directed Tree layout used for the automatic arrangement of nodes. The tree is created based on the Layout root specified.
    /// </summary>
    public class BowtieLayout : TreeLayoutBase
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
        /// Used to store the depths
        /// </summary>
        private float[] mdepths = new float[10];

        /// <summary>
        /// Used to store the temporary anchor point
        /// </summary>
        private Point mtempAnchorPoint;

        /// <summary>
        /// Used to store the anchor point
        /// </summary>
        private Point manchorPoint;

        /// <summary>
        /// Used to store the maximum depth
        /// </summary>
        private int mmaxDepth = 0;

        /// <summary>
        /// Used to store the default offset value
        /// </summary>
        private float moffsetValue = 50F;
                
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedTreeLayout"/> class.
        /// </summary>
        /// <param name="model">The model instance.</param>
        public BowtieLayout(DiagramModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedTreeLayout"/> class.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <param name="view">The view object.</param>
        public BowtieLayout(DiagramModel model, DiagramView view)
            : base(model, view)
        {
        }

        /// <summary>
        /// Occurs when [property changed].
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
                    this.mtempAnchorPoint = new Point(bounds.Right / 2F, bounds.Bottom / 2F);
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
        ///        DirectedTreeLayout tree = new DirectedTreeLayout();
        ///        tree.StartNodeArrangement();
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void StartNodeArrangement()
        {
                View.IsLayout = true;
                this.diagrampage = View.Page as DiagramPage;
                Units = this.diagrampage.MeasurementUnits;
                if (this.Model.Nodes.Count != 0)
                {
                    this.InitializeLayout();
                    this.DoLayout();
                }

                View.IsLayout = false;
#if WPF
                Model.dc.m_delayLayout = true;
#endif
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

                IShape root = this.LayoutRoot as IShape;
                LayoutInfo rp = this.equalities[root.ID.ToString()];

                this.DoFirstWalk(root, 0, 1);

                this.CheckDepths();

                this.DoSecondWalk(root, null, -rp.Prelim, 0);
                this.View.IsLayout = false;
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
        ///        DirectedTreeLayout tree = new DirectedTreeLayout();
        ///        tree.PrepareActivity(tree);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void PrepareActivity(ILayout layout)
        {
            layout.Model = Model;
#if WPF
            layout.Bounds = View.PxBounds;
            layout.LayoutBounds = View.PxLayoutBounds;
#endif
#if SILVERLIGHT
            layout.Bounds = View.Bounds;
#endif
            layout.Center = new Point(View.LayoutBounds.Right / 2, View.LayoutBounds.Bottom / 2);

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
        ///        DirectedTreeLayout tree=new DirectedTreeLayout();
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

            Model.BowtieOrientation = TreeOrientation.RightLeft;
            BowtieLayout layout = new BowtieLayout(this.Model, this.View);
            layout.RefreshPart();


            Model.BowtieOrientation = TreeOrientation.LeftRight;
            layout = new BowtieLayout(this.Model, this.View);
            layout.RefreshPart();

            //this.StartNodeArrangement();
            //(View.Page as DiagramPage).InvalidateMeasure();
            //(View.Page as DiagramPage).InvalidateArrange();
        }

        private void RefreshPart()
        {
            this.PrepareActivity(this);
            this.StartNodeArrangement();
            (this.Model as DiagramModel).CommonUpdate(null, this.Model.Nodes.OfType<IShape>().ToList(), this);
            (View.Page as DiagramPage).InvalidateMeasure();
            (View.Page as DiagramPage).InvalidateArrange();
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

        /// <summary>
        /// Gets the left node.
        /// </summary>
        /// <param name="shape">Node object</param>
        /// <returns>Adjacent left Node object</returns>
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
        /// Gets the right node.
        /// </summary>
        /// <param name="shape">Node object</param>
        /// <returns>Adjacent right Node object</returns>
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
        /// Allocates the space between nodes.
        /// </summary>
        /// <param name="v">The shape 1.</param>
        /// <param name="a">The shape 2.</param>
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
                        this.ShiftSubTree(this.AncestorShape(vim, v, a), v, shift);
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
        /// Gets the parent shape.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <param name="shape1">The shape1.</param>
        /// <param name="adjacentShape">The adjacent shape.</param>
        /// <returns>THe ancestor shape</returns>
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

        /// <summary>
        /// Checks the depth.
        /// </summary>
        private void CheckDepths()
        {
            for (int i = 1; i < this.mmaxDepth; ++i)
            {
                float vspace = 0;
                vspace = (float)this.Model.PxVerticalSpacing;

                if (this.Model.BowtieOrientation == TreeOrientation.BottomTop || this.Model.BowtieOrientation == TreeOrientation.RightLeft)
                {
                    vspace = -vspace;
                }

                this.mdepths[i] += this.mdepths[i - 1] + vspace;
            }
        }

        /// <summary>
        /// First traversal.
        /// </summary>
        /// <param name="shape">The Node object. </param>
        /// <param name="number">layout info number</param>
        /// <param name="depth">integer depth value</param>
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
                    layoutInfo.Prelim = 0;
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
        /// Second Traversal of the tree.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <param name="previousShape">The previous shape.</param>
        /// <param name="space">The space value.</param>
        /// <param name="depth">The depth value.</param>
        private void DoSecondWalk(IShape node, IShape previousShape, double space, int depth)
        {
            LayoutInfo np = this.equalities[node.ID.ToString()];
            if (this.Model.BowtieOrientation == TreeOrientation.LeftRight || this.Model.BowtieOrientation == TreeOrientation.RightLeft)
            {
                this.SetBreadthSpace(node, previousShape, this.mdepths[depth], depth);
                this.SetDepthSpace(node, previousShape, np.Prelim + space, depth);
            }
            else
            {
                this.SetBreadthSpace(node, previousShape, np.Prelim + space, depth);
                this.SetDepthSpace(node, previousShape, this.mdepths[depth], depth);
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
        /// Gets the space between nodes.
        /// </summary>
        /// <param name="l"> Left node object</param>
        /// <param name="r"> Right node object</param>
        /// <param name="siblings">Has siblings</param>
        /// <returns>The space between nodes</returns>
        private double GetSpace(IShape l, IShape r, bool siblings)
        {
            float hspace = 0;
            float stree = 0;
            bool w = this.Model.BowtieOrientation == TreeOrientation.TopBottom || this.Model.BowtieOrientation == TreeOrientation.BottomTop;

            hspace = (float)this.Model.PxHorizontalSpacing;
            stree = (float)this.Model.PxSpaceBetweenSubTrees;

            return (siblings ? hspace : stree) + (0.5 * (w ? ((l as Node)._Width + (r as Node)._Width) : ((l as Node)._Height + (r as Node)._Height)));
        }

        /// <summary>
        /// Initializes the  tree layout.
        /// </summary>
        /// <returns>true if initialization succeeds.</returns>
        private bool InitializeLayout()
        {
            this.Graph = this.Model as IGraph;
            if (Graph == null)
            {
                throw new Exception("Diagram Model is Empty");
            }
            if (Model.Nodes.Contains(Model.LayoutRoot))
            {
                this.LayoutRoot = this.Model.LayoutRoot;
            }
            Graph.ClearTraversing();

            Graph.MakeTraversing(LayoutRoot as IShape);

            if (Graph.Tree == null)
            {
                throw new Exception("RootLayout must be set");
            }

            this.equalities = new Dictionary<string, LayoutInfo>();
            if (Graph.Nodes.Count == 0)
            {
                return false;
            }

            if (Graph.Edges.Count == 0)
            {
                return false;
            }

            LayoutInfo par;

            foreach (IShape node in Graph.Nodes)
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
        /// <param name="shapeNext">This node object </param>
        /// <param name="previousShape">The previous node </param>
        /// <param name="space">The spacing value </param>
        /// <param name="depth">The depth value </param>
        private void SetBreadthSpace(IShape shapeNext, IShape previousShape, double space, int depth)
        {
            double a = 0;
            if (this.Model.BowtieOrientation == TreeOrientation.RightLeft)
            {
                if (shapeNext != LayoutRoot)
                {
                    a = this.manchorX + space - (depth * (shapeNext as Node)._Width);
                }
                else
                {
                    a = this.manchorX + space;
                }
            }
            else
            {
                a = this.manchorX + space;
            }

            this.SetTempOffX(shapeNext, previousShape, a);
        }

        /// <summary>
        /// Sets the depth space between each node.
        /// </summary>
        /// <param name="shapeNext">this node object</param>
        /// <param name="previousShape">previous node object</param>
        /// <param name="space">spacing value</param>
        /// <param name="depth">depth value</param>
        private void SetDepthSpace(IShape shapeNext, IShape previousShape, double space, int depth)
        {
            double a = 0;
            if (this.Model.BowtieOrientation == TreeOrientation.BottomTop)
            {
                if (shapeNext != LayoutRoot)
                {
                    a = this.manchorY + space - (depth * (shapeNext as Node)._Height);
                }
                else
                {
                    a = this.manchorY + space;
                }
            }
            else
            {
                a = this.manchorY + space;
            }

            this.SetTempOffY(shapeNext, previousShape, a);
        }

        /// <summary>
        /// Shifts the tree.
        /// </summary>
        /// <param name="shape">Node object</param>
        /// <param name="shape2">The shape2.</param>
        /// <param name="shift">The shift .</param>
        private void ShiftSubTree(IShape shape, IShape shape2, double shift)
        {
            LayoutInfo wmp = this.equalities[shape.ID.ToString()];
            LayoutInfo wpp = this.equalities[shape2.ID.ToString()];
            double distance = wpp.Number - wmp.Number;
            wpp.Change -= shift / distance;
            wpp.Shift += shift;
            wmp.Change += shift / distance;
            wpp.Prelim += shift;
            wpp.Mod += shift;
        }
       
        /// <summary>
        /// Translates the node position.
        /// </summary>
        /// <param name="shape">Shape object</param>
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
        /// Updates the depth of the tree.
        /// </summary>
        /// <param name="depth">Depth value </param>
        /// <param name="item">Node object </param>
        private void UpdateDepths(int depth, IShape item)
        {
            bool v = this.Model.BowtieOrientation == TreeOrientation.TopBottom || this.Model.BowtieOrientation == TreeOrientation.BottomTop;
            double d = v ? (item as Node)._Height : (item as Node)._Width;
            if (this.Model.BowtieOrientation == TreeOrientation.BottomTop || this.Model.BowtieOrientation == TreeOrientation.RightLeft)
            {
                d = -d;
            }

            if (this.mdepths.Length <= depth)
            {
                Array.Resize<float>(ref this.mdepths, (int)(3 * depth / 2));
            }

            this.mdepths[depth] = (float)Math.Max(this.mdepths[depth], d);
            this.mmaxDepth = (int)Math.Max(this.mmaxDepth, depth);
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
}
