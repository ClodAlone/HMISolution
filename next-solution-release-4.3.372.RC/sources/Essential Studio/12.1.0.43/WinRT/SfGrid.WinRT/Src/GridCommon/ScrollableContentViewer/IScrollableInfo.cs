#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#else
using System.Windows;
using System.Windows.Controls.Primitives;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
#if WinRT
    public interface IScrollableInfo
#else
    public interface IScrollableInfo : IScrollInfo
#endif
    {
#if WinRT
        void LineDown();
        void LineLeft();
        void LineRight();
        void LineUp();
        Rect MakeVisible(UIElement visual, Rect rectangle);
        void MouseWheelDown();
        void MouseWheelLeft();
        void MouseWheelRight();
        void MouseWheelUp();
        void PageDown();
        void PageLeft();
        void PageRight();
        void PageUp();
        void SetHorizontalOffset(double offset);
        void SetVerticalOffset(double offset);
        bool CanHorizontallyScroll { get; set; }
        bool CanVerticallyScroll { get; set; }
        double ExtentHeight { get; }
        double ExtentWidth { get; }
        double HorizontalOffset { get; }
        double VerticalOffset { get; }
        double ViewportHeight { get; }
        double ViewportWidth { get; }
#endif
        ScrollableContentViewer ScrollableOwner { get; set; }
        double VerticalPadding { get; set; }
        double HorizontalPadding { get; set; }
    }
}
