#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Windows.Markup;
using System.Linq;
using System.Xml;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    public class XAMLImporting:DependencyObject
    {
        #region Constructor
        /// <summary>
        /// Create instance for the XAML Importing.
        /// </summary>
        public XAMLImporting()
        {
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Returns the DocumentAdv from the XamlStream
        /// </summary>
        /// <param name="xamlStream"></param>
        /// <returns></returns>
        public static DocumentAdv ConvertToDocumentAdv(Stream xamlStream)
        {
            DocumentAdv richDocument = null;
#if WPF
            richDocument = XamlReader.Load(xamlStream) as DocumentAdv;
#else
            using (StreamReader reader = new StreamReader(xamlStream))
            {
                string xaml = reader.ReadToEnd();
                richDocument = XamlReader.Load(xaml) as DocumentAdv;
            }
#endif
            if (richDocument == null)
                throw new Exception("The input Xaml content is not in RichTextBoxAdv’s Document model. Kindly use the Xaml content created by RichTextBoxAdv control.");
            return richDocument;
        }

        /// <summary>
        /// Returns the DocumentAdv from the Xaml string.
        /// </summary>
        /// <param name="xamlstring"></param>
        /// <returns></returns>
        public static DocumentAdv ConvertToDocumentAdv(string xamlstring)
        {
            DocumentAdv document = null;
#if WPF
            XamlReader xamlreader = new XamlReader();
            StringReader stringreader = new StringReader(xamlstring);
            XmlReader xmlreader = XmlReader.Create(stringreader);
            document = XamlReader.Load(xmlreader) as DocumentAdv;
#else
            document = XamlReader.Load(xamlstring) as DocumentAdv;
#endif
            if (document == null)
                throw new Exception("The input Xaml content is not in RichTextBoxAdv’s Document model. Kindly use the Xaml content created by RichTextBoxAdv control.");
            return document;
        }

        #endregion
    }
}
