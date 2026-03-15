// <copyright file="ChartPointAndFigureType.cs" company="Syncfusion">
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
    using System.Windows;
    using System.Windows.Shapes;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Linq;

  
    /// <summary>
    /// Represents Point and Figure chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartPointAndFigureType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartPointAndFigureSegment : ChartSegment
    {
        #region Members

/*
        /// <summary>
        /// Declares m_topRightPoint
        /// </summary>
        private IChartDataPoint m_topRightPoint;
*/

        /// <summary>
        /// Declares m_figurecost
        /// </summary>
        private double m_figurecost;
        
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the Shape dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("Shape", typeof(ChartPointAndFigure), typeof(ChartPointAndFigureSegment));

        /// <summary>
        /// Identifies the StrokeColor dependency property.
        /// </summary>
        internal static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register("StrokeColor", typeof(Brush), typeof(ChartPointAndFigureSegment));

        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartPointAndFigureSegment), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartPointAndFigureSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartPointAndFigureSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartPointAndFigureSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartPointAndFigureSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X co-ordinate of segment.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the StrokeColor of segment.
        /// </summary>
        /// <value>The StrokeColor value.</value>
        internal Brush StrokeColor
        {
            get { return (Brush)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the segment geometry. This is a dependency property.
        /// </summary>
        /// <value>The <see cref="Geometry"/>.</value>
        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate of segment.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of segment.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of segment.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the shape.
        /// </summary>
        /// <value>The shape.</value>
        public ChartPointAndFigure Shape
        {
            get { return (ChartPointAndFigure)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartPointAndFigureSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default segment template is assigned automatically.
        /// </remarks>
        static ChartPointAndFigureSegment()
        {
            Type type = typeof(ChartPointAndFigureSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }
        /// <summary>
        /// Get or Set PointColl1 property
        /// </summary>
        private IChartDataPoint[] PointColl1
        {
            get;
            set;
        }
        /// <summary>
        /// Get or Set PointColl2 property
        /// </summary>
        private IChartDataPoint[] PointColl2
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointAndFigureSegment"/> class.
        /// </summary>
        /// <param name="pt1">The bottom left PNT.</param>
        /// <param name="pt2">The top right PNT.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        /// <param name="figcost"></param>
        internal ChartPointAndFigureSegment(IChartDataPoint[] pt1, IChartDataPoint[] pt2, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series, double figcost)
            : base(series, correspondingPoints)
        {
            this.PointColl1 = pt1;
            this.PointColl2 = pt2;
            this.m_figurecost = figcost;
            double X_MAX = (pt2.ToList()).Max(x => x.X);
            double Y_MAX = (pt2.ToList()).Max(y => y.Y);
            double X_MIN = (pt1.ToList()).Min(x => x.X);
            double Y_MIN = (pt1.ToList()).Min(y => y.Y);           

            this.SetXRange(X_MIN, X_MAX);
            this.SetYRange(Y_MIN, Y_MAX);
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {

            if (this.Interior != null && this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }
            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }
            base.Update(transformer);
            Point blPoint = transformer.TransformToVisible(this.PointColl1[0].X, this.PointColl1[0].Y);
            Point trPoint = transformer.TransformToVisible(this.PointColl2[this.PointColl2.Length - 1].X, this.PointColl2[this.PointColl2.Length - 1].Y);
            Rect columnRect = new Rect(blPoint, trPoint);

            List<PathFigure> figurecoll = new List<PathFigure>();
            GeometryGroup ellipses = new GeometryGroup();
            double each_ht = columnRect.Height / PointColl1.Count(); 
            if (this.PointColl1.Length > 0)
            {
                double figcost = this.m_figurecost;
                for (int i = 0; i < this.PointColl1.Length; i++)
                {
                    List<Point> pointcoll= new List<Point> ();

                    if (this.PointColl1[i] != null && this.Shape== ChartPointAndFigure.Figure)
                    {
                        PathFigure figure = new PathFigure();
                        Point actualpt = transformer.TransformToVisible(this.PointColl1[i].X, this.PointColl1[i].Y);
                        Point linept = transformer.TransformToVisible(this.PointColl2[i].X, this.PointColl2[i].Y);
                        figure.StartPoint = actualpt;
                        figure.Segments.Add(new LineSegment(linept, true));                        
                        figure.IsClosed = false;
                        
                        figurecoll.Add(figure); 
                        
                        PathFigure figure1 = new PathFigure();
                        Point actualpt1 = transformer.TransformToVisible(this.PointColl2[i].X, this.PointColl1[i].Y);
                        Point linept1 = transformer.TransformToVisible(this.PointColl1[i].X, this.PointColl2[i].Y);
                        figure1.StartPoint = actualpt1;
                        figure1.Segments.Add(new LineSegment(linept1, true));
                        figure1.IsClosed = false;
                        figurecoll.Add(figure1);                        
                    }
                    else if (this.PointColl1[i] != null && this.Shape== ChartPointAndFigure.Point)
                    {                                            
                        Point actualpt = transformer.TransformToVisible(this.PointColl1[i].X, this.PointColl1[i].Y);
                        ellipses.Children.Add(new EllipseGeometry(actualpt, (this.PointColl2[i].X - this.PointColl1[i].X) / 2, each_ht/2));                        
                    }
                }                
                
            }
            if (this.Shape == ChartPointAndFigure.Point)
            {
                this.StrokeColor = Brushes.Red;
                this.Geometry = ellipses;
            }
            else
            {
                this.StrokeColor = Brushes.Green;
                this.Geometry = new PathGeometry(figurecoll.ToArray());
            }
           
            this.X = columnRect.X;
            this.Y = columnRect.Y;
            this.Width = columnRect.Width;
            this.Height = columnRect.Height;
        }
        #endregion
    }

    /// <summary>
    /// Represents ChartPointAndFigureType class
    /// </summary>
    /// <remarks>
    /// Point and Figure Chart is used to identify support levels, resistance levels and
    /// chart patterns. The chart ignores the time factor and concentrates solely on
    /// movements in price - a column of Xs or Os may take one day or several weeks to
    /// complete. By convention, the first X in a column is plotted one box above the
    /// last O in the previous column (and the first O in a column is plotted one box
    /// below the highest X).
    /// </remarks>
    /// <seealso cref="ChartPointAndFigureSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartPointAndFigureType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the FigureCost dependency property.
        /// </summary>
        public static readonly DependencyProperty FigureCostProperty =
      DependencyProperty.RegisterAttached("FigureCost", typeof(double), typeof(ChartPointAndFigureType), new ChartPropertyMetadata(1d, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Identifies the ReversalAmount dependency property.
        /// </summary>
        public static readonly DependencyProperty ReversalAmountProperty =
      DependencyProperty.RegisterAttached("ReversalAmount", typeof(double), typeof(ChartPointAndFigureType), new ChartPropertyMetadata(1d, new PropertyChangedCallback(OnReverseAmountChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Identifies the StartFrom dependency property.
        /// </summary>
        public static readonly DependencyProperty StartFromProperty =
      DependencyProperty.RegisterAttached("StartFrom", typeof(ChartPointAndFigure), typeof(ChartPointAndFigureType), new ChartPropertyMetadata(ChartPointAndFigure.Point, new PropertyChangedCallback(onStartFromChanged), ChartPropertyMetadataOptions.AffectsUpdate));
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointAndFigureType"/> class.
        /// </summary>
        internal ChartPointAndFigureType()
        {
        }

        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.CustomAxisLabels | ChartTypeFlags.Indexed;
            }
        }

        /// <summary>
        /// Gets the require data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 2;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the starting series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The start from value</returns>
        public static ChartPointAndFigure GetStartFrom(ChartSeries series)
        {
            return (ChartPointAndFigure)series.GetValue(StartFromProperty);
        }

        /// <summary>
        /// Sets the starting series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetStartFrom(ChartSeries series, ChartPointAndFigure value)
        {
            series.SetValue(StartFromProperty, value);
        }

        /// <summary>
        /// Gets the reversal amount.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Returns the reversal amount</returns>
        public static double GetReversalAmount(ChartSeries series)
        {
            return (double)series.GetValue(ReversalAmountProperty);
        }

        /// <summary>
        /// Sets the reversal amount.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetReversalAmount(ChartSeries series, double value)
        {
            series.SetValue(ReversalAmountProperty, value);
        }

        /// <summary>
        /// Gets the figure cost.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Returns the figure cost</returns>
        public static double GetFigureCost(ChartSeries series)
        {
            return (double)series.GetValue(FigureCostProperty);
        }

        /// <summary>
        /// Sets the figure cost.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetFigureCost(ChartSeries series, double value)
        {
            series.SetValue(FigureCostProperty, value);
        }

        /// <summary>
        /// Determines whether the this type is compatible with specified type.
        /// </summary>
        /// <param name="type">The type value.</param>
        /// <returns>
        /// <c>true</c> if the type is compatible; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsCompatible(ChartType type)
        {
            return false;
        }

        /// <summary>
        /// Called when StartFrom property changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void onStartFromChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries ser = obj as ChartSeries;
            if (ser != null)
            {
                if (ser.ChartType is ChartPointAndFigureType)
                {
                    ((ChartPointAndFigureType)(ser.ChartType)).Update(ser);
                }
            }
        }

        /// <summary>
        /// Called when ReversalAmount property changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnReverseAmountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries ser = obj as ChartSeries;
            if (ser != null)
            {
                if (ser.ChartType is ChartPointAndFigureType)
                {
                    ((ChartPointAndFigureType)(ser.ChartType)).Update(ser);
                }
            }
        }


        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        public override void Calculate(ChartSeries series)
        {
            double figureCost = GetFigureCost(series);
            double reversalAmount = GetReversalAmount(series);
            int count = series.PointsCount;
            List<ChartSegment> drawingList = new List<ChartSegment>(count);
            ChartIndexedDataPoint[] cdpwiA = new ChartIndexedDataPoint[count];

            for (int i = 0; i < count; i++)
            {
                cdpwiA[i] = new ChartIndexedDataPoint(series.GetPoint(i), i);
            }

            Array.Sort(cdpwiA, new ChartIndexedDataPointByXComparer());

            double currX = 0;
            double currLow = double.NaN;
            double currHigh = double.NaN;
            ChartPointAndFigure currShape = GetStartFrom(series);
            List<ChartIndexedDataPoint> correspondingPoints = new List<ChartIndexedDataPoint>();

            for (int i = 0; i < cdpwiA.Length; i++)
            {
                IChartDataPoint dataPoint = cdpwiA[i].DataPoint;

                double low = Math.Min(dataPoint.Values[0], dataPoint.Values[1]);
                double high = Math.Max(dataPoint.Values[0], dataPoint.Values[1]);

                low = ChartMath.Round(low, figureCost, false);
                high = ChartMath.Round(high, figureCost, true);

                if (i == 0)
                {
                    currLow = low;
                    currHigh = high;
                }
                else if (currShape == ChartPointAndFigure.Point)
                {
                    double minLow = Math.Min(currLow, low);

                    if (high >= currLow + reversalAmount)
                    {
                        drawingList.Add(GenerateSegments(currX, minLow, currHigh, correspondingPoints.ToArray(), currShape, series, figureCost));

                        correspondingPoints.Clear();
                        currShape = ChartPointAndFigure.Figure;
                        currLow = minLow + figureCost;
                        currHigh = high;
                        currX++;
                    }
                    else
                    {
                        currLow = minLow;
                    }
                }
                else
                {
                    double maxHigh = Math.Max(currHigh, high);

                    if (low <= currHigh - reversalAmount)
                    {
                        drawingList.Add(GenerateSegments(currX, currLow, maxHigh, correspondingPoints.ToArray(), currShape, series, figureCost));

                        correspondingPoints.Clear();
                        currShape = ChartPointAndFigure.Point;
                        currLow = low;
                        currHigh = maxHigh - figureCost;
                        currX++;
                    }
                    else
                    {
                        currHigh = maxHigh;
                    }
                }

                correspondingPoints.Add(cdpwiA[i]);
            }

            if (correspondingPoints.Count != 0)
            {
                drawingList.Add(GenerateSegments(currX, currLow, currHigh, correspondingPoints.ToArray(), currShape, series, figureCost));
            }

            drawingList.Reverse();

            foreach (ChartSegment segment in drawingList)
            {
                series.Segments.Add(segment);
            }
        }

        /// <summary>
        /// Updates chart.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        /// <seealso cref="ChartPointAndFigureType"/>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }
        #endregion

        #region Implementaion
        /// <summary>
        /// Generates the segments.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Ending value.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="shape">The shape value.</param>
        /// <param name="series">The series.</param>
        /// <param name="delta">The delta.</param>
        /// <returns>Returns the ChartSegments</returns>
        private static ChartSegment GenerateSegments(double x, double from, double to, ChartIndexedDataPoint[] correspondingPoints, ChartPointAndFigure shape, ChartSeries series, double delta)
        {
            int count = (int)Math.Round((to - from) / delta);

            IChartDataPoint[] ptcoll1 = new IChartDataPoint[count+1];
            IChartDataPoint[] ptcoll2 = new IChartDataPoint[count+1];
            for (int i = 0; i <= count; i++)
            {
                ChartPoint pt1 = new ChartPoint(x - 0.5d, from + i * delta);
                ChartPoint pt2 = new ChartPoint(x + 0.5d, from + (i + 1) * delta);
                ptcoll1[i] = pt1;
                ptcoll2[i] = pt2;
                
            }
            ChartPointAndFigureSegment segment = new ChartPointAndFigureSegment(ptcoll1, ptcoll2, correspondingPoints, series,delta);

            segment.AxisLabelInfo = new ChartAxisLabelInfo(x, correspondingPoints[0].DataPoint.X);
            segment.Shape = shape;            

            return segment;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartPointAndFigureType"/>
        public override string ToString()
        {
            return "PointAndFigure";
        }
        #endregion
    }
}
