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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Contains static GraphicsPath objects that can be used for the
    /// visual representation of ports.
    /// </summary>
    public class EndPointVisuals
    {
        /// <summary>
        /// Gets open arrow visual.
        /// </summary>
        public static GraphicsPath OpenArrow
        {
            get
            {
                if (EndPointVisuals.openArrow == null)
                {
                    EndPointVisuals.openArrow = new GraphicsPath();

                    PointF[] pts = new PointF[3]
                    {
                        new PointF(-1, 1),
                        new PointF(0, 0),
                        new PointF(-1, -1)
                    };

                    EndPointVisuals.openArrow.AddLines(pts);

                    Matrix xform = new Matrix();
                    xform.Scale(6.0f, 4.5f, MatrixOrder.Append);
                    EndPointVisuals.openArrow.Transform(xform);
                }

                return EndPointVisuals.openArrow;
            }
        }

        /// <summary>
        /// Gets closed arrow visual.
        /// </summary>
        public static GraphicsPath ClosedArrow
        {
            get
            {
                if (EndPointVisuals.closedArrow == null)
                {
                    EndPointVisuals.closedArrow = new GraphicsPath();

                    PointF[] pts = new PointF[3]
                    {
                        new PointF(-1, 1),
                        new PointF(0, 0),
                        new PointF(-1, -1)
                    };

                    EndPointVisuals.closedArrow.AddPolygon(pts);

                    Matrix xform = new Matrix();
                    xform.Scale(6.0f, 4.5f, MatrixOrder.Append);
                    EndPointVisuals.closedArrow.Transform(xform);
                }

                return EndPointVisuals.closedArrow;
            }
        }

        /// <summary>
        /// Gets circle visual.
        /// </summary>
        public static GraphicsPath Circle
        {
            get
            {
                if (EndPointVisuals.circle == null)
                {
                    EndPointVisuals.circle = new GraphicsPath();

                    PointF[] pts = new PointF[3]
                    {
                        new PointF(-1, 1),
                        new PointF(0, 0),
                        new PointF(-1, -1)
                    };

                    EndPointVisuals.circle.AddEllipse(-1.0f, -1.0f, 2.0f, 2.0f);

                    Matrix xform = new Matrix();
                    xform.Scale(3.5f, 3.5f, MatrixOrder.Append);
                    EndPointVisuals.circle.Transform(xform);
                }

                return EndPointVisuals.circle;
            }
        }

        /// <summary>
        /// Gets diamond visual.
        /// </summary>
        public static GraphicsPath Diamond
        {
            get
            {
                if (EndPointVisuals.diamond == null)
                {
                    EndPointVisuals.diamond = new GraphicsPath();

                    PointF[] pts = new PointF[4]
                    {
                        new PointF(-1, 1),
                        new PointF(0, 0),
                        new PointF(-1, -1),
                        new PointF(-2, 0)
                    };

                    EndPointVisuals.diamond.AddPolygon(pts);

                    Matrix xform = new Matrix();
                    xform.Scale(6.0f, 4.5f, MatrixOrder.Append);
                    EndPointVisuals.diamond.Transform(xform);
                }

                return EndPointVisuals.diamond;
            }
        }

        [ThreadStaticAttribute]
        private static GraphicsPath openArrow = null;
        [ThreadStaticAttribute]
        private static GraphicsPath closedArrow = null;
        [ThreadStaticAttribute]
        private static GraphicsPath circle = null;
        [ThreadStaticAttribute]
        private static GraphicsPath diamond = null;
    }
}
