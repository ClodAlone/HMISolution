// <copyright file="_delegates.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the delegate for Drag TreeViewItemAdv Handler
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.Controls.DragTreeViewItemAdvEventArgs"/> that contains the event data.</param>
    public delegate void DragTreeViewItemAdvHandler(object sender, DragTreeViewItemAdvEventArgs e);

    /// <summary>
    /// Represents the delegate for Edit Mode Change Handler
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.Controls.EditModeChangeEventArgs"/> that contains the event data.</param>
    public delegate void EditModeChangeHandler(object sender, EditModeChangeEventArgs e);

    /// <summary>
    /// Represents the delegate for Sort Mode Change Handler
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.Controls.SortModeChangeEventArgs"/> that contains the event data.</param>
    public delegate void SortModeChangeHandler(object sender, SortModeChangeEventArgs e);

    /// <summary>
    /// Represents the delegate for Expanding/Collapsing Handler
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.Controls.ExpandCollapseEventArgs"/> that contains the event data.</param>
    public delegate void ExpandingCollapsingHandler(object sender, ExpandingCollapsingEventArgs e);

    /// <summary>
    /// Represents the delegate for Expanded/Collapsed Handler
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.Controls.ExpandCollapseEventArgs"/> that contains the event data.</param>
    public delegate void ExpandedCollapsedHandler(object sender, ExpandedCollapsedEventArgs e);

    public delegate void LoadOnDemandEventHandler(object sender, LoadonDemandEventArgs args);
}