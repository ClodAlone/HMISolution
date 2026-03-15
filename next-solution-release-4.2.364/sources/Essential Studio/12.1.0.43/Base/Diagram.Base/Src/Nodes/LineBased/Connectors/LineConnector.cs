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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class containing Line Connector.
    /// </summary>
    [Serializable]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class LineConnector
        : ConnectorBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        public LineConnector(PointF ptStart, PointF ptEnd)
            : base(ptStart, ptEnd)
        {
            // Fixed D9480
            // if( ptStart == ptEnd )
            //    throw new ArgumentException( "ptStart equals ptEnd" );

            // update line bounds
            InitializeLine();
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public LineConnector(PointF ptStart, PointF ptEnd, MeasureUnits measureUnits)
            : base(ptStart, ptEnd, measureUnits)
        {
            // Fixed D9480
            // if( ptStart == ptEnd )
            //    throw new ArgumentException( "ptStart equals ptEnd" );

            // update line bounds
            InitializeLine();
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        public LineConnector(PointF[] pts)
            : base(pts[0], pts[pts.Length - 1])
        {
            // update line bounds
            InitializePolyLine(pts);
            this.EditStyle.DefaultHandleEditMode = HandleEditMode.Vertex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public LineConnector(LineConnector src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineConnector"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected LineConnector(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class initialize shape
        private void InitializeLine()
        {
            PointF ptStart = this.TailEndPoint.Location;
            PointF ptEnd = this.HeadEndPoint.Location;
            PointF[] ptPointsRelative = new PointF[] { ptStart, ptEnd };
            this.PathPointsRelative = ptPointsRelative;
            // calc line length
            float fLineLength = (float)Geometry.PointDistance(ptStart, ptEnd);

            // assign new GraphicsPath
            GraphicsPath gpPath = new GraphicsPath();
            gpPath.AddLine(0, 0, fLineLength, 0);

            m_gpPath = gpPath;
            this.PathPoints = (PointF[])this.GraphicsPath.PathPoints.Clone();

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

            // cals rotation angle
            float fRotationAngle = (float)Math.Atan2(ptEnd.Y - ptStart.Y, ptEnd.X - ptStart.X);

            // Lock update rotating angle. 
            m_bLockUpdate = true;
            this.RotationAngle = (float)(fRotationAngle * (180f / Math.PI));
            m_bLockUpdate = false;

            m_bIsVertexEditable = false;

            // update bounds
            UpdateBoundingRectangle();

            // update line segments
            UpdateSegments();
        }
        private void InitializePolyLine(PointF[] pts)
        {
            PointF ptStart = this.TailEndPoint.Location;
            PointF ptEnd = this.HeadEndPoint.Location;

            // get bounding rect
            RectangleF rectBounding = Geometry.CreateRect(pts);

            // assign new GraphicsPath
            GraphicsPath gpPath = new GraphicsPath();
            Geometry.TranslateToGridOrigin(pts);
            gpPath.AddLines(pts);

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

            // update line segments
            UpdateSegments();

            // set new points
            SetPointsInternal(pts);

            // quiet update node path
            UpdatePathNodeData();
            FinishSetPoints();
        }
        #endregion

        #region Class overrides
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
        /// Merges the control points placed in line.
        /// </summary>
        /// <param name="ptsNew">New points.</param>
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
        /// Moves the end points to given offset.
        /// </summary>
        /// <param name="szOffset">The move offset.</param>
        protected override void MoveEndPoints(SizeF szOffset)
        {
            if (this.RotationAngle != 0)
            {
                PointF[] ptsOffset = new PointF[] { szOffset.ToPointF() };

                Matrix mtxRotate = new Matrix();
                mtxRotate.Rotate(this.RotationAngle);

                mtxRotate.TransformPoints(ptsOffset);
                szOffset.Width = ptsOffset[0].X;
                szOffset.Height = ptsOffset[0].Y;
            }

            base.MoveEndPoints(szOffset);
        }

        /// <summary>
        /// Updates the Pin position, pin offset and size from new bounds rectangle.
        /// </summary>
        /// <param name="rcBounds">The bounds rectangle.</param>
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

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new LineConnector(this);
        }

        /// <summary>
        /// Perfoms additional changes on handle move.
        /// </summary>
        /// <param name="handleMoved">The moved handle .</param>
        /// <param name="szOffset">The move offset.</param>
        protected override void DoHandleMoveRelated(IHandle handleMoved, SizeF szOffset)
        {
            // update all node info's -> then update connector's bridges
            base.DoHandleMoveRelated(handleMoved, szOffset);

            // update cache segments regions
            UpdateSegmentsRegion();

            // update bridges
            if (m_mgrBridge != null)
                m_mgrBridge.AddToIntersectCollection(this);
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
                m_mgrLink.BeginSynchronization();

                m_mgrLink.SynchronizeEndPoint(this.HeadEndPoint);
                m_mgrLink.SynchronizeEndPoint(this.TailEndPoint);

                m_mgrLink.EndSynchronization();
            }

            m_bLockHandleMove = bLockMove;
        }
        #endregion

        #region Class helper methods
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
    }
}
