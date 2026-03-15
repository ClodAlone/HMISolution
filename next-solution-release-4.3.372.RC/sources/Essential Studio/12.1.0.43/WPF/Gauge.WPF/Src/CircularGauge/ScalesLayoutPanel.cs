// <copyright file="ScalesLayoutPanel.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents layout panel for the <see cref="CircularGauge"/>.
    /// </summary>
    /// /// <remarks>
    /// All the elements of the <see cref="CircularGauge"/> are arranged at the center of the Panel and
    /// then transformed to their respective positions.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScalesLayoutPanel : Panel
    {
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="PanelSize"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelSizeChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="PanelSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelSizeProperty =
            DependencyProperty.Register("PanelSize", typeof(double), typeof(ScalesLayoutPanel), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnPanelSizeChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the size of the panel.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public double PanelSize
        {
            get
            {
                return (double)GetValue(PanelSizeProperty);
            }

            set
            {
                SetValue(PanelSizeProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="ScalesLayoutPanel"/> class.
        /// </summary>
        public ScalesLayoutPanel()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(ScalesLayoutPanel));
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
                if (element is CircularScale)
                {
                    width = (element as CircularScale).Radius * 2;
                    height = (element as CircularScale).Radius * 2;
                }

                element.Measure(new Size(width, height));
            }

            return availableSize;
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size desiredSize = new Size(25, 25);
            if (PanelSize > 0)
            {
                foreach (FrameworkElement element in InternalChildren)
                {
                    double width = element.DesiredSize.Width;
                    double height = element.DesiredSize.Height;
                    element.Arrange(new Rect(PanelSize - (width / 2), PanelSize - (height / 2), width, height));
                }

                desiredSize = new Size(PanelSize * 2, PanelSize * 2);
            }

            return desiredSize;
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Calls OnPanelSizeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScalesLayoutPanel instance = (ScalesLayoutPanel)d;
            instance.OnPanelSizeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PanelSizeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPanelSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PanelSizeChanged != null)
            {
                this.PanelSizeChanged(this, e);
            }
        }
        #endregion Implementation
    }
}
