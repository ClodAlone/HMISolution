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
    /// coloring Structure.
    /// </summary>
    public class XmpColorantStruct : XmpStructure
    {
        #region Constants
        /// <summary>
        /// Prefix of the schema.
        /// </summary>
        private const string c_prefix = "xapG";

        /// <summary>
        /// Nasme of the schema.
        /// </summary>
        private const string c_name = "http://ns.adobe.com/xap/1.0/g/";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_swatchName = "swatchName";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_mode = "mode";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_type = "type";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_cyan = "cyan";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_magenta = "magenta";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_black = "black";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_red = "red";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_green = "green";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_blue = "blue";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_L = "L";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_A = "A";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_B = "B";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_yellow = "yellow";
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
        /// Gets or sets yellow value when the mode is CMYK. Range 0-100.
        /// </summary>
        public float Yellow
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_yellow);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_yellow);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets B value when the mode is LAB. Range -128 to 127.
        /// </summary>
        public float B
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_B);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_B);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets A value when the mode is LAB. Range -128 to 127.
        /// </summary>
        public float A
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_A);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_A);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets L value when the mode is LAB. Range 0-100.
        /// </summary>
        public float L
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_L);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_L);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets blue value when the mode is RGB. Range 0-255.
        /// </summary>
        public float Blue
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_blue);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_blue);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets green value when the mode is RGB. Range 0-255.
        /// </summary>
        public float Green
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_green);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_green);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets red value when the mode is RGB. Range 0-255.
        /// </summary>
        public float Red
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_red);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_red);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets black value when the mode is CMYK. Range 0-100.
        /// </summary>
        public float Black
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_black);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_black);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets magenta value when the mode is CMYK. Range 0-100.
        /// </summary>
        public float Magenta
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_magenta);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_magenta);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets Cyan value when the mode is CMYK. Range 0-100.
        /// </summary>
        public float Cyan
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_cyan);
                return val.GetReal();
            }
            set
            {
                XmpSimpleType val = GetSimpleProperty(c_cyan);
                val.SetReal(value);
            }
        }

        /// <summary>
        /// Gets or sets the type of color, one of PROCESS or SPOT.
        /// </summary>
        public string Type
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_type);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("type");

                XmpSimpleType val = GetSimpleProperty(c_type);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the color space in which the color is defined. One of:
        /// CMYK, RGB, LAB. Library colors are represented in
        /// the color space for which they are defined.
        /// </summary>
        public string Mode
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_mode);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("mode");

                XmpSimpleType val = GetSimpleProperty(c_mode);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the swatch.
        /// </summary>
        public string SwatchName
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_swatchName);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("swatchName");

                XmpSimpleType val = GetSimpleProperty(c_swatchName);
                val.Value = value;
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
        internal XmpColorantStruct(XmpMetadata xmp, XmlNode parent, string prefix,
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
