#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image
{
    internal class ImgDataJoiner : BlockImageDataSource
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
        private int w;
        private int h;
        private int nc;
        private BlockImageDataSource[] imageData;
        private int[] compIdx;
        private int[] subsX;
        private int[] subsY;
        internal ImgDataJoiner(BlockImageDataSource[] imD, int[] cIdx)
        {
            int i;
            int maxW, maxH;
            imageData = imD;
            compIdx = cIdx;
            if (imageData.Length != compIdx.Length)
                throw new System.ArgumentException("imD and cIdx must have the" + " same length");
            nc = imD.Length;
            subsX = new int[nc];
            subsY = new int[nc];
            for (i = 0; i < nc; i++)
            {
                if (imD[i].getNumTiles() != 1 || imD[i].getCompUpperLeftCornerX(cIdx[i]) != 0 || imD[i].getCompUpperLeftCornerY(cIdx[i]) != 0)
                {
                    throw new System.ArgumentException("All input components must, " + "not use tiles and must " + "have " + "the origin at the canvas " + "origin");
                }
            }
            maxW = 0;
            maxH = 0;
            for (i = 0; i < nc; i++)
            {
                if (imD[i].getCompImgWidth(cIdx[i]) > maxW)
                    maxW = imD[i].getCompImgWidth(cIdx[i]);
                if (imD[i].getCompImgHeight(cIdx[i]) > maxH)
                    maxH = imD[i].getCompImgHeight(cIdx[i]);
            }
            w = maxW;
            h = maxH;
            for (i = 0; i < nc; i++)
            {
                subsX[i] = (maxW + imD[i].getCompImgWidth(cIdx[i]) - 1) / imD[i].getCompImgWidth(cIdx[i]);
                subsY[i] = (maxH + imD[i].getCompImgHeight(cIdx[i]) - 1) / imD[i].getCompImgHeight(cIdx[i]);
                if ((maxW + subsX[i] - 1) / subsX[i] != imD[i].getCompImgWidth(cIdx[i]) || (maxH + subsY[i] - 1) / subsY[i] != imD[i].getCompImgHeight(cIdx[i]))
                {
                    //throw new System.ApplicationException("Can not compute component subsampling " + "factors: strange subsampling.");
                }
            }
        }
        public virtual int getCompSubsX(int c)
        {
            return subsX[c];
        }
        public virtual int getCompSubsY(int c)
        {
            return subsY[c];
        }
        public virtual int getTileComponentWidth(int t, int c)
        {
            return imageData[c].getTileComponentWidth(t, compIdx[c]);
        }
        public virtual int getTileComponentHeight(int t, int c)
        {
            return imageData[c].getTileComponentHeight(t, compIdx[c]);
        }
        public virtual int getCompImgWidth(int c)
        {
            return imageData[c].getCompImgWidth(compIdx[c]);
        }
        public virtual int getCompImgHeight(int n)
        {
            return imageData[n].getCompImgHeight(compIdx[n]);
        }
        public virtual int getNomRangeBits(int c)
        {
            return imageData[c].getNomRangeBits(compIdx[c]);
        }
        public virtual int getFixedPoint(int c)
        {
            return imageData[c].getFixedPoint(compIdx[c]);
        }
        public virtual DataBlock getInternCompData(DataBlock blk, int c)
        {
            return imageData[c].getInternCompData(blk, compIdx[c]);
        }
        public virtual DataBlock getCompData(DataBlock blk, int c)
        {
            return imageData[c].getCompData(blk, compIdx[c]);
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
        public override System.String ToString()
        {
            System.String string_Renamed = "ImgDataJoiner: WxH = " + w + "x" + h;
            for (int i = 0; i < nc; i++)
            {
                string_Renamed += ("\n- Component " + i + " " + imageData[i]);
            }
            return string_Renamed;
        }
    }
}