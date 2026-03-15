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
using System.Reflection;
using System.Collections;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.Foundation;
using Windows.Data.Xml.Dom;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains utility methods to manipulate data.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartDataUtils
    {
        internal static object GetPropertyDescriptor(object obj, string path)
        {
            IPropertyAccessor propertyAccessor = null;
            if (path.Contains(".") || path.Contains("["))
            {
                if (path.Contains("."))
                {
                    string[] childProperties = path.Split('.');
                    int i = 0;
                    object parentObj = obj;
                    while (i != childProperties.Length)
                    {
                        var xPropertyInfo = parentObj.GetType().GetTypeInfo().GetDeclaredProperty(childProperties[i]);
                        if (xPropertyInfo != null)
                            propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                        object Val = propertyAccessor.GetValue(parentObj);
                        if (i == childProperties.Length - 1)
                        {
                            return Val;
                        }
                        i++;
                    }
                }
                else if (path.Contains("["))
                {
                    int index = Convert.ToInt32(path.Substring(path.IndexOf('[') + 1, path.IndexOf(']') - path.IndexOf('[') - 1));
                    string tempPath = path.Replace(path.Substring(path.IndexOf('[')), string.Empty);
                    var propertyInfo = obj.GetType().GetTypeInfo().GetDeclaredProperty(tempPath);
                    if (propertyInfo != null)
                        propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                    object Val = propertyAccessor.GetValue(obj);
                    IList array = Val as IList;
                    if (array != null && array.Count > index)
                        return array[index];
                }
            }
            else if ((obj.GetType() == typeof(DictionaryEntry)) || (obj.GetType().ToString().Contains("KeyValuePair")))
            {

                var propertyInfo = obj.GetType().GetTypeInfo().GetDeclaredProperty("Value");
                if (propertyInfo != null)
                    propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                object valueObj = propertyAccessor.GetValue(obj);

                if (valueObj != null && path != "Key")
                {
                    var propertyInfo1 = valueObj.GetType().GetTypeInfo().GetDeclaredProperty(path);
                    if (propertyInfo1 != null)
                        propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo1);
                    return propertyAccessor.GetValue(valueObj);
                }
                else
                {
                    var propertyInfo2 = obj.GetType().GetTypeInfo().GetDeclaredProperty(path);
                    if (propertyInfo2 != null)
                        propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo2);
                    return propertyAccessor.GetValue(obj);
                }
            }
            else
            {
                var propertyInfo = obj.GetType().GetTypeInfo().GetDeclaredProperty(path);
                if (propertyInfo != null)
                    propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                return propertyAccessor.GetValue(obj);
            }
            return null;
        }

        ///<summary>
        /// Gets the object by path.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>Returns the object</returns>
        public static object GetObjectByPath(object obj, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
#if WINDOWS_PHONE
                 try
                    {
                        return GetPropertyDescriptor(obj, path);

                    }
                    catch
                    {
                        return null;
                    }
#else
                if (obj is XmlElement)
                {
                    obj = ((XmlElement)obj).GetAttribute(path);
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
#endif
            }
            return obj;
        }


        /// <summary>
        /// Converts to double.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>The double value</returns>
        /// <seealso cref="ChartDataUtils"/>
        public static int ConvertPathObjectToPositionValue(object obj)
        {
            int value = 0;
            try
            {
                value = Convert.ToInt32(obj);
            }
            catch
            {
                value = Int32.MinValue;
            }
            return value;
        }

        /// <summary>
        /// Gets the double by path.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>The double value</returns>
        public static double GetPositionalPathValue(object obj, string path)
        {

            return ConvertPathObjectToPositionValue(GetObjectByPath(obj, path));
        }

    }
    /// <summary>
    /// Custom comaprer to compare the chart points by x-value.
    /// </summary>
    public class PointsSortByXComparer : Comparer<Point>
    {
        #region Members
        /// <summary>
        /// Initializes diff
        /// </summary>
        private double diff;
        #endregion

        /// <summary>
        /// Compares the specified p1 with the specified p2.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>
        /// negative value if point1 &lt; point2
        /// <para>
        /// zero if point1 = point2.
        /// </para>
        /// <para>
        /// positive value if point1 &gt; point2
        /// </para>
        /// </returns>
        public override int Compare(Point point1,Point point2)
        {
            diff = point1.X - point2.X;
            if (diff == 0)
            {
                return 0;
            }

            return diff < 0 ? -1 : 1;
        }
    }
}
