#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    public abstract class MultiResImgDataAdapter : MultiResImgData
    {
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
        internal int tIdx = 0;
        internal MultiResImgData mressrc;
        internal MultiResImgDataAdapter(MultiResImgData src)
        {
            mressrc = src;
        }
        public virtual int getTileWidth(int rl)
        {
            return mressrc.getTileWidth(rl);
        }
        public virtual int getTileHeight(int rl)
        {
            return mressrc.getTileHeight(rl);
        }
        public virtual int getImgWidth(int rl)
        {
            return mressrc.getImgWidth(rl);
        }
        public virtual int getImgHeight(int rl)
        {
            return mressrc.getImgHeight(rl);
        }
        public virtual int getCompSubsX(int c)
        {
            return mressrc.getCompSubsX(c);
        }
        public virtual int getCompSubsY(int c)
        {
            return mressrc.getCompSubsY(c);
        }
        public virtual int getTileCompWidth(int t, int c, int rl)
        {
            return mressrc.getTileCompWidth(t, c, rl);
        }
        public virtual int getTileCompHeight(int t, int c, int rl)
        {
            return mressrc.getTileCompHeight(t, c, rl);
        }
        public virtual int getCompImgWidth(int c, int rl)
        {
            return mressrc.getCompImgWidth(c, rl);
        }
        public virtual int getCompImgHeight(int c, int rl)
        {
            return mressrc.getCompImgHeight(c, rl);
        }
        public virtual void setTile(int x, int y)
        {
            mressrc.setTile(x, y);
            tIdx = TileIdx;
        }
        public virtual void nextTile()
        {
            mressrc.nextTile();
            tIdx = TileIdx;
        }
        public virtual JPXImageCoordinates getTile(JPXImageCoordinates co)
        {
            return mressrc.getTile(co);
        }
        public virtual int getResULX(int c, int rl)
        {
            return mressrc.getResULX(c, rl);
        }
        public virtual int getResULY(int c, int rl)
        {
            return mressrc.getResULY(c, rl);
        }
        public virtual int getImgULX(int rl)
        {
            return mressrc.getImgULX(rl);
        }
        public virtual int getImgULY(int rl)
        {
            return mressrc.getImgULY(rl);
        }
        public virtual JPXImageCoordinates getNumTiles(JPXImageCoordinates co)
        {
            return mressrc.getNumTiles(co);
        }
        public virtual int getNumTiles()
        {
            return mressrc.getNumTiles();
        }
        public abstract Syncfusion.Pdf.JPEG2000.wavelet.synthesis.SubbandSyn getSynSubbandTree(int param1, int param2);
    }
}