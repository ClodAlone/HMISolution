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
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Shared
{
    internal static class Extenstion
    {
        internal static void SetSafeHorizontalOffset(this ScrollViewer sv, double offset)
        {
            if (sv.Content != null && sv.Content is IScrollInfo)
            {
                (sv.Content as IScrollInfo).SetHorizontalOffset(offset);
            }
            else
            {
                sv.ScrollToHorizontalOffset(offset);
                //sv.SetValue(ScrollViewer.HorizontalOffsetProperty, offset);
                //sv.InvalidateScrollInfo();
                //sv.ScrollToHorizontalOffset(offset);
            }
        }

        internal static void SetSafeVerticalOffset(this ScrollViewer sv, double offset)
        {
            if (sv.Content != null && sv.Content is IScrollInfo)
            {
                (sv.Content as IScrollInfo).SetVerticalOffset(offset);
            }
            else
            {
                sv.ScrollToVerticalOffset(offset);
                //sv.SetValue(ScrollViewer.VerticalOffsetProperty, offset);
                //sv.InvalidateScrollInfo();
                //sv.ScrollToHorizontalOffset(offset);
            }
        }
    }
}
