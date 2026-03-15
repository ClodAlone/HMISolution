#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !Orubase
namespace Syncfusion.Linq.Data
#else
namespace Orubase.Linq.Data
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Linq.Expressions;
#if !SILVERLIGHT
    using System.Data;
#endif

    public static class DataSetExtensions
    {
        public static string Predicate(this string thisString, string property, object value, FilterType filterType)
        {
            if (property.Contains(']'))
            {
                property = property.Replace("]", @"\]");
            }
            if (filterType == FilterType.Equals)
            {
                return thisString += Equals(property, value);
            }
            else if (filterType == FilterType.NotEquals)
            {
                return thisString += NotEquals(property, value);
            }
            else if (filterType == FilterType.GreaterThan)
            {
                return thisString += GreaterThan(property, value);
            }
            else if (filterType == FilterType.GreaterThanOrEqual)
            {
                return thisString += GreaterThanOrEquals(property, value);
            }
            else if (filterType == FilterType.LessThan)
            {
                return thisString += LessThan(property, value);
            }
            else if (filterType == FilterType.LessThanOrEqual)
            {
                return thisString += LessThanOrEquals(property, value);
            }
            else if (filterType == FilterType.StartsWith)
            {
                return thisString += StartsWith(property, value);
            }
            else if (filterType == FilterType.Contains)
            {
                return thisString += Contains(property, value);
            }
            else if (filterType == FilterType.EndsWith)
            {
                return thisString += EndsWith(property, value);
            }

            return string.Empty;
        }

        public static string AndPredicate(this string thisString)
        {
            return thisString += " AND ";
        }

        public static string OrPredicate(this string thisString)
        {
            return thisString += " OR ";
        }

        private static string Equals(string property, object value)
        {
            return string.Format("[{0}] = '{1}'", property, value != null ? value.ToString() : string.Empty);
        }

        private static string NotEquals(string property, object value)
        {
            // below code is changed for null filtering implementation.
            return string.Format("ISNULL([{0}],'Null Column') <> '{1}'", property, value != null ? value.ToString() : string.Empty);
        }

        private static string GreaterThan(string property, object value)
        {
            return string.Format("[{0}] > '{1}'", property, value != null ? value.ToString() : string.Empty);
        }

        private static string LessThan(string property, object value)
        {
            return string.Format("[{0}] < '{1}'", property, value != null ? value.ToString() : string.Empty);
        }

        private static string GreaterThanOrEquals(string property, object value)
        {
            return string.Format("[{0}] >= '{1}'", property, value != null ? value.ToString() : string.Empty);
        }

        private static string LessThanOrEquals(string property, object value)
        {
            return string.Format("[{0}] <= '{1}'", property, value != null ? value.ToString() : string.Empty);
        }

        private static string StartsWith(string property, object value)
        {
            return string.Format("Convert([{0}], 'System.String') LIKE '{1}*'", property, value != null ? EscapeLikeValue(value.ToString()) : string.Empty);
        }

        private static string EndsWith(string property, object value)
        {
            return string.Format("Convert([{0}], 'System.String') LIKE '*{1}'", property, value != null ? EscapeLikeValue(value.ToString()) : string.Empty);
        }

        private static string Contains(string property, object value)
        {
            return string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", property, value != null ? EscapeLikeValue(value.ToString()) : string.Empty);
        }

        public static string OrderBy(this string thisString, string propertyName)
        {
            return thisString += string.Format("[{0}] ASC", propertyName);
        }

        public static string OrderByDescending(this string thisString, string propertyName)
        {
            return thisString += string.Format("[{0}] DESC", propertyName);
        }

        public static string ThenBy(this string thisString, string propertyName)
        {
            return thisString += string.Format(",[{0}] ASC", propertyName);
        }

        public static string ThenByDescending(this string thisString, string propertyName)
        {
            return thisString += string.Format(",[{0}] DESC", propertyName);
        }

        /// <summary>
        /// Insert [ wildcard ] in LIKE Queries.
        /// http://msdn.microsoft.com/en-us/library/ms179859.aspx
        /// </summary>
        private static string EscapeLikeValue(string property)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < property.Length; i++)
            {
                char c = property[i];
                if (c == '*' || c == '%' || c == '[' || c == ']' || c == '_')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
