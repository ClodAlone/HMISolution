#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image
{
    internal abstract class ImgDataAdapter : ImageData
    {
        virtual public int TileWidth
        {
            get
            {
                return imgdatasrc.TileWidth;
            }
        }
        virtual public int TileHeight
        {
            get
            {
                return imgdatasrc.TileHeight;
            }
        }
        virtual public int NomTileWidth
        {
            get
            {
                return imgdatasrc.NomTileWidth;
            }
        }
        virtual public int NomTileHeight
        {
            get
            {
                return imgdatasrc.NomTileHeight;
            }
        }
        virtual public int ImgWidth
        {
            get
            {
                return imgdatasrc.ImgWidth;
            }
        }
        virtual public int ImgHeight
        {
            get
            {
                return imgdatasrc.ImgHeight;
            }
        }
        virtual public int NumComps
        {
            get
            {
                return imgdatasrc.NumComps;
            }
        }
        virtual public int TileIdx
        {
            get
            {
                return imgdatasrc.TileIdx;
            }
        }
        virtual public int TilePartULX
        {
            get
            {
                return imgdatasrc.TilePartULX;
            }
        }
        virtual public int TilePartULY
        {
            get
            {
                return imgdatasrc.TilePartULY;
            }
        }
        virtual public int ImgULX
        {
            get
            {
                return imgdatasrc.ImgULX;
            }
        }
        virtual public int ImgULY
        {
            get
            {
                return imgdatasrc.ImgULY;
            }
        }
        internal int tIdx = 0;
        internal ImageData imgdatasrc;
        internal ImgDataAdapter(ImageData src)
        {
            imgdatasrc = src;
        }
        public virtual int getCompSubsX(int c)
        {
            return imgdatasrc.getCompSubsX(c);
        }
        public virtual int getCompSubsY(int c)
        {
            return imgdatasrc.getCompSubsY(c);
        }
        public virtual int getTileComponentWidth(int t, int c)
        {
            return imgdatasrc.getTileComponentWidth(t, c);
        }
        public virtual int getTileComponentHeight(int t, int c)
        {
            return imgdatasrc.getTileComponentHeight(t, c);
        }
        public virtual int getCompImgWidth(int c)
        {
            return imgdatasrc.getCompImgWidth(c);
        }
        public virtual int getCompImgHeight(int c)
        {
            return imgdatasrc.getCompImgHeight(c);
        }
        public virtual int getNomRangeBits(int c)
        {
            return imgdatasrc.getNomRangeBits(c);
        }
        public virtual void setTile(int x, int y)
        {
            imgdatasrc.setTile(x, y);
            tIdx = TileIdx;
        }
        public virtual void nextTile()
        {
            imgdatasrc.nextTile();
            tIdx = TileIdx;
        }
        public virtual JPXImageCoordinates getTile(JPXImageCoordinates co)
        {
            return imgdatasrc.getTile(co);
        }
        public virtual int getCompUpperLeftCornerX(int c)
        {
            return imgdatasrc.getCompUpperLeftCornerX(c);
        }
        public virtual int getCompUpperLeftCornerY(int c)
        {
            return imgdatasrc.getCompUpperLeftCornerY(c);
        }
        public virtual JPXImageCoordinates getNumTiles(JPXImageCoordinates co)
        {
            return imgdatasrc.getNumTiles(co);
        }
        public virtual int getNumTiles()
        {
            return imgdatasrc.getNumTiles();
        }
    }
}