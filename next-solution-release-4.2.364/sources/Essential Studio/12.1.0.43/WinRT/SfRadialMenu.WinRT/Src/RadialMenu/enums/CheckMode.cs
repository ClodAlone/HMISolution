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
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
namespace Syncfusion.Windows.Controls.Navigation
#else
namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
    /// <summary>
    /// Creates an enumeration list for Checkmode options for the Radial Menu <see 
    /// cref="Syncfusion.UI.Xaml.Controls.Navigation.SfRadialMenu"/> control.
    /// </summary>
    public enum CheckMode
    {
        /// <summary>
        /// All items can be selected.
        /// </summary>
        None,
        /// <summary>
        /// An item will be unchecked if checked again.
        /// </summary>
        CheckBox,
        /// <summary>
        /// Only one item can be selected at a time.
        /// </summary>
        RadioButton
    }
#endif
}
