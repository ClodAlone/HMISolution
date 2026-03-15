#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.quantization.dequantizer;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
using Syncfusion.Pdf.JPEG2000.entropy.decoder;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.roi;
using Syncfusion.Pdf.JPEG2000.io;
using HashTable = System.Collections.Generic.Dictionary<object, object>;
using ArrayList = System.Collections.Generic.List<object>;
namespace Syncfusion.Pdf.JPEG2000.codestream.reader
{
    internal class HeaderDecoder
    {
        virtual public int MaxCompImgHeight
        {
            get
            {
                return hi.sizValue.MaxCompHeight;
            }
        }
        virtual public int MaxCompImgWidth
        {
            get
            {
                return hi.sizValue.MaxCompWidth;
            }
        }
        virtual public int ImgWidth
        {
            get
            {
                return hi.sizValue.xsiz - hi.sizValue.x0siz;
            }
        }
        virtual public int ImgHeight
        {
            get
            {
                return hi.sizValue.ysiz - hi.sizValue.y0siz;
            }
        }
        virtual public int ImgULX
        {
            get
            {
                return hi.sizValue.x0siz;
            }
        }
        virtual public int ImgULY
        {
            get
            {
                return hi.sizValue.y0siz;
            }
        }
        virtual public int NomTileWidth
        {
            get
            {
                return hi.sizValue.xtsiz;
            }
        }
        virtual public int NomTileHeight
        {
            get
            {
                return hi.sizValue.ytsiz;
            }
        }
        virtual public int NumComps
        {
            get
            {
                return nComp;
            }
        }
        virtual public int CbULX
        {
            get
            {
                return cb0x;
            }
        }
        virtual public int CbULY
        {
            get
            {
                return cb0y;
            }
        }
        virtual public DecodeHelper DecoderHelper
        {
            get
            {
                return decSpec;
            }
        }
        public static System.String[][] ParameterInfo
        {
            get
            {
                return pinfo;
            }
        }
        virtual public int NumTiles
        {
            get
            {
                return nTiles;
            }
        }
        virtual public int TileOfTileParts
        {
            set
            {
                if (nPPMMarkSeg != 0)
                {
                    tileOfTileParts.Add((System.Int32)value);
                }
            }
        }
        virtual public int NumFoundMarkSeg
        {
            get
            {
                return nfMarkSeg;
            }
        }
        public const char OPT_PREFIX = 'H';
        private static readonly System.String[][] pinfo = null;
        private HeaderInformation hi;
        private System.String hdStr = "";
        private int nTiles;
        public int[] nTileParts;
        private int nfMarkSeg = 0;
        private int nCOCMarkSeg = 0;
        private int nQCCMarkSeg = 0;
        private int nCOMMarkSeg = 0;
        private int nRGNMarkSeg = 0;
        private int nPPMMarkSeg = 0;
        private int[][] nPPTMarkSeg = null;
        private const int SIZ_FOUND = 1;
        private const int COD_FOUND = 1 << 1;
        private const int COC_FOUND = 1 << 2;
        private const int QCD_FOUND = 1 << 3;
        private const int TLM_FOUND = 1 << 4;
        private const int PLM_FOUND = 1 << 5;
        private const int SOT_FOUND = 1 << 6;
        private const int PLT_FOUND = 1 << 7;
        private const int QCC_FOUND = 1 << 8;
        private const int RGN_FOUND = 1 << 9;
        private const int POC_FOUND = 1 << 10;
        private const int COM_FOUND = 1 << 11;
        public const int SOD_FOUND = 1 << 13;
        public const int PPM_FOUND = 1 << 14;
        public const int PPT_FOUND = 1 << 15;
        public const int CRG_FOUND = 1 << 16;
        private HashTable ht = null;
        private int nComp;
        private int cb0x = -1;
        private int cb0y = -1;
        private DecodeHelper decSpec;
        internal bool precinctPartitionIsUsed;
        public int mainHeadOff;
        private ArrayList tileOfTileParts;
        private byte[][] pPMMarkerData;
        private byte[][][][] tilePartPkdPktHeaders;
        private System.IO.MemoryStream[] pkdPktHeaders;
        public JPXImageCoordinates getTilingOrigin(JPXImageCoordinates co)
        {
            if (co != null)
            {
                co.x = hi.sizValue.xt0siz;
                co.y = hi.sizValue.yt0siz;
                return co;
            }
            else
            {
                return new JPXImageCoordinates(hi.sizValue.xt0siz, hi.sizValue.yt0siz);
            }
        }
        public bool isOriginalSigned(int c)
        {
            return hi.sizValue.isOrigSigned(c);
        }
        public int GetActualBitDepth(int c)
        {
            return hi.sizValue.getOrigBitDepth(c);
        }
        public int getCompSubsX(int c)
        {
            return hi.sizValue.xrsiz[c];
        }
        public int getCompSubsY(int c)
        {
            return hi.sizValue.yrsiz[c];
        }
        public Dequantizer createDequantizer(CBlkQuantDataSrcDec src, int[] rb, DecodeHelper decSpec2)
        {
            return new StdDequantizer(src, rb, decSpec2);
        }
        public int getPPX(int t, int c, int rl)
        {
            return decSpec.pss.getPPX(t, c, rl);
        }
        public int getPPY(int t, int c, int rl)
        {
            return decSpec.pss.getPPY(t, c, rl);
        }
        public bool precinctPartitionUsed()
        {
            return precinctPartitionIsUsed;
        }
        private SynWTFilter readFilter(System.IO.BinaryReader ehs, int[] filtIdx)
        {
            int kid; 
            kid = filtIdx[0] = ehs.ReadByte();
            if (kid >= (1 << 7))
            {
                throw new System.ArgumentException("Custom filters are used");
            }            
            switch (kid)
            {
                case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W9X7:
                    return new SynWTFilterFloatLift9x7();
                case Syncfusion.Pdf.JPEG2000.wavelet.FilterTypes_Fields.W5X3:
                    return new SynWTFilterIntLift5x3();
                default:
                    throw new System.Exception();
            }
        }
        public virtual void checkMarkerLength(System.IO.BinaryReader ehs, System.String str)
        {
            long available;
            available = ehs.BaseStream.Length - ehs.BaseStream.Position;            
        }
        private void readSIZ(System.IO.BinaryReader ehs)
        {
            HeaderInformation.SIZ ms = hi.NewSIZ;
            hi.sizValue = ms;
            ms.lsiz = ehs.ReadUInt16();
            ms.rsiz = ehs.ReadUInt16();
            if (ms.rsiz > 2)
            {
                //throw new System.ApplicationException("Codestream capabiities not JPEG 2000 - Part I" + " compliant");
            }
            ms.xsiz = ehs.ReadInt32();
            ms.ysiz = ehs.ReadInt32();
            if (ms.xsiz <= 0 || ms.ysiz <= 0)
            {
                throw new System.IO.IOException("JJ2000 does not support images whose " + "width and/or height not in the " + "range: 1 -- (2^31)-1");
            }
            ms.x0siz = ehs.ReadInt32();
            ms.y0siz = ehs.ReadInt32();
            if (ms.x0siz < 0 || ms.y0siz < 0)
            {
                throw new System.IO.IOException("JJ2000 does not support images offset " + "not in the range: 0 -- (2^31)-1");
            }
            ms.xtsiz = ehs.ReadInt32();
            ms.ytsiz = ehs.ReadInt32();
            if (ms.xtsiz <= 0 || ms.ytsiz <= 0)
            {
                throw new System.IO.IOException("JJ2000 does not support tiles whose " + "width and/or height are not in  " + "the range: 1 -- (2^31)-1");
            }
            ms.xt0siz = ehs.ReadInt32();
            ms.yt0siz = ehs.ReadInt32();
            if (ms.xt0siz < 0 || ms.yt0siz < 0)
            {
                throw new System.IO.IOException("JJ2000 does not support tiles whose " + "offset is not in  " + "the range: 0 -- (2^31)-1");
            }
            nComp = ms.csiz = ehs.ReadUInt16();
            if (nComp < 1 || nComp > 16384)
            {
                throw new System.ArgumentException("Number of component out of " + "range 1--16384: " + nComp);
            }
            ms.ssiz = new int[nComp];
            ms.xrsiz = new int[nComp];
            ms.yrsiz = new int[nComp];
            for (int i = 0; i < nComp; i++)
            {
                ms.ssiz[i] = ehs.ReadByte();
                ms.xrsiz[i] = ehs.ReadByte();
                ms.yrsiz[i] = ehs.ReadByte();
            }
            checkMarkerLength(ehs, "SIZ marker");
            nTiles = ms.NumTiles;
            decSpec = new DecodeHelper(nTiles, nComp);
        }
        private void readCRG(System.IO.BinaryReader ehs)
        {
            HeaderInformation.CRG ms = hi.NewCRG;
            hi.crgValue = ms;
            ms.lcrg = ehs.ReadUInt16();
            ms.xcrg = new int[nComp];
            ms.ycrg = new int[nComp];
            for (int c = 0; c < nComp; c++)
            {
                ms.xcrg[c] = ehs.ReadUInt16();
                ms.ycrg[c] = ehs.ReadUInt16();
            }
            checkMarkerLength(ehs, "CRG marker");
        }
        private void readCOM(System.IO.BinaryReader ehs, bool mainh, int tileIdx, int comIdx)
        {
            HeaderInformation.COM ms = hi.NewCOM;
            ms.lcom = ehs.ReadUInt16();
            ms.rcom = ehs.ReadUInt16();
            switch (ms.rcom)
            {
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.RCOM_GEN_USE:
                    ms.ccom = new byte[ms.lcom - 4];
                    for (int i = 0; i < ms.lcom - 4; i++)
                    {
                        ms.ccom[i] = ehs.ReadByte();
                    }
                    break;
                default:                    
                    System.IO.BinaryReader temp_BinaryReader;
                    System.Int64 temp_Int64;
                    temp_BinaryReader = ehs;
                    temp_Int64 = temp_BinaryReader.BaseStream.Position;
                    temp_Int64 = temp_BinaryReader.BaseStream.Seek(ms.lcom - 4, System.IO.SeekOrigin.Current) - temp_Int64;
                    int generatedAux2 = (int)temp_Int64;
                    break;
            }
            if (mainh)
            {
                hi.comValue["main_" + comIdx] = ms;
            }
            else
            {
                hi.comValue["t" + tileIdx + "_" + comIdx] = ms;
            }
            checkMarkerLength(ehs, "COM marker");
        }
        private void readQCD(System.IO.BinaryReader ehs, bool mainh, int tileIdx, int tpIdx)
        {
            StdDequantizerParams qParms;
            int guardBits;
            int[][] exp;
            float[][] nStep = null;
            HeaderInformation.QCD ms = hi.NewQCD;
            ms.lqcd = ehs.ReadUInt16();
            ms.sqcd = ehs.ReadByte();
            guardBits = ms.NumGuardBits;
            int qType = ms.QuantType;
            if (mainh)
            {
                hi.qcdValue["main"] = ms;
                switch (qType)
                {
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION:
                        decSpec.qts.setDefault("reversible");
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_DERIVED:
                        decSpec.qts.setDefault("derived");
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_EXPOUNDED:
                        decSpec.qts.setDefault("expounded");
                        break;
                }
            }
            else
            {
                hi.qcdValue["t" + tileIdx] = ms;
                switch (qType)
                {
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION:
                        decSpec.qts.setTileDef(tileIdx, "reversible");
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_DERIVED:
                        decSpec.qts.setTileDef(tileIdx, "derived");
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_EXPOUNDED:
                        decSpec.qts.setTileDef(tileIdx, "expounded");
                        break;
                }
            }
            qParms = new StdDequantizerParams();
            if (qType == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION)
            {
                int maxrl = (mainh ? ((System.Int32)decSpec.dls.getDefault()) : ((System.Int32)decSpec.dls.getTileDef(tileIdx)));
                int j, rl; // i removed
                int minb, maxb, hpd;
                int tmp;
                exp = qParms.exp = new int[maxrl + 1][];
                int[][] tmpArray = new int[maxrl + 1][];
                for (int i2 = 0; i2 < maxrl + 1; i2++)
                {
                    tmpArray[i2] = new int[4];
                }
                ms.spqcd = tmpArray;
                for (rl = 0; rl <= maxrl; rl++)
                {
                    if (rl == 0)
                    {
                        minb = 0;
                        maxb = 1;
                    }
                    else
                    {
                        hpd = 1;
                        if (hpd > maxrl - rl)
                        {
                            hpd -= (maxrl - rl);
                        }
                        else
                        {
                            hpd = 1;
                        }
                        minb = 1 << ((hpd - 1) << 1);
                        maxb = 1 << (hpd << 1);
                    }
                    exp[rl] = new int[maxb];
                    for (j = minb; j < maxb; j++)
                    {
                        tmp = ms.spqcd[rl][j] = ehs.ReadByte();
                        exp[rl][j] = (tmp >> Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_SHIFT) & Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_MASK;
                    }
                }
            }
            else
            {
                int maxrl = (qType == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_DERIVED) ? 0 : (mainh ? ((System.Int32)decSpec.dls.getDefault()) : ((System.Int32)decSpec.dls.getTileDef(tileIdx)));
                int j, rl;
                int minb, maxb, hpd;
                int tmp;
                exp = qParms.exp = new int[maxrl + 1][];
                nStep = qParms.nStep = new float[maxrl + 1][];
                int[][] tmpArray2 = new int[maxrl + 1][];
                for (int i3 = 0; i3 < maxrl + 1; i3++)
                {
                    tmpArray2[i3] = new int[4];
                }
                ms.spqcd = tmpArray2;
                for (rl = 0; rl <= maxrl; rl++)
                {
                    if (rl == 0)
                    {
                        minb = 0;
                        maxb = 1;
                    }
                    else
                    {
                        hpd = 1;
                        if (hpd > maxrl - rl)
                        {
                            hpd -= (maxrl - rl);
                        }
                        else
                        {
                            hpd = 1;
                        }
                        minb = 1 << ((hpd - 1) << 1);
                        maxb = 1 << (hpd << 1);
                    }
                    exp[rl] = new int[maxb];
                    nStep[rl] = new float[maxb];
                    for (j = minb; j < maxb; j++)
                    {
                        tmp = ms.spqcd[rl][j] = ehs.ReadUInt16();
                        exp[rl][j] = (tmp >> 11) & 0x1f;
                        nStep[rl][j] = (-1f - ((float)(tmp & 0x07ff)) / (1 << 11)) / (-1 << exp[rl][j]);
                    }
                }
            }
            if (mainh)
            {
                decSpec.qsss.setDefault(qParms);
                decSpec.gbs.setDefault((System.Object)guardBits);
            }
            else
            {
                decSpec.qsss.setTileDef(tileIdx, qParms);
                decSpec.gbs.setTileDef(tileIdx, (System.Object)guardBits);
            }
            checkMarkerLength(ehs, "QCD marker");
        }
        private void readQCC(System.IO.BinaryReader ehs, bool mainh, int tileIdx, int tpIdx)
        {
            int cComp;
            int tmp;
            StdDequantizerParams qParms;
            int[][] expC;
            float[][] nStepC = null;
            HeaderInformation.QCC ms = hi.NewQCC;
            ms.lqcc = ehs.ReadUInt16();
            if (nComp < 257)
            {
                cComp = ms.cqcc = ehs.ReadByte();
            }
            else
            {
                cComp = ms.cqcc = ehs.ReadUInt16();
            }
            if (cComp >= nComp)
            {
                throw new System.Exception();
            }
            ms.sqcc = ehs.ReadByte();
            int guardBits = ms.NumGuardBits;
            int qType = ms.QuantType;
            if (mainh)
            {
                hi.qccValue["main_c" + cComp] = ms;
                switch (qType)
                {
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION:
                        decSpec.qts.setCompDef(cComp, "reversible");
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_DERIVED:
                        decSpec.qts.setCompDef(cComp, "derived");
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_EXPOUNDED:
                        decSpec.qts.setCompDef(cComp, "expounded");
                        break;
                    default:
                        throw new System.Exception();
                }
            }
            else
            {
                hi.qccValue["t" + tileIdx + "_c" + cComp] = ms;
                switch (qType)
                {
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION:
                        decSpec.qts.setTileCompVal(tileIdx, cComp, "reversible");
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_DERIVED:
                        decSpec.qts.setTileCompVal(tileIdx, cComp, "derived");
                        break;
                    case Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_EXPOUNDED:
                        decSpec.qts.setTileCompVal(tileIdx, cComp, "expounded");
                        break;
                    default:
                        throw new System.Exception();
                }
            }
            qParms = new StdDequantizerParams();
            if (qType == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_NO_QUANTIZATION)
            {
                int maxrl = (mainh ? ((System.Int32)decSpec.dls.getCompDef(cComp)) : ((System.Int32)decSpec.dls.getTileCompVal(tileIdx, cComp)));
                int j, rl;
                int minb, maxb, hpd;
                expC = qParms.exp = new int[maxrl + 1][];
                int[][] tmpArray = new int[maxrl + 1][];
                for (int i2 = 0; i2 < maxrl + 1; i2++)
                {
                    tmpArray[i2] = new int[4];
                }
                ms.spqcc = tmpArray;
                for (rl = 0; rl <= maxrl; rl++)
                {
                    if (rl == 0)
                    {
                        minb = 0;
                        maxb = 1;
                    }
                    else
                    {
                        hpd = 1;
                        if (hpd > maxrl - rl)
                        {
                            hpd -= (maxrl - rl);
                        }
                        else
                        {
                            hpd = 1;
                        }
                        minb = 1 << ((hpd - 1) << 1);
                        maxb = 1 << (hpd << 1);
                    }
                    expC[rl] = new int[maxb];
                    for (j = minb; j < maxb; j++)
                    {
                        tmp = ms.spqcc[rl][j] = ehs.ReadByte();
                        expC[rl][j] = (tmp >> Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_SHIFT) & Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_EXP_MASK;
                    }
                }
            }
            else
            {
                int maxrl = (qType == Syncfusion.Pdf.JPEG2000.codestream.Markers.SQCX_SCALAR_DERIVED) ? 0 : (mainh ? ((System.Int32)decSpec.dls.getCompDef(cComp)) : ((System.Int32)decSpec.dls.getTileCompVal(tileIdx, cComp)));
                int j, rl;
                int minb, maxb, hpd;
                nStepC = qParms.nStep = new float[maxrl + 1][];
                expC = qParms.exp = new int[maxrl + 1][];
                int[][] tmpArray2 = new int[maxrl + 1][];
                for (int i3 = 0; i3 < maxrl + 1; i3++)
                {
                    tmpArray2[i3] = new int[4];
                }
                ms.spqcc = tmpArray2;
                for (rl = 0; rl <= maxrl; rl++)
                {
                    if (rl == 0)
                    {
                        minb = 0;
                        maxb = 1;
                    }
                    else
                    {
                        hpd = 1;
                        if (hpd > maxrl - rl)
                        {
                            hpd -= (maxrl - rl);
                        }
                        else
                        {
                            hpd = 1;
                        }
                        minb = 1 << ((hpd - 1) << 1);
                        maxb = 1 << (hpd << 1);
                    }
                    expC[rl] = new int[maxb];
                    nStepC[rl] = new float[maxb];
                    for (j = minb; j < maxb; j++)
                    {
                        tmp = ms.spqcc[rl][j] = ehs.ReadUInt16();
                        expC[rl][j] = (tmp >> 11) & 0x1f;
                        nStepC[rl][j] = (-1f - ((float)(tmp & 0x07ff)) / (1 << 11)) / (-1 << expC[rl][j]);
                    }
                }
            }
            if (mainh)
            {
                decSpec.qsss.setCompDef(cComp, qParms);
                decSpec.gbs.setCompDef(cComp, (System.Object)guardBits);
            }
            else
            {
                decSpec.qsss.setTileCompVal(tileIdx, cComp, qParms);
                decSpec.gbs.setTileCompVal(tileIdx, cComp, (System.Object)guardBits);
            }
            checkMarkerLength(ehs, "QCC marker");
        }
        private void readCOD(System.IO.BinaryReader ehs, bool mainh, int tileIdx, int tpIdx)
        {
            int cstyle;
            SynWTFilter[] hfilters, vfilters;
            System.Int32[] cblk;
            System.String errMsg;
            HeaderInformation.COD ms = hi.NewCOD;
            ms.lcod = ehs.ReadUInt16();
            cstyle = ms.scod = ehs.ReadByte();
            if ((cstyle & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_PRECINCT_PARTITION) != 0)
            {
                precinctPartitionIsUsed = true;
                cstyle &= ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_PRECINCT_PARTITION);
            }
            else
            {
                precinctPartitionIsUsed = false;
            }
            if (mainh)
            {
                hi.codValue["main"] = ms;
                if ((cstyle & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_SOP) != 0)
                {
                    decSpec.sops.setDefault((System.Object)"true".ToUpper().Equals("TRUE"));
                    cstyle &= ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_SOP);
                }
                else
                {
                    decSpec.sops.setDefault((System.Object)"false".ToUpper().Equals("TRUE"));
                }
            }
            else
            {
                hi.codValue["t" + tileIdx] = ms;
                if ((cstyle & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_SOP) != 0)
                {
                    decSpec.sops.setTileDef(tileIdx, (System.Object)"true".ToUpper().Equals("TRUE"));
                    cstyle &= ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_SOP);
                }
                else
                {
                    decSpec.sops.setTileDef(tileIdx, (System.Object)"false".ToUpper().Equals("TRUE"));
                }
            }
            if (mainh)
            {
                if ((cstyle & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_EPH) != 0)
                {
                    decSpec.ephs.setDefault((System.Object)"true".ToUpper().Equals("TRUE"));
                    cstyle &= ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_EPH);
                }
                else
                {
                    decSpec.ephs.setDefault((System.Object)"false".ToUpper().Equals("TRUE"));
                }
            }
            else
            {
                if ((cstyle & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_EPH) != 0)
                {
                    decSpec.ephs.setTileDef(tileIdx, (System.Object)"true".ToUpper().Equals("TRUE"));
                    cstyle &= ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_USE_EPH);
                }
                else
                {
                    decSpec.ephs.setTileDef(tileIdx, (System.Object)"false".ToUpper().Equals("TRUE"));
                }
            }
            if ((cstyle & (Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_HOR_CB_PART | Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_VER_CB_PART)) != 0)
            {
            }
            if ((cstyle & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_HOR_CB_PART) != 0)
            {
                if (cb0x != -1 && cb0x == 0)
                {
                    throw new System.ArgumentException("Code-block partition " + "origin redefined in new" + " COD marker segment. Not" + " supported by JJ2000");
                }
                cb0x = 1;
                cstyle &= ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_HOR_CB_PART);
            }
            else
            {
                if (cb0x != -1 && cb0x == 1)
                {
                    throw new System.ArgumentException("Code-block partition " + "origin redefined in new" + " COD marker segment. Not" + " supported by JJ2000");
                }
                cb0x = 0;
            }
            if ((cstyle & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_VER_CB_PART) != 0)
            {
                if (cb0y != -1 && cb0y == 0)
                {
                    throw new System.ArgumentException("Code-block partition " + "origin redefined in new" + " COD marker segment. Not" + " supported by JJ2000");
                }
                cb0y = 1;
                cstyle &= ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_VER_CB_PART);
            }
            else
            {
                if (cb0y != -1 && cb0y == 1)
                {
                    throw new System.ArgumentException("Code-block partition " + "origin redefined in new" + " COD marker segment. Not" + " supported by JJ2000");
                }
                cb0y = 0;
            }
            ms.sgcod_po = ehs.ReadByte();
            ms.sgcod_nl = ehs.ReadUInt16();
            if (ms.sgcod_nl <= 0 || ms.sgcod_nl > 65535)
            {
                throw new System.ArgumentException("Number of layers out of " + "range: 1--65535");
            }
            ms.sgcod_mct = ehs.ReadByte();
            int mrl = ms.spcod_ndl = ehs.ReadByte();
            if (mrl > 32)
            {
                throw new System.ArgumentException("Number of decomposition " + "levels out of range: " + "0--32");
            }
            cblk = new System.Int32[2];
            ms.spcod_cw = ehs.ReadByte();
            cblk[0] = (System.Int32)(1 << (ms.spcod_cw + 2));
            if (cblk[0] < Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MIN_CB_DIM || cblk[0] > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_DIM)
            {
                errMsg = "Non-valid code-block width in SPcod field, " + "COD marker";
                throw new System.ArgumentException(errMsg);
            }
            ms.spcod_ch = ehs.ReadByte();
            cblk[1] = (System.Int32)(1 << (ms.spcod_ch + 2));
            if (cblk[1] < Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MIN_CB_DIM || cblk[1] > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_DIM)
            {
                errMsg = "Non-valid code-block height in SPcod field, " + "COD marker";
                throw new System.ArgumentException(errMsg);
            }
            if ((cblk[0] * cblk[1]) > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_AREA)
            {
                errMsg = "Non-valid code-block area in SPcod field, " + "COD marker";
                throw new System.ArgumentException(errMsg);
            }
            if (mainh)
            {
                decSpec.cblks.setDefault((System.Object)(cblk));
            }
            else
            {
                decSpec.cblks.setTileDef(tileIdx, (System.Object)(cblk));
            }
            int ecOptions = ms.spcod_cs = ehs.ReadByte();
            if ((ecOptions & ~(Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_RESET_MQ | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_VERT_STR_CAUSAL | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_PRED_TERM | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_SEG_SYMBOLS)) != 0)
            {
                throw new System.ArgumentException("Unknown \"code-block " + "style\" in SPcod field, " + "COD marker: 0x" + System.Convert.ToString(ecOptions, 16));
            }
            hfilters = new SynWTFilter[1];
            vfilters = new SynWTFilter[1];
            hfilters[0] = readFilter(ehs, ms.spcod_t);
            vfilters[0] = hfilters[0];
            SynWTFilter[][] hvfilters = new SynWTFilter[2][];
            hvfilters[0] = hfilters;
            hvfilters[1] = vfilters;
            ArrayList[] v = new ArrayList[2];
            v[0] = new ArrayList(10);
            v[1] = new ArrayList(10);
            int val = Syncfusion.Pdf.JPEG2000.codestream.Markers.PRECINCT_PARTITION_DEF_SIZE;
            if (!precinctPartitionIsUsed)
            {
                System.Int32 w, h;
                w = (System.Int32)(1 << (val & 0x000F));
                v[0].Add(w);
                h = (System.Int32)(1 << (((val & 0x00F0) >> 4)));
                v[1].Add(h);
            }
            else
            {
                ms.spcod_ps = new int[mrl + 1];
                for (int rl = mrl; rl >= 0; rl--)
                {
                    System.Int32 w, h;
                    val = ms.spcod_ps[mrl - rl] = ehs.ReadByte();
                    w = (System.Int32)(1 << (val & 0x000F));
                    v[0].Insert(0, w);
                    h = (System.Int32)(1 << (((val & 0x00F0) >> 4)));
                    v[1].Insert(0, h);
                }
            }
            if (mainh)
            {
                decSpec.pss.setDefault(v);
            }
            else
            {
                decSpec.pss.setTileDef(tileIdx, v);
            }
            precinctPartitionIsUsed = true;
            checkMarkerLength(ehs, "COD marker");
            if (mainh)
            {
                decSpec.wfs.setDefault(hvfilters);
                decSpec.dls.setDefault((System.Object)mrl);
                decSpec.ecopts.setDefault((System.Object)ecOptions);
                decSpec.cts.setDefault((System.Object)ms.sgcod_mct);
                decSpec.nls.setDefault((System.Object)ms.sgcod_nl);
                decSpec.pos.setDefault((System.Object)ms.sgcod_po);
            }
            else
            {
                decSpec.wfs.setTileDef(tileIdx, hvfilters);
                decSpec.dls.setTileDef(tileIdx, (System.Object)mrl);
                decSpec.ecopts.setTileDef(tileIdx, (System.Object)ecOptions);
                decSpec.cts.setTileDef(tileIdx, (System.Object)ms.sgcod_mct);
                decSpec.nls.setTileDef(tileIdx, (System.Object)ms.sgcod_nl);
                decSpec.pos.setTileDef(tileIdx, (System.Object)ms.sgcod_po);
            }
        }
        private void readCOC(System.IO.BinaryReader ehs, bool mainh, int tileIdx, int tpIdx)
        {
            int cComp;
            SynWTFilter[] hfilters, vfilters;
            int ecOptions;
            System.Int32[] cblk;
            System.String errMsg;
            HeaderInformation.COC ms = hi.NewCOC;
            ms.lcoc = ehs.ReadUInt16();
            if (nComp < 257)
            {
                cComp = ms.ccoc = ehs.ReadByte();
            }
            else
            {
                cComp = ms.ccoc = ehs.ReadUInt16();
            }
            if (cComp >= nComp)
            {
                throw new System.ArgumentException("Invalid component index " + "in QCC marker");
            }
            int cstyle = ms.scoc = ehs.ReadByte();
            if ((cstyle & Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_PRECINCT_PARTITION) != 0)
            {
                precinctPartitionIsUsed = true;
                cstyle &= ~(Syncfusion.Pdf.JPEG2000.codestream.Markers.SCOX_PRECINCT_PARTITION);
            }
            else
            {
                precinctPartitionIsUsed = false;
            }
            int mrl = ms.spcoc_ndl = ehs.ReadByte();
            cblk = new System.Int32[2];
            ms.spcoc_cw = ehs.ReadByte();
            cblk[0] = (System.Int32)(1 << (ms.spcoc_cw + 2));
            if (cblk[0] < Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MIN_CB_DIM || cblk[0] > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_DIM)
            {
                errMsg = "Non-valid code-block width in SPcod field, " + "COC marker";
                throw new System.ArgumentException(errMsg);
            }
            ms.spcoc_ch = ehs.ReadByte();
            cblk[1] = (System.Int32)(1 << (ms.spcoc_ch + 2));
            if (cblk[1] < Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MIN_CB_DIM || cblk[1] > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_DIM)
            {
                errMsg = "Non-valid code-block height in SPcod field, " + "COC marker";
                throw new System.ArgumentException(errMsg);
            }
            if ((cblk[0] * cblk[1]) > Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.MAX_CB_AREA)
            {
                errMsg = "Non-valid code-block area in SPcod field, " + "COC marker";
                throw new System.ArgumentException(errMsg);
            }
            if (mainh)
            {
                decSpec.cblks.setCompDef(cComp, (System.Object)(cblk));
            }
            else
            {
                decSpec.cblks.setTileCompVal(tileIdx, cComp, (System.Object)(cblk));
            }
            ecOptions = ms.spcoc_cs = ehs.ReadByte();
            if ((ecOptions & ~(Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_RESET_MQ | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_VERT_STR_CAUSAL | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_PRED_TERM | Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_SEG_SYMBOLS)) != 0)
            {
                throw new System.ArgumentException("Unknown \"code-block " + "context\" in SPcoc field, " + "COC marker: 0x" + System.Convert.ToString(ecOptions, 16));
            }
            hfilters = new SynWTFilter[1];
            vfilters = new SynWTFilter[1];
            hfilters[0] = readFilter(ehs, ms.spcoc_t);
            vfilters[0] = hfilters[0];
            SynWTFilter[][] hvfilters = new SynWTFilter[2][];
            hvfilters[0] = hfilters;
            hvfilters[1] = vfilters;
            ArrayList[] v = new ArrayList[2];
            v[0] = new ArrayList(10);
            v[1] = new ArrayList(10);
            int val = Syncfusion.Pdf.JPEG2000.codestream.Markers.PRECINCT_PARTITION_DEF_SIZE;
            if (!precinctPartitionIsUsed)
            {
                System.Int32 w, h;
                w = (System.Int32)(1 << (val & 0x000F));
                v[0].Add(w);
                h = (System.Int32)(1 << (((val & 0x00F0) >> 4)));
                v[1].Add(h);
            }
            else
            {
                ms.spcoc_ps = new int[mrl + 1];
                for (int rl = mrl; rl >= 0; rl--)
                {
                    System.Int32 w, h;
                    val = ms.spcoc_ps[rl] = ehs.ReadByte();
                    w = (System.Int32)(1 << (val & 0x000F));
                    v[0].Insert(0, w);
                    h = (System.Int32)(1 << (((val & 0x00F0) >> 4)));
                    v[1].Insert(0, h);
                }
            }
            if (mainh)
            {
                decSpec.pss.setCompDef(cComp, v);
            }
            else
            {
                decSpec.pss.setTileCompVal(tileIdx, cComp, v);
            }
            precinctPartitionIsUsed = true;
            checkMarkerLength(ehs, "COD marker");
            if (mainh)
            {
                hi.cocValue["main_c" + cComp] = ms;
                decSpec.wfs.setCompDef(cComp, hvfilters);
                decSpec.dls.setCompDef(cComp, (System.Object)mrl);
                decSpec.ecopts.setCompDef(cComp, (System.Object)ecOptions);
            }
            else
            {
                hi.cocValue["t" + tileIdx + "_c" + cComp] = ms;
                decSpec.wfs.setTileCompVal(tileIdx, cComp, hvfilters);
                decSpec.dls.setTileCompVal(tileIdx, cComp, (System.Object)mrl);
                decSpec.ecopts.setTileCompVal(tileIdx, cComp, (System.Object)ecOptions);
            }
        }
        private void readPOC(System.IO.BinaryReader ehs, bool mainh, int t, int tpIdx)
        {
            bool useShort = (nComp >= 256) ? true : false;
            int tmp;
            int nOldChg = 0;
            HeaderInformation.POC ms;
            if (mainh || hi.pocValue["t" + t] == null)
            {
                ms = hi.NewPOC;
            }
            else
            {
                ms = (HeaderInformation.POC)hi.pocValue["t" + t];
                nOldChg = ms.rspoc.Length;
            }
            ms.lpoc = ehs.ReadUInt16();
            int newChg = (ms.lpoc - 2) / (5 + (useShort ? 4 : 2));
            int ntotChg = nOldChg + newChg;
            int[][] change;
            if (nOldChg != 0)
            {
                int[][] tmpArray = new int[ntotChg][];
                for (int i = 0; i < ntotChg; i++)
                {
                    tmpArray[i] = new int[6];
                }
                change = tmpArray;
                int[] tmprspoc = new int[ntotChg];
                int[] tmpcspoc = new int[ntotChg];
                int[] tmplyepoc = new int[ntotChg];
                int[] tmprepoc = new int[ntotChg];
                int[] tmpcepoc = new int[ntotChg];
                int[] tmpppoc = new int[ntotChg];
                int[][] prevChg = (int[][])decSpec.pcs.getTileDef(t);
                for (int chg = 0; chg < nOldChg; chg++)
                {
                    change[chg] = prevChg[chg];
                    tmprspoc[chg] = ms.rspoc[chg];
                    tmpcspoc[chg] = ms.cspoc[chg];
                    tmplyepoc[chg] = ms.lyepoc[chg];
                    tmprepoc[chg] = ms.repoc[chg];
                    tmpcepoc[chg] = ms.cepoc[chg];
                    tmpppoc[chg] = ms.ppoc[chg];
                }
                ms.rspoc = tmprspoc;
                ms.cspoc = tmpcspoc;
                ms.lyepoc = tmplyepoc;
                ms.repoc = tmprepoc;
                ms.cepoc = tmpcepoc;
                ms.ppoc = tmpppoc;
            }
            else
            {
                int[][] tmpArray2 = new int[newChg][];
                for (int i2 = 0; i2 < newChg; i2++)
                {
                    tmpArray2[i2] = new int[6];
                }
                change = tmpArray2;
                ms.rspoc = new int[newChg];
                ms.cspoc = new int[newChg];
                ms.lyepoc = new int[newChg];
                ms.repoc = new int[newChg];
                ms.cepoc = new int[newChg];
                ms.ppoc = new int[newChg];
            }
            for (int chg = nOldChg; chg < ntotChg; chg++)
            {
                change[chg][0] = ms.rspoc[chg] = ehs.ReadByte();
                if (useShort)
                {
                    change[chg][1] = ms.cspoc[chg] = ehs.ReadUInt16();
                }
                else
                {
                    change[chg][1] = ms.cspoc[chg] = ehs.ReadByte();
                }
                change[chg][2] = ms.lyepoc[chg] = ehs.ReadUInt16();
                if (change[chg][2] < 1)
                {
                    throw new System.ArgumentException("LYEpoc value must be greater than 1 in POC marker " + "segment of tile " + t + ", tile-part " + tpIdx);
                }
                change[chg][3] = ms.repoc[chg] = ehs.ReadByte();
                if (change[chg][3] <= change[chg][0])
                {
                    throw new System.ArgumentException("REpoc value must be greater than RSpoc in POC marker " + "segment of tile " + t + ", tile-part " + tpIdx);
                }
                if (useShort)
                {
                    change[chg][4] = ms.cepoc[chg] = ehs.ReadUInt16();
                }
                else
                {
                    tmp = ms.cepoc[chg] = ehs.ReadByte();
                    if (tmp == 0)
                    {
                        change[chg][4] = 0;
                    }
                    else
                    {
                        change[chg][4] = tmp;
                    }
                }
                if (change[chg][4] <= change[chg][1])
                {
                    throw new System.ArgumentException("CEpoc value must be greater than CSpoc in POC marker " + "segment of tile " + t + ", tile-part " + tpIdx);
                }
                change[chg][5] = ms.ppoc[chg] = ehs.ReadByte();
            }
            checkMarkerLength(ehs, "POC marker");
            if (mainh)
            {
                hi.pocValue["main"] = ms;
                decSpec.pcs.setDefault(change);
            }
            else
            {
                hi.pocValue["t" + t] = ms;
                decSpec.pcs.setTileDef(t, change);
            }
        }
        private void readTLM(System.IO.BinaryReader ehs)
        {
            int length;
            length = ehs.ReadUInt16();
            System.IO.BinaryReader temp_BinaryReader;
            System.Int64 temp_Int64;
            temp_BinaryReader = ehs;
            temp_Int64 = temp_BinaryReader.BaseStream.Position;
            temp_Int64 = temp_BinaryReader.BaseStream.Seek(length - 2, System.IO.SeekOrigin.Current) - temp_Int64;
            int generatedAux = (int)temp_Int64;
            //FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.INFO, "Skipping unsupported TLM marker");
        }
        private void readPLM(System.IO.BinaryReader ehs)
        {
            int length;
            length = ehs.ReadUInt16();
            System.IO.BinaryReader temp_BinaryReader;
            System.Int64 temp_Int64;
            temp_BinaryReader = ehs;
            temp_Int64 = temp_BinaryReader.BaseStream.Position;
            temp_Int64 = temp_BinaryReader.BaseStream.Seek(length - 2, System.IO.SeekOrigin.Current) - temp_Int64;
            int generatedAux = (int)temp_Int64;
            //FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.INFO, "Skipping unsupported PLM marker");
        }
        private void readPLTFields(System.IO.BinaryReader ehs)
        {
            int length;
            length = ehs.ReadUInt16();
            System.IO.BinaryReader temp_BinaryReader;
            System.Int64 temp_Int64;
            temp_BinaryReader = ehs;
            temp_Int64 = temp_BinaryReader.BaseStream.Position;
            temp_Int64 = temp_BinaryReader.BaseStream.Seek(length - 2, System.IO.SeekOrigin.Current) - temp_Int64;
            int generatedAux = (int)temp_Int64;
            //FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.INFO, "Skipping unsupported PLT marker");
        }
        private void readRGN(System.IO.BinaryReader ehs, bool mainh, int tileIdx, int tpIdx)
        {
            int comp;
            HeaderInformation.RGN ms = hi.NewRGN;
            ms.lrgn = ehs.ReadUInt16();
            ms.crgn = comp = (nComp < 257) ? ehs.ReadByte() : ehs.ReadUInt16();
            if (comp >= nComp)
            {
                throw new System.ArgumentException("Invalid component " + "index in RGN marker" + comp);
            }
            ms.srgn = ehs.ReadByte();
            if (ms.srgn != Syncfusion.Pdf.JPEG2000.codestream.Markers.SRGN_IMPLICIT)
                throw new System.ArgumentException("Unknown or unsupported " + "Srgn parameter in ROI " + "marker");
            if (decSpec.rois == null)
            {
                decSpec.rois = new MaxShiftSpec(nTiles, nComp, ModuleSpec.SPEC_TYPE_TILE_COMP);
            }
            ms.sprgn = ehs.ReadByte();
            if (mainh)
            {
                hi.rgnValue["main_c" + comp] = ms;
                decSpec.rois.setCompDef(comp, (System.Object)ms.sprgn);
            }
            else
            {
                hi.rgnValue["t" + tileIdx + "_c" + comp] = ms;
                decSpec.rois.setTileCompVal(tileIdx, comp, (System.Object)ms.sprgn);
            }
            checkMarkerLength(ehs, "RGN marker");
        }
        private void readPPM(System.IO.BinaryReader ehs)
        {
            int curMarkSegLen;
            int indx;
            int remSegLen;
            if (pPMMarkerData == null)
            {
                pPMMarkerData = new byte[nPPMMarkSeg][];
                tileOfTileParts = new ArrayList(10);
                decSpec.pphs.setDefault((System.Object)true);
            }
            curMarkSegLen = ehs.ReadUInt16();
            remSegLen = curMarkSegLen - 3;
            indx = ehs.ReadByte();
            pPMMarkerData[indx] = new byte[remSegLen];
            ehs.BaseStream.Read(pPMMarkerData[indx], 0, remSegLen);
            checkMarkerLength(ehs, "PPM marker");
        }
        private void readPPT(System.IO.BinaryReader ehs, int tile, int tpIdx)
        {
            int curMarkSegLen;
            int indx;
            byte[] temp;
            if (tilePartPkdPktHeaders == null)
            {
                tilePartPkdPktHeaders = new byte[nTiles][][][];
            }
            if (tilePartPkdPktHeaders[tile] == null)
            {
                tilePartPkdPktHeaders[tile] = new byte[nTileParts[tile]][][];
            }
            if (tilePartPkdPktHeaders[tile][tpIdx] == null)
            {
                tilePartPkdPktHeaders[tile][tpIdx] = new byte[nPPTMarkSeg[tile][tpIdx]][];
            }
            curMarkSegLen = ehs.ReadUInt16();
            indx = ehs.ReadByte();
            temp = new byte[curMarkSegLen - 3];
            ehs.BaseStream.Read(temp, 0, temp.Length);
            tilePartPkdPktHeaders[tile][tpIdx][indx] = temp;
            checkMarkerLength(ehs, "PPT marker");
            decSpec.pphs.setTileDef(tile, (System.Object)true);
        }
        private void extractMainMarkSeg(short marker, JPXRandomAccessStream ehs)
        {
            if (nfMarkSeg == 0)
            {
                if (marker != Syncfusion.Pdf.JPEG2000.codestream.Markers.SIZ)
                {
                    throw new System.ArgumentException("First marker after " + "SOC " + "must be SIZ " + System.Convert.ToString(marker, 16));
                }
            }
            System.String htKey = "";
            if (ht == null)
            {
                ht = new HashTable();
            }
            switch (marker)
            {
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.SIZ:
                    if ((nfMarkSeg & SIZ_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one SIZ marker " + "segment found in main " + "header");
                    }
                    nfMarkSeg |= SIZ_FOUND;
                    htKey = "SIZ";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.SOD:
                    throw new System.ArgumentException("SOD found in main header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.EOC:
                    throw new System.ArgumentException("EOC found in main header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.SOT:
                    if ((nfMarkSeg & SOT_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one SOT " + "marker " + "found right after " + "main " + "or tile header");
                    }
                    nfMarkSeg |= SOT_FOUND;
                    return;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.COD:
                    if ((nfMarkSeg & COD_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one COD " + "marker " + "found in main header");
                    }
                    nfMarkSeg |= COD_FOUND;
                    htKey = "COD";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.COC:
                    nfMarkSeg |= COC_FOUND;
                    htKey = "COC" + (nCOCMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.QCD:
                    if ((nfMarkSeg & QCD_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one QCD " + "marker " + "found in main header");
                    }
                    nfMarkSeg |= QCD_FOUND;
                    htKey = "QCD";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.QCC:
                    nfMarkSeg |= QCC_FOUND;
                    htKey = "QCC" + (nQCCMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.RGN:
                    nfMarkSeg |= RGN_FOUND;
                    htKey = "RGN" + (nRGNMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.COM:
                    nfMarkSeg |= COM_FOUND;
                    htKey = "COM" + (nCOMMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.CRG:
                    if ((nfMarkSeg & CRG_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one CRG " + "marker " + "found in main header");
                    }
                    nfMarkSeg |= CRG_FOUND;
                    htKey = "CRG";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.PPM:
                    nfMarkSeg |= PPM_FOUND;
                    htKey = "PPM" + (nPPMMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.TLM:
                    if ((nfMarkSeg & TLM_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one TLM " + "marker " + "found in main header");
                    }
                    nfMarkSeg |= TLM_FOUND;
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.PLM:
                    if ((nfMarkSeg & PLM_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one PLM " + "marker " + "found in main header");
                    }
                    //FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "PLM marker segment found but " + "not used by by JJ2000 decoder.");
                    nfMarkSeg |= PLM_FOUND;
                    htKey = "PLM";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.POC:
                    if ((nfMarkSeg & POC_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one POC " + "marker segment found " + "in main header");
                    }
                    nfMarkSeg |= POC_FOUND;
                    htKey = "POC";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.PLT:
                    throw new System.ArgumentException("PLT found in main header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.PPT:
                    throw new System.ArgumentException("PPT found in main header");
                default:
                    htKey = "UNKNOWN";
                    //FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Non recognized marker segment (0x" + System.Convert.ToString(marker, 16) + ") in main header!");
                    break;
            }
            if (marker < unchecked((short)0xffffff30) || marker > unchecked((short)0xffffff3f))
            {
                // Read marker segment length and create corresponding byte buffer
                int markSegLen = ehs.readUnsignedShort();
                byte[] buf = new byte[markSegLen];
                // Copy data (after re-insertion of the marker segment length);
                buf[0] = (byte)((markSegLen >> 8) & 0xFF);
                buf[1] = (byte)(markSegLen & 0xFF);
                ehs.readFully(buf, 2, markSegLen - 2);
                if (!htKey.Equals("UNKNOWN"))
                {
                    // Store array in hashTable
                    ht[htKey] = buf;
                }
            }
        }
        internal virtual void extractTilePartMarkSeg(short marker, JPXRandomAccessStream ehs, int tileIdx, int tilePartIdx)
        {
            System.String htKey = "";
            if (ht == null)
            {
                ht = new HashTable();
            }
            switch (marker)
            {
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.SOT:
                    throw new System.ArgumentException("Second SOT marker " + "segment found in tile-" + "part header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.SIZ:
                    throw new System.ArgumentException("SIZ found in tile-part" + " header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.EOC:
                    throw new System.ArgumentException("EOC found in tile-part" + " header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.TLM:
                    throw new System.ArgumentException("TLM found in tile-part" + " header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.PLM:
                    throw new System.ArgumentException("PLM found in tile-part" + " header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.PPM:
                    throw new System.ArgumentException("PPM found in tile-part" + " header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.COD:
                    if ((nfMarkSeg & COD_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one COD " + "marker " + "found in tile-part" + " header");
                    }
                    nfMarkSeg |= COD_FOUND;
                    htKey = "COD";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.COC:
                    nfMarkSeg |= COC_FOUND;
                    htKey = "COC" + (nCOCMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.QCD:
                    if ((nfMarkSeg & QCD_FOUND) != 0)
                    {
                        throw new System.ArgumentException("More than one QCD " + "marker " + "found in tile-part" + " header");
                    }
                    nfMarkSeg |= QCD_FOUND;
                    htKey = "QCD";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.QCC:
                    nfMarkSeg |= QCC_FOUND;
                    htKey = "QCC" + (nQCCMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.RGN:
                    nfMarkSeg |= RGN_FOUND;
                    htKey = "RGN" + (nRGNMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.COM:
                    nfMarkSeg |= COM_FOUND;
                    htKey = "COM" + (nCOMMarkSeg++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.CRG:
                    throw new System.ArgumentException("CRG marker found in " + "tile-part header");
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.PPT:
                    nfMarkSeg |= PPT_FOUND;
                    if (nPPTMarkSeg == null)
                    {
                        nPPTMarkSeg = new int[nTiles][];
                    }
                    if (nPPTMarkSeg[tileIdx] == null)
                    {
                        nPPTMarkSeg[tileIdx] = new int[nTileParts[tileIdx]];
                    }
                    htKey = "PPT" + (nPPTMarkSeg[tileIdx][tilePartIdx]++);
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.SOD:
                    nfMarkSeg |= SOD_FOUND;
                    return;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.POC:
                    if ((nfMarkSeg & POC_FOUND) != 0)
                        throw new System.ArgumentException("More than one POC " + "marker segment found " + "in tile-part" + " header");
                    nfMarkSeg |= POC_FOUND;
                    htKey = "POC";
                    break;
                case Syncfusion.Pdf.JPEG2000.codestream.Markers.PLT:
                    if ((nfMarkSeg & PLM_FOUND) != 0)
                    {
                        throw new System.ArgumentException("PLT marker found even" + "though PLM marker " + "found in main header");
                    }
                    //FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "PLT marker segment found but " + "not used by JJ2000 decoder.");
                    htKey = "UNKNOWN";
                    break;
                default:
                    htKey = "UNKNOWN";
                    //FacilityManager.getMsgLogger().printmsg(CSJ2K.j2k.util.MsgLogger_Fields.WARNING, "Non recognized marker segment (0x" + System.Convert.ToString(marker, 16) + ") in tile-part header" + " of tile " + tileIdx + " !");
                    break;
            }
            int markSegLen = ehs.readUnsignedShort();
            byte[] buf = new byte[markSegLen];
            buf[0] = (byte)((markSegLen >> 8) & 0xFF);
            buf[1] = (byte)(markSegLen & 0xFF);
            ehs.readFully(buf, 2, markSegLen - 2);
            if (!htKey.Equals("UNKNOWN"))
            {
                ht[htKey] = buf;
            }
        }
        private void readFoundMainMarkSeg()
        {
            System.IO.MemoryStream bais;
            if ((nfMarkSeg & SIZ_FOUND) != 0)
            {
                bais = new System.IO.MemoryStream((byte[])ht["SIZ"]);
                readSIZ(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true));
            }
            if ((nfMarkSeg & COM_FOUND) != 0)
            {
                for (int i = 0; i < nCOMMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["COM" + i]);
                    readCOM(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), true, 0, i);
                }
            }
            if ((nfMarkSeg & CRG_FOUND) != 0)
            {
                bais = new System.IO.MemoryStream((byte[])ht["CRG"]);
                readCRG(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true));
            }
            if ((nfMarkSeg & COD_FOUND) != 0)
            {
                bais = new System.IO.MemoryStream((byte[])ht["COD"]);
                readCOD(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), true, 0, 0);
            }
            if ((nfMarkSeg & COC_FOUND) != 0)
            {
                for (int i = 0; i < nCOCMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["COC" + i]);
                    readCOC(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), true, 0, 0);
                }
            }
            if ((nfMarkSeg & RGN_FOUND) != 0)
            {
                for (int i = 0; i < nRGNMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["RGN" + i]);
                    readRGN(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), true, 0, 0);
                }
            }
            if ((nfMarkSeg & QCD_FOUND) != 0)
            {
                bais = new System.IO.MemoryStream((byte[])ht["QCD"]);
                readQCD(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), true, 0, 0);
            }
            if ((nfMarkSeg & QCC_FOUND) != 0)
            {
                for (int i = 0; i < nQCCMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["QCC" + i]);
                    readQCC(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), true, 0, 0);
                }
            }
            if ((nfMarkSeg & POC_FOUND) != 0)
            {
                bais = new System.IO.MemoryStream((byte[])ht["POC"]);
                readPOC(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), true, 0, 0);
            }
            if ((nfMarkSeg & PPM_FOUND) != 0)
            {
                for (int i = 0; i < nPPMMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["PPM" + i]);
                    readPPM(new Syncfusion.Pdf.Util.EndianBinaryReader(bais));
                }
            }
            ht = null;
        }
        public virtual void readFoundTilePartMarkSeg(int tileIdx, int tpIdx)
        {
            System.IO.MemoryStream bais;
            if ((nfMarkSeg & COD_FOUND) != 0)
            {
                bais = new System.IO.MemoryStream((byte[])ht["COD"]);
                readCOD(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), false, tileIdx, tpIdx);
            }
            if ((nfMarkSeg & COC_FOUND) != 0)
            {
                for (int i = 0; i < nCOCMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["COC" + i]);
                    readCOC(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), false, tileIdx, tpIdx);
                }
            }
            if ((nfMarkSeg & RGN_FOUND) != 0)
            {
                for (int i = 0; i < nRGNMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["RGN" + i]);
                    readRGN(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), false, tileIdx, tpIdx);
                }
            }
            if ((nfMarkSeg & QCD_FOUND) != 0)
            {
                bais = new System.IO.MemoryStream((byte[])ht["QCD"]);
                readQCD(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), false, tileIdx, tpIdx);
            }
            if ((nfMarkSeg & QCC_FOUND) != 0)
            {
                for (int i = 0; i < nQCCMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["QCC" + i]);
                    readQCC(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), false, tileIdx, tpIdx);
                }
            }
            if ((nfMarkSeg & POC_FOUND) != 0)
            {
                bais = new System.IO.MemoryStream((byte[])ht["POC"]);
                readPOC(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), false, tileIdx, tpIdx);
            }
            if ((nfMarkSeg & COM_FOUND) != 0)
            {
                for (int i = 0; i < nCOMMarkSeg; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["COM" + i]);
                    readCOM(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), false, tileIdx, i);
                }
            }
            if ((nfMarkSeg & PPT_FOUND) != 0)
            {
                for (int i = 0; i < nPPTMarkSeg[tileIdx][tpIdx]; i++)
                {
                    bais = new System.IO.MemoryStream((byte[])ht["PPT" + i]);
                    readPPT(new Syncfusion.Pdf.Util.EndianBinaryReader(bais, true), tileIdx, tpIdx);
                }
            }
            ht = null;
        }
        internal HeaderDecoder(JPXRandomAccessStream ehs, JPXParameters pl, HeaderInformation hi)
        {
            this.hi = hi;
            pl.checkList(OPT_PREFIX, JPXParameters.toNameArray(pinfo));
            mainHeadOff = ehs.Pos;
            if (((short)ehs.readShort()) != Syncfusion.Pdf.JPEG2000.codestream.Markers.SOC)
            {
                throw new System.ArgumentException("SOC marker segment not " + " found at the " + "beginning of the " + "codestream.");
            }
            nfMarkSeg = 0;
            do
            {
                extractMainMarkSeg(ehs.readShort(), ehs);
            }
            while ((nfMarkSeg & SOT_FOUND) == 0);
            ehs.seek(ehs.Pos - 2);
            readFoundMainMarkSeg();
        }
        internal virtual EntropyDecoder createEntropyDecoder(CodedCBlkDataSrcDec src, JPXParameters pl)
        {
            bool doer;
            bool verber;
            int mMax;
            pl.checkList(EntropyDecoder.OPT_PREFIX, JPXParameters.toNameArray(EntropyDecoder.ParameterInfo));
            doer = pl.getBooleanParameter("Cer");
            verber = pl.getBooleanParameter("Cverber");
            mMax = pl.getIntParameter("m_quit");
            return new StdEntropyDecoder(src, decSpec, doer, verber, mMax);
        }
        internal virtual DeScalerROI createROIDeScaler(CBlkQuantDataSrcDec src, JPXParameters pl, DecodeHelper decSpec2)
        {
            return DeScalerROI.createInstance(src, pl, decSpec2);
        }
        public virtual void resetHeaderMarkers()
        {
            // The found status of PLM remains since only PLM OR PLT allowed
            // Same goes for PPM and PPT
            nfMarkSeg = nfMarkSeg & (PLM_FOUND | PPM_FOUND);
            nCOCMarkSeg = 0;
            nQCCMarkSeg = 0;
            nCOMMarkSeg = 0;
            nRGNMarkSeg = 0;
        }
        public override System.String ToString()
        {
            return hdStr;
        }
        public virtual System.IO.MemoryStream getPackedPktHead(int tile)
        {
            if (pkdPktHeaders == null)
            {
                int i, t;
                pkdPktHeaders = new System.IO.MemoryStream[nTiles];
                for (i = nTiles - 1; i >= 0; i--)
                {
                    pkdPktHeaders[i] = new System.IO.MemoryStream();
                }
                if (nPPMMarkSeg != 0)
                {
                    int nppm;
                    int nTileParts = tileOfTileParts.Count;
                    byte[] temp;
                    System.IO.MemoryStream pph;
                    System.IO.MemoryStream allNppmIppm = new System.IO.MemoryStream();
                    for (i = 0; i < nPPMMarkSeg; i++)
                    {
                        byte[] temp_byteArray;
                        temp_byteArray = pPMMarkerData[i];
                        allNppmIppm.Write(temp_byteArray, 0, temp_byteArray.Length);
                    }
                    pph = new System.IO.MemoryStream(allNppmIppm.ToArray());
                    for (i = 0; i < nTileParts; i++)
                    {
                        t = ((System.Int32)tileOfTileParts[i]);
                        nppm = (pph.ReadByte() << 24) | (pph.ReadByte() << 16) | (pph.ReadByte() << 8) | (pph.ReadByte());
                        temp = new byte[nppm];
                        pph.Read(temp, 0, temp.Length);
                        byte[] temp_byteArray2;
                        temp_byteArray2 = temp;
                        pkdPktHeaders[t].Write(temp_byteArray2, 0, temp_byteArray2.Length);
                    }
                }
                else
                {
                    int tp;
                    for (t = nTiles - 1; t >= 0; t--)
                    {
                        for (tp = 0; tp < nTileParts[t]; tp++)
                        {
                            for (i = 0; i < nPPTMarkSeg[t][tp]; i++)
                            {
                                byte[] temp_byteArray3;
                                temp_byteArray3 = tilePartPkdPktHeaders[t][tp][i];
                                pkdPktHeaders[t].Write(temp_byteArray3, 0, temp_byteArray3.Length);
                            }
                        }
                    }
                }
            }
            return new System.IO.MemoryStream(pkdPktHeaders[tile].ToArray());
        }
    }
}