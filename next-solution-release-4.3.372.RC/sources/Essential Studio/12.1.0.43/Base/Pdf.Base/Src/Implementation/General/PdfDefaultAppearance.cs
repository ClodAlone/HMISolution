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

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents default appearance string.
    /// </summary>
    internal class PdfDefaultAppearance
    {
        #region Fields
        /// <summary>
        /// Internal variable to store fore color.
        /// </summary>
        private PdfColor m_foreColor = new PdfColor(0, 0, 0);

        /// <summary>
        /// Internal variable to store font name.
        /// </summary>
        private string m_fontName = String.Empty;

        /// <summary>
        /// Internal variable to store font size.
        /// </summary>
        private float m_fontSize = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDefaultAppearance"/> class.
        /// </summary>
        public PdfDefaultAppearance()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get
            {
                return m_fontName;
            }

            set
            {
                if (m_fontName != value)
                {
                    m_fontName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public float FontSize
        {
            get
            {
                return m_fontSize;
            }

            set
            {
                if (m_fontSize != value)
                {
                    m_fontSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>The color of the fore.</value>
        public PdfColor ForeColor
        {
            get
            {
                return m_foreColor;
            }

            set
            {
                if (m_foreColor != value)
                {
                    m_foreColor = value;
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(Operators.Slash);
            builder.Append(FontName);
            builder.Append(Operators.WhiteSpace);
            builder.Append(m_fontSize.ToString());
            builder.Append(Operators.WhiteSpace);
            builder.Append(Operators.SetFont);
            builder.Append(Operators.WhiteSpace);
            builder.Append(m_foreColor.ToString(PdfColorSpace.RGB, false));

            return builder.ToString();
        }
        #endregion
    }
}
