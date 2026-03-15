#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
using System;

namespace Syncfusion.UI.Xaml.Collections.ComponentModel
{
    /// <summary>
    /// An interface for the <see cref="Disposed"/> event.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface IDisposedEvent
    {
        /// <summary>
        /// Occurs when Dispose was called.
        /// </summary>
        event EventHandler Disposed;
    }
}
