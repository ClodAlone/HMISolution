// <copyright file="GroupPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is responsible for layout of the ribbon bar
    /// content.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GroupPanel : Panel
    {
        #region Constants
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines panel height.
        /// </summary>
        public const int C_PanelHeight = 67;
        #endregion

        #region Private structures
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// ElementInfo storage structure.
        /// </summary>
        private struct ElementInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ElementInfo"/> struct.
            /// </summary>
            /// <param name="size">The size value</param>
            /// <param name="index">The index value.</param>
            public ElementInfo(Size size, int index)
            {
                m_ElementSize = size;
                m_Index = index;
                m_Added = false;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ElementInfo"/> struct.
            /// </summary>
            /// <param name="element">The element.</param>
            /// <param name="added">if set to <c>true</c> [added].</param>
            public ElementInfo(ElementInfo element, bool added)
            {
                this = element;
                m_Added = added;
            }

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Defines element size.
            /// </summary>
            public Size m_ElementSize;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Defines whether element is added.
            /// </summary>
            public bool m_Added;

            /// <property name="flag" value="Finished" />
            /// <summary>
            /// Defines element index.
            /// </summary>
            public int m_Index;
        }
        #endregion

        #region Members
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Using a DependencyProperty as the backing store for
        /// LauncherButton. This enables animation, styling, binding data
        /// etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(GroupPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), new ValidateValueCallback(GroupPanel.IsWidthHeightValid));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Using a DependencyProperty as the backing store for
        /// LauncherButton. This enables animation, styling, binding data
        /// etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(GroupPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), new ValidateValueCallback(GroupPanel.IsWidthHeightValid));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Using a DependencyProperty as the backing store for
        /// LauncherButton. This enables animation, styling, binding data
        /// etc...
        /// </summary>
        public static readonly DependencyProperty PanelStateProperty = DependencyProperty.Register("PanelState", typeof(RibbonBarState), typeof(GroupPanel), new FrameworkPropertyMetadata(RibbonBarState.TwoRow, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPanelStateChanged)));

        /// <summary>
        /// Represents the ItemMarginProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(GroupPanel), new UIPropertyMetadata(new Thickness(2, 8, 1, 0)));
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the item margin.
        /// </summary>
        /// <value>The item margin.</value>
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)GetValue(ItemMarginProperty);
            }

            set
            {
                SetValue(ItemMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets item height value.
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double ItemHeight
        {
            get
            {
                return (double)base.GetValue(ItemHeightProperty);
            }

            set
            {
                base.SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets item width value.
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double ItemWidth
        {
            get
            {
                return (double)base.GetValue(ItemWidthProperty);
            }

            set
            {
                base.SetValue(ItemWidthProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets RibbonBar PanelState.
        /// </summary>
        public RibbonBarState PanelState
        {
            get
            {
                return (RibbonBarState)base.GetValue(PanelStateProperty);
            }

            set
            {
                base.SetValue(PanelStateProperty, value);
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupPanel"/> class.
        /// </summary>
        public GroupPanel()
        {
        }
        #endregion

        #region Implementation
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Arrange all items in one line.
        /// </summary>
        /// <param name="y">y coordinate.</param>
        /// <param name="height">Height of the line.</param>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index.</param>
        /// <param name="useItemU">Defines whether to use itemU
        /// parameter.</param>
        /// <param name="itemU">Element width.</param>
        private void ArrangeLine(double y, double height, int start, int end, bool useItemU, double itemU)
        {
            double x = 0;

            UIElementCollection internalChildren = base.InternalChildren;
            for (int i = start; i < end; i++)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    Size size = new Size(element.DesiredSize.Width, element.DesiredSize.Height);
                    double width = useItemU ? itemU : size.Width;
                    element.Arrange(new Rect(x, y, width, height));
                    x += width;
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Positions child elements.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int start = 0;
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            double occupiedHeight = 0;

            Size occupiedLineSize = new Size();
            Size realSize = new Size(finalSize.Width, finalSize.Height);

            bool isItemWidth = !Double.IsNaN(itemWidth);
            bool isItemHeight = !Double.IsNaN(itemHeight);

            bool useItemU = isItemWidth;
            UIElementCollection internalChildren = base.InternalChildren;

            int i = 0;
            int count = VisualChildrenCount;

            i = 0;
            while (i < count)
            {
                UIElement element = GetVisualChild(i) as UIElement;

                if (element != null)
                {
                    Size elementSize = new Size(isItemWidth ? itemWidth : element.DesiredSize.Width, isItemHeight ? itemHeight : element.DesiredSize.Height);

                    if ((occupiedLineSize.Width + elementSize.Width) > realSize.Width)
                    {
                        this.ArrangeLine(occupiedHeight, occupiedLineSize.Height, start, i, useItemU, itemWidth);
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
                this.ArrangeLine(occupiedHeight, occupiedLineSize.Height, start, internalChildren.Count, useItemU, itemWidth);
            }
            return finalSize;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Checks if value is valid.
        /// </summary>
        /// <param name="value">Value needed to check.</param>
        /// <returns>
        /// True if value is valid; false if not.
        /// </returns>
        private static bool IsWidthHeightValid(object value)
        {
            double num = (double)value;
            if (Double.IsNaN(num))
            {
                return true;
            }

            if (num >= 0)
            {
                return !double.IsPositiveInfinity(num);
            }

            return false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to the child.
        /// Infinity can be specified as a
        /// value to indicate that the element
        /// will size to whatever content is
        /// available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of children's sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            bool isItemWidth = !Double.IsNaN(itemWidth);
            bool isItemHeight = !Double.IsNaN(itemHeight);
            int i = 0;
            int count = InternalChildren.Count;
            double totalWidth = 0d;
            double averageRowWidth = 0d;
            double firstRowWidht = 0d;
            double secondRowWidht = 0d;
            double neededWidth = 0d;
            List<ElementInfo> elementSizes = new List<ElementInfo>();
            ElementInfo currentElement;

            foreach (FrameworkElement el in InternalChildren)
            {
                el.Margin = ItemMargin;
            }

            Size realSize = new Size(isItemWidth ? itemWidth : availableSize.Width, isItemHeight ? itemHeight : availableSize.Height);
            UIElementCollection internalChildren = base.InternalChildren;
            while (i < count)
            {
                UIElement element = internalChildren[i];

                if (element != null)
                {
                    element.Measure(realSize);
                    Size elementSize = new Size(isItemWidth ? itemWidth : element.DesiredSize.Width, isItemHeight ? itemHeight : element.DesiredSize.Height);
                    elementSizes.Add(new ElementInfo(elementSize, i));
                    totalWidth += elementSize.Width;
                }

                i++;
            }

            averageRowWidth = totalWidth / 2;
            //elementSizes.Sort(CompareByWidth);
            i = 0;
            while (firstRowWidht <= averageRowWidth && i < count)
            {
                currentElement = elementSizes[i];
                if (firstRowWidht + currentElement.m_ElementSize.Width <= averageRowWidth)
                {
                    firstRowWidht += currentElement.m_ElementSize.Width;
                    elementSizes[i] = new ElementInfo(currentElement, true);
                }
                else
                    break;
                i++;
            }

            i = 0;
            while (firstRowWidht <= averageRowWidth && i < count)
            {
                currentElement = elementSizes[i];
                if (!currentElement.m_Added)
                {
                    if (secondRowWidht + currentElement.m_ElementSize.Width <= averageRowWidth)
                    {
                        secondRowWidht += currentElement.m_ElementSize.Width;
                        elementSizes[i] = new ElementInfo(currentElement, true);
                    }
                    else
                        break;
                }

                i++;
            }

            foreach (ElementInfo element in elementSizes)
            {
                if (!element.m_Added)
                {
                    neededWidth += element.m_ElementSize.Width;
                }
            }

            double delta = neededWidth - (averageRowWidth -  secondRowWidht);
            if (delta > 0)
            {
                delta += 6;
            }

            Size occupiedLineSize = new Size();
            int start = 0, rows = 0;
            double occupiedHeight = 0;
            bool useItemU = isItemWidth;
            int j = 0;

            //while (j < count)
            //{
            //    UIElement element = GetVisualChild(j) as UIElement;

            //    if (element != null)
            //    {
            //        Size elementSize = new Size(isItemWidth ? itemWidth : element.DesiredSize.Width, isItemHeight ? itemHeight : element.DesiredSize.Height);

            //        if ((occupiedLineSize.Width + elementSize.Width) > (averageRowWidth + delta))
            //        {
            //            occupiedHeight += occupiedLineSize.Height;
            //            occupiedLineSize = elementSize;
            //            rows++;
            //            start = j;
            //        }
            //        else
            //        {
            //            occupiedLineSize.Width += elementSize.Width;
            //            occupiedLineSize.Height = Math.Max(elementSize.Height, occupiedLineSize.Height);
            //        }
            //    }

            //    j++;
            //}

            //if (start < internalChildren.Count)
            //{
            //    rows++;
            //}

            //if (rows > 2)
            //{
            //    UIElement element = internalChildren[internalChildren.Count - 1];
            //    delta += (element.DesiredSize.Width+6);
            //}

            return new Size(delta+ averageRowWidth, availableSize.Height);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Compares two elements by width.
        /// </summary>
        /// <param name="x">First element.</param>
        /// <param name="y">Second element.</param>
        /// <returns>
        /// 0 if equal; -1 x is less then y; 1 if x is greater then y.
        /// </returns>
        private static int CompareByWidth(ElementInfo x, ElementInfo y)
        {
            if (x.m_ElementSize.Width == y.m_ElementSize.Width)
            {
                return 0;
            }
            else
            {
                if (x.m_ElementSize.Width < y.m_ElementSize.Width)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Returns a child at the specified index from that element's
        /// collection of child elements.
        /// </summary>
        /// <param name="index">The index of the visual object in the
        /// VisualCollection.</param>
        /// <returns>
        /// The child in the VisualCollection at the specified index
        /// value.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return base.GetVisualChild(index);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnPanelStateChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="obj">Dependency object, the change occurs on.</param>
        /// <param name="args">Property changes details, such as old value
        /// and new value.</param>
        private static void OnPanelStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            GroupPanel panel = obj as GroupPanel;
            switch (panel.PanelState)
            {
                case RibbonBarState.TwoRow:
                    {
                        break;
                    }

                case RibbonBarState.Collapsed:
                    {
                        break;
                    }
            }
        }

        #endregion
    }
}
