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
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Schedule
{
    public class ContentViewItemsControl : ItemsControl
    {
        public ContentViewItemsControl()
        {
            this.DefaultStyleKey = typeof(ContentViewItemsControl);
        }

        #region SelectedIndex (DependencyProperty)

        /// <summary>
        /// Gets / Sets the SelectedIndex for the item to be Visible.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        private bool isSelectedIndexChangedBeforeLoaded = false;
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ContentViewItemsControl), new PropertyMetadata(0, OnSelectedIndexChanged));

        private static void OnSelectedIndexChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var contentView = dpo as ContentViewItemsControl;
            if (contentView.isTemplateApplied)
            {
                contentView.UpdateView();
            }
            else
            {
                contentView.isSelectedIndexChangedBeforeLoaded = true;
            }
        }

        private void UpdateView()
        {
            if (!this.isTemplateApplied)
            {
                return;
            }

            if (this.SelectedIndex > this.Items.Count || this.Items.Count == 0)
            {
                return;
            }


            var item = (UIElement)this.Items[this.SelectedIndex];
            item.Visibility = Visibility.Visible;
            foreach (UIElement el in this.Items)
            {
                if (el != item)
                {
                    el.Visibility = Visibility.Collapsed;
                }
            }

        }

        #endregion

        private bool isTemplateApplied = false;
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            this.UpdateView();
        }
    }

}
