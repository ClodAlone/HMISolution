#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image.input
{
    public abstract class ImgReader : BlockImageDataSource
    {
        virtual public int TileWidth
        {
            get
            {
                return w;
            }
        }
        virtual public int TileHeight
        {
            get
            {
                return h;
            }
        }
        virtual public int NomTileWidth
        {
            get
            {
                return w;
            }
        }
        virtual public int NomTileHeight
        {
            get
            {
                return h;
            }
        }
        virtual public int ImgWidth
        {
            get
            {
                return w;
            }
        }
        virtual public int ImgHeight
        {
            get
            {
                return h;
            }
        }
        virtual public int NumComps
        {
            get
            {
                return nc;
            }
        }
        virtual public int TileIdx
        {
            get
            {
                return 0;
            }
        }
        virtual public int TilePartULX
        {
            get
            {
                return 0;
            }
        }
        virtual public int TilePartULY
        {
            get
            {
                return 0;
            }
        }
        virtual public int ImgULX
        {
            get
            {
                return 0;
            }
        }
        virtual public int ImgULY
        {
            get
            {
                return 0;
            }
        }
        internal int w;
        internal int h;
        internal int nc;
        public abstract void close();
        public virtual int getCompSubsX(int c)
        {
            return 1;
        }
        public virtual int getCompSubsY(int c)
        {
            return 1;
        }
        public virtual int getTileComponentWidth(int t, int c)
        {
            if (t != 0)
            {
                //throw new System.ApplicationException("Asking a tile-component width for a tile index" + " greater than 0 whereas there is only one tile");
            }
            return w;
        }
        public virtual int getTileComponentHeight(int t, int c)
        {
            if (t != 0)
            {
                //throw new System.ApplicationException("Asking a tile-component width for a tile index" + " greater than 0 whereas there is only one tile");
            }
            return h;
        }
        public virtual int getCompImgWidth(int c)
        {
            return w;
        }
        public virtual int getCompImgHeight(int c)
        {
            return h;
        }
        public virtual void setTile(int x, int y)
        {
            if (x != 0 || y != 0)
            {
                throw new System.ArgumentException();
            }
        }
        public virtual void nextTile()
        {
            throw new System.Exception();
        }
        public virtual JPXImageCoordinates getTile(JPXImageCoordinates co)
        {
            if (co != null)
            {
                co.x = 0;
                co.y = 0;
                return co;
            }
            else
            {
                return new JPXImageCoordinates(0, 0);
            }
        }
        public virtual int getCompUpperLeftCornerX(int c)
        {
            return 0;
        }
        public virtual int getCompUpperLeftCornerY(int c)
        {
            return 0;
        }
        public virtual JPXImageCoordinates getNumTiles(JPXImageCoordinates co)
        {
            if (co != null)
            {
                co.x = 1;
                co.y = 1;
                return co;
            }
            else
            {
                return new JPXImageCoordinates(1, 1);
            }
        }
        public virtual int getNumTiles()
        {
            return 1;
        }
        public abstract bool isOrigSigned(int c);
        public abstract int getFixedPoint(int param1);
        public abstract Syncfusion.Pdf.JPEG2000.image.DataBlock getInternCompData(Syncfusion.Pdf.JPEG2000.image.DataBlock param1, int param2);
        public abstract int getNomRangeBits(int param1);
        public abstract Syncfusion.Pdf.JPEG2000.image.DataBlock getCompData(Syncfusion.Pdf.JPEG2000.image.DataBlock param1, int param2);
    }
}