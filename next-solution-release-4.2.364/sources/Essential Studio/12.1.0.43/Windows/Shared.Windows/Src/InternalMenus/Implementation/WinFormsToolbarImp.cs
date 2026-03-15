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
using System.Collections;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.InternalMenus
{
    /// <exclude/>
    /// <summary>
	/// Summary description for XMLToolBarImp.
	/// </summary>
	internal class WinFormsToolBarImp : ToolBarImp
	{
		public WinFormsToolBarImp()
		{
		}
		#region Factory Methods for Toolbar Creation
		public override object[] CreateToolBars(ResourceManager manager, ToolBarItemStructCollection[] structCollectionArray)
		{
            resourceManager = manager;
            ArrayList toolbarList = new ArrayList();
			foreach(ToolBarItemStructCollection collection in structCollectionArray)
			{
				toolbarList.Add(CreateToolBar(collection));
			}
			return (ToolBar[])toolbarList.ToArray(typeof(ToolBar));
		}

		private ToolBar CreateToolBar(ToolBarItemStructCollection defaultToolBarStructCollection)
		{
            return new StandardToolBar(resourceManager, defaultToolBarStructCollection);
		}
		#endregion
	}
}
