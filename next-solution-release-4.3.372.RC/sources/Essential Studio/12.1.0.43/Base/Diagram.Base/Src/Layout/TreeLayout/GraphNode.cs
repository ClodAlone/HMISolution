#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Graph node.
    /// </summary>
    public class GraphNode
        : GraphNodeBase
    {
        #region Fields
        /// <summary>
        /// Tag for related parameters.
        /// </summary>
        private object m_tag;

        /// <summary>
        /// Hashtable containing parents relations.
        /// </summary>
        private ArrayList m_hashParents;

        /// <summary>
        /// Hashtable containing children relations.
        /// </summary>
        private ArrayList m_hashChildren;

        private float distance = 0.0f;
        private bool m_bVisited = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphNode"/> class.
        /// </summary>
        /// <param name="nodeGraph">The node graph.</param>
        public GraphNode(Node nodeGraph)
            : base(nodeGraph)
        { 
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the distance
        /// </summary>
        public float Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the node has been traversed or not.
        /// </summary>
        public bool Visited
        {
            get { return m_bVisited; }
            set { m_bVisited = value; }
        }

        /// <summary>
        /// Gets or sets tag field.
        /// </summary>
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        /// <summary>
        /// Gets or sets this node's parents.
        /// </summary>
        public ArrayList Parents
        {
            get
            {
                if (m_hashParents == null)
                    m_hashParents = new ArrayList();

                return m_hashParents;
            }
            set
            {
                if (m_hashParents != value)
                    m_hashParents = value;
            }
        }

        /// <summary>
        /// Gets or sets this node's children.
        /// </summary>
        public ArrayList Children
        {
            get
            {
                if (m_hashChildren == null)
                    m_hashChildren = new ArrayList();

                return m_hashChildren;
            }
            set
            {
                if (m_hashChildren != value)
                    m_hashChildren = value;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Gets all connection to other nodes.
        /// </summary>
        /// <returns>The node connections.</returns>
        public ArrayList GetAllConnections()
        {
            ArrayList list = new ArrayList(this.Parents);
            list.AddRange(this.Children);

            return list;
        }
        #endregion
    }

    /// <summary>
    /// Base class for GraphNodes.
    /// </summary>
    public abstract class GraphNodeBase
        : IGBounds,
          IGTransform
    {
        private bool DEF_AUTO_UPDATE = false;

        #region Class members
        private RectangleF m_rcBounds;
        private PointF m_ptPinPoint;
        private SizeF m_szSize;
        #endregion

        #region Initialize / Finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphNodeBase"/> class.
        /// </summary>
        /// <param name="node">The node. If set null node will mark as fictitious.</param>
        public GraphNodeBase(Node node)
        {
            // if( node == null )
            //     throw new ArgumentNullException( "node", "This parameter cannot be null!" );
            m_node = node;
            m_nodeUnitIndependent = node;

            m_strFullName = (node != null) ? node.FullName : "FictitiousNode";
            InitializeProperties(node);
        }
        #endregion

        #region Fields
        /// <summary>
        /// Name of node.
        /// </summary>
        private string m_strFullName;
        private IUnitIndependent m_nodeUnitIndependent;

        /// <summary>
        /// SymbolBase derived object.
        /// </summary>
        private Node m_node;

        /// <summary>
        /// Internal usage flag.
        /// Indicates whether node has been already added to typeOrder hashtable in Graph.
        /// </summary>
        private bool m_bAdded = false;
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Append bounds and size changes to kept node.
        /// </summary>
        public void ApplyChanges()
        {
            IUnitIndependent node = m_nodeUnitIndependent;

            if (node != null)
            {
                MeasureUnits units = MeasureUnits.Pixel;

                node.SetSize(m_szSize, units);
                node.SetPinPoint(m_ptPinPoint, units);
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Cache node position node size properties.
        /// </summary>
        /// <param name="node">The node.</param>
        private void InitializeProperties(IUnitIndependent node)
        {
            if (node != null)
            {
                MeasureUnits units = MeasureUnits.Pixel;

                m_rcBounds = node.GetBoundingRectangle(units, false);
                m_ptPinPoint = node.GetPinPoint(units);
                m_szSize = node.GetSize(units);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of node.
        /// </summary>
        public string FullName
        {
            get
            {
                return m_strFullName;
            }
        }

        /// <summary>
        /// Gets SymbolBase derived object.
        /// </summary>
        public Node Node
        {
            get
            {
                return m_node;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node has been already added to typeOrder hashtable in Graph.
        /// Internal usage flag.
        /// </summary>
        public bool Added
        {
            get
            {
                return m_bAdded;
            }
            set
            {
                m_bAdded = value;
            }
        }
        #endregion

        #region IGBounds Members
        /// <summary>
        /// Gets node bounding rectangle.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                if (DEF_AUTO_UPDATE)
                {
                    return m_nodeUnitIndependent.GetBoundingRectangle(MeasureUnits.Pixel, false);
                }

                return m_rcBounds;
            }
        }

        /// <summary>
        /// Gets or sets node location point.
        /// </summary>
        public PointF Location
        {
            get 
            { 
                return new PointF(this.Bounds.X, this.Bounds.Y); 
            }
            set
            {
                if (this.Location != value && m_nodeUnitIndependent != null)
                {
                    RectangleF rectBounds = this.Bounds;

                    SizeF szOffset = SizeF.Empty;
                    szOffset.Width = value.X - rectBounds.X;
                    szOffset.Height = value.Y - rectBounds.Y;

                    if (DEF_AUTO_UPDATE)
                        m_ptPinPoint = m_nodeUnitIndependent.GetPinPoint(MeasureUnits.Pixel);

                    m_ptPinPoint.X += szOffset.Width;
                    m_ptPinPoint.Y += szOffset.Height;

                    if (DEF_AUTO_UPDATE)
                        m_nodeUnitIndependent.SetPinPoint(m_ptPinPoint, MeasureUnits.Pixel);

                    // update bounds
                    m_rcBounds.Offset(szOffset.ToPointF());
                }
            }
        }

        /// <summary>
        /// Gets or sets location X coordinate.
        /// </summary>
        public float X
        {
            get 
            { 
                return this.Location.X; 
            }
            set
            {
                if (this.Location.X != value)
                {
                    RectangleF rectBounds = this.Bounds;

                    SizeF szOffset = SizeF.Empty;
                    szOffset.Width = value - rectBounds.X;

                    if (DEF_AUTO_UPDATE)
                        m_ptPinPoint = m_nodeUnitIndependent.GetPinPoint(MeasureUnits.Pixel);

                    m_ptPinPoint.X += szOffset.Width;

                    if (DEF_AUTO_UPDATE)
                        m_nodeUnitIndependent.SetPinPoint(m_ptPinPoint, MeasureUnits.Pixel);

                    m_rcBounds.Offset(szOffset.ToPointF());
                }
            }
        }

        /// <summary>
        /// Gets or sets location Y coordinate.
        /// </summary>
        public float Y
        {
            get 
            { 
                return this.Location.Y; 
            }
            set
            {
                if (this.Location.Y != value)
                {
                    RectangleF rectBounds = this.Bounds;

                    SizeF szOffset = SizeF.Empty;
                    szOffset.Height = value - rectBounds.Y;

                    if (DEF_AUTO_UPDATE)
                        m_ptPinPoint = m_nodeUnitIndependent.GetPinPoint(MeasureUnits.Pixel);

                    m_ptPinPoint.Y += szOffset.Height;

                    if (DEF_AUTO_UPDATE)
                        m_nodeUnitIndependent.SetPinPoint(m_ptPinPoint, MeasureUnits.Pixel);

                    m_rcBounds.Offset(szOffset.ToPointF());
                }
            }
        }

        /// <summary>
        /// Gets or sets node size.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return m_szSize;
            }
            set
            {
                if (this.Size != value)
                {
                    m_szSize = value;
                    m_rcBounds.Size = m_szSize;

                    if (m_nodeUnitIndependent != null)
                    {
                        m_nodeUnitIndependent.SetSize(value, MeasureUnits.Pixel);
                        m_rcBounds = m_nodeUnitIndependent.GetBoundingRectangle(MeasureUnits.Pixel, false);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets node width.
        /// </summary>
        public float Width
        {
            get 
            { 
                return this.Size.Width; 
            }
            set
            {
                SizeF szCur = this.Size;

                if (m_nodeUnitIndependent != null && szCur.Width != value)
                {
                    this.Size = new SizeF(value, szCur.Height);
                }
            }
        }

        /// <summary>
        /// Gets or sets node height.
        /// </summary>
        public float Height
        {
            get 
            { 
                return this.Size.Height; 
            }
            set
            {
                SizeF szCur = this.Size;

                if (m_nodeUnitIndependent != null && szCur.Height != value)
                {
                    this.Size = new SizeF(szCur.Width, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets node center point.
        /// </summary>
        public PointF Center
        {
            get 
            { 
                return new PointF(this.X + this.Width / 2, this.Y + this.Height / 2); 
            }
            set
            {
                if (m_nodeUnitIndependent != null)
                {
                    SizeF szOffset = SizeF.Empty;
                    szOffset.Width = value.X - (this.X + this.Width / 2);
                    szOffset.Height = value.Y - (this.Y + this.Height / 2);

                    if (DEF_AUTO_UPDATE)
                        m_ptPinPoint = m_nodeUnitIndependent.GetPinPoint(MeasureUnits.Pixel);

                    m_ptPinPoint.X += szOffset.Width;
                    m_ptPinPoint.Y += szOffset.Height;

                    if (DEF_AUTO_UPDATE)
                        m_nodeUnitIndependent.SetPinPoint(m_ptPinPoint, MeasureUnits.Pixel);

                    // update bounds
                    m_rcBounds.Offset(szOffset.ToPointF());
                }
            }
        }
        #endregion

        #region IGTransform Members
        /// <summary>
        /// Moves Graph node to a new location.
        /// </summary>
        /// <param name="dx">New location X coordinate.</param>
        /// <param name="dy">New location Y coordinate.</param>
        public void Translate(float dx, float dy)
        {
            this.X = dx;
            this.Y = dy;
        }

        /// <summary>
        /// Rotates the node a specified number of degrees about a given
        /// anchor point.
        /// </summary>
        /// <param name="ptAnchor">anchor point.</param>
        /// <param name="fDegree">rotation degree.</param>
        public void RotateAt(PointF ptAnchor, float fDegree)
        {
            Matrix matrix = new Matrix();
            matrix.RotateAt(fDegree, ptAnchor);
            PointF[] pts = { new PointF(this.Center.X, this.Center.Y) };
            matrix.TransformPoints(pts);
            this.Center = new PointF(pts[0].X, pts[0].Y);
        }

        /// <summary>
        /// Rotates the node a specified number of degrees.
        /// </summary>
        /// <param name="fDegree">Rotation degree.</param>
        public void Rotate(float fDegree)
        {
            if (this.Node != null)
            {
                ApplyChanges();
                this.Node.Rotate(fDegree);
            }
        }
        #endregion
    }
}
