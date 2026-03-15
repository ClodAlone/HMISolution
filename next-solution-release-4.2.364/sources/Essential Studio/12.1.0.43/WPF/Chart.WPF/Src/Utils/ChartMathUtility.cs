// <copyright file="ChartMathUtility.cs" company="Syncfusion">
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
    using System.ComponentModel;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Xml;
    using System.Security.Permissions;
    using System.Windows.Controls;
    using System.IO;
    using System.Windows.Xps.Packaging;
    using System.Windows.Xps;
    using System.IO.Packaging;
    using Syncfusion.Licensing;
    using System.Collections;

    /// <summary>
    /// Represents chart indexed range struct.
    /// </summary>
    /// <exclude/>
    public struct ChartIndexRange
    {
        #region Members
        /// <summary>
        /// Initializes m_start
        /// </summary>
        private int m_start;

        /// <summary>
        /// Initializes m_interval
        /// </summary>
        private int m_interval;

        /// <summary>
        /// Initializes m_end
        /// </summary>
        private int m_end;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the start of range.
        /// </summary>
        /// <value>The start.</value>
        public int Start
        {
            get { return m_start; }
        }

        /// <summary>
        /// Gets the range interval.
        /// </summary>
        /// <value>The interval.</value>
        public int Interval
        {
            get { return m_interval; }
        }

        /// <summary>
        /// Gets the endof range.
        /// </summary>
        /// <value>The end value.</value>
        public int End
        {
            get { return m_end; }
        }

        /// <summary>
        /// Gets the points count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return (m_end - m_start) / m_interval + 1;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartIndexRange"/> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end value.</param>
        /// <param name="interval">The interval.</param>
        public ChartIndexRange(int start, int end, int interval)
        {
            m_start = start;
            m_interval = interval == 0 ? 1 : interval;
            m_end = end;
        }
        #endregion
    }

    /// <summary>
    /// Represents class for specific chart mathematical values.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class ChartMath
    {
        #region Constants
        /// <summary>
        /// Initializes ToDegree
        /// </summary>
        public const double ToDegree = 180 / Math.PI;

        /// <summary>
        /// Initializes ToRadial
        /// </summary>
        public const double ToRadial = Math.PI / 180;

        /// <summary>
        /// Initializes Percent
        /// </summary>
        public const double Percent = 0.01d;

        /// <summary>
        /// Initializes DoublePI
        /// </summary>
        public const double DoublePI = 2 * Math.PI;

        /// <summary>
        /// Initializes HalfPI
        /// </summary>
        public const double HalfPI = 0.5 * Math.PI;

        /// <summary>
        /// Initializes OneAndHalfPI
        /// </summary>
        public const double OneAndHalfPI = 1.5 * Math.PI;
        #endregion

        #region Public methods
        /// <summary>
        /// Solves quadratic equation in form a*x^2 + b*x + c = 0
        /// </summary>
        /// <param name="a">The A component</param>
        /// <param name="b">The B component</param>
        /// <param name="c">The C component</param>
        /// <param name="root1">First root.</param>
        /// <param name="root2">Second root.</param>
        /// <returns>Bool value</returns>
        public static bool SolveQuadraticEquation(double a, double b, double c, out double root1, out double root2)
        {
            root1 = 0;
            root2 = 0;

            if (a != 0)
            {
                double d = b * b - 4 * a * c;

                if (d >= 0)
                {
                    double sd = Math.Sqrt(d);

                    root1 = (-b - sd) / (2 * a);
                    root2 = (-b + sd) / (2 * a);

                    return true;
                }
            }
            else if (b != 0)
            {
                root1 = -c / b;
                root2 = -c / b;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Solves the simultaneous equations in from a*x1 + b*x2 = c.
        /// </summary>
        /// <param name="a">The a parameters.</param>
        /// <param name="b">The b parameters.</param>
        /// <param name="c">The c parameters.</param>
        /// <returns>The Vector</returns>
        public static Vector SolveSimultaneousEquations(Vector a, Vector b, Vector c)
        {
            double d = a.X * b.Y - a.Y * b.X;
            double d1 = c.X * b.Y - c.Y * b.X;
            double d2 = a.X * c.Y - a.Y * c.X;

            return new Vector(d1 / d, d2 / d);
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

        /// <summary>
        /// Gets minimal value from parameters.
        /// </summary>
        /// <param name="values">The parameters</param>
        /// <returns>The minimal value.</returns>
        public static double Min(params double[] values)
        {
            double result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result = Math.Min(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets maximal value from parameters.
        /// </summary>
        /// <param name="values">The parameters</param>
        /// <returns>The maximal value.</returns>
        public static double Max(params double[] values)
        {
            double result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result = Math.Max(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets maximal value from parameter or zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The double value</returns>
        public static double MaxZero(double value)
        {
            return value > 0d ? value : 0d;
        }

        /// <summary>
        /// Gets minimal value from parameter or zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The MinZero value</returns>
        public static double MinZero(double value)
        {
            return value < 0d ? value : 0d;
        }

        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="div">The divider.</param>
        /// <param name="up">if set to <c>true</c> value will be rounded up.</param>
        /// <returns>The Round off value</returns>
        public static double Round(double x, double div, bool up)
        {
            return (int)(up ? Math.Ceiling(x / div) : Math.Floor(x / div)) * div;
        }
        #endregion
    }

    /// <summary>
    /// Represents chart layout utils class.
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal static class ChartLayoutUtils
    {
        #region Constants
        /// <summary>
        /// Initializes c_half
        /// </summary>
        private const double C_half = 0.5d;
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the center.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <returns>The vector center value</returns>
        public static Vector GetCenter(Size size)
        {
            return new Vector(C_half * size.Width, C_half * size.Height);
        }

        /// <summary>
        /// Gets the center.
        /// </summary>
        /// <param name="rect">The rect value.</param>
        /// <returns>The center point value</returns>
        public static Point GetCenter(Rect rect)
        {
            return Point.Add(rect.Location, GetCenter(rect.Size));
        }

        /// <summary>
        /// Gets the rect by center.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="size">The size value.</param>
        /// <returns>The Rect value</returns>
        public static Rect GetRectByCenter(Point center, Size size)
        {
            return new Rect(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
        }

        /// <summary>
        /// Gets the rect by center.
        /// </summary>
        /// <param name="cx">The cx value.</param>
        /// <param name="cy">The cy value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The Rect value</returns>
        public static Rect GetRectByCenter(double cx, double cy, double width, double height)
        {
            return new Rect(cx - width / 2, cy - height / 2, width, height);
        }

        /// <summary>
        /// Subtracts the thickness.
        /// </summary>
        /// <param name="rect">The rect value.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns>The Rectangle</returns>
        public static Rect Subtractthickness(Rect rect, Thickness thickness)
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
        /// Subtracts the thickness.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns>Returns the size</returns>
        public static Size Subtractthickness(Size size, Thickness thickness)
        {
            size.Width = Math.Max(size.Width - thickness.Left - thickness.Right, 0);
            size.Height = Math.Max(size.Height - thickness.Top - thickness.Bottom, 0);

            return size;
        }

        /// <summary>
        /// The Addthickness method
        /// </summary>
        /// <param name="rect">The Rect value</param>
        /// <param name="thickness">The thickness</param>
        /// <returns>The rectangle</returns>
        /// <seealso cref="ChartLayoutUtils"/>
        public static Rect Addthickness(Rect rect, Thickness thickness)
        {
            rect.X -= thickness.Left;
            rect.Y -= thickness.Top;
            rect.Width += thickness.Left + thickness.Right;
            rect.Height += thickness.Top + thickness.Bottom;

            return rect;
        }

        /// <summary>
        /// The Addthickness method
        /// </summary>
        /// <param name="size">The size value</param>
        /// <param name="thickness">The thickness value</param>
        /// <returns>Returns the size</returns>
        ///  <seealso cref="ChartLayoutUtils"/>
        public static Size Addthickness(Size size, Thickness thickness)
        {
            if(thickness.Left >=0 && thickness.Right >=0)
                size.Width += thickness.Left + thickness.Right;
            if(thickness.Top >=0 && thickness.Bottom >=0)
                size.Height += thickness.Top + thickness.Bottom;
            return size;
        }

        /// <summary>
        /// The GetUIElementBounds method
        /// </summary>
        /// <param name="element">The element</param>
        /// <returns>The Rect value</returns>
        public static Rect GetUIElementBounds(UIElement element)
        {
            return new Rect((Point)VisualTreeHelper.GetOffset(element), element.DesiredSize);
        }

        /// <summary>
        /// The CalcTransform method
        /// </summary>
        /// <param name="oldLoc">The old location</param>
        /// <param name="oldDir">The old direction</param>
        /// <param name="newLoc">The new location</param>      
        /// <param name="newDir">The new direction</param>  
        /// <returns>The matrix</returns>
        public static Matrix CalcTransform(Point oldLoc, Vector oldDir, Point newLoc, Vector newDir)
        {
            Matrix matrix = Matrix.Identity;

            matrix.Rotate(Math.Atan2(newDir.X - oldDir.X, newDir.Y - oldDir.Y));
            matrix.Translate(newLoc.X - oldLoc.X, newLoc.X - oldLoc.Y);

            return matrix;
        }

        /// <summary>
        /// Gets the start point by.
        /// </summary>
        /// <param name="realSize">Size of the real.</param>
        /// <param name="fullRect">The full rect.</param>
        /// <param name="horizontal">The horizontal.</param>
        /// <param name="vertical">The vertical.</param>
        /// <returns>The start point</returns>
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
        /// <returns>The start point</returns>
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
        /// <returns>The start value</returns>
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
        /// Checks the members of size by infinity.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <returns>Returns the size</returns>
        public static Size CheckSize(Size size)
        {
            size.Width = double.IsInfinity(size.Width) ? 0d : size.Width;
            size.Height = double.IsInfinity(size.Height) ? 0d : size.Height;

            return size;
        }
        #endregion
    }

    /// <summary>
    /// Represents chart data utils class.
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal static class ChartDataUtils
    {
        #region Implementation
        /// <summary>
        /// Gets the object by path.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>Returns the object</returns>
        public static object GetObjectByPath(object obj, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (obj is XmlElement)
                {
                    obj = ((XmlElement)obj).GetAttribute(path);
                }
                else if (obj is DataRow)
                {
                    obj = ((DataRow)obj)[path];
                }
                else
                {
                    try
                    {
                        return GetPropertyDescriptor(obj, path);

                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return obj;
        }


        /// <summary>
        /// returns the value of the given path.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static object GetPropertyDescriptor(object obj, string path)
        {
            PropertyDescriptor propDescriptor = null;
            if (path.Contains(".") || path.Contains("["))
            {
                if (path.Contains("."))
                {
                    string[] childProperties = path.Split('.');
                    int i = 0;
                    object parentObj = obj;
                    while (i != childProperties.Length)
                    {
                        propDescriptor = TypeDescriptor.GetProperties(parentObj)[childProperties[i]];
                        parentObj = propDescriptor.GetValue(parentObj);

                        if (i == childProperties.Length - 1)
                        {
                            return parentObj;
                        }
                        i++;
                    }
                }
                else if (path.Contains("[") )
                {
                    int index = Convert.ToInt32(path.Substring(path.IndexOf('[') + 1, path.IndexOf(']') - path.IndexOf('[') - 1));
                    string tempPath = path.Replace(path.Substring(path.IndexOf('[')), string.Empty);
                    propDescriptor = TypeDescriptor.GetProperties(obj)[tempPath];
                    object parentObj = obj;
                    parentObj = propDescriptor.GetValue(parentObj);
                    IList array = parentObj as IList;
                    if (array != null && array.Count > index)
                        return array[index];

                }
            }
            else if ((obj.GetType() == typeof(DictionaryEntry)) || (obj.GetType().ToString().Contains("KeyValuePair")))
            {
                propDescriptor = TypeDescriptor.GetProperties(obj)["Value"];
                object valueObj = propDescriptor.GetValue(obj);
                if (valueObj != null && path != "Key")
                {
                    propDescriptor = TypeDescriptor.GetProperties(valueObj)[path];
                    if (propDescriptor != null)
                        return propDescriptor.GetValue(valueObj);
                }
                else
                {
                    propDescriptor = TypeDescriptor.GetProperties(obj)[path];
                    return propDescriptor.GetValue(obj);
                }

            }
            else
            {
                propDescriptor = TypeDescriptor.GetProperties(obj)[path];
                if(propDescriptor!=null)
                return propDescriptor.GetValue(obj);
            }

            return null;
        }

        /// <summary>
        /// Converts to double.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>The double value</returns>
        /// <seealso cref="ChartDataUtils"/>
        public static double ConvertToDouble(object obj)
        {
            double value = 0;
            if (obj is DateTime)
            {
                value = ((DateTime)obj).ToOADate();
            }

            else if (obj is TimeSpan)
            {
                TimeSpan ts = (TimeSpan)obj;
                value = ts.TotalMilliseconds;
            }
            else
            {
                if (obj is IConvertible)
                {
                    try
                    {
                        value = ((IConvertible)obj).ToDouble(CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        value = double.NaN;
                    }
                }
                else
                {
                    value = double.NaN;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the double by path.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>The double value</returns>
        public static double GetDoubleByPath(object obj, string path)
        {
            return ConvertToDouble(GetObjectByPath(obj, path));
        }

       internal static ResourceDictionary rd;
        /// <summary>
        /// Resolves the segment template.
        /// </summary>
        /// <param name="segmentType">Type of the segment.</param>
        /// <returns>Corresponding <see cref="DataTemplate"/>.</returns>
        public static DataTemplate ResolveSegmentTemplate(Type segmentType)
        {
            rd = ChartDictionaries.GenericSeriesGUIDictionary;
            //ResourceDictionary rd = new SharedResourceDictionary()
            //{
            //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
            //};
            return rd[segmentType] as DataTemplate;
        }

        /// <summary>
        /// Resolves the segment data template by string.
        /// </summary>
        /// <param name="key">string data.</param>
        /// <returns>Corresponding <see cref="DataTemplate"/>.</returns>
        public static DataTemplate GetResourceByString(string key)
        {
             rd = ChartDictionaries.GenericSeriesGUIDictionary;
            //ResourceDictionary rd = new SharedResourceDictionary()
            //{
            //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
            //};
            return rd[key] as DataTemplate;
        }

        /// <summary>
        /// Clones the state of the target visual element.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <returns>Returns the Visual</returns>
        public static Visual CloneVisualState(Visual targetElement)
        {
            Visual retvalue = null;
            string memoryStreamUri = "memorystream://printstream";
            try
            {
                using (MemoryStream xpsStream = new MemoryStream())
                {
                    using (Package package = Package.Open(xpsStream, FileMode.Create))
                    {
                        Uri packageUri = new Uri(memoryStreamUri);
                        PackageStore.AddPackage(packageUri, package);
                        XpsDocument doc = new XpsDocument(package, CompressionOption.Fast, memoryStreamUri);
                        XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                        writer.Write(targetElement);
                        retvalue = doc.GetFixedDocumentSequence().DocumentPaginator.GetPage(0).Visual;
                        PackageStore.RemovePackage(packageUri);
                        doc.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retvalue;
        }
        #endregion
    }
}
