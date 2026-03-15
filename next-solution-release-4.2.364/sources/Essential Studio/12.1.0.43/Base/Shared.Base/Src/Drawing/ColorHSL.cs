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

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// HSL color space.
	/// </summary>
	public class ColorHSL
	{
		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <param name="s"></param>
		/// <param name="l"></param>
		public ColorHSL( int h, int s, int l )
		{
			m_iH = h;
			m_iS = s;
			m_iL = l;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pColor"></param>
		public ColorHSL( Color pColor )
		{
			Initialize( pColor );
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Method converts Color defined in RGB values to HSL color space.
		/// </summary>
		/// <param name="hue"> Hue value. </param>
		/// <param name="sat"> Saturation value. </param>
		/// <param name="lum"> Luminance value. </param>
		public void Modify( int hue, int sat, int lum )
		{
			m_iH += hue;
			m_iS += sat;
			m_iL += lum;

			if( m_iH < 0 )
				m_iH = 0;

			if( m_iH > 240 )
				m_iH = 240;

			if( m_iS < 0 )
				m_iS = 0;

			if( m_iS > 240 )
				m_iS = 240;

			if( m_iL < 0 )
				m_iL = 0;

			if( m_iL > 240 )
				m_iL = 240;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		/// <returns></returns>
		private void Initialize( Color pColor )
		{
			double h = 0, s = 0, l = 0;

			// normalize red, green, blue values
			double r = ( double ) pColor.R / 255.0;
			double g = ( double ) pColor.G / 255.0;
			double b = ( double ) pColor.B / 255.0;

			double max = Math.Max( r, Math.Max( g, b ) );
			double min = Math.Min( r, Math.Min( g, b ) );

			// hue
			if( max == min )
			{
				h = 0; // undefined
			}
			else if( max == r && g >= b )
			{
				h = 60.0 * ( g - b ) / ( max - min );
			}
			else if( max == r && g < b )
			{
				h = 60.0 * ( g - b ) / ( max - min ) + 360.0;
			}
			else if( max == g )
			{
				h = 60.0 * ( b - r ) / ( max - min ) + 120.0;
			}
			else if( max == b )
			{
				h = 60.0 * ( r - g ) / ( max - min ) + 240.0;
			}

			// luminance
			l = ( max + min ) / 2.0;

			// saturation
			if( l == 0 || max == min )
			{
				s = 0;
			}
			else if( 0 < l && l <= 0.5 )
			{
				s = ( max - min ) / ( max + min );
			}
			else if( l > 0.5 )
			{
				s = ( max - min ) / ( 2 - ( max + min ) ); //(max-min > 0)?
			}

			m_iH = ( int ) ( h * 240 / 360 );
			m_iS = ( int ) ( s * 240 );
			m_iL = ( int ) ( l * 240 );
		}
		/// <summary>
		/// Method converts Color defined in HSL values to RGB color space.
		/// </summary>
		/// <param name="h"> Hue value. </param>
		/// <param name="s"> Saturation value. </param>
		/// <param name="l"> Luminance value. </param>
		/// <returns></returns>
		public Color ToRGB()
		{
			double Hue = ( double ) m_iH * 360 / 240;
			double Sat = ( double ) m_iS / 240;
			double Lum = ( double ) m_iL / 240;

			double q = ( Lum < 0.5 ) ? ( Lum * ( 1.0 + Sat ) ) : ( Lum + Sat - ( Lum * Sat ) );
			double p = ( 2.0 * Lum ) - q;

			double Hk = Hue / 360.0;
			double[] T = new double[ 3 ];
			T[ 0 ] = Hk + ( 1.0 / 3.0 );    // Tr
			T[ 1 ] = Hk;					// Tb
			T[ 2 ] = Hk - ( 1.0 / 3.0 );    // Tg

			for( int i = 0; i < 3; i++ )
			{
				if( T[ i ] < 0 ) T[ i ] += 1.0;
				if( T[ i ] > 1 ) T[ i ] -= 1.0;

				if( ( T[ i ] * 6 ) < 1 )
				{
					T[ i ] = p + ( ( q - p ) * 6.0 * T[ i ] );
				}
				else if( ( T[ i ] * 2.0 ) < 1 )
				{
					T[ i ] = q;
				}
				else if( ( T[ i ] * 3.0 ) < 2 )
				{
					T[ i ] = p + ( q - p ) * ( ( 2.0 / 3.0 ) - T[ i ] ) * 6.0;
				}
				else T[ i ] = p;
			}

			return Color.FromArgb( ( int ) ( T[ 0 ] * 255 ), ( int ) ( T[ 1 ] * 255 ), ( int ) ( T[ 2 ] * 255 ) );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets hue component of a color space. 
		/// </summary>
		public int H
		{
			get
			{
				return m_iH;
			}
			set
			{
				if (value > 240)
				{
					value = 240;
				}
				else if(value<0)
				{
					value = 0;
				}
				m_iH = value;
			}
		}
		/// <summary>
		/// Gets or sets saturation component of a color space. 
		/// </summary>
		public int S
		{
			get
			{
				return m_iS;
			}
			set
			{
				if (value > 240)
				{
					value = 240;
				}
				else if (value < 0)
				{
					value = 0;
				}
				m_iS = value;
			}
		}
		/// <summary>
		/// Gets or sets luminance component of a color space. 
		/// </summary>
		public int L
		{
			get 
			{ 
				return m_iL; 
			}
			set 
			{
				if (value > 240)
				{
					value = 240;
				}
				else if (value < 0)
				{
					value = 0;
				}
				m_iL = value; 
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// Hue component of a color space. 
		/// </summary>
		private int m_iH;
		/// <summary>
		/// Saturation component of a color space.
		/// </summary>
		private int m_iS;
		/// <summary>
		/// Luminance component of a color space.
		/// </summary>
		private int m_iL;
		#endregion
	}
}
