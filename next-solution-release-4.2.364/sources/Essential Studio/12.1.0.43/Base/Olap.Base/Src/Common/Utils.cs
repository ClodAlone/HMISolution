//-------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.Runtime.Serialization;


#if !SILVERLIGHT
namespace Syncfusion.Olap.Common
#else
namespace Syncfusion.OlapSilverlight.Common
#endif
{
    /// <summary>
    /// Query generator utility class
    /// </summary>
    internal class Utils
    {
        #region Internal Methods
        /// <summary>
        /// Quotes the identifier.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal static string QuoteIdentifier(string expression)
        {
            return "[" + expression + "]";
        }
        #endregion
    }
#if !SILVERLIGHT
    [Serializable]
#endif    
    [XmlRoot("dictionary")]
    /// <summary>
    /// A Dictionary will hold the key-value pair which can be serialize.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable

#if !SILVERLIGHT
    , ISerializable
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        public SerializableDictionary() { }

        #region IXmlSerializable Members

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is de-serialized.</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = null;
#if SILVERLIGHT
            System.Runtime.Serialization.DataContractSerializer dcs = null;
#endif
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty) return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("Type");
                string typeName = reader.ReadContentAsString();
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value;
                if (!string.IsNullOrEmpty(typeName) && typeName != " ")
                {
                    var type = System.Type.GetType(typeName);
                    if (type == null) type = System.Type.GetType(typeName.Replace(".Olap.", ".OlapSilverlight."));
                    if (type == null) type = System.Type.GetType(typeName.Replace(".OlapSilverlight.", ".Olap."));
#if SILVERLIGHT
                    if (type.GetCustomAttributes(typeof(System.Runtime.Serialization.DataContractAttribute), false).Length != 0 && typeName == "Syncfusion.OlapSilverlight.Data.CellSet")
                    {
                        dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(Syncfusion.OlapSilverlight.Data.CellSet));
                        value = (TValue)dcs.ReadObject(reader);
                    }
                    else
#endif
                    {
                        valueSerializer = new XmlSerializer(type);
                        value = (TValue)valueSerializer.Deserialize(reader);
                    }
                }
                else
                {
                    reader.ReadContentAsString();
                    value = default(TValue);
                }
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = null;
#if SILVERLIGHT
            System.Runtime.Serialization.DataContractSerializer dcs = null;
#endif
            
            foreach (TKey key in this.Keys)
            {
                TValue value = this[key];
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("Type");
                writer.WriteValue(value != null ? value.GetType().ToString() : " ");
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                if (value != null)
                {
#if SILVERLIGHT
                    if (value.GetType().GetCustomAttributes(typeof(System.Runtime.Serialization.DataContractAttribute), false).Length != 0 && value.GetType().Name == "CellSet")
                    {
                        dcs = new System.Runtime.Serialization.DataContractSerializer(value.GetType());
                        dcs.WriteObject(writer, value);
                    }
                    else
#endif
                    {
                        valueSerializer = new XmlSerializer(value.GetType()); ;
                        valueSerializer.Serialize(writer, value);
                    }
                }
                else
                    writer.WriteValue(" ");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        #endregion

#if !SILVERLIGHT
        #region ISerializable Members

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var list = new List<ValuePair<TKey, TValue>>();
            foreach (var item in this)
                list.Add(new ValuePair<TKey, TValue>() { Key = item.Key, Value = item.Value });
            info.AddValue("data", list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
        {
            var list = info.GetValue("data", typeof(List<ValuePair<TKey, TValue>>)) as List<ValuePair<TKey, TValue>>;
            foreach (var item in list)
                this.Add(item.Key, item.Value);
        }

        [Serializable]
        public struct ValuePair<K, V>
        {
            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            /// <value>The key.</value>
            public K Key { get; set; }
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public V Value { get; set; }
        }

        #endregion
#endif
    }
}
