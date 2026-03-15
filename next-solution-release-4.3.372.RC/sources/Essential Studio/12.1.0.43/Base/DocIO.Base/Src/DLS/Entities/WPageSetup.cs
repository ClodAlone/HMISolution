#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using Syncfusion.DocIO.DLS;
using System.Windows;
using Syncfusion.DocIO.DLS.XML;
#if !WINRT && !WP
using System.Drawing;
#endif
#if !SILVERLIGHT && !WP
using System.Text;
using System.Collections.Generic;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WPageSetup.
    /// </summary>
    public class WPageSetup : XDLSSerializableBase
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const float DEF_PAGE_WIDTH = 595.3f; //595
        /// <summary>
        /// 
        /// </summary>
        private const float DEF_PAGE_HEIGHT = 841.9f; //842
        /// <summary>
        /// 
        /// </summary>
        private const float DEF_PAGE_MARGINS = 20; //42.5f;
        /// <summary>
        /// 
        /// </summary>
        private const float DEF_PAGE_MARGIN_LEFT = 50; //70.85f;
        /// <summary>
        /// 
        /// </summary>
        internal const float DEF_AUTO_TAB_LENGHT = 36f;
        /// <summary>
        /// Limit number of converting arabic to \"A\" format.
        /// </summary>
        private const float DEF_AR_TO_LETTER_LIMIT = 26.0f;
        /// <summary>
        /// Index of A char in the ASCII table.
        /// </summary>
        private const int DEF_A_ASCII_INDEX = (int)('A' - 1);
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private SizeF m_pageSize;
        private PageOrientation m_orientation;
        private MarginsF m_margins;
        protected float m_fHeaderDistance;
        protected float m_fFooterDistance;
        private PageAlignment m_vertAlignment;
        /// <summary>
        /// The title page mark
        /// </summary>
        private bool m_titlePage = false;
        /// <summary>
        /// 
        /// </summary>
        private float m_fDefaultTabWidth = DEF_AUTO_TAB_LENGHT;
        /// <summary>
        /// 
        /// </summary>
        private LineNumberingMode m_lineNumberingMode = LineNumberingMode.None;
        /// <summary>
        /// 
        /// </summary>
        private int m_lineNumberingMod = 1;
        /// <summary>
        /// 
        /// </summary>
        private int m_lineNumberingStartValue = 0;
        /// <summary>
        /// 
        /// </summary>
        private float m_distanceFromText = 0;
        /// <summary>
        /// 
        /// </summary>
        private PageBordersApplyType m_pageBorderApply = PageBordersApplyType.AllPages;
        /// <summary>
        /// 
        /// </summary>
        private PageBorderOffsetFrom m_pageBorderOffsetFrom = PageBorderOffsetFrom.Text;
        /// <summary>
        /// 
        /// </summary>
        private bool m_pageBorderIsInFront = true;
        /// <summary>
        /// 
        /// </summary>
        private Borders m_borders = new Borders();
        /// <summary>
        /// 
        /// </summary>
        private bool m_isBidi = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_equalColWidth;
        /// <summary>
        /// 
        /// </summary>
        private PageNumberStyle m_pageNumberStyle = 0;
        /// <summary>
        /// 
        /// </summary>
        private int m_pageNumberStartAt = 0;
        /// <summary>
        /// 
        /// </summary>
        private bool m_pageNumberRestart = false;
        /// <summary>
        /// 
        /// </summary>
        private float m_linePitch;
        /// <summary>
        /// 
        /// </summary>
        private GridPitchType m_pitchType;
        /// <summary>
        /// 
        /// </summary>
        private bool m_drawLinesBetwCols;
        /// <summary>
        /// 
        /// </summary>
        private bool m_hasLineNum;
        /// <summary>
        /// Destined when Page Setup is read
        /// </summary>
        internal bool m_isUpdated;
        /// <summary>
        /// 
        /// </summary>
        private PageNumbers m_pageNumbers;
        /// <summary>
        /// 
        /// </summary>
        private FootEndNoteNumberFormat m_endnoteNumberFormat = FootEndNoteNumberFormat.LowerCaseRoman;
        /// <summary>
        /// 
        /// </summary>
        private FootEndNoteNumberFormat m_footnoteNumberFormat = FootEndNoteNumberFormat.Arabic;
        /// <summary>
        /// 
        /// </summary>
        private EndnoteRestartIndex m_restartIndexForEndnote;
        /// <summary>
        /// 
        /// </summary>
        private EndnotePosition m_endnotePosition = EndnotePosition.DisplayEndOfDocument;
        /// <summary>
        /// 
        /// </summary>
        private FootnoteRestartIndex m_restartIndexForFootnotes;
        /// <summary>
        /// 
        /// </summary>
        private FootnotePosition m_footnotePosition = FootnotePosition.PrintAtBottomOfPage;
        /// <summary>
        /// 
        /// </summary>
        private int m_initialEndnoteNumber = 1;
        /// <summary>
        /// 
        /// </summary>
        private int m_initialFootnoteNumber = 1;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets / sets endnote numbering format
        /// </summary>
        internal FootEndNoteNumberFormat EndnoteNumberFormat
        {
            get
            {
                return (FootEndNoteNumberFormat)m_endnoteNumberFormat;
            }
            set
            {
                m_endnoteNumberFormat = value;
            }
        }

        /// <summary>
        /// Gets / sets footnote numbering format
        /// </summary>
        internal FootEndNoteNumberFormat FootnoteNumberFormat
        {
            get
            {
                return m_footnoteNumberFormat;
            }
            set
            {
                m_footnoteNumberFormat = value;
            }
        }

        /// <summary>
        /// Gets / sets the restart index for endnote
        /// </summary>
        internal EndnoteRestartIndex RestartIndexForEndnote
        {
            get
            {
                return (EndnoteRestartIndex)m_restartIndexForEndnote;
            }
            set
            {
                m_restartIndexForEndnote = value;
            }
        }

        /// <summary>
        /// Gets / sets the restart index for footnotes
        /// </summary>
        internal FootnoteRestartIndex RestartIndexForFootnotes
        {
            get
            {
                return (FootnoteRestartIndex)m_restartIndexForFootnotes;
            }
            set
            {
                m_restartIndexForFootnotes = value;
            }
        }

        /// <summary>
        /// Gets / sets footnote position in the document
        /// </summary>
        internal FootnotePosition FootnotePosition
        {
            get
            {
                return (FootnotePosition)m_footnotePosition;
            }
            set
            {
                m_footnotePosition = value;
            }
        }
        /// <summary>
        /// Gets / sets endnote position in the document
        /// </summary>
        internal EndnotePosition EndnotePosition
        {
            get
            {
                return m_endnotePosition;
            }
            set
            {
                m_endnotePosition = value;
            }
        }
        /// <summary>
        /// Gets / sets the initial footnote number
        /// </summary>
        internal int InitialFootnoteNumber
        {
            get
            {
                return m_initialFootnoteNumber;
            }
            set
            {
                m_initialFootnoteNumber = value;
            }
        }

        /// <summary>
        /// Gets / sets the initial endnote number
        /// </summary>
        internal int InitialEndnoteNumber
        {
            get
            {
                return m_initialEndnoteNumber;
            }
            set
            {
                m_initialEndnoteNumber = value;
            }
        }
        /// <summary>
        /// Gets or sets the length of the auto tab.
        /// </summary>
        /// <value>The length of the auto tab.</value>
        [Obsolete("This property has been deprecated. Use the DefaultTabWidth property of WordDocument class to set default tab width for the document.")]
        public float DefaultTabWidth
        {
            get
            {
                if (Document == null)
                    return 36f;
                else
                    return Document.DefaultTabWidth;
            }
            set
            {
                if (Document != null)
                    Document.DefaultTabWidth = value;                
            }
        }
        /// <summary>
        /// Gets /sets page size in points.
        /// </summary>
        public SizeF PageSize
        {
            get
            {
                return m_pageSize;
            }
            set
            {
                m_pageSize = value;
            }
        }
        /// <summary>
        /// Gets / sets orientation of a page.
        /// </summary>
        public PageOrientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                if (m_orientation != value)
                {
                    m_orientation = value;
                    switch (value)
                    {
                        case PageOrientation.Portrait:
                            if (PageSize.Width > PageSize.Height)
                            {
                                PageSize = new SizeF(PageSize.Height, PageSize.Width);
                            }
                            break;
                        case PageOrientation.Landscape:
                            if (PageSize.Height > PageSize.Width)
                            {
                                PageSize = new SizeF(PageSize.Height, PageSize.Width);
                            }
                            break;
                    }
                    //PageSize = new SizeF( PageSize.Height, PageSize.Width );
                }
            }
        }
        /// <summary>
        /// Gets / sets vertical alignment.
        /// </summary>
        /// <remarks>Not supported by Essential DPF</remarks>
        public PageAlignment VerticalAlignment
        {
            get
            {
                return m_vertAlignment;
            }
            set
            {
                m_vertAlignment = value;
            }
        }
        /// <summary>
        /// Gets / sets page margins in points.
        /// </summary>
        public MarginsF Margins
        {
            get
            {
                if (m_margins == null)
                {
                    m_margins = new MarginsF();
                }
                return m_margins;
            }
            set
            {
                m_margins = value;
            }
        }
        /// <summary>
        /// Gets / sets height of header in points.
        /// </summary>
        /// <remarks>Not supported by Essential DPF</remarks>
        public float HeaderDistance
        {
            get
            {
                return m_fHeaderDistance;
            }
            set
            {
                m_fHeaderDistance = value;
            }
        }
        /// <summary>
        /// Gets / sets footer height in points.
        /// </summary>
        /// <remarks>Not supported by Essential DPF</remarks>
        public float FooterDistance
        {
            get
            {
                return m_fFooterDistance;
            }
            set
            {
                m_fFooterDistance = value;
            }
        }
        /// <summary>
        /// Gets width of client area.
        /// </summary>
        public float ClientWidth
        {
            get
            {
                return (float)(PageSize.Width - Margins.Left - Margins.Right);
            }
        }
        /// <summary>
        /// Setting to specify that the current section has a different header/footer for first page.
        /// </summary>
        public bool DifferentFirstPage
        {
            get
            {
                return m_titlePage;
            }
            set
            {
                m_titlePage = value;
            }
        }
        /// <summary>
        /// True if the document has different headers and footers 
        /// for odd-numbered and even-numbered pages. 
        /// </summary>
        public bool DifferentOddAndEvenPages
        {
            get
            {
                if (Document == null)
                    return false;
                else
                    return Document.DifferentOddAndEvenPages;
            }
            set
            {
                if (Document != null)
                    Document.DifferentOddAndEvenPages = value;
            }
        }
        /// <summary>
        /// Gets / sets line numbering mode
        /// </summary>
        public LineNumberingMode LineNumberingMode
        {
            get
            {
                return m_lineNumberingMode;
            }
            set
            {
                m_lineNumberingMode = value;
            }
        }
        /// <summary>
        /// Gets / sets line numbering step
        /// </summary>
        public int LineNumberingStep
        {
            get
            {
                return m_lineNumberingMod;
            }
            set
            {
                m_lineNumberingMod = value;
            }
        }
        /// <summary>
        /// Gets / sets line numbering start value
        /// </summary>
        public int LineNumberingStartValue
        {
            get
            {
                return m_lineNumberingStartValue;
            }
            set
            {
                m_lineNumberingStartValue = value;
            }
        }
        /// <summary>
        /// Gets / sets distance from text in lines numbering
        /// </summary>
        public float LineNumberingDistanceFromText
        {
            get
            {
                return m_distanceFromText;
            }
            set
            {
                m_distanceFromText = value;
            }
        }
        /// <summary>
        /// Gets / sets the value that determine on which pages border is applied
        /// </summary>
        public PageBordersApplyType PageBordersApplyType
        {
            get
            {
                return m_pageBorderApply;
            }
            set
            {
                m_pageBorderApply = value;
            }
        }
        /// <summary>
        /// Gets / sets the position of page border
        /// </summary>
        public PageBorderOffsetFrom PageBorderOffsetFrom
        {
            get
            {
                return m_pageBorderOffsetFrom;
            }
            set
            {
                m_pageBorderOffsetFrom = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is front page border.
        /// </summary>
        /// <value>
        /// 	if this instance is front page border, set to <c>true</c>.
        /// </value>
        public bool IsFrontPageBorder
        {
            get
            {
                return m_pageBorderIsInFront;
            }
            set
            {
                m_pageBorderIsInFront = value;
            }
        }
        /// <summary>
        /// Gets page borders collection
        /// </summary>
        public Borders Borders
        {
            get
            {
                return m_borders;
            }
        }
        /// <summary>
        /// Gets / sets whether section contains right-to-left text. 
        /// </summary>
        public bool Bidi
        {
            get
            {
                return m_isBidi;
            }
            set
            {
                m_isBidi = value;
            }
        }
        /// <summary>
        /// Get/set Equal column width property.
        /// </summary>
        internal bool EqualColumnWidth
        {
            get
            {
                return m_equalColWidth;
            }
            set
            {
                m_equalColWidth = value;
            }
        }
        /// <summary>
        /// Gets or sets the page number style.
        /// </summary>
        /// <value>The page number style.</value>
        public PageNumberStyle PageNumberStyle
        {
            get
            {
                return m_pageNumberStyle;
            }
            set
            {
                m_pageNumberStyle = value;
            }
        }
        /// <summary>
        /// Gets or sets the page starting number.
        /// </summary>
        /// <value>The page starting number.</value>
        public int PageStartingNumber
        {
            get
            {
                return m_pageNumberStartAt;
            }
            set
            {
                m_pageNumberStartAt = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to restart page numbering.
        /// </summary>
        /// <value>
        /// 	if restart page numbering, set to <c>true</c>.
        /// </value>
        public bool RestartPageNumbering
        {
            get
            {
                return m_pageNumberRestart;
            }
            set
            {
                m_pageNumberRestart = value;
            }
        }
        /// <summary>
        /// Gets or sets the page line pitch.
        /// </summary>
        /// <value>The line pitch.</value>
        internal float LinePitch
        {
            get
            {
                return m_linePitch;
            }
            set
            {
                m_linePitch = value;
            }
        }
        /// <summary>
        /// Gets or sets the type of the pitch.
        /// </summary>
        /// <value>The type of the pitch.</value>
        internal GridPitchType PitchType
        {
            get
            {
                return m_pitchType;
            }
            set
            {
                m_pitchType = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to draw lines between columns.
        /// </summary>
        /// <value>
        /// 	 if draw lines between columns, set to <c>true</c>.
        /// </value>
        internal bool DrawLinesBetweenCols
        {
            get
            {
                return m_drawLinesBetwCols;
            }
            set
            {
                m_drawLinesBetwCols = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has line numbering.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has line numbering; otherwise, <c>false</c>.
        /// </value>
        internal bool HasLineNumbering
        {
            get
            {
                return m_hasLineNum;
            }
            set
            {
                m_hasLineNum = true;
            }
        }
        /// <summary>
        /// Gets the page number setup.
        /// </summary>        
        public PageNumbers PageNumbers
        {
            get
            {
                if (m_pageNumbers == null)
                    m_pageNumbers = new PageNumbers();
                return m_pageNumbers;
            }

        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WPageSetup"/> class.
        /// </summary>
        /// <param name="sec">The sec.</param>
        internal WPageSetup(WSection sec)
            : base(sec.Document, sec)
        {
            m_pageSize = new SizeF(DEF_PAGE_WIDTH, DEF_PAGE_HEIGHT);
            m_margins = new MarginsF();
            m_margins.All = DEF_PAGE_MARGINS;
            m_margins.Left = DEF_PAGE_MARGIN_LEFT;
            m_fFooterDistance = m_fHeaderDistance = -0.05f;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Inserts the page numbers.
        /// </summary>
        /// <param name="topOfPage">if it specifies the top of page, set to <c>true</c>.</param>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        public void InsertPageNumbers(bool topOfPage, PageNumberAlignment horizontalAlignment)
        {
            HeaderFooter hf = topOfPage ?
              (OwnerBase as WSection).HeadersFooters.Header :
              (OwnerBase as WSection).HeadersFooters.Footer;

            IWParagraph pnPara = null;
            IWField pnField = null;

            // Search existing page field
            for (int i = 0, len = hf.Paragraphs.Count; i < len; i++)
            {
                pnPara = hf.Paragraphs[i];

                for (int j = 0, lenj = pnPara.Items.Count; j < lenj; j++)
                {
                    if (pnPara.Items[j].EntityType == EntityType.Field)
                    {
                        WField field = (WField)pnPara.Items[j];

                        if (field.FieldType == FieldType.FieldPage)
                        {
                            pnField = field;
                            break;
                        }
                    }
                }
            }

            if (pnField == null)
            {
                pnPara = hf.AddParagraph();
                pnField = pnPara.AppendField("", FieldType.FieldPage);
            }

            pnPara.ParagraphFormat.WrapFrameAround = FrameWrapMode.Around;
            pnPara.ParagraphFormat.FrameX = (short)horizontalAlignment;
            pnPara.ParagraphFormat.FrameVerticalPos = (byte)FrameVertAnchor.Text;
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Get PageNumber/FootnoteID based on NumberFormat
        internal string GetNumberFormatValue(byte numberFormat, int number)
        {
            string text = number.ToString();
            switch (numberFormat)
            {
                case 1:
                    text = GetAsRoman(number).ToUpper();
                    break;
                case 2:
                    text = GetAsRoman(number).ToLower();
                    break;
                case 3:
                    text = GetAsLetter(number).ToUpper();
                    break;
                case 4:
                    text = GetAsLetter(number).ToLower();
                    break;
                default:
                    text = number.ToString();
                    break;
            }
            return text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private string GetAsRoman(int number)
        {
            StringBuilder retval = new StringBuilder();
            retval.Append(GenerateNumber(ref number, 1000, "M"));
            retval.Append(GenerateNumber(ref number, 900, "CM"));
            retval.Append(GenerateNumber(ref number, 500, "D"));
            retval.Append(GenerateNumber(ref number, 400, "CD"));
            retval.Append(GenerateNumber(ref number, 100, "C"));
            retval.Append(GenerateNumber(ref number, 90, "XC"));
            retval.Append(GenerateNumber(ref number, 50, "L"));
            retval.Append(GenerateNumber(ref number, 40, "XL"));
            retval.Append(GenerateNumber(ref number, 10, "X"));
            retval.Append(GenerateNumber(ref number, 9, "IX"));
            retval.Append(GenerateNumber(ref number, 5, "V"));
            retval.Append(GenerateNumber(ref number, 4, "IV"));
            retval.Append(GenerateNumber(ref number, 1, "I"));
            return retval.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private string GetAsLetter(int number)
        {
            Stack<int> stack = ConvertToLetter(number);
            StringBuilder result = new StringBuilder();
            while (stack.Count > 0)
            {
                int num = stack.Pop();
                AppendChar(result, num);
            }
            return result.ToString();
        }
        /// <summary>
        /// Adds letter instead of number.
        /// </summary>
        /// <param name="builder">String builder object.</param>
        /// <param name="number">Number to be converted to letter.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        private static void AppendChar(StringBuilder builder, int number)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (number <= 0 || number > 26)
                throw new ArgumentOutOfRangeException("number", "Value can not be less 0 and greater 26");
            char letter = (char)(DEF_A_ASCII_INDEX + number);
            builder.Append(letter);
        }
        /// <summary>
        /// Utility metnod. Helps to convert arabic number to \"A\" format.
        /// </summary>
        /// <param name="arabic">Arabic number.</param>
        /// <returns>Sequence of number.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private static Stack<int> ConvertToLetter(float arabic)
        {
            if (arabic < 0)
#if SILVERLIGHT || WP
        throw new ArgumentOutOfRangeException( "arabic", "Value can not be less 0" );
#else
                throw new ArgumentOutOfRangeException("arabic", arabic, "Value can not be less 0");
#endif

            Stack<int> stack = new Stack<int>();


            while (((int)arabic) > DEF_AR_TO_LETTER_LIMIT)
            {
                float remainder = arabic % DEF_AR_TO_LETTER_LIMIT;

                if (remainder == 0.0f)
                {
                    arabic = arabic / DEF_AR_TO_LETTER_LIMIT - 1f;
                    remainder = DEF_AR_TO_LETTER_LIMIT;
                }
                else
                {
                    arabic /= DEF_AR_TO_LETTER_LIMIT;
                }

                stack.Push((int)remainder);
            }

            if (arabic > 0f)
            {
                stack.Push((int)arabic);
            }

            return stack;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="magnitude"></param>
        /// <param name="letter"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private string GenerateNumber(ref int value, int magnitude, string letter)
        {
            StringBuilder numberstring = new StringBuilder();

            while (value >= magnitude)
            {
                value -= magnitude;
                numberstring.Append(letter);
            }

            return numberstring.ToString();
        }
#endregion
#endif

        #region Class overrides
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (DefaultTabWidth != DEF_AUTO_TAB_LENGHT)
            {
                writer.WriteValue(XDLSConstants.AutoTabWidthAttr, DefaultTabWidth);
            }
            if (PageSize.Height != 0)
            {
                writer.WriteValue(XDLSConstants.PSPageHeightAttr, PageSize.Height);
            }
            if (PageSize.Width != 0)
            {
                writer.WriteValue(XDLSConstants.PSPageWidthAttr, PageSize.Width);
            }
            if (VerticalAlignment != 0)
            {
                writer.WriteValue(XDLSConstants.PSAlignmentAttr, VerticalAlignment);
            }
            if (FooterDistance >= 0)
            {
                writer.WriteValue(XDLSConstants.PSFooterDistanceAttr, FooterDistance);
            }
            if (HeaderDistance >= 0)
            {
                writer.WriteValue(XDLSConstants.PSHeaderDistanceAttr, HeaderDistance);
            }
            if (Orientation != 0)
            {
                writer.WriteValue(XDLSConstants.PSOrientationAttr, Orientation);
            }
            if (Margins.Bottom >= 0)
            {
                writer.WriteValue(XDLSConstants.PSBottomMarginAttr, Margins.Bottom);
            }
            if (Margins.Top >= 0)
            {
                writer.WriteValue(XDLSConstants.PSTopMarginAttr, Margins.Top);
            }
            if (Margins.Left >= 0)
            {
                writer.WriteValue(XDLSConstants.PSLeftMarginAttr, Margins.Left);
            }
            if (Margins.Right >= 0)
            {
                writer.WriteValue(XDLSConstants.PSRightMarginAttr, Margins.Right);
            }

            if (DifferentFirstPage)
            {
                writer.WriteValue(XDLSConstants.PageSetupFirstPageAttr, DifferentFirstPage);
            }

            if (DifferentOddAndEvenPages)
            {
                writer.WriteValue(XDLSConstants.PageSetupDiffOddEvenPagesAttr, DifferentOddAndEvenPages);
            }

            if (LineNumberingMode != LineNumberingMode.None)
            {
                writer.WriteValue(XDLSConstants.PageSetupLineNumModeAttr, LineNumberingMode);

                if (LineNumberingStep != 0)
                {
                    writer.WriteValue(XDLSConstants.PageSetupLineNumStepAttr, LineNumberingStep);
                }
                if (LineNumberingDistanceFromText != 0)
                {
                    writer.WriteValue(XDLSConstants.PageSetupLineNumDistanceAttr, LineNumberingDistanceFromText);
                }

                writer.WriteValue(XDLSConstants.PageSetupLineNumStartValueAttr, LineNumberingStartValue);
            }

            if (PageBordersApplyType != DocIO.PageBordersApplyType.AllPages)
            {
                writer.WriteValue(XDLSConstants.PageSetupBorderApplyAttr, PageBordersApplyType);
            }
            if (!IsFrontPageBorder)
            {
                writer.WriteValue(XDLSConstants.PageSetupBorderIsInFrontAttr, IsFrontPageBorder);
            }
            if (PageBorderOffsetFrom != DocIO.PageBorderOffsetFrom.Text)
            {
                writer.WriteValue(XDLSConstants.PageSetupBorderOffsetFromAttr, PageBorderOffsetFrom);
            }
            if (m_equalColWidth)
            {
                writer.WriteValue(XDLSConstants.PageSetupColumnEqualAttr, m_equalColWidth);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.AutoTabWidthAttr))
            {
                DefaultTabWidth = reader.ReadFloat(XDLSConstants.AutoTabWidthAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PSPageHeightAttr))
            {
                PageSize = new SizeF(PageSize.Width, reader.ReadFloat(XDLSConstants.PSPageHeightAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PSPageWidthAttr))
            {
                PageSize = new SizeF(reader.ReadFloat(XDLSConstants.PSPageWidthAttr), PageSize.Height);
            }
            if (reader.HasAttribute(XDLSConstants.PSAlignmentAttr))
            {
                VerticalAlignment = (PageAlignment)reader.ReadEnum(XDLSConstants.PSAlignmentAttr, typeof(PageAlignment));
            }
            if (reader.HasAttribute(XDLSConstants.PSFooterDistanceAttr))
            {
                FooterDistance = reader.ReadFloat(XDLSConstants.PSFooterDistanceAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PSHeaderDistanceAttr))
            {
                HeaderDistance = reader.ReadFloat(XDLSConstants.PSHeaderDistanceAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PSOrientationAttr))
            {
                Orientation = (PageOrientation)reader.ReadEnum(XDLSConstants.PSOrientationAttr, typeof(PageOrientation));
            }
            if (reader.HasAttribute(XDLSConstants.PSBottomMarginAttr))
            {
                Margins.Bottom = reader.ReadFloat(XDLSConstants.PSBottomMarginAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PSTopMarginAttr))
            {
                Margins.Top = reader.ReadFloat(XDLSConstants.PSTopMarginAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PSLeftMarginAttr))
            {
                Margins.Left = reader.ReadFloat(XDLSConstants.PSLeftMarginAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PSRightMarginAttr))
            {
                Margins.Right = reader.ReadFloat(XDLSConstants.PSRightMarginAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PageSetupFirstPageAttr))
            {
                DifferentFirstPage = reader.ReadBoolean(XDLSConstants.PageSetupFirstPageAttr);
            }

            if (reader.HasAttribute(XDLSConstants.PageSetupDiffOddEvenPagesAttr))
            {
                DifferentOddAndEvenPages = reader.ReadBoolean(XDLSConstants.PageSetupDiffOddEvenPagesAttr);
            }

            if (reader.HasAttribute(XDLSConstants.PageSetupLineNumStepAttr))
            {
                LineNumberingStep = reader.ReadInt(XDLSConstants.PageSetupLineNumStepAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PageSetupLineNumDistanceAttr))
            {
                LineNumberingDistanceFromText = reader.ReadFloat(XDLSConstants.PageSetupLineNumDistanceAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PageSetupLineNumModeAttr))
            {
                LineNumberingMode = (LineNumberingMode)reader.ReadEnum(XDLSConstants.PageSetupLineNumModeAttr, typeof(LineNumberingMode));
            }
            if (reader.HasAttribute(XDLSConstants.PageSetupLineNumStartValueAttr))
            {
                LineNumberingStartValue = reader.ReadInt(XDLSConstants.PageSetupLineNumStartValueAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PageSetupBorderApplyAttr))
            {
                PageBordersApplyType = (PageBordersApplyType)reader.ReadEnum(XDLSConstants.PageSetupBorderApplyAttr, typeof(PageBordersApplyType));
            }
            if (reader.HasAttribute(XDLSConstants.PageSetupBorderIsInFrontAttr))
            {
                IsFrontPageBorder = reader.ReadBoolean(XDLSConstants.PageSetupBorderIsInFrontAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PageSetupBorderOffsetFromAttr))
            {
                PageBorderOffsetFrom = (PageBorderOffsetFrom)reader.ReadEnum(XDLSConstants.PageSetupBorderOffsetFromAttr, typeof(PageBorderOffsetFrom));
            }
            if (reader.HasAttribute(XDLSConstants.PageSetupColumnEqualAttr))
            {
                m_equalColWidth = reader.ReadBoolean(XDLSConstants.PageSetupColumnEqualAttr);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();

            XDLSHolder.AddElement(XDLSConstants.BordersItemTag, Borders);
        }

        public override string ToString()
        {
#if DEBUG
          string str = Environment.NewLine
              + "Width : " + PageSize.Width.ToString()
              + Environment.NewLine
              + "Height : " + PageSize.Height.ToString()
              + Environment.NewLine
              + "Orientation : " + Orientation.ToString()
              + Environment.NewLine
              + "ClientWidth : " + ClientWidth.ToString();
          return str;
#else
            return base.ToString();
#endif
        }
//#endif
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns></returns>
        internal WPageSetup Clone()
        {
          WPageSetup pSett = ( WPageSetup )base.CloneImpl();
          pSett.m_borders = Borders.Clone();
          return pSett;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Clone page size
        /// </summary>
        /// <returns>PageSize</returns>
        internal SizeF ClonePageSize()
        {
            return new SizeF(PageSize);
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for PageNumbers.
    /// </summary>
    public class PageNumbers
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ChapterPageSeparatorType m_chapterPageSeparator = ChapterPageSeparatorType.Hyphen;
        /// <summary>
        /// 
        /// </summary>
        private HeadingLevel m_headingLevelForChapter = HeadingLevel.None;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating  [Chapter numbering seprator in page number].
        /// </summary>
        /// <value>
        /// Chapter Page Separator 
        /// </value>
        public ChapterPageSeparatorType ChapterPageSeparator
        {
            get
            {
                return m_chapterPageSeparator;
            }
            set
            {
                m_chapterPageSeparator = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating  [Chapter numbering level in page number].
        /// </summary>
        /// <value>
        /// Chapter Heading Level
        /// </value>
        public HeadingLevel HeadingLevelForChapter
        {
            get
            {
                return m_headingLevelForChapter;
            }
            set
            {
                m_headingLevelForChapter = value;
            }
        }
        #endregion

        #region Constructor
        public PageNumbers()
        {
            m_chapterPageSeparator = ChapterPageSeparatorType.Hyphen;
            m_headingLevelForChapter = HeadingLevel.None;
        }
        #endregion
    }

}
