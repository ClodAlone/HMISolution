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
//using System;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// The properties types
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum WordSprmType
    {
        /// <summary>
        /// Paragraph properties = 1,
        /// </summary>
        ParagraphProperties = 1,

        /// <summary>
        /// Character Properties = 2,
        /// </summary>
        CharacterProperties = 2,

        /// <summary>
        /// Picture Properties = 3,
        /// </summary>
        PictureProperties = 3,

        /// <summary>
        /// Section Properties = 4,
        /// </summary>
        SectionProperties = 4,

        /// <summary>
        /// Table Properties = 5,
        /// </summary>
        TableProperties = 5,
    }

    /// <summary>
    /// Size of the operand for sprm (Single PRoperty Modifier).
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum WordSprmOperandSize
    {
        /// <summary>
        /// 1 byte, operand affects 1 bit.
        /// </summary>
        OneBit = 0,

        /// <summary>
        /// Size of the operand, 1 byte.
        /// </summary>
        OneByte = 1,

        /// <summary>
        ///  Size of the operand, 2 bytes
        /// </summary>
        TwoBytes = 2,

        /// <summary>
        ///  Size of the operand, 4 bytes.
        /// </summary>
        FourBytes = 3,

        /// <summary>
        ///  Size of the operand, TwoBytes2.
        /// </summary>
        TwoBytes2 = 4,

        /// <summary>
        ///  Size of the operand, TwoBytes3.
        /// </summary>
        TwoBytes3 = 5,

        /// <summary>
        /// Variable length - following byte is size of operand.
        /// </summary>
        Variable = 6,

        /// <summary>
        ///  Size of the operand, 3 bytes.
        /// </summary>
        ThreeBytes = 7,
    }

    #region WordSprmOptionType enum
    /// <summary>
    /// Specifies the WordSprmOptionType.
    /// </summary>
    public enum WordSprmOptionType
    {
        sprmPIstd = 0x4600,
        sprmPIstdPermute = 0xC601,
        sprmPIncLvl = 0x2602,
        sprmPJc = 0x2403,
        sprmPFSideBySide = 0x2404,
        sprmPFKeep = 0x2405,
        sprmPFKeepFollow = 0x2406,
        sprmPFPageBreakBefore = 0x2407,
        sprmPBrcl = 0x2408,
        sprmPBrcp = 0x2409,
        sprmPIlvl = 0x260A,
        sprmPIlfo = 0x460B,
        sprmPFNoLineNumb = 0x240C,
        sprmPChgTabsPapx = 0xC60D,
        sprmPDxaRight = 0x840E,
        //// sprmPDxaRight2	      = 0x845E,
        sprmPDxaLeft = 0x840F,
        sprmPNest = 0x4610,
        sprmPDxaLeft1 = 0x8411,
        sprmPDyaLine = 0x6412,
        sprmPDyaBefore = 0xA413,
        sprmPDyaAfter = 0xA414,
        sprmPChgTabs = 0xC615,
        sprmPFInTable = 0x2416,
        sprmPFTtp = 0x2417,
        sprmPDxaAbs = 0x8418,
        sprmPDyaAbs = 0x8419,
        sprmPDxaWidth = 0x841A,
        sprmPPc = 0x261B,
        sprmPBrcTop10 = 0x461C,
        sprmPBrcLeft10 = 0x461D,
        sprmPBrcBottom10 = 0x461E,
        sprmPBrcRight10 = 0x461F,
        sprmPBrcBetween10 = 0x4620,
        sprmPBrcBar10 = 0x4621,
        sprmPDxaFromText10 = 0x4622,
        sprmPWr = 0x2423,
        sprmPBrcTop = 0x6424,
        sprmPBrcLeft = 0x6425,
        sprmPBrcBottom = 0x6426,
        sprmPBrcRight = 0x6427,
        sprmPBrcBetween = 0x6428,
        sprmPBrcBar = 0x6629,
        sprmPBrcTopNew = 0xC64E,
        sprmPBrcLeftNew = 0xC64F,
        sprmPBrcBottomNew = 0xC650,
        sprmPBrcRightNew = 0xC651,
        sprmPFNoAutoHyph = 0x242A,
        sprmPWHeightAbs = 0x442B,
        sprmPDcs = 0x442C,
        sprmPShd = 0x442D,
        sprmPDyaFromText = 0x842E,
        sprmPDxaFromText = 0x842F,
        sprmPFLocked = 0x2430,
        sprmPFWidowControl = 0x2431,
        sprmPRuler = 0xC632,
        sprmPFKinsoku = 0x2433,
        sprmPFWordWrap = 0x2434,
        sprmPFOverflowPunct = 0x2435,
        sprmPFTopLinePunct = 0x2436,
        sprmPFAutoSpaceDE = 0x2437,
        sprmPFAutoSpaceDN = 0x2438,
        sprmPWAlignFont = 0x4439,
        sprmPFrameTextFlow = 0x443A,
        sprmPISnapBaseLine = 0x243B,
        sprmPAnld = 0xC63E,
        sprmPPropRMark = 0xC63F,
        sprmPOutLvl = 0x2640,
        sprmPFBiDi = 0x2441,
        sprmPFNumRMIns = 0x2443,
        sprmPCrLf = 0x2444,
        sprmPNumRM = 0xC645,
        sprmPHugePapx = 0x6645,
        sprmPHugePapx2 = 0x6646,
        sprmPFUsePgsuSettings = 0x2447,
        sprmPFAdjustRight = 0x2448,
        sprmCFRMarkDel = 0x0800,
        sprmCFRMark = 0x0801,
        sprmCFFldVanish = 0x0802,
        sprmCPicLocation = 0x6A03,
        sprmCIbstRMark = 0x4804,
        sprmCDttmRMark = 0x6805,
        sprmCFData = 0x0806,
        sprmCIdslRMark = 0x4807,
        sprmCChs = 0xEA08,
        sprmCSymbol = 0x6A09,
        sprmCFOle2 = 0x080A,
        sprmCIdCharType = 0x480B,
        sprmCHighlight = 0x2A0C,
        sprmCObjLocation = 0x680E,
        sprmCFFtcAsciSymb = 0x2A10,
        sprmCIstd = 0x4A30,
        sprmCIstdPermute = 0xCA31,
        sprmCDefault = 0x2A32,
        sprmCPlain = 0x2A33,
        sprmCKcd = 0x2A34,
        sprmCFBold = 0x0835,
        sprmCFItalic = 0x0836,
        sprmCFStrike = 0x0837,
        sprmCFOutline = 0x0838,
        sprmCFShadow = 0x0839,
        sprmCFSmallCaps = 0x083A,
        sprmCFCaps = 0x083B,
        sprmCFVanish = 0x083C,
        sprmCFtcDefault = 0x4A3D,
        sprmCKul = 0x2A3E,
        sprmCSizePos = 0xEA3F,
        sprmCDxaSpace = 0x8840,
        sprmCLid = 0x4A41,
        sprmCIco = 0x2A42,
        sprmCIcoe = 0x6870,
        sprmCHps = 0x4A43,
        sprmCHpsInc = 0x2A44,
        sprmCHpsPos = 0x4845,
        sprmCHpsPosAdj = 0x2A46,
        sprmCMajority = 0xCA47,
        sprmCIss = 0x2A48,
        sprmCHpsNew50 = 0xCA49,
        sprmCHpsInc1 = 0xCA4A,
        sprmCHpsKern = 0x484B,
        sprmCMajority50 = 0xCA4C,
        sprmCHpsMul = 0x4A4D,
        sprmCYsri = 0x484E,
        sprmCRgFtc0 = 0x4A4F,
        sprmCRgFtc1 = 0x4A50,
        sprmCRgFtc2 = 0x4A51,
        sprmCCharScale = 0x4852,
        sprmCFDStrike = 0x2A53,
        sprmCFImprint = 0x0854,
        sprmCFSpec = 0x0855,
        sprmCFObj = 0x0856,
        sprmCPropRMark = 0xCA57,
        sprmCFEmboss = 0x0858,
        sprmCSfxText = 0x2859,
        sprmCFBiDi = 0x85A,
        sprmCFDiacColor = 0x085B,
        sprmCFBoldBi = 0x85c,
        sprmCFItalicBi = 0x085D,
        sprmCFtcBi = 0x4A5E,
        sprmCLidBi = 0x485F,
        sprmCIcoBi = 0x4A60,
        sprmCHpsBi = 0x4A61,
        sprmCDispFldRMark = 0xCA62,
        sprmCIbstRMarkDel = 0x4863,
        sprmCDttmRMarkDel = 0x6864,
        sprmCBrc = 0x6865,
        sprmCShd = 0x4866,
        sprmCIdslRMarkDel = 0x4867,
        sprmCFUsePgsuSettings = 0x0868,
        sprmCCpg = 0x486B,
        sprmCRgLid0 = 0x486D,
        sprmCRgLid1 = 0x486E,
        sprmCIdctHint = 0x286F,
        sprmPicBrcl = 0x2E00,
        sprmPicScale = 0xCE01,
        sprmPicBrcTop = 0x6C02,
        sprmPicBrcLeft = 0x6C03,
        sprmPicBrcBottom = 0x6C04,
        sprmPicBrcRight = 0x6C05,
        sprmScnsPgn = 0x3000,
        sprmSiHeadingPgn = 0x3001,
        sprmSOlstAnm = 0xD202,
        sprmSDxaColWidth = 0xF203,
        sprmSDxaColSpacing = 0xF204,
        sprmSFEvenlySpaced = 0x3005,
        sprmSFProtected = 0x3006,
        sprmSDmBinFirst = 0x5007,
        sprmSDmBinOther = 0x5008,
        sprmSBkc = 0x3009,
        sprmSFTitlePage = 0x300A,
        sprmSCcolumns = 0x500B,
        ///Footnote/Endnotes Sprms
        sprmSNfcFtnRef = 0x5040,
        sprmSNfcEdnRef = 0x5042,
        sprmSNFtn = 0x503F,
        sprmSNEdn = 0x5041,
        sprmSFpc = 0x303B,
        sprmSRncFtn = 0x303C,
        sprmSRncEdn = 0x303E,
        sprmSDxaColumns = 0x900C,
        sprmSFAutoPgn = 0x300D,
        sprmSNfcPgn = 0x300E,
        sprmSDyaPgn = 0xB00F,
        sprmSDxaPgn = 0xB010,
        sprmSFPgnRestart = 0x3011,
        sprmSFEndnote = 0x3012,

        sprmSLnc = 0x3013,
        sprmSGprfIhdt = 0x3014,
        sprmSNLnnMod = 0x5015,
        sprmSDxaLnn = 0x9016,
        sprmSDyaHdrTop = 0xB017,
        sprmSDyaHdrBottom = 0xB018,
        sprmSLBetween = 0x3019,
        sprmSVjc = 0x301A,
        sprmSLnnMin = 0x501B,
        sprmSPgnStart = 0x501C,
        sprmSBOrientation = 0x301D,
        sprmSBCustomize = 0x301E,
        sprmSXaPage = 0xB01F,
        sprmSYaPage = 0xB020,
        sprmSDxaLeft = 0xB021,
        sprmSDxaRight = 0xB022,
        sprmSDyaTop = 0x9023,
        sprmSDyaBottom = 0x9024,
        sprmSDzaGutter = 0xB025,
        sprmSDmPaperReq = 0x5026,
        sprmSPropRMark = 0xD227,
        sprmSFBiDi = 0x3228,
        sprmSFFacingCol = 0x3229,
        sprmSFRTLGutter = 0x322A,
        sprmSBrcTop = 0x702B,
        sprmSBrcLeft = 0x702C,
        sprmSBrcBottom = 0x702D,
        sprmSBrcRight = 0x702E,
        sprmSPgbProp = 0x522F,
        sprmSDxtCharSpace = 0x7030,
        sprmSDyaLinePitch = 0x9031,
        sprmSClm = 0x5032,
        sprmSTextFlow = 0x5033,
        sprmTJc = 0x5400,
        sprmTDxaLeft = 0x9601,
        sprmTDxaGapHalf = 0x9602,
        sprmTFCantSplit = 0x3403,
        sprmTFCantSplit90 = 0x3466,
        sprmTTableHeader = 0x3404,
        sprmTTableBorders = 0xD605,
        sprmTDefTable10 = 0xD606,
        sprmTDyaRowHeight = 0x9407,
        sprmTDefTable = 0xD608,
        sprmTDefTableShd = 0xD609,
        sprmTTlp = 0x740A,
        sprmTFBiDi = 0x560B,
        sprmTHTMLProps = 0x740C,
        sprmTSetBrc = 0xD620,
        sprmTInsert = 0x7621,
        sprmTDelete = 0x5622,
        sprmTDxaCol = 0x7623,
        sprmTMerge = 0x5624,
        sprmTSplit = 0x5625,
        sprmTSetBrc10 = 0xD626,
        sprmTSetShd = 0x7627,
        sprmTSetShdOdd = 0x7628,
        sprmTTextFlow = 0x7629,
        sprmTDiagLine = 0xD62A,
        sprmTVertMerge = 0xD62B,
        sprmTVertAlign = 0xD62C,
        //// Newly added sprms
        sprmTCellFHideMark = 0xD642,
        sprmPTimeStamp = 0x6467,
        sprmCShdNew = 0xca71,
        sprmPShdNew = 0xc64d,
        sprmTTableBordersNew = 0xd613,
        sprmTCellMargins = 0xd632,
        sprmTTableCellMargins = 0xd634,
        sprmNone = 0x0000,
        sprmUnknown1 = 0x6815,
        sprmUnknown2 = 0x6816,
        sprmCRgLid3 = 0x4873,
        sprmCRgLid3_2 = 0x4874,
        sprmPSubTableCellEnd = 0x244B,
        sprmPSubTableRowEnd = 0x244C,
        sprmTTopBorderColor = 0xd61a,
        sprmTLeftBorderColor = 0xd61b,
        sprmTBottomBorderColor = 0xd61c,
        sprmTRightBorderColor = 0xd61d,
        sprmTCellSpacing = 0xd633,
        sprmTAutoResizeCells = 0x3615,
        sprmSBrcBottomNew = 0xd236,
        sprmSBrcLeftNew = 0xd235,
        sprmSBrcRightNew = 0xd237,
        sprmSBrcTopNew = 0xd234,
        //// Debug sprms
        sprmPDxaLeft1Bi = 0x8460,
        sprmPTableProps = 0x646B,
        sprmTIstd = 0x563A,
        sprmTNestingLevel = 0x6649,
        sprmTableUnknown1 = 0xf203,
        sprmTableUnknown2 = 0xf204,
        sprmTPreferredWidth = 0xf614,
        sprmTWidthBefore = 0xf617,
        sprmTWidthAfter = 0xf618,
        sprmTWidthIndent = 0xf661,
        sprmCUnderlineColor = 0x6877,
        sprmCFNoProof = 0x875,
        //// Bidi support
        sprmPJcBi = 0x2461,
        sprmPDxaLeftBi = 0x845e,
        sprmPDxaRightBi = 0x845d,
        sprmPFBeforeAuto = 0x245b,
        sprmPFAfterAuto = 0x245c,

        sprmTPropRMark = 0xd667,
        sprmTPositionCode = 0x360d,
        sprmTFrameLeft = 0x940e,
        sprmTFrameTop = 0x940f,
        sprmTFromTextBottom = 0x941f,
        sprmTFromTextLeft = 0x9410,
        sprmTFromTextRight = 0x941e,
        sprmTFromTextTop = 0x9411,
        sprmTCellShdNew = 0xd612,
        sprmTCellShdNew2 = 0xd616,
        SprmTCellShdNew2Dup = 0xd671,
        sprmTCellShdNew3 = 0xd60c,
        SprmTCellShdNew3Dup = 0xd673,
        sprmTCellShdNewDup = 0xd670,
        sprmTTableShd = 0xd660,
        sprmPUndocumented2462 = 0x2462,
        sprmPUndocumented4458 = 0x4458,
        sprmPUndocumented4459 = 0x4459,
        sprmPUndocumented6465 = 0x6465,
        sprmPUndocumented6654 = 0x6654,
        sprmPUndocumentedC653 = 0xc653,
        sprmPUndocumentedC66C = 0xc66c,
        sprmCUndocumented2879 = 0x2879,
        sprmCUndocumented2A86 = 0x2a86,
        sprmCPbiHasImage = 0x4888,
        sprmCUndocumented6815 = 0x6815,
        sprmCUndocumented6816 = 0x6816,
        sprmCUndocumented6817 = 0x6817,
        sprmCPbiImageIndex = 0x6887,
        sprmCUndocumented811 = 0x811,

        /// <summary>
        /// newly added
        /// </summary>
        sprmCUndocumentedSpacing = 0x6800,
        sprmCUndocumentedRevisionProblem = 0xa00,
        sprmCUndocumented1 = 0x2a00,
        sprmCUndocumented2 = 0x6900,
        sprmCUndocumented3 = 0xc800,
        sprmCUndocumented4 = 0x6a00,
        sprmCUndocumented5 = 0x4a00,
        sprmCUndocumented6 = 0xa900,
        sprmCUndocumented7 = 0xaa00,
        sprmCUndocumented8 = 0xab00,
        sprmTCellBrcType = 0xd662,
        sprmCPropRMark1 = 0xca89,
        sprmCWall = 0x2a83,
        sprmPWall = 0x2664,
        sprmPPropRMark90 = 0xc66f,
        sprmTWall = 0x3668,
        sprmPFContSpacing = 0x246d,
        sprmSWall = 0x3239,
        //Paragraph indentations in character units.
        sprmPDxcLeft = 0x4456,
        sprmPDxcLeft1 = 0x4457,
        sprmPDxcRight = 0x4455,
        sprmTFNoAllowOverlap = 0x3465
    }
    #endregion

    /// <summary>
    /// Specifies word complex block type.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum WordComplexBlockType
    {
        /// <summary>
        /// Specifies Type as Sprms.
        /// </summary>
        Sprms = 1,

        /// <summary>
        /// Specifies Type as PieceTable.
        /// </summary>
        PieceTable = 2,
    }

    /// <summary>
    /// Style type.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum WordStyleType
    {
        /// <summary>
        /// Specifies Paragraph style.
        /// </summary>
        ParagraphStyle = 1,

        /// <summary>
        /// Specifies character style.
        /// </summary>
        CharacterStyle = 2,

        /// <summary>
        /// Specifies Table style.
        /// </summary>
        TableStyle = 3,

        /// <summary>
        /// Specifies List style.
        /// </summary>
        ListStyle = 4,
    }

    /// <summary>
    /// Paragraph justification code.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum ParagraphJustify
    {
        /// <summary>
        /// Left justification.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Center justification.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Right justification.
        /// </summary>
        Right = 2,

        /// <summary>
        /// LeftAndRight justification.
        /// </summary>
        LeftAndRight = 3
    }

    /// <summary>
    /// Word Sprm Options.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordSprmOptions
    {
        #region Constants
        public const int sprmPIstd = 0x4600;
        public const int sprmPIstdPermute = 0xC601;
        public const int sprmPIncLvl = 0x2602;
        public const int sprmPJc = 0x2403;
        public const int sprmPFSideBySide = 0x2404;
        public const int sprmPFKeep = 0x2405;
        public const int sprmPFKeepFollow = 0x2406;
        public const int sprmPFPageBreakBefore = 0x2407;
        public const int sprmPBrcl = 0x2408;
        public const int sprmPBrcp = 0x2409;
        public const int sprmPIlvl = 0x260A;
        public const int sprmPIlfo = 0x460B;
        public const int sprmPFNoLineNumb = 0x240C;
        public const int sprmPChgTabsPapx = 0xC60D;
        public const int sprmPDxaRight = 0x840E;
        //// sprmPDxaRight2	      = 0x845E,
        public const int sprmPDxaLeft = 0x840F;
        public const int sprmPNest = 0x4610;
        public const int sprmPDxaLeft1 = 0x8411;
        public const int sprmPDyaLine = 0x6412;
        public const int sprmPDyaBefore = 0xA413;
        public const int sprmPDyaAfter = 0xA414;
        public const int sprmPChgTabs = 0xC615;
        public const int sprmPFInTable = 0x2416;
        public const int sprmPFTtp = 0x2417;
        public const int sprmPDxaAbs = 0x8418;
        public const int sprmPDyaAbs = 0x8419;
        public const int sprmPDxaWidth = 0x841A;
        public const int sprmPPc = 0x261B;
        public const int sprmPBrcTop10 = 0x461C;
        public const int sprmPBrcLeft10 = 0x461D;
        public const int sprmPBrcBottom10 = 0x461E;
        public const int sprmPBrcRight10 = 0x461F;
        public const int sprmPBrcBetween10 = 0x4620;
        public const int sprmPBrcBar10 = 0x4621;
        public const int sprmPDxaFromText10 = 0x4622;
        public const int sprmPWr = 0x2423;
        public const int sprmPBrcTop = 0x6424;
        public const int sprmPBrcLeft = 0x6425;
        public const int sprmPBrcBottom = 0x6426;
        public const int sprmPBrcRight = 0x6427;
        public const int sprmPBrcBetween = 0x6428;
        public const int sprmPBrcBar = 0x6629;
        public const int sprmPBrcTopNew = 0xC64E;
        public const int sprmPBrcLeftNew = 0xC64F;
        public const int sprmPBrcBottomNew = 0xC650;
        public const int sprmPBrcRightNew = 0xC651;
        public const int sprmPFNoAutoHyph = 0x242A;
        public const int sprmPWHeightAbs = 0x442B;
        public const int sprmPDcs = 0x442C;
        public const int sprmPShd = 0x442D;
        public const int sprmPDyaFromText = 0x842E;
        public const int sprmPDxaFromText = 0x842F;
        public const int sprmPFLocked = 0x2430;
        public const int sprmPFMirrorIndents = 0x2470;
        public const int sprmPFWidowControl = 0x2431;
        public const int sprmPRuler = 0xC632;
        public const int sprmPFKinsoku = 0x2433;
        public const int sprmPFWordWrap = 0x2434;
        public const int sprmPFOverflowPunct = 0x2435;
        public const int sprmPFTopLinePunct = 0x2436;
        public const int sprmPFAutoSpaceDE = 0x2437;
        public const int sprmPFAutoSpaceDN = 0x2438;
        public const int sprmPWAlignFont = 0x4439;
        public const int sprmPFrameTextFlow = 0x443A;
        public const int sprmPISnapBaseLine = 0x243B;
        public const int sprmPAnld = 0xC63E;
        public const int sprmPPropRMark = 0xC63F;
        public const int sprmPOutLvl = 0x2640;
        public const int sprmPFBiDi = 0x2441;
        public const int sprmPFNumRMIns = 0x2443;
        public const int sprmPCrLf = 0x2444;
        public const int sprmPNumRM = 0xC645;
        public const int sprmPHugePapx = 0x6645;
        public const int sprmPHugePapx2 = 0x6646;
        public const int sprmPFUsePgsuSettings = 0x2447;
        public const int sprmPFAdjustRight = 0x2448;
        public const int sprmCFRMarkDel = 0x0800;
        public const int sprmCFRMark = 0x0801;
        public const int sprmCFFldVanish = 0x0802;
        public const int sprmCPicLocation = 0x6A03;
        public const int sprmCIbstRMark = 0x4804;
        public const int sprmCDttmRMark = 0x6805;
        public const int sprmCFData = 0x0806;
        public const int sprmCIdslRMark = 0x4807;
        public const int sprmCChs = 0xEA08;
        public const int sprmCSymbol = 0x6A09;
        public const int sprmCFOle2 = 0x080A;
        public const int sprmCIdCharType = 0x480B;
        public const int sprmCHighlight = 0x2A0C;
        public const int sprmCObjLocation = 0x680E;
        public const int sprmCFFtcAsciSymb = 0x2A10;
        public const int sprmCIstd = 0x4A30;
        public const int sprmCIstdPermute = 0xCA31;
        public const int sprmCDefault = 0x2A32;
        public const int sprmCPlain = 0x2A33;
        public const int sprmCKcd = 0x2A34;
        public const int sprmCFBold = 0x0835;
        public const int sprmCFItalic = 0x0836;
        public const int sprmCFStrike = 0x0837;
        public const int sprmCFOutline = 0x0838;
        public const int sprmCFShadow = 0x0839;
        public const int sprmCFSmallCaps = 0x083A;
        public const int sprmCFCaps = 0x083B;
        public const int sprmCFVanish = 0x083C;
        public const int sprmCFtcDefault = 0x4A3D;
        public const int sprmCKul = 0x2A3E;
        public const int sprmCSizePos = 0xEA3F;
        public const int sprmCDxaSpace = 0x8840;
        public const int sprmCLid = 0x4A41;
        public const int sprmCIco = 0x2A42;
        public const int sprmCIcoe = 0x6870;
        public const int sprmCHps = 0x4A43;
        public const int sprmCHpsInc = 0x2A44;
        public const int sprmCHpsPos = 0x4845;
        public const int sprmCHpsPosAdj = 0x2A46;
        public const int sprmCMajority = 0xCA47;
        public const int sprmCIss = 0x2A48;
        public const int sprmCHpsNew50 = 0xCA49;
        public const int sprmCHpsInc1 = 0xCA4A;
        public const int sprmCHpsKern = 0x484B;
        public const int sprmCMajority50 = 0xCA4C;
        public const int sprmCHpsMul = 0x4A4D;
        public const int sprmCYsri = 0x484E;
        public const int sprmCRgFtc0 = 0x4A4F;
        public const int sprmCRgFtc1 = 0x4A50;
        public const int sprmCRgFtc2 = 0x4A51;
        public const int sprmCCharScale = 0x4852;
        public const int sprmCFDStrike = 0x2A53;
        public const int sprmCFImprint = 0x0854;
        public const int sprmCFSpec = 0x0855;
        public const int sprmCFObj = 0x0856;
        public const int sprmCPropRMark = 0xCA57;
        public const int sprmCFEmboss = 0x0858;
        public const int sprmCSfxText = 0x2859;
        public const int sprmCFBiDi = 0x85A;
        public const int sprmCFDiacColor = 0x085B;
        public const int sprmCFComplexScripts = 0x0882;
        public const int sprmCFBoldBi = 0x85c;
        public const int sprmCFItalicBi = 0x085D;
        public const int sprmCFtcBi = 0x4A5E;
        public const int sprmCLidBi = 0x485F;
        public const int sprmCIcoBi = 0x4A60;
        public const int sprmCHpsBi = 0x4A61;
        public const int sprmCDispFldRMark = 0xCA62;
        public const int sprmCIbstRMarkDel = 0x4863;
        public const int sprmCDttmRMarkDel = 0x6864;
        public const int sprmCBrc = 0x6865;
        public const int sprmCShd = 0x4866;
        public const int sprmCIdslRMarkDel = 0x4867;
        public const int sprmCFUsePgsuSettings = 0x0868;
        public const int sprmCCpg = 0x486B;
        public const int sprmCRgLid0 = 0x486D;
        public const int sprmCRgLid1 = 0x486E;
        public const int sprmCIdctHint = 0x286F;
        public const int sprmPicBrcl = 0x2E00;
        public const int sprmPicScale = 0xCE01;
        public const int sprmPicBrcTop = 0x6C02;
        public const int sprmPicBrcLeft = 0x6C03;
        public const int sprmPicBrcBottom = 0x6C04;
        public const int sprmPicBrcRight = 0x6C05;
        public const int sprmScnsPgn = 0x3000;
        public const int sprmSiHeadingPgn = 0x3001;
        public const int sprmSOlstAnm = 0xD202;
        public const int sprmSDxaColWidth = 0xF203;
        public const int sprmSDxaColSpacing = 0xF204;
        public const int sprmSFEvenlySpaced = 0x3005;
        public const int sprmSFProtected = 0x3006;
        public const int sprmSDmBinFirst = 0x5007;
        public const int sprmSDmBinOther = 0x5008;
        public const int sprmSBkc = 0x3009;
        public const int sprmSFTitlePage = 0x300A;
        public const int sprmSCcolumns = 0x500B;
        ///Footnote/Endnotes Sprms
        public const int sprmSNfcFtnRef = 0x5040;
        public const int sprmSNfcEdnRef = 0x5042;
        public const int sprmSNFtn = 0x503F;
        public const int sprmSNEdn = 0x5041;
        public const int sprmSFpc = 0x303B;
        public const int sprmSRncFtn = 0x303C;
        public const int sprmSRncEdn = 0x303E;
        public const int sprmSDxaColumns = 0x900C;
        public const int sprmSFAutoPgn = 0x300D;
        public const int sprmSNfcPgn = 0x300E;
        public const int sprmSDyaPgn = 0xB00F;
        public const int sprmSDxaPgn = 0xB010;
        public const int sprmSFPgnRestart = 0x3011;
        public const int sprmSFEndnote = 0x3012;

        public const int sprmSLnc = 0x3013;
        public const int sprmSGprfIhdt = 0x3014;
        public const int sprmSNLnnMod = 0x5015;
        public const int sprmSDxaLnn = 0x9016;
        public const int sprmSDyaHdrTop = 0xB017;
        public const int sprmSDyaHdrBottom = 0xB018;
        public const int sprmSLBetween = 0x3019;
        public const int sprmSVjc = 0x301A;
        public const int sprmSLnnMin = 0x501B;
        public const int sprmSPgnStart = 0x501C;
        public const int sprmSBOrientation = 0x301D;
        public const int sprmSBCustomize = 0x301E;
        public const int sprmSXaPage = 0xB01F;
        public const int sprmSYaPage = 0xB020;
        public const int sprmSDxaLeft = 0xB021;
        public const int sprmSDxaRight = 0xB022;
        public const int sprmSDyaTop = 0x9023;
        public const int sprmSDyaBottom = 0x9024;
        public const int sprmSDzaGutter = 0xB025;
        public const int sprmSDmPaperReq = 0x5026;
        public const int sprmSPropRMark = 0xD227;
        public const int sprmSFBiDi = 0x3228;
        public const int sprmSFFacingCol = 0x3229;
        public const int sprmSFRTLGutter = 0x322A;
        public const int sprmSBrcTop = 0x702B;
        public const int sprmSBrcLeft = 0x702C;
        public const int sprmSBrcBottom = 0x702D;
        public const int sprmSBrcRight = 0x702E;
        public const int sprmSPgbProp = 0x522F;
        public const int sprmSDxtCharSpace = 0x7030;
        public const int sprmSDyaLinePitch = 0x9031;
        public const int sprmSClm = 0x5032;
        public const int sprmSTextFlow = 0x5033;
        public const int sprmTJc = 0x5400;
        public const int sprmTDxaLeft = 0x9601;
        public const int sprmTDxaGapHalf = 0x9602;
        public const int sprmTFCantSplit = 0x3403;
        public const int sprmTFCantSplit90 = 0x3466;
        public const int sprmTTableHeader = 0x3404;
        public const int sprmTTableBorders = 0xD605;
        public const int sprmTDefTable10 = 0xD606;
        public const int sprmTDyaRowHeight = 0x9407;
        public const int sprmTDefTable = 0xD608;
        public const int sprmTDefTableShd = 0xD609;
        public const int sprmTTlp = 0x740A;
        public const int sprmTFBiDi = 0x560B;
        public const int sprmTHTMLProps = 0x740C;
        public const int sprmTSetBrc = 0xD620;
        public const int sprmTInsert = 0x7621;
        public const int sprmTDelete = 0x5622;
        public const int sprmTDxaCol = 0x7623;
        public const int sprmTMerge = 0x5624;
        public const int sprmTSplit = 0x5625;
        public const int sprmTSetBrc10 = 0xD626;
        public const int sprmTSetShd = 0x7627;
        public const int sprmTSetShdOdd = 0x7628;
        public const int sprmTTextFlow = 0x7629;
        public const int sprmTDiagLine = 0xD62A;
        public const int sprmTVertMerge = 0xD62B;
        public const int sprmTVertAlign = 0xD62C;
        // Newly added sprms
        public const int sprmTCellFHideMark = 0xD642;
        public const int sprmPTimeStamp = 0x6467;
        public const int sprmCShdNew = 0xca71;
        public const int sprmPShdNew = 0xc64d;
        public const int sprmTTableBordersNew = 0xd613;
        public const int sprmTCellMargins = 0xd632;
        public const int sprmTTableCellMargins = 0xd634;
        public const int sprmNone = 0x0000;
        public const int sprmUnknown1 = 0x6815;
        public const int sprmUnknown2 = 0x6816;
        public const int sprmCRgLid3 = 0x4873;
        public const int sprmCRgLid3_2 = 0x4874;
        public const int sprmPSubTableCellEnd = 0x244B;
        public const int sprmPSubTableRowEnd = 0x244C;
        public const int sprmTTopBorderColor = 0xd61a;
        public const int sprmTLeftBorderColor = 0xd61b;
        public const int sprmTBottomBorderColor = 0xd61c;
        public const int sprmTRightBorderColor = 0xd61d;
        public const int sprmTCellSpacing = 0xd633;
        public const int sprmTAutoResizeCells = 0x3615;
        public const int sprmSBrcBottomNew = 0xd236;
        public const int sprmSBrcLeftNew = 0xd235;
        public const int sprmSBrcRightNew = 0xd237;
        public const int sprmSBrcTopNew = 0xd234;
        // Debug sprms
        public const int sprmPDxaLeft1Bi = 0x8460;
        public const int sprmPTableProps = 0x646B;
        public const int sprmTIstd = 0x563A;
        public const int sprmTNestingLevel = 0x6649;
        public const int sprmTableUnknown1 = 0xf203;
        public const int sprmTableUnknown2 = 0xf204;
        public const int sprmTPreferredWidth = 0xf614;
        public const int sprmTWidthBefore = 0xf617;
        public const int sprmTWidthAfter = 0xf618;
        public const int sprmTWidthIndent = 0xf661;
        public const int sprmCUnderlineColor = 0x6877;
        public const int sprmCFNoProof = 0x875;
        // Bidi support
        public const int sprmPJcBi = 0x2461;
        public const int sprmPDxaLeftBi = 0x845e;
        public const int sprmPDxaRightBi = 0x845d;
        public const int sprmPFBeforeAuto = 0x245b;
        public const int sprmPFAfterAuto = 0x245c;

        public const int sprmTPropRMark = 0xd667;
        public const int sprmTPositionCode = 0x360d;
        public const int sprmTFrameLeft = 0x940e;
        public const int sprmTFrameTop = 0x940f;
        public const int sprmTFromTextBottom = 0x941f;
        public const int sprmTFromTextLeft = 0x9410;
        public const int sprmTFromTextRight = 0x941e;
        public const int sprmTFromTextTop = 0x9411;
        public const int sprmTCellShdNew = 0xd612;
        public const int sprmTCellShdNew2 = 0xd616;
        public const int SprmTCellShdNew2Dup = 0xd671;
        public const int sprmTCellShdNew3 = 0xd60c;
        public const int SprmTCellShdNew3Dup = 0xd673;
        public const int sprmTCellShdNewDup = 0xd670;
        public const int sprmTTableShd = 0xd660;
        public const int sprmPUndocumented2462 = 0x2462;
        public const int sprmPUndocumented4458 = 0x4458;
        public const int sprmPUndocumented4459 = 0x4459;
        public const int sprmPUndocumented6465 = 0x6465;
        public const int sprmPUndocumented6654 = 0x6654;
        public const int sprmPUndocumentedC653 = 0xc653;
        public const int sprmPUndocumentedC66C = 0xc66c;
        public const int sprmCUndocumented2879 = 0x2879;
        public const int sprmCUndocumented2A86 = 0x2a86;
        public const int sprmCPbiHasImage = 0x4888;
        public const int sprmCUndocumented6815 = 0x6815;
        public const int sprmCUndocumented6816 = 0x6816;
        public const int sprmCUndocumented6817 = 0x6817;
        public const int sprmCPbiImageIndex = 0x6887;
        public const int sprmCUndocumented811 = 0x811;

        /// <summary>
        /// newly added
        /// </summary>
        public const int sprmCUndocumentedSpacing = 0x6800;
        public const int sprmCUndocumentedRevisionProblem = 0xa00;
        public const int sprmCUndocumented1 = 0x2a00;
        public const int sprmCUndocumented2 = 0x6900;
        public const int sprmCUndocumented3 = 0xc800;
        public const int sprmCUndocumented4 = 0x6a00;
        public const int sprmCUndocumented5 = 0x4a00;
        public const int sprmCUndocumented6 = 0xa900;
        public const int sprmCUndocumented7 = 0xaa00;
        public const int sprmCUndocumented8 = 0xab00;
        public const int sprmTCellBrcType = 0xd662;
        public const int sprmCPropRMark1 = 0xca89;
        public const int sprmCWall = 0x2a83;
        public const int sprmPWall = 0x2664;
        public const int sprmPPropRMark90 = 0xc66f;
        public const int sprmTWall = 0x3668;
        public const int sprmPFContSpacing = 0x246d;
        public const int sprmSWall = 0x3239;
        //Paragraph indentations in character units.
        public const int sprmPDxcLeft = 0x4456;
        public const int sprmPDxcLeft1 = 0x4457;
        public const int sprmPDxcRight = 0x4455;
        public const int sprmTFNoAllowOverlap = 0x3465;
        #endregion
    }
}
