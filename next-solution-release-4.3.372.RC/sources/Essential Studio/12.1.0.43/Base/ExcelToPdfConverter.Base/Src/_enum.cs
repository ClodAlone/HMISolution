#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

namespace Syncfusion.ExcelToPdfConverter
{
    /// <summary>
    /// Enumeration represents the Displaystyle of the Gridlines
    /// </summary>
    public enum GridLinesDisplayStyle
    {
        /// <summary>
        /// Represents the automatic gridline visibility.
        /// </summary>
        Auto,

        /// <summary>
        /// Represents that the gridline should be rendered in the output page.
        /// </summary>
        Visible,

        /// <summary>
        /// Represents that the gridline should not be rendered in the output page.
        /// </summary>
        Invisible,
    }

    /// <summary>
    /// Enumeration represents the Layout of the Output Document.
    /// </summary>
    public enum LayoutOptions
    {
        /// <summary>
        /// Represents the FitSheetOnOnePage option of Excel pagesetup.
        /// </summary>
        FitSheetOnOnePage=1,

        /// <summary>
        /// Represents the NoScaling option of Excel pagesetup.
        /// </summary>
        NoScaling=2,

        /// <summary>
        /// Represents the FitAllColumnsOnOnePage option of Excel pagesetup.
        /// </summary>
        FitAllColumnsOnOnePage=4,

        /// <summary>
        /// Represents the FitAllRowsOnOnePage option of Excel pagesetup.
        /// </summary>
        FitAllRowsOnOnePage=8,
       
        /// <summary>
        /// Represents the Custom Scaling option of Excel pagesetup.
        /// </summary>
        CustomScaling =16,
        /// <summary>
        /// Represents the Automatic Layout Option.
        /// </summary>
        Automatic=32
    }
}
