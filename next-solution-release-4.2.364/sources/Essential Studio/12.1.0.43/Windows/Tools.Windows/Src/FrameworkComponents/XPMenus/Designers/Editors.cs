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
using Syncfusion;
using Syncfusion.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Collections;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CustDlgEditor : UITypeEditor
	{
		public override object EditValue(
			ITypeDescriptorContext context, IServiceProvider provider, 
			object value) 
		{
			BarManager manager = null;
			if(context.Instance is BarManager)
				manager = context.Instance as BarManager;
			else if(context.Instance is Bar)
				manager = ((Bar)context.Instance).Manager;
			else if(value is BarItems)
				manager = ((BarItems)value).Manager;

			if(manager != null)
			{
				IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null)
					manager.Customize(host);
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
    
	}
}