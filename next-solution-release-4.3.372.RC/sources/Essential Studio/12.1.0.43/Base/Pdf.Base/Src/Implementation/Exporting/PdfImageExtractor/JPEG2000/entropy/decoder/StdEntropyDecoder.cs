#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
using Syncfusion.Pdf.JPEG2000.wavelet;
using Syncfusion.Pdf.JPEG2000.entropy;
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
using Syncfusion.Pdf.JPEG2000.util;
using Syncfusion.Pdf.JPEG2000.io;
using Syncfusion.Pdf.JPEG2000;
namespace Syncfusion.Pdf.JPEG2000.entropy.decoder
{
    internal class StdEntropyDecoder : EntropyDecoder
    {
        private const bool DO_TIMING = false;
        private ByteToBitInput bin;
        private MQDecoder mq;
        private DecodeHelper decSpec;
        private int options;
        private bool doer;
        private bool verber;
        private const int ZC_LUT_BITS = 8;
        private static readonly int[] ZC_LUT_LH = new int[1 << ZC_LUT_BITS];
        private static readonly int[] ZC_LUT_HL = new int[1 << ZC_LUT_BITS];
        private static readonly int[] ZC_LUT_HH = new int[1 << ZC_LUT_BITS];
        private const int SC_LUT_BITS = 9;
        private static readonly int[] SC_LUT = new int[1 << SC_LUT_BITS];
        private const int SC_LUT_MASK = (1 << 4) - 1;
        private const int SC_SPRED_SHIFT = 31;
        private const int INT_SIGN_BIT = 1 << 31;
        private const int MR_LUT_BITS = 9;
        private static readonly int[] MR_LUT = new int[1 << MR_LUT_BITS];
        private const int NUM_CTXTS = 19;
        private const int RLC_CTXT = 1;
        private const int UNIF_CTXT = 0;
        private static readonly int[] MQ_INIT = new int[] { 46, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private const int SEG_SYMBOL = 10;
        private int[] state;
        private const int STATE_SEP = 16;
        private const int STATE_SIG_R1 = 1 << 15;
        private const int STATE_VISITED_R1 = 1 << 14;
        private const int STATE_NZ_CTXT_R1 = 1 << 13;
        private const int STATE_H_L_SIGN_R1 = 1 << 12;
        private const int STATE_H_R_SIGN_R1 = 1 << 11;
        private const int STATE_V_U_SIGN_R1 = 1 << 10;
        private const int STATE_V_D_SIGN_R1 = 1 << 9;
        private const int STATE_PREV_MR_R1 = 1 << 8;
        private const int STATE_H_L_R1 = 1 << 7;
        private const int STATE_H_R_R1 = 1 << 6;
        private const int STATE_V_U_R1 = 1 << 5;
        private const int STATE_V_D_R1 = 1 << 4;
        private const int STATE_D_UL_R1 = 1 << 3;
        private const int STATE_D_UR_R1 = 1 << 2;
        private const int STATE_D_DL_R1 = 1 << 1;
        private const int STATE_D_DR_R1 = 1;
        private static readonly int STATE_SIG_R2 = STATE_SIG_R1 << STATE_SEP;
        private static readonly int STATE_VISITED_R2 = STATE_VISITED_R1 << STATE_SEP;
        private static readonly int STATE_NZ_CTXT_R2 = STATE_NZ_CTXT_R1 << STATE_SEP;
        private static readonly int STATE_H_L_SIGN_R2 = STATE_H_L_SIGN_R1 << STATE_SEP;
        private static readonly int STATE_H_R_SIGN_R2 = STATE_H_R_SIGN_R1 << STATE_SEP;
        private static readonly int STATE_V_U_SIGN_R2 = STATE_V_U_SIGN_R1 << STATE_SEP;
        private static readonly int STATE_V_D_SIGN_R2 = STATE_V_D_SIGN_R1 << STATE_SEP;
        private static readonly int STATE_PREV_MR_R2 = STATE_PREV_MR_R1 << STATE_SEP;
        private static readonly int STATE_H_L_R2 = STATE_H_L_R1 << STATE_SEP;
        private static readonly int STATE_H_R_R2 = STATE_H_R_R1 << STATE_SEP;
        private static readonly int STATE_V_U_R2 = STATE_V_U_R1 << STATE_SEP;
        private static readonly int STATE_V_D_R2 = STATE_V_D_R1 << STATE_SEP;
        private static readonly int STATE_D_UL_R2 = STATE_D_UL_R1 << STATE_SEP;
        private static readonly int STATE_D_UR_R2 = STATE_D_UR_R1 << STATE_SEP;
        private static readonly int STATE_D_DL_R2 = STATE_D_DL_R1 << STATE_SEP;
        private static readonly int STATE_D_DR_R2 = STATE_D_DR_R1 << STATE_SEP;
        private static readonly int SIG_MASK_R1R2 = STATE_SIG_R1 | STATE_SIG_R2;
        private static readonly int VSTD_MASK_R1R2 = STATE_VISITED_R1 | STATE_VISITED_R2;
        private static readonly int RLC_MASK_R1R2 = STATE_SIG_R1 | STATE_SIG_R2 | STATE_VISITED_R1 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2;
        private const int ZC_MASK = (1 << 8) - 1;
        private const int SC_SHIFT_R1 = 4;
        private static readonly int SC_SHIFT_R2 = SC_SHIFT_R1 + STATE_SEP;
        private static readonly int SC_MASK = (1 << SC_LUT_BITS) - 1;
        private const int MR_MASK = (1 << 9) - 1;
        private DecLyrdCBlk srcblk;
        private int mQuit;
        internal StdEntropyDecoder(CodedCBlkDataSrcDec src, DecodeHelper decSpec, bool doer, bool verber, int mQuit)
            : base(src)
        {
            this.decSpec = decSpec;
            this.doer = doer;
            this.verber = verber;
            this.mQuit = mQuit;
            state = new int[(decSpec.cblks.MaxCBlkWidth + 2) * ((decSpec.cblks.MaxCBlkHeight + 1) / 2 + 2)];
        }
        public override DataBlock getCodeBlock(int c, int m, int n, SubbandSyn sb, DataBlock cblk)
        {
            int[] zc_lut;
            int[] out_data;
            int npasses;
            int curbp;
            bool error;
            int tslen;
            int tsidx;
            ByteInputBuffer in_Renamed = null;
            bool isterm;
            srcblk = src.getCodeBlock(c, m, n, sb, 1, -1, srcblk);
            options = ((System.Int32)decSpec.ecopts.getTileCompVal(tIdx, c));
            ArrayUtil.intArraySet(state, 0);
            if (cblk == null)
                cblk = new DataBlockInt();
            cblk.progressive = srcblk.prog;
            cblk.ulx = srcblk.ulx;
            cblk.uly = srcblk.uly;
            cblk.w = srcblk.w;
            cblk.h = srcblk.h;
            cblk.offset = 0;
            cblk.scanw = cblk.w;
            out_data = (int[])cblk.Data;
            if (out_data == null || out_data.Length < srcblk.w * srcblk.h)
            {
                out_data = new int[srcblk.w * srcblk.h];
                cblk.Data = out_data;
            }
            else
            {
                ArrayUtil.intArraySet(out_data, 0);
            }
            if (srcblk.nl <= 0 || srcblk.nTrunc <= 0)
            {
                return cblk;
            }
            tslen = (srcblk.tsLengths == null) ? srcblk.dl : srcblk.tsLengths[0];
            tsidx = 0;
            npasses = srcblk.nTrunc;
            if (mq == null)
            {
                in_Renamed = new ByteInputBuffer(srcblk.data, 0, tslen);
                mq = new MQDecoder(in_Renamed, NUM_CTXTS, MQ_INIT);
            }
            else
            {
                mq.nextSegment(srcblk.data, 0, tslen);
                mq.resetCtxts();
            }
            error = false;
            if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0)
            {
                if (bin == null)
                {
                    if (in_Renamed == null)
                        in_Renamed = mq.ByteInputBuffer;
                    bin = new ByteToBitInput(in_Renamed);
                }
            }
            switch (sb.orientation)
            {
                case Subband.WT_ORIENT_HL:
                    zc_lut = ZC_LUT_HL;
                    break;
                case Subband.WT_ORIENT_LH:
                case Subband.WT_ORIENT_LL:
                    zc_lut = ZC_LUT_LH;
                    break;
                case Subband.WT_ORIENT_HH:
                    zc_lut = ZC_LUT_HH;
                    break;
                default:
                    throw new System.Exception("JJ2000 internal error");
            }
            curbp = 30 - srcblk.skipMSBP;
            if (mQuit != -1 && (mQuit * 3 - 2) < npasses)
            {
                npasses = mQuit * 3 - 2;
            }
            if (curbp >= 0 && npasses > 0)
            {
                isterm = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0 || ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0 && (31 - Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_NON_BYPASS_MS_BP - srcblk.skipMSBP) >= curbp);
                error = cleanuppass(cblk, mq, curbp, state, zc_lut, isterm);
                npasses--;
                if (!error || !doer)
                    curbp--;
            }
            if (!error || !doer)
            {
                while (curbp >= 0 && npasses > 0)
                {
                    if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0 && (curbp < 31 - Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_NON_BYPASS_MS_BP - srcblk.skipMSBP))
                    {
                        bin.setByteArray(null, -1, srcblk.tsLengths[++tsidx]);
                        isterm = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0;
                        error = rawSigProgPass(cblk, bin, curbp, state, isterm);
                        npasses--;
                        if (npasses <= 0 || (error && doer))
                            break;
                        if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0)
                        {
                            bin.setByteArray(null, -1, srcblk.tsLengths[++tsidx]);
                        }
                        isterm = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0 || ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0 && (31 - Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_NON_BYPASS_MS_BP - srcblk.skipMSBP > curbp));
                        error = rawMagRefPass(cblk, bin, curbp, state, isterm);
                    }
                    else
                    {
                        if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0)
                        {
                            mq.nextSegment(null, -1, srcblk.tsLengths[++tsidx]);
                        }
                        isterm = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0;
                        error = sigProgPass(cblk, mq, curbp, state, zc_lut, isterm);
                        npasses--;
                        if (npasses <= 0 || (error && doer))
                            break;
                        if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0)
                        {
                            mq.nextSegment(null, -1, srcblk.tsLengths[++tsidx]);
                        }
                        isterm = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0 || ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0 && (31 - Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_NON_BYPASS_MS_BP - srcblk.skipMSBP > curbp));
                        error = magRefPass(cblk, mq, curbp, state, isterm);
                    }
                    npasses--;
                    if (npasses <= 0 || (error && doer))
                        break;
                    if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0 || ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0 && (curbp < 31 - Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_NON_BYPASS_MS_BP - srcblk.skipMSBP)))
                    {
                        mq.nextSegment(null, -1, srcblk.tsLengths[++tsidx]);
                    }
                    isterm = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_TERM_PASS) != 0 || ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_BYPASS) != 0 && (31 - Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.NUM_NON_BYPASS_MS_BP - srcblk.skipMSBP) >= curbp);
                    error = cleanuppass(cblk, mq, curbp, state, zc_lut, isterm);
                    npasses--;
                    if (error && doer)
                        break;
                    curbp--;
                }
            }
            if (error && doer)
            {
                if (verber)
                {
                }
                conceal(cblk, curbp);
            }
            return cblk;
        }
        public override DataBlock getInternCodeBlock(int c, int m, int n, SubbandSyn sb, DataBlock cblk)
        {
            return getCodeBlock(c, m, n, sb, cblk);
        }
        private bool sigProgPass(DataBlock cblk, MQDecoder mq, int bp, int[] state, int[] zc_lut, bool isterm)
        {
            int j, sj;
            int k, sk;
            int dscanw;
            int sscanw;
            int jstep;
            int kstep;
            int stopsk;
            int csj;
            int setmask;
            int sym;
            int ctxt;
            int[] data;
            int s;
            bool causal;
            int nstripes;
            int sheight;
            int off_ul, off_ur, off_dr, off_dl;
            bool error;
            dscanw = cblk.scanw;
            sscanw = cblk.w + 2;
            jstep = sscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT / 2 - cblk.w;
            kstep = dscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - cblk.w;
            setmask = (3 << bp) >> 1;
            data = (int[])cblk.Data;
            nstripes = (cblk.h + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - 1) / Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
            causal = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_VERT_STR_CAUSAL) != 0;
            off_ul = -sscanw - 1;
            off_ur = -sscanw + 1;
            off_dr = sscanw + 1;
            off_dl = sscanw - 1;
            sk = cblk.offset;
            sj = sscanw + 1;
            for (s = nstripes - 1; s >= 0; s--, sk += kstep, sj += jstep)
            {
                sheight = (s != 0) ? Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT : cblk.h - (nstripes - 1) * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
                stopsk = sk + cblk.w;
                for (; sk < stopsk; sk++, sj++)
                {
                    j = sj;
                    csj = state[j];
                    if ((((~csj) & (csj << 2)) & SIG_MASK_R1R2) != 0)
                    {
                        k = sk;
                        if ((csj & (STATE_SIG_R1 | STATE_NZ_CTXT_R1)) == STATE_NZ_CTXT_R1)
                        {
                            if (mq.decodeSymbol(zc_lut[csj & ZC_MASK]) != 0)
                            {
                                ctxt = SC_LUT[(SupportClass.URShift(csj, SC_SHIFT_R1)) & SC_MASK];
                                sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                data[k] = (sym << 31) | setmask;
                                if (!causal)
                                {
                                    state[j + off_ul] |= STATE_NZ_CTXT_R2 | STATE_D_DR_R2;
                                    state[j + off_ur] |= STATE_NZ_CTXT_R2 | STATE_D_DL_R2;
                                }
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2 | STATE_V_U_SIGN_R2;
                                    if (!causal)
                                    {
                                        state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2 | STATE_V_D_SIGN_R2;
                                    }
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_H_L_SIGN_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_H_R_SIGN_R1 | STATE_D_UR_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2;
                                    if (!causal)
                                    {
                                        state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2;
                                    }
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_D_UR_R2;
                                }
                            }
                            else
                            {
                                csj |= STATE_VISITED_R1;
                            }
                        }
                        if (sheight < 2)
                        {
                            state[j] = csj;
                            continue;
                        }
                        if ((csj & (STATE_SIG_R2 | STATE_NZ_CTXT_R2)) == STATE_NZ_CTXT_R2)
                        {
                            k += dscanw;
                            if (mq.decodeSymbol(zc_lut[(SupportClass.URShift(csj, STATE_SEP)) & ZC_MASK]) != 0)
                            {
                                ctxt = SC_LUT[(SupportClass.URShift(csj, SC_SHIFT_R2)) & SC_MASK];
                                sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                data[k] = (sym << 31) | setmask;
                                state[j + off_dl] |= STATE_NZ_CTXT_R1 | STATE_D_UR_R1;
                                state[j + off_dr] |= STATE_NZ_CTXT_R1 | STATE_D_UL_R1;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1 | STATE_V_D_SIGN_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1 | STATE_V_U_SIGN_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2 | STATE_H_L_SIGN_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2 | STATE_H_R_SIGN_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2;
                                }
                            }
                            else
                            {
                                csj |= STATE_VISITED_R2;
                            }
                        }
                        state[j] = csj;
                    }
                    if (sheight < 3)
                        continue;
                    j += sscanw;
                    csj = state[j];
                    if ((((~csj) & (csj << 2)) & SIG_MASK_R1R2) != 0)
                    {
                        k = sk + (dscanw << 1);
                        if ((csj & (STATE_SIG_R1 | STATE_NZ_CTXT_R1)) == STATE_NZ_CTXT_R1)
                        {
                            if (mq.decodeSymbol(zc_lut[csj & ZC_MASK]) != 0)
                            {
                                ctxt = SC_LUT[(SupportClass.URShift(csj, SC_SHIFT_R1)) & SC_MASK];
                                sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                data[k] = (sym << 31) | setmask;
                                state[j + off_ul] |= STATE_NZ_CTXT_R2 | STATE_D_DR_R2;
                                state[j + off_ur] |= STATE_NZ_CTXT_R2 | STATE_D_DL_R2;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2 | STATE_V_U_SIGN_R2;
                                    state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2 | STATE_V_D_SIGN_R2;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_H_L_SIGN_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_H_R_SIGN_R1 | STATE_D_UR_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2;
                                    state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_D_UR_R2;
                                }
                            }
                            else
                            {
                                csj |= STATE_VISITED_R1;
                            }
                        }
                        if (sheight < 4)
                        {
                            state[j] = csj;
                            continue;
                        }
                        if ((csj & (STATE_SIG_R2 | STATE_NZ_CTXT_R2)) == STATE_NZ_CTXT_R2)
                        {
                            k += dscanw;
                            if (mq.decodeSymbol(zc_lut[(SupportClass.URShift(csj, STATE_SEP)) & ZC_MASK]) != 0)
                            {
                                ctxt = SC_LUT[(SupportClass.URShift(csj, SC_SHIFT_R2)) & SC_MASK];
                                sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                data[k] = (sym << 31) | setmask;
                                state[j + off_dl] |= STATE_NZ_CTXT_R1 | STATE_D_UR_R1;
                                state[j + off_dr] |= STATE_NZ_CTXT_R1 | STATE_D_UL_R1;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1 | STATE_V_D_SIGN_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1 | STATE_V_U_SIGN_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2 | STATE_H_L_SIGN_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2 | STATE_H_R_SIGN_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2;
                                }
                            }
                            else
                            {
                                csj |= STATE_VISITED_R2;
                            }
                        }
                        state[j] = csj;
                    }
                }
            }
            error = false;
            if (isterm && (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_PRED_TERM) != 0)
            {
                error = mq.checkPredTerm();
            }
            if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_RESET_MQ) != 0)
            {
                mq.resetCtxts();
            }
            return error;
        }
        private bool rawSigProgPass(DataBlock cblk, ByteToBitInput bin, int bp, int[] state, bool isterm)
        {
            int j, sj;
            int k, sk;
            int dscanw;
            int sscanw;
            int jstep;
            int kstep;
            int stopsk;
            int csj;
            int setmask;
            int sym;
            int[] data;
            int s;
            bool causal;
            int nstripes;
            int sheight;
            int off_ul, off_ur, off_dr, off_dl;
            bool error;
            dscanw = cblk.scanw;
            sscanw = cblk.w + 2;
            jstep = sscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT / 2 - cblk.w;
            kstep = dscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - cblk.w;
            setmask = (3 << bp) >> 1;
            data = (int[])cblk.Data;
            nstripes = (cblk.h + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - 1) / Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
            causal = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_VERT_STR_CAUSAL) != 0;
            off_ul = -sscanw - 1;
            off_ur = -sscanw + 1;
            off_dr = sscanw + 1;
            off_dl = sscanw - 1;
            sk = cblk.offset;
            sj = sscanw + 1;
            for (s = nstripes - 1; s >= 0; s--, sk += kstep, sj += jstep)
            {
                sheight = (s != 0) ? Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT : cblk.h - (nstripes - 1) * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
                stopsk = sk + cblk.w;
                for (; sk < stopsk; sk++, sj++)
                {
                    j = sj;
                    csj = state[j];
                    if ((((~csj) & (csj << 2)) & SIG_MASK_R1R2) != 0)
                    {
                        k = sk;
                        if ((csj & (STATE_SIG_R1 | STATE_NZ_CTXT_R1)) == STATE_NZ_CTXT_R1)
                        {
                            if (bin.readBit() != 0)
                            {
                                sym = bin.readBit();
                                data[k] = (sym << 31) | setmask;
                                if (!causal)
                                {
                                    state[j + off_ul] |= STATE_NZ_CTXT_R2 | STATE_D_DR_R2;
                                    state[j + off_ur] |= STATE_NZ_CTXT_R2 | STATE_D_DL_R2;
                                }
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2 | STATE_V_U_SIGN_R2;
                                    if (!causal)
                                    {
                                        state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2 | STATE_V_D_SIGN_R2;
                                    }
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_H_L_SIGN_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_H_R_SIGN_R1 | STATE_D_UR_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2;
                                    if (!causal)
                                    {
                                        state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2;
                                    }
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_D_UR_R2;
                                }
                            }
                            else
                            {
                                csj |= STATE_VISITED_R1;
                            }
                        }
                        if (sheight < 2)
                        {
                            state[j] = csj;
                            continue;
                        }
                        if ((csj & (STATE_SIG_R2 | STATE_NZ_CTXT_R2)) == STATE_NZ_CTXT_R2)
                        {
                            k += dscanw;
                            if (bin.readBit() != 0)
                            {
                                sym = bin.readBit();
                                data[k] = (sym << 31) | setmask;
                                state[j + off_dl] |= STATE_NZ_CTXT_R1 | STATE_D_UR_R1;
                                state[j + off_dr] |= STATE_NZ_CTXT_R1 | STATE_D_UL_R1;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1 | STATE_V_D_SIGN_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1 | STATE_V_U_SIGN_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2 | STATE_H_L_SIGN_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2 | STATE_H_R_SIGN_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2;
                                }
                            }
                            else
                            {
                                csj |= STATE_VISITED_R2;
                            }
                        }
                        state[j] = csj;
                    }
                    if (sheight < 3)
                        continue;
                    j += sscanw;
                    csj = state[j];
                    if ((((~csj) & (csj << 2)) & SIG_MASK_R1R2) != 0)
                    {
                        k = sk + (dscanw << 1);
                        if ((csj & (STATE_SIG_R1 | STATE_NZ_CTXT_R1)) == STATE_NZ_CTXT_R1)
                        {
                            if (bin.readBit() != 0)
                            {
                                sym = bin.readBit();
                                data[k] = (sym << 31) | setmask;
                                state[j + off_ul] |= STATE_NZ_CTXT_R2 | STATE_D_DR_R2;
                                state[j + off_ur] |= STATE_NZ_CTXT_R2 | STATE_D_DL_R2;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2 | STATE_V_U_SIGN_R2;
                                    state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2 | STATE_V_D_SIGN_R2;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_H_L_SIGN_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_H_R_SIGN_R1 | STATE_D_UR_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2;
                                    state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_D_UR_R2;
                                }
                            }
                            else
                            {
                                csj |= STATE_VISITED_R1;
                            }
                        }
                        if (sheight < 4)
                        {
                            state[j] = csj;
                            continue;
                        }
                        if ((csj & (STATE_SIG_R2 | STATE_NZ_CTXT_R2)) == STATE_NZ_CTXT_R2)
                        {
                            k += dscanw;
                            if (bin.readBit() != 0)
                            {
                                sym = bin.readBit();
                                data[k] = (sym << 31) | setmask;
                                state[j + off_dl] |= STATE_NZ_CTXT_R1 | STATE_D_UR_R1;
                                state[j + off_dr] |= STATE_NZ_CTXT_R1 | STATE_D_UL_R1;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1 | STATE_V_D_SIGN_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1 | STATE_V_U_SIGN_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2 | STATE_H_L_SIGN_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2 | STATE_H_R_SIGN_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2;
                                }
                            }
                            else
                            {
                                csj |= STATE_VISITED_R2;
                            }
                        }
                        state[j] = csj;
                    }
                }
            }
            error = false;
            if (isterm && (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_PRED_TERM) != 0)
            {
                error = bin.checkBytePadding();
            }
            return error;
        }
        private bool magRefPass(DataBlock cblk, MQDecoder mq, int bp, int[] state, bool isterm)
        {
            int j, sj;
            int k, sk;
            int dscanw;
            int sscanw;
            int jstep;
            int kstep;
            int stopsk;
            int csj;
            int setmask;
            int resetmask;
            int sym;
            int[] data;
            int s;
            int nstripes;
            int sheight;
            bool error;
            dscanw = cblk.scanw;
            sscanw = cblk.w + 2;
            jstep = sscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT / 2 - cblk.w;
            kstep = dscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - cblk.w;
            setmask = (1 << bp) >> 1;
            resetmask = (-1) << (bp + 1);
            data = (int[])cblk.Data;
            nstripes = (cblk.h + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - 1) / Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
            sk = cblk.offset;
            sj = sscanw + 1;
            for (s = nstripes - 1; s >= 0; s--, sk += kstep, sj += jstep)
            {
                sheight = (s != 0) ? Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT : cblk.h - (nstripes - 1) * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
                stopsk = sk + cblk.w;
                for (; sk < stopsk; sk++, sj++)
                {
                    j = sj;
                    csj = state[j];
                    if ((((SupportClass.URShift(csj, 1)) & (~csj)) & VSTD_MASK_R1R2) != 0)
                    {
                        k = sk;
                        if ((csj & (STATE_SIG_R1 | STATE_VISITED_R1)) == STATE_SIG_R1)
                        {
                            sym = mq.decodeSymbol(MR_LUT[csj & MR_MASK]);
                            data[k] &= resetmask;
                            data[k] |= (sym << bp) | setmask;
                            csj |= STATE_PREV_MR_R1;
                        }
                        if (sheight < 2)
                        {
                            state[j] = csj;
                            continue;
                        }
                        if ((csj & (STATE_SIG_R2 | STATE_VISITED_R2)) == STATE_SIG_R2)
                        {
                            k += dscanw;
                            sym = mq.decodeSymbol(MR_LUT[(SupportClass.URShift(csj, STATE_SEP)) & MR_MASK]);
                            data[k] &= resetmask;
                            data[k] |= (sym << bp) | setmask;
                            csj |= STATE_PREV_MR_R2;
                        }
                        state[j] = csj;
                    }
                    if (sheight < 3)
                        continue;
                    j += sscanw;
                    csj = state[j];
                    if ((((SupportClass.URShift(csj, 1)) & (~csj)) & VSTD_MASK_R1R2) != 0)
                    {
                        k = sk + (dscanw << 1);
                        if ((csj & (STATE_SIG_R1 | STATE_VISITED_R1)) == STATE_SIG_R1)
                        {
                            sym = mq.decodeSymbol(MR_LUT[csj & MR_MASK]);
                            data[k] &= resetmask;
                            data[k] |= (sym << bp) | setmask;
                            csj |= STATE_PREV_MR_R1;
                        }
                        if (sheight < 4)
                        {
                            state[j] = csj;
                            continue;
                        }
                        if ((state[j] & (STATE_SIG_R2 | STATE_VISITED_R2)) == STATE_SIG_R2)
                        {
                            k += dscanw;
                            sym = mq.decodeSymbol(MR_LUT[(SupportClass.URShift(csj, STATE_SEP)) & MR_MASK]);
                            data[k] &= resetmask;
                            data[k] |= (sym << bp) | setmask;
                            csj |= STATE_PREV_MR_R2;
                        }
                        state[j] = csj;
                    }
                }
            }
            error = false;
            if (isterm && (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_PRED_TERM) != 0)
            {
                error = mq.checkPredTerm();
            }
            if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_RESET_MQ) != 0)
            {
                mq.resetCtxts();
            }
            return error;
        }
        private bool rawMagRefPass(DataBlock cblk, ByteToBitInput bin, int bp, int[] state, bool isterm)
        {
            int j, sj;
            int k, sk;
            int dscanw;
            int sscanw;
            int jstep;
            int kstep;
            int stopsk;
            int csj;
            int setmask;
            int resetmask;
            int sym;
            int[] data;
            int s;
            int nstripes;
            int sheight;
            bool error;
            dscanw = cblk.scanw;
            sscanw = cblk.w + 2;
            jstep = sscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT / 2 - cblk.w;
            kstep = dscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - cblk.w;
            setmask = (1 << bp) >> 1;
            resetmask = (-1) << (bp + 1);
            data = (int[])cblk.Data;
            nstripes = (cblk.h + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - 1) / Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
            sk = cblk.offset;
            sj = sscanw + 1;
            for (s = nstripes - 1; s >= 0; s--, sk += kstep, sj += jstep)
            {
                sheight = (s != 0) ? Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT : cblk.h - (nstripes - 1) * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
                stopsk = sk + cblk.w;
                for (; sk < stopsk; sk++, sj++)
                {
                    j = sj;
                    csj = state[j];
                    if ((((SupportClass.URShift(csj, 1)) & (~csj)) & VSTD_MASK_R1R2) != 0)
                    {
                        k = sk;
                        if ((csj & (STATE_SIG_R1 | STATE_VISITED_R1)) == STATE_SIG_R1)
                        {
                            sym = bin.readBit();
                            data[k] &= resetmask;
                            data[k] |= (sym << bp) | setmask;
                        }
                        if (sheight < 2)
                            continue;
                        if ((csj & (STATE_SIG_R2 | STATE_VISITED_R2)) == STATE_SIG_R2)
                        {
                            k += dscanw;
                            sym = bin.readBit();
                            data[k] &= resetmask;
                            data[k] |= (sym << bp) | setmask;
                        }
                    }
                    if (sheight < 3)
                        continue;
                    j += sscanw;
                    csj = state[j];
                    if ((((SupportClass.URShift(csj, 1)) & (~csj)) & VSTD_MASK_R1R2) != 0)
                    {
                        k = sk + (dscanw << 1);
                        if ((csj & (STATE_SIG_R1 | STATE_VISITED_R1)) == STATE_SIG_R1)
                        {
                            sym = bin.readBit();
                            data[k] &= resetmask;
                            data[k] |= (sym << bp) | setmask;
                        }
                        if (sheight < 4)
                            continue;
                        if ((state[j] & (STATE_SIG_R2 | STATE_VISITED_R2)) == STATE_SIG_R2)
                        {
                            k += dscanw;
                            sym = bin.readBit();
                            data[k] &= resetmask;
                            data[k] |= (sym << bp) | setmask;
                        }
                    }
                }
            }
            error = false;
            if (isterm && (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_PRED_TERM) != 0)
            {
                error = bin.checkBytePadding();
            }
            return error;
        }
        private bool cleanuppass(DataBlock cblk, MQDecoder mq, int bp, int[] state, int[] zc_lut, bool isterm)
        {
            int j, sj;
            int k, sk;
            int dscanw;
            int sscanw;
            int jstep;
            int kstep;
            int stopsk;
            int csj;
            int setmask;
            int sym;
            int rlclen;
            int ctxt;
            int[] data;
            int s;
            bool causal;
            int nstripes;
            int sheight;
            int off_ul, off_ur, off_dr, off_dl;
            bool error;
            dscanw = cblk.scanw;
            sscanw = cblk.w + 2;
            jstep = sscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT / 2 - cblk.w;
            kstep = dscanw * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - cblk.w;
            setmask = (3 << bp) >> 1;
            data = (int[])cblk.Data;
            nstripes = (cblk.h + Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT - 1) / Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
            causal = (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_VERT_STR_CAUSAL) != 0;
            off_ul = -sscanw - 1;
            off_ur = -sscanw + 1;
            off_dr = sscanw + 1;
            off_dl = sscanw - 1;
            sk = cblk.offset;
            sj = sscanw + 1;
            for (s = nstripes - 1; s >= 0; s--, sk += kstep, sj += jstep)
            {
                sheight = (s != 0) ? Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT : cblk.h - (nstripes - 1) * Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT;
                stopsk = sk + cblk.w;
                for (; sk < stopsk; sk++, sj++)
                {
                    j = sj;
                    csj = state[j];
                    {
                        if (csj == 0 && state[j + sscanw] == 0 && sheight == Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.STRIPE_HEIGHT)
                        {
                            if (mq.decodeSymbol(RLC_CTXT) != 0)
                            {
                                rlclen = mq.decodeSymbol(UNIF_CTXT) << 1;
                                rlclen |= mq.decodeSymbol(UNIF_CTXT);
                                k = sk + rlclen * dscanw;
                                if (rlclen > 1)
                                {
                                    j += sscanw;
                                    csj = state[j];
                                }
                            }
                            else
                            {
                                continue;
                            }
                            if ((rlclen & 0x01) == 0)
                            {
                                ctxt = SC_LUT[(csj >> SC_SHIFT_R1) & SC_MASK];
                                sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                data[k] = (sym << 31) | setmask;
                                if (rlclen != 0 || !causal)
                                {
                                    state[j + off_ul] |= STATE_NZ_CTXT_R2 | STATE_D_DR_R2;
                                    state[j + off_ur] |= STATE_NZ_CTXT_R2 | STATE_D_DL_R2;
                                }
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2 | STATE_V_U_SIGN_R2;
                                    if (rlclen != 0 || !causal)
                                    {
                                        state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2 | STATE_V_D_SIGN_R2;
                                    }
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_H_L_SIGN_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_H_R_SIGN_R1 | STATE_D_UR_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2;
                                    if (rlclen != 0 || !causal)
                                    {
                                        state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2;
                                    }
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_D_UR_R2;
                                }
                                if ((rlclen >> 1) != 0)
                                {
                                    goto top_half_brk;
                                }
                            }
                            else
                            {
                                ctxt = SC_LUT[(csj >> SC_SHIFT_R2) & SC_MASK];
                                sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                data[k] = (sym << 31) | setmask;
                                state[j + off_dl] |= STATE_NZ_CTXT_R1 | STATE_D_UR_R1;
                                state[j + off_dr] |= STATE_NZ_CTXT_R1 | STATE_D_UL_R1;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1 | STATE_V_D_SIGN_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1 | STATE_V_U_SIGN_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2 | STATE_H_L_SIGN_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2 | STATE_H_R_SIGN_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2;
                                }
                                state[j] = csj;
                                if ((rlclen >> 1) != 0)
                                {
                                    continue;
                                }
                                j += sscanw;
                                csj = state[j];
                                goto top_half_brk;
                            }
                        }
                        if ((((csj >> 1) | csj) & VSTD_MASK_R1R2) != VSTD_MASK_R1R2)
                        {
                            k = sk;
                            if ((csj & (STATE_SIG_R1 | STATE_VISITED_R1)) == 0)
                            {
                                if (mq.decodeSymbol(zc_lut[csj & ZC_MASK]) != 0)
                                {
                                    ctxt = SC_LUT[(SupportClass.URShift(csj, SC_SHIFT_R1)) & SC_MASK];
                                    sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                    data[k] = (sym << 31) | setmask;
                                    if (!causal)
                                    {
                                        state[j + off_ul] |= STATE_NZ_CTXT_R2 | STATE_D_DR_R2;
                                        state[j + off_ur] |= STATE_NZ_CTXT_R2 | STATE_D_DL_R2;
                                    }
                                    if (sym != 0)
                                    {
                                        csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2 | STATE_V_U_SIGN_R2;
                                        if (!causal)
                                        {
                                            state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2 | STATE_V_D_SIGN_R2;
                                        }
                                        state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_H_L_SIGN_R1 | STATE_D_UL_R2;
                                        state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_H_R_SIGN_R1 | STATE_D_UR_R2;
                                    }
                                    else
                                    {
                                        csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2;
                                        if (!causal)
                                        {
                                            state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2;
                                        }
                                        state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_D_UL_R2;
                                        state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_D_UR_R2;
                                    }
                                }
                            }
                            if (sheight < 2)
                            {
                                csj &= ~(STATE_VISITED_R1 | STATE_VISITED_R2);
                                state[j] = csj;
                                continue;
                            }
                            if ((csj & (STATE_SIG_R2 | STATE_VISITED_R2)) == 0)
                            {
                                k += dscanw;
                                if (mq.decodeSymbol(zc_lut[(SupportClass.URShift(csj, STATE_SEP)) & ZC_MASK]) != 0)
                                {
                                    ctxt = SC_LUT[(SupportClass.URShift(csj, SC_SHIFT_R2)) & SC_MASK];
                                    sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                    data[k] = (sym << 31) | setmask;
                                    state[j + off_dl] |= STATE_NZ_CTXT_R1 | STATE_D_UR_R1;
                                    state[j + off_dr] |= STATE_NZ_CTXT_R1 | STATE_D_UL_R1;
                                    if (sym != 0)
                                    {
                                        csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1 | STATE_V_D_SIGN_R1;
                                        state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1 | STATE_V_U_SIGN_R1;
                                        state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2 | STATE_H_L_SIGN_R2;
                                        state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2 | STATE_H_R_SIGN_R2;
                                    }
                                    else
                                    {
                                        csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1;
                                        state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1;
                                        state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2;
                                        state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2;
                                    }
                                }
                            }
                        }
                        csj &= ~(STATE_VISITED_R1 | STATE_VISITED_R2);
                        state[j] = csj;
                        if (sheight < 3)
                            continue;
                        j += sscanw;
                        csj = state[j];
                    }
                top_half_brk: ;
                    if ((((csj >> 1) | csj) & VSTD_MASK_R1R2) != VSTD_MASK_R1R2)
                    {
                        k = sk + (dscanw << 1);
                        if ((csj & (STATE_SIG_R1 | STATE_VISITED_R1)) == 0)
                        {
                            if (mq.decodeSymbol(zc_lut[csj & ZC_MASK]) != 0)
                            {
                                ctxt = SC_LUT[(csj >> SC_SHIFT_R1) & SC_MASK];
                                sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                data[k] = (sym << 31) | setmask;
                                state[j + off_ul] |= STATE_NZ_CTXT_R2 | STATE_D_DR_R2;
                                state[j + off_ur] |= STATE_NZ_CTXT_R2 | STATE_D_DL_R2;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2 | STATE_V_U_SIGN_R2;
                                    state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2 | STATE_V_D_SIGN_R2;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_H_L_SIGN_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_H_R_SIGN_R1 | STATE_D_UR_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R1 | STATE_VISITED_R1 | STATE_NZ_CTXT_R2 | STATE_V_U_R2;
                                    state[j - sscanw] |= STATE_NZ_CTXT_R2 | STATE_V_D_R2;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_L_R1 | STATE_D_UL_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_H_R_R1 | STATE_D_UR_R2;
                                }
                            }
                        }
                        if (sheight < 4)
                        {
                            csj &= ~(STATE_VISITED_R1 | STATE_VISITED_R2);
                            state[j] = csj;
                            continue;
                        }
                        if ((csj & (STATE_SIG_R2 | STATE_VISITED_R2)) == 0)
                        {
                            k += dscanw;
                            if (mq.decodeSymbol(zc_lut[(SupportClass.URShift(csj, STATE_SEP)) & ZC_MASK]) != 0)
                            {
                                ctxt = SC_LUT[(SupportClass.URShift(csj, SC_SHIFT_R2)) & SC_MASK];
                                sym = mq.decodeSymbol(ctxt & SC_LUT_MASK) ^ (SupportClass.URShift(ctxt, SC_SPRED_SHIFT));
                                data[k] = (sym << 31) | setmask;
                                state[j + off_dl] |= STATE_NZ_CTXT_R1 | STATE_D_UR_R1;
                                state[j + off_dr] |= STATE_NZ_CTXT_R1 | STATE_D_UL_R1;
                                if (sym != 0)
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1 | STATE_V_D_SIGN_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1 | STATE_V_U_SIGN_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2 | STATE_H_L_SIGN_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2 | STATE_H_R_SIGN_R2;
                                }
                                else
                                {
                                    csj |= STATE_SIG_R2 | STATE_VISITED_R2 | STATE_NZ_CTXT_R1 | STATE_V_D_R1;
                                    state[j + sscanw] |= STATE_NZ_CTXT_R1 | STATE_V_U_R1;
                                    state[j + 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DL_R1 | STATE_H_L_R2;
                                    state[j - 1] |= STATE_NZ_CTXT_R1 | STATE_NZ_CTXT_R2 | STATE_D_DR_R1 | STATE_H_R_R2;
                                }
                            }
                        }
                    }
                    csj &= ~(STATE_VISITED_R1 | STATE_VISITED_R2);
                    state[j] = csj;
                }
            }
            if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_SEG_SYMBOLS) != 0)
            {
                sym = mq.decodeSymbol(UNIF_CTXT) << 3;
                sym |= mq.decodeSymbol(UNIF_CTXT) << 2;
                sym |= mq.decodeSymbol(UNIF_CTXT) << 1;
                sym |= mq.decodeSymbol(UNIF_CTXT);
                error = sym != SEG_SYMBOL;
            }
            else
            {
                error = false;
            }
            if (isterm && (options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_PRED_TERM) != 0)
            {
                error = mq.checkPredTerm();
            }
            if ((options & Syncfusion.Pdf.JPEG2000.entropy.StdEntropyCoderOptions.OPT_RESET_MQ) != 0)
            {
                mq.resetCtxts();
            }
            return error;
        }
        private void conceal(DataBlock cblk, int bp)
        {
            int l;
            int k;
            int kmax;
            int dk;
            int[] data;
            int setmask;
            int resetmask;
            setmask = 1 << bp;
            resetmask = (-1) << (bp);
            data = (int[])cblk.Data;
            for (l = cblk.h - 1, k = cblk.offset; l >= 0; l--)
            {
                for (kmax = k + cblk.w; k < kmax; k++)
                {
                    dk = data[k];
                    if ((dk & resetmask & 0x7FFFFFFF) != 0)
                    {
                        data[k] = (dk & resetmask) | setmask;
                    }
                    else
                    {
                        data[k] = 0;
                    }
                }
                k += cblk.scanw - cblk.w;
            }
        }
        static StdEntropyDecoder()
        {
            {
                int i, j;
                int[] inter_sc_lut;
                int ds, us, rs, ls;
                int dsgn, usgn, rsgn, lsgn;
                int h, v;
                ZC_LUT_LH[0] = 2;
                for (i = 1; i < 16; i++)
                {
                    ZC_LUT_LH[i] = 4;
                }
                for (i = 0; i < 4; i++)
                {
                    ZC_LUT_LH[1 << i] = 3;
                }
                for (i = 0; i < 16; i++)
                {
                    ZC_LUT_LH[STATE_V_U_R1 | i] = 5;
                    ZC_LUT_LH[STATE_V_D_R1 | i] = 5;
                    ZC_LUT_LH[STATE_V_U_R1 | STATE_V_D_R1 | i] = 6;
                }
                ZC_LUT_LH[STATE_H_L_R1] = 7;
                ZC_LUT_LH[STATE_H_R_R1] = 7;
                for (i = 1; i < 16; i++)
                {
                    ZC_LUT_LH[STATE_H_L_R1 | i] = 8;
                    ZC_LUT_LH[STATE_H_R_R1 | i] = 8;
                }
                for (i = 1; i < 4; i++)
                {
                    for (j = 0; j < 16; j++)
                    {
                        ZC_LUT_LH[STATE_H_L_R1 | (i << 4) | j] = 9;
                        ZC_LUT_LH[STATE_H_R_R1 | (i << 4) | j] = 9;
                    }
                }
                for (i = 0; i < 64; i++)
                {
                    ZC_LUT_LH[STATE_H_L_R1 | STATE_H_R_R1 | i] = 10;
                }
                ZC_LUT_HL[0] = 2;
                for (i = 1; i < 16; i++)
                {
                    ZC_LUT_HL[i] = 4;
                }
                for (i = 0; i < 4; i++)
                {
                    ZC_LUT_HL[1 << i] = 3;
                }
                for (i = 0; i < 16; i++)
                {
                    ZC_LUT_HL[STATE_H_L_R1 | i] = 5;
                    ZC_LUT_HL[STATE_H_R_R1 | i] = 5;
                    ZC_LUT_HL[STATE_H_L_R1 | STATE_H_R_R1 | i] = 6;
                }
                ZC_LUT_HL[STATE_V_U_R1] = 7;
                ZC_LUT_HL[STATE_V_D_R1] = 7;
                for (i = 1; i < 16; i++)
                {
                    ZC_LUT_HL[STATE_V_U_R1 | i] = 8;
                    ZC_LUT_HL[STATE_V_D_R1 | i] = 8;
                }
                for (i = 1; i < 4; i++)
                {
                    for (j = 0; j < 16; j++)
                    {
                        ZC_LUT_HL[(i << 6) | STATE_V_U_R1 | j] = 9;
                        ZC_LUT_HL[(i << 6) | STATE_V_D_R1 | j] = 9;
                    }
                }
                for (i = 0; i < 4; i++)
                {
                    for (j = 0; j < 16; j++)
                    {
                        ZC_LUT_HL[(i << 6) | STATE_V_U_R1 | STATE_V_D_R1 | j] = 10;
                    }
                }
                int[] twoBits = new int[] { 3, 5, 6, 9, 10, 12 };
                int[] oneBit = new int[] { 1, 2, 4, 8 };
                int[] twoLeast = new int[] { 3, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15 }; // Figures
                int[] threeLeast = new int[] { 7, 11, 13, 14, 15 }; // Figures
                ZC_LUT_HH[0] = 2;
                for (i = 0; i < oneBit.Length; i++)
                    ZC_LUT_HH[oneBit[i] << 4] = 3;
                for (i = 0; i < twoLeast.Length; i++)
                    ZC_LUT_HH[twoLeast[i] << 4] = 4;
                for (i = 0; i < oneBit.Length; i++)
                    ZC_LUT_HH[oneBit[i]] = 5;
                for (i = 0; i < oneBit.Length; i++)
                    for (j = 0; j < oneBit.Length; j++)
                        ZC_LUT_HH[(oneBit[i] << 4) | oneBit[j]] = 6;
                for (i = 0; i < twoLeast.Length; i++)
                    for (j = 0; j < oneBit.Length; j++)
                        ZC_LUT_HH[(twoLeast[i] << 4) | oneBit[j]] = 7;
                for (i = 0; i < twoBits.Length; i++)
                    ZC_LUT_HH[twoBits[i]] = 8;
                for (j = 0; j < twoBits.Length; j++)
                    for (i = 1; i < 16; i++)
                        ZC_LUT_HH[(i << 4) | twoBits[j]] = 9;
                for (i = 0; i < 16; i++)
                    for (j = 0; j < threeLeast.Length; j++)
                        ZC_LUT_HH[(i << 4) | threeLeast[j]] = 10;
                inter_sc_lut = new int[36];
                inter_sc_lut[(2 << 3) | 2] = 15;
                inter_sc_lut[(2 << 3) | 1] = 14;
                inter_sc_lut[(2 << 3) | 0] = 13;
                inter_sc_lut[(1 << 3) | 2] = 12;
                inter_sc_lut[(1 << 3) | 1] = 11;
                inter_sc_lut[(1 << 3) | 0] = 12 | INT_SIGN_BIT;
                inter_sc_lut[(0 << 3) | 2] = 13 | INT_SIGN_BIT;
                inter_sc_lut[(0 << 3) | 1] = 14 | INT_SIGN_BIT;
                inter_sc_lut[(0 << 3) | 0] = 15 | INT_SIGN_BIT;
                for (i = 0; i < (1 << SC_LUT_BITS) - 1; i++)
                {
                    ds = i & 0x01;
                    us = (i >> 1) & 0x01;
                    rs = (i >> 2) & 0x01;
                    ls = (i >> 3) & 0x01;
                    dsgn = (i >> 5) & 0x01;
                    usgn = (i >> 6) & 0x01;
                    rsgn = (i >> 7) & 0x01;
                    lsgn = (i >> 8) & 0x01;
                    h = ls * (1 - 2 * lsgn) + rs * (1 - 2 * rsgn);
                    h = (h >= -1) ? h : -1;
                    h = (h <= 1) ? h : 1;
                    v = us * (1 - 2 * usgn) + ds * (1 - 2 * dsgn);
                    v = (v >= -1) ? v : -1;
                    v = (v <= 1) ? v : 1;
                    SC_LUT[i] = inter_sc_lut[(h + 1) << 3 | (v + 1)];
                }
                inter_sc_lut = null;
                MR_LUT[0] = 16;
                for (i = 1; i < (1 << (MR_LUT_BITS - 1)); i++)
                {
                    MR_LUT[i] = 17;
                }
                for (; i < (1 << MR_LUT_BITS); i++)
                {
                    MR_LUT[i] = 18;
                }
            }
        }
    }
}