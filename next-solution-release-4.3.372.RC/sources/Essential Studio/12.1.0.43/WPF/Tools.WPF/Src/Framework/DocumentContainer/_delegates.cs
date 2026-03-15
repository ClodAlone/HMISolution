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
    /// Represents a method that will handle CloseButtonClick event.
    /// </summary>
    /// <param name="sender">FrameworkElement where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void CloseButtonEventHandler(object sender, CloseButtonEventArgs e);
    
    /// <summary>
    /// Represents a method that will handle canceling event.
    /// </summary>
    /// <param name="sender">FrameworkElement where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void CancelingRoutedEventHandler(object sender, CancelingRoutedEventArgs e);

    
    /// <summary>
    /// Represents IsSelectedChangedHandler that handles when IsSelected property changed for Document
    /// </summary>
    public delegate void IsSelectedChangedHandler(FrameworkElement sender,IsSelectedChangedEventArgs e);

    /// <summary>
    /// Represents TabGroupEventHandler when TabGroup is created.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TabGroupEventHandler(object sender, TabGroupEventArgs e);
}