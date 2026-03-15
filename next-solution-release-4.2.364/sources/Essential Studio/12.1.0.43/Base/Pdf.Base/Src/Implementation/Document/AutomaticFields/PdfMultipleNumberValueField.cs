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
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents automatic field which has the same value within the <see cref="PdfGraphics"/>
    /// </summary>
    /// <seealso cref="PdfMultipleValueField"/> Class   
    public abstract class PdfMultipleNumberValueField : PdfMultipleValueField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store numbering style.
        /// </summary>
        private PdfNumberStyle m_numberStyle = PdfNumberStyle.Numeric;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMultipleNumberValueField"/> class.
        /// </summary>
        public PdfMultipleNumberValueField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMultipleNumberValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfMultipleNumberValueField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMultipleNumberValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfMultipleNumberValueField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMultipleNumberValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        public PdfMultipleNumberValueField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the number style.
        /// </summary>
        /// <value>The number style.</value>
        public PdfNumberStyle NumberStyle
        {
            get
            {
                return m_numberStyle;
            }

            set
            {
                m_numberStyle = value;
            }
        }
        #endregion
    }
}
