#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Text;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// This layout manager is used to draw organizational layouts.
    /// </summary>
    public class OrgChartLayoutManager : DirectedTreeLayoutManager
    {
        #region Class Members
        private RotateDirection m_Rotate;
        private int m_TreeLevel = 0;
        private bool m_SingleChildLayout = false;
        #endregion

        #region Class contructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OrgChartLayoutManager"/> class.
        /// </summary>
        public OrgChartLayoutManager()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgChartLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        public OrgChartLayoutManager(Model model, float fHorizontalOffset, float fVerticalOffset)
            : base(model, 0, fHorizontalOffset, fVerticalOffset)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgChartLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="direction">The angular orientation of the tree.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        public OrgChartLayoutManager(Model model, RotateDirection direction, float fHorizontalOffset, float fVerticalOffset)
            : base(model, ((float)direction), fHorizontalOffset, fVerticalOffset)
        {
            m_Rotate = direction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgChartLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="direction">The angular orientation of the tree.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        /// <param name="layoutType">Specifies the layout type of tree</param>
        /// <param name="nTreeLevel">Specifies the tree level upto which the default layout has to be maintained</param>
        public OrgChartLayoutManager(Model model, RotateDirection direction, float fHorizontalOffset, float fVerticalOffset, LayoutType layoutType, int nTreeLevel)
            : base(model, ((float)direction), fVerticalOffset, fHorizontalOffset, layoutType, nTreeLevel)
        {
            m_Rotate = direction;
            m_LayoutType = layoutType;
            if (layoutType != LayoutType.Horizontal)
                m_TreeLevel = nTreeLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgChartLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="direction">The angular orientation of the tree.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        /// <param name="layoutType">Specifies the layout type of tree</param>
        /// <param name="nTreeLevel">Specifies the tree level upto which the default layout has to be maintained</param>
        /// <param name="bSingleChildLayout">Determines whether the layout needs to done on single child node</param>
        public OrgChartLayoutManager(Model model, RotateDirection direction, float fHorizontalOffset, float fVerticalOffset, LayoutType layoutType, int nTreeLevel, bool bSingleChildLayout)
            : base(model, ((float)direction), fVerticalOffset, fHorizontalOffset, layoutType, nTreeLevel, bSingleChildLayout)
        {
            m_Rotate = direction;
            m_LayoutType = layoutType;
            if (layoutType != LayoutType.Horizontal)
                m_TreeLevel = nTreeLevel;

            m_SingleChildLayout = bSingleChildLayout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgChartLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fRotation">The angular orientation of the tree.</param>
        /// <param name="fHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        /// <param name="fVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        public OrgChartLayoutManager(Model model, float fRotation, float fHorizontalOffset, float fVerticalOffset)
            : base(model, fRotation, fHorizontalOffset, fVerticalOffset)
        {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the rotate direction.
        /// </summary>
        public RotateDirection RotateDirection
        {
            get
            {
                return m_Rotate;
            }
            set
            {
                m_Rotate = value;
                base.RotationAngle = (float)m_Rotate;
            }
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Applies the directed tree layout strategy on the diagram.
        /// </summary>
        protected override void DoGraphLayout()
        {
            base.DoGraphLayout();

            IEndPointContainer container;

            Graph graph = this.SelectedNode as Graph;
            ArrayList lstToSort = graph.TypeOrdered;
            
            // Set the value of m_TreeLevel to total number of levels when its orginal value is 0.
            if (m_TreeLevel == 0 || m_LayoutType != LayoutType.Waterfall)
                m_TreeLevel = lstToSort.Count;

            // update connector heading
            foreach (Node node in this.Nodes)
            {
                container = node as IEndPointContainer;

                if (container == null)
                    continue;

                if (!(container is OrgLineConnector))
                {
                    container = ReplaceConnector(container);
                }
                else
                {
                    OrgLineConnector orgLine = container as OrgLineConnector;
                    if (orgLine == null)
                    {
                        container = ReplaceConnector(container);
                        orgLine = container as OrgLineConnector;
                        continue;

                    }

                    Node toNode = orgLine.ToNode as Node;
                    int nCounter = 0;
                    
                    // Get the level or rank to which the toNode belong.
                    foreach (ArrayList lstSort in lstToSort)
                    {
                        ArrayList alRankedNode = new ArrayList();
                        foreach (GraphNode gn in lstSort)
                        {
                            alRankedNode.Add(gn.Node);
                        }
                        if (alRankedNode.Contains(toNode))
                            break;
                        nCounter++;
                    }

                    if (nCounter >= lstToSort.Count)
                        continue;
                    
                    if (nCounter <= m_TreeLevel)
                    {
                        CompassHorizontalTree(ref container);
                    }
                    else
                    {
                        CompassVerticalTree(ref container);
                    }
                }

            }
        }

        /// <summary>
        /// Sets the HeadingHead and HeadingTail for the connectors in Horizontal tree
        /// </summary>
        /// <param name="container">The container</param>
        private void CompassHorizontalTree(ref IEndPointContainer container)
        {
            OrgLineConnector orgLine = container as OrgLineConnector;
            switch (m_Rotate)
            {
                case RotateDirection.BottomToTop:
                    orgLine.HeadingHead = CompassHeading.South;
                    orgLine.HeadingTail = CompassHeading.North;
                    break;
                case RotateDirection.LeftToRight:
                    orgLine.HeadingHead = CompassHeading.West;
                    orgLine.HeadingTail = CompassHeading.East;
                    break;
                case RotateDirection.RightToLeft:
                    orgLine.HeadingHead = CompassHeading.East;
                    orgLine.HeadingTail = CompassHeading.West;
                    break;
                case RotateDirection.TopToBottom:
                    orgLine.HeadingHead = CompassHeading.North;
                    orgLine.HeadingTail = CompassHeading.South;
                    break;
                default:
                    orgLine.HeadingHead = CompassHeading.None;
                    orgLine.HeadingTail = CompassHeading.None;
                    break;
            }
        }

        /// <summary>
        /// Sets the HeadingHead and HeadingTail for the connectors in vertical tree
        /// </summary>
        /// <param name="container">The container</param>
        private void CompassVerticalTree(ref IEndPointContainer container)
        {
            OrgLineConnector orgLine = container as OrgLineConnector;
            Node fromNode = orgLine.FromNode as Node;
            bool IsSingleChild =false;
            if (fromNode != null)
            {
               IsSingleChild = (fromNode.EdgesLeaving.Count == 1) && m_SingleChildLayout;
            }
            switch (m_Rotate)
            {
                case RotateDirection.BottomToTop:
                    orgLine.HeadingHead = CompassHeading.South;
                    orgLine.HeadingTail = (IsSingleChild ? CompassHeading.North : CompassHeading.East);
                    break;
                case RotateDirection.LeftToRight:
                    orgLine.HeadingHead = CompassHeading.West;
                    orgLine.HeadingTail = (IsSingleChild ? CompassHeading.East : CompassHeading.South);
                    break;
                case RotateDirection.RightToLeft:
                    orgLine.HeadingHead = CompassHeading.East;
                    orgLine.HeadingTail = (IsSingleChild ? CompassHeading.West : CompassHeading.South);
                    break;
                case RotateDirection.TopToBottom:
                    orgLine.HeadingHead = (IsSingleChild ? CompassHeading.North : CompassHeading.West);
                    orgLine.HeadingTail = CompassHeading.South;
                    break;
                default:
                    orgLine.HeadingHead = CompassHeading.None;
                    orgLine.HeadingTail = CompassHeading.None;
                    break;
            }
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// Replace the current connector to OrgLineconnector.
        /// </summary>
        /// <param name="container">The end point container.</param>
        /// <returns>Replaced orthogonal connector.</returns>
        private IEndPointContainer ReplaceConnector(IEndPointContainer container)
        {
            Node node = container as Node;
            LineBase line = container as LineBase;

            if (node == null)
                return container;

            // safe node name and parent
            string name = node.Name;
            ICompositeNode parent = node.Parent;

            PointF ptStart = node.BoundingRectangle.Location;
            PointF ptEnd = new PointF(node.BoundingRectangle.Right, node.BoundingRectangle.Bottom);

            ConnectionPoint headPort = container.HeadEndPoint.Port;
            ConnectionPoint tailPort = container.TailEndPoint.Port;

            // remove itself from parent
            int index = this.Nodes.IndexOf(node);
            parent.RemoveChild(node);
            this.Nodes.Remove(node);

            // create orthogonal connector
            // OrthogonalConnector ortho = new OrthogonalConnector( ptStart, ptEnd );
            OrgLineConnector ortho = new OrgLineConnector(ptStart, ptEnd);
            ortho.Name = name;

            // add orthogonal connector to parent node
            this.Nodes.Insert(index, ortho);
            parent.AppendChild(ortho);
            (parent as Model).SendToBack(ortho);

            // customize connector
            if (line != null)
            {
                ortho.HeadDecorator.DecoratorShape = line.HeadDecorator.DecoratorShape;
                ortho.TailDecorator.DecoratorShape = line.TailDecorator.DecoratorShape;
            }

            // connect connector
            if (headPort != null)
            {
                headPort.TryConnect(ortho.HeadEndPoint);
            }

            if (tailPort != null)
            {
                tailPort.TryConnect(ortho.TailEndPoint);
            }

            return ortho;
        }
        #endregion
    }

    #region Enum
    /// <summary>
    /// The four different rotation direction of the organization chart.
    /// </summary>
    public enum RotateDirection
    {
        /// <summary>
        /// Layout from Bottom to Top.
        /// </summary>
        BottomToTop = 180,

        /// <summary>
        /// Layout from Left to Right.
        /// </summary>
        LeftToRight = 270,

        /// <summary>
        /// Layout from Right to Left.
        /// </summary>
        RightToLeft = 90,

        /// <summary>
        /// Layout from Top to Bottom.
        /// </summary>
        TopToBottom = 0,
    }
    #endregion
}
