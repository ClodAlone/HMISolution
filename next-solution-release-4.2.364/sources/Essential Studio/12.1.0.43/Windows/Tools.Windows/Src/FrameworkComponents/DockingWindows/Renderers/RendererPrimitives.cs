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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	internal class RendererPrimitives
	{
		static void PaintBackground( Graphics g, Rectangle rectangle, Color color )
		{
            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle (brush, rectangle);
            }
		}

		static public void PaintBackground( Graphics g, Rectangle rectangle, Brush brush )
		{
			g.FillRectangle( brush, rectangle );
		}

		static public void PaintBorders( Graphics g, Rectangle rectangle, 
			Brush brush, int width )
		{
			g.FillRectangles( brush, new Rectangle[] { 
				new Rectangle( rectangle.Left, rectangle.Top, rectangle.Width, width ),
				new Rectangle( rectangle.Right - width, rectangle.Top, 
				width, rectangle.Height),
				new Rectangle( rectangle.Left, rectangle.Bottom - width,
				rectangle.Width, width),
				new Rectangle( rectangle.Left, rectangle.Top, width, rectangle.Height )} );
		}

		static public void PaintGripper( Graphics g, Orientation orientation,
			Point location, Brush foreBrush, Brush backBrush, int dotCount )
		{
			if( orientation == Orientation.Vertical )
			{
				for( int i = 0; i < dotCount; i++ )
				{
					g.FillRectangle( backBrush, 
						new Rectangle( location.X + 1, location.Y + 4 * i + 1, 2, 2 ) );
					g.FillRectangle( foreBrush, 
						new Rectangle( location.X, location.Y + 4 * i, 2, 2 ) );
				}
			}
			else
			{
				for( int i = 0; i < dotCount; i++ )
				{
					g.FillRectangle( backBrush, 
						new Rectangle( location.X + 4 * i + 1, location.Y + 1, 2, 2 ) );
					g.FillRectangle( foreBrush, 
						new Rectangle( location.X + 4 * i, location.Y, 2, 2 ) );
				}
			}
		}

		static public void PaintGripper( Graphics g, Orientation orientation,
			Point location, Bitmap bitmap, int dotCount )
		{
			if( orientation == Orientation.Vertical )
			{
				for( int i = 0; i < dotCount; i++ )
				{
					g.DrawImage(bitmap, location.X, location.Y + 4 * i);
				}
			}
			else
			{
				for( int i = 0; i < dotCount; i++ )
				{
					g.DrawImage(bitmap, location.X + 4 * i, location.Y);
				}
			}
		}


		static public void PaintButtonBackground( Graphics g, Rectangle rectangle, Pen borderPen, 
			Brush backgroundBrush )
		{
			Rectangle rectangleToFill = rectangle;
			if (backgroundBrush != null)
			{
				g.FillRectangle(backgroundBrush, rectangle);
			}
			if (borderPen != null)
			{
				g.DrawRectangle(borderPen, rectangle.X,
					rectangle.Y, rectangleToFill.Width - 1, rectangleToFill.Height - 1);
			}
		}

		static public void DrawText( Graphics g, string text, Font font, Brush brush,
			Rectangle rectangle, StringFormat format )
		{
			g.DrawString( text, font, brush, rectangle, format );
		}

		static Rectangle GetImageButtonRect( Rectangle rectangle )
		{
			return new Rectangle( 
				rectangle.X + rectangle.Width / 2 - 4, 
				rectangle.Y + rectangle.Height / 2 - 4,
				9, 9); 
		}

		static public void PaintCustomButtonImage( Graphics g, Image image, Rectangle rect )
		{
			if( image.Height > rect.Height || image.Width > rect.Width )
			{
				g.DrawImage( image, rect );
			}
			else
			{
				int x = rect.X + (rect.Width - image.Width) / 2;
				int y = rect.Y + (rect.Height - image.Height) / 2;
				g.DrawImageUnscaled( image, x, y );
			}
		}

		static public void PaintCloseButtonImage( Graphics g, Pen pen, Rectangle rectangle )
		{
			Rectangle rc = RendererPrimitives.GetImageButtonRect( rectangle );
			g.DrawLine( pen, rc.X, rc.Y, rc.X + 9, rc.Y + 9 );
			g.DrawLine( pen, rc.X, rc.Y + 9, rc.X + 9, rc.Y );
		}
		static public void PaintPinButtonImageVertical( Graphics g, Pen pen, Rectangle rectangle )
		{
			Rectangle rc = RendererPrimitives.GetImageButtonRect( rectangle );
			g.DrawLine( pen, rc.X + 3, rc.Y, rc.X + 4, rc.Y );
			g.DrawLine( pen, rc.X + 2, rc.Y, rc.X + 2, rc.Y + 4 );
			g.DrawLine( pen, rc.X, rc.Y + 5, rc.X + 8, rc.Y + 5 );
			g.DrawLine( pen, rc.X + 5, rc.Y, rc.X + 5, rc.Y + 4 );
			g.DrawLine( pen, rc.X + 6, rc.Y, rc.X + 6, rc.Y + 4 );
			g.DrawLine( pen, rc.X + 4, rc.Y + 6, rc.X + 4, rc.Y + 8 );
		}
		static public void PaintPinButtonImageHorizontal( Graphics g, Pen pen, Rectangle rectangle )
		{
			Rectangle rc = RendererPrimitives.GetImageButtonRect( rectangle );
			g.DrawLine( pen, rc.X + 8, rc.Y + 3, rc.X + 8, rc.Y + 5 );
			g.DrawLine( pen, rc.X + 8, rc.Y + 2, rc.X + 4, rc.Y + 2 );
			g.DrawLine( pen, rc.X + 3, rc.Y, rc.X + 3, rc.Y + 8 );
			g.DrawLine( pen, rc.X + 8, rc.Y + 5, rc.X + 4, rc.Y + 5 );
			g.DrawLine( pen, rc.X + 8, rc.Y + 6, rc.X + 4, rc.Y + 6 );
			g.DrawLine( pen, rc.X + 2, rc.Y + 4, rc.X, rc.Y + 4 );
		}
		static public void PaintMenuButtonImage( Graphics g, Pen pen, Rectangle rectangle )
		{
			Rectangle rc = RendererPrimitives.GetImageButtonRect( rectangle );
			g.DrawLine( pen, rc.X + 1, rc.Y + 3, rc.X + 8, rc.Y + 3 );
			g.DrawLine( pen, rc.X + 2, rc.Y + 4, rc.X + 7, rc.Y + 4 );
			g.DrawLine( pen, rc.X + 3, rc.Y + 5, rc.X + 6, rc.Y + 5 );
			g.DrawLine( pen, rc.X + 4, rc.Y + 6, rc.X + 5, rc.Y + 6 );
		}

		static public void PaintMaximizeButtonImage( Graphics g, Pen pen, Rectangle rectangle )
		{
			Rectangle rc = RendererPrimitives.GetImageButtonRect(rectangle);

			int height = rc.Height;
			int width = rc.Width;

			Point topleft = new Point(rc.Left, rc.Top);
			Point topright = new Point(topleft.X + width, topleft.Y);
			Rectangle window = new Rectangle(topleft, new Size(width, height));

			g.DrawRectangle(pen, window);
			g.DrawLine(pen, new Point(topleft.X, topleft.Y + 1), new Point(topright.X, topright.Y + 1));
		}

		static public void PaintRestoreButtonImage( Graphics g, Pen pen, Rectangle rectangle )
		{
			Rectangle rc = RendererPrimitives.GetImageButtonRect(rectangle);
			int height = rc.Height/3*2;
			int width = rc.Width/3*2;

			Point frontTopLeft = new Point(rc.Left , rc.Top);
			Point rearBottomRight = new Point(rc.Right, rc.Bottom);
			Point rearBottomLeft = new Point(rearBottomRight.X - width, rearBottomRight.Y);
			Point rearTopRight = new Point(rearBottomRight.X, rearBottomRight.Y - height);
			Point rearTopLeftB = new Point(rearBottomLeft.X, rearBottomLeft.Y - 2);
			Point rearTopLeftR = new Point(rearTopRight.X - 3, rearTopRight.Y);

			Rectangle front = new Rectangle(frontTopLeft, new Size(width, height));

			g.DrawRectangle(pen, front);
			g.DrawLine(pen, rearBottomRight, rearBottomLeft);
			g.DrawLine(pen, rearBottomRight, rearTopRight);
			g.DrawLine(pen, rearBottomLeft, rearTopLeftB);
			g.DrawLine(pen, rearTopRight, rearTopLeftR);
		}
	}
}
