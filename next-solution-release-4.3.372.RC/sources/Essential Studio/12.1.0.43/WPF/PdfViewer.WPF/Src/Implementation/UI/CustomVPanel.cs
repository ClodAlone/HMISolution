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
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Graphics;

namespace Syncfusion.Windows.PdfViewer
{
    internal class CustomVPanel : VirtualizingPanel, IScrollInfo
    {
        # region Fields
        /// <summary>
        /// the bounds are a combination of all pages from the same row (plus the offset borders, if not ViewType.SinglePage)
        /// </summary>
        public Size[] PageRowBounds { get; set; }
        public Page[] Pages;
        internal Dictionary<object, int> PageKidsCollection { get; set; }
        private TranslateTransform m_trans = new TranslateTransform();
        private ScrollViewer m_owner;
        private bool m_canHScroll = false;
        private bool m_canVScroll = false;
        private Size m_extent = new Size(0, 0);
        private Size m_viewport = new Size(0, 0);
        private Point m_offset;
        Label m_lbl = new Label();
        internal DocumentView ParentView { get; set; }
        PdfUnitConvertor m_unitConvertor = new Pdf.Graphics.PdfUnitConvertor();
        #endregion

        # region Constructor
        public CustomVPanel()
        {
            this.RenderTransform = m_trans;
            MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(CustomVPanel_MouseLeftButtonUp);
            MouseMove += new System.Windows.Input.MouseEventHandler(CustomVPanel_MouseMove);
        }
        #endregion

        # region Methods
        void CustomVPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Dictionary<Rect, string> combainedDest = new Dictionary<Rect, string>();
            int currentRectHeight = 0, currentPage = 0;
            string destURL = string.Empty;
            bool defaultCursor = true;
            UIElement controlPresenter = null;
            if (this.Children.Count > 0)
                controlPresenter = this.Children[0];
            else
                controlPresenter = this;
            Point mousePositionPixel = e.GetPosition(controlPresenter);
            System.Drawing.PointF currentPoint = m_unitConvertor.ConvertFromPixels(new System.Drawing.PointF((float)mousePositionPixel.X, (float)e.GetPosition(this).Y), PdfGraphicsUnit.Point);
            CustomVPanel pnl = sender as CustomVPanel;
            foreach (Page pge in pnl.Pages)
            {
                Dictionary<Rect, string> destDictonary = pnl.Pages[currentPage].m_currentPageDest;
                foreach (KeyValuePair<Rect, string> item in destDictonary)
                {
                    if (!combainedDest.ContainsKey(item.Key))
                    {
                        combainedDest.Add(item.Key, item.Value);
                    }
                }
                currentPage++;
            }

            foreach (KeyValuePair<Rect, string> item in combainedDest)
            {
                if (item.Key.Contains(new Point(currentPoint.X, currentPoint.Y)))
                {
                    defaultCursor = false;
                    destURL = item.Value;
                    currentRectHeight = (int)item.Key.Height;
                }
            }

            if (defaultCursor == true)
            {
                this.Cursor = Cursors.Arrow;
                //if(this.Children.Contains(m_lbl))
                //this.Children.Remove(m_lbl);
                //ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                //if (!itemsControl.Items.Contains(m_lbl))
                //    itemsControl.Items.Remove(m_lbl);
            }
            else
            {
                this.Cursor = Cursors.Hand;
                //ShowToolTip(destURL, mousePositionPixel, currentRectHeight);
                PdfDocumentView panel = ParentView.parent as PdfDocumentView;
                PdfViewerControl pdfViewerControl = null;
                if (panel != null)
                {
                    DependencyObject parent = (sender as CustomVPanel) as UIElement;
                    while (parent != null)
                    {
                        Type parentType = parent.GetType();
                        if (parentType.Name == "PdfViewerControl")
                        {
                            pdfViewerControl = parent as PdfViewerControl;
                        }
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                    if (pdfViewerControl != null && pdfViewerControl.IslinkMouseOver == true)
                    {
                        pdfViewerControl.OnMouseOver(EventArgs.Empty);
                        //if (this.Children.Contains(m_lbl))
                        //{
                        //    this.Children.Remove(m_lbl);
                        //}
                        //ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                        //if (!itemsControl.Items.Contains(m_lbl))
                        //    itemsControl.Items.Remove(m_lbl);
                    }
                }
            }
        }

