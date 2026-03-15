// <copyright file="TitleButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents window maximize, Minimize or close button.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TitleButton : Button
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TitleButton"/> class.
        /// </summary>
        static TitleButton()
        {
            EnvironmentTest.ValidateLicense(typeof(TitleButton));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleButton), new FrameworkPropertyMetadata(typeof(TitleButton)));
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is close button.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is close button; otherwise, <c>false</c>.
        /// </value>
        public bool IsCloseButton
        {
            get
            {
                return (bool)GetValue(IsCloseButtonProperty);
            }

            internal set
            {
                SetValue(IsCloseButtonProperty, value);
            }
        }

        #endregion Properties

        #region Depeandancy Properties

        /// <summary>
        /// Identifies <see cref="IsCloseButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCloseButtonProperty =
            DependencyProperty.Register("IsCloseButton", typeof(bool), typeof(TitleButton), new UIPropertyMetadata(false));

        #endregion Depeandancy Properties
    }
}