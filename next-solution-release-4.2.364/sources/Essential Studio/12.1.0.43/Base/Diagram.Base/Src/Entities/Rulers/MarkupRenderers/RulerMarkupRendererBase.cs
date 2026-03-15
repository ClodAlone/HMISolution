#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

#region File using derectives

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Windows.Forms.Diagram;
using System.Collections.Generic;

#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Provides the scale for the rulers.
	/// </summary>
	public abstract class RulerMarkupRenderer
	{
		#region Class members
		/// <summary>
		/// Store ruler to mark up.
		/// </summary>
		private Ruler m_ruler;
		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// Creates instance of RulerMarkupRenderer
		/// </summary>
		/// <param name="ruler"></param>
		public RulerMarkupRenderer( Ruler ruler )
		{
			if( ruler == null )
			{
				throw new ArgumentNullException( "ruler" );
			}
			
			m_ruler = ruler;
		}
		#endregion
		
		#region Class properties
		/// <summary>
		/// Gets precession in pixels.
		/// </summary>
		public virtual float PixelInUnit
		{
			get
			{
				return MeasureUnitsConverter.ToPixelX( Precision, m_ruler.MeasureUnit );
			}
		}
		/// <summary>
		/// Gets minimum particle of the ruler.
		/// </summary>
		public virtual float Precision
		{
			get
			{
				return 1F;
			}
		}
		/// <summary>
		/// Gets specified precision for round mark.
		/// </summary>
		public virtual int RoundPrecision
		{
			get
			{
				return 0;
			}
		}
		/// <summary>
		/// Gets numeric number format.
		/// </summary>
		public virtual string NumericFormat
		{
			get
			{
				return string.Empty;
			}
		}
		#endregion
		
		#region Class overrides
		/// <summary>
		/// Render scale on ruler 
		/// </summary>
		/// <param name="gfx">Graphics render to</param>
		/// <param name="bounds">Bounds of the ruler</param>
        /// <param name="view">Diagram view</param>
		public void RenderMarkup( Graphics gfx, RectangleF bounds, View view )
		{
			if ( m_ruler.Size.Width > m_ruler.Size.Height )
			{
				DrawHorizontalRuler( gfx, bounds, view );
			}
			else
			{
				DrawVerticalRuler( gfx, bounds, view );
			}
		}
		/// <summary>
		/// Render scale on horizontal ruler 
		/// </summary>
		/// <param name="gfx">Graphics render to</param>
		/// <param name="bounds">Bounds of the ruler</param>
        /// <param name="view">Diagram view</param>
		protected void DrawHorizontalRuler( Graphics gfx, RectangleF bounds ,View view )
		{
            float realPageScale = view.Magnification / 100;
			float koef = PixelInUnit;
			float step = PixelInUnit;
            float fontSize = MeasureUnitsConverter.FromPixelX(gfx.PageScale * (m_ruler.Size.Height / 2 - 3), MeasureUnits.Point);

            if (fontSize > 0)
            using (Font font = new Font(FontFamily.GenericSansSerif, m_ruler.Size.Height / 2 - 3))
			{
				float strLenght = gfx.MeasureString( "0000000000", font ).Width / realPageScale;

				while( ( step / strLenght ) < 1F )
				{
					step += koef;
				}

                if (view.Grid.HorizontalSpacing < m_ruler.MajorCalibratedLineUnits)
                    step = MeasureUnitsConverter.ToPixelX(m_ruler.MajorCalibratedLineUnits, m_ruler.MeasureUnit);
                else
                    step = MeasureUnitsConverter.ToPixelX(view.Grid.HorizontalSpacing, m_ruler.MeasureUnit);

				float incStep = step / 10 * realPageScale;
				int j = 0;

				using( Pen pnMinorLines = new Pen( m_ruler.MinorLinesColor, 1 ) )
				using( Pen pnMajorLines = new Pen( m_ruler.MajorLinesColor, 1 ) )
				{
					for( float i = bounds.X; i <= m_ruler.Location.X + m_ruler.Size.Width; i += incStep )
					{
						gfx.DrawLine( pnMinorLines, i, 0, i, m_ruler.Size.Height / 2 - 2 );

						if( j % 5 == 0 )
						{
							gfx.DrawLine( pnMinorLines, i, 0, i, m_ruler.Size.Height / 2 );
						}

						if( j % 10 == 0 )
						{
							gfx.DrawLine( pnMajorLines, i, 0, i, m_ruler.Size.Height - 5);	
							gfx.DrawString( ( Math.Round( ( i - bounds.X ) / PixelInUnit / realPageScale * Precision, RoundPrecision ) ).ToString( NumericFormat ), 
								font, pnMajorLines.Brush, i + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( font.Size, MeasureUnits.Point ) - 2 );
						}

						j++;
					}

					float left = bounds.X - incStep;
					j = 1;
					while( left > 0 )
					{
						gfx.DrawLine( pnMinorLines, left, 0, left, m_ruler.Size.Height / 2 - 2 );

						if( j % 5 == 0 )
						{
							gfx.DrawLine( pnMinorLines, left, 0, left, m_ruler.Size.Height / 2 );
						}	

						if( j % 10 == 0 )
						{
							gfx.DrawLine( pnMajorLines, left, 0, left, m_ruler.Size.Height - 5);	
							gfx.DrawString( ( Math.Round( ( left - bounds.X ) / PixelInUnit / realPageScale * Precision, RoundPrecision ) ) .ToString( NumericFormat ), 
								font, pnMajorLines.Brush, left + 1, m_ruler.Size.Height - MeasureUnitsConverter.ToPixelY( font.Size, MeasureUnits.Point ) - 2 );
						}

						j++;
						left -= incStep;

					}
				}
			}
		}
		/// <summary>
		/// Render scale on vertical ruler 
		/// </summary>
		/// <param name="gfx">Graphics render to</param>
		/// <param name="bounds">Bounds of the ruler</param>
        /// <param name="view">Diagram view</param>
		protected virtual void DrawVerticalRuler( Graphics gfx, RectangleF bounds ,View view )
		{
            float realPageScale = view.Magnification / 100;
			float koef = PixelInUnit;
			float step = PixelInUnit;
            float fontSize = MeasureUnitsConverter.FromPixelX(gfx.PageScale * (m_ruler.Size.Width / 2 -3), MeasureUnits.Point);

            if (fontSize > 0)
            using (Font font = new Font(FontFamily.GenericSansSerif, m_ruler.Size.Width / 2 - 3))
			{
				float strLenght = gfx.MeasureString( "0000000000", font ).Width / realPageScale;

				while( ( step / strLenght ) < 1F )
				{
					step += koef;
				}

                if (view.Grid.VerticalSpacing < m_ruler.MajorCalibratedLineUnits)
                    step = MeasureUnitsConverter.ToPixelX(m_ruler.MajorCalibratedLineUnits, m_ruler.MeasureUnit);
                else
                    step = MeasureUnitsConverter.ToPixelX(view.Grid.VerticalSpacing, m_ruler.MeasureUnit);

				float incStep = step / 10 * realPageScale;
				int j = 0;

				using( StringFormat drawFormat = new StringFormat() )
				{
					drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

					using( Pen pnMinorLines = new Pen( m_ruler.MinorLinesColor, 1 ) )
					using( Pen pnMajorLines = new Pen( m_ruler.MajorLinesColor, 1 ) )
					{
						for( float i = bounds.Y; i <= m_ruler.Location.Y + m_ruler.Size.Height; i += incStep )
						{
							gfx.DrawLine( pnMinorLines, 0, i, m_ruler.Size.Width / 2 - 2, i );

							if( j % 5 == 0 )
							{
								gfx.DrawLine( pnMinorLines, 0, i, m_ruler.Size.Width / 2, i );
							}

							if( j % 10 == 0 )
							{
								gfx.DrawLine( pnMajorLines,  0, i, m_ruler.Size.Width - 5, i );	
								gfx.DrawString( ( Math.Round( ( i - bounds.Y ) / PixelInUnit / realPageScale * Precision, RoundPrecision ) ).ToString( NumericFormat ), 
									font, pnMajorLines.Brush, m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( font.Size, MeasureUnits.Point ) - 4 , i + 1, drawFormat );
							}

							j++;
						}

						float left = bounds.Y - incStep;
						j = 1;
						while( left > 0 )
						{
							gfx.DrawLine( pnMinorLines, 0, left, m_ruler.Size.Width / 2 - 2, left );

							if( j % 5 == 0 )
							{
								gfx.DrawLine( pnMajorLines, 0, left, m_ruler.Size.Width / 2, left );
							}

							if( j % 10 == 0 )
							{
								gfx.DrawLine( pnMajorLines,  0, left, m_ruler.Size.Width - 5, left );
								gfx.DrawString(  ( Math.Round( ( left - bounds.Y ) / PixelInUnit / realPageScale * Precision, RoundPrecision ) ) .ToString( NumericFormat ), 
									font, pnMajorLines.Brush, m_ruler.Size.Width - MeasureUnitsConverter.ToPixelX( font.Size, MeasureUnits.Point ) - 3, left + 1, drawFormat );
							}

							j++;
							left -= incStep;

						}
					}
				}
			}
		}
		#endregion
	}
}
