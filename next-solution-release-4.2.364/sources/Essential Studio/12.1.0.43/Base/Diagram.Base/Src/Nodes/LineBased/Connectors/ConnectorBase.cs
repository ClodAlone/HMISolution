#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Template connector class that contains Head and Tail EndPoints, CompassHeadings, Segments and Bridges.
    /// </summary>
    [Serializable]
    public abstract class ConnectorBase
        : LineBase,
          IEndPointContainer,
          IGraphEdge
    {
        #region Class members
        /// <summary>
        /// Indicate that line bridging was updated in parent changing.
        /// </summary>
        private bool m_bParentChangingBridgeUpdate = false;

        /// <summary>
        /// Store bridge style.
        /// </summary>
        private BridgeStyle m_bridgeStyle;
        private bool m_bLineRoutingEnabled;
        private CompassHeading m_headingHead;
        private CompassHeading m_headingTail;
        private HeadEndPoint m_endPointHead;
        private TailEndPoint m_endPointTail;

        /// <summary>
        /// Collection of segments with bridges.
        /// </summary>
        protected ArrayList m_lineSegments;

        /// <summary>
        /// Flag used to enable line bridging.
        /// </summary>
        protected bool m_bLineBridgingEnabled;

        /// <summary>
        /// Line bridge size.
        /// </summary>
        protected float m_fLineBridgeSize;

        /// <summary>
        /// Defines connector state.
        /// </summary>
        private ConnectorState m_cntState;

        /// <summary>
        /// Default value of heading distance.
        /// </summary>
        private float m_fHeadingDistance = CommonUsedValues.DEF_ROUTE_DISTANCE;

        /// <summary>
        /// Flag what use for lock handle moving while EditStyle.AllowRotate = false;
        /// </summary>
        protected bool m_bLockHandleMove = true;

        /// <summary>
        /// Defines the edge of the connector
        /// </summary>
        private bool m_bEnableRoundedCorner;
        private bool m_bObstaclesInPath;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorBase"/> class.
        /// </summary>
        /// <param name="ptStart">The tail end point location.</param>
        /// <param name="ptEnd">The head end point location.</param>
        public ConnectorBase(PointF ptStart, PointF ptEnd)
            : base()
        {
            // Fixed D9480
            // if( ptStart == ptEnd )
            //    throw new ArgumentException( "ptStart equals ptEnd" );

            // enable line bridging by default
            m_bLineBridgingEnabled = true;

            InitEndPoints(ptStart, ptEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorBase"/> class.
        /// </summary>
        /// <param name="ptStart">The tail end point location.</param>
        /// <param name="ptEnd">The head end point location.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public ConnectorBase(PointF ptStart, PointF ptEnd, MeasureUnits measureUnits)
            : base()
        {
            if (ptStart == ptEnd)
                throw new ArgumentException("ptStart equals ptEnd");

            // enable line bridging by default
            m_bLineBridgingEnabled = true;

            ptStart = MeasureUnitsConverter.ToPixels(ptStart, measureUnits);
            ptEnd = MeasureUnitsConverter.ToPixels(ptEnd, measureUnits);

            InitEndPoints(ptStart, ptEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorBase"/> class.
        /// </summary>
        /// <param name="src">The source connector.</param>
        public ConnectorBase(ConnectorBase src)
            : base(src)
        {
            m_endPointHead = (HeadEndPoint)src.m_endPointHead.Clone();
            m_endPointHead.Container = this;

            m_endPointTail = (TailEndPoint)src.m_endPointTail.Clone();
            m_endPointTail.Container = this;

            m_headingHead = src.m_headingHead;
            m_headingTail = src.m_headingTail;

            m_bLineBridgingEnabled = src.m_bLineBridgingEnabled;
            m_bLineRoutingEnabled = src.m_bLineRoutingEnabled;
            m_fLineBridgeSize = src.m_fLineBridgeSize;
            m_bEnableRoundedCorner = src.m_bEnableRoundedCorner;
            m_bObstaclesInPath = src.m_bObstaclesInPath;
            // initialize necessary variables.
            InitConnector();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorBase"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ConnectorBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bool endpointheadpresent = true;
            bool endpointtailpresent = true;

            m_headingHead = CompassHeading.None;
            m_headingTail = CompassHeading.None;
            m_bLineBridgingEnabled = true;
            m_bLineRoutingEnabled = true;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "endPointHeadPresent":
                        endpointheadpresent = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "headingHead":
                        m_headingHead = (CompassHeading)entry.Value;
                        break;
                    case "headingTail":
                        m_headingTail = (CompassHeading)entry.Value;
                        break;
                    case "lineBridgingEnabled":
                        m_bLineBridgingEnabled = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "lineRoutingEnabled":
                        m_bLineRoutingEnabled = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "endPointTailPresent":
                        endpointtailpresent = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "EnableRoundedCorner":
                        m_bEnableRoundedCorner = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "ObstaclesInPath":
                        m_bObstaclesInPath = Boolean.Parse(entry.Value.ToString());
                        break;
                }
            }

            if (endpointheadpresent)
            {
                m_endPointHead = (HeadEndPoint)info.GetValue("endPointHead", (typeof(HeadEndPoint)));

                if (m_endPointHead != null)
                    m_endPointHead.Container = this;
            }

            if (endpointtailpresent)
            {
                m_endPointTail = (TailEndPoint)info.GetValue("endPointTail", (typeof(TailEndPoint)));

                if (m_endPointTail != null)
                    m_endPointTail.Container = this;
            }

            // initialize necessary variables.
            InitConnector();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the distance from heading to obstacle node.
        /// </summary>
        /// <value>The heading distance.</value>
        [DefaultValue(5f), Description("Gets or sets distance from heading to obstacle node.")]
        public float HeadingDistance
        {
            get 
            { 
                return m_fHeadingDistance; 
            }
            set
            {
                if (value > 0 && m_fHeadingDistance != value && OnPropertyChanging(this.FullContainerName, DPN.HeadingDistance, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.HeadingDistance);

                    m_fHeadingDistance = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.HeadingDistance);
                }
            }
        }

        /// <summary>
        /// Gets or sets bridge style.
        /// </summary>
        [
          DefaultValue(Diagram.BridgeStyle.Arc),
          Description("Gets or sets bridge style."),
          Category("Line Routing")
        ]
        public BridgeStyle BridgeStyle
        {
            get
            {
                return m_bridgeStyle;
            }
            set
            {
                if (m_bridgeStyle != value && OnPropertyChanging(this.FullContainerName, DPN.BridgeStyle, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.BridgeStyle);

                    m_bridgeStyle = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.BridgeStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether that line routing is enabled.
        /// </summary>
        [
        Browsable(true),
        Category("Line Routing"),
        Description("Gets or sets value indicates that line routing is enabled."),
        DefaultValue(false)
        ]
        public bool LineRoutingEnabled
        {
            get
            {
                return m_bLineRoutingEnabled;
            }
            set
            {
                if (m_bLineRoutingEnabled != value && OnPropertyChanging(this.FullContainerName, DPN.LineRoutingEnabled, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.LineRoutingEnabled);

                    // set new value
                    m_bLineRoutingEnabled = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.LineRoutingEnabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets the heading head.
        /// </summary>
        /// <value>The heading head.</value>
        public CompassHeading HeadingHead
        {
            get 
            { 
                return m_headingHead; 
            }
            set
            {
                if (m_headingHead != value && OnPropertyChanging(this.FullContainerName, DPN.HeadingHead, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.HeadingHead);

                    // set new value
                    m_headingHead = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.HeadingHead);
                }
            }
        }

        /// <summary>
        /// Gets or sets the heading tail.
        /// </summary>
        /// <value>The heading tail.</value>
        public CompassHeading HeadingTail
        {
            get 
            { 
                return m_headingTail; 
            }
            set
            {
                if (m_headingTail != value && OnPropertyChanging(this.FullContainerName, DPN.HeadingTail, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.HeadingTail);

                    // set new value
                    m_headingTail = value;

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.HeadingTail);
                }
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="ConnectorLineSegment"/> items.
        /// </summary>
        /// <value>The collection of <see cref="ConnectorLineSegment"/> items.</value>
        public ArrayList LineSegments
        {
            get
            {
                if (m_lineSegments == null)
                    m_lineSegments = new ArrayList();

                return m_lineSegments;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flip X is enable.
        /// </summary>
        /// <value><c>true</c> if node vertical flipped; otherwise, <c>false</c>.</value>
        public override bool FlipX
        {
            get 
            { 
                return base.FlipX; 
            }
            set
            {
                if (!IsConnected())
                    base.FlipX = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flip Y is enable.
        /// </summary>
        /// <value><c>true</c> if node horizontal flipped; otherwise, <c>false</c>.</value>
        public override bool FlipY
        {
            get 
            { 
                return base.FlipY; 
            }
            set
            {
                if (!IsConnected())
                    base.FlipY = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        /// <value>The rotation angle in range [ -180; 180 ].</value>
        /// <remarks>
        /// Rotation angle saved in rotation range [ -180; 180 ]
        /// But on set this property new rotation angle must be
        /// in range [ 0; 360 ]
        /// </remarks>
        public override float RotationAngle
        {
            get 
            { 
                return base.RotationAngle; 
            }
            set
            {
                if (!IsConnected())
                    base.RotationAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether line bridging is enabled for this connector only.
        /// </summary>
        /// <remarks>
        /// Used by BridgeManager to check if current 
        /// segment can contains bridges.
        /// </remarks>
        [
        Browsable(true),
        Category("Line Routing"),
        Description("Gets or sets value that line bridging is enabled."),
        DefaultValue(true)
        ]
        public bool LineBridgingEnabled
        {
            get 
            { 
                return m_bLineBridgingEnabled; 
            }
            set
            {
                if (m_bLineBridgingEnabled != value && OnPropertyChanging(this.FullContainerName, DPN.LineBridgingEnabled, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.LineBridgingEnabled);

                    // set new value
                    m_bLineBridgingEnabled = value;

                    // Update bridges.
                    if (m_mgrBridge != null)
                        m_mgrBridge.AddToIntersectCollection(this);

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.LineBridgingEnabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets the state of the connector.
        /// </summary>
        /// <value>The state of the connector.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ConnectorState ConnectorState
        {
            get { return m_cntState; }
            set { m_cntState = value; }
        }

        /// <summary>
        /// Gets or sets the edge type of the connector.
        /// </summary>
        /// <value>The edge of the connector.</value>
        [Browsable(true)]        
        public bool EnableRoundedCorner
        {
            get { return m_bEnableRoundedCorner; }
            set 
            {
                if (m_bEnableRoundedCorner != value && OnPropertyChanging(this.FullContainerName, DPN.EnableRoundedCorner, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.EnableRoundedCorner);

                    // set new value
                    m_bEnableRoundedCorner = value;

                    //Updates the path of the connector
                    if (this.PathPoints != null && this.PathPoints.Length > 0)
                        this.UpdateGraphicsPath(this.GetPathPoints());

                    // Update bridges.
                    if (m_mgrBridge != null)
                        m_mgrBridge.AddToIntersectCollection(this);

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.EnableRoundedCorner);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an obstacle present in the the path of connector.
        /// </summary>        
        [DefaultValue(false)]
        [Description("Indicates whether an obstacle present in the the path of connector")]
        internal bool ObstaclesInPath
        {
            get
            {
                return m_bObstaclesInPath;
            }
            set
            {
                if (m_bObstaclesInPath != value)
                    m_bObstaclesInPath = value;
            }
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Updates the connectors intersecting.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment.</param>
        /// <param name="nodes">The collection of <see cref="Node"/> items.</param>
        public void IntersectSegmentWith(int segmentIndex, NodeCollection nodes)
        {
            // Update segment bridges.
            ConnectorLineSegment segment = this.LineSegments[segmentIndex] as ConnectorLineSegment;

            if (segment == null)
                return;

            // get bounds in model coordinates
            RectangleF thisBounds = ((IUnitIndependent)this).GetBoundingRectangle(MeasureUnits.Pixel, true);

            // Get current segment points.
            PointF[] ptsSegment = new PointF[] { segment.Point1, segment.Point2 };

            // get this node transformation
            using (Matrix mtxNodeTransform = this.GetTransformations())
            {
                this.AppendFlipTransforms(mtxNodeTransform);
                mtxNodeTransform.TransformPoints(ptsSegment);
            }

            Matrix mtxInvertLine = HandlesHitTesting.GetParentsTransformations(this, false);
            mtxInvertLine.Invert();

            // reset native bridges
            segment.Bridges.Clear();

            ConnectorLineSegment lineSegment;
            RectangleF testBounds;
            bool updateLineNode;
            bool bHigher;

            foreach (ConnectorBase connectorBase in nodes)
            {
                if (connectorBase == null || this.Equals(connectorBase))
                    continue;

                updateLineNode = false;
                testBounds = ((IUnitIndependent)connectorBase).GetBoundingRectangle(MeasureUnits.Pixel, true);
                bHigher = ZOrderHigher(connectorBase, this);

                // if bounds don't intersect then only reset bridges
                if (!thisBounds.IntersectsWith(testBounds))
                {
                    // reset all intersect bridges
                    for (int nSgmIdx = 0, nLength = connectorBase.LineSegments.Count; nSgmIdx < nLength; nSgmIdx++)
                    {
                        lineSegment = connectorBase.GetLineSegmentAt(nSgmIdx) as ConnectorLineSegment;

                        if (lineSegment != null)
                            ResetSegmentBridges(lineSegment, this, segmentIndex);
                    }

                    // refresh line node
                    updateLineNode = true;
                }
                else
                {
                    PointF ptIntersect;
                    Bridge bridgeTemp;
                    PointF[] ptsCheckSegment = new PointF[2];

                    // generate line transformation to get segment points in current node coordinates.
                    // By transform connectorBase to model coordinates and then transform points to current node coordinates
                    Matrix lineNodeMtx = HandlesHitTesting.GetParentsTransformations(connectorBase, true);
                    lineNodeMtx.Multiply(mtxInvertLine, MatrixOrder.Append);

                    // check each line segment
                    for (int k = 0, nLengthForeign = connectorBase.LineSegments.Count - 1; k < nLengthForeign; k++)
                    {
                        lineSegment = (ConnectorLineSegment)connectorBase.GetLineSegmentAt(k);

                        // Reset bridges intersections.
                        updateLineNode = ResetSegmentBridges(lineSegment, this, segmentIndex);

                        // Get check segment points.
                        ptsCheckSegment[0] = lineSegment.Point1;
                        ptsCheckSegment[1] = lineSegment.Point2;
                        lineNodeMtx.TransformPoints(ptsCheckSegment);

                        // if zOrder lower zero this node removed from model and check for intersection
                        if (this.ZOrder >= 0 &&
                            CheckLineSegmentIntersect(ptsSegment[0], ptsSegment[1], ptsCheckSegment[0], ptsCheckSegment[1], out ptIntersect))
                        {
                            // update bridges if intersection found
                            if (this.LineBridgingEnabled && (!bHigher || !connectorBase.LineBridgingEnabled))
                            {
                                // update native bridges
                                float fBridgeOffset = (float)Geometry.PointDistance(ptsSegment[0], ptIntersect);
                                bridgeTemp = new Bridge(connectorBase, lineSegment, fBridgeOffset);

                                segment.Bridges.Add(bridgeTemp);
                            }
                            else if (connectorBase.LineBridgingEnabled)
                            {
                                float fBridgeOffset = (float)Geometry.PointDistance(ptsCheckSegment[0], ptIntersect);
                                bridgeTemp = new Bridge(this, segment, fBridgeOffset);

                                lineSegment.Bridges.Add(bridgeTemp);
                                updateLineNode = true;
                            }
                        }
                    }
                }

                // update fireing node's refresh rectangle
                // if it's LineBridgingEnabled and
                // it is higher in ZOrder than current node
                if (updateLineNode && bHigher && connectorBase.LineBridgingEnabled)
                {
                    // update foreign node's refresh rect
                    UpdateCallback delUC = (UpdateCallback)Delegate.CreateDelegate(typeof(UpdateCallback), connectorBase, "UpdateRefreshRect");

                    if (delUC != null)
                    {
                        delUC();
                    }
                }
            }
        }

        /// <summary>
        /// Draws the curve for the line
        /// </summary>
        /// <param name="startPoint">start point</param>
        /// <param name="endPoint"> end point</param>
        /// <param name="pixelCount"> curve radius</param>
        public void LengthenLine(PointF startPoint, ref PointF endPoint, float pixelCount)
        {
            if (startPoint.Equals(endPoint))
                return; // not a line

            double dx = endPoint.X - startPoint.X;
            double dy = endPoint.Y - startPoint.Y;
            if (dx == 0)
            {
                // vertical line:
                if (endPoint.Y < startPoint.Y)
                    endPoint.Y -= pixelCount;
                else
                    endPoint.Y += pixelCount;
            }
            else if (dy == 0)
            {
                // horizontal line:
                if (endPoint.X < startPoint.X)
                    endPoint.X -= pixelCount;
                else
                    endPoint.X += pixelCount;
            }
            else
            {
                // non-horizontal, non-vertical line:
                double length = Math.Sqrt(dx * dx + dy * dy);
                double scale = (length + pixelCount) / length;
                dx *= scale;
                dy *= scale;
                endPoint.X = startPoint.X + Convert.ToSingle(dx);
                endPoint.Y = startPoint.Y + Convert.ToSingle(dy);
            }
        }
        #endregion

        #region Class virtual methods
        /// <summary>
        /// Raise when node property is changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void EventSink_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            // Model model = this.Root;
            // string strPropertyName = evtArgs.PropertyName;

            // if ( model != null )
            // {
            //    model.BridgeManager.BeginUpdateIntersection();
            //    model.LinkManager.BeginSynchronization();

            //    // model.BeginUpdate();
            //    // synchronize head end point with port
            //    if ( strPropertyName == DPN.HeadingHead && this.HeadEndPoint != null )
            //    {
            //        model.LinkManager.SynchronizeEndPoint( this.HeadEndPoint );
            //    }
            //    // synchronize tail end point with port
            //    if ( strPropertyName == DPN.HeadingTail && this.TailEndPoint != null )
            //    {
            //        model.LinkManager.SynchronizeEndPoint( this.TailEndPoint );
            //    }
            //    model.BridgeManager.EndUpdateIntersection();
            //    model.LinkManager.EndSynchronization();
            //    // model.EndUpdate();
            // }
            base.EventSink_PropertyChanged(evtArgs);
             
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Determines whether the Model.Bridging is enable.
        /// </summary>
        /// <returns>
        /// <c>true</c> if model.bridging is enable; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsLineBridgingEnabled()
        {
            Model model = this.Root;
            return model != null && model.LineBridgingEnabled;
        }

        /// <summary>
        /// Check who node has higher ZOrder.
        /// </summary>
        /// <param name="higherNode">Possible higher node.</param>
        /// <param name="smallerNode">Possible smaller node.</param>
        /// <returns>If <b>true</b> higherNode is higher, elsewere  smallerNode is higher.</returns>
        private bool ZOrderHigher(Node higherNode, Node smallerNode)
        {
            bool bHigher = false;

            if (higherNode != null && smallerNode != null)
            {
                // check is higherNode ZOrder higher
                bHigher = higherNode.ZOrder > smallerNode.ZOrder;

                Node higherNodeParent = higherNode;
                Node smallerNodeParent = smallerNode;

                // check if nodes has difference parents
                while (higherNodeParent != null)
                {
                    // check higher node parents
                    while (smallerNodeParent != null)
                    {
                        if (smallerNodeParent.Parent == higherNodeParent.Parent && smallerNodeParent.Parent != null)
                        {
                            return smallerNodeParent.ZOrder < higherNodeParent.ZOrder;
                        }

                        smallerNodeParent = smallerNodeParent.Parent as Node;
                    }

                    smallerNodeParent = smallerNode;
                    higherNodeParent = higherNodeParent.Parent as Node;
                }
            }

            return bHigher;
        }

        /// <summary>
        /// Draws the line segment to specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to on.</param>
        /// <param name="segment">The segment.</param>
        protected void DrawSegment(Graphics gfx, ConnectorLineSegment segment)
        {
            Bridge bridge;
            float fOffsetHelper;
            float fLineBridgeSize = fOffsetHelper = GetLineBridgeSize();

            using (Pen penOutline = this.LineStyle.CreatePen())
            {
                if (segment.Bridges.Count > 0)
                {
                    segment.Bridges.Sort(new BridgeComparer());
                    PointF ptStart = segment.Point1;
                    PointF ptEnd = segment.Point2;
                    float fBridgeOffset;

                    for (int n = 0, nLength = segment.Bridges.Count; n < nLength; n++)
                    {
                        // Get bridge and it offset from start point.
                        bridge = (Bridge)segment.Bridges[n];
                        fBridgeOffset = (float)Geometry.PointDistance(segment.Point1, segment.Point2) - bridge.Offset;

                        // Get bridge start point concidering bridge size
                        ptEnd = Geometry.GetPointByOffset(ptStart, segment.Point2, fBridgeOffset + fLineBridgeSize / 2);

                        // Draw first line part.
                        if (ptStart != ptEnd)
                            gfx.DrawLine(penOutline, ptStart, ptEnd);

                        // Merge near bridges in one.
                        MergeNearBridges(ref n, bridge, segment, ref fOffsetHelper, ref fBridgeOffset);

                        // Find bridge rectangle
                        ptStart = ptEnd;
                        ptEnd = Geometry.GetPointByOffset(ptStart, segment.Point2, fBridgeOffset - fOffsetHelper / 2);
                        PointF ptCenter = Geometry.GetPointByOffset(ptStart, segment.Point2, fBridgeOffset);

                        if (bridge.ConnectorIntersecting.Visible)
                            RenderBridge(gfx, penOutline, ptStart, ptCenter, ptEnd);
                        else
                            gfx.DrawLine(penOutline, ptStart, ptEnd);

                        ptStart = ptEnd;
                        fOffsetHelper = fLineBridgeSize;
                    }

                    // Draw final line segment.
                    if (ptEnd != segment.Point2)
                    {
                        gfx.DrawLine(penOutline, ptEnd, segment.Point2);
                    }
                }
                else
                {
                    if (segment.Point1 != segment.Point2)
                    {
                        gfx.DrawLine(penOutline, segment.Point1, segment.Point2);
                    }
                }
            }
        }
        private void AccumulateBridgesRects(ConnectorLineSegment segment, ref RectangleF rcRefresh)
        {
            // line bridge size
            float fLineBridgeSize = GetLineBridgeSize();

            // segment bounds
            PointF pt1 = segment.Point1;
            PointF pt2 = segment.Point2;

            RectangleF rcSegmentBounds = RectangleF.Empty;
            rcSegmentBounds.Location = pt1;
            rcSegmentBounds.Size = new SizeF(pt2.X - pt1.X, pt2.Y - pt1.Y);

            // get segment center point
            PointF ptCenter = PointF.Empty;
            ptCenter.X = rcSegmentBounds.X + rcSegmentBounds.Width / 2;
            ptCenter.Y = rcSegmentBounds.Y + rcSegmentBounds.Height / 2;

            // update segment rect
            RectangleF rcTemp = Geometry.CreateRect(ptCenter, new SizeF(fLineBridgeSize, fLineBridgeSize));
            rcSegmentBounds = RectangleF.Union(rcSegmentBounds, rcTemp);

            // merge port bounding rect with current refresh rect
            if (rcRefresh.Size.IsEmpty)
            {
                rcRefresh = rcSegmentBounds;
            }
            else
            {
                rcRefresh = RectangleF.Union(rcRefresh, rcSegmentBounds);
            }
        }

        /// <summary>
        /// Render given line segment to specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics render to.</param>
        /// <param name="segment">Line segment to render.</param>
        private void DrawShadowSegment(Graphics gfx, ConnectorLineSegment segment)
        {
            Bridge bridge;
            float fOffsetHelper;
            float fLineBridgeSize = fOffsetHelper = GetLineBridgeSize();

            using (Pen penShadow = new Pen(Color.FromArgb(80, this.ShadowStyle.Color), this.LineStyle.LineWidth))
            {
                if (segment.Bridges.Count > 0)
                {
                    segment.Bridges.Sort(new BridgeComparer());
                    PointF ptStart = segment.Point1;
                    PointF ptEnd = segment.Point2;
                    int nLength = segment.Bridges.Count;
                    float fBridgeOffset;

                    for (int n = 0; n < nLength; n++)
                    {
                        // Get bridge and it offset from start point.
                        bridge = (Bridge)segment.Bridges[n];
                        fBridgeOffset = (float)Geometry.PointDistance(segment.Point1, segment.Point2) - bridge.Offset;

                        // Get bridge start point concidering bridge size
                        ptEnd = Geometry.GetPointByOffset(ptStart, segment.Point2, fBridgeOffset + fLineBridgeSize / 2);

                        // Draw first line part.
                        if (ptStart != ptEnd)
                            gfx.DrawLine(penShadow, ptStart, ptEnd);

                        // Merge near bridges in one.
                        MergeNearBridges(ref n, bridge, segment, ref fOffsetHelper, ref fBridgeOffset);

                        // Find bridge rectangle.
                        ptStart = ptEnd;
                        ptEnd = Geometry.GetPointByOffset(ptStart, segment.Point2, fBridgeOffset - fOffsetHelper / 2);
                        PointF ptCenter = Geometry.GetPointByOffset(ptStart, segment.Point2, fBridgeOffset);
                        if (bridge.ConnectorIntersecting.Visible)
                            RenderBridge(gfx, penShadow, ptStart, ptCenter, ptEnd);
                        else
                            gfx.DrawLine(penShadow, ptStart, ptEnd);
                        ptStart = ptEnd;
                        fOffsetHelper = fLineBridgeSize;
                    }

                    // Draw final line segment.
                    if (ptEnd != segment.Point2)
                        gfx.DrawLine(penShadow, ptEnd, segment.Point2);
                }
                else
                {
                    if (segment.Point1 != segment.Point2)
                        gfx.DrawLine(penShadow, segment.Point1, segment.Point2);
                }
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the handle by index of pathPoints.
        /// </summary>
        /// <param name="nIndex">Index of handle in points list.</param>
        /// <returns>Handle by path points index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index less that zero or larger that handle count.</exception>
        protected IHandle GetHandleAt(int nIndex)
        {
            if (nIndex < 0 || nIndex >= (this.ControlPoints.Count + 2))
                throw new ArgumentOutOfRangeException("Connectorbase.GetHandleAt() method.");

            IHandle handle;

            if (nIndex == 0)
            {
                handle = this.TailEndPoint;
            }
            else if (nIndex == this.ControlPoints.Count + 1)
            {
                handle = this.HeadEndPoint;
            }
            else
            {
                handle = (IHandle)this.ControlPoints[nIndex - 1];
            }

            return handle;
        }

        #region Bridges renderer
        /// <summary>
        /// Render bridge appropriate to its style defenited by BridgeStyle property.
        /// </summary>
        /// <param name="gfx">Graphics bridge render to.</param>
        /// <param name="pen">Pen used to draw bridge.</param>
        /// <param name="startPoint">Start point of the bridge.</param>
        /// <param name="centerPoint">Center point of the bgidge.</param>
        /// <param name="endPoint">End point of the bridge.</param>
        private void RenderBridge(Graphics gfx, Pen pen, PointF startPoint, PointF centerPoint, PointF endPoint)
        {
            switch (BridgeStyle)
            {
                case BridgeStyle.Arc:
                    RenderArcBridge(gfx, pen, startPoint, centerPoint, endPoint);
                    break;
                case BridgeStyle.Gap:
                    break;
                case BridgeStyle.Square:
                    RenderSquareBridge(gfx, pen, startPoint, endPoint);
                    break;
                case BridgeStyle.Sides2:
                    RenderSides2Bridge(gfx, pen, startPoint, centerPoint, endPoint);
                    break;
                case BridgeStyle.Sides3:
                    RenderSides3Bridge(gfx, pen, startPoint, endPoint);
                    break;
                case BridgeStyle.Sides4:
                    RenderSides4Bridge(gfx, pen, startPoint, centerPoint, endPoint);
                    break;
                case BridgeStyle.Sides5:
                    RenderSides5Bridge(gfx, pen, startPoint, endPoint);
                    break;
                case BridgeStyle.Sides6:
                    RenderSides6Bridge(gfx, pen, startPoint, centerPoint, endPoint);
                    break;
                case BridgeStyle.Sides7:
                    RenderSides7Bridge(gfx, pen, startPoint, endPoint);
                    break;
            }
        }

        /// <summary>
        /// Render arc bridge.
        /// </summary>
        /// <param name="gfx">Graphics bridge render to.</param>
        /// <param name="pen">Pen used to draw bridge.</param>
        /// <param name="ptStart">Start point of the bridge.</param>
        /// <param name="ptCenter">Center point of the bgidge.</param>
        /// <param name="ptEnd">End point of the bridge.</param>
        private void RenderArcBridge(Graphics gfx, Pen pen, PointF ptStart, PointF ptCenter, PointF ptEnd)
        {
            float height = (float)Geometry.PointDistance(ptStart, ptEnd);
            RectangleF rect = Geometry.CreateRect(ptCenter, new SizeF(height, height));

            if (Math.Round(rect.Width, 5) != 0 && Math.Round(rect.Height, 5) != 0)
            {
                float fBridgeAngle = (float)((Geometry.LineAngle(ptStart, ptEnd) * 180f) / Math.PI);

                while (fBridgeAngle >= 180)
                    fBridgeAngle -= 180;

                gfx.DrawArc(pen, rect, fBridgeAngle + 180, 180);
            }
        }

        /// <summary>
        /// Render square bridge.
        /// </summary>
        /// <param name="gfx">Graphics to render bridge to.</param>
        /// <param name="pen">Pen to render bridge.</param>
        /// <param name="startPoint">Start point of the bridge.</param>
        /// <param name="endPoint">End point of the bridge.</param>
        private void RenderSquareBridge(Graphics gfx, Pen pen, PointF startPoint, PointF endPoint)
        {
            float height = (float)Geometry.PointDistance(startPoint, endPoint) / 2;
            PointF[] pts = new PointF[4] { startPoint, PointF.Empty, PointF.Empty, endPoint };

            if (Point.Round(startPoint).X == Point.Round(endPoint).X)
            {
                pts[1] = new PointF(startPoint.X + height, startPoint.Y);
                pts[2] = new PointF(endPoint.X + height, endPoint.Y);
            }
            else
            {
                pts[1] = new PointF(startPoint.X, startPoint.Y - height);
                pts[2] = new PointF(endPoint.X, endPoint.Y - height);
            }

            gfx.DrawLines(pen, pts);
        }

        /// <summary>
        /// Render sides2 bridge.
        /// </summary>
        /// <param name="gfx">Graphics to render bridge to.</param>
        /// <param name="pen">Pen to render bridge.</param>
        /// <param name="startPoint">Start point of the bridge.</param>
        /// <param name="centerPoint">Center point of the bgidge.</param>
        /// <param name="endPoint">End point of the bridge.</param>
        private void RenderSides2Bridge(Graphics gfx, Pen pen, PointF startPoint, PointF centerPoint, PointF endPoint)
        {
            float height = (float)Geometry.PointDistance(startPoint, endPoint) / 2;
            PointF[] pts = new PointF[3] { startPoint, PointF.Empty, endPoint };

            if (Point.Round(startPoint).X == Point.Round(endPoint).X)
            {
                pts[1] = new PointF(centerPoint.X + height, centerPoint.Y);
            }
            else
            {
                pts[1] = new PointF(centerPoint.X, centerPoint.Y - height);
            }

            gfx.DrawLines(pen, pts);
        }

        /// <summary>
        /// Render sides3 bridge.
        /// </summary>
        /// <param name="gfx">Graphics to render bridge to.</param>
        /// <param name="pen">Pen to render bridge.</param>
        /// <param name="startPoint">Start point of the bridge.</param>
        /// <param name="endPoint">End point of the bridge.</param>
        private void RenderSides3Bridge(Graphics gfx, Pen pen, PointF startPoint, PointF endPoint)
        {
            float angle = (float)(Math.Round(Geometry.LineAngle(startPoint, endPoint) * 180 / Math.PI));

            while (angle >= 360)
                angle -= 360;

            float height = (float)Geometry.PointDistance(startPoint, endPoint) / 2;
            PointF[] pts = new PointF[4] { startPoint, PointF.Empty, PointF.Empty, endPoint };

            if (angle == 0 || angle == 90)
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(startPoint.X + height * (float)Math.Sin(Math.PI / 3), startPoint.Y + height * (float)Math.Cos(Math.PI / 3));
                    pts[2] = new PointF(endPoint.X + height * (float)Math.Sin(Math.PI / 3), endPoint.Y - height * (float)Math.Cos(Math.PI / 3));
                }
                else
                {
                    pts[1] = new PointF(startPoint.X + height * (float)Math.Cos(Math.PI / 3), startPoint.Y - height * (float)Math.Sin(Math.PI / 3));
                    pts[2] = new PointF(endPoint.X - height * (float)Math.Cos(Math.PI / 3), endPoint.Y - height * (float)Math.Sin(Math.PI / 3));
                }
            }
            else
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(startPoint.X + height * (float)Math.Sin(Math.PI / 3), startPoint.Y - height * (float)Math.Cos(Math.PI / 3));
                    pts[2] = new PointF(endPoint.X + height * (float)Math.Sin(Math.PI / 3), endPoint.Y + height * (float)Math.Cos(Math.PI / 3));
                }
                else
                {
                    pts[1] = new PointF(startPoint.X - height * (float)Math.Cos(Math.PI / 3), startPoint.Y - height * (float)Math.Sin(Math.PI / 3));
                    pts[2] = new PointF(endPoint.X + height * (float)Math.Cos(Math.PI / 3), endPoint.Y - height * (float)Math.Sin(Math.PI / 3));
                }
            }

            gfx.DrawLines(pen, pts);
        }

        /// <summary>
        /// Render sides4 bridge.
        /// </summary>
        /// <param name="gfx">Graphics to render bridge to.</param>
        /// <param name="pen">Pen to render bridge.</param>
        /// <param name="startPoint">Start point of the bridge.</param>
        /// <param name="centerPoint">Center point of the bgidge.</param>
        /// <param name="endPoint">End point of the bridge.</param>
        private void RenderSides4Bridge(Graphics gfx, Pen pen, PointF startPoint, PointF centerPoint, PointF endPoint)
        {
            float angle = (float)(Math.Round(Geometry.LineAngle(startPoint, endPoint) * 180 / Math.PI));

            while (angle >= 360)
                angle -= 360;

            float height = (float)Geometry.PointDistance(startPoint, endPoint) / 2;
            PointF[] pts = new PointF[5] { startPoint, PointF.Empty, PointF.Empty, PointF.Empty, endPoint };

            if (angle == 0 || angle == 90)
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(
                        startPoint.X + height * (float)Math.Sin(Math.PI / 4),
                        startPoint.Y - height * ((float)Math.Cos(Math.PI / 4) - 1));

                    pts[2] = new PointF(centerPoint.X + height, centerPoint.Y);

                    pts[3] = new PointF(
                        endPoint.X + height * (float)Math.Sin(Math.PI / 4),
                        endPoint.Y - height * (1 - (float)Math.Cos(Math.PI / 4)));
                }
                else
                {
                    pts[1] = new PointF(
                        startPoint.X - height * ((float)Math.Cos(Math.PI / 4) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 4));

                    pts[2] = new PointF(centerPoint.X, centerPoint.Y - height);

                    pts[3] = new PointF(
                        endPoint.X - height * (1 - (float)Math.Cos(Math.PI / 4)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 4));
                }
            }
            else
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(
                        startPoint.X + height * (float)Math.Sin(Math.PI / 4),
                        startPoint.Y + height * ((float)Math.Cos(Math.PI / 4) - 1));

                    pts[2] = new PointF(centerPoint.X + height, centerPoint.Y);

                    pts[3] = new PointF(
                        endPoint.X + height * (float)Math.Sin(Math.PI / 4),
                        endPoint.Y + height * (1 - (float)Math.Cos(Math.PI / 4)));
                }
                else
                {
                    pts[1] = new PointF(
                        startPoint.X + height * ((float)Math.Cos(Math.PI / 4) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 4));

                    pts[2] = new PointF(centerPoint.X, centerPoint.Y - height);

                    pts[3] = new PointF(
                        endPoint.X + height * (1 - (float)Math.Cos(Math.PI / 4)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 4));
                }
            }

            gfx.DrawLines(pen, pts);
        }

        /// <summary>
        /// Render sides5 bridge.
        /// </summary>
        /// <param name="gfx">Graphics to render bridge to.</param>
        /// <param name="pen">Pen to render bridge.</param>
        /// <param name="startPoint">Start point of the bridge.</param>
        /// <param name="endPoint">End point of the bridge.</param>
        private void RenderSides5Bridge(Graphics gfx, Pen pen, PointF startPoint, PointF endPoint)
        {
            float angle = (float)(Math.Round(Geometry.LineAngle(startPoint, endPoint) * 180 / Math.PI));

            while (angle >= 360)
                angle -= 360;

            float height = (float)Geometry.PointDistance(startPoint, endPoint) / 2;
            PointF[] pts = new PointF[6] { startPoint, PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty, endPoint };

            if (angle == 0 || angle == 90)
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(
                        startPoint.X + height * (float)Math.Sin(Math.PI / 5),
                        startPoint.Y - height * ((float)Math.Cos(Math.PI / 5) - 1));

                    pts[2] = new PointF(
                        startPoint.X + height * (float)Math.Sin(2 * Math.PI / 5),
                        startPoint.Y - height * ((float)Math.Cos(2 * Math.PI / 5) - 1));

                    pts[3] = new PointF(
                        endPoint.X + height * (float)Math.Sin(2 * Math.PI / 5),
                        endPoint.Y - height * (1 - (float)Math.Cos(2 * Math.PI / 5)));

                    pts[4] = new PointF(
                        endPoint.X + height * (float)Math.Sin(Math.PI / 5),
                        endPoint.Y - height * (1 - (float)Math.Cos(Math.PI / 5)));
                }
                else
                {
                    pts[1] = new PointF(
                        startPoint.X - height * ((float)Math.Cos(Math.PI / 5) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 5));

                    pts[2] = new PointF(
                        startPoint.X - height * ((float)Math.Cos(2 * Math.PI / 5) - 1),
                        startPoint.Y - height * (float)Math.Sin(2 * Math.PI / 5));

                    pts[3] = new PointF(
                        endPoint.X - height * (1 - (float)Math.Cos(2 * Math.PI / 5)),
                        endPoint.Y - height * (float)Math.Sin(2 * Math.PI / 5));

                    pts[4] = new PointF(
                        endPoint.X - height * (1 - (float)Math.Cos(Math.PI / 5)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 5));
                }
            }
            else
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(
                        startPoint.X + height * (float)Math.Sin(Math.PI / 5),
                        startPoint.Y + height * ((float)Math.Cos(Math.PI / 5) - 1));

                    pts[2] = new PointF(
                        startPoint.X + height * (float)Math.Sin(2 * Math.PI / 5),
                        startPoint.Y + height * ((float)Math.Cos(2 * Math.PI / 5) - 1));

                    pts[3] = new PointF(
                        endPoint.X + height * (float)Math.Sin(2 * Math.PI / 5),
                        endPoint.Y + height * (1 - (float)Math.Cos(2 * Math.PI / 5)));

                    pts[4] = new PointF(
                        endPoint.X + height * (float)Math.Sin(Math.PI / 5),
                        endPoint.Y + height * (1 - (float)Math.Cos(Math.PI / 5)));
                }
                else
                {
                    pts[1] = new PointF(
                        startPoint.X + height * ((float)Math.Cos(Math.PI / 5) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 5));

                    pts[2] = new PointF(
                        startPoint.X + height * ((float)Math.Cos(2 * Math.PI / 5) - 1),
                        startPoint.Y - height * (float)Math.Sin(2 * Math.PI / 5));

                    pts[3] = new PointF(
                        endPoint.X + height * (1 - (float)Math.Cos(2 * Math.PI / 5)),
                        endPoint.Y - height * (float)Math.Sin(2 * Math.PI / 5));

                    pts[4] = new PointF(
                        endPoint.X + height * (1 - (float)Math.Cos(Math.PI / 5)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 5));
                }
            }

            gfx.DrawLines(pen, pts);
        }

        /// <summary>
        /// Render sides6 bridge.
        /// </summary>
        /// <param name="gfx">Graphics to render bridge to.</param>
        /// <param name="pen">Pen to render bridge.</param>
        /// <param name="startPoint">Start point of the bridge.</param>
        /// <param name="centerPoint">Center point of the bgidge.</param>
        /// <param name="endPoint">End point of the bridge.</param>
        private void RenderSides6Bridge(Graphics gfx, Pen pen, PointF startPoint, PointF centerPoint, PointF endPoint)
        {
            float angle = (float)(Math.Round(Geometry.LineAngle(startPoint, endPoint) * 180 / Math.PI));

            while (angle >= 360)
                angle -= 360;

            float height = (float)Geometry.PointDistance(startPoint, endPoint) / 2;
            PointF[] pts = new PointF[7] { startPoint, PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty, endPoint };

            if (angle == 0 || angle == 90)
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(
                        startPoint.X + height * (float)Math.Sin(Math.PI / 6),
                        startPoint.Y - height * ((float)Math.Cos(Math.PI / 6) - 1));

                    pts[2] = new PointF(
                        startPoint.X + height * (float)Math.Sin(2 * Math.PI / 6),
                        startPoint.Y - height * ((float)Math.Cos(Math.PI / 3) - 1));

                    pts[3] = new PointF(centerPoint.X + height, centerPoint.Y);

                    pts[4] = new PointF(
                        endPoint.X + height * (float)Math.Sin(2 * Math.PI / 6),
                        endPoint.Y - height * (1 - (float)Math.Cos(Math.PI / 3)));

                    pts[5] = new PointF(
                        endPoint.X + height * (float)Math.Sin(Math.PI / 6),
                        endPoint.Y - height * (1 - (float)Math.Cos(Math.PI / 6)));
                }
                else
                {
                    pts[1] = new PointF(
                        startPoint.X - height * ((float)Math.Cos(Math.PI / 6) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 6));

                    pts[2] = new PointF(
                        startPoint.X - height * ((float)Math.Cos(Math.PI / 3) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 3));

                    pts[3] = new PointF(centerPoint.X, centerPoint.Y - height);

                    pts[4] = new PointF(
                        endPoint.X - height * (1 - (float)Math.Cos(Math.PI / 3)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 3));

                    pts[5] = new PointF(
                        endPoint.X - height * (1 - (float)Math.Cos(Math.PI / 6)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 6));
                }
            }
            else
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(
                        startPoint.X + height * (float)Math.Sin(Math.PI / 6),
                        startPoint.Y + height * ((float)Math.Cos(Math.PI / 6) - 1));

                    pts[2] = new PointF(
                        startPoint.X + height * (float)Math.Sin(2 * Math.PI / 6),
                        startPoint.Y + height * ((float)Math.Cos(Math.PI / 3) - 1));

                    pts[3] = new PointF(centerPoint.X + height, centerPoint.Y);

                    pts[4] = new PointF(
                        endPoint.X + height * (float)Math.Sin(2 * Math.PI / 6),
                        endPoint.Y + height * (1 - (float)Math.Cos(Math.PI / 3)));

                    pts[5] = new PointF(
                        endPoint.X + height * (float)Math.Sin(Math.PI / 6),
                        endPoint.Y + height * (1 - (float)Math.Cos(Math.PI / 6)));
                }
                else
                {
                    pts[1] = new PointF(
                        startPoint.X + height * ((float)Math.Cos(Math.PI / 6) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 6));

                    pts[2] = new PointF(
                        startPoint.X + height * ((float)Math.Cos(Math.PI / 3) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 3));

                    pts[3] = new PointF(centerPoint.X, centerPoint.Y - height);

                    pts[4] = new PointF(
                        endPoint.X + height * (1 - (float)Math.Cos(Math.PI / 3)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 3));

                    pts[5] = new PointF(
                        endPoint.X + height * (1 - (float)Math.Cos(Math.PI / 6)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 6));
                }
            }

            gfx.DrawLines(pen, pts);
        }

        /// <summary>
        /// Render sides7 bridge.
        /// </summary>
        /// <param name="gfx">Graphics to render bridge to.</param>
        /// <param name="pen">Pen to render bridge.</param>
        /// <param name="startPoint">Start point of the bridge.</param>
        /// <param name="endPoint">End point of the bridge.</param>
        private void RenderSides7Bridge(Graphics gfx, Pen pen, PointF startPoint, PointF endPoint)
        {
            float angle = (float)(Math.Round(Geometry.LineAngle(startPoint, endPoint) * 180 / Math.PI));

            while (angle >= 360)
                angle -= 360;

            float height = (float)Geometry.PointDistance(startPoint, endPoint) / 2;
            PointF[] pts = new PointF[8]
            { 
                startPoint, 
                PointF.Empty,
                PointF.Empty,
                PointF.Empty, 
                PointF.Empty, 
                PointF.Empty,
                PointF.Empty,
                endPoint
            };

            if (angle == 0 || angle == 90)
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(
                        startPoint.X + height * (float)Math.Sin(Math.PI / 7),
                        startPoint.Y - height * ((float)Math.Cos(Math.PI / 7) - 1));

                    pts[2] = new PointF(
                        startPoint.X + height * (float)Math.Sin(2 * Math.PI / 7),
                        startPoint.Y - height * ((float)Math.Cos(2 * Math.PI / 7) - 1));

                    pts[3] = new PointF(
                        startPoint.X + height * (float)Math.Sin(3 * Math.PI / 7),
                        startPoint.Y - height * ((float)Math.Cos(3 * Math.PI / 7) - 1));

                    pts[4] = new PointF(
                        endPoint.X + height * (float)Math.Sin(3 * Math.PI / 7),
                        endPoint.Y - height * (1 - (float)Math.Cos(3 * Math.PI / 7)));

                    pts[5] = new PointF(
                        endPoint.X + height * (float)Math.Sin(2 * Math.PI / 7),
                        endPoint.Y - height * (1 - (float)Math.Cos(2 * Math.PI / 7)));

                    pts[6] = new PointF(
                        endPoint.X + height * (float)Math.Sin(Math.PI / 7),
                        endPoint.Y - height * (1 - (float)Math.Cos(Math.PI / 7)));
                }
                else
                {
                    pts[1] = new PointF(
                        startPoint.X - height * ((float)Math.Cos(Math.PI / 7) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 7));

                    pts[2] = new PointF(
                        startPoint.X - height * ((float)Math.Cos(2 * Math.PI / 7) - 1),
                        startPoint.Y - height * (float)Math.Sin(2 * Math.PI / 7));

                    pts[3] = new PointF(
                        startPoint.X - height * ((float)Math.Cos(3 * Math.PI / 7) - 1),
                        startPoint.Y - height * (float)Math.Sin(3 * Math.PI / 7));

                    pts[4] = new PointF(
                        endPoint.X - height * (1 - (float)Math.Cos(3 * Math.PI / 7)),
                        endPoint.Y - height * (float)Math.Sin(3 * Math.PI / 7));

                    pts[5] = new PointF(
                        endPoint.X - height * (1 - (float)Math.Cos(2 * Math.PI / 7)),
                        endPoint.Y - height * (float)Math.Sin(2 * Math.PI / 7));

                    pts[6] = new PointF(
                        endPoint.X - height * (1 - (float)Math.Cos(Math.PI / 7)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 7));
                }
            }
            else
            {
                if (Point.Round(startPoint).X == Point.Round(endPoint).X)
                {
                    pts[1] = new PointF(
                        startPoint.X + height * (float)Math.Sin(Math.PI / 7),
                        startPoint.Y + height * ((float)Math.Cos(Math.PI / 7) - 1));

                    pts[2] = new PointF(
                        startPoint.X + height * (float)Math.Sin(2 * Math.PI / 7),
                        startPoint.Y + height * ((float)Math.Cos(2 * Math.PI / 7) - 1));

                    pts[3] = new PointF(
                        startPoint.X + height * (float)Math.Sin(3 * Math.PI / 7),
                        startPoint.Y + height * ((float)Math.Cos(3 * Math.PI / 7) - 1));

                    pts[4] = new PointF(
                        endPoint.X + height * (float)Math.Sin(3 * Math.PI / 7),
                        endPoint.Y + height * (1 - (float)Math.Cos(3 * Math.PI / 7)));

                    pts[5] = new PointF(
                        endPoint.X + height * (float)Math.Sin(2 * Math.PI / 7),
                        endPoint.Y + height * (1 - (float)Math.Cos(2 * Math.PI / 7)));

                    pts[6] = new PointF(
                        endPoint.X + height * (float)Math.Sin(Math.PI / 7),
                        endPoint.Y + height * (1 - (float)Math.Cos(Math.PI / 7)));
                }
                else
                {
                    pts[1] = new PointF(
                        startPoint.X + height * ((float)Math.Cos(Math.PI / 7) - 1),
                        startPoint.Y - height * (float)Math.Sin(Math.PI / 7));

                    pts[2] = new PointF(
                        startPoint.X + height * ((float)Math.Cos(2 * Math.PI / 7) - 1),
                        startPoint.Y - height * (float)Math.Sin(2 * Math.PI / 7));

                    pts[3] = new PointF(
                        startPoint.X + height * ((float)Math.Cos(3 * Math.PI / 7) - 1),
                        startPoint.Y - height * (float)Math.Sin(3 * Math.PI / 7));

                    pts[4] = new PointF(
                        endPoint.X + height * (1 - (float)Math.Cos(3 * Math.PI / 7)),
                        endPoint.Y - height * (float)Math.Sin(3 * Math.PI / 7));

                    pts[5] = new PointF(
                        endPoint.X + height * (1 - (float)Math.Cos(2 * Math.PI / 7)),
                        endPoint.Y - height * (float)Math.Sin(2 * Math.PI / 7));

                    pts[6] = new PointF(
                        endPoint.X + height * (1 - (float)Math.Cos(Math.PI / 7)),
                        endPoint.Y - height * (float)Math.Sin(Math.PI / 7));
                }
            }

            gfx.DrawLines(pen, pts);
        }
        #endregion

        /// <summary>
        /// Inits the end points from two points.
        /// </summary>
        /// <param name="ptStart">The tail end point position .</param>
        /// <param name="ptEnd">The head end point position.</param>
        private void InitEndPoints(PointF ptStart, PointF ptEnd)
        {
            // set endpoints
            m_endPointTail = new TailEndPoint(this, ptStart);
            m_endPointHead = new HeadEndPoint(this, ptEnd);

            // initialize necessary variables.
            InitConnector();
        }

        /// <summary>
        /// Initialize connector necessary variables..
        /// </summary>
        private void InitConnector()
        {
            // update connector segments
            UpdateSegments();

            // enable control points merging
            m_cntState |= ConnectorState.MergeControlPoints;
        }

        /// <summary>
        /// Merges the near bridges.
        /// </summary>
        /// <param name="nIndex">Index of the bridge.</param>
        /// <param name="bridge">The bridge.</param>
        /// <param name="lineSegment">The line segment.</param>
        /// <param name="fOffsetHelper">The bridegSize helper.</param>
        /// <param name="fBridgeOffset">The bridge offset.</param>
        protected void MergeNearBridges(ref int nIndex, Bridge bridge, ConnectorLineSegment lineSegment, ref float fOffsetHelper, ref float fBridgeOffset)
        {
            int nIndexHelper = nIndex + 1;
            int nLength = lineSegment.Bridges.Count;
            float fLineBridgeSize = GetLineBridgeSize();
            float fLineOffset;

            bool bMerged = false;
            Bridge bridgeTemp;
            Bridge bridgeCurrent = bridge;

            // merge bridges until distance between neighbours is less than bridge size
            while (nIndexHelper < nLength)
            {
                bridgeTemp = (Bridge)lineSegment.Bridges[nIndexHelper];

                // Get bridge offset.
                fLineOffset = bridgeTemp.Offset - bridgeCurrent.Offset;

                // Check for offset must biggest to bridge size.
                if (fLineOffset <= fLineBridgeSize)
                {
                    bridgeCurrent = bridgeTemp;

                    bMerged = true;

                    // Skip merged bridges.
                    nIndex++;

                    fBridgeOffset -= fLineOffset / 2;
                    fOffsetHelper += fLineOffset;
                }

                // proceed to next bridge
                nIndexHelper++;
            }

            if (!bMerged)
                fOffsetHelper = fLineBridgeSize;
        }

        /// <summary>
        /// Gets the length of the line bridge.
        /// </summary>
        /// <returns>The bridge length.</returns>
        protected float GetLineBridgeSize()
        {
            float fBridgeSize = 0f;
            Model model = this.Root;

            if (model != null)
            {
                fBridgeSize = model.LineBridgeSize;
                fBridgeSize = MeasureUnitsConverter.ToPixelX(fBridgeSize, this.MeasurementUnit);
            }

            return fBridgeSize;
        }

        /// <summary>
        /// Resets the segment bridges.
        /// </summary>
        /// <param name="segmentBridges">The segment bridges.</param>
        /// <param name="connector">The segment checking container.</param>
        /// <param name="nSegmentCheckingID">The checking segment ID.</param>
        /// <returns>true, if reset segment bridges</returns>
        private bool ResetSegmentBridges(ConnectorLineSegment segmentBridges, ConnectorBase connector, int nSegmentCheckingID)
        {
            bool res = false;

            // Reset bridges from intersecting segments
            foreach (Bridge bridge in segmentBridges.Bridges)
            {
                if (bridge.ConnectorIntersecting == connector && bridge.SegmentIntersectingID == nSegmentCheckingID)
                {
                    segmentBridges.Bridges.Remove(bridge);
                    res = true;
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// Checks the line segment intersect.
        /// </summary>
        /// <param name="ptStart1">The first start point.</param>
        /// <param name="ptEnd1">The first end point.</param>
        /// <param name="ptStart2">The second start point.</param>
        /// <param name="ptEnd2">The pt second end point.</param>
        /// <param name="ptIntersect">The intersection point.</param>
        /// <returns>true, if line segment intersect.</returns>
        protected bool CheckLineSegmentIntersect(PointF ptStart1, PointF ptEnd1, PointF ptStart2, PointF ptEnd2, out PointF ptIntersect)
        {
            ptIntersect = PointF.Empty;

            bool bSuccess = Geometry.CalcLineSegmentIntersect(ptStart1, ptEnd1, ptStart2, ptEnd2, ref ptIntersect);
            float lineBridgeSize = GetLineBridgeSize();

            // Fixed: D8901
            // Check distance from intersec point to segment end point.
            // if( lineBridgeSize > 0 )
            // {
            //    lineBridgeSize /= 2f;

            //    if( bSuccess )
            //    {
            //        float distToFirstPointStart = ( float ) Geometry.PointDistance( ptIntersect, ptStart1 );

            //        if( distToFirstPointStart <= lineBridgeSize  )
            //            bSuccess = false;
            //    }

            //    if( bSuccess )
            //    {
            //        float distToFirstPointEnd = ( float ) Geometry.PointDistance( ptIntersect, ptEnd1 );

            //        if( distToFirstPointEnd <= lineBridgeSize )
            //            bSuccess = false;
            //    }

            //    if( bSuccess )
            //    {
            //        float distToSecondPointStart = ( float ) Geometry.PointDistance( ptIntersect, ptStart2 );

            //        if( distToSecondPointStart <= lineBridgeSize )
            //            bSuccess = false;
            //    }

            //    if( bSuccess )
            //    {
            //        float distToSecondPointEnd = ( float ) Geometry.PointDistance( ptIntersect, ptEnd2 );

            //        if( distToSecondPointEnd <= lineBridgeSize )
            //            bSuccess = false;
            //    }
            // }
            return bSuccess;
        }

        /// <summary>
        /// Gets all connectors in composite nodes in deep.
        /// </summary>
        /// <param name="composite">The composite node.</param>
        /// <returns>
        /// The <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.
        /// </returns>
        protected NodeCollection GetAllConnectors(ICompositeNode composite)
        {
            NodeCollection nodesToReturn = new NodeCollection();

            // iterate all group nodes and find connectors
            for (int i = 0, length = composite.ChildCount; i < length; i++)
            {
                Node node = composite.GetChild(i);
                ICompositeNode group = node as ICompositeNode;
                ConnectorBase connector = node as ConnectorBase;

                if (group != null)
                {
                    nodesToReturn.AddRange(GetAllConnectors(group));
                }
                else if (connector != null)
                {
                    // filter nodes
                    nodesToReturn.Add(node);
                }
            }

            return nodesToReturn;
        }

        /// <summary>
        /// Collect all group nodes in deep.
        /// </summary>
        /// <param name="compositeNode">The composite node.</param>
        /// <returns>
        /// The <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.
        /// </returns>
        private NodeCollection GetChildren(ICompositeNode compositeNode)
        {
            NodeCollection nodesToReturn = new NodeCollection();

            for (int i = 0, nLength = compositeNode.ChildCount; i < nLength; i++)
            {
                // get group child
                Node node = compositeNode.GetChild(i);

                // check if node is composite
                ICompositeNode parent = node as ICompositeNode;

                // add only children to collection
                if (parent != null)
                {
                    nodesToReturn.AddRange(GetChildren(parent));
                }
                else
                {
                    nodesToReturn.Add(node);
                }
            }

            return nodesToReturn;
        }

        /// <summary>
        /// Flips the end points by horizontal and vertical axis.
        /// </summary>
        /// <param name="bFlipX">if set to <c>true</c> to flipped by X axis.</param>
        /// <param name="bFlipY">if set to <c>true</c> to flipped by Y axis.</param>
        private void FlipEndPoints(bool bFlipX, bool bFlipY)
        {
            Matrix mtxFlip = new Matrix();

            PointF ptPinPointUnitIndependent = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);

            // Reset location to origin
            mtxFlip.Translate(-ptPinPointUnitIndependent.X, -ptPinPointUnitIndependent.Y, MatrixOrder.Append);

            // Flip by vertical and horizontal
            if (bFlipX)
                mtxFlip.Scale(-1.0f, 1.0f, MatrixOrder.Append);

            if (bFlipY)
                mtxFlip.Scale(1.0f, -1.0f, MatrixOrder.Append);

            // Restore last location
            mtxFlip.Translate(ptPinPointUnitIndependent.X, ptPinPointUnitIndependent.Y, MatrixOrder.Append);

            PointF[] pts = new PointF[] { this.HeadEndPoint.Location, this.TailEndPoint.Location };

            mtxFlip.TransformPoints(pts);

            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            this.HeadEndPoint.Location = pts[0];
            this.TailEndPoint.Location = pts[1];

            m_bLockHandleMove = bLockMove;

            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);
        }

        /// <summary>
        /// Updates EndPoints locations.
        /// </summary>
        /// <param name="ptsPath">New path points.</param>
        protected void SetEndPoints(PointF[] ptsPath)
        {
            if (this.EventSink != null)
                this.EventSink.Pause();

            Matrix mtxTemp = GetTransformations();
            AppendFlipTransforms(mtxTemp);

            PointF[] ptsEP = new PointF[] { ptsPath[0], ptsPath[ptsPath.Length - 1] };
            mtxTemp.TransformPoints(ptsEP);

            // update tail end point
            this.TailEndPoint.Location = ptsEP[0];

            // update head end point
            this.HeadEndPoint.Location = ptsEP[1];

            if (this.EventSink != null)
                this.EventSink.Resume();
        }

        /// <summary>
        /// Updates connector's segments.
        /// </summary>
        /// <remarks>
        /// Update using path points and related 
        /// handles to create new segments.
        /// </remarks>
        protected virtual void UpdateSegments()
        {
            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);

            // get path node points
            PointF[] ptsPoints = this.GetPathPoints();

            // clear line segments
            this.LineSegments.Clear();

            // repopulate line segments
            ConnectorLineSegment segmentTemp;

            for (int i = 0, nLength = ptsPoints.Length; nLength - 1 > i; i++)
            {
                segmentTemp = new ConnectorLineSegment(GetHandleAt(i), GetHandleAt(i + 1));
                this.LineSegments.Add(segmentTemp);
            }
        }

        /// <summary>
        /// Determines whether this line is connected.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this line is connected; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsConnected()
        {
            return IsHeadConnected() || IsTailConnected();
        }

        /// <summary>
        /// Determines whether HeadEdnPoints is connected
        /// </summary>
        /// <returns>true - if HeadEndPoints is connected otherwise - false.</returns>
        protected virtual bool IsHeadConnected()
        {
            return this.HeadEndPoint.Port != null;
        }

        /// <summary>
        /// Determines whether TailEdnPoints is connected
        /// </summary>
        /// <returns>true - if TailEndPoints is connected otherwise - false.</returns>
        protected virtual bool IsTailConnected()
        {
            return this.TailEndPoint.Port != null;
        }

        /// <summary>
        /// Merges the control points placed in line.
        /// </summary>
        /// <remarks>
        /// Used with handle moving.
        /// </remarks>
        protected virtual void MergeControlPoints() 
        { 
        }

        /// <summary>
        /// Merges the control points placed in line.
        /// </summary>
        /// <param name="ptsNew">The new points.</param>
        /// <remarks>
        /// Used with line segment move.
        /// </remarks>
        protected virtual void MergeControlPoints(ref PointF[] ptsNew) 
        { 
        }

        /// <summary>
        /// Synchronize head and tail end points with port if need.
        /// </summary>
        protected virtual void SyncEndPointsWithPort()
        {
            // synchronize head and tail end points
            if (m_mgrLink != null)
            {
                m_mgrLink.SynchronizeEndPoint(this.HeadEndPoint);
                m_mgrLink.SynchronizeEndPoint(this.TailEndPoint);
            }
        }
        #endregion

        #region Class public overrides
        /// <summary>
        /// Determines whether this node can edit control points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit control points; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanEditControlPoint()
        {
            return true;
        }

        /// <summary>
        /// Determines whether this node can draw control points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can draw control points; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanDrawControlPoints()
        {
            return true;
        }

        /// <summary>
        /// Determines whether this node can edit segments.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit segments; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanEditSegment()
        {
            if (this.EditStyle.AllowMoveX || this.EditStyle.AllowMoveY)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines whether this node can edit vertex points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit vertex point; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanEditVertexPoint()
        {
            return true;
        }

        /// <summary>
        /// Determines whether the node contains specific handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>
        /// <c>true</c> if the handle contains specific  handle; otherwise, <c>false</c>.
        /// </returns>
        public override bool ContainsHandle(IHandle handle)
        {
            bool bSuccess = base.ContainsHandle(handle);

            if (!bSuccess)
                bSuccess = (handle == this.HeadEndPoint || handle == this.TailEndPoint);

            return bSuccess;
        }

        /// <summary>
        /// Determines whether this node allow move it handle.
        /// </summary>
        /// <param name="handle">The handle to move.</param>
        /// <param name="ptNewLocation">The new endpoint location to check.</param>
        /// <returns>
        /// <c>true</c> if this node allow move it handle; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanMoveHandle(IHandle handle, PointF ptNewLocation)
        {
            bool bSuccess = base.CanMoveHandle(handle, ptNewLocation);

            if (bSuccess && !this.EditStyle.AllowRotate)
            {
                EndPoint endPoint = handle as EndPoint;
                EndPoint secondEndPoint;

                if (endPoint != null)
                {
                    IEndPointContainer container = endPoint.Container as IEndPointContainer;

                    if (container != null)
                    {
                        // get second endpoint
                        secondEndPoint = (endPoint is HeadEndPoint) ? container.TailEndPoint : container.HeadEndPoint;
                        double dOldAngle = Geometry.LineAngle(endPoint.Location, secondEndPoint.Location);
                        double dNewAngle = Geometry.LineAngle(ptNewLocation, secondEndPoint.Location);
                        double dAngleOffset = CommonUsedValues.ALLOWED_ROTATE_ANGLE * Math.PI / (CommonUsedValues.CIRCLE / 2);

                        if (dOldAngle - dAngleOffset > dNewAngle && dOldAngle + dAngleOffset < dNewAngle)
                            bSuccess = !m_bLockHandleMove;
                    }
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Gets the line segment by it index.
        /// </summary>
        /// <param name="nSegmentIndex">Index of the segment.</param>
        /// <returns>Line segment</returns>
        public override LineSegment GetLineSegmentAt(int nSegmentIndex)
        {
            if (nSegmentIndex < 0 || nSegmentIndex >= this.LineSegments.Count)
                throw new ArgumentOutOfRangeException("nSegmentIndex");

            return this.LineSegments[nSegmentIndex] as LineSegment;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Inserts the point to the <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/> at the specified index.
        /// </summary>
        /// <param name="ptIdx">The zero-based index ar which <paramref name="ptIdx"/> should be inserted.</param>
        /// <param name="val">The point in local coordinates.</param>
        public override void InsertPoint(int ptIdx, PointF val)
        {
            bool bMCP = (this.ConnectorState & ConnectorState.MergeControlPoints)
                        == ConnectorState.MergeControlPoints;

            this.ConnectorState = this.ConnectorState & (~ConnectorState.MergeControlPoints);

            base.InsertPoint(ptIdx, val);

            if (bMCP)
                this.ConnectorState |= ConnectorState.MergeControlPoints;
        }

        /// <summary>
        /// Moves the segment to given offset.
        /// </summary>
        /// <param name="lineSegmentIndex">Index of the line segment.</param>
        /// <param name="szOffset">The segment move offset.</param>
        /// <returns>The points to offset.</returns>
        protected override PointF[] MoveSegment(int lineSegmentIndex, SizeF szOffset)
        {
            if ((lineSegmentIndex == 0 && IsTailConnected())
                || (lineSegmentIndex == this.LineSegments.Count - 1 && IsHeadConnected()))
                szOffset = SizeF.Empty;

            return base.MoveSegment(lineSegmentIndex, szOffset);
        }

        /// <summary>
        /// Called when node scale factor is changed.
        /// </summary>
        /// <param name="strPropertyName">The property name that change scale factor value.</param>
        protected override void OnNodeScaleChanged(string strPropertyName)
        {
            Model model = this.Root;

            if (model != null)
            {
                model.BeginUpdate();
                model.EventSink.Pause();
                model.HistoryManager.Pause();
            }

            PointF[] pts = this.GraphicsPath.PathPoints;

            // update end point location
            SetEndPoints(pts);

            this.PathPoints = pts;
            UpdateGraphicsPath(pts);
            SyncEndPointsWithPort();

            if (model != null)
            {
                model.HistoryManager.Resume();
                model.EventSink.Resume();
                model.EndUpdate();
            }

            base.OnNodeScaleChanged(strPropertyName);
        }

        /// <summary>
        /// Perfoms additional changes on handle move.
        /// </summary>
        /// <param name="handleMoved">The moved handle .</param>
        /// <param name="szOffset">The move offset.</param>
        protected override void DoHandleMoveRelated(IHandle handleMoved, SizeF szOffset)
        {
            // pause history for a while
            SafeHistoryPause();

            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);

            // synchronize endpoints
            SyncEndPointsWithPort();

            // resume recording to history
            SafeHistoryResume();

            // if there are no control points sync opposite end point with its port
            MergeControlPoints();

            // call UpdatePathNodeData() method
            base.DoHandleMoveRelated(handleMoved, szOffset);
        }

        /// <summary>
        /// Performs additional changes on pin position changed.
        /// </summary>
        /// <param name="fX">The pin offset by x axis.</param>
        /// <param name="fY">The pin offset by y axis.</param>
        protected override void DoMoveRelatedActions(float fX, float fY)
        {
            // append flips to move offset
            SizeF szOffset = SizeF.Empty;
            szOffset.Width = this.FlipX ? -fX : fX;
            szOffset.Height = this.FlipY ? -fY : fY;

            if (m_mgrBridge != null)
            {
                m_mgrBridge.BeginUpdateIntersection();
                m_mgrBridge.AddToIntersectCollection(this);
            }

            if (m_mgrLink != null)
                m_mgrLink.BeginSynchronization();

            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            // synchronize head end point
            SynchronizeEndPoint(this.HeadEndPoint, szOffset);

            // synchronize tail end point
            SynchronizeEndPoint(this.TailEndPoint, szOffset);

            // call base to update bounding rectangle
            bool bLock = m_bPortUpdating;
            m_bPortUpdating = true;

            base.DoMoveRelatedActions(fX, fY);

            m_bPortUpdating = bLock;

            // pause history recording
            SafeHistoryPause();

            // udpate node path points
            UpdatePathNodeData();

            // allow to move handle while node not allow to rotate
            m_bLockHandleMove = bLockMove;

            if (m_mgrBridge != null)
                m_mgrBridge.EndUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.EndSynchronization();

            // resore history recording
            SafeHistoryResume();
        }

        /// <summary>
        /// Used to update child nodes sizes.
        /// </summary>
        /// <param name="szOldSize">The old size.</param>
        /// <param name="szNewSize">The new size.</param>
        protected override void DoSizeRelatedActions(SizeF szOldSize, SizeF szNewSize)
        {
            HistoryManager mngHistory = this.HistoryManager;

            if (mngHistory != null)
                mngHistory.Pause();

            float fWidthFactor = (szOldSize.Width == 0) ? 1 : szNewSize.Width / szOldSize.Width;
            float fHeigthFactor = (szOldSize.Height == 0) ? 1 : szNewSize.Height / szOldSize.Height;

            // generate matrix with scale transfomration
            Matrix mtxTransform = new Matrix(fWidthFactor, 0, 0, fHeigthFactor, 0, 0);

            // append transformation to get real locations data points
            mtxTransform.Multiply(GetTransformations(), MatrixOrder.Append);
            AppendFlipTransforms(mtxTransform);

            // get clone of data points after scale graphics path
            PointF[] pts = GetPathPoints(mtxTransform);

            if (m_mgrBridge != null)
                m_mgrBridge.BeginUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.BeginSynchronization();

            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            // append first and last data points as head and tail endPoints
            this.HeadEndPoint.Location = pts[pts.Length - 1];
            this.TailEndPoint.Location = pts[0];

            // update new path points to endPoints
            pts[pts.Length - 1] = this.HeadEndPoint.Location;
            pts[0] = this.TailEndPoint.Location;

            // set new scale points
            SetPoints(pts);

            if (mngHistory != null)
                mngHistory.Resume();

            // update graphicsPath and control points to scale factor
            base.DoSizeRelatedActions(szOldSize, szNewSize);

            // update path node data to endPoints
            UpdatePathNodeData();

            m_bLockHandleMove = bLockMove;

            if (m_mgrLink != null)
                m_mgrLink.EndSynchronization();

            if (m_mgrBridge != null)
                m_mgrBridge.EndUpdateIntersection();
        }

        /// <summary>
        /// Called after change the flipX value.
        /// </summary>
        /// <param name="value">New flipX value.</param>
        protected override void ChangeFlipX(bool value)
        {
            if (!m_bLockUpdate && !IsConnected())
            {
                m_bLockUpdate = true;

                HistoryManager mngHistory = this.HistoryManager;

                if (mngHistory != null)
                    mngHistory.Pause();

                FlipEndPoints(true, false);

                if (mngHistory != null)
                    mngHistory.Resume();

                // call base to update conections
                base.ChangeFlipX(value);

                m_bLockUpdate = false;
            }
        }

        /// <summary>
        /// Called after change the flipY value.
        /// </summary>
        /// <param name="value">New flipY value.</param>
        protected override void ChangeFlipY(bool value)
        {
            if (!m_bLockUpdate && !IsConnected())
            {
                m_bLockUpdate = true;

                HistoryManager mngHistory = this.HistoryManager;

                if (mngHistory != null)
                    mngHistory.Pause();

                FlipEndPoints(false, true);

                if (mngHistory != null)
                    mngHistory.Resume();

                // call base to update conections
                base.ChangeFlipY(value);

                m_bLockUpdate = false;
            }
        }

        /// <summary>
        /// Called after change the rotation by give angle.
        /// </summary>
        /// <param name="fRotationChange">The rotation angle offset.</param>
        protected override void ChangeRotationBy(float fRotationChange)
        {
            if (!m_bLockUpdate && !IsConnected())
            {
                m_bLockUpdate = true;

                HistoryManager mngHistory = this.HistoryManager;

                if (mngHistory != null)
                    mngHistory.Pause();

                PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
                PointF[] pts = new PointF[] { m_endPointHead.Location, m_endPointTail.Location };

                // append flips to rotate
                float angle = fRotationChange;

                if (this.FlipX)
                    angle = -angle;

                if (this.FlipY)
                    angle = -angle;

                if (m_mgrBridge != null)
                    m_mgrBridge.BeginUpdateIntersection();

                if (m_mgrLink != null)
                    m_mgrLink.BeginSynchronization();

                Matrix mtxTemp = new Matrix();
                mtxTemp.RotateAt(angle, ptPinPoint);
                mtxTemp.TransformPoints(pts);

                // assign new end point locations
                this.HeadEndPoint.Location = pts[0];
                this.TailEndPoint.Location = pts[1];

                if (mngHistory != null)
                    mngHistory.Resume();

                // call base to update connections
                base.ChangeRotationBy(fRotationChange);

                if (m_mgrLink != null)
                    m_mgrLink.EndSynchronization();

                if (m_mgrBridge != null)
                    m_mgrBridge.EndUpdateIntersection();

                m_bLockUpdate = false;
            }
        }

        /// <summary>
        /// Performs additional changes on pin offset value changed.
        /// </summary>
        /// <param name="szOldPinOffset">The old pin offset value.</param>
        /// <param name="szNewPinOffset">The new pin offset value.</param>
        protected override void DoPinOffsetRelatedActions(SizeF szOldPinOffset, SizeF szNewPinOffset)
        {
            if (m_mgrBridge != null)
                m_mgrBridge.BeginUpdateIntersection();

            if (m_mgrLink != null)
                m_mgrLink.BeginSynchronization();

            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            // Append flips to move offset
            SizeF szOffset = new SizeF(szOldPinOffset.Width - szNewPinOffset.Width, szOldPinOffset.Height - szNewPinOffset.Height);

            // move endPoints to pinOffset offset
            MoveEndPoints(szOffset);

            // call  base for update connections and cache region
            base.DoPinOffsetRelatedActions(szOldPinOffset, szNewPinOffset);

            // update graphics path
            UpdatePathNodeData();

            m_bLockHandleMove = bLockMove;

            if (m_mgrLink != null)
                m_mgrLink.EndSynchronization();

            if (m_mgrBridge != null)
                m_mgrBridge.EndUpdateIntersection();
        }

        /// <summary>
        /// Finishes the set points operation.
        /// </summary>
        protected override void FinishSetPoints()
        {
            base.FinishSetPoints();

            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);
        }

        /// <summary>
        /// Draws line in graphics.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        protected override void DrawPath(Graphics gfx)
        {
            if (IsLineBridgingEnabled())
            {
                foreach (ConnectorLineSegment segment in this.LineSegments)
                {
                    DrawSegment(gfx, segment);
                }
            }
            else
            {
                base.DrawPath(gfx);
            }
        }

        /// <summary>
        /// Render connector's shadow.
        /// </summary>
        /// <param name="gfx">Graphics render shadow to.</param>
        protected override void DrawShadowInternal(Graphics gfx)
        {
            if (IsLineBridgingEnabled())
            {
                foreach (ConnectorLineSegment segment in this.LineSegments)
                {
                    DrawShadowSegment(gfx, segment);
                }

                DrawDecoratorsShadow(gfx);
            }
            else
            {
                base.DrawShadowInternal(gfx);
            }
        }

        /// <summary>
        /// Retrieves array of points needed to construct node's GraphicsPath.
        /// </summary>
        /// <returns>Array of path points</returns>
        protected override PointF[] GetPathPoints()
        {
            ArrayList controlPoint = this.ControlPoints;
            int nControlCount = controlPoint.Count;

            // create matrix transformations
            Matrix mtxTemp = GetTransformations();
            AppendFlipTransforms(mtxTemp);
            mtxTemp.Invert();

            // get path points.
            PointF[] ptsToReturn = new PointF[nControlCount + 2];

            // fill point to path points
            ptsToReturn[nControlCount + 1] = this.HeadEndPoint.Location;
            ptsToReturn[0] = this.TailEndPoint.Location;
            mtxTemp.TransformPoints(ptsToReturn);

            for (int i = 0; i < nControlCount; i++)
            {
                ptsToReturn[i + 1] = ((ControlPoint)controlPoint[i]).Location;
            }

            return ptsToReturn;
        }

        /// <summary>
        /// Called when parent property changed.
        /// </summary>
        protected override void OnParentChanging()
        {
            m_bParentChangingBridgeUpdate = (this.Root != null);

            // updating will ignore when ( root == null )
            // for example: on add node to model
            if (m_bParentChangingBridgeUpdate && m_mgrBridge != null)
            {
                m_mgrBridge.AddToIntersectCollection(this);
            }

            base.OnParentChanging();
        }

        /// <summary>
        /// Called when parent changed.
        /// </summary>
        protected override void OnParentChanged()
        {
            // updating will ignore when ( root == null )  or bridging was updated in parent changing
            // for example: on remove node from model
            if (!m_bParentChangingBridgeUpdate && this.LineBridgingEnabled && m_mgrBridge != null)
            {
                m_mgrBridge.AddToIntersectCollection(this);
            }

            m_bParentChangingBridgeUpdate = false;
            base.OnParentChanged();
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("endPointHeadPresent", (m_endPointHead != null));
            info.AddValue("endPointTailPresent", (m_endPointTail != null));

            if (m_endPointHead != null)
                info.AddValue("endPointHead", m_endPointHead);

            if (m_endPointTail != null)
                info.AddValue("endPointTail", m_endPointTail);

            info.AddValue("lineBridgeSize", m_fLineBridgeSize);
            info.AddValue("lineBridgingEnabled", m_bLineBridgingEnabled);
            info.AddValue("lineRoutingEnabled", m_bLineRoutingEnabled);
            info.AddValue("headingHead", m_headingHead);
            info.AddValue("headingTail", m_headingTail);
            info.AddValue("EnableRoundedCorner", m_bEnableRoundedCorner);
            info.AddValue("ObstaclesInPath", m_bObstaclesInPath);
        }

        /// <summary>
        /// Updates the references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public override void UpdateReferences(IServiceReferenceProvider provider)
        {
            base.UpdateReferences(provider);

            this.HeadEndPoint.UpdateServiceReferences(provider);
            this.TailEndPoint.UpdateServiceReferences(provider);
        }

        /// <summary>
        /// Gets the property container.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the STR property container.</param>
        /// <returns>The property container object.</returns>
        protected override object GetPropertyContainer(string strPropertyContainerName)
        {
            object objToReturn = base.GetPropertyContainer(strPropertyContainerName);

            if (objToReturn == null && strPropertyContainerName == "HeadEndPoint")
            {
                objToReturn = m_endPointHead;
            }
            else if (strPropertyContainerName == "TailEndPoint")
            {
                objToReturn = m_endPointTail;
            }

            return objToReturn;
        }

        /// <summary>
        /// Helper method used to perform additional
        /// actions before updating path node data.
        /// Used by Connectors to merge control points.
        /// </summary>
        /// <param name="ptsNew">The new path points.</param>
        protected override void BeforeMoveSegment(ref PointF[] ptsNew)
        {
            MergeControlPoints(ref ptsNew);
        }

        /// <summary>
        /// Accumulates the refresh rect.
        /// </summary>
        /// <param name="rcRefresh">The refresh rectangle.</param>
        protected override void AccumulateRefreshRect(ref RectangleF rcRefresh)
        {
            base.AccumulateRefreshRect(ref rcRefresh);

            // if line bridging is not enabled -> return
            if (this.Root != null && !this.Root.LineBridgingEnabled) return;

            // BRIDGES
            foreach (ConnectorLineSegment segment in this.LineSegments)
            {
                AccumulateBridgesRects(segment, ref rcRefresh);
            }
        }

        /// <summary>
        /// Checks the new pin point.
        /// </summary>
        /// <param name="ptPinPoint">The pin point.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>
        /// <c>true</c> if this value can set; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CheckNewPinPoint(PointF ptPinPoint, MeasureUnits unit)
        {
            PointF ptUnitIndependentPinPoint = this.BoundsInfo.GetPinPoint(unit);

            // 1 - check for value change
            bool bConstrain = (ptUnitIndependentPinPoint != ptPinPoint);

            // 2 - check for allow change
            if (bConstrain && ptUnitIndependentPinPoint.X != ptPinPoint.X)
                bConstrain = EditStyle.CanMoveX(this);

            if (bConstrain && ptUnitIndependentPinPoint.Y != ptPinPoint.Y)
                bConstrain = EditStyle.CanMoveY(this);
            bool bSuccess = true;

            if (bConstrain)
            {
                // get path points update end points location considering whether they are connected
                PointF[] pts = GetPathPoints();

                // get move offset
                PointF ptOldPin = ((IUnitIndependent)this).GetPinPoint(MeasureUnits.Pixel);

                SizeF szOffset = SizeF.Empty;
                szOffset.Width = ptPinPoint.X - ptOldPin.X;
                szOffset.Height = ptPinPoint.Y - ptOldPin.Y;

                // if end point is connected translate it back
                // ------------------------------------------
                // tail end point
                if (IsTailConnected())
                {
                    pts[0].X -= szOffset.Width;
                    pts[0].Y -= szOffset.Height;
                }

                // head end point
                if (IsHeadConnected())
                {
                    int nLength = pts.Length;
                    pts[nLength - 1].X -= szOffset.Width;
                    pts[nLength - 1].Y -= szOffset.Height;
                }

                Matrix mtxTransforms = HandlesHitTesting.GetParentsTransformations(this, true);
                mtxTransforms.Translate(szOffset.Width, szOffset.Height, MatrixOrder.Append);

                // create graphics path
                GraphicsPath path = CreateGraphicsPath(pts);
                Model model = this.Root;

                // get model bounding rect
                if (model != null && model.BoundaryConstraintsEnabled)
                {
                    RectangleF rcBounds = MeasureUnitsConverter.Convert(
                        model.Bounds, model.MeasurementUnits, MeasureUnits.Pixel);

                    if (!rcBounds.Contains(path.GetBounds(mtxTransforms)))
                    {
                        bSuccess = false;
                    }
                }
            }

            return bSuccess & bConstrain;
        }

        /// <summary>
        /// Updates node's GraphicsPath on given array of points.
        /// </summary>
        /// <param name="pts">The new path points array.</param>
        protected override void UpdateGraphicsPath(PointF[] pts)
        {
            PointF[] ptsPath = (PointF[])pts.Clone();

            Matrix mtxScale = GetScaleTransformation();
            mtxScale.Invert();
            mtxScale.TransformPoints(ptsPath);

            // translate points to local
            Geometry.TranslateToGridOrigin(ptsPath);
            Geometry.TranslateToGridOrigin(pts);

            // update control points list
            UpdateControlPoints(pts, 1);

            m_gpPath = CreateLogicalGraphicsPath(ptsPath);

            // update segments after update graphics path and handles
            UpdateSegments();
        }

        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            GraphicsPath gpPath;

            if (pts.Length == 2)
            {
                float fLineLength = (float)Geometry.PointDistance(pts[0], pts[1]);

                // create new GraphicsPath
                gpPath = new GraphicsPath();
                gpPath.AddLine(0, 0, fLineLength, 0);
            }
            else
            {
                // assign new GraphicsPath
                // get polygon's bounding rectangle
                gpPath = new GraphicsPath();
                gpPath.AddLines(pts);
            }

            return gpPath;
        }

        /// <summary>
        /// Moves the end points to given offset.
        /// </summary>
        /// <param name="szOffset">The move offset.</param>
        protected virtual void MoveEndPoints(SizeF szOffset)
        {
            // move head endPoint
            if (this.HeadEndPoint != null)
                this.HeadEndPoint.Move(szOffset, MeasureUnits.Pixel);

            // move tail endPoint
            if (this.TailEndPoint != null)
                this.TailEndPoint.Move(szOffset, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Set the control points location.
        /// </summary>
        /// <param name="ptsPath">The points path.</param>
        /// <param name="nStartIndex">Index of the beginning control path points.</param>
        protected override void SetControlPoints(PointF[] ptsPath, int nStartIndex)
        {
            bool bMerge = (this.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints;

            if (bMerge)
                this.ConnectorState = this.ConnectorState & (~ConnectorState.MergeControlPoints);

            // update points to local coordinates
            for (int n = 0, nLength = this.ControlPoints.Count; n < nLength; n++)
            {
                ControlPoint cntPoint = (ControlPoint)this.ControlPoints[n];
                PointF ptPoint = ptsPath[n + nStartIndex];

                if (!Geometry.EqualPoints(cntPoint.Location, ptPoint, 4))
                    cntPoint.Location = ptPoint;
            }

            if (bMerge)
                this.ConnectorState |= ConnectorState.MergeControlPoints;
        }

        /// <summary>
        /// Called when node is deserialized.
        /// </summary>
        protected override void OnDeserialized()
        {
            base.OnDeserialized();

            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);
        }
        #endregion

        #region IEndPointContainer Members
        /// <summary>
        /// Gets the head end point.
        /// </summary>
        /// <value>The head end point.</value>
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public EndPoint HeadEndPoint
        {
            get { return m_endPointHead; }
        }

        /// <summary>
        /// Gets the tail end point.
        /// </summary>
        /// <value>The tail end point.</value>
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public EndPoint TailEndPoint
        {
            get { return m_endPointTail; }
        }
        #endregion

        #region IGraphEdge Members
        /// <summary>
        /// Gets node connected to the tail of the edge.
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public IGraphNode FromNode
        {
            get
            {
                Node nodeToReturn = null;

                if (this.TailEndPoint.Port != null)
                {
                    nodeToReturn = this.TailEndPoint.Port.Container;
                }

                return nodeToReturn;
            }
        }

        /// <summary>
        /// Gets node connected to the head of the edge.
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public IGraphNode ToNode
        {
            get
            {
                Node nodeToReturn = null;

                if (this.HeadEndPoint.Port != null)
                {
                    nodeToReturn = this.HeadEndPoint.Port.Container;
                }

                return nodeToReturn;
            }
        }

        /// <summary>
        /// Gets weight value associated with the edge.
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public int EdgeWeight
        {
            get { return 0; }
        }

        /// <summary>
        /// Determines if this edge is leaving the given node.
        /// </summary>
        /// <param name="graphNode">Node to test.</param>
        /// <returns>True if edge is leaving the given node.</returns>
        public bool IsNodeLeaving(IGraphNode graphNode)
        {
            bool bSuccess = false;

            return bSuccess;
        }

        /// <summary>
        /// Determines if this edge is entering the given node.
        /// </summary>
        /// <param name="graphNode">Node to test.</param>
        /// <returns>True if edge is entering the given node.</returns>
        public bool IsNodeEntering(IGraphNode graphNode)
        {
            // TODO:  Add Line.IsNodeEntering implementation
            return false;
        }
        #endregion
    }
}