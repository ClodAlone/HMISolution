#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    public abstract class InvWTAdapter : InvWT
    {
        virtual public int ImgResLevel
        {
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException("Resolution level index " + "cannot be negative.");
                }
                reslvl = value;
            }
        }
        virtual public int TileWidth
        {
            get
            {
                int tIdx = TileIdx;
                int rl = 10000;
                int mrl;
                int nc = mressrc.NumComps;
                for (int c = 0; c < nc; c++)
                {
                    mrl = mressrc.getSynSubbandTree(tIdx, c).resLvl;
                    if (mrl < rl)
                        rl = mrl;
                }
                return mressrc.getTileWidth(rl);
            }
        }
        virtual public int TileHeight
        {
            get
            {
                int tIdx = TileIdx;
                int rl = 10000;
                int mrl;
                int nc = mressrc.NumComps;
                for (int c = 0; c < nc; c++)
                {
                    mrl = mressrc.getSynSubbandTree(tIdx, c).resLvl;
                    if (mrl < rl)
                        rl = mrl;
                }
                return mressrc.getTileHeight(rl);
            }
        }
        virtual public int NomTileWidth
        {
            get
            {
                return mressrc.NomTileWidth;
            }
        }
        virtual public int NomTileHeight
        {
            get
            {
                return mressrc.NomTileHeight;
            }
        }
        virtual public int ImgWidth
        {
            get
            {
                return mressrc.getImgWidth(reslvl);
            }
        }
        virtual public int ImgHeight
        {
            get
            {
                return mressrc.getImgHeight(reslvl);
            }
        }
        virtual public int NumComps
        {
            get
            {
                return mressrc.NumComps;
            }
        }
        virtual public int TileIdx
        {
            get
            {
                return mressrc.TileIdx;
            }
        }
        virtual public int ImgULX
        {
            get
            {
                return mressrc.getImgULX(reslvl);
            }
        }
        virtual public int ImgULY
        {
            get
            {
                return mressrc.getImgULY(reslvl);
            }
        }
        virtual public int TilePartULX
        {
            get
            {
                return mressrc.TilePartULX;
            }
        }
        virtual public int TilePartULY
        {
            get
            {
                return mressrc.TilePartULY;
            }
        }
        internal DecodeHelper decSpec;
        internal MultiResImgData mressrc;
        internal int reslvl;
        internal int maxImgRes;
        internal InvWTAdapter(MultiResImgData src, DecodeHelper decSpec)
        {
            mressrc = src;
            this.decSpec = decSpec;
            maxImgRes = decSpec.dls.Min;
        }
        public virtual int getCompSubsX(int c)
        {
            return mressrc.getCompSubsX(c);
        }
        public virtual int getCompSubsY(int c)
        {
            return mressrc.getCompSubsY(c);
        }
        public virtual int getTileComponentWidth(int t, int c)
        {
            int rl = mressrc.getSynSubbandTree(t, c).resLvl;
            return mressrc.getTileCompWidth(t, c, rl);
        }
        public virtual int getTileComponentHeight(int t, int c)
        {
            int rl = mressrc.getSynSubbandTree(t, c).resLvl;
            return mressrc.getTileCompHeight(t, c, rl);
        }
        public virtual int getCompImgWidth(int c)
        {
            int rl = decSpec.dls.getMinInComp(c);
            return mressrc.getCompImgWidth(c, rl);
        }
        public virtual int getCompImgHeight(int c)
        {
            int rl = decSpec.dls.getMinInComp(c);
            return mressrc.getCompImgHeight(c, rl);
        }
        public virtual void setTile(int x, int y)
        {
            mressrc.setTile(x, y);
        }
        public virtual void nextTile()
        {
            mressrc.nextTile();
        }
        public virtual JPXImageCoordinates getTile(JPXImageCoordinates co)
        {
            return mressrc.getTile(co);
        }
        public virtual int getCompUpperLeftCornerX(int c)
        {
            int tIdx = TileIdx;
            int rl = mressrc.getSynSubbandTree(tIdx, c).resLvl;
            return mressrc.getResULX(c, rl);
        }
        public virtual int getCompUpperLeftCornerY(int c)
        {
            int tIdx = TileIdx;
            int rl = mressrc.getSynSubbandTree(tIdx, c).resLvl;
            return mressrc.getResULY(c, rl);
        }
        public virtual JPXImageCoordinates getNumTiles(JPXImageCoordinates co)
        {
            return mressrc.getNumTiles(co);
        }
        public virtual int getNumTiles()
        {
            return mressrc.getNumTiles();
        }
        internal virtual SubbandSyn getSynSubbandTree(int t, int c)
        {
            return mressrc.getSynSubbandTree(t, c);
        }
        public abstract bool isReversible(int param1, int param2);
        public abstract int getNomRangeBits(int param1);
        public abstract int getImplementationType(int param1);
    }
}