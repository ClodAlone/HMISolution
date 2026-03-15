#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Node that will be rendered as Poly line.
    /// </summary>
    [Serializable]
    public class PolylineNode
        : LineBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineNode"/> class.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        public PolylineNode(PointF[] pts)
        {
            // update line bounds
            InitializePolyLine(pts);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineNode"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public PolylineNode(PolylineNode src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineNode"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected PolylineNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Indicate that selection and resize handles will draw on diagram canvas.
        /// </summary>
        /// <returns><c>True</c> if node can be resized, otherwise - <c>false</c>.</returns>
        public override bool ShowResizeHandles()
        {
            return this.EditStyle.DefaultHandleEditMode == HandleEditMode.Resize;
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
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new PolylineNode(this);
        }

        /// <summary>
        /// Updates the graphics path.
        /// </summary>
        /// <param name="pts">The new path points array.</param>
        protected override void UpdateGraphicsPath(PointF[] pts)
        {
            Geometry.TranslateToGridOrigin(pts);

            // assign new GraphicsPath
            m_gpPath = CreateLogicalGraphicsPath(pts);
        }

        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLines(pts);

            return path;
        }

        /// <summary>
        /// Sets the points internal.
        /// </summary>
        /// <param name="ptsPath">The new path points.</param>
        protected override void SetPointsInternal(PointF[] ptsPath)
        {
            base.SetPointsInternal(ptsPath);

            // save pin offset to history
            UpdatePathNodeData();
        }
        #endregion

        #region Class Initialize shape
        /// <summary>
        /// Initializes the poly line.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        private void InitializePolyLine(PointF[] pts)
        {
            // get bounding rect
            RectangleF rectBounding = Geometry.CreateRect(pts);
            Geometry.TranslateToGridOrigin(pts);

            m_gpPath = CreateLogicalGraphicsPath(pts);
            GraphicsPath path = this.GraphicsPath;

            this.PathPoints = (PointF[])path.PathPoints.Clone();

            m_minPoints = 3;
            m_bIsVertexEditable = true;
            m_bCanChangePath = true;

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

            UpdateBoundingRectangle();
        }
        #endregion
    }
}
