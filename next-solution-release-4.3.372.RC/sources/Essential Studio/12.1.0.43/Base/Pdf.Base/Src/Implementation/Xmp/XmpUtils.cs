#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion
#if !NETFX_CORE
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Utility class.
    /// </summary>
    internal class XmpUtils
    {
        #region Constants
        /// <summary>
        /// False string.
        /// </summary>
        private const string c_False = "False";

        /// <summary>
        /// True string.
        /// </summary>
        private const string c_True = "True";

        /// <summary>
        /// Real pattern.
        /// </summary>
        private const string c_realPattern = @"^[+-]?[\d]+([.]?[\d])*$";

        /// <summary>
        /// Default date format.
        /// </summary>
        private const string c_dateFormat = "yyyy-MM-dd'T'HH:mm:ss.ffzzz";
        #endregion

        #region Constructors
        /// <summary>
        /// Private constructor.
        /// </summary>
        private XmpUtils()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets text value to the element.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="value">Value data.</param>
        public static void SetTextValue(XmlElement parent, string value)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            ClearChildren(parent);
            XmlText text = parent.OwnerDocument.CreateTextNode(value);
            parent.AppendChild(text);
        }

        /// <summary>
        /// Sets text value to the element.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="value">Value data.</param>
        public static void SetBoolValue(XmlElement parent, bool value)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            string strValue = (value) ? c_True : c_False;
            SetTextValue(parent, strValue);
        }

        /// <summary>
        /// Retrieves boolean value from the string.
        /// </summary>
        /// <param name="value">String representation of the boolean value.</param>
        /// <returns>Boolean value from the string.</returns>
        public static bool GetBoolValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            bool boolValue = value.Equals(c_True) ? true : false;

            return boolValue;
        }

        /// <summary>
        /// Sets text value to the element.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="value">Value data.</param>
        public static void SetRealValue(XmlElement parent, float value)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            string strValue = value.ToString(CultureInfo.InvariantCulture);
            SetTextValue(parent, strValue);
        }

        /// <summary>
        /// Retrieves float value from the string.
        /// </summary>
        /// <param name="value">String representation of the float value.</param>
        /// <returns>Float value from the string.</returns>
        public static float GetRealValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            double result = 0f;
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

            return (float)result;
        }

        /// <summary>
        /// Sets text value to the element.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="value">Value data.</param>
        public static void SetIntValue(XmlElement parent, int value)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            string strValue = value.ToString(CultureInfo.InvariantCulture);
            SetTextValue(parent, strValue);
        }

        /// <summary>
        /// Retrieves float value from the string.
        /// </summary>
        /// <param name="value">String representation of the float value.</param>
        /// <returns>Int value from the string.</returns>
        public static int GetIntValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            double result = 0;
            double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

            return (int)result;
        }

        /// <summary>
        /// Sets unique resource identifier value to the element.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="value">Value data.</param>
        public static void SetUriValue(XmlElement parent, Uri value)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            string strValue = value.ToString();
            SetTextValue(parent, strValue);
        }

        /// <summary>
        /// Retrieves float value from the string.
        /// </summary>
        /// <param name="value">String representation of the float value.</param>
        /// <returns>Uri value from the string.</returns>
        public static Uri GetUriValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Uri result = new Uri(value);
            return result;
        }

        /// <summary>
        /// Sets DateTime value to the element.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="value">Value data.</param>
        public static void SetDateTimeValue(XmlElement parent, DateTime value)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            string strValue = value.ToString(XmpArray.c_dateFormat);
            SetTextValue(parent, strValue);
        }

        /// <summary>
        /// Retrieves float value from the string.
        /// </summary>
        /// <param name="value">String representation of the float value.</param>
        /// <returns>Uri value from the string.</returns>
        public static DateTime GetDateTimeValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            DateTime result = DateTime.Now;
            if (value != string.Empty)
            {
                string dateTimeFormat = "yyyyMMddHHmmss";
                DateTime.TryParseExact(value, dateTimeFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite, out result);
            }
            
            return result;
        }

        /// <summary>
        /// Sets an XML value.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        public static void SetXmlValue(XmlElement parent, XmlElement child)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            ClearChildren(parent);

            parent.AppendChild(child);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Removes all childrens from the 
        /// </summary>
        /// <param name="node">Node element.</param>
        private static void ClearChildren(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            XmlNodeList list = node.ChildNodes;
            for (int i = 0, len = list.Count; i < len; i++)
            {
                XmlNode child = list[0];
                node.RemoveChild(child);
            }
        }
        #endregion
    }
}
#endif