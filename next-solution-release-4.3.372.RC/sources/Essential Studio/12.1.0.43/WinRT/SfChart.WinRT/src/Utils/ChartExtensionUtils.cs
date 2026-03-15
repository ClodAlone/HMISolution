#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using System.Reflection;
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains Chart extension methods.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public static class ChartExtensionUtils
    {
        internal static DateTime ValidateNonWorkingDate(this DateTime date, string days, bool isToBack)
        {
            var i = 0;
            var isNonWorkingDay = false;
            do
            {
                isNonWorkingDay = false;
                if (!days.Contains(date.DayOfWeek.ToString()))
                {
                    i++;
                    isNonWorkingDay = true;
                    date = date.AddDays(isToBack ? -2 : 2);
                    break;
                }
            } while (isNonWorkingDay);
            return date;
        }

        internal static DateTime ValidateNonWorkingHours(this DateTime date, double startTime, double endTime, bool isToBack)
        {

            var time = date.TimeOfDay.TotalHours;
            if (isToBack)
            {
                if (time < startTime)
                {
                    date = date.AddDays(-1);//.AddHours(-(endTime - (time - startTime)));
                    date = new DateTime(date.Year, date.Month, date.Day, (int)(endTime - (startTime - time)), date.Minute, date.Second);
                }
                else if (time > endTime)
                {
                    date = date.AddHours(-(time - endTime));
                }
            }
            else
            {
                if (time < startTime)
                {
                    date = date.AddHours(startTime - time);
                }
                else if (time > endTime)
                {
                    date = date.AddHours((24 - time) + startTime + time - endTime);
                }
            }
            return date;
        }

#if !WPF
        public static bool IntersectsWith(this Rect rect, Rect other)
        {
            if (other.Bottom < rect.Top || other.Right < rect.Left
             || other.Top > rect.Bottom || other.Left > rect.Right)
            {
                return false;
            }
            return true;
        }
#endif

#if !WPF
        public static Rect Offset(this Rect rect, double x, double y)
        {
            return new Rect(rect.X + x, rect.Y + y, rect.Width - x, rect.Height - y);
        }
#endif

        internal static List<Vector3D> Get3DVector(this List<Point> points, double depth)
        {
            var vector3D = new List<Vector3D>();
            foreach (var point in points)
            {
                vector3D.Add(new Vector3D(point, depth));
            }
            return vector3D;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
           (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source != null ? source.Where(element => seenKeys.Add(keySelector(element))) : null;
        }

        private static DateTime BaseDate = new DateTime(1899, 12, 30);

        /// <summary>
        /// Converts the value of this instance to the equivalent OLE Automation date.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public static double ToOADate(this DateTime time)
        {
            return time.Subtract(BaseDate).TotalDays;
        }

        /// <summary>
        /// Returns a DateTime equivalent to the specified OLE Automation Date.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public static DateTime FromOADate(this Double value)
        {
            return BaseDate.AddDays(value);
        }

#if NETFX_CORE

        internal static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.SetMethod;
        }

        internal static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod;
        }

#endif

        /// <summary>
        /// Returns sum of DoubleRange
        /// </summary>
        /// <param name="ranges">Collection of DoubleRange</param>
        /// <returns></returns>
        public static DoubleRange Sum(this IEnumerable<DoubleRange> ranges)
        {
            var sum = DoubleRange.Empty;
            IEnumerator<DoubleRange> enumerator = ranges.GetEnumerator();
            while (enumerator.MoveNext())
            {
                sum += enumerator.Current;
            }
            return sum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static ChartSeriesBase Max(this IEnumerable<ChartSeriesBase> source, Func<ChartSeriesBase, double> selector)
        {
            var chartSeries = source as ChartSeriesBase[] ?? source.ToArray();
            if (chartSeries.Any())
            {
                ChartSeriesBase maxObject = chartSeries[0];
                double maxVal = selector(maxObject);
                for (int i = 0; i < chartSeries.Count(); i++)
                {
                    double value = selector(chartSeries[i]);
                    if (value > maxVal)
                    {
                        maxVal = value;
                        maxObject = chartSeries[i];
                    }
                }

                return maxObject;
            }
            return null;
        }
    }

    internal static class ClearUIElementProperties
    {
        internal static void ClearUIValues(this Shape element)
        {
            if (element is Line)
            {
                element.ClearValue(Line.X1Property);
                element.ClearValue(Line.X2Property);
                element.ClearValue(Line.Y1Property);
                element.ClearValue(Line.Y2Property);
            }
            else if (element is Rectangle)
            {
                element.ClearValue(Rectangle.WidthProperty);
                element.ClearValue(Rectangle.HeightProperty);
            }
            else if (element is Ellipse)
            {
                element.ClearValue(Ellipse.WidthProperty);
                element.ClearValue(Ellipse.HeightProperty);
            }
        }
    }

#if WINDOWS_PHONE7
    public class HashSet<T> : ICollection<T>
    {
        private readonly Dictionary<T, bool> data;

        public HashSet()
        {
            data = new Dictionary<T, bool>();
        }

        public IEqualityComparer<T> Comparer
        {
            get { return data.Comparer; }
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool Add(T item)
        {
            if (data.ContainsKey(item))
                return false;

            data.Add(item, true);
            return true;
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(T item)
        {
            return data.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            if (data.ContainsKey(item))
                return false;

            data.Remove(item);
            return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return data.Keys.GetEnumerator();
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
#endif
}
