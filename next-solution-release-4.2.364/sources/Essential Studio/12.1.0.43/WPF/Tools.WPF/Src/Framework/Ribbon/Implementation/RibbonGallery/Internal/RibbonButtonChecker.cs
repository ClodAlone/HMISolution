// <copyright file="RibbonButtonChecker.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents RibbonButtonChecker items control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonButtonChecker : ItemsControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the checked button.
        /// </summary>
        /// <value>The checked button.</value>
        public RibbonButton CheckedButton
        {
            get
            {
                return (RibbonButton)GetValue(CheckedButtonProperty);
            }

            set
            {
                SetValue(CheckedButtonProperty, value);
            }
        } 
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines check button.
        /// </summary>
        public static readonly DependencyProperty CheckedButtonProperty =
            DependencyProperty.Register("CheckedButton", typeof(RibbonButton), typeof(RibbonButtonChecker), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCheckedButtonChanged)));

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonButtonChecker"/> class.
        /// </summary>
        static RibbonButtonChecker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonButtonChecker), new FrameworkPropertyMetadata(typeof(RibbonButtonChecker)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonButtonChecker"/> class.
        /// </summary>
        public RibbonButtonChecker()
        {
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when CheckedButton property is changed.
        /// </summary>
        public event PropertyChangedCallback CheckedButtonChanged; 
        #endregion

        #region Imlementation

        /// <summary>
        /// Called when [checked button changed].
        /// </summary>
        /// <param name="d">The d param value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCheckedButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButtonChecker instance = (RibbonButtonChecker)d;
            instance.OnCheckedButtonChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CheckedButtonChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCheckedButtonChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                (e.OldValue as RibbonButton).IsSelected = false;
            }

            if (e.NewValue != null)
            {
                (e.NewValue as RibbonButton).IsSelected = true;
            }

            if (CheckedButtonChanged != null)
            {
                CheckedButtonChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseLeftButtonDown routed event reaches an element in 
        /// its route that is derived from this class. Implement this method to add class handling 
        /// for this event.
        /// </summary>
        /// <param name="e">Returns the button value</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || ( ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnPreviewMouseLeftButtonDown(e);

                Visual visual = e.OriginalSource as Visual;

                if (visual != null)
                {
                    RibbonButton button = VisualUtils.FindAncestor(visual, typeof(RibbonButton)) as RibbonButton;

                    if (button != null)
                    {
                        CheckedButton = button;
                    }
                }
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch)
            {
                base.OnPreviewTouchDown(e);

                Visual visual = e.OriginalSource as Visual;

                if (visual != null)
                {
                    RibbonButton button = VisualUtils.FindAncestor(visual, typeof(RibbonButton)) as RibbonButton;

                    if (button != null)
                    {
                        CheckedButton = button;
                    }
                }
            }
        }
    #endif

        #endregion       
    }
}
