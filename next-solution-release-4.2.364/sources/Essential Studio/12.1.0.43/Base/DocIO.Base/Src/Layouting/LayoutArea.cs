#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !SILVERLIGHT

using System;
using Syncfusion.DocIO.DLS;
using System.Drawing;

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Class representing the layout area.
    /// </summary>
    internal class LayoutArea
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private RectangleF m_area;

        /// <summary>
        /// 
        /// </summary>
        private RectangleF m_clientArea;

        /// <summary>
        /// 
        /// </summary>
        private RectangleF m_clientActiveArea;

        /// <summary>
        /// 
        /// </summary>
        private ILayoutSpacingsInfo m_spacings = null;

        /// <summary>
        /// 
        /// </summary>
        private bool m_bSkipSubtractWhenInvalidParameter = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get
            {
                return OuterArea.Width;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get
            {
                return OuterArea.Height;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [skips subtract when invalid parameter].
        /// </summary>
        /// <value>
        /// true if [skips subtract when invalid parameter]; otherwise, false.
        /// </value>
        public bool SkipSubtractWhenInvalidParameter
        {
            get
            {
                return m_bSkipSubtractWhenInvalidParameter;
            }

            set
            {
                m_bSkipSubtractWhenInvalidParameter = value;
            }
        }

        /// <summary>
        /// Gets the margins.
        /// </summary>
        /// <value>The margins.</value>
        public Spacings Margins
        {
            get
            {
                return m_spacings.Margins;
            }
        }

        /// <summary>
        /// Gets the paddings.
        /// </summary>
        /// <value>The paddings.</value>
        public Spacings Paddings
        {
            get
            {
                return m_spacings.Paddings;
            }
        }

        /// <summary>
        /// Gets the outer area.
        /// </summary>
        /// <value>The outer area.</value>
        public RectangleF OuterArea
        {
            get
            {
                return m_area;
            }
        }

        /// <summary>
        /// Gets the client area.
        /// </summary>
        /// <value>The client area.</value>
        public RectangleF ClientArea
        {
            get
            {
                return m_clientArea;
            }
        }

        /// <summary>
        /// Gets the client active area.
        /// </summary>
        /// <value>The client active area.</value>
        public RectangleF ClientActiveArea
        {
            get
            {
                return m_clientActiveArea;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutArea"/> class.
        /// </summary>
        public LayoutArea()
            : this(new RectangleF(), null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutArea"/> class.
        /// </summary>
        /// <param name="area">The area.</param>
        public LayoutArea(RectangleF area)
            : this(area, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutArea"/> class.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="spacings">The spacings.</param>
        public LayoutArea(RectangleF area, ILayoutSpacingsInfo spacings, IWidget widget)
        {
            m_area = area;
            m_spacings = spacings;
            UpdateClientArea(widget);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Cuts from left.
        /// </summary>
        /// <param name="x">The x.</param>
        public void CutFromLeft(double x)
        {
            if (x < m_clientActiveArea.Left || x > m_clientActiveArea.Right)
            {
                if (SkipSubtractWhenInvalidParameter)
                //return;
                {
                    if (x < m_clientActiveArea.Left)
                        x = m_clientActiveArea.Left;
                }
                else
                    throw new ArgumentException("x");
            }

            RectangleF rect = m_clientActiveArea;
            rect.Width = (float)(rect.Right - x);
            if (rect.Width < 0)
                rect.Width = 0;
            rect.X = (float)x;
            m_clientActiveArea = rect;
        }

        /// <summary>
        /// Cuts from top.
        /// </summary>
        /// <param name="y">The y.</param>
        public void CutFromTop(double y)
        {
            CutFromTop(y, 0);
        }
        /// <summary>
        /// Cuts from top.
        /// </summary>
        /// <param name="y">The y.</param>
        public void CutFromTop(double y,float footnoteHeight)
        {
            if (y < m_clientActiveArea.Top || y > m_clientActiveArea.Bottom)
            {
                if (SkipSubtractWhenInvalidParameter)
                {
                    if (y < m_clientActiveArea.Top)
                        y = m_clientActiveArea.Top;
                    else if (y > m_clientActiveArea.Bottom)
                        y = m_clientActiveArea.Bottom;
                }
                else
                    throw new ArgumentException("y");
            }

            RectangleF rect = m_clientActiveArea;
            rect.Height = (float)(rect.Bottom - y - footnoteHeight);
            rect.Y = (float)y;
            m_clientActiveArea = rect;
        }

        /// <summary>
        /// Cuts from top.
        /// </summary>
        public void CutFromTop()
        {
            CutFromTop(ClientActiveArea.Bottom);
        }

        /// <summary>
        /// Updates the client area.
        /// </summary>
        private void UpdateClientArea(IWidget widget)
        {
            double leftPad = 0f;
            double topPad = 0f;
            double rightPad = 0f;
            double bottomPad = 0f;

            if (m_spacings != null)
            {
                leftPad = Margins.Left + Paddings.Left;
                topPad = Margins.Top + Paddings.Top;
                rightPad = Margins.Right + Paddings.Right;
                bottomPad = Paddings.Bottom;
            }

            double left = m_area.X;

            if (left < 0)
            {
                left = m_area.X;
            }
            WParagraph paragraph = (widget is WParagraph) ? (widget as WParagraph)
                : (widget is SplitWidgetContainer && (widget as SplitWidgetContainer).RealWidgetContainer is WParagraph)
                ? ((widget as SplitWidgetContainer).RealWidgetContainer as WParagraph) : null;
            if (paragraph != null)
            {
                leftPad = Margins.Left;
                rightPad = Margins.Right;
                if (paragraph.IsInCell && !((paragraph.OwnerTextBody as WTableCell).m_layoutInfo as TableLayoutInfo).IsExactlyRowHeight)
                    bottomPad += Margins.Bottom;
            }
            double top = m_area.Y;
            double width = m_area.Width;
            double height = m_area.Height;
            if (widget != null && widget is WTableCell && widget.LayoutInfo.IsVerticalText)
            {
                left = m_area.X + topPad;
                top = m_area.Y + leftPad;
                width = m_area.Width - topPad - bottomPad;
                height = m_area.Height - leftPad;//No need to add the Right padding of the cell height when the cell is having TextDirection as vertical.
            }
            else
            {
                left = m_area.X + leftPad;
                top = m_area.Y + topPad;
                width = m_area.Width - leftPad - rightPad;
                height = m_area.Height - topPad - bottomPad;
            }
            //Update client area based on cell Spacing
            if ((widget is WTableCell) && (widget as WTableCell).OwnerRow.OwnerTable.TableFormat.CellSpacing > 0)
            {
                width += ((widget as WTableCell).OwnerRow.OwnerTable.TableFormat.CellSpacing * 2);
                left -= (widget as WTableCell).OwnerRow.OwnerTable.TableFormat.CellSpacing;
            }
            if (width < 0)
                width = 0;
            if (height < 0)
                height = 0;
            //Update Exact client area
            left = Math.Round(left, 2);
            top = Math.Round(top, 2);
            width = Math.Round(width, 2);
            height = Math.Round(height, 2);

            m_clientArea = new RectangleF((float)left, (float)top, (float)width, (float)height);
            m_clientActiveArea = m_clientArea;
        }
        /// <summary>
        /// 
        /// </summary>
        internal void UpdateBounds(float topPad)
        {
            float diff = (float)Math.Abs(topPad - (Margins.Top + Paddings.Top));
            m_clientArea.Y += diff;
            m_clientArea.Height -= diff;
            m_clientActiveArea = m_clientArea;
        }
        /// <summary>
        /// Update Bounds based on Text Wrap
        /// </summary>
        /// <param name="textBoxBounds"></param>
        /// <param name="layoutInfo"></param>
        internal void UpdateBoundsBasedOnTextWrap(RectangleF textBoxBounds)
        {
            float diff = textBoxBounds.Bottom - m_clientArea.Y;
            m_clientArea.Y = textBoxBounds.Bottom;
            m_clientArea.Height = m_clientArea.Height - diff;
            m_clientActiveArea = m_clientArea;
        }
        /// <summary>
        /// Updates the Width
        /// </summary>
        /// <param name="width"></param>
        internal void UpdateWidth()
        {
            m_clientArea.Width = LayoutContext.MAX_WIDTH - (m_clientActiveArea.X - m_area.X);
            m_clientActiveArea.Width = m_clientArea.Width;
        }
        #endregion
    }
}

#endif