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
using System.Resources;

namespace Syncfusion.Windows.Forms.InternalMenus
{
    /// <exclude/>
    public abstract class MenuFactory
	{
		#region Members
		private object[] menus = null;
		private object[] toolbars = null;
		private MenuImp mImp;
		private ToolBarImp tImp;
		#endregion

		#region Creation Methods
		/// <exclude/>
		public MenuFactory():this(true)
		{
		}
		private MenuFactory(bool create)
		{
			if(create)
			{
				CreateMenuItems();
			}
		}

		
		private void CreateMenuItems()
		{
			CreateMenus();
			CreateToolBars();
		}
		private void CreateMenus()
		{
			mImp = CreateMenuImp();
			menus = mImp.CreateMenus(MenuLoader.CurrentResourceManager,MenuLoader.CurrentMenuItemStructCollection);
		}

		private void CreateToolBars()
		{
			tImp = CreateToolBarImp();
			toolbars = tImp.CreateToolBars(MenuLoader.CurrentResourceManager,MenuLoader.CurrentToolBarItemStructCollection);

		}
		#endregion

		#region Factory Methods
		/// <summary>Derived classes must implement to create a <see cref="MenuImp"/> object.</summary>
		public abstract MenuImp CreateMenuImp();
		/// <summary>Derived classes must implement to create a <see cref="ToolBarImp"/> object.</summary>
		public abstract ToolBarImp CreateToolBarImp();
		#endregion

		#region Readonly Public Properties
		/// <exclude/>
		public object[] Menus
		{
			get
			{
				return menus;
			}
		}

		/// <exclude/>
		public object[] ToolBars
		{
			get
			{
				return toolbars;
			}
		}

		private MenuImp MenuImplementation
		{
			get
			{
				return mImp;
			}
		}

		private ToolBarImp ToolBarImplementation
		{
			get
			{
				return tImp;
			}
		}
		#endregion
	}
}
