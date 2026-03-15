#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [DocumentationExclude()]
    public class TreeNodeAdvCollectionEditor : UITypeEditor
    {
        #region Class members
        private IWindowsFormsEditorService edSvc = null;
        private IServiceProvider serviceProvider;
        #endregion

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
                    if (context.Instance is MultiColumnTreeView)
                    {
                        TreeViewAdvEditorForm form = new TreeViewAdvEditorForm(context.Instance as MultiColumnTreeView, provider);
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

    [DocumentationExclude()]
    public class TreeNodeAdvBaseStylesEditor : UITypeEditor
    {
        #region Class members
        private IWindowsFormsEditorService edSvc = null;
        private IServiceProvider serviceProvider;
        #endregion

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
                    if (context.Instance is MultiColumnTreeView)
                    {
                        TreeViewAdvBaseStylesEditorForm form = new TreeViewAdvBaseStylesEditorForm(context.Instance as MultiColumnTreeView);
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