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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

using Syncfusion.Diagnostics;

namespace Syncfusion.Drawing
{
	/// <summary>
	/// IconPaint is a helper class for drawing and caching bitmaps from a resource manifest with a given forecolor.
	/// </summary>
	/// <remarks>
	/// The bitmaps are loaded from the manifest and cached. The PaintIcon routine
	/// will substitute black pixels of the original bitmap and draw them with a 
	/// specified forecolor.
	/// </remarks>
	public class IconPaint
	{
		// Example usage:
//		/// <summary>
//		/// GridDataBoundIconPaint is a helper class for drawing and caching bitmaps from the DataBound.Resources folder of the Grid assembly
//		/// </summary>
//		/// <remarks>
//		/// The bitmaps are loaded from the manifest and cached. The PaintIcon routine
//		/// will substitute black pixels of the original bitmap and draw them with a 
//		/// specified forecolor.<para/>
//		/// Example:<para/>
//		///	GridDataBoundIconPaint.Paint.PaintIcon(g, rect, Point.Empty, bitmapName, Color.Black);
//		/// </remarks>
//		public class GridDataBoundIconPaint 
//		{
//			[ThreadStaticAttribute] static IconPaint gridPainter;
//
//			internal static IconPaint Paint
//			{
//				get
//				{
//					if (gridPainter == null)
//						gridPainter = new IconPaint(GridAssembly.RootNamespace + @".DataBound\Resources.", GridAssembly.Assembly);
//					return gridPainter;
//				}
//			}
//		}

		/// <summary>
		/// The bitmap cache.
		/// </summary>
		private Hashtable bitmaps = new Hashtable();

		/// <summary>
		/// The manifest to load from. The bitmaps should be saved in the Resources
		/// tree in the Visual Studio project with the build action set to "Embedded Resource".
		/// </summary>
		private string manifestPrefix;

		/// <summary>
		/// The assembly to load from. The bitmaps should be saved in the Resources
		/// tree in Visual Studio project with the build action set to "Embedded Resource".
		/// </summary>
		private Assembly assembly;

		/// <summary>
		/// Initializes a new <see cref="IconPaint"/> object with manifestPrefix and a reference to the assembly
		/// to load bitmaps from. You should save this object in a static variable.
		/// </summary>
		/// <param name="manifestPrefix"> The manifest to load from. The bitmaps should be saved in the Resources
		/// tree in the Visual Studio project with the build action set to "Embedded Resource".</param>
		/// <param name="ass">The assembly to load from. The bitmaps should be saved in the Resources
		/// tree in Visual Studio project with the build action set to "Embedded Resource".</param>
		public IconPaint(string manifestPrefix, Assembly ass)
		{
			if (ass == null)
				throw new ArgumentNullException("assembly");
			this.manifestPrefix = manifestPrefix;
			this.assembly = ass;
		}

		/// <summary>
		/// Loads bitmap from manifest.
		/// </summary>
		/// <param name="bitmapName">The bitmap name.</param>
		/// <returns>Reference to bitmap; NULL if bitmap failed to load.</returns>
		private Bitmap _GetBitmap(string bitmapName)  
		{
			Bitmap bitmap = null;

			try
			{
				Stream stream = assembly.GetManifestResourceStream(manifestPrefix + bitmapName);
				bitmap = new Bitmap(stream);
				if (bitmap != null)
					bitmap.MakeTransparent();
			}  
			catch(Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
//				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
//					throw ex;
			}  

			return bitmap;
		}

		/// <summary>
		/// Returns bitmap from cache or loads bitmap from manifest on first use.
		/// </summary>
		/// <param name="bitmapName">The bitmap name.</param>
		/// <returns>Reference to bitmap; NULL if bitmap failed to load.</returns>
		public Bitmap GetBitmap(string bitmapName)  
		{
			if (!bitmaps.Contains(bitmapName))
				bitmaps[bitmapName] = _GetBitmap(bitmapName);
			
			return bitmaps[bitmapName] as Bitmap;
		}

