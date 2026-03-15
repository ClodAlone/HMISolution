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
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using Syncfusion.Windows.Controls.Gantt.Schedule;

namespace Syncfusion.Windows.Controls.Gantt.Chart
{
    /// <summary>
    /// Represents a virtualizing panel that will arrange the Chart items
    /// </summary>
    public class GanttChartItemsPanel : VirtualizingPanel, IScrollInfo
    {
        #region  Private members

        GanttChart HostItemsControl;

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

        internal GanttNodeConnector NodeConnectorPanel
        {
            get
            {
                return this.HostItemsControl != null ? this.HostItemsControl.NodeConnectorPanel : null;
            }
        }

        bool _canHScroll = false, _canVScroll = false;
        Size _viewport, _offset, _extent;
        Point _scrollOffset;
        ScrollViewer _scrollOwner;
        TranslateTransform _transform = new TranslateTransform();

        public GanttChartItemsPanel()
        {
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
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(GanttChartItemsPanel), new PropertyMetadata(24d));

        #endregion

        #region Overrides

#if !SILVERLIGHT
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.HostItemsControl = (GanttChart)ItemsControl.GetItemsOwner(this);
            this.RowHeight = this.HostItemsControl.RowHeight;
        }
#else
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.HostItemsControl = (GanttChart)ItemsControl.GetItemsOwner(this);
        }
#endif

        #region  MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateScrollInfo(availableSize);

            int startIndex, endIndex;

            CalculateVisibleRange(out startIndex, out endIndex);

            AddItems(startIndex, endIndex);

            RemoveItems(startIndex, endIndex);
//#if !SILVERLIGHT
//            UpdateConnectorPanel(startIndex, endIndex);
//#else
            if (this.HostItemsControl != null)
                this.HostItemsControl.isConnectorRefreshed = false;
//#endif

            return base.MeasureOverride(availableSize);
        }

        #endregion

        #region Arrange override

        protected override Size ArrangeOverride(Size finalSize)
        {
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            UpdateScrollInfo(finalSize);

            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement child = this.Children[i];

                // This index is to map the child UI to exact index of the item, because of this we can achieve the smooth scrolling
                // We can also directly use the Y coordinate as i*RowHeight but it wont give smoont scrolling. 
                // By having this index and appying transForm we can get smooth scrolling.
                int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

                (child as GanttChartRow).Height = RowHeight;

                //// As of now the row height is fixed so we have calculated the y position by itemIndex * RowHeight. 
                //// In future while supporting row resizing we have to change this implementation
                child.Arrange(new Rect(0, itemIndex * RowHeight, finalSize.Width, RowHeight));
            }

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

        //private void UpdateConnectorPanel(int startIndex, int endIndex)
        //{
        //    if (this.NodeConnectorPanel != null)
        //    {
        //        NodeConnectorPanel.StartIndex = startIndex;
        //        NodeConnectorPanel.EndIndex = endIndex;
        //        NodeConnectorPanel.RefreshConnectors();
        //    }
        //}

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


        private void UpdateScrollInfo(Size availableSize)
        {
            bool viewPortChanged = false, extentChanged = false;

#if !SILVERLIGHT
            int itemCount = this.HostItemsControl.HasItems ? this.HostItemsControl.Items.Count : 0;
#else
            if (this.HostItemsControl == null)
            {
                this.HostItemsControl = (GanttChart)ItemsControl.GetItemsOwner(this);
                this.RowHeight = this.HostItemsControl.RowHeight;
            }

            int itemCount = this.HostItemsControl.Items != null ? this.HostItemsControl.Items.Count : 0;
#endif

            Size tempExtent = new Size(this.ActualWidth, itemCount * RowHeight);

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
            startIndex = (int)Math.Floor(_scrollOffset.Y / this.RowHeight);
            endIndex = (int)Math.Ceiling((_scrollOffset.Y + _viewport.Height) / this.RowHeight) - 1;

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
            this.SetVerticalOffset(VerticalOffset + RowHeight);
        }

        public void LineUp()
        {
            this.SetVerticalOffset(VerticalOffset - RowHeight);
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

        #region Non implemented methods

        /// <summary>
        ///  Since we are not providing virtualizaion towards horizantal, it is not manditory to implement these methods.
        /// </summary>

        public void LineLeft() { }

        public void LineRight() { }

        public void MouseWheelDown() { }

        public void MouseWheelLeft() { }

        public void MouseWheelRight() { }

        public void MouseWheelUp() { }

        public void PageLeft() { }

        public void PageRight() { }

        public void SetHorizontalOffset(double offset) { }

        #endregion

        #endregion
    }
}
