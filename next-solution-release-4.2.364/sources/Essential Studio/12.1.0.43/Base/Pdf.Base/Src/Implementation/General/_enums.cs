#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// The encoding format for the sample data.
    /// </summary>
    public enum PdfSoundEncoding
    {
        /// <summary>
        /// Unspecified or unsigned values in the range 0 to 2^B - 1.
        /// </summary>
        Raw,

        /// <summary>
        /// Twos-complement values.
        /// </summary>
        Signed,

        /// <summary>
        /// M-law�encoded samples.
        /// </summary>
        MuLaw,

        /// <summary>
        /// A-law�encoded samples.
        /// </summary>
        ALaw
    }

    /// <summary>
    /// The number of sound channels.
    /// </summary>
    public enum PdfSoundChannels
    {
        /// <summary>
        /// One channel.
        /// </summary>
        Mono = 1,

        /// <summary>
        /// Two channels.
        /// </summary>
        Stereo = 2
    }

    /// <summary>
    /// Enumeration that represents fit mode.
    /// </summary>
    public enum PdfDestinationMode
    {
        /// <summary>
        /// Display the page designated by page, with the coordinates (left, top) positioned
        /// at the top-left corner of the window and the contents of the page magnified
        /// by the factor zoom. A NULL value for any of the parameters left, top, or
        /// zoom specifies that the current value of that parameter is to be retained unchanged.
        /// A zoom value of 0 has the same meaning as a NULL value.
        /// </summary>
        Location,

        /// <summary>
        /// Display the page designated by page, with its contents magnified just enough
        /// to fit the entire page within the window both horizontally and vertically. If
        /// the required horizontal and vertical magnification factors are different, use
        /// the smaller of the two, centering the page within the window in the other
        /// dimension.
        /// </summary>
        FitToPage,
        
        FitR

        /*,
    /// <summary>
    /// Display the page designated by page, with the vertical coordinate top positioned
    /// at the top edge of the window and the contents of the page magnified
    /// just enough to fit the entire width of the page within the window.
    /// </summary>
    FitH,
    /// <summary>
    /// Display the page designated by page, with the horizontal coordinate left positioned
    /// at the left edge of the window and the contents of the page magnified
    /// just enough to fit the entire height of the page within the window.
    /// </summary>
    FitV,
    /// <summary>
    /// Display the page designated by page, with its contents magnified
    /// just enough to fit its bounding box entirely within the window both horizontally
    /// and vertically. If the required horizontal and vertical magnification
    /// factors are different, use the smaller of the two, centering the bounding box
    /// within the window in the other dimension.
    /// </summary>
    FitB,
    /// <summary>
    /// Display the page designated by page, with the vertical coordinate
    /// top positioned at the top edge of the window and the contents of the page
    /// magnified just enough to fit the entire width of its bounding box within the
    /// window.
    /// </summary>
    FitBH,
    /// <summary>
    /// Display the page designated by page, with the horizontal coordinate
    /// left positioned at the left edge of the window and the contents of the page
    /// magnified just enough to fit the entire height of its bounding box within the
    /// window.
    /// </summary>
    FitBV*/
    }
}
