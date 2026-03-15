//-------------------------------------------------------------------------------------------------
// <copyright file="GridMetaFileConverter.cs" company="Syncfusion">
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
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Grid.Grouping;

    /// <summary>
    /// Uses GridPrintDocument class to create Metafiles from Grid
    /// </summary>
    [ToolboxItem(false)]
    class GridMetaFileConverter : GridPrintDocument, IDisposable
    {
        GridControlBase grid;
        List<Metafile> list = new List<Metafile>();
        int page = 0;
        bool morePages = true;

        /// <summary>
        /// Gets the Metafiles of the GridImages
        /// </summary>
        public List<Metafile> Metafiles
        {
            get 
            { 
                return this.list; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are MorePages to convert to Metafiles
        /// </summary>
        public bool MorePages
        {
            get 
            { 
                return this.morePages; 
            }
        }

        /// <summary>
        /// Initializes new MeteFileGridPrintDocument
        /// </summary>
        /// <param name="grid">The grid control base</param>
        /// <param name="imgSize">The image size</param>
        public GridMetaFileConverter(GridControlBase grid, Size imgSize)
            : base(grid)
        {
            this.PrinterSettings.PrintToFile = true;
            this.grid = grid;
            if (grid is GridTableControl && !(grid as GridTableControl).GroupingControl.TopLevelGroupOptions.ShowCaption)
                this.IsPDFExport = false;
            else
                this.IsPDFExport = true;
            this.CustomBounds = new Rectangle(Point.Empty, imgSize);
        }

        /// <summary>
        /// Calls the base class's BeginPrint
        /// </summary>
        /// <param name="ev">The PrintEventArgs</param>
        protected void DoBeginPrint(System.Drawing.Printing.PrintEventArgs ev)
        {
            Trace.WriteLine("BeginPrint..");
            base.OnBeginPrint(ev);
        }

        /// <summary>
        /// Calls the base class's EndPrint
        /// </summary>
        /// <param name="e">The PrintEventArgs</param>
        protected void DoEndPrint(System.Drawing.Printing.PrintEventArgs e)
        {
            Trace.WriteLine("EndPrint..");
            base.OnEndPrint(e);
            this.page = 0;
        }

        /// <summary>
        /// Does a page wise conversion of Grid to Metafiles
        /// </summary>
        protected void DoPrintPage()
        {
            Metafile mf = this.ConvertGridPageToMetafile(this.grid);
            this.list.Add(mf);

            this.grid.PrintInfo.m_nCurrentPageColIndex++;
            this.morePages = true;
            if (this.grid.PrintInfo.m_nCurrentPageColIndex >= this.grid.PrintInfo.m_awPageFirstCol.Count - 1)
            {
                this.grid.PrintInfo.m_nCurrentPageColIndex = 0;
                this.grid.PrintInfo.m_nCurrentPageRowIndex++;
                if (this.grid.PrintInfo.m_nCurrentPageRowIndex >= this.grid.PrintInfo.m_awPageFirstRow.Count - 1)
                {
                    this.morePages = false;
                }
            }

            this.page++;
        }

        /// <summary>
        /// Gets the converted MetaFiles
        /// </summary>
        /// <returns>List of type Metafiles</returns>
        public List<Metafile> GetMetafiles()
        {
            PrintEventArgs args = new PrintEventArgs();
            this.DoBeginPrint(args);

            // Convert pages to Metafile using Grid Print library
            while (this.MorePages)
            {
                this.DoPrintPage();
            }

            this.DoEndPrint(args);

            return this.Metafiles;
        }

        /// <summary>
        /// Creates Metafile
        /// </summary>
        /// <param name="grid">The grid control base</param>
        /// <returns>The Metafile</returns>
        protected Metafile ConvertGridPageToMetafile(GridControlBase grid)
        {
            System.Drawing.Imaging.Metafile mf;

            try
            {
                Graphics gr = Graphics.FromHwndInternal(IntPtr.Zero);
                IntPtr hdc = gr.GetHdc();

                // Create a metafile in MemoryStream
                mf = new System.Drawing.Imaging.Metafile(hdc, this.CustomBounds, MetafileFrameUnit.Pixel, EmfType.EmfOnly);

                // Make a Graphics object to work with the metafile.
                Graphics g = Graphics.FromImage(mf);

                this.grid.Model.Properties.CenterHorizontal = false;
                this.grid.Model.Properties.PrintFrame = false;

                PrintPageEventArgs pageev = new PrintPageEventArgs(g, this.CustomBounds, this.CustomBounds, this.DefaultPageSettings);

                base.OnPrintPage(pageev);

                // Dispose and release
                g.Dispose();
                gr.ReleaseHdc(hdc);
                gr.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }

            return mf;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.list = null;
        }

        #endregion
    }
}
