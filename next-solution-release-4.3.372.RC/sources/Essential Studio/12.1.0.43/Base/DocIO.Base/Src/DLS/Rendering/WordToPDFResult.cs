#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Syncfusion.DocIO.Rendering;

namespace Syncfusion.DocIO.DLS.Rendering
{
    internal class WordToPDFResult
    {
        #region Fields
        private List<PageResult> m_pages;
        #endregion
        #region Properties
        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public List<PageResult> Pages
        {
            get
            {
                return m_pages;
            }
        }
        #endregion
        #region Construcor
        /// <summary>
        /// Initializes a new instance of the <see cref="WordToPDFResult"/> class.
        /// </summary>
        public WordToPDFResult()
        {
            m_pages = new List<PageResult>();
        }
        #endregion
    }

    internal class PageResult
    {
        #region Fields
        private Image m_image;
        private List<Dictionary<string, RectangleF>> m_hyperLinks;
        private List<Dictionary<string, BookmarkHyperlink>> m_bookmarkHyperlinks;
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the rendered image.
        /// </summary>
        /// <value>The rendered image.</value>
        public Image PageImage
        {
            get
            {
                return m_image;
            }
            set
            {
                m_image = value;
            }
        }

        /// <summary>
        /// Gets or sets the hyperlinks.
        /// </summary>
        /// <value>The hyperlinks.</value>
        public List<Dictionary<string, RectangleF>> Hyperlinks
        {
            get
            {
                return m_hyperLinks;
            }
            set
            {
                m_hyperLinks = value;
            }
        }
        public List<Dictionary<string, BookmarkHyperlink>> BookmarkHyperlinks
        {
            get
            {
                return m_bookmarkHyperlinks;
            }
            set
            {
                m_bookmarkHyperlinks = value;
            }
        }
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PageResult"/> class.
        /// </summary>
        public PageResult()
        {
        }

        public PageResult(Image image, List<Dictionary<string, RectangleF>> hyperlinks, List<Dictionary<string, BookmarkHyperlink>> bookmarkHyperlinks)
        {
            m_image = image;
            m_hyperLinks = hyperlinks;
            m_bookmarkHyperlinks = bookmarkHyperlinks;
        }
        #endregion
    }
}

#endif
