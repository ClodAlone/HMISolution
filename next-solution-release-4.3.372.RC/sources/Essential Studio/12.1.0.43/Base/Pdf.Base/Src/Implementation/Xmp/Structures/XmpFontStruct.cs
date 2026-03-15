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
    /// Font Structure.
    /// </summary>
    public class XmpFontStruct : XmpStructure
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "stFnt";

        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http:ns.adobe.com/xap/1.0/sType/Font#";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_fontName = "fontName";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_fontFamily = "fontFamily";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_fontFace = "fontFace";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_fontType = "fontType";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_versionString = "versionString";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_composite = "composite";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_fontFileName = "fontFileName";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_childFontFiles = "childFontFiles";
        #endregion

        #region Properties
        /// <summary>
        /// Gets prefix of the structure
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
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_fontName);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("fontName");

                XmpSimpleType val = GetSimpleProperty(c_fontName);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public string FontFamily
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_fontFamily);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("fontFamily");

                XmpSimpleType val = GetSimpleProperty(c_fontFamily);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the font face name.
        /// </summary>
        /// <value>The font face.</value>
        public string FontFace
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_fontFace);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("fontFace");

                XmpSimpleType val = GetSimpleProperty(c_fontFace);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the font.
        /// </summary>
        /// <value>The type of the font.</value>
        public string FontType
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_fontType);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("fontType");

                XmpSimpleType val = GetSimpleProperty(c_fontType);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the version string.
        /// </summary>
        /// <value>The version string.</value>
        public string VersionString
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_versionString);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("versionString");

                XmpSimpleType val = GetSimpleProperty(c_versionString);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="XmpFontStruct"/> is composite.
        /// </summary>
        /// <value><c>true</c> if composite; otherwise, <c>false</c>.</value>
        public bool Composite
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_composite);
                return val.GetBool();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_composite);
                val.SetBool(value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the font file.
        /// </summary>
        /// <value>The name of the font file.</value>
        public string FontFileName
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_fontFileName);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("fontFileName");

                XmpSimpleType val = GetSimpleProperty(c_fontFileName);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets the list of file names for the fonts that make up a
        /// composite font.
        /// </summary>
        public XmpArray ChildFontFiles
        {
            get
            {
                XmpArray val = GetArray(c_childFontFiles, XmpArrayType.Seq);
                return val;
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
        internal XmpFontStruct(XmpMetadata xmp, XmlNode parent, string prefix,
            string localName, string namespaceURI, bool insideArray)
            : base(xmp, parent, prefix, localName, namespaceURI, insideArray)
        {
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Initializes structure.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitializeEntities()
        {
        }
        #endregion
    }
}
