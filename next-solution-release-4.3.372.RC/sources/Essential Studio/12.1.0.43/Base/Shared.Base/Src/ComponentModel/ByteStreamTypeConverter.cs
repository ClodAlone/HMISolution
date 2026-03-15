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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using Syncfusion.Runtime.Serialization;

namespace Syncfusion.ComponentModel
{
	/// <summary>
	/// Special type converter that can convert the associated type to a byte array and vice-versa when
	/// requested by the design-time, for example.
	/// </summary>
	public class ByteStreamTypeConverter : ExpandableObjectConverter
	{
		public virtual void OnBeforeDeserialize()
		{
		}

		public virtual void OnAfterDeserialize()
		{
		}


		/// <override/>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(byte[]))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}

		/// <override/>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			byte[] bytes = value as byte[];
			
				if(bytes == null)
					return base.ConvertFrom(context, culture, value);
			
			object o = null;
			MemoryStream ms = new MemoryStream(bytes);
			BinaryFormatter bf = new BinaryFormatter();
			bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
			bf.Binder = AppStateSerializer.CustomBinder;

			this.OnBeforeDeserialize();

			o = bf.Deserialize(ms);

			this.OnAfterDeserialize();

			ms.Close();
		
			return o;
		}

		/// <override/>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if(destinationType == typeof(byte[]))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		}
		
		/// <override/>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if(value != null && destinationType == typeof(byte[]))
			{
				MemoryStream ms = new MemoryStream();
				BinaryFormatter bf = new BinaryFormatter();
				bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
				
				bf.Serialize(ms, value);
				byte[] bytes = ms.ToArray();
				ms.Close();
				return bytes;
			}
			else
				return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}