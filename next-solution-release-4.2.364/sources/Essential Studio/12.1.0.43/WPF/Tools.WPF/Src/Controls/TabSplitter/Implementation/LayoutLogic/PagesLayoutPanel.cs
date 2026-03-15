// <copyright file="PagesLayoutPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the page lay out panel class for the TabSplitter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PagesLayoutPanel : DockPanel
    {
        #region Constants
        /// <summary>
        /// Presents the tab shift
        /// </summary>
        private const int VS2008_TAB_SHIFT = 12;
        
        /// <summary>
        /// Presents the Scroll button
        /// </summary>
        private const string URI_SPLITTER_SCROLL_BUTTON = "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Generic.xaml";
        #endregion

        #region Private members
        /// <summary>
        /// Presents the Previous Page
        /// </summary>
        private Button m_prevPage = null;
        
        /// <summary>
        /// Presents the Next Page
        /// </summary>
        private Button m_nextPage = null;
        
        /// <summary>
        /// Presents the Row Height
        /// </summary>
        private double m_rowHeight;
        
        /// <summary>
        /// Presents the DesiredWidth
        /// </summary>
        private double m_desiredWidth;
        
        /// <summary>
        /// Presents the DesiredWidth
        /// </summary>
        private double m_actualWidth;
        
        /// <summary>
        /// Presents the ScrollingButtonStyle
        /// </summary>
        private Style m_scrollingButtonStyle = null;
        
        /// <summary>
        /// Presents the start index
        /// </summary>
        private int m_startIndex = 0;
        
        /// <summary>
        /// Presents the Scroll next
        /// </summary>
        private bool m_canScrollNext = false;
        
        /// <summary>
        /// Presents the Scroll previous
        /// </summary>
        private bool m_canScrollPrev = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>The start index.</value>
        private int StartIndex
        {
            get
            {
                return m_startIndex;
            }

            set
            {
                if (value >= 0 && value < InternalChildren.Count)
                {
                    m_startIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets the scrolling button style.
        /// </summary>
        /// <value>The scrolling button style.</value>
        private Style ScrollingButtonStyle
        {
            get
            {
                if (m_scrollingButtonStyle == null)
                {
                    ResourceDictionary dictionary = new ResourceDictionary
                    {
                        Source = new Uri(URI_SPLITTER_SCROLL_BUTTON, UriKind.RelativeOrAbsolute)
                    };
                    m_scrollingButtonStyle = (Style)dictionary["SplitScrollingButton"];
                }

                return m_scrollingButtonStyle;
            }
        }
        
        /// <summary>
        /// Gets previous navigation button.
        /// </summary>
        private Button PrevButton
        {
            get
            {
                if (m_prevPage == null)
                {
                    m_prevPage = new Button
                    {
                        Tag = "PrevTab",
                        Style = ScrollingButtonStyle
                    };
                    AddLogicalChild(PrevButton);
                    AddVisualChild(PrevButton);
                    PrevButton.Click += new RoutedEventHandler(OnPrevPageClick);
                }

                return m_prevPage;
            }
        }
        
        /// <summary>
        /// Gets next navigation button.
        /// </summary>
        private Button NextButton
        {
            get
            {
                if (m_nextPage == null)
                {
                    m_nextPage = new Button
                    {
                        Tag = "NextTab",
                        Style = ScrollingButtonStyle
                    };
                    AddLogicalChild(NextButton);
                    AddVisualChild(NextButton);
                    NextButton.Click += new RoutedEventHandler(OnNextPageClick);
                }

                return m_nextPage;
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="PagesLayoutPanel"/> class.
        /// </summary>
        public PagesLayoutPanel()
        {
            ClipToBounds = true;
        }

        /// <summary>
        /// Initializes static members of the <see cref="PagesLayoutPanel"/> class.
        /// </summary>
        static PagesLayoutPanel()
        {
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(PagesLayoutPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(PagesLayoutPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Previews the measure.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        private void PreviewMeasure(Size availableSize)
        {
            m_rowHeight = 0;
            m_desiredWidth = VS2008_TAB_SHIFT;
            double afterStartIndexWidth = 0;
            NextButton.Measure(availableSize);
            PrevButton.Measure(availableSize);

            foreach (UIElement element in InternalChildren)
            {
                if (element.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                element.Measure(availableSize);
                double currentHeight = element.DesiredSize.Height;
                m_desiredWidth += element.DesiredSize.Width;

                if (m_rowHeight < currentHeight)
                {
                    m_rowHeight = currentHeight;
                }
            }

            if (availableSize.Width < m_desiredWidth && m_startIndex > 0)
            {
                for (int i = 0; i < InternalChildren.Count; i++)
                {
                    UIElement child = InternalChildren[i];

                    if (i < m_startIndex)
                    {
                    }
                    else
                    {
                        afterStartIndexWidth += child.DesiredSize.Width;
                    }
                }

                if (afterStartIndexWidth + 0.1 < m_actualWidth)
                {
                    int oldStartIndex = m_startIndex;

                    while (true)
                    {
                        if (oldStartIndex > 0 && afterStartIndexWidth + InternalChildren[oldStartIndex - 1].DesiredSize.Width <= m_actualWidth)
                        {
                            afterStartIndexWidth += InternalChildren[oldStartIndex - 1].DesiredSize.Width;
                            oldStartIndex--;
                        }
                        else
                        {
                            break;
                        }
                    }

                    StartIndex = oldStartIndex;
                }
            }
        }
        
        /// <summary>
        /// Selects current splitter page.
        /// </summary>
        /// <param name="page">Page to select</param>
        private static void SelectPage(SplitterPage page)
        {
            if (page != null)
            {
                page.SelectThisPage();
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            PreviewMeasure(availableSize);
            if (availableSize.Width >= m_desiredWidth)
            {
                PrevButton.Visibility = Visibility.Collapsed;
                NextButton.Visibility = Visibility.Collapsed;
                return new Size(m_desiredWidth, m_rowHeight);
            }
            else
            {
                PrevButton.Visibility = Visibility.Visible;
                NextButton.Visibility = Visibility.Visible;
                return new Size(availableSize.Width, m_rowHeight);
            }
        }
        
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int lastTrimmedIndex = InternalChildren.Count;
            double tabShift = 0;
            double offset = (m_rowHeight - m_prevPage.DesiredSize.Height) / 2;
            double currentWidth = PrevButton.DesiredSize.Width;
            m_actualWidth = finalSize.Width - (2 * PrevButton.DesiredSize.Width) - VS2008_TAB_SHIFT - 0.1;
            m_canScrollNext = false;
            m_canScrollPrev = false;

            if (PrevButton.Visibility == Visibility.Collapsed)
            {
                StartIndex = 0;
            }

            Dock tabStripPlacement = SplitterPage.GetTabStripPlacement(this);

            if (Dock.Top == tabStripPlacement)
            {
                currentWidth += VS2008_TAB_SHIFT;
            }
            else if (Dock.Bottom == tabStripPlacement)
            {
                tabShift = VS2008_TAB_SHIFT;
            }

            PrevButton.Arrange(new Rect(0, offset, PrevButton.DesiredSize.Width, PrevButton.DesiredSize.Height));

            for (int i = 0; i < m_startIndex; i++)
            {
                UIElement child = InternalChildren[i];
                child.Arrange(new Rect(0, 0, 0, 0));
                m_canScrollPrev = true;
            }

            for (int i = StartIndex; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                child.Arrange(new Rect(0, 0, 0, 0));

                if (currentWidth + child.DesiredSize.Width + tabShift <= finalSize.Width - NextButton.DesiredSize.Width + 0.1)
                {
                    child.Arrange(new Rect(currentWidth, 0, child.DesiredSize.Width, m_rowHeight));
                    currentWidth += child.DesiredSize.Width;
                }
                else
                {
                    lastTrimmedIndex = i;
                    m_canScrollNext = true;
                    break;
                }
            }

            for (int i = lastTrimmedIndex; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                child.Arrange(new Rect(currentWidth, 0, 0, 0));
            }

            currentWidth = finalSize.Width - NextButton.DesiredSize.Width;
            NextButton.Arrange(new Rect(currentWidth, offset, NextButton.DesiredSize.Width, NextButton.DesiredSize.Height));

            if (Dock.Top == tabStripPlacement && NextButton.Visibility != Visibility.Visible)
            {
                finalSize.Width -= VS2008_TAB_SHIFT;
            }

            return new Size(finalSize.Width, m_rowHeight);
        }
        
        /// <summary>
        /// Gets the number of child elements for the control.
        /// </summary>
        /// <returns>An Int32 value that represents the number of child elements.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                int count = base.VisualChildrenCount;
                return (count >= 1) ? (count + 2) : count;
            }
        }
        
        /// <summary>
        /// Returns a child at the specified index from a collection of child elements. 
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>The requested child element. This should not return null; if the provided index is out of range, an exception is raised.</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index == VisualChildrenCount - 2)
            {
                return PrevButton;
            }

            return index == VisualChildrenCount - 1 ? NextButton : base.GetVisualChild(index);
        }
        
        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed. Overrides <see cref="M:System.Windows.UIElement.OnVisualParentChanged(System.Windows.DependencyObject)"/>.
        /// </summary>
        /// <param name="oldParent">The old parent element. May be null to indicate that the element did not have a visual parent previously.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            Binding visualStyleBinding = new Binding
            {
                Source = TemplatedParent,
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(SkinStorage.VisualStyleProperty)
            };
            Binding visualStyleListBinding = new Binding
            {
                Source = TemplatedParent,
                Mode = BindingMode.OneWay,
                //Path = new PropertyPath(SkinStorage.VisualStylesListProperty)
            };

            PrevButton.SetBinding(SkinStorage.VisualStyleProperty, visualStyleBinding);
            //PrevButton.SetBinding(SkinStorage.VisualStylesListProperty, visualStyleListBinding);
            NextButton.SetBinding(SkinStorage.VisualStyleProperty, visualStyleBinding);
            //NextButton.SetBinding(SkinStorage.VisualStylesListProperty, visualStyleListBinding);
        }
        
        /// <summary>
        /// Returns a geometry for a clipping mask. The mask applies if the layout system attempts to arrange an element that is larger than the available display space.
        /// </summary>
        /// <param name="layoutSlotSize">The size of the part of the element that does visual presentation.</param>
        /// <returns>The clipping geometry.</returns>
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            if (ClipToBounds && SplitterPage.GetTabStripPlacement(this) == Dock.Top)
            {
                return new RectangleGeometry(new Rect(0, -2, RenderSize.Width + VS2008_TAB_SHIFT, 2 * RenderSize.Height));
            }

            return null;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Navigates to the previous splitter page.  Shows and selects previous page.
        /// </summary>
        /// <param name="sender">Previous button that sent the event</param>
        /// <param name="e">Contains the event data.</param>
        private void OnPrevPageClick(object sender, RoutedEventArgs e)
        {
            int startIndex = m_startIndex;
            if (startIndex == 0)
            {
                startIndex = 1;
            }

            UIElement prevElement = InternalChildren[startIndex - 1];
            SplitterPage page = prevElement as SplitterPage;

            SplitterPagesCollection pageStorage = page.GetPageStorage();
            int selIndex = pageStorage.IndexOf(pageStorage.SelectedItem);
            selIndex = (selIndex != 0) ? selIndex - 1 : 0;

            SelectPage(pageStorage[selIndex]);

            if (m_canScrollPrev)
            {
                if (prevElement.DesiredSize.Width <= m_actualWidth)
                {
                    StartIndex--;
                    InvalidateArrange();
                }
            }
        }
        
        /// <summary>
        /// Navigates to the next splitter page. Shows and selects next page.
        /// </summary>
        /// <param name="sender">Next button that sent the event.</param>
        /// <param name="e">Contains the event data.</param>
        private void OnNextPageClick(object sender, RoutedEventArgs e)
        {
            int startIndex = m_startIndex;
            if (startIndex == InternalChildren.Count - 1)
            {
                startIndex--;
            }

            UIElement nextElement = InternalChildren[startIndex + 1];
            SplitterPage page = nextElement as SplitterPage;

            SplitterPagesCollection pageStorage = page.GetPageStorage();
            int selIndex = pageStorage.IndexOf(pageStorage.SelectedItem);
            selIndex = (selIndex != pageStorage.Count - 1) ? selIndex + 1 : pageStorage.Count - 1;

            SelectPage(pageStorage[selIndex]);

            if (m_canScrollNext)
            {
                if (nextElement.DesiredSize.Width <= m_actualWidth)
                {
                    StartIndex++;
                    InvalidateArrange();
                }
            }
        }
        #endregion
    }
}