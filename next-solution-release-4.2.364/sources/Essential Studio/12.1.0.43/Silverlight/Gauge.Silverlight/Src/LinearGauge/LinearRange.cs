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
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Data;
    using Syncfusion.Windows.Controls.Theming;

    /// <summary>
    /// Represents the range scale visual element.
    /// </summary>
    /// <example>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C#:</term></listheader>
    /// <item>
    /// <description> LinearRange range = new LinearRange(); 
    /// <para>                 range.StartValue = 70; </para>
    /// <para>                 range.EndValue = 100; </para>
    /// <para>                 range.StartWidth = 0; </para>
    /// <para>                 range.EndWidth = 10; </para>
    /// <para>                 range.RangePosition = ScalePlacement.Inside; </para>
    /// <para>                 range.DistanceFromScale = 2; </para>
    /// <para>                 range.Background = new SolidColorBrush( Colors.Orange ); </para>
    /// <para>scale.Children.Add(range);</para></description></item></list>
    /// <para>          </para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>&lt;syncfusion:LinearScale.Ranges&gt; 
    /// <para>    &lt;syncfusion:LinearRange StartValue=&quot;70&quot; EndValue=&quot;100&quot; </para>
    /// <para>          StartWidth=&quot;0&quot; EndWidth=&quot;10&quot;  RangePosition=&quot;Inside&quot;                                  DistanceFromScale=&quot;2&quot;   Background=&quot;Orange&quot;/&gt; </para>
    /// <para>      &lt;/syncfusion:LinearScale.Ranges&gt;</para></description></item></list>
    /// <para></para>
    /// <para></para>
    /// <para> </para>
    /// </example>
    public class LinearRange : RangeBase
    {
        #region Private members

        /// <summary>
        /// Range Geometry
        /// </summary>
        private PathGeometry mrangeGeometry;

        /// <summary>
        /// The path used to draw the range.
        /// </summary>
        private Path mrangePath;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.LinearRange">LinearRange</see> class.
        /// </summary>
        public LinearRange()
        {
            DefaultStyleKey = typeof(LinearRange);

            this.Loaded += new RoutedEventHandler(this.LinearRangeLoaded);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.mrangePath = this.GetTemplateChild("PART_RangePath") as Path;
            this.RefreshRange();
            base.OnApplyTemplate();
        }


        protected override void OnBindIndicatorChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnBindIndicatorChanged(e);

            this.InvalidateArrange();
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Method for Range refresh
        /// </summary>
        internal void RefreshRange()
        {
            if (this.IndicatorColor == null)
            {
                this.IndicatorColor = this.Background;
            }

            if (this.GaugeElementParent is LinearScale)
            {
                LinearScale scale = this.GaugeElementParent as LinearScale;
                double startPos = (scale.ScaleBarLength / 2) - scale.GetPositionByValue(this.StartValue);
                double endPos = (scale.ScaleBarLength / 2) - scale.GetPositionByValue(this.EndValue);
                double posX = 0;
                double mul = 1;
                   if (this.RangePosition == ScalePlacement.Inside)
                    {
                        posX = (-scale.ScaleBarSize / 2) - this.DistanceFromScale;
                    }
                    else if (this.RangePosition == ScalePlacement.Cross)
                    {
                        posX = (this.EndWidth / 2) - this.DistanceFromScale;
                    }
                    else if (this.RangePosition == ScalePlacement.Outside)
                    {
                        posX = (scale.ScaleBarSize / 2) + this.DistanceFromScale;
                        mul *= -1;
                    }
                
                Point leftBottom = new Point(posX, startPos);
                Point leftTop = new Point(posX, endPos);
                Point rightBottom = new Point(posX - (this.StartWidth * mul), startPos);
                Point rightTop = new Point(posX - (this.EndWidth * mul), endPos);

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = leftBottom;
                LineSegment line = new LineSegment();
                line.Point = leftBottom;
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = leftTop;
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = rightTop;
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = rightBottom;
                figure1.Segments.Add(line);
                line = new LineSegment();
                line.Point = leftBottom;
                figure1.Segments.Add(line);
                this.mrangeGeometry = new PathGeometry();
                this.mrangeGeometry.Figures.Add(figure1);
                if (this.mrangePath != null)
                {
                    this.mrangePath.Data = this.mrangeGeometry;
                }
                if (this.mrangePath != null)
                {
                    if (scale.Orientation == GaugeOrientation.Horizontal)
                    {
                        this.mrangePath.RenderTransformOrigin = new Point(0.5, 0.5);
                        
                        if (this.RangePosition == ScalePlacement.Inside)
                        {
                            TransformGroup gro = new TransformGroup();
                            RotateTransform rot = new RotateTransform() { Angle = 90 };
                            TranslateTransform tra = new TranslateTransform() { X = 0, Y = (-scale.ScaleBarSize / 2 - this.DistanceFromScale) };
                            gro.Children.Add(rot);
                            gro.Children.Add(tra);
                            this.mrangePath.RenderTransform = gro;
                        }
                        else if (this.RangePosition == ScalePlacement.Outside)
                        {
                            TransformGroup gro = new TransformGroup();
                            RotateTransform rot = new RotateTransform() { Angle = 90 };
                            TranslateTransform tra = new TranslateTransform() { X = 0, Y = (scale.ScaleBarSize / 2 + this.DistanceFromScale) };
                            gro.Children.Add(rot);
                            gro.Children.Add(tra);
                            this.mrangePath.RenderTransform = gro;
                            
                        }
                        else
                        {
                            this.mrangePath.RenderTransform = new RotateTransform() { Angle = 90 };
                        }
                    }
                    else
                    {
                        this.mrangePath.RenderTransform = null;
                    }
                }

                LinearGauge lineargauge = scale.GaugeElementParent as LinearGauge;

                if (SkinManager.GetVisualStyle(lineargauge).ToString() == "VS2010" && lineargauge.Orientation == GaugeOrientation.Vertical)
                {
                    ResourceDictionary r = new ResourceDictionary();
                    r.Source = new Uri("/Syncfusion.Theming.VS2010;component/Gauge.xaml", UriKind.RelativeOrAbsolute);
                    this.Background = r["VerticalLinearBarBackgroundBrush"] as Brush;
                }

                if (this.BindIndicator)
                {
                    LinearGauge gauge = scale.GaugeElementParent as LinearGauge;
                    if (gauge.StateIndicators.Count == 0)
                    {
                        StateIndicator indicator = new StateIndicator();
                        indicator.IndicatorHeight = 20;
                        indicator.IndicatorWidth = 20;
                        indicator.Location = new Point(50, 80);
                        indicator.IndicatorStyle = IndicatorStyle.CircularLED;
                        Binding binding = new Binding();
                        binding.Source = scale.Pointers[0];
                        binding.Path = new PropertyPath("Value");
                        indicator.SetBinding(StateIndicator.ValueProperty, binding);
                        gauge.StateIndicators.Add(indicator);
                        gauge.RefreshScalesPanel();

                    }
                }
                if (this.BindIndicator)
                {
                    if (scale.GaugeElementParent is LinearGauge)
                    {
                        LinearGauge gauge = scale.GaugeElementParent as LinearGauge;
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
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            return new Size(1, 1);
        }

        /// <summary>
        /// Method called when Range distance from scale changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnDistanceFromScaleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshRange();
            base.OnDistanceFromScaleChanged(e);
        }

        /// <summary>
        /// Method called when Range end value changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnEndValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            this.RefreshRange();
            base.OnEndValueChanged(e);
        }

        /// <summary>
        /// Method called when Range end width changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnEndWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshRange();
            base.OnEndWidthChanged(e);
        }

        /// <summary>
        /// Method called when Range position changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnRangePositionChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshRange();
            base.OnRangePositionChanged(e);
        }

        /// <summary>
        /// Method called when Range Start value changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnStartValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            this.RefreshRange();
            base.OnStartValueChanged(e);
        }

        /// <summary>
        /// Method called when Range start width changed
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnStartWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshRange();
            base.OnStartWidthChanged(e);
        }

        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        private void LinearRangeLoaded(object sender, RoutedEventArgs e)
        {
            LinearScale scale = this.GaugeElementParent as LinearScale;
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

        #endregion
    }
}
