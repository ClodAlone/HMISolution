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
using System.Windows.Input;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;
#if WPF
using System.Windows.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class XAMLImporting : DependencyObject
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
            try
            {
#if WPF
                richDocument = (DocumentAdv)XamlReader.Load(xamlStream);
#else
                using (StreamReader reader = new StreamReader(xamlStream))
                {
                    string xaml = reader.ReadToEnd();
                    richDocument = (DocumentAdv)XamlReader.Load(xaml);
                }
#endif
            }
            catch(Exception)
            {
                throw new System.InvalidOperationException("UnSupported File");
            }
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
            try
            {

#if !WPF
                document = (DocumentAdv)XamlReader.Load(xamlstring);
#endif
#if WPF
                XamlReader xamlreader = new XamlReader();
                StringReader stringreader = new StringReader(xamlstring);
                XmlReader xmlreader = XmlReader.Create(stringreader);
                document = (DocumentAdv)XamlReader.Load(xmlreader);
#endif
            }
            catch (Exception)
            {           
                throw new System.InvalidOperationException("UnSupported File");
            }
            return document;
        }

        #endregion
    }
}
