// <copyright file="_delegates.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a method that will handle DockStateChanged event.
    /// </summary>
    /// <param name="sender">FrameworkElement where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void DockStateHandler(FrameworkElement sender, DockStateEventArgs e);


    /// <summary>
    /// Represents a method that will handle DockWindowStateChanged event.
    /// </summary>
    public delegate void DockWindowStateHandler(FrameworkElement sender, DockWindowStateEventArgs e);

    /// <summary>
    /// Represents a method that will handle events when target name of element in float or dock state changed.
    /// </summary>
    /// <param name="sender">FrameworkElement where the event handler is attached.</param>
    /// <param name="args">The event data.</param>
    public delegate void DockTargetNameChangedEventHandler(UIElement sender, DockTargetNameChangedEventArgs args);
    
    /// <summary>
    /// Represents a method that will handle ElementHiding event.
    /// </summary>
    /// <param name="sender">FrameworkElement where the event handler is attached.</param>
    public delegate void ElementHiddenEventHandler(object sender);
    
    /// <summary>
    /// Represents a method that will handle ElementShowing event.
    /// </summary>
    /// <param name="sender">FrameworkElement where the event handler is attached.</param>
    public delegate void ElementShownEventHandler(object sender);

    /// <summary>
    /// Represents DockStateChaningHandler that handles before dockstate change
    /// </summary>
    public delegate void DockStateChangingHandler(FrameworkElement sender,DockStateChangingEventArgs e);

    /// <summary>
    /// Represents ActiveWindowChangingHandler that handles before ActiveWindow change
    /// </summary>
    public delegate void ActiveWindowChangingHandler(FrameworkElement sender,ActiveWindowChangingEventArgs e);

    /// <summary>
    /// Represents WindowResizingEventHandler that handles whenever we resize the window 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void WindowResizingEventHandler(object sender,WindowResizingEventArgs e);

    /// <summary>
    /// Represents WindowMovingEventHandler that handles whenever we move the window 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void WindowMovingEventHandler(object sender, WindowMovingEventArgs e);

    /// <summary>
    /// Represents WindowClosingHandler that handles before Closing Docked or Float Window
    /// </summary>
    public delegate void WindowClosingEventHandler(object sender, WindowClosingEventArgs e);

    /// <summary>
    /// Represents TransferManagerEventHandler that handles while transferring Window from one DockingManager to another DockingManager
    /// </summary>
    public delegate void TransferManagerEventHandler(object sender,TransferManagerEventArgs e);

    /// <summary>
    /// Represents DockPreviewOpeningEventHandler before Dock preview is opened.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DockProviderShownEventHandler(object sender,DockProviderShownEventArgs e);

    /// <summary>
    /// Represents TabClosedEventHandler after tab is closed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TabClosedEventHandler(object sender,CloseTabEventArgs e);
}
