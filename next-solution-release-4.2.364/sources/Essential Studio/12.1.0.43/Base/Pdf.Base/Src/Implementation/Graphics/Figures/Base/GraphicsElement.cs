#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Text;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents a base class for all page graphics elements.
    /// </summary>
    public abstract class PdfGraphicsElement
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGraphicsElement"/> class.
        /// </summary>
        protected PdfGraphicsElement()
            : base()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        public void Draw(PdfGraphics graphics)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            Draw(graphics, PointF.Empty);
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        /// <param name="location">Location of the element in the Graphics' co-ordinate system.</param>
        public void Draw(PdfGraphics graphics, PointF location)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            Draw(graphics, location.X, location.Y);
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        /// <param name="x">X co-ordinate of the element.</param>
        /// <param name="y">Y co-ordinate of the element.</param>
        public virtual void Draw(PdfGraphics graphics, float x, float y)
        {
            bool bNeedSave = (x != 0f || y != 0f);
            PdfGraphicsState gState = null;

            // Translate co-ordinates.
            if (bNeedSave)
            {
                // Save state.
                gState = graphics.Save();
                graphics.TranslateTransform(x, y);
            }

            // Draw the element.
            DrawInternal(graphics);

            if (bNeedSave)
            {
                // Restore state.
                graphics.Restore(gState);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        #if !NETFX_CORE && !WP
        [Syncfusion.Documentation.DocumentationExclude()]
        #endif
        protected abstract void DrawInternal(PdfGraphics graphics);
        #endregion
    }
}
