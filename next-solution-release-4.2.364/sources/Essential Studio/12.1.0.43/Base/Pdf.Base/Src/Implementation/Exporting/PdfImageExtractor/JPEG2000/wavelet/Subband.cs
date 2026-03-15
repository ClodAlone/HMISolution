#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet
{
    public abstract class Subband
    {
        public abstract Subband Parent { get; }
        public abstract Subband LL { get; }
        public abstract Subband HL { get; }
        public abstract Subband LH { get; }
        public abstract Subband HH { get; }
        virtual public Subband NextResLevel
        {
            get
            {
                Subband sb;
                if (level == 0)
                {
                    return null;
                }
                sb = this;
                do
                {
                    sb = sb.Parent;
                    if (sb == null)
                    {
                        return null;
                    }
                }
                while (sb.resLvl == resLvl);
                sb = sb.HL;
                while (sb.isNode)
                {
                    sb = sb.LL;
                }
                return sb;
            }
        }
        internal abstract WaveletFilter HorWFilter { get; }
        internal abstract WaveletFilter VerWFilter { get; }
        public const int WT_ORIENT_LL = 0;
        public const int WT_ORIENT_HL = 1;
        public const int WT_ORIENT_LH = 2;
        public const int WT_ORIENT_HH = 3;
        public bool isNode;
        public int orientation;
        public int level;
        public int resLvl;
        internal JPXImageCoordinates numCb = null;
        public int anGainExp;
        public int sbandIdx = 0;
        public int ulcx;
        public int ulcy;
        public int ulx;
        public int uly;
        public int w;
        public int h;
        public int nomCBlkW;
        public int nomCBlkH;
        internal abstract Subband split(WaveletFilter hfilter, WaveletFilter vfilter);
        internal virtual void initChilds()
        {
            Subband subb_LL = LL;
            Subband subb_HL = HL;
            Subband subb_LH = LH;
            Subband subb_HH = HH;
            subb_LL.level = level + 1;
            subb_LL.ulcx = (ulcx + 1) >> 1;
            subb_LL.ulcy = (ulcy + 1) >> 1;
            subb_LL.ulx = ulx;
            subb_LL.uly = uly;
            subb_LL.w = ((ulcx + w + 1) >> 1) - subb_LL.ulcx;
            subb_LL.h = ((ulcy + h + 1) >> 1) - subb_LL.ulcy;
            subb_LL.resLvl = (orientation == WT_ORIENT_LL) ? resLvl - 1 : resLvl;
            subb_LL.anGainExp = anGainExp;
            subb_LL.sbandIdx = (sbandIdx << 2);
            subb_HL.orientation = WT_ORIENT_HL;
            subb_HL.level = subb_LL.level;
            subb_HL.ulcx = ulcx >> 1;
            subb_HL.ulcy = subb_LL.ulcy;
            subb_HL.ulx = ulx + subb_LL.w;
            subb_HL.uly = uly;
            subb_HL.w = ((ulcx + w) >> 1) - subb_HL.ulcx;
            subb_HL.h = subb_LL.h;
            subb_HL.resLvl = resLvl;
            subb_HL.anGainExp = anGainExp + 1;
            subb_HL.sbandIdx = (sbandIdx << 2) + 1;
            subb_LH.orientation = WT_ORIENT_LH;
            subb_LH.level = subb_LL.level;
            subb_LH.ulcx = subb_LL.ulcx;
            subb_LH.ulcy = ulcy >> 1;
            subb_LH.ulx = ulx;
            subb_LH.uly = uly + subb_LL.h;
            subb_LH.w = subb_LL.w;
            subb_LH.h = ((ulcy + h) >> 1) - subb_LH.ulcy;
            subb_LH.resLvl = resLvl;
            subb_LH.anGainExp = anGainExp + 1;
            subb_LH.sbandIdx = (sbandIdx << 2) + 2;
            subb_HH.orientation = WT_ORIENT_HH;
            subb_HH.level = subb_LL.level;
            subb_HH.ulcx = subb_HL.ulcx;
            subb_HH.ulcy = subb_LH.ulcy;
            subb_HH.ulx = subb_HL.ulx;
            subb_HH.uly = subb_LH.uly;
            subb_HH.w = subb_HL.w;
            subb_HH.h = subb_LH.h;
            subb_HH.resLvl = resLvl;
            subb_HH.anGainExp = anGainExp + 2;
            subb_HH.sbandIdx = (sbandIdx << 2) + 3;
        }
        public Subband()
        {
        }
        internal Subband(int w, int h, int ulcx, int ulcy, int lvls, WaveletFilter[] hfilters, WaveletFilter[] vfilters)
        {
            int i, hi, vi;
            Subband cur;
            this.w = w;
            this.h = h;
            this.ulcx = ulcx;
            this.ulcy = ulcy;
            this.resLvl = lvls;
            cur = this;
            for (i = 0; i < lvls; i++)
            {
                hi = (cur.resLvl <= hfilters.Length) ? cur.resLvl - 1 : hfilters.Length - 1;
                vi = (cur.resLvl <= vfilters.Length) ? cur.resLvl - 1 : vfilters.Length - 1;
                cur = cur.split(hfilters[hi], vfilters[vi]);
            }
        }
        public virtual Subband nextSubband()
        {
            Subband sb;
            if (isNode)
            {
                throw new System.ArgumentException();
            }
            switch (orientation)
            {
                case WT_ORIENT_LL:
                    sb = Parent;
                    if (sb == null || sb.resLvl != resLvl)
                    {
                        return null;
                    }
                    else
                    {
                        return sb.HL;
                    }
                case WT_ORIENT_HL:
                    return Parent.LH;
                case WT_ORIENT_LH:
                    return Parent.HH;
                case WT_ORIENT_HH:
                    sb = this;
                    while (sb.orientation == WT_ORIENT_HH)
                    {
                        sb = sb.Parent;
                    }
                    switch (sb.orientation)
                    {
                        case WT_ORIENT_LL:
                            sb = sb.Parent;
                            if (sb == null || sb.resLvl != resLvl)
                            {
                                return null;
                            }
                            else
                            {
                                sb = sb.HL;
                            }
                            break;
                        case WT_ORIENT_HL:
                            sb = sb.Parent.LH;
                            break;
                        case WT_ORIENT_LH:
                            sb = sb.Parent.HH;
                            break;
                        default:
                            throw new System.ArgumentException();
                    }
                    while (sb.isNode)
                    {
                        sb = sb.LL;
                    }

                    return sb;
                default:
                    throw new System.ArgumentException();
                    
            }
        }
        public virtual Subband getSubbandByIdx(int rl, int sbi)
        {
            Subband sb = this;
            if (rl > sb.resLvl || rl < 0)
            {
                throw new System.ArgumentException("Resolution level index " + "out of range");
            }
            if (rl == sb.resLvl && sbi == sb.sbandIdx)
                return sb;
            if (sb.sbandIdx != 0)
                sb = sb.Parent;
            while (sb.resLvl > rl)
                sb = sb.LL;
            while (sb.resLvl < rl)
                sb = sb.Parent;
            switch (sbi)
            {
                case 0:
                default:
                    return sb;
                case 1:
                    return sb.HL;
                case 2:
                    return sb.LH;
                case 3:
                    return sb.HH;
            }
        }
        public virtual Subband getSubband(int x, int y)
        {
            Subband cur, hhs;
            if (x < ulx || y < uly || x >= ulx + w || y >= uly + h)
            {
                throw new System.ArgumentException();
            }
            cur = this;
            while (cur.isNode)
            {
                hhs = cur.HH;
                if (x < hhs.ulx)
                {
                    if (y < hhs.uly)
                    {
                        cur = cur.LL;
                    }
                    else
                    {
                        cur = cur.LH;
                    }
                }
                else
                {
                    if (y < hhs.uly)
                    {
                        cur = cur.HL;
                    }
                    else
                    {
                        cur = cur.HH;
                    }
                }
            }
            return cur;
        }
        public override System.String ToString()
        {
            System.String string_Renamed = "w=" + w + ",h=" + h + ",ulx=" + ulx + ",uly=" + uly + ",ulcx=" + ulcx + ",ulcy=" + ulcy + ",idx=" + sbandIdx + ",orient=" + orientation + ",node=" + isNode + ",level=" + level + ",resLvl=" + resLvl + ",nomCBlkW=" + nomCBlkW + ",nomCBlkH=" + nomCBlkH + ",numCb=" + numCb;
            return string_Renamed;
        }
    }
}