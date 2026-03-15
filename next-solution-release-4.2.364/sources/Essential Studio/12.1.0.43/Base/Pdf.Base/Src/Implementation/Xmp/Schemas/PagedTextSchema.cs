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
    /// Represents Paged Text Schema.
    /// </summary>
    public class PagedTextSchema : XmpSchema
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "xmpTPg";
        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http://ns.adobe.com/xap/1.0/t/pg/";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_NPages = "NPages";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_Fonts = "Fonts";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_PlateName = "PlateNames";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_Colorants = "Colorants";
        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_MaxPageSize = "MaxPageSize";
        #endregion

        #region Properties
        /// <summary>
        /// Gets type of the Schema.
        /// </summary>
        public override XmpSchemaType SchemaType
        {
            get
            {
                return XmpSchemaType.PagedTextSchema;
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
        /// Gets the size of the largest page in the document (including
        /// any in contained documents).
        /// </summary>
        public XmpDimensionsStruct MaxPageSize
        {
            get
            {
                XmpStructure val = GetStructure(c_MaxPageSize, XmpStructureType.Dimensions);
                return (val as XmpDimensionsStruct);
            }
        }
        /// <summary>
        /// Gets or sets the number of pages in the document (including any in
        /// contained documents).
        /// </summary>
        public int NPages
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_NPages);
                return val.GetInt();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_NPages);
                val.SetInt(value);
            }
        }
        /// <summary>
        /// Gets an unordered array of fonts that are used in the
        /// document (including any in contained documents).
        /// </summary>
        public XmpArray Fonts
        {
            get
            {
                XmpArray val = GetArray(c_Fonts, XmpArrayType.Bag);
                return val;
            }
        }
        /// <summary>
        /// Gets an unordered array of fonts that are used in the
        /// document (including any in contained documents).
        /// </summary>
        public XmpArray PlateNames
        {
            get
            {
                XmpArray val = GetArray(c_PlateName, XmpArrayType.Seq);
                return val;
            }
        }
        /// <summary>
        /// Gets an ordered array of colorants (swatches) that are used
        /// in the document (including any in contained
        /// documents).
        /// </summary>
        public XmpArray Colorants
        {
            get
            {
                XmpArray val = GetArray(c_Colorants, XmpArrayType.Seq);
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
        protected internal PagedTextSchema(XmpMetadata xmp)
            : base(xmp)
        {
        }
        #endregion
    }
}
