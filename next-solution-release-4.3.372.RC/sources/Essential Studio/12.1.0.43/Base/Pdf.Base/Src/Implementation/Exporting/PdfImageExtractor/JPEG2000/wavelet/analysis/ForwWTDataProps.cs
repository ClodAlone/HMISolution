#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.analysis
{
    internal interface ForwWTDataProps : ImageData
    {
        int CbULX
        {
            get;
        }
        int CbULY
        {
            get;
        }
        bool isReversible(int t, int c);
        SubbandAn getAnSubbandTree(int t, int c);
    }
}