#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Collections;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Shared;
using Syncfusion.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections;
using Syncfusion.Windows.Data;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Data;

namespace Syncfusion.Windows.Controls.Grid
{

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class VirtualizingItemsPanel : VirtualizingPanel, IScrollInfo
    {
        #region  Private members

        // int _vStartIndex = -1, _vEndIndex = -1; The variable is assigned but it is never used
        ItemsControl HostItemsControl;

        internal int VirtualStartIndex
        {
            get;
            set;
        }

        internal int VirtualEndIndex
        {
            get;
            set;
        }


        bool _canHScroll = false, _canVScroll = false;
        Size _viewport, _offset, _extent;
        Point _scrollOffset;
        ScrollViewer _scrollOwner;
        TranslateTransform _transform = new TranslateTransform();

#if !SILVERLIGHT

        static VirtualizingItemsPanel()
        {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualizingItemsPanel), new FrameworkPropertyMetadata(typeof(VirtualizingItemsPanel)));
        }
#endif

        public VirtualizingItemsPanel()
        {
            
           // this.DefaultStyleKey = typeof(VirtualizingItemsPanel);
            _viewport = new Size(0, 0);
            _offset = new Size(0, 0);
            _extent = new Size(0, 0);
            this.RenderTransform = _transform;
#if !SyncfusionFramework3_5
            UseLayoutRounding = false;
#endif
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the row.</value>
        public double ItemsHeight
        {
            get { return (double)GetValue(ItemsHeightProperty); }
            set { SetValue(ItemsHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsHeightProperty =
            DependencyProperty.Register("ItemsHeight", typeof(double), typeof(VirtualizingItemsPanel), new PropertyMetadata(18d));

        #endregion

        #region Overrides

#if !SILVERLIGHT
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.HostItemsControl = (ItemsControl)ItemsControl.GetItemsOwner(this);
            double fontsize = ItemsControl.GetItemsOwner(this).FontSize;
            FontFamily fontfamily = ItemsControl.GetItemsOwner(this).FontFamily;
            this.ItemsHeight = Math.Ceiling(fontsize * fontfamily.LineSpacing);
            ItemsPresenter itempresenter = this.HostItemsControl.FindElementOfType<ItemsPresenter>();
            itempresenter.Margin = new Thickness(0, this.ItemsHeight, 0, 0);            
        }
#else
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.HostItemsControl = (ItemsControl)ItemsControl.GetItemsOwner(this);
        }
#endif

        #region  MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
            // UpdateScrollInfo(availableSize);

            int startIndex, endIndex;

            CalculateVisibleRange(out startIndex, out endIndex);

            AddItems(startIndex, endIndex);

            RemoveItems(startIndex, endIndex);

            return base.MeasureOverride(availableSize);
        }

        #endregion

        #region Arrange override

        protected override Size ArrangeOverride(Size finalSize)
        {
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            

            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement child = this.Children[i];

                // This index is to map the child UI to exact index of the item, because of this we can achieve the smooth scrolling
                // We can also directly use the Y coordinate as i*RowHeight but it wont give smoont scrolling. 
                // By having this index and appying transForm we can get smooth scrolling.
                int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                //// As of now the row height is fixed so we have calculated the y position by itemIndex * RowHeight. 
                //// In future while supporting row resizing we have to change this implementation
                child.Arrange(new Rect(0, itemIndex * ItemsHeight, child.DesiredSize.Width, ItemsHeight));
            }
            UpdateScrollInfo(finalSize);
            return finalSize;
        }

        /// <summary>
        /// When items are removed, remove the corresponding UI if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
            }
        }

        #endregion

        #endregion

        #region Helper method

        /// <summary>
        /// Validates the vertical offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private double ValidateVerticalOffset(double offset)
        {
            if (offset < 0 || _viewport.Height >= _extent.Height)
            {
                offset = 0;
            }
            else
            {
                if (offset + _viewport.Height >= _extent.Height)
                {
                    offset = _extent.Height - _viewport.Height;
                }
            }

            return offset;
        }

        /// <summary>
        /// Validates the horizontal offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private double ValidateHorizontalOffset(double offset)
        {
            if (offset < 0 || _viewport.Width >= _extent.Width)
            {
                offset = 0;
            }
            else
            {
                if (offset + _viewport.Width >= _extent.Width)
                {
                    offset = _extent.Width - _viewport.Width;
                }
            }

            return offset;
        }

        /// <summary>
        /// Updates the scroll info.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        private void UpdateScrollInfo(Size availableSize)
        {
            bool viewPortChanged = false, extentChanged = false;

#if !SILVERLIGHT
            int itemCount = this.HostItemsControl.HasItems ? this.HostItemsControl.Items.Count : 0;
#else
            if (this.HostItemsControl == null)
                this.HostItemsControl = (ItemsControl)ItemsControl.GetItemsOwner(this);

            int itemCount = this.HostItemsControl.Items != null ? this.HostItemsControl.Items.Count : 0;
#endif
            double childWidth = availableSize.Width;
            var children = this.Children.ToList<UIElement>();

            if (children.Count() != 0)
            {
                childWidth = children.OrderByDescending(s => s.DesiredSize.Width > this.ActualWidth).First().DesiredSize.Width;    
            }

            Size tempExtent = new Size(childWidth, itemCount * ItemsHeight);

            if (_extent != tempExtent)
            {
                _extent = tempExtent;
                extentChanged = true;
            }

            if (_viewport != availableSize)
            {
                _viewport = availableSize;
                viewPortChanged = true;
            }

            if (_scrollOwner != null && (viewPortChanged || extentChanged))
            {
                if (_scrollOwner != null)
                    _scrollOwner.InvalidateScrollInfo();
               
            }
        }

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="startIndex">The item index of the first visible item</param>
        /// <param name="endIndex">The item index of the last visible item</param>
        private void CalculateVisibleRange(out int startIndex, out int endIndex)
        {
            startIndex = (int)Math.Floor(_scrollOffset.Y / this.ItemsHeight);
            endIndex = (int)Math.Ceiling((_scrollOffset.Y + _viewport.Height) / this.ItemsHeight) - 1;

            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
#if !SILVERLIGHT
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
#else
            int itemCount = itemsControl.Items != null ? itemsControl.Items.Count : 0;
#endif
            if (endIndex >= itemCount)
                endIndex = itemCount - 1;

            this.VirtualStartIndex = startIndex;
            this.VirtualEndIndex = endIndex;
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        private void AddItems(int startIndex, int endIndex)
        {
#if !SILVERLIGHT
            UIElementCollection children = this.InternalChildren;
#else
            UIElementCollection children = this.Children;
#endif
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

            // Get the generator position of the first visible data item
            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(startIndex);

            // Get the index to decide whether we need to add it in sequential flow or have to insert at some where 
            int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = startIndex; itemIndex <= endIndex && itemIndex < itemsControl.Items.Count; ++itemIndex, ++childIndex)
                {
                    bool newlyRealized;

                    // Get or create the child
                    UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                    if (newlyRealized)
                    {
                        if (childIndex >= children.Count)
                        {
                            base.AddInternalChild(child);
                        }
                        else
                        {
                            // this will woks when we scroll from bottom to top.
                            base.InsertInternalChild(childIndex, child);
                        }
                        generator.PrepareItemContainer(child);
                    }
                }
            }
        }

        /// <summary>
        /// Remove items that are no longer visible
        /// </summary>
        /// <param name="startIndex">first item index that should be visible</param>
        /// <param name="endIndex">last item index that should be visible</param>
        private void RemoveItems(int startIndex, int endIndex)
        {
#if !SILVERLIGHT
            UIElementCollection children = this.InternalChildren;
#else
            UIElementCollection children = this.Children;
#endif
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
                if (itemIndex > -1 && (itemIndex < startIndex || itemIndex > endIndex))
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        #endregion

        #region IScrollInfo Members

        public bool CanHorizontallyScroll
        {
            get
            {
                return _canHScroll;
            }
            set
            {
                _canHScroll = value;
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                return _canVScroll;
            }
            set
            {
                _canVScroll = value;
            }
        }

        public double ExtentHeight
        {
            get
            {
                return _extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                return _extent.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return _scrollOffset.X;
            }
        }

        public void LineDown()
        {
            this.SetVerticalOffset(VerticalOffset + ItemsHeight);
        }

        public void LineUp()
        {
            this.SetVerticalOffset(VerticalOffset - ItemsHeight);
        }

        public void PageDown()
        {
            this.SetHorizontalOffset(VerticalOffset - _viewport.Height);
        }

        public void PageUp()
        {
            this.SetHorizontalOffset(VerticalOffset + _viewport.Height);
        }

#if !SILVERLIGHT
        public Rect MakeVisible(Visual visual, Rect rectangle)
#else
        public Rect MakeVisible(UIElement visual, Rect rectangle)
#endif
        {
            return new Rect();
        }

        public ScrollViewer ScrollOwner
        {
            get
            {
                return _scrollOwner;
            }
            set
            {
                _scrollOwner = value;
                if (_scrollOwner != null)
                {
                    _scrollOwner.InvalidateMeasure();
                    _scrollOwner.InvalidateArrange();
                }
            }
        }

        public void SetVerticalOffset(double offset)
        {
            offset = ValidateVerticalOffset(offset);

            _scrollOffset.Y = offset;

            if (_scrollOwner != null)
                _scrollOwner.InvalidateScrollInfo();

            _transform.Y = -VerticalOffset;

            InvalidateMeasure();
        }

        public double VerticalOffset
        {
            get
            {
                return _scrollOffset.Y;
            }
        }

        public double ViewportHeight
        {
            get
            {
                return _viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                return _viewport.Width;
            }
        }

        #region Horizontal Scrollbar

        /// <summary>
        ///  Since we are not providing virtualizaion towards horizantal, it is not manditory to implement these methods.
        /// </summary>
        public void LineLeft()
        {
            this.SetHorizontalOffset(HorizontalOffset - ItemsHeight);
        }

        public void LineRight()
        {
            this.SetHorizontalOffset(HorizontalOffset + ItemsHeight);
        }

        public void MouseWheelDown()
        {
            LineDown();
        }

        public void MouseWheelLeft() { }

        public void MouseWheelRight() { }

        public void MouseWheelUp()
        {
            LineUp();
        }

        public void PageLeft()
        {
            LineLeft();
        }

        public void PageRight()
        {
            LineRight();
        }

        public void SetHorizontalOffset(double offset)
        {
            offset = ValidateHorizontalOffset(offset);

            _scrollOffset.X = offset;

            if (_scrollOwner != null)
                _scrollOwner.InvalidateScrollInfo();

            _transform.X = -HorizontalOffset;

            InvalidateMeasure();
        }

        #endregion

        #endregion
    }
}
