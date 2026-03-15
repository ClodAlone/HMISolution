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

using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace Syncfusion.Pdf.HtmlToPdf
{
    /// <summary>
    /// Represents the html hyperlink used during the html to pdf conversion to preserve live-links.    
    /// </summary>
	/// <para>This class is used internally and should not be used directly.</para>
    internal class HtmlHyperLink
    {
#region Fields
        /// <summary>
        /// The bounds which the html element occupies.
        /// </summary>
        private RectangleF m_bounds;

        /// <summary>
        /// The target Url.
        /// </summary>
        private string m_href;
        /// <summary>
        /// The matching name of the document link.
        /// </summary>
        private string m_name;
        /// <summary>
        /// The id of the destination.
        /// </summary>
        private string m_hash;
        #endregion

#region Construtor
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlHyperLink"/> class.
        /// </summary>
        /// <param name="Bounds">The bounds.</param>
        /// <param name="Href">The href.</param>
        public HtmlHyperLink(RectangleF Bounds, string Href)
        {
            m_bounds = Bounds;
            m_href = Href;
            ConvertBoundsToPoint();
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets or sets the bounds.
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
        /// Gets or sets the id of the destination.
        /// </summary>
        internal String Hash
        {
            get
            {
                return m_hash;
            }
            set
            {
                m_hash = value;
            }
        }

        /// <summary>
        /// Gets or sets the name (id) of the document link.
        /// </summary>
        internal string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or Sets the Url.
        /// </summary>
        public string Href
        {
            get
            {
                return m_href;
            }
        }
        #endregion

#region Implementation
        /// <summary>
        /// Converts the bounds from pixel to point.
        /// </summary>
        internal void ConvertBoundsToPoint()
        {
            PdfUnitConvertor converter = new PdfUnitConvertor();
            m_bounds = converter.ConvertFromPixels(m_bounds, PdfGraphicsUnit.Point);
        }
        #endregion
    }
}
#endif