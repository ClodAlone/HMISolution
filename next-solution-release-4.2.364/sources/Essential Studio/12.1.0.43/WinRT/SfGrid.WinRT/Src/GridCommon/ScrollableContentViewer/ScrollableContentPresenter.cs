#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Input;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.UI.Xaml.Grid;
using System;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class ScrollableContentPresenter : ContentPresenter,IScrollableInfo
    {
        #region Properties
        private RectangleGeometry _clippingRectangle;
        private bool _isClipPropertySet = false;
        private ScrollData _scrollData = new ScrollData();
        private IScrollableInfo _scrollInfo;

#if WPF
        public new DependencyObject TemplatedParent
#else
        public DependencyObject TemplatedParent
#endif
        {
            get { return (DependencyObject)GetValue(TemplatedParentProperty); }
            set { SetValue(TemplatedParentProperty, value); }
        }

        public static readonly DependencyProperty TemplatedParentProperty =
            DependencyProperty.Register("TemplatedParent", typeof(DependencyObject), typeof(ScrollableContentPresenter), new PropertyMetadata(null));

        public double VerticalPadding { get; set; }

        public double HorizontalPadding { get; set; }

        public bool CanHorizontallyScroll
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return false;
                }
                return this._scrollData._canHorizontallyScroll;
            }
            set
            {
                if (this.IsScrollClient && (this._scrollData._canHorizontallyScroll != value))
                {
                    this._scrollData._canHorizontallyScroll = value;
                    base.InvalidateMeasure();
                }
            }
        }

#if WPF
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
        }
#endif

        public bool CanVerticallyScroll
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return false;
                }
                return this._scrollData._canVerticallyScroll;
            }
            set
            {
                if (this.IsScrollClient && (this._scrollData._canVerticallyScroll != value))
                {
                    this._scrollData._canVerticallyScroll = value;
                    base.InvalidateMeasure();
                }
            }
        }

        public double ExtentHeight
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this._scrollData._extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this._scrollData._extent.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this._scrollData._computedOffset.X;
            }
        }

        private bool IsScrollClient
        {
            get
            {
                return (this._scrollInfo == this);
            }
        }

        public ScrollableContentViewer ScrollableOwner
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return null;
                }
                return this._scrollData._scrollOwner;
            }
            set
            {
                if (this.IsScrollClient)
                {
                    this._scrollData._scrollOwner = value;
                }
            }
        }

        public double VerticalOffset
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this._scrollData._computedOffset.Y;
            }
        }
#if !WinRT
        private ScrollViewer _scrollOwner;
        ScrollViewer IScrollInfo.ScrollOwner
        {
            get { return _scrollOwner; }
            set { _scrollOwner = value; }
        }
