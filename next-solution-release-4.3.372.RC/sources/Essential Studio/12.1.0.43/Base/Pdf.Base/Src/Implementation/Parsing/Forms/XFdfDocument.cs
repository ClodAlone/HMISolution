#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents base class of XFDF.
    /// </summary>
    public class XFdfDocument
    {
#region Fields
        Dictionary<object, object> table = new Dictionary<object, object>();
        string PdfFilePath = "";
        #endregion

#region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="XFdfDocument"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public XFdfDocument(string filename)
        {
            PdfFilePath = filename;
        }
        #endregion

#region Methods
        internal void SetFields(object fieldName, object Fieldvalue)
        {
            table.Add(fieldName, Fieldvalue);
        }

        internal void Save(Stream stream)
        {
            XmlTextWriter textWriter = new XmlTextWriter(stream, new UTF8Encoding());
            textWriter.Formatting = Formatting.Indented;
            textWriter.WriteStartDocument();
            textWriter.WriteStartElement(DictionaryProperties.XFdf.ToLower());
            textWriter.WriteAttributeString("xmlns", null, null, "http://ns.adobe.com/xfdf/");
            // Write the xml:space attribute.
            textWriter.WriteAttributeString("xml", "space", null, "preserve");

            // Writting Fields
            textWriter.WriteStartElement(DictionaryProperties.Fields.ToLower());

            foreach (KeyValuePair<object,object> entry in table)
            {
                textWriter.WriteStartElement(DictionaryProperties.Field);
                textWriter.WriteAttributeString(DictionaryProperties.Name.ToLower(), entry.Key.ToString());

                if (entry.Value.GetType().Name == "PdfArray")
                {
                    PdfArray array = entry.Value as PdfArray;

                    foreach (PdfString str1 in array)
                    {
                        textWriter.WriteStartElement(DictionaryProperties.Value);
                        textWriter.WriteString(str1.Value.ToString());
                        textWriter.WriteEndElement();
                    }
                }
                else
                {
                    textWriter.WriteStartElement(DictionaryProperties.Value); // قفز الثعلب ا
                    textWriter.WriteString(entry.Value.ToString());
                    textWriter.WriteEndElement();
                }
                textWriter.WriteEndElement();
            }


            textWriter.WriteEndElement(); // Fields Element

            //#region Field
            //textWriter.WriteStartElement(DictionaryProperties.Ids);

            //byte[]  m_randomBytes = new byte[ 16 ];

            //for( byte i = 0; i < 16; i++ )
            //{
            //    m_randomBytes[ i ] = i;
            //}

            //PdfString str = new PdfString(m_randomBytes);
            //textWriter.WriteAttributeString("original", PdfString.BytesToHex(m_randomBytes));
            //textWriter.WriteAttributeString("modified", PdfString.BytesToHex(m_randomBytes));
            //textWriter.WriteEndElement();
            //#endregion

            textWriter.WriteStartElement("f");
            textWriter.WriteAttributeString("href", PdfFilePath);
            textWriter.WriteEndElement();


            textWriter.WriteEndElement(); // xfdf Element
            textWriter.WriteEndDocument();
            textWriter.Flush();
        }
        #endregion
    }
}
#endif