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
    /// Represents the delegate Splitter page selection changed event handler.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.Controls.SplitterPagesSelectionChangedEventArgs"/> that contains the event data.</param>
    public delegate void SplitterPagesSelectionChangedEventHandler(object sender, SplitterPagesSelectionChangedEventArgs e);

    /// <summary>
    /// Represents the delegate Selected Page Changed Handler.
    /// </summary>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.Controls.SplitterPagesSelectionChangedEventArgs"/> that contains the event data.</param>
    public delegate void SelectedPageChangedHandler(SplitterPagesSelectionChangedEventArgs e);
}