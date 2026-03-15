#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.JPEG2000.decoder;
using Syncfusion.Pdf.JPEG2000.image;
namespace Syncfusion.Pdf.JPEG2000.wavelet.synthesis
{
    public abstract class WaveletTransformInverse : InvWTAdapter, BlockImageDataSource
    {
        internal WaveletTransformInverse(MultiResImgData src, DecodeHelper decSpec)
            : base(src, decSpec)
        {
        }
        internal static WaveletTransformInverse createInstance(CBlkWTDataSrcDec src, DecodeHelper decSpec)
        {
            return new InvWTFull(src, decSpec);
        }
        public abstract int getFixedPoint(int param1);
        public abstract Syncfusion.Pdf.JPEG2000.image.DataBlock getInternCompData(Syncfusion.Pdf.JPEG2000.image.DataBlock param1, int param2);
        public abstract Syncfusion.Pdf.JPEG2000.image.DataBlock getCompData(Syncfusion.Pdf.JPEG2000.image.DataBlock param1, int param2);
    }
}