#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for PolarSegment
    /// </summary>
    public class PolarSegment : Segment
    {
        #region Depdency Properties
        /// <summary>
        /// Identifies the X depency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(PolarSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y depency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(PolarSegment), new PropertyMetadata(0d));

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(PolarSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Points property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
          DependencyProperty.Register("Points", typeof(PointCollection), typeof(PolarSegment), new PropertyMetadata(new PointCollection()));
        /// <summary>
        ///  Identifies the FillColor dependency property.
        /// </summary>
        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register("FillColor", typeof(Brush), typeof(PolarSegment), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        /// <summary>
        ///  Identifies the SegInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty SegInteriorProperty =
           DependencyProperty.Register("SegInterior", typeof(Brush), typeof(PolarSegment), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        #endregion

        #region variables
        /// <summary>
        /// MPoint variable declaration in type of chartPointsCollction
        /// </summary>
        protected ChartPointsCollection MPoints = new ChartPointsCollection();
        /// <summary>
        /// MPoint variable declaration in type of ChartPoint
        /// </summary>
        protected ChartPoint MPoint;
        /// <summary>
        /// MIswholeSegment variable declaration
        /// </summary>
        protected bool MIswholeSegment = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Template. This is a depency property.
        /// </summary>
        /// <value>The DataTemplate value.</value>
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
        /// Get or Set fillColor property
        /// </summary>
        public Brush FillColor
        {
            get
            {
                return (Brush)GetValue(FillColorProperty);
            }

            set
            {
                SetValue(FillColorProperty, value);
            }
        }

        /// <summary>
        /// Get or Set segInterior property
        /// </summary>
        public Brush SegInterior
        {
            get
            {
                return (Brush)GetValue(SegInteriorProperty);
            }

            set
            {
                SetValue(SegInteriorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Points co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The Points value.</value>
        public PointCollection Points
        {
            get
            {
                return (PointCollection)GetValue(PointsProperty);
            }

            set
            {
                SetValue(PointsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get
            {
                return (double)GetValue(XProperty);
            }

            set
            {
                SetValue(XProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get
            {
                return (double)GetValue(YProperty);
            }

            set
            {
                SetValue(YProperty, value);
            }
        }
        #endregion

        #region Constructor
        internal PolarSegment(ChartPointsCollection points, ChartPoint point, ChartPointsCollection correspondingPoint, ChartSeries series, bool iswholesegment)
            : base(series, correspondingPoint)
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartPolarType), ChartTypes.Polar);
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = (iswholesegment == false) ? ResourceManager.GetAdornmentsTemplate(typeof(ChartPolarType), "Symbol") : this.Template;
                this.SegmentTemplate = (iswholesegment == false) ? ResourceManager.GetAdornmentsTemplate(typeof(ChartPolarType), "Symbol") : this.Template;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            MPoint = point;
            MPoints = points;
            MIswholeSegment = iswholesegment;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            this.Points.Clear();
            this.Points = new PointCollection();
            if (MIswholeSegment)
            {
                foreach (ChartPoint data in MPoints)
                {
                    Point pt = transformer.TransformToVisible(data.X, data.Y, series);
                    this.Points.Add(pt);
                }
            }
            else
            {
                Point pt = transformer.TransformToVisible(MPoint.X, MPoint.Y, series);
                this.X = pt.X;
                this.Y = pt.Y;
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.MPoints != null)
            {
                for (int temp = 0; temp < this.MPoints.Count; temp++)
                    this.MPoints[temp] = null;
                this.MPoints.Clear();
                this.MPoints.series = null;
                this.MPoints = null;
            }
            if (this.MPoint != null)
                this.MPoint = null;
            if (this.Points != null)
                this.Points.Clear();
            this.Points = null;
        }
    }
    /// <summary>
    /// Class implementation for ChartPolarType
    /// </summary>
    public class ChartPolarType : ChartType
    {
        #region Depedency Property
        /// <summary>
        ///  Identifies the IsClockWise dependency property.
        /// </summary>
        public static readonly DependencyProperty IsClockWiseProperty =
                DependencyProperty.RegisterAttached("IsClockWise", typeof(bool), typeof(ChartPolarType), new PropertyMetadata(false, new PropertyChangedCallback(OnDataChanged)));
        /// <summary>
        ///  Identifies the IsClosed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsClosedProperty =
        DependencyProperty.RegisterAttached("IsClosed", typeof(bool), typeof(ChartPolarType), new PropertyMetadata(true, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        ///  Identifies the DrawType dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawTypeProperty =
        DependencyProperty.RegisterAttached("DrawType", typeof(ChartPolarDrawType), typeof(ChartPolarType), new PropertyMetadata(ChartPolarDrawType.Line, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Static Methods
        /// <summary>
        /// Gets the IsClosed property value.
        /// </summary>
        /// <param name="area">The Chartarea.</param>
        /// <returns>The IsClosed</returns>
        public static bool GetIsClosed(ChartArea area)
        {
            return (bool)area.GetValue(IsClosedProperty);
        }

        /// <summary>
        /// Sets the IsClosed property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public static void SetIsClosed(ChartArea area, bool value)
        {
            area.SetValue(IsClosedProperty, value);
        }

        /// <summary>
        /// Gets the DrawType property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <returns>The Drawtype</returns>
        public static ChartPolarDrawType GetDrawType(ChartArea area)
        {
            return (ChartPolarDrawType)area.GetValue(DrawTypeProperty);
        }

        /// <summary>
        /// Sets the DrawType property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public static void SetDrawType(ChartArea area, ChartPolarDrawType value)
        {
            area.SetValue(DrawTypeProperty, value);
        }

        /// <summary>
        /// Gets the IsClockWise property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <returns>The SplineCoefficient</returns>
        public static bool GetIsClockWise(ChartArea area)
        {
            return (bool)area.GetValue(IsClockWiseProperty);
        }

        /// <summary>
        /// Sets the IsClockWise property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public static void SetIsClockWise(ChartArea area, bool value)
        {
            area.SetValue(IsClockWiseProperty, value);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                area.LoadArea();
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            ChartPolarDrawType drawtype = ChartPolarType.GetDrawType(series.Area);
            bool isclose = ChartPolarType.GetIsClosed(series.Area);
            SetRange(series, points, 1);
            ChartPointsCollection linepoints = new ChartPointsCollection();
            SegmentsCollection segmentsdata = new SegmentsCollection();
            if (points.Count != 0)
            {
                double[] datapoints = (from point in points where point.Visible == true select point.Y).ToArray<double>();
                if (datapoints.Length != 0d)
                {
                    series.sum = datapoints.Sum();
                }
            }

            linepoints.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                if (double.IsNaN(points[i].Y))
                    continue;
                double x1 = points[i].X + (series.XAxis.VisibleRange.Start * (-1));
                double y1 = points[i].Y + (series.YAxis.VisibleRange.Start * (-1));
                if (drawtype == ChartPolarDrawType.Symbol)
                {
                    PolarSegment pol = new PolarSegment(null, new ChartPoint(x1, y1), points, series, false);
                    series.Segments.Add(pol);
                }

                linepoints.Add(new ChartPoint(x1, y1));
                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible == true)
                {
                    segmentsdata.Add(new ChartAdornment(points[i], new ChartPoint(x1, y1), points, series, 0d));
                }
            }

            if (isclose && linepoints.Count > 0)
            {
                linepoints.Add(new ChartPoint(linepoints[0].X, linepoints[0].Y));
            }
            else if(!isclose && linepoints.Count>0 && drawtype== ChartPolarDrawType.Area)
            {
                linepoints.Add(new ChartPoint(0,0));
            }

            if (drawtype != ChartPolarDrawType.Symbol)
            {
                PolarSegment fastseg = new PolarSegment(linepoints, new ChartPoint(0, 0), points, series, true);
                series.Segments.Add(fastseg);
                if (drawtype == ChartPolarDrawType.Line)
                {
                    fastseg.SegInterior = fastseg.Interior;
                    fastseg.FillColor = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    fastseg.SegInterior = series.Stroke;
                    fastseg.FillColor = fastseg.Interior;
                    fastseg.StrokeThickness = series.StrokeThickness;
                }
            }

            foreach (Segment seg in segmentsdata)
            {
                series.Adornments.Add(seg);
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
            return "Polar";
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
