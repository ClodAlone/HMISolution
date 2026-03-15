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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Design
{
	[
	DesignTimeVisible(false), 
	TypeConverter(@"System.Windows.Forms.Design.AdvancedBindingConverter, System.Design"), 
	ToolboxItem(false)
	]
	class AdvancedBindingObject : ICustomTypeDescriptor, 
		IComponent, 
		ISite
	{
        
		// Fields
		private ControlBindingsCollection bindings;
		private PropertyDescriptorCollection propsCollection;
		private PropertyDescriptor defaultProp;
		private bool showAll;
		private bool changed;
        
		/// <summary>
		///   <para>Adds an event handler to listen to the disposed event on the component.</para>
		/// </summary>
		private EventHandler disposed;
        
		// Constructors
        
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="T:System.Windows.Forms.Design.AdvancedBindingObject" /> class.</para>
		/// </summary>
		/// <param name="bindings">The collection of bindings to store.</param>
		public AdvancedBindingObject(ControlBindingsCollection bindings)
		{
			this.defaultProp = null;
			this.changed = false;
			this.bindings = bindings;
		}
        
        
		// Events
        
		/// <summary>
		///   <para>Adds an event handler to listen to the disposed event on the component.</para>
		/// </summary>
		public event EventHandler Disposed 
		{
			add
			{
				this.disposed = ((EventHandler)(Delegate.Combine(this.disposed, value)));
			}
			remove
			{
				this.disposed = ((EventHandler)(Delegate.Remove(this.disposed, value)));
			}
		}
        
		// Methods
        
        
		/// <summary>
		///   <para>Gets / sets the name for this object.</para>
		/// </summary>
		public virtual /*ISite*/ string Name 
		{ 
			set
			{}
			get
			{
				if (this.bindings.Control.Site == null)
					return "";
				return this.bindings.Control.Site.Name;
			}
		}
        
		public virtual /*IComponent*/ ISite Site 
		{ 
			set
			{
			}
			get
			{
				return this;
			}
		}
        
		object System.IServiceProvider.GetService(Type service)
		{
			if (this.bindings.Control.Site != null)
				return this.bindings.Control.Site.GetService(service);
			return null;
		}
        
        
		bool System.ComponentModel.ISite.DesignMode
		{
			get
			{
				if (this.bindings.Control.Site == null)
					return false;
				return this.bindings.Control.Site.DesignMode;
			}
		}
        
        
		IContainer System.ComponentModel.ISite.Container
		{
			get
			{
				if (this.bindings.Control.Site == null)
					return null;
				return this.bindings.Control.Site.Container;
			}
		}
        
        
		IComponent System.ComponentModel.ISite.Component
		{
			get{return this;}
		}
        
        
		void System.IDisposable.Dispose()
		{
			if (this.disposed != null)
				this.disposed(this, EventArgs.Empty);
		}
        
        
		object System.ComponentModel.ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
        
        
		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties(System.Attribute[] attributes) 
		{
			Control ctl;
			Type componentType;
			PropertyDescriptorCollection pdc;
			AttributeCollection attColl;
			DefaultPropertyAttribute defaultPA;
			ArrayList arrayList;
			int index;
			bool bindable;
			DesignOnlyAttribute designOnlyAttribute;
			DesignBindingPropertyDescriptor value;
			System.ComponentModel.PropertyDescriptor[] array;

			if (this.propsCollection == null) 
			{
				ctl = this.bindings.Control;
				componentType = ctl.GetType();
				pdc = TypeDescriptor.GetProperties(ctl, attributes);
				attColl = TypeDescriptor.GetAttributes(componentType);
				defaultPA = (DefaultPropertyAttribute) attColl[typeof(DefaultPropertyAttribute)];
				arrayList = new ArrayList();
				index = 0;
				while (index < pdc.Count) 
				{
					if (!(pdc[index].IsReadOnly)) 
					{
						bindable = ((BindableAttribute) pdc[index].Attributes[typeof(BindableAttribute)]).Bindable;
						designOnlyAttribute = (DesignOnlyAttribute) pdc[index].Attributes[typeof(DesignOnlyAttribute)];

						// Syncfusion change:
						Attribute[] attrs = new Attribute[2];
						attrs[0] = new BrowsableAttribute(false);
						attrs[1] = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);

						PropertyDescriptor pd = pdc[index];
						value = new DesignBindingPropertyDescriptor(pd.Name, pd.DisplayName, pd, attrs);
						value.AddValueChanged(this.bindings, new EventHandler(this.OnBindingChanged));
						if (bindable || this.showAll && !(designOnlyAttribute.IsDesignOnly) || !(((DesignBinding) value.GetValue(this)).IsNull)) 
						{
							arrayList.Add(value);
							if (defaultPA != null && value.Name == defaultPA.Name)
								this.defaultProp = value;
						}
					}
					index++;
				}
				array = new PropertyDescriptor[checked((uint) arrayList.Count)];
				arrayList.CopyTo(array, 0);
				this.propsCollection = new PropertyDescriptorCollection(array);
			}
			return this.propsCollection;
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}
        
        
		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return null;
		}
        
        
		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return null;
		}
        
        
		object System.ComponentModel.ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}
        
        
		PropertyDescriptor System.ComponentModel.ICustomTypeDescriptor.GetDefaultProperty()
		{
			if (this.defaultProp != null)
				return this.defaultProp;
			if (this.propsCollection != null && this.propsCollection.Count > 0)
				return this.propsCollection[0];
			return null;
		}
        
		EventDescriptor System.ComponentModel.ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}
        
        
		TypeConverter System.ComponentModel.ICustomTypeDescriptor.GetConverter()
		{
			return null;
		}
        
        
		string System.ComponentModel.ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}
        
        
		string System.ComponentModel.ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}
        
        
		AttributeCollection System.ComponentModel.ICustomTypeDescriptor.GetAttributes()
		{
			return new AttributeCollection(null);
		}
        
        
		public override /*Object*/ string ToString()
		{
			return "";
		}
        
        
        
		/// <summary>
		///   <para>Gets / sets the collection of bindings.</para>
		/// </summary>
		public ControlBindingsCollection Bindings 
		{ 
			get
			{
				return this.bindings;
			}
		}
        
		internal bool Changed 
		{ 
			get
			{
				return this.changed;
			}
			set
			{
				this.changed = value;
			}
		}
        
        
		/// <summary>
		///   <para>Indicates whether to show all bindings.</para>
		/// </summary>
		public bool ShowAll 
		{ 
			get
			{
				return this.showAll;
			}
			set
			{
				this.showAll = value;
				this.propsCollection = null;
				this.defaultProp = null;
			}
		}
        
		private void OnBindingChanged(object sender, EventArgs e)
		{
			this.changed = true;
		}
	}
}

