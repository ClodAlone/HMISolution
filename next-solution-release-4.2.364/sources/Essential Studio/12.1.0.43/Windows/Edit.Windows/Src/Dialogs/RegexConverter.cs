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
using System.Globalization;
using System.Diagnostics;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Designer tool. Allows edit regex as string.
	/// </summary>
	public class RegexConverter
		: TypeConverter
	{
		#region Constants
		/// <summary>
		/// Regex options.
		/// </summary>
		private const RegexOptions DEF_REGEX = Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX;
		#endregion

		#region Overrides
		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="sourceType">A Type that represents the type you want to convert from.</param>
		/// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
		public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
		{
			if( sourceType == typeof( string ) )
				return true;

			return base.CanConvertFrom( context, sourceType );
		}
		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">The CultureInfo to use as the current culture.</param>
		/// <param name="value">The Object to convert.</param>
		/// <returns>An Object that represents the converted value.</returns>
		public override object ConvertFrom( ITypeDescriptorContext context,
			CultureInfo culture, object value )
		{
			if( value is string )
			{
				try
				{
					Regex reg = new Regex( ( string )value, DEF_REGEX );
					return reg;
				}
				catch( ArgumentException ex )
				{
					Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Convert Error" );
					return null;
				}
			}

			return base.ConvertFrom( context, culture, value );
		}
		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">A CultureInfo. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed.</param>
		/// <param name="value">The Object to convert.</param>
		/// <param name="destinationType">The Type to convert the value parameter to.</param>
		/// <returns>An Object that represents the converted value.</returns>
		public override object ConvertTo( ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType )
		{
			if( destinationType == typeof( string ) )
			{
				return ( ( Regex )value ).ToString();
			}

			return base.ConvertTo( context, culture, value, destinationType );
		}

		/// <summary>
		///	Returns whether the given value object is valid for this type and for the specified context.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="value">The Object to test for validity.</param>
		/// <returns>True if the specified value is valid for this object; otherwise, false.</returns>
		public override bool IsValid( ITypeDescriptorContext context, object value )
		{
			try
			{
				if( value is string )
				{
					Regex reg = new Regex( ( string )value, DEF_REGEX );
					return true;
				}
			}
			catch( ArgumentException ex )
			{
				Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Convert Error" );
			}

			return false;
		}
		#endregion
	}
}