#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet
{
    public struct WaveletTransform_Fields
    {
        public readonly static int WT_IMPL_LINE = 0;
        public readonly static int WT_IMPL_FULL = 2;
    }
    internal interface WaveletTransform : ImageData
    {
        bool isReversible(int t, int c);
        int getImplementationType(int c);
    }
}