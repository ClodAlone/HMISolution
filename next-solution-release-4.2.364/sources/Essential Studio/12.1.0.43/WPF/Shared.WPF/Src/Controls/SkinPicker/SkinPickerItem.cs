// <copyright file="SkinPickerItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// SkinPickerItem to list the skin in <see cref="SkinPicker"/>/>
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SkinPickerItem : ContentControl
    {
        /// <summary>
        /// Initializes static members of the <see cref="SkinPickerItem"/> class.
        /// </summary>
        static SkinPickerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SkinPickerItem), new FrameworkPropertyMetadata(typeof(SkinPickerItem)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinPickerItem"/> class.
        /// </summary>
        public SkinPickerItem()
        {
        }

        /// <summary>
        /// Identifies the IsSelected property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SkinPickerItem), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
    }
}