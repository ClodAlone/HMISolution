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
    /// Implementation of polygon shapes.
    /// </summary>
    [Serializable]
    public class Polygon
        : FilledPath
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="pts">Points to add to polygon.</param>
        public Polygon(PointF[] pts)
            : base()
        {
            InitializePolygon(pts);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public Polygon(Polygon src)
            : base(src)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected Polygon(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
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
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public override object Clone()
        {
            return new Polygon(this);
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
            GraphicsPath pathToReturn = null;

            if (pts.Length == 2 && (pts[0] != pts[1]))
            {
                pathToReturn = new GraphicsPath();
                pathToReturn.AddLine(pts[0], pts[1]);
            }
            else if (pts.Length > 2)
            {
                pathToReturn = new GraphicsPath();
                pathToReturn.AddPolygon(pts);
            }

            return pathToReturn;
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

        #region Class helper methods
        /// <summary>
        /// Initializes the polygon.
        /// </summary>
        /// <param name="pts">The points.</param>
        private void InitializePolygon(PointF[] pts)
        {
            RectangleF rectBounds = Geometry.CreateRect(pts);

            UpdateGraphicsPath(pts);
            this.PathPoints = (PointF[])this.GraphicsPath.PathPoints.Clone();

            // calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);

            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);

            // assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;

            // Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            // Set minimum path points.
            m_minPoints = 3;
            m_bIsVertexEditable = true;
            UpdateBoundingRectangle();
        }
        #endregion
    }
}
