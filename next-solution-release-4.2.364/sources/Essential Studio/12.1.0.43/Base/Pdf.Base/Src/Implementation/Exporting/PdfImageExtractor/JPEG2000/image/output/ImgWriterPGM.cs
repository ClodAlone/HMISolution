#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.image.output
{
    internal class ImgWriterPGM 
    {
        private int levShift;
        private System.IO.FileStream out_Renamed;
        private int c;
        private int fb;
        private DataBlockInt db = new DataBlockInt();
        private int offset;
        private byte[] buf;
        
    }
}