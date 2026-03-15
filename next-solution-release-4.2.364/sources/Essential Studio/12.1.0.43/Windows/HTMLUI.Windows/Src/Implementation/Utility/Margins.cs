#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Utility class representing margins support in the document.
    /// </summary>
    public class Margins
    {
        #region Class members
        /// <summary>
        /// Body tag element.
        /// </summary>
        private BODYElementImpl m_body;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the left margin attribute for the element.
        /// </summary>
        public int Left
        {
            get
            {
                return Body.LeftMargin;
            }
            set
            {
                Body.LeftMargin = value;
            }
        }

        /// <summary>
        /// Gets or sets the top margin attribute for the element.
        /// </summary>
        public int Top
        {
            get
            {
                return Body.TopMargin;
            }
            set
            {
                Body.TopMargin = value;
            }
        }

        /// <summary>
        /// Gets or sets the right margin attribute for the element.
        /// </summary>
        public int Right
        {
            get
            {
                return Body.RightMargin;
            }
            set
            {
                Body.RightMargin = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom margin attribute for the element.
        /// </summary>
        public int Bottom
        {
            get
            {
                return Body.BottomMargin;
            }
            set
            {
                Body.BottomMargin = value;
            }
        }

        /// <summary>
        /// Gets the body tag element containing margins values.
        /// </summary>
        private BODYElementImpl Body
        {
            get
            {
                return m_body;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the Margins class from being created
        /// </summary>
        private Margins()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Margins class
        /// </summary>
        /// <param name="body">Body tag element.</param>
        internal Margins(BODYElementImpl body)
        {
            if (body == null)
                throw new ArgumentNullException("body");

            m_body = body;
        }
        #endregion
    }
}
