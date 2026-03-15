#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT
using System.Windows.Controls;
namespace Syncfusion.Windows.Controls
#else
using System;

namespace Syncfusion.WinRT.Controls
#endif
{
    public interface IScrollableInfo
    {
        void SetHorizontalOffset(double offset);
        void SetVerticalOffset(double offset);

        bool CanHorizontallyScroll { get; set; }

        bool CanVerticallyScroll { get; set; }

        double ExtentHeight { get; }

        double ExtentWidth { get; }

        double HorizontalOffset { get; }

        ScrollableContentViewer ScrollOwner { get; set; }

        double VerticalOffset { get; }

        double ViewportHeight { get; }

        double ViewportWidth { get; }

        double ZoomScale { get; set; }

        void LineLeft();

        void LineRight();

        void LineUp();

        void LineDown();

        void MouseWheelUp();

        void MouseWheelDown();

        void MouseWheelLeft();

        void MouseWheelRight();

        void PageUp();

        void PageDown();

        void PageRight();

        void PageLeft();
    }
}
