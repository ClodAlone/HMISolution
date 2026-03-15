#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
#if !WINRT && !WP
using System.Drawing;
#endif

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// Summary description for IXDLSAttributeWriter.
    /// </summary>
    public interface IXDLSAttributeWriter
    {
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void WriteValue(string name, float value);
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void WriteValue(string name, double value);
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void WriteValue(string name, int value);
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void WriteValue(string name, string value);
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void WriteValue(string name, Enum value);
        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">if it specifies value, set to <c>true</c>.</param>
        void WriteValue(string name, bool value);
        /// <summary>
        /// Writes color as string to XML.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <param name="value">Color structure.</param>
        void WriteValue(string name, Color value);
        /// <summary>
        /// Writes DateTime as string to XML.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <param name="value">Color structure.</param>
        void WriteValue(string name, DateTime value);
    }

    /// <summary>
    /// Summary description for IXDLSAttributeReader.
    /// </summary>
    public interface IXDLSAttributeReader
    {
        /// <summary>
        /// Determines whether the current node has attribute with specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	if has attribute with specified name, set to <c>true</c>.
        /// </returns>
        bool HasAttribute(string name);
        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string ReadString(string name);
        /// <summary>
        /// Reads the int.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        int ReadInt(string name);
        /// <summary>
        /// Reads the short.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        short ReadShort(string name);
        /// <summary>
        /// Reads the float.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        float ReadFloat(string name);
        /// <summary>
        /// Reads the boolean.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        bool ReadBoolean(string name);
        /// <summary>
        /// Reads the byte.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        byte ReadByte(string name);
//#if !SILVERLIGHT
        /// <summary>
        /// Reads the enum.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        Enum ReadEnum(string name, Type enumType);
//#endif
        /// <summary>
        /// Reads color from XML.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <returns></returns>
        Color ReadColor(string name);
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Reads the date time.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        DateTime ReadDateTime(string s);
#endif
    }
}