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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for SectionProperties.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SectionProperties
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private SectionPropertyException m_sepx;

        private ColumnArray m_columnsArray = null;

        /// <summary>
        /// 
        /// </summary>
        private bool m_stickProperties = true;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        internal SectionProperties()
        {
            m_sepx = new SectionPropertyException();
            m_columnsArray = new ColumnArray(Sprms);
            //m_columnsArray.AddColumn();
        }

        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal SectionProperties(SectionPropertyException sepx)
        {
            m_sepx = sepx;

            m_columnsArray = new ColumnArray(Sprms);
            m_columnsArray.ReadColumnsProperties();
        }
        #endregion

        #region Class properties
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
        internal SinglePropertyModifierArray Sprms
        {
            get
            {
                return m_sepx.Properties;
            }
        }
        /// <summary>
        /// Gets/sets the space between the header and the top of the page. 
        /// </summary>
        internal short HeaderHeight
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmSDyaHdrTop, -1);
            }
            set
            {
                if (value != -1)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDyaHdrTop, value);
                }
                else
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDyaHdrTop, (short)720);
                }
            }
        }
        /// <summary>
        /// Gets/sets the space between the footer and the bottom of the page. 
        /// </summary>
        internal short FooterHeight
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmSDyaHdrBottom, -1);
            }
            set
            {
                if (value != -1)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDyaHdrBottom, value);
                }
                else
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDyaHdrBottom, (short)720);
                }
            }
        }
        /// <summary>
        /// Gets/sets whether a title page is to be displayed
        /// </summary>
        internal bool TitlePage
        {
            get
            {
                return Sprms.GetBoolean(WordSprmOptions.sprmSFTitlePage, false);
            }
            set
            {
                if (value != false)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSFTitlePage, value);
                }
            }
        }
        /// <summary>
        /// Gets / sets break code
        /// </summary>
        internal byte BreakCode
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSBkc, 2);
            }
            set
            {
                //if( value != 2 )
                {
                    Sprms.SetValue(WordSprmOptions.sprmSBkc, value);
                }
            }
        }
        /// <summary>
        /// Gets / sets text direction
        /// </summary>
        internal byte TextDirection
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSTextFlow, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSTextFlow, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ColumnArray Columns
        {
            get
            {
                return m_columnsArray;
            }
        }
        /// <summary>
        /// Gets/sets the distance between the bottom edge of the page and 
        /// the bottom boundary of the text. 
        /// </summary>
        internal short BottomMargin
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmSDyaBottom, -1);
            }
            set
            {
                if (value != -1)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDyaBottom, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets the distance between the top edge of the page and 
        /// the top boundary of the text.
        /// </summary>
        internal short TopMargin
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmSDyaTop, -1);
            }
            set
            {
                if (value != -1)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDyaTop, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets the distance between the left edge of the page and 
        /// the left boundary of the text.
        /// </summary>
        internal short LeftMargin
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmSDxaLeft, -1);
            }
            set
            {
                if (value != -1)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDxaLeft, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets the distance between the right edge of the page and 
        /// the right boundary of the text.
        /// </summary>
        internal short RightMargin
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmSDxaRight, -1);
            }
            set
            {
                if (value != -1)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDxaRight, value);
                }
            }
        }
        /// <summary>
        /// Orientation of pages in that section. Set page size firstly! 
        /// Set to 0 - when portrait, 2 - when landscape
        /// </summary>
        internal byte Orientation
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSBOrientation, 0);
            }
            set
            {
                //        switch ( value )
                //        {
                //          case 2:
                //            if ( Orientation == 0 )
                //            {
                //              ushort ch = PageHeight;
                //              PageHeight = PageWidth;
                //              PageWidth = ch;
                //            }
                //            Sprms.SetValue( WordSprmOptions.sprmSBOrientation, value );
                //            break;
                //          case 0:
                //            if ( Orientation == 2 )
                //            {
                //              ushort ch = PageHeight;
                //              PageHeight = PageWidth;
                //              PageWidth = ch;
                //            }
                //            Sprms.SetValue( WordSprmOptions.sprmSBOrientation, value );
                //            break;
                //          default:
                Sprms.SetValue(WordSprmOptions.sprmSBOrientation, value);
                //            break;
                //        }
            }

        }
        /// <summary>
        /// Gets/sets the height of the page in twips. 
        /// </summary>
        internal ushort PageHeight
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmSYaPage, 0);
            }
            set
            {
                if (value > 0)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSYaPage, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets the width of the page in twips. 
        /// </summary>
        internal ushort PageWidth
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmSXaPage, 0);
            }
            set
            {
                if (value > 0)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSXaPage, value);
                }
            }
        }
        /// <summary>
        /// Vertical justification code
        ///       0 top justified
        ///       1 centered
        ///       2 fully justified vertically 
        ///       3 bottom justified 
        /// </summary>
        internal byte VerticalAlignment
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSVjc, 0);
            }
            set
            {
                if (value >= 0)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSVjc, value);
                }
            }
        }
        /// <summary>
        /// Gets/sets the size of the page gutter.
        /// </summary>
        internal short Gutter
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmSDzaGutter, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSDzaGutter, value);
            }
        }
        /// <summary>
        /// Gets / sets whether section contains right-to-left text.
        /// </summary>
        internal bool Bidi
        {
            get
            {
                return Sprms.GetBoolean(WordSprmOptions.sprmSFBiDi, false);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSFBiDi, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte PageNfc
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSNfcPgn, 0);
            }
            set
            {
                if (value >= 0)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSNfcPgn, value);
                }
            }
        }
        /// <summary>
        /// Gets or sets the value page start at.
        /// </summary>
        /// <value>The page start at.</value>
        internal ushort PageStartAt
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmSPgnStart, 0);
            }
            set
            {
                if (value >= 0)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSPgnStart, value);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to restart page.
        /// </summary>
        /// <value> if page restart, set to <c>true</c>.</value>
        internal bool PageRestart
        {
            get
            {
                return Sprms.GetBoolean(WordSprmOptions.sprmSFPgnRestart, false);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSFPgnRestart, value);
            }
        }
        /// <summary>
        /// Gets or sets the line pitch.
        /// </summary>
        /// <value>The line pitch.</value>
        internal ushort LinePitch
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmSDyaLinePitch, 0);
            }
            set
            {
                if (value >= 0)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSDyaLinePitch, value);
                }
            }
        }
        /// <summary>
        /// Gets or sets the type of the pitch.
        /// </summary>
        /// <value>The type of the pitch.</value>
        internal ushort PitchType
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmSClm, 0);
            }
            set
            {
                if (value > 0 && value < 4)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSClm, value);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to draw lines between columns.
        /// </summary>
        /// <value>
        /// 	if draw lines between columns, set to <c>true</c>.
        /// </value>
        internal bool DrawLinesBetweenCols
        {
            get
            {
                return Sprms.GetBoolean(WordSprmOptions.sprmSLBetween, false);
            }
            set
            {
                if (value)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSLBetween, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [form protect].
        /// </summary>
        /// <value><c>true</c> if [form protect]; otherwise, <c>false</c>.</value>
        internal bool ProtectForm
        {
            get
            {
                return Sprms.GetBoolean(WordSprmOptions.sprmSFProtected, false);
            }
            set
            {
                if (value)
                {
                    Sprms.SetValue(WordSprmOptions.sprmSFProtected, false);
                }
                else
                {
                    Sprms.SetValue(WordSprmOptions.sprmSFProtected, true);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating  [Chapter numbering seprator in page number].
        /// </summary>
        /// <value>
        /// Chapter Page Separator 
        /// </value>
        internal byte ChapterPageSeparator
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmScnsPgn, 0);
            }
            set
            {                
                Sprms.SetValue(WordSprmOptions.sprmScnsPgn,value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating  [Chapter numbering level in page number].
        /// </summary>
        /// <value>
        /// Chapter Heading Level
        /// </value>
        internal byte HeadingLevelForChapter
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSiHeadingPgn, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSiHeadingPgn, value);
            }
        }
        #endregion

        #region Class properties / line numbering
        /// <summary>
        /// 
        /// </summary>
        internal LineNumberingMode LineNumberingMode
        {
            get
            {
                return (LineNumberingMode)Sprms.GetByte(WordSprmOptions.sprmSLnc, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSLnc, (byte)value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort LineNumberingStep
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmSNLnnMod, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSNLnnMod, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short LineNumberingStartValue
        {
            get
            {
                return (short)(Sprms.GetShort(WordSprmOptions.sprmSLnnMin, 0) + 1);
            }
            set
            {
                short res = (short)(value - 1);
                Sprms.SetValue(WordSprmOptions.sprmSLnnMin, res);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short LineNumberingDistanceFromText
        {
            get
            {
                return Sprms.GetShort(WordSprmOptions.sprmSDxaLnn, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSDxaLnn, value);
            }
        }
        #endregion

        #region Class properties / borders
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode LeftBorder
        {
            get
            {
                byte[] arr = Sprms.GetByteArray(WordSprmOptions.sprmSBrcLeft);

                if (arr != null)
                {
                    return new BorderCode(arr, 0);
                }
                else
                {
                    return new BorderCode();
                }
            }
            set
            {
                byte[] arr = new byte[4];
                value.Save(arr, 0);
                Sprms.SetValue(WordSprmOptions.sprmSBrcLeft, arr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode TopBorder
        {
            get
            {
                byte[] arr = Sprms.GetByteArray(WordSprmOptions.sprmSBrcTop);

                if (arr != null)
                {
                    return new BorderCode(arr, 0);
                }
                else
                {
                    return new BorderCode();
                }
            }
            set
            {
                byte[] arr = new byte[4];
                value.Save(arr, 0);
                Sprms.SetValue(WordSprmOptions.sprmSBrcTop, arr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode RightBorder
        {
            get
            {
                byte[] arr = Sprms.GetByteArray(WordSprmOptions.sprmSBrcRight);

                if (arr != null)
                {
                    return new BorderCode(arr, 0);
                }
                else
                {
                    return new BorderCode();
                }
            }
            set
            {
                byte[] arr = new byte[4];
                value.Save(arr, 0);
                Sprms.SetValue(WordSprmOptions.sprmSBrcRight, arr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode BottomBorder
        {
            get
            {
                byte[] arr = Sprms.GetByteArray(WordSprmOptions.sprmSBrcBottom);

                if (arr != null)
                {
                    return new BorderCode(arr, 0);
                }
                else
                {
                    return new BorderCode();
                }
            }
            set
            {
                byte[] arr = new byte[4];
                value.Save(arr, 0);
                Sprms.SetValue(WordSprmOptions.sprmSBrcBottom, arr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode LeftBorderNew
        {
            get
            {
                byte[] arr = Sprms.GetByteArray(WordSprmOptions.sprmSBrcLeftNew);

                if (arr != null)
                {
                    BorderCode border = new BorderCode();
                    border.ParseNewBrc(arr, 0);
                    return border;
                }
                else
                {
                    return new BorderCode();
                }
            }
            set
            {
                byte[] arr = new byte[8];
                value.SaveNewBrc(arr, 0);
                Sprms.SetValue(WordSprmOptions.sprmSBrcLeftNew, arr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode TopBorderNew
        {
            get
            {
                byte[] arr = Sprms.GetByteArray(WordSprmOptions.sprmSBrcTopNew);

                if (arr != null)
                {
                    BorderCode border = new BorderCode();
                    border.ParseNewBrc(arr, 0);
                    return border;
                }
                else
                {
                    return new BorderCode();
                }
            }
            set
            {
                byte[] arr = new byte[8];
                value.SaveNewBrc(arr, 0);
                Sprms.SetValue(WordSprmOptions.sprmSBrcTopNew, arr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode RightBorderNew
        {
            get
            {
                byte[] arr = Sprms.GetByteArray(WordSprmOptions.sprmSBrcRightNew);

                if (arr != null)
                {
                    BorderCode border = new BorderCode();
                    border.ParseNewBrc(arr, 0);
                    return border;
                }
                else
                {
                    return new BorderCode();
                }
            }
            set
            {
                byte[] arr = new byte[8];
                value.SaveNewBrc(arr, 0);
                Sprms.SetValue(WordSprmOptions.sprmSBrcRightNew, arr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal BorderCode BottomBorderNew
        {
            get
            {
                byte[] arr = Sprms.GetByteArray(WordSprmOptions.sprmSBrcBottomNew);

                if (arr != null)
                {
                    BorderCode border = new BorderCode();
                    border.ParseNewBrc(arr, 0);
                    return border;
                }
                else
                {
                    return new BorderCode();
                }
            }
            set
            {
                byte[] arr = new byte[8];
                value.SaveNewBrc(arr, 0);
                Sprms.SetValue(WordSprmOptions.sprmSBrcBottomNew, arr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal PageBordersApplyType PageBorderApply
        {
            get
            {
                short pgbProp = Sprms.GetShort(WordSprmOptions.sprmSPgbProp, 0);
                return (PageBordersApplyType)(pgbProp & 0x0007);
            }
            set
            {
                short pgbProp = Sprms.GetShort(WordSprmOptions.sprmSPgbProp, 0);
                short res = (short)value;

                pgbProp = (short)((pgbProp & 0xfff8) + res);
                if (pgbProp != 0)
                    Sprms.SetValue(WordSprmOptions.sprmSPgbProp, pgbProp);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool PageBorderIsInFront
        {
            get
            {
                short pgbProp = Sprms.GetShort(WordSprmOptions.sprmSPgbProp, 0);
                return ((pgbProp & 0x0018) >> 3) != 1;
            }
            set
            {
                short pgbProp = Sprms.GetShort(WordSprmOptions.sprmSPgbProp, 0);
                short res = (short)((!value ? 1 : 0) << 3);
                pgbProp = (short)((pgbProp & 0xffe7) + res);
                if (pgbProp != 0)
                    Sprms.SetValue(WordSprmOptions.sprmSPgbProp, pgbProp);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal PageBorderOffsetFrom PageBorderOffsetFrom
        {
            get
            {
                short pgbProp = Sprms.GetShort(WordSprmOptions.sprmSPgbProp, 0);
                return (PageBorderOffsetFrom)((pgbProp & 0x00e0) >> 5);
            }
            set
            {
                short pgbProp = Sprms.GetShort(WordSprmOptions.sprmSPgbProp, 0);
                short res = (short)((short)(value) << 5);
                pgbProp = (short)((pgbProp & 0xff1f) + res);
                if (pgbProp != 0)
                    Sprms.SetValue(WordSprmOptions.sprmSPgbProp, pgbProp);
            }
        }
        #endregion

        #region Class properties / columns
        /// <summary>
        /// 
        /// </summary>
        internal ushort ColumnsCount
        {
            get
            {
                return (ushort)(Sprms.GetUShort(WordSprmOptions.sprmSCcolumns, 0) + 1);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSCcolumns, (ushort)(value - 1));
            }
        }
        #endregion       
        #region Class properties / Footnote/Endnote
        /// <summary>
        /// Gets / sets endnote numbering format
        /// </summary>
        internal ushort EndnoteNumberFormat
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmSNfcEdnRef, 2);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSNfcEdnRef, value);
            }
        }
        /// <summary>
        /// Gets / sets footnote numbering format
        /// </summary>
        internal ushort FootnoteNumberFormat
        {
            get
            {
                return Sprms.GetUShort(WordSprmOptions.sprmSNfcFtnRef, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSNfcFtnRef, value);
            }
        }
        /// <summary>
        /// Gets / sets the restart index for endnote
        /// </summary>
        internal byte RestartIndexForEndnote
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSRncEdn, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSRncEdn, value);
            }
        }
        /// <summary>
        /// Gets / sets the restart index for footnotes
        /// </summary>
        internal byte RestartIndexForFootnotes
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSRncFtn, 0);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSRncFtn, value);
            }
        }
        /// <summary>
        /// Gets / sets footnote position in the document
        /// </summary>
        internal byte FootnotePosition
        {
            get
            {
                return Sprms.GetByte(WordSprmOptions.sprmSFpc, 1);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSFpc, value);
            }
        }
        /// <summary>
        /// Gets / sets the initial footnote number
        /// </summary>
        internal ushort InitialFootnoteNumber
        {
            get
            {
                return (ushort)(Sprms.GetUShort(WordSprmOptions.sprmSNFtn, 0) + 1);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSNFtn, (ushort)(value - 1));
            }
        }
        /// <summary>
        /// Gets / sets the initial endnote number
        /// </summary>
        internal ushort InitialEndnoteNumber
        {
            get
            {
                return (ushort)(Sprms.GetUShort(WordSprmOptions.sprmSNEdn, 0) + 1);
            }
            set
            {
                Sprms.SetValue(WordSprmOptions.sprmSNEdn, (ushort)(value - 1));
            }
        }
        #endregion
        #region Class internal
        /// <summary>
        /// Returns object with cloned members
        /// </summary>
        /// <returns></returns>
        internal SectionPropertyException CloneSepx()
        {
            SectionPropertyException cloneSepx = m_sepx;
            m_sepx = new SectionPropertyException();

            if (StickProperties)
            {
                for (int i = 0, len = cloneSepx.Properties.Count; i < len; i++)
                {
                    SinglePropertyModifierRecord sprm = cloneSepx.Properties.GetSprmByIndex(i).Clone();
                    if (sprm != null)
                    {
                        m_sepx.Properties.Add(sprm);
                    }
                }
            }

            return cloneSepx;
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
                if (options != WordSprmOptions.sprmSCcolumns &&
                  options != WordSprmOptions.sprmSXaPage &&
                  options != WordSprmOptions.sprmSDxaLeft &&
                  options != WordSprmOptions.sprmSDxaRight &&
                  options != WordSprmOptions.sprmSDxaColWidth &&
                  options != WordSprmOptions.sprmSDyaHdrTop &&
                  options != WordSprmOptions.sprmSDyaHdrBottom &&
                  options != WordSprmOptions.sprmSVjc &&
                  options != WordSprmOptions.sprmSBOrientation &&
                  options != WordSprmOptions.sprmSDyaBottom &&
                  options != WordSprmOptions.sprmSDyaTop &&
                  options != WordSprmOptions.sprmNone)
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
        #endregion
    }
}