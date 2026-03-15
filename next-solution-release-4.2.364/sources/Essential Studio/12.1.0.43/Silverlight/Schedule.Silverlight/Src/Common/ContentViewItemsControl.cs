#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
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
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows.Data;

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Class that holds content view items control
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ContentViewItemsControl : ItemsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ContentViewItemsControl"/> class.
        /// </summary>
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

       
        /// <summary>
        ///dependency property of type integer for selected index 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ContentViewItemsControl), new PropertyMetadata(0, OnSelectedIndexChanged));

        private static void OnSelectedIndexChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var contentView = dpo as ContentViewItemsControl;
            if (contentView.isTemplateApplied)
            {
                contentView.UpdateView();
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
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            this.UpdateView();
        }
    }

//#if SyncfusionFramework4_0 && !SILVERLIGHT
//    [System.ComponentModel.DesignTimeVisible(false)]
//#endif

   
}
