// <copyright file="TabSplitter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents TabSplitter partial class
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
    [SkinType(SkinVisualStyle = Skin.Default,
 Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
 Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
 Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
Type = typeof(TabSplitter), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/TransparentStyle.xaml")]
    public partial class TabSplitter : Selector
    {
        #region Constants
        /// <summary>
        /// Presents topPages
        /// </summary>
        private const string C_topPages = "PART_TopPages";

        /// <summary>
        /// Presents bottomPages
        /// </summary>
        private const string C_bottomPages = "PART_BottomPages";

        /// <summary>
        /// Contains name of the grid that changes resize direction.
        /// </summary>
        private const string C_resizedContentGrid = "ResizedContentGrid";

        /// <summary>
        /// Name of the inner border.
        /// </summary>
        private const string C_contentPanelInnerBorder = "PART_ContentPanelInnerBorder";

        /// <summary>
        /// Name of the splitter.
        /// </summary>
        private const string C_splitter = "PART_Splitter";

        /// <summary>
        /// Message of the exception when template is incorrect.
        /// </summary>
        private const string C_errMess = "Template is incorrect.";

        /// <summary>
        /// Name of the Expand_ColapseButton.
        /// </summary>
        private const string C_expandCollapleButton = "PART_Expand_ColapseButtons";

        /// <summary>
        /// Tooltip text of the button in the collapsed state.
        /// </summary>
        private const string C_expandTooltip = "Expand Pane";

        /// <summary>
        /// Tooltip text of the button in the expanded state.
        /// </summary>
        private const string C_collapseTooltip = "Collapse Pane";

        #endregion

        #region Private members
        /// <summary>
        /// Presents CollapseButtuon
        /// </summary>
        internal ToggleButton m_expand_CollapseButtuon = null;

		/// <summary>
		/// Caches the bottom panel height ratio, later used in measure override.
		/// </summary>
		internal double BottomPanelHeightRatio = 0;

        internal double RightPanelWidthRatio = 0;

        /// <summary>
        /// Presents ListMenuButton
        /// </summary>
        private ToggleButton m_listMenuButton = null;

        /// <summary>
        /// Presents TopPanel
        /// </summary>
        private LayoutPanel m_topPanel = null;

        /// <summary>
        /// Presents BottomPanel
        /// </summary>
        private LayoutPanel m_bottomPanel = null;

        /// <summary>
        /// Presents TopPages
        /// </summary>
        private ItemsControl m_topPages = null;

        /// <summary>
        /// Presents BottomPages
        /// </summary>
        private ItemsControl m_bottomPages = null;

        /// <summary>
        /// Presents GridSplitter
        /// </summary>
        internal CustomGridSplitter m_gridSplitter = null;

        /// <summary>
        /// 
        /// </summary>
        internal ScrollContentPresenter TopContentPresenter;

        /// <summary>
        /// 
        /// </summary>
        internal ScrollContentPresenter BottomContentPresenter;

        /// <summary>
        /// Store Toptab header
        /// </summary>
        private string topHeader = string.Empty;
        
        /// <summary>
        /// Store Bottomtab header
        /// </summary>
        private string bottomHeader = string.Empty;

        /// <summary>
        /// Presents TabPanel
        /// </summary>
        private TabPanelAdv m_tabPanel;

        /// <summary>
        /// Presents borderContentPanel
        /// </summary>
        private Border m_borderContentPanel;

        /// <summary>
        /// Presents TabSplitterItemPanel
        /// </summary>
        private TabSplitterItemPanel m_tabSplitterItemPanel;

        /// <summary>
        /// Contains grid that changes resize direction.
        /// </summary>
        private Grid m_resizedGrid = null;

        /// <summary>
        /// Contains rows of the resized grid (is used when resize direction=rows)
        /// </summary>
        private RowDefinitionCollection m_gridRows = null;

        /// <summary>
        /// Contains columns of the resized grid (is used when resize direction=cols)
        /// </summary>
        private ColumnDefinitionCollection m_gridCols = null;

        /// <summary>
        /// Contains height of the row that must collapse (is used when resize direction=rows)
        /// </summary>
        private double m_collapsedRowHeight = 0;

        /// <summary>
        /// Contains width of the column that must collapse (is used when resize direction=cols)
        /// </summary>
        private double m_collapsedColWidth = 0;

        internal double bottomPanelHeight = 0;

        //SU I78477
        //internal double Margin = 0;
        internal new double Margin = 0;
        //EU I78477

        internal double requiredHeight = 0;

        internal double requiredWidth = 0;
        /// <summary>
        /// Presents VerticalButtuon
        /// </summary>
        internal Button m_verticalButton = null;

        /// <summary>
        /// Presents HorizontalButtuon
        /// </summary>
        internal Button m_horizontalButton = null;

        /// <summary>
        /// Contains inner Border of the content panel.
        /// </summary>
        internal Border m_contentPanelInnerBorder = null;
        internal int iscollapse = 0;
        internal int noChange = 0;
        internal  Brush splitterbtnselectedborderbrush;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="TabSplitter"/> class.
        /// </summary>
        static TabSplitter()
        {
            EnvironmentTest.ValidateLicense(typeof(TabSplitter));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabSplitter), new FrameworkPropertyMetadata(typeof(TabSplitter)));
            Control.IsTabStopProperty.OverrideMetadata(typeof(TabSplitter), new FrameworkPropertyMetadata(false));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabSplitter"/> class.
        /// </summary>
        public TabSplitter()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(TabSplitter));
            }
            CommandBinding closeCurrentTabSplitterItemCommandBinding = new CommandBinding(TabSplitterCommands.CloseCurrentTabSplitterItem, ProcessCloseCurrentTabSplitterItemCommand, CanProcessCloseCurrentTabSplitterItemCommand);
            CommandBinding openContextMenuCommandBinding = new CommandBinding(TabSplitterCommands.OpenContextMenu, ProcessOpenContextMenu, CanProcessOpenContextMenu);
            CommandBinding collapseBottomSelectedItemCommandBinding = new CommandBinding(TabSplitterCommands.CollapseBottomSelectedItem, ProcessCollapseBottomSelectedItemCommand, CanProcessCollapseBottomSelectedItemCommand);

            CommandBindings.Add(closeCurrentTabSplitterItemCommandBinding);
            CommandBindings.Add(openContextMenuCommandBinding);
            CommandBindings.Add(collapseBottomSelectedItemCommandBinding);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Sets the Splitter distance from Top
        /// </summary>
        public double SplitterDistanceFromTop
        {
            get
            {
                return (double)GetValue(SplitterDistanceFromTopProperty);
            }
            set
            {
                SetValue(SplitterDistanceFromTopProperty, value);
            }
        }

        /// <summary>
        /// Gets CustomGridSplitter.
        /// </summary>
        internal CustomGridSplitter GridSplitter
        {
            get
            {
                return m_gridSplitter;
            }
        }

        /// <summary>
        /// Gets the tab panel.
        /// </summary>
        /// <value>The tab panel.</value>
        protected TabPanelAdv TabPanel
        {
            get
            {
                if (m_tabPanel == null)
                {
                    m_tabPanel = (TabPanelAdv)GetTemplateChild("PART_TabPanel");
                }

                return m_tabPanel;
            }
        }

        /// <summary>
        /// Gets the resized grid.
        /// </summary>
        /// <value>The resized grid.</value>
        internal Grid ResizedGrid
        {
            get
            {
                if (m_resizedGrid == null)
                {
                    m_resizedGrid = GetResizedGrid();
                }

                return m_resizedGrid;
            }
        }

        /// <summary>
        /// Gets the top panel.
        /// </summary>
        /// <value>The top panel.</value>
        internal LayoutPanel TopPanel
        {
            get
            {
                if (m_topPanel == null)
                {
                    m_topPanel = (LayoutPanel)GetTemplateChild("PART_TopPanel");
                }

                return m_topPanel;
            }
        }

        /// <summary>
        /// Gets the bottom panel.
        /// </summary>
        /// <value>The bottom panel.</value>
        internal LayoutPanel BottomPanel
        {
            get
            {
                if (m_bottomPanel == null)
                {
                    m_bottomPanel = (LayoutPanel)GetTemplateChild("PART_BottomPanel");
                }

                return m_bottomPanel;
            }
        }

        /// <summary>
        /// Gets the bottom pages.
        /// </summary>
        /// <value>The bottom pages.</value>
        internal ItemsControl BottomPages
        {
            get
            {
                if (m_bottomPages == null)
                {
                    m_bottomPages = (ItemsControl)GetTemplateChild(C_bottomPages);
                }

                return m_bottomPages;
            }
        }

        /// <summary>
        /// Gets the top pages.
        /// </summary>
        /// <value>The top pages.</value>
        internal ItemsControl TopPages
        {
            get
            {
                if (m_topPages == null)
                {
                    m_topPages = (ItemsControl)GetTemplateChild(C_topPages);
                }

                return m_topPages;
            }
        }

        /// <summary>
        /// Gets the tab layout panel.
        /// </summary>
        /// <value>The tab layout panel.</value>
        protected internal Border BorderContentPanel
        {
            get
            {
                if (m_borderContentPanel == null && TabPanel != null)
                {
                    m_borderContentPanel = (Border)GetTemplateChild("ContentPanel");
                }

                return m_borderContentPanel;
            }
        }

        /// <summary>
        /// Gets the value of the BottomSelectedContent dependency property.
        /// </summary>
        public object BottomSelectedContent
        {
            get
            {
                return GetValue(BottomSelectedContentProperty);
            }

            internal set
            {
                SetValue(BottomSelectedContentPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the value of the TopSelectedContent dependency property.
        /// </summary>
        public object TopSelectedContent
        {
            get
            {
                return GetValue(TopSelectedContentProperty);
            }

            internal set
            {
                SetValue(TopSelectedContentPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab splitter list context menu item template.
        /// </summary>
        /// <value>The tab splitter list context menu item template.</value>
        public DataTemplate TabSplitterListContextMenuItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TabSplitterListContextMenuItemTemplateProperty);
            }

            set
            {
                SetValue(TabSplitterListContextMenuItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the tab layout panel.
        /// </summary>
        /// <value>The tab layout panel.</value>
        protected internal TabSplitterItemPanel TabLayoutPanel
        {
            get
            {
                if (m_tabSplitterItemPanel == null && TabPanel != null)
                {
                    m_tabSplitterItemPanel = (TabSplitterItemPanel)GetTemplateChild("PART_TabLayoutPanel");
                }

                return m_tabSplitterItemPanel;
            }
        }

        /// <summary>
        /// Gets or sets TopPanelHeight property. Is used to regulate height of the top panel 
        /// (in the 0 row of the resized grid when resize direction = rows).
        /// </summary>
        internal double TopPanelHeight
        {
            get
            {
                return (double)GetValue(TopPanelHeightProperty);
            }

            set
            {
                SetValue(TopPanelHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets BottomPanelHeight property. Is used to regulate height of the bottom panel 
        /// (in the 2 row of the resized grid when resize direction = rows).
        /// </summary>
        public double BottomPanelHeight
        {
            get
            {
                return (double)GetValue(BottomPanelHeightProperty);
            }

            set
            {
                SetValue(BottomPanelHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets LeftPanelWidth property. Is used to regulate width of the left panel 
        /// (in the 0 column of the resized grid when resize direction = cols).
        /// </summary>
        internal double LeftPanelWidth
        {
            get
            {
                return (double)GetValue(LeftPanelWidthProperty);
            }

            set
            {
                SetValue(LeftPanelWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets RightPanelWidth property. Is used to regulate width of the right panel 
        /// (in the 2 column of the resized grid when resize direction = cols).
        /// </summary>
        internal double RightPanelWidth
        {
            get
            {
                return (double)GetValue(RightPanelWidthProperty);
            }

            set
            {
                SetValue(RightPanelWidthProperty, value);
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (null != m_gridSplitter)
            {
                m_gridSplitter.ReplacemenButton.Click -= new RoutedEventHandler(OnReplacemenButtonClick);
            }

            base.OnApplyTemplate();
            string currentskin = SkinStorage.GetVisualStyle(this);
            splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromArgb(255, 49, 106, 197));
            if (currentskin == "Default")
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromArgb(255, 49, 106, 197));
            if (currentskin == "Office2007Blue")
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromArgb(255, 49, 106, 197));
            if (currentskin == "Office2007Black")
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromRgb(83, 83, 83));
            if (currentskin == "Office2007Silver")
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            if (currentskin == "Office2010Blue")
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromArgb(255, 49, 106, 197));
            if (currentskin == "Office2010Silver")
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            if (currentskin == "Office2010Black")
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromRgb(71, 71, 71));
            if (currentskin == "Blend")
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromRgb(178, 178, 178));
            if (currentskin == "Transparent")
          {
                splitterbtnselectedborderbrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            }

            UpdateSelectedContent();
            m_gridSplitter = GetTemplateChild(C_splitter) as CustomGridSplitter;
            m_contentPanelInnerBorder = GetTemplateChild(C_contentPanelInnerBorder) as Border;
            TopContentPresenter = GetTemplateChild("PART_TopScrollContent") as ScrollContentPresenter;
            BottomContentPresenter = GetTemplateChild("PART_BottomScrollContent") as ScrollContentPresenter;

            if (null == m_gridSplitter)
            {
                throw new NotImplementedException(C_errMess);
            }

            m_resizedGrid = GetResizedGrid();
            m_gridRows = m_resizedGrid.RowDefinitions;
            m_gridCols = m_resizedGrid.ColumnDefinitions;
            if (m_gridSplitter != null)
            {
                m_gridSplitter.ApplyTemplate();
                m_expand_CollapseButtuon = (ToggleButton)m_gridSplitter.Template.FindName(C_expandCollapleButton, m_gridSplitter);
                m_verticalButton = (Button)m_gridSplitter.Template.FindName("PART_VerticalButton", m_gridSplitter);
                m_horizontalButton = (Button)m_gridSplitter.Template.FindName("PART_HorizontalButton", m_gridSplitter);
                m_gridSplitter.ReplacemenButton.Click += new RoutedEventHandler(OnReplacemenButtonClick);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Verifies the index of the Z.
        /// </summary>
        protected internal void VerifyZIndex()
        {
            int count = Items.Count - 1;
            int firstTabIndex;

            for (firstTabIndex = 0; firstTabIndex <= count; firstTabIndex++)
            {
                TabSplitterItem element = GetTabSplitterItem(Items[firstTabIndex]);

                if (element.Visibility == Visibility.Visible)
                {
                    break;
                }
            }

            for (int i = 0; i <= count; i++)
            {
                TabSplitterItem element = GetTabSplitterItem(Items[i]);

                if (element != null)
                {
                    int zIndex = i == SelectedIndex ? 10000 : count - i;

                    if (i == firstTabIndex)
                    {
                        zIndex = 9999;
                    }

                    Panel.SetZIndex(element, zIndex);
                }
            }
        }

        /// <summary>
        /// Items the initialization.
        /// </summary>
        /// <param name="topPages">The top pages.</param>
        /// <param name="bottomPages">The bottom pages.</param>
        internal void ItemsInitialization(ItemsControl topPages, ItemsControl bottomPages)
        {
            if (topPages != null && bottomPages != null)
            {
                m_topPages = topPages;
                m_bottomPages = bottomPages;
                UpdateOnSelection();
            }
            else
            {
                throw new NullReferenceException("parameters can not be null");
            }
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabSplitterItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabSplitterItem;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ItemContainerGenerator.StatusChanged += new EventHandler(OnGeneratorStatusChanged);
        }

        /// <summary>
        /// Updates the current selection when an item in the <see cref="T:System.Windows.Controls.Primitives.Selector"/> has changed
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if ((e.Action == NotifyCollectionChangedAction.Remove) && (SelectedIndex == -1))
            {
                int startIndex = e.OldStartingIndex + 1;

                if (startIndex > Items.Count)
                {
                    startIndex = 0;
                }

                TabSplitterItem item = FindNextTabSplitterItem(startIndex, -1);

                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            int direction = 0;
            int startIndex = -1;
            switch (e.Key)
            {
                case Key.End:
                    direction = -1;
                    startIndex = Items.Count;
                    break;

                case Key.Home:
                    direction = 1;
                    startIndex = -1;
                    break;

                case Key.Tab:
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        startIndex = ItemContainerGenerator.IndexFromContainer(ItemContainerGenerator.ContainerFromItem(SelectedItem));
                        direction = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? -1 : 1;
                    }

                    break;
            }

            TabSplitterItem item = FindNextTabSplitterItem(startIndex, direction);

            if ((item != null) && (item != SelectedItem))
            {
                e.Handled = item.SetFocus();
            }

            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            TabSplitterItem oldSelectedItem = null;
            TabSplitterItem newSelectedItem = null;

            if (e.RemovedItems.Count > 0)
            {
                oldSelectedItem = e.RemovedItems[0] as TabSplitterItem;
            }

            if (e.AddedItems.Count > 0)
            {
                newSelectedItem = e.AddedItems[0] as TabSplitterItem;
                UpdateSelectedBottomPanel(newSelectedItem);
            }

            if (oldSelectedItem != null)
            {
                oldSelectedItem.SplitterPagesSelectionChanged -= new SplitterPagesSelectionChangedEventHandler(OnNewSelectedItemSplitterPagesSelectionChanged);
                oldSelectedItem.m_rightpanelwidthratio = RightPanelWidthRatio;
            }

            if (newSelectedItem != null)
            {
                newSelectedItem.SplitterPagesSelectionChanged += new SplitterPagesSelectionChangedEventHandler(OnNewSelectedItemSplitterPagesSelectionChanged);

                if (newSelectedItem.ItemBottomPanelHeight == 0 && newSelectedItem.ItemTopPanelHeight == 0)
                {
                    if (SplitterDistanceFromTop != 0.0) TopPanelHeight = SplitterDistanceFromTop;
                    newSelectedItem.ItemTopPanelHeight = requiredHeight;
                    newSelectedItem.ItemBottomPanelHeight = requiredHeight;
                }
                if (newSelectedItem.ItemLeftPanelWidht == 0 && newSelectedItem.ItemRightPanelWidth == 0)
                {
                    newSelectedItem.ItemLeftPanelWidht = requiredWidth;
                    newSelectedItem.ItemRightPanelWidth = requiredWidth;
                }
                RightPanelWidthRatio = newSelectedItem.m_rightpanelwidthratio;

                if (m_gridSplitter != null)
                    m_gridSplitter.InvalidateArrange();
            }

            UpdateOnSelection();

            if (newSelectedItem != null)
            {
                SplitterPage page = GetTopSelectedPage(newSelectedItem);
                CorrectMaxMinSize(TopContentPresenter, page);
                page = GetBottomSelectedPage(newSelectedItem);
                CorrectMaxMinSize(BottomContentPresenter, page);
            }
        }

        /// <summary>
        /// Called when [new selected item splitter pages selection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.SplitterPagesSelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnNewSelectedItemSplitterPagesSelectionChanged(object sender, SplitterPagesSelectionChangedEventArgs e)
        {
            SplitterPage selectedPage = e.NewSelectedPage;

            if (null != selectedPage)
            {
                switch (SplitterPage.GetTabStripPlacement(selectedPage))
                {
                    case Dock.Bottom:
                        TopSelectedContent = e.NewSelectedPage.Content;
                        CorrectMaxMinSize(TopContentPresenter, e.NewSelectedPage);
                        break;
                    case Dock.Left:
                        TopSelectedContent = e.NewSelectedPage.Content;
                        CorrectMaxMinSize(TopContentPresenter, e.NewSelectedPage);
                        break;
                    case Dock.Right:
                        BottomSelectedContent = e.NewSelectedPage.Content;
                        CorrectMaxMinSize(BottomContentPresenter, e.NewSelectedPage);
                        break;
                    case Dock.Top:
                        BottomSelectedContent = e.NewSelectedPage.Content;
                        CorrectMaxMinSize(BottomContentPresenter, e.NewSelectedPage);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Updates the on selection.
        /// </summary>
        internal void UpdateOnSelection()
        {
            TabSplitterItem selectedTabSplitterItem = GetSelectedTabSplitterItem();

            if (selectedTabSplitterItem != null && m_topPages != null)
            {
                m_topPages.ItemsSource = selectedTabSplitterItem.TopPanelItems;
                m_bottomPages.ItemsSource = selectedTabSplitterItem.BottomPanelItems;

                SelectFistPage(selectedTabSplitterItem.TopPanelItems);
                SelectFistPage(selectedTabSplitterItem.BottomPanelItems);

                m_topPages.ClearValue(SplitterPage.TabStripPlacementPropertyKey);
                m_bottomPages.ClearValue(SplitterPage.TabStripPlacementPropertyKey);

                SplitterPage.SetTabStripPlacement(m_topPages, Dock.Bottom);
                SplitterPage.SetTabStripPlacement(m_bottomPages, Dock.Top);

                SplitterPage bottomSelectedPage = GetBottomSelectedPage(selectedTabSplitterItem);
                SplitterPage topSelectedPage = GetTopSelectedPage(selectedTabSplitterItem);

                TopSelectedContent = topSelectedPage.Content;
                BottomSelectedContent = bottomSelectedPage.Content;
            }

            UpdateSelectedContent();
            VerifyZIndex();
        }

        /// <summary>
        /// Gets selected page of the top items. If SelectedItem=null than creates new SplitterPage.
        /// </summary>
        /// <param name="selectedTabSplitterItem">TabSplitterItem that contains TopPanelItems</param>
        /// <returns>Selected SplitterPage or new SplitterPage</returns>
        private static SplitterPage GetTopSelectedPage(TabSplitterItem selectedTabSplitterItem)
        {
            SplitterPage topSelectedPage = selectedTabSplitterItem.TopPanelItems.SelectedItem;

            if (topSelectedPage == null)
            {
                topSelectedPage = new SplitterPage();
            }

            return topSelectedPage;
        }

        /// <summary>
        ///  Gets selected page of the bottom items. If SelectedItem=null than creates new SplitterPage.
        /// </summary>
        /// <param name="selectedTabSplitterItem">TabSplitterItem that contains BottomPanelItems</param>
        /// <returns>Selected SplitterPage or new SplitterPage</returns>
        private static SplitterPage GetBottomSelectedPage(TabSplitterItem selectedTabSplitterItem)
        {
            SplitterPage bottomSelectedPage = selectedTabSplitterItem.BottomPanelItems.SelectedItem;

            if (bottomSelectedPage == null)
            {
                bottomSelectedPage = new SplitterPage();
            }

            return bottomSelectedPage;
        }

        /// <summary>
        /// Finds the next tab splitter item.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>TabSplitter Item</returns>
        private TabSplitterItem FindNextTabSplitterItem(int startIndex, int direction)
        {
            if (direction != 0)
            {
                int index = startIndex;

                for (int i = 0; i < Items.Count; i++)
                {
                    index += direction;

                    if (index >= Items.Count)
                    {
                        index = 0;
                    }
                    else if (index < 0)
                    {
                        index = Items.Count - 1;
                    }

                    TabSplitterItem item2 = ItemContainerGenerator.ContainerFromIndex(index) as TabSplitterItem;

                    if (((item2 != null) && item2.IsEnabled) && (item2.Visibility == Visibility.Visible))
                    {
                        return item2;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the selected tab splitter item.
        /// </summary>
        /// <returns>TabSplitter Item</returns>
        private TabSplitterItem GetSelectedTabSplitterItem()
        {
            object selectedItem = SelectedItem;

            if (selectedItem == null)
            {
                return null;
            }

            return selectedItem as TabSplitterItem ??
                                   ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TabSplitterItem;
        }

        /// <summary>
        /// Called when [generator status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (HasItems && (SelectedItem == null))
                {
                    SelectedIndex = 0;
                }

                UpdateSelectedContent();
            }
        }

        /// <summary>
        /// Updates the content of the selected.
        /// </summary>
        private void UpdateSelectedContent()
        {
            if (SelectedIndex < 0)
            {
                TopSelectedContent = null;
                BottomSelectedContent = null;
            }
            else
            {
            }
        }

        /// <summary>
        /// Gets the tab splitter item.
        /// </summary>
        /// <param name="item">The item TabSplitterItem.</param>
        /// <returns>TabSplitter Item</returns>
        internal TabSplitterItem GetTabSplitterItem(object item)
        {
            return item is TabSplitterItem
                    ? (TabSplitterItem)item
                    : (TabSplitterItem)ItemContainerGenerator.ContainerFromItem(item);
        }

        /// <summary>
        /// Selects the fist page.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private static void SelectFistPage(SplitterPagesCollection collection)
        {
            if (collection.Count > 0 && collection.SelectedItem == null)
            {
                collection.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Collapses the selected bottom panel.
        /// </summary>
        internal void CollapseSelectedBottomPanel()
        {
            DoubleAnimation collapseAnim = null;
            DoubleAnimation rotateAnim;
            RotateTransform rotateTransform = new RotateTransform();
            Duration duration = new Duration(GetSelectedTabSplitterItem().RotateDuration);
            TabSplitterItem selectedSplitItem = GetSelectedTabSplitterItem();

            if (selectedSplitItem.IsCollapsedBottomPanel == false)
            {
                rotateAnim = new DoubleAnimation
                {
                    To = 180,
                    Duration = duration,
                };
                collapseAnim = new DoubleAnimation
                {
                    To = 0,
                    From = selectedSplitItem.DesiredHeightInBottomPanel,
                    Duration = duration,
                };
                selectedSplitItem.IsCollapsedBottomPanel = true;
            }
            else
            {
                rotateAnim = new DoubleAnimation
                {
                    From = 180,
                    To = 0,
                    Duration = duration,
                };
                collapseAnim = new DoubleAnimation
                {
                    To = selectedSplitItem.DesiredHeightInBottomPanel,
                    From = 0,
                    Duration = duration,
                };
                selectedSplitItem.IsCollapsedBottomPanel = false;
            }

            BottomPanel.BeginAnimation(LayoutPanel.HeightProperty, collapseAnim);

            rotateTransform.CenterX = m_expand_CollapseButtuon.ActualWidth / 2;
            rotateTransform.CenterY = m_expand_CollapseButtuon.ActualHeight / 2;
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);
            m_expand_CollapseButtuon.RenderTransform = rotateTransform;
        }

		/// <summary>
		/// Measures child elements accordingly constraint size.
		/// </summary>
		/// <param name="constraint">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
		/// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
		protected override Size MeasureOverride(Size constraint)
		{
            if (double.IsInfinity(constraint.Height))
                constraint.Height = MinHeight;

			double constraintHeight = constraint.Height - 22; /*Padding and Thickness*/
            double constraintWidth = constraint.Width - 22; /*Padding and Thickness*/
			double splitterHeight = (m_gridSplitter == null ? 0 : m_gridSplitter.CustomGridSplitterHeight);
			Margin = m_contentPanelInnerBorder == null ? 0 : m_contentPanelInnerBorder.Margin.Top + m_contentPanelInnerBorder.Margin.Bottom;

			double bottomHeight = constraintHeight - Margin - splitterHeight - TopPanelHeight;

			requiredHeight = (constraintHeight - splitterHeight - Margin) / 2;
            requiredWidth = (constraintWidth - splitterHeight - Margin) / 2;

            if (m_collapsedColWidth == 0)
                m_collapsedColWidth = (constraint.Width) / 2;
            if (m_collapsedRowHeight == 0)
                m_collapsedRowHeight = (constraint.Height) / 2;

            if (m_gridSplitter != null && m_gridSplitter.ResizeDirection == GridResizeDirection.Rows)
            {
                if (iscollapse == 0)
                {
                    if (BottomPanelHeightRatio > 0 && BottomPanelHeightRatio < 1)
                    {
                        BottomPanelHeight = constraintHeight * BottomPanelHeightRatio;
                        TopPanelHeight = constraintHeight - BottomPanelHeight - Margin - splitterHeight;
                    }
                    else
                    {
                        TopPanelHeight = BottomPanelHeight = requiredHeight;
                        BottomPanelHeightRatio = 0.5;
                    }
                }
                else
                {
                    BottomPanelHeight = 0.0;
                    TopPanelHeight = constraintHeight - BottomPanelHeight - Margin - splitterHeight;
                }
                
                SetGridRowsHeight();
            }
            else
            {
                if (iscollapse == 0)
                {
                    if (RightPanelWidthRatio > 0 && RightPanelWidthRatio < 1)
                    {
                        RightPanelWidth = constraintWidth * RightPanelWidthRatio;
                        LeftPanelWidth = constraintWidth - RightPanelWidth - Margin;
                    }
                    else
                    {
                        RightPanelWidth = LeftPanelWidth = (constraintWidth - Margin) / 2;
                        RightPanelWidthRatio = 0.5;
                    }
                }
                else
                {
                    RightPanelWidth = 0.0;
                    LeftPanelWidth = constraintWidth - RightPanelWidth - Margin;
                }
                
                SetGridRowsWidth();
            }
			return base.MeasureOverride(constraint);
		}

       
        /// <summary>
        /// Is used to collapse bottom or right panel of the resized grid.
        /// </summary>
        internal void CollapsePanel()
        {
            UpdateOnSelection();
            TabSplitterItem item = GetSelectedTabSplitterItem();

            if (item != null)
            {
                SplitterPagesCollection topItems = item.TopPanelItems;
                SplitterPagesCollection bottomItems = item.BottomPanelItems;

                if (m_gridSplitter != null && m_gridSplitter.ResizeDirection == GridResizeDirection.Rows)
                {
                    if (BottomPanel.ActualHeight > 0 && iscollapse==0)
                    {
                        MoveBottomPagesToTop(item, topItems, bottomItems, Dock.Bottom);
                        CollapseHeight();
                        iscollapse = 1;
                    }
                    else
                    {
                        RestoreBottomPages(item, topItems, bottomItems);
                        ExpandHeight(0);
                        iscollapse = 0;
                    }

                    SetGridRowsHeight();
                }
                else
                {
                    if (BottomPanel.ActualWidth > 0 && iscollapse == 0)
                    {
                        MoveBottomPagesToTop(item, topItems, bottomItems, Dock.Left);
                        CollapseWidth();
                        iscollapse = 1;
                    }
                    else
                    {
                        RestoreBottomPages(item, topItems, bottomItems);
                        ExpandWidth(0);
                        iscollapse = 0;
                    }

                    SetGridRowsWidth();
                }
                if(m_gridSplitter!=null)
                    m_gridSplitter.InvalidateArrange();
                UpdateCollapseButtonState();
            }
        }
        internal void collapseBottomPanel()
        {
            UpdateOnSelection();
            TabSplitterItem item = GetSelectedTabSplitterItem();

            if (item != null)
            {
                SplitterPagesCollection topItems = item.TopPanelItems;
                SplitterPagesCollection bottomItems = item.BottomPanelItems;

                if (m_gridSplitter != null && m_gridSplitter.ResizeDirection == GridResizeDirection.Rows)
                {
                    if (item.IsCollapsedBottomPanel == true)
                    {
                        //if (BottomPanel.ActualHeight > 0 || iscollapse != 1)
                        if (iscollapse != 1 || BottomPanel.ActualWidth < 0)
                        {                           
                            MoveBottomPagesToTop(item, topItems, bottomItems, Dock.Bottom);
                            CollapseHeight();
                            iscollapse = 1;
                            noChange = 0;
                        }
                        else
                        {
                            noChange = 1;
                        }
                    }
                    else
                    {
                        //if (BottomPanel.ActualHeight <= 0 || iscollapse ==1)
                        if (iscollapse == 1)

                        {
                            if (BottomPanel.ActualHeight <= 0)
                            {
                                RestoreBottomPages(item, topItems, bottomItems);
                                ExpandHeight(0);
                                iscollapse = 0;
                                noChange = 0;
                            }
                        }
                        else
                        {
                            noChange = 1;
                        }
                    }

                    SetGridRowsHeight();
                }
                else
                {
                    if (item.IsCollapsedBottomPanel == true)
                    {
                        //if (BottomPanel.ActualWidth > 0 || iscollapse != 1)
                            if (iscollapse != 1)
                        {
                            MoveBottomPagesToTop(item, topItems, bottomItems, Dock.Left);
                            CollapseWidth();
                            iscollapse = 1;
                            noChange = 0;
                        }
                        else
                            {
                            noChange = 1;
                        }
                    }
                    else
                    {
                        //if (BottomPanel.ActualWidth <= 0 || iscollapse == 1)
                        if (iscollapse == 1)
                        {
                            if (BottomPanel.ActualWidth <= 0)
                            {
                                RestoreBottomPages(item, topItems, bottomItems);
                                ExpandWidth(0);
                                iscollapse = 0;
                                noChange = 0;
                            }
                        }
                        else
                        {
                            noChange = 1;
                        }
                    }

                    SetGridRowsWidth();
                }
                if (m_gridSplitter != null)
                m_gridSplitter.InvalidateArrange();
            }
        }
        /// <summary>
        /// Restores bottom pages from top pages when expanding of the splitter done.
        /// </summary>
        /// <param name="item">Selected splitter item</param>
        /// <param name="topItems">Top pages collection</param>
        /// <param name="bottomItems">Bottom pages collection</param>
        internal void RestoreBottomPages(TabSplitterItem item, SplitterPagesCollection topItems, SplitterPagesCollection bottomItems)
        {
            SplitterPagesCollection bottomPages = item.BottomPages;

            if (bottomPages != null)
            {
                while (bottomPages.Count != 0)
                {
                    SplitterPage bottomPage = bottomPages[0];
                    bottomPage.ClearValue(SplitterPage.TabStripPlacementPropertyKey);

                    bottomPages.Remove(bottomPage);
                    topItems.Remove(bottomPage);

                    if (bottomPage.IsSelected || bottomPage.IsSelectedPage)
                    {
                        topItems.Remove(bottomPage);
                        var topPage = topItems[0];
                        topItems.Remove(topPage);
                        SplitterPage.SetTabStripPlacement(bottomPage,Dock.Bottom);
                        SplitterPage.SetTabStripPlacement(topPage, Dock.Top);
                        UpdateSkinToContent(topPage);
                        UpdateSkinToContent(bottomPage);
                        bottomItems.Add(topPage);
                        topItems.Add(bottomPage);
                    }
                    else
                    {
                        UpdateSkinToContent(bottomPage);
                        bottomItems.Add(bottomPage);
                    }
                }
            }
        }

        private void UpdateSkinToContent(SplitterPage page)
        {
            if (SkinStorage.GetVisualStyle(this as DependencyObject).ToString() != SkinStorage.GetVisualStyle(page as DependencyObject))
            {
                SkinStorage.SetVisualStyle(page, SkinStorage.GetVisualStyle(this as DependencyObject));
            }
            if (page.Content != null && page.Content is DependencyObject && SkinStorage.GetVisualStyle(page.Content as DependencyObject) != SkinStorage.GetVisualStyle(page))
            {
                SkinStorage.SetVisualStyle(page.Content as DependencyObject, SkinStorage.GetVisualStyle(page).ToString());
            }
        }

        /// <summary>
        /// Moves bottom pages to top pages collection when collapsing of the splitter done.
        /// </summary>
        /// <param name="item">Selected splitter item</param>
        /// <param name="topItems">Top pages collection</param>
        /// <param name="bottomItems">Bottom pages collection</param>
        /// <param name="placement">The placement.</param>
        internal void MoveBottomPagesToTop(TabSplitterItem item, SplitterPagesCollection topItems, SplitterPagesCollection bottomItems, Dock placement)
        {
            item.BottomPages = new SplitterPagesCollection(item);

            while (bottomItems.Count != 0)
            {
                SplitterPage bottomPage = bottomItems[0];
                bottomPage.ClearValue(SplitterPage.TabStripPlacementPropertyKey);
                item.BottomPages.Add(bottomPage);
                Object tempContent = bottomPage;
                object temcon = bottomPage.Content;
                bottomPage.Content = null;
                bottomItems.Remove(bottomPage);

                bottomPage = tempContent as SplitterPage;
                bottomPage.Content = temcon;
                if (bottomPage != null && bottomPage.Content != null)
                {
                    UpdateSkinToContent(bottomPage);
                }
                SplitterPage.SetTabStripPlacement(bottomPage, placement);

                topItems.Insert(0, bottomPage);
                var topSelectedPage = topItems[topItems.Count - 1];
                var selectedTabSplitterItem = GetSelectedTabSplitterItem();

                bottomPage.IsSelectedPage = bottomPage.IsSelected = false;
                topItems[topItems.Count - 1].SelectThisPage();
                TopSelectedContent = topSelectedPage.Content;
            }
        }

        /// <summary>
        /// Sets width to the grid of the  splitter.
        /// </summary>
        internal void SetGridRowsWidth()
        {
            if (!double.IsInfinity(LeftPanelWidth) && LeftPanelWidth >= 0 && m_gridCols[0] != null)
                m_gridCols[0].Width = new GridLength(LeftPanelWidth);
            if (!double.IsInfinity(RightPanelWidth) && RightPanelWidth >= 0 && m_gridCols[2] != null)
                m_gridCols[2].Width = new GridLength(RightPanelWidth);
        }

        /// <summary>
        /// Sets width to the LeftPanel and RightPanel in vertical splitter.
        /// </summary>
        /// <param name="offset">The offset.</param>
        internal void ExpandWidth(double offset)
        {
            RightPanelWidth = m_collapsedColWidth;
            LeftPanelWidth = TopPanel.ActualWidth - m_collapsedColWidth - offset;
        }

        /// <summary>
        /// Sets height to the height to the grid of the splitter.
        /// </summary>
        internal void SetGridRowsHeight()
        {
            if (!double.IsInfinity(TopPanelHeight) && TopPanelHeight >= 0) 
            m_gridRows[0].Height = new GridLength(TopPanelHeight);
            if(!double.IsInfinity(BottomPanelHeight) && BottomPanelHeight>=0)
            m_gridRows[2].Height = new GridLength(BottomPanelHeight);
        }

        /// <summary>
        /// Sets height to the TopPanel and BottomPanel in horizontal splitter.
        /// </summary>
        /// <param name="offset">The offset.</param>
        internal void ExpandHeight(double offset)
        {
            BottomPanelHeight = m_collapsedRowHeight;
            TopPanelHeight = TopPanel.ActualHeight - m_collapsedRowHeight - offset;
        }

        /// <summary>
        /// Sets width of the 2nd column (of the resized grid) to 0.
        /// </summary>
        internal void CollapseColWidth()
        {
            if (TopPanel.ActualWidth > 0)
            {
                CollapseWidth();
            }
        }

        /// <summary>
        /// Sets width of the 2nd column (of the resized grid) to 0.
        /// </summary>
        internal void CollapseWidth()
        {
            m_collapsedColWidth = BottomPanel.ActualWidth;
            RightPanelWidth = 0;
            LeftPanelWidth = m_collapsedColWidth + TopPanel.ActualWidth;
        }

        /// <summary>
        /// Sets height of the 2nd row (of the resized grid) to 0.
        /// </summary>
        internal void CollapseRowHeight()
        {
            if (BottomPanel.ActualHeight > 0)
            {
                CollapseHeight();
            }

            m_gridRows[0].Height = new GridLength(TopPanelHeight);
            m_gridRows[2].Height = new GridLength(BottomPanelHeight);
        }

        /// <summary>
        /// Sets height of the 2nd row (of the resized grid) to 0.
        /// </summary>
        internal void CollapseHeight()
        {
            m_collapsedRowHeight = BottomPanel.ActualHeight;
            BottomPanelHeight = 0;
            if (bottomPanelHeight == -1)
                TopPanelHeight = m_collapsedRowHeight + TopPanel.ActualHeight;
            else
                TopPanelHeight = 2 *requiredHeight;
        }

        /// <summary>
        /// Updates the visible bottom panel.
        /// </summary>
        /// <param name="item">The item TabSplitterItem.</param>
        private void UpdateSelectedBottomPanel(TabSplitterItem item)
        {
            if (!Double.IsNaN(item.DesiredHeightInBottomPanel))
            {
                DoubleAnimation collapseAnim = null;
                DoubleAnimation rotateAnim;
                RotateTransform rotateTransform = new RotateTransform();
                Duration duration = new Duration(TimeSpan.FromMilliseconds(0));

                if (item.IsCollapsedBottomPanel == false)
                {
                    rotateAnim = new DoubleAnimation
                    {
                        From = 180,
                        To = 0,
                        Duration = duration,
                    };
                    collapseAnim = new DoubleAnimation
                    {
                        From = 0,
                        To = item.DesiredHeightInBottomPanel,
                        Duration = duration,
                    };
                }
                else
                {
                    rotateAnim = new DoubleAnimation
                    {
                        To = 180,
                        Duration = duration,
                    };
                    rotateTransform.CenterX = m_expand_CollapseButtuon.ActualWidth / 2;
                    rotateTransform.CenterY = m_expand_CollapseButtuon.ActualHeight / 2;

                    collapseAnim = new DoubleAnimation
                    {
                        To = 0,
                        Duration = duration,
                    };
                }

                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);
                m_expand_CollapseButtuon.RenderTransform = rotateTransform;
            }
        }

        /// <summary>
        /// Close TabItem.
        /// </summary>
        /// <param name="item">TabItem that intend to be closed.</param>
        private void CloseTabSplitterItem(UIElement item)
        {
            int itemIndex = ItemContainerGenerator.IndexFromContainer(item);
            item.Visibility = Visibility.Collapsed;

            if (itemIndex == SelectedIndex)
            {
                if (itemIndex < Items.Count + 1)
                {
                    for (int i = itemIndex + 1; i < Items.Count; i++)
                    {
                        if (GetTabSplitterItem(Items[i]).Visibility == Visibility.Visible)
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }
                }

                if (itemIndex == SelectedIndex)
                {
                    for (int i = itemIndex - 1; i >= 0; i--)
                    {
                        if (GetTabSplitterItem(Items[i]).Visibility == Visibility.Visible)
                        {
                            SelectedIndex = i;
                            break;
                        }
                    }
                }

                if (itemIndex == SelectedIndex)
                {
                    SelectedIndex = -1;
                    TabPanel.Visibility = Visibility.Hidden;
                    BorderContentPanel.Visibility = Visibility.Hidden;
                }
            }
        }


        
        /// <summary>
        /// Called when [hide header on single child changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHideHeaderOnSingleChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.UpdateChildLayout();
        }

        /// <summary>
        /// Updates the child layout.
        /// </summary>
        private void UpdateChildLayout()
        {
            if (Items.Count > 0)
            {
                if (Items[0] is TabSplitterItem)
                {
                    foreach (TabSplitterItem item in Items)
                    {
                        item.InvalidateMeasure();
                        item.InvalidateArrange();
                    }
                }
                else
                {
                    foreach (object obj in Items)
                    {
                        TabSplitterItem item = GetTabSplitterItem(obj);
                        if (item != null)
                        {
                            item.InvalidateMeasure();
                            item.InvalidateArrange();
                        }
                    }
                }
            }
        }
        #endregion

    

        #region Command handlers
        /// <summary>
        /// Processes the close current tab splitter item command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessCloseCurrentTabSplitterItemCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                CloseTabSplitterItem(GetSelectedTabSplitterItem());
            }
        }

        /// <summary>
        /// Processes the open context menu.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessOpenContextMenu(object sender, ExecutedRoutedEventArgs e)
        {
            ResourceDictionary dictionary = new ResourceDictionary
            {
                Source =
                    new Uri(
                    "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/CommonResources/TabItemContextMenu.xaml",
                    UriKind.RelativeOrAbsolute)
            };
            Style contextMenuStyle = (Style)dictionary["CustomContextMenu"];
            ContextMenu contextMenu = new ContextMenu
            {
                Style = contextMenuStyle,
                FlowDirection = FlowDirection
            };
            string skin = SkinStorage.GetVisualStyle(this);
            if (skin == "Office2007Blue")
            {

                contextMenuStyle = (Style)dictionary["Office2007BlueCustomContextMenu"];
            }
            else if (skin == "Office2007Silver")
            {
                contextMenuStyle = (Style)dictionary["Office2007SilverCustomContextMenu"];
            }
            else if (skin == "Office2007Black")
            {
                contextMenuStyle = (Style)dictionary["Office2007BlackCustomContextMenu"];
            }
            else if (skin == "Office2010Blue")
            {
                contextMenuStyle = (Style)dictionary["Office2010BlueCustomContextMenu"];
            }
            else if (skin == "Office2010Silver")
            {
                contextMenuStyle = (Style)dictionary["Office2010SilverCustomContextMenu"];
            }
            else if (skin == "Office2010Black")
            {
                contextMenuStyle = (Style)dictionary["Office2010BlackCustomContextMenu"];
            }
            else if (skin == "Office2003")
            {
                contextMenuStyle = (Style)dictionary["Office2003CustomContextMenu"];
            }
            else if (skin == "Blend")
            {
                contextMenuStyle = (Style)dictionary["BlendCustomContextMenu"];
            }
            else if (skin == "SyncOrange")
            {
                contextMenuStyle = (Style)dictionary["OrangeCustomContextMenu"];
            }
            else if (skin == "ShinyRed")
            {
                contextMenuStyle = (Style)dictionary["ShinyRedCustomContextMenu"];
            }
            else if (skin == "ShinyBlue")
            {
                contextMenuStyle = (Style)dictionary["ShinyBlueCustomContextMenu"];
            }
            else if (skin == "Default")
            {

                contextMenuStyle = (Style)dictionary["CustomContextMenu"];
            }
            else if (skin == "VS2010")
            {
                contextMenuStyle = (Style)dictionary["VS2010CustomContextMenu"];
            }
            else if (skin == "Metro")
            {
                contextMenuStyle = (Style)dictionary["MetroCustomContextMenu"];
            }
            else if (skin == "Transparent")
            {
                contextMenuStyle = (Style)dictionary["TransparentCustomContextMenu"];
            }
            contextMenu.Style = contextMenuStyle;
            m_listMenuButton = e.OriginalSource as ToggleButton;
            m_listMenuButton.ContextMenu = contextMenu;
            contextMenu.PlacementTarget = m_listMenuButton;
            m_listMenuButton.PreviewMouseRightButtonDown += new MouseButtonEventHandler(M_ListMenuButton_PreviewMouseRightButtonDown);

            foreach (object element in Items)
            {
                TabSplitterItem item = GetTabSplitterItem(element);

                if (item.Visibility != Visibility.Visible)
                {
                    continue;
                }

                MenuItem newItem = new MenuItem();
                ImageSource imageSource = TabControlExt.GetImage(item);

                if (imageSource != null)
                {
                    Image image = new Image
                    {
                        Source = imageSource
                    };
                    newItem.Icon = image;
                }

                if (TabSplitterListContextMenuItemTemplate != null)
                {
                    newItem.HeaderTemplate = TabSplitterListContextMenuItemTemplate;
                }

                newItem.Header = item.Header;
                newItem.Tag = item;
                newItem.Click += new RoutedEventHandler(OnListContextMenuItem_Click);
                contextMenu.Items.Add(newItem);
            }

            contextMenu.Closed += new RoutedEventHandler(OnListContextMenu_Closed);
            contextMenu.IsOpen = true;
        }

        /// <summary>
        /// Determines whether this instance [can process open context menu] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanProcessOpenContextMenu(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Determines whether this instance [can process close current tab splitter item command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanProcessCloseCurrentTabSplitterItemCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Processes the collapse bottom selected item command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ProcessCollapseBottomSelectedItemCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CollapsePanel();
        }

        private void UpdateCollapseButtonState()
        {
            ToggleButton button = m_expand_CollapseButtuon;
            TabSplitterItem selected = SelectedItem as TabSplitterItem;

            if (selected != null && selected.Orientation == Orientation.Horizontal)
            {
                if (BottomPanelHeight > 0)
                {
                    button.LayoutTransform = new RotateTransform();
                    m_horizontalButton.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    button.LayoutTransform = new RotateTransform(180);

                    m_horizontalButton.BorderBrush = splitterbtnselectedborderbrush;
                }
            }
            else
            {
                if (RightPanelWidth > 0)
                {
                    button.LayoutTransform = new RotateTransform(180);
                    m_verticalButton.BorderBrush = splitterbtnselectedborderbrush;
                }
                else
                {
                    button.LayoutTransform = new RotateTransform(-360);
                    m_verticalButton.BorderBrush = Brushes.Transparent;
                }
            }

        }

        /// <summary>
        /// Determines whether this instance [can process collapse bottom selected item command] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanProcessCollapseBottomSelectedItemCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// Updates property value cache and raises BottomSelectedContentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnBottomSelectedContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BottomSelectedContentChanged != null)
            {
                BottomSelectedContentChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises TopSelectedContentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTopSelectedContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TopSelectedContentChanged != null)
            {
                TopSelectedContentChanged(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="presenter"></param>
        /// <param name="splitterPage"></param>
        internal void CorrectMaxMinSize(ScrollContentPresenter presenter, SplitterPage splitterPage)
        {
            if (splitterPage != null && presenter != null)
            {
                presenter.MaxHeight = splitterPage.MaxHeight;
                presenter.MaxWidth = splitterPage.MaxWidth;
                presenter.MinHeight = splitterPage.MinHeight;
                presenter.MinWidth = splitterPage.MinWidth;
            }
        }

        /// <summary>
        /// Called when [replacement button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnReplacemenButtonClick(object sender, RoutedEventArgs e)
        {
            TabSplitterItem item = GetSelectedTabSplitterItem();

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
                    topPage.Header = topHeader;
                    bottomPage.Header = bottomHeader;
                }
            }

            if (topPage != null && bottomPage != null)
            {
                int topId = topItems.SelectedIndex;
                int bottmId = bottomItems.SelectedIndex;

                topItems.Remove(topPage);
                bottomItems.Remove(bottomPage);

                SplitterPage.SetTabStripPlacement(topPage, Dock.Top);
                SplitterPage.SetTabStripPlacement(bottomPage, Dock.Bottom);

                topItems.Insert(topId, bottomPage);
                bottomItems.Insert(bottmId, topPage);
            }
        }

        /// <summary>
        /// Calls OnTopSelectedContentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTopSelectedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnTopSelectedContentChanged(e);
        }

        /// <summary>
        /// Calls OnBottomSelectedContentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBottomSelectedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnBottomSelectedContentChanged(e);
        }

        /// <summary>
        /// Handles the PreviewMouseRightButtonDown event of the m_ListMenuButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void M_ListMenuButton_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_listMenuButton.ContextMenu = null;
        }

        /// <summary>
        /// Called when [tab splitter context menu item template changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabSplitterListContextMenuItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnTabSplitterListContextMenuItemTemplateChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabSplitterListContextMenuItemTemplateChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabSplitterListContextMenuItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabSplitterListContextMenuItemTemplateChanged != null)
            {
                TabSplitterListContextMenuItemTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [list context menu item_ click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Tag is TabSplitterItem)
            {
                TabSplitterItem item = (TabSplitterItem)menuItem.Tag;
                int itemIndex = ItemContainerGenerator.IndexFromContainer(item);

                if (SelectedIndex != itemIndex)
                {
                    SelectedIndex = itemIndex;
                }
            }
        }

        /// <summary>
        /// Called when [list context menu_ closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnListContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            FireBeforeDropDownContextMenuClose();
        }

        /// <summary>
        /// Finds resized grid in the tab splitter template.
        /// </summary>
        /// <returns>resized grid</returns>
        private Grid GetResizedGrid()
        {
            return Template.FindName(C_resizedContentGrid, this) as Grid;
        }

        /// <summary>
        /// Raises RightPanelWidthChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnRightPanelWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RightPanelWidthChanged != null)
            {
                RightPanelWidthChanged(this, e);
            }

            if (SelectedSplitterItem != null)
            {
                SelectedSplitterItem.ItemRightPanelWidth = (double)e.NewValue;
            }

            if (m_gridSplitter != null && this.SelectedSplitterItem != null)
            {
                m_gridSplitter.InvalidateMeasure();
                m_gridSplitter.InvalidateArrange();
            }

            SetExpandCollapseButtonToolTip(e);
        }

        /// <summary>
        /// Raises LeftPanelWidthChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnLeftPanelWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LeftPanelWidthChanged != null)
            {
                LeftPanelWidthChanged(this, e);
            }

            if (SelectedSplitterItem != null)
            {
                SelectedSplitterItem.ItemLeftPanelWidht = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Raises BottomPanelHeightChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnBottomPanelHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BottomPanelHeightChanged != null)
            {
                BottomPanelHeightChanged(this, e);
            }

            if (SelectedSplitterItem != null)
            {
                SelectedSplitterItem.ItemBottomPanelHeight = (double)e.NewValue;                
            }

			if (m_gridSplitter != null && this.SelectedSplitterItem != null)
			{
				m_gridSplitter.InvalidateMeasure();
				m_gridSplitter.InvalidateArrange();
			}

            SetExpandCollapseButtonToolTip(e);
        }

        /// <summary>
        /// Raises TopPanelHeightChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTopPanelHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TopPanelHeightChanged != null)
            {
                TopPanelHeightChanged(this, e);
            }

            if (SelectedSplitterItem != null && (double)e.NewValue >= 0)
            {
                SelectedSplitterItem.ItemTopPanelHeight = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Calls TopPanelHeightChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTopPanelHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnTopPanelHeightChanged(e);
        }

        /// <summary>
        /// Calls BottomPanelHeightChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBottomPanelHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnBottomPanelHeightChanged(e);
        }

        /// <summary>
        /// Calls LeftPanelWidthChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLeftPanelWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnLeftPanelWidthChanged(e);
        }

        /// <summary>
        /// Calls RightPanelWidthChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRightPanelWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnRightPanelWidthChanged(e);
        }

        /// <summary>
        /// Sets tooltip to the expand/collapse button of the custom grid splitter.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void SetExpandCollapseButtonToolTip(DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue > 0)
            {
                if (m_expand_CollapseButtuon != null)
                {
                    m_expand_CollapseButtuon.ToolTip = C_collapseTooltip;
                }
            }
            else
            {
                if (m_expand_CollapseButtuon != null)
                {

                    m_expand_CollapseButtuon.ToolTip = C_expandTooltip;
                }
            }
        }
        #endregion

        #region Event raisers
        /// <summary>
        /// Fires the before drop down context menu open.
        /// </summary>
        protected virtual void FireBeforeDropDownContextMenuOpen()
        {
            if (BeforeDropDownContextMenuOpen != null)
            {
                BeforeDropDownContextMenuOpen(this, new EventArgs());
            }
        }

        /// <summary>
        /// Fires the before drop down context menu close.
        /// </summary>
        protected virtual void FireBeforeDropDownContextMenuClose()
        {
            if (BeforeDropDownContextMenuClose != null)
            {
                BeforeDropDownContextMenuClose(this, new EventArgs());
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when BottomSelectedContent property is changed.
        /// </summary>
        public event PropertyChangedCallback BottomSelectedContentChanged;

        /// <summary>
        /// Event that is raised when TopSelectedContent property is changed.
        /// </summary>
        public event PropertyChangedCallback TopSelectedContentChanged;

        /// <summary>
        /// Event that is raised when TabListContextMenuItemTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback TabSplitterListContextMenuItemTemplateChanged;

        /// <summary>
        /// Occurs when [before drop down context menu open].
        /// </summary>
        public event EventHandler BeforeDropDownContextMenuOpen;

        /// <summary>
        /// Occurs when [before drop down context menu close].
        /// </summary>
        public event EventHandler BeforeDropDownContextMenuClose;

        /// <summary>
        /// Event that is raised when TopPanelHeight property is changed.
        /// </summary>
        internal event PropertyChangedCallback TopPanelHeightChanged;

        /// <summary>
        /// Event that is raised when BottomPanelHeight property is changed.
        /// </summary>
        internal event PropertyChangedCallback BottomPanelHeightChanged;

        /// <summary>
        /// Event that is raised when LeftPanelWidth property is changed.
        /// </summary>
        internal event PropertyChangedCallback LeftPanelWidthChanged;

        /// <summary>
        /// Event that is raised when RightPanelWidth property is changed.
        /// </summary>
        internal event PropertyChangedCallback RightPanelWidthChanged;
        #endregion

        #region Attached DP Getters &  Setters
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>Image Source</returns>
        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageProperty);
        }

        /// <summary>
        /// Sets the value of the Image dependency property.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value.</param>
        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageProperty, value);
        }
        #endregion

        #region Dependency Properies
        /// <summary>
        /// Represents bottom selected content property key.
        /// </summary>
        protected static readonly DependencyPropertyKey BottomSelectedContentPropertyKey = DependencyProperty.RegisterReadOnly("BottomSelectedContent", typeof(object), typeof(TabSplitter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBottomSelectedContentChanged)));

        /// <summary>
        /// Represents top selected content property key.
        /// </summary>
        protected static readonly DependencyPropertyKey TopSelectedContentPropertyKey = DependencyProperty.RegisterReadOnly("TopSelectedContent", typeof(object), typeof(TabSplitter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTopSelectedContentChanged)));

        /// <summary>
        /// Represents bottom selected content property.
        /// </summary>
        public static readonly DependencyProperty BottomSelectedContentProperty = BottomSelectedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents top selected content property.
        /// </summary>
        public static readonly DependencyProperty TopSelectedContentProperty = TopSelectedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the TabSplitter List context menu item template property
        /// </summary>
        public static readonly DependencyProperty TabSplitterListContextMenuItemTemplateProperty = DependencyProperty.Register("TabSplitterListContextMenuItemTemplate", typeof(DataTemplate), typeof(TabSplitter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabSplitterListContextMenuItemTemplateChanged)));

        /// <summary>
        /// Represents the Image Dependency property
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(TabSplitter), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents TopPanelHeight dependency property.
        /// </summary>
        internal static readonly DependencyProperty TopPanelHeightProperty = DependencyProperty.Register("TopPanelHeight", typeof(double), typeof(TabSplitter), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnTopPanelHeightChanged)));

        /// <summary>
        /// Represents BottomPanelHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomPanelHeightProperty = DependencyProperty.Register("BottomPanelHeight", typeof(double), typeof(TabSplitter), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnBottomPanelHeightChanged)));

        /// <summary>
        /// Represents LeftPanelWidth dependency property.
        /// </summary>
        internal static readonly DependencyProperty LeftPanelWidthProperty = DependencyProperty.Register("LeftPanelWidth", typeof(double), typeof(TabSplitter), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnLeftPanelWidthChanged)));

        /// <summary>
        /// Represents RightPanelWidth dependency property.
        /// </summary>
        internal static readonly DependencyProperty RightPanelWidthProperty = DependencyProperty.Register("RightPanelWidth", typeof(double), typeof(TabSplitter), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnRightPanelWidthChanged)));


        /// <summary>
        /// Represents the HideHeaderOnSingleChild Dependency Property
        /// </summary>
        public static readonly DependencyProperty HideHeaderOnSingleChildProperty =
          DependencyProperty.Register("HideHeaderOnSingleChild", typeof(bool), typeof(TabSplitter), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHideHeaderOnSingleChildChanged)));


        /// <summary>
        /// Represents the SplitterDistance from Top
        /// </summary>
        public static readonly DependencyProperty SplitterDistanceFromTopProperty = DependencyProperty.Register("SplitterDistanceFromTop", typeof(double), typeof(TabSplitter), new FrameworkPropertyMetadata(0.0));
        
        #endregion
    }
}
