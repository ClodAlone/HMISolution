#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if !SILVERLIGHT
using System.Threading.Tasks;
#endif
using Syncfusion.UI.Xaml.Grid;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
#endif
namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    public class NumericButtonPanel : Panel, IScrollableInfo
    {
        #region Private Members

        private bool canHorizontallyScroll = false;
        private bool canVerticallyScroll = false;
        private bool isItemsPregenerated;

        private double extentHeight;
        private double extentWidth;
        private double horizontalOffset;
        private double verticalOffset;
        private double viewPortWidth;
        private double viewPortHeight;
        private double horizontalPadding;
        private double verticalPadding;
        private double lastArrangedOffset = -1;

        private ScrollableContentViewer scrollableOwner;

        internal Size DefaultItemSize;
        internal bool internalOffset;

        internal IItemGenerator ItemGenerator
        {
            get { return this.DataPager.ItemGenerator; }
        }

        internal SfDataPager DataPager { get; set; }

        #endregion

        #region Ctor

        public NumericButtonPanel()
        {
#if WinRT
            DefaultItemSize = new Size(50, 50);
#else
            DefaultItemSize = new Size(35, 40);
#endif
        }

        #endregion

        #region Overrides

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.ArrangeItems(finalSize);
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {            
            if (!isItemsPregenerated && this.DataPager.PagedSource!=null)
            {
                this.InitializeScrollViewerValue();
                PreGenerateItems();
            }
            if (this.DataPager.PagedSource != null)
                EnsureItems();

            this.InvalidatScrollInfo();
            
            if (availableSize.Width == double.PositiveInfinity || availableSize.Width == double.NegativeInfinity)
            {
                availableSize.Width = Math.Min(this.DataPager.NumericButtonCount, this.DataPager.PageCount)*
                                      DefaultItemSize.Width;
            }

            if (double.IsInfinity(availableSize.Height))
                availableSize.Height = DefaultItemSize.Height;

            return availableSize;
        }

        #endregion

        #region IScrollInfo Members

        public bool CanHorizontallyScroll
        {
            get { return canHorizontallyScroll; }
            set { canHorizontallyScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return canVerticallyScroll; }
            set { canVerticallyScroll = value; }
        }

        public double ExtentHeight
        {
            get { return extentHeight; }
        }

        public double ExtentWidth
        {
            get { return extentWidth; }
        }

        public double HorizontalOffset
        {
            get { return horizontalOffset; }
        }

        public ScrollableContentViewer ScrollableOwner
        {
            get { return scrollableOwner; }
            set { scrollableOwner = value; }
        }

#if !WinRT
        private ScrollViewer scrollOwner;
        ScrollViewer IScrollInfo.ScrollOwner
        {
            get { return scrollOwner; }
            set { scrollOwner = value; }
        }
#endif

        public double VerticalOffset
        {
            get { return verticalOffset; }
        }

        public double ViewportHeight
        {
            get { return viewPortHeight; }
        }

        public double ViewportWidth
        {
            get { return viewPortWidth; }
        }

        public void LineDown()
        {
            SetVerticalOffset(this.verticalOffset + this.DefaultItemSize.Height);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(this.horizontalOffset - this.DefaultItemSize.Width);
        }

        public void LineRight()
        {
            SetHorizontalOffset(this.horizontalOffset + this.DefaultItemSize.Width);
        }

        public void LineUp()
        {
            SetVerticalOffset(this.verticalOffset - this.DefaultItemSize.Height);
        }

        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            return rectangle;
        }

#if WPF
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
        }
