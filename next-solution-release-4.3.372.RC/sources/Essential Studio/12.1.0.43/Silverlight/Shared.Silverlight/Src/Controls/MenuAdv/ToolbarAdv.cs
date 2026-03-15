#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a ToolbarAdv Control.
    /// </summary>
    /// <example>
    /// The control can be added to the application in the following ways. 
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml</term></listheader>
    /// <item>
    /// <description>&lt;syncfusion:ToolbarAdv Height=&quot;150&quot;
    /// Name=&quot;toolbar'/&gt; 
    /// <para>                                </para></description></item></list>
    /// <list type="table">
    /// <listheader>
    /// <term>C#</term></listheader>
    /// <item>
    /// <description>ToolbarAdv toolbar=new ToolbarAdv(); 
    public class ToolbarAdv : MenuAdv
    {
        #region Constructors
        /// <summary>
        /// Public parameterless constructor.
        /// </summary>
        public ToolbarAdv()
        {
            this.DefaultStyleKey = typeof(MenuAdv);
            ContainersToItems = new Dictionary<DependencyObject, object>();
        } 
        #endregion

        #region Overrides
        /// <summary>
        /// Checks if the item added to the ToolbarAdv control is either ToolbarItemAdv or ToolbarSplitItemAdv Control
        /// </summary>
        /// <param name="item">item which is added</param>
        /// <returns>true if item is either ToolbarItemAdv or ToolbarSplitItemAdv; false otherwise.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if ((item is ToolbarItemAdv) || (item is ToolbarSplitItemAdv))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a ToolbarItemAdv object as container for an item.
        /// </summary>
        /// <returns>ToolbarItemAdv object</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolbarItemAdv();
        }

        /// <summary>
        /// Ensures that the item added to the ToolbarAdv control is either ToolbarItemAdv or ToolbarSplitItemAdv Control.
        /// </summary>
        /// <param name="element">Container (Prepared or Own)</param>
        /// <param name="item">item that is added</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ToolbarSplitItemAdv tsiadv = element as ToolbarSplitItemAdv;
            ToolbarItemAdv tiadv = element as ToolbarItemAdv;

            if (tsiadv != null)
            {
                tsiadv.ParentMenuAdv = this;
                tsiadv.MenuReference = this;
            }
            if (tiadv != null)
            {
                tiadv.ParentMenuAdv = this;
            }

            if (!(item is ToolbarItemAdv) && !(item is ToolbarSplitItemAdv))
            {
                if (element is ToolbarItemAdv)
                {
                    tiadv.Content = item;
                }
                else if (element is ToolbarSplitItemAdv)
                {
                    tsiadv.Header = item;
                }
            }

            if ((item is Control) && !((Control)item).IsEnabled)
            {
                if (element is ToolbarItemAdv)
                {
                    tiadv.IsEnabled = false;
                }
                else if (element is ToolbarSplitItemAdv)
                {
                    tsiadv.IsEnabled = false;
                }
            }

            if (element is ToolbarItemAdv)
            {
                if (!tiadv.IsEnabled)
                {
                    tiadv.Opacity = 0.5;
                }
            }
            else if (element is ToolbarSplitItemAdv)
            {
                if (!tsiadv.IsEnabled)
                {
                    tsiadv.Opacity = 0.5;
                }
            }

            ContainersToItems[element] = item;
        } 
        #endregion
    }
}