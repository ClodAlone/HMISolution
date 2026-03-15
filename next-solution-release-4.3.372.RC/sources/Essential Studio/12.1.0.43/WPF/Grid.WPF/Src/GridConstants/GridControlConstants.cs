#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Windows.Controls.Grid
{
    public static class GridControlConstants
    {
        private static Color c = SystemColors.HighlightColor;
        public static SolidColorBrush HighlightSelectionAlphaBlend = new SolidColorBrush(Color.FromArgb(96, c.R, c.G, c.B));
        public static SolidColorBrush HighlightSelectionBorder = Brushes.Black;
#if SILVERLIGHT
        public static SolidColorBrush HighlightSelectionBackground = new SolidColorBrush(SystemColors.HighlightColor);
        public static SolidColorBrush HighlightSelectionForeground = new SolidColorBrush(SystemColors.HighlightTextColor);
#else
        public static SolidColorBrush HighlightSelectionBackground = SystemColors.HighlightBrush;
        public static SolidColorBrush HighlightSelectionForeground = SystemColors.HighlightTextBrush;
#endif
        public static double HighlightSelectionBorderWidth = 2;
        public static SolidColorBrush CurrentCellBorder = Brushes.Black;
        public static double CurrentCellBorderWidth = 1;

        public static double HiddenBorderThickness = 2;
        public static Brush HiddenBorderBrush = Brushes.Black;
    }
}
