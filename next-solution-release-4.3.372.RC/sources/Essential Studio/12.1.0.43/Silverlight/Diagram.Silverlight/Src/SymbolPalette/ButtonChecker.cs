// <copyright file="ButtonChecker.cs" company="Syncfusion">
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
using System.ComponentModel;


namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents ButtonChecker items control.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class ButtonChecker : ItemsControl
    {
        
        #region Properties

        /// <summary>
        /// Gets or sets the checked button.
        /// </summary>
        /// <value>The checked button.</value>
        public FilterRibbonButton CheckedButton
        {
            get
            {
                return (FilterRibbonButton)GetValue(CheckedButtonProperty);
            }

            set
            {
                SetValue(CheckedButtonProperty, value);
            }
        }

        #endregion

        #region Dependency properties

        /// <summary>
        /// Defines check button. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckedButtonProperty =
            DependencyProperty.Register("CheckedButton", typeof(FilterRibbonButton), typeof(ButtonChecker), new PropertyMetadata(null, new PropertyChangedCallback(OnCheckedButtonChanged)));

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ButtonChecker"/> class.
        /// </summary>
        static ButtonChecker()
        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonChecker), new FrameworkPropertyMetadata(typeof(ButtonChecker)));
#endif
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonChecker"/> class.
        /// </summary>
        public ButtonChecker()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ButtonChecker);
#endif
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
        /// Calls OnCheckedButtonChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCheckedButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonChecker instance = (ButtonChecker)d;
            instance.OnCheckedButtonChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CheckedButtonChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCheckedButtonChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (e.OldValue != null)
            //{
            //    (e.OldValue as FilterRibbonButton).IsSelected = false;
            //}

            //if ((e.NewValue as FilterRibbonButton) != null)
            //{
            //    (e.NewValue as FilterRibbonButton).IsSelected = true;
            //}    
            if (CheckedButton != null)
            {
                CheckedButton.IsSelected = true;
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
        /// <param name="e">The MouseButtonEventArgs.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            //Visual visual = e.OriginalSource as Visual;

            //if (visual != null)
            //{
            //    FilterRibbonButton button = VisualUtils.FindAncestor(visual, typeof(FilterRibbonButton)) as FilterRibbonButton;

            //    if (button != null)
            //    {
            //        CheckedButton = button;
            //    }
            //}
        }
        #endregion
    }
}
