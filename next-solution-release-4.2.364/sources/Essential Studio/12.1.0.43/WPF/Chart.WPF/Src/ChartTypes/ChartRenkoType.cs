// <copyright file="ChartRenkoType.cs" company="Syncfusion">
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
    using System.Windows.Controls;

    /// <summary>
    /// Represents chart renko segment.
    /// </summary>
    /// <remarks>
    /// Single renko segment is created for 
    /// </remarks>
    /// <seealso cref="ChartRenkoType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartRenkoSegment : ChartSegment
    {
        #region Members
        /// <summary>
        /// Initializes m_transformedRectangles
        /// </summary>
        private List<Rect> m_transformedRectangles = new List<Rect>();
       
        /// <summary>
        /// Holds represented rectangles array that are used to build the segment.
        /// </summary>
        private Rect[] m_representedRectangles;
    
        /// <summary>
        /// Identifies IsPriceUp dependency property key.
        /// </summary>
        internal static readonly DependencyPropertyKey IsPriceUpPropertyKey =
            DependencyProperty.RegisterReadOnly("IsPriceUp", typeof(bool), typeof(ChartRenkoSegment), new FrameworkPropertyMetadata(false));
       
        /// <summary>
        /// Identifies IsPriceDown dependency property key.
        /// </summary>
        internal static readonly DependencyPropertyKey IsPriceDownPropertyKey =
            DependencyProperty.RegisterReadOnly("IsPriceDown", typeof(bool), typeof(ChartRenkoSegment), new FrameworkPropertyMetadata(false));
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies IsPriceUp dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceUpProperty = IsPriceUpPropertyKey.DependencyProperty;
      
        /// <summary>
        /// Identifies IsPriceDown dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceDownProperty = IsPriceDownPropertyKey.DependencyProperty;
       
        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(ChartRenkoSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the geometry. This is a dependency property.
        /// </summary>
        /// <remarks>Represents the geometry of the segment.</remarks>
        /// <value>The geometry.</value>
        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether this renko segment represents price moving up.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this instance is price up; otherwise, <c>false</c>.
        /// </value>
        public bool IsPriceUp
        {
            get { return (bool)GetValue(IsPriceUpProperty); }
        }

        /// <summary>
        /// Gets a value indicating whether this renko segment represents price moving down.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this segment is price down; otherwise, <c>false</c>.
        /// </value>
        public bool IsPriceDown
        {
            get { return (bool)GetValue(IsPriceDownProperty); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartRenkoSegment"/> class.
        /// </summary>
        static ChartRenkoSegment()
        {
            Type type = typeof(ChartRenkoSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRenkoSegment"/> class.
        /// </summary>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        /// <param name="representedRects">The represented rects.</param>
        internal ChartRenkoSegment(ChartIndexedDataPoint correspondingPoint, ChartSeries series, Rect[] representedRects)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
            this.m_representedRectangles = representedRects;
            xRange = DoubleRange.Empty;
            yRange = DoubleRange.Empty;
            yRange += representedRects[representedRects.Length - 1].Bottom;
            xRange += representedRects[representedRects.Length - 1].Left;
            ////Saving range represented by each rectangle. 
            foreach (Rect rectangle in representedRects)
            {
                xRange += rectangle.Right;
                yRange += rectangle.Top;
            }

            if (series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
            {
                xRange += xRange.Start - 1;
                xRange += xRange.End + 0.5;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            if (this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }
            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }
            bool shouldReassignGeometry = false;
            PathFigure figure;
            PathFigure[] figures = new PathFigure[m_representedRectangles.Length];
            for (int i = 0; i < m_representedRectangles.Length; i++)
            {
                ////Retrieving real coordinates of renko rectangle in chart area presenter.
                Point leftTopPoint = transformer.TransformToVisible(m_representedRectangles[i].TopLeft.X, m_representedRectangles[i].TopLeft.Y);
                Point rightBottomPoint = transformer.TransformToVisible(m_representedRectangles[i].BottomRight.X, m_representedRectangles[i].BottomRight.Y);
                ////Renko rectangle in real coordinates.
                Rect rectangle = new Rect(leftTopPoint, rightBottomPoint);
                if (!m_transformedRectangles.Contains(rectangle))
                {
                    ////m_transformedRectangles.RemoveAt(i);
                    if (m_transformedRectangles.Count - 1 >= i)
                    {
                        m_transformedRectangles[i] = rectangle; ////.Add(rectangle);
                    }
                    else
                    {
                        m_transformedRectangles.Add(rectangle);
                    }

                    shouldReassignGeometry = true;
                }
                ////Building rectangle figure.
                figure = new PathFigure();
                figure.StartPoint = rectangle.TopLeft;
                figure.Segments.Add(new LineSegment(rectangle.TopRight, true));
                figure.Segments.Add(new LineSegment(rectangle.BottomRight, true));
                figure.Segments.Add(new LineSegment(rectangle.BottomLeft, true));
                figure.IsClosed = true;
                figures[i] = figure;
            }

            if (shouldReassignGeometry)
            {
                this.Geometry = new PathGeometry(figures);
                ////m_transformedRectangles.Clear();
            }

            if (m_transformedRectangles.Count > m_representedRectangles.Length)
            {
                throw null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
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
            this.AxisLabelInfo = null;            
        }
        #endregion
    }

    /// <summary>
    /// Represents Renko chart type.
    /// </summary>
    /// <remarks>
    /// Renko charting method is thought to have acquired its name from
    /// &quot;renga&quot; which is the Japanese word for bricks. Renko Charts were
    /// introduced by Steve Nison. Renko (Bricks) are drawn equal in size for a
    /// determined amount. A brick is drawn in the direction of the prior move only if
    /// prices move by a minimum amount. If prices change by the determined amount or
    /// more, a new brick is drawn. If prices change by less than the determined amount
    /// (specified by ReversalAmount), the new price is ignored. The default value of
    /// ReversalAmount is 1.
    /// <para></para>
    /// <para>If the new closing price penetrates the previous bricks closing price in
    /// the opposite direction a trend reversal highlighted by the change in color of
    /// the bricks happens. Use the PriceUpColor to indicate bullish trend and
    /// PriceDownColor to indicate bearish trend.</para>
    /// <para></para>
    /// <para>Since a Renko chart isolates the underlying trends by filtering out the
    /// minor ups and downs, Renko charts are excellent in determining support and
    /// resistance levels.  </para>
    /// </remarks>
    /// <seealso cref="ChartRenkoSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartRenkoType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the RenkoCost dependency property.
        /// </summary>
        public static readonly DependencyProperty RenkoCostProperty =
          DependencyProperty.RegisterAttached("RenkoCost", typeof(double), typeof(ChartRenkoType), new ChartPropertyMetadata(1d, new PropertyChangedCallback(OnRenkoCostChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Identifies the BoxSize dependency property.
        /// </summary>
        public static readonly DependencyProperty BoxSizeProperty =
            DependencyProperty.RegisterAttached("BoxSize", typeof(string), typeof(ChartRenkoType), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the IsPercentageSize dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPercentageSizeProperty =
            DependencyProperty.RegisterAttached("IsPercentageSize", typeof(bool), typeof(ChartRenkoType), new PropertyMetadata(false));

        #endregion

        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Called when renkocost property value changed in RenKo Chart
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnRenkoCostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries ser = obj as ChartSeries;
            if(ser != null)
                ((Syncfusion.Windows.Chart.ChartRenkoType)(ser.ChartType)).Update(ser);
        }

        /// <summary>
        /// Called when BoxSize property is changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries ser = obj as ChartSeries;
            if (ser != null && ((Syncfusion.Windows.Chart.ChartRenkoType)(ser.ChartType))!= null)
            {
                if (Convert.ToString(args.NewValue).Contains("%"))
                {
                    ChartRenkoType.SetIsPercentageSize(ser, true);                    
                    
                }
                else
                {
                    ChartRenkoType.SetIsPercentageSize(ser, false);
                }
                ((Syncfusion.Windows.Chart.ChartRenkoType)(ser.ChartType)).Update(ser);
            }
        }

        /// <summary>
        /// Gets the renko cost.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Renko Cost</returns>
        public static double GetRenkoCost(ChartSeries series)
        {
            return (double)series.GetValue(RenkoCostProperty);
        }

        /// <summary>
        /// Sets the renko cost.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetRenkoCost(ChartSeries series, double value)
        {
            series.SetValue(RenkoCostProperty, value);
        }


        /// <summary>
        /// Return the string Value from the given ChartSeries
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public static string GetBoxSize(ChartSeries ser)
        {
            return (string)ser.GetValue(BoxSizeProperty);
        }


        /// <summary>
        /// Set BoxSize to the Corresponding ChartSeries from the Given value.
        /// </summary>
        /// <param name="ser"></param>
        /// <param name="value"></param>
        public static void SetBoxSize(ChartSeries ser, string value)
        {
            ser.SetValue(BoxSizeProperty, value);
        }


        /// <summary>
        /// Return the bool Value from the given ChartSeries
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public static bool GetIsPercentageSize(ChartSeries ser)
        {
            return (bool)ser.GetValue(IsPercentageSizeProperty);
        }


        /// <summary>
        /// Set SignalLineColor to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="ser"></param>
        /// <param name="value"></param>
        public static void SetIsPercentageSize(ChartSeries ser, bool value)
        {
            ser.SetValue(IsPercentageSizeProperty, value);
        }
        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public override void Calculate(ChartSeries series)
        {
            int count = series.PointsCount;
            double renkoCost = ChartRenkoType.GetRenkoCost(series);
            object boxSize = ChartRenkoType.GetBoxSize(series);
            if (ChartRenkoType.GetIsPercentageSize(series))
            {
                int index = Convert.ToString(boxSize).IndexOf('%');
                string value = Convert.ToString(boxSize).Remove(index);
                boxSize = Convert.ToDouble(value) / 100;
            }

            double Max = 0d;
            double Min = double.PositiveInfinity;
            ChartIndexedDataPoint[] indexedDataPoints = new ChartIndexedDataPoint[count];
            if (count == 0)
                return;
            for (int i = 0; i < count; i++)
            {
                Max = Math.Max(series.GetPoint(i).Y, Max);
                Min = Math.Min(series.GetPoint(i).Y, Min);
                indexedDataPoints[i] = new ChartIndexedDataPoint(series.GetPoint(i), i);
            }

            Array.Sort(indexedDataPoints, new ChartIndexedDataPointByXComparer());

            double currentX = indexedDataPoints[0].DataPoint.X;
            double dataPointsRange = indexedDataPoints[indexedDataPoints.Length - 1].DataPoint.X - indexedDataPoints[0].DataPoint.X;
            double previousLow = indexedDataPoints[0].DataPoint.Y;
            double previousHigh = indexedDataPoints[0].DataPoint.Y;
            Rect[] representedRectangles;            
            for (int i = 1; i <= indexedDataPoints.Length - 1; i++)
            {
                IChartDataPoint point = indexedDataPoints[i].DataPoint;

                double deltaYMin = point.Y - previousLow;
                double deltaYMax = point.Y - previousHigh;
                double deltaX;
                int renkoCount;
                renkoCost = renkoCost <= 0 ? 0.1 : renkoCost;
                if (deltaYMin <= -renkoCost)
                {
                    renkoCount = (int)Math.Floor(Math.Abs(deltaYMin / renkoCost));
                    deltaX =  boxSize != null ? (ChartRenkoType.GetIsPercentageSize(series) == true ? ((Convert.ToDouble(boxSize)) * (Max - Min)) : Convert.ToDouble(boxSize)) : ((0.04) * (Max - Min)); // (point.X - currentX) / renkoCount;
                    representedRectangles = new Rect[renkoCount];
                    for (int ik = 0; ik < renkoCount; ik++, currentX += deltaX)
                    {
                        Point topLeftPoint = new Point(currentX, (previousLow - ik * renkoCost));
                        Point bottomRightPoint = new Point((currentX + deltaX), (previousLow - (ik + 1) * renkoCost));
                        representedRectangles[ik] = new Rect(topLeftPoint, bottomRightPoint);
                    }

                    ChartRenkoSegment renkoSegment = new ChartRenkoSegment(indexedDataPoints[i - 1], series, representedRectangles);
                    renkoSegment.SetValue(ChartRenkoSegment.IsPriceDownPropertyKey, true);
                    series.Segments.Add(renkoSegment);
                    previousLow -= renkoCount * renkoCost;
                    previousHigh = previousLow + renkoCost;
                }

                if (deltaYMax >= renkoCost)
                {
                    renkoCount = (int)Math.Floor(Math.Abs(deltaYMin/ renkoCost));
                    deltaX =  boxSize != null ? (ChartRenkoType.GetIsPercentageSize(series) == true ? ((Convert.ToDouble(boxSize)) * (Max - Min)) : Convert.ToDouble(boxSize)) : ((0.04) * (Max - Min)); // (point.X - currentX) / renkoCount;
                    representedRectangles = new Rect[renkoCount];
                    for (int ik = 0; ik < renkoCount; ik++, currentX += deltaX)
                    {
                        Point ltPoint = new Point(currentX, (previousHigh + ik * renkoCost));
                        Point rbPoint = new Point((currentX + deltaX), (previousHigh + (ik + 1) * renkoCost));
                        representedRectangles[ik] = new Rect(ltPoint, rbPoint);
                    }

                    ChartRenkoSegment renkoSegment = new ChartRenkoSegment(indexedDataPoints[i - 1], series, representedRectangles);
                    renkoSegment.SetValue(ChartRenkoSegment.IsPriceUpPropertyKey, true);
                    series.Segments.Add(renkoSegment);
                    previousHigh += renkoCount * renkoCost;
                    previousLow = previousHigh - renkoCost;
                }
            }
        }

        /// <summary>
        /// Updates the specified series.
        /// </summary>
        /// <param name="series">The series.</param>
          /// <seealso cref="ChartRenkoType"/>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartRenkoType"/>
        public override string ToString()
        {
            return "Renko";
        }
        #endregion
    }
}
