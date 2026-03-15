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
using Syncfusion.Diagnostics;

namespace Syncfusion.Drawing
{
	/// <summary>
	/// Paints window background using <see cref="BrushInfo"/> information.
	/// </summary>
	public sealed class BrushPaint
	{
		/// <summary>
		/// Overloaded. Fills the interior of a rectangle using <see cref="BrushInfo"/> information.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="Rectangle"/> structure that represents the rectangle to fill. </param>
		/// <param name="brush"><see cref="BrushInfo"/> object that determines the characteristics of the fill.</param>
		public static void FillRectangle(Graphics g, Rectangle r, BrushInfo brush)
		{
			FillRectangle(g, (RectangleF)r, brush);
		}
		/// <summary>
		/// Fills the interior of a rectangle using <see cref="BrushInfo"/> information.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="RectangleF"/> Structure that represents the rectangle to fill. </param>
		/// <param name="brush"><see cref="BrushInfo"/> Object that determines the characteristics of the fill.</param>
		public static void FillRectangle(Graphics g, RectangleF r, BrushInfo brush)
		{
			if (r.Height == 0 || r.Width == 0)
				return;

			switch (brush.Style)
			{
				case BrushStyle.None:
					g.FillRectangle(SystemBrushes.Window, r);
					break;

				case BrushStyle.Solid:
					//g.FillRectangle(brush.BackColorBrush, r);
					FillRectangle(g, r, brush.BackColor);
					break;

				case BrushStyle.Pattern:
					if (brush.PatternStyle == PatternStyle.None)
						goto case BrushStyle.Solid;
					//g.FillRectangle(brush.PatternedBrush, r);
					FillRectangle(g, r, brush.PatternStyle, brush.ForeColor, brush.BackColor);
					break;

				case BrushStyle.Gradient:
					if (brush.GradientStyle == GradientStyle.None)
						goto case BrushStyle.Solid;
					FillRectangle(g, r, brush.GradientStyle, (Color[])brush.GradientColors.ToArray(typeof(Color)));
					break;
			}
		}

