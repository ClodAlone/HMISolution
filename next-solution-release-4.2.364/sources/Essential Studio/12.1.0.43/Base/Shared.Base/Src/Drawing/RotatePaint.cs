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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace Syncfusion.Drawing
{
	/// <summary>
	/// Helper routines for drawing rotated text.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class RotatePaint
	{
		/// <internalonly/>
		public static Point RotatePoint(Point p, double angle)
		{
			return Point.Ceiling(RotatePoint(new PointF(p.X, p.Y), angle, PointF.Empty));
		}

		/// <internalonly/>
		public static Point RotatePoint(Point p, double angle, Point origin)
		{
			return Point.Ceiling(RotatePoint(new PointF(p.X, p.Y), angle, origin));
		}

		/// <internalonly/>
		public static Point[] RotateRectangle(Rectangle r, double angle)
		{
			return RotateRectangle(r, angle, Point.Empty);
		}

		/// <internalonly/>
		public static Point[] RotateRectangle(Rectangle r, double angle, Point origin)
		{
			PointF originF = new PointF(origin.X, origin.Y);
			return new Point[] 
			{
				Point.Ceiling(RotatePoint(new PointF(r.Left, r.Top), angle, originF)),
				Point.Ceiling(RotatePoint(new PointF(r.Left, r.Bottom), angle, originF)),
				Point.Ceiling(RotatePoint(new PointF(r.Right, r.Bottom), angle, originF)),
				Point.Ceiling(RotatePoint(new PointF(r.Right, r.Top), angle, originF))
			};
		}

		/// <internalonly/>
		public static Rectangle CalcOutsideRect(Rectangle rect, float angle)
		{
			return Rectangle.Ceiling(CalcOutsideRect(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), angle));
		}
           
		/// <internalonly/>
		public static Rectangle CalcInsideRect(Rectangle rect, float angle)
		{
			return Rectangle.Ceiling(CalcInsideRect(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), angle));
		}

		/// <internalonly/>
		static public Rectangle CenterInRect(Rectangle rect, Size size)
		{
			return Rectangle.Ceiling(CenterInRect(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), new SizeF(size.Width, size.Height)));
		}

		/// <internalonly/>
		public  static void DrawRotatedString(Graphics g, string text, Font font, Brush br, Rectangle rect, StringFormat format, float angle)
		{
			DrawRotatedString(g, text, font, br, new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), format, angle);
		}

		/// <internalonly/>
		public static PointF RotatePoint(PointF p, double angle)
		{
			return RotatePoint(p, angle, PointF.Empty);
		}

		/// <internalonly/>
		public static PointF RotatePoint(PointF p, double angle, PointF origin)
		{
			double normAngle = angle;
	        
			while (normAngle < 0)
				normAngle += 360.0;
	        
			while (normAngle >= 360.0)
				normAngle -= 360.0;

			double radianAngle = angle*Math.PI/180.0;

			double ox = origin.X;
			double oy = origin.Y;
			double cos = Math.Cos(radianAngle);
			double sin = Math.Sin(radianAngle);
			double dx = p.X-origin.X;
			double dy = p.Y-origin.Y;
			double x = ox+dx*cos-dy*sin;
			double y = oy+dy*cos+dx*sin;

			return new PointF((float) x, (float) y);
		}

		/// <internalonly/>
		public static PointF[] RotateRectangle(RectangleF r, double angle)
		{
			return RotateRectangle(r, angle, PointF.Empty);
		}

		/// <internalonly/>
		public static PointF[] RotateRectangle(RectangleF r, double angle, PointF origin)
		{
			return new PointF[] 
			{
				RotatePoint(new PointF(r.Left, r.Top), angle, origin),
				RotatePoint(new PointF(r.Left, r.Bottom), angle, origin),
				RotatePoint(new PointF(r.Right, r.Bottom), angle, origin),
				RotatePoint(new PointF(r.Right, r.Top), angle, origin)
			};
		}

		/// <internalonly/>
		public static RectangleF CalcOutsideRect(RectangleF rect, float angle)
		{
			// Calculated rotated rectangle that fits inside the given cell
			RectangleF r = rect; //new RectangleF(PointF.Empty, rect.SizeF);
			PointF center = new PointF(r.X+r.Width/2, r.Y+r.Height/2);
			PointF[] points = RotateRectangle(r, angle, center);
			float left = float.MaxValue, right = 0; 
			float top = float.MaxValue, bottom = 0;
			foreach (PointF pt in points)
			{
				left = Math.Min(left, pt.X);
				right = Math.Max(right, pt.X);
				top = Math.Min(top, pt.Y);
				bottom = Math.Max(bottom, pt.Y);
			}
			return RectangleF.FromLTRB(left, top, right, bottom);
		}
           
		/// <internalonly/>
		public static RectangleF CalcInsideRect(RectangleF rect, float angle)
		{
			// Calculated rotated rectangle that fits inside the given cell
			RectangleF r = CalcOutsideRect(rect, angle); //new RectangleF(PointF.Empty, rect.SizeF);
			RectangleF rcInside = CenterInRect(rect, new SizeF(
				(float) (rect.Width * ((float) rect.Width/ (float) r.Width)),
				(float) (rect.Height *((float) rect.Height/ (float) r.Height))
				));
			return rcInside;
		}
		
		/// <internalonly/>
		public static SizeF MeasureStringBounds(SizeF sz, float angle)
		{
			float h = sz.Height;
			float w = sz.Width;
					
			double radianAngle = angle*Math.PI/180.0;
			double cosAngle = Math.Abs(Math.Cos(radianAngle));
			double sinAngle = Math.Abs(Math.Sin(radianAngle));

			SizeF textSize = new SizeF(0, 0);
			textSize.Height = (float) ( h * cosAngle + w * sinAngle);
			textSize.Width = (float) ( w * cosAngle + h * sinAngle);
			return textSize;
		}

		/// <internalonly/>
		public static SizeF MeasureStringBounds(Graphics g, string sOutput, Font font, int width, StringFormat format, float angle)
		{
			SizeF sz = g.MeasureString(sOutput, font, width, format);  

			float h = sz.Height;
			float w = sz.Width;
					
			double radianAngle = angle*Math.PI/180.0;
			double cosAngle = Math.Abs(Math.Cos(radianAngle));
			double sinAngle = Math.Abs(Math.Sin(radianAngle));

			SizeF textSize = new SizeF(0, 0);
			textSize.Height = (float) ( h * cosAngle + w * sinAngle);
			textSize.Width = (float) ( w * cosAngle + h * sinAngle);
			return textSize;
		}


		/// <internalonly/>
		static public RectangleF CenterInRect(RectangleF rect, SizeF size)
		{
			float dx = 0;
			if (size.Width < rect.Width)
				dx = rect.Width-size.Width;

			float dy = 0;
			if (size.Height < rect.Height)
				dy = rect.Height-size.Height;

			return new RectangleF(rect.Left+dx/2, rect.Top+dy/2, 
				Math.Min(size.Width, rect.Width), Math.Min(size.Height, rect.Height));
		}

		/// <internalonly/>
		public  static void DrawRotatedString(Graphics g, string text, Font font, Brush br, RectangleF rect, StringFormat format, float angle)
		{
			float normAngle = angle;
            if (normAngle > 0)
                normAngle = -normAngle;
			while (normAngle < 0)
				normAngle += 360.0f;
	        
			while (normAngle >= 360.0)
				normAngle -= 360.0f;

			Region oldClip = g.Clip;
			g.IntersectClip(rect);
			PointF center = new PointF(rect.X+rect.Width/2, rect.Y+rect.Height/2);

			// Calculated rotated rectangle that fits inside the given cell.
			RectangleF rcOutside = CalcOutsideRect(rect, angle);
			PointF[] points = RotateRectangle(rcOutside, angle, center);
			//g.FillPolygon(new SolidBrush(Color.Gold), points, System.Drawing.Drawing2D.FillMode.Winding);

			System.Drawing.Drawing2D.Matrix m = g.Transform;  

			g.TranslateTransform(center.X, center.Y);
			g.RotateTransform(normAngle);
			rcOutside.Offset(-center.X, -center.Y);
			

			//					if ((int) normAngle % 90 != 0)
			//					{
			//						 Adjust clipping rectangle - this is not mathematically perfect, just an estimate.
			//						float adjAngle = normAngle; 
			//						if (adjAngle >= 180.0)
			//							adjAngle -= 180.0f;
			//					
			//						if (adjAngle >= 45 && adjAngle <= 135)
			//							rcOutside.Inflate(-rcOutside.Width * Math.Abs(90-adjAngle)/45f*0.2f, -rcOutside.Height * Math.Abs(90-adjAngle)/45f*0.1f);
			//						else 
			//						{
			//							if (adjAngle > 90)
			//								adjAngle -= 180;
			//							rcOutside.Inflate(-rcOutside.Width * adjAngle/45f*0.1f, -rcOutside.Height * adjAngle/45f*0.2f);
			//						}
			//					}

			g.DrawString(text, font, br, rcOutside, format);
			//			g.RotateTransform(-normAngle);
			//			g.TranslateTransform(-center.X, -center.Y);
			//g.ResetTransform();
			g.Transform = m;          
			g.Clip = oldClip;
		}

	}
}
