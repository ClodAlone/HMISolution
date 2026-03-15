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
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;

namespace Syncfusion.Windows.Tools.Controls
{
    public class HTMLImporting
    {
        #region Constructors

        public HTMLImporting()
        {            
        }
        
        #endregion

        #region Implementation
        
        /// <summary>
        /// Convert the Boxes from the RootBox to the DocumentAdv
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static DocumentAdv ConvertToDocumentAdv(Stream fileStream)
        {
            DocumentAdv richTextDocument = null;
            fileStream.Seek(0, SeekOrigin.Begin);
            using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
            {
                RootNode rootcontainer = new RootNode(reader.ReadToEnd());
                richTextDocument = rootcontainer.AssignTo(rootcontainer);
            }
            return richTextDocument;                
        }

        public static DocumentAdv ConvertToDocumentAdv(string htmlstring)
        {
            DocumentAdv document = null;
            RootNode root = new RootNode(htmlstring);
            document = root.AssignTo(root);
            return document;
        }
        
        #endregion
    }
}
