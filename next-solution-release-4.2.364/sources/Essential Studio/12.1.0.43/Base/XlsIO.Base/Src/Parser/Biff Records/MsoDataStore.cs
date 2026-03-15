#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.CompoundFile.XlsIO;
using System.IO;
using System.Xml;
using Syncfusion.XlsIO.Implementation.XmlSerialization;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    class MsoDataStore 
    {
        private WorkbookImpl m_book;
        private IApplication m_application;
        private ICompoundStorage m_storage;
        private int m_Count;

        /// <summary>
        /// Main class constructor. Application and Parent properties are set.
        /// </summary>
        /// <param name="book">WorkbookImpl</param>
        /// <param name="compoundStorage">CompoundStorage</param>
        public MsoDataStore(ICompoundStorage compoundStorage, WorkbookImpl book)
        {
            m_book = book;
            m_storage = compoundStorage;
            //m_Count = compoundStorage.Storages.Count();
        }
        /// <summary>
        /// Main class constructor. Application and Parent properties are set.
        /// </summary>
        public void ParseMsoDataStore()
        {
            List<string> schemas = null;
            ICustomXmlPartCollection customXmlParts = m_book.CustomXmlparts;

            foreach (string items in m_storage.Storages)
            {
                ICompoundStorage value=m_storage.OpenStorage(items);

                string XmlId = null;

                    Stream propertystream = value.OpenStream("Properties") as Stream;
                    Stream xmlstream = value.OpenStream("Item") as Stream;

                    XmlId = ParseCustomXmlItemProperties(propertystream, ref schemas);
                    ParseCustomXmlParts(xmlstream, customXmlParts, XmlId, schemas);
                
            }
          
        }
        /// <summary>
        /// Parse Customxml parts
        /// </summary>
        /// <param name="customXmlParts">CustomXmlParts</param>
        /// <param name="schemas">Schemas collection</param>
        /// <param name="stream">Stream Data</param>
        /// <param name="XmlId">XmlID</param>
        private void ParseCustomXmlParts(Stream stream, ICustomXmlPartCollection customXmlParts, string XmlId, List<string> schemas)
        {
            if (stream != null)
            {
                stream.Position = 0;

                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }

                    ICustomXmlPart customxmlpart = customXmlParts.Add(XmlId, ms.ToArray());

                    if (schemas != null && schemas.Count > 0)
                    {
                        foreach (string schema in schemas)
                        {
                            customxmlpart.Schemas.Add(schema);
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Parses CustomXml Item Properties
        /// </summary>
        /// <param name="propertystream">Property Stream</param>
        /// <param name="schemas">SchemasCollection</param>
        private string ParseCustomXmlItemProperties(Stream propertystream, ref List<string> schemas)
        {
            string id;

            propertystream.Position = 0;

            XmlReader reader = UtilityMethods.CreateReader(propertystream);

            id = ParseItemProperties(reader, ref schemas);

            return id;
        }
        /// <summary>
        /// Parses Item properties
        /// </summary>
        /// <param name="reader">Xml Reader</param>
        /// <param name="schemas">SchemasCollection</param>
        private string ParseItemProperties(XmlReader reader, ref List<string> schemas)
        {
            string value = null;

            if (reader == null)
                throw new ArgumentNullException("reader");

            List<string> schemaValues = new List<string>();

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Excel2007Serializator.DataStoreItem:
                            if (reader.MoveToAttribute(Excel2007Serializator.ItemIdAttribute))
                                value = reader.Value;
                            reader.Read();
                            break;

                        case Excel2007Serializator.CustomXmlSchemaReferences:
                            ParseschemaReference(reader, ref schemas);
                            reader.Read();
                            break;
                    }
                }
            }
            return value;

        }
        /// <summary>
        /// Parses Schema Reference and add to collection
        /// </summary>
        /// <param name="reader">Xml Reader</param>
        /// <param name="schemas">SchemasCollection</param>
        private void ParseschemaReference(XmlReader reader, ref List<string> schemas)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");


            if (reader.LocalName != Excel2007Serializator.CustomXmlSchemaReferences)
                throw new XmlException("Wrong xml tag");

            schemas = new List<string>();

            if (!reader.IsEmptyElement)
            {
                reader.Read();

                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    switch (reader.LocalName)
                    {
                        case Excel2007Serializator.CustomXmlSchemaReference:
                            ParseSchemaRef(reader, ref schemas);
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
            }


        }
        /// <summary>
        /// Parses Individual schemas
        /// </summary>
        /// <param name="reader">Xml Reader</param>
        /// <param name="schemas">Schemas Collection</param>
        private void ParseSchemaRef(XmlReader reader, ref List<string> schemas)
        {
            string uri;

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != Excel2007Serializator.CustomXmlSchemaReference)
                throw new XmlException("Wrong xml tag");

            if (reader.MoveToAttribute(Excel2007Serializator.CustomXmlUriAttribute))
            {
                uri = reader.Value;
                schemas.Add(uri);
            }

            reader.Read();
        }
        /// <summary>
        /// Serialize CustomXml Parts
        /// </summary>
        internal void SerializeMetaStore()
        {
            ICustomXmlPartCollection customXmlParts = m_book.CustomXmlparts;

            if (customXmlParts != null && customXmlParts.Count > 0)
            {
                ICompoundStorage storage=m_storage.CreateStorage("MsoDataStore");

                for (int i = 0; i < customXmlParts.Count; i++)
                {
                    ICompoundStorage itemstorage = storage.CreateStorage(string.Format("cxds{0}", i));

                    Stream propertySteam = itemstorage.CreateStream("Properties") as Stream;
                    Stream Xmlstream = itemstorage.CreateStream("Item") as Stream;

                    ICustomXmlPart customXmlPart = customXmlParts[i];
                    
                    SerializeCustomXmlProperty(propertySteam, customXmlPart);
                    SerializeCustomXmlPart(Xmlstream, customXmlPart.Data);
                }
            }

        }
        /// <summary>
        /// Serialize Custom Xml properties
        /// </summary>
        /// <param name="customXmlPart">Custom Xml Parts</param>
        /// <param name="itemsteam">Item Stream</param>
        private void SerializeCustomXmlProperty(Stream itemsteam, ICustomXmlPart customXmlPart)
        {
            byte[] propertyValue;
            MemoryStream stream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(stream);
            XmlWriter writer = UtilityMethods.CreateWriter(streamWriter/*, Encoding.UTF8*/ );

            SerializeCustomXmlPartProperty(writer, customXmlPart);
            writer.Flush();
            stream.Position = 0;
            propertyValue = stream.ToArray();

            itemsteam.Write(propertyValue,0,propertyValue.Length);
            
        }
        /// <summary>
        /// Serialize CustomXml Properties
        /// </summary>
        /// <param name="customXmlPart">Custom Xml Parts</param>
        /// <param name="writer">XmlWriter</param>
        public void SerializeCustomXmlPartProperty(XmlWriter writer, ICustomXmlPart customXmlPart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (customXmlPart == null)
                throw new ArgumentNullException("customXmlPart");

            string id = customXmlPart.Id;
            ICustomXmlSchemaCollection schemaCollection = customXmlPart.Schemas;

            writer.WriteStartDocument(true);
            writer.WriteStartElement(Excel2007Serializator.ItemPropertiesPrefix, Excel2007Serializator.DataStoreItem, Excel2007Serializator.CustomXmlNameSpace);

            writer.WriteAttributeString(Excel2007Serializator.ItemPropertiesPrefix, Excel2007Serializator.CustomXmlItemID, null, id);

            if (schemaCollection != null && schemaCollection.Count > 0)
            {
                writer.WriteStartElement(Excel2007Serializator.ItemPropertiesPrefix, Excel2007Serializator.CustomXmlSchemaReferences, null);

                foreach (string schemas in schemaCollection)
                {
                    writer.WriteStartElement(Excel2007Serializator.ItemPropertiesPrefix, Excel2007Serializator.CustomXmlSchemaReference, null);
                    writer.WriteAttributeString(Excel2007Serializator.ItemPropertiesPrefix, Excel2007Serializator.ItemPrpertiesUri, null, schemas);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize Xml Data in Storage
        /// </summary>
        /// <param name="data">Xml Data</param>
        /// <param name="storage">Storage</param>
        private void SerializeCustomXmlPart(Stream storage, byte[] data)
        {
            if (data != null)
            {
                storage.Write(data, 0, data.Length);
            }
        }


    }
}
