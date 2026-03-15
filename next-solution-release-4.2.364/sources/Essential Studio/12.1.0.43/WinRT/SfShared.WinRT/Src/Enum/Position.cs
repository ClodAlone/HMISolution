// <copyright file="Position.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE|| WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Controls
#else
#if WPF
namespace Syncfusion.Windows.Controls
#else
namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    /// Represents an enum list for the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Position"/>
    /// </summary>
    public enum Position
    {
        /// <summary>
        /// Bottom position
        /// </summary>
        Bottom,

        /// <summary>
        /// Center position
        /// </summary>
        Center,

        /// <summary>
        /// Top position
        /// </summary>
        Top,

        /// <summary>
        /// Left position
        /// </summary>
        Left,

        /// <summary>
        /// Right position
        /// </summary>
        Right

    }
}
