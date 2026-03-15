// <copyright file="LinearScaleLayoutPanel.cs" company="Syncfusion Software">
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
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the layout panel for the Linear Gauge.
    /// </summary>
    /// <remarks>
    /// All the elements of the <see cref="LinearGauge"/> are arranged at the center of the Panel and
    /// then transformed to their respective positions.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LinearScaleLayoutPanel : Panel
    {
        #region Event
        /// <summary>
        /// Event that is raised when <see cref="PanelHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelWidthChanged;
        #endregion Event

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="PanelHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelHeightProperty =
            DependencyProperty.Register("PanelHeight", typeof(double), typeof(LinearScaleLayoutPanel), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnPanelHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="PanelWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelWidthProperty =
            DependencyProperty.Register("PanelWidth", typeof(double), typeof(LinearScaleLayoutPanel), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnPanelWidthChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the height of the panel.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="PanelWidth"/>
        public double PanelHeight
        {
            get
            {
                return (double)GetValue(PanelHeightProperty);
            }

            set
            {
                SetValue(PanelHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the panel.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="PanelHeight"/>
        public double PanelWidth
        {
            get
            {
                return (double)GetValue(PanelWidthProperty);
            }

            set
            {
                SetValue(PanelWidthProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearScaleLayoutPanel"/> class.
        /// </summary>
        public LinearScaleLayoutPanel()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in InternalChildren)
            {
                double width = availableSize.Width;
                double height = availableSize.Height;
                if (element is LinearScale)
                {
                    width = (element as LinearScale).ScaleWidth;
                    height = (element as LinearScale).ScaleBarLength;
                }

                element.Measure(new Size(width, height));
            }

            return new Size(this.PanelWidth, this.PanelHeight);
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        /// <remarks>The children(both the LinearScale and StateIndicator) are arranged at the center
        /// of the Panel,with its(i.e children's) center coinciding with that of the Panel's center.They are then 
        /// transformed to their respective positions(ScaleBar stays at the center but the StateIndicator
        /// moves to the new position)all by themselves</remarks>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size desiredSize = new Size(25, 25);
            if (PanelWidth > 0 && PanelHeight > 0)
            {
                foreach (FrameworkElement element in InternalChildren)
                {
                    double width = element.DesiredSize.Width;
                    double height = element.DesiredSize.Height;
                    element.Arrange(new Rect((PanelWidth - width) / 2, (PanelHeight - height) / 2, width, height));
                }

                desiredSize = new Size(PanelWidth, PanelHeight);
            }

            return desiredSize;
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Updates property value cache and raises <see cref="PanelHeightChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelHeightChanged != null)
            {
                this.PanelHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPanelHeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScaleLayoutPanel instance = (LinearScaleLayoutPanel)d;
            instance.OnPanelHeightChanged(e);
        }

        /// <summary>
        /// Calls OnPanelWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScaleLayoutPanel instance = (LinearScaleLayoutPanel)d;
            instance.OnPanelWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PanelWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelWidthChanged != null)
            {
                PanelWidthChanged(this, e);
            }
        }
        #endregion Implementation
    }
}
