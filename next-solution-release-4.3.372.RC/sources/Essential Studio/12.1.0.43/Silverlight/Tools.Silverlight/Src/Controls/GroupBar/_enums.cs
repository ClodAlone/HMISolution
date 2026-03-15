#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Specifies drag directions.
    /// </summary>
    public enum DragDirection
    {
        /// <summary>
        /// Up drag direction.
        /// </summary>
        Up = 0,

        /// <summary>
        /// Down drag direction.
        /// </summary>
        Down,

        /// <summary>
        /// There is no drag direction.
        /// </summary>
        None
    }

    /// <summary>
    /// Specifies visual modes of <see cref="Syncfusion.Windows.Tools.Controls.GroupBar"/> control.
    /// </summary>
    public enum VisualMode
    {
        /// <summary>
        /// Default visual mode. Only one item can be expanded. 
        /// </summary>
        SingleExpansion,

        /// <summary>
        /// Multiple expansion visual mode. More than one item can be expanded.
        /// </summary>
        MultipleExpansion,

        /// <summary>
        /// Stack visual mode. Only one item can be expanded. Items are organized in stack-like mode.
        /// </summary>
        StackMode
    }
}
