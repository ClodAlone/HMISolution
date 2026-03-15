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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	/// <summary>
	/// TypeConverter for ToolStripItemAdvInfo.
	/// </summary>
	class ToolStripItemAdvInfoTypeConverter
		: TypeConverter
	{
		#region Overrides
		/// <summary>
		/// Checks whether type can be converted.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="destinationType"></param>
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
		/// Converts types.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(
			ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				Type type = value.GetType();
				FieldInfo fi = type.GetField("Item");
				if(fi!=null)
				{
					System.Windows.Forms.ToolStripItem item = fi.GetValue(value) as System.Windows.Forms.ToolStripItem;

					ConstructorInfo ci = type.GetConstructor(new Type[] { typeof(System.Windows.Forms.ToolStripItem) });
					return new InstanceDescriptor(ci, new object[] { item }, true);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		#endregion
	}
}
#endif
