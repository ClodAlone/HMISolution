// <copyright file="DateTimeComboItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a DateTimeComboItem <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>.
    /// </summary>
    public class DateTimeComboItem : ComboBoxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeComboItem"/>.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDateTimeCombo"/>
        public DateTimeComboItem()
        {
            DefaultStyleKey = typeof(DateTimeComboItem);
            this.Loaded += DateTimeComboItem_Loaded;
        }

        private DateTimeItem _parentDateTimeItem;

        /// <summary>
        /// Represents ParentDateTimeItem
        /// </summary>
        public DateTimeItem ParentDateTimeItem
        {
            get { return _parentDateTimeItem; }
            internal set { _parentDateTimeItem = value; }
        }

        void DateTimeComboItem_Loaded(object sender, RoutedEventArgs e)
        {
            DateTimeComboItem comboitem = sender as DateTimeComboItem;
            DateTimeWrapper wrapper = comboitem.DataContext as DateTimeWrapper;
            if (ParentDateTimeItem.ItemTemplateSelector != null)
            {
                comboitem.ContentTemplate = ParentDateTimeItem.ItemTemplateSelector.SelectTemplate(wrapper, comboitem) as DataTemplate;
            }
        }        
    }
}