        void CustomVPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Dictionary<Rect, string> combainedDest = new Dictionary<Rect, string>();
            Dictionary<Rect, PdfArray> m_combainedPageDest = new Dictionary<Rect, PdfArray>();
            int currentPage = 0;

            string destURL = string.Empty;
            UIElement controlPresenter = null;
            if (this.Children.Count > 0)
                controlPresenter = this.Children[0];
            else
                controlPresenter = this;
            Point mousePositionPixel = e.GetPosition(controlPresenter);
            System.Drawing.PointF currentPoint = m_unitConvertor.ConvertFromPixels(new System.Drawing.PointF((float)mousePositionPixel.X, (float)e.GetPosition(this).Y), PdfGraphicsUnit.Point);

            CustomVPanel pnl = sender as CustomVPanel;
            foreach (Page pge in pnl.Pages)
            {
                Dictionary<Rect, string> destDictonary = pnl.Pages[currentPage].m_currentPageDest;
                foreach (KeyValuePair<Rect, string> item in destDictonary)
                {
                    if (!combainedDest.ContainsKey(item.Key))
                    {
                        combainedDest.Add(item.Key, item.Value);
                    }
                }
                Dictionary<Rect, PdfArray> destPageAnnotDictonary = pnl.Pages[currentPage].m_PageAnnotDest;
                foreach (KeyValuePair<Rect, PdfArray> item in destPageAnnotDictonary)
                {
                    if (!m_combainedPageDest.ContainsKey(item.Key))
                    {
                        m_combainedPageDest.Add(item.Key, item.Value);
                    }
                }
                currentPage++;
            }

