#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Base class for elements lay outing.
    /// </summary>
    internal abstract class ElementLayouter
    {
        #region Fields
        /// <summary>
        /// Layout the element.
        /// </summary>
        private PdfLayoutElement m_element;
        protected bool m_isImagePath;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementLayouter"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public ElementLayouter(PdfLayoutElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            m_element = element;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets  element`s layout.
        /// </summary>
        public PdfLayoutElement Element
        {
            get
            {
                return m_element;
            }
        }
        internal bool IsImagePath
        {
            get
            {
                return m_isImagePath;
            }
            set
            {
                m_isImagePath = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        public PdfLayoutResult Layout(PdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            return LayoutInternal(param);
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Layouts the HtmlToPdf element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        public PdfLayoutResult Layout(HtmlToPdf.HtmlToPdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            return LayoutInternal(param);
        }
#endif

        /// <summary>
        /// Returns the next page.
        /// </summary>
        /// <param name="currentPage">Current page.</param>
        /// <returns>The next page.</returns>
        /// <remarks>The next page is taken from the same section the current one was.
        /// If there is not enough pages within the section, the new one is appended.</remarks>
        public PdfPage GetNextPage(PdfPage currentPage)
        {
            if (currentPage == null)
            {
                throw new ArgumentNullException("currentPage");
            }

            PdfSection section = currentPage.Section;
            PdfPage nextPage = null;
            int index = section.IndexOf(currentPage);

            if (index == section.Count - 1)
            {
                nextPage = section.Add();
            }
            else
            {
                nextPage = section[index + 1];
            }

            return nextPage;
        }

        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        protected abstract PdfLayoutResult LayoutInternal(PdfLayoutParams param);

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        protected virtual PdfLayoutResult LayoutInternal(HtmlToPdf.HtmlToPdfLayoutParams param)
        {
            return null;
        }
#endif

        /// <summary>
        /// Gets paginate bounds.
        /// </summary>
        /// <param name="param">Layout parameters.</param>
        /// <returns>Gets paginate bounds.</returns>
        protected RectangleF GetPaginateBounds(PdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            RectangleF result = (param.Format.UsePaginateBounds) ?
                param.Format.PaginateBounds :
                new RectangleF(param.Bounds.X, 0, param.Bounds.Width, param.Bounds.Height);

            return result;
        }
        #endregion
    }

    /// <summary>
    /// Class that defines layouting settings.
    /// </summary>
    public class PdfLayoutFormat
    {
        #region Fields
        /// <summary>
        /// Indicates whether PaginateBounds were set and should be used or not.
        /// </summary>
        private bool m_boundsSet;

        /// <summary>
        /// Bounds for the paginating.
        /// </summary>
        private RectangleF m_paginateBounds;

        /// <summary>
        /// Layout type of the element.
        /// </summary>
        private PdfLayoutType m_layout;

        /// <summary>
        /// Break type of the element.
        /// </summary>
        private PdfLayoutBreakType m_break;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets layout type of the element.
        /// </summary>
        public PdfLayoutType Layout
        {
            get
            {
                return m_layout;
            }

            set
            {
                m_layout = value;
            }
        }

        /// <summary>
        /// Gets or sets break type of the element.
        /// </summary>
        public PdfLayoutBreakType Break
        {
            get
            {
                return m_break;
            }

            set
            {
                m_break = value;
            }
        }

        /// <summary>
        /// Gets or sets the bounds on the next page.
        /// </summary>
        /// <remarks>If this property is set, the element will use it for the layouting on the next pages,
        /// otherwise, the element will be layout according to the bounds, used on the first page.</remarks>
        public RectangleF PaginateBounds
        {
            get
            {
                return m_paginateBounds;
            }

            set
            {
                m_paginateBounds = value;
                m_boundsSet = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [use paginate bounds].
        /// </summary>
        /// <value><c>true</c> if [use paginate bounds]; otherwise, <c>false</c>.</value>
        internal bool UsePaginateBounds
        {
            get
            {
                return m_boundsSet;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLayoutFormat"/> class.
        /// </summary>
        public PdfLayoutFormat()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLayoutFormat"/> class.
        /// </summary>
        /// <param name="baseFormat">The base format.</param>
        public PdfLayoutFormat(PdfLayoutFormat baseFormat)
            : this()
        {
            if (baseFormat == null)
            {
                throw new ArgumentNullException("baseFormat");
            }

            Break = baseFormat.Break;
            Layout = baseFormat.Layout;
            PaginateBounds = baseFormat.PaginateBounds;
            m_boundsSet = baseFormat.UsePaginateBounds;
        }
        #endregion
    }

    /// <summary>
    /// Represents the layouting result settings.
    /// </summary>
    public class PdfLayoutResult
    {
        #region Fields
        /// <summary>
        /// The last page where the element was drawn.
        /// </summary>
        private PdfPage m_page;

        /// <summary>
        /// The bounds of the element on the last page where it was drawn.
        /// </summary>
        private RectangleF m_bounds;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the last page where the element was drawn.
        /// </summary>
        public PdfPage Page
        {
            get
            {
                return m_page;
            }
        }

        /// <summary>
        /// Gets the bounds of the element on the last page where it was drawn.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="page">The current page.</param>
        /// <param name="bounds">The current bounds.</param>
        /// <remarks>The page might be null, which means that
        /// lay outing was performed on PdfGraphics.</remarks>
        internal PdfLayoutResult(PdfPage page, RectangleF bounds)
        {
            m_page = page;
            m_bounds = bounds;
        }
        #endregion
    }

    /// <summary>
    /// Represents the layouting parameters.
    /// </summary>
    #if !NETFX_CORE  && !WP
    [Syncfusion.Documentation.DocumentationExclude()] 
    #endif
    public class PdfLayoutParams
    {
        #region Fields
        /// <summary>
        /// Start lay outing page.
        /// </summary>
        private PdfPage m_page;

        /// <summary>
        /// Lay outing bounds.
        /// </summary>
        private RectangleF m_bounds;

        /// <summary>
        /// Layout settings.
        /// </summary>
        private PdfLayoutFormat m_format;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets start layouting page.
        /// </summary>
        public PdfPage Page
        {
            get
            {
                return m_page;
            }

            set
            {
                m_page = value;
            }
        }

        /// <summary>
        /// Gets or sets layouting bounds.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }

            set
            {
                m_bounds = value;
            }
        }

        /// <summary>
        /// Gets or sets layouting settings.
        /// </summary>
        public PdfLayoutFormat Format
        {
            get
            {
                return m_format;
            }

            set
            {
                m_format = value;
            }
        }
        #endregion
    }
}
