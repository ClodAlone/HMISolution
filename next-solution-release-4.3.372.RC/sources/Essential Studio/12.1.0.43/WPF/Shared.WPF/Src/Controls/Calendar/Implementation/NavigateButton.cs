// <copyright file="NavigateButton.cs" company="Syncfusion">
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
    /// Represents a navigate button of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class NavigateButton : ContentControl
    {
        #region Dependency Property

        /// <summary>
        /// Identifies <see cref="Enabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(NavigateButton), new UIPropertyMetadata(true));

        #endregion Dependency Property

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="NavigateButton"/> class.  It overrides some dependency properties.
        /// </summary>
        static NavigateButton()
        {
            // This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            // This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigateButton), new FrameworkPropertyMetadata(typeof(NavigateButton)));
        }

        /// <summary>
        /// Gets or sets a value indicating whether navigate button is enabled.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Navigate button becomes disabled if there is no more
        /// available dates.
        /// </remarks>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool Enabled
        {
            get
            {
                return (bool)GetValue(EnabledProperty);
            }

            set
            {
                SetValue(EnabledProperty, value);
            }
        }

        /// <summary>
        /// Updates data template of the NavigateButton.
        /// </summary>
        /// <param name="template">Data template to be set to the NavigateButton. If it is
        /// null the local value of data template would be cleared.</param>
        protected internal void UpdateCellTemplate(ControlTemplate template)
        {
            if (template != null)
            {
                Template = template;
            }
        }

        #endregion Initialization
    }
}