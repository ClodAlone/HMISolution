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

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Documentation;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
#endregion

namespace Syncfusion.Drawing
{
	/// <internalonly/>
	/// <summary></summary>
	[DocumentationExclude()]
	public interface INonClientPaintingSupport
	{
		/// <summary>
		/// Implement this method and draw your NonClient area using the passed in parameters.
		/// </summary>
		/// <param name="e">The PaintEventArgs using this to draw the non client area.</param>
		/// <param name="displayRect">The control's window bounds into which to draw. Left and Top are usually zero.</param>
		/// <param name="windowRectInScreen">The control's bounds in screen co-ordinates.</param>
		/// <returns>
		/// HRgn (as IntPtr) that excludes the region you just drew in the displayRect.
		/// </returns>
		IntPtr NonClientPaint( PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen );
	}

	/// <summary></summary>
	[DocumentationExclude()]
	public class DrawingUtils
	{
		// Using this method rather than Bounds, which at NCPaint is not updated properly, sometimes.
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="c"/>
		private static Rectangle GetDesktopBounds( Control c )
		{
			NativeMethods.RECT r = new NativeMethods.RECT();

			NativeMethods.GetWindowRect( (int)c.Handle, ref r );

			return new Rectangle( r.left, r.top, r.Width, r.Height );
		}

