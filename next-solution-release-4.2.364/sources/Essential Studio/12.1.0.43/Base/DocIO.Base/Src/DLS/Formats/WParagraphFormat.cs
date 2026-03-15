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
using System.Collections.Generic;
using System.Collections.Specialized;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.Documentation;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// WParagraphFormat is used for representation of paragraph properties. 
    /// </summary>
    [DocumentationExclude()]
    public class WParagraphFormat : FormatBase
    {
        #region Class constants
        internal const short HrAlignmentKey = 0;
        internal const short LeftIndentKey = 2;
        internal const short LeftIndentBiKey = 68;
        internal const short RightIndentKey = 3;
        internal const short RightIndentBiKey = 69;
        internal const short FirstLineIndentKey = 5;
        internal const short FirstLineIndentBiKey = 70;
        internal const short KeepKey = 6;
        internal const short BeforeSpacingKey = 8;
        internal const short AfterSpacingKey = 9;
        internal const short KeepFollowKey = 10;
        internal const short WidowControlKey = 11;
        internal const short PageBreakBeforeKey = 12;
        internal const short PageBreakAfterKey = 13;
        internal const short BordersKey = 20;
        internal const short BackColorKey = 21;
        internal const short ColumnBreakAfterKey = 22;
        internal const short TabsKey = 30;
        internal const short BidiKey = 31;
        internal const short ForeColorKey = 32;
        internal const short TextureStyleKey = 33;
        internal const short DataKey = 50;
        internal const short AdjustRightIndentKey = 80;
        internal const short AutoSpaceDEKey = 81;
        internal const short AutoSpaceDNKey = 82;
        /// <summary>
        /// 
        /// </summary>    
        internal const short LineSpacingKey = 52;
        internal const short LineSpacingRuleKey = 53;
        internal const short SpacingBeforeAutoKey = 54;
        internal const short SpacingAfterAutoKey = 55;
        internal const short OutlineLevelKey = 56;

        internal const short LeftBorderKey = 57;
        internal const short RightBorderKey = 58;
        internal const short TopBorderKey = 59;
        internal const short BottomBorderKey = 60;
        internal const short LeftBorderNewKey = 61;
        internal const short RightBorderNewKey = 62;
        internal const short TopBorderNewKey = 63;
        internal const short BottomBorderNewKey = 64;
        internal const short ChangedFormatKey = 65;
        internal const short BetweenBorderKey = 66;
        internal const short BarBorderKey = 67;
        internal const short ContextualSpacingKey = 71;
        internal const short FramePosKey = 72;
        internal const short FrameXKey = 73;
        internal const short FrameYKey = 74;
        internal const short FrameWidthKey = 76;
        internal const short FrameHeightKey = 77;
        internal const short FrameHorizontalDistanceFromTextKey = 83;
        internal const short FrameVerticalDistanceFromTextKey = 84;
        internal const short WrapFrameAroundKey = 88;
        internal const short SuppressAutoHyphensKey = 78;
        // Docx specific property key (both Word-2007 and Word-2010)
        internal const short MirrorIndentsKey = 75;
        //Paragraph indentations in character units.
        internal const int LeftIndentCharsKey = 85;
        internal const int FirstLineIndentCharsKey = 86;
        internal const int RightIndentCharsKey = 87;
        internal const int WordWrapKey = 89;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ParagraphProperties m_paraProps;
        private WParagraphFormat m_tableStyleParagraphFormat;
        private bool m_cancelOnChange;
        private bool m_hasClonedProps;
        internal WAbsoluteTab m_absoluteTab;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether to break lines on word or character level. By default line breaks on word level.
        /// </summary>
        /// <value>
        ///   <c>true</c> if line breaks on word level; otherwise, <c>false</c>.
        /// </value>
        internal bool WordWrap
        {
            get
            {
                return (bool)GetPropertyValue(WordWrapKey);
            }
            set
            {
                SetPropertyValue(WordWrapKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the value that represents the Absolute Tab of paragraph.
        /// </summary>
        internal WAbsoluteTab AbsoluteTab
        {
            get
            {
                return m_absoluteTab;
            }
            set
            {
                m_absoluteTab = value;
            }
        }
        /// <summary>
        /// Gets / sets the value that represents the paragraph first line indent in character units.
        /// </summary>
        internal float FirstLineIndentChars
        {
            get
            {
                return (float)GetPropertyValue(FirstLineIndentCharsKey);
            }
            set
            {
                SetPropertyValue(FirstLineIndentCharsKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the value that represents the paragraph left indent in character units.
        /// </summary>
        internal float LeftIndentChars
        {
            get
            {
                return (float)GetPropertyValue(LeftIndentCharsKey);
            }
            set
            {
                SetPropertyValue(LeftIndentCharsKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the value that represents the paragraph right indent in character units.
        /// </summary>
        internal float RightIndentChars
        {
            get
            {
                return (float)GetPropertyValue(RightIndentCharsKey);
            }
            set
            {
                SetPropertyValue(RightIndentCharsKey, value);
            }
        }
        /// <summary>
        /// Gets / sets right-to-left property of the paragraph.
        /// </summary>
        /// <value>if bidi, set to <c>true</c>.</value>
        public bool Bidi
        {
            get
            {
                return (bool)GetPropertyValue(BidiKey);
                //        return ( bool )this[ BidiKey ];
            }
            set
            {
                //bool existingValue = (bool)GetPropertyValue(BidiKey);
                SetPropertyValue(BidiKey, value);
                //For existing RTL paragraph, should not update the justification.
                //if (!existingValue)
                //   UpdateParaProps(HrAlignmentKey, HorizontalAlignment);
            }
        }
        /// <summary>
        /// Gets the tabs info.
        /// </summary>
        /// <value>The tabs info.</value>
        public TabCollection Tabs
        {
            get
            {
                if (!this.HasValue(TabsKey))
                    CreateTabsCol();

                return (TabCollection)GetPropertyValue(TabsKey);
            }
        }
        /// <summary>
        /// True if all lines in the paragraph are to remain on the same page. 
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public bool Keep
        {
            get
            {
                return (bool)GetPropertyValue(KeepKey);
            }
            set
            {
                SetPropertyValue(KeepKey, value);
            }
        }
        /// <summary>
        /// True if the paragraph is to remains on the same page as the 
        /// paragraph that follows it. 
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public bool KeepFollow
        {
            get
            {
                return (bool)GetPropertyValue(KeepFollowKey);
            }
            set
            {
                SetPropertyValue(KeepFollowKey, value);
            }
        }
        /// <summary>
        /// True if a page break is forced before the paragraph
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public bool PageBreakBefore
        {
            get
            {
                return (bool)GetPropertyValue(PageBreakBeforeKey);
            }
            set
            {
                SetPropertyValue(PageBreakBeforeKey, value);
            }
        }
        /// <summary>
        /// True if a page break is forced after the paragraph
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public bool PageBreakAfter
        {
            get
            {
                if (this[PageBreakAfterKey] == null)
                {
                    return false;
                }
                return (bool)this[PageBreakAfterKey];
            }
            set
            {
                this[PageBreakAfterKey] = value;
            }
        }
        /// <summary>
        /// True if the first and last lines in the paragraph 
        /// are to remain on the same page as the rest of the paragraph. 
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public bool WidowControl
        {
            get
            {
                return (bool)GetPropertyValue(WidowControlKey);
            }
            set
            {
                SetPropertyValue(WidowControlKey, value);
            }
        }
        /// <summary>
        /// True, if space is automatically inserted between East Asian text and numbers. 
        /// </summary>
        internal bool AutoSpaceDN
        {
            get
            {
                return (bool)GetPropertyValue(AutoSpaceDNKey);
            }
            set
            {
                SetPropertyValue(AutoSpaceDNKey, value);
            }
        }
        /// <summary>
        /// True, if space is automatically inserted between East Asian text and Latin text. 
        /// </summary>
        internal bool AutoSpaceDE
        {
            get
            {
                return (bool)GetPropertyValue(AutoSpaceDEKey);
            }
            set
            {
                SetPropertyValue(AutoSpaceDEKey, value);
            }
        }
        /// <summary>
        /// True, if the paragraph is set to automatically adjust the right indent
        /// when a document grid for East Asian characters is defined. 
        /// </summary>
        internal bool AdjustRightIndent
        {
            get
            {
                return (bool)GetPropertyValue(AdjustRightIndentKey);
            }
            set
            {
                SetPropertyValue(AdjustRightIndentKey, value);
            }
        }
        /// <summary>
        /// Gets / sets horizontal alignment for the paragraph. 
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetPropertyValue(HrAlignmentKey);
            }
            set
            {
                SetPropertyValue(HrAlignmentKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the value that represents the left indent for paragraph. 
        /// </summary>
        public float LeftIndent
        {
            get
            {
                return (float)GetPropertyValue(LeftIndentKey);
            }
            set
            {
                SetPropertyValue(LeftIndentKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the value that represents the left indent bi for paragraph.
        /// </summary>
        internal float LeftIndentBi
        {
            get
            {
                return (float)GetPropertyValue(LeftIndentBiKey);
            }
            set
            {
                SetPropertyValue(LeftIndentBiKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the value that represents the right indent for paragraph.
        /// </summary>
        public float RightIndent
        {
            get
            {
                return (float)GetPropertyValue(RightIndentKey);
            }
            set
            {
                SetPropertyValue(RightIndentKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the value that represents the right indent bi for paragraph.
        /// </summary>
        internal float RightIndentBi
        {
            get
            {
                return (float)GetPropertyValue(RightIndentBiKey);
            }
            set
            {
                SetPropertyValue(RightIndentBiKey, value);
            }
        }
        /// <summary>
        /// Get / set first paragraph line indent
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public float FirstLineIndent
        {
            get
            {
                return (float)GetPropertyValue(FirstLineIndentKey);
            }
            set
            {
                SetPropertyValue(FirstLineIndentKey, value);
            }
        }
        /// <summary>
        /// Get / set first paragraph line indent bi
        /// </summary>
        internal float FirstLineIndentBi
        {
            get
            {
                return (float)GetPropertyValue(FirstLineIndentBiKey);
            }
            set
            {
                SetPropertyValue(FirstLineIndentBiKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the spacing (in points) before the paragraph. 
        /// </summary>
        public float BeforeSpacing
        {
            get
            {
                return (float)GetPropertyValue(BeforeSpacingKey);
            }
            set
            {
                SetPropertyValue(BeforeSpacingKey, value);
            }
        }
        /// <summary>
        /// Gets / sets the spacing (in points) after the paragraph.
        /// </summary>
        public float AfterSpacing
        {
            get
            {
                return (float)GetPropertyValue(AfterSpacingKey);
            }
            set
            {
                SetPropertyValue(AfterSpacingKey, value);
            }
        }
        /// <summary>
        /// Gets collection of borders in the paragraph
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public Borders Borders
        {
            get
            {
                return GetPropertyValue(BordersKey) as Borders;
            }
        }
        /// <summary>
        /// Gets/sets background color of the paragraph 
        /// </summary>
        public Color BackColor
        {
            get
            {
                return (Color)GetPropertyValue(BackColorKey);
            }
            set
            {
                SetPropertyValue(BackColorKey, value);
            }
        }
        /// <summary>
        /// True if a column break is forced after the paragraph
        /// </summary>
        /// <remarks>Not supported by Essential PDF</remarks>
        public bool ColumnBreakAfter
        {
            get
            {
                if (this[ColumnBreakAfterKey] == null)
                {
                    return false;
                }
                return (bool)this[ColumnBreakAfterKey];
            }
            set
            {
                this[ColumnBreakAfterKey] = value;
            }
        }
        /// <summary>
        /// Gets or sets the SPRMS.
        /// </summary>
        /// <value>The SPRMS.</value>
        internal SinglePropertyModifierArray Sprms
        {
            get
            {
                return m_sprms;
            }
            set
            {
                m_paraProps = null;
                m_sprms = value;
            }
        }
        /// <summary>
        /// Gets / sets line spacing property of the paragraph.
        /// </summary>
        public float LineSpacing
        {
            get
            {
                return (float)GetPropertyValue(LineSpacingKey);
            }
            set
            {
                SetPropertyValue(LineSpacingKey, value);
            }
        }
        /// <summary>
        /// Gets / sets line spacing rule property of the paragraph.
        /// </summary>
        public LineSpacingRule LineSpacingRule
        {
            get
            {
                return (LineSpacingRule)GetPropertyValue(LineSpacingRuleKey);
            }
            set
            {
                SetPropertyValue(LineSpacingRuleKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Color ForeColor
        {
            get
            {
                return (Color)GetPropertyValue(ForeColorKey);
            }
            set
            {
                SetPropertyValue(ForeColorKey, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TextureStyle TextureStyle
        {
            get
            {
                return (TextureStyle)GetPropertyValue(TextureStyleKey);
            }
            set
            {
                   SetPropertyValue(TextureStyleKey , value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ParagraphProperties ParaProps
        {
            get
            {
                CheckParaProps();
                return m_paraProps;
            }
        }
        /// <summary>
        /// Gets a value indicating whether spacing before is automatic.
        /// </summary>
        /// <value>if spacing before is automatic, set to <c>true</c>.</value>
        public bool SpaceBeforeAuto
        {
            get
            {
                return (bool)GetPropertyValue(SpacingBeforeAutoKey);
            }
            set
            {
                SetPropertyValue(SpacingBeforeAutoKey, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether spacing after is automatic.
        /// </summary>
        /// <value>if spacing after is automatic, set to <c>true</c>.</value>
        public bool SpaceAfterAuto
        {
            get
            {
                return (bool)GetPropertyValue(SpacingAfterAutoKey);
            }
            set
            {
                SetPropertyValue(SpacingAfterAutoKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the outline level.
        /// </summary>
        /// <value>The outline level.</value>
        public OutlineLevel OutlineLevel
        {
            get
            {
                byte value = (byte)GetPropertyValue(OutlineLevelKey);
                if (value >= 0 && value <= 9)
                    return (OutlineLevel)Enum.ToObject(typeof(OutlineLevel), value);
                else
                    return OutlineLevel.BodyText;
            }
            set
            {
                //if( value > 9 || value < 0 )
                //  throw new ArgumentOutOfRangeException( "Outline level must be not less than 0 and not larger than 8" );
                SetPropertyValue(OutlineLevelKey, (byte)value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is frame.
        /// </summary>
        /// <value> if this instance is frame, set to <c>true</c>.</value>
        internal bool IsFrame
        {
            get
            {
                if (FrameWidth != 0 || FrameHeight != 0 || FrameX != 0 || FrameY != 0
                || FrameHorizontalPos != (byte)FrameHorzAnchor.Text || (FrameWrapMode)WrapFrameAround != FrameWrapMode.Auto)
                    return true;
                else if (HasValue(FramePosKey) && FrameVerticalPos == (byte)FrameVertAnchor.Text)
                    return true;
				else
	                return false;
            }
        }
        /// <summary>
        /// Gets or sets the frame vertical pos.
        /// </summary>
        /// <value>The frame vertical pos.</value>
        internal byte FrameVerticalPos
        {
            get
            {
                CheckParaProps();
                if (!HasValue(FrameYKey))
                    return (byte)FrameVertAnchor.Text;
                else
                    return m_paraProps.FrameVerticalPos;
            }
            set
            {
                CheckParaProps();
                m_paraProps.FrameVerticalPos = value;
            }
        }
        /// <summary>
        /// Gets or sets the frame horizontal pos.
        /// </summary>
        /// <value>The frame horizontal pos.</value>
        internal byte FrameHorizontalPos
        {
            get
            {
                CheckParaProps();
                return m_paraProps.FrameHorizontalPos;
            }
            set
            {
                CheckParaProps();
                m_paraProps.FrameHorizontalPos = value;
            }
        }
        /// <summary>
        /// Gets or sets the frame X.
        /// </summary>
        /// <value>The frame X.</value>
        internal float FrameX
        {
            get
            {
                return (float)GetPropertyValue(FrameXKey);
            }
            set
            {
                SetPropertyValue(FrameXKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the frame Y.
        /// </summary>
        /// <value>The frame Y.</value>
        internal float FrameY
        {
            get
            {
                return (float)GetPropertyValue(FrameYKey);
            }
            set
            {
                SetPropertyValue(FrameYKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the frame width
        /// </summary>
        /// <value>The frame Width.</value>
        internal float FrameWidth
        {
            get
            {
                return (float)GetPropertyValue(FrameWidthKey);
            }
            set
            {
                SetPropertyValue(FrameWidthKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the frame Height
        /// </summary>
        /// <value>The frame Height.</value>
        internal float FrameHeight
        {
            get
            {
                return (float)GetPropertyValue(FrameHeightKey);
            }
            set
            {
                SetPropertyValue(FrameHeightKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the frame horizontal distance from text.
        /// </summary>
        /// <value>The frame horizontal distance from text.</value>
        internal float FrameHorizontalDistanceFromText
        {
            get
            {
                return (float)GetPropertyValue(FrameHorizontalDistanceFromTextKey);
            }
            set
            {
                SetPropertyValue(FrameHorizontalDistanceFromTextKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the frame vertical distance from text.
        /// </summary>
        /// <value>The frame vertical distance from text.</value>
        internal float FrameVerticalDistanceFromText
        {
            get
            {
                return (float)GetPropertyValue(FrameVerticalDistanceFromTextKey);
            }
            set
            {
                SetPropertyValue(FrameVerticalDistanceFromTextKey, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to wrap frame around.
        /// </summary>
        /// <value>The frame wrap mode</value>
        internal FrameWrapMode WrapFrameAround
        {
            get
            {
                return (FrameWrapMode)GetPropertyValue(WrapFrameAroundKey);
            }
            set
            {
                SetPropertyValue(WrapFrameAroundKey, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has reference on list.
        /// </summary>
        /// <value>
        /// 	 if this instance has list reference, set to <c>true</c>.
        /// </value>
        internal bool HasListReference
        {
            get
            {
                if (m_sprms != null)
                {
                    return (m_sprms[WordSprmOptions.sprmPIlfo] == null) ? false : true;
                }
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsChangedFormat
        {
            get
            {
                return (bool)GetPropertyValue(ChangedFormatKey);
            }
            set
            {
                if (value)
                    SetPropertyValue(ChangedFormatKey, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [contextual spacing].
        /// </summary>
        /// <value>if it specifies contextual spacing, set to <c>true</c>.</value>
        public bool ContextualSpacing
        {
            get
            {
                return (bool)GetPropertyValue(ContextualSpacingKey);
            }
            set
            {
                SetPropertyValue(ContextualSpacingKey, value);
            }
        }
        /// <summary>
        /// Gets or sets the table style paragraph format.
        /// </summary>
        /// <value>The table style paragraph format.</value>
        internal WParagraphFormat TableStyleParagraphFormat
        {
            get
            {
                return m_tableStyleParagraphFormat;
            }
            set
            {
                m_tableStyleParagraphFormat = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether indentation type is mirror indents.
        /// </summary>
        /// <value><c>true</c> if use mirror indents; otherwise, <c>false</c>.</value>
        public bool MirrorIndents
        {
            get
            {
                return (bool)GetPropertyValue(MirrorIndentsKey);
            }
            set
            {
                SetPropertyValue(MirrorIndentsKey, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to suppress automatic hyphenation for the paragraph.
        /// </summary>
        /// <value><c>true</c> if suppress auto hyphens; otherwise, <c>false</c>.</value>
        public bool SuppressAutoHyphens
        {
            get
            {
                return (bool)GetPropertyValue(SuppressAutoHyphensKey);
            }
            set
            {
                SetPropertyValue(SuppressAutoHyphensKey, value);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WParagraphFormat"/> class.
        /// </summary>
        public WParagraphFormat()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="WParagraphFormat"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public WParagraphFormat(IWordDocument document)
            : base((WordDocument)document)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Determine whether the paragraph format is have frame properties
        /// </summary>
        /// <returns></returns>
        internal bool IsInFrame()
        {
            if (m_sprms != null && m_sprms.Count > 0)
            {
                return (m_sprms.Contain(WordSprmOptions.sprmPPc) ||
                m_sprms.Contain(WordSprmOptions.sprmPWr) ||
                m_sprms.Contain(WordSprmOptions.sprmPDxaAbs) ||
                m_sprms.Contain(WordSprmOptions.sprmPDyaAbs) ||
                m_sprms.Contain(WordSprmOptions.sprmPWHeightAbs) ||
                m_sprms.Contain(WordSprmOptions.sprmPDxaWidth) ||
                m_sprms.Contain(WordSprmOptions.sprmPDxaFromText) ||
                m_sprms.Contain(WordSprmOptions.sprmPDyaFromText) ||
                m_sprms.Contain(WordSprmOptions.sprmPDxaFromText10));
            }
            return false;
        }
        /// <summary>
        /// Determine whether the frame is have Horizontal alignment
        /// </summary>
        /// <returns></returns>
        internal bool IsFrameXAlign(short xPosition)
        {
            return (xPosition == (short)PageNumberAlignment.Left
                    || xPosition == (short)PageNumberAlignment.Center
                    || xPosition == (short)PageNumberAlignment.Right
                    || xPosition == (short)PageNumberAlignment.Inside
                    || xPosition == (short)PageNumberAlignment.Outside);
        }
        /// <summary>
        /// Determine whether the frame is have Vertical alignment
        /// </summary>
        /// <returns></returns>
        internal bool IsFrameYAlign(short yPosition)
        {
            return (yPosition == (short)FrameVerticalPosition.Inline
                    || yPosition == (short)FrameVerticalPosition.Top
                    || yPosition == (short)FrameVerticalPosition.Center
                    || yPosition == (short)FrameVerticalPosition.Bottom
                    || yPosition == (short)FrameVerticalPosition.Inside
                    || yPosition == (short)FrameVerticalPosition.Outside);
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        /// <returns></returns>
        private object GetPropertyValue(int propKey)
        {
            UpdateParaFormat(propKey);
            return this[propKey];
        }
        /// <summary>
        /// Updates the paragraph format.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        internal void UpdateParaFormat(int propKey)
        {
            if (IsPropertyUpdated(propKey) && !ContainsBordersSprm())
                return;

            CheckParaProps();

            SetPropUpdateFlag(propKey);
            PropToFormat(propKey);
        }
        /// <summary>
        /// Determines whether the specified option key contains SPRM.
        /// </summary>
        /// <param name="optionKey">The option key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified option key contains SPRM; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsBordersSprm()
        {
            if (m_sprms != null)
            {
                int[] borderSprms = {WordSprmOptions.sprmPBrcTop, WordSprmOptions.sprmPBrcTopNew, 
                                    WordSprmOptions.sprmPBrcBottom, WordSprmOptions.sprmPBrcBottomNew, 
                                    WordSprmOptions.sprmPBrcRight, WordSprmOptions.sprmPBrcRightNew, 
                                    WordSprmOptions.sprmPBrcLeft, WordSprmOptions.sprmPBrcLeftNew,
                                    WordSprmOptions.sprmPBrcBetween, WordSprmOptions.sprmPBrcBetween10,
                                    WordSprmOptions.sprmPBrcBar, WordSprmOptions.sprmPBrcBar10};
                for (int i = 0; i < m_sprms.Modifiers.Count; i++)
                {
                    if (Array.IndexOf(borderSprms, m_sprms.Modifiers[i].TypedOptions) != -1)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks the paragraph props.
        /// </summary>
        private void CheckParaProps()
        {
            if (m_paraProps == null)
            {
                if (m_sprms == null)
                {
                    m_paraProps = new ParagraphProperties();
                    m_sprms = m_paraProps.Sprms;
                }
                else
                {
                    m_paraProps = new ParagraphProperties();
                    m_paraProps.ParagraphPropertyException.PropertyModifiers = m_sprms;
                }
            }
        }
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        /// <param name="value">The value.</param>
        private void SetPropertyValue(int propKey, object value)
        {
            this[propKey] = value;
            UpdateParaProps(propKey, value);
        }
        /// <summary>
        /// Updates the paragraph props.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        /// <param name="value">The value.</param>
        private void UpdateParaProps(int propKey, object value)
        {
            if (m_hasClonedProps && m_sprms != null)
            {
                m_sprms = m_sprms.Clone();
                m_paraProps.ParagraphPropertyException.PropertyModifiers = m_sprms;
                m_hasClonedProps = false;
            }

            CheckParaProps();
            SetPropUpdateFlag(propKey);
            ParagraphPropertiesConverter.FormatToProp(propKey, value, m_paraProps, this);
        }
        /// <summary>
        /// Changes the tabs.
        /// </summary>
        /// <param name="tabs">The tabs.</param>
        internal void ChangeTabs(TabCollection tabs)
        {
            this[TabsKey] = tabs;
            UpdateParaProps(TabsKey, tabs);
        }
        /// <summary>
        /// Creates the new collection of tabs.
        /// </summary>
        internal void CreateTabsCol()
        {
            this[TabsKey] = new TabCollection(m_doc, this);
        }
        /// <summary>
        /// Updates the default formats.
        /// Updates the document default formattings while cloning the document.
        /// </summary>
        internal void UpdateDefaultFormats()
        {
            if (m_doc.m_defParaFormat == null)
                return;

            foreach (KeyValuePair<int, object> keyValuePair in m_doc.m_defParaFormat.PropertiesHash)
            {
                if (keyValuePair.Key != BordersKey
                    && !this.ContainsValue(keyValuePair.Key))
                {
                    PropertiesHash.Add(keyValuePair.Key, keyValuePair.Value);

                    if (m_doc.m_defParaFormat.Sprms != null)
                    {
                        int option = GetSprmOption(keyValuePair.Key);
                        SinglePropertyModifierRecord sprm = m_doc.m_defParaFormat.Sprms[option];
                        if (sprm != null)
                        {
                            if (m_sprms == null)
                                m_sprms = new SinglePropertyModifierArray();
                            m_sprms.Add(sprm);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determines whether the specified key contains value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key contains value; otherwise, <c>false</c>.
        /// </returns>
        internal bool ContainsValue(int key)
        {
            if (PropertiesHash.ContainsKey(key)
                || (BaseFormat is WParagraphFormat
                && (BaseFormat as WParagraphFormat).ContainsValue(key)))
            {
                return true;
            }
            else if ((this.OwnerBase is WParagraph)//check whether the specified key in the table style or not
                    && (this.OwnerBase as WParagraph).IsInCell
                    && (this.OwnerBase as WParagraph).Owner.Owner is WTableRow
                    && (this.OwnerBase as WParagraph).Owner.Owner.Owner is WTable)
            {
                WTableStyle tblStyle = ((this.OwnerBase as WParagraph).Owner.Owner.Owner as WTable).GetStyle() as WTableStyle;
                if(tblStyle!=null
                    && tblStyle.ParagraphFormat.Sprms[GetSprmOption(key)] != null)                                      
                    return true;                
            }

            return false;
        }
        /// <summary>
        /// Determine whether the Current paragraph is in Same Frame of the previous paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal bool IsPreviousParagraphInSameFrame()
        {
            if ((OwnerBase is WParagraph) && (OwnerBase as WParagraph).PreviousSibling != null && (OwnerBase as WParagraph).PreviousSibling is WParagraph)
            {
                WParagraphFormat prevParaFormat = ((OwnerBase as WParagraph).PreviousSibling as WParagraph).ParagraphFormat;
                return IsInSameFrame(prevParaFormat);
            }
            return false;
        }
        /// <summary>
        /// Determine whether the Next paragraph is in Same Frame of the current paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal bool IsNextParagraphInSameFrame()
        {
            if ((OwnerBase is WParagraph) && (OwnerBase as WParagraph).NextSibling != null && (OwnerBase as WParagraph).NextSibling is WParagraph)
            {
                WParagraphFormat nextParaFormat = ((OwnerBase as WParagraph).NextSibling as WParagraph).ParagraphFormat;
                return IsInSameFrame(nextParaFormat);
            }
            return false;
        }
        /// <summary>
        /// Determine whether the current paragraph is having a same frame properties
        /// </summary>
        /// <param name="paraFormat"></param>
        /// <returns></returns>
        private bool IsInSameFrame(WParagraphFormat paraFormat)
        {
            //Reshift short value to float value
            ushort heightValue = (ushort)Math.Round(paraFormat.FrameHeight * DLSConstants.TwipsInOnePoint);
            float prevframeHeight = (heightValue & ((1 << 15) - 1));
            heightValue = (ushort)Math.Round(FrameHeight * DLSConstants.TwipsInOnePoint);
            float currentframeHeight = (heightValue & ((1 << 15) - 1));
            return (paraFormat.IsFrame && paraFormat.FrameX == this.FrameX
                    && paraFormat.FrameWidth == this.FrameWidth
                    && prevframeHeight == currentframeHeight
                    && paraFormat.FrameHorizontalPos == this.FrameHorizontalPos
                    && paraFormat.FrameVerticalPos == this.FrameVerticalPos
                    && paraFormat.FrameY == this.FrameY
                    && paraFormat.WrapFrameAround == this.WrapFrameAround);
        }
        /// <summary>
        /// Sets the default properties.
        /// </summary>
        internal void SetDefaultProperties()
        {
            // Only handled properties are set here as of now
            PropertiesHash.Add(WParagraphFormat.BackColorKey, Color.Empty);
            PropertiesHash.Add(WParagraphFormat.BeforeSpacingKey, 0f);
            PropertiesHash.Add(WParagraphFormat.AfterSpacingKey, 0f);
            PropertiesHash.Add(WParagraphFormat.BidiKey, false);
            PropertiesHash.Add(WParagraphFormat.ColumnBreakAfterKey, false);
            PropertiesHash.Add(WParagraphFormat.ContextualSpacingKey, false);
            PropertiesHash.Add(WParagraphFormat.FirstLineIndentKey, 0f);
            PropertiesHash.Add(WParagraphFormat.FirstLineIndentBiKey, 0f);
            PropertiesHash.Add(WParagraphFormat.ForeColorKey, Color.Empty);
            PropertiesHash.Add(WParagraphFormat.HrAlignmentKey, HorizontalAlignment.Left);
            PropertiesHash.Add(WParagraphFormat.KeepKey, false);
            PropertiesHash.Add(WParagraphFormat.KeepFollowKey, false);
            PropertiesHash.Add(WParagraphFormat.LeftIndentKey, 0f);
            PropertiesHash.Add(WParagraphFormat.LineSpacingKey, 12f);
            PropertiesHash.Add(WParagraphFormat.LineSpacingRuleKey, LineSpacingRule.Multiple);
            PropertiesHash.Add(WParagraphFormat.OutlineLevelKey, (byte)OutlineLevel.BodyText);
            PropertiesHash.Add(WParagraphFormat.PageBreakAfterKey, false);
            PropertiesHash.Add(WParagraphFormat.PageBreakBeforeKey, false);
            PropertiesHash.Add(WParagraphFormat.RightIndentKey, 0f);
            PropertiesHash.Add(WParagraphFormat.SpacingAfterAutoKey, false);
            PropertiesHash.Add(WParagraphFormat.SpacingBeforeAutoKey, false);
            PropertiesHash.Add(WParagraphFormat.TextureStyleKey, TextureStyle.TextureNone);
            PropertiesHash.Add(WParagraphFormat.WidowControlKey, false);
            PropertiesHash.Add(WParagraphFormat.WordWrapKey, true);
            Borders.SetDefaultProperties();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        protected internal override void EnsureComposites()
        {
            if (HasKey(BordersKey))
            {
                EnsureComposites(BordersKey);
            }
        }
        /// <summary>
        /// Overrides method : Get default value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetDefValue(int key)
        {
            if (m_doc.m_defParaFormat != null && m_doc.m_defParaFormat != this)
            {
                return m_doc.m_defParaFormat[key];
            }
            else
            {
                switch (key)
                {
                    case HrAlignmentKey:
                        return HorizontalAlignment.Left;
                    //        case VrAlignmentKey:
                    //          return VerticalAlignment.Bottom;
                    case LeftIndentCharsKey:
                    case RightIndentCharsKey:
                    case FirstLineIndentCharsKey:
                    case LeftIndentKey:
                    case LeftIndentBiKey:
                    case RightIndentKey:
                    case RightIndentBiKey:
                    //case BorderColorKey:
                    //  return Color.Empty;
                    case FirstLineIndentKey:
                    case FirstLineIndentBiKey:
                        return 0f;
                    case KeepKey:
                    case PageBreakAfterKey:
                    case ColumnBreakAfterKey:
                    case PageBreakBeforeKey:
                    case KeepFollowKey:
                    case BidiKey:
                    case ChangedFormatKey:
                        return false;
                    case AfterSpacingKey:
                    case BeforeSpacingKey:
                        return 0f;
                    case BackColorKey:
                    case ForeColorKey:
                        return Color.Empty;
                    case DataKey:
                        return null;
                    case LineSpacingKey:
                        return (float)12.0;
                    case LineSpacingRuleKey:
                        return LineSpacingRule.Multiple;
                    case TabsKey:
                        return new TabCollection(Document, this);
                    case TextureStyleKey:
                        return Syncfusion.DocIO.TextureStyle.TextureNone;
                    case SpacingAfterAutoKey:
                    case SpacingBeforeAutoKey:
                    case ContextualSpacingKey:
                    case MirrorIndentsKey:
                    case SuppressAutoHyphensKey:
                    case AutoSpaceDEKey:
                    case AutoSpaceDNKey:
                    case AdjustRightIndentKey:
                        return false;
                    case OutlineLevelKey:
                        return byte.MaxValue;
                    case WidowControlKey:
                    case WordWrapKey:
                        return true;
                    case FrameVerticalDistanceFromTextKey:
                    case FrameHorizontalDistanceFromTextKey:
                    case FrameHeightKey:
                    case FrameWidthKey:
                    case FrameXKey:
                    case FrameYKey:
                        return (float)0;
                    case WrapFrameAroundKey:
                        return FrameWrapMode.Auto;
                    default:
                        throw new ArgumentException("key has invalid value");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override FormatBase GetDefComposite(int key)
        {
            switch (key)
            {
                case BordersKey:
                    return GetDefComposite(BordersKey, new Borders(this, BordersKey));
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        protected override void ImportMembers(FormatBase format)
        {
            base.ImportMembers(format);

            WParagraphFormat paraFormat = format as WParagraphFormat;

            if (paraFormat != null)
            {
                if (Document.IsMailMerge || Document.IsCloning)
                {
                    if (paraFormat.Sprms != null)
                    {
                        m_sprms = paraFormat.Sprms.Clone();
                        m_hasClonedProps = true;
                    }
                }
                else
                {
                    if (paraFormat.Sprms != null)
                    {
                        m_paraProps = null;
                        m_sprms = paraFormat.Sprms.Clone();
                    }
                    m_propsUpdateFlags = null;
                }
                if (paraFormat.HasValue(PageBreakAfterKey))
                    this[PageBreakAfterKey] = paraFormat.PageBreakAfter;
                if (paraFormat.HasValue(ColumnBreakAfterKey))
                    this[ColumnBreakAfterKey] = paraFormat.ColumnBreakAfter;
            }
        }
//#if !SILVERLIGHT
        /// <summary>
        /// Overrides method : Read attributes from xml
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.ParagraphBidiAttr))
            {
                Bidi = reader.ReadBoolean(XDLSConstants.ParagraphBidiAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphHrAlignmentAttr))
            {
                HorizontalAlignment =
                  (HorizontalAlignment)
                  reader.ReadEnum(XDLSConstants.ParagraphHrAlignmentAttr, typeof(HorizontalAlignment));
            }
            //      if( reader.HasAttribute( XDLSConstants.ParagraphVrAlignmentAttr ) )
            //      {
            //        VerticalAlignment =
            //          ( VerticalAlignment )reader.ReadEnum( XDLSConstants.ParagraphVrAlignmentAttr, typeof( VerticalAlignment ) );
            //      }
            if (reader.HasAttribute(XDLSConstants.ParagraphLeftIndentAttr))
            {
                LeftIndent = reader.ReadFloat(XDLSConstants.ParagraphLeftIndentAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphRightIndentAttr))
            {
                RightIndent = reader.ReadFloat(XDLSConstants.ParagraphRightIndentAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphFirstLineIndentAttr))
            {
                FirstLineIndent = reader.ReadFloat(XDLSConstants.ParagraphFirstLineIndentAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphKeepAttr))
            {
                Keep = reader.ReadBoolean(XDLSConstants.ParagraphKeepAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphBeforeSpacingAttr))
            {
                BeforeSpacing = reader.ReadFloat(XDLSConstants.ParagraphBeforeSpacingAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphAfterSpacingAttr))
            {
                AfterSpacing = reader.ReadFloat(XDLSConstants.ParagraphAfterSpacingAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphKeepFollowAttr))
            {
                KeepFollow = reader.ReadBoolean(XDLSConstants.ParagraphKeepFollowAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphWidowControlAttr))
            {
                WidowControl = reader.ReadBoolean(XDLSConstants.ParagraphWidowControlAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphPageBreakBeforeAttr))
            {
                PageBreakBefore = reader.ReadBoolean(XDLSConstants.ParagraphPageBreakBeforeAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphPageBreakAfterAttr))
            {
                PageBreakAfter = reader.ReadBoolean(XDLSConstants.ParagraphPageBreakAfterAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphBackColorAttr))
            {
                BackColor = reader.ReadColor(XDLSConstants.ParagraphBackColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphColumnBreakAfterAttr))
            {
                ColumnBreakAfter = reader.ReadBoolean(XDLSConstants.ParagraphColumnBreakAfterAttr);
            }

            if (reader.HasAttribute(XDLSConstants.ParagraphLineSpacingAttr))
            {
                LineSpacing = reader.ReadFloat(XDLSConstants.ParagraphLineSpacingAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ParagraphLineSpacingRuleAttr))
            {
                LineSpacingRule = (LineSpacingRule)reader.ReadEnum(XDLSConstants.ParagraphLineSpacingRuleAttr, typeof(LineSpacingRule));
            }
            if (reader.HasAttribute(XDLSConstants.TextForeColorAttr))
            {
                ForeColor = reader.ReadColor(XDLSConstants.TextForeColorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.TextTextureAttr))
            {
                TextureStyle = (TextureStyle)reader.ReadEnum(XDLSConstants.TextTextureAttr, typeof(TextureStyle));
            }
        }
        /// <summary>
        /// Overrides method : Write attributes to xml
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (HasKey(PageBreakAfterKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphPageBreakAfterAttr, PageBreakAfter);
            }
            if (HasKey(ColumnBreakAfterKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphColumnBreakAfterAttr, ColumnBreakAfter);
            }

            if (m_sprms != null)
                return;

            if (HasValue(BidiKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphBidiAttr, Bidi);
            }
            if (HasValue(HrAlignmentKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphHrAlignmentAttr, HorizontalAlignment);
            }
            //      if( HasKey( VrAlignmentKey ) )
            //      {
            //        writer.WriteValue( XDLSConstants.ParagraphVrAlignmentAttr, VerticalAlignment );
            //      }
            if (HasValue(LeftIndentKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphLeftIndentAttr, LeftIndent);
            }
            if (HasValue(RightIndentKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphRightIndentAttr, RightIndent);
            }
            if (HasValue(FirstLineIndentKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphFirstLineIndentAttr, FirstLineIndent);
            }
            if (HasValue(KeepKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphKeepAttr, Keep);
            }
            if (HasValue(BeforeSpacingKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphBeforeSpacingAttr, BeforeSpacing);
            }
            if (HasValue(AfterSpacingKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphAfterSpacingAttr, AfterSpacing);
            }
            if (HasValue(KeepFollowKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphKeepFollowAttr, KeepFollow);
            }
            if (HasValue(WidowControlKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphWidowControlAttr, WidowControl);
            }
            if (HasValue(PageBreakBeforeKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphPageBreakBeforeAttr, PageBreakBefore);
            }


            if (!BackColor.IsEmpty)
            {
                writer.WriteValue(XDLSConstants.ParagraphBackColorAttr, BackColor);
            }
            //      if( HasKey( BordersKey ))
            //      {
            //        writer.WriteValue( "Borders",  );
            //      }

            if (HasValue(LineSpacingKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphLineSpacingAttr, LineSpacing);
            }
            if (HasValue(LineSpacingRuleKey))
            {
                writer.WriteValue(XDLSConstants.ParagraphLineSpacingRuleAttr, LineSpacingRule);
            }
            if (ForeColor != Color.Empty)
            {
                writer.WriteValue(XDLSConstants.TextForeColorAttr, ForeColor);
            }
            if (TextureStyle != TextureStyle.TextureNone)
            {
                writer.WriteValue(XDLSConstants.TextTextureAttr, TextureStyle);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(Syncfusion.DocIO.DLS.XML.IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);
            if (m_sprms != null)
            {
                byte[] internalData = new byte[Sprms.Length];
                Sprms.Save(internalData, 0);
                writer.WriteChildBinaryElement(XDLSConstants.InternalDataTag, internalData);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override bool ReadXmlContent(Syncfusion.DocIO.DLS.XML.IXDLSContentReader reader)
        {
            bool retValue = base.ReadXmlContent(reader);

            if (reader.TagName == XDLSConstants.InternalDataTag)
            {
                byte[] internalData = reader.ReadChildBinaryElement();
                m_sprms = new SinglePropertyModifierArray(internalData);
                retValue = true;
                if (m_paraProps != null)
                {
                    m_paraProps.ParagraphPropertyException.PropertyModifiers = m_sprms;
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            if (m_sprms == null)
            {
                XDLSHolder.AddElement(XDLSConstants.BordersItemTag, Borders);
                XDLSHolder.AddElement(XDLSConstants.ParagraphTabsAttr, Tabs);
            }
        }
//#endif
        
        /// <summary>
        /// Action on format change.
        /// </summary>
        /// <param name="format">The format.</param>
        protected override void OnChange(FormatBase format, int propKey)
        {
            if (m_cancelOnChange)
                return;
#if SILVERLIGHT || WP
            if (this.OwnerBase != null && this.OwnerBase.Document.IsOpening
                /*&& this.OwnerBase.Document.DocxPackage == null*/)
#else
            if (this.OwnerBase != null && this.OwnerBase.Document.IsOpening
                    && this.OwnerBase.Document.DocxPackage == null && this .OwnerBase .Document .ActualFormatType != FormatType.Rtf )
#endif
                return;

            int propertyKey = int.MinValue;

            if (format is Border || format is Borders)
            {
                propertyKey = BordersKey;
            }

            if (propertyKey != int.MinValue)
            {
                UpdateParaProps(propertyKey, this[propertyKey]);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override void AcceptChanges()
        {
            this[ChangedFormatKey] = false;
            if (m_sprms != null && m_sprms.Length > 0)
            {
                m_sprms.RemoveValue(WordSprmOptions.sprmPPropRMark);
                m_sprms.RemoveValue(WordSprmOptions.sprmPPropRMark90);

                base.AcceptChanges();
            }
        }
        /// <summary>
        /// Removes the positioning.
        /// </summary>
        internal override void RemovePositioning()
        {
            if (m_sprms != null && m_sprms.Count > 0)
            {
                m_sprms.RemoveValue(WordSprmOptions.sprmPPc);
                m_sprms.RemoveValue(WordSprmOptions.sprmPWr);
                m_sprms.RemoveValue(WordSprmOptions.sprmPDxaAbs);
                m_sprms.RemoveValue(WordSprmOptions.sprmPDyaAbs);
                m_sprms.RemoveValue(WordSprmOptions.sprmPDxaFromText);
                m_sprms.RemoveValue(WordSprmOptions.sprmPDxaFromText10);
                m_sprms.RemoveValue(WordSprmOptions.sprmPDyaFromText);
                m_sprms.RemoveValue(WordSprmOptions.sprmPDxaWidth);
                m_sprms.RemoveValue(WordSprmOptions.sprmPWHeightAbs);
            }
        }
        /// <summary>
        /// Apply base style
        /// </summary>
        /// <param name="baseFormat"></param>
        internal override void ApplyBase(FormatBase baseFormat)
        {
            base.ApplyBase(baseFormat);
            Borders.ApplyBase((baseFormat as WParagraphFormat).Borders);
            if (Document.IsOpening)
            {
                ParagraphProperties props = (baseFormat as WParagraphFormat).ParaProps;
                if (props == null)
                    return;
                ParaProps.BaseProperties = props;
            }
        }
        #endregion

        #region Implementation / xml helper methods
        /// <summary>
        /// Determines whether the specified property key has value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	if the specified property key has value, set to <c>true</c>.
        /// </returns>
        internal override bool HasValue(int propertyKey)
        {
            //Updates key and value to the PropertiesHash collection, if corresponding format is defined.
            UpdateParaFormat(propertyKey);
            if (HasKey(propertyKey))
                return true;

            if (m_sprms == null || m_sprms.Count == 0)
                return false;

            int sprmOptionKey = GetSprmOption(propertyKey);
            if (sprmOptionKey == int.MaxValue)
                return false;

            SinglePropertyModifierRecord sprm = m_sprms[sprmOptionKey];
            if (sprm == null)
                return false;

            return true;
        }
        /// <summary>
        /// Determines whether the specified property key has boolean value
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	if the specified property key has value, set to <c>true</c>.
        /// </returns>
        internal  bool HasBoolValue(int propertyKey)
        {
            if (HasBoolKey (propertyKey))
                return true;

            if (m_sprms == null || m_sprms.Count == 0)
                return false;

            int sprmOptionKey = GetSprmOption(propertyKey);
            if (sprmOptionKey == int.MaxValue)
                return false;

            SinglePropertyModifierRecord sprm = m_sprms[sprmOptionKey];
            if (sprm == null)
                return false;

            return false; ;
        }
        /// <summary>
        /// Determines whether the specified property is available. Search includes base formats.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	<c>true</c> if has value; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasValueWithParent(int propertyKey)
        {
            bool hasValue = HasValue(propertyKey);
            if (!hasValue)
            {
                if (this.BaseFormat != null)
                {
                    int sprmOptionKey = GetSprmOption(propertyKey);
                    ParaProps.BaseProperties = (this.BaseFormat as WParagraphFormat).ParaProps;
                    if (ParaProps.HasValue(sprmOptionKey))
                        return true;
                }
            }

            return hasValue;
        }
        /// <summary>
        /// Determines whether the specified property is available and checkfir its boolean value
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	<c>true</c> if has value; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasBoolValueWithParent(int propertyKey)
        {
            bool hasValue = HasBoolValue(propertyKey);
            if (!hasValue)
            {
                if (this.BaseFormat != null)
                {
                    int sprmOptionKey = GetSprmOption(propertyKey);
                    ParaProps.BaseProperties = (this.BaseFormat as WParagraphFormat).ParaProps;
                    if (ParaProps.Sprms.Contain(propertyKey))
                    {
                        bool value = ParaProps.Sprms[sprmOptionKey].BoolValue;
                        if (ParaProps.HasValue(sprmOptionKey) && value)
                            return true;
                    }
                }
            }

            return hasValue;
        }

        /// <summary>
        /// Determines whether this instance has shading.
        /// </summary>
        /// <returns>
        /// 	if this instance has shading, set to <c>true</c>.
        /// </returns>
        internal bool HasShading()
        {
            if (m_paraProps != null && (m_paraProps.Sprms[WordSprmOptions.sprmPShdNew] != null ||
              m_paraProps.Sprms[WordSprmOptions.sprmPShd] != null))
            {
                return true;
            }

            return false;

        }
        /// <summary>
        /// Gets the SPRM option.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns></returns>
        protected override int GetSprmOption(int propertyKey)
        {
            switch (propertyKey)
            {
                case BidiKey:
                    return WordSprmOptions.sprmPFBiDi;
                case KeepKey:
                    return WordSprmOptions.sprmPFKeep;
                case KeepFollowKey:
                    return WordSprmOptions.sprmPFKeepFollow;
                case PageBreakBeforeKey:
                    return WordSprmOptions.sprmPFPageBreakBefore;
                case MirrorIndentsKey:
                    return WordSprmOptions.sprmPFMirrorIndents;
                case WidowControlKey:
                    return WordSprmOptions.sprmPFWidowControl;
                case LeftIndentKey:
                    return WordSprmOptions.sprmPDxaLeft;
                case RightIndentKey:
                    return WordSprmOptions.sprmPDxaRight;
                case FirstLineIndentKey:
                    return WordSprmOptions.sprmPDxaLeft1;
                case LeftIndentBiKey:
                    return WordSprmOptions.sprmPDxaLeftBi;
                case RightIndentBiKey:
                    return WordSprmOptions.sprmPDxaRightBi;
                case FirstLineIndentBiKey:
                    return WordSprmOptions.sprmPDxaLeft1Bi;
                case BeforeSpacingKey:
                    return WordSprmOptions.sprmPDyaBefore;
                case AfterSpacingKey:
                    return WordSprmOptions.sprmPDyaAfter;
                case LineSpacingKey:
                case LineSpacingRuleKey:
                    return WordSprmOptions.sprmPDyaLine;
                case HrAlignmentKey:
                    return WordSprmOptions.sprmPJc;
                case SpacingAfterAutoKey:
                    return WordSprmOptions.sprmPFAfterAuto;
                case SpacingBeforeAutoKey:
                    return WordSprmOptions.sprmPFBeforeAuto;
                case OutlineLevelKey:
                    return WordSprmOptions.sprmPOutLvl;
                case LeftBorderKey:
                    return WordSprmOptions.sprmPBrcLeft;
                case LeftBorderNewKey:
                    return WordSprmOptions.sprmPBrcLeftNew;
                case RightBorderKey:
                    return WordSprmOptions.sprmPBrcRight;
                case RightBorderNewKey:
                    return WordSprmOptions.sprmPBrcRightNew;
                case TopBorderKey:
                    return WordSprmOptions.sprmPBrcTop;
                case TopBorderNewKey:
                    return WordSprmOptions.sprmPBrcTopNew;
                case BottomBorderKey:
                    return WordSprmOptions.sprmPBrcBottom;
                case BottomBorderNewKey:
                    return WordSprmOptions.sprmPBrcBottomNew;
                case BetweenBorderKey:
                    return WordSprmOptions.sprmPBrcBetween;
                case BarBorderKey:
                    return WordSprmOptions.sprmPBrcBar;
                case FramePosKey:
                    return WordSprmOptions.sprmPPc;
                case FrameXKey:
                    return WordSprmOptions.sprmPDxaAbs;
                case FrameYKey:
                    return WordSprmOptions.sprmPDyaAbs;
                case FrameWidthKey:
                    return WordSprmOptions.sprmPDxaWidth;
                case FrameHeightKey:
                    return WordSprmOptions.sprmPWHeightAbs;
                case FrameHorizontalDistanceFromTextKey:
                    return WordSprmOptions.sprmPDxaFromText;
                case FrameVerticalDistanceFromTextKey:
                    return WordSprmOptions.sprmPDyaFromText;
                case WrapFrameAroundKey:
                    return WordSprmOptions.sprmPWr;
                case ContextualSpacingKey:
                    return WordSprmOptions.sprmPFContSpacing;
                case SuppressAutoHyphensKey:
                    return WordSprmOptions.sprmPFNoAutoHyph;
                case AutoSpaceDEKey:
                    return WordSprmOptions.sprmPFAutoSpaceDE;
                case AutoSpaceDNKey:
                    return WordSprmOptions.sprmPFAutoSpaceDN;
                case AdjustRightIndentKey:
                    return WordSprmOptions.sprmPFAdjustRight;
                case LeftIndentCharsKey:
                    return WordSprmOptions.sprmPDxcLeft;
                case RightIndentCharsKey:
                    return WordSprmOptions.sprmPDxcRight;
                case FirstLineIndentCharsKey:
                    return WordSprmOptions.sprmPDxcLeft1;
                case WordWrapKey:
                    return WordSprmOptions.sprmPFWordWrap;
                default:
                    return int.MaxValue;
            }
        }

        #endregion

        #region Internal properties converter
        /// <summary>
        /// Converts ParagraphProperties property to format property.
        /// </summary>
        /// <param name="propKey">The property key.</param>
        private void PropToFormat(int propKey)
        {
            if (this.BaseFormat != null)
                ParaProps.BaseProperties = (this.BaseFormat as WParagraphFormat).ParaProps;

            switch (propKey)
            {
                #region Boolean properties
                case BidiKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[BidiKey] = m_paraProps.Bidi;
                    break;
                case KeepKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[KeepKey] = m_paraProps.Keep;
                    break;
                case KeepFollowKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[KeepFollowKey] = m_paraProps.KeepFollow;
                    break;
                case PageBreakBeforeKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[PageBreakBeforeKey] = m_paraProps.PageBreakBefore;
                    break;
                case MirrorIndentsKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[MirrorIndentsKey] = m_paraProps.MirrorIndents;
                    break;
                case WidowControlKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[WidowControlKey] = m_paraProps.WidowControl;
                    break;
                case AutoSpaceDEKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[AutoSpaceDEKey] = m_paraProps.AutoSpaceDE;
                    break;
                case AutoSpaceDNKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[AutoSpaceDNKey] = m_paraProps.AutoSpaceDN;
                    break;
                case AdjustRightIndentKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[AdjustRightIndentKey] = m_paraProps.AdjustRightIndent;
                    break;
                case SpacingBeforeAutoKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[SpacingBeforeAutoKey] = (m_paraProps.SpacingBeforeAuto == 1);
                    break;
                case SpacingAfterAutoKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null) 
                        this[SpacingAfterAutoKey] = (m_paraProps.SpacingAfterAuto == 1);
                    break;
                case ChangedFormatKey:
                    UpdateChangedFormat();
                    break;
                case ContextualSpacingKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null) 
                        this[ContextualSpacingKey] = m_paraProps.ContextualSpacing;
                    break;
                case SuppressAutoHyphensKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[SuppressAutoHyphensKey] = m_paraProps.SuppressAutoHyphens;
                    break;
                case WordWrapKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[propKey] = m_paraProps.WordWrap;
                    break;
                #endregion

                #region Float properties
                    case FrameYKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        if (IsFrameYAlign(m_paraProps.FrameYCoordinate))
                            this[FrameYKey] = (float)m_paraProps.FrameYCoordinate;
                        else
                            this[FrameYKey] = (float)m_paraProps.FrameYCoordinate / DLSConstants.TwipsInOnePoint;
                    break;
                case FrameXKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        if (IsFrameXAlign(m_paraProps.FrameXCoordinate))
                            this[FrameXKey] = (float)m_paraProps.FrameXCoordinate;
                        else
                            this[FrameXKey] = (float)m_paraProps.FrameXCoordinate / DLSConstants.TwipsInOnePoint;
                    break;
                case FrameHeightKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[FrameHeightKey] = (float)m_paraProps.FrameHeight / DLSConstants.TwipsInOnePoint;
                    break;
                case FrameWidthKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[FrameWidthKey] = (float)m_paraProps.FrameWidth / DLSConstants.TwipsInOnePoint;
                    break;
                case FrameHorizontalDistanceFromTextKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[FrameHorizontalDistanceFromTextKey] = (float)m_paraProps.FrameHorizontalDistanceFromText / DLSConstants.TwipsInOnePoint;
                    break;
                case FrameVerticalDistanceFromTextKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[FrameVerticalDistanceFromTextKey] = (float)m_paraProps.FrameVerticalDistanceFromText / DLSConstants.TwipsInOnePoint;
                    break;
                case WrapFrameAroundKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[WrapFrameAroundKey] = (FrameWrapMode)m_paraProps.WrapFrameAround;
                    break;
                case FirstLineIndentCharsKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[FirstLineIndentCharsKey] = (float)m_paraProps.IndentLeftFirstChars / DLSConstants.HundredthsUnit;
                    break;
                case LeftIndentCharsKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[LeftIndentCharsKey] = (float)m_paraProps.IndentLeftChars / DLSConstants.HundredthsUnit;
                    break;
                case RightIndentCharsKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null)
                        this[RightIndentCharsKey] = (float)m_paraProps.IndentRightChars / DLSConstants.HundredthsUnit;
                    break;
                case LeftIndentKey:
                    UpdateLeftIndent();
                    break;
                case LeftIndentBiKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null) 
                        this[LeftIndentBiKey] = (float)m_paraProps.IndentLeftBi / DLSConstants.TwipsInOnePoint;
                    break;
                case RightIndentKey:
                    UpdateRightIndent();
                    break;
                case RightIndentBiKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null) 
                        this[RightIndentBiKey] = (float)m_paraProps.IndentRightBi / DLSConstants.TwipsInOnePoint;
                    break;
                case FirstLineIndentKey:
                    UpdateFirstLineIndent();
                    break;
                case FirstLineIndentBiKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null) 
                        this[FirstLineIndentBiKey] = (float)m_paraProps.IndentLeftFirstBi / DLSConstants.TwipsInOnePoint;
                    break;
                case BeforeSpacingKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null) 
                        this[BeforeSpacingKey] = (float)m_paraProps.BeforeSpacing / DLSConstants.TwipsInOnePoint;
                    break;
                case AfterSpacingKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null) 
                        this[AfterSpacingKey] = (float)m_paraProps.AfterSpacing / DLSConstants.TwipsInOnePoint;
                    break;
                case LineSpacingKey:
                case LineSpacingRuleKey:
                    UpdateLineSpacing(propKey);
                    break;
                #endregion

                #region Other
                case TabsKey:
                    UpdateTabs();
                    break;
                case HrAlignmentKey:
                    UpdateJustification();
                    break;
                case BackColorKey:
                case ForeColorKey:
                case TextureStyleKey:
                    UpdateShading(propKey);
                    break;
                case BordersKey:
                    UpdateBorders();
                    break;
                case OutlineLevelKey:
                    if (m_paraProps.Sprms[GetSprmOption(propKey)] != null) 
                        this[OutlineLevelKey] = m_paraProps.OutlineLevel;
                    break;
                #endregion
            }
        }
        /// <summary>
        /// Updates the left indent.
        /// </summary>
        private void UpdateLeftIndent()
        {
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[WordSprmOptions.sprmPDxaLeft];

            if (sprm == null)
            {
                sprm = m_paraProps.Sprms[WordSprmOptions.sprmPDxaLeftBi];
                if (sprm != null && !Bidi)
                    this[LeftIndentKey] = (float)m_paraProps.IndentLeftBi / DLSConstants.TwipsInOnePoint;
            }
            else
                this[LeftIndentKey] = (float)m_paraProps.IndentLeft / DLSConstants.TwipsInOnePoint;
        }
        /// <summary>
        /// Updates the right indent.
        /// </summary>
        private void UpdateRightIndent()
        {
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[WordSprmOptions.sprmPDxaRight];

            if (sprm == null)
            {
                sprm = m_paraProps.Sprms[WordSprmOptions.sprmPDxaRightBi];
                if (sprm != null && !Bidi)
                    this[RightIndentKey] = (float)m_paraProps.IndentRightBi / DLSConstants.TwipsInOnePoint;
            }
            else
                this[RightIndentKey] = (float)m_paraProps.IndentRight / DLSConstants.TwipsInOnePoint;
        }
        /// <summary>
        /// Updates the first line indent.
        /// </summary>
        private void UpdateFirstLineIndent()
        {
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[WordSprmOptions.sprmPDxaLeft1];

            if (sprm == null)
            {
                sprm = m_paraProps.Sprms[WordSprmOptions.sprmPDxaLeft1Bi];
                if (sprm != null && !Bidi)
                    this[FirstLineIndentKey] = (float)m_paraProps.IndentLeftFirstBi / DLSConstants.TwipsInOnePoint;
            }
            else
                this[FirstLineIndentKey] = (float)m_paraProps.IndentLeftFirst / DLSConstants.TwipsInOnePoint;
        }
        /// <summary>
        /// Updates the justification.
        /// </summary>
        private void UpdateJustification()
        {
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[WordSprmOptions.sprmPJc];

            if (sprm == null)
            {
                sprm = m_paraProps.Sprms[WordSprmOptions.sprmPJcBi];
                if (sprm != null)
                    this[HrAlignmentKey] = m_paraProps.JustificationBidi;
            }
            else
                this[HrAlignmentKey] = m_paraProps.Justification;
        }
        /// <summary>
        /// Updates the changed format.
        /// </summary>
        private void UpdateChangedFormat()
        {
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[WordSprmOptions.sprmPPropRMark];

            if (sprm == null)
                sprm = m_paraProps.Sprms[WordSprmOptions.sprmPPropRMark90];

            if (sprm != null)
            {
                this[ChangedFormatKey] = m_paraProps.IsChangedFormat;
            }
        }
        /// <summary>
        /// Updates the line spacing.
        /// </summary>
        private void UpdateLineSpacing(int key)
        {
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[WordSprmOptions.sprmPDyaLine];

            if (sprm == null && OwnerBase is WParagraph)
            {
                WParagraph para = OwnerBase as WParagraph;
                WParagraphStyle paraStyle = para.ParaStyle as WParagraphStyle;
                if (paraStyle != null)
                    UpdateLineSpacingSprm(paraStyle, out sprm);
            }

            if (sprm != null)
            {
                LineSpacingDescriptor descriptor = m_paraProps.GetLineSpacingDescriptor(sprm);
                if (key == LineSpacingKey)
                    this[LineSpacingKey] = (float)descriptor.LineSpacing / DLSConstants.TwipsInOnePoint;
                else if (key == LineSpacingRuleKey)
                    this[LineSpacingRuleKey] = descriptor.LineSpacingRule;
            }
        }
        /// <summary>
        /// Updates the tabs.
        /// </summary>
        private void UpdateTabs()
        {
            bool isListTabs = false;
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[WordSprmOptions.sprmPChgTabsPapx];

            if (sprm == null)
            {
                sprm = m_paraProps.Sprms[WordSprmOptions.sprmPChgTabs];
                isListTabs = true;
            }

            if (sprm != null)
            {
                TabsInfo info = (isListTabs) ? m_paraProps.ListTabs : m_paraProps.TabsInfo;
                TabCollection tabCollection = new TabCollection(Document, this);
                tabCollection.CancelOnChangeEvent = true;
                ParagraphPropertiesConverter.ExportTabs(info, tabCollection);
                this[TabsKey] = tabCollection;
                tabCollection.CancelOnChangeEvent = false;
            }
        }
        /// <summary>
        /// Updates the shading.
        /// </summary>
        private void UpdateShading(int key)
        {
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[WordSprmOptions.sprmPShdNew];
            if (sprm == null)
            {
                sprm = m_paraProps.Sprms[WordSprmOptions.sprmPShd];
            }

            if (sprm != null)
            {
                ShadingDescriptor shading = m_paraProps.GetShading(sprm);
                switch (key)
                {
                    case BackColorKey:
                        this[BackColorKey] = shading.BackColor;
                        break;
                    case ForeColorKey:
                        this[ForeColorKey] = shading.ForeColor;
                        break;
                    case TextureStyleKey:
                        this[TextureStyleKey] = shading.Pattern;
                        break;
                }
            }
        }
        /// <summary>
        /// Updates the borders.
        /// </summary>
        private void UpdateBorders()
        {
            m_cancelOnChange = true;
            UpdateBorder(WordSprmOptions.sprmPBrcTop, WordSprmOptions.sprmPBrcTopNew, BorderSide.Top);
            UpdateBorder(WordSprmOptions.sprmPBrcBottom, WordSprmOptions.sprmPBrcBottomNew, BorderSide.Bottom);
            UpdateBorder(WordSprmOptions.sprmPBrcRight, WordSprmOptions.sprmPBrcRightNew, BorderSide.Right);
            UpdateBorder(WordSprmOptions.sprmPBrcLeft, WordSprmOptions.sprmPBrcLeftNew, BorderSide.Left);
            UpdateBorder(WordSprmOptions.sprmPBrcBetween, WordSprmOptions.sprmPBrcBetween10, BorderSide.Between);
            UpdateBorder(WordSprmOptions.sprmPBrcBar, WordSprmOptions.sprmPBrcBar10, BorderSide.Bar);
            m_cancelOnChange = false;
        }
        /// <summary>
        /// Updates the border.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="optionNew">The option new.</param>
        /// <param name="borderSide">The border side.</param>
        private void UpdateBorder(int option, int optionNew, BorderSide borderSide)
        {
            Borders destBorders = null;
            SinglePropertyModifierRecord sprm = m_paraProps.Sprms[optionNew];
            if (sprm == null)
            {
                sprm = m_paraProps.Sprms[option];
            }

            if (sprm != null)
            {
                destBorders = this[BordersKey] as Borders;
                BorderCode srcBorder = m_paraProps.GetBorder(sprm);
                switch (borderSide)
                {
                    case BorderSide.Top:
                        ParagraphPropertiesConverter.ExportBorder(srcBorder, destBorders.Top);
                        break;
                    case BorderSide.Bottom:
                        ParagraphPropertiesConverter.ExportBorder(srcBorder, destBorders.Bottom);
                        break;
                    case BorderSide.Left:
                        ParagraphPropertiesConverter.ExportBorder(srcBorder, destBorders.Left);
                        break;
                    case BorderSide.Right:
                        ParagraphPropertiesConverter.ExportBorder(srcBorder, destBorders.Right);
                        break;
                    case BorderSide.Between:
                        ParagraphPropertiesConverter.ExportBorder(srcBorder, destBorders.Horizontal);
                        break;
                    case BorderSide.Bar:
                        ParagraphPropertiesConverter.ExportBorder(srcBorder, destBorders.Vertical);
                        break;
                }
            }
        }
        /// <summary>
        /// Update LineSpacing sprm
        /// </summary>
        /// <param name="style"></param>
        /// <param name="sprm"></param>
        private void UpdateLineSpacingSprm(WParagraphStyle style, out SinglePropertyModifierRecord sprm) 
        {
            sprm = style.ParagraphFormat.ParaProps.Sprms[WordSprmOptions.sprmPDyaLine];
            if (sprm == null && style.BaseStyle != null)
                UpdateLineSpacingSprm(style.BaseStyle, out sprm);
        }
        #endregion

        #region Implementation / Import contents
        /// <summary>
        /// Updates the source format.
        /// </summary>
        /// <param name="sourceBaseFormat">The dest base format.</param>
        internal void UpdateSourceFormat(WParagraphFormat sourceBaseFormat)
        {
            //Imports the source format
            this.ApplyBase(new WParagraphFormat(Document));
            CopySourceSprms(sourceBaseFormat);
            CopySourceProperties(sourceBaseFormat);
            UpdateSourceFormatting(sourceBaseFormat);
        }
        /// <summary>
        /// Copy source sprms
        /// </summary>
        /// <param name="sourceBaseFormat"></param>
        private void CopySourceSprms(WParagraphFormat sourceBaseFormat)
        {
            if (sourceBaseFormat.m_sprms != null)
            {
                foreach (SinglePropertyModifierRecord sprm in sourceBaseFormat.m_sprms)
                {
                    if (!m_sprms.Contain(sprm.Options))
                        m_sprms.Add(sprm.Clone());
                }
            }
        }
        /// <summary>
        /// Copy Source properties
        /// </summary>
        /// <param name="sourceBaseFormat"></param>
        private void CopySourceProperties(WParagraphFormat sourceBaseFormat)
        {
            foreach (KeyValuePair<int, Object> keyValue in sourceBaseFormat.PropertiesHash)
            {
                if (!(keyValue.Value is Borders || keyValue.Value is Border))
                {
                    if (!PropertiesHash.ContainsKey(keyValue.Key))
                        PropertiesHash.Add(keyValue.Key, keyValue.Value);
                }
            }
        }
        /// <summary>
        /// Updates the source formatting.
        /// </summary>
        /// <param name="format">The format.</param>
        private void UpdateSourceFormatting(WParagraphFormat format)
        {
            if (!HasKey(AdjustRightIndentKey) && format.AdjustRightIndent != AdjustRightIndent)
                AdjustRightIndent = format.AdjustRightIndent;
            if (!HasKey(AfterSpacingKey) && format.AfterSpacing != AfterSpacing)
                AfterSpacing = format.AfterSpacing;
            if (format.AutoSpaceDE != AutoSpaceDE && !HasKey(AutoSpaceDEKey))
                AutoSpaceDE = format.AutoSpaceDE;
            if (format.AutoSpaceDN != AutoSpaceDN && !HasKey(AutoSpaceDNKey))
                AutoSpaceDN = format.AutoSpaceDN;
            if (format.BackColor != BackColor && !HasKey(BackColorKey))
                BackColor = format.BackColor;
            if (format.BeforeSpacing != BeforeSpacing && !HasKey(BeforeSpacingKey))
                BeforeSpacing = format.BeforeSpacing;
            if (format.Bidi != Bidi && !HasKey(BidiKey))
                Bidi = format.Bidi;
            if (format.ColumnBreakAfter != ColumnBreakAfter && !HasKey(ColumnBreakAfterKey))
                ColumnBreakAfter = format.ColumnBreakAfter;
            if (format.ContextualSpacing != ContextualSpacing && !HasKey(ContextualSpacingKey))
                ContextualSpacing = format.ContextualSpacing;
            if (format.FirstLineIndent != FirstLineIndent && !HasKey(FirstLineIndentKey))
                FirstLineIndent = format.FirstLineIndent;
            if (format.FirstLineIndentBi != FirstLineIndentBi && !HasKey(FirstLineIndentBiKey))
                FirstLineIndentBi = format.FirstLineIndentBi;
            if (format.ForeColor != ForeColor && !HasKey(ForeColorKey))
                ForeColor = format.ForeColor;
            if (format.FrameHeight != FrameHeight && !HasKey(FrameHeightKey))
                FrameHeight = format.FrameHeight;
            if (format.FrameHorizontalDistanceFromText != FrameHorizontalDistanceFromText && !HasKey(FrameHorizontalDistanceFromTextKey))
                FrameHorizontalDistanceFromText = format.FrameHorizontalDistanceFromText;
            if (format.FrameHorizontalPos != FrameHorizontalPos)
                FrameHorizontalPos = format.FrameHorizontalPos;
            if (format.FrameVerticalDistanceFromText != FrameVerticalDistanceFromText && !HasKey(FrameVerticalDistanceFromTextKey))
                FrameVerticalDistanceFromText = format.FrameVerticalDistanceFromText;
            if (format.FrameVerticalPos != FrameVerticalPos)
                FrameVerticalPos = format.FrameVerticalPos;
            if (format.FrameWidth != FrameWidth && !HasKey(FrameWidthKey))
                FrameWidth = format.FrameWidth;
            if (format.FrameX != FrameX && !HasKey(FrameXKey))
                FrameX = format.FrameX;
            if (format.FrameY != FrameY && !HasKey(FrameYKey))
                FrameY = format.FrameY;
            if (format.HorizontalAlignment != HorizontalAlignment && !HasKey(HrAlignmentKey))
                HorizontalAlignment = format.HorizontalAlignment;
            if (format.Keep != Keep && !HasKey(KeepKey))
                Keep = format.Keep;
            if (format.KeepFollow != KeepFollow && !HasKey(KeepFollowKey))
                KeepFollow = format.KeepFollow;
            if (format.LeftIndent != LeftIndent && !HasKey(LeftIndentKey))
                LeftIndent = format.LeftIndent;
            if (format.LeftIndentBi != LeftIndentBi && !HasKey(LeftIndentBiKey))
                LeftIndentBi = format.LeftIndentBi;
            if (format.LineSpacing != LineSpacing && !HasKey(LineSpacingKey))
                LineSpacing = format.LineSpacing;
            if (format.LineSpacingRule != LineSpacingRule && !HasKey(LineSpacingRuleKey))
                LineSpacingRule = format.LineSpacingRule;
            if (format.MirrorIndents != MirrorIndents && !HasKey(MirrorIndentsKey))
                MirrorIndents = format.MirrorIndents;
            if (format.OutlineLevel != OutlineLevel && !HasKey(OutlineLevelKey))
                OutlineLevel = format.OutlineLevel;
            if (format.PageBreakAfter != PageBreakAfter && !HasKey(PageBreakAfterKey))
                PageBreakAfter = format.PageBreakAfter;
            if (format.PageBreakBefore != PageBreakBefore && !HasKey(PageBreakBeforeKey))
                PageBreakBefore = format.PageBreakBefore;
            if (format.RightIndent != RightIndent && !HasKey(RightIndentKey))
                RightIndent = format.RightIndent;
            if (format.RightIndentBi != RightIndentBi && !HasKey(RightIndentBiKey))
                RightIndentBi = format.RightIndentBi;
            if (format.SpaceAfterAuto != SpaceAfterAuto && !HasKey(SpacingAfterAutoKey))
                SpaceAfterAuto = format.SpaceAfterAuto;
            if (format.SpaceBeforeAuto != SpaceBeforeAuto && !HasKey(SpacingBeforeAutoKey))
                SpaceBeforeAuto = format.SpaceBeforeAuto;
            if (format.SuppressAutoHyphens != SuppressAutoHyphens && !HasKey(SuppressAutoHyphensKey))
                SuppressAutoHyphens = format.SuppressAutoHyphens;
            if (format.TextureStyle != TextureStyle && !HasKey(TextureStyleKey))
                TextureStyle = format.TextureStyle;
            if (format.WidowControl != WidowControl && !HasKey(WidowControlKey))
                WidowControl = format.WidowControl;
            if (format.WrapFrameAround != WrapFrameAround && !HasKey(WrapFrameAroundKey))
                WrapFrameAround = format.WrapFrameAround;
            if (format.LeftIndentChars != LeftIndentChars && !HasKey(LeftIndentCharsKey))
                LeftIndentChars = format.LeftIndentChars;
            if (format.FirstLineIndentChars != FirstLineIndentChars && !HasKey(FirstLineIndentCharsKey))
                FirstLineIndentChars = format.FirstLineIndentChars;
            if (format.RightIndentChars != RightIndentChars && !HasKey(RightIndentCharsKey))
                RightIndentChars = format.RightIndentChars;
            bool istext = (this.OwnerBase is WParagraph) && (this.OwnerBase as WParagraph).Text.Contains("Notice");
            Borders.UpdateSourceFormatting(format.Borders);
            CompareListFormat(format);
        }
        /// <summary>
        /// Compares the list format.
        /// </summary>
        /// <param name="format">The format.</param>
        private void CompareListFormat(WParagraphFormat format)
        {
            //cheks whether list type is No List
            if (format.OwnerBase is WParagraphStyle && (OwnerBase is WParagraph)
                && (format.OwnerBase as WParagraphStyle).ListFormat.ListType != ListType.NoList
                && (format.OwnerBase as WParagraphStyle).ListFormat.CurrentListLevel != null)
            {               
                //Applies left indent value
                if (((format.OwnerBase as WParagraphStyle).ListFormat.CurrentListLevel.ParagraphFormat.HasKey(WParagraphFormat.LeftIndentKey)) && !PropertiesHash.ContainsKey(WParagraphFormat.LeftIndentKey))
                    LeftIndent = (format.OwnerBase as WParagraphStyle).ListFormat.CurrentListLevel.ParagraphFormat.LeftIndent;
                //Applies right indent value
                else if (((format.OwnerBase as WParagraphStyle).ListFormat.CurrentListLevel.ParagraphFormat.HasKey(WParagraphFormat.RightIndentKey)) && !PropertiesHash.ContainsKey(WParagraphFormat.RightIndentKey))
                    RightIndent = (format.OwnerBase as WParagraphStyle).ListFormat.CurrentListLevel.ParagraphFormat.RightIndent;
                //Applies First line indent value
                else if (((format.OwnerBase as WParagraphStyle).ListFormat.CurrentListLevel.ParagraphFormat.HasKey(WParagraphFormat.FirstLineIndentKey)) && !PropertiesHash.ContainsKey(WParagraphFormat.FirstLineIndentKey))
                    FirstLineIndent = (format.OwnerBase as WParagraphStyle).ListFormat.CurrentListLevel.ParagraphFormat.FirstLineIndent;                
             }
        }
        /// <summary>
        /// Determines whether [is value defined] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [is value defined] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValueDefined(int key)
        {
            //Update parent key for composite keys
            while (key > byte.MaxValue)
                key >>= 8;
            return HasValue(key);
        }
        /// <summary>
        /// Removes the value.
        /// </summary>
        /// <param name="key">The key.</param>
        private void RemoveValue(int key)
        {
            PropertiesHash.Remove(key);
            //Update parent key for composite keys
            while (key > byte.MaxValue)
                key >>= 8;
            if (Sprms != null
                && Sprms[GetSprmOption(key)] != null)
                Sprms.RemoveValue(GetSprmOption(key));
        }
        #endregion
    }
}
