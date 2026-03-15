// <copyright file="TabSplitterItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the Tab splitter item
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Default,
 Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
 Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
 Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
Type = typeof(TabSplitterItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabSplitter/Themes/TransparentStyle.xaml")]
    public class TabSplitterItem : Control, IDisposable
    {
        #region Enums
        /// <summary>
        /// Presents BoolField
        /// </summary>
        [Flags]
        private enum BoolField
        {
            /// <summary>
            /// Presents DefaultValue
            /// </summary>
            DefaultValue = 0,

            /// <summary>
            /// Presents SetFocusOnContent
            /// </summary>
            SetFocusOnContent = 0x10,

            /// <summary>
            /// Presents SettingFocus
            /// </summary>
            SettingFocus = 0x20
        }
        #endregion

        #region Private members
        /// <summary>
        /// Presents TopPanelItems
        /// </summary>
        private readonly SplitterPagesCollection m_TopPanelItems;

        /// <summary>
        /// Presents BottomPanelItems
        /// </summary>
        private readonly SplitterPagesCollection m_BottomPanelItems;

        /// <summary>
        /// Presents TabItemBoolFieldStore
        /// </summary>
        private BoolField m_tabItemBoolFieldStore;

        /// <summary>
        /// Height of the top panel.
        /// </summary>
        private double m_topPanelHeight = 0;

        /// <summary>
        /// Height of the bottom panel.
        /// </summary>
        private double m_bottomPanelHeight = 0;

        /// <summary>
        /// Contains bottom pages of the current splitter item when bottom panel is collapsed
        /// and all bottom pages are in the top of the splitter.
        /// </summary>
        private SplitterPagesCollection m_bottomPages = null;
        
        /// <summary>
        /// Width of the left panel.
        /// </summary>
        private double m_leftPanelWidth = 0;

        /// <summary>
        /// Width of the right panel.
        /// </summary>
        private double m_rightPanelWidth = 0;

        internal double m_rightpanelwidthratio = 0;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets width of the left panel in the current splitter item.
        /// </summary>
        internal double ItemLeftPanelWidht
        {
            get
            {
                return m_leftPanelWidth;
            }

            set
            {
                m_leftPanelWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets width of the right panel in the current splitter item.
        /// </summary>
        internal double ItemRightPanelWidth
        {
            get
            {
                return m_rightPanelWidth;
            }

            set
            {
                m_rightPanelWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets height of the top panel in the current splitter item.
        /// </summary>
        internal double ItemTopPanelHeight
        {
            get
            {
                return m_topPanelHeight;
            }

            set
            {
                m_topPanelHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets height of the bottom panel in the current splitter item.
        /// </summary>
        internal double ItemBottomPanelHeight
        {
            get
            {
                return m_bottomPanelHeight;
            }

            set
            {
                m_bottomPanelHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets bottom pages of the current splitter item.
        /// </summary>
        internal SplitterPagesCollection BottomPages
        {
            get
            {
                return m_bottomPages;
            }

            set
            {
                m_bottomPages = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the Orientation dependency property.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is collapsed bottom panel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is collapsed bottom panel; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollapsedBottomPanel
        {
            get
            {
                return (bool)GetValue(IsCollapsedBottomPanelProperty);
            }

            set
            {
                SetValue(IsCollapsedBottomPanelProperty, BooleanBoxes.Box(value));
            }
        }

        /// <summary>
        /// Gets or sets the duration of the rotate.
        /// </summary>
        /// <value>The duration of the rotate.</value>
        public TimeSpan RotateDuration
        {
            get
            {
                return (TimeSpan)GetValue(RotateDurationProperty);
            }

            set
            {
                SetValue(RotateDurationProperty, (TimeSpan)value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        [Category("Appearance"), Bindable(true)]
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, BooleanBoxes.Box(value));
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        [Category("Appearance"), Bindable(true)]
        public object Header
        {
            get
            {
                return GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the desired height in bottom panel.
        /// </summary>
        /// <value>The desired height in bottom panel.</value>
        internal double DesiredHeightInBottomPanel
        {
            get
            {
                return (double)GetValue(DesiredHeightInBottomPanelProperty);
            }

            set
            {
                SetValue(DesiredHeightInBottomPanelProperty, (double)value);
            }
        }

        /// <summary>
        /// Gets pages collection the top panel items.
        /// </summary>
        /// <value>The top panel items.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SplitterPagesCollection TopPanelItems
        {
            get
            {
                return m_TopPanelItems;
            }
        }

        /// <summary>
        /// Gets the bottom panel items.
        /// </summary>
        /// <value>The bottom panel items.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SplitterPagesCollection BottomPanelItems
        {
            get
            {
                return m_BottomPanelItems;
            }
        }

        /// <summary>
        /// Gets the tab splitter parent.
        /// </summary>
        /// <value>The tab splitter parent.</value>
        internal TabSplitter TabSplitterParent
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as TabSplitter;
            }
        }

        /// <summary>
        /// Gets or sets the selected page.
        /// </summary>
        /// <value>The selected page.</value>
        internal SplitterPage SelectedPage
        {
            get
            {
                return (SplitterPage)GetValue(SelectedPageProperty);
            }

            set
            {
                SetValue(SelectedPageProperty, (SplitterPage)value);
            }
        }

        /// <summary>
        /// Gets the ResizeDirection of the GridSplitter.
        /// </summary>
        private GridResizeDirection SplitterResizeDirection
        {
            get
            {
                return TabSplitterParent.GridSplitter.ResizeDirection;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="TabSplitterItem"/> class.
        /// </summary>
        static TabSplitterItem()
        {
            EventManager.RegisterClassHandler(typeof(TabSplitterItem), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(TabSplitterItem.OnAccessKeyPressed));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabSplitterItem), new FrameworkPropertyMetadata(typeof(TabSplitterItem)));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TabSplitterItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TabSplitterItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabSplitterItem"/> class.
        /// </summary>
        public TabSplitterItem()
        {
            m_TopPanelItems = InitializeSplitterPageCollection();
            m_BottomPanelItems = InitializeSplitterPageCollection();
            Loaded += new RoutedEventHandler(TabSplitterItem_Loaded);
        }

        /// <summary>
        /// Initializes the splitter page collection.
        /// </summary>
        /// <returns>SplitterPages Collection</returns>
        private SplitterPagesCollection InitializeSplitterPageCollection()
        {
            SplitterPagesCollection pageCollection = new SplitterPagesCollection(this);
            pageCollection.SelectedItemChanged += new SelectedPageChangedHandler(OnPageCollectionSelectedItemChanged);
            return pageCollection;
        }

        /// <summary>
        /// Raises the <see cref="E:PageCollectionSelectedItemChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.SplitterPagesSelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnPageCollectionSelectedItemChanged(SplitterPagesSelectionChangedEventArgs e)
        {
            if (SplitterPagesSelectionChanged != null)
            {
                RaiseSplitterPagesSelectionChangedEvent(this, e.OldSelectedPage, e.NewSelectedPage);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the focus.
        /// </summary>
        /// <returns>bool flag value</returns>
        internal bool SetFocus()
        {
            bool flag = false;

            if (!GetBoolField(BoolField.SettingFocus))
            {
                TabSplitterItem focusedElement = Keyboard.FocusedElement as TabSplitterItem;
                bool flag2 = ((focusedElement == this) || (focusedElement == null)) || (focusedElement.TabSplitterParent != TabSplitterParent);
                SetBoolField(BoolField.SettingFocus, true);
                SetBoolField(BoolField.SetFocusOnContent, flag2);

                try
                {
                    flag = Focus() || flag2;
                }
                finally
                {
                    SetBoolField(BoolField.SettingFocus, false);
                    SetBoolField(BoolField.SetFocusOnContent, false);
                }
            }

            return flag;
        }

        /// <summary>
        /// Raises the <see cref="E:Selected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(true, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Unselected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(false, e);
        }

        /// <summary>
        /// Handles the is selected changed.
        /// </summary>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleIsSelectedChanged(bool newValue, RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Sets the bool field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void SetBoolField(BoolField field, bool value)
        {
            if (value)
            {
                m_tabItemBoolFieldStore |= field;
            }
            else
            {
                m_tabItemBoolFieldStore &= ~field;
            }
        }

        /// <summary>
        /// Gets the bool field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns> bool DefaultValue</returns>
        private bool GetBoolField(BoolField field)
        {
            return (m_tabItemBoolFieldStore & field) != BoolField.DefaultValue;
        }

        /// <summary>
        /// Called when [access key pressed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.AccessKeyPressedEventArgs"/> instance containing the event data.</param>
        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && (e.Scope == null))
            {
                TabSplitterItem item = (TabSplitterItem)sender;

                if (e.Target == null)
                {
                    e.Target = item;
                }
                else if (!item.IsSelected)
                {
                    e.Scope = item;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitterItem container = (TabSplitterItem)d;
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }
        }

        /// <summary>
        /// Raises OrientationChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != OrientationChanged)
            {
                OrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitterItem instance = (TabSplitterItem)d;
            instance.OnOrientationChanged(e);
        }

        private static void OnIsCollapsedBottomPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitterItem instance = (TabSplitterItem)d;
            instance.OnIsCollapsedBottomPanelChanged(e);
        }

        protected virtual void OnIsCollapsedBottomPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            IsCollapsePanel();
            if (null != IsCollapsedBottomPanelChanged)
            {
                IsCollapsedBottomPanelChanged(this, e);
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        /// <summary>
        /// Called when [desired height in bottom panel changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredHeightInBottomPanelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedPageChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedPageChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedPageChanged != null)
            {
                SelectedPageChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [selected page changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (null != e.OldValue)
            {
                SplitterPage instance = (SplitterPage)e.OldValue;
                instance.IsSelectedPage = false;
            }

            if (null != e.NewValue)
            {
                SplitterPage instance = (SplitterPage)e.NewValue;
                instance.IsSelectedPage = true;
            }
        }

        /// <summary>
        /// Collapses the tab splitter bottom panel.
        /// </summary>
        internal void CollapseTabSplitterBottomPanel()
        {
            if (IsCollapsedBottomPanel == false && (
                (TabSplitterParent.BottomPanelHeight > 0 && SplitterResizeDirection == GridResizeDirection.Rows) ||
                (TabSplitterParent.RightPanelWidth > 0 && SplitterResizeDirection == GridResizeDirection.Columns)))
            {
                TabSplitterItem item = (TabSplitterItem)TabSplitterParent.SelectedItem;

                if (SplitterResizeDirection == GridResizeDirection.Rows)
                {
                    TabSplitterParent.MoveBottomPagesToTop(item, item.TopPanelItems, item.BottomPanelItems, Dock.Bottom);
                    TabSplitterParent.CollapseRowHeight();
                }
                else
                {
                    TabSplitterParent.MoveBottomPagesToTop(item, item.TopPanelItems, item.BottomPanelItems, Dock.Bottom);
                    TabSplitterParent.CollapseColWidth();
                }

                TabSplitterParent.GridSplitter.InvalidateArrange();
            }
        }


         protected override Size MeasureOverride(Size constraint)
        {
            if (TabSplitterParent != null)
            {
                if (TabSplitterParent.HideHeaderOnSingleChild)
                {
                    if (CheckItemsVisibility())
                    {
                        SetTemplate(Visibility.Collapsed);
                    }
                    else
                    {
                        SetTemplate(Visibility.Visible);
                    }
                }
                else
                {
                    SetTemplate(Visibility.Visible);
                }
            }   
            return base.MeasureOverride(constraint);
        }

        internal void SetTemplate(Visibility visibility)
        {
            if (this.Template != null)
            {
                FrameworkElement bd = this.Template.FindName("Bd", this) as FrameworkElement;

                if (bd != null)
                {
                    bd.Visibility = visibility;
                }
            }
        }

        /// <summary>
        /// Checks the items visibility.
        /// </summary>
        /// <returns></returns>
        private bool CheckItemsVisibility()
        {
            int count = 0;
            if (TabSplitterParent.Items.Count > 0)
            {
                if (TabSplitterParent.Items[0] is TabSplitterItem)
                {
                    foreach (TabSplitterItem item in TabSplitterParent.Items)
                    {
                        if (item.Visibility == Visibility.Visible)
                        {
                            count++;
                        }
                        if (count > 1)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    foreach (object obj in TabSplitterParent.Items)
                    {
                        TabSplitterItem item = TabSplitterParent.GetTabSplitterItem(obj);
                        if (item != null)
                        {
                            if (item.Visibility == Visibility.Visible)
                            {
                                count++;
                            }
                            if (count > 1)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

    

        #region Events
        /// <summary>
        /// SplitterPagesSelectionChanged Routed Event
        /// </summary>
        public event SplitterPagesSelectionChangedEventHandler SplitterPagesSelectionChanged;

        /// <summary>
        /// Event that is raised when Orientation property is changed.
        /// </summary>
        public event PropertyChangedCallback OrientationChanged;

        public event PropertyChangedCallback IsCollapsedBottomPanelChanged;


        /// <summary>
        /// Occurs when [selected page changed].
        /// </summary>
        public event PropertyChangedCallback SelectedPageChanged;

        #endregion

        #region Event raisers
        /// <summary>
        /// A static helper method to raise the SplitterPagesSelectionChanged event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <param name="oldSelectedPage">The old selected page.</param>
        /// <param name="newSelectedPage">The new selected page.</param>
        /// <returns>SplitterPagesSelection ChangedEventArgs</returns>
        private static SplitterPagesSelectionChangedEventArgs RaiseSplitterPagesSelectionChangedEvent(UIElement target, SplitterPage oldSelectedPage, SplitterPage newSelectedPage)
        {
            if (target == null)
            {
                return null;
            }

            SplitterPagesSelectionChangedEventArgs args = new SplitterPagesSelectionChangedEventArgs(oldSelectedPage, newSelectedPage);
            TabSplitterItem instance = (TabSplitterItem)target;
            instance.SplitterPagesSelectionChanged(instance, args);
            return args;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_TopPanelItems.SelectedItemChanged -= new SelectedPageChangedHandler(OnPageCollectionSelectedItemChanged);
            m_BottomPanelItems.SelectedItemChanged -= new SelectedPageChangedHandler(OnPageCollectionSelectedItemChanged);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Handles the Loaded event of the TabSplitterItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TabSplitterItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (TabSplitterParent != null && TabSplitterParent.BottomPanel != null)
            {
                DesiredHeightInBottomPanel = TabSplitterParent.BottomPanel.ActualHeight;
                SelectedPage = (SplitterPage)m_BottomPanelItems.SelectedItem;

                IsCollapsePanel();
            }

        }

        private void IsCollapsePanel()
        {
            TabSplitterParent.collapseBottomPanel();

            ToggleButton button = TabSplitterParent.m_expand_CollapseButtuon;
            TabSplitterItem selected = TabSplitterParent.SelectedItem as TabSplitterItem;
            if (button != null)
            {
                if (selected != null && selected.Orientation == Orientation.Horizontal)
                {
                    if (TabSplitterParent.noChange == 1)
                    {

                    }
                    else
                    {
                        if (TabSplitterParent.BottomPanel.Height > 0)
                        {
                            button.LayoutTransform = new RotateTransform(180);
                            TabSplitterParent.m_horizontalButton.BorderBrush = Brushes.Transparent;
                        }
                        else if (TabSplitterParent.BottomPanelHeight == 0)
                        {
                        }
                        else
                        {
                            button.LayoutTransform = new RotateTransform(-360);
                            if (this.TabSplitterParent != null)
                                TabSplitterParent.m_horizontalButton.BorderBrush = this.TabSplitterParent.splitterbtnselectedborderbrush;
                        }

                    }
                }
                else
                {
                    if (TabSplitterParent.noChange == 1)
                    {
                    }
                    else
                    {
                        if (TabSplitterParent.RightPanelWidth > 0)
                        {
                            button.LayoutTransform = new RotateTransform(180);
                            if(TabSplitterParent!=null)
                            TabSplitterParent.m_verticalButton.BorderBrush = this.TabSplitterParent.splitterbtnselectedborderbrush;
                        }
                        else
                        {
                            button.LayoutTransform = new RotateTransform(-360);
                            TabSplitterParent.m_verticalButton.BorderBrush = Brushes.Transparent;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Provides class handling for when an access key that is meaningful for this element is invoked.
        /// </summary>
        /// <param name="e">The event data to the access key event. The event data reports which key was invoked, and indicate whether the <see cref="T:System.Windows.Input.AccessKeyManager"/> object that controls the sending of these events also sent this access key invocation to other elements.</param>
        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            Focus();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (((e.Source == this) || !IsSelected) && SetFocus())
            {
                e.Handled = true;
            }

            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewGotKeyboardFocus"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewGotKeyboardFocus(e);

            if ((!e.Handled && (e.NewFocus == this)) && (!IsSelected && (TabSplitterParent != null)))
            {
                IsSelected = true;

                if (TabSplitterParent.SelectedItem != null)
                {
                    ((TabSplitterItem)TabSplitterParent.SelectedItem).IsSelected = false;
                    TabSplitterParent.SelectedItem = this;
                }

                if (e.OldFocus != Keyboard.FocusedElement)
                {
                    e.Handled = true;
                }
                else if (GetBoolField(BoolField.SetFocusOnContent))
                {
                    TabSplitterParent.UpdateLayout();
                }
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies IsSelected dependency property of the TabSplitterItem.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(TabSplitterItem), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(TabSplitterItem.OnIsSelectedChanged)));

        /// <summary>
        /// Identifies Header dependency property of the TabSplitterItem.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = HeaderedContentControl.HeaderProperty.AddOwner(typeof(TabSplitterItem), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies Orientation dependency property of the TabSplitterItem.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(TabSplitterItem), new FrameworkPropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Identifies IsCollapsedBottomPanel dependency property of the TabSplitterItem.
        /// </summary>
        public static readonly DependencyProperty IsCollapsedBottomPanelProperty = DependencyProperty.Register("IsCollapsedBottomPanel", typeof(bool), typeof(TabSplitterItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCollapsedBottomPanelChanged)));

        /// <summary>
        /// Identifies RotateDurationProperty dependency property of the TabSplitterItem.
        /// </summary>
        public static readonly DependencyProperty RotateDurationProperty = DependencyProperty.Register("RotateDuration", typeof(TimeSpan), typeof(TabSplitterItem), new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(80), null));

        /// <summary>
        /// Identifies DesiredHeightInBottomPanel dependency property of the TabSplitterItem.
        /// </summary>
        public static readonly DependencyProperty DesiredHeightInBottomPanelProperty = DependencyProperty.Register("DesiredHeightInBottomPanel", typeof(double), typeof(TabSplitterItem), new FrameworkPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnDesiredHeightInBottomPanelChanged)));

        /// <summary>
        /// Identifies the SelectedPage Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectedPageProperty = DependencyProperty.Register("SelectedPage", typeof(SplitterPage), typeof(TabSplitterItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedPageChanged)));

        #endregion
    }
}
