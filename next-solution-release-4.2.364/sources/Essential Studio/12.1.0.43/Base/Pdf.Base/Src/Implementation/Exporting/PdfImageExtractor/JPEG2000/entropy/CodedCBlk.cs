#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.entropy
{
    public class CodedCBlk
    {
        public int n;
        public int m;
        public int skipMSBP;
        public byte[] data;
        public CodedCBlk()
        {
        }
        public CodedCBlk(int m, int n, int skipMSBP, byte[] data)
        {
            this.m = m;
            this.n = n;
            this.skipMSBP = skipMSBP;
            this.data = data;
        }
        public override System.String ToString()
        {
            return "m=" + m + ", n=" + n + ", skipMSBP=" + skipMSBP + ", data.length=" + ((data != null) ? "" + data.Length : "(null)");
        }
    }
}