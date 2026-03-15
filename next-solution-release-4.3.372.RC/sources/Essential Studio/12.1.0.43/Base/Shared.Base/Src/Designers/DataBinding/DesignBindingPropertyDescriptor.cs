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
using System.Windows.Forms;
namespace Syncfusion.Windows.Forms.Design
{
	class DesignBindingPropertyDescriptor : 
		PropertyDescriptor
	{
        
		// Fields
		private PropertyDescriptor property;
		private static TypeConverter designBindingConverter;
		private string displayName;
        
		// Constructors
		internal DesignBindingPropertyDescriptor(string name, string displayName, PropertyDescriptor property, Attribute[] attrs)
			: base(name, attrs)
		{
			this.displayName = displayName;
			this.property = property;
		}
        
		static DesignBindingPropertyDescriptor()
		{
			DesignBindingPropertyDescriptor.designBindingConverter = new DesignBindingConverter();
		}
        
        
		// Methods
        
        
		/// <summary>
		///   <para>Indicates whether the specified component should persist the value.</para>
		/// </summary>
		/// <param name="component">The component to determine whether the value of should be persisted.</param>
		/// <returns>
		///   <para>
		///     <see langword="true" /> if the value should be persisted;
		/// <see langword="false" /> otherwise.</para>
		/// </returns>
		public override /*PropertyDescriptor*/ bool ShouldSerializeValue(object component)
		{
			return false;
		}
        
		/// <summary>
		///   <para>Sets the specified value for the specified component.</para>
		/// </summary>
		/// <param name="component">The component to set the value for.</param>
		/// <param name=" value">The value to set.</param>
		public override /*PropertyDescriptor*/ void SetValue(object component, object value)
		{
			//TODO:
//			if (!(component is System.Windows.Forms.Design.AdvancedBindingObject)) goto IL_0015;
//			component = ((AdvancedBindingObject)(component)).Bindings;
//			IL_0015: ;
			// Syncfusion change: Changing the 2nd argument from this.property to this!
			DesignBindingPropertyDescriptor.SetBinding(((ControlBindingsCollection)(component)), this, ((DesignBinding)(value)));
			this.OnValueChanged(component, EventArgs.Empty);
		}
                
		/// <summary>
		///   <para>Resets the value of the specified component.</para>
		/// </summary>
		/// <param name="component">The component whose value is to be reset.</param>
		public override /*PropertyDescriptor*/ void ResetValue(object component)
		{
			//TODO:
//			if (!(component is System.Windows.Forms.Design.AdvancedBindingObject)) goto IL_0015;
//			component = ((AdvancedBindingObject)(component)).Bindings;
//			IL_0015: ;
			// Syncfusion change: Changing the 2nd argument from this.property to this!
			DesignBindingPropertyDescriptor.SetBinding(((ControlBindingsCollection)(component)), this, DesignBinding.Null);
		}
        
		/// <summary>
		///   <para>Returns a value from the specified component.</para>
		/// </summary>
		/// <param name="component">The component to retrieve a value from.</param>
		/// <returns>
		///   <para>The value of the specified component.</para>
		/// </returns>
		public override /*PropertyDescriptor*/ object GetValue(object component)
		{
			//TODO:
//			if (!(component is System.Windows.Forms.Design.AdvancedBindingObject)) goto IL_0015;
//			component = ((AdvancedBindingObject)(component)).Bindings;
//			IL_0015: ;
			// Syncfusion Change: Replaced this.property to this in arg 2.
			return DesignBindingPropertyDescriptor.GetBinding(((ControlBindingsCollection)(component)), this);
		}        
        
		/// <summary>
		///   <para>Indicates whether the specified component can reset the value
		///  of the property.</para>
		/// </summary>
		/// <param name="component">The component to test whether it can change the value of the property.</param>
		/// <returns>
		///   <para>
		///     <see langword="true" /> if the value can be reset; 
		/// <see langword="false" /> otherwise.</para>
		/// </returns>
		public override /*PropertyDescriptor*/ bool CanResetValue(object component)
		{
			//TODO:
//			if (!(component is System.Windows.Forms.Design.AdvancedBindingObject)) goto IL_0015;
//			component = ((AdvancedBindingObject)(component)).Bindings;
//			IL_0015: ;
			return !(DesignBindingPropertyDescriptor.GetBinding(((ControlBindingsCollection)(component)), this).IsNull);
		}
        
		/// <summary>
		///   <para>Returns the type of the property.</para>
		/// </summary>
		public override /*PropertyDescriptor*/ Type PropertyType 
		{ 
			get
			{
				return typeof(Syncfusion.Windows.Forms.Design.DesignBinding);
			}
		}
        
		public override string DisplayName 
		{
			get{return this.displayName;}
		}
        
		/// <summary>
		///   <para>Indicates whether the property is read-only.</para>
		/// </summary>
		public override /*PropertyDescriptor*/ bool IsReadOnly 
		{ 
			get
			{
				return false;
			}
		}
        
        
		/// <summary>
		///   <para>Returns the type converter.</para>
		/// </summary>
		public override /*PropertyDescriptor*/ TypeConverter Converter 
		{ 
			get
			{
				return DesignBindingPropertyDescriptor.designBindingConverter;
			}
		}
        
		/// <summary>
		///   <para>Returns the type of the component that owns the property.</para>
		/// </summary>
		public override /*PropertyDescriptor*/ Type ComponentType 
		{ 
			get
			{
				return typeof(System.Windows.Forms.ControlBindingsCollection);
			}
		}
        
		private static void SetBinding(ControlBindingsCollection bindings, PropertyDescriptor property, DesignBinding designBinding) 
		{
			Binding binding;

			if (designBinding == null)
				return;
			binding = bindings[property.Name];
			if (binding != null)
				bindings.Remove(binding);
			if (!(designBinding.IsNull))
				bindings.Add(property.Name, designBinding.DataSource, designBinding.DataMember);
		}
        
		private static DesignBinding GetBinding(ControlBindingsCollection bindings, PropertyDescriptor property)
		{
			Binding binding;
			BindingMemberInfo bindingMemberInfo;

			binding = bindings[property.Name];
			if (binding == null)
				return DesignBinding.Null;
			bindingMemberInfo = binding.BindingMemberInfo;
			return new DesignBinding(binding.DataSource, bindingMemberInfo.BindingMember);
		}
        
	}
}