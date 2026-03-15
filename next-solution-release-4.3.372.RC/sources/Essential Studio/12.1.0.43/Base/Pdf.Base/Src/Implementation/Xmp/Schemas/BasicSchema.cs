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
    /// Represents Basic Schema.
    /// </summary>
    public class BasicSchema : XmpSchema
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "xap";
        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http://ns.adobe.com/xap/1.0/";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propAdvisory = "Advisory";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propIdentifier = "Identifier";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propLabel = "Label";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propNickname = "Nickname";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propBaseUrl = "BaseURL";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propCreatorTool = "CreatorTool";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propCreateData = "CreateDate";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propMetadataDate = "MetadataDate";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propModifyDate = "ModifyDate";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propThumbnail = "Thumbnails";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propRating = "Rating";
        #endregion

        #region Properties
        /// <summary>
        /// Gets type of the Schema.
        /// </summary>
        public override XmpSchemaType SchemaType
        {
            get
            {
                return XmpSchemaType.BasicSchema;
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
                if (base.Xmp.XmlData.InnerXml.ToString().Contains("xmlns:xmp"))
                    return "xmp";
                else
                    return c_prefix;
            }
        }
        #endregion

        #region Schema properties
        /// <summary>
        /// Gets an unordered array specifying properties that were
        /// edited outside the authoring application.
        /// </summary>
        public XmpArray Advisory
        {
            get
            {
                XmpArray val = GetArray(c_propAdvisory, XmpArrayType.Bag);
                return val;
            }
        }
        /// <summary>
        /// Gets an unordered array of text strings that unambiguously
        /// identify the resource within a given context. An array
        /// item may be qualified with xmpidq:Scheme to denote
        /// the formal identification system to which that identifier
        /// conforms.
        /// </summary>
        public XmpArray Identifier
        {
            get
            {
                XmpArray val = GetArray(c_propIdentifier, XmpArrayType.Bag);
                return val;
            }
        }
        /// <summary>
        /// Gets or sets a word or short phrase that identifies a document as a
        /// member of a user-defined collection. Used to organize
        /// documents in a file browser.
        /// </summary>
        public string Label
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_propLabel);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Label");

                XmpSimpleType val = GetSimpleProperty(c_propLabel);
                val.Value = value;
            }
        }
        /// <summary>
        /// Gets or sets a short informal name for the resource.
        /// </summary>
        public string Nickname
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_propNickname);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Nickname");

                XmpSimpleType val = GetSimpleProperty(c_propNickname);
                val.Value = value;
            }
        }
        /// <summary>
        /// Gets or sets The base URL for relative URLs in the document
        /// content. If this document contains Internet links, and
        /// those links are relative, they are relative to this base
        /// URL.
        /// </summary>
        public Uri BaseURL
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_propBaseUrl);
                return val.GetUri();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("BaseURL");

                XmpSimpleType val = GetSimpleProperty(c_propBaseUrl);
                val.SetUri(value);
            }
        }
        /// <summary>
        /// Gets or sets The name of the first known tool used to create the
        /// resource. If history is present in the metadata, this value
        /// should be equivalent to that of xmpMM:History�s
        /// softwareAgent property.
        /// </summary>
        public string CreatorTool
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_propCreatorTool);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("CreatorTool");

                XmpSimpleType val = GetSimpleProperty(c_propCreatorTool);
                val.Value = value;
            }
        }
        /// <summary>
        /// Gets or sets the date and time the resource was originally created.
        /// </summary>
        public DateTime CreateDate
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_propCreateData);
                return val.GetDateTime();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_propCreateData);
                val.SetDateTime(value);
            }
        }
        /// <summary>
        /// Gets or sets the date and time that any metadata for this resource
        /// was last changed. It should be the same as or more
        /// recent than xmp:ModifyDate.
        /// </summary>
        public DateTime MetadataDate
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_propMetadataDate);
                return val.GetDateTime();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_propMetadataDate);
                val.SetDateTime(value);
            }
        }
        /// <summary>
        /// Gets  sets the date and time the resource was last modified.
        /// </summary>
        public DateTime ModifyDate
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_propModifyDate);
                return val.GetDateTime();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_propModifyDate);
                val.SetDateTime(value);
            }
        }
        /// <summary>
        /// Gets an alternative array of thumbnail images for a file,
        /// which can differ in characteristics such as size or image
        /// encoding.
        /// </summary>
        public XmpArray Thumbnails
        {
            get
            {
                XmpArray val = GetArray(c_propThumbnail, XmpArrayType.Alt);
                return val;
            }
        }
        /// <summary>
        /// Gets a number that indicates a document�s status relative to
        /// other documents, used to organize documents in a file
        /// browser. Values are user-defined within an application defined range.
        /// </summary>
        public XmpArray Rating
        {
            get
            {
                XmpArray val = GetArray(c_propRating, XmpArrayType.Bag);
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
        protected internal BasicSchema(XmpMetadata xmp)
            : base(xmp)
        {
        }
        #endregion
    }
}
