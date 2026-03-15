// <copyright file="AppMenuAdornerProvider.cs" company="Syncfusion">
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
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Tools.WPF.Expression.Design
{
    /// <summary>
    /// Represents AdornerProvider for SmartTag Support
    /// </summary>
    public class AppMenuAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        /// <summary>
        /// Creates the smart tag.
        /// </summary>
        /// <param name="item">The ModelItem.</param>
        /// <returns>It returns the SmartTagBase</returns>
        protected override SmartTagBase CreateSmartTag(ModelItem item)
        {
            AppMenuSmartTag appMenuSmartTag = new AppMenuSmartTag();
            appMenuSmartTag.ModelItem = item;            
            appMenuSmartTag.Context = base.Context;
            return appMenuSmartTag;          
        }

        ///// <summary>
        ///// Called when adorners are requested for the first time by the designer.
        ///// </summary>
        ///// <param name="item">A <see cref="T:Microsoft.Windows.Design.Model.ModelItem"/> representing the adorned element.</param>
        ///// <param name="view">An instance of the adorned element.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        ///// 	<paramref name="view"/> is null.
        ///// </exception>
        #if SyncfusionFramework3_5
        protected override void Activate(ModelItem item, DependencyObject view)
        {
            base.Activate(item, view);
            if (SmartTag != null)
                SmartTag.ModelItem.Properties["IsPopupOpen"].SetValue(true);
        }
     #endif
        /// <summary>
        /// Called when an adorner provider is about to be discarded by the designer.
        /// </summary>
        protected override void Deactivate()
        {
            if (SmartTag != null)
                SmartTag.ModelItem.Properties["IsPopupOpen"].SetValue(false);
            base.Deactivate();
        }                
    }
}
