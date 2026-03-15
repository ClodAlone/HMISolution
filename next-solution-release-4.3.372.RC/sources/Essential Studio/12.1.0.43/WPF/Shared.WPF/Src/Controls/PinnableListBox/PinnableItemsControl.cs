#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class PinnableItemsControl : ItemsControl
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsPinnedContainer
        {
            get;

            set;
        }
        /// <summary>
        /// 
        /// </summary>
        internal PinnableListBox pinnableListBox;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PinnableListBoxItem;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PinnableListBoxItem();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            PinnableListBoxItem p_item = element as PinnableListBoxItem;
            if (p_item != null)
            {
                p_item.pinnableListBox = pinnableListBox;
                if (p_item.pinnableListBox.pinnableItem != null)
                    p_item.pinnableListBox.pinnableItem = p_item;
                if(p_item.pinnableListBox.isCalledByUpdateItems)
                    p_item.IsPinned = IsPinnedContainer;

            }
            
            base.PrepareContainerForItemOverride(element, item);
        }

    }
}
