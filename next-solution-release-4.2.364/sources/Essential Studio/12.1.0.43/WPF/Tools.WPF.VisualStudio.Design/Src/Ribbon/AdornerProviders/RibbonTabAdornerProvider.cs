// <copyright file="RibbonTabAdornerProvider.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Class Represents the Adorner
    /// </summary>
    public class RibbonTabAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        /// <summary>
        /// Creates the smart tag.
        /// </summary>
        /// <param name="item">The ModelItem.</param>
        /// <returns>It returns the SmartTagBase</returns>
        protected override SmartTagBase CreateSmartTag(ModelItem item)
        {
            RibbonTabSmartTag TabSmartTag = new RibbonTabSmartTag();
            TabSmartTag.ModelItem = item;
            TabSmartTag.Context = base.Context;

            if (item != null)
                item.Properties["IsChecked"].SetValue(true);

            return TabSmartTag;
        }

        
#if SyncfusionFramework3_5
        protected override void Activate(ModelItem item, DependencyObject view)
        {
            base.Activate(item, view);
            if (SmartTag != null)
                SmartTag.ModelItem.Properties["IsChecked"].SetValue(true);
        }
#endif
#if SyncfusionFramework4_0
        /// <summary>
        /// This method is called whe nit is activated.
        /// </summary>
        /// <param name="item"></param>
        protected override void Activate(ModelItem item)
        {
            base.Activate(item);
            if (SmartTag != null)
                SmartTag.ModelItem.Properties["IsChecked"].SetValue(true);
        }
#endif
        /// <summary>
        /// Called when an adorner provider is about to be discarded by the designer.
        /// </summary>
        protected override void Deactivate()
        {
            if (SmartTag != null)
                SmartTag.ModelItem.Properties["IsChecked"].SetValue(false);
            base.Deactivate();
        }
    }
}
