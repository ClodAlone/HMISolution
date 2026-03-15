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
using System.Drawing.Drawing2D;

using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class for storing data about one format in strings.
	/// </summary>
	[Serializable]
	public class TextFormatInfo
	{
		#region Class Constants
		/// <summary>
		/// Prefix for known color string representation.
		/// </summary>
		private string DEF_KNOWN_COLOR_PREFIX = "known: ";
		/// <summary>
		/// Prefix for Argb color string representation.
		/// </summary>
		private string DEF_ARGB_COLOR_PREFIX = "argb: ";
		#endregion

		#region Class Public Members
		/// <summary>
		/// Font size.
		/// </summary>
		public string FontSize;
		/// <summary>
		/// Font name.
		/// </summary>
		public string FontName;
		/// <summary>
		/// Font style.
		/// </summary>
		public string FontStyleLoc;
		/// <summary>
		/// Font color.
		/// </summary>
		public string FontColor;
		/// <summary>
		/// Background color.
		/// </summary>
		public string BackColor;
		/// <summary>
		/// Fore color (the second background color in certain hatch styles), string representation.
		/// </summary>
		public string ForeColor;
		/// <summary>
		/// Background style.
		/// </summary>
		public string BackgroundStyle;
		/// <summary>
		/// Underline weight.
		/// </summary>
		public string UnderlineWeightLoc;
		/// <summary>
		/// Underline color.
		/// </summary>
		public string UnderlineColor;
		/// <summary>
		/// Underline style.
		/// </summary>
		public string UnderlineStyleLoc;
		/// <summary>
		/// Text strike out color.
		/// </summary>
		public string StrikeOutColor;
		/// <summary>
		/// Border weight.
		/// </summary>
		public string BorderWeightLoc;
		/// <summary>
		/// Border color.
		/// </summary>
		public string BorderColor;
		/// <summary>
		/// Underline style.
		/// </summary>
		public string BorderStyle;
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates and initializes new instance of TextFormatInfo.
		/// </summary>
		/// <param name="format">Format instance to extract data from.</param>
		public TextFormatInfo( Format format )
		{
			if( format == null ) throw new ArgumentNullException( "format" );

			ExtractFormatInfo( format );
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Extracts info about format.
		/// </summary>
		/// <param name="format">Format to extract info from.</param>
		public void ExtractFormatInfo( Format format )
		{
			if( format == null ) throw new ArgumentNullException( "format" );

			this.FontSize = format.Font.SizeInPoints.ToString();
			this.FontName = format.Font.FontFamily.Name;
			this.FontStyleLoc = GetFontStyle( format.Font.Bold, format.Font.Italic ).ToString();
			this.FontColor = GetStringByColor( format.FontColor );
			this.BackColor = GetStringByColor( format.BackColor );
			this.ForeColor = GetStringByColor( format.ForeColor );
			this.BackgroundStyle = ( format.UseHatchFill ) ? ( format.HatchStyle.ToString() ) : ( string.Empty );
			this.UnderlineWeightLoc = format.UnderlineWeight.ToString();
			this.UnderlineColor = GetStringByColor( format.LineColor );
			this.UnderlineStyleLoc = format.UnderlineStyle.ToString();
			this.StrikeOutColor = GetStringByColor( format.StrikeOutColor );
			this.BorderWeightLoc = format.BorderWeight.ToString();
			this.BorderColor = GetStringByColor( format.BorderColor );
			this.BorderStyle = format.BorderStyle.ToString();
		}
		/// <summary>
		/// Applies info to specified format instance.
		/// </summary>
		/// <param name="format">Format instance to apply info to.</param>
		public void ApplyFormatInfo( Format format )
		{
			if( format == null ) throw new ArgumentNullException( "format" );

			float fontSize = float.Parse( this.FontSize );
			FontStyle fontStyle = ( FontStyle )Enum.Parse( typeof( FontStyle ), this.FontStyleLoc );
			FontStyle formatFontStyle = GetFontStyle( format.Font.Bold, format.Font.Italic );

			if( this.FontName != format.Font.FontFamily.Name
				|| fontSize != format.Font.SizeInPoints
				|| fontStyle != formatFontStyle )
			{
				format.Font = new Font( this.FontName, fontSize, fontStyle );
			}

			format.FontColor = GetColorByString( this.FontColor );
			format.BackColor = GetColorByString( this.BackColor );
			format.ForeColor = GetColorByString( this.ForeColor );

			if( this.BackgroundStyle != string.Empty )
			{
				format.UseHatchFill = true;
				format.HatchStyle = ( HatchStyle )Enum.Parse( typeof( HatchStyle ), this.BackgroundStyle );
			}
			else
			{
				format.UseHatchFill = false;
			}

			format.UnderlineWeight = ( UnderlineWeight )Enum.Parse( typeof( UnderlineWeight ), this.UnderlineWeightLoc );
			format.LineColor = GetColorByString( this.UnderlineColor );
			format.UnderlineStyle = ( UnderlineStyle )Enum.Parse( typeof( UnderlineStyle ), this.UnderlineStyleLoc );
			format.StrikeOutColor = GetColorByString( this.StrikeOutColor );
			format.BorderWeight = ( BorderWeight )Enum.Parse( typeof( BorderWeight ), this.BorderWeightLoc );
			format.BorderColor = GetColorByString( this.BorderColor );
			format.BorderStyle = ( FrameBorderStyle )Enum.Parse( typeof( FrameBorderStyle ), this.BorderStyle );
		}
		#endregion

		#region Class Private Methods
		/// <summary>
		/// Converts bold and italic values combination to FontStyle.
		/// </summary>
		/// <param name="bBold">Font style bold value.</param>
		/// <param name="bItalic">Font style italic value.</param>
		/// <returns>FontStyle value.</returns>
		private FontStyle GetFontStyle( bool bBold, bool bItalic )
		{
			FontStyle result = 0;

			if( bBold ) result |= FontStyle.Bold;
			if( bItalic ) result |= FontStyle.Italic;

			return result;
		}
		/// <summary>
		/// Gets string representation of color.
		/// </summary>
		/// <param name="color">Color to transform to string.</param>
		/// <returns>String with text representation of color.</returns>
		private string GetStringByColor( Color color )
		{
			if( color.IsEmpty ) return string.Empty;

			KnownColor known = color.ToKnownColor();

			if( known != 0 )
			{
				return DEF_KNOWN_COLOR_PREFIX + known.ToString();
			}
			else
			{
				return DEF_ARGB_COLOR_PREFIX + color.ToArgb();
			}
		}
		/// <summary>
		/// Gets color represented in string.
		/// </summary>
		/// <param name="strColor">String with text color representation.</param>
		/// <returns>Color from string.</returns>
		private Color GetColorByString( string strColor )
		{
			if( strColor == null ) throw new ArgumentNullException( "strColor" );

			if( strColor == string.Empty ) return Color.Empty;

			if( strColor.StartsWith( DEF_KNOWN_COLOR_PREFIX ) )
			{
				strColor = strColor.Remove( 0, DEF_KNOWN_COLOR_PREFIX.Length );
				return Color.FromKnownColor( ( KnownColor )Enum.Parse( typeof( KnownColor ), strColor ) );
			}

			if( strColor.StartsWith( DEF_ARGB_COLOR_PREFIX ) )
			{
				strColor = strColor.Remove( 0, DEF_ARGB_COLOR_PREFIX.Length );
				return Color.FromArgb( int.Parse( strColor ) );
			}

			throw new ArgumentException( "strColor" );
		}
		#endregion
	}
}