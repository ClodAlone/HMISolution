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
	/// HSV color space.
	/// </summary>
	public class ColorHSV
	{
		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <param name="s"></param>
		/// <param name="l"></param>
		public ColorHSV( int h, int s, int v )
		{
			m_iH = h;
			m_iS = s;
			m_iV = v;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pColor"></param>
		public ColorHSV( Color pColor )
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
		public void Modify( int hue, int sat, int val )
		{
			m_iH += hue;
			m_iS += sat;
			m_iV += val;

			if( m_iH < 0 )
				m_iH = 0;

			if( m_iH > 240 )
				m_iH = 240;

			if( m_iS < 0 )
				m_iS = 0;

			if( m_iS > 240 )
				m_iS = 240;

			if( m_iV < 0 )
				m_iV = 0;

			if( m_iV > 240 )
				m_iV = 240;
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
			double r = ( double ) pColor.R / 255;
			double g = ( double ) pColor.G / 255;
			double b = ( double ) pColor.B / 255;

			double max = Math.Max( r, Math.Max( g, b ) );
			double min = Math.Min( r, Math.Min( g, b ) );
			double delta = max - min;
			double h, s, v;

			v = max; // value

			if( max != 0 )
				s = delta / max;		// saturation
			else
			{
				// r = g = b = 0		// s = 0, v is undefined
				s = 0;
				h = -1;
				return;
			}

			if( delta == 0 )
				h = 0;
			else if( r == max )
				h = ( g - b ) / delta;		// between yellow & magenta
			else if( g == max )
				h = 2 + ( b - r ) / delta;	// between cyan & yellow
			else
				h = 4 + ( r - g ) / delta;	// between magenta & cyan

			h *= 60;				// degrees
			if( h < 0 )
				h += 360;

			m_iH = ( int ) ( h + 0.5f );
			m_iS = ( int ) ( s * 255 + 0.5f );
			m_iV = ( int ) ( v * 255 + 0.5f );
		}
		/// <summary>
		/// Method converts Color defined in HSV values to RGB color space.
		/// </summary>
		/// <returns></returns>
		public Color ToRGB()
		{
			double h = m_iH;
			double s = ( double ) m_iS / 255;
			double v = ( double ) m_iV / 255;

			int i;
			double f, p, q, t, r, g, b;

			if( s == 0 )
			{
				// achromatic (grey)
				return Color.FromArgb( m_iV, m_iV, m_iV );
			}

			h /= 60;			// sector 0 to 5
			i = ( int )Math.Floor( h );
			f = h - i;			// factorial part of h
			p = v * ( 1 - s );
			q = v * ( 1 - s * f );
			t = v * ( 1 - s * ( 1 - f ) );

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
				default:		// case 5:
					r = v;
					g = p;
					b = q;
					break;
			}

			return Color.FromArgb( 
				( int ) ( r * 255 + 0.5f ),
				( int ) ( g * 255 + 0.5f ),
				( int ) ( b * 255 + 0.5f ) );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets hue component of a color space. 
		/// </summary>
		public int H
		{
			get { return m_iH; }
			set { m_iH = value; }
		}
		/// <summary>
		/// Gets or sets saturation component of a color space. 
		/// </summary>
		public int S
		{
			get { return m_iS; }
			set { m_iS = value; }
		}
		/// <summary>
		/// Gets or sets value component of a color space. 
		/// </summary>
		public int V
		{
			get { return m_iV; }
			set { m_iV = value; }
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
		/// Value component of a color space.
		/// </summary>
		private int m_iV;
		#endregion
	}
}
