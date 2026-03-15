#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.Pdf.JPEG2000.codestream.reader
{
    internal class PktInfo
    {
        public int packetIdx;
        public int layerIdx;
        public int cbOff = 0;
        public int cbLength;
        public int[] segLengths;
        public int numTruncPnts;
        public PktInfo(int lyIdx, int pckIdx)
        {
            layerIdx = lyIdx;
            packetIdx = pckIdx;
        }
        public override System.String ToString()
        {
            return "packet " + packetIdx + " (lay:" + layerIdx + ", off:" + cbOff + ", len:" + cbLength + ", numTruncPnts:" + numTruncPnts + ")\n";
        }
    }
}