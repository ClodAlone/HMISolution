// <copyright file="CustomItemsControlcs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the custom Items control
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CustomItemsControl : ItemsControl
    {
        #region Private members
        /// <summary>
        /// Represents the path
        /// </summary>
        private ContentControl m_path;
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path param value.</value>
        public ContentControl Path
        {
            get
            {
                return (ContentControl)GetValue(PathProperty);
            }

            set
            {
                SetValue(PathProperty, value);
            }
        }
        #endregion

        #region DP properties

        /// <summary>
        /// Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding data etc...
        /// </summary>
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(ContentControl), typeof(CustomItemsControl), new UIPropertyMetadata(null));
        #endregion

        #region Overrides

        /// <summary>
        /// Called when the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property changes.
        /// </summary>
        /// <param name="oldValue">Old value of the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property.</param>
        /// <param name="newValue">New value of the <see cref="P:System.Windows.Controls.ItemsControl.ItemsSource"/> property.</param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            ObservableCollection<object> items = (ObservableCollection<object>)ItemsSource;

            if (Path != null && m_path == null)
            {
                m_path = new ContentControl();
                m_path.Template = Path.Template;
            }

            if (items != null && m_path != null)
            {
                items.Add(m_path);
            }

            if (m_path != null)
            {
                m_path.Focusable = false;
            }
        }
        #endregion
    }
}
