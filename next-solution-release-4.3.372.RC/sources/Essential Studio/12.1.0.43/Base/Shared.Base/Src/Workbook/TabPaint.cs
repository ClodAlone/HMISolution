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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
 	/// <summary>
	///      This class fully encapsulates the painting logic for a tab in a TabBarSplitterControl.  
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
    public sealed class TabPaint
    {
        private TabPaint() { }

		#region Constants
		private static int TEXT_FLAGS = DrawTextFormats.DT_EXPANDTABS | DrawTextFormats.DT_NOPREFIX |
			DrawTextFormats.DT_SINGLELINE | DrawTextFormats.DT_VCENTER | DrawTextFormats.DT_END_ELLIPSIS;
		#endregion

		/// <internalonly/>
		public static void DrawTab(Graphics g, Rectangle bounds, ImageList imageList, int imageIndex, string label, Brush fillBrush, Color textColor, Font font, bool enabled, int delta, bool overlapping)
		{
			Debug.Assert(g != null, "Must pass valid graphics");

			// First the background: a tab.
			DrawTabButton(g, bounds, label, fillBrush, delta, overlapping);
			
			// Now draw the image and text on the tab.
			bounds.Inflate(-delta, -1); // Clipping bounds
			DrawTabText(g, bounds, imageList, imageIndex, label, textColor, font, enabled);
		}

//		public static void DrawTab(Graphics g, Rectangle bounds, string label, Brush fillBrush, Color textColor, Font font, bool enabled, int delta, bool overlapping)
//		{
//			Debug.Assert(g != null, "Must pass valid graphics");
//
//			// First the background: a tab.
//			DrawTabButton(g, bounds, label, fillBrush, delta, overlapping);
//			
//			// Now draw the text on the tab.
//			bounds.Inflate(-delta, -1); // Clipping bounds
//			DrawTabText(g, bounds, label, textColor, font, enabled);
//		}

		/// <internalonly/>
		public static void DrawTabButton(Graphics g, Rectangle bounds, string label, Brush fillBrush, int delta, bool overlapping)
		{
			// Tab Polygon
			Point[] ptFrame;
			//Point[] ptFrame2;

			/*if (false && !overlapping)
			{
				ptFrame  = new Point[] {
						new Point(bounds.Left+bounds.Width,			bounds.Top),
						new Point(bounds.Left+bounds.Width-delta,   bounds.Bottom),
						new Point(bounds.Left+delta,              bounds.Bottom),
						new Point(bounds.Left+delta/2,            bounds.Top+bounds.Height/2),
						new Point(bounds.Left+delta,              bounds.Top),
					};

			}
			else*/
			{
				ptFrame = new Point[] {
						new Point(bounds.Left+bounds.Width,			bounds.Top),
						new Point(bounds.Left+bounds.Width-delta,   bounds.Bottom),
						new Point(bounds.Left+delta,              bounds.Bottom),
						new Point(bounds.Left,	                bounds.Top),
					};
			}
			/*ptFrame2 = new Point[] {
					new Point(bounds.Left+delta/2,            bounds.Top+bounds.Height/2),
					new Point(bounds.Left+delta,              bounds.Bottom),
					new Point(bounds.Left,					bounds.Bottom),
					};
			*/

			// shadow
			Point[] ptShadow = new Point[] {
				new Point(bounds.Left+bounds.Width,		    bounds.Top),
				new Point(bounds.Left+bounds.Width-delta,   bounds.Bottom),
				new Point(bounds.Left+delta+1,            bounds.Bottom),
			};

			g.FillPolygon(fillBrush, ptFrame, FillMode.Alternate);
			//g.FillPolygon(SystemBrushes.ScrollBar, ptFrame2, FillMode.Alternate);

			Pen shadowPen = new Pen(SystemColors.ControlDark, 2);
			g.DrawLines(shadowPen, ptShadow);
			shadowPen.Dispose();
			
			Pen framePen = SystemPens.WindowFrame;
			g.DrawLines(framePen, ptFrame);
			/*
			Rectangle r1 = new Rectangle(bounds.Right-delta, bounds.Top, delta/2, bounds.Height);
			Rectangle r2 = new Rectangle(bounds.Right-delta/2, bounds.Top, delta/2, bounds.Height/2);
			g.SetClip(r1, CombineMode.Exclude);			
			g.SetClip(r2, CombineMode.Exclude);			
			*/
		}
		
		/// <internalonly/>
		public static void DrawTabText(Graphics g, Rectangle bounds, ImageList imageList, int imageIndex, string label, Color textColor, Font font, bool enabled)
		{
			// Text size and center text.
			int imageWidth = 0;
			//FR 1081
            SizeF sizeF;
            #if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
								sizeF = g.MeasureString(label, font);
            #else
                sizeF = TextRenderer.MeasureText(label, font);
            #endif

			if (imageList != null && imageIndex >= 0)
			{	
				imageWidth = imageList.ImageSize.Width;
				sizeF.Width += imageList.ImageSize.Width;
				sizeF.Height = System.Math.Max(imageList.ImageSize.Height, sizeF.Height);
			}
			Rectangle textArea = bounds;
			textArea.Inflate(((int) sizeF.Width-bounds.Width)/2, ((int) sizeF.Height-bounds.Height)/2);

			// Draw enabled or disabled.			
            Region originalClip = null;
			Rectangle intersectArea = textArea;
			intersectArea.Intersect(bounds);
			
			try
			{
				if (intersectArea != textArea)
				{
		            originalClip = g.Clip;
					g.IntersectClip(bounds);
				}
				
				if (enabled)
				{
					Brush textBrush = new SolidBrush(textColor);
					try
					{
						// Image
						if (imageIndex >= 0 && imageIndex < imageList.Images.Count)
						{
							imageList.Draw(g, textArea.Left-2, textArea.Top, imageIndex);
							textArea.X += imageWidth;
							textArea.Width -= imageWidth;
						}

						DrawTextInternal( g, font, textColor, textArea, label );
					}
					finally
					{
						textBrush.Dispose();
					}
				}
				else
					ControlPaint.DrawStringDisabled(g, label, font, textColor, textArea, StringFormat.GenericDefault);
			}
			finally
			{
				if (originalClip != null)
					g.Clip = originalClip;
			}
		}
		/// <summary>
		/// Draws text by native GDI API.
		/// </summary>
		/// <param name="g"> Graphics object which ised for drawing. </param>
		/// <param name="f"> Font of the text. </param>
		/// <param name="color"> Color of the text. </param>
		/// <param name="rcBounds"> Bounds of tge text. </param>
		/// <param name="text"> Text which has to be drawn. </param>
		private static void DrawTextInternal( Graphics g, Font f, Color color, Rectangle rcBounds, string text )
		{
			IntPtr clipRgn = g.Clip.GetHrgn( g );
			IntPtr currentRgn = IntPtr.Zero;
			IntPtr hdc = g.GetHdc();
			IntPtr hFont = f.ToHfont();
			IntPtr prevFont = NativeMethods.SelectObject( hdc, hFont );

			NativeMethods.GetClipRgn( hdc, currentRgn );
			NativeMethods.SelectClipRgn( hdc, clipRgn );
			NativeMethods.SetTextColor( hdc, color.ToArgb() & 0xFFFFFF );
			NativeMethods.SetBkMode( hdc, 1 ); // TRANSPARENT

			NativeMethods.RECT rect = new NativeMethods.RECT( rcBounds );

			NativeMethods.DrawText( hdc, text, text.Length, ref rect, TEXT_FLAGS );

			NativeMethods.SelectClipRgn( hdc, currentRgn ); 
			prevFont = NativeMethods.SelectObject( hdc, prevFont );
			NativeMethods.DeleteObject( hFont );
			NativeMethods.DeleteObject( clipRgn );

			g.ReleaseHdc( hdc );
		}
    }
}
