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

#if !SILVERLIGHT

#region file using directives
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO
{
    /// <summary>
    /// Summary description for DocIOXsdGenerator.
    /// </summary>
    public class DocIOXsdGenerator : XsdGenerator
    {
        #region Class constants
        /// <summary>
        /// constant string value.
        /// </summary>
        protected const string DEF_DOCIO_RESOURCES = "Syncfusion.DocIO.Resources";
        #endregion

        #region Class public methods
        /// <summary>
        /// Returns Schema of DocIO.
        /// </summary>
        /// <returns>Returns the Schema in XML format.</returns>
        public static XmlSchema GetDocIOLocalSchema()
        {
            Stream stream = GetDocIOResourceStream("docio-schema.xsd");
            return XmlSchema.Read(stream, new ValidationEventHandler(OnValidation));
        }

        /// <summary>
        /// Generates Schema for DocIO.
        /// </summary>
        /// <returns>Returns the Schema in XML format.</returns>
        public XmlSchema GenerateDocIOSchema()
        {
            Stream stream = GetDocIOResourceStream("docio-meta-schema.xml");
            XmlDocument dlsMetaSchema = LoadXmlDocument(stream);
            return GenerateSchema(dlsMetaSchema);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the doc IO resource stream.
        /// </summary>
        /// <param name="resName">Name of the res.</param>
        /// <returns></returns>
        protected static Stream GetDocIOResourceStream(string resName)
        {
            Assembly execAssm = Assembly.GetExecutingAssembly();
            return execAssm.GetManifestResourceStream(DEF_DOCIO_RESOURCES + "." + resName);
        }

        /// <summary>
        /// Returns Stream for a specified resource.
        /// </summary>
        /// <param name="resName">Specifies resource Name.</param>
        /// <param name="resNamespace">Specified Resource NameSpace.</param>
        /// <returns></returns>
        protected override Stream GetResourceStream(string resName, string resNamespace)
        {
            if (resNamespace == "Syncfusion.DocIO")
            {
                return GetDocIOResourceStream(resName);
            }

            return base.GetResourceStream(resName, resNamespace);
        }
        #endregion
    }
}

#endif