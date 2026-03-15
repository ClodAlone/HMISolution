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
    /// Represents automatic field which has the same value within the <see cref="PdfGraphics"/>
    /// </summary>
    /// <seealso cref="PdfDynamicField"/> Class  
    public abstract class PdfMultipleValueField : PdfDynamicField
    {
        #region Fields
        /// <summary>
        /// Internal variable to store templates associated with the graphics.
        /// </summary>
        private Dictionary<PdfGraphics, PdfTemplateValuePair> m_list = new Dictionary<PdfGraphics, PdfTemplateValuePair>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMultipleValueField"/> class.
        /// </summary>
        public PdfMultipleValueField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMultipleValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public PdfMultipleValueField(PdfFont font)
            : base(font)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMultipleValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public PdfMultipleValueField(PdfFont font, PdfBrush brush)
            : base(font, brush)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMultipleValueField"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="bounds">The bounds.</param>
        public PdfMultipleValueField(PdfFont font, RectangleF bounds)
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
        protected internal override void PerformDraw(PdfGraphics graphics, PointF location, float scalingX, float scalingY)
        {
            base.PerformDraw(graphics, location, scalingX, scalingY);

            string value = GetValue(graphics);

            if (m_list.ContainsKey(graphics))
            {
                PdfTemplateValuePair pair = (PdfTemplateValuePair)m_list[graphics];

                if (pair.Value != value)
                {
                    SizeF size = GetSize();
                    pair.Template.Reset(size);
                    pair.Template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(), new RectangleF(PointF.Empty, size), StringFormat);
                }
            }
            else
            {
                PdfTemplate template = new PdfTemplate(GetSize());

                m_list[graphics] = new PdfTemplateValuePair(template, value);
                template.Graphics.DrawString(value, GetFont(), Pen, GetBrush(), new RectangleF(PointF.Empty, GetSize()), StringFormat);

                PointF drawLocation = new PointF(location.X + Location.X, location.Y + Location.Y);

                graphics.DrawPdfTemplate(template, drawLocation,
                    new SizeF(template.Width * scalingX, template.Height * scalingY));
            }
        }
        #endregion
    }
}
