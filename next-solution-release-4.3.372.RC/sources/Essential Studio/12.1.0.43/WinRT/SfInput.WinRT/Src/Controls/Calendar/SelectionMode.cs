#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#else
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a list for the selection mode
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// Select single item at a time
        /// </summary>
        Single,

        /// <summary>
        /// Select single item at a time
        /// </summary>
        Multiple,

        /// <summary>
        /// Select more than one item at a time
        /// </summary>
        None
    }
}
