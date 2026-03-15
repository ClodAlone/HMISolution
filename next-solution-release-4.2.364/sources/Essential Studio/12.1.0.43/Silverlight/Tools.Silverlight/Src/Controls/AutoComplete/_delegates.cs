#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools
{
    using System;

    /// <summary>
    /// Represents delegate for handlers that receive LoadedDispatcherInternalBorder routed event.
    /// </summary>
    internal delegate void DispatcherHandler();

    /// <summary>
    /// Represents delegate for handlers that receive date from async thread.
    /// </summary>
    /// <param name="items">gets this control collection and maintains in items.</param>
    internal delegate void CallBackFromAsyncLoad(Syncfusion.Windows.Tools.Controls.AutocompleteItemCollection items);
}