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
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
  public class RibbonTabItemsControl : ItemsControl
    {
        public RibbonTabItemsControl()
        {
           
        }
        bool isItemsSourceAvailable = true;
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RibbonBar;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (this.TemplatedParent != null && this.TemplatedParent is Ribbon)
            {
                if ((this.TemplatedParent as Ribbon).SelectedTabItem != null && (this.TemplatedParent as Ribbon).SelectedTabItem.ItemsSource == null)
                {
                    isItemsSourceAvailable = false;
                    return new ContentPresenter();                    
                }
            }
            return new RibbonBar();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if ((element as ContentPresenter) != null || ((element as RibbonBar) != null && (element as RibbonBar).Items.Count == 0))
            {
                RibbonBar con_Tab;

                if (!isItemsSourceAvailable || item is RibbonBar)
                {
                    base.PrepareContainerForItemOverride(element, item);
                }
                else
                {
                    con_Tab = GetBar(element, item);
                    base.PrepareContainerForItemOverride(con_Tab, con_Tab);
                }
            }
        }

        private RibbonBar GetBar(DependencyObject element, object item)
        {
            RibbonBar returnBar = element as RibbonBar;
            if ((null != ItemTemplate || ItemsSource != null) && returnBar.Header.Trim() == "RibbonBar")
            {
                if (DisplayMemberPath == string.Empty && ItemTemplate == null)
                    returnBar.Header = new string(' ', item.ToString().Length);
                else
                {
                    PropertyDescriptor pDescriptor = DisplayMemberPath!=null ? TypeDescriptor.GetProperties(item)[DisplayMemberPath] : null;
                    if (pDescriptor != null)
                    {
                        object caption = pDescriptor.GetValue(item);
                        if (caption != null)
                        {
                            returnBar.Header = caption.ToString();
                        }
                        else
                            returnBar.Header = item.ToString();
                    }                    
                }

            }

            return returnBar;
        }
    }
}
