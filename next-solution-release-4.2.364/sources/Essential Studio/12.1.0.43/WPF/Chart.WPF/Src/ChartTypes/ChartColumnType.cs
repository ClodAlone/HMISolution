// <copyright file="ChartColumnType.cs" company="Syncfusion">
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
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Windows.Markup;
    using System.Collections;
    using System.Globalization;

    /// <summary>
    /// Represents column chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartColumnSegment : ChartSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the LayerBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LayerBrushProperty =
             DependencyProperty.Register("LayerBrush", typeof(Brush), typeof(ChartColumnSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartColumnSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartColumnSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartColumnSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartColumnSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X co-ordinate of segment. This is a dependency property.
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
        /// Gets or sets the Y co-ordinate of segment. This is a dependency property.
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

        /// <summary>
        /// Gets or sets the width of segment. This is a dependency property.
        /// </summary>
        /// <value>The width value.</value>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }

            set
            {
                SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of segment. This is a dependency property.
        /// </summary>
        /// <value>The height value.</value>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }

            set
            {
                SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Layer brush. This is a dependency property.
        /// </summary>
        /// <value>The Brush value.</value>
        public Brush LayerBrush
        {
            get { return (Brush)GetValue(LayerBrushProperty); }
            set { SetValue(LayerBrushProperty, value); }
        }
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_bottomLeftPoint
        /// </summary>
        private IChartDataPoint m_bottomLeftPoint;

        /// <summary>
        /// Initializes m_topRightPoint
        /// </summary>
        private IChartDataPoint m_topRightPoint;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartColumnSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default segment template is assigned automatically.
        /// </remarks>
        static ChartColumnSegment()
        {
            Type type = typeof(ChartColumnSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartColumnSegment"/> class.
        /// </summary>
        /// <param name="bottomLeftPnt">The bottom left PNT.</param>
        /// <param name="topRightPnt">The top right PNT.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        public ChartColumnSegment(IChartDataPoint bottomLeftPnt, IChartDataPoint topRightPnt, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
            m_bottomLeftPoint = bottomLeftPnt;
            m_topRightPoint = topRightPnt;
            if (this.Series.Area.EnableDepthAxis)
                m_bottomLeftPoint.Values = correspondingPoint.DataPoint.Values;
            //separate behavior for indexed axis
            if (series.ActualXAxis.Indexed == true)
            {
                if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes || series.ActualXAxis.IsLogarithmic==true)
                {
                    this.SetXRange(bottomLeftPnt.X, topRightPnt.X);
                }
                else
                {
                    this.SetXRange(correspondingPoint.DataPoint.X);
                }
            }
                //non indexed axis
            else
            {
                if (series.ActualXAxis.RangePadding == ChartRangePaddingType.None)
                {
                    this.SetXRange(correspondingPoint.DataPoint.X);
                }
                else if (series.ActualXAxis.RangePadding == ChartRangePaddingType.Normal)
                {
                    this.SetXRange(bottomLeftPnt.X, topRightPnt.X);
                }
                else
                {
                    this.SetXRange(bottomLeftPnt.X, topRightPnt.X);
                }
            }
            if (this.Series != null && this.Series.EnableEffects == true)
            {
                if (this.Series.Type == ChartTypes.Bar || this.Series.Type == ChartTypes.StackingBar || this.Series.Type == ChartTypes.Tornado || this.Series.Type == ChartTypes.Gantt || this.Series.Type == ChartTypes.StackingBar100)
                {
                    this.LayerBrush = XamlReader.Parse("<LinearGradientBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' EndPoint='0.5,1.58' StartPoint='0.5,-0.436'><GradientStop Color='#26675A5A' Offset='0.33'/><GradientStop Color='#26675A5A' Offset='0.601'/><GradientStop Color='#33B3ACAC' Offset='0.3'/><GradientStop Color='#33B3ACAC' Offset='0.635'/><GradientStop Color='#25282323' Offset='0.47'/></LinearGradientBrush>") as Brush;
                }
                else
                {
                    this.LayerBrush = XamlReader.Parse("<LinearGradientBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' EndPoint='1.58,0.5' StartPoint='-0.436,0.5'><GradientStop Color='#26675A5A' Offset='0.275'/><GradientStop Color='#26675A5A' Offset='0.652'/><GradientStop Color='#33B3ACAC' Offset='0.252'/><GradientStop Color='#33B3ACAC' Offset='0.672'/><GradientStop Color='#25282323' Offset='0.47'/></LinearGradientBrush>") as Brush;
                }
            }
            this.SetYRange(bottomLeftPnt.Y, topRightPnt.Y);
            if (this.Series.Area.EnableDepthAxis && correspondingPoint.DataPoint.Values.Length > 1)
                this.SetZRange(correspondingPoint.DataPoint.Values[1]);
            SetRange(series);
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        /// <seealso cref="ChartColumnSegment"/>
        public override void Update(IChartTransformer transformer)
        {
            if (this.Interior != null && this.Interior.CanFreeze && this.Series.ShowEmptyPoints == false)
            {
               this.Interior.Freeze();
            }
            if (this.Stroke != null && this.Stroke.CanFreeze && this.Series.ShowEmptyPoints == false)
            {
               this.Stroke.Freeze();
            }
            base.Update(transformer);
            Point blpoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y);
            Point trpoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y);
            Rect columnRect = new Rect(blpoint, trpoint);
            this.X = columnRect.X;
            this.Y = columnRect.Y;
            this.Width = columnRect.Width;
            this.Height = columnRect.Height;
        }

        /// <summary>
        /// Draws the 3D segment.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            base.Draw3DSegment(transformer);
            DoubleRange axisRange = Series.XAxis.VisibleRange;
            double zindex = (this.Series.Area.EnableDepthAxis && m_bottomLeftPoint.Values.Length > 1) ? m_bottomLeftPoint.Values[1] : 0.2;
            if (this.Series.Type == ChartTypes.Candle)
                zindex = m_bottomLeftPoint.Values.Length > 4 ? m_bottomLeftPoint.Values[4] : 0.2;
            ////Skipping the segment if it falls out of axis' range.
            if (xRange.Start >= axisRange.Start && xRange.End <= axisRange.End)
            {
                Point3D blPoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y, zindex);
                blPoint = new Point3D(blPoint.X, blPoint.Y, this.Series.Area.EnableDepthAxis==true?(1 - blPoint.Z):0.2);
                Point3D trPoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y, zindex);
                trPoint = new Point3D(trPoint.X, trPoint.Y, this.Series.Area.EnableDepthAxis == true ? (1 - blPoint.Z) : 0.2);
                Rect columnRect = new Rect(new Point(blPoint.X,blPoint.Y), new Point(trPoint.X,trPoint.Y));
                double seriesIndex = this.Series.Area.Series.IndexOf(this.Series);
                double deep = (this.Series.Type == ChartTypes.Tornado) ? .04 : .1 ;
                Point3D p = this.Series.Area.CameraController.Camera.Position;
                GeometryModel3D geometryModel = new GeometryModel3D();                
                if (Series.Area.isClustered || Series.isLastSeries)
                {
                    geometryModel.Geometry = MeshGenerator.ColumnParallelotope(columnRect.Width, columnRect.Height, deep);                   
                }
                else
                {
                    geometryModel.Geometry = MeshGenerator.ColumnParallelotope(columnRect.Width, columnRect.Height, (deep * this.Series.Area.Series.Count), seriesIndex);
                }

              
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
                geometryModel.Material = materialGroup;

                if (this.Series.Type == ChartTypes.Tornado)
                {
                    geometryModel.Transform = new TranslateTransform3D(columnRect.X + columnRect.Width / 2 - 0.5, 0.5 - columnRect.Y - columnRect.Height / 2, (this.Series.Area.Series.IndexOf(this.Series) + 2) * 0.05);
                }
                else
                {
                    geometryModel.Transform = new TranslateTransform3D(columnRect.X + columnRect.Width / 2 - 0.5, 0.5 - columnRect.Y - columnRect.Height / 2, !this.Series.Area.IsClustered ? 0.06 : blPoint.Z);

                }
                Geometry3DGroup.Children.Add(geometryModel);
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

            this.m_bottomLeftPoint = null;
            this.m_topRightPoint = null;
            this.Item = null;
            this.seriesCorrespondingPoints = null;

        }


        #endregion
    }
    /// <summary>
    /// Represents ChartColumnType class
    /// </summary>
    /// <remarks>
    /// Column Charts are among the most common chart types that are being used. It uses
    /// vertical bars (called columns) to display different values of one or more items.
    /// It is similar to a bar chart except that here the bars are vertical and not
    /// horizontal. Points from adjacent series are drawn as bars next to each other.  
    /// <para></para>
    /// <para>It is used for comparing the frequency, count, total or average of data in
    /// different categories. It is ideal for showing the variations in the value of an
    /// item over time.</para>
    /// </remarks>
    /// <seealso cref="ChartColumnSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartColumnType : ChartType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.SideBySide | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts ChartColumnType to string 
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartColumnType"/>
        public override string ToString()
        {
            return "Column";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            double origin0 = series.ActualXAxis.Origin;
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);

            for (int i = 0; i < points.Length; i++)
            {

                double x1 =0 , x2 = 0, y1 = 0, y2 = 0;
                if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Fixed)
                {
                    x1 = points[i].DataPoint.X + sbsInfo.Start;
                    x2 = points[i].DataPoint.X + sbsInfo.End;
                }
                else if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative)
                {
                    if (!series.IsIndexed)
                    {
                        var relativePos = points[i].DataPoint.Values.Length > 1 ? points[i].DataPoint.Values[1] * 0.5 : 0;
                        x1 = points[i].DataPoint.X - relativePos;
                        x2 = points[i].DataPoint.X + relativePos;
                    }
                    else
                    {
                        var relativePos = points[i].DataPoint.Values.Length > 1 ? points[i].DataPoint.Values[1] * 0.5: 0;
                        x1 = points[i].DataPoint.X + sbsInfo.Start;
                        x2 = points[i].DataPoint.X + sbsInfo.End;
                    }
                }
                y1 = points[i].DataPoint.Y;
                y2 = origin0;
                
                ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                ChartPoint cdpRightTop = new ChartPoint(x2, y2);

                ////If Emptypoint, make a difference in segment rendering
                
                if (points[i].DataPoint.EmptyPoint)
                {
                    double centerpoint1 = x1 + ((x2 - x1) / 2);
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments.Add(new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                        else
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                    }
                    else
                    {
                       
                            series.Segments.Add(new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            series.Segments[i].Interior = new SolidColorBrush(Colors.Transparent);
                            series.Segments[i].Stroke = new SolidColorBrush(Colors.Transparent);
                       
                    }
                }
                else
                {
                    series.Segments.Add(new ChartColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                }

                if(i==0)
                  ChartType.SetColumnInitialSegmentWidthValue(series, x1);
               if(i==points.Length-1)
                  ChartType.SetEndSegmentWidth(series, x2);
            }
            
            if (series.AdornmentsInfo !=null && series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length; i++)
                {
                    double y1 = points[i].DataPoint.Y;
                    double y2 = origin0;
                    if (y1 < 0)
                        series.AdornmentsInfo.m_requiresSymmetricLabelling = true;
                    DoubleRange yRange = new DoubleRange(y1, y2);
                    double bottomPoint = (Math.Abs(yRange.Start) < Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                    double topPoint = (Math.Abs(yRange.Start) > Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                    double midPoint = yRange.Median;
                     //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, bottomPoint), points[i], series, yRange));
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, topPoint), points[i], series, yRange));
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, midPoint), points[i], series, yRange));
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, bottomPoint), points[i], series, yRange));
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, topPoint), points[i], series, yRange));
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, midPoint), points[i], series, yRange));
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartColumnType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            double origin0 = series.ActualXAxis.Origin;
            double x1 = 0, x2 =0, y1, y2;
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);

            ////Removing last segments.
            while (series.Segments.Count > points.Length)
            {
                series.Segments.RemoveAt(series.Segments.Count - 1);
            }

            ////Synchronizing existing segments.
            for (int i = 0; i < series.Segments.Count; i++)
            {
                IChartDataPoint segmentPoint = series.Segments[i].CorrespondingPoints[0].DataPoint;
                if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Fixed)
                {
                    x1 = points[i].DataPoint.X + sbsInfo.Start;
                    x2 = points[i].DataPoint.X + sbsInfo.End;
                }
                else if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative)
                {
                    if (!series.IsIndexed)
                    {
                        var relativePos = points[i].DataPoint.Values.Length > 1 ? points[i].DataPoint.Values[1] * 0.5 : 0;
                        x1 = points[i].DataPoint.X - relativePos;
                        x2 = points[i].DataPoint.X + relativePos;
                    }
                    else
                    {
                        x1 = points[i].DataPoint.X + sbsInfo.Start;
                        x2 = points[i].DataPoint.X + sbsInfo.End;
                    }
                }
                y1 = points[i].DataPoint.Y;
                y2 = origin0;

                ////If Emptypoint, make a difference in segment rendering
                if (points[i].DataPoint.EmptyPoint)
                {
                   // double centerpoint1 = x1 + ((x2 - x1) / 2);
                    double centerpoint1 = x1 + ((x2 - x1) / 2);
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments[i] = new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate);
                            series.Segments[i].Interior = series.Interior;
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments[i] = new ChartColumnSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[i], series);
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                        else
                        {
                            series.Segments[i] = new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate);
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                    }
                    else
                    {
                        if (series.Segments[i] != null)
                        {
                            series.Segments[i] = new ChartColumnSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[i], series);
                            series.Segments[i].Interior = new SolidColorBrush(Colors.Transparent);
                            series.Segments[i].Stroke = new SolidColorBrush(Colors.Transparent);
                        }
                    }
                }

                else if (segmentPoint.X != points[i].DataPoint.X || segmentPoint.Y != points[i].DataPoint.Y)
                {
                    series.Segments[i] = new ChartColumnSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[i], series);
                }
                else if ((segmentPoint.Item != points[i].DataPoint.Item || series.Area.View3DMode||!series.Area.clusterd) && !segmentPoint.EmptyPoint)
                {
                    series.Segments[i] = new ChartColumnSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[i], series);                    
                }
                x1 = x2 = 0;
            }
            series.Area.clusterd = false;
            ////Adding missing segments.
            int counter = 0;
            int segmentsPosition = series.Segments.Count;
            while (series.Segments.Count < points.Length)
            {
                x1 = points[segmentsPosition + counter].DataPoint.X + sbsInfo.Start;
                x2 = points[segmentsPosition + counter].DataPoint.X + sbsInfo.End;
                y1 = points[segmentsPosition + counter].DataPoint.Y;
                y2 = origin0;

                series.Segments.Add(new ChartColumnSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[segmentsPosition + counter], series));
                counter++;
            }
            if (series.AdornmentsInfo!= null && series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length; i++)
                {
                    double _y1 = points[i].DataPoint.Y;
                    double _y2 = origin0;
                    if (_y1 < 0)
                        series.AdornmentsInfo.m_requiresSymmetricLabelling = true;
                    DoubleRange yRange = new DoubleRange(_y1, _y2);
                    double bottomPoint = (Math.Abs(yRange.Start) < Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                    double topPoint = (Math.Abs(yRange.Start) > Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                    double midPoint = yRange.Median;
                    //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, bottomPoint), points[i], series, yRange));
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, topPoint), points[i], series, yRange));
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                                series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, midPoint), points[i], series, yRange));
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, bottomPoint), points[i], series, yRange));
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, topPoint), points[i], series, yRange));
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, midPoint), points[i], series, yRange));
                    }
                }
            }
        }

        #endregion
    }
}
