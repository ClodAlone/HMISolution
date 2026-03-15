#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Lists namespace contains classes for creating structure elements in PDF document.
/// </summary>
namespace Syncfusion.Pdf.Lists
{
    /// <summary>
    /// Represents base class for markers.
    /// </summary>
    public abstract class PdfMarker
    {
        #region Fields
        /// <summary>
        /// Marker font.
        /// </summary>
        private PdfFont m_font;

        /// <summary>
        /// Marker brush.
        /// </summary>
        private PdfBrush m_brush;

        /// <summary>
        /// Marker pen.
        /// </summary>
        private PdfPen m_pen;

        /// <summary>
        /// The string format of the marker.
        /// </summary>
        private PdfStringFormat m_format;

        /// <summary>
        /// Marker alignment.
        /// </summary>
        private PdfListMarkerAlignment m_alignment;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets marker font.
        /// </summary>		
        public PdfFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }

        /// <summary>
        /// Gets or sets marker brush.
        /// </summary>
        public PdfBrush Brush
        {
            get
            {
                return m_brush;
            }
            set
            {
                m_brush = value;
            }
        }

        /// <summary>
        /// Gets or sets marker pen.
        /// </summary>
        public PdfPen Pen
        {
            get
            {
                return m_pen;
            }
            set
            {
                m_pen = value;
            }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public PdfStringFormat StringFormat
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

        /// <summary>
        /// Gets or sets a value indicating whether the marker is
        /// situated at the left of the list or at the right of the list.
        /// </summary>
        public PdfListMarkerAlignment Alignment
        {
            get
            {
                return m_alignment;
            }
            set
            {
                m_alignment = value;
            }
        }

        /// <summary>
        /// Indicates is alignment right.
        /// </summary>
        internal bool RightToLeft
        {
            get
            {
                return (m_alignment == PdfListMarkerAlignment.Right);
            }
        }
        #endregion
    }
}