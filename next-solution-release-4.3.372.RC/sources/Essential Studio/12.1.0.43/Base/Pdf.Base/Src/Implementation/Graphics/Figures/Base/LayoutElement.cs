#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Text;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the base class for all elements that can be layout on the pages.
    /// </summary>
    /// [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public abstract class PdfLayoutElement : PdfGraphicsElement
    {
        #region Fields
        private bool m_bEmbedFonts;
        #endregion


        #region Events
        /// <summary>
        /// Event. Raises after the element was printed on the page.
        /// </summary>
        public event EndPageLayoutEventHandler EndPageLayout;

        /// <summary>
        /// Event. Raises before the element should be printed on the page.
        /// </summary>
        public event BeginPageLayoutEventHandler BeginPageLayout;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether EndPageLayout is not null.
        /// </summary>
        internal bool RaiseEndPageLayout
        {
            get
            {
                return (EndPageLayout != null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether BeginPageLayout is not null.
        /// </summary>
        internal bool RaiseBeginPageLayout
        {
            get
            {
                return (BeginPageLayout != null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [embed fonts].
        /// </summary>
        /// <value><c>true</c> if [embed fonts]; otherwise, <c>false</c>.</value>
        internal bool EmbedFontResource
        {
            get
            {
                return m_bEmbedFonts;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draws the element on the page.
        /// </summary>
        /// <param name="page">Current page where the element should be drawn.</param>
        /// <param name="location">Start location on the page.</param>
        /// <returns>Layouting result.</returns>
        public PdfLayoutResult Draw(PdfPage page, PointF location)
        {
            return Draw(page, location.X, location.Y);
        }

        /// <summary>
        /// Draws the element on the page.
        /// </summary>
        /// <param name="page">Current page where the element should be drawn.</param>
        /// <param name="x">X co-ordinate of the element on the page.</param>
        /// <param name="y">Y co-ordinate of the element on the page.</param>
        /// <returns>Lay outing result.</returns>
        public PdfLayoutResult Draw(PdfPage page, float x, float y)
        {
            return Draw(page, x, y, null);
        }

        /// <summary>
        /// Draws the element on the page.
        /// </summary>
        /// <param name="page">Current page where the element should be drawn.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the element.</param>
        /// <returns>Lay outing result.</returns>
        public PdfLayoutResult Draw(PdfPage page, RectangleF layoutRectangle)
        {
            return Draw(page, layoutRectangle, null);
        }

        /// <summary>
        /// Draws the element on the page.
        /// </summary>
        /// <param name="page">Current page where the element should be drawn.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the element.</param>
        /// <returns>Lay outing result.</returns>
        internal PdfLayoutResult Draw(PdfPage page, RectangleF layoutRectangle, bool embedFonts)
        {
            m_bEmbedFonts = embedFonts;
            return Draw(page, layoutRectangle, null);
        }

        /// <summary>
        /// Draws the element on the page.
        /// </summary>
        /// <param name="page">Current page where the element should be drawn.</param>
        /// <param name="location">Start location on the page.</param>
        /// <param name="format">Lay outing format.</param>
        /// <returns>Lay outing result.</returns>
        public PdfLayoutResult Draw(PdfPage page, PointF location, PdfLayoutFormat format)
        {
            return Draw(page, location.X, location.Y, format);
        }

        /// <summary>
        /// Draws the element on the page.
        /// </summary>
        /// <param name="page">Current page where the element should be drawn.</param>
        /// <param name="x">X co-ordinate of the element on the page.</param>
        /// <param name="y">Y co-ordinate of the element on the page.</param>
        /// <param name="format">Layout format.</param>
        /// <returns>Layout result.</returns>
        public PdfLayoutResult Draw(PdfPage page, float x, float y, PdfLayoutFormat format)
        {
            RectangleF layoutRectangle = new RectangleF(x, y, 0f, 0f);

            return Draw(page, layoutRectangle, format);
        }

        /// <summary>
        /// Draws the element on the page.
        /// </summary>
        /// <param name="page">Current page where the element should be drawn.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the element.</param>
        /// <param name="format">Layout format.</param>
        /// <returns>Layout result.</returns>
        public PdfLayoutResult Draw(PdfPage page, RectangleF layoutRectangle, PdfLayoutFormat format)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            PdfLayoutParams param = new PdfLayoutParams();

            param.Page = page;
            param.Bounds = layoutRectangle;
            param.Format = (format != null) ? format : new PdfLayoutFormat();

            PdfLayoutResult result = Layout(param);

            return result;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="Top">The top.</param>
        /// <param name="format">The format.</param>
        internal PdfLayoutResult Draw(PdfPage page, RectangleF bounds, float[] pageOffsets, PdfLayoutFormat format)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            HtmlToPdf.HtmlToPdfLayoutParams param = new HtmlToPdf.HtmlToPdfLayoutParams();
            param.VerticalOffsets = pageOffsets;
            param.Page = page;
            param.Bounds = bounds;
            param.Format = (format != null) ? format : new PdfLayoutFormat();

            PdfLayoutResult result = Layout(param);

            return result;
        }
#endif
        #endregion

        #region Implementation
        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Layout parameters.</param>
        /// <returns>Returns the results of layout.</returns>
        #if !NETFX_CORE && !WP
        [Syncfusion.Documentation.DocumentationExclude()]
        #endif
        protected abstract PdfLayoutResult Layout(PdfLayoutParams param);

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Layouts the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns>null</returns>
        protected virtual PdfLayoutResult Layout(HtmlToPdf.HtmlToPdfLayoutParams param)
        {
            return null;
        }
#endif
        #endregion

        #region Event handlers
        /// <summary>
        /// Raises EndPageLayout event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal void OnEndPageLayout(EndPageLayoutEventArgs e)
        {
            if (EndPageLayout != null)
            {
                EndPageLayout(this, e);
            }
        }

        /// <summary>
        /// Raises BeginPageLayout event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal void OnBeginPageLayout(BeginPageLayoutEventArgs e)
        {
            if (BeginPageLayout != null)
            {
                BeginPageLayout(this, e);
            }
        }
        #endregion
    }
}
