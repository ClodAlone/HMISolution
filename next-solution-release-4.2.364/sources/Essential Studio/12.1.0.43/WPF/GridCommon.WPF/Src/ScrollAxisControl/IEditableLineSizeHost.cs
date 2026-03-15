#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// A collection that manages lines with varying height and hidden state. 
    /// It has properties for header and footer lines, total line count, default
    /// size of a line and also lets you add nested collections. Methods
    /// are provided for changing the values and getting the total extent.
    /// </summary>
    public interface IEditableLineSizeHost : ILineSizeHost
    {
        /// <summary>
        /// Gets the total extent which is the total of all line sizes. Note: This propert only 
        /// works if the DistanceCollection has been setup for pixel scrolling; otherwise it returns
        /// double.NaN.
        /// </summary>
        /// <value>The total extent or double.NaN.</value>
        double TotalExtent { get; }

        /// <summary>
        /// Gets whether the host supports nesting.
        /// </summary>
        bool SupportsNestedLines { get; }


        /// <summary>
        /// Gets the nested lines.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        IEditableLineSizeHost GetNestedLines(int index);

        /// <summary>
        /// Sets the nested lines.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="nestedLines">The nested lines.</param>
        void SetNestedLines(int index, IEditableLineSizeHost nestedLines);

        /// <summary>
        /// Gets whether the host supports inserting and removing lines.
        /// </summary>
        bool SupportsInsertRemove { get; }

        /// <summary>
        /// Insert a number of lines.
        /// </summary>
        /// <param name="insertAtLine">The index of the first line to insert.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveLines">A container with saved state from a preceeding <see cref="RemoveLines"/> call when lines should be moved. When it is null empty lines with default size are inserted.</param>
        void InsertLines(int insertAtLine, int count, IEditableLineSizeHost moveLines);

        /// <summary>
        /// Removes a number of lines.
        /// </summary>
        /// <param name="removeAtLine">The index of the first line to be removed.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveLines">A container to save state for a subsequent <see cref="InsertLines"/> call when lines should be moved.</param>
        void RemoveLines(int removeAtLine, int count, IEditableLineSizeHost moveLines);

        /// <summary>
        /// Creates the object which holds temporary state when moving lines.
        /// </summary>
        /// <returns></returns>
        IEditableLineSizeHost CreateMoveLines();

        /// <summary>
        /// Gets or sets the default size of lines.
        /// </summary>
        /// <value>The default size of lines.</value>
        double DefaultLineSize { get; set; }

        /// <summary>
        /// Gets or sets the footer line count.
        /// </summary>
        /// <value>The footer line count.</value>
        int FooterLineCount { get; set; }

        /// <summary>
        /// Gets or sets the header line count.
        /// </summary>
        /// <value>The header line count.</value>
        int HeaderLineCount { get; set; }

        /// <summary>
        /// Gets or sets the line count.
        /// </summary>
        /// <value>The line count.</value>
        int LineCount { get; set; }

        /// <summary>
        /// Sets the hidden state for a range of lines.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="hide">if set to <c>true</c> hide the lines.</param>
        void SetHidden(int from, int to, bool hide);

        /// <summary>
        /// Sets the line size for a range.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="size">The size.</param>
        void SetRange(int from, int to, double size);

        /// <summary>
        /// Gets or sets the line size at the specified index.
        /// </summary>
        /// <value></value>
        double this[int index] { get; set; }
    }

    public interface IPaddedEditableLineSizeHost : IEditableLineSizeHost
    {
        double PaddingDistance { get; set; }
        IDisposable DeferRefresh();
    }
}