#endif

        public double ViewportHeight
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this._scrollData._viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                if (!this.IsScrollClient)
                {
                    return 0.0;
                }
                return this._scrollData._viewport.Width;
            }
        }
        #endregion

        #region Ctor
        public ScrollableContentPresenter()
        {
#if WinRT
            this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY| ManipulationModes.TranslateRailsX| ManipulationModes.TranslateRailsY | ManipulationModes.TranslateInertia;
#endif
        }
        #endregion

        #region public methods

        public void LineDown()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset + 16.0);
            }
        }

        public void LineLeft()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset - 16.0);
            }
        }

        public void LineRight()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset + 16.0);
            }
        }

        public void LineUp()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset - 16.0);
            }
        }

        public void PageDown()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
            }
        }

        public void PageLeft()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
            }
        }

        public void PageRight()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
            }
        }

        public void PageUp()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
            }
        }

        public void MouseWheelDown()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset + 48.0);
            }
        }

        public void MouseWheelLeft()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset - 48.0);
            }
        }

        public void MouseWheelRight()
        {
            if (this.IsScrollClient)
            {
                this.SetHorizontalOffset(this.HorizontalOffset + 48.0);
            }
        }

        public void MouseWheelUp()
        {
            if (this.IsScrollClient)
            {
                this.SetVerticalOffset(this.VerticalOffset - 48.0);
            }
        }

        public void SetHorizontalOffset(double offset)
        {
            if (this.CanHorizontallyScroll)
            {
                double num = ValidateInputOffset(offset);
                if (!DoubleUtil.AreClose(this._scrollData._offset.X, num))
                {
                    this._scrollData._offset.X = num;
                    base.InvalidateArrange();
                }
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (this.CanVerticallyScroll)
            {
                double num = ValidateInputOffset(offset);
                if (!DoubleUtil.AreClose(this._scrollData._offset.Y, num))
                {
                    this._scrollData._offset.Y = num;
                    base.InvalidateArrange();
                }
            }
        }

        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            //if ((rectangle.IsEmpty || (visual == null)) || (visual == this))
            //{
            //    return Rect.Empty;
            //}
            //Point point = visual.TransformToVisual(this).TransformPoint(new Point(rectangle.X, rectangle.Y));
            //rectangle.X = point.X;
            //rectangle.Y = point.Y;
            //if (this.IsScrollClient)
            //{
            //    var rect = new Rect(this.HorizontalOffset, this.VerticalOffset, this.ViewportWidth, this.ViewportHeight);
            //    rectangle.X += rect.X;
            //    rectangle.Y += rect.Y;
            //    double offset = ComputeScrollOffsetWithMinimalScroll(rect.Left, rect.Right, rectangle.Left, rectangle.Right);
            //    double num2 = ComputeScrollOffsetWithMinimalScroll(rect.Top, rect.Bottom, rectangle.Top, rectangle.Bottom);
            //    this.SetHorizontalOffset(offset);
            //    this.SetVerticalOffset(num2);
            //    rect.X = offset;
            //    rect.Y = num2;
            //    rectangle.Intersect(rect);
            //    if (!rectangle.IsEmpty)
            //    {
            //        rectangle.X -= rect.X;
            //        rectangle.Y -= rect.Y;
            //    }
            //}
            return rectangle;
        }

        #endregion

        #region internal methods

        internal static double CoerceOffset(double offset, double extent, double viewport)
        {
            if (offset > (extent - viewport))
            {
                offset = extent - viewport;
            }
            if (offset < 0.0)
            {
                offset = 0.0;
            }
            return offset;
        }

        internal static double ComputeScrollOffsetWithMinimalScroll(double topView, double bottomView, double topChild, double bottomChild)
        {
            bool flag = DoubleUtil.LessThan(topChild, topView) && DoubleUtil.LessThan(bottomChild, bottomView);
            bool flag2 = DoubleUtil.GreaterThan(bottomChild, bottomView) && DoubleUtil.GreaterThan(topChild, topView);
            bool flag3 = (bottomChild - topChild) > (bottomView - topView);
            if (flag && !flag3)
            {
                return topChild;
            }
            if (flag2 && flag3)
            {
                return topChild;
            }
            if (!flag && !flag2)
            {
                return topView;
            }
            return (bottomChild - (bottomView - topView));
        }

        internal static double ValidateInputOffset(double offset)
        {
            if (double.IsNaN(offset))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            return Math.Max(0.0, offset);
        }
        #endregion

        #region protected methods

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.TemplatedParent != null)
            {
                this.UpdateClip(finalSize);
            }
            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            if (this.IsScrollClient)
            {
                this.VerifyScrollData(finalSize, this._scrollData._extent);
            }
            if (element != null)
            {
                var finalRect = new Rect(0.0, 0.0, element.DesiredSize.Width, element.DesiredSize.Height);
                if (this.IsScrollClient)
                {
                    finalRect.X = -this._scrollData._computedOffset.X;
                    finalRect.Y = -this._scrollData._computedOffset.Y;
                }
                finalRect.Width = Math.Max(finalRect.Width, finalSize.Width);
                finalRect.Height = Math.Max(finalRect.Height, finalSize.Height);
                element.Arrange(finalRect);
            }
            return finalSize;
        }
        #endregion

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            UIElement element = (VisualTreeHelper.GetChildrenCount(this) == 0) ? null : (VisualTreeHelper.GetChild(this, 0) as UIElement);
            var desiredSize = new Size(0.0, 0.0);
            var extent = new Size(0.0, 0.0);
            if (element != null)
            {
                if (!this.IsScrollClient)
                {
                    desiredSize = base.MeasureOverride(availableSize);
                }
                else
                {
                    Size MeasureSize = availableSize;
                    if (this._scrollData._canHorizontallyScroll || ((element is FrameworkElement) && (((FrameworkElement)element).FlowDirection != base.FlowDirection)))
                    {
                        MeasureSize.Width = double.PositiveInfinity;
                    }
                    if (this._scrollData._canVerticallyScroll)
                    {
                        MeasureSize.Height = double.PositiveInfinity;
                    }
                    element.Measure(MeasureSize);
                    desiredSize = element.DesiredSize;
                }
                extent = element.DesiredSize;
            }
            if (this.IsScrollClient)
            {
                this.VerifyScrollData(availableSize, extent);
            }
            desiredSize.Width = Math.Min(availableSize.Width, desiredSize.Width);
            desiredSize.Height = Math.Min(availableSize.Height, desiredSize.Height);
            return desiredSize;
        }
        #endregion

        #region OnApplyTemplate
