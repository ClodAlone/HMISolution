#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents layout panel for the linear gauge child elements.
    /// </summary>
    public class LinearScaleLayoutPanel : Panel
    {  
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="BorderWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(Thickness), typeof(LinearScaleLayoutPanel), new PropertyMetadata(new PropertyChangedCallback(OnBorderWidthChanged)));

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearScaleLayoutPanel"/> class.
        /// </summary>
        public LinearScaleLayoutPanel()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.Loaded += new RoutedEventHandler(this.LinearScaleLayoutPanelLoaded);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="BorderWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderWidthChanged;

        #endregion

        #region DP getters & setters
        
        /// <summary>
        /// Gets or sets the Border width
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// </value>
        /// <seealso cref="Thickness"/>
        public Thickness BorderWidth
        {
            get
            {
                return (Thickness)GetValue(BorderWidthProperty);
            }

            set
            {
                SetValue(BorderWidthProperty, value);
            }
        }

        #endregion

        #region Overrides

        #endregion

        #region Implementation

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            foreach (FrameworkElement element in Children)
            {
                double width = element.DesiredSize.Width;
                double height = element.DesiredSize.Height;
                if (element is LabelTickSet)
                {
                    LabelTickSet tick = element as LabelTickSet;
                    element.Arrange(new Rect(tick.XOffset, tick.YOffset, width, height));
                }
                else if (element is MarkTickSet)
                {
                    MarkTickSet tick = element as MarkTickSet;
                    element.Arrange(new Rect(tick.XOffset, tick.YOffset, width, height));
                }
                else if (element is LinearBarPointer || element is LinearMarkerPointer)
                {
                          element.Arrange(new Rect(0, 0, width, height));
                }
                else if (element is LinearRange)
                {
                    element.Arrange(new Rect(0, 0, width, height));
                }
                else if (element is GaugeBorder || element.Parent is DigitalGauge)
                {
                    element.Arrange(new Rect(0,0, width, height));
                }
                else
                {
                    element.Arrange(new Rect((finalSize.Width - width)/2, (finalSize.Height - height)/2, width, height));                    
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
            /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement element in Children)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="BorderWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnBorderWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorderThickness();
            if (this.BorderWidthChanged != null)
            {
                this.BorderWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBorderWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScaleLayoutPanel instance = (LinearScaleLayoutPanel)d;
            instance.OnBorderWidthChanged(e);
        }

        /// <summary>
        /// Refreshes Border Thickness
        /// </summary>
        private void RefreshBorderThickness()
        {
            foreach (FrameworkElement elem in this.Children)
            {
                if (elem is Ellipse)
                {
                    Ellipse pathElem = elem as Ellipse;
                    pathElem.StrokeThickness = this.BorderWidth.Left;
                }

                if (elem is Path)
                {
                    Path pathElem = elem as Path;
                    pathElem.StrokeThickness = this.BorderWidth.Left;
                }

                if (elem is GaugeBorder)
                {
                    GaugeBorder gaugeBorder = elem as GaugeBorder;
                    gaugeBorder.RefreshGaugeBorder();
                }
            }
        }

        /// <summary>
        /// Invoked when Scale layout panel is loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        private void LinearScaleLayoutPanelLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshBorderThickness();
        }

        #endregion
    }
}
