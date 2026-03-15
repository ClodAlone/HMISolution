#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Encapsulates a page template for all the pages in the section.
    /// </summary>
    public class PdfSectionTemplate : PdfDocumentTemplate
    {
        #region Fields
        /// <summary>
        /// Left settings.
        /// </summary>
        private bool m_left;
        /// <summary>
        /// Top settings.
        /// </summary>
        private bool m_top;
        /// <summary>
        /// Right settings.
        /// </summary>
        private bool m_right;
        /// <summary>
        /// Bottom settings.
        /// </summary>
        private bool m_bottom;
        /// <summary>
        /// Other templates settings
        /// </summary>
        private bool m_stamp;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value indicating whether parent Left page template should be used or not.
        /// </summary>
        public bool ApplyDocumentLeftTemplate
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether parent Top page template should be used or not.
        /// </summary>
        public bool ApplyDocumentTopTemplate
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether parent Right page template should be used or not.
        /// </summary>
        public bool ApplyDocumentRightTemplate
        {
            get
            {
                return m_right;
            }
            set
            {
                m_right = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether parent Bottom page template should be used or not.
        /// </summary>
        public bool ApplyDocumentBottomTemplate
        {
            get
            {
                return m_bottom;
            }
            set
            {
                m_bottom = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether 
        /// the parent stamp elements should be used or not.
        /// </summary>
        public bool ApplyDocumentStamps
        {
            get
            {
                return m_stamp;
            }
            set
            {
                m_stamp = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new object.
        /// </summary>
        public PdfSectionTemplate()
            : base()
        {
            m_left = m_top = m_right = m_bottom = m_stamp = true;
        }
        #endregion
    }
}
