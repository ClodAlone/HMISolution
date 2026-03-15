// <copyright file="_delegates.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Represents delegate for handlers that receive LoadedDispatcherInternalBorder routed event.
    /// </summary>
    internal delegate void DispatcherHandler();

    /// <summary>
    /// Represents delegate for handlers that receive date from async thread.
    /// </summary>
    /// <param name="items">Represents the Items</param>
    internal delegate void CallBackFromAsyncLoad(Syncfusion.Windows.Tools.Controls.AutocompleteItemCollection items);
}