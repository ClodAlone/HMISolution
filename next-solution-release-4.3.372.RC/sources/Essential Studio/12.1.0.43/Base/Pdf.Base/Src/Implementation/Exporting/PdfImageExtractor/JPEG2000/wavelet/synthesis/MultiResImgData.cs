#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    public interface MultiResImgData
    {
        int NomTileWidth
        {
            get;
        }
        int NomTileHeight
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
        int getTileWidth(int rl);
        int getTileHeight(int rl);
        int getImgWidth(int rl);
        int getImgHeight(int rl);
        int getCompSubsX(int c);
        int getCompSubsY(int c);
        int getTileCompWidth(int t, int c, int rl);
        int getTileCompHeight(int t, int c, int rl);
        int getCompImgWidth(int c, int rl);
        int getCompImgHeight(int n, int rl);
        void setTile(int x, int y);
        void nextTile();
        JPXImageCoordinates getTile(JPXImageCoordinates co);
        int getResULX(int c, int rl);
        int getResULY(int c, int rl);
        int getImgULX(int rl);
        int getImgULY(int rl);
        JPXImageCoordinates getNumTiles(JPXImageCoordinates co);
        int getNumTiles();
        SubbandSyn getSynSubbandTree(int t, int c);
    }
}