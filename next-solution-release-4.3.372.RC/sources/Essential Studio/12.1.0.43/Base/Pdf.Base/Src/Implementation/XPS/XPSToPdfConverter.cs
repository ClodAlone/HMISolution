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
using System.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.IO;

namespace Syncfusion.XPS
{
    /// <summary>
    /// Represents the XPS to PDF converter.
    /// </summary>
    public class XPSToPdfConverter
    {
        #region Members
        /// <summary>
        /// Represents the PdfUnitConverter
        /// </summary>
        private PdfUnitConvertor m_unitConvertor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XPSToPdfConverter"/> class.
        /// </summary>
        public XPSToPdfConverter()
        {
            m_unitConvertor = new PdfUnitConvertor();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts the XPS document in the disk to PDF.
        /// </summary>
        /// <param name="fileName">Path to the XPS document</param>
        /// <returns>PdfDocument</returns>
        public PdfDocument Convert(string fileName)
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Margins.All = 0;

            using (XPSDocumentReader reader = new XPSDocumentReader(fileName))
            {
                reader.Read();

                foreach (FixedPage xpsPage in reader.Pages)
                {
                    PdfSection section = document.Sections.Add();
                    section.PageSettings.Size = new SizeF(PixelsToPoints(xpsPage.Width), PixelsToPoints(xpsPage.Height));
                    if (xpsPage.Width > xpsPage.Height)
                        section.PageSettings.Orientation = PdfPageOrientation.Landscape;

                    PdfPage page = section.Pages.Add();

                    using (XPSRenderer renderer = new XPSRenderer(page, reader))
                    {
                        using (XPSParser parser = new XPSParser(xpsPage, renderer))
                        {
                            parser.Enumerate();
                        }
                    }
                }
            }

            return document;
        }

        /// <summary>
        /// Converts the XPS document in stream to PDF.
        /// </summary>
        /// <param name="file">XPS document</param>
        /// <returns>PdfDocument</returns>
        public PdfDocument Convert(Stream file)
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Margins.All = 0;

            using (XPSDocumentReader reader = new XPSDocumentReader(file))
            {
                reader.Read();

                foreach (FixedPage xpsPage in reader.Pages)
                {
                    PdfSection section = document.Sections.Add();
                    section.PageSettings.Size = new SizeF(PixelsToPoints(xpsPage.Width), PixelsToPoints(xpsPage.Height));
                    if (xpsPage.Width > xpsPage.Height)
                        section.PageSettings.Orientation = PdfPageOrientation.Landscape;

                    PdfPage page = section.Pages.Add();

                    using (XPSRenderer renderer = new XPSRenderer(page, reader))
                    {
                        using (XPSParser parser = new XPSParser(xpsPage, renderer))
                        {
                            parser.Enumerate();
                        }
                    }
                }
            }

            return document;
        }

        /// <summary>
        /// Converts the byte array with XPS file content to PDF
        /// </summary>
        /// <param name="file">XPS document</param>
        /// <returns>PdfDocument</returns>
        public PdfDocument Convert(byte[] file)
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Margins.All = 0;

            Stream xpsStream = new MemoryStream(file);
            using (XPSDocumentReader reader = new XPSDocumentReader(xpsStream))
            {
                reader.Read();

                foreach (FixedPage xpsPage in reader.Pages)
                {
                    PdfSection section = document.Sections.Add();
                    section.PageSettings.Size = new SizeF(PixelsToPoints(xpsPage.Width), PixelsToPoints(xpsPage.Height));
                    if (xpsPage.Width > xpsPage.Height)
                        section.PageSettings.Orientation = PdfPageOrientation.Landscape;

                    PdfPage page = section.Pages.Add();

                    using (XPSRenderer renderer = new XPSRenderer(page, reader))
                    {
                        using (XPSParser parser = new XPSParser(xpsPage, renderer))
                        {
                            parser.Enumerate();
                        }
                    }
                }
            }

            return document;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Converts the Pixel to Point value.
        /// </summary>
        /// <param name="value">value in pixel</param>
        /// <returns>value in points</returns>
        private float PixelsToPoints(double value)
        {
            return m_unitConvertor.ConvertFromPixels((float)value, PdfGraphicsUnit.Point);
        }
        #endregion
    }
}
