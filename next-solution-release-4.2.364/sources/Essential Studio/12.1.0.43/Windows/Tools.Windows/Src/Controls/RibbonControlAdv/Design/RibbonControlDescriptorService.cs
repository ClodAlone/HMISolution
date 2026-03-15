#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	class RibbonControlDescriptorService : ITypeDescriptorFilterService
	{
		#region Constructors
		protected RibbonControlDescriptorService(ITypeDescriptorFilterService baseSvc)
		{
			m_baseSvc = baseSvc;
		}
		#endregion

		#region Methods
		public static void Initialize(ISite site)
		{
			if (site != null)
			{
				IServiceContainer services = site.GetService(typeof(IServiceContainer)) as IServiceContainer;
				if (services != null)
				{
					Type typeNew = typeof(RibbonControlDescriptorService);
					if (services.GetService(typeNew) == null)
					{
						Type typeOld = typeof(ITypeDescriptorFilterService);

						ITypeDescriptorFilterService currentService = services.GetService(typeOld) as ITypeDescriptorFilterService;
						if (currentService != null)
						{
							RibbonControlDescriptorService svc = new RibbonControlDescriptorService(currentService);

							services.RemoveService(typeOld);
							services.AddService(typeOld, svc);
							services.AddService(typeNew, svc);
						}
					}
				}
			}
		}
		#endregion

		#region ITypeDescriptorFilterService Members

		bool ITypeDescriptorFilterService.FilterAttributes(IComponent component, IDictionary attributes)
		{
			return m_baseSvc.FilterAttributes(component, attributes);
		}

		bool ITypeDescriptorFilterService.FilterEvents(IComponent component, IDictionary events)
		{
			return m_baseSvc.FilterEvents(component, events);
		}

		bool ITypeDescriptorFilterService.FilterProperties(IComponent component, IDictionary properties)
		{
			bool bResult = m_baseSvc.FilterProperties(component, properties);

			if (component is IQuickItem)
			{
				UpdateQuickItemProperties(component, properties);
			}

			if (component is ToolStripItem)
			{
				UpdateToolStripItemProperties(component, properties);
			}

			if (component is ToolStripEx)
			{
				UpdateToolStripExProperties(component, properties);
			}

			return bResult;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="properties"></param>
		void UpdateQuickItemProperties(IComponent component, IDictionary properties)
		{
			foreach (string sName in sQuickItemHiddenProperties)
			{
				PropertyDescriptor pd = properties[sName] as PropertyDescriptor;
				if (pd != null)
				{
					Attribute[] attributes = new Attribute[]
					{
						new BrowsableAttribute(false),
						new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)
					};
					properties[sName] = new CustomDescriptor(pd, attributes);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="properties"></param>
		private void UpdateToolStripItemProperties(IComponent component, IDictionary properties)
		{
			PropertyDescriptor pd = properties[sTextProperty] as PropertyDescriptor;
			if (pd != null)
			{
				Attribute[] attributes = new Attribute[]
				{
					new EditorAttribute(typeof(MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))
				};
				properties[sTextProperty] = new CustomDescriptor(pd, attributes);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="properties"></param>
		private void UpdateToolStripExProperties(IComponent component, IDictionary properties)
		{
			ToolStripEx ts = component as ToolStripEx;
			if (ts != null && ts.Parent is RibbonPanel)
			{
				foreach (string sName in sToolStripExHiddenProperties)
				{
					PropertyDescriptor pd = properties[sName] as PropertyDescriptor;
					if (pd != null)
					{
						Attribute[] attributes = new Attribute[]
						{
							new BrowsableAttribute(false)
						};
						properties[sName] = new CustomDescriptor(pd, attributes);
					}
				}
			}
		}
		#endregion

		#region Fields
		ITypeDescriptorFilterService m_baseSvc;

		static string[] sQuickItemHiddenProperties =
		{
			"Text", 
			"Image", 
			"Enabled",
			"DisplayStyle",
			"DropDownItems",
			"RightToLeft",
		};

		static string[] sToolStripExHiddenProperties =
		{
			"Dock", 
		};

		static string sTextProperty = "Text";
		#endregion

		#region *** CustomDescriptor
		class CustomDescriptor : PropertyDescriptor
		{
			#region Constructors
			public CustomDescriptor(PropertyDescriptor baseDescriptor)
				: this(baseDescriptor, new Attribute[] { })
			{
			}
			public CustomDescriptor(PropertyDescriptor baseDescriptor, Attribute[] attributes)
				: base(baseDescriptor, attributes)
			{
				m_baseDescriptor = baseDescriptor;
			}
			#endregion

			#region Properties
			public override Type ComponentType
			{
				get { return m_baseDescriptor.ComponentType; }
			}
			public override Type PropertyType
			{
				get { return m_baseDescriptor.PropertyType; }
			}
			public override bool IsReadOnly
			{
				get { return m_baseDescriptor.IsReadOnly; }
			}
			public override bool IsBrowsable
			{
				get { return m_baseDescriptor.IsBrowsable; }
			}
			#endregion

			#region Overrides
			public override object GetValue(object component)
			{
				return m_baseDescriptor.GetValue(component);
			}
			public override void SetValue(object component, object value)
			{
				m_baseDescriptor.SetValue(component, value);
			}
			public override bool CanResetValue(object component)
			{
				return m_baseDescriptor.CanResetValue(component);
			}
			public override void ResetValue(object component)
			{
				m_baseDescriptor.ResetValue(component);
			}
			public override bool ShouldSerializeValue(object component)
			{
				return m_baseDescriptor.ShouldSerializeValue(component);
			}
			#endregion

			#region Fields
			PropertyDescriptor m_baseDescriptor;
			#endregion
		}
		#endregion
	}
}
#endif
