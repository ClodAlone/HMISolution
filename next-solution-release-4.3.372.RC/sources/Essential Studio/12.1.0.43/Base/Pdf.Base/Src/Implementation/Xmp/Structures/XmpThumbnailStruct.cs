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

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Xmp Thumbnail Structure.
    /// </summary>
    public class XmpThumbnailStruct : XmpStructure
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "xapG";

        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http://ns.adobe.com/xap/1.0/g/img/";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_height = "height";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_width = "width";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_format = "format";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_image = "image";
        #endregion

        #region Properties
        /// <summary>
        /// Gets prefix of the structure.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override string StructurePrefix
        {
            get
            {
                return c_prefix;
            }
        }

        /// <summary>
        /// Gets name pf the structure.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override string StructureURI
        {
            get
            {
                return c_name;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_height);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_height);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_width);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_width);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_format);
                return val.Value;
            }
            set
            {
                if (Format == null)
                    throw new ArgumentNullException("format");

                XmpSimpleType val = GetSimpleProperty(c_format);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the thumbnail image (pixel data only) converted to base 64
        /// notation.
        /// </summary>
        /// <value>The image.</value>
        public byte[] Image
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_image);
                return Convert.FromBase64String(val.Value);
            }
            set
            {
                if (Image == null)
                    throw new ArgumentNullException("Image");

                XmpSimpleType val = GetSimpleProperty(c_image);
                val.Value = (Convert.ToBase64String(value));
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates xmp simple type instance.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        /// <param name="parent">Parent xml node.</param>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="localName">Name of the tag.</param>
        /// <param name="namespaceURI">Namespace URI.</param>
        /// <param name="insideArray">Indicates whether structure is inside of the array.</param>
        internal XmpThumbnailStruct(XmpMetadata xmp, XmlNode parent, string prefix,
            string localName, string namespaceURI, bool insideArray)
            : base(xmp, parent, prefix, localName, namespaceURI, insideArray)
        {
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Initializes.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitializeEntities()
        {
        }
        #endregion
    }
}
