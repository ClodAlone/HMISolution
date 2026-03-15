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

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Represents Basic Job Ticket Schema.
    /// </summary>
    public class BasicJobTicketSchema : XmpSchema
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "xmpBJ";
        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http://ns.adobe.com/xap/1.0/bj/";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_propJobRef = "JobRef";
        #endregion

        #region Properties
        /// <summary>
        /// Gets type of the Schema.
        /// </summary>
        public override XmpSchemaType SchemaType
        {
            get
            {
                return XmpSchemaType.BasicJobTicketSchema;
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

        #region Schema properties
        /// <summary>
        /// Gets references an external job management file for a job
        /// process in which the document is being used. Use of job
        /// names is under user control. Typical use would be to
        /// identify all documents that are part of a particular job or
        /// contract.
        /// </summary>
        public XmpArray JobRef
        {
            get
            {
                XmpArray val = GetArray(c_propJobRef, XmpArrayType.Bag);
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
        protected internal BasicJobTicketSchema(XmpMetadata xmp)
            : base(xmp)
        {
        }
        #endregion
    }
}
