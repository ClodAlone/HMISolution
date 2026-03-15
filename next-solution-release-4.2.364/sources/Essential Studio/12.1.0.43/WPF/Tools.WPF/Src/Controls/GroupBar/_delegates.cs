// <copyright file="_delegates.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Represents delegate for the <see cref="OrientationChangeEventHandler"/> routed event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.OrientationChangeEventArgs"/> that contains the event data.</param>
    public delegate void OrientationChangeEventHandler(object sender, OrientationChangeEventArgs e);
    
    /// <summary>
    /// Represents delegate for the <see cref="FlowDirectionChangedEventHandler"/> routed event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.FlowDirectionChangedEventArgs"/> that contains the event data.</param>
    public delegate void FlowDirectionChangedEventHandler(object sender, FlowDirectionChangedEventArgs e);
    
    /// <summary>
    /// Represents delegate for the <see cref="BeforeGroupBarItemPopupOpenedEventHandler"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.BeforeGroupBarItemPopupOpenedEventArgs"/> that contains the event data.</param>
    public delegate void BeforeGroupBarItemPopupOpenedEventHandler(object sender, BeforeGroupBarItemPopupOpenedEventArgs e);

    /// <summary>
    /// Represents delegate for the <see cref="GroupBarContextMenuItemEventHandler"/> routed event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="Syncfusion.Windows.Tools.GroupBarContextMenuItemEventArgs"/> that contains the event data.</param>
    public delegate void GroupBarContextMenuItemEventHandler(object sender, GroupBarContextMenuItemEventArgs e);
}