#endif

        public void MouseWheelDown()
        {
            LineDown();
        }

        public void MouseWheelLeft()
        {
            LineLeft();
        }

        public void MouseWheelRight()
        {
            LineRight();
        }

        public void MouseWheelUp()
        {
            LineUp();
        }

        public void PageDown()
        {
            this.SetVerticalOffset(this.DefaultItemSize.Height);
        }

        public void PageLeft()
        {
            var nextPageOffset = this.DefaultItemSize.Width*this.DataPager.NumericButtonCount;
            this.SetHorizontalOffset(this.horizontalOffset - nextPageOffset);
        }

        public void PageRight()
        {
            var nextPageOffset = this.DefaultItemSize.Width * this.DataPager.NumericButtonCount;
            this.SetHorizontalOffset(this.horizontalOffset + nextPageOffset);
        }

        public void PageUp()
        {
            this.SetVerticalOffset(this.DefaultItemSize.Height);
        }

        public void SetHorizontalOffset(double offset)
        {
            if (CanHorizontallyScroll)
            {
                offset = Math.Max(0, offset);
                this.horizontalOffset = offset;
                this.InvalidateMeasure();
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (CanVerticallyScroll)
            {
                offset = Math.Max(0, offset);
                this.verticalOffset = offset;
                this.InvalidateMeasure();
            }
        }

        internal void SetHorizontalOffset(double offset, bool isInternal)
        {
            this.SetHorizontalOffset(offset);
            internalOffset = true;
        }

        #endregion

        #region Helping Methods

        private void InvalidatScrollInfo()
        {
            if (this.ScrollableOwner != null)
            {
                this.ScrollableOwner.InvalidateScrollInfo();
            }
        }

        private void PreGenerateItems()
        {
            if (DataPager != null)
            {
                this.ItemGenerator.PreGenerateItems(0, DataPager.NumericButtonCount - 1);
                this.isItemsPregenerated = true;
            }
        }

        private void InitializeScrollViewerValue()
        {
            this.extentWidth = DefaultItemSize.Width*this.DataPager.PageCount;
            this.extentHeight = DefaultItemSize.Height;
        }

        private void UpdateScrollBarValues(double newViewPortWidth, double newViewPortHeight, double newExtendedWidth, double newExtendedHeight)
        {
            if (extentWidth < newExtendedWidth)
                this.extentWidth = newExtendedWidth;
            this.extentHeight = newExtendedHeight;
            this.viewPortWidth = newViewPortWidth;
            this.viewPortHeight = newViewPortHeight;

            if (this.DataPager.PageCount <= this.DataPager.NumericButtonCount)
                this.extentWidth = this.viewPortWidth;

            this.DataPager.ScrollViewer.Height = viewPortHeight;
            this.DataPager.ScrollViewer.Width = this.viewPortWidth;
        }

        private void EnsureItems()
        {
            var start = 0;
            if (lastArrangedOffset == horizontalOffset && !internalOffset)
                return;
            start = DataPager.Orientation == Orientation.Horizontal
                  ? (int) (this.horizontalOffset/DefaultItemSize.Width)
                  : (int) (this.verticalOffset/DefaultItemSize.Height);
            int end = start + Math.Min(this.DataPager.NumericButtonCount, this.DataPager.PageCount) - 1;
            if(internalOffset)
                lastArrangedOffset = this.horizontalOffset;
            this.ItemGenerator.EnsureItems(start, end, internalOffset);
            internalOffset = false;

            if (this.ItemGenerator != null && this.ItemGenerator.Items.Count != this.Children.Count)
            {
                foreach (var item in this.ItemGenerator.Items)
                {
                    if (!this.Children.Contains(item.Element))
                    {
                        this.Children.Add(item.Element);
                    }
                }
            }
            
        }

        private void ArrangeItems(Size availableSize)
        {
            bool isLeftElipsisAvailable = false;
            if (this.ItemGenerator == null)
                return;

            double xPos = (0 - (this.HorizontalOffset - (Convert.ToDouble(DefaultItemSize.Width) * Math.Floor((this.HorizontalOffset/(Convert.ToDouble(DefaultItemSize.Width)))))));
            double yPos=(0 - (this.VerticalOffset - (Convert.ToDouble(DefaultItemSize.Height) * Math.Floor((this.VerticalOffset / (Convert.ToDouble(DefaultItemSize.Height)))))));
            var orderedItems = this.ItemGenerator.Items.OrderBy(item => item.Index);

            double newViewPortWidth = 0d;
            double newExendedWidth = this.extentWidth;
            double newViewPortHeight = 0d;
            double newExtendedHeight = this.extentHeight;
            Size elementSize = new Size();
            if (DataPager.Orientation == Orientation.Horizontal)
            {
#if WPF               
                yPos = (availableSize.Height / 2)-(DefaultItemSize.Height/2);
#elif SILVERLIGHT
                yPos = (availableSize.Height / 2) -Math.Round(DefaultItemSize.Height / 2) ;               
#else
                yPos = (availableSize.Height / 2) -Math.Round(DefaultItemSize.Height / 2) ;
#endif
            }
            else
            {
#if WPF
                xPos = (availableSize.Width / 2) - (DefaultItemSize.Width / 2);
#elif SILVERLIGHT
                xPos = (availableSize.Width / 2) - Math.Round(DefaultItemSize.Width / 2);
#else
                xPos = (availableSize.Width / 2) -Math.Round(DefaultItemSize.Width / 2) ;
#endif

            }
            foreach (var item in orderedItems)
            {
                if (item.Element.Visibility == Visibility.Collapsed)
                    continue;

                item.Element.Measure(availableSize);

                this.GetSize(item.Element.DesiredSize, ref elementSize, ref newViewPortWidth, ref newViewPortHeight);
                var rect = new Rect(new Point(xPos, yPos), elementSize);
                if (item.IsElipsisElement)
                {
                    item.Element.Arrange(DataPager.Orientation == Orientation.Horizontal
                                        ? new Rect(new Point(GetElipsisXPos(item as ScrollableElement), yPos), DefaultItemSize)
                                        : new Rect(new Point(0, GetElipsisYPos(item as ScrollableElement)), DefaultItemSize));
                    isLeftElipsisAvailable = (item as ScrollableElement).ElipsisPosition == ElipsisPosition.Left;
                }
                else
                {
                    item.Element.Clip = null;
                    if (this.DataPager.AutoEllipsisMode == AutoEllipsisMode.Both ||
                        this.DataPager.AutoEllipsisMode == AutoEllipsisMode.Before)
                    {
                        if (DataPager.Orientation == Orientation.Horizontal && isLeftElipsisAvailable && rect.X < DefaultItemSize.Width)
                        {
                            var clipRect = new Rect(DefaultItemSize.Width - rect.X, 0, elementSize.Width, elementSize.Height);
                            item.Element.Clip = new RectangleGeometry { Rect = clipRect };
                        }
                        else if (DataPager.Orientation == Orientation.Vertical && isLeftElipsisAvailable && rect.Y < DefaultItemSize.Height)
                        {
                            var clipRect = new Rect(0, DefaultItemSize.Height - rect.Y, elementSize.Height, elementSize.Width);
                            item.Element.Clip = new RectangleGeometry { Rect = clipRect };
                        }
                        
                    }
                    item.Element.Arrange(rect);
                }
                if (DataPager.Orientation == Orientation.Horizontal)
                    xPos += DefaultItemSize.Width;
                else
                    yPos += DefaultItemSize.Height;                
            }
            if (this.viewPortWidth >0 && newViewPortWidth > this.viewPortWidth)
            {
                newExendedWidth += newViewPortWidth - this.viewPortWidth;
            }

            if (this.viewPortWidth == 0 && newViewPortWidth > 0)
            {
                newExendedWidth += newViewPortWidth - (Math.Min(this.DataPager.NumericButtonCount, this.DataPager.PageCount) * this.DefaultItemSize.Width);
            }

            if (newExendedWidth < this.DataPager.PageCount*DefaultItemSize.Width)
                newExendedWidth = this.DataPager.PageCount*DefaultItemSize.Width;
            if (DataPager.Orientation == Orientation.Vertical)
            {
                if (newExtendedHeight < this.DataPager.PageCount * DefaultItemSize.Height)
                    newExtendedHeight = this.DataPager.PageCount * DefaultItemSize.Height;                
                newViewPortWidth = DefaultItemSize.Height;
                newExendedWidth = DefaultItemSize.Width;
            }
            if (DataPager.Orientation == Orientation.Horizontal)
            {                
                 newExtendedHeight= DefaultItemSize.Height;
            } 
            UpdateScrollBarValues(newViewPortWidth, newViewPortHeight, newExendedWidth, newExtendedHeight);
        }

        private double GetElipsisXPos(ScrollableElement element)
        {
            if (element.ElipsisPosition == ElipsisPosition.Right)
                return this.viewPortWidth - DefaultItemSize.Width;
            return 0;
        }

        private double GetElipsisYPos(ScrollableElement element)
        {
            if (element.ElipsisPosition == ElipsisPosition.Right)
                return this.viewPortHeight - DefaultItemSize.Height;
            return 0;
        }


        private void GetSize(Size originalSize, ref Size elementSize, ref double viewPortWidth, ref double newViewPortHeight)
        {
            elementSize.Width = originalSize.Width < DefaultItemSize.Width ? DefaultItemSize.Width : originalSize.Width;
            elementSize.Height = originalSize.Height < DefaultItemSize.Height
                               ? DefaultItemSize.Height
                               : originalSize.Height;

            if (this.DataPager.Orientation == Orientation.Horizontal)
            {
                viewPortWidth += elementSize.Width;
                newViewPortHeight = DefaultItemSize.Height;
            }
            else
            {
                newViewPortHeight += DefaultItemSize.Height;
                viewPortWidth = elementSize.Width;
            }
        }

        #endregion


        public double VerticalPadding
        {
            get
            {
                return verticalPadding;
            }
            set
            {
                verticalPadding=value;
            }
        }

        public double HorizontalPadding
        {
            get
            {
                return horizontalPadding;
            }
            set
            {
                horizontalPadding=value;
            }
        }
    }
}