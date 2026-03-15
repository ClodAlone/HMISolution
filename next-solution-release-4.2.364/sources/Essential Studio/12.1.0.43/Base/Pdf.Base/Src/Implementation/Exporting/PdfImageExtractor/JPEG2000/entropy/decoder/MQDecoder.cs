#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.entropy.decoder;
using Syncfusion.Pdf.JPEG2000.entropy;
using Syncfusion.Pdf.JPEG2000.io;
using Syncfusion.Pdf.JPEG2000.util;
namespace Syncfusion.Pdf.JPEG2000.entropy.decoder
{
    internal class MQDecoder
    {
        virtual public int NumCtxts
        {
            get
            {
                return I.Length;
            }
        }
        virtual public ByteInputBuffer ByteInputBuffer
        {
            get
            {
                return in_Renamed;
            }
        }
        internal static readonly uint[] qe = new uint[] { 0x5601, 0x3401, 0x1801, 0x0ac1, 0x0521, 0x0221, 0x5601, 0x5401, 0x4801, 0x3801, 0x3001, 0x2401, 0x1c01, 0x1601, 0x5601, 0x5401, 0x5101, 0x4801, 0x3801, 0x3401, 0x3001, 0x2801, 0x2401, 0x2201, 0x1c01, 0x1801, 0x1601, 0x1401, 0x1201, 0x1101, 0x0ac1, 0x09c1, 0x08a1, 0x0521, 0x0441, 0x02a1, 0x0221, 0x0141, 0x0111, 0x0085, 0x0049, 0x0025, 0x0015, 0x0009, 0x0005, 0x0001, 0x5601 };
        internal static readonly int[] nMPS = new int[] { 1, 2, 3, 4, 5, 38, 7, 8, 9, 10, 11, 12, 13, 29, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 45, 46 };
        internal static readonly int[] nLPS = new int[] { 1, 6, 9, 12, 29, 33, 6, 14, 14, 14, 17, 18, 20, 21, 14, 14, 15, 16, 17, 18, 19, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 46 };
        internal static readonly int[] switchLM = new int[] { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        internal ByteInputBuffer in_Renamed;
        internal int[] mPS;
        internal int[] I;
        internal uint c;
        internal uint cT;
        internal uint a;
        internal uint b;
        internal bool markerFound;
        internal int[] initStates;
        public MQDecoder(ByteInputBuffer iStream, int nrOfContexts, int[] initStates)
        {
            in_Renamed = iStream;
            I = new int[nrOfContexts];
            mPS = new int[nrOfContexts];
            this.initStates = initStates;
            init();
            resetCtxts();
        }
        internal bool fastDecodeSymbols(int[] bits, int ctxt, uint n)
        {
            uint q;
            int idx;
            uint la;
            int i;
            idx = I[ctxt];
            q = qe[idx];
            if ((q < 0x4000) && (n <= (a - (c >> 16) - 1) / q) && (n <= (a - 0x8000) / q + 1))
            {
                a -= n * q;
                if (a >= 0x8000)
                {
                    bits[0] = mPS[ctxt];
                    return true;
                }
                else
                {
                    I[ctxt] = nMPS[idx];
                    if (cT == 0)
                        byteIn();
                    a <<= 1;
                    c <<= 1;
                    cT--;
                    bits[0] = mPS[ctxt];
                    return true;
                }
            }
            else
            {
                la = a;
                for (i = 0; i < n; i++)
                {
                    la -= q;
                    if ((c >> 16) < la)
                    {
                        if (la >= 0x8000)
                        {
                            bits[i] = mPS[ctxt];
                        }
                        else
                        {
                            if (la >= q)
                            {
                                bits[i] = mPS[ctxt];
                                idx = nMPS[idx];
                                q = qe[idx];
                                if (cT == 0)
                                    byteIn();
                                la <<= 1;
                                c <<= 1;
                                cT--;
                            }
                            else
                            {
                                bits[i] = 1 - mPS[ctxt];
                                if (switchLM[idx] == 1)
                                    mPS[ctxt] = 1 - mPS[ctxt];
                                idx = nLPS[idx];
                                q = qe[idx];
                                do
                                {
                                    if (cT == 0)
                                        byteIn();
                                    la <<= 1;
                                    c <<= 1;
                                    cT--;
                                }
                                while (la < 0x8000);
                            }
                        }
                    }
                    else
                    {
                        c -= (la << 16);
                        if (la < q)
                        {
                            la = q;
                            bits[i] = mPS[ctxt];
                            idx = nMPS[idx];
                            q = qe[idx];
                            if (cT == 0)
                                byteIn();
                            la <<= 1;
                            c <<= 1;
                            cT--;
                        }
                        else
                        {
                            la = q;
                            bits[i] = 1 - mPS[ctxt];
                            if (switchLM[idx] == 1)
                                mPS[ctxt] = 1 - mPS[ctxt];
                            idx = nLPS[idx];
                            q = qe[idx];
                            do
                            {
                                if (cT == 0)
                                    byteIn();
                                la <<= 1;
                                c <<= 1;
                                cT--;
                            }
                            while (la < 0x8000);
                        }
                    }
                }
                a = la;
                I[ctxt] = idx;
                return false;
            }
        }
        public void decodeSymbols(int[] bits, int[] cX, int n)
        {
            uint q;
            int ctxt;
            uint la;
            int index;
            int i;
            for (i = 0; i < n; i++)
            {
                ctxt = cX[i];
                index = I[ctxt];
                q = qe[index];
                a -= q;
                if ((c >> 16) < a)
                {
                    if (a >= 0x8000)
                    {
                        bits[i] = mPS[ctxt];
                    }
                    else
                    {
                        la = a;
                        if (la >= q)
                        {
                            bits[i] = mPS[ctxt];
                            I[ctxt] = nMPS[index];
                            if (cT == 0)
                                byteIn();
                            la <<= 1;
                            c <<= 1;
                            cT--;
                        }
                        else
                        {
                            bits[i] = 1 - mPS[ctxt];
                            if (switchLM[index] == 1)
                                mPS[ctxt] = 1 - mPS[ctxt];
                            I[ctxt] = nLPS[index];
                            do
                            {
                                if (cT == 0)
                                    byteIn();
                                la <<= 1;
                                c <<= 1;
                                cT--;
                            }
                            while (la < 0x8000);
                        }
                        a = la;
                    }
                }
                else
                {
                    la = a;
                    c -= (la << 16);
                    if (la < q)
                    {
                        la = q;
                        bits[i] = mPS[ctxt];
                        I[ctxt] = nMPS[index];
                        if (cT == 0)
                            byteIn();
                        la <<= 1;
                        c <<= 1;
                        cT--;
                    }
                    else
                    {
                        la = q;
                        bits[i] = 1 - mPS[ctxt];
                        if (switchLM[index] == 1)
                            mPS[ctxt] = 1 - mPS[ctxt];
                        I[ctxt] = nLPS[index];
                        do
                        {
                            if (cT == 0)
                                byteIn();
                            la <<= 1;
                            c <<= 1;
                            cT--;
                        }
                        while (la < 0x8000);
                    }
                    a = la;
                }
            }
        }
        public int decodeSymbol(int context)
        {
            uint q;
            uint la;
            int index;
            int decision;
            index = I[context];
            q = qe[index];
            a -= q;
            if ((c >> 16) < a)
            {
                if (a >= 0x8000)
                {
                    decision = mPS[context];
                }
                else
                {
                    la = a;
                    if (la >= q)
                    {
                        decision = mPS[context];
                        I[context] = nMPS[index];
                        if (cT == 0)
                            byteIn();
                        la <<= 1;
                        c <<= 1;
                        cT--;
                    }
                    else
                    {
                        decision = 1 - mPS[context];
                        if (switchLM[index] == 1)
                            mPS[context] = 1 - mPS[context];
                        I[context] = nLPS[index];
                        do
                        {
                            if (cT == 0)
                                byteIn();
                            la <<= 1;
                            c <<= 1;
                            cT--;
                        }
                        while (la < 0x8000);
                    }
                    a = la;
                }
            }
            else
            {
                la = a;
                c -= (la << 16);
                if (la < q)
                {
                    la = q;
                    decision = mPS[context];
                    I[context] = nMPS[index];
                    if (cT == 0)
                        byteIn();
                    la <<= 1;
                    c <<= 1;
                    cT--;
                }
                else
                {
                    la = q;
                    decision = 1 - mPS[context];
                    if (switchLM[index] == 1)
                        mPS[context] = 1 - mPS[context];
                    I[context] = nLPS[index];
                    do
                    {
                        if (cT == 0)
                            byteIn();
                        la <<= 1;
                        c <<= 1;
                        cT--;
                    }
                    while (la < 0x8000);
                }
                a = la;
            }
            return decision;
        }
        public virtual bool checkPredTerm()
        {
            int k;
            uint q;
            if (b != 0xFF && !markerFound)
                return true;
            if (cT != 0 && !markerFound)
                return true;
            if (cT == 1)
                return false;
            if (cT == 0)
            {
                if (!markerFound)
                {
                    b = (uint)in_Renamed.read() & 0xFF;
                    if (b <= 0x8F)
                        return true;
                }
                cT = 8;
            }
            k = (int)(cT - 1);
            q = ((uint)0x8000) >> k;
            a -= q;
            if ((c >> 16) < a)
            {
                return true;
            }
            c -= (a << 16);
            a = q;
            do
            {
                if (cT == 0)
                    byteIn();
                a <<= 1;
                c <<= 1;
                cT--;
            }
            while (a < 0x8000);
            return false;
        }
        private void byteIn()
        {
            if (!markerFound)
            {
                if (b == 0xFF)
                {
                    b = ((uint)in_Renamed.read()) & 0xFF;
                    if (b > 0x8F)
                    {
                        markerFound = true;
                        cT = 8;
                    }
                    else
                    {
                        c += 0xFE00 - (b << 9);
                        cT = 7;
                    }
                }
                else
                {
                    b = ((uint)in_Renamed.read()) & 0xFF;
                    c += 0xFF00 - (b << 8);
                    cT = 8;
                }
            }
            else
            {
                cT = 8;
            }
        }
        public void resetCtxt(int c)
        {
            I[c] = initStates[c];
            mPS[c] = 0;
        }
        public void resetCtxts()
        {
            Array.Copy(initStates, 0, I, 0, I.Length);
            ArrayUtil.intArraySet(mPS, 0);
        }
        public void nextSegment(byte[] buf, int off, int len)
        {
            in_Renamed.setByteArray(buf, off, len);
            init();
        }
        private void init()
        {
            markerFound = false;
            b = ((uint)in_Renamed.read()) & 0xFF;
            c = (b ^ 0xFF) << 16;
            byteIn();
            c = c << 7;
            cT = cT - 7;
            a = 0x8000;
        }
    }
}