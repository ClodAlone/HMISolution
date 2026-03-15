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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using Syncfusion.Windows.Forms.Tools.Win32API;
using Syncfusion.Windows.Forms.Collections;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region RibbonControlAdvDesigner
	/// <summary>
	/// Designer for RibbonControlAdv.
	/// </summary>
	public class RibbonControlAdvDesigner
		: ControlDesigner, IMessageFilter
	{
		#region Enums
		/// <summary>
		/// Placement of item in RibbonControlAdvHeader.
		/// </summary>
		private enum ItemPlacement
		{
			/// <summary>
			/// Item is situated in the quick panel.
			/// </summary>
			Quick,
			/// <summary>
			/// Item is situated in the main panel.
			/// </summary>
			Main
		}
		#endregion

		#region *** RibbonControlAdvHeaderDesignerActionList
		/// <summary>
		/// Action list for RibbonControlAdvHeaderDesigner.
		/// </summary>
		private class RibbonControlAdvHeaderDesignerActionList
			: DesignerActionList
		{
			#region Fields
			/// <summary>
			/// Underlying RibbonControlAdvHeaderDesigner.
			/// </summary>
			private RibbonControlAdvDesigner m_designer;
			/// <summary>
			/// Collection of action items.
			/// </summary>
			private DesignerActionItemCollection m_actionItems;
			#endregion

			#region Initialization
			/// <summary>
			/// Creates and initializes new instance of RibbonControlAdvHeaderDesignerActionList.
			/// </summary>
			/// <param name="control">Design time RibbonControlAdv instance.</param>
			/// <param name="designer">Underlying RibbonControlAdvDesigner.</param>
			public RibbonControlAdvHeaderDesignerActionList( RibbonControlAdv control, RibbonControlAdvDesigner designer )
				: base( control )
			{
				m_designer = designer;

				m_actionItems = new DesignerActionItemCollection();

				m_actionItems.Add(new DesignerActionMethodItem(this, "EditQuickItems", "Edit quick items..."));
				m_actionItems.Add(new DesignerActionMethodItem(this, "AddMainTabItem", "Add main tab item"));

				m_actionItems.Add(new DesignerActionHeaderItem("Layout", "Layout"));
				m_actionItems.Add(new DesignerActionPropertyItem("ShowQuickPanelBelowRibbon", "ShowQuickPanelBelowRibbon", "Layout"));
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public bool ShowQuickPanelBelowRibbon
			{
				get
				{
					return m_designer.m_control.ShowQuickPanelBelowRibbon;
				}
				set
				{
					PropertyDescriptor pd = TypeDescriptor.GetProperties(m_designer.m_control)["ShowQuickPanelBelowRibbon"];
					if (pd != null)
					{
						pd.SetValue(m_designer.m_control, value);
					}
				}
			}
			#endregion

			#region Private Methods
			/// <summary>
			/// Adds new button to the top items.
			/// </summary>
			private void EditQuickItems()
			{
				RibbonControlAdv ribbon = this.Component as RibbonControlAdv;
				if (ribbon != null)
				{
                    if (ribbon.RibbonStyle == RibbonStyle.Office2007 || ribbon.RibbonStyle == RibbonStyle.Office2010 || ribbon.Show2010CustomizeQuickItemDialog)
                        CustomizeQuickItemsDialog.Execute(ribbon.HeaderInternal);
                    else
                        Office2013CustomizeQuickItemsDialog.Execute(ribbon.HeaderInternal);
					ribbon.HeaderInternal.PerformLayout();

					MemberDescriptor md = TypeDescriptor.GetProperties(ribbon)["Header"];
					m_designer.RaiseComponentChanging(md);
					m_designer.RaiseComponentChanged(md, null, null);
				}
			}
			/// <summary>
			/// Adds new tab item to the main items.
			/// </summary>
			private void AddMainTabItem()
			{
				m_designer.AddNewItem( typeof( ToolStripTabItem ), ItemPlacement.Main );
			}
			#endregion

			#region Overrides
			/// <summary>
			/// Returns collection of action list items.
			/// </summary>
			/// <returns></returns>
			public override DesignerActionItemCollection GetSortedActionItems()
			{
				return m_actionItems;
			}
			#endregion
		}
		#endregion

		#region *** RibbonControlAdvHeaderGlyph
		/// <summary>
		/// Glyph for RibbonControlAdvHeader.
		/// </summary>
		private class RibbonControlAdvHeaderGlyph : ControlBodyGlyph
		{
			#region Fields
			/// <summary>
			/// Underlying control.
			/// </summary>
			private RibbonControlAdv m_control;
			/// <summary>
			/// Behavior sefrvice.
			/// </summary>
			private BehaviorService m_behService;
			#endregion

			#region Initialization
			/// <summary>
			/// Creates and initializes new instance of RibbonControlAdvHeaderGlyph.
			/// </summary>
			/// <param name="control">Underlying control.</param>
			/// <param name="designer">Behavior.</param>
			public RibbonControlAdvHeaderGlyph(RibbonControlAdv control, RibbonControlAdvDesigner designer)
				: base(Rectangle.Empty, Cursors.Default, control, designer )
			{
				m_control = control;
				m_behService = designer.BehaviorService;
			}
			#endregion

			#region Overrides
			/// <summary>
			/// Gets bounds of items area.
			/// </summary>
			public override Rectangle Bounds
			{
				get
				{
					Point edge = m_behService.ControlToAdornerWindow( m_control );

					return new Rectangle( edge.X, edge.Y, m_control.ClientSize.Width - 1, m_control.ClientSize.Height - 1 );
				}
			}
			/// <summary>
			/// Returns true if hit text succeeds.
			/// </summary>
			/// <param name="p"></param>
			/// <returns></returns>
			public override Cursor GetHitTest( Point p )
			{
				if( this.Bounds.Contains( p ) )
				{
					return Cursors.Default;
				}
				return null;
			}
			/// <summary>
			/// Gets bounds of header.
			/// </summary>
			protected Rectangle HeaderBounds
			{
				get
				{
					Point edge = m_behService.ControlToAdornerWindow(m_control.HeaderInternal);

					return new Rectangle(edge.X, edge.Y, m_control.HeaderInternal.ClientSize.Width - 1, m_control.HeaderInternal.ClientSize.Height - 1);
				}
			}
			#endregion
		}
		#endregion

		#region Fields
		/// <summary>
		/// Design time RibbonControlAdv instance.
		/// </summary>
		private RibbonControlAdv m_control;
		/// <summary>
		/// Action lists.
		/// </summary>
		private DesignerActionListCollection m_actionLists;
		/// <summary>
		/// Glyph.
		/// </summary>
		private ControlBodyGlyph m_gRibbonControl;
		/// <summary>
		/// 
		/// </summary>
		private GlyphCollection m_ribbonGlyphs;
		/// <summary>
		/// 
		/// </summary>
		private BehaviorService m_behaviorSvc;
		/// <summary>
		/// 
		/// </summary>
		private ISelectionService m_selectionSvc;
		/// <summary>
		/// 
		/// </summary>
		private IComponentChangeService m_componentChangeSvc;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripExService m_toolStripSvc;
		/// <summary>
		/// 
		/// </summary>
		private RibbonAdornerService m_ribbonAdornerSvc;
		/// <summary>
		/// 
		/// </summary>
		private MenuDropDown m_officeMenu;
		/// <summary>
		/// 
		/// </summary>
		private NativeWindowEx m_adornerWindow;
		/// <summary>
		/// 
		/// </summary>
		static string sOfficeMenu = "OfficeMenu";
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		public RibbonControlAdvDesigner()
		{
			m_ribbonGlyphs = new GlyphCollection();
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes members.
		/// </summary>
		/// <param name="component"></param>
		public override void Initialize( System.ComponentModel.IComponent component )
		{																																																
			base.Initialize( component );

			m_control = component as RibbonControlAdv;

			if (m_control != null)
			{
				m_control.ControlAdded += new ControlEventHandler(OnControlAdded);
				m_control.ControlRemoved += new ControlEventHandler(OnControlRemoved);

				RibbonControlAdvHeader header = m_control.HeaderInternal;
				if(header!=null)
				{
					header.QuickItems.ItemRemoved += new EventHandler<Syncfusion.Windows.Forms.Collections.ListItemEventArgs<ToolStripItem>>(OnQuickItemRemoved);
					header.MainItems.ItemAdded += new EventHandler<Syncfusion.Windows.Forms.Collections.ListItemEventArgs<ToolStripItem>>( OnMainItemAdded );

					ToolStripDropDownItem button = header.MenuButton;
					if (button != null)
					{
						button.DropDownOpening += new EventHandler(OnMenuButtonDropDownOpening);
					}
					ToolStripDropDownItem overflowButton = header.QuickOverflowButton;
					if (overflowButton != null)
					{
						overflowButton.DropDownOpening += new EventHandler(OnQuickOverflowOpening);
					}
				}

				m_behaviorSvc = DesignerUtils.GetBehaviorService(m_control);
				m_selectionSvc = DesignerUtils.GetSelectionService(m_control);
				m_componentChangeSvc = DesignerUtils.GetComponentChangeService(m_control);

				m_toolStripSvc = ToolStripExService.Get(m_control.Site);
				m_ribbonAdornerSvc = RibbonAdornerService.Get(m_control.Site);

				IExtenderProviderService extProvider = GetService(typeof(IExtenderProviderService)) as IExtenderProviderService;

				if (extProvider != null)
				{
					extProvider.AddExtenderProvider(m_control.TabGroups);
				}

				m_selectionSvc.SelectionChanged += new EventHandler(OnSelectionChanged);

				m_componentChangeSvc.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
				m_componentChangeSvc.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			}

			m_actionLists = new DesignerActionListCollection();
			m_actionLists.Add( new RibbonControlAdvHeaderDesignerActionList( m_control, this ) );

			UpdateNestedContainer();
			UpdateRibbonGlyphs();

			SubclassAdornerWindow();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			m_selectionSvc.SelectionChanged -= new EventHandler(OnSelectionChanged);
			m_componentChangeSvc.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
			m_componentChangeSvc.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);

			IExtenderProviderService extProvider = GetService(typeof(IExtenderProviderService)) as IExtenderProviderService;

			if (extProvider != null)
			{
				extProvider.RemoveExtenderProvider(m_control.TabGroups);
			}
			if (m_adornerWindow != null)
			{
				m_adornerWindow.MessageFilter = null;
				m_adornerWindow = null;
			}
			if (m_ribbonAdornerSvc != null)
			{
				m_ribbonAdornerSvc.RemoveGlyphs(m_ribbonGlyphs);
			}

			base.Dispose(disposing);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Adds new item to the control.
		/// </summary>
		/// <param name="toolStripItemType">Type of item to be added.</param>
		/// <param name="placement">Placement of new item.</param>
		private void AddNewItem( Type toolStripItemType, ItemPlacement placement )
		{
			IDesignerHost host = m_control.Site.GetService( typeof( IDesignerHost ) ) as IDesignerHost;

			if( host != null )
			{
				ToolStripItem item = ( ToolStripItem )host.CreateComponent( toolStripItemType );

				if( item != null )
				{
					item.Text = item.Name;
                    item.Tag = item.Name.Remove(0, 16);
					switch( placement )
					{
						case ItemPlacement.Quick:
							item.Image = new Bitmap(typeof(ToolStripButton), "blank.bmp");
							item.DisplayStyle = ToolStripItemDisplayStyle.Image;

							m_control.Header.AddQuickItem( item );
							break;

						case ItemPlacement.Main:
							m_control.Header.AddMainItem( item );
							break;
					}
				}
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Gets hit test.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		protected override bool GetHitTest(Point point)
		{
			return false;
		}
		/// <summary>
		/// Gets collection of action lists.
		/// </summary>
		public override DesignerActionListCollection ActionLists
		{
			get
			{
				return m_actionLists;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="selectionType"></param>
		/// <returns></returns>
		protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
		{
			if (m_gRibbonControl == null)
			{
				m_gRibbonControl = new RibbonControlAdvHeaderGlyph(m_control, this);
			}
			return m_gRibbonControl;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Enables design mode for newly added panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnControlAdded(object sender, ControlEventArgs e)
		{
			RibbonPanel panel = e.Control as RibbonPanel;
			if (panel!=null)
			{
				INestedContainer nestedContainer = this.GetService(typeof(INestedContainer)) as INestedContainer;
				if (nestedContainer != null)
				{
					INameCreationService nameSvc = this.GetService(typeof(INameCreationService)) as INameCreationService;
					if (nameSvc != null)
					{
						string sName = nameSvc.CreateName(nestedContainer, panel.GetType());
						nestedContainer.Add(panel, sName);
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnControlRemoved(object sender, ControlEventArgs e)
		{
			RibbonPanel panel = e.Control as RibbonPanel;
			if (panel != null)
			{
				INestedContainer nestedContainer = this.GetService(typeof(INestedContainer)) as INestedContainer;
				if (nestedContainer != null)
				{
					nestedContainer.Remove(panel);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnQuickItemRemoved(object sender, ListItemEventArgs<ToolStripItem> e)
		{
			QuickToolstripReflectable dropDownButton = e.Item as QuickToolstripReflectable;

			if (dropDownButton != null)
			{
				dropDownButton.DropDown.Opening -= new CancelEventHandler(OnDropDownOpening);
			}
		}
		/// <summary>
		/// Adds new item to container if needed and subscribes for events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMainItemAdded( object sender, ListItemEventArgs<ToolStripItem> e )
		{
			if( e.Item.Site == null )
			{
				ISite site = m_control.Site;

				if( site != null && site.Container != null )
				{
					site.Container.Add( e.Item );
				}
			}
			UpdateRibbonGlyphs();
		}
		/// <summary>
		/// Opening event handler for QuickToolStripDropDownButton
		/// DropDown is disabled in design mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnDropDownOpening( object sender, CancelEventArgs e )
		{
			e.Cancel = true;
		}
		/// <summary>
		/// Opening event handler for MenuButton dropdown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMenuButtonDropDownOpening(object sender, EventArgs e)
		{
			ToolStripMenuButton item = sender as ToolStripMenuButton;

			RibbonControlAdv ribbon = this.Component as RibbonControlAdv;

            if (ribbon != null && (ribbon.RibbonStyle != RibbonStyle.Office2010) && (ribbon.RibbonStyle != RibbonStyle.Office2013)
				&& item != null && item.DropDownItems.Count == 0)
			{
				WindowsAPI.ShowWindow(item.DropDown.Handle, 1);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnQuickOverflowOpening(object sender, EventArgs e)
		{
			ToolStripDropDownItem item = sender as ToolStripDropDownItem;
			if(item !=null)
			{
				DesignerUtils.UpdateDropDownParent(item.DropDown);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnSelectionChanged(object sender, EventArgs e)
		{
			if( !m_control.IsDisposed )
			{
				object selection = m_toolStripSvc.PrimarySelection;

				ToolStripDropDown menuDropDown = m_control.HeaderInternal.MenuButton.DropDown;

				if(menuDropDown.IsAutoGenerated)
				{
					UpdateSelection(menuDropDown, selection);
				}

				ToolStripTabItem tabItem = selection as ToolStripTabItem;
				if (tabItem != null)
				{
					CheckTabItem(tabItem);
				}
				else
				{
					ToolStripEx ts = selection as ToolStripEx;
					if (ts != null)
					{
						RibbonPanel panel = ts.Parent as RibbonPanel;
						
						if (panel != null && panel.TabItem!=null)
						{
							CheckTabItem(panel.TabItem);
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnComponentRemoved(object sender, ComponentEventArgs e)
		{
			ObservableList<ToolStripItem> list = m_control.HeaderInternal.QuickItems;
			for(int i=0, count = list.Count; i<count; i++)
			{
				IQuickItem item = list[i] as IQuickItem;

				if (item!=null && item.Reflects(e.Component))
				{
					list.RemoveAt(i);
					break;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			if (e.Component == m_control && e.Member != null && e.Member.Name == "MenuButtonDropDown")
			{
				UpdateNestedContainer();
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dropDown"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		private bool IsDropDownComponent(ToolStripDropDown dropDown, object component)
		{
			if (component != null)
			{
				return component.Equals(dropDown) || DesignerUtils.IsDropDownItem(dropDown, component as ToolStripItem);
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private bool IsMenuDropDownItem(ToolStripItem item)
		{
			bool bResult = false;

			if (item != null)
			{
				ToolStrip ts = item.Owner;
				if (ts != null)
				{
					ToolStripDropDown dropDown = m_control.HeaderInternal.MenuButton.DropDown;
					if (dropDown != null)
					{
						bResult = dropDown.Equals(ts) || dropDown.Contains(ts);
					}
				}
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ts"></param>
		/// <returns></returns>
		private Component GetOwnerItem(ToolStrip ts)
		{
			Component owner = null;

			ISite site = ts.Site;
			
			if (site != null)
			{
				IContainer container = site.Container;
				if (container != null)
				{
					owner = container.Components[site.Name] as ToolStripItem;
				}
			}

			return owner;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private Component GetNextSelection(ToolStripItem item)
		{
			Component next = null;

			if (item != null)
			{
				ToolStrip ts = item.Owner;

				if (ts != null)
				{
					next = ts.GetNextItem(item, ArrowDirection.Down);

					if (next == null || next == item)
					{
						next = GetOwnerItem(ts);
					}
				}
			}
			return next;
		}
		/// <summary>
		/// 
		/// </summary>
		private void UpdateNestedContainer()
		{
			INestedContainer nestedContainer = this.GetService(typeof(INestedContainer)) as INestedContainer;
			if (nestedContainer != null)
			{
				for (int i = 0, len = nestedContainer.Components.Count; m_officeMenu != null && i < len; i++)
				{
					if (nestedContainer.Components[i] == m_officeMenu)
					{
						nestedContainer.Remove(m_officeMenu);
						m_officeMenu = null;
					}
				}
                MenuDropDown officeMenu = m_control.MenuButtonDropDown as MenuDropDown;
                if (officeMenu != null && officeMenu.IsAutoGenerated)
                {
                    nestedContainer.Add(officeMenu, sOfficeMenu);
                    m_officeMenu = officeMenu;
                }
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void UpdateRibbonGlyphs()
		{
			if (m_ribbonAdornerSvc != null)
			{
				m_ribbonAdornerSvc.RemoveGlyphs(m_ribbonGlyphs);

				m_ribbonGlyphs.Clear();
				m_ribbonGlyphs.Insert(0, new MenuButtonGlyph(m_control));

				for (int i = 0; i < m_control.HeaderInternal.MainItems.Count; i++)
				{
					ToolStripItem item = m_control.HeaderInternal.MainItems[i];

					ComponentGlyph glyph = DesignerUtils.GetToolStripItemGlyph(item);

					if (glyph != null)
					{
						m_ribbonGlyphs.Insert(0, glyph);
					}
				}

				m_ribbonAdornerSvc.AddGlyphs(m_ribbonGlyphs);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dropDown"></param>
		private void UpdateSelection(ToolStripDropDown dropDown, object selection)
		{
			if (dropDown != null && dropDown.Visible)
			{
				if (selection != dropDown && !DesignerUtils.IsDropDownItem(dropDown, selection as ToolStripItem))
				{
					dropDown.Hide();
				}
				else m_behaviorSvc.Invalidate();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="region"></param>
		private void Invalidate(Region region)
		{
			if (m_behaviorSvc != null)
			{
				m_behaviorSvc.Invalidate(region);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rc"></param>
		private void Invalidate(Rectangle rc)
		{
			if (m_behaviorSvc != null)
			{
				m_behaviorSvc.Invalidate(rc);
			}
		}
		/// <summary>
		/// Set Checked state item if necessary.
		/// </summary>
		/// <param name="item"></param>
		private void CheckTabItem(ToolStripTabItem item)
		{
			PropertyDescriptor pd = TypeDescriptor.GetProperties(item).Find("Checked", false);

			bool bChecked = (bool)pd.GetValue(item);

			if (!bChecked)
			{
				pd.SetValue(item, true);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void SubclassAdornerWindow()
		{
			if (m_behaviorSvc != null)
			{
				Type type = m_behaviorSvc.GetType();

				PropertyInfo pi = type.GetProperty("AdornerWindowControl", BindingFlags.Instance | BindingFlags.NonPublic);
				if (pi != null)
				{
					Control adornerWindow = pi.GetValue(m_behaviorSvc, new object[] { }) as Control;
					if (adornerWindow != null)
					{
						m_adornerWindow = new NativeWindowEx(adornerWindow.Handle);
						m_adornerWindow.MessageFilter = this;
					}
				}
			}
		}

		#endregion

		#region IMessageFilter Members

		bool IMessageFilter.PreFilterMessage(ref Message m)
		{
			switch ((Msg)m.Msg)
			{
				case Msg.WM_CONTEXTMENU:
					if (m_selectionSvc!=null)
					{
						if (m_selectionSvc.SelectionCount == 1 && m_selectionSvc.PrimarySelection is ToolStripTabItem)
						{
							return true;
						}
					}
					break;
			}
			return false;
		}

		#endregion
	}
	#endregion
}
#endif
