#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Text;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Encapsulates a page template for all the pages in the document.
    /// </summary>
    public class PdfDocumentTemplate
    {
        #region Fields
        /// <summary>
        /// Left page template object.
        /// </summary>
        private PdfPageTemplateElement m_left;
        /// <summary>
        /// Top page template object.
        /// </summary>
        private PdfPageTemplateElement m_top;
        /// <summary>
        /// Right page template object.
        /// </summary>
        private PdfPageTemplateElement m_right;
        /// <summary>
        /// Bottom page template object.
        /// </summary>
        private PdfPageTemplateElement m_bottom;

        /// <summary>
        /// Even Left page template object.
        /// </summary>
        private PdfPageTemplateElement m_evenLeft;
        /// <summary>
        /// Even Top page template object.
        /// </summary>
        private PdfPageTemplateElement m_evenTop;
        /// <summary>
        /// Even Right page template object.
        /// </summary>
        private PdfPageTemplateElement m_evenRight;
        /// <summary>
        /// Even Bottom page template object.
        /// </summary>
        private PdfPageTemplateElement m_evenBottom;

        /// <summary>
        /// Odd Left page template object.
        /// </summary>
        private PdfPageTemplateElement m_oddLeft;
        /// <summary>
        /// Odd Top page template object.
        /// </summary>
        private PdfPageTemplateElement m_oddTop;
        /// <summary>
        /// Odd Right page template object.
        /// </summary>
        private PdfPageTemplateElement m_oddRight;
        /// <summary>
        /// Odd Bottom page template object.
        /// </summary>
        private PdfPageTemplateElement m_oddBottom;

        /// <summary>
        /// The collection of the stamp elements.
        /// </summary>
        private PdfStampCollection m_stamps;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a left page template.
        /// </summary>
        public PdfPageTemplateElement Left
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = CheckElement(value, TemplateType.Left);
            }
        }

        /// <summary>
        /// Gets or sets a top page template.
        /// </summary>
        public PdfPageTemplateElement Top
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = CheckElement(value, TemplateType.Top);
            }
        }

        /// <summary>
        /// Gets or sets a right page template.
        /// </summary>
        public PdfPageTemplateElement Right
        {
            get
            {
                return m_right;
            }
            set
            {
                m_right = CheckElement(value, TemplateType.Right);
            }
        }

        /// <summary>
        /// Gets or sets a bottom page template.
        /// </summary>
        public PdfPageTemplateElement Bottom
        {
            get
            {
                return m_bottom;
            }
            set
            {
                m_bottom = CheckElement(value, TemplateType.Bottom);
            }
        }

        /// <summary>
        /// Gets or sets a left page template using on the even pages.
        /// </summary>
        public PdfPageTemplateElement EvenLeft
        {
            get
            {
                return m_evenLeft;
            }
            set
            {
                m_evenLeft = CheckElement(value, TemplateType.Left);
            }
        }

        /// <summary>
        /// Gets or sets a top page template using on the even pages.
        /// </summary>
        public PdfPageTemplateElement EvenTop
        {
            get
            {
                return m_evenTop;
            }
            set
            {
                m_evenTop = CheckElement(value, TemplateType.Top);
            }
        }

        /// <summary>
        /// Gets or sets a right page template using on the even pages.
        /// </summary>
        public PdfPageTemplateElement EvenRight
        {
            get
            {
                return m_evenRight;
            }
            set
            {
                m_evenRight = CheckElement(value, TemplateType.Right);
            }
        }

        /// <summary>
        /// Gets or sets a bottom page template using on the even pages.
        /// </summary>
        public PdfPageTemplateElement EvenBottom
        {
            get
            {
                return m_evenBottom;
            }
            set
            {
                m_evenBottom = CheckElement(value, TemplateType.Bottom);
            }
        }


        /// <summary>
        /// Gets or sets a left page template using on the odd pages.
        /// </summary>
        public PdfPageTemplateElement OddLeft
        {
            get
            {
                return m_oddLeft;
            }
            set
            {
                m_oddLeft = CheckElement(value, TemplateType.Left);
            }
        }

        /// <summary>
        /// Gets or sets a top page template using on the odd pages.
        /// </summary>
        public PdfPageTemplateElement OddTop
        {
            get
            {
                return m_oddTop;
            }
            set
            {
                m_oddTop = CheckElement(value, TemplateType.Top);
            }
        }

        /// <summary>
        /// Gets or sets a right page template using on the odd pages.
        /// </summary>
        public PdfPageTemplateElement OddRight
        {
            get
            {
                return m_oddRight;
            }
            set
            {
                m_oddRight = CheckElement(value, TemplateType.Right);
            }
        }

        /// <summary>
        /// Gets or sets a bottom page template using on the odd pages.
        /// </summary>
        public PdfPageTemplateElement OddBottom
        {
            get
            {
                return m_oddBottom;
            }
            set
            {
                m_oddBottom = CheckElement(value, TemplateType.Bottom);
            }
        }


        /// <summary>
        /// Gets a collection of stamp elements.
        /// </summary>
        public PdfStampCollection Stamps
        {
            get
            {
                if (m_stamps == null)
                {
                    m_stamps = new PdfStampCollection();
                }

                return m_stamps;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfDocumentTemplate"/> class.
        /// </summary>
        public PdfDocumentTemplate()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns left template.
        /// </summary>
        /// <param name="page">Page where the template should be printed.</param>
        /// <returns>Returns left template.</returns>
        internal PdfPageTemplateElement GetLeft(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            PdfPageTemplateElement template = null;
            bool even = IsEven(page);

            if (even)
            {
                template = (EvenLeft != null) ? EvenLeft : Left;
            }
            else
            {
                template = (OddLeft != null) ? OddLeft : Left;
            }

            return template;
        }

        /// <summary>
        /// Returns top template.
        /// </summary>
        /// <param name="page">Page where the template should be printed.</param>
        /// <returns>Returns top template.</returns>
        internal PdfPageTemplateElement GetTop(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            PdfPageTemplateElement template = null;
            bool even = IsEven(page);

            if (even)
            {
                template = (EvenTop != null) ? EvenTop : Top;
            }
            else
            {
                template = (OddTop != null) ? OddTop : Top;
            }

            return template;
        }

        /// <summary>
        /// Returns right template.
        /// </summary>
        /// <param name="page">Page where the template should be printed.</param>
        /// <returns>Returns right template.</returns>
        internal PdfPageTemplateElement GetRight(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            PdfPageTemplateElement template = null;
            bool even = IsEven(page);

            if (even)
            {
                template = (EvenRight != null) ? EvenRight : Right;
            }
            else
            {
                template = (OddRight != null) ? OddRight : Right;
            }

            return template;
        }

        /// <summary>
        /// Returns bottom template.
        /// </summary>
        /// <param name="page">Page where the template should be printed.</param>
        /// <returns>Returns bottom template.</returns>
        internal PdfPageTemplateElement GetBottom(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            PdfPageTemplateElement template = null;
            bool even = IsEven(page);

            if (even)
            {
                template = (EvenBottom != null) ? EvenBottom : Bottom;
            }
            else
            {
                template = (OddBottom != null) ? OddBottom : Bottom;
            }

            return template;
        }

        /// <summary>
        /// Checks whether the page is even or odd.
        /// </summary>
        /// <param name="page">The page object.</param>
        /// <returns>True if the page is even, false otherwise.</returns>
        private bool IsEven(PdfPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            PdfDocumentPageCollection pages = page.Section.Document.Pages;
            int index = 0;
            if(pages.PageCollectionIndex.ContainsKey(page))
            {
                index = pages.PageCollectionIndex[page] + 1;
            }
            else
            {
                index = pages.IndexOf(page) + 1;
            }

            bool even = ((index % 2) == 0);
            return even;
        }

        /// <summary>
        /// Checks a template element.
        /// </summary>
        /// <param name="templateElement">The template element.</param>
        /// <param name="type">The type that should be assigned to the template element.</param>
        /// <returns>
        /// The template element which passed the check.
        /// </returns>
        private PdfPageTemplateElement CheckElement(PdfPageTemplateElement templateElement, TemplateType type)
        {
            if (templateElement != null)
            {
                if (templateElement.Type != TemplateType.None)
                    throw new NotSupportedException("Can't reassign the template element. Please, create new one.");

                templateElement.Type = type;
            }

            return templateElement;
        }
        #endregion
    }
}
