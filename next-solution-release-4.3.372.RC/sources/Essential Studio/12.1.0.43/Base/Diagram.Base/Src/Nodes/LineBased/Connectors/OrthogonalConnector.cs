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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Line connector that contains horizontal and vertical line segments.
    /// </summary>
    [Serializable]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class OrthogonalConnector
        : ConnectorBase
    {
        #region Class members
        /// <summary>
        /// Saved path poitns before route end point to heading in model coordinates.
        /// </summary>
        private PointF[] m_ptsPathClone;

        /// <summary>
        /// Indicate what heading is changes.
        /// </summary>
        private HeadingState m_eHeadingState;
        private bool m_bMoveConnectedSegments;
        private IHandle m_handleMoved;
        private float m_fCurveRadius = 8;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="measureUnits">The measure units.</param>
        public OrthogonalConnector(PointF ptStart, PointF ptEnd, MeasureUnits measureUnits)
            : base(ptStart, ptEnd, measureUnits)
        {
            ptStart = MeasureUnitsConverter.ToPixels(ptStart, measureUnits);
            ptEnd = MeasureUnitsConverter.ToPixels(ptEnd, measureUnits);

            Init(ptStart, ptEnd, false);
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="measureUnits">The measure units.</param>
        /// <param name="flip">true if path should be flipped; otherwise, false.</param>
        public OrthogonalConnector(PointF ptStart, PointF ptEnd, MeasureUnits measureUnits, bool flip)
            : base(ptStart, ptEnd, measureUnits)
        {
            ptStart = MeasureUnitsConverter.ToPixels(ptStart, measureUnits);
            ptEnd = MeasureUnitsConverter.ToPixels(ptEnd, measureUnits);

            Init(ptStart, ptEnd, flip);
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The tail end point location.</param>
        /// <param name="ptEnd">The head end point location.</param>
        public OrthogonalConnector(PointF ptStart, PointF ptEnd)
            : base(ptStart, ptEnd)
        {
            Init(ptStart, ptEnd, false);
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The tail end point location.</param>
        /// <param name="ptEnd">The head end point location.</param>
        /// <param name="flip">true if path should be flipped; otherwise, false.</param>
        public OrthogonalConnector(PointF ptStart, PointF ptEnd, bool flip)
            : base(ptStart, ptEnd)
        {
            Init(ptStart, ptEnd, flip);
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The tail end point location.</param>
        /// <param name="ptEnd">The head end point location.</param>
        /// <param name="flip">true if path should be flipped; otherwise, false.</param>
        /// <param name="roundCorner">true if corner should be rounded; otherwise, false.</param>
        public OrthogonalConnector(PointF ptStart, PointF ptEnd, bool flip, bool roundCorner)
            : base(ptStart, ptEnd)
        {
            if (roundCorner)
                this.EnableRoundedCorner = true;
            else
                this.EnableRoundedCorner = false;

            Init(ptStart, ptEnd, flip);
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalConnector"/> class.
        /// </summary>
        /// <param name="src">The connector source.</param>
        public OrthogonalConnector(OrthogonalConnector src)
            : base(src)
        {
            m_bMoveConnectedSegments = src.m_bMoveConnectedSegments;
            m_fCurveRadius = src.m_fCurveRadius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalConnector"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OrthogonalConnector(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "moveconnectedsegments":
                        m_bMoveConnectedSegments = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "CurveRadius":
                        m_fCurveRadius = float.Parse(entry.Value.ToString());
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// orthogonal connector is not rotatable.
        /// </summary>
        public override float RotationAngle
        {
            get { return base.RotationAngle; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether connected segments are movable.
        /// </summary>       
        [Browsable(true)]
        [DefaultValue(false)]        
        [Description("Indicates whether connected segments are movable.")]
        public bool MoveConnectedSegments
        {
            get
            {
                return m_bMoveConnectedSegments;
            }
            set
            {
                if (m_bMoveConnectedSegments != value)
                    m_bMoveConnectedSegments = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the curve.
        /// </summary>       
        [Browsable(true)]
        [DefaultValue(8f)]
        [Description("Radius of the curve")]
        public float CurveRadius
        {
            get { return m_fCurveRadius; }
            set
            {
                if (m_fCurveRadius != value && OnPropertyChanging(this.FullContainerName, DPN.CurveRadius, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.CurveRadius);

                    // set new value
                    m_fCurveRadius = value;

                    InitializeOrthogonalLine(this.GetPathPoints());

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.CurveRadius);
                }
            }
        }
        #endregion

        #region Class public overrides
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new OrthogonalConnector(this);
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
            return this.ContainsHandle(handle);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Used to update child nodes sizes.
        /// </summary>
        /// <param name="szOldSize">The old size.</param>
        /// <param name="szNewSize">The new size.</param>
        protected override void DoSizeRelatedActions(SizeF szOldSize, SizeF szNewSize)
        {
            // synchronized control points with moved endPoint
            SyncEndPointSegments();

            base.DoSizeRelatedActions(szOldSize, szNewSize);

            // reset cloned path point
            m_ptsPathClone = null;
        }

        /// <summary>
        /// Performs additional changes on pin position changed.
        /// </summary>
        /// <param name="fX">The pin offset by x axis.</param>
        /// <param name="fY">The pin offset by y axis.</param>
        protected override void DoMoveRelatedActions(float fX, float fY)
        {
            // synchronize handles with moved endPoints
            // SyncEndPointSegments();
            base.DoMoveRelatedActions(fX, fY);

            // reset cloned path point
            m_ptsPathClone = null;
        }

        /// <summary>
        /// Performs pin offset related actions.
        /// </summary>
        /// <param name="szOldPinOffset">Old pin offset</param>
        /// <param name="szNewPinOffset">New pin offset</param>
        protected override void DoPinOffsetRelatedActions(SizeF szOldPinOffset, SizeF szNewPinOffset)
        {
            base.DoPinOffsetRelatedActions(szOldPinOffset, szNewPinOffset);

            // synchronized end points segments
            SyncEndPointSegments();

            // reset cloned path point
            m_ptsPathClone = null;
        }

        /// <summary>
        /// Does the handle move related.
        /// </summary>
        /// <param name="handleMoved">The handle moved.</param>
        /// <param name="szOffset">The size offset.</param>
        protected override void DoHandleMoveRelated(IHandle handleMoved, SizeF szOffset)
        {
            if (handleMoved is EndPoint)
                m_handleMoved = handleMoved;

            // check connector state
            // update connectors' control points if needed
            UpdateConnectorState(handleMoved);
            
            // synchronize segments with handle
            SyncSegments(handleMoved);

            if (!this.BoundsInfo.IsResizing)
                //Updates the loops in the connector
                MergeSegements();

            // call base to MergeControlPoints
            base.DoHandleMoveRelated(handleMoved, szOffset);

            if (!this.BoundsInfo.IsResizing)
                UpdateConnector(handleMoved);

            // update bridges
            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);

            // reset cloned path point
            m_eHeadingState = HeadingState.None;
        }

        /// <summary>
        /// Determines whether TailEndPoints is connected
        /// </summary>
        /// <returns>
        /// true - if TailEndPoints is connected otherwise - false.
        /// </returns>
        protected override bool IsTailConnected()
        {
            return base.IsTailConnected() || ((int)this.ConnectorState >= 0 && (this.ConnectorState & ConnectorState.TailEndPointConnected) == ConnectorState.TailEndPointConnected);
        }

        /// <summary>
        /// Determines whether HeadEndPoints is connected
        /// </summary>
        /// <returns>
        /// true - if HeadEndPoints is connected otherwise - false.
        /// </returns>
        protected override bool IsHeadConnected()
        {
            return base.IsHeadConnected() || ((int)this.ConnectorState >= 0 && (this.ConnectorState & ConnectorState.HeadEndPointConnected) == ConnectorState.HeadEndPointConnected);
        }

        /// <summary>
        /// Updates the Pin position, pin offset and size from new bounds rectangle.
        /// </summary>
        /// <param name="rcBounds">The bounds rectangle.</param>
        protected override void UpdateBoundsInfo(RectangleF rcBounds)
        {
            UpdateNonLineBoundsInfo(rcBounds);
        }

        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            float fcurveRadius = this.CurveRadius;
            GraphicsPath gpPath = new GraphicsPath();
            if (!this.EnableRoundedCorner)
                gpPath = PathFactory.CreateOrthogonalLine(pts);
            else
            {
                PointF previousEndPoint = PointF.Empty;
                for (int i = 1; i < pts.Length; i++)
                {
                    PointF startPoint = pts[i - 1];
                    PointF endPoint = pts[i];
                    float fLineLength = (float)Geometry.PointDistance(startPoint, endPoint);
                    if (fLineLength < CurveRadius * 2)
                        fcurveRadius = fLineLength / 2;
                    using (Pen pen = this.LineStyle.CreatePen())
                    {
                        if (i > 1)
                        {
                            PointF cornerPoint = startPoint;
                            LengthenLine(endPoint, ref startPoint, -fcurveRadius);
                            PointF controlPoint1 = cornerPoint;
                            PointF controlPoint2 = cornerPoint;
                            LengthenLine(previousEndPoint, ref controlPoint1, -fcurveRadius / 2);
                            LengthenLine(startPoint, ref controlPoint2, -fcurveRadius / 2);
                            gpPath.AddBezier(previousEndPoint, controlPoint1, controlPoint2, startPoint);
                        }
                        if (i + 1 < pts.Length) // shorten end point of all but the last line segment.
                            LengthenLine(startPoint, ref endPoint, -fcurveRadius);

                        gpPath.AddLine(startPoint, endPoint);
                        previousEndPoint = endPoint;
                    }
                }
            }
            return gpPath;
        }

        /// <summary>
        /// Determines whether this points can locate in current model.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <returns>
        /// <c>true</c> if this points can locate in model; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanChangePoints(PointF[] pts)
        {
            bool bSuccess = true;

            for (int i = 0, nLength = pts.Length - 1; i < nLength; i++)
            {
                if (!Geometry.CheckPointOnLine(pts[i], pts[i + 1], 0))
                {
                    bSuccess = false;
                    break;
                }
            }

            if (bSuccess)
                bSuccess = base.CanChangePoints(pts);

            return bSuccess;
        }

        /// <summary>
        /// Sets the points internal.
        /// </summary>
        /// <param name="ptsPath">The new path points.</param>
        protected override void SetPointsInternal(PointF[] ptsPath)
        {
            PointF[] ptsPathPoints = (PointF[])ptsPath.Clone();
            SetEndPoints(ptsPath);
            FilterControlPoints(ref ptsPath);

            int[] IDs = GetValidCtrlPtsID(ptsPath.Length);
            int nCtrlPtsCounter = 0;
            ControlPoint ctrlPt;

            // Update control points
            for (int n = 0, nLength = IDs.Length; n < nLength; n++)
            {
                // get ctrl pt position to insert
                ctrlPt = this.ControlPoints[nCtrlPtsCounter] as ControlPoint;

                while (ctrlPt != null)
                {
                    nCtrlPtsCounter++;
                    ctrlPt = this.ControlPoints[nCtrlPtsCounter] as ControlPoint;
                }

                ControlPoint ctrlPtNew = new ControlPoint(this, ptsPath[n], IDs[n]);
                ctrlPtNew.UpdateServiceReferences(this);
                this.ControlPoints[nCtrlPtsCounter] = ctrlPtNew;
            }

            // call base to set PathPoints property
            base.SetPointsInternal(ptsPathPoints);

            // megre same points or points in line
            MergeControlPoints();

            // update segment to new points
            UpdateSegments();

            // synchronize end points segments if they connected
            SyncEndPointSegments();

            // update bounds rectange
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Merges the control points placed in line.
        /// </summary>
        /// <remarks>
        /// Used with handle moving.
        /// </remarks>
        protected override void MergeControlPoints()
        {
            if ((this.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints)
            {
                PointF[] ptsPath = GetPathPoints();

                for (int i = 0, nLength = ptsPath.Length - 2; i < nLength; i++)
                {
                    if (!CanMergeControlPoints(i, nLength, ptsPath))
                        continue;

                    // if three contiguous points lie on the same line
                    // merge this points' segments and update path points
                    if (Geometry.CheckPointOnLine(ptsPath[i], ptsPath[i + 2], 0))
                    {
                        this.ControlPoints.RemoveAt(i);

                        MergeControlPoints();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates node's GraphicsPath on given array of points.
        /// </summary>
        /// <param name="pts">The new path points array.</param>
        protected override void UpdateGraphicsPath(PointF[] pts)
        {
            // Find line bounds size.
            SizeF szLineBounds = new SizeF(pts[pts.Length - 1].X - pts[0].X, pts[pts.Length - 1].Y - pts[0].Y);
            //updates path points
            if (m_handleMoved != null && m_handleMoved.IsMoving)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    if (!m_handleMoved.InUpdate)
                    {
                        if (pts[0].Y != pts[1].Y)
                            pts = new PointF[]
                            {
                                PointF.Empty,
                                new PointF( szLineBounds.Width, 0 ),
                                new PointF( szLineBounds.Width, szLineBounds.Height )
                                
                            };
                        else
                            pts = new PointF[]
                            {
                                PointF.Empty,
                                new PointF( 0, szLineBounds.Height ),
                                new PointF( szLineBounds.Width, szLineBounds.Height )
                            };
                        m_handleMoved.InUpdate = true;
                    }
                }
                else
                {
                    if (m_handleMoved.InUpdate)
                    {
                        if (pts[0].Y != pts[1].Y)
                            pts = new PointF[]
                            {
                                PointF.Empty,
                                new PointF( szLineBounds.Width, 0 ),
                                new PointF( szLineBounds.Width, szLineBounds.Height )
                            };
                        else
                            pts = new PointF[]
                            {
                                PointF.Empty,
                                new PointF( 0, szLineBounds.Height ),
                                new PointF( szLineBounds.Width, szLineBounds.Height )
                            };
                        m_handleMoved.InUpdate = false;
                    }
                }
            }
            base.UpdateGraphicsPath(pts);
        }
        /// <summary>
        /// Merges the control points placed in line.
        /// </summary>
        /// <param name="ptsNew">New points</param>
        /// <remarks>
        /// Used with line segment move.
        /// </remarks>
        protected override void MergeControlPoints(ref PointF[] ptsNew)
        {
            if ((this.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints)
            {
                ArrayList lstPts = new ArrayList(ptsNew);

                for (int i = 0, nLength = lstPts.Count - 2; i < nLength; i++)
                {
                    if (!CanMergeControlPoints(i, nLength, ptsNew))
                        continue;

                    if (Geometry.CheckPointOnLine((PointF)lstPts[i], (PointF)lstPts[i + 2], 0))
                    {
                        // update path points
                        lstPts.RemoveAt(i + 1);

                        // convert to pointF array
                        ptsNew = (PointF[])lstPts.ToArray(typeof(PointF));

                        // call recursively itself
                        MergeControlPoints(ref ptsNew);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Prepares new path points
        /// </summary>
        /// <param name="ptIdx">The point index.</param>
        /// <param name="val">The point.</param>
        /// <returns>The new path points.</returns>
        /// <remarks>
        /// You should override this method if custom login needed
        /// (e.g. - if you are inserting point into orthogonal line -&gt; two points must be inserted!)
        /// </remarks>
        protected override PointF[] PreparePathPointsForInsert(int ptIdx, PointF val)
        {
            // correct inserting point coordinates
            // get inserting points contiguous points
            PointF pt1 = this.PathPoints[ptIdx - 1];
            PointF pt2 = this.PathPoints[ptIdx];

            // update inserting points value depending
            // on inserting segment orientation
            if (pt1.X == pt2.X)
                val.X = pt1.X;

            if (pt1.Y == pt2.Y)
                val.Y = pt1.Y;

            int nLength = this.PathPoints.Length;

            // copy part of array
            PointF[] ptsNew = new PointF[nLength + 2];
            Array.Copy(this.PathPoints, ptsNew, ptIdx);

            // add new point
            ptsNew[ptIdx] = ptsNew[ptIdx + 1] = val;

            // copy remaining part of array
            Array.Copy(this.PathPoints, ptIdx, ptsNew, ptIdx + 2, nLength - ptIdx);

            return ptsNew;
        }

        /// <summary>
        /// Updates connector's segments.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If handles count less that pathPoints count.</exception>
        protected override void UpdateSegments()
        {
            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);

            // get path node points
            PointF[] ptsPoints = this.GetPathPoints();

            // clear line segments
            this.LineSegments.Clear();

            // repopulate line segments
            OrthogonalLineSegment segmentTemp;
            Orientation orientation = GetFirstSegmentOrientation(ptsPoints);

            for (int i = 0, nLength = ptsPoints.Length; nLength - 1 > i; i++)
            {
                // create new segment
                segmentTemp = new OrthogonalLineSegment(GetHandleAt(i), GetHandleAt(i + 1), orientation);

                // add segment
                this.LineSegments.Add(segmentTemp);

                // invert orientation for next segment
                orientation = (orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal;
            }
        }

        /// <summary>
        /// Get the first segment orientation in points array.
        /// </summary>
        /// <param name="ptsPoints">The array of points.</param>
        /// <returns>Orientation of first segment in points array.</returns>
        private Orientation GetFirstSegmentOrientation(PointF[] ptsPoints)
        {
            Orientation orientationResult = Orientation.Vertical;

            if (ptsPoints != null && ptsPoints.Length > 0)
            {
                PointF ptStart;
                PointF ptEnd;
                int nDigits = 2;

                // get first segment orientation
                for (int i = 0, nLength = ptsPoints.Length; nLength - 1 > i; i++)
                {
                    ptStart = new Point((int)Math.Round(ptsPoints[i].X, nDigits), (int)Math.Round(ptsPoints[i].Y, nDigits));
                    ptEnd = new Point((int)Math.Round(ptsPoints[i + 1].X, nDigits), (int)Math.Round(ptsPoints[i + 1].Y, nDigits));

                    // check for vertical orientation
                    if ((ptStart.X == ptEnd.X) && (ptStart.Y != ptEnd.Y))
                    {
                        orientationResult = (i % 2 == 0) ? Orientation.Vertical : Orientation.Horizontal;
                        break;
                    }

                    // check for horizontal orientation
                    if ((ptStart.X != ptEnd.X) && (ptStart.Y == ptEnd.Y))
                    {
                        orientationResult = (i % 2 == 0) ? Orientation.Horizontal : Orientation.Vertical;
                        break;
                    }
                }
            }

            return orientationResult;
        }

        /// <summary>
        /// Validates the offset.
        /// </summary>
        /// <param name="lineSegmentIndex">Index of the line segment.</param>
        /// <param name="szOffset">The move offset.</param>
        protected override void ValidateOffset(int lineSegmentIndex, ref SizeF szOffset)
        {
            szOffset = GetOrientationOffset(szOffset, lineSegmentIndex);
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

            Model model = this.Root;

            if (bConstrain && model != null && model.BoundaryConstraintsEnabled)
            {
                // get path points update end points location considering whether they are connected
                PointF[] pts = GetPathPoints();

                // get move offset
                PointF ptOldPin = ((IUnitIndependent)this).GetPinPoint(MeasureUnits.Pixel);

                SizeF szOffset = SizeF.Empty;
                szOffset.Width = ptPinPoint.X - ptOldPin.X;
                szOffset.Height = ptPinPoint.Y - ptOldPin.Y;
                ArrayList controlPoints = this.ControlPoints;
                bool bControlPointExist = controlPoints.Count > 0;
                bool bChangeControlPoint = false;

                // if end point is connected translate it back
                // ------------------------------------------
                // tail end point
                if (IsTailConnected())
                {
                    pts[0].X -= szOffset.Width;
                    pts[0].Y -= szOffset.Height;

                    if (bControlPointExist)
                    {
                        pts[1].X -= szOffset.Width;
                        pts[1].Y -= szOffset.Height;
                        bChangeControlPoint = true;
                    }
                }

                // head end point
                if (IsHeadConnected())
                {
                    int nLength = pts.Length;
                    pts[nLength - 1].X -= szOffset.Width;
                    pts[nLength - 1].Y -= szOffset.Height;

                    if (bControlPointExist && !(nLength == 3 && bChangeControlPoint))
                    {
                        pts[nLength - 2].X -= szOffset.Width;
                        pts[nLength - 2].Y -= szOffset.Height;
                    }
                }

                Matrix mtxTransforms = HandlesHitTesting.GetParentsTransformations(this, true);
                mtxTransforms.Translate(szOffset.Width, szOffset.Height, MatrixOrder.Append);

                mtxTransforms.TransformPoints(pts);

                // create graphics path
                GraphicsPath path = CreateGraphicsPath(pts);

                // get model bounding rect
                if (model != null && model.BoundaryConstraintsEnabled)
                {
                    RectangleF rcBounds = MeasureUnitsConverter.Convert(
                        model.Bounds, model.MeasurementUnits, MeasureUnits.Pixel);

                    if (!rcBounds.Contains(path.GetBounds()))
                    {
                        bSuccess = false;
                    }
                }
            }
            return bSuccess & bConstrain;
        }

        /// <summary>
        /// Finishes the set points operation.
        /// </summary>
        protected override void FinishSetPoints()
        {
            // synchronize segments
            SyncSegments(this.HeadEndPoint);
            SyncSegments(this.TailEndPoint);

            // update graphics path
            UpdatePathNodeData();

            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);
        }

        /// <summary>
        /// Synchronize head and tail end points with port if need.
        /// </summary>
        protected override void SyncEndPointsWithPort()
        {
            // call base to synchronize 
            // head and tail points
            base.SyncEndPointsWithPort();

            // synchronize segments with end points
            SyncEndPointSegments();
        }

        /// <summary>
        /// Raise when node property changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void EventSink_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            string strPropertyName = evtArgs.PropertyName;

            if (evtArgs.NodeAffected == this && m_mgrLink != null)
            {
                SafeHistoryPause();
                this.Root.BeginUpdate();

                if (strPropertyName == DPN.HeadingHead)
                {
                    m_mgrLink.BeginSynchronization();

                    if (m_eHeadingState != HeadingState.HeadingHead)
                        SaveSynchronizationState();

                    // convert points to model coordinates
                    Matrix mtxPathMatrix = HandlesHitTesting.GetParentsTransformations(this, true);
                    PointF[] pts = (PointF[])m_ptsPathClone.Clone();
                    mtxPathMatrix.Invert();
                    mtxPathMatrix.TransformPoints(pts);
                    SetPointsInternal(pts);

                    m_mgrLink.SynchronizeEndPoint(this.HeadEndPoint);
                    m_mgrLink.EndSynchronization();

                    this.Root.EndUpdate();
                    m_eHeadingState = HeadingState.HeadingHead;
                }
                else if (strPropertyName == DPN.HeadingTail)
                {
                    m_mgrLink.BeginSynchronization();

                    if (m_eHeadingState != HeadingState.HeadingTail)
                        SaveSynchronizationState();

                    // convert points to model coordinates
                    Matrix mtxPathMatrix = HandlesHitTesting.GetParentsTransformations(this, true);
                    PointF[] pts = (PointF[])m_ptsPathClone.Clone();
                    mtxPathMatrix.Invert();
                    mtxPathMatrix.TransformPoints(pts);
                    SetPointsInternal(pts);

                    m_mgrLink.SynchronizeEndPoint(this.TailEndPoint);
                    m_mgrLink.EndSynchronization();

                    this.Root.EndUpdate();
                    m_eHeadingState = HeadingState.HeadingTail;
                }
                else if (strPropertyName == DPN.HeadingDistance)
                {
                    m_mgrLink.SynchronizeNodeConnections(this);
                    m_eHeadingState = HeadingState.None;
                    this.Root.EndUpdate();
                }
                else
                {
                    this.Root.EndUpdate();
                }

                SafeHistoryResume();
            }

            // call base to synchronize end point with port
            base.EventSink_PropertyChanged(evtArgs);
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);                   
            info.AddValue("moveconnectedsegments", m_bMoveConnectedSegments);
            info.AddValue("CurveRadius", m_fCurveRadius);
        }

        #endregion

        #region Class helper methods
        #region initialization
        private void Init(PointF ptStart, PointF ptEnd, bool flip)
        {
            // Find line bounds size.
            SizeF szLineBounds = new SizeF(ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y);

            // Create path points.
            PointF[] pts;

            if (ptEnd.X == ptStart.X)
            {
                if(ptEnd.Y > ptStart.Y)
                    pts = new PointF[] { PointF.Empty, new PointF(0, Math.Abs(szLineBounds.Height)) };
                else
                    pts = new PointF[] {new PointF(0, Math.Abs(szLineBounds.Height)), PointF.Empty };
            }
            else if (ptEnd.Y == ptStart.Y)
            {
                if(ptEnd.X > ptStart.X)
                    pts = new PointF[] { PointF.Empty, new PointF(Math.Abs(szLineBounds.Width), 0) };
                else
                    pts = new PointF[] { new PointF(Math.Abs(szLineBounds.Width), 0), PointF.Empty };
            }
            else if(flip)
            {
                pts = new PointF[]
                {
                    PointF.Empty,
                    new PointF( szLineBounds.Width, 0 ),
                    new PointF( szLineBounds.Width, szLineBounds.Height )
                };
            }
            else
            {
                pts = new PointF[]
                {
                    PointF.Empty,
                    new PointF( 0, szLineBounds.Height ),
                    new PointF( szLineBounds.Width, szLineBounds.Height )
                };
            }

            // Initialize graphics path.
            InitializeOrthogonalLine(pts);
        }

        /// <summary>
        /// Initializes the orthogonal.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        protected void InitializeOrthogonalLine(PointF[] pts)
        {
            // get bounding rect
            RectangleF rectBounding = Geometry.CreateRect(this.HeadEndPoint.Location, this.TailEndPoint.Location);

            RectangleF rcLocalBounds = Geometry.CreateRect(pts);

            for (int i = 0, nLength = pts.Length; i < nLength; i++)
            {
                pts[i].X -= rcLocalBounds.X;
                pts[i].Y -= rcLocalBounds.Y;
            }

            // init control points
            InitControlPoints(pts);

            // assign new GraphicsPath
            m_gpPath = this.CreateLogicalGraphicsPath(pts);
            
            this.PathPoints = (PointF[])this.GraphicsPath.PathPoints;

            // calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounding.Width / 2, rectBounding.Height / 2);

            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounding.X + rectBounding.Width / 2,
                rectBounding.Y + rectBounding.Height / 2);

            // assign node size value
            SizeF szSizeUnitIndependent = new SizeF(rectBounding.Width, rectBounding.Height);

            // Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            this.EditStyle.AllowRotate = false;
            m_bIsVertexEditable = false;

            UpdateBoundingRectangle();

            // init line segments
            UpdateSegments();
        }

        /// <summary>
        /// Updates the control points.
        /// </summary>
        /// <param name="ptsPoints">The array of points.</param>
        private void InitControlPoints(PointF[] ptsPoints)
        {
            if (ptsPoints.Length == 3)
            {
                PointF ptControlLocation = ptsPoints[1];
                ControlPoint ctrlPt = new ControlPoint(this, ptControlLocation, 0);
                ctrlPt.UpdateServiceReferences(this);

                this.ControlPoints.Add(ctrlPt);
            }
        }
        #endregion

        #region Synhronization methods
        /// <summary>
        /// Saves the path points to save form. 
        /// Used for correct heading calculation.
        /// </summary>
        private void SaveSynchronizationState()
        {
            // save path points before route to heading
            m_ptsPathClone = this.GetPathPoints();

            // convert points to model coordinates
            Matrix mtxPathMatrix = HandlesHitTesting.GetParentsTransformations(this, true);
            mtxPathMatrix.TransformPoints(m_ptsPathClone);
        }

        /// <summary>
        /// Synchronization control point to heading direction.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        internal void SyncWithHeading(EndPoint endPoint)
        {
            // get head and tail end point ports
            ConnectionPoint port = endPoint.Port;
            EndPoint headEndPoint = endPoint as HeadEndPoint;
            EndPoint tailEndPoint = endPoint as TailEndPoint;

            // get control point count
            int nLength = this.ControlPoints.Count;

            if (nLength > 0 && port != null && !this.LineRoutingEnabled)
            {
                // disconnect for a while
                NodeCollection node = new NodeCollection();
                node.Add(this);
                ArrayList list = HandlesHitTesting.SaveConnections(node);

                // synchronize tail control point
                if (tailEndPoint != null)
                {
                    ControlPoint cntFirstPoint = this.ControlPoints[0] as ControlPoint;
                    SyncWithHeading(port, this.HeadingTail, cntFirstPoint, this.TailEndPoint);
                }
                else if (headEndPoint != null)
                {
                    // synchronize head control point
                    ControlPoint cntLastPoint = this.ControlPoints[nLength - 1] as ControlPoint;
                    SyncWithHeading(port, this.HeadingHead, cntLastPoint, this.HeadEndPoint);
                }

                HandlesHitTesting.RestoreConnections(list);
            }
        }

        /// <summary>
        /// Synchronization control point to the with port using heading direction.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="heading">The heading.</param>
        /// <param name="cntPoint">The control point.</param>
        /// <param name="endPoint">The end point.</param>
        private void SyncWithHeading(ConnectionPoint port, CompassHeading heading, ControlPoint cntPoint, EndPoint endPoint)
        {
            // dock near segment to heading
            if ((int)heading > 0 && (int)heading < 5 && port != null && !this.LineRoutingEnabled)
            {
                // get default line route distance
                float fDistance = this.HeadingDistance;

                RectangleF rcBounds = Geometry.GetRouteBounds(port);
                rcBounds.Inflate(fDistance, fDistance);

                Matrix mtx = HandlesHitTesting.GetParentsTransformations(endPoint.Container, false);
                PointF ptPoint = Geometry.AppendMatrix(endPoint.Location, mtx);
                ptPoint = Geometry.GetBoundaryPoint(rcBounds, ptPoint, heading);

                PointF[] pts = new PointF[] { ptPoint };
                Matrix mtxTransform = HandlesHitTesting.GetParentsTransformations(this, true);
                mtxTransform.Invert();
                mtxTransform.TransformPoints(pts);

                if (cntPoint.Location != pts[0])
                {
                    cntPoint.Location = pts[0];
                    SyncSegments(cntPoint);
                }
            }
        }

        /// <summary>
        /// Synchronize end point segments.
        /// </summary>
        private void SyncEndPointSegments()
        {
            SafeHistoryPause();

            // synchronize head end point
            if (IsHeadConnected())
                SyncSegments(this.HeadEndPoint);

            // synchonize tail end point
            if (IsTailConnected())
                SyncSegments(this.TailEndPoint);

            SafeHistoryResume();
        }

        /// <summary>
        /// Synchronized realted sement with moved handle.
        /// </summary>
        /// <param name="handleMoved">Moved handle.</param>
        private void SyncSegments(IHandle handleMoved)
        {
            // get segment index of moved handle
            int nIndex = this.ControlPoints.IndexOf(handleMoved) + 1;

            if (nIndex == 0 && handleMoved is HeadEndPoint)
                nIndex = this.ControlPoints.Count;

            // move fisrt segment of handle
            IConnectorLineSegment segment = (IConnectorLineSegment)GetLineSegmentAt(nIndex);
            segment.SyncSegmentHandle(handleMoved);

            // move second segment of handle if needed
            if (!(handleMoved is EndPoint) && nIndex > 0)
            {
                segment = (IConnectorLineSegment)GetLineSegmentAt(nIndex - 1);
                segment.SyncSegmentHandle(handleMoved);
            }
            if (this.ControlPoints.Count != 0 && !this.BoundsInfo.IsResizing)
            {
                ControlPoint extraCtrlPt = (ControlPoint)this.ControlPoints[0];
                PointF[] pathPts = GetPathPoints();
                PointF lastPt = pathPts[pathPts.Length - 1];
                if (lastPt.X == extraCtrlPt.Location.X && lastPt.Y == extraCtrlPt.Location.Y && this.ControlPoints.Count == 1)
                {
                    this.ControlPoints.RemoveAt(0);
                }

                if (this.ControlPoints.Count > 0)
                {
                    //Removes the control points which are located at the EndPoints
                    if (pathPts[0].X == extraCtrlPt.Location.X && pathPts[0].Y == extraCtrlPt.Location.Y)
                    {
                        this.ControlPoints.Remove(extraCtrlPt);
                    }

                    if (this.ControlPoints.Count > 0)
                    {
                        extraCtrlPt = (ControlPoint)this.ControlPoints[this.ControlPoints.Count - 1];
                        if (lastPt.X == extraCtrlPt.Location.X && lastPt.Y == extraCtrlPt.Location.Y)
                        {
                            if (this.ControlPoints.Count > 2)
                                this.ControlPoints.Remove(extraCtrlPt);
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Determines whether connector can merge control points by specified position.
        /// </summary>
        /// <param name="nIndex">Index of the path point.</param>
        /// <param name="nLength">Length of the path points.</param>
        /// <param name="ptsPath">The array of points.</param>
        /// <returns>
        /// <c>true</c> if this connector can merge control points by specified position; otherwise, <c>false</c>.
        /// </returns>
        private bool CanMergeControlPoints(int nIndex, int nLength, PointF[] ptsPath)
        {
            bool bCanMegre = true;

            // check for head heading 
            if ((int)this.HeadingHead > 0 && (int)this.HeadingHead < 5)
            {
                if (Geometry.EqualPoints(ptsPath[nLength], ptsPath[nLength - 1], 0) && nIndex > nLength - 3)
                {
                    bCanMegre = false;
                }
            }

            // check for tail heading 
            if (bCanMegre && (int)this.HeadingTail > 0 && (int)this.HeadingTail < 5)
            {
                if (Geometry.EqualPoints(ptsPath[1], ptsPath[2], 0) && nIndex < 2)
                {
                    bCanMegre = false;
                }
            }

            return bCanMegre;
        }

        /// <summary>
        /// Route the end points to heading.
        /// </summary>
        /// <param name="strPropertyName">Name of the property from DPN.</param>
        /// <param name="fDistance">The route distance.</param>
        private void RouteHeading(string strPropertyName, float fDistance)
        {
            PointF[] ptsPath = null;
            bool bMerge = (this.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints;

            // relate with rpoerty changed
            if (strPropertyName == DPN.HeadingHead || strPropertyName == DPN.HeadingDistance)
                ptsPath = GetRoutedPath(this.HeadEndPoint, fDistance);

            if (strPropertyName == DPN.HeadingTail || strPropertyName == DPN.HeadingDistance)
                ptsPath = GetRoutedPath(this.TailEndPoint, fDistance);

            if (ptsPath != null)
            {
                // turn off merging for a while update points
                if (bMerge)
                    this.ConnectorState &= ~ConnectorState.MergeControlPoints;

                SetPointsInternal(ptsPath);

                // SetPoints( ptsPath );
                // restore previous megre state
                if (bMerge)
                    this.ConnectorState |= ConnectorState.MergeControlPoints;
            }
        }

        /// <summary>
        /// Gets the end point routed path in local coordinates.
        /// </summary>
        /// <param name="endPoint">The end point to route.</param>
        /// <param name="fDistance">The route distance.</param>
        /// <returns>
        /// The new routed path points in local coordinates.
        /// </returns>
        private PointF[] GetRoutedPath(EndPoint endPoint, float fDistance)
        {
            if (m_ptsPathClone == null)
                SaveSynchronizationState();

            // prepare to route
            PointF[] ptsPathRouted = (PointF[])m_ptsPathClone.Clone();

            // router path points
            if (endPoint != null && endPoint.Port != null)
            {
                Geometry.RouteEndPointToHeading(ref ptsPathRouted, endPoint, fDistance);
            }

            // routed path that contain new routed points
            if (ptsPathRouted != null)
            {
                // get parent transformation
                Matrix mtxPathMatrix = HandlesHitTesting.GetParentsTransformations(this, true);
                mtxPathMatrix.Invert();

                // convert to local coordinates
                mtxPathMatrix.TransformPoints(ptsPathRouted);
            }

            return ptsPathRouted;
        }

        /// <summary>
        /// Calcs the segment center point.
        /// </summary>
        /// <param name="ptSegmentStart">Segment's start point.</param>
        /// <param name="ptSegmentEnd">Segment's end point.</param>
        /// <returns>Segment's center point.</returns>
        private PointF GetSegmentCenter(PointF ptSegmentStart, PointF ptSegmentEnd)
        {
            SizeF szSize = new SizeF(ptSegmentEnd.X - ptSegmentStart.X, ptSegmentEnd.Y - ptSegmentStart.Y);
            return new PointF(ptSegmentStart.X + szSize.Width / 2, ptSegmentStart.Y + szSize.Height / 2);
        }

        /// <summary>
        /// Filters the control points.
        /// </summary>
        /// <param name="ptsPath">The path points.</param>
        protected void FilterControlPoints(ref PointF[] ptsPath)
        {
            // array containing path points excluding endpoints
            // and valid corresponding control points
            ArrayList ptsNew = new ArrayList();
            ptsNew.AddRange(ptsPath);

            // remove endpoints
            ptsNew.RemoveAt(0);
            ptsNew.RemoveAt(ptsNew.Count - 1);

            ArrayList lstToDelete = new ArrayList();

            // array containing valid control points
            ControlPoint[] lstCtrlPts = new ControlPoint[ptsNew.Count];

            PointF ptCtrl;

            // iterate through control points removing all control points
            // which does not match new PathPoints
            foreach (ControlPoint ctrlCur in this.ControlPoints)
            {
                ptCtrl = ctrlCur.Location;
                PointF ptCur;

                for (int k = 0, nLngth = ptsNew.Count; nLngth > k; k++)
                {
                    ptCur = (PointF)ptsNew[k];

                    if (Point.Round(ptCur) == Point.Round(ptCtrl) && (lstCtrlPts[k] == null))
                    {
                        // save valid control point
                        lstCtrlPts[k] = ctrlCur;

                        lstToDelete.Add(ptCur);

                        break;
                    }
                }
            }

            foreach (object ptCur in lstToDelete)
            {
                ptsNew.Remove(ptCur);
            }

            ptsPath = (PointF[])ptsNew.ToArray(typeof(PointF));

            // update deleting CtrlPts service references
            foreach (ControlPoint ctrlPt in this.ControlPoints)
            {
                ctrlPt.UpdateServiceReferences(null);
            }

            // clear control point collection
            this.ControlPoints.Clear();
            this.ControlPoints.AddRange(lstCtrlPts);
        }
        private int[] GetValidCtrlPtsID(int nNewPathPtsCount)
        {
            if (nNewPathPtsCount == 0) return new int[0];

            int nLength = this.ControlPoints.Count;
            ControlPoint ctrlPt;
            ArrayList lstIDs = new ArrayList();
            int nCtrlPtsCounter = 0;

            while (nLength > nCtrlPtsCounter)
            {
                // get ctrl pt position to insert
                ctrlPt = this.ControlPoints[nCtrlPtsCounter] as ControlPoint;

                if (ctrlPt != null)
                {
                    lstIDs.Add(Convert.ToInt64(ctrlPt.ID));
                }

                nCtrlPtsCounter++;
            }

            Int64[] ids = HandlesHitTesting.GetSkippedIndexes((Int64[])lstIDs.ToArray(typeof(Int64)), nNewPathPtsCount);
            int[] idsToReturn = new int[ids.Length];
            for (int i = 0; i < ids.Length; i++)
                idsToReturn[i] = Convert.ToInt32(ids[i]);
            return idsToReturn;
        }
        private void UpdateConnector(IHandle handleMoved)
        {
            if (IsTailConnected() || IsHeadConnected())
            {
                if (handleMoved is ControlPoint && (this.ControlPoints.IndexOf(handleMoved) > 0) && (this.ControlPoints.IndexOf(handleMoved) != this.ControlPoints.Count - 1))
                {
                    // get current path points
                    PointF[] ptsNewPath = GetPathPoints();
                    ConnectorState state = this.ConnectorState;
                    this.ConnectorState = ConnectorState.MergeControlPoints;
                    this.MergeControlPoints(ref ptsNewPath);

                    if (this.PathPoints.Length != ptsNewPath.Length)
                        SetPointsInternal(ptsNewPath);

                    this.ConnectorState = state;
                }
            }           
            
        }
        private void UpdateConnectorState(IHandle handleMoved)
        {
            bool bUpdate = false;

            // get current path points
            ArrayList ptsNewPath = new ArrayList(GetPathPoints());

            if (ptsNewPath.Count == 2)
            {
                PointF[] ptsCurPath = (PointF[])this.PathPoints.Clone();

                if (handleMoved is TailEndPoint)
                {
                    ptsNewPath.Insert(1, ptsCurPath[0]);
                }
                else
                {
                    ptsNewPath.Insert(1, ptsCurPath[1]);
                }

                bUpdate = true;
            }
            else
            {
                // define whether connector's points must be updated
                // -------------------------------------------------
                // if connector's TailEndPoint is connected
                // or ConnectorState's TailEndPointConnected flag is set
                if (IsTailConnected())
                {
                    // consider moving handle
                    // -------------------------------------------------
                    // if we are moving control point which forms
                    // first line segment insert two more equal points before it
                    // else
                    // if handleMoved is TailEndPoint and
                    // there are only two path points add control point
                    if (handleMoved is ControlPoint && (this.ControlPoints.IndexOf(handleMoved) == 0))
                    {
                        // as currently path points !!DOES NOT!! form ortho line
                        // define second segment point manually depending on current valid path points
                        PointF[] ptsCurPath = (PointF[])this.PathPoints.Clone();

                        PointF[] pts = new PointF[2];

                        if (GetFirstSegmentOrientation(ptsCurPath) == Orientation.Vertical)
                        {
                            // get first segment center point
                            pts[0] = new PointF(((PointF)ptsNewPath[0]).X, ((PointF)ptsNewPath[1]).Y);
                            pts[1] = new PointF(((PointF)ptsNewPath[1]).X, ((PointF)ptsNewPath[2]).Y);
                        }
                        else
                        {
                            // get first segment center point
                            pts[0] = new PointF(((PointF)ptsNewPath[1]).X, ((PointF)ptsNewPath[0]).Y);
                            pts[1] = new PointF(((PointF)ptsNewPath[2]).X, ((PointF)ptsNewPath[1]).Y);
                        }

                        // insert new points after corresponding TailEndPoint point
                        ptsNewPath.Insert(1, pts[0]);
                        ptsNewPath.Insert(3, pts[1]);

                        // update connector
                        bUpdate = true;
                    }
                }

                if (IsHeadConnected())
                {
                    // consider moving handle
                    // -------------------------------------------------
                    // if we are moving control point which forms
                    // last line segment insert two more equal points before it
                    // else
                    // if handleMoved is TailEndPoint and
                    // there are only two path points add control point
                    if (!bUpdate && handleMoved is ControlPoint && (this.ControlPoints.IndexOf(handleMoved) == this.ControlPoints.Count - 1))
                    {
                        // as currently path points !!DOES NOT!! form ortho line
                        // define second segment point manually depending on current valid path points
                        PointF[] ptsCurPath = (PointF[])this.PathPoints.Clone();

                        PointF[] pts = new PointF[2];
                        int nDigits = 2;
                        PointF ptStart, ptEnd;
                        //gets the orientation of last segment
                        ptStart = new Point((int)Math.Round(ptsCurPath[ptsCurPath.Length - 1].X, nDigits), (int)Math.Round(ptsCurPath[ptsCurPath.Length - 1].Y, nDigits));
                        ptEnd = new Point((int)Math.Round(ptsCurPath[ptsCurPath.Length - 2].X, nDigits), (int)Math.Round(ptsCurPath[ptsCurPath.Length - 2].Y, nDigits));

                        if ((ptStart.X == ptEnd.X) && (ptStart.Y != ptEnd.Y))
                        {
                            // get last segment center point
                            pts[0] = new PointF(((PointF)ptsNewPath[ptsNewPath.Count - 1]).X, ((PointF)ptsNewPath[ptsNewPath.Count - 2]).Y);
                            pts[1] = new PointF(((PointF)ptsNewPath[ptsNewPath.Count - 2]).X, ((PointF)ptsNewPath[ptsNewPath.Count - 3]).Y);
                        }
                        else
                        {
                            // get last segment center point
                            pts[0] = new PointF(((PointF)ptsNewPath[ptsNewPath.Count - 2]).X, ((PointF)ptsNewPath[ptsNewPath.Count - 1]).Y);
                            pts[1] = new PointF(((PointF)ptsNewPath[ptsNewPath.Count - 3]).X, ((PointF)ptsNewPath[ptsNewPath.Count - 2]).Y);
                        }
                        // insert new points after corresponding HeadEndPoint point
                        ptsNewPath.Insert(ptsNewPath.Count - 2, pts[1]);
                        ptsNewPath.Insert(ptsNewPath.Count - 1, pts[0]);

                        // update connector
                        bUpdate = true;
                    }
                }
            }

            if (bUpdate)
            {
                // covert to array
                PointF[] ptsTemp = (PointF[])ptsNewPath.ToArray(typeof(PointF));
                bool bMerge = (this.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints;

                // turn off merging for a while update points
                if (bMerge)
                    this.ConnectorState &= ~ConnectorState.MergeControlPoints;

                ConnectorState state = this.ConnectorState;
                this.ConnectorState = ConnectorState.MergeControlPoints;
                if (this.PathPoints.Length > 3)
                    this.MergeControlPoints(ref ptsTemp);
                // set new path points and update segments
                SetPointsInternal(ptsTemp);
                this.ConnectorState = state;
                // restore previous megre state
                if (bMerge)
                    this.ConnectorState |= ConnectorState.MergeControlPoints;
            }
        }

        /// <summary>
        /// Updates the looping segments of the connector 
        /// </summary>
        public void MergeSegements()
        {
            ArrayList points = new ArrayList();
            ConnectorLineSegment baseSegment = null;
            if (this.LineSegments.Count > 2)
            {
                for (int segIndex = 0; segIndex < this.LineSegments.Count; segIndex++)
                {
                    baseSegment = this.LineSegments[segIndex] as ConnectorLineSegment;

                    if (baseSegment == null)
                        return;

                    RectangleF baseSegmentRect = Geometry.CreateRect(baseSegment.Point1, baseSegment.Point2);

                    foreach (ConnectorLineSegment curSegment in this.LineSegments)
                    {
                        RectangleF curSegmentRect = Geometry.CreateRect(curSegment.Point1, curSegment.Point2);
                        if (baseSegmentRect.IntersectsWith(curSegmentRect))
                        {
                            PointF ptIntersect = PointF.Empty;
                            baseSegment.GetOrthogonalIntersect(curSegment, out ptIntersect);
                            if (ptIntersect != PointF.Empty)
                            {
                                int baseSegmentIndex = this.LineSegments.IndexOf(baseSegment);
                                int curSegmentIndex = this.LineSegments.IndexOf(curSegment);
                                int count = 0;
                                if (baseSegmentIndex < curSegmentIndex)
                                {
                                    this.LineSegments.RemoveRange(baseSegmentIndex + 1, (curSegmentIndex - baseSegmentIndex) - 1);
                                    
                                    PointF[] pathPts = this.PathPoints;
                                    foreach (LineSegment conSegment in this.LineSegments)
                                    {
                                        if (count == 0 && this.TailEndPoint.Container != null)
                                            pathPts[0] = baseSegment.GetFirstPointLocation(null);
                                        points.Add(conSegment.Point1);
                                        if (count < this.PathPoints.Length -1 && pathPts[count] == ((LineSegment)this.LineSegments[baseSegmentIndex]).Point1)
                                            points.Add(ptIntersect);
                                        else
                                            points.Add(conSegment.Point2);
                                        count++;
                                    }
                                }
                                else
                                {
                                    this.LineSegments.RemoveRange(curSegmentIndex + 1, (baseSegmentIndex - curSegmentIndex) - 1);
                                    foreach (LineSegment conSegment in this.LineSegments)
                                    {
                                        points.Add(conSegment.Point1);
                                        if (count < this.PathPoints.Length -1 && this.PathPoints[count] == ((LineSegment)this.LineSegments[baseSegmentIndex]).Point2)
                                            points.Add(ptIntersect);
                                        else
                                            points.Add(conSegment.Point2);
                                        count++;
                                    }
                                }
                                PointF[] pts = (PointF[])points.ToArray(typeof(PointF));
                                ConnectorState state = this.ConnectorState;
                                this.ConnectorState = ConnectorState.MergeControlPoints;
                                this.MergeControlPoints(ref pts);
                                if (points.Count != pts.Length)
                                    SetPointsInternal(pts);
                                this.ConnectorState = state;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the orientation offset.
        /// </summary>
        /// <param name="szOffset">The offset size.</param>
        /// <param name="segIndex">Index of the segment.</param>
        /// <returns>Offset size.</returns>
        private SizeF GetOrientationOffset(SizeF szOffset, int segIndex)
        {
            OrthogonalLineSegment segment = (OrthogonalLineSegment)GetLineSegmentAt(segIndex);

            if (segment.Orientation == Orientation.Horizontal)
                szOffset.Width = 0;
            else
                szOffset.Height = 0;

            return szOffset;
        }
        #endregion

        /// <summary>
        /// Helper enumerator of heading states.
        /// Use internal only.
        /// </summary>
        private enum HeadingState
        {
            /// <summary>
            /// No heading state.
            /// </summary>
            None = 0,

            /// <summary>
            /// Heading head state.
            /// </summary>
            HeadingHead,

            /// <summary>
            /// Heading tail state.
            /// </summary>
            HeadingTail
        }
    }
}