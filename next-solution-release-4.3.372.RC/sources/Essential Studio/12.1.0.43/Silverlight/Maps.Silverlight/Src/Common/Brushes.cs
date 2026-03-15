#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// <para></para>
    /// </summary>
    internal static class ColorExtensions
    {
        public static Color StringToColor(string hexaColor)
        {
            var color = Color.FromArgb(Convert.ToByte(hexaColor.Substring(1, 2), 16), Convert.ToByte(hexaColor.Substring(3, 2), 16), Convert.ToByte(hexaColor.Substring(5, 2), 16), Convert.ToByte(hexaColor.Substring(7, 2), 16));
            return color;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class Brushes
    {
        public static SolidColorBrush Black
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// Gets the white.
        /// </summary>
        /// <value>The white.</value>
        public static SolidColorBrush White
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// Gets the white smoke.
        /// </summary>
        /// <value>The white smoke.</value>
        public static SolidColorBrush WhiteSmoke
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF5F5F5"));
            }
        }

        /// <summary>
        /// Gets the yellow.
        /// </summary>
        /// <value>The yellow.</value>
        public static SolidColorBrush Yellow
        {
            get
            {
                return new SolidColorBrush(Colors.Yellow);
            }
        }

        /// <summary>
        /// Gets the maroon.
        /// </summary>
        /// <value>The maroon.</value>
        public static SolidColorBrush Maroon
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF800000"));
            }
        }

        /// <summary>
        /// Gets the midnight blue.
        /// </summary>
        /// <value>The midnight blue.</value>
        public static SolidColorBrush MidnightBlue
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FF191970"));
            }
        }

        /// <summary>
        /// Gets the alice blue.
        /// </summary>
        /// <value>The alice blue.</value>
        public static SolidColorBrush AliceBlue
        {
            get
            {
                return new SolidColorBrush(ColorExtensions.StringToColor("#FFF0F8FF"));
            }
        }

        /// <summary>
        /// Gets the gray.
        /// </summary>
        /// <value>The gray.</value>
        public static SolidColorBrush Gray
        {
            get
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        /// <summary>
        /// Gets the transparent.
        /// </summary>
        /// <value>The transparent.</value>
        public static SolidColorBrush Transparent
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// Gets the red.
        /// </summary>
        /// <value>The red.</value>
        public static SolidColorBrush Red
        {
            get
            {
                return new SolidColorBrush(Colors.Red);
            }
        }
    }
}
