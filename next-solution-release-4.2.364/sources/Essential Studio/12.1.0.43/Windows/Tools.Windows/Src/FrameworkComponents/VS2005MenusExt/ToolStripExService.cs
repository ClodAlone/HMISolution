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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design.Behavior;
using System.Drawing;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region Delegates
	delegate object GetObject();
	delegate void SetObject(object value);
	delegate bool GetBoolean();
	#endregion

	#region SelectionManagerProxy
	class SelectionManagerProxy
	{
		#region Constructors/Destructors
		/// <summary>
		/// 
		/// </summary>
		static SelectionManagerProxy()
		{
			m_tBaseSvc = Type.GetType("System.Windows.Forms.Design.Behavior.SelectionManager, System.Design");
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="svcProvider"></param>
		public SelectionManagerProxy(IServiceProvider svcProvider)
		{
			if (svcProvider != null)
			{
                try
                {
                    if (m_tBaseSvc == null)
                        m_tBaseSvc = Type.GetType("System.Windows.Forms.Design.Behavior.SelectionManager, System.Design");

                    m_oBaseSvc = svcProvider.GetService(m_tBaseSvc);
                }
                catch { }
				if (m_oBaseSvc != null)
				{
					m_getBodyAdorner = Delegate.CreateDelegate(typeof(GetAdorner), m_oBaseSvc, "get_BodyGlyphAdorner") as GetAdorner;
					m_getSelectionAdorner = Delegate.CreateDelegate(typeof(GetAdorner), m_oBaseSvc, "get_SelectionGlyphAdorner") as GetAdorner;
				}
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public Adorner BodyGlyphAdorner
		{
			get
			{
				if (m_getBodyAdorner != null)
				{
					return m_getBodyAdorner();
				}
				return null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Adorner SelectionGlyphAdorner
		{
			get
			{
				if (m_getSelectionAdorner != null)
				{
					return m_getSelectionAdorner();
				}
				return null;
			}
		}
		#endregion

		#region Fields
		static Type m_tBaseSvc = null;
		private object m_oBaseSvc = null;

		private GetAdorner m_getBodyAdorner = null;
		private GetAdorner m_getSelectionAdorner = null;
		#endregion
	}
	#endregion

	#region ToolStripAdornerWindowServiceProxy
	class ToolStripAdornerWindowServiceProxy
	{
		#region Constructors/Destructors
		static ToolStripAdornerWindowServiceProxy()
		{
			m_tBaseSvc = Type.GetType("System.Windows.Forms.Design.ToolStripAdornerWindowService, System.Design");
		}
		public ToolStripAdornerWindowServiceProxy(IServiceProvider svcProvider)
		{
			if (svcProvider != null && m_tBaseSvc != null)
			{
				m_oBaseSvc = svcProvider.GetService(m_tBaseSvc);
				if (m_oBaseSvc != null)
				{
					m_getDropDownAdorner = Delegate.CreateDelegate(typeof(GetAdorner), m_oBaseSvc, "get_DropDownAdorner") as GetAdorner;
					m_getToolStripAdornerWindowGraphics = Delegate.CreateDelegate(typeof(GetGraphics), m_oBaseSvc, "get_ToolStripAdornerWindowGraphics") as GetGraphics;
					m_getToolStripAdornerWindowControl = Delegate.CreateDelegate(typeof(GetControl), m_oBaseSvc, "get_ToolStripAdornerWindowControl") as GetControl;
				}
			}
		}
		#endregion

		#region Methods
		#endregion

		#region Properties
		public Adorner DropDownAdorner
		{
			get
			{
				if (m_getDropDownAdorner != null)
				{
					return m_getDropDownAdorner();
				}
				return null;
			}
		}
		public Graphics ToolStripAdornerWindowGraphics
		{
			get
			{
				if (m_getToolStripAdornerWindowGraphics != null)
				{
					return m_getToolStripAdornerWindowGraphics();
				}
				return null;
			}
		}
		public Control ToolStripAdornerWindowControl
		{
			get
			{
				if (m_getToolStripAdornerWindowControl != null)
				{
					return m_getToolStripAdornerWindowControl();
				}
				return null;
			}
		}
		#endregion

		#region Fields
		static Type m_tBaseSvc = null;
		private object m_oBaseSvc = null;

		private GetAdorner m_getDropDownAdorner = null;
		private GetGraphics m_getToolStripAdornerWindowGraphics = null;
		private GetControl m_getToolStripAdornerWindowControl = null;
		#endregion
	}
	#endregion

	#region ToolStripKeyboardHandlingServiceProxy
	class ToolStripKeyboardHandlingServiceProxy
	{
		#region Constructors
		public ToolStripKeyboardHandlingServiceProxy(IServiceProvider provider)
		{
			m_baseType = Type.GetType("System.Windows.Forms.Design.ToolStripKeyboardHandlingService, System.Design");
			m_provider = provider;
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public Object SelectedDesignerControl
		{
			get
			{
				object baseSvc = this.BaseSvc;
				if (baseSvc != null)
				{
					PropertyInfo pi = this.SelectedDesignerControlInfo;
					if (pi != null)
					{
						return pi.GetValue(baseSvc, new object[] { });
					}
				}
				return null;
			}
			set
			{
				object baseSvc = this.BaseSvc;
				if (baseSvc != null)
				{
					PropertyInfo pi = this.SelectedDesignerControlInfo;
					if (pi != null)
					{
						pi.SetValue(baseSvc, value, new object[] { });
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool CopyInProgress
		{
			get
			{
				object baseSvc = this.BaseSvc;
				if (baseSvc != null)
				{
					PropertyInfo pi = this.CopyInProgressInfo;
					if (pi != null)
					{
						return (bool)pi.GetValue(baseSvc, new object[] { });
					}
				}
				return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private object BaseSvc
		{
			get
			{
				object baseSvc = null;
				
				if (m_baseType != null && m_provider != null)
				{
					baseSvc = m_provider.GetService(m_baseType);
					if (baseSvc == null)
					{
						ConstructorInfo ci = m_baseType.GetConstructor(new Type[] { typeof(IServiceProvider) });
						if (ci != null)
						{
							baseSvc = ci.Invoke(new object[] { m_provider });
						}
					}
				}
				
				return baseSvc;
			}
		}

		private PropertyInfo SelectedDesignerControlInfo
		{
			get { return m_baseType.GetProperty("SelectedDesignerControl", BindingFlags.Instance | BindingFlags.NonPublic); }
		}
		private PropertyInfo CopyInProgressInfo
		{
			get { return m_baseType.GetProperty("CopyInProgress", BindingFlags.Instance | BindingFlags.NonPublic); }
		}
		#endregion

		#region Fields
		Type m_baseType = null;
		IServiceProvider m_provider = null;
		#endregion
	}
	#endregion

	#region ToolStripExService
	public class ToolStripExService : IServiceProvider
	{
		#region *** ToolStripPanelItemSite
		class ToolStripPanelItemSite : ISite
		{
#region Constructors
			ToolStripPanelItemSite(ISite parentSite, ToolStripPanelItemHost host)
			{
				m_parentSite = parentSite;
				m_host = host;
			}
			#endregion

#region Methods
			public static void Update(ToolStripItem item, ToolStripPanelItemHost host)
			{
				if (item != null)
				{
					ISite site = item.Site;

					if (site != null)
					{
						item.Site = new ToolStripPanelItemSite(site, host);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			public static void Restore(ToolStripItem item)
			{
				if (item != null)
				{
					ToolStripPanelItemSite site = item.Site as ToolStripPanelItemSite;

					if (site != null)
					{
						item.Site = site.ParentSite;
					}
				}
			}
			#endregion

#region ISite implementation
			/// <summary>
			/// 
			/// </summary>
			IComponent ISite.Component
			{
				get
				{
					return m_parentSite.Component;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			IContainer ISite.Container
			{
				get
				{
					return m_parentSite.Container;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			bool ISite.DesignMode
			{
				get
				{
					return m_parentSite.DesignMode;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			string ISite.Name
			{
				get
				{
					return m_parentSite.Name;
				}
				set
				{
					m_parentSite.Name = value;
				}
			}
			#endregion

#region IServiceProvider implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="serviceType"></param>
			/// <returns></returns>
			object IServiceProvider.GetService(Type serviceType)
			{
				if (serviceType == typeof(IDesignerHost) || serviceType == typeof(ISelectionService))
				{
					return m_host;
				}
				return m_parentSite.GetService(serviceType);
			}
			#endregion

#region Properties
			public ISite ParentSite
			{
				get
				{
					return m_parentSite;
				}
			}
			#endregion

#region Fields
			ISite m_parentSite = null;
			ToolStripPanelItemHost m_host = null;
			#endregion
		}
		#endregion

		#region *** ToolStripPanelItemHost
		public class ToolStripPanelItemHost : IDesignerHost, ISelectionService
		{
#region Constructors
			public ToolStripPanelItemHost(ToolStripPanelItem item, IDesignerHost parentHost)
			{
				m_item = item;

				m_parentHost = parentHost;
				m_parentSelectionSvc = parentHost.GetService(typeof(ISelectionService)) as ISelectionService;
			}
			#endregion

#region Methods
			/// <summary>
			/// 
			/// </summary>
			public virtual void OnItemAdded()
			{
				ToolStrip ts = m_item.Control as ToolStrip;
				if (ts != null)
				{
					ts.ItemAdded += new ToolStripItemEventHandler(OnToolStripItemAdded);
					ts.ItemRemoved += new ToolStripItemEventHandler(OnToolStripItemRemoved);
					ts.ParentChanged += new EventHandler(OnParentChanged);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public virtual void OnItemRemoved()
			{
				ToolStrip ts = m_item.Control as ToolStrip;
				if (ts != null)
				{
					ts.ItemAdded -= new ToolStripItemEventHandler(OnToolStripItemAdded);
					ts.ItemRemoved -= new ToolStripItemEventHandler(OnToolStripItemRemoved);
					ts.ParentChanged -= new EventHandler(OnParentChanged);
				}
			}
			#endregion

#region IDesignerHost implementation

#region Properties
			/// <summary>
			/// 
			/// </summary>
			IContainer IDesignerHost.Container
			{
				get { return m_parentHost.Container; }
			}
			/// <summary>
			/// 
			/// </summary>
			bool IDesignerHost.InTransaction
			{
				get { return m_parentHost.InTransaction; }
			}
			/// <summary>
			/// 
			/// </summary>
			bool IDesignerHost.Loading
			{
				get { return m_parentHost.Loading; }
			}
			/// <summary>
			/// 
			/// </summary>
			IComponent IDesignerHost.RootComponent
			{
				get
				{
					return m_item.Control;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			string IDesignerHost.RootComponentClassName
			{
				get
				{
					return m_item.Control.Site.Name;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			string IDesignerHost.TransactionDescription
			{
				get { return m_parentHost.TransactionDescription; }
			}
			#endregion

#region Events
			/// <summary>
			/// 
			/// </summary>
			event EventHandler IDesignerHost.Activated
			{
				add { m_parentHost.Activated += value; }
				remove { m_parentHost.Activated -= value; }
			}
			/// <summary>
			/// 
			/// </summary>
			event EventHandler IDesignerHost.Deactivated
			{
				add { m_parentHost.Deactivated += value; }
				remove { m_parentHost.Deactivated -= value; }
			}
			/// <summary>
			/// 
			/// </summary>
			event EventHandler IDesignerHost.LoadComplete
			{
				add { m_parentHost.LoadComplete += value; }
				remove { m_parentHost.LoadComplete -= value; }
			}
			/// <summary>
			/// 
			/// </summary>
			event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosed
			{
				add { m_parentHost.TransactionClosed += value; }
				remove { m_parentHost.TransactionClosed -= value; }
			}
			/// <summary>
			/// 
			/// </summary>
			event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosing
			{
				add { m_parentHost.TransactionClosing += value; }
				remove { m_parentHost.TransactionClosing -= value; }
			}
			/// <summary>
			/// 
			/// </summary>
			event EventHandler IDesignerHost.TransactionOpened
			{
				add { m_parentHost.TransactionOpened += value; }
				remove { m_parentHost.TransactionOpened -= value; }
			}
			/// <summary>
			/// 
			/// </summary>
			event EventHandler IDesignerHost.TransactionOpening
			{
				add { m_parentHost.TransactionOpening += value; }
				remove { m_parentHost.TransactionOpening -= value; }
			}
			#endregion

#region Methods
			/// <summary>
			/// 
			/// </summary>
			void IDesignerHost.Activate()
			{
				m_parentHost.Activate();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="componentClass"></param>
			/// <returns></returns>
			IComponent IDesignerHost.CreateComponent(Type componentClass)
			{
				return m_parentHost.CreateComponent(componentClass);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="componentClass"></param>
			/// <param name="name"></param>
			/// <returns></returns>
			IComponent IDesignerHost.CreateComponent(Type componentClass, string name)
			{
				return m_parentHost.CreateComponent(componentClass, name);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			DesignerTransaction IDesignerHost.CreateTransaction()
			{
				return m_parentHost.CreateTransaction();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="description"></param>
			/// <returns></returns>
			DesignerTransaction IDesignerHost.CreateTransaction(string description)
			{
				return m_parentHost.CreateTransaction(description);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="component"></param>
			void IDesignerHost.DestroyComponent(IComponent component)
			{
				m_parentHost.DestroyComponent(component);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="component"></param>
			/// <returns></returns>
			IDesigner IDesignerHost.GetDesigner(IComponent component)
			{
				return m_parentHost.GetDesigner(component);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="typeName"></param>
			/// <returns></returns>
			Type IDesignerHost.GetType(string typeName)
			{
				return m_parentHost.GetType(typeName);
			}
			#endregion

			#endregion

#region IServiceContainer implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="serviceType"></param>
			/// <param name="serviceInstance"></param>
			void IServiceContainer.AddService(Type serviceType, object serviceInstance)
			{
				m_parentHost.AddService(serviceType, serviceInstance);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="serviceType"></param>
			/// <param name="callback"></param>
			void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
			{
				m_parentHost.AddService(serviceType, callback);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="serviceType"></param>
			/// <param name="serviceInstance"></param>
			/// <param name="promote"></param>
			void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
			{
				m_parentHost.AddService(serviceType, serviceInstance, promote);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="serviceType"></param>
			/// <param name="callback"></param>
			/// <param name="promote"></param>
			void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
			{
				m_parentHost.AddService(serviceType, callback, promote);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="serviceType"></param>
			void IServiceContainer.RemoveService(Type serviceType)
			{
				m_parentHost.RemoveService(serviceType);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="serviceType"></param>
			/// <param name="promote"></param>
			void IServiceContainer.RemoveService(Type serviceType, bool promote)
			{
				m_parentHost.RemoveService(serviceType, promote);
			}
			#endregion

#region IServiceProvider implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="serviceType"></param>
			/// <returns></returns>
			object IServiceProvider.GetService(Type serviceType)
			{
				return m_parentHost.GetService(serviceType);
			}
			#endregion

#region ISelectionService implementation

#region Properties
			object ISelectionService.PrimarySelection
			{
				get
				{
					return m_parentSelectionSvc.PrimarySelection;
				}
			}
			int ISelectionService.SelectionCount
			{
				get
				{
					return m_parentSelectionSvc.SelectionCount;
				}
			}
			#endregion

#region Events
			event EventHandler ISelectionService.SelectionChanged
			{
				add { m_parentSelectionSvc.SelectionChanged += value; }
				remove { m_parentSelectionSvc.SelectionChanged -= value; }
			}
			event EventHandler ISelectionService.SelectionChanging
			{
				add { m_parentSelectionSvc.SelectionChanging += value; }
				remove { m_parentSelectionSvc.SelectionChanging -= value; }
			}
			#endregion

#region Methods
			/// <summary>
			/// 
			/// </summary>
			/// <param name="component"></param>
			/// <returns></returns>
			bool ISelectionService.GetComponentSelected(object component)
			{
				return m_parentSelectionSvc.GetComponentSelected(component);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			ICollection ISelectionService.GetSelectedComponents()
			{
				return m_parentSelectionSvc.GetSelectedComponents();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="components"></param>
			void ISelectionService.SetSelectedComponents(ICollection components)
			{
				m_parentSelectionSvc.SetSelectedComponents(components);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="components"></param>
			/// <param name="selectionType"></param>
			void ISelectionService.SetSelectedComponents(ICollection components, SelectionTypes selectionType)
			{
				object[] objSelected = null;

				if (components != null)
				{
					ToolStrip owner = m_item.Control as ToolStrip;
					if (owner != null)
					{
						objSelected = new object[components.Count];
						components.CopyTo(objSelected, 0);

						for (int i = 0, len = objSelected.Length; i < len; i++)
						{
							if (objSelected[i] == owner)
							{
								objSelected[i] = m_item;
							}
						}
					}
				}

				m_parentSelectionSvc.SetSelectedComponents(objSelected, selectionType);
			}
			#endregion

			#endregion

#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="newParent"></param>
			void SetParent(ToolStripDropDown newParent)
			{
				if (m_dropDownParent != null)
				{
					m_dropDownParent.Closing -= new ToolStripDropDownClosingEventHandler(OnParentClosing);
				}
				if (newParent != null)
				{
					newParent.Closing += new ToolStripDropDownClosingEventHandler(OnParentClosing);
				}
				m_dropDownParent = newParent;
			}
			#endregion

#region Event handlers
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnParentChanged(object sender, EventArgs e)
			{
				SetParent(m_item.Control.Parent as ToolStripDropDown);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnToolStripItemAdded(object sender, ToolStripItemEventArgs e)
			{
				ToolStripPanelItemSite.Update(e.Item, this);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnToolStripItemRemoved(object sender, ToolStripItemEventArgs e)
			{
				ToolStripPanelItemSite.Restore(e.Item);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnParentClosing(object sender, ToolStripDropDownClosingEventArgs e)
			{
				if (!e.Cancel)
				{
					if(this.IsSelected)
					{
						e.Cancel = true;
					}
				}
			}
			#endregion

#region Properties
			bool IsSelected
			{
				get
				{
					ToolStripExService toolStripSvc = m_parentHost.GetService(typeof(ToolStripExService)) as ToolStripExService;
					if (toolStripSvc != null)
					{
						ToolStripItem item = toolStripSvc.PrimarySelection as ToolStripItem;
						if (item != null)
						{
							ToolStrip ts = item.GetCurrentParent();
							while (ts != null)
							{
								if (ts != m_item.Control)
								{
									if (ts is ToolStripDropDown)
									{
										ToolStripItem owner = ((ToolStripDropDown)ts).OwnerItem;
										ts = owner != null ? owner.GetCurrentParent() : null;
									}
									else ts = ts.Parent as ToolStrip;
								}
								else return true;
							}
						}
					}
					return false;
				}
			}
			#endregion

#region Fields
			ToolStripPanelItem m_item = null;
			ToolStripDropDown m_dropDownParent = null;

			IDesignerHost m_parentHost = null;
			ISelectionService m_parentSelectionSvc = null;
			#endregion
		}
		#endregion

		#region *** ToolStripExBehavior
		class ToolStripExBehavior : Behavior
		{
			#region Constructors
			/// <summary>
			/// 
			/// </summary>
			static ToolStripExBehavior()
			{
				m_tToolStripItemDataObject = Type.GetType("System.Windows.Forms.Design.ToolStripItemDataObject, System.Design");
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="behaviorSvc"></param>
			public ToolStripExBehavior(BehaviorService behaviorSvc) : base(true, behaviorSvc)
			{
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="e"></param>
			public override void OnDragEnter(Glyph g, DragEventArgs e)
			{
				IDataObject data = e.Data;
				
				if (m_tToolStripItemDataObject != null && m_tToolStripItemDataObject.IsInstanceOfType(data))
				{
					ComponentGlyph componentGlyph = g as ComponentGlyph;
					if (componentGlyph != null)
					{
						ToolStripEx ts = componentGlyph.RelatedComponent as ToolStripEx;
						if (ts != null)
						{
							FieldInfo fi = data.GetType().GetField("owner", BindingFlags.Instance | BindingFlags.NonPublic);
							if (fi != null)
							{
								fi.SetValue(data, ts);
							}
						}
					}
				}

				base.OnDragEnter(g, e);
			}
			#endregion

			#region Fields
			/// <summary>
			/// 
			/// </summary>
			static Type m_tToolStripItemDataObject = null;
			#endregion
		}
		#endregion

		#region Constants
		const BindingFlags BF_NONPUBLIC = BindingFlags.Instance | BindingFlags.NonPublic;
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		static ToolStripExService()
		{
			m_tGlyphCollection = Type.GetType("System.Windows.Forms.Design.Behavior.GlyphCollection, System.Design");

			Type tAdorner = Type.GetType("System.Windows.Forms.Design.Behavior.Adorner, System.Design");
			if (tAdorner != null)
			{
				m_fiGlyphs = tAdorner.GetField("glyphs", BindingFlags.Instance | BindingFlags.NonPublic);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		protected ToolStripExService(IServiceContainer services)
		{
			m_parentProvider = services;

			IComponentChangeService changeSvc = m_parentProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if (changeSvc != null)
			{
				changeSvc.ComponentAdding += new ComponentEventHandler(OnComponentAdding);
				changeSvc.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeSvc.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
				changeSvc.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
			}

			m_behaviorSvc = m_parentProvider.GetService(typeof(BehaviorService)) as BehaviorService;
			if (m_behaviorSvc != null)
			{
				IDesignerHost designerHost = m_parentProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (designerHost != null)
				{
					m_toolstripBehavior = new ToolStripExBehavior(m_behaviorSvc);

					if (designerHost.Loading)
					{
						designerHost.LoadComplete += new EventHandler(OnDesignerHostLoadComplete);
					}
					else UpdateBehaviors();
				}
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		public static ToolStripExService Get(ISite site)
		{
			ToolStripExService service = null;

			if (site != null)
			{
				IServiceContainer services = site.GetService(typeof(IServiceContainer)) as IServiceContainer;
				if (services != null)
				{
					service = services.GetService(typeof(ToolStripExService)) as ToolStripExService;
					if (service == null)
					{
						service = new ToolStripExService(services);

						service.UpdateAdornerGlyphs();

						services.AddService(typeof(ToolStripExService), service);
					}
				}
				RibbonControlDescriptorService.Initialize(site);
			}

			return service;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public virtual void AddItem(ToolStripPanelItem item)
		{
			if (item != null)
			{
				IDesignerHost parentHost = m_parentProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (parentHost != null)
				{
					ToolStripPanelItemHost itemHost = new ToolStripPanelItemHost(item, parentHost);

					this.Items[item] = itemHost;
					itemHost.OnItemAdded();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public virtual void RemoveItem(ToolStripPanelItem item)
		{
			if (item != null)
			{
				ToolStripPanelItemHost itemHost = this.Items[item] as ToolStripPanelItemHost;
				if (itemHost != null)
				{
					this.Items.Remove(item);
					itemHost.OnItemRemoved();
				}
			}
		}
		#endregion

		#region IServiceProvider implementation
		public virtual object GetService(Type serviceType)
		{
			return m_parentProvider.GetService(serviceType);
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		void UpdateAdornerGlyphs()
		{
			SelectionManagerProxy selectionManager = this.SelectionManager;
            if (selectionManager != null)
            {
                UpdateAdornerGlyphs(selectionManager.BodyGlyphAdorner);
                UpdateAdornerGlyphs(selectionManager.SelectionGlyphAdorner);
            }
			UpdateAdornerGlyphs(this.AdornerWindowService.DropDownAdorner);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="adorner"></param>
		void UpdateAdornerGlyphs(Adorner adorner)
		{
			if (m_fiGlyphs != null && adorner != null)
			{
				GlyphCollection glyphs = adorner.Glyphs;
				if (m_tGlyphCollection == glyphs.GetType())
				{
					m_fiGlyphs.SetValue(adorner, new ToolStripExGlyphCollection());
					adorner.Glyphs.AddRange(glyphs);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void UpdateBehaviors()
		{
			if (m_behaviorSvc != null)
			{
				Behavior current = m_behaviorSvc.CurrentBehavior;

				if (current != m_toolstripBehavior)
				{
					if (current != null)
					{
						m_behaviorSvc.PopBehavior(m_toolstripBehavior);
					}
					m_behaviorSvc.PushBehavior(m_toolstripBehavior);
				}
			}
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnComponentAdding(object sender, ComponentEventArgs e)
		{
			if (!this.ToolStripKeyboardHandlingService.CopyInProgress)
			{
				ToolStripItem item = e.Component as ToolStripItem;
				if (item != null)
				{
					ToolStripItem selected = this.PrimarySelection as ToolStripItem;
					if (selected != null)
					{
						ToolStripDropDownItem ddItem = selected as ToolStripDropDownItem;
						if (ddItem == null)
						{
							ToolStripPanelItem panelItem = selected as ToolStripPanelItem;
							if (panelItem == null)
							{
								ToolStrip parent = selected.GetCurrentParent();
								if (parent is ToolStripDropDown && !(parent is ToolStripOverflow))
								{
									m_Destination = parent;
								}
							}
							else m_Destination = panelItem.ToolStrip;
						}
						else m_Destination = ddItem.DropDown;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnComponentAdded(object sender, ComponentEventArgs e)
		{
			ToolStripItem item = e.Component as ToolStripItem;
			if (item != null)
			{
				if(m_Destination != null )
				{
					if (item.Owner == null)
					{
						m_Destination.Items.Add(item);
					}
					m_Destination = null;
				}
				
				if (ToolStripItemAdded != null)
				{
					ToolStripItemAdded(this, new ToolStripItemEventArgs(item));
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnComponentRemoving(object sender, ComponentEventArgs e)
		{
			if (e.Component is ToolStripItem)
			{
				ToolStripPanelItemSite.Restore(e.Component as ToolStripItem);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnComponentRemoved(object sender, ComponentEventArgs e)
		{
			if(e.Component is ToolStripItem)
			{
				if (ToolStripItemRemoved != null)
				{
					ToolStripItemRemoved(this, new ToolStripItemEventArgs((ToolStripItem)e.Component));
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnDesignerHostLoadComplete(object sender, EventArgs e)
		{
			IDesignerHost host = sender as IDesignerHost;
			if (host != null)
			{
				host.LoadComplete -= new EventHandler(OnDesignerHostLoadComplete);
			}
			UpdateBehaviors();
		}

		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public Hashtable Designers
		{
			get
			{
				if (m_Designers == null)
				{
					IDesignerHost host = m_parentProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if (host != null)
					{
						FieldInfo fiDesigners = host.GetType().GetField("_designers", BF_NONPUBLIC);
						if (fiDesigners != null)
						{
							m_Designers = fiDesigners.GetValue(host) as Hashtable;
						}
					}
				}
				return m_Designers;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public object PrimarySelection
		{
			get
			{
				object oResult = null;

				ISelectionService selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
				if (selSvc != null)
				{
					oResult = selSvc.PrimarySelection;

					if (oResult == null)
					{
						oResult = this.ToolStripKeyboardHandlingService.SelectedDesignerControl;
					}
				}
				return oResult;
			}
		}
		/// <summary>
		/// Destination for newly created ToolStripItems
		/// </summary>
		public ToolStrip Destination
		{
			get { return m_Destination;  }
			set { m_Destination = value;  }
		}
		/// <summary>
		/// 
		/// </summary>
		internal ToolStripKeyboardHandlingServiceProxy ToolStripKeyboardHandlingService
		{
			get
			{
				if (m_kbdSvc == null)
				{
					m_kbdSvc = new ToolStripKeyboardHandlingServiceProxy(m_parentProvider);
				}
				return m_kbdSvc;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal SelectionManagerProxy SelectionManager
		{
			get
			{
				if (m_selectionManager == null)
				{
					m_selectionManager = new SelectionManagerProxy(this);
				}
				return m_selectionManager;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal ToolStripAdornerWindowServiceProxy AdornerWindowService
		{
			get
			{
				if (m_adornerSvc == null)
				{
					m_adornerSvc = new ToolStripAdornerWindowServiceProxy(this);
				}
				return m_adornerSvc;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private Hashtable Items
		{
			get
			{
				if (m_Items == null)
				{
					m_Items = new Hashtable();
				}
				return m_Items;
			}
		}
		#endregion

		#region Events
		public event ToolStripItemEventHandler ToolStripItemAdded;
		public event ToolStripItemEventHandler ToolStripItemRemoved;
		#endregion

		#region Fields
		IServiceProvider m_parentProvider = null;
		BehaviorService m_behaviorSvc = null;

		ToolStrip m_Destination = null;
		ToolStripKeyboardHandlingServiceProxy m_kbdSvc = null;
		SelectionManagerProxy m_selectionManager = null;
		ToolStripAdornerWindowServiceProxy m_adornerSvc = null;
		ToolStripExBehavior m_toolstripBehavior = null;

		Hashtable m_Items = null;
		Hashtable m_Designers = null;

		static Type m_tGlyphCollection = null;
		static FieldInfo m_fiGlyphs = null;

		#endregion
	}
#endregion
}
#endif
