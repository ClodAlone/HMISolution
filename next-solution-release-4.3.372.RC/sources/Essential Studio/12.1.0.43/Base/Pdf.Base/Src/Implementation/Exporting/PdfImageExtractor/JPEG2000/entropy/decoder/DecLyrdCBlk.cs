#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.entropy.decoder
{
    public class DecLyrdCBlk : CodedCBlk
    {
        public int ulx;
        public int uly;
        public int w;
        public int h;
        public int dl;
        public bool prog;
        public int nl;
        public int ftpIdx;
        public int nTrunc;
        public int[] tsLengths;
        public override System.String ToString()
        {
            System.String str = "Coded code-block (" + m + "," + n + "): " + skipMSBP + " MSB skipped, " + dl + " bytes, " + nTrunc + " truncation points, " + nl + " layers, " + "progressive=" + prog + ", ulx=" + ulx + ", uly=" + uly + ", w=" + w + ", h=" + h + ", ftpIdx=" + ftpIdx;
            if (tsLengths != null)
            {
                str += " {";
                for (int i = 0; i < tsLengths.Length; i++)
                    str += (" " + tsLengths[i]);
                str += " }";
            }
            return str;
        }
    }
}