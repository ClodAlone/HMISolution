// <copyright file="CustomGridSplitter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents split panel between top bottom items.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CustomGridSplitter : GridSplitter
    {
        #region Constants
        /// <summary>
        /// Presents name of split button.
        /// </summary>
        private const string BUTTON_NAME = "PART_SplitButton";

        /// <summary>
        /// Presents name of HorizontalButton
        /// </summary>
        private const string HORIZONTAL_BUTTON_NAME = "PART_HorizontalButton";

        /// <summary>
        /// Presents name of VerticalButton
        /// </summary>
        private const string VERTICAL_BUTTON_NAME = "PART_VerticalButton";

        /// <summary>
        /// Presents name of Expand and ColapseButtons
        /// </summary>
        private const string EXPAND_COLAPSE_BUTTONS_NAME = "PART_Expand_ColapseButtons";

        /// <summary>
        /// Contains name of the SplitBorder.
        /// </summary>
        private const string CsplitBorder = "PART_SplitBorder";

        /// <summary>
        /// Name of the element that contains preview line of dragging.
        /// </summary>
        private const string CdragPreviewLine = "DragPreviewLine";

        /// <summary>
        /// Contains message of the incorrect template exception.
        /// </summary>
        private const string CerrIncorrectTemplate = "Template is incorrect.";

        /// <summary>
        /// Name of the resized line.
        /// </summary>
        private const string CresizedLine = "ResizedOptionLine";

        /// <summary>
        /// Path to the vertical *.cur file.
        /// </summary>
        private const string CverticalCursorPath = "Syncfusion.Tools.WPF.Controls.TabSplitter.Themes.Resources.vResize.cur";

        /// <summary>
        /// Path to the horizontal *.cur file.
        /// </summary>
        private const string ChorizCursorPath = "Syncfusion.Tools.WPF.Controls.TabSplitter.Themes.Resources.hResize.cur";

        /// <summary>
        /// Contains CustomGridSplitter height.
        /// </summary>
        private const double CcustomGridSplitterHeight = 22;
        #endregion

        #region Private members
        /// <summary>
        /// Presents value of ChangeCursor.
        /// </summary>
        private bool m_changeCursor = false;

        /// <summary>
        /// Presents value of Resizing.
        /// </summary>
        private bool m_isResizing = false;

        /// <summary>
        /// Presents button for replacement of tab.
        /// </summary>
        private Button m_replacemenButton = null;

        /// <summary>
        /// Presents value of HorizontalButton.
        /// </summary>
        private Button m_horizontalButton = null;

        /// <summary>
        /// Presents value of VerticalButton.
        /// </summary>
        private Button m_verticalButton = null;

        /// <summary>
        /// Store Toptab header
        /// </summary>
        private string topHeader = string.Empty;

        /// <summary>
        /// Store Bottomtab header
        /// </summary>
        private string bottomHeader = string.Empty;

        /// <summary>
        /// Presents value of ExpandCollapseButton.
        /// </summary>
        private ToggleButton m_expandCollapseButton = null;

        /// <summary>
        /// Presents value of ParentTabSplitter.
        /// </summary>
        private TabSplitter m_parentTabSplitter = null;

        /// <summary>
        /// Contains preview line of dragging.
        /// </summary>
        private Canvas m_dragPreviewLine = null;

        /// <summary>
        /// Contains true when DragDelta event raised.
        /// </summary>
        private bool m_isDragDelta = false;

        /// <summary>
        /// Contains resized line.
        /// </summary>
        private Border m_resizeLine = null;

        /// <summary>
        /// Contains cursor of the vertical splitter.
        /// </summary>
        private Cursor m_verticalCursor = null;

        /// <summary>
        /// Contains cursor of the horizontal splitter.
        /// </summary>
        private Cursor m_horizontalCursor = null;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="CustomGridSplitter"/> class.
        /// </summary>
        static CustomGridSplitter()
        {
            FrameworkElement.CursorProperty.OverrideMetadata(typeof(CustomGridSplitter), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CustomGridSplitter.CoerceCursor)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGridSplitter"/> class.
        /// </summary>
        public CustomGridSplitter()
        {
            DragStarted += new DragStartedEventHandler(OnDragStarted);
            DragCompleted += new DragCompletedEventHandler(OnDragCompleted);
            DragDelta += new DragDeltaEventHandler(OnDragDelta);

            MouseDoubleClick += new MouseButtonEventHandler(OnMouseDoubleClick);
            this.Loaded += new RoutedEventHandler(CustomGridSplitter_Loaded);
        }

        void CustomGridSplitter_Loaded(object sender, RoutedEventArgs e)
        {
            m_resizeLine = (Border)GetTemplateChild(CresizedLine);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets height of the CustomGridSplitter.
        /// </summary>
        internal double CustomGridSplitterHeight
        {
            get
            {
                return CcustomGridSplitterHeight;
            }
        }

        /// <summary>
        /// Gets the replacement button.
        /// </summary>
        /// <value>The replacement button.</value>
        public Button ReplacemenButton
        {
            get
            {
                return m_replacemenButton;
            }
        }

        /// <summary>
        /// Gets the horizontal button.
        /// </summary>
        /// <value>The horizontal button.</value>
        public Button HorizontalButton
        {
            get
            {
                return m_horizontalButton;
            }
        }

        /// <summary>
        /// Gets the vertical button.
        /// </summary>
        /// <value>The vertical button.</value>
        public Button VerticalButton
        {
            get
            {
                return m_verticalButton;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_dragPreviewLine = Template.FindName(CdragPreviewLine, this) as Canvas;

            m_replacemenButton = (Button)GetTemplateChild(BUTTON_NAME);
            m_horizontalButton = (Button)GetTemplateChild(HORIZONTAL_BUTTON_NAME);
            m_verticalButton = (Button)GetTemplateChild(VERTICAL_BUTTON_NAME);
            m_expandCollapseButton = (ToggleButton)GetTemplateChild(EXPAND_COLAPSE_BUTTONS_NAME);

            m_resizeLine = (Border)GetTemplateChild(CresizedLine);
            if(m_resizeLine!=null)
                m_resizeLine.Unloaded += new RoutedEventHandler(m_resizeLine_Unloaded);
            if (null == m_replacemenButton || null == m_horizontalButton || null == m_verticalButton)
            {
                throw new NotImplementedException(CerrIncorrectTemplate);
            }

            if (null != m_horizontalButton)
            {
                m_horizontalButton.Click += new RoutedEventHandler(OnHorizontalButtonClick);
            }

            if (null != m_verticalButton)
            {
                m_verticalButton.Click += new RoutedEventHandler(OnVerticalButtonClick);
            }

            if (null != m_expandCollapseButton)
            {
                m_expandCollapseButton.Click += new RoutedEventHandler(OnExpandCollapseButtonClick);
                m_expandCollapseButton.MouseDoubleClick += new MouseButtonEventHandler(OnExpandCollapseButtonDoubleClick);
            }

            m_verticalCursor = GetCursor(Cursors.SizeWE, CverticalCursorPath);
            m_horizontalCursor = GetCursor(Cursors.SizeNS, ChorizCursorPath);
        }

        void m_resizeLine_Unloaded(object sender, RoutedEventArgs e)
        {
            m_resizeLine = null;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Invoked when an unhandled MouseDoubleClick routed event is raised on custom grid splitter element.
        /// </summary>
        /// <param name="sender"> The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Border border = e.OriginalSource as Border;

            if (border != null)
            {
                m_parentTabSplitter.CollapsePanel();
            }
        }

        /// <summary>
        /// Occurs one or more times as the mouse changes position when a Thumb control has logical focus and mouse capture.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The DragDeltaEventArgs instance containing the event data.</param>
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            m_isDragDelta = true;
        }

        /// <summary>
        /// Handles the Click event of the m_horizontalButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnHorizontalButtonClick(object sender, RoutedEventArgs e)
        {
            if (null != m_parentTabSplitter)
            {
                bool wasVertical = false;
                m_verticalButton.BorderBrush = Brushes.Transparent;
                if (this.m_parentTabSplitter != null)
                    m_horizontalButton.BorderBrush = this.m_parentTabSplitter.splitterbtnselectedborderbrush;
                m_expandCollapseButton.LayoutTransform = new RotateTransform(0);
                TabSplitterItem item = (TabSplitterItem)m_parentTabSplitter.SelectedItem;
                if (item.Orientation == Orientation.Vertical)
                {
                    wasVertical = true;
                }

                item.Orientation = Orientation.Horizontal;

                if (wasVertical && m_parentTabSplitter.BottomPanelHeight == 0 ||m_parentTabSplitter.TopPanelHeight==m_parentTabSplitter.BottomPanelHeight)
                {
                    m_parentTabSplitter.ExpandHeight(CcustomGridSplitterHeight);
                    m_parentTabSplitter.SetGridRowsHeight();
                }
                else if (m_parentTabSplitter.BottomPanel.Height == 0)
                {
                    m_parentTabSplitter.CollapsePanel();
                }

                if (m_parentTabSplitter.iscollapse == 1)
                {                    
                    m_parentTabSplitter.CollapsePanel();
                }
                m_parentTabSplitter.RestoreBottomPages(item, item.TopPanelItems, item.BottomPanelItems);

                SplitterPagesCollection topItems = item.TopPanelItems;
                SplitterPagesCollection bottomItems = item.BottomPanelItems;

                SplitterPage topPage = topItems.SelectedItem;
                SplitterPage bottomPage = bottomItems.SelectedItem;
                if (bottomPage != null && topPage != null)
                {
                    if (topPage.Header.ToString() != string.Empty)
                    {
                        topHeader = topPage.Header.ToString();
                        bottomHeader = bottomPage.Header.ToString();
                    }
                    else if (topPage.ToolTip.ToString() != string.Empty)
                    {
                        topHeader = topPage.ToolTip.ToString();
                        bottomHeader = bottomPage.ToolTip.ToString();
                    }

                    if (item.Orientation == Orientation.Vertical)
                    {
                        topPage.Header = string.Empty;
                        bottomPage.Header = string.Empty;
                        topPage.ToolTip = topHeader;
                        bottomPage.ToolTip = bottomHeader; 
                    }
                    else
                    {
                        if (!wasVertical)
                        {
                            topPage.Header = topHeader;
                            bottomPage.Header = bottomHeader;
                        }
                        else
                        {
                            topPage.Header = topPage.m_Header;
                            bottomPage.Header = bottomPage.m_Header;
                        }
                    }
                }

                ClearPagesTabPlacement();
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Clears TabStripPlacementPropertyKey values of all pages.
        /// </summary>
        private void ClearPagesTabPlacement()
        {
            ItemCollection topPages = m_parentTabSplitter.TopPages.Items;

            for (int i = 0, cnt = topPages.Count; i < cnt; i++)
            {
                SplitterPage page = (SplitterPage)topPages[i];
                page.ClearValue(SplitterPage.TabStripPlacementPropertyKey);
            }

            ItemCollection botPages = m_parentTabSplitter.BottomPages.Items;

            for (int i = 0, cnt = botPages.Count; i < cnt; i++)
            {
                SplitterPage page = (SplitterPage)botPages[i];
                page.ClearValue(SplitterPage.TabStripPlacementPropertyKey);
            }
        }

        /// <summary>
        /// Handles the Click event of the m_verticalButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnVerticalButtonClick(object sender, RoutedEventArgs e)
        {
            if (null != m_parentTabSplitter)
            {
                bool wasHorizontal = false;
                m_horizontalButton.BorderBrush = Brushes.Transparent;
                if(m_parentTabSplitter!=null)
                m_verticalButton.BorderBrush = this.m_parentTabSplitter.splitterbtnselectedborderbrush;
                m_expandCollapseButton.LayoutTransform = new RotateTransform(180);
                TabSplitterItem item = (TabSplitterItem)m_parentTabSplitter.SelectedItem;
                if (item.Orientation == Orientation.Horizontal)
                {
                    wasHorizontal = true;
                }

                item.Orientation = Orientation.Vertical;

                if (wasHorizontal && m_parentTabSplitter.RightPanelWidth == 0 ||m_parentTabSplitter.LeftPanelWidth==m_parentTabSplitter.RightPanelWidth)
                {
                    m_parentTabSplitter.ExpandWidth(CcustomGridSplitterHeight);
                    m_parentTabSplitter.SetGridRowsWidth();
                }
                else if (m_parentTabSplitter.BottomPanel.Width == 0)
                {
                    m_parentTabSplitter.CollapsePanel();
                }

                if (m_parentTabSplitter.iscollapse == 1)
                {                    
                    m_parentTabSplitter.CollapsePanel();
                }
                SplitterPagesCollection topItems = item.TopPanelItems;
                SplitterPagesCollection bottomItems = item.BottomPanelItems;

                SplitterPage topPage = topItems.SelectedItem;
                SplitterPage bottomPage = bottomItems.SelectedItem;
                if (topPage != null && bottomPage != null)
                {
                    if (topPage.Header.ToString() != string.Empty)
                    {
                        topHeader = topPage.Header.ToString();
                        bottomHeader = bottomPage.Header.ToString();
                    }
                    else if (topPage.ToolTip.ToString() != string.Empty)
                    {
                        topHeader = topPage.ToolTip.ToString();
                        bottomHeader = bottomPage.ToolTip.ToString();
                    }

                    if (item.Orientation == Orientation.Vertical)
                    {
                        topPage.Header =string.Empty;
                        bottomPage.Header = string.Empty;
                        topPage.ToolTip = topHeader;
                        bottomPage.ToolTip = bottomHeader; 
                    }
                    else
                    {
                        topPage.Header = topHeader;                        
                        bottomPage.Header = bottomHeader;
                    }
                }

                m_parentTabSplitter.RestoreBottomPages(item, item.TopPanelItems, item.BottomPanelItems);

                ClearPagesTabPlacement();
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Handles the Click event of the m_Expand_CollapseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnExpandCollapseButtonClick(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the DoubleClick event of the m_Expand_CollapseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnExpandCollapseButtonDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Called when [custom grid splitter drag completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            m_isResizing = false;

            Grid resizedGrid = m_parentTabSplitter.ResizedGrid;
            TabSplitterItem item = m_parentTabSplitter.SelectedSplitterItem;
            if (e.VerticalChange != 0 || e.HorizontalChange != 0)
            {
                if (ResizeDirection == GridResizeDirection.Rows)
                {
                    double topHeight = resizedGrid.RowDefinitions[0].Height.Value;
                    m_parentTabSplitter.TopPanelHeight = topHeight;

                    double bottomHeight = resizedGrid.ActualHeight - topHeight - CcustomGridSplitterHeight;

                    if (bottomHeight > 0)
                    {
                        m_parentTabSplitter.BottomPanelHeight = bottomHeight;
                        m_parentTabSplitter.BottomPanelHeightRatio = bottomHeight / resizedGrid.ActualHeight;
                        m_parentTabSplitter.iscollapse = 1;
                    }
                    else
                    {
                        m_parentTabSplitter.CollapsePanel();
                        m_parentTabSplitter.iscollapse = 0;
                        InvalidateArrange();
                    }

                    if (m_parentTabSplitter.iscollapse == 1)
                    {
                        m_parentTabSplitter.CollapsePanel();
                        m_parentTabSplitter.TopPanelHeight = topHeight;
                        m_parentTabSplitter.BottomPanelHeight = bottomHeight;
                        m_parentTabSplitter.SetGridRowsHeight();
                        InvalidateArrange();
                    }

                    HorisRestoreBottomPages(item);
                }
                else
                {
                    double leftWidth = resizedGrid.ColumnDefinitions[0].Width.Value;
                    m_parentTabSplitter.LeftPanelWidth = leftWidth;

                    double rightWidth = resizedGrid.ActualWidth - leftWidth - CcustomGridSplitterHeight;

                    if (rightWidth > 0)
                    {
                        m_parentTabSplitter.RightPanelWidth = rightWidth;
                        m_parentTabSplitter.RightPanelWidthRatio = rightWidth / resizedGrid.ActualWidth;
                        item.m_rightpanelwidthratio = rightWidth / resizedGrid.ActualWidth;
                        m_parentTabSplitter.iscollapse = 1;
                    }
                    else
                    {
                        m_parentTabSplitter.CollapsePanel();
                        m_parentTabSplitter.iscollapse = 0;
                        InvalidateArrange();
                    }

                    if (m_parentTabSplitter.iscollapse == 1)
                    {
                        m_parentTabSplitter.CollapsePanel();
                        m_parentTabSplitter.RightPanelWidth = rightWidth;
                        m_parentTabSplitter.LeftPanelWidth = leftWidth;
                        m_parentTabSplitter.SetGridRowsWidth();
                        InvalidateArrange();
                        VertRestoreBottomPages(item);
                    }
                }
            }
        }

        /// <summary>
        /// Restores bottom pages of the splitter and sets height to the panels of the splitter item.
        /// </summary>
        /// <param name="item">The item value.</param>
        private void HorisRestoreBottomPages(TabSplitterItem item)
        {
            if (m_parentTabSplitter.BottomPanelHeight > 0)
            {
                m_parentTabSplitter.RestoreBottomPages(item, item.TopPanelItems, item.BottomPanelItems);
                item.ItemTopPanelHeight = m_parentTabSplitter.TopPanelHeight;
                item.ItemBottomPanelHeight = m_parentTabSplitter.BottomPanelHeight;

                InvalidateArrange();
            }
        }

        /// <summary>
        /// Restores bottom pages of the splitter and sets height to the panels of the splitter item.
        /// </summary>
        /// <param name="item">The item value.</param>
        private void VertRestoreBottomPages(TabSplitterItem item)
        {
            if (m_parentTabSplitter.LeftPanelWidth > 0)
            {
                m_parentTabSplitter.RestoreBottomPages(item, item.TopPanelItems, item.BottomPanelItems);
                item.ItemLeftPanelWidht = m_parentTabSplitter.LeftPanelWidth;
                item.ItemRightPanelWidth = m_parentTabSplitter.RightPanelWidth;

                InvalidateArrange();
            }
        }

        /// <summary>
        /// Called when [custom grid splitter drag started].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            m_isResizing = true;
            CoerceValue(CursorProperty);
        }

        /// <summary>
        /// Coerces the cursor.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="value">The value.</param>
        /// <returns>object type</returns>
        private static object CoerceCursor(DependencyObject d, object value)
        {
            CustomGridSplitter splitter = (CustomGridSplitter)d;

            if ((value == null) && (splitter != null && (splitter.m_changeCursor || splitter.m_isResizing)))
            {
                splitter.m_changeCursor = false;

                switch (splitter.ResizeDirection)
                {
                    case GridResizeDirection.Columns:
                        if (splitter.m_resizeLine != null)
                            splitter.m_resizeLine.Cursor = splitter.m_verticalCursor;
                        return splitter.m_verticalCursor;
                    case GridResizeDirection.Rows:
                        if (splitter.m_resizeLine != null)
                            splitter.m_resizeLine.Cursor = splitter.m_horizontalCursor;
                        return splitter.m_horizontalCursor;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets cursor for horizontal or vertical splitter.
        /// </summary>
        /// <param name="defaultCursor">default cursor</param>
        /// <param name="path">path to *.cur file</param>
        /// <returns>created cursor</returns>
        private static Cursor GetCursor(Cursor defaultCursor, string path)
        {
            Cursor cursor = null;
            Assembly ass = Assembly.GetExecutingAssembly();
            Stream stream = null;

            if (EnvironmentTest.IsSecurityGranted)
            {
                stream = ass.GetManifestResourceStream(path);
            }

            cursor = (stream != null) ? new Cursor(stream) : defaultCursor;
            return cursor;
        }

        /// <summary>
        /// Sets visible or collapsed to split button when splitter is expanded or collapsed.
        /// </summary>
        private void UpdateSplitButtonVisibility()
        {
            if (m_parentTabSplitter.BottomPages.Items.Count == 0)
            {
                ReplacemenButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ReplacemenButton.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Arranges the content of a CustomGridSplitter element.
        /// </summary>
        /// <param name="arrangeBounds">this element uses to arrange its child content.</param>
        /// <returns>The Size that represents the arranged size of CustomGridSplitter.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Grid resizedGrid = m_parentTabSplitter.ResizedGrid;
            LayoutPanel topPanel = m_parentTabSplitter.TopPanel;
            LayoutPanel bottomPanel = m_parentTabSplitter.BottomPanel;

            if (m_parentTabSplitter.GridSplitter.ResizeDirection == GridResizeDirection.Rows)
            {
               // double panelsWidth = resizedGrid.ActualWidth;
                double panelsWidth = 0.0;
                panelsWidth= resizedGrid.DesiredSize.Width;
                
                TabSplitterItem selected = m_parentTabSplitter.SelectedSplitterItem;

                topPanel.Width = panelsWidth;
                bottomPanel.Width = panelsWidth;

                if (selected != null && !double.IsInfinity(selected.ItemTopPanelHeight) && ! double.IsInfinity(selected.ItemBottomPanelHeight))
                {
                    topPanel.Height = selected.ItemTopPanelHeight;

					if (resizedGrid.IsLoaded && resizedGrid.ActualHeight > (selected.ItemTopPanelHeight + CcustomGridSplitterHeight))
					{
						bottomPanel.Height = resizedGrid.ActualHeight - CcustomGridSplitterHeight - selected.ItemTopPanelHeight;
						//m_parentTabSplitter.BottomPanelHeightRatio = (bottomPanel.Height / resizedGrid.ActualHeight);
					}
					else
						bottomPanel.Height = selected.ItemBottomPanelHeight;
                }
            }
            else
            {
                double panelsHeight = resizedGrid.DesiredSize.Height;
                topPanel.Height = panelsHeight;
                bottomPanel.Height = panelsHeight;

                if (m_parentTabSplitter.LeftPanelWidth == 0 && m_parentTabSplitter.RightPanelWidth == 0)
                {
                    double middleWidth = (resizedGrid.ActualWidth / 2) - (arrangeBounds.Width / 2);
                    m_parentTabSplitter.LeftPanelWidth = middleWidth;
                    m_parentTabSplitter.RightPanelWidth = middleWidth;
                }

                //topPanel.Width = resizedGrid.DesiredSize.Width / 2;
                //bottomPanel.Width = topPanel.Width;                
                TabSplitterItem selected = m_parentTabSplitter.SelectedSplitterItem;
                if (selected != null && selected.ItemLeftPanelWidht >= 0 && selected.ItemRightPanelWidth >= 0)
                {
                    topPanel.Width = selected.ItemLeftPanelWidht;
                    bottomPanel.Width = selected.ItemRightPanelWidth;                    
                }
                else
                {
                    topPanel.Width = resizedGrid.DesiredSize.Width / 2;
                    bottomPanel.Width = topPanel.Width;
                }
            }

            UpdateSplitButtonVisibility();

            return base.ArrangeOverride(arrangeBounds);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            m_parentTabSplitter = TemplatedParent as TabSplitter;
         }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            m_changeCursor = false;

            if (e.OriginalSource is Border && ((Border)e.OriginalSource).Name == CsplitBorder)
            {
                m_changeCursor = true;
            }

            CoerceValue(CursorProperty);
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseMove"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.OriginalSource is Border && ((Border)e.OriginalSource).Name == CsplitBorder)
            {
                m_changeCursor = true;
            }

            if (m_isDragDelta && IsDragging)
            {
                

                if (ResizeDirection == GridResizeDirection.Rows)
                {
                    m_dragPreviewLine.Visibility = Visibility.Visible;
                    m_dragPreviewLine.Arrange(new Rect(0, e.GetPosition(this).Y, m_dragPreviewLine.ActualWidth, m_dragPreviewLine.ActualHeight));
                }
                else
                {
                    double newYPosition = -e.GetPosition(this).X + 20;
                    m_dragPreviewLine.Visibility = Visibility.Visible;
                    m_dragPreviewLine.Arrange(new Rect(0, newYPosition, m_dragPreviewLine.ActualWidth, m_dragPreviewLine.ActualHeight));
                }
            }

            CoerceValue(CursorProperty);
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.ContentElement.MouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Border)
            {
                string borderName = ((Border)e.OriginalSource).Name;

                if (borderName == CsplitBorder || borderName == CresizedLine)
                {
                    m_isDragDelta = false;

                    base.OnMouseLeftButtonDown(e);
                }
            }
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.ContentElement.MouseLeftButtonUp"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            m_isDragDelta = false;
            m_dragPreviewLine.Visibility = Visibility.Collapsed;

            base.OnMouseLeftButtonUp(e);
        }

        #endregion
    }
}
