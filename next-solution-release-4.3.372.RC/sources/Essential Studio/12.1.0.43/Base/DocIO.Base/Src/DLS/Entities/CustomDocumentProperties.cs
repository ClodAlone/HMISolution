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
using System.Collections.Generic;

using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.Utilities;
using Syncfusion.CompoundFile.DocIO;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the custom document properties in a MS Word document.
    /// </summary>
    public class CustomDocumentProperties : XDLSSerializableBase
    {
        #region Class Constants
        internal const string TagName = "property";
        internal const string NameAttribute = "name";
        internal const string PIDAttribute = "pid";
        internal const string FMTIDAttribute = "fmtid";
        #endregion

        #region Class members
        /// <summary>
        /// Sorted list of properties
        /// </summary>
        protected Dictionary<string, DocumentProperty> m_customList;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, DocumentProperty> CustomHash
        {
            get
            {
                return m_customList;
            }
        }
        /// <summary>
        /// Gets / sets property by specified name.
        /// </summary>
        public DocumentProperty this[string name]
        {
            get
            {
                if (m_customList.ContainsKey(name))
                    return m_customList[name];
                else
                    return null;
            }
        }
        /// <summary>
        /// Gets / sets property by specified index.
        /// </summary>
        public DocumentProperty this[int index]
        {
            get
            {
                int curIndex = 0;
                foreach (string key in m_customList.Keys)
                {
                    if (curIndex == index)
                        return m_customList[key];
                    else
                        curIndex++;
                }
                return null;
            }
        }
        /// <summary>
        /// Gets count of the properties.
        /// </summary>
        public int Count
        {
            get
            {
                return m_customList.Count;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initialize
        /// </summary>
        internal CustomDocumentProperties()
            : this(0)
        {
        }
        /// <summary>
        /// Initialize
        /// </summary>
        internal CustomDocumentProperties(int count)
            : base(null, null)
        {
            m_customList = new Dictionary<string, DocumentProperty>(count);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public DocumentProperty Add(string name, object value)
        {
            DocumentProperty property = new DocumentProperty(name, value, DocumentProperty.DetectPropertyType(value));
            m_customList.Add(name, property);

            return property;
        }
        /// <summary>
        /// Remove property specified by name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            CustomHash.Remove(name);
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public CustomDocumentProperties Clone()
        {
            CustomDocumentProperties customDocumentProperties = new CustomDocumentProperties(m_customList.Count);

            foreach (string key in m_customList.Keys)
            {
                DocumentProperty property = m_customList[key];
                customDocumentProperties.m_customList.Add(key, property.Clone() as DocumentProperty);
            }

            return customDocumentProperties;
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Class XDLSSerializable implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);

            IXDLSAttributeWriter attrWriter = writer as IXDLSAttributeWriter;
            XmlWriter xmlWriter = (writer as XDLSWriter).InnerWriter;

            if (m_customList != null && m_customList.Count > 0)
            {
                foreach (string key in m_customList.Keys)
                {
                    //object obj = entry.Value;
                    //DocumentProperty property = (DocumentProperty)obj;
                    //string key = "";

                    //if (entry.Key is int)
                    //{
                    //    key += ((int)entry.Key).ToString();
                    //}
                    //else if (entry.Key is string)
                    //{
                    //    key = (string)entry.Key;
                    //}
                    DocumentProperty property = m_customList[key];
                    xmlWriter.WriteStartElement(XDLSConstants.PropertyTag);

                    xmlWriter.WriteAttributeString(XDLSConstants.PropertiesNameAttr, key);

                    switch (property.PropertyType)
                    {
                        //case Syncfusion.CompoundFile.DocIO.PropertyType.ClipData:
                        //    ClipDataWrapper clipdata = property.ToClipData();
                        //    xmlWriter.WriteAttributeString(XDLSConstants.PropertiesTypeAttr, "clip");
                        //    xmlWriter.WriteAttributeString(XDLSConstants.PropertiesValueAttr, clipdata.WriteToString());
                        //    break;
                        //case Syncfusion.CompoundFile.DocIO.PropertyType.ObjectArray:
                        //    object[] arr = property.ObjectArray;
                        //    xmlWriter.WriteAttributeString(XDLSConstants.PropertiesTypeAttr, "array");
                        //    writer.WriteChildBinaryElement(XDLSConstants.InternalDataTag, arr);
                        //    break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.String:
                            xmlWriter.WriteAttributeString(XDLSConstants.PropertiesTypeAttr, "string");
                            attrWriter.WriteValue(XDLSConstants.PropertiesValueAttr, property.ToString());
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Int:
                            xmlWriter.WriteAttributeString(XDLSConstants.PropertiesTypeAttr, "int");
                            attrWriter.WriteValue(XDLSConstants.PropertiesValueAttr, property.Integer);
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Double:
                            xmlWriter.WriteAttributeString(XDLSConstants.PropertiesTypeAttr, "double");
                            attrWriter.WriteValue(XDLSConstants.PropertiesValueAttr, property.Double);
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.DateTime:
                            xmlWriter.WriteAttributeString(XDLSConstants.PropertiesTypeAttr, "DateTime");
                            attrWriter.WriteValue(XDLSConstants.PropertiesValueAttr, property.DateTime);
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Bool:
                            xmlWriter.WriteAttributeString(XDLSConstants.PropertiesTypeAttr, "bool");
                            attrWriter.WriteValue(XDLSConstants.PropertiesValueAttr, property.Boolean);
                            break;
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool ReadXmlContent(IXDLSContentReader reader)
        {
            bool result = base.ReadXmlContent(reader);

            ReadProperty(reader as XDLSReader);

            return result;
        }
        /// <summary>
        /// Reads point and it's type from XML.
        /// </summary>
        /// <param name="reader">Reader object.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void ReadProperty(XDLSReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Read collection of points and types.
            XmlReader xmlReader = reader.InnerReader;

            if (xmlReader.LocalName == XDLSConstants.PropertyTag)
            {
                string type = reader.ReadString(XDLSConstants.PropertiesTypeAttr);
                string key = reader.ReadString(XDLSConstants.PropertiesNameAttr);

                object value = null;

                switch (type)
                {
                    case "bool":
                        value = reader.ReadBoolean(XDLSConstants.PropertiesValueAttr);
                        break;
                    case "string":
                        value = reader.ReadString(XDLSConstants.PropertiesValueAttr);
                        break;
                    case "DateTime":
                        value = reader.ReadDateTime(XDLSConstants.PropertiesValueAttr);
                        break;
                    case "int":
                        value = reader.ReadInt(XDLSConstants.PropertiesValueAttr);
                        break;
                    case "double":
                        value = reader.ReadDouble(XDLSConstants.PropertiesValueAttr);
                        break;
                    case "array":
                        //string base64 = reader.InnerReader.ReadElementString();
                        //value = Convert.FromBase64String( base64 );
                        if (!reader.InnerReader.IsEmptyElement)
                        {
                            reader.InnerReader.ReadStartElement();
                            while (reader.NodeType != XmlNodeType.Element)
                            {
                                reader.InnerReader.Read();
                            }
                            value = reader.ReadChildBinaryElement();
                        }
                        break;
                    case "clip":
                        string strValue = reader.ReadString(XDLSConstants.PropertiesValueAttr);
                        ClipDataWrapper clipdata = new ClipDataWrapper();
                        clipdata.Read(strValue);
                        value = clipdata;
                        break;
                }
                DocumentProperty property = new DocumentProperty(key, value);
                m_customList.Add(key, property);
            }
        }
        #endregion
#endif
    }
}

