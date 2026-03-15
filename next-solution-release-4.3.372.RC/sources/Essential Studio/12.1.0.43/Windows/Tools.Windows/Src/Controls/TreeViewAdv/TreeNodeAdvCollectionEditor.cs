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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;


namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Summary description for TreeNodeAdvCollectionEditor.
	/// </summary>
	[Documentation.DocumentationExclude()]
	public class TreeNodeAdvCollectionEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc = null;
		private IServiceProvider serviceProvider;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{

			if (context != null
				&& context.Instance != null
				&& provider != null) 
			{

				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				serviceProvider = provider;

				if (edSvc != null) 
				{
					if(context.Instance is TreeViewAdv)
					{
						TreeViewAdvEditorForm form = new TreeViewAdvEditorForm(context.Instance as TreeViewAdv, provider);
						edSvc.ShowDialog(form);
					}
				}
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
	/// <summary>
	/// Summary description for TreeNodeAdvBaseStylesEditor.
	/// </summary>
	[Documentation.DocumentationExclude()]
	public class TreeNodeAdvBaseStylesEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc = null;
		private IServiceProvider serviceProvider;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{

			if (context != null
				&& context.Instance != null
				&& provider != null) 
			{

				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				serviceProvider = provider;

				if (edSvc != null) 
				{
					if(context.Instance is TreeViewAdv)
					{
						TreeViewAdvBaseStylesEditorForm form = new TreeViewAdvBaseStylesEditorForm(context.Instance as TreeViewAdv);
						edSvc.ShowDialog(form);
					}
				}
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