		/// <summary>
		/// Call this method to help you prepare for non client painting. This method will in turn
		/// call your INonClientPaintingSupport.NonClientPaint implementation.
		/// </summary>
		/// <param name="control">The control in which we will be drawing.</param>
		/// <param name="ncPaintDelegate">The INonClientPaintingSupport implementation to which we will delegate the final non client drawing.</param>
		/// <param name="m">The WM_NCPAINT message.</param>
		/// <returns></returns>
		public static IntPtr NCPaintHelper( Control control, INonClientPaintingSupport ncPaintDelegate, ref Message m )
		{
			if( ncPaintDelegate == null )
			{
				return IntPtr.Zero;
			}

			IntPtr hdc = IntPtr.Zero;
			Graphics g = null;
			IntPtr hwnd = m.HWnd;
			IntPtr remainingClipRegion = IntPtr.Zero;

			try
			{
				Rectangle clipRect;
				Rectangle desktopBounds = GetDesktopBounds( control );
				Rectangle bounds = desktopBounds;
				bounds.X = bounds.Y = 0;

				hdc = NativeMethods.GetWindowDC( m.HWnd );

				if( hdc != IntPtr.Zero )
				{
					if( m.WParam.ToInt32() == 1 || m.WParam == IntPtr.Zero )
					{
						// Since we will be painting the client rect.
						clipRect = bounds;
					}
					else
					{
						Rectangle rect = desktopBounds;
						Point pt = new Point( rect.Left, rect.Top );
						NativeMethods.RECT r = new NativeMethods.RECT();

						NativeMethods.GetRgnBox( m.WParam, ref r );
						clipRect = new Rectangle( r.left, r.top, r.Width, r.Height );
						clipRect.Intersect( rect );
						clipRect.Offset( -pt.X, -pt.Y );
					}

					g = Graphics.FromHdc( hdc );
					//g.SetClip(clipRect, CombineMode.Replace);
					g.Clip = new Region( clipRect );
					PaintEventArgs pe = new PaintEventArgs( g, clipRect );
					remainingClipRegion = ncPaintDelegate.NonClientPaint( pe, bounds, desktopBounds );

					// When wParam is 1, the entire window frame needs to be updated.
					if( m.WParam.ToInt32() == 1 || m.WParam == IntPtr.Zero )
					{
						// No existing clipping region, just replace it.
						m.WParam = remainingClipRegion;
					}
					else
					{
						// Combine with exiting clipping region.
						NativeMethods.CombineRgn( m.WParam, m.WParam, remainingClipRegion, NativeMethods.RGN_AND );
						NativeMethods.DeleteObject( remainingClipRegion );
						remainingClipRegion = IntPtr.Zero;
					}
				}
			}
			finally
			{
				if( g != null )
				{
					g.Dispose();
				}
				if( hdc != IntPtr.Zero )
				{
					NativeMethods.ReleaseDC( hwnd, hdc );
				}
			}

			return remainingClipRegion;
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="control"/>
		public static void DrawDesignTimeBorder( Graphics g, Control control )
		{
			Rectangle clientRectangle;
			Color bgColor, adjustedBgColor;
			Pen pen;

			clientRectangle = control.ClientRectangle;
			bgColor = control.BackColor;
			if( ( (double)bgColor.GetBrightness() ) >= 0.5 )
			{
				adjustedBgColor = ControlPaint.Dark( control.BackColor );
			}
			else
			{
				adjustedBgColor = ControlPaint.Light( control.BackColor );
			}

			pen = new Pen( adjustedBgColor );
			pen.DashStyle = DashStyle.Dash;

			clientRectangle.Width = ( clientRectangle.Width - 1 );
			clientRectangle.Height = ( clientRectangle.Height - 1 );

			g.DrawRectangle( pen, clientRectangle );
			pen.Dispose();
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="iconImage"/>
		/// <param name="left"/>
		/// <param name="top"/>
		public static void DrawShadow( Graphics g, Image iconImage, int left, int top )
		{
			ImageAttributes ia = new ImageAttributes();
			ColorMatrix cm = new ColorMatrix();

			cm.Matrix00 = 0;
			cm.Matrix11 = 0;
			cm.Matrix22 = 0;
			cm.Matrix33 = 0.25f;

			ia.SetColorMatrix( cm );

			g.DrawImage( iconImage, new Rectangle( left, top, iconImage.Width, iconImage.Height ),
			  0, 0, iconImage.Width, iconImage.Height,
			  GraphicsUnit.Pixel, ia );

			ia.Dispose();
		}

		/// <summary>
		/// Paints a rectangular area with the given colors in Office2007 style.
		/// </summary>
		/// <param name="g">A <see cref="Graphics"/> object.</param>
		/// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
		/// <param name="clrBorder">The border color.</param>
		/// <param name="topFirst">The top gradient start color.</param>
		/// <param name="topLast">The top gradient end color.</param>
		/// <param name="bottomFirst">The bottom gradient start color.</param>
		/// <param name="bottomLast">The bottom gradient end color.</param>
		/// <param name="bottomLine">The bottom line color.</param>
		public static void PaintButtonGradient( Graphics g, Rectangle rect, Color clrBorder, Color topFirst, Color topLast, Color bottomFirst, Color bottomLast, Color bottomLine )
		{
			// Draw Border
			Pen borderPen = new Pen( clrBorder );

			g.DrawLine( borderPen, rect.X + 1, rect.Y, rect.Right - 2, rect.Y );
			g.DrawLine( borderPen, rect.X, rect.Y + 1, rect.X, rect.Bottom - 2 );
			g.DrawLine( borderPen, rect.X + 1, rect.Bottom - 1, rect.Right - 2, rect.Bottom - 1 );
			g.DrawLine( borderPen, rect.Right - 1, rect.Y + 1, rect.Right - 1, rect.Bottom - 2 );

			rect.Inflate( -2, -2 );

			// Draw foreground
			RectangleF foreRectTop = new RectangleF( rect.X, rect.Y, rect.Width, rect.Height / 2f );
			RectangleF foreRectBottom = new RectangleF( rect.X, rect.Y + rect.Height / 2f, rect.Width, rect.Height / 2f );

			//Paints TopRect
			using( LinearGradientBrush brush = new LinearGradientBrush( foreRectTop, topFirst, topLast, LinearGradientMode.Vertical ) )
			{
				g.FillRectangle( brush, foreRectTop );
			}

			//Paints BottomRect
			using( LinearGradientBrush brush = new LinearGradientBrush( foreRectBottom, bottomFirst, bottomLast, LinearGradientMode.Vertical ) )
			{
				g.FillRectangle( brush, foreRectBottom );
			}

			//Draws bottom line
			Pen linePen = new Pen( bottomLine );

			g.DrawLine( linePen, foreRectBottom.X - 1, foreRectBottom.Y, foreRectBottom.X - 1, foreRectBottom.Bottom );
			g.DrawLine( linePen, foreRectBottom.Right, foreRectBottom.Y, foreRectBottom.Right, foreRectBottom.Bottom );
			g.DrawLine( linePen, foreRectBottom.X, foreRectBottom.Bottom, foreRectBottom.Right, foreRectBottom.Bottom );

			borderPen.Dispose();
			linePen.Dispose();
		}

		// Not the closest grayscale representation in the RGB space, but
		// pretty close.
		// Closest would be the cubic root of the product of the RGB colors,
		// but that cannot be represented in a ColorMatrix.
		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="image"/>
		/// <param name="left"/>
		/// <param name="top"/>
		public static void DrawGrayedImage( Graphics g, Image image, int left, int top )
		{
			DrawingUtils.DrawGrayedImage( g, image, left, top, 1 );
		}

		// grayScale between 0 to 1
		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="image"/>
		/// <param name="left"/>
		/// <param name="top"/>
		/// <param name="transparency"/>
		public static void DrawGrayedImage( Graphics g, Image image, int left, int top, float transparency )
		{
			Rectangle destRect = new Rectangle( left, top, image.Width, image.Height );
			RectangleF srcRect = new RectangleF( 0, 0, image.Width, image.Height );

			DrawGrayedImage( g, image, destRect, srcRect, transparency );
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="image"/>
		/// <param name="destRect"/>
		/// <param name="srcRect"/>
		public static void DrawGrayedImage( Graphics g, Image image, Rectangle destRect, RectangleF srcRect )
		{
			DrawingUtils.DrawGrayedImage( g, image, destRect, srcRect, 1 );
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="image"/>
		/// <param name="destRect"/>
		/// <param name="srcRect"/>
		/// <param name="transparency"/>
		public static void DrawGrayedImage( Graphics g, Image image, Rectangle destRect, RectangleF srcRect, float transparency )
		{
			ImageAttributes ia = new ImageAttributes();
			ColorMatrix cm = new ColorMatrix();

			// 1/3 on the top 3 rows and 3 columns
			cm.Matrix00 = 1 / 3f;
			cm.Matrix01 = 1 / 3f;
			cm.Matrix02 = 1 / 3f;
			cm.Matrix10 = 1 / 3f;
			cm.Matrix11 = 1 / 3f;
			cm.Matrix12 = 1 / 3f;
			cm.Matrix20 = 1 / 3f;
			cm.Matrix21 = 1 / 3f;
			cm.Matrix22 = 1 / 3f;

			cm.Matrix33 = transparency;

			ia.SetColorMatrix( cm );

			g.DrawImage( image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, ia );

			ia.Dispose();
		}

        // Transparency should be in the range 0 to 1.
		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rect"/>
		/// <param name="image"/>
		/// <param name="transparency"/>
		protected void DrawTransparentImage( Graphics g, Rectangle rect, Image image, float transparency )
		{
			ImageAttributes ia = new ImageAttributes();
			ColorMatrix cm = new ColorMatrix();
			cm.Matrix00 = 1;
			cm.Matrix11 = 1;
			cm.Matrix22 = 1;
			cm.Matrix33 = transparency;

			ia.SetColorMatrix( cm );

			g.DrawImage( image, rect, 0, 0, image.Width, image.Height,
			  GraphicsUnit.Pixel, ia );

			ia.Dispose();
		}

		/// <param name="rect"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static GraphicsPath GetRoundedRectangle( Rectangle rect, int radius )
		{
			GraphicsPath path = new GraphicsPath();

			int iLeft = rect.X;
			int iTop = rect.Y;
			int iRight = rect.Right;
			int iBottom = rect.Bottom;

			Point[] points = new Point[]
				{
					new Point( iLeft, iTop + radius ),
					new Point( iLeft + radius, iTop ),
					new Point( iRight - radius, iTop ),
					new Point( iRight, iTop + radius ),
					new Point( iRight, iBottom - radius ),
					new Point( iRight - radius, iBottom ),
					new Point( iLeft + radius, iBottom ),
					new Point( iLeft, iBottom - radius )
				};

			path.AddLines( points );
			path.CloseFigure();

			return path;
		}

		/// <summary>
		/// Adjusts the specified forecolor's brightness based on the specified backcolor and preferred contrast.
		/// </summary>
		/// <param name="foreColor">The forecolor to adjust.</param>
		/// <param name="backColor">The backcolor for reference.</param>
		/// <param name="prefContrastLevel">Preferred contrast level.</param>
		/// <remarks>
		/// This method checks if the current contrast in brightness between the 2 colors is
		/// less than the specified contrast level. If so, it brightens or darkens the forecolor appropriately.
		/// </remarks>
		public static void AdjustForeColorBrightnessForBackColor( ref Color foreColor, Color backColor, float prefContrastLevel )
		{
			float fBrightness = foreColor.GetBrightness();
			float bBrightness = backColor.GetBrightness();

			float curContrast = fBrightness - bBrightness;

			if( Math.Abs( curContrast ) < prefContrastLevel )
			{
				if( bBrightness < 0.5f )
				{
					fBrightness = bBrightness + prefContrastLevel;
					if( fBrightness > 1.0f )
					{
						fBrightness = 1.0f;
					}
				}
				else
				{
					fBrightness = bBrightness - prefContrastLevel;
					if( fBrightness < 0.0f )
					{
						fBrightness = 0.0f;
					}
				}

				float newr, newg, newb;
				ConvertHSBToRGB( foreColor.GetHue(), foreColor.GetSaturation(), fBrightness, out newr, out newg, out newb );

				foreColor = Color.FromArgb( foreColor.A, (int)Math.Floor( newr * 255f ),
				  (int)Math.Floor( newg * 255f ),
				  (int)Math.Floor( newb * 255f ) );
			}
		}

		/// <summary>
		/// Draws an image using the ImageList, taking into account the Graphics.ClipBounds.
		/// </summary>
		/// <param name="g">The Graphics object into which to draw.</param>
		/// <param name="il">The ImageList containing the image.</param>
		/// <param name="index">The index of the image.</param>
		/// <param name="rect">The rectangle into which to draw.</param>
		/// <remarks>
		/// This method will use ImageList_DrawEx to draw the image (to use the transparency info in the embedded images).
		/// We use the PInvoke rather than ImageList.Draw because, the Draw method
		/// uses the PaintEventArgs.ClipRectangle rather than g.ClipBounds (both
		/// can be different) and here we force the ClipBounds on the DC.
		/// <p>
		/// Also, you do not have to use this method if your images will never be
		/// drawn clipped (in that case just use ImageList.Draw).
		/// </p></remarks>
		public static void DrawImageViaImageList( Graphics g, ImageList il, int index, Rectangle rect )
		{
			//			Rectangle clipBounds = Rectangle.Ceiling(g.ClipBounds);
			//			Rectangle rect2 = rect;
			//			rect2.Intersect(clipBounds);
			//			//if(rect2 == rect)
			//			{
			//				// Within clipbounds, so go ahead and use the source ImageList.
			//				il.Draw(g, rect.X, rect.Y, rect.Width, rect.Height, index);
			//			}
			if( index < 0 || index >= il.Images.Count )
			{
				throw new ArgumentOutOfRangeException( "InvalidArgument" );
			}

			Rectangle clipBounds = Rectangle.Ceiling( g.ClipBounds );
			if( clipBounds.IntersectsWith( rect ) )
			{
				IntPtr hDC = g.GetHdc();
                try
                {
                    NativeMethods.RECT rcClipBox = new NativeMethods.RECT();
                    NativeMethods.GetClipBox(hDC, ref rcClipBox);
                    NativeMethods.IntersectClipRect(hDC,
                      clipBounds.Left, clipBounds.Top, clipBounds.Right, clipBounds.Bottom);

                    uint CLR_DEFAULT = NativeMethods.CLR_DEFAULT;
                    NativeMethods.ImageList_DrawEx(il.Handle,
                      index, hDC, rect.X, rect.Y, rect.Width, rect.Height,
                      CLR_DEFAULT, CLR_DEFAULT, 0);

                    NativeMethods.SelectClipRgn(hDC, IntPtr.Zero);
                    NativeMethods.IntersectClipRect(hDC, rcClipBox.left, rcClipBox.top, rcClipBox.right, rcClipBox.bottom);
                }
                finally
                {
                    g.ReleaseHdc(hDC);
                }
			}
		}

		/// <summary>
		/// Draws specified image to graphics context.
		/// If image size is less than maxWidth, maxHeight parameters, it will be drawn
        /// without any changes, otherwise - it will be scaled proportionally to those values.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="image">Image object which must be drawing.</param>
		/// <param name="x">X coordinate of the destination location.</param>
		/// <param name="y">Y coordinate of the destination location.</param>
		/// <param name="maxWidth">Maximum width of the image.</param>
		/// <param name="maxHeight">Maximum height of the image.</param>
		public static void DrawImage( Graphics g, Image image, float x, float y, float maxWidth, float maxHeight )
		{
			if( g == null )
			{
				throw new ArgumentNullException( "g" );
			}

			if( image == null )
			{
				throw new ArgumentNullException( "image" );
			}

			if( maxWidth <= 0 || maxHeight <= 0 )
			{
				return;
			}

			/*
			float imgWidth  = image.Width;
			float imgHeight = image.Height;

			if( imgWidth > maxWidth )
			{
			  float difference = imgWidth - maxWidth;
			  float coeff = 1.0f - ( float )Math.Round( difference / imgWidth, 3 );
		
			  imgWidth  *= coeff;
			  imgHeight *= coeff;
			}

			if( imgHeight > maxHeight )
			{
			  float difference = imgHeight - maxHeight;
			  float coeff = 1.0f - ( float )Math.Floor( difference / imgHeight );
		
			  imgWidth  *= coeff;
			  imgHeight *= coeff;
			}

			imgWidth  = ( imgWidth > maxWidth )   ? maxWidth  : imgWidth;
			imgHeight = ( imgHeight > maxHeight ) ? maxHeight : imgHeight;
			g.DrawImage( image, x, y, imgWidth, imgHeight );
			*/
			g.DrawImage( image, x, y, maxWidth, maxHeight );
		}

		/// <summary>
		/// Converts the HSB value to RGB.
		/// </summary>
		/// <param name="h">Hue.</param>
		/// <param name="s">Saturation.</param>
		/// <param name="v">Brightness.</param>
		/// <param name="r">Red.</param>
		/// <param name="g">Green.</param>
		/// <param name="b">Blue.</param>
		/// <remarks>
		/// This does not seem to yield accurate results, but very close.
		/// </remarks>
		public static void ConvertHSBToRGB( float h, float s, float v, out float r, out float g, out float b )
		{
			if( s == 0f )
			{
				// if s = 0 then h is undefined
				r = v;
				g = v;
				b = v;
			}
			else
			{
				float hue = h;
				if( h == 360.0f )
				{
					hue = 0.0f;
				}
				hue /= 60.0f;
				int i = (int)Math.Floor( (double)hue );
				float f = hue - i;
				float p = v * ( 1.0f - s );
				float q = v * ( 1.0f - ( s * f ) );
				float t = v * ( 1.0f - ( s * ( 1 - f ) ) );

				switch( i )
				{
					case 0:
					r = v;
					g = t;
					b = p;
					break;
					case 1:
					r = q;
					g = v;
					b = p;
					break;
					case 2:
					r = p;
					g = v;
					b = t;
					break;
					case 3:
					r = p;
					g = q;
					b = v;
					break;
					case 4:
					r = t;
					g = p;
					b = v;
					break;
					case 5:
					r = v;
					g = p;
					b = q;
					break;

					default:
					r = 0.0f;
					g = 0.0f;
					b = 0.0f;
					break; /*Trace.Assert(false);*/ // hue out of range
				}
			}
		}
	}


	/// <summary>
	/// Enumeration of Panose Font Family Types.  These can be used for
	/// determining the similarity of two fonts or for detecting non-character
	/// fonts like WingDings.
	/// </summary>
	public enum PanoseFontFamilyTypes: int
	{
		/// <summary>
		///  Any
		/// </summary>
		PAN_ANY=0,
		/// <summary>
		/// No Fit
		/// </summary>
		PAN_NO_FIT=1,
		/// <summary>
		/// Text and Display
		/// </summary>
		PAN_FAMILY_TEXT_DISPLAY=2,
		/// <summary>
		/// Script
		/// </summary>
		PAN_FAMILY_SCRIPT=3,
		/// <summary>
		/// Decorative
		/// </summary>
		PAN_FAMILY_DECORATIVE=4,
		/// <summary>
		/// Pictorial                      
		/// </summary>
		PAN_FAMILY_PICTORIAL=5
	}

	/// <summary></summary>
	[DocumentationExclude()]
	public class FontUtil
	{
		/// <summary></summary>
		[ThreadStatic]
		private static PrivateFontCollection privateFonts = null;

		/// <summary></summary>
		internal static PrivateFontCollection PrivateFonts
		{
			get
			{
				if( privateFonts == null )
				{
					privateFonts = new PrivateFontCollection();
				}
				return privateFonts;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="s"/>
		[DocumentationExclude()]
		public static FontFamily AddMemoryFont( Stream s )
		{
			byte[ ] buffer = new byte[s.Length];
			s.Read( buffer, 0, (int)s.Length );
			IntPtr myPtr = Marshal.AllocCoTaskMem( buffer.Length );
			Marshal.Copy( buffer, 0, myPtr, buffer.Length );

			return AddMemoryFont( myPtr, buffer.Length );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="memory"/>
		/// <param name="length"/>
		[DocumentationExclude()]
		public static FontFamily AddMemoryFont( IntPtr memory, int length )
		{
			PrivateFonts.AddMemoryFont( memory, length );
			FontFamily[ ] families = privateFonts.Families;
			return families[families.Length - 1];
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="fileName"/>
		[DocumentationExclude()]
		public static FontFamily AddFontFile( string fileName )
		{
			PrivateFonts.AddFontFile( fileName );
			FontFamily[ ] families = privateFonts.Families;
			return families[families.Length - 1];
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="familyName"/>
		[DocumentationExclude()]
		public static FontFamily GetPrivateFont( string familyName )
		{
			if( privateFonts == null )
			{
				return null;
			}

			foreach( FontFamily ff in privateFonts.Families )
			{
				if( ff.Name == familyName )
				{
					return ff;
				}
			}

			return null;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="facename"/>
		/// <param name="size"/>
		[DocumentationExclude()]
		public static Font CreateFont( string facename, float size )
		{
			return CreateFont( facename, size, FontStyle.Regular, GraphicsUnit.Point );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="facename"/>
		/// <param name="size"/>
		/// <param name="fontStyle"/>
		[DocumentationExclude()]
		public static Font CreateFont( string facename, float size, FontStyle fontStyle )
		{
			return CreateFont( facename, size, fontStyle, GraphicsUnit.Point );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="srcFont"/>
		/// <param name="fontStyle"/>
		[DocumentationExclude()]
		public static Font CreateFont( Font srcFont, FontStyle fontStyle )
		{
			Font font = null;

			try
			{
				font = new Font( srcFont, fontStyle );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );

				if( !ExceptionManager.RaiseExceptionCatched( typeof( FontUtil ), ex ) )
				{
					throw;
				}
			}

			if( font == null )
			{
				font = FontUtil.CreateFont( srcFont.Name, srcFont.Size, srcFont.Style, srcFont.Unit );
			}

			return font;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="facename"/>
		/// <param name="size"/>
		/// <param name="fontStyle"/>
		/// <param name="unit"/>
		[DocumentationExclude()]
		public static Font CreateFont( string facename, float size, FontStyle fontStyle, GraphicsUnit unit )
		{
			Font font = null;

			FontFamily privateFontFamily = FontUtil.GetPrivateFont( facename );

			if( privateFontFamily != null )
			{
				font = new Font( privateFontFamily, size, fontStyle, unit );
				if( font != null )
				{
					return font;
				}
			}

			try
			{
				font = new Font( facename, size, fontStyle, unit );
			}
			catch( ArgumentException ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( typeof( FontUtil ), ex ) )
				{
					throw;
				}
			}

			// try regular style
			if( font == null )
			{
				try
				{
					FontFamily ff = new FontFamily( facename );
					if( ff.IsStyleAvailable( FontStyle.Regular ) )
					{
						font = new Font( facename, size, FontStyle.Regular, unit );
					}
					else if( ff.IsStyleAvailable( FontStyle.Bold ) )
					{
						font = new Font( facename, size, FontStyle.Bold, unit );
					}
					else if( ff.IsStyleAvailable( FontStyle.Italic ) )
					{
						font = new Font( facename, size, FontStyle.Italic, unit );
					}
					else if( ff.IsStyleAvailable( FontStyle.Underline ) )
					{
						font = new Font( facename, size, FontStyle.Underline, unit );
					}
				}
				catch( ArgumentException ex )
				{
					TraceUtil.TraceExceptionCatched( ex );
				}
				catch( Exception ex )
				{
					TraceUtil.TraceExceptionCatched( ex );
					if( !ExceptionManager.RaiseExceptionCatched( typeof( FontUtil ), ex ) )
					{
						throw;
					}
				}

				// try different font family
				if( font == null )
				{
					try
					{
						font = new Font( FontFamily.GenericSansSerif, size, fontStyle, unit );
					}
					catch( ArgumentException ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( typeof( FontUtil ), ex ) )
						{
							throw;
						}
					}

					try
					{
						font = new Font( FontFamily.GenericSansSerif, size, FontStyle.Regular, unit );
					}
					catch( ArgumentException ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( typeof( FontUtil ), ex ) )
						{
							throw;
						}
					}
				}
			}

			return font;
		}

		/// <summary>
		/// Gets the <see cref="PanoseFontFamilyTypes"/> for the specified font.
		/// </summary>
		/// <param name="graphics">A graphics object to use when detecting the Panose
		/// family.</param>
		/// <param name="font">The font to check.</param>
		/// <returns>The Panose font family type.</returns>
		public static PanoseFontFamilyTypes PanoseFontFamilyType(
			Graphics graphics, Font font )
		{
			byte bFamilyType = 0;

			IntPtr hdc = graphics.GetHdc();
            try
            {
                IntPtr hFontOld = NativeMethods.SelectObject(hdc, font.ToHfont());

                int bufSize = NativeMethods.GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
                IntPtr lpOtm = Marshal.AllocCoTaskMem(bufSize);
                Marshal.WriteInt32(lpOtm, bufSize);
                int success = NativeMethods.GetOutlineTextMetrics(hdc, bufSize, lpOtm);
                if (success != 0)
                {
                    int offset = 61;
                    bFamilyType = Marshal.ReadByte(lpOtm, offset);
                }

                Marshal.FreeCoTaskMem(lpOtm);

                NativeMethods.SelectObject(hdc, hFontOld);
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
			return (PanoseFontFamilyTypes)bFamilyType;
		}
	}

	/// <summary></summary>
	[DocumentationExclude(),
   ToolboxItem( false )]
	public class ImageListSourceFiles: Component
	{
		/// <summary></summary>
		private ImageList il;
		/// <summary></summary>
		private ArrayListExt sourceFilesList;
		/// <summary></summary>
		private string baseDir = String.Empty;

		/// <summary></summary>
		public ImageListSourceFiles()
		{
			this.sourceFilesList = new ArrayListExt();
			this.sourceFilesList.CollectionChanged += new CollectionChangeEventHandler( this.Collection_Changed );
		}

		/// <summary></summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.sourceFilesList.CollectionChanged -= new CollectionChangeEventHandler( this.Collection_Changed );
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Gets / sets the ImageList to wrap around.
		/// </summary>
		[Description( "The ImageList to wrap around." )]
		public ImageList ImageList
		{
			get
			{
				return il;
			}
			set
			{
				il = value;
			}
		}

		/// <summary>
		/// Gets/ sets the base directory from which the images will be added.
		/// </summary>
		/// <remarks>
		/// This helps to maintain a relative path in the SourceFiles list.
		/// </remarks>
		public string BaseDirectory
		{
			get
			{
				return this.baseDir;
			}
			set
			{
				value = value.Trim( '\\', '/' );
				if( this.baseDir != value )
				{
					value = value.ToLower( CultureInfo.CurrentUICulture );
					if( value.Length == 0 || Directory.Exists( value ) )
					{
						this.RemoveRelativePaths();
						this.baseDir = value;
						this.ApplyRelativePaths();
					}
					else if( this.DesignMode )
					{
						MessageBox.Show( value + " is not a valid directory." );
					}
				}
			}
		}

		/// <summary></summary>
		protected virtual void RemoveRelativePaths()
		{
			if( this.baseDir.Length == 0 )
			{
				return;
			}

			this.sourceFilesList.SuspendEvents();

			for( int i = 0; i < this.sourceFilesList.Count; i++ )
			{
				string relativeFilePath = (string)this.sourceFilesList[i];
				if( relativeFilePath.IndexOf( ":" ) == -1 )
				{
					this.sourceFilesList[i] = this.baseDir + "\\" + relativeFilePath;
				}
			}
			this.sourceFilesList.ResumeEvents( false );
		}

		/// <summary></summary>
		protected virtual void ApplyRelativePaths()
		{
			this.sourceFilesList.SuspendEvents();
			for( int i = 0; i < this.sourceFilesList.Count; i++ )
			{
				string originalPath = (string)this.sourceFilesList[i];
				if( originalPath.IndexOf( this.baseDir ) == 0 )
				{
					string relativePath = originalPath.Substring( this.baseDir.Length );
					relativePath = relativePath.Trim( '\\', '/' );
					this.sourceFilesList[i] = relativePath;
				}
			}
			this.sourceFilesList.ResumeEvents( false );
		}

		/// <summary>
		/// Returns the source of the images in the underlying ImageList.
		/// </summary>
		[Editor( typeof( SourceImageFilesEditor ), typeof( UITypeEditor ) ),
	   Description( "Returns the source of the Images in the underlying ImageList. Bind this object to an ImageList before editing this property." ),
	   DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public ArrayListExt SourceFiles
		{
			get
			{
				return this.sourceFilesList;
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void Collection_Changed( object sender, CollectionChangeEventArgs e )
		{
			this.OnSourceFilesChanged();
		}

		/// <summary></summary>
		protected virtual void OnSourceFilesChanged()
		{
			this.ApplyRelativePaths();
			this.il.Images.Clear();
			foreach( string fileName in this.sourceFilesList )
			{
				string sourceFile = fileName;
				if( sourceFile.IndexOf( ":" ) == -1 )
				{
					sourceFile = this.baseDir + "\\" + sourceFile;
				}
				if( File.Exists( sourceFile ) )
				{
					if( sourceFile.IndexOf( ".ico" ) != -1 )
					{
						Icon icon = new Icon( sourceFile );
						this.il.Images.Add( icon );
					}
					else
					{
						this.il.Images.Add( Bitmap.FromFile( sourceFile ) );
					}
				}
			}
		}

		/// <summary></summary>
		internal IDesignerHost Designer
		{
			get
			{
				return this.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
			}
		}
	}

	/// <summary></summary>
	[DocumentationExclude()]
	public class SourceImageFilesEditor:
	  CollectionEditor
	{
		// Constructors

		/// <summary><para>Initializes a new instance of the <see cref="SourceImageFilesEditor"/> class.</para></summary>
		/// <param name="type">The type of the collection to edit.</param>
		public SourceImageFilesEditor( Type type )
			: base( type )
		{
		}

		// Methods

		/// <summary><para>Creates an instance of the specified type in the collection.</para></summary>
		/// <param name="type">The type of the image to insert in the collection.</param>
		/// <returns></returns>
		protected override object CreateInstance( Type type )
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			//openFileDialog1.InitialDirectory = "c:\\" ;
			//openFileDialog1.Filter = "Icon files (*.Ico)|*.Ico" ;
			openFileDialog1.Filter = "All Image Files(*.bmp,*.gif,*.jpg,*.jpeg,*.png,*.ico)|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.ico";
			openFileDialog1.FilterIndex = 1;
			openFileDialog1.RestoreDirectory = true;
			openFileDialog1.Multiselect = false;

			if( openFileDialog1.ShowDialog() == DialogResult.OK )
			{
				return openFileDialog1.FileName.ToLower( CultureInfo.CurrentUICulture );
			}
			return null;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="editValue"/>
		/// <param name="value"/>
		protected override object SetItems( object editValue, object[] value )
		{
			ArrayListExt sourceFilesCollection = editValue as ArrayListExt;
			sourceFilesCollection.SuspendEvents();
			sourceFilesCollection.Clear();

			if( value.Length > 0 )
			{
				foreach( object obj in value )
				{
					sourceFilesCollection.Add( (string)obj );
				}
			}
			sourceFilesCollection.ResumeEvents( true );
			return editValue;
		}
	}

	/// <summary>
	/// Apply functionality to choose alpha-blended icon and correctly change it to bitmap.
	/// </summary>
	public class DrawIconHelper
	{
		/// <summary>
		/// Stores info about drawing icons.
		/// </summary>
		protected static Hashtable m_drawIcons = new Hashtable();

		//possible icon Width or Height 
		/// <summary></summary>
		private static readonly int[] c_masIconWidth = new int[] { 16, 24, 32, 48, 64, 128, 256 };

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="icon"/>
		/// <param name="relatedObject"/>
		static public Image GetIconToDraw( Icon icon, object relatedObject )
		{
			bool bIsAlphablended = false;
			int iconWidth = 0;

			if( !m_drawIcons.ContainsKey( relatedObject ) )
			{
				int startIndex = 0;

				for( int i = c_masIconWidth.Length - 1; i > 0; i-- )
				{
					if( c_masIconWidth[i] == icon.Width )
					{
						startIndex = i;
					}
				}

				for( int k = startIndex; !bIsAlphablended && k >= 0; k-- )
				{
					iconWidth = c_masIconWidth[k];

					Icon ico = new Icon( icon, iconWidth, iconWidth );
					NativeMethods.ICONINFO ii = new NativeMethods.ICONINFO();
					NativeMethods.BITMAP bm = new NativeMethods.BITMAP();

					NativeMethods.GetIconInfo( ico.Handle, out ii );
					NativeMethods.GetObject( ii.hbmColor, Marshal.SizeOf( bm ), out bm );

					if( bm.bmBitsPixel * bm.bmPlanes == 32 )
					{
						Bitmap fixedBmp = IconToBitmap( ii );
						Color color;

						for( int i = 0; !bIsAlphablended && i < fixedBmp.Height; i++ )
						{
							for( int j = 0; j < fixedBmp.Width; j++ )
							{
								color = fixedBmp.GetPixel( i, j );

								if( color.A != 0 )
								{
									iconWidth = Convert.ToInt32( bm.bmWidth );
									bIsAlphablended = true;
									break;
								}
							}
						}

						fixedBmp.Dispose();
					}

					NativeMethods.DeleteObject( ii.hbmColor );
					NativeMethods.DeleteObject( ii.hbmMask );

					ico.Dispose();
				}

				m_drawIcons.Add( relatedObject, new IconDrawInfo( bIsAlphablended, iconWidth ) );

				Component component = relatedObject as Component;

				if( null != component )
				{
					component.Disposed += new EventHandler( component_Disposed );
				}
			}
			else
			{
				IconDrawInfo fixBitmap = m_drawIcons[relatedObject] as IconDrawInfo;

				bIsAlphablended = fixBitmap.bIsAlphablended;
				iconWidth = fixBitmap.nIconWidth;
			}

			Icon drawIcon = new Icon( icon, iconWidth, iconWidth );
			Image drawImage;

			if( bIsAlphablended )
			{
				try
				{
					NativeMethods.ICONINFO ii = new NativeMethods.ICONINFO();
					NativeMethods.GetIconInfo( drawIcon.Handle, out ii );
					Bitmap drawbm = IconToBitmap( ii );

					drawImage = drawbm;

					NativeMethods.DeleteObject( ii.hbmColor );
					NativeMethods.DeleteObject( ii.hbmMask );
				}
				catch
				{
					drawImage = drawIcon.ToBitmap();
				}
			}
			else
			{
				drawImage = drawIcon.ToBitmap();
			}

			drawIcon.Dispose();

			return drawImage;
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private static void component_Disposed( object sender, EventArgs e )
		{
			Component component = sender as Component;

			if( null != component )
			{
				if( m_drawIcons.ContainsKey( component ) )
				{
					component.Disposed -= new EventHandler( component_Disposed );

					m_drawIcons.Remove( component );
				}
			}
		}

		/// <summary></summary>
		internal class IconDrawInfo
		{
			/// <summary></summary>
			public readonly bool bIsAlphablended;
			/// <summary></summary>
			public readonly int nIconWidth;

			/// <summary></summary>
			/// <param name="alphaBlended"/>
			/// <param name="width"/>
			public IconDrawInfo( bool alphaBlended, int width )
			{
				this.bIsAlphablended = alphaBlended;
				this.nIconWidth = width;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="ii"/>
		private static Bitmap IconToBitmap( NativeMethods.ICONINFO ii )
		{
			IntPtr hBitmap = ii.hbmColor;
			Bitmap bmp = Bitmap.FromHbitmap( hBitmap );
			Bitmap fixedBmp = FixAlphaBitmap( bmp );

			return fixedBmp;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="bmSource"/>
		private static Bitmap FixAlphaBitmap( Bitmap bmSource )
		{
			Rectangle bmBounds = new Rectangle( Point.Empty, bmSource.Size );
			BitmapData bmData = bmSource.LockBits( bmBounds, ImageLockMode.ReadWrite, bmSource.PixelFormat );
			Bitmap destBitmap = new Bitmap( bmData.Width, bmData.Height, bmData.Stride, PixelFormat.Format32bppArgb, bmData.Scan0 );

			bmSource.UnlockBits( bmData );

			return destBitmap;
		}
	}

	/// <summary>
	/// Utility class for mirroring graphics output if needed.
	/// </summary>
	/// <internalonly/>
	[DocumentationExclude()]
	public sealed class CMirroredDrawer: IDisposable
	{
		#region Class constants
		/// <summary>
		/// Value of advanced graphics state.
		/// </summary>
		private const int DEF_ADVANCED_GR_STATE = 2;
		/// <summary>
		/// Mode of transformation matrix multiplication.
		/// </summary>
		private const int DEF_MULTI_MODE = 2;
		#endregion

		#region Class members
		/// <summary>
		/// Graphics object for target context.
		/// </summary>
		private Graphics m_gfxTarget = null;
		/// <summary>
		/// Temporary virtual Graphics object.
		/// </summary>
		private Graphics m_gfxCanvas = null;
		/// <summary>
		/// Temporary bitmap object.
		/// </summary>
		private Bitmap m_bmpCanvas = null;
		/// <summary>
		/// Target rectangle. 
		/// </summary>
		private Rectangle m_rectTargetBounds;
		/// <summary>
		/// Rectangle of virtual temporary area.
		/// </summary>
		private Rectangle m_rectCanvas;
		/// <summary>
		/// Target graphics path.
		/// </summary>
		private GraphicsPath m_pathCanvas = null;
		/// <summary>
		/// Indicates whether image must be mirrored.
		/// </summary>
		private bool m_bDrawMirrored = false;
		/// <summary>
		/// Indicates whether OS supports needed API functions.
		/// </summary>
		private bool m_bOSSupports;
		/// <summary>
		/// Holds old value of source graphics mode.
		/// </summary>
		private int m_oldGraphicsMode;
		/// <summary>
		/// Holds old value of source graphics transformation.
		/// </summary>
		private Matrix m_oldMatrix;
		/// <summary>
		/// Handle wrapper of graphics object.
		/// </summary>
		private HandleRef m_gfxHandleWrap;
		/// <summary>
		/// Handle of graphics object.
		/// </summary>
		private IntPtr m_gfxHandle;
		#endregion

		#region Class properties
		/// <summary>
		/// Returns virtual graphics context object.
		/// </summary>
		public Graphics VirtualGfx
		{
			get
			{
				return this.NeedBitmap ? m_gfxCanvas : m_gfxTarget;
			}
		}
		/// <summary>
		/// Returns virtual rectangle.
		/// </summary>
		public Rectangle VirtualBounds
		{
			get
			{
				return m_rectCanvas;
			}
		}
		/// <summary>
		/// Returns virtual Graphics path object.
		/// </summary>
		public GraphicsPath VirtualPath
		{
			get
			{
				return m_pathCanvas;
			}
		}
		/// <summary>
		/// Indicates whether additional bitmap is needed.
		/// </summary>
		private bool NeedBitmap
		{
			get
			{
				return ( !m_bOSSupports && m_bDrawMirrored );
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Private constructor.
		/// </summary>
		private CMirroredDrawer()
		{
			m_bOSSupports = IsSupportedByOS();
		}

		/// <summary>
		/// Overloaded. Creates a new mirror object for drawing mirrored images.
		/// </summary>
		/// <param name="gfxTarget">Source graphics object.</param>
		/// <param name="pathTarget">Source graphics path object.</param>
		/// <param name="bDrawMirrored">If True  mirror output; False otherwise.</param>
		public CMirroredDrawer( Graphics gfxTarget, GraphicsPath pathTarget, bool bDrawMirrored )
			: this()
		{
			if( gfxTarget == null )
			{
				throw new ArgumentNullException( "gfxTarget" );
			}

			if( pathTarget == null )
			{
				throw new ArgumentNullException( "pathTarget" );
			}

			Rectangle rectTarget = Rectangle.Ceiling( pathTarget.GetBounds() );

			Matrix matrIdentity = new Matrix();
			matrIdentity.Translate( 0, -rectTarget.Top );

			m_pathCanvas = pathTarget.Clone() as GraphicsPath;
			m_pathCanvas.Transform( matrIdentity );

			Initialize( gfxTarget, rectTarget, bDrawMirrored );
		}

		/// <summary>
		/// Creates a new mirror object for drawing mirrored images.
		/// </summary>
		/// <param name="gfxTarget">Source graphics object.</param>
		/// <param name="rectBounds">Source rectangle structure.</param>
		/// <param name="bDrawMirrored">If True mirror output; False otherwise.</param>
		public CMirroredDrawer( Graphics gfxTarget, Rectangle rectBounds, bool bDrawMirrored )
			: this()
		{
			if( gfxTarget == null )
			{
				throw new ArgumentNullException( "gfxTarget" );
			}

			Initialize( gfxTarget, rectBounds, bDrawMirrored );
		}

		/// <summary>
		/// Overloaded ctor.. as a workaround for Themed Mirrored rendering issue.
		/// <para/>
		/// Creates a new mirror object for drawing mirrored images.
		/// </summary>
		/// <param name="gfxTarget">Source graphics object.</param>
		/// <param name="rectBounds">Source rectangle structure.</param>
		/// <param name="bDrawMirrored">If True mirror output; False otherwise.</param>
		/// <param name="bOSSupports">False as a workaround when drawing themed mirrored graphics.</param>
		public CMirroredDrawer( Graphics gfxTarget, Rectangle rectBounds, bool bDrawMirrored, bool bOSSupports )
		{
			m_bOSSupports = bOSSupports;

			if( gfxTarget == null )
			{
				throw new ArgumentNullException( "gfxTarget" );
			}

			Initialize( gfxTarget, rectBounds, bDrawMirrored );

		}

		/// <summary>
		/// Disposes all resources, but first paints all data 
		/// from the buffer to source device context.
		/// </summary>
		public void Dispose()
		{
			Flush();

			// Clean up resources.
			if( m_gfxCanvas != null )
			{
				m_gfxCanvas.Dispose();
				m_gfxCanvas = null;
			}

			if( m_bmpCanvas != null )
			{
				m_bmpCanvas.Dispose();
				m_bmpCanvas = null;
			}

			if( m_oldMatrix != null )
			{
				m_oldMatrix.Dispose();
				m_oldMatrix = null;
			}

			m_gfxTarget = null;
			m_pathCanvas = null;

			GC.SuppressFinalize( this );
		}
		#endregion

		#region Class Utility methods
		/// <summary>
		/// Initializes an object.
		/// </summary>
		/// <param name="gfxTarget">Source graphics object.</param>
		/// <param name="rectBounds">Source rectangle structure.</param>
		/// <param name="bDrawMirrored">If True mirror output; False otherwise.</param>
		private void Initialize( Graphics gfxTarget, Rectangle rectBounds, bool bDrawMirrored )
		{
			if( gfxTarget == null )
			{
				throw new ArgumentNullException( "gfxTarget" );
			}

			m_gfxTarget = gfxTarget;
			m_bDrawMirrored = bDrawMirrored;
			m_rectTargetBounds = rectBounds;
			m_rectCanvas = rectBounds;

			// Create bitmap if it is needed.
			if( this.NeedBitmap )
			{
				if( rectBounds.Width > 0 && rectBounds.Height > 0 )
				{
					m_bmpCanvas = new Bitmap( rectBounds.Width, rectBounds.Height, m_gfxTarget );
				}

				m_gfxCanvas = ( null != m_bmpCanvas ) ? Graphics.FromImage( m_bmpCanvas ) : m_gfxTarget;

				m_rectCanvas.X = 0;
				m_rectCanvas.Y = 0;
			}
			else if( m_bOSSupports && m_bDrawMirrored )
			{
				TransformGraphics();
			}
		}

		/// <summary>
		/// Checks OS Version for API functions support.
		/// Need NT 3.1 or later.
		/// </summary>
		/// <returns>True if current OS is NT 3.1 or later; False otherwise.</returns>
		private bool IsSupportedByOS()
		{
			OperatingSystem curOS = Environment.OSVersion;
			bool result = false;

			bool bNumberGreater = ( ( curOS.Version.Major > 3 ) ||
		( curOS.Version.Major == 3 && curOS.Version.Minor >= 1 ) );

			if( curOS.Platform == PlatformID.Win32NT && bNumberGreater )
			{
				result = true;
			}

			return result;
		}

		/// <summary>
		/// Transforms graphics.
		/// </summary>
		private void TransformGraphics()
		{
			if( m_bOSSupports && m_bDrawMirrored )
			{
				// Transform coordinates for mirroring.
				// Save old state.
				m_oldMatrix = m_gfxTarget.Transform;

				m_gfxHandle = m_gfxTarget.GetHdc();
                float em1 = 0f;
                float xOffset = 0f;
                try
                {
                    m_gfxHandleWrap = new HandleRef(m_gfxTarget, m_gfxHandle);

                    m_oldGraphicsMode = NativeMethods.SetGraphicsMode(m_gfxHandleWrap,
                      DEF_ADVANCED_GR_STATE);

                    // Graphics mode is set.
                    if (m_oldGraphicsMode != 0)
                    {
                        // Define transformation matrix.
                        em1 = m_bDrawMirrored ? -1.0f : 1.0f;
                        xOffset = m_bDrawMirrored ?
                2 * m_rectTargetBounds.X + m_rectTargetBounds.Width : 0;

                        NativeMethods.XFORM form = new NativeMethods.XFORM();
                        form.eM11 = em1;
                        form.eM12 = 0.0f;
                        form.eM21 = 0.0f;
                        form.eM22 = 1.0f;
                        form.eDx = xOffset;
                        form.eDy = 0.0f;
                       
                        NativeMethods.ModifyWorldTransform(HandleRef.ToIntPtr(m_gfxHandleWrap), ref form, DEF_MULTI_MODE);
                    }
                }
                finally
                {
                    m_gfxTarget.ReleaseHdc(m_gfxHandle);
                    Matrix newMatrix = new Matrix(em1, 0.0f, 0.0f, 1.0f, xOffset, 0.0f);
                    m_gfxTarget.MultiplyTransform(newMatrix);
                }
			}
		}

		/// <summary>
		/// Restores graphics to its previous state.
		/// </summary>
		private void RestoreGraphics()
		{
			if( m_bOSSupports && m_bDrawMirrored && m_oldGraphicsMode != 0 )
			{
				NativeMethods.XFORM form = new NativeMethods.XFORM();
				form.eM11 = m_oldMatrix.Elements[0];
				form.eM12 = m_oldMatrix.Elements[1];
				form.eM21 = m_oldMatrix.Elements[2];
				form.eM22 = m_oldMatrix.Elements[3];
				form.eDx = m_oldMatrix.Elements[4];
				form.eDy = m_oldMatrix.Elements[5];

                NativeMethods.SetWorldTransform(HandleRef.ToIntPtr(m_gfxHandleWrap), ref  form);
				NativeMethods.SetGraphicsMode( m_gfxHandleWrap, m_oldGraphicsMode );
				m_gfxTarget.Transform = m_oldMatrix;
			}
		}

		/// <summary>
		/// Flushes all drawing data to destination if needed.
		/// </summary>
		private void Flush()
		{
			// Transform graphics and write bitmap.
			if( this.NeedBitmap )
			{
				if( m_bmpCanvas != null )
				{
					Matrix mtrxBack = m_gfxTarget.Transform;

					Matrix mtrxMult = new Matrix( -1, 0, 0, 1,
					  m_rectTargetBounds.Right + m_rectTargetBounds.Left, 0 );

					m_gfxTarget.MultiplyTransform( mtrxMult );
					m_gfxTarget.DrawImage( m_bmpCanvas, m_rectTargetBounds );
					m_gfxTarget.Transform = mtrxBack;
				}
			}
			else if( m_bOSSupports )
			{
				// Restore graphics.
				RestoreGraphics();
			}
		}
		#endregion
	}

	/// <summary><para>
	/// Class does 2D ratio and offset tranformation for <see cref="Graphics"/> object using <see cref="Graphics.Transform"/> property.
	/// </para><para>
	/// Transform is done in constructor and reverted in Dispose() method.
	/// </para></summary>
	/// <remarks>
	/// Supposed to be used with <see cref="using"/> keyword.
	/// </remarks>
	public class GraphicsAutoAfineTransfrom:
	  IDisposable
	{
		#region Private Data
		/// <summary></summary>
		private Graphics m_gph = null;
		/// <summary></summary>
		private Matrix m_matrixPrev = null;
		#endregion

		#region Construction
		/// <summary></summary>
		/// <param name="gph"/>
		/// <param name="fRatioX"/>
		/// <param name="fRatioY"/>
		/// <param name="fOffsetX"/>
		/// <param name="fOffsetY"/>
		public GraphicsAutoAfineTransfrom( Graphics gph, float fRatioX, float fRatioY, float fOffsetX, float fOffsetY )
		{
			if( null != gph )
			{
				m_gph = gph;
				m_matrixPrev = gph.Transform;

				m_gph.MultiplyTransform( new Matrix( fRatioX, 0, 0, fRatioY, fOffsetX, fOffsetY ) );
			}
		}
		#endregion

		#region IDisposable implementation
		/// <summary></summary>
		void IDisposable.Dispose()
		{
			if( null != m_gph && null != m_matrixPrev )
			{
				m_gph.Transform = m_matrixPrev;

				m_matrixPrev = null;
				m_gph = null;
			}
		}
		#endregion
	}

	/// <summary><para>
	/// Class does 2D mirroring for X axis for <see cref="Graphics"/>.
	/// </para><para>
	/// Transform is done in constructor and reverted in Dispose() method.
	/// </para></summary>
	/// <remarks>
	/// Supposed to be used with <see cref="using"/> keyword.
	/// </remarks>
	public class GraphicsAutoMirrorX:
	  GraphicsAutoAfineTransfrom
	{
		#region Construction
		/// <summary></summary>
		/// <param name="gph"/>
		/// <param name="fOffsetX"/>
		/// <param name="fOffsetY"/>
		public GraphicsAutoMirrorX( Graphics gph, float fOffsetX, float fOffsetY ) :
			base( gph, -1f, 1f, fOffsetX, fOffsetY )
		{
		}

		/// <summary></summary>
		/// <param name="gph"/>
		/// <param name="fOffsetX"/>
		public GraphicsAutoMirrorX( Graphics gph, float fOffsetX ) :
			this( gph, fOffsetX, 0f )
		{
		}
		#endregion
	}

	/// <summary><para>
	/// Class does 2D mirroring for Y axis for <see cref="Graphics"/>.
	/// </para><para>
	/// Transform is done in constructor and reverted in Dispose() method.
	/// </para></summary>
	/// <remarks>
	/// Supposed to be used with <see cref="using"/> keyword.
	/// </remarks>
	public class GraphicsAutoMirrorY:
	  GraphicsAutoAfineTransfrom
	{
		#region Construction
		/// <summary></summary>
		/// <param name="gph"/>
		/// <param name="fOffsetX"/>
		/// <param name="fOffsetY"/>
		public GraphicsAutoMirrorY( Graphics gph, float fOffsetX, float fOffsetY ) :
			base( gph, 1f, -1f, fOffsetX, fOffsetY )
		{
		}

		/// <summary></summary>
		/// <param name="gph"/>
		/// <param name="fOffsetY"/>
		public GraphicsAutoMirrorY( Graphics gph, float fOffsetY ) :
			this( gph, 0f, fOffsetY )
		{
		}
		#endregion
	}

	public class TextRendererDC:
		IDeviceContext,
		IDisposable
	{
		#region Native methods

		private const int GM_ADVANCED = 2;

		[DllImport( "Gdi32" )]
		private static extern int SetGraphicsMode( IntPtr hdc, int mode );
		[DllImport( "Gdi32" )]
		private static extern bool SetWorldTransform( IntPtr hDC, ref XFORM xform );
		[DllImport( "Gdi32" )]
		private static extern bool GetWorldTransform( IntPtr hdc, ref XFORM xform );
		[DllImport( "Gdi32" )]
		private static extern bool ModifyWorldTransform( IntPtr hdc, ref XFORM xform, int iMode );
		[DllImport( "Gdi32" )]
		private static extern int SelectClipRgn( IntPtr hDC, IntPtr hRgn );
		[DllImport( "gdi32.dll" )]
		static extern int SaveDC( IntPtr hdc );
		[DllImport( "gdi32.dll" )]
		static extern bool RestoreDC( IntPtr hdc, int nSavedDC );

		[StructLayout( LayoutKind.Sequential, CharSet=CharSet.Auto )]
		private struct XFORM
		{
			public float eM11;
			public float eM12;
			public float eM21;
			public float eM22;
			public float eDx;
			public float eDy;

			public XFORM( float[] elements )
			{
				eM11 = elements[0];
				eM12 = elements[1];
				eM21 = elements[2];
				eM22 = elements[3];
				eDx  = elements[4];
				eDy  = elements[5];
			}
		}

		#endregion

		#region Fields
		
		private Graphics _g;
		private IntPtr _dc;
		IntPtr _hClipRgn;
		private int _savedDC;
		
		#endregion

		#region Construction
		
		private TextRendererDC()
		{
		}

		public TextRendererDC( Graphics g )
		{
			_g = g;
		}

		#endregion

		#region IDeviceContext Members

		public IntPtr GetHdc()
		{
			XFORM xform;			

			using( Matrix transf = _g.Transform )
			{
				xform = new XFORM( transf.Elements );
			}

			using( Region clip = _g.Clip )
			{
				_hClipRgn = clip.GetHrgn( _g );
			}

			_dc = _g.GetHdc();

			_savedDC = SaveDC( _dc );

			SetTransform( ref xform );
			SetClip( _hClipRgn );

			return _dc;
		}

		public void ReleaseHdc()
		{
			if( _dc != IntPtr.Zero )
			{
				RestoreDC( _dc, _savedDC );
				_g.ReleaseHdc();
				NativeMethods.DeleteObject( _hClipRgn );

				_dc = _hClipRgn = IntPtr.Zero;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			ReleaseHdc();
		}

		#endregion

		private void SetTransform( ref XFORM xform )
		{
			SetGraphicsMode( _dc, GM_ADVANCED );
			SetWorldTransform( _dc, ref xform );
		}

		private void SetClip( IntPtr hRegion )
		{
			SelectClipRgn( _dc, hRegion );
		}
	}
}