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
    using System.Linq;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Represents base abstract class for the tree layout .
    /// </summary>
    public abstract class LayoutBase : ILayout
    {
        /// <summary>
        /// Used to store the default run span
        /// </summary>
        private const int DefaultSpan = 400;

        /// <summary>
        /// Used to store the graph
        /// </summary>
        private IGraph graph;

        /// <summary>
        /// Used to store the bounds
        /// </summary>
        private Thickness mbounds;

        /// <summary>
        ///  Used to store the center point.
        /// </summary>
        private Point mcenter;

        /// <summary>
        ///  Used to store the DiagramControl instance.
        /// </summary>
        private DiagramControl mcontroller;

        /// <summary>
        ///  Used to store the DiagramModel instance.
        /// </summary>
        private DiagramModel mmodel = null;

        /// <summary>
        /// Used to store the DiagramView instance.
        /// </summary>
        private DiagramView mview = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutBase"/> class.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <param name="view">The view object.</param>
        protected LayoutBase(DiagramModel model, DiagramView view)
            : base()
        {
            this.mmodel = model;
            this.mview = view;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutBase"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        protected LayoutBase(DiagramModel model)
            : base()
        {
            this.mmodel = model;
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public Thickness Bounds
        {
            get { return this.mbounds; }
            set { this.mbounds = value; }
        }

        Rect lbounds;
        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
      public Rect LayoutBounds
        {
            get { return this.lbounds; }
            set { this.lbounds = value; }
        }
        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        /// <value>The center.</value>
        public Point Center
        {
            get { return this.mcenter; }
            set { this.mcenter = value; }
        }

        /// <summary>
        /// Gets or sets the DiagramControl.
        /// Type:<see cref="DiagramControl"/>
        /// </summary>
        public DiagramControl Controller
        {
            get { return this.mcontroller; }
            set { this.mcontroller = value; }
        }

        /// <summary>
        /// Gets the collection of edges.
        /// Type:<see cref="CollectionExt"/>
        /// </summary>
        public CollectionExt Edges
        {
            get { return this.graph.Edges; }
        }

        /// <summary>
        /// Gets or sets the graph.
        /// </summary>
        /// <value>The graph.</value>
        public IGraph Graph
        {
            get { return this.graph; }
            set { this.graph = value; }
        }

        /// <summary>
        /// Gets or sets the Model property.
        /// Type:<see cref="DiagramModel"/>
        /// </summary>
        public DiagramModel Model
        {
            get { return this.mmodel; }
            set { this.mmodel = value; }
        }

        /// <summary>
        /// Gets the collection of nodes.
        /// Type:<see cref="CollectionExt"/>
        /// </summary>
        public CollectionExt Nodes
        {
            get { return this.graph.Nodes; }
        }

        /// <summary>
        /// Gets or sets the View property.
        /// Type:<see cref="DiagramView"/>
        /// </summary>
        public DiagramView View
        {
            get { return this.mview; }
            set { this.mview = value; }
        }

        /// <summary>
        /// Gets the default run span.
        /// </summary>
        /// <value>The default run span.</value>
        protected int DefaultRunSpan
        {
            get { return DefaultSpan; }
        }

        /// <summary>
        /// Sets the OffsetX value for the IShape item.
        /// </summary>
        /// <param name="item">IShape item.</param>
        /// <param name="referrer">The IShape referrer</param>
        /// <param name="x">Double value.</param>
        protected void SetX(IShape item, IShape referrer, double x)
        {
            (item as Node).PxOffsetX = x;
        }

        /// <summary>
        ///  Sets the OffsetY value.
        /// </summary>
        /// <param name="item">IShape item.</param>
        /// <param name="referrer">The IShape referrer.</param>
        /// <param name="y">Double Value.</param>
        protected void SetY(IShape item, IShape referrer, double y)
        {
            (item as Node).PxOffsetY = y;
        }
        protected void SetTempOffX(IShape item, IShape referrer, double x)
        {
            (item as Node)._tempOffsetX = x;
        }
        protected void SetTempOffY(IShape item, IShape referrer, double y)
        {
            (item as Node)._tempOffsetY = y;
        }

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor

        #endregion

        #region Methods

        #endregion
    }
}
