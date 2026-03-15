#if !WinRT
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
using System.ComponentModel;
using System.Globalization;

namespace Syncfusion.Windows.Styles
{
	/// <summary>
	///    <para>Provides a type converter to convert expandable objects to and from various
	///       other representations.</para>
	/// </summary>
	public class StyleInfoBaseConverter : 
		TypeConverter
	{
   
        
		/// <summary>
		///    <para>Indicates whether this converter can
		///       convert an object to the given destination type using the specified context.</para>
		/// </summary>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		} // end of method CanConvertTo

		/// <summary>
		///    <para>Converts the given value object to
		///       the specified destination type using the specified context and arguments.</para>
		/// </summary>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return string.Empty;
		} // end of method ConvertTo


	} 

}
#endif