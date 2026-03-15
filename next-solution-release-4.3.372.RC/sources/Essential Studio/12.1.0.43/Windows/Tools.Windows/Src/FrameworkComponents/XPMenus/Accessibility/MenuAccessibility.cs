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
using System.Drawing.Design;
using System.Design;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Text;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
//	public class DockAreaAccessibleObject : Control.ControlAccessibleObject
//	{
//		private MainFrameBarManager manager;
//		private CommandDockBarExt dockBar;
//		private BarControlInternal mainMenuBarControl;
//		public DockAreaAccessibleObject(MainFrameBarManager manager, CommandDockBarExt dockBar)
//			: base(dockBar)
//		{
//			this.manager = manager;
//			this.dockBar = dockBar;
//			CommandBarExt cbe = this.dockBar.GetMainMenuBar();
//			if(cbe != null)
//				this.mainMenuBarControl = cbe.BarControl;
//		}
//
//		public override /*AccessibleObject*/ string Name
//		{
//			get
//			{
//				return "Menu Dock Area";
//			} // end of method get_Name
//		}
//
//		public override /*AccessibleObject*/ Rectangle Bounds
//		{
//			get
//			{
//				return this.dockBar.RectangleToScreen(this.dockBar.ClientRectangle);
//			} // end of method get_Bounds
//		}
//
//		public override string Description
//		{
//			get
//			{
//				return "The area where the menus and toolbars are docked.";
//			}
//		}
//
//		public override string Help
//		{
//			get
//			{
//				return "";
//			}
//		}
//		public override string Value
//		{
//			get{return null;}
//			set{}
//		}
//		public override AccessibleObject GetFocused()
//		{
//			return null;
//		}
//
//		public override AccessibleObject HitTest(int x, int y)
//		{
//			return this.dockBar.HitTest(x, y);
//		}
//	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class BarItemsContainerAccessibleObject : Control.ControlAccessibleObject
	{
		private bool m_bNeedInitChildList = false;
		private ArrayList childList = null;
		protected IBarItemContainer container = null;
		private Hashtable htItemsVsObject = new Hashtable();

		public BarItemsContainerAccessibleObject(IBarItemContainer container, Control parent)
			: base(parent)
		{
			if(!(parent is IBarItemContainerControl))
				throw new ArgumentException("parent should implement the IBarItemContainerControl interface.", "parent");

			this.SetBarItemContainer(container);
		}
		
		public void SetBarItemContainer(IBarItemContainer container)
		{
			if(this.container != container)
			{
				this.container = container;

				this.htItemsVsObject.Clear();

				if(this.childList != null)
					this.childList.Clear();

				if( !m_bNeedInitChildList )
				{
					this.childList = null;
				}
			}
		}

		public override void Select(AccessibleSelection flags)
		{
			base.Select(flags);
		}

		public override string Help
		{
			get
			{
				return "";
			}
		}

		public override AccessibleStates State
		{
			get
			{
				AccessibleStates state = base.State;

				return state;
			}
		}

		public override string Value
		{
			get
			{
				return null;
			}
			set{}
		}

		public override AccessibleObject GetFocused()
		{
			return null;
		}

		public override int GetChildCount()
		{
			if(this.childList == null)
			{
				this.InitChildItemsAccessibilityObjects();
			}
			return this.childList.Count;
		}
		private void InitChildItemsAccessibilityObjects()
		{
			this.childList = new ArrayList();
			this.htItemsVsObject.Clear();
			if (this.container != null)							//Added this check to allow container to be null
				AccessibilityUtils.AddItemsToList(this.childList, this.container, this.Owner, this, this.htItemsVsObject);
		}
		public override AccessibleObject GetChild(int index)
		{
			if(this.childList != null && index < this.childList.Count)
				return this.childList[index] as AccessibleObject;
			else
				return null;
		}
		public int GetAccessibleObjectIndexFromBarItem(BarItem item)
		{
			m_bNeedInitChildList = true;

			if(this.childList == null)
			{
				this.InitChildItemsAccessibilityObjects();
			}

			m_bNeedInitChildList = false;
			return this.childList.IndexOf(this.GetAccessibleObjectFromBarItem(item));
		}
		public AccessibleObject GetAccessibleObjectFromBarItem(BarItem item)
		{
			return this.htItemsVsObject[item] as AccessibleObject;
		}

		public override AccessibleObject HitTest(int x, int y)
		{
			if(this.childList == null)
			{
				this.InitChildItemsAccessibilityObjects();
			}
			BarItem hitItem = ((IBarItemContainerControl)this.Owner).HitTest(x, y);
			if(hitItem != null && this.htItemsVsObject[hitItem] != null)
				return (AccessibleObject)this.htItemsVsObject[hitItem];
			else
				return this;
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ToolBarAccessibleObject : BarItemsContainerAccessibleObject
	{
		Bar bar;
		public ToolBarAccessibleObject(Bar container, Control parent)
			: base(container, parent)
		{
			this.bar = container;
		}
		public override AccessibleObject Navigate(AccessibleNavigation navdir)
		{
			Control c = this.Owner;
			// Get the toolbar control.
			Control toolbar = c.Parent;

			// Calculating the left/right siblings based on the corresponding Control order.
			int tindex = toolbar.Parent.Controls.IndexOf(toolbar);

			if(navdir == AccessibleNavigation.Right)
				tindex++;
			else if(navdir == AccessibleNavigation.Left)
				tindex--;
			else if(navdir == AccessibleNavigation.Up)
			{
				return toolbar.Parent.AccessibilityObject;
			}
			else if(navdir == AccessibleNavigation.Down)
			{
				if(this.GetChildCount() > 0)
					return this.GetChild(0);
				else
					return null;
			}
			else
				return base.Navigate(navdir);

			if(tindex >= toolbar.Parent.Controls.Count)
				tindex = 0;
			else if(tindex < 0)
				tindex = toolbar.Parent.Controls.Count - 1;

			return toolbar.Parent.Controls[tindex].AccessibilityObject;
		}

		public override AccessibleRole Role
		{
			get
			{
				return AccessibleRole.MenuBar;
			}
		}
		public override /*AccessibleObject*/ string Name
		{
			get
			{
				return this.bar.BarName;
			} // end of method get_Name
		}
		public override AccessibleObject GetSelected()
		{
			BarControlInternal bc = this.Owner as BarControlInternal;
			return this.GetAccessibleObjectFromBarItem(bc.barRenderer.CurrentHotTrackItem);
		}
		public override AccessibleObject GetFocused()
		{
			return this.GetSelected();
		}

		public void SetBarItemContainer(Bar bar)
		{
			this.bar = bar;
			SetBarItemContainer((IBarItemContainer)bar);
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class MenuGridAccessibleObject : BarItemsContainerAccessibleObject
	{
		public MenuGridAccessibleObject(ParentBarItem parentItem, Control parent)
			: base(parentItem, parent)
		{
		}
		public override AccessibleRole Role
		{
			get
			{
				return AccessibleRole.MenuPopup;
			}
		}
		public override /*AccessibleObject*/ string Name
		{
			get
			{
				if (this.container is ParentBarItem)
					return ((ParentBarItem)this.container).Text;

				return base.Name;
			}
		}
		public override AccessibleObject GetSelected()
		{
			MenuGrid mg = this.Owner as MenuGrid;
			return this.GetAccessibleObjectFromBarItem(mg.CurrentHotTrackItem);
		}
		public override AccessibleObject GetFocused()
		{
			return this.GetSelected();
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class AccessibilityUtils
	{
		public static void AddItemsToList(ArrayList list, IBarItemContainer container, Control owner, AccessibleObject parentAO, Hashtable ht)
		{
			int i = -1;
			foreach(BarItem item in container.Items)
			{
				if(!(item is NewMenuItem))
				{
					i++;
					AccessibleObject ao = null;
					if(item is ParentBarItem)
					{
						ParentBarItem mergedItem = item as ParentBarItem;
						if(container.Manager != null)
							mergedItem = container.Manager.GetMergedEquivalent(mergedItem, mergedItem);
						ao = new SubMenuAccessibleObject(mergedItem, owner, parentAO, i);
					}
					else
						ao = new MenuItemAccessibleObject(item, owner, parentAO, i);

					list.Add(ao);
					ht[item] = ao;
				}
			}
		}
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class SubMenuAccessibleObject : MenuItemAccessibleObject
	{
		private ArrayList childList = null;
		private Hashtable htItemsVsObject = new Hashtable();
		public SubMenuAccessibleObject(ParentBarItem subMenu, Control parent, AccessibleObject parentaso, int childIndex)
			: base(subMenu, parent, parentaso, childIndex)
		{
		}

		// Gets the role for the grid. This is used by accessibility programs.
		public override AccessibleRole Role
		{
			get
			{
				return AccessibleRole.MenuPopup;
			}
		}
		public ParentBarItem SubMenu
		{
			get{return this.Item as ParentBarItem;}
		}
		public override string DefaultAction
		{
			get
			{
				ParentBarItem parentItem = this.Item as ParentBarItem;
				if(parentItem.ParentStyle == ParentBarItemStyle.Default)
					return "Open";
				else
					return base.DefaultAction;
			}
		}

		public override void DoDefaultAction()
		{
			ParentBarItem parentItem = this.Item as ParentBarItem;
			if(parentItem.ParentStyle == ParentBarItemStyle.Default)
				return;
			else
				base.DoDefaultAction();
		}

		public override int GetChildCount()
		{
			if(this.childList == null)
				this.InitChildItemsAccessibilityObjects();

			return this.childList.Count;
		}
		private void InitChildItemsAccessibilityObjects()
		{
			this.childList = new ArrayList();
			this.htItemsVsObject.Clear();
			AccessibilityUtils.AddItemsToList(this.childList, this.SubMenu, this.Owner as BarControlInternal, this, this.htItemsVsObject);
		}
		public override AccessibleObject GetChild(int index)
		{
			if(this.childList == null)
				this.InitChildItemsAccessibilityObjects();

			if(this.childList != null && index < this.childList.Count)
				return this.childList[index] as AccessibleObject;
			else
				return null;
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class MenuItemAccessibleObject : Control.ControlAccessibleObject
	{
		BarItem item;
		AccessibleObject parentaso;
		IBarItemContainerControl containerControl;
		int childIndex;
		public MenuItemAccessibleObject(BarItem item, Control parent, AccessibleObject parentaso, int childIndex)
			: base(parent)
		{
			this.item = item;
			this.parentaso = parentaso;
			this.containerControl = parent as IBarItemContainerControl;
			this.childIndex = childIndex;
		}

		public BarItem Item
		{
			get{return this.item;}
		}


		// Gets the role for the grid. This is used by accessibility programs.
		public override AccessibleRole Role
		{
			get
			{
				return AccessibleRole.MenuItem;
			}
		}

		public override /*AccessibleObject*/ string Name
		{
			get
			{
				string name = this.item.Text.Replace("&", String.Empty);
				if(this.Item is DropDownBarItem)
					name += " (DropDown)";
				return name;
			} // end of method get_Name
		}

		public override /*AccessibleObject*/ Rectangle Bounds
		{
			get
			{
				return this.containerControl.GetBoundsOf(this.item);
			} // end of method get_Bounds
		}

		public override string Description
		{
			get
			{
				return this.item.Tooltip;
			}
		}

		public override string KeyboardShortcut
		{
			get
			{
				return this.item.Shortcut.ToString();
			}
		}

		public override string DefaultAction
		{
			get
			{
				return "Press";
			}
		}
		public override string Help
		{
			get
			{
				return "";
			}
		}

		public override AccessibleObject Parent
		{
			get
			{
				return this.parentaso;
			}
		}

		public override AccessibleStates State
		{
			get
			{
				return this.containerControl.GetAccessibilityState(this.Item);
			}
		}

		public override string Value
		{
			get
			{
				if(this.Item is ComboBoxBarItem)
				{
					ComboBoxBarItem cbitem = this.Item as ComboBoxBarItem;
					return cbitem.TextBoxValue;
				}
				else
					return null;
			}
			set{}
		}

		public override void DoDefaultAction()
		{
			this.Item.PerformClick();
		}

		public override AccessibleObject GetSelected()
		{
			return this;
		}

		public override AccessibleObject HitTest(int x, int y)
		{
			return this;
		}
		public override int GetChildCount()
		{
			return 0;
		}
		public override AccessibleObject Navigate(AccessibleNavigation navdir)
		{
			int myIndex = childIndex;

			if(navdir == AccessibleNavigation.Right || navdir == AccessibleNavigation.Next)
				myIndex++;
			else if(navdir == AccessibleNavigation.Left || navdir == AccessibleNavigation.Previous)
				myIndex--;
			else if(navdir == AccessibleNavigation.Up)
			{
				return this.parentaso;
			}
			else
				return base.Navigate(navdir);

			if(myIndex >= this.parentaso.GetChildCount())
				myIndex = 0;
			else if(myIndex < 0)
				myIndex = this.parentaso.GetChildCount() - 1;

			return this.parentaso.GetChild(myIndex);
		}
	}
}