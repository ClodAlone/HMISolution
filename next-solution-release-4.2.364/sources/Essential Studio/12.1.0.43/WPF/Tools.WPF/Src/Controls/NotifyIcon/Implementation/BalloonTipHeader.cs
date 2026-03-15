// <copyright file="BalloonTipHeader.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///     xmlns:MyNamespace="clr-namespace:Syncfusion.Windows.Tools.Controls.NotifyIcon.Implementation"
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///     xmlns:MyNamespace="clr-namespace:Syncfusion.Windows.Tools.Controls.NotifyIcon.Implementation;assembly=Syncfusion.Windows.Tools.Controls.NotifyIcon.Implementation"
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///     <MyNamespace:BalloonTipHeader  xmlns:MyNamespace="http://schemas.syncfusion.com/wpf"/>
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class BalloonTipHeader : Control
    {
        #region Private members

        /// <summary>
        /// Stores the close button
        /// </summary>
        private Button m_closeBtn = null;

        /// <summary>
        /// Stores the parent.
        /// </summary>
        private BalloonTip m_parent = null;

        #endregion //Private members

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="BalloonTipHeader"/> class.
        /// </summary>
        static BalloonTipHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BalloonTipHeader), new FrameworkPropertyMetadata(typeof(BalloonTipHeader)));
        }

        #endregion //Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the balloon tip header image.
        /// </summary>
        /// <value>The balloon tip header image.</value>
        public ImageSource HeaderImage
        {
            get
            {
                return (ImageSource)GetValue(HeaderImageProperty);
            }

            set
            {
                SetValue(HeaderImageProperty, value);
            }
        }

        #endregion

        #region dependency properties

        /// <summary>
        /// Identifies the <see cref="HeaderImage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderImageProperty =
            DependencyProperty.Register("HeaderImage", typeof(ImageSource), typeof(BalloonTipHeader), new FrameworkPropertyMetadata(null));

        #endregion //dependency properties

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_parent = (BalloonTip)TemplatedParent;
            if (m_parent == null)
            {
                m_parent = (BalloonTip)VisualUtils.FindAncestor(this, typeof(BalloonTip));
            }

            m_closeBtn = (Button)GetTemplateChild("PART_CloseButton");
            if (m_closeBtn != null)
            {
                m_closeBtn.Click += new RoutedEventHandler(CloseBtn_Click);
            }
        }

        #endregion //Overrides

        #region Implementation

        /// <summary>
        /// Handles the Click event of the CloseBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (m_parent != null)
            {
                m_parent.FireCloseButtonClick(e);
            }
        }

        #endregion //Implementation
    }
}
