#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image.output
{
    internal class ImgWriterPGX 
    {
        internal int maxVal;
        internal int minVal;
        internal int levShift;
        internal bool isSigned;
        private int bitDepth;
        private System.IO.FileStream out_Renamed;
        private int offset;
        private DataBlockInt db = new DataBlockInt();
        private int fb;
        private int c;
        private int packBytes;
        private byte[] buf;
        
    }
}