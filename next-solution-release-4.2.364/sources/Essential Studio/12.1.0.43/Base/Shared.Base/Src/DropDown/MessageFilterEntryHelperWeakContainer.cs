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
using Syncfusion.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;


namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Summary description for MessageFilterEntryHelperWeakContainer.
	/// </summary>
	public class MessageFilterEntryHelperWeakContainer: WeakReference
	{
		public MessageFilterEntryHelperWeakContainer(MessageFilterEntryHelper target) 
			: base(target) 
		{
		}

		public void AppIdleWeakEventHandler(object sender, EventArgs args)
		{
			MessageFilterEntryHelper comboDropDown = (MessageFilterEntryHelper)this.Target;

			if (comboDropDown != null)
			{
				comboDropDown.OnAppIdle(sender, args);
			}
			else
			{
				Application.Idle -= new EventHandler(this.AppIdleWeakEventHandler);
			}
		}
	}
}
