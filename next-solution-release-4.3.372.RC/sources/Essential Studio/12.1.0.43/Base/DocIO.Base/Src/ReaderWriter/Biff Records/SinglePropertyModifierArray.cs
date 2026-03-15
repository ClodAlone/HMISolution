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
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for SinglePropertyModifierArray.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SinglePropertyModifierArray
      : BaseWordRecord
        , ICollection
    {
        #region Class members
#if DEBUG_SPRM
    internal byte[] __UnderlineData;
#endif
        /// <summary>
        /// List of single property modifiers.
        /// </summary>
        private List<SinglePropertyModifierRecord> m_arrModifiers = new List<SinglePropertyModifierRecord>();
        #endregion

        #region Inner classes
        private class SPRMEnumerator : IEnumerator
        {
            #region Class members
            /// <summary>
            /// 
            /// </summary>
            private int m_iIndex = -1;
            /// <summary>
            /// Parent collection.
            /// </summary>
            private SinglePropertyModifierArray m_parent;
            #endregion

            #region Class Initialize/Finalize methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="parent"></param>
            internal SPRMEnumerator(SinglePropertyModifierArray parent)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");

                m_parent = parent;
            }
            #endregion

            #region IEnumerator Members
            /// <summary>
            /// 
            /// </summary>
            public void Reset()
            {
                m_iIndex = -1;
            }
            /// <summary>
            /// 
            /// </summary>
            public object Current
            {
                get
                {
                    if (m_iIndex < 0 || m_iIndex >= m_parent.Count)
                        return null;

                    return m_parent.GetSprmByIndex(m_iIndex);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                m_iIndex++;

                if (m_iIndex < 0 || m_iIndex >= m_parent.Count)
                {
                    return false;
                }

                return true;
            }
            #endregion
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal SinglePropertyModifierArray()
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal SinglePropertyModifierArray(byte[] data)
            : base(data)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal SinglePropertyModifierArray(byte[] arrData, int iOffset)
            : base(arrData, iOffset)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="iCount">Number of bytes for the new record.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal SinglePropertyModifierArray(byte[] arrData, int iOffset, int iCount)
            : base(arrData, iOffset, iCount)
        {
        }
        /// <summary>
        /// Creates new record from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal SinglePropertyModifierArray(Stream stream, int iCount)
            : base(stream, iCount)
        {
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Remove modifier from collection
        /// </summary>
        /// <param name="options"></param>
        internal void RemoveValue(int options)
        {
            SinglePropertyModifierRecord record = this[options];

            while (record != null)
            {
                m_arrModifiers.Remove(record);
                record = this[options];
            }
        }
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal override void Parse(byte[] arrData, int iOffset, int iCount)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0)
                throw new ArgumentOutOfRangeException("iOffset < 0");

            if (iCount < 0)
                throw new ArgumentOutOfRangeException("iCount < 0 ");

            if (iOffset + iCount > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset + iCount");

#if DEBUG_SPRM
      __UnderlineData = new byte[ iCount ];
      Array.Copy( arrData, iOffset, __UnderlineData, 0, iCount );
#endif
            Clear();

            int iStartOffset = iOffset;

            while (iCount - (iOffset - iStartOffset) > 1)
            {
                SinglePropertyModifierRecord sprm = new SinglePropertyModifierRecord();
                iOffset = sprm.Parse(arrData, iOffset);
                if (IsCorrectSprm(sprm))
                    m_arrModifiers.Add(sprm);
            }
        }
        /// <summary>
        /// Determines whether sprm is correct.
        /// </summary>
        /// <param name="sprm">The Single Property Modifier Record.</param>
        /// <returns>
        /// 	<c>true</c> if sprm is correct; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCorrectSprm(SinglePropertyModifierRecord sprm)
        {
            if (IsValidCharacterPropertySprm(sprm) || IsValidParagraphPropertySprm(sprm) || IsValidTablePropertySprm(sprm) || IsValidSectionPropertySprm(sprm) || IsValidPicturePropertySprm(sprm))
                return true;
            else
                return false;           
        }
        /// <summary>
        /// Checks for valid Character Property sprm
        /// </summary>
        /// <param name="sprm"></param>
        /// <returns>
        /// <c>true</c> if sprm is correct; otherwise, <c>false</c>.
        /// </returns>       
        private bool IsValidCharacterPropertySprm(SinglePropertyModifierRecord sprm)
        {
            #region Character properties
            switch (sprm.Options)
            {

                //Character property sprms
                //sprmCFRMarkDel
                case 0x0800:
                    if (sprm.ByteValue != 0 && sprm.ByteValue != 1 && sprm.ByteValue != 128 && sprm.ByteValue != 129)
                        sprm.ByteValue = 0;
                    return true;
                    break;
                //sprmCFRMarkIns
                case 0x0801:
                //sprmCFFldVanish
                case 0x0802:
                //sprmCPicLocation
                case 0x6A03:
                //sprmCIbstRMark
                case 0x4804:
                //sprmCDttmRMark
                case 0x6805:
                //sprmCFData
                case 0x0806:
                //sprmCIdslRMark
                case 0x4807:
                //sprmCSymbol
                case 0x6A09:
                //sprmCFOle2
                case 0x080A:
                //sprmCHighlight
                case 0x2A0C:
                //sprmCFWebHidden
                case 0x0811:
                //sprmCRsidProp
                case 0x6815:
                //sprmCRsidText
                case 0x6816:
                //sprmCRsidRMDel
                case 0x6817:
                //sprmCFSpecVanish
                case 0x0818:
                //sprmCFMathPr
                case 0xC81A:
                //sprmCIstd
                case 0x4A30:
                //sprmCIstdPermute
                case 0xCA31:
                //sprmCPlain
                case 0x2A33:
                //sprmCKcd
                case 0x2A34:
                //sprmCFBold
                case 0x0835:
                //sprmCFItalic
                case 0x0836:
                //sprmCFStrike
                case 0x0837:
                //sprmCFOutline
                case 0x0838:
                //sprmCFShadow
                case 0x0839:
                //sprmCFSmallCaps
                case 0x083A:
                //sprmCFCaps
                case 0x083B:
                //sprmCFVanish
                case 0x083C:
                //sprmCKul
                case 0x2A3E:
                //sprmCDxaSpace
                case 0x8840:
                //sprmCIco
                case 0x2A42:
                //sprmCHps
                case 0x4A43:
                //sprmCHpsPos
                case 0x4845:
                //sprmCMajority
                case 0xCA47:
                //sprmCIss
                case 0x2A48:
                //sprmCHpsKern
                case 0x484B:
                //sprmCHresi
                case 0x484E:
                //sprmCRgFtc0
                case 0x4A4F:
                //sprmCRgFtc1
                case 0x4A50:
                //sprmCRgFtc2
                case 0x4A51:
                //sprmCCharScale
                case 0x4852:
                //sprmCFDStrike
                case 0x2A53:
                //sprmCFImprint
                case 0x0854:
                //sprmCFSpec
                case 0x0855:
                //sprmCFObj
                case 0x0856:
                //sprmCPropRMark90
                case 0xCA57:
                //sprmCFEmboss
                case 0x0858:
                //sprmCSfxText
                case 0x2859:
                //sprmCFBiDi
                case 0x085A:
                //sprmCFBoldBi
                case 0x085C:
                //sprmCFItalicBi
                case 0x085D:
                //sprmCFtcBi
                case 0x4A5E:
                //sprmCLidBi
                case 0x485F:
                //sprmCIcoBi
                case 0x4A60:
                //sprmCHpsBi
                case 0x4A61:
                //sprmCDispFldRMark
                case 0xCA62:
                //sprmCIbstRMarkDel
                case 0x4863:
                //sprmCDttmRMarkDel
                case 0x6864:
                //sprmCBrc80
                case 0x6865:
                //sprmCShd80
                case 0x4866:
                //sprmCIdslRMarkDel
                case 0x4867:
                //sprmCFUsePgsuSettings
                case 0x0868:
                //sprmCRgLid0_80
                case 0x486D:
                //sprmCRgLid1_80
                case 0x486E:
                //sprmCIdctHint
                case 0x286F:
                //sprmCCv
                case 0x6870:
                //sprmCShd
                case 0xCA71:
                //sprmCBrc
                case 0xCA72:
                //sprmCRgLid0
                case 0x4873:
                //sprmCRgLid1
                case 0x4874:
                //sprmCFNoProof
                case 0x0875:
                //sprmCFitText
                case 0xCA76:
                //sprmCCvUl
                case 0x6877:
                //sprmCFELayout
                case 0xCA78:
                //sprmCLbcCRJ
                case 0x2879:
                //sprmCFComplexScripts
                case 0x0882:
                //sprmCWall
                case 0x2A83:
                //sprmCCnf
                case 0xCA85:
                //sprmCNeedFontFixup
                case 0x2A86:
                //sprmCPbiIBullet
                case 0x6887:
                //sprmCPbiGrf
                case 0x4888:
                //sprmCPropRMark
                case 0xCA89:
                //sprmCFSdtVanish
                case 0x2A90:
                    return true;
                default:
                    return false;
            }
                #endregion
        }
        /// <summary>
        /// Checks for valid paragraph Property sprm
        /// </summary>
        /// <param name="sprm"></param>
        /// <returns>
        /// <c>true</c> if sprm is correct; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidParagraphPropertySprm(SinglePropertyModifierRecord sprm)
        {
            # region paragraph properties

            switch (sprm.Options)
            {
                //Paragraph Property sprms
                //sprmPIstd
                case 0x4600:
                //sprmPIstdPermute
                case 0xC601:
                //sprmPIncLvl
                case 0x2602:
                //sprmPJc80
                case 0x2403:
                //sprmPFKeep
                case 0x2405:
                //sprmPFKeepFollow
                case 0x2406:
                //sprmPFPageBreakBefore
                case 0x2407:
                //sprmPIlvl
                case 0x260A:
                //sprmPIlfo
                case 0x460B:
                //sprmPFNoLineNumb
                case 0x240C:
                //sprmPChgTabsPapx
                case 0xC60D:
                //sprmPDxaRight80
                case 0x840E:
                //sprmPDxaLeft80
                case 0x840F:
                //sprmPNest80
                case 0x4610:
                //sprmPDxaLeft180
                case 0x8411:
                //sprmPDyaLine
                case 0x6412:
                //sprmPDyaBefore
                case 0xA413:
                //sprmPDyaAfter
                case 0xA414:
                //sprmPChgTabs
                case 0xC615:
                //sprmPFInTable
                case 0x2416:
                //sprmPFTtp
                case 0x2417:
                //sprmPDxaAbs
                case 0x8418:
                //sprmPDyaAbs
                case 0x8419:
                //sprmPDxaWidth
                case 0x841A:
                //sprmPPc
                case 0x261B:
                //sprmPWr
                case 0x2423:
                //sprmPBrcTop80
                case 0x6424:
                //sprmPBrcLeft80
                case 0x6425:
                //sprmPBrcBottom80
                case 0x6426:
                //sprmPBrcRight80
                case 0x6427:
                //sprmPBrcBetween80
                case 0x6428:
                //sprmPBrcBar80
                case 0x6629:
                //sprmPFNoAutoHyph
                case 0x242A:
                //sprmPWHeightAbs
                case 0x442B:
                //sprmPDcs
                case 0x442C:
                //sprmPShd80
                case 0x442D:
                //sprmPDyaFromText
                case 0x842E:
                //sprmPDxaFromText
                case 0x842F:
                //sprmPFLocked
                case 0x2430:
                //sprmPFWidowControl
                case 0x2431:
                //sprmPFKinsoku
                case 0x2433:
                //sprmPFWordWrap
                case 0x2434:
                //sprmPFOverflowPunct
                case 0x2435:
                //sprmPFTopLinePunct
                case 0x2436:
                //sprmPFAutoSpaceDE
                case 0x2437:
                //sprmPFAutoSpaceDN
                case 0x2438:
                //sprmPWAlignFont
                case 0x4439:
                //sprmPFrameTextFlow
                case 0x443A:
                //sprmPOutLvl
                case 0x2640:
                //sprmPFBiDi
                case 0x2441:
                //sprmPFNumRMIns
                case 0x2443:
                //sprmPNumRM
                case 0xC645:
                //sprmPHugePapx
                case 0x6646:
                //sprmPFUsePgsuSettings
                case 0x2447:
                //sprmPFAdjustRight
                case 0x2448:
                //sprmPItap
                case 0x6649:
                //sprmPDtap
                case 0x664A:
                //sprmPFInnerTableCell
                case 0x244B:
                //sprmPFInnerTtp
                case 0x244C:
                //sprmPShd
                case 0xC64D:
                //sprmPBrcTop
                case 0xC64E:
                //sprmPBrcLeft
                case 0xC64F:
                //sprmPBrcBottom
                case 0xC650:
                //sprmPBrcRight
                case 0xC651:
                //sprmPBrcBetween
                case 0xC652:
                //sprmPBrcBar
                case 0xC653:
                //sprmPDxcRight
                case 0x4455:
                //sprmPDxcLeft
                case 0x4456:
                //sprmPDxcLeft1
                case 0x4457:
                //sprmPDylBefore
                case 0x4458:
                //sprmPDylAfter
                case 0x4459:
                //sprmPFOpenTch
                case 0x245A:
                //sprmPFDyaBeforeAuto
                case 0x245B:
                //sprmPFDyaAfterAuto
                case 0x245C:
                //sprmPDxaRight
                case 0x845D:
                //sprmPDxaLeft
                case 0x845E:
                //sprmPNest
                case 0x465F:
                //sprmPDxaLeft1
                case 0x8460:
                //sprmPJc
                case 0x2461:
                //sprmPFNoAllowOverlap
                case 0x2462:
                //sprmPWall
                case 0x2664:
                //sprmPIpgp
                case 0x6465:
                //sprmPCnf
                case 0xC666:
                //sprmPRsid
                case 0x6467:
                //sprmPIstdListPermute
                case 0xC669:
                //sprmPTableProps
                case 0x646B:
                //sprmPTIstdInfo
                case 0xC66C:
                //sprmPFContextualSpacing
                case 0x246D:
                //sprmPPropRMark
                case 0xC66F:
                //sprmPFMirrorIndents
                case 0x2470:
                //sprmPTtwo
                case 0x2471:
                    return true;
                default:
                    return false;
            }

                # endregion
        }
        /// <summary>
        /// Checks for valid Table Property sprm
        /// </summary>
        /// <param name="sprm"></param>
        /// <returns>
        /// <c>true</c> if sprm is correct; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidTablePropertySprm(SinglePropertyModifierRecord sprm)
        {
            #region Table properties

            switch (sprm.Options)
            {
                //Table property sprms
                //sprmTJc90
                case 0x5400:
                //sprmTDxaLeft
                case 0x9601:
                //sprmTDxaGapHalf
                case 0x9602:
                //sprmTFCantSplit90
                case 0x3403:
                //sprmTTableHeader
                case 0x3404:
                //sprmTTableBorders80
                case 0xD605:
                //sprmTDyaRowHeight
                case 0x9407:
                //sprmTDefTable
                case 0xD608:
                //sprmTDefTableShd80
                case 0xD609:
                //sprmTTlp
                case 0x740A:
                //sprmTFBiDi
                case 0x560B:
                //sprmTDefTableShd3rd
                case 0xD60C:
                //sprmTPc
                case 0x360D:
                //sprmTDxaAbs
                case 0x940E:
                //sprmTDyaAbs
                case 0x940F:
                //sprmTDxaFromText
                case 0x9410:
                //sprmTDyaFromText
                case 0x9411:
                //sprmTDefTableShd
                case 0xD612:
                //sprmTTableBorders
                case 0xD613:
                //sprmTTableWidth
                case 0xF614:
                //sprmTFAutofit
                case 0x3615:
                //sprmTDefTableShd2nd
                case 0xD616:
                //sprmTWidthBefore
                case 0xF617:
                //sprmTWidthAfter
                case 0xF618:
                //sprmTFKeepFollow
                case 0x3619:
                //sprmTBrcTopCv
                case 0xD61A:
                //sprmTBrcLeftCv
                case 0xD61B:
                //sprmTBrcBottomCv
                case 0xD61C:
                //sprmTBrcRightCv
                case 0xD61D:
                //sprmTDxaFromTextRight
                case 0x941E:
                //sprmTDyaFromTextBottom
                case 0x941F:
                //sprmTSetBrc80
                case 0xD620:
                //sprmTInsert
                case 0x7621:
                //sprmTDelete
                case 0x5622:
                //sprmTDxaCol
                case 0x7623:
                //sprmTMerge
                case 0x5624:
                //sprmTSplit
                case 0x5625:
                //sprmTTextFlow
                case 0x7629:
                //sprmTVertMerge
                case 0xD62B:
                //sprmTVertAlign
                case 0xD62C:
                //sprmTSetShd
                case 0xD62D:
                //sprmTSetShdOdd
                case 0xD62E:
                //sprmTSetBrc
                case 0xD62F:
                //sprmTCellPadding
                case 0xD632:
                //sprmTCellSpacingDefault
                case 0xD633:
                //sprmTCellPaddingDefault
                case 0xD634:
                //sprmTCellWidth
                case 0xD635:
                //sprmTFitText
                case 0xF636:
                //sprmTFCellNoWrap
                case 0xD639:
                //sprmTIstd
                case 0x563A:
                //sprmTCellPaddingStyle
                case 0xD63E:
                //sprmTCellFHideMark
                case 0xD642:
                //sprmTSetShdTable
                case 0xD660:
                //sprmTWidthIndent
                case 0xF661:
                //sprmTCellBrcType
                case 0xD662:
                //sprmTFBiDi90
                case 0x5664:
                //sprmTFNoAllowOverlap
                case 0x3465:
                //sprmTFCantSplit
                case 0x3466:
                //sprmTPropRMark
                case 0xD667:
                //sprmTWall
                case 0x3668:
                //sprmTIpgp
                case 0x7469:
                //sprmTCnf
                case 0xD66A:
                //sprmTDefTableShdRaw
                case 0xD670:
                //sprmTDefTableShdRaw2nd
                case 0xD671:
                //sprmTDefTableShdRaw3rd
                case 0xD672:
                //sprmTRsid
                case 0x7479:
                //sprmTCellVertAlignStyle
                case 0x347C:
                //sprmTCellNoWrapStyle
                case 0x347D:
                //sprmTCellBrcTopStyle
                case 0xD47F:
                //sprmTCellBrcBottomStyle
                case 0xD680:
                //sprmTCellBrcLeftStyle
                case 0xD681:
                //sprmTCellBrcRightStyle
                case 0xD682:
                //sprmTCellBrcInsideHStyle
                case 0xD683:
                //sprmTCellBrcInsideVStyle
                case 0xD684:
                //sprmTCellBrcTL2BRStyle
                case 0xD685:
                //sprmTCellBrcTR2BLStyle
                case 0xD686:
                //sprmTCellShdStyle
                case 0xD687:
                //sprmTCHorzBands
                case 0x3488:
                //sprmTCVertBands
                case 0x3489:
                //sprmTJc
                case 0x548A:
                    return true;               
                default:
                    return false;
            }
             #endregion
        }
        /// <summary>
        /// Checks for valid Section Property sprm
        /// </summary>
        /// <param name="sprm"></param>
        /// <returns>
        /// <c>true</c> if sprm is correct; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidSectionPropertySprm(SinglePropertyModifierRecord sprm)
        {
            #region Section properties

            switch (sprm.Options)
            {         

                //Section property sprms
                //sprmScnsPgn 
                case 0x3000:
                //sprmSiHeadingPgn 
                case 0x3001:
                //sprmSDxaColWidth 
                case 0xF203:
                //sprmSDxaColSpacing 
                case 0xF204:
                //sprmSFEvenlySpaced 
                case 0x3005:
                //sprmSFProtected 
                case 0x3006:
                //sprmSDmBinFirst 
                case 0x5007:
                //sprmSDmBinOther 
                case 0x5008:
                //sprmSBkc 
                case 0x3009:
                //sprmSFTitlePage 
                case 0x300A:
                //sprmSCcolumns 
                case 0x500B:
                //sprmSDxaColumns 
                case 0x900C:
                //sprmSNfcPgn 
                case 0x300E:
                //sprmSFPgnRestart 
                case 0x3011:
                //sprmSFEndnote 
                case 0x3012:
                //sprmSLnc 
                case 0x3013:
                //sprmSNLnnMod 
                case 0x5015:
                //sprmSDxaLnn 
                case 0x9016:
                //sprmSDyaHdrTop 
                case 0xB017:
                //sprmSDyaHdrBottom 
                case 0xB018:
                //sprmSLBetween 
                case 0x3019:
                //sprmSVjc 
                case 0x301A:
                //sprmSLnnMin 
                case 0x501B:
                //sprmSPgnStart97 
                case 0x501C:
                //sprmSBOrientation 
                case 0x301D:
                //sprmSXaPage 
                case 0xB01F:
                //sprmSYaPage 
                case 0xB020:
                //sprmSDxaLeft 
                case 0xB021:
                //sprmSDxaRight 
                case 0xB022:
                //sprmSDyaTop 
                case 0x9023:
                //sprmSDyaBottom 
                case 0x9024:
                //sprmSDzaGutter 
                case 0xB025:
                //sprmSDmPaperReq 
                case 0x5026:
                //sprmSFBiDi 
                case 0x3228:
                //sprmSFRTLGutter 
                case 0x322A:
                //sprmSBrcTop80 
                case 0x702B:
                //sprmSBrcLeft80 
                case 0x702C:
                //sprmSBrcBottom80 
                case 0x702D:
                //sprmSBrcRight80 
                case 0x702E:
                //sprmSPgbProp 
                case 0x522F:
                //sprmSDxtCharSpace 
                case 0x7030:
                //sprmSDyaLinePitch 
                case 0x9031:
                //sprmSClm 
                case 0x5032:
                //sprmSTextFlow 
                case 0x5033:
                //sprmSBrcTop 
                case 0xD234:
                //sprmSBrcLeft 
                case 0xD235:
                //sprmSBrcBottom 
                case 0xD236:
                //sprmSBrcRight 
                case 0xD237:
                //sprmSWall 
                case 0x3239:
                //sprmSRsid 
                case 0x703A:
                //sprmSFpc 
                case 0x303B:
                //sprmSRncFtn 
                case 0x303C:
                //sprmSRncEdn 
                case 0x303E:
                //sprmSNFtn 
                case 0x503F:
                //sprmSNfcFtnRef 
                case 0x5040:
                //sprmSNEdn 
                case 0x5041:
                //sprmSNfcEdnRef 
                case 0x5042:
                //sprmSPropRMark 
                case 0xD243:
                //sprmSPgnStart 
                case 0x7044:
                    return true;
                default:
                    return false;
            }

            #endregion
        }
        /// <summary>
        /// Checks for valid Picture Property sprm
        /// </summary>
        /// <param name="sprm"></param>
        /// <returns>
        /// <c>true</c> if sprm is correct; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidPicturePropertySprm(SinglePropertyModifierRecord sprm)
        {
            #region picture properties sprms

            switch (sprm.Options)
            {
                //sprmPicBrcTop80
                case 0x6C02:
                //sprmPicBrcLeft80 
                case 0x6C03:
                //sprmPicBrcBottom80 
                case 0x6C04:
                //sprmPicBrcRight80 
                case 0x6C05:
                //sprmPicBrcTop 
                case 0xCE08:
                //sprmPicBrcLeft 
                case 0xCE09:
                //sprmPicBrcBottom 
                case 0xCE0A:
                //sprmPicBrcRight 
                case 0xCE0B:
                    return true;
                default:
                    return false;
            }
            #endregion          
        }
        /// <summary>
        /// Saves record into array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            int iLength = Length;

            if (iLength == 0) return 0;

            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset + iLength > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            int iResult = 0;
            int iPartLen = 0;

            //BitConverter.GetBytes( ( ushort )iLength ).CopyTo( arrData, iOffset );
            //iOffset += Constants.BytesInWord;

            for (int i = 0, len = Count; i < len; i++)
            {
                SinglePropertyModifierRecord sprm = m_arrModifiers[i];
                iPartLen = sprm.Save(arrData, iOffset);
                iResult += iPartLen;
                iOffset += iPartLen;
            }

            return iResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal int Save(BinaryWriter writer, Stream stream, int length)
        {
            //      int iLength = Length;
            int iLength = length;

            if (iLength == 0) return 0;

            if (writer == null)
                throw new ArgumentNullException("stream");

            int iResult = 0;
            int iPartLen = 0;

            for (int i = 0, len = Count; i < len; i++)
            {
                SinglePropertyModifierRecord sprm = m_arrModifiers[i];
                if (sprm.Operand != null)
                {
                    iPartLen = sprm.Save(writer, stream);
                    iResult += iPartLen;
                }
            }

            return iResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal int Save(BinaryWriter writer, Stream stream)
        {
            long start = stream.Position;
            //int iLength = Length;

            if (Modifiers.Count == 0) return 0;

            if (writer == null)
                throw new ArgumentNullException("stream");

            for (int i = 0, len = Count; i < len; i++)
            {
                (m_arrModifiers[i]).Save(writer, stream);
            }

            return (int)(stream.Position - start);
        }
        /// <summary>
        /// Removes all modifiers from the collection.
        /// </summary>
        internal void Clear()
        {
            m_arrModifiers.Clear();
        }
        /// <summary>
        /// Adds new modifier to the collection.
        /// </summary>
        /// <param name="modifier">Modifier to add.</param>
        internal void Add(SinglePropertyModifierRecord modifier)
        {
            m_arrModifiers.Add(modifier);
        }
        /// <summary>
        /// Sort Sprms based on Unique ID
        /// </summary>
        internal void SortSprms()
        {
            SinglePropertyModifierRecord temp;
            if (!IsContainTrackChangesSprm())
            {
                for (int i = 0; i < m_arrModifiers.Count; i++)
                {
                    for (int j = i + 1; j < m_arrModifiers.Count; j++)
                    {
                        if (m_arrModifiers[i].UniqueID > m_arrModifiers[j].UniqueID)
                        {
                            temp = m_arrModifiers[i];
                            m_arrModifiers[i] = m_arrModifiers[j];
                            m_arrModifiers[j] = temp;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Check whether the Sprms contain TrackChanges SPRMs
        /// </summary>
        /// <returns></returns>
        private bool IsContainTrackChangesSprm()
        {
            for (int i = 0; i < m_arrModifiers.Count; i++)
            {
                if (m_arrModifiers[i].Options == WordSprmOptions.sprmCWall
                    || m_arrModifiers[i].Options == WordSprmOptions.sprmPWall
                    || m_arrModifiers[i].Options == WordSprmOptions.sprmTWall
                    || m_arrModifiers[i].Options == WordSprmOptions.sprmSWall)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Adds new modifier to the collection.
        /// </summary>
        /// <param name="modifier">Modifier to add.</param>
        /// <param name="index"></param>
        internal void InsertAt(SinglePropertyModifierRecord modifier, int index)
        {
            m_arrModifiers.Insert(index, modifier);
        }
        /// <summary>
        /// Adds new modifier to the collection.
        /// </summary>
        /// <param name="modifiers">Modifier to add.</param>
        /// <param name="index"></param>
        internal void InsertRangeAt(SinglePropertyModifierArray modifiers, int index)
        {
            m_arrModifiers.InsertRange(index, modifiers.Modifiers);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        internal bool GetBoolean(int options, bool defValue)
        {
            SinglePropertyModifierRecord sprm = TryGetSprm(options);

            if (sprm != null) return sprm.BoolValue;

            return defValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        internal byte GetByte(int options, byte defValue)
        {
            SinglePropertyModifierRecord sprm = TryGetSprm(options);

            if (sprm != null) return sprm.ByteValue;

            return defValue;
        }
        /// <summary>
        /// Returns true if sprm present
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        internal bool HasSprm(int options)
        {
            SinglePropertyModifierRecord sprm = this[options];
            if (sprm != null) 
                return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        internal ushort GetUShort(int options, ushort defValue)
        {
            SinglePropertyModifierRecord sprm = TryGetSprm(options);
            if (sprm != null) return sprm.UshortValue;

            return defValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        internal short GetShort(int options, short defValue)
        {
            SinglePropertyModifierRecord sprm = TryGetSprm(options);

            if (sprm != null) return sprm.ShortValue;

            return defValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="icoe"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        internal int GetInt(int icoe, int defVal)
        {
            SinglePropertyModifierRecord sprm = TryGetSprm(icoe);

            if (sprm != null) return sprm.IntValue;

            return defVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="icoe"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        internal uint GetUInt(int icoe, uint defVal)
        {
            SinglePropertyModifierRecord sprm = TryGetSprm(icoe);

            if (sprm != null) return sprm.UIntValue;

            return defVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        internal byte[] GetByteArray(int options)
        {
            SinglePropertyModifierRecord sprm = TryGetSprm(options);

            if (sprm != null) return sprm.ByteArray;

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="flag"></param>
        internal void SetValue(int options, bool flag)
        {
            GetSPRM(options).BoolValue = flag;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        internal void SetValue(int options, byte value)
        {
            GetSPRM(options).ByteValue = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        internal void SetValue(int options, ushort value)
        {
            GetSPRM(options).UshortValue = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        internal void SetValue(int options, short value)
        {
            GetSPRM(options).ShortValue = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        internal void SetValue(int options, int value)
        {
            GetSPRM(options).IntValue = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        internal void SetValue(int options, uint value)
        {
            GetSPRM(options).UIntValue = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        internal void SetValue(int options, byte[] value)
        {
            GetSPRM(options).ByteArray = value;
        }
        /// <summary>
        /// Saves the table descriptor.
        /// </summary>
        /// <param name="rowDescr"></param>
        internal void SaveTableDescriptor(TableRowDescriptor rowDescr)
        {
            rowDescr.Save(this);
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal SinglePropertyModifierArray Clone()
        {
            SinglePropertyModifierRecord sprm = null;
            SinglePropertyModifierArray sprms = new SinglePropertyModifierArray();
            for (int i = 0, cnt = m_arrModifiers.Count; i < cnt; i++)
            {
                sprm = GetSprmByIndex(i);
                SinglePropertyModifierRecord clonedSprm = sprm.Clone();
                if (clonedSprm != null)
                {
                    sprms.Add(clonedSprm);
                }
            }

            return sprms;
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// List of single property modifiers. Read-only.
        /// </summary>
        internal List<SinglePropertyModifierRecord> Modifiers
        {
            get
            {
                return m_arrModifiers;
            }
        }
        /// <summary>
        /// Indexer by sprm options.
        /// </summary>
        internal SinglePropertyModifierRecord this[int option]
        {
            get
            {
                for (int i = 0, cnt = m_arrModifiers.Count; i < cnt; i++)
                {
                    if (m_arrModifiers[i].TypedOptions == option)
                    {
                        return m_arrModifiers[i];
                    }
                }

                return null;
            }
        }
        //    /// <summary>
        //    /// Indexer property.
        //    /// </summary>
        //    internal SinglePropertyModifierRecord this[ int index ]
        //    {
        //      get
        //      {
        //        if( index < 0 || index >= m_arrModifiers.Count )
        //          throw new ArgumentOutOfRangeException( "index", index, "Value can not be less than 0 and greater than Length" );
        //
        //        return ( SinglePropertyModifierRecord )m_arrModifiers[ index ];
        ////        try
        ////        {
        ////          return ( SinglePropertyModifierRecord )m_arrModifiers[ index ];
        ////        }
        ////        catch
        ////        {
        ////          throw new ArgumentOutOfRangeException( "index", index, "Value can not be less than 0 and greater than Length" );
        ////        }
        //      }
        //    }
        /// <summary>
        /// Returns number of elements in the array. Read-only.
        /// </summary>
        public int Count
        {
            get
            {
                return m_arrModifiers.Count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                int iResult = 0;

                for (int i = 0, len = Count; i < len; i++)
                {
                    iResult += GetSprmByIndex(i).Length;
                }

                return iResult;
            }
        }
        #endregion

        #region ICollection Members
        /// <summary>
        /// 
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                // TODO:  Add SinglePropertyModifierArray.IsSynchronized getter implementation
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            // TODO:  Add SinglePropertyModifierArray.CopyTo implementation
        }
        /// <summary>
        /// 
        /// </summary>
        public object SyncRoot
        {
            get
            {
                // TODO:  Add SinglePropertyModifierArray.SyncRoot getter implementation
                return null;
            }
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new SPRMEnumerator(this);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private SinglePropertyModifierRecord GetSPRM(int options)
        {
            SinglePropertyModifierRecord sprm = this[options];

            if (sprm == null)
            {
                sprm = new SinglePropertyModifierRecord(options);
                Add(sprm);
            }

            return sprm;
        }
        /// <summary>
        /// Gets the sprm by index.
        /// </summary>
        /// <param name="sprmIndex">Index of the SPRM.</param>
        /// <returns></returns>
        internal SinglePropertyModifierRecord GetSprmByIndex(int sprmIndex)
        {
            if (sprmIndex < 0 || sprmIndex >= m_arrModifiers.Count)
                throw new ArgumentOutOfRangeException("index", "Value can not be less than 0 and greater than Length");

            return m_arrModifiers[sprmIndex];
        }
        /// <summary>
        /// Contains the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        internal bool Contain(int option)
        {
            foreach (SinglePropertyModifierRecord sprm in m_arrModifiers)
            {
                if (sprm.Options == option)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Returns corresponding sprm if it
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns></returns>
        internal SinglePropertyModifierRecord TryGetSprm(int options)
        {
            SinglePropertyModifierRecord sprm = this[options];
            //Paragraph property revision check
            if (this.HasSprm(WordSprmOptions.sprmPPropRMark) || this.HasSprm(WordSprmOptions.sprmPPropRMark90))
            {
                for (int i = this.m_arrModifiers.Count - 1; i >= 0; i--)
                {
                    if (this.m_arrModifiers[i].Options == options)
                    {
                        return this.m_arrModifiers[i];
                    }
                }
            }
            //Character property revision check
            if (this.HasSprm(WordSprmOptions.sprmCPropRMark) || this.HasSprm(WordSprmOptions.sprmCPropRMark1))
            {
                for (int i = this.m_arrModifiers.Count - 1; i >= 0; i--)
                {
                    if (this.m_arrModifiers[i].Options == options)
                    {
                        return this.m_arrModifiers[i];
                    }
                }
            }
            //Section property revision check
            if (this.HasSprm(WordSprmOptions.sprmSPgbProp) || this.HasSprm(WordSprmOptions.sprmSPropRMark))
            {
                for (int i = this.m_arrModifiers.Count - 1; i >= 0; i--)
                {
                    if (this.m_arrModifiers[i].Options == options)
                    {
                        return this.m_arrModifiers[i];
                    }
                }
            }
            //Table property revision check
            if (this.HasSprm(WordSprmOptions.sprmTHTMLProps) || this.HasSprm(WordSprmOptions.sprmTPropRMark))
            {
                for (int i = this.m_arrModifiers.Count - 1; i >= 0; i--)
                {
                    if (this.m_arrModifiers[i].Options == options)
                    {
                        return this.m_arrModifiers[i];
                    }
                }
            }
            return sprm;
        }
        #endregion
    }
}
