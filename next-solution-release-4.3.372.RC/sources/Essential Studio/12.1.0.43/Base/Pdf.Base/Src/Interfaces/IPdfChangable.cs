#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.Pdf
{
    /// <summary>
    ///  Interface of the objects that support Changable of their internals.
    /// </summary>
    internal interface IPdfChangable
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IPdfChangable"/> is changed.
        /// </summary>
        /// <value><c>true</c> if changed; otherwise, <c>false</c>.</value>
        bool Changed { get; }

        /// <summary>
        /// Freezes the changes.
        /// </summary>
        /// <param name="freezer">The freezer.</param>
        void FreezeChanges(object freezer);
    }
}
