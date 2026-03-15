#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.GridCommon
#else
using Syncfusion.WinRT.ComponentModel;

namespace Syncfusion.WinRT.GridCommon
#endif
{
    /// <summary>
    /// Handles the <see cref="ExceptionManager.ExceptionCatched"/> event.
    /// </summary>
    public delegate void ExceptionCatchedEventHandler(object sender, ExceptionCatchedEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="ExceptionManager.ExceptionCatched"/> event.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class ExceptionCatchedEventArgs : SyncfusionEventArgs
    {
        /// <summary></summary>
        private Exception ex;

        /// <summary>
        /// Constructs a <see cref="ExceptionCatchedEventArgs"/> object.
        /// </summary>
        /// <param name="ex">The exception that was cached.</param>
        public ExceptionCatchedEventArgs(Exception ex)
        {
            this.ex = ex;
        }

        /// <summary>
        /// Returns the exception that was cached.
        /// </summary>
        public Exception Exception
        {
            get
            {
                return ex;
            }
        }
    }

}
