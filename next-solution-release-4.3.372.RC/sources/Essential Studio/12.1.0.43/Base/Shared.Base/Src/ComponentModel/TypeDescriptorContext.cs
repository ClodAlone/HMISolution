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
using System.ComponentModel.Design;

namespace Syncfusion.ComponentModel
{
	/// <summary>
	/// Provides a basic implementation for the ITypeDescriptorContext interface and can be used
	/// together with WindowsFormsEditorService to launch an Editor directly outside a property grid.
	/// </summary>
	/// <example>
	/// The grid uses this class to display a collection editor:
	/// <code lang="C#">
	/// public static DialogResult ShowGridBaseStylesMapDialog(object instance, string propertyName)
	/// {
	///     GridBaseStyleCollectionEditor ce = new GridBaseStyleCollectionEditor(typeof(ArrayList));
	/// WindowsFormsEditorServiceContainer esc = new WindowsFormsEditorServiceContainer(null);
	///     PropertyDescriptor pd = TypeDescriptor.GetProperties(instance)[propertyName];
	///     TypeDescriptorContext tdc = new TypeDescriptorContext(instance, pd);
	///     tdc.ServiceProvider = esc;
	///     object v = ce.EditValue(tdc, esc, ((ICloneable) pd.GetValue(instance)).Clone());
	///     if (esc.DialogResult == DialogResult.OK)
	///     {
	///         pd.SetValue(instance, v);
	///     }
	///     return esc.DialogResult;
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="WindowsFormsEditorServiceContainer"/>
	public class TypeDescriptorContext : ITypeDescriptorContext
	{
		object instance;
		PropertyDescriptor propertyDescriptor;
		private IServiceProvider serviceProvider;

		/// <summary>
		/// Initializes a TypeDescriptorContext for the given object and PropertyDescriptor.
		/// </summary>
		/// <param name="instance">The instance of the property to be edited.</param>
		/// <param name="propertyDescriptor">A PropertyDescriptor that contains information about the property.</param>
		public TypeDescriptorContext(object instance, PropertyDescriptor propertyDescriptor)
		{
			this.instance = instance;
			this.propertyDescriptor = propertyDescriptor;
		}

		void ITypeDescriptorContext.OnComponentChanged()
		{
		} 
        
        
		bool ITypeDescriptorContext.OnComponentChanging()
		{
			return true;
		} 
        
        
		object ITypeDescriptorContext.Instance 
		{ 
			get
			{
				return instance;
			} 
		}
        
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor 
		{ 
			get
			{
				return propertyDescriptor;
			} 
		}

		IContainer ITypeDescriptorContext.Container 
		{ 
			get
			{
				return null;
			} 
		}

		object IServiceProvider.GetService(Type classService)
		{
			if (this.ServiceProvider != null) 
				return this.serviceProvider.GetService(classService);

			return null;
		} 

		/// <summary>
		/// Gets / sets the associated IServiceProvider.
		/// </summary>
		/// <value>An IServiceProvider value.</value>
		public IServiceProvider ServiceProvider 
		{ 
			get
			{
				return this.serviceProvider;
			} 
			set
			{
				if (value != this.serviceProvider)
				{
					this.serviceProvider = value;
					//this.helpService = null;
				}
			} 
		}
	}
}
