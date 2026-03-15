#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Reflection;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartPathsConverter
    /// </summary>
    public class ChartPathsConverter : TypeConverter
    {
        #region Constants
        private const string C_pathGroup = "path";
        private const string C_regex = @"[\[](?<path>.+)?[\]]|(?<path>[^, ]+)";
        #endregion

        #region Implementation
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                IList<string> paths = new List<string>();
                MatchCollection matchPaths = Regex.Matches(value as string, C_regex);

                foreach (Match match in matchPaths)
                {
                    if (match.Success)
                    {
                        paths.Add(match.Groups[C_pathGroup].Value);
                    }
                }

                return paths;
            }

            return base.ConvertFrom(context, culture, value);
        }
        #endregion
    }


    #region Series Adornment Conterver
    /// <summary>
    /// Rotate the AdonmentLabel in the Given RotationAngle value.
    /// </summary>
    public class RotateAdornmentLabel : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CompositeTransform rt = null;
            List<object> data = (List<object>)parameter;
            ContentControl p = (ContentControl)data[0];
            ChartSeries series = (ChartSeries)data[1];
            if (p != null)
            {
                rt = !(p.RenderTransform is CompositeTransform) ? new CompositeTransform() : p.RenderTransform as CompositeTransform;
            }

            rt.CenterX = p.DesiredSize.Width / 2;
            rt.CenterY = p.DesiredSize.Height / 2;
            rt.Rotation = (double)values;
            if (series.Area.SecondaryAxis.IsInversed && series.Area.PrimaryAxis.IsInversed)
            {
                rt.Rotation = 180 + (double)values;
            }
            else if (series.ScaleX < 0)
            {
                if (series.Type == ChartTypes.Gantt || series.Type == ChartTypes.StackingBar || series.Type == ChartTypes.Bar || series.Type == ChartTypes.StackingBar100 || series.Type == ChartTypes.Tornado)
                {
                    rt.ScaleY = -1;
                }
                else
                {
                    rt.ScaleX = -1;
                }

            }
            else if (series.ScaleY < 0)
            {
                if (series.Type == ChartTypes.Gantt || series.Type == ChartTypes.StackingBar || series.Type == ChartTypes.Bar || series.Type == ChartTypes.StackingBar100 || series.Type == ChartTypes.Tornado)
                {
                    rt.ScaleX = -1;
                }
                else
                {
                    rt.ScaleY = -1;
                }
            }
            return rt;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Position the Adonrment label Based on the Rotated value
    /// </summary>
    public class PositionAdornmentLabel : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AdornmentsPosition val = (AdornmentsPosition)values;
            List<object> data = (List<object>)parameter;
            double newvalue = (double)data[2];
            ContentControl connector = (ContentControl)data[4];
            ChartSeries series = (ChartSeries)data[3];
            double angle = (double)data[5];
            Line l = new Line();
            l.X1 = l.Y1 = 0;
            l.X2 = connector.DesiredSize.Width + 10;
            l.Y2 = 2;
            Point p = Rotation.GeneralPointRotation(new Point(l.X1, l.Y1), new Point(l.X2, l.Y2), angle);

            if (series.Type == ChartTypes.Column && val == AdornmentsPosition.Bottom && series.YAxis.Origin == series.YAxis.VisibleRange.Start)
            {
                if (series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Center)
                {
                    newvalue = (double)data[2] - (double)data[0] / 2 - series.AdornmentsInfo.SymbolHeight / 2;
                }
                if (series.AdornmentsInfo.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    newvalue = (double)data[2] - (double)data[0] - series.AdornmentsInfo.SymbolHeight;
                }

            }

            if ((series.Type == ChartTypes.Pie || series.Type == ChartTypes.Doughnut)
                && series.AdornmentsInfo.SegmentIsOut == true && series.AdornmentsInfo.SegmentShowLine == true)
            {
                newvalue = newvalue + p.Y;
            }
            if(series.Type != ChartTypes.RangeColumn)
            {
               if (newvalue < 0)
               {
                 newvalue = newvalue + (double)data[0];
               }
               if (newvalue > series.Area.seriesGrid.ActualHeight)
               {
                 newvalue = newvalue - (double)data[0];
               }
            }
            else if(series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.TopAndBottom)
            {
              if (newvalue == 0)
               {
                 newvalue = newvalue + (double)data[0];
               }
              if (newvalue == series.Area.seriesGrid.ActualHeight)
               {
                 newvalue = newvalue - (double)data[0];
               }
            }
            return newvalue;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// This converter is used to add the X position values based on the Connector line template width.
    /// </summary>
    public class XPositionAdornmentLabel : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AdornmentsPosition val = (AdornmentsPosition)values;
            List<object> data = (List<object>)parameter;
            double newvalue = (double)data[0];
            ContentControl connector = (ContentControl)data[2];
            ChartSeries series = (ChartSeries)data[1];
            double angle = (double)data[3];
            Line l = new Line();
            l.X1 = l.Y1 = 0;
            l.X2 = connector.DesiredSize.Width + 10;
            l.Y2 = 2;
            Point p = Rotation.GeneralPointRotation(new Point(l.X1, l.Y1), new Point(l.X2, l.Y2), angle);
            Size size = connector.DesiredSize;
            if ((series.Type == ChartTypes.Pie || series.Type == ChartTypes.Doughnut) && series.AdornmentsInfo.SegmentIsOut == true
                && series.AdornmentsInfo.SegmentShowLine == true)
            {
                newvalue = newvalue + p.X;
            }
            else if ((series.Type == ChartTypes.Pyramid || series.Type == ChartTypes.Funnel) && series.AdornmentsInfo.SegmentIsOut == true
                && series.AdornmentsInfo.SegmentShowLine == true)
            {
                switch (series.AdornmentsInfo.SegmentHorizontalAlignment)
                {
                    case HorizontalAlignment.Right:
                        newvalue = newvalue + p.X + size.Width;
                        break;
                    case HorizontalAlignment.Left:
                        newvalue = newvalue - p.X - size.Width;
                        break;
                }
            }

            return newvalue;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// This converter is used to identify the segment label data based on the SegmentLabel property value changed
    /// </summary>
    public class SegmentLabelConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartAdornment data = parameter as ChartAdornment;
            LabelContent val = (LabelContent)values;
            switch (val)
            {
                case LabelContent.XValue:
                    data.SegmentLabel = data.m_point.X.ToString(data.Series.AdornmentsInfo.SegmentLabelFormat);
                    break;
                case LabelContent.YValue:
                    data.SegmentLabel = data.DataPoint.Y.ToString(data.Series.AdornmentsInfo.SegmentLabelFormat);
                    break;
                case LabelContent.YofTot:
                    data.SegmentLabel = data.DataPoint.Y.ToString(data.Series.AdornmentsInfo.SegmentLabelFormat) + " of " + data.Series.sum;
                    break;
                case LabelContent.Percentage:
                    data.SegmentLabel = ((data.DataPoint.Y / data.Series.sum) * 100d).ToString(data.Series.AdornmentsInfo.SegmentLabelFormat) + " %";
                    break;
                case LabelContent.DateTime:
                    DateTime date = DateTime.FromOADate(data.m_point.X);
                    data.SegmentLabel = date.ToString(data.Series.AdornmentsInfo.SegmentLabelDataTimeFormat);
                    break;
                case LabelContent.LabelContentPath:
                    Type t = data.GetType();
                    if (data.Series.AdornmentsInfo.LabelContentPath != string.Empty)
                    {
                        string[] properties = data.Series.AdornmentsInfo.LabelContentPath.Split('.', '[', ']');
                        int res = 0;
                        object result = string.Empty;
                        object obj = data.DataPoint;
                        if (properties[0].Equals("DataPoint"))
                        {
                            Type sourceType = obj.GetType();
                            TypeDelegator currentsourceObj = new TypeDelegator(sourceType);
                            PropertyInfo propinfo = currentsourceObj.GetProperty(properties[1]);
                            if (properties[1].Equals("Values") && int.TryParse(properties[2], out res) == true)
                            {
                                result = ((double[])propinfo.GetValue(obj, null))[res].ToString();
                            }
                            else if (properties[1].Equals("Tag"))
                            {
                                if (properties.Count() != 3)
                                {
                                    result = data.Series.DataModel.ChartPoints[data.index].Tag;
                                }
                                else
                                {
                                    result = DataBinding.GetPropertyDataAsObject(data.Series.DataSource, properties[2])[data.index + 1];
                                }
                            }
                            else
                            {
                                switch (data.Series.Type)
                                {
                                    case ChartTypes.Gantt:
                                    case ChartTypes.HiLo:
                                    case ChartTypes.HiLoOpenClose:
                                    case ChartTypes.RangeColumn:
                                    case ChartTypes.Candle:
                                    case ChartTypes.Histogram:
                                        propinfo = currentsourceObj.GetProperty("Values");
                                        result = ((double[])propinfo.GetValue(obj, null))[data.adornemntLabelIndex].ToString();
                                        break;
                                    default:
                                        result = ((double)propinfo.GetValue(obj, null)).ToString(data.Series.AdornmentsInfo.SegmentLabelFormat);
                                        break;
                                }
                            }
                        }
                        else if (properties[0].Equals("Series"))
                        {
                            Type sourceType = data.Series.GetType();
                            TypeDelegator currentsourceObj = new TypeDelegator(sourceType);
                            PropertyInfo propinfo = currentsourceObj.GetProperty(properties[1]);
                            result = (propinfo != null && propinfo.GetValue(data.Series, null) != null) ? propinfo.GetValue(data.Series, null).ToString() : string.Empty;
                        }

                        data.SegmentLabel = result;
                    }
                    else
                    {
                        data.SegmentLabel = string.Empty;
                    }

                    break;
                default:
                    data.SegmentLabel = data.DataPoint.Y.ToString();
                    break;
            }

            //HorizontalAlignment temp = data.Series.AdornmentsInfo.HorizontalAlignment;
            //data.Series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Stretch;
            //data.Series.AdornmentsInfo.HorizontalAlignment = temp;
            return data.SegmentLabel;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Used to intimate the Actual locaton after the data Align horizontally
    /// </summary>
    public class AdornmentHorizontalConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<object> data = (List<object>)parameter;
            double result = (double)data[1];
            Size availablesize = (Size)data[3];
            HorizontalAlignment val = (HorizontalAlignment)values;
            ChartAdornment adorn = (ChartAdornment)data[4];
            ChartTypes ctype = (ChartTypes)data[5];
            switch (val)
            {
                case HorizontalAlignment.Left:
                    if (ctype == ChartTypes.Gantt || ctype == ChartTypes.Tornado || ctype == ChartTypes.HiLoOpenClose)
                    {
                        if (adorn.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        {
                            if (adorn.adornemntLabelIndex <= 1 && ctype != ChartTypes.HiLoOpenClose)
                            {
                                if (adorn.DataPoint.Values[adorn.adornemntLabelIndex] <= Math.Min(adorn.DataPoint.Values[0], adorn.DataPoint.Values[1]))
                                {
                                    result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                }
                                else
                                {
                                    result = (double)data[1] + ((double)data[2] / 2);
                                }
                            }
                            else
                            {
                                if (adorn.DataPoint.Values[adorn.adornemntLabelIndex] >= Math.Max(adorn.DataPoint.Values[2], adorn.DataPoint.Values[3]))
                                {
                                    result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                }
                                else
                                {
                                    result = (double)data[1] + ((double)data[2] / 2);
                                }
                            }
                        }
                        else
                        {
                            result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                        }
                    }
                    else if (ctype == ChartTypes.Bar || ctype == ChartTypes.StackingBar)
                    {
                        if (adorn.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                        {
                            if (adorn.DataPoint.Values[0] >= 0)
                            {
                                result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                            }
                            else
                            {
                                result = (double)data[1] + ((double)data[2] / 2);
                            }
                        }                       
                        else
                        {
                            result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                        }
                    }
                    else
                    {
                        result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                    }
                    break;
                case HorizontalAlignment.Right:
                    if (ctype == ChartTypes.Gantt || ctype == ChartTypes.Tornado || ctype == ChartTypes.HiLoOpenClose)
                    {
                        if (adorn.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        {
                            if (adorn.adornemntLabelIndex <= 1 && ctype != ChartTypes.HiLoOpenClose)
                            {
                                if (adorn.DataPoint.Values[adorn.adornemntLabelIndex] <= Math.Min(adorn.DataPoint.Values[0], adorn.DataPoint.Values[1]))
                                {
                                    result = (double)data[1] + ((double)data[2] / 2);
                                }
                                else
                                {
                                    result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                }
                            }
                            else
                            {
                                if (adorn.DataPoint.Values[adorn.adornemntLabelIndex] >= Math.Max(adorn.DataPoint.Values[2], adorn.DataPoint.Values[3]))
                                {
                                    result = (double)data[1] + ((double)data[2] / 2);
                                }
                                else
                                {
                                    result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                }
                            }
                        }
                        else
                        {
                            result = (double)data[1] + ((double)data[2] / 2);
                        }
                    }
                    else if (ctype == ChartTypes.Bar || ctype == ChartTypes.StackingBar)
                    {
                        if (adorn.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                        {
                            if (adorn.DataPoint.Values[0] >= 0)
                            {
                                result = (double)data[1] + ((double)data[2] / 2);
                            }
                            else
                            {
                                result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                            }
                        }                       
                        else
                        {
                            result = (double)data[1] + ((double)data[2] / 2);
                        }
                    }
                    else
                    {
                        result = (double)data[1] + ((double)data[2] / 2);
                    }
                    break;
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    if ((double)data[1] >= ((Size)data[3]).Width - ((double)data[0] / 2))
                    {
                        result = (double)((Size)data[3]).Width;
                    }
                    else
                    {
                        result = (double)data[1] - ((double)data[0] / 2);
                    }
                    break;

            }

            ////return result;
            ////return (result < 0) ? 0 : result;
            if (result >= availablesize.Width - ((double)data[0] / 2))
            {
                result = availablesize.Width - ((double)data[2] / 2 + (double)data[0]);
            }
            return (result < 0) ? (0 + (double)data[2] / 2) : result;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Used to identify the new location after the Vertical alignment get changed.
    /// </summary>
    public class AdornmentVerticalConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<object> data = (List<object>)parameter;
            double result = (double)data[1];
            VerticalAlignment val = (VerticalAlignment)values;
            Size availableSize = (Size)data[3];
            ChartAdornment adorn = (ChartAdornment)data[4];
            ChartTypes ctype = (ChartTypes)data[5];
            if (!((((double)data[1] - (double)data[0] - ((double)data[2] / 2)) <= 0) || (((double)data[1] + ((double)data[2] / 2)) >= (availableSize.Height - (double)data[0])) || (((double)data[1] - ((double)data[0] / 2)) >= (availableSize.Height - (double)data[0]))))
            {
                switch (val)
                {
                    case VerticalAlignment.Top:
                        if (adorn != null)
                        {

                            if (ctype == ChartTypes.RangeColumn || ctype == ChartTypes.HiLo || ctype == ChartTypes.HiLoOpenClose)
                            {

                                if (adorn.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom )
                                {
                                    if (adorn.adornemntLabelIndex <= 1)
                                    {
                                        if (adorn.DataPoint.Values[adorn.adornemntLabelIndex] >= Math.Max(adorn.DataPoint.Values[0], adorn.DataPoint.Values[1]))
                                        {
                                            result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                        }
                                        else
                                        {
                                            result = (double)data[1] + ((double)data[2] / 2);
                                        }
                                    }
                                    else
                                    {
                                        if (adorn.DataPoint.Values[adorn.adornemntLabelIndex] >= Math.Max(adorn.DataPoint.Values[2], adorn.DataPoint.Values[3]))
                                        {
                                            result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                        }
                                        else
                                        {
                                            result = (double)data[1] + ((double)data[2] / 2);
                                        }
                                    }
                                }
                                else
                                {
                                    result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                }
                            }
                            else if (ctype == ChartTypes.Column || ctype == ChartTypes.StackingColumn)
                            {
                                if (adorn.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                {
                                    if (adorn.DataPoint.Values[0] >= 0)
                                    {
                                        result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                    }
                                    else
                                    {                                       
                                        result = (double)data[1] + ((double)data[2] / 2);
                                    }
                                }
                                else
                                {
                                    result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                }
                            }
                            else
                            {
                                result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                            }
                        }
                        break;
                    case VerticalAlignment.Bottom:
                        if (ctype == ChartTypes.RangeColumn || ctype == ChartTypes.HiLo || ctype == ChartTypes.HiLoOpenClose)
                        {
                            if (adorn.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            {
                                if (adorn.adornemntLabelIndex <= 1)
                                {
                                    if (adorn.DataPoint.Values[adorn.adornemntLabelIndex] >= Math.Max(adorn.DataPoint.Values[0], adorn.DataPoint.Values[1]))
                                    {
                                        result = (double)data[1] + ((double)data[2] / 2);
                                    }
                                    else
                                    {
                                        result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                    }
                                }
                                else
                                {
                                    if (adorn.DataPoint.Values[adorn.adornemntLabelIndex] >= Math.Max(adorn.DataPoint.Values[2], adorn.DataPoint.Values[3]))
                                    {
                                        result = (double)data[1] + ((double)data[2] / 2);
                                    }
                                    else
                                    {
                                        result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                    }
                                }
                            }
                            else
                            {
                                result = (double)data[1] + ((double)data[2] / 2);
                            }
                        }
                        else if (ctype == ChartTypes.Column || ctype == ChartTypes.StackingColumn)
                        {
                            if (adorn.Series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            {
                                if (adorn.DataPoint.Values[0] >= 0)
                                {
                                    result = (double)data[1] + ((double)data[2] / 2);
                                }
                                else
                                {
                                    result = (double)data[1] - (double)data[0] - ((double)data[2] / 2);
                                }
                            }
                            else
                            {
                                result = (double)data[1] + ((double)data[2] / 2);
                            }
                        }
                        else
                        {
                            result = (double)data[1] + ((double)data[2] / 2);
                        }
                        break;
                    case VerticalAlignment.Center:
                    case VerticalAlignment.Stretch:
                        result = (double)data[1] - ((double)data[0] / 2);
                        break;
                }

            }            ////return (result < 0) ? 0 : result;
            if (result >= availableSize.Height - ((double)data[0]))
            {
                result = result - (double)data[0];
            }
            return (result < 0) ? ((double)data[2] / 2) : result;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Position the Symbol in the Chart Area based on the Symbol height
    /// </summary>
    public class SymbolHeightConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double actualposition = (double)parameter;
            double height = (double)values;
            return actualposition - (height / 2);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Position the Symbol in the Chart Area based on the Symbol width
    /// </summary>
    public class SymbolWidthConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double actualposition = (double)parameter;
            double width = (double)values;
            ////return actualposition - (width / 2);
            return actualposition - (width / 2);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Used to specify the Connector template visiblity.  If ShowSegment Line property is true then Visible
    /// </summary>
    public class ConnectorVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = false;
            ChartSeries val = (ChartSeries)parameter;
            if ((val.Type == ChartTypes.Pie || val.Type == ChartTypes.Doughnut || val.Type == ChartTypes.Pyramid || val.Type == ChartTypes.Funnel) && val.AdornmentsInfo.SegmentIsOut)
            {
                result = true;
            }

            return ((bool)values && result) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Rotate the Connectory template for point out the Adonrnments of chart area.
    /// </summary>
    public class ConnectorRotateConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RotateTransform rt = new RotateTransform();
            List<object> data = (List<object>)parameter;
            ContentControl p = (ContentControl)data[0];
            double d = Canvas.GetTop(p);
            rt.CenterX = p.ActualWidth / 2;
            rt.CenterY = p.ActualHeight / 2;
            rt.Angle = (double)data[1];
            return rt;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }
    #endregion
    #region SymbolHeightWidthConvertor

    /// <summary>
    /// Class implementation for SymbolHeightWidthConvertor
    /// </summary>
    public class SymbolHeightWidthConvertor : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChartSeries series = (ChartSeries)parameter;
            if (series.Type == ChartTypes.Funnel || series.Type == ChartTypes.Pyramid || series.Type == ChartTypes.Pie || series.Type == ChartTypes.Doughnut)
            {
                return 0;
            }
            return (double)value;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    #endregion

    #region Area Related Converter
    /// <summary>
    /// Class implementation for AreatypeConverter
    /// </summary>
    public class AreaTypeConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartTypes val = (ChartTypes)values;
            ChartArea area = (ChartArea)parameter;
            ////if (val == ChartTypes.Pie || val==ChartTypes.Doughnut || val==ChartTypes.Pyramid)
            ////{
            ////    return ChartAxesType.None;
            ////}
            ////else
            ////{
            ////    return ChartAxesType.CartesianAxes;
            ////}
            return area.GetSeriesAxesType(val);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Return bool value for Visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values"></param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)values;
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility val = (Visibility)value;
            return val == Visibility.Visible ? true : false;
        }
    }

    /// <summary>
    /// Return Bool value from the Visibility value 
    /// </summary>
    public class VisibilityToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility val = (Visibility)values;
            return val == Visibility.Visible ? true : false;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            return val ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    /// <summary>
    /// Class implementation for EnableEffectWidthConverter
    /// </summary>
    public class EnableEffectWidthConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double width = (double)value;
            if (width < 10)
            {
                return 0;
            }
            else
            {
                width = 10;
                return width;
            }
           
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }


    /// <summary>
    /// Return String value for WaterMarkContent
    /// </summary>
    public class WatermarkContentConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value != null)
            {
                ChartArea area = value as ChartArea;
                switch ((string)parameter)
                {
                    case "Text":
                        {
                            if (area.WatermarkType == WatermarkTypes.Text)
                                return area.m_watermarkText;
                            else
                                return string.Empty;
                        }
                    case "Image":
                        {
                            if (area.WatermarkType == WatermarkTypes.Image)
                                return area.m_watermarkimageSource;
                            else
                                return null;
                        }
                }
            }
            return null;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Return Alignment mode for WatermarkAlignment
    /// </summary>
    public class WatermarkAlignmentConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                ChartArea area = value as ChartArea;
                switch ((string)parameter)
                {
                    case "AlignmentX":
                        switch (area.WatermarkAlignmentX)
                        {
                            case AlignmentX.Center:
                                return HorizontalAlignment.Center;
                            case AlignmentX.Left:
                                return HorizontalAlignment.Left;
                            case AlignmentX.Right:
                                return HorizontalAlignment.Right;
                        }
                        break;
                    case "AlignmentY":
                        switch (area.WatermarkAlignmentY)
                        {
                            case AlignmentY.Center:
                                return VerticalAlignment.Center;
                            case AlignmentY.Top:
                                return VerticalAlignment.Top;
                            case AlignmentY.Bottom:
                                return VerticalAlignment.Bottom;
                        }
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }
    #endregion
}
