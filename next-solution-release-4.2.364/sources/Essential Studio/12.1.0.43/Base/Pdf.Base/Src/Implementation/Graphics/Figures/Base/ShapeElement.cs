#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Base class for the main shapes.
    /// </summary>
    public abstract class PdfShapeElement : PdfLayoutElement
    {
        #region Public methods
        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <returns>rect</returns>
        public RectangleF GetBounds()
        {
            RectangleF rect = GetBoundsInternal();

            return rect;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns a rectangle that bounds this element.
        /// </summary>
        /// <returns>Returns a rectangle that bounds this element.</returns>
        /// <remarks>This method doesn't take into consideration a rotation of the element.</remarks>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected abstract RectangleF GetBoundsInternal();

        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Returns lay outing results.</returns>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override PdfLayoutResult Layout(PdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            ShapeLayouter layouter = new ShapeLayouter(this);
            PdfLayoutResult result = layouter.Layout(param);

            return result;
        }
        #endregion
    }
}
