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
    /// Represents layout panel for the circular gauge child elements.
    /// </summary>
    public class ScalesLayoutPanel : Panel
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.ScalesLayoutPanel.BorderWidth">BorderWidth</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see cref="T:System.Windows.Thickness">System.Windows.Thickness</see></para>
        /// </returns>
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(Thickness), typeof(ScalesLayoutPanel), new PropertyMetadata(new PropertyChangedCallback(OnBorderWidthChanged)));

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.ScalesLayoutPanel">ScalesLayoutPanel</see> class.
        /// </summary>
        public ScalesLayoutPanel()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.Loaded += new RoutedEventHandler(this.ScalesLayoutPanelLoaded);
        }
        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.ScalesLayoutPanel.BorderWidth">BorderWidth</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderWidthChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the borderwidth
        /// </summary>
        /// <value>
        /// Type : <see cref="T:System.Windows.Thickness">System.Window.Thickness</see>
        /// </value>
        /// <seealso cref="Thickness">Thickness</seealso>
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
                if (element is Path)
                {
                    element.Arrange(new Rect(0, 0, width, height));
                }
                else if (element is Ellipse)
                {
                    Ellipse elli = element as Ellipse;
                    width = elli.Width;
                    height = elli.Height;

                    element.Arrange(new Rect((finalSize.Width - width) / 2, (finalSize.Height - height) / 2, width, elli.Height));

                }
                else if (element is CircularPointer)
                {
                    CircularPointer pointer = element as CircularPointer;
                    CircularScale scale = pointer.GaugeElementParent as CircularScale;

                    if (scale != null)
                    {
                        CircularGauge gauge = scale.GaugeElementParent as CircularGauge;
                        if (pointer.PointerNeedleType == PointerNeedleType.Needle)
                        {
                            pointer.Arrange(new Rect(finalSize.Width / 2, (finalSize.Height - height) / 2, width, height));
                        }
                        else if (pointer.PointerNeedleType == PointerNeedleType.Marker)
                        {
                            double pos = 0;
                            if (pointer.PointerPlacement == ScalePlacement.Cross)
                            {
                                pos = (scale.Radius * 2) - ((scale.ScaleBarSize + pointer.DesiredSize.Width) / 2);
                            }
                            else if (pointer.PointerPlacement == ScalePlacement.Inside)
                            {
                                pos = scale.Radius - pointer.DesiredSize.Width;
                                if (scale.Radius > scale.ScaleBarSize)
                                {
                                    pos -= scale.ScaleBarSize - scale.Radius;
                                }
                            }
                            else if (pointer.PointerPlacement == ScalePlacement.Outside)
                            {
                                pos = scale.Radius * 2;
                            }

                            pointer.Arrange(
                                new Rect(pos, scale.Radius - (pointer.DesiredSize.Height / 2), pointer.DesiredSize.Width, pointer.DesiredSize.Height));
                        }
                        else
                        {
                            pointer.Arrange(
                                new Rect(scale.Radius, scale.Radius, pointer.DesiredSize.Width, pointer.DesiredSize.Height));
                        }
                    }
                }
                else if (element is MarkTickSet)
                {
                    MarkTickSet tick = element as MarkTickSet;
                    element.Arrange(new Rect(tick.XOffset, (finalSize.Height - height) / 2, width, height));
                }
                else if (element is LabelTickSet)
                {
                    LabelTickSet tick = element as LabelTickSet;
                    element.Arrange(new Rect(tick.XOffset, (finalSize.Height - height) / 2, width, height));
                }
                //else if (element is LabelTickSet)
                //{
                //    LabelTickSet tick = element as LabelTickSet;
                //    element.Arrange(new Rect(1, (finalSize.Height - height) / 2, width, height));
                //}
                else if (element is CircularScale)
                {
                    CircularScale scale = element as CircularScale;
                    width = scale.Radius * 2;
                    height = scale.Radius * 2;
                    element.Arrange(new Rect((finalSize.Width - width) / 2, (finalSize.Height - height) / 2, width, height));
                }
                else if (element is CircularKnob)
                {
                    CircularKnob knob = element as CircularKnob;
                    width = knob.KnobRadius * 2;
                    height = knob.KnobRadius * 2;
                    element.Arrange(new Rect((finalSize.Width - width) / 2, (finalSize.Height - height) / 2, width, height));
                }
                else if (element is PointerCap)
                {
                    CircularScale scale = ((PointerCap)element).GaugeElementParent as CircularScale;
                    CircularGauge gauge = scale.GaugeElementParent as CircularGauge;
                    element.Arrange(new Rect((finalSize.Width - width) / 2, (finalSize.Height - height) / 2, width, height));
                }
                else if (element is GaugeBorder)
                {
                    GaugeBorder gaugeborder = element as GaugeBorder;
                    width = 300;
                    height = 300;
                    element.Arrange(new Rect((finalSize.Width - width) / 2, (finalSize.Height - height) / 2, width, height));
                }
                else if (element is CircularRange)
                {
                    CircularRange range = element as CircularRange;
                    element.Arrange(new Rect((finalSize.Width - width) / 2 + range.StartWidth / 2, (finalSize.Height - height) / 2, width, height));
                }
                else
                {
                    element.Arrange(new Rect((finalSize.Width - width) / 2, (finalSize.Height - height) / 2, width, height));
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Measures the size
        /// </summary>
        /// <param name="availableSize">size available</param>
        /// <returns>Returns the measured Size</returns>
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
            ScalesLayoutPanel instance = (ScalesLayoutPanel)d;
            instance.OnBorderWidthChanged(e);
        }

        /// <summary>
        /// Refreshs the Border Thickness
        /// </summary>
        private void RefreshBorderThickness()
        {
            foreach (FrameworkElement elem in this.Children)
            {
                if (elem is Path)
                {
                    Path pathElem = elem as Path;
                    pathElem.StrokeThickness = this.BorderWidth.Left;
                }
                if (elem is Ellipse)
                {
                    Ellipse pathElem = elem as Ellipse;
                    pathElem.StrokeThickness = this.BorderWidth.Left;
                }
            }
        }

        /// <summary>
        /// Invoked when the Scales Layout Panel is Loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void ScalesLayoutPanelLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshBorderThickness();
        }

        #endregion
    }
}
