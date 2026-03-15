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
using System.IO;
using System.Runtime.InteropServices;
using Syncfusion.DocIO.ReaderWriter.Security;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for DOPStructure.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DOPDescriptor : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_PROTECTION_KEY = 0;//0x56ae3201;
        /// <summary>
        /// Maximum length of the password.
        /// </summary>
        internal const int DEF_MAX_PASSWORDLEN = 15;
        /// <summary>
        /// Default password hash value.
        /// </summary>
        private const ushort DEF_PASSWORD_CONST = 0xCE4B;
        #endregion

        #region Class members
        private byte m_nfcEdnRef = 2;
        private byte m_nfcFtnRef = 0;
        private byte m_bepc = 3;
        private int m_nEdn = 1;
        private byte m_rncEdn = 0;
        private int m_nFtn = 1;
        private byte m_rncFtn = 0;
        private bool m_bFacingPage = false;
        private bool m_bWidowControl = true;
        private bool m_bPMHMainDoc = false;
        private int m_grfSuppression = 0;
        private byte m_fpc = 1;
        private int m_unused0_7 = 0;
        private int m_grpfIhdt = 0;
        private byte m_bOutlineDirtySave = 241;
        private byte m_prot1 = 8;
        private bool m_bBackup = false;
        private bool m_bExactCWords = true;
        private bool m_bPagHidden = true;
        private bool m_bPagResults = false;
        internal bool m_bLockAtn = false;
        private bool m_bMirrorMargins = false;
        private bool m_unused6_6 = false;
        private bool m_bDfltTrueType = true;
        private bool m_bPagSuppressTopSpacin = false;
        internal bool m_bProtEnabled = false;
        private bool m_bDispFormFldSel = false;
        private bool m_bRMView = true;
        private bool m_bRMPrint = true;
        private bool m_unused7_5 = false;
        internal bool m_bLockRev = false;
        private bool m_bEmbedFonts = false;
        private Copts60 m_copts60;
        private ushort m_dxaTabs = 720;
        private int m_wSpare = 1251;
        private int m_dxaHotZ = 360;
        private int m_cConsecHypLim = 0;
        private int m_wSpare2 = 0;
        private uint m_dttmCreated = 0;
        private uint m_dttmRevised = 0;
        private uint m_dttmLastPrint = 0;
        private int m_nRevision = 0;
        private int m_tmEdited = 0;
        private int m_cWords = 0;
        private int m_cCh = 0;
        private int m_cPg = 0;
        private int m_cParas = 0;
        private ushort m_End = 4;
        private ushort m_epc = 4099;
        private int m_cLines = 0;
        private int m_wordsFtnEnd = 0;
        private int m_cChFtnEdn = 0;
        private int m_cPgFtnEdn = 0;
        private int m_cParasFrnEdn = 0;
        private int m_cLinesFtnEdn = 0;
        private uint m_lKeyProtDoc = 0;
        private ushort m_wvkSaved = 801;
        private bool m_shadeFormData = true;
        private bool m_trackChanges;
        internal byte[] m_dopLeftData;
        private DateTime m_created = DateTime.Now;
        private DateTime m_revised = DateTime.Now;
        private DateTime m_lastPrinted = DateTime.MinValue;
        private TimeSpan m_editTime = TimeSpan.MinValue;
        private DOP95 m_dop95;
        private DOP97 m_dop97;
        private DOP2000 m_dop2000;
        private DOP2002 m_dop2002;
        private DOP2003 m_dop2003;
        private DOP2007 m_dop2007;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the dop95.
        /// </summary>
        /// <value>The dop95.</value>
        internal DOP95 Dop95
        {
            get
            {
                if (m_dop95 == null)
                    m_dop95 = new DOP95(this);
                return m_dop95;
            }
        }
        /// <summary>
        /// Gets the dop97.
        /// </summary>
        /// <value>The dop97.</value>
        internal DOP97 Dop97
        {
            get
            {
                if (m_dop97 == null)
                    m_dop97 = new DOP97(this);
                return m_dop97;
            }
        }
        /// <summary>
        /// Gets the dop2000.
        /// </summary>
        /// <value>The dop2000.</value>
        internal DOP2000 Dop2000
        {
            get
            {
                if (m_dop2000 == null)
                    m_dop2000 = new DOP2000(this);
                return m_dop2000;
            }
        }
        /// <summary>
        /// Gets the dop2002.
        /// </summary>
        /// <value>The dop2002.</value>
        internal DOP2002 Dop2002
        {
            get
            {
                if (m_dop2002 == null)
                    m_dop2002 = new DOP2002(this);
                return m_dop2002;
            }
        }
        /// <summary>
        /// Gets the dop2003.
        /// </summary>
        /// <value>The dop2003.</value>
        internal DOP2003 Dop2003
        {
            get
            {
                if (m_dop2003 == null)
                    m_dop2003 = new DOP2003(this);
                return m_dop2003;
            }
        }
        /// <summary>
        /// Gets the dop2007.
        /// </summary>
        /// <value>The dop2007.</value>
        internal DOP2007 Dop2007
        {
            get
            {
                if (m_dop2007 == null)
                    m_dop2007 = new DOP2007(this);
                return m_dop2007;
            }
        }
        /// <summary>
        /// Gets the copts60.
        /// </summary>
        /// <value>The copts60.</value>
        internal Copts60 Copts60
        {
            get
            {
                if (m_copts60 == null)
                    m_copts60 = new Copts60(this);
                return m_copts60;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte EndnoteNumberFormat
        {
            get
            {
                return (byte)Dop97.NfcEdnRef;
            }
            set
            {
                if (Dop97.NfcEdnRef >= 0 && Dop97.NfcEdnRef < 5)
                    Dop97.NfcEdnRef = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte FootnoteNumberFormat
        {
            get
            {
                return (byte)Dop97.NfcFtnRef;
            }
            set
            {
                if (Dop97.NfcFtnRef >= 0 && Dop97.NfcFtnRef < 5)
                    Dop97.NfcFtnRef = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte EndnotePosition
        {
            get
            {
                return m_bepc;
            }
            set
            {
                m_bepc = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int InitialEndnoteNumber
        {
            get
            {
                return m_nEdn;
            }
            set
            {
                m_nEdn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte RestartIndexForEndnote
        {
            get
            {
                return m_rncEdn;
            }
            set
            {
                if (m_rncEdn < 3 && m_rncEdn >= 0)
                    m_rncEdn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int InitialFootnoteNumber
        {
            get
            {
                return m_nFtn;
            }
            set
            {
                m_nFtn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte RestartIndexForFootnotes
        {
            get
            {
                return m_rncFtn;
            }
            set
            {
                if (value >= 0 && value < 3)
                    m_rncFtn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte FootnotePosition
        {
            get
            {
                return m_fpc;
            }
            set
            {
                if (value >= 0 && value < 3)
                    m_fpc = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ProtectionType ProtectionType
        {
            get
            {
                if (!Dop2003.EnforceDocProt || Dop2003.DocProtCur == 7)
                    return ProtectionType.NoProtection;

                if (m_bProtEnabled && Dop2003.DocProtCur == 2)
                {
                    return ProtectionType.AllowOnlyFormFields;
                }
                if (m_bLockAtn)
                {
                    if (Dop2003.TreatLockAtnAsReadOnly && Dop2003.DocProtCur == 3)
                        return ProtectionType.AllowOnlyReading;
                    else if (Dop2003.DocProtCur == 1)
                        return ProtectionType.AllowOnlyComments;
                }
                if (m_bLockRev && Dop2003.DocProtCur == 0)
                {
                    return ProtectionType.AllowOnlyRevisions;
                }
                return ProtectionType.NoProtection;
            }
            set
            {
                m_bLockAtn = false;
                m_bProtEnabled = false;
                m_bLockRev = false;
                Dop2003.EnforceDocProt = false;
                Dop2003.DocProtCur = 3;
                switch (value)
                {
                    case ProtectionType.AllowOnlyComments:
                        {
                            Dop2003.EnforceDocProt = true;
                            Dop2003.DocProtCur = 1;
                            m_bLockAtn = true;
                            break;
                        }
                    case ProtectionType.AllowOnlyFormFields:
                        {
                            Dop2003.EnforceDocProt = true;
                            Dop2003.DocProtCur = 2;
                            m_bProtEnabled = true;
                            break;
                        }
                    case ProtectionType.AllowOnlyRevisions:
                        {
                            Dop2003.EnforceDocProt = true;
                            Dop2003.DocProtCur = 0;
                            m_bLockRev = true;
                            break;
                        }
                    case ProtectionType.AllowOnlyReading:
                        {
                            m_bLockAtn = true;
                            Dop2003.TreatLockAtnAsReadOnly = true;
                            Dop2003.EnforceDocProt = true;
                            Dop2003.DocProtCur = 3;
                            break;
                        }
                    case ProtectionType.NoProtection:
                        {
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("Unknown protection specified.");
                        }
                }
                if ((ProtectionType != ProtectionType.NoProtection) && (m_lKeyProtDoc == 0))
                {
                    m_lKeyProtDoc = DEF_PROTECTION_KEY;
                }
            }
        }
        /// <summary>
        /// Gets the protection key.
        /// </summary>
        /// <value>The protection key.</value>
        internal uint ProtectionKey
        {
            get
            {
                return m_lKeyProtDoc;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool OddAndEvenPagesHeaderFooter
        {
            get
            {
                return m_bFacingPage;
            }
            set
            {
                m_bFacingPage = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte ViewType
        {
            get
            {
                return (byte)(m_wvkSaved & 0x0007);
            }
            set
            {
                m_wvkSaved = (ushort)(m_wvkSaved & 0xfff8);
                m_wvkSaved = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort ZoomPercent
        {
            get
            {
                return (ushort)((m_wvkSaved & 0x0ff8) >> 3);
            }
            set
            {
                m_wvkSaved = (ushort)(m_wvkSaved & 0xf007);
                m_wvkSaved += (ushort)(value << 3);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte ZoomType
        {
            get
            {
                return (byte)((m_wvkSaved & 0x3000) >> 12);
            }
            set
            {
                m_wvkSaved = (ushort)(m_wvkSaved & 0xcfff);
                m_wvkSaved += (ushort)(value << 12);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte GuuterPosition
        {
            get
            {
                return (byte)(m_wvkSaved & 0x8000);
            }
            set
            {
                m_wvkSaved = (ushort)(m_wvkSaved & ~0x8000);
                m_wvkSaved += value;
            }
        }
        /// <summary>
        /// Gets or sets the width of the default tab.
        /// </summary>
        /// <value>The width of the default tab.</value>
        internal ushort DefaultTabWidth
        {
            get
            {
                return m_dxaTabs;
            }
            set
            {
                m_dxaTabs = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte Formatting1
        {
            get
            {
                return m_prot1;
            }
            set
            {
                m_prot1 = value;
            }
        }
        /// <summary>
        /// Specifies the maximum amount of white space, in twips, allowed at the end of the line before attempting to hyphenate the next word.
        /// </summary>
        internal int DxaHotZ
        {
            get
            {
                return m_dxaHotZ;
            }
            set
            {
                m_dxaHotZ = value;
            }
        }
        /// <summary>
        /// Specifies the maximum number of consecutive lines that can end in a hyphenated word before ignoring automatic hyphenation rules for a line.
        /// </summary>
        internal int ConsecHypLim
        {
            get
            {
                return m_cConsecHypLim;
            }
            set
            {
                m_cConsecHypLim = value;
            }
        }
        /// <summary>
        /// Specifies the capital letters are hyphenated in a given document
        /// </summary>
        internal bool HyphCapitals
        {
            get
            {
                return ((m_prot1 & 0x8) >> 3) != 0;
            }
            set
            {
                m_prot1 = (byte)((m_prot1 & 0xF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// When 1, Word will hyphenate newly typed text as a background task
        /// </summary>
        internal bool AutoHyphen
        {
            get
            {
                return ((m_prot1 & 0x10) >> 4) != 0;
            }
            set
            {
                m_prot1 = (byte)((m_prot1 & 0xEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        ///when 1, Word will merge styles from its template
        /// </summary>
        internal bool LinkStyles
        {
            get
            {
                return ((m_prot1 & 0x40) >> 6) != 0;
            }
            set
            {
                m_prot1 = (byte)((m_prot1 & 0xBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] DopInternalData
        {
            get
            {
                return m_dopLeftData;
            }
            set
            {
                m_dopLeftData = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether to mirror page margins.
        /// </summary>
        /// <value><c>true</c> if than mirror margins; otherwise, <c>false</c>.</value>
        internal bool MirrorMargins
        {
            get
            {
                return m_bMirrorMargins;
            }
            set
            {
                m_bMirrorMargins = value;
            }
        }
        /// <summary>
        /// Specifies whether to apply shading on form fields.
        /// </summary>
        /// <value><c>true</c> if form field shading is applied; otherwise, <c>false</c>.</value>
        internal bool FormFieldShading
        {
            get
            {
                return m_shadeFormData;
            }
            set
            {
                m_shadeFormData = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to track changes.
        /// </summary>
        /// <value><c>true</c> if track changes; otherwise, <c>false</c>.</value>
        internal bool TrackChanges
        {
            get
            {
                return m_trackChanges;
            }
            set
            {
                m_trackChanges = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal DOPDescriptor()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DOPDescriptor"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="dopStart">The DOPDescriptor start.</param>
        /// <param name="dopLength">Length of the dop.</param>
        /// <param name="isTemplate">if set to <c>true</c> document is template.</param>
        internal DOPDescriptor(Stream stream, int dopStart, int dopLength, bool isTemplate)
            : this()
        {
            stream.Position = dopStart;

            int temp = ReadUInt16(stream);
            m_bFacingPage = (temp & 1) != 0;
            m_bWidowControl = (temp & 2) != 0;
            m_bPMHMainDoc = (temp & 4) != 0;
            m_grfSuppression = (temp & 0x18) >> 3;
            m_fpc = (byte)((temp & 0x60) >> 5);
            m_unused0_7 = (temp & 0x80) >> 7;
            m_grpfIhdt = (temp & 0xff00) >> 8;
            temp = ReadUInt16(stream);

            m_rncFtn = (byte)(temp & 3);
            m_nFtn = (temp & 0xfffc) >> 2;

            m_bOutlineDirtySave = (byte)stream.ReadByte();
            //    fOnlyMacPics 	U8 	:1 	0100 	when 1, Word believes all pictures recorded in the document were created on a Macintosh
            //		fOnlyWinPics 	U8 	:1 	0200 	when 1, Word believes all pictures recorded in the document were created in Windows
            //		fLabelDoc 	  U8 	:1 	0400 	when 1, document was created as a print merge labels document
            //		fHyphCapitals U8 	:1 	0800 	when 1, Word is allowed to hyphenate words that are capitalized. When 0, capitalized may not be hyphenated
            //		fAutoHyphen 	U8 	:1 	1000 	when 1, Word will hyphenate newly typed text as a background task
            //		fFormNoFields U8 	:1 	2000 	
            //		fLinkStyles 	U8 	:1 	4000 	when 1, Word will merge styles from its template
            //		fRevMarking 	U8 	:1 	8000 	when 1, Word will mark revisions as the document is edited
            // Indent 5
            m_prot1 = (byte)stream.ReadByte();
            m_trackChanges = (m_prot1 & 0x80) != 0;

            // Indent 6,7
            int num2 = ReadUInt16(stream);
            m_bBackup = (num2 & 1) != 0;
            m_bExactCWords = (num2 & 2) != 0;
            m_bPagHidden = (num2 & 4) != 0;
            m_bPagResults = (num2 & 8) != 0;
            m_bLockAtn = (num2 & 0x10) != 0;
            m_bMirrorMargins = (num2 & 0x20) != 0;
            m_unused6_6 = (num2 & 0x40) != 0;
            m_bDfltTrueType = (num2 & 0x80) != 0;
            m_bPagSuppressTopSpacin = (num2 & 0x100) != 0;
            m_bProtEnabled = (num2 & 0x200) != 0;
            m_bDispFormFldSel = (num2 & 0x400) != 0;
            m_bRMView = (num2 & 0x800) != 0;
            m_bRMPrint = (num2 & 0x1000) != 0;
            m_unused7_5 = (num2 & 0x2000) != 0;
            m_bLockRev = (num2 & 0x4000) != 0;
            m_bEmbedFonts = (num2 & 0x8000) != 0;

            // 8, 9
            Copts60.Parse(stream);
            // 10, 11
            m_dxaTabs = ReadUInt16(stream);
            // 12, 13
            m_wSpare = ReadUInt16(stream);
            // 14, 15
            m_dxaHotZ = ReadUInt16(stream);
            // 16, 17
            m_cConsecHypLim = ReadUInt16(stream);
            // 18, 19
            m_wSpare2 = ReadUInt16(stream);
            // Read Create Date( 20-23 )
            m_dttmCreated = ReadUInt32(stream);
            if (isTemplate)
                m_created = DateTime.Now;
            else
                m_created = ParseDateTime(m_dttmCreated);

            // 24-27
            m_dttmRevised = ReadUInt32(stream);
            if (isTemplate)
                m_revised = DateTime.Now;
            else
                m_revised = ParseDateTime(m_dttmRevised);
            // 28-31
            m_dttmLastPrint = ReadUInt32(stream);
            if (isTemplate)
                m_lastPrinted = DateTime.Now;
            else
                m_lastPrinted = ParseDateTime(m_dttmLastPrint);
            // 32, 33
            m_nRevision = ReadInt16(stream);
            // 34-37
            m_tmEdited = ReadInt32(stream);
            // Updates total editing time.
            m_editTime = TimeSpan.FromMinutes(m_tmEdited);
            // 38-41
            m_cWords = ReadInt32(stream);
            // 42-45
            m_cCh = ReadInt32(stream);
            // 46, 47
            m_cPg = ReadInt16(stream);
            //48-51
            m_cParas = ReadInt32(stream);
            // 52,53
            m_End = ReadUInt16(stream);

            m_rncEdn = (byte)(m_End & 3);
            m_nEdn = (m_End & 0xfffc) >> 2;
            // 54, 55
            m_epc = ReadUInt16(stream);

            m_nfcFtnRef = (byte)((m_epc & 0x003C) >> 2);
            m_nfcEdnRef = (byte)((m_epc & 0x03C0) >> 6);

            m_bepc = (byte)(m_epc & 3);
            m_shadeFormData = (m_epc & 0x1000) == 0x1000;
            // 56-59
            m_cLines = ReadInt32(stream);
            // 60-63
            m_wordsFtnEnd = ReadInt32(stream);
            // 64-67
            m_cChFtnEdn = ReadInt32(stream);
            // 68-69
            m_cPgFtnEdn = ReadInt16(stream);
            // 70-73
            m_cParasFrnEdn = ReadInt32(stream);
            // 74-77
            m_cLinesFtnEdn = ReadInt32(stream);
            // 78-81
            m_lKeyProtDoc = ReadUInt32(stream);
            // 82, 83
            m_wvkSaved = ReadUInt16(stream);

            //Parse DOP95 records - 88 bytes (0x58)
            if (dopLength >= 0x58)
                Dop95.Parse(stream);
            //Parse DOP97 records - 500 bytes (0x1F4)
            if (dopLength >= 0x1F4)
                Dop97.Parse(stream);
            //Parse DOP2000 records - 544 bytes (0x220)
            if (dopLength >= 0x220)
                Dop2000.Parse(stream);
            //Parse DOP2002 records - 594 bytes (0x252)
            if (dopLength >= 0x252)
                Dop2002.Parse(stream);
            //Parse DOP2003 records - 616 bytes (0x268)
            if (dopLength >= 0x268)
                Dop2003.Parse(stream);
            else
            {
                if (m_bLockAtn)
                {
                    Dop2003.EnforceDocProt = true;
                    Dop2003.DocProtCur = 1;
                }
                else if (m_bProtEnabled)
                {
                    Dop2003.EnforceDocProt = true;
                    Dop2003.DocProtCur = 2;
                }
                else if (m_bLockRev)
                {
                    Dop2003.EnforceDocProt = true;
                    Dop2003.DocProtCur = 0;
                }
            }
            //Parse DOP2007 records - 674 bytes (0x2a2)
            if (dopLength > 0x2a2)
                Dop2007.Parse(stream);
            //Parse remaining Dop data.
            if (dopLength > ((int)stream.Position - dopStart))
            {
                int dopLeftBytes = dopLength - ((int)stream.Position - dopStart);
                m_dopLeftData = new byte[dopLeftBytes];
                stream.Read(m_dopLeftData, 0, dopLeftBytes);
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Update date time values with reference to built in document properties.
        /// </summary>
        /// <param name="builtInDocumnetProperties">Built in document properties</param>
        internal void UpdateDateTime(Syncfusion.DocIO.DLS.BuiltinDocumentProperties builtInDocumnetProperties)
        {
            m_created = new DateTime(builtInDocumnetProperties.CreateDate.Ticks);
            m_revised = new DateTime(builtInDocumnetProperties.LastSaveDate.Ticks);
            m_lastPrinted = new DateTime(builtInDocumnetProperties.LastPrinted.Ticks);
            m_editTime = builtInDocumnetProperties.TotalEditingTime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal int Write(Stream stream)
        {
            m_bWidowControl = true;

            long pos = stream.Position;
            int temp = 0;
            temp |= (m_bFacingPage ? 1 : 0);
            temp |= (m_bWidowControl ? 2 : 0);
            temp |= (m_bPMHMainDoc ? 4 : 0);
            temp |= (m_grfSuppression << 3);
            temp |= (m_fpc << 5);
            temp |= (m_unused0_7 << 7);
            temp |= (m_grpfIhdt << 8);
            WriteUInt16(stream, (ushort)temp);

            temp = 0;
            temp |= m_rncFtn;
            temp |= m_nFtn << 2;
            WriteUInt16(stream, (ushort)temp);

            stream.WriteByte(m_bOutlineDirtySave);

            if (m_trackChanges) m_prot1 |= 0x80;
            else m_prot1 = (byte)SetBitsByMask(m_prot1, 0x80, 0);
            stream.WriteByte(m_prot1);

            int num3 = 0;
            num3 |= (m_bBackup ? 1 : 0);
            num3 |= (m_bExactCWords ? 2 : 0);
            num3 |= (m_bPagHidden ? 4 : 0);
            num3 |= (m_bPagResults ? 8 : 0);
            num3 |= (m_bLockAtn ? 0x10 : 0);
            num3 |= (m_bMirrorMargins ? 0x20 : 0);
            num3 |= (m_unused6_6 ? 0x40 : 0);
            num3 |= (m_bDfltTrueType ? 0x80 : 0);
            num3 |= (m_bPagSuppressTopSpacin ? 0x100 : 0);
            num3 |= (m_bProtEnabled ? 0x200 : 0);
            num3 |= (m_bDispFormFldSel ? 0x400 : 0);
            num3 |= (m_bRMView ? 0x800 : 0);
            num3 |= (m_bRMPrint ? 0x1000 : 0);
            num3 |= (m_unused7_5 ? 0x2000 : 0);
            num3 |= (m_bLockRev ? 0x4000 : 0);
            num3 |= (m_bEmbedFonts ? 0x8000 : 0);
            WriteUInt16(stream, (ushort)num3);
            Copts60.Write(stream);
            WriteUInt16(stream, m_dxaTabs);
            WriteUInt16(stream, (ushort)m_wSpare);
            WriteUInt16(stream, (ushort)m_dxaHotZ);
            WriteUInt16(stream, (ushort)m_cConsecHypLim);
            WriteUInt16(stream, (ushort)m_wSpare2);

            WriteUInt32(stream, SetDateTime(m_created));
            WriteUInt32(stream, SetDateTime(m_revised));
            if (m_lastPrinted != DateTime.MinValue)
                WriteUInt32(stream, SetDateTime(m_lastPrinted));
            else
                WriteInt32(stream, 0);

            WriteInt16(stream, (short)m_nRevision);

            if (m_editTime != TimeSpan.MinValue)
                m_tmEdited = (int)m_editTime.TotalMinutes;
            WriteInt32(stream, m_tmEdited);
            WriteInt32(stream, m_cWords);
            WriteInt32(stream, m_cCh);
            WriteInt16(stream, (short)m_cPg);
            WriteInt32(stream, m_cParas);

            m_End = 0;
            m_End |= m_rncEdn;
            m_End |= (ushort)(m_nEdn << 2);

            WriteUInt16(stream, m_End);

            m_epc = (ushort)SetBitsByMask(m_epc, 0x003, m_bepc);
            m_epc = (ushort)SetBitsByMask(m_epc, 0x003C, m_nfcFtnRef);
            m_epc = (ushort)SetBitsByMask(m_epc, 0x03C0, m_nfcEdnRef);

            // Set form field shading property
            if (m_shadeFormData) m_epc |= 0x1000;
            else m_epc = (ushort)SetBitsByMask(m_epc, 0x1000, 0);

            WriteUInt16(stream, m_epc);
            WriteInt32(stream, m_cLines);
            WriteInt32(stream, m_wordsFtnEnd);
            WriteInt32(stream, m_cChFtnEdn);
            WriteInt16(stream, (short)m_cPgFtnEdn);
            WriteInt32(stream, m_cParasFrnEdn);
            WriteInt32(stream, m_cLinesFtnEdn);
            WriteUInt32(stream, m_lKeyProtDoc);
            WriteUInt16(stream, m_wvkSaved);

            //Writes DOP95 records - 88 bytes (0x58)
            Dop95.Write(stream);
            //Writes DOP97 records - 500 bytes (0x1F4)
            Dop97.Write(stream);
            //Writes DOP2000 records - 544 bytes (0x220)
            Dop2000.Write(stream);
            //Writes DOP2002 records - 594 bytes (0x252)
            Dop2002.Write(stream);
            //Writes DOP2003 records - 616 bytes (0x268)
            Dop2003.Write(stream);
            //Writes DOP2007 records - 674 bytes (0x2a2)
            Dop2007.Write(stream);

            if (m_dopLeftData != null)
            {
                stream.Write(m_dopLeftData, 0, m_dopLeftData.Length);
            }
            return (int)(stream.Position - pos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal DOPDescriptor Clone()
        {
            return (DOPDescriptor)base.MemberwiseClone();
        }
        /// <summary>
        /// Sets the document protection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="password">The password.</param>
        internal void SetProtection(ProtectionType type, string password)
        {
            ProtectionType = type;
            if (string.IsNullOrEmpty(password))
                m_lKeyProtDoc = DEF_PROTECTION_KEY;
            else if (type != ProtectionType.NoProtection)
                m_lKeyProtDoc = WordDecryptor.GetPasswordHash(password);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Parses the date time.
        /// Date and Time (internal date format) (DTTM)
        /// field | type | size | bitfield | comment 
        /// mint  | U16  |  :6  |  003F    | minutes (0-59) 
        /// hr    | U16  |  :5  |  07C0    | hours (0-23) 
        /// dom   | U16  |  :5  |  F800    | days of month (1-31) 
        /// mon   | U16  |  :4  |  000F    | months (1-12) 
        /// yr    | U16  |  :9  |  1FF0    | years (1900-2411)-1900 
        /// wdy   | U16  |  :3  |  E000    | weekday
        /// Weekday: Sunday=0, Monday=1, Tuesday=2, Wednesday=3, Thursday=4, Friday=5, Saturday=6 
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        private DateTime ParseDateTime(uint dateTime)
        {
            // If the value is not defined in the document level.
            if (dateTime == 0)
                return DateTime.MinValue;
            ushort daysHoursMin = (ushort)(dateTime & 0x0000ffff);
            ushort monYears = (ushort)((dateTime & 0xffff0000) >> 16);

            int minutes = (daysHoursMin & 0x003F);
            if (minutes < 0 || minutes > 59)
                return DateTime.Now;

            int hours = (daysHoursMin & 0x07c0) >> 6;
            if (hours < 0 || hours > 23)
                return DateTime.Now;

            int dom = (daysHoursMin & 0xf800) >> 11;
            if (dom < 1 || dom > 31)
                return DateTime.Now;

            int month = monYears & 0x000F;
            if (month < 1 || month > 12)
                return DateTime.Now;

            int year = (int)((monYears & 0x1FF0) >> 4) + 1900;
            if (year < 1900 || year > 2411)
                return DateTime.Now;

            DateTime dt = new DateTime(year, month, dom, hours, minutes, 0, 0);

            return dt;
        }
        /// <summary>
        /// Sets the date time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        private uint SetDateTime(DateTime dt)
        {
            uint dateTime = (uint)dt.Minute;
            dateTime |= (uint)dt.Hour << 6;
            dateTime |= (uint)dt.Day << 11;
            dateTime |= (uint)dt.Month << 16;
            dateTime |= (uint)(dt.Year - 1900) << 20;
            dateTime |= ConvertDayOfWeek(dt.DayOfWeek) << 29;

            return dateTime;
        }
        /// <summary>
        /// Converts the day of week to integer.
        /// </summary>
        /// <param name="dow">The DayOfWeek member.</param>
        /// <returns></returns>
        private uint ConvertDayOfWeek(DayOfWeek dow)
        {
            switch (dow)
            {
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                default:
                    return 0;
            }
        }
        #endregion

        #region Class Static Methods
        /// <summary>
        /// Returns hash value for the password string.
        /// </summary>
        /// <param name="password">Password to hash.</param>
        /// <returns>Hash value for the password string.</returns>
        [CLSCompliant(false)]
        internal static ushort GetPasswordHash(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (password.Length > DEF_MAX_PASSWORDLEN)
                throw new ArgumentOutOfRangeException("Length of the password can't be more than "
                  + DEF_MAX_PASSWORDLEN.ToString());

            ushort usHash = 0;

            for (int iCharIndex = 0, len = password.Length; iCharIndex < len; iCharIndex++)
            {
                bool[] bits = GetCharBits15(password[iCharIndex]);
                bits = RotateBits(bits, iCharIndex + 1);
                ushort curNumber = GetUInt16FromBits(bits);
                usHash ^= curNumber;
            }

            return (ushort)(usHash ^ password.Length ^ DEF_PASSWORD_CONST);
        }
        /// <summary>
        /// Converts character to 15 bits sequence
        /// </summary>
        /// <param name="charToConvert">Character to convert.</param>
        private static bool[] GetCharBits15(char charToConvert)
        {
            bool[] arrResult = new bool[15];
            ushort usSource = Convert.ToUInt16(charToConvert);
            ushort curBit = 1;

            for (int i = 0; i < 15; i++)
            {
                arrResult[i] = ((usSource & curBit) == curBit);
                curBit <<= 1;
            }

            return arrResult;
        }
        /// <summary>
        /// Converts bits array to UInt16 value.
        /// </summary>
        /// <param name="bits">Array to convert.</param>
        /// <returns>Converted UInt16 value.</returns>
        private static ushort GetUInt16FromBits(bool[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException("bits");

            if (bits.Length > 16)
                throw new ArgumentOutOfRangeException("There can't be more than 16 bits");

            bool[] arrResult = new bool[15];
            ushort usResult = 0;
            ushort curBit = 1;

            for (int i = 0, len = bits.Length; i < len; i++)
            {
                if (bits[i]) usResult += curBit;
                curBit <<= 1;
            }

            return usResult;
        }
        /// <summary>
        /// Rotates (cyclic shift) bits in the array specified number of times
        /// </summary>
        /// <param name="bits">Array to rotate</param>
        /// <param name="count">Number of times to rotate</param>
        /// <returns>Rotated array.</returns>
        private static bool[] RotateBits(bool[] bits, int count)
        {
            if (bits == null)
                throw new ArgumentNullException("bits");

            if (bits.Length == 0)
                return bits;

            if (count < 0)
                throw new ArgumentOutOfRangeException("count can't be less than zero");

            bool[] arrResult = new bool[bits.Length];

            for (int i = 0, len = bits.Length; i < len; i++)
            {
                int newPos = (i + count) % len;

                arrResult[newPos] = bits[i];
            }

            return arrResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        internal static int Round(int value, int degree)
        {
            if (degree == 0)
                throw new ArgumentOutOfRangeException("degree can't be 0");

            int mod = value % degree;

            return value - mod + degree;
        }

        #endregion
    }
    /// <summary>
    /// Specifies the structure of Dop95.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DOP95
    {
        #region Fields
        private Copts80 m_copts80;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the copts80.
        /// </summary>
        /// <value>The copts80.</value>
        internal Copts80 Copts80
        {
            get
            {
                if (m_copts80 == null)
                    m_copts80 = new Copts80(m_dopBase);
                return m_copts80;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DOP95"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal DOP95(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //copts80 (4 bytes): A copts80 specifying compatibility options. Copts80.copts60 components MUST be equal to DopBase.copts60.
            Copts80.Parse(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //copts80 (4 bytes): A copts80 specifying compatibility options. Copts80.copts60 components MUST be equal to DopBase.copts60.
            Copts80.Write(stream);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Dop97.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DOP97
    {
        #region Fields
        private ushort m_adt;
        private DopTypography m_doptypography;
        private Dogrid m_dogrid; 
        private ushort m_flagsA = 13426;
        private Asumyi m_asumyi;
        private uint m_cChWS;
        private uint m_cChWSWithSubdocs;
        private uint m_grfDocEvents;
        private uint m_flagsM;
        private uint m_cpMaxListCacheMainDoc;
        private ushort m_ilfoLastBulletMain;
        private ushort m_ilfoLastNumberMain;
        private uint m_cDBC;
        private uint m_cDBCWithSubdocs;
        private ushort m_nfcFtnRef;
        private ushort m_nfcEdnRef = 2;
        private ushort m_hpsZoomFontPag;
        private ushort m_dywDispPag;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the adt.
        /// </summary>
        /// <value>The adt.</value>
        internal ushort Adt
        {
            get
            {
                return m_adt;
            }
            set
            {
                m_adt = value;
            }
        }
        /// <summary>
        /// Gets the dop typography.
        /// </summary>
        /// <value>The dop typography.</value>
        internal DopTypography DopTypography
        {
            get
            {
                if (m_doptypography == null)
                    m_doptypography = new DopTypography();
                return m_doptypography;
            }
        }
        /// <summary>
        /// Gets the dogrid.
        /// </summary>
        /// <value>The dogrid.</value>
        internal Dogrid Dogrid
        {
            get
            {
                if (m_dogrid == null)
                    m_dogrid = new Dogrid();
                return m_dogrid;
            }
        }
        /// <summary>
        /// Gets or sets the LVL dop.
        /// </summary>
        /// <value>The LVL dop.</value>
        internal byte LvlDop
        {
            get
            {
                return (byte)((m_flagsA & 0x1E) >> 1);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFE1) | (value << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [gram all done].
        /// </summary>
        /// <value><c>true</c> if [gram all done]; otherwise, <c>false</c>.</value>
        internal bool GramAllDone
        {
            get
            {
                return ((m_flagsA & 0x20) >> 5) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFDF) | ((value? 1 : 0) << 5));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [gram all clean].
        /// </summary>
        /// <value><c>true</c> if [gram all clean]; otherwise, <c>false</c>.</value>
        internal bool GramAllClean
        {
            get
            {
                return ((m_flagsA & 0x40) >> 6) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [subset fonts].
        /// </summary>
        /// <value><c>true</c> if [subset fonts]; otherwise, <c>false</c>.</value>
        internal bool SubsetFonts
        {
            get
            {
                return ((m_flagsA & 0x80) >> 7) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFF7F) | ((value ? 1 : 0) << 7));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [HTML doc].
        /// </summary>
        /// <value><c>true</c> if [HTML doc]; otherwise, <c>false</c>.</value>
        internal bool HtmlDoc
        {
            get
            {
                return ((m_flagsA & 0x200) >> 9) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFDFF) | ((value ? 1 : 0) << 9));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [disk LVC invalid].
        /// </summary>
        /// <value><c>true</c> if [disk LVC invalid]; otherwise, <c>false</c>.</value>
        internal bool DiskLvcInvalid
        {
            get
            {
                return ((m_flagsA & 0x400) >> 10) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [snap border].
        /// </summary>
        /// <value><c>true</c> if [snap border]; otherwise, <c>false</c>.</value>
        internal bool SnapBorder
        {
            get
            {
                return ((m_flagsA & 0x800) >> 11) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xF7FF) | ((value ? 1 : 0) << 11));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [include header].
        /// </summary>
        /// <value><c>true</c> if [include header]; otherwise, <c>false</c>.</value>
        internal bool IncludeHeader
        {
            get
            {
                return ((m_flagsA & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [include footer].
        /// </summary>
        /// <value><c>true</c> if [include footer]; otherwise, <c>false</c>.</value>
        internal bool IncludeFooter
        {
            get
            {
                return ((m_flagsA & 0x2000) >> 13) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xDFFF) | ((value ? 1 : 0) << 13));
            }
        }
        /// <summary>
        /// Gets the asumyi.
        /// </summary>
        /// <value>The asumyi.</value>
        internal Asumyi Asumyi
        {
            get
            {
                if (m_asumyi == null)
                    m_asumyi = new Asumyi();
                return m_asumyi;
            }
        }
        /// <summary>
        /// Gets or sets the C ch WS.
        /// </summary>
        /// <value>The C ch WS.</value>
        internal uint CChWS
        {
            get
            {
                return m_cChWS;
            }
            set
            {
                m_cChWS = value;
            }
        }
        /// <summary>
        /// Gets or sets the C ch WS with subdocs.
        /// </summary>
        /// <value>The C ch WS with subdocs.</value>
        internal uint CChWSWithSubdocs
        {
            get
            {
                return m_cChWSWithSubdocs;
            }
            set
            {
                m_cChWSWithSubdocs = value;
            }
        }
        /// <summary>
        /// Gets or sets the GRF doc events.
        /// </summary>
        /// <value>The GRF doc events.</value>
        internal uint GrfDocEvents
        {
            get
            {
                return m_grfDocEvents;
            }
            set
            {
                m_grfDocEvents = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [virus prompted].
        /// </summary>
        /// <value><c>true</c> if [virus prompted]; otherwise, <c>false</c>.</value>
        internal bool VirusPrompted
        {
            get
            {
                return (m_flagsM & 0x1) != 0;
            }
            set
            {
                m_flagsM = (uint)((m_flagsM & 0xFFFFFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [virus load safe].
        /// </summary>
        /// <value><c>true</c> if [virus load safe]; otherwise, <c>false</c>.</value>
        internal bool VirusLoadSafe
        {
            get
            {
                return ((m_flagsM & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsM = (uint)((m_flagsM & 0xFFFFFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets the key virus session30.
        /// </summary>
        /// <value>The key virus session30.</value>
        internal uint KeyVirusSession30
        {
            get
            {
                return (uint)((m_flagsM & 0xFFFFFFFC) >> 2);
            }
            set
            {
                m_flagsM = (uint)((m_flagsM & 0x3) | (value << 2));
            }
        }
        /// <summary>
        /// Gets or sets the cp max list cache main doc.
        /// </summary>
        /// <value>The cp max list cache main doc.</value>
        internal uint CpMaxListCacheMainDoc
        {
            get
            {
                return m_cpMaxListCacheMainDoc;
            }
            set
            {
                m_cpMaxListCacheMainDoc = value;
            }
        }
        /// <summary>
        /// Gets or sets the ilfo last bullet main.
        /// </summary>
        /// <value>The ilfo last bullet main.</value>
        internal ushort IlfoLastBulletMain
        {
            get
            {
                return m_ilfoLastBulletMain;
            }
            set
            {
                m_ilfoLastBulletMain = value;
            }
        }
        /// <summary>
        /// Gets or sets the ilfo last number main.
        /// </summary>
        /// <value>The ilfo last number main.</value>
        internal ushort IlfoLastNumberMain
        {
            get
            {
                return m_ilfoLastNumberMain;
            }
            set
            {
                m_ilfoLastNumberMain = value;
            }
        }
        /// <summary>
        /// Gets or sets the CDBC.
        /// </summary>
        /// <value>The CDBC.</value>
        internal uint CDBC
        {
            get
            {
                return m_cDBC;
            }
            set
            {
                m_cDBC = value;
            }
        }
        /// <summary>
        /// Gets or sets the CDBC with subdocs.
        /// </summary>
        /// <value>The CDBC with subdocs.</value>
        internal uint CDBCWithSubdocs
        {
            get
            {
                return m_cDBCWithSubdocs;
            }
            set
            {
                m_cDBCWithSubdocs = value;
            }
        }
        /// <summary>
        /// Gets or sets the NFC FTN ref.
        /// </summary>
        /// <value>The NFC FTN ref.</value>
        internal ushort NfcFtnRef
        {
            get
            {
                return m_nfcFtnRef;
            }
            set
            {
                m_nfcFtnRef = value;
            }
        }
        /// <summary>
        /// Gets or sets the NFC edn ref.
        /// </summary>
        /// <value>The NFC edn ref.</value>
        internal ushort NfcEdnRef
        {
            get
            {
                return m_nfcEdnRef;
            }
            set
            {
                m_nfcEdnRef = value;
            }
        }
        /// <summary>
        /// Gets or sets the HPS zoom font pag.
        /// </summary>
        /// <value>The HPS zoom font pag.</value>
        internal ushort HpsZoomFontPag
        {
            get
            {
                return m_hpsZoomFontPag;
            }
            set
            {
                m_hpsZoomFontPag = value;
            }
        }
        /// <summary>
        /// Gets or sets the dyw disp pag.
        /// </summary>
        /// <value>The dyw disp pag.</value>
        internal ushort DywDispPag
        {
            get
            {
                return m_dywDispPag;
            }
            set
            {
                m_dywDispPag = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DOP97"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal DOP97(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //adt (2 bytes): Specifies the document classification as specified in [ECMA-376] Part 4, Section 2.15.1.29 documentType.
            m_adt = BaseWordRecord.ReadUInt16(stream);
            //doptypography (310 bytes): A DopTypography that specifies some typography settings.
            DopTypography.Parse(stream);
            //dogrid (10 bytes): A Dogrid that specifies the draw object grid settings.
            Dogrid.Parse(stream);
            //A - unused1 (1 bit): This bit is undefined and MUST be ignored.
            //lvlDop (4 bits): This value SHOULD<176> specify which outline levels were showing in outline view at the time of the last save operation. This MUST be a value between 0 and 9, inclusive, or this value MUST be 15. 
            //B - fGramAllDone (1 bit): Specifies whether the grammar of all content in this document was checked.
            //C - fGramAllClean (1 bit): Specifies whether all content in this document can be considered grammatically correct.
            //D - fSubsetFonts (1 bit): Specifies whether to subset fonts when embedding as specified in [ECMA-376] Part 4, Section 2.8.2.15 saveSubsetFonts, where embedTrueTypeFonts refers to DopBase.fEmbedFonts.
            //E - unused2 (1 bit): This value is undefined and MUST be ignored.
            //F - fHtmlDoc (1 bit): This value SHOULD<177> be 0.
            //G - fDiskLvcInvalid (1 bit): This bit MAY<178> specify whether the saved ListNum field cache contains valid information. The ListNum field cache is specified by FibRgFcLcb97.fcPlcfBteLvc.
            //H - fSnapBorder (1 bit): Specifies whether to align paragraph and table borders with the page border, as specified in [ECMA-376] Part 4, Section 2.15.1.2 alignBordersAndEdges.
            //I - fIncludeHeader (1 bit): Specifies whether to draw the page border so that it includes the header area.
            //J - fIncludeFooter (1 bit): Specifies whether to draw the page border so that it includes the footer area. 
            //K - unused3 (1 bit): This value is undefined and MUST be ignored.
            //L - unused4 (1 bit): This value is undefined and MUST be ignored.
            m_flagsA = BaseWordRecord.ReadUInt16(stream);
            //unused5 (2 bytes): This value is undefined and MUST be ignored.
            BaseWordRecord.ReadUInt16(stream);
            //asumyi (12 bytes): An Asumyi that specifies the AutoSummary settings.
            Asumyi.Parse(stream);
            //cChWS (4 bytes): Specifies the last calculated or estimated count of characters in the main document depending on the values of fExactCWords and fIncludeSubdocsInStats. The count of characters includes whitespace.
            m_cChWS = BaseWordRecord.ReadUInt32(stream);
            //cChWSWithSubdocs (4 bytes): Specifies the last calculated or estimated count of characters in the main document, footnotes, endnotes, and text boxes that are anchored in the main document, depending on fExactCWords and fIncludeSubdocsInStats. The count of characters includes whitespace.
            m_cChWSWithSubdocs = BaseWordRecord.ReadUInt32(stream);
            //grfDocEvents (4 bytes): A bit field that specifies which document events are fired. 
            m_grfDocEvents = BaseWordRecord.ReadUInt32(stream);
            //M - fVirusPrompted (1 bit): Specifies whether the macro security prompt is shown in this session for this document.
            //N - fVirusLoadSafe (1 bit): Specifies whether to disable macros for this session.
            //KeyVirusSession30 (30 bits): A random value to match against the current session key. If they match, this is the same session.
            m_flagsM = BaseWordRecord.ReadUInt32(stream);
            //space (30 bytes): This value is undefined and MUST be ignored.
            byte[] space = new byte[30];
            stream.Read(space, 0, space.Length);
            //cpMaxListCacheMainDoc (4 bytes): This value MAY<179> specify the maximum CP value for which the ListNum field cache contains valid information. The ListNum field cache is specified by FibRgFcLcb97.fcPlcfBteLvc.
            m_cpMaxListCacheMainDoc = BaseWordRecord.ReadUInt32(stream);
            //ilfoLastBulletMain (2 bytes): Specifies the index of the last LFO structure that was used for bullets in the document before the save operation. This value MUST be between 0 and a number that is one less than the number of entries in FibRgFcLcb97.fcPlfLfo, unless there are 0 entries, in which case this value MUST be 0.
            m_ilfoLastBulletMain = BaseWordRecord.ReadUInt16(stream);
            //ilfoLastNumberMain (2 bytes): Specifies the index of the last LFO structure that was used for list numbering in the document before the save operation. This value MUST be between 0 and a number that is one less than the number of entries in FibRgFcLcb97.fcPlfLfo, unless there are 0 entries, in which case this value MUST be 0. 
            m_ilfoLastNumberMain = BaseWordRecord.ReadUInt16(stream);
            //cDBC (4 bytes): Specifies the last calculated or estimated count of double-byte characters in the main document, depending on the values of DopBase.fExactCWords and DopBase.fIncludeSubdocsInStats. The count of characters includes whitespace.
            m_cDBC = BaseWordRecord.ReadUInt32(stream);
            //cDBCWithSubdocs (4 bytes): Specifies the last calculated or estimated count of double-byte characters in the main document, footnotes, endnotes, and text boxes anchored in the main document depending on DopBase.fExactCWords and DopBase.fIncludeSubdocsInStats. The character count includes whitespace.
            m_cDBCWithSubdocs = BaseWordRecord.ReadUInt32(stream);
            //reserved3a (4 bytes): This value is undefined and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //nfcFtnRef (2 bytes): An MSONFC (as specified in [MS-OSHARED] section 2.2.1.3) that, for those documents that have an nFib which is less than or equal to 0x00D9, specifies the numbering format code to use for footnotes in the document.
            m_nfcFtnRef = BaseWordRecord.ReadUInt16(stream);
            //nfcEdnRef (2 bytes): An MSONFC (as specified in [MS-OSHARED] section 2.2.1.3) that, for those documents that have an nFib which is less than or equal to 0x00D9, specifies the numbering format code to use for endnotes in the document.
            m_nfcEdnRef = BaseWordRecord.ReadUInt16(stream);
            //hpsZoomFontPag (2 bytes): Specifies the size, in half points, of the maximum font size to be enlarged in the view "online layout" at the time the document was last paginated. This value SHOULD<180> be ignored.
            m_hpsZoomFontPag = BaseWordRecord.ReadUInt16(stream);
            //dywDispPag (2 bytes): Height of the screen, in pixels, at the time that the document was last paginated. This value SHOULD<181> be ignored.
            m_dywDispPag = BaseWordRecord.ReadUInt16(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //adt (2 bytes): Specifies the document classification as specified in [ECMA-376] Part 4, Section 2.15.1.29 documentType.
            BaseWordRecord.WriteUInt16(stream, m_adt);
            //doptypography (310 bytes): A DopTypography that specifies some typography settings.
            DopTypography.Write(stream);
            //dogrid (10 bytes): A Dogrid that specifies the draw object grid settings.
            Dogrid.Write(stream);
            //A - unused1 (1 bit): This bit is undefined and MUST be ignored.
            //lvlDop (4 bits): This value SHOULD<176> specify which outline levels were showing in outline view at the time of the last save operation. This MUST be a value between 0 and 9, inclusive, or this value MUST be 15. 
            //B - fGramAllDone (1 bit): Specifies whether the grammar of all content in this document was checked.
            //C - fGramAllClean (1 bit): Specifies whether all content in this document can be considered grammatically correct.
            //D - fSubsetFonts (1 bit): Specifies whether to subset fonts when embedding as specified in [ECMA-376] Part 4, Section 2.8.2.15 saveSubsetFonts, where embedTrueTypeFonts refers to DopBase.fEmbedFonts.
            //E - unused2 (1 bit): This value is undefined and MUST be ignored.
            //F - fHtmlDoc (1 bit): This value SHOULD<177> be 0.
            //G - fDiskLvcInvalid (1 bit): This bit MAY<178> specify whether the saved ListNum field cache contains valid information. The ListNum field cache is specified by FibRgFcLcb97.fcPlcfBteLvc.
            //H - fSnapBorder (1 bit): Specifies whether to align paragraph and table borders with the page border, as specified in [ECMA-376] Part 4, Section 2.15.1.2 alignBordersAndEdges.
            //I - fIncludeHeader (1 bit): Specifies whether to draw the page border so that it includes the header area.
            //J - fIncludeFooter (1 bit): Specifies whether to draw the page border so that it includes the footer area. 
            //K - unused3 (1 bit): This value is undefined and MUST be ignored.
            //L - unused4 (1 bit): This value is undefined and MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, m_flagsA);
            //unused5 (2 bytes): This value is undefined and MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //asumyi (12 bytes): An Asumyi that specifies the AutoSummary settings.
            Asumyi.Write(stream);
            //cChWS (4 bytes): Specifies the last calculated or estimated count of characters in the main document depending on the values of fExactCWords and fIncludeSubdocsInStats. The count of characters includes whitespace.
            BaseWordRecord.WriteUInt32(stream, m_cChWS);
            //cChWSWithSubdocs (4 bytes): Specifies the last calculated or estimated count of characters in the main document, footnotes, endnotes, and text boxes that are anchored in the main document, depending on fExactCWords and fIncludeSubdocsInStats. The count of characters includes whitespace.
            BaseWordRecord.WriteUInt32(stream, m_cChWSWithSubdocs);
            //grfDocEvents (4 bytes): A bit field that specifies which document events are fired. 
            BaseWordRecord.WriteUInt32(stream, m_grfDocEvents);
            //M - fVirusPrompted (1 bit): Specifies whether the macro security prompt is shown in this session for this document.
            //N - fVirusLoadSafe (1 bit): Specifies whether to disable macros for this session.
            //KeyVirusSession30 (30 bits): A random value to match against the current session key. If they match, this is the same session.
            BaseWordRecord.WriteUInt32(stream, m_flagsM);
            //space (30 bytes): This value is undefined and MUST be ignored.
            byte[] space = new byte[30];
            stream.Write(space, 0, space.Length);
            //cpMaxListCacheMainDoc (4 bytes): This value MAY<179> specify the maximum CP value for which the ListNum field cache contains valid information. The ListNum field cache is specified by FibRgFcLcb97.fcPlcfBteLvc.
            BaseWordRecord.WriteUInt32(stream, m_cpMaxListCacheMainDoc);
            //ilfoLastBulletMain (2 bytes): Specifies the index of the last LFO structure that was used for bullets in the document before the save operation. This value MUST be between 0 and a number that is one less than the number of entries in FibRgFcLcb97.fcPlfLfo, unless there are 0 entries, in which case this value MUST be 0.
            BaseWordRecord.WriteUInt16(stream, m_ilfoLastBulletMain);
            //ilfoLastNumberMain (2 bytes): Specifies the index of the last LFO structure that was used for list numbering in the document before the save operation. This value MUST be between 0 and a number that is one less than the number of entries in FibRgFcLcb97.fcPlfLfo, unless there are 0 entries, in which case this value MUST be 0. 
            BaseWordRecord.WriteUInt16(stream, m_ilfoLastNumberMain);
            //cDBC (4 bytes): Specifies the last calculated or estimated count of double-byte characters in the main document, depending on the values of DopBase.fExactCWords and DopBase.fIncludeSubdocsInStats. The count of characters includes whitespace.
            BaseWordRecord.WriteUInt32(stream, m_cDBC);
            //cDBCWithSubdocs (4 bytes): Specifies the last calculated or estimated count of double-byte characters in the main document, footnotes, endnotes, and text boxes anchored in the main document depending on DopBase.fExactCWords and DopBase.fIncludeSubdocsInStats. The character count includes whitespace.
            BaseWordRecord.WriteUInt32(stream, m_cDBCWithSubdocs);
            //reserved3a (4 bytes): This value is undefined and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //nfcFtnRef (2 bytes): An MSONFC (as specified in [MS-OSHARED] section 2.2.1.3) that, for those documents that have an nFib which is less than or equal to 0x00D9, specifies the numbering format code to use for footnotes in the document.
            BaseWordRecord.WriteUInt16(stream, m_nfcFtnRef);
            //nfcEdnRef (2 bytes): An MSONFC (as specified in [MS-OSHARED] section 2.2.1.3) that, for those documents that have an nFib which is less than or equal to 0x00D9, specifies the numbering format code to use for endnotes in the document.
            BaseWordRecord.WriteUInt16(stream, m_nfcEdnRef);
            //hpsZoomFontPag (2 bytes): Specifies the size, in half points, of the maximum font size to be enlarged in the view "online layout" at the time the document was last paginated. This value SHOULD<180> be ignored.
            BaseWordRecord.WriteUInt16(stream, m_hpsZoomFontPag);
            //dywDispPag (2 bytes): Height of the screen, in pixels, at the time that the document was last paginated. This value SHOULD<181> be ignored.
            BaseWordRecord.WriteUInt16(stream, m_dywDispPag);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Dop2000.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DOP2000
    {
        #region Fields
        private byte m_ilvlLastBulletMain;
        private byte m_ilvlLastNumberMain;
        private ushort m_istdClickParaType;
        private ushort m_flagsA = 12800;
        private ushort m_flagsJ = 387;
        private Copts m_copts;
        private ushort m_verCompatPre10;
        private ushort m_flagsP = 4160;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ilvl last bullet main.
        /// </summary>
        /// <value>The ilvl last bullet main.</value>
        internal byte IlvlLastBulletMain
        {
            get
            {
                return m_ilvlLastBulletMain;
            }
            set
            {
                m_ilvlLastBulletMain = value;
            }
        }
        /// <summary>
        /// Gets or sets the ilvl last number main.
        /// </summary>
        /// <value>The ilvl last number main.</value>
        internal byte IlvlLastNumberMain
        {
            get
            {
                return m_ilvlLastNumberMain;
            }
            set
            {
                m_ilvlLastNumberMain = value;
            }
        }
        /// <summary>
        /// Gets or sets the type of the istd click para.
        /// </summary>
        /// <value>The type of the istd click para.</value>
        internal ushort IstdClickParaType
        {
            get
            {
                return m_istdClickParaType;
            }
            set
            {
                m_istdClickParaType = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [LAD all done].
        /// </summary>
        /// <value><c>true</c> if [LAD all done]; otherwise, <c>false</c>.</value>
        internal bool LADAllDone
        {
            get
            {
                return (m_flagsA & 0x1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [envelope vis].
        /// </summary>
        /// <value><c>true</c> if [envelope vis]; otherwise, <c>false</c>.</value>
        internal bool EnvelopeVis
        {
            get
            {
                return ((m_flagsA & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [maybe tentative list in doc].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [maybe tentative list in doc]; otherwise, <c>false</c>.
        /// </value>
        internal bool MaybeTentativeListInDoc
        {
            get
            {
                return ((m_flagsA & 0x4) >> 2) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFB) | ((value ? 1 : 0) << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [maybe fit text].
        /// </summary>
        /// <value><c>true</c> if [maybe fit text]; otherwise, <c>false</c>.</value>
        internal bool MaybeFitText
        {
            get
            {
                return ((m_flagsA & 0x8) >> 3) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [FCC all done].
        /// </summary>
        /// <value><c>true</c> if [FCC all done]; otherwise, <c>false</c>.</value>
        internal bool FCCAllDone
        {
            get
            {
                return ((m_flagsA & 0x100) >> 8) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFEFF) | ((value ? 1 : 0) << 8));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [rely on CS s_ web opt].
        /// </summary>
        /// <value><c>true</c> if [rely on CS s_ web opt]; otherwise, <c>false</c>.</value>
        internal bool RelyOnCSS_WebOpt
        {
            get
            {
                return ((m_flagsA & 0x200) >> 9) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFDFF) | ((value ? 1 : 0) << 9));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [rely on VM l_ web opt].
        /// </summary>
        /// <value><c>true</c> if [rely on VM l_ web opt]; otherwise, <c>false</c>.</value>
        internal bool RelyOnVML_WebOpt 
        {
            get
            {
                return ((m_flagsA & 0x400) >> 10) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [allow PN g_ web opt].
        /// </summary>
        /// <value><c>true</c> if [allow PN g_ web opt]; otherwise, <c>false</c>.</value>
        internal bool AllowPNG_WebOpt
        {
            get
            {
                return ((m_flagsA & 0x800) >> 11) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xF7FF) | ((value ? 1 : 0) << 11));
            }
        }
        /// <summary>
        /// Gets or sets the screen size_ web opt.
        /// </summary>
        /// <value>The screen size_ web opt.</value>
        internal byte ScreenSize_WebOpt
        {
            get
            {
                return (byte)((m_flagsA & 0xF000) >> 12);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFF) | (value << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [organize in folder_ web opt].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [organize in folder_ web opt]; otherwise, <c>false</c>.
        /// </value>
        internal bool OrganizeInFolder_WebOpt 
        {
            get
            {
                return (m_flagsJ & 0x1) != 0;
            }
            set
            {
                m_flagsJ = (ushort)((m_flagsJ & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [use long file names_ web opt].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use long file names_ web opt]; otherwise, <c>false</c>.
        /// </value>
        internal bool UseLongFileNames_WebOpt 
        {
            get
            {
                return ((m_flagsJ & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsJ = (ushort)((m_flagsJ & 0xFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets the pixels per inch_ web opt.
        /// </summary>
        /// <value>The pixels per inch_ web opt.</value>
        internal ushort PixelsPerInch_WebOpt
        {
            get
            {
                return (ushort)((m_flagsJ & 0xFFC) >> 2);
            }
            set
            {
                m_flagsJ = (ushort)((m_flagsJ & 0xF003) | (value << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [web options init].
        /// </summary>
        /// <value><c>true</c> if [web options init]; otherwise, <c>false</c>.</value>
        internal bool WebOptionsInit
        {
            get
            {
                return ((m_flagsJ & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flagsJ = (ushort)((m_flagsJ & 0xEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [maybe FEL].
        /// </summary>
        /// <value><c>true</c> if [maybe FEL]; otherwise, <c>false</c>.</value>
        internal bool MaybeFEL
        {
            get
            {
                return ((m_flagsJ & 0x1000) >> 13) != 0;
            }
            set
            {
                m_flagsJ = (ushort)((m_flagsJ & 0xEFFF) | ((value ? 1 : 0) << 13));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [char line units].
        /// </summary>
        /// <value><c>true</c> if [char line units]; otherwise, <c>false</c>.</value>
        internal bool CharLineUnits
        {
            get
            {
                return ((m_flagsJ & 0x1000) >> 14) != 0;
            }
            set
            {
                m_flagsJ = (ushort)((m_flagsJ & 0xEFFF) | ((value ? 1 : 0) << 14));
            }
        }
        /// <summary>
        /// Gets the copts.
        /// </summary>
        /// <value>The copts.</value>
        internal Copts Copts
        {
            get
            {
                if (m_copts == null)
                    m_copts = new Copts(m_dopBase);
                return m_copts;
            }
        }
        /// <summary>
        /// Gets or sets the ver compat pre10.
        /// </summary>
        /// <value>The ver compat pre10.</value>
        internal ushort VerCompatPre10
        {
            get
            {
                return m_verCompatPre10;
            }
            set
            {
                m_verCompatPre10 = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether not to display page boundaries (doNotDisplayPageBoundaries - Open xml format property).
        /// </summary>
        /// <value><c>true</c> if [no marg PGVW saved]; otherwise, <c>false</c>.</value>
        internal bool NoMargPgvwSaved
        {
            get
            {
                return (m_flagsP & 0x1) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [bullet proofed].
        /// </summary>
        /// <value><c>true</c> if [bullet proofed]; otherwise, <c>false</c>.</value>
        internal bool BulletProofed
        {
            get
            {
                return ((m_flagsP & 0x10) >> 4) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xFFEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [save uim].
        /// </summary>
        /// <value><c>true</c> if [save uim]; otherwise, <c>false</c>.</value>
        internal bool SaveUim
        {
            get
            {
                return ((m_flagsP & 0x40) >> 6) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xFFBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [filter privacy].
        /// </summary>
        /// <value><c>true</c> if [filter privacy]; otherwise, <c>false</c>.</value>
        internal bool FilterPrivacy
        {
            get
            {
                return ((m_flagsP & 0x80) >> 7) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xFF7F) | ((value ? 1 : 0) << 7));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [seen repairs].
        /// </summary>
        /// <value><c>true</c> if [seen repairs]; otherwise, <c>false</c>.</value>
        internal bool SeenRepairs
        {
            get
            {
                return ((m_flagsP & 0x200) >> 9) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xFDFF) | ((value ? 1 : 0) << 9));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has XML.
        /// </summary>
        /// <value><c>true</c> if this instance has XML; otherwise, <c>false</c>.</value>
        internal bool HasXML
        {
            get
            {
                return ((m_flagsP & 0x400) >> 10) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [validate XML].
        /// </summary>
        /// <value><c>true</c> if [validate XML]; otherwise, <c>false</c>.</value>
        internal bool ValidateXML
        {
            get
            {
                return ((m_flagsP & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [save invalid XML].
        /// </summary>
        /// <value><c>true</c> if [save invalid XML]; otherwise, <c>false</c>.</value>
        internal bool SaveInvalidXML
        {
            get
            {
                return ((m_flagsP & 0x2000) >> 13) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xDFFF) | ((value ? 1 : 0) << 13));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show XML errors].
        /// </summary>
        /// <value><c>true</c> if [show XML errors]; otherwise, <c>false</c>.</value>
        internal bool ShowXMLErrors
        {
            get
            {
                return ((m_flagsP & 0x4000) >> 14) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0xBFFF) | ((value ? 1 : 0) << 14));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [always merge empty namespace].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [always merge empty namespace]; otherwise, <c>false</c>.
        /// </value>
        internal bool AlwaysMergeEmptyNamespace
        {
            get
            {
                return ((m_flagsP & 0x8000) >> 15) != 0;
            }
            set
            {
                m_flagsP = (ushort)((m_flagsP & 0x7FFF) | ((value ? 1 : 0) << 15));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DOP2000"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal DOP2000(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //ilvlLastBulletMain (1 byte): SHOULD<182> specify the last bullet level applied via the toolbar before saving. MUST be between 0 and 9. Default is 0.
            m_ilvlLastBulletMain = (byte)stream.ReadByte();
            //ilvlLastNumberMain (1 byte): SHOULD<183> specify the last list numbering level applied via the toolbar before saving. MUST be between 0 and 9. Default is 0.
            m_ilvlLastNumberMain = (byte)stream.ReadByte();
            //istdClickParaType (2 bytes): Specifies the ISTD of the paragraph style to use for paragraphs that are automatically created by the click and type feature to place the cursor where the user clicked. Default value is 0 (Normal paragraph style).
            m_istdClickParaType = BaseWordRecord.ReadUInt16(stream);
            //A - fLADAllDone (1 bit): Specifies whether language auto-detection has run to completion for the document. Default is 0.
            //B - fEnvelopeVis (1 bit): Specifies whether to show the E-Mail message header as specified in [ECMA-376] Part 4, Section 2.15.1.80 showEnvelope. Default is 0.
            //C - fMaybeTentativeListInDoc (1 bit): Specifies whether the document potentially contains tentative lists<184>. Default is 0. See LVLF.fTentative.
            //D - fMaybeFitText (1 bit): If this is 0, then there MUST NOT be any fit text (see sprmCFitText) in the document. Default is 0.
            //empty1 (4 bits): MUST be zero, and MUST be ignored.
            //E - fFCCAllDone (1 bit): Specifies whether the format consistency checker has run to completion for the document. Default is 0.
            //F - fRelyOnCSS_WebOpt (1 bit): Specifies whether to rely on CSS for font face formatting when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.11 doNotRelyOnCSS, where the meaning is the opposite of fRelyOnCSS_WebOpt. The default is 1.
            //G - fRelyOnVML_WebOpt (1 bit): Specifies whether to use VML when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.34 relyOnVML. The default is 0.
            //H - fAllowPNG_WebOpt (1 bit): Specifies whether to allow Portable Network Graphics (PNG) format as a graphic format when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.1 allowPNG. Default value is 0.
            //I - screenSize_WebOpt (4 bits): Specifies what the target screen size for the Web page is as specified in [ECMA-376] Part 4, Section 2.15.2.41 targetScreenSz, where screenSize_WebOpt value maps to ST_TargetScreenSz types
            m_flagsA = BaseWordRecord.ReadUInt16(stream); 
            //J - fOrganizeInFolder_WebOpt (1 bit): Specifies whether to place supporting files in a subdirectory when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.10 doNotOrganizeInFolder, where the meaning is the opposite of fOrganizeInFolder_WebOpt. The default is 1.
            //K - fUseLongFileNames_WebOpt (1 bit): Specifies whether to use file names longer than 8.3 characters when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.13 doNotUseLongFileNames, where the meaning is the opposite of fUseLongFileNames_WebOpt. The default is 1.
            //iPixelsPerInch_WebOpt (10 bits): Specifies the pixels per inch for graphics/images when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.33 pixelsPerInch. If fWebOptionsInit is 1 then this MUST be between 19 and 480; otherwise, this is ignored. The default is 96.
            //L - fWebOptionsInit (1 bit): Specifies whether fRelyOnCSS_WebOpt, fRelyOnVML_WebOpt, fAllowPNG_WebOpt, screenSize_WebOpt, fOrganizeInFolder_WebOpt, fUseLongFileNames_WebOpt and iPixelsPerInch_WebOpt contain valid data. When fWebOptionsInit is set to 0, the value of all those fields MUST be ignored. The default is 0.
            //M - fMaybeFEL (1 bit): If this is 0, then there MUST NOT be any Warichu, Tatenakayoko, Ruby, Kumimoji or EncloseText in the document. The default is 0.
            //N - fCharLineUnits (1 bit): If this is 0, then there MUST NOT be any character unit indents (sprmPDxcLeft, sprmPDxcLeft1, sprmPDxcRight) or line units (sprmPDylBefore, sprmPDylAfter) in use. The default is 0.
            //O - unused1 (1 bit): Undefined and MUST be ignored.
            m_flagsJ = BaseWordRecord.ReadUInt16(stream);
            //copts (32 bytes): A copts that specifies compatibility options. Components of Copts.copts80 MUST be equal to components of Dop97.copts80.
            Copts.Parse(stream);
            //verCompatPre10 (16 bits): A bit field that specifies the desired feature set to use for the document. This overrides DopBase.fWord97Compat. 
            m_verCompatPre10 = BaseWordRecord.ReadUInt16(stream);
            //P - fNoMargPgvwSaved (1 bit): Specifies whether to suppress the display of the header and footer area when in print layout view so that the main text area of one page is displayed adjacent to the main text area of the next page as specified in [ECMA-376] Part 4, Section 2.15.1.34 doNotDisplayPageBoundaries. Default is 0. 
            //Q - unused2 (1 bit): Undefined and MUST be ignored.
            //R - unused3 (1 bit): Undefined and MUST be ignored.
            //S - unused4 (1 bit): Undefined and MUST be ignored.
            //T - fBulletProofed (1 bit): Specifies that this document was produced by the Open and Repair feature. Default is 0.
            //U - empty2 (1 bit): MUST be zero, and MUST be ignored.
            //V - fSaveUim (1 bit): Specifies whether to save UIM data in the document. Default is 1.
            //W - fFilterPrivacy (1 bit): Specifies whether to remove personal information from the document properties on save as specified in [ECMA-376] Part 4, Section 2.15.1.68 removePersonalInformation. Default is 0.
            //X - empty3 (1 bit): MUST be zero, and MUST be ignored.
            //Y - fSeenRepairs (1 bit): Specifies whether the user has seen any repairs made by the Open and Repair feature. Default is 0.
            //Z - fHasXML (1 bit): Specifies whether the document has any form of structured document tags in it. Default is 0.
            //a - unused5 (1 bit): Undefined and MUST be ignored.
            //b - fValidateXML (1 bit): Specifies whether to validate custom XML markup against any attached schemas as specified in [ECMA-376] Part 4, Section 2.15.1.42 doNotValidateAgainstSchema, where the meaning is the opposite of fValidateXML. Default is 1
            //c - fSaveInvalidXML (1 bit): Specifies whether to allow saving the document as an XML file when the custom XML markup is invalid with respect to the attached schemas as specified in [ECMA-376] Part 4, Section 2.15.1.74 saveInvalidXml. Default is 0.
            //d - fShowXMLErrors (1 bit): Specifies whether to show a visual indicator for invalid custom XML markup as specified in [ECMA-376] Part 4, Section 2.15.1.33 doNotDemarcateInvalidXml, where the meaning is the opposite of fShowXMLErrors.
            //e - fAlwaysMergeEmptyNamespace (1 bit): Specifies whether to consider custom XML elements with no namespace as valid on open as specified in [ECMA-376] Part 4, Section 2.15.1.3 alwaysMergeEmptyNamespace. Default is 0.
            m_flagsP = BaseWordRecord.ReadUInt16(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //ilvlLastBulletMain (1 byte): SHOULD<182> specify the last bullet level applied via the toolbar before saving. MUST be between 0 and 9. Default is 0.
            stream.WriteByte(m_ilvlLastBulletMain);
            //ilvlLastNumberMain (1 byte): SHOULD<183> specify the last list numbering level applied via the toolbar before saving. MUST be between 0 and 9. Default is 0.
            stream.WriteByte(m_ilvlLastNumberMain);
            //istdClickParaType (2 bytes): Specifies the ISTD of the paragraph style to use for paragraphs that are automatically created by the click and type feature to place the cursor where the user clicked. Default value is 0 (Normal paragraph style).
            BaseWordRecord.WriteUInt16(stream, m_istdClickParaType);
            //A - fLADAllDone (1 bit): Specifies whether language auto-detection has run to completion for the document. Default is 0.
            //B - fEnvelopeVis (1 bit): Specifies whether to show the E-Mail message header as specified in [ECMA-376] Part 4, Section 2.15.1.80 showEnvelope. Default is 0.
            //C - fMaybeTentativeListInDoc (1 bit): Specifies whether the document potentially contains tentative lists<184>. Default is 0. See LVLF.fTentative.
            //D - fMaybeFitText (1 bit): If this is 0, then there MUST NOT be any fit text (see sprmCFitText) in the document. Default is 0.
            //empty1 (4 bits): MUST be zero, and MUST be ignored.
            //E - fFCCAllDone (1 bit): Specifies whether the format consistency checker has run to completion for the document. Default is 0.
            //F - fRelyOnCSS_WebOpt (1 bit): Specifies whether to rely on CSS for font face formatting when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.11 doNotRelyOnCSS, where the meaning is the opposite of fRelyOnCSS_WebOpt. The default is 1.
            //G - fRelyOnVML_WebOpt (1 bit): Specifies whether to use VML when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.34 relyOnVML. The default is 0.
            //H - fAllowPNG_WebOpt (1 bit): Specifies whether to allow Portable Network Graphics (PNG) format as a graphic format when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.1 allowPNG. Default value is 0.
            //I - screenSize_WebOpt (4 bits): Specifies what the target screen size for the Web page is as specified in [ECMA-376] Part 4, Section 2.15.2.41 targetScreenSz, where screenSize_WebOpt value maps to ST_TargetScreenSz types
            BaseWordRecord.WriteUInt16(stream, m_flagsA);
            //J - fOrganizeInFolder_WebOpt (1 bit): Specifies whether to place supporting files in a subdirectory when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.10 doNotOrganizeInFolder, where the meaning is the opposite of fOrganizeInFolder_WebOpt. The default is 1.
            //K - fUseLongFileNames_WebOpt (1 bit): Specifies whether to use file names longer than 8.3 characters when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.13 doNotUseLongFileNames, where the meaning is the opposite of fUseLongFileNames_WebOpt. The default is 1.
            //iPixelsPerInch_WebOpt (10 bits): Specifies the pixels per inch for graphics/images when saving as a Web page as specified in [ECMA-376] Part 4, Section 2.15.2.33 pixelsPerInch. If fWebOptionsInit is 1 then this MUST be between 19 and 480; otherwise, this is ignored. The default is 96.
            //L - fWebOptionsInit (1 bit): Specifies whether fRelyOnCSS_WebOpt, fRelyOnVML_WebOpt, fAllowPNG_WebOpt, screenSize_WebOpt, fOrganizeInFolder_WebOpt, fUseLongFileNames_WebOpt and iPixelsPerInch_WebOpt contain valid data. When fWebOptionsInit is set to 0, the value of all those fields MUST be ignored. The default is 0.
            //M - fMaybeFEL (1 bit): If this is 0, then there MUST NOT be any Warichu, Tatenakayoko, Ruby, Kumimoji or EncloseText in the document. The default is 0.
            //N - fCharLineUnits (1 bit): If this is 0, then there MUST NOT be any character unit indents (sprmPDxcLeft, sprmPDxcLeft1, sprmPDxcRight) or line units (sprmPDylBefore, sprmPDylAfter) in use. The default is 0.
            //O - unused1 (1 bit): Undefined and MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, m_flagsJ);
            //copts (32 bytes): A copts that specifies compatibility options. Components of Copts.copts80 MUST be equal to components of Dop97.copts80.
            Copts.Write(stream);
            //verCompatPre10 (16 bits): A bit field that specifies the desired feature set to use for the document. This overrides DopBase.fWord97Compat. 
            BaseWordRecord.WriteUInt16(stream, m_verCompatPre10);
            //P - fNoMargPgvwSaved (1 bit): Specifies whether to suppress the display of the header and footer area when in print layout view so that the main text area of one page is displayed adjacent to the main text area of the next page as specified in [ECMA-376] Part 4, Section 2.15.1.34 doNotDisplayPageBoundaries. Default is 0. 
            //Q - unused2 (1 bit): Undefined and MUST be ignored.
            //R - unused3 (1 bit): Undefined and MUST be ignored.
            //S - unused4 (1 bit): Undefined and MUST be ignored.
            //T - fBulletProofed (1 bit): Specifies that this document was produced by the Open and Repair feature. Default is 0.
            //U - empty2 (1 bit): MUST be zero, and MUST be ignored.
            //V - fSaveUim (1 bit): Specifies whether to save UIM data in the document. Default is 1.
            //W - fFilterPrivacy (1 bit): Specifies whether to remove personal information from the document properties on save as specified in [ECMA-376] Part 4, Section 2.15.1.68 removePersonalInformation. Default is 0.
            //X - empty3 (1 bit): MUST be zero, and MUST be ignored.
            //Y - fSeenRepairs (1 bit): Specifies whether the user has seen any repairs made by the Open and Repair feature. Default is 0.
            //Z - fHasXML (1 bit): Specifies whether the document has any form of structured document tags in it. Default is 0.
            //a - unused5 (1 bit): Undefined and MUST be ignored.
            //b - fValidateXML (1 bit): Specifies whether to validate custom XML markup against any attached schemas as specified in [ECMA-376] Part 4, Section 2.15.1.42 doNotValidateAgainstSchema, where the meaning is the opposite of fValidateXML. Default is 1
            //c - fSaveInvalidXML (1 bit): Specifies whether to allow saving the document as an XML file when the custom XML markup is invalid with respect to the attached schemas as specified in [ECMA-376] Part 4, Section 2.15.1.74 saveInvalidXml. Default is 0.
            //d - fShowXMLErrors (1 bit): Specifies whether to show a visual indicator for invalid custom XML markup as specified in [ECMA-376] Part 4, Section 2.15.1.33 doNotDemarcateInvalidXml, where the meaning is the opposite of fShowXMLErrors.
            //e - fAlwaysMergeEmptyNamespace (1 bit): Specifies whether to consider custom XML elements with no namespace as valid on open as specified in [ECMA-376] Part 4, Section 2.15.1.3 alwaysMergeEmptyNamespace. Default is 0.
            BaseWordRecord.WriteUInt16(stream, m_flagsP);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Dop2002.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DOP2002
    {
        #region Fields
        private ushort m_flagsA = 61449;
        private ushort m_istdTableDflt = 4095;
        private ushort m_verCompat;
        private ushort m_grfFmtFilter = 0x5024;
        private ushort m_iFolioPages;
        private int m_cpgText = 1252;
        private uint m_cpMinRMText;
        private uint m_cpMinRMFtn;
        private uint m_cpMinRMHdd;
        private uint m_cpMinRMAtn;
        private uint m_cpMinRMEdn;
        private uint m_cpMinRmTxbx;
        private uint m_cpMinRmHdrTxbx;
        private uint m_rsidRoot;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [do not embed system font].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [do not embed system font]; otherwise, <c>false</c>.
        /// </value>
        internal bool DoNotEmbedSystemFont
        {
            get
            {
                return (m_flagsA & 0x1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [word compat].
        /// </summary>
        /// <value><c>true</c> if [word compat]; otherwise, <c>false</c>.</value>
        internal bool WordCompat
        {
            get
            {
                return ((m_flagsA & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [live recover].
        /// </summary>
        /// <value><c>true</c> if [live recover]; otherwise, <c>false</c>.</value>
        internal bool LiveRecover
        {
            get
            {
                return ((m_flagsA & 0x4) >> 2) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFB) | ((value ? 1 : 0) << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [embed factoids].
        /// </summary>
        /// <value><c>true</c> if [embed factoids]; otherwise, <c>false</c>.</value>
        internal bool EmbedFactoids
        {
            get
            {
                return ((m_flagsA & 0x8) >> 3) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [factoid XML].
        /// </summary>
        /// <value><c>true</c> if [factoid XML]; otherwise, <c>false</c>.</value>
        internal bool FactoidXML
        {
            get
            {
                return ((m_flagsA & 0x10) >> 4) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [factoid all done].
        /// </summary>
        /// <value><c>true</c> if [factoid all done]; otherwise, <c>false</c>.</value>
        internal bool FactoidAllDone
        {
            get
            {
                return ((m_flagsA & 0x20) >> 5) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFDF) | ((value ? 1 : 0) << 5));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [folio print].
        /// </summary>
        /// <value><c>true</c> if [folio print]; otherwise, <c>false</c>.</value>
        internal bool FolioPrint
        {
            get
            {
                return ((m_flagsA & 0x40) >> 6) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [reverse folio].
        /// </summary>
        /// <value><c>true</c> if [reverse folio]; otherwise, <c>false</c>.</value>
        internal bool ReverseFolio
        {
            get
            {
                return ((m_flagsA & 0x80) >> 7) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFF7F) | ((value ? 1 : 0) << 7));
            }
        }
        /// <summary>
        /// Gets or sets the text line ending.
        /// </summary>
        /// <value>The text line ending.</value>
        internal byte TextLineEnding
        {
            get
            {
                return (byte)((m_flagsA & 0x700) >> 8);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xF8FF) | (value << 8));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [hide FCC].
        /// </summary>
        /// <value><c>true</c> if [hide FCC]; otherwise, <c>false</c>.</value>
        internal bool HideFcc
        {
            get
            {
                return ((m_flagsA & 0x800) >> 11) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xF7FF) | ((value ? 1 : 0) << 11));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [acetate show markup].
        /// </summary>
        /// <value><c>true</c> if [acetate show markup]; otherwise, <c>false</c>.</value>
        internal bool AcetateShowMarkup
        {
            get
            {
                return ((m_flagsA & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [acetate show atn].
        /// </summary>
        /// <value><c>true</c> if [acetate show atn]; otherwise, <c>false</c>.</value>
        internal bool AcetateShowAtn
        {
            get
            {
                return ((m_flagsA & 0x2000) >> 13) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xDFFF) | ((value ? 1 : 0) << 13));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [acetate show ins del].
        /// </summary>
        /// <value><c>true</c> if [acetate show ins del]; otherwise, <c>false</c>.</value>
        internal bool AcetateShowInsDel
        {
            get
            {
                return ((m_flagsA & 0x4000) >> 14) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xBFFF) | ((value ? 1 : 0) << 14));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [acetate show props].
        /// </summary>
        /// <value><c>true</c> if [acetate show props]; otherwise, <c>false</c>.</value>
        internal bool AcetateShowProps
        {
            get
            {
                return ((m_flagsA & 0x8000) >> 15) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0x7FFF) | ((value ? 1 : 0) << 15));
            }
        }
        /// <summary>
        /// Gets or sets the istd table DFLT.
        /// </summary>
        /// <value>The istd table DFLT.</value>
        internal ushort IstdTableDflt
        {
            get
            {
                return m_istdTableDflt;
            }
            set
            {
                m_istdTableDflt = value;
            }
        }
        /// <summary>
        /// Gets or sets the ver compat.
        /// </summary>
        /// <value>The ver compat.</value>
        internal ushort VerCompat
        {
            get
            {
                return m_verCompat;
            }
            set
            {
                m_verCompat = value;
            }
        }
        /// <summary>
        /// Gets or sets the GRF FMT filter.
        /// </summary>
        /// <value>The GRF FMT filter.</value>
        internal ushort GrfFmtFilter
        {
            get
            {
                return m_grfFmtFilter;
            }
            set
            {
                m_grfFmtFilter = value;
            }
        }
        /// <summary>
        /// Gets or sets the I folio pages.
        /// </summary>
        /// <value>The I folio pages.</value>
        internal ushort IFolioPages
        {
            get
            {
                return m_iFolioPages;
            }
            set
            {
                m_iFolioPages = value;
            }
        }
        /// <summary>
        /// Gets or sets the CPG text.
        /// </summary>
        /// <value>The CPG text.</value>
        internal int CpgText
        {
            get
            {
                return m_cpgText;
            }
            set
            {
                m_cpgText = value;
            }
        }
        /// <summary>
        /// Gets or sets the cp min RM text.
        /// </summary>
        /// <value>The cp min RM text.</value>
        internal uint CpMinRMText
        {
            get
            {
                return m_cpMinRMText;
            }
            set
            {
                m_cpMinRMText = value;
            }
        }
        /// <summary>
        /// Gets or sets the cp min RM FTN.
        /// </summary>
        /// <value>The cp min RM FTN.</value>
        internal uint CpMinRMFtn
        {
            get
            {
                return m_cpMinRMFtn;
            }
            set
            {
                m_cpMinRMFtn = value;
            }
        }
        /// <summary>
        /// Gets or sets the cp min RM HDD.
        /// </summary>
        /// <value>The cp min RM HDD.</value>
        internal uint CpMinRMHdd
        {
            get
            {
                return m_cpMinRMHdd;
            }
            set
            {
                m_cpMinRMHdd = value;
            }
        }
        /// <summary>
        /// Gets or sets the cp min RM atn.
        /// </summary>
        /// <value>The cp min RM atn.</value>
        internal uint CpMinRMAtn
        {
            get
            {
                return m_cpMinRMAtn;
            }
            set
            {
                m_cpMinRMAtn = value;
            }
        }
        /// <summary>
        /// Gets or sets the cp min RM edn.
        /// </summary>
        /// <value>The cp min RM edn.</value>
        internal uint CpMinRMEdn
        {
            get
            {
                return m_cpMinRMEdn;
            }
            set
            {
                m_cpMinRMEdn = value;
            }
        }
        /// <summary>
        /// Gets or sets the cp min rm TXBX.
        /// </summary>
        /// <value>The cp min rm TXBX.</value>
        internal uint CpMinRmTxbx
        {
            get
            {
                return m_cpMinRmTxbx;
            }
            set
            {
                m_cpMinRmTxbx = value;
            }
        }
        /// <summary>
        /// Gets or sets the cp min rm HDR TXBX.
        /// </summary>
        /// <value>The cp min rm HDR TXBX.</value>
        internal uint CpMinRmHdrTxbx
        {
            get
            {
                return m_cpMinRmHdrTxbx;
            }
            set
            {
                m_cpMinRmHdrTxbx = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DOP2002"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal DOP2002(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //unused (4 bytes): Undefined and MUST be ignored.
            BaseWordRecord.ReadInt32(stream);
            //A - fDoNotEmbedSystemFont (1 bit): Specifies whether common system fonts are not to be embedded as specified in [ECMA-376] Part 4, Section 2.8.2.7 embedSystemFonts, where the meaning is the opposite of fDoNotEmbedSystemFont and the embedTrueTypeFonts element refers to DopBase.fEmbedFonts. Default is 1.
            //B - fWordCompat (1 bit): Specifies that features not compatible with the settings specified in verCompat will be disabled or removed when saving. Default is 0.
            //C - fLiveRecover (1 bit): Specifies that this file is a recovered document from after a crash. Default is 0.
            //D - fEmbedFactoids (1 bit): Specifies whether smart tags are to remain in the document when saving. Smart tags are to be removed when fEmbedFactoids is set to 0. See [ECMA-376] Part 4, Section 2.15.1.35 doNotEmbedSmartTags, where the meaning is the opposite of fEmbedFactoids. Default is 1.
            //E - fFactoidXML (1 bit): Specifies whether to save smart tag data as an XML-based property bag at the head of the HTML page when saving as HTML as specified in [ECMA-376] Part 4, Section 2.15.2.36 saveSmartTagsAsXml. Default is 0.
            //F - fFactoidAllDone (1 bit): Specifies whether the document has been completely scanned for all possible smart tag creations. Default is 0.
            //G - fFolioPrint (1 bit): Specifies whether to use book fold printing as specified in [ECMA-376] Part 4, Section 2.15.1 11 bookFoldPrinting. Default is 0.
            //H - fReverseFolio (1 bit): Specifies whether to use reverse book fold printing as specified in [ECMA-376] Part 4, Section 2.15.1.13 bookFoldRevPrinting. If this is 1 then fFolioPrint MUST be 1. Default is 0.
            //I - iTextLineEnding (3 bits): Specifies what to end a line of text with when saving as a text file via automation. 
            //J - fHideFcc (1 bit): Specifies whether to refrain from showing a visual cue around ranges flagged by the format consistency checker as suspect. Default is 0.
            //K - fAcetateShowMarkup (1 bit): Specifies whether to visually indicate any additional nonprinting area used to display annotations when the annotations in this document are displayed. Default is 1.
            //L - fAcetateShowAtn (1 bit): Specifies if comments are included when the contents of this document are displayed. Default is 1.
            //M - fAcetateShowInsDel (1 bit): Specifies if revisions to content are included when the contents of this document are displayed. Default is 1.
            //N - fAcetateShowProps (1 bit): Specifies whether property revision marks are included when the contents of this document are displayed. Default is 1.
            m_flagsA = BaseWordRecord.ReadUInt16(stream);
            //istdTableDflt (16 bits): An istd that specifies the default table style for newly inserted tables.
            m_istdTableDflt = BaseWordRecord.ReadUInt16(stream);
            //verCompat (16 bits): A bit field that specifies the desired feature set to use for the document. This overrides DopBase.fWord97Compat and Dop2000.verCompatPre10. 
            m_verCompat = BaseWordRecord.ReadUInt16(stream);
            //grfFmtFilter (2 bytes): Specifies the suggested filtering for the list of document styles as specified in [ECMA-376] Part 4, Section 2.15.1.86 stylePaneFormatFilter. Default is 0x5024.
            m_grfFmtFilter = BaseWordRecord.ReadUInt16(stream);
            //iFolioPages (2 bytes): Specifies the number of pages per booklet as specified in [ECMA-376] Part 4, Section 2.15.1.12 bookFoldPrintingSheets, where bookFoldPrinting refers to fFolioPrint and bookFoldRevPrinting refers to fReverseFolio. Default is 0.
            m_iFolioPages = BaseWordRecord.ReadUInt16(stream);
            //cpgText (4 bytes): Specifies the code page to use when saving as encoded text. Default is the current Windows ANSI code page for the system.
            m_cpgText = BaseWordRecord.ReadInt32(stream);
            //cpMinRMText (4 bytes): A CP in the main document before which there are no revisions. Default is 0.
            m_cpMinRMText = BaseWordRecord.ReadUInt32(stream);
            //cpMinRMFtn (4 bytes): A CP in the footnote document before which there are no revisions. Default is 0.
            m_cpMinRMFtn = BaseWordRecord.ReadUInt32(stream);
            //cpMinRMHdd (4 bytes): A CP in the header document before which there are no revisions. Default is 0.
            m_cpMinRMHdd = BaseWordRecord.ReadUInt32(stream);
            //cpMinRMAtn (4 bytes): A CP in the comment document before which there are no revisions. Default is 0.
            m_cpMinRMAtn = BaseWordRecord.ReadUInt32(stream);
            //cpMinRMEdn (4 bytes): A CP in the endnote document before which there are no revisions. Default is 0.
            m_cpMinRMEdn = BaseWordRecord.ReadUInt32(stream);
            //cpMinRmTxbx (4 bytes): A CP in the textbox document for the main document before which there are no revisions. Default is 0.
            m_cpMinRmTxbx = BaseWordRecord.ReadUInt32(stream);
            //cpMinRmHdrTxbx (4 bytes): A CP in the header textbox document before which there are no revisions. Default is 0.
            m_cpMinRmHdrTxbx = BaseWordRecord.ReadUInt32(stream);
            //rsidRoot (4 bytes): Specifies the original document revision save ID as specified in [ECMA-376] Part 4, Section 2.15.1.71 rsidRoot. By default the rsidRoot is not that of the currently running session.
            m_rsidRoot = BaseWordRecord.ReadUInt32(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //unused (4 bytes): Undefined and MUST be ignored.
            BaseWordRecord.WriteInt32(stream, 0);
            //A - fDoNotEmbedSystemFont (1 bit): Specifies whether common system fonts are not to be embedded as specified in [ECMA-376] Part 4, Section 2.8.2.7 embedSystemFonts, where the meaning is the opposite of fDoNotEmbedSystemFont and the embedTrueTypeFonts element refers to DopBase.fEmbedFonts. Default is 1.
            //B - fWordCompat (1 bit): Specifies that features not compatible with the settings specified in verCompat will be disabled or removed when saving. Default is 0.
            //C - fLiveRecover (1 bit): Specifies that this file is a recovered document from after a crash. Default is 0.
            //D - fEmbedFactoids (1 bit): Specifies whether smart tags are to remain in the document when saving. Smart tags are to be removed when fEmbedFactoids is set to 0. See [ECMA-376] Part 4, Section 2.15.1.35 doNotEmbedSmartTags, where the meaning is the opposite of fEmbedFactoids. Default is 1.
            //E - fFactoidXML (1 bit): Specifies whether to save smart tag data as an XML-based property bag at the head of the HTML page when saving as HTML as specified in [ECMA-376] Part 4, Section 2.15.2.36 saveSmartTagsAsXml. Default is 0.
            //F - fFactoidAllDone (1 bit): Specifies whether the document has been completely scanned for all possible smart tag creations. Default is 0.
            //G - fFolioPrint (1 bit): Specifies whether to use book fold printing as specified in [ECMA-376] Part 4, Section 2.15.1 11 bookFoldPrinting. Default is 0.
            //H - fReverseFolio (1 bit): Specifies whether to use reverse book fold printing as specified in [ECMA-376] Part 4, Section 2.15.1.13 bookFoldRevPrinting. If this is 1 then fFolioPrint MUST be 1. Default is 0.
            //I - iTextLineEnding (3 bits): Specifies what to end a line of text with when saving as a text file via automation. 
            //J - fHideFcc (1 bit): Specifies whether to refrain from showing a visual cue around ranges flagged by the format consistency checker as suspect. Default is 0.
            //K - fAcetateShowMarkup (1 bit): Specifies whether to visually indicate any additional nonprinting area used to display annotations when the annotations in this document are displayed. Default is 1.
            //L - fAcetateShowAtn (1 bit): Specifies if comments are included when the contents of this document are displayed. Default is 1.
            //M - fAcetateShowInsDel (1 bit): Specifies if revisions to content are included when the contents of this document are displayed. Default is 1.
            //N - fAcetateShowProps (1 bit): Specifies whether property revision marks are included when the contents of this document are displayed. Default is 1.
            BaseWordRecord.WriteUInt16(stream, m_flagsA);
            //istdTableDflt (16 bits): An istd that specifies the default table style for newly inserted tables.
            BaseWordRecord.WriteUInt16(stream, m_istdTableDflt);
            //verCompat (16 bits): A bit field that specifies the desired feature set to use for the document. This overrides DopBase.fWord97Compat and Dop2000.verCompatPre10. 
            BaseWordRecord.WriteUInt16(stream, m_verCompat);
            //grfFmtFilter (2 bytes): Specifies the suggested filtering for the list of document styles as specified in [ECMA-376] Part 4, Section 2.15.1.86 stylePaneFormatFilter. Default is 0x5024.
            BaseWordRecord.WriteUInt16(stream, m_grfFmtFilter);
            //iFolioPages (2 bytes): Specifies the number of pages per booklet as specified in [ECMA-376] Part 4, Section 2.15.1.12 bookFoldPrintingSheets, where bookFoldPrinting refers to fFolioPrint and bookFoldRevPrinting refers to fReverseFolio. Default is 0.
            BaseWordRecord.WriteUInt16(stream, m_iFolioPages);
            //cpgText (4 bytes): Specifies the code page to use when saving as encoded text. Default is the current Windows ANSI code page for the system.
            BaseWordRecord.WriteInt32(stream, m_cpgText);
            //cpMinRMText (4 bytes): A CP in the main document before which there are no revisions. Default is 0.
            BaseWordRecord.WriteUInt32(stream, m_cpMinRMText);
            //cpMinRMFtn (4 bytes): A CP in the footnote document before which there are no revisions. Default is 0.
            BaseWordRecord.WriteUInt32(stream, m_cpMinRMFtn);
            //cpMinRMHdd (4 bytes): A CP in the header document before which there are no revisions. Default is 0.
            BaseWordRecord.WriteUInt32(stream, m_cpMinRMHdd);
            //cpMinRMAtn (4 bytes): A CP in the comment document before which there are no revisions. Default is 0.
            BaseWordRecord.WriteUInt32(stream, m_cpMinRMAtn);
            //cpMinRMEdn (4 bytes): A CP in the endnote document before which there are no revisions. Default is 0.
            BaseWordRecord.WriteUInt32(stream, m_cpMinRMEdn);
            //cpMinRmTxbx (4 bytes): A CP in the textbox document for the main document before which there are no revisions. Default is 0.
            BaseWordRecord.WriteUInt32(stream, m_cpMinRmTxbx);
            //cpMinRmHdrTxbx (4 bytes): A CP in the header textbox document before which there are no revisions. Default is 0.
            BaseWordRecord.WriteUInt32(stream, m_cpMinRmHdrTxbx);
            //rsidRoot (4 bytes): Specifies the original document revision save ID as specified in [ECMA-376] Part 4, Section 2.15.1.71 rsidRoot. By default the rsidRoot is not that of the currently running session.
            BaseWordRecord.WriteUInt32(stream, m_rsidRoot);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Dop2003.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DOP2003
    {
        #region Fields
        private ushort m_flagsA;
        private byte m_flagsO = 50;
        private uint m_dxaPageLock;
        private uint m_dyaPageLock;
        private uint m_pctFontLock;
        private byte m_grfitbid;
        private ushort m_ilfoMacAtCleanup;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [treat lock atn as read only].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [treat lock atn as read only]; otherwise, <c>false</c>.
        /// </value>
        internal bool TreatLockAtnAsReadOnly
        {
            get
            {
                return (m_flagsA & 0x1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [style lock].
        /// </summary>
        /// <value><c>true</c> if [style lock]; otherwise, <c>false</c>.</value>
        internal bool StyleLock
        {
            get
            {
                return ((m_flagsA & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [auto FMT override].
        /// </summary>
        /// <value><c>true</c> if [auto FMT override]; otherwise, <c>false</c>.</value>
        internal bool AutoFmtOverride
        {
            get
            {
                return ((m_flagsA & 0x4) >> 2) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFB) | ((value ? 1 : 0) << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [remove word ML].
        /// </summary>
        /// <value><c>true</c> if [remove word ML]; otherwise, <c>false</c>.</value>
        internal bool RemoveWordML
        {
            get
            {
                return ((m_flagsA & 0x8) >> 3) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [apply custom X form].
        /// </summary>
        /// <value><c>true</c> if [apply custom X form]; otherwise, <c>false</c>.</value>
        internal bool ApplyCustomXForm
        {
            get
            {
                return ((m_flagsA & 0x10) >> 4) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [style lock enforced].
        /// </summary>
        /// <value><c>true</c> if [style lock enforced]; otherwise, <c>false</c>.</value>
        internal bool StyleLockEnforced
        {
            get
            {
                return ((m_flagsA & 0x20) >> 5) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFDF) | ((value ? 1 : 0) << 5));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [fake lock atn].
        /// </summary>
        /// <value><c>true</c> if [fake lock atn]; otherwise, <c>false</c>.</value>
        internal bool FakeLockAtn
        {
            get
            {
                return ((m_flagsA & 0x40) >> 6) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [ignore mixed content].
        /// </summary>
        /// <value><c>true</c> if [ignore mixed content]; otherwise, <c>false</c>.</value>
        internal bool IgnoreMixedContent
        {
            get
            {
                return ((m_flagsA & 0x80) >> 7) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFF7F) | ((value ? 1 : 0) << 7));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show placeholder text].
        /// </summary>
        /// <value><c>true</c> if [show placeholder text]; otherwise, <c>false</c>.</value>
        internal bool ShowPlaceholderText
        {
            get
            {
                return ((m_flagsA & 0x100) >> 8) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFEFF) | ((value ? 1 : 0) << 8));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [word97 doc].
        /// </summary>
        /// <value><c>true</c> if [word97 doc]; otherwise, <c>false</c>.</value>
        internal bool Word97Doc
        {
            get
            {
                return ((m_flagsA & 0x400) >> 10) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [style lock theme].
        /// </summary>
        /// <value><c>true</c> if [style lock theme]; otherwise, <c>false</c>.</value>
        internal bool StyleLockTheme
        {
            get
            {
                return ((m_flagsA & 0x800) >> 11) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xF7FF) | ((value ? 1 : 0) << 11));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [style lock QF set].
        /// </summary>
        /// <value><c>true</c> if [style lock QF set]; otherwise, <c>false</c>.</value>
        internal bool StyleLockQFSet
        {
            get
            {
                return ((m_flagsA & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [reading mode ink lock down].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [reading mode ink lock down]; otherwise, <c>false</c>.
        /// </value>
        internal bool ReadingModeInkLockDown
        {
            get
            {
                return (m_flagsO & 0x1) != 0;
            }
            set
            {
                m_flagsO = (byte)((m_flagsO & 0xFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [acetate show ink atn].
        /// </summary>
        /// <value><c>true</c> if [acetate show ink atn]; otherwise, <c>false</c>.</value>
        internal bool AcetateShowInkAtn
        {
            get
            {
                return ((m_flagsO & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsO = (byte)((m_flagsO & 0xFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [filter DTTM].
        /// </summary>
        /// <value><c>true</c> if [filter DTTM]; otherwise, <c>false</c>.</value>
        internal bool FilterDttm
        {
            get
            {
                return ((m_flagsO & 0x4) >> 2) != 0;
            }
            set
            {
                m_flagsO = (byte)((m_flagsO & 0xFB) | ((value ? 1 : 0) << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enforce doc prot].
        /// </summary>
        /// <value><c>true</c> if [enforce doc prot]; otherwise, <c>false</c>.</value>
        internal bool EnforceDocProt
        {
            get
            {
                return ((m_flagsO & 0x8) >> 3) != 0;
            }
            set
            {
                m_flagsO = (byte)((m_flagsO & 0xF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// Gets or sets the doc prot cur.
        /// </summary>
        /// <value>The doc prot cur.</value>
        internal byte DocProtCur
        {
            get
            {
                return (byte)((m_flagsO & 0x70) >> 4);
            }
            set
            {
                m_flagsO = (byte)((m_flagsO & 0x8F) | (value << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to display background objects (displayBackgroundShape - Open xml property).
        /// </summary>
        /// <value><c>true</c> if display background objects; otherwise, <c>false</c>.</value>
        internal bool DispBkSpSaved
        {
            get
            {
                return ((m_flagsO & 0x80) >> 7) != 0;
            }
            set
            {
                m_flagsO = (byte)((m_flagsO & 0x7F) | ((value ? 1 : 0) << 7));
            }
        }
        /// <summary>
        /// Gets or sets the dxa page lock.
        /// </summary>
        /// <value>The dxa page lock.</value>
        internal uint DxaPageLock
        {
            get
            {
                return m_dxaPageLock;
            }
            set
            {
                m_dxaPageLock = value;
            }
        }
        /// <summary>
        /// Gets or sets the dya page lock.
        /// </summary>
        /// <value>The dya page lock.</value>
        internal uint DyaPageLock
        {
            get
            {
                return m_dyaPageLock;
            }
            set
            {
                m_dyaPageLock = value;
            }
        }
        /// <summary>
        /// Gets or sets the PCT font lock.
        /// </summary>
        /// <value>The PCT font lock.</value>
        internal uint PctFontLock
        {
            get
            {
                return m_pctFontLock;
            }
            set
            {
                m_pctFontLock = value;
            }
        }
        /// <summary>
        /// Gets or sets the grfitbid.
        /// </summary>
        /// <value>The grfitbid.</value>
        internal byte Grfitbid
        {
            get
            {
                return m_grfitbid;
            }
            set
            {
                m_grfitbid = value;
            }
        }
        /// <summary>
        /// Gets or sets the ilfo mac at cleanup.
        /// </summary>
        /// <value>The ilfo mac at cleanup.</value>
        internal ushort IlfoMacAtCleanup
        {
            get
            {
                return m_ilfoMacAtCleanup;
            }
            set
            {
                m_ilfoMacAtCleanup = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DOP2003"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal DOP2003(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //A - fTreatLockAtnAsReadOnly (1 bit): Specifies whether DopBase.fLockAtn means read-only protection instead of protect for comments. By default, this value is 0.
            //B - fStyleLock (1 bit): Specifies whether the styles available to use in the document are restricted to those styles with STD.Stdf.StdfBase.GRFSTD.fLocked set to 1 when style lock is enforced (fStyleLockEnforced is 1). By default, this value is 0.
            //C - fAutoFmtOverride (1 bit): Specifies whether to allow automatic formatting to override the fStyleLock setting as specified in [ECMA-376] Part 4, Section 2.15.1.9 autoFormatOverride. By default, this value is 0.
            //D - fRemoveWordML (1 bit): Specifies whether to save only custom XML markup when saving to XML as specified in [ECMA-376] Part 4, Section 2.15.1.77 saveXmlDataOnly. By default, this value is 0.
            //E - fApplyCustomXForm (1 bit): Specifies whether to save the document through the custom XML transform specified via FibRgFcLcb2003.fcCustomXForm and FibRgFcLcb2003.lcbCustomXForm when saving to XML as specified in [ECMA-376] Part 4, Section 2.15.1.92 useXSLTWhenSaving. By default, this value is 0.
            //F - fStyleLockEnforced (1 bit): Specifies whether to actively enforce the style restriction as specified by fStyleLock. If fStyleLockEnforced is 1, fStyleLock MUST be 1. By default, this value is 0.
            //G - fFakeLockAtn (1 bit): Specifies that the DopBase.fLockAtn setting is to be honored only if the application does not support fStyleLock. By default, this value is 0.
            //H - fIgnoreMixedContent (1 bit): Specifies whether to ignore all text not in leaf nodes of the custom XML when validating custom XML markup as specified in [ECMA-376] Part 4, Section 2.15.1.54 ignoreMixedContent. By default, this value is 0.
            //I - fShowPlaceholderText (1 bit): Specifies whether to show some form of in-document placeholder text when custom XML markup contains no content and the custom XML tags are not being displayed as specified in [ECMA-376] Part 4, Section 2.15.1.4 alwaysShowPlaceholderText. By default, this value is 0.
            //J - unused (1 bit): This value is undefined and MUST be ignored.
            //K - fWord97Doc (1 bit): Specifies whether to disable UI for features incompatible with the Word Binary File Format as specified in [ECMA-376] Part 4, Section 2.15.3.54 uiCompat97To2003. By default, this value is 0.
            //L - fStyleLockTheme (1 bit): Specifies whether to prevent modification of the document theme information as specified in [ECMA-376] Part 4, Section 2.15.1.85 styleLockTheme. By default, this value is 0.
            //M - fStyleLockQFSet (1 bit): Specifies whether to prevent the replacement of style sets as specified in [ECMA-376] Part 4, Section 2.15.1.84 styleLockQFSet. By default, this value is 0.
            //N - empty1 (19 bits): This value MUST be zero, and MUST be ignored.
            m_flagsA = BaseWordRecord.ReadUInt16(stream);
            //N - empty1 (19 bits) Reads remaining 16 bits.
            BaseWordRecord.ReadUInt16(stream);
            //O - fReadingModeInkLockDown (1 bit): Specifies whether to permanently set the layout to the specific set of page and text-sizing parameters specified by dxaPageLock, dyaPageLock and pctFontLock as specified in [ECMA-376] Part 4, Section 2.15.1.66 readModeInkLockDown. By default, this value is 0.
            //P - fAcetateShowInkAtn (1 bit): Specifies whether to include ink annotations when the contents of this document are displayed. By default, this value is 1.
            //Q - fFilterDttm (1 bit): Specifies whether to remove date and time information from annotations as specified in [ECMA-376] Part 4, Section 2.15.1.67 removeDateAndTime. By default, this value is 0.
            //R - fEnforceDocProt (1 bit): Specifies whether to enforce the document protection mode that is specified by iDocProtCur. By default, this value is 0.
            //S - iDocProtCur (3 bits): Specifies the document protection mode that is in effect when fEnforceDocProt is set to 1.
            //T - fDispBkSpSaved (1 bit): Specifies whether to display background objects when displaying the document in print layout view as specified in [ECMA-376] Part 4, Section 2.15.1.25 displayBackgroundShape. By default, this value is 0.
            m_flagsO = (byte)stream.ReadByte();
            //empty2 (8 bits): This value MUST be zero, and MUST be ignored.
            stream.ReadByte();
            //dxaPageLock (4 bytes): Specifies the width, in twips, of the virtual pages that are used in this document when fReadingModeInkLockDown is 1. By default, this value is 0.
            m_dxaPageLock = BaseWordRecord.ReadUInt32(stream);
            //dyaPageLock (4 bytes): Specifies the height, in twips, of the virtual pages that are used in this document when fReadingModeInkLockDown is 1. By default, this value is 0.
            m_dyaPageLock = BaseWordRecord.ReadUInt32(stream);
            //pctFontLock (4 bytes): Specifies the percentage to which text in the document is scaled before it is displayed on a virtual page when fReadingModeInkLockDown is 1. By default, this value is 0.
            m_pctFontLock = BaseWordRecord.ReadUInt32(stream);
            //grfitbid (1 byte): A bit field that specifies what toolbars were shown because of document state rather than explicit user action at the moment of saving. 
            m_grfitbid = (byte)stream.ReadByte();
            //empty3 (1 byte): This value MUST be zero, and MUST be ignored.
            stream.ReadByte();
            //ilfoMacAtCleanup (2 bytes): Specifies the largest ilfo value (index into PlfLfo) such that all PlfLfo entries from 0 to ilfoMacAtCleanup are searched for unused values to be pruned as specified in [ECMA-376] Part 4, Section 2.9.20 numIdMacAtCleanup. By default, this value is 0.
            m_ilfoMacAtCleanup = BaseWordRecord.ReadUInt16(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //A - fTreatLockAtnAsReadOnly (1 bit): Specifies whether DopBase.fLockAtn means read-only protection instead of protect for comments. By default, this value is 0.
            //B - fStyleLock (1 bit): Specifies whether the styles available to use in the document are restricted to those styles with STD.Stdf.StdfBase.GRFSTD.fLocked set to 1 when style lock is enforced (fStyleLockEnforced is 1). By default, this value is 0.
            //C - fAutoFmtOverride (1 bit): Specifies whether to allow automatic formatting to override the fStyleLock setting as specified in [ECMA-376] Part 4, Section 2.15.1.9 autoFormatOverride. By default, this value is 0.
            //D - fRemoveWordML (1 bit): Specifies whether to save only custom XML markup when saving to XML as specified in [ECMA-376] Part 4, Section 2.15.1.77 saveXmlDataOnly. By default, this value is 0.
            //E - fApplyCustomXForm (1 bit): Specifies whether to save the document through the custom XML transform specified via FibRgFcLcb2003.fcCustomXForm and FibRgFcLcb2003.lcbCustomXForm when saving to XML as specified in [ECMA-376] Part 4, Section 2.15.1.92 useXSLTWhenSaving. By default, this value is 0.
            //F - fStyleLockEnforced (1 bit): Specifies whether to actively enforce the style restriction as specified by fStyleLock. If fStyleLockEnforced is 1, fStyleLock MUST be 1. By default, this value is 0.
            //G - fFakeLockAtn (1 bit): Specifies that the DopBase.fLockAtn setting is to be honored only if the application does not support fStyleLock. By default, this value is 0.
            //H - fIgnoreMixedContent (1 bit): Specifies whether to ignore all text not in leaf nodes of the custom XML when validating custom XML markup as specified in [ECMA-376] Part 4, Section 2.15.1.54 ignoreMixedContent. By default, this value is 0.
            //I - fShowPlaceholderText (1 bit): Specifies whether to show some form of in-document placeholder text when custom XML markup contains no content and the custom XML tags are not being displayed as specified in [ECMA-376] Part 4, Section 2.15.1.4 alwaysShowPlaceholderText. By default, this value is 0.
            //J - unused (1 bit): This value is undefined and MUST be ignored.
            //K - fWord97Doc (1 bit): Specifies whether to disable UI for features incompatible with the Word Binary File Format as specified in [ECMA-376] Part 4, Section 2.15.3.54 uiCompat97To2003. By default, this value is 0.
            //L - fStyleLockTheme (1 bit): Specifies whether to prevent modification of the document theme information as specified in [ECMA-376] Part 4, Section 2.15.1.85 styleLockTheme. By default, this value is 0.
            //M - fStyleLockQFSet (1 bit): Specifies whether to prevent the replacement of style sets as specified in [ECMA-376] Part 4, Section 2.15.1.84 styleLockQFSet. By default, this value is 0.
            //N - empty1 (19 bits): This value MUST be zero, and MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, m_flagsA);
            //N - empty1 (19 bits) Reads remaining 16 bits.
            BaseWordRecord.WriteUInt16(stream, 0);
            //O - fReadingModeInkLockDown (1 bit): Specifies whether to permanently set the layout to the specific set of page and text-sizing parameters specified by dxaPageLock, dyaPageLock and pctFontLock as specified in [ECMA-376] Part 4, Section 2.15.1.66 readModeInkLockDown. By default, this value is 0.
            //P - fAcetateShowInkAtn (1 bit): Specifies whether to include ink annotations when the contents of this document are displayed. By default, this value is 1.
            //Q - fFilterDttm (1 bit): Specifies whether to remove date and time information from annotations as specified in [ECMA-376] Part 4, Section 2.15.1.67 removeDateAndTime. By default, this value is 0.
            //R - fEnforceDocProt (1 bit): Specifies whether to enforce the document protection mode that is specified by iDocProtCur. By default, this value is 0.
            //S - iDocProtCur (3 bits): Specifies the document protection mode that is in effect when fEnforceDocProt is set to 1.
            //T - fDispBkSpSaved (1 bit): Specifies whether to display background objects when displaying the document in print layout view as specified in [ECMA-376] Part 4, Section 2.15.1.25 displayBackgroundShape. By default, this value is 0.
            stream.WriteByte(m_flagsO);
            //empty2 (8 bits): This value MUST be zero, and MUST be ignored.
            stream.WriteByte(0);
            //dxaPageLock (4 bytes): Specifies the width, in twips, of the virtual pages that are used in this document when fReadingModeInkLockDown is 1. By default, this value is 0.
            BaseWordRecord.WriteUInt32(stream, m_dxaPageLock);
            //dyaPageLock (4 bytes): Specifies the height, in twips, of the virtual pages that are used in this document when fReadingModeInkLockDown is 1. By default, this value is 0.
            BaseWordRecord.WriteUInt32(stream, m_dyaPageLock);
            //pctFontLock (4 bytes): Specifies the percentage to which text in the document is scaled before it is displayed on a virtual page when fReadingModeInkLockDown is 1. By default, this value is 0.
            BaseWordRecord.WriteUInt32(stream, m_pctFontLock);
            //grfitbid (1 byte): A bit field that specifies what toolbars were shown because of document state rather than explicit user action at the moment of saving. 
            stream.WriteByte(m_grfitbid);
            //empty3 (1 byte): This value MUST be zero, and MUST be ignored.
            stream.WriteByte(0);
            //ilfoMacAtCleanup (2 bytes): Specifies the largest ilfo value (index into PlfLfo) such that all PlfLfo entries from 0 to ilfoMacAtCleanup are searched for unused values to be pruned as specified in [ECMA-376] Part 4, Section 2.9.20 numIdMacAtCleanup. By default, this value is 0.
            BaseWordRecord.WriteUInt16(stream, m_ilfoMacAtCleanup);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Dop2007.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DOP2007
    {
        #region Fields
        private ushort m_flagsA = 1059;
        private DopMth m_dopMath;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [RM track formatting].
        /// </summary>
        /// <value><c>true</c> if [RM track formatting]; otherwise, <c>false</c>.</value>
        internal bool RMTrackFormatting
        {
            get
            {
                return (m_flagsA & 0x1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [RM track moves].
        /// </summary>
        /// <value><c>true</c> if [RM track moves]; otherwise, <c>false</c>.</value>
        internal bool RMTrackMoves
        {
            get
            {
                return ((m_flagsA & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets the SSM.
        /// </summary>
        /// <value>The SSM.</value>
        internal byte Ssm
        {
            get
            {
                return (byte)((m_flagsA & 0x1E0) >> 5);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFE1F) | (value << 5));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [reading mode ink lock down actual page].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [reading mode ink lock down actual page]; otherwise, <c>false</c>.
        /// </value>
        internal bool ReadingModeInkLockDownActualPage
        {
            get
            {
                return ((m_flagsA & 0x200) >> 9) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFDFF) | ((value ? 1 : 0) << 9));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [auto compress pictures].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [auto compress pictures]; otherwise, <c>false</c>.
        /// </value>
        internal bool AutoCompressPictures
        {
            get
            {
                return ((m_flagsA & 0x400) >> 10) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets the dop math.
        /// </summary>
        /// <value>The dop math.</value>
        internal DopMth DopMath
        {
            get
            {
                if (m_dopMath == null)
                    m_dopMath = new DopMth();
                return m_dopMath;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DOP2007"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal DOP2007(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //reserved1 (4 bytes): This value is undefined, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //A - fRMTrackFormatting (1 bit): Specifies whether to track format changes when tracking for revisions (DopBase.fRevMarking). By default, this value is 1.
            //B - fRMTrackMoves (1 bit): Specifies whether to track moved text when tracking for revisions (DopBase.fRevMarking) instead of tracking for the deletions and insertions that are made. By default, this value is 1.
            //C - reserved2 (1 bit): This value MUST be 0, and MUST be ignored.
            //D - empty1 (1 bit): This value MUST be 0, and MUST be ignored.
            //E - empty2 (1 bit): This value MUST be 0, and MUST be ignored.
            //ssm (4 bits): An unsigned integer that specifies the sorting method to use when displaying document styles. 
            //F - fReadingModeInkLockDownActualPage (1 bit): Specifies whether to render the document with actual pages or virtual pages as specified in [ECMA-376] Part 4, Section 2.15.1.66 readModeInkLockDown. By default, this value is 0.
            //G - fAutoCompressPictures (1 bit): Specifies whether pictures in the document are automatically compressed when the document is saved as specified in [ECMA-376] Part 4, Section 2.15.1.32 doNotAutoCompressPictures, where the meaning is the opposite of fAutoCompressPictures. By default, this value is 1.
            //reserved3 (21 bits): This value MUST be 0, and MUST be ignored.
            m_flagsA = BaseWordRecord.ReadUInt16(stream);
            //reserved3 (21 bits): Reads remaining 16 bits.
            BaseWordRecord.ReadUInt16(stream);
            //empty3 (4 bytes): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty4 (4 bytes): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty5 (4 bytes): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty6 (4 bytes): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //dopMth (34 bytes): A DopMth that specifies various math properties.
            DopMath.Parse(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //reserved1 (4 bytes): This value is undefined, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //A - fRMTrackFormatting (1 bit): Specifies whether to track format changes when tracking for revisions (DopBase.fRevMarking). By default, this value is 1.
            //B - fRMTrackMoves (1 bit): Specifies whether to track moved text when tracking for revisions (DopBase.fRevMarking) instead of tracking for the deletions and insertions that are made. By default, this value is 1.
            //C - reserved2 (1 bit): This value MUST be 0, and MUST be ignored.
            //D - empty1 (1 bit): This value MUST be 0, and MUST be ignored.
            //E - empty2 (1 bit): This value MUST be 0, and MUST be ignored.
            //ssm (4 bits): An unsigned integer that specifies the sorting method to use when displaying document styles. 
            //F - fReadingModeInkLockDownActualPage (1 bit): Specifies whether to render the document with actual pages or virtual pages as specified in [ECMA-376] Part 4, Section 2.15.1.66 readModeInkLockDown. By default, this value is 0.
            //G - fAutoCompressPictures (1 bit): Specifies whether pictures in the document are automatically compressed when the document is saved as specified in [ECMA-376] Part 4, Section 2.15.1.32 doNotAutoCompressPictures, where the meaning is the opposite of fAutoCompressPictures. By default, this value is 1.
            //reserved3 (21 bits): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, m_flagsA);
            //reserved3 (21 bits): Reads remaining 16 bits.
            BaseWordRecord.WriteUInt16(stream, 0);
            //empty3 (4 bytes): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //empty4 (4 bytes): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //empty5 (4 bytes): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //empty6 (4 bytes): This value MUST be 0, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //dopMth (34 bytes): A DopMth that specifies various math properties.
            DopMath.Write(stream);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Asumyi.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Asumyi
    {
        #region Fields
        private byte m_flagsA;
        private ushort m_wDlgLevel = 25;
        private uint m_lHighestLevel;
        private uint m_lCurrentLevel;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Asumyi"/> is valid.
        /// </summary>
        /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
        internal bool Valid
        {
            get
            {
                return (m_flagsA & 0x1) != 0;
            }
            set
            {
                m_flagsA = (byte)((m_flagsA & 0xFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Asumyi"/> is view.
        /// </summary>
        /// <value><c>true</c> if view; otherwise, <c>false</c>.</value>
        internal bool View
        {
            get
            {
                return ((m_flagsA & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsA = (byte)((m_flagsA & 0xFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets the view by.
        /// </summary>
        /// <value>The view by.</value>
        internal byte ViewBy
        {
            get
            {
                return (byte)((m_flagsA & 0xC) >> 2);
            }
            set
            {
                m_flagsA = (byte)((m_flagsA & 0xF3) | (value << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [update props].
        /// </summary>
        /// <value><c>true</c> if [update props]; otherwise, <c>false</c>.</value>
        internal bool UpdateProps
        {
            get
            {
                return ((m_flagsA & 0x10) >> 4) != 0;
            }
            set
            {
                m_flagsA = (byte)((m_flagsA & 0xEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        /// Gets or sets the W DLG level.
        /// </summary>
        /// <value>The W DLG level.</value>
        internal ushort WDlgLevel
        {
            get
            {
                return m_wDlgLevel;
            }
            set
            {
                m_wDlgLevel = value;
            }
        }
        /// <summary>
        /// Gets or sets the L highest level.
        /// </summary>
        /// <value>The L highest level.</value>
        internal uint LHighestLevel
        {
            get
            {
                return m_lHighestLevel;
            }
            set
            {
                m_lHighestLevel = value;
            }
        }
        /// <summary>
        /// Gets or sets the L current level.
        /// </summary>
        /// <value>The L current level.</value>
        internal uint LCurrentLevel
        {
            get
            {
                return m_lCurrentLevel;
            }
            set
            {
                m_lCurrentLevel = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Asumyi"/> class.
        /// </summary>
        internal Asumyi()
        {
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //A - fValid (1 bit): Specifies whether the rest of the information in the Asumyi is currently valid.
            //B - fView (1 bit): Specifies whether the AutoSummary view is currently active.
            //C - iViewBy (2 bits): Specifies the type of AutoSummary to use. 
            //D - fUpdateProps (1 bit): Specifies whether to update the document summary information to reflect the AutoSummary results after the next summarization.
            //reserved (11 bits): This value MUST be zero, and MUST be ignored.
            m_flagsA = (byte)stream.ReadByte();
            //reserved (11 bits): Reads remaining 8 bits.
            stream.ReadByte();
            //wDlgLevel (2 bytes): Specifies the desired size of the summary.
            m_wDlgLevel = BaseWordRecord.ReadUInt16(stream);
            //lHighestLevel (4 bytes): If fValid is set to 1, this value MUST be greater than or equal to the highest value of ASUMY.lLevel. 
            m_lHighestLevel = BaseWordRecord.ReadUInt32(stream);
            //lCurrentLevel (4 bytes): If fValid is set to 1.
            m_lCurrentLevel = BaseWordRecord.ReadUInt32(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //A - fValid (1 bit): Specifies whether the rest of the information in the Asumyi is currently valid.
            //B - fView (1 bit): Specifies whether the AutoSummary view is currently active.
            //C - iViewBy (2 bits): Specifies the type of AutoSummary to use. 
            //D - fUpdateProps (1 bit): Specifies whether to update the document summary information to reflect the AutoSummary results after the next summarization.
            //reserved (11 bits): This value MUST be zero, and MUST be ignored.
            stream.WriteByte(m_flagsA);
            //reserved (11 bits): Reads remaining 8 bits.
            stream.WriteByte(0);
            //wDlgLevel (2 bytes): Specifies the desired size of the summary.
            BaseWordRecord.WriteUInt16(stream, m_wDlgLevel);
            //lHighestLevel (4 bytes): If fValid is set to 1, this value MUST be greater than or equal to the highest value of ASUMY.lLevel. 
            BaseWordRecord.WriteUInt32(stream, m_lHighestLevel);
            //lCurrentLevel (4 bytes): If fValid is set to 1.
            BaseWordRecord.WriteUInt32(stream, m_lCurrentLevel);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Dogrid.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Dogrid
    {
        #region Fields
        private ushort m_xaGrid = 1701;
        private ushort m_yaGrid = 1984;
        private ushort m_dxaGrid = 180;
        private ushort m_dyaGrid = 180;
        private byte m_flagsA = 1;
        private byte m_flagsB = 129;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the xa grid.
        /// </summary>
        /// <value>The xa grid.</value>
        internal ushort XaGrid
        {
            get
            {
                return m_xaGrid;
            }
            set
            {
                m_xaGrid = value;
            }
        }
        /// <summary>
        /// Gets or sets the ya grid.
        /// </summary>
        /// <value>The ya grid.</value>
        internal ushort YaGrid
        {
            get
            {
                return m_yaGrid;
            }
            set
            {
                m_yaGrid = value;
            }
        }
        /// <summary>
        /// Gets or sets the dxa grid.
        /// </summary>
        /// <value>The dxa grid.</value>
        internal ushort DxaGrid
        {
            get
            {
                return m_dxaGrid;
            }
            set
            {
                m_dxaGrid = value;
            }
        }
        /// <summary>
        /// Gets or sets the dya grid.
        /// </summary>
        /// <value>The dya grid.</value>
        internal ushort DyaGrid
        {
            get
            {
                return m_dyaGrid;
            }
            set
            {
                m_dyaGrid = value;
            }
        }
        /// <summary>
        /// Gets or sets the dy grid display.
        /// </summary>
        /// <value>The dy grid display.</value>
        internal byte DyGridDisplay
        {
            get
            {
                return (byte)(m_flagsA & 0x7F);
            }
            set
            {
                m_flagsA = (byte)((m_flagsA & 0x80) | value);
            }
        }
        /// <summary>
        /// Gets or sets the dx grid display.
        /// </summary>
        /// <value>The dx grid display.</value>
        internal byte DxGridDisplay
        {
            get
            {
                return (byte)(m_flagsB & 0x7F);
            }
            set
            {
                m_flagsB = (byte)((m_flagsB & 0x80) | value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [follow margins].
        /// </summary>
        /// <value><c>true</c> if [follow margins]; otherwise, <c>false</c>.</value>
        internal bool FollowMargins
        {
            get
            {
                return ((m_flagsB & 0x80) >> 7) != 0;
            }
            set
            {
                m_flagsB = (byte)((m_flagsB & 0x7F) | ((value ? 1 : 0) << 7));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Dogrid"/> class.
        /// </summary>
        internal Dogrid()
        {
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //xaGrid (2 bytes): An XAS_nonNeg that specifies horizontal origin point of the drawing grid. See [ECMA-376] Part 4, Section 2.15.1.43 (drawingGridHorizontalOrigin), where doNotUseMarginsForDrawingGridOrigin has the opposite meaning of fFollowMargins. The default value is 1701.
            m_xaGrid = BaseWordRecord.ReadUInt16(stream);
            //yaGrid (2 bytes): A YAS_nonNeg that specifies the vertical origin point of the drawing grid. See [ECMA-376] Part 4, Section 2.15.1.45 (drawingGridVerticalOrigin), where doNotUseMarginsForDrawingGridOrigin has the opposite meaning of fFollowMargins. The default value is 1984.
            m_yaGrid = BaseWordRecord.ReadUInt16(stream);
            //dxaGrid (2 bytes): An XAS_nonNeg that specifies the horizontal grid unit size of the drawing grid. See [ECMA-376] Part 4, Section 2.15.1.44 (drawingGridHorizontalSpacing). The default value is 180.
            m_dxaGrid = BaseWordRecord.ReadUInt16(stream);
            //dyaGrid (2 bytes): A YAS_nonNeg that specifies the vertical grid unit size of the drawing grid. See [ECMA-376] Part 4, Section 2.15.1.46 (drawingGridVerticalSpacing). The default value is 180.
            m_dyaGrid = BaseWordRecord.ReadUInt16(stream);
            //dyGridDisplay (7 bits): A positive value, in units specified by dyaGrid, that specifies the distance between vertical gridlines. See [ECMA-376] Part 4, Section 2.15.1.27 (displayVerticalDrawingGridEvery) where drawingGridVerticalSpacing refers to dyaGrid. The default value is 1.
            //A - unused (1 bit): This value is undefined, and MUST be ignored.
            m_flagsA = (byte)stream.ReadByte();
            //dxGridDisplay (7 bits): A positive value, in units specified by dxaGrid, that specifies the distance between horizontal gridlines. See [ECMA-376] Part 4, Section 2.15.1.26. (displayHorizontalDrawingGridEvery) where drawingGridHorizontalSpacing refers to dxaGrid. The default value is 1.
            //B - fFollowMargins (1 bit): A value that specifies whether to use margins for drawing grid origin. See [ECMA-376] Part 4, Section 2.15.1.41 (doNotUseMarginsForDrawingGridOrigin), where the meaning is the opposite of fFollowMargins. The default is 1.
            m_flagsB = (byte)stream.ReadByte();
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //xaGrid (2 bytes): An XAS_nonNeg that specifies horizontal origin point of the drawing grid. See [ECMA-376] Part 4, Section 2.15.1.43 (drawingGridHorizontalOrigin), where doNotUseMarginsForDrawingGridOrigin has the opposite meaning of fFollowMargins. The default value is 1701.
            BaseWordRecord.WriteUInt16(stream, m_xaGrid);
            //yaGrid (2 bytes): A YAS_nonNeg that specifies the vertical origin point of the drawing grid. See [ECMA-376] Part 4, Section 2.15.1.45 (drawingGridVerticalOrigin), where doNotUseMarginsForDrawingGridOrigin has the opposite meaning of fFollowMargins. The default value is 1984.
            BaseWordRecord.WriteUInt16(stream, m_yaGrid);
            //dxaGrid (2 bytes): An XAS_nonNeg that specifies the horizontal grid unit size of the drawing grid. See [ECMA-376] Part 4, Section 2.15.1.44 (drawingGridHorizontalSpacing). The default value is 180.
            BaseWordRecord.WriteUInt16(stream, m_dxaGrid);
            //dyaGrid (2 bytes): A YAS_nonNeg that specifies the vertical grid unit size of the drawing grid. See [ECMA-376] Part 4, Section 2.15.1.46 (drawingGridVerticalSpacing). The default value is 180.
            BaseWordRecord.WriteUInt16(stream, m_dyaGrid);
            //dyGridDisplay (7 bits): A positive value, in units specified by dyaGrid, that specifies the distance between vertical gridlines. See [ECMA-376] Part 4, Section 2.15.1.27 (displayVerticalDrawingGridEvery) where drawingGridVerticalSpacing refers to dyaGrid. The default value is 1.
            //A - unused (1 bit): This value is undefined, and MUST be ignored.
            stream.WriteByte(m_flagsA);
            //dxGridDisplay (7 bits): A positive value, in units specified by dxaGrid, that specifies the distance between horizontal gridlines. See [ECMA-376] Part 4, Section 2.15.1.26. (displayHorizontalDrawingGridEvery) where drawingGridHorizontalSpacing refers to dxaGrid. The default value is 1.
            //B - fFollowMargins (1 bit): A value that specifies whether to use margins for drawing grid origin. See [ECMA-376] Part 4, Section 2.15.1.41 (doNotUseMarginsForDrawingGridOrigin), where the meaning is the opposite of fFollowMargins. The default is 1.
            stream.WriteByte(m_flagsB);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of DopTypography.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DopTypography
    {
        #region Fields
        private ushort m_flagsA;
        private ushort m_cchFollowingPunct;
        private ushort m_cchLeadingPunct;
        private byte[] m_rgxchFPunct = new byte[101 * Constants.BytesInWord];
        private byte[] m_rgxchLPunct = new byte[51 * Constants.BytesInWord];
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [kerning punct].
        /// </summary>
        /// <value><c>true</c> if [kerning punct]; otherwise, <c>false</c>.</value>
        internal bool KerningPunct
        {
            get
            {
                return (m_flagsA & 0x1) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets the justification.
        /// </summary>
        /// <value>The justification.</value>
        internal byte Justification
        {
            get
            {
                return (byte)((m_flagsA & 0x6) >> 1);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFF9) | (value << 1));
            }
        }
        /// <summary>
        /// Gets or sets the level of kinsoku.
        /// </summary>
        /// <value>The level of kinsoku.</value>
        internal byte LevelOfKinsoku
        {
            get
            {
                return (byte)((m_flagsA & 0x18) >> 3);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFE7) | (value << 3));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DopTypography"/> is print2on1.
        /// </summary>
        /// <value><c>true</c> if print2on1; otherwise, <c>false</c>.</value>
        internal bool Print2on1
        {
            get
            {
                return ((m_flagsA & 0x20) >> 5) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFDF) | ((value ? 1 : 0) << 5));
            }
        }
        /// <summary>
        /// Gets or sets the custom ksu.
        /// </summary>
        /// <value>The custom ksu.</value>
        internal byte CustomKsu
        {
            get
            {
                return (byte)((m_flagsA & 0x380) >> 7);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFC7F) | (value << 7));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [japanese use level2].
        /// </summary>
        /// <value><c>true</c> if [japanese use level2]; otherwise, <c>false</c>.</value>
        internal bool JapaneseUseLevel2
        {
            get
            {
                return ((m_flagsA & 0x400) >> 10) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets the CCH following punct.
        /// </summary>
        /// <value>The CCH following punct.</value>
        internal ushort CchFollowingPunct
        {
            get
            {
                return m_cchFollowingPunct;
            }
            set
            {
                m_cchFollowingPunct = value;
            }
        }
        /// <summary>
        /// Gets or sets the CCH leading punct.
        /// </summary>
        /// <value>The CCH leading punct.</value>
        internal ushort CchLeadingPunct
        {
            get
            {
                return m_cchLeadingPunct;
            }
            set
            {
                m_cchLeadingPunct = value;
            }
        }
        /// <summary>
        /// Gets or sets the RGXCH F punct.
        /// </summary>
        /// <value>The RGXCH F punct.</value>
        internal byte[] RgxchFPunct
        {
            get
            {
                return m_rgxchFPunct;
            }
            set
            {
                m_rgxchFPunct = value;
            }
        }
        /// <summary>
        /// Gets or sets the RGXCH L punct.
        /// </summary>
        /// <value>The RGXCH L punct.</value>
        internal byte[] RgxchLPunct
        {
            get
            {
                return m_rgxchLPunct;
            }
            set
            {
                m_rgxchLPunct = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DopTypography"/> class.
        /// </summary>
        internal DopTypography()
        {
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //A - fKerningPunct (1 bit): Specifies whether to kern punctuation characters as specified in [ECMA-376] Part 4, Section 2.15.1.60 noPunctuationKerning, where the meaning of noPunctuationKerning is the opposite of fKerningPunct.
            //B - iJustification (2 bits): Specifies the character-level whitespace compression as specified in [ECMA-376] Part 4, Section 2.15.1.18 characterSpacingControl. 
            //C - iLevelOfKinsoku (2 bits): This value MAY<201> specify which set of line breaking rules to use for East Asian characters.
            //D - f2on1 (1 bit): Specifies whether to print two pages per sheet, as specified in [ECMA-376] Part 4, Section 2.15.1.64 printTwoOnOne.
            //E - unused (1 bit): This value is undefined and MUST be ignored.
            //F - iCustomKsu (3 bits): This value specifies for what language the characters in rgxchFPunct are kinsoku overrides<202>. All other languages act according to the description of iLevelOfKinsoku with a value of 0.
            //G - fJapaneseUseLevel2 (1 bit): This value specifies that line breaking rules for Japanese should act according to the description of iLevelOfKinsoku with a value of 1<203>. The default value is 0.
            //reserved (5 bits): This value MUST be zero, and MUST be ignored.
            m_flagsA = BaseWordRecord.ReadUInt16(stream);
            //cchFollowingPunct (2 bytes): A signed integer that specifies the number of characters in rgxchFPunct. This MUST be a value between 0x0000 and 0x0064 inclusive. By default, this value is 0x0000.
            m_cchFollowingPunct = BaseWordRecord.ReadUInt16(stream);
            //cchLeadingPunct (2 bytes): A signed integer that specifies the number of characters in rgxchLPunct. This MUST be a value between 0x0000 and 0x0032, inclusive. By default, this value is 0x0000.
            m_cchLeadingPunct = BaseWordRecord.ReadUInt16(stream);
            //rgxchFPunct (202 bytes): An array of cchFollowingPunctUnicode characters that cannot start a line if the language of the text matches the language specified in iCustomKsu. If iCustomKsu has a value of 0, this array has no effect on the document.
            stream.Read(m_rgxchFPunct, 0, m_rgxchFPunct.Length);
            //rgxchLPunct (102 bytes): An array of cchLeadingPunct Unicode characters that cannot end a line if the language of the text matches the language specified in iCustomKsu. If iCustomKsu has a value of 0, this array has no effect on the document.
            stream.Read(m_rgxchLPunct, 0, m_rgxchLPunct.Length);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //A - fKerningPunct (1 bit): Specifies whether to kern punctuation characters as specified in [ECMA-376] Part 4, Section 2.15.1.60 noPunctuationKerning, where the meaning of noPunctuationKerning is the opposite of fKerningPunct.
            //B - iJustification (2 bits): Specifies the character-level whitespace compression as specified in [ECMA-376] Part 4, Section 2.15.1.18 characterSpacingControl. 
            //C - iLevelOfKinsoku (2 bits): This value MAY<201> specify which set of line breaking rules to use for East Asian characters.
            //D - f2on1 (1 bit): Specifies whether to print two pages per sheet, as specified in [ECMA-376] Part 4, Section 2.15.1.64 printTwoOnOne.
            //E - unused (1 bit): This value is undefined and MUST be ignored.
            //F - iCustomKsu (3 bits): This value specifies for what language the characters in rgxchFPunct are kinsoku overrides<202>. All other languages act according to the description of iLevelOfKinsoku with a value of 0.
            //G - fJapaneseUseLevel2 (1 bit): This value specifies that line breaking rules for Japanese should act according to the description of iLevelOfKinsoku with a value of 1<203>. The default value is 0.
            //reserved (5 bits): This value MUST be zero, and MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, m_flagsA);
            //cchFollowingPunct (2 bytes): A signed integer that specifies the number of characters in rgxchFPunct. This MUST be a value between 0x0000 and 0x0064 inclusive. By default, this value is 0x0000.
            BaseWordRecord.WriteUInt16(stream, m_cchFollowingPunct);
            //cchLeadingPunct (2 bytes): A signed integer that specifies the number of characters in rgxchLPunct. This MUST be a value between 0x0000 and 0x0032, inclusive. By default, this value is 0x0000.
            BaseWordRecord.WriteUInt16(stream, m_cchLeadingPunct);
            //rgxchFPunct (202 bytes): An array of cchFollowingPunctUnicode characters that cannot start a line if the language of the text matches the language specified in iCustomKsu. If iCustomKsu has a value of 0, this array has no effect on the document.
            stream.Write(m_rgxchFPunct, 0, m_rgxchFPunct.Length);
            //rgxchLPunct (102 bytes): An array of cchLeadingPunct Unicode characters that cannot end a line if the language of the text matches the language specified in iCustomKsu. If iCustomKsu has a value of 0, this array has no effect on the document.
            stream.Write(m_rgxchLPunct, 0, m_rgxchLPunct.Length);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of DopMth.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DopMth
    {
        #region Fields
        private ushort m_flagsA = 6160;
        private ushort m_ftcMath;
        private uint m_dxaLeftMargin;
        private uint m_dxaRightMargin;
        private uint m_dxaIndentWrapped = 1440;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the MTHBRK.
        /// </summary>
        /// <value>The MTHBRK.</value>
        internal byte Mthbrk
        {
            get
            {
                return (byte)(m_flagsA & 0x3);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFFC) | value);
            }
        }
        /// <summary>
        /// Gets or sets the MTHBRK sub.
        /// </summary>
        /// <value>The MTHBRK sub.</value>
        internal byte MthbrkSub 
        {
            get
            {
                return (byte)((m_flagsA & 0xC) >> 2);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFFF3) | (value << 2));
            }
        }
        /// <summary>
        /// Gets or sets the MTHBPJC.
        /// </summary>
        /// <value>The MTHBPJC.</value>
        internal byte Mthbpjc
        {
            get
            {
                return (byte)((m_flagsA & 0x70) >> 4);
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFF8F) | (value << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [math small frac].
        /// </summary>
        /// <value><c>true</c> if [math small frac]; otherwise, <c>false</c>.</value>
        internal bool MathSmallFrac
        {
            get
            {
                return ((m_flagsA & 0x100) >> 8) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFEFF) | ((value ? 1 : 0) << 8));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [math int lim und ovr].
        /// </summary>
        /// <value><c>true</c> if [math int lim und ovr]; otherwise, <c>false</c>.</value>
        internal bool MathIntLimUndOvr
        {
            get
            {
                return ((m_flagsA & 0x200) >> 9) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFDFF) | ((value ? 1 : 0) << 9));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [math nary lim und ovr].
        /// </summary>
        /// <value><c>true</c> if [math nary lim und ovr]; otherwise, <c>false</c>.</value>
        internal bool MathNaryLimUndOvr
        {
            get
            {
                return ((m_flagsA & 0x400) >> 10) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [math wrap align left].
        /// </summary>
        /// <value><c>true</c> if [math wrap align left]; otherwise, <c>false</c>.</value>
        internal bool MathWrapAlignLeft
        {
            get
            {
                return ((m_flagsA & 0x800) >> 11) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xF7FF) | ((value ? 1 : 0) << 11));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [math use disp defaults].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [math use disp defaults]; otherwise, <c>false</c>.
        /// </value>
        internal bool MathUseDispDefaults
        {
            get
            {
                return ((m_flagsA & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flagsA = (ushort)((m_flagsA & 0xEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets the FTC math.
        /// </summary>
        /// <value>The FTC math.</value>
        internal ushort FtcMath
        {
            get
            {
                return m_ftcMath;
            }
            set
            {
                m_ftcMath = value;
            }
        }
        /// <summary>
        /// Gets or sets the dxa left margin.
        /// </summary>
        /// <value>The dxa left margin.</value>
        internal uint DxaLeftMargin
        {
            get
            {
                return m_dxaLeftMargin;
            }
            set
            {
                m_dxaLeftMargin = value;
            }
        }
        /// <summary>
        /// Gets or sets the dxa right margin.
        /// </summary>
        /// <value>The dxa right margin.</value>
        internal uint DxaRightMargin
        {
            get
            {
                return m_dxaRightMargin;
            }
            set
            {
                m_dxaRightMargin = value;
            }
        }
        /// <summary>
        /// Gets or sets the dxa indent wrapped.
        /// </summary>
        /// <value>The dxa indent wrapped.</value>
        internal uint DxaIndentWrapped
        {
            get
            {
                return m_dxaIndentWrapped;
            }
            set
            {
                m_dxaIndentWrapped = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DopMth"/> class.
        /// </summary>
        internal DopMth()
        {
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //A - mthbrk (2 bits): Specifies how to break on binary operators as specified in [ECMA-376] Part 4, Section 7.1.2.16 brkBin.
            //B - mthbrkSub (2 bits): Specifies how to break on binary subtraction when mthbrk is 2 as specified in [ECMA-376] Part 4, Section 7.1.2.17 brkBinSub.
            //C - mthbpjc (3 bits): Specifies the default justification of math as specified in [ECMA-376] Part 4, Section 7.1.2.25 defJc.
            //D - reserved1 (1 bit): This value is undefined and MUST be ignored.
            //E - fMathSmallFrac (1 bit): Specifies whether to use a reduced fraction size when displaying math that contains fractions as specified in [ECMA-376] Part 4, Section 7.1.2.98 smallFrac. By default, this value is 0.
            //F - fMathIntLimUndOvr (1 bit): Specifies that the default placement of integral limits when converting from a linear format is directly above and below the base as opposed to on the side of the base as specified in [ECMA-376] Part 4, Section 7.1.2.49 intLim. By default, this value is 0.
            //G - fMathNaryLimUndOvr (1 bit): Specifies that the default placement of n-ary limits other than integrals is directly above and below the base, as opposed to on the side of the base, as specified in [ECMA-376] Part 4, Section 7.1.2.71 naryLim. By default, this value is 0.
            //H - fMathWrapAlignLeft (1 bit): Specifies the left justification of the wrapped line of an equation as opposed to right justification of the wrapped line of an equation as specified in [ECMA-376] Part 4, Section 7.1.2.121 wrapRight where the meaning is the opposite of fMathWrapAlignLeft. By default, this value is 1.
            //I - fMathUseDispDefaults (1 bit): Specifies whether to use display math defaults as specified in [ECMA-376] Part 4, Section 7.1.2.30 dispDef. By default, this value is 1.
            //reserved2 (19 bits): This value MUST be zero, and MUST be ignored.
            m_flagsA = BaseWordRecord.ReadUInt16(stream);
            //reserved2 (19 bits): Reads remaining 16 bits.
            BaseWordRecord.ReadUInt16(stream);
            //ftcMath (2 bytes): An index into an SttbfFfn structure that specifies the font to use for new equations in the document. The default font is Cambria Math.
            m_ftcMath = BaseWordRecord.ReadUInt16(stream);
            //dxaLeftMargin (4 bytes): A signed integer, in twips, that specifies the left margin for math. MUST be greater than or equal to 0 and less than or equal to 31680 as specified in [ECMA-376] Part 4, Section 7.1.2.59 lMargin. By default, this value is 0.
            m_dxaLeftMargin = BaseWordRecord.ReadUInt32(stream);
            //dxaRightMargin (4 bytes): A signed integer in twips that specifies the right margin for math. This value MUST be greater than or equal to 0 and less than or equal to 31680, as specified in [ECMA-376] Part 4, Section 7.1.2.90 rMargin. By default, this value is 0.
            m_dxaRightMargin = BaseWordRecord.ReadUInt32(stream);
            //empty1 (4 bytes): This value MUST be 120, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty2 (4 bytes): This value MUST be 120, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty3 (4 bytes): This value MUST be zero, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty4 (4 bytes): This value MUST be zero, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //dxaIndentWrapped (4 bytes): A signed integer, in twips, that specifies the indentation of the wrapped line of an equation. This value MUST be greater than or equal to 0 and less than or equal to 31680 as specified in [ECMA-376] Part 4, Section 7.1.2.120 wrapIndent. By default, this value is 1440.
            m_dxaIndentWrapped = BaseWordRecord.ReadUInt32(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //A - mthbrk (2 bits): Specifies how to break on binary operators as specified in [ECMA-376] Part 4, Section 7.1.2.16 brkBin.
            //B - mthbrkSub (2 bits): Specifies how to break on binary subtraction when mthbrk is 2 as specified in [ECMA-376] Part 4, Section 7.1.2.17 brkBinSub.
            //C - mthbpjc (3 bits): Specifies the default justification of math as specified in [ECMA-376] Part 4, Section 7.1.2.25 defJc.
            //D - reserved1 (1 bit): This value is undefined and MUST be ignored.
            //E - fMathSmallFrac (1 bit): Specifies whether to use a reduced fraction size when displaying math that contains fractions as specified in [ECMA-376] Part 4, Section 7.1.2.98 smallFrac. By default, this value is 0.
            //F - fMathIntLimUndOvr (1 bit): Specifies that the default placement of integral limits when converting from a linear format is directly above and below the base as opposed to on the side of the base as specified in [ECMA-376] Part 4, Section 7.1.2.49 intLim. By default, this value is 0.
            //G - fMathNaryLimUndOvr (1 bit): Specifies that the default placement of n-ary limits other than integrals is directly above and below the base, as opposed to on the side of the base, as specified in [ECMA-376] Part 4, Section 7.1.2.71 naryLim. By default, this value is 0.
            //H - fMathWrapAlignLeft (1 bit): Specifies the left justification of the wrapped line of an equation as opposed to right justification of the wrapped line of an equation as specified in [ECMA-376] Part 4, Section 7.1.2.121 wrapRight where the meaning is the opposite of fMathWrapAlignLeft. By default, this value is 1.
            //I - fMathUseDispDefaults (1 bit): Specifies whether to use display math defaults as specified in [ECMA-376] Part 4, Section 7.1.2.30 dispDef. By default, this value is 1.
            //reserved2 (19 bits): This value MUST be zero, and MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, m_flagsA);
            //reserved2 (19 bits): Reads remaining 16 bits.
            BaseWordRecord.WriteUInt16(stream, 0);
            //ftcMath (2 bytes): An index into an SttbfFfn structure that specifies the font to use for new equations in the document. The default font is Cambria Math.
            BaseWordRecord.WriteUInt16(stream, m_ftcMath);
            //dxaLeftMargin (4 bytes): A signed integer, in twips, that specifies the left margin for math. MUST be greater than or equal to 0 and less than or equal to 31680 as specified in [ECMA-376] Part 4, Section 7.1.2.59 lMargin. By default, this value is 0.
            BaseWordRecord.WriteUInt32(stream, m_dxaLeftMargin);
            //dxaRightMargin (4 bytes): A signed integer in twips that specifies the right margin for math. This value MUST be greater than or equal to 0 and less than or equal to 31680, as specified in [ECMA-376] Part 4, Section 7.1.2.90 rMargin. By default, this value is 0.
            BaseWordRecord.WriteUInt32(stream, m_dxaRightMargin);
            //empty1 (4 bytes): This value MUST be 120, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 120);
            //empty2 (4 bytes): This value MUST be 120, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 120);
            //empty3 (4 bytes): This value MUST be zero, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //empty4 (4 bytes): This value MUST be zero, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //dxaIndentWrapped (4 bytes): A signed integer, in twips, that specifies the indentation of the wrapped line of an equation. This value MUST be greater than or equal to 0 and less than or equal to 31680 as specified in [ECMA-376] Part 4, Section 7.1.2.120 wrapIndent. By default, this value is 1440.
            BaseWordRecord.WriteUInt32(stream, m_dxaIndentWrapped);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Copts.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Copts
    {
        #region Fields
        private uint m_flagsA = 8;
        private byte m_flagsg = 0;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the copts80.
        /// </summary>
        /// <value>The copts80.</value>
        internal Copts80 Copts80
        {
            get
            {
                return m_dopBase.Dop95.Copts80;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [sp layout like W w8].
        /// </summary>
        /// <value><c>true</c> if [sp layout like W w8]; otherwise, <c>false</c>.</value>
        internal bool SpLayoutLikeWW8
        {
            get
            {
                return (m_flagsA & 0x1) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [FTN layout like W w8].
        /// </summary>
        /// <value><c>true</c> if [FTN layout like W w8]; otherwise, <c>false</c>.</value>
        internal bool FtnLayoutLikeWW8
        {
            get
            {
                return ((m_flagsA & 0x2) >> 1) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont use HTML paragraph auto spacing].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont use HTML paragraph auto spacing]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontUseHTMLParagraphAutoSpacing
        {
            get
            {
                return ((m_flagsA & 0x4) >> 2) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFFFB) | ((value ? 1 : 0) << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont adjust line height in table].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont adjust line height in table]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontAdjustLineHeightInTable
        {
            get
            {
                return ((m_flagsA & 0x8) >> 3) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFFF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [forget last tab align].
        /// </summary>
        /// <value><c>true</c> if [forget last tab align]; otherwise, <c>false</c>.</value>
        internal bool ForgetLastTabAlign
        {
            get
            {
                return ((m_flagsA & 0x10) >> 4) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFFEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [use autospace for full width alpha].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use autospace for full width alpha]; otherwise, <c>false</c>.
        /// </value>
        internal bool UseAutospaceForFullWidthAlpha
        {
            get
            {
                return ((m_flagsA & 0x20) >> 5) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFFDF) | ((value ? 1 : 0) << 5));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [align tables row by row].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [align tables row by row]; otherwise, <c>false</c>.
        /// </value>
        internal bool AlignTablesRowByRow
        {
            get
            {
                return ((m_flagsA & 0x40) >> 6) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFFBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [layout raw table width].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [layout raw table width]; otherwise, <c>false</c>.
        /// </value>
        internal bool LayoutRawTableWidth
        {
            get
            {
                return ((m_flagsA & 0x80) >> 7) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFF7F) | ((value ? 1 : 0) << 7));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [layout table rows apart].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [layout table rows apart]; otherwise, <c>false</c>.
        /// </value>
        internal bool LayoutTableRowsApart
        {
            get
            {
                return ((m_flagsA & 0x100) >> 8) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFEFF) | ((value ? 1 : 0) << 8));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [use word97 line breaking rules].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use word97 line breaking rules]; otherwise, <c>false</c>.
        /// </value>
        internal bool UseWord97LineBreakingRules
        {
            get
            {
                return ((m_flagsA & 0x200) >> 9) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFDFF) | ((value ? 1 : 0) << 9));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont break wrapped tables].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont break wrapped tables]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontBreakWrappedTables
        {
            get
            {
                return ((m_flagsA & 0x400) >> 10) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont snap to grid in cell].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont snap to grid in cell]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontSnapToGridInCell
        {
            get
            {
                return ((m_flagsA & 0x800) >> 11) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFF7FF) | ((value ? 1 : 0) << 11));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont allow field end select].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont allow field end select]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontAllowFieldEndSelect
        {
            get
            {
                return ((m_flagsA & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [apply breaking rules].
        /// </summary>
        /// <value><c>true</c> if [apply breaking rules]; otherwise, <c>false</c>.</value>
        internal bool ApplyBreakingRules
        {
            get
            {
                return ((m_flagsA & 0x2000) >> 13) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFDFFF) | ((value ? 1 : 0) << 13));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont wrap text with punct].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont wrap text with punct]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontWrapTextWithPunct
        {
            get
            {
                return ((m_flagsA & 0x4000) >> 14) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFFBFFF) | ((value ? 1 : 0) << 14));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont use asian break rules].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont use asian break rules]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontUseAsianBreakRules
        {
            get
            {
                return ((m_flagsA & 0x8000) >> 15) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFF7FFF) | ((value ? 1 : 0) << 15));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [use word2002 table style rules].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use word2002 table style rules]; otherwise, <c>false</c>.
        /// </value>
        internal bool UseWord2002TableStyleRules
        {
            get
            {
                return ((m_flagsA & 0x10000) >> 16) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFEFFFF) | ((value ? 1 : 0) << 16));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [grow auto fit].
        /// </summary>
        /// <value><c>true</c> if [grow auto fit]; otherwise, <c>false</c>.</value>
        internal bool GrowAutoFit
        {
            get
            {
                return ((m_flagsA & 0x20000) >> 17) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFDFFFF) | ((value ? 1 : 0) << 17));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [use normal style for list].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use normal style for list]; otherwise, <c>false</c>.
        /// </value>
        internal bool UseNormalStyleForList
        {
            get
            {
                return ((m_flagsA & 0x40000) >> 18) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFFBFFFF) | ((value ? 1 : 0) << 18));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont use indent as numbering tab stop].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont use indent as numbering tab stop]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontUseIndentAsNumberingTabStop
        {
            get
            {
                return ((m_flagsA & 0x80000) >> 19) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFF7FFFF) | ((value ? 1 : 0) << 19));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [FE line break11].
        /// </summary>
        /// <value><c>true</c> if [FE line break11]; otherwise, <c>false</c>.</value>
        internal bool FELineBreak11
        {
            get
            {
                return ((m_flagsA & 0x100000) >> 20) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFEFFFFF) | ((value ? 1 : 0) << 20));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [allow space of same style in table].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow space of same style in table]; otherwise, <c>false</c>.
        /// </value>
        internal bool AllowSpaceOfSameStyleInTable
        {
            get
            {
                return ((m_flagsA & 0x200000) >> 21) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFDFFFFF) | ((value ? 1 : 0) << 21));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [W W11 indent rules].
        /// </summary>
        /// <value><c>true</c> if [W W11 indent rules]; otherwise, <c>false</c>.</value>
        internal bool WW11IndentRules
        {
            get
            {
                return ((m_flagsA & 0x400000) >> 22) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFFBFFFFF) | ((value ? 1 : 0) << 22));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont autofit constrained tables].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont autofit constrained tables]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontAutofitConstrainedTables
        {
            get
            {
                return ((m_flagsA & 0x800000) >> 23) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFF7FFFFF) | ((value ? 1 : 0) << 23));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [autofit like W W11].
        /// </summary>
        /// <value><c>true</c> if [autofit like W W11]; otherwise, <c>false</c>.</value>
        internal bool AutofitLikeWW11
        {
            get
            {
                return ((m_flagsA & 0x1000000) >> 24) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFEFFFFFF) | ((value ? 1 : 0) << 24));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [underline tab in num list].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [underline tab in num list]; otherwise, <c>false</c>.
        /// </value>
        internal bool UnderlineTabInNumList
        {
            get
            {
                return ((m_flagsA & 0x2000000) >> 25) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFDFFFFFF) | ((value ? 1 : 0) << 25));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [hangul width like W W11].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [hangul width like W W11]; otherwise, <c>false</c>.
        /// </value>
        internal bool HangulWidthLikeWW11
        {
            get
            {
                return ((m_flagsA & 0x4000000) >> 26) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xFBFFFFFF) | ((value ? 1 : 0) << 26));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [split pg break and para mark].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [split pg break and para mark]; otherwise, <c>false</c>.
        /// </value>
        internal bool SplitPgBreakAndParaMark
        {
            get
            {
                return ((m_flagsA & 0x8000000) >> 27) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xF7FFFFFF) | ((value ? 1 : 0) << 27));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont vert align cell with sp].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont vert align cell with sp]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontVertAlignCellWithSp
        {
            get
            {
                return ((m_flagsA & 0x10000000) >> 28) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xEFFFFFFF) | ((value ? 1 : 0) << 28));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont break constrained forced tables].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont break constrained forced tables]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontBreakConstrainedForcedTables
        {
            get
            {
                return ((m_flagsA & 0x20000000) >> 29) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xDFFFFFFF) | ((value ? 1 : 0) << 29));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont vert align in TXBX].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont vert align in TXBX]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontVertAlignInTxbx
        {
            get
            {
                return ((m_flagsA & 0x40000000) >> 30) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0xBFFFFFFF) | ((value ? 1 : 0) << 30));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [word11 kerning pairs].
        /// </summary>
        /// <value><c>true</c> if [word11 kerning pairs]; otherwise, <c>false</c>.</value>
        internal bool Word11KerningPairs
        {
            get
            {
                return ((m_flagsA & 0x80000000) >> 31) != 0;
            }
            set
            {
                m_flagsA = (uint)((m_flagsA & 0x7FFFFFFF) | ((value ? 1 : 0) << 31));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [cached col balance].
        /// </summary>
        /// <value><c>true</c> if [cached col balance]; otherwise, <c>false</c>.</value>
        internal bool CachedColBalance
        {
            get
            {
                return (m_flagsg & 0x1) != 0;
            }
            set
            {
                m_flagsg = (byte)((m_flagsg & 0xFE) | (value ? 1 : 0));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Copts"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal Copts(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //copts80 (4 bytes): A Copts80 that specifies additional compatibility options. Components of Copts.copts80 MUST be equal to components of Dop95.copts80.
            Copts80.Parse(stream);
            //A - fSpLayoutLikeWW8 (1 bit): Specifies whether to emulate Microsoft� Word 97 text wrapping around floating objects. Specified in [ECMA-376] part 4, 2.15.3.41 (shapeLayoutLikeWW8).
            //B - fFtnLayoutLikeWW8 (1 bit): Specifies whether to emulate Microsoft� Word 6.0, Microsoft� Word for Windows� 95, or Word 97 footnote placement. Specified in [ECMA-376] Part 4, 2.15.3.26 (footnoteLayoutLikeWW8).
            //C - fDontUseHTMLParagraphAutoSpacing (1 bit): Specifies whether to use fixed paragraph spacing for paragraphs specifying auto spacing. Specified in [ECMA-376] Part 4, 2.15.3.21 (doNotUseHTMLParagraphAutoSpacing).
            //D - fDontAdjustLineHeightInTable (1 bit): Prevents lines within tables from having their heights adjusted to comply with the document grid. See sprmSDyaLinePitch and [ECMA-376] Part 4, 2.15.3.1 (adjustLineHeightInTable) where the meaning is the opposite of fDontAdjustLineHeightInTable.
            //E - fForgetLastTabAlign (1 bit): Specifies whether to ignore width of the last tab stop when aligning a paragraph if the tab stop is not left aligned. Specified in [ECMA-376] Part 4, 2.15.3.27 (forgetLastTabAlignment) where jc refers to sprmPJc and the tab element refers to either sprmPChgTabs or sprmPChgTabsPapx.
            //F - fUseAutospaceForFullWidthAlpha (1 bit): Specifies whether to emulate Word for Windows 95 full-width character spacing. Specified in [ECMA-376] Part 4, 2.15.3.6 (autoSpaceLikeWord for Windows 95).
            //G - fAlignTablesRowByRow (1 bit): Specifies whether to align table rows independently. Specified in [ECMA-376] Part 4, 2.15.3.2 (alignTablesRowByRow) where the jc element refers to sprmTJc or sprmTJc90.
            //H - fLayoutRawTableWidth (1 bit): Specifies whether to ignore space before tables when deciding if a table should wrap a floating object. Specified in [ECMA-376] Part 4, 2.15.3.29 (layoutRawTableWidth).
            //I - fLayoutTableRowsApart (1 bit): Specifies whether to allow table rows to wrap inline objects independently. Specified in [ECMA-376] Part 4, 2.15.3.30 (layoutTableRowsApart).
            //J - fUseWord97LineBreakingRules (1 bit): Specifies whether to emulate Word 97 East Asian line breaking rules. Specified in [ECMA-376] Part 4, 2.15.3.64 (useWord97LineBreakRules).
            //K - fDontBreakWrappedTables (1 bit): Specifies whether to prevent floating tables from breaking across pages. Specified in [ECMA-376] Part 4, 2.15.3.14 (doNotBreakWrappedTables) where the tblpPr element refers to any of sprmTDxaAbs, sprmTDyaAbs, sprmTPc, sprmTDyaFromTextBottom, sprmTDyaFromText, sprmTDxaFromTextRight, or sprmTDxaFromText with a nondefault value specified.
            //L - fDontSnapToGridInCell (1 bit): Specifies whether to not snap to the document grid in table cells with objects. Specified in [ECMA-376] Part 4, 2.15.3.17 (doNotSnapToGridInCell) where the docGrid element refers to any of sprmSClm, sprmSDyaLinePitch or sprmSDxtCharSpace with a nondefault value specified.
            //M - fDontAllowFieldEndSelect (1 bit): Specifies whether to select an entire field when the first or last character of the field is selected. Specified in [ECMA-376] Part 4, 2.15.3.40 (selectFldWithFirstOrLastChar).
            //N - fApplyBreakingRules (1 bit): Specifies whether to use legacy Ethiopic and Amharic line breaking rules. Specified in [ECMA-376] Part 4, 2.15.3.4 (applyBreakingRules).
            //O - fDontWrapTextWithPunct (1 bit): Specifies whether to prevent hanging punctuation with the character grid. Specified in [ECMA-376] Part 4, 2.15.3.25 (doNotWrapTextWithPunct) where the docGrid element refers to any of sprmSClm, sprmSDyaLinePitch or sprmSDxtCharSpace with a nondefault value specified and the overflowPunct element refers to sprmPFOverflowPunct.
            //P - fDontUseAsianBreakRules (1 bit): Specifies whether to disallow the compressing of compressible characters when using the document grid. Specified in [ECMA-376] Part 4, 2.15.3.20 (doNotUseEastAsianBreakRules) where the docGrid element refers to any of sprmSClm, sprmSDyaLinePitch, or sprmSDxtCharSpace with a nondefault value specified
            //Q - fUseWord2002TableStyleRules (1 bit): Specifies whether to emulate Microsoft� Word 2002 table style rules. Specified in [ECMA-376] Part 4, 2.15.3.63 (useWord2002TableStyleRules).
            //R - fGrowAutoFit (1 bit): Specifies whether to allow tables to autofit into the page margins. Specified in [ECMA-376] Part 4, 2.15.3.28 (growAutofit).
            //S - fUseNormalStyleForList (1 bit): Specifies whether to not automatically apply the list paragraph style to bulleted or numbered text. Specified in [ECMA-376] Part 4, 2.15.3.60 (useNormalStyleForList). MAY<185> be ignored.
            //T - fDontUseIndentAsNumberingTabStop (1 bit): Specifies whether to ignore the hanging indent when creating a tab stop after numbering. Specified in [ECMA-376] Part 4, 2.15.3.22 (doNotUseIndentAsNumberingTabStop). MAY<186> be ignored.
            //U - fFELineBreak11 (1 bit): Specifies whether to use an alternate set of East Asian line breaking rules. Specified in [ECMA-376] Part 4, 2.15.3.57 (useAltKinsokuLineBreakRules). MAY<187> be ignored.
            //V - fAllowSpaceOfSameStyleInTable (1 bit): Specifies whether to allow contextual spacing of paragraphs in tables. Specified in [ECMA-376] Part 4, 2.15.3.3 (allowSpaceOfSameStyleInTable) where the contextualSpacing element refers to sprmPFContextualSpacing. MAY<188> be ignored.
            //W - fWW11IndentRules (1 bit): Specifies whether to not ignore floating objects when calculating paragraph indentation. Specified in [ECMA-376] Part 4, 2.15.3.18 (doNotSuppressIndentation). MAY<189> be ignored.
            //X - fDontAutofitConstrainedTables (1 bit): Specifies whether to not autofit tables such that they fit next to wrapped objects. Specified in [ECMA-376] Part 4, 2.15.3.12 (doNotAutofitConstrainedTables). MAY<190> be ignored.
            //Y - fAutofitLikeWW11 (1 bit): Specifies whether to allow table columns to exceed the preferred widths of the constituent cells. Specified in [ECMA-376] Part 4, 2.15.3.5 (autofitToFirstFixedWidthCell). MAY<191> be ignored.
            //Z - fUnderlineTabInNumList (1 bit): Specifies whether to underline the tab following numbering when both the numbering and the first character of the numbered paragraph are underlined. Specified in [ECMA-376] Part 4, 2.15.3.56 (underlineTabInNumList). MAY<192> be ignored.
            //a - fHangulWidthLikeWW11 (1 bit): Specifies whether to always use fixed width for Hangul characters. Specified in [ECMA-376] Part 4, 2.15.3.11 (displayHangulFixedWidth). MAY<193> be ignored.
            //b - fSplitPgBreakAndParaMark (1 bit): Specifies whether to always move paragraph marks to the page after a page break. Specified in [ECMA-376] Part 4, 2.15.3.45 (splitPgBreakAndParaMark). MAY<194> be ignored.
            //c - fDontVertAlignCellWithSp (1 bit): Specifies whether to not vertically align cells containing floating objects. Specified in [ECMA-376] Part 4, 2.15.3.23 (doNotVertAlignCellWithSp). MAY<195> be ignored.
            //d - fDontBreakConstrainedForcedTables (1 bit): Specifies whether to not break table rows around floating tables. Specified in [ECMA-376] Part 4, 2.15.3.13 (doNotBreakConstrainedForcedTable) where cantSplit element refers to either sprmTFCantSplit or sprmTFCantSplit90 and tblpPr element refers to any of sprmTDxaAbs, sprmTDyaAbs, sprmTPc, sprmTDyaFromTextBottom, sprmTDyaFromText, sprmTDxaFromTextRight, or sprmTDxaFromText with a nondefault value specified. MAY<196> be ignored.
            //e - fDontVertAlignInTxbx (1 bit): Specifies whether to ignore vertical alignment in text boxes. Specified in [ECMA-376] Part 4, 2.15.3.24 (doNotVertAlignInTxbx). MAY<197> be ignored.
            //f - fWord11KerningPairs (1 bit): Specifies whether to use ANSI kerning pairs from fonts instead of the Unicode kerning pair info. Specified in [ECMA-376] Part 4, 2.15.3.58 (useAnsiKerningPairs). MAY<198> be ignored.
            m_flagsA = BaseWordRecord.ReadUInt32(stream);
            //g - fCachedColBalance (1 bit): Specifies whether to use cached paragraph information for column balancing. Specified in [ECMA-376] Part 4, 2.15.3.8 (cachedColBalance). MAY<199> be ignored.
            m_flagsg = (byte)stream.ReadByte();
            //empty1 (31 bits): Undefined, and MUST be ignored.
            stream.ReadByte();
            BaseWordRecord.ReadUInt16(stream);
            //empty2 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty3 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty4 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty5 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
            //empty6 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.ReadUInt32(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //copts80 (4 bytes): A Copts80 that specifies additional compatibility options. Components of Copts.copts80 MUST be equal to components of Dop95.copts80.
            Copts80.Write(stream);
            //A - fSpLayoutLikeWW8 (1 bit): Specifies whether to emulate Microsoft� Word 97 text wrapping around floating objects. Specified in [ECMA-376] part 4, 2.15.3.41 (shapeLayoutLikeWW8).
            //B - fFtnLayoutLikeWW8 (1 bit): Specifies whether to emulate Microsoft� Word 6.0, Microsoft� Word for Windows� 95, or Word 97 footnote placement. Specified in [ECMA-376] Part 4, 2.15.3.26 (footnoteLayoutLikeWW8).
            //C - fDontUseHTMLParagraphAutoSpacing (1 bit): Specifies whether to use fixed paragraph spacing for paragraphs specifying auto spacing. Specified in [ECMA-376] Part 4, 2.15.3.21 (doNotUseHTMLParagraphAutoSpacing).
            //D - fDontAdjustLineHeightInTable (1 bit): Prevents lines within tables from having their heights adjusted to comply with the document grid. See sprmSDyaLinePitch and [ECMA-376] Part 4, 2.15.3.1 (adjustLineHeightInTable) where the meaning is the opposite of fDontAdjustLineHeightInTable.
            //E - fForgetLastTabAlign (1 bit): Specifies whether to ignore width of the last tab stop when aligning a paragraph if the tab stop is not left aligned. Specified in [ECMA-376] Part 4, 2.15.3.27 (forgetLastTabAlignment) where jc refers to sprmPJc and the tab element refers to either sprmPChgTabs or sprmPChgTabsPapx.
            //F - fUseAutospaceForFullWidthAlpha (1 bit): Specifies whether to emulate Word for Windows 95 full-width character spacing. Specified in [ECMA-376] Part 4, 2.15.3.6 (autoSpaceLikeWord for Windows 95).
            //G - fAlignTablesRowByRow (1 bit): Specifies whether to align table rows independently. Specified in [ECMA-376] Part 4, 2.15.3.2 (alignTablesRowByRow) where the jc element refers to sprmTJc or sprmTJc90.
            //H - fLayoutRawTableWidth (1 bit): Specifies whether to ignore space before tables when deciding if a table should wrap a floating object. Specified in [ECMA-376] Part 4, 2.15.3.29 (layoutRawTableWidth).
            //I - fLayoutTableRowsApart (1 bit): Specifies whether to allow table rows to wrap inline objects independently. Specified in [ECMA-376] Part 4, 2.15.3.30 (layoutTableRowsApart).
            //J - fUseWord97LineBreakingRules (1 bit): Specifies whether to emulate Word 97 East Asian line breaking rules. Specified in [ECMA-376] Part 4, 2.15.3.64 (useWord97LineBreakRules).
            //K - fDontBreakWrappedTables (1 bit): Specifies whether to prevent floating tables from breaking across pages. Specified in [ECMA-376] Part 4, 2.15.3.14 (doNotBreakWrappedTables) where the tblpPr element refers to any of sprmTDxaAbs, sprmTDyaAbs, sprmTPc, sprmTDyaFromTextBottom, sprmTDyaFromText, sprmTDxaFromTextRight, or sprmTDxaFromText with a nondefault value specified.
            //L - fDontSnapToGridInCell (1 bit): Specifies whether to not snap to the document grid in table cells with objects. Specified in [ECMA-376] Part 4, 2.15.3.17 (doNotSnapToGridInCell) where the docGrid element refers to any of sprmSClm, sprmSDyaLinePitch or sprmSDxtCharSpace with a nondefault value specified.
            //M - fDontAllowFieldEndSelect (1 bit): Specifies whether to select an entire field when the first or last character of the field is selected. Specified in [ECMA-376] Part 4, 2.15.3.40 (selectFldWithFirstOrLastChar).
            //N - fApplyBreakingRules (1 bit): Specifies whether to use legacy Ethiopic and Amharic line breaking rules. Specified in [ECMA-376] Part 4, 2.15.3.4 (applyBreakingRules).
            //O - fDontWrapTextWithPunct (1 bit): Specifies whether to prevent hanging punctuation with the character grid. Specified in [ECMA-376] Part 4, 2.15.3.25 (doNotWrapTextWithPunct) where the docGrid element refers to any of sprmSClm, sprmSDyaLinePitch or sprmSDxtCharSpace with a nondefault value specified and the overflowPunct element refers to sprmPFOverflowPunct.
            //P - fDontUseAsianBreakRules (1 bit): Specifies whether to disallow the compressing of compressible characters when using the document grid. Specified in [ECMA-376] Part 4, 2.15.3.20 (doNotUseEastAsianBreakRules) where the docGrid element refers to any of sprmSClm, sprmSDyaLinePitch, or sprmSDxtCharSpace with a nondefault value specified
            //Q - fUseWord2002TableStyleRules (1 bit): Specifies whether to emulate Microsoft� Word 2002 table style rules. Specified in [ECMA-376] Part 4, 2.15.3.63 (useWord2002TableStyleRules).
            //R - fGrowAutoFit (1 bit): Specifies whether to allow tables to autofit into the page margins. Specified in [ECMA-376] Part 4, 2.15.3.28 (growAutofit).
            //S - fUseNormalStyleForList (1 bit): Specifies whether to not automatically apply the list paragraph style to bulleted or numbered text. Specified in [ECMA-376] Part 4, 2.15.3.60 (useNormalStyleForList). MAY<185> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            UseNormalStyleForList = true;
            //T - fDontUseIndentAsNumberingTabStop (1 bit): Specifies whether to ignore the hanging indent when creating a tab stop after numbering. Specified in [ECMA-376] Part 4, 2.15.3.22 (doNotUseIndentAsNumberingTabStop). MAY<186> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            DontUseIndentAsNumberingTabStop = true;
            //U - fFELineBreak11 (1 bit): Specifies whether to use an alternate set of East Asian line breaking rules. Specified in [ECMA-376] Part 4, 2.15.3.57 (useAltKinsokuLineBreakRules). MAY<187> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            FELineBreak11 = true;
            //V - fAllowSpaceOfSameStyleInTable (1 bit): Specifies whether to allow contextual spacing of paragraphs in tables. Specified in [ECMA-376] Part 4, 2.15.3.3 (allowSpaceOfSameStyleInTable) where the contextualSpacing element refers to sprmPFContextualSpacing. MAY<188> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            AllowSpaceOfSameStyleInTable = true;
            //W - fWW11IndentRules (1 bit): Specifies whether to not ignore floating objects when calculating paragraph indentation. Specified in [ECMA-376] Part 4, 2.15.3.18 (doNotSuppressIndentation). MAY<189> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            WW11IndentRules = true;
            //X - fDontAutofitConstrainedTables (1 bit): Specifies whether to not autofit tables such that they fit next to wrapped objects. Specified in [ECMA-376] Part 4, 2.15.3.12 (doNotAutofitConstrainedTables). MAY<190> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            DontAutofitConstrainedTables = true;
            //Y - fAutofitLikeWW11 (1 bit): Specifies whether to allow table columns to exceed the preferred widths of the constituent cells. Specified in [ECMA-376] Part 4, 2.15.3.5 (autofitToFirstFixedWidthCell). MAY<191> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            AutofitLikeWW11 = true;
            //Z - fUnderlineTabInNumList (1 bit): Specifies whether to underline the tab following numbering when both the numbering and the first character of the numbered paragraph are underlined. Specified in [ECMA-376] Part 4, 2.15.3.56 (underlineTabInNumList). MAY<192> be ignored.
            //a - fHangulWidthLikeWW11 (1 bit): Specifies whether to always use fixed width for Hangul characters. Specified in [ECMA-376] Part 4, 2.15.3.11 (displayHangulFixedWidth). MAY<193> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            HangulWidthLikeWW11 = true;
            //b - fSplitPgBreakAndParaMark (1 bit): Specifies whether to always move paragraph marks to the page after a page break. Specified in [ECMA-376] Part 4, 2.15.3.45 (splitPgBreakAndParaMark). MAY<194> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            SplitPgBreakAndParaMark = true;
            //c - fDontVertAlignCellWithSp (1 bit): Specifies whether to not vertically align cells containing floating objects. Specified in [ECMA-376] Part 4, 2.15.3.23 (doNotVertAlignCellWithSp). MAY<195> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            DontVertAlignCellWithSp = true;
            //d - fDontBreakConstrainedForcedTables (1 bit): Specifies whether to not break table rows around floating tables. Specified in [ECMA-376] Part 4, 2.15.3.13 (doNotBreakConstrainedForcedTable) where cantSplit element refers to either sprmTFCantSplit or sprmTFCantSplit90 and tblpPr element refers to any of sprmTDxaAbs, sprmTDyaAbs, sprmTPc, sprmTDyaFromTextBottom, sprmTDyaFromText, sprmTDxaFromTextRight, or sprmTDxaFromText with a nondefault value specified. MAY<196> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            DontBreakConstrainedForcedTables = true;
            //e - fDontVertAlignInTxbx (1 bit): Specifies whether to ignore vertical alignment in text boxes. Specified in [ECMA-376] Part 4, 2.15.3.24 (doNotVertAlignInTxbx). MAY<197> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            DontVertAlignInTxbx = true;
            //f - fWord11KerningPairs (1 bit): Specifies whether to use ANSI kerning pairs from fonts instead of the Unicode kerning pair info. Specified in [ECMA-376] Part 4, 2.15.3.58 (useAnsiKerningPairs). MAY<198> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            Word11KerningPairs = true;
            BaseWordRecord.WriteUInt32(stream, m_flagsA);
            //g - fCachedColBalance (1 bit): Specifies whether to use cached paragraph information for column balancing. Specified in [ECMA-376] Part 4, 2.15.3.8 (cachedColBalance). MAY<199> be ignored.
            //Only supported in Office Word 2007, Word 2010, and Word 2013.
            CachedColBalance = true;
            stream.WriteByte(m_flagsg);
            //empty1 (31 bits): Undefined, and MUST be ignored.
            stream.WriteByte(0);
            BaseWordRecord.WriteUInt16(stream, 0);
            //empty2 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //empty3 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //empty4 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //empty5 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
            //empty6 (4 bytes): Undefined, and MUST be ignored.
            BaseWordRecord.WriteUInt32(stream, 0);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Copts80.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Copts80
    {
        #region Fields
        private ushort m_flags = 16;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the copts60.
        /// </summary>
        /// <value>The copts60.</value>
        internal Copts60 Copts60
        {
            get
            {
                return m_dopBase.Copts60;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [suppress top spacing mac5].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [suppress top spacing mac5]; otherwise, <c>false</c>.
        /// </value>
        internal bool SuppressTopSpacingMac5
        {
            get
            {
                return (m_flags & 0x1) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [trunc dxa expand].
        /// </summary>
        /// <value><c>true</c> if [trunc dxa expand]; otherwise, <c>false</c>.</value>
        internal bool TruncDxaExpand
        {
            get
            {
                return ((m_flags & 0x2) >> 1) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [print body before HDR].
        /// </summary>
        /// <value><c>true</c> if [print body before HDR]; otherwise, <c>false</c>.</value>
        internal bool PrintBodyBeforeHdr
        {
            get
            {
                return ((m_flags & 0x4) >> 2) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFFB) | ((value ? 1 : 0) << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [no ext leading].
        /// </summary>
        /// <value><c>true</c> if [no ext leading]; otherwise, <c>false</c>.</value>
        internal bool NoExtLeading
        {
            get
            {
                return ((m_flags & 0x8) >> 3) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [dont make space for UL].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont make space for UL]; otherwise, <c>false</c>.
        /// </value>
        internal bool DontMakeSpaceForUL
        {
            get
            {
                return ((m_flags & 0x10) >> 4) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [MW small caps].
        /// </summary>
        /// <value><c>true</c> if [MW small caps]; otherwise, <c>false</c>.</value>
        internal bool MWSmallCaps
        {
            get
            {
                return ((m_flags & 0x20) >> 5) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFDF) | ((value ? 1 : 0) << 5));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [F2PT ext leading only].
        /// </summary>
        /// <value><c>true</c> if [F2PT ext leading only]; otherwise, <c>false</c>.</value>
        internal bool F2ptExtLeadingOnly
        {
            get
            {
                return ((m_flags & 0x40) >> 6) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [trunc font height].
        /// </summary>
        /// <value><c>true</c> if [trunc font height]; otherwise, <c>false</c>.</value>
        internal bool TruncFontHeight
        {
            get
            {
                return ((m_flags & 0x80) >> 7) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFF7F) | ((value ? 1 : 0) << 7));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [sub on size].
        /// </summary>
        /// <value><c>true</c> if [sub on size]; otherwise, <c>false</c>.</value>
        internal bool SubOnSize
        {
            get
            {
                return ((m_flags & 0x100) >> 8) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFEFF) | ((value ? 1 : 0) << 8));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [line wrap like word6].
        /// </summary>
        /// <value><c>true</c> if [line wrap like word6]; otherwise, <c>false</c>.</value>
        internal bool LineWrapLikeWord6
        {
            get
            {
                return ((m_flags & 0x200) >> 9) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFDFF) | ((value ? 1 : 0) << 9));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [W w6 border rules].
        /// </summary>
        /// <value><c>true</c> if [W w6 border rules]; otherwise, <c>false</c>.</value>
        internal bool WW6BorderRules
        {
            get
            {
                return ((m_flags & 0x400) >> 10) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [exact on top].
        /// </summary>
        /// <value><c>true</c> if [exact on top]; otherwise, <c>false</c>.</value>
        internal bool ExactOnTop
        {
            get
            {
                return ((m_flags & 0x800) >> 11) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xF7FF) | ((value ? 1 : 0) << 11));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [extra after].
        /// </summary>
        /// <value><c>true</c> if [extra after]; otherwise, <c>false</c>.</value>
        internal bool ExtraAfter
        {
            get
            {
                return ((m_flags & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [WP space].
        /// </summary>
        /// <value><c>true</c> if [WP space]; otherwise, <c>false</c>.</value>
        internal bool WPSpace
        {
            get
            {
                return ((m_flags & 0x2000) >> 13) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xDFFF) | ((value ? 1 : 0) << 13));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [WP just].
        /// </summary>
        /// <value><c>true</c> if [WP just]; otherwise, <c>false</c>.</value>
        internal bool WPJust
        {
            get
            {
                return ((m_flags & 0x4000) >> 14) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xBFFF) | ((value ? 1 : 0) << 14));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [print met].
        /// </summary>
        /// <value><c>true</c> if [print met]; otherwise, <c>false</c>.</value>
        internal bool PrintMet
        {
            get
            {
                return ((m_flags & 0x8000) >> 15) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0x7FFF) | ((value ? 1 : 0) << 15));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Copts80"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal Copts80(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //copts60 (2 bytes): A Copts60 that specifies additional compatibility options. Copts80.copts60 components MUST be equal to DopBase.copts60.
            Copts60.Parse(stream);
            //A - fSuppressTopSpacingMac5 (1 bit): Specifies whether the minimum line height for the first line on the page is ignored as specified in [ECMA-376] Part 4, Section 2.15.3.48 suppressSpacingAtTopOfPage, where a spacing element with a lineRule attribute value of atLeast refers to sprmPDyaLine with a LSPD.fMultLinespace of 0 and LSPD.dyaline greater than 0.
            //B - fTruncDxaExpand (1 bit): Specifies whether text is expanded or condensed by whole points as specified in [ECMA-376] Part 4, Section 2.15.3.44 spacingInWholePoints, where spacing refers to sprmPDyaBefore and sprmPDyaAfter.
            //C - fPrintBodyBeforeHdr (1 bit): Specifies whether body text is printed before header and footer contents as specified in [ECMA-376] Part 4, Section 2.15.3.38 printBodyTextBeforeHeader.
            //D - fNoExtLeading (1 bit): Specifies whether leading is not added between lines of text as specified in [ECMA-376] Part 4, Section 2.15.3.35 noLeading.
            //E - fDontMakeSpaceForUL (1 bit): Specifies whether additional space is not added below the baseline for underlined East Asian characters as specified in [ECMA-376] Part 4, Section 2.15.3.43 spaceForUL, where u is sprmCKul and textAlignment with val of baseline is sprmPWAlignFont with a value of 2 and the overall meaning is the opposite of fDontMakeSpaceForUL.
            //F - fMWSmallCaps (1 bit): Specifies whether Word 5.x for the Macintosh small caps formatting is to be used as specified in [ECMA-376] Part 4, Section 2.15.3.32 mwSmallCaps.
            //G - f2ptExtLeadingOnly (1 bit): Specifies whether line spacing emulates WordPerfect 5.x line spacing as specified in [ECMA-376] Part 4, Section 2.15.3.51 suppressTopSpacingWP.
            //H - fTruncFontHeight (1 bit): Specifies whether font height calculation emulates WordPerfect 6.x font height calculation as specified in [ECMA-376] Part 4, Section 2.15.3.53 truncateFontHeightsLikeWP6.
            //I - fSubOnSize (1 bit): Specifies whether the priority of font size is increased during font substitution as specified in [ECMA-376] Part 4, Section 2.15.3.46 subFontBySize.
            //J - fLineWrapLikeWord6 (1 bit): Specifies whether line wrapping emulates Microsoft� Word 6.0 line wrapping for East Asian characters as specified in [ECMA-376] Part 4, Section 2.15.3.31 lineWrapLikeWord6.
            //K - fWW6BorderRules (1 bit): Specifies whether the paragraph borders next to frames are not suppressed as specified in [ECMA-376] Part 4, Section 2.15.3.19 doNotSuppressParagraphBorders.
            //L - fExactOnTop (1 bit): Specifies whether content on lines with exact line height is not to be centered as specified in [ECMA-376] Part 4, Section 2.15.3.34 noExtraLineSpacing, where exact line height using the spacing element refers to sprmPDyaLine with LSPD.fMultLinespace of 0 and LSPD.dyaline is less than 0.
            //M - fExtraAfter (1 bit): Specifies whether the exact line height for the last line on a page is ignored as specified in [ECMA-376] Part 4, Section 2.15.3.47 suppressBottomSpacing, where exact line height has using the spacing element refers to sprmPDyaLine with LSPD.fMultLinespace of 0 and LSPD.dyaline is less than 0.
            //N - fWPSpace (1 bit): Specifies whether the width of a space emulates WordPerfect 5.x space width as specified in [ECMA-376] Part 4, Section 2.15.3.66 wpSpaceWidth. 
            //O - fWPJust (1 bit): Specifies whether paragraph justification emulates WordPerfect 6.x paragraph justification as specified in [ECMA-376] Part 4, Section 2.15.3.65 wpJustification, where the val attribute value of both on the jc element refers to sprmPJc with a value of 3.
            //P - fPrintMet (1 bit): Specifies whether printer metrics are used to display documents as specified in [ECMA-376] Part 4, Section 2.15.3.61 usePrinterMetrics.
            m_flags = BaseWordRecord.ReadUInt16(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //copts60 (2 bytes): A Copts60 that specifies additional compatibility options. Copts80.copts60 components MUST be equal to DopBase.copts60.
            Copts60.Write(stream);
            //A - fSuppressTopSpacingMac5 (1 bit): Specifies whether the minimum line height for the first line on the page is ignored as specified in [ECMA-376] Part 4, Section 2.15.3.48 suppressSpacingAtTopOfPage, where a spacing element with a lineRule attribute value of atLeast refers to sprmPDyaLine with a LSPD.fMultLinespace of 0 and LSPD.dyaline greater than 0.
            //B - fTruncDxaExpand (1 bit): Specifies whether text is expanded or condensed by whole points as specified in [ECMA-376] Part 4, Section 2.15.3.44 spacingInWholePoints, where spacing refers to sprmPDyaBefore and sprmPDyaAfter.
            //C - fPrintBodyBeforeHdr (1 bit): Specifies whether body text is printed before header and footer contents as specified in [ECMA-376] Part 4, Section 2.15.3.38 printBodyTextBeforeHeader.
            //D - fNoExtLeading (1 bit): Specifies whether leading is not added between lines of text as specified in [ECMA-376] Part 4, Section 2.15.3.35 noLeading.
            //E - fDontMakeSpaceForUL (1 bit): Specifies whether additional space is not added below the baseline for underlined East Asian characters as specified in [ECMA-376] Part 4, Section 2.15.3.43 spaceForUL, where u is sprmCKul and textAlignment with val of baseline is sprmPWAlignFont with a value of 2 and the overall meaning is the opposite of fDontMakeSpaceForUL.
            //F - fMWSmallCaps (1 bit): Specifies whether Word 5.x for the Macintosh small caps formatting is to be used as specified in [ECMA-376] Part 4, Section 2.15.3.32 mwSmallCaps.
            //G - f2ptExtLeadingOnly (1 bit): Specifies whether line spacing emulates WordPerfect 5.x line spacing as specified in [ECMA-376] Part 4, Section 2.15.3.51 suppressTopSpacingWP.
            //H - fTruncFontHeight (1 bit): Specifies whether font height calculation emulates WordPerfect 6.x font height calculation as specified in [ECMA-376] Part 4, Section 2.15.3.53 truncateFontHeightsLikeWP6.
            //I - fSubOnSize (1 bit): Specifies whether the priority of font size is increased during font substitution as specified in [ECMA-376] Part 4, Section 2.15.3.46 subFontBySize.
            //J - fLineWrapLikeWord6 (1 bit): Specifies whether line wrapping emulates Microsoft� Word 6.0 line wrapping for East Asian characters as specified in [ECMA-376] Part 4, Section 2.15.3.31 lineWrapLikeWord6.
            //K - fWW6BorderRules (1 bit): Specifies whether the paragraph borders next to frames are not suppressed as specified in [ECMA-376] Part 4, Section 2.15.3.19 doNotSuppressParagraphBorders.
            //L - fExactOnTop (1 bit): Specifies whether content on lines with exact line height is not to be centered as specified in [ECMA-376] Part 4, Section 2.15.3.34 noExtraLineSpacing, where exact line height using the spacing element refers to sprmPDyaLine with LSPD.fMultLinespace of 0 and LSPD.dyaline is less than 0.
            //M - fExtraAfter (1 bit): Specifies whether the exact line height for the last line on a page is ignored as specified in [ECMA-376] Part 4, Section 2.15.3.47 suppressBottomSpacing, where exact line height has using the spacing element refers to sprmPDyaLine with LSPD.fMultLinespace of 0 and LSPD.dyaline is less than 0.
            //N - fWPSpace (1 bit): Specifies whether the width of a space emulates WordPerfect 5.x space width as specified in [ECMA-376] Part 4, Section 2.15.3.66 wpSpaceWidth. 
            //O - fWPJust (1 bit): Specifies whether paragraph justification emulates WordPerfect 6.x paragraph justification as specified in [ECMA-376] Part 4, Section 2.15.3.65 wpJustification, where the val attribute value of both on the jc element refers to sprmPJc with a value of 3.
            //P - fPrintMet (1 bit): Specifies whether printer metrics are used to display documents as specified in [ECMA-376] Part 4, Section 2.15.3.61 usePrinterMetrics.
            BaseWordRecord.WriteUInt16(stream, m_flags);
        }
        #endregion
    }
    /// <summary>
    /// Specifies the structure of Copts60.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class Copts60
    {
        #region Fields
        private ushort m_flags = 61440;
        private DOPDescriptor m_dopBase;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [no tab for ind].
        /// </summary>
        /// <value><c>true</c> if [no tab for ind]; otherwise, <c>false</c>.</value>
        internal bool NoTabForInd
        {
            get
            {
                return (m_flags & 0x1) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFFE) | (value ? 1 : 0));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [no space raise lower].
        /// </summary>
        /// <value><c>true</c> if [no space raise lower]; otherwise, <c>false</c>.</value>
        internal bool NoSpaceRaiseLower
        {
            get
            {
                return ((m_flags & 0x2) >> 1) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFFD) | ((value ? 1 : 0) << 1));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [suppress sp bf after pg BRK].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [suppress sp bf after pg BRK]; otherwise, <c>false</c>.
        /// </value>
        internal bool SuppressSpBfAfterPgBrk
        {
            get
            {
                return ((m_flags & 0x4) >> 2) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFFB) | ((value ? 1 : 0) << 2));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [wrap trail spaces].
        /// </summary>
        /// <value><c>true</c> if [wrap trail spaces]; otherwise, <c>false</c>.</value>
        internal bool WrapTrailSpaces
        {
            get
            {
                return ((m_flags & 0x8) >> 3) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFF7) | ((value ? 1 : 0) << 3));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [map print text color].
        /// </summary>
        /// <value><c>true</c> if [map print text color]; otherwise, <c>false</c>.</value>
        internal bool MapPrintTextColor
        {
            get
            {
                return ((m_flags & 0x10) >> 4) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFEF) | ((value ? 1 : 0) << 4));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [no column balance].
        /// </summary>
        /// <value><c>true</c> if [no column balance]; otherwise, <c>false</c>.</value>
        internal bool NoColumnBalance
        {
            get
            {
                return ((m_flags & 0x20) >> 5) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFDF) | ((value ? 1 : 0) << 5));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [conv mail merge esc].
        /// </summary>
        /// <value><c>true</c> if [conv mail merge esc]; otherwise, <c>false</c>.</value>
        internal bool ConvMailMergeEsc
        {
            get
            {
                return ((m_flags & 0x40) >> 6) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFFBF) | ((value ? 1 : 0) << 6));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [suppress top spacing].
        /// </summary>
        /// <value><c>true</c> if [suppress top spacing]; otherwise, <c>false</c>.</value>
        internal bool SuppressTopSpacing
        {
            get
            {
                return ((m_flags & 0x80) >> 7) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFF7F) | ((value ? 1 : 0) << 7));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [orig word table rules].
        /// </summary>
        /// <value><c>true</c> if [orig word table rules]; otherwise, <c>false</c>.</value>
        internal bool OrigWordTableRules
        {
            get
            {
                return ((m_flags & 0x100) >> 8) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFEFF) | ((value ? 1 : 0) << 8));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show breaks in frames].
        /// </summary>
        /// <value><c>true</c> if [show breaks in frames]; otherwise, <c>false</c>.</value>
        internal bool ShowBreaksInFrames
        {
            get
            {
                return ((m_flags & 0x400) >> 10) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xFBFF) | ((value ? 1 : 0) << 10));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [swap borders facing PGS].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [swap borders facing PGS]; otherwise, <c>false</c>.
        /// </value>
        internal bool SwapBordersFacingPgs
        {
            get
            {
                return ((m_flags & 0x800) >> 11) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xF7FF) | ((value ? 1 : 0) << 11));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [leave backslash alone].
        /// </summary>
        /// <value><c>true</c> if [leave backslash alone]; otherwise, <c>false</c>.</value>
        internal bool LeaveBackslashAlone
        {
            get
            {
                return ((m_flags & 0x1000) >> 12) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xEFFF) | ((value ? 1 : 0) << 12));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [exp sh RTN].
        /// </summary>
        /// <value><c>true</c> if [exp sh RTN]; otherwise, <c>false</c>.</value>
        internal bool ExpShRtn
        {
            get
            {
                return ((m_flags & 0x2000) >> 13) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xDFFF) | ((value ? 1 : 0) << 13));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [DNT UL TRL SPC].
        /// </summary>
        /// <value><c>true</c> if [DNT UL TRL SPC]; otherwise, <c>false</c>.</value>
        internal bool DntULTrlSpc
        {
            get
            {
                return ((m_flags & 0x4000) >> 14) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0xBFFF) | ((value ? 1 : 0) << 14));
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [DNT BLN sb db wid].
        /// </summary>
        /// <value><c>true</c> if [DNT BLN sb db wid]; otherwise, <c>false</c>.</value>
        internal bool DntBlnSbDbWid
        {
            get
            {
                return ((m_flags & 0x8000) >> 15) != 0;
            }
            set
            {
                m_flags = (ushort)((m_flags & 0x7FFF) | ((value ? 1 : 0) << 15));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Copts60"/> class.
        /// </summary>
        /// <param name="dopBase">The dop base.</param>
        internal Copts60(DOPDescriptor dopBase)
        {
            m_dopBase = dopBase;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Parse(Stream stream)
        {
            //copts60 (2 bytes): A Copts60 that specifies additional compatibility options. Copts80.copts60 components MUST be equal to DopBase.copts60.
            m_flags = BaseWordRecord.ReadUInt16(stream);
        }
        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Write(Stream stream)
        {
            //copts60 (2 bytes): A Copts60 that specifies additional compatibility options. Copts80.copts60 components MUST be equal to DopBase.copts60.
            BaseWordRecord.WriteUInt16(stream, m_flags);
        }
        #endregion
    }
}

