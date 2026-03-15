/*
 *  Copyright © Northwoods Software Corporation, 1998-2016. All Rights Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the U.S.
 *  Government is subject to restrictions as set forth in subparagraph
 *  (c) (1) (ii) of DFARS 252.227-7013, or in FAR 52.227-19, or in FAR
 *  52.227-14 Alt. III, as applicable.
 *
 *  This software is proprietary to and embodies the confidential
 *  technology of Northwoods Software Corporation. Possession, use, or
 *  copying of this software and media is authorized only pursuant to a
 *  valid written license from Northwoods or an authorized sublicensor.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Drawing;

namespace Northwoods.GoXam.Model
{

    /// <summary>
    /// This static class holds various methods that are useful in saving and loading
    /// model data to and from Linq for XML <c>XElement</c>s.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are three kinds of methods defined in this class.
    /// There are basic data conversion methods, converting data types to and from Strings.
    /// There are methods that produce <c>XAttribute</c>s and <c>XElement</c>s
    /// containing given some data.
    /// There are methods that get data from <c>XAttribute</c>s and <c>XElement</c>s.
    /// </para>
    /// <para>
    /// The conversion method names are all prefixed with "To".
    /// They use the <c>XmlConvert</c> methods for achieving their results.
    /// For the basic data types (String, bool, int, Guid, and double), the conversion
    /// methods are the same as the equivalent <c>XmlConvert</c> methods.
    /// For the additional data types (Point, Size, Rect, Thickness)
    /// the conversion methods assume the numbers are separated by spaces.
    /// </para>
    /// <para>
    /// When generating XML using Linq for XML you will often want to avoid
    /// generating attributes or elements when the values are the same as the property defaults.
    /// To help with that situation the methods named "Attribute"
    /// take the attribute name, a property value, and the property's default value.
    /// If the value is null or if the value is equal to the default value,
    /// the method returns null; otherwise it returns a new <c>XAttribute</c>.
    /// This is convenient when building an <c>XElement</c>.
    /// </para>
    /// <para>
    /// When consuming XML using Linq for XML you need to deal with situations
    /// where the expected attribute or child element is not present
    /// or where the value is in the wrong format or for some other reason
    /// cannot be converted to the expected data type.
    /// To help with these cases the methods named "Read"
    /// take the attribute name, the <c>XElement</c> on which the attribute may exist,
    /// and the default value for that property.
    /// If the <c>XElement</c> is null,
    /// or if it has no <c>XAttribute</c> with the given name,
    /// or if there is an exception converting the string value to the expected data type,
    /// it will return the default value.
    /// </para>
    /// <para>
    /// There are also generic methods for handling enumerations and for sequences of data.
    /// For data sequences, of type <c>IEnumerable</c>, the methods will generate
    /// and expect to consume child elements, one for each item.
    /// You will need to specify the name of those child elements,
    /// and the conversion function for the item data type.
    /// </para>
    /// <para>
    /// For examples of how to use these methods,
    /// look at the descriptions of the predefined data methods for making elements and loading
    /// data from them:
    /// <see cref="GraphLinksModelNodeData{NodeKey}.MakeXElement"/>,
    /// <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.MakeXElement"/>,
    /// <see cref="GraphModelNodeData{NodeKey}.MakeXElement"/>,
    /// <see cref="TreeModelNodeData{NodeKey}.MakeXElement"/>,
    /// <see cref="GraphLinksModelNodeData{NodeKey}.LoadFromXElement"/>,
    /// <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.LoadFromXElement"/>,
    /// <see cref="GraphModelNodeData{NodeKey}.LoadFromXElement"/>,
    /// <see cref="TreeModelNodeData{NodeKey}.LoadFromXElement"/>
    /// </para>
    /// </remarks>
    public static class XHelper
    {

        // for general XML construction for non-standard types

        /// <summary>
        /// This generic method produces an <c>XAttribute</c> with the given value
        /// if the value is not equal to the given default value, using the given
        /// conversion function to convert the value to a string.
        /// </summary>
        /// <typeparam name="T">the value type</typeparam>
        /// <param name="n">the name of the <c>XAttribute</c></param>
        /// <param name="v">the value</param>
        /// <param name="defval">the default value for this attribute/property</param>
        /// <param name="conv">a function from <typeparamref name="T"/> to String</param>
        /// <returns>an <c>XAttribute</c>, or null if the value is null or if it is equal to the default value</returns>
        public static XAttribute Attribute<T>(XName n, T v, T defval, Func<T, String> conv)
        {
            if (v == null) return null;
            if (v.Equals(defval)) return null;
            return new XAttribute(n, conv(v));
        }

        /// <summary>
        /// This generic method produces a property value from an <c>XElement</c>'s
        /// attribute value, using a conversion function to convert the string to
        /// the expected type, and returning a default value if the conversion fails.
        /// </summary>
        /// <typeparam name="T">the value type</typeparam>
        /// <param name="n">the name of the attribute</param>
        /// <param name="e">the <c>XElement</c> that may have an <c>XAttribute</c> with the name <paramref name="n"/></param>
        /// <param name="defval">
        /// the default value to return if there's no attribute named <paramref name="n"/>
        /// or if the attribute value string cannot be converted to the value type
        /// </param>
        /// <param name="conv">a function from String to <typeparamref name="T"/></param>
        /// <returns>either the attribute value string converted to <typeparamref name="T"/>,
        /// or else the <paramref name="defval"/></returns>
        public static T Read<T>(XName n, XElement e, T defval, Func<String, T> conv)
        {
            if (e == null) return defval;
            XAttribute a = e.Attribute(n);
            if (a == null) return defval;
            try
            {
                return conv(a.Value);
            }
            catch (Exception)
            {
                return defval;
            }
        }


        // String
        /// <summary>
        /// Generate an <c>XAttribute</c> for a String value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, String v, String defval) { return Attribute<String>(n, v, defval, s => s); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a String,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static String Read(XName n, XElement e, String defval) { return Read<String>(n, e, defval, s => s); }

        /// <summary>
        /// This data conversion function converts a String to a String by just returning its argument.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(String v) { return v; }

        // bool
        /// <summary>
        /// Generate an <c>XAttribute</c> for a Boolean value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, bool v, bool defval) { return Attribute<bool>(n, v, defval, XHelper.ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Boolean,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static bool Read(XName n, XElement e, bool defval) { return Read<bool>(n, e, defval, XHelper.ToBoolean); }

        /// <summary>
        /// This data conversion function converts a Boolean to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(bool v) { return XmlConvert.ToString(v); }
        /// <summary>
        /// This data conversion function converts a String to a Boolean.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ToBoolean(String s)
        {
            if (s == null) return false;
            return XmlConvert.ToBoolean(s.ToLower(System.Globalization.CultureInfo.InvariantCulture));
        }

        // int
        /// <summary>
        /// Generate an <c>XAttribute</c> for an Int32 value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, int v, int defval) { return Attribute<int>(n, v, defval, XHelper.ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to an Int32,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static int Read(XName n, XElement e, int defval) { return Read<int>(n, e, defval, XHelper.ToInt32); }

        /// <summary>
        /// This data conversion function converts an Int32 to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(int v) { return XmlConvert.ToString(v); }
        /// <summary>
        /// This data conversion function converts a String to an Int32.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt32(String s) { return XmlConvert.ToInt32(s); }

        // GUID
        /// <summary>
        /// Generate an <c>XAttribute</c> for a Guid value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, Guid v, Guid defval) { return Attribute<Guid>(n, v, defval, XHelper.ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Guid,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static Guid Read(XName n, XElement e, Guid defval) { return Read<Guid>(n, e, defval, XHelper.ToGuid); }

        /// <summary>
        /// This data conversion function converts a Guid to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(Guid v) { return XmlConvert.ToString(v); }
        /// <summary>
        /// This data conversion function converts a String to a Guid.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Guid ToGuid(String s) { return XmlConvert.ToGuid(s); }

        // double
        /// <summary>
        /// Generate an <c>XAttribute</c> for a Double value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, double v, double defval) { return Attribute<double>(n, v, defval, XHelper.ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Double,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static double Read(XName n, XElement e, double defval) { return Read<double>(n, e, defval, XHelper.ToDouble); }

        /// <summary>
        /// This data conversion function converts a Double to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(double v) { return XmlConvert.ToString(v); }
        /// <summary>
        /// This data conversion function converts a String to a Double.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ToDouble(String s) { return XmlConvert.ToDouble(s); }

        // Point
        /// <summary>
        /// Generate an <c>XAttribute</c> for a Point value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, PointF v, PointF defval) { return Attribute<PointF>(n, v, defval, ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Point,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static PointF Read(XName n, XElement e, PointF defval) { return Read<PointF>(n, e, defval, ToPoint); }

        /// <summary>
        /// This data conversion function converts a Point to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(PointF v)
        {
            return XmlConvert.ToString(v.X) + " " + XmlConvert.ToString(v.Y);
        }
        /// <summary>
        /// This data conversion function converts a String to a Point.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static PointF ToPoint(String s)
        {
            if (s == null) return new PointF();
            char[] separators = { ' ' };
            String[] nums = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            PointF v = new PointF();
            if (nums.Length > 0) v.X = XmlConvert.ToSingle(nums[0]);
            if (nums.Length > 1) v.Y = XmlConvert.ToSingle(nums[1]);
            return v;
        }

        // Size
        /// <summary>
        /// Generate an <c>XAttribute</c> for a Size value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, SizeF v, SizeF defval) { return Attribute<SizeF>(n, v, defval, ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Size,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static SizeF Read(XName n, XElement e, SizeF defval) { return Read<SizeF>(n, e, defval, ToSize); }

        /// <summary>
        /// This data conversion function converts a Size to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(SizeF v)
        {
            return XmlConvert.ToString(v.Width) + " " + XmlConvert.ToString(v.Height);
        }
        /// <summary>
        /// This data conversion function converts a String to a Size.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static SizeF ToSize(String s)
        {
            if (s == null) return new SizeF();
            char[] separators = { ' ' };
            String[] nums = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            SizeF v = new SizeF();
            if (nums.Length > 0) v.Width = XmlConvert.ToSingle(nums[0]);
            if (nums.Length > 1) v.Height = XmlConvert.ToSingle(nums[1]);
            return v;
        }

        // Rect
        /// <summary>
        /// Generate an <c>XAttribute</c> for a Rect value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, RectangleF v, RectangleF defval) { return Attribute<RectangleF>(n, v, defval, ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Rect,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static RectangleF Read(XName n, XElement e, RectangleF defval) { return Read<RectangleF>(n, e, defval, ToRect); }

        /// <summary>
        /// This data conversion function converts a Rect to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(RectangleF v)
        {
            return XmlConvert.ToString(v.X) + " " + XmlConvert.ToString(v.Y) + " " + XmlConvert.ToString(v.Width) + " " + XmlConvert.ToString(v.Height);
        }
        /// <summary>
        /// This data conversion function converts a String to a Rect.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static RectangleF ToRect(String s)
        {
            if (s == null) return new RectangleF();
            char[] separators = { ' ' };
            String[] nums = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            RectangleF v = new RectangleF();
            if (nums.Length > 0) v.X = XmlConvert.ToSingle(nums[0]);
            if (nums.Length > 1) v.Y = XmlConvert.ToSingle(nums[1]);
            if (nums.Length > 2) v.Width = XmlConvert.ToSingle(nums[2]);
            if (nums.Length > 3) v.Height = XmlConvert.ToSingle(nums[3]);
            return v;
        }

        // Thickness
        /// <summary>
        /// Generate an <c>XAttribute</c> for a Thickness value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, Thickness v, Thickness defval) { return Attribute<Thickness>(n, v, defval, ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Thickness,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static Thickness Read(XName n, XElement e, Thickness defval) { return Read<Thickness>(n, e, defval, ToThickness); }

        /// <summary>
        /// This data conversion function converts a Thickness to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(Thickness v)
        {
            return XmlConvert.ToString(v.Left) + " " + XmlConvert.ToString(v.Top) + " " + XmlConvert.ToString(v.Right) + " " + XmlConvert.ToString(v.Bottom);
        }
        /// <summary>
        /// This data conversion function converts a String to a Thickness.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Thickness ToThickness(String s)
        {
            if (s == null) return new Thickness();
            char[] separators = { ' ' };
            String[] nums = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Thickness v = new Thickness();
            if (nums.Length > 0) v.Left = XmlConvert.ToDouble(nums[0]);
            if (nums.Length > 1) v.Top = XmlConvert.ToDouble(nums[1]);
            if (nums.Length > 2) v.Right = XmlConvert.ToDouble(nums[2]);
            if (nums.Length > 3) v.Bottom = XmlConvert.ToDouble(nums[3]);
            return v;
        }

        // DateTime
        /// <summary>
        /// Generate an <c>XAttribute</c> for a DateTime value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, DateTime v, DateTime defval) { return Attribute<DateTime>(n, v, defval, XHelper.ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a DateTime,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static DateTime Read(XName n, XElement e, DateTime defval) { return Read<DateTime>(n, e, defval, XHelper.ToDateTime); }

        /// <summary>
        /// This data conversion function converts a DateTime to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(DateTime v) { return XmlConvert.ToString(v, XmlDateTimeSerializationMode.RoundtripKind); }
        /// <summary>
        /// This data conversion function converts a String to a DateTime.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(String s) { return XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind); }

        // TimeSpan
        /// <summary>
        /// Generate an <c>XAttribute</c> for a TimeSpan value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, TimeSpan v, TimeSpan defval) { return Attribute<TimeSpan>(n, v, defval, XHelper.ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a TimeSpan,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static TimeSpan Read(XName n, XElement e, TimeSpan defval) { return Read<TimeSpan>(n, e, defval, XHelper.ToTimeSpan); }

        /// <summary>
        /// This data conversion function converts a TimeSpan to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(TimeSpan v) { return XmlConvert.ToString(v); }
        /// <summary>
        /// This data conversion function converts a String to a TimeSpan.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(String s) { return XmlConvert.ToTimeSpan(s); }

        // Decimal
        /// <summary>
        /// Generate an <c>XAttribute</c> for a Decimal value, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, Decimal v, Decimal defval) { return Attribute<Decimal>(n, v, defval, XHelper.ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Decimal,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static Decimal Read(XName n, XElement e, Decimal defval) { return Read<Decimal>(n, e, defval, XHelper.ToDecimal); }

        /// <summary>
        /// This data conversion function converts a Decimal to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(Decimal v) { return XmlConvert.ToString(v); }
        /// <summary>
        /// This data conversion function converts a String to a Decimal.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Decimal ToDecimal(String s) { return XmlConvert.ToDecimal(s); }


        // Enum
        /// <summary>
        /// Generate an <c>XAttribute</c> for an enumerated type value, unless the value is equal to the given default value.
        /// </summary>
        /// <typeparam name="T">this must be an <c>Enum</c> type</typeparam>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute AttributeEnum<T>(XName n, T v, T defval) where T : struct, IComparable
        {
            if (v.CompareTo(defval) == 0) return null;
            return new XAttribute(n, v.ToString());
        }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to an enumerated type,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <typeparam name="T">this must be an <c>Enum</c> type</typeparam>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the <typeparamref name="T"/>, or the default value</returns>
        public static T ReadEnum<T>(XName n, XElement e, T defval) where T : struct, IComparable
        {
            if (e == null) return defval;
            XAttribute a = e.Attribute(n);
            if (a == null) return defval;
            try
            {
                return (T)Enum.Parse(typeof(T), a.Value, true);
            }
            catch (ArgumentException)
            {
                return defval;
            }
            catch (OverflowException)
            {
                return defval;
            }
        }


        //?? Color


        // IEnumerable<int>
        /// <summary>
        /// Generate an <c>XAttribute</c> for a sequence of Int32 values, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, IEnumerable<int> v, IEnumerable<int> defval) { return Attribute<IEnumerable<int>>(n, v, defval, ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Thickness,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static IEnumerable<int> Read(XName n, XElement e, IEnumerable<int> defval) { return Read<IEnumerable<int>>(n, e, defval, ToIEnumerableOfInt32); }

        /// <summary>
        /// This data conversion function converts a sequence of Int32 values to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(IEnumerable<int> v)
        {
            if (v == null) return "";
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (int x in v)
            {
                if (!first) sb.Append(" ");
                first = false;
                sb.Append(XmlConvert.ToString(x));
            }
            return sb.ToString();
        }
        /// <summary>
        /// This data conversion function converts a String to a sequence of Int32.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<int> ToIEnumerableOfInt32(String s)
        {
            if (s == null || s.Length == 0) return Enumerable.Empty<int>();
            char[] separators = { ' ' };
            String[] nums = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int[] v = new int[nums.Length];
            for (int i = 0; i < nums.Length; i++)
            {
                v[i] = XmlConvert.ToInt32(nums[i]);
            }
            return v;
        }


        // IEnumerable<double>
        /// <summary>
        /// Generate an <c>XAttribute</c> for a sequence of Double values, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, IEnumerable<double> v, IEnumerable<double> defval) { return Attribute<IEnumerable<double>>(n, v, defval, ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Thickness,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static IEnumerable<double> Read(XName n, XElement e, IEnumerable<double> defval) { return Read<IEnumerable<double>>(n, e, defval, ToIEnumerableOfDouble); }

        /// <summary>
        /// This data conversion function converts a sequence of Double values to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(IEnumerable<double> v)
        {
            if (v == null) return "";
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (double x in v)
            {
                if (!first) sb.Append(" ");
                first = false;
                sb.Append(XmlConvert.ToString(x));
            }
            return sb.ToString();
        }
        /// <summary>
        /// This data conversion function converts a String to a sequence of Doubles.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<double> ToIEnumerableOfDouble(String s)
        {
            if (s == null || s.Length == 0) return Enumerable.Empty<double>();
            char[] separators = { ' ' };
            String[] nums = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            double[] v = new double[nums.Length];
            for (int i = 0; i < nums.Length; i++)
            {
                v[i] = XmlConvert.ToDouble(nums[i]);
            }
            return v;
        }


        // IEnumerable<Point>
        /// <summary>
        /// Generate an <c>XAttribute</c> for a sequence of Point values, unless the value is equal to the given default value.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="v">the property value</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>an <c>XAttribute</c> or null</returns>
        public static XAttribute Attribute(XName n, IEnumerable<PointF> v, IEnumerable<PointF> defval) { return Attribute<IEnumerable<PointF>>(n, v, defval, ToString); }
        /// <summary>
        /// Consume an <c>XAttribute</c> on an <c>XElement</c>, converting the attribute's value to a Thickness,
        /// or return the default value if the attribute is not present or if there is a conversion exception.
        /// </summary>
        /// <param name="n">the attribute name, often the same as the data's property name</param>
        /// <param name="e">the <c>XElement</c> holding the expected attribute</param>
        /// <param name="defval">the default value for the property</param>
        /// <returns>the attribute value converted to the expected data type, or the default value</returns>
        public static IEnumerable<PointF> Read(XName n, XElement e, IEnumerable<PointF> defval) { return Read<IEnumerable<PointF>>(n, e, defval, ToIEnumerableOfPoint); }

        /// <summary>
        /// This data conversion function converts a sequence of Point values to a String.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static String ToString(IEnumerable<PointF> v)
        {
            if (v == null) return "";
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (PointF p in v)
            {
                if (!first) sb.Append(" ");
                first = false;
                sb.Append(XmlConvert.ToString(p.X));
                sb.Append(" ");
                sb.Append(XmlConvert.ToString(p.Y));
            }
            return sb.ToString();
        }
        /// <summary>
        /// This data conversion function converts a String to a sequence of PointFs.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<PointF> ToIEnumerableOfPoint(String s)
        {
            if (s == null || s.Length == 0) return Enumerable.Empty<PointF>();
            char[] separators = { ' ' };
            String[] nums = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            PointF[] v = new PointF[nums.Length / 2];
            for (int i = 0; i < nums.Length; i++)
            {
                float x = XmlConvert.ToSingle(nums[i]);
                i++;
                float y = ((i < nums.Length) ? XmlConvert.ToSingle(nums[i]) : 0);
                v[i / 2] = new PointF(x, y);
            }
            return v;
        }


        //?? IEnumerable<Color>


        // IEnumerable
        // even though it's non-generic IEnumerable, we still need to know the item type, so this is a generic method
        /// <summary>
        /// Generate an <c>XElement</c> containing nested child <c>XElement</c>s, one for each item in the given collection.
        /// </summary>
        /// <typeparam name="T">the item data type</typeparam>
        /// <param name="n">the name for the new <c>XElement</c></param>
        /// <param name="c">the name for the child elements</param>
        /// <param name="v"></param>
        /// <param name="conv"></param>
        /// <returns>an <c>XElement</c> or null if the given collection was null or empty</returns>
        public static XElement Elements<T>(XName n, XName c, System.Collections.IEnumerable v, Func<T, String> conv)
        {
            if (v == null) return null;
            // if there are no elements in V, just return null -- i.e. don't create a child XElement
            XElement e = null;
            foreach (T item in v)
            {
                if (e == null) e = new XElement(n);
                e.Add(new XElement(c, conv(item)));
            }
            return e;
        }
        /// <summary>
        /// Consume an <c>XContainer</c> that is assumed to hold a collection of nested child <c>XElement</c>s
        /// each holding a value to be converted to the given data type.
        /// </summary>
        /// <typeparam name="T">the item data type</typeparam>
        /// <param name="e">the <c>XContainer</c> to be read</param>
        /// <param name="c">the name of the child elements</param>
        /// <param name="coll">an empty collection to which items are added</param>
        /// <param name="conv">a conversion function from String to the item data type, <typeparamref name="T"/></param>
        /// <returns>the <paramref name="coll"/> collection, containing all of the data items that were read</returns>
        /// <remarks>
        /// If there is an exception converting any of the child elements to data values,
        /// this will return the collection gathered so far.
        /// </remarks>
        public static System.Collections.IEnumerable ReadElements<T>(XContainer e, XName c, ICollection<T> coll, Func<String, T> conv)
        {
            if (e != null && coll != null)
            {
                try
                {
                    foreach (T s in e.Elements(c).Select(x => conv(x.Value)))
                    {
                        coll.Add(s);
                    }
                }
                catch (Exception)
                {
                }
            }
            return coll;  // if exception happens midway, might return partially constructed list
        }
    }
}
