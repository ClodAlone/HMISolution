// <copyright file="SfTabPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Controls.Navigation
{
    /// <summary>
    /// Represents a panel for displaying the <see 
    /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/> elements.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class SfTabPanel : Panel, IDisposable
    {

        /// <summary>
        /// Structure for ScrollInfo.
        /// </summary>
        internal struct ScrollInfo
        {
            /// <summary>
            /// Stores bool value based on whether the Scroll button should be displayed
            /// </summary>
            public bool NeedScrollButtonsShow;

            /// <summary>
            /// Stores the first trimmed tab index.
            /// </summary>
            public int FirstTrimmedTabIndex;

            /// <summary>
            /// Stores the last trimmed tab index.
            /// </summary>
            public int LastTrimmedTabIndex;

            /// <summary>
            /// Stores the desired width.
            /// </summary>
            public double DesiredWidth;

            /// <summary>
            /// Stores the desired height.
            /// </summary>
            public double DesiredHeight;

            /// <summary>
            /// Stores the last tab trimmed width.
            /// </summary>
            public double LastTabTrimmedWidth;

            /// <summary>
            /// Stores the first tab trimmed width.
            /// </summary>
            public double FirstTabTrimmedWidth;

            /// <summary>
            /// Stores the AllTrimmed width.
            /// </summary>
            public double AllTrimmedWidth;

            /// <summary>
            /// Stores the last tab trimmed height.
            /// </summary>
            public double LastTabTrimmedHeight;

            /// <summary>
            /// Stores the first tab trimmed height.
            /// </summary>
            public double FirstTabTrimmedHeight;

            /// <summary>
            /// Stores the AllTrimmed height.
            /// </summary>
            public double AllTrimmedHeight;

            /// <summary>
            /// Stores the offset.
            /// </summary>
            public double Offset;
        }

        #region Private members

        /// <summary>
        /// Stores the scroll information.
        /// </summary>
        internal SfTabPanel.ScrollInfo m_scrollInfo;

        private double previouswidth, previousheight = 0;

        internal bool m_updatescrollinfo = true;

        internal bool m_updatetabindex = false;

        internal double prevHeight = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the scroll offset.
        /// </summary>
        /// <value>The scroll offset.</value>
        internal double ScrollOffset
        {
            get
            {
                return (double)GetValue(ScrollOffsetProperty);
            }

            set
            {
                SetValue(ScrollOffsetProperty, value);
            }
        }

        private SfTabControl parentItemsControl;

        internal SfTabControl ParentItemsControl
        {
            get
            {
                if (parentItemsControl == null)
                {
                    parentItemsControl = ItemsControl.GetItemsOwner(this) as SfTabControl;
                }
                return parentItemsControl;
            }
            set { parentItemsControl = value; }
        }
        #endregion
        
        #region Dependency properties
        /// <summary>
        /// Represents the ScrollOffset Dependency Property
        /// </summary>
        internal static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.Register("ScrollOffset", typeof(double), typeof(SfTabPanel), new PropertyMetadata(0d, new PropertyChangedCallback(OnScrollOffsetChanged)));

        #endregion
        
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SfTabPanel"/> class.
        /// </summary>
        public SfTabPanel()
        {
            m_scrollInfo = new ScrollInfo();
            Loaded += SfTabPanel_Loaded;
        }

        public void Dispose()
        {
            this.Loaded -= SfTabPanel_Loaded;
            this.LayoutUpdated -= SfTabPanel_LayoutUpdated;
            ParentItemsControl = null;
        }

void SfTabPanel_Loaded(object sender, RoutedEventArgs e)
        {
            this.LayoutUpdated -= SfTabPanel_LayoutUpdated;
            this.LayoutUpdated += SfTabPanel_LayoutUpdated;
        }
        
        void SfTabPanel_LayoutUpdated(object sender, object e)
        {
            if (this.ActualWidth > 0 && this.ActualHeight > 0 && previouswidth!=ActualWidth && previousheight != ActualHeight)
            {
                previouswidth = ActualWidth;
                previousheight = ActualHeight;
                if(Name.Equals("PinnedPanel"))
                    ParentItemsControl.CheckPinnedNavigationButtonVisibility(m_scrollInfo.NeedScrollButtonsShow, this.ActualWidth);
                else
                    ParentItemsControl.CheckNavigationButtonVisibility(m_scrollInfo.NeedScrollButtonsShow, this.ActualWidth);
            }
        }
        #endregion

        #region Implementation

        private static void OnScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfTabPanel panel = d as SfTabPanel;
            panel.m_scrollInfo.Offset = (double)e.NewValue;
            panel.ValidateScrollOffset(panel.ParentItemsControl.TabStripPlacement == TabStripPlacement.Top ||
                                       panel.ParentItemsControl.TabStripPlacement == TabStripPlacement.Bottom);
            if (panel.m_updatescrollinfo)
            {
                if ((double) e.NewValue > (double) e.OldValue)
                    panel.UpdateScrollOffset(true, (double) e.NewValue - (double) e.OldValue);
                else
                    panel.UpdateScrollOffset(false, (double) e.OldValue - (double) e.NewValue);
            }
            else
                panel.m_updatescrollinfo = true;
        }

        /// <summary>
        /// Returns an array with headers size of each tab.
        /// </summary>
        /// <returns>returns an array of double value</returns>
        internal Size[] GetHeadersSize()
        {
            Size[] numArray = new Size[Children.Count];
            int index = 0;

            foreach (UIElement element in Children)
            {
                if (element != null)
                {
                    Size desiredSize = GetDesiredSize(element);
                    numArray[index] = element.Visibility == Visibility.Collapsed ? new Size(0.0, 0.0) : desiredSize;
                    index++;
                }
            }

            return numArray;
        }

        internal Size GetTotalHeadersSize(int count)
        {
            Size totalsize = new Size(0,0);

            for (int i = 0; i <= count;i++ )
            {
                UIElement element = Children[i];
                if (element != null)
                {
                    Size desiredSize = GetDesiredSize(element);
                    totalsize = element.Visibility == Visibility.Collapsed
                                    ? totalsize
                                    : new Size(totalsize.Width + desiredSize.Width,
                                               totalsize.Height + desiredSize.Height);
                }
            }

            return totalsize;
        }

        internal Size GetHeaderSize(UIElement element)
        {
            Size desiredSize = new Size(0, 0);
            if (element != null)
            {
                desiredSize = element.Visibility == Visibility.Collapsed ? new Size(0.0, 0.0) : GetDesiredSize(element);
            }
            return desiredSize;
        }

        /// <summary>
        /// Changes the prev part.
        /// </summary>
        private void ChangePrevPart()
        {
            if (m_scrollInfo.Offset == 0)
            {
                DisablePrevPart();
            }
            else
            {
                EnablePrevPart();
            }
        }

        /// <summary>
        /// Disables Next part of navigation buttons.
        /// </summary>
        public void DisableNextPart()
        {
            if (Name.Equals("PinnedPanel"))
            {
                ParentItemsControl.pinnedNextTabButton.IsEnabled = false;
                VisualStateManager.GoToState(ParentItemsControl, "PinnedNextTabUnSelected", true);
                VisualStateManager.GoToState(ParentItemsControl.pinnedNextTabButton, "Disabled", true);
            }
            else
            {
                ParentItemsControl.nextTabButton.IsEnabled = false;
                VisualStateManager.GoToState(ParentItemsControl, "NextTabUnSelected", true);
                VisualStateManager.GoToState(ParentItemsControl.nextTabButton, "Disabled", true);
            }
        }

        /// <summary>
        /// Enables Next part of navigation buttons.
        /// </summary>
        public void EnableNextPart()
        {
            if (Name.Equals("PinnedPanel"))
            {
                ParentItemsControl.pinnedNextTabButton.IsEnabled = true;
                VisualStateManager.GoToState(ParentItemsControl.pinnedNextTabButton, "Normal", true);
            }
            else
            {
                ParentItemsControl.nextTabButton.IsEnabled = true;
                VisualStateManager.GoToState(ParentItemsControl.nextTabButton, "Normal", true);
            }
        }

        /// <summary>
        /// Disables Prev part of navigation buttons.
        /// </summary>
        public void DisablePrevPart()
        {
            if (Name.Equals("PinnedPanel"))
            {
                ParentItemsControl.pinnedPreviousTabButton.IsEnabled = false;
                VisualStateManager.GoToState(ParentItemsControl, "PinnedPreviousTabUnSelected", true);
                VisualStateManager.GoToState(ParentItemsControl.pinnedPreviousTabButton, "Disabled", true);
            }
            else
            {
                ParentItemsControl.previousTabButton.IsEnabled = false;
                VisualStateManager.GoToState(ParentItemsControl, "PreviousTabUnSelected", true);
                VisualStateManager.GoToState(ParentItemsControl.previousTabButton, "Disabled", true);
            }
        }

        /// <summary>
        /// Enables Prev part of scrolling buttons.
        /// </summary>
        public void EnablePrevPart()
        {
            if (Name.Equals("PinnedPanel"))
            {
                ParentItemsControl.pinnedPreviousTabButton.IsEnabled = true;
                VisualStateManager.GoToState(ParentItemsControl.pinnedPreviousTabButton, "Normal", true);
            }
            else
            {
                ParentItemsControl.previousTabButton.IsEnabled = true;
                VisualStateManager.GoToState(ParentItemsControl.previousTabButton, "Normal", true);
            }
        }

        private bool UpdateIndex(Size[] headersSize, int index, int nextindex, ref double trimmedsize, bool horizontalplacement,
                                 bool updateLast,bool updatenext, double offset)
        {

            if (trimmedsize < offset)
            {
                if (nextindex < headersSize.Count() && ((updatenext && index + 1 < Children.Count) || (!updatenext && index - 1 >= 0)))
                    trimmedsize = trimmedsize +
                                  (horizontalplacement
                                       ? headersSize[nextindex].Width
                                       : headersSize[nextindex].Height);
                else
                {
                    if (updateLast)
                    {
                        if (horizontalplacement)
                            m_scrollInfo.LastTabTrimmedWidth = 0;
                        else
                            m_scrollInfo.LastTabTrimmedHeight = 0;
                        m_scrollInfo.LastTrimmedTabIndex = Children.Count - 1;
                    }
                    else
                    {
                        if (horizontalplacement)
                            m_scrollInfo.FirstTabTrimmedWidth = 0;
                        else
                            m_scrollInfo.FirstTabTrimmedHeight = 0;
                        m_scrollInfo.FirstTrimmedTabIndex = Children.Count - 1;
                    }
                    return true;
                }
            }
            else if (headersSize.Count() > index && headersSize[index].Width != trimmedsize)
            {
                if (updateLast)
                {
                    if (horizontalplacement)
                    {
                        m_scrollInfo.LastTabTrimmedWidth = trimmedsize - offset;
                        if (m_scrollInfo.LastTabTrimmedWidth <= headersSize[index].Width/3)
                        {
                            int addedindex = (index < headersSize.Length - 1) ? 1 : 0;
                            m_scrollInfo.LastTabTrimmedWidth = addedindex == 1 && headersSize.Count() > nextindex
                                                                   ? m_scrollInfo.LastTabTrimmedWidth +
                                                                     headersSize[nextindex].Width
                                                                   : m_scrollInfo.LastTabTrimmedWidth;
                            m_updatetabindex = true;
                        }
                    }
                    else
                    {
                        m_scrollInfo.LastTabTrimmedHeight = trimmedsize - offset;
                        if (m_scrollInfo.LastTabTrimmedHeight < headersSize[index].Height/3)
                        {
                            int addedindex = (index < headersSize.Length - 1) ? 1 : 0;
                            m_scrollInfo.LastTabTrimmedHeight = addedindex == 1 && headersSize.Count() > nextindex
                                                                    ? m_scrollInfo.LastTabTrimmedHeight +
                                                                      headersSize[nextindex].Height
                                                                    : m_scrollInfo.LastTabTrimmedHeight;
                            m_updatetabindex = true;
                        }
                    }
                    m_scrollInfo.LastTrimmedTabIndex = index;
                }
                else
                {
                    if (horizontalplacement)
                    {
                        m_scrollInfo.FirstTabTrimmedWidth = trimmedsize - offset;
                        if (m_scrollInfo.FirstTabTrimmedWidth <= headersSize[index].Width / 3)
                        {
                            int addedindex = (index < headersSize.Length - 1) ? 1 : 0;
                            m_scrollInfo.FirstTabTrimmedWidth = addedindex == 1 && headersSize.Count() > nextindex
                                                                   ? m_scrollInfo.FirstTabTrimmedWidth +
                                                                     headersSize[nextindex].Width
                                                                   : m_scrollInfo.FirstTabTrimmedWidth;
                            m_updatetabindex = true;
                        }
                    }
                    else
                    {
                        m_scrollInfo.FirstTabTrimmedHeight = trimmedsize - offset;
                        if (m_scrollInfo.FirstTabTrimmedHeight < headersSize[index].Height / 3)
                        {
                            int addedindex = (index < headersSize.Length - 1) ? 1 : 0;
                            m_scrollInfo.FirstTabTrimmedHeight = addedindex == 1 && headersSize.Count()>nextindex
                                                                    ? m_scrollInfo.FirstTabTrimmedHeight +
                                                                      headersSize[nextindex].Height
                                                                    : m_scrollInfo.FirstTabTrimmedHeight;
                            m_updatetabindex = true;
                        }
                    }
                    m_scrollInfo.FirstTrimmedTabIndex = index;
                }
                return true;
            }
            else
                return true;
            return false;
        }

        private void UpdateScrollOffset(bool next, double offset)
        {
            bool horizontalplacement = ParentItemsControl.TabStripPlacement == TabStripPlacement.Left ||
                                       ParentItemsControl.TabStripPlacement == TabStripPlacement.Right
                                           ? false
                                           : true;
            Size[] headersSize = GetHeadersSize();
            double trimmedsize = 0;

            if (next)
            {
                trimmedsize = horizontalplacement
                                  ? m_scrollInfo.LastTabTrimmedWidth
                                  : m_scrollInfo.LastTabTrimmedHeight;
                for (int i = m_scrollInfo.LastTrimmedTabIndex; i < Children.Count; i++)
                {
                    UIElement element = Children[i];
                    if (element != null)
                    {
                        if (UpdateIndex(headersSize, i, i + 1, ref trimmedsize, horizontalplacement, true, true, offset))
                            break;
                    }
                }

                m_scrollInfo.FirstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex < 0
                                                        ? 0
                                                        : m_scrollInfo.FirstTrimmedTabIndex;
                trimmedsize = horizontalplacement
                                  ? m_scrollInfo.FirstTabTrimmedWidth
                                  : m_scrollInfo.FirstTabTrimmedHeight;
                for (int i = m_scrollInfo.FirstTrimmedTabIndex; i < Children.Count; i++)
                {
                    if (UpdateIndex(headersSize, i, i + 1, ref trimmedsize, horizontalplacement, false, true, offset))
                        break;
                }
            }
            else
            {
                m_scrollInfo.FirstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex < 0
                                                        ? 0
                                                        : m_scrollInfo.FirstTrimmedTabIndex;
                trimmedsize = horizontalplacement
                                  ? m_scrollInfo.FirstTabTrimmedWidth
                                  : m_scrollInfo.FirstTabTrimmedHeight;
                for (int i = m_scrollInfo.FirstTrimmedTabIndex; i > 0; i--)
                {
                    if(UpdateIndex(headersSize, i, i - 1, ref trimmedsize, horizontalplacement, false,false, offset))
                        break;
                }

                trimmedsize = horizontalplacement
                                  ? m_scrollInfo.LastTabTrimmedWidth
                                  : m_scrollInfo.LastTabTrimmedHeight;
                for (int i = m_scrollInfo.LastTrimmedTabIndex; i > 0; i--)
                {
                    if (UpdateIndex(headersSize, i, i - 1, ref trimmedsize, horizontalplacement, true, false, offset))
                        break;
                }
            }

        }

        internal void ValidateScrollOffset(bool horizontalplacement)
        {
            double viewportwidth = Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportWidth : ParentItemsControl.ScrollViewer.ViewportWidth;
            double viewportheight = Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportHeight : ParentItemsControl.ScrollViewer.ViewportHeight;
            if (horizontalplacement && viewportwidth < m_scrollInfo.DesiredWidth)
            {
                if (Math.Abs(m_scrollInfo.Offset) + viewportwidth >= m_scrollInfo.DesiredWidth)
                {
                    m_scrollInfo.Offset += Math.Abs(m_scrollInfo.Offset) + viewportwidth - m_scrollInfo.DesiredWidth;
                }
                if (m_scrollInfo.DesiredWidth > (Math.Abs(m_scrollInfo.Offset) + viewportwidth))
                {
                    EnableNextPart();
                }
                else
                {
                    DisableNextPart();
                }
            }
            else if (!horizontalplacement && viewportheight < m_scrollInfo.DesiredHeight)
            {
                if (Math.Abs(m_scrollInfo.Offset) + viewportheight >= m_scrollInfo.DesiredHeight)
                {
                    m_scrollInfo.Offset += Math.Abs(m_scrollInfo.Offset) + viewportheight - m_scrollInfo.DesiredHeight;
                }
                if (m_scrollInfo.DesiredHeight > (Math.Abs(m_scrollInfo.Offset) + viewportheight))
                {
                    EnableNextPart();
                }
                else
                {
                    DisableNextPart();
                }
            }
            else
                m_scrollInfo.Offset = 0;

            if ((horizontalplacement && m_scrollInfo.DesiredWidth < (Math.Abs(m_scrollInfo.Offset) + viewportwidth)) || (!horizontalplacement && m_scrollInfo.DesiredHeight < (Math.Abs(m_scrollInfo.Offset) + viewportheight)))
                DisableNextPart();

            ChangePrevPart();
        }

        /// <summary>
        ///  Positions tabs in SingleLine mode. 
        /// </summary>
        /// <param name="availiableWidth">The width of the final area within the TabControlExt that this element should use to arrange itself and its children.</param>
        /// <param name="availableheight">The height of the final area within the TabControlExt that this element should use to arrange itself and its children.</param>
        private void ArrangeSingleLine(double availiableWidth,double availableheight)
        {
            bool horizontalplacement = ParentItemsControl.TabStripPlacement == TabStripPlacement.Left ||
                                       ParentItemsControl.TabStripPlacement == TabStripPlacement.Right
                                       ? false : true;
            double viewportwidth = Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportWidth : ParentItemsControl.ScrollViewer.ViewportWidth;
            double viewportheight = Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportHeight : ParentItemsControl.ScrollViewer.ViewportHeight;
            double width,height;
            Point startPoint = new Point(0, 0);
            Size[] headersSize = GetHeadersSize();
            m_scrollInfo.LastTrimmedTabIndex = Children.Count + 1;
            m_scrollInfo.FirstTrimmedTabIndex = -1;

            ValidateScrollOffset(horizontalplacement);
            
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement element = Children[i];
                width = headersSize[i].Width;
                height = headersSize[i].Height;
                if (element != null)
                {
                    if (horizontalplacement)
                    {
                        element.Arrange(new Rect(startPoint.X, startPoint.Y, width,viewportheight));
                        if (startPoint.X + width < viewportwidth)
                        {
                            if ((startPoint.X < 0 && ((startPoint.X + width) >= 0)) || Math.Abs(startPoint.X + width) < 0.01)
                            {
                                if (Math.Abs(startPoint.X) < (width / 3))
                                {
                                    int addedindex = (i != 0) ? 1 : 0;
                                    m_scrollInfo.FirstTabTrimmedWidth = (addedindex == 0)
                                                                            ? Math.Abs(startPoint.X)
                                                                            : Math.Abs(startPoint.X) + headersSize[i - 1].Width;
                                }
                                else
                                {
                                    m_scrollInfo.FirstTabTrimmedWidth = Math.Abs(startPoint.X);
                                }
                                m_scrollInfo.FirstTrimmedTabIndex = i;

                                if (m_scrollInfo.FirstTabTrimmedWidth < 0.01)
                                {
                                    m_scrollInfo.FirstTabTrimmedWidth = width;
                                }
                            }
                            if (Math.Abs(startPoint.X) < 0.01 && Math.Abs(startPoint.X) > 0)
                            {
                                startPoint.X = 0;
                            }
                        }
                        else if (startPoint.X < viewportwidth)
                        {
                            if (Math.Abs(width - (viewportwidth - startPoint.X)) > 0.01)
                            {
                                if ((width - (viewportwidth - startPoint.X)) < (width / 3))
                                {
                                    int addedindex = (i < headersSize.Length - 1) ? 1 : 0;
                                    m_scrollInfo.LastTabTrimmedWidth = (addedindex == 0)
                                                                           ? width - (viewportwidth - startPoint.X)
                                                                           : width - (viewportwidth - startPoint.X) +
                                                                             headersSize[i + 1].Width;
                                    m_updatetabindex = true;
                                }
                                else
                                {
                                    m_scrollInfo.LastTabTrimmedWidth = width - (viewportwidth - startPoint.X);
                                    m_updatetabindex = false;
                                }
                                m_scrollInfo.LastTrimmedTabIndex = i;
                            }
                            else
                            {
                                DisableNextPart();
                                m_scrollInfo.LastTabTrimmedWidth = 0;
                            }
                        }
                        else
                        {
                            if (m_scrollInfo.LastTabTrimmedWidth == 0)
                            {
                                m_scrollInfo.LastTabTrimmedWidth = width;
                                m_scrollInfo.LastTrimmedTabIndex = i;
                            }
                            EnableNextPart();
                        }
                        startPoint.X += width;
                    }
                    else
                    {
                        element.Arrange(new Rect(startPoint.X, startPoint.Y, viewportwidth,height));
                        if (startPoint.Y + height < viewportheight)
                        {
                            if ((startPoint.Y < 0 && ((startPoint.Y + width) >= 0)) || Math.Abs(startPoint.Y + height) < 0.01)
                            {
                                if (Math.Abs(startPoint.Y) < (height / 3))
                                {
                                    int addedindex = (i != 0) ? 1 : 0;
                                    m_scrollInfo.FirstTabTrimmedHeight = (addedindex == 0)
                                                                            ? Math.Abs(startPoint.Y)
                                                                            : Math.Abs(startPoint.Y) + headersSize[i - 1].Height;
                                }
                                else
                                {
                                    m_scrollInfo.FirstTabTrimmedHeight = Math.Abs(startPoint.Y);
                                }
                                m_scrollInfo.FirstTrimmedTabIndex = i;

                                if (m_scrollInfo.FirstTabTrimmedHeight < 0.01)
                                {
                                    m_scrollInfo.FirstTabTrimmedHeight = height;
                                }
                            }
                            DisableNextPart();

                            if (Math.Abs(startPoint.Y) < 0.01 && Math.Abs(startPoint.Y) > 0)
                            {
                                startPoint.Y = 0;
                            }
                        }
                        else if (startPoint.Y < viewportheight)
                        {
                            if (Math.Abs(height - (viewportheight - startPoint.Y)) > 0.01)
                            {
                                if ((height - (viewportheight - startPoint.Y)) < (height / 3))
                                {
                                    int addedindex = (i < headersSize.Length - 1) ? 1 : 0;
                                    m_scrollInfo.LastTabTrimmedHeight = (addedindex == 0)
                                                                           ? height - (viewportheight - startPoint.Y)
                                                                           : height - (viewportheight - startPoint.Y) +
                                                                             headersSize[i + 1].Height;
                                }
                                else
                                {
                                    m_scrollInfo.LastTabTrimmedHeight = height - (viewportheight - startPoint.Y);
                                }
                                m_scrollInfo.LastTrimmedTabIndex = i;
                            }
                            else
                            {
                                DisableNextPart();
                                m_scrollInfo.LastTabTrimmedHeight = 0;
                            }
                        }
                        else
                        {
                            if (m_scrollInfo.LastTabTrimmedHeight == 0)
                            {
                                m_scrollInfo.LastTabTrimmedHeight = height;
                                m_scrollInfo.LastTrimmedTabIndex = i;
                            }
                            EnableNextPart();
                        }

                        startPoint.Y += height;
                    }
                }
            }
        }

        /// <summary>
        /// Positions tabs in SingleLine mode. 
        /// </summary>
        /// <param name="arrangeSize">The final area within the TabControlExt that this element should use to arrange itself and its children.</param>
        private void ArrangeElements(Size arrangeSize)
        {
            double availableWidth = Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportWidth : ParentItemsControl.ScrollViewer.ViewportWidth;
            double availableHeight = Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportHeight : ParentItemsControl.ScrollViewer.ViewportHeight;
            m_scrollInfo.AllTrimmedWidth = m_scrollInfo.DesiredWidth > availableWidth
                                               ? m_scrollInfo.DesiredWidth - availableWidth
                                               : 0;
            m_scrollInfo.AllTrimmedHeight = m_scrollInfo.DesiredHeight > availableHeight
                                               ? m_scrollInfo.DesiredHeight - availableHeight
                                               : 0;
            ArrangeSingleLine(arrangeSize.Width, arrangeSize.Height);
        }

        /// <summary>
        /// Measure all elements.
        /// </summary>
        /// <param name="availableSize">Value of the availableSize</param>
        /// <returns>returns a double value</returns>
        private Size MeasureElements(Size availableSize)
        {
            double totalWidth = 0;
            double totalHeight = 0;
            Size totalSize;
            if (ParentItemsControl != null)
            {
                if (ParentItemsControl.TabStripPlacement == TabStripPlacement.Bottom ||
                    ParentItemsControl.TabStripPlacement == TabStripPlacement.Top)
                {
                    foreach (UIElement element in Children)
                    {
                        if (element != null)
                        {
                            element.Measure(new Size(availableSize.Width, availableSize.Height));
                            totalWidth += element.DesiredSize.Width;
                            totalHeight = Math.Max(totalHeight, element.DesiredSize.Height);
                            if ((element as SfTabItem).Header == null && totalHeight != 0)
                                prevHeight = totalHeight;
                            if ((element as SfTabItem).Header == null && totalHeight == 0)
                                totalHeight = prevHeight;
                        }
                    }

                    totalSize = new Size(totalWidth, totalHeight);
                }
                else
                {
                    foreach (UIElement element in Children)
                    {
                        if (element != null)
                        {
                            element.Measure(new Size(availableSize.Width, availableSize.Height));
                            totalWidth = Math.Max(totalWidth, element.DesiredSize.Width);
                            totalHeight += element.DesiredSize.Height;
                        }
                    }

                    totalSize = new Size(totalWidth, totalHeight);
                }
            }

            m_scrollInfo.DesiredWidth = totalWidth;
            m_scrollInfo.DesiredHeight = totalHeight;
            return totalSize;
        }

        private void ValidateFirstTrimmedTab(double totalSize, bool horizontalplacement)
        {
            int lastTrimmedTabIndex = m_scrollInfo.LastTrimmedTabIndex;
            double actualwidth = totalSize - (Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportWidth : ParentItemsControl.ScrollViewer.ViewportWidth);
            double actualheight = totalSize - (Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportHeight : ParentItemsControl.ScrollViewer.ViewportHeight);
            for (int i = lastTrimmedTabIndex; i >= 0; i--)
            {
                double headersize = horizontalplacement ? GetHeaderSize(Children[i]).Width : GetHeaderSize(Children[i]).Height;
                totalSize = totalSize - headersize;
                if (totalSize <= actualwidth && horizontalplacement)
                {
                    m_scrollInfo.FirstTabTrimmedWidth = actualwidth - totalSize > headersize/3
                                                            ? actualwidth - totalSize
                                                            : 0;
                    m_scrollInfo.FirstTrimmedTabIndex = i;
                    break;
                }
                else if (totalSize <= actualheight && !horizontalplacement)
                {
                    m_scrollInfo.FirstTabTrimmedHeight = actualheight - totalSize > headersize/3
                                                            ? actualheight - totalSize
                                                            : 0;
                    m_scrollInfo.FirstTrimmedTabIndex = i;
                    break;
                }
            }
        }

        private void ValidateLastTrimmedTab(double totalSize, bool horizontalplacement)
        {
            int firstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex;

            if (firstTrimmedTabIndex == -1)
                firstTrimmedTabIndex = 0;
            double actualwidth = totalSize + (Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportWidth : ParentItemsControl.ScrollViewer.ViewportWidth);
            double actualheight = totalSize + (Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportHeight : ParentItemsControl.ScrollViewer.ViewportHeight);
            for (int i = firstTrimmedTabIndex; i < Children.Count; i++)
            {
                double headersize = horizontalplacement ? GetHeaderSize(Children[i]).Width : GetHeaderSize(Children[i]).Height;
                totalSize = totalSize + headersize;
                if (actualwidth <= totalSize && horizontalplacement)
                {
                    m_scrollInfo.LastTabTrimmedWidth = totalSize - actualwidth > headersize / 3
                                                            ? totalSize - actualwidth
                                                            : 0;
                    m_scrollInfo.LastTrimmedTabIndex = m_scrollInfo.LastTabTrimmedWidth > 0 ? i - 1 : i;
                    break;
                }
                else if (actualheight <= totalSize && !horizontalplacement)
                {
                    m_scrollInfo.LastTabTrimmedHeight = totalSize - actualheight > headersize / 3
                                                            ? totalSize - actualheight
                                                            : 0;
                    m_scrollInfo.LastTrimmedTabIndex = m_scrollInfo.LastTabTrimmedHeight > 0 ? i - 1 : i;
                    break;
                }
            }
        }

        /// <summary>
        /// Launch scroll int to the next tab.
        /// </summary>
        internal void ScrollToNextTab()
        {
            int lasttabtrimmedtabindex = m_scrollInfo.LastTrimmedTabIndex;
            if (parentItemsControl != null && parentItemsControl.TabStripMenu != null && parentItemsControl.TabStripMenu.IsOpen)
                parentItemsControl.TabStripMenu.IsOpen = false;
            bool horizontalplacement = ParentItemsControl.TabStripPlacement == TabStripPlacement.Left ||
                                       ParentItemsControl.TabStripPlacement == TabStripPlacement.Right
                                       ? false : true;
            if (!parentItemsControl.previousTabButton.IsEnabled)
            {
                ValidateLastTrimmedTab(0, horizontalplacement);
                if (!horizontalplacement)
                {
                    if (lasttabtrimmedtabindex <= m_scrollInfo.LastTrimmedTabIndex)
                        m_scrollInfo.LastTrimmedTabIndex++;
                    else if (lasttabtrimmedtabindex > m_scrollInfo.LastTrimmedTabIndex)
                        m_scrollInfo.LastTrimmedTabIndex = lasttabtrimmedtabindex;
                }
                lasttabtrimmedtabindex = m_scrollInfo.LastTrimmedTabIndex;
            }            
            if (parentItemsControl != null && parentItemsControl.TabStripMenu != null && parentItemsControl.TabStripMenu.IsOpen)
                parentItemsControl.TabStripMenu.IsOpen = false;
            
            if (m_scrollInfo.LastTrimmedTabIndex < Children.Count-1  &&
                ((m_scrollInfo.LastTabTrimmedWidth == 0.0 && horizontalplacement) ||
                 (m_scrollInfo.LastTabTrimmedHeight == 0.0 && !horizontalplacement)))
            {
                m_scrollInfo.LastTrimmedTabIndex = m_scrollInfo.LastTrimmedTabIndex + 1;
                m_scrollInfo.FirstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex + 1;
            }

            if (m_scrollInfo.LastTrimmedTabIndex < Children.Count && horizontalplacement)
            {
                if (lasttabtrimmedtabindex + 1 != m_scrollInfo.LastTrimmedTabIndex)
                {
                    m_scrollInfo.LastTrimmedTabIndex = m_scrollInfo.LastTrimmedTabIndex + 1;
                    m_scrollInfo.FirstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex + 1;
                }
                m_updatescrollinfo = false;
                double totalsize, viewportwidth;
                viewportwidth = Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportWidth : ParentItemsControl.ScrollViewer.ViewportWidth;
                if (m_updatetabindex)
                {
                    totalsize = GetTotalHeadersSize(m_scrollInfo.LastTrimmedTabIndex+1).Width;
                    if (lasttabtrimmedtabindex + 1 != m_scrollInfo.LastTrimmedTabIndex)
                        m_scrollInfo.LastTrimmedTabIndex = m_scrollInfo.LastTrimmedTabIndex + 1;
                    ValidateFirstTrimmedTab(totalsize,horizontalplacement);
                }
                else
                    totalsize = GetTotalHeadersSize(m_scrollInfo.LastTrimmedTabIndex).Width;
                if(Name.Equals("PinnedPanel"))
                    ParentItemsControl.PinnedScrollViewer.ScrollToHorizontalOffset(totalsize - viewportwidth);
                else
                    ParentItemsControl.ScrollViewer.ScrollToHorizontalOffset(totalsize - viewportwidth);
                m_scrollInfo.LastTabTrimmedWidth = 0;
                m_updatetabindex = false;
            }
            else if (m_scrollInfo.LastTrimmedTabIndex < Children.Count && !horizontalplacement)
            {
                m_updatescrollinfo = false;
                double totalsize, viewportheight;
                viewportheight = Name.Equals("PinnedPanel") ? ParentItemsControl.PinnedScrollViewer.ViewportHeight : ParentItemsControl.ScrollViewer.ViewportHeight;
                if (m_updatetabindex)
                {
                    totalsize = GetTotalHeadersSize(m_scrollInfo.LastTrimmedTabIndex + 1).Height;
                    m_scrollInfo.LastTrimmedTabIndex = m_scrollInfo.LastTrimmedTabIndex + 1;
                    ValidateFirstTrimmedTab(totalsize,horizontalplacement);
                }
                else
                    totalsize = GetTotalHeadersSize(m_scrollInfo.LastTrimmedTabIndex).Height;
                if(Name.Equals("PinnedPanel"))
                    ParentItemsControl.PinnedScrollViewer.ScrollToVerticalOffset(totalsize - viewportheight);
                else
                    ParentItemsControl.ScrollViewer.ScrollToVerticalOffset(totalsize - viewportheight);
                m_scrollInfo.LastTabTrimmedHeight = 0;
                m_updatetabindex = false;
            }
        }

        /// <summary>
        /// Launch scroll int to the previous tab.
        /// </summary>
        internal void ScrollToPrevTab()
        {
            if (parentItemsControl != null && parentItemsControl.TabStripMenu != null && parentItemsControl.TabStripMenu.IsOpen)
                parentItemsControl.TabStripMenu.IsOpen = false;
            bool horizontalplacement = ParentItemsControl.TabStripPlacement == TabStripPlacement.Left ||
                                       ParentItemsControl.TabStripPlacement == TabStripPlacement.Right
                                       ? false : true;

            if (m_scrollInfo.FirstTrimmedTabIndex > 0 && horizontalplacement)
            {
                if (m_scrollInfo.FirstTabTrimmedWidth == 0.0)
                    m_scrollInfo.FirstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex - 1;
                m_updatescrollinfo = false;
                double totalsize;
                if (m_updatetabindex)
                {
                    m_scrollInfo.FirstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex - 1;
                }
                totalsize = GetTotalHeadersSize(m_scrollInfo.FirstTrimmedTabIndex).Width;
                ValidateLastTrimmedTab(totalsize,horizontalplacement);
                if(Name.Equals("PinnedPanel"))
                    ParentItemsControl.PinnedScrollViewer.ScrollToHorizontalOffset(totalsize);
                else
                    ParentItemsControl.ScrollViewer.ScrollToHorizontalOffset(totalsize);
                m_scrollInfo.FirstTabTrimmedWidth = 0;
                m_updatetabindex = false;
            }
            else if (m_scrollInfo.FirstTrimmedTabIndex > 0 && !horizontalplacement)
            {
                if (m_scrollInfo.FirstTabTrimmedHeight == 0.0)
                    m_scrollInfo.FirstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex - 1;
                m_updatescrollinfo = false;
                double totalsize;
                if (m_updatetabindex)
                {
                    m_scrollInfo.FirstTrimmedTabIndex = m_scrollInfo.FirstTrimmedTabIndex - 1;
                }
                totalsize = GetTotalHeadersSize(m_scrollInfo.FirstTrimmedTabIndex).Height;
                ValidateLastTrimmedTab(totalsize, horizontalplacement);
                if(Name.Equals("PinnedPanel"))
                    ParentItemsControl.PinnedScrollViewer.ScrollToVerticalOffset(totalsize);
                else
                    ParentItemsControl.ScrollViewer.ScrollToVerticalOffset(totalsize);
                m_scrollInfo.FirstTabTrimmedHeight = 0;
                m_updatetabindex = false;
            }
            else
            {
                if (Name.Equals("PinnedPanel"))
                {
                    if (horizontalplacement)
                        ParentItemsControl.PinnedScrollViewer.ScrollToHorizontalOffset(0);
                    else
                        ParentItemsControl.PinnedScrollViewer.ScrollToVerticalOffset(0);
                }
                else
                {
                    if (horizontalplacement)
                        ParentItemsControl.ScrollViewer.ScrollToHorizontalOffset(0);
                    else
                        ParentItemsControl.ScrollViewer.ScrollToVerticalOffset(0);
                }
                ValidateLastTrimmedTab(0,horizontalplacement);
            }
        }

        /// <summary>
        /// Returns desired size for specified element.
        /// </summary>
        /// <param name="element">Value of the element</param>
        /// <returns>required size</returns>
        private static Size GetDesiredSize(UIElement element)
        {
            return new Size(element.DesiredSize.Width, element.DesiredSize.Height);
        }


        private static T GetVisualChild<T>(DependencyObject child) where T : DependencyObject
        {
            T parent = default(T);

            object v = (object)VisualTreeHelper.GetParent(child);
            parent = v as T;
            if (parent == null)
            {
                parent = GetVisualChild<T>(v as DependencyObject);
            }

            return parent;
        }

        #endregion

        #region Override

        /// <summary>
        /// Called to remeasure a control. 
        /// </summary>
        /// <param name="availableSize">Measurement constraints, a control cannot return a size larger than the constraint.</param>
        /// <returns>The size of the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (parentItemsControl == null)
            {
                parentItemsControl = GetVisualChild<SfTabControl>(this);
            }
            bool horizontalplacement = ParentItemsControl.TabStripPlacement == TabStripPlacement.Left ||
                                       ParentItemsControl.TabStripPlacement == TabStripPlacement.Right
                                           ? false
                                           : true;
            
            Size totalSize = MeasureElements(new Size(availableSize.Width, availableSize.Height));

            if (Name.Equals("PinnedPanel"))
            {
                if (horizontalplacement)
                {
                    ParentItemsControl.PinnedScrollViewer.ScrollToHorizontalOffset(0);
                    if (ParentItemsControl.PinnedScrollViewer.ViewportWidth > 0)
                    {
                        double completewidth = (ParentItemsControl.PinnedScrollViewer.ViewportWidth +
                                                ParentItemsControl.ScrollViewer.ViewportWidth);
                        if (ParentItemsControl.commonButtonGrid != null)
                            completewidth -= ParentItemsControl.pinnedCommonButtonGrid.ActualWidth;
                        ParentItemsControl.PinnedScrollViewer.MaxWidth = completewidth - completewidth/1.5;
                    }
                }
                else
                {
                    if (ParentItemsControl.PinnedScrollViewer.ViewportHeight > 0)
                    {
                        ParentItemsControl.PinnedScrollViewer.ScrollToVerticalOffset(0);
                        double completeheight = (ParentItemsControl.PinnedScrollViewer.ViewportHeight +
                                                 ParentItemsControl.ScrollViewer.ViewportHeight);
                        if (ParentItemsControl.commonButtonGrid != null)
                            completeheight -= ParentItemsControl.pinnedCommonButtonGrid.ActualHeight;
                        ParentItemsControl.PinnedScrollViewer.MaxHeight = completeheight - completeheight/1.5;
                    }
                }
            }
            else
            {
                if (horizontalplacement)
                    ParentItemsControl.ScrollViewer.ScrollToHorizontalOffset(0);
                else
                    ParentItemsControl.ScrollViewer.ScrollToVerticalOffset(0);    
            }
            return new Size(totalSize.Width, totalSize.Height);
        }

        /// <summary>
        /// Called to arrange and size tabs of a TabControlExt object. 
        /// </summary>
        /// <param name="finalSize">The computed size that is used to arrange tabs.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            bool horizontalplacement = this.ParentItemsControl.TabStripPlacement == TabStripPlacement.Left ||
                                       this.ParentItemsControl.TabStripPlacement == TabStripPlacement.Right
                                       ? false : true;
            if (Name.Equals("PinnedPanel"))
            {
                m_scrollInfo.NeedScrollButtonsShow = ParentItemsControl.TabStripPlacement == TabStripPlacement.Left ||
                                                     ParentItemsControl.TabStripPlacement == TabStripPlacement.Right
                                                         ? ParentItemsControl.PinnedScrollViewer.ViewportHeight <
                                                           finalSize.Height
                                                         : ParentItemsControl.PinnedScrollViewer.ViewportWidth <
                                                           finalSize.Width;

                ParentItemsControl.CheckPinnedNavigationButtonVisibility(m_scrollInfo.NeedScrollButtonsShow,
                                                                         finalSize.Width);
            }
            else
            {
                m_scrollInfo.NeedScrollButtonsShow = ParentItemsControl.TabStripPlacement == TabStripPlacement.Left ||
                                                     ParentItemsControl.TabStripPlacement == TabStripPlacement.Right
                                                         ? ParentItemsControl.ScrollViewer.ViewportHeight <
                                                           finalSize.Height
                                                         : ParentItemsControl.ScrollViewer.ViewportWidth <
                                                           finalSize.Width;
                if (parentItemsControl.nextTabButton!=null && parentItemsControl.nextTabButton.Visibility == Visibility.Collapsed && parentItemsControl.previousTabButton!=null && parentItemsControl.previousTabButton.Visibility == Visibility.Collapsed)
                    ParentItemsControl.CheckNavigationButtonVisibility(m_scrollInfo.NeedScrollButtonsShow,
                                                                         finalSize.Width);
            }
            ArrangeElements(finalSize);
            if (Name.Equals("PinnedPanel"))
                ValidateScrollOffset(horizontalplacement);
            return finalSize;
        }

        #endregion
    }
}
