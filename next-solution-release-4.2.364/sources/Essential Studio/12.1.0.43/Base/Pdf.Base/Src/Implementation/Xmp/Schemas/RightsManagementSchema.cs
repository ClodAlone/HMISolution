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
    /// Represents Rights Management Schema.
    /// </summary>
    public class RightsManagementSchema : XmpSchema
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "xmpRights";

        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http://ns.adobe.com/xap/1.0/rights/";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_Certificate = "Certificate";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_Marked = "Marked";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_Owner = "Owner";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_UsageTerms = "UsageTerms";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_WebStatement = "WebStatement";
        #endregion

        #region Properties
        /// <summary>
        /// Gets type of the Schema.
        /// </summary>
        public override XmpSchemaType SchemaType
        {
            get
            {
                return XmpSchemaType.RightsManagementSchema;
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
        /// Gets or sets online rights management certificate.
        /// </summary>
        public Uri Certificate
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_Certificate);
                return val.GetUri();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Certificate");

                XmpSimpleType val = GetSimpleProperty(c_Certificate);
                val.SetUri(value);
            }
        }

        /// <summary>
        /// Gets or sets indicates that this is a rights-managed resource.
        /// </summary>
        public bool Marked
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_Marked);
                return val.GetBool();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_Marked);
                val.SetBool(value);
            }
        }

        /// <summary>
        /// Gets an unordered array specifying the legal owner(s)of a resource.
        /// </summary>
        public XmpArray Owner
        {
            get
            {
                XmpArray val = GetArray(c_Owner, XmpArrayType.Bag);
                return val;
            }
        }

        /// <summary>
        /// Gets text instructions on how a resource can be legally used.
        /// </summary>
        public XmpLangArray UsageTerms
        {
            get
            {
                XmpLangArray val = GetLangArray(c_UsageTerms);
                return val;
            }
        }

        /// <summary>
        /// Gets or sets the location of a web page describing the owner
        /// and/or rights statement for this resource.
        /// </summary>
        public Uri WebStatement
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_WebStatement);
                return val.GetUri();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("WebStatement");

                XmpSimpleType val = GetSimpleProperty(c_WebStatement);
                val.SetUri(value);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates schema object.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal RightsManagementSchema(XmpMetadata xmp)
            : base(xmp)
        {
        }
        #endregion
    }
}
