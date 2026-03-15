#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Xml;


namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Represents custom Schema.
    /// </summary>
    public class CustomSchema : XmpSchema
    {
        #region Fields
        private string m_namespace;
        private string m_namespaceUri;
        #endregion

        #region Properties
        /// <summary>
        /// Sets the xmp property.
        /// </summary>
        public string this[string name]
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(name);
                return val.Value;
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(name);
                val.Value = value;

            }
        }

        /// <summary>
        /// Gets type of the schema.
        /// </summary>
        public override XmpSchemaType SchemaType
        {
            get
            {
                return XmpSchemaType.Custom;
            }
        }

        /// <summary>
        /// Gets schema prefix.
        /// </summary>
        protected override string Prefix
        {
            get
            {
                return m_namespace;
            }
        }

        /// <summary>
        /// Gets name (URI) of the schema.
        /// </summary>
        protected override string Name
        {
            get
            {
                return m_namespaceUri;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSchema"/> class.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        /// <param name="xmlNamespace">The XML namespace.</param>
        /// <param name="namespaceUri">The namespace URI.</param>
        public CustomSchema(XmpMetadata xmp, string xmlNamespace, string namespaceUri)
            : base(xmp)
        {
            if (xmlNamespace == null)
                throw new ArgumentNullException("xmlNamespace");

            if (namespaceUri == null)
                throw new ArgumentNullException("namespaceUri");

            m_namespace = xmlNamespace;
            m_namespaceUri = namespaceUri;

            Initialize();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets Xml data of the entity.
        /// </summary>
        /// <returns>XmlElement containing entity data.</returns>
        protected override XmlElement GetEntityXml()
        {
            XmlElement element = null;

            if (m_namespace != null)
            {
                element = base.GetEntityXml();
            }

            return element;
        }
        #endregion
    }
}
