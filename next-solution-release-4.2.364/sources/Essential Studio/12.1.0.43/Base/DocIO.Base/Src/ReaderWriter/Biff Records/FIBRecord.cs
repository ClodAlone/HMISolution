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

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for FIBRecord.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class FIBRecord : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_DOT = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_GLSY = 1;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_COMPLEX = 2;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_HAS_PICTURE = 3;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_ENCRYPTED = 8;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_WHICH_TABLE = 9;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_READONLY_RECOMMENDED = 10;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_WRITE_RESERVATION = 11;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_EXTENDED_CHAR = 12;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_LOAD_OVERRIDE = 13;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_FAREAST = 14;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_CRYPTO = 15;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_QUICKSAVES = 0x00F0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_QUICKSAVES_START = 4;

        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_MAC = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_EMPTYSPECIAL = 1;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_LOADOVERRIDEPAGE = 2;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_FUTURESAVEDUNDO = 3;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_WORD97SAVED = 4;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_SPARE0 = 0xE0; // 0xFE - in other sources
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_SPARE0_START = 5;
        #endregion

        #region Class members
        /// <summary>
        /// Underlying FIB structure.
        /// </summary>
        private FIBStructure m_fibStructure = new FIBStructure();
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal FIBRecord()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal FIBRecord(byte[] arrData)
            : base(arrData)
        {
        }
        /// <summary>
        /// Reads FIB record from the stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal FIBRecord(Stream stream)
        {
            Parse(stream);

        }
        internal FIBRecord(Stream stream, bool isDecrepted)
        {
            if (isDecrepted)
                ParseDecrypted(stream);
            else
                Parse(stream);
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Returns underlying class or structure.
        /// </summary>
        protected override DataStructure UnderlyingStructure
        {
            get
            {
                return m_fibStructure;
            }
        }

        #endregion

        #region Class Publiic Properties
        #region Header
        /// <summary>
        /// magic number
        /// </summary>
        internal ushort wIdent
        {
            get
            {
                return m_fibStructure.header.wIdent;
            }
            set
            {
                m_fibStructure.header.wIdent = value;
            }
        }
        /// <summary>
        /// FIB version written. This will be >= 101 for all Word 6.0 for Windows and after documents.
        /// </summary>
        internal ushort nFib
        {
            get
            {
                return m_fibStructure.header.nFib;
            }
            set
            {
                m_fibStructure.header.nFib = value;
            }
        }
        /// <summary>
        /// product version written by
        /// </summary>
        internal ushort nProduct
        {
            get
            {
                return m_fibStructure.header.nProduct;
            }
            set
            {
                m_fibStructure.header.nProduct = value;
            }
        }
        /// <summary>
        /// language stamp -- localized version
        /// </summary>
        internal ushort lid
        {
            get
            {
                return m_fibStructure.header.lid;
            }
            set
            {
                m_fibStructure.header.lid = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal short pnNext
        {
            get
            {
                return m_fibStructure.header.pnNext;
            }
            set
            {
                m_fibStructure.header.pnNext = value;
            }
        }


        #region BitField 10
        //[ FieldOffset( 10 ) ]
        /// <summary>
        /// Set if this document is a template
        /// </summary>
        internal bool IsDot
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_DOT);// 0x0001;
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_DOT, value);
            }
        }
        /// <summary>
        /// Set if this document is a glossary
        /// </summary>
        internal bool IsGlosary
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_GLSY);// 0x0002
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_GLSY, value);
            }
        }
        /// <summary>
        /// when 1, file is in complex, fast-saved format.
        /// </summary>
        internal bool IsComplex
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_COMPLEX); //0x0004
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_COMPLEX, value);
            }
        }
        /// <summary>
        /// set if file contains 1 or more pictures
        /// </summary>
        internal bool HasPicture
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_HAS_PICTURE);//0x0008
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_HAS_PICTURE, value);
            }
        }
        /// <summary>
        /// count of times file was quicksaved
        /// </summary>
        internal ushort QuickSavesCount
        {
            get
            {
                return (ushort)GetBitsByMask(m_fibStructure.header.m_usOptions,
                  DEF_BIT_QUICKSAVES, DEF_BIT_QUICKSAVES_START); //0x00F0
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBitsByMask(
                  m_fibStructure.header.m_usOptions, DEF_BIT_QUICKSAVES,
                  value << DEF_BIT_QUICKSAVES_START);
            }
        }
        /// <summary>
        /// Set if file is encrypted
        /// </summary>
        internal bool IsEncrypted
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_ENCRYPTED);// 0x0100
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_ENCRYPTED, value);
            }
        }
        /// <summary>
        /// When 0, this fib refers to the table stream named "0Table", when 1, this fib refers to the table stream named "1Table". Normally, a file will have only one table stream, but under unusual circumstances a file may have table streams with both names. In that case, this flag must be used to decide which table stream is valid.
        /// </summary>
        internal bool IsUse1Table//fWhichTblStm
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_WHICH_TABLE);//0x0200
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_WHICH_TABLE, value);
            }
        }
        /// <summary>
        /// Set when user has recommended that file be read read-only
        /// </summary>
        internal bool IsReadOnlyRecommended
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_READONLY_RECOMMENDED);// 0x0400
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_READONLY_RECOMMENDED, value);
            }
        }
        /// <summary>
        /// Set when file owner has made the file write reserved
        /// </summary>
        internal bool IsWriteReservation
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_WRITE_RESERVATION);// 0x0800
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_WRITE_RESERVATION, value);
            }
        }
        /// <summary>
        /// Set when using extended character set in file
        /// </summary>
        internal bool IsExtendedChar
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions,
                  DEF_BIT_EXTENDED_CHAR);// 0x1000
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_EXTENDED_CHAR, value);
            }
        }
        /// <summary>
        /// REVIEW
        /// </summary>
        internal bool IsLoadOverride
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_LOAD_OVERRIDE);// 0x2000
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_LOAD_OVERRIDE, value);
            }
        }
        /// <summary>
        /// REVIEW
        /// </summary>
        internal bool IsFarEast
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions,
                  DEF_BIT_FAREAST);// 0x4000
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_FAREAST, value);
            }
        }
        /// <summary>
        /// REVIEW
        /// </summary>
        internal bool IsCrypto
        {
            get
            {
                return GetBit(m_fibStructure.header.m_usOptions, DEF_BIT_CRYPTO);// 0x8000
            }
            set
            {
                m_fibStructure.header.m_usOptions = (ushort)SetBit(
                  m_fibStructure.header.m_usOptions, DEF_BIT_CRYPTO, value);
            }
        }
        #endregion

        /// <summary>
        /// This file format it compatible with readers that understand nFib at or above this value.
        /// </summary>
        internal ushort nFibBack
        {
            get
            {
                return m_fibStructure.header.nFibBack;
            }
            set
            {
                m_fibStructure.header.nFibBack = value;
            }
        }
        /// <summary>
        /// File encrypted key, only valid if fEncrypted.
        /// </summary>
        internal int lKey
        {
            get
            {
                return m_fibStructure.header.lKey;
            }
            set
            {
                m_fibStructure.header.lKey = value;
            }
        }
        /// <summary>
        /// environment in which file was created
        /// </summary>
        internal byte envr
        {
            get
            {
                return m_fibStructure.header.envr;
            }
            set
            {
                m_fibStructure.header.envr = value;
            }
        }

        #region BitField 19
        //[ FieldOffset( 19 ) ]
        /// <summary>
        /// when 1, this file was last saved in the Mac environment
        /// </summary>
        internal bool IsMac
        {
            get
            {
                return GetBit(m_fibStructure.header.m_btOptions2, DEF_BIT_MAC);// 0x01
            }
            set
            {
                m_fibStructure.header.m_btOptions2 = (byte)SetBit(
                  m_fibStructure.header.m_btOptions2, DEF_BIT_MAC, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsEmptySpecial
        {
            get
            {
                return GetBit(m_fibStructure.header.m_btOptions2, DEF_BIT_EMPTYSPECIAL);// 0x02
            }
            set
            {
                m_fibStructure.header.m_btOptions2 = (byte)SetBit(
                  m_fibStructure.header.m_btOptions2, DEF_BIT_EMPTYSPECIAL, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool fLoadOverridePage
        {
            get
            {
                return GetBit(m_fibStructure.header.m_btOptions2, DEF_BIT_LOADOVERRIDEPAGE);// 0x04
            }
            set
            {
                m_fibStructure.header.m_btOptions2 = (byte)SetBit(
                  m_fibStructure.header.m_btOptions2, DEF_BIT_LOADOVERRIDEPAGE, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsFutureSavedUndo
        {
            get
            {
                return GetBit(m_fibStructure.header.m_btOptions2, DEF_BIT_FUTURESAVEDUNDO);// 0x08
            }
            set
            {
                m_fibStructure.header.m_btOptions2 = (byte)SetBit(
                  m_fibStructure.header.m_btOptions2, DEF_BIT_FUTURESAVEDUNDO, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsWord97Saved
        {
            get
            {
                return GetBit(m_fibStructure.header.m_btOptions2, DEF_BIT_WORD97SAVED);// 0x10
            }
            set
            {
                m_fibStructure.header.m_btOptions2 = (byte)SetBit(
                  m_fibStructure.header.m_btOptions2, DEF_BIT_WORD97SAVED, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte Spare0
        {
            get
            {
                return (byte)GetBitsByMask(m_fibStructure.header.m_btOptions2,
                  DEF_BIT_SPARE0, DEF_BIT_SPARE0_START); // 0xFE
            }
            set
            {
                m_fibStructure.header.m_btOptions2 = (byte)SetBitsByMask(
                  m_fibStructure.header.m_btOptions2, DEF_BIT_SPARE0,
                  value << DEF_BIT_SPARE0_START);
            }
        }
        #endregion

        /// <summary>
        /// Default extended character set id for text in document stream. (overridden by chp.chse)
        /// </summary>
        internal ushort chs
        {
            get
            {
                return m_fibStructure.header.chs;
            }
            set
            {
                m_fibStructure.header.chs = value;
            }
        }
        /// <summary>
        /// Default extended character set id for text in internal data structures
        /// </summary>
        internal ushort chsTables
        {
            get
            {
                return m_fibStructure.header.chsTables;
            }
            set
            {
                m_fibStructure.header.chsTables = value;
            }
        }
        /// <summary>
        /// file offset of first character of text. In non-complex files a CP can be transformed into an FC by the following transformation:
        /// </summary>
        internal uint fcMin
        {
            get
            {
                return m_fibStructure.header.fcMin;
            }
            set
            {
                m_fibStructure.header.fcMin = value;
            }
        }
        /// <summary>
        /// file offset of last character of text in document text stream + 1
        /// </summary>
        internal uint fcMac
        {
            get
            {
                return m_fibStructure.header.fcMac;
            }
            set
            {
                m_fibStructure.header.fcMac = value;
            }
        }
        #endregion

        #region Array of shorts
        /// <summary>
        /// Count of fields in the array of "shorts"
        /// </summary>
        internal ushort csw
        {
            get
            {
                return m_fibStructure.header.csw;
            }
            set
            {
                m_fibStructure.header.csw = value;
                m_fibStructure.arrShorts.Length = value;
            }
        }

        //    /// <summary>
        //    /// Beginning of the array of shorts
        //    /// </summary>
        //    internal ushort rgsw
        //    {
        //      get
        //      { 
        //        return m_fibStructure.rgsw;
        //      }
        //      set
        //      {
        //        m_fibStructure.rgsw = value;
        //      }
        //    }
        /// <summary>
        /// unique number Identifying the File's creator 0x6A62 is the creator ID for Word and is reserved. Other creators should choose a different value.
        /// </summary>
        internal ushort wMagicCreated
        {
            get
            {
                return m_fibStructure.arrShorts.wMagicCreated;
            }
            set
            {
                m_fibStructure.arrShorts.wMagicCreated = value;
            }
        }
        /// <summary>
        /// identifies the File's last modifier
        /// </summary>
        internal ushort wMagicRevised
        {
            get
            {
                return m_fibStructure.arrShorts.wMagicRevised;
            }
            set
            {
                m_fibStructure.arrShorts.wMagicRevised = value;
            }
        }
        /// <summary>
        /// private data
        /// </summary>
        internal ushort wMagicCreatedPrivate
        {
            get
            {
                return m_fibStructure.arrShorts.wMagicCreatedPrivate;
            }
            set
            {
                m_fibStructure.arrShorts.wMagicCreatedPrivate = value;
            }
        }
        /// <summary>
        /// private data
        /// </summary>
        internal ushort wMagicRevisedPrivate
        {
            get
            {
                return m_fibStructure.arrShorts.wMagicRevisedPrivate;
            }
            set
            {
                m_fibStructure.arrShorts.wMagicRevisedPrivate = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short pnFbpChpFirst_W6
        {
            get
            {
                return m_fibStructure.arrShorts.pnFbpChpFirst_W6;
            }
            set
            {
                m_fibStructure.arrShorts.pnFbpChpFirst_W6 = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short pnChpFirst_W6
        {
            get
            {
                return m_fibStructure.arrShorts.pnChpFirst_W6;
            }
            set
            {
                m_fibStructure.arrShorts.pnChpFirst_W6 = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short cpnBteChp_W6
        {
            get
            {
                return m_fibStructure.arrShorts.cpnBteChp_W6;
            }
            set
            {
                m_fibStructure.arrShorts.cpnBteChp_W6 = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short pnFbpPapFirst_W6
        {
            get
            {
                return m_fibStructure.arrShorts.pnFbpPapFirst_W6;
            }
            set
            {
                m_fibStructure.arrShorts.pnFbpPapFirst_W6 = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short pnPapFirst_W6
        {
            get
            {
                return m_fibStructure.arrShorts.pnPapFirst_W6;
            }
            set
            {
                m_fibStructure.arrShorts.pnPapFirst_W6 = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short cpnBtePap_W6
        {
            get
            {
                return m_fibStructure.arrShorts.cpnBtePap_W6;
            }
            set
            {
                m_fibStructure.arrShorts.cpnBtePap_W6 = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short pnFbpLvcFirst_W6
        {
            get
            {
                return m_fibStructure.arrShorts.pnFbpLvcFirst_W6;
            }
            set
            {
                m_fibStructure.arrShorts.pnFbpLvcFirst_W6 = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short pnLvcFirst_W6
        {
            get
            {
                return m_fibStructure.arrShorts.pnLvcFirst_W6;
            }
            set
            {
                m_fibStructure.arrShorts.pnLvcFirst_W6 = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal short cpnBteLvc_W6
        {
            get
            {
                return m_fibStructure.arrShorts.cpnBteLvc_W6;
            }
            set
            {
                m_fibStructure.arrShorts.cpnBteLvc_W6 = value;
            }
        }
        /// <summary>
        /// Language id if document was written by Far East version of Word (i.e. FIB.fFarEast is on)
        /// </summary>
        internal short lidFE
        {
            get
            {
                return m_fibStructure.arrShorts.lidFE;
            }
            set
            {
                m_fibStructure.arrShorts.lidFE = value;
            }
        }
        #endregion

        #region Array of longs
        /// <summary>
        /// Number of fields in the array of longs
        /// </summary>
        internal ushort clw
        {
            get
            {
                return m_fibStructure.clw;
            }
            set
            {
                m_fibStructure.clw = value;
                m_fibStructure.arrLongs.Length = value;
            }
        }
        //    /// <summary>
        //    /// Beginning of the array of longs
        //    /// </summary>
        //    internal  rglw
        //    {
        //    get
        //    { 
        //    return m_fibStructure.rglw;
        //  }
        //  set
        //{
        //  m_fibStructure.rglw = value;
        //}
        //}
        /// <summary>
        /// file offset of last byte written to file + 1.
        /// </summary>
        internal int cbMac
        {
            get
            {
                return m_fibStructure.arrLongs.cbMac;
            }
            set
            {
                m_fibStructure.arrLongs.cbMac = value;
            }
        }
        /// <summary>
        /// contains the build date of the creator. 10695 indicates the creator program was compiled on Jan 6, 1995
        /// </summary>
        internal int lProductCreated
        {
            get
            {
                return m_fibStructure.arrLongs.lProductCreated;
            }
            set
            {
                m_fibStructure.arrLongs.lProductCreated = value;
            }
        }
        /// <summary>
        /// contains the build date of the File's last modifier
        /// </summary>
        internal int lProductRevised
        {
            get
            {
                return m_fibStructure.arrLongs.lProductRevised;
            }
            set
            {
                m_fibStructure.arrLongs.lProductRevised = value;
            }
        }
        /// <summary>
        /// length of main document text stream
        /// </summary>
        internal int ccpText
        {
            get
            {
                return m_fibStructure.arrLongs.ccpText;
            }
            set
            {
                m_fibStructure.arrLongs.ccpText = value;
            }
        }
        /// <summary>
        /// length of footnote subdocument text stream
        /// </summary>
        internal int ccpFtn
        {
            get
            {
                return m_fibStructure.arrLongs.ccpFtn;
            }
            set
            {
                m_fibStructure.arrLongs.ccpFtn = value;
            }
        }
        /// <summary>
        /// length of header subdocument text stream
        /// </summary>
        internal int ccpHdr
        {
            get
            {
                return m_fibStructure.arrLongs.ccpHdr;
            }
            set
            {
                m_fibStructure.arrLongs.ccpHdr = value;
            }
        }
        /// <summary>
        /// length of macro subdocument text stream, which should now always be 0.
        /// </summary>
        internal int ccpMcr
        {
            get
            {
                return m_fibStructure.arrLongs.ccpMcr;
            }
            set
            {
                m_fibStructure.arrLongs.ccpMcr = value;
            }
        }
        /// <summary>
        /// length of annotation subdocument text stream
        /// </summary>
        internal int ccpAtn
        {
            get
            {
                return m_fibStructure.arrLongs.ccpAtn;
            }
            set
            {
                m_fibStructure.arrLongs.ccpAtn = value;
            }
        }
        /// <summary>
        /// length of endnote subdocument text stream
        /// </summary>
        internal int ccpEdn
        {
            get
            {
                return m_fibStructure.arrLongs.ccpEdn;
            }
            set
            {
                m_fibStructure.arrLongs.ccpEdn = value;
            }
        }
        /// <summary>
        /// length of textbox subdocument text stream
        /// </summary>
        internal int ccpTxbx
        {
            get
            {
                return m_fibStructure.arrLongs.ccpTxbx;
            }
            set
            {
                m_fibStructure.arrLongs.ccpTxbx = value;
            }
        }
        /// <summary>
        /// length of header textbox subdocument text stream.
        /// </summary>
        internal int ccpHdrTxbx
        {
            get
            {
                return m_fibStructure.arrLongs.ccpHdrTxbx;
            }
            set
            {
                m_fibStructure.arrLongs.ccpHdrTxbx = value;
            }
        }
        /// <summary>
        /// when there was insufficient memory for Word to expand the plcfbte at save time, the plcfbte is written to the file in a linked list of 512-byte pieces starting with this pn
        /// </summary>
        internal int pnFbpChpFirst
        {
            get
            {
                return m_fibStructure.arrLongs.pnFbpChpFirst;
            }
            set
            {
                m_fibStructure.arrLongs.pnFbpChpFirst = value;
            }
        }
        /// <summary>
        /// the page number of the lowest numbered page in the document that records CHPX FKP information
        /// </summary>
        internal int pnChpFirst
        {
            get
            {
                return m_fibStructure.arrLongs.pnChpFirst;
            }
            set
            {
                m_fibStructure.arrLongs.pnChpFirst = value;
            }
        }
        /// <summary>
        /// count of CHPX FKPs recorded in file. In non-complex files if the number of entries in the plcfbteChpx is less than this, the plcfbteChpx is incomplete.
        /// </summary>
        internal int cpnBteChp
        {
            get
            {
                return m_fibStructure.arrLongs.cpnBteChp;
            }
            set
            {
                m_fibStructure.arrLongs.cpnBteChp = value;
            }
        }
        /// <summary>
        /// when there was insufficient memory for Word to expand the plcfbte at save time, the plcfbte is written to the file in a linked list of 512-byte pieces starting with this pn
        /// </summary>
        internal int pnFbpPapFirst
        {
            get
            {
                return m_fibStructure.arrLongs.pnFbpPapFirst;
            }
            set
            {
                m_fibStructure.arrLongs.pnFbpPapFirst = value;
            }
        }
        /// <summary>
        /// the page number of the lowest numbered page in the document that records PAPX FKP information
        /// </summary>
        internal int pnPapFirst
        {
            get
            {
                return m_fibStructure.arrLongs.pnPapFirst;
            }
            set
            {
                m_fibStructure.arrLongs.pnPapFirst = value;
            }
        }
        /// <summary>
        /// count of PAPX FKPs recorded in file. In non-complex files if the number of entries in the plcfbtePapx is less than this, the plcfbtePapx is incomplete.
        /// </summary>
        internal int cpnBtePap
        {
            get
            {
                return m_fibStructure.arrLongs.cpnBtePap;
            }
            set
            {
                m_fibStructure.arrLongs.cpnBtePap = value;
            }
        }
        /// <summary>
        /// when there was insufficient memory for Word to expand the plcfbte at save time, the plcfbte is written to the file in a linked list of 512-byte pieces starting with this pn
        /// </summary>
        internal int pnFbpLvcFirst
        {
            get
            {
                return m_fibStructure.arrLongs.pnFbpLvcFirst;
            }
            set
            {
                m_fibStructure.arrLongs.pnFbpLvcFirst = value;
            }
        }
        /// <summary>
        /// the page number of the lowest numbered page in the document that records LVC FKP information
        /// </summary>
        internal int pnLvcFirst
        {
            get
            {
                return m_fibStructure.arrLongs.pnLvcFirst;
            }
            set
            {
                m_fibStructure.arrLongs.pnLvcFirst = value;
            }
        }
        /// <summary>
        /// count of LVC FKPs recorded in file. In non-complex files if the number of entries in the plcfbtePapx is less than this, the plcfbtePapx is incomplete.
        /// </summary>
        internal int cpnBteLvc
        {
            get
            {
                return m_fibStructure.arrLongs.cpnBteLvc;
            }
            set
            {
                m_fibStructure.arrLongs.cpnBteLvc = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int fcIslandFirst
        {
            get
            {
                return m_fibStructure.arrLongs.fcIslandFirst;
            }
            set
            {
                m_fibStructure.arrLongs.fcIslandFirst = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int fcIslandLim
        {
            get
            {
                return m_fibStructure.arrLongs.fcIslandLim;
            }
            set
            {
                m_fibStructure.arrLongs.fcIslandLim = value;
            }
        }
        #endregion

        /// <summary>
        /// Number of fields in the array of FC/LCB pairs.
        /// </summary>
        internal ushort cfclcb
        {
            get
            {
                return m_fibStructure.cfclcb;
            }
            set
            {
                m_fibStructure.cfclcb = value;
            }
        }


        #region Array of FC/LCB pairs
        //    /// <summary>
        //    /// Beginning of array of FC/LCB pairs.
        //    /// </summary>
        //    internal  rgfclcb
        //    {
        //    get
        //    { 
        //    return m_fibStructure.arrFCLCB.rgfclcb;
        //  }
        //  set
        //{
        //  m_fibStructure.arrFCLCB.rgfclcb = value;
        //}
        //}
        /// <summary>
        /// file offset of original allocation for STSH in table stream. During fast save Word will attempt to reuse this allocation if STSH is small enough to fit.
        /// </summary>
        internal int fcStshfOrig
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcStshfOrig;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcStshfOrig = value;
            }
        }
        /// <summary>
        /// count of bytes of original STSH allocation
        /// </summary>
        internal uint lcbStshfOrig
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbStshfOrig;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbStshfOrig = value;
            }
        }
        /// <summary>
        /// offset of STSH in table stream.
        /// </summary>
        internal int fcStshf
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcStshf;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcStshf = value;
            }
        }
        /// <summary>
        /// count of bytes of current STSH allocation
        /// </summary>
        internal uint lcbStshf
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbStshf;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbStshf = value;
            }
        }
        /// <summary>
        /// offset in table stream of footnote reference PLCF of FRD structures. CPs in PLC are relative to main document text stream and give location of footnote references.
        /// </summary>
        internal int fcPlcffndRef
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffndRef;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffndRef = value;
            }
        }
        /// <summary>
        /// count of bytes of footnote reference PLC== 0 if no footnotes defined in document.
        /// </summary>
        internal uint lcbPlcffndRef
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffndRef;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffndRef = value;
            }
        }
        /// <summary>
        /// offset in table stream of footnote text PLC. CPs in PLC are relative to footnote subdocument text stream and give location of beginnings of footnote text for corresponding references recorded in plcffndRef. No structure is stored in this plc. There will just be n+1 FC entries in this PLC when there are n footnotes
        /// </summary>
        internal int fcPlcffndTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffndTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffndTxt = value;
            }
        }
        /// <summary>
        /// count of bytes of footnote text PLC. == 0 if no footnotes defined in document
        /// </summary>
        internal uint lcbPlcffndTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffndTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffndTxt = value;
            }
        }
        /// <summary>
        /// offset in table stream of annotation reference ATRD PLC. The CPs recorded in this PLC give the offset of annotation references in the main document.
        /// </summary>
        internal int fcPlcfandRef
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfandRef;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfandRef = value;
            }
        }
        /// <summary>
        /// count of bytes of annotation reference PLC.
        /// </summary>
        internal uint lcbPlcfandRef
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfandRef;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfandRef = value;
            }
        }
        /// <summary>
        /// offset in table stream of annotation text PLC. The Cps recorded in this PLC give the offset of the annotation text in the annotation sub document corresponding to the references stored in the plcfandRef. There is a 1 to 1 correspondence between entries recorded in the plcfandTxt and the plcfandRef. No structure is stored in this PLC.
        /// </summary>
        internal int fcPlcfandTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfandTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfandTxt = value;
            }
        }
        /// <summary>
        /// count of bytes of the annotation text PLC
        /// </summary>
        internal uint lcbPlcfandTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfandTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfandTxt = value;
            }
        }
        /// <summary>
        /// offset in table stream of section descriptor SED PLC. CPs in PLC are relative to main document.
        /// </summary>
        internal int fcPlcfsed
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfsed;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfsed = value;
            }
        }
        /// <summary>
        /// count of bytes of section descriptor PLC.
        /// </summary>
        internal uint lcbPlcfsed
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfsed;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfsed = value;
            }
        }
        /// <summary>
        /// no longer used
        /// </summary>
        internal int fcPlcpad
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcpad;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcpad = value;
            }
        }
        /// <summary>
        /// no longer used
        /// </summary>
        internal uint lcbPlcpad
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcpad;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcpad = value;
            }
        }
        /// <summary>
        /// offset in table stream of PHE PLC of paragraph heights. CPs in PLC are relative to main document text stream. Only written for files in complex format. Should not be written by third party creators of Word files.
        /// </summary>
        internal int fcPlcfphe
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfphe;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfphe = value;
            }
        }
        /// <summary>
        /// count of bytes of paragraph height PLC. ==0 when file is non-complex.
        /// </summary>
        internal uint lcbPlcfphe
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfphe;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfphe = value;
            }
        }
        /// <summary>
        /// offset in table stream of glossary string table. This table consists of Pascal style strings (strings stored prefixed with a length byte) concatenated one after another.
        /// </summary>
        internal int fcSttbfglsy
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfglsy;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfglsy = value;
            }
        }
        /// <summary>
        /// count of bytes of glossary string table. == 0 for non-glossary documents.!=0 for glossary documents.
        /// </summary>
        internal uint lcbSttbfglsy
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfglsy;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfglsy = value;
            }
        }
        /// <summary>
        /// offset in table stream of glossary PLC. CPs in PLC are relative to main document and mark the beginnings of glossary entries and are in 1-1 correspondence with entries of sttbfglsy. No structure is stored in this PLC. There will be n+1 FC entries in this PLC when there are n glossary entries.
        /// </summary>
        internal int fcPlcfglsy
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfglsy;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfglsy = value;
            }
        }
        /// <summary>
        /// count of bytes of glossary PLC.== 0 for non-glossary documents.!=0 for glossary documents.
        /// </summary>
        internal uint lcbPlcfglsy
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfglsy;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfglsy = value;
            }
        }
        /// <summary>
        /// byte offset in table stream of header HDD PLC. CPs are relative to header subdocument and mark the beginnings of individual headers in the header subdocument. No structure is stored in this PLC. There will be n+1 FC entries in this PLC when there are n headers stored for the document.
        /// </summary>
        internal int fcPlcfhdd
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfhdd;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfhdd = value;
            }
        }
        /// <summary>
        /// count of bytes of header PLC.
        /// </summary>
        internal uint lcbPlcfhdd
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfhdd;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfhdd = value;
            }
        }
        /// <summary>
        /// offset in table stream of character property bin table.PLC. FCs in PLC are file offsets in the main stream. Describes text of main document and all subdocuments.
        /// </summary>
        internal int fcPlcfbteChpx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfbteChpx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfbteChpx = value;
            }
        }
        /// <summary>
        /// count of bytes of character property bin table PLC.
        /// </summary>
        internal uint lcbPlcfbteChpx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfbteChpx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfbteChpx = value;
            }
        }
        /// <summary>
        /// offset in table stream of paragraph property bin table.PLC. FCs in PLC are file offsets in the main stream. Describes text of main document and all subdocuments.
        /// </summary>
        internal int fcPlcfbtePapx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfbtePapx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfbtePapx = value;
            }
        }
        /// <summary>
        /// count of bytes of paragraph property bin table PLC
        /// </summary>
        internal uint lcbPlcfbtePapx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfbtePapx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfbtePapx = value;
            }
        }
        /// <summary>
        /// offset in table stream of PLC reserved for private use. The SEA is 6 bytes long.
        /// </summary>
        internal int fcPlcfsea
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfsea;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfsea = value;
            }
        }
        /// <summary>
        /// count of bytes of private use PLC.
        /// </summary>
        internal uint lcbPlcfsea
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfsea;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfsea = value;
            }
        }
        /// <summary>
        /// Offset in table stream of font information STTBF. 
        /// The sttbfffn is a STTBF where is string is actually an FFN 
        /// structure. The nth entry in the STTBF describes the font that 
        /// will be displayed when the chp.ftc for text is equal to n. 
        /// See the FFN file structure definition.
        /// </summary>
        internal int fcSttbfffn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfffn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfffn = value;
            }
        }
        /// <summary>
        /// count of bytes in sttbfffn.
        /// </summary>
        internal uint lcbSttbfffn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfffn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfffn = value;
            }
        }
        /// <summary>
        /// offset in table stream to the FLD PLC of field positions in the main document. The CPs point to the beginning CP of a field, the CP of field separator character inside a field and the ending CP of the field. A field may be nested within another field. 20 levels of field nesting are allowed.
        /// </summary>
        internal int fcPlcffldMom
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffldMom;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffldMom = value;
            }
        }
        /// <summary>
        /// count of bytes in plcffldMom
        /// </summary>
        internal uint lcbPlcffldMom
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffldMom;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffldMom = value;
            }
        }
        /// <summary>
        /// offset in table stream to the FLD PLC of field positions in the header subdocument.
        /// </summary>
        internal int fcPlcffldHdr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffldHdr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffldHdr = value;
            }
        }
        /// <summary>
        /// count of bytes in plcffldHdr
        /// </summary>
        internal uint lcbPlcffldHdr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffldHdr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffldHdr = value;
            }
        }
        /// <summary>
        /// offset in table stream to the FLD PLC of field positions in the footnote subdocument.
        /// </summary>
        internal int fcPlcffldFtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffldFtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffldFtn = value;
            }
        }
        /// <summary>
        /// count of bytes in plcffldFtn
        /// </summary>
        internal uint lcbPlcffldFtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffldFtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffldFtn = value;
            }
        }
        /// <summary>
        /// offset in table stream to the FLD PLC of field positions in the annotation subdocument.
        /// </summary>
        internal int fcPlcffldAtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffldAtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffldAtn = value;
            }
        }
        /// <summary>
        /// count of bytes in plcffldAtn
        /// </summary>
        internal uint lcbPlcffldAtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffldAtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffldAtn = value;
            }
        }
        /// <summary>
        /// no longer used
        /// </summary>
        internal int fcPlcffldMcr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffldMcr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffldMcr = value;
            }
        }
        /// <summary>
        /// no longer used
        /// </summary>
        internal uint lcbPlcffldMcr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffldMcr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffldMcr = value;
            }
        }
        /// <summary>
        /// offset in table stream of the STTBF that records bookmark names in the main document
        /// </summary>
        internal int fcSttbfbkmk
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfbkmk;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfbkmk = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbfbkmk
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfbkmk;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfbkmk = value;
            }
        }
        /// <summary>
        /// offset in table stream of the PLCF that records the beginning CP offsets of bookmarks in the main document. See BKF structure definition
        /// </summary>
        internal int fcPlcfbkf
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfbkf;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfbkf = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfbkf
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfbkf;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfbkf = value;
            }
        }
        /// <summary>
        /// offset in table stream of the PLCF that records the ending CP offsets of bookmarks recorded in the main document. No structure is stored in this PLCF.
        /// </summary>
        internal int fcPlcfbkl
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfbkl;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfbkl = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfbkl
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfbkl;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfbkl = value;
            }
        }
        /// <summary>
        /// offset in table stream of the macro commands. These commands are private and undocumented.
        /// </summary>
        internal int fcCmds
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcCmds;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcCmds = value;
            }
        }
        /// <summary>
        /// undocument size of undocument structure not documented above
        /// </summary>
        internal uint lcbCmds
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbCmds;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbCmds = value;
            }
        }
        /// <summary>
        /// no longer used
        /// </summary>
        internal int fcPlcmcr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcmcr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcmcr = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcmcr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcmcr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcmcr = value;
            }
        }
        /// <summary>
        /// no longer used
        /// </summary>
        internal int fcSttbfmcr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfmcr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfmcr = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbfmcr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfmcr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfmcr = value;
            }
        }
        /// <summary>
        /// offset in table stream of the printer driver information (names of drivers, port, etc.)
        /// </summary>
        internal int fcPrDrvr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPrDrvr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPrDrvr = value;
            }
        }
        /// <summary>
        /// count of bytes of the printer driver information (names of drivers, port, etc.)
        /// </summary>
        internal uint lcbPrDrvr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPrDrvr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPrDrvr = value;
            }
        }
        /// <summary>
        /// offset in table stream of the print environment in portrait mode.
        /// </summary>
        internal int fcPrEnvPort
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPrEnvPort;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPrEnvPort = value;
            }
        }
        /// <summary>
        /// count of bytes of the print environment in portrait mode.
        /// </summary>
        internal uint lcbPrEnvPort
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPrEnvPort;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPrEnvPort = value;
            }
        }
        /// <summary>
        /// offset in table stream of the print environment in landscape mode.
        /// </summary>
        internal int fcPrEnvLand
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPrEnvLand;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPrEnvLand = value;
            }
        }
        /// <summary>
        /// count of bytes of the print environment in landscape mode.
        /// </summary>
        internal uint lcbPrEnvLand
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPrEnvLand;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPrEnvLand = value;
            }
        }
        /// <summary>
        /// offset in table stream of Window Save State data structure. WSS contains dimensions of document's main text window and the last selection made by Word user.
        /// </summary>
        internal int fcWss
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcWss;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcWss = value;
            }
        }
        /// <summary>
        /// count of bytes of WSS. ==0 if unable to store the window state. Should not be written by third party creators of Word files.
        /// </summary>
        internal uint lcbWss
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbWss;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbWss = value;
            }
        }
        /// <summary>
        /// offset in table stream of document property data structure.
        /// </summary>
        internal int fcDop
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcDop;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcDop = value;
            }
        }
        /// <summary>
        /// count of bytes of document properties.
        /// </summary>
        internal uint lcbDop
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbDop;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbDop = value;
            }
        }
        /// <summary>
        /// offset in table stream of STTBF of associated strings. The strings in this table specify document summary info and the paths to special documents related to this document. See documentation of the STTBFASSOC.
        /// </summary>
        internal int fcSttbfAssoc
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfAssoc;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfAssoc = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbfAssoc
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfAssoc;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfAssoc = value;
            }
        }
        /// <summary>
        /// offset in table stream of beginning of information for complex files. Consists of an encoding of all of the prms quoted by the document followed by the plcpcd (piece table) for the document.
        /// </summary>
        internal int fcClx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcClx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcClx = value;
            }
        }
        /// <summary>
        /// count of bytes of complex file information == 0 if file is non-complex.
        /// </summary>
        internal uint lcbClx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbClx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbClx = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal int fcPlcfpgdFtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfpgdFtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfpgdFtn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfpgdFtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfpgdFtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfpgdFtn = value;
            }
        }
        /// <summary>
        /// offset in table stream of the name of the original file. fcAutosaveSource and cbAutosaveSource should both be 0 if autosave is off.
        /// </summary>
        internal int fcAutosaveSource
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcAutosaveSource;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcAutosaveSource = value;
            }
        }
        /// <summary>
        /// count of bytes of the name of the original file.
        /// </summary>
        internal uint lcbAutosaveSource
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbAutosaveSource;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbAutosaveSource = value;
            }
        }
        /// <summary>
        /// offset in table stream of group of strings recording the names of the owners of annotations stored in the document
        /// </summary>
        internal int fcGrpXstAtnOwners
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcGrpXstAtnOwners;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcGrpXstAtnOwners = value;
            }
        }
        /// <summary>
        /// count of bytes of the group of strings
        /// </summary>
        internal uint lcbGrpXstAtnOwners
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbGrpXstAtnOwners;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbGrpXstAtnOwners = value;
            }
        }
        /// <summary>
        /// offset in table stream of the sttbf that records names of bookmarks for the annotation subdocument
        /// </summary>
        internal int fcSttbfAtnbkmk
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfAtnbkmk;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfAtnbkmk = value;
            }
        }
        /// <summary>
        /// length in bytes of the sttbf that records names of bookmarks for the annotation subdocument
        /// </summary>
        internal uint lcbSttbfAtnbkmk
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfAtnbkmk;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfAtnbkmk = value;
            }
        }
        /// <summary>
        /// no longer used
        /// </summary>
        internal int fcPlcdoaMom
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcdoaMom;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcdoaMom = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcdoaMom
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcdoaMom;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcdoaMom = value;
            }
        }
        /// <summary>
        /// no longer used
        /// </summary>
        internal int fcPlcdoaHdr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcdoaHdr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcdoaHdr = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcdoaHdr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcdoaHdr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcdoaHdr = value;
            }
        }
        /// <summary>
        /// offset in table stream of the FSPA PLC for main document. == 0 if document has no office art objects.
        /// </summary>
        internal int fcPlcspaMom
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcspaMom;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcspaMom = value;
            }
        }
        /// <summary>
        /// length in bytes of the FSPA PLC of the main document.
        /// </summary>
        internal uint lcbPlcspaMom
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcspaMom;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcspaMom = value;
            }
        }
        /// <summary>
        /// offset in table stream of the FSPA PLC for header document. == 0 if document has no office art objects.
        /// </summary>
        internal int fcPlcspaHdr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcspaHdr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcspaHdr = value;
            }
        }
        /// <summary>
        /// length in bytes of the FSPA PLC of the header document.
        /// </summary>
        internal uint lcbPlcspaHdr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcspaHdr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcspaHdr = value;
            }
        }
        /// <summary>
        /// offset in table stream of BKF (bookmark first) PLC of the annotation subdocument
        /// </summary>
        internal int fcPlcfAtnbkf
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfAtnbkf;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfAtnbkf = value;
            }
        }
        /// <summary>
        /// length in bytes of BKF (bookmark first) PLC of the annotation subdocument
        /// </summary>
        internal uint lcbPlcfAtnbkf
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfAtnbkf;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfAtnbkf = value;
            }
        }
        /// <summary>
        /// offset in table stream of BKL (bookmark last) PLC of the annotation subdocument
        /// </summary>
        internal int fcPlcfAtnbkl
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfAtnbkl;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfAtnbkl = value;
            }
        }
        /// <summary>
        /// length in bytes of PLC marking the CP limits of the annotation bookmarks. No structure is stored in this PLC.
        /// </summary>
        internal uint lcbPlcfAtnbkl
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfAtnbkl;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfAtnbkl = value;
            }
        }
        /// <summary>
        /// offset in table stream of PMS (Print Merge State) information block. This contains the current state of a print merge operation
        /// </summary>
        internal int fcPms
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPms;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPms = value;
            }
        }
        /// <summary>
        /// length in bytes of PMS. ==0 if no current print merge state. Should not be written by third party creators of Word files.
        /// </summary>
        internal uint lcbPms
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPms;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPms = value;
            }
        }
        /// <summary>
        /// offset in table stream of form field Sttbf which contains strings used in form field dropdown controls
        /// </summary>
        internal int fcFormFldSttbs
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcFormFldSttbs;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcFormFldSttbs = value;
            }
        }
        /// <summary>
        /// length in bytes of form field Sttbf
        /// </summary>
        internal uint lcbFormFldSttbs
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbFormFldSttbs;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbFormFldSttbs = value;
            }
        }
        /// <summary>
        /// offset in table stream of endnote reference PLCF of FRD structures. CPs in PLCF are relative to main document text stream and give location of endnote references.
        /// </summary>
        internal int fcPlcfendRef
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfendRef;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfendRef = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfendRef
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfendRef;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfendRef = value;
            }
        }
        /// <summary>
        /// offset in table stream of PlcfendRef which points to endnote text in the endnote document stream which corresponds with the plcfendRef. No structure is stored in this PLC.
        /// </summary>
        internal int fcPlcfendTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfendTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfendTxt = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfendTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfendTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfendTxt = value;
            }
        }
        /// <summary>
        /// offset in table stream to FLD PLCF of field positions in the endnote subdoc
        /// </summary>
        internal int fcPlcffldEdn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffldEdn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffldEdn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcffldEdn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffldEdn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffldEdn = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        internal int fcPlcfpgdEdn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfpgdEdn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfpgdEdn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfpgdEdn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfpgdEdn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfpgdEdn = value;
            }
        }
        /// <summary>
        /// offset in table stream of the office art object table data. The format of office art object table data is found in a separate document.
        /// </summary>
        internal int fcDggInfo
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcDggInfo;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcDggInfo = value;
            }
        }
        /// <summary>
        /// length in bytes of the office art object table data
        /// </summary>
        internal uint lcbDggInfo
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbDggInfo;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbDggInfo = value;
            }
        }
        /// <summary>
        /// offset in table stream to STTBF that records the author abbreviations for authors who have made revisions in the document.
        /// </summary>
        internal int fcSttbfRMark
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfRMark;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfRMark = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbfRMark
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfRMark;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfRMark = value;
            }
        }
        /// <summary>
        /// offset in table stream to STTBF that records caption titles used in the document.
        /// </summary>
        internal int fcSttbCaption
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbCaption;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbCaption = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbCaption
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbCaption;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbCaption = value;
            }
        }
        /// <summary>
        /// offset in table stream to the STTBF that records the object names and indices into the caption STTBF for objects which get auto captions.
        /// </summary>
        internal int fcSttbAutoCaption
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbAutoCaption;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbAutoCaption = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbAutoCaption
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbAutoCaption;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbAutoCaption = value;
            }
        }
        /// <summary>
        /// offset in table stream to WKB PLCF that describes the boundaries of contributing documents in a master document
        /// </summary>
        internal int fcPlcfwkb
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfwkb;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfwkb = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfwkb
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfwkb;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfwkb = value;
            }
        }
        /// <summary>
        /// offset in table stream of PLCF (of SPLS structures) that records spell check state
        /// </summary>
        internal int fcPlcfspl
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfspl;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfspl = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfspl
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfspl;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfspl = value;
            }
        }
        /// <summary>
        /// offset in table stream of PLCF that records the beginning CP in the text box subdoc of the text of individual text box entries. No structure is stored in this PLCF
        /// </summary>
        internal int fcPlcftxbxTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcftxbxTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcftxbxTxt = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcftxbxTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcftxbxTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcftxbxTxt = value;
            }
        }
        /// <summary>
        /// offset in table stream of the FLD PLCF that records field boundaries recorded in the textbox subdoc.
        /// </summary>
        internal int fcPlcffldTxbx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffldTxbx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffldTxbx = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcffldTxbx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffldTxbx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffldTxbx = value;
            }
        }
        /// <summary>
        /// offset in table stream of PLCF that records the beginning CP in the header text box subdoc of the text of individual header text box entries. No structure is stored in this PLC.
        /// </summary>
        internal int fcPlcfhdrtxbxTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfhdrtxbxTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfhdrtxbxTxt = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfhdrtxbxTxt
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfhdrtxbxTxt;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfhdrtxbxTxt = value;
            }
        }
        /// <summary>
        /// offset in table stream of the FLD PLCF that records field boundaries recorded in the header textbox subdoc.
        /// </summary>
        internal int fcPlcffldHdrTxbx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcffldHdrTxbx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcffldHdrTxbx = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcffldHdrTxbx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcffldHdrTxbx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcffldHdrTxbx = value;
            }
        }
        /// <summary>
        /// Macro User storage
        /// </summary>
        internal int fcStwUser
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcStwUser;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcStwUser = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbStwUser
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbStwUser;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbStwUser = value;
            }
        }
        /// <summary>
        /// offset in table stream of embedded true type font data.
        /// </summary>
        internal int fcSttbttmbd
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbttmbd;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbttmbd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint cbSttbttmbd
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.cbSttbttmbd;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.cbSttbttmbd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int fcUnused
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcUnused;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcUnused = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbUnused
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbUnused;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbUnused = value;
            }
        }
        //    /// <summary>
        //    /// beginning of array of fcPgd / fcBkd pairs
        //    /// </summary>
        //    internal FCPGD rgpgdbkd
        //    {
        //      get
        //      { 
        //        return m_fibStructure.arrFCLCB.knownPart.rgpgdbkd;
        //      }
        //      set
        //      {
        //        m_fibStructure.arrFCLCB.knownPart.rgpgdbkd = value;
        //      }
        //    }
        /// <summary>
        /// offset in table stream of the PLF that records the page descriptors for the main text of the doc.
        /// </summary>
        internal int fcPgdMother
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPgdMother;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPgdMother = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPgdMother
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPgdMother;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPgdMother = value;
            }
        }
        /// <summary>
        /// offset in table stream of the PLCF that records the break descriptors for the main text of the doc.
        /// </summary>
        internal int fcBkdMother
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcBkdMother;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcBkdMother = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbBkdMother
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbBkdMother;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbBkdMother = value;
            }
        }
        /// <summary>
        /// offset in table stream of the PLF that records the page descriptors for the footnote text of the doc.
        /// </summary>
        internal int fcPgdFtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPgdFtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPgdFtn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPgdFtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPgdFtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPgdFtn = value;
            }
        }
        /// <summary>
        /// offset in table stream of the PLCF that records the break descriptors for the footnote text of the doc.
        /// </summary>
        internal int fcBkdFtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcBkdFtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcBkdFtn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbBkdFtn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbBkdFtn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbBkdFtn = value;
            }
        }
        /// <summary>
        /// offset in table stream of the PLF that records the page descriptors for the endnote text of the doc.
        /// </summary>
        internal int fcPgdEdn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPgdEdn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPgdEdn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPgdEdn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPgdEdn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPgdEdn = value;
            }
        }
        /// <summary>
        /// offset in table stream of the PLCF that records the break descriptors for the endnote text of the doc.
        /// </summary>
        internal int fcBkdEdn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcBkdEdn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcBkdEdn = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbBkdEdn
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbBkdEdn;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbBkdEdn = value;
            }
        }
        /// <summary>
        /// offset in table stream of the STTBF containing field keywords. This is only used in a small number of the international versions of word. This field is no longer written to the file for nFib >= 167.
        /// </summary>
        internal int fcSttbfIntlFld
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfIntlFld;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfIntlFld = value;
            }
        }
        /// <summary>
        /// Always 0 for nFib >= 167.
        /// </summary>
        internal uint lcbSttbfIntlFld
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfIntlFld;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfIntlFld = value;
            }
        }
        /// <summary>
        /// offset in table stream of a mailer routing slip.
        /// </summary>
        internal int fcRouteSlip
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcRouteSlip;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcRouteSlip = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbRouteSlip
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbRouteSlip;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbRouteSlip = value;
            }
        }
        /// <summary>
        /// offset in table stream of STTBF recording the names of the users who have saved this document alternating with the save locations.
        /// </summary>
        internal int fcSttbSavedBy
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbSavedBy;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbSavedBy = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbSavedBy
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbSavedBy;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbSavedBy = value;
            }
        }
        /// <summary>
        /// offset in table stream of STTBF recording filenames of documents which are referenced by this document.
        /// </summary>
        internal int fcSttbFnm
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbFnm;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbFnm = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbFnm
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbFnm;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbFnm = value;
            }
        }
        /// <summary>
        /// Offset in the table stream of list format information.
        /// </summary>
        internal int fcPlcfLst
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfLst;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfLst = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfLst
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfLst;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfLst = value;
            }
        }
        /// <summary>
        /// offset in the table stream of list format override information.
        /// </summary>
        internal int fcPlfLfo
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlfLfo;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlfLfo = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlfLfo
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlfLfo;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlfLfo = value;
            }
        }
        /// <summary>
        /// offset in the table stream of the textbox break table (a PLCF of BKDs) for the main document
        /// </summary>
        internal int fcPlcftxbxBkd
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcftxbxBkd;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcftxbxBkd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcftxbxBkd
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcftxbxBkd;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcftxbxBkd = value;
            }
        }
        /// <summary>
        /// offset in the table stream of the textbox break table (a PLCF of BKDs) for the header subdocument
        /// </summary>
        internal int fcPlcftxbxHdrBkd
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcftxbxHdrBkd;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcftxbxHdrBkd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcftxbxHdrBkd
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcftxbxHdrBkd;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcftxbxHdrBkd = value;
            }
        }
        /// <summary>
        /// offset in main stream of undocumented undo / versioning data
        /// </summary>
        internal int fcDocUndo
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcDocUndo;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcDocUndo = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbDocUndo
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbDocUndo;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbDocUndo = value;
            }
        }
        /// <summary>
        /// offset in main stream of undocumented undo / versioning data
        /// </summary>
        internal int fcRgbuse
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcRgbuse;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcRgbuse = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbRgbuse
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbRgbuse;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbRgbuse = value;
            }
        }
        /// <summary>
        /// offset in main stream of undocumented undo / versioning data
        /// </summary>
        internal int fcUsp
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcUsp;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcUsp = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbUsp
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbUsp;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbUsp = value;
            }
        }
        /// <summary>
        /// offset in table stream of undocumented undo / versioning data
        /// </summary>
        internal int fcUskf
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcUskf;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcUskf = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbUskf
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbUskf;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbUskf = value;
            }
        }
        /// <summary>
        /// offset in table stream of undocumented undo / versioning data
        /// </summary>
        internal int fcPlcupcRgbuse
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcupcRgbuse;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcupcRgbuse = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcupcRgbuse
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcupcRgbuse;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcupcRgbuse = value;
            }
        }
        /// <summary>
        /// offset in table stream of undocumented undo / versioning data
        /// </summary>
        internal int fcPlcupcUsp
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcupcUsp;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcupcUsp = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcupcUsp
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcupcUsp;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcupcUsp = value;
            }
        }
        /// <summary>
        /// offset in table stream of string table of style names for glossary entries
        /// </summary>
        internal int fcSttbGlsyStyle
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbGlsyStyle;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbGlsyStyle = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbGlsyStyle
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbGlsyStyle;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbGlsyStyle = value;
            }
        }
        /// <summary>
        /// offset in table stream of undocumented grammar options PL
        /// </summary>
        internal int fcPlgosl
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlgosl;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlgosl = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlgosl
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlgosl;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlgosl = value;
            }
        }
        /// <summary>
        /// offset in table stream of undocumented ocx data
        /// </summary>
        internal int fcPlcocx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcocx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcocx = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcocx
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcocx;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcocx = value;
            }
        }
        /// <summary>
        /// offset in table stream of character property bin table.PLC. FCs in PLC are file offsets. Describes text of main document and all subdocuments.
        /// </summary>
        internal int fcPlcfbteLvc
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfbteLvc;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfbteLvc = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfbteLvc
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfbteLvc;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfbteLvc = value;
            }
        }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    internal FILETIME ftModified
        //    {
        //      get
        //      { 
        //        return m_fibStructure.arrFCLCB.knownPart.ftModified;
        //      }
        //      set
        //      {
        //        m_fibStructure.arrFCLCB.knownPart.ftModified = value;
        //      }
        //    }
        /// <summary>
        /// 
        /// </summary>
        internal uint dwLowDateTime
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.dwLowDateTime;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.dwLowDateTime = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint dwHighDateTime
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.dwHighDateTime;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.dwHighDateTime = value;
            }
        }
        /// <summary>
        /// offset in table stream of LVC PLCF
        /// </summary>
        internal int fcPlcflvc
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcflvc;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcflvc = value;
            }
        }
        /// <summary>
        /// size of LVC PLCF, ==0 for non-complex files
        /// </summary>
        internal uint lcbPlcflvc
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcflvc;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcflvc = value;
            }
        }
        /// <summary>
        /// offset in table stream of autosummary ASUMY PLCF.
        /// </summary>
        internal int fcPlcasumy
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcasumy;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcasumy = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcasumy
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcasumy;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcasumy = value;
            }
        }
        /// <summary>
        /// offset in table stream of PLCF (of SPLS structures) which records grammar check state
        /// </summary>
        internal int fcPlcfgram
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcPlcfgram;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcPlcfgram = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbPlcfgram
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbPlcfgram;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbPlcfgram = value;
            }
        }
        /// <summary>
        /// offset in table stream of list names string table
        /// </summary>
        internal int fcSttbListNames
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbListNames;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbListNames = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbListNames
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbListNames;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbListNames = value;
            }
        }
        /// <summary>
        /// offset in table stream of undocumented undo / versioning data
        /// </summary>
        internal int fcSttbfUssr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.fcSttbfUssr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.fcSttbfUssr = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal uint lcbSttbfUssr
        {
            get
            {
                return m_fibStructure.arrFCLCB.knownPart.lcbSttbfUssr;
            }
            set
            {
                m_fibStructure.arrFCLCB.knownPart.lcbSttbfUssr = value;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return m_fibStructure.Length;
            }
        }


        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="provider"></param>
        internal void ParseDecrypted(Stream stream)
        {
            m_fibStructure.Parse(stream, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="provider">MemoryConverter to convert array of bytes into structure.</param>
        private void Parse(Stream stream)
        {
            m_fibStructure.Parse(stream, false);
            //      int iSize = m_fibStructure.PrepareStructure( stream );
            //
            //      if( iSize < 0 )
            //        throw new ApplicationException( "Can't create array with negative size" );
            //
            //      byte[] arrBuffer = new byte[ iSize ];
            //      int iRealLen = stream.Read( arrBuffer, 0, arrBuffer.Length );
            //
            //      if( iRealLen != iSize )
            //        throw new ApplicationException( "End of the stream reached before enough bytes were read from the stream" );
            //
            //      Parse( arrBuffer, provider );
        }
        /// <summary>
        /// Saves record into array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            return m_fibStructure.Save(arrData, iOffset);
        }

        #endregion
    }
}
