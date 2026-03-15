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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the simple 3D polygon.
    /// </summary>
    public class Polygon : Plane3D
    {
        #region Members
        /// <summary>
        /// Points of polygon.
        /// </summary>
        protected Vector3D[] m_points;
        /// <summary>
        /// The <see cref="Pen"/> for border drawing.
        /// </summary>
        protected Pen m_pen = null;
        /// <summary>
        /// The <see cref="Pen"/> for border drawing.
        /// </summary>
        protected Pen m_FigurePen = null;

        /// <summary>
        /// The <see cref="Brush"/> for polygon filling.
        /// </summary>
        protected Brush m_brush = null;

        /// <summary>
        /// The <see cref="BrushInfo"/> for polygon filling.
        /// </summary>
        protected BrushInfo m_brInfo = null;

        /// <summary>
        /// Indicates whether this polygon is used as clip plane.
        /// </summary>
        protected bool isClipPolygon = false;

        /// <summary>
        /// The data of result regions.
        /// </summary>
        protected ChartRegionData m_dataRegion = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the points of polygon.
        /// </summary>
        /// <value>The points.</value>
        public virtual Vector3D[] Points
        {
            get
            {
                return m_points;
            }
        }

        /// <summary>
        /// Gets the brush.
        /// </summary>
        /// <value>The brush.</value>
        public Brush Brush
        {
            get
            {
                return m_brush;
            }
        }

        /// <summary>
        /// Gets the pen.
        /// </summary>
        /// <value>The pen.</value>
        public Pen Pen
        {
            get
            {
                return m_pen;
            }
        }

        /// <summary>
        /// Gets the brush info.
        /// </summary>
        /// <value>The brush info.</value>
        public BrushInfo BrushInfo
        {
            get
            {
                return m_brInfo;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether polygon is used as clip plane.
        /// </summary>
        /// <value><c>true</c> if it's used as clip plane; otherwise, <c>false</c>.</value>
        public bool ClipPolygon
        {
            get
            {
                return isClipPolygon;
            }

            set
            {
                if (isClipPolygon != value)
                {
                    isClipPolygon = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the region data.
        /// </summary>
        /// <value>The region data.</value>
        public ChartRegionData RegionData
        {
            get
            {
                return m_dataRegion;
            }

            set
            {
                m_dataRegion = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public Polygon(Vector3D[] points)
            : base(points[0], points[1], points[2])
        {
            m_points = points;

            this.CalcNormal();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="br">The br.</param>
        public Polygon(Vector3D[] points, Brush br)
            : this(points)
        {
            m_brush = br;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="polygon">The PLG.</param>
        public Polygon(Vector3D[] points, Polygon polygon)
            : this(points)
        {
            m_brush = polygon.Brush;
            m_brInfo = polygon.BrushInfo;
            m_pen = polygon.Pen;
            m_FigurePen = polygon.m_FigurePen;
            m_dataRegion = polygon.RegionData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="br">The br.</param>
        /// <param name="pen">The pen.</param>
        public Polygon(Vector3D[] points, Brush br, Pen pen)
            : this(points)
        {
            m_brush = br;
            m_pen = pen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="pen">The pen.</param>
        public Polygon(Vector3D[] points, Pen pen)
            : this(points)
        {
            m_pen = pen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="br">The br.</param>
        public Polygon(Vector3D[] points, BrushInfo br)
            : this(points)
        {
            m_brInfo = br;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="br">The br.</param>
        /// <param name="pen">The pen.</param>
        public Polygon(Vector3D[] points, BrushInfo br, Pen pen)
            : this(points)
        {
            m_brInfo = br;
            m_pen = pen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="br">The br.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="IsPNF">The PNF.</param>
        public Polygon(Vector3D[] points, BrushInfo br, Pen pen, bool IsPNF)
            : this(points)
        {
            m_brInfo = br;
            if (IsPNF)
                m_FigurePen = pen;
            else
                m_pen = pen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <param name="d">The d.</param>
        public Polygon(Vector3D normal, double d)
            : base(normal, d)
        {
            m_points = null;
            m_brInfo = null;
            m_pen = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="clipPolygon">if set to <c>true</c> [clip polygon].</param>
        public Polygon(Vector3D[] points, bool clipPolygon)
            : this(points)
        {
            m_brInfo = null;
            //here pen should be null
            m_pen = new Pen(Color.White);
            isClipPolygon = clipPolygon;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="poly">The poly.</param>
        public Polygon(Polygon poly)
            : base(poly.Points[0], poly.Points[1], poly.Points[2])
        {
            m_points = new Vector3D[poly.Points.Length];

            for (int i = 0; i < poly.Points.Length; i++)
            {
                Vector3D tv = poly.Points[i];

                m_points[i] = new Vector3D(tv.X, tv.Y, tv.Z);
            }

            this.CalcNormal();

            m_brush = poly.Brush;
            m_brInfo = poly.BrushInfo;
            m_pen = poly.Pen;
            m_FigurePen = poly.m_FigurePen;
            m_dataRegion = poly.RegionData;
            isClipPolygon = poly.isClipPolygon;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Polygon"/> is reclaimed by garbage collection.
        /// </summary>
        ~Polygon()
        {
            m_brInfo = null;
            m_brush = null;
            m_pen = null;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates the polygon by specified rectangle.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <param name="brushInfo">The brush info.</param>
        /// <param name="pen">The pen.</param>
        /// <returns>Returns Polygon.</returns>
        internal static Polygon CreateRectangle(RectangleF bounds, double z, BrushInfo brushInfo, Pen pen)
        {
            Vector3D[] vs = new Vector3D[]{
        new Vector3D( bounds.Left, bounds.Top, z ),
        new Vector3D( bounds.Right, bounds.Top, z ),
        new Vector3D( bounds.Right, bounds.Bottom, z ),
        new Vector3D( bounds.Left, bounds.Bottom, z ) };

            return new Polygon(vs, brushInfo, pen);
        }

        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns>Returns Vector3D instance.</returns>
        internal virtual Vector3D GetNormal(Matrix3D transform)
        {
            Vector3D norm = new Vector3D(double.NaN, double.NaN, double.NaN);

            if (m_points != null)
            {
                norm = ChartMath.GetNormal(transform * m_points[0],
                    transform * m_points[1], transform * m_points[2]);


                for (int i = 3; (i < m_points.Length) && !norm.IsValid; i++)
                {
                    Vector3D v1 = transform * m_points[i];
                    Vector3D v2 = transform * m_points[0];
                    Vector3D v3 = transform * m_points[i / 2];

                    norm = ChartMath.GetNormal(v1, v2, v3);
                }
            }
            else
            {
                norm = transform & m_normal;
                norm.Normalize();
            }

            return norm;
        }

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D"/>.
        /// </summary>
        /// <param name="g3d">The g3d.</param>
        /// <returns>Return ChartRegion.</returns>
        public virtual ChartRegion Draw(Graphics3D g3d)
        {
            ChartRegion region = null;

            if (m_points != null && m_points.Length > 0 && !isClipPolygon)
            {
                Transform3D transform = g3d.Transform;
                PointF[] points = new PointF[m_points.Length];
                GraphicsPath gp = new GraphicsPath(FillMode.Winding);

                for (int i = 0; i < m_points.Length; i++)
                {
                    points[i] = transform.ToScreen(m_points[i]);
                }

                gp.AddPolygon(points);

                if (g3d.Light)
                {
                    Vector3D ln = !g3d.LightPosition;
                    ln.Normalize();
                    int lcoef = (int)(g3d.LightCoeficient * (2 * Math.Abs(m_normal & ln) - 1));

                    if (m_brInfo != null)
                    {
                        FillPolygon(g3d.Graphics, m_brInfo, gp, lcoef);
                    }

                    if (m_brush != null)
                    {
                        FillPolygon(g3d.Graphics, m_brush, gp, lcoef);
                    }

                    if (m_pen != null)
                    {
                        DrawPolygon(g3d.Graphics, m_pen, gp, lcoef);
                    }
                    if (m_FigurePen != null)
                    {
                        g3d.Graphics.DrawLine(m_FigurePen, points[0], points[2]);
                        g3d.Graphics.DrawLine(m_FigurePen, points[1], points[3]);
                    }
                }
                else
                {
                    if (m_brInfo != null)
                    {
                        BrushPaint.FillPath(g3d.Graphics, gp, m_brInfo);
                    }

                    if (m_brush != null)
                    {
                        g3d.Graphics.FillPolygon(m_brush, points);
                    }

                    if (m_pen != null)
                    {
                        g3d.Graphics.DrawPolygon(m_pen, points);
                    }
                    if (m_FigurePen != null)
                    {
                        g3d.Graphics.DrawLine(m_FigurePen, points[0], points[2]);
                        g3d.Graphics.DrawLine(m_FigurePen, points[1], points[3]);
                    }
                }

                if (m_dataRegion != null)
                {
                    region = m_dataRegion.GetChartRegion(new Region(gp));
                }
            }

            return region;
        }

        /// <summary>
        /// Transforms by the specified <see cref="Matrix3D"/>.
        /// </summary>
        /// <param name="matrix3D">The <see cref="Matrix3D"/>.</param>
        public override void Transform(Matrix3D matrix3D)
        {
            if (Points != null)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i] = matrix3D * Points[i];
                }

                CalcNormal();
            }
            else
            {
                base.Transform(matrix3D);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns Polygon.</returns>
        public virtual Polygon Clone()
        {
            return new Polygon(this);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Calculates the normal.
        /// </summary>
        protected void CalcNormal()
        {
            CalcNormal(Points[0], Points[1], Points[2]);

            for (int i = 3; (i < Points.Length) && (Test()); i++)
            {
                CalcNormal(Points[i], Points[0], Points[i / 2]);
            }
        }

        /// <summary>
        /// Draws the polygon.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="gp">The gp.</param>
        /// <param name="coef">The coefficient.</param>
        protected void DrawPolygon(Graphics g, Pen pen, GraphicsPath gp, int coef)
        {
            using (Pen pn = (Pen)pen.Clone())
            {
                pn.Color = LigthColor(pn.Color, coef);
                g.DrawPath(pn, gp);
            }
        }

        /// <summary>
        /// Fills the polygon.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="br">The br.</param>
        /// <param name="gp">The gp.</param>
        /// <param name="coef">The coefficient.</param>
        protected void FillPolygon(Graphics g, Brush br, GraphicsPath gp, int coef)
        {
            if (br is SolidBrush)
            {
                using (SolidBrush sbr = br.Clone() as SolidBrush)
                {
                    sbr.Color = LigthColor(sbr.Color, coef);
                    g.FillPath(sbr, gp);
                }
            }
            else if (br is HatchBrush)
            {
                using (HatchBrush hbr = new HatchBrush((br as HatchBrush).HatchStyle,
                                 LigthColor((br as HatchBrush).ForegroundColor, coef),
                                 LigthColor((br as HatchBrush).BackgroundColor, coef)))
                {
                    g.FillPath(hbr, gp);
                }
            }
            else if (br is LinearGradientBrush)
            {
                using (LinearGradientBrush lbr = br as LinearGradientBrush)
                {
                    ColorBlend cb = null;

                    try
                    {
                        cb = lbr.InterpolationColors;

                        for (int i = 0, l = cb.Colors.Length; i < l; i++)
                        {
                            cb.Colors[i] = LigthColor(cb.Colors[i], coef);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace, "Exception");

                        for (int i = 0, l = lbr.LinearColors.Length; i < l; i++)
                        {
                            lbr.LinearColors[i] = LigthColor(lbr.LinearColors[i], coef);
                        }
                    }

                    g.FillPath(lbr, gp);
                }
            }
            else if (br is PathGradientBrush)
            {
                using (PathGradientBrush pbr = br.Clone() as PathGradientBrush)
                {
                    ColorBlend cb = null;

                    try
                    {
                        cb = pbr.InterpolationColors;

                        for (int i = 0, l = cb.Colors.Length; i < l; i++)
                        {
                            cb.Colors[i] = LigthColor(cb.Colors[i], coef);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace, "Exception");

                        for (int i = 0, l = pbr.SurroundColors.Length; i < l; i++)
                        {
                            pbr.SurroundColors[i] = LigthColor(pbr.SurroundColors[i], coef);
                        }
                    }

                    g.FillPath(pbr, gp);
                }
            }
        }

        /// <summary>
        /// Fills the polygon.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="brInfo">The br info.</param>
        /// <param name="gp">The gp.</param>
        /// <param name="coef">The coefficient.</param>
        protected void FillPolygon(Graphics g, BrushInfo brInfo, GraphicsPath gp, int coef)
        {
            BrushPaint.FillPath(g, gp, DrawingHelper.AddColor(brInfo, coef));
        }

        /// <summary>
        /// Lights the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="coef">The coefficient.</param>
        /// <returns>Returns the light color.</returns>
        protected Color LigthColor(Color color, int coef)
        {
            int r = ChartMath.MinMax(color.R + coef, 0, byte.MaxValue);
            int g = ChartMath.MinMax(color.G + coef, 0, byte.MaxValue);
            int b = ChartMath.MinMax(color.B + coef, 0, byte.MaxValue);

            return Color.FromArgb(color.A, r, g, b);
        }
        #endregion
    }
}
