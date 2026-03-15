#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.Graphics;

namespace Syncfusion.Pdf.HtmlToPdf
{
    /// <summary>
    /// Represents the layout parameters.
    /// </summary>
    
    public class HtmlToPdfLayoutParams : PdfLayoutParams
    {
#region Fields
        /// <summary>
        /// Start lay outing page.
        /// </summary>
        private PdfPage m_page;

        /// <summary>
        /// The top
        /// </summary>
        private float[] m_verticalOffsets;

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
        /// Gets or sets the starting layout page.
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
        /// Gets or sets the lay outing bounds.
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
        /// Gets or sets the vertical offsets.
        /// </summary>
        /// <value>The vertical offsets.</value>
        public float[] VerticalOffsets
        {
            get
            {
                return m_verticalOffsets;
            }
           
            set
            {
                m_verticalOffsets = value;
            }
        }
       
        /// <summary>
        /// Gets or sets the lay outing settings.
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
#endif