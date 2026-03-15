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
    /// Represents automatic field which value is dynamically evaluated.
    /// </summary>
    /// <seealso cref="PdfAutomaticField"/> Class  
    public abstract class PdfDynamicField : PdfAutomaticField
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDynamicField"/> class.
        /// </summary>
        public PdfDynamicField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDynamicField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfDynamicField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDynamicField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfDynamicField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDynamicField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        public PdfDynamicField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the page from a graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <returns>The proper PdfPage instance.</returns>
        internal static PdfPage GetPageFromGraphics(PdfGraphics graphics)
        {
            PdfPage page = graphics.Page as PdfPage;

            if (page == null)
            {
                throw new NotSupportedException("The field was placed on not PdfPage class instance.");
            }

            return page;
        }

        /// <summary>
        /// Gets the  Loaded page from a graphics.
        /// </summary>
        /// <param name="graphics">The graphics</param>
        /// <returns>The graphics</returns>
        internal static PdfLoadedPage GetLoadedPageFromGraphics(PdfGraphics graphics)
        {
            PdfLoadedPage page = graphics.Page as PdfLoadedPage;

            if (page == null)
            {
                throw new NotSupportedException("The field was placed on not PdfPage class instance.");
            }

            return page;
        }
        #endregion
    }
}
