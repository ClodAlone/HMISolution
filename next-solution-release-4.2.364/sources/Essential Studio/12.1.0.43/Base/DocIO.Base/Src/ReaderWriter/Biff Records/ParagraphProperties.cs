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

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for CharacterProperty.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ParagraphProperties
    {
        #region Class constants
        internal const int DEF_BORDER_COUNT = 6;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ParagraphProperties m_baseProps = null;
        /// <summary>
        /// 
        /// </summary>
        private ParagraphPropertyException m_papx = null;
        /// <summary>
        /// 
        /// </summary>
        private bool m_stickProperties = true;
        /// <summary>
        /// 
        /// </summary>
        private BorderCode m_topBorder;
        /// <summary>
        /// 
        /// </summary>
        private BorderCode m_leftBorder;
        /// <summary>
        /// 
        /// </summary>
        private BorderCode m_bottomBorder;
        /// <summary>
        /// 
        /// </summary>
        private BorderCode m_rightBorder;
        /// <summary>
        /// 
        /// </summary>
        private BorderCode m_betweenBorder;
        /// <summary>
        /// 
        /// </summary>
        private BorderCode m_barBorder;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        internal ParagraphProperties()
        {
            m_papx = new ParagraphPropertyException();
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal ParagraphProperties(ParagraphPropertyException papx)
        {
            m_papx = papx;
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal ParagraphProperties(ParagraphPropertyException papx,
                                    ParagraphProperties baseParagraphProperties)
        {
            m_papx = papx;
            m_baseProps = baseParagraphProperties;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets/sets the value that represents the paragraph first line indent in character units.
        /// </summary>
        internal short IndentLeftFirstChars
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentLeftFirstChars : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxcLeft1, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxcLeft1, res);
            }
        }
        /// <summary>
        /// Gets/sets the value that represents the paragraph left indent in character units.
        /// </summary>
        internal short IndentLeftChars
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentLeftChars : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxcLeft, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxcLeft, res);
            }
        }
        /// <summary>
        /// Gets/sets the value that represents the paragraph right indent in character units.
        /// </summary>
        internal short IndentRightChars
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentRightChars : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxcRight, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxcRight, res);
            }
        }
        /// <summary>
        /// Gets or sets the base properties.
        /// </summary>
        /// <value>The base properties.</value>
        internal ParagraphProperties BaseProperties
        {
            get
            {
                return m_baseProps;
            }
            set
            {
                m_baseProps = value;
            }
        }
        /// <summary>
        /// Gets sprms
        /// </summary>
        internal SinglePropertyModifierArray Sprms
        {
            get
            {
                return m_papx.PropertyModifiers;
            }
        }
        /// <summary>
        /// Gets ParagraphPropertyException.
        /// </summary>
        internal ParagraphPropertyException ParagraphPropertyException
        {
            get
            {
                return m_papx;
            }
        }
        /// <summary>
        /// Gets/sets justification of a paragraph;
        /// </summary>
        internal ParagraphJustify Justification
        {
            get
            {
                byte defVal =
                  (byte)((m_baseProps != null) ? m_baseProps.Justification : ParagraphJustify.Left);
                return (ParagraphJustify)Sprms.GetByte(WordSprmOptions.sprmPJc, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPJc, (byte)value);
            }
        }
        /// <summary>
        /// True - all lines in the paragraph are to remain on the same page.
        /// </summary>
        internal bool Keep
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Keep : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFKeep, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFKeep, value);
            }
        }
        /// <summary>
        /// True - the paragraph is to remain on the same page as the paragraph that follows it,
        /// if possible
        /// </summary>
        internal bool KeepFollow
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.KeepFollow : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFKeepFollow, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFKeepFollow, value);
            }
        }
        /// <summary>
        /// True - a page break is forced before the paragraph.
        /// </summary>
        internal bool PageBreakBefore
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.PageBreakBefore : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFPageBreakBefore, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFPageBreakBefore, value);
            }
        }
        /// <summary>
        /// Gets/sets the value (in points) that represents the left indent for 
        /// of the first line of the paragraph
        /// </summary>
        internal short IndentLeftFirst
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentLeftFirst : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxaLeft1, defVal);

            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxaLeft1, res);
            }
        }
        /// <summary>
        /// Gets/sets the value (in points) that represents the left indent for a paragraph.
        /// </summary>
        internal short IndentLeft
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentLeft : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxaLeft, defVal);

            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxaLeft, res);
            }
        }
        /// <summary>
        /// Gets/sets the value (in points) that represents the left indent for a paragraph.
        /// </summary>
        internal short IndentRight
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentRight : (short)0;
                short ret = Sprms.GetShort(WordSprmOptions.sprmPDxaRight, defVal);
                //        if( ret == 0 )
                //        {
                //          ret =  Sprms.GetShort( WordSprmOptions.sprmPDxaRight2, defVal );
                //        }
                return ret;
            }
            set
            {
                short res = value;
                //Sprms.SetValue( WordSprmOptions.sprmPDxaRight2, res );
                Sprms.SetValue(WordSprmOptions.sprmPDxaRight, res);
            }
        }
        /// <summary>
        /// Gets/sets the spacing (in points) before the paragraph. 
        /// </summary>
        internal ushort BeforeSpacing
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.BeforeSpacing : (ushort)0;
                return Sprms.GetUShort(WordSprmOptions.sprmPDyaBefore, defVal);

            }
            set
            {
                ushort res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDyaBefore, res);
            }
        }
        /// <summary>
        /// Gets/sets the spacing (in points) after the paragraph.
        /// </summary>
        internal ushort AfterSpacing
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.AfterSpacing : (ushort)0;
                return Sprms.GetUShort(WordSprmOptions.sprmPDyaAfter, defVal);

            }
            set
            {
                ushort res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDyaAfter, res);
            }
        }
        /// <summary>
        /// Gets the shading value.
        /// </summary>
        /// <value>The shading value.</value>
        internal short ShadingValue
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.ShadingValue : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPShd, defVal);
            }
        }
        /// <summary>
        /// Gets/sets the ShadingDescriptor object that refers to the shading 
        /// formatting for the paragraph.
        /// </summary>
        internal ShadingDescriptor Shading
        {
            get
            {
                short shortRes = ShadingValue;
                return new ShadingDescriptor(shortRes);

            }
            set
            {
                short res = value.Save();
                Sprms.SetValue(WordSprmOptions.sprmPShd, res);
            }
        }
        /// <summary>
        /// Gets the shading value.
        /// </summary>
        internal byte[] ShadingNewValue
        {
            get
            {
                byte[] defVal = (m_baseProps != null) ? m_baseProps.ShadingNewValue : null;
                byte[] res = Sprms.GetByteArray(WordSprmOptions.sprmPShdNew);
                return (res != null) ? res : defVal;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ShadingDescriptor ShadingNew
        {
            get
            {
                byte[] res = ShadingNewValue;
                ShadingDescriptor shading = new ShadingDescriptor();

                if (res != null)
                {
                    shading.ReadNewShd(res, 0);
                }
                return shading;
            }
            set
            {
                byte[] res = value.SaveNewShd();
                Sprms.SetValue(WordSprmOptions.sprmPShdNew, res);
            }
        }
        /// <summary>
        /// Gets/sets Mirror Indents
        /// </summary>
        internal bool MirrorIndents
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.MirrorIndents : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFMirrorIndents, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFMirrorIndents, value);
            }
        }
        /// <summary>
        /// Gets/sets widow control
        /// </summary>
        internal bool WidowControl
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.WidowControl : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFWidowControl, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFWidowControl, value);
            }
        }
        /// <summary>
        /// Gets/sets AutoSpaceDN
        /// </summary>
        internal bool AutoSpaceDN
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.AutoSpaceDN : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFAutoSpaceDN, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFAutoSpaceDN, value);
            }
        }
        /// <summary>
        /// Gets/sets AutoSpaceDE
        /// </summary>
        internal bool AutoSpaceDE
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.AutoSpaceDE : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFAutoSpaceDE, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFAutoSpaceDE, value);
            }
        }
        /// <summary>
        /// Gets/sets AdjustRightIndent
        /// </summary>
        internal bool AdjustRightIndent 
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.AdjustRightIndent : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFAdjustRight, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFAdjustRight, value);
            }
        }
        /// <summary>
        /// Copy paragraph properties or not
        /// </summary>
        internal bool StickProperties
        {
            get
            {
                return m_stickProperties;
            }
            set
            {
                m_stickProperties = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short ListFormatIndex
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.ListFormatIndex : (short)-1;
                return Sprms.GetShort(WordSprmOptions.sprmPIlfo, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPIlfo, res);
            }
        }
        /// <summary>
        /// Gets the new index of the list format.
        /// </summary>
        /// <value>The new index of the list format.</value>
        internal short NewListFormatIndex
        {
            get
            {
                SinglePropertyModifierRecord sprm = GetNewSprm(WordSprmOptions.sprmPIlfo);
                if (sprm == null)
                    return short.MaxValue;
                else
                    return sprm.ShortValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte OutlineNumbered
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.OutlineNumbered : byte.MaxValue;
                return Sprms.GetByte(WordSprmOptions.sprmPOutLvl, defVal);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte ListLevelIndex
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.ListLevelIndex : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmPIlvl, defVal);
            }
            set
            {
                byte res = value;
                Sprms.SetValue(WordSprmOptions.sprmPIlvl, res);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte NewListLevelIndex
        {
            get
            {
                SinglePropertyModifierRecord sprm = GetNewSprm(WordSprmOptions.sprmPIlvl);
                if (sprm == null)
                    return byte.MaxValue;
                else
                    return sprm.ByteValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsList
        {
            get
            {
                return (ListFormatIndex > 0 && ListLevelIndex >= 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] ChgTabsPapx
        {
            get
            {
                return Sprms.GetByteArray(WordSprmOptions.sprmPChgTabsPapx);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPChgTabsPapx, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] ChgTabs
        {
            get
            {
                return Sprms.GetByteArray(WordSprmOptions.sprmPChgTabs);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPChgTabs, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TabsInfo TabsInfo
        {
            get
            {
                return new TabsInfo(Sprms, WordSprmOptions.sprmPChgTabsPapx);
            }
            set
            {
                value.Save(Sprms, WordSprmOptions.sprmPChgTabsPapx);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TabsInfo ListTabs
        {
            get
            {
                return new TabsInfo(Sprms, WordSprmOptions.sprmPChgTabs);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal LineSpacingDescriptor LineSpacingDescriptor
        {
            get
            {
                byte[] operand = Sprms.GetByteArray(WordSprmOptions.sprmPDyaLine);
                LineSpacingDescriptor desc = new LineSpacingDescriptor();

                if (operand != null)
                {
                    desc.Parse(operand);
                }

                return desc;
            }
            set
            {
                byte[] operand = value.Save();

                Sprms.SetValue(WordSprmOptions.sprmPDyaLine, operand);
            }
        }
        /// <summary>
        /// Defines whether spacing before is automatic.
        /// </summary>
        /// <value>The spacing before auto.</value>
        internal byte SpacingBeforeAuto
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.SpacingBeforeAuto : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmPFBeforeAuto, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFBeforeAuto, value);
            }
        }
        /// <summary>
        /// Defines whether spacing after is automatic.
        /// </summary>
        /// <value>The spacing before auto.</value>
        internal byte SpacingAfterAuto
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.SpacingAfterAuto : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmPFAfterAuto, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFAfterAuto, value);
            }
        }
        /// <summary>
        /// Gets or sets the outline level.
        /// </summary>
        /// <value>The outline level.</value>
        internal byte OutlineLevel
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.OutlineLevel : byte.MaxValue;
                return Sprms.GetByte(WordSprmOptions.sprmPOutLvl, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPOutLvl, value);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this format is or was changed.
        /// </summary>
        /// <value>
        /// 	 if this format changed format, set to <c>true</c>.
        /// </value>
        internal bool IsChangedFormat
        {
            get
            {
                byte[] data = Sprms.GetByteArray(WordSprmOptions.sprmPPropRMark);
                if (data == null)
                {
                    data = Sprms.GetByteArray(WordSprmOptions.sprmPPropRMark90);
                }

                if (data != null)
                {
                    return (data[0] == 1);
                }
                return false;
            }
            set
            {
                byte[] res = new byte[7];
                res[0] = 1;
                Sprms.SetValue(WordSprmOptions.sprmPPropRMark90, res);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [contextual spacing].
        /// </summary>
        /// <value> if it is contextual spacing, set to <c>true</c>.</value>
        internal bool ContextualSpacing
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.ContextualSpacing : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFContSpacing, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFContSpacing, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to suppress automatic hyphenation for the paragraph.
        /// </summary>
        /// <value><c>true</c> if suppress auto hyphens; otherwise, <c>false</c>.</value>
        internal bool SuppressAutoHyphens
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.SuppressAutoHyphens : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFNoAutoHyph, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFNoAutoHyph, value);
            }
        }
        internal bool WordWrap
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.WordWrap : true;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFWordWrap, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFWordWrap, value);
            }
        }
        #endregion

        #region Class properties/bidi
        /// <summary>
        /// 
        /// </summary>
        internal bool Bidi
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.Bidi : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFBiDi, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFBiDi, value);
            }
        }
        /// <summary>
        /// Gets/sets justification of a paragraph;
        /// </summary>
        internal ParagraphJustify JustificationBidi
        {
            get
            {
                byte defVal =
                  (byte)((m_baseProps != null) ? m_baseProps.JustificationBidi : ParagraphJustify.Right);
                return (ParagraphJustify)Sprms.GetByte(WordSprmOptions.sprmPJcBi, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPJcBi, (byte)value);
            }
        }
        /// <summary>
        /// Gets/sets the value (in points) that represents the left indent for 
        /// of the first line of the bidi paragraph
        /// </summary>
        internal short IndentLeftFirstBi
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentLeftFirstBi : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxaLeft1Bi, defVal);

            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxaLeft1Bi, res);
            }
        }
        /// <summary>
        /// Gets/sets the value (in points) that represents the left indent for a bidi paragraph.
        /// </summary>
        internal short IndentLeftBi
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentLeftBi : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxaLeftBi, defVal);

            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxaLeftBi, res);
            }
        }
        /// <summary>
        /// Gets/sets the value (in points) that represents the left indent for a bidi paragraph.
        /// </summary>
        internal short IndentRightBi
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.IndentRightBi : (short)0;
                short ret = Sprms.GetShort(WordSprmOptions.sprmPDxaRightBi, defVal);
                return ret;
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPDxaRightBi, value);
            }
        }
        #endregion

        #region Class properties/borders
        /// <summary>
        /// Gets/sets top BorderCode object
        /// </summary>
        internal BorderCode TopBorder
        {
            get
            {
                byte[] iRes = new byte[4];
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcTop);
                if (iRes != null)
                {
                    m_topBorder = new BorderCode(iRes, 0);
                }
                else
                {
                    m_topBorder = new BorderCode();
                }

                return m_topBorder;
            }
            set
            {
                if (value != null)
                {
                    m_topBorder = value;
                    byte[] iRes = new byte[4];
                    value.Save(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcTop, iRes);
                }

            }
        }
        /// <summary>
        /// Gets/sets top BorderCode object
        /// </summary>
        internal BorderCode BetweenBorder
        {
            get
            {
                byte[] iRes = new byte[4];
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcBetween);
                if (iRes != null)
                {
                    m_betweenBorder = new BorderCode(iRes, 0);
                }
                else
                {
                    m_betweenBorder = new BorderCode();
                }

                return m_betweenBorder;
            }
            set
            {
                if (value != null)
                {
                    m_betweenBorder = value;
                    byte[] iRes = new byte[4];
                    value.Save(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcBetween, iRes);
                }
            }
        }
        /// <summary>
        /// Gets/sets top BorderCode object
        /// </summary>
        internal BorderCode BarBorder
        {
            get
            {
                byte[] iRes = new byte[4];
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcBar);
                if (iRes != null)
                {
                    m_barBorder = new BorderCode(iRes, 0);
                }
                else
                {
                    m_barBorder = new BorderCode();
                }

                return m_barBorder;
            }
            set
            {
                if (value != null)
                {
                    m_barBorder = value;
                    byte[] iRes = new byte[4];
                    value.Save(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcBar, iRes);
                }
            }
        }
        /// <summary>
        /// Gets/sets bottom BorderCode object
        /// </summary>
        internal BorderCode BottomBorder
        {
            get
            {
                byte[] iRes = new byte[4];
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcBottom);
                if (iRes != null)
                {
                    m_bottomBorder = new BorderCode(iRes, 0);
                }
                else
                {
                    m_bottomBorder = new BorderCode();
                }
                return m_bottomBorder;
            }
            set
            {
                if (value != null)
                {
                    m_bottomBorder = value;
                    byte[] iRes = new byte[4];
                    value.Save(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcBottom, iRes);
                }

            }
        }
        /// <summary>
        /// Gets/sets left BorderCode object
        /// </summary>
        internal BorderCode LeftBorder
        {
            get
            {
                byte[] iRes = new byte[4];
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcLeft);
                if (iRes != null)
                {
                    m_leftBorder = new BorderCode(iRes, 0);
                }
                else
                {
                    m_leftBorder = new BorderCode();
                }

                return m_leftBorder;
            }
            set
            {
                if (value != null)
                {
                    m_leftBorder = value;
                    byte[] iRes = new byte[4];
                    value.Save(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcLeft, iRes);
                }
            }
        }

        /// <summary>
        /// Gets/sets right BorderCode object
        /// </summary>
        internal BorderCode RightBorder
        {
            get
            {
                byte[] iRes = new byte[4];
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcRight);
                if (iRes != null)
                {
                    m_rightBorder = new BorderCode(iRes, 0);
                }
                else
                {
                    m_rightBorder = new BorderCode();
                }

                return m_rightBorder;
            }
            set
            {
                if (value != null)
                {
                    m_rightBorder = value;
                    byte[] iRes = new byte[4];
                    value.Save(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcRight, iRes);
                }

            }
        }
        /// <summary>
        /// Gets/sets top BorderCode object
        /// </summary>
        internal BorderCode TopBorderNew
        {
            get
            {
                byte[] iRes;
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcTopNew);
                if (iRes != null)
                {
                    m_topBorder = new BorderCode();
                    m_topBorder.ParseNewBrc(iRes, 0);
                }
                else
                {
                    m_topBorder = new BorderCode();
                }
                return m_topBorder;
            }
            set
            {
                if (value != null)
                {
                    m_topBorder = value;
                    byte[] iRes = new byte[8];
                    value.SaveNewBrc(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcTopNew, iRes);
                }
            }
        }
        /// <summary>
        /// Gets/sets bottom BorderCode object
        /// </summary>
        internal BorderCode BottomBorderNew
        {
            get
            {
                byte[] iRes;
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcBottomNew);
                if (iRes != null)
                {
                    m_topBorder = new BorderCode();
                    m_topBorder.ParseNewBrc(iRes, 0);
                }
                else
                {
                    m_topBorder = new BorderCode();
                }
                return m_topBorder;
            }
            set
            {
                if (value != null)
                {
                    m_topBorder = value;
                    byte[] iRes = new byte[8];
                    value.SaveNewBrc(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcBottomNew, iRes);
                }
            }
        }
        /// <summary>
        /// Gets/sets left BorderCode object
        /// </summary>
        internal BorderCode LeftBorderNew
        {
            get
            {
                byte[] iRes;
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcLeftNew);
                if (iRes != null)
                {
                    m_topBorder = new BorderCode();
                    m_topBorder.ParseNewBrc(iRes, 0);
                }
                else
                {
                    m_topBorder = new BorderCode();
                }
                return m_topBorder;
            }
            set
            {
                if (value != null)
                {
                    m_topBorder = value;
                    byte[] iRes = new byte[8];
                    value.SaveNewBrc(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcLeftNew, iRes);
                }
            }
        }

        /// <summary>
        /// Gets/sets right BorderCode object
        /// </summary>
        internal BorderCode RightBorderNew
        {
            get
            {
                byte[] iRes;
                iRes = Sprms.GetByteArray(WordSprmOptions.sprmPBrcRightNew);
                if (iRes != null)
                {
                    m_topBorder = new BorderCode();
                    m_topBorder.ParseNewBrc(iRes, 0);
                }
                else
                {
                    m_topBorder = new BorderCode();
                }
                return m_topBorder;
            }
            set
            {
                if (value != null)
                {
                    m_topBorder = value;
                    byte[] iRes = new byte[8];
                    value.SaveNewBrc(iRes, 0);
                    Sprms.SetValue(WordSprmOptions.sprmPBrcRightNew, iRes);
                }
            }
        }
        #endregion

        #region Class properties/tables
        /// <summary>
        /// Is it a cellmark
        /// </summary>
        internal bool IsCellMark
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.IsCellMark : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFInTable, defVal);
            }
            set
            {
                if (value)
                {
                    Sprms.SetValue(WordSprmOptions.sprmPFInTable, value);
                }
                else
                {
                    Sprms.RemoveValue(WordSprmOptions.sprmPFInTable);
                }
            }
        }
        /// <summary>
        /// Is it a rowmark
        /// </summary>
        internal bool IsRowMark
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.IsRowMark : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmPFTtp, defVal) &&
                       Sprms.GetBoolean(WordSprmOptions.sprmPFInTable, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPFTtp, value);
            }
        }
        /// <summary>
        ///  Is called to adjust the x position within a column which marks 
        ///  the left boundary of text within the first cell of a table row.
        /// </summary>
        internal ushort TableXLeft
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.TableXLeft : (ushort)0;
                return Sprms.GetUShort(WordSprmOptions.sprmTDxaLeft, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmTDxaLeft, value);
            }
        }
        /// <summary>
        ///  Gets/sets height of the table row
        /// </summary>
        internal short TableYRowHeight
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.TableYRowHeight : (short)0;
                short rowHeight = (short)Sprms.GetShort(WordSprmOptions.sprmTDyaRowHeight, defVal);
                return (short)rowHeight;
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmTDyaRowHeight, value);
            }
        }
        /// <summary>
        /// Adjusts the white space that is maintained between columns. 
        /// </summary>
        internal ushort TableXGapHalf
        {
            get
            {
                ushort defVal = (m_baseProps != null) ? m_baseProps.TableXGapHalf : (ushort)0;
                return Sprms.GetUShort(WordSprmOptions.sprmTDxaGapHalf, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmTDxaGapHalf, value);
            }
        }
        /// <summary>
        /// Gets/sets TableBorders object that describes a table row borders
        /// </summary>
        internal TableBorders TableBorders
        {
            get
            {
                //        byte[] buf = Sprms.GetByteArray( WordSprmOptions.sprmTTableBorders );
                //        TableBorders tableBorders = new TableBorders();
                //
                //        if( buf == null )
                //        {
                //          buf = new byte[ DEF_BORDER_COUNT * Constants.BytesInInt ];
                //        }
                //        
                //        for( int i = 0; i < DEF_BORDER_COUNT; i++ )
                //        {
                //          tableBorders[ i ] = new BorderCode( buf, i * Constants.BytesInInt );
                //        }
                //
                //        return tableBorders;
                return new TableBorders(Sprms);
            }
            set
            {
                byte[] buf = new byte[DEF_BORDER_COUNT * Constants.BytesInInt];
                TableBorders tableBorders = value;

                for (int i = 0; i < DEF_BORDER_COUNT; i++)
                {
                    tableBorders[i].Save(buf, i * Constants.BytesInInt);
                }

                Sprms.SetValue(WordSprmOptions.sprmTTableBorders, buf);
            }
        }
        /// <summary>
        /// Gets/sets TableBorders object that describes a table row borders
        /// </summary>
        internal TableBorders TableBordersNew
        {
            get
            {
                byte[] buf = Sprms.GetByteArray(WordSprmOptions.sprmTTableBordersNew);
                TableBorders tableBorders = new TableBorders();

                if (buf == null)
                {
                    buf = new byte[DEF_BORDER_COUNT * 8];
                }

                for (int i = 0; i < DEF_BORDER_COUNT; i++)
                {
                    tableBorders[i] = new BorderCode();
                    tableBorders[i].ParseNewBrc(buf, i * 8);
                }

                return tableBorders;
            }
            set
            {
                byte[] buf = new byte[DEF_BORDER_COUNT * 8];
                TableBorders tableBorders = value;

                for (int i = 0; i < DEF_BORDER_COUNT; i++)
                {
                    tableBorders[i].SaveNewBrc(buf, i * 8);
                }

                Sprms.SetValue(WordSprmOptions.sprmTTableBordersNew, buf);
            }
        }
        /// <summary>
        /// Gets/sets table row properies
        /// </summary>
        internal TableRowDescriptor TableRowProperties
        {
            set
            {
                Sprms.SaveTableDescriptor(value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int TablesNestingLevel
        {
            get
            {
                int defValue = 1;
                return Sprms.GetInt(WordSprmOptions.sprmTNestingLevel, defValue);
            }
            set
            {
                if (value > 0)
                {
                    Sprms.SetValue(WordSprmOptions.sprmTNestingLevel, value);
                }
                else
                {
                    Sprms.RemoveValue(WordSprmOptions.sprmTNestingLevel);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsSubCell
        {
            get
            {
                byte defValue = 0;
                return (Sprms.GetByte(WordSprmOptions.sprmPSubTableCellEnd, defValue) == 1);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPSubTableCellEnd, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsSubRow
        {
            get
            {
                return Sprms.GetBoolean(WordSprmOptions.sprmPSubTableRowEnd, false);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPSubTableRowEnd, value);
            }
        }
        /// <summary>
        /// Is it not allow split.
        /// </summary>
        internal bool IsCantSplit
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.IsCantSplit : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmTFCantSplit, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmTFCantSplit, value);
            }
        }
        /// <summary>
        /// Is it not allow split.
        /// </summary>
        internal bool IsCantSplit90
        {
            get
            {
                bool defVal = (m_baseProps != null) ? m_baseProps.IsCantSplit90 : false;
                return Sprms.GetBoolean(WordSprmOptions.sprmTFCantSplit90, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmTFCantSplit90, value);
            }
        }
        /// <summary>
        /// Gets or sets the table alignment.
        /// </summary>
        /// <value>The table alignment.</value>
        internal ParagraphJustify TableAlignment
        {
            get
            {
                short defVal = (short)((m_baseProps != null) ? m_baseProps.TableAlignment : ParagraphJustify.Left);
                return (ParagraphJustify)Sprms.GetShort(WordSprmOptions.sprmTJc, defVal);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmTJc, (short)value);
            }
        }
        /// <summary>
        /// Gets/sets internal width indent.
        /// </summary>
        internal short TableWidthIndent
        {
            get
            {
                short defVal = 0;
                byte[] indentBytes = Sprms.GetByteArray(WordSprmOptions.sprmTWidthIndent);
                if (indentBytes != null && indentBytes.Length == 3)
                {
                    return BitConverter.ToInt16(indentBytes, 1);
                }
                else if (m_baseProps != null)
                    return m_baseProps.TableWidthIndent;

                return defVal;
            }
            set
            {
                byte[] indentBytes = new byte[2];
                indentBytes = BitConverter.GetBytes(value);
                byte[] sprmBytes = new byte[3];
                sprmBytes[0] = 3;
                sprmBytes[1] = indentBytes[0];
                sprmBytes[2] = indentBytes[1];
                Sprms.SetValue(WordSprmOptions.sprmTWidthIndent, sprmBytes);
            }
        }

        #endregion

        #region Class properties/frames
        /// <summary>
        /// 
        /// </summary>
        internal short FrameXCoordinate
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.FrameXCoordinate : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxaAbs, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxaAbs, res);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short FrameYCoordinate
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.FrameYCoordinate : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDyaAbs, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDyaAbs, res);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short FrameWidth
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.FrameWidth : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxaWidth, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxaWidth, res);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte FramePc
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.FramePc : (byte)0;
                return Sprms.GetByte(WordSprmOptions.sprmPPc, defVal);
            }
            set
            {
                byte res = value;
                Sprms.SetValue(WordSprmOptions.sprmPPc, res);
            }
        }
        /// <summary>
        /// Gets the frame vertical pos.
        /// </summary>
        /// <value>The frame vertical pos.</value>
        internal byte FrameVerticalPos
        {
            get
            {
                return (byte)(FramePc >> 4 & 0x03);
            }
            set
            {
                int pc = (byte)FrameHorizontalPos << 2;
                pc = (pc | (byte)value) << 4;
                FramePc = (byte)(pc);
            }
        }
        /// <summary>
        /// Gets the frame horizontal pos.
        /// </summary>
        /// <value>The frame horizontal pos.</value>
        internal byte FrameHorizontalPos
        {
            get
            {
                return (byte)(FramePc >> 6);
            }
            set
            {
                int pc = (byte)value << 2;
                pc = (pc | (byte)FrameVerticalPos) << 4;
                FramePc = (byte)(pc);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsFrame
        {
            get
            {
                byte defVal = 0;
                return (Sprms.GetByte(WordSprmOptions.sprmPWr, defVal) == 2);
            }
            set
            {
                byte res = (byte)((value) ? 2 : 0);
                Sprms.SetValue(WordSprmOptions.sprmPWr, res);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short FrameHeight
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.FrameHeight : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPWHeightAbs, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPWHeightAbs, res);
            }
        }
        /// <summary>
        /// Gets or sets the Wrapping around type of the frame.
         /// </summary>
        /// <value>The vertical distance from text.</value>
        internal byte WrapFrameAround
        {
            get
            {
                byte defVal = (m_baseProps != null) ? m_baseProps.WrapFrameAround : (byte)0;
                return (Sprms.GetByte(WordSprmOptions.sprmPWr, defVal));
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmPWr, value);
            }
        }
        /// <summary>
        /// Gets or sets the frame horizontal distance from text.
        /// </summary>
        /// <value>The frame horizontal distance from text.</value>
        internal short FrameHorizontalDistanceFromText
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.FrameHorizontalDistanceFromText : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDxaFromText, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDxaFromText, res);
            }
        }
        /// <summary>
        /// Gets or sets the frame vertical distance from text.
        /// </summary>
        /// <value>The frame vertical distance from text.</value>
        internal short FrameVerticalDistanceFromText
        {
            get
            {
                short defVal = (m_baseProps != null) ? m_baseProps.FrameVerticalDistanceFromText : (short)0;
                return Sprms.GetShort(WordSprmOptions.sprmPDyaFromText, defVal);
            }
            set
            {
                short res = value;
                Sprms.SetValue(WordSprmOptions.sprmPDyaFromText, res);
            }
        }
        #endregion

        #region Class properties/special
        /// <summary>
        /// 
        /// </summary>
        internal ushort ParagraphStyleId
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmPIstd, 0);
            }
            set
            {
                if (ParagraphStyleId != value)
                {
                    //TODO: validates is stid is correct!
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool PresentRowDescriptor
        {
            get
            {
                return (Sprms[WordSprmOptions.sprmTDefTable] != null);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool PresentTableBorders
        {
            get
            {
                return (Sprms[WordSprmOptions.sprmTTableBorders] != null);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int PTableProps
        {
            get
            {
                return Sprms.GetInt(WordSprmOptions.sprmPTableProps, int.MaxValue);
            }
            set
            {
                if (value == int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("HugePapx3 value is Max.");
                }

                Sprms.SetValue(WordSprmOptions.sprmPTableProps, value);
            }
        }
        #endregion       

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        internal bool HasOptions(int options)
        {
            return Sprms[options] != null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal SinglePropertyModifierArray GetCopiableSprm()
        {
            SinglePropertyModifierArray cloned = new SinglePropertyModifierArray();
            int count = Sprms.Modifiers.Count;
            for (int i = 0; i < count; i++)
            {
                SinglePropertyModifierRecord record = Sprms.GetSprmByIndex(i);
                int options = record.TypedOptions;
                if (options != WordSprmOptions.sprmPFInTable &&
                    options != WordSprmOptions.sprmPFTtp &&
                    options != WordSprmOptions.sprmPIstd &&
                    options != WordSprmOptions.sprmTDefTable &&
                    options != WordSprmOptions.sprmTTableBordersNew &&
                    options != WordSprmOptions.sprmTTableBorders &&
                    options != WordSprmOptions.sprmTDefTableShd &&
                    options != WordSprmOptions.sprmTDyaRowHeight &&
                    options != WordSprmOptions.sprmTCellMargins &&
                    options != WordSprmOptions.sprmTTableCellMargins &&
                    options != WordSprmOptions.sprmPChgTabsPapx &&
                    //            record.TypedOptions != WordSprmOptions.sprmTTlp &&
                    options != WordSprmOptions.sprmTTopBorderColor &&
                    options != WordSprmOptions.sprmTLeftBorderColor &&
                    options != WordSprmOptions.sprmTBottomBorderColor &&
                    options != WordSprmOptions.sprmTRightBorderColor &&
                    options != WordSprmOptions.sprmPTimeStamp &&
                    options != WordSprmOptions.sprmNone &&
                    options != WordSprmOptions.sprmPDyaBefore &&
                    options != WordSprmOptions.sprmPDyaAfter &&
                    options != WordSprmOptions.sprmPUndocumented4458 &&
                    options != WordSprmOptions.sprmPUndocumented4459 &&
                    options != WordSprmOptions.sprmPJc &&
                    //            options != WordSprmOptions.sprmPJcBi &&
                    options != WordSprmOptions.sprmTNestingLevel &&
                    options != WordSprmOptions.sprmPSubTableCellEnd &&
                    options != WordSprmOptions.sprmPDxaRight &&
                    //            options != WordSprmOptions.sprmPDxaRightBi &&
                    //            options != WordSprmOptions.sprmPDxaLeftBi &&
                    //            record.TypedOptions != WordSprmOptions.sprmTableUnknown1 )//&&
                    options != WordSprmOptions.sprmTableUnknown2)
                //            record.TypedOptions != WordSprmOptions.sprmPBrcTop &&
                //            record.TypedOptions != WordSprmOptions.sprmPBrcTopNew &&
                //            record.TypedOptions != WordSprmOptions.sprmPBrcBottom &&
                //            record.TypedOptions != WordSprmOptions.sprmPBrcBottomNew &&
                //            record.TypedOptions != WordSprmOptions.sprmPBrcLeft &&
                //            record.TypedOptions != WordSprmOptions.sprmPBrcLeftNew &&
                //            record.TypedOptions != WordSprmOptions.sprmPBrcRight &&
                //            record.TypedOptions != WordSprmOptions.sprmPBrcRightNew )
                //            options != WordSprmOptions.sprmPIlfo &&
                //            options != WordSprmOptions.sprmPIlvl )
                {
                    cloned.Modifiers.Add(record);
                }
            }
            return cloned;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal SinglePropertyModifierArray GetCopiableTableSprm()
        {
            SinglePropertyModifierArray cloned = new SinglePropertyModifierArray();
            int count = Sprms.Modifiers.Count;
            for (int i = 0; i < count; i++)
            {
                SinglePropertyModifierRecord record = Sprms.GetSprmByIndex(i);
                int options = record.TypedOptions;

                if (record.SprmType == WordSprmType.TableProperties &&
                    options != WordSprmOptions.sprmPFInTable &&
                    options != WordSprmOptions.sprmPFTtp &&
                    options != WordSprmOptions.sprmPIstd &&
                    options != WordSprmOptions.sprmTDefTable &&
                    options != WordSprmOptions.sprmTTableBordersNew &&
                    options != WordSprmOptions.sprmTTableBorders &&
                    options != WordSprmOptions.sprmTDefTableShd &&
                    options != WordSprmOptions.sprmTDyaRowHeight &&
                    options != WordSprmOptions.sprmTCellMargins &&
                    options != WordSprmOptions.sprmTTableCellMargins &&
                    //          options != WordSprmOptions.sprmTTlp &&
                    options != WordSprmOptions.sprmTTopBorderColor &&
                    options != WordSprmOptions.sprmTLeftBorderColor &&
                    options != WordSprmOptions.sprmTBottomBorderColor &&
                    options != WordSprmOptions.sprmTRightBorderColor &&
                    options != WordSprmOptions.sprmPTimeStamp &&
                    options != WordSprmOptions.sprmNone &&
                    options != WordSprmOptions.sprmTCellShdNewDup &&
                    //          record.TypedOptions != WordSprmOptions.sprmSmthColorUnknown1 &&
                    //          record.TypedOptions != WordSprmOptions.sprmTableUnknown2 &&
                    options != WordSprmOptions.sprmTCellSpacing &&
                    options != WordSprmOptions.sprmTNestingLevel &&
                    //          options != WordSprmOptions.sprmTDxaGapHalf &&                 
                    options != WordSprmOptions.sprmTFCantSplit &&
                    options != WordSprmOptions.sprmPSubTableCellEnd &&

                    //options != WordSprmOptions.sprmTCellFitText &&      
                    //  options != WordSprmOptions.sprmTDxaGapHalf &&                 
                    //          options != WordSprmOptions.sprmTPreferredWidth &&                 
                    //          options != WordSprmOptions.sprmTableUnknown4 &&                 
                    //          options != WordSprmOptions.sprmTableUnknown5 &&                 
                    //          options != WordSprmOptions.sprmTableUnknown6 &&                 
                    //          options != WordSprmOptions.sprmTPreferredWidth &&                 
                    //          options != WordSprmOptions.sprmTableUnknown6 &&                 
                    options != WordSprmOptions.sprmTAutoResizeCells)
                {
                    cloned.Modifiers.Add(record);
                }
            }
            return cloned;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal TabsInfo GetTabsInfo(SinglePropertyModifierRecord record)
        {
            return new TabsInfo(record);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal BorderCode GetBorder(SinglePropertyModifierRecord record)
        {
            BorderCode brc;
            byte[] arr = record.ByteArray;

            if (arr.Length == 4)
            {
                brc = new BorderCode(record.ByteArray, 0);
            }
            else
            {
                brc = new BorderCode();
                brc.ParseNewBrc(arr, 0);
            }

            return brc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        internal ShadingDescriptor GetShading(SinglePropertyModifierRecord record)
        {
            ShadingDescriptor shading;
            byte[] arr = record.ByteArray;

            if (arr.Length == 2)
            {
                shading = new ShadingDescriptor(record.ShortValue);
            }
            else
            {
                shading = new ShadingDescriptor();
                shading.ReadNewShd(arr, 0);
            }

            return shading;
        }
        /// <summary>
        /// Gets the line spacing descriptor.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        internal LineSpacingDescriptor GetLineSpacingDescriptor(SinglePropertyModifierRecord record)
        {
            byte[] operand = record.ByteArray;
            LineSpacingDescriptor desc = new LineSpacingDescriptor();

            if (operand != null)
            {
                desc.Parse(operand);
            }

            return desc;
        }
        /// <summary>
        /// Removes the list Single Property Modifier Record Array.
        /// </summary>
        internal void RemoveListSprms()
        {
            int cnt = Sprms.Modifiers.Count;
            for (int i = 0; i < cnt; i++)
            {
                SinglePropertyModifierRecord sprm = Sprms.Modifiers[i];
                if (sprm.TypedOptions == WordSprmOptions.sprmPIlfo || sprm.TypedOptions == WordSprmOptions.sprmPIlvl)
                {
                    Sprms.Modifiers.Remove(sprm);
                    cnt -= 1;
                }
            }
        }
        /// <summary>
        /// Writes the empty list.
        /// </summary>
        internal void WriteEmptyList()
        {
            SinglePropertyModifierRecord sprm = Sprms[(int)WordSprmOptions.sprmPIlfo];
            if (sprm == null)
            {
                sprm = new SinglePropertyModifierRecord(WordSprmOptions.sprmPIlfo);
                Sprms.Modifiers.Add(sprm);
            }
            sprm.ShortValue = 0;

            sprm = Sprms[(int)WordSprmOptions.sprmPIlvl];
            if (sprm == null)
            {
                sprm = new SinglePropertyModifierRecord(WordSprmOptions.sprmPIlvl);
                Sprms.Modifiers.Add(sprm);
            }
            sprm.ShortValue = 0;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Returns object with cloned members
        /// </summary>
        internal ParagraphPropertyException ClonePapx()
        {
            ParagraphPropertyException clonePapx = m_papx;
            m_papx = new ParagraphPropertyException();

            if (StickProperties && clonePapx != null)
            {
                for (int i = 0, len = clonePapx.ModifiersCount; i < len; i++)
                {
                    SinglePropertyModifierRecord modifier = clonePapx.PropertyModifiers.GetSprmByIndex(i);

                    if (modifier.TypedOptions != WordSprmOptions.sprmTDefTable
                        && modifier.TypedOptions != WordSprmOptions.sprmTTableBorders
                        && modifier.TypedOptions != WordSprmOptions.sprmTDxaGapHalf
                        && modifier.TypedOptions != WordSprmOptions.sprmTDxaLeft
                        && modifier.TypedOptions != WordSprmOptions.sprmTDyaRowHeight
                        && modifier.TypedOptions != WordSprmOptions.sprmPFTtp
                        && modifier.TypedOptions != WordSprmOptions.sprmPSubTableCellEnd
                        && modifier.TypedOptions != WordSprmOptions.sprmPSubTableRowEnd
                        //&& modifier.TypedOptions != WordSprmOptions.sprmTNestingLevel
                        //&& modifier.TypedOptions != WordSprmOptions.sprmTDefTableShd
                        //&& modifier.TypedOptions != WordSprmOptions.sprmTDefTableShdNew
                        //&& modifier.TypedOptions != WordSprmOptions.sprmTTableHeader
                        //&& modifier.TypedOptions != WordSprmOptions.sprmTTableBordersNew
                        //&& modifier.TypedOptions != WordSprmOptions.sprmPFInTable
                       )
                    {
                        SinglePropertyModifierRecord sprm = clonePapx.PropertyModifiers.GetSprmByIndex(i).Clone();
                        if (sprm != null)
                        {
                            m_papx.PropertyModifiers.Add(sprm);
                        }
                    }
                }
            }

            return clonePapx;
        }
        
        /// <summary>
        /// Determines whether the specified format has specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns>
        /// <c>true</c> if the specified option has value; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasValue(int option)
        {
            bool defVal = (m_baseProps != null) ? m_baseProps.HasValue(option) : false;
            return (Sprms[option] != null) ? true : defVal;

        }

        /// <summary>
        /// Gets the new (added when track changes property is on) Single Property Modifier Record.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        private SinglePropertyModifierRecord GetNewSprm(int option)
        {
            SinglePropertyModifierRecord sprm = null;

            int index = GetNewPropsStartIndex();
            if (index == -1)
                return sprm;

            for (int i = index, cnt = m_papx.ModifiersCount; i < cnt; i++)
            {
                sprm = m_papx.PropertyModifiers.GetSprmByIndex(i);
                if (sprm.OptionType == (WordSprmOptionType)option)
                {
                    return sprm;
                }
            }

            return null;
        }
        /// <summary>
        /// Gets the index of the new (changed) paragraph properties.
        /// </summary>
        private int GetNewPropsStartIndex()
        {
            SinglePropertyModifierRecord sprm = m_papx.PropertyModifiers[WordSprmOptions.sprmPWall];
            if (sprm != null)
            {
                return m_papx.PropertyModifiers.Modifiers.IndexOf(sprm) + 1;
            }
            return -1;
        }

        public override string ToString()
        {
#if DEBUG_TRACE
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("AfterSpacing : " + this.AfterSpacing.ToString());
            builder.AppendLine("BarBorder : " + this.BarBorder.ToString());
            builder.AppendLine("BeforeSpacing : " + this.BeforeSpacing.ToString());
            builder.AppendLine("BetweenBorder : " + this.BetweenBorder.ToString());
            builder.AppendLine("Bidi : " + this.Bidi.ToString());
            builder.AppendLine("BottomBorder : " + this.BottomBorder.ToString());
            builder.AppendLine("BottomBorderNew : " + this.BottomBorderNew.ToString());
            builder.AppendLine("ContextualSpacing : " + this.ContextualSpacing.ToString());
            builder.AppendLine("FrameDistanceFromText : " + this.FrameDistanceFromText.ToString());
            builder.AppendLine("FrameHeight : " + this.FrameHeight.ToString());
            builder.AppendLine("FrameHorizontalPos : " + this.FrameHorizontalPos.ToString());
            builder.AppendLine("FramePc : " + this.FramePc.ToString());
            builder.AppendLine("FrameVerticalPos : " + this.FrameVerticalPos.ToString());
            builder.AppendLine("FrameWidth : " + this.FrameWidth.ToString());
            builder.AppendLine("FrameXCoordinate : " + this.FrameXCoordinate.ToString());
            builder.AppendLine("FrameYCoordinate : " + this.FrameYCoordinate.ToString());
            builder.AppendLine("HugePapx3 : " + this.HugePapx3.ToString());
            builder.AppendLine("IndentLeft : " + this.IndentLeft.ToString());
            builder.AppendLine("IndentLeftBi : " + this.IndentLeftBi.ToString());
            builder.AppendLine("IndentLeftFirst : " + this.IndentLeftFirst.ToString());
            builder.AppendLine("IndentLeftFirstBi : " + this.IndentLeftFirstBi.ToString());
            builder.AppendLine("IndentRight : " + this.IndentRight.ToString());
            builder.AppendLine("IndentRightBi : " + this.IndentRightBi.ToString());
            builder.AppendLine("IsCantSplit : " + this.IsCantSplit.ToString());
            builder.AppendLine("IsCantSplit90 : " + this.IsCantSplit90.ToString());
            builder.AppendLine("IsCellMark : " + this.IsCellMark.ToString());
            builder.AppendLine("IsChangedFormat : " + this.IsChangedFormat.ToString());
            builder.AppendLine("IsFrame : " + this.IsFrame.ToString());
            builder.AppendLine("IsList : " + this.IsList.ToString());
            builder.AppendLine("IsRowMark : " + this.IsRowMark.ToString());
            builder.AppendLine("IsSubCell : " + this.IsSubCell.ToString());
            builder.AppendLine("IsSubRow : " + this.IsSubRow.ToString());
            builder.AppendLine("Justification : " + this.Justification.ToString());
            builder.AppendLine("JustificationBidi : " + this.JustificationBidi.ToString());
            builder.AppendLine("Keep : " + this.Keep.ToString());
            builder.AppendLine("KeepFollow : " + this.KeepFollow.ToString());
            builder.AppendLine("LeftBorder : " + this.LeftBorder.ToString());
            builder.AppendLine("LeftBorderNew : " + this.LeftBorderNew.ToString());
            builder.AppendLine("LineSpacingDescriptor : " + this.LineSpacingDescriptor.ToString());
            builder.AppendLine("ListFormatIndex : " + this.ListFormatIndex.ToString());
            builder.AppendLine("ListLevelIndex : " + this.ListLevelIndex.ToString());
            //builder.AppendLine("ListTabs : " + this.ListTabs.ToString());
            builder.AppendLine("NewListFormatIndex : " + this.NewListFormatIndex.ToString());
            builder.AppendLine("NewListLevelIndex : " + this.NewListLevelIndex.ToString());
            builder.AppendLine("OutlineLevel : " + this.OutlineLevel.ToString());
            builder.AppendLine("OutlineNumbered : " + this.OutlineNumbered.ToString());
            builder.AppendLine("PageBreakBefore : " + this.PageBreakBefore.ToString());
            builder.AppendLine("ParagraphStyleId : " + this.ParagraphStyleId.ToString());
            builder.AppendLine("PresentRowDescriptor : " + this.PresentRowDescriptor.ToString());
            builder.AppendLine("PresentTableBorders : " + this.PresentTableBorders.ToString());
            builder.AppendLine("RightBorder : " + this.RightBorder.ToString());
            builder.AppendLine("RightBorderNew : " + this.RightBorderNew.ToString());
            builder.AppendLine("Shading : " + this.Shading.ToString());
            builder.AppendLine("ShadingNew : " + this.ShadingNew.ToString());
            //builder.AppendLine("ShadingNewValue : " + this.ShadingNewValue.ToString());
            builder.AppendLine("SpacingAfterAuto : " + this.SpacingAfterAuto.ToString());
            builder.AppendLine("SpacingBeforeAuto : " + this.SpacingBeforeAuto.ToString());
            builder.AppendLine("Sprms : " + this.Sprms.Count.ToString());
            builder.AppendLine("StickProperties : " + this.StickProperties.ToString());
            builder.AppendLine("TableAlignment : " + this.TableAlignment.ToString());
            builder.AppendLine("TableBorders : " + this.TableBorders.ToString());
            builder.AppendLine("TableBordersNew : " + this.TableBordersNew.ToString());
            //builder.AppendLine("TableRowProperties : " + this.TableRowProperties.ToString());
            builder.AppendLine("TablesNestingLevel : " + this.TablesNestingLevel.ToString());
            builder.AppendLine("TableWidthIndent : " + this.TableWidthIndent.ToString());
            builder.AppendLine("TableXGapHalf : " + this.TableXGapHalf.ToString());
            builder.AppendLine("TableXLeft : " + this.TableXLeft.ToString());
            builder.AppendLine("TableYRowHeight : " + this.TableYRowHeight.ToString() ?? string.Empty);
            //builder.AppendLine("TabsInfo : " + this.TabsInfo.ToString());
            builder.AppendLine("TopBorder : " + this.TopBorder.ToString());
            builder.AppendLine("TopBorderNew : " + this.TopBorderNew.ToString());
            builder.AppendLine("WidowControl : " + this.WidowControl.ToString());

            return builder.ToString();
            
#else
            return base.ToString();
#endif

        }
        #endregion
    }
}
