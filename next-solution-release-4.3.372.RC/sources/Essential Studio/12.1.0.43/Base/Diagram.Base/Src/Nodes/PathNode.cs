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
    /// Basic shape node that contain collection of <see cref="Syncfusion.Windows.Forms.Diagram.ControlPoint"/>,
    /// segments, vertices to draw simple shapes.
    /// </summary>
    /// <remarks>
    /// Used as base class for <see cref="Syncfusion.Windows.Forms.Diagram.FilledPath"/> 
    /// and <see cref="Syncfusion.Windows.Forms.Diagram.LineBase"/> nodes.
    /// </remarks>
    public abstract class PathNode
        : Node
    {
        #region Class constants
        /// <summary>
        /// Constant segments count of bezier shape.
        /// </summary>
        private const int c_nBEZIER_SEGMENT = 3;
        #endregion

        #region Class members
        /// <summary>
        /// Cached regions array for line segments hit testing.
        /// </summary>
        /// <remarks>
        /// Internal usage only. Optimization.
        /// Created on MouseEnter for optimization.
        /// Cleared on MouseLeave.
        /// </remarks>
        private Region[] m_cacheRegionsSegment;

        /// <summary>
        /// Collection of control points. 
        /// </summary>
        private ArrayList m_controlPoints;

        /// <summary>
        /// Indicate that m_gpPath member can be changed.
        /// </summary>
        protected bool m_bCanChangePath;

        /// <summary>
        /// Indicate that node vertex can be changed.
        /// </summary>
        protected bool m_bIsVertexEditable;

        /// <summary>
        /// The maximum of path points.
        /// </summary>
        protected int m_maxPoints;

        /// <summary>
        /// The minimum of path points.
        /// </summary>
        protected int m_minPoints;

        private LabelCollection m_labels;

        /// <summary>
        /// GraphicsPath containing points and instructions for rendering the shape.
        /// </summary>
        protected GraphicsPath m_gpPath;
        private PointF[] m_pathPoints;
        private PointF[] m_pathPointsRelative;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PathNode"/> class.
        /// </summary>
        public PathNode()
            : base()
        {
            m_minPoints = 2;
            m_maxPoints = Int32.MaxValue;
            m_bCanChangePath = true;
            m_bIsVertexEditable = true;
            m_gpPath = new GraphicsPath();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathNode"/> class.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public PathNode(PathNode src)
            : base(src)
        {
            m_gpPath = (GraphicsPath)src.m_gpPath.Clone();
            m_pathPoints = this.GraphicsPath.PathPoints;

            if (src.Labels.Count > 0)
            {
                LabelCollection labels = new LabelCollection(this);
                Label labelTemp;

                foreach (Label label in src.Labels)
                {
                    labelTemp = (Label)label.Clone();
                    labelTemp.Container = this;

                    labels.Add(labelTemp);
                }

                m_labels = labels;
            }

            m_minPoints = src.m_minPoints;
            m_maxPoints = src.m_maxPoints;
            m_bCanChangePath = src.m_bCanChangePath;
            m_bIsVertexEditable = src.m_bIsVertexEditable;

            // clone control points only if there is more than 1
            if (src.ControlPoints.Count > 0)
            {
                ArrayList ctrlPoints = new ArrayList();
                ControlPoint ctrlPointTemp;

                foreach (ControlPoint ctrlPoint in src.ControlPoints)
                {
                    ctrlPointTemp = (ControlPoint)ctrlPoint.Clone();
                    ctrlPointTemp.Container = this;

                    ctrlPoints.Add(ctrlPointTemp);
                }

                m_controlPoints = ctrlPoints;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathNode"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected PathNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Create graphics path
            PointF[] pathPoints = null;
            byte[] pathTypes = null;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "pathPoints":
                        pathPoints = (PointF[])info.GetValue("pathPoints", typeof(PointF[]));
                        break;
                    case "pathTypes":
                        pathTypes = (byte[])info.GetValue("pathTypes", typeof(byte[]));
                        break;
                    case "minPoints":
                        m_minPoints = info.GetInt32("minPoints");
                        break;
                    case "maxPoints":
                        m_maxPoints = info.GetInt32("maxPoints");
                        break;
                    case "canChangePath":
                        m_bCanChangePath = info.GetBoolean("canChangePath");
                        break;
                    case "controlPoints":
                        m_controlPoints = (ArrayList)info.GetValue("controlPoints", typeof(ArrayList));
                        break;
                    case "IsVertexEditable":
                        m_bIsVertexEditable = info.GetBoolean("IsVertexEditable");
                        break;
                    case "labels":
                        LabelCollection labels = (LabelCollection)info.GetValue("labels", typeof(LabelCollection));

                        // clone control points only if there is more than 1
                        if (labels != null && labels.Count > 0)
                        {
                            foreach (object obj in labels)
                            {
                                Label label = obj as Label;

                                if (label != null)
                                {
                                    label.Container = this;
                                }
                            }
                            labels.Container = this;
                        }
                        else
                        {
                            labels = new LabelCollection(this);
                        }

                        m_labels = labels;
                        break;
                }
            }

            if (pathPoints.Length > 0 && pathTypes.Length > 0)
            {
                m_gpPath = new GraphicsPath(pathPoints, pathTypes);
                this.PathPoints = this.GraphicsPath.PathPoints;
            }

            // clone control points only if there is more than 1
            if (m_controlPoints != null && m_controlPoints.Count > 0)
            {
                foreach (ControlPoint ctrlPoint in m_controlPoints)
                {
                    ctrlPoint.Container = this;
                    ctrlPoint.UpdateServiceReferences(this);
                }
            }

            UpdateBoundingRectangle();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the node collection of <see cref="Syncfusion.Windows.Forms.Diagram.Label"/> items.
        /// </summary>
        /// <value>The collection of <see cref="Syncfusion.Windows.Forms.Diagram.Label"/> items.</value>
        [Browsable(true)]
        [Description("Collection of labels.")]
        public LabelCollection Labels
        {
            get
            {
                if (m_labels == null)
                {
                    m_labels = new LabelCollection(this);
                    m_labels.UpdateServiceReferences(this);
                }

                return m_labels;
            }
        }

        /// <summary>
        /// GraphicsPath containing points and instructions for rendering the shape.
        /// </summary>
        protected override GraphicsPath LogicalGraphicsPath
        {
            get
            {
                return m_gpPath;
            }
        }

        /// <summary>
        /// Gets number of vertices contained by the shape.
        /// </summary>
        [Browsable(false)]
        public int PointCount
        {
            get { return GetPathPoints().Length; }
        }

        /// <summary>
        /// Gets the line segment count.
        /// </summary>
        /// <remarks>
        /// Segment count equal to path point count for a closed path, 
        /// and one less count for non-closed shape.
        /// </remarks>
        /// <value>The line segment count.</value>
        [Browsable(false)]
        public virtual int LineSegmentCount
        {
            get
            {
                return this.PathPoints.Length;
            }
        }

        /// <summary>
        /// Gets or sets minimum number of vertices this shape may contain.
        /// </summary>
        [Browsable(true)]
        [Description("Minimal allowed points quantity needed to construct node's path.")]
        public int MinPoints
        {
            get { return m_minPoints; }
            set { m_minPoints = value; }
        }

        /// <summary>
        /// Gets or sets maximum number of vertices this shape may contain.
        /// </summary>
        [Browsable(true)]
        [Description("Maximum allowed points quantity needed to construct node's path.")]
        public int MaxPoints
        {
            get { return m_maxPoints; }
            set { m_maxPoints = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can change path.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can change path; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool CanChangePath
        {
            get { return m_bCanChangePath; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is vertex editable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is vertex editable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool IsVertexEditable
        {
            get { return m_bIsVertexEditable; }
        }

        #region protected
        /// <summary>
        /// Gets or sets the path points cache.
        /// </summary>
        /// <value>The path points array.</value>
        [Browsable(false)]
        protected PointF[] PathPoints
        {
            get
            {
                return m_pathPoints;
            }
            set
            {
                if (m_pathPoints != value)
                {
                    // asign new value
                    m_pathPoints = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the internal relative path points cache.
        /// </summary>
        /// <value>The path points array.</value>
        [Browsable(false)]
        protected PointF[] PathPointsRelative
        {
            get
            {
                return m_pathPointsRelative;
            }
            set
            {
                if (m_pathPointsRelative != value)
                {
                    // asign new value
                    m_pathPointsRelative = value;
                }
            }
        }
        /// <summary>
        /// Gets the collection of <see cref="Syncfusion.Windows.Forms.Diagram.ControlPoint"/> items.
        /// </summary>
        /// <value>The collection of <see cref="Syncfusion.Windows.Forms.Diagram.ControlPoint"/> items.</value>
        [Browsable(false)]
        protected ArrayList ControlPoints
        {
            get
            {
                if (m_controlPoints == null)
                {
                    m_controlPoints = new ArrayList();
                }

                return m_controlPoints;
            }
        }
        #endregion

        #endregion

        #region Class overrides
        /// <summary>
        /// Performs additional changes on size value changed.
        /// </summary>
        /// <param name="szOldSize">Old size value.</param>
        /// <param name="szNewSize">New size value.</param>
        protected override void DoSizeRelatedActions(SizeF szOldSize, SizeF szNewSize)
        {
            // prevent divide by zero
            float fScaleFactorX = (szOldSize.Width == 0) ? 1 : szNewSize.Width / szOldSize.Width;
            float fScaleFactorY = (szOldSize.Height == 0) ? 1 : szNewSize.Height / szOldSize.Height;

            // 2 - apply scale factor to graphicspath and region
            Matrix mtxTransform = new Matrix(fScaleFactorX, 0, 0, fScaleFactorY, 0, 0);

            TransformControlPoints(mtxTransform);

            // update graphics path
            base.DoSizeRelatedActions(szOldSize, szNewSize);
            
            // scale path points
            mtxTransform.TransformPoints(m_pathPoints);
        }

        /// <summary>
        /// Called when node is deserialized.
        /// </summary>
        protected override void OnDeserialized()
        {
            foreach (Label label in this.Labels)
            {
                label.Container = this;
            }

            foreach (ControlPoint cpoint in this.ControlPoints)
            {
                cpoint.Container = this;
            }

            base.OnDeserialized();
        }

        /// <summary>
        /// Updates the references from service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public override void UpdateReferences(IServiceReferenceProvider provider)
        {
            base.UpdateReferences(provider);

            // update control points service references
            foreach (ControlPoint ptCtrl in this.ControlPoints)
            {
                ptCtrl.UpdateServiceReferences(provider);
            }

            this.Labels.UpdateServiceReferences(provider);

            foreach (Label label in this.Labels)
            {
                label.UpdateServiceReferences(provider);
                label.Container = this;
            }
        }

        /// <summary>
        /// The methods used to draw contiguous date. Such as labels or ports.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void RenderContiguousData(Graphics gfx)
        {
            foreach (Label label in this.Labels)
            {
                label.Draw(gfx);
            }

            base.RenderContiguousData(gfx);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            // Since GraphicsPath is node Serializable -- serialize
            // its PathPoints and PathTypes
            info.AddValue("pathPoints", m_gpPath.PathPoints);
            info.AddValue("pathTypes", m_gpPath.PathTypes);

            info.AddValue("minPoints", m_minPoints);
            info.AddValue("maxPoints", m_maxPoints);
            info.AddValue("canChangePath", m_bCanChangePath);
            info.AddValue("controlPoints", m_controlPoints);
            info.AddValue("IsVertexEditable", m_bIsVertexEditable);
            info.AddValue("labels", m_labels);
        }

        /// <summary>
        /// Raises the mouse enter event.
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            // Create regions array for line segments hit testing.
            UpdateSegmentsRegion();
        }

        /// <summary>
        /// Raises the mouse leave event.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Clear region cache.
            m_cacheRegionsSegment = null;
        }

        /// <summary>
        /// Accumulates the refresh rectangle.
        /// </summary>
        /// <param name="rcRefresh">The refresh rectangle.</param>
        protected override void AccumulateRefreshRect(ref RectangleF rcRefresh)
        {
            base.AccumulateRefreshRect(ref rcRefresh);

            // LABELS
            // label location
            PointF ptLLoc;
            
            // label's bounding rect
            RectangleF rcLBounds = RectangleF.Empty;

            // current label's bounding rect
            foreach (Label label in this.Labels)
            {
                if (label == null) continue;

                Font fntTemp = label.FontStyle.CreateFont();
                //// calc label rect
                rcLBounds.Size = label.Size;
                //// get label location
                ptLLoc = label.GetPosition();
                //// update label location
                if (label.UpdatePosition == false)
                {
                    switch (label.Position)
                    {
                        case Position.BottomCenter:
                            ptLLoc = new PointF(ptLLoc.X - rcLBounds.Size.Width / 2, ptLLoc.Y);
                            break;
                        case Position.Center:
                            ptLLoc = new PointF(ptLLoc.X - rcLBounds.Size.Width / 2, ptLLoc.Y - rcLBounds.Size.Height / 2);
                            break;
                        case Position.TopCenter:
                            ptLLoc = new PointF(ptLLoc.X - rcLBounds.Size.Width / 2, ptLLoc.Y - rcLBounds.Size.Height);
                            break;
                        case Position.TopLeft:
                            ptLLoc = new PointF(ptLLoc.X, ptLLoc.Y - rcLBounds.Size.Height);
                            break;
                        case Position.MiddleLeft:
                            ptLLoc = new PointF(ptLLoc.X - rcLBounds.Size.Width, ptLLoc.Y - rcLBounds.Size.Height / 2);
                            break;
                        case Position.MiddleRight:
                            ptLLoc = new PointF(ptLLoc.X, ptLLoc.Y - rcLBounds.Size.Height / 2);
                            break;
                        case Position.TopRight:
                            ptLLoc = new PointF(ptLLoc.X - rcLBounds.Size.Width, ptLLoc.Y - rcLBounds.Size.Height);
                            break;
                        case Position.BottomRight:
                            ptLLoc = new PointF(ptLLoc.X - rcLBounds.Size.Width, ptLLoc.Y);
                            break;
                    }
                }
                rcLBounds.Location = ptLLoc;

                // merge port bounding rect with current refresh rect
                if (rcRefresh.Size.IsEmpty)
                {
                    rcRefresh = rcLBounds;
                }
                else
                {
                    rcRefresh = RectangleF.Union(rcRefresh, rcLBounds);
                }
            }
        }

        /// <summary>
        /// Called when node scale factor is changed.
        /// </summary>
        /// <param name="strPropertyName">The property name that change scale factor value.</param>
        protected override void OnNodeScaleChanged(string strPropertyName)
        {
            // recreate path points
            m_pathPoints = this.GraphicsPath.PathPoints;

            base.OnNodeScaleChanged(strPropertyName);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Determines whether given control point is valid.
        /// </summary>
        /// <param name="ctrlPtValidating">The control point to validating.</param>
        /// <returns>
        /// <c>true</c> if given control point is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsControlPointValid(ControlPoint ctrlPtValidating)
        {
            return this.ControlPoints.Contains(ctrlPtValidating);
        }

        /// <summary>
        /// Does the handle move related.
        /// </summary>
        /// <param name="handleMoved">The handle moved.</param>
        /// <param name="szOffset">The move offset.</param>
        protected virtual void DoHandleMoveRelated(IHandle handleMoved, SizeF szOffset)
        {
            UpdatePathNodeData();
        }

        /// <summary>
        /// Handles the move.
        /// </summary>
        /// <param name="handleMoved">The moving handle.</param>
        /// <param name="szOffset">Handle move offset.</param>
        protected void HandleMove(IHandle handleMoved, SizeF szOffset)
        {
            if (!m_bLockUpdate)
            {
                m_bLockUpdate = true;
                SafeHistoryPause();

                DoHandleMoveRelated(handleMoved, szOffset);

                SafeHistoryResume();
                m_bLockUpdate = false;
            }
        }

        /// <summary>
        /// Transforms the node control points.
        /// </summary>
        /// <param name="mtxTransform">Matrix transformations.</param>
        protected void TransformControlPoints(Matrix mtxTransform)
        {
            // Create points array with one point for transform.
            PointF[] pts = new PointF[1];

            // Update control points.
            for (int i = 0, nLength = this.ControlPoints.Count; i < nLength; i++)
            {
                pts[0] = ((ControlPoint)this.ControlPoints[i]).Location;

                // Scale control point.
                mtxTransform.TransformPoints(pts);

                // Set scaled point as control location.
                ((ControlPoint)this.ControlPoints[i]).Location = pts[0];
            }
        }

        /// <summary>
        /// Updates the segments region cache.
        /// </summary>
        /// <remarks>
        /// Create region segment cache and save to m_cacheRegionsSegment.
        /// </remarks>
        protected void UpdateSegmentsRegion()
        {
            if (CanEditSegment())
                m_cacheRegionsSegment = CreateSegmentsRegions();
        }

        /// <summary>
        /// Updates the graphics path.
        /// </summary>
        protected void UpdatePathNodeData()
        {
            // get array of points to construct GraphicsPath from.
            PointF[] pts = GetPathPoints();

            // get new bounds from points.
            RectangleF rectPathBounding = GetNewBounds(pts);

            // update BoundsInfo
            UpdateBoundsInfo(rectPathBounding);

            // update graphics path with new path points.
            UpdateGraphicsPath(pts);

            // update PathPoints
            this.PathPoints = this.GraphicsPath.PathPoints;

            // reset hittesting region.
            m_rgnCache = null;

            // update cached bounding rectangle.
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Gets array of points needed to construct node's GraphicsPath.
        /// </summary>
        /// <returns>Array of path points</returns>
        protected virtual PointF[] GetPathPoints()
        {
            return (PointF[])this.PathPoints.Clone();
        }

        /// <summary>
        /// Gets the transformed path points.
        /// </summary>
        /// <param name="mtxTransform">Matrix transformations.</param>
        /// <returns>Array of transformed path points.</returns>
        protected PointF[] GetPathPoints(Matrix mtxTransform)
        {
            PointF[] pts = GetPathPoints();
            mtxTransform.TransformPoints(pts);

            return pts;
        }

        /// <summary>
        /// Updates node's GraphicsPath on given array of points.
        /// </summary>
        /// <param name="pts">The new path points array.</param>
        protected virtual void UpdateGraphicsPath(PointF[] pts)
        { 
        }

        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected virtual GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            return null;
        }

        /// <summary>
        /// Creates the graphics path from path points.
        /// </summary>
        /// <param name="pts">The path points array.</param>
        /// <returns>Graphics path</returns>
        protected GraphicsPath CreateGraphicsPath(PointF[] pts)
        {
            GraphicsPath path = CreateLogicalGraphicsPath(pts);

            if (path != null)
            {
                Matrix mtxScale = this.GetScaleTransformation();
                path.Transform(mtxScale);
            }

            return path;
        }

        /// <summary>
        /// Updates the bounds info.
        /// </summary>
        /// <param name="rcBounds">The bounds rectangle.</param>
        protected virtual void UpdateBoundsInfo(RectangleF rcBounds)
        {
            UpdateNonLineBoundsInfo(rcBounds);
        }

        /// <summary>
        /// Prepares new path points
        /// </summary>
        /// <param name="ptIdx">The point index.</param>
        /// <param name="val">The point.</param>
        /// <returns>The new path points.</returns>
        /// <remarks>
        /// You should override this method if custom login needed
        /// (e.g. - if You are inserting point into orthogonal line -&gt; two points must be inserted!)
        /// </remarks>
        protected virtual PointF[] PreparePathPointsForInsert(int ptIdx, PointF val)
        {
            int nLength = this.PathPoints.Length;
            //// copy part of array
            PointF[] ptsNew = new PointF[nLength + 1];
            Array.Copy(this.PathPoints, ptsNew, ptIdx);
            //// add new point
            ptsNew[ptIdx] = val;
            //// copy remaining part of array
            Array.Copy(this.PathPoints, ptIdx, ptsNew, ptIdx + 1, nLength - ptIdx);

            return ptsNew;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Searches for adjacent point from points that forms pathnode's path.
        /// </summary>
        /// <param name="ptPathPoint">Point to search adjacent point to.</param>
        /// <param name="bInLocalCoordinates">Indicates whether adjacent point is in local coordinates.</param>
        /// <param name="bLeftHand">Indicates whether adjacent point is previous or next to given point.</param>
        /// <param name="ptAdjacentPoint">Adjacent point.</param>
        /// <returns>Value indicating whether search was successful.</returns>
        public bool GetAdjacentPathPoint(PointF ptPathPoint, bool bInLocalCoordinates, bool bLeftHand, out PointF ptAdjacentPoint)
        {
            bool bSuccess = false;
            
            // point to return
            ptAdjacentPoint = PointF.Empty;

            // update point to node's coordinates
            if (!bInLocalCoordinates)
                ptPathPoint = ConvertToNodeCoordinates(ptPathPoint);

            ptPathPoint.X = (float)Math.Round(ptPathPoint.X, 2);
            ptPathPoint.Y = (float)Math.Round(ptPathPoint.Y, 2);
            
            // get points to search among
            PointF[] pts = GetPathPoints();
            PointF ptTemp;

            // perform search
            for (int i = 0, nLength = pts.Length; nLength > i; i++)
            {
                ptTemp = pts[i];

                ptTemp.X = (float)Math.Round(ptTemp.X, 2);
                ptTemp.Y = (float)Math.Round(ptTemp.Y, 2);

                if (ptPathPoint == ptTemp)
                {
                    if (bLeftHand && i > 0)
                    {
                        ptAdjacentPoint = pts[i - 1];
                        bSuccess = true;
                        break;
                    }
                    else if (!bLeftHand && i < nLength - 1)
                    {
                        ptAdjacentPoint = pts[i + 1];
                        bSuccess = true;
                        break;
                    }
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Determines whether this node can edit segments.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit segments; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanEditSegment()
        {
            return false;
        }

        /// <summary>
        /// Determines whether this node can edit control points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit control points; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanEditControlPoint()
        {
            return false;
        }

        /// <summary>
        /// Determines whether this node can edit vertex points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit vertex point; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanEditVertexPoint()
        {
            return false;
        }

        /// <summary>
        /// Determines whether this node allow move it handle.
        /// </summary>
        /// <param name="handle">The handle to move.</param>
        /// <returns>
        /// <c>true</c> if this node allow move it handle; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMoveHandle(IHandle handle)
        {
            return CanMoveHandle(handle, handle.Location);
        }

        /// <summary>
        /// Determines whether this node allow move it handle.
        /// </summary>
        /// <param name="handle">The handle to move.</param>
        /// <param name="ptNewLocation">The new endpoint location to check.</param>
        /// <returns>
        /// <c>true</c> if this node allow move it handle; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanMoveHandle(IHandle handle, PointF ptNewLocation)
        {
            return ContainsHandle(handle);
        }

        /// <summary>
        /// Determines whether the node contains specific handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>
        /// <c>true</c> if the handle contains specific  handle; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ContainsHandle(IHandle handle)
        {
            return this.ControlPoints.Contains(handle);
        }

        /// <summary>
        /// Gets the array of control points.
        /// </summary>
        /// <returns>Control points array.</returns>
        public ControlPoint[] GetControlPoints()
        {
            return (ControlPoint[])this.ControlPoints.ToArray(typeof(ControlPoint));
        }

        /// <summary>
        /// Determines whether this node can draw control points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can draw control points; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanDrawControlPoints()
        {
            return false;
        }

        /// <summary>
        /// Draws the control points.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public virtual void DrawControlPoints(Graphics gfx)
        {
            if (this.ControlPoints.Count <= 0)
                return;

            // Control points are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleSize;
            if (HandlesHitTesting.TouchMode)
                fHandleSize = CommonUsedValues.DEF_CONTROL_POINT_TOUCH_SIZE / fPageScale;
            else
                fHandleSize = CommonUsedValues.RESIZE_HANDLE_SIZE / fPageScale;

            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            // Create brush to fill handle interiors
            using (SolidBrush brushHandle = new SolidBrush(Color.Yellow))
            
                // Create pen to draw handle outline
            using (Pen penHandleOutline = new Pen(Color.Black, 1f / fPageScale))
            {
                PointF ptUpperLeft = GetUpperLeftPoint(MeasureUnits.Pixel);
                gfx.TranslateTransform(ptUpperLeft.X, ptUpperLeft.Y);

                Matrix mtxLocalTransform = GetLocalTransformations();
                AppendLocalFlipTransforms(mtxLocalTransform);
                gfx.MultiplyTransform(mtxLocalTransform);

                // Draw control points
                foreach (ControlPoint ctrlPoint in this.ControlPoints)
                {
                    DrawControlPoint(gfx, ctrlPoint, new SizeF(fHandleSize, fHandleSize), penHandleOutline, brushHandle);
                }
            }

            // Restore Graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Draw the control point on graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="ctrlPoint">The control point.</param>
        /// <param name="szHandleSize">Size of the handle.</param>
        /// <param name="penHandleOutline">The pen handle outline.</param>
        /// <param name="brushHandle">The fill brush handle.</param>
        public virtual void DrawControlPoint(Graphics gfx, ControlPoint ctrlPoint, SizeF szHandleSize, Pen penHandleOutline, Brush brushHandle)
        {
            PointF ptContolPoint = ctrlPoint.Location;
            //// Create control point shape( rhomb )
            PointF[] pts = Geometry.CreateRhomb(ptContolPoint, szHandleSize);
            //// Fill handle interiors
            gfx.FillPolygon(brushHandle, pts);
            //// Outline control point
            gfx.DrawPolygon(penHandleOutline, pts);
        }

        /// <summary>
        /// Gets the control point at position.
        /// </summary>
        /// <param name="ptTesting">The given point.</param>
        /// <returns>Control point at the specified point.</returns>
        public ControlPoint GetControlPointAt(PointF ptTesting)
        {
            ControlPoint ctrlPointToReturn = null;
            RectangleF rectTemp;
            PointF[] pts = new PointF[1];
            Matrix matrixParents = HandlesHitTesting.GetParentsTransformations(this);

            foreach (ControlPoint ctrlPoint in this.ControlPoints)
            {
                PointF ptControl = MeasureUnitsConverter.Convert(ctrlPoint.Location, ctrlPoint.MeasureUnit, this.MeasurementUnit);
                Matrix mtxLocalTransform = GetLocalTransformations();
                AppendLocalFlipTransforms(mtxLocalTransform);
                ptControl = Geometry.AppendMatrix(ptControl, mtxLocalTransform);

                ptControl.X += this.PinPoint.X - this.PinPointOffset.Width;
                ptControl.Y += this.PinPoint.Y - this.PinPointOffset.Height;

                pts[0] = ptControl;
                matrixParents.TransformPoints(pts);
                pts[0] = MeasureUnitsConverter.ToPixels(pts[0], this.MeasurementUnit);
                float handleSize;
                if (HandlesHitTesting.TouchMode)
                {
                    handleSize = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE;
                }
                else
                    handleSize = CommonUsedValues.RESIZE_HANDLE_SIZE ;
                rectTemp = Geometry.CreateRect(pts[0], new SizeF(handleSize, handleSize));

                if (rectTemp.Contains(ptTesting))
                {
                    ctrlPointToReturn = ctrlPoint;
                    break;
                }
            }

            return ctrlPointToReturn;
        }

        /// <summary>
        /// Gets the control point by list index.
        /// </summary>
        /// <param name="nIndex">Point list index.</param>
        /// <returns>Control point at the specified index.</returns>
        public ControlPoint GetControlPointAt(int nIndex)
        {
            return this.ControlPoints[nIndex] as ControlPoint;
        }

        /// <summary>
        /// Gets the control point by ID.
        /// </summary>
        /// <param name="nID">The control point unique ID.</param>
        /// <returns>The <see cref="Syncfusion.Windows.Forms.Diagram.ControlPoint"/>.</returns>
        public ControlPoint GetControlPointByID(int nID)
        {
            ControlPoint ctrlPointToReturn = null;

            foreach (ControlPoint point in this.ControlPoints)
            {
                if (point.ID == nID)
                {
                    ctrlPointToReturn = point;
                    break;
                }
            }

            return ctrlPointToReturn;
        }

        /// <summary>
        /// Tries to combine given graphics path with current.
        /// </summary>
        /// <param name="pathToCombineWith">The path to combine with.</param>
        /// <param name="bConnect">if set to <c>true</c> connect.</param>
        /// <returns>
        /// If path's combined successfully - true, otherwise - false
        /// </returns>
        /// <remarks>
        /// All combining path pathPoints must be in GraphicsUnit.Pixel.
        /// </remarks>
        public bool TryCombine(GraphicsPath pathToCombineWith, bool bConnect)
        {
            bool bSuccess = false;

            if (CanCombine(pathToCombineWith))
            {
                PointF[] pts = (PointF[])pathToCombineWith.PathPoints.Clone();
                RectangleF rect = pathToCombineWith.GetBounds();

                for (int nCounter = 0, nLength = pts.Length; nCounter < nLength; nCounter++)
                {
                    pts[nCounter].X -= rect.X;
                    pts[nCounter].Y -= rect.Y;
                }

                pathToCombineWith = new GraphicsPath(pts, pathToCombineWith.PathTypes);
                
                // combine paths
                GraphicsPath pathCombined = (GraphicsPath)pathToCombineWith.Clone();
                GraphicsPath pathCurClone = this.GraphicsPath;

                if (pathCurClone.PointCount > 0)
                    pathCombined.AddPath(pathCurClone, bConnect);

                rect = pathCombined.GetBounds();
                UpdatePathLocation(ref pathCombined, rect);

                // update size
                ((IUnitIndependent)this).SetSize(rect.Size, MeasureUnits.Pixel);

                // iterate through new path's path data and check 
                // whether vertex edit mode can be activated
                m_bIsVertexEditable = IsPathVertexEditable(pathCombined);

                RecordSetGraphicsPath(pathCurClone, pathToCombineWith, bConnect);

                this.PathPoints = (PointF[])pathCombined.PathPoints.Clone();

                m_gpPath = pathCombined;
                m_rgnCache = null;

                bSuccess = true;
            }

            return bSuccess;
        }

        /// <summary>
        /// Determines whether this instance can combine the specified path to combine with.
        /// </summary>
        /// <param name="pathToCombineWith">The path to combine with.</param>
        /// <returns>
        /// <c>true</c> if this instance can combine the specified path to combine with; otherwise, <c>false</c>.
        /// </returns>
        public bool CanCombine(GraphicsPath pathToCombineWith)
        {
            bool bSuccess = this.CanChangePath;
            Model document = this.Root;

            if (bSuccess && document != null)
            {
                // combine paths
                GraphicsPath pathCombined = (GraphicsPath)pathToCombineWith.Clone();
                GraphicsPath pathCurClone = this.GraphicsPath;

                pathCombined.AddPath(pathCurClone, false);

                // get transformations
                Matrix matrix = GetTransformations();
                AppendFlipTransforms(matrix);

                // transform path
                RectangleF rectBoudning = pathCombined.GetBounds(matrix);

                if (document.BoundaryConstraintsEnabled)
                {
                    RectangleF rectDocumentBounds = MeasureUnitsConverter.ToPixels(rectBoudning, document.MeasurementUnits);

                    if (!rectDocumentBounds.Contains(rectBoudning))
                    {
                        bSuccess = false;
                    }
                }
            }

            return bSuccess;
        }
        #endregion

        #region Class points methods
        /// <summary>
        /// Returns an array containing all vertices belonging to this shape.
        /// </summary>
        /// <param name="bInLocal">Set to <c>true</c> to get points in local coordinates, otherwise - <b>false</b>.</param>
        /// <returns>
        /// The <see cref="System.Drawing.PointF"/>.
        /// </returns>
        public virtual PointF[] GetPoints(bool bInLocal)
        {
            PointF[] pts = GetPoints();

            if (!bInLocal)
            {
                for (int i = 0, length = pts.Length; i < length; i++)
                {
                    pts[i] = ConvertToModelCoordinates(pts[i]);
                }
            }

            return pts;
        }

        /// <summary>
        /// Returns an array containing all vertices belonging to this shape.
        /// </summary>
        /// <returns>Array of points in local coordinates.</returns>
        public virtual PointF[] GetPoints()
        {
            // Return clone of points array.
            return (PointF[])this.PathPoints.Clone();
        }

        /// <summary>
        /// Sets the new points.
        /// </summary>
        /// <param name="pts">The points to set.</param>
        /// <param name="bInLocal">Set to <c>true</c> if given points in local coordinates, otherwise - <b>false</b>.</param>
        public void SetPoints(PointF[] pts, bool bInLocal)
        {
            if (!bInLocal)
            {
                for (int i = 0, length = pts.Length; i < length; i++)
                {
                    pts[i] = ConvertToNodeCoordinates(pts[i]);
                }
            }

            SetPoints(pts);
        }

        /// <summary>
        /// Sets the new points in local coordinates.
        /// </summary>
        /// <param name="pts">The local points.</param>
        public void SetPoints(PointF[] pts)
        {
            if (!m_bLockUpdate && pts.Length <= this.MaxPoints)
            {
                if (CanChangePoints(pts) && OnPropertyChanging(this.FullContainerName, DPN.PathPoints, pts))
                {
                    m_bLockUpdate = true;
                    HistoryManager mngHistory = this.HistoryManager;

                    if (mngHistory != null)
                    {
                        mngHistory.StartAtomicAction("Update Path");
                    }

                    // record pending changes
                    RecordSetPoints();

                    // set new points
                    SetPointsInternal(pts);

                    // quiet update node path
                    SafeHistoryPause();
                    UpdatePathNodeData();
                    SafeHistoryResume();

                    FinishSetPoints();

                    if (mngHistory != null)
                    {
                        mngHistory.EndAtomicAction();
                    }

                    m_bLockUpdate = false;
                    OnPropertyChanged(this.FullContainerName, DPN.PathPoints);
                }
            }
        }

        /// <summary>
        /// Finishes the set points operation.
        /// </summary>
        protected virtual void FinishSetPoints()
        {
            // update connections and update container
            if (m_mgrLink != null)
                m_mgrLink.SynchronizeNodeConnections(this);

            // update cache segments regions
            UpdateSegmentsRegion();
        }

        /// <summary>
        /// Sets the points internal.
        /// </summary>
        /// <param name="ptsPath">The new path points.</param>
        protected virtual void SetPointsInternal(PointF[] ptsPath)
        {
            this.PathPoints = ptsPath;
            this.PathPointsRelative = ptsPath;
        }

        /// <summary>
        /// Moves the segment to given offset.
        /// </summary>
        /// <param name="lineSegmentIndex">Index of the line segment.</param>
        /// <param name="szOffset">The segment move offset.</param>
        /// <returns>The points to offset.</returns>
        protected virtual PointF[] MoveSegment(int lineSegmentIndex, SizeF szOffset)
        {
            PointF[] pts = GetPathPoints();

            // Get segment vertex index.
            int nStartPointIndex = lineSegmentIndex;
            int nEndPointIndex = (lineSegmentIndex + 1) % pts.Length;

            // Get segments points.
            PointF ptStartPoint = pts[nStartPointIndex];
            PointF ptEndPoint = pts[nEndPointIndex];

            // Offset first point
            ptStartPoint.X += szOffset.Width;
            ptStartPoint.Y += szOffset.Height;

            // Offset second point.
            ptEndPoint.X += szOffset.Width;
            ptEndPoint.Y += szOffset.Height;

            // Append moved point to path points.
            pts[nStartPointIndex] = ptStartPoint;
            pts[nEndPointIndex] = ptEndPoint;

            return pts;
        }

        /// <summary>
        /// Gets the vertex point in local coordinates.
        /// </summary>
        /// <param name="ptIdx">The vertex index.</param>
        /// <returns>
        /// The <see cref="System.Drawing.PointF"/>.
        /// </returns>
        public virtual PointF GetPoint(int ptIdx)
        {
            // Return from array members.
            return this.PathPoints[ptIdx];
        }
        /// <summary>
        /// Gets the vertex point in local coordinates. Set the boolean parameter to True to get the Graphical path for line connector.
        /// </summary>
        /// <param name="ptIdx">The vertex index.</param>
        /// <param name="bReturnGraphical">Determines whether the points should be relative to grpah.</param>
        /// <returns>
        /// The <see cref="System.Drawing.PointF"/>.
        /// </returns>

        public virtual PointF GetPoint(int ptIdx,bool bReturnGraphical)
        {
            if (bReturnGraphical)
            {
                return this.PathPoints[ptIdx];
            }
            else
            {
                if (this.PathPoints.Length == 2)
                {
                    return this.PathPointsRelative[ptIdx];
                }
                else
                {
                    return this.PathPoints[ptIdx];
                }
            }
        }
        /// <summary>
        /// Sets the point new value.
        /// </summary>
        /// <param name="ptIdx">The vertex index.</param>
        /// <param name="val">The point to set.</param>
        /// <param name="bInLocal">Set to <c>true</c> if given point in local coordinates, otherwise - <b>false</b>.</param>
        public void SetPoint(int ptIdx, PointF val, bool bInLocal)
        {
            if (!bInLocal)
            {
                val = ConvertToNodeCoordinates(val);
            }

            SetPoint(ptIdx, val);
        }

        /// <summary>
        /// Sets the point new value.
        /// </summary>
        /// <param name="ptIdx">The vertex index.</param>
        /// <param name="val">The new value point in local coordinates.</param>
        public virtual void SetPoint(int ptIdx, PointF val)
        {
            if (0 > ptIdx || ptIdx >= this.PathPoints.Length)
                throw new ArgumentOutOfRangeException("path points");

            // Get path points.
            PointF[] pts = (PointF[])this.PathPoints.Clone();

            // Set point to path points.
            pts[ptIdx] = val;

            PointF ptOffset = PointF.Empty;
            ptOffset.X = val.X - this.PathPoints[ptIdx].X;
            ptOffset.Y = val.Y - this.PathPoints[ptIdx].Y;

            if (OnVertexChanging(VertexChangeType.Set, ptIdx, val) && CanChangePoints(pts))
            {
                //Record vertex change
                RecordVertexChanged(VertexChangeType.Set, ptIdx, val);

                SetPoints(pts);
                
                // Update cached bounding rect
                UpdateBoundingRectangle();
                
                // Raise VertexChanged event
                OnVertexChanged(VertexChangeType.Set, ptIdx, val);
            }
        }

        /// <summary>
        /// Adds the point to the <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/>.
        /// </summary>
        /// <param name="val">The <see cref="System.Drawing.PointF"/> to add to the <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/>.</param>
        /// <param name="bInLocal">Set to <c>true</c> if given point in local coordinates, otherwise - <b>false</b>.</param>
        public void AddPoint(PointF val, bool bInLocal)
        {
            if (!bInLocal)
            {
                val = ConvertToNodeCoordinates(val);
            }

            AddPoint(val);
        }

        /// <summary>
        /// Adds the point to the <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/>.
        /// </summary>
        /// <param name="val">The <see cref="System.Drawing.PointF"/> to add to the <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/> in local coordinates.</param>
        public virtual void AddPoint(PointF val)
        {
            int nLength = this.PathPoints.Length;

            if (this.MaxPoints > nLength)
            {
                PointF[] ptsNew = new PointF[nLength + 1];
                Array.Copy(this.PathPoints, ptsNew, nLength);

                // add new point
                ptsNew[nLength] = val;

                if (OnVertexChanging(VertexChangeType.Insert, nLength, val) && CanChangePoints(ptsNew))
                {
                    SetPoints(ptsNew);
                    
                    // Update cached bounding rect
                    UpdateBoundingRectangle();
                    
                    // Raise VertexChanged event
                    OnVertexChanged(VertexChangeType.Insert, nLength, val);
                }
            }
        }

        /// <summary>
        /// Inserts the point to the <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/> at the specified index.
        /// </summary>
        /// <param name="ptIdx">The zero-based index ar which <paramref name="ptIdx"/> should be inserted.</param>
        /// <param name="val">The point to insert.</param>
        /// <param name="bInLocal">Set to <c>true</c> if given point in local coordinates, otherwise - <b>false</b>.</param>
        public void InsertPoint(int ptIdx, PointF val, bool bInLocal)
        {
            if (!bInLocal)
            {
                val = ConvertToNodeCoordinates(val);
            }

            InsertPoint(ptIdx, val);
        }

        /// <summary>
        /// Inserts the point to the <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/> at the specified index.
        /// </summary>
        /// <param name="ptIdx">The zero-based index ar which <paramref name="ptIdx"/> should be inserted.</param>
        /// <param name="val">The point in local coordinates.</param>
        public virtual void InsertPoint(int ptIdx, PointF val)
        {
            int nLength = this.PathPoints.Length;

            if (0 > ptIdx || ptIdx > nLength)
                throw new ArgumentOutOfRangeException("path points");

            if (this.MaxPoints > nLength && OnVertexChanging(VertexChangeType.Insert, ptIdx, val))
            {
                PointF[] ptsNew = PreparePathPointsForInsert(ptIdx, val);

                if (m_mgrLink != null)
                    m_mgrLink.BeginSynchronization();

                SetPoints(ptsNew);
                
                // Raise VertexChanged event
                OnVertexChanged(VertexChangeType.Insert, ptIdx, val);

                if (m_mgrLink != null)
                    m_mgrLink.EndSynchronization();
            }
        }

        /// <summary>
        /// Removes the path point by it id.
        /// </summary>
        /// <param name="ptIdx">The vertex index.</param>
        public virtual void RemovePoint(int ptIdx)
        {
            int nLength = this.PathPoints.Length;

            if (0 > ptIdx || ptIdx >= nLength)
                throw new ArgumentOutOfRangeException("path points");

            PointF ptRemovingVertexLocation = this.PathPoints[ptIdx];

            if (this.MinPoints <= nLength - 1 && OnVertexChanging(VertexChangeType.Remove, ptIdx, ptRemovingVertexLocation))
            {
                // copy part of array
                PointF[] ptsNew = new PointF[nLength - 1];
                Array.Copy(this.PathPoints, ptsNew, ptIdx);
                
                // copy remaining part of array
                Array.Copy(this.PathPoints, ptIdx + 1, ptsNew, ptIdx, nLength - (ptIdx + 1));

                if (m_mgrLink != null)
                    m_mgrLink.BeginSynchronization();

                SetPoints(ptsNew);
               
                // Update cached bounding rect
                UpdateBoundingRectangle();

                // Raise VertexChanged event
                OnVertexChanged(VertexChangeType.Remove, ptIdx, ptRemovingVertexLocation);

                if (m_mgrLink != null)
                    m_mgrLink.EndSynchronization();
            }
        }

        /// <summary>
        /// Gets the line segment at point it local coordinates.
        /// </summary>
        /// <param name="ptTesting">The point testing.</param>
        /// <returns>return the value for the Line segment.</returns>
        public virtual int GetLineSegmentAtPoint(PointF ptTesting)
        {
            int nSegmentID = -1;

            PointF[] pts = new PointF[1] { ptTesting };

            Matrix mtxParent = GetParentTransformation(false);
            mtxParent.Invert();
            mtxParent.TransformPoints(pts);
            RectangleF rect = this.BoundingRect;
            if (HandlesHitTesting.TouchMode)
                rect.Inflate(CommonUsedValues.TOUCH__HIT_TEST_PADDING, CommonUsedValues.TOUCH__HIT_TEST_PADDING);
            else
                rect.Inflate(this.LineHitTestPadding, this.LineHitTestPadding);

            if (CanEditSegment() && rect.Contains(ptTesting))
            {
                UpdateSegmentsRegion();

                if (m_cacheRegionsSegment != null && m_cacheRegionsSegment.Length > 0)
                {
                    for (int i = 0, len = m_cacheRegionsSegment.Length; i < len; i++)
                    {
                        Region rgnSegment = m_cacheRegionsSegment[i];

                        if (rgnSegment.IsVisible(pts[0]))
                        {
                            nSegmentID = i;
                            break;
                        }
                    }
                }
            }

            return nSegmentID;
        }

        /// <summary>
        /// Gets the segment points.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment.</param>
        /// <returns>One-dimension array with two elements where 
        /// first element is start point and second is end point.</returns>
        public virtual PointF[] GetLineSegmentPoints(int segmentIndex)
        {
            int segmentsCount = this.LineSegmentCount;
            PointF[] ptsReturn;
            PointF ptStart = PointF.Empty, ptEnd = PointF.Empty;
            // Check for contains.
            if (segmentIndex < 0 || segmentIndex >= segmentsCount)
                throw new ArgumentOutOfRangeException();

            // Calculate first and last point of segment.
            int firstPoint = segmentIndex;
            int lastPoint = (segmentIndex + 1) % segmentsCount;

            PointF[] pathPts = this.GetPathPoints();
            if (lastPoint < pathPts.Length)
            {
                ptStart = pathPts[firstPoint];
                ptEnd = pathPts[lastPoint];
            }
            // Create points array with first and last point.
            ptsReturn = new PointF[] { ptStart, ptEnd };

            // Append transformations.
            Matrix matrix = GetTransformations();
            this.AppendFlipTransforms(matrix);
            matrix.TransformPoints(ptsReturn);

            return ptsReturn;
        }

        /// <summary>
        /// Moves the line segment.
        /// </summary>
        /// <param name="lineSegmentIndex">Index of the line segment.</param>
        /// <param name="szOffset">The size offset.</param>
        /// <returns>If move operation is succeed.</returns>
        public bool MoveLineSegment(int lineSegmentIndex, SizeF szOffset)
        {
            bool bSuccess = false;

            if (lineSegmentIndex != -1)
            {
                ValidateOffset(lineSegmentIndex, ref szOffset);
                PointF[] ptsNew = MoveSegment(lineSegmentIndex, szOffset);

                if (CanChangePoints(ptsNew))
                {
                    BeforeMoveSegment(ref ptsNew);

                    SetPoints(ptsNew);

                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Helper method used to perform additional
        /// actions before updating path node data.
        /// Used by Connectors to merge control points.
        /// </summary>
        /// <param name="ptsNew">The new path points.</param>
        protected virtual void BeforeMoveSegment(ref PointF[] ptsNew)
        { 
        }

        /// <summary>
        /// Validates the offset.
        /// </summary>
        /// <param name="lineSegmentIndex">Index of the line segment.</param>
        /// <param name="szOffset">The move offset.</param>
        protected virtual void ValidateOffset(int lineSegmentIndex, ref SizeF szOffset)
        { 
        }

        /// <summary>
        /// Update the control points using given path points.
        /// </summary>
        /// <param name="ptsPath">The array of points.</param>
        /// <param name="nStartIndex">Index of the beginning control path points.</param>
        protected void UpdateControlPoints(PointF[] ptsPath, int nStartIndex)
        {
            if (this.ControlPoints != null)
            {
                // pause rasing event for a while
                if (this.EventSink != null)
                    this.EventSink.Pause();

                // update points to local coordinates
                SetControlPoints(ptsPath, nStartIndex);

                // resume rasing
                if (this.EventSink != null)
                    this.EventSink.Resume();
            }
        }

        /// <summary>
        /// Set the control points location.
        /// </summary>
        /// <param name="ptsPath">The points path.</param>
        /// <param name="nStartIndex">Index of the beginning control path points.</param>
        protected virtual void SetControlPoints(PointF[] ptsPath, int nStartIndex)
        {
            for (int n = 0, nLength = this.ControlPoints.Count; n < nLength; n++)
            {
                ((ControlPoint)this.ControlPoints[n]).Location = ptsPath[n + nStartIndex];
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Updates the non line bounds info.
        /// </summary>
        /// <param name="rcBounds">The bounds rectangle.</param>
        protected void UpdateNonLineBoundsInfo(RectangleF rcBounds)
        {
            // 1 - Get pin point, pin offset and size.
            PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            SizeF szPinOffset = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);
            SizeF szSize = this.BoundsInfo.GetSize(MeasureUnits.Pixel);

            // 2 - Calc pin offset factor.
            float fWidthFactor = (szSize.Width != 0) ? szPinOffset.Width / szSize.Width : 0.5f;
            float fHeightFactor = (szSize.Height != 0) ? szPinOffset.Height / szSize.Height : 0.5f;

            // 3 - calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rcBounds.Width * fWidthFactor, rcBounds.Height * fHeightFactor);

            // 4 - assign default PinLocation
            PointF ptPinPointUnitIndependent = Point.Empty;

            ptPinPointUnitIndependent.X = this.FlipX ? rcBounds.Right - szPinOffsetUnitIndependent.Width
                : rcBounds.X + szPinOffsetUnitIndependent.Width;
            ptPinPointUnitIndependent.Y = this.FlipY ? rcBounds.Bottom - szPinOffsetUnitIndependent.Height
                : rcBounds.Y + szPinOffsetUnitIndependent.Height;

            // 5 - Rotate pin around old.
            UpdatePinPoint(ptPinPoint, ref ptPinPointUnitIndependent);

            // 6 - assign node size value
            SizeF szSizeUnitIndependent = rcBounds.Size;

            // 7 - update BoundsInfo
            this.BoundsInfo.SetSize(szSizeUnitIndependent, MeasureUnits.Pixel);
            this.BoundsInfo.SetPinOffset(szPinOffsetUnitIndependent, MeasureUnits.Pixel);
            this.BoundsInfo.SetPinPoint(ptPinPointUnitIndependent, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Updates the pin point.
        /// </summary>
        /// <param name="oldPinPoint">The old pin point.</param>
        /// <param name="newPinPoint">The new pin point.</param>
        private void UpdatePinPoint(PointF oldPinPoint, ref PointF newPinPoint)
        {
            Matrix mtxPin = new Matrix();

            float fAngle = Geometry.ConvertToFullCircle(this.RotationAngle);

            if (this.FlipX)
                fAngle = -fAngle;

            if (this.FlipY)
                fAngle = -fAngle;

            mtxPin.RotateAt(fAngle, oldPinPoint, MatrixOrder.Append);

            PointF[] ptsPin = new PointF[] { newPinPoint };
            mtxPin.TransformPoints(ptsPin);

            newPinPoint = ptsPin[0];
        }

        /// <summary>
        /// Gets the new bounds.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        /// <returns>The node bounds.</returns>
        protected RectangleF GetNewBounds(PointF[] pts)
        {
            PointF[] ptsClone = (PointF[])pts.Clone();

            PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            SizeF szPinOffset = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);

            PointF ptUpperLeft = new PointF(
                ptPinPoint.X - szPinOffset.Width,
                ptPinPoint.Y - szPinOffset.Height);

            // Create matrix transformations - Without rotate.
            Matrix mtx = new Matrix();
            mtx.Translate(ptUpperLeft.X, ptUpperLeft.Y);
            AppendFlipTransforms(mtx);

            // 3 - Transform control points to real coordinates.
            mtx.TransformPoints(ptsClone);

            return GetNewPathBounds(ptsClone);
        }

        /// <summary>
        /// Gets the new path points bounds.
        /// </summary>
        /// <param name="pathPoints">The path points.</param>
        /// <returns>
        /// The <see cref="System.Drawing.RectangleF"/>.
        /// </returns>
        protected virtual RectangleF GetNewPathBounds(PointF[] pathPoints)
        {
            return Geometry.CreateRect(pathPoints);
        }

        /// <summary>
        /// Updates the path location.
        /// </summary>
        /// <param name="pathCombined">The combined path .</param>
        /// <param name="rect">The rectangle to combine.</param>
        protected void UpdatePathLocation(ref GraphicsPath pathCombined, RectangleF rect)
        {
            // update path points if path's bounding recteangle is located not at 0,0
            SizeF szOffset = SizeF.Empty;

            if (rect.X < 0)
            {
                szOffset.Width = -rect.X;
            }
            else
            {
                szOffset.Width = rect.X;
            }

            if (rect.Y < 0)
            {
                szOffset.Height = -rect.Y;
            }
            else
            {
                szOffset.Height = rect.Y;
            }

            if (!szOffset.IsEmpty)
            {
                PointF[] pts = (PointF[])pathCombined.PathPoints.Clone();

                for (int nCounter = 0, nLength = pts.Length; nCounter < nLength; nCounter++)
                {
                    pts[nCounter].X += szOffset.Width;
                    pts[nCounter].Y += szOffset.Height;
                }

                pathCombined = new GraphicsPath(pts, pathCombined.PathTypes);
            }
        }

        /// <summary>
        /// Checks whether VertexEditMode can be applied to current PathNode.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if specified path is vertex editable; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsPathVertexEditable(GraphicsPath path)
        {
            bool bSuccess = true;

            byte[] pathTypes = path.PathTypes;

            foreach (byte type in pathTypes)
            {
                // 3 - Bezier curve
                if (type == c_nBEZIER_SEGMENT)
                {
                    bSuccess = false;
                    break;
                }
            }

            return bSuccess;
        }
        private void RecordSetGraphicsPath(GraphicsPath pathCurrent, GraphicsPath pathToCombineWith, bool bConnect)
        {
            if (this.HistoryManager != null)
            {
                this.HistoryManager.RecordSetGraphicsPath(this, pathCurrent, pathToCombineWith, bConnect);
            }
        }
        private void RecordSetPoints()
        {
            if (this.HistoryManager != null)
            {
                this.HistoryManager.RecordMoveHandle(this);
            }
        }

        /// <summary>
        /// Records the vertex changed.
        /// </summary>
        /// <param name="changeType">Type of collection change.</param>
        /// <param name="nVertexIdx">The vertex index.</param>
        /// <param name="ptVertexNewLocation">The vertex new location.</param>
        protected void RecordVertexChanged(VertexChangeType changeType, int nVertexIdx, PointF ptVertexNewLocation)
        {
            if (this.HistoryManager != null)
            {
                this.HistoryManager.RecordVertexChanged(this, changeType, nVertexIdx, ptVertexNewLocation);
            }
        }

        /// <summary>
        /// Called when vertex changing.
        /// </summary>
        /// <param name="changeType">Type of vertex the change.</param>
        /// <param name="nVertexIdx">The vertex id.</param>
        /// <param name="ptLocation">The  point location.</param>
        /// <returns>true, if vertex is changing.</returns>
        public virtual bool OnVertexChanging(VertexChangeType changeType, int nVertexIdx, PointF ptLocation)
        {
            bool bSuccess = true;

            if (this.EventSink != null)
            {
                VertexChangingEventArgs evtArgs =
                    new VertexChangingEventArgs(this, changeType, nVertexIdx, ptLocation);

                bSuccess = this.EventSink.RaiseVertexChanging(evtArgs);

                // pause event sink till location changed
                //if (bSuccess)
                //{
                //    this.EventSink.Pause();
                //}
            }

            return bSuccess;
        }

        /// <summary>
        /// Called when vertex change.
        /// </summary>
        /// <param name="changeType">Type of collection change.</param>
        /// <param name="nVertexIdx">The vertex index.</param>
        /// <param name="ptLocation">The given point.</param>
        public virtual void OnVertexChanged(VertexChangeType changeType, int nVertexIdx, PointF ptLocation)
        {
            if (this.EventSink != null)
            {
                // resume previous paused event sink
                //this.EventSink.Resume();

                VertexChangedEventArgs evtArgs =
                    new VertexChangedEventArgs(this, changeType, nVertexIdx, ptLocation);

                this.EventSink.RaiseVertexChanged(evtArgs);
            }
        }

        /// <summary>
        /// Determines whether this points can locate in current model.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <returns>
        /// <c>true</c> if this points can locate in model; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanChangePoints(PointF[] pts)
        {
            bool bSuccess = true;
            Model model = this.Root;

            if (model != null && model.BoundaryConstraintsEnabled)
            {
                // 1 - Calculate rectangle from points
                PointF[] ptsClone = (PointF[])pts.Clone();
                RectangleF rcBounds;

                // 2 - Calculate current bounds
                Matrix mtxParent = HandlesHitTesting.GetParentsTransformations(this, true);

                mtxParent.TransformPoints(ptsClone);
                rcBounds = Geometry.CreateRect(ptsClone);

                RectangleF rectModelBounds = MeasureUnitsConverter.ToPixels(model.Bounds, model.MeasurementUnits);

                rcBounds.X = (float)Math.Round(rcBounds.X, 2);
                rcBounds.Y = (float)Math.Round(rcBounds.Y, 2);

                if (!rectModelBounds.Contains(rcBounds))
                {
                    bSuccess = false;
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Creates the segments regions for hit testing.
        /// </summary>
        /// <returns>
        /// The array of <see cref="System.Drawing.Region"/> items.
        /// </returns>
        private Region[] CreateSegmentsRegions()
        {
            PointF[] pathPoints = (PointF[])this.GetPathPoints().Clone();
            int segmentsCount = pathPoints.Length;

            // if GraphicsPath is not closed segmentsCount = this.PathPoints.Count - 1
            byte[] btPathTypes = this.GraphicsPath.PathTypes;

            if ((btPathTypes[btPathTypes.Length - 1] & 128) != 128)
            {
                segmentsCount -= 1;
            }

            // get node transformations
            Matrix mtxTemp = GetTransformations();
            AppendFlipTransforms(mtxTemp);

            mtxTemp.TransformPoints(pathPoints);

            // Create regions cache --> number of regions in array is equal to line segments number
            ArrayList regionsToReturns = new ArrayList();
            GraphicsPath gphSegment;
            Region rgnAdding;
            
            // iterate through each segment and create corresponding region
            for (int i = 0; i < segmentsCount; i++)
            {
                pathPoints = GetLineSegmentPoints(i);
                PointF ptStart = pathPoints[0];
                PointF ptEnd = pathPoints[1];

                gphSegment = new GraphicsPath();

                if (ptStart == ptEnd)
                {
                    ptStart.X -= 0.5f;
                    ptEnd.X += 0.5f;
                }

                gphSegment.AddLine(ptStart, ptEnd);
                float fHitTestPadding;
                if (HandlesHitTesting.TouchMode)
                    fHitTestPadding = MeasureUnitsConverter.ToPixelX(CommonUsedValues.TOUCH__HIT_TEST_PADDING, this.MeasurementUnit);
                else
                    fHitTestPadding = MeasureUnitsConverter.ToPixelX(this.LineHitTestPadding, this.MeasurementUnit); ;

                using (Pen pen = new Pen(Color.Black, fHitTestPadding))
                {
                    gphSegment.Widen(pen);
                }

                rgnAdding = new Region(gphSegment);
                regionsToReturns.Add(rgnAdding);
            }

            return (Region[])regionsToReturns.ToArray(typeof(Region));
        }
        #endregion
    }
}
