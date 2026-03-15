#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart.Olap
{
    public class PieLabelConvertor:IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartPoint chartPoint = value as ChartPoint;
            if (chartPoint!=null && !(chartPoint.Item is Double))
            {
               string PieLabel = ((DataPointInfoProvider)chartPoint.Item).Row;
               string PieValue = ((DataPointInfoProvider)chartPoint.Item).Value;
                return String.Format("{0}\n{1}", PieLabel, PieValue);
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class PieLegendConvertor : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartPieSegment pieSegment = value as ChartPieSegment;
            string PieLabel = string.Empty;
            if (pieSegment!=null && pieSegment.CorrespondingPoints != null && pieSegment.CorrespondingPoints[0].DataPoint.Item is DataPointInfoProvider)
            {
                PieLabel = ((DataPointInfoProvider)pieSegment.CorrespondingPoints[0].DataPoint.Item).Row;
                return String.Format("{0}", PieLabel);
            }
            else
            {
                PieLabel = "null";
                return String.Format("{0}", PieLabel);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class IEnumLegendConvertor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartPieSegment pieSegment = value as ChartPieSegment;
            string PieLabel = string.Empty;
            if ((pieSegment != null) && (pieSegment.Series!=null))
            {
                PieLabel = pieSegment.Series.Label;
                return String.Format("{0}", PieLabel);
            }
            else
            {
                PieLabel = "null";
                return String.Format("{0}", PieLabel);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
