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
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the label positioned in the 3D.
    /// </summary>
    public sealed class Pseudo3DText : Polygon
    {
        #region Members
        private Font m_font;
        private string m_text;
        private Matrix m_matrix = null;
        private ContentAlignment m_alignment = ContentAlignment.BottomRight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <value>The font.</value>
        public Font Font
        {
            get
            {
                return m_font;
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return m_text;
            }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Vector3D Location
        {
            get
            {
                return m_points[0];
            }
        }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public ContentAlignment Alignment
        {
            get
            {
                return m_alignment;
            }

            set
            {
                m_alignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the matrix.
        /// </summary>
        /// <value>The matrix.</value>
        public Matrix Matrix
        {
            get
            {
                return m_matrix;
            }

            set
            {
                m_matrix = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Pseudo3DText"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="br">The br.</param>
        /// <param name="loc">The loc.</param>
        public Pseudo3DText(string text, Font font, Brush br, Vector3D loc)
            : base(new Vector3D[] { loc, loc + new Vector3D(1, 0, 0), loc + new Vector3D(0, 1, 0) })
        {
            m_text = text;
            m_font = font;
            m_brush = br;
            m_points = new Vector3D[] { loc, loc + new Vector3D(1, 0, 0), loc + new Vector3D(0, 1, 0) };
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draws to the specified <see cref="Graphics3D"/>.
        /// </summary>
        /// <param name="g3d">The <see cref="Graphics3D"/>.</param>
        /// <returns>Returns ChartRegion.</returns>
        public override ChartRegion Draw(Graphics3D g3d)
        {
            Transform3D transform = g3d.Transform;
            PointF[] points = new PointF[m_points.Length];

            for (int i = 0; i < m_points.Length; i++)
            {
                points[i] = transform.ToScreen(m_points[i]);
            }

            PointF loc = points[0];

            if (m_brush != null)
            {
                SizeF sz = g3d.Graphics.MeasureString(m_text, m_font);

                switch (m_alignment)
                {
                    case ContentAlignment.BottomCenter:
                        loc = new PointF(loc.X - sz.Width / 2, loc.Y);
                        break;
                    case ContentAlignment.BottomLeft:
                        loc = new PointF(loc.X - sz.Width, loc.Y);
                        break;
                    case ContentAlignment.BottomRight:
                        loc = new PointF(loc.X, loc.Y);
                        break;

                    case ContentAlignment.MiddleCenter:
                        loc = new PointF(loc.X - sz.Width / 2, loc.Y - sz.Height / 2);
                        break;
                    case ContentAlignment.MiddleLeft:
                        loc = new PointF(loc.X - sz.Width, loc.Y - sz.Height / 2);
                        break;
                    case ContentAlignment.MiddleRight:
                        loc = new PointF(loc.X, loc.Y - sz.Height / 2);
                        break;

                    case ContentAlignment.TopCenter:
                        loc = new PointF(loc.X - sz.Width / 2, loc.Y - sz.Height);
                        break;
                    case ContentAlignment.TopLeft:
                        loc = new PointF(loc.X - sz.Width, loc.Y - sz.Height);
                        break;
                    case ContentAlignment.TopRight:
                        loc = new PointF(loc.X, loc.Y - sz.Height);
                        break;
                }

                GraphicsContainer gc = DrawingHelper.BeginTransform(g3d.Graphics);

                if (m_matrix != null)
                {
                    g3d.Graphics.Transform = m_matrix;
                }

                g3d.Graphics.DrawString(m_text, m_font, m_brush, loc);

                DrawingHelper.EndTransform(g3d.Graphics, gc);
            }

            return null;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Return polygon.</returns>
        public override Polygon Clone()
        {
            Pseudo3DText res = new Pseudo3DText(Text, Font, Brush, Location);

            res.Alignment = Alignment;

            return res;
        }
        #endregion
    }
}
