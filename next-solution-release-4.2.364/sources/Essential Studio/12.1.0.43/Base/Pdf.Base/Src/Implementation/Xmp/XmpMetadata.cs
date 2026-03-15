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
using System.Xml;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Represents XMP metadata of the document.
    /// </summary>
    public class XmpMetadata : IPdfWrapper
    {
        #region Constants
        /// <summary>
        /// Xpath for the RDF element.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_xpathRdf = "/x:xmpmeta/rdf:RDF";

        /// <summary>
        /// Extensible Markup Language Namespace prefix.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_xmlnsPrefix = "xmlns";

        /// <summary>
        /// Namespace unique resource identifier for the xmlns attribute.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_xmlnsUri = "http://www.w3.org/2000/xmlns/";

        /// <summary>
        /// Xml prefix.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_xmlPefix = "xml";

        /// <summary>
        /// Namespace uri for the xml namespace.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_xmlUri = "http://www.w3.org/XML/1998/namespace";

        /// <summary>
        /// Uri of the Resource Description Framework namespace.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_rdfUri = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        /// <summary>
        /// Prefix of the PDF namespace.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_rdfPrefix = "rdf";

        /// <summary>
        /// Start packet.
        /// </summary>
        private const string c_startPacket = "begin=\"\uFEFF\" id=\"W5M0MpCehiHzreSzNTczkc9d\"";

        /// <summary>
        /// Namespace of the xmpmeta tag.
        /// </summary>
        private const string c_xmpMetaUri = "adobe:ns:meta";

        /// <summary>
        /// End packet.
        /// </summary>
        private const string c_endPacket = "end=\"r\"";

        /// <summary>
        /// Uri of the Resource Description Framework PDF/A namespace.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_rdfPdfa = "http://www.aiim.org/pdfa/ns/id/";

        /// <summary>
        /// Uri of the Extensible Authoring Publishing namespace.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_xap = "http://ns.adobe.com/xap/1.0/";

        /// <summary>
        /// Uri of the Adobe PDF schema namespace.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_pdfschema = "http://ns.adobe.com/pdf/1.3/";

        /// <summary>
        /// Uri of the Dublin Core schema namespace.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected const string c_dublinSchema = "http://purl.org/dc/elements/1.1/";
        #endregion

        #region Fields
        /// <summary>
        /// XmlDocument containing xmp data.
        /// </summary>
        private XmlDocument m_xmlDocument;

        /// <summary>
        /// Namespace manager.
        /// </summary>
        private XmlNamespaceManager m_nmpManager;

        /// <summary>
        /// Dublin Core Schema.
        /// </summary>
        private DublinCoreSchema m_dublinCoreSchema;

        /// <summary>
        /// Dublin Core Schema.
        /// </summary>
        private PagedTextSchema m_pagedTextSchemaSchema;

        /// <summary>
        /// Basic Job Ticket Schema.
        /// </summary>
        private BasicJobTicketSchema m_basicJobTicketSchema;

        /// <summary>
        /// Basic Schema.
        /// </summary>
        private BasicSchema m_basicSchema;

        /// <summary>
        /// Rights Management Schema.
        /// </summary>
        private RightsManagementSchema m_rightsManagementSchema;

        /// <summary>
        /// Indicates PDFSchema.
        /// </summary>
        private PDFSchema m_pdfSchema;

        /// <summary>
        /// PdfStream container.
        /// </summary>
        private PdfStream m_stream;
        internal bool isLoadedDocument = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets Dublin Core Schema.
        /// </summary>
        public DublinCoreSchema DublinCoreSchema
        {
            get
            {
                if (this.m_dublinCoreSchema == null)
                {
                    this.m_dublinCoreSchema = new DublinCoreSchema(this);
                }

                return this.m_dublinCoreSchema;
            }
        }

        /// <summary>
        /// Gets Dublin Core Schema.
        /// </summary>
        public PagedTextSchema PagedTextSchema
        {
            get
            {
                if (this.m_pagedTextSchemaSchema == null)
                {
                    this.m_pagedTextSchemaSchema = new PagedTextSchema(this);
                }

                return this.m_pagedTextSchemaSchema;
            }
        }

        /// <summary>
        /// Gets Basic Job Ticket Schema.
        /// </summary>
        public BasicJobTicketSchema BasicJobTicketSchema
        {
            get
            {
                if (this.m_basicJobTicketSchema == null)
                {
                    this.m_basicJobTicketSchema = new BasicJobTicketSchema(this);
                }

                return this.m_basicJobTicketSchema;
            }
        }

        /// <summary>
        /// Gets Basic Schema.
        /// </summary>
        public BasicSchema BasicSchema
        {
            get
            {
                if (this.m_basicSchema == null)
                {
                    this.m_basicSchema = new BasicSchema(this);
                }

                return this.m_basicSchema;
            }
        }

        /// <summary>
        /// Gets Rights Management Schema.
        /// </summary>
        public RightsManagementSchema RightsManagementSchema
        {
            get
            {
                if (this.m_rightsManagementSchema == null)
                {
                    this.m_rightsManagementSchema = new RightsManagementSchema(this);
                }

                return this.m_rightsManagementSchema;
            }
        }

        /// <summary>
        /// Gets a schema specifying properties used with Adobe PDF documents.
        /// </summary>
        public PDFSchema PDFSchema
        {
            get
            {
                if (this.m_pdfSchema == null)
                {
                    this.m_pdfSchema = new PDFSchema(this);
                }

                return this.m_pdfSchema;
            }
        }

        /// <summary>
        /// Gets XMP data in XML format.
        /// </summary>
        public XmlDocument XmlData
        {
            get
            {
                return this.m_xmlDocument;
            }
        }

        /// <summary>
        /// Gets namespace manager of the Xmp metadata.
        /// </summary>
        public XmlNamespaceManager NamespaceManager
        {
            get
            {
                return this.m_nmpManager;
            }
        }

        /// <summary>
        /// Gets xmpmeta element of the packet.
        /// </summary>
        internal XmlElement Xmpmeta
        {
            get
            {
                XmlNode node = this.XmlData.SelectSingleNode("/x:xmpmeta", this.NamespaceManager);
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }

                return node as XmlElement;
            }
        }

        /// <summary>
        /// Gets RDF element of the packet.
        /// </summary>
        internal XmlElement Rdf
        {
            get
            {
                string path = c_xpathRdf;
                if (!this.XmlData.DocumentElement.Prefix.Equals("x"))
                {
                    path = this.XmlData.DocumentElement.Name;
                }
                XmlNode node = this.XmlData.SelectSingleNode(path, this.NamespaceManager);
                if (node == null)
                {
                    node = this.XmlData.SelectSingleNode("/"+this.XmlData.DocumentElement.Name+"/rdf:RDF", this.NamespaceManager);
                    if(node==null)
                    throw new ArgumentNullException("node");
                }

                return node as XmlElement;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmpMetadata"/> class.
        /// </summary>
        public XmpMetadata(PdfDocumentInformation documentInfo)
        {
            this.Init(documentInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmpMetadata"/> class.
        /// </summary>
        /// <param name="xmp">The XMP.</param>
        public XmpMetadata(XmlDocument xmp)
        {
            if (xmp == null)
            {
                throw new ArgumentNullException("xmpMetadata");
            }

            this.Load(xmp);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Loads XMP from the XML.
        /// </summary>
        /// <param name="xmp">XMP data in XMLDocument.</param>
        /// <remarks>If there was any data in this XMP, it will be replaced by the data from the XML.</remarks>
        public void Load(XmlDocument xmp)
        {
            if (xmp == null)
            {
                throw new ArgumentNullException("xmp");
            }

            this.Reset();

            this.m_xmlDocument = xmp;
            this.m_nmpManager = new XmlNamespaceManager(this.m_xmlDocument.NameTable);

            // Import namespaces.
            this.ImportNamespaces(m_xmlDocument.DocumentElement, m_nmpManager);
        }

        /// <summary>
        /// Adds schema to the XMP in XML format.
        /// </summary>
        /// <param name="schema">XMP schema in XML format.</param>
        /// <remarks>If XMP already contains such schema - there will be two equal schemas at the xmp.</remarks>
        public void Add(XmlElement schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }

            schema = this.XmlData.ImportNode(schema, true) as XmlElement;

            // Import namespaces.
            this.ImportNamespaces(schema, this.m_nmpManager);

            // Append schema.
            this.Rdf.AppendChild(schema);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes a packet of the XMP.
        /// </summary>
        private void Init(PdfDocumentInformation documentInfo)
        {
            this.m_xmlDocument = new XmlDocument();
            this.m_nmpManager = new XmlNamespaceManager(this.XmlData.NameTable);
            this.m_stream = new PdfStream();

            this.InitStream();
            this.CreateStartPacket();
            this.CreateXmpmeta();
            this.CreateRdf(documentInfo);
            this.CreateEndPacket();
        }

        /// <summary>
        /// Initializes stream.
        /// </summary>
        private void InitStream()
        {
            this.m_stream.BeginSave += new SavePdfPrimitiveEventHandler(this.BeginSave);
            this.m_stream.EndSave += new SavePdfPrimitiveEventHandler(this.EndSave);

            this.m_stream[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Metadata);
            this.m_stream[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.XML);
            this.m_stream.Compress = false;
        }

        /// <summary>
        /// Creates packet element.
        /// </summary>
        private void CreateStartPacket()
        {
            string data = c_startPacket;
            XmlProcessingInstruction packet = this.XmlData.CreateProcessingInstruction("xpacket", data);
            this.XmlData.AppendChild(packet);
        }

        /// <summary>
        /// Creates xmpmeta element.
        /// </summary>
        private void CreateXmpmeta()
        {
            XmlElement xmpmeta = this.CreateElement("x", "xmpmeta", c_xmpMetaUri);           
            this.XmlData.AppendChild(xmpmeta);
        }

        /// <summary>
        /// Creates Resource Description Framework element.
        /// </summary>
        private void CreateRdf(PdfDocumentInformation documentInfo)
        {
            XmlElement rdf = this.CreateElement("rdf", "RDF", c_rdfUri);

            if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
            {
                //Pdf Schema
                if (!string.IsNullOrEmpty(documentInfo.Producer) || !string.IsNullOrEmpty(documentInfo.Keywords))
                {
                    this.NamespaceManager.AddNamespace("pdf", c_pdfschema);
                    XmlElement rdfDescription = this.CreateElement("rdf", "Description", c_pdfschema);
                    XmlAttribute xmlnsPdf = this.CreateAttribute("xmlns", "pdf", c_pdfschema, c_pdfschema);
                    rdfDescription.Attributes.Append(xmlnsPdf);
                    if (!string.IsNullOrEmpty(documentInfo.Producer))
                    {
                        XmlElement producer = this.CreateElement("pdf", "Producer", c_pdfschema);
                        producer.InnerText = documentInfo.Producer;
                        rdfDescription.AppendChild(producer);
                    }
                    if (!string.IsNullOrEmpty(documentInfo.Keywords))
                    {
                        XmlElement keyword = this.CreateElement("pdf", "Keywords", c_pdfschema);
                        keyword.InnerText = documentInfo.Keywords;
                        rdfDescription.AppendChild(keyword);
                    }
                    this.Xmpmeta.AppendChild(rdfDescription);
                    rdf.AppendChild(rdfDescription);
                }

                //Dublin Core Schema
                XmlElement dublinDescription = this.CreateElement("rdf", "Description", c_pdfschema);
                XmlAttribute dublinxmlns = this.CreateAttribute("xmlns", "dc", c_dublinSchema, c_dublinSchema);
                dublinDescription.Attributes.Append(dublinxmlns);
                XmlElement format = this.CreateElement("dc", "format", c_dublinSchema);
                format.InnerText = "application/pdf";
                dublinDescription.AppendChild(format);
                CreateDublinCoreContainer(rdf, dublinDescription, "title", documentInfo.Title, true, XmpArrayType.Alt);
                CreateDublinCoreContainer(rdf, dublinDescription, "description", documentInfo.Subject, true, XmpArrayType.Alt);
                CreateDublinCoreContainer(rdf, dublinDescription, "subject", documentInfo.Keywords, false, XmpArrayType.Bag);
                CreateDublinCoreContainer(rdf, dublinDescription, "creator", documentInfo.Author, false, XmpArrayType.Seq);
                this.NamespaceManager.AddNamespace("pdfaid", c_rdfPdfa);
                XmlElement pdfA = this.CreateElement("rdf", "Description", c_rdfUri);
                XmlAttribute about = this.CreateAttribute("rdf", "about", c_rdfUri, " ");
                XmlAttribute part = this.CreateAttribute("pdfaid", "part", c_rdfPdfa, "1");
                XmlAttribute conformance = this.CreateAttribute("pdfaid", "conformance", c_rdfPdfa, "B");
                pdfA.Attributes.Append(about);
                pdfA.Attributes.Append(part);
                pdfA.Attributes.Append(conformance);
                this.Xmpmeta.AppendChild(pdfA);
                rdf.AppendChild(pdfA);

                //Add Creation Date
                //NamespaceManager.AddNamespace("xamp", c_xap);
                //XmlElement docInfo = CreateElement("rdf", "Description", c_xap);
                //XmlAttribute docInfo_about = CreateAttribute("rdf", "about", c_rdfUri, "");

                //XmlAttribute docInfo_createDate = CreateAttribute("xmp", "CreateDate", c_xap, PdfDocumentInformation.DocumentCreationDate.ToString());
                //docInfo.Attributes.Append(docInfo_about);
                //docInfo.Attributes.Append(docInfo_createDate);
                //rdf.AppendChild(docInfo);
            }

            this.Xmpmeta.AppendChild(rdf);
        }

        /// <summary>
        /// Creates a Dublin core containers.
        /// </summary>
        private void CreateDublinCoreContainer(XmlElement rdf, XmlElement dublinDesc, string containerName, string value, bool defaultLang, XmpArrayType element)
        {
            if (!string.IsNullOrEmpty(value))
            {
                XmlElement title = this.CreateElement("dc", containerName, c_dublinSchema);
                XmlElement alt = this.CreateElement("rdf", element.ToString(), c_dublinSchema);
                XmlElement li = this.CreateElement("rdf", "li", c_dublinSchema);

                li.InnerText = value;
                alt.AppendChild(li);
                title.AppendChild(alt);
                dublinDesc.AppendChild(title);
                
                if (defaultLang)
                {
                    XmlAttribute lang = this.CreateAttribute("xml", "lang", c_dublinSchema, "x-default");
                    li.Attributes.Append(lang);
                }
            }
            rdf.AppendChild(dublinDesc);
        }
        /// <summary>
        /// Creates packet element.
        /// </summary>
        private void CreateEndPacket()
        {
            string data = c_endPacket;
            XmlProcessingInstruction packet = this.XmlData.CreateProcessingInstruction("xpacket", data);
            this.XmlData.AppendChild(packet);
        }

        /// <summary>
        /// Resets current xmp metadata.
        /// </summary>
        private void Reset()
        {
            this.m_xmlDocument = null;
            this.m_nmpManager = null;
            this.m_dublinCoreSchema = null;
        }

        /// <summary>
        /// Imports all namespaces to the namespace manager.
        /// </summary>
        /// <param name="elm">Current element.</param>
        /// <param name="nsm">Namespace Manager.</param>
        private void ImportNamespaces(XmlElement elm, XmlNamespaceManager nsm)
        {
            if (elm == null)
            {
                throw new ArgumentNullException("elm");
            }

            if (nsm == null)
            {
                throw new ArgumentNullException("nsm");
            }

            string prefix = elm.Prefix;
            string namespaceURI = elm.NamespaceURI;

            // Add namespace.
            if (prefix != null && prefix.Length > 0 &&
                namespaceURI != null && !nsm.HasNamespace(prefix))
            {
                nsm.AddNamespace(prefix, namespaceURI);
            }

            // Run through children.
            if (elm.HasChildNodes)
            {
                XmlNode current = elm.FirstChild;

                while (current != null)
                {
                    XmlNode childNode = current;
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        this.ImportNamespaces(childNode as XmlElement, nsm);
                    }

                    current = current.NextSibling;
                }
            }
        }
        #endregion

        #region xml entities add methods
        /// <summary>
        /// Creates element.
        /// </summary>
        /// <param name="name">Name of the element.</param>
        /// <returns>Created element.</returns>
        internal XmlElement CreateElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return this.XmlData.CreateElement(name);
        }

        /// <summary>
        /// Creates element.
        /// </summary>
        /// <param name="prefix">Prefix of the element.</param>
        /// <param name="localName">Local name of the element.</param>
        /// <param name="namespaceURI">Namespace URI.</param>
        /// <returns>Created element.</returns>
        internal XmlElement CreateElement(string prefix, string localName, string namespaceURI)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            if (localName == null)
            {
                throw new ArgumentNullException("localName");
            }

            namespaceURI = this.AddNamespace(prefix, namespaceURI);

            XmlElement elm = this.XmlData.CreateElement(prefix, localName, namespaceURI);

            return elm;
        }

        /// <summary>
        /// Creates attribute.
        /// </summary>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="value">Value of the attribute.</param>
        /// <returns>Created XmlAttribute.</returns>
        internal XmlAttribute CreateAttribute(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            XmlAttribute attr = this.XmlData.CreateAttribute(name);
            attr.Value = value;

            return attr;
        }

        /// <summary>
        /// Creates attribute.
        /// </summary>
        /// <param name="prefix">Prefix of the attribute.</param>
        /// <param name="localName">Name of the attribute.</param>
        /// <param name="namespaceURI">Namespace Uri.</param>
        /// <param name="value">Value of the attribute.</param>
        /// <returns>Created XmlAttribute.</returns>
        internal XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI, string value)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            if (localName == null)
            {
                throw new ArgumentNullException("localName");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            namespaceURI = this.AddNamespace(prefix, namespaceURI);

            XmlAttribute attr = this.XmlData.CreateAttribute(prefix, localName, namespaceURI);
            attr.Value = value;

            return attr;
        }

        /// <summary>
        /// Adds namespace.
        /// </summary>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="namespaceURI">Namespace Uri.</param>
        /// <returns>Uri of the namespace.</returns>
        internal string AddNamespace(string prefix, string namespaceURI)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            string result = namespaceURI;

            if (!this.NamespaceManager.HasNamespace(prefix) &&
                prefix != c_xmlPefix && prefix != c_xmlnsPrefix)
            {
                if (namespaceURI == null)
                {
                    throw new ArgumentNullException("namespaceURI");
                }

                this.NamespaceManager.AddNamespace(prefix, namespaceURI);
            }
            else
            {
                result = this.NamespaceManager.LookupNamespace(prefix);
            }

            return result;
        }
        #endregion

        #region IPdfWrapper Members

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return this.m_stream;
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Raises before stream saves.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="ars">Event data.</param>
        private void BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            // Save Xml to the stream.
            this.XmlData.Save(this.m_stream.InternalStream);
        }

        /// <summary>
        /// Raises after stream saves.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="ars">Event data.</param>
        private void EndSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            // Reset stream data.
            this.m_stream.Clear();
        }
        #endregion
    }
}
