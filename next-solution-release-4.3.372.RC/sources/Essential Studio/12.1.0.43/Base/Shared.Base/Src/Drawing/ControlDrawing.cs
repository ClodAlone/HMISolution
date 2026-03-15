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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Syncfusion.Documentation;
using Syncfusion.Runtime.InteropServices;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary></summary>
	[ DocumentationExclude() ]
	public class ControlDrawing
	{
		#region Class constants
		/// <summary>
		/// Inflate offset for drawing selection rectangle.
		/// </summary>
    public const int DrawTextFlags = DrawTextFormats.DT_EXPANDTABS |
      DrawTextFormats.DT_NOPREFIX |
      DrawTextFormats.DT_NOCLIP |
      DrawTextFormats.DT_SINGLELINE | 
			DrawTextFormats.DT_VCENTER;
		#endregion

		#region Class members
		/// <summary></summary>
		private Point offset = new Point();
		#endregion

		#region Class properties
		/// <summary></summary>
		public Point Offset
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
			}
		}

		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		public ControlDrawing()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion
		
		#region Class Public Methods
		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="text"/>
		/// <param name="font"/>
		/// <param name="color"/>
		/// <param name="rc"/>
		/// <param name="align"/>
		public void DrawAlignedText( Graphics g, string text, Font font, Color color, Rectangle rc, ContentAlignment align )
		{
			Size sz = g.MeasureString( text, font ).ToSize();
			Point pt = new Point( 0, 0 );
			int hMiddle = rc.Left + ( rc.Width - sz.Width ) / 2;
			int hRight = rc.Right - sz.Width;
			int vMiddle = rc.Top + ( rc.Height - sz.Height ) / 2;
			int vBottom = rc.Bottom - sz.Height;
			
			switch( align )
			{
				case ContentAlignment.TopLeft:
				{
					pt = new Point( rc.Left, rc.Top );
					break;
				}
				case ContentAlignment.TopCenter:
				{
					pt = new Point( hMiddle, rc.Top );
					break;
				}
				case ContentAlignment.TopRight:
				{
					pt = new Point( hRight, rc.Top );
					break;
				}
				case ContentAlignment.MiddleLeft:
				{
					pt = new Point( rc.Left, vMiddle );
					break;
				}
				case ContentAlignment.MiddleCenter:
				{
					pt = new Point( hMiddle, vMiddle );
					break;
				}
				case ContentAlignment.MiddleRight:
				{
					pt = new Point( hRight, vMiddle );
					break;
				}
				case ContentAlignment.BottomLeft:
				{
					pt = new Point( rc.Left, vBottom );
					break;
				}
				case ContentAlignment.BottomCenter:
				{
					pt = new Point( hMiddle, vBottom );
					break;
				}
				case ContentAlignment.BottomRight:
				{
					pt = new Point( hRight, vBottom );
					break;
				}
			}
			
			using( SolidBrush brush = new SolidBrush( color ) )
			{
				g.DrawString( text, font, brush, pt );
			}
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rc"/>
		/// <param name="borderStyle"/>
		/// <param name="border3DStyle"/>
		/// <param name="borderSingle"/>
		/// <param name="borderColor"/>
		public void DrawBorder( Graphics g, Rectangle rc, BorderStyle borderStyle, Border3DStyle border3DStyle, ButtonBorderStyle borderSingle, Color borderColor )
		{
			if( borderStyle == BorderStyle.Fixed3D )
			{
				ControlPaint.DrawBorder3D( g, rc, border3DStyle );
			}
			if( borderStyle == BorderStyle.FixedSingle )
			{
				ControlPaint.DrawBorder( g, rc, borderColor, borderSingle );
			}
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rc"/>
		/// <param name="borderStyle"/>
		/// <param name="border3DStyle"/>
		/// <param name="borderSingle"/>
		/// <param name="borderColor"/>
		/// <param name="sides"/>
		public void DrawBorder( Graphics g, Rectangle rc, BorderStyle borderStyle,
			Border3DStyle border3DStyle, ButtonBorderStyle borderSingle,
			Color borderColor, Border3DSide sides )
		{
			ControlDrawing.DrawBorderInternal( g, rc, borderStyle, border3DStyle,
				borderSingle, borderColor, sides, false );
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rc"/>
		/// <param name="fillColor"/>
		public void DrawFillColor( Graphics g, Rectangle rc, Color fillColor )
		{
			using( SolidBrush brush = new SolidBrush( fillColor ) )
			{
				g.FillRectangle( brush, new Rectangle( offset.X, offset.Y, rc.Width, rc.Height ) );
			}
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rc"/>
		/// <param name="color1"/>
		/// <param name="color2"/>
		public void DrawVerticalGradient( Graphics g, Rectangle rc, Color color1, Color color2 )
		{
			using( Brush brush = GetVerticalGradientBrush( rc, color1, color2 ) )
			{
				g.FillRectangle( brush, new Rectangle( offset.X, offset.Y, rc.Width, rc.Height ) );
			}
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rc"/>
		/// <param name="color1"/>
		/// <param name="color2"/>
		public void DrawGradient( Graphics g, Rectangle rc, Color color1, Color color2 )
		{
			using( Brush brush = GetGradientBrush( rc, color1, color2 ) )
			{
				g.FillRectangle( brush, new Rectangle( offset.X, offset.Y, rc.Width, rc.Height ) );
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="rc"/>
		/// <param name="color1"/>
		/// <param name="color2"/>
		public Brush GetGradientBrush( Rectangle rc, Color color1, Color color2 )
		{
			return new LinearGradientBrush( Point.Empty, new Point( rc.Width, 0 ),
				color1, color2 );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="rc"/>
		/// <param name="color1"/>
		/// <param name="color2"/>
		public Brush GetVerticalGradientBrush( Rectangle rc, Color color1, Color color2 )
		{
			return new LinearGradientBrush( Point.Empty, new Point( 0, rc.Height ),
				color1, color2 );
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rc"/>
		/// <param name="img"/>
		/// <param name="stretchImage"/>
		public void DrawImage( Graphics g, Rectangle rc, Image img, bool stretchImage )
		{
			Image image = img;
			int width = image.Width;
			int height = image.Height;

			if( stretchImage )
			{
				g.DrawImage( image, new Rectangle( offset.X, offset.Y, rc.Width, rc.Height ), 0, 0, width, height, GraphicsUnit.Pixel );
			}
			else
			{
				int Count = rc.Width / width;
				Rectangle destRect = new Rectangle( offset.X, offset.Y, width, rc.Height );
				
				// BUGBUG: this method can be implemented in different and more faster way. For
				// finding such code please look into HTML UI project and it image rendering
				// code base.
				for( int i = 0 ; i < Count ; i++ )
				{
					g.DrawImage( image, destRect, 0, 0, width, rc.Height, GraphicsUnit.Pixel );
					destRect.Offset( width, 0 );
				}
				
				destRect.Width = rc.Width - Count * width;
				g.DrawImage( image, destRect, 0, 0, rc.Width - Count * width, rc.Height, GraphicsUnit.Pixel );
			}
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rc"/>
		/// <param name="color1"/>
		/// <param name="color2"/>
		public void DrawTube( Graphics g, Rectangle rc, Color color1, Color color2 )
		{
			using( Brush brush = GetTubeBrush( color1, color2, rc ) )
			{
				g.FillRectangle( brush, new Rectangle( offset.X, offset.Y, rc.Width, rc.Height ) );
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="color1"/>
		/// <param name="color2"/>
		/// <param name="rc"/>
		public Brush GetTubeBrush( Color color1, Color color2, Rectangle rc )
		{
			Point[ ] points = new Point[]
			{
				new Point( offset.X, offset.Y ),
				new Point( offset.X + rc.Width, offset.Y ),
				new Point( offset.X + rc.Width, offset.Y + rc.Height / 2 ),
				new Point( offset.X + rc.Width, offset.Y + rc.Height ),
				new Point( offset.X, offset.Y + rc.Height ),
				new Point( offset.X, offset.Y + rc.Height / 2 )
			};

			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );
			
			PathGradientBrush pthGrBrush = new PathGradientBrush( path );
			pthGrBrush.CenterColor = color1;
			Color[ ] colors = { 
													color2,
													color2,
													color1,
													color2,
													color2,
													color1
												};
			
			pthGrBrush.SurroundColors = colors;
			return pthGrBrush;
		}

		/// <summary></summary>
		/// <param name="g"/>
		/// <param name="rc"/>
		/// <param name="foreColors"/>
		/// <param name="vertical"/>
		public void DrawMultipleGradient( Graphics g, Rectangle rc, ArrayList foreColors, bool vertical )
		{
			if( foreColors.Count == 0 )
			{
				return;
			}
			
			if( foreColors.Count == 1 )
			{
				g.FillRectangle( new SolidBrush( ( Color )foreColors[ 0 ] ), rc );
				return;
			}

			using( Bitmap bmp = new Bitmap( rc.Width, rc.Height, g ) )
			{
				using( Graphics gr = Graphics.FromImage( bmp ) )
				{
					LinearGradientBrush linGrBrush = new LinearGradientBrush(
						new Point( 0, offset.Y ),
						new Point( bmp.Width, offset.Y ),
						Color.Black, Color.Black );

					ColorBlend cb = new ColorBlend( foreColors.Count );
					for( int i = 0 ; i < foreColors.Count ; i++ )
					{
						cb.Colors.SetValue( foreColors[ i ], i );
						cb.Positions.SetValue( ( float )i / ( foreColors.Count - 1 ), i );
					}

					linGrBrush.InterpolationColors = cb;

					gr.FillRectangle( linGrBrush, new Rectangle( 0, 0, bmp.Width, bmp.Height ) );
				}

				if( vertical )
				{
					bmp.RotateFlip( RotateFlipType.Rotate90FlipNone );
				}

				Rectangle destination = new Rectangle( offset.X, offset.Y, rc.Width, rc.Height );

				if( vertical )
				{
					g.DrawImage( bmp, destination, 0, 0, rc.Height, rc.Width, GraphicsUnit.Pixel );
				}
				else
				{
					g.DrawImage( bmp, destination, 0, 0, rc.Width, rc.Height, GraphicsUnit.Pixel );
				}
			}
		}
		#endregion

		#region Class static Public Methods
		/// <summary>Methods recalculate top left and right bottom points 
		/// using Graphics matrix settings. This method required when we will
		/// draw on Graphics created from HDC that does not have applied transformations.
		/// Very useful when used ControlPaint class for drawing.</summary>
		/// <param name="g">reference on Graphics</param>
		/// <param name="rect">rectangle to recalculate.</param>
		/// <returns>Transformed rectangle.</returns>
		/// <remarks>Methods will work fine only in limited cases. If on graphics applied 
		/// rotations then in most cases will be wrong calculated rectangle.</remarks>
		/// <example language="C#">
		/// Rectangle rcBorders = ControlDrawing.Transform( g, this.Bounds );
		/// ControlDrawing.DrawBorderInternal( g, rcBorders, this.BorderStyle, this.Border3DStyle, this.BorderSingle, this.BorderColor, this.BorderSides );
		/// </example>
		public static Rectangle Transform( Graphics g, Rectangle rect )
		{
			if( g == null )
			{
				throw new ArgumentNullException( "g" );
			}

			Point[ ] points = new Point[ ]
				{
					new Point( rect.Left, rect.Top ),
					new Point( rect.Right, rect.Bottom )
				};

			g.Transform.TransformPoints( points );

			return Rectangle.FromLTRB( points[ 0 ].X, points[ 0 ].Y, points[ 1 ].X, points[ 1 ].Y );
		}

		/// <summary>Methods recalculate point using Graphics matrix settings. 
		/// This method required when we will draw on Graphics created from HDC 
		/// that does not have applied transformations. Very useful when used 
		/// ControlPaint class for drawing.</summary>
		/// <param name="g">reference on Graphics</param>
		/// <param name="point">Point to recalculate.</param>
		/// <returns>Transformed point.</returns>
		public static Point Transform( Graphics g, Point point )
		{
			if( g == null )
			{
				throw new ArgumentNullException( "g" );
			}

			Point[ ] points = new Point[ ]{point};
			g.Transform.TransformPoints( points );

			return points[ 0 ];
		}
		/// <summary>
		/// Method allow to draw borders according to specified styles.
		/// </summary>
		/// <param name="g">Reference on Graphics.</param>
		/// <param name="rc">Output rectangle.</param>
		/// <param name="borderStyle">border style.</param>
		/// <param name="border3DStyle">3D border style.</param>
		/// <param name="borderSingle">border style in single mode.</param>
		/// <param name="borderColor">border color in single mode.</param>
		/// <param name="sides">sides that have to be drawn by methods.</param>
		/// <param name="useTransform">True - fix known bug in method, otherwise leave 
		/// old code (for compatibility only).</param>
		public static void DrawBorderInternal( Graphics g, Rectangle rc, BorderStyle borderStyle,
			Border3DStyle border3DStyle, ButtonBorderStyle borderSingle,
			Color borderColor, Border3DSide sides, bool useTransform )
		{
			if( borderStyle == BorderStyle.Fixed3D )
			{
				// required for ControlPaint.DrawBorder3D bug fixing with matrix 
				// transformation applying on 
				Rectangle output = ( useTransform ) ? Transform( g, rc ) : rc;
        		Region oldRegion = g.Clip;

		        if( useTransform )
		        {
		          oldRegion = g.Clip;
		          Rectangle rect = Rectangle.Inflate( output, -2, -2 );
		          g.SetClip( rect, CombineMode.Exclude );
		        }

				ControlPaint.DrawBorder3D( g, output, border3DStyle, sides );
        		g.Clip = oldRegion;
			}
			else if( borderStyle == BorderStyle.FixedSingle )
			{
				// Support for drawing only certain borders.
				ButtonBorderStyle leftStyle, rightStyle, topStyle, bottomStyle;
				leftStyle = rightStyle = topStyle = bottomStyle = ButtonBorderStyle.None;

				if( ( sides & Border3DSide.Left ) > 0 )
				{
					leftStyle = borderSingle;
				}
				if( ( sides & Border3DSide.Top ) > 0 )
				{
					topStyle = borderSingle;
				}
				if( ( sides & Border3DSide.Right ) > 0 )
				{
					rightStyle = borderSingle;
				}
				if( ( sides & Border3DSide.Bottom ) > 0 )
				{
					bottomStyle = borderSingle;
				}

				// bug fixing: lines drawn on one pixcel more then required
				Rectangle output = ( useTransform ) ?
					new Rectangle( rc.X, rc.Y, rc.Width - 1, rc.Height - 1 ) : rc;

		        Region oldRegion = g.Clip; 
		
		        if( useTransform )
		        {
		          oldRegion = g.Clip;
		          Rectangle rect = Rectangle.Inflate( output, -2, -2 );
		          g.SetClip( rect, CombineMode.Exclude );
		        }

				ControlPaint.DrawBorder( g, output,
					borderColor, 1, leftStyle,
					borderColor, 1, topStyle,
					borderColor, 1, rightStyle,
					borderColor, 1, bottomStyle
					);

        		g.Clip = oldRegion;
			}
		}

		/// <summary>
		/// Returns	the	width	required to	draw the text	specified	using	the	font specified.
		/// </summary>
		/// <param name="graphics">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="text">The text	that is	to be	drawn.</param>
		/// <param name="font">The <see cref="System.Drawing.Font"/> using which to	draw.</param>
		/// <param name="mirrored">True - we use RTL, otherwise normal drawing.</param>
		/// <returns>Width required.</returns>
		public static Size MeasureDisplayStringSize( Graphics graphics, string text, Font font, bool mirrored )
		{
			return MeasureDisplayStringSize( graphics, text, font, mirrored, -1 );
		}
		/// <summary>Measure string with limit by width.</summary>
		/// <param name="graphics"></param>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <param name="mirrored"></param>
		/// <param name="width">-1 - apply single line mode for measuring, values greater zero 
		/// enables multiline measuring mode.</param>
		/// <returns></returns>
		public static Size MeasureDisplayStringSize( Graphics graphics, string text, Font font, bool mirrored, int width )
		{
			if( text == null || text.Length == 0 )
			{
				return Size.Empty;
			}

			int nFlags;
            NativeMethods.RECT rect = new NativeMethods.RECT(0, 0, width, 0);
			IntPtr hdc = graphics.GetHdc();
			IntPtr hFont = font.ToHfont();
            try
            {
                IntPtr prevFont = NativeMethods.SelectObject(hdc, hFont);

                // if we measure single line
                if (width < 0)
                {
                    nFlags = ControlDrawing.DrawTextFlags | DrawTextFormats.DT_CALCRECT;
                    nFlags &= ~DrawTextFormats.DT_VCENTER;
                }
                else // if we measure multiline
                {
                    nFlags =
                        DrawTextFormats.DT_CALCRECT |
                        DrawTextFormats.DT_EXPANDTABS |
                        DrawTextFormats.DT_NOPREFIX;
                }

                if (mirrored)
                {
                    nFlags |= DrawTextFormats.DT_RTLREADING;
                }

                NativeMethods.DrawText(hdc, text, text.Length, ref rect, nFlags);

                prevFont = NativeMethods.SelectObject(hdc, prevFont);                
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
                NativeMethods.DeleteObject(hFont);
            }

			return new Size( rect.Width, rect.Height );
		}
		#endregion
	}
}