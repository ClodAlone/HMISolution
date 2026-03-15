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
    /// Line node that contains endpoints and control points. Can be used as connector.
    /// </summary>
    [Serializable]
    public class Line
        : LineBase,
          IEndPointContainer,
          IGraphEdge
    {
        #region Class members
        /// <summary>
        /// Head end point.
        /// </summary>
        protected HeadEndPoint m_endPointHead;

        /// <summary>
        /// Tail end point.
        /// </summary>
        protected TailEndPoint m_endPointTail;

        /// <summary>
        /// Collection of segments with bridges.
        /// </summary>
        protected ArrayList m_lineSegments;

        /// <summary>
        /// Defines connector state.
        /// </summary>
        private ConnectorState m_cntState;

        /// <summary>
        /// Flag what use for lock handle moving while EditStyle.AllowRotate = false;
        /// </summary>
        private bool m_bLockHandleMove = true;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        public Line()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public Line(PointF[] pts, MeasureUnits measureUnits)
            : this(pts[0], pts[1], measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        public Line(PointF[] pts)
            : this(pts[0], pts[1])
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        public Line(PointF ptStart, PointF ptEnd)
            : base()
        {
            // set endpoints
            m_endPointTail = new TailEndPoint(this, ptStart);
            m_endPointTail.UpdateServiceReferences(this);

            m_endPointHead = new HeadEndPoint(this, ptEnd);
            m_endPointHead.UpdateServiceReferences(this);

            // update line bounds
            InitializeLine();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public Line(PointF ptStart, PointF ptEnd, MeasureUnits measureUnits)
            : base()
        {
            ptStart = MeasureUnitsConverter.ToPixels(ptStart, measureUnits);
            ptEnd = MeasureUnitsConverter.ToPixels(ptEnd, measureUnits);

            // set endpoints
            m_endPointTail = new TailEndPoint(this, ptStart);
            m_endPointTail.UpdateServiceReferences(this);

            m_endPointHead = new HeadEndPoint(this, ptEnd);
            m_endPointHead.UpdateServiceReferences(this);

            // update line bounds
            InitializeLine();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// <param name="src">The Line instance.</param>
        public Line(Line src)
            : base(src)
        {
            m_endPointHead = (HeadEndPoint)src.m_endPointHead.Clone();
            m_endPointHead.Container = this;

            m_endPointTail = (TailEndPoint)src.m_endPointTail.Clone();
            m_endPointTail.Container = this;

            UpdateSegments();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// <param name="info">The serialization Info.</param>
        /// <param name="context">The streaming context.</param>
        protected Line(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_endPointHead = (HeadEndPoint)info.GetValue("headEndPoint", typeof(HeadEndPoint));

            if (m_endPointHead != null)
            {
                m_endPointHead.Container = this;
                m_endPointHead.UpdateServiceReferences(this);
            }

            m_endPointTail = (TailEndPoint)info.GetValue("tailEndPoint", typeof(TailEndPoint));

            if (m_endPointTail != null)
            {
                m_endPointTail.Container = this;
                m_endPointTail.UpdateServiceReferences(this);
            }
        }
        #endregion

        #region Class properties
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
        /// Gets the line segments list.
        /// </summary>
        /// <value>The line segments.</value>
        protected ArrayList LineSegments
        {
            get
            {
                if (m_lineSegments == null)
                    m_lineSegments = new ArrayList();

                return m_lineSegments;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [flip X].
        /// </summary>
        /// <value><c>true</c> if flip X; otherwise, <c>false</c>.</value>
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
        /// Gets or sets a value indicating whether [flip Y].
        /// </summary>
        /// <value><c>true</c> if flip Y; otherwise, <c>false</c>.</value>
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
        /// <value>The rotation angle.</value>
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
        #endregion

        #region Class transformations overrides
        /// <summary>
        /// Called when node is moved.
        /// </summary>
        /// <param name="fX">The x offset.</param>
        /// <param name="fY">The y offset.</param>
        protected override void DoMoveRelatedActions(float fX, float fY)
        {
            // Append flips to move offset
            SizeF szOffset = SizeF.Empty;
            szOffset.Width = this.FlipX ? -fX : fX;
            szOffset.Height = this.FlipY ? -fY : fY;

            // pause history recording
            SafeHistoryPause();

            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            // move head endPoint
            SynchronizeEndPoint(this.HeadEndPoint, szOffset);

            // move tail endPoint
            SynchronizeEndPoint(this.TailEndPoint, szOffset);

            // update path 
            UpdatePathNodeData();

            // restore history recording
            SafeHistoryResume();

            // call base to update bounding rectangle
            bool bLock = m_bPortUpdating;
            m_bPortUpdating = true;
            base.DoMoveRelatedActions(fX, fY);
            m_bPortUpdating = bLock;

            m_bLockHandleMove = bLockMove;
        }

        /// <summary>
        /// Called after change the flipX value.
        /// </summary>
        /// <param name="value">New flip x value.</param>
        protected override void ChangeFlipX(bool value)
        {
            if (!m_bLockUpdate && !IsConnected())
            {
                m_bLockUpdate = true;
                HistoryManager mngHistory = this.HistoryManager;

                if (mngHistory != null)
                    mngHistory.Pause();

                // flip endpoint position
                FlipEndPoints(true, false);

                if (mngHistory != null)
                    mngHistory.Resume();

                // call base for update connections
                base.ChangeFlipX(value);

                UpdateBoundingRectangle();

                m_bLockUpdate = false;
            }
        }

        /// <summary>
        /// Called after change the flipY value.
        /// </summary>
        /// <param name="value">New flip y value.</param>
        protected override void ChangeFlipY(bool value)
        {
            if (!m_bLockUpdate && !IsConnected())
            {
                m_bLockUpdate = true;
                HistoryManager mngHistory = this.HistoryManager;

                if (mngHistory != null)
                    mngHistory.Pause();

                // flip endpoint position
                FlipEndPoints(false, true);

                if (mngHistory != null)
                    mngHistory.Resume();

                // call base for update connections
                base.ChangeFlipY(value);

                UpdateBoundingRectangle();

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

                // get pin position
                PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
                PointF[] pts = new PointF[] { this.HeadEndPoint.Location, this.TailEndPoint.Location };

                // Append flip.
                if (this.FlipX)
                    fRotationChange = -fRotationChange;

                if (this.FlipY)
                    fRotationChange = -fRotationChange;

                // Create flip martrix
                Matrix mtxTemp = new Matrix();
                mtxTemp.RotateAt(fRotationChange, ptPinPoint, MatrixOrder.Append);

                // transform points.
                mtxTemp.TransformPoints(pts);

                // append transforms points as head and tail endPoints
                this.HeadEndPoint.Location = pts[0];
                this.TailEndPoint.Location = pts[1];

                if (mngHistory != null)
                    mngHistory.Resume();

                // call base for update connections
                base.ChangeRotationBy(fRotationChange);

                m_bLockUpdate = false;
            }
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

            // calc scale factors
            float fScaleFactorX = (szOldSize.Width == 0) ? 1 : szNewSize.Width / szOldSize.Width;
            float fScaleFactorY = (szOldSize.Height == 0) ? 1 : szNewSize.Height / szOldSize.Height;

            Matrix mtxTransform = new Matrix(fScaleFactorX, 0, 0, fScaleFactorY, 0, 0);

            // get clone  of data points after scale graphics path
            PointF[] pts = GetPoints();

            // append transformation to get real locations data points
            mtxTransform.Multiply(GetTransformations(), MatrixOrder.Append);
            AppendFlipTransforms(mtxTransform);
            mtxTransform.TransformPoints(pts);

            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            // append first and last data points as head and tail endPoints
            this.TailEndPoint.Location = pts[0];
            this.HeadEndPoint.Location = pts[pts.Length - 1];

            base.DoSizeRelatedActions(szOldSize, szNewSize);

            m_bLockHandleMove = bLockMove;

            if (mngHistory != null)
                mngHistory.Resume();
        }

        /// <summary>
        /// Performs additional changes on pin offset value changed.
        /// </summary>
        /// <param name="szOldPinOffset">The old pin offset value.</param>
        /// <param name="szNewPinOffset">The new pin offset value.</param>
        protected override void DoPinOffsetRelatedActions(SizeF szOldPinOffset, SizeF szNewPinOffset)
        {
            // Append flips to move offset
            SizeF szOffset = new SizeF(
                szOldPinOffset.Width - szNewPinOffset.Width,
                szOldPinOffset.Height - szNewPinOffset.Height);

            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            // move head endPoint
            this.HeadEndPoint.Move(szOffset, MeasureUnits.Pixel);

            // move tail endPoint
            this.TailEndPoint.Move(szOffset, MeasureUnits.Pixel);

            // call  base for update connections and cache region
            base.DoPinOffsetRelatedActions(szOldPinOffset, szNewPinOffset);

            m_bLockHandleMove = bLockMove;
        }

        /// <summary>
        /// Perfoms additional changes on handle move.
        /// </summary>
        /// <param name="handleMoved">The moved handle .</param>
        /// <param name="szOffset">The move offset.</param>
        protected override void DoHandleMoveRelated(IHandle handleMoved, SizeF szOffset)
        {
            // if there are no control points sync opposite end point with its port
            MergeControlPoints();

            // pause history for a while
            SafeHistoryPause();

            // synchronize head and tail end points
            if (m_mgrLink != null)
            {
                m_mgrLink.SynchronizeEndPoint(this.HeadEndPoint);
                m_mgrLink.SynchronizeEndPoint(this.TailEndPoint);
            }

            // resume recording to history
            SafeHistoryResume();

            // call UpdatePathNodeData() method
            base.DoHandleMoveRelated(handleMoved, szOffset);
        }
        #endregion

        #region Class overrides
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

            // synchronize head and tail end points
            if (m_mgrLink != null)
            {
                m_mgrLink.SynchronizeEndPoint(this.HeadEndPoint);
                m_mgrLink.SynchronizeEndPoint(this.TailEndPoint);
            }

            if (model != null)
            {
                model.HistoryManager.Resume();
                model.EventSink.Resume();
                model.EndUpdate();
            }

            base.OnNodeScaleChanged(strPropertyName);
        }

        /// <summary>
        /// Sets the points internal.
        /// </summary>
        /// <param name="ptsPath">The new path points.</param>
        protected override void SetPointsInternal(PointF[] ptsPath)
        {
            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            // repopulate control points
            // -------------------------
            // remove control points updating service references
            foreach (ControlPoint ctrlPt in this.ControlPoints)
            {
                ctrlPt.UpdateServiceReferences(null);
            }

            this.ControlPoints.Clear();

            // create new control points
            ControlPoint ctrlPtTemp;

            for (int n = 1, nLength = ptsPath.Length - 1; nLength > n; n++)
            {
                ctrlPtTemp = new ControlPoint(this, ptsPath[n], n - 1);
                ctrlPtTemp.UpdateServiceReferences(this);

                this.ControlPoints.Add(ctrlPtTemp);
            }

            // update end points
            SetEndPoints(ptsPath);

            base.SetPointsInternal(ptsPath);

            // synchronize end points
            if (m_mgrLink != null)
            {
                m_mgrLink.SynchronizeEndPoint(this.HeadEndPoint);
                m_mgrLink.SynchronizeEndPoint(this.TailEndPoint);
            }

            m_bLockHandleMove = bLockMove;
        }

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
        /// Gets the line segment by its index.
        /// </summary>
        /// <param name="nSegmentIndex">Index of the segment.</param>
        /// <returns>The line segment</returns>
        public override LineSegment GetLineSegmentAt(int nSegmentIndex)
        {
            if (nSegmentIndex < 0 || nSegmentIndex >= this.LineSegments.Count)
                throw new ArgumentOutOfRangeException("nSegmentIndex");

            return this.LineSegments[nSegmentIndex] as LineSegment;
        }

        /// <summary>
        /// Retrieves array of points needed to construct node's GraphicsPath.
        /// </summary>
        /// <returns>The path points.</returns>
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
            Geometry.TranslateToGridOrigin(pts);
            Geometry.TranslateToGridOrigin(ptsPath);

            // update control points list
            UpdateControlPoints(pts, 1);

            m_gpPath = CreateLogicalGraphicsPath(ptsPath);

            // update segments after update graphics path and handles
            UpdateSegments(pts);
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
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object</returns>
        public override object Clone()
        {
            return new Line(this);
        }

        /// <summary>
        /// Updates the references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public override void UpdateReferences(IServiceReferenceProvider provider)
        {
            base.UpdateReferences(provider);

            if (m_endPointHead != null)
            {
                m_endPointHead.UpdateServiceReferences(provider);
            }

            if (m_endPointTail != null)
            {
                m_endPointTail.UpdateServiceReferences(provider);
            }
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization Info.</param>
        /// <param name="context">The streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("headEndPoint", m_endPointHead);
            info.AddValue("tailEndPoint", m_endPointTail);
        }

        /// <summary>
        /// Gets the property container.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the STR property container.</param>
        /// <returns>Property container value.</returns>
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
        /// Updates the Pin position, pin offset and size from new bounds rectangle.
        /// </summary>
        /// <param name="rcBounds">The updated bounds.</param>
        protected override void UpdateBoundsInfo(RectangleF rcBounds)
        {
            if (this.ControlPoints.Count == 0)
            {
                // ge pin offset and size to calc pin offset fator
                PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
                SizeF szPinOffset = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);
                SizeF szSize = this.BoundsInfo.GetSize(MeasureUnits.Pixel);

                // append flip to end points
                PointF[] ptsLine = new PointF[] { this.TailEndPoint.Location, this.HeadEndPoint.Location };
                Matrix mtxFlip = new Matrix();
                this.AppendFlipTransforms(mtxFlip);
                mtxFlip.TransformPoints(ptsLine);

                // calc line length
                float fLineLength = (float)Geometry.PointDistance(ptsLine[0], ptsLine[1]);
                float fLengthFactor = (szSize.Width != 0) ? szPinOffset.Width / szSize.Width : 1;

                // calc pin offset
                SizeF szPinOffsetUnitIndependent = new SizeF(fLineLength * fLengthFactor, 0);

                // calc node size value
                SizeF szSizeUnitIndependent = new SizeF(fLineLength, 0);

                // update rotation angle
                float angle = UpdateRotationAngle(ptsLine[0], ptsLine[1]);

                // calc pin point by rotate pinOffset arround start point( tail endPoint )
                PointF[] ptPinPointUnitIndependent = new PointF[] { szPinOffsetUnitIndependent.ToPointF() };
                Matrix mtxRotate = new Matrix(1, 0, 0, 1, ptsLine[0].X, ptsLine[0].Y);
                mtxRotate.Rotate(this.RotationAngle);
                AppendFlipTransforms(mtxRotate, ptPinPoint, this.FlipX, this.FlipY);
                mtxRotate.TransformPoints(ptPinPointUnitIndependent);

                // Init BoundsInfo
                this.BoundsInfo.SetSize(szSizeUnitIndependent, MeasureUnits.Pixel);
                if (angle != this.RotationAngle)
                {
                    UpdateRotationAngle(ptsLine[0], ptsLine[1]);
                    ptPinPointUnitIndependent = new PointF[] { szPinOffsetUnitIndependent.ToPointF() };
                    mtxRotate = new Matrix(1, 0, 0, 1, ptsLine[0].X, ptsLine[0].Y);
                    mtxRotate.Rotate(this.RotationAngle);
                    AppendFlipTransforms(mtxRotate, ptPinPoint, this.FlipX, this.FlipY);
                    mtxRotate.TransformPoints(ptPinPointUnitIndependent);
                }
                this.BoundsInfo.SetPinOffset(szPinOffsetUnitIndependent, MeasureUnits.Pixel);
                this.BoundsInfo.SetPinPoint(ptPinPointUnitIndependent[0], MeasureUnits.Pixel);
            }
            else
            {
                base.UpdateBoundsInfo(rcBounds);
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Merges the control points.
        /// </summary>
        protected void MergeControlPoints()
        {
            if ((this.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints)
            {
                PointF[] ptsPath = GetPathPoints();

                for (int i = 0, nLength = ptsPath.Length - 2; i < nLength; i++)
                {
                    // if three contiguous points lie on the same line ( slop = 5 pixels )
                    // merge this points' segments and update path points
                    if (Geometry.HitTestLine(ptsPath[i], ptsPath[i + 2], ptsPath[i + 1], 5))
                    {
                        // update ControlPoints
                        this.ControlPoints.RemoveAt(i);

                        // call recursively itself
                        MergeControlPoints();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Merges the control points.
        /// </summary>
        /// <param name="ptsNew">The points.</param>
        protected void MergeControlPoints(ref PointF[] ptsNew)
        {
            if ((this.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints)
            {
                ArrayList lstPts = new ArrayList(ptsNew);

                for (int i = 0, nLength = lstPts.Count - 2; i < nLength; i++)
                {
                    // if three contiguous points lie on the same line ( slop = 5 pixels )
                    // merge this points' segments and update path points
                    if (Geometry.HitTestLine((PointF)lstPts[i], (PointF)lstPts[i + 2], (PointF)lstPts[i + 1], 5))
                    {
                        // update path points
                        lstPts.RemoveAt(i + 1);

                        // convert to array
                        ptsNew = (PointF[])lstPts.ToArray(typeof(PointF));

                        // call recursively itself
                        MergeControlPoints(ref ptsNew);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates EndPoints locations.
        /// </summary>
        /// <param name="ptsPath">New path points.</param>
        protected void SetEndPoints(PointF[] ptsPath)
        {
            // pause raising events
            if (this.EventSink != null)
                this.EventSink.Pause();

            Matrix mtxTemp = GetTransformations();
            AppendFlipTransforms(mtxTemp);
            mtxTemp.Multiply(GetScaleTransformation());

            PointF[] ptsEP = new PointF[] { ptsPath[0], ptsPath[ptsPath.Length - 1] };
            mtxTemp.TransformPoints(ptsEP);

            // update tail end point
            this.TailEndPoint.Location = ptsEP[0];

            // update head end point
            this.HeadEndPoint.Location = ptsEP[1];

            // resume raising events
            if (this.EventSink != null)
                this.EventSink.Resume();
        }

        /// <summary>
        /// Updates connector's segments from PathPoints.
        /// </summary>
        protected void UpdateSegments()
        {
            UpdateSegments(this.GetPathPoints());
        }

        /// <summary>
        /// Updates connector's segments.
        /// </summary>
        /// <param name="ptsPoints">The array of points.</param>
        protected virtual void UpdateSegments(PointF[] ptsPoints)
        {
            // clear line segments
            this.LineSegments.Clear();

            // repopulate line segments
            LineSegment segmentTemp;

            for (int i = 0, nLength = ptsPoints.Length; nLength - 1 > i; i++)
            {
                segmentTemp = new LineSegment(ptsPoints[i], ptsPoints[i + 1]);

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
        /// Flips the end points.
        /// </summary>
        /// <param name="bFlipX">if set to <c>true</c> flip by X.</param>
        /// <param name="bFlipY">if set to <c>true</c> flip by Y.</param>
        private void FlipEndPoints(bool bFlipX, bool bFlipY)
        {
            Matrix mtxFlip = new Matrix();
            PointF ptPinPointUnitIndependent = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            PointF[] pts = new PointF[] { this.HeadEndPoint.Location, this.TailEndPoint.Location };

            // Reset location to origin
            mtxFlip.Translate(-ptPinPointUnitIndependent.X, -ptPinPointUnitIndependent.Y, MatrixOrder.Append);

            // allow to move handle while node not allow to rotate
            bool bLockMove = m_bLockHandleMove;
            m_bLockHandleMove = false;

            // Flip by vertical and horizontal
            if (bFlipX)
                mtxFlip.Scale(-1.0f, 1.0f, MatrixOrder.Append);

            if (bFlipY)
                mtxFlip.Scale(1.0f, -1.0f, MatrixOrder.Append);

            // Restore last location
            mtxFlip.Translate(ptPinPointUnitIndependent.X, ptPinPointUnitIndependent.Y, MatrixOrder.Append);
            mtxFlip.TransformPoints(pts);

            this.HeadEndPoint.Location = pts[0];
            this.TailEndPoint.Location = pts[1];

            m_bLockHandleMove = bLockMove;
        }

        /// <summary>
        /// Updates the rotation angle.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        private float UpdateRotationAngle(PointF ptStart, PointF ptEnd)
        {
            // calc rotation angle
            // don't flip rotation angle value
            float fRotationAngle = (float)Math.Atan2(ptEnd.Y - ptStart.Y, ptEnd.X - ptStart.X);
            fRotationAngle = (float)(fRotationAngle * (180f / Math.PI));

            // Assign new angle.
            if (!IsConnected())
                this.RotationAngle = fRotationAngle;
            this.m_fRotationAngle = fRotationAngle;

            return fRotationAngle;
        }
        #endregion

        #region Class initialize shape
        /// <summary>
        /// Initializes the line.
        /// </summary>
        private void InitializeLine()
        {
            PointF ptStart = this.TailEndPoint.Location;
            PointF ptEnd = this.HeadEndPoint.Location;

            // calc line length
            float fLineLength = (float)Geometry.PointDistance(ptStart, ptEnd);

            // assign new GraphicsPath
            m_gpPath = CreateGraphicsPath(new PointF[] { ptStart, ptEnd });
            GraphicsPath gpPath = this.GraphicsPath;

            this.PathPoints = (PointF[])gpPath.PathPoints.Clone();

            // get bounding rect
            RectangleF rectBounding = Geometry.CreateRect(ptStart, ptEnd);

            // calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(fLineLength / 2, 0);

            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounding.X + rectBounding.Width / 2,
                rectBounding.Y + rectBounding.Height / 2);

            // assign node size value
            SizeF szSizeUnitIndependent = new SizeF(fLineLength, 0);

            // Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            // calc rotation angle
            float fRotationAngle = (float)Math.Atan2(ptEnd.Y - ptStart.Y, ptEnd.X - ptStart.X);

            // Lock update rotating angle.
            m_bLockUpdate = true;
            this.RotationAngle = (float)(fRotationAngle * (180f / Math.PI));
            m_bLockUpdate = false;

            // update connector segments
            UpdateSegments();

            // enable control points merging
            m_cntState |= ConnectorState.MergeControlPoints;

            m_bIsVertexEditable = false;
            UpdateBoundingRectangle();
        }
        #endregion

        #region IEndPointContainer Members
        /// <summary>
        /// Gets the head end point handle.
        /// </summary>
        /// <value>The head end point.</value>
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public EndPoint HeadEndPoint
        {
            get { return m_endPointHead; }
        }

        /// <summary>
        /// Gets the tail end point handle.
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
            // TODO: Add Line.IsNodeEntering implementation
            return false;
        }
        #endregion
    }
}
