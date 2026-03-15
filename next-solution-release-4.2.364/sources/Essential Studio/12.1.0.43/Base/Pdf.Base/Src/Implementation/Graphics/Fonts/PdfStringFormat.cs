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
    /// Represents the text layout information.
    /// </summary>
    public sealed class PdfStringFormat : ICloneable
    {
        #region Fields
        /// <summary>
        /// Horizontal text alignment.
        /// </summary>
        private PdfTextAlignment m_alignment;

        /// <summary>
        /// Vertical text alignment.
        /// </summary>
        private PdfVerticalAlignment m_lineAlignment;

        /// <summary>
        /// Indicates whether RTL should be checked.
        /// </summary>
        private bool m_rightToLeft;

        /// <summary>
        /// Character spacing value.
        /// </summary>
        private float m_characterSpacing;

        /// <summary>
        /// Word spacing value.
        /// </summary>
        private float m_wordSpacing;

        /// <summary>
        /// Text leading.
        /// </summary>
        private float m_leading;

        /// <summary>
        /// Shows if the text should be a part of the current clipping path.
        /// </summary>
        private bool m_clip;

        /// <summary>
        /// Indicates whether the text is in subscript or superscript mode.
        /// </summary>
        private PdfSubSuperScript m_subSuperScript;

        /// <summary>
        /// The scaling factor of the text being drawn.
        /// </summary>
        private float m_scalingFactor = 100.0f;

        /// <summary>
        /// Indent of the first line in the text.
        /// </summary>
        private float m_firstLineIndent;

        /// <summary>
        /// Indent of the first line in the paragraph.
        /// </summary>
        private float m_paragraphIndent;

        /// <summary>
        /// Indicates whether entire lines are laid out in the formatting rectangle only or not.
        /// </summary>
        private bool m_lineLimit;

        /// <summary>
        /// Indicates whether spaces at the end of the line should be left or removed.
        /// </summary>
        private bool m_measureTrailingSpaces;

        /// <summary>
        /// Indicates whether the text region should be clipped or not.
        /// </summary>
        private bool m_noClip;

        /// <summary>
        /// Indicates text wrapping type.
        /// </summary>
        private PdfWordWrapType m_wrapType;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStringFormat"/> class.
        /// </summary>
        public PdfStringFormat()
        {
            m_lineLimit = true;
            m_wrapType = PdfWordWrapType.Word;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStringFormat"/> class.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        public PdfStringFormat(PdfTextAlignment alignment)
            : this()
        {
            m_alignment = alignment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStringFormat"/> class.
        /// </summary>
        /// <param name="columnFormat">The column format.</param>
        public PdfStringFormat(string columnFormat)
            : this()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStringFormat"/> class.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        /// <param name="lineAlignment">The vertical alignment.</param>
        public PdfStringFormat(PdfTextAlignment alignment, PdfVerticalAlignment lineAlignment)
            : this(alignment)
        {
            m_lineAlignment = lineAlignment;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public PdfTextAlignment Alignment
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
        /// Gets or sets the vertical text alignment.
        /// </summary>
        public PdfVerticalAlignment LineAlignment
        {
            get
            {
                return m_lineAlignment;
            }

            set
            {
                m_lineAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the value that indicates text direction mode.
        /// </summary>
        /// <remarks>Note, that this property doesn't change any alignment of the text. 
        /// <see cref="Alignment"/> property should be set manually to align the text. This property just enables or disables
        /// support of right to left approach. 
        /// If the value is False, the text won't be checked for right to left symbols occurrence.</remarks>
#if AllowUnsafeCode
        public bool RightToLeft
#else
        internal bool RightToLeft
#endif
        {
            get
            {
                return m_rightToLeft;
            }

            set
            {
                m_rightToLeft = value;
            }
        }

        /// <summary>
        /// Gets or sets value that indicates a size among the characters in the text.
        /// When the glyph for each character in the string is rendered, this value is
        /// added to the the glyph�s displacement.
        /// </summary>
        /// <remarks>
        /// Default value is 0.</remarks>
        public float CharacterSpacing
        {
            get
            {
                return m_characterSpacing;
            }

            set
            {
                m_characterSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets value that indicates a size among the words in the text.
        /// Word spacing works the same way as character spacing but applies only to the
        /// space character, code 32.
        /// </summary>
        /// <remarks>Default value is 0.</remarks>
        public float WordSpacing
        {
            get
            {
                return m_wordSpacing;
            }

            set
            {
                m_wordSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets value that indicates the vertical distance between the baselines of adjacent lines of text.
        /// </summary>
        /// <remarks>Default value is 0.</remarks>
        public float LineSpacing
        {
            get
            {
                return m_leading;
            }

            set
            {
                m_leading = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text
        /// should be a part of the clipping path.
        /// </summary>
        public bool ClipPath
        {
            get
            {
                return m_clip;
            }

            set
            {
                m_clip = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether the text is in subscript or superscript mode.
        /// </summary>
        public PdfSubSuperScript SubSuperScript
        {
            get
            {
                return m_subSuperScript;
            }

            set
            {
                m_subSuperScript = value;
            }
        }

        /// <summary>
        /// Gets or sets the indent of the first line in the paragraph.
        /// </summary>
        public float ParagraphIndent
        {
            get
            {
                return m_paragraphIndent;
            }

            set
            {
                m_paragraphIndent = value;
                FirstLineIndent = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [line limit].
        /// </summary>
        /// <value><c>true</c> if [line limit]; otherwise, <c>false</c>.</value>
        public bool LineLimit
        {
            get
            {
                return m_lineLimit;
            }

            set
            {
                m_lineLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [measure trailing spaces].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [measure trailing spaces]; otherwise, <c>false</c>.
        /// </value>
        public bool MeasureTrailingSpaces
        {
            get
            {
                return m_measureTrailingSpaces;
            }

            set
            {
                m_measureTrailingSpaces = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no clip].
        /// </summary>
        /// <value><c>true</c> if [no clip]; otherwise, <c>false</c>.</value>
        public bool NoClip
        {
            get
            {
                return m_noClip;
            }

            set
            {
                m_noClip = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating type of the text wrapping.
        /// </summary>
        public PdfWordWrapType WordWrap
        {
            get
            {
                return m_wrapType;
            }

            set
            {
                m_wrapType = value;
            }
        }

        /// <summary>
        /// Gets or sets the scaling factor.
        /// </summary>
        /// <remarks>The default scaling factor is 100, which means 100% and original size.
        /// It's used to make PDF font looking smaller when metafile is rendered into PDF.</remarks>
        internal float HorizontalScalingFactor
        {
            get
            {
                return m_scalingFactor;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("The scaling factor can't be less of equal to zero.", "ScalingFactor");
                }

                m_scalingFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the indent of the first line in the text.
        /// </summary>
        internal float FirstLineIndent
        {
            get
            {
                return m_firstLineIndent;
            }

            set
            {
                m_firstLineIndent = value;
            }
        }

        #endregion

        #region IClonable implementation
        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The new created object.</returns>
        public object Clone()
        {
            PdfStringFormat format = (PdfStringFormat)MemberwiseClone();
            return format;
        }
        #endregion
    }
}
