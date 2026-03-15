#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Provides additional hints about a call to <see cref="GridStyleInfo.GetFormattedText"/>,
    /// or <see cref="GridStyleInfo.ApplyFormattedText"/>.
    /// </summary>
    public class GridCellBaseTextInfo
    {
        /// <summary>
        /// Clear cells operation.
        /// </summary>
        public const int ClearCells = 2;

        /// <summary>
        /// Copy text operation.
        /// </summary>
        public const int CopyText = 1;

        /// <summary>
        /// Current text query.
        /// </summary>
        public const int CurrentText = 3;

        /// <summary>
        /// Display text operation.
        /// </summary>
        public const int DisplayText = 0;

        /// <summary>
        /// No hint specified.
        /// </summary>
        public const int None = 0;

        /// <summary>
        /// Paste text operation.
        /// </summary>
        public const int PasteText = 1;

        /// <summary>
        /// Replace selection.
        /// </summary>
        public const int ReplaceSelection = 4;

        /// <summary>
        /// Initialize text box with text.
        /// </summary>
        public const int TextBox = 5;

        /// <summary>
        /// ValidateString checking if string is valid.
        /// </summary>
        public const int Validate = 6;

        private GridCellBaseTextInfo()
        {
        }
    }
}
