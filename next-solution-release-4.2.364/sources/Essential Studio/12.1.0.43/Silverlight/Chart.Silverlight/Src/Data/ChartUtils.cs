#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// IChartTransformer interface
    /// </summary>
    /// <exclude/>
    public interface IChartTransformer
    {
        /// <summary>
        /// Gets the viewport.
        /// </summary>
        /// <value>The viewport.</value>
        /// <exclude/>
        Rect Viewport { get; }

        /// <summary>
        /// Transforms chart cordinates to real coordinates.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>Type : Point</returns>
        /// <exclude/>
        Point TransformToVisible(double x, double y);
        /// <summary>
        /// Transform chartCoordinates to real co ordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="series"></param>
        /// <returns></returns>
        Point TransformToVisible(double x, double y, ChartSeries series);

        /// <summary>
        /// Method declaration for GetCenterpoint
        /// </summary>
        /// <returns></returns>
        Point GetCenterPoint();
        /// <summary>
        /// Method implementation for GetMinimumValue
        /// </summary>
        /// <returns></returns>
        double GetMinimumValue();
    }

    /// <summary>
    /// ChartTransform class
    /// </summary>
    /// <exclude/>
    public static class ChartTransform
    {
        /// <summary>
        /// Creates the Cartesian transformer.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="series">The series.</param>
        /// <returns>Type : IChartTransformer</returns>
        public static IChartTransformer CreateCartesian(Rect viewport, ChartSeries series)
        {
            return new ChartCartesianTransformer(viewport, series);
        }

        /// <summary>
        /// Creates the Cartesian transformer.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>Type : IChartTransformer</returns>
        public static IChartTransformer CreateSimple(Rect viewport)
        {
            return new ChartSimpleTransformer(viewport);
        }

        #region Internal types
        /// <summary>
        /// ChartSimpleTransformer class
        /// </summary>
        private class ChartSimpleTransformer : IChartTransformer
        {
            private Rect m_viewport = Rect.Empty;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChartSimpleTransformer"/> class.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            public ChartSimpleTransformer(Rect viewport)
            {
                m_viewport = viewport;
            }

            /// <summary>
            /// Gets the viewport.
            /// </summary>
            /// <value>The viewport.</value>
            public Rect Viewport
            {
                get
                {
                    return m_viewport;
                }
            }

            public Point TransformToVisible(double x, double y, ChartSeries series)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Transforms chart cordinates to real coordinates.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            /// <returns>Type : Point</returns>
            public Point TransformToVisible(double x, double y)
            {
                return new Point(x, y);
            }

            public Point GetCenterPoint()
            {
                return new Point(m_viewport.Width / 2, m_viewport.Height / 2);
            }

            public double GetMinimumValue()
            {
                return (m_viewport.Width > m_viewport.Height) ? m_viewport.Height : m_viewport.Width;
            }
            #region Members

            #endregion

            #region Properties

            #endregion

            #region Constructor

            #endregion

            #region Public methods

            #endregion

            #region IChartTransformer Members
            #endregion
        }

        /// <summary>
        /// Main class for ChartCartesianTransformer
        /// </summary>
        private class ChartCartesianTransformer : IChartTransformer
        {
            private Rect m_viewport = Rect.Empty;
            internal DoubleRange m_XRange = DoubleRange.Empty;
            internal DoubleRange m_YRange = DoubleRange.Empty;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChartCartesianTransformer"/> class.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            /// <param name="series">The series.</param>
            public ChartCartesianTransformer(Rect viewport, ChartSeries series)
            {
                m_viewport = viewport;
            }

            /// <summary>
            /// Gets the viewport.
            /// </summary>
            /// <value>The viewport.</value>
            public Rect Viewport
            {
                get
                {
                    return m_viewport;
                }
            }

            public Point TransformToVisible(double x, double y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Transforms chart cordinates to real coordinates.
            /// </summary>
            /// <param name="x">The x value.</param>
            /// <param name="y">The y value.</param>
            /// <param name="series"></param>
            /// <returns>Type : Point</returns>
            public Point TransformToVisible(double x, double y, ChartSeries series)
            {
                Point point = new Point();
                ChartAxesType axesType = series.Area != null ? series.Area.GetSeriesAxesType(series.Type) : ChartAxesType.CartesianAxes;
                if (series.IsRotated(series.Type))
                {
                    if (series.Area != null && series.Area.SecondaryAxis != null && series.Area.SecondaryAxis.m_enableBreaks)
                    {
                        point.X = Math.Round(m_viewport.Width * series.YAxis.ValueToCoefficient(x));
                        double xVal = y;
                        double xrangeDiff = Math.Round((series.XAxis.Range.End - series.XAxis.Range.Start));
                        point.Y = m_viewport.Height * (1 - ((1 / (xrangeDiff)) * xVal));
                    }
                    else
                    {
                        double yVal = x;
                        double yrangeDiff = (series.YAxis.Range.End - series.YAxis.Range.Start);
                        point.X = Math.Round(m_viewport.Width / (yrangeDiff) * yVal);
                        //point.X = m_viewport.Width / (series.YAxis.Range.End - series.YAxis.Range.Start) * x;

                        double xVal = y;
                        double xrangeDiff = (series.XAxis.Range.End - series.XAxis.Range.Start);
                        point.Y = Math.Round(m_viewport.Height * (1 - ((1 / (xrangeDiff)) * xVal)));//m_viewport.Height * (1 - ((1 / (series.XAxis.Range.End - series.XAxis.Range.Start)) * y));
                    }
                }
                else if (axesType == ChartAxesType.None)
                {
                    double diff = (series.maximum - series.minimum);
                    diff = diff == 0 ? 1 : diff;
                    point.X = m_viewport.Width / diff * x;
                    point.Y = m_viewport.Height * (1 - ((1 / (diff)) * y));
                }
                else if (axesType == ChartAxesType.RadarAxes)
                {
                    double clockwise = 1;
                    if (!ChartRadarType.GetIsClockWise(series.Area))
                    {
                        clockwise = clockwise * -1;
                    }

                    double y1 = series.Area.centerPoint.Y - (series.Area.maxRadius * (1 - ((1 / (series.YAxis.Range.End - series.YAxis.Range.Start)) * ((series.YAxis.Range.End - series.YAxis.Range.Start) - y))));
                    Point endpoint = GeneralPointRotation(series.Area.centerPoint, new Point(series.Area.centerPoint.X, y1), x * (360 / (series.XAxis.Range.End - series.XAxis.Range.Start + 1)) * clockwise);
                    point = endpoint;
                }
                else if (axesType == ChartAxesType.PolarAxes)
                {
                    double clockwise = 1;
                    if (!ChartPolarType.GetIsClockWise(series.Area))
                    {
                        clockwise = clockwise * -1;
                    }

                    double y1 = series.Area.centerPoint.Y - (series.Area.maxRadius * (1 - ((1 / (series.YAxis.Range.End - series.YAxis.Range.Start)) * ((series.YAxis.Range.End - series.YAxis.Range.Start) - y))));
                    Point endpoint = GeneralPointRotation(series.Area.centerPoint, new Point(series.Area.centerPoint.X, y1), x * (360 / (series.XAxis.Range.End - series.XAxis.Range.Start + 1)) * clockwise);
                    point = endpoint;
                }
                else
                {
                    if (series.Area != null && series.Area.SecondaryAxis != null && series.Area.SecondaryAxis.m_enableBreaks)
                    {
                        double xVal = x;
                        point.X = Math.Round(m_viewport.Width * (xVal / (series.XAxis.Range.End - series.XAxis.Range.Start)));
                        double yVal = y;
                        point.Y = Math.Round(m_viewport.Height * (1 - (series.YAxis.ValueToCoefficient(yVal))));
                    }
                    else
                    {
                        double xVal = x;
                        double xrangeDiff = (series.XAxis.Range.End - series.XAxis.Range.Start);
                        point.X = Math.Round(m_viewport.Width / (xrangeDiff) * xVal);
                        //point.X = m_viewport.Width / (series.XAxis.Range.End - series.XAxis.Range.Start) * x;
                        double yVal = y;
                        double yrangeDiff = (series.YAxis.Range.End - series.YAxis.Range.Start);
                        point.Y = Math.Round(m_viewport.Height * (1 - ((1 / (yrangeDiff)) * yVal)));
                        //point.Y = m_viewport.Height * (1 - ((1 / (series.YAxis.Range.End - series.YAxis.Range.Start)) * y));
                    }
                }

                return point;
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

            public Point GetCenterPoint()
            {
                return new Point(m_viewport.Width / 2, m_viewport.Height / 2);
            }

            public double GetMinimumValue()
            {
                return (m_viewport.Width > m_viewport.Height) ? m_viewport.Height : m_viewport.Width;
            }

            #region Members

            #endregion

            #region Properties

            #endregion

            #region Constructor

            #endregion

            #region Public methods

            #endregion

            #region IChartTransformer Members
            #endregion
        }
        #endregion

        #region Implementation
        #endregion
    }

    /// <summary>
    /// Class implentation for ChartListDataConverter
    /// </summary>
    public class ChartListDataConveter : TypeConverter
    {
        private const string C_regex = "(?<x>[^ ,]+)([ ]*[ ,][ ]*)(([{](?<y>[^}]+)[}])|(?<y>[^ ,]+))";
        private const string C_regexSplitter = "[, ]+";
        private const string C_xGroup = "x";
        private const string C_yGroup = "y";

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
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
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
            string sData = value as string;
            if (sData != null)
            {
                ChartPointsCollection points = new ChartPointsCollection();
                MatchCollection mathes = Regex.Matches(sData, C_regex);
                foreach (Match match in mathes)
                {
                    if (match.Success)
                    {
                        double x = Convert.ToDouble(match.Groups[C_xGroup].Value, CultureInfo.InvariantCulture);
                        string[] yStrings = Regex.Split(match.Groups[C_yGroup].Value, C_regexSplitter);
                        double[] yValues = new double[yStrings.Length];
                        for (int i = 0; i < yValues.Length; i++)
                        {
                            yValues[i] = Convert.ToDouble(yStrings[i], CultureInfo.InvariantCulture);
                        }

                        points.Add(new ChartPoint(x, yValues));
                    }
                }

                return points;
            }

            return base.ConvertFrom(context, culture, value);
        }
        #region Constants

        #endregion

        #region Implementation

        #endregion
    }
    /// <summary>
    /// Class implementation for ChartYPointsConverter
    /// </summary>
    public class ChartYPointsConveter : TypeConverter
    {
        private const string C_regex = "(?<x>[^ ,]+)([ ]*[ ,][ ]*)(([{](?<y>[^}]+)[}])|(?<y>[^ ,]+))";
        private const string C_regexSplitter = "[, ]+";
        private const string C_xGroup = "x";
        private const string C_yGroup = "y";

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
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
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
            string sData = value as string;
            if (sData != null)
            {
                string[] yStrings = Regex.Split(sData, C_regexSplitter);
                double[] yValues = new double[yStrings.Length];
                for (int i = 0; i < yValues.Length; i++)
                {
                    yValues[i] = Convert.ToDouble(yStrings[i], CultureInfo.InvariantCulture);
                }

                return yValues;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    internal static class ChartLayoutUtils
    {
        private const double C_half = 0.5d;

        /// <summary>
        /// AddThickness of rect
        /// </summary>
        /// <param name="rect">rect is used for setting X and Y values.</param>
        /// <param name="thickness">this is used to set margin sizes</param>
        /// <returns>Type : Rect</returns>
        public static Rect AddThinckness(Rect rect, Thickness thickness)
        {
            rect.X -= thickness.Left;
            rect.Y -= thickness.Top;
            rect.Width += thickness.Left + thickness.Right;
            rect.Height += thickness.Top + thickness.Bottom;
            return rect;
        }

        /// <summary>
        /// AddThinckness of Size and Tickness
        /// </summary>
        /// <param name="size">size of viewport</param>
        /// <param name="thickness">thichness of margin</param>
        /// <returns>Type : Size</returns>
        public static Size AddThinckness(Size size, Thickness thickness)
        {
            size.Width += thickness.Left + thickness.Right;
            size.Height += thickness.Top + thickness.Bottom;

            return size;
        }

        /// <summary>
        /// Checks the members of size by infinity.
        /// </summary>
        /// <param name="size">The size is to set Width and Height.</param>
        /// <returns>Type : Size</returns>
        public static Size CheckSize(Size size)
        {
            size.Width = double.IsInfinity(size.Width) ? 0d : size.Width;
            size.Height = double.IsInfinity(size.Height) ? 0d : size.Height;
            return size;
        }

        /// <summary>
        /// Gets the rect by center.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="size">The size is to set Width and Height.</param>
        /// <returns>Type : Rect</returns>
        public static Rect GetRectByCenter(Point center, Size size)
        {
            return new Rect(center.X - (size.Width / 2), center.Y - (size.Height / 2), size.Width, size.Height);
        }

        /// <summary>
        /// Gets the rect by center.
        /// </summary>
        /// <param name="cx">The cx to calculate.</param>
        /// <param name="cy">The cy to calculate.</param>
        /// <param name="width">The width to calculate.</param>
        /// <param name="height">The height to calculate.</param>
        /// <returns>Type : Rect</returns>
        public static Rect GetRectByCenter(double cx, double cy, double width, double height)
        {
            return new Rect(cx - (width / 2), cy - (height / 2), width, height);
        }

        /// <summary>
        /// Gets the start point by.
        /// </summary>
        /// <param name="realSize">Size of the real.</param>
        /// <param name="fullRect">The full rect.</param>
        /// <param name="horizontal">The horizontal.</param>
        /// <param name="vertical">The vertical.</param>
        /// <returns>Type : Point</returns>
        public static Point GetStartPointBy(Size realSize, Rect fullRect, ChartAlignment horizontal, ChartAlignment vertical)
        {
            return new Point(fullRect.X + GetStartValueBy(realSize.Width, fullRect.Width, horizontal), fullRect.Y + GetStartValueBy(realSize.Height, fullRect.Height, vertical));
        }

        /// <summary>
        /// Gets the start point by.
        /// </summary>
        /// <param name="realSize">Size of the real.</param>
        /// <param name="fullSize">The full size.</param>
        /// <param name="horizontal">The horizontal.</param>
        /// <param name="vertical">The vertical.</param>
        /// <returns>Type : Point</returns>
        public static Point GetStartPointBy(Size realSize, Size fullSize, ChartAlignment horizontal, ChartAlignment vertical)
        {
            return new Point(GetStartValueBy(realSize.Width, fullSize.Width, horizontal), GetStartValueBy(realSize.Height, fullSize.Height, vertical));
        }

        /// <summary>
        /// Gets the start value by.
        /// </summary>
        /// <param name="realSize">Size of the real.</param>
        /// <param name="fullSize">The full size.</param>
        /// <param name="alignment">The alignment.</param>
        /// <returns>Type : double</returns>
        public static double GetStartValueBy(double realSize, double fullSize, ChartAlignment alignment)
        {
            double result = 0;
            if (alignment == ChartAlignment.Center)
            {
                result = 0.5 * (fullSize - realSize);
            }
            else if (alignment == ChartAlignment.Far)
            {
                result = fullSize - realSize;
            }

            return result;
        }

        /// <summary>
        /// Subtracts the thinckness.
        /// </summary>
        /// <param name="rect">The rect to set X and Y values.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns>Type : Rect</returns>
        public static Rect SubtractThinckness(Rect rect, Thickness thickness)
        {
            rect.X += thickness.Left;
            rect.Y += thickness.Top;
            if (rect.Width > thickness.Left + thickness.Right)
            {
                rect.Width -= thickness.Left + thickness.Right;
            }

            if (rect.Height > (thickness.Top + thickness.Bottom))
            {
                rect.Height -= thickness.Top + thickness.Bottom;
            }

            return rect;
        }

        /// <summary>
        /// Subtracts the thinckness.
        /// </summary>
        /// <param name="size">The size to calculate.</param>
        /// <param name="thickness">The thickness to calculate.</param>
        /// <returns>Type : Size</returns>
        public static Size SubtractThinckness(Size size, Thickness thickness)
        {
            size.Width = Math.Max(size.Width - thickness.Left - thickness.Right, 0);
            size.Height = Math.Max(size.Height - thickness.Top - thickness.Bottom, 0);
            return size;
        }

        #region Constants

        #endregion
    }

    /// <summary>
    /// Class implementation for ChartConvert
    /// </summary>
    public class ChartConvert
    {
        /// <summary>
        /// Return ChartPointcollectin from the given List values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static ChartPointsCollection ToChartData(List<double> values)
        {
            ChartPointsCollection pointscollction = new ChartPointsCollection();
            if (values.Count < 2 || values.Count % 2 != 0)
            {
                return null;
            }

            for (int i = 0; i < values.Count; i += 2)
            {
                pointscollction.Add(new ChartPoint(values[i], values[i + 1]));
            }

            return pointscollction;
        }

        /// <summary>
        /// Method implementation for to return ChartPointsCollection from given String values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static ChartPointsCollection ToChartData(string values)
        {
            ChartPointsCollection pointscollction = new ChartPointsCollection();
            string[] datas = values.Split(',');
            double x, y;
            int i;
            if (datas.Length > 2)
            {
                for (i = 0; i < datas.Length; i += 2)
                {
                    try
                    {
                        x = Convert.ToDouble(datas[i]);
                        if (i + 1 != datas.Length)
                        {
                            y = Convert.ToDouble(datas[i + 1]);
                        }
                        else
                        {
                            y = 0;
                        }

                        pointscollction.Add(new ChartPoint(x, y));
                    }
                    catch (FormatException)
                    {
                        ////throw new FormatException(ex.Message);
                    }
                }
            }
            else if (datas.Length == 2)
            {
                x = Convert.ToDouble(datas[0]);
                if (datas[1] == "")
                {
                    y = 0;
                }
                else
                {
                    y = Convert.ToDouble(datas[1]);
                }

                pointscollction.Add(new ChartPoint(x, y));
            }
            else
            {
                pointscollction.Add(new ChartPoint(1, 0));
            }

            return pointscollction;
        }
    }
    /// <summary>
    /// Class implementation for ChartMath 
    /// </summary>
    public static class ChartMath
    {
        /// <summary>
        /// Return double value based on given value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="LogarithmicBase"></param>
        /// <returns></returns>
        public static double ValueToLogValue(double value, double LogarithmicBase)
        {
            double result = double.NaN;
            //if (this.IsLogarithmic)
            //{
            //double logDelta = Range.Delta; //LogarithmicVisibleRange.Delta;
            //double logStart = Range.Start; //LogarithmicVisibleRange.Start;
            if (value > 0)
            {
                result = Math.Log(value, LogarithmicBase); //- logStart) / logDelta;
                return result;
            }
            else
                return result = 0.0;
            //}
            //else
            //    return value;
        }

        /// <summary>
        /// Return Point value based on given values
        /// </summary>
        /// <param name="originpoint"></param>
        /// <param name="endpoint"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
            endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
            endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
            endpoint.X += originpoint.X;
            endpoint.Y += originpoint.Y;
            return endpoint;
        }

        /// <summary>
        /// Gets minimal value from <c>value</c> or <c>min</c> and maximal from <c>value</c> or <c>max</c>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <returns>The MinMax value</returns>
        public static double MinMax(double value, double min, double max)
        {
            return value > max ? max : (value < min ? min : value);
        }
    }

    /// <summary>
    /// Class implementation for DataBinding
    /// </summary>
    public class DataBinding
    {
        
        //public static List<double> CombinePropertyData(List<double> xdata, List<double> ydata)
        //{
        //    int i;
        //    List<double> seriesdata = new List<double>();
        //    for (i = 0; i < xdata.Count && i < ydata.Count; i++)
        //    {
        //        seriesdata.Add(xdata[i]);
        //        seriesdata.Add(ydata[i]);
        //    }

        //    return seriesdata;
        //}

        /// <summary>
        /// Get the list of object from the given collections
        /// </summary>
        /// <returns>
        /// List of double value
        /// </returns>
        public static List<double> GetPropertyData(IEnumerable bindingsource, string property)
        {
            List<double> propertyData = new List<double>();
            int count = 0;
            double noofDatas = 0d;
            foreach (object obj in bindingsource)
            {
                Type sourceType = obj.GetType();
                TypeDelegator currentsourceObj = new TypeDelegator(sourceType);
                PropertyInfo propinfo = currentsourceObj.GetProperty(property);
                if (propinfo == null)
                {
                    ////return null;
                    return new List<double>();
                }

                object data = propinfo.GetValue(obj, null);
                if (data == null)
                {
                    return null;
                }

                double doubleval=0;
                DateTime date=new DateTime();
                bool canconvertDouble = double.TryParse(data.ToString(), out doubleval);
                bool canconvertDate = DateTime.TryParse(data.ToString(), out date);

                if (canconvertDouble)
                {
                    propertyData.Add(doubleval);
                }
                else if (canconvertDate)
                {
                    propertyData.Add(date.ToOADate());
                }
                else
                {
                    noofDatas++;
                    propertyData.Add(noofDatas);
                }

                count++;
            }

            return propertyData;
        }

        /// <summary>
        /// Get the XBinding Data alone for the Histogram chart type.
        /// </summary>
        /// <param name="bindingsource">Collection of Data</param>
        /// <param name="xproperty">Property name</param>
        /// <returns>ChartPoints Collection</returns>
        public static ChartPointsCollection GetXBindingData(IEnumerable bindingsource, string xproperty)
        {
            ChartPointsCollection datapoints = new ChartPointsCollection();
            double noofXDatas = 0d;
            foreach (object obj in bindingsource)
            {
                double xpoint;
                Type sourceType = obj.GetType();
                TypeDelegator currentsourceObj = new TypeDelegator(sourceType);
                PropertyInfo propinfo = currentsourceObj.GetProperty(xproperty);
                if (propinfo == null)
                {
                    return null;
                }

                object xdata = propinfo.GetValue(obj, null);
                double xdoubleval = 0d;
                DateTime xdate = new DateTime();
                bool canconvertDoubleX = double.TryParse(xdata.ToString(), out xdoubleval);
                bool canconvertDateX = DateTime.TryParse(xdata.ToString(), out xdate);
                if (canconvertDoubleX)
                {
                    xpoint = xdoubleval;
                }
                else if (canconvertDateX)
                {
                    xpoint = xdate.ToOADate();
                }
                else
                {
                    noofXDatas++;
                    xpoint = noofXDatas;
                }

                datapoints.Add(new ChartPoint(xpoint, 0));
            }

            ////if (datapoints.Count == 0)
            ////{
            ////    return null;
            ////}

            return datapoints;
        }

       
        //public static ChartPointsCollection GetDataBinding(IEnumerable bindingsource, string xproperty, List<string> yproperties)
        //{
        //    ChartPointsCollection datapoints = new ChartPointsCollection();
        //    double noofXDatas = 0d, noofYDatas = 0d;
        //    foreach (object obj in bindingsource)
        //    {
        //        double xpoint;
        //        List<double> ypoint = new List<double>();
        //        Type sourceType = obj.GetType();
        //        TypeDelegator currentsourceObj = new TypeDelegator(sourceType);
        //        PropertyInfo propinfo = currentsourceObj.GetProperty(xproperty);
        //        if (propinfo == null)
        //        {
        //            return null;
        //        }

        //        object xdata = propinfo.GetValue(obj, null);
        //        if (xdata == null)
        //        {
        //            return null;
        //        }

        //        double xdoubleval = 0d;
        //        DateTime xdate = new DateTime();
        //        bool canconvertDoubleX = double.TryParse(xdata.ToString(), out xdoubleval);
        //        bool canconvertDateX = DateTime.TryParse(xdata.ToString(), out xdate);
        //        if (canconvertDoubleX)
        //        {
        //            xpoint = xdoubleval;
        //        }
        //        else if (canconvertDateX)
        //        {
        //            xpoint = xdate.ToOADate();
        //        }
        //        else
        //        {
        //            noofXDatas++;
        //            xpoint = noofXDatas;
        //        }

        //        foreach (string str in yproperties)
        //        {
        //            PropertyInfo propinfo1 = currentsourceObj.GetProperty(str);
        //            if (propinfo1 == null)
        //            {
        //                return null;
        //            }

        //            object ydata = propinfo1.GetValue(obj, null);
        //            if (ydata == null)
        //            {
        //                return null;
        //            }

        //            double ydoubleval = 0d;
        //            DateTime ydate = new DateTime();
        //            bool canconvertDoubleY = double.TryParse(ydata.ToString(), out ydoubleval);
        //            bool canconvertDateY = DateTime.TryParse(ydata.ToString(), out ydate);
        //            if (canconvertDoubleY)
        //            {
        //                ypoint.Add(ydoubleval);
        //            }
        //            else if (canconvertDateY)
        //            {
        //                ypoint.Add(ydate.ToOADate());
        //            }
        //            else
        //            {
        //                noofYDatas++;
        //                ypoint.Add(noofYDatas);
        //            }
        //        }

        //        datapoints.Add(new ChartPoint(xpoint, ypoint.ToArray()));
        //    }

        //    ////if (datapoints.Count == 0)
        //    ////{
        //    ////    return null;
        //    ////}

        //    for (int i=0;i<datapoints.Count;i++)
        //    {
        //        if (datapoints[i] == null)
        //        {
        //            datapoints.RemoveAt(i);
        //        }
        //    }

        //    return datapoints;
        //}

        /// <summary>
        /// Used to Get the property type in Binding DataSource collection.
        /// </summary>
        /// <returns>
        /// Type value
        /// </returns>
        public static Type GetPropertyType(IEnumerable bindingsource, string property)
        {
            if (bindingsource == null || property == null)
            {
                return null;
            }

            object firstdata = new object();
            foreach (object obj in bindingsource)
            {
                firstdata = obj;
                break;
            }

            Type sourceType = firstdata.GetType();
            TypeDelegator currentsourceObj = new TypeDelegator(sourceType);
            PropertyInfo propinfo = currentsourceObj.GetProperty(property);
            if (propinfo == null)
            {
                return null;
            }

            return (Type)propinfo.PropertyType;
        }

        /// <summary>
        /// Get the ordered content for chart Axis labels on Binding
        /// </summary>
        /// <returns>
        /// List of Objects
        /// </returns>
        public static List<object> GetOrderedContentData(ChartSeries series, ChartAxis axis)
        {
            List<double> positionproperty = axis.AxisDataPosition;
            List<object> contentproperty = axis.AxisDataContents;
            double start = axis.ActualVisibleRange.Start; //axis.Range.Start;
            double end = axis.ActualVisibleRange.End; //axis.Range.End;
            double interval = axis.VisibleInterval;

            List<object> result = new List<object>();
            if (start == double.MinValue)
            {
                start = end = 0;
            }

            interval = axis.VisibleInterval;
            if (double.IsNaN(axis.firstinterval) == false)// && series.XAxis.ValueType!=ChartValueType.DateTime)
            {
                interval = axis.firstinterval;
            }

            ////interval = ((Math.Ceiling(start) - start)!=0)?Math.Ceiling(start) - start:interval;

            //for (double i = start; i <= end; i += interval)
            //{
            //    if (i != axis.Range.Start)
            //    {
            //        interval = axis.m_visibleInterval;
            //    }

            //    finished = false;
            //    for (int j = 1; j < positionproperty.Count - 1; j++)
            //    {
            //        if (i == positionproperty[j] && j < contentproperty.Count)
            //        {
            //            result.Add(contentproperty[j]);
            //            finished = true;
            //            break;
            //        }
            //    }

            //    if (finished == false)
            //    {
            //        result.Add((object)"");
            //    }
            //}

            //For Custom Chart Axis Panel
            axis.ActualLabels = (from label in axis.AxisDataPosition where label <= axis.Range.End && label % axis.VisibleInterval != 0 && axis.ActualLabelsSourceSet == true orderby label ascending select new ChartAxisLabel(label, axis.AxisDataContents[axis.AxisDataPosition.IndexOf(label)]));
            ////result.Insert(result.Count, (object)"");
            return result;
        }

        /// <summary>
        /// Return collection of Object values from the given ChartAxis
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static List<object> GetOrderedContentData(ChartAxis axis)
        {
            List<double> positionproperty = axis.AxisDataPosition;
            List<object> contentproperty = axis.AxisDataContents;
            double start = axis.ActualVisibleRange.Start > -1 ? axis.ActualVisibleRange.Start : 0; //axis.Range.Start;
            double end = axis.ActualVisibleRange.End; //axis.Range.End;
            double interval = axis.ActualVisibleInterval;

            List<object> result = new List<object>();
            bool finished = false;
            if (start == double.MinValue)
            {
                start = end = 0;
            }

            interval = axis.ActualVisibleInterval;
            if (double.IsNaN(axis.firstinterval) == false)// && series.XAxis.ValueType!=ChartValueType.DateTime)
            {
                interval = axis.firstinterval;
            }

            ////interval = ((Math.Ceiling(start) - start)!=0)?Math.Ceiling(start) - start:interval;

            for (double i = start; i <= end && contentproperty!=null; i += interval)
            {
                if (i != axis.ActualVisibleRange.Start)
                {
                    interval = axis.ActualVisibleInterval;
                }

                finished = false;
                for (int j = 1; j < positionproperty.Count - 1; j++)
                {
                    if (i == positionproperty[j] && j < contentproperty.Count)
                    {
                        result.Add(contentproperty[j]);
                        finished = true;
                        break;
                    }
                }

                if (finished == false)// && i != -1)
                {
                    result.Add((object)"");
                }
            }

            //For showing last labels irrespective of the intervals
            if (axis.AxisDataContents.Count > 0 && axis.ShowEdgeLabels)
                result.Add(axis.AxisDataContents[axis.AxisDataContents.Count - 1]);

            //For Custom Chart Axis Panel
            //axis.ActualLabels = (from label in axis.AxisDataPosition where label <= axis.Range.End && label % axis.m_visibleInterval != 0 && axis.ActualLabelsSourceSet == true orderby label ascending select new ChartAxisLabel(label, axis.AxisDataContents[axis.AxisDataPosition.IndexOf(label)]));
            ////result.Insert(result.Count, (object)"");
            return axis.Isindexedseries ? contentproperty : result;
        }

        /// <summary>
        /// Return Collection of object from the given ChartSeries
        /// </summary>
        /// <param name="series"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static List<object> GetPropertyDataAsObject(ChartSeries series, string property)
        {
            if (series != null && series.DataModel != null && series.Data.Count > 0)
            {
                if (series.DataModel.ContentPath != property)
                {
                    series.DataModel.ContentPath = property;
                }

                return series.Data.Select(point => point.AxisContent).Cast<object>().ToList();
            }
            return null;
        }

        /// <summary>
        /// Return collection of double value from the given ChartSeries
        /// </summary>
        /// <param name="series"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static List<double> GetPropertyData(ChartSeries series, string property)
        {
            if (series != null && series.DataModel != null && series.Data.Count > 0)
            {
                if (series.DataModel.ContentPath != property)
                {
                    series.DataModel.ContentPath = property;
                }

                return series.Data.Select(point => point.AxisPosition).Cast<double>().ToList();
            }
            return null;
        }

        /// <summary>
        /// Get the Binding data property values
        /// </summary>
        /// <returns>
        /// List of Objects
        /// </returns>
        public static List<object> GetPropertyDataAsObject(IEnumerable bindingsource, string property)
        {
            List<object> propertyData = new List<object>();
            foreach (object obj in bindingsource)
            {
                Type sourceType = obj.GetType();
                TypeDelegator currentsourceObj = new TypeDelegator(sourceType);
                if (property == null)
                {
                    propertyData.Add((object)obj.GetType().ToString());
                    continue;
                }

                PropertyInfo propinfo = currentsourceObj.GetProperty(property);
                if (propinfo == null)
                {
                    propertyData.Add((object)obj.GetType().ToString());
                    continue;
                }

                object data = propinfo.GetValue(obj, null);

                DateTime date=new DateTime();
                if (GetPropertyType(bindingsource, property) == typeof(DateTime) && DateTime.TryParse(data.ToString(), out date))
                {
                    propertyData.Add((object)date.ToOADate());
                }
                else
                {
                    propertyData.Add(data);
                }
            }
            
            //// For Blank Data for First and Last Axis Values
            string str, str1;
            str = ""; 
            str1 = "";
            propertyData.Insert(0, (object)str);
            propertyData.Insert(propertyData.Count, (object)str1);
            return propertyData;
        }
    }

    /// <summary>
    /// Class implementation for ContextMenucommand
    /// </summary>
    public class ContextMenuCommand : ICommand
    {
        private Predicate<object> canExecute;
        private Action<object> method;

        /// <summary>
        /// Called when instance created for ContextMenuCommand
        /// </summary>
        /// <param name="method"></param>
        public ContextMenuCommand(Action<object> method)
            : this(method, null)
        {
        }


        /// <summary>
        /// Called when instance created for ContextMenuCommand with two arguments
        /// </summary>
        /// <param name="method"></param>
        /// <param name="canExecute"></param>
        public ContextMenuCommand(Action<object> method, Predicate<object> canExecute)
        {
            this.method = method;
            this.canExecute = canExecute;
        }


        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }

            return this.canExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public void Execute(object parameter)
        {
            this.method.Invoke(parameter);
        }
#pragma warning disable 0067
        /// <summary>
        /// Create event for CanExecuteChanged
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
    }
}
