#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Collections;

#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotGrid
#else
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Class that holds the generic method GetParentITem
    /// </summary>

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class Utils
    {
        /// <summary>
        /// Static method to get the parent item of the control
        /// </summary>
        /// <typeparam name="T">Generic Class</typeparam>
        /// <param name="child">Dependency object</param>
        /// <returns>generic type</returns>
        public static T GetParentItem<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);
            if (dependencyObject != null)
            {
                T parent = dependencyObject as T;
                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    return GetParentItem<T>(dependencyObject);
                }
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Class that holds the commonly utilizing methods and propeties
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Gets the size of the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>size of the text</returns>
        public static Size GetTextSize(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
#if SILVERLIGHT
            return new Size(textBlock.ActualWidth, textBlock.ActualHeight);
#else
            return textBlock.DesiredSize;
#endif
        }

        /// <summary>
        /// Gets the font weight from input string.
        /// </summary>
        /// <param name="fontWeight">The font weight.</param>
        /// <returns>font weight</returns>
        public static FontWeight GetFontWeightFromString(string fontWeight)
        {
            if (fontWeight.ToLower().Equals("normal"))
            {
                return FontWeights.Normal;
            }
            if (fontWeight.ToLower().Equals("bold"))
            {
                return FontWeights.Bold;
            }
            if (fontWeight.ToLower().Equals("extrabold"))
            {
                return FontWeights.ExtraBold;
            }
            if (fontWeight.ToLower().Equals("black"))
            {
                return FontWeights.Black;
            }
            if (fontWeight.ToLower().Equals("extrablack"))
            {
                return FontWeights.ExtraBlack;
            }
            if (fontWeight.ToLower().Equals("extralight"))
            {
                return FontWeights.ExtraLight;
            }
            if (fontWeight.ToLower().Equals("light"))
            {
                return FontWeights.Light;
            }
            if (fontWeight.ToLower().Equals("medium"))
            {
                return FontWeights.Medium;
            }
            if (fontWeight.ToLower().Equals("semibold"))
            {
                return FontWeights.SemiBold;
            }
            if (fontWeight.ToLower().Equals("thin"))
            {
                return FontWeights.Thin;
            }

            return FontWeights.Normal;
        }

        /// <summary>
        /// Gets the color from Hexadecimal string.
        /// </summary>
        /// <param name="hexaColor">Hexadecimal format string for a Color object.</param>
        /// <returns>Brush</returns>
        public static SolidColorBrush GetColorFromHexaDecimal(string hexaColor)
        {
            if (hexaColor.Length == 9)
            {
                return new SolidColorBrush(
                                Color.FromArgb(
                                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                                )
                            );
            }

            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte("ff", 16),
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16)
                )
            );
        }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child">The child.</param>
        /// <returns>parent element</returns>
        public static T GetParentElement<T>(DependencyObject child) where T : DependencyObject
        {
            if (child != null)
            {
                DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);
                T parent = dependencyObject as T;
                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    return GetParentElement<T>(dependencyObject);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the visual child.
        /// </summary>
        /// <typeparam name="childItem">The type of the child item.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>child item</returns>
        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


        /// <summary>
        /// Finds the visual child.
        /// </summary>
        /// <typeparam name="childItem">The type of the child item.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="name">Name of child item</param>
        /// <returns>child item</returns>
        public static childItem FindVisualChildWithName<childItem>(DependencyObject obj,string name)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {               
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem && child.GetName() == name)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChildWithName<childItem>(child, name);
                    if (childOfChild != null && childOfChild.GetName() == name)
                        return childOfChild;
                }
            }
            return null;
        }
        /// <summary>
        /// Method to get the geometry of given path
        /// </summary>
        /// <param name="data">string</param>
        /// <returns>PathGeometry</returns>
        public static PathGeometry GetPathGeometry(string data)
        {
            PathGeometry pg = new PathGeometry();
            PathSegmentCollection psc = new PathSegmentCollection();
            PathFigure pf = new PathFigure();
            PathFigureCollection pfc = new PathFigureCollection();
            data = data.Replace("M ", " M").Replace("C ", " C").Replace("L ", " L");
            string[] str = data.Split(' ');
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].StartsWith("C") || str[i].StartsWith("c"))
                {
                    string[] item = str[i].Split(',');
                    string[] item1 = str[i + 1].Split(',');
                    string[] item2 = str[i + 2].Split(',');
                    BezierSegment bs = new BezierSegment();
                    bs.Point1 = new Point(double.Parse(item[0].Substring(1), System.Globalization.CultureInfo.InvariantCulture), double.Parse(item[1], System.Globalization.CultureInfo.InvariantCulture));
                    bs.Point2 = new Point(double.Parse(item1[0], System.Globalization.CultureInfo.InvariantCulture), double.Parse(item1[1], System.Globalization.CultureInfo.InvariantCulture));
                    bs.Point3 = new Point(double.Parse(item2[0], System.Globalization.CultureInfo.InvariantCulture), double.Parse(item2[1], System.Globalization.CultureInfo.InvariantCulture));
                    i += 2;
                    psc.Add(bs);
                }
                else
                    if (str[i].StartsWith("L") || str[i].StartsWith("l"))
                    {
                        string[] item = str[i].Split(',');
                        LineSegment ls = new LineSegment();
                        ls.Point = new Point(double.Parse(item[0].Substring(1), System.Globalization.CultureInfo.InvariantCulture), double.Parse(item[1], System.Globalization.CultureInfo.InvariantCulture));
                        psc.Add(ls);
                    }
                    else
                        if (str[i].StartsWith("M") || str[i].StartsWith("m"))
                        {
                            string[] item = str[i].Split(',');
                            pf.StartPoint = new Point(double.Parse(item[0].Substring(1), System.Globalization.CultureInfo.InvariantCulture), double.Parse(item[1], System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else if (str[i].StartsWith("z") || str[i].StartsWith("Z"))
                        {
                            pf.IsClosed = true;
                        }
            }
            pf.Segments = psc;
            pfc.Add(pf);
            pg.Figures = pfc;
            return pg;
        }

        /// <summary>
        /// Returns an array of enumeration constants of respective type
        /// </summary>
        /// <param name="type">Enumeration type</param>
        /// <returns>Array of Enumeration Constants</returns>        
        public static Array GetValues(Type type)
        {
            Type enumType = type;
            System.Reflection.FieldInfo[] fields = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Array enumArray = Array.CreateInstance(enumType, fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                object obj = fields[i].GetValue(null);
                enumArray.SetValue(obj, i);
            }
            return enumArray;
        }
    }
    /// <summary>
    /// Specifies the type of AutoSize options to be used in PivotGrid
    /// </summary>
    public enum GridAutoSizeOption
    {
        /// <summary>
        /// All Rows will be resized
        /// </summary>
        All,
        /// <summary>
        /// None of the rows will be resized
        /// </summary>
        None,
        /// <summary>
        /// The value specified in AutoSizeRowCount will be resized
        /// </summary>
        FixedCount,
        /// <summary>
        /// The Grand Total rows will be used to determine column size.
        /// </summary>
        TotalRows,

        /// <summary>
        /// The visible range only will be resized.
        /// </summary>
        VisibleRange
    }
    /// <summary>
    /// Class that holds the member which retrives the name of the dependency elment
    /// </summary>
    public static class DependencyObjectExtension
    {
        /// <summary>
        /// Gets the name of the dependency element
        /// </summary>
        /// <param name="dependencyObject">The dependency element</param>
        /// <returns>string</returns>
        public static string GetName(this DependencyObject dependencyObject)
        {
            if (dependencyObject is FrameworkElement)
            {
                FrameworkElement element = dependencyObject as FrameworkElement;
                return element.Name;
            }
            else
                return null;
        }
    }
    /// <summary>
    /// Class used for sorting the objects in reverse
    /// </summary>
    public class ReverseCustomComparer : IComparer
    {
        IComparer comparer = null;
        /// <summary>
        /// Initializes the <see cref="ReverseCustomComparer"/> class.
        /// </summary>
        /// <param name="comparer">The comparer</param>
        public ReverseCustomComparer(IComparer comparer)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// Gets the custom comparer set before sorting applied
        /// </summary>
        public IComparer CustomComparer
        {
            get { return comparer; }
        }
        /// <summary>
        /// Gets the compared result
        /// </summary>
        /// <param name="x">Object</param>
        /// <param name="y">Object</param>
        /// <returns>int</returns>
        public int Compare(object x, object y)
        {
            return -comparer.Compare(x, y);
        }
    }

    /// <summary>
    /// Class used for sorting the objects in reverse order
    /// </summary>
    public class ReverseOrderComparer : IComparer
    {
        #region IComparer Members
        /// <summary>
        /// Method to compare two objects
        /// </summary>
        /// <param name="x">object</param>
        /// <param name="y">object</param>
        /// <returns>int</returns>
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
                return 0;
            else if (y == null)
                return 1;
            else if (x == null)
                return -1;
            else
                return -x.ToString().CompareTo(y.ToString());
        }

        #endregion
    }
}
