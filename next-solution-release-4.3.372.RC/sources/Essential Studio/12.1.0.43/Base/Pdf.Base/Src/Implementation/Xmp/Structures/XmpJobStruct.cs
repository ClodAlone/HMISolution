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
    /// Declares Job structure.
    /// </summary>
    public class XmpJobStruct : XmpStructure
    {
        #region Constants
        /// <summary>
        /// Prefix of the structure.
        /// </summary>
        private const string c_prefix = "stJob";

        /// <summary>
        /// Name of the structure.
        /// </summary>
        private const string c_structName = "http://ns.adobe.com/xap/1.0/sType/Job#";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_name = "name";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_id = "id";

        /// <summary>
        /// Name of the property.
        /// </summary>
        private const string c_url = "url";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets informal name of job. This name is for user display and
        /// informal systems.
        /// </summary>
        public string Name
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_name);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Name");

                XmpSimpleType val = GetSimpleProperty(c_name);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets unique ID for the job. This field is a reference into some
        /// external job management system..
        /// </summary>
        public string ID
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_id);
                return val.Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("ID");

                XmpSimpleType val = GetSimpleProperty(c_id);
                val.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a file URL referencing an external job management file.
        /// </summary>
        public Uri Url
        {
            get
            {
                XmpSimpleType val = GetSimpleProperty(c_url);
                return val.GetUri();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Url");

                XmpSimpleType val = GetSimpleProperty(c_url);
                val.SetUri(value);
            }
        }

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
                return c_structName;
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
        internal XmpJobStruct(XmpMetadata xmp, XmlNode parent, string prefix,
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
