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
using System.Xml;
using System.Xml.Serialization;
using System.IO;


namespace Syncfusion.Windows.Forms.InternalMenus
{
    /// <exclude/>
    /// <summary>
	/// Abstract class used to provide implementation of ToolBars
	/// </summary>
	public abstract class ToolBarImp
	{
		/// <exclude/>
		protected ResourceManager resourceManager;
		
		/// <exclude/>
		public ToolBarImp()
		{
		}
		#region Factory Methods
		/// <summary>Derived classes must override.</summary>
		public abstract object[] CreateToolBars(ResourceManager manager,ToolBarItemStructCollection[] structCollectionArray);
		#endregion
	}
	

}
