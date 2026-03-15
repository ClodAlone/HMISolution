#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.util;
namespace Syncfusion.Pdf.JPEG2000.codestream.reader
{
    internal class TagTreeDecoder
    {
        virtual public int Width
        {
            get
            {
                return w;
            }
        }
        virtual public int Height
        {
            get
            {
                return h;
            }
        }
        internal int w;
        internal int h;
        internal int lvls;
        internal int[][] treeV;
        internal int[][] treeS;
        public TagTreeDecoder(int h, int w)
        {
            int i;
            if (w < 0 || h < 0)
            {
                throw new System.ArgumentException();
            }
            this.w = w;
            this.h = h;
            if (w == 0 || h == 0)
            {
                lvls = 0;
            }
            else
            {
                lvls = 1;
                while (h != 1 || w != 1)
                {
                    w = (w + 1) >> 1;
                    h = (h + 1) >> 1;
                    lvls++;
                }
            }
            treeV = new int[lvls][];
            treeS = new int[lvls][];
            w = this.w;
            h = this.h;
            for (i = 0; i < lvls; i++)
            {
                treeV[i] = new int[h * w];
                ArrayUtil.intArraySet(treeV[i], System.Int32.MaxValue);
                treeS[i] = new int[h * w];
                w = (w + 1) >> 1;
                h = (h + 1) >> 1;
            }
        }
        public virtual int update(int m, int n, int t, PktHeaderBitReader in_Renamed)
        {
            int k, tmin;
            int idx, ts, tv;
            if (m >= h || n >= w || t < 0)
            {
                throw new System.ArgumentException();
            }
            k = lvls - 1;
            tmin = treeS[k][0];
            idx = (m >> k) * ((w + (1 << k) - 1) >> k) + (n >> k);
            while (true)
            {
                ts = treeS[k][idx];
                tv = treeV[k][idx];
                if (ts < tmin)
                {
                    ts = tmin;
                }
                while (t > ts)
                {
                    if (tv >= ts)
                    {
                        if (in_Renamed.readBit() == 0)
                        {
                            ts++;
                        }
                        else
                        {
                            tv = ts++;
                        }
                    }
                    else
                    {
                        ts = t;
                        break;
                    }
                }
                treeS[k][idx] = ts;
                treeV[k][idx] = tv;
                if (k > 0)
                {
                    tmin = ts < tv ? ts : tv;
                    k--;
                    idx = (m >> k) * ((w + (1 << k) - 1) >> k) + (n >> k);
                }
                else
                {
                    return tv;
                }
            }
        }
        public virtual int getValue(int m, int n)
        {
            if (m >= h || n >= w)
            {
                throw new System.ArgumentException();
            }
            return treeV[0][m * w + n];
        }
    }
}