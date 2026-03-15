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
using System.Collections.Generic;
using Syncfusion.Silverlight.Shared;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the AutoCompleteListBox Class.
    /// </summary>
    public class AutoCompleteListBox : ListBox
    {

        private Dictionary<object, ListBoxItem> objectToListBoxItem;
        /// <summary>
        /// Gets the object to list box item.
        /// </summary>
        /// <value>The object to list box item.</value>
        public IDictionary<object, ListBoxItem> ObjectToListBoxItem
        {
            get
            {
                if (null == objectToListBoxItem)
                {
                    objectToListBoxItem = new Dictionary<object, ListBoxItem>();
                }
                return objectToListBoxItem;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteListBox"/> class.
        /// </summary>
        public AutoCompleteListBox()
        {           
            
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own item container.
        /// </summary>
        /// <param name="item">The specified item.</param>
        /// <returns>
        /// true if the item is its own item container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is ListBoxItem)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Creates or identifies the element used to display a specified item.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ListBoxItem"/> corresponding to a specified item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            if (null != ItemContainerStyle)
            {
                listBoxItem.Style = ItemContainerStyle;
            }
           
            return listBoxItem;

        }


        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ListBoxItem listBoxItem = element as ListBoxItem; 
            ObjectToListBoxItem[item] = listBoxItem; 
        }
    }
}
