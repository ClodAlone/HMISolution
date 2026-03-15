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

#if WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Notification
#elif SILVERLIGHT
namespace Syncfusion.Tools.Controls.Notification
#elif WPF
namespace Syncfusion.Windows.Controls.Notification
#else
namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
{
    /// <summary>
    /// Represents a list of animation types that allows the user to select and apply for the
    /// control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public enum AnimationTypes
    {
        /// <summary>
        /// Represents a flower type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Flower,

        /// <summary>
        /// Represents a Gear type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Gear,

        /// <summary>
        /// Represents a Liquid type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Liquid,

        /// <summary>
        /// Represents a Box type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Box,

        /// <summary>
        /// Represents a HorizontalPulsingBox type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        HorizontalPulsingBox,
#if !(WINDOWS_PHONE||SILVERLIGHT)
        /// <summary>
        /// Represents a Rotation type animation that allows the user to select and apply for the
        /// <see cref="Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> control.
        /// </summary>
        Rotation,
#endif
        /// <summary>
        /// Represents a SliceBox type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        SliceBox,

        /// <summary>
        /// Represents a DoubleCircle type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        DoubleCircle,

#if WINDOWS_PHONE || WPF || WINRT
        /// <summary>
        /// Represents a Drop type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Drop,

        /// <summary>
        /// Represents a Ball type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Ball,

        /// <summary>
        /// Represents a Delete type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Delete,

        /// <summary>
        /// Represents a Sunny type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Sunny,

        /// <summary>
        /// Represents a ECG type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        ECG,
#endif

        /// <summary>
        /// Represents a GPS type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        GPS,

        /// <summary>
        /// Represents a Pen type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Pen,

        /// <summary>
        /// Represents a Globe type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Globe,

        /// <summary>
        /// Represents a Print type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Print,

        /// <summary>
        /// Represents a Rectangle type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Rectangle,

        /// <summary>
        /// Represents a ArrowTrack type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        ArrowTrack,

        /// <summary>
        /// Represents a Temperature type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Temperature,

        /// <summary>
        /// Represents a Umbrella type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Umbrella,

        /// <summary>
        /// Represents a Battery type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Battery,

        /// <summary>
        /// Represents a Windmill type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Windmill,

        /// <summary>
        /// Represents a Rainy type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Rainy,

        /// <summary>
        /// Represents a Snow type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Snow,

        /// <summary>
        /// Represents a Flight type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Flight,

#if !WPF
        /// <summary>
        /// Represents a Bulb type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Bulb,

        /// <summary>
        /// Represents a Sunrise type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Sunrise,

        /// <summary>
        /// Represents a Thunder type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        Thunder,
#endif

        /// <summary>
        /// Represents a SingleCircle type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        SingleCircle,

        /// <summary>
        /// Represents a SlicedCircle type animation that allows the user to select and apply for the
        /// control.
        /// </summary>
        SlicedCircle,

    }
}
