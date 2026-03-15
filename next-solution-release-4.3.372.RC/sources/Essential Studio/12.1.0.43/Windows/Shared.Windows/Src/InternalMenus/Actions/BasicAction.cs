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
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.InternalMenus
{

    /// <exclude/>
    /// <summary>
    /// This class is the common base class for actions such as
    /// MenuActions, Editactions or plugins
    /// </summary>
    public abstract class BasicAction
	{
		object window   = null;
		
		/// <summary>
 	   /// Inheriting actions must overwrite this method, it is called, when the action
 	   /// occurs.
		/// </summary>
		public abstract void InvokeAction(object sender, EventArgs e);

		/// <exclude/>
		public object MainWindow 
		{
			get {
				return window;
			}
			set {
				window = value;
			}
		}
	}
}
