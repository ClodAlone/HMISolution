#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Syncfusion.UI.Xaml.Grid
{

    public sealed class PrintPreviewPanel : Panel, IScrollInfo
    {

        #region Fields

        readonly Size InfiniteSize =
      new Size(double.PositiveInfinity, double.PositiveInfinity);
        PrintManagerBase printBase;
        double yPosition;
        double xPosition;
        int previousPageIndex;
        internal Action<int> SetPageIndex;
        internal Action InValidateParent;
        
        #endregion

        #region Ctor

        public PrintPreviewPanel()
        {

        }

        public PrintPreviewPanel(PrintManagerBase printBase)
        {
            SetPrintManagerBase(printBase);
        }

        #endregion 

        #region Child Property

        internal PrintPageControl Child { get; set; }

        #endregion

        #region Override

        protected override Size MeasureOverride(Size availableSize)
        {

            Child.Measure(InfiniteSize);
            var elementSize = Child.DesiredSize;

            var size = new Size
                {
                    Width = double.IsInfinity(availableSize.Width) ? elementSize.Width : availableSize.Width,
                    Height = double.IsInfinity(availableSize.Height) ? elementSize.Height : availableSize.Height
                };

            UpdateScrollInfo(size, new Size(Child.DesiredSize.Width, Child.DesiredSize.Height*printBase.pageCount));

            return base.MeasureOverride(size);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var xpos = (finalSize.Width/2 - Child.DesiredSize.Width/2);
            var yPos = (VerticalOffset -
                        ((int)(VerticalOffset/Child.DesiredSize.Height)*Child.DesiredSize.Height));

            xPosition = xpos < 0 ? -HorizontalOffset : xpos;
            if (Child.DesiredSize.Height > finalSize.Height)
            {
                var pageIndex = ((VerticalOffset + finalSize.Height)%Child.DesiredSize.Height) > 0 ? 1 : 0;
                pageIndex = (int) ((VerticalOffset + finalSize.Height)/Child.DesiredSize.Height) + pageIndex;
                if (previousPageIndex != pageIndex && pageIndex >= 1 && pageIndex <= printBase.pageCount)
                {
                    printBase.CreatePage((int)pageIndex, Child);
                    previousPageIndex = (int)pageIndex;
                }
            }
            else
            {
                var pageIndex = ((int)((VerticalOffset + finalSize.Height) / Child.DesiredSize.Height));
                if (pageIndex > 0 && previousPageIndex != pageIndex && pageIndex <= printBase.pageCount)
                {
                    printBase.CreatePage(pageIndex, Child);
                    previousPageIndex = pageIndex;
                }
                else if (Child.PageIndex != 1 && VerticalOffset == 0)
                {
                    printBase.CreatePage(1, Child);
                    previousPageIndex = pageIndex;
                }
            }

            yPosition = ((VerticalOffset -
                          ((int) (VerticalOffset/Child.DesiredSize.Height)*Child.DesiredSize.Height)) +
                         finalSize.Height) > Child.DesiredSize.Height
                ? finalSize.Height > Child.DesiredSize.Height
                    ? finalSize.Height/2 - Child.DesiredSize.Height/2
                    : 0
                : -yPos;
            Child.Arrange(new Rect(xPosition, yPosition, Child.DesiredSize.Width,
                                   Child.DesiredSize.Height));
            UpdateScrollInfo(finalSize,
                             new Size(Child.DesiredSize.Width,
                                      Child.DesiredSize.Height * printBase.pageCount));

            if (SetPageIndex != null) SetPageIndex(Child.PageIndex);

            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #region Internal Methods

        internal void SetPrintManagerBase(PrintManagerBase printBase)
        {
            if(printBase == null)
                return;
            this.printBase = printBase;
            printBase.InValidate = InValidate;
            printBase.InitializePrint(true);
            Child = printBase.CreatePage(1);
            Children.Add(Child);
        }

        #endregion

        #region Private Methods

        private void UpdateScrollInfo(Size viewport, Size extent)
        {
            if (double.IsInfinity(viewport.Width))
            { viewport.Width = extent.Width; }

            if (double.IsInfinity(viewport.Height))
            { viewport.Height = extent.Height; }

            _Extent = extent;
            _Viewport = viewport;

            _Offset.X = Math.Max(0,
              Math.Min(_Offset.X, ExtentWidth - ViewportWidth));
            _Offset.Y = Math.Max(0,
              Math.Min(_Offset.Y, ExtentHeight - ViewportHeight));

            if (ScrollOwner != null)
            { ScrollOwner.InvalidateScrollInfo(); }
        }


        public void InValidate(bool needToInitProperties)
        {
            printBase.InitializePrint(needToInitProperties);
            if (printBase.pageCount <= 0) return;
            Children.Clear();
            Child = printBase.CreatePage(1, Child);
            Children.Add(Child);
            if (InValidateParent != null) InValidateParent();
        }

        #endregion

        #region IScrollInfo Members

        #region Fields

        private const double LineSize = 16;
        private const double WheelSize = 3 * LineSize;
        private bool _CanHorizontallyScroll;
        private bool _CanVerticallyScroll;
        private ScrollViewer _ScrollOwner;
        private Point _Offset;
        private Size _Extent;
        private Size _Viewport;

        #endregion

        #region Properties

        public bool CanHorizontallyScroll
        {
            get { return _CanHorizontallyScroll; }
            set { _CanHorizontallyScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return _CanVerticallyScroll; }
            set { _CanVerticallyScroll = value; }
        }

        public double ExtentHeight
        {
            get { return _Extent.Height; }
        }

        public double ExtentWidth
        {
            get { return _Extent.Width; }
        }

        public double HorizontalOffset
        {
            get { return _Offset.X; }
        }

        public double VerticalOffset
        {
            get { return _Offset.Y; }
        }

        public double ViewportHeight
        {
            get { return _Viewport.Height; }
        }

        public double ViewportWidth
        {
            get { return _Viewport.Width; }
        }

        public ScrollViewer ScrollOwner
        {
            get { return _ScrollOwner; }
            set { _ScrollOwner = value; }
        }

        #endregion

        #region Methods

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + LineSize);
        }

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - LineSize);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LineSize);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + LineSize);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + WheelSize);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - WheelSize);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - WheelSize);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + WheelSize);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

#if WPF
        public Rect MakeVisible(Visual visual, Rect rectangle)
#elif SILVERLIGHT
        public Rect MakeVisible(UIElement uiElement, Rect rectangle)
#endif
        {
            return rectangle;
        }


        public void SetHorizontalOffset(double offset)
        {
            offset = Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
            if (offset != _Offset.Y)
            {
                _Offset.X = offset;
                InvalidateArrange();
            }
        }

        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
            if (offset != _Offset.Y)
            {
                _Offset.Y = offset;
                InvalidateArrange();
            }
        }

        #endregion

        #endregion

        

    }
    
}

