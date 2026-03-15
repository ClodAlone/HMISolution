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
using System.Web.UI.Design;
using System.Xml.Serialization;

namespace Syncfusion.Design
{
	[Documentation.DocumentationExclude()]
	public class UseShouldSerializeAttribute : Attribute
	{
	}

	/// <summary>
	/// If the static DesignerPersistance.IsPersisting property gets set, then deriving from this type will do the following:
	/// It will return a custom property descriptor for the properties it contains that will return property values
	/// after checking the ShouldSerializeXXX impl. This is useful while serializing web pages in the designer - as the
	/// web designer doesn't check for ShouldSerializeXXX, by default.
	/// </summary>
	[Documentation.DocumentationExclude()]
	public abstract class ShouldSerializeBasedPersisterType 
//#if SyncfusionFramework2_0
//#else
		: ICustomTypeDescriptor
//#endif
	{
		public ShouldSerializeBasedPersisterType()
		{
		}

		[Syncfusion.Documentation.DocumentationExclude]
		[XmlIgnore()]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public virtual bool CheckAllProperties { get { return false; } }
#else
		public virtual bool CheckAllProperties{get {return true;}}
#endif
		
		protected virtual int GetNewAttributesCount(PropertyDescriptor curPD)
		{
			return 0;
		}

		protected virtual void AddNewAttributes(PropertyDescriptor curPD, ref Attribute[] atts, int startIndex)
		{

		}

		protected virtual PropertyDescriptorCollection GetCustomPDC(PropertyDescriptorCollection baseprops)
		{
			PropertyDescriptor[] newprops = new PropertyDescriptor[baseprops.Count];

			int i = -1;
			foreach(PropertyDescriptor oldPD in baseprops)
			{
				i++;

				if(this.CheckAllProperties || oldPD.Attributes[typeof(UseShouldSerializeAttribute)] != null)
				{
					Attribute[] atts = new Attribute[oldPD.Attributes.Count + this.GetNewAttributesCount(oldPD)];
					oldPD.Attributes.CopyTo(atts, 0);
					this.AddNewAttributes(oldPD, ref atts, oldPD.Attributes.Count);
					ShouldSerializeCheckingPropertyDescriptor newDesc = new ShouldSerializeCheckingPropertyDescriptor(oldPD, atts);
					newprops[i] = newDesc;
				}
				else
					newprops[i] = oldPD;
			}

			return new PropertyDescriptorCollection(newprops);
		}

		
//#if SyncfusionFramework2_0
//#else
		#region ICustomTypeDescriptor Members

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()  
		{ 
			if(!DesignerPersistance.IsPersisting)
				return TypeDescriptor.GetProperties(this, true);
			else
			{
				PropertyDescriptorCollection baseprops = TypeDescriptor.GetProperties(this, true); 
 
				return this.GetCustomPDC(baseprops);
			}
		}
 
		
		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties(System.Attribute[] attributes) 
		{
			if(!DesignerPersistance.IsPersisting)
				return TypeDescriptor.GetProperties(this, attributes, true);
			else			
			{
				PropertyDescriptorCollection baseprops = TypeDescriptor.GetProperties(this, attributes, true);
				
				return this.GetCustomPDC(baseprops);
				
			}
		}

		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{  
			return TypeDescriptor.GetAttributes(this, true);  
		}
 
		
		string ICustomTypeDescriptor.GetClassName()  
		{  
			return TypeDescriptor.GetClassName(this, true);
		}           
 
		
		string ICustomTypeDescriptor.GetComponentName()
		{ 
			return TypeDescriptor.GetComponentName(this, true);  
		}
 
		
		TypeConverter ICustomTypeDescriptor.GetConverter() 
		{ 
			return TypeDescriptor.GetConverter(this, true);  
		}
 
		
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()  
		{ 
			return TypeDescriptor.GetDefaultEvent(this, true);  
		}
 
		
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(System.Attribute[] attributes)  
		{ 
			return TypeDescriptor.GetEvents(this, attributes, true);  
		} 
 
		
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() 
		{ 
			return TypeDescriptor.GetEvents(this, true);  
		}
 
		
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()  
		{ 
			return TypeDescriptor.GetDefaultProperty(this, true);  
		}
 

		object ICustomTypeDescriptor.GetEditor(System.Type editorBaseType)  
		{ 
			return TypeDescriptor.GetEditor(this, editorBaseType, true);  
		}
 
		
		object ICustomTypeDescriptor.GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd)  
		{ 
			return this;  
		}


		#endregion
//#endif
	}
}