#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents Bubble chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    /// <seealso cref="ChartBubbleType"/>
    public sealed class ChartBubbleSegment : Segment
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartBubbleSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Radius dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(ChartBubbleSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties

        /// <summary>
        /// Get or Set TemplateProperty
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the X co-ordinate value. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the X co-ordinate of the bubble.</remarks>
        /// <value>The X co-ordinate value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate value. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the Y co-ordinate of the bubble.</remarks>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the width of the bubble's ellipse.</remarks>
        /// <value>The width.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the height of ellipse.</remarks>
        /// <value>The height.</value>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the radius. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the radius of the bubble.</remarks>
        /// <value>The radius.</value>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_point
        /// </summary>
        private ChartPoint m_point;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBubbleSegment"/> class.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal ChartBubbleSegment(ChartPoint point, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            this.Template=ResourceManager.GetSeriesTemplate(typeof(ChartBubbleType), ChartTypes.Bubble);
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = !series.EnableEffects ? this.Template : ResourceManager.GetSeriesTemplateWithEffects(typeof(ChartBubbleType), ChartTypes.Bubble);
                this.SegmentTemplate = series.ActualSegmentTemplate;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            m_point = point;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            Point pt = transformer.TransformToVisible(m_point.X + (series.XAxis.VisibleRange.Start * (-1)), m_point.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
            this.X = pt.X - Radius;
            this.Y = pt.Y - Radius;
            this.Height = 2 * Radius;
            this.Width = 2 * Radius;
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.m_point = null;
        }
    }

    /// <summary>
    /// Class implementation for ChartBubbleType
    /// </summary>
    public class ChartBubbleType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the MinRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty MinRadiusProperty =
      DependencyProperty.RegisterAttached("MinRadius", typeof(double), typeof(ChartBubbleType), new PropertyMetadata(10d, new PropertyChangedCallback(OnRadiusChanged)));

        /// <summary>
        /// Identifies the MaxRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxRadiusProperty =
      DependencyProperty.RegisterAttached("MaxRadius", typeof(double), typeof(ChartBubbleType), new PropertyMetadata(30d, new PropertyChangedCallback(OnRadiusChanged)));
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the minimal radius of bubble.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The minimal radius</returns>
        public static double GetMinRadius(ChartSeries series)
        {
            return (double)series.GetValue(MinRadiusProperty);
        }

        /// <summary>
        /// Sets the minimal radius of bubble.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetMinRadius(ChartSeries series, double value)
        {
            series.SetValue(MinRadiusProperty, value);
        }

        /// <summary>
        /// Gets the maximal radius of bubble.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The double radius</returns>
        public static double GetMaxRadius(ChartSeries series)
        {
            return (double)series.GetValue(MaxRadiusProperty);
        }

        /// <summary>
        /// Sets the max radius of bubble.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetMaxRadius(ChartSeries series, double value)
        {
            series.SetValue(MaxRadiusProperty, value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Bubble";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            double maxValue = double.MinValue;
            double minRadius = GetMinRadius(series);
            double radius = GetMaxRadius(series) - minRadius;

            SetRange(series, points, 1);
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            if (points.Count != 0)
            {
                maxValue = (from point in points where !point.Y.Equals(double.NaN) select point.Values[1]).Max();
                series.sum = (from point in points select point.Y).Sum();
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (!double.IsNaN(points[i].Y) && !points[i].EmptyPoint)
                {
                    double x1 = points[i].X ;
                    double y1 = points[i].Y;
                    double value1 = points[i].Values[1];
                    if (double.IsNaN(points[i].Values[1]))
                        value1 = (series.sum == 0 ? 1 : series.sum) / points.Count;
                        //points[i].Values[1] = series.sum / points.Count;
                    
                    ChartBubbleSegment segment = new ChartBubbleSegment(new ChartPoint(x1, y1), points[i], series);
                    //segment.Radius = minRadius + ((radius * points[i].Values[1]) / maxValue);
                    segment.Radius = minRadius + ((radius * value1) / maxValue);
                    series.Segments.Add(segment);
                }
                else if (series.ShowEmptyPoints)
                {
                    points[i].EmptyPoint = true;
                    if (series.EmptyPointValue == EmptyPointValue.Zero)
                    {
                        points[i].Y = 0;
                    }
                    else
                    {
                        if (i + 1 == points.Count)
                            points[i].Y = points[i - 1].Y / 2;
                        else
                        {
                            int index;
                            for (index = i + 1; index < points.Count; index++)
                                if (!double.IsNaN(points[index].Y))
                                    break;
                            if (i == 0)
                                points[i].Y = (index == points.Count ? 40 : points[index].Y) / 2;
                            else
                                points[i].Y = points[i - 1].Y / 2 + (index == points.Count ? 40 : points[index].Y) / 2;
                        }
                    }
                    if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                    {
                        double x1 = points[i].X ;
                        double y1 = points[i].Y ;
                        double value1 = points[i].Values[1];
                        if (double.IsNaN(points[i].Values[1]))
                            value1 = (series.sum == 0 ? 1 : series.sum) / points.Count;
                        ChartBubbleSegment segment = new ChartBubbleSegment(new ChartPoint(x1, y1), points[i], series);
                        segment.Radius = minRadius + ((radius * value1) / maxValue);
                        series.Segments.Add(segment);
                    }
                    else if (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                    {
                        double x1 = points[i].X ;
                        double y1 = points[i].Y ;
                        double x2 = 0;
                        double y2 = 0;
                        ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                        ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                        
                        series.Segments.Add(new ScatterSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                    }
                }
            }

            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                    if (!double.IsNaN(points[i].Y))
                        series.Adornments.Add(new ChartAdornment(points[i], points, series, 0d));
                }
            }
        }

        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Updates the series.
        /// </summary>
        /// <param name="series">The Chart series</param>
        /// <param name="points">The indexed points</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            this.CalculateSegments(series, points);
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
