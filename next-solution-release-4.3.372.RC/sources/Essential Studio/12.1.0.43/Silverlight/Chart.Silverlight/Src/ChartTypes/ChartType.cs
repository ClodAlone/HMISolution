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
    using System.Windows.Data;
    using System.Collections.Generic;

    /// <summary>
    /// Represents chart type.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public abstract class ChartType : IDisposable
    {

        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public virtual void Calculate(ChartSeries series)
        {


            series.Segments.Clear();
            series.Adornments.Clear();
            if (series.Type == ChartTypes.Area || series.Type == ChartTypes.StackingArea ||
                series.Type == ChartTypes.StepArea || series.Type == ChartTypes.SplineArea || ChartPolarType.GetDrawType(series.Area) == ChartPolarDrawType.Area)
            {
                if ((series.Area.isZoomactivated == true))
                {
                    series.ActualOpacityOnZoom = 0.8d;
                }
                else
                    series.ActualOpacityOnZoom = 1d;
            }
            else
            {
                series.ActualOpacityOnZoom = 1d;
            }

            if (ChartSeries.zoomseries != null)
            {

                if (series.IsZoomable == true)
                {
                    for (int i = 0; i < series.Area.Series.Count; i++)
                    {

                        if (series.Area.Series[i].IsZoomable != true)
                        {
                            series.Area.Series[i].Opacity = series.InactiveSeriesOpacityOnZoom;
                        }
                    }
                }

            }

            series.Area.IntializeZoomingScrollBarVisibility();
            series.Area.indexedarea = this.findIndexed(series);
            if (series.Area.indexedarea)
            {
                series.IndexActualData = series.ConvertToActualData(series.Data);
                var data = series.IndexActualData;
                if (series.IsSortData)
                {
                    var spoints = data.OrderBy(point => point.X);
                    if (series.SortingDirection == Direction.Ascending)
                    {
                        spoints = data.OrderBy(point => point.Y);
                    }
                    else
                    {
                        spoints = data.OrderByDescending(point => point.Y);
                    }
                    data = new ChartPointsCollection();
                    int index = -1;
                    foreach (var pt in spoints)
                    {
                        index++;
                        data.Add(new ChartPoint(index, pt.Values));
                    }
                    series.IndexActualData = data;
                }
                if (series.IndexActualData!= null && series.IndexActualData.Count > 0)
                this.CalculateSegments(series, series.IndexActualData);
            }
            else
            {
                ChartPointsCollection data = series != null && ((series.XAxis != null && series.XAxis.IsLogarithmic) || (series.YAxis != null && series.YAxis.IsLogarithmic)) ? series.ConvertToLogData(series.Data) : series.Data;
                if (series.IsSortData && series.XAxis != null)
                {
                    var XValues = series.XAxis.IndexDataContents;
                    var spoints = data.OrderBy(point => point.X);
                    switch (series.SortBy)
                    {
                        case SortingAxis.X:
                            if (series.SortingDirection == Direction.Ascending)
                            {
                                spoints = data.OrderBy(point => point.X);
                            }
                            else
                            {
                                spoints = data.OrderByDescending(point => point.X);
                            }
                            break;
                        case SortingAxis.Y:
                            if (series.SortingDirection == Direction.Ascending)
                            {
                                spoints = data.OrderBy(point => point.Y);
                            }
                            else
                            {
                                spoints = data.OrderByDescending(point => point.Y);
                            }
                            break;
                        case SortingAxis.XY:
                            if (series.SortingDirection == Direction.Ascending)
                            {
                                spoints = data.OrderBy(point => point.X).ThenBy(point => point.Y);
                            }
                            else
                            {
                                spoints = data.OrderByDescending(point => point.X).ThenBy(point => point.Y);
                            }
                            break;
                    }
                    data = new ChartPointsCollection();
                    int index = -1;
                    foreach (var pt in spoints)
                    {
                        index++;
                        data.Add(new ChartPoint(pt.X, pt.Y));
                    }
                }
                if(data!=null && data.Count>0  )
                this.CalculateSegments(series, data);
            }

            if (ChartSeries.zoomseries != null && series != null && series.XAxis != null && series.YAxis != null && series.IsZoomable == true)
            {
                series.XAxis.EnableZooming = true;
                series.YAxis.EnableZooming = true;
            }

            if (series.XAxis != null || series.YAxis != null)
            {
                ////Visibility m_zoomvisibility = ZoomingToolKit.GetZoomingToolkitVisibility(series.Area);
                ////if (m_zoomvisibility == Visibility.Collapsed)
                ////    if (series.XAxis.ZoomRange == series.XAxis.ZoomVisibleRange)
                ////    {
                ////        //series.XAxis.ZoomRange = series.XAxis.Range;
                ////        //series.YAxis.ZoomRange = series.YAxis.Range;
                ////        //series.XAxis.ZoomVisibleRange = series.XAxis.Range;
                ////        //series.YAxis.ZoomVisibleRange = series.YAxis.Range;
                ////        series.XAxis.isUpdateZoomrange = true;
                ////        series.YAxis.isUpdateZoomrange = true;
                ////    }
                if(series.XAxis !=null)
                series.ZoomCenter(series.XAxis);
                if(series.YAxis !=null)
                series.ZoomCenter(series.YAxis);
            }
        }

        bool findIndexed(ChartSeries series)
        {
            bool result = true;
            series.Area.indexedarea = series.Area.Series[0].IsIndexed;
            foreach (ChartSeries ser in series.Area.Series)
            {
                series.Area.indexedarea = series.Area.indexedarea & ser.IsIndexed;
                int[] data = (from oneseries in series.Area.Series select oneseries.Data.Count).ToArray<int>();
                foreach (int d in data)
                {
                    if (d != data[0])
                    {
                        result = false;
                        break;
                    }
                }

                for (int i = 0; i < ser.Area.Series.Count && result; i++)
                {
                    for (int j = 0; j < ser.Data.Count; j++)
                    {
                        if (ser.Data[j].X != ser.Area.Series[i].Data[j].X)
                        {
                            result = false;
                            break;
                        }
                    }
                }

                if (result == false)
                {
                    break;
                }
            }

            if (series.Area.indexedarea && result)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates Series X-Axis and y-Axis Range.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <param name="numberofYvalues">Number of Y values.</param>
        protected void SetRange(ChartSeries series, ChartPointsCollection points, int numberofYvalues)
        {
            ////To Initialize the Series X-Axis and Y-Axis
            InitializeSeriesAxis(series);
            double minx = double.MaxValue, miny = double.MaxValue, maxx = double.MinValue, maxy = double.MinValue;
            if (points.Count > 0 && points[0].Values.Length >= numberofYvalues)
            {
                if (series.Area != null && series.Area.ChartAreaParent != null && series.Area.ChartAreaParent.PrimaryAxis.IsAutoSetRange)
                {
                    ChartPointsCollection syncpts = new ChartPointsCollection();

                    foreach (ChartSeries ser in series.Area.ChartAreaParent.Series)
                    {
                        foreach (ChartPoint pt in ser.Data)
                        {
                            syncpts.Add(pt);
                        }
                    }

                    var xdatas = from point in syncpts select point.X;
                    minx = xdatas.Min();
                    maxx = xdatas.Max();
                }
                else
                {
                    IEnumerable<double> xdatas = new List<double>();
					if(series.DataModel != null && series.DataModel.ChartPoints != null && series.DataModel.ChartPoints.Count > 0 &&
                       points.Count != series.DataModel.ChartPoints.Count)
                        xdatas = from point in series.DataModel.ChartPoints select point.X;
                    else
                    xdatas = from point in points select point.X;
                    minx = xdatas.Min();
                    maxx = xdatas.Max();
                }

                miny = (from point in points select (from d in point.Values.Take<double>(numberofYvalues) select d).Min()).Min();
                maxy = (from point in points select (from d in point.Values.Take<double>(numberofYvalues) select d).Max()).Max();

                if (series.BindingPathsY == null && series.Type == ChartTypes.Histogram && maxy == 0d)
                {
                    maxy = series.Data.Count;
                }
            }
            else
            {
                minx = miny = 1;
                maxx = maxy = 0;
            }

            if (miny < 0)
            {
                miny += miny;
            }
            else if (series != null && series.YAxis != null && series.YAxis.ValueType != ChartValueType.DateTime)
            {
                miny = 0;
            }

            double interval = !series.XAxis.IsFractionalData ? series.XAxis.VisibleInterval : series.XAxis.GetNiceInterval(new DoubleRange(minx, maxx), series.XAxis.DesiredIntervalsCount);
            #region IndexedAxis
            if(series.IsIndexed)
            {
                switch (series.XAxis.RangeCalculationMode)
                {
                    case RangeCalculationMode.AdjustAcrossChartTypes:
                        //This condition is included because for polar and radar by default an extra range will be added at end, so there is no need to add range at end
                        if (series.Type == ChartTypes.Polar || series.Type == ChartTypes.Radar)
                        {
                            minx = minx - (interval < 1 ? interval : 1d);
                        }
                        else
                        {
                            minx = minx - (interval < 1 ? interval : 1d);
                            maxx = maxx + (interval < 1 ? interval : 1d);
                        }
                        break;
                }
            }
            #endregion
            #region Non-IndexedAxis
            else
                switch (series.XAxis.RangePadding)
                {
                    case ChartRangePaddingType.Normal:
                        {
                            if (series.Type != ChartTypes.Polar && series.Type != ChartTypes.Radar)
                            {
                                minx = minx - (interval <= 1 ? interval : 1d);
                                maxx = maxx + (interval <= 1 ? interval : 1d);
                            }
                        }

                        break;
                    case ChartRangePaddingType.Additional:
                        {
                            minx = minx -  (interval * series.Area.PrimaryAxis.AdditionalPadding.Start);
                            maxx = maxx +  (interval * series.Area.PrimaryAxis.AdditionalPadding.End);
                        }
                        break;
                }
            #endregion

            series.XAxis.firstinterval = double.NaN;
            if (minx == double.MaxValue)
            {
                minx = miny = 0;
                maxx = maxy = 1;
            }

            if (series.Visibility == Visibility.Collapsed)
            {
                miny = maxy = 0;
                minx = maxx = double.NaN;
                if ((from ser in series.Area.Series where ser.Visibility == Visibility.Collapsed select ser).Count<ChartSeries>() == series.Area.Series.Count)
                {
                    minx = maxx = 0;
                }
            }

            ////To Set MinXY and MaxXY of Series
            series.XCidsRange = new DoubleRange(minx, maxx);
            series.YCidsRange = new DoubleRange(miny, maxy);
            InitializeAxes(series);
        }
        bool IsAdjustAxisValues(ChartTypes val)
        {
            bool result = false;
            switch (val)
            {
                case ChartTypes.Column:
                case ChartTypes.FastColumn:
                case ChartTypes.Bar:
                case ChartTypes.StackingBar:
                case ChartTypes.StackingColumn:
                case ChartTypes.Bubble:
                case ChartTypes.HiLo:
                case ChartTypes.HiLoOpenClose:
                case ChartTypes.Gantt:
                case ChartTypes.BoxAndWhisker:
                case ChartTypes.Candle:
                case ChartTypes.RangeColumn:
                case ChartTypes.Tornado:
                case ChartTypes.StackingColumn100:
                case ChartTypes.StackingArea100:
                case ChartTypes.StackingBar100:
                case ChartTypes.PointAndFigure:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }
        /// <summary>
        /// Method implementation for Initialize the Range and Visible range for Axis 
        /// </summary>
        /// <param name="series"></param>
        protected void InitializeAxes(ChartSeries series)
        {
            DoubleRange m_visibleRange = new DoubleRange();
            if (series.XAxis.IsAutoSetRange == true)
            {
                if (series.Area != null && series.Area.Series.Count > 0 && series.Equals(series.Area.Series[0]))
                {
                    series.XAxis.Range = new DoubleRange(0, 1);
                    ////series.XAxis.m_visibleInterval = 1;
                    series.XAxis.VisibleRange = DoubleRange.Empty;
                }

                series.XAxis.isUpdateActualVisibleRange = series.XAxis.EnableZooming && !series.XAxis.Range.Equals(DoubleRange.Union(series.XAxis.VisibleRange, series.XRange));
                series.XAxis.Range = DoubleRange.Union(series.XAxis.VisibleRange, series.XRange);
                series.XAxis.VisibleRange = DoubleRange.Union(series.XAxis.VisibleRange, series.XRange);
            }
            else
            {
                series.XCidsRange = series.XAxis.Range;
                series.XAxis.VisibleRange = series.XAxis.Range;
            }

            if (series.Area != null && series.Area.Series.Count > 0 && series.Area.Series[0].Type == ChartTypes.StackingColumn100 || series.Area.Series[0].Type == ChartTypes.StackingBar100 || series.Area.Series[0].Type==ChartTypes.StackingArea100)
            {
                if (series.Area.isSwitchzoom == true && series.YAxis.EnableZooming == true)
                {
                    series.YAxis.VisibleRange = series.YAxis.Range;
                }
                else
                {
                    if (series.YAxis.m_VisibleRange.IsEmpty)
                        series.YAxis.m_VisibleRange = series.YAxis.Range;
                    if (series.Area.Series[0].Type == ChartTypes.StackingColumn100 || series.Area.Series[0].Type == ChartTypes.StackingBar100)
                    {
                        series.YAxis.Range = ChartStackingColumn100Type.GetShowValueAsProbability(series.Area) ? new DoubleRange(0, 1) : new DoubleRange(0, 100);
                        series.YAxis.VisibleInterval = ChartStackingColumn100Type.GetShowValueAsProbability(series.Area) ? 0.2 : 20;
                        series.YAxis.VisibleRange = ChartStackingColumn100Type.GetShowValueAsProbability(series.Area) ? new DoubleRange(0, 1) : new DoubleRange(0, 100);
                    }
                    else
                    {
                        series.YAxis.Range = ChartStackingArea100Type.GetShowValueAsProbability(series.Area) ? new DoubleRange(0, 1) : new DoubleRange(0, 100);
                        series.YAxis.VisibleInterval = ChartStackingArea100Type.GetShowValueAsProbability(series.Area) ? 0.2 : 20;
                        series.YAxis.VisibleRange = ChartStackingArea100Type.GetShowValueAsProbability(series.Area) ? new DoubleRange(0, 1) : new DoubleRange(0, 100);
                    }
                }
            }
            else if (series.YAxis.IsAutoSetRange == true)
            {
                if (series.Area != null && series.Area.Series.Count > 0 && series.Equals(series.Area.Series[0]))
                {
                    series.YAxis.Range = new DoubleRange(0, 1);
                    //series.YAxis.m_visibleInterval = 1;
                    series.YAxis.VisibleRange = DoubleRange.Empty;
                }

                this.CalculateYRange(series);
                series.YAxis.isUpdateActualVisibleRange = series.YAxis.EnableZooming;
                series.YAxis.Range = DoubleRange.Union(series.YAxis.VisibleRange, series.YRange);
                series.YAxis.VisibleRange = DoubleRange.Union(series.YAxis.VisibleRange, series.YRange);
            }
            else
            {
                if (!series.YAxis.m_VisibleRange.IsEmpty)
                {
                    series.YAxis.Range = series.YAxis.m_VisibleRange;
                    series.YAxis.m_VisibleRange = DoubleRange.Empty;
                }
                series.YCidsRange = series.YAxis.Range;
                series.YAxis.VisibleRange = series.YAxis.Range;
            }
            series.XAxis._IsSortedSeries = series.IsSortData;
            series.XAxis._SortingDirection = series.SortingDirection;
            series.XAxis._SortingAxis = series.SortBy;
            series.XAxis.BindDataFromDataSource();
            if (series.Area.indexedarea)
            {
                series.XAxis.Isindexedseries = true;
                series.XAxis.IndexDataContents = series.Indexeddata(series.Data);
                switch (series.Type)
                {
                    case ChartTypes.FastLine:
                    case ChartTypes.Line:
                    case ChartTypes.Scatter:
                    case ChartTypes.StackingArea:
                    case ChartTypes.Area:
                    case ChartTypes.RangeArea:
                    case ChartTypes.StepArea:
                    case ChartTypes.StepLine:
                    case ChartTypes.Spline:
                    case ChartTypes.SplineArea:
                    case ChartTypes.RotatedSpline:
                        if (series.XAxis.AxisDataContents != null && series.XAxis.AxisDataContents.Count > 0 && series.XAxis.AxisDataContents[0].Equals("") == true && series.XAxis.ValueType == ChartValueType.DateTime)
                        {
                            series.XAxis.AxisDataContents.RemoveAt(0);
                        }

                        break;
                }
            }
            else
            {
                double diff = Math.Ceiling(series.XAxis.Range.Start) - series.XAxis.Range.Start;
                if (series.XAxis.ValueType != ChartValueType.DateTime && (series.XAxis.IsFractionEnabledOnZoom && series.DataSource != null) && diff > 0)
                {
                    series.XAxis.firstinterval = diff;
                }
                else if (series.XAxis.ValueType == ChartValueType.DateTime && series.XAxis.ZoomFactor < 1)
                {
                    DateTime datetimeinterval = DateTime.FromOADate(series.XAxis.VisibleInterval);
                    double dateinterval = Math.Floor(datetimeinterval.ToOADate());
                    double timeinterval = datetimeinterval.ToOADate() - Math.Floor(datetimeinterval.ToOADate());
                    if (dateinterval == 0 && timeinterval > 0 && timeinterval < 1)
                    {
                        long ticks = series.XAxis.DateTimeInterval.Ticks - (DateTime.FromOADate(series.XAxis.Range.Start).TimeOfDay.Ticks % series.XAxis.DateTimeInterval.Ticks);
                        series.XAxis.firstinterval = new DateTime(ticks).ToOADate();
                    }
                    else
                    {
                        double days = series.XAxis.VisibleInterval - (series.XAxis.Range.Start % series.XAxis.VisibleInterval);
                        series.XAxis.firstinterval = days;
                    }
                }
                if (series.XAxis.AxisDataPosition != null && series.XAxis.AxisDataContents != null)
                {
                    series.XAxis.IndexDataContents = DataBinding.GetOrderedContentData(series, series.XAxis);
                    series.YAxis.IndexDataContents = DataBinding.GetOrderedContentData(series, series.YAxis);
                }
                series.XAxis.Isindexedseries = false;
            }

            series.XAxis.AxisType = DataBinding.GetPropertyType(series.DataSource, series.XAxis.PositionPath);
            if (series.BindingPathsY != null)
            {
                series.YAxis.AxisType = DataBinding.GetPropertyType(series.DataSource, series.YAxis.PositionPath);
            }

            series.XAxis.AxisType = series.XAxis.AxisType == null || CanConvertDouble(series.XAxis.AxisType) ? typeof(double) : series.XAxis.AxisType;
            if (series.XAxis.Header == null)
            {
                series.XAxis.Header = "";
            }

            if (series.YAxis.Header == null)
            {
                series.YAxis.Header = "";
            }

            if (series.Area.PrimaryAxis.Orientation == Orientation.Horizontal)
            {
                if (series.Area != null)
                {
                    if (series.Area.ChartAreaParent != null)
                    {
                        if (series.Area.ChartAreaParent.PrimaryAxis.IsAutoSetRange == true)
                        {
                            series.Area.ChartAreaParent.SetPrimaryAxisRange();
                            m_visibleRange = series.Area.ChartAreaParent.PrimaryAxis.VisibleRange;
                            series.Area.ChartAreaParent.PrimaryAxis.Range = series.Area.ChartAreaParent.PrimaryAxis.VisibleRange;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Return bool value from the given type value 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanConvertDouble(Type type)
        {
            if (type == typeof(string) || type == typeof(String) || type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Use to Initialize the series X-Axis and Y-Axis
        /// </summary>
        /// <param name="series">The series.</param>
        protected void InitializeSeriesAxis(ChartSeries series)
        {
            if (series.XAxis == null)
            {
                series.XAxis = series.Area.Axes[0];
            }
            else
            {
                if (series.XAxis != series.Area.PrimaryAxis)
                {
                    series.Area.Axes.Add(series.XAxis);
                }
            }

            if (series.YAxis == null)
            {
                series.YAxis = series.Area.Axes[1];
            }
            else
            {
                if (series.YAxis != series.Area.SecondaryAxis && !series.Area.Axes.Contains(series.YAxis))
                {
                    series.Area.Axes.Add(series.YAxis);
                }
            }
        }

        /// <summary>
        /// Calculates Y-Axis Range to perform RangePadding function.
        /// </summary>
        /// <param name="series">The series.</param>
        protected void CalculateYRange(ChartSeries series)
        {
            double start = series.YCidsRange.Start < 0 ? series.YCidsRange.Start + (series.YCidsRange.Start * (1)) : series.YCidsRange.Start + (series.YCidsRange.Start * (-1));
            double end = series.YCidsRange.End < 0 ? series.YCidsRange.End + (series.YCidsRange.Start * (1)) : series.YCidsRange.End + (series.YCidsRange.Start * (-1));
            double newinterval = Math.Floor((start + end) / (double)series.YAxis.DesiredIntervalsCount);
            newinterval = series.YAxis.CalculateNiceInterval(new DoubleRange(start, end), series.YAxis.DesiredIntervalsCount);
            newinterval = Math.Abs((newinterval == 0) ? 1 : newinterval);
            series.YAxis.endpadding = newinterval - (series.YCidsRange.End % newinterval);
            series.YAxis.startpadding = 0d;
            series.YAxis.iscalculateinterval = false;
            switch (series.YAxis.RangePadding)
            {
                case ChartRangePaddingType.Additional:
                    series.YCidsRange += series.YCidsRange.Start - (double.IsNaN(series.YAxis.Interval) ? newinterval : series.YAxis.Interval);
                    series.YAxis.startpadding = (double.IsNaN(series.YAxis.Interval) ? newinterval : series.YAxis.Interval);
                    series.YCidsRange += series.YCidsRange.End + ((double.IsNaN(series.YAxis.Interval) ? newinterval : series.YAxis.Interval) - (series.YCidsRange.End % (double.IsNaN(series.YAxis.Interval) ? newinterval : series.YAxis.Interval)));
                    series.YAxis.VisibleInterval = double.IsNaN(series.YAxis.Interval) ? ((newinterval > series.YAxis.VisibleInterval) ? newinterval : series.YAxis.VisibleInterval) : series.YAxis.Interval;
                        if (series.YCidsRange.End < 0)
                        {
                            series.YCidsRange = new DoubleRange(series.YCidsRange.Start, newinterval+series.XAxis.Origin);
                        }
                    break;
                case ChartRangePaddingType.Normal:
                    series.YCidsRange += series.YCidsRange.End + ((double.IsNaN(series.YAxis.Interval) ? newinterval : series.YAxis.Interval) - (series.YCidsRange.End % (double.IsNaN(series.YAxis.Interval) ? newinterval : series.YAxis.Interval)));
                    series.YAxis.VisibleInterval = double.IsNaN(series.YAxis.Interval) ? ((newinterval > series.YAxis.VisibleInterval) ? newinterval : series.YAxis.VisibleInterval) : series.YAxis.Interval;
                        if (series.YCidsRange.End < 0)
                        {
                            series.YCidsRange = new DoubleRange(series.YCidsRange.Start, series.XAxis.Origin);
                        }
                    break;
                case ChartRangePaddingType.None:
                    series.YAxis.iscalculateinterval = true;
                        if (series.YCidsRange.End < 0)
                        {
                            series.YCidsRange = new DoubleRange(series.YCidsRange.Start,series.XAxis.Origin );
                        }
                    break;
            }
        }
        /// <summary>
        /// Return ChartPointCollection from the given points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="series"></param>
        /// <returns></returns>
        protected ChartPointsCollection ConvertToInternalPoints(ChartPointsCollection points, ChartSeries series)
        {
            ChartPointsCollection newpoints = new ChartPointsCollection();
            foreach (ChartPoint point in points)
            {
                newpoints.Add(new ChartPoint(point.X + (series.XAxis.VisibleRange.Start * (-1)), point.Y + (series.YAxis.VisibleRange.Start * (-1))));
            }

            return newpoints;
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected virtual void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
        }

        /// <summary>
        /// Updates chart.
        /// </summary>
        public virtual void Update(ChartSeries series)
        {
            this.UpdateSegments(series, series.Data);
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected virtual void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
        }
        #region Public methods

        #endregion

        #region Implementation

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {

        }

        #endregion
    }

    /// <summary>
    /// public Class implementation for Segment
    /// </summary>
    public abstract class Segment : DependencyObject, IDisposable
    {
        /// <summary>
        ///  Identifies the SegmentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentTemplateProperty =
    DependencyProperty.Register("SegmentTemplate", typeof(DataTemplate), typeof(Segment), new PropertyMetadata(null));
      /// <summary>
      /// Get or Set segmentTemplate
      /// </summary>
        public DataTemplate SegmentTemplate
        {
            set { SetValue(SegmentTemplateProperty, value); }
            get { return (DataTemplate)GetValue(SegmentTemplateProperty); }
        }
        /// <summary>
        /// Series Variable declaration
        /// </summary>
        protected ChartSeries series = null;

        /// <summary>
        /// Identifies the IsSelected property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(Segment), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this segment is Selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is Selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsHighlighted property.
        /// </summary>
        public static readonly DependencyProperty HighlightedProperty =
            DependencyProperty.Register("Highlighted", typeof(bool), typeof(Segment), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this segment is highlighted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is highlighted; otherwise, <c>false</c>.
        /// </value>
        public bool Highlighted
        {
            get { return (bool)GetValue(HighlightedProperty); }
            set { SetValue(HighlightedProperty, value); }
        }

        /// <summary>
        /// Identifies the DataPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty DataPointProperty =
            DependencyProperty.Register("DataPoint", typeof(ChartDataPoint), typeof(Segment), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data point.
        /// </summary>
        /// <value>The data point.</value>
        public ChartDataPoint DataPoint
        {
            get { return (ChartDataPoint)GetValue(DataPointProperty); }
            set { SetValue(DataPointProperty, value); }
        }
        internal Brush SelectedSegmentInterior  = null;

        internal Brush HighlightedSegmentInterior = null;

        internal bool IsSegmentSelected = false;
        internal bool IsSegmentHighlightedOnMouseMove = false;
        
        /// <summary>
        /// Represents X-axis range of segment.
        /// </summary>
        protected DoubleRange xRange = DoubleRange.Empty;

        /// <summary>
        /// Represents Y-axis range of segment. 
        /// </summary>
        protected DoubleRange yRange = DoubleRange.Empty;

        /// <summary>
        /// Initializes a new instance of the <see>
        ///                                       <cref>ChartSegment</cref>
        ///                                   </see>
        ///     class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        protected Segment(ChartSeries series, ChartPointsCollection correspondingPoints)
        {
            this.DataPoint = new ChartDataPoint();
            if (correspondingPoints.Count > 0)
            {
                this.DataPoint.X = correspondingPoints[0].X;
                this.DataPoint.Y = correspondingPoints[0].Y;
                this.DataPoint.Values = correspondingPoints[0].Values;
                this.DataPoint.Visible = correspondingPoints[0].Visible;
            }

            this.series = series;
            if (series.Interior == null)
            {
                Brush[] colors = series.Area.ColorModel.GetBrushes(series.Area.ColorModel.Palette);
                this.Interior = series.PaletteInterior = (colors == null || colors.Count() == 0) ? new SolidColorBrush(Colors.Transparent) : colors[series.Area.Series.IndexOf(series) % colors.Count()];
            }
            else
            {
                this.Interior = series.PaletteInterior = series.Interior;
            }

            Binding paletteInteriorBinding = new Binding();
            paletteInteriorBinding.Path = new PropertyPath("PaletteInterior");
            paletteInteriorBinding.Source = this;
            BindingOperations.SetBinding(this, Segment.PaletteInteriorProperty, paletteInteriorBinding);

            if (correspondingPoints.Count > 0)
                if (correspondingPoints[0].EmptyPoint == true)
                    if (series.EmptyPointStyle == EmptyPointStyle.Interior || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                        this.Interior = series.EmptyPointInterior;

            Binding strokeBinding = new Binding();
            strokeBinding.Path = new PropertyPath("Stroke");
            strokeBinding.Source = series;
            BindingOperations.SetBinding(this, Segment.StrokeProperty, strokeBinding);

            Binding strokeThicknessBinding = new Binding();
            strokeThicknessBinding.Path = new PropertyPath("StrokeThickness");
            strokeThicknessBinding.Source = series;
            BindingOperations.SetBinding(this, Segment.StrokeThicknessProperty, strokeThicknessBinding);

            ////if (series.ChartType == ChartTypes.Line && series.StrokeThickness<2)
            ////{
            ////    //Set the Minimum Stroke Thickness for Line Chart
            ////    this.StrokeThickness = 2;
            ////}
            this.Opacity = series.ActualOpacityOnZoom;
            Binding effectsBinding = new Binding();
            effectsBinding.Source = this.Series;
            effectsBinding.Path = new PropertyPath("EnableEffects");
            effectsBinding.Converter = new EffectsVisibilityConverter();
            effectsBinding.Mode = System.Windows.Data.BindingMode.OneWay;
            effectsBinding.ConverterParameter = series;
            BindingOperations.SetBinding(series, ChartSeries.AdditionalEffectVisibilityProperty, effectsBinding);
        }

        /// <summary>
        /// Idenfities Interior dependency property.
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(Segment), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Interior. This is dependency property.
        /// </summary>
        /// <value>The Brush value.</value>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        /// <summary>
        /// Idenfities PaletteInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteInteriorProperty =
            DependencyProperty.Register("PaletteInterior", typeof(Brush), typeof(Segment), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Interior. This is dependency property.
        /// </summary>
        /// <value>The Brush value.</value>
        public Brush PaletteInterior
        {
            get { return (Brush)GetValue(PaletteInteriorProperty); }
            set { SetValue(PaletteInteriorProperty, value); }
        }

        /// <summary>
        /// Idenfities Stroke dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(Segment), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Stroke. This is dependency property.
        /// </summary>
        /// <value>The Brush value.</value>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Idenfities StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Segment), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the stroke thickness. 
        /// </summary>
        /// <remarks>
        /// This property in bound with similar property on series that segment belongs to.
        /// </remarks>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Idenfities Opacity dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(Segment), new PropertyMetadata(1d));

        /// <summary>
        /// Gets or sets the opacity. 
        /// </summary>
        /// <remarks>
        /// This property in bound with similar property on series that segment belongs to.
        /// </remarks>
        /// <value>The double.</value>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        Storyboard animatingStoryBoard = new Storyboard();
        internal Storyboard AnimatingStoryBoard
        {
            get { return animatingStoryBoard; }
            set { animatingStoryBoard = value; }
        }

        Storyboard m_MouseAnimation = new Storyboard();
        /// <summary>
        /// Get and Set MouseAnimation
        /// </summary>
        public Storyboard MouseAnimation
        {
            get
            {
                return m_MouseAnimation;
            }
            set
            {
                m_MouseAnimation = value;
            }
        }

        /// <summary>
        /// Gets the value of the parent Series. This is a depency property.
        /// </summary>
        [CLSCompliantAttribute(false)]
        public ChartSeries Series
        {
            get { return series; }
        }

        /// <summary>
        /// Gets the X data measure of series.
        /// </summary>
        /// <remarks>
        /// Used to determine X data range that segment represents.
        /// </remarks>
        /// <value>The X data measure.</value>
        public DoubleRange XDataMeasure
        {
            get { return xRange; }
        }

        /// <summary>
        /// Gets the Y data measure of series.
        /// </summary>
        /// <remarks>
        /// Used to determine Y data range that segment represents.
        /// </remarks>
        /// <value>The Y data measure.</value>
        public DoubleRange YDataMeasure
        {
            get { return yRange; }
        }

        /// <summary>
        /// Sets the X range for segment.
        /// </summary>
        /// <param name="values">The values.</param>
        protected void SetXRange(params double[] values)
        {
            xRange = DoubleRange.Union(values);
        }

        /// <summary>
        /// Sets the Y range for segment.
        /// </summary>
        /// <param name="values">The values.</param>
        protected void SetYRange(params double[] values)
        {
            yRange = DoubleRange.Union(values);
        }

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public virtual void Update(IChartTransformer transformer)
        {
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.series.ClearValue(ChartSeries.AdditionalEffectVisibilityProperty);
            if (this.series != null)
                this.series = null;
            if (this.DataPoint != null)
                this.DataPoint = null;

            if (this.xRange != null)
                this.xRange = DoubleRange.Empty;
            if (this.yRange != null)
                this.yRange = DoubleRange.Empty;
        }

        #endregion
    }

    /// <summary>
    /// Represents chart series points data.
    /// </summary>
    /// <remarks>
    /// Chart adornments are used to show additional information about displaying series.
    /// Adornments are widely used to simplify chart look and give user more idea of information that is
    /// represented via chart.
    /// </remarks>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    /// <!--Chart with Adornments-->
    ///   &lt;sfchart:Chart&gt;
    ///             &lt;sfchart:ChartArea Background="LightGray" GridBackground="White"&gt;  
    ///                 &lt;sfchart:ChartSeries Type="Column" &gt;
    ///                &lt;sfchart:ChartSeries.AdornmentsInfo&gt;
    ///                         &lt;sfchart:ChartAdornmentInfo 
    /// LabelContentPath="DataPoint.X" Visible="True"  /&gt;
    ///                     &lt;/sfchart:ChartSeries.AdornmentsInfo&gt;
    ///                 &lt;/sfchart:ChartSeries&gt;       
    ///             &lt;/sfchart:ChartArea&gt;          
    ///         &lt;/sfchart:Chart&gt;
    /// </code>
    /// C#:
    /// <code language="C#">
    /// ChartSeries series = Chart1.Areas[0].Series[0];      
    /// ChartAdornmentInfo adornments = series.AdornmentsInfo;
    /// adornments.LabelContentPath = "DataPoint.X";
    /// adornments.Visible = true;
    /// </code>
    /// </example>
    public class ChartAdornment : Segment
    {
        #region Members
        /// <summary>
        /// Initializes m_point
        /// </summary>
        internal ChartPoint m_point;
        internal int index = 0;
        internal double additionalspace = 0d;
        internal int adornemntLabelIndex = 0;
        #endregion

        #region Dependency properties

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartAdornment), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set TemplateProperty
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the X value dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(ChartAdornment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y value dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(ChartAdornment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the DataPoint dependency property.
        /// </summary>
        public new static readonly DependencyProperty DataPointProperty =
            DependencyProperty.Register("DataPoint", typeof(ChartDataPoint), typeof(ChartAdornment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Index dependency property.
        /// </summary>
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(ChartAdornment), new PropertyMetadata(-1));

        /// <summary>
        /// Identifies the SegmentLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelProperty =
            DependencyProperty.Register("SegmentLabel", typeof(object), typeof(ChartAdornment), new PropertyMetadata(string.Empty));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the segment label. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Allows specify value that should be displayed for the segment's adorner.
        /// </remarks>
        /// <example>
        /// This property is not intended to be used from user code. All segment labels are 
        /// assigned automatically. 
        /// </example>
        /// <value>The segment label type of <see cref="String"/>.</value>
        public object SegmentLabel
        {
            get
            {
                return (object)GetValue(SegmentLabelProperty);
            }

            set
            {
                SetValue(SegmentLabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X. This is a dependency property.
        /// </summary>
        /// <example>
        /// This property is not intended to be used from user code. All segment positions are 
        /// assigned automatically with respect to segments. 
        /// </example>
        /// <value>The X value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y. This is a dependency property.
        /// </summary>
        /// <example>
        /// This property is not intended to be used from user code. All segment positions are 
        /// assigned automatically with respect to segments. 
        /// </example>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data point.
        /// </summary>
        /// <value>The data point.</value>
        internal new ChartDataPoint DataPoint
        {
            get { return (ChartDataPoint)GetValue(DataPointProperty); }
            set { SetValue(DataPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAdornment"/> class.
        /// </summary>
        /// <remarks>
        /// Chart adornments are used to show additional information about displaying series.
        /// </remarks>
        /// <param name="position">Adornment's position.</param>
        /// <param name="point">Point to represent.</param>
        /// <param name="series">Connected to series.</param>
        /// <param name="space">Additional space for column and bar types</param>
        internal ChartAdornment(ChartPoint position, ChartPointsCollection point, ChartSeries series, double space)
            : base(series, point)
        {
            m_point = new ChartPoint(position.X, position.Y);
            this.index = point.IndexOf(position);
            this.DataPoint = new ChartDataPoint();
            this.DataPoint.X = m_point.X;
            this.DataPoint.Y = m_point.Y;
            this.DataPoint.Values = new double[] { m_point.Y };
            this.DataPoint.Label = m_point.Y.ToString();
            this.DataPoint.Visible = m_point.Visible;
            this.DataPoint.Tag = this.DataPoint;
            this.Index = this.index;
            this.additionalspace = space;
            this.Template = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "AdornmentLabelTemplate");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAdornment"/> class.
        /// </summary>
        /// <remarks>
        /// Chart adornments are used to show additional information about displaying series.
        /// </remarks>
        /// <param name="position">Adornment's position.</param>
        /// <param name="actualposition">Actual position</param>
        /// <param name="point">Point to represent.</param>
        /// <param name="series">Connected to series.</param>
        /// <param name="space">Additional space for column and bar types</param>
        internal ChartAdornment(ChartPoint position, ChartPoint actualposition, ChartPointsCollection point, ChartSeries series, double space)
            : base(series, point)
        {
            m_point = new ChartPoint(position.X, position.Y);
            this.index = point.IndexOf(position);
            this.DataPoint = new ChartDataPoint();
            this.DataPoint.X = m_point.X;
            this.DataPoint.Y = m_point.Y;
            this.DataPoint.Values = position.Values;
            this.DataPoint.Label = m_point.Y.ToString();
            this.DataPoint.Visible = m_point.Visible;
            this.DataPoint.Tag = this.DataPoint;
            this.Index = this.index;
            this.additionalspace = space;
            m_point = new ChartPoint(actualposition.X, actualposition.Y);
            this.Template = ResourceManager.GetAdornmentsTemplate(typeof(ChartAdornment), "AdornmentLabelTemplate");
        }
       
        #endregion

        #region Implementation

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of the class that implements <see cref="IChartTransformer"/></param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            Point pt = new Point();
            if (series.IsRotated(series.Type))
            {
                pt = transformer.TransformToVisible(m_point.Y + (series.YAxis.Range.Start * (-1)), m_point.X + (series.XAxis.Range.Start * (-1)) + additionalspace, series);
            }
            else if (series.Area.GetSeriesAxesType(series.Type) == ChartAxesType.None)
            {
                pt = transformer.TransformToVisible(m_point.X + additionalspace, m_point.Y, series);
            }
            else if (series.Area.GetSeriesAxesType(series.Type) == ChartAxesType.RadarAxes || series.Area.GetSeriesAxesType(series.Type) == ChartAxesType.PolarAxes)
            {
                pt = transformer.TransformToVisible(m_point.X, m_point.Y, series);
            }
            else
            {
                pt = transformer.TransformToVisible(m_point.X + (series.XAxis.Range.Start * (-1)) + additionalspace, m_point.Y + (series.YAxis.Range.Start * (-1)), series);
            }

            this.X = pt.X;
            this.Y = pt.Y;
        }

        Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
            endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
            endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
            endpoint.X += originpoint.X;
            endpoint.Y += originpoint.Y;
            return endpoint;
        }
        #endregion
    }
}
