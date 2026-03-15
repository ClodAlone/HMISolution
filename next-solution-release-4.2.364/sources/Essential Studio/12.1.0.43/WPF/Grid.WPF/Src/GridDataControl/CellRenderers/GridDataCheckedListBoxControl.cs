#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Customized ListBoxItem implemented for Filters in Grid Data Control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridDataFilterCheckedListBoxItem : ListBoxItem, INotifyPropertyChanged
    {
#if !SILVERLIGHT
        static GridDataFilterCheckedListBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDataFilterCheckedListBoxItem), new FrameworkPropertyMetadata(typeof(GridDataFilterCheckedListBoxItem)));
        }
#else
        public GridDataFilterCheckedListBoxItem()
        {
            this.DefaultStyleKey = typeof(GridDataFilterCheckedListBoxItem);
        }
#endif

        private bool? isChecked = true;

        private string text = string.Empty;

        private object actualValue = null;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.RaisePropertyChangedEvent(new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.RaisePropertyChangedEvent(new PropertyChangedEventArgs("Text"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object ActualValue
        {
            get
            {
                return this.actualValue;
            }

            set
            {
                if (this.actualValue != value)
                {
                    this.actualValue = value;
                    this.RaisePropertyChangedEvent(new PropertyChangedEventArgs("ActualValue"));
                }
            }
        }

        public Type ItemType { get; set; }

        private void RaisePropertyChangedEvent(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
        }
    }

    /// <summary>
    /// Customized ListBox Control implemented for Filters in Grid Data Control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridDataCheckedListBoxControl : ListBox
    {
    }
}
