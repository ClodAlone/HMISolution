// <copyright file="TitleBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// This class is a container for title bar items in <see cref="VistaWindow"/>.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TitleBar : ContentControl
    {
       
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBar"/> class.
        /// </summary>
        public TitleBar()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(TitleBar));
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="TitleBar"/> class.
        /// </summary>
        static TitleBar()
        {
            //// This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //// This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBar), new FrameworkPropertyMetadata(typeof(TitleBar)));
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion Implementation

        #region Properties

        /// <summary>
        /// Gets or sets the value that represents image source of the <see cref="VistaTitleBar"/>.
        /// </summary>
        /// <value>The icon of the window.</value>
        public ImageSource Icon
        {
            get
            {
                return (ImageSource)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <value>The main window.</value>
        internal Window MainWindow
        {
            get
            {
                return this.TemplatedParent as Window;
            }
        }

        #endregion Properties

        #region Dependency Properties

        /// <summary>
        /// Identifies <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
                DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TitleBar), new PropertyMetadata(null));

        #endregion Dependency Properties
    }
}