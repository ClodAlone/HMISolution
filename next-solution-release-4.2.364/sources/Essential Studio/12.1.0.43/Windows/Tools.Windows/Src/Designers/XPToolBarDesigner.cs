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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.Drawing;

using Syncfusion;
using Syncfusion.ComponentModel;
using Syncfusion.Collections;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.ComponentModel.Design.Serialization;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
	public class XPToolBarDesigner : ControlDesigner, IToolbarDesigners
	{
		private ToolTip tooltip;
		private XPToolBarSerializationProvider bcSerProvider;

		private ISelectionService m_selectionService = null;
		private IDesignerSerializationManager m_designSerMan = null;
		private IComponentChangeService m_componentChangeService = null;

		private bool m_bSelecting = false;

		private XPToolBar m_xpToolBar = null;
		private Bar m_bar = null;
		private BarRenderer m_barRenderer = null;


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		DesignerActionListCollection actionLists;

		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if (null == actionLists)
				{
					actionLists = new DesignerActionListCollection();
					actionLists.Add(
						new XPToolBarActionList(this.Component));
				}
				return actionLists;
			}
		}

#endif

		public override /*ControlDesigner*/ void Initialize(IComponent component)
		{
            BarManager.DesignerBarClone = false;
			base.Initialize(component);

			tooltip = new ToolTip();
			tooltip.AutomaticDelay = 0;
			tooltip.InitialDelay = 0;
			tooltip.ReshowDelay = 0;
			tooltip.Active = true;
			tooltip.ShowAlways = true;
			tooltip.SetToolTip(this.Control, "Add Bar Items into this XPToolBar through the Items Collection or from the BarManager list via simple drag and drop.");

			this.EnableDragDrop(true);
			this.InitBarControl();

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if( m_xpToolBar != null && m_xpToolBar.Bar != null && 
                m_xpToolBar.Bar.Items != null )
            {
                m_xpToolBar.Bar.Items.CollectionChanged += new CollectionChangeEventHandler( Items_CollectionChanged );
            }
#endif
			this.bcSerProvider = new XPToolBarSerializationProvider();
			IDesignerSerializationManager m_designSerMan = this.GetService(typeof(IDesignerSerializationManager)) as IDesignerSerializationManager;

			if( m_designSerMan != null )
			{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

                // Invalid operation exception eliminated.
				CodeDomSerializer bcSerializer =
					(CodeDomSerializer)m_designSerMan.GetSerializer(typeof(XPToolBar), typeof(CodeDomSerializer));
				if(bcSerializer == null || !(bcSerializer is BarControlCodeDomSerializer))
#endif
                m_designSerMan.AddSerializationProvider(this.bcSerProvider);

				OnControlLoad();
			//	m_designSerMan.SerializationComplete += new EventHandler(SerializationComplete);
			}

			m_componentChangeService = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

			if( null != m_componentChangeService )
			{
				m_componentChangeService.ComponentAdded += new ComponentEventHandler(ComponentAdded);
				m_componentChangeService.ComponentRemoved += new ComponentEventHandler(ComponentRemoved);
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        private const string DEF_PROP_NAME_ITEMS = "Items";

        private void Items_CollectionChanged( object sender, CollectionChangeEventArgs e )
        {
            if( m_xpToolBar != null && e.Element != null )
            {
                IDesignerHost designerHost = this.GetService( typeof( IDesignerHost ) ) as IDesignerHost;

                if( designerHost != null && !designerHost.Loading )
                {
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties( m_xpToolBar );
                    MemberDescriptor memberDescriptor = properties[ DEF_PROP_NAME_ITEMS ] as MemberDescriptor;
                    this.RaiseComponentChanged( memberDescriptor, null, null );
                }
            }
        }
#endif

		protected virtual void InitBarControl()
		{
			BarManager manager = FindBarManager();			

			InitBarControl(manager);
		}

		protected virtual void InitBarControl( BarManager manager )
		{
			m_xpToolBar = this.Control as XPToolBar;

			if( m_xpToolBar != null )
			{
				m_xpToolBar.InitBarFromDesigner( manager );
			}
		}
		
		protected BarManager FindBarManager()
		{
			// Parse through all the components in the component collection of the designer
			IDesignerHost iDesignerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			ComponentCollection componentCollection = iDesignerHost.Container.Components;
			BarManager manager = null;

			foreach(IComponent component in componentCollection)
			{
				if(component is BarManager)
				{
					manager = component as BarManager;
					break;
				}
			}

			return manager;
		}

		protected override bool GetHitTest(Point point)
		{
			Point ptClient = m_xpToolBar.PointToClient(point);

			return m_xpToolBar.IsItemHit(ptClient);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if( m_designSerMan != null )
				{
					m_designSerMan.RemoveSerializationProvider(this.bcSerProvider);
					this.bcSerProvider.Dispose();
					this.bcSerProvider = null;

					m_designSerMan.SerializationComplete -= new EventHandler(SerializationComplete);
				}
			}

			base.Dispose(disposing);
		}

		void IToolbarDesigners.SetDirty()
		{
			System.ComponentModel.MemberDescriptor memberDescriptor;
			System.ComponentModel.Design.IDesignerHost iDesignerHost;

			memberDescriptor = (MemberDescriptor)TypeDescriptor.GetProperties((object)this.Component)[(string)"Bar"];
			iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			if (iDesignerHost != null)
				this.RaiseComponentChanged(memberDescriptor,null,null);
		}

	#region Design-time item selection support
		private void SelectionChanged(object sender, EventArgs e)
		{
			if( null != m_selectionService && !m_bSelecting )
			{
				BarItem barItem = m_selectionService.PrimarySelection as BarItem;
				bool bResetCustomizingItem = (null == barItem) || (m_bar.Items.IndexOf(barItem) < 0);

				if( bResetCustomizingItem )
				{
					this.CustomizingItemIndex = -1;
				}
			}
		}

		private void SerializationComplete( object sender, EventArgs e )
		{
			OnControlLoad();
		}

		/// <summary>
		/// Called after control is desserialized.
		/// </summary>
		protected virtual void OnControlLoad()
		{
            m_selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));

            if (null != m_selectionService)
            {
                m_selectionService.SelectionChanged += new EventHandler(SelectionChanged);

                m_barRenderer = m_xpToolBar.Renderer as BarRenderer;
                m_bar = m_xpToolBar.Bar;

                m_barRenderer.CustomizingItemIndexChanged += new EventHandler(CustomizingItemIndexChanged);
            }
		}

		protected int CustomizingItemIndex
		{
			get
			{
				return m_barRenderer.CustomizingItemIndex;
			}
			set
			{
				m_barRenderer.CustomizingItemIndex = value;
			}
		}

		private void CustomizingItemIndexChanged( object sender, EventArgs e )
		{
			int nItem = this.CustomizingItemIndex;
			
			if( nItem >= 0 )
			{
				BarItems items = m_bar.Items;

				if( nItem < items.Count )
				{
					BarItem barItem = items[nItem];

					m_bSelecting = true;
					m_selectionService.SetSelectedComponents( null, SelectionTypes.Replace );
					m_bSelecting = false;
					m_selectionService.SetSelectedComponents( new Object[1] { barItem }, SelectionTypes.Replace );
				}
			}
		}

		private void ComponentAdded(object sender, ComponentEventArgs e)
		{
			IComponent component = e.Component;

            if (null != component)
            {
                BarManager barMan = component as BarManager;

                if (null != barMan)
                {
                    InitBarControl(barMan);
                }
            }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            XPToolBar toolBar = e.Component as XPToolBar;

            if(toolBar != null && toolBar == m_xpToolBar )
            {
                toolBar.SubscribeDragDrop();
            }
#endif
		}

		private void ComponentRemoved(object sender, ComponentEventArgs e)
		{
			IComponent component = e.Component;

            if (null != component && (component is BarManager))
            {
                InitBarControl(null);
            }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            XPToolBar toolBar = e.Component as XPToolBar;

            if (toolBar != null && toolBar == m_xpToolBar )
            {
                toolBar.UnSubscribeDragDrop();
            }
#endif
		}
	#endregion
	}
}
