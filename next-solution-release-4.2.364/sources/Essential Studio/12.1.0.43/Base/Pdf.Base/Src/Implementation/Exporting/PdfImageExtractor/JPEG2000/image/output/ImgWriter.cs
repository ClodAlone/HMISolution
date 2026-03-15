#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image.output
{
    public abstract class ImgWriter
    {
        public const int DEF_STRIP_HEIGHT = 64;
        internal BlockImageDataSource src;
        internal int w;
        internal int h;
        public abstract void close();
        public abstract void flush();
        ~ImgWriter()
        {
            flush();
        }
        public abstract void write();
        public virtual void writeAll()
        {
            JPXImageCoordinates nT = src.getNumTiles(null);
            for (int y = 0; y < nT.y; y++)
            {
                for (int x = 0; x < nT.x; x++)
                {
                    src.setTile(x, y);
                    write();
                }
            }
        }
        public abstract void write(int ulx, int uly, int w, int h);
    }
}