		/// <summary>
		/// Fills the interior of a rectangle with a gradient.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="Rectangle"/> Structure that represents the rectangle to fill. </param>
		/// <param name="gradientStyle"><see cref="GradientStyle"/>.</param>
		/// <param name="foreColor">A <see cref="Color"/> used for the gradient fill.</param>
		/// <param name="backColor">A <see cref="Color"/> used for the gradient fill.</param>
		public static void FillRectangle(Graphics g, Rectangle r, GradientStyle gradientStyle, Color foreColor, Color backColor)
		{
			FillRectangle(g, r, gradientStyle, new Color[]{foreColor, backColor});
		}
		/// <summary>
		/// Fills the interior of a rectangle with a gradient.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context</param>
		/// <param name="r"><see cref="RectangleF"/> Structure that represents the rectangle to fill. </param>
		/// <param name="gradientStyle"><see cref="GradientStyle"/>.</param>
		/// <param name="foreColor">A <see cref="Color"/> used for the gradient fill.</param>
		/// <param name="backColor">A <see cref="Color"/> used for the gradient fill.</param>
		public static void FillRectangle(Graphics g, RectangleF r, GradientStyle gradientStyle, Color foreColor, Color backColor)
		{
			FillRectangle(g, r, gradientStyle, new Color[]{foreColor, backColor});
		}
		private static ColorBlend GetGenericColorBlend(Color[] colors)
		{
			ColorBlend blend = new ColorBlend(colors.Length);
			Array.Reverse(colors);
			blend.Colors = colors;
			float[] positions = new float[colors.Length];

			positions[0] = 0f;
			float position = 0f;
			for(int i = 1; i < colors.Length - 1; i++)
			{
				position += 1f/(colors.Length - 1f);
				positions[i] = position;
			}
			positions[positions.Length - 1] = 1f;
			blend.Positions = positions;
			return blend;
		}
		/// <summary>
		/// Fills the interior of a rectangle with a gradient.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context</param>
		/// <param name="r"><see cref="RectangleF"/> Structure that represents the rectangle to fill. </param>
		/// <param name="gradientStyle"><see cref="GradientStyle"/>.</param>
		/// <param name="colors">An array of <see cref="Color"/> used for the gradient fill.</param>
		public static void FillRectangle(Graphics g, Rectangle r, GradientStyle gradientStyle, Color[] colors)
		{
			FillRectangle(g, (RectangleF)r, gradientStyle, colors);
		}
		/// <summary>
		/// Fills the interior of a rectangle with a gradient.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context</param>
		/// <param name="r"><see cref="RectangleF"/> Structure that represents the rectangle to fill. </param>
		/// <param name="gradientStyle"><see cref="GradientStyle"/>.</param>
		/// <param name="colors">An array of <see cref="Color"/> used for the gradient fill.</param>
		public static void FillRectangle(Graphics g, RectangleF r, GradientStyle gradientStyle, Color[] colors)
		{
			Trace.Assert(colors.Length >= 2, "The colors arrayed in the call to FillRectangle should have at least 2 entries.");
#if DEBUG
			if (Switches.BrushPaint.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(r, gradientStyle, colors);
#endif                      

			if (r.Height == 0 || r.Width == 0)
				return;

			Brush br = null;
			PathGradientBrush pbr = null;
			LinearGradientBrush lgbr = null;
			GraphicsPath path = null;
			RectangleF clip = RectangleF.Empty;

			switch (gradientStyle)
			{
				case GradientStyle.ForwardDiagonal:
					lgbr = new LinearGradientBrush(r, Color.Empty, Color.Empty, LinearGradientMode.ForwardDiagonal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					break;

				case GradientStyle.BackwardDiagonal:
					lgbr = new LinearGradientBrush(r, Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					break;

				case GradientStyle.Horizontal:
					lgbr = new LinearGradientBrush(r, Color.Empty, Color.Empty, LinearGradientMode.Horizontal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					break;

				case GradientStyle.Vertical:
					lgbr = new LinearGradientBrush(r, Color.Empty, Color.Empty, LinearGradientMode.Vertical);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					break;

				case GradientStyle.PathRectangle:
					path = new GraphicsPath();
					path.AddRectangle(r);
					break;

				case GradientStyle.PathEllipse:
					path = new GraphicsPath();
					clip = g.ClipBounds;
					g.IntersectClip(r);
					r.Inflate(r.Width/4, r.Height/4);
					path.AddEllipse(r);
					break;

					//				case GradientStyle.PathPie:
					//					path = new GraphicsPath();
					//					path.AddPie(r, -30, 200);
					//					break;
			}

			if( path != null && path.PointCount > 0 )
			{
				pbr = new PathGradientBrush(path);
				pbr.CenterColor = colors[colors.Length - 1];

                Color[] scolors;
                if( path.PointCount < colors.Length - 1 )
                {
                    scolors = new Color[ path.PointCount ];
                }
                else
                {
                    scolors = new Color[ colors.Length - 1 ];
                }

				int j = 0;
				for(int i = colors.Length - 2; i >= 0 && j<path.PointCount; i--)
					scolors[j++] = colors[i];
				pbr.SurroundColors = scolors;
				//pbr.CenterPoint = new Point(r.X, r.Y+r.Height/2);
				br = pbr;
                pbr = null;
			}

			if (br != null)
			{
				g.FillRectangle(br, r);

				if (path != null)
					path.Dispose();

				br.Dispose();
				br = null;
			}

			if (!clip.IsEmpty)
				g.SetClip(clip);
		}

		/// <summary>
		/// Fills the interior of a rectangle with a pattern.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context</param>
		/// <param name="r"><see cref="Rectangle"/> Structure that represents the rectangle to fill. </param>
		/// <param name="hatchStyle"><see cref="PatternStyle"/>.</param>
		/// <param name="foreColor">A <see cref="Color"/> used for the pattern fill.</param>
		/// <param name="backColor">A <see cref="Color"/> used for the pattern fill.</param>
		public static void FillRectangle(Graphics g, Rectangle r, PatternStyle hatchStyle, Color foreColor, Color backColor)
		{
			FillRectangle(g, (RectangleF)r, hatchStyle, foreColor, backColor);
		}
		/// <summary>
		/// Fills the interior of a rectangle with a pattern.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context</param>
		/// <param name="r"><see cref="RectangleF"/> Structure that represents the rectangle to fill. </param>
		/// <param name="hatchStyle"><see cref="PatternStyle"/>.</param>
		/// <param name="foreColor">A <see cref="Color"/> used for the pattern fill.</param>
		/// <param name="backColor">A <see cref="Color"/> used for the pattern fill.</param>
		public static void FillRectangle(Graphics g, RectangleF r, PatternStyle hatchStyle, Color foreColor, Color backColor)
		{
#if DEBUG
			if (Switches.BrushPaint.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(r, hatchStyle, foreColor, backColor);
#endif


			if (r.Height == 0 || r.Width == 0)
				return;

			if (hatchStyle != PatternStyle.None)
			{
				Brush br = new HatchBrush((HatchStyle) (hatchStyle-1), foreColor, backColor);
				g.FillRectangle(br, r);
				br.Dispose();
			}
		}

		/// <summary>
		/// Fills the interior of a rectangle with a solid color.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="Rectangle"/> Structure that represents the rectangle to fill. </param>
		/// <param name="color">A <see cref="Color"/>.</param>
		public static void FillRectangle(Graphics g, Rectangle r, Color color)
		{
			FillRectangle(g, (RectangleF)r, color);
		}
		/// <summary>
		/// Fills the interior of a rectangle with a solid color.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="RectangleF"/> Structure that represents the rectangle to fill. </param>
		/// <param name="color">A <see cref="Color"/>.</param>
		public static void FillRectangle(Graphics g, RectangleF r, Color color)
		{
#if DEBUG
			if (Switches.BrushPaint.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(r, color);
#else
			;
#endif


			if (r.Height == 0 || r.Width == 0)
				return;

			Brush br = new SolidBrush(color);
			g.FillRectangle(br, r);
			br.Dispose();
		}

		/// <summary>
		/// Overloaded. Fills the interior of a region using <see cref="BrushInfo"/> information.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="Region"/> Structure that represents the region to fill. </param>
		/// <param name="brush"><see cref="BrushInfo"/> object that determines the characteristics of the fill.</param>
		public static void FillRegion(Graphics g, Region r, BrushInfo brush)
		{
			//if(r.IsEmpty(g))
			//	return;

			switch (brush.Style)
			{
				case BrushStyle.None:
					g.FillRegion(SystemBrushes.Window, r);
					break;

				case BrushStyle.Solid:
				{
					//g.FillRegion(brush.BackColorBrush, r);
					Brush br = new SolidBrush(brush.BackColor);
					g.FillRegion(br, r);
					br.Dispose();
					break;
				}

				case BrushStyle.Pattern:
					if (brush.PatternStyle == PatternStyle.None)
						goto case BrushStyle.Solid;
					//g.FillRegion(brush.PatternedBrush, r);
					FillRegion(g, r, brush.PatternStyle, brush.ForeColor, brush.BackColor);
					break;

				case BrushStyle.Gradient:
					if (brush.GradientStyle == GradientStyle.None)
						goto case BrushStyle.Solid;
					FillRegion(g, r, brush.GradientStyle, (Color[])brush.GradientColors.ToArray(typeof(Color)));
					break;
			}
		}

		/// <summary>
		/// Fills the interior of a region with a gradient.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="Region"/> Structure that represents the region to fill. </param>
		/// <param name="gradientStyle"><see cref="GradientStyle"/>.</param>
		/// <param name="foreColor">A <see cref="Color"/> used for the gradient fill.</param>
		/// <param name="backColor">A <see cref="Color"/> used for the gradient fill.</param>
		public static void FillRegion(Graphics g, Region r, GradientStyle gradientStyle, Color foreColor, Color backColor)
		{
		}
		/// <summary>
		/// Fills the interior of a region with a gradient.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="Region"/> structure that represents the region to fill. </param>
		/// <param name="gradientStyle"><see cref="GradientStyle"/>.</param>
		/// <param name="colors">An array of <see cref="Color"/> used for the gradient fill.</param>
		public static void FillRegion(Graphics g, Region r, GradientStyle gradientStyle, Color[] colors)
		{
			Trace.Assert(colors.Length >= 2, "The colors arrayed in the call to FillRectangle should have at least 2 entries.");
#if DEBUG
			if (Switches.BrushPaint.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(r, gradientStyle, colors);
#else
			;
#endif


			if(r.IsEmpty(g))
				return;

			Brush br = null;
			LinearGradientBrush lgbr = null;
			PathGradientBrush pbr = null;
			GraphicsPath path = null;
			RectangleF clip = RectangleF.Empty;

			switch (gradientStyle)
			{
				case GradientStyle.ForwardDiagonal:
					lgbr = new LinearGradientBrush(r.GetBounds(g), Color.Empty, Color.Empty, LinearGradientMode.ForwardDiagonal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					lgbr = null;
					break;

				case GradientStyle.BackwardDiagonal:
					lgbr = new LinearGradientBrush(r.GetBounds(g), Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					lgbr = null;
					break;

				case GradientStyle.Horizontal:
					lgbr = new LinearGradientBrush(r.GetBounds(g), Color.Empty, Color.Empty, LinearGradientMode.Horizontal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					lgbr = null;
					break;

				case GradientStyle.Vertical:
					lgbr = new LinearGradientBrush(r.GetBounds(g), Color.Empty, Color.Empty, LinearGradientMode.Vertical);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					lgbr = null;
					break;

				case GradientStyle.PathRectangle:
					path = new GraphicsPath();

					clip = g.ClipBounds;
					g.IntersectClip(r);

					path.AddRectangle(r.GetBounds(g));
					break;

				case GradientStyle.PathEllipse:
					path = new GraphicsPath();

					clip = g.ClipBounds;
					g.IntersectClip(r);

					Rectangle rt = Rectangle.Ceiling(r.GetBounds(g));
					rt.Inflate(rt.Width/4, rt.Height/4);
					path.AddEllipse(rt);
					break;

					//				case GradientStyle.PathPie:
					//					path = new GraphicsPath();
					//					path.AddPie(r, -30, 200);
					//					break;
			}

			if (path != null)
			{
				pbr = new PathGradientBrush(path);
				pbr.CenterColor = colors[0];

                Color[] scolors;
                if ( path.PointCount < colors.Length - 1 )
                {
                    scolors = new Color[ path.PointCount ];
                }
                else
                {
                    scolors = new Color[ colors.Length - 1 ];
                }

				int j = 0;
				for (int i = colors.Length - 2; i >= 0 && j < path.PointCount; i--)
					scolors[j++] = colors[i];
				pbr.SurroundColors = scolors;
				//pbr.CenterPoint = new Point(r.X, r.Y+r.Height/2);
				br = pbr;
				pbr = null;

			}

			if (br != null)
			{
				g.FillRegion(br, r);

				if (path != null)
					path.Dispose();

				br.Dispose();
				br = null;
			}

			if (!clip.IsEmpty)
				g.SetClip(clip);
		}
		/// <summary>
		/// Fills the interior of a region with a pattern.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="Region"/> Structure that represents the region to fill. </param>
		/// <param name="hatchStyle"><see cref="PatternStyle"/>.</param>
		/// <param name="foreColor">A <see cref="Color"/> used for the pattern fill.</param>
		/// <param name="backColor">A <see cref="Color"/> used for the pattern fill.</param>
		public static void FillRegion(Graphics g, Region r, PatternStyle hatchStyle, Color foreColor, Color backColor)
		{
#if DEBUG
			if (Switches.BrushPaint.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(r, hatchStyle, foreColor, backColor);
#else
			;
#endif


			//if(r.IsEmpty(g))
			//	return;

			if (hatchStyle != PatternStyle.None)
			{
				Brush br = new HatchBrush((HatchStyle) (hatchStyle-1), foreColor, backColor);
				g.FillRegion(br, r);
				br.Dispose();
			}
		}

		/// <summary>
		/// Fills the interior of a region with a solid color.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> context.</param>
		/// <param name="r"><see cref="Region"/> Structure that represents the region to fill. </param>
		/// <param name="color">A <see cref="Color"/>.</param>
		public static void FillRegion(Graphics g, Region r, Color color)
		{
#if DEBUG
			if (Switches.BrushPaint.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(r, color);
#else
			;
#endif


			if(r.IsEmpty(g))
				return;

			Brush br = new SolidBrush(color);
			g.FillRegion(br, r);
			br.Dispose();
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="g"></param>
		/// <param name="p"></param>
		/// <param name="color"></param>
		public static void FillPath (Graphics g, GraphicsPath p, Color color)
		{
			FillPath(g, p, new BrushInfo(color));
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="g"></param>
		/// <param name="p"></param>
		/// <param name="brush"></param>
		public static void FillPath (Graphics g, GraphicsPath p, BrushInfo brush)
		{
			switch (brush.Style)
			{
				case BrushStyle.None:
					g.FillPath(SystemBrushes.Window, p);
					break;

				case BrushStyle.Solid:
				{
					Brush br = new SolidBrush(brush.BackColor);
					g.FillPath(br, p);
					br.Dispose();
					//g.FillPath(brush.BackColorBrush, p);
					break;
				}

				case BrushStyle.Pattern:
				{
					if (brush.PatternStyle == PatternStyle.None)
						goto case BrushStyle.Solid;
					Brush br = new HatchBrush((HatchStyle) (brush.PatternStyle-1), brush.ForeColor, brush.BackColor);
					g.FillPath(br, p);
					br.Dispose();
					//g.FillPath(brush.PatternedBrush, p);
					break;
				}

				case BrushStyle.Gradient:
					if (brush.GradientStyle == GradientStyle.None)
						goto case BrushStyle.Solid;
					FillPath( g, p, brush.GradientStyle, (Color[])brush.GradientColors.ToArray(typeof(Color)) );
					break;
			}
		}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="p"></param>
    /// <param name="gradientStyle"></param>
    /// <param name="colors"></param>
		public static void FillPath (Graphics g, GraphicsPath p, GradientStyle gradientStyle, Color[] colors)
		{
			Trace.Assert(colors.Length >= 2, "The colors arrayed in the call to FillRectangle should have at least 2 entries.");
#if DEBUG
            if (Switches.BrushPaint.TraceVerbose)
                TraceUtil.TraceCurrentMethodInfo(p, gradientStyle, colors);
#endif

      RectangleF bounds = p.GetBounds();

      if( bounds.Width == 0 || bounds.Height == 0 )
      {
        return;
      }

			Brush br = null;
			LinearGradientBrush lgbr = null;
			PathGradientBrush pbr = null;
			GraphicsPath path = null;
			RectangleF clip = RectangleF.Empty;

			switch (gradientStyle)
			{
				case GradientStyle.ForwardDiagonal:
					lgbr = new LinearGradientBrush(p.GetBounds(), Color.Empty, Color.Empty, LinearGradientMode.ForwardDiagonal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					lgbr = null;
					break;

				case GradientStyle.BackwardDiagonal:
					lgbr = new LinearGradientBrush(p.GetBounds(), Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					lgbr = null;
					break;

				case GradientStyle.Horizontal:
					lgbr = new LinearGradientBrush(p.GetBounds(), Color.Empty, Color.Empty, LinearGradientMode.Horizontal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					lgbr = null;
					break;

				case GradientStyle.Vertical:
					lgbr = new LinearGradientBrush(p.GetBounds(), Color.Empty, Color.Empty, LinearGradientMode.Vertical);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					lgbr = null;
					break;

				case GradientStyle.PathRectangle:
					path = new GraphicsPath();
					path.AddRectangle(p.GetBounds());
					break;

				case GradientStyle.PathEllipse:
					path = new GraphicsPath();

					Rectangle rt = Rectangle.Ceiling(p.GetBounds());
					rt.Inflate(rt.Width/4, rt.Height/4);
					path.AddEllipse(rt);
					break;
			}

			if (path != null)
			{
				pbr = new PathGradientBrush(path);
				pbr.CenterColor = colors[0];

                Color[] scolors;
                if ( path.PointCount < colors.Length - 1 )
                {
                    scolors = new Color[ path.PointCount ];
                }
                else
                {
                    scolors = new Color[ colors.Length - 1 ];
                }

				int j = 0;
				for(int i = colors.Length - 2; i >= 0 && j< path.PointCount; i--)
					scolors[j++] = colors[i];
				pbr.SurroundColors = scolors;
				br = pbr;
				pbr = null;
			}

			if (br != null)
			{
				g.FillPath( br, p );

				if (path != null)
					path.Dispose();

				br.Dispose();
				br = null;
			}

			if (!clip.IsEmpty)
				g.SetClip(clip);
		}
	}
}
