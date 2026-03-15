#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Pdf
{
    enum TiffTag
    {
        /// <summary>
        /// Width of the image in pixels
        /// </summary>
        ImageWidth = 256,

        /// <summary>
        /// Height of the image in pixels
        /// </summary>
        ImageLength = 257,

        /// <summary>
        /// Bits per channel (sample).
        /// </summary>
        BitsPerSample = 258,

        /// <summary>
        /// Compression technique
        /// </summary>
        Compression = 259,

        /// <summary>
        /// Photometric interpretation.
        /// </summary>
        Photometric = 262,

        /// <summary>
        /// Offsets to data strips.
        /// </summary>
        StripOffset = 273,

        /// <summary>
        /// Samples per pixel.
        /// </summary>
        SamplesPerPixel = 277,

        /// <summary>
        /// Bytes counts for strips.
        /// </summary>
        StripByteCounts = 279,
    }
    enum TiffType : short
    {
        /// <summary>
        /// 16-bit unsigned integer.
        /// </summary>
        Short = 3,

        /// <summary>
        /// 32-bit unsigned integer.
        /// </summary>
        Long = 4,
    }
}
