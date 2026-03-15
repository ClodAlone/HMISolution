#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image
{
    public interface ImageData
    {
        int TileWidth
        {
            get;
        }
        int TileHeight
        {
            get;
        }
        int NomTileWidth
        {
            get;
        }
        int NomTileHeight
        {
            get;
        }
        int ImgWidth
        {
            get;
        }
        int ImgHeight
        {
            get;
        }
        int NumComps
        {
            get;
        }
        int TileIdx
        {
            get;
        }
        int TilePartULX
        {
            get;
        }
        int TilePartULY
        {
            get;
        }
        int ImgULX
        {
            get;
        }
        int ImgULY
        {
            get;
        }
        int getCompSubsX(int c);
        int getCompSubsY(int c);
        int getTileComponentWidth(int t, int c);
        int getTileComponentHeight(int t, int c);
        int getCompImgWidth(int c);
        int getCompImgHeight(int c);
        int getNomRangeBits(int c);
        void setTile(int x, int y);
        void nextTile();
        JPXImageCoordinates getTile(JPXImageCoordinates co);
        int getCompUpperLeftCornerX(int c);
        int getCompUpperLeftCornerY(int c);
        JPXImageCoordinates getNumTiles(JPXImageCoordinates co);
        int getNumTiles();
    }
}