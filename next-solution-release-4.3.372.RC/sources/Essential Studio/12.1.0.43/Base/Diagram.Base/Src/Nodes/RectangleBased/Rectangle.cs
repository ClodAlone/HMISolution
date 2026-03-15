#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Implementation of rectangle shapes.
    /// </summary>
    [Serializable]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class Rectangle
        : FilledPath
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="x">X-coordinate of rectangle.</param>
        /// <param name="y">Y-coordinate of rectangle.</param>
        /// <param name="width">Width of rectangle.</param>
        /// <param name="height">Height of rectangle.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public Rectangle(float x, float y, float width, float height, MeasureUnits measureUnits)
            : this(new RectangleF(x, y, width, height), measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="rcBounds">Rectangle containing position and size.</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        public Rectangle(RectangleF rcBounds, MeasureUnits measureUnits)
            : base()
        {
            InitializeRectangle(rcBounds, measureUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="pts">Points to add to rectangle.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        /// <remarks>
        /// The array passed in must contain two points that specify a rectangle.
        /// The first point in the array is the upper-left corner of the rectangle
        /// and the second point is the lower-right corner of the rectangle.
        /// </remarks>
        public Rectangle(PointF[] pts, MeasureUnits measureUnits)
            : this(new RectangleF(pts[0], new SizeF(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y)), measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="x">X-coordinate of rectangle.</param>
        /// <param name="y">Y-coordinate of rectangle.</param>
        /// <param name="width">Width of rectangle.</param>
        /// <param name="height">Height of rectangle.</param>
        public Rectangle(float x, float y, float width, float height)
            : this(new RectangleF(x, y, width, height))
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="rcBounds">Rectangle containing position and size.</param>
        public Rectangle(RectangleF rcBounds)
            : base()
        {
            InitializeRectangle(rcBounds, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="pts">Points to add to rectangle.</param>
        /// <remarks>
        /// The array passed in must contain two points that specify a rectangle.
        /// The first point in the array is the upper-left corner of the rectangle
        /// and the second point is the lower-right corner of the rectangle.
        /// </remarks>
        public Rectangle(PointF[] pts)
            : this(new RectangleF(pts[0], new SizeF(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y)))
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public Rectangle(Rectangle src)
            : base(src)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected Rectangle(SerializationInfo info, StreamingContext context)
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
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public override object Clone()
        {
            return new Rectangle(this);
        }

        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            RectangleF rcBounds = Geometry.CreateRect(pts);
            return PathFactory.CreateRectangle(rcBounds.Location, rcBounds.Size);
        }

        #endregion

        #region Class helper methods
        private void InitializeRectangle(RectangleF rectBounds, MeasureUnits measureUnits)
        {
            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);

            // assign new GraphicsPath
            m_gpPath = CreateGraphicsPath(new PointF[] { new PointF( 0, 0 ), new PointF( rectBounds.Width, rectBounds.Height ) });

            // Set path point.
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

            this.BoundsInfo.Unit = measureUnits;
            UpdateBoundingRectangle();
            m_bCanChangePath = false;
            m_bIsVertexEditable = false;
        }
        #endregion
    }
}
