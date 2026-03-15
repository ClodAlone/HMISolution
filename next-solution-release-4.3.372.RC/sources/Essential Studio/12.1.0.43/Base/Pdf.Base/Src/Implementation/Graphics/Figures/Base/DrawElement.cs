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
    /// Describes a graphics element which can be drawn by a pen.
    /// </summary>
    public abstract class PdfDrawElement : PdfShapeElement
    {
        #region Fields
        /// <summary>
        /// A pen object.
        /// </summary>
        private PdfPen m_pen;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDrawElement"/> class.
        /// </summary>
        protected PdfDrawElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDrawElement"/> class.
        /// </summary>
        /// <param name="pen">The pen.</param>
        protected PdfDrawElement(PdfPen pen)
            : this()
        {
            m_pen = pen;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a pen that will be used to draw the element.
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
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the pen. If both pen and brush are not explicitly defined, default pen will be used.
        /// </summary>
        /// <returns>Gets the pen for drawing.</returns>
        /// <exclude/>
        protected virtual PdfPen GetPen()
        {
            return (m_pen == null) ? PdfPens.Black : m_pen;
        }
        #endregion
    }
}