#if WinRT 
        protected override void OnApplyTemplate() 
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.HookupScrollingComponents();
        }
        #endregion

        #endregion

        #region  private methods

        private void HookupScrollingComponents()
        {
            var templatedParent = this.TemplatedParent as ScrollableContentViewer;
            if (templatedParent != null)
            {
                IScrollableInfo content = null;
                content = base.Content as IScrollableInfo ?? this;
                if ((content != this._scrollInfo) && (this._scrollInfo != null))
                {
                    if (this.IsScrollClient)
                    {
                        this._scrollData = new ScrollData();
                    }
                    else
                    {
                        this._scrollInfo.ScrollableOwner = null;
                    }
                }
                if (content != null)
                {
                    this._scrollInfo = content;
                    content.ScrollableOwner = templatedParent;
                    templatedParent.ScrollInfo = content;
                }
            }
            else if (this._scrollInfo != null)
            {
                if (this._scrollInfo.ScrollableOwner != null)
                {
                    this._scrollInfo.ScrollableOwner.ScrollInfo = null;
                }
                this._scrollInfo.ScrollableOwner = null;
                this._scrollInfo = null;
                this._scrollData = new ScrollData();
            }
        }

        private Rect CalculateTextBoxClipRect(Size arrangeSize)
        {
            double num = 0.0;
            double num2 = 0.0;
            var templatedParent = this.TemplatedParent as ScrollableContentViewer;
            double width = this._scrollData._extent.Width;
            double num4 = this._scrollData._viewport.Width;
            double x = this._scrollData._offset.X;
            var box = templatedParent.Parent as TextBox;
            var noWrap = TextWrapping.NoWrap;
            var disabled = ScrollBarVisibility.Disabled;
            if (box != null)
            {
                noWrap = box.TextWrapping;
            }
            if (noWrap == TextWrapping.Wrap)
            {
                num = templatedParent.Padding.Left + 1.0;
                num2 = templatedParent.Padding.Right + 1.0;
            }
            else
            {
                if ((num4 > width) || (x == 0.0))
                {
                    num = templatedParent.Padding.Left + 1.0;
                }
                if ((num4 > width) || ((disabled != ScrollBarVisibility.Disabled) && (Math.Abs((double)(width - (x + num4))) <= 1.0)))
                {
                    num2 = templatedParent.Padding.Right + 1.0;
                }
            }
            num = Math.Max(0.0, num);
            num2 = Math.Max(0.0, num2);
            return new Rect(-num, 0.0, (arrangeSize.Width + num) + num2, arrangeSize.Height);
        }

        private bool CoerceOffsets()
        {
            double x = CoerceOffset(this._scrollData._offset.X, this._scrollData._extent.Width, this._scrollData._viewport.Width);
            var vector = new Point(x, CoerceOffset(this._scrollData._offset.Y, this._scrollData._extent.Height, this._scrollData._viewport.Height));
            bool flag = DoubleUtil.AreClose(this._scrollData._computedOffset, vector);
            this._scrollData._computedOffset = vector;
            return flag;
        }

        private void UpdateClip(Size arrangeSize)
        {
            if (!this._isClipPropertySet)
            {
                this._clippingRectangle = new RectangleGeometry();
                base.Clip = this._clippingRectangle;
                this._isClipPropertySet = true;
            }
            //if ((base.Parent is ScrollableContentViewer) && ((base.Content is TextBoxView) || (base.Content is RichTextBoxView)))
            //{
            //    this._clippingRectangle.Rect = this.CalculateTextBoxClipRect(arrangeSize);
            //}
            //else
            {
                this._clippingRectangle.Rect = new Rect(0.0, 0.0, arrangeSize.Width, arrangeSize.Height);
            }
        }

        private void VerifyScrollData(Size viewport, Size extent)
        {
            bool flag = viewport == this._scrollData._viewport;
            flag &= extent == this._scrollData._extent;
            this._scrollData._viewport = viewport;
            this._scrollData._extent = extent;
            if (!(flag & this.CoerceOffsets()) && (this.ScrollableOwner != null))
            {
                this.ScrollableOwner.InvalidateScrollInfo();
            }
        }

        #endregion

    }

    internal class ScrollData
    {
        internal bool _canHorizontallyScroll;
        internal bool _canVerticallyScroll;
        internal Point _computedOffset = new Point(0.0, 0.0);
        internal Size _extent = new Size(0.0, 0.0);
        internal Size _maxDesiredSize = new Size(0.0, 0.0);
        internal Point _offset = new Point(0.0, 0.0);
        internal ScrollableContentViewer _scrollOwner;
        internal Size _viewport = new Size(0.0, 0.0);

        internal void ClearLayout()
        {
            this._offset = new Point(0.0, 0.0);
            this._viewport = this._extent = this._maxDesiredSize = new Size(0.0, 0.0);
        }
    }
}
