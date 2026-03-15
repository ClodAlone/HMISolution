#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.wavelet.synthesis;
namespace Syncfusion.Pdf.JPEG2000.entropy.decoder
{
    public interface CodedCBlkDataSrcDec : InvWTData
    {
        DecLyrdCBlk getCodeBlock(int c, int m, int n, SubbandSyn sb, int fl, int nl, DecLyrdCBlk ccb);
    }
}