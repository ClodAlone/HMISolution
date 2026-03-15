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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace Syncfusion.Windows.Forms
{

	/// <summary>
	/// An identifier for various arrow buttons in a <see cref="ArrowButtonBar"/>.
	/// </summary>
    [
	Flags,
	]
    public enum ArrowType
    {
		/// <summary>
		/// None.
		/// </summary>
		None     = 0x00,
		/// <summary>
		/// The "previous item" button.
		/// </summary>
		Previous = 0x01,
		/// <summary>
		/// The "next item" button.
		/// </summary>
		Next     = 0x02,
		/// <summary>
		/// The "first item" button.
		/// </summary>
		First    = 0x04,
		/// <summary>
		/// The "last item" button.
		/// </summary>
		Last     = 0x08,
		/// <summary>
		/// The "AddNew item" button for items in a table.
		/// </summary>
		AddNew   = 0x10,
		/// <summary>
		/// All buttons.
		/// </summary>
		[Browsable(false)]
		All      = 0xff
	}
	
	/// <summary>
	/// Helper routines for drawing arrow buttons.
	/// </summary>
    public sealed class ArrowPaint
    {
        private ArrowPaint() {}

        [ThreadStatic] static Bitmap starBmp = null;
    	
		private static System.Drawing.Imaging.ColorMap[] colorMap = new ColorMap[1]
            {
                new ColorMap()
            };

		// TODO: Workbook Resources
		static string manifestPrefix = AssemblyInfo.RootNamespace + @".RecordNavigate.";

        static Bitmap GetBitmap(string bitmapName)  
        {
            Bitmap bitmap = null;

            try
            {
                Type type = typeof(ArrowPaint);
                Stream stream = type.Module.Assembly.GetManifestResourceStream(manifestPrefix + bitmapName);
                bitmap = new Bitmap(stream);
                if (bitmap != null)
                    bitmap.MakeTransparent();
            }  
            catch(System.Exception exception)
            {
				Debug.WriteLine("Unable to load bitmap " + bitmapName);
				Debug.WriteLine(exception.ToString());
                throw;
            }  

            return bitmap;
        }

	    static Bitmap GetStarBitmap()  
	    {
            // "borrow" start bitmap from DataGrid
            if (ArrowPaint.starBmp == null) 
                ArrowPaint.starBmp = GetBitmap(@"ArrowPaint.star.bmp");

            return ArrowPaint.starBmp;
	    }

		/// <summary>
		/// Calculates coordinates for a centered rectangle.
		/// </summary>
		/// <param name="rect">The existing bounds.</param>
		/// <param name="size">The size of the rectangle to be centered.</param>
		/// <returns>A rectangle inside the specified bounds.</returns>
		static public Rectangle CenterInRect(Rectangle rect, Size size)
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
		/// <param name="visualBounds">A Rectangle which contains the boundary data of the rectangle.</param>
		/// <param name="offset">A Point that specifies pixel to offset the bitmap from its origin point.</param>
		/// <param name="bmp">The Bitmap to be drawn on the screen.</param>
		/// <param name="foreColor">The new color used to substitute black pixels.</param>
		/// <returns>A Rectangle which contains the boundary data of the drawn bitmap.</returns>
		/// <remarks>
		/// The PaintIcon routine
		/// will substitute black pixels of the original bitmap and draw them with the
		/// specified forecolor. The bitmap is centered inside the specified bounds. 
		/// Use the offset if you want to display a "pressed button" state. If the button is
		/// pressed, specify offset = new Point(1, 1).
		/// </remarks>
		public static Rectangle PaintIcon(Graphics g, Rectangle visualBounds, Point offset, Bitmap bmp, Color foreColor)  
        {
            Size size = bmp.Size;
            Rectangle rect;
            ImageAttributes imageAttributes;

            rect = CenterInRect(visualBounds, size);
            rect.Offset(offset);

            // g.FillRectangle(backColor,visualBounds);
            colorMap[0].NewColor = foreColor;
            colorMap[0].OldColor = Color.Black;
            imageAttributes = new ImageAttributes();
            imageAttributes.SetRemapTable(colorMap, ColorAdjustType.Bitmap);
            g.DrawImage((Image) bmp, rect, 0, 0, rect.Width, rect.Height,
                GraphicsUnit.Pixel, imageAttributes);
            imageAttributes.Dispose();

            return rect;
        }

		/// <summary>
		/// Loads the bitmap from manifest and paints it substituting black pixels with a new color.
		/// </summary>
		/// <param name="g">A Graphics object used to draw the bitmap.</param>
		/// <param name="bounds">A Rectangle which contains the boundary data of the rectangle.</param>
		/// <param name="arrowType">The type of button to draw.</param>
		/// <param name="offset">A Point that specifies pixels to offset the bitmap from its origin point.</param>
		/// <param name="arrowColor">The new color used to substitute black pixels.</param>
		/// <returns>A Rectangle which contains the boundary data of the drawn bitmap.</returns>
		/// <remarks>
		/// The DrawArrow routine
		/// will substitute black pixels of the original bitmap and draw them with the
		/// specified forecolor. The bitmap is centered inside the specified bounds. 
		/// Use the offset if you want to display a "pressed button" state. If the button is
		/// pressed, specify offset = new Point(1, 1).
		/// </remarks>
		public static void DrawArrow(Graphics g, Rectangle bounds, ArrowType arrowType, Point offset, Color arrowColor)
		{
			// Draw arrow inside the button.
			SolidBrush arrowBrush = new SolidBrush(arrowColor);
					
			try
			{
				int x, y;
				Rectangle rect = bounds;
				int cx = rect.Height > 19 ? 8 : 7;
				int cy = rect.Height > 19 ? 9 : 7;

				// center the 9-pixel high arrow vertically
				y = rect.Top + ((rect.Height - 9) / 2);
				int y2 = rect.Top + ((rect.Height - cy) / 2);

				// if pressed, image gets moved
				y += offset.Y;

				switch(arrowType) 
				{
					case ArrowType.First:
						// Most left
						x = rect.Left + ((rect.Width - cx) / 2) - 1 + offset.X;

						g.FillRectangle(arrowBrush, x, y2, 1, cy);
						goto case ArrowType.Previous;

					case ArrowType.Previous:
						// Left Arrow
						x = rect.Left + ((rect.Width - cx) / 2) - 1 + offset.X;

						g.FillRectangle(arrowBrush, x+2, y+4, 1, 1);
						g.FillRectangle(arrowBrush, x+3, y+3, 1, 3);
						g.FillRectangle(arrowBrush, x+4, y+2, 1, 5);
						g.FillRectangle(arrowBrush, x+5, y+1, 1, 7);

						if (rect.Height > 19)
							g.FillRectangle(arrowBrush, x+6,   y, 1, 9);
						break;

					case ArrowType.Last:
						// Most Right
						x = rect.Right - ((rect.Width - cx) / 2) + offset.X;

						g.FillRectangle(arrowBrush, x, y2, 1, cy);
						goto case ArrowType.Next;

					case ArrowType.Next:
						// Right Arrow
						x = rect.Right - ((rect.Width - cx) / 2) + offset.X;

						g.FillRectangle(arrowBrush, x-2, y+4, 1, 1);
						g.FillRectangle(arrowBrush, x-3, y+3, 1, 3);
						g.FillRectangle(arrowBrush, x-4, y+2, 1, 5);
						g.FillRectangle(arrowBrush, x-5, y+1, 1, 7);
						if (rect.Height > 19)
							g.FillRectangle(arrowBrush, x-6,   y, 1, 9);
						break;

                    case ArrowType.AddNew:
                        Bitmap bitmap = GetStarBitmap();
                        if (bitmap != null)
		                    PaintIcon(g, rect, offset, bitmap, arrowColor);
                        break;
				}
			}
			finally
			{				
				arrowBrush.Dispose();
			}
		}
        
    }
}
