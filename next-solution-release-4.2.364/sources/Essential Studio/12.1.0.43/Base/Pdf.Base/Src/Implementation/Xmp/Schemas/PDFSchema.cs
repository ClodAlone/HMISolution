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

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// This schema specifies properties used with Adobe PDF documents.
    /// </summary>
    public class PDFSchema : XmpSchema
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "pdf";
        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http://ns.adobe.com/pdf/1.3/";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_Keywords = "Keywords";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_PDFVersion = "PDFVersion";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_Producer = "Producer";
        #endregion

        #region Properties
        /// <summary>
        /// Gets type of the Schema.
        /// </summary>
        public override XmpSchemaType SchemaType
        {
            get
            {
                return XmpSchemaType.PDFSchema;
            }
        }

        /// <summary>
        /// Gets name pf the schema.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override string Name
        {
            get
            {
                return c_name;
            }
        }

        /// <summary>
        /// Gets prefix of the schema.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override string Prefix
        {
            get
            {
                return c_prefix;
            }
        }
        #endregion

        #region Class Schema properties
        /// <summary>
        /// Gets or sets keywords of the document.
        /// </summary>
        public string Keywords
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_Keywords);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Keywords");

                XmpSimpleType val = GetSimpleProperty(c_Keywords);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the PDF file version (for example: 1.0, 1.3, and so on).
        /// </summary>
        public string PDFVersion
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_PDFVersion);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("PDFVersion");

                XmpSimpleType val = GetSimpleProperty(c_PDFVersion);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the tool that created the PDF document.
        /// </summary>
        public string Producer
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_Producer);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Producer");

                XmpSimpleType val = GetSimpleProperty(c_Producer);
                val.Value = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates schema object.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal PDFSchema(XmpMetadata xmp)
            : base(xmp)
        {
        }
        #endregion
    }
}
