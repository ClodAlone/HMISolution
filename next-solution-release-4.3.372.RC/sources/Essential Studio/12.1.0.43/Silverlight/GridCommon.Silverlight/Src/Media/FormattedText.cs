#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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

namespace Syncfusion.Windows
{
    /// <summary>
    /// Helper class for storing TextInformation
    /// </summary>
    public class FormattedText
    {
        public string Text { get; set; }
        public double FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public FontStretch FontStretch { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }
        public Brush Foreground { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Thickness Margin { get; set; }
        public Thickness Padding { get; set; }
        public TextDecorationCollection TextDecorations { get; set; }
        public TextWrapping TextWrapping { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public int FontOrientation { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public TextTrimming TextTrimming { get; set; }

        public double Height { get; set; }
        public double Width { get; set; }
        public double LineHeight { get; set; }
        public int MaxLineCount { get; set; }
        public double MaxTextHeight { get; set; }
        public double MaxTextWidth { get; set; }
        public double MinWidth { get; set; }

        // Converts a HorizontalAlignment enum to a TextAlignment enum.
        internal static TextAlignment HorizontalAlignmentToTextAlignment(HorizontalAlignment horizontalAlignment)
        {
            TextAlignment textAlignment;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                default:
                    textAlignment = TextAlignment.Left;
                    break;

                case HorizontalAlignment.Right:
                    textAlignment = TextAlignment.Right;
                    break;

                case HorizontalAlignment.Center:
                    textAlignment = TextAlignment.Center;
                    break;

                case HorizontalAlignment.Stretch:
                    textAlignment = TextAlignment.Justify;
                    break;
            }

            return textAlignment;
        }
    }
}
