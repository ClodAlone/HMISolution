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
    /// Represents Dublin Core Schema.
    /// </summary>
    public class DublinCoreSchema : XmpSchema
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "dc";

        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http://purl.org/dc/elements/1.1/";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_coverage = "coverage";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_identifier = "identifier";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_format = "format";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_source = "source";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_subject = "subject";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_type = "type";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_contributor = "contributor";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_creator = "creator";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_date = "date";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_publisher = "publisher";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_relation = "relation";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_description = "description";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_rights = "rights";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_title = "title";

        /// <summary>
        /// Mime-Type of the document.
        /// </summary>
        private const string c_mimeType = "application/pdf";
        #endregion

        #region Properties
        /// <summary>
        /// Gets type of the Schema.
        /// </summary>
        public override XmpSchemaType SchemaType
        {
            get
            {
                return XmpSchemaType.DublinCoreSchema;
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
        /// Gets contributors to the resource (other than the authors).
        /// </summary>
        public XmpArray Contributor
        {
            get
            {
                XmpArray val = GetArray(c_contributor, XmpArrayType.Bag);
                return val;
            }
        }

        /// <summary>
        /// Gets or sets the extent or scope of the resource.
        /// </summary>
        public string Coverage
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_coverage);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Coverage");

                XmpSimpleType val = GetSimpleProperty(c_coverage);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets the authors of the resource (listed in order of precedence, if
        /// significant).
        /// </summary>
        public XmpArray Creator
        {
            get
            {
                XmpArray val;
                if (base.XmlData.InnerXml.Contains("rdf:Bag"))
                    val = GetArray(c_creator, XmpArrayType.Bag);
                else
                    val = GetArray(c_creator, XmpArrayType.Seq);
                return val;
            }
        }

        /// <summary>
        /// Gets date(s) that something interesting happened to the resource.
        /// </summary>
        public XmpArray Date
        {
            get
            {
                XmpArray val = GetArray(c_date, XmpArrayType.Seq);
                return val;
            }
        }

        /// <summary>
        /// Gets or sets a textual description of the content of the resource. Multiple
        /// values may be present for different languages.
        /// </summary>
        public XmpLangArray Description
        {
            get
            {
                XmpLangArray val = GetLangArray(c_description);
                return val;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the resource.
        /// </summary>
        public string Identifier
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_identifier);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Identifier");

                XmpSimpleType val = GetSimpleProperty(c_identifier);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets publishers.
        /// </summary>
        public XmpArray Publisher
        {
            get
            {
                XmpArray val = GetArray(c_publisher, XmpArrayType.Bag);
                return val;
            }
        }

        /// <summary>
        /// Gets relationships to other documents.
        /// </summary>
        public XmpArray Relation
        {
            get
            {
                XmpArray val = GetArray(c_relation, XmpArrayType.Bag);
                return val;
            }
        }

        /// <summary>
        /// Gets informal rights statement, selected by language.
        /// </summary>
        public XmpLangArray Rights
        {
            get
            {
                XmpLangArray val = GetLangArray(c_rights);
                return val;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the work from which this resource was derived.
        /// </summary>
        public string Source
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_source);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Source");

                XmpSimpleType val = GetSimpleProperty(c_source);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets an unordered array of descriptive phrases or keywords that
        /// specify the topic of the content of the resource.
        /// </summary>
        public XmpArray Sublect
        {
            get
            {
                XmpArray val = GetArray(c_subject, XmpArrayType.Bag);
                return val;
            }
        }

        /// <summary>
        /// Gets the title of the document, or the name given to the resource.
        /// Typically, it will be a name by which the resource is
        /// formally known.
        /// </summary>
        public XmpLangArray Title
        {
            get
            {
                XmpLangArray val = GetLangArray(c_title);
                return val;
            }
        }

        /// <summary>
        /// Gets a document type; for example, novel, poem, or working
        /// paper.
        /// </summary>
        public XmpArray Type
        {
            get
            {
                XmpArray val = GetArray(c_type, XmpArrayType.Bag);
                return val;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates schema object.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal DublinCoreSchema(XmpMetadata xmp)
            : base(xmp)
        {
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Initializes object.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateEntity()
        {
            base.CreateEntity();

            XmpSimpleType format = GetSimpleProperty(c_format);
            format.Value = c_mimeType;
        }
        #endregion
    }
}
