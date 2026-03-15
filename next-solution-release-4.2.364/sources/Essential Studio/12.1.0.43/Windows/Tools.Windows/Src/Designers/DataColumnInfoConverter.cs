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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// TypeConverter for DataColumnInfo.
	/// </summary>
	public class DataColumnInfoConverter : TypeConverter
	{
		/// <summary>
		/// Indicates whether this converter can convert an object to
		/// the given destination type using the context.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext object that provides a format context. </param>
		/// <param name="destinationType">A <see cref="Type"/> object that represents the type to which you want to convert. </param>
		/// <returns></returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}

			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
        /// This member overrides <see cref="TypeConverter.ConvertTo(System.ComponentModel.ITypeDescriptorContext, System.Globalization.CultureInfo, Object, System.Type)"/>.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">A CultureInfo object. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed. </param>
		/// <param name="value">The Object to convert.</param>
		/// <param name="destinationType">The Type to convert the value parameter to.</param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) 
			{
				AutoCompleteDataColumnInfo info = value as AutoCompleteDataColumnInfo;

				return new InstanceDescriptor(typeof(AutoCompleteDataColumnInfo).GetConstructor
					(new Type[3] {typeof(string),typeof(int),typeof(bool)}), 
					new object[3] {info.ColumnHeaderText,info.MinColumnWidth,true});
			}
			return base.ConvertTo(context, culture, value, destinationType);

		}
	}

	/// <summary>
	/// TypeConverter for AutoCompleteTarget.
	/// </summary>
	public class AutoCompleteTargetConverter : TypeConverter
	{
		/// <summary>
		/// Indicates whether this converter can convert an object to
		/// the given destination type using the context.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext object that provides a format context. </param>
		/// <param name="destinationType">A <see cref="Type"/> object that represents the type to which you want to convert. </param>
		/// <returns></returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}

			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// This member overrides <see cref="TypeConverter.ConvertTo(System.ComponentModel.ITypeDescriptorContext, System.Globalization.CultureInfo, Object, System.Type)"/>.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">A CultureInfo object. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed. </param>
		/// <param name="value">The Object to convert.</param>
		/// <param name="destinationType">The Type to convert the value parameter to.</param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) 
			{
				AutoCompleteTarget info = value as AutoCompleteTarget;

				return new InstanceDescriptor(typeof(AutoCompleteDataColumnInfo).GetConstructor
					(new Type[2] {typeof(Control),typeof(AutoCompleteModes)}), 
					new object[2] {info.EditControl ,info.AutoCompleteMode });
			}
			return base.ConvertTo(context, culture, value, destinationType);

		}
	}
}
