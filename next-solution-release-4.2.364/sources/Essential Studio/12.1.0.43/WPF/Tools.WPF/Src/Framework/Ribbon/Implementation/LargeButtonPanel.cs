// <copyright file="LargeButtonPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
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
    public class LargeButtonPanel : Panel
    {
        #region Constants
        /// <summary>
        /// Height of the panel in pixels.
        /// </summary>
        public const int C_PanelHeight = 67;
        #endregion

        #region Dependency Properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Identifies Item width. This is dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(LargeButtonPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), new ValidateValueCallback(LargeButtonPanel.IsWidthHeightValid));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Identifies Item height. This is dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(LargeButtonPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), new ValidateValueCallback(LargeButtonPanel.IsWidthHeightValid));

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets item height.
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
        /// Gets or sets item width.
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

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeButtonPanel"/> class.
        /// </summary>
        public LargeButtonPanel()
        {
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Arranges the line.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="width">The width value.</param>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="useItemU">if set to <c>true</c> [use item U].</param>
        /// <param name="itemU">The item U.</param>
        private void ArrangeLine(double x, double width, int start, int end, bool useItemU, double itemU)
        {
            UIElementCollection internalChildren = base.InternalChildren;
            bool isTwoItem = false;
            int itemCount = 0;

            for (int i = start; i < end; i++)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    if (element is RibbonButton && (element as RibbonButton).SizeForm == SizeForm.Small)
                    {
                        itemCount++;
                    }
                }
            }

            if (itemCount == 2)
                isTwoItem = true;

            double y = 0;

            if (isTwoItem)
                y = 10;
            
            for (int i = start; i < end; i++)
            {
                UIElement element = internalChildren[i];
                if (element != null)
                {
                    Size size = new Size(element.DesiredSize.Width, element.DesiredSize.Height);
                    double height = useItemU ? itemU : size.Height;
                    element.Arrange(new Rect(x, y, width, height));
                    y += height;
                    if (isTwoItem)
                        y += 10;
                }
            }
        }

        /// <summary>
        /// Positions child elements and determines a size for a panel.
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
            double occupiedWidth = 0;

            Size occupiedLineSize = new Size();
            Size realSize = new Size(finalSize.Width, finalSize.Height);

            bool isItemWidth = !Double.IsNaN(itemWidth);
            bool isItemHeight = !Double.IsNaN(itemHeight);

            bool useItemU = isItemHeight;
            UIElementCollection internalChildren = base.InternalChildren;

            int i = 0;
            int count = VisualChildrenCount;

            while (i < count)
            {
                UIElement element = GetVisualChild(i) as UIElement;
                if (element != null)
                {
                    Size elementSize = new Size(isItemWidth ? itemWidth : element.DesiredSize.Width, isItemHeight ? itemHeight : element.DesiredSize.Height);

                    if ((occupiedLineSize.Height + elementSize.Height) > realSize.Height)
                    {
                        this.ArrangeLine(occupiedWidth, occupiedLineSize.Width, start, i, useItemU, itemWidth);
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
                this.ArrangeLine(occupiedWidth, occupiedLineSize.Width, start, internalChildren.Count, useItemU, itemWidth);
            }

            //if (VisualChildrenCount == 1 && (internalChildren[0] is RibbonButton || internalChildren[0] is DropDownButton))
            //{
            //    MaxWidth = finalSize.Width;
            //}
            return finalSize;
        }

        /// <summary>
        /// Determines whether value is valid.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        /// True if value is valid; otherwise false.
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
        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for a panel.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to child
        /// elements. Infinity can be
        /// specified as a value to indicate
        /// that the element will size to
        /// whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(availableSize.Width, availableSize.Height);
            return MeasureElements(ref size);
        }

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for a panel.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to child
        /// elements. Infinity can be
        /// specified as a value to indicate
        /// that the element will size to
        /// whatever content is available.</param>
        /// <returns>
        /// Needed size for child elements.
        /// </returns>
        private Size MeasureElements(ref Size availableSize)
        {
            Size occupiedLineSize = new Size();
            Size desiredSize = new Size();
            Size panelSize = new Size(availableSize.Width, availableSize.Height);

            double itemWidth = this.ItemWidth;
            double itemHeight = this.ItemHeight;
            bool isItemWidth = !Double.IsNaN(itemWidth);
            bool isItemHeight = !Double.IsNaN(itemHeight);

            Size realSize = new Size(isItemWidth ? itemWidth : availableSize.Width, isItemHeight ? itemHeight : availableSize.Height);
            UIElementCollection internalChildren = base.InternalChildren;

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
        }

        /// <summary>
        /// Gets visual child by index.
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

        /// <summary>
        /// Gets count of visual children.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount;
            }
        }
        #endregion
    }
}
