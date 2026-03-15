#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents Scatter chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ScatterSegment : Segment
    {
        #region Members
        internal ChartPoint m_bottomLeftPnt;
        internal ChartPoint m_topRightPnt;
        #endregion

        #region Dependency properties
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
                DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ScatterSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the X1 depency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
                DependencyProperty.Register("X1", typeof(double), typeof(ScatterSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y2 depency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
                DependencyProperty.Register("X2", typeof(double), typeof(ScatterSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y1 depency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
                DependencyProperty.Register("Y1", typeof(double), typeof(ScatterSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y2 depency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
                DependencyProperty.Register("Y2", typeof(double), typeof(ScatterSegment), new PropertyMetadata(0d));
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see>
        ///                                       <cref>ChartColumnSegment</cref>
        ///                                   </see>
        ///     class.
        /// </summary>
        /// <param name="bottomLeftPnt">The bottom left PNT.</param>
        /// <param name="topRightPnt">The top right PNT.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal ScatterSegment(ChartPoint bottomLeftPnt, ChartPoint topRightPnt, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection(){ correspondingPoint })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartScatterType), ChartTypes.Scatter);
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = !series.EnableEffects ? this.Template : ResourceManager.GetSeriesTemplateWithEffects(typeof(ChartScatterType), ChartTypes.Scatter);
                this.SegmentTemplate = series.ActualSegmentTemplate;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            m_bottomLeftPnt = bottomLeftPnt;
            m_topRightPnt = topRightPnt;
            this.SetXRange(bottomLeftPnt.X, topRightPnt.X);
            this.SetYRange(bottomLeftPnt.Y, topRightPnt.Y);
        }

        internal ScatterSegment(ChartPoint bottmLeftpnt, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection() { correspondingPoint })
        {
            m_bottomLeftPnt = bottmLeftpnt;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Get or Set Template property
        /// </summary>
        public DataTemplate Template
        {
            get
            {
                return (DataTemplate)GetValue(TemplateProperty);
            }

            set
            {
                SetValue(TemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the starting X line co-ordinate. This is a depency property.
        /// </summary>
        /// <value>The X1 Dependency Property value.</value>
        public double X1
        {
            get
            {
                return (double)GetValue(X1Property);
            }

            set
            {
                SetValue(X1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the ending X line co-ordinate. This is a depency property.
        /// </summary>
        /// <value>The X2 Dependency Property value.</value>
        public double X2
        {
            get
            {
                return (double)GetValue(X2Property);
            }

            set
            {
                SetValue(X2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the starting Y line co-ordinate. This is a depency property.
        /// </summary>
        /// <value>The Y1 Dependency Property value.</value>
        public double Y1
        {
            get
            {
                return (double)GetValue(Y1Property);
            }

            set
            {
                SetValue(Y1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the ending Y line co-ordinate. This is a depency property.
        /// </summary>
        /// <value>The Y2 Dependency Property value.</value>
        public double Y2
        {
            get
            {
                return (double)GetValue(Y2Property);
            }

            set
            {
                SetValue(Y2Property, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            var bottomLeftPointX = m_bottomLeftPnt.X + (series.XAxis.VisibleRange.Start * (-1));
            if (bottomLeftPointX >= -1 && bottomLeftPointX <= (series.XAxis.VisibleRange.End - series.XAxis.VisibleRange.Start) + 1)
            {
                base.Update(transformer);
                Point blPoint = transformer.TransformToVisible( bottomLeftPointX, m_bottomLeftPnt.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                Point trPoint = transformer.TransformToVisible(m_topRightPnt.X, m_topRightPnt.Y, series);


                this.X2 = ChartScatterType.GetScatterWidth(series);
                this.X1 = blPoint.X - (this.X2 / 2);

                this.Y2 = ChartScatterType.GetScatterHeight(series);
                this.Y1 = blPoint.Y - (this.Y2 / 2);
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.m_bottomLeftPnt = null;
            this.m_topRightPnt = null;
        }

       
    }

    /// <summary>
    /// Represents Scatter chart type.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ChartScatterType : ChartType
    {
        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 1);
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            for (int i = 0; i < points.Count; i++)
            {
                if (!double.IsNaN(points[i].Y) && !points[i].EmptyPoint)
                {
                    double x1 = points[i].X ;
                    double y1 = points[i].Y ;
                    double x2 = 0;
                    double y2 = 0;

                    if (series.Area.Host == Host.OLAPChart)
                    {
                        x1 -= 0.5;
                        //y1 -= 0.5;
                    }

                   
                        series.Segments.Add(new ScatterSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[i], series));
                        if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                        {
                            ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                            series.Adornments.Add(new ChartAdornment(series.Area.Host == Host.OLAPChart ? new ChartPoint(points[i].X - 0.5, points[i].Y) : points[i], points, series, 0d));
                        }
                    
                }
                else if (series.ShowEmptyPoints)
                {
                    if (series.EmptyPointValue == EmptyPointValue.Average)
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
                    else
                    {
                        points[i].Y = 0;
                    }
                    double x1 = points[i].X ;
                    double y1 = points[i].Y ;
                    double x2 = 0;
                    double y2 = 0;
                    
                        series.Segments.Add(new ScatterSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[i], series));
                        if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                        {
                        //    ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                        //    series.Adornments.Add(new ChartAdornment(points[i], points, series, 0d));
                            series.Adornments.Add(new ChartAdornment(series.Area.Host == Host.OLAPChart ? new ChartPoint(points[i].X - 0.5, points[i].Y) : points[i], points, series, 0d));
                        }
                    
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Scatter";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            Update(series);
        }
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Return the double Value from the given DependencyObject
       /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetScatterHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ScatterHeightProperty);
        }

        /// <summary>
        /// Sets the value of the FastScatterHeight dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetScatterHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ScatterHeightProperty, value);
        }

        /// <summary>
        /// Indicates the FastScatterHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty ScatterHeightProperty =
                DependencyProperty.RegisterAttached("ScatterHeight", typeof(double), typeof(ChartScatterType), new PropertyMetadata(10d, new PropertyChangedCallback(onHeightDataChanged)));


        private static void onHeightDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Return the Double Value from the given ChartSeries object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetScatterWidth(ChartSeries obj)
        {
            return (double)obj.GetValue(ScatterWidthProperty);
        }

        /// <summary>
        /// Sets the value of the LowValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetScatterWidth(ChartSeries obj, double value)
        {
            obj.SetValue(ScatterWidthProperty, value);
        }

        /// <summary>
        /// Indicates the LowValueInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty ScatterWidthProperty =
                DependencyProperty.RegisterAttached("ScatterWidth", typeof(double), typeof(ChartScatterType), new PropertyMetadata(10d, new PropertyChangedCallback(OnDataChanged)));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
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
