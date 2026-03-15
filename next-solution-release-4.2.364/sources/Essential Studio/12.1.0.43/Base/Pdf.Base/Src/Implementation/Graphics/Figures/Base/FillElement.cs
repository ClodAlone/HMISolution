#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents an element that could be drawn and/or filled.
    /// </summary>
    public abstract class PdfFillElement : PdfDrawElement
    {
        #region Fields
        /// <summary>
        /// Internal variable to store brush.
        /// </summary>
        private PdfBrush m_brush = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFillElement"/> class.
        /// </summary>
        protected PdfFillElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFillElement"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        protected PdfFillElement(PdfPen pen)
            : base(pen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFillElement"/> class.
        /// </summary>
        /// <param name="brush">The brush.</param>
        protected PdfFillElement(PdfBrush brush)
            : this()
        {
            m_brush = brush;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFillElement"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        protected PdfFillElement(PdfPen pen, PdfBrush brush)
            : this(pen)
        {
            m_brush = brush;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the brush.
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
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the pen. If both pen and brush are not explicitly defined, default pen will be used.
        /// </summary>
        /// <exclude/>
        /// <returns> brush </returns>
        protected override PdfPen GetPen()
        {
            return (m_brush == null) && (Pen == null) ? PdfPens.Black : Pen;
        }
        #endregion
    }
}
