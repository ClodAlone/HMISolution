#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;
using System.Drawing;

using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents automatic field which value can be evaluated in the moment of creation.
    /// </summary>
    /// <seealso cref="PdfAutomaticField"/> Class    
    public abstract class PdfStaticField : PdfAutomaticField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store template of the field.
        /// </summary>
        private PdfTemplate m_template = null;

        /// <summary>
        /// Internal variable to store list of graphicses.
        /// </summary>
        private List<PdfGraphics> m_graphicsList = new List<PdfGraphics>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStaticField"/> class.
        /// </summary>
        public PdfStaticField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStaticField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfStaticField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStaticField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfStaticField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStaticField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        public PdfStaticField(PdfFont font, RectangleF bounds)
            : base(font, bounds)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Performs draw.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="location">The location.</param>
        /// <param name="scalingX">The scaling X.</param>
        /// <param name="scalingY">The scaling Y.</param>
        protected internal override void PerformDraw(PdfGraphics graphics, PointF location,
            float scalingX, float scalingY)
        {
            base.PerformDraw(graphics, location, scalingX, scalingY);

            string value = GetValue(graphics);
            PointF drawLocation = new PointF(location.X + Location.X, location.Y + Location.Y);

            if (m_template == null)
            {
                m_template = new PdfTemplate(GetSize());
                m_template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(),
                    new RectangleF(PointF.Empty, GetSize()), StringFormat);
                graphics.DrawPdfTemplate(m_template, drawLocation,
                    new SizeF(m_template.Width * scalingX, m_template.Height * scalingY));
                m_graphicsList.Add(graphics);
            }
            else
            {
                if (!m_graphicsList.Contains(graphics))
                {
                    graphics.DrawPdfTemplate(m_template, drawLocation,
                        new SizeF(m_template.Width * scalingX, m_template.Height * scalingY));
                    m_graphicsList.Add(graphics);
                }
            }
        }
        #endregion
    }
}
