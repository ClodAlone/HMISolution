// <copyright file="SlideDirection.cs" company="Syncfusion">
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

#if WINDOWS_PHONE || WINDOWS_PHONE_7
namespace Syncfusion.WP.Primitives
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Primitives
#else
#if WPF
namespace Syncfusion.Windows.Primitives
#else
namespace Syncfusion.UI.Xaml.Primitives
#endif
#endif
#endif
{
    /// <summary>
    /// Creates an enum list for the Slide Direction
    /// </summary>
    public enum SlideDirection
    {
        /// <summary>
        /// Up Direction
        /// </summary>
        Up,

        /// <summary>
        /// Down Direction
        /// </summary>
        Down,

        /// <summary>
        /// Up Direction
        /// </summary>
        Left,

        /// <summary>
        /// Down Direction
        /// </summary>
        Right,

        /// <summary>
        /// Default Direction
        /// </summary>
        Default
    }

    /// <summary>
    /// Creates an enum list for the precision
    /// </summary>
    public enum Precision
    {
        /// <summary>
        /// Standard Precision
        /// </summary>
        Standard,

        /// <summary>
        /// Half Precision
        /// </summary>
        Half,

        /// <summary>
        /// Exact Precision
        /// </summary>
        Exact
    }
}
