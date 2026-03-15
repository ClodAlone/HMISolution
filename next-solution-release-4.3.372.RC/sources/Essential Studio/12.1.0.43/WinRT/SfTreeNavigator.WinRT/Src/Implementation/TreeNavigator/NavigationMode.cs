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
#if !WINDOWS_PHONE_7 && !Silverlight4
using System.Threading.Tasks;
#endif
#if SILVERLIGHT
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
namespace Syncfusion.Windows.Controls.Navigation
#else
namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Defines a enum list for the mode of navigation of the child items
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>
        /// On clicking parent item the child items are displayed.
        /// </summary>
        Default,
        /// <summary>
        /// On clicking parent item the child items are displayed with the parent item as header.
        /// </summary>
        Extended
    }
}
