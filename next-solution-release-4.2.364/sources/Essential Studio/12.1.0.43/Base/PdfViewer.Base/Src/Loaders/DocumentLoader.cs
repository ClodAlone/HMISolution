#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Pdf.Parsing;
using System.IO;

namespace Syncfusion.PdfViewer.Base
{
    sealed class DocumentLoader
    {
        static DocumentLoader s_instance;
        static readonly object s_lock = new object();

        public static DocumentLoader Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (s_lock)
                    {
                        s_instance = new DocumentLoader();
                    }
                }
                return s_instance;
            }
        }

        public PdfLoadedDocument Load(string filePath)
        {
            return new PdfLoadedDocument(filePath);
        }
        public PdfLoadedDocument Load(string filePath, string password)
        {
            return new PdfLoadedDocument(filePath, password);
        }

        public PdfLoadedDocument Load(Stream stream)
        {
            return new PdfLoadedDocument(stream);
        }
    }
}
