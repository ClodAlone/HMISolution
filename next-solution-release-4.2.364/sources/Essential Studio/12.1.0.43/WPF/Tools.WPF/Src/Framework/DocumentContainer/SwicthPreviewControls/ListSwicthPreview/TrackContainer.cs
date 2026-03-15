// <copyright file="TrackContainer.cs" company="Syncfusion">
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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents track container.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TrackContainer : Border
    {
        #region Private members
        /// <summary>
        /// Empty point.
        /// </summary>
        private readonly Point EMPTY_POINT = new Point(0, 0);

        /// <summary>
        /// Offset of preview window.
        /// </summary>
        private double m_offset = 0;
        #endregion

        #region Implementation
        /// <summary>
        /// Arranges the contents of a <see cref="T:System.Windows.Controls.Border"/> element.
        /// </summary>
        /// <param name="finalSize">The <see cref="T:System.Windows.Size"/> this element uses to arrange its child element.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.Border"/> element and its child element.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement child = (FrameworkElement)Child;

            if (null != child)
            {
                Point startPoint;
                Point endPoint;
                double cHeight = child.Height;
                double middle = child.Height / 2;
                double fWidth = finalSize.Width;
                double fHeight = finalSize.Height;

                if (m_offset < middle)
                {
                    startPoint = new Point(0, 0);
                    endPoint = new Point(fWidth, cHeight);
                }
                else if (m_offset > fHeight - middle)
                {
                    startPoint = new Point(0, fHeight - cHeight);
                    endPoint = new Point(fWidth, fHeight);
                }
                else
                {
                    startPoint = new Point(0, m_offset - middle);
                    endPoint = new Point(fWidth, m_offset + middle);
                }

                Rect rect = new Rect(startPoint, endPoint);
                child.Arrange(rect);
                child.BringIntoView();
                return finalSize;
            }
            else
            {
                return base.ArrangeOverride(finalSize);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ListBox tParent = TemplatedParent as ListBox;

            if (null != tParent)
            {
                tParent.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            }
            else
            {
                throw new NotSupportedException("This control can be used in ListBox only!");
            }
        }

        /// <summary>
        /// Called when [selection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                ListBox senderList = (ListBox)sender;
                UIElement item = (UIElement)senderList.SelectedItem;

                if (null != item)
                {
                    Point mainPoint = VisualUtils.PointToScreen(this, EMPTY_POINT);
                    Visual visualParent = (Visual)VisualTreeHelper.GetParent(item);

                    if (visualParent != null)
                    {
                        Point itemPoint = VisualUtils.PointToScreen(item, EMPTY_POINT);
                        m_offset = itemPoint.Y - mainPoint.Y + item.RenderSize.Height / 2;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                m_offset = 0;
            }

            InvalidateArrange();
        }
        #endregion
    }
}
