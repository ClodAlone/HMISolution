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
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// GroupBarItemAdornerProvider class
    /// </summary>
    public class GroupBarItemAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        /// <summary>
        /// Creates Smart Tag.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override SmartTagBase CreateSmartTag( ModelItem item )
        {
            GroupBarItemSmartTag GroupBarItemSmarTag = new GroupBarItemSmartTag();

            GroupBarItemSmarTag.ModelItem = item;
            GroupBarItemSmarTag.Context = base.Context;

            return GroupBarItemSmarTag;
        }

#if !SyncfusionFramework4_0 && !SyncfusionFramework4_5
        /// <summary>
        /// This method is called when it is activated.
        /// </summary>
        /// <param name="item"></param>
        protected override void Activate(ModelItem item, DependencyObject view)
        {            
			base.Activate(item, view);
            //GroupBarItem bar_Item = view as GroupBarItem;
            //if (bar_Item != null)
            //{
            //    bar_Item.IsSelected = true;
            //}
        }
#else
        /// <summary>
        /// This method is called when it is activated.
        /// </summary>
        /// <param name="item"></param>
        protected override void Activate(ModelItem item)
        {
            base.Activate(item);
            //if (item != null)
            //{
            //    item.Properties["IsSelected"].SetValue(true);
            //}
        }
#endif
        
    }
}
