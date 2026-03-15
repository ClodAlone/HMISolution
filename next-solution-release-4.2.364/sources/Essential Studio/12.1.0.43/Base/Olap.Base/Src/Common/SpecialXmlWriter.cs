//-------------------------------------------------------------------------------------------------
// <copyright file="SpecialXmlWriter.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.IO;
using System.Text;
using System.Xml;

namespace Syncfusion.Olap.Common
{
    /// <summary>
    /// This class can be used to implement special affects while producing xml documents.
    /// At the moment it is only used for excluding the xml start line(<?xml version="1.0" encoding="utf-8" ?> )
    /// </summary>
    public class SpecialXmlWriter : XmlTextWriter
    {
        bool m_includeStartDocument = true;
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialXmlWriter"/> class.
        /// </summary>
        /// <param name="tw">The TextWriter object.</param>
        /// <param name="includeStartDocument">if set to <c>true</c> [include start document].</param>
        public SpecialXmlWriter(TextWriter tw, bool includeStartDocument)
            : base(tw)
        {
            m_includeStartDocument = includeStartDocument;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialXmlWriter"/> class.
        /// </summary>
        /// <param name="sw">The StreamWriter object.</param>
        /// <param name="encoding">The encoding object.</param>
        /// <param name="includeStartDocument">if set to <c>true</c> [include start document].</param>
        public SpecialXmlWriter(Stream sw, Encoding encoding, bool includeStartDocument)
            : base(sw, null)
        {            
            m_includeStartDocument = includeStartDocument;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialXmlWriter"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding object.</param>
        /// <param name="includeStartDocument">if set to <c>true</c> [include start document].</param>
        public SpecialXmlWriter(string filePath, Encoding encoding, bool includeStartDocument)
            : base(filePath, null)
        {
            m_includeStartDocument = includeStartDocument;
        }

        /// <summary>
        /// Writes the XML declaration with the version "1.0".
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
        public override void WriteStartDocument()
        {
            if (m_includeStartDocument)
            {
                base.WriteStartDocument();
            }
        }
    }
}
