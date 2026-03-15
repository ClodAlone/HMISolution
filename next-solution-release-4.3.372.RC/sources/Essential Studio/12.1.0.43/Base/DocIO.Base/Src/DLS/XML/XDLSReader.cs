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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS.Entities;
using Syncfusion.DocIO.Utilities;
using System.Globalization;
#if !SILVERLIGHT && !WP
using Image = System.Drawing.Image;
#else
using Image = Syncfusion.DocIO.DLS.Entities.Image;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// Summary description for XDLSReader.
    /// </summary>
    public class XDLSReader
      : IXDLSAttributeReader,
        IXDLSContentReader
    {
        #region Class members
        private Dictionary<Type, object> s_enumHashEntryDict = new Dictionary<Type, object>();
        /// <summary>
        /// 
        /// </summary>
        private XmlReader m_reader;
        private XDLSCustomRW m_customRW = new XDLSCustomRW();
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="XDLSReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public XDLSReader(XmlReader reader)
        {
            m_reader = reader;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Deserializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Deserialize(IXDLSSerializable value)
        {
            while (m_reader.NodeType != XmlNodeType.Element)
            {
                m_reader.Read();
            }

            ReadElement(value);
            value.XDLSHolder.AfterDeserialization(value);
        }
        #endregion

        #region IXDLSAttributeReader implement
        /// <summary>
        /// Determines whether the current node has attribute with specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	if has attribute with specified name, set to <c>true</c>.
        /// </returns>
        public bool HasAttribute(string name)
        {
            return (m_reader.GetAttribute(name) != null);
        }
        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string ReadString(string name)
        {
            return m_reader.GetAttribute(name);
        }
        /// <summary>
        /// Reads the int.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int ReadInt(string name)
        {
            return XmlConvert.ToInt32(m_reader.GetAttribute(name));
        }
        /// <summary>
        /// Reads the short.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public short ReadShort(string name)
        {
            return XmlConvert.ToInt16(m_reader.GetAttribute(name));
        }
        /// <summary>
        /// Reads the double.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public double ReadDouble(string name)
        {
            return XmlConvert.ToDouble(m_reader.GetAttribute(name));
        }
        /// <summary>
        /// Reads the float.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public float ReadFloat(string name)
        {
            return XmlConvert.ToSingle(m_reader.GetAttribute(name));
        }
        /// <summary>
        /// Reads the boolean.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool ReadBoolean(string name)
        {
            string s = m_reader.GetAttribute(name);
            return XmlConvert.ToBoolean(s);
        }
        /// <summary>
        /// Reads the byte.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public byte ReadByte(string name)
        {
            string s = m_reader.GetAttribute(name);
            return XmlConvert.ToByte(s);
        }
//#if !SILVERLIGHT
        /// <summary>
        /// Reads the enum.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        public Enum ReadEnum(string name, Type enumType)
        {
            string value = m_reader.GetAttribute(name);
#if !SILVERLIGHT && !WP
            return (Enum)Enum.Parse(enumType, value);
#else
            return (Enum)Enum.Parse(enumType, value, true);
#endif
        }
//#endif
        /// <summary>
        /// Reads color from XML.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <returns>Color structure.</returns>
        public Color ReadColor(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            string value = m_reader.GetAttribute(name);
            Color color = GetHexColor(value);
            return color;
        }
        /// <summary>
        /// Gets the hexadecimal color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Color GetHexColor(string color)
        {
            // Parse hexadecimal color
            color = color.Replace("#", string.Empty);
            
            try
            {
                string strA = color.Substring(0, 2);
                string strR = color.Substring(2, 2);
                string strG = color.Substring(4, 2);
                string strB = color.Substring(6, 2);
                int a = Int32.Parse(strA, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                int r = Int32.Parse(strR, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                int g = Int32.Parse(strG, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                int b = Int32.Parse(strB, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                return Color.FromArgb(a, r, g, b);
            }
            catch
            { }

            return Color.Empty;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Reads color from XML.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <returns>Color structure.</returns>
        public DateTime ReadDateTime(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            string value = m_reader.GetAttribute(name);
            DateTime time;
#if !SyncfusionFramework2_0
      time = XmlConvert.ToDateTime( value );
#else
            time = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
#endif

            return time;
        }
#endif
        #endregion

        #region IXDLSContentReader implement
        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        /// <value>The name of the tag.</value>
        public string TagName
        {
            get
            {
                return m_reader.LocalName;
            }
        }
        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        public XmlNodeType NodeType
        {
            get
            {
                return m_reader.NodeType;
            }
        }
        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetAttributeValue(string name)
        {
            return m_reader.GetAttribute(name);
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Parses the type of the element.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns></returns>
        public bool ParseElementType(Type enumType, out Enum elementType)
        {
            string[] enNames = null;
            Array enValues = null;
            object enumHashEntry = null;
            if (s_enumHashEntryDict.ContainsKey(enumType) )
                enumHashEntry = s_enumHashEntryDict[enumType];

            if (enumHashEntry == null)
            {
                enNames = Enum.GetNames(enumType);
                enValues = Enum.GetValues(enumType);
                enumHashEntry = new object[2] { enNames, enValues };

                s_enumHashEntryDict.Add(enumType, enumHashEntry);
            }
            else
            {
                enNames = (string[])((object[])enumHashEntry)[0];
                enValues = (Array)((object[])enumHashEntry)[1];
            }

            string typeValueName = GetAttributeValue("type");
            object typeValue = 0;

            for (int i = 0; i < enNames.Length; i++)
            {
                if (enNames[i] == typeValueName)
                {
                    elementType = (Enum)enValues.GetValue(i);
                    return true;
                }
            }

            elementType = (Enum)enValues.GetValue(0);
            return false;
        }
#endif
        /// <summary>
        /// Reads the child element.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool ReadChildElement(object value)
        {
            IXDLSSerializable dlsSer = value as IXDLSSerializable;

            if (dlsSer != null)
            {
                ReadElement(dlsSer);
            }
            else
            {
                IXDLSSerializableCollection dlsSerColl =
                  value as IXDLSSerializableCollection;

                if (dlsSerColl != null)
                {
                    ReadElementCollection(dlsSerColl);
                }
                else
                {
                    // We don't move to next element 
                    return false;
                }
            }

            // We moved to next element
            return true;
        }
        /// <summary>
        /// Reads the child element.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public object ReadChildElement(Type type)
        {
            object value = m_customRW.Read(m_reader, type);

            return value;
        }
//#if !SILVERLIGHT
        /// <summary>
        /// Reads the content of the child string.
        /// </summary>
        /// <returns></returns>
        public string ReadChildStringContent()
        {
            return m_reader.ReadContentAsString();
        }
//#endif
        /// <summary>
        /// Reads binary value.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadChildBinaryElement()
        {
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      XmlTextReader reader = ( XmlTextReader )m_reader;
#elif SyncfusionFramework2_0 || SILVERLIGHT || WP
            XmlReader reader = m_reader;
#endif

            int base64len = 0;
            byte[] resData = new byte[0];
            byte[] base64 = new byte[1000];

            do
            {
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
        base64len = reader.ReadBase64( base64, 0, base64.Length );
#elif SyncfusionFramework2_0 || SILVERLIGHT || WP
                base64len = reader.ReadElementContentAsBase64(base64, 0, base64.Length);
#endif
                // Expands resData and copy to resData new portion from 
                // Base64 stream
                byte[] newData = new byte[resData.Length + base64len];
                resData.CopyTo(newData, 0);
                Array.Copy(base64, 0, newData, resData.Length, base64len);
                resData = newData;

                if (base64len < base64.Length)
                {
                    break;
                }
                else
                {
                    base64 = new byte[resData.Length * 2];
                }
            }
            while (!reader.EOF);

            return resData;
        }
        /// <summary>
        /// Reads the image.
        /// </summary>
        /// <returns></returns>
        internal Image ReadImage()
        {
            return ReadImage(false);
        }
        /// <summary>
        /// Reads the image.
        /// </summary>
        /// <param name="isMetafile">if it is a metafile, set to <c>true</c>.</param>
        /// <returns></returns>
        internal Image ReadImage(bool isMetafile)
        {
        	Image image = null;
#if !SILVERLIGHT && !WP
            byte[] buf = ReadChildBinaryElement();
            
            if (buf.Length > 0)
            {
                MemoryStream memStream = new MemoryStream(buf);
                if (isMetafile)
                {
                    image = new System.Drawing.Imaging.Metafile(memStream);
                }
                else
                {
                    image = new System.Drawing.Bitmap(memStream);
                }
            }
#endif
            return image;
        }
        /// <summary>
        /// Gets the inner reader.
        /// </summary>
        /// <value>The inner reader.</value>
        public XmlReader InnerReader
        {
            get
            {
                return m_reader;
            }
        }
        /// <summary>
        /// Gets the attribute reader.
        /// </summary>
        /// <value>The attribute reader.</value>
        public IXDLSAttributeReader AttributeReader
        {
            get
            {
                return this;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void ReadElement(IXDLSSerializable value)
        {
            if (value == null)
            {
                m_reader.Skip();
                return;
            }

            if (m_reader.HasAttributes)
            {
                if (m_reader.MoveToAttribute("id"))
                {
                    value.XDLSHolder.ID = XmlConvert.ToInt32(m_reader.GetAttribute("id"));
                }
                value.ReadXmlAttributes(this);
                m_reader.MoveToElement();
            }

            bool hasChildElements = !m_reader.IsEmptyElement;

            int currDepth = m_reader.Depth;

            //try
            //{
            m_reader.ReadStartElement();
            //}
            //catch(Exception ex)
            //{
            //  if( m_reader.NodeType == XmlNodeType.Whitespace )
            //  {
            //    do
            //    {
            //      m_reader.Read();
            //    }m
            //    while( m_reader.NodeType == XmlNodeType.Whitespace );
            //  }
            //  else
            //  {
            //    throw new Exception( ex.Message );
            //  }
            //}

            if (hasChildElements)
            {
                while (m_reader.Depth > currDepth && !m_reader.EOF)
                {
                    // Skips if node is not Element type
                    if (m_reader.NodeType != XmlNodeType.Element)
                    {
                        m_reader.Read();
                        continue;
                    }

                    if (!value.ReadXmlContent(this))
                    {
                        m_reader.Skip();
                    }
                }

                if (m_reader.NodeType == XmlNodeType.EndElement)
                {
                    m_reader.ReadEndElement();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coll"></param>
        private void ReadElementCollection(IXDLSSerializableCollection coll)
        {
            bool hasChildElements = !m_reader.IsEmptyElement;
            int currDepth = m_reader.Depth;
            m_reader.ReadStartElement();

            if (hasChildElements)
            {
                while (m_reader.Depth > currDepth && !m_reader.EOF)
                {
                    // Skips if node is not Element type
                    if (m_reader.NodeType != XmlNodeType.Element)
                    {
                        m_reader.Read();
                        continue;
                    }

                    if (m_reader.LocalName == coll.TagItemName)
                    {
                        IXDLSSerializable newItem = coll.AddNewItem(this);
                        ReadElement(newItem);
                    }
                }

                if (m_reader.NodeType == XmlNodeType.EndElement)
                {
                    m_reader.ReadEndElement();
                }
            }
        }
        #endregion
    }
}
