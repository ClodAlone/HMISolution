#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion


#region File using directives

using System;

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class RtfNavigator
    {
        #region Constants

        #region String Constants

        #endregion

        #region Int constants
        /// <summary>
        /// 
        /// </summary>
        internal const int c_two = 2;
        internal const int c_twentiethOfPoint = 20;
        internal const int c_fiftiethOfPoint = 50;
        internal const float c_thirtyfive = 35.5F;
        #endregion

        #endregion

        #region Enums
        /// <summary>
        /// Specifies the flags/options for the unmanaged call to the GDI+ method
        /// Metafile.EmfToWmfBits().
        /// </summary>
        internal enum EmfToWmfBitsFlags
        {
            // Use the default conversion
            EmfToWmfBitsFlagsDefault = 0x00000000,

            // Embedded the source of the EMF metafiel within the resulting WMF
            // metafile
            EmfToWmfBitsFlagsEmbedEmf = 0x00000001,

            // Place a 22-byte header in the resulting WMF file.  The header is
            // required for the metafile to be considered placeable.
            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,

            // Don't simulate clipping by using the XOR operator.
            EmfToWmfBitsFlagsNoXORClip = 0x00000004
        };
        #endregion
    }
}

