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

#region file using directives
using System;
using System.Xml;
using Syncfusion.DocIO.DLS.Entities;

#if !SILVERLIGHT && !WP
using Image = System.Drawing.Image;
#else
using Image = Syncfusion.DocIO.DLS.Entities.Image;
#endif

#endregion

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// Summary description for IXDLSContentWriter.
    /// </summary>
    public interface IXDLSContentWriter
    {
        /// <summary>
        /// Writes the child binary element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void WriteChildBinaryElement(string name, byte[] value);
        /// <summary>
        /// Writes the child string element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void WriteChildStringElement(string name, string value);
        /// <summary>
        /// Writes the child element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void WriteChildElement(string name, object value);
        /// <summary>
        /// Writes the child ref element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="refToElement">The ref to element.</param>
        void WriteChildRefElement(string name, int refToElement);
        ///// <summary>
        ///// Writes the image.
        ///// </summary>
        ///// <param name="image">The image.</param>
        //void WriteImage(Image image);
        /// <summary>
        /// Gets the inner writer.
        /// </summary>
        /// <value>The inner writer.</value>
        XmlWriter InnerWriter
        {
            get;
        }
    }

    /// <summary>
    /// Summary description for IXDLSContentReader.
    /// </summary>
    public interface IXDLSContentReader
    {
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        /// <value>The name of the tag.</value>
        string TagName
        {
            get;
        }
        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        XmlNodeType NodeType
        {
            get;
        }
        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string GetAttributeValue(string name);
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Parses the type of the element.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns></returns>
        bool ParseElementType(Type enumType, out Enum elementType);
#endif
        /// <summary>
        /// Reads the child element.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        bool ReadChildElement(object value);
        /// <summary>
        /// Reads the child element.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object ReadChildElement(Type type);
//#if !SILVERLIGHT
        /// <summary>
        /// Reads the content of the child string.
        /// </summary>
        /// <returns></returns>
        string ReadChildStringContent();
//#endif
        /// <summary>
        /// Reads the child binary element.
        /// </summary>
        /// <returns></returns>
        byte[] ReadChildBinaryElement();
        ///// <summary>
        ///// Reads the image.
        ///// </summary>
        ///// <returns></returns>
        //Image ReadImage();
        ///// <summary>
        ///// Reads the image.
        ///// </summary>
        ///// <param name="isMetafile">if it is a metafile, set to <c>true</c>.</param>
        ///// <returns></returns>
        //Image ReadImage(bool isMetafile);
        /// <summary>
        /// Gets the inner reader.
        /// </summary>
        /// <value>The inner reader.</value>
        XmlReader InnerReader
        {
            get;
        }
        /// <summary>
        /// Gets the attribute reader.
        /// </summary>
        /// <value>The attribute reader.</value>
        IXDLSAttributeReader AttributeReader
        {
            get;
        }
    }
}