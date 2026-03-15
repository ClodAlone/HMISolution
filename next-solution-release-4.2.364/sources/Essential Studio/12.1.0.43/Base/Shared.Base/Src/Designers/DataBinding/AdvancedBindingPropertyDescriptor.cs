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
using System.Design;
using System.Drawing.Design;
using System.Windows.Forms;


namespace Syncfusion.Windows.Forms.Design
{
	class AdvancedBindingPropertyDescriptor : 
		PropertyDescriptor
	{
		// Fields
		internal static AdvancedBindingEditor advancedBindingEditor;
        
		// Constructors
		internal AdvancedBindingPropertyDescriptor()
			: base("AdvancedBindingPropertyDescName", null)
		{
		}
        
		static AdvancedBindingPropertyDescriptor()
		{
			AdvancedBindingPropertyDescriptor.advancedBindingEditor = new AdvancedBindingEditor();
		}        
        
		// Methods
        
        
		/// <summary>
		///   <para>Indicates whether the value of this property should be persisted.</para>
		/// </summary>
		/// <param name="component">The component that owns the property to determine whether the value should be persisted.</param>
		/// <returns>
		///   <para>
		///     <see langword="true" /> if the property should be persisted; 
		/// <see langword="false" /> otherwise.</para>
		/// </returns>
		public override /*PropertyDescriptor*/ bool ShouldSerializeValue(object component)
		{
			return false;
		}
        
		/// <summary>
		///   <para>Sets the specified value of the property on the specified component.
		///  </para>
		/// </summary>
		/// <param name="component">The component that owns the property whose value has to be set.</param>
		/// <param name=" value">The value to set the property to.</param>
		public override /*PropertyDescriptor*/ void SetValue(object component, object value)
		{
		}
        
        
        
		/// <summary>
		///   <para>Resets the value of the property on the specified component.</para>
		/// </summary>
		/// <param name="component">The component with this property that should be reset.</param>
		public override /*PropertyDescriptor*/ void ResetValue(object component)
		{
		}
        
		/// <summary>
		///   <para>Returns the current value of the property on the specified
		///  component.</para>
		/// </summary>
		/// <param name="component">The component from which to get the value of the property this descriptor describes.</param>
		/// <returns>
		///   <para>The value of the property on the specified component.</para>
		/// </returns>
		public override /*PropertyDescriptor*/ object GetValue(object component)
		{
			return new AdvancedBindingObject(((ControlBindingsCollection)(component)));
		}
        
		/// <summary>
		///   <para>Returns an editor of the specified type.</para>
		/// </summary>
		/// <param name="type">The type of editor to retrieve.</param>
		/// <returns>
		///   <para>An instance of the requested editor type, or <see langword="null " />if an
		/// editor could not be found. </para>
		/// </returns>
		public override /*PropertyDescriptor*/ object GetEditor(Type type)
		{
			if (type == typeof(UITypeEditor))
				return AdvancedBindingPropertyDescriptor.advancedBindingEditor;
			return base.GetEditor(type);
		}
        
		/// <summary>
		///   <para>Indicates whether resetting the component will change the value of the
		///  component.</para>
		/// </summary>
		/// <param name="component">The component to determine if resetting will change the value of the property on.</param>
		/// <returns>
		///   <para>
		///     <see langword="true" /> if the component can be reset without changing the
		/// value of the property; <see langword="false" /> otherwise.</para>
		/// </returns>
		public override /*PropertyDescriptor*/ bool CanResetValue(object component)
		{
			return false;
		}
        
		/// <summary>
		///   <para>Returns the type of the property.</para>
		/// </summary>
		public override /*PropertyDescriptor*/ Type PropertyType 
		{ 
			get
			{
				return typeof(object);
			}
		}
        
        
		/// <summary>
		///   <para>Indicates whether this property is read-only.</para>
		/// </summary>
		public override /*PropertyDescriptor*/ bool IsReadOnly 
		{ 
			get
			{
				return false;
			}
		}
        
        
		/// <summary>
		///   <para>Returns the type of component this property is bound to.</para>
		/// </summary>
		public override /*PropertyDescriptor*/ Type ComponentType 
		{ 
			get
			{
				return typeof(System.Windows.Forms.ControlBindingsCollection);
			}
		}
        
        
		/// <summary>
		///   <para>In a derived class, adds the attributes of the inherited class to the
		///  specified list of attributes in the parent class.</para>
		/// </summary>
		/// <param name="attributeList">An <see cref="T:System.Collections.IList" /> that lists the attributes in the parent class. Initially, this will be empty.</param>
		protected override /*MemberDescriptor*/ void FillAttributes(IList attributeList)
		{
			attributeList.Add(RefreshPropertiesAttribute.All);
			base.FillAttributes(attributeList);
		}
        
        
		public override /*MemberDescriptor*/ AttributeCollection Attributes 
		{ 
			get
			{
				// Syncfusion Change:
//				Attribute[] attributes0;
//				attributes0 = new Attribute[1];
//				attributes0[0] = new SRDescriptionAttribute("AdvancedBindingPropertyDescriptorDesc");
//				return new AttributeCollection(attributes0);
				return base.Attributes;
			}
		}
	}
}