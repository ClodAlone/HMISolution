// <copyright file="ChartAreaType.cs" company="Syncfusion">
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
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Collections;
    using System.Globalization;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents chart area segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class ChartAreaSegment : ChartSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartAreaSegment), new PropertyMetadata(null));
        #endregion     

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartAreaSegment"/> class.
        /// </summary>
        static ChartAreaSegment()
        {
            Type type = typeof(ChartAreaSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAreaSegment"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        public ChartAreaSegment(IChartDataPoint[] points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            this.AreaPoints = points;

            xRange = DoubleRange.Empty;
            yRange = DoubleRange.Empty;

            foreach (IChartDataPoint cdpt in points)
            {
                if (cdpt != null)
                {
                    xRange += cdpt.X;
                    yRange += cdpt.Y;
                    if (this.Series.Area.EnableDepthAxis && cdpt.Values.Length > 1)
                        zRange += cdpt.Values[1];
                }
            }

            //if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
            //{
            //    xRange += xRange.Start - 0.5;
            //    xRange += xRange.End + 0.5;
            //}

            SetRange(series);
        }
        #endregion

        #region Properties

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
        /// Gets or sets the points array that area initially represents.
        /// </summary>
        /// <value>The area points.</value>
        protected IChartDataPoint[] AreaPoints
        {
            get;
            set;
        }

        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        /// <seealso cref="ChartAreaSegment"/>
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
            PathFigure figure = new PathFigure();

            if (this.AreaPoints.Length > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(this.AreaPoints[0].X, this.AreaPoints[0].Y);

                for (int i = 1; i < this.AreaPoints.Length; i++)
                {
                    if (this.AreaPoints[i] != null)
                    {
                        figure.Segments.Add(new LineSegment(transformer.TransformToVisible(this.AreaPoints[i].X, this.AreaPoints[i].Y), true));
                    }
                }

                figure.IsClosed = true;
            }

            this.Geometry = new PathGeometry(new PathFigure[] { figure });
        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            if (this.AreaPoints.Length > 2)
            {
                Point ex = transformer.TransformToVisible(this.AreaPoints[2].X, this.AreaPoints[2].Y);
                double sideA;
                double sideB;
                double sideC;
                GeometryModel3D model = new GeometryModel3D();

                if (Series.Type == ChartTypes.StackingArea)
                {
                    model.Geometry = MeshGenerator.AreaSegmentFull(this.AreaPoints, 0.05, transformer);

                    MaterialGroup materialGroup;

                    DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                    Binding binding = new Binding("Interior");
                    binding.Source = Series;
                    bool colorEachValue = (this.Series.ColorEach == null ? false : (bool)this.Series.ColorEach);
                    if (!colorEachValue)
                        BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                    else
                        this.Series.UpdateColorEachSegments(this.Series, this, this.Series.Segments.IndexOf(this), difuseMaterial);
                    materialGroup = new MaterialGroup();
                    materialGroup.Children.Add(difuseMaterial);

                    model.Material = materialGroup;
                    model.BackMaterial = materialGroup;

                    model.Transform = new TranslateTransform3D(-0.5, -0.5, 0.1);
                    Geometry3DGroup.Children.Add(model);
                }
                else
                {
                    for (int i = 3; i < this.AreaPoints.Length; i++)
                    {
                        Point point = transformer.TransformToVisible(this.AreaPoints[i].X, this.AreaPoints[i].Y);
                        sideA = point.X - ex.X;
                        sideB = 1 - ex.Y;
                        sideC = 1 - point.Y;

                        if (Series.XAxis.VisibleRange.Inside(this.AreaPoints[i].X))
                        {
                            model = new GeometryModel3D();
                            double seriesIndex = this.Series.Area.Series.IndexOf(Series);
                            model.Geometry = MeshGenerator.AreaSegment(sideA, sideB, sideC, 0.1, seriesIndex, 0.1 * Series.Area.Series.Count, Series.Area.isClustered);

                            MaterialGroup materialGroup;

                            DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                            Binding binding = new Binding("Interior");
                            binding.Source = Series;
                            BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                            materialGroup = new MaterialGroup();
                            materialGroup.Children.Add(difuseMaterial);

                            model.Material = materialGroup;
                            model.BackMaterial = materialGroup;

                            model.Transform = new TranslateTransform3D(ex.X + sideA / 2 - 0.5, -0.5, (this.Series.Area.Series.IndexOf(this.Series) + 2) * 0.05);
                            Geometry3DGroup.Children.Add(model);
                        }

                        ex = point;
                    }
                }
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (this.seriesCorrespondingPoints != null)
            {
                foreach (var item in this.seriesCorrespondingPoints)
                {
                    if (item.DataPoint is ChartPoint)
                    {
                        (item.DataPoint as ChartPoint).DisposePoint();
                    }
                }
            }
            SetValue(SeriesPropertyKey, null);
            base.Dispose();
        }
        #endregion
    }

    /// <summary>
    /// Represents the CahrtArea type
    /// </summary>
    /// <seealso cref="ChartAreaSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class ChartAreaType : ChartType
    {
        #region Properties
        /// <summary>
        /// Gets the flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region  Methods

        /// <summary>
        /// Converts ChartAreaType to string 
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartAreaType"/>
        public override string ToString()
        {
            return "Area";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            if (points.Length > 0 && series.Type != ChartTypes.Polar)
            {
                double origin = series.ActualXAxis.Origin;
                double origin1 = series.ActualYAxis.Origin;

                ObservableCollection<IChartDataPoint> areaPoints = new ObservableCollection<IChartDataPoint>();
                IChartDataPoint[] act_areapoints;
                bool isclose = ChartPolarType.GetIsClosed(series.Area);
                if (series.Type == ChartTypes.Polar)
                {
                    areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, points[points.Length - 1].DataPoint.Y));
                    areaPoints.Add(new ChartPoint(points[0].DataPoint.X, points[0].DataPoint.Y));


                    for (int i = 0; i < points.Length; i++)
                    {
                        areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));
                    }
                    if (isclose && points.Length > 0)
                    {
                        areaPoints.Add(new ChartPoint(points[0].DataPoint.X, points[0].DataPoint.Y));
                    }
                    // areaPoints.Add(new ChartPoint(points[0].DataPoint.X, points[0].DataPoint.Y));
                    act_areapoints = new ChartPoint[areaPoints.Count];
                    for (int j = 0; j < areaPoints.Count; j++)
                    {
                        act_areapoints[j] = areaPoints[j];
                    }
                }
                else
                {
                    areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, origin));
                    areaPoints.Add(new ChartPoint(points[0].DataPoint.X, origin));

                    int count = points.Length;
                    for (int i = 0; i < count; i++)
                    {
                        if (i < points.Length && points[i].DataPoint.EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                            {
                                areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));
                            }
                            else
                            {
                                if (i > 0)
                                {
                                    ChartPoint p = new ChartPoint();
                                    p.X = points[i - 1].DataPoint.X;
                                    p.Y = origin1;
                                    areaPoints.Add(p);

                                }
                                areaPoints.Add(new ChartPoint(points[i].DataPoint.X, origin1));

                                if (i < points.Length - 1)
                                {

                                    ChartPoint p1 = new ChartPoint();
                                    p1.X = points[i + 1].DataPoint.X;
                                    p1.Y = origin1;
                                    areaPoints.Add(p1);

                                }
                            }
                        }
                        else
                        {

                            areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));

                        }
                    }
                    act_areapoints = new ChartPoint[areaPoints.Count];
                    for (int j = 0; j < areaPoints.Count; j++)
                    {
                        act_areapoints[j] = areaPoints[j];
                    }

                }


                ChartAreaSegment seg = new ChartAreaSegment(act_areapoints, points, series);
                series.Segments.Add(seg);


                if (series.AdornmentsInfo.Visible)
                {
                    series.Adornments.Clear();
                    for (int i = 0; i < points.Length; i++)
                    {
                        if (points[i].DataPoint.Y < 0)
                            series.AdornmentsInfo.m_requiresSymmetricLabelling = true;
                         //Add for SD11357
                        if (points[i].DataPoint.EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                                series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                        }
                        else
                        {
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                        }
                    }
                }
            }
            else if(points.Length >0)
            {
                series.Area.m_isRadar = false;
                #region ChartPolarDrawType.Area
                if (ChartPolarType.GetDrawType(series.Area) == ChartPolarDrawType.Area)
                {
                    double origin = series.ActualXAxis.Origin;
                    double origin1 = series.ActualYAxis.Origin;

                    ObservableCollection<IChartDataPoint> areaPoints = new ObservableCollection<IChartDataPoint>();
                    IChartDataPoint[] act_areapoints;
                    bool isclose = ChartPolarType.GetIsClosed(series.Area);
                    if (series.Type == ChartTypes.Polar)
                    {
                        if (isclose)
                        {
                            areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, points[points.Length - 1].DataPoint.Y));
                        }
                        else
                            areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, origin1));                            
                        areaPoints.Add(new ChartPoint(points[0].DataPoint.X, points[0].DataPoint.Y));


                        for (int i = 0; i < points.Length; i++)
                        {
                            areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));
                        }
                        if (!isclose && points.Length > 0)
                        {
                            areaPoints.Add(new ChartPoint(points[0].DataPoint.X, origin1));
                        }
                        // areaPoints.Add(new ChartPoint(points[0].DataPoint.X, points[0].DataPoint.Y));
                        act_areapoints = new ChartPoint[areaPoints.Count];
                        for (int j = 0; j < areaPoints.Count; j++)
                        {
                            act_areapoints[j] = areaPoints[j];
                        }
                    }
                    else
                    {
                        areaPoints.Add(new ChartPoint(points[points.Length - 1].DataPoint.X, origin));
                        areaPoints.Add(new ChartPoint(points[0].DataPoint.X, origin));

                        int count = points.Length;
                        for (int i = 0; i < count; i++)
                        {
                            if (i < points.Length && points[i].DataPoint.EmptyPoint)
                            {
                                if (series.ShowEmptyPoints)
                                {
                                    areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));
                                }
                                else
                                {
                                    if (i > 0)
                                    {
                                        ChartPoint p = new ChartPoint();
                                        p.X = points[i - 1].DataPoint.X;
                                        p.Y = origin1;
                                        areaPoints.Add(p);

                                    }
                                    areaPoints.Add(new ChartPoint(points[i].DataPoint.X, origin1));

                                    if (i < points.Length - 1)
                                    {

                                        ChartPoint p1 = new ChartPoint();
                                        p1.X = points[i + 1].DataPoint.X;
                                        p1.Y = origin1;
                                        areaPoints.Add(p1);

                                    }
                                }
                            }
                            else
                            {

                                areaPoints.Add(new ChartPoint(points[i].DataPoint.X, points[i].DataPoint.Y));

                            }
                        }
                        act_areapoints = new ChartPoint[areaPoints.Count];
                        for (int j = 0; j < areaPoints.Count; j++)
                        {
                            act_areapoints[j] = areaPoints[j];
                        }

                    }


                    ChartAreaSegment seg = new ChartAreaSegment(act_areapoints, points, series);
                    series.Segments.Add(seg);


                    if (series.AdornmentsInfo.Visible)
                    {
                        series.Adornments.Clear();
                        for (int i = 0; i < points.Length; i++)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                        }
                    }
                }
                #endregion
                #region ChartPolarDrawType.Line
                else if (ChartPolarType.GetDrawType(series.Area) == ChartPolarDrawType.Line)
                {
                    bool dotSegmentRequired = ChartLineType.GetBreakLineForNonIndexedData(series) && !series.IsIndexed && !series.Area.View3DMode;
                    double gapCount = 0;
                    if (dotSegmentRequired)
                    {
                        if (series.ActualXAxis.ValueType == ChartValueType.Double)
                        {
                            gapCount = ChartLineType.GetBreakLineForDoublePointsDistanceMoreThan(series);
                        }
                        else
                        {
                            gapCount = (DateTime.Now + (TimeSpan)ChartLineType.GetBreakLineForTimeSpanPointsDistanceMoreThan(series)).ToOADate() - DateTime.Now.ToOADate();
                        }
                    }

                    dotSegmentRequired = dotSegmentRequired && (gapCount > 0);
                    for (int i = 0; i < points.Length; i++)
                    {
                        ////If Emptypoint, make a difference in segment rendering
                        if (points[i].DataPoint.EmptyPoint || (i + 1 > points.Length && points[i + 1].DataPoint.EmptyPoint))
                        {
                            if (series.ShowEmptyPoints && points[i].DataPoint.EmptyPoint)
                            {
                                if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                                {
                                    if (series.Segments.Count > 0)
                                    {
                                        if (series.Segments[series.Segments.Count - 1].GetType() == typeof(ChartLineSegment))
                                        {
                                            series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                                        }
                                    }

                                    series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                                }
                                else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                                {
                                    if (i + 1 < points.Length)
                                    {
                                        series.Segments.Add(new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series));
                                    }

                                    if (i > 0 && i < points.Length)
                                    {
                                        series.Segments[i - 1].Interior = series.EmptyPointInterior;
                                        if (i != series.Segments.Count)
                                            series.Segments[i].Interior = series.EmptyPointInterior;
                                    }
                                }
                                else
                                {
                                    if (series.Segments.Count > 0)
                                    {
                                        if (series.Segments[series.Segments.Count - 1].GetType() == typeof(ChartLineSegment))
                                        {
                                            series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;

                                        }
                                    }

                                    ChartEmptySymbolSegment segment = new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate);
                                    series.Segments.Add(segment);
                                }
                            }
                            else
                            {
                                if (i + 1 < points.Length)
                                {
                                    ChartLineSegment segment = new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series);
                                    segment.Interior = Brushes.Transparent;
                                    series.Segments.Add(segment);
                                }
                            }
                        }
                        else
                        {
                            if (i + 1 < points.Length)
                            {
                                series.Segments.Add(new ChartLineSegment(points[i].DataPoint, points[i + 1].DataPoint, points[i], points[i + 1], series));
                                if (points[i + 1].DataPoint.EmptyPoint && !series.ShowEmptyPoints)
                                {
                                    series.Segments[series.Segments.Count - 1].Interior = Brushes.Transparent;
                                }
                            }

                            if (dotSegmentRequired && i + 2 <= points.Length)
                            {
                                if (points[i + 1].DataPoint.X - points[i].DataPoint.X > gapCount)
                                {
                                    if (points.Length > i + 2 && points[i + 2].DataPoint.X - points[i + 1].DataPoint.X > gapCount)
                                    {
                                        ChartIndexedDataPoint indexedPoint2 = points[i + 1];
                                    }
                                }
                            }
                        }

                    }
                    bool isclose = ChartPolarType.GetIsClosed(series.Area);
                    if (isclose && points.Length > 0 && series.Type == ChartTypes.Polar)
                        series.Segments.Add(new ChartLineSegment(points[points.Length - 1].DataPoint, points[0].DataPoint, points[points.Length - 1], points[0], series));
                    if (series.AdornmentsInfo.Visible)
                    {
                        series.Adornments.Clear();
                        for (int i = 0; i < points.Length; i++)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                        }
                    }
                }
                #endregion
                #region ChartPolarDrawType.Symbol
                else
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        //Since ChartEmptySymbolSegment is similar implementation for drawing symbol, so here ChartEmptySymbolSegment class is used for DrawType "Symbol"
                        series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series,ChartPolarType.GetPolarSymbol(series)));


                    }


                    if (series.AdornmentsInfo.Visible)
                    {
                        series.Adornments.Clear();
                        for (int i = 0; i < points.Length; i++)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                        }
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The Chart Series</param>
        /// <param name="points">The series points</param>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }
        #endregion
    }
}
