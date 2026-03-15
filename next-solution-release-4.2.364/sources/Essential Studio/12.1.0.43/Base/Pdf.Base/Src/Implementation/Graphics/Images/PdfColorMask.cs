#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the color mask for bitmaps.
    /// </summary>
    public class PdfColorMask : PdfMask
    {
        #region Fields
        /// <summary>
        /// Holds start color of color mask.
        /// </summary>
        private PdfColor m_startColor;
        /// <summary>
        /// Holds end color of color mask.
        /// </summary>
        private PdfColor m_endColor;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the start color.
        /// </summary>
        /// <value>The start color.</value>
        public PdfColor StartColor
        {
            get
            {
                return m_startColor;
            }
            set
            {
                m_startColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the end color.
        /// </summary>
        /// <value>The end color.</value>
        public PdfColor EndColor
        {
            get
            {
                return m_endColor;
            }
            set
            {
                m_endColor = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates new PdfColorMask object.
        /// </summary>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        public PdfColorMask(PdfColor startColor, PdfColor endColor)
        {
            m_endColor = endColor;
            m_startColor = startColor;
        }
        #endregion
    }
}
