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
using System.Collections;

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for MainStatePositions.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class MainStatePositions : StatePositionsBase
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_iEndSecPos;

        private int m_iCurrentSepxIndex = 0;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int SectionIndex
        {
            get
            {
                return m_iCurrentSepxIndex;
            }
        }

        /// <summary>
        /// Gets current section properties exception
        /// </summary>
        /// <returns></returns>
        internal SectionPropertyException CurrentSepx
        {
            get
            {
                return m_fkp.GetSepx(m_iCurrentSepxIndex);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Creates object and fills it's members from fkp
        /// </summary>
        internal MainStatePositions(WordFKPData fkp)
            : base(fkp)
        {
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Move to next section properties exception
        /// </summary>
        /// <param name="iEndPos"></param>
        /// <returns></returns>
        internal bool NextSepx(out int iEndPos)
        {
            bool bOk = true;

            if (m_iCurrentSepxIndex < Tables.SectionsTable.Positions.Length - 2)
            {
                m_iCurrentSepxIndex++;
            }
            else
            {
                bOk = false;
            }

            iEndPos = Tables.SectionsTable.Positions[m_iCurrentSepxIndex + 1];
            return bOk;
        }

        /// <summary>
        /// Update end position of section
        /// </summary>
        /// <param name="iEndPos"></param>
        internal bool UpdateSepxEndPos(long iEndPos)
        {
            bool bOk = false;
            if (iEndPos >= m_iEndSecPos)
            {
                int iEndSecPos;
                if (NextSepx(out iEndSecPos))
                {
                    ////          m_iEndSecPos = iEndSecPos + m_iStartText;
                    m_iEndSecPos = (int)Tables.ConvertCharPosToFileCharPos((uint)iEndSecPos);
                    //// After changed curren section properties - 
                    //// also changed SectionNumber property.
                    bOk = true;
                }
                else
                {
                    m_iEndSecPos = -1;
                }
            }

            return bOk;
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void InitStartEndPos()
        {
            base.InitStartEndPos();
            ////m_iEndSecPos = Tables.SectionsTable.Positions[ 1 ] * m_fkp.FIBData.EncodingCharSize + m_iStartText;
            m_iEndSecPos = (int)Tables.ConvertCharPosToFileCharPos((uint)Tables.SectionsTable.Positions[1]);
            ////m_iEndSecPos = Tables.SectionsTable.Positions[ 1 ] + m_iStartText;
        }

        /// <summary>
        /// Gets min end position
        /// </summary>
        /// <returns></returns>
        internal override long GetMinEndPos(long curPos)
        {
            return Math.Min(base.GetMinEndPos(curPos), m_iEndSecPos);
        }
        #endregion
    }

    /// <summary>
    /// Summary description for HFStatePositions
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class FootnoteStatePositions : StatePositionsBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal FootnoteStatePositions(WordFKPData fkp)
            : base(fkp)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        internal override int MoveToItem(int itemIndex)
        {
            m_iItemIndex = itemIndex;

            int start = m_fkp.FIBData.ccpText;
            if (m_iStartItemPos == 0)
            {
                m_iStartItemPos =
                  (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)start));
                m_iEndText =
                  (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)(start + m_fkp.FIBData.ccpFtn)));
            }

            m_iStartText = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                  (uint)(start + m_fkp.Tables.Footnotes.GetTxtPosition(m_iItemIndex))));

            m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                  (uint)(start + m_fkp.Tables.Footnotes.GetTxtPosition(m_iItemIndex + 1))));

            MoveToCurrentChpxPapx();

            return m_iStartText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEndPos"></param>
        /// <returns></returns>
        internal override bool UpdateItemEndPos(long iEndPos)
        {
            if (iEndPos >= m_iEndItemPos)
            {
                uint start = (uint)(m_fkp.FIBData.ccpText);
                m_iItemIndex++;
                m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                    (uint)(start + m_fkp.Tables.Footnotes.GetTxtPosition(m_iItemIndex + 1))));

                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for HFStatePositions
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class HFStatePositions : StatePositionsBase
    {
        #region Class members
        private int m_iSectionIndex = 0;
        private bool m_isNextItemText;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int SectionIndex
        {
            get
            {
                return m_iSectionIndex;
            }

            set
            {
                ////        int secEnd = m_fkp.Tables.SectionsTable.EntriesCount;
                ////        
                ////        if( value < 0 || value >= secEnd )
                ////        {
                ////          throw new ArgumentOutOfRangeException( string.Format(
                ////            "value must be between 1 and {0}", secEnd ) );
                ////        }

                m_iSectionIndex = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        ///  Initializing constructor
        /// </summary>
        internal HFStatePositions(WordFKPData fkp)
            : base(fkp)
        {
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        internal override int MoveToItem(int itemIndex)
        {
            m_iItemIndex = m_iSectionIndex * 6 + itemIndex;

            int startHeader = m_fkp.FIBData.ccpText + m_fkp.FIBData.ccpFtn;

            if (m_iStartItemPos == 0)
            {
                m_iStartItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)startHeader));
                m_iEndText = (int)m_fkp.Tables.ConvertCharPosToFileCharPos((uint)(startHeader + m_fkp.FIBData.ccpHdr));
            }

            m_iStartText = (int)m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(startHeader + m_fkp.Tables.HeaderFooterCharPosTable.Positions[m_iItemIndex]));

            m_iEndItemPos = (int)m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(startHeader + m_fkp.Tables.HeaderFooterCharPosTable.Positions[m_iItemIndex + 1]));

            MoveToCurrentChpxPapx();

            return m_iStartText;
        }

        /// <summary>
        /// Move to next header/footer position
        /// </summary>
        internal void MoveToNextHeaderPos()
        {
            m_iItemIndex++;
            int startHeader = m_fkp.FIBData.ccpText + m_fkp.FIBData.ccpFtn;

            m_iEndItemPos = (int)m_fkp.Tables.ConvertCharPosToFileCharPos((uint)(
              startHeader + m_fkp.Tables.HeaderFooterCharPosTable.Positions[m_iItemIndex + 1]));

            ////      m_iEndHeaderPos = Tables.HeaderFooterCharPosTable.Positions[m_iHeaderIndex + 1 ] * maxByteCount
            ////        + m_iStartHeaderPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEndPos"></param>
        /// <param name="headerType"></param>
        /// <returns></returns>
        internal bool UpdateHeaderEndPos(long iEndPos, HeaderType headerType)
        {
            ////      m_isNextItemText = ( iEndPos  >= m_iEndItemPos && ( int )headerType < 5 ) ? true : false;
            ////      if( iEndPos >= m_iEndItemPos )      
            ////      {
            ////        if( ( int )headerType < 5 )
            ////        {
            ////          headerType++;
            ////          MoveToNextHeaderPos();
            ////          return true;
            ////        }
            ////        else
            ////        {
            ////          m_iEndItemPos = -1;
            ////        }
            ////      }
            ////      return false;

            m_isNextItemText = (iEndPos >= m_iEndItemPos && (int)headerType < 5) ? true : false;
            if (m_isNextItemText)
            {
                headerType++;
                MoveToNextHeaderPos();
            }
            else if (iEndPos >= m_iEndItemPos)
            {
                m_iEndItemPos = -1;
            }
            return m_isNextItemText;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iPos"></param>
        /// <returns></returns>
        internal override bool IsEndOfSubdocItemText(long iPos)
        {
            return m_isNextItemText;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    internal class AtnStatePositions : StatePositionsBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal AtnStatePositions(WordFKPData fkp)
            : base(fkp)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        internal override int MoveToItem(int itemIndex)
        {
            m_iItemIndex = itemIndex;

            int start = m_fkp.FIBData.ccpText + m_fkp.FIBData.ccpFtn + m_fkp.FIBData.ccpHdr;
            if (m_iStartItemPos == 0)
            {
                m_iStartItemPos =
                  (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)start));
                m_iEndText =
                  (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)(start + m_fkp.FIBData.ccpAtn)));
            }

            m_iStartText = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(start + m_fkp.Tables.Annotations.GetTxtPosition(m_iItemIndex))));

            m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(start + m_fkp.Tables.Annotations.GetTxtPosition(m_iItemIndex + 1))));

            MoveToCurrentChpxPapx();

            return m_iStartText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEndPos"></param>
        /// <returns></returns>
        internal override bool UpdateItemEndPos(long iEndPos)
        {
            if (iEndPos >= m_iEndItemPos)
            {
                uint start = (uint)(m_fkp.FIBData.ccpText
                  + m_fkp.FIBData.ccpFtn
                  + m_fkp.FIBData.ccpHdr);
                m_iItemIndex++;
                m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                    (uint)(start + m_fkp.Tables.Annotations.GetTxtPosition(m_iItemIndex + 1))));

                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for HFStatePositions
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class EndnoteStatePositions : StatePositionsBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal EndnoteStatePositions(WordFKPData fkp)
            : base(fkp)
        {
        }
        #endregion

        #region Class internal overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        internal override int MoveToItem(int itemIndex)
        {
            m_iItemIndex = itemIndex;

            int start = m_fkp.FIBData.ccpText
              + m_fkp.FIBData.ccpFtn
              + m_fkp.FIBData.ccpHdr
              + m_fkp.FIBData.ccpAtn;
            if (m_iStartItemPos == 0)
            {
                m_iStartItemPos =
                  (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)start));
                m_iEndText =
                  (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)(start + m_fkp.FIBData.ccpEdn)));
            }

            m_iStartText = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(start + m_fkp.Tables.Endnotes.GetTxtPosition(m_iItemIndex))));
            
            m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(start + m_fkp.Tables.Endnotes.GetTxtPosition(m_iItemIndex + 1))));

            MoveToCurrentChpxPapx();

            return m_iStartText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEndPos"></param>
        /// <returns></returns>
        internal override bool UpdateItemEndPos(long iEndPos)
        {
            if (iEndPos >= m_iEndItemPos)
            {
                uint start = (uint)(m_fkp.FIBData.ccpText
                  + m_fkp.FIBData.ccpFtn
                  + m_fkp.FIBData.ccpHdr
                  + m_fkp.FIBData.ccpAtn);
                m_iItemIndex++;
                m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                    (uint)(start + m_fkp.Tables.Endnotes.GetTxtPosition(m_iItemIndex + 1))));

                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// Main doc textbox state position class
    /// </summary>
    [CLSCompliant(false)]
    internal class TextBoxStatePositions : StatePositionsBase
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxStatePositions"/> class.
        /// </summary>
        /// <param name="fkp"></param>
        internal TextBoxStatePositions(WordFKPData fkp)
            : base(fkp)
        {
        }
        #endregion

        #region Class overrides
        ////    /// <summary>
        ////    /// Initialize the write.
        ////    /// 
        ////    /// </summary>
        ////    /// <returns>Returns textbox start write position</returns>
        ////internal override int InitWrite()
        ////{
        ////  int maxByteCount = m_fkp.FIBData.EncodingCharSize;

        ////  int start = m_fkp.FIBData.ccpText
        ////              + m_fkp.FIBData.ccpHdr
        ////              + m_fkp.FIBData.ccpFtn
        ////              + m_fkp.FIBData.ccpAtn
        ////              + m_fkp.FIBData.ccpEdn;

        ////  m_iStartItemPos = m_iStartText + start * maxByteCount;

        ////  return m_iStartItemPos;
        ////}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        internal override int MoveToItem(int itemIndex)
        {
            m_iItemIndex = itemIndex;
            uint start = (uint)(m_fkp.FIBData.ccpText
              + m_fkp.FIBData.ccpFtn
              + m_fkp.FIBData.ccpAtn
              + m_fkp.FIBData.ccpHdr
              + m_fkp.FIBData.ccpEdn);
            if (m_iStartItemPos == 0)
            {
                m_iStartItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(start));
                m_iEndText = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)(start + m_fkp.FIBData.ccpTxbx)));
            }

            m_iStartText = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(start + m_fkp.Tables.ArtObj.GetTxbxPosition(false, m_iItemIndex))));

            m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(start + m_fkp.Tables.ArtObj.GetTxbxPosition(false, m_iItemIndex + 1))));

            MoveToCurrentChpxPapx();
            return m_iStartText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEndPos"></param>
        /// <returns></returns>
        internal override bool UpdateItemEndPos(long iEndPos)
        {
            if (iEndPos >= m_iEndItemPos)
            {
                uint start = (uint)(m_fkp.FIBData.ccpText
                  + m_fkp.FIBData.ccpFtn
                  + m_fkp.FIBData.ccpAtn
                  + m_fkp.FIBData.ccpHdr
                  + m_fkp.FIBData.ccpEdn);
                bool isHdrTxbx = this is HFTextBoxStatePositions;
                if (isHdrTxbx)
                {
                    start += (uint)m_fkp.FIBData.ccpTxbx;
                }

                m_iItemIndex++;
                m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                    (uint)(start + m_fkp.Tables.ArtObj.GetTxbxPosition(isHdrTxbx, m_iItemIndex + 1))));
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// Header/footer textbox state position class
    /// </summary>
    [CLSCompliant(false)]
    internal class HFTextBoxStatePositions : TextBoxStatePositions
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxStatePositions"/> class.
        /// </summary>
        /// <param name="fkp"></param>
        internal HFTextBoxStatePositions(WordFKPData fkp)
            : base(fkp)
        {
        }
        #endregion

        #region Class overrides
        ////<summary>
        ////Initialize the write.         
        ////</summary>
        ////<returns>Returns textbox start write position</returns>
        ////internal override int InitWrite()
        ////{
        ////    int maxByteCount = m_fkp.FIBData.EncodingCharSize;

        ////    int start = m_fkp.FIBData.ccpText
        ////                + m_fkp.FIBData.ccpHdr
        ////                + m_fkp.FIBData.ccpFtn
        ////                + m_fkp.FIBData.ccpAtn
        ////                + m_fkp.FIBData.ccpEdn
        ////                + m_fkp.FIBData.ccpTxbx;

        ////    m_iStartItemPos = m_iStartText + start * maxByteCount;

        ////    return m_iStartItemPos;
        ////}

        /// <summary>
        /// Moves to text box.
        /// </summary>
        /// <param name="itemIndex">Index of the Text box.</param>
        /// <returns></returns>
        internal override int MoveToItem(int itemIndex)
        {
            m_iItemIndex = itemIndex;
            int start = m_fkp.FIBData.ccpText
                        + m_fkp.FIBData.ccpFtn
                        + m_fkp.FIBData.ccpAtn
                        + m_fkp.FIBData.ccpHdr
                        + m_fkp.FIBData.ccpEdn
                        + m_fkp.FIBData.ccpTxbx;
            if (m_iStartItemPos == 0)
            {
                m_iStartItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)start));
                m_iEndText = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos((uint)(start + m_fkp.FIBData.ccpHdrTxbx)));
            }

            m_iStartText = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(start + m_fkp.Tables.ArtObj.GetTxbxPosition(true, m_iItemIndex))));

            m_iEndItemPos = (int)(m_fkp.Tables.ConvertCharPosToFileCharPos(
                (uint)(start + m_fkp.Tables.ArtObj.GetTxbxPosition(true, m_iItemIndex + 1))));

            MoveToCurrentChpxPapx();

            return m_iStartText;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for StatePositionsBase.
    /// </summary>
    [CLSCompliant(false)]
    internal abstract class StatePositionsBase
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected int m_iStartItemPos;

        /// <summary>
        /// 
        /// </summary>
        protected int m_iEndItemPos;

        /// <summary>
        /// 
        /// </summary>
        protected int m_iItemIndex;

        /// <summary>
        /// 
        /// </summary>
        protected long m_iEndCHPxPos;

        /// <summary>
        /// 
        /// </summary>
        internal long m_iEndPAPxPos;

        /// <summary>
        /// 
        /// </summary>
        internal long m_iEndPieceTablePos;
        internal long m_iStartPieceTablePos;
        /// <summary>
        /// The position of first byte of text run.
        /// </summary>
        protected int m_iStartText;

        /// <summary>
        /// The position of last byte of text run.
        /// </summary>
        protected int m_iEndText;

        /// <summary>
        /// FKP data
        /// </summary>
        protected WordFKPData m_fkp;

        /// <summary>
        /// 
        /// </summary>
        protected BookmarkInfo[] m_bookmarks = null;

        /// <summary>
        /// 
        /// </summary>
        private int m_curTextPosition = 0;

        /// <summary>
        /// Currents papx/chpx page indexs
        /// </summary>
        private int m_iCurrentPapxFKPIndex = 0;

        private int m_iCurrentChpxFKPIndex = 0;
        private int m_iCurrentPapxIndex = 0;
        private int m_iCurrentChpxIndex = 0;
        //// <summary>         
        //// </summary>
        ////protected bool m_bBookmarkChecked = false;
        ////protected int m_minBkmkStart = int.MaxValue;
        ////protected int m_minBkmkEnd = int.MaxValue;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal int StartItemPos
        {
            get
            {
                return m_iStartItemPos;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int EndItemPos
        {
            get
            {
                return m_iEndItemPos;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ItemIndex
        {
            get
            {
                return m_iItemIndex;
            }

            set
            {
                m_iItemIndex = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int StartText
        {
            get
            {
                return m_iStartText;
            }

            set
            {
                m_iStartText = value;
            }
        }

        /// <summary>
        /// Gets current chpx
        /// </summary>
        /// <returns></returns>
        internal CharacterPropertyException CurrentChpx
        {
            get
            {
                return
                  m_fkp.GetChpxPage(m_iCurrentChpxFKPIndex).CharacterProperties[m_iCurrentChpxIndex];
            }
        }


        /// <summary>
        /// Gets current papx
        /// </summary>
        /// <returns></returns>
        internal ParagraphPropertyException CurrentPapx
        {
            get
            {
                ParagraphPropertyException papx = m_fkp.GetPapxPage(m_iCurrentPapxFKPIndex).ParagraphProperties[m_iCurrentPapxIndex];
                ////        short ilfo = papx.PropertyModifiers.GetShort( WordSprmOptions.sprmPIlfo, -1 );
                ////        byte ilvl = papx.PropertyModifiers.GetByte( WordSprmOptions.sprmPIlvl, 0 );
                ////        if( ilfo > 0 && ilvl >= 0 )
                ////        {
                ////          papx = m_fkp.Tables.GetListPapx( ilfo, ilvl );
                ////          papx.PropertyModifiers.SetValue( WordSprmOptions.sprmPIlfo, ilfo );
                ////          papx.PropertyModifiers.SetValue( WordSprmOptions.sprmPIlvl, ilvl );
                ////        }
                return papx;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int CurrentTextPosition
        {
            get
            {
                return m_curTextPosition;
            }

            set
            {
                m_curTextPosition = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected WPTablesData Tables
        {
            get
            {
                return m_fkp.Tables;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsCurrentPapxPosition
        {
            get
            {
                uint start = CurrentPapxPage.FileCharPos[m_iCurrentPapxIndex];
                uint end = CurrentPapxPage.FileCharPos[m_iCurrentPapxIndex + 1];
                {
                    if (m_iStartText >= start && m_iStartText <= end)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsCurrentChpxPosition
        {
            get
            {
                uint start = CurrentChpxPage.FileCharPos[m_iCurrentChpxIndex];
                uint end = CurrentChpxPage.FileCharPos[m_iCurrentChpxIndex + 1];
                {
                    if (m_iStartText >= start && m_iStartText <= end)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private CharacterPropertiesPage CurrentChpxPage
        {
            get
            {
                return m_fkp.GetChpxPage(m_iCurrentChpxFKPIndex);
            }
        }

        /// <summary>
        /// Gets current papx page
        /// </summary>
        /// <returns></returns>
        private ParagraphPropertiesPage CurrentPapxPage
        {
            get
            {
                return m_fkp.GetPapxPage(m_iCurrentPapxFKPIndex);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializing constructor
        /// </summary>
        internal StatePositionsBase(WordFKPData fkp)
        {
            m_fkp = fkp;
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Initialize start/end positions of text run
        /// </summary>
        internal virtual void InitStartEndPos()
        {
            m_iStartText = (int)(Tables.ConvertCharPosToFileCharPos(0));
            ////m_iEndText = m_iStartText + m_fkp.FIBData.ccpText * m_fkp.FIBData.EncodingCharSize;
            ////int index = Tables.PieceTablePositions.Count - 1;
            ////uint end = ( uint )Tables.PieceTablePositions[ index ];
            ////m_iEndText = ( int )end;
            m_iEndText = (int)Tables.ConvertCharPosToFileCharPos((uint)m_fkp.FIBData.ccpText);
            ////m_iEndText = m_iStartText + m_fkp.FIBData.ccpText;
            m_iEndPAPxPos = (int)m_fkp.GetPapxPage(0).FileCharPos[1];
            m_iEndCHPxPos = (int)m_fkp.GetChpxPage(0).FileCharPos[1];

            ////      // cheking bookmarks
            ////      BookmarkInfo[] bookmarkInfos = m_fkp.Tables.GetBookmarks();
            ////      
            ////      if( bookmarkInfos != null && bookmarkInfos.Length != 0 )
            ////      {
            ////        m_bookmarks = m_fkp.Tables.GetBookmarks();
            ////        // Change CP to FC
            ////        for( int i = 0; i < m_bookmarks.Length; i++ )
            ////        {
            ////          m_bookmarks[ i ].StartPos = ( int )m_fkp.Tables.ConvertCharPosToFileCharPos( ( uint )bookmarkInfos[i].StartPos );
            ////          m_bookmarks[ i ].EndPos = ( int )m_fkp.Tables.ConvertCharPosToFileCharPos( ( uint )bookmarkInfos[i].EndPos );
            ////        }
            ////      }
        }

        /// <summary>
        /// Move to next character properties exception
        /// </summary>
        /// <returns></returns>
        internal bool NextChpx()
        {
            CharacterPropertiesPage page = CurrentChpxPage;

            // Next papx pos. in current FKP page
            if (m_iCurrentChpxIndex < page.RunsCount - 1)
            {
                m_iCurrentChpxIndex++;
            }
            else // Next FKP page
            {
                // If next FKP block doesn't exist - return false
                if (m_iCurrentChpxFKPIndex + 1 > m_fkp.Tables.CHPXBinaryTable.EntriesCount - 1)
                {
                    m_iEndCHPxPos = -1;
                    return false;
                }
                else
                {
                    m_iCurrentChpxFKPIndex++;
                    m_iCurrentChpxIndex = 0;
                    page = CurrentChpxPage;
                }
            }

            // Gets data
            m_iEndCHPxPos = (int)page.FileCharPos[m_iCurrentChpxIndex + 1];
            return true;
        }

        /// <summary>
        /// Move to next paragraph properties exception
        /// </summary>
        /// <returns></returns>
        internal bool NextPapx()
        {
            ParagraphPropertiesPage page = CurrentPapxPage;

            // Next papx pos. in current FKP page
            if (m_iCurrentPapxIndex < page.RunsCount - 1)
            {
                m_iCurrentPapxIndex++;
            }
            else // Next FKP page
            {
                // If next FKP block not exists - return false
                if (m_iCurrentPapxFKPIndex + 1 > m_fkp.Tables.PAPXBinaryTable.EntriesCount - 1)
                {
                    m_iEndPAPxPos = -1;
                    ////height = page.Heights[ m_iCurrentPapxIndex ];
                    return false;
                }
                else
                {
                    m_iCurrentPapxFKPIndex++;
                    m_iCurrentPapxIndex = 0;
                    page = CurrentPapxPage;
                }
            }

            // Gets data
            m_iEndPAPxPos = (int)page.FileCharPos[m_iCurrentPapxIndex + 1];
            ////height = page.Heights[ m_iCurrentPapxIndex ];
            return true;
        }

        /// <summary>
        /// Get min end pos from chpx and papx
        /// </summary>
        /// <returns></returns>
        internal virtual long GetMinEndPos(long curPos)
        {
            ////      // If we got thru bookmark position than update min positions
            ////      // and erase bookmark positions that we checked
            ////      if( m_bookmarks != null && !m_bBookmarkChecked )
            ////      {
            ////        for( int i = 0; i < m_bookmarks.Length; i++ )
            ////        {
            ////          if( m_bookmarks[i].StartPos < m_minBkmkStart )
            ////          {
            ////            m_minBkmkStart = m_bookmarks[ i ].StartPos;
            ////            m_bookmarks[ i ].StartPos = int.MaxValue;
            ////          }
            ////          if( m_bookmarks[i].EndPos < m_minBkmkEnd )
            ////          {
            ////            m_minBkmkEnd = m_bookmarks[ i ].EndPos;
            ////            m_bookmarks[ i ].EndPos = int.MaxValue;
            ////          }
            ////        }
            ////        m_bBookmarkChecked = true;
            ////      }
            ////      // Min positions of properties and bookmarks in the document
            ////      int minPropPos = Math.Min( m_iEndCHPxPos, m_iEndPAPxPos );
            ////      int minBkmkPos = Math.Min( m_minBkmkStart, m_minBkmkEnd );
            ////
            ////      // Get min text chunk end
            ////      int minPos;
            ////      // Check if we are at bookmark start than read whole chunk
            ////      // don't pay attention to bookmark start
            ////      if( m_minBkmkStart == ( m_curTextPosition + m_iStartText ) )
            ////      {
            ////        //minPos = minPropPos;  
            ////        minPos = m_minBkmkEnd;
            ////      }
            ////      else
            ////      {
            ////        minPos =  Math.Min( minPropPos, minBkmkPos);
            ////      }
            ////      
            ////      // check if we hit on bookmark position
            ////      // than erase it
            ////      if( minBkmkPos <= minPropPos )
            ////      {
            ////        m_bBookmarkChecked = false;
            ////        if( m_minBkmkEnd > m_minBkmkStart )
            ////        {
            ////          m_minBkmkStart = int.MaxValue;
            ////        }
            ////        else
            ////        {
            ////          m_minBkmkEnd = int.MaxValue;
            ////        }
            ////      }
            ////      
            ////      return minPos;
            //Get index of current end piece table position if available
            int index = m_iEndPieceTablePos != 0 ? Tables.PieceTablePositions.IndexOf((uint)m_iEndPieceTablePos) : 0;
            //Total File character length - Difference between current and previous file character position considering the Encoding type
            //Encoding.GetByteCount("a") is used to retrieve the number of bytes used for Encoding a single character
            long totalFileCharacterLength = (index > 0 ?
             ((Tables.m_pieceTable.FileCharacterPos[index] - Tables.m_pieceTable.FileCharacterPos[index - 1]) *
             (Tables.m_pieceTableEncodings[index - 1].GetByteCount("a"))) : 0);

          
            //if end piece table position is less than current stream position update piece table information with new end piece table position for the current state position
            //or Check if the current position reached the end of file character position referred in piece table, if it is update piece table information in state position
            if (m_iEndPieceTablePos <= curPos || 
                curPos == m_iStartPieceTablePos + totalFileCharacterLength)
            {
                List<UInt32> pieceTablePositions = Tables.PieceTablePositions;
                //Set i = index which denotes index of current end piece table position
                for (int i = index, count = pieceTablePositions.Count - 1; i < count; i++)
                {
                    uint curTablePos = pieceTablePositions[i];
                    uint nextTablePos = pieceTablePositions[i + 1];
                    //Update piece table information if current position lies within the range of current table and next piece table position
                    if (curPos < nextTablePos)
                    {
                        m_iStartPieceTablePos = curTablePos;
                        m_iEndPieceTablePos = (int)nextTablePos;
                        index++;
                        break;
                    }
                }
            }
            long endPosition;
            if ((m_iEndCHPxPos != -1 && m_iStartPieceTablePos > m_iEndCHPxPos) || (m_iEndPAPxPos != -1 && m_iStartPieceTablePos > m_iEndPAPxPos))
                endPosition = Math.Min(m_iStartPieceTablePos, m_iEndPieceTablePos);
            else
                endPosition = Math.Min(Math.Min(m_iEndCHPxPos, m_iEndPAPxPos), m_iEndPieceTablePos);
            
            //Update Total File character length - Difference between current and previous file character position 
            //considering the Encoding type
            totalFileCharacterLength = (index > 0 ?
             ((Tables.m_pieceTable.FileCharacterPos[index] - Tables.m_pieceTable.FileCharacterPos[index - 1]) *
             (Tables.m_pieceTableEncodings[index - 1].GetByteCount("a"))) : 0);

            //Maximum file character position allowed in current piece table
            long maximumFileCharacterPosition = m_iStartPieceTablePos + totalFileCharacterLength;
            //Limit the end position if it exceeds the maximum file character position in current piece table
            if (endPosition > maximumFileCharacterPosition && 
                CurrentTextPosition < Tables.m_pieceTable.FileCharacterPos[index] &&
                curPos < maximumFileCharacterPosition)
            {
                endPosition = maximumFileCharacterPosition;
            }
            return endPosition;
        }

        /// <summary>
        /// Update end position of chpx
        /// </summary>
        /// <param name="iEndPos"></param>
        internal bool UpdateCHPxEndPos(long iEndPos)
        {
            return (iEndPos >= m_iEndCHPxPos && NextChpx());
        }

        /// <summary>
        /// Update end position of papx
        /// </summary>
        /// <param name="iEndPos"></param>
        internal bool UpdatePAPxEndPos(long iEndPos)
        {
            return (iEndPos >= m_iEndPAPxPos && NextPapx());
        }

        /// <summary>
        /// Is it first pass in text run
        /// </summary>
        /// <param name="iPos"></param>
        /// <returns></returns>
        internal bool IsFirstPass(long iPos)
        {
            return !(iPos >= m_iStartText);
        }

        /// <summary>
        /// Is it end of text
        /// </summary>
        /// <param name="iPos"></param>
        /// <returns></returns>
        internal bool IsEndOfText(long iPos)
        {
            return (iPos >= m_iEndText);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        ////internal virtual int InitWrite()
        ////{
        ////  return 0;
        ////}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEndPos"></param>
        /// <returns></returns>
        internal virtual bool UpdateItemEndPos(long iEndPos)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        internal virtual int MoveToItem(int itemIndex)
        {
            return 0;
        }

        /// <summary>
        ///  Check if current position is less then subdocument item text end.
        /// </summary>
        /// <param name="iPos"></param>
        /// <returns></returns>
        internal virtual bool IsEndOfSubdocItemText(long iPos)
        {
            return false;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Initializes chpx/papx
        /// </summary>
        protected void MoveToCurrentChpxPapx()
        {
            ////m_iEndCHPxPos = 0;
            ////m_iEndPAPxPos = 0;
            ////m_iCurrentPapxIndex = 0;
            ////m_iCurrentPapxFKPIndex = 0;

            if (!IsCurrentPapxPosition)
            {
                while (m_iStartText >= m_iEndPAPxPos)
                {
                    if (!NextPapx())
                        break;
                }
            }

            if (!IsCurrentChpxPosition)
            {
                while (m_iStartText >= m_iEndCHPxPos)
                {
                    if (!NextChpx())
                        break;
                }
            }
        }
        #endregion
    }
}
