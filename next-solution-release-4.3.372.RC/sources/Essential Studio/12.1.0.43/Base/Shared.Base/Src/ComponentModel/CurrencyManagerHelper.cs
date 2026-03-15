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
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Collections;
using Syncfusion.Windows.Forms;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{


	/// <summary>
	/// Provides support for the <see cref="GetCurrencyManager"/> method that returns a <see cref="CurrencyManager"/>.
	/// </summary>
	public interface ICurrencyManagerSource
	{
		/// <summary>
		/// Returns a <see cref="CurrencyManager"/> that is associated with the current object.
		/// </summary>
		CurrencyManager GetCurrencyManager();
	}

	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CurrencyManagerMappingNameConverter : TypeConverter
	{
		private bool PropertyDescriptorIsARelation(PropertyDescriptor prop) 
		{
			if (typeof(IList).IsAssignableFrom(prop.PropertyType))
				return !(typeof(Array).IsAssignableFrom(prop.PropertyType));
			return false;
		}


		/// <override/>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)  
		{
			if (context != null)
			{
				PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
				CurrencyManager lm = context.Instance as CurrencyManager;
				if (lm == null)
				{
					ICurrencyManagerSource gcs = context.Instance as ICurrencyManagerSource;
					if (gcs != null)
						lm = gcs.GetCurrencyManager();

					if (lm != null)
						pdc = lm.GetItemProperties();
					else
					{
						IItemPropertiesSource ips = context.Instance as IItemPropertiesSource;
						if (ips != null)
							pdc = ips.GetItemProperties();
						else if (context.Instance is ITypedList)
							pdc = ((ITypedList) context.Instance).GetItemProperties(null);
					}
				}

				if (pdc.Count > 0)
				{
					ArrayList keys = new ArrayList();
					int count = pdc.Count;
					for (int index = 0; index < count; index++) 
					{
						PropertyDescriptor pd = pdc[index];
						if (pd.IsBrowsable) 
						{
							if (this.PropertyDescriptorIsARelation(pd))
							{
								//this.relationsList.Add(pd.Name);
							}
							else 
							{
								keys.Add(pd.Name);
							}
						}
					}

					return new TypeConverter.StandardValuesCollection(keys);
				}
			}

			return new TypeConverter.StandardValuesCollection(new string[] { "" } );
		}


		/// <override/>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;	// enables support for late bound scenario
		}

		/// <override/>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <override/>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		} 

		/// <override/>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
				return (string) value;

			return base.ConvertFrom(context, culture, value);
		} 
	}
}
