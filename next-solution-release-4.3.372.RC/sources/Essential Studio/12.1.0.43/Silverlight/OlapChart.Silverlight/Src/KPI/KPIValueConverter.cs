#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Data;
using Syncfusion.Windows.Chart;
using Syncfusion.OlapSilverlight.Data;
using System.Windows.Media;

namespace Syncfusion.Silverlight.Chart.Olap
{
    #region KPI Series Identifier

    public static class KPISeriesSegmentHelper
    {
        public static void GetSeriesSegment(object value, ref ChartSeries series, ref Segment segment)
        {
            //// Returns the current series
            switch (value.GetType().Name)
            {
                case "ColumnSegment":
                    segment = (value as ColumnSegment);
                    series = segment.Series;
                    break;
                case "BarSegment":
                    segment = (value as BarSegment);
                    series = segment.Series;
                    break;
                case "ChartPieSegment":
                    segment = (value as ChartPieSegment);
                    series = segment.Series;
                    break;
                case "LineSegment":
                    segment = (value as Syncfusion.Windows.Chart.LineSegment);
                    series = segment.Series;
                    break;
                case "PolarSegment":
                    segment = (value as PolarSegment);
                    series = segment.Series;
                    break;
                case "ScatterSegment":
                    segment = (value as ScatterSegment);
                    series = segment.Series;
                    break;
                case "AreaSegment":
                    segment = (value as AreaSegment);
                    series = segment.Series;
                    break;
                case "ChartFunnelSegment":
                    segment = (value as ChartFunnelSegment);
                    series = segment.Series;
                    break;
                case "StackingAreaSegment":
                    segment = (value as StackingAreaSegment);
                    series = segment.Series;
                    break;
                case "StackingBarSegment":
                    segment = (value as StackingBarSegment);
                    series = segment.Series;
                    break;
                case "StackingColumnSegment":
                    segment = (value as StackingColumnSegment);
                    series = segment.Series;
                    break;
                case "ChartStepLineSegment":
                    segment = (value as ChartStepLineSegment);
                    series = segment.Series;
                    break;
                case "ChartSplineSegment":
                    segment = (value as ChartSplineSegment);
                    series = segment.Series;
                    break;
                case "ChartSplineAreaSegment":
                    segment = (value as ChartSplineAreaSegment);
                    series = segment.Series;
                    break;
                case "ChartPyramidSegment":
                    segment = (value as ChartPyramidSegment);
                    series = segment.Series;
                    break;
            }
        }
    } 

    #endregion

    #region Image Margin Converter
    
    public class KPIMarginValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartSeries series = null;
            Segment segment = null;

            System.Windows.Controls.Orientation orientation = System.Windows.Controls.Orientation.Horizontal;

            KPISeriesSegmentHelper.GetSeriesSegment(value, ref series, ref segment);

            if (series != null && segment != null)
            {
                orientation = series.Area.Axes[0].Orientation;

                if (orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    return new Thickness(5, -30, 0, 30);
                }
                else
                    return new Thickness(5, 0, 0, 0);
            }

            return new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    } 

    #endregion

    #region Image Source Converter
    
    public class KPIImageSourceConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartSeries series = null;
            Segment segment = null;

            KPISeriesSegmentHelper.GetSeriesSegment(value, ref series, ref segment);

            if (series != null && segment != null)
            {
                //// Our data context is ChartSegment. So, passing that to get the segment index.
                //// Using which we'll get the data from the data source. 
                //// Since, storing the value in the Series.Tag will no help since, all segments will use same series.
                int index = series.Segments.IndexOf(segment);
                var data = (series.DataSource as OlapChartPointCollection);
                string tagValue = ((int)data[index].BindingY).ToString();
                var pivotCellDescriptor = (data[index].Tag as Syncfusion.OlapSilverlight.Engine.PivotCellDescriptor);

                return GetImageSource(tagValue, pivotCellDescriptor);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helper Methods

        private static object GetImageSource(string tagValue, Syncfusion.OlapSilverlight.Engine.PivotCellDescriptor cellDescriptor)
        {
            if (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status)
            {
                if (cellDescriptor.KpiGraphicsStyle == KpiGraphics.RoadSigns)
                {
                    if (tagValue == "1")
                    {
                        return
                            new ImageSourceConverter().ConvertFromString(
                                "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/Green.png");
                    }
                    else if (tagValue == "0")
                    {
                        return
                            new ImageSourceConverter().ConvertFromString(
                                "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/Yellow.png");

                    }
                    else if (tagValue == "-1")
                    {
                        return
                            new ImageSourceConverter().ConvertFromString(
                                "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/Red.png");
                    }
                }
                else
                {
                    if (tagValue == "1")
                    {
                        return
                            new ImageSourceConverter().ConvertFromString(
                                "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/Circle.png");
                    }
                    else if (tagValue == "0")
                    {
                        return
                            new ImageSourceConverter().ConvertFromString(
                                "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/Triangle.png");

                    }
                    else if (tagValue == "-1")
                    {
                        return
                            new ImageSourceConverter().ConvertFromString(
                                "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/Diamond.png");
                    }
                }
            }
            else
            {
                if (tagValue == "1")
                {
                    return
                        new ImageSourceConverter().ConvertFromString(
                            "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/UpArrow.png");
                }
                else if (tagValue == "0")
                {
                    return
                        new ImageSourceConverter().ConvertFromString(
                            "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/RightArrow.png");

                }
                else if (tagValue == "-1")
                {
                    return
                        new ImageSourceConverter().ConvertFromString(
                            "/Syncfusion.OlapChart.Silverlight;component/Images/KPI/DownArrow.png");
                }
            }

            return new System.Windows.Media.Imaging.BitmapImage();
        }

        #endregion
    } 

    #endregion
}
