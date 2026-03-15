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
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Design
{
	class DesignBindingEditor : 
		UITypeEditor
	{
        
		// Fields
		private DesignBindingPicker designBindingPicker;
        
		// Constructors
		public DesignBindingEditor()
		{
		}
        
		// Methods
		public override /*UITypeEditor*/ UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
                
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{
			IWindowsFormsEditorService edSvc;

			if (provider != null) 
			{
				edSvc = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) 
				{
					if (this.designBindingPicker == null)
						this.designBindingPicker = new DesignBindingPicker(context, true, false);
					this.designBindingPicker.Start(context, edSvc, null, (DesignBinding) value);
					edSvc.DropDownControl(this.designBindingPicker);
					value = this.designBindingPicker.SelectedItem;
					this.designBindingPicker.End();
				}
			}
			return value;
		}
	}
}

