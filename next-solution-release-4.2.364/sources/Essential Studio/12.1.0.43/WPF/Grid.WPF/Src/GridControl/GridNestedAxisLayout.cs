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
    /// Defines the ways of nesting a grid inside another grid.
    /// </summary>
    public enum GridNestedAxisLayout
    {
        /// <summary>
        /// Maintain own sizes, independent of parent grid.
        /// </summary>
        Normal,
        /// <summary>
        /// Share row or column sizes with parent grid.
        /// </summary>
        Shared,
        /// <summary>
        /// Rows and columns are nested inside a single row or column in parent grid.
        /// </summary>
        Nested
    }
}
