#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows.Media;

namespace Syncfusion.Windows.GridCommon
{
    /// <summary>
    /// Provides some helper routines related to brushes and colors.
    /// </summary>
    public class ColorHelper
    {
        /// <summary>
        /// Creates the frozen solid color brush from an underlying color and alpha value.
        /// </summary>
        /// <param name="alpha">The alpha value.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static SolidColorBrush CreateFrozenSolidColorBrush(byte alpha, Color color)
        {
            SolidColorBrush br = new SolidColorBrush();
            color.A = alpha;
            br.Color = color;
            br.Freeze();
            return br;
        }
    }


}
