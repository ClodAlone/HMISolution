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
using System.Drawing;
using System.Drawing.Drawing2D;

using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Drawing;


namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <exclude/>
	/// <summary>
	/// Class contains helper methods to work with graphics.
	/// </summary>
	public sealed class GraphicsUtils
	{
		#region Constants
		/// <summary>
		/// Length of the block for measuring. It must be small because MeasureCharacterRanges function can not work with large amount of data at once.
		/// </summary>
		private const int DEF_MAX_TEXT_MEASURE_LENGTH = 30;
		/// <summary>
		/// Length of one little wave of wave lines.
		/// </summary>
		private const int WAVE_LENGTH = 2;
		/// <summary>
		/// White space delimiter of double line.
		/// </summary>
		private const int DOUBLE_LINE_DELIMITER = 2;
		/// <summary>
		/// Default flags for StringFormat instances.
		/// </summary>
		private const StringFormatFlags DEF_FORMAT_FLAGS = StringFormatFlags.DisplayFormatControl | StringFormatFlags.MeasureTrailingSpaces;
		#endregion

		#region Public Fields
		/// <summary>
		/// StringFormat instance used for drawing and measuring strings
		/// </summary>
		public static readonly StringFormat DefaultFormat = ( StringFormat )StringFormat.GenericTypographic.Clone();
		/// <summary>
		/// Default graphics object.
		/// </summary>
		public static readonly Graphics DefaultGraphics = Graphics.FromImage( new Bitmap( 1, 1 ) );
		/// <summary>
		/// Default lagre rectangle, used to measure and draw text without clipping.
		/// </summary>
		public static readonly Rectangle LargeRectangle = new Rectangle( 0, 0, int.MaxValue, int.MaxValue );
		#endregion

		#region Fields
		/// <summary>
		/// Flag, that specifies, whether all data is initialized.
		/// </summary>
		private static bool m_bInitialized = false;
		/// <summary>
		/// 
		/// </summary>
		internal static Hashtable m_fontHandles;
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		static GraphicsUtils()
		{
			m_fontHandles = new Hashtable();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Measures string and it's every character.
		/// </summary>
		/// <param name="text">String, to be measured.</param>
		/// <param name="font">Font, the string will be drawn with.</param>
		/// <param name="bMeasureWholeWord">Indicates whether whole word should be measured or every single character/</param>
		/// <param name="g">Graphics.</param>
		/// <param name="tabReplace">String to replace tab symbols with.</param>
		/// <param name="bNativeGdi">Indicates whether native GDI should be used.</param>
		/// <returns>TextInfo structure, which contains all information about string and it's characters sizes.</returns>
		public static TextInfo MeasureString( string text, Font font, bool bMeasureWholeWord, Graphics g, string tabReplace, bool bNativeGdi )
		{
			if( bNativeGdi )
			{
				return MeasureStringNativeGdi( text, font, bMeasureWholeWord, g, tabReplace );
			}
			else
			{
				return MeasureStringGdiPlus( text, font, bMeasureWholeWord, g, tabReplace );
			}
		}
		/// <summary>
		/// Draw border around border rectangle using specified format settings.
		/// </summary>
		/// <param name="g">Graphics object to draw border on.</param>
		/// <param name="p1">Top left corner of border rectangle.</param>
		/// <param name="p2">Bottom right corner of border rectangle.</param>
		/// <param name="style">Syle of border.</param>
		/// <param name="color">Color of border.</param>
		/// <param name="weight">Weight of border line.</param>
		public static void DrawBorder( Graphics g, Point p1, Point p2, FrameBorderStyle style, Color color, BorderWeight weight )
		{
			if( null == g ) throw new ArgumentNullException( "g" );
			if( p1.IsEmpty ) throw new ArgumentOutOfRangeException( "p1" );
			if( p2.IsEmpty ) throw new ArgumentOutOfRangeException( "p2" );

			if( style != FrameBorderStyle.None )
			{
				if( FrameBorderStyle.Wave == style )
				{
					if( BorderWeight.Bold != ( weight & BorderWeight.Bold ) )
					{
						DrawWaveRect( g, p1, p2, false, color );

						if( BorderWeight.Double == ( weight & BorderWeight.Double ) )
						{
							DrawWaveRect( g, new Point( p1.X + 3, p1.Y + 3 ), new Point( p2.X - 3, p2.Y - 3 ), false, color );
						}
					}
					else
					{
						DrawWaveRect( g, p1, p2, true, color );
					}
				}
				else
				{
					Pen pen = new Pen( color, 1 );

					switch( style )
					{
						case FrameBorderStyle.Dash:
							pen.DashStyle = DashStyle.Dash;
							break;
						case FrameBorderStyle.DashDot:
							pen.DashStyle = DashStyle.DashDot;
							break;
						case FrameBorderStyle.Dot:
							pen.DashStyle = DashStyle.Dot;
							break;
						case FrameBorderStyle.Solid:
							pen.DashStyle = DashStyle.Solid;
							break;
					}

					if( BorderWeight.Bold == ( weight & BorderWeight.Bold ) )
					{
						pen.Width = 2;
					}
					g.DrawRectangle( pen, new Rectangle( p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y ) );

					if( BorderWeight.Double == ( weight & BorderWeight.Double ) )
					{
						int delimiter = DOUBLE_LINE_DELIMITER;
						if( BorderWeight.Bold == ( weight & BorderWeight.Bold ) )
						{
							delimiter++;
						}

						g.DrawRectangle( pen, new Rectangle( p1.X + delimiter, p1.Y + delimiter, p2.X - p1.X - 2 * delimiter, p2.Y - p1.Y - 2 * delimiter ) );
					}
					pen.Dispose();
				}
			}
		}
		/// <summary>
		/// Draw wave line rectangle with top left cornet in p1 and bottom right corner in p2.
		/// </summary>
		/// <param name="g">Grafics object to draw rectangle.</param>
		/// <param name="p1">Top left corner of rectangle.</param>
		/// <param name="p2">Bottom right corner of rectangle.</param>
		/// <param name="bBold">Indicates whether line should be bold.</param>
		/// <param name="color">Color of border.</param>
		public static void DrawWaveRect( Graphics g, Point p1, Point p2, bool bBold, Color color )
		{
			if( null == g ) throw new ArgumentNullException( "g" );
			if( Point.Empty == p1 ) throw new ArgumentOutOfRangeException( "p1" );
			if( Point.Empty == p2 ) throw new ArgumentOutOfRangeException( "p2" );

			DrawHorizontalWaveLine( g, p1.Y, p1.X, p2.X, color, bBold );
			DrawVerticalWaveLine( g, p2.X, p1.Y, p2.Y + 1, color, bBold );
			DrawHorizontalWaveLine( g, p2.Y, p1.X, p2.X, color, bBold );
			DrawVerticalWaveLine( g, p1.X, p1.Y, p2.Y, color, bBold );
		}
		/// <summary>
		/// Draw wave line rectangle with coordinates in rect.
		/// </summary>
		/// <param name="g">Grafics object to draw rectangle.</param>
		/// <param name="rect">Rectangle to draw wave line.</param>
		/// <param name="bBold">Indicates whether line should be bold.</param>
		/// <param name="color">Color of border.</param>
		public static void DrawWaveRect( Graphics g, Rectangle rect, bool bBold, Color color )
		{
			if( null == g ) throw new ArgumentNullException( "g" );

			DrawWaveRect( g, new Point( rect.Left, rect.Top ), new Point( rect.Right, rect.Bottom ), bBold, color );
		}
		/// <summary>
		/// Draws horizontal wave line.
		/// </summary>
		/// <param name="g">Graphics object to draw line.</param>
		/// <param name="y">"Y" coordinate of line.</param>
		/// <param name="x1">"X" coordinate of line start.</param>
		/// <param name="x2">"X" coordinate of line end.</param>
		/// <param name="color">Color of line.</param>
		/// <param name="bBold">Indicates whether line should be bold.</param>
		public static void DrawHorizontalWaveLine( Graphics g, int y, int x1, int x2, Color color, bool bBold )
		{
			if( null == g ) throw new ArgumentNullException( "g" );

			if( bBold )
			{
				Pen pen1 = new Pen( color );
				pen1.DashPattern = new float[] { WAVE_LENGTH, WAVE_LENGTH + 1 };

				Pen pen2 = new Pen( color );
				pen2.DashPattern = new float[] { WAVE_LENGTH, WAVE_LENGTH - 1, WAVE_LENGTH - 1, WAVE_LENGTH - 1 };

				g.DrawLine( pen1, x1, y, x2, y );
				g.DrawLine( pen2, x1 + WAVE_LENGTH - 1, y + 1, x2, y + 1 );
				g.DrawLine( pen1, x1 + WAVE_LENGTH, y + 2, x2, y + 2 );

				pen1.Dispose();
				pen2.Dispose();
			}
			else
			{
				Pen pen = new Pen( color );
				pen.DashPattern = new float[] { WAVE_LENGTH, WAVE_LENGTH };

				g.DrawLine( pen, x1, y, x2, y );
				g.DrawLine( pen, x1 + WAVE_LENGTH, y + 1, x2, y + 1 );

				pen.Dispose();
			}
		}
		/// <summary>
		/// Draws vertical wave line.
		/// </summary>
		/// <param name="g">Graphics object to draw line.</param>
		/// <param name="x">"X" coordinate of line.</param>
		/// <param name="y1">"Y" coordinate of line start.</param>
		/// <param name="y2">"Y" coordinate of line end.</param>
		/// <param name="color">Color of line.</param>
		/// <param name="bBold">Indicates whether line should be bold.</param>
		public static void DrawVerticalWaveLine( Graphics g, int x, int y1, int y2, Color color, bool bBold )
		{
			if( null == g ) throw new ArgumentNullException( "g" );

			if( bBold )
			{
				Pen pen1 = new Pen( color );
				pen1.DashPattern = new float[] { WAVE_LENGTH, WAVE_LENGTH + 1 };

				Pen pen2 = new Pen( color );
				pen2.DashPattern = new float[] { WAVE_LENGTH, WAVE_LENGTH - 1, WAVE_LENGTH - 1, WAVE_LENGTH - 1 };

				g.DrawLine( pen1, x, y1, x, y2 );
				g.DrawLine( pen2, x + 1, y1 + WAVE_LENGTH - 1, x + 1, y2 );
				g.DrawLine( pen1, x + 2, y1 + WAVE_LENGTH, x + 2, y2 );
				pen1.Dispose();
				pen2.Dispose();
			}
			else
			{
				Pen pen = new Pen( color );
				pen.DashPattern = new float[] { WAVE_LENGTH, WAVE_LENGTH };

				g.DrawLine( pen, x, y1, x, y2 );
				g.DrawLine( pen, x + 1, y1 + WAVE_LENGTH, x + 1, y2 );

				pen.Dispose();
			}
		}
		/// <summary>
		/// Draw border around border rectangle using specified format settings.
		/// </summary>
		/// <param name="g">Graphics object to draw border on.</param>
		/// <param name="rect">Border rectangle.</param>
		/// <param name="style">Syle of border.</param>
		/// <param name="color">Color of border.</param>
		/// <param name="weight">Weight of border line.</param>
		public static void DrawBorder( Graphics g, ref RectangleF rect, FrameBorderStyle style, Color color, BorderWeight weight )
		{
			if( null == g ) throw new ArgumentNullException( "g" );

			int offset = 0;
			Point pointLeftTop = new Point( ( int )rect.Left, ( int )rect.Top + offset );
			Point pointRightBottom = new Point( ( int )rect.Right, ( int )rect.Bottom - offset );

			DrawBorder( g, pointLeftTop, pointRightBottom, style, color, weight );
		}
		/// <summary>
		/// Draws rounded rectangle.
		/// </summary>
		/// <param name="g">graphics object to draw rectangle.</param>
		/// <param name="rect">Rectangle that should be drawn rounded.</param>
		/// <param name="roundingFactor">Rounding factor: the bigger - the bigger rounding.</param>
		/// <param name="brush">Brush to draw rectangle with.</param>
		/// <param name="pen">Pen object to draw border.</param>
		public static void DrawRoundedRect( Graphics g, Rectangle rect, int roundingFactor, BrushInfo brush, Pen pen )
		{
			if( null == g ) throw new ArgumentNullException( "g" );
			if( null == brush ) throw new ArgumentNullException( "brus" );

			using( GraphicsPath niceRect = new GraphicsPath() )
			{
				niceRect.AddLine( rect.Left + roundingFactor, rect.Top, rect.Right - roundingFactor, rect.Top );
				niceRect.AddArc( rect.Right - 2 * roundingFactor, rect.Top, 2 * roundingFactor, 2 * roundingFactor, 270, 90 );
				niceRect.AddLine( rect.Right, rect.Top + roundingFactor, rect.Right, rect.Bottom - roundingFactor );
				niceRect.AddArc( rect.Right - 2 * roundingFactor, rect.Bottom - 2 * roundingFactor, 2 * roundingFactor, 2 * roundingFactor, 0, 90 );
				niceRect.AddLine( rect.Right - roundingFactor, rect.Bottom, rect.Left + roundingFactor, rect.Bottom );
				niceRect.AddArc( rect.Left, rect.Bottom - 2 * roundingFactor, 2 * roundingFactor, 2 * roundingFactor, 90, 90 );
				niceRect.AddLine( rect.Left, rect.Bottom - roundingFactor, rect.Left, rect.Top + roundingFactor );
				niceRect.AddArc( rect.Left, rect.Top, 2 * roundingFactor, 2 * roundingFactor, 180, 90 );
				BrushPaint.FillRegion( g, new Region( niceRect ), brush );
				g.DrawPath( pen, niceRect );
			}
		}
		/// <summary>
		/// Draws XP styled 3D border.
		/// </summary>
		/// <param name="g">Graphics object to draw.</param>
		/// <param name="rect">Rectangle to draw the border in.</param>
		public static void Draw3DBorder( Graphics g, Rectangle rect )
		{
			int x = rect.X;
			int y = rect.Y;
			int w = rect.Width;
			int h = rect.Height;

			g.DrawLine( Pens.LightGray, x, y, w, y );
			g.DrawLine( Pens.LightGray, x, y, x, h );
			g.DrawLine( Pens.Gray, w, y, w, h );
			g.DrawLine( Pens.Gray, x, h, w, h );
			g.DrawLine( Pens.White, x + 1, y + 1, w - 1, y + 1 );
			g.DrawLine( Pens.White, x + 1, y + 1, x + 1, h - 1 );
			g.DrawLine( Pens.DarkGray, w - 1, y, w - 1, h - 1 );
			g.DrawLine( Pens.DarkGray, x + 1, h - 1, w - 1, h - 1 );
			g.DrawLine( Pens.LightGray, x + 2, y + 2, w - 2, y + 2 );
			g.DrawLine( Pens.LightGray, x + 2, y + 2, x + 2, h - 2 );
			g.DrawLine( Pens.LightGray, w - 2, y + 2, w - 2, h - 2 );
			g.DrawLine( Pens.LightGray, x + 2, h - 2, w - 2, h - 2 );
		}
		/// <summary>
		/// Draws graphics path using specified line settings.
		/// </summary>
		/// <param name="g">Graphics object to draw at.</param>
		/// <param name="path">Graphics path to draw.</param>
		/// <param name="style">Style of line to draw the path.</param>
		/// <param name="color">Color of line to draw the path.</param>
		public static void DrawPath( Graphics g, GraphicsPath path, FrameBorderStyle style, Color color )
		{
			if( FrameBorderStyle.Wave != style )
			{
				Pen pen = new Pen( color );

				switch( style )
				{
					case FrameBorderStyle.Dash:
						pen.DashStyle = DashStyle.Dash;
						break;
					case FrameBorderStyle.DashDot:
						pen.DashStyle = DashStyle.DashDot;
						break;
					case FrameBorderStyle.Dot:
						pen.DashStyle = DashStyle.Dot;
						break;
					case FrameBorderStyle.Solid:
						pen.DashStyle = DashStyle.Solid;
						break;
				}

				g.DrawPath( pen, path );
			}
			else
			{
				PointF[] points = path.PathPoints;
				Point p1 = new Point( 0, 0 );
				Point p2 = new Point( 0, 0 );
				PointF p;

				for( int i = 0, len = points.Length; i < len; i++ )
				{
					p = points[ i ];
					p1.X = ( int )p.X;
					p1.Y = ( int )p.Y;

					p = ( len - 1 == i ) ? ( points[ 0 ] ) : ( points[ i + 1 ] );
					p2.X = ( int )p.X;
					p2.Y = ( int )p.Y;

					if( p1.X == p2.X )
					{
						DrawVerticalWaveLine( g, p1.X, p1.Y, p2.Y, color, false );
					}
					else if( p1.Y == p2.Y )
					{
						DrawHorizontalWaveLine( g, p1.Y, p1.X, p2.X, color, false );
					}
					else
					{
						throw new Exception( "Only vertical & horizontal lines can be drawn using wave style." );
					}
				}
			}
		}
		#endregion

		#region Internal Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="font"></param>
		/// <returns></returns>
		internal static IntPtr GetFontHandle( Font font )
		{
			IntPtr hFont;
			if( m_fontHandles.ContainsKey( font ) )
			{
				hFont = ( IntPtr )m_fontHandles[ font ];
			}
			else
			{
				hFont = font.ToHfont();
				m_fontHandles.Add( font, hFont );
			}
			return hFont;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Initializes all needed data.
		/// </summary>
		private static void Initialize()
		{
			if( !m_bInitialized )
			{
				DefaultFormat.FormatFlags = DEF_FORMAT_FLAGS;
				DefaultFormat.Trimming = StringTrimming.None;
				DefaultFormat.LineAlignment = StringAlignment.Near;
				DefaultFormat.Alignment = StringAlignment.Near;

				m_bInitialized = true;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <param name="bMeasureWholeWord"></param>
		/// <param name="g"></param>
		/// <param name="tabReplace"></param>
		/// <returns></returns>
		private static TextInfo MeasureStringGdiPlus( string text, Font font, bool bMeasureWholeWord, Graphics g, string tabReplace )
		{
			if( text == null || text.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_16, "text" );

			if( g == null )
			{
				g = DefaultGraphics;
			}

			TextInfo result = new TextInfo();
			result.Length = text.Length;

			Initialize();
			DefaultFormat.LineAlignment = StringAlignment.Near;

			if( !bMeasureWholeWord )
			{
				// MeasureCharacterRanges method doesn't work correctly: def. OT3908

				int len = text.Length;
				result.Characters = new CharInfo[ len ];
				SizeF tabSize = g.MeasureString( tabReplace, font, int.MaxValue, DefaultFormat );
				float width = 0;

				for( int i = 0; i < len; i++ )
				{
					char c = text[ i ];
					result.Characters[ i ].Char = c;
					SizeF size = ( c == '\t' ) ? ( tabSize ) : ( g.MeasureString( c.ToString(), font, int.MaxValue, DefaultFormat ) );

					result.Characters[ i ].CharLeft = width;
					result.Characters[ i ].CharWidth = size.Width;
					result.Height = Math.Max( result.Height, size.Height );
					width += size.Width;
				}

				result.Width = width;
			}
			else
			{
				text = text.Replace( "\t", tabReplace );
				SizeF size = g.MeasureString( text, font, int.MaxValue, DefaultFormat );
				result.Width = size.Width;
				result.Height = size.Height;
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <param name="bMeasureWholeWord"></param>
		/// <param name="g"></param>
		/// <param name="tabReplace"></param>
		/// <returns></returns>
		private static TextInfo MeasureStringNativeGdi( string text, Font font, bool bMeasureWholeWord, Graphics g, string tabReplace )
		{
			if( text == null || text.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_16, "text" );

			if( g == null )
			{
				g = DefaultGraphics;
			}

			TextInfo result = new TextInfo();
			result.Length = text.Length;

			Initialize();
			DefaultFormat.LineAlignment = StringAlignment.Near;

			SIZE size = new SIZE();
			IntPtr hdc = g.GetHdc();

			IntPtr hFont = GetFontHandle( font );

			IntPtr oldFont = GDIAppi.SelectObject( hdc, hFont );

			if( !bMeasureWholeWord )
			{
				result.Characters = new CharInfo[ result.Length ];

				for( int i = 0; i < result.Length; i++ )
				{
					result.Characters[ i ].Char = text[ i ];
					result.Characters[ i ].CharLeft = result.Width;

					string str = result.Characters[ i ].Char.ToString().Replace( "\t", tabReplace );
					GDIAppi.GetTextExtentPoint32( hdc, str, str.Length, ref size );

					result.Characters[ i ].CharWidth = size.cx;
					result.Width += size.cx;
					result.Height = Math.Max( size.cy, result.Height );
				}
			}
			else
			{
				text = text.Replace( "\t", tabReplace );

				GDIAppi.GetTextExtentPoint32( hdc, text, text.Length, ref size );
				result.Width = size.cx;
				result.Height = size.cy;
			}

			GDIAppi.SelectObject( hdc, oldFont );
			g.ReleaseHdc( hdc );
			return result;
		}
		#endregion
	}
}