#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    public class RibbonFormShortcutEditor : ShortcutKeysEditor
    {
        #region Overrides
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            object obj = base.EditValue(context, provider, value);

            if (CheckValue(context, provider, obj))
            {
                return obj;
            }

            return value;
        }
        #endregion

        #region Implementation
       public bool CheckValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            bool bResult = true;

            if (!Keys.None.Equals(value))
            {
                if (provider != null && context != null)
                {
                    object newItem = context.Instance;
                    if (newItem != null)
                    {
                        IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
                        if (designerHost != null)
                        {
                            RibbonForm form = designerHost.RootComponent as RibbonForm;
                            if (form != null)
                            {
                                object oldItem = form.GetShortcutTarget(value);

                                if (oldItem != null && oldItem != newItem)
                                {
                                    CultureInfo ci = CultureInfo.CurrentCulture;

                                    string sTitle = SR.GetString(ci, "RibbonFormShortcutConfirmTitle",this );
                                    string sText = SR.GetString(ci, "RibbonFormShortcutConfirmText", this);

                                    bResult = MessageBox.Show(sText, sTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                                }
                            }
                        }
                    }
                }
            }

            return bResult;
        }
        #endregion
    }
}
#endif
