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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;

using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Design
{
	public class ControlBindingsConverter : 
		TypeConverter
	{
		// Methods
		public override /*TypeConverter*/ bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	    
		public override /*TypeConverter*/ PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (value is System.Windows.Forms.ControlBindingsCollection)
			{
				if(!(context.Instance is IDataBindingSupport))
				{
					throw new ArgumentException("The ControlBindingsConverter attribute can only be applied to properties in classes that implement IDataBindingSupport.");
				}

				ControlBindingsCollection cbc = value as System.Windows.Forms.ControlBindingsCollection;
				if(context != null && context.Instance != null)
				{
					PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(context.Instance, null);
					ArrayList newProps = new ArrayList();
					for(int i = 0; i < pdc.Count; i++)
					{
						IDataBindingSupport dbs = context.Instance as IDataBindingSupport;
						
						// Syncfusion change:
						Attribute[] attrs = new Attribute[2];
						attrs[0] = new BrowsableAttribute(false);
						attrs[1] = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);

						DesignBindingPropertyDescriptor dbpd 
							= new DesignBindingPropertyDescriptor(dbs.PropertyNamePrefix + "." + pdc[i].Name, pdc[i].Name, pdc[i], attrs);

						bool bindable = ((BindableAttribute)(pdc[i].Attributes[typeof(System.ComponentModel.BindableAttribute)])).Bindable;
						if(bindable || !((DesignBinding)(dbpd.GetValue(cbc))).IsNull)
						{
							newProps.Add(dbpd);
						}
					}
					PropertyDescriptor[] pds = new PropertyDescriptor[newProps.Count];
					newProps.CopyTo(pds, 0);
					return new PropertyDescriptorCollection(pds);
				}
			}

			return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
		}
	}
}