            foreach (KeyValuePair<Rect, string> item in combainedDest)
            {
                if (item.Key.Contains(new Point(currentPoint.X, currentPoint.Y)))
                {
                    destURL = item.Value;
                    PdfDocumentView panel = ParentView.parent as PdfDocumentView;
                    PdfViewerControl pdfViewerControl = null;
                    if (panel != null)
                    {
                        DependencyObject parent = (sender as CustomVPanel) as UIElement;
                        while (parent != null)
                        {
                            Type parentType = parent.GetType();
                            if (parentType.Name == "PdfViewerControl")
                            {
                                pdfViewerControl = parent as PdfViewerControl;
                            }
                            parent = VisualTreeHelper.GetParent(parent);
                        }
                        if (pdfViewerControl != null && pdfViewerControl.IslinkClicked == true)
                        {
                            AnnotEventArgs args = new AnnotEventArgs(destURL);
                            pdfViewerControl.OnClicked(args);
                            //this.Children.Remove(m_lbl);
                            //ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                            //if (!itemsControl.Items.Contains(m_lbl))
                            //    itemsControl.Items.Remove(m_lbl);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(destURL))
                            {
                                Process.Start(destURL);
                            }
                            if (m_combainedPageDest.ContainsKey(item.Key))
                            {
                                PdfArray destArray = m_combainedPageDest[item.Key];
                                if (destArray.Count > 0)
                                {
                                    PdfReferenceHolder destPageRef = destArray[0] as PdfReferenceHolder;
                                    object pageRef = destPageRef.Reference;
                                    int destPage = -1;
                                    if (PageKidsCollection.ContainsKey(pageRef))
                                    {
                                        destPage = PageKidsCollection[pageRef];
                                        double destPageHeight = Pages[destPage].Height;
                                        double destAnnotLoc = (((m_combainedPageDest[item.Key])[3]) as PdfNumber).FloatValue;
                                        double tempDest = m_unitConvertor.ConvertFromPixels((float)destPageHeight, PdfGraphicsUnit.Point);
                                        double dest = tempDest - destAnnotLoc;
                                        (Parent as DocumentView).GoToPageAtIndexAndOffset(destPage + 1, (float)dest);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shows tooltip while the mouse is over an URL
        /// </summary>
        private void ShowToolTip(string destURL, Point mousePositionPixel, int currentRectHeight)
        {
            if (!string.IsNullOrEmpty(destURL))
            {
                m_lbl.Content = destURL;
            }
            m_lbl.Margin = new Thickness(mousePositionPixel.X, mousePositionPixel.Y + currentRectHeight, 0, 0);
            m_lbl.BorderBrush = new SolidColorBrush(Colors.Black);
            m_lbl.BorderThickness = new Thickness(0.5);
            m_lbl.Background = new SolidColorBrush(Colors.WhiteSmoke);
            this.ApplyTemplate();
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            
            //if (!this.Children.Contains(m_lbl))
            //{
            //    this.Children.Add(m_lbl);
            //}
            //if (!itemsControl.Items.Contains(m_lbl))
            //    itemsControl.Items.Add(m_lbl);
        }

        public int GetItemIndex(double yOffset)
        {
            var sum = 0.0;

            for (int i = 0; i < PageRowBounds.Length; i++)
            {
                sum += PageRowBounds[i].Height;

                if (yOffset < sum)
                {
                    sum = sum - (PageRowBounds[i].Height / 3);
                    if (yOffset > sum)
                    {
                        return i+1;
                    }
                    return i;
                }
            }

            return PageRowBounds.Length - 1;
        }

        public double GetVerticalOffset(int itemIndex)
        {
            return PageRowBounds.Take(itemIndex).Sum(f => f.Height);
        }

        /// <summary>
        /// Measure the children
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>Size desired</returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            float zero = 0;
            if (availableSize.Height == 1 / zero)
            {
                availableSize = new Size(availableSize.Width, 702);
            }
            UpdateScrollInformation(availableSize);

            if (PageRowBounds == null || PageRowBounds.Length == 0)
                return availableSize;

            int firstVisibleItemIndex, lastVisibleItemIndex;
            VisibleRange(out firstVisibleItemIndex, out lastVisibleItemIndex);

            if (firstVisibleItemIndex == -1)
                return availableSize;

            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

            int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex)
                {
                    bool newlyRealized;

                    UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                    if (newlyRealized)
                    {
                        if (childIndex >= children.Count)
                        {
                            base.AddInternalChild(child);
                        }
                        else
                        {
                            base.InsertInternalChild(childIndex, child);
                        }
                        generator.PrepareItemContainer(child);
                    }
                    child.Measure(PageRowBounds[itemIndex]);
                }
            }
            CleanItems(firstVisibleItemIndex, lastVisibleItemIndex);

            return availableSize;
        }

        /// <summary>
        /// Arrange the children
        /// </summary>
        /// <param name="finalSize">Size available</param>
        /// <returns>Size used</returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            UpdateScrollInformation(finalSize);

            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement child = this.Children[i];

                int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

                ArrangeChild(itemIndex, child, finalSize);
            }

            return finalSize;
        }

        /// <summary>
        /// Revirtualize items that are no longer visible
        /// </summary>
        /// <param name="minDesiredGenerated">first item index that should be visible</param>
        /// <param name="maxDesiredGenerated">last item index that should be visible</param>
        private void CleanItems(int minDesiredGenerated, int maxDesiredGenerated)
        {
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);

                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        /// <summary>
        /// Calculate the extent of the view based on the available size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <param name="itemCount">number of data items</param>
        private System.Windows.Size CalculateExtent(System.Windows.Size availableSize, int itemCount)
        {
            if (PageRowBounds == null || PageRowBounds.Length == 0)
                return new Size(availableSize.Width, m_extent.Height);

            var maxWidth = PageRowBounds.Select(f => f.Width).Max();
            var totalHeight = PageRowBounds.Sum(f => f.Height);

            return new Size(maxWidth, totalHeight);
        }

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
        /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
        public void VisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            firstVisibleItemIndex = -1;
            lastVisibleItemIndex = -1;

            if (PageRowBounds == null || PageRowBounds.Length == 0)
                return;

            double sum = 0.0;
            var bottom = m_offset.Y + m_viewport.Height;

            for (int i = 0; i < PageRowBounds.Length; i++)
            {
                sum += PageRowBounds[i].Height;

                if (m_offset.Y < sum)
                {
                    firstVisibleItemIndex = i;
                    lastVisibleItemIndex = i;

                    for (int k = i + 1; k < PageRowBounds.Length; k++)
                    {
                        sum += PageRowBounds[k].Height;

                        if (bottom < sum || k == PageRowBounds.Length - 1)
                        {
                            lastVisibleItemIndex = k;
                            break;
                        }
                    }

                    break;
                }
            }

            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

            if (lastVisibleItemIndex >= itemCount)
                lastVisibleItemIndex = itemCount - 1;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (sizeInfo.WidthChanged && sizeInfo.NewSize.Width > sizeInfo.PreviousSize.Width)
            {
                var widthOffset = sizeInfo.NewSize.Width - sizeInfo.PreviousSize.Width;
                this.ScrollOwner.ScrollToHorizontalOffset(this.ScrollOwner.HorizontalOffset - widthOffset);
            }
        }

        /// <summary>
        /// Position a child
        /// </summary>
        /// <param name="itemIndex">The data item index of the child</param>
        /// <param name="child">The element to position</param>
        /// <param name="finalSize">The size of the panel</param>
        private void ArrangeChild(int itemIndex, UIElement child, System.Windows.Size finalSize)
        {
            var size = PageRowBounds[itemIndex];
            var x = Math.Max(0, (finalSize.Width / 2) - (size.Width / 2)); // used to center the content horizontally
            var y = GetVerticalOffset(itemIndex);
            Dispatcher.BeginInvoke(new Action(() => 
            child.Arrange(new Rect(x, y, size.Width, size.Height))));
        }

        # endregion

        #region IScrollInfo implementation

        private void UpdateScrollInformation(Size availableSize)
        {
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

            Size extent = CalculateExtent(availableSize, itemCount);
            if (extent != m_extent)
            {
                m_extent = extent;
                if (m_owner != null)
                    m_owner.InvalidateScrollInfo();
            }

            if (availableSize != m_viewport)
            {
                m_viewport = availableSize;
                if (m_owner != null)
                    m_owner.InvalidateScrollInfo();
            }
        }

        public ScrollViewer ScrollOwner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public bool CanHorizontallyScroll
        {
            get { return m_canHScroll; }
            set { m_canHScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return m_canVScroll; }
            set { m_canVScroll = value; }
        }

        public double HorizontalOffset
        {
            get { return m_offset.X; }
        }

        public double VerticalOffset
        {
            get { return m_offset.Y; }
        }

        public double ExtentHeight
        {
            get { return m_extent.Height; }
        }

        public double ExtentWidth
        {
            get { return m_extent.Width; }
        }

        public double ViewportHeight
        {
            get { return m_viewport.Height; }
        }

        public double ViewportWidth
        {
            get { return m_viewport.Width; }
        }

        private double CalculateVerticalScrollOffset()
        {
            return ViewportHeight * 0.06;
        }

        private double CalculateHorizontalScrollOffset()
        {
            return ViewportWidth * 0.06;
        }

        public void LineUp()
        {
            SetVerticalOffset(this.VerticalOffset - CalculateVerticalScrollOffset());
        }

        public void LineDown()
        {
            SetVerticalOffset(this.VerticalOffset + CalculateVerticalScrollOffset());
        }

        public void PageUp()
        {
            SetVerticalOffset(this.VerticalOffset - m_viewport.Height);
        }

        public void PageDown()
        {
            SetVerticalOffset(this.VerticalOffset + m_viewport.Height);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(this.VerticalOffset - (3 * CalculateVerticalScrollOffset()));
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(this.VerticalOffset + (3 * CalculateVerticalScrollOffset()));
        }

        public void LineLeft()
        {
            SetHorizontalOffset(this.HorizontalOffset - CalculateHorizontalScrollOffset());
        }

        public void LineRight()
        {
            SetHorizontalOffset(this.HorizontalOffset + CalculateHorizontalScrollOffset());
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        public void MouseWheelLeft()
        {
            this.LineLeft();
        }

        public void MouseWheelRight()
        {
            this.LineRight();
        }

        public void PageLeft()
        {
            SetHorizontalOffset(this.HorizontalOffset - m_viewport.Width);
        }

        public void PageRight()
        {
            SetHorizontalOffset(this.HorizontalOffset + m_viewport.Width);
        }

        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || m_viewport.Width >= m_extent.Width)
            {
                offset = 0;
            }
            else if (offset + m_viewport.Width >= m_extent.Width)
            {
                offset = m_extent.Width - m_viewport.Width;
            }

            m_offset.X = offset;

            if (m_owner != null)
                m_owner.InvalidateScrollInfo();

            m_trans.X = -offset;

            InvalidateMeasure();
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || m_viewport.Height >= m_extent.Height)
            {
                offset = 0;
            }
            else if (offset + m_viewport.Height >= m_extent.Height)
            {
                offset = m_extent.Height - m_viewport.Height;
            }

            m_offset.Y = offset;

            if (m_owner != null)
                m_owner.InvalidateScrollInfo();

            m_trans.Y = -offset;

            InvalidateMeasure();
        }
        #endregion
    }
}
