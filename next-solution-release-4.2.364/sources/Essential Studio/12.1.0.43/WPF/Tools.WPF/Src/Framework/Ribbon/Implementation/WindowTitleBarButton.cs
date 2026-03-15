// <copyright file="WindowTitleBarButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Window TitleBar Button
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WindowTitleBarButton : Button
    {
        SystemGesture m_systemGesture;
        
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="WindowTitleBarButton"/> class.
        /// </summary>
        static WindowTitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowTitleBarButton), new FrameworkPropertyMetadata(typeof(WindowTitleBarButton)));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the Initialized event. This method is invoked
        /// whenever IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event
        /// data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
            TitleButtonAdorner titleButtonAdorner = new TitleButtonAdorner(this);
            SkinStorage.SetVisualStyle(titleButtonAdorner, SkinStorage.GetVisualStyle(this).ToString());

            try
            {
                //WindowChrome.SetIsHitTestVisibleInChrome(titleButtonAdorner, true);
                adornerLayer.Add(titleButtonAdorner);
            }
            catch
            {
            }

            titleButtonAdorner.MouseUp += new MouseButtonEventHandler(TitleButtonAdorner_MouseUp);
             #if !SyncfusionFramework3_5
            titleButtonAdorner.TouchUp += new EventHandler<TouchEventArgs>(titleButtonAdorner_TouchUp);
#endif

            Binding binding = new Binding("Visibility");
            binding.Source = this;
            Binding binding1 = new Binding("IsEnabled");
            binding1.Source = this;
            Binding binding2 = new Binding("Tag");
            binding2.Source = this;
            titleButtonAdorner.SetBinding(VisibilityProperty, binding);
            titleButtonAdorner.SetBinding(IsEnabledProperty, binding1);
            titleButtonAdorner.SetBinding(TagProperty, binding2);
            titleButtonAdorner.OffsetY = 1.35d;

            if (TemplatedParent is TitleBar && Background == null)
            {
                BindingUtils.SetBinding(this, TemplatedParent as TitleBar, WindowTitleBarButton.BackgroundProperty, TitleBar.BackgroundProperty);
            }
        }

         #if !SyncfusionFramework3_5
        void titleButtonAdorner_TouchUp(object sender, TouchEventArgs e)
        {
            // if (this.AreAnyTouchesOver)
            {
                // if (m_systemGesture == SystemGesture.Tap)
                    this.OnClick();
            }
        }
#endif

        /// <summary>
        /// Handles the MouseUp event of the TitleButtonAdorner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TitleButtonAdorner_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.OnClick();
            }
        }

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        #endregion
    }
}
