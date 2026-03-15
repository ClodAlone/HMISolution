#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
    /// Represents layout panel for digital gauge child elements.
    /// </summary>
    public class DigitalGaugeLayoutPanel : Panel
    {
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="GaugeParent"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GaugeParentChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="PanelWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PanelWidthChanged;
        #endregion Events        

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="GaugeParent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GaugeParentProperty =
            DependencyProperty.Register("GaugeParent", typeof(GaugeBase), typeof(DigitalGaugeLayoutPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnGaugeParentChanged)));

        /// <summary>
        /// Identifies the <see cref="PanelHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelHeightProperty =
            DependencyProperty.Register("PanelHeight", typeof(double), typeof(DigitalGaugeLayoutPanel), new PropertyMetadata(0d, new PropertyChangedCallback(OnPanelHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="PanelWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelWidthProperty =
            DependencyProperty.Register("PanelWidth", typeof(double), typeof(DigitalGaugeLayoutPanel), new PropertyMetadata(0d, new PropertyChangedCallback(OnPanelWidthChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the parent of the panel.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is null.
        /// </value>
        public GaugeBase GaugeParent
        {
            get
            {
                return (GaugeBase)GetValue(GaugeParentProperty);
            }

            set
            {
                SetValue(GaugeParentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the panel.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Initializes a new instance of the <see cref="DigitalGaugeLayoutPanel"/> class.
        /// </summary>
        public DigitalGaugeLayoutPanel()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.Loaded += new RoutedEventHandler(DigitalGaugeLayoutPanelLoaded);
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
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
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
            if (PanelWidth > 0 && PanelHeight > 0)
            {
                double width = 0;
                foreach (FrameworkElement element in Children)
                {
                    width += element.DesiredSize.Width;
                    if (element is CharacterBase)
                    {
                        width += (element as CharacterBase).SegmentWidth;
                    }

                    if (this.GaugeParent is DigitalGauge)
                    {
                        width += (this.GaugeParent as DigitalGauge).CharacterSpacing;
                    }
                }

                width -= (this.GaugeParent as DigitalGauge).CharacterSpacing;
                double offset = 0;
                foreach (FrameworkElement element in Children)
                {
                    double height = element.DesiredSize.Height;
                    element.Arrange(new Rect(((PanelWidth - width) / 2) + offset, (PanelHeight - height) / 2, element.DesiredSize.Width, height));

                    offset += element.DesiredSize.Width;
                    if (this.GaugeParent is DigitalGauge)
                    {
                        offset += (this.GaugeParent as DigitalGauge).CharacterSpacing;
                    }

                    if (element is CharacterBase)
                    {
                        offset += (element as CharacterBase).SegmentWidth;
                    }
                }

                desiredSize = new Size(PanelWidth, PanelHeight);
            }

            return desiredSize;
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Occurs when when parent CharacterSpacing or SegmentWidth properties are changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void CharacterPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           ////this.InvalidateVisual();
        }

        /// <summary>
        /// Occurs when control is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void DigitalGaugeLayoutPanelLoaded(object sender, RoutedEventArgs e)
        {
            if (this.GaugeParent != null && GaugeParent is DigitalGauge)
            {
                (this.GaugeParent as DigitalGauge).CharacterSpacingChanged += new PropertyChangedCallback(CharacterPropertyChanged);
                (this.GaugeParent as DigitalGauge).SegmentWidthChanged += new PropertyChangedCallback(CharacterPropertyChanged);
            }
        }

        /// <summary>
        /// Calls OnGaugeParentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGaugeParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGaugeLayoutPanel instance = (DigitalGaugeLayoutPanel)d;
            instance.OnGaugeParentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="GaugeParentChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGaugeParentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GaugeParentChanged != null)
            {
                this.GaugeParentChanged(this, e);
            }
        }

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
            DigitalGaugeLayoutPanel instance = (DigitalGaugeLayoutPanel)d;
            instance.OnPanelHeightChanged(e);
        }

        /// <summary>
        /// Calls OnPanelWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPanelWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalGaugeLayoutPanel instance = (DigitalGaugeLayoutPanel)d;
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
                this.PanelWidthChanged(this, e);
            }
        }
        #endregion Implementation
    }
}
