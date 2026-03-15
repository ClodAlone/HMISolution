// <copyright file="CustomWrapPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents panel that replaces in ItemsPanelTemplate of Command Menu. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CustomWrapPanel : WrapPanel
    {
        #region Private members
        /// <summary>
        /// Represents Corner panel
        /// </summary>
        private CornerPanel m_cornerPanel = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the corner panel.
        /// </summary>
        /// <value>The corner panel.</value>
        private CornerPanel CornerPanel
        {
            get
            {
                if (null == m_cornerPanel)
                {
                    m_cornerPanel = new CornerPanel();
                    AddLogicalChild(m_cornerPanel);
                    AddVisualChild(m_cornerPanel);
                }

                return m_cornerPanel;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the number of child elements for the control.
        /// </summary>
        /// <returns>An Int32 value that represents the number of child elements.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                int count = base.VisualChildrenCount;
                return (count >= 1) ? (count + 1) : count;
            }
        }
        
        /// <summary>
        /// Gets a <see cref="T:System.Windows.Media.Visual"/> child of this <see cref="T:System.Windows.Controls.Panel"/> at the specified index position.
        /// </summary>
        /// <param name="index">The index position of the <see cref="T:System.Windows.Media.Visual"/> child.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.Visual"/> child of the parent <see cref="T:System.Windows.Controls.Panel"/> element.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (VisualChildrenCount - 1 == index)
            {
                return CornerPanel;
            }
            else
            {
                return base.GetVisualChild(index);
            }
        }
        
        /// <summary>
        /// Arranges the content of a <see cref="T:System.Windows.Controls.WrapPanel"/> element.
        /// </summary>
        /// <param name="finalSize">The <see cref="T:System.Windows.Size"/> that this element should use to arrange its child elements.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.WrapPanel"/> element and its children.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int start = 0;
            double y = 0.0;
            Size longSize = new Size(0, 0);
            bool isWidthNaN = !double.IsNaN(ItemWidth);
            bool isHeightNaN = !double.IsNaN(ItemHeight);

            Size bSize = CornerPanel.DesiredSize;
            Rect rect = new Rect(new Point(finalSize.Width - bSize.Width, finalSize.Height - bSize.Height), bSize);
            CornerPanel.Arrange(rect);
            Size aviableSize = finalSize;

            int end = 0;
            int count = InternalChildren.Count;
            int lineCount = GetLineCount(isWidthNaN, finalSize.Width, CornerPanel.ActualWidth, count);
            int line = 1;

            if (1 == lineCount)
            {
                aviableSize.Width -= bSize.Width;
            }

            while (end < count)
            {
                UIElement element = InternalChildren[end];

                if (element != null)
                {
                    Size elementSize = new Size(isWidthNaN ? ItemWidth : element.DesiredSize.Width, isHeightNaN ? ItemHeight : element.DesiredSize.Height);

                    if (longSize.Width + elementSize.Width > aviableSize.Width)
                    {
                        ArrangeLine(y, longSize.Height, start, end, isWidthNaN);
                        y += longSize.Height;
                        longSize = elementSize;
                        start = end;

                        if (++line == lineCount)
                        {
                            aviableSize.Width -= bSize.Width;
                        }
                    }
                    else
                    {
                        longSize.Width += elementSize.Width;
                        longSize.Height = Math.Max(elementSize.Height, longSize.Height);
                    }
                }

                ++end;
            }

            if (start < count)
            {
                ArrangeLine(y, longSize.Height, start, InternalChildren.Count, isWidthNaN);
            }

            return finalSize;
        }
        
        /// <summary>
        /// Measures the child elements of a <see cref="T:System.Windows.Controls.WrapPanel"/> in anticipation of arranging them during the <see cref="M:System.Windows.Controls.WrapPanel.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the desired size of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            CornerPanel.Measure(constraint);
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Arranges the line.
        /// </summary>
        /// <param name="y">The y ArrangeLine.</param>
        /// <param name="height">The height ArrangeLine.</param>
        /// <param name="start">The start ArrangeLine.</param>
        /// <param name="end">The end ArrangeLine.</param>
        /// <param name="isWidthNaN">if set to <c>true</c> [is width na N].</param>
        private void ArrangeLine(double y, double height, int start, int end, bool isWidthNaN)
        {
            double x = 0.0;

            for (int i = start; i < end; i++)
            {
                UIElement element = InternalChildren[i];

                if (element != null)
                {
                    Size size = new Size(element.DesiredSize.Width, element.DesiredSize.Height);
                    double width = isWidthNaN ? ItemWidth : size.Width;
                    element.Arrange(new Rect(x, y, width, height));
                    x += width;
                }
            }
        }
        
        /// <summary>
        /// Gets the line count.
        /// </summary>
        /// <param name="isWidthNaN">if set to <c>true</c> [is width na N].</param>
        /// <param name="finalSizeWidth">Final width of the size.</param>
        /// <param name="indent">The indent.</param>
        /// <param name="cnt">The items count.</param>
        /// <returns>The line count.</returns>
        private int GetLineCount(bool isWidthNaN, double finalSizeWidth, double indent, int cnt)
        {
            int line = 1;
            double longSizeWidth = indent;

            for (int i = cnt - 1; i > -1; --i)
            {
                UIElement element = InternalChildren[i];
                double width = isWidthNaN ? ItemWidth : element.DesiredSize.Width;

                if (longSizeWidth + width > finalSizeWidth)
                {
                    longSizeWidth = width;
                    ++line;
                }
                else
                {
                    longSizeWidth += width;
                }
            }

            return line;
        }
        #endregion
    }
}