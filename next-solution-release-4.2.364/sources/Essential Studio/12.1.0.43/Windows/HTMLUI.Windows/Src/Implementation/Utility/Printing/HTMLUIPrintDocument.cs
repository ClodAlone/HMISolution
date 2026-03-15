#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that represents printing support in the HTMLUIControl.
    /// </summary>
    [ToolboxItem(false)]
    public class HTMLUIPrintDocument : PrintDocument
    {
        #region Class constants
        /// <summary>
        /// Measure factor.
        /// </summary>
        private const float DEF_MEASURE_FACTOR = 100f;
        #endregion

        #region Class members
        /// <summary>
        /// Document object to be printed.
        /// </summary>
        private InputHTML m_document;

        /// <summary>
        /// Start point of the document before printing.
        /// </summary>
        private Rectangle m_oldMargins = Rectangle.Empty;

        /// <summary>
        /// Size of the document before printing.
        /// </summary>
        private Size m_oldDocSize;

        /// <summary>
        /// Scroll position of the document before printing.
        /// </summary>
        private Point m_oldDocScrollPos;

        /// <summary>
        /// Index of the currently printing page.
        /// </summary>
        private int m_curPage;

        /// <summary>
        /// Indicates the current copy of the page.
        /// </summary>
        private int m_curCopy;

        /// <summary>
        /// Indicates the number of pages already printed.
        /// </summary>
        private int m_numPrinted;

        /// <summary>
        /// Indicates whether the first page is printed.
        /// </summary>
        private bool m_bFirstPage;

        /// <summary>
        /// Current scroll position.
        /// </summary>
        private Point m_nextScrollPosition;

        /// <summary>
        /// Contains original graphics of the document.
        /// </summary>
        private Graphics m_oldGraphics;

        /// <summary>
        /// Array of pages.
        /// </summary>
        private TextRegion[] m_pages;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the HTMLUIPrintDocument class from being created
        /// </summary>
        private HTMLUIPrintDocument()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HTMLUIPrintDocument class
        /// </summary>
        /// <param name="document">Document object to be printed.</param>
        public HTMLUIPrintDocument(IInputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (document.Root == null)
                throw new ArgumentException("Document does not contain 'Root' element.", "document");

            m_document = document as InputHTML;
        }

        /// <summary>
        /// Disposes the document.
        /// </summary>
        /// <param name="disposing">True to dispose all resources, False - to unmanaged resources only.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            m_oldGraphics = null;
            m_document = null;
            m_pages = null;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Overridden. Raised before the document prints.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);

            if (GetFrom() > GetTo())
            {
                e.Cancel = true;
            }
            else
            {
                SaveBounds();

                m_curPage = 0;
                m_numPrinted = 0;
                m_curCopy = 0;
                m_bFirstPage = true;
                m_document.TextRegionManager = new TextRegionManager();
                m_nextScrollPosition = Point.Empty;

                SetFirstPage();

                // Enable quiet mode.
                m_document.Root.Control.BeginUpdate();
                m_document.IsPrinting = true;
            }
        }

        /// <summary>
        /// Overridden. Raised when printing is finished.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnEndPrint(PrintEventArgs e)
        {
            base.OnEndPrint(e);

            // Restore default graphics.
            BaseElement.SelectGraphics(m_oldGraphics);
            m_oldGraphics = null;

            RestoreBounds();

            m_document.IsPrinting = false;
            m_pages = null;

            m_document.Root.Control.SelectionManager.ResetCalculation();
            m_document.Recalculate();

            // Disable quiet mode.
            m_document.Root.Control.EndUpdate();
        }

        /// <summary>
        /// Overridden. Prints the page on the printer.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            Size size = GetNonPrintableSize(e.Graphics);

            // Recalculate document at first page.
            if (m_bFirstPage)
            {
                // Set graphics from printer to render the document.
                m_oldGraphics = BaseElement.SelectGraphics(e.Graphics);
                PrepareDocument();
                RenderPages(e);

                m_bFirstPage = false;
                m_document.TextRegionManager.Clear();
                m_document.TextRegionManager = null;
            }

            GraphicsState oldState = e.Graphics.Save();

            try
            {
                // Check if page is in printable pages region.
                if (IsPagePrintable() && m_curPage < m_pages.Length)
                {
                    TextRegion pageRegion = m_pages[m_curPage];
                    int pageY = pageRegion.Y;
                    int pageHeight = pageRegion.Height;
                    m_document.AutoScrollPosition = new Point(m_document.AutoScrollPosition.X, pageY);

                    Rectangle rect = e.MarginBounds;
                    rect.Height = pageHeight;
                    e.Graphics.SetClip(rect, CombineMode.Replace);

                    PaintEventArgs args = new PaintEventArgs(e.Graphics, rect);
                    m_document.Draw(args, m_document.AutoScrollPosition);

                    CalculateCopies();
                    m_numPrinted++;
                }
            }
            finally
            {
                e.Graphics.Restore(oldState);
                CheckForMorePages(e);

                if (e.HasMorePages)
                {
                    SetNextPage();
                }
				GC.WaitForPendingFinalizers();
				GC.Collect();
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Prepares the document for printing. 
        /// </summary>
        private void PrepareDocument()
        {
            // Calculate document's size and location.
            Size docSize = this.DefaultPageSettings.Bounds.Size;

            System.Drawing.Printing.Margins margins = this.DefaultPageSettings.Margins;

            // Set new size and location fo the document.
            m_document.Margins.Left = margins.Left;
            m_document.Margins.Top = margins.Top;
            m_document.Margins.Right = margins.Right;
            m_document.Margins.Bottom = margins.Bottom;

            m_document.ClientSize = docSize;
            m_document.AutoScrollMinSize = docSize;
            m_document.AutoScrollPosition = m_nextScrollPosition;
            m_document.Root.Control.SelectionManager.ResetCalculation();
            m_document.Recalculate();
        }

        /// <summary>
        /// Checks if there are any pages to be printed next.
        /// </summary>
        /// <param name="e">Printing data.</param>
        private void CheckForMorePages(PrintPageEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            int fromPage = GetFrom();
            int toPage = GetTo();

            int numDefinedPages = toPage - fromPage + 1;
            int numOfCopies = numDefinedPages * this.PrinterSettings.Copies;

            e.HasMorePages = m_numPrinted < numOfCopies;
        }

        /// <summary>
        /// Sets the index of next printed page.
        /// </summary>
        private void SetNextPage()
        {
            if (this.PrinterSettings.Collate && m_curPage >= GetTo())
            {
                SetFirstPage();
            }
            else if (this.PrinterSettings.Collate ||
              (!this.PrinterSettings.Collate &&
              m_curCopy == this.PrinterSettings.Copies) ||
              !IsPagePrintable())
            {
                m_curPage++;
                m_curCopy = 0;
            }
        }

        /// <summary>
        /// Indicates whether the current page must be printed.
        /// </summary>
        /// <returns>True if page is printable; False otherwise.</returns>
        private bool IsPagePrintable()
        {
            return m_curPage >= GetFrom() && m_curPage <= GetTo();
        }

        /// <summary>
        /// Returns the index of the first printing page.
        /// </summary>
        /// <returns>Index of the first printing page.</returns>
        private int GetFrom()
        {
            int fromPage = 0;

            if (this.PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                fromPage = this.PrinterSettings.FromPage;
            }

            return fromPage;
        }

        /// <summary>
        /// Returns the index of the last printing page if it's set by user.
        /// </summary>
        /// <returns>Index of the last printing page.</returns>
        private int GetTo()
        {
            int toPage = 0;

            if (m_pages == null)
            {
                int numDocPages = GetPagesInDocument();
                toPage = numDocPages - 1;
            }
            else
            {
                toPage = m_pages.Length - 1;
            }

            if (this.PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                toPage = Math.Min(toPage, this.PrinterSettings.ToPage);
            }

            return toPage;
        }

        /// <summary>
        /// Calculates the number of pages in the document.
        /// </summary>
        /// <returns>Count of pages in the document.</returns>
        private int GetPagesInDocument()
        {
            int docHeight = m_document.AutoScrollMinSize.Height;
            int pageHeight = GetPageMarginBound().Height;
            int numDocPages = (int)Math.Ceiling((float)docHeight / (float)pageHeight);

            return numDocPages;
        }

        /// <summary>
        /// Calculates the number of copies of the pages printed.
        /// </summary>
        private void CalculateCopies()
        {
            if (!this.PrinterSettings.Collate && IsPagePrintable())
            {
                m_curCopy++;
            }
        }

        /// <summary>
        /// Sets the index of the first printed page.
        /// </summary>
        private void SetFirstPage()
        {
            m_curPage = GetFrom();
        }

        /// <summary>
        /// Calculates the printable area of the page.
        /// </summary>
        /// <returns>Printable area of the page.</returns>
        private Rectangle GetPageMarginBound()
        {
            Rectangle bound = this.DefaultPageSettings.Bounds;
            System.Drawing.Printing.Margins margins = this.DefaultPageSettings.Margins;

            bound.X += margins.Left;
            bound.Y += margins.Top;

            bound.Width -= margins.Left + margins.Right;
            bound.Height -= margins.Top + margins.Bottom;

            return bound;
        }

        /// <summary>
        /// Returns the nonprintable area for the printer.
        /// </summary>
        /// <param name="g">Device context.</param>
        /// <returns>Nonprintable area for the printer.</returns>
        private Size GetNonPrintableSize(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            IntPtr hDC = g.GetHdc();

            int left = NativeMethods.GetDeviceCaps(hDC, NativeMethods.PHYSICALOFFSETX);
            int top = NativeMethods.GetDeviceCaps(hDC, NativeMethods.PHYSICALOFFSETY);

            g.ReleaseHdc(hDC);

            return new Size(left, top);
        }

        /// <summary>
        /// Stores the current bounds of the document.
        /// </summary>
        private void SaveBounds()
        {
            // Save document settings.
            m_oldMargins.X = m_document.Margins.Left;
            m_oldMargins.Y = m_document.Margins.Top;
            m_oldMargins.Width = m_document.Margins.Right;
            m_oldMargins.Height = m_document.Margins.Bottom;

            m_oldDocSize = m_document.ClientSize;
            m_oldDocSize.Width += m_document.Margins.Left + m_document.Margins.Right;
            m_oldDocSize.Height += m_document.Margins.Top + m_document.Margins.Bottom;
            m_oldDocScrollPos = m_document.AutoScrollPosition;
        }

        /// <summary>
        /// Restores the bounds of the document.
        /// </summary>
        private void RestoreBounds()
        {
            // Restore cached document's size and location.
            m_document.Margins.Left = m_oldMargins.X;
            m_document.Margins.Top = m_oldMargins.Y;
            m_document.Margins.Right = m_oldMargins.Width;
            m_document.Margins.Bottom = m_oldMargins.Height;

            m_document.ClientSize = m_oldDocSize;

            m_document.AutoScrollMinSize = m_oldDocSize;
            m_document.AutoScrollPosition = m_oldDocScrollPos;
        }

        /// <summary>
        /// Calculates current scroll position.
        /// </summary>
        /// <param name="marginBounds">Page bounds.</param>
        /// <returns>The height of the page.</returns>
        private int CalculateScrollPosition(Rectangle marginBounds)
        {
            int potentialY = -m_nextScrollPosition.Y;
            potentialY += marginBounds.Height;
            potentialY += marginBounds.Y;

            TextRegion rgn = m_document.TextRegionManager.GetTopCoordinate(potentialY);
            potentialY = rgn.Y - marginBounds.Y;

            int height = potentialY + m_nextScrollPosition.Y;
            height = (height == 0) ? marginBounds.Height : height;

            // To prevent drawing at the same scroll position twice draw at the end of the region.
            int tmpY = potentialY;
            tmpY += marginBounds.Height;
            tmpY += marginBounds.Y;
            TextRegion testRgn = m_document.TextRegionManager.GetTopCoordinate(tmpY);
            tmpY = testRgn.Y - marginBounds.Y;
            if (tmpY == -m_nextScrollPosition.Y)
            {
                potentialY += (int)testRgn.Height;
            }

            m_nextScrollPosition.Y = -potentialY;

            return height;
        }

        /// <summary>
        /// Checks whether there are any unprinted pages.
        /// </summary>
        /// <returns>True - if there are any unprinted pages, False otherwise.</returns>
        private bool HasMorePages()
        {
            int height = m_document.AutoScrollMinSize.Height;
            int y = -m_nextScrollPosition.Y;
            bool hasPages = y < height;

            return hasPages;
        }

        /// <summary>
        /// Renders the pages into the array of bitmaps.
        /// </summary>
        /// <param name="e">Page print arguments.</param>
        private void RenderPages(PrintPageEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            ArrayList pages = new ArrayList();
            Rectangle margins = e.MarginBounds;
            int width = margins.Right;
            int height = margins.Bottom;

            while (HasMorePages())
            {
                using (Bitmap tmp = new Bitmap(width, height))
                {
                    using (Graphics tmpGr = Graphics.FromImage(tmp))
                    {
                        tmpGr.SetClip(margins, CombineMode.Replace);
                        using (PaintEventArgs pArgs = new PaintEventArgs(tmpGr, margins))
                        {
                            m_document.AutoScrollPosition = m_nextScrollPosition;
                            m_document.Draw(pArgs, m_document.AutoScrollPosition);
                            //m_document.TextRegionManager.Clear();
                            m_document.Root.Control.SelectionManager.ResetCalculation();

                            int pageHeight = CalculateScrollPosition(e.MarginBounds);
                            TextRegion region = new TextRegion(m_document.AutoScrollPosition.Y, pageHeight);
                            pages.Add(region);
                        }
                    }
                }
            }

            m_pages = (TextRegion[])pages.ToArray(typeof(TextRegion));

            m_nextScrollPosition = Point.Empty;
            m_document.AutoScrollPosition = Point.Empty;
        }
        #endregion
    }
}
