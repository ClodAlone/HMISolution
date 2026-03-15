#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Panel for layoting <see cref="RibbonBar"/>'s content.
    /// </summary>
    public class RibbonBarPanel : Panel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonBarPanel"/> class.
        /// </summary>
        public RibbonBarPanel()
        {
        }

        #endregion

        #region Properties

        #region LayoutMode

        /// <summary>
        /// Gets or sets a value indicating how the <see cref="RibbonBarPanel"/> lays out the items.
        /// </summary>
        public LayoutMode LayoutMode
        {
            get { return (LayoutMode)GetValue(LayoutModeProperty); }
            set { SetValue(LayoutModeProperty, value); }
        }

        /// <summary>
        /// The identifier of <see cref="LayoutMode"/> property
        /// </summary>
        public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register("LayoutMode", typeof(LayoutMode), typeof(RibbonBarPanel), new PropertyMetadata(LayoutMode.Table));

        #endregion

        #endregion

        #region Overrided

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (LayoutMode == LayoutMode.Flow)
            {
                this.ArrangeFlow(finalSize);
            }
            else
            {
                this.ArrangeTable(finalSize);
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var parentRbnBar = VisualUtils.FindAncestor(this, typeof(RibbonBar));

            if (parentRbnBar != null)
            {
                this.LayoutMode = ((RibbonBar)parentRbnBar).LayoutMode;
            }

            return this.LayoutMode == LayoutMode.Flow ? this.GetMeasureFlow(availableSize) : this.GetMeasureTable(availableSize);
        }

        #endregion

        #region Implementation

        #region Table Layout

        private void ArrangeTableLine(double x, double width, int start, int end, bool useItemU, double itemU)
        {
            double y = 0;
            UIElementCollection internalChildren = base.Children;
            for (int i = start; i < end; i++)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    Size size = new Size(element.DesiredSize.Width, element.DesiredSize.Height);
                    double height = (useItemU && double.IsNaN(itemU) == false) ? itemU : size.Height;
                    element.Arrange(new Rect(x, y, width, height));
                    y += height;
                }
            }
        }

        /// <summary>
        /// Arranges the table.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        private void ArrangeTable(Size finalSize)
        {
            int start = 0;
            double itemWidth = this.Width;
            double itemHeight = this.Height;
            double occupiedWidth = 0;

            Size occupiedLineSize = new Size();
            Size realSize = new Size(finalSize.Width, finalSize.Height);

            bool isItemWidth = !Double.IsNaN(itemWidth);
            bool isItemHeight = !Double.IsNaN(itemHeight);

            bool useItemU = isItemHeight;
            UIElementCollection internalChildren = base.Children;

            int i = 0;
            int count = Children.Count;

            while (i < count)
            {
                UIElement element = this.Children[i] as UIElement;
                if (element != null)
                {
                    Size elementSize = new Size(isItemWidth ? itemWidth : element.DesiredSize.Width, isItemHeight ? itemHeight : element.DesiredSize.Height);

                    if ((occupiedLineSize.Height + elementSize.Height) > realSize.Height)
                    {
                        this.ArrangeTableLine(occupiedWidth, occupiedLineSize.Width, start, i, useItemU, itemWidth);
                        occupiedWidth += occupiedLineSize.Width;
                        occupiedLineSize = elementSize;

                        start = i;
                    }
                    else
                    {
                        occupiedLineSize.Height += elementSize.Height;
                        occupiedLineSize.Width = Math.Max(elementSize.Width, occupiedLineSize.Width);
                    }
                }

                i++;
            }

            if (start < internalChildren.Count)
            {
                this.ArrangeTableLine(occupiedWidth, occupiedLineSize.Width, start, internalChildren.Count, useItemU, itemWidth);
            }
            /*int count = Children.Count;
            Rect rect = new Rect(new Point(0, 0), finalSize);
            double rowHeight = finalSize.Height / 3;
            double width = 0;
            int rows = 0;

            for (int i = 0; i < count; i++)
            {
                UIElement element = Children[i];
                if (element != null)
                {
                    var item = element as IRibbonItem;

                    Size desiredSize = element.DesiredSize;

                    if (item != null)
                    {
                        if (item.SizeForm == SizeForm.Large)
                        {
                            rect.X += width != 0 ? width + COLUMNSPAN : width;
                            rect.Width = width = desiredSize.Width;
                            rect.Height = finalSize.Height;
                            rect.Y = 0;
                            rows = 0;
                        }
                        else
                        {
                            if (rows == 3)
                            {
                                rect.Y = rows = 0;
                                rect.X += width + COLUMNSPAN;
                                rect.Width = width = desiredSize.Width;
                            }
                            else
                            {
                                if (rows == 0)
                                {
                                    if (width > 0)
                                    {
                                        rect.X += width + COLUMNSPAN;
                                        width = 0;
                                    }
                                }
                                else
                                {
                                    rect.Y += rowHeight;
                                }

                                width = Math.Max(width, desiredSize.Width);

                                rect.Width = desiredSize.Width;
                            }

                            rect.Height = rowHeight;

                            rows++;
                        }
                    }
                    if (element is RibbonGallery)
                    {
                        rect.X += width != 0 ? width + COLUMNSPAN : width;
                        rect.Width = width = desiredSize.Width;
                        rect.Height = finalSize.Height;
                        rect.Y = 0;
                        rows = 0;
                    }
                    else if (element is RibbonItemsGroup)
                    {
                        rect.X += width != 0 ? width + COLUMNSPAN : width;
                        rect.Width = width = desiredSize.Width;
                        rect.Height = desiredSize.Height;
                        rect.Y = 0;
                        rows = 0;
                    }
                    element.Arrange(rect);
                }
            }*/
        }
        
        /// <summary>
        /// Gets the measure table.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <returns></returns>
        private Size GetMeasureTable(Size availableSize)
        {
            Size occupiedLineSize = new Size();
            Size desiredSize = new Size();
            //if (double.IsInfinity(availableSize.Height)) availableSize.Height = 67;
            Size panelSize = new Size(availableSize.Width, availableSize.Height);

            double itemWidth = this.Width;
            double itemHeight = this.Height;
            bool isItemWidth = !Double.IsNaN(itemWidth);
            bool isItemHeight = !Double.IsNaN(itemHeight);

            Size realSize = new Size(isItemWidth ? itemWidth : availableSize.Width, isItemHeight ? itemHeight : availableSize.Height);
            UIElementCollection internalChildren = base.Children;

            int i = 0;
            int count = internalChildren.Count;

            while (i < count)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    element.Measure(realSize);
                    Size elementSize = new Size(isItemWidth ? itemWidth : element.DesiredSize.Width, isItemHeight ? itemHeight : element.DesiredSize.Height);

                    if ((occupiedLineSize.Height + elementSize.Height) > panelSize.Height)
                    {
                        desiredSize.Height = Math.Max(occupiedLineSize.Height, desiredSize.Height);
                        desiredSize.Width += occupiedLineSize.Width;
                        occupiedLineSize = elementSize;
                    }
                    else
                    {
                        occupiedLineSize.Height += elementSize.Height;
                        occupiedLineSize.Width = Math.Max(elementSize.Width, occupiedLineSize.Width);
                    }
                }

                i++;
            }

            desiredSize.Height = Math.Max(occupiedLineSize.Height, desiredSize.Height);
            desiredSize.Width += occupiedLineSize.Width;

            return new Size(desiredSize.Width, desiredSize.Height);
            //return new Size(desiredSize.Width, 91);
            /*int count = Children.Count;

            int cols = 0;
            int rows = 0;

            double totalWidth = 0;
            double colWidth = 0;

            double maxLitemHeight = 0;
            double maxSitemHeight = 0;

            for (int i = 0; i < count; i++)
            {
                UIElement element = Children[i];
                if (element != null)
                {
                    element.Measure(availableSize);

                    Size size = element.DesiredSize;
                    var item = element as IRibbonItem;
                    double itemWidth = size.Width;
                    double itemHeight = size.Height;
                    if (item != null)
                    {
                        if (item.SizeForm == SizeForm.Large)
                        {
                            if (rows > 0)
                            {
                                totalWidth += colWidth;

                                colWidth = 0;

                                rows = 0;

                                cols++;
                            }

                            if (itemHeight > maxLitemHeight)
                            {
                                maxLitemHeight = itemHeight;
                            }

                            totalWidth += itemWidth;
                            cols++;
                        }
                        else
                        {
                            if (rows == 3)
                            {
                                totalWidth += colWidth;
                                colWidth = 0;
                                rows = 0;
                                cols++;
                            }

                            if (colWidth < itemWidth)
                            {
                                colWidth = itemWidth;
                            }

                            if (itemHeight > maxSitemHeight)
                            {
                                maxSitemHeight = itemHeight;
                            }

                            rows++;
                        }
                    }
                    if (element is RibbonGallery || element is RibbonItemsGroup)
                    {
                        if (rows > 0)
                        {
                            totalWidth += colWidth;

                            colWidth = 0;

                            rows = 0;

                            cols++;
                        }

                        if (itemHeight > maxLitemHeight)
                        {
                            maxLitemHeight = itemHeight;
                        }

                        totalWidth += itemWidth;
                        cols++;
                    }
                }

            }

            maxSitemHeight *= 3;

            if (maxLitemHeight > maxSitemHeight)
            {
                maxSitemHeight = maxLitemHeight;
            }

            if (rows > 0)
            {
                totalWidth += colWidth;
                cols++;
            }

            if (cols > 0)
            {
                totalWidth += (cols - 1) * COLUMNSPAN;
            }

            return new Size(totalWidth, maxSitemHeight);*/
        }
        #endregion

        #region Flow Layout

        private void ArrangeFlowLine(double y, double height, int start, int end, bool useItemU, double itemU)
        {
            double x = 0;

            UIElementCollection internalChildren = base.Children;
            for (int i = start; i < end; i++)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    Size size = new Size(element.DesiredSize.Width, element.DesiredSize.Height);
                    double width = ( useItemU && double.IsNaN(itemU) == false) ? itemU : size.Width;
                    element.Arrange(new Rect(x, y, width, height));
                    x += width;
                }
            }
        }

        /// <summary>
        /// Arranges the flow.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        private void ArrangeFlow(Size finalSize)
        {
            int start = 0;
            double itemWidth = this.Width;
            double itemHeight = this.Height;
            double occupiedHeight = 0;

            Size occupiedLineSize = new Size();
            Size realSize = new Size(finalSize.Width, finalSize.Height);

            bool isItemWidth = !Double.IsNaN(itemWidth);
            bool isItemHeight = !Double.IsNaN(itemHeight);

            bool useItemU = isItemWidth;
            UIElementCollection internalChildren = base.Children;

            int i = 0;
            int count = Children.Count;

            i = 0;
            while (i < count)
            {
                UIElement element = Children[i] as UIElement;

                if (element != null)
                {
                    Size elementSize = new Size(isItemWidth ? itemWidth : element.DesiredSize.Width, isItemHeight ? itemHeight : element.DesiredSize.Height);

                    if ((occupiedLineSize.Width + elementSize.Width) > realSize.Width)
                    {
                        this.ArrangeFlowLine(occupiedHeight, occupiedLineSize.Height, start, i, useItemU, itemWidth);
                        occupiedHeight += occupiedLineSize.Height;
                        occupiedLineSize = elementSize;
                        start = i;
                    }
                    else
                    {
                        occupiedLineSize.Width += elementSize.Width;
                        occupiedLineSize.Height = Math.Max(elementSize.Height, occupiedLineSize.Height);
                    }
                }

                i++;
            }

            if (start < internalChildren.Count)
            {
                this.ArrangeFlowLine(occupiedHeight, occupiedLineSize.Height, start, internalChildren.Count, useItemU, itemWidth);
            }

            /*int count = this.Children.Count;
            Rect rect = new Rect(new Point(0, 0), new Size(0, this.GetMaxRowHeight(this.Children)));
            double maxWidthOfRow = finalSize.Width;
            double totalWidth = 0;
            double rowSpanSum = 0;
            double rowSpan = 0;

            rect.Y = rowSpan = (finalSize.Height - (rect.Height * 2)) / 4;

            if (rect.Y < 0)
            {
                rect.Y = 0;
                rowSpan = 0;
            }

            for (int i = 0; i < count; i++)
            {
                UIElement element = Children[i] as UIElement;

                Size desiredSize = element.DesiredSize;

                if ((totalWidth + desiredSize.Width + rowSpanSum) <= maxWidthOfRow)
                {
                    rect.X += rect.Width == 0 ? rect.Width : rect.Width + COLUMNSPAN;
                    totalWidth += rect.Width = desiredSize.Width;
                    rowSpanSum += COLUMNSPAN;
                }
                else
                {
                    rect.Y += rect.Height + (rowSpan * 2);
                    rect.Width = desiredSize.Width;
                    totalWidth = 0;
                    rowSpanSum = 0;
                    rect.X = 0;
                }

                element.Arrange(rect);
            }*/
        }

        /// <summary>
        /// Gets the measure flow.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <returns></returns>
        private Size GetMeasureFlow(Size availableSize)
        {
            this.MeasureChildren(availableSize);

            Size size = this.GetMaxSizeOfRows(this.Children);

            if (!double.IsInfinity(availableSize.Height))
            {
                size.Height = availableSize.Height;
            }
            else
            {
                if (size.Height < RibbonBar.MinHeightOfRibbonBar)
                {
                    size.Height = RibbonBar.MinHeightOfRibbonBar;
                }
            }

            return size;
        }
        #endregion

        /// <summary>
        /// Measures the children.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        private void MeasureChildren(Size availableSize)
        {
            foreach (UIElement ui in this.Children)
            {
                ui.Measure(availableSize);
                Size size = ui.DesiredSize;
            }
        }

        /// <summary>
        /// Gets the summary width of children.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <returns></returns>
        private double GetSummaryWidthOfChildren(UIElementCollection children)
        {
            double width = 0;

            foreach (UIElement ui in children)
            {
                width += ui.DesiredSize.Width;
            }

            return width;
        }

        /// <summary>
        /// Gets the max size of rows.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <returns></returns>
        private Size GetMaxSizeOfRows(UIElementCollection children)
        {
            double middleWidthOfRow = this.GetSummaryWidthOfChildren(children) / 2;
            double rowWidthFirst = 0.0;
            double rowWidthSecond = 0.0;
            double columnSpanSum = 0.0;
            double maxHeight = 0.0;
            int count = children.Count;
            int iTemp = 0;
            int i = 0;

            while (i < count)
            {
                Size desiredSize = children[i].DesiredSize;

                if (((rowWidthFirst + desiredSize.Width) <= middleWidthOfRow && count > 2) || i == 0)
                {
                    rowWidthFirst += desiredSize.Width;
                    columnSpanSum += COLUMNSPAN;

                    if (desiredSize.Height > maxHeight)
                    {
                        maxHeight = desiredSize.Height;
                    }

                    i++;
                }
                else
                {
                    break;
                }
            }

            iTemp = i;
            rowWidthFirst += columnSpanSum == 0 ? 0 : columnSpanSum - COLUMNSPAN;
            columnSpanSum = 0;

            while (i < count)
            {
                Size desiredSize = children[i++].DesiredSize;

                rowWidthSecond += desiredSize.Width;
                columnSpanSum += COLUMNSPAN;

                if (desiredSize.Height > maxHeight)
                {
                    maxHeight = desiredSize.Height;
                }
            }

            rowWidthSecond += columnSpanSum == 0 ? 0 : columnSpanSum - COLUMNSPAN;

            if (count > 2)
            {
                while (rowWidthFirst < rowWidthSecond)
                {
                    rowWidthFirst += children[iTemp].DesiredSize.Width + COLUMNSPAN;
                    rowWidthSecond -= children[iTemp].DesiredSize.Width + COLUMNSPAN;
                    iTemp++;
                }
            }
            else
            {
                rowWidthFirst = Math.Max(rowWidthFirst, rowWidthSecond);
            }

            return new Size(rowWidthFirst, count > 1 ? maxHeight * 2 : maxHeight);
        }

        /// <summary>
        /// Gets the height of the max row.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <returns></returns>
        private double GetMaxRowHeight(UIElementCollection children)
        {
            int count = children.Count;
            double max = 0.0;

            for (int i = 0; i < count; i++)
            {
                double height = children[i].DesiredSize.Height;

                if (max < height)
                {
                    max = height;
                }
            }

            return max;
        }

        #endregion

        #region Fields

        private const double COLUMNSPAN = 4;

        #endregion
    }
}
