#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT

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
#else
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class Pen
    {
        // Summary:
        //     Initializes a new instance of the System.Windows.Media.Pen class.
        public Pen()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Media.Pen class with the
        //     specified System.Windows.Media.Brush and thickness.
        //
        // Parameters:
        //   brush:
        //     The Brush for this Pen.
        //
        //   thickness:
        //     The thickness of the Pen.
        public Pen(Brush brush, double thickness)
        {
            Brush = brush;
            Thickness = thickness;
            Style = BorderStyle.Standard;
            DashArray = null;
        }

        public Pen(Brush brush, double thickness, BorderStyle style)
        {
            Brush = brush;
            Thickness = thickness;
            Style = style;
            DashArray = null;
        }

        public Pen(Brush brush, double thickness, BorderStyle style, DoubleCollection dashArray)
        {
            Brush = brush;
            Thickness = thickness;
            Style = style;
            DashArray = dashArray;
        }

        public Brush Brush { get; set; }
        public double Thickness { get; set; }
        public BorderStyle Style { get; set; }
        public DoubleCollection DashArray { get; set; }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;
            
            Pen other = obj as Pen;
            if (other == null)
                return false;

            return other.Brush == Brush
                && other.Thickness == Thickness;
        }

        public override int GetHashCode()
        {
            return Brush == null ? 0 : Brush.GetHashCode();
        }
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public enum BorderStyle
    {
        Standard,

        Dotted,

        Dashed,

        DashDot,

        DashDotDot,

        Custom,

        None
    }
}
