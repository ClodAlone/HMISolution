#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT

namespace Syncfusion.Windows.ComponentModel
#else

namespace Syncfusion.WinRT.ComponentModel
#endif
{
    /// <summary>
    /// An interface for the <see cref="Disposed"/> event.
    /// </summary>
    public interface IDisposedEvent
    {
        /// <summary>
        /// Occurs when Dispose was called.
        /// </summary>
        event EventHandler Disposed;
    }

}
