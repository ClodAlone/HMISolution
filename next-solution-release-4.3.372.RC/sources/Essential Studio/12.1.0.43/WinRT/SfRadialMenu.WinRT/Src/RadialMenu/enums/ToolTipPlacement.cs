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
    /// Creates an enumeration list for the position of ToolTipPlacement
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfColorPicker"/>
    public enum ToolTipPlacement
    {
        /// <summary>
        /// Sets none for the ToolTipPlacement. It takes the default value.
        /// </summary>
        None,
        /// <summary>
        /// The tooltip is placed on the top.
        /// </summary>
        Top,
        /// <summary>
        /// The tooltip is placed on the left.
        /// </summary>
        Left,
        /// <summary>
        /// The tooltip is placed on the bottom.
        /// </summary>
        Bottom,
        /// <summary>
        /// The tooltip is placed on the right.
        /// </summary>
        Right
    }
#endif
}
