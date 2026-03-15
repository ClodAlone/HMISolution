#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class RibbonMenuGroup : DropDownMenuGroup
    {
        /// <summary>
        /// 
        /// </summary>
        public RibbonMenuGroup()
        {
            this.Loaded += new RoutedEventHandler(RibbonMenuGroup_Loaded);            
        }

        /// <summary>
        /// 
        /// </summary>
        public new void HandleLoadedEvent()
        {
            base.HandleLoadedEvent();
            this.Loaded -= new RoutedEventHandler(RibbonMenuGroup_Loaded);
            this.Loaded += new RoutedEventHandler(RibbonMenuGroup_Loaded);           
        }
 
        private void RibbonMenuGroup_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.parentApplicationMenu != null)
            {
                if (this.Parent is SplitMenuButton && ((SplitMenuButton)this.Parent).menuGroupLoadCount == 0)
                {
                    ((SplitMenuButton)this.Parent).menuGroupLoadCount++;
                    var p = parentApplicationMenu.ItemsPane.Items[0];
                    if (p is ButtonBase)
                        ((ButtonBase)p).Focus();
                    if (p is HeaderedItemsControl)
                        ((HeaderedItemsControl)p).Focus();
                }
            }
        }

        internal ApplicationMenu parentApplicationMenu = null;
    }
}
