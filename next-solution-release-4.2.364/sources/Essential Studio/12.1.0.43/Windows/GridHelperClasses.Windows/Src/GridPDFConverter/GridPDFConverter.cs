//-------------------------------------------------------------------------------------------------
// <copyright file="GridPDFConverter.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System.Collections.Generic;
    using System.Text;
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;    
    using System.Drawing.Imaging;
    using System.Drawing.Printing;
    using System.IO;
    using System.Diagnostics;
    using Syncfusion.Pdf;
    using Syncfusion.Pdf.Graphics;
    using Syncfusion.Windows.Forms.Grid;
    using System.Runtime.InteropServices;
    using Syncfusion.ComponentModel;

    /// <summary>
    /// GridPDFConverter class exports Grid to PDF.
    /// </summary>
    /// <remarks>It has support for Header, Footer and Margins. It uses Grid's printing library to generate Metafile images and generates PDF file</remarks>
    public class GridPDFConverter
    {
        /// <summary>
        /// Initializes a new GridPDFConverter
        /// </summary>
        public GridPDFConverter()
        {
        }

        /// <summary>
        /// Initializes a new GridPDFConverter
        /// </summary>
        /// <param name="showHeader">True if Header should be shown; Default is false</param>
        /// <param name="showFooter">True if Foorter should be shown; Default is false</param>
        public GridPDFConverter(bool showHeader, bool showFooter)
        {
            this.showHeader = showHeader;
            this.showFooter = showFooter;
        }

        int headerHeight = 0;
        int footerHeight = 0;
        bool showHeader = false;
        bool showFooter = false;
        PdfMargins margins = new PdfMargins();
        
        /// <summary>
        /// Represents the method that handles <see cref="Exporting"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PDFExportingEventArgs"/> that contains the event data.</param>
        public delegate void PDFExportingEventHandler(object sender, PDFExportingEventArgs e);
        
        /// <summary>
        /// Occurs while exporting to PDF
        /// </summary>
        public event PDFExportingEventHandler Exporting;
        
        /// <summary>
        /// Raises PDFExporting event.
        /// </summary>
        /// <param name="e">Event data</param>
        protected virtual void OnPDFExporting(PDFExportingEventArgs e)
        {
            if (this.Exporting != null)
            {
                this.Exporting(this, e);
            }
        }

        /// <summary>
        /// Represents the method that handles <see cref="Exported"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PDFExportingEventArgs"/> that contains the event data.</param>
        public delegate void PDFExportedEventHandler(object sender, PDFExportedEventArgs e);

        /// <summary>
        /// Occurs after exporting to PDF
        /// </summary>
        public event PDFExportedEventHandler Exported;

        /// <summary>
        /// Raises PDFExported event.
        /// </summary>
        /// <param name="e">Event data</param>
        protected virtual void OnPDFExported(PDFExportedEventArgs e)
        {
            if (this.Exported != null)
            {
                this.Exported(this, e);
            }
        }

        /// <summary>
        /// Represents the method that handles <see cref="DrawPDFHeader"/> and <see cref="DrawPDFFooter"/> events.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PDFHeaderFooterEventArgs"/> that contains the event data.</param>
        public delegate void DrawPDFHeaderFooterEventHandler(object sender, PDFHeaderFooterEventArgs e);

        /// <summary>
        /// Lets the user draw a header for the PDF Docment
        /// </summary>
        public event DrawPDFHeaderFooterEventHandler DrawPDFHeader;

        /// <summary>
        /// Lets the user draw a Footer for the PDF Docment
        /// </summary>
        public event DrawPDFHeaderFooterEventHandler DrawPDFFooter;

        /// <summary>
        /// Gets or sets the Margins for the PDF document
        /// </summary>
        public PdfMargins Margins
        {
            get
            {
                return this.margins;
            }

            set
            {
                this.margins = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a Header should be shown. True if Header should be shown; Default is false
        /// </summary>
        /// <remarks>GridPDFConverter lets the user to draw the required Header. Handle the <see cref="DrawPDFHeader"/> event. Height is set with <see cref="HeaderHeight"/></remarks>
        public bool ShowHeader
        {
            get
            {
                return this.showHeader;
            }

            set
            {
                this.showHeader = value;

                // Set default value for HeaderHeight
                if (this.showHeader && this.headerHeight == 0)
                {
                    this.headerHeight = 50;
                }
                else
                {
                    this.headerHeight = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a Footer should be shown. True if Header should be shown; Default is false
        /// </summary>
        /// <remarks>GridPDFConverter lets the user to draw the required Footer. Handle the <see cref="DrawPDFFooter"/> event. Height is set with <see cref="FooterHeight"/></remarks>
        public bool ShowFooter
        {
            get
            {
                return this.showFooter;
            }

            set
            {
                this.showFooter = value;

                // Set default value for HeaderHeight
                if (this.showFooter && this.footerHeight == 0)
                {
                    this.footerHeight = 50;
                }
                else
                {
                    this.footerHeight = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the Header
        /// </summary>
        public int HeaderHeight
        {
            get
            {
                return this.headerHeight;
            }

            set
            {
                this.headerHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the Footer
        /// </summary>
        public int FooterHeight
        {
            get
            {
                return this.footerHeight;
            }

            set
            {
                this.footerHeight = value;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridPDFConverter.DrawPDFHeader"/> event.
        /// </summary>
        /// <param name="e">The <see cref="OnDrawPDFHeader"/> that contains the event data.</param>
        protected virtual void OnDrawPDFHeader(PDFHeaderFooterEventArgs e)
        {
            if (this.DrawPDFHeader != null)
            {
                this.DrawPDFHeader(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridPDFConverter.DrawPDFFooter"/> event.
        /// </summary>
        /// <param name="e">A <see cref="OnDrawPDFFooter"/> that contains the event data</param>
        protected virtual void OnDrawPDFFooter(PDFHeaderFooterEventArgs e)
        {
            if (this.DrawPDFFooter != null)
            {
                this.DrawPDFFooter(this, e);
            }
        }

        /// <summary>
        /// Exports the Grid contents to PDF file
        /// </summary>
        /// <param name="filename">File name to write the generated PDF</param>
        /// <param name="grid">Grid instance to export</param>
        public void ExportToPdf(string filename, GridControlBase grid)
        {
            ExportToPdf(filename, grid, GridRangeInfo.Empty);
        }
       /// <summary>
        /// Exports the contents of the cells in a range to PDF file
       /// </summary>
        /// <param name="filename">File name to write the generated PDF</param>
        /// <param name="grid">Grid instance</param>
       /// <param name="exportRange">The range to be exported</param>
        /// <remarks>If exportRange is GridRangeInfo.Empty, the entire grid is exported.</remarks>
        public void ExportToPdf(string filename, GridControlBase grid, GridRangeInfo exportRange)
        {
            if (filename.Length == 0)
            {
                throw new Exception("Specify a valid file name to export");
            }

            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }

            // Create a new PDFDocument and add a section
            PdfDocument pdfDocument = new PdfDocument();

            // Export
            this.ExportToPdf(pdfDocument, grid, exportRange);

            // Save
            pdfDocument.Save(filename);

            // Dispose PDFDocument
            pdfDocument.Close(true);
        }

        /// <summary>
        /// Exports the Grid contents to a PDF Document
        /// </summary>
        /// <param name="pdfDocument">Destination PDF Document</param>
        /// <param name="grid">Source Grid to Export</param>
        public void ExportToPdf(PdfDocument pdfDocument, GridControlBase grid)
        {
            ExportToPdf(pdfDocument, grid, GridRangeInfo.Empty);
        }

        /// <summary>
        /// Exports grid with huge data in to two or more PDF documents internally and Merges 
        /// them into single document(pdfDocument). This method can be used to handle "out of memory exception"
        /// that happens when huge amount of data (more than 60k records) is exported to PDF.
        /// </summary>
        /// <param name="pdfDocument"></param>
        /// <param name="grid"></param>
        public void ExportToPdfWithMerge(ref PdfDocument pdfDocument, GridControlBase grid)
        {            
            int rowCount = grid.Model.RowCount;

            if (rowCount <= this.exportRange)
            {
                ExportToPdf(pdfDocument, grid);
                return;
            }

            int lowerRange = 1;
            int upperRange = this.exportRange;

            int pdfCount = 0;

            string[] docs = null;

            while (rowCount > 0)
            {
                PdfDocument pdf = new PdfDocument();                
                pdfCount++;
                this.ExportToPdf(pdf, grid, GridRangeInfo.Rows(lowerRange, upperRange));

                // Save
                pdf.Save(string.Format("Temp{0}.pdf",pdfCount));

                // Dispose PDFDocument
                pdf.Close(true);

                lowerRange = upperRange + 1;
                upperRange = rowCount - this.exportRange > this.exportRange ? upperRange + this.exportRange : upperRange + rowCount - this.exportRange;
                rowCount -= this.exportRange;
            }

            docs = new string[pdfCount];

            for (int i = 0; i < pdfCount; i++)
            {
                docs[i] = string.Format("Temp{0}.pdf", i+1);
            }
            pdfDocument = PdfDocument.Merge(docs);
        }

        private int exportRange = 60000;

        /// <summary>
        /// Determines the export range of records when ExportToPdfWithMerge is used
        /// Note:
        /// the method ExportToPdfWithMerge is used to handle "out of memory exception" which happens when
        /// the record count exceeds 60k records hence this value determines the no of records that can be exported
        /// in each single pdf file. This value can't be set more than 60k 
        /// </summary>
        public int ExportRange
        {
            get { return this.exportRange; }
            set
            {
                if (value > 60000)
                    this.exportRange = 60000;
                else
                    this.exportRange = value;
            }
        }

        /// <summary>
        /// Exports grid with huge data in to two or more PDF documents internally and Merges 
        /// them into single document(filename). This method can be used to handle out of memory exception
        /// that happens when huge amount of data (more than 60k records) is exported to PDF.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="grid"></param>
        public void ExportToPdfWithMerge(string filename, GridControlBase grid)
        {
            if (filename.Length == 0)
            {
                throw new Exception("Specify a valid file name to export");
            }

            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }

            // Create a new PDFDocument and add a section
            PdfDocument pdfDocument = new PdfDocument();

            // Export
            this.ExportToPdfWithMerge(ref pdfDocument, grid);
            
            // Save
            pdfDocument.Save(filename);
            
            // Dispose PDFDocument
            pdfDocument.Close(true);
            
        }
        

        /// <summary>
        /// Exports the contents of the cells in a range to PDF file
        /// </summary>
        /// <param name="pdfDocument">Destination PDF Document</param>
        /// <param name="grid">Source Grid to Export</param>
        /// <param name="exportRange">The range to be exported</param>
        /// <remarks>If exportRange is GridRangeInfo.Empty, the entire grid is exported.</remarks>
        public void ExportToPdf(PdfDocument pdfDocument, GridControlBase grid, GridRangeInfo exportRange)
        {
            if (pdfDocument == null)
            {
                throw new ArgumentNullException("pdfDocument");
            }

            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }

            PDFExportingEventArgs pe = new PDFExportingEventArgs(pdfDocument);
            this.OnPDFExporting(pe);
            if (!pe.Cancel)
            {
                try
                {
                    if (!exportRange.IsEmpty)
                    {
                        LockWindowUpdate(grid.Handle);
                        grid.Model.QueryRowHeight += new GridRowColSizeEventHandler(Model_QueryRowHeight);
                        grid.Model.QueryColWidth += new GridRowColSizeEventHandler(Model_QueryColWidth);

                        exportCellRange = exportRange.ExpandRange(grid.Model.Rows.HeaderCount + 1, grid.Model.Cols.HeaderCount + 1, grid.Model.RowCount, grid.Model.ColCount);
                    }
                    PdfSection section = pe.PdfDocument.Sections.Add();

                    section.PageSettings.Margins = this.margins;

                    // Add a page so that we could get PageSize to generate Metafiles
                    section.Pages.Add();

                    // DrawHeaderFooter
                    this.DrawHeaderFooter(pe.PdfDocument, this.showHeader, this.showFooter);

                    // Convert Page size from Points to Pixels
                    SizeF pageSize = section.Pages[0].GetClientSize();
                    Size imgSize = this.PointToPixelConverter(pageSize);

                    List<Metafile> gridPages = this.DrawGridToMetafiles(grid, imgSize);

                    // First page is already added
                    for (int page = 1; page < gridPages.Count; page++)
                    {
                        section.Pages.Add();
                    }
                    this.ConvertMetafilesToPdf(gridPages, section);
                    gridPages = null;

                    PDFExportedEventArgs pd = new PDFExportedEventArgs(pe.PdfDocument);
                    this.OnPDFExported(pd);
                }
                finally
                {
                    if (!exportRange.IsEmpty)
                    {
                        grid.Model.QueryRowHeight -= new GridRowColSizeEventHandler(Model_QueryRowHeight);
                        grid.Model.QueryColWidth -= new GridRowColSizeEventHandler(Model_QueryColWidth);
                        LockWindowUpdate(IntPtr.Zero);
                        grid.Refresh();
                    }

                }
            }
        }

        private GridRangeInfo exportCellRange = GridRangeInfo.Empty;
         void Model_QueryColWidth(object sender, GridRowColSizeEventArgs e)
        {
            GridModel model = sender as GridModel;
            if(e.Index > model.Cols.HeaderCount && (e.Index < exportCellRange.Left || e.Index > exportCellRange.Right))
            {
                e.Size = 0;
                e.Handled = true;
            }
        }

        void Model_QueryRowHeight(object sender, GridRowColSizeEventArgs e)
        {
            GridModel model = sender as GridModel;
            if(e.Index > model.Rows.HeaderCount && (e.Index < exportCellRange.Top || e.Index > exportCellRange.Bottom))
            {
                e.Size = 0;
                e.Handled = true;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool LockWindowUpdate(IntPtr hWndLock);
 

        /// <summary>
        /// Inserts the provided Metafiles to the PDF section
        /// </summary>
        /// <param name="metaFiles">list containing Grid images</param>
        /// <param name="section">PDF section to insert</param>
        protected void ConvertMetafilesToPdf(List<Metafile> metaFiles, PdfSection section)
        {
            PdfMetafileLayoutFormat format = new PdfMetafileLayoutFormat();
            format.Break = PdfLayoutBreakType.FitPage;
            format.Layout = PdfLayoutType.Paginate;
            format.SplitTextLines = false;

            for (int imgIndex = 0; imgIndex < metaFiles.Count; imgIndex++)
            {
                PdfPage page = section.Pages[imgIndex];
                PdfGraphics gg = page.Graphics;
                PdfMetafile metafile = new PdfMetafile(metaFiles[imgIndex]);

                metafile.Draw(page, new RectangleF(0, 0, page.GetClientSize().Width, metafile.Height), format);
            }
        }

        /// <summary>
        /// Generated Metafiles from the Grid instance
        /// </summary>
        /// <param name="grid">Grid instance</param>
        /// <param name="imgSize">Size of the image to create</param>
        /// <returns>List of Metafile type</returns>
        protected List<Metafile> DrawGridToMetafiles(GridControlBase grid, Size imgSize)
        {
            GridMetaFileConverter pd = new GridMetaFileConverter(grid, imgSize);
            pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            return pd.GetMetafiles();
        }

        /// <summary>
        /// Draws the Header and Footer.
        /// </summary>
        /// <param name="pdfDocument">The PdfDocument</param>
        /// <param name="top">bool to draw header</param>
        /// <param name="bottom">bool to draw footer</param>
        protected void DrawHeaderFooter(PdfDocument pdfDocument, bool top, bool bottom)
        {
            float width = pdfDocument.Pages[0].GetClientSize().Width;

            if (top && this.headerHeight > 0)
            {
                RectangleF headerRect = new RectangleF(0, 0, width, this.headerHeight);

                // Create page template
                PdfPageTemplateElement headerTemplate = new PdfPageTemplateElement(headerRect);

                // Raise DrawHeader
                this.OnDrawPDFHeader(new PDFHeaderFooterEventArgs(headerTemplate));

                // Add header template at the top.
                pdfDocument.Template.Top = headerTemplate;
            }

            if (bottom && this.footerHeight > 0)
            {
                RectangleF footerRect = new RectangleF(0, 0, width, this.footerHeight);

                // Create a page template
                PdfPageTemplateElement footerTemplate = new PdfPageTemplateElement(footerRect);

                // Riase DrawFooter
                this.OnDrawPDFFooter(new PDFHeaderFooterEventArgs(footerTemplate));

                // Add the footer template at the bottom
                pdfDocument.Template.Bottom = footerTemplate;
            }
        }

        /// <summary>
        /// Point to pixel converter
        /// </summary>
        /// <param name="size">The size of point to pixel converter</param>
        /// <returns>The point to pixel converter</returns>
        protected Size PointToPixelConverter(SizeF size)
        {
            PdfUnitConvertor convertor = new PdfUnitConvertor();
            float width = convertor.ConvertToPixels(size.Width, PdfGraphicsUnit.Point);
            float height = convertor.ConvertToPixels(size.Height, PdfGraphicsUnit.Point);

            return new Size((int)width, (int)height);
        }
    }

    /// <summary>
    /// Provides data for the <see cref="PDFHeaderFooterEventArgs"/> and <see cref="PDFHeaderFooterEventArgs"/> events
    /// </summary>
    /// <remarks>To draw the Header / Footer for the created PDF, handle the <see cref="PDFHeaderFooterEventArgs"/> / <see cref="PDFHeaderFooterEventArgs"/> events
    /// </remarks>
    public class PDFHeaderFooterEventArgs : EventArgs
    {
        PdfPageTemplateElement headerFooterTemplate;

        /// <summary>
        /// Gets or sets the PDF header and footer.
        /// </summary>
        public PdfPageTemplateElement HeaderFooterTemplate
        {
            get
            {
                return this.headerFooterTemplate;
            }

            set
            {
                this.headerFooterTemplate = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="PDFHeaderFooterEventArgs"/>
        /// </summary>
        /// <param name="HeaderFooterTemplate">Specifies the header and footer for the PDF document.</param>
        public PDFHeaderFooterEventArgs(PdfPageTemplateElement HeaderFooterTemplate)
        {
            this.headerFooterTemplate = HeaderFooterTemplate;
        }
    }


    /// <summary>
    /// Provides data for the <see cref="PDFExportingEventArgs"/>event
    /// </summary>
    public class PDFExportingEventArgs : SyncfusionCancelEventArgs
    {
        private PdfDocument document;

        /// <summary>
        /// Initializes a new <see cref="PDFExportingEventArgs"/>
        /// </summary>
        /// <param name="document">PDF document being export</param>
        public PDFExportingEventArgs(PdfDocument document)
        {
            this.document = document;
        }

        /// <summary>
        /// Gets or sets the current PdfDocument to be exported
        /// </summary>
        public PdfDocument PdfDocument
        {
            get
            {
                return document;
            }
            set
            {
                if (document != value)
                    document = value;
            }
        }
    }

    /// <summary>
    /// Provides data for the <see cref="PDFExportedEventArgs"/>event
    /// </summary>
    public class PDFExportedEventArgs : SyncfusionEventArgs
    {
        private PdfDocument document;

        /// <summary>
        /// Initializes a new <see cref="PDFExportedEventArgs"/>
        /// </summary>
        /// <param name="document">PDF document exported</param>
        public PDFExportedEventArgs(PdfDocument document)
        {
            this.document = document;
        }

        /// <summary>
        /// Gets the current PdfDocument to be exported
        /// </summary>
        public PdfDocument PdfDocument
        {
            get
            {
                return document;
            }
        }
    }

    

}

