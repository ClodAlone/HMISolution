#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Text;
using Syncfusion.Windows.Forms.Diagram;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// OrgLine connector is used to create Org chart diagram connector.
    /// </summary>
    [Serializable]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class OrgLineConnector : ConnectorBase
    {
        #region Class Members.

        private float m_fVerticalDistance = 25f;
        private bool distanceEnabled = false;
        private bool rotationEnabled = false;
        private float m_fCurveRadius = 8;
        #endregion.

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OrgLineConnector"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public OrgLineConnector(PointF start, PointF end)
            : base(start, end)
        {
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
            InitializeOrgLine(start, end);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgLineConnector"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="units">The units.</param>
        public OrgLineConnector(PointF start, PointF end, MeasureUnits units)
            : base(start, end, units)
        {
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
            InitializeOrgLine(start, end);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgLineConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The tail end point location.</param>
        /// <param name="ptEnd">The head end point location.</param>
        /// <param name="edge">true if the edge should be round; otherwise, false.</param>
        public OrgLineConnector(PointF ptStart, PointF ptEnd, bool edge)
            : base(ptStart, ptEnd)
        {
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
            if (edge)
                this.EnableRoundedCorner = true;
            else
                this.EnableRoundedCorner = false;
            InitializeOrgLine(ptStart, ptEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgLineConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The tail end point location.</param>
        /// <param name="ptEnd">The head end point location.</param>
        /// <param name="edge">true if the edge should be round; otherwise, false.</param>
        /// <param name="curveRadius">Radius of the curve.</param>
        public OrgLineConnector(PointF ptStart, PointF ptEnd, bool edge, float curveRadius)
            : base(ptStart, ptEnd)
        {
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
            this.CurveRadius = curveRadius;
            if (edge)
                this.EnableRoundedCorner = true;
            else
                this.EnableRoundedCorner = false;
            InitializeOrgLine(ptStart, ptEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgLineConnector"/> class.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public OrgLineConnector(OrgLineConnector src)
            : base(src)
        {
            rotationEnabled = src.rotationEnabled;
            m_fVerticalDistance = src.m_fVerticalDistance;
            m_fCurveRadius = src.m_fCurveRadius;
            distanceEnabled = src.distanceEnabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgLineConnector"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OrgLineConnector(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                if (entry.Name == "rotationEnabled")
                {
                    rotationEnabled = (bool)info.GetValue("rotationEnabled", typeof(bool));
                }
                else if (entry.Name == "VerticalDistance")
                {
                    m_fVerticalDistance = (float)entry.Value;
                }
                else if (entry.Name == "CurveRadius")
                    m_fCurveRadius = float.Parse(entry.Value.ToString());                        
            }
        }
        #endregion.

        #region Properties.

        /// <summary>
        /// Gets or sets the vertical distance of this connector.
        /// </summary>
        public float VerticalDistance
        {
            get
            {
                return m_fVerticalDistance;
            }
            set
            {
                if (m_fVerticalDistance != value && OnPropertyChanging(this.FullContainerName, DPN.VerticalDistance, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.VerticalDistance);

                    m_fVerticalDistance = value;
                    if (value > 0 && value < 100)
                        VerticalDistanceEnabled = true;
                    else
                        VerticalDistanceEnabled = false;

                    //Updates the org line based on its vertical distance
                    UpdateConnector(null);

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.VerticalDistance);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user to set heading distance of the connector.
        /// </summary>
        internal bool VerticalDistanceEnabled
        {
            get
            {
                return distanceEnabled;
            }
            set
            {
                distanceEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the organization chart rotation property enabled.
        /// </summary>
        internal bool OrgRotationEnabled
        {
            get
            {
                return rotationEnabled;
            }
            set
            {
                rotationEnabled = value;
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

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.CurveRadius);
                }
            }
        }
        #endregion

        #region Public Overrides.
        /// <summary>
        /// Clones this OrgLineConnector instance
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new OrgLineConnector(this);
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

        #region Overrides
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
        }

        /// <summary>
        /// Performs additional changes on pin position changed.
        /// </summary>
        /// <param name="fX">The pin offset by x axis.</param>
        /// <param name="fY">The pin offset by y axis.</param>
        protected override void DoMoveRelatedActions(float fX, float fY)
        {
            base.DoMoveRelatedActions(fX, fY);
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
        }

        /// <summary>
        /// Does the handle move related. It will calculate path points between tail and head endpoint
        /// </summary>
        /// <param name="handleMoved">The handle moved.</param>
        /// <param name="szOffset">The size offset.</param>
        protected override void DoHandleMoveRelated(IHandle handleMoved, SizeF szOffset)
        {
            if (!this.ObstaclesInPath && !this.BoundsInfo.IsSegmentChanging)
            {
                // update connectors' control points if needed
                UpdateConnector(handleMoved);
            }

            // synchronize segments with handle
            SyncSegments(handleMoved);

            if (!this.BoundsInfo.IsResizing)
                //Updates the loops in the connector
                MergeSegements();

            // call base to MergeControlPoints
            base.DoHandleMoveRelated(handleMoved, szOffset);

            UpdatePathNodeData();

            // update bridges
            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);

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
        /// Set only to the created path points.
        /// </summary>
        /// <param name="ptsPath">No of path points</param>
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
        /// Add the values to the serialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("rotationEnabled", rotationEnabled);
            info.AddValue("VerticalDistance", m_fVerticalDistance, typeof(float));
            info.AddValue("CurveRadius", m_fCurveRadius);
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
        /// Validates the offset.
        /// </summary>
        /// <param name="lineSegmentIndex">Index of the line segment.</param>
        /// <param name="szOffset">The move offset.</param>
        protected override void ValidateOffset(int lineSegmentIndex, ref SizeF szOffset)
        {
            if (!distanceEnabled)
                szOffset = GetOrientationOffset(szOffset, lineSegmentIndex);
            else
                szOffset = SizeF.Empty;
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
        
        #endregion

        #region Helpher methods.
        /// <summary>
        /// Initializes the orthogonal.
        /// </summary>
        /// <param name="ptStart">The start point location.</param>
        /// <param name="ptEnd">The end point location.</param>
        protected void InitializeOrgLine(PointF ptStart, PointF ptEnd)
        {
            PointF[] pathPts;
            PointF ptInt1 = PointF.Empty, ptInt2 = PointF.Empty;
            float offset, distance;

            RectangleF frmNodeBounds = RectangleF.Empty, toNodeBounds = RectangleF.Empty, modelBounds = RectangleF.Empty;
            Model model = this.Parent as Model;
            if (model != null)
                modelBounds.Width = model.LogicalSize.Width;

            if (distanceEnabled)
                offset = m_fVerticalDistance;
            else
                offset = 50;
            distance = (float)ptEnd.Y - ptStart.Y;
            offset = (distance * offset) / 100;
            if (ptStart.X != ptEnd.X && ptStart.Y != ptEnd.Y)
            {
                ptInt1 = new PointF(ptStart.X, ptStart.Y + offset);
                ptInt2 = new PointF(ptEnd.X, ptStart.Y + offset);
                pathPts = new PointF[] { ptStart, ptInt1, ptInt2, ptEnd };
            }
            else
                pathPts = new PointF[] { ptStart, ptEnd };

            // get bounding rect
            RectangleF rectBounding = Geometry.CreateRect(pathPts);

            Geometry.TranslateToGridOrigin(pathPts);
            // assign new GraphicsPath
            System.Drawing.Drawing2D.GraphicsPath gpPath = CreateLogicalGraphicsPath(pathPts);
            m_gpPath = gpPath;
            this.PathPoints = (PointF[])this.GraphicsPath.PathPoints.Clone();

            // calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounding.Width / 2, rectBounding.Height / 2);

            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounding.X + rectBounding.Width / 2,
                rectBounding.Y + rectBounding.Height / 2);

            // assign node size value
            SizeF szSizeUnitIndependent = rectBounding.Size;

            // Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            m_bIsVertexEditable = false;

            // update bounds
            UpdateBoundingRectangle();

            // set new points
            this.SetPointsInternal(pathPts);

            // quiet update node path
            UpdatePathNodeData();

            FinishSetPoints();
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
        }

        /// <summary>
        /// updates the connector's control points
        /// </summary>
        /// <param name="handleMoved">Handle moved</param>
        private void UpdateConnector(IHandle handleMoved)
        {
            // get current path points
            ArrayList ptsNewPath = new ArrayList(GetPathPoints());

            float offset, distance;            
            Matrix mtxTemp = GetTransformations();
            bool bUpdate = false, bUpdatePath = false;
            PointF[] ptsToReturn = new PointF[this.ControlPoints.Count + 2];
            PointF[] ptsCurPath = (PointF[])this.PathPoints.Clone();
            PointF ptStart = PointF.Empty, ptEnd = PointF.Empty;
            PointF ptInt1 = PointF.Empty, ptInt2 = PointF.Empty;
            RectangleF frmNodeBounds, toNodeBounds, modelBounds = RectangleF.Empty;
            Model model = this.Parent as Model;
            if (model != null)
                modelBounds.Width = model.LogicalSize.Width;
            
            if (handleMoved!= null && handleMoved is ControlPoint)
            {
                int index = this.ControlPoints.IndexOf(handleMoved);
                if (!IsTailConnected() || !IsHeadConnected())
                {
                    PointF curCtrlPnt = handleMoved.Location;
                    PointF[] pathPts = (PointF[])ptsNewPath.ToArray(typeof(PointF));
                    if (index == this.ControlPoints.Count - 1)
                    {
                        pathPts[1] = new PointF(pathPts[1].X, curCtrlPnt.Y);
                        pathPts[pathPts.Length - 1] = new PointF(curCtrlPnt.X, pathPts[pathPts.Length - 1].Y);
                    }
                    else if (index == 0)
                    {
                        pathPts[0] = new PointF(curCtrlPnt.X, pathPts[0].Y);
                        pathPts[pathPts.Length - 2] = new PointF(pathPts[pathPts.Length - 2].X, curCtrlPnt.Y);
                    }

                    // update connector
                    bUpdate = true;

                    // convert to array
                    ptsToReturn = pathPts;
                }

                if (IsTailConnected() && this.ControlPoints.IndexOf(handleMoved) == 0)
                {
                    // define second segment point manually depending on current valid path points
                    PointF ptTemp = new PointF(((PointF)ptsNewPath[0]).X, ((PointF)ptsNewPath[1]).Y);
                    PointF ptTemp1 = new PointF(((PointF)ptsNewPath[1]).X, ((PointF)ptsNewPath[2]).Y);

                    // insert new points after corresponding TailEndPoint point
                    ptsNewPath.Insert(1, ptTemp);
                    ptsNewPath.Insert(3, ptTemp1);

                    // update connector
                    bUpdate = true;

                    // convert to array
                    ptsToReturn = (PointF[])ptsNewPath.ToArray(typeof(PointF));
                }
                if (IsHeadConnected() && index == this.ControlPoints.Count - 1)
                {
                    PointF ptTemp = new PointF(((PointF)ptsNewPath[ptsNewPath.Count - 3]).X, ((PointF)ptsNewPath[ptsNewPath.Count - 2]).Y);
                    PointF ptTemp1 = new PointF(((PointF)ptsNewPath[ptsNewPath.Count - 2]).X, ((PointF)ptsNewPath[ptsNewPath.Count - 1]).Y);

                    toNodeBounds = ((Node)this.ToNode).BoundingRectangle;
                    modelBounds = new RectangleF(0, toNodeBounds.Y, modelBounds.Width, toNodeBounds.Height);
                    if (IsTailConnected())
                        frmNodeBounds = ((Node)this.FromNode).BoundingRectangle;
                    else
                        frmNodeBounds = new RectangleF(this.TailEndPoint.Location, new SizeF(8, 8));
                    if (modelBounds.IntersectsWith(frmNodeBounds))
                    {
                        // insert new points after corresponding TailEndPoint point
                        ptsNewPath.Insert(ptsNewPath.Count - 2, ptTemp);
                        ptsNewPath.Insert(ptsNewPath.Count - 1, ptTemp1);
                    }
                    else
                    {
                        ptTemp = new PointF(((PointF)ptsNewPath[ptsNewPath.Count - 1]).X, ((PointF)ptsNewPath[ptsNewPath.Count - 2]).Y);
                        ptTemp1 = new PointF(((PointF)ptsNewPath[ptsNewPath.Count - 2]).X, ((PointF)ptsNewPath[ptsNewPath.Count - 3]).Y);
                        // insert new points after corresponding TailEndPoint point
                        ptsNewPath.Insert(ptsNewPath.Count - 2, ptTemp1);
                        ptsNewPath.Insert(ptsNewPath.Count - 1, ptTemp);
                    }

                    // update connector
                    bUpdate = true;

                    // convert to array
                    ptsToReturn = (PointF[])ptsNewPath.ToArray(typeof(PointF));
                }
            }
            else if (!this.ObstaclesInPath)
            {                
                ptStart = this.TailEndPoint.Location;
                ptEnd = this.HeadEndPoint.Location;
                if (distanceEnabled)
                    offset = m_fVerticalDistance;
                else
                    offset = 50;

                if (this.ControlPoints.Count <= 2)
                {
                    string headName, tailName;
                    tailName = this.HeadingTail.ToString();
                    headName = this.HeadingHead.ToString();

                    if (this.HeadingHead == CompassHeading.Southeast || this.HeadingHead == CompassHeading.Northeast)
                    {
                        headName = "East";
                    }
                    if (this.HeadingHead == CompassHeading.Northwest || this.HeadingHead == CompassHeading.Southwest)
                    {
                        headName = "West";
                    }
                    if (this.HeadingTail == CompassHeading.Northwest || this.HeadingHead == CompassHeading.Southwest)
                    {
                        tailName = "West";
                    }
                    if (this.HeadingTail == CompassHeading.Northeast || this.HeadingTail == CompassHeading.Southeast)
                    {
                        tailName = "East";
                    }
                    string direction = tailName + "To" + headName;
                    if (direction == "EastToEast" || direction == "WestToEast" || direction == "EastToWest" || direction == "WestToWest")
                    {
                        bUpdate = true;
                        distance = (float)ptEnd.X - ptStart.X;
                        offset = (distance * offset) / 100;
                        ptInt1 = new PointF(ptStart.X + offset, ptStart.Y);
                        ptInt2 = new PointF(ptStart.X + offset, ptEnd.Y);
                        ptsToReturn = new PointF[] { ptStart, ptInt1, ptInt2, ptEnd };
                    }
                    else if (direction == "SouthToWest" && (IsHeadConnected() && IsTailConnected()))
                    {
                        bUpdate = true;                       
                        ptInt1 = new PointF(ptStart.X, ptEnd.Y);
                        ptInt2 = new PointF(ptStart.X ,ptEnd.Y);
                        ptsToReturn = new PointF[] { ptStart, ptInt1, ptEnd };
                    }
                    else if (direction == "EastToSouth" && (IsHeadConnected() && IsTailConnected()))
                    {
                        bUpdate = true;
                        ptInt1 = new PointF(ptStart.X, ptEnd.Y);
                        ptInt2 = new PointF(ptStart.X, ptEnd.Y);
                        ptsToReturn = new PointF[] { ptStart, ptInt1, ptEnd };
                    }
                    else if (direction == "SouthToEast" && (IsHeadConnected() && IsTailConnected()))
                    {
                        bUpdate = true;
                        ptInt1 = new PointF(ptStart.X, ptEnd.Y);
                        ptInt2 = new PointF(ptStart.X, ptEnd.Y);
                        ptsToReturn = new PointF[] { ptStart, ptInt1, ptEnd };
                    }
                    else if (this.HeadEndPoint.Location.X != this.TailEndPoint.Location.X && this.HeadEndPoint.Location.Y != this.TailEndPoint.Location.Y)
                    {
                        bUpdate = true;
                        if (!IsHeadConnected() && IsTailConnected() && this.TailEndPoint.Port is CentralPort)
                        {
                            frmNodeBounds = ((Node)this.FromNode).BoundingRectangle;
                            modelBounds = new RectangleF(0, frmNodeBounds.Y, modelBounds.Width, frmNodeBounds.Height);
                            toNodeBounds = new RectangleF(this.HeadEndPoint.Location, new SizeF(8, 8));

                            if (modelBounds.IntersectsWith(toNodeBounds))
                            {
                                bUpdatePath = true;
                            }
                        }
                        else if (!IsTailConnected() && IsHeadConnected() && this.HeadEndPoint.Port is CentralPort)
                        {
                            toNodeBounds = ((Node)this.ToNode).BoundingRectangle;
                            modelBounds = new RectangleF(0, toNodeBounds.Y, modelBounds.Width, toNodeBounds.Height);
                            frmNodeBounds = new RectangleF(this.TailEndPoint.Location, new SizeF(8, 8));
                            if (modelBounds.IntersectsWith(frmNodeBounds))
                            {
                                bUpdatePath = true;                               
                            }
                        }
                        else if (IsTailConnected() && IsHeadConnected() && this.HeadEndPoint.Port is CentralPort && this.TailEndPoint.Port is CentralPort)
                        {
                            frmNodeBounds = ((Node)this.FromNode).BoundingRectangle;
                            toNodeBounds = ((Node)this.ToNode).BoundingRectangle;
                            modelBounds = new RectangleF(0, frmNodeBounds.Y, modelBounds.Width, frmNodeBounds.Height);
                            if (modelBounds.IntersectsWith(toNodeBounds))
                            {
                                bUpdatePath = true;                                
                            }
                        }
                        if (bUpdatePath)
                        {
                            distance = (float)ptEnd.X - ptStart.X;
                            offset = (distance * offset) / 100;
                            ptInt1 = new PointF(ptStart.X + offset, ptStart.Y);
                            ptInt2 = new PointF(ptStart.X + offset, ptEnd.Y);                            
                        }
                        else
                        {
                            distance = (float)ptEnd.Y - ptStart.Y;
                            offset = (distance * offset) / 100;
                            ptInt1 = new PointF(ptStart.X, ptStart.Y + offset);
                            ptInt2 = new PointF(ptEnd.X, ptStart.Y + offset);
                        }
                        ptsToReturn = new PointF[] { ptStart, ptInt1, ptInt2, ptEnd };
                    }
                    else
                    {
                        bUpdate = true;
                        ptsToReturn = new PointF[] { ptStart, ptEnd };
                    }
                    AppendFlipTransforms(mtxTemp);
                    mtxTemp.Invert();
                    mtxTemp.TransformPoints(ptsToReturn);
                    this.PathPoints = ptsToReturn;
                }
                else
                {
                    bUpdate = true;
                    ptsToReturn = this.GetPathPoints();
                }
            }
            if (bUpdate)
            {
                bool bMerge = (this.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints;

                // turn off merging for a while update points
                if (bMerge)
                    this.ConnectorState &= ~ConnectorState.MergeControlPoints;

                ConnectorState state = this.ConnectorState;
                this.ConnectorState = ConnectorState.MergeControlPoints;
                this.MergeControlPoints(ref ptsToReturn);
                // set new path points and update segments
                SetPointsInternal(ptsToReturn);
                this.ConnectorState = state;

                // restore previous megre state
                if (bMerge)
                    this.ConnectorState |= ConnectorState.MergeControlPoints;
                UpdatePathNodeData();
            }
        }

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
        /// Filters the unwanted control points
        /// </summary>
        /// <param name="ptsPath">path points</param>
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

        /// <summary>
        /// Gets the ids of the valid control points
        /// </summary>
        /// <param name="nNewPathPtsCount">Path point length</param>
        /// <returns>Array of ids of the control point</returns>
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

        /// <summary>
        /// Gets the orientation offset.
        /// </summary>
        /// <param name="szOffset">The offset size.</param>
        /// <param name="segIndex">Index of the segment.</param>
        /// <returns>Offset size.</returns>
        private SizeF GetOrientationOffset(SizeF szOffset, int segIndex)
        {
            ConnectorLineSegment segment = (ConnectorLineSegment)GetLineSegmentAt(segIndex);

            if (segment.Point1.Y == segment.Point2.Y)
                szOffset.Width = 0;
            else
                szOffset.Height = 0;

            return szOffset;
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
                                        if (count < this.PathPoints.Length - 1 && pathPts[count] == ((LineSegment)this.LineSegments[baseSegmentIndex]).Point1)
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
                                        if (count < this.PathPoints.Length - 1 && this.PathPoints[count] == ((LineSegment)this.LineSegments[baseSegmentIndex]).Point2)
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
                                if(points.Count != pts.Length)
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
        /// Updates the control points.
        /// </summary>
        /// <param name="ptsPoints">The array of points.</param>
        private void InitControlPoints(PointF[] ptsPoints)
        {
            PointF ptControlLocation = ptsPoints[1];
            ControlPoint ctrlPt = new ControlPoint(this, ptControlLocation, 0);
            ctrlPt.UpdateServiceReferences(this);

            this.ControlPoints.Add(ctrlPt);

            ptControlLocation = ptsPoints[2];
            ctrlPt = new ControlPoint(this, ptControlLocation, 0);
            ctrlPt.UpdateServiceReferences(this);

            this.ControlPoints.Add(ctrlPt);
        }
        #endregion
    }
}
