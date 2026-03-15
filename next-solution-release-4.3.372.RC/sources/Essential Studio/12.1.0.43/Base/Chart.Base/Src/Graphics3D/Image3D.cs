#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the <see cref="Image"/> in the 3D.
    /// </summary>
    public sealed class Image3D : Polygon
    {
        #region Members
        private Image m_image = null;
        private ImageAttributes m_attributes = null;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public ImageAttributes Attributes
        {
            get { return m_attributes; }
            set { m_attributes = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Image3D"/> class.
        /// </summary>
        /// <param name="vs">The positions of polygon.</param>
        /// <param name="img">The image.</param>
        public Image3D(Vector3D[] vs, Image img)
            : base(vs)
        {
            m_image = img;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draws to the specified <see cref="Graphics3D"/>.
        /// </summary>
        /// <param name="g3d">The g3d.</param>
        /// <returns>Returns ChartRegion.</returns>
        public override ChartRegion Draw(Graphics3D g3d)
        {
            Transform3D transform = g3d.Transform;
            PointF[] points = new PointF[m_points.Length];

            for (int i = 0; i < m_points.Length; i++)
            {
                points[i] = transform.ToScreen(m_points[i]);
            }

            if (m_attributes == null)
            {
                g3d.Graphics.DrawImage(m_image, new PointF[] { points[0], points[1], points[2] });
            }
            else
            {
                g3d.Graphics.DrawImage(m_image, new PointF[] { points[0], points[1], points[2] },
                    new Rectangle(0, 0, m_image.Width, m_image.Height), g3d.Graphics.PageUnit, m_attributes);
            }

            ChartRegion res = null;

            if (m_dataRegion != null)
            {
                PointF px = new PointF(points[1].X + points[2].X - points[0].X,
                    points[1].Y + points[2].Y - points[0].Y);

                GraphicsPath gp = new GraphicsPath();
                gp.AddLines(new PointF[] { points[0], points[1], px, points[2] });
                gp.CloseFigure();

                res = m_dataRegion.GetChartRegion(new Region(gp));
            }

            return res;
        }

        /// <summary>
        /// Create the new instance from the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="z">The depth coordinate.</param>
        /// <returns>Returns Image3D.</returns>
        public static Image3D FromImage(Image image, RectangleF bounds, float z)
        {
            Vector3D[] vs = new Vector3D[3];

            vs[0] = new Vector3D(bounds.Left, bounds.Top, z);
            vs[1] = new Vector3D(bounds.Right, bounds.Top, z);
            vs[2] = new Vector3D(bounds.Left, bounds.Bottom, z);

            return new Image3D(vs, image);
        }

        /// <summary>
        /// Create the new instance and copy all members.
        /// </summary>
        /// <returns>Returns Polygon after clone.</returns>
        public override Polygon Clone()
        {
            return new Image3D(this.Points.Clone() as Vector3D[], m_image);
        }
        #endregion
    }
}
