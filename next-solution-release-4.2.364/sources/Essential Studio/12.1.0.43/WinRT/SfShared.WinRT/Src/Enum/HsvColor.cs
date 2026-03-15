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
#if WINDOWS_PHONE || WINDOWS_PHONE_7
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
    /// Represents a structure for the HSV color values
    /// </summary>
    public struct HsvColor
    {
        /// <summary>
        /// The Hue in 0..360 range.
        /// </summary>
        public double H;
        /// <summary>
        /// The Saturation in 0..1 range.
        /// </summary>
        public double S;
        /// <summary>
        /// The Value in 0..1 range.
        /// </summary>
        public double V;
        /// <summary>
        /// The Alpha/opacity in 0..1 range.
        /// </summary>
        public double A;
    }
}
