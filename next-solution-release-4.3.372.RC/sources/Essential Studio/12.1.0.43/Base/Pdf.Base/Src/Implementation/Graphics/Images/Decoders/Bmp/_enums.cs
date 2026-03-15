#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Pdf.Graphics.Images.Decoder
{
    public enum BitmapType
    {
        Version2_1Bit,
        Version2_4Bit,
        Version2_8Bit,
        Version2_24Bit,
        Version3_1Bit,
        Version3_4Bit,
        Version3_8Bit,
        Version3_16Bit,
        Version3_24Bit,
        Version3_32Bit,
        Version4_1Bit,
        Version4_4Bit,
        Version4_8Bit,
        Version4_16Bit,
        Version4_24Bit,
        Version4_32Bit,
    }

    public enum BitmapColorSpace
    {
        CalibratedRGB,
        SRGB,
        CMYK
    }

    public enum BitmapCompression
    {
        RGB = 0,
        RunlengthEncoding8 = 1,
        RunlengthEncoding4 = 2,
        Bitfield = 3
    }
}
