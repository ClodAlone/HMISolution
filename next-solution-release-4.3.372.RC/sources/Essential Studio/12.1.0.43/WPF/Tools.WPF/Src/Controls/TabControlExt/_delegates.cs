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
    /// Delegate for OnCloseTabsEvent.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Contains the Event data.</param>
    public delegate void OnCloseTabsEventHandler(object sender, CloseTabEventArgs e);

    /// <summary>
    /// Delegate for BeforeLabelEdit.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Contains the Event data.</param>
    public delegate void BeforeLabelEditHandler(object sender, BeforeLabelEditEventArgs e);

    /// <summary>
    /// Delegate for AfterLabelEdit.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Contains the Event data.</param>
    public delegate void AfterLabelEditHandler(object sender, AfterLabelEditEventArgs e);

    /// <summary>
    /// Delegate for SelectedItemChangedEvent.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Contains the Event data.</param>
    public delegate void SelectedItemChangedEventHandler(object sender, SelectedItemChangedEventArgs e);

    public delegate void PreviewSelectedItemChangedEventHandler(object sender, PreviewSelectedItemChangedEventArgs e);
    /// <summary>
    /// Presents handler for take drag item event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="arg">Contains the Event data.</param>
    public delegate void TakeDragItemHandler(object sender, TakeDragItemEventArgs arg);
}
