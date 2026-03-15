#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    public class TreeColumnAdvCollectionEditor : UITypeEditor
    {
        #region Class members

        private IWindowsFormsEditorService edSvc;
        #endregion

        #region Class Initialize/Finalize methods

        public TreeColumnAdvCollectionEditor()
        {
        }
        #endregion

        #region Class overrides
 
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null
              && context.Instance != null
              && provider != null)
            {
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    if (context.Instance is MultiColumnTreeView)
                    {
                        MultiColumnTreeView tree = (MultiColumnTreeView)context.Instance;

                        using (ColumnsEditorForm form = new ColumnsEditorForm(tree.Columns, provider))
                        {
                            edSvc.ShowDialog(form);
                        }
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
        #endregion
    }
}