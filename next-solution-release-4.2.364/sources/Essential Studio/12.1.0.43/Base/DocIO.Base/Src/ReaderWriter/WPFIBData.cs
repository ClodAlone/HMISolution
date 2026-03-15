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
using System.Text;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Wrapper class for FIBRecord object.
    /// </summary>
    [CLSCompliant(false)]
    [DocumentationExclude()]
    internal class WPFIBData
    {
        #region Constants
        internal const int Size = 0x3fa;
        #endregion

        #region Class members
        //    /// <summary>
        //    /// The FIB records contains structure.
        //    /// </summary>
        //    private FIBRecord m_fib;
        //    /// <summary>
        //    /// Reference to main MemoryConverter class instance
        //    /// </summary>
        //    private MemoryConverter m_memConverter;

        /// <summary>
        /// 
        /// </summary>
        private Encoding m_encoding = Encoding.Unicode; //Encoding.ASCII; //Encoding.Unicode;

        /// <summary>
        /// 
        /// </summary>
        private ushort m_wIdent;

        private ushort m_cfclcb;
        private int m_fcStshfOrig;
        private int m_lcbStshfOrig;
        private int m_fcStshf;
        private int m_lcbStshf;
        private int m_fcPlcffndRef;
        private int m_lcbPlcffndRef;
        private int m_fcPlcffndTxt;
        private int m_lcbPlcffndTxt;
        private int m_fcPlcfandRef;
        private int m_fcMac;
        private ushort m_csw;
        private ushort m_wMagicCreated;
        private ushort m_wMagicRevised;
        private ushort m_wMagicCreatedPrivate;
        private ushort m_wMagicRevisedPrivate;
        private ushort m_pnFbpChpFirst_W6;
        private ushort m_pnChpFirst_W6;
        private ushort m_cpnBteChp_W6;
        private ushort m_pnFbpPapFirst_W6;
        private ushort m_pnPapFirst_W6;
        private ushort m_cpnBtePap_W6;
        private ushort m_pnFbpLvcFirst_W6;
        private ushort m_pnLvcFirst_W6;
        private ushort m_cpnBteLvc_W6;
        private short m_lidFE;
        private ushort m_clw;
        private int m_cbMac;
        private int m_lProductCreated;
        private int m_lProductRevised;
        private int m_ccpText;
        private int m_ccpFtn;
        private int m_ccpHdd;
        private int m_ccpMcr;
        private int m_ccpAtn;
        private int m_ccpEdn;
        private int m_ccpTxbx;
        private int m_ccpHdrTxbx;
        private int m_pnFbpChpFirst;
        private int m_pnFbpPapFirst;
        private int m_pnFbpLvcFirst;
        private int m_pnLvcFirst;
        private int m_cpnBteLvc;
        private int m_fcIslandFirst;
        private int m_fcIslandLim;
        private ushort m_fibVersion;
        private int m_lcbPlcfFldHdr;
        private int m_fcPlcffldFtn;
        private int m_lcbPlcffldFtn;
        private int m_fcPlcffldAtn;
        private int m_lcbPlcffldAtn;
        private int m_fcPlcffldMcr;
        private int m_lcbPlcffldMcr;
        private int m_fcSttbfbkmk;
        private int m_lcbSttbfbkmk;
        private int m_fcPlcfbkf;
        private int m_lcbPlcfandRef;
        private int m_fcPlcfandTxt;
        private int m_lcbPlcfandTxt;
        private int m_fcPlcfsed;
        private int m_lcbPlcfsed;
        private int m_fcPlcfpad;
        private int m_lcbPlcfpad;
        private int m_fcPlcfphe;
        private int m_lcbPlcfphe;
        private int m_fcSttbfglsy;
        private int m_lcbSttbfglsy;
        private int m_fcPlcfglsy;
        private int m_lcbPlcfglsy;
        private int m_fcPlcfhdd;
        private int m_lcbPlcfhdd;
        private int m_fcPlcfbteChpx;
        private int m_lcbPlcfbteChpx;
        private int m_fcPlcfbtePapx;
        private int m_lcbPlcfbtePapx;
        private int m_fcPlcfsea;
        private int m_lcbPlcfsea;
        private int m_fcSttbfffn;
        private int m_lcbSttbfffn;
        private int m_fcPlcfFldMom;
        private int m_lcbPlcfFldMom;
        private int m_fcPlcfFldHdr;
        private ushort m_nProduct;
        private int m_lcbAutosaveSource;
        private int m_fcGrpXstAtnOwners;
        private int m_lcbGrpXstAtnOwners;
        private int m_fcSttbfAtnbkmk;
        private int m_lcbfcSttbfAtnbkmk;
        private int m_fcPlcdoaMom;
        private int m_lcbPlcdoaMom;
        private int m_fcPlcdoaHdr;
        private int m_lcbPlcdoaHdr;
        private int m_fcPlcspaMom;
        private int m_lcbPlcfbkf;
        private int m_fcPlcfbkl;
        private int m_lcbPlcfbkl;
        private int m_fcCmds;
        private int m_lcbCmds;
        private int m_fcPlcmcr;
        private int m_lcbPlcmcr;
        private int m_fcSttbfmcr;
        private int m_lcbSttbfmcr;
        private int m_fcPrDrvr;
        private int m_lcbPrDrvr;
        private int m_fcPrEnvPort;
        private int m_lcbPrEnvPort;
        private int m_fcPrEnvLand;
        private int m_lcbPrEnvLand;
        private int m_fcWss;
        private int m_lcbWss;
        private int m_fcDop;
        private int m_lcbDop;
        private int m_fcSttbfAssoc;
        private int m_lcbSttbfAssoc;
        private int m_fcClx;
        private int m_lcbClx;
        private int m_fcPlcfpgdFtn;
        private int m_lcbPlcfpgdFtn;
        private int m_fcAutosaveSource;
        private short m_lid;
        private int m_lcbSttbfAutoCaption;
        private int m_fcPlcfwkb;
        private int m_lcbPlcfwkb;
        private int m_fcPlcfspl;
        private int m_lcbPlcfspl;
        private int m_fcPlcftxbxTxt;
        private int m_lcbPlcftxbxTxt;
        private int m_fcPlcffldTxbx;
        private int m_lcbPlcffldTxbx;
        private int m_fcPlcfHdrtxbxTxt;
        private int m_lcbPlcspaMom;
        private int m_fcPlcspaHdr;
        private int m_lcbPlcspaHdr;
        private int m_fcPlcfAtnbkf;
        private int m_lcbPlcfAtnbkf;
        private int m_fcPlcfAtnbkl;
        private int m_lcbPlcfAtnbkl;
        private int m_fcPms;
        private int m_lcbPms;
        private int m_fcFormFldSttbf;
        private int m_lcbFormFldSttbf;
        private int m_fcPlcfendRef;
        private int m_lcbPlcfendRef;
        private int m_fcPlcfendTxt;
        private int m_lcbPlcfendTxt;
        private int m_fcPlcffldEdn;
        private int m_lcbPlcffldEdn;
        private int m_fcPlcfpgdEdn;
        private int m_lcbPlcfpgdEdn;
        private int m_fcDggInfo;
        private int m_lcbDggInfo;
        private int m_fcSttbfRMark;
        private int m_lcbSttbfRMark;
        private int m_fcSttbfCaption;
        private int m_lcbSttbfCaption;
        private int m_fcSttbfAutoCaption;
        private short m_pnNext;
        private int m_lcbPlcfHdrtxbxTxt;
        private int m_fcPlcffldHdrTxbx;
        private int m_lcbPlcffldHdrTxbx;
        private int m_fcStwUser;
        private int m_lcbStwUser;
        private int m_fcSttbttmbd;
        private int m_lcbSttbttmbd;
        private int m_fcUnused;
        private int m_lcbUnused;

        // Added
        private int m_fcPgdMother;
        private int m_lcbPgdMother;
        private int m_fcBkdMother;
        private int m_lcbBkdMother;
        private int m_fcPgdFtn;
        private int m_lcbPgdFtn;
        private int m_fcBkdFtn;
        private int m_lcbBkdFtn;
        private int m_fcPgdEdn;
        private int m_lcbPgdEdn;
        private int m_fcBkdEdn;
        private int m_lcbBkdEdn;
        private int m_fcSttbfIntlFld;
        private int m_lcbSttbfIntlFld;
        private int m_fcRouteSlip;
        private int m_lcbRouteSlip;
        private int m_fcSttbSavedBy;
        private int m_lcbSttbSavedBy;
        private int m_fcSttbFnm;
        private int m_lcbSttbFnm;


        private int m_fcPlcfLst;
        private int m_lcbPlcfLst;
        private int m_fcPlfLfo;
        private int m_lcbPlfLfo;
        private int m_fcPlcftxbxBkd;
        private int m_lcbPlcftxbxBkd;
        private int m_fcPlcfHdrtxbxBkd;
        private int m_lcbPlcfHdrtxbxBkd;

        // Added
        private int m_fcDocUndo;
        private int m_lcbDocUndo;
        private int m_fcRgbuse;
        private int m_lcbRgbuse;
        private int m_fcUsp;
        private int m_lcbUsp;
        private int m_fcUskf;
        private int m_lcbUskf;
        private int m_fcPlcupcRgbuse;
        private int m_lcbPlcupcRgbuse;
        private int m_fcPlcupcUsp;
        private int m_lcbPlcupcUsp;
        private int m_fcSttbGlsyStyle;
        private int m_lcbSttbGlsyStyle;
        private int m_fcPlgosl;
        private int m_lcbPlgosl;

        private int m_fcPlcocx;
        private int m_lcbPlcocx;
        private int m_fcPlcfbteLvc;
        private int m_lcbPlcfbteLvc;

        // Added
        private int m_dwLowDateTime;
        private int m_wHighDateTime;
        private int m_fcPlcflvc;
        private int m_lcbPlcflvc;

        private int m_fcPlcasumy;
        private int m_lcbPlcasumy;
        private int m_fcPlcfgram;
        private int m_lcbPlcfgram;

        private int m_fcSttbListNames;
        private int m_lcbSttbListNames;

        //Added
        private int m_fcSttbfUssr;
        private int m_lcbSttbfUssr;

        private int m_pnChpFirst;
        private int m_pnPapFirst;
        private int m_cpnBteChp;
        private int m_cpnBtePap;
        private int m_version;
        private bool m_fDot;
        private bool m_fGlsy;
        private bool m_fComplex;
        private bool m_fHasPic;
        private int m_cQuickSaves;
        private bool m_fEncrypted;
        private bool m_fWhichTblStm;
        private bool m_fExtChar;
        private ushort m_nFibBack;
        private uint m_lKey;
        private ushort m_lKey2;
        private byte m_envr;
        private bool m_fMac;
        private bool m_fEmptySpecial;
        private bool m_fLoadOverridePage;
        private bool m_fFutureSavedUndo;
        private bool m_fWord97Saved;
        private bool m_fSpare0;
        private ushort m_chs;
        private ushort m_chsTables;
        private int m_fcMin;

        /// <summary>
        /// 
        /// </summary>
        private ushort m_prevCsw;

        private ushort m_prevClw;
        private bool m_fReadOnlyRecom;
        private bool m_fWriteReserve;
        private bool m_fFarEast;
        private bool m_fCrypto;

        private ushort m_cswNew;
        private ushort m_nFibNew;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WPFIBData"/> class.
        /// </summary>
        internal WPFIBData()
        {
            m_wIdent = 0xa5ec;
            m_fibVersion = 0xc1;
            m_nProduct = 0x4035;
            m_lid = 0x409;
            m_fComplex = false;
            m_fExtChar = true;
            m_nFibBack = 0xbf;
            m_fWord97Saved = true;
            m_fSpare0 = false;
            m_csw = 14;
            m_wMagicCreated = 0x6a62;
            m_wMagicRevised = 0x6a62;
            m_wMagicCreatedPrivate = 0x32cf;
            m_wMagicRevisedPrivate = 0x32cf;
            m_lidFE = 0x409;
            m_clw = 0x16;
            m_pnFbpChpFirst = m_pnFbpPapFirst = m_pnFbpLvcFirst = 0xfffff;
            m_cfclcb = 0x6c;
            m_lProductCreated = 0x2c8c;
            m_lProductRevised = 0x2c8c;

            fcMin = 2048;
            fWhichTblStm = true;
        }
        #endregion

        #region Class properties
        ///// <summary>
        ///// 
        ///// </summary>
        //internal int StartFootnote
        //{
        //  get
        //  {
        //    return ( m_ccpText ) * EncodingCharSize;
        //  }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //internal int StartHeader
        //{
        //  get
        //  {
        //    return ( m_ccpText + m_ccpFtn ) * EncodingCharSize;
        //  }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //internal int StartAnnotation
        //{
        //  get
        //  {
        //    return ( m_ccpText + m_ccpFtn + m_ccpHdd ) * EncodingCharSize;
        //  }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //internal int StartEndnote
        //{
        //  get
        //  {
        //    return ( m_ccpText + m_ccpFtn + m_ccpHdd + m_ccpAtn ) * EncodingCharSize;
        //  }
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        //internal int StartTextBox
        //{
        //  get
        //  {
        //    return ( m_ccpText + m_ccpFtn + m_ccpHdd + m_ccpAtn + m_ccpEdn ) * EncodingCharSize;
        //  }
        //}

        //    /// <summary>
        //    /// Gets orginal FIB record structure.
        //    /// </summary>
        ////    internal FIBRecord FIB
        ////    {
        ////      get
        ////      {
        ////        return m_fib;
        ////      }
        ////    }

        /// <summary>
        /// Indicates the format data in "Main" stream.
        /// </summary>
        internal bool IsComplexFile
        {
            get
            {
                return fComplex;
            }
        }

        /// <summary>
        /// Gets used stream name by Tables.
        /// </summary>
        internal string TableStreamName
        {
            get
            {
                return (fWhichTblStm ? "1" : "0") + "Table";
            }
        }

        /// <summary>
        /// Gets/sets current encoding
        /// </summary>
        internal Encoding Encoding
        {
            get
            {
                return m_encoding;
            }

            set
            {
                m_encoding = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int EncodingCharSize
        {
            get
            {
#if SILVERLIGHT || WP
                return (m_encoding == Encoding.UTF8) ? 1 : 2;
#else
				return (m_encoding == Encoding.ASCII) ? 1 : 2;
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort wIdent
        {
            get
            {
                return m_wIdent;
            }

            set
            {
                m_wIdent = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort cfclcb
        {
            get
            {
                return m_cfclcb;
            }

            set
            {
                m_cfclcb = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcStshfOrig
        {
            get
            {
                return m_fcStshfOrig;
            }

            set
            {
                m_fcStshfOrig = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbStshfOrig
        {
            get
            {
                return m_lcbStshfOrig;
            }

            set
            {
                m_lcbStshfOrig = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcStshf
        {
            get
            {
                return m_fcStshf;
            }

            set
            {
                m_fcStshf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbStshf
        {
            get
            {
                return m_lcbStshf;
            }

            set
            {
                m_lcbStshf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcffndRef
        {
            get
            {
                return m_fcPlcffndRef;
            }

            set
            {
                m_fcPlcffndRef = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcffndRef
        {
            get
            {
                return m_lcbPlcffndRef;
            }

            set
            {
                m_lcbPlcffndRef = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcffndTxt
        {
            get
            {
                return m_fcPlcffndTxt;
            }

            set
            {
                m_fcPlcffndTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcffndTxt
        {
            get
            {
                return m_lcbPlcffndTxt;
            }

            set
            {
                m_lcbPlcffndTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfandRef
        {
            get
            {
                return m_fcPlcfandRef;
            }

            set
            {
                m_fcPlcfandRef = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcMac
        {
            get
            {
                return m_fcMac;
            }

            set
            {
                m_fcMac = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort csw
        {
            get
            {
                return m_csw;
            }

            set
            {
                m_csw = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort wMagicCreated
        {
            get
            {
                return m_wMagicCreated;
            }

            set
            {
                m_wMagicCreated = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort wMagicRevised
        {
            get
            {
                return m_wMagicRevised;
            }

            set
            {
                m_wMagicRevised = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort wMagicCreatedPrivate
        {
            get
            {
                return m_wMagicCreatedPrivate;
            }

            set
            {
                m_wMagicCreatedPrivate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort WMagicRevisedPrivate
        {
            get
            {
                return m_wMagicRevisedPrivate;
            }

            set
            {
                m_wMagicRevisedPrivate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal short lidFE
        {
            get
            {
                return m_lidFE;
            }

            set
            {
                m_lidFE = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort clw
        {
            get
            {
                return m_clw;
            }

            set
            {
                m_clw = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int cbMac
        {
            get
            {
                return m_cbMac;
            }

            set
            {
                m_cbMac = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lProductCreated
        {
            get
            {
                return m_lProductCreated;
            }

            set
            {
                m_lProductCreated = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lProductRevised
        {
            get
            {
                return m_lProductRevised;
            }

            set
            {
                m_lProductRevised = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ccpText
        {
            get
            {
                return m_ccpText;
            }

            set
            {
                m_ccpText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ccpFtn
        {
            get
            {
                return m_ccpFtn;
            }

            set
            {
                m_ccpFtn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ccpHdr
        {
            get
            {
                return m_ccpHdd;
            }

            set
            {
                m_ccpHdd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ccpMcr
        {
            get
            {
                return m_ccpMcr;
            }

            set
            {
                m_ccpMcr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ccpAtn
        {
            get
            {
                return m_ccpAtn;
            }

            set
            {
                m_ccpAtn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ccpEdn
        {
            get
            {
                return m_ccpEdn;
            }

            set
            {
                m_ccpEdn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ccpTxbx
        {
            get
            {
                return m_ccpTxbx;
            }

            set
            {
                m_ccpTxbx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int ccpHdrTxbx
        {
            get
            {
                return m_ccpHdrTxbx;
            }

            set
            {
                m_ccpHdrTxbx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int pnFbpChpFirst
        {
            get
            {
                return m_pnFbpChpFirst;
            }

            set
            {
                m_pnFbpChpFirst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int pnFbpPapFirst
        {
            get
            {
                return m_pnFbpPapFirst;
            }

            set
            {
                m_pnFbpPapFirst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int pnFbpLvcFirst
        {
            get
            {
                return m_pnFbpLvcFirst;
            }

            set
            {
                m_pnFbpLvcFirst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int pnLvcFirst
        {
            get
            {
                return m_pnLvcFirst;
            }

            set
            {
                m_pnLvcFirst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int cpnBteLvc
        {
            get
            {
                return m_cpnBteLvc;
            }

            set
            {
                m_cpnBteLvc = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcIslandFirst
        {
            get
            {
                return m_fcIslandFirst;
            }

            set
            {
                m_fcIslandFirst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcIslandLim
        {
            get
            {
                return m_fcIslandLim;
            }

            set
            {
                m_fcIslandLim = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort fibVersion
        {
            get
            {
                return m_fibVersion;
            }

            set
            {
                m_fibVersion = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfFldHdr
        {
            get
            {
                return m_lcbPlcfFldHdr;
            }

            set
            {
                m_lcbPlcfFldHdr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcffldFtn
        {
            get
            {
                return m_fcPlcffldFtn;
            }

            set
            {
                m_fcPlcffldFtn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcffldFtn
        {
            get
            {
                return m_lcbPlcffldFtn;
            }

            set
            {
                m_lcbPlcffldFtn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcffldAtn
        {
            get
            {
                return m_fcPlcffldAtn;
            }

            set
            {
                m_fcPlcffldAtn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcffldAtn
        {
            get
            {
                return m_lcbPlcffldAtn;
            }

            set
            {
                m_lcbPlcffldAtn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcffldMcr
        {
            get
            {
                return m_fcPlcffldMcr;
            }

            set
            {
                m_fcPlcffldMcr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcffldMcr
        {
            get
            {
                return m_lcbPlcffldMcr;
            }

            set
            {
                m_lcbPlcffldMcr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfbkmk
        {
            get
            {
                return m_fcSttbfbkmk;
            }

            set
            {
                m_fcSttbfbkmk = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfbkmk
        {
            get
            {
                return m_lcbSttbfbkmk;
            }

            set
            {
                m_lcbSttbfbkmk = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfbkf
        {
            get
            {
                return m_fcPlcfbkf;
            }

            set
            {
                m_fcPlcfbkf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfandRef
        {
            get
            {
                return m_lcbPlcfandRef;
            }

            set
            {
                m_lcbPlcfandRef = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfandTxt
        {
            get
            {
                return m_fcPlcfandTxt;
            }

            set
            {
                m_fcPlcfandTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfandTxt
        {
            get
            {
                return m_lcbPlcfandTxt;
            }

            set
            {
                m_lcbPlcfandTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfsed
        {
            get
            {
                return m_fcPlcfsed;
            }

            set
            {
                m_fcPlcfsed = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfsed
        {
            get
            {
                return m_lcbPlcfsed;
            }

            set
            {
                m_lcbPlcfsed = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfpad
        {
            get
            {
                return m_fcPlcfpad;
            }

            set
            {
                m_fcPlcfpad = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfpad
        {
            get
            {
                return m_lcbPlcfpad;
            }

            set
            {
                m_lcbPlcfpad = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfphe
        {
            get
            {
                return m_fcPlcfphe;
            }

            set
            {
                m_fcPlcfphe = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfphe
        {
            get
            {
                return m_lcbPlcfphe;
            }

            set
            {
                m_lcbPlcfphe = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfglsy
        {
            get
            {
                return m_fcSttbfglsy;
            }

            set
            {
                m_fcSttbfglsy = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfglsy
        {
            get
            {
                return m_lcbSttbfglsy;
            }

            set
            {
                m_lcbSttbfglsy = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfglsy
        {
            get
            {
                return m_fcPlcfglsy;
            }

            set
            {
                m_fcPlcfglsy = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfglsy
        {
            get
            {
                return m_lcbPlcfglsy;
            }

            set
            {
                m_lcbPlcfglsy = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfhdd
        {
            get
            {
                return m_fcPlcfhdd;
            }

            set
            {
                m_fcPlcfhdd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfhdd
        {
            get
            {
                return m_lcbPlcfhdd;
            }

            set
            {
                m_lcbPlcfhdd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfbteChpx
        {
            get
            {
                return m_fcPlcfbteChpx;
            }

            set
            {
                m_fcPlcfbteChpx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfbteChpx
        {
            get
            {
                return m_lcbPlcfbteChpx;
            }

            set
            {
                m_lcbPlcfbteChpx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfbtePapx
        {
            get
            {
                return m_fcPlcfbtePapx;
            }

            set
            {
                m_fcPlcfbtePapx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfbtePapx
        {
            get
            {
                return m_lcbPlcfbtePapx;
            }

            set
            {
                m_lcbPlcfbtePapx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfsea
        {
            get
            {
                return m_fcPlcfsea;
            }

            set
            {
                m_fcPlcfsea = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfsea
        {
            get
            {
                return m_lcbPlcfsea;
            }

            set
            {
                m_lcbPlcfsea = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfffn
        {
            get
            {
                return m_fcSttbfffn;
            }
            set
            {
                m_fcSttbfffn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfffn
        {
            get
            {
                return m_lcbSttbfffn;
            }
            set
            {
                m_lcbSttbfffn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfFldMom
        {
            get
            {
                return m_fcPlcfFldMom;
            }

            set
            {
                m_fcPlcfFldMom = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfFldMom
        {
            get
            {
                return m_lcbPlcfFldMom;
            }

            set
            {
                m_lcbPlcfFldMom = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfFldHdr
        {
            get
            {
                return m_fcPlcfFldHdr;
            }

            set
            {
                m_fcPlcfFldHdr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort nProduct
        {
            get
            {
                return m_nProduct;
            }

            set
            {
                m_nProduct = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbAutosaveSource
        {
            get
            {
                return m_lcbAutosaveSource;
            }

            set
            {
                m_lcbAutosaveSource = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcGrpXstAtnOwners
        {
            get
            {
                return m_fcGrpXstAtnOwners;
            }

            set
            {
                m_fcGrpXstAtnOwners = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbGrpXstAtnOwners
        {
            get
            {
                return m_lcbGrpXstAtnOwners;
            }

            set
            {
                m_lcbGrpXstAtnOwners = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfAtnbkmk
        {
            get
            {
                return m_fcSttbfAtnbkmk;
            }

            set
            {
                m_fcSttbfAtnbkmk = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfAtnbkmk
        {
            get
            {
                return m_lcbfcSttbfAtnbkmk;
            }

            set
            {
                m_lcbfcSttbfAtnbkmk = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcdoaMom
        {
            get
            {
                return m_fcPlcdoaMom;
            }

            set
            {
                m_fcPlcdoaMom = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcdoaMom
        {
            get
            {
                return m_lcbPlcdoaMom;
            }

            set
            {
                m_lcbPlcdoaMom = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcdoaHdr
        {
            get
            {
                return m_fcPlcdoaHdr;
            }

            set
            {
                m_fcPlcdoaHdr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcdoaHdr
        {
            get
            {
                return m_lcbPlcdoaHdr;
            }

            set
            {
                m_lcbPlcdoaHdr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcspaMom
        {
            get
            {
                return m_fcPlcspaMom;
            }

            set
            {
                m_fcPlcspaMom = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfbkf
        {
            get
            {
                return m_lcbPlcfbkf;
            }

            set
            {
                m_lcbPlcfbkf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfbkl
        {
            get
            {
                return m_fcPlcfbkl;
            }

            set
            {
                m_fcPlcfbkl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfbkl
        {
            get
            {
                return m_lcbPlcfbkl;
            }

            set
            {
                m_lcbPlcfbkl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcCmds
        {
            get
            {
                return m_fcCmds;
            }

            set
            {
                m_fcCmds = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbCmds
        {
            get
            {
                return m_lcbCmds;
            }

            set
            {
                m_lcbCmds = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcmcr
        {
            get
            {
                return m_fcPlcmcr;
            }

            set
            {
                m_fcPlcmcr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcmcr
        {
            get
            {
                return m_lcbPlcmcr;
            }

            set
            {
                m_lcbPlcmcr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfmcr
        {
            get
            {
                return m_fcSttbfmcr;
            }

            set
            {
                m_fcSttbfmcr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfmcr
        {
            get
            {
                return m_lcbSttbfmcr;
            }
            set
            {
                m_lcbSttbfmcr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPrDrvr
        {
            get
            {
                return m_fcPrDrvr;
            }

            set
            {
                m_fcPrDrvr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPrDrvr
        {
            get
            {
                return m_lcbPrDrvr;
            }

            set
            {
                m_lcbPrDrvr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPrEnvPort
        {
            get
            {
                return m_fcPrEnvPort;
            }

            set
            {
                m_fcPrEnvPort = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPrEnvPort
        {
            get
            {
                return m_lcbPrEnvPort;
            }

            set
            {
                m_lcbPrEnvPort = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPrEnvLand
        {
            get
            {
                return m_fcPrEnvLand;
            }

            set
            {
                m_fcPrEnvLand = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPrEnvLand
        {
            get
            {
                return m_lcbPrEnvLand;
            }

            set
            {
                m_lcbPrEnvLand = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcWss
        {
            get
            {
                return m_fcWss;
            }

            set
            {
                m_fcWss = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbWss
        {
            get
            {
                return m_lcbWss;
            }

            set
            {
                m_lcbWss = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcDop
        {
            get
            {
                return m_fcDop;
            }

            set
            {
                m_fcDop = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbDop
        {
            get
            {
                return m_lcbDop;
            }

            set
            {
                m_lcbDop = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfAssoc
        {
            get
            {
                return m_fcSttbfAssoc;
            }

            set
            {
                m_fcSttbfAssoc = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfAssoc
        {
            get
            {
                return m_lcbSttbfAssoc;
            }

            set
            {
                m_lcbSttbfAssoc = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcClx
        {
            get
            {
                return m_fcClx;
            }

            set
            {
                m_fcClx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbClx
        {
            get
            {
                return m_lcbClx;
            }

            set
            {
                m_lcbClx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfpgdFtn
        {
            get
            {
                return m_fcPlcfpgdFtn;
            }

            set
            {
                m_fcPlcfpgdFtn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfpgdFtn
        {
            get
            {
                return m_lcbPlcfpgdFtn;
            }

            set
            {
                m_lcbPlcfpgdFtn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcAutosaveSource
        {
            get
            {
                return m_fcAutosaveSource;
            }

            set
            {
                m_fcAutosaveSource = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal short lid
        {
            get
            {
                return m_lid;
            }

            set
            {
                m_lid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfAutoCaption
        {
            get
            {
                return m_lcbSttbfAutoCaption;
            }

            set
            {
                m_lcbSttbfAutoCaption = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfwkb
        {
            get
            {
                return m_fcPlcfwkb;
            }

            set
            {
                m_fcPlcfwkb = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfwkb
        {
            get
            {
                return m_lcbPlcfwkb;
            }

            set
            {
                m_lcbPlcfwkb = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfspl
        {
            get
            {
                return m_fcPlcfspl;
            }

            set
            {
                m_fcPlcfspl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfspl
        {
            get
            {
                return m_lcbPlcfspl;
            }

            set
            {
                m_lcbPlcfspl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfgram
        {
            get
            {
                return m_fcPlcfgram;
            }

            set
            {
                m_fcPlcfgram = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfgram
        {
            get
            {
                return m_lcbPlcfgram;
            }

            set
            {
                m_lcbPlcfgram = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcftxbxTxt
        {
            get
            {
                return m_fcPlcftxbxTxt;
            }

            set
            {
                m_fcPlcftxbxTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcftxbxTxt
        {
            get
            {
                return m_lcbPlcftxbxTxt;
            }

            set
            {
                m_lcbPlcftxbxTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcffldTxbx
        {
            get
            {
                return m_fcPlcffldTxbx;
            }

            set
            {
                m_fcPlcffldTxbx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcffldTxbx
        {
            get
            {
                return m_lcbPlcffldTxbx;
            }

            set
            {
                m_lcbPlcffldTxbx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfHdrtxbxTxt
        {
            get
            {
                return m_fcPlcfHdrtxbxTxt;
            }

            set
            {
                m_fcPlcfHdrtxbxTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcspaMom
        {
            get
            {
                return m_lcbPlcspaMom;
            }

            set
            {
                m_lcbPlcspaMom = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcspaHdr
        {
            get
            {
                return m_fcPlcspaHdr;
            }

            set
            {
                m_fcPlcspaHdr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcspaHdr
        {
            get
            {
                return m_lcbPlcspaHdr;
            }

            set
            {
                m_lcbPlcspaHdr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfAtnbkf
        {
            get
            {
                return m_fcPlcfAtnbkf;
            }

            set
            {
                m_fcPlcfAtnbkf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfAtnbkf
        {
            get
            {
                return m_lcbPlcfAtnbkf;
            }

            set
            {
                m_lcbPlcfAtnbkf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfAtnbkl
        {
            get
            {
                return m_fcPlcfAtnbkl;
            }

            set
            {
                m_fcPlcfAtnbkl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfAtnbkl
        {
            get
            {
                return m_lcbPlcfAtnbkl;
            }

            set
            {
                m_lcbPlcfAtnbkl = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPms
        {
            get
            {
                return m_fcPms;
            }

            set
            {
                m_fcPms = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPms
        {
            get
            {
                return m_lcbPms;
            }
            set
            {
                m_lcbPms = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcFormFldSttbf
        {
            get
            {
                return m_fcFormFldSttbf;
            }
            set
            {
                m_fcFormFldSttbf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbFormFldSttbf
        {
            get
            {
                return m_lcbFormFldSttbf;
            }

            set
            {
                m_lcbFormFldSttbf = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfendRef
        {
            get
            {
                return m_fcPlcfendRef;
            }

            set
            {
                m_fcPlcfendRef = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfendRef
        {
            get
            {
                return m_lcbPlcfendRef;
            }

            set
            {
                m_lcbPlcfendRef = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfendTxt
        {
            get
            {
                return m_fcPlcfendTxt;
            }

            set
            {
                m_fcPlcfendTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfendTxt
        {
            get
            {
                return m_lcbPlcfendTxt;
            }

            set
            {
                m_lcbPlcfendTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcffldEdn
        {
            get
            {
                return m_fcPlcffldEdn;
            }

            set
            {
                m_fcPlcffldEdn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcffldEdn
        {
            get
            {
                return m_lcbPlcffldEdn;
            }

            set
            {
                m_lcbPlcffldEdn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfpgdEdn
        {
            get
            {
                return m_fcPlcfpgdEdn;
            }

            set
            {
                m_fcPlcfpgdEdn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfpgdEdn
        {
            get
            {
                return m_lcbPlcfpgdEdn;
            }

            set
            {
                m_lcbPlcfpgdEdn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcDggInfo
        {
            get
            {
                return m_fcDggInfo;
            }

            set
            {
                m_fcDggInfo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbDggInfo
        {
            get
            {
                return m_lcbDggInfo;
            }

            set
            {
                m_lcbDggInfo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfRMark
        {
            get
            {
                return m_fcSttbfRMark;
            }

            set
            {
                m_fcSttbfRMark = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfRMark
        {
            get
            {
                return m_lcbSttbfRMark;
            }

            set
            {
                m_lcbSttbfRMark = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfCaption
        {
            get
            {
                return m_fcSttbfCaption;
            }

            set
            {
                m_fcSttbfCaption = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbfCaption
        {
            get
            {
                return m_lcbSttbfCaption;
            }

            set
            {
                m_lcbSttbfCaption = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbfAutoCaption
        {
            get
            {
                return m_fcSttbfAutoCaption;
            }

            set
            {
                m_fcSttbfAutoCaption = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal short pnNext
        {
            get
            {
                return m_pnNext;
            }

            set
            {
                m_pnNext = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfHdrtxbxTxt
        {
            get
            {
                return m_lcbPlcfHdrtxbxTxt;
            }

            set
            {
                m_lcbPlcfHdrtxbxTxt = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcffldHdrTxbx
        {
            get
            {
                return m_fcPlcffldHdrTxbx;
            }

            set
            {
                m_fcPlcffldHdrTxbx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcffldHdrTxbx
        {
            get
            {
                return m_lcbPlcffldHdrTxbx;
            }

            set
            {
                m_lcbPlcffldHdrTxbx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbttmbd
        {
            get
            {
                return m_fcSttbttmbd;
            }

            set
            {
                m_fcSttbttmbd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbttmbd
        {
            get
            {
                return m_lcbSttbttmbd;
            }

            set
            {
                m_lcbSttbttmbd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcUnused
        {
            get
            {
                return m_fcUnused;
            }

            set
            {
                m_fcUnused = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbUnused
        {
            get
            {
                return m_lcbUnused;
            }

            set
            {
                m_lcbUnused = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfLst
        {
            get
            {
                return m_fcPlcfLst;
            }

            set
            {
                m_fcPlcfLst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfLst
        {
            get
            {
                return m_lcbPlcfLst;
            }

            set
            {
                m_lcbPlcfLst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlfLfo
        {
            get
            {
                return m_fcPlfLfo;
            }

            set
            {
                m_fcPlfLfo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlfLfo
        {
            get
            {
                return m_lcbPlfLfo;
            }

            set
            {
                m_lcbPlfLfo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcftxbxBkd
        {
            get
            {
                return m_fcPlcftxbxBkd;
            }

            set
            {
                m_fcPlcftxbxBkd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcftxbxBkd
        {
            get
            {
                return m_lcbPlcftxbxBkd;
            }

            set
            {
                m_lcbPlcftxbxBkd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfHdrtxbxBkd
        {
            get
            {
                return m_fcPlcfHdrtxbxBkd;
            }

            set
            {
                m_fcPlcfHdrtxbxBkd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfHdrtxbxBkd
        {
            get
            {
                return m_lcbPlcfHdrtxbxBkd;
            }

            set
            {
                m_lcbPlcfHdrtxbxBkd = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcocx
        {
            get
            {
                return m_fcPlcocx;
            }

            set
            {
                m_fcPlcocx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcocx
        {
            get
            {
                return m_lcbPlcocx;
            }

            set
            {
                m_lcbPlcocx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcfbteLvc
        {
            get
            {
                return m_fcPlcfbteLvc;
            }

            set
            {
                m_fcPlcfbteLvc = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcfbteLvc
        {
            get
            {
                return m_lcbPlcfbteLvc;
            }

            set
            {
                m_lcbPlcfbteLvc = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcSttbListNames
        {
            get
            {
                return m_fcSttbListNames;
            }

            set
            {
                m_fcSttbListNames = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbSttbListNames
        {
            get
            {
                return m_lcbSttbListNames;
            }

            set
            {
                m_lcbSttbListNames = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int pnChpFirst
        {
            get
            {
                return m_pnChpFirst;
            }

            set
            {
                m_pnChpFirst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int pnPapFirst
        {
            get
            {
                return m_pnPapFirst;
            }

            set
            {
                m_pnPapFirst = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int cpnBteChp
        {
            get
            {
                return m_cpnBteChp;
            }

            set
            {
                m_cpnBteChp = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int cpnBtePap
        {
            get
            {
                return m_cpnBtePap;
            }

            set
            {
                m_cpnBtePap = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int version
        {
            get
            {
                return m_version;
            }

            set
            {
                m_version = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsDocumentTemplate
        {
            get
            {
                return m_fDot;
            }

            set
            {
                m_fDot = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fGlsy
        {
            get
            {
                return m_fGlsy;
            }

            set
            {
                m_fGlsy = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fComplex
        {
            get
            {
                return m_fComplex;
            }

            set
            {
                m_fComplex = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fHasPic
        {
            get
            {
                return m_fHasPic;
            }

            set
            {
                m_fHasPic = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int cQuickSaves
        {
            get
            {
                return m_cQuickSaves;
            }

            set
            {
                m_cQuickSaves = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fEncrypted
        {
            get
            {
                return m_fEncrypted;
            }

            set
            {
                m_fEncrypted = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fWhichTblStm
        {
            get
            {
                return m_fWhichTblStm;
            }

            set
            {
                m_fWhichTblStm = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fExtChar
        {
            get
            {
                return m_fExtChar;
            }

            set
            {
                m_fExtChar = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort nFibBack
        {
            get
            {
                return m_nFibBack;
            }

            set
            {
                m_nFibBack = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal uint lKey
        {
            get
            {
                return m_lKey;
            }

            set
            {
                m_lKey = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort lKey2
        {
            get
            {
                return m_lKey2;
            }

            set
            {
                m_lKey2 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal byte envr
        {
            get
            {
                return m_envr;
            }

            set
            {
                m_envr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fMac
        {
            get
            {
                return m_fMac;
            }

            set
            {
                m_fMac = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fEmptySpecial
        {
            get
            {
                return m_fEmptySpecial;
            }

            set
            {
                m_fEmptySpecial = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fLoadOverridePage
        {
            get
            {
                return m_fLoadOverridePage;
            }

            set
            {
                m_fLoadOverridePage = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fFutureSavedUndo
        {
            get
            {
                return m_fFutureSavedUndo;
            }

            set
            {
                m_fFutureSavedUndo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fWord97Saved
        {
            get
            {
                return m_fWord97Saved;
            }

            set
            {
                m_fWord97Saved = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool fSpare0
        {
            get
            {
                return m_fSpare0;
            }

            set
            {
                m_fSpare0 = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort chs
        {
            get
            {
                return m_chs;
            }

            set
            {
                m_chs = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal ushort chsTables
        {
            get
            {
                return m_chsTables;
            }

            set
            {
                m_chsTables = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcMin
        {
            get
            {
                return m_fcMin;
            }

            set
            {
                m_fcMin = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int fcPlcflvc
        {
            get
            {
                return m_fcPlcflvc;
            }

            set
            {
                m_fcPlcflvc = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int lcbPlcflvc
        {
            get
            {
                return m_lcbPlcflvc;
            }

            set
            {
                m_lcbPlcflvc = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [if read only recommended].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [if read only recommended]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Specifies that applications should provide user interface 
        /// recommending that the user open this document in write protected state.
        /// </remarks>
        internal bool fReadOnlyRecommended
        {
            get
            {
                return m_fReadOnlyRecom;
            }
            set
            {
                m_fReadOnlyRecom = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset of macro user storage.
        /// </summary>
        /// <value>The fc STW user.</value>
        internal int fcStwUser
        {
            get
            {
                return m_fcStwUser;
            }

            set
            {
                m_fcStwUser = value;
            }
        }

        /// <summary>
        /// Gets/sets the length of macro user storage.
        /// </summary>
        internal int lcbStwUser
        {
            get
            {
                return m_lcbStwUser;
            }

            set
            {
                m_lcbStwUser = value;
            }
        }

        /// <summary>
        /// Gets or sets the CSW new.
        /// </summary>
        /// <value>The CSW new.</value>
        internal ushort cswNew
        {
            get
            {
                return m_cswNew;
            }
            set
            {
                m_cswNew = value;
            }
        }

        /// <summary>
        /// Gets or sets the fib new.
        /// </summary>
        /// <value>The fib new.</value>
        internal ushort nFibNew
        {
            get
            {
                return m_nFibNew;
            }
            set
            {
                m_nFibNew = value;
            }
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            Read(stream, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            //      byte[] empty = new byte[1022];
            //      stream.Write( empty, 0, empty.Length );
            //      stream.Position = 0;
            CorrectFib();
            BinaryWriter writer = new BinaryWriter(stream);

            WriteNotEncryptedFib(writer);

            writer.Write(m_cbMac);
            writer.Write(m_lProductCreated);
            writer.Write(m_lProductRevised);
            writer.Write(m_ccpText);
            writer.Write(m_ccpFtn);
            writer.Write(m_ccpHdd);
            writer.Write(m_ccpMcr);
            writer.Write(m_ccpAtn);
            writer.Write(m_ccpEdn);
            writer.Write(m_ccpTxbx);
            writer.Write(m_ccpHdrTxbx);
            writer.Write(m_pnFbpChpFirst);
            writer.Write(m_pnChpFirst);
            writer.Write(m_cpnBteChp);
            writer.Write(m_pnFbpPapFirst);
            writer.Write(m_pnPapFirst);
            writer.Write(m_cpnBtePap);
            writer.Write(m_pnFbpLvcFirst);
            writer.Write(m_pnLvcFirst);
            writer.Write(m_cpnBteLvc);
            writer.Write(m_fcIslandFirst);
            writer.Write(m_fcIslandLim);
            writer.Write(m_cfclcb);
            writer.Write(m_fcStshfOrig);
            writer.Write(m_lcbStshfOrig);
            writer.Write(m_fcStshf);
            writer.Write(m_lcbStshf);
            writer.Write(m_fcPlcffndRef);
            writer.Write(m_lcbPlcffndRef);
            writer.Write(m_fcPlcffndTxt);
            writer.Write(m_lcbPlcffndTxt);
            writer.Write(m_fcPlcfandRef);
            writer.Write(m_lcbPlcfandRef);
            writer.Write(m_fcPlcfandTxt);
            writer.Write(m_lcbPlcfandTxt);
            writer.Write(m_fcPlcfsed);
            writer.Write(m_lcbPlcfsed);
            writer.Write(m_fcPlcfpad);
            writer.Write(m_lcbPlcfpad);
            writer.Write(m_fcPlcfphe);
            writer.Write(m_lcbPlcfphe);
            writer.Write(m_fcSttbfglsy);
            writer.Write(m_lcbSttbfglsy);
            writer.Write(m_fcPlcfglsy);
            writer.Write(m_lcbPlcfglsy);
            writer.Write(m_fcPlcfhdd);
            writer.Write(m_lcbPlcfhdd);
            writer.Write(m_fcPlcfbteChpx);
            writer.Write(m_lcbPlcfbteChpx);
            writer.Write(m_fcPlcfbtePapx);
            writer.Write(m_lcbPlcfbtePapx);
            writer.Write(m_fcPlcfsea);
            writer.Write(m_lcbPlcfsea);
            writer.Write(m_fcSttbfffn);
            writer.Write(m_lcbSttbfffn);
            writer.Write(m_fcPlcfFldMom);
            writer.Write(m_lcbPlcfFldMom);
            writer.Write(m_fcPlcfFldHdr);
            writer.Write(m_lcbPlcfFldHdr);
            writer.Write(m_fcPlcffldFtn);
            writer.Write(m_lcbPlcffldFtn);
            writer.Write(m_fcPlcffldAtn);
            writer.Write(m_lcbPlcffldAtn);
            writer.Write(m_fcPlcffldMcr);
            writer.Write(m_lcbPlcffldMcr);
            writer.Write(m_fcSttbfbkmk);
            writer.Write(m_lcbSttbfbkmk);
            writer.Write(m_fcPlcfbkf);
            writer.Write(m_lcbPlcfbkf);
            writer.Write(m_fcPlcfbkl);
            writer.Write(m_lcbPlcfbkl);
            writer.Write(m_fcCmds);
            writer.Write(m_lcbCmds);
            writer.Write(m_fcPlcmcr);
            writer.Write(m_lcbPlcmcr);
            writer.Write(m_fcSttbfmcr);
            writer.Write(m_lcbSttbfmcr);
            writer.Write(m_fcPrDrvr);
            writer.Write(m_lcbPrDrvr);
            writer.Write(m_fcPrEnvPort);
            writer.Write(m_lcbPrEnvPort);
            writer.Write(m_fcPrEnvLand);
            writer.Write(m_lcbPrEnvLand);
            writer.Write(m_fcWss);
            writer.Write(m_lcbWss);
            writer.Write(m_fcDop);
            writer.Write(m_lcbDop);
            writer.Write(m_fcSttbfAssoc);
            writer.Write(m_lcbSttbfAssoc);
            writer.Write(m_fcClx);
            writer.Write(m_lcbClx);
            writer.Write(m_fcPlcfpgdFtn);
            writer.Write(m_lcbPlcfpgdFtn);
            writer.Write(m_fcAutosaveSource);
            writer.Write(m_lcbAutosaveSource);
            writer.Write(m_fcGrpXstAtnOwners);
            writer.Write(m_lcbGrpXstAtnOwners);
            writer.Write(m_fcSttbfAtnbkmk);
            writer.Write(m_lcbfcSttbfAtnbkmk);
            writer.Write(m_fcPlcdoaMom);
            writer.Write(m_lcbPlcdoaMom);
            writer.Write(m_fcPlcdoaHdr);
            writer.Write(m_lcbPlcdoaHdr);
            writer.Write(m_fcPlcspaMom);
            writer.Write(m_lcbPlcspaMom);
            writer.Write(m_fcPlcspaHdr);
            writer.Write(m_lcbPlcspaHdr);
            writer.Write(m_fcPlcfAtnbkf);
            writer.Write(m_lcbPlcfAtnbkf);
            writer.Write(m_fcPlcfAtnbkl);
            writer.Write(m_lcbPlcfAtnbkl);
            writer.Write(m_fcPms);
            writer.Write(m_lcbPms);
            writer.Write(m_fcFormFldSttbf);
            writer.Write(m_lcbFormFldSttbf);
            writer.Write(m_fcPlcfendRef);
            writer.Write(m_lcbPlcfendRef);
            writer.Write(m_fcPlcfendTxt);
            writer.Write(m_lcbPlcfendTxt);
            writer.Write(m_fcPlcffldEdn);
            writer.Write(m_lcbPlcffldEdn);
            writer.Write(m_fcPlcfpgdEdn);
            writer.Write(m_lcbPlcfpgdEdn);
            writer.Write(m_fcDggInfo);
            writer.Write(m_lcbDggInfo);
            writer.Write(m_fcSttbfRMark);
            writer.Write(m_lcbSttbfRMark);
            writer.Write(m_fcSttbfCaption);
            writer.Write(m_lcbSttbfCaption);
            writer.Write(m_fcSttbfAutoCaption);
            writer.Write(m_lcbSttbfAutoCaption);
            writer.Write(m_fcPlcfwkb);
            writer.Write(m_lcbPlcfwkb);
            writer.Write(m_fcPlcfspl);
            writer.Write(m_lcbPlcfspl);
            writer.Write(m_fcPlcftxbxTxt);
            writer.Write(m_lcbPlcftxbxTxt);
            writer.Write(m_fcPlcffldTxbx);
            writer.Write(m_lcbPlcffldTxbx);
            writer.Write(m_fcPlcfHdrtxbxTxt);
            writer.Write(m_lcbPlcfHdrtxbxTxt);
            writer.Write(m_fcPlcffldHdrTxbx);
            writer.Write(m_lcbPlcffldHdrTxbx);
            writer.Write(m_fcStwUser);
            writer.Write(m_lcbStwUser);
            writer.Write(m_fcSttbttmbd);
            writer.Write(m_lcbSttbttmbd);
            writer.Write(m_fcUnused);
            writer.Write(m_lcbUnused);

            //Added
            writer.Write(m_fcPgdMother);
            writer.Write(m_lcbPgdMother);
            writer.Write(m_fcBkdMother);
            writer.Write(m_lcbBkdMother);
            writer.Write(m_fcPgdFtn);
            writer.Write(m_lcbPgdFtn);
            writer.Write(m_fcBkdFtn);
            writer.Write(m_lcbBkdFtn);
            writer.Write(m_fcPgdEdn);
            writer.Write(m_lcbPgdEdn);
            writer.Write(m_fcBkdEdn);
            writer.Write(m_lcbBkdEdn);
            writer.Write(m_fcSttbfIntlFld);
            writer.Write(m_lcbSttbfIntlFld);
            writer.Write(m_fcRouteSlip);
            writer.Write(m_lcbRouteSlip);
            writer.Write(m_fcSttbSavedBy);
            writer.Write(m_lcbSttbSavedBy);
            writer.Write(m_fcSttbFnm);
            writer.Write(m_lcbSttbFnm);
            //      byte[] empty = new byte[ 0x2e2 - stream.Position ];
            //      stream.Write( empty, 0, empty.Length );

            writer.Write(m_fcPlcfLst);
            writer.Write(m_lcbPlcfLst);
            writer.Write(m_fcPlfLfo);
            writer.Write(m_lcbPlfLfo);
            writer.Write(m_fcPlcftxbxBkd);
            writer.Write(m_lcbPlcftxbxBkd);
            writer.Write(m_fcPlcfHdrtxbxBkd);
            writer.Write(m_lcbPlcfHdrtxbxBkd);

            //Added
            writer.Write(m_fcDocUndo);
            writer.Write(m_lcbDocUndo);
            writer.Write(m_fcRgbuse);
            writer.Write(m_lcbRgbuse);
            writer.Write(m_fcUsp);
            writer.Write(m_lcbUsp);
            writer.Write(m_fcUskf);
            writer.Write(m_lcbUskf);
            writer.Write(m_fcPlcupcRgbuse);
            writer.Write(m_lcbPlcupcRgbuse);
            writer.Write(m_fcPlcupcUsp);
            writer.Write(m_lcbPlcupcUsp);
            writer.Write(m_fcSttbGlsyStyle);
            writer.Write(m_lcbSttbGlsyStyle);
            writer.Write(m_fcPlgosl);
            writer.Write(m_lcbPlgosl);
            //      empty = new byte[ 0x342 - stream.Position ];
            //      stream.Write( empty, 0, empty.Length );

            writer.Write(m_fcPlcocx);
            writer.Write(m_lcbPlcocx);
            writer.Write(m_fcPlcfbteLvc);
            writer.Write(m_lcbPlcfbteLvc);

            //      writer.Seek( 16, SeekOrigin.Current );

            //Added
            writer.Write(m_dwLowDateTime);
            writer.Write(m_wHighDateTime);
            writer.Write(m_fcPlcflvc);
            writer.Write(m_lcbPlcflvc);
            writer.Write(m_fcPlcasumy);
            writer.Write(m_lcbPlcasumy);
            writer.Write(m_fcPlcfgram);
            writer.Write(m_lcbPlcfgram);
            //      empty = new byte[ 0x372 - stream.Position ];
            //      stream.Write( empty, 0, empty.Length );

            writer.Write(m_fcSttbListNames);
            writer.Write(m_lcbSttbListNames);
            writer.Write(m_fcSttbfUssr);
            writer.Write(m_lcbSttbfUssr);

            byte[] empty = new byte[0x3fa - stream.Position];
            stream.Write(empty, 0, empty.Length);

            writer.Write((ushort)2);
            writer.Write((ushort)0xd9);
        }

        /// <summary>
        /// Reads fib record form decrypted stream.
        /// </summary>
        /// <param name="stream"></param>
        internal void ReadDecrypted(Stream stream)
        {
            Read(stream, true);
        }

        /// <summary>
        /// Writes the decrypted.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void WriteDecrypted(MemoryStream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Seek(0, SeekOrigin.Begin);
            WriteNotEncryptedFib(writer);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Updates the fcMac.
        /// </summary>
        internal void UpdateFcMac()
        {
            fcMac = fcMin
              + (ccpText + ccpFtn + ccpHdr + ccpAtn + ccpEdn + ccpTxbx + ccpHdrTxbx)
                * EncodingCharSize;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CorrectFib()
        {
            if (m_ccpHdrTxbx > 0)
            {
                m_ccpHdrTxbx--;
            }
            else if (m_ccpTxbx > 0)
            {
                m_ccpTxbx--;
            }
            else if (m_ccpEdn > 0)
            {
                m_ccpEdn--;
            }
            else if (m_ccpAtn > 0)
            {
                m_ccpAtn--;
            }
            else if (m_ccpHdd > 0)
            {
                m_ccpHdd--;
            }
            else if (m_ccpFtn > 0)
            {
                m_ccpFtn--;
            }
        }

        //    /// <summary>
        //    /// Generates default FIB record data.
        //    /// </summary>
        //    private void GenerateFIB()
        //    {
        //      m_fib = new FIBRecord();
        //
        //      m_fib.wIdent = 42476;
        //      m_fib.nFib = 193;
        //      m_fib.nProduct = 16437;
        //      m_fib.lid = 1049;
        //      m_fib.pnNext = 0;
        //      m_fib.IsDot = false;
        //      m_fib.IsGlosary = false;
        //      m_fib.IsComplex = false;
        //      m_fib.HasPicture = false;
        //      m_fib.QuickSavesCount = 15;
        //      m_fib.IsEncrypted = false;
        //      m_fib.IsUse1Table = true;
        //      m_fib.IsReadOnlyRecommended = false;
        //
        //      m_fib.IsWriteReservation = false;
        //      m_fib.IsExtendedChar = true;
        //      m_fib.IsLoadOverride = false;
        //      m_fib.IsFarEast = false;
        //      m_fib.IsCrypto = false;
        //      m_fib.nFibBack = 191;
        //      m_fib.lKey = 0;
        //      m_fib.envr = 0;
        //      m_fib.IsMac = false;
        //      m_fib.IsEmptySpecial = false;
        //      m_fib.fLoadOverridePage = false;
        //      m_fib.IsFutureSavedUndo = false;
        //      m_fib.IsWord97Saved = true;
        //      m_fib.Spare0 = 0;
        //      m_fib.chs = 0;
        //      m_fib.chsTables = 0;
        ////      m_fib.fcMin	 = 1536;
        //      m_fib.fcMin = 2048;
        //
        //      //---------------------------------------------------------------
        //      // Filling array of shorts
        //      //---------------------------------------------------------------
        //      m_fib.csw = 14;
        //      m_fib.wMagicCreated = 27234;
        //      m_fib.wMagicRevised = 27234;
        //      m_fib.wMagicCreatedPrivate = 13007;
        //      m_fib.wMagicRevisedPrivate = 13007;
        //      m_fib.pnFbpChpFirst_W6 = 0;
        //      m_fib.pnChpFirst_W6 = 0;
        //      m_fib.cpnBteChp_W6 = 0;
        //      m_fib.pnFbpPapFirst_W6 = 0;
        //      m_fib.pnPapFirst_W6 = 0;
        //      m_fib.cpnBtePap_W6 = 0;
        //      m_fib.pnFbpLvcFirst_W6 = 0;
        //      m_fib.pnLvcFirst_W6 = 0;
        //      m_fib.cpnBteLvc_W6 = 0;
        //      m_fib.lidFE = 1033;
        //
        //      //---------------------------------------------------------------
        //      // Filling array of longs
        //      //---------------------------------------------------------------
        //      m_fib.clw = 22;
        //      m_fib.lProductCreated = 22701;
        //      m_fib.lProductRevised = 22701;
        //
        //      m_fib.pnFbpChpFirst = 1048575;
        //      m_fib.pnChpFirst = 0;
        //      m_fib.cpnBteChp = 0;
        //      m_fib.pnFbpPapFirst = 1048575;
        //      m_fib.pnPapFirst = 0;
        //      m_fib.cpnBtePap = 0;
        //      m_fib.pnFbpLvcFirst = 1048575;
        //      m_fib.pnLvcFirst = 0;
        //      m_fib.cpnBteLvc = 0;
        //      m_fib.fcIslandFirst = 0;
        //      m_fib.fcIslandLim = 0;
        //
        //      m_fib.dwLowDateTime = 1521308192;
        //      m_fib.dwHighDateTime = 29675898;
        //
        //      //---------------------------------------------------------------
        //      // Filling array of fc / lcb
        //      //---------------------------------------------------------------
        //      m_fib.cfclcb = 93; //136;
        //
        //      // Init length of main text lengths
        //      m_fib.fcMac = 2049;
        //      m_fib.ccpText = 0;
        //      m_fib.ccpHdr = 0;
        //      m_fib.ccpMcr = 0;
        //      m_fib.ccpAtn = 0;
        //      m_fib.ccpEdn = 0;
        //      m_fib.ccpFtn = 0;
        //      m_fib.ccpTxbx = 0;
        //      m_fib.ccpHdrTxbx = 0;
        //
        //      m_fib.wIdent = 0xa5ec;
        //      m_fib.nFib = 0xc2;
        //      m_fib.nFibBack = 0xbf;
        //      m_fib.nProduct = 0x204d;
        //      m_fib.cfclcb = 0x6c;
        //      m_fib.csw = 14;
        //      m_fib.clw = 0x16;
        //
        //      m_fib.pnFbpLvcFirst = 0xfffff;
        //      m_fib.pnFbpPapFirst = 0xfffff;
        //      m_fib.pnFbpChpFirst = 0xfffff;
        //      m_fib.IsExtendedChar = true;
        //      m_fib.Spare0 = 1;
        //
        //      m_fib.wMagicCreated = 0x6143;
        //      m_fib.wMagicRevised = 0x6c6f;
        //      m_fib.wMagicCreatedPrivate = 0x6e61;
        //      m_fib.wMagicRevisedPrivate = 0x3038;
        //      m_fib.lProductCreated = m_fib.lProductRevised = 0x2c8c;
        //      m_fib.lidFE = 0x409;
        //    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="isDecryptedStream"></param>
        private void Read(Stream stream, bool isDecryptedStream)
        {
            BinaryReader reader = new BinaryReader(stream);

            m_wIdent = reader.ReadUInt16();
            m_fibVersion = reader.ReadUInt16();
            if ((m_fibVersion >= 101) && (m_fibVersion <= 105))
            {
                throw new Exception("This file format is not supported");
            }
            else
            {
                m_version = 8;
            }
            m_nProduct = reader.ReadUInt16();
            m_lid = reader.ReadInt16();
            m_pnNext = reader.ReadInt16();

            byte tmp = reader.ReadByte();
            m_fDot = (tmp & 1) != 0;
            m_fGlsy = (tmp & 2) != 0;
            m_fComplex = (tmp & 4) != 0;
            m_fHasPic = (tmp & 8) != 0;
            m_cQuickSaves = (tmp & 240) >> 4;

            tmp = reader.ReadByte();
            m_fEncrypted = (tmp & 1) != 0;
            m_fWhichTblStm = (tmp & 2) != 0;
            m_fReadOnlyRecom = (tmp & 4) != 0;
            m_fWriteReserve = (tmp & 8) != 0;
            m_fExtChar = (tmp & 0x10) != 0;
            m_fLoadOverridePage = (tmp & 0x20) != 0;
            m_fFarEast = (tmp & 0x40) != 0;
            m_fCrypto = (tmp & 0x80) != 0;


            m_nFibBack = reader.ReadUInt16();
            m_lKey = reader.ReadUInt32();
            //m_lKey2 = reader.ReadUInt16();
            m_envr = reader.ReadByte();

            tmp = reader.ReadByte();
            m_fMac = (tmp & 1) != 0;
            m_fEmptySpecial = (tmp & 2) != 0;
            m_fLoadOverridePage = (tmp & 4) != 0;
            m_fFutureSavedUndo = (tmp & 8) != 0;
            m_fWord97Saved = (tmp & 0x10) != 0;
            m_fSpare0 = (tmp & 0x20) != 0;

            m_chs = reader.ReadUInt16();
            m_chsTables = reader.ReadUInt16();
            m_fcMin = reader.ReadInt32();
            m_fcMac = reader.ReadInt32();

            m_csw = reader.ReadUInt16();
            if (isDecryptedStream)
                m_csw = m_prevCsw;
            else
                m_prevCsw = m_csw;

            m_wMagicCreated = reader.ReadUInt16();
            m_wMagicRevised = reader.ReadUInt16();
            m_wMagicCreatedPrivate = reader.ReadUInt16();
            m_wMagicRevisedPrivate = reader.ReadUInt16();

            //Added
            m_pnFbpChpFirst_W6 = reader.ReadUInt16();
            m_pnChpFirst_W6 = reader.ReadUInt16();
            m_cpnBteChp_W6 = reader.ReadUInt16();
            m_pnFbpPapFirst_W6 = reader.ReadUInt16();
            m_pnPapFirst_W6 = reader.ReadUInt16();
            m_cpnBtePap_W6 = reader.ReadUInt16();
            m_pnFbpLvcFirst_W6 = reader.ReadUInt16();
            m_pnLvcFirst_W6 = reader.ReadUInt16();
            m_cpnBteLvc_W6 = reader.ReadUInt16();
            // skip unneeded bytes and go to 60 position
            //      reader.BaseStream.Seek( 0x12, SeekOrigin.Current );

            m_lidFE = reader.ReadInt16();

            m_clw = reader.ReadUInt16();
            if (isDecryptedStream)
                m_clw = m_prevClw;
            else
                m_prevClw = m_clw;

            m_cbMac = reader.ReadInt32();
            m_lProductCreated = reader.ReadInt32();
            m_lProductRevised = reader.ReadInt32();
            m_ccpText = reader.ReadInt32();
            m_ccpFtn = reader.ReadInt32();
            m_ccpHdd = reader.ReadInt32();
            m_ccpMcr = reader.ReadInt32();
            m_ccpAtn = reader.ReadInt32();
            m_ccpEdn = reader.ReadInt32();
            m_ccpTxbx = reader.ReadInt32();
            m_ccpHdrTxbx = reader.ReadInt32();
            m_pnFbpChpFirst = reader.ReadInt32();
            m_pnChpFirst = reader.ReadInt32();
            m_cpnBteChp = reader.ReadInt32();
            m_pnFbpPapFirst = reader.ReadInt32();
            m_pnPapFirst = reader.ReadInt32();
            m_cpnBtePap = reader.ReadInt32();
            m_pnFbpLvcFirst = reader.ReadInt32();
            m_pnLvcFirst = reader.ReadInt32();
            m_cpnBteLvc = reader.ReadInt32();
            m_fcIslandFirst = reader.ReadInt32();
            m_fcIslandLim = reader.ReadInt32();

            m_cfclcb = reader.ReadUInt16();
            int num = (int)reader.BaseStream.Position;

            m_fcStshfOrig = reader.ReadInt32();
            m_lcbStshfOrig = reader.ReadInt32();
            m_fcStshf = reader.ReadInt32();
            m_lcbStshf = reader.ReadInt32();
            m_fcPlcffndRef = reader.ReadInt32();
            m_lcbPlcffndRef = reader.ReadInt32();
            m_fcPlcffndTxt = reader.ReadInt32();
            m_lcbPlcffndTxt = reader.ReadInt32();
            m_fcPlcfandRef = reader.ReadInt32();
            m_lcbPlcfandRef = reader.ReadInt32();
            m_fcPlcfandTxt = reader.ReadInt32();
            m_lcbPlcfandTxt = reader.ReadInt32();
            m_fcPlcfsed = reader.ReadInt32();
            m_lcbPlcfsed = reader.ReadInt32();
            m_fcPlcfpad = reader.ReadInt32();
            m_lcbPlcfpad = reader.ReadInt32();
            m_fcPlcfphe = reader.ReadInt32();
            m_lcbPlcfphe = reader.ReadInt32();
            m_fcSttbfglsy = reader.ReadInt32();
            m_lcbSttbfglsy = reader.ReadInt32();
            m_fcPlcfglsy = reader.ReadInt32();
            m_lcbPlcfglsy = reader.ReadInt32();
            m_fcPlcfhdd = reader.ReadInt32();
            m_lcbPlcfhdd = reader.ReadInt32();
            m_fcPlcfbteChpx = reader.ReadInt32();
            m_lcbPlcfbteChpx = reader.ReadInt32();
            m_fcPlcfbtePapx = reader.ReadInt32();
            m_lcbPlcfbtePapx = reader.ReadInt32();
            m_fcPlcfsea = reader.ReadInt32();
            m_lcbPlcfsea = reader.ReadInt32();
            m_fcSttbfffn = reader.ReadInt32();
            m_lcbSttbfffn = reader.ReadInt32();
            m_fcPlcfFldMom = reader.ReadInt32();
            m_lcbPlcfFldMom = reader.ReadInt32();
            m_fcPlcfFldHdr = reader.ReadInt32();
            m_lcbPlcfFldHdr = reader.ReadInt32();
            m_fcPlcffldFtn = reader.ReadInt32();
            m_lcbPlcffldFtn = reader.ReadInt32();
            m_fcPlcffldAtn = reader.ReadInt32();
            m_lcbPlcffldAtn = reader.ReadInt32();
            m_fcPlcffldMcr = reader.ReadInt32();
            m_lcbPlcffldMcr = reader.ReadInt32();
            m_fcSttbfbkmk = reader.ReadInt32();
            m_lcbSttbfbkmk = reader.ReadInt32();
            m_fcPlcfbkf = reader.ReadInt32();
            m_lcbPlcfbkf = reader.ReadInt32();
            m_fcPlcfbkl = reader.ReadInt32();
            m_lcbPlcfbkl = reader.ReadInt32();
            m_fcCmds = reader.ReadInt32();
            m_lcbCmds = reader.ReadInt32();
            m_fcPlcmcr = reader.ReadInt32();
            m_lcbPlcmcr = reader.ReadInt32();
            m_fcSttbfmcr = reader.ReadInt32();
            m_lcbSttbfmcr = reader.ReadInt32();
            m_fcPrDrvr = reader.ReadInt32();
            m_lcbPrDrvr = reader.ReadInt32();
            m_fcPrEnvPort = reader.ReadInt32();
            m_lcbPrEnvPort = reader.ReadInt32();
            m_fcPrEnvLand = reader.ReadInt32();
            m_lcbPrEnvLand = reader.ReadInt32();
            m_fcWss = reader.ReadInt32();
            m_lcbWss = reader.ReadInt32();
            m_fcDop = reader.ReadInt32();
            m_lcbDop = reader.ReadInt32();
            m_fcSttbfAssoc = reader.ReadInt32();
            m_lcbSttbfAssoc = reader.ReadInt32();
            m_fcClx = reader.ReadInt32();
            m_lcbClx = reader.ReadInt32();
            m_fcPlcfpgdFtn = reader.ReadInt32();
            m_lcbPlcfpgdFtn = reader.ReadInt32();
            m_fcAutosaveSource = reader.ReadInt32();
            m_lcbAutosaveSource = reader.ReadInt32();
            m_fcGrpXstAtnOwners = reader.ReadInt32();
            m_lcbGrpXstAtnOwners = reader.ReadInt32();
            m_fcSttbfAtnbkmk = reader.ReadInt32();
            m_lcbfcSttbfAtnbkmk = reader.ReadInt32();
            m_fcPlcdoaMom = reader.ReadInt32();
            m_lcbPlcdoaMom = reader.ReadInt32();
            m_fcPlcdoaHdr = reader.ReadInt32();
            m_lcbPlcdoaHdr = reader.ReadInt32();
            m_fcPlcspaMom = reader.ReadInt32();
            m_lcbPlcspaMom = reader.ReadInt32();
            m_fcPlcspaHdr = reader.ReadInt32();
            m_lcbPlcspaHdr = reader.ReadInt32();
            m_fcPlcfAtnbkf = reader.ReadInt32();
            m_lcbPlcfAtnbkf = reader.ReadInt32();
            m_fcPlcfAtnbkl = reader.ReadInt32();
            m_lcbPlcfAtnbkl = reader.ReadInt32();
            m_fcPms = reader.ReadInt32();
            m_lcbPms = reader.ReadInt32();
            m_fcFormFldSttbf = reader.ReadInt32();
            m_lcbFormFldSttbf = reader.ReadInt32();
            m_fcPlcfendRef = reader.ReadInt32();
            m_lcbPlcfendRef = reader.ReadInt32();
            m_fcPlcfendTxt = reader.ReadInt32();
            m_lcbPlcfendTxt = reader.ReadInt32();
            m_fcPlcffldEdn = reader.ReadInt32();
            m_lcbPlcffldEdn = reader.ReadInt32();
            m_fcPlcfpgdEdn = reader.ReadInt32();
            m_lcbPlcfpgdEdn = reader.ReadInt32();
            m_fcDggInfo = reader.ReadInt32();
            m_lcbDggInfo = reader.ReadInt32();
            m_fcSttbfRMark = reader.ReadInt32();
            m_lcbSttbfRMark = reader.ReadInt32();
            m_fcSttbfCaption = reader.ReadInt32();
            m_lcbSttbfCaption = reader.ReadInt32();
            m_fcSttbfAutoCaption = reader.ReadInt32();
            m_lcbSttbfAutoCaption = reader.ReadInt32();
            m_fcPlcfwkb = reader.ReadInt32();
            m_lcbPlcfwkb = reader.ReadInt32();
            m_fcPlcfspl = reader.ReadInt32();
            m_lcbPlcfspl = reader.ReadInt32();
            m_fcPlcftxbxTxt = reader.ReadInt32();
            m_lcbPlcftxbxTxt = reader.ReadInt32();
            m_fcPlcffldTxbx = reader.ReadInt32();
            m_lcbPlcffldTxbx = reader.ReadInt32();
            m_fcPlcfHdrtxbxTxt = reader.ReadInt32();
            m_lcbPlcfHdrtxbxTxt = reader.ReadInt32();
            m_fcPlcffldHdrTxbx = reader.ReadInt32();
            m_lcbPlcffldHdrTxbx = reader.ReadInt32();
            m_fcStwUser = reader.ReadInt32();
            m_lcbStwUser = reader.ReadInt32();
            m_fcSttbttmbd = reader.ReadInt32();
            m_lcbSttbttmbd = reader.ReadInt32();
            m_fcUnused = reader.ReadInt32();
            m_lcbUnused = reader.ReadInt32();

            //Added
            m_fcPgdMother = reader.ReadInt32();
            m_lcbPgdMother = reader.ReadInt32();
            m_fcBkdMother = reader.ReadInt32();
            m_lcbBkdMother = reader.ReadInt32();
            m_fcPgdFtn = reader.ReadInt32();
            m_lcbPgdFtn = reader.ReadInt32();
            m_fcBkdFtn = reader.ReadInt32();
            m_lcbBkdFtn = reader.ReadInt32();
            m_fcPgdEdn = reader.ReadInt32();
            m_lcbPgdEdn = reader.ReadInt32();
            m_fcBkdEdn = reader.ReadInt32();
            m_lcbBkdEdn = reader.ReadInt32();
            m_fcSttbfIntlFld = reader.ReadInt32();
            m_lcbSttbfIntlFld = reader.ReadInt32();
            m_fcRouteSlip = reader.ReadInt32();
            m_lcbRouteSlip = reader.ReadInt32();
            m_fcSttbSavedBy = reader.ReadInt32();
            m_lcbSttbSavedBy = reader.ReadInt32();
            m_fcSttbFnm = reader.ReadInt32();
            m_lcbSttbFnm = reader.ReadInt32();
            //      reader.BaseStream.Position = 0x2e2;

            m_fcPlcfLst = reader.ReadInt32();
            m_lcbPlcfLst = reader.ReadInt32();
            m_fcPlfLfo = reader.ReadInt32();
            m_lcbPlfLfo = reader.ReadInt32();
            m_fcPlcftxbxBkd = reader.ReadInt32();
            m_lcbPlcftxbxBkd = reader.ReadInt32();
            m_fcPlcfHdrtxbxBkd = reader.ReadInt32();
            m_lcbPlcfHdrtxbxBkd = reader.ReadInt32();

            //Added
            m_fcDocUndo = reader.ReadInt32();
            m_lcbDocUndo = reader.ReadInt32();
            m_fcRgbuse = reader.ReadInt32();
            m_lcbRgbuse = reader.ReadInt32();
            m_fcUsp = reader.ReadInt32();
            m_lcbUsp = reader.ReadInt32();
            m_fcUskf = reader.ReadInt32();
            m_lcbUskf = reader.ReadInt32();
            m_fcPlcupcRgbuse = reader.ReadInt32();
            m_lcbPlcupcRgbuse = reader.ReadInt32();
            m_fcPlcupcUsp = reader.ReadInt32();
            m_lcbPlcupcUsp = reader.ReadInt32();
            m_fcSttbGlsyStyle = reader.ReadInt32();
            m_lcbSttbGlsyStyle = reader.ReadInt32();
            m_fcPlgosl = reader.ReadInt32();
            m_lcbPlgosl = reader.ReadInt32();
            //reader.BaseStream.Position = 0x342;

            m_fcPlcocx = reader.ReadInt32();
            m_lcbPlcocx = reader.ReadInt32();
            m_fcPlcfbteLvc = reader.ReadInt32();
            m_lcbPlcfbteLvc = reader.ReadInt32();

            //Added
            m_dwLowDateTime = reader.ReadInt32();
            m_wHighDateTime = reader.ReadInt32();
            m_fcPlcflvc = reader.ReadInt32();
            m_lcbPlcflvc = reader.ReadInt32();
            m_fcPlcasumy = reader.ReadInt32();
            m_lcbPlcasumy = reader.ReadInt32();
            m_fcPlcfgram = reader.ReadInt32();
            m_lcbPlcfgram = reader.ReadInt32();
            //      reader.BaseStream.Position = 0x372;

            m_fcSttbListNames = reader.ReadInt32();
            m_lcbSttbListNames = reader.ReadInt32();
            m_fcSttbfUssr = reader.ReadInt32();
            m_lcbSttbfUssr = reader.ReadInt32();

            // Handled to read csw new and fib new values.
            if (m_cfclcb >= 0x6c
                && reader.BaseStream.Length > (long)(num + (m_cfclcb * 8) + 4))
            {
                reader.BaseStream.Position = (long)(num + (m_cfclcb * 8));
                m_cswNew = reader.ReadUInt16();
                if (m_cswNew >= 2)
                    m_nFibNew = reader.ReadUInt16();
            }
        }

        /// <summary>
        /// Writes the not encrypted fib data.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void WriteNotEncryptedFib(BinaryWriter writer)
        {
            writer.Write(m_wIdent);
            writer.Write(m_fibVersion);
            writer.Write(m_nProduct);
            writer.Write(m_lid);
            writer.Write(m_pnNext);
            ushort tmp = 0;
            if (m_fDot)
            {
                tmp = (ushort)(tmp | 1);
            }
            if (m_fGlsy)
            {
                tmp = (ushort)(tmp | 2);
            }
            if (m_fComplex)
            {
                tmp = (ushort)(tmp | 4);
            }
            if (m_fHasPic)
            {
                tmp = (ushort)(tmp | 8);
            }
            tmp = (ushort)(tmp | Convert.ToUInt16((int)(240 & (m_cQuickSaves << 4))));
            if (m_fEncrypted)
            {
                tmp = (ushort)(tmp | 0x100);
            }
            if (m_fWhichTblStm)
            {
                tmp = (ushort)(tmp | 0x200);
            }
            if (m_fReadOnlyRecom)
            {
                tmp = (ushort)(tmp | 0x600);
            }
            if (m_fExtChar)
            {
                tmp = (ushort)(tmp | 0x1000);
            }
            writer.Write(tmp);

            writer.Write(m_nFibBack);
            if (!m_fEncrypted)
                writer.Write(m_lKey);
            else
                writer.Write(52);
            //writer.Write( m_lKey2 );
            writer.Write(m_envr);

            byte sum = 0;
            if (m_fMac)
            {
                sum = (byte)(sum | 1);
            }
            if (m_fEmptySpecial)
            {
                sum = (byte)(sum | 2);
            }
            if (m_fLoadOverridePage)
            {
                sum = (byte)(sum | 4);
            }
            if (m_fFutureSavedUndo)
            {
                sum = (byte)(sum | 8);
            }
            if (m_fWord97Saved)
            {
                sum = (byte)(sum | 0x10);
            }
            if (m_fSpare0)
            {
                sum = (byte)(sum | 0x20);
            }
            writer.Write(sum);

            writer.Write(m_chs);
            writer.Write(m_chsTables);
            writer.Write(m_fcMin);
            writer.Write(m_fcMac);
            writer.Write(m_csw);
            writer.Write(m_wMagicCreated);
            writer.Write(m_wMagicRevised);
            writer.Write(m_wMagicCreatedPrivate);
            writer.Write(m_wMagicRevisedPrivate);

            writer.Write(m_pnFbpChpFirst_W6);
            writer.Write(m_pnChpFirst_W6);
            writer.Write(m_cpnBteChp_W6);
            writer.Write(m_pnFbpPapFirst_W6);
            writer.Write(m_pnPapFirst_W6);
            writer.Write(m_cpnBtePap_W6);
            writer.Write(m_pnFbpLvcFirst_W6);
            writer.Write(m_pnLvcFirst_W6);
            writer.Write(m_cpnBteLvc_W6);

            writer.Write(m_lidFE);
            writer.Write(m_clw);
        }
        #endregion
    }
}