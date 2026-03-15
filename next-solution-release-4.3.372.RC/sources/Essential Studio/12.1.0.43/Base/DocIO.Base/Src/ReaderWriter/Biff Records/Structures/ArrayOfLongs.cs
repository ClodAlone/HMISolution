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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for ArrayOfShorts.
    /// </summary>
    //[ StructLayout( LayoutKind.Sequential ) ]
    internal class ArrayOfLongs : BaseArrayOfLongs
    {
        #region Class constants
        private const int DEF_OFFSET_CBMAC = 0;
        private const int DEF_OFFSET_LPRODUCTCREATED = 1;
        private const int DEF_OFFSET_LPRODUCTREVISED = 2;
        private const int DEF_OFFSET_CCPTEXT = 3;
        private const int DEF_OFFSET_CCPFTN = 4;
        private const int DEF_OFFSET_CCPHDR = 5;
        private const int DEF_OFFSET_CCPMCR = 6;
        private const int DEF_OFFSET_CCPATN = 7;
        private const int DEF_OFFSET_CCPEDN = 8;
        private const int DEF_OFFSET_CCPTXBX = 9;
        private const int DEF_OFFSET_CCPHDRTXBX = 10;
        private const int DEF_OFFSET_PNFBPCHPFIRST = 11;
        private const int DEF_OFFSET_PNCHPFIRST = 12;
        private const int DEF_OFFSET_CPNBTECHP = 13;
        private const int DEF_OFFSET_PNFBPPAPFIRST = 14;
        private const int DEF_OFFSET_PNPAPFIRST = 15;
        private const int DEF_OFFSET_CPNBTEPAP = 16;
        private const int DEF_OFFSET_PNFBPLVCFIRST = 17;
        private const int DEF_OFFSET_PNLVCFIRST = 18;
        private const int DEF_OFFSET_CPNBTELVC = 19;
        private const int DEF_OFFSET_FCISLANDFIRST = 20;
        private const int DEF_OFFSET_FCISLANDLIM = 21;
        #endregion

        #region Class Properties
        /// <summary>
        /// file offset of last byte written to file + 1.
        /// </summary>
        //[ FieldOffset( 64 ) ]
        internal int cbMac
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CBMAC];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CBMAC] = value;
            }
        }
        /// <summary>
        /// contains the build date of the creator. 10695 indicates the creator program was compiled on Jan 6, 1995
        /// </summary>
        //[ FieldOffset( 68 ) ]
        internal int lProductCreated
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_LPRODUCTCREATED];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_LPRODUCTCREATED] = value;
            }
        }
        /// <summary>
        /// contains the build date of the File's last modifier
        /// </summary>
        //[ FieldOffset( 72 ) ]
        internal int lProductRevised
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_LPRODUCTREVISED];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_LPRODUCTREVISED] = value;
            }
        }
        /// <summary>
        /// length of main document text stream
        /// </summary>
        //[ FieldOffset( 76 ) ]
        internal int ccpText
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CCPTEXT];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CCPTEXT] = value;
            }
        }
        /// <summary>
        /// length of footnote subdocument text stream
        /// </summary>
        //[ FieldOffset( 80 ) ]
        internal int ccpFtn
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CCPFTN];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CCPFTN] = value;
            }
        }
        /// <summary>
        /// length of header subdocument text stream
        /// </summary>
        //[ FieldOffset( 84 ) ]
        internal int ccpHdr
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CCPHDR];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CCPHDR] = value;
            }
        }
        /// <summary>
        /// length of macro subdocument text stream, which should now always be 0.
        /// </summary>
        //[ FieldOffset( 88 ) ]
        internal int ccpMcr
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CCPMCR];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CCPMCR] = value;
            }
        }
        /// <summary>
        /// length of annotation subdocument text stream
        /// </summary>
        //[ FieldOffset( 92 ) ]
        internal int ccpAtn
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CCPATN];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CCPATN] = value;
            }
        }
        /// <summary>
        /// length of endnote subdocument text stream
        /// </summary>
        //[ FieldOffset( 96 ) ]
        internal int ccpEdn
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CCPEDN];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CCPEDN] = value;
            }
        }
        /// <summary>
        /// length of textbox subdocument text stream
        /// </summary>
        //[ FieldOffset( 100 ) ]
        internal int ccpTxbx
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CCPTXBX];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CCPTXBX] = value;
            }
        }
        /// <summary>
        /// length of header textbox subdocument text stream.
        /// </summary>
        //[ FieldOffset( 104 ) ]
        internal int ccpHdrTxbx
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CCPHDRTXBX];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CCPHDRTXBX] = value;
            }
        }
        /// <summary>
        /// when there was insufficient memory for Word to expand the plcfbte at save time, the plcfbte is written to the file in a linked list of 512-byte pieces starting with this pn
        /// </summary>
        //[ FieldOffset( 108 ) ]
        internal int pnFbpChpFirst
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_PNFBPCHPFIRST];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_PNFBPCHPFIRST] = value;
            }
        }
        /// <summary>
        /// the page number of the lowest numbered page in the document that records CHPX FKP information
        /// </summary>
        //[ FieldOffset( 112 ) ]
        internal int pnChpFirst
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_PNCHPFIRST];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_PNCHPFIRST] = value;
            }
        }
        /// <summary>
        /// count of CHPX FKPs recorded in file. In non-complex files if the number of entries in the plcfbteChpx is less than this, the plcfbteChpx is incomplete.
        /// </summary>
        //[ FieldOffset( 116 ) ]
        internal int cpnBteChp
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CPNBTECHP];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CPNBTECHP] = value;
            }
        }
        /// <summary>
        /// when there was insufficient memory for Word to expand the plcfbte at save time, the plcfbte is written to the file in a linked list of 512-byte pieces starting with this pn
        /// </summary>
        //[ FieldOffset( 120 ) ]
        internal int pnFbpPapFirst
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_PNFBPPAPFIRST];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_PNFBPPAPFIRST] = value;
            }
        }
        /// <summary>
        /// the page number of the lowest numbered page in the document that records PAPX FKP information
        /// </summary>
        //[ FieldOffset( 124 ) ]
        internal int pnPapFirst
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_PNPAPFIRST];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_PNPAPFIRST] = value;
            }
        }
        /// <summary>
        /// count of PAPX FKPs recorded in file. In non-complex files if the number of entries in the plcfbtePapx is less than this, the plcfbtePapx is incomplete.
        /// </summary>
        //[ FieldOffset( 128 ) ]
        internal int cpnBtePap
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CPNBTEPAP];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CPNBTEPAP] = value;
            }
        }
        /// <summary>
        /// when there was insufficient memory for Word to expand the plcfbte at save time, the plcfbte is written to the file in a linked list of 512-byte pieces starting with this pn
        /// </summary>
        //[ FieldOffset( 132 ) ]
        internal int pnFbpLvcFirst
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_PNFBPLVCFIRST];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_PNFBPLVCFIRST] = value;
            }
        }
        /// <summary>
        /// the page number of the lowest numbered page in the document that records LVC FKP information
        /// </summary>
        //[ FieldOffset( 136 ) ]
        internal int pnLvcFirst
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_PNLVCFIRST];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_PNLVCFIRST] = value;
            }
        }
        /// <summary>
        /// count of LVC FKPs recorded in file. In non-complex files if the number of entries in the plcfbtePapx is less than this, the plcfbtePapx is incomplete.
        /// </summary>
        //[ FieldOffset( 140 ) ]
        internal int cpnBteLvc
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_CPNBTELVC];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_CPNBTELVC] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        //[ FieldOffset( 144 ) ]
        internal int fcIslandFirst
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_FCISLANDFIRST];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_FCISLANDFIRST] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        //[ FieldOffset( 148 ) ]
        internal int fcIslandLim
        {
            get
            {
                return m_arrLongs[DEF_OFFSET_FCISLANDLIM];
            }
            set
            {
                m_arrLongs[DEF_OFFSET_FCISLANDLIM] = value;
            }
        }
        /// <summary>
        /// Length of the array.
        /// </summary>
        internal int Length
        {
            get
            {
                return m_arrLongs.Length;
            }
            set
            {
                Resize(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int BytesCount
        {
            get
            {
                return Length * Constants.BytesInInt;
            }
        }

        #endregion
    }
}
