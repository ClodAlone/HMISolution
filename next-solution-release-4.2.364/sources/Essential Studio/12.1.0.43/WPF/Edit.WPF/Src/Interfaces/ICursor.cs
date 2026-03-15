// <copyright file="ICursor.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// ICursor interface has the Cursor properties of Cursor layer
    /// </summary>
    /// <remarks>ICorsor has CursorIndex and LineNumber property.  The CursorIndex specifies the current position of the cursor and the LineNumber specifies the current cursor linenumber.</remarks>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public interface ICursor
    {
        /// <summary>
        /// Gets or sets the index of the cursor.
        /// </summary>
        /// <value>The index of the cursor.</value>
        int CursorIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>The line number.</value>
        int LineNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Moves the cursor pointer to a particular location.
        /// </summary>
        /// <param name="i">The index of cursor position.</param>
        /// <returns>Returns the cursor index</returns>
        int MoveTo(int i);

        /// <summary>
        /// Moves to begin.
        /// </summary>
        /// <returns>Returns the cursor index.</returns>
        int MoveToBegin();

        /// <summary>
        /// Moves to end.
        /// </summary>
        /// <returns>Returns the cursor index.</returns>
        int MoveToEnd();

        /// <summary>
        /// Shows the cursor.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the cursor.
        /// </summary>
        void Hide();
    }
}