// <copyright file="InRibbonItemsPresenter.cs" company="Syncfusion">
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
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents InRibbonItemsPresenter class.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class InRibbonItemsPresenter : ItemsPresenter
    {
        #region Properties

        /// <summary>
        /// Gets the actual width of the panel.
        /// </summary>
        /// <value>The actual width of the panel.</value>
        public double PanelActualWidth
        {
            get
            {
                if (VisualChildrenCount > 0)
                {
                    return (GetVisualChild(0) as FrameworkElement).ActualWidth;
                }

                return Double.NaN;
            }
        }

        /// <summary>
        /// Gets the actual height of the panel.
        /// </summary>
        /// <value>The actual height of the panel.</value>
        public double PanelActualHeight
        {
            get
            {
                if (VisualChildrenCount > 0)
                {
                    return (GetVisualChild(0) as FrameworkElement).ActualHeight;
                }

                return Double.NaN;
            }
        }

        /// <summary>
        /// Gets or sets vertical offset of layout.
        /// </summary>
        public double ChildVerticalOffset
        {
            get
            {
                return (double)GetValue(ChildVerticalOffsetProperty);
            }

            set
            {
                SetValue(ChildVerticalOffsetProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when ChildVerticalOffset property is changed.
        /// </summary>
        public event PropertyChangedCallback ChildVerticalOffsetChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines panel vertical offset of layout.
        /// </summary>
        public static readonly DependencyProperty ChildVerticalOffsetProperty =
            DependencyProperty.Register("ChildVerticalOffset", typeof(double), typeof(InRibbonItemsPresenter), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnChildVerticalOffsetChanged)));
        #endregion

        #region Implpementation
        /// <summary>
        /// Calls OnChildVerticalOffsetChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnChildVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InRibbonItemsPresenter instance = (InRibbonItemsPresenter)d;
            instance.OnChildVerticalOffsetChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ChildVerticalOffsetChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnChildVerticalOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            UIElement child = GetVisualChild(0) as UIElement;

            child.Arrange(new Rect(0, (double)e.NewValue, child.RenderSize.Width, child.RenderSize.Height));

            if (ChildVerticalOffsetChanged != null)
            {
                ChildVerticalOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Gets the wrap pabel.
        /// </summary>
        /// <returns>return wrap panel object.</returns>
        internal WrapPanel GetWrapPabel()
        {
            if (VisualChildrenCount > 0)
                return GetVisualChild(0) as WrapPanel;
            else
                return null;
        }

        #endregion
    }
}
