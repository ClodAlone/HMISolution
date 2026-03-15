#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartGraph class provides methods for drawing primitives to the chart.
    /// </summary>
    public abstract class ChartGraph
    {
        #region Members
        private Stack<Matrix> m_transformStack = new Stack<Matrix>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        public abstract Matrix Transform
        {
            get;

            set;
        }
        /// <summary>
        /// Gets or sets the SmoothingMode.
        /// </summary>
        /// <value>The SmoothingMode.</value>
         public abstract SmoothingMode SmoothingMode
        {
            get;

            set;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Pushes the transform to the stack.
        /// </summary>
        public void PushTranfsorm()
        {
            m_transformStack.Push(this.Transform);
        }

        /// <summary>
        /// Translates the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Translate(SizeF offset)
        {
            this.Transform.Translate(offset.Width, offset.Height);
        }

        /// <summary>
        /// Multiplies the transform.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void MultiplyTransform(Matrix matrix)
        {
            Matrix transform = this.Transform.Clone();

            transform.Multiply(matrix);

            this.Transform = transform;
        }

        /// <summary>
        /// Pops the transform from the stack.
        /// </summary>
        public void PopTransform()
        {
            this.Transform = m_transformStack.Pop();
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="pt1">The start point.</param>
        /// <param name="pt2">The end point.</param>
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            this.DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="brushInfo">The <see cref="BrushInfo"/>.</param>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawRect(BrushInfo brushInfo, Pen pen, float x, float y, float width, float height)
        {
            using (Brush brush = this.GetBrush(brushInfo, new RectangleF(x, y, width, height)))
            {
                this.DrawRect(brush, pen, x, y, width, height);
            }
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="brushInfo">The <see cref="BrushInfo"/>.</param>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="rect">The rectangle.</param>
        public void DrawRect(BrushInfo brushInfo, Pen pen, RectangleF rect)
        {
            using (Brush brush = this.GetBrush(brushInfo, rect))
            {
                this.DrawRect(brush, pen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
		/// <summary>
		/// Draws the rectangle.
		/// </summary>
		/// <param name="brush">The <see cref="Brush"/>.</param>
		/// <param name="pen">The <see cref="Pen"/>.</param>
		/// <param name="rect">The rectangle.</param>
		public void DrawRect(Brush brush, Pen pen, RectangleF rect)
		{
			this.DrawRect(brush, pen, rect.X, rect.Y, rect.Width, rect.Height);
		}
		/// <summary>
		/// Draws the rectangle.
		/// </summary>
		/// <param name="pen">The <see cref="Pen"/>.</param>
		/// <param name="rect">The rectangle.</param>
		public void DrawRect(Pen pen, RectangleF rect)
		{
			this.DrawRect(null as Brush, pen, rect.X, rect.Y, rect.Width, rect.Height);
		}
        /// <summary>
        /// Draws the ellipse.
        /// </summary>
        /// <param name="brushInfo">The <see cref="BrushInfo"/>.</param>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawEllipse(BrushInfo brushInfo, Pen pen, float x, float y, float width, float height)
        {
            using (Brush brush = this.GetBrush(brushInfo, new RectangleF(x, y, width, height)))
            {
                this.DrawEllipse(brush, pen, x, y, width, height);
            }
        }

        /// <summary>
        /// Draws the specified <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="brushInfo">The <see cref="BrushInfo"/>.</param>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="path">The <see cref="GraphicsPath"/>.</param>
        public void DrawPath(BrushInfo brushInfo, Pen pen, GraphicsPath path)
        {        
            using (Brush brush = this.GetBrush(brushInfo, path.GetBounds()))
            {
                this.DrawPath(brush, pen, path);
            }
        }

        /// <summary>
        /// Draws the specified <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="path">The <see cref="GraphicsPath"/>.</param>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            this.DrawPath(null as Brush, pen, path);
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="rect">The bounds of image.</param>
        public void DrawImage(Image image, RectangleF rect)
        {
            this.DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="brush">The <see cref="Brush"/>.</param>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public abstract void DrawRect(Brush brush, Pen pen, float x, float y, float width, float height);

        /// <summary>
        /// Draws the ellipse.
        /// </summary>
        /// <param name="brush">The <see cref="Brush"/>.</param>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public abstract void DrawEllipse(Brush brush, Pen pen, float x, float y, float width, float height);

        /// <summary>
        /// Draws the path.
        /// </summary>
        /// <param name="brush">The <see cref="Brush"/>.</param>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="path">The <see cref="GraphicsPath"/>.</param>
        public abstract void DrawPath(Brush brush, Pen pen, GraphicsPath path);

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public abstract void DrawLine(Pen pen, float x1, float y1, float x2, float y2);

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="image">The <see cref="Image"/>.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public abstract void DrawImage(Image image, float x, float y, float width, float height);

        /// <summary>
        /// Draws the polyline.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="points">The points.</param>
        public abstract void DrawPolyline(Pen pen, PointF[] points);

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The rect.</param>
        public abstract void DrawString(string text, Font font, Brush brush, RectangleF rect);

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="location">The location.</param>
        /// <param name="stringformat">The stringformat.</param>
        public abstract void DrawString(string text, Font font, Brush brush, PointF location, StringFormat stringformat);

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The rectangle.</param>
        /// <param name="stringformat">The stringformat.</param>
        public abstract void DrawString(string text, Font font, Brush brush, RectangleF rect, StringFormat stringformat);

        /// <summary>
        /// Measures the specified string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <returns>Returns the size of the Text.</returns>
        public abstract SizeF MeasureString(string text, Font font);

        /// <summary>
        /// Measures the specified string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="maxWidth">Maximal width of row.</param>
        /// <returns>Returns the size of the Text.</returns>
        public abstract SizeF MeasureString(string text, Font font, float maxWidth);
		/// <summary>
		/// Measures the specified string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="font">The font.</param>
		/// <param name="maxWidth">Width of the max.</param>
		/// <param name="stringFormat">The string format.</param>
		/// <returns></returns>
		public abstract SizeF MeasureString(string text, Font font, float maxWidth, StringFormat stringFormat);
		/// <summary>
		/// Measures the specified string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="font">The font.</param>
		/// <param name="layoutArea">The layout area.</param>
		/// <param name="stringFormat">The string format.</param>
		/// <returns></returns>
		public abstract SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat);
        #endregion

        #region Helper methods
        /// <summary>
        /// Gets the brush.
        /// </summary>
        /// <param name="brushInfo">The brush info.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns>Returns the Brush.</returns>
        private Brush GetBrush(BrushInfo brushInfo, RectangleF bounds)
        {
            Brush result = null;

            if (!bounds.IsEmpty)
            {
                switch (brushInfo.Style)
                {
                    case BrushStyle.Gradient:
                        if (brushInfo.GradientStyle != GradientStyle.None)
                        {
                            #region Gradient brush
                            switch (brushInfo.GradientStyle)
                            {
                                case GradientStyle.BackwardDiagonal:
                                    {
                                        LinearGradientBrush lgb = new LinearGradientBrush(bounds,
                                            Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal);

                                        lgb.WrapMode = WrapMode.TileFlipXY;
                                        lgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);

                                        result = lgb;
                                    }

                                    break;
                                case GradientStyle.ForwardDiagonal:
                                    {
                                        LinearGradientBrush lgb = new LinearGradientBrush(bounds,
                                            Color.Empty, Color.Empty, LinearGradientMode.ForwardDiagonal);

                                        lgb.WrapMode = WrapMode.TileFlipXY;
                                        lgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);

                                        result = lgb;
                                    }

                                    break;

                                case GradientStyle.Horizontal:
                                    {
                                        LinearGradientBrush lgb = new LinearGradientBrush(bounds,
                                            Color.Empty, Color.Empty, LinearGradientMode.Horizontal);

                                        lgb.WrapMode = WrapMode.TileFlipXY;
                                        lgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);

                                        result = lgb;
                                    }

                                    break;

                                case GradientStyle.PathEllipse:
                                    {
                                        GraphicsPath gp = new GraphicsPath();
                                        gp.AddEllipse(RectangleF.Inflate(bounds, 0.25f * bounds.Width, 0.25f * bounds.Height));

                                        PathGradientBrush pgb = new PathGradientBrush(gp);
                                        pgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);
                                        result = pgb;
                                    }

                                    break;

                                case GradientStyle.PathRectangle:
                                    {
                                        GraphicsPath gp = new GraphicsPath();
                                        gp.AddRectangle(bounds);

                                        PathGradientBrush pgb = new PathGradientBrush(gp);
                                        pgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);
                                        result = pgb;
                                    }

                                    break;

                                case GradientStyle.Vertical:
                                    {
                                        LinearGradientBrush lgb = new LinearGradientBrush(bounds,
                                            Color.Empty, Color.Empty, LinearGradientMode.Vertical);

                                        lgb.WrapMode = WrapMode.TileFlipXY;
                                        lgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);

                                        result = lgb;
                                    }

                                    break;
                            }

                            #endregion
                        }
                        else
                        {
                            result = new SolidBrush(brushInfo.BackColor);
                        }

                        break;

                    case BrushStyle.Pattern:
                        if (brushInfo.PatternStyle != PatternStyle.None)
                        {
                            result = new HatchBrush((HatchStyle)(brushInfo.PatternStyle - 1),
                                brushInfo.ForeColor, brushInfo.BackColor);
                        }
                        else
                        {
                            result = new SolidBrush(brushInfo.BackColor);
                        }
                        break;

                    case BrushStyle.Solid:
                        result = new SolidBrush(brushInfo.BackColor);
                        break;
                }
            }

            return result;
        }

        #region GetBrushItem
        /// <summary>
        /// Gets the brush item.
        /// </summary>
        /// <param name="brushInfo">The brush info.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        public static Brush GetBrushItem(BrushInfo brushInfo, RectangleF bounds)
        {
            Brush result = null;

            if (!bounds.IsEmpty)
            {
                switch (brushInfo.Style)
                {
                    case BrushStyle.Gradient:
                        if (brushInfo.GradientStyle != GradientStyle.None)
                        {
                            #region Gradient brush
                            switch (brushInfo.GradientStyle)
                            {
                                case GradientStyle.BackwardDiagonal:
                                    {
                                        LinearGradientBrush lgb = new LinearGradientBrush(bounds,
                                            Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal);

                                        lgb.WrapMode = WrapMode.TileFlipXY;
                                        lgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);

                                        result = lgb;
                                    }

                                    break;
                                case GradientStyle.ForwardDiagonal:
                                    {
                                        LinearGradientBrush lgb = new LinearGradientBrush(bounds,
                                            Color.Empty, Color.Empty, LinearGradientMode.ForwardDiagonal);

                                        lgb.WrapMode = WrapMode.TileFlipXY;
                                        lgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);

                                        result = lgb;
                                    }

                                    break;

                                case GradientStyle.Horizontal:
                                    {
                                        LinearGradientBrush lgb = new LinearGradientBrush(bounds,
                                            Color.Empty, Color.Empty, LinearGradientMode.Horizontal);

                                        lgb.WrapMode = WrapMode.TileFlipXY;
                                        lgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);

                                        result = lgb;
                                    }

                                    break;

                                case GradientStyle.PathEllipse:
                                    {
                                        GraphicsPath gp = new GraphicsPath();
                                        gp.AddEllipse(RectangleF.Inflate(bounds, 0.25f * bounds.Width, 0.25f * bounds.Height));

                                        PathGradientBrush pgb = new PathGradientBrush(gp);
                                        pgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);
                                        result = pgb;
                                    }

                                    break;

                                case GradientStyle.PathRectangle:
                                    {
                                        GraphicsPath gp = new GraphicsPath();
                                        gp.AddRectangle(bounds);

                                        PathGradientBrush pgb = new PathGradientBrush(gp);
                                        pgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);
                                        result = pgb;
                                    }

                                    break;

                                case GradientStyle.Vertical:
                                    {
                                        LinearGradientBrush lgb = new LinearGradientBrush(bounds,
                                            Color.Empty, Color.Empty, LinearGradientMode.Vertical);

                                        lgb.WrapMode = WrapMode.TileFlipXY;
                                        lgb.InterpolationColors = GetGenericColorBlend(brushInfo.GradientColors);

                                        result = lgb;
                                    }

                                    break;
                            }

                            #endregion
                        }
                        else
                        {
                            result = new SolidBrush(brushInfo.BackColor);
                        }

                        break;

                    case BrushStyle.Pattern:
                        if (brushInfo.PatternStyle != PatternStyle.None)
                        {
                            result = new HatchBrush((HatchStyle)(brushInfo.PatternStyle - 1),
                                brushInfo.ForeColor, brushInfo.BackColor);
                        }
                        else
                        {
                            result = new SolidBrush(brushInfo.BackColor);
                        }
                        break;

                    case BrushStyle.Solid:
                        result = new SolidBrush(brushInfo.BackColor);
                        break;
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Gets the generic color blend.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <returns>Returns the ColorBlend.</returns>
        private static ColorBlend GetGenericColorBlend(BrushInfoColorArrayList colors)
        {
            ColorBlend blend = new ColorBlend(colors.Count);

            float position = 0f;
            float step = 1f / (colors.Count - 1);

            for (int i = 0, ci = colors.Count; i < ci; i++)
            {
                blend.Positions[i] = position;
                blend.Colors[i] = colors[ci - i - 1];
                position += 1f / ci;
            }

            blend.Positions[blend.Positions.Length - 1] = 1f;

            return blend;
        }
        #endregion
    }

    /// <summary>
    /// The ChartGDIGraph.
    /// </summary>
    class ChartGDIGraph : ChartGraph
    {
        #region Members
        private Graphics m_g;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public Graphics Graphics
        {
            get
            {
                return m_g;
            }
        }

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        public override Matrix Transform
        {
            get
            {
                return m_g.Transform;
            }

            set
            {
                m_g.Transform = value;
            }
        }
        /// <summary>
        /// Gets or sets the SmoothingMode.
        /// </summary>
        /// <value>The SmoothingMode.</value>
        public override SmoothingMode  SmoothingMode
        {
            get
            {
                return m_g.SmoothingMode;
            }

            set
            {
                m_g.SmoothingMode = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartGDIGraph"/> class.
        /// </summary>
        /// <param name="g">The g.</param>
        public ChartGDIGraph(Graphics g)
        {
            m_g = g;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws the rect.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public override void DrawRect(Brush brush, Pen pen, float x, float y, float width, float height)
        {
            if (brush != null && height > 1 && width > 1)
            {
                m_g.FillRectangle(brush, x, y, width, height);
            }

            if (pen != null)
            {
                m_g.DrawRectangle(pen, x, y, width, height);
            }
        }

        /// <summary>
        /// Draws the ellipse.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public override void DrawEllipse(Brush brush, Pen pen, float x, float y, float width, float height)
        {
            if (brush != null)
            {
                m_g.FillEllipse(brush, x, y, width, height);
            }

            if (pen != null)
            {
                m_g.DrawEllipse(pen, x, y, width, height);
            }
        }

        /// <summary>
        /// Draws the path.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="gp">The gp.</param>
        public override void DrawPath(Brush brush, Pen pen, GraphicsPath gp)
        {
		try{
            if (brush != null)
            {
                m_g.FillPath(brush, gp);
            }

            if (pen != null)
            {
                m_g.DrawPath(pen, gp);
            }
			}
			catch {}
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            m_g.DrawLine(pen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public override void DrawImage(Image image, float x, float y, float width, float height)
        {
            m_g.DrawImage(image, x, y, width, height);
        }

        /// <summary>
        /// Draws the polyline.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        public override void DrawPolyline(Pen pen, PointF[] points)
        {
            m_g.DrawLines(pen, points);
        }

        /// <summary>
        /// Measures the specified string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <returns></returns>
        public override SizeF MeasureString(string text, Font font)
        {
            return m_g.MeasureString(text, font);
        }

        /// <summary>
        /// Measures the specified string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="maxWidth">Maximal width of row.</param>
        /// <returns></returns>
        public override SizeF MeasureString(string text, Font font, float maxWidth)
        {
			return m_g.MeasureString(text, font, (int)Math.Ceiling(maxWidth));
		}
		/// <summary>
		/// Measures the specified string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="font">The font.</param>
		/// <param name="maxWidth">Maximal width of row.</param>
		/// <param name="stringFormat">StringFormat instance.</param>
		/// <returns></returns>
		public override SizeF MeasureString(string text, Font font, float maxWidth, StringFormat stringFormat)
		{
			return m_g.MeasureString(text, font, (int)Math.Ceiling(maxWidth), stringFormat);
		}
		/// <summary>
		/// Measures the specified string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="font">The font.</param>
        /// <param name="layoutArea">Maximal width of row.</param>
		/// <param name="stringFormat">StringFormat instance.</param>
		/// <returns></returns>
		public override SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			return m_g.MeasureString(text, font, layoutArea, stringFormat);
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The rect.</param>
        public override void DrawString(string text, Font font, Brush brush, RectangleF rect)
        {
            m_g.DrawString(text, font, brush, rect);
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="location">The location.</param>
        /// <param name="stringformat">The stringformat.</param>
        public override void DrawString(string text, Font font, Brush brush, PointF location, StringFormat stringformat)
        {
            m_g.DrawString(text, font, brush, location, stringformat);
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The rect.</param>
        /// <param name="stringformat">The stringformat.</param>
        public override void DrawString(string text, Font font, Brush brush, RectangleF rect, StringFormat stringformat)
        {
            m_g.DrawString(text, font, brush, rect, stringformat);
        }
        #endregion
    }
}
