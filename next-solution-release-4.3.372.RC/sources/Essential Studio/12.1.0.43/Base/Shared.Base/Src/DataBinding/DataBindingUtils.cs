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

namespace Syncfusion.Windows.Forms
{
	// The Secondary DataBinding framework for component-based classes falls back 
	// on a control-derived class's DataBinding capabilities.
	// Hence this can be used only when a component is associated (usually parented) statically by a control
	// (for example: a main-menu (Component derived) and its form (control derived) or
	// a GroupBarItem and a GroupBarControl.
	// Here are the steps for enabling DataBinding in such components:
	// 1) Dynamically insert properties into the parent control:
	//		Make the parent control implement ICustomTypeDescriptor and insert a 
	//		custom property for each child component's bindable property.
	// 2) Use Custom PropertyDescriptors that act as a bridge:
	//		The above custom property will be implemented using a custom PropertyDescriptor
	//		that will handle transfer of data to and from the component's property.
	// 3) Implement IDataBindingSupport in the Component:
	//		This will let you specify the unique prefix in the dynamic property that will identify this component 
	//		(from others parented by the same control, if any).
	// 4) Enable design time support in the component:
	//		To enable design-time support in the component class, include the DataBinding's
	//		property which will return the parent control's list. Make the ControlBindingsConverter (in Shared.Design.dll)
	//		its TypeConverter.
	[Documentation.DocumentationExclude()]
	public class DataBindingUtils
	{
		/// <summary>
        /// Overloaded. Looks for bindable properties in each of the component objects
        ///  and create a new PD entry for it. It then creates a brand new PDC combining the PDs of
        ///  the originalList and the new entries and returns the new PDC.
		/// </summary>
        /// <param name="originalList">The originalList consists of the PDC of the parent control.</param>
        /// <param name="lookupObjects">The lookupObjects list consists of the array of instances of the component classes.</param>
		/// <returns>A collection of property descriptors.</returns>
        public static PropertyDescriptorCollection AppendBindableProperties(PropertyDescriptorCollection originalList, IList lookupObjects)
		{
			// Prepare a list of bindable properties.
			ArrayList al = new ArrayList();
		
			//foreach(object lookupObject in lookupObjects)
			for(int i = 0; i < lookupObjects.Count; i++)
			{
				DataBindingUtils.RetrieveBindableProperties(lookupObjects[i], al);
			}
			
			PropertyDescriptor[] pds = new PropertyDescriptor[originalList.Count + al.Count];
			originalList.CopyTo(pds, 0);
			al.CopyTo(pds, originalList.Count);

			return new PropertyDescriptorCollection(pds);
		}
		/// <summary>
        /// Looks for bindable properties in each of the component objects
        ///  and create a new PD entry for it. It then creates a brand new PDC combining the PDs of
        ///  the originalList and the new entries and returns the new PDC.
		/// </summary>
        /// <param name="originalList">The originalList consists of the PDC of the parent control.</param>
        /// <param name="lookupObject">The lookupObjects list consists of the array of instances of the component classes.</param>
		/// <returns>A collection of property descriptors.</returns>
        public static PropertyDescriptorCollection AppendBindableProperties(PropertyDescriptorCollection originalList, object lookupObject)
		{
			// Prepare a list of bindable properties.
			ArrayList al = new ArrayList();
			
			DataBindingUtils.RetrieveBindableProperties(lookupObject, al);
			
			PropertyDescriptor[] pds = new PropertyDescriptor[originalList.Count + al.Count];
			originalList.CopyTo(pds, 0);
			al.CopyTo(pds, originalList.Count);

			return new PropertyDescriptorCollection(pds);
		}

		public static void RetrieveBindableProperties(object lookupObject, ArrayList resultantList)
		{
			PropertyDescriptorCollection lookupList = TypeDescriptor.GetProperties(lookupObject, null, false);

			if(!(lookupObject is IDataBindingSupport))
				throw new ArgumentException("Argument lookupObject should implement interface IDataBindingSupport.");

			string newPropPrefix = ((IDataBindingSupport)lookupObject).PropertyNamePrefix;

			foreach(PropertyDescriptor pd in lookupList)
			{
				BindableAttribute ba = pd.Attributes[typeof(System.ComponentModel.BindableAttribute)] as BindableAttribute;
				if(ba != null && ba.Bindable)
				{
					// Set BrowsableAttribute(false) and prevent designer serialization.
					Attribute[] attrs = new Attribute[pd.Attributes.Count + 2];
					int i = 0;
					bool browsableAttributeFound = false;
					bool serializerionAttributeFound = false;
					foreach(Attribute a in pd.Attributes)
					{
						if(a is BrowsableAttribute)
						{
							browsableAttributeFound = true;
							attrs[i++] = new BrowsableAttribute(false);
						}
						else if(a is DesignerSerializationVisibilityAttribute)
						{
							serializerionAttributeFound = true;
							attrs[i++] = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);
						}
						else
							attrs[i++] = a;
					}

					int attrCount = attrs.Length;
					if(!browsableAttributeFound)
						attrs[i++] = new BrowsableAttribute(false);
					else
						attrCount--;
					if(!serializerionAttributeFound)
						attrs[i++] = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);
					else
						attrCount--;
					
					Attribute[] adjustedAttrs = new Attribute[attrCount];
					for(int index = 0 ; index < attrCount; index++)
						adjustedAttrs[index] = attrs[index];

					resultantList.Add(new PropertyDescriptorWrapper(newPropPrefix + "." +  pd.Name, pd, adjustedAttrs, lookupObject));
				}
			}
		}
	}
	internal class PropertyDescriptorWrapper : PropertyDescriptor
	{
		PropertyDescriptor baseProp;
		object baseObj;
		internal PropertyDescriptorWrapper(string propertyName, PropertyDescriptor baseProp, Attribute[] attrArray, object baseObj)
			: base(propertyName, attrArray)
		{
			this.baseProp = baseProp;
			this.baseObj = baseObj;
		}
		public override bool CanResetValue(object comp)  
		{
			return baseProp.CanResetValue(this.baseObj);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return baseProp.ShouldSerializeValue(this.baseObj);
		}

		public override object GetValue(object comp)  
		{
			return baseProp.GetValue(this.baseObj);
		}

		public override void ResetValue(object comp)  
		{
			baseProp.ResetValue(this.baseObj);
		}

		public override void SetValue(object comp, object value)  
		{
			baseProp.SetValue(this.baseObj, value);
		}

		public override Type ComponentType
		{
			get 
			{
				return baseProp.ComponentType;
			}
		}
		public override string DisplayName
		{
			get 
			{
				return baseProp.DisplayName;
			}
		}
		public override bool IsReadOnly
		{
			get 
			{
				return baseProp.IsReadOnly;
			}
		}
		public override Type PropertyType
		{
			get 
			{
				return baseProp.PropertyType;
			}
		}
	}

	[Documentation.DocumentationExclude()]
	public interface IDataBindingSupport
	{
		string PropertyNamePrefix{get;}
	}
}