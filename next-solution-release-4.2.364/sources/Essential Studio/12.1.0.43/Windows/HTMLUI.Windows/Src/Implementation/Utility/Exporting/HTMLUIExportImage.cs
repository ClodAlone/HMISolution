#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
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
    /// Class that represents exporting support in the HTMLUIControl.
    /// </summary>
    [ToolboxItem(false)]
    public class HTMLUIExportImage
    {
        #region Class static members
        /// <summary>
        /// Size of the document before printing.
        /// </summary>
        private static Size m_oldDocSize;

        /// <summary>
        /// Start point of the document before printing.
        /// </summary>
        private static Rectangle m_oldMargins = Rectangle.Empty;

        /// <summary>
        /// Scroll position of the document before printing.
        /// </summary>
        private static Point m_oldDocScrollPos;

        /// <summary>
        /// Indicates number of pages already exported.
        /// </summary>
        private static int m_numExported;

        /// <summary>
        /// Indicates whether first page is printed.
        /// </summary>
        private static bool m_bFirstImage;
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates an array of images where the document will be printed.
        /// </summary>
        /// <param name="document">Document object to be exported.</param>
        /// <param name="imageSize">Size of the images where the document will be printed.</param>
        /// <returns>Image array</returns>
        public static Image[] GetExportedImages(IInputHTML document, Size imageSize)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (document.Root == null)
                throw new ArgumentException("Document does not contain 'Root' element.", "document");

            InputHTML documentEx = document as InputHTML;

            // NOTE: lock following snippet of code for preventing data damaging.
            lock (typeof(HTMLUIExportImage))
            {
                SaveBounds(documentEx);

                m_numExported = 0;
                m_bFirstImage = true;

                //// Enable quiet mode.
                documentEx.Root.Control.BeginUpdate();
                documentEx.IsPrinting = true;

                Image[] exportedImages = DrawToImages(documentEx, imageSize);

                //// Restore cached document's size and location.
                RestoreBounds(documentEx);

                documentEx.IsPrinting = false;

                document.Root.Control.SelectionManager.ResetCalculation();
                document.Recalculate();

                //// Disable quiet mode.
                document.Root.Control.EndUpdate();

                //// Return the images
                return exportedImages;
            }
        }

        /// <summary>
        /// Prints document to image.
        /// </summary>
        /// <param name="document">Document containing the data to be printed to images.</param>
        /// <param name="img">Image where document will be printed.</param>
        /// <param name="imageSize">Size of the image.</param>
        private static void SaveImage(InputHTML document, Image img, Size imageSize)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (img == null)
                throw new ArgumentNullException("img");

            Graphics g = Graphics.FromImage(img);

            // Recalculate document at first page.
            // Set graphics from printer to render the document.
            if (m_bFirstImage)
            {
                Graphics oldGraphics = BaseElement.SelectGraphics(g);
                PrepareDocument(document, imageSize);
                BaseElement.SelectGraphics(oldGraphics);
                m_bFirstImage = false;
            }

            // Set clip rectangle.
            Rectangle rect = CalculateClipRegion(document, imageSize);
            GraphicsState oldState = g.Save();
            g.SetClip(rect);

            PaintEventArgs args = new PaintEventArgs(g, rect);
            document.Draw(args, document.AutoScrollPosition);

            m_numExported++;
            g.Restore(oldState);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Prepares document for exporting. 
        /// </summary>
        /// <param name="document">Document containing the data to be printed to images.</param>
        /// <param name="imageSize">Size of the image.</param>
        private static void PrepareDocument(InputHTML document, Size imageSize)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            //// Calculate document's size and location.
            Point docLocation = new Point(0, 0); //// Change if actual margins needed

            document.Margins.Left = 0;
            document.Margins.Top = 0;
            document.Margins.Right = 0;
            document.Margins.Bottom = 0;

            //// Set new size and location fo the document.
            document.ClientSize = imageSize;
            document.AutoScrollMinSize = imageSize;
            document.AutoScrollPosition = Point.Empty;
            document.Root.Control.SelectionManager.ResetCalculation();
            document.Recalculate();
        }

        /// <summary>
        /// Calculates the clip rectangle for the document in the current page.
        /// </summary>
        /// <param name="document">Document containing the data to be printed to images.</param>
        /// <param name="imageSize">Size of the image.</param>
        /// <returns>Clip rectangle for the current image.</returns>
        private static Rectangle CalculateClipRegion(InputHTML document, Size imageSize)
        {
            // Calculate Y offset for printing current page.
            Point docScrollPos = document.AutoScrollPosition;
            int yOffset = m_numExported * imageSize.Height;
            docScrollPos.Y = -yOffset;

            document.AutoScrollPosition = docScrollPos;
            Rectangle rect = new Rectangle(Point.Empty, imageSize);

            return rect;
        }

        /// <summary>
        /// Calculates the number of images in the document.
        /// </summary>
        /// <param name="document">Document containing the data to be printed to images.</param>
        /// <param name="imageSize">Size of the image.</param>
        /// <returns>Count of images in the document.</returns>
        private static int GetImagesCount(InputHTML document, Size imageSize)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            int docHeight = document.AutoScrollMinSize.Height;
            int numDocImages = (int)Math.Ceiling((float)docHeight /
        (float)imageSize.Height);

            return numDocImages;
        }

        /// <summary>
        /// Calculates the printable area of the image.
        /// </summary>
        /// <param name="imageSize">Size of the image.</param>
        /// <returns>Printable area of the image.</returns>
        private static Rectangle GetEachImageBound(Size imageSize)
        {
            Rectangle bound = new Rectangle(Point.Empty, imageSize);

            return bound;
        }

        /// <summary>
        /// Stores the current bounds of the document.
        /// </summary>
        /// <param name="document">Document containing the data to be printed to images.</param>
        private static void SaveBounds(InputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            // Save document settings.
            m_oldMargins.X = document.Margins.Left;
            m_oldMargins.Y = document.Margins.Top;
            m_oldMargins.Width = document.Margins.Right;
            m_oldMargins.Height = document.Margins.Bottom;

            m_oldDocSize = document.ClientSize;
            m_oldDocSize.Width += document.Margins.Left + document.Margins.Right;
            m_oldDocSize.Height += document.Margins.Top + document.Margins.Bottom;
            m_oldDocScrollPos = document.AutoScrollPosition;
        }

        /// <summary>
        /// Restores the bounds of the document.
        /// </summary>
        /// <param name="document">Document containing the data to be printed to images.</param>
        private static void RestoreBounds(InputHTML document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            // Restore cached document's size and location.
            document.Margins.Left = m_oldMargins.X;
            document.Margins.Top = m_oldMargins.Y;
            document.Margins.Right = m_oldMargins.Width;
            document.Margins.Bottom = m_oldMargins.Height;

            document.ClientSize = m_oldDocSize;

            document.AutoScrollMinSize = m_oldDocSize;
            document.AutoScrollPosition = m_oldDocScrollPos;
        }

        /// <summary>
        /// Method that supports printing the document to Images
        /// </summary>
        /// <param name="document">Document containing the data to be printed to images.</param>
        /// <param name="imageSize">Size of the images.</param>
        /// <returns>Array of images containing the document.</returns>
        private static Image[] DrawToImages(InputHTML document, Size imageSize)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            int imagesCount = GetImagesCount(document, imageSize);
            Image[] exportedImages = new Image[imagesCount];

            // Exporting data.
            for (int i = 0; i < imagesCount; i++)
            {
                exportedImages[i] = new Bitmap(imageSize.Width, imageSize.Height);

                SaveImage(document, exportedImages[i], imageSize);
            }

            return exportedImages;
        }
        #endregion
    }
}