		/// <summary>
		/// Calculates coordinates for a centered rectangle.
		/// </summary>
		/// <param name="rect">The existing bounds.</param>
		/// <param name="size">The size of the rectangle to be centered.</param>
		/// <returns>A rectangle inside the specified bounds.</returns>
		public Rectangle CenterInRect(Rectangle rect, Size size)
		{
			int dx = 0;
			if (size.Width < rect.Width)
				dx = rect.Width-size.Width+1;

			int dy = 0;
			if (size.Height < rect.Height)
				dy = rect.Height-size.Height+1;

			return new Rectangle(rect.Left+dx/2, rect.Top+dy/2, 
				Math.Min(size.Width, rect.Width), Math.Min(size.Height, rect.Height));
		}

		/// <summary>
		/// Paints the specified bitmap substituting black pixels with a new color.
		/// </summary>
		/// <param name="g">A Graphics object used to draw the bitmap.</param>
		/// <param name="bounds">A Rectangle which contains the boundary data of the rectangle.</param>
		/// <param name="offset">A Point that specifies pixels to offset the bitmap from its origin point.</param>
		/// <param name="bmp">The bitmap to be drawn on the screen.</param>
		/// <param name="foreColor">The new color used to substitute black pixels.</param>
		/// <returns>A Rectangle which contains the boundary data of the drawn bitmap.</returns>
		/// <remarks>
		/// The PaintIcon routine
		/// will substitute black pixels of the original bitmap and draw them with the
		/// specified forecolor. The bitmap is centered inside the specified bounds. 
		/// Use the offset if you want to display a "pressed button" state. If the button is
		/// pressed, specify offset = new Point(1, 1).
		/// </remarks>
		public Rectangle PaintIcon(Graphics g, Rectangle bounds, Point offset, Bitmap bmp, Color foreColor)  
		{
			Size size = bmp.Size;
			Rectangle rect;
			ImageAttributes imageAttributes;

			rect = CenterInRect(bounds, size);
			rect.Offset(offset);

			ColorMap colorMap = new ColorMap();
			colorMap.NewColor = foreColor;
			colorMap.OldColor = Color.Black;
			imageAttributes = new ImageAttributes();
			imageAttributes.SetRemapTable(new ColorMap[] { colorMap }, ColorAdjustType.Bitmap);
			g.DrawImage((Image) bmp, rect, 0, 0, rect.Width, rect.Height,
				GraphicsUnit.Pixel, imageAttributes);
			imageAttributes.Dispose();

			return rect;
		}

		/// <summary>
		/// Loads the bitmap from the manifest and paints it substituting black pixels with a new color.
		/// </summary>
		/// <param name="g">A Graphics object used to draw the bitmap.</param>
		/// <param name="bounds">A Rectangle which contains the boundary data of the rectangle.</param>
		/// <param name="offset">A Point that specifies pixel to offset the bitmap from its origin point.</param>
		/// <param name="bitmapName">The name of the bitmap.</param>
		/// <param name="foreColor">The new color used to substitute black pixels.</param>
		/// <returns>A Rectangle which contains the boundary data of the drawn bitmap.</returns>
		/// <remarks>
		/// The PaintIcon routine
		/// will substitute black pixels of the original bitmap and draw them with the
		/// specified forecolor. The bitmap is centered inside the specified bounds. 
		/// Use the offset if you want to display a "pressed button" state. If the button is
		/// pressed, specify offset = new Point(1, 1).
		/// </remarks>
		public void PaintIcon(Graphics g, Rectangle bounds, Point offset, string bitmapName, Color foreColor)
		{
			// Draw arrow inside the button.
			Bitmap bitmap = GetBitmap(bitmapName);
			if (bitmap != null)
				PaintIcon(g, bounds, offset, bitmap, foreColor);
		}
	}
}
