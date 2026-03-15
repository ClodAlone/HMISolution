#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    #region ToolTipEditor

   public class ToolTipEditor : UITypeEditor
    {
        #region Overrides

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
   
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            object oResult = value;

            IWindowsFormsEditorService editorSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorSvc != null)
            {
                using (ToolTipEditorForm frmEditor = new ToolTipEditorForm(value as ToolTipInfo, provider))
                {
                    if (editorSvc.ShowDialog(frmEditor) == DialogResult.OK)
                    {
                        oResult = new ToolTipInfo(frmEditor.ToolTip);
                    }
                }
            }

            return oResult;
        }
        #endregion
    }
    #endregion
}

#endif