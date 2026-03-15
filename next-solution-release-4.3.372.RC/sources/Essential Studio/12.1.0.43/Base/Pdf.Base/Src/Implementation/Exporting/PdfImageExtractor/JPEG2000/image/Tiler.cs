#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image
{
    internal class Tiler : ImgDataAdapter, BlockImageDataSource
    {
        override public int TileWidth
        {
            get
            {
                return tileW;
            }
        }
        override public int TileHeight
        {
            get
            {
                return tileH;
            }
        }
        override public int TileIdx
        {
            get
            {
                return ty * ntX + tx;
            }
        }
        override public int TilePartULX
        {
            get
            {
                return xt0siz;
            }
        }
        override public int TilePartULY
        {
            get
            {
                return yt0siz;
            }
        }
        override public int ImgULX
        {
            get
            {
                return x0siz;
            }
        }
        override public int ImgULY
        {
            get
            {
                return y0siz;
            }
        }
        override public int NomTileWidth
        {
            get
            {
                return xtsiz;
            }
        }
        override public int NomTileHeight
        {
            get
            {
                return ytsiz;
            }
        }
        private BlockImageDataSource src = null;
        private int x0siz;
        private int y0siz;
        private int xt0siz;
        private int yt0siz;
        private int xtsiz;
        private int ytsiz;
        private int ntX;
        private int ntY;
        private int[] compW = null;
        private int[] compH = null;
        private int[] tcx0 = null;
        private int[] tcy0 = null;
        private int tx;
        private int ty;
        private int tileW;
        private int tileH;
        internal Tiler(BlockImageDataSource src, int ax, int ay, int px, int py, int nw, int nh)
            : base(src)
        {
            this.src = src;
            this.x0siz = ax;
            this.y0siz = ay;
            this.xt0siz = px;
            this.yt0siz = py;
            this.xtsiz = nw;
            this.ytsiz = nh;
            if (src.getNumTiles() != 1)
            {
                throw new System.ArgumentException("Source is tiled");
            }
            if (src.ImgULX != 0 || src.ImgULY != 0)
            {
                throw new System.ArgumentException("Source is \"canvased\"");
            }
            if (x0siz < 0 || y0siz < 0 || xt0siz < 0 || yt0siz < 0 || xtsiz < 0 || ytsiz < 0 || xt0siz > x0siz || yt0siz > y0siz)
            {
                throw new System.ArgumentException("Invalid image origin, " + "tiling origin or nominal " + "tile size");
            }
            if (xtsiz == 0)
                xtsiz = x0siz + src.ImgWidth - xt0siz;
            if (ytsiz == 0)
                ytsiz = y0siz + src.ImgHeight - yt0siz;
            if (x0siz - xt0siz >= xtsiz)
            {
                xt0siz += ((x0siz - xt0siz) / xtsiz) * xtsiz;
            }
            if (y0siz - yt0siz >= ytsiz)
            {
                yt0siz += ((y0siz - yt0siz) / ytsiz) * ytsiz;
            }
            if (x0siz - xt0siz >= xtsiz || y0siz - yt0siz >= ytsiz)
            {
            }
            ntX = (int)System.Math.Ceiling((x0siz + src.ImgWidth) / (double)xtsiz);
            ntY = (int)System.Math.Ceiling((y0siz + src.ImgHeight) / (double)ytsiz);
        }
        public override int getTileComponentWidth(int t, int c)
        {
            if (t != TileIdx)
            {
                //throw new System.ApplicationException("Asking the width of a tile-component which is " + "not in the current tile (call setTile() or " + "nextTile() methods before).");
            }
            return compW[c];
        }
        public override int getTileComponentHeight(int t, int c)
        {
            if (t != TileIdx)
            {
                //throw new System.ApplicationException("Asking the width of a tile-component which is " + "not in the current tile (call setTile() or " + "nextTile() methods before).");
            }
            return compH[c];
        }
        public virtual int getFixedPoint(int c)
        {
            return src.getFixedPoint(c);
        }
        public DataBlock getInternCompData(DataBlock blk, int c)
        {
            if (blk.ulx < 0 || blk.uly < 0 || blk.w > compW[c] || blk.h > compH[c])
            {
                throw new System.ArgumentException("Block is outside the tile");
            }
            int incx = (int)System.Math.Ceiling(x0siz / (double)src.getCompSubsX(c));
            int incy = (int)System.Math.Ceiling(y0siz / (double)src.getCompSubsY(c));
            blk.ulx -= incx;
            blk.uly -= incy;
            blk = src.getInternCompData(blk, c);
            blk.ulx += incx;
            blk.uly += incy;
            return blk;
        }
        public DataBlock getCompData(DataBlock blk, int c)
        {
            if (blk.ulx < 0 || blk.uly < 0 || blk.w > compW[c] || blk.h > compH[c])
            {
                throw new System.ArgumentException("Block is outside the tile");
            }
            int incx = (int)System.Math.Ceiling(x0siz / (double)src.getCompSubsX(c));
            int incy = (int)System.Math.Ceiling(y0siz / (double)src.getCompSubsY(c));
            blk.ulx -= incx;
            blk.uly -= incy;
            blk = src.getCompData(blk, c);
            blk.ulx += incx;
            blk.uly += incy;
            return blk;
        }
        public override void setTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= ntX || y >= ntY)
            {
                throw new System.ArgumentException("Tile's indexes out of bounds");
            }
            tx = x;
            ty = y;
            int tx0 = (x != 0) ? xt0siz + x * xtsiz : x0siz;
            int ty0 = (y != 0) ? yt0siz + y * ytsiz : y0siz;
            int tx1 = (x != ntX - 1) ? (xt0siz + (x + 1) * xtsiz) : (x0siz + src.ImgWidth);
            int ty1 = (y != ntY - 1) ? (yt0siz + (y + 1) * ytsiz) : (y0siz + src.ImgHeight);
            tileW = tx1 - tx0;
            tileH = ty1 - ty0;
            int nc = src.NumComps;
            if (compW == null)
                compW = new int[nc];
            if (compH == null)
                compH = new int[nc];
            if (tcx0 == null)
                tcx0 = new int[nc];
            if (tcy0 == null)
                tcy0 = new int[nc];
            for (int i = 0; i < nc; i++)
            {
                tcx0[i] = (int)System.Math.Ceiling(tx0 / (double)src.getCompSubsX(i));
                tcy0[i] = (int)System.Math.Ceiling(ty0 / (double)src.getCompSubsY(i));
                compW[i] = (int)System.Math.Ceiling(tx1 / (double)src.getCompSubsX(i)) - tcx0[i];
                compH[i] = (int)System.Math.Ceiling(ty1 / (double)src.getCompSubsY(i)) - tcy0[i];
            }
        }
        public override void nextTile()
        {
            if (tx == ntX - 1 && ty == ntY - 1)
            {
                throw new System.Exception();
            }
            else if (tx < ntX - 1)
            {
                setTile(tx + 1, ty);
            }
            else
            {
                setTile(0, ty + 1);
            }
        }
        public override JPXImageCoordinates getTile(JPXImageCoordinates co)
        {
            if (co != null)
            {
                co.x = tx;
                co.y = ty;
                return co;
            }
            else
            {
                return new JPXImageCoordinates(tx, ty);
            }
        }
        public override int getCompUpperLeftCornerX(int c)
        {
            return tcx0[c];
        }
        public override int getCompUpperLeftCornerY(int c)
        {
            return tcy0[c];
        }
        public override JPXImageCoordinates getNumTiles(JPXImageCoordinates co)
        {
            if (co != null)
            {
                co.x = ntX;
                co.y = ntY;
                return co;
            }
            else
            {
                return new JPXImageCoordinates(ntX, ntY);
            }
        }
        public override int getNumTiles()
        {
            return ntX * ntY;
        }
        public JPXImageCoordinates getTilingOrigin(JPXImageCoordinates co)
        {
            if (co != null)
            {
                co.x = xt0siz;
                co.y = yt0siz;
                return co;
            }
            else
            {
                return new JPXImageCoordinates(xt0siz, yt0siz);
            }
        }
        public override System.String ToString()
        {
            return "Tiler: source= " + src + "\n" + getNumTiles() + " tile(s), nominal width=" + xtsiz + ", nominal height=" + ytsiz;
        }
    }
}