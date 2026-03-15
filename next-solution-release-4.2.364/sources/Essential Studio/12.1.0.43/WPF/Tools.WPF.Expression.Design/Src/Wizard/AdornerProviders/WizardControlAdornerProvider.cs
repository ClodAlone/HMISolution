#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Design;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Tools.WPF.Expression.Design
{
    /// <summary>
    /// Represents AdornerProvider for SmartTag Support
    /// </summary>
    public class WizardControlAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        /// <summary>
        /// Creates the smart tag.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override SmartTagBase CreateSmartTag(ModelItem item)
        {
            WizardControlSmartTag wcSmartTag = new WizardControlSmartTag();

            wcSmartTag.ModelItem = item;
            wcSmartTag.Context = base.Context;

            return wcSmartTag;
        }
          #if SyncfusionFramework3_5
        /// <summary>
        /// Activates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="view">The view.</param>
        protected override void Activate(ModelItem item, System.Windows.DependencyObject view)
        {
            base.Activate(item, view);
        }
#endif
    }
}
