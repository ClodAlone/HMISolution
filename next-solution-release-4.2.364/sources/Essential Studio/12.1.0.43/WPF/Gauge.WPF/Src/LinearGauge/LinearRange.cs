// <copyright file="LinearRange.cs" company="Syncfusion Software">
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Data;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the range visual element, which spans a range of consecutive values 
    /// represented by a visual.
    /// </summary>
    /// <remarks>
    /// The Range's <see cref="RangeBase.EndValue"/> should be specified before <see cref="RangeBase.StartValue"/>
    /// because of the constraint that the <see cref="RangeBase.StartValue"/> should not be greater
    /// than <see cref="RangeBase.EndValue"/>    
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="LinearRangeSample.Window1" Title="LinearRangeSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:LinearGauge CenterFrameFillColor="Brown" Name="linearGauge1">
    ///             <syncfusion:LinearGauge.Scales>
    ///                 <syncfusion:LinearScale Name="LinearScale" Minimum="0" Maximum="100" 
    ///                                         MinorIntervalValue="2" MajorIntervalValue="10" ScaleBarSize="20"
    ///                                         ScaleBarLength="260">
    ///                     <syncfusion:LinearScale.Ranges>
    ///                         <syncfusion:LinearRange StartValue="70" EndValue="100" 
    ///                                                 StartWidth="0" EndWidth="10" 
    ///                                                 RangePosition="Inside" DistanceFromScale="1"
    ///                                                 BackgroundBrush="OrangeRed" />
    ///                     </syncfusion:LinearScale.Ranges>
    ///                 </syncfusion:LinearScale>
    ///             </syncfusion:LinearGauge.Scales>
    ///         </syncfusion:LinearGauge>
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace LinearRangeSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private LinearGauge linearGauge1;<para/>
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 linearGauge1 = new LinearGauge();
    ///                 this.linearGauge1.CenterFrameFillColor = Colors.Brown;<para/>
    ///                 LinearScale scale = new LinearScale();
    ///                 scale.Minimum = 0;
    ///                 scale.Maximum = 100;
    ///                 scale.MinorIntervalValue = 2;
    ///                 scale.MajorIntervalValue = 10;
    ///                 scale.ScaleBarSize = 20;
    ///                 scale.ScaleBarLength = 260;
    ///                 linearGauge1.Scales.Add(scale);<para/>
    ///                 LinearRange range = new LinearRange();
    ///                 range.StartValue = 70;
    ///                 range.EndValue = 100;
    ///                 range.StartWidth = 0;
    ///                 range.EndWidth = 10;
    ///                 range.RangePosition = ScalePlacement.Inside;
    ///                 range.DistanceFromScale = 2;
    ///                 range.BackgroundBrush = new SolidColorBrush(Colors.OrangeRed);
    ///                 scale.Ranges.Add(range);<para/>
    ///                 this.Content = linearGauge1;
    ///             }
    ///         }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LinearRange : RangeBase
    {
        #region Private Members
        /// <summary>
        /// The geometry used to draw the range.
        /// </summary>
        private PathGeometry m_rangePath;

        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_startsizeRatio;

        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_endsizeRatio;

        #endregion Private Members

        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of LinearGauge's Width.
        /// </summary>
        internal double startSizeRatio
        {
            get
            {
                return m_startsizeRatio;
            }

            set
            {
                m_startsizeRatio = value;
            }
        }
        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of LinearGauge's Width.
        /// </summary>
        internal double endSizeRatio
        {
            get
            {
                return m_endsizeRatio;
            }

            set
            {
                m_endsizeRatio = value;
            }
        }
        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="LinearRange"/> class.
        /// Overrides the meta data for default template.
        /// </summary>
        static LinearRange()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinearRange), new FrameworkPropertyMetadata(typeof(LinearRange)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearRange"/> class.
        /// </summary>
        public LinearRange()
        {
            this.Loaded += new RoutedEventHandler(LinearRangeLoaded);
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
            return new Size(this.StartWidth, 1);
        }

        /// <summary>
        /// Updates property value cache and raises StartValueChanged /> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected override void OnBindIndicatorChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnBindIndicatorChanged(e);
            if (this.BindIndicator)
            {
                if (this.VisualParent is LinearScale)
                {
                    LinearScale scale = this.VisualParent as LinearScale;
                    LinearGauge gauge = scale.gaugeReference as LinearGauge;
                    if (gauge.StateIndicators.Count == 0)
                    {
                        StateIndicator indicator = new StateIndicator();
                        indicator.Height = 15;
                        indicator.Width = 15;
                        indicator.Location = new Point(15, 95);
                        indicator.IndicatorStyle = IndicatorStyle.CircularLED;
                        Binding binding = new Binding();
                        binding.Source = scale.Pointers[0];
                        binding.Path = new PropertyPath("Value");
                        indicator.ActiveBackgroundBrush = this.IndicatorColor;
                        indicator.SetBinding(StateIndicator.ValueProperty, binding);
                        gauge.StateIndicators.Add(indicator);
                        gauge.InvalidateArrange();
                    }
                }
            }
            this.InvalidateArrange();
        }


        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.IndicatorColor == null)
                this.IndicatorColor = this.BackgroundBrush;
            if (this.VisualParent is LinearScale)
            {
                double posX = 0;
                double mul = 1;
                double startValue = this.StartValue;
                double endValue = this.EndValue;
                double startWidth = this.StartWidth;
                double endWidth = this.EndWidth;
                double startPos;
                double endPos;
                m_rangePath = new PathGeometry();
                LinearScale scale = this.VisualParent as LinearScale;

                if (startValue > endValue)
                {
                    double temp;
                    temp = startWidth;
                    startWidth = endWidth;
                    endWidth = temp;
                }

                if (startValue < endValue)
                {
                    startPos = (scale.ScaleBarLength / 2) - scale.GetPositionByValue(startValue);
                    endPos = (scale.ScaleBarLength / 2) - scale.GetPositionByValue(endValue);
                }
                else
                {
                    startPos = (scale.ScaleBarLength / 2) - scale.GetPositionByValue(endValue);
                    endPos = (scale.ScaleBarLength / 2) - scale.GetPositionByValue(startValue);
                }

                if (this.RangePosition == ScalePlacement.Inside)
                {
                    posX = (-scale.ScaleBarSize / 2) - this.DistanceFromScale;
                }
                else if (this.RangePosition == ScalePlacement.Cross)
                {
                    posX = (endWidth / 2) + this.DistanceFromScale;
                }
                else if (this.RangePosition == ScalePlacement.Outside)
                {
                    posX = (scale.ScaleBarSize / 2) + this.DistanceFromScale;
                    mul *= -1;
                }

                Point leftBottom = new Point(posX, startPos);
                Point leftTop = new Point(posX, endPos);
                Point rightBottom = new Point(posX - (startWidth * mul), startPos);
                Point rightTop = new Point(posX - (endWidth * mul), endPos);

                if (scale.Orientation == GaugeOrientation.Horizontal)
                {
                    leftBottom = new Point(startPos, posX);
                    leftTop = new Point(endPos, posX);
                    rightBottom = new Point(startPos, posX - (startWidth * mul));
                    rightTop = new Point(endPos, posX - (endWidth * mul));
                }
                LineSegment lb = new LineSegment(leftBottom, true);
                LineSegment lt = new LineSegment(leftTop, true);
                LineSegment rt = new LineSegment(rightTop, true);
                LineSegment rb = new LineSegment(rightBottom, true);
                PathFigure figure1 = new PathFigure(
                    leftBottom,
                    new PathSegment[] { lb, lt, rt, rb },
                    true);
                (m_rangePath as PathGeometry).Figures.Add(figure1);


                if (this.BindIndicator)
                {
                    if (this.VisualParent is LinearScale)
                    {
                        LinearGauge gauge = scale.gaugeReference as LinearGauge;
                        if (gauge.StateIndicators.Count == 1)
                        {
                            StateIndicator indicator = gauge.StateIndicators[0] as StateIndicator;
                            indicator.StateRanges.Add(new StateRange(this.StartValue, this.EndValue, this.IndicatorColor));
                        }
                        else if (gauge.StateIndicators.Count > 1)
                        {
                            StateIndicator indicator = gauge.StateIndicators[1] as StateIndicator;
                            indicator.StateRanges.Add(new StateRange(this.StartValue, this.EndValue, this.IndicatorColor));

                        }
                        gauge.InvalidateArrange();
                    }
                }

            }

            return finalSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (m_rangePath != null)
            {
                drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_rangePath);
            }
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        private void LinearRangeLoaded(object sender, RoutedEventArgs e)
        {
            LinearScale scale = this.VisualParent as LinearScale;
            if (scale != null)
            {
                if (this.StartValue < scale.Minimum)
                {
                    this.StartValue = scale.Minimum;
                }

                if (this.EndValue > scale.Maximum)
                {
                    this.EndValue = scale.Maximum;
                }
            }
        }
        #endregion Implementation

        #region Added Code
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
        }

        /// <summary>
        /// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="LinearGauge"/> that contains the reference to the Gauge.</param>
        internal void SetScope(LinearGauge gauge)
        {
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which contains the element to set the scope for.</param>
        internal void CalculateScope(DependencyObject obj)
        {
            DependencyObject ele = obj;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!(ns is System.Windows.NameScope))
                    {
                        break;
                    }

                    NameScope.SetNameScope(this, ns);
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }
        #endregion Added Code
    }
}
