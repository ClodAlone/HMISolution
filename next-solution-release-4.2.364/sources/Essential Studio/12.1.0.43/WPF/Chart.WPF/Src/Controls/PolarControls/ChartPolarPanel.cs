// <copyright file="ChartPolarPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Data;

    /// <summary>
    /// Represents ChartPolarPanel class
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartPolarPanel : FrameworkElement
    {
        #region Members
        /// <summary>
        /// Initializes m_polarAxisElement
        /// </summary>
        private FrameworkElement m_polarAxisElement;

        /// <summary>
        /// Initializes m_cartesianAxisElement
        /// </summary>
        private FrameworkElement m_cartesianAxisElement;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for AxesThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxesThicknessProperty =
            DependencyProperty.Register("AxesThickness", typeof(Thickness), typeof(ChartPolarPanel), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
          DependencyProperty.Register("Radius", typeof(double), typeof(ChartPolarPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the polar axis.
        /// </summary>
        /// <value>The polar axis.</value>
        public FrameworkElement PolarAxis
        {
            get
            {
                return m_polarAxisElement;
            }

            set
            {
                if (m_polarAxisElement != value)
                {
                    if (m_polarAxisElement != null)
                    {
                        this.RemoveVisualChild(m_polarAxisElement);
                    }

                    m_polarAxisElement = value;

                    if (m_polarAxisElement != null)
                    {
                        this.AddVisualChild(m_polarAxisElement);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Cartesian axis.
        /// </summary>
        /// <value>The Cartesian axis.</value>
        public FrameworkElement CartesianAxis
        {
            get
            {
                return m_cartesianAxisElement;
            }

            set
            {
                if (m_cartesianAxisElement != value)
                {
                    if (m_cartesianAxisElement != null)
                    {
                        this.RemoveVisualChild(m_cartesianAxisElement);
                    }

                    m_cartesianAxisElement = value;

                    if (m_cartesianAxisElement != null)
                    {
                        this.AddVisualChild(m_cartesianAxisElement);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the axes thickness.
        /// </summary>
        /// <value>The axes thickness.</value>
        public Thickness AxesThickness
        {
            get
            {
                return (Thickness)GetValue(AxesThicknessProperty);
            }

            set
            {
                SetValue(AxesThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }

            set
            {
                SetValue(RadiusProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"></see>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is raised.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return index == 0 ? m_polarAxisElement : m_cartesianAxisElement;
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            PolarAxis.Measure(availableSize);
            CartesianAxis.Measure(new Size(availableSize.Width, Math.Min(availableSize.Width, availableSize.Height) / 2));

            double width = Math.Max(availableSize.Width / 2 - Radius, 0);
            double height = Math.Max(availableSize.Height / 2 - Radius, 0);

            this.AxesThickness = new Thickness(width, height, width, height);

            return availableSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double radius = Math.Max(0, this.Radius);
            Rect clientRect = new Rect(finalSize);

            this.PolarAxis.Arrange(clientRect);

            ChartAxis yAxis = this.CartesianAxis.DataContext as ChartAxis;

            if (yAxis != null)
            {
                Point center = ChartLayoutUtils.GetCenter(clientRect);

                if (!yAxis.OpposedPosition)
                {
                    this.CartesianAxis.Arrange(new Rect(center.X - this.CartesianAxis.DesiredSize.Width, center.Y - radius, this.CartesianAxis.DesiredSize.Width, radius));
                }
                else
                {
                    this.CartesianAxis.Arrange(new Rect(center.X, center.Y - radius, this.CartesianAxis.DesiredSize.Width, radius));
                }
            }
            else
            {
                this.CartesianAxis.Arrange(clientRect);
            }

            double width = Math.Max(finalSize.Width / 2 - radius, 0);
            double height = Math.Max(finalSize.Height / 2 - radius, 0);

            this.AxesThickness = new Thickness(width, height, width, height);

            return finalSize;
        }
        #endregion
    }
